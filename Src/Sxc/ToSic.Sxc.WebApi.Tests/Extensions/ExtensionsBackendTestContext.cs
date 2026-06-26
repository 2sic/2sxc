using ToSic.Eav.Apps.Mocks;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Services;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using ToSic.Sys.Configuration;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test context for ExtensionsBackend tests providing setup/teardown and test extension creation
/// </summary>
internal sealed class ExtensionsBackendTestContext : IDisposable
{
    #region Properties

    public string TempRoot { get; }
    public ExtensionReaderBackend Reader { get; }
    public ExtensionWriterBackend Writer { get; }
    public ExtensionInstallBackend Zip { get; }
    public IJsonService JsonSvc { get; }

    #endregion

    #region Constructor / Factory

    private ExtensionsBackendTestContext(string tempRoot, ExtensionReaderBackend reader, ExtensionWriterBackend writer, ExtensionInstallBackend zip, IJsonService jsonSvc)
    {
        TempRoot = tempRoot;
        Reader = reader;
        Writer = writer;
        Zip = zip;
        JsonSvc = jsonSvc;
    }

    public static ExtensionsBackendTestContext Create(
        LazySvc<IAppReaderFactory> appReadersLazy,
        LazySvc<IJsonService> jsonLazy,
        IJsonService jsonSvc,
        IGlobalConfiguration globalConfiguration,
        ExtensionManifestService manifestService,
        LazySvc<ExtensionInspectBackend> inspectorLazy,
        IDataSourceGenerator<AppEditions> appEditions,
        LazySvc<AppCachePurger> appCachePurgerLazy,
        ExtensionsTestAppJsonConfigurationService appJsonService)
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "2sxc-ext-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        globalConfiguration.GlobalFolder(tempRoot);
        appJsonService.UseAppRoot(tempRoot);

        var site = MockSiteTestHelpers.CreateSite(tempRoot);
        var appPathSvc = new MockAppPathsMicroSvc(tempRoot);
        inspectorLazy.Inject(new ExtensionInspectBackend(appReadersLazy, site, appPathSvc));

        var reader = new ExtensionReaderBackend(appReadersLazy, site, appPathSvc, jsonLazy, manifestService, appEditions);
        var writer = new ExtensionWriterBackend(appReadersLazy, site, appPathSvc);
        var zip = new ExtensionInstallBackend(appReadersLazy, site, appPathSvc, globalConfiguration, manifestService, inspectorLazy, appEditions, appCachePurgerLazy);

        return new ExtensionsBackendTestContext(tempRoot, reader, writer, zip, jsonSvc);
    }

    #endregion
    
    #region Disposal

    public void Dispose()
    {
        try { DeleteDirectory(TempRoot); } catch { /* Ignore */ }
    }

    private static void DeleteDirectory(string directory)
    {
        // Long-path install tests can leave temp trees that Directory.Delete cannot address through
        // legacy Windows paths. Cleanup through \\?\ keeps test isolation reliable after success and
        // after assertion failures.
        var diskDirectory = PathForDiskAccess(directory);
        if (Directory.Exists(diskDirectory))
            Directory.Delete(diskDirectory, recursive: true);
    }

    private static string PathForDiskAccess(string path)
    {
        // Test-only copy of the production long-path fallback. This avoids exposing production helpers
        // just for cleanup while still exercising the same Windows filesystem behavior.
        if (Path.DirectorySeparatorChar != '\\' || path.StartsWith(ExtendedPathPrefix, StringComparison.Ordinal))
            return path;

        var fullPath = Path.GetFullPath(path);
        return fullPath.StartsWith(UncPrefix, StringComparison.Ordinal)
            ? ExtendedUncPathPrefix + fullPath.Substring(UncPrefix.Length)
            : ExtendedPathPrefix + fullPath;
    }

    private const string ExtendedPathPrefix = @"\\?\";
    private const string ExtendedUncPathPrefix = @"\\?\UNC\";
    private const string UncPrefix = @"\\";

    #endregion

}
