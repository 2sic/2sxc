using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys; // IAppIdentityPure, IAppStateCache
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Services;
using ToSic.Sxc.Data; // ITyped
using ToSic.Sys.Coding; // NoParamOrder
using ToSic.Sys.DI;
using ToSic.Sys.Logging; // ILog

namespace ToSic.Sxc.WebApi.Tests.Extensions;

public class ExtensionsBackendTests
{
    [Fact]
    public void SaveThenRead_Roundtrip_Works()
    {
        using var ctx = TestContext.Create();

        var config = new { enabled = true, version = "1.0.0", list = new[] { 1, 2 } };

        // ensure extension folder exists before saving (SaveExtension now requires the folder to exist)
        var fooFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, "foo");
        Directory.CreateDirectory(fooFolder);

        // serialize config to JsonElement for the new SaveExtension signature
        var configJson = ctx.JsonSvc.ToJson(config);
        using var configDoc = JsonDocument.Parse(configJson);
        var configElem = configDoc.RootElement;

        var saved = ctx.Backend.SaveExtension(zoneId: 1, appId: 42, name: "foo", configuration: configElem);
        Assert.True(saved);

        // also create another extension folder without a config file
        var barFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, "bar");
        Directory.CreateDirectory(barFolder);

        var result = ctx.Backend.GetExtensions(42);
        Assert.NotNull(result);
        Assert.NotNull(result.Extensions);

        var foo = result.Extensions.FirstOrDefault(e => e.Folder == "foo");
        Assert.NotNull(foo);
        Assert.NotNull(foo!.Configuration);

        var jsonSvc = ctx.JsonSvc;
        var expectedJson = jsonSvc.ToJson(config);
        var actualJson = jsonSvc.ToJson(foo.Configuration!);
        Assert.Equal(expectedJson, actualJson);

        var bar = result.Extensions.FirstOrDefault(e => e.Folder == "bar");
        Assert.NotNull(bar);
        Assert.NotNull(bar!.Configuration);
    }

    [Fact]
    public void SaveThenRead_WithSampleSimpleExtension_Config_Works()
    {
        const string folder = "sample-simple-extension";

        using var ctx = TestContext.Create();

        // ensure extension folder exists before saving (SaveExtension now requires the folder to exist)
        var folderPath = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, folder);
        Directory.CreateDirectory(folderPath);

        // parse sample JSON into JsonElement
        using var cfgDoc = JsonDocument.Parse(SampleSimpleJson);
        var cfgElem = cfgDoc.RootElement;

        var saved = ctx.Backend.SaveExtension(zoneId: 1, appId: 1, name: folder, configuration: cfgElem);
        Assert.True(saved);

        var result = ctx.Backend.GetExtensions(1);
        Assert.NotNull(result);
        var item = result.Extensions.FirstOrDefault(e => e.Folder == folder);
        Assert.NotNull(item);
        Assert.NotNull(item!.Configuration);

        // Normalize both to minified JSON and compare
        var expected = ctx.JsonSvc.ToJson(ctx.JsonSvc.ToObject(SampleSimpleJson)!);
        var actual = ctx.JsonSvc.ToJson(item.Configuration!);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadExisting_WithComplexFeatureExtension_Config_Works()
    {
        const string complexFeatureExtension = "complex-feature-extension";

        using var ctx = TestContext.Create();

        var extDir = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, complexFeatureExtension, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(extDir);
        var jsonPath = Path.Combine(extDir, FolderConstants.AppExtensionJsonFile);

        // Write pretty JSON as it might come from source control
        File.WriteAllText(jsonPath, ComplexFeatureJson, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var result = ctx.Backend.GetExtensions(99);
        Assert.NotNull(result);
        var item = result.Extensions.FirstOrDefault(e => e.Folder == complexFeatureExtension);
        Assert.NotNull(item);
        Assert.NotNull(item!.Configuration);

        // Normalize both to minified JSON and compare
        var expected = ctx.JsonSvc.ToJson(ctx.JsonSvc.ToObject(ComplexFeatureJson)!);
        var actual = ctx.JsonSvc.ToJson(item.Configuration!);
        Assert.Equal(expected, actual);
    }

    private sealed class TestContext : IDisposable
    {
        public string TempRoot { get; }
        public ExtensionsBackend Backend { get; }
        public IJsonService JsonSvc { get; }

        private readonly ServiceProvider _sp;

        private TestContext(string tempRoot, ServiceProvider sp, ExtensionsBackend backend, IJsonService jsonSvc)
        {
            TempRoot = tempRoot;
            _sp = sp;
            Backend = backend;
            JsonSvc = jsonSvc;
        }

        public static TestContext Create()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-ext-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            var services = new ServiceCollection();
            services.AddSingleton<IAppReaderFactory, FakeAppReaderFactory>();
            services.AddSingleton<IJsonService, SimpleJsonService>();
            var sp = services.BuildServiceProvider() as ServiceProvider ?? throw new InvalidOperationException("Failed to build service provider");

            var appReadersLazy = new LazySvc<IAppReaderFactory>(sp);
            var jsonLazy = new LazySvc<IJsonService>(sp);

            var site = new FakeSite(tempRoot);
            var appPathSvc = new FakeAppPathsMicroSvc(tempRoot);

            var backend = new ExtensionsBackend(appReadersLazy, site, appPathSvc, jsonLazy);

            var jsonSvc = jsonLazy.Value;

            return new TestContext(tempRoot, sp, backend, jsonSvc);
        }

        public void Dispose()
        {
            try { _sp.Dispose(); } catch { }
            try { Directory.Delete(TempRoot, recursive: true); } catch { }
        }
    }

    private const string SampleSimpleJson = """
    {
      "id": "sample-simple",
      "name": "Sample Simple Extension",
      "version": "1.0.0",
      "author": {
        "name": "Example Corp",
        "contact": "dev@example.com"
      },
      "description": "A minimal extension used for w2w integration testing.",
      "enabled": true,
      "settings": {
        "theme": "light",
        "maxItems": 10,
        "showPreview": false
      },
      "scripts": [
        { "url": "/extensions/sample-simple/dist/script.js", "type": "module", "defer": true }
      ],
      "styles": [
        { "url": "/extensions/sample-simple/dist/styles.css", "media": "all" }
      ],
      "dependencies": [
        { "folder": "common-utils", "version": ">=1.2.0" }
      ],
      "createdAt": "2025-09-16T12:00:00Z"
    }
    """;

    private const string ComplexFeatureJson = """
    {
      "id": "complex-feature",
      "name": "Complex Feature Extension",
      "version": "2.4.1",
      "author": { "name": "ACME Integrations", "url": "https://acme.example" },
      "description": "Extension with nested config, feature flags and translations for w2w testing.",
      "enabled": true,
      "features": {
        "betaMode": true,
        "analytics": {
          "enabled": false,
          "provider": "ga",
          "samplingRate": 0.1
        },
        "dataSync": {
          "enabled": true,
          "intervalSeconds": 3600
        }
      },
      "translations": {
        "en": { "title": "Complex Feature", "button": "Run" },
        "de": { "title": "Komplexes Feature", "button": "Ausführen" }
      },
      "mappings": [
        { "source": "legacyId", "target": "id", "type": "int" },
        { "source": "priceCents", "target": "price", "transform": "divide100" }
      ],
      "configurationData": {
        "rules": [
          { "name": "ruleA", "enabled": true, "conditions": [] },
          { "name": "ruleB", "enabled": false, "conditions": [{ "field": "status", "op": "==", "value": "active" }] }
        ],
        "metadata": {},
        "exampleArray": [1, 2, 3, { "nested": "value" }]
      },
      "permissions": {
        "admin": ["read", "write", "delete"],
        "editor": ["read", "write"],
        "viewer": ["read"]
      },
      "notes": null
    }
    """;

    private class FakeAppReaderFactory : IAppReaderFactory
    {
        public IAppReader Get(int appId) => null!; // not used by our fake path svc
        public IAppReader Get(IAppIdentity appIdentity) => null!;
        public IAppReader GetSystemPreset() => null!;
        public IAppIdentityPure AppIdentity(int appId) => throw new NotImplementedException();
        public IAppReader GetZonePrimary(int zoneId) => throw new NotImplementedException();
        public IAppReader? TryGet(IAppIdentity appIdentity) => null;
        public IAppReader? ToReader(IAppStateCache state) => null;
        public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded) => null;
        public IAppReader GetOrKeep(IAppIdentity appIdOrReader) => throw new NotImplementedException();
    }

    private class FakeAppPathsMicroSvc : IAppPathsMicroSvc
    {
        private readonly string _root;
        public FakeAppPathsMicroSvc(string root) => _root = root;
        public IAppPaths Get(IAppReader appReader) => new FakeAppPaths(_root);
        public IAppPaths Get(IAppReader appReader, ISite? siteOrNull) => new FakeAppPaths(_root);
    }

    private class FakeAppPaths : IAppPaths
    {
        public FakeAppPaths(string physicalPath)
        {
            PhysicalPath = physicalPath;
            Path = "/";
            PathShared = "/";
            PhysicalPathShared = physicalPath;
            RelativePath = "/";
            RelativePathShared = "/";
        }

        public string Path { get; }
        public string PhysicalPath { get; }
        public string PathShared { get; }
        public string PhysicalPathShared { get; }
        public string RelativePath { get; }
        public string RelativePathShared { get; }
    }

    private class SimpleJsonService : IJsonService
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = null,
            WriteIndented = false
        };

        public string ToJson(object item) => JsonSerializer.Serialize(item, Options);
        public string ToJson(object item, int indentation) => JsonSerializer.Serialize(item, new JsonSerializerOptions(Options) { WriteIndented = indentation > 0 });
        public T? To<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
        public object? ToObject(string json) => JsonSerializer.Deserialize<object>(json, Options);
        public ITyped? ToTyped(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) => throw new NotImplementedException();
        public IEnumerable<ITyped>? ToTypedList(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default) => throw new NotImplementedException();
    }

    private class FakeSite : ISite
    {
        public FakeSite(string appsRootPhysicalFull)
        {
            AppsRootPhysicalFull = appsRootPhysicalFull;
            AppsRootPhysical = appsRootPhysicalFull;
            AppAssetsLinkTemplate = "/app/{appFolder}";
            Url = "/";
            UrlRoot = "/";
            Name = "Test";
        }

        public ISite Init(int siteId, ILog? parentLogOrNull) => this;

        public int Id { get; } = 1;
        public string Name { get; }
        public string AppsRootPhysical { get; }
        public string AppsRootPhysicalFull { get; }
        public string AppAssetsLinkTemplate { get; }
        public string ContentPath { get; } = "/";
        public string Url { get; }
        public string UrlRoot { get; }
        public string CurrentCultureCode { get; } = "en-us";
        public string DefaultCultureCode { get; } = "en-us";
        public int ZoneId { get; } = 1;
    }
}
