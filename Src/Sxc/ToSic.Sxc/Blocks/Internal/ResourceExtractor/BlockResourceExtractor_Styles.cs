using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Lib.Logging;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Internal;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.HtmlParsing;

namespace ToSic.Sxc.Blocks.Internal;

public abstract partial class BlockResourceExtractor
{
    protected string ExtractStyles(string renderedTemplate, ClientAssetsExtractSettings settings) => Log.Func(() =>
    {
        var styleMatches = RegexUtil.StyleDetection.Value.Matches(renderedTemplate);
        var styleMatchesToRemove = new List<Match>();

        Log.A($"Found {styleMatches.Count} external styles");
        foreach (Match match in styleMatches)
        {
            // Also get the ID (new in v12)
            var idMatches = RegexUtil.IdDetection.Value.Match(match.Value);
            var id = idMatches.Success ? idMatches.Groups["Id"].Value : null;

            // todo: ATM the priority and type is only detected in the Regex which expects "enable-optimizations"
            // ...so to improve this code, we would have to use 2 regexs - one for detecting "enable-optimizations" 
            // ...and another for the priority etc.

            // skip if not stylesheet
            if (!RegexUtil.StyleRelDetect.Value.IsMatch(match.Value)) continue;

            // skip if not matched and setting only wants matches
            var (skip, posInPage, priority) = CheckOptimizationSettings(match.Value, settings.Css);
            if (skip)
            {
                if (!settings.Css.ExtractAll) continue;
                // if Auto-Optimize, then set these defaults
                posInPage = ClientAssetConstants.AddToBottom;
                priority = ClientAssetConstants.CssDefaultPriority;
            }


            // get all Attributes
            var (_, cspCode) = GetHtmlAttributes(match.Value);
            var forCsp = cspCode == pageServiceShared.CspEphemeralMarker;

            // Register, then remember to remove later on
            var url = FixUrlWithSpaces(match.Groups["Src"].Value);
            Assets.Add(new ClientAsset { Id = id, IsJs = false, PosInPage = posInPage, Priority = priority, Url = url, WhitelistInCsp = forCsp});
            styleMatchesToRemove.Add(match);
        }

        styleMatchesToRemove.Reverse();
        styleMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
        return renderedTemplate;
    });


}