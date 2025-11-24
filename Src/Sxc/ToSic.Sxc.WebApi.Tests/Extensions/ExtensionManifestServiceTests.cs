using System.Text;
using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for ExtensionManifestService serialization behavior
/// </summary>
public class ExtensionManifestServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ExtensionManifestService _service;

    public ExtensionManifestServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "extension-manifest-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        _service = new ExtensionManifestService();
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); } catch { /* Ignore cleanup errors */ }
    }

    #region JsonElement Serialization Tests

    [Fact]
    public void LoadManifest_JsonElements_CanBeSerializedWithoutError()
    {
        // Arrange
        var manifestDir = Path.Combine(_tempDir, "test-extension");
        var dataDir = Path.Combine(manifestDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var manifestPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var manifestJson = @"{
            ""inputTypeInside"": ""string-font-icon"",
            ""inputTypeAssets"": {
                ""default"": ""index.js""
            },
            ""version"": ""1.0.0"",
            ""dataBundles"": [""bundle-guid-1"", ""bundle-guid-2""],
            ""releases"": [
                {
                    ""version"": ""1.0.0"",
                    ""breaking"": false,
                    ""notes"": ""Initial release""
                }
            ]
        }";
        File.WriteAllText(manifestPath, manifestJson, new UTF8Encoding(false));

        // Act
        var manifest = _service.LoadManifestTac(new FileInfo(manifestPath));
        
        // Assert - should not throw when serializing
        Assert.NotNull(manifest);
        var exception = Record.Exception(() =>
        {
            var serialized = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
            Assert.NotEmpty(serialized);
        });
        
        Assert.Null(exception);
    }

    [Fact]
    public void LoadManifest_ComplexJsonElements_PreserveStructure()
    {
        // Arrange
        var manifestDir = Path.Combine(_tempDir, "complex-extension");
        var dataDir = Path.Combine(manifestDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var manifestPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var manifestJson = @"{
            ""inputTypeInside"": ""string-wysiwyg"",
            ""inputTypeAssets"": [
                ""styles.css"",
                ""editor.js"",
                ""config.json""
            ],
            ""version"": ""2.5.1"",
            ""editionsSupported"": true,
            ""dataBundles"": [
                {
                    ""guid"": ""abc-123"",
                    ""name"": ""Template Data"",
                    ""type"": ""ContentType""
                }
            ],
            ""releases"": [
                {
                    ""version"": ""2.5.1"",
                    ""breaking"": true,
                    ""notes"": ""Major refactor""
                },
                {
                    ""version"": ""2.0.0"",
                    ""breaking"": false,
                    ""notes"": ""Stable release""
                }
            ]
        }";
        File.WriteAllText(manifestPath, manifestJson, new UTF8Encoding(false));

        // Act
        var manifest = _service.LoadManifestTac(new FileInfo(manifestPath));
        
        // Assert
        Assert.NotNull(manifest);
        Assert.Equal("string-wysiwyg", manifest.InputTypeInside);
        Assert.Equal("2.5.1", manifest.Version);
        Assert.True(manifest.EditionsSupported);
        
        // Verify JsonElements can be accessed and re-serialized
        var serialized = JsonSerializer.Serialize(manifest);
        Assert.Contains("string-wysiwyg", serialized);
        Assert.Contains("2.5.1", serialized);
    }

    [Fact]
    public void LoadManifest_MultipleLoads_ProduceIndependentManifests()
    {
        // Arrange
        var manifestDir = Path.Combine(_tempDir, "multi-load-test");
        var dataDir = Path.Combine(manifestDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var manifestPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var manifestJson = @"{
            ""inputTypeInside"": ""string-test"",
            ""version"": ""1.0.0"",
            ""inputTypeAssets"": { ""js"": ""main.js"" }
        }";
        File.WriteAllText(manifestPath, manifestJson, new UTF8Encoding(false));

        // Act - load the same manifest twice
        var manifest1 = _service.LoadManifestTac(new FileInfo(manifestPath));
        var manifest2 = _service.LoadManifestTac(new FileInfo(manifestPath));
        
        // Assert - both should serialize independently without errors
        Assert.NotNull(manifest1);
        Assert.NotNull(manifest2);
        
        var json1 = JsonSerializer.Serialize(manifest1);
        var json2 = JsonSerializer.Serialize(manifest2);
        
        Assert.NotEmpty(json1);
        Assert.NotEmpty(json2);
        Assert.Contains("string-test", json1);
        Assert.Contains("string-test", json2);
    }

    [Fact]
    public void LoadManifest_EmptyJsonElements_HandleCorrectly()
    {
        // Arrange
        var manifestDir = Path.Combine(_tempDir, "minimal-extension");
        var dataDir = Path.Combine(manifestDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var manifestPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var manifestJson = @"{
            ""inputTypeInside"": ""string-minimal"",
            ""version"": ""1.0.0""
        }";
        File.WriteAllText(manifestPath, manifestJson, new UTF8Encoding(false));

        // Act
        var manifest = _service.LoadManifestTac(new FileInfo(manifestPath));
        
        // Assert - should handle undefined/missing JsonElements
        Assert.NotNull(manifest);
        var serialized = JsonSerializer.Serialize(manifest);
        Assert.NotEmpty(serialized);
    }

    #endregion

    #region Load Basic Tests

    [Fact]
    public void LoadManifest_ValidFile_ReturnsManifest()
    {
        // Arrange
        var manifestDir = Path.Combine(_tempDir, "valid-extension");
        var dataDir = Path.Combine(manifestDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var manifestPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var manifestJson = @"{
            ""inputTypeInside"": ""string-font-icon"",
            ""version"": ""1.2.3"",
            ""editionsSupported"": true
        }";
        File.WriteAllText(manifestPath, manifestJson, new UTF8Encoding(false));

        // Act
        var result = _service.LoadManifestTac(new FileInfo(manifestPath));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("string-font-icon", result.InputTypeInside);
        Assert.Equal("1.2.3", result.Version);
        Assert.True(result.EditionsSupported);
    }

    [Fact]
    public void LoadManifest_EmptyFile_ReturnsNull()
    {
        // Arrange
        var manifestDir = Path.Combine(_tempDir, "empty-extension");
        var dataDir = Path.Combine(manifestDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var manifestPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        File.WriteAllText(manifestPath, "", new UTF8Encoding(false));

        // Act
        var result = _service.LoadManifestTac(new FileInfo(manifestPath));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void LoadManifest_InvalidJson_ReturnsNull()
    {
        // Arrange
        var manifestDir = Path.Combine(_tempDir, "invalid-extension");
        var dataDir = Path.Combine(manifestDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);

        var manifestPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        File.WriteAllText(manifestPath, "{ invalid json }", new UTF8Encoding(false));

        // Act
        var result = _service.LoadManifestTac(new FileInfo(manifestPath));

        // Assert
        Assert.Null(result);
    }

    #endregion
}
