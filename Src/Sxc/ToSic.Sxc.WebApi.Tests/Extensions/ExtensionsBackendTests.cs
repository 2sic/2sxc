using System.IO.Compression;
using System.Text;
using System.Text.Json;
using ToSic.Eav.Sys;
using ToSic.Sys.Security.Encryption;
using Tests.ToSic.ToSxc.WebApi.Extensions;
using static ToSic.Sxc.WebApi.Tests.Extensions.ExportExtensionTestHelpers;

namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for ExtensionsBackend service covering read, write, and install operations
/// </summary>
public class ExtensionsBackendTests
{
    #region Constants

    private const int TestZoneId = 1;
    private const int TestAppId = 42;
    private const string TestVersion = "1.0.0";

    #endregion

    #region Save and Read Tests

    [Fact]
    public void SaveThenRead_Roundtrip_Works()
    {
        // Arrange
        using var ctx = ExtensionsBackendTestContext.Create();
        const string extensionName = "foo";
        var config = new { enabled = true, version = TestVersion, list = new[] { 1, 2 } };

        // Ensure extension folder exists before saving
        var fooFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, extensionName);
        Directory.CreateDirectory(fooFolder);

        // Serialize config to JsonElement
        var configJson = ctx.JsonSvc.ToJson(config);
        using var configDoc = JsonDocument.Parse(configJson);
        var configElem = configDoc.RootElement;

        // Act
        var saved = ctx.Backend.SaveExtensionTac(zoneId: TestZoneId, appId: TestAppId, name: extensionName, configuration: configElem);

        // Assert
        Assert.True(saved);

        // Create another extension folder without a config file for comparison
        var barFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, "bar");
        Directory.CreateDirectory(barFolder);

        var result = ctx.Backend.GetExtensionsTac(TestAppId);
        Assert.NotNull(result);
        Assert.NotNull(result.Extensions);

        var foo = result.Extensions.FirstOrDefault(e => e.Folder == extensionName);
        Assert.NotNull(foo);
        Assert.NotNull(foo!.Configuration);

        var expectedJson = ctx.JsonSvc.ToJson(config);
        var actualJson = ctx.JsonSvc.ToJson(foo.Configuration!);
        Assert.Equal(expectedJson, actualJson);

        var bar = result.Extensions.FirstOrDefault(e => e.Folder == "bar");
        Assert.NotNull(bar);
        Assert.NotNull(bar!.Configuration);
    }

    [Fact]
    public void SaveThenRead_WithSampleSimpleExtension_Config_Works()
    {
        // Arrange
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "sample-simple-extension";

        // Ensure extension folder exists before saving
        var folderPath = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, folder);
        Directory.CreateDirectory(folderPath);

        // Parse sample JSON into JsonElement
        using var cfgDoc = JsonDocument.Parse(SampleSimpleJson);
        var cfgElem = cfgDoc.RootElement;

        // Act
        var saved = ctx.Backend.SaveExtensionTac(zoneId: TestZoneId, appId: TestAppId, name: folder, configuration: cfgElem);

        // Assert
        Assert.True(saved);

        var result = ctx.Backend.GetExtensionsTac(TestAppId);
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
        // Arrange
        using var ctx = ExtensionsBackendTestContext.Create();
        const string complexFeatureExtension = "complex-feature-extension";
        const int complexAppId = 99;

        var extDir = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, complexFeatureExtension, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(extDir);
        var jsonPath = Path.Combine(extDir, FolderConstants.AppExtensionJsonFile);

        // Write pretty JSON as it might come from source control
        File.WriteAllText(jsonPath, ComplexFeatureJson, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        // Act
        var result = ctx.Backend.GetExtensionsTac(complexAppId);

        // Assert
        Assert.NotNull(result);
        var item = result.Extensions.FirstOrDefault(e => e.Folder == complexFeatureExtension);
        Assert.NotNull(item);
        Assert.NotNull(item!.Configuration);

        // Normalize both to minified JSON and compare
        var expected = ctx.JsonSvc.ToJson(ctx.JsonSvc.ToObject(ComplexFeatureJson)!);
        var actual = ctx.JsonSvc.ToJson(item.Configuration!);
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Install ZIP Tests

    [Fact]
    public void InstallZip_Simple_Works()
    {
        // Arrange
        using var ctx = ExtensionsBackendTestContext.Create();
        const string extensionName = "color-picker";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Create proper extensions/color-picker/ structure
            var extensionJson = zip.CreateEntry($"extensions/{extensionName}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{extensionName}\", \"enabled\": true, \"isInstalled\": true }}");
            }

            var distFile = zip.CreateEntry($"extensions/{extensionName}/dist/script.js");
            using (var stream = distFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("console.log('ok');");
            }

            // Create lock file
            var lockJson = zip.CreateEntry($"extensions/{extensionName}/App_Data/extension.lock.json");
            using (var stream = lockJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new
                        {
                            file = $"extensions/{extensionName}/App_Data/extension.json",
                            hash = Sha256.Hash($"{{ \"id\":\"{extensionName}\", \"enabled\": true, \"isInstalled\": true }}")
                        },
                        new
                        {
                            file = $"extensions/{extensionName}/dist/script.js",
                            hash = Sha256.Hash("console.log('ok');")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        // Act
        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, name: null, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert
        Assert.True(ok);

        var result = ctx.Backend.GetExtensionsTac(TestAppId);
        Assert.Contains(result.Extensions, e => e.Folder == extensionName);
        var cfg = result.Extensions.First(e => e.Folder == extensionName).Configuration;
        Assert.NotNull(cfg);
    }

    #endregion

    #region Security Tests

    [Fact]
    public void InstallZip_BlocksTraversal()
    {
        // Arrange
        using var ctx = ExtensionsBackendTestContext.Create();
        const string extensionName = "bad";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Create proper structure but with path traversal attempt
            var extensionJson = zip.CreateEntry($"extensions/{extensionName}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}");
            }

            // Try to create a file outside the extension folder
            var badFile = zip.CreateEntry($"extensions/{extensionName}/../../outside.txt");
            using (var stream = badFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("nope");
            }

            // Create lock file (it should fail validation due to the traversal)
            var lockJson = zip.CreateEntry($"extensions/{extensionName}/App_Data/extension.lock.json");
            using (var stream = lockJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new
                        {
                            file = $"extensions/{extensionName}/App_Data/extension.json",
                            hash = Sha256.Hash($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}")
                        },
                        new
                        {
                            file = $"extensions/{extensionName}/../../outside.txt",
                            hash = Sha256.Hash("nope")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        // Act
        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, name: extensionName, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert
        Assert.False(ok);
    }

    #endregion

    #region Overwrite Behavior Tests

    [Fact]
    public void InstallZip_Overwrite_Behavior()
    {
        // Arrange
        using var ctx = ExtensionsBackendTestContext.Create();
        const string extensionName = "dup";

        // Create a properly structured extension ZIP
        using var ms1 = new MemoryStream();
        using (var zip = new ZipArchive(ms1, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Create extensions/dup/ structure
            var extensionJson = zip.CreateEntry($"extensions/{extensionName}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}");
            }

            // Create a simple file to include
            var distFile = zip.CreateEntry($"extensions/{extensionName}/dist/main.js");
            using (var stream = distFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"console.log('{extensionName}');");
            }

            // Add another placeholder asset to ensure multi-file lock scenarios
            var placeholder = zip.CreateEntry($"extensions/{extensionName}/readme.txt");
            using (var stream = placeholder.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{extensionName} extension placeholder");
            }

            // Create lock file with file hashes (must include all installed files except the lock itself)
            var lockJson = zip.CreateEntry($"extensions/{extensionName}/App_Data/extension.lock.json");
            using (var stream = lockJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new
                        {
                            file = $"extensions/{extensionName}/App_Data/extension.json",
                            hash = Sha256.Hash($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}")
                        },
                        new
                        {
                            file = $"extensions/{extensionName}/dist/main.js",
                            hash = Sha256.Hash($"console.log('{extensionName}');")
                        },
                        new
                        {
                            file = $"extensions/{extensionName}/readme.txt",
                            hash = Sha256.Hash($"{extensionName} extension placeholder")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms1.Position = 0;

        // Act - First install
        var ok1 = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms1, name: extensionName, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert - First install succeeds
        Assert.True(ok1);

        // Act - Try installing again without overwrite should fail
        using var ms2 = new MemoryStream(ms1.ToArray());
        ms2.Position = 0;
        var ok2 = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms2, name: extensionName, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert - Second install without overwrite fails
        Assert.False(ok2);

        // Act - With overwrite should succeed
        using var ms3 = new MemoryStream(ms1.ToArray());
        ms3.Position = 0;
        var ok3 = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms3, name: extensionName, overwrite: true, originalZipFileName: $"{extensionName}.zip");

        // Assert - Third install with overwrite succeeds
        Assert.True(ok3);
    }

    #endregion

    #region Test Data

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

    #endregion
}
