using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Code;
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
        /// Get a Root CMS Block if you know the TabId and the ModId
        /// </summary>
        /// <param name="tabId">The DNN tab id (page id)</param>
        /// <param name="modId">The DNN Module id</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static IBlockBuilder CmsBlock(int tabId, int modId)
        {
            var moduleInfo = new ModuleController().GetModule(modId, tabId, false);
            var instance = Eav.Factory.StaticBuild<DnnContainer>().Init(moduleInfo, null);
            return CmsBlock(instance);
        }

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object
        /// </summary>
        /// <param name="moduleInfo">A DNN ModuleInfo object</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static IBlockBuilder CmsBlock(ModuleInfo moduleInfo)
            => CmsBlock(Eav.Factory.StaticBuild<DnnContainer>().Init(moduleInfo, null));

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static IBlockBuilder CmsBlock(IContainer container, ILog parentLog = null)
        {
            var dnnModule = ((Container<ModuleInfo>)container).UnwrappedContents;
            var tenant = new DnnSite(new PortalSettings(dnnModule.OwnerPortalID));
            return Eav.Factory.StaticBuild<BlockFromModule>()
                .Init(DnnContext.Create(tenant, container, new DnnUser(), Eav.Factory.GetServiceProvider()), parentLog).BlockBuilder;
        }

        /// <summary>
        /// Retrieve a helper object which provides commands like AsDynamic, AsEntity etc.
        /// </summary>
        /// <param name="blockBuilder">CMS Block for which the helper is targeted. </param>
        /// <returns>A Code Helper based on <see cref="IDnnDynamicCode"/></returns>
        public static IDnnDynamicCode DynamicCode(IBlockBuilder blockBuilder) => new DnnDynamicCodeRoot().Init(blockBuilder.Block, null);

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
            => App(Eav.Apps.App.AutoLookupZone, appId, null, showDrafts, parentLog);

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
            => App(Eav.Apps.App.AutoLookupZone, appId, new DnnSite(ownerPortalSettings), showDrafts, parentLog);

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
            var app = Eav.Factory.StaticBuild<App>();
            if (site != null) app.PreInit(site);
            var appStuff = app.Init(new AppIdentity(zoneId, appId), 
                Eav.Factory.StaticBuild<AppConfigDelegate>().Init(parentLog).Build(showDrafts, new LookUpEngine(parentLog)),
                true, parentLog);
            return appStuff;
        }

    }
}