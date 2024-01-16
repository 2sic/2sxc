using System.Reflection;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyResult(
    Assembly assembly = null,
    string errorMessages = null,
    string[] assemblyLocations = null,
    string safeClassName = null,
    Type mainType = default, 
    Dictionary<string, string> infos = default)
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
    // WIP - should be more functional, this get/set is still hacky
    public IList<string> WatcherFolders { get; set; }

    public Dictionary<string, string> Infos { get; } = infos ?? [];

    /// <summary>
    /// True if an assembly was created without compile errors.
    /// </summary>
    public bool HasAssembly => Assembly != null;
    
}