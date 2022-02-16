using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks.Output
{
    public abstract class BlockResourceExtractor: HasLog<IBlockResourceExtractor>, IBlockResourceExtractor
    {
        #region Construction / DI
        protected BlockResourceExtractor() : base("Sxc.AstOpt") { }

        #endregion

        #region Settings

        /// <summary>
        /// Priority of a CSS - will be used for sorting when added to page
        /// </summary>
        protected int CssDefaultPriority = 99;

        /// <summary>
        /// Priority of a CSS - will be used for sorting when added to page
        /// </summary>
        protected int JsDefaultPriority = 100;

        /// <summary>
        /// Extract only script tags which are marked for extraction
        /// </summary>
        public bool ExtractOnlyEnableOptimization = true;

        #endregion

        /// <summary>
        /// List of extracted assets - this must be processed later by the caller
        /// </summary>
        public List<IClientAsset> Assets { get; }= new List<IClientAsset>();
        
        /// <summary>
        /// Run the sequence to extract assets
        /// </summary>
        /// <param name="renderedTemplate"></param>
        /// <returns></returns>
        public abstract Tuple<string, bool> Process(string renderedTemplate);



        protected string ExtractStyles(string renderedTemplate)
        {
            var wrapLog = Log.Call<string>();
            var styleMatches = StyleDetection.Matches(renderedTemplate);
            var styleMatchesToRemove = new List<Match>();

            Log.Add($"Found {styleMatches.Count} external styles");
            foreach (Match match in styleMatches)
            {
                var posInPage = "head"; // default for styles
                var priority = CssDefaultPriority;

                var optMatch = OptimizeDetection.Match(match.Value);

                // Also get the ID (new in v12)
                var idMatches = IdDetection.Match(match.Value);
                var id = idMatches.Success ? idMatches.Groups["Id"].Value : null;

                // todo: ATM the priority and type is only detected in the Regex which expects "enable-optimizations"
                // ...so to improve this code, we would have to use 2 regexs - one for detecting "enable-optimizations" 
                // ...and another for the priority etc.

                // skip if not matched and setting only wants matches
                if (ExtractOnlyEnableOptimization)
                {
                    if (!optMatch.Success) continue;

                    // skip if not stylesheet
                    if (!StyleRelDetect.IsMatch(match.Value)) continue;

                    posInPage = optMatch.Groups["Position"]?.Value ?? posInPage;

                    priority = GetPriority(optMatch, priority);

                    // don't register/remove if not within specs
                    if (priority <= 0) continue; 
                }

                // Register, then remember to remove later on
                var url = FixUrlWithSpaces(match.Groups["Src"].Value);
                Assets.Add(new ClientAsset { Id = id, IsJs = false, PosInPage = posInPage, Priority = priority, Url = url });
                styleMatchesToRemove.Add(match);
            }

            styleMatchesToRemove.Reverse();
            styleMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            return wrapLog(null, renderedTemplate);
        }

        protected string ExtractExternalScripts(string renderedTemplate, ref bool include2SxcJs)
        {
            var wrapLog = Log.Call<string>();

            var scriptMatches = ScriptSrcDetection.Matches(renderedTemplate);
            var scriptMatchesToRemove = new List<Match>();

            Log.Add($"Found {scriptMatches.Count} external scripts");
            foreach (Match match in scriptMatches)
            {
                var url = FixUrlWithSpaces(match.Groups["Src"].Value);

                // always remove 2sxc JS requests from template and ensure it's added the standard way
                if (Is2SxcApiJs(url))
                {
                    include2SxcJs = true;
                    scriptMatchesToRemove.Add(match);
                    continue;
                }
                
                // Also get the ID (new in v12)
                var idMatches = IdDetection.Match(match.Value);
                var id = idMatches.Success ? idMatches.Groups["Id"].Value : null;

                var providerName = "body";
                var priority = JsDefaultPriority;

                // todo: ATM the priority and type is only detected in the Regex which expects "enable-optimizations"
                // ...so to improve this code, we would have to use 2 regexs - one for detecting "enable-optimizations" 
                // ...and another for the priority etc.

                // skip if not matched and setting only wants matches
                if (ExtractOnlyEnableOptimization)
                {
                    var optMatch = OptimizeDetection.Match(match.Value);
                    if (!optMatch.Success) continue;

                    providerName = optMatch.Groups["Position"]?.Value ?? providerName;

                    priority = GetPriority(optMatch, priority);

                    if (priority <= 0) continue; // don't register/remove if not within specs
                }

                // Register, then add to remove-queue
                Assets.Add(new ClientAsset { Id = id, IsJs = true, PosInPage = providerName, Priority = priority, Url = url });
                scriptMatchesToRemove.Add(match);
            }

            // remove in reverse order, so that the indexes don't change
            scriptMatchesToRemove.Reverse();
            scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            return wrapLog(null, renderedTemplate);
        }


        protected string ExtractInlineScripts(string renderedTemplate)
        {
            var wrapLog = Log.Call<string>();

            var scriptMatches = ScriptContentDetection.Matches(renderedTemplate);
            var scriptMatchesToRemove = new List<Match>();

            Log.Add($"Found {scriptMatches.Count} inline scripts");
            var order = 1000;
            foreach (Match match in scriptMatches)
            {
                // Register, then add to remove-queue
                Assets.Add(new ClientAsset { IsJs = true, Priority = order++, PosInPage = "inline", Content = match.Groups["Content"]?.Value, IsExternal = false});
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
        private static bool Is2SxcApiJs(string url) => url.ToLowerInvariant()
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
            var priority = (optMatch.Groups["Priority"]?.Value ?? "true").ToLowerInvariant();
            var prio = priority == "true" || priority == ""
                ? defValue
                : int.Parse(priority);
            return prio;
        }


        #region RegEx formulas and static compiled RegEx objects (performance)

        private const string ClientDependencyRegex =
            "\\sdata-enableoptimizations=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

        private const string ScriptSrcFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
        private const string ScriptContentFormula = @"<script[^>]*>(?<Content>(.|\n)*?)</script[^>]*>";
        private const string StyleSrcFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/?>)";
        private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";
        private const string IdFormula = "('|\"|\\s)id=('|\")(?<Id>.*?)('|\")";

        internal static readonly Regex ScriptSrcDetection = new Regex(ScriptSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex ScriptContentDetection = new Regex(ScriptContentFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        internal static readonly Regex StyleDetection = new Regex(StyleSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex StyleRelDetect = new Regex(StyleRelFormula, RegexOptions.IgnoreCase);
        internal static readonly Regex OptimizeDetection = new Regex(ClientDependencyRegex, RegexOptions.IgnoreCase);
        internal static readonly Regex IdDetection = new Regex(IdFormula, RegexOptions.IgnoreCase);

        #endregion

    }
}