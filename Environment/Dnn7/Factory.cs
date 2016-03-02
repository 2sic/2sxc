using DotNetNuke.Entities.Modules;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// This is a factory to create 2sxc-instance objects and related objects from
    /// non-2sxc environments.
    /// </summary>
    public static class Factory
    {
        public static ISxcInstance GetSxcInstanceForModule(int modId, int tabId)
        {
            var moduleInfo = new ModuleController().GetModule(modId, tabId, false);

            return SxcInstanceForModule(moduleInfo);
        }

        public static ISxcInstance SxcInstanceForModule(ModuleInfo moduleInfo)
        {
            var appId = AppHelpers.GetAppIdFromModule(moduleInfo);
            var zoneId = ZoneHelpers.GetZoneID(moduleInfo.PortalID);
            ISxcInstance sxcSxcInstance = new SxcInstance(zoneId.Value, appId.Value, true, moduleInfo.OwnerPortalID, moduleInfo);
            return sxcSxcInstance;
        }

        public static IAppAndDataHelpers CodingHelpers(ISxcInstance sxc)
        {
            var appAndDataHelpers = new AppAndDataHelpers(sxc as SxcInstance);

            return appAndDataHelpers;
        }
    }
}