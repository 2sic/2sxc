using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sys.Configuration;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test context for ExtensionsBackend tests providing setup/teardown and test extension creation
/// </summary>
internal sealed class ExtensionsBackendTestContext : IDisposable
{
    #region Properties

    public string TempRoot { get; }
    public ExtensionReaderBackend Reader { get; }
    public ExtensionWriterBackend Writer { get; }
    public ExtensionInstallBackend Zip { get; }
    public IJsonService JsonSvc { get; }

    #endregion

    #region Constructor / Factory

    private readonly ServiceProvider _sp;

    private ExtensionsBackendTestContext(string tempRoot, ServiceProvider sp, ExtensionReaderBackend reader, ExtensionWriterBackend writer, ExtensionInstallBackend zip, IJsonService jsonSvc)
    {
        TempRoot = tempRoot;
        _sp = sp;
        Reader = reader;
        Writer = writer;
        Zip = zip;
        JsonSvc = jsonSvc;
    }

    public static ExtensionsBackendTestContext Create()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-ext-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        // Create site & path services first so they can be registered
        var site = new FakeSite(tempRoot);
        var appPathSvc = new FakeAppPathsMicroSvc(tempRoot);

        var services = new ServiceCollection();
        services.AddSingleton<IAppReaderFactory, FakeAppReaderFactory>();
        services.AddSingleton<IJsonService, SimpleJsonService>();
        services.AddSingleton<IGlobalConfiguration, FakeGlobalConfiguration>();
        services.AddSingleton<ISite>(site);
        services.AddSingleton<IAppPathsMicroSvc>(appPathSvc);

        // Register LazySvc wrappers for basic services using factory overloads (DI will provide sp when resolving)
        services.AddSingleton(sp => new LazySvc<IAppReaderFactory>(sp));
        services.AddSingleton(sp => new LazySvc<IJsonService>(sp));
        services.AddSingleton<IEnumerable<IFileGenerator>>(_ => Array.Empty<IFileGenerator>());
        services.AddSingleton(sp => new LazySvc<IEnumerable<IFileGenerator>>(sp));
        services.AddSingleton<IAppJsonConfigurationService, FakeAppJsonConfigurationService>();
        services.AddSingleton(sp => new LazySvc<IAppJsonConfigurationService>(sp));

        // Register backend dependencies explicitly so they can be resolved by LazySvc later
        services.AddTransient<ExtensionManifestService>();
        services.AddSingleton(sp => new LazySvc<ExtensionInspectBackend>(sp));
        services.AddTransient<ExtensionInspectBackend>();
        services.AddSingleton<FileSaver>();
        services.AddSingleton<CodeControllerReal>(sp => new CodeControllerReal(
            sp.GetRequiredService<FileSaver>(),
            sp.GetRequiredService<LazySvc<IEnumerable<IFileGenerator>>>(),
            sp.GetRequiredService<LazySvc<IAppJsonConfigurationService>>(),
            sp.GetRequiredService<LazySvc<IAppReaderFactory>>()));
        services.AddSingleton(sp => new LazySvc<CodeControllerReal>(sp));
        
        services.AddSingleton<ExtensionReaderBackend>(sp => new ExtensionReaderBackend(
            sp.GetRequiredService<LazySvc<IAppReaderFactory>>(),
            sp.GetRequiredService<ISite>(),
            sp.GetRequiredService<IAppPathsMicroSvc>(),
            sp.GetRequiredService<LazySvc<IJsonService>>(),
            sp.GetRequiredService<ExtensionManifestService>(),
            sp.GetRequiredService<LazySvc<CodeControllerReal>>()));

        services.AddSingleton<ExtensionWriterBackend>(sp => new ExtensionWriterBackend(
            sp.GetRequiredService<LazySvc<IAppReaderFactory>>(),
            sp.GetRequiredService<ISite>(),
            sp.GetRequiredService<IAppPathsMicroSvc>()));

        services.AddSingleton<ExtensionInstallBackend>(sp => new ExtensionInstallBackend(
            sp.GetRequiredService<LazySvc<IAppReaderFactory>>(),
            sp.GetRequiredService<ISite>(),
            sp.GetRequiredService<IAppPathsMicroSvc>(),
            sp.GetRequiredService<IGlobalConfiguration>(),
            sp.GetRequiredService<ExtensionManifestService>(),
            sp.GetRequiredService<LazySvc<ExtensionInspectBackend>>(),
            sp.GetRequiredService<LazySvc<CodeControllerReal>>()));

        var sp = services.BuildServiceProvider() 
            ?? throw new InvalidOperationException("Failed to build service provider");

        // Configure global folder as previous tests relied on it for path resolution helpers
        var globalConfig = sp.GetRequiredService<IGlobalConfiguration>();
        globalConfig.GlobalFolder(tempRoot);

        // Resolve backends
        var reader = sp.GetRequiredService<ExtensionReaderBackend>();
        var writer = sp.GetRequiredService<ExtensionWriterBackend>();
        var zip = sp.GetRequiredService<ExtensionInstallBackend>();

        var jsonSvc = sp.GetRequiredService<IJsonService>();

        return new ExtensionsBackendTestContext(tempRoot, sp, reader, writer, zip, jsonSvc);
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
            => null!; // not used by our fake path svc
        public IAppReader Get(IAppIdentity appIdentity)
            => null!;
        public IAppReader GetSystemPreset()
            => null!;
        public IAppIdentityPure AppIdentity(int appId)
            => throw new NotImplementedException();
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
            WriteIndented = true
        };

        public string ToJson(object item) => item switch
        {
            string s => s,
            JsonElement je => je.GetRawText(),
            _ => JsonSerializer.Serialize(item, Options)
        };
        public string ToJson(object item, int indentation) => JsonSerializer.Serialize(item, 
            new JsonSerializerOptions(Options) { WriteIndented = indentation > 0 });
        public T? To<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
        public object? ToObject(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.Clone();
            }
            catch
            {
                return null;
            }
        }
        public ITyped ToTyped(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) 
            => throw new NotImplementedException();
        public IEnumerable<ITyped> ToTypedList(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) 
            => throw new NotImplementedException();
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

    private class FakeGlobalConfiguration : IGlobalConfiguration
    {
        private readonly Dictionary<string, string?> _values = new();

        public FakeGlobalConfiguration()
        {
            var tempFolder =
                // Create a unique temp folder for this test run
                Path.Combine(Path.GetTempPath(), "2sxc-test-temp", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempFolder);
        }

        public string? GetThis(string? key = null) => _values.TryGetValue(key!, out var value) ? value : null;

        public string? GetThisOrSet(Func<string> generator, string? key = null)
        {
            if (!_values.TryGetValue(key!, out var value))
            {
                value = generator();
                _values[key!] = value;
            }
            return value;
        }

        public string GetThisErrorOnNull(string? key = null) => GetThis(key) ?? throw new InvalidOperationException($"Config key '{key}' is null");

        public string? SetThis(string? value, string? key = null)
        {
            _values[key!] = value;
            return value;
        }
    }

    private class FakeAppJsonConfigurationService : IAppJsonConfigurationService
    {
        public void MoveAppJsonTemplateFromOldToNewLocation()
        {
        }

        public AppJsonConfiguration? GetAppJson(int appId, bool useShared) => null;

        public string AppJsonCacheKey(int appId, bool useShared) => string.Empty;

        public ICollection<string> ExcludeSearchPatterns(string sourceFolder, int appId, bool useShared)
            => Array.Empty<string>();
    }

    #endregion
}
