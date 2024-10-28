using ToSic.Eav.Caching;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Services.Cache;

/// <summary>
/// Experimental!
/// Cache Specs which determine how the key is generated, additional dependencies and expiration policies.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v17.09")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICacheSpecs
{
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    string Key { get; }

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IPolicyMaker PolicyMaker { get; }

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
    /// <param name="slidingExpiration"></param>
    /// <returns></returns>
    /// <remarks>
    /// If neither absolute nor sliding are set, a sliding expiration of 1 hour will be used.
    /// Setting both is invalid and will throw an exception.
    /// </remarks>
    ICacheSpecs SetSlidingExpiration(TimeSpan slidingExpiration);

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

    /// <summary>
    /// Vary the cache by page, so that each page has its own cache.
    /// By default, it will take the current page, but you can optionally specify a custom page or ID.
    /// </summary>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="id">optional page id to use instead of the default</param>
    /// <param name="page">optional page object to use instead of the default</param>
    /// <returns></returns>
    ICacheSpecs VaryByPage(NoParamOrder protector = default, ICmsPage page = default, int? id = default);

    /// <summary>
    /// Vary the cache by module, so that each module has its own cache.
    /// By default, it will take the current module, but you can optionally specify a custom module or ID.
    /// </summary>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="id">optional page id to use instead of the default</param>
    /// <param name="module">optional module object to use instead of the default</param>
    ICacheSpecs VaryByModule(NoParamOrder protector = default, ICmsModule module = default, int? id = default);

    /// <summary>
    /// Vary the cache by user, so that each user has its own cache.
    /// By default, it will take the current user, but you can specify a custom user or ID.
    /// </summary>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="id">optional page id to use instead of the default</param>
    /// <param name="user">optional module object to use instead of the default</param>
    /// <returns></returns>
    ICacheSpecs VaryByUser(NoParamOrder protector = default, ICmsUser user = default, int? id = default);

    /// <summary>
    /// Vary the cache by a specific page parameter, like `?category=1`.
    /// Using this method will only vary the cache by this specific parameter and ignore the rest.
    /// </summary>
    /// <param name="name">Name of a single parameter</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="caseSensitive">Determines if the value should be treated case-sensitive, default is `false`</param>
    /// <returns></returns>
    ICacheSpecs VaryByPageParameter(string name, NoParamOrder protector = default, bool caseSensitive = false);

    /// <summary>
    /// Vary the cache by one or more specific page parameter, like `?category=1&amp;sort=asc`.
    /// Using this method will only vary the cache by the mentioned parameters and ignore the rest.
    /// </summary>
    /// <param name="names">Names of one or more parameters, comma-separated</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="caseSensitive">Determines if the value should be treated case-sensitive, default is `false`</param>
    ICacheSpecs VaryByPageParameters(string names = default, NoParamOrder protector = default, bool caseSensitive = false);

    /// <summary>
    /// Vary the cache by parameters which may not come from the page. 
    /// </summary>
    /// <param name="parameters">parameters object</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="names">Names of one or more parameters, comma-separated</param>
    /// <param name="caseSensitive">Determines if the value should be treated case-sensitive, default is `false`</param>
    /// <returns></returns>
    ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder protector = default, string names = default, bool caseSensitive = false);

    #endregion

}