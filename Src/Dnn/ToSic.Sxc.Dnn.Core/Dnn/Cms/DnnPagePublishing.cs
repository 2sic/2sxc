using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;

using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Dnn.Cms
{
    public partial class DnnPagePublishing : HasLog<IPagePublishing>, IPagePublishing
    {
        private readonly IServiceProvider _serviceProvider;

        #region DI Constructors and More
        
        public DnnPagePublishing(IServiceProvider serviceProvider) : base("Dnn.Publsh")
        {
            _serviceProvider = serviceProvider;
        }
        
        #endregion

        public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
        {
            var possibleContextOfBlock = context as IContextOfBlock;
            var enabled = possibleContextOfBlock?.Publishing.ForceDraft ?? false;
            var instanceId = possibleContextOfBlock?.Module.Id ?? Eav.Constants.IdNotInitialized;
            var userId = (context.User as DnnUser)?.UnwrappedContents.UserID ?? Eav.Constants.IdNotInitialized;
            Log.Add($"DoInsidePublishing(module:{instanceId}, user:{userId}, enabled:{enabled})");

            if (enabled)
            {
                var moduleVersionSettings = new DnnPagePublishing.ModuleVersions(instanceId, Log);
                
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



        public int GetLatestVersion(int instanceId)
        {
            var moduleVersionSettings = new DnnPagePublishing.ModuleVersions(instanceId, Log);
            var ver = moduleVersionSettings.GetLatestVersion();
            Log.Add($"GetLatestVersion(m:{instanceId}) = ver:{ver}");
            return ver;
        }

        public int GetPublishedVersion(int instanceId)
        {
            var moduleVersionSettings = new DnnPagePublishing.ModuleVersions(instanceId, Log);
            var publ = moduleVersionSettings.GetPublishedVersion();
            Log.Add($"GetPublishedVersion(m:{instanceId}) = pub:{publ}");
            return publ;
        }


        public void Publish(int instanceId, int version)
        {
            Log.Add($"Publish(m:{instanceId}, v:{version})");
            try
            {
                // publish all entites of this content block
                var dnnModule = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
                //var container = _serviceProvider.Build<DnnContainer>().Init(dnnModule, Log);
                // must find tenant through module, as the Portal-Settings.Current is null in search mode
                //var tenant = new DnnSite().Init(dnnModule.OwnerPortalID);
                var dnnContext = _serviceProvider.Build<IContextOfBlock>().InitDnnSiteModuleAndBlockContext(dnnModule, Log);
                var cb = _serviceProvider.Build<BlockFromModule>().Init(dnnContext, Log);
                    //.Init(DnnContextOfBlock.Create(tenant, container, _serviceProvider), Log);

                Log.Add($"found dnn mod {dnnContext.Module.Id}, tenant {dnnContext.Site.Id}, cb exists: {cb.ContentGroupExists}");
                if (cb.ContentGroupExists)
                {
                    Log.Add("cb exists");
                    var appManager = _serviceProvider.Build<AppManager>().Init(cb, Log);

                    // Add content entities
                    IEnumerable<IEntity> list = new List<IEntity>();
                    list = TryToAddStream(list, cb.Data, Eav.Constants.DefaultStreamName);
                    list = TryToAddStream(list, cb.Data, "ListContent");
                    list = TryToAddStream(list, cb.Data, "PartOfPage");

                    // ReSharper disable PossibleMultipleEnumeration
                    // Find related presentation entities
                    var attachedPresItems = list
                        .Select(e => e.GetDecorator<EntityInBlockDecorator>()?.Presentation)
                        .Where(p => p != null);
                        //.Where(e => (e as EntityInBlock)?.Presentation != null)
                        //.Select(e => ((EntityInBlock)e).Presentation);
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
                new ModuleVersions(instanceId, Log).PublishLatestVersion();
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
            var cont = data.GetStream(key, nullIfNotFound: true)?.List.ToImmutableList(); //  data.Out.ContainsKey(key) ? data[key]?.List?.ToImmutableList() : null;
            Log.Add($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count ?? 0}" );
            if (cont != null) list = list.Concat(cont);
            return list;
        }

    }
}
