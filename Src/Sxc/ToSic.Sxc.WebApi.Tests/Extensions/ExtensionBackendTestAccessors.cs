using System.Text.Json;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Backend.App;

namespace Tests.ToSic.ToSxc.WebApi.Extensions;
internal static class ExtensionBackendTestAccessors
{
    public static ExtensionsResultDto GetExtensionsTac(this ExtensionsBackend backend, int appId)
        => backend.GetExtensions(appId: appId);

    public static bool SaveExtensionTac(this ExtensionsBackend backend, int zoneId, int appId, string name, JsonElement configuration)
        => backend.SaveExtension(zoneId: zoneId, appId: appId, name: name, configuration: configuration);

    public static bool InstallExtensionZipTac(this ExtensionsBackend backend, int zoneId, int appId, Stream zipStream, string? name = null, bool overwrite = false, string? originalZipFileName = null)
        => backend.InstallExtensionZip(zoneId: zoneId, appId: appId, zipStream: zipStream, name: name, overwrite: overwrite, originalZipFileName: originalZipFileName);
}
