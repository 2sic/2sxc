using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.Interfaces;

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

            return SxcInstanceForModule(moduleInfo);
        }

        public static ISxcInstance SxcInstanceForModule(ModuleInfo moduleInfo)
        {
            ModuleContentBlock mcb = new ModuleContentBlock(moduleInfo);
            return mcb.SxcInstance;
        }

        public static IAppAndDataHelpers CodingHelpers(ISxcInstance sxc)
        {
            var appAndDataHelpers = new AppAndDataHelpers(sxc as SxcInstance);

            return appAndDataHelpers;
        }

        /// <summary>
        /// get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static IApp App(int appId, bool versioningEnabled)
        {
            return App(appId, PortalSettings.Current, versioningEnabled);
        }

        public static IApp App(int appId, PortalSettings ownerPortalSettings, bool versioningEnabled)
        {
            var appStuff = new App(ownerPortalSettings, appId);

            var provider = new ValueCollectionProvider(); // use blank provider for now

            appStuff.InitData(false, versioningEnabled, provider);

            return appStuff;
        }

    }
}