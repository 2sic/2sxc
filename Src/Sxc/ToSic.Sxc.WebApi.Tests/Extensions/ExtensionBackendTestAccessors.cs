using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.App;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

internal static class ExtensionBackendTestAccessors
{
    public static ExtensionsResultDto GetExtensionsTac(this ExtensionsBackend backend, int appId)
        => backend.GetExtensions(appId: appId);

    public static bool SaveExtensionTac(this ExtensionsBackend backend, int zoneId, int appId, string name, ExtensionManifest manifest)
        => backend.SaveExtension(zoneId: zoneId, appId: appId, name: name, manifest: manifest);

    public static bool InstallExtensionZipTac(this ExtensionsBackend backend, int zoneId, int appId, Stream zipStream,
        bool overwrite = false, string? originalZipFileName = null, string[]? editions = null)
        => backend.InstallExtensionZip(zoneId: zoneId, appId: appId, zipStream: zipStream, overwrite: overwrite, originalZipFileName: originalZipFileName, editions: editions);
}
