using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is a factory to create CmsBlocks, Apps etc. and related objects from DNN.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public static class Factory
    {
        /// <summary>
        /// Workaround - static build should actually be completely deprecated, but as it's not possible yet,
        /// we'll provide this for now
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T StaticBuild<T>() => DnnStaticDi.GetServiceProvider().Build<T>();

        /// <summary>
        /// Get a Root CMS Block if you know the TabId and the ModId
        /// </summary>
        /// <param name="pageId">The DNN tab id (page id)</param>
        /// <param name="modId">The DNN Module id</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static IBlockBuilder CmsBlock(int pageId, int modId) => CmsBlock(pageId, modId, null);

        /// <summary>
        /// Get a Root CMS Block if you know the TabId and the ModId
        /// </summary>
        /// <param name="pageId">The DNN tab id (page id)</param>
        /// <param name="modId">The DNN Module id</param>
        /// <param name="parentLog">The parent log, optional</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static IBlockBuilder CmsBlock(int pageId, int modId, ILog parentLog)
        {
            var wrapLog = parentLog?.Call($"{pageId}, {modId}");
            var moduleInfo = new ModuleController().GetModule(modId, pageId, false);
            if (moduleInfo == null)
            {
                var msg = $"Can't find module {modId} on page {pageId}. Maybe you reversed the ID-order?";
                parentLog?.Add(msg);
                throw new Exception(msg);
            }
            var container = ((DnnModule)StaticBuild<IModule>()).Init(moduleInfo, parentLog);
            wrapLog?.Invoke("ok");
            return CmsBlock(container, parentLog);
        }

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object
        /// </summary>
        /// <param name="moduleInfo">A DNN ModuleInfo object</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static IBlockBuilder CmsBlock(ModuleInfo moduleInfo)
            => CmsBlock(((DnnModule)StaticBuild<IModule>()).Init(moduleInfo, null));

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static IBlockBuilder CmsBlock(IModule module, ILog parentLog = null)
        {
            var dnnModule = ((Module<ModuleInfo>)module)?.UnwrappedContents;
            var context = StaticBuild<IContextOfBlock>().Init(dnnModule, parentLog);
            return StaticBuild<BlockFromModule>().Init(context, parentLog).BlockBuilder;
        }

        /// <summary>
        /// Retrieve a helper object which provides commands like AsDynamic, AsEntity etc.
        /// </summary>
        /// <param name="blockBuilder">CMS Block for which the helper is targeted. </param>
        /// <returns>A Code Helper based on <see cref="IDnnDynamicCode"/></returns>
        public static IDnnDynamicCode DynamicCode(IBlockBuilder blockBuilder) 
            => StaticBuild<DnnDynamicCodeRoot>().Init(blockBuilder.Block, null, Constants.CompatibilityLevel10) as DnnDynamicCodeRoot;

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="unusedButKeepForApiStability">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public static IApp App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
            => App(AppConstants.AutoLookupZone, appId, null, showDrafts, parentLog);

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="zoneId">The zone the app is in.</param>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="unusedButKeepForApiStability">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public static IApp App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
            => App(zoneId, appId, null, showDrafts, parentLog);

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="ownerPortalSettings">The owner portal - this is important when retrieving Apps from another portal.</param>
        /// <param name="unusedButKeepForApiStability">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public static IApp App(int appId,
            PortalSettings ownerPortalSettings,
            bool unusedButKeepForApiStability = false,
            bool showDrafts = false,
            ILog parentLog = null)
            => App(AppConstants.AutoLookupZone, appId,
                ((DnnSite)StaticBuild<ISite>()).Swap(ownerPortalSettings), showDrafts, parentLog);

        [InternalApi_DoNotUse_MayChangeWithoutNotice]
        private static IApp App(
            int zoneId,
            int appId,
            ISite site,
            bool showDrafts,
            ILog parentLog)
        {
            var log = new Log("Dnn.Factry", parentLog);
            log.Add($"Create App(z:{zoneId}, a:{appId}, tenantObj:{site != null}, showDrafts: {showDrafts}, parentLog: {parentLog != null})");
            var app = StaticBuild<App>();
            if (site != null) app.PreInit(site);
            var appStuff = app.Init(new AppIdentity(zoneId, appId), 
                StaticBuild<AppConfigDelegate>().Init(parentLog).Build(showDrafts),
                parentLog);
            return appStuff;
        }

    }
}