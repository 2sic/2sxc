using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

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
            var appId = AppHelpers.GetAppIdFromModule(moduleInfo).Value;
            var zoneId = ZoneHelpers.GetZoneID(moduleInfo.PortalID).Value;
            ISxcInstance sxcSxcInstance = new SxcInstance(zoneId, appId, moduleInfo.OwnerPortalID, moduleInfo);
            return sxcSxcInstance;
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
        public static IApp App(int appId)
        {
            return App(appId, PortalSettings.Current);
        }

        public static IApp App(int appId, PortalSettings ownerPortalSettings)
        {
            if(ownerPortalSettings == null)
                throw new Exception("no portal settings received");

            var zoneId = ZoneHelpers.GetZoneID(ownerPortalSettings.PortalId);

            if (!zoneId.HasValue)
                throw new Exception("Cannot find zone-id for portal specified");

            var appStuff = new App(appId, zoneId.Value, ownerPortalSettings);

            var provider = new ValueCollectionProvider(); // use blank provider for now

            appStuff.InitData(false, provider);

            return appStuff;
        }

    }
}