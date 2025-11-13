using System.Text;
using System.Text.Json;
using ToSic.Eav.Sys;
using Tests.ToSic.ToSxc.WebApi.Extensions;
using static ToSic.Sxc.WebApi.Tests.Extensions.ExportExtensionTestHelpers;

namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for ExtensionsBackend service covering read and write operations
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
