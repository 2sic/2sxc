using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using ToSic.Eav.Sys;

namespace ToSic.Sxc.Code.Sys.HotBuild;

/// <summary>
/// Helper that discovers compile-time references defined inside app extensions.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExtensionCompileReferenceReader
{
    private const string CompileConfigFileName = "compile.json.resources";

    private static readonly JsonDocumentOptions ConfigJsonOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    private static readonly ConcurrentDictionary<string, CompileConfigCache> ConfigCache = new(StringComparer.OrdinalIgnoreCase);

    public readonly struct ExtensionReference(string value, string extensionFolder)
    {
        public string Value { get; } = value;

        public string ExtensionFolder { get; } = extensionFolder;
    }

    private sealed record CompileConfigCache(DateTime LastWriteUtc, IReadOnlyList<string> NetCoreReferences, IReadOnlyList<string> NetFrameworkReferences);

    /// <summary>
    /// Load extension reference definitions underneath the AppCode/Extensions folder that contains <paramref name="startPath"/>.
    /// </summary>
    public static IReadOnlyList<ExtensionReference> GetReferences(string? startPath, bool netFramework)
    {
        var appCodeFolder = FindAppCodeFolder(startPath);
        if (appCodeFolder.IsEmpty())
            return Array.Empty<ExtensionReference>();

        var extensionsRoot = Path.Combine(appCodeFolder, FolderConstants.AppExtensionsFolder);
        if (!Directory.Exists(extensionsRoot))
            return Array.Empty<ExtensionReference>();

        var references = new List<ExtensionReference>();
        foreach (var extensionDir in Directory.EnumerateDirectories(extensionsRoot))
        {
            var configPath = Path.Combine(extensionDir, CompileConfigFileName);
            if (!File.Exists(configPath))
                continue;

            var config = LoadConfig(configPath);
            var entries = netFramework ? config.NetFrameworkReferences : config.NetCoreReferences;
            foreach (var entry in entries)
            {
                if (!entry.HasValue())
                    continue;

                references.Add(new(entry.Trim(), extensionDir));
            }
        }

        return references;
    }

    public static bool IsAssemblyName(string referenceValue)
    {
        if (referenceValue.IsEmpty())
            return false;

        if (Path.IsPathRooted(referenceValue))
            return false;

        return !referenceValue.Contains(Path.DirectorySeparatorChar)
               && !referenceValue.Contains(Path.AltDirectorySeparatorChar);
    }

    internal static string NormalizeAssemblyName(string referenceValue)
    {
        if (referenceValue.IsEmpty())
            return referenceValue;

        return referenceValue.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
            ? referenceValue.Substring(0, referenceValue.Length - 4)
            : referenceValue;
    }

    public static string? ResolveReferencePath(ExtensionReference reference)
    {
        if (reference.Value.IsEmpty())
            return null;

        if (Path.IsPathRooted(reference.Value))
            return reference.Value;

        var combined = Path.Combine(reference.ExtensionFolder, reference.Value);
        try
        {
            return Path.GetFullPath(combined);
        }
        catch
        {
            return null;
        }
    }

    public static string? TryResolveAssemblyLocation(string assemblyName)
    {
        var normalized = NormalizeAssemblyName(assemblyName);
        if (normalized.IsEmpty())
            return null;

        try
        {
            var loaded = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, normalized, StringComparison.OrdinalIgnoreCase));

            loaded ??= Assembly.Load(new AssemblyName(normalized));

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            return loaded?.Location.HasValue() == true ? loaded.Location : null;
        }
        catch
        {
            return null;
        }
    }

    private static string? FindAppCodeFolder(string? startPath)
    {
        if (startPath.IsEmpty())
            return null;

        string? normalized;
        try
        {
            normalized = Path.GetFullPath(startPath);
        }
        catch
        {
            return null;
        }

        var directory = Directory.Exists(normalized)
            ? new DirectoryInfo(normalized)
            : new FileInfo(normalized).Directory;

        while (directory != null)
        {
            if (directory.Name.Equals(FolderConstants.AppCodeFolder, StringComparison.OrdinalIgnoreCase))
                return directory.FullName;

            var candidate = Path.Combine(directory.FullName, FolderConstants.AppCodeFolder);
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return null;
    }

    private static CompileConfigCache LoadConfig(string configPath)
    {
        var lastWrite = File.GetLastWriteTimeUtc(configPath);
        if (ConfigCache.TryGetValue(configPath, out var cached) && cached.LastWriteUtc == lastWrite)
            return cached;

        try
        {
            var json = File.ReadAllText(configPath);
            using var doc = JsonDocument.Parse(json, ConfigJsonOptions);
            var root = doc.RootElement;

            var entry = new CompileConfigCache(
                lastWrite,
                ReadReferences(root, "References"),
                ReadReferences(root, "References.net4"));

            ConfigCache[configPath] = entry;
            return entry;
        }
        catch
        {
            return new CompileConfigCache(lastWrite, Array.Empty<string>(), Array.Empty<string>());
        }
    }

    private static IReadOnlyList<string> ReadReferences(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var element))
            return Array.Empty<string>();

        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                return element
                    .EnumerateArray()
                    .Select(val => val.ValueKind == JsonValueKind.String ? val.GetString() : null)
                    .Where(val => val.HasValue())
                    .Select(val => val!.Trim())
                    .Where(val => val.HasValue())
                    .ToList();
            case JsonValueKind.String:
                var single = element.GetString();
                return single.HasValue()
                    ? new[] { single.Trim() }
                    : Array.Empty<string>();
            default:
                return Array.Empty<string>();
        }
    }
}
