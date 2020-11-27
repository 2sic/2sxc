using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;


namespace ToSic.Sxc.Dnn.Run
{
    public static class ContextOfBlockExtensions
    {
        public static IContextOfBlock Init(this IContextOfBlock context, ModuleInfo dnnModule, ILog parentLog)
        {
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
                ((SxcPage)context.Page).Url = activeTab?.FullUrl;
            }
            catch
            {
                /* ignore */
            }

            return context;
        }
    }
}
