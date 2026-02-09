using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test context for ExtensionReaderBackend tests
/// </summary>
internal sealed class ExtensionsReaderTestContext : IDisposable
{
    #region Properties

    public string TempRoot { get; }
    public ExtensionReaderBackend ReaderBackend { get; }

    #endregion

    #region Constructor / Factory

    private readonly ServiceProvider _sp;
    private readonly FakeAppJsonConfigurationService _appJsonService;

    private ExtensionsReaderTestContext(string tempRoot, ServiceProvider sp, ExtensionReaderBackend readerBackend, FakeAppJsonConfigurationService appJsonService)
    {
        TempRoot = tempRoot;
        _sp = sp;
        ReaderBackend = readerBackend;
        _appJsonService = appJsonService;
    }

    public static ExtensionsReaderTestContext Create()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-extensions-reader-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        var site = new FakeSite(tempRoot);
        var appPathSvc = new FakeAppPathsMicroSvc(tempRoot);

        var services = new ServiceCollection();
        services.AddSingleton<IAppReaderFactory, FakeAppReaderFactory>();
        services.AddSingleton<IJsonService, SimpleJsonService>();
        services.AddTransient<ExtensionManifestService>();
        services.AddSingleton<ISite>(site);
        services.AddSingleton<IAppPathsMicroSvc>(appPathSvc);
        services.AddSingleton(sp => new LazySvc<IAppReaderFactory>(sp));
        services.AddSingleton<IEnumerable<IFileGenerator>>(_ => []);
        services.AddSingleton(sp => new LazySvc<IEnumerable<IFileGenerator>>(sp));
        services.AddSingleton<FakeAppJsonConfigurationService>(_ => new FakeAppJsonConfigurationService(tempRoot));
        services.AddSingleton<IAppJsonConfigurationService>(sp => sp.GetRequiredService<FakeAppJsonConfigurationService>());
        services.AddSingleton(sp => new LazySvc<IAppJsonConfigurationService>(sp));
        services.AddSingleton<FileSaver>();
        services.AddSingleton<CodeControllerReal>(sp => new CodeControllerReal(
            sp.GetRequiredService<FileSaver>(),
            sp.GetRequiredService<LazySvc<IEnumerable<IFileGenerator>>>(),
            sp.GetRequiredService<LazySvc<IAppJsonConfigurationService>>(),
            sp.GetRequiredService<LazySvc<IAppReaderFactory>>()));
        services.AddSingleton(sp => new LazySvc<CodeControllerReal>(sp));
            
        var sp = services.BuildServiceProvider() 
            ?? throw new InvalidOperationException("Failed to build service provider");

        var appReadersLazy = new LazySvc<IAppReaderFactory>(sp);
        var jsonLazy = new LazySvc<IJsonService>(sp);
        var manifestHelper = sp.GetRequiredService<ExtensionManifestService>();
        var codeLazy = sp.GetRequiredService<LazySvc<CodeControllerReal>>();

        var readerBackend = new ExtensionReaderBackend(
            appReadersLazy, 
            sp.GetRequiredService<ISite>(), 
            sp.GetRequiredService<IAppPathsMicroSvc>(), 
            jsonLazy,
            manifestHelper,
            codeLazy);

        var appJsonService = sp.GetRequiredService<FakeAppJsonConfigurationService>();

        return new ExtensionsReaderTestContext(tempRoot, sp, readerBackend, appJsonService);
    }

    #endregion

    #region Setup Helpers

    /// <summary>
    /// Setup a test extension with given configuration in the primary extensions folder
    /// </summary>
    public void SetupExtension(string name, object config)
    {
        _appJsonService.EnsureEdition(string.Empty);

        var extDir = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name);
        var dataDir = Path.Combine(extDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);
            
        var jsonPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var json = config is ExtensionManifest manifest 
            ? ExtensionManifestSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true })
            : JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    }

    /// <summary>
    /// Setup an edition of an extension with given configuration
    /// </summary>
    public void SetupEdition(string editionName, string extensionName, object config)
    {
        _appJsonService.EnsureEdition(editionName);

        var editionExtDir = Path.Combine(TempRoot, editionName, FolderConstants.AppExtensionsFolder, extensionName);
        var dataDir = Path.Combine(editionExtDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);
            
        var jsonPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var json = config is ExtensionManifest manifest 
            ? ExtensionManifestSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true })
            : JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    }

    /// <summary>
    /// Create an edition folder structure without manifest (for negative testing)
    /// </summary>
    public void CreateEditionFolderOnly(string editionName, string extensionName)
    {
        _appJsonService.EnsureEdition(editionName);

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
        public IAppReader Get(int appId)
            => null!;
        public IAppReader Get(IAppIdentity appIdentity)
            => null!;
        public IAppReader GetSystemPreset()
            => null!;
        public IAppIdentityPure AppIdentity(int appId)
            => new AppIdentity(1, appId) as IAppIdentityPure ?? throw new();
        public IAppReader GetZonePrimary(int zoneId)
            => throw new NotImplementedException();
        public IAppReader? TryGet(IAppIdentity appIdentity)
            => null;
        public IAppReader? ToReader(IAppStateCache? state)
            => null;
        public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded)
            => null;
        public IAppReader GetOrKeep(IAppIdentity appIdOrReader)
            => throw new NotImplementedException();
    }

    private class FakeAppPathsMicroSvc(string root) : IAppPathsMicroSvc
    {
        public IAppPaths Get(IAppReader appReader)
            => new FakeAppPaths(root);
        public IAppPaths Get(IAppReader appReader, ISite? siteOrNull)
            => new FakeAppPaths(root);
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

        public string ToJson(object item)
            => JsonSerializer.Serialize(item, Options);
        public string ToJson(object item, int indentation)
            => JsonSerializer.Serialize(item, new JsonSerializerOptions(Options) { WriteIndented = indentation > 0 });
        public T? To<T>(string json)
            => JsonSerializer.Deserialize<T>(json, Options);
        public object? ToObject(string json)
            => JsonSerializer.Deserialize<object>(json, Options);
        public ITyped? ToTyped(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) 
            => null;
        public IEnumerable<ITyped>? ToTypedList(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) 
            => null;
    }

    private class FakeAppJsonConfigurationService : IAppJsonConfigurationService
    {
        private readonly string _appRoot;
        private readonly AppJsonConfiguration _configuration;
        private readonly object _lock = new();

        public FakeAppJsonConfigurationService(string appRoot)
        {
            _appRoot = appRoot;
            _configuration = new AppJsonConfiguration
            {
                IsConfigured = true,
                Editions =
                {
                    [string.Empty] = new AppJsonConfiguration.EditionInfo
                    {
                        Description = "Root edition shared by all variants.",
                        IsDefault = true
                    }
                }
            };
            PersistConfiguration();
        }

        public void MoveAppJsonTemplateFromOldToNewLocation()
        {
        }

        public AppJsonConfiguration? GetAppJson(int appId, bool useShared) => _configuration;

        public string AppJsonCacheKey(int appId, bool useShared) => string.Empty;

        public ICollection<string> ExcludeSearchPatterns(string sourceFolder, int appId, bool useShared)
            => Array.Empty<string>();

        public void EnsureEdition(string editionName)
        {
            if (editionName == null)
                return;

            lock (_lock)
            {
                if (_configuration.Editions.ContainsKey(editionName))
                    return;

                _configuration.Editions[editionName] = new AppJsonConfiguration.EditionInfo();
                PersistConfiguration();
            }
        }

        private void PersistConfiguration()
        {
            var appData = Path.Combine(_appRoot, FolderConstants.DataFolderProtected);
            Directory.CreateDirectory(appData);
            var appJsonPath = Path.Combine(appData, FolderConstants.AppJsonFile);
            var json = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(appJsonPath, json, new UTF8Encoding(false));
        }
    }

    private class FakeSite(string appsRootPhysicalFull) : ISite
    {
        public ISite Init(int siteId, ILog? parentLogOrNull)
            => this;
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
