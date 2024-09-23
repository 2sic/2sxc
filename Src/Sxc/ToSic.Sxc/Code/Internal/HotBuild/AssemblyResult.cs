using System.Reflection;
using ToSic.Eav.Caching;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyResult(
    Assembly assembly = null,
    string errorMessages = null,
    string[] assemblyLocations = null,
    string safeClassName = null,
    Type mainType = default,
    Dictionary<string, string> infos = default): ICanBeCacheDependency, ITimestamped
{
    public Assembly Assembly { get; } = assembly;
    public string ErrorMessages { get; } = errorMessages;
    public string[] AssemblyLocations { get; } = assemblyLocations ?? []; // TODO: refactor this to not use array
    public string SafeClassName { get; } = safeClassName;

    /// <summary>
    /// The main type of this assembly - typically for Razor files which usually just publish a single type.
    /// This is to speed up performance, so the user of it doesn't need to find it again. 
    /// </summary>
    public Type MainType => mainType;

    /// <summary>
    /// The list of folders which must be watched for changes when using this assembly.
    /// ATM just used for AppCode assemblies, should maybe be in an inheriting class...
    /// </summary>
    // TODO: WIP - should be more functional, this get/set is still hacky
    public IDictionary<string, bool> WatcherFolders { get; internal set; }

    public Dictionary<string, string> Infos { get; } = infos ?? [];

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
    public string CacheDependencyId { get; set; }

    public long CacheTimestamp { get; } = DateTime.Now.Ticks;

    #endregion

}