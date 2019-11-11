using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.LookUp;

using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.DataSources;
using ToSic.Sxc;
using ToSic.Sxc.Blocks;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// This is a factory to create 2sxc-instance objects and related objects from
    /// non-2sxc environments.
    /// </summary>
    public static class Factory
    {
        public static ICmsBlock SxcInstanceForModule(int modId, int tabId)
        {
            var moduleInfo = new ModuleController().GetModule(modId, tabId, false);
            var instance = new DnnInstanceInfo(moduleInfo);
            return SxcInstanceForModule(instance);
        }

        public static ICmsBlock SxcInstanceForModule(ModuleInfo moduleInfo)
            => SxcInstanceForModule(new DnnInstanceInfo(moduleInfo));

        public static ICmsBlock SxcInstanceForModule(IInstanceInfo moduleInfo)
        {
            var dnnModule = ((EnvironmentInstance<ModuleInfo>) moduleInfo).Original;
            var tenant = new DnnTenant(new PortalSettings(dnnModule.OwnerPortalID));
            return new BlockFromModule(moduleInfo, parentLog: null, tenant: tenant).CmsInstance;
        }

        public static IDynamicCode CodingHelpers(ICmsBlock cms) 
            => new DnnAppAndDataHelpers(cms as CmsInstance);

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
            // 2018-09-22 new
            var appStuff = new App(new DnnTenant(ownerPortalSettings), Eav.Apps.App.AutoLookupZone, appId, 
                ConfigurationProvider.Build(showDrafts, versioningEnabled, new TokenListFiller()), true, null);
            return appStuff;
        }

        public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool versioningEnabled = false, bool showDrafts = false)
        {
            var appStuff = new App(new DnnTenant(ownerPortalSettings), zoneId, appId,
                ConfigurationProvider.Build(showDrafts, versioningEnabled, new TokenListFiller()), true, null);
            return appStuff;
        }

    }
}