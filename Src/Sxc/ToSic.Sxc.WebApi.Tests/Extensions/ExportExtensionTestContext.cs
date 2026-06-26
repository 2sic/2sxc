using System.Text;
using System.Text.Json;
using ToSic.Eav.Apps.Mocks;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Backend.App;
using static ToSic.Sxc.ImportExport.Package.Sys.PackageIndexFile;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test context for ExtensionExportService tests providing setup/teardown and test extension creation
/// </summary>
internal sealed class ExportExtensionTestContext : IDisposable
{
    #region Properties

    public string TempRoot { get; }
    public ExtensionExportService ExportBackend { get; }

    #endregion

    #region Constructor / Factory

    private ExportExtensionTestContext(string tempRoot, ExtensionExportService exportBackend)
    {
        TempRoot = tempRoot;
        ExportBackend = exportBackend;
    }

    public static ExportExtensionTestContext Create(
        LazySvc<IAppReaderFactory> appReadersLazy,
        LazySvc<ContentExportApi> contentExportLazy,
        ExtensionManifestService manifestService)
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-export-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        var site = MockSiteTestHelpers.CreateSite(tempRoot);
        var appPathSvc = new MockAppPathsMicroSvc(tempRoot);

        var exportBackend = new ExtensionExportService(
            appReadersLazy, 
            site, 
            appPathSvc, 
            contentExportLazy,
            manifestService);

        return new ExportExtensionTestContext(tempRoot, exportBackend);
    }

    #endregion

    #region Setup Helpers

    /// <summary>
    /// Setup a test extension with given manifest
    /// </summary>
    public void SetupExtension(string name, ExtensionManifest manifest)
    {
        var extDir = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name);
        var dataDir = Path.Combine(extDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var jsonPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);

        // Sanitize JsonElements (Undefined -> null) then serialize directly
        var sanitized = manifest with
        {
            DataBundles = SanitizeJsonElement(manifest.DataBundles),
            //InputTypeAssets = SanitizeJsonElement(manifest.InputTypeAssets),
            InputFieldAssets = SanitizeJsonElement(manifest.InputFieldAssets),
            Releases = SanitizeJsonElement(manifest.Releases),
        };

        var json = JsonSerializer.Serialize(sanitized, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    }

    public void SetExtensionsBundled(string name, string bundledCommaSeparated)
    {
        var jsonPath = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name,
            FolderConstants.DataFolderProtected, FolderConstants.AppExtensionJsonFile);
        var json = File.ReadAllText(jsonPath);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement.ValueKind == JsonValueKind.Object
            ? doc.RootElement
            : throw new InvalidOperationException("extension.json root must be an object");

        var dict = root
            .EnumerateObject()
            .ToDictionary(p => p.Name, p => p.Value.Clone(), StringComparer.Ordinal);

        using var bundledDoc = JsonDocument.Parse(JsonSerializer.Serialize(bundledCommaSeparated));
        dict["extensionsBundled"] = bundledDoc.RootElement.Clone();

        var newJson = JsonSerializer.Serialize(dict, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        File.WriteAllText(jsonPath, newJson, new UTF8Encoding(false));
    }

    public void WriteInstalledLockFile(string name, string lockJson)
    {
        var lockPath = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name,
            FolderConstants.DataFolderProtected, LockFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(lockPath)!);
        File.WriteAllText(lockPath, lockJson, new UTF8Encoding(false));
    }

    private static readonly JsonElement JsonNullElement = JsonDocument.Parse("null").RootElement.Clone();
    private static JsonElement SanitizeJsonElement(JsonElement el) => el.ValueKind == JsonValueKind.Undefined ? JsonNullElement : el;

    /// <summary>
    /// Create extension files in dist folder
    /// </summary>
    public void CreateExtensionFiles(string name, params (string fileName, string content)[] files)
    {
        var extDir = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name);
        var distDir = Path.Combine(extDir, "dist");
        Directory.CreateDirectory(distDir);

        foreach (var (fileName, content) in files)
        {
            File.WriteAllText(Path.Combine(distDir, fileName), content);
        }
    }

    public string CreateExtensionFile(string name, string relativePath, string content)
    {
        // Test setup must be able to create the same long physical paths that production export is
        // expected to read. Use the Windows extended path only for disk writes; returned paths stay in
        // normal form because that is what production code receives from app path services.
        var filePath = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name, relativePath);
        Directory.CreateDirectory(PathForDiskAccess(Path.GetDirectoryName(filePath)!));
        File.WriteAllText(PathForDiskAccess(filePath), content);
        return filePath;
    }

    /// <summary>
    /// Create AppCode files for extension
    /// </summary>
    public void CreateAppCodeFiles(string name, params (string fileName, string content)[] files)
    {
        var appCodePath = Path.Combine(TempRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, name);
        Directory.CreateDirectory(appCodePath);

        foreach (var (fileName, content) in files)
        {
            File.WriteAllText(Path.Combine(appCodePath, fileName), content);
        }
    }

    private const string ExtendedPathPrefix = @"\\?\";
    private const string UncPrefix = @"\\";

    private static string PathForDiskAccess(string path)
    {
        // Keep this local to tests so the fixture can create/verify long paths without depending on
        // production internals. The behavior intentionally mirrors the export service helper.
        if (Path.DirectorySeparatorChar != '\\' || path.StartsWith(ExtendedPathPrefix, StringComparison.Ordinal))
            return path;

        var fullPath = Path.GetFullPath(path);

        return fullPath.StartsWith(UncPrefix, StringComparison.Ordinal)
            ? @"\\?\UNC\" + fullPath.Substring(UncPrefix.Length)
            : ExtendedPathPrefix + fullPath;
    }

    #endregion

    #region Disposal

    public void Dispose()
    {
        try { Directory.Delete(TempRoot, recursive: true); } catch { /* Ignore */ }
    }

    #endregion
}
