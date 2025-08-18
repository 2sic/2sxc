using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sys.Caching.Policies;

namespace ToSic.Sxc.Services.Cache;

/// <summary>
/// Cache Specs contain the definition of what the cached data should depend on, how long it should be cached and how the cache key should be generated.
/// It uses a fluent API to continue adding rules / expirations / dependencies.
/// Internally this is then used to create a cache policy.
/// </summary>
/// <remarks>
/// * Introduced as experimental in v17.09
/// * Released in 19.01
/// </remarks>
[PublicApi]
public interface ICacheSpecs
{
    /// <summary>
    /// The final cache key.
    /// </summary>
    /// <remarks>
    /// This services will always add a prefix to the key, to avoid conflicts with other cache keys.
    /// The Key itself is only ever needed if you want to see a key manually, mainly for debugging purposes.
    /// </remarks>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    string Key { get; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    internal IPolicyMaker PolicyMaker { get; }

    /// <summary>
    /// Determine if caching is enabled. Default is `true`.
    /// </summary>
    [WorkInProgressApi("wip v20.00-05")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    bool IsEnabled { get; }

    /// <summary>
    /// Disable caching for this data, so it will not be cached. Rarely used. 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This rarely makes sense, since it's better to just not add something to the cache in the first place.
    /// 
    /// WIP v20.01: This is a work in progress, and the implementation may change in future versions.
    /// </remarks>
    public ICacheSpecs Disable(NoParamOrder protector = default, UserElevation minElevation = default, UserElevation maxElevation = default);

    /// <summary>
    /// Disable caching for this data, so it will not be cached. Rarely used. 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// WIP v20.01: This is a work in progress, and the implementation may change in future versions.
    /// </remarks>
    public ICacheSpecs Enable();

    /// <summary>
    /// Set absolute expiration, alternative is sliding expiration.
    /// If neither are set, a sliding expiration of 1 hour will be used.
    /// </summary>
    /// <param name="absoluteExpiration"></param>
    /// <returns></returns>
    /// <remarks>
    /// If neither absolute nor sliding are set, a sliding expiration of 1 hour will be used.
    /// Setting both is invalid and will throw an exception.
    /// </remarks>
    ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration);

    /// <summary>
    /// Set sliding expiration, alternative is absolute expiration.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="seconds">time in seconds - if specified, takes precedence - new v20.01</param>
    /// <returns></returns>
    /// <remarks>
    /// If neither absolute nor sliding are set, a sliding expiration of 1 hour will be used.
    /// Setting both is invalid and will throw an exception.
    /// </remarks>
    ICacheSpecs SetSlidingExpiration(TimeSpan? timeSpan = default, NoParamOrder protector = default, int? seconds = null);

    /// <summary>
    /// Depend on the app data, so if any data changes, the cache will be invalidated.
    /// </summary>
    /// <returns></returns>
    ICacheSpecs WatchAppData(NoParamOrder protector = default);

    /// <summary>
    /// Depend on the app folder, so if any file in the app folder changes, the cache will be invalidated. WIP!
    /// </summary>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="withSubfolders">should it also watch subfolders? default is `true`</param>
    /// <returns></returns>
    ICacheSpecs WatchAppFolder(NoParamOrder protector = default, bool? withSubfolders = default);

    ///// <summary>
    ///// Add files
    ///// </summary>
    ///// <param name="filePaths"></param>
    ///// <returns></returns>
    //ICacheSpecs WatchFiles(IEnumerable<string> filePaths);

    #region VaryBy

    /// <summary>
    /// Vary the cache by a specific name and value.
    /// All cache items where this value is the same, will be considered the same.
    /// For example, this could be a category name or something where the data for this category is always the same.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    ICacheSpecs VaryBy(string name, string value, NoParamOrder protector = default, bool caseSensitive = false);

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    internal CacheConfig Configuration { get; }

    /// <summary>
    /// Vary the cache by a specific name and value.
    /// All cache items where this value is the same, will be considered the same.
    /// For example, this could be a category name or something where the data for this category is always the same.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ICacheSpecs VaryBy(string name, int value);

    /// <summary>
    /// Vary the cache by the _current_ module, so that each module has its own cache.
    /// </summary>
    ICacheSpecs VaryByModule();

    /// <summary>
    /// Vary the cache by module, so that each module has its own cache.
    /// </summary>
    /// <param name="id">Module id to use</param>
    ICacheSpecs VaryByModule(int id);

    // Note: I don't think there is great value in providing ICms... overloads, so comment out to prevent next person from creating them again
    ///// <summary>
    ///// Vary the cache by module, so that each module has its own cache.
    ///// </summary>
    ///// <param name="module">module to use</param>
    //ICacheSpecs VaryByModule(ICmsModule module);

    /// <summary>
    /// Vary the cache by the _current_ page, so that each page has its own cache.
    /// </summary>
    /// <returns></returns>
    ICacheSpecs VaryByPage();

    /// <summary>
    /// Vary the cache by page, so that each page has its own cache.
    /// By default, it will take the current page, but you can optionally specify a custom page or ID.
    /// </summary>
    /// <param name="id">page id to use</param>
    /// <returns></returns>
    ICacheSpecs VaryByPage(int id);

    // Note: I don't think there is great value in providing ICms... overloads, so comment out to prevent next person from creating them again
    ///// <summary>
    ///// Vary the cache by page, so that each page has its own cache.
    ///// By default, it will take the current page, but you can optionally specify a custom page or ID.
    ///// </summary>
    ///// <param name="page">page object to use</param>
    ///// <returns></returns>
    //ICacheSpecs VaryByPage(ICmsPage page);

    /// <summary>
    /// Vary the cache by _current_ user, so that each user has its own cache.
    /// </summary>
    /// <returns></returns>
    ICacheSpecs VaryByUser();

    /// <summary>
    /// Vary the cache by user, so that each user has its own cache.
    /// </summary>
    /// <param name="id">User id to use</param>
    /// <returns></returns>
    ICacheSpecs VaryByUser(int id);

    // Note: I don't think there is great value in providing ICms... overloads, so comment out to prevent next person from creating them again
    ///// <summary>
    ///// Vary the cache by user, so that each user has its own cache.
    ///// </summary>
    ///// <param name="user">user object to use</param>
    ///// <returns></returns>
    //ICacheSpecs VaryByUser(ICmsUser user);

    /// <summary>
    /// Vary the cache by one or more specific page parameter, like `?category=1` or `?category=1&amp;sort=asc`.
    /// Using this method will only vary the cache by the mentioned parameters and ignore the rest.
    /// </summary>
    /// <param name="names">Names of one or more parameters, comma-separated. If null, all parameters are used, if `""`, no parameters are used.</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="caseSensitive">Determines if the value should be treated case-sensitive, default is `false`</param>
    ICacheSpecs VaryByPageParameters(string? names = default, NoParamOrder protector = default, bool caseSensitive = false);

    /// <summary>
    /// Vary the cache by a custom parameters list.
    /// </summary>
    /// <param name="parameters">parameters object</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="names">Names of one or more parameters, comma-separated</param>
    /// <param name="caseSensitive">Determines if the value should be treated case-sensitive, default is `false`</param>
    /// <returns></returns>
    /// <remarks>
    /// 2025-08-18 v20.00-05 made internal; testing only. - as it cannot make sense - since the parameters won't be available during following cache-checks
    /// </remarks>
    internal ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder protector = default, string? names = default, bool caseSensitive = false);

    #endregion

    #region Vary By Model EXPERIMENTAL

    /// <summary>
    /// Vary the cache by values in the model, so that each sample has its own cache. Used in Partial-Caching only. WORK-IN-PROGRESS!
    /// </summary>
    /// <param name="names">Names of one or more parameters, comma-separated</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="caseSensitive">Determines if the value should be treated case-sensitive, default is `false`</param>
    /// <returns></returns>
    /// <remarks>
    /// This is only meant for partial razor caching, since that would have a model available.
    /// If used elsewhere, it will be ignored.
    /// 
    /// WIP v20.01
    /// </remarks>
    [WorkInProgressApi("WIP v20.01")]
    ICacheSpecs VaryByModel(string? names = default, NoParamOrder protector = default, bool caseSensitive = false);

    #endregion
}