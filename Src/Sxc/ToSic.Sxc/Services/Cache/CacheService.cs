using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Caching;
using ToSic.Lib.DI;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services.Cache;

/// <summary>
/// WIP thoughts...
/// - policy variants
/// - also vary-by variants, eg. vary by user, by culture, by role, by device, by query, by url, by page, by module, by app, by tenant, by site, by domain, by host, by path, by query, by form, by cookie, by header, by session, by cache, by request, by response, by server, by client, by browser, by os, by device, by location, by time, by date, by day, by month, by year, by week, by hour, by minute, by second, by millisecond, by timezone, by language, by currency, by country, by region, by state, by city, by zip, by postal, by address, by phone, by fax, by email, by name, by title, by description, by keyword, by tag, by category, by group, by list, by array, by object, by property, by field, by column, by row, by table, by view, by form, by control, by input, by button, by link, by image, by icon, by logo, by text, by number, by integer, by float, by decimal, by boolean
/// </summary>
/// <param name="cache"></param>
internal class CacheService(MemoryCacheService cache, LazySvc<IAppStates> appStates, Generator<IAppPathsMicroSvc> appPathsLazy) : ServiceForDynamicCode($"{SxcLogName}.CchSvc", connect: [cache, appStates]), ICacheService
{
    /// <summary>
    /// Create cache specs for a specific key and optional segment.
    ///
    /// This is used for complex setups where the same specs will be reused for multiple operations.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="protector"></param>
    /// <param name="segment"></param>
    /// <returns></returns>
    public ICacheSpecs CreateSpecs(string key, NoParamOrder protector = default, string segment = default)
    {
        var l = Log.Fn<ICacheSpecs>($"Key: {key} / Segment: {segment}");
        var specs = new CacheSpecs(Log, _CodeApiSvc, appStates, appPathsLazy, new(key, segment), cache.NewPolicyMaker());
        return l.Return(specs);
    }

    public bool Contains(ICacheSpecs specs)
        => cache.Contains(specs.Key);

    public bool Contains(string key)
        => cache.Contains(new CacheKeySpecs(key).Key);

    public bool Contains<T>(ICacheSpecs specs)
        => cache.TryGet<T>(specs.Key, out _);

    public bool Contains<T>(string key)
        => cache.TryGet<T>(new CacheKeySpecs(key).Key, out _);

    public T Get<T>(ICacheSpecs specs, NoParamOrder protector = default, T fallback = default) 
        => cache.Get(specs.Key, fallback);

    public T Get<T>(string key, NoParamOrder protector = default, T fallback = default) 
        => cache.Get(new CacheKeySpecs(key).Key, fallback);


    private ICacheSpecs ProcessSpecs(string key = default, Func<ICacheSpecs, ICacheSpecs> tweak = default)
    {
        if (key == default)
            throw new ArgumentException("key must be set");

        var specs = CreateSpecs(key);
        return tweak == null
            ? specs
            : tweak(specs);
    }

    public T GetOrSet<T>(string key, NoParamOrder protector = default, Func<T> generate = default, Func<ICacheSpecs, ICacheSpecs> tweak = default)
        => GetOrSet(ProcessSpecs(key ?? throw new ArgumentException("key must be set"), tweak: tweak), generate: generate);

    public T GetOrSet<T>(ICacheSpecs specs, NoParamOrder protector = default, Func<T> generate = default)
    {
        if (cache.TryGet(specs.Key, out T value)) return value;
        
        if (generate == null) return default;
        var newValue = generate();

        cache.SetNew(specs.Key, newValue, _ => specs.PolicyMaker);
        return newValue;
    }

    public bool TryGet<T>(ICacheSpecs specs, out T value)
        => cache.TryGet(specs.Key, out value);

    public bool TryGet<T>(string key, out T value)
        => cache.TryGet(new CacheKeySpecs(key).Key, out value);

    public object Remove(string key)
        => cache.Remove(new CacheKeySpecs(key).Key);

    public object Remove(ICacheSpecs specs)
        => cache.Remove(specs.Key);

    public void Set<T>(string key, T value, NoParamOrder protector = default, Func<ICacheSpecs, ICacheSpecs> tweak = default)
        => Set(ProcessSpecs(key ?? throw new ArgumentException("key must be set"), tweak: tweak), value);

    public void Set<T>(ICacheSpecs specs, T value, NoParamOrder protector = default)
        => cache.SetNew(specs.Key, value, _ => specs.PolicyMaker);
}