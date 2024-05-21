using ToSic.Sxc.Services.Cache;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services;

/// <summary>
/// Experimental!
/// Cache service to help your code cache data.
///
/// It does quite a bit of magic, for example:
///
/// - scope the cache to the current App, so a key "data" will not bleed to other apps
/// - change the key when the data is for the current user only
/// - ...and more.
/// 
/// It's not yet fully documented.
/// </summary>
/// <remarks>
/// Basic use will just use a string key. The internal key will be more complex,
/// while advanced use would first create the specs using <see cref="CreateSpecs"/> and then use those specs for all operations.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v17.09")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICacheService
{
    /// <summary>
    /// Create cache specs for a specific key and optional segment.
    /// 
    /// This is used for complex setups where the same specs will be reused for multiple operations.
    /// </summary>
    /// <param name="key">The main cache key (name) to use. It will be extended internally, to prevent collisions, so it can be fairly short.</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="regionName">a cache region to segment the cache into multiple regions</param>
    /// <param name="shared">
    /// If set to `true` it will make this key available on other apps which access the data with allApps = `true`.
    /// By default, each app has its own region, preventing key collisions between apps.
    /// </param>
    /// <returns></returns>
    ICacheSpecs CreateSpecs(string key, NoParamOrder protector = default, string regionName = default, bool? shared = default);

    /// <summary>
    /// Check if the cache contains data for the given specs.
    /// </summary>
    bool Contains(ICacheSpecs specs);

    /// <summary>
    /// Check if the cache contains data for the given key.
    /// </summary>
    bool Contains(string key);

    /// <summary>
    /// Check if the cache contains data of specified type for the given specs.
    /// </summary>
    bool Contains<T>(ICacheSpecs specs);

    /// <summary>
    /// Check if the cache contains data of specified type for the given key.
    /// </summary>
    bool Contains<T>(string key);

    /// <summary>
    /// Get data from the cache of the given type for the given specs, with optional fallback.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback"></param>
    T Get<T>(ICacheSpecs specs, NoParamOrder protector = default, T fallback = default);

    /// <summary>
    /// Get data from the cache of the given type for the given key, with optional fallback.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback"></param>
    /// <returns></returns>
    T Get<T>(string key, NoParamOrder protector = default, T fallback = default);

    /// <summary>
    /// Get or set data in the cache for the given key, with optional generation and specs-tweaking.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="generate"></param>
    /// <returns></returns>
    T GetOrSet<T>(string key, NoParamOrder protector = default, Func<T> generate = default);

    /// <summary>
    /// Get or set data in the cache for the given specs, with optional generation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="generate"></param>
    /// <returns></returns>
    T GetOrSet<T>(ICacheSpecs specs, NoParamOrder protector = default, Func<T> generate = default);

    /// <summary>
    /// Try to get data of the specified type from the cache for the given specs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <param name="value"></param>
    /// <returns>`true` if found, `false` if not found</returns>
    bool TryGet<T>(ICacheSpecs specs, out T value);

    /// <summary>
    /// Try to get data of the specified type from the cache for the given key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>`true` if found, `false` if not found</returns>
    bool TryGet<T>(string key, out T value);

    /// <summary>
    /// Remove a cache entry.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The object if it was in the cache, otherwise null.</returns>
    object Remove(string key);

    /// <summary>
    /// Remove a cache entry.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The object if it was in the cache, otherwise null.</returns>
    object Remove(ICacheSpecs key);

    /// <summary>
    /// Set a value in the cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    void Set<T>(string key, T value, NoParamOrder protector = default);

    /// <summary>
    /// Set a value in the cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <param name="value"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    void Set<T>(ICacheSpecs specs, T value, NoParamOrder protector = default);
}