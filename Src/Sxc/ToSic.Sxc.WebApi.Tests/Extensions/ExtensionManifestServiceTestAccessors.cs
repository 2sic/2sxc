using ToSic.Eav.Apps.Sys.FileSystemState;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test accessor methods for ExtensionManifestService.
/// Provides logic-free pass-through methods to keep production API usage analysis clean.
/// </summary>
internal static class ExtensionManifestServiceTestAccessors
{
    /// <summary>
    /// Test accessor for LoadManifest method.
    /// </summary>
    public static ExtensionManifest? LoadManifestTac(this ExtensionManifestService service, FileInfo manifestFile)
        => service.LoadManifest(manifestFile: manifestFile);

    /// <summary>
    /// Test accessor for GetManifestFile method.
    /// </summary>
    public static FileInfo GetManifestFileTac(this ExtensionManifestService service, DirectoryInfo extensionFolder)
        => service.GetManifestFile(extensionFolder: extensionFolder);
}
