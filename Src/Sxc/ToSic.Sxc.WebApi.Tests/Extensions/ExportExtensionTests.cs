using System.IO.Compression;
using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Sys;
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
        ctx.SetupExtension(extName, new ExtensionManifest { Version = version, IsInstalled = false, HasAppCode = false, DataInside = false });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
        Assert.NotNull(result);
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        Assert.NotNull(fileResult);
        Assert.NotEmpty(fileResult.FileContents);
        Assert.Contains(fileResult.ContentType, new[] { "application/zip", "application/octet-stream" });
        Assert.Contains($"app-extension-{extName}-v{version}.zip", fileResult.FileDownloadName);
#endif
    }

    [Fact]
    public void Export_VerifyZipContainsExtensionJson()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith("/App_Data/extension.json", StringComparison.OrdinalIgnoreCase));
#endif
    }

    [Fact]
    public void Export_VerifyZipContainsLockJson()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        Assert.Contains(zip.Entries, e => e.FullName.EndsWith("/App_Data/extension.lock.json", StringComparison.OrdinalIgnoreCase));
#endif
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
        var extDir = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, extName);
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

#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var config = GetJsonFileFromZip(zip, "/App_Data/extension.json");
        Assert.True(config.ContainsKey("isInstalled"));
        Assert.True(config.GetBool("isInstalled"));
#endif
    }

    [Fact]
    public void Export_SetsIsInstalledToTrue_WhenMissing()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var config = GetJsonFileFromZip(zip, "/App_Data/extension.json");
        Assert.True(config.ContainsKey("isInstalled"));
        Assert.True(config.GetBool("isInstalled"));
#endif
    }

    [Fact]
    public void Export_DoesNotModifyOriginalExtensionJson()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0", IsInstalled = false });
        
        var originalPath = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, extName, 
            FolderConstants.DataFolderProtected, FolderConstants.AppExtensionJsonFile);
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
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0", HasAppCode = true });
        ctx.CreateAppCodeFiles(extName, 
            ("Helper.cs", "// test helper"),
            ("Service.cs", "// test service"));
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.Contains(zip.Entries, e => e.FullName.Contains($"AppCode/Extensions/{extName}/Helper.cs", StringComparison.InvariantCultureIgnoreCase));
        Assert.Contains(zip.Entries, e => e.FullName.Contains($"AppCode/Extensions/{extName}/Service.cs", StringComparison.InvariantCultureIgnoreCase));
#endif
    }

    [Fact]
    public void Export_ExcludesAppCode_WhenHasAppCodeFalse()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "without-appcode";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0", HasAppCode = false });
        ctx.CreateAppCodeFiles(extName, ("Helper.cs", "// should not be included"));
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.DoesNotContain(zip.Entries, e => e.FullName.Contains("AppCode/"));
#endif
    }

    [Fact]
    public void Export_ExcludesAppCode_WhenPropertyMissing()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "no-appcode-property";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.DoesNotContain(zip.Entries, e => e.FullName.Contains("AppCode/"));
#endif
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
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var lockData = GetJsonFileFromZip(zip, "/App_Data/extension.lock.json");
        Assert.True(lockData.ContainsKey("version"));
        Assert.Equal(version, lockData.GetString("version"));
#endif
    }

    [Fact]
    public void Export_LockFileContainsFilesArray()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var extDir = Path.Combine(ctx.TempRoot, FolderConstants.AppExtensionsFolder, extName);
        File.WriteAllText(Path.Combine(extDir, "readme.txt"), "test");
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var lockData = GetJsonFileFromZip(zip, "/App_Data/extension.lock.json");
        Assert.True(lockData.ContainsKey("files"));
        var filesElem = lockData.GetElement("files");
        Assert.True(filesElem.ValueKind == JsonValueKind.Array);
        Assert.True(filesElem.GetArrayLength() > 0);
#endif
    }

    [Fact]
    public void Export_LockFileEntryHasFileAndHash()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var lockEntry = zip.Entries.First(e => e.FullName.EndsWith("/App_Data/extension.lock.json"));
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
#endif
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
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        Assert.Contains(zip.Entries, e => e.FullName.Contains("/dist/script.js"));
        Assert.Contains(zip.Entries, e => e.FullName.Contains("/dist/styles.css"));
#endif
    }

    [Fact]
    public void Export_FilePathsStartWithExtensions()
    {
        using var ctx = ExportExtensionTestContext.Create();
        
        const string extName = "test-extension";
        ctx.SetupExtension(extName, new ExtensionManifest { Version = "1.0.0" });
        
        var result = ctx.ExportBackend.ExportTac(zoneId: 1, appId: 42, name: extName);
        
#if !NETFRAMEWORK
        var fileResult = result as FileContentResult;
        using var zipStream = new MemoryStream(fileResult!.FileContents);
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        foreach (var entry in zip.Entries.Where(e => !string.IsNullOrEmpty(e.Name)))
        {
            if (!entry.FullName.StartsWith("install-package.json")
                && !entry.FullName.StartsWith("App_Data"))
                Assert.StartsWith("extensions/", entry.FullName);
        }
#endif
    }

    #endregion
}
