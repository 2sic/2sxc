using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Helpers
{
    public class HtmlHelper
    {

        //public string Link(string id) => $"<link id=\"{id}\" rel=\"shortcut icon\" type=\"image/{favicontype}\" href=\"{favicon}\" />\n";

        //public static string SetMetaTag(string id, string name, string content)
        //{
        //    var rez = "<meta";
        //    if (!string.IsNullOrEmpty(id)) rez += $" id=\"{WebUtility.HtmlEncode(id)}\"";
        //    if (!string.IsNullOrEmpty(name)) rez += $" name=\"{WebUtility.HtmlEncode(name)}\"";
        //    if (!string.IsNullOrEmpty(content)) rez += $" content=\"{WebUtility.HtmlEncode(content)}\"";
        //    rez += " />\n";
        //    return rez;
        //}

        //public static string GetAttribute(string element, string attribute) 
        //    => XElement.Parse(element).Attribute(attribute)?.Value;


        //public void AddHeadContent(SiteState siteState, string content)
        //{
        //    if (!string.IsNullOrEmpty(content))
        //    {
        //        // format html content, remove scripts, and filter duplicate elements
        //        var elements = (">" + content.Replace("\n", "") + "<").Split("><");
        //        foreach (var element in elements)
        //        {
        //            if (!string.IsNullOrEmpty(element) && !element.ToLower().StartsWith("script"))
        //            {
        //                if (!siteState.Properties.HeadContent.Contains("<" + element + ">"))
        //                {
        //                    siteState.Properties.HeadContent += "<" + element + ">" + "\n";
        //                }
        //            }
        //        }
        //    }
        //}

        //public void GetHeadContent(SiteState siteState, string attribute)
        //{
        //    if (!string.IsNullOrEmpty(content))
        //    {
        //        // format html content, remove scripts, and filter duplicate elements
        //        var elements = (">" + content.Replace("\n", "") + "<").Split("><");
        //        foreach (var element in elements)
        //        {
        //            if (!string.IsNullOrEmpty(element) && !element.ToLower().StartsWith("script"))
        //            {
        //                if (!siteState.Properties.HeadContent.Contains("<" + element + ">"))
        //                {
        //                    siteState.Properties.HeadContent += "<" + element + ">" + "\n";
        //                }
        //            }
        //        }
        //    }
        //}

        public static string GetMetaTagContent(string html, string name, bool decode = true)
        {
            if (string.IsNullOrEmpty(html) || string.IsNullOrEmpty(name))
                return null;

            // Define a regex pattern to match the meta tag
            var pattern = $@"<meta\s+name\s*=\s*[""']{WebUtility.HtmlEncode(name)}[""']\s+content\s*=\s*[""'](.*?)[""']\s*/?>";

            // Use the regex to find a match in the provided HTML content
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

            // If a match is found, return the value of the content attribute
            return match.Success ? (decode ? WebUtility.HtmlDecode(match.Groups[1].Value) : match.Groups[1].Value) :
                // If no match is found, return null
                null;
        }

        public static string AddOrUpdateMetaTagContent(string html, string name, string content, bool encode = true)
        {
            if (html == null || string.IsNullOrEmpty(name)) return html;

            // Define a regex pattern to match the meta tag
            var pattern = $@"(<meta\s+name\s*=\s*[""']{WebUtility.HtmlEncode(name)}[""']\s+content\s*=\s*[""'])(.*?)([""']\s*/?>)";

            // If the meta tag exists, replace its content
            if (Regex.IsMatch(html, pattern, RegexOptions.IgnoreCase))
                return Regex.Replace(html, pattern, $"$1{(encode ? WebUtility.HtmlEncode(content) : content)}$3", RegexOptions.IgnoreCase);

            // If the meta tag doesn't exist, add it
            return html + $"{Environment.NewLine}<meta name=\"{WebUtility.HtmlEncode(name)}\" content=\"{(encode ? WebUtility.HtmlEncode(content) : content)}\">";
        }


        public static string UpdateProperty(string original, OqtPagePropertyChanges change, IOqtHybridLog page)
        {
            var logPrefix = $"{nameof(UpdateProperty)}(original:{original}) - ";

            if (string.IsNullOrEmpty(original))
            {
                var result = change.Value ?? original;
                page?.Log($"{logPrefix}is empty, UpdateTitle:{result}");
                return result;
            };

            // 1. Check if we have a replacement token - if yes, try to replace it
            if (!string.IsNullOrEmpty(change.Placeholder))
            {
                var pos = original.IndexOf(change.Placeholder, StringComparison.InvariantCultureIgnoreCase);
                if (pos >= 0)
                {
                    var suffixPos = pos + change.Placeholder.Length;
                    var suffix = suffixPos < original.Length ? original.Substring(suffixPos) : "";
                    var result2 = original.Substring(0, pos) + change.Value + suffix;
                    page?.Log($"{logPrefix}token replaced, UpdateTitle:{result2}");
                    return result2;
                }

                page?.Log($"{logPrefix}replace token not found, UpdateTitle:{original}");
                if (change.Change == OqtPagePropertyOperation.ReplaceOrSkip) return original;
            }

            // 2. If not, try to prefix / suffix / replace depending on the property
            var result3 = change.Change switch
            {
                OqtPagePropertyOperation.Replace => change.Value ?? original,
                OqtPagePropertyOperation.Suffix => $"{original}{change.Value}",
                OqtPagePropertyOperation.Prefix => $"{change.Value}{original}",
                OqtPagePropertyOperation.ReplaceOrSkip => original,
                _ => throw new ArgumentOutOfRangeException()
            };
            page?.Log($"{logPrefix}{change.Change}, UpdateTitle:{result3}");
            return result3;
        }

        public static void UpdatePageProperties(SiteState siteState, OqtViewResultsDto viewResults, ModuleProBase page)
        {
            var logPrefix = $"{nameof(UpdatePageProperties)}(...) - ";

            // Go through Page Properties
            foreach (var p in viewResults.PageProperties)
            {
                switch (p.Property)
                {
                    case OqtPageProperties.Title:
                        var currentTitle = siteState.Properties.PageTitle;
                        var updatedTitle = UpdateProperty(currentTitle, p.InjectOriginalInValue(currentTitle), page);
                        page?.Log($"{logPrefix}UpdateTitle:", updatedTitle);
                        siteState.Properties.PageTitle = updatedTitle;
                        break;
                    case OqtPageProperties.Keywords:
                        var currentKeywords = GetMetaTagContent(siteState.Properties.HeadContent, "KEYWORDS");
                        var updatedKeywords = UpdateProperty(currentKeywords, p.InjectOriginalInValue(currentKeywords), page);
                        page?.Log($"{logPrefix}Keywords:", updatedKeywords);
                        siteState.Properties.HeadContent = AddOrUpdateMetaTagContent(siteState.Properties.HeadContent, "KEYWORDS", updatedKeywords);
                        break;
                    case OqtPageProperties.Description:
                        var currentDescription = GetMetaTagContent(siteState.Properties.HeadContent, "DESCRIPTION");
                        var updatedDescription = UpdateProperty(currentDescription, p.InjectOriginalInValue(currentDescription), page);
                        page?.Log($"{logPrefix}Description:", updatedDescription);
                        siteState.Properties.HeadContent = AddOrUpdateMetaTagContent(siteState.Properties.HeadContent, "DESCRIPTION", updatedDescription);
                        break;
                    case OqtPageProperties.Base:
                        // For base - ignore for now as we don't know what side-effects this could have
                        page?.Log($"{logPrefix}Base ignore for now");
                        break;
                    default:
                        page?.Log($"{logPrefix} ArgumentOutOfRangeException");
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
