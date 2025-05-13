using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.LookUp.Internal;

public abstract class LookUpEngineResolverBase(LazySvc<IEnumerable<ILookUp>> builtInSources, string logName, NoParamOrder protect = default, object[] connect = default)
    : ServiceBase(logName, protect, connect), ILookUpEngineResolver
{
    /// <summary>
    /// Get the lookup engine - if possible from cache, otherwise create a new one
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    public virtual ILookUpEngine GetLookUpEngine(int moduleId)
    {
        var l = Log.Fn<ILookUpEngine>($"{nameof(moduleId)}:{moduleId}");

        // Try Cached first
        // if we already have a list of shared sources, return that
        // as the sources don't change per request, but per module
        if (TryReuseFromCache(moduleId, out var cached))
            return l.Return(cached, $"reuse {cached.Sources.Count()} sources");

        var luEngine = BuildLookupEngine(moduleId);
        return l.Return(AddToCache(moduleId, luEngine), "created and cached for reuse");
    }

    /// <summary>
    /// Build a new lookup engine - to be overriden in Dnn and other implementations
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    protected virtual LookUpEngine BuildLookupEngine(int moduleId)
    {
        var l = Log.Fn<LookUpEngine>($"{nameof(moduleId)}:{moduleId}");
        var sources = AddHttpAndDiSources([]);
        var luEngine = new LookUpEngine(Log, sources: sources);
        //AddHttpAndDiSources(/*luEngine,*/ []).DoIfNotNull(luEngine.Add);
        return l.Return(luEngine);
    }

    /// <summary>
    /// Cache sources by module ID, so we don't have to re-create them every time.
    /// They remain the same throughout a request.
    /// </summary>
    protected readonly Dictionary<int, List<ILookUp>> SourcesByModuleId = [];

    protected LookUpEngine AddToCache(int moduleId, LookUpEngine engine)
    {
        SourcesByModuleId[moduleId] = [.. engine.Sources];
        return engine;
    }

    protected bool TryReuseFromCache(int moduleId, out LookUpEngine engine)
    {
        if (!SourcesByModuleId.TryGetValue(moduleId, out var sources))
        {
            engine = null;
            return false;
        }

        engine = new(Log, sources: sources);
        return true;
    }


    /// <summary>
    /// Get all http sources which are available in the current context.
    /// But only if they have not already been added to the list.
    /// - QueryString
    /// - Query
    /// - Form (Dnn only)
    /// </summary>
    /// <returns></returns>
    protected List<ILookUp> AddHttpAndDiSources(/*LookUpEngine existingList,*/ List<ILookUp> sources)
    {
        sources ??= [];
        var l = Log.Fn<List<ILookUp>>($"provider: {sources.Count}");

        l.A("Found Http-Context, will ty to add params for querystring, server etc.");

        // Prepare additions to return
        var additions = builtInSources.Value
            //.Where(lu => !existingList.HasSource(lu.Name))
            .Where(lu => !sources.HasSource(lu.Name))
            .ToList();

        // add "query" if it was not already added previously (Oqt has it) based on "querystring"
        //if (!existingList.HasSource(LookUpConstants.SourceQuery))
        if (!sources.HasSource(LookUpConstants.SourceQuery))
            additions
                .FirstOrDefault(lu => lu.Name.EqualsInsensitive(LookUpConstants.SourceQueryString))
                .DoIfNotNull(qsl => additions.Add(new LookUpInLookUps(LookUpConstants.SourceQuery, [qsl])));

        return l.Return(additions, $"{additions.Count} additions");
    }
}