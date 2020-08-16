using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Run;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Dnn.Cms
{
    public partial class PagePublishing : HasLog, IPagePublishing
    {
        #region DI Constructors and More
        
        public PagePublishing(): base("Dnn.Publsh") { }

        public IPagePublishing Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        #endregion

        public bool Supported => true;

        private readonly Dictionary<int, PublishingMode> _cache = new Dictionary<int, PublishingMode>();
        
        public PublishingMode Requirements(int moduleId)
        {
            var wrapLog = Log.Call<PublishingMode>($"{moduleId}");
            if (_cache.ContainsKey(moduleId)) return wrapLog("in cache", _cache[moduleId]);

            Log.Add($"Requirements(mod:{moduleId}) - checking first time (others will be cached)");
            try
            {
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
                PublishingMode decision;
                var versioningEnabled =
                    TabChangeSettings.Instance.IsChangeControlEnabled(moduleInfo.PortalID, moduleInfo.TabID);
                if (!versioningEnabled)
                    decision = PublishingMode.DraftOptional;
                else if (!new PortalSettings(moduleInfo.PortalID).UserInfo.IsSuperUser)
                    decision = PublishingMode.DraftRequired;
                else
                    decision = PublishingMode.DraftRequired;

                //Log.Add($"decision: {decision}");
                _cache.Add(moduleId, decision);
                return wrapLog("decision: ", decision);
            }
            catch
            {
                Log.Add("Requirements had exception!");
                throw;
            }
        }
        
        public bool IsEnabled(int moduleId) => Requirements(moduleId) != PublishingMode.DraftOptional;

        public void DoInsidePublishing(int moduleId, int userId, Action<VersioningActionInfo> action)
        {
            var enabled = IsEnabled(moduleId);
            Log.Add($"DoInsidePublishing(module:{moduleId}, user:{userId}, enabled:{enabled})");
            if (enabled)
            {
                var moduleVersionSettings = new PagePublishing.ModuleVersions(moduleId, Log);
                
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
            Log.Add("/DoInsidePublishing");
        }



        public int GetLatestVersion(int moduleId)
        {
            var moduleVersionSettings = new PagePublishing.ModuleVersions(moduleId, Log);
            var ver = moduleVersionSettings.GetLatestVersion();
            Log.Add($"GetLatestVersion(m:{moduleId}) = ver:{ver}");
            return ver;
        }

        public int GetPublishedVersion(int moduleId)
        {
            var moduleVersionSettings = new PagePublishing.ModuleVersions(moduleId, Log);
            var publ = moduleVersionSettings.GetPublishedVersion();
            Log.Add($"GetPublishedVersion(m:{moduleId}) = pub:{publ}");
            return publ;
        }


        public void Publish(int instanceId, int version)
        {
            Log.Add($"Publish(m:{instanceId}, v:{version})");
            try
            {
                // publish all entites of this content block
                var dnnModule = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
                var container = new DnnContainer(dnnModule, Log);
                // must find tenant through module, as the Portal-Settings.Current is null in search mode
                var tenant = new DnnTenant(new PortalSettings(dnnModule.OwnerPortalID));
                var cb = new BlockFromModule().Init(tenant, container, Log);

                Log.Add($"found dnn mod {container.Id}, tenant {tenant.Id}, cb exists: {cb.ContentGroupExists}");
                if (cb.ContentGroupExists)
                {
                    Log.Add("cb exists");
                    var appManager = new AppManager(cb, Log);

                    // Add content entities
                    IEnumerable<IEntity> list = new List<IEntity>();
                    list = TryToAddStream(list, cb.Data, Constants.DefaultStreamName);
                    list = TryToAddStream(list, cb.Data, "ListContent");
                    list = TryToAddStream(list, cb.Data, "PartOfPage");

                    // ReSharper disable PossibleMultipleEnumeration
                    // Find related presentation entities
                    var attachedPresItems = list
                        .Where(e => (e as EntityInBlock)?.Presentation != null)
                        .Select(e => ((EntityInBlock)e).Presentation);
                    Log.Add($"adding presentation item⋮{attachedPresItems.Count()}");
                    list = list.Concat(attachedPresItems);
                    // ReSharper restore PossibleMultipleEnumeration

                    var ids = list.Where(e => !e.IsPublished).Select(e => e.EntityId).ToList();

                    // publish BlockConfiguration as well - if there already is one
                    if (cb.Configuration != null)
                    {
                        Log.Add($"add group id:{cb.Configuration.Id}");
                        ids.Add(cb.Configuration.Id);
                    }

                    Log.Add(() => $"will publish id⋮{ids.Count} ids:[{ string.Join(",", ids.Select(i => i.ToString()).ToArray()) }]");

                    if (ids.Any())
                        appManager.Entities.Publish(ids.ToArray());
                    else
                        Log.Add("no ids found, won\'t publish items");
                }

                // Set published version
                new PagePublishing.ModuleVersions(instanceId, Log).PublishLatestVersion();
                Log.Add("publish completed");
            }
            catch (Exception ex)
            {
                DnnLogging.LogToDnn("exception", "publishing", Log, force:true);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                throw;
            }

        }

        private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, IBlockDataSource data, string key)
        {
            var cont = data.Out.ContainsKey(key) ? data[key]?.List?.ToList() : null;
            Log.Add($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count ?? 0}" );
            if (cont != null) list = list.Concat(cont);
            return list;
        }

    }
}
