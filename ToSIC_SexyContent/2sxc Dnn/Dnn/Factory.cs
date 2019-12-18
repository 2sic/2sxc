using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
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
    [PublicApi]
    public static class Factory
    {
        /// <summary>
        /// Get a Root CMS Block if you know the TabId and the ModuleId
        /// </summary>
        /// <param name="tabId">The DNN tab id (page id)</param>
        /// <param name="modId">The DNN Module id</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static ICmsBlock CmsBlock(int tabId, int modId)
        {
            var moduleInfo = new ModuleController().GetModule(modId, tabId, false);
            var instance = new DnnContainer(moduleInfo);
            return CmsBlock(instance);
        }

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object
        /// </summary>
        /// <param name="moduleInfo">A DNN ModuleInfo object</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static ICmsBlock CmsBlock(ModuleInfo moduleInfo)
            => CmsBlock(new DnnContainer(moduleInfo));

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static ICmsBlock CmsBlock(IContainer container, ILog parentLog = null)
        {
            var dnnModule = ((Container<ModuleInfo>)container).UnwrappedContents;
            var tenant = new DnnTenant(new PortalSettings(dnnModule.OwnerPortalID));
            return new BlockFromModule(container, parentLog: parentLog, tenant: tenant).CmsInstance;
        }

        /// <summary>
        /// Retrieve a helper object which provides commands like AsDynamic, AsEntity etc.
        /// </summary>
        /// <param name="cmsBlock">The CMS Block for which the helper is targeted. </param>
        /// <returns>A Code Helper based on <see cref="IDnnDynamicCode"/></returns>
        public static IDnnDynamicCode DynamicCode(ICmsBlock cmsBlock)
            => new DnnDynamicCode(cmsBlock as CmsBlock, 10);

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="publishingEnabled">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public static IApp App(int appId, bool publishingEnabled = false, bool showDrafts = false, ILog parentLog = null)
            => App(appId, PortalSettings.Current, publishingEnabled, showDrafts, parentLog);

        ///// <summary>
        ///// Get a full app-object for accessing data of the app from outside
        ///// </summary>
        ///// <param name="zoneId">The zone the app is in.</param>
        ///// <param name="appId">The AppID of the app you need</param>
        ///// <param name="publishingEnabled">
        ///// Tells the App that you'll be using page-publishing.
        ///// So changes will me auto-drafted for a future release as the whole page together.
        ///// </param>
        ///// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        ///// <returns>An initialized App object which you can use to access App.Data</returns>
        //public static IApp App(int zoneId, int appId, bool publishingEnabled = false, bool showDrafts = false)
        //    => App(zoneId, appId, PortalSettings.Current, publishingEnabled, showDrafts);

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="ownerPortalSettings">The owner portal - this is important when retrieving Apps from another portal.</param>
        /// <param name="publishingEnabled">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public static IApp App(int appId, PortalSettings ownerPortalSettings, bool publishingEnabled = false, bool showDrafts = false, ILog parentLog = null)
        {
            var appStuff = new App(
                new DnnTenant(ownerPortalSettings), 
                Eav.Apps.App.AutoLookupZone, appId,
                ConfigurationProvider.Build(showDrafts, publishingEnabled, 
                    new LookUpEngine(parentLog)), 
                true, parentLog);
            return appStuff;
        }

        //public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool publishingEnabled = false, bool showDrafts = false)
        //{
        //    var appStuff = new App(new DnnTenant(ownerPortalSettings), zoneId, appId,
        //        ConfigurationProvider.Build(showDrafts, publishingEnabled, new TokenListFiller()), true, null);
        //    return appStuff;
        //}

    }
}