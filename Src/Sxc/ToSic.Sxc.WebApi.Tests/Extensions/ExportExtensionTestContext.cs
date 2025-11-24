using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sys.Coding;
using ToSic.Sys.DI;
using ToSic.Sys.Logging;
using ToSic.Eav.Apps.Sys.FileSystemState;

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

    private readonly ServiceProvider _sp;

    private ExportExtensionTestContext(string tempRoot, ServiceProvider sp, ExtensionExportService exportBackend)
    {
        TempRoot = tempRoot;
        _sp = sp;
        ExportBackend = exportBackend;
    }

    public static ExportExtensionTestContext Create()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-export-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        var services = new ServiceCollection();
        services.AddSingleton<IAppReaderFactory, FakeAppReaderFactory>();
        services.AddSingleton<IJsonService, SimpleJsonService>();
            
        var sp = services.BuildServiceProvider() 
            ?? throw new InvalidOperationException("Failed to build service provider");

        var appReadersLazy = new LazySvc<IAppReaderFactory>(sp);
        var contentExportLazy = new LazySvc<ContentExportApi>(sp);

        var site = new FakeSite(tempRoot);
        var appPathSvc = new FakeAppPathsMicroSvc(tempRoot);

        // Create manifest service
        var manifestService = new ExtensionManifestService();

        var exportBackend = new ExtensionExportService(
            appReadersLazy, 
            site, 
            appPathSvc, 
            contentExportLazy,
            manifestService);

        return new ExportExtensionTestContext(tempRoot, sp, exportBackend);
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
            InputTypeAssets = SanitizeJsonElement(manifest.InputTypeAssets),
            Releases = SanitizeJsonElement(manifest.Releases),
        };

        var json = JsonSerializer.Serialize(sanitized, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
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

    #endregion

    #region Disposal

    public void Dispose()
    {
        try { _sp.Dispose(); } catch { /* Ignore */ }
        try { Directory.Delete(TempRoot, recursive: true); } catch { /* Ignore */ }
    }

    #endregion

    #region Fake Implementations

    private class FakeAppReaderFactory : IAppReaderFactory
    {
        public IAppReader Get(int appId) => null!;
        public IAppReader Get(IAppIdentity appIdentity) => null!;
        public IAppReader GetSystemPreset() => null!;
        public IAppIdentityPure AppIdentity(int appId) => new AppIdentity(1, appId) as IAppIdentityPure ?? throw new();
        public IAppReader GetZonePrimary(int zoneId) => throw new NotImplementedException();
        public IAppReader? TryGet(IAppIdentity appIdentity) => null;
        public IAppReader? ToReader(IAppStateCache? state) => null;
        public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded) => null;
        public IAppReader GetOrKeep(IAppIdentity appIdOrReader) => throw new NotImplementedException();
    }

    private class FakeAppPathsMicroSvc(string root) : IAppPathsMicroSvc
    {
        public IAppPaths Get(IAppReader appReader) => new FakeAppPaths(root);
        public IAppPaths Get(IAppReader appReader, ISite? siteOrNull) => new FakeAppPaths(root);
    }

    private class FakeAppPaths(string physicalPath) : IAppPaths
    {
        public string Path => "/";
        public string PhysicalPath { get; } = physicalPath;
        public string PathShared => "/";
        public string PhysicalPathShared { get; } = physicalPath;
        public string RelativePath => "/";
        public string RelativePathShared => "/";
    }

    private class SimpleJsonService : IJsonService
    {
        private static readonly JsonSerializerOptions Options = new() 
        { 
            PropertyNamingPolicy = null,
            WriteIndented = false 
        };

        public string ToJson(object item) => JsonSerializer.Serialize(item, Options);
        public string ToJson(object item, int indentation) => JsonSerializer.Serialize(item, 
            new JsonSerializerOptions(Options) { WriteIndented = indentation > 0 });
        public T? To<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
        public object? ToObject(string json) => JsonSerializer.Deserialize<object>(json, Options);
        public ITyped? ToTyped(string json, NoParamOrder npo = default, string? fallback = default, bool? propsRequired = default) 
            => null;
        public IEnumerable<ITyped>? ToTypedList(string json, NoParamOrder npo = default, string? fallback = default, bool? propsRequired = default) 
            => null;
    }

    private class FakeSite(string appsRootPhysicalFull) : ISite
    {
        public ISite Init(int siteId, ILog? parentLogOrNull) => this;
        public int Id { get; } = 1;
        public string Name { get; } = "Test";
        public string AppsRootPhysical { get; } = appsRootPhysicalFull;
        public string AppsRootPhysicalFull { get; } = appsRootPhysicalFull;
        public string AppAssetsLinkTemplate { get; } = "/app/{appFolder}";
        public string ContentPath { get; } = "/";
        public string Url { get; } = "/";
        public string UrlRoot { get; } = "/";
        public string CurrentCultureCode { get; } = "en-us";
        public string DefaultCultureCode { get; } = "en-us";
        public int ZoneId { get; } = 1;
    }

    #endregion
}
