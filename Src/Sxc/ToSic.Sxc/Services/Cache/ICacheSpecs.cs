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

    ICacheSpecs WatchAppFolder(NoParamOrder protector = default, bool? withSubfolders = true);

    ///// <summary>
    ///// Add files
    ///// </summary>
    ///// <param name="filePaths"></param>
    ///// <returns></returns>
    //ICacheSpecs WatchFiles(IEnumerable<string> filePaths);

    #region VaryBy

    // Disabled for now, not sure if this single-value case makes sense
    ///// <summary>
    ///// Vary the cache by a specific value.
    ///// All cache items where this value is the same, will be considered the same.
    ///// For example, this could be a category name or something where the data for this category is always the same.
    ///// </summary>
    ///// <param name="value"></param>
    ///// <param name="protector"></param>
    ///// <param name="caseSensitive"></param>
    ///// <returns></returns>
    //ICacheSpecs VaryBy(string value, NoParamOrder protector = default, bool caseSensitive = false);

    /// <summary>
    /// Vary the cache by a specific name and value.
    /// All cache items where this value is the same, will be considered the same.
    /// For example, this could be a category name or something where the data for this category is always the same.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="protector"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    ICacheSpecs VaryBy(string name, string value, NoParamOrder protector = default, bool caseSensitive = false);

    //ICacheSpecs VaryByModule(int id);

    ICacheSpecs VaryByPage(NoParamOrder protector = default, ICmsPage page = default, int? id = default);

    ICacheSpecs VaryByModule(NoParamOrder protector = default, ICmsModule module = default, int? id = default);

    ICacheSpecs VaryByUser(NoParamOrder protector = default, ICmsUser user = default, int? id = default);

    //ICacheSpecs VaryByPage(int id);
    //ICacheSpecs VaryByPage(ICmsPage page = default);
    ICacheSpecs VaryByPageParameter(string name, NoParamOrder protector = default, bool caseSensitive = false);

    ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder protector = default, string names = default, bool caseSensitive = false);
    //ICacheSpecs VaryByUser(int id);
    //ICacheSpecs VaryByUser(ICmsUser user = default);

    #endregion

}