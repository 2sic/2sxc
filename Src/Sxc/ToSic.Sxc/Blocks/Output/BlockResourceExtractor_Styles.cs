using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks.Output
{
    public abstract partial class BlockResourceExtractor
    {
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


    }
}
