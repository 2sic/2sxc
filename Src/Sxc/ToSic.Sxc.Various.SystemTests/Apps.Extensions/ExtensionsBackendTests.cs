using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Apps.Sys.Extensions;
using ToSic.Eav.Services;
using ToSic.Eav.Sys;
using ToSic.Sxc.Apps.Extensions.Sys;
using ToSic.Sxc.Apps.Extensions.Sys.Inspect;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using ToSic.Sys.Configuration;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for extension read/write backends covering read and write operations
/// </summary>
[Startup(typeof(StartupExtensionsTests))]
public class ExtensionsBackendTests(
    LazySvc<IAppReaderFactory> appReadersLazy,
    LazySvc<IJsonService> jsonLazy,
    IJsonService jsonSvc,
    IGlobalConfiguration globalConfiguration,
    ExtensionManifestService manifestService,
    LazySvc<ExtensionInspectBackend> inspectorLazy,
    IDataSourceGenerator<AppEditions> appEditions,
    LazySvc<IAppCachePurge> appCachePurgerLazy,
    ExtensionsTestAppJsonConfigurationService appJsonService)
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
        using var ctx = CreateContext();
        const string extensionName = "test";
        var manifest = new ExtensionManifest
        {
            Version = TestVersion,
            IsInstalled = true
        };

        // Ensure extension folder exists before saving
        var fooFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, extensionName);
        Directory.CreateDirectory(fooFolder);

        // Act
        var saved = ctx.Writer.SaveExtensionTac(zoneId: TestZoneId, appId: TestAppId, name: extensionName, manifest: manifest);

        // Assert
        True(saved);

        // Create another extension folder without a config file for comparison
        var barFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, "bar");
        Directory.CreateDirectory(barFolder);

        var result = ctx.Reader.GetExtensionsTac(TestAppId);
        NotNull(result);
        NotNull(result);

        var foo = result.FirstOrDefault(e => e.Folder == extensionName);
        NotNull(foo);
        NotNull(foo.Configuration);
        Empty(foo.Icon);

        var expectedJson = ExtensionManifestSerializer.Serialize(manifest);
        var actualJson = ctx.JsonSvc.ToJson(foo.Configuration);
        Equal(expectedJson, actualJson);

        var bar = result.FirstOrDefault(e => e.Folder == "bar");
        NotNull(bar);
        NotNull(bar.Configuration);
        Empty(bar.Icon);
    }

    [Fact]
    public void SaveThenRead_WithSampleSimpleExtension_Config_Works()
    {
        using var ctx = CreateContext();
        const string folder = "test";
        var folderPath = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, folder);
        Directory.CreateDirectory(folderPath);

        var manifest = new ExtensionManifest
        {
            Version = "00.00.03",
            IsInstalled = false,
            //InputTypeInside = "empty-app-hello-world-simple",
            Name = "empty-app-hello-world-simple",
            EditionsSupported = false
        };

        var saved = ctx.Writer.SaveExtensionTac(zoneId: TestZoneId, appId: TestAppId, name: folder, manifest: manifest);
        True(saved);

        var result = ctx.Reader.GetExtensionsTac(TestAppId);
        NotNull(result);
        var item = result.FirstOrDefault(e => e.Folder == folder);
        NotNull(item);
        NotNull(item.Configuration);
        Empty(item.Icon);

        var expected = ExtensionManifestSerializer.Serialize(manifest);
        var actual = ctx.JsonSvc.ToJson(item.Configuration);
        Equal(expected, actual);
    }

    #endregion

    private ExtensionsBackendTestContext CreateContext()
        => ExtensionsBackendTestContext.Create(
            appReadersLazy,
            jsonLazy,
            jsonSvc,
            globalConfiguration,
            manifestService,
            inspectorLazy,
            appEditions,
            appCachePurgerLazy,
            appJsonService);
}
