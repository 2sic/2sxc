using System.IO.Compression;
using System.Text;
using ToSic.Sys.Security.Encryption;
using static ToSic.Eav.Sys.FolderConstants;
using static ToSic.Sxc.ImportExport.Package.Sys.PackageIndexFile;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for ExtensionInstallBackend focusing on ZIP install scenarios
/// </summary>
public class ExtensionInstallBackendTest
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
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{extensionName}\", \"enabled\": true, \"isInstalled\": true }}");
            }

            var distFile = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/dist/script.js");
            using (var stream = distFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("console.log('ok');");
            }

            // Create lock file
            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{LockFileName}");
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
                            file = $"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{AppExtensionJsonFile}",
                            hash = Sha256.Hash($"{{ \"id\":\"{extensionName}\", \"enabled\": true, \"isInstalled\": true }}")
                        },
                        new
                        {
                            file = $"{AppExtensionsFolder}/{extensionName}/dist/script.js",
                            hash = Sha256.Hash("console.log('ok');")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        // Act
        var ok = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert
        Assert.True(ok);

        var result = ctx.Reader.GetExtensionsTac(TestAppId);
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
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}");
            }

            // Try to create a file outside the extension folder
            var badFile = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/../../outside.txt");
            using (var stream = badFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("nope");
            }

            // Create lock file (it should fail validation due to the traversal)
            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{LockFileName}");
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
                            file = $"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{AppExtensionJsonFile}",
                            hash = Sha256.Hash($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}")
                        },
                        new
                        {
                            file = $"{AppExtensionsFolder}/{extensionName}/../../outside.txt",
                            hash = Sha256.Hash("nope")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: $"{extensionName}.zip"));
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
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}");
            }

            // Create a simple file to include
            var distFile = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/dist/main.js");
            using (var stream = distFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"console.log('{extensionName}');");
            }

            // Add another placeholder asset to ensure multi-file lock scenarios
            var placeholder = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/readme.txt");
            using (var stream = placeholder.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{extensionName} extension placeholder");
            }

            // Create lock file with file hashes (must include all installed files except the lock itself)
            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{LockFileName}");
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
                            file = $"{AppExtensionsFolder}/{extensionName}/{DataFolderProtected}/{AppExtensionJsonFile}",
                            hash = Sha256.Hash($"{{ \"id\":\"{extensionName}\", \"isInstalled\": true }}")
                        },
                        new
                        {
                            file = $"{AppExtensionsFolder}/{extensionName}/dist/main.js",
                            hash = Sha256.Hash($"console.log('{extensionName}');")
                        },
                        new
                        {
                            file = $"{AppExtensionsFolder}/{extensionName}/readme.txt",
                            hash = Sha256.Hash($"{extensionName} extension placeholder")
                        }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms1.Position = 0;

        // Act - First install
        var ok1 = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms1, overwrite: false, originalZipFileName: $"{extensionName}.zip");

        // Assert - First install succeeds
        Assert.True(ok1);

        // Act - Try installing again without overwrite should fail
        using var ms2 = new MemoryStream(ms1.ToArray());
        ms2.Position = 0;
        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms2, overwrite: false, originalZipFileName: $"{extensionName}.zip"));

        // Act - With overwrite should succeed
        using var ms3 = new MemoryStream(ms1.ToArray());
        ms3.Position = 0;
        var ok3 = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms3, overwrite: true, originalZipFileName: $"{extensionName}.zip");

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
            var entry = zip.CreateEntry($"notextensions/foo/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = entry.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("{ \"id\":\"foo\", \"isInstalled\": true }");
            }
        }
        ms.Position = 0;

        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "missing-extensions.zip"));
    }

    [Fact]
    public void InstallZip_MissingLockFile_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "no-lock";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            // Intentionally do not add lock file
            var extra = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/dist/extra.js");
            using (var s2 = extra.Open())
            {
                using var w2 = new StreamWriter(s2, new UTF8Encoding(false));
                w2.Write("console.log('x');");
            }
        }
        ms.Position = 0;

        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "no-lock.zip"));
    }

    [Fact]
    public void InstallZip_InvalidExtensionJson_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "invalid-json";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                // Missing isInstalled true
                writer.Write($"{{ \"id\":\"{folder}\", \"enabled\": true }}");
            }

            // Provide a lock file so validation still checks extension.json
            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                var lockData = new { version = TestVersion, files = new[] { new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash("{}") } } };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "invalid-json.zip"));
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
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") },
                        new { file = $"{AppExtensionsFolder}/{folder}/dist/missing.js", hash = Sha256.Hash("console.log('x');") }
                    }
                };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "missing-file.zip"));
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
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            // an extra file not listed in lock
            var extra = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/dist/extra.js");
            using (var s2 = extra.Open())
            {
                using var w2 = new StreamWriter(s2, new UTF8Encoding(false));
                w2.Write("console.log('extra');");
            }

            // lock only lists the extension.json
            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                var lockData = new { version = TestVersion, files = new[] { new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") } } };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "extra-file.zip"));
    }

    [Fact]
    public void InstallZip_HashMismatch_Fails()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "bad-hash";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            var file = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/dist/main.js");
            using (var s = file.Open())
            using (var w = new StreamWriter(s, new UTF8Encoding(false)))
            {
                w.Write("console.log('real');");
            }

            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
            using (var lstream = lockJson.Open())
            {
                using var lw = new StreamWriter(lstream, new UTF8Encoding(false));
                // Intentionally put wrong hash for main.js
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") },
                        new { file = $"{AppExtensionsFolder}/{folder}/dist/main.js", hash = "0000000000000000000000000000000000000000000000000000000000000000" }
                    }
                };
                lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "bad-hash.zip"));
    }

    [Fact]
    public void InstallZip_WithAppCodeFiles()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "with-appcode";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }

            var jsFile = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/dist/script.js");
            using (var stream = jsFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("console.log('ac');");
            }

            var codeFile = zip.CreateEntry($"AppCode/extensions/{folder}/Helper.cs");
            using (var stream = codeFile.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write("public class Helper { public static string Hi() => \"hi\"; }");
            }

            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
            using (var stream = lockJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var lockData = new
                {
                    version = TestVersion,
                    files = new[]
                    {
                        new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") },
                        new { file = $"{AppExtensionsFolder}/{folder}/dist/script.js", hash = Sha256.Hash("console.log('ac');") },
                        new { file = $"AppCode/extensions/{folder}/Helper.cs", hash = Sha256.Hash("public class Helper { public static string Hi() => \"hi\"; }") }
                    }
                };
                writer.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "with-appcode.zip");
        Assert.True(ok);
        var result = ctx.Reader.GetExtensionsTac(TestAppId);
        Assert.Contains(result.Extensions, e => e.Folder == folder);
    }

    [Fact]
    public void InstallZip_MultipleExtensions()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string a = "ext-a";
        const string b = "ext-b";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            void Add(string folder)
            {
                var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
                using (var stream = extensionJson.Open())
                {
                    using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                    writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
                }
                var asset = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/dist/asset.js");
                using (var s = asset.Open())
                {
                    using var w = new StreamWriter(s, new UTF8Encoding(false));
                    w.Write($"console.log('{folder}');");
                }
                var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
                using (var ls = lockJson.Open())
                {
                    using var lw = new StreamWriter(ls, new UTF8Encoding(false));
                    var lockData = new
                    {
                        version = TestVersion,
                        files = new[]
                        {
                            new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") },
                            new { file = $"{AppExtensionsFolder}/{folder}/dist/asset.js", hash = Sha256.Hash($"console.log('{folder}');") }
                        }
                    };
                    lw.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
                }
            }

            Add(a);
            Add(b);
        }
        ms.Position = 0;

        var ok = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "multi.zip");
        Assert.True(ok);

        var result = ctx.Reader.GetExtensionsTac(TestAppId);
        Assert.Contains(result.Extensions, e => e.Folder == a);
        Assert.Contains(result.Extensions, e => e.Folder == b);
    }

    [Fact]
    public void InstallZip_MultipleExtensions_OneInvalid_FailsAndNoneInstalled()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string good = "ext-good";
        const string bad = "ext-bad";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Good extension
            var gJson = zip.CreateEntry($"{AppExtensionsFolder}/{good}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var s = gJson.Open())
            {
                using var w = new StreamWriter(s, new UTF8Encoding(false));
                w.Write($"{{ \"id\":\"{good}\", \"isInstalled\": true }}");
            }
            var gScript = zip.CreateEntry($"{AppExtensionsFolder}/{good}/dist/a.js");
            using (var s = gScript.Open())
            {
                using var w = new StreamWriter(s, new UTF8Encoding(false));
                w.Write("console.log('g');");
            }
            var gLock = zip.CreateEntry($"{AppExtensionsFolder}/{good}/{DataFolderProtected}/{LockFileName}");
            using (var s = gLock.Open())
            {
                using var w = new StreamWriter(s, new UTF8Encoding(false));
                var lockData = new { version = TestVersion, files = new[] { new { file = $"{AppExtensionsFolder}/{good}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{good}\", \"isInstalled\": true }}") }, new { file = $"{AppExtensionsFolder}/{good}/dist/a.js", hash = Sha256.Hash("console.log('g');") } } };
                w.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }

            // Bad extension (missing lock file)
            var bJson = zip.CreateEntry($"{AppExtensionsFolder}/{bad}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var s = bJson.Open())
            {
                using var w = new StreamWriter(s, new UTF8Encoding(false));
                w.Write($"{{ \"id\":\"{bad}\", \"isInstalled\": true }}");
            }
        }
        ms.Position = 0;

        Assert.Throws<InvalidOperationException>(() =>
            ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "multi-invalid.zip"));

        var result = ctx.Reader.GetExtensionsTac(TestAppId);
        Assert.DoesNotContain(result.Extensions, e => e.Folder == good);
        Assert.DoesNotContain(result.Extensions, e => e.Folder == bad);
    }

    [Fact]
    public void InstallZip_LockFileCopiedAndReadOnly()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "lockfile";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
            using (var stream = extensionJson.Open())
            {
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
            }
            var asset = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/dist/x.js");
            using (var s = asset.Open())
            {
                using var w = new StreamWriter(s, new UTF8Encoding(false));
                w.Write("console.log('x');");
            }
            var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
            using (var s = lockJson.Open())
            {
                using var w = new StreamWriter(s, new UTF8Encoding(false));
                var lockData = new { version = TestVersion, files = new[] { new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") }, new { file = $"{AppExtensionsFolder}/{folder}/dist/x.js", hash = Sha256.Hash("console.log('x');") } } };
                w.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
            }
        }
        ms.Position = 0;

        var ok = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: ms, overwrite: false, originalZipFileName: "lockfile.zip");
        Assert.True(ok);

        var lockPath = Path.Combine(ctx.TempRoot, AppExtensionsFolder, folder, DataFolderProtected, LockFileName);
        Assert.True(File.Exists(lockPath));
        Assert.True(File.GetAttributes(lockPath).HasFlag(FileAttributes.ReadOnly));
    }

    [Fact]
    public void InstallZip_Overwrite_ReplacesPreviousFileContent()
    {
        using var ctx = ExtensionsBackendTestContext.Create();
        const string folder = "replace";

        MemoryStream MakeZip(string jsContent)
        {
            var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                var extensionJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}");
                using (var stream = extensionJson.Open())
                {
                    using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                    writer.Write($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}");
                }
                var asset = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/dist/main.js");
                using (var s = asset.Open())
                {
                    using var w = new StreamWriter(s, new UTF8Encoding(false));
                    w.Write(jsContent);
                }
                var lockJson = zip.CreateEntry($"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{LockFileName}");
                using (var s = lockJson.Open())
                {
                    using var w = new StreamWriter(s, new UTF8Encoding(false));
                    var lockData = new { version = TestVersion, files = new[] { new { file = $"{AppExtensionsFolder}/{folder}/{DataFolderProtected}/{AppExtensionJsonFile}", hash = Sha256.Hash($"{{ \"id\":\"{folder}\", \"isInstalled\": true }}") }, new { file = $"{AppExtensionsFolder}/{folder}/dist/main.js", hash = Sha256.Hash(jsContent) } } };
                    w.Write(ctx.JsonSvc.ToJson(lockData, indentation: 2));
                }
            }
            ms.Position = 0;
            return ms;
        }

        var first = MakeZip("console.log('v1');");
        var ok1 = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: first, overwrite: false, originalZipFileName: "replace-v1.zip");
        Assert.True(ok1);

        var filePath = Path.Combine(ctx.TempRoot, AppExtensionsFolder, folder, "dist", "main.js");
        Assert.True(File.Exists(filePath));
        Assert.Equal("console.log('v1');", File.ReadAllText(filePath));

        var second = MakeZip("console.log('v2');");
        var ok2 = ctx.Zip.InstallExtensionZipTac(zoneId: TestZoneId, appId: TestAppId, zipStream: second, overwrite: true, originalZipFileName: "replace-v2.zip");
        Assert.True(ok2);
        Assert.Equal("console.log('v2');", File.ReadAllText(filePath));
    }
}
