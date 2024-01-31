using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Internal;
using ToSic.Sxc.LookUp.Internal;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// Generic Lookup Resolver - will provide all lookups which are registered in DI
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LookUpEngineResolverGeneric(LazySvc<IEnumerable<ILookUp>> lookUps, LazySvc<IHttp> httpLazy)
    : LookUpEngineResolverBase(httpLazy, $"{SxcLogging.SxcLogName}.LUpEnR"), ILookUpEngineResolver
{
    /// <summary>
    /// Get all lookup engines which are registered in DI.
    /// 2024-01-30 ATM probably only used in Oqtane.
    /// </summary>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    public override ILookUpEngine GetLookUpEngine(int instanceId)
    {
        var l = Log.Fn<ILookUpEngine>($"{nameof(instanceId)}:{instanceId}");

        // Try Cached first
        if (TryGetFromCache(instanceId, out var cached))
            return l.Return(cached, $"reuse {cached.Sources.Count} sources");

        var luEngine = new LookUpEngine(Log);
        luEngine.Add(lookUps.Value.ToList());

        GetHttpSources(luEngine).UseIfNotNull(luEngine.Add);

        return l.Return(AddToCache(instanceId, luEngine), "created");
    }


}