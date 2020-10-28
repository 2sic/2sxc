using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web
{
    public abstract class ClientDependencyOptimizer: HasLog<IClientDependencyOptimizer>, IClientDependencyOptimizer
    {
        protected ClientDependencyOptimizer() : base("Sxc.AstOpt") { }

        public List<ClientAssetInfo> Assets { get; }= new List<ClientAssetInfo>();

        protected int CssDefaultPriority = 99;
        protected int JsDefaultPriority = 100;

        public abstract Tuple<string, bool> Process(string renderedTemplate);

        #region RegEx formulas and static compiled RegEx objects (performance)

        private const string ClientDependencyRegex =
            "\\sdata-enableoptimizations=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

        private const string ScriptRegExFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
        private const string StyleRegExFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/?>)";
        private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";

        internal static readonly Regex ScriptDetection = new Regex(ScriptRegExFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex StyleDetection = new Regex(StyleRegExFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex StyleRelDetect = new Regex(StyleRelFormula, RegexOptions.IgnoreCase);
        internal static readonly Regex OptimizeDetection = new Regex(ClientDependencyRegex, RegexOptions.IgnoreCase);

        #endregion



        protected string ExtractStyles(string renderedTemplate)
        {
            var wrapLog = Log.Call<string>();
            var styleMatches = StyleDetection.Matches(renderedTemplate);
            var styleMatchesToRemove = new List<Match>();

            foreach (Match match in styleMatches)
            {
                var optMatch = OptimizeDetection.Match(match.Value);
                if (!optMatch.Success)
                    continue;

                // skip If the Rel attribute is not stylesheet
                if (!StyleRelDetect.IsMatch(match.Value))
                    continue;

                var posInPage = optMatch.Groups["Position"]?.Value ?? "head"; //DnnProviderName(optMatch.Groups["Position"]?.Value, "head");

                var prio = GetPriority(optMatch, CssDefaultPriority);

                if (prio <= 0) continue; // don't register/remove if not within specs

                // Register, then remember to remove later on
                var url = FixUrlWithSpaces(match.Groups["Src"].Value);
                Assets.Add(new ClientAssetInfo { IsJs = false, PosInPage = posInPage, Priority = prio, Url = url });
                //ClientResourceManager.RegisterStyleSheet(page, url, prio, providerName);
                styleMatchesToRemove.Add(match);
            }

            styleMatchesToRemove.Reverse();
            styleMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            return wrapLog(null, renderedTemplate);
        }

        protected string ExtractScripts(string renderedTemplate, ref bool include2SxcJs)
        {
            var wrapLog = Log.Call<string>();

            var scriptMatches = ScriptDetection.Matches(renderedTemplate);
            var scriptMatchesToRemove = new List<Match>();

            foreach (Match match in scriptMatches)
            {
                // always remove 2sxc JS requests from template and ensure it's added the standard way
                var url = FixUrlWithSpaces(match.Groups["Src"].Value);
                if (Is2SxcApiJs(url))
                {
                    include2SxcJs = true;
                    scriptMatchesToRemove.Add(match);
                    continue;
                }

                var optMatch = OptimizeDetection.Match(match.Value);
                if (!optMatch.Success) continue;

                var providerName = optMatch.Groups["Position"]?.Value ?? "body"; //DnnProviderName(optMatch.Groups["Position"]?.Value, "body");

                var prio = GetPriority(optMatch, JsDefaultPriority);

                if (prio <= 0) continue; // don't register/remove if not within specs

                // Register, then add to remove-queue
                Assets.Add(new ClientAssetInfo { IsJs = true, PosInPage = providerName, Priority = prio, Url = url });
                //ClientResourceManager.RegisterScript(page, url, prio, providerName);
                scriptMatchesToRemove.Add(match);
            }

            // remove in reverse order, so that the indexes don't change
            scriptMatchesToRemove.Reverse();
            scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            return wrapLog(null, renderedTemplate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// check special case: the 2sxc.api script. only check the first part of the path
        /// because it could be .min, or have versions etc.
        /// </remarks>
        /// <param name="url"></param>
        /// <returns></returns>
        private static bool Is2SxcApiJs(string url) => url.ToLower()
            .Replace("\\", "/")
            .Contains("/js/2sxc.api");

        /// <summary>
        /// Because of an issue with spaces, prepend tilde to urls that start at root
        /// and contain spaces: https://github.com/2sic/2sxc/issues/1566
        /// </summary>
        /// <returns></returns>
        private string FixUrlWithSpaces(string url)
        {
            if (!url.Contains(" ")) return url;
            if (!url.StartsWith("/") || url.StartsWith("//")) return url;
            return "~" + url;
        }

        private int GetPriority(Match optMatch, int defValue)
        {
            var priority = (optMatch.Groups["Priority"]?.Value ?? "true").ToLower();
            var prio = priority == "true" || priority == ""
                ? defValue
                : int.Parse(priority);
            return prio;
        }
    }
}