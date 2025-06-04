using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Caching;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.DI;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Caching;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Services.Cache;

/// <summary>
/// WIP thoughts...
/// - policy variants
/// 
/// Also vary-by variants, like vary
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
/// - by cookie
/// - by header
/// - by session, by cache, by request, by response, by server, by client, by browser, by os, by device...
/// </summary>
/// <param name="cache"></param>
internal class CacheService(
    MemoryCacheService cache,
    LazySvc<IAppReaderFactory> appReaders,
    Generator<IAppPathsMicroSvc> appPathsLazy,
    ISysFeaturesService features
    ) : ServiceWithContext($"{SxcLogName}.CchSvc", connect: [cache, appReaders]), ICacheService
{
    /// <summary>
    /// AppId to use in key generation, so it won't collide with other apps.
    /// </summary>
    private int AppId => _appId ??= ExCtxOrNull?.GetApp().AppId ?? -1;
    private int? _appId;

    private bool IsEnabled => _isEnabled ??= features.IsEnabled(SxcFeatures.SmartDataCache);
    private bool? _isEnabled;

    public ICacheSpecs CreateSpecs(string key, NoParamOrder protector = default, string regionName = default, bool? shared = default)
    {
        var l = Log.Fn<ICacheSpecs>($"Key: {key} / Segment: {regionName}");
        var keySpecs = new CacheKeySpecs(shared == true ? CacheKeySpecs.NoApp : AppId, key, regionName);
        var specs = new CacheSpecs
        {
            AppPathsLazy = appPathsLazy,
            AppReaders = appReaders,
            ExCtx = ExCtx,
            KeySpecs = keySpecs,
            PolicyMaker = cache.NewPolicyMaker(),
        };
        return l.Return(specs);
    }

    public bool Contains(ICacheSpecs specs)
        => IsEnabled && cache.Contains(specs.Key);

    //public bool Contains(string key)
    //    => cache.Contains(new CacheKeySpecs(AppId, key).Key);

    public bool Contains<T>(ICacheSpecs specs)
        => IsEnabled && cache.TryGet<T>(specs.Key, out _);

    //public bool Contains<T>(string key)
    //    => cache.TryGet<T>(new CacheKeySpecs(AppId, key).Key, out _);

    public T Get<T>(ICacheSpecs specs, NoParamOrder protector = default, T fallback = default) 
        => IsEnabled ? cache.Get(specs.Key, fallback) : fallback;

    public T GetOrSet<T>(ICacheSpecs specs, NoParamOrder protector = default, Func<T> generate = default)
    {
        if (!IsEnabled)
            return generate == null ? default : generate();
        if (cache.TryGet(specs.Key, out T value)) return value;
        
        if (generate == null) return default;
        var newValue = generate();

        cache.Set(specs.Key, newValue, specs.PolicyMaker);
        return newValue;
    }

    public bool TryGet<T>(ICacheSpecs specs, out T value)
    {
        if (IsEnabled)
            return cache.TryGet(specs.Key, out value);
        value = default;
        return false;
    }

    //public bool TryGet<T>(string key, out T value)
    //    => cache.TryGet(new CacheKeySpecs(AppId, key).Key, out value);

    //public object Remove(string key)
    //    => cache.Remove(new CacheKeySpecs(AppId, key).Key);

    public object Remove(ICacheSpecs specs) =>
        IsEnabled ? cache.Remove(specs.Key) : null;

    //public void Set<T>(string key, T value, NoParamOrder protector = default)
    //    => Set(ProcessSpecs(key), value);

    public void Set<T>(ICacheSpecs specs, T value, NoParamOrder protector = default)
    {
        if (!IsEnabled) return;
        cache.Set(specs.Key, value, specs.PolicyMaker);
    }
}