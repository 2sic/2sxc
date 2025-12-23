using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.App;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for extension read/write backends covering read and write operations
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
        Assert.True(saved);

        // Create another extension folder without a config file for comparison
        var barFolder = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, "bar");
        Directory.CreateDirectory(barFolder);

        var result = ctx.Reader.GetExtensionsTac(TestAppId);
        Assert.NotNull(result);
        Assert.NotNull(result.Extensions);

        var foo = result.Extensions.FirstOrDefault(e => e.Folder == extensionName);
        Assert.NotNull(foo);
        Assert.NotNull(foo.Configuration);
        Assert.Empty(foo.Icon);

        var expectedJson = ExtensionManifestSerializer.Serialize(manifest);
        var actualJson = ctx.JsonSvc.ToJson(foo.Configuration);
        Assert.Equal(expectedJson, actualJson);

        var bar = result.Extensions.FirstOrDefault(e => e.Folder == "bar");
        Assert.NotNull(bar);
        Assert.NotNull(bar.Configuration);
        Assert.Empty(bar.Icon);
    }

    [Fact]
    public void SaveThenRead_WithSampleSimpleExtension_Config_Works()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
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
        Assert.True(saved);

        var result = ctx.Reader.GetExtensionsTac(TestAppId);
        Assert.NotNull(result);
        var item = result.Extensions.FirstOrDefault(e => e.Folder == folder);
        Assert.NotNull(item);
        Assert.NotNull(item.Configuration);
        Assert.Empty(item.Icon);

        var expected = ExtensionManifestSerializer.Serialize(manifest);
        var actual = ctx.JsonSvc.ToJson(item.Configuration);
        Assert.Equal(expected, actual);
    }

    #endregion
}
