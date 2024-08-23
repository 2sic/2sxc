using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HotBuildSpec(int appId, string edition, string appName)
{
    public int AppId => appId;

    public string Edition => edition;

    public string EditionToLog => $"/{Edition}"; // for logging

    /// <summary>
    /// AppName for logging - ATM no other purpose, so it can be null
    /// </summary>
    public string AppName => appName;

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => _toString ??= $"{nameof(HotBuildSpec)} - {nameof(AppId)}: {appId} {(appName.HasValue() ? $"({appName})" : "")}; {nameof(Edition)}: '{EditionToLog}'";
    private string _toString;

    /// <summary>
    /// Create a dictionary of the specs for logging
    /// </summary>
    public IDictionary<string, string> ToDictionary() => new Dictionary<string, string>
    {
        { nameof(AppId), AppId.ToString() },
        { nameof(AppName), AppName ?? ""},
        { nameof(Edition), EditionToLog },
    };

    /// <summary>
    /// CacheKey for this spec
    /// </summary>
    /// <remarks>
    /// should not use optional parameters like: addSharedSuffixToAssemblyName or appName
    /// </remarks>
    public string CacheKey() => _cacheKey ??= $"{nameof(HotBuildSpec)}.{nameof(AppId)}:{AppId}.{nameof(Edition)}:{Edition}";
    private string _cacheKey;

    /// <summary>
    /// Use when fallback from edition to root app
    /// </summary>
    /// <returns></returns>
    public HotBuildSpec CloneWithoutEdition() => new(AppId, null, appName);

    /// <summary>
    /// Use in very special case for AppCode in site local path
    /// need temp name assembly without shared suffix
    /// </summary>
    /// <returns></returns>
    public HotBuildSpecWithSharedSuffix WithoutSharedSuffix() => new(AppId, edition, appName, false);

    /// <summary>
    /// Use in very special case for AppCode in shared (global) path
    /// need temp name assembly with shared suffix
    /// </summary>
    /// <returns></returns>
    public HotBuildSpecWithSharedSuffix WithSharedSuffix() => new(AppId, edition, appName, true);
}


public class HotBuildSpecWithSharedSuffix(int appId, string edition, string appName, bool addSharedSuffixToAssemblyName) : HotBuildSpec(appId, edition, appName)
{
    /// <summary>
    /// "addSharedSuffixToAssemblyName" is optional parameter used just as info for AppCode assembly naming in very special case,
    /// it has no other purpose, and should not be used for anything else easily
    /// </summary>
    /// <remarks>
    /// Take a care that AppCode assembly could be only one per app,
    /// and that is located in local app folder per site or in shared location,
    /// contrary to editions are separate apps (one per edition) again on "local" or "shared" path.
    /// </remarks>
    public string SharedSuffix => addSharedSuffixToAssemblyName ? "Shared" : "";

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => _toString ??= $"{base.ToString()}; {nameof(SharedSuffix)}: '{SharedSuffix}'";
    private string _toString;

    /// <summary>
    /// Create a dictionary of the specs for logging
    /// </summary>
    public new IDictionary<string, string> ToDictionary()
    {
        var dictionary = base.ToDictionary();
        dictionary.Add(nameof(SharedSuffix), SharedSuffix);
        return dictionary;
    }
}