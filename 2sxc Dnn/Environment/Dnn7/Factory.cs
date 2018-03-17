using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.ContentBlocks;
using IApp = ToSic.SexyContent.Interfaces.IApp;

namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// This is a factory to create 2sxc-instance objects and related objects from
    /// non-2sxc environments.
    /// </summary>
    public static class Factory
    {
        public static ISxcInstance SxcInstanceForModule(int modId, int tabId)
        {
            var moduleInfo = new ModuleController().GetModule(modId, tabId, false);
            var instance = new DnnInstanceInfo(moduleInfo);
            return SxcInstanceForModule(instance);
        }

        public static ISxcInstance SxcInstanceForModule(ModuleInfo moduleInfo)
            => SxcInstanceForModule(new DnnInstanceInfo(moduleInfo));

        public static ISxcInstance SxcInstanceForModule(IInstanceInfo moduleInfo)
        {
            var dnnModule = ((EnvironmentInstance<ModuleInfo>) moduleInfo).Original;
            var tenant = new DnnTenant(new PortalSettings(dnnModule.OwnerPortalID));
            return new ModuleContentBlock(moduleInfo, parentLog: null, tenant: tenant).SxcInstance;
        }

        public static IAppAndDataHelpers CodingHelpers(ISxcInstance sxc) 
            => new DnnAppAndDataHelpers(sxc as SxcInstance);

        /// <summary>
        /// get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <returns></returns>
        public static IApp App(int appId, bool versioningEnabled = false, bool showDrafts = false) 
            => App(appId, PortalSettings.Current, versioningEnabled, showDrafts);

        /// <summary>
        /// get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <returns></returns>
        public static IApp App(int zoneId, int appId, bool versioningEnabled = false, bool showDrafts = false) 
            => App(zoneId, appId, PortalSettings.Current, versioningEnabled, showDrafts);

        public static IApp App(int appId, PortalSettings ownerPortalSettings, bool versioningEnabled = false, bool showDrafts = false)
        {
            var appStuff = new App(new DnnTenant(ownerPortalSettings), appId);
            return ExtendAppWithDefaultProvider(versioningEnabled, showDrafts, appStuff);
        }

        public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool versioningEnabled = false, bool showDrafts = false)
        {
            var appStuff = new App(new DnnTenant(ownerPortalSettings), zoneId, appId);
            return ExtendAppWithDefaultProvider(versioningEnabled, showDrafts, appStuff);
        }
        private static IApp ExtendAppWithDefaultProvider(bool versioningEnabled, bool showDrafts, App appStuff)
        {
            var provider = new ValueCollectionProvider(); // use blank provider for now
            appStuff.InitData(showDrafts, versioningEnabled, provider);
            return appStuff;
        }
    }
}