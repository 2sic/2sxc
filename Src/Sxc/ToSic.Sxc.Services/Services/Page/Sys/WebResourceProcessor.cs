using System.Text.RegularExpressions;
using ToSic.Lib.Services;
using ToSic.Sxc.Data.Sys;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.HtmlParsing;
using ToSic.Sxc.Web.Sys.WebResources;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Page.Sys;

internal class WebResourceProcessor(IFeaturesService features, string? cdnSource, ILog parentLog)
    : HelperBase(parentLog, "Sxc.WebRHl")
{
    #region Constants


    private const string WebResHtmlField = "Html";

    private const string WebResEnabledField = "Enabled";

    private const string WebResAutoOptimizeField = "AutoOptimize";

    #endregion

    #region Constructor

    public string? CdnSource { get; } = cdnSource;

    #endregion

    public PageFeatureFromSettings? Process(string key, DynamicEntity webRes)
    {
        var l = Log.Fn<PageFeatureFromSettings>(key);

        // Check if it's enabled
        if (webRes.Get(WebResEnabledField) as bool? == false)
            return l.ReturnNull("not enabled");

        // Check if we really have HTML to use
        if (webRes.Get(WebResHtmlField) is not string html || html.IsEmpty())
            return l.ReturnNull("no html");

        // TODO: HANDLE AUTO-ENABLE-OPTIMIZATIONS
        var autoOptimize = webRes.Get(WebResAutoOptimizeField, fallback: false);

        if (!CdnSource.HasValue() || CdnSource == WebResourceConstants.CdnDefault)
            return l.Return(new() { NameId = key, Html = html, AutoOptimize = autoOptimize }, "ok, using built-in cdn-path");

        // check if feature is enabled
        if (!features.IsEnabled(SxcFeatures.CdnSourcePublic.NameId))
            return l.Return(new() { NameId = key, Html = html, AutoOptimize = autoOptimize }, "ok, cdn-swap feature not enabled");

        // Set new root based on CdnSource settings
        var newRoot = CdnSource + WebResourceConstants.VersionSuffix;

        // Replacements will be delayed until preparing to generate the final HTML
        // to be sure we only replace things in the url.
        var srcMatches = RegexUtil.ScriptSrcDetectionMultiLine.Matches(html)
            .Cast<Match>()
            .Concat(RegexUtil.StyleDetectionMultiLine.Matches(html).Cast<Match>());
        foreach (var match in srcMatches)
        {
            var orig = match.Groups[RegexUtil.SrcKey].Value;
            var thirdSlash = orig.GetNthIndex('/', 3); // all paths start with "https://xxx/" - which we want to truncate
            var updated = thirdSlash == -1
                ? "error-path-should-have-at-least-3-slashes"
                : newRoot + orig
                    .Substring(thirdSlash)
                    .Replace("@", "-"); // not underscore, because this fails on github cdn where folders starting with underscore are hidden?
            html = html.Replace(orig, updated);
        }

        // When going local, drop integrity property because ATM DNN changes it, and we would need to ensure it's not changed
        // TODO: ideally we only do this, if we don't have another CDN - or make it optional... ? - where would the setting be?
        var integrityMatches = RegexUtil.IntegrityAttribute.Matches(html);
        foreach (Match match in integrityMatches)
        {
            var orig = match.Groups[RegexUtil.IntegrityKey].Value;
            html = html.Replace(orig, "");
        }

        return l.Return(new() { NameId = key, Html = html, AutoOptimize = autoOptimize }, $"ok; root now {newRoot}");
    }

}