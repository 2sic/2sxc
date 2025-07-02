using ToSic.Sxc.Apps.Sys.Work;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Blocks.Sys;

/// <summary>
/// This contains the logic to decide which view a block will have
/// Basically the one which is configured, or a replacement based on the url
/// </summary>
internal class BlockViewLoader(ILog parentLog) : HelperBase(parentLog, "Blk.ViewLd")
{
    internal IView? PickView(BlockSpecs block, IView? configView, WorkViews views)
    {
        // #1 skip on ContentApp (not a feature there) or if not relevant or not yet initialized
        if (block.IsContentApp || !block.DataIsReady)
            return configView;

        // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
        var viewFromUrlParam = TryGetViewBasedOnUrlParams(block.Context, views);
        return viewFromUrlParam ?? configView;
    }


    private IView? TryGetViewBasedOnUrlParams(IContextOfBlock context, WorkViews views)
    {
        var l = Log.Fn<IView>("template override - check");

        // Get the parameters from the page, and exit early if there are none
        var parameters = context.Page.Parameters;
        if (parameters == null || parameters.Count == 0)
            return l.ReturnNull("no params");

        // Get all views which can switch, and exit early if there are none
        var templatesWithUrlIdentifier = views.GetForViewSwitch();
        if (templatesWithUrlIdentifier.Count == 0)
            return l.ReturnNull("no templates with view-switch");

        // Log url parameters and convert to dictionary for use below
        var urlParameterString = string.Join(", ", parameters.Select(pair => $"'{pair.Key}'='{pair.Value}'"));
        l.A($"url params: {parameters.Count} - {urlParameterString}");

        var urlParameterDict = parameters
            .ToDictionary(
                pair => pair.Key?.ToLowerInvariant() ?? "",
                pair => $"{pair.Key}/{pair.Value}".ToLowerInvariant()
            );

        // check all views if they apply here
        foreach (var (finalView, name, urlIdentifier, isRegex, mainKey) in templatesWithUrlIdentifier)
        {
            l.A($"checking: '{urlIdentifier}'; IsRegex: {isRegex}; MainKey: '{mainKey}'");
            var foundMatch = isRegex
                ? urlParameterDict.ContainsKey(mainKey)         // match details/.*
                : urlParameterDict.ContainsValue(mainKey);      // match view/details

            if (!foundMatch) continue;

            //var originalWithoutServices = set.View;
            //var finalView = views.Recreate(originalWithoutServices);
            return l.Return(finalView, "template override: " + name);
        }

        return l.ReturnNull("template override: none");
    }
}