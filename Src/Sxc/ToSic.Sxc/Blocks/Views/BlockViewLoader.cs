using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.CmsSys;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// This contains the logic to decide which view a block will have
    /// Basically the one which is configured, or a replacement based on the url
    /// </summary>
    internal class BlockViewLoader: HelperBase
    {
        public BlockViewLoader(ILog parentLog) : base(parentLog, "Blk.ViewLd") { }

        internal IView PickView(IBlock block, IView configView, IContextOfBlock context, AppViews views)
        {
            //View = configView;
            // skip on ContentApp (not a feature there) or if not relevant or not yet initialized
            if (block.IsContentApp || block.App == null) return configView;

            // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
            var viewFromUrlParam = TryGetViewBasedOnUrlParams(context, views);
            return viewFromUrlParam ?? configView;
        }

        private IView TryGetViewBasedOnUrlParams(IContextOfBlock context, AppViews views)
        {
            var wrapLog = Log.Fn<IView>("template override - check");
            if (context.Page.Parameters == null) return wrapLog.ReturnNull("no params");

            var urlParameterDict = context.Page.Parameters.ToDictionary(pair => pair.Key?.ToLowerInvariant() ?? "", pair =>
                $"{pair.Key}/{pair.Value}".ToLowerInvariant());

            var allTemplates = views.GetAll();

            foreach (var template in allTemplates.Where(t => !string.IsNullOrEmpty(t.UrlIdentifier)))
            {
                var desiredFullViewName = template.UrlIdentifier.ToLowerInvariant();
                if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                {
                    var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
                    if (urlParameterDict.ContainsKey(keyName))
                        return wrapLog.Return(template, "template override - found:" + template.Name);
                }
                else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
                    return wrapLog.Return(template, "template override - found:" + template.Name);
            }

            return wrapLog.ReturnNull("template override - none");
        }

    }
}
