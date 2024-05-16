using System.Text.RegularExpressions;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.HtmlParsing;
using static ToSic.Sxc.Web.Internal.ClientAssets.ClientAssetConstants;

namespace ToSic.Sxc.Blocks.Internal;

public abstract partial class BlockResourceExtractor
{
    protected string ExtractExternalScripts(string renderedTemplate, ref bool include2SxcJs, ClientAssetsExtractSettings settings)
    {
        var l = Log.Fn<string>();

        var scriptMatches = RegexUtil.ScriptSrcDetection.Value.Matches(renderedTemplate);
        var scriptMatchesToRemove = new List<Match>();

        Log.A($"Found {scriptMatches.Count} external scripts");
        foreach (Match match in scriptMatches.Cast<Match>())
        {
            var url = FixUrlWithSpaces(match.Groups[RegexUtil.SrcKey].Value);

            // always remove 2sxc JS requests from template and ensure it's added the standard way
            if (Is2SxcApiJs(url))
            {
                include2SxcJs = true;
                scriptMatchesToRemove.Add(match);
                continue;
            }

            // Also get the ID (new in v12)
            var idMatches = RegexUtil.IdDetection.Value.Match(match.Value);
            var id = idMatches.Success ? idMatches.Groups["Id"].Value : null;

            // todo: ATM the priority and type is only detected in the Regex which expects "enable-optimizations"
            // ...so to improve this code, we would have to use 2 regexs - one for detecting "enable-optimizations" 
            // ...and another for the priority etc.

            // skip if not matched and setting only wants matches
            var jsSettings = settings.Js;
            var (skip, posInPage, priority) = CheckOptimizationSettings(match.Value, jsSettings);
            if (skip) continue;

            // get all Attributes
            var (attributes, cspCode) = GetHtmlAttributes(match.Value);
            var forCsp = cspCode == pageServiceShared.CspEphemeralMarker;
            if (jsSettings.AutoDefer && !attributes.ContainsKey(AttributeDefer)) attributes[AttributeDefer] = null;
            if (jsSettings.AutoAsync && !attributes.ContainsKey(AttributeAsync)) attributes[AttributeAsync] = null;

            // Register, then add to remove-queue
            Assets.Add(new ClientAsset { Id = id, IsJs = true, PosInPage = posInPage, Priority = priority, Url = url, HtmlAttributes = attributes, WhitelistInCsp = forCsp });
            scriptMatchesToRemove.Add(match);
        }

        // remove in reverse order, so that the indexes don't change as we remove scripts in the HTML
        scriptMatchesToRemove.Reverse();
        scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
        return l.Return(renderedTemplate);
    }

    private (bool Skip, string PosInPage, int Priority) CheckOptimizationSettings(string value, ClientAssetExtractSettings settings)
    {
        // Check if we have the optimize attribute
        var optMatch = RegexUtil.OptimizeDetection.Value.Match(value);
        if (!optMatch.Success && !settings.ExtractAll) return (true, settings.Location, settings.Priority);

        var finalPosInPage = (optMatch.Groups[RegexUtil.PositionKey]?.Value).UseFallbackIfNoValue(settings.Location);

        var priority = GetPriority(optMatch, settings.Priority);

        // Return as skip if priority is not known
        return (priority <= 0, finalPosInPage, priority); // don't register/remove if not within specs
    }

    private int GetPriority(Match optMatch, int defaultPriority)
    {
        var priorityString = (optMatch.Groups[RegexUtil.PriorityKey]?.Value ?? "true").ToLowerInvariant();
        var priority = priorityString == "true" || priorityString == ""
            ? defaultPriority
            : int.Parse(priorityString);
        return priority;
    }


    protected string ExtractInlineScripts(string renderedTemplate) => Log.Func(() =>
    {
        var scriptMatches = RegexUtil.ScriptContentDetection.Value.Matches(renderedTemplate);
        var scriptMatchesToRemove = new List<Match>();

        Log.A($"Found {scriptMatches.Count} inline scripts");
        var order = 1000;
        foreach (Match match in scriptMatches)
        {
            // Register, then add to remove-queue
            Assets.Add(new ClientAsset
            {
                IsJs = true, Priority = order++, PosInPage = "inline", Content = match.Groups["Content"]?.Value,
                IsExternal = false
            });
            scriptMatchesToRemove.Add(match);
        }

        // remove in reverse order, so that the indexes don't change
        scriptMatchesToRemove.Reverse();
        scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
        return renderedTemplate;
    });

}