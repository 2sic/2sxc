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
    /// Set absolute expiration
    /// </summary>
    /// <param name="absoluteExpiration"></param>
    /// <returns></returns>
    ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration);
    
    /// <summary>
    /// Set sliding expiration
    /// </summary>
    /// <param name="slidingExpiration"></param>
    /// <returns></returns>
    ICacheSpecs SetSlidingExpiration(TimeSpan slidingExpiration);

    ICacheSpecs VaryByUser(ICmsUser user = default);

    ICacheSpecs WatchApp();

    ///// <summary>
    ///// Add files
    ///// </summary>
    ///// <param name="filePaths"></param>
    ///// <returns></returns>
    //ICacheSpecs WatchFiles(IEnumerable<string> filePaths);
}