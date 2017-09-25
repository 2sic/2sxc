using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent.Environment.Dnn7
{
    internal partial class PagePublishing : IPagePublishing
    {
        #region logging

        private Log Log { get; }
        #endregion

        public PagePublishing(Log parentLog)
        {
            Log = new Log("DnPubl", parentLog);
        }

        public bool Supported => true;
        
        public PublishingMode Requirements(int moduleId)
        {
            Log.Add($"requirements for mod:{moduleId}");
            var moduleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            var versioningEnabled = TabChangeSettings.Instance.IsChangeControlEnabled(moduleInfo.PortalID, moduleInfo.TabID);
            if (!versioningEnabled)
            {
                Log.Add("result: is optional");
                return PublishingMode.DraftOptional;
            }

            var portalSettings = new PortalSettings(moduleInfo.PortalID);
            if (!portalSettings.UserInfo.IsSuperUser)
            {
                Log.Add("is super user, but draft still required");
                return PublishingMode.DraftRequired;
            }

            // Else versioningEnabled && IsSuperUser
            Log.Add("normal user, draft required");
            return PublishingMode.DraftRequired;
        }
        
        public bool IsVersioningEnabled(int moduleId)
        {
            return Requirements(moduleId) != PublishingMode.DraftOptional;
        }

        public void DoInsidePublishing(int moduleId, int userId, Action<VersioningActionInfo> action)
        {
            var enabled = IsVersioningEnabled(moduleId);
            Log.Add($"do inside publishing - module:{moduleId}, user:{userId}, enabled:{enabled}");
            if (enabled)
            {
                var moduleVersionSettings = new ModuleVersions(moduleId);
                
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
            Log.Add("do inside publishing - completed");
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
            var moduleVersionSettings = new ModuleVersions(moduleId);
            var ver = moduleVersionSettings.GetLatestVersion();
            Log.Add($"get latest for m:{moduleId} ver:{ver}");
            return ver;
        }

        public int GetPublishedVersion(int moduleId)
        {
            var moduleVersionSettings = new ModuleVersions(moduleId);
            var publ = moduleVersionSettings.GetPublishedVersion();
            Log.Add($"get latest for m:{moduleId} pub:{publ}");
            return publ;
        }


        public void Publish(int instanceId, int version)
        {
            Log.Add($"publish started m:{instanceId}, v:{version}");
            try
            {
                // publish all entites of this content block
                var moduleInfo = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
                var cb = new ModuleContentBlock(moduleInfo, Log);

                if (cb.ContentGroupExists)
                {
                    Log.Add("cb exists");
                    var appManager = new AppManager(cb.AppId, Log);

                    // Add content entities
                    IEnumerable<IEntity> list = new List<IEntity>();
                    list = TryToAddStream(list, cb.Data, "Default");
                    list = TryToAddStream(list, cb.Data, "ListContent");
                    list = TryToAddStream(list, cb.Data, "PartOfPage");

                    // ReSharper disable PossibleMultipleEnumeration
                    // Find related presentation entities
                    var attachedPresItems = list
                        .Where(e => (e as EntityInContentGroup)?.Presentation != null)
                        .Select(e => ((EntityInContentGroup)e).Presentation);
                    Log.Add($"adding presentation item⋮{attachedPresItems.Count()}");
                    list = list.Concat(attachedPresItems);
                    // ReSharper restore PossibleMultipleEnumeration

                    var ids = list.Where(e => !e.IsPublished).Select(e => e.EntityId).ToList();

                    // publish ContentGroup as well - if there already is one
                    if (cb.ContentGroup != null)
                    {
                        Log.Add($"add group id:{cb.ContentGroup.ContentGroupId}");
                        ids.Add(cb.ContentGroup.ContentGroupId);
                    }

                    Log.Add(() => $"will publish id⋮{ids.Count} ids:[{ string.Join(",", ids.Select(i => i.ToString()).ToArray()) }]");

                    if (ids.Any())
                        appManager.Entities.Publish(ids.ToArray());
                    else
                        Log.Add("no ids found, won\'t publish items");
                }

                // Set published version
                new ModuleVersions(instanceId).PublishLatestVersion();
                Log.Add("publish completed");
            }
            catch (Exception ex)
            {
                Logging.LogToDnn("exception", "publishing", Log, force:true);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                throw;
            }

        }

        private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, ViewDataSource data, string key)
        {
            var cont = data.Out.ContainsKey(key) ? data[key]?.LightList?.ToList() : null;
            Log.Add($"try to add stream:{key}, found:{cont != null} add⋮{cont?.Count ?? 0}" );
            if (cont != null) list = list.Concat(cont);
            return list;
        }
    }
}
