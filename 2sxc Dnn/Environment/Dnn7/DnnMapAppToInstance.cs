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
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.Environment.Dnn7
{

    public class DnnMapAppToInstance : IMapAppToInstance
    {

        public int? GetAppIdFromInstance(IInstanceInfo instance, int zoneId)
        {
            var module = (instance as InstanceInfo<ModuleInfo>).Info;

            if (module.DesktopModule.ModuleName == "2sxc")
                return new ZoneRuntime(zoneId, null).DefaultAppId;

            if(module.ModuleSettings.ContainsKey(Settings.AppNameString))
                return AppHelpers.GetAppIdFromGuidName(zoneId, module.ModuleSettings[Settings.AppNameString].ToString());
            
            return null;
        }


        
        public void SetAppIdForInstance(IInstanceInfo instance, IEnvironment env, int? appId, Log parentLog)
        {
            // Reset temporary template
            ContentGroupManager.DeletePreviewTemplateId(instance.Id);

            // ToDo: Should throw exception if a real ContentGroup exists

            var module = (instance as InstanceInfo<ModuleInfo>).Info;
            var zoneId = env.ZoneMapper.GetZoneId(module.OwnerPortalID);

            if (appId == 0 || !appId.HasValue)
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(instance.Id, Settings.AppNameString, null);
            else
            {
                var appName = ((BaseCache)DataSource.GetCache(0, 0)).ZoneApps[zoneId].Apps[appId.Value];
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(instance.Id, Settings.AppNameString, appName);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                var app = new App(zoneId, appId.Value, new DnnTennant(null));
                var templateGuid = app.TemplateManager.GetAllTemplates().FirstOrDefault(t => !t.IsHidden)?.Guid;
                if (templateGuid.HasValue)
                    app.ContentGroupManager.SetModulePreviewTemplateId(instance.Id, templateGuid.Value);
            }
        }


        // todo: remove this, replace with calls to the current tennant -> RootPath
        public static string AppBasePath() 
            => Path.Combine(PortalSettings.Current.HomeDirectory, Settings.AppsRootFolder);
    }
}