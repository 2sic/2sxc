using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToSic.Eav.Sys;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Code.Sys.HotBuild;

/// <summary>
/// Helper that discovers compile-time references defined inside app extensions.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionCompileReferenceService(MemoryCacheService cache)
    : ServiceBase("Sxc.ExtRefRd", connect: [cache])
{
    private const string CompileConfigFileName = "compile.json";

    private static readonly JsonSerializerOptions ConfigSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    private const string CachePrefix = "Sxc.ExtRefRdr";
    
    /// <summary>
    /// Load extension reference definitions underneath the AppCode/Extensions folder that contains <paramref name="startPath"/>.
    /// </summary>
    public IReadOnlyList<ExtensionReference> GetReferences(string? startPath, bool netFramework)
    {
        var l = Log.Fn<List<ExtensionReference>>($"start:{startPath}, net4:{netFramework}");
        var appCodeFolder = FindAppCodeFolder(startPath);
        if (appCodeFolder.IsEmpty())
            return l.Return([], "no-appcode");

        var extensionsRoot = Path.Combine(appCodeFolder, FolderConstants.AppExtensionsFolder);
        if (!Directory.Exists(extensionsRoot))
            return l.Return([], "no-extensions");

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

        return l.Return(references, $"found:{references.Count}");
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

    public static string NormalizeAssemblyName(string referenceValue)
    {
        if (referenceValue.IsEmpty())
            return referenceValue;

        return referenceValue.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
            ? referenceValue.Substring(0, referenceValue.Length - 4)
            : referenceValue;
    }

    public string? ResolveReferencePath(ExtensionReference reference)
    {
        var l = Log.Fn<string?>($"value:{reference.Value}");
        if (reference.Value.IsEmpty())
            return l.ReturnNull("empty");

        if (Path.IsPathRooted(reference.Value))
            return l.Return(reference.Value, "rooted");

        var combined = Path.Combine(reference.ExtensionFolder, reference.Value);
        try
        {
            return l.ReturnAsOk(Path.GetFullPath(combined));
        }
        catch
        {
            return l.ReturnNull("invalid");
        }
    }

    public string? TryResolveAssemblyLocation(string assemblyName)
    {
        var l = Log.Fn<string?>($"assembly:{assemblyName}");
        var normalized = ExtensionCompileReferenceService.NormalizeAssemblyName(assemblyName);
        if (normalized.IsEmpty())
            return l.ReturnNull("empty");

        try
        {
            var loaded = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, normalized, StringComparison.OrdinalIgnoreCase));

            loaded ??= Assembly.Load(new AssemblyName(normalized));

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            var location = loaded?.Location.HasValue() == true ? loaded.Location : null;
            return location.HasValue()
                ? l.ReturnAsOk(location)
                : l.ReturnNull("no-location");
        }
        catch
        {
            return l.ReturnNull("error");
        }
    }

    private string? FindAppCodeFolder(string? startPath)
    {
        var l = Log.Fn<string?>($"start:{startPath}");
        if (startPath.IsEmpty())
            return l.ReturnNull("empty");

        string? normalized;
        try
        {
            normalized = Path.GetFullPath(startPath);
        }
        catch
        {
            return l.ReturnNull("invalid");
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
                return l.ReturnAsOk(candidate);

            directory = directory.Parent;
        }

        return l.ReturnNull("not-found");
    }

    private CompileConfigCache LoadConfig(string configPath)
    {
        var cacheKey = $"{CachePrefix}:{configPath}";
        if (cache.TryGet<CompileConfigCache>(cacheKey, out var cached) && cached is not null)
            return cached;

        var l = Log.Fn<CompileConfigCache>($"config:{configPath}");
        var entry = ReadConfig(configPath);
        cache.Set(cacheKey, entry, options =>
        {
            if (File.Exists(configPath))
            {
                options.WatchFiles([configPath]);
            }
            else
            {
                var parents = GetExistingParent(configPath);
                if (parents.Count > 0)
                    options.WatchFolders(parents.ToDictionary(p => p, _ => true));
            }
            return options;
        });

        return l.ReturnAsOk(entry);
    }
    private CompileConfigCache ReadConfig(string configPath)
    {
        var l = Log.Fn<CompileConfigCache>($"config:{configPath}");

        if (!File.Exists(configPath))
            return l.Return(new CompileConfigCache([], []), "missing");

        try
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<CompileConfig>(json, ConfigSerializerOptions) ?? new CompileConfig();
            var entry = new CompileConfigCache(config.NetCoreReferences,
                config.NetFrameworkReferences);
            return l.Return(entry, "ok");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new CompileConfigCache([], []), "error");
        }
    }

    private List<string> GetExistingParent(string filePath)
    {
        var l = Log.Fn<List<string>>($"parent:{filePath}");

        var parentPath = Path.GetDirectoryName(filePath);
        while (!parentPath.IsEmpty())
        {
            if (Directory.Exists(parentPath))
                return l.Return([parentPath], "found");
            parentPath = Path.GetDirectoryName(parentPath);
        }

        return l.Return([], "not-found");
    }

    public readonly struct ExtensionReference(string value, string extensionFolder)
    {
        public string Value { get; } = value;

        public string ExtensionFolder { get; } = extensionFolder;
    }

    private sealed record CompileConfigCache(IReadOnlyList<string> NetCoreReferences, IReadOnlyList<string> NetFrameworkReferences);

    private sealed class CompileConfig
    {
        [JsonPropertyName("References")]
        public List<string> NetCoreReferences { get; init; } = [];

        [JsonPropertyName("References.net4")]
        public List<string> NetFrameworkReferences { get; init; } = [];
    }
}
