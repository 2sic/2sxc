using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using ToSic.Eav.Sys;
using ToSic.Sys.Security.Encryption;
using Tests.ToSic.ToSxc.WebApi.Extensions;
using static ToSic.Sxc.WebApi.Tests.Extensions.ExportExtensionTestHelpers;

namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for ExtensionsZipInstallerBackend focusing on ZIP install scenarios
/// </summary>
public class ExtensionsZipInstallerBackendTest
{
    private const int TestZoneId = 1;
    private const int TestAppId = 42;
    private const string TestVersion = "1.0.0";

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
        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert
        Assert.True(ok);

        var result = ctx.Backend.GetExtensionsTac(TestAppId);
        Assert.Contains(result.Extensions, e => e.Folder == extensionName);
        var cfg = result.Extensions.First(e => e.Folder == extensionName).Configuration;
        Assert.NotNull(cfg);
    }

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
        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert
        Assert.False(ok);
    }

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
        var ok1 = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms1, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert - First install succeeds
        Assert.True(ok1);

        // Act - Try installing again without overwrite should fail
        using var ms2 = new MemoryStream(ms1.ToArray());
        ms2.Position = 0;
        var ok2 = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms2, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert - Second install without overwrite fails
        Assert.False(ok2);

        // Act - With overwrite should succeed
        using var ms3 = new MemoryStream(ms1.ToArray());
        ms3.Position = 0;
        var ok3 = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms3, overwrite: true, originalZipFileName: $"{extensionName}.zip");

        // Assert - Third install with overwrite succeeds
        Assert.True(ok3);
    }
}
