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
            Log = new Log("DNN.Publsh", parentLog);
        }

        public bool Supported => true;

        private readonly Dictionary<int, PublishingMode> _cache = new Dictionary<int, PublishingMode>();
        
        public PublishingMode Requirements(int moduleId)
        {
            if (_cache.ContainsKey(moduleId)) return _cache[moduleId];

            Log.Add($"checking requirements first time for mod:{moduleId}");
            var moduleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            PublishingMode decision;
            var versioningEnabled = TabChangeSettings.Instance.IsChangeControlEnabled(moduleInfo.PortalID, moduleInfo.TabID);
            if (!versioningEnabled)
                decision = PublishingMode.DraftOptional;
            else if (!new PortalSettings(moduleInfo.PortalID).UserInfo.IsSuperUser)
                decision = PublishingMode.DraftRequired;
            else
                decision = PublishingMode.DraftRequired;

            Log.Add($"decision: {decision}");
            _cache.Add(moduleId, decision);
            return decision;
        }
        
        public bool IsEnabled(int moduleId) => Requirements(moduleId) != PublishingMode.DraftOptional;

        public void DoInsidePublishing(int moduleId, int userId, Action<VersioningActionInfo> action)
        {
            var enabled = IsEnabled(moduleId);
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
                var dnnModule = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
                var instanceInfo = new DnnInstanceInfo(dnnModule);
                // must find tenant through module, as the PortalSettings.Current is null in search mode
                var tenant = new DnnTenant(new PortalSettings(dnnModule.OwnerPortalID));
                var cb = new ModuleContentBlock(instanceInfo, Log, tenant);

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
            var cont = data.Out.ContainsKey(key) ? data[key]?.List?.ToList() : null;
            Log.Add($"try to add stream:{key}, found:{cont != null} add⋮{cont?.Count ?? 0}" );
            if (cont != null) list = list.Concat(cont);
            return list;
        }
    }
}
