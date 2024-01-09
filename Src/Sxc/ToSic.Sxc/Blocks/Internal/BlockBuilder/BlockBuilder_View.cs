namespace ToSic.Sxc.Blocks.Internal;

public partial class BlockBuilder
{

    //internal IView SetTemplateOrOverrideFromUrl(IView configView)
    //{
    //    //View = configView;
    //    // skip on ContentApp (not a feature there) or if not relevant or not yet initialized
    //    if (Block.IsContentApp || Block.App == null) return configView;

    //    // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
    //    var viewFromUrlParam = TryToGetTemplateBasedOnUrlParams();
    //    return viewFromUrlParam ?? configView;
    //}

    //private IView TryToGetTemplateBasedOnUrlParams()
    //{
    //    var wrapLog = Log.Call<IView>("template override - check");
    //    if (Context.Page.Parameters == null) return wrapLog("no params", null);

    //    var urlParameterDict = Context.Page.Parameters.ToDictionary(pair => pair.Key?.ToLowerInvariant() ?? "", pair =>
    //        $"{pair.Key}/{pair.Value}".ToLowerInvariant());

    //    var allTemplates = new CmsRuntime(App, Log, UserMayEdit, false).Views.GetAll();

    //    foreach (var template in allTemplates.Where(t => !string.IsNullOrEmpty(t.UrlIdentifier)))
    //    {
    //        var desiredFullViewName = template.UrlIdentifier.ToLowerInvariant();
    //        if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
    //        {
    //            var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
    //            if (urlParameterDict.ContainsKey(keyName))
    //                return wrapLog("template override - found:" + template.Name, template);
    //        }
    //        else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
    //            return wrapLog("template override - found:" + template.Name, template);
    //    }

    //    return wrapLog("template override - none", null);
    //}


}