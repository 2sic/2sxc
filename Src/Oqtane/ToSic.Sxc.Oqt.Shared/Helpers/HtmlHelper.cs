using Oqtane.Models;
using Oqtane.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ToSic.Sxc.Oqt.Shared.Models;
using ResourceLevel = Oqtane.Shared.ResourceLevel;

namespace ToSic.Sxc.Oqt.Shared.Helpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HtmlHelper
{
    private static readonly string Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

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

    //public static string AddScript(string html, string src, Alias alias)
    //{
    //    if (src.IsNullOrEmpty())
    //        return html;
    //    var script = CreateScript(new() { Url = src }, alias);
    //    if (!html.Contains(script))
    //        html += script + Environment.NewLine;
    //    return html;
    //}

    public static string GetMetaTagContent(string html, string name, bool decode = true)
    {
        if (html.IsNullOrEmpty() || name.IsNullOrEmpty())
            return null;
        var pattern = "<meta\\s+name\\s*=\\s*[\"']" + WebUtility.HtmlEncode(name) + "[\"']\\s+content\\s*=\\s*[\"'](.*?)[\"']\\s*/?>";
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
        return match.Success ? (decode ? WebUtility.HtmlDecode(match.Groups[1].Value) : match.Groups[1].Value) : null;
    }

    public static string AddOrUpdateMetaTagContent(string html, string name, string content, bool encode = true)
    {
        if (name.IsNullOrEmpty()) return html;
        html ??= string.Empty;

        // Define a regex pattern to match the meta tag
        var pattern = $@"(<meta\s+name\s*=\s*[""']{WebUtility.HtmlEncode(name)}[""']\s+content\s*=\s*[""'])(.*?)([""']\s*/?>)";

        // If the meta tag exists, replace its content
        if (Regex.IsMatch(html, pattern, RegexOptions.IgnoreCase))
            return Regex.Replace(html, pattern, $"$1{(encode ? WebUtility.HtmlEncode(content) : content)}$3", RegexOptions.IgnoreCase);

        // If the meta tag doesn't exist, add it
        return html + $"<meta name=\"{WebUtility.HtmlEncode(name)}\" content='{(encode ? WebUtility.HtmlEncode(content) : content)}'>{Environment.NewLine}";
    }

    public static string AddHeadChanges(string html, IEnumerable<OqtHeadChange> headChanges)
    {
        if (headChanges == null) return html;
        html ??= string.Empty;

        var str = string.Empty;
        foreach (var headChange in headChanges)
        {
            if (!string.IsNullOrEmpty(headChange.Tag))
                str += headChange.Tag + Environment.NewLine;
        }
        return html + str;
    }

    public static string ManageStyleSheets(string html, OqtViewResultsDto viewResults, Alias alias, string themeName, string pageHtml = "")
    {
        if (viewResults == null) return html;
        html ??= string.Empty;

        var list = viewResults.TemplateResources.Where(r => r.IsExternal && r.ResourceType == ResourceType.Stylesheet)
            .Select(r => r.Url).ToList();
        var count = 0;
        foreach (var url in viewResults.SxcStyles.Union(list))
        {
            var src = url;
            if (src.StartsWith("~"))
                src = src.Replace("~", "/Themes/" + themeName + "/").Replace("//", "/");
            if (!src.Contains("://") && !string.IsNullOrEmpty(alias.BaseUrl) && !src.StartsWith(alias.BaseUrl))
                src = alias.BaseUrl + src;
            if (!html.Contains(src, StringComparison.OrdinalIgnoreCase) && !pageHtml.Contains(src, StringComparison.OrdinalIgnoreCase))
            {
                ++count;
                var assetHtml= "<link"
                    + " id=\"app-stylesheet-" + ResourceLevel.Page.ToString().ToLower() + "-" + Timestamp + "-" + count.ToString("00") + "\""
                    + " rel=\"stylesheet\" href=\"" + src + "\" type=\"text/css\"/>";
                html = AddAssetWhenMissing(html, assetHtml);
            }
        }
        return html;
    }

    public static string ManageScripts(string html, OqtViewResultsDto viewResults, Alias alias, string pageHtml = "")
    {
        if (viewResults == null) return html;
        html ??= string.Empty;

        foreach (var sxcResource in viewResults.TemplateResources.Where(r => r.IsExternal && r.ResourceType == ResourceType.Script))
            html = AddScript(html, sxcResource, alias, pageHtml);
        var count = 0;
        foreach (var sxcScript in viewResults.SxcScripts) 
            html = AddScript(html, new Resource { Url = sxcScript, Reload = false }, alias, pageHtml, ++count);

        return html;
    }

    public static string ManageInlineScripts(string html, OqtViewResultsDto viewResults, Alias alias, string pageHtml = "")
    {
        if (viewResults == null) return html;
        html ??= string.Empty;

        foreach (var sxcResource in viewResults.TemplateResources.Where(r => !r.IsExternal))
            html = AddScript(html, sxcResource, alias, pageHtml);

        return html;
    }

    private static string AddScript(string html, Resource resource, Alias alias, string pageHtml = "", int count = 0)
    {
        if (resource == null || resource.Url.IsNullOrEmpty() && resource.Content.IsNullOrEmpty()) return html;
        html ??= string.Empty;

        var script = CreateScript(resource, alias, count);
        if (!pageHtml.Contains(script, StringComparison.OrdinalIgnoreCase))
            html = AddAssetWhenMissing(html, script);

        return html;
    }

    private static string AddAssetWhenMissing(string html, string assetHtml)
    {
        if (assetHtml.IsNullOrEmpty())  return html;
        html ??= string.Empty;

        if (!html.Contains(assetHtml, StringComparison.OrdinalIgnoreCase))
            html += assetHtml + Environment.NewLine;

        return html;
    }

    private static string CreateScript(Resource resource, Alias alias, int count)
    {
        if (!resource.Content.IsNullOrEmpty())
            return "<script" +
                (resource.ES6Module ? " type=\"module\">" : ">") +
                resource.Content + "</script>";

        // use custom element which can execute script on every page transition
        if (resource.Reload)
            return "<page-script src=\"" + resource.Url + "\"></page-script>";

        var str = resource.Url.Contains("://") ? resource.Url : alias.BaseUrl + resource.Url;
        return "<script" + 
            //" id=\"app-script-" + ResourceLevel.Page.ToString().ToLower() + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + "-" + count.ToString("00") + "\"" +
            (!string.IsNullOrEmpty(resource.Integrity) ? " integrity=\"" + resource.Integrity + "\"" : "") +
            (!string.IsNullOrEmpty(resource.CrossOrigin) ? " crossorigin=\"" + resource.CrossOrigin + "\"" : "") +
            (resource.ES6Module ? " type=\"module\"" : "") +
            " src=\"" + str + "\"></script>";
    }

}