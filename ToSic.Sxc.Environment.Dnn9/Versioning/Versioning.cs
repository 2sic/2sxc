using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using System;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Apps.Environment;

namespace ToSic.Sxc.Environment.Dnn9.Environment
{
    public class Versioning : IEnvironmentVersioning
    {
        public bool Supported => true;
        
        public VersioningRequirements Requirements(int moduleId)
        {
            var moduleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            var versioningEnabled = TabChangeSettings.Instance.IsChangeControlEnabled(moduleInfo.PortalID, moduleInfo.TabID);
            if (!versioningEnabled)
                return VersioningRequirements.DraftOptional;
            
            var portalSettings = new PortalSettings(moduleInfo.PortalID);
            if (!portalSettings.UserInfo.IsSuperUser)
                return VersioningRequirements.DraftRequired;
            
            // Else versioningEnabled && IsSuperUser
            return VersioningRequirements.DraftRecommended;
        }
        
        public bool IsVersioningEnabled(int moduleId)
        {
            return Requirements(moduleId) != VersioningRequirements.DraftOptional;
        }

        public void DoInsideVersioning(int moduleId, int userId, Action<VersioningActionInfo> action)
        {
            if (IsVersioningEnabled(moduleId))
            {
                var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
                if (moduleVersionSettings.IsLatestVersionPublished())
                {
                    // If the latest version is published, get an new version number and submit it to DNN
                    TabChangeTracker.Instance.TrackModuleModification
                    (
                        moduleVersionSettings.ModuleInfo, moduleVersionSettings.IncreaseLatestVersion(), userId
                    );
                }
            }

            var versioningActionInfo = new VersioningActionInfo() { };
            action.Invoke(versioningActionInfo);
        }

        public void DoInsidePublishLatestVersion(int moduleId, Action<VersioningActionInfo> action)
        {
            if(IsVersioningEnabled(moduleId))
            {
                var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
                moduleVersionSettings.PublishLatestVersion();
            }

            var versioningActionInfo = new VersioningActionInfo() { };
            action.Invoke(versioningActionInfo);
        }

        public void DoInsideDeleteLatestVersion(int moduleId, Action<VersioningActionInfo> action)
        {
            if (IsVersioningEnabled(moduleId))
            {
                var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
                moduleVersionSettings.DeleteLatestVersion();
            }

            var versioningActionInfo = new VersioningActionInfo() { };
            action.Invoke(versioningActionInfo);
        }

        public int GetLatestVersion(int moduleId)
        {
            var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
            return moduleVersionSettings.GetLatestVersion();
        }

        public int GetPublishedVersion(int moduleId)
        {
            var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
            return moduleVersionSettings.GetPublishedVersion();
        }
    }
}
