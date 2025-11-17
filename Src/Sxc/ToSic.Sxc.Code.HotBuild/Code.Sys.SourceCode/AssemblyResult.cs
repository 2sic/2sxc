using System.Reflection;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Code.Sys.SourceCode;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AssemblyResult(Assembly? assembly = null): ICanBeCacheDependency, ITimestamped
{
    public Assembly? Assembly { get; } = assembly;
    public string? ErrorMessages { get; init; }
    public string[] AssemblyLocations { get; init; } = []; // TODO: refactor this to not use array
    public string? SafeClassName { get; init; }

    /// <summary>
    /// The main type of this assembly - typically for Razor files which usually just publish a single type.
    /// This is to speed up performance, so the user of it doesn't need to find it again. 
    /// </summary>
    public Type? MainType { get; set; }

    /// <summary>
    /// The list of folders which must be watched for changes when using this assembly.
    /// ATM just used for AppCode assemblies, should maybe be in an inheriting class...
    /// </summary>
    // TODO: WIP - should be more functional, this get/set is still hacky
    public IDictionary<string, bool>? WatcherFolders { get; internal set; }

    public Dictionary<string, string> Infos { get; init; } = [];

    /// <summary>
    /// True if an assembly was created without compile errors.
    /// </summary>
    public bool HasAssembly => Assembly != null;

    /// <summary>
    /// True if an assembly was not created and we get compile errors.
    /// </summary>
    public bool HasError => !HasAssembly && ErrorMessages.HasValue();

    /// <summary>
    /// True if there is a value, either an assembly or an error.
    /// </summary>
    public bool HasValue => HasAssembly || HasError;

    #region CacheDependency

    public bool CacheIsNotifyOnly => false;

    /// <summary>
    /// Used to create cache dependency with CacheEntryChangeMonitor
    /// </summary>
    /// <remarks>
    /// This cache item with Assembly usually has cache dependency on WatchFolders.
    /// Other cache items can depend on this cache item, instead of creating additional file monitors on same WatchFolders.
    ///
    /// WARNING: AS OF 2024-06-01 it uses the same name as the ICanBeCacheDependency, but ATM it's used without the prefix
    /// </remarks>
    public string CacheDependencyId { get; set; } = null!; // not sure when this is actually used; null! may be wrong.

    public long CacheTimestamp { get; } = DateTime.Now.Ticks;

    #endregion

    /// <summary>
    /// Optional cache dependency to attach when this assembly is reused (e.g., AppCode dependency for Razor cache).
    /// </summary>
    public ICanBeCacheDependency? AppCodeDependency { get; init; }

}
