using System.Text;
using System.Text.Json;
using ToSic.Eav.Apps.Mocks;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Services;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test context for ExtensionReaderBackend tests
/// </summary>
internal sealed class ExtensionsReaderTestContext : IDisposable
{
    #region Properties

    public string TempRoot { get; }
    public ExtensionReaderBackend ReaderBackend { get; }

    #endregion

    #region Constructor / Factory

    private readonly ExtensionsTestAppJsonConfigurationService _appJsonService;

    private ExtensionsReaderTestContext(string tempRoot, ExtensionReaderBackend readerBackend, ExtensionsTestAppJsonConfigurationService appJsonService)
    {
        TempRoot = tempRoot;
        ReaderBackend = readerBackend;
        _appJsonService = appJsonService;
    }

    public static ExtensionsReaderTestContext Create(
        LazySvc<IAppReaderFactory> appReadersLazy,
        LazySvc<IJsonService> jsonLazy,
        ExtensionManifestService manifestService,
        IDataSourceGenerator<AppEditions> appEditions,
        ExtensionsTestAppJsonConfigurationService appJsonService)
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-extensions-reader-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        appJsonService.UseAppRoot(tempRoot);

        var site = MockSiteTestHelpers.CreateSite(tempRoot);
        var appPathSvc = new MockAppPathsMicroSvc(tempRoot);

        var readerBackend = new ExtensionReaderBackend(
            appReadersLazy, 
            site, 
            appPathSvc, 
            jsonLazy,
            manifestService,
            appEditions);

        return new ExtensionsReaderTestContext(tempRoot, readerBackend, appJsonService);
    }

    #endregion

    #region Setup Helpers

    /// <summary>
    /// Setup a test extension with given configuration in the primary extensions folder
    /// </summary>
    public void SetupExtension(string name, object config)
    {
        _appJsonService.EnsureEdition(string.Empty);

        var extDir = Path.Combine(TempRoot, FolderConstants.AppExtensionsFolder, name);
        var dataDir = Path.Combine(extDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);
            
        var jsonPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var json = config is ExtensionManifest manifest 
            ? ExtensionManifestSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true })
            : JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    }

    /// <summary>
    /// Setup an edition of an extension with given configuration
    /// </summary>
    public void SetupEdition(string editionName, string extensionName, object config)
    {
        _appJsonService.EnsureEdition(editionName);

        var editionExtDir = Path.Combine(TempRoot, editionName, FolderConstants.AppExtensionsFolder, extensionName);
        var dataDir = Path.Combine(editionExtDir, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(dataDir);
            
        var jsonPath = Path.Combine(dataDir, FolderConstants.AppExtensionJsonFile);
        var json = config is ExtensionManifest manifest 
            ? ExtensionManifestSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true })
            : JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
    }

    /// <summary>
    /// Create an edition folder structure without manifest (for negative testing)
    /// </summary>
    public void CreateEditionFolderOnly(string editionName, string extensionName)
    {
        _appJsonService.EnsureEdition(editionName);

        var editionExtDir = Path.Combine(TempRoot, editionName, FolderConstants.AppExtensionsFolder, extensionName);
        Directory.CreateDirectory(editionExtDir);
    }

    #endregion

    #region Disposal

    public void Dispose()
    {
        try { Directory.Delete(TempRoot, recursive: true); } catch { /* Ignore */ }
    }

    #endregion
}
