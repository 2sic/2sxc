using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.DataSources.Caches;
using static System.String;

namespace ToSic.SexyContent.Internal
{
    public class AppHelpers
    {
        //public class AppResponse
        //{
        //    public int AppId;
        //    public bool IsStored;
        //    public bool IsDefaultApp;
        //}

        public static int? GetAppIdFromModule(ModuleInfo module, int zoneId)
        {
            if (module.DesktopModule.ModuleName == "2sxc")
                return GetDefaultAppId(zoneId);// : new int?();

            var appName = DnnStuffToRefactor.TryToGetReliableSetting(module, Settings.AppNameString);

            if (appName != null)
                return GetAppIdFromName(zoneId, appName);

            return null;
        }



        internal static int GetAppIdFromName(int zoneId, string appName)
        {
            // ToDo: Fix issue in EAV (cache is only ensured when a CacheItem-Property is accessed like LastRefresh)
            var baseCache = ((BaseCache) DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId));
            var dummy = baseCache.LastRefresh;

            if (IsNullOrEmpty(appName))
                appName = Constants.DefaultAppName;

            var appId = baseCache.ZoneApps[zoneId].Apps
                    .Where(p => p.Value == appName).Select(p => p.Key).FirstOrDefault();

            return appId;
        }

        public static void SetAppIdForModule(ModuleInfo module, int? appId)
        {
            //var moduleController = new ModuleController();

            // Reset temporary template
            ContentGroupManager.DeletePreviewTemplateId(module.ModuleID);

            // ToDo: Should throw exception if a real ContentGroup exists

            var zoneId = ZoneHelpers.GetZoneID(module.OwnerPortalID);

            if (appId == 0 || !appId.HasValue)
                //moduleController.DeleteModuleSetting(module.ModuleID, AppNameString);
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(module.ModuleID, Settings.AppNameString, null);
            else
            {
                var appName = ((BaseCache)DataSource.GetCache(0, 0)).ZoneApps[zoneId.Value].Apps[appId.Value];
                //moduleController.UpdateModuleSetting(module.ModuleID, AppNameString, appName);
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(module.ModuleID, Settings.AppNameString, appName);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                //var sexyForNewApp = new SxcInstance(zoneId.Value, appId.Value);// 2016-03-26 2dm this used to have a third parameter false = don't enable caching, which hasn't been respected for a while; removed it
                var app = new App(PortalSettings.Current, appId.Value, zoneId.Value);
                var templates = app.TemplateManager.GetAvailableTemplatesForSelector(module.ModuleID, app.ContentGroupManager).ToList();
                if (templates.Any())
                    app.ContentGroupManager.SetModulePreviewTemplateId(module.ModuleID, templates.First().Guid /* .TemplateId */);
            }
        }


        public static int GetDefaultAppId(int zoneId)
        {
            return ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].DefaultAppId;
        }


        public static string AppBasePath(PortalSettings ownerPS)
        {
            return Path.Combine(ownerPS.HomeDirectory, Settings.TemplateFolder);
        }
    }
}