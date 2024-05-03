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

        var allTemplates = views.GetAll();
        l.A($"all templates: {allTemplates.Count}");

        foreach (var template in allTemplates.Where(t => !string.IsNullOrEmpty(t.UrlIdentifier)))
        {
            var desiredFullViewName = template.UrlIdentifier.ToLowerInvariant();
            l.A($"checking: {desiredFullViewName}");

            if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
            {
                var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
                l.A($"checking: {keyName}");

                if (urlParameterDict.ContainsKey(keyName))
                    return l.Return(template, "template override - found:" + template.Name);
            }
            else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
                return l.Return(template, "template override - found:" + template.Name);
        }

        return l.ReturnNull("template override - none");
    }

}