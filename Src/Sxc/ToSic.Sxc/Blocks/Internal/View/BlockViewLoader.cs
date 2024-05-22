using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// This contains the logic to decide which view a block will have
/// Basically the one which is configured, or a replacement based on the url
/// </summary>
internal class BlockViewLoader(ILog parentLog) : HelperBase(parentLog, "Blk.ViewLd")
{
    internal IView PickView(IBlock block, IView configView, IContextOfBlock context, WorkViews views)
    {
        //View = configView;
        // skip on ContentApp (not a feature there) or if not relevant or not yet initialized
        if (block.IsContentApp || block.App == null) return configView;

        // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
        var viewFromUrlParam = TryGetViewBasedOnUrlParams(context, views);
        return viewFromUrlParam ?? configView;
    }


    private IView TryGetViewBasedOnUrlParams(IContextOfBlock context, WorkViews views)
    {
        var l = Log.Fn<IView>("template override - check");
        if (context.Page.Parameters == null) return l.ReturnNull("no params");

        var urlParameterString = string.Join(", ", context.Page.Parameters.Select(pair => $"'{pair.Key}'='{pair.Value}'"));
        l.A($"url params: {context.Page.Parameters.Count} - {urlParameterString}");

        var urlParameterDict = context.Page.Parameters.ToDictionary(pair => pair.Key?.ToLowerInvariant() ?? "", pair =>
            $"{pair.Key}/{pair.Value}".ToLowerInvariant());

        // Get all views which can switch, and exit early if there are none
        var templatesWithUrlIdentifier = views.GetForViewSwitch();
        if (templatesWithUrlIdentifier.Count == 0)
            return l.ReturnNull("no templates with view-switch");

        // check all views if they apply here
        foreach (var set in templatesWithUrlIdentifier)
        {
            l.A($"checking: '{set.UrlIdentifier}'; IsRegex: {set.IsRegex}; MainKey: '{set.MainKey}'");
            var foundMatch = set.IsRegex
                ? urlParameterDict.ContainsKey(set.MainKey)         // match details/.*
                : urlParameterDict.ContainsValue(set.MainKey);      // match view/details
            if (foundMatch)
            {
                var originalWithoutServices = set.View;
                var finalView = views.Recreate(originalWithoutServices);
                return l.Return(finalView, "template override: " + set.Name);
            }
        }

        return l.ReturnNull("template override: none");
    }
}