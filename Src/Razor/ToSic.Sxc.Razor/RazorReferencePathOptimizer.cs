using System.Reflection;

namespace ToSic.Sxc.Razor;

/// <summary>
/// Cleans the metadata-reference path list used by Roslyn for dynamic Razor/AppCode compilation.
///
/// Why this exists:
/// Oqtane major upgrades can leave old framework/runtime DLLs in the application root while the upgraded
/// installation also contains the correct compile reference assemblies in the /refs folder. This is not a
/// problem for the normal .NET runtime loader in every case, but it is a problem for Roslyn: Razor runtime
/// compilation receives a flat list of physical DLL paths and treats them all as compile references.
///
/// Typical bad states we have seen:
/// 1. Root contains Microsoft.AspNetCore.Authorization.dll v9 and /refs contains v10.
///    Razor then fails with CS0433 because AllowAnonymousAttribute exists in both assemblies.
/// 2. A root DLL was deleted as cleanup, but an ApplicationPart still reports its old path.
///    Roslyn metadata creation then fails because the referenced file no longer exists.
/// 3. Runtime implementation assemblies such as System.Private.CoreLib/System.Private.Xml are mixed
///    with reference assemblies such as System.Runtime/System.Xml.ReaderWriter from /refs.
///    Roslyn then reports CS0518 predefined types missing and CS0433 duplicate framework types.
///
/// Important boundary:
/// This is intentionally compile-reference cleanup only. It does not delete Oqtane files, does not repair
/// the host installation, and does not change runtime assembly loading. It only decides what 2sxc exposes
/// to Roslyn while compiling dynamic Razor views and AppCode.
///
/// Policy:
/// - Ignore empty and missing paths. Roslyn MetadataReference requires an existing physical DLL.
/// - Group readable assemblies by assembly identity: name, culture, public key token.
/// - Keep the highest assembly version for duplicate identities.
/// - If duplicate identities have the same version, prefer reference-assembly paths (/refs or /ref).
///   Reference assemblies are the intended input for compilation, while root files are usually runtime
///   implementation assemblies.
/// - Preserve unreadable/non-assembly paths individually instead of collapsing them by file name.
/// - When a /refs System.Runtime.dll is available, remove System.Private.* implementation assemblies.
///   The System.Private.* assemblies belong to runtime execution, not compile references, and mixing
///   them with the facade/reference assembly set causes noisy framework-type conflicts.
/// </summary>
internal static class RazorReferencePathOptimizer
{
    private static readonly Version NoVersion = new(0, 0);

    internal static IReadOnlyList<string> PreferCompileReferences(IEnumerable<string> referencePaths)
        => PreferCompileReferences(referencePaths, File.Exists, TryGetAssemblyName);

    internal static IReadOnlyList<string> PreferCompileReferences(
        IEnumerable<string> referencePaths,
        Func<string, AssemblyName?> getAssemblyName)
        => PreferCompileReferences(referencePaths, _ => true, getAssemblyName);

    internal static IReadOnlyList<string> PreferCompileReferences(
        IEnumerable<string> referencePaths,
        Func<string, bool> pathExists,
        Func<string, AssemblyName?> getAssemblyName)
    {
        if (referencePaths == null! /* paranoid */)
            return [];

        // First normalize the raw list into entries with assembly metadata.
        // We deliberately check File.Exists before reading metadata:
        // ApplicationParts may keep old paths cached after cleanup deleted a DLL from the Oqtane root.
        // Passing such a missing path to ModuleMetadata.CreateFromStream would fail before Roslyn
        // even reaches compilation, so missing files must be filtered here.
        var entries = referencePaths
            .Where(path => !string.IsNullOrWhiteSpace(path) && pathExists(path))
            .Select((path, index) => Create(path, index, getAssemblyName))
            .ToList();

        // If the /refs reference assembly set is present, keep that compile-time view of the framework.
        // System.Private.* assemblies are runtime implementation assemblies. They are loaded and useful
        // at runtime, but they are not the right Roslyn references when System.Runtime and friends from
        // /refs are also present. Mixing both worlds causes duplicate framework types and missing
        // predefined types, for example:
        // - List<T> in System.Collections and System.Private.CoreLib
        // - XmlElement in System.Xml.ReaderWriter and System.Private.Xml
        // - CS0518 for System.Object/System.Void/System.String
        if (entries.Any(entry => entry.IsSystemRuntimeReference))
            entries = entries
                .Where(entry => !entry.IsSystemPrivateAssembly)
                .ToList();

        // Now collapse true duplicate assembly identities. The key intentionally ignores file path and
        // version but includes name/culture/public-key-token. That allows us to choose between
        // Microsoft.AspNetCore.Authorization v9 in the root and v10 in /refs as "same assembly family",
        // while keeping unrelated assemblies with the same file name but different strong identity apart.
        return entries
            .GroupBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
            .Select(PickBest)
            .OrderBy(entry => entry.Index)
            .Select(entry => entry.Path)
            .ToList()
            .AsReadOnly();
    }

    private static ReferencePathEntry Create(string path, int index, Func<string, AssemblyName?> getAssemblyName)
    {
        var assemblyName = SafeGetAssemblyName(path, getAssemblyName);

        // If the file is not a readable .NET assembly, keep it as its own path-keyed entry.
        // This is conservative: we do not want a helper meant for framework DLL conflicts to remove
        // custom references we do not fully understand.
        return assemblyName == null
            ? new(path, index, $"path:{path}", "", NoVersion, false)
            : new(path, index, AssemblyKey(assemblyName), assemblyName.Name ?? "", assemblyName.Version ?? NoVersion, IsReferenceAssemblyPath(path));
    }

    private static AssemblyName? SafeGetAssemblyName(string path, Func<string, AssemblyName?> getAssemblyName)
    {
        try
        {
            return getAssemblyName(path);
        }
        catch
        {
            return null;
        }
    }

    private static AssemblyName? TryGetAssemblyName(string path)
    {
        try
        {
            return AssemblyName.GetAssemblyName(path);
        }
        catch
        {
            return null;
        }
    }

    private static ReferencePathEntry PickBest(IEnumerable<ReferencePathEntry> entries)
        => entries
            // Highest version wins, so stale .NET 9 root assemblies lose to .NET 10 refs assemblies.
            .OrderByDescending(entry => entry.Version)
            // Same version tie-breaker: prefer compile reference assemblies over runtime root assemblies.
            .ThenByDescending(entry => entry.IsReferenceAssemblyPath)
            // Stable final tie-breaker so the output is deterministic if two equivalent paths remain.
            .ThenBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase)
            .First();

    private static string AssemblyKey(AssemblyName assemblyName)
    {
        var publicKeyToken = assemblyName.GetPublicKeyToken();
        var token = publicKeyToken is { Length: > 0 }
            ? Convert.ToHexString(publicKeyToken)
            : "";

        return $"{assemblyName.Name}|{assemblyName.CultureName ?? ""}|{token}";
    }

    private static bool IsReferenceAssemblyPath(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (directory == null)
            return false;

        return directory
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(part =>
                part.Equals("refs", StringComparison.OrdinalIgnoreCase)
                || part.Equals("ref", StringComparison.OrdinalIgnoreCase));
    }

    private sealed record ReferencePathEntry(
        string Path,
        int Index,
        string Key,
        string AssemblyName,
        Version Version,
        bool IsReferenceAssemblyPath)
    {
        public bool IsSystemRuntimeReference
            => IsReferenceAssemblyPath && AssemblyName.Equals("System.Runtime", StringComparison.OrdinalIgnoreCase);

        public bool IsSystemPrivateAssembly
            => AssemblyName.StartsWith("System.Private.", StringComparison.OrdinalIgnoreCase);
    }
}
