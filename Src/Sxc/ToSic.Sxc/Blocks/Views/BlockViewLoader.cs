using System.Linq;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Blocks.Views
{
    /// <summary>
    /// This contains the logic to decide which view a block will have
    /// Basically the one which is configured, or a replacement based on the url
    /// </summary>
    internal class BlockViewLoader: HasLog
    {
        public BlockViewLoader(ILog parentLog) : base("Blk.ViewLd", parentLog)
        {

        }


        internal IView PickView(IBlock block, IView configView, IContextOfBlock context, CmsRuntime cms)
        {
            //View = configView;
            // skip on ContentApp (not a feature there) or if not relevant or not yet initialized
            if (block.IsContentApp || block.App == null) return configView;

            // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
            var viewFromUrlParam = TryToGetTemplateBasedOnUrlParams(context, cms);
            return viewFromUrlParam ?? configView;
        }

        private IView TryToGetTemplateBasedOnUrlParams(IContextOfBlock context, CmsRuntime cms)
        {
            var wrapLog = Log.Call<IView>("template override - check");
            if (context.Page.ParametersInternalOld == null) return wrapLog("no params", null);

            var urlParameterDict = context.Page.ParametersInternalOld.ToDictionary(pair => pair.Key?.ToLowerInvariant() ?? "", pair =>
                $"{pair.Key}/{pair.Value}".ToLowerInvariant());

            var allTemplates = cms.Views.GetAll();

            foreach (var template in allTemplates.Where(t => !string.IsNullOrEmpty(t.UrlIdentifier)))
            {
                var desiredFullViewName = template.UrlIdentifier.ToLowerInvariant();
                if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                {
                    var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
                    if (urlParameterDict.ContainsKey(keyName))
                        return wrapLog("template override - found:" + template.Name, template);
                }
                else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
                    return wrapLog("template override - found:" + template.Name, template);
            }

            return wrapLog("template override - none", null);
        }

    }
}
