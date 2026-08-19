using System.Collections.Concurrent;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// Work service to figure out if a block/page must be versioned.
/// </summary>
/// <param name="workSequence">DI injected list of <see cref="IWorkBlockPublishingLookup"/> parts.</param>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class BlockPublishingSettingsService(IWorkSequence<IWorkBlockPublishingLookup, BlockPublishingSettings> workSequence): ServiceBase("Pub.Mgr"),
    IWork<BlockPublishingSettings>
{
    /// <summary>
    /// The main work. Uses static cache as the settings shouldn't really change at runtime.
    /// </summary>
    public async Task<Package<BlockPublishingSettings>> Handle(WorkContext workCtx, Package<BlockPublishingSettings> package)
    {
        var instanceId = package.Data.ModuleId;
        var l = Log.Fn<Package<BlockPublishingSettings>>($"ModuleId: {instanceId}");
        
        // If no ID, exit early
        if (instanceId < 0)
            return l.Return(Stop(package, PublishingMode.DraftOptional), "no instance");
        
        // Check if cached
        if (Cache.TryGetValue(instanceId, out var value))
            return l.Return(Stop(package, value), "from cache");
        

        // Ask derived class for the requirements
        var decision = await workSequence.Handle(workCtx, package);

        // Cache and return
        if (decision.Decision is ResultState.Default or ResultState.StopSequence)
            Cache.TryAdd(instanceId, decision.Data.Mode);
        
        return l.Return(decision, $"decision:{decision}");
    }
    
    /// <summary>
    /// Cache
    /// </summary>
    protected static readonly ConcurrentDictionary<int, PublishingMode> Cache = new();

    /// <summary>
    /// Helper to repackage the result.
    /// </summary>
    public static Package<BlockPublishingSettings> Stop(Package<BlockPublishingSettings> package, PublishingMode mode)
        => package with
        {
            Data = WithNewMode(package.Data, mode),
            Decision = ResultState.StopSequence,
        };

    /// <summary>
    /// Helper to repackage the result.
    /// </summary>
    public static BlockPublishingSettings WithNewMode(BlockPublishingSettings options, PublishingMode mode)
        => options with
        {
            AllowDraft = mode != PublishingMode.DraftForbidden,
            ForceDraft = mode == PublishingMode.DraftRequired,
            Mode = mode
        };

}