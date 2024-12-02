using ToSic.Eav;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Internal.EditUi;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HtmlDialog
{
    public const string PageIdInUrl = "pageId";
    public const string WithPublicKey = "wpk";

    public const string BasePlaceholder = "@base";
    public const string CustomBodyPlaceholder = "@custombody";
    public const string CacheBreakPlaceholder = "@sxcver";
    public const string CustomHeadersPlaceholder = "@customheaders";
    public const string JsApiPlaceholder = "@jsapi";

    public static string[] Placeholders =
    [
        BasePlaceholder,
        CacheBreakPlaceholder,
        CustomBodyPlaceholder,
        CustomHeadersPlaceholder,
        JsApiPlaceholder
    ];

    public static string CleanImport(string html)
    {
        if (!html.HasValue()) return html;

        // Placeholders may be wrapped in <!-- --> because of angular compiler - we should first strip these
        foreach (var ph in Placeholders)
            html = html.Replace($"<!--{ph}-->", ph);

        // Correct quotes, as the index-raw will always have double quotes (angular-compiler replaces single quotes)
        html = html.Replace($"\"{JsApiPlaceholder}\"", $"'{JsApiPlaceholder}'");

        return html;
    }

    public static string UpdatePlaceholders(string html, string content, int pageId, string customBaseParams, string customHeaders, string customBody)
    {
        if (!html.HasValue()) return html;

        var result = html;

        // Add context variables
        result = result.Replace(JsApiPlaceholder, content);

        // Add version cachebreak
        result = result.Replace(CacheBreakPlaceholder, EavSystemInfo.VersionWithStartUpBuild);

        // Replace base url
        result = result.Replace(BasePlaceholder, $"./?{PageIdInUrl}={pageId}{customBaseParams}");

        // Custom Headers - Experimental WIP


        // Add any custom headers / body - like for the Oqtane Request Verification Token
        result = result.Replace(CustomHeadersPlaceholder, customHeaders);
        result = result.Replace(CustomBodyPlaceholder, customBody);

        return result;
    }
}