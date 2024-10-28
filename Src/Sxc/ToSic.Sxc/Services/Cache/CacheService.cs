using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Caching;
using ToSic.Lib.DI;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services.Cache;

/// <summary>
/// WIP thoughts...
/// - policy variants
/// 
/// Also vary-by variants, like vary
/// - by user
/// - by culture
/// - by role
/// - by device
/// - by query
/// - by url
/// - by page
/// - by module
/// - by app, by tenant
/// - by site
/// - by domain
/// - by host
/// - by path
/// - by query
/// - by form
/// - by cookie
/// - by header
/// - by session, by cache, by request, by response, by server, by client, by browser, by os, by device...
/// </summary>
/// <param name="cache"></param>
internal class CacheService(MemoryCacheService cache, LazySvc<IAppReaderFactory> appStates, Generator<IAppPathsMicroSvc> appPathsLazy)
    : ServiceForDynamicCode($"{SxcLogName}.CchSvc", connect: [cache, appStates]), ICacheService
{
    /// <summary>
    /// AppId to use in key generation, so it won't collide with other apps.
    /// </summary>
    private int AppId => _appId ??= CodeApiSvc?.App.AppId ?? -1;
    private int? _appId;

    public ICacheSpecs CreateSpecs(string key, NoParamOrder protector = default, string regionName = default, bool? shared = default)
    {
        var l = Log.Fn<ICacheSpecs>($"Key: {key} / Segment: {regionName}");
        var keySpec = new CacheKeySpecs(shared == true ? CacheKeySpecs.NoApp : AppId, key, regionName);
        var specs = new CacheSpecs(Log, _CodeApiSvc, appStates, appPathsLazy, keySpec, cache.NewPolicyMaker());
        return l.Return(specs);
    }

    public bool Contains(ICacheSpecs specs)
        => cache.Contains(specs.Key);

    public bool Contains(string key)
        => cache.Contains(new CacheKeySpecs(AppId, key).Key);

    public bool Contains<T>(ICacheSpecs specs)
        => cache.TryGet<T>(specs.Key, out _);

    public bool Contains<T>(string key)
        => cache.TryGet<T>(new CacheKeySpecs(AppId, key).Key, out _);

    public T Get<T>(ICacheSpecs specs, NoParamOrder protector = default, T fallback = default) 
        => cache.Get(specs.Key, fallback);

    public T Get<T>(string key, NoParamOrder protector = default, T fallback = default) 
        => cache.Get(new CacheKeySpecs(AppId, key).Key, fallback);


    private ICacheSpecs ProcessSpecs(string key = default, Func<ICacheSpecs, ICacheSpecs> tweak = default)
    {
        if (key == default)
            throw new ArgumentException("key must be set");

        var specs = CreateSpecs(key);
        return tweak == null
            ? specs
            : tweak(specs);
    }

    public T GetOrSet<T>(string key, NoParamOrder protector = default, Func<T> generate = default)
        => GetOrSet(ProcessSpecs(key), generate: generate);

    public T GetOrSet<T>(ICacheSpecs specs, NoParamOrder protector = default, Func<T> generate = default)
    {
        if (cache.TryGet(specs.Key, out T value)) return value;
        
        if (generate == null) return default;
        var newValue = generate();

        cache.SetNew(specs.Key, newValue, specs.PolicyMaker);
        return newValue;
    }

    public bool TryGet<T>(ICacheSpecs specs, out T value)
        => cache.TryGet(specs.Key, out value);

    public bool TryGet<T>(string key, out T value)
        => cache.TryGet(new CacheKeySpecs(AppId, key).Key, out value);

    public object Remove(string key)
        => cache.Remove(new CacheKeySpecs(AppId, key).Key);

    public object Remove(ICacheSpecs specs)
        => cache.Remove(specs.Key);

    public void Set<T>(string key, T value, NoParamOrder protector = default)
        => Set(ProcessSpecs(key), value);

    public void Set<T>(ICacheSpecs specs, T value, NoParamOrder protector = default)
        => cache.SetNew(specs.Key, value, specs.PolicyMaker);
}