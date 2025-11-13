using System.IO.Compression;
using System.Text;
using Tests.ToSic.ToSxc.WebApi.Extensions;
using ToSic.Sys.Security.Encryption;

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

    // Additional tests covering validation and lock/hash failure modes

    [Fact]
    public void InstallZip_MissingTopLevelExtensionsFolder_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = zip.CreateEntry($"notextensions/foo/App_Data/extension.json");
            using (var stream = entry.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("{ \"id\":\"foo\", \"isInstalled\": true }");
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "missing-extensions.zip");
        Assert.False(ok);
    }

    [Fact]
    public void InstallZip_MissingLockFile_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "no-lock";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            // Intentionally do not add lock file
            var extra = zip.CreateEntry($"extensions/{folder}/dist/extra.js");
            using (var s2 = extra.Open())
            {
                using var w2 = new StreamWriter(s2, new UTF8Encoding(false));
                w2.Write("console.log('x');");
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "no-lock.zip");
        Assert.False(ok);
    }

    [Fact]
    public void InstallZip_InvalidExtensionJson_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "invalid-json";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                // Missing isInstalled true
                writer.Write($"{{ \"id\":\"{folder}\", \"enabled\": true }}");
            }

            // Provide a lock file so validation still checks extension.json
            var lockJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.lock.json");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                var lockData = new { version = TestVersion, files = new[] { new { file = $"extensions/{folder}/App_Data/extension.json", hash = Sha256.Hash("{}") } } };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "invalid-json.zip");
        Assert.False(ok);
    }

    [Fact]
    public void InstallZip_MissingFilesListedInLock_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "missing-file";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Only include extension.json but lock lists another file
            var extensionJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            var lockJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.lock.json");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new { file = $"extensions/{folder}/App_Data/extension.json", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") },
                        new { file = $"extensions/{folder}/dist/missing.js", hash = Sha256.Hash("console.log('x');") }
                    }
                };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "missing-file.zip");
        Assert.False(ok);
    }

    [Fact]
    public void InstallZip_UnexpectedExtraFiles_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "extra-file";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // extension.json
            var extensionJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            // an extra file not listed in lock
            var extra = zip.CreateEntry($"extensions/{folder}/dist/extra.js");
            using (var s2 = extra.Open())
            {
                using var w2 = new StreamWriter(s2, new UTF8Encoding(false));
                w2.Write("console.log('extra');");
            }

            // lock only lists the extension.json
            var lockJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.lock.json");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                var lockData = new { version = TestVersion, files = new[] { new { file = $"extensions/{folder}/App_Data/extension.json", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") } } };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "extra-file.zip");
        Assert.False(ok);
    }

    [Fact]
    public void InstallZip_HashMismatch_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "bad-hash";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.json");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            var file = zip.CreateEntry($"extensions/{folder}/dist/main.js");
            using (var s = file.Open())
            using (var w = new StreamWriter(s, new UTF8Encoding(false)))
            {
                w.Write("console.log('real');");
            }

            var lockJson = zip.CreateEntry($"extensions/{folder}/App_Data/extension.lock.json");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                // Intentionally put wrong hash for main.js
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new { file = $"extensions/{folder}/App_Data/extension.json", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") },
                        new { file = $"extensions/{folder}/dist/main.js", hash = "0000000000000000000000000000000000000000000000000000000000000000" }
                    }
                };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Backend.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "bad-hash.zip");
        Assert.False(ok);
    }
}
