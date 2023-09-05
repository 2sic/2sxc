using System;
using System.Net;
using System.Text.RegularExpressions;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client
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

        public static string AddScript(string html, string src, Alias alias)
        {
            if (string.IsNullOrEmpty(src)) return html;
            var script = CreateScript(src, alias);
            if (!html.Contains(script)) html += script + Environment.NewLine;
            return html;
        }

        private static string CreateScript(string src, Alias alias)
        {
            if (string.IsNullOrEmpty(src)) return null;
            var url = (src.Contains("://")) ? src : alias.BaseUrl + src;
            return "<script src=\"" + url + "\"" + "></script>";
        }

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
            return html + $"<meta name=\"{WebUtility.HtmlEncode(name)}\" content=\"{(encode ? WebUtility.HtmlEncode(content) : content)}\">{Environment.NewLine}";
        }
    }
}
