using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sys.Coding;
using ToSic.Sys.DI;
using ToSic.Sys.Logging;

namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test context for ExtensionsReaderBackend tests
/// </summary>
internal sealed class ExtensionsReaderTestContext : IDisposable
{
    #region Properties

    public string TempRoot { get; }
    public ExtensionsReaderBackend ReaderBackend { get; }

    #endregion

    #region Constructor / Factory

    private readonly ServiceProvider _sp;

    private ExtensionsReaderTestContext(string tempRoot, ServiceProvider sp, ExtensionsReaderBackend readerBackend)
    {
        TempRoot = tempRoot;
        _sp = sp;
        ReaderBackend = readerBackend;
    }

    public static ExtensionsReaderTestContext Create()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-extensions-reader-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        var services = new ServiceCollection();
        services.AddSingleton<IAppReaderFactory, FakeAppReaderFactory>();
        services.AddSingleton<IJsonService, SimpleJsonService>();
        services.AddTransient<ExtensionManifestService>();
            
        var sp = services.BuildServiceProvider() as ServiceProvider 
            ?? throw new InvalidOperationException("Failed to build service provider");

        var appReadersLazy = new LazySvc<IAppReaderFactory>(sp);
        var jsonLazy = new LazySvc<IJsonService>(sp);
        var site = new FakeSite(tempRoot);
        var appPathSvc = new FakeAppPathsMicroSvc(tempRoot);
        var manifestHelper = sp.GetRequiredService<ExtensionManifestService>();

        var readerBackend = new ExtensionsReaderBackend(
            appReadersLazy, 
            site, 
            appPathSvc, 
            jsonLazy,
            manifestHelper);

        return new ExtensionsReaderTestContext(tempRoot, sp, readerBackend);
    }

    #endregion

    #region Setup Helpers

    /// <summary>
    /// Setup a test extension with given configuration in the primary extensions folder
    /// </summary>
    public void SetupExtension(string name, object config)
    {
        var extDir = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name);
        var dataDir = Path.Combine(extDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);
            
        var jsonPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    }

    /// <summary>
    /// Setup an edition of an extension with given configuration
    /// </summary>
    public void SetupEdition(string editionName, string extensionName, object config)
    {
        var editionExtDir = Path.Combine(TempRoot, editionName, FolderConstants.AppExtensionsFolder, extensionName);
        var dataDir = Path.Combine(editionExtDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);
            
        var jsonPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    }

    /// <summary>
    /// Create an edition folder structure without manifest (for negative testing)
    /// </summary>
    public void CreateEditionFolderOnly(string editionName, string extensionName)
    {
        var editionExtDir = Path.Combine(TempRoot, editionName, FolderConstants.AppExtensionsFolder, extensionName);
        Directory.CreateDirectory(editionExtDir);
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
        public ITyped? ToTyped(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) 
            => null;
        public IEnumerable<ITyped>? ToTypedList(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) 
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
