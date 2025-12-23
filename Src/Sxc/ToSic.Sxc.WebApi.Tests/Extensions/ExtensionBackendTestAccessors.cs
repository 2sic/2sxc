using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.App;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

internal static class ExtensionBackendTestAccessors
{
    public static ExtensionsResultDto GetExtensionsTac(this ExtensionReaderBackend reader, int appId)
        => reader.GetExtensions(appId: appId);

    public static bool SaveExtensionTac(this ExtensionWriterBackend writer, int zoneId, int appId, string name, ExtensionManifest manifest)
        => writer.SaveConfiguration(appId: appId, name: name, manifest: manifest);

    public static bool InstallExtensionZipTac(this ExtensionInstallBackend zip, int zoneId, int appId, Stream zipStream,
        bool overwrite = false, string? originalZipFileName = null, string editions = null!)
        => zip.InstallExtensionZip(zoneId, appId: appId, zipStream: zipStream, overwrite: overwrite, originalZipFileName: originalZipFileName, editions: editions);
}