using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Dnn.Run
{
    public static class ContextOfBlockExtensions
    {
        public static IContextOfBlock Init(this IContextOfBlock context, ModuleInfo dnnModule, ILog parentLog)
        {
            context.Init(parentLog);
            ((DnnSite)context.Site).TrySwap(dnnModule);
            ((DnnModule)context.Module).Init(dnnModule, parentLog);
            return InitPageOnly(context);
        }

        public static IContextOfBlock InitPageOnly(this IContextOfBlock context)
        {
            // Collect / assemble page information
            var activeTab = (context.Site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;
            context.Page.Init(activeTab?.TabID ?? Eav.Constants.NullId);

            // the FullUrl will throw an error in DNN search scenarios
            try
            {
                ((Page)context.Page).Url = activeTab?.FullUrl.TrimLastSlash();
            }
            catch
            {
                /* ignore */
            }

            return context;
        }
    }
}
