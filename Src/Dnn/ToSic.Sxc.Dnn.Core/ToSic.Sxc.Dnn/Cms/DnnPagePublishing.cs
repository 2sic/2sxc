using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataSource;
using ToSic.Eav.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Dnn.Cms;

internal partial class DnnPagePublishing(
    LazySvc<DnnModuleContextFactory> ctxOfModFac,
    LazySvc<BlockOfModule> moduleAndBlockBuilder,
    AppWorkQuick<WorkEntityPublish> entPublish,
    IContextCompleteService ctxSvc,
    IUser user)
    : ServiceBase("Dnn.Publsh", connect: [ctxOfModFac, moduleAndBlockBuilder, entPublish, ctxSvc]), IPagePublishing
{

    public void DoInsidePublishing(Action<VersioningActionInfo> action)
    {
        var blockCtxOrNull = ctxSvc.BlockContextOrNull();
        var publishingEnabled = blockCtxOrNull?.Publishing.ForceDraft ?? false;
        var modId = blockCtxOrNull?.Module.Id ?? EavConstants.IdNotInitialized;
        
        var userId = user.Id;

        var l = Log.Fn($"{nameof(modId)}:{modId}, user:{userId}, {nameof(publishingEnabled)}:{publishingEnabled}");

        if (publishingEnabled)
        {
            var moduleVersionSettings = new ModuleVersions(modId, Log);
                
            // Get an new version number and submit it to DNN
            // The submission must be made every time something changes, because a "discard" could have happened
            // in the meantime.
            TabChangeTracker.Instance.TrackModuleModification(
                moduleVersionSettings.ModuleInfo, 
                moduleVersionSettings.IncreaseLatestVersion(), 
                userId
            );
        }

        var versioningActionInfo = new VersioningActionInfo { ForceDraft = publishingEnabled };
        action.Invoke(versioningActionInfo);
        l.Done();
    }



    public int GetLatestVersion(int instanceId)
    {
        var moduleVersionSettings = new ModuleVersions(instanceId, Log);
        var ver = moduleVersionSettings.GetLatestVersion();
        Log.A($"GetLatestVersion(m:{instanceId}) = ver:{ver}");
        return ver;
    }

    public int GetPublishedVersion(int instanceId)
    {
        var moduleVersionSettings = new ModuleVersions(instanceId, Log);
        var pubVersion = moduleVersionSettings.GetPublishedVersion();
        Log.A($"GetPublishedVersion(m:{instanceId}) = pub:{pubVersion}");
        return pubVersion;
    }


    public void Publish(int instanceId, int version)
    {
        var l = Log.Fn($"Publish(m:{instanceId}, v:{version})");
        try
        {
            // publish all entities of this content block
            var dnnModule = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
            // must find tenant through module, as the Portal-Settings.Current is null in search mode
            var modCtx = ctxOfModFac.Value.InitDnnSiteModuleAndBlockContext(dnnModule);
            var cb = moduleAndBlockBuilder.Value.GetBlockOfModule(modCtx);

            l.A($"found dnn mod {cb.Context.Module.Id}, tenant {cb.Context.Site.Id}, cb exists: {cb.ContentGroupExists}");
            if (cb.ContentGroupExists)
            {
                l.A("cb exists");

                // Add content entities
                IEnumerable<IEntity> list = new List<IEntity>();
                list = TryToAddStream(list, cb.Data, DataSourceConstants.StreamDefaultName);
                list = TryToAddStream(list, cb.Data, "ListContent");
                list = TryToAddStream(list, cb.Data, "PartOfPage");

                // ReSharper disable PossibleMultipleEnumeration
                // Find related presentation entities
                var attachedPresItems = list
                    .Select(e => e.GetDecorator<EntityInBlockDecorator>()?.Presentation)
                    .Where(p => p != null);
                l.A($"adding presentation item⋮{attachedPresItems.Count()}");
                list = list.Concat(attachedPresItems);
                // ReSharper restore PossibleMultipleEnumeration

                var ids = list
                    .Where(e => !e.IsPublished)
                    .Select(e => e.EntityId)
                    .ToList();

                // publish BlockConfiguration as well - if there already is one
                if (cb.ConfigurationIsReady)
                {
                    l.A($"add group id:{cb.Configuration.Id}");
                    ids.Add(cb.Configuration.Id);
                }

                l.A(Log.Try(() => $"will publish id⋮{ids.Count} ids:[{ string.Join(",", ids.Select(i => i.ToString()).ToArray()) }]"));

                if (ids.Any())
                {
                    entPublish.New(cb.Context.AppReaderRequired).Publish([.. ids]);
                }
                else
                    l.A("no ids found, won\'t publish items");
            }

            // Set published version
            new ModuleVersions(instanceId, Log).PublishLatestVersion();
            l.Done("publish completed");
        }
        catch (Exception ex)
        {
            DnnLogging.LogToDnn("exception", "publishing", Log, force: true);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            l.Done(ex);
            throw;
        }

    }

    private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, IDataSource data, string key)
    {
        var cont = data.GetStream(key, nullIfNotFound: true)?.List.ToImmutableOpt();
        Log.A($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count ?? 0}" );
        if (cont != null) list = list.Concat(cont);
        return list;
    }

}