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
using ToSic.Sys.Configuration;
using ToSic.Sys.DI;
using ToSic.Sys.Logging; // ILog
using ToSic.Sys.Security.Encryption;
using System.IO.Compression;
using Tests.ToSic.ToSxc.WebApi.Extensions;
using static Xunit.Assert;

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

        var saved = ctx.Backend.SaveExtensionTac(zoneId: 1, appId: 42, name: "foo", configuration: configElem);
        True(saved);

        // also create another extension folder without a config file
        var barFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, "bar");
        Directory.CreateDirectory(barFolder);

        var result = ctx.Backend.GetExtensionsTac(42);
        NotNull(result);
        NotNull(result.Extensions);

        var foo = result.Extensions.FirstOrDefault(e => e.Folder == "foo");
        NotNull(foo);
        NotNull(foo!.Configuration);

        var jsonSvc = ctx.JsonSvc;
        var expectedJson = jsonSvc.ToJson(config);
        var actualJson = jsonSvc.ToJson(foo.Configuration!);
        Equal(expectedJson, actualJson);

        var bar = result.Extensions.FirstOrDefault(e => e.Folder == "bar");
        NotNull(bar);
        NotNull(bar!.Configuration);
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

        var saved = ctx.Backend.SaveExtensionTac(zoneId: 1, appId: 1, name: folder, configuration: cfgElem);
        True(saved);

        var result = ctx.Backend.GetExtensionsTac(1);
        NotNull(result);
        var item = result.Extensions.FirstOrDefault(e => e.Folder == folder);
        NotNull(item);
        NotNull(item!.Configuration);

        // Normalize both to minified JSON and compare
        var expected = ctx.JsonSvc.ToJson(ctx.JsonSvc.ToObject(SampleSimpleJson)!);
        var actual = ctx.JsonSvc.ToJson(item.Configuration!);
        Equal(expected, actual);
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

        var result = ctx.Backend.GetExtensionsTac(99);
        NotNull(result);
        var item = result.Extensions.FirstOrDefault(e => e.Folder == complexFeatureExtension);
        NotNull(item);
        NotNull(item!.Configuration);

        // Normalize both to minified JSON and compare
        var expected = ctx.JsonSvc.ToJson(ctx.JsonSvc.ToObject(ComplexFeatureJson)!);
        var actual = ctx.JsonSvc.ToJson(item.Configuration!);
        Equal(expected, actual);
    }

    [Fact]
    public void InstallZip_Simple_Works()
    {
        using var ctx = TestContext.Create();

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Create proper extensions/color-picker/ structure
            var extensionJson = zip.CreateEntry("extensions/color-picker/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("{ \"id\":\"color-picker\", \"enabled\": true, \"isInstalled\": true }");
            }

            var distFile = zip.CreateEntry("extensions/color-picker/dist/script.js");
            using (var stream = distFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("console.log('ok');");
            }

            // Create lock file
            var lockJson = zip.CreateEntry("extensions/color-picker/App_Data/extension.lock.json");
            using (var stream = lockJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = "1.0.0",
                    files = new[]
                    {
                        new
                        {
                            file = "extensions/color-picker/App_Data/extension.json",
                            hash = Sha256.Hash("{ \"id\":\"color-picker\", \"enabled\": true, \"isInstalled\": true }")
                        },
                        new
                        {
                            file = "extensions/color-picker/dist/script.js",
                            hash = Sha256.Hash("console.log('ok');")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: 1, appId: 42, zipStream: ms, name: null, overwrite: false, originalZipFileName: "color-picker.zip");
        True(ok);

        var result = ctx.Backend.GetExtensionsTac(42);
        Contains(result.Extensions, e => e.Folder == "color-picker");
        var cfg = result.Extensions.First(e => e.Folder == "color-picker").Configuration;
        NotNull(cfg);
    }

    [Fact]
    public void InstallZip_BlocksTraversal()
    {
        using var ctx = TestContext.Create();

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Create proper structure but with path traversal attempt
            var extensionJson = zip.CreateEntry("extensions/bad/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("{ \"id\":\"bad\", \"isInstalled\": true }");
            }

            // Try to create a file outside the extension folder
            var badFile = zip.CreateEntry("extensions/bad/../../outside.txt");
            using (var stream = badFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("nope");
            }

            // Create lock file (it should fail validation due to the traversal)
            var lockJson = zip.CreateEntry("extensions/bad/App_Data/extension.lock.json");
            using (var stream = lockJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = "1.0.0",
                    files = new[]
                    {
                        new
                        {
                            file = "extensions/bad/App_Data/extension.json",
                            hash = Sha256.Hash("{ \"id\":\"bad\", \"isInstalled\": true }")
                        },
                        new
                        {
                            file = "extensions/bad/../../outside.txt",
                            hash = Sha256.Hash("nope")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: 1, appId: 42, zipStream: ms, name: "bad", overwrite: false, originalZipFileName: "bad.zip");
        False(ok);
    }

    [Fact]
    public void InstallZip_Overwrite_Behavior()
    {
        using var ctx = TestContext.Create();

        // Create a properly structured extension ZIP
        using var ms1 = new MemoryStream();
        using (var zip = new ZipArchive(ms1, ZipArchiveMode.Create, leaveOpen: true))
        {
            //// Explicitly add root folder entries (optional but increases compatibility)
            //zip.CreateEntry("extensions/");
            //zip.CreateEntry("extensions/dup/");

            // Create extensions/dup/ structure
            var extensionJson = zip.CreateEntry("extensions/dup/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("{ \"id\":\"dup\", \"isInstalled\": true }");
            }

            // Create a simple file to include
            var distFile = zip.CreateEntry("extensions/dup/dist/main.js");
            using (var stream = distFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("console.log('dup');");
            }

            // Add another placeholder asset to ensure multi-file lock scenarios
            var placeholder = zip.CreateEntry("extensions/dup/readme.txt");
            using (var stream = placeholder.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("dup extension placeholder");
            }

            // Create lock file with file hashes (must include all installed files except the lock itself)
            var lockJson = zip.CreateEntry("extensions/dup/App_Data/extension.lock.json");
            using (var stream = lockJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = "1.0.0",
                    files = new[]
                    {
                        new
                        {
                            file = "extensions/dup/App_Data/extension.json",
                            hash = Sha256.Hash("{ \"id\":\"dup\", \"isInstalled\": true }")
                        },
                        new
                        {
                            file = "extensions/dup/dist/main.js",
                            hash = Sha256.Hash("console.log('dup');")
                        },
                        new
                        {
                            file = "extensions/dup/readme.txt",
                            hash = Sha256.Hash("dup extension placeholder")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms1.Position = 0;
        // Provide explicit name parameter to avoid folder derivation edge cases
        var ok1 = ctx.Backend.InstallExtensionZipTac(zoneId: 1, appId: 42, zipStream: ms1, name: "dup", overwrite: false, originalZipFileName: "dup.zip");
        True(ok1);

        // Try installing again without overwrite should fail
        using var ms2 = new MemoryStream(ms1.ToArray());
        ms2.Position = 0;
        var ok2 = ctx.Backend.InstallExtensionZipTac(zoneId: 1, appId: 42, zipStream: ms2, name: "dup", overwrite: false, originalZipFileName: "dup.zip");
        False(ok2);

        // With overwrite should succeed
        using var ms3 = new MemoryStream(ms1.ToArray());
        ms3.Position = 0;
        var ok3 = ctx.Backend.InstallExtensionZipTac(zoneId: 1, appId: 42, zipStream: ms3, name: "dup", overwrite: true, originalZipFileName: "dup.zip");
        True(ok3);
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

            // Register backend dependencies explicitly so they can be resolved by LazySvc later
            services.AddSingleton<ExtensionsReaderBackend>(sp => new ExtensionsReaderBackend(
                sp.GetRequiredService<LazySvc<IAppReaderFactory>>(),
                sp.GetRequiredService<ISite>(),
                sp.GetRequiredService<IAppPathsMicroSvc>(),
                sp.GetRequiredService<LazySvc<IJsonService>>()));

            services.AddSingleton<ExtensionsWriterBackend>(sp => new ExtensionsWriterBackend(
                sp.GetRequiredService<LazySvc<IAppReaderFactory>>(),
                sp.GetRequiredService<ISite>(),
                sp.GetRequiredService<IAppPathsMicroSvc>()));

            services.AddSingleton<ExtensionsZipInstallerBackend>(sp => new ExtensionsZipInstallerBackend(
                sp.GetRequiredService<LazySvc<IAppReaderFactory>>(),
                sp.GetRequiredService<ISite>(),
                sp.GetRequiredService<IAppPathsMicroSvc>(),
                sp.GetRequiredService<IGlobalConfiguration>()));

            // Register LazySvc wrappers for the backend classes themselves
            services.AddSingleton(sp => new LazySvc<ExtensionsReaderBackend>(sp));
            services.AddSingleton(sp => new LazySvc<ExtensionsWriterBackend>(sp));
            services.AddSingleton(sp => new LazySvc<ExtensionsZipInstallerBackend>(sp));

            var sp = services.BuildServiceProvider() as ServiceProvider ?? throw new InvalidOperationException("Failed to build service provider");

            // Configure global folder as previous tests relied on it for path resolution helpers
            var globalConfig = sp.GetRequiredService<IGlobalConfiguration>();
            globalConfig.GlobalFolder(tempRoot);

            // Resolve LazySvc backends
            var readerLazy = sp.GetRequiredService<LazySvc<ExtensionsReaderBackend>>();
            var writerLazy = sp.GetRequiredService<LazySvc<ExtensionsWriterBackend>>();
            var zipLazy = sp.GetRequiredService<LazySvc<ExtensionsZipInstallerBackend>>();

            var backend = new ExtensionsBackend(readerLazy, writerLazy, zipLazy);

            var jsonSvc = sp.GetRequiredService<IJsonService>();

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

    private class FakeGlobalConfiguration : IGlobalConfiguration
    {
        private readonly Dictionary<string, string?> _values = new();
        private readonly string _tempFolder;

        public FakeGlobalConfiguration()
        {
            // Create a unique temp folder for this test run
            _tempFolder = Path.Combine(Path.GetTempPath(), "2sxc-test-temp", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempFolder);
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

        public string ConnectionString() => GetThis() ?? "";
        public IGlobalConfiguration ConnectionString(string value) { SetThis(value); return this; }
        public string GlobalFolder() => GetThis() ?? "";
        public IGlobalConfiguration GlobalFolder(string path) { SetThis(path); return this; }
        public string AssetsVirtualUrl() => GetThis() ?? "/assets/";
        public IGlobalConfiguration AssetsVirtualUrl(string path) { SetThis(path); return this; }
        public string SharedAppsFolder() => GetThis() ?? "/shared/";
        public IGlobalConfiguration SharedAppsFolder(string path) { SetThis(path); return this; }
        public string AppDataTemplateFolder() => GetThis() ?? "";
        public IGlobalConfiguration AppDataTemplateFolder(string path) { SetThis(path); return this; }
        public string DataFolder() => GetThis() ?? "";
        public IGlobalConfiguration DataFolder(string path) { SetThis(path); return this; }
        public string TemporaryFolder() => _tempFolder;
        public IGlobalConfiguration TemporaryFolder(string path) { SetThis(path); return this; }
        public string InstructionsFolder() => GetThis() ?? "";
        public IGlobalConfiguration InstructionsFolder(string path) { SetThis(path); return this; }
        public string TempAssemblyFolder() => GetThis() ?? "";
        public IGlobalConfiguration TempAssemblyFolder(string path) { SetThis(path); return this; }
        public string CshtmlAssemblyFolder() => GetThis() ?? "";
        public IGlobalConfiguration CshtmlAssemblyFolder(string path) { SetThis(path); return this; }
        public string CryptoFolder() => GetThis() ?? "";
        public IGlobalConfiguration CryptoFolder(string path) { SetThis(path); return this; }
    }
}
