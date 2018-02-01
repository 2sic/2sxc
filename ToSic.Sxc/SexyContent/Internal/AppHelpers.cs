using System.IO;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.Logging.Simple;
using static System.String;

namespace ToSic.SexyContent.Internal
{
    public class AppHelpers
    {

        public static int? GetAppIdFromModule(ModuleInfo module, int zoneId)
        {
            if (module.DesktopModule.ModuleName == "2sxc")
                return new ZoneRuntime(zoneId, null).DefaultAppId;

            if(module.ModuleSettings.ContainsKey(Settings.AppNameString))
                return GetAppIdFromGuidName(zoneId, module.ModuleSettings[Settings.AppNameString].ToString());
            
            return null;
        }



        internal static int GetAppIdFromGuidName(int zoneId, string appName, bool alsoCheckFolderName = false)
        {
            // ToDo: Fix issue in EAV (cache is only ensured when a CacheItem-Property is accessed like LastRefresh)
            var baseCache = (BaseCache) DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId);
            // ReSharper disable once UnusedVariable
            var dummy = baseCache.CacheTimestamp;

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
                            .GetMetadata(SystemRuntime.MetadataType(Constants.AppAssignmentName), p.Key,
                                AppConstants.AttributeSetStaticNameApps)
                            .FirstOrDefault();
                        string folder = appMetaData?.GetBestValue("Folder").ToString();
                    if (!IsNullOrEmpty(folder) && folder.ToLower() == nameLower)
                        return p.Key;

                }
            }
            return appId > 0 ? appId : Settings.DataIsMissingInDb;
        }


        internal static void SetAppIdForModule(IInstanceInfo instanceInfo, IEnvironment env, int? appId, Log parentLog)
        {
            // Reset temporary template
            ContentGroupManager.DeletePreviewTemplateId(instanceInfo.Id);

            // ToDo: Should throw exception if a real ContentGroup exists

            var module = (instanceInfo as InstanceInfo<ModuleInfo>).Info;
            var zoneId = env.ZoneMapper.GetZoneId(module.OwnerPortalID);

            if (appId == 0 || !appId.HasValue)
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(instanceInfo.Id, Settings.AppNameString, null);
            else
            {
                var appName = ((BaseCache)DataSource.GetCache(0, 0)).ZoneApps[zoneId].Apps[appId.Value];
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(instanceInfo.Id, Settings.AppNameString, appName);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                var app = new App(zoneId, appId.Value, PortalSettings.Current);
                var templateGuid = app.TemplateManager.GetAllTemplates().FirstOrDefault(t => !t.IsHidden)?.Guid;
                if (templateGuid.HasValue)
                    app.ContentGroupManager.SetModulePreviewTemplateId(instanceInfo.Id, templateGuid.Value);
            }
        }


        // todo: remove this, replace with calls to the current tennant -> RootPath
        public static string AppBasePath(PortalSettings ownerPS )
        {
            if (ownerPS == null)
                ownerPS = PortalSettings.Current;
            return Path.Combine(ownerPS.HomeDirectory, Settings.TemplateFolder);
        }
    }
}