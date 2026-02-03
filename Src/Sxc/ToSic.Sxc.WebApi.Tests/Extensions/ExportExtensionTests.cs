using System.IO.Compression;
using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.WebApi.Sys.ImportExport;
using static ToSic.Eav.Sys.FolderConstants;
using static ToSic.Sxc.ImportExport.Package.Sys.PackageIndexFile;
using static ToSic.Sxc.ImportExport.Package.Sys.PackageInstallFile;
using static ToSic.Sxc.WebApi.Tests.Extensions.ExportExtensionTestHelpers;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for ExtensionExportService service
/// </summary>
public class ExportExtensionTests
{
    #region Basic Export Tests

    [Fact]
    public void Export_BasicExtension_CreatesZipStructure()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        const string version = "1.0.0";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = version, IsInstalled = false, AppCodeInside = false, DataInside = false });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        Assert.NotNull(result);
        Assert.NotEmpty(result.FileBytes);
        Assert.Contains(result.ContentType, new[] { "application/zip", "application/octet-stream" });
        Assert.Contains($"app-extension-{extName}-v{version}.zip", result.FileName);
    }

    [Fact]
    public void Export_VerifyZipContainsExtensionJson()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"/{DataFolderProtected}/{AppExtensionJsonFile}", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Export_VerifyZipContainsLockJson()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"/{DataFolderProtected}/{LockFileName}", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Bundled Extensions Tests

    [Fact]
    public void Export_IncludesBundledExtensions_AndListsAllInPackageInstall()
    {
        using var ctx = ExportExtensionTestContext.Create();

        const string extName = "primary-ext";
        ctx.SetupExtension(extName, new ExtensionManifest
        {
            Version = "1.0.0",
            IsInstalled = false,
            AppCodeInside = false,
            DataInside = false,
        });
        ctx.SetExtensionsBundled(extName, "bundle-a,bundle-b");
        ctx.CreateExtensionFiles(extName, ("primary.txt", "primary"));

        ctx.SetupExtension("bundle-a", new ExtensionManifest
        {
            Version = "2.0.0",
            IsInstalled = false,
            AppCodeInside = false,
            DataInside = false,
        });
        ctx.CreateExtensionFiles("bundle-a", ("a.txt", "a"));

        ctx.SetupExtension("bundle-b", new ExtensionManifest
        {
            Version = "3.0.0",
            IsInstalled = true,
            AppCodeInside = false,
            DataInside = false,
        });
        ctx.CreateExtensionFiles("bundle-b", ("b.txt", "b"));

        // Simulate previous installation lock file which should be included unchanged
        const string installedLock = "{\n  \"version\": \"3.0.0\",\n  \"files\": []\n}";
        ctx.WriteInstalledLockFile("bundle-b", installedLock);

        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);

        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        // Verify files from all 3 extensions are present
        Assert.Contains(zip.Entries, e => e.FullName.Equals($"{AppExtensionsFolder}/{extName}/dist/primary.txt", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.Equals($"{AppExtensionsFolder}/bundle-a/dist/a.txt", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.Equals($"{AppExtensionsFolder}/bundle-b/dist/b.txt", StringComparison.OrdinalIgnoreCase));

        // Verify extension.json exists for each
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"{AppExtensionsFolder}/{extName}/{DataFolderProtected}/{AppExtensionJsonFile}", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"{AppExtensionsFolder}/bundle-a/{DataFolderProtected}/{AppExtensionJsonFile}", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"{AppExtensionsFolder}/bundle-b/{DataFolderProtected}/{AppExtensionJsonFile}", StringComparison.OrdinalIgnoreCase));

        // Verify lock file exists for each
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"{AppExtensionsFolder}/{extName}/{DataFolderProtected}/{LockFileName}", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"{AppExtensionsFolder}/bundle-a/{DataFolderProtected}/{LockFileName}", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"{AppExtensionsFolder}/bundle-b/{DataFolderProtected}/{LockFileName}", StringComparison.OrdinalIgnoreCase));

        // Verify installed lock json preserved
        var bundleBLockEntry = zip.Entries.Single(e => e.FullName.EndsWith($"{AppExtensionsFolder}/bundle-b/{DataFolderProtected}/{LockFileName}", StringComparison.OrdinalIgnoreCase));
        using (var lockStream = bundleBLockEntry.Open())
        using (var reader = new StreamReader(lockStream))
        {
            var lockText = reader.ReadToEnd();
            Assert.Contains("\"version\": \"3.0.0\"", lockText);
            Assert.Contains("\"files\": []", lockText);
        }

        // Verify package-install.json lists all extensions
        var package = GetJsonFileFromZip(zip, FileName);
        Assert.True(package.ContainsKey("extensions"));
        var exts = package.GetElement("extensions");
        Assert.Equal(JsonValueKind.Array, exts.ValueKind);
        var names = exts.EnumerateArray().Select(e => e.GetProperty("name").GetString()).ToList();
        Assert.Contains(extName, names);
        Assert.Contains("bundle-a", names);
        Assert.Contains("bundle-b", names);
        Assert.Equal(3, names.Count);
    }

    [Fact]
    public void Export_SkipsMissingBundledExtensions()
    {
        using var ctx = ExportExtensionTestContext.Create();

        const string extName = "primary-ext";
        ctx.SetupExtension(extName, new ExtensionManifest
        {
            Version = "1.0.0",
            IsInstalled = false,
            AppCodeInside = false,
            DataInside = false,
        });

        // One missing, one present
        ctx.SetExtensionsBundled(extName, "text-ext2,bundle-a");

        ctx.SetupExtension("bundle-a", new ExtensionManifest
        {
            Version = "2.0.0",
            IsInstalled = false,
            AppCodeInside = false,
            DataInside = false,
        });

        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);

        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        // Present bundled extension should be included
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith($"{AppExtensionsFolder}/bundle-a/{DataFolderProtected}/{AppExtensionJsonFile}", StringComparison.OrdinalIgnoreCase));

        // Missing bundled extension should not appear anywhere
        Assert.DoesNotContain(zip.Entries, e => e.FullName.Contains($"{AppExtensionsFolder}/text-ext2/", StringComparison.OrdinalIgnoreCase));

        // package-install.json should only list primary + existing bundled
        var package = GetJsonFileFromZip(zip, FileName);
        var exts = package.GetElement("extensions");
        var names = exts.EnumerateArray().Select(e => e.GetProperty("name").GetString()).ToList();
        Assert.Contains(extName, names);
        Assert.Contains("bundle-a", names);
        Assert.DoesNotContain("text-ext2", names);
        Assert.Equal(2, names.Count);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void Export_ThrowsWhenExtensionNotFound()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        Assert.Throws<DirectoryNotFoundException>(() =>
            ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: "nonexistent"));
    }

    [Fact]
    public void Export_ThrowsWhenExtensionJsonMissing()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "incomplete";
        var extDir = Path.Combine(ctx.TempRoot, AppExtensionsFolder, extName);
        Directory.CreateDirectory(extDir);
        
        Assert.Throws<FileNotFoundException>(() =>
            ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName));
    }

    #endregion

    #region Installation Status Tracking

    [Fact]
    public void Export_SetsIsInstalledToTrue_WhenFalse()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0", IsInstalled = false });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);

        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var config = GetJsonFileFromZip(zip, $"/{DataFolderProtected}/{AppExtensionJsonFile}");
        Assert.True(config.ContainsKey("isInstalled"));
        Assert.True(config.GetBool("isInstalled"));
    }

    [Fact]
    public void Export_SetsIsInstalledToTrue_WhenMissing()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var config = GetJsonFileFromZip(zip, $"/{DataFolderProtected}/{AppExtensionJsonFile}");
        Assert.True(config.ContainsKey("isInstalled"));
        Assert.True(config.GetBool("isInstalled"));
    }

    [Fact]
    public void Export_DoesNotModifyOriginalExtensionJson()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0", IsInstalled = false });
        
        var originalPath = Path.Combine(ctx.TempRoot, AppExtensionsFolder, extName, 
            DataFolderProtected, AppExtensionJsonFile);
        var originalContent = File.ReadAllText(originalPath);
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        var afterExportContent = File.ReadAllText(originalPath);
        Assert.Equal(originalContent, afterExportContent);
        
        var originalConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(originalContent);
        Assert.NotNull(originalConfig);
        Assert.False(originalConfig.GetBool("isInstalled"));
    }

    #endregion

    #region Conditional AppCode Inclusion

    [Fact]
    public void Export_IncludesAppCode_WhenHasAppCodeTrue()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "with-appcode";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0", AppCodeInside = true });
        ctx.CreateAppCodeFiles(extName, 
            ("Helper.cs", "// test helper"),
            ("Service.cs", "// test service"));
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.Contains(zip.Entries, e => e.FullName.Contains($"AppCode/Extensions/{extName}/Helper.cs", StringComparison.InvariantCultureIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.Contains($"AppCode/Extensions/{extName}/Service.cs", StringComparison.InvariantCultureIgnoreCase));
    }

    [Fact]
    public void Export_ExcludesAppCode_WhenHasAppCodeFalse()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "without-appcode";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0", AppCodeInside = false });
        ctx.CreateAppCodeFiles(extName, ("Helper.cs", "// should not be included"));
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.DoesNotContain(zip.Entries, e => e.FullName.Contains("AppCode/"));
    }

    [Fact]
    public void Export_ExcludesAppCode_WhenPropertyMissing()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "no-appcode-property";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.DoesNotContain(zip.Entries, e => e.FullName.Contains("AppCode/"));
    }

    #endregion

    #region Extension Integrity Verification (Lock File)

    [Fact]
    public void Export_LockFileHasCorrectVersion()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        const string version = "2.4.7";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = version });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var lockData = GetJsonFileFromZip(zip, $"/{DataFolderProtected}/{LockFileName}");
        Assert.True(lockData.ContainsKey("version"));
        Assert.Equal(version, lockData.GetString("version"));
    }

    [Fact]
    public void Export_LockFileContainsFilesArray()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var extDir = Path.Combine(ctx.TempRoot, AppExtensionsFolder, extName);
        File.WriteAllText(Path.Combine(extDir, "readme.txt"), "test");
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        var lockData = GetJsonFileFromZip(zip, $"/{DataFolderProtected}/{LockFileName}");
        Assert.True(lockData.ContainsKey("files"));
        var filesElem = lockData.GetElement("files");
        Assert.True(filesElem.ValueKind == JsonValueKind.Array);
        Assert.True(filesElem.GetArrayLength() > 0);
    }

    [Fact]
    public void Export_LockFileEntryHasFileAndHash()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        var lockEntry = zip.Entries.First(e => e.FullName.EndsWith($"/{DataFolderProtected}/{LockFileName}"));
        using var lockStream = lockEntry.Open();
        using var reader = new StreamReader(lockStream);
        var lockJson = reader.ReadToEnd();
        
        Assert.Contains("\"file\":", lockJson);
        Assert.Contains("\"hash\":", lockJson);
        
        var lockData = JsonSerializer.Deserialize<Dictionary<string, object>>(lockJson);
        var filesElem = lockData!.GetElement("files");
        var firstFile = filesElem[0];
        var hash = firstFile.GetProperty("hash").GetString();
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length);
        Assert.Matches("^[a-f0-9]{64}$", hash);
    }

    #endregion

    #region ZIP Structure Tests

    [Fact]
    public void Export_ExtensionFilesIncluded()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        ctx.CreateExtensionFiles(extName,
            ("script.js", "console.log('test');"),
            ("styles.css", ".test { color: red; }"));
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.Contains(zip.Entries, e => e.FullName.Contains("/dist/script.js"));
        Assert.Contains(zip.Entries, e => e.FullName.Contains("/dist/styles.css"));
    }

    [Fact]
    public void Export_FilePathsStartWithExtensions()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        using var zipStream = new MemoryStream(result.FileBytes);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        foreach (var entry in zip.Entries.Where(e => !string.IsNullOrEmpty(e.Name)))
        {
            if (!entry.FullName.Equals(FileName, StringComparison.OrdinalIgnoreCase)
                && !entry.FullName.StartsWith(DataFolderProtected))
                Assert.StartsWith($"{AppExtensionsFolder}/", entry.FullName);
        }
    }

    #endregion
}
