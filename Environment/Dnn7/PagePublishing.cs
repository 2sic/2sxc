using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using System;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Environment.Dnn7
{
    internal partial class PagePublishing : IPagePublishing
    {
        #region logging
        private readonly Log _log = new Log("DnPubl");
        #endregion

        public PagePublishing(Log parentLog = null)
        {
            _log.LinkTo(parentLog);
        }

        public bool Supported => true;
        
        public PublishingMode Requirements(int moduleId)
        {
            var moduleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            var versioningEnabled = TabChangeSettings.Instance.IsChangeControlEnabled(moduleInfo.PortalID, moduleInfo.TabID);
            if (!versioningEnabled)
                return PublishingMode.DraftOptional;
            
            var portalSettings = new PortalSettings(moduleInfo.PortalID);
            if (!portalSettings.UserInfo.IsSuperUser)
                return PublishingMode.DraftRequired;
            
            // Else versioningEnabled && IsSuperUser
            return PublishingMode.DraftRequired;
        }
        
        public bool IsVersioningEnabled(int moduleId)
        {
            return Requirements(moduleId) != PublishingMode.DraftOptional;
        }

        public void DoInsidePublishing(int moduleId, int userId, Action<VersioningActionInfo> action)
        {
            var enabled = IsVersioningEnabled(moduleId);
            _log.Add("do inside mid:" + moduleId + ", uid:" + userId + ", enabled:" + enabled);
            if (enabled)
            {
                var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
                
                // Get an new version number and submit it to DNN
                // The submission must be made every time something changes, because a "discard" could have happened
                // in the meantime.
                TabChangeTracker.Instance.TrackModuleModification(
                    moduleVersionSettings.ModuleInfo, 
                    moduleVersionSettings.IncreaseLatestVersion(), 
                    userId
                );
            }

            var versioningActionInfo = new VersioningActionInfo();
            action.Invoke(versioningActionInfo);
        }

        //public void DoInsidePublishLatestVersion(int moduleId, Action<VersioningActionInfo> action)
        //{
        //    if(IsVersioningEnabled(moduleId))
        //    {
        //        var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
        //        moduleVersionSettings.PublishLatestVersion();
        //    }

        //    var versioningActionInfo = new VersioningActionInfo();
        //    action.Invoke(versioningActionInfo);
        //}

        //public void DoInsideDeleteLatestVersion(int moduleId, Action<VersioningActionInfo> action)
        //{
        //    if (IsVersioningEnabled(moduleId))
        //    {
        //        var moduleVersionSettings = new ModuleVersionSettingsController(moduleId);
        //        moduleVersionSettings.DeleteLatestVersion();
        //    }

        //    var versioningActionInfo = new VersioningActionInfo();
        //    action.Invoke(versioningActionInfo);
        //}

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
