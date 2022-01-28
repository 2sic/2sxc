using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;


namespace ToSic.Sxc.Dnn.Run
{
    public static class ContextOfBlockExtensions
    {
        public static IContextOfBlock Init(this IContextOfBlock context, ModuleInfo dnnModule, ILog parentLog)
        {
            context.Init(parentLog);
            var log = context.Log;
            var wrapLog = log.Call<IContextOfBlock>(message: "static init");
            log.Add($"Will try-swap module info of {dnnModule.ModuleID} into site");
            ((DnnSite)context.Site).TrySwap(dnnModule, log);
            log.Add("Will init module");
            ((DnnModule)context.Module).Init(dnnModule, log);
            return wrapLog(null, InitPageOnly(context));
        }

        public static IContextOfBlock InitPageOnly(this IContextOfBlock context)
        {
            var log = context.Log;
            var wrapLog = log.Call<IContextOfBlock>();
            // Collect / assemble page information
            var activeTab = (context.Site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;
            context.Page.Init(activeTab?.TabID ?? Eav.Constants.NullId);

            // the FullUrl will throw an error in DNN search scenarios
            string url = null;
            try
            {
                // skip during search (usual HttpContext is missing for search)
                if (System.Web.HttpContext.Current == null) return wrapLog("no http-context, can't add page", context);

                url = activeTab?.FullUrl.TrimLastSlash();
                ((Page)context.Page).Url = url;
            }
            catch
            {
                /* ignore */
            }

            return wrapLog(url, context);
        }
    }
}
