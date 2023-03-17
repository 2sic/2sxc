using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using ToSic.Sxc.Utils;
using ToSic.Sxc.Web.PageFeatures;
using static ToSic.Sxc.Web.WebResources.WebResourceConstants;

namespace ToSic.Sxc.Web.WebResources
{
    internal class WebResourceProcessor: HelperBase
    {
        public string CdnMode { get; }
        public string AlternateRoot { get; }

        public WebResourceProcessor(string cdnMode, string alternateRoot, ILog parentLog) : base(parentLog, "Sxc.WebRHl")
        {
            CdnMode = cdnMode;
            AlternateRoot = alternateRoot;
        }

        public PageFeatureFromSettings Process(string key, DynamicEntity webRes)
        {
            var l = Log.Fn<PageFeatureFromSettings>(key);

            // Check if it's enabled
            if (webRes.Get(WebResEnabledField) as bool? == false) return l.ReturnNull("not enabled");

            // Check if we really have HTML to use
            if (!(webRes.Get(WebResHtmlField) is string html) || html.IsEmpty()) return l.ReturnNull("no html");

            // TODO: HANDLE AUTO-ENABLE-OPTIMIZATIONS
            var autoOptimize = webRes.Get(WebResAutoOptimizeField, fallback: false);

            if (!CdnMode.HasValue() || CdnMode == CdnDefault)
                return l.Return(new PageFeatureFromSettings(key, html: html, autoOptimize: autoOptimize), "ok, using built-in cdn-path");

            // override temp dev
            var newRoot = PickBestNewRoot();
            var currentResRoot = webRes.Get<string>(WebResRootPathField);
            if (!currentResRoot.HasValue())
                return l.Return(new PageFeatureFromSettings(key, html: html, autoOptimize: autoOptimize), "ok, tried to replace root but original not set");

            // Replacements will be delayed until preparing to generate the final HTML
            // to be sure we only replace things in the url.
            var srcMatches = RegexUtil.ScriptSrcDetectionMultiLine.Matches(html)
                .Cast<Match>()
                .Concat(RegexUtil.StyleDetectionMultiLine.Matches(html).Cast<Match>());
            foreach (var match in srcMatches)
            {
                var orig = match.Groups[RegexUtil.SrcKey].Value;
                var updated = orig
                    .Replace(currentResRoot, newRoot)
                    .Replace("@", "-"); // not underscore, because this fails on github cdn where folders starting with underscore are hidden?
                html = html.Replace(orig, updated);
            }

            // When going local, drop integrity property because ATM DNN changes it, and we would need to ensure it's not changed
            var integrityMatches = RegexUtil.IntegrityAttribute.Matches(html);
            foreach (Match match in integrityMatches)
            {
                var orig = match.Groups[RegexUtil.IntegrityKey].Value;
                html = html.Replace(orig, "");
            }

            return l.Return(new PageFeatureFromSettings(key, html: html, autoOptimize: autoOptimize), 
                $"ok; replace root '{currentResRoot}' with {newRoot}");
        }

        private string PickBestNewRoot()
        {
            switch (CdnMode)
            {
                case Cdn2Sxc: return Cdn2SxcRoot;
                case CdnLocal: return CdnLocalRoot;
                case CdnCustom: return AlternateRoot;
            }

            return "cdn-mode-unknown";
        }

    }
}
