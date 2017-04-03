using System.IO;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.DataSources.Caches;
using static System.String;

namespace ToSic.SexyContent.Internal
{
    public class AppHelpers
    {
        public static int? GetAppIdFromModule(ModuleInfo module, int zoneId)
        {
            if (module.DesktopModule.ModuleName == "2sxc")
                return State.GetDefaultAppId(zoneId);

            var appName = DnnStuffToRefactor.TryToGetReliableSetting(module, Settings.AppNameString);

            if (appName != null)
                return GetAppIdFromGuidName(zoneId, appName);

            return null;
        }



        internal static int GetAppIdFromGuidName(int zoneId, string appName, bool alsoCheckFolderName = false)
        {
            // ToDo: Fix issue in EAV (cache is only ensured when a CacheItem-Property is accessed like LastRefresh)
            var baseCache = ((BaseCache) DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId));
            // ReSharper disable once UnusedVariable
            var dummy = baseCache.LastRefresh;

            if (IsNullOrEmpty(appName))
                return 0; 

            var appId = baseCache.ZoneApps[zoneId].Apps
                    .Where(p => p.Value == appName).Select(p => p.Key).FirstOrDefault();

            // optionally check folder names
            if (appId == 0 && alsoCheckFolderName)
            {
                var nameLower = appName.ToLower();
                foreach (var p in baseCache.ZoneApps[zoneId].Apps)
                {
                        var mds = DataSource.GetMetaDataSource(zoneId, p.Key);
                        var appMetaData = mds
                            .GetAssignedEntities(State.GetAssignmentTypeId(Constants.AppAssignmentName), p.Key,
                                Settings.AttributeSetStaticNameApps)
                            .FirstOrDefault();
                        string folder = appMetaData?.GetBestValue("Folder").ToString();
                    if (!IsNullOrEmpty(folder) && folder.ToLower() == nameLower)
                        return p.Key;

                }
            }
            return appId > 0 ? appId : Settings.DataIsMissingInDb;
        }


        public static void SetAppIdForModule(ModuleInfo module, int? appId)
        {
            // Reset temporary template
            ContentGroupManager.DeletePreviewTemplateId(module.ModuleID);

            // ToDo: Should throw exception if a real ContentGroup exists

            var zoneId = new Environment.Environment().ZoneMapper.GetZoneId(module.OwnerPortalID);// ZoneHelpers.GetZoneId(module.OwnerPortalID);

            if (appId == 0 || !appId.HasValue)
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(module.ModuleID, Settings.AppNameString, null);
            else
            {
                var appName = ((BaseCache)DataSource.GetCache(0, 0)).ZoneApps[zoneId].Apps[appId.Value];
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(module.ModuleID, Settings.AppNameString, appName);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                var app = new App(zoneId, appId.Value, PortalSettings.Current);
                var templates = app.TemplateManager.GetAvailableTemplatesForSelector(module.ModuleID, app.ContentGroupManager).ToList();
                if (templates.Any())
                    app.ContentGroupManager.SetModulePreviewTemplateId(module.ModuleID, templates.First().Guid /* .TemplateId */);
            }
        }


        //public static int GetDefaultAppId(int zoneId)
        //    => State.GetDefaultAppId(zoneId);// ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].DefaultAppId;


        public static string AppBasePath(PortalSettings ownerPS )


        {
            if (ownerPS == null)
                ownerPS = PortalSettings.Current;
            return Path.Combine(ownerPS.HomeDirectory, Settings.TemplateFolder);
        }
    }
}