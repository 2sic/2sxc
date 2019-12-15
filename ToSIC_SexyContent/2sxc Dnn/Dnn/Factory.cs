using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
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
            var instance = new Container(moduleInfo);
            return CmsBlock(instance);
        }

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object
        /// </summary>
        /// <param name="moduleInfo">A DNN ModuleInfo object</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static ICmsBlock CmsBlock(ModuleInfo moduleInfo)
            => CmsBlock(new Container(moduleInfo));

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        public static ICmsBlock CmsBlock(IContainer container)
        {
            var dnnModule = ((Container<ModuleInfo>)container).Original;
            var tenant = new Tenant(new PortalSettings(dnnModule.OwnerPortalID));
            return new BlockFromModule(container, parentLog: null, tenant: tenant).CmsInstance;
        }

        /// <summary>
        /// Retrieve a helper object which provides commands like AsDynamic, AsEntity etc.
        /// </summary>
        /// <param name="cmsBlock">The CMS Block for which the helper is targeted. </param>
        /// <returns>A Code Helper based on <see cref="IDynamicCode"/></returns>
        public static IDynamicCode CodeHelpers(ICmsBlock cmsBlock)
            => new DynamicCode(cmsBlock as CmsBlock);

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="publishingEnabled">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public static IApp App(int appId, bool publishingEnabled = false, bool showDrafts = false)
            => App(appId, PortalSettings.Current, publishingEnabled, showDrafts);

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
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public static IApp App(int appId, PortalSettings ownerPortalSettings, bool publishingEnabled = false, bool showDrafts = false)
        {
            var appStuff = new App(
                new Tenant(ownerPortalSettings), 
                Eav.Apps.App.AutoLookupZone, appId,
                ConfigurationProvider.Build(showDrafts, publishingEnabled, 
                    new LookUpEngine(null as ILog)), 
                true, null);
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