using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Lib.Logging;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks.Output
{
    public abstract partial class BlockResourceExtractor
    {
        protected string ExtractExternalScripts(string renderedTemplate, ref bool include2SxcJs)
        {
            var wrapLog = Log.Fn<string>();

            var scriptMatches = ScriptSrcDetection.Matches(renderedTemplate);
            var scriptMatchesToRemove = new List<Match>();

            Log.A($"Found {scriptMatches.Count} external scripts");
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

                // todo: ATM the priority and type is only detected in the Regex which expects "enable-optimizations"
                // ...so to improve this code, we would have to use 2 regexs - one for detecting "enable-optimizations" 
                // ...and another for the priority etc.

                // skip if not matched and setting only wants matches
                // bool skip;
                var (skip, posInPage, priority) = CheckOptimizationSettings(match, "body", JsDefaultPriority);
                if (skip) continue;

                // get all Attributes
                var (attributes, cspCode) = GetHtmlAttributes(match.Value);
                var forCsp = cspCode == _pageServiceShared.CspEphemeralMarker;

                // Register, then add to remove-queue
                Assets.Add(new ClientAsset { Id = id, IsJs = true, PosInPage = posInPage, Priority = priority, Url = url, HtmlAttributes = attributes, WhitelistInCsp = forCsp });
                scriptMatchesToRemove.Add(match);
            }

            // remove in reverse order, so that the indexes don't change as we remove scripts in the HTML
            scriptMatchesToRemove.Reverse();
            scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            return wrapLog.Return(renderedTemplate);
        }

        private (bool Skip, string PosInPage, int Priority) CheckOptimizationSettings(Match match, string posInPage, int priority)
        {
            var optMatch = OptimizeDetection.Match(match.Value);
            if (!optMatch.Success && ExtractOnlyEnableOptimization) return (true, null, 0);

            posInPage = optMatch.Groups[TokenPosition]?.Value ?? posInPage;

            priority = GetPriority(optMatch, priority);

            if (priority <= 0) return (true, null, 0); // don't register/remove if not within specs

            return (false, posInPage, priority);
        }


        protected string ExtractInlineScripts(string renderedTemplate)
        {
            var wrapLog = Log.Fn<string>();

            var scriptMatches = ScriptContentDetection.Matches(renderedTemplate);
            var scriptMatchesToRemove = new List<Match>();

            Log.A($"Found {scriptMatches.Count} inline scripts");
            var order = 1000;
            foreach (Match match in scriptMatches)
            {
                // Register, then add to remove-queue
                Assets.Add(new ClientAsset { IsJs = true, Priority = order++, PosInPage = "inline", Content = match.Groups["Content"]?.Value, IsExternal = false });
                scriptMatchesToRemove.Add(match);
            }

            // remove in reverse order, so that the indexes don't change
            scriptMatchesToRemove.Reverse();
            scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            return wrapLog.Return(renderedTemplate);
        }

    }
}
