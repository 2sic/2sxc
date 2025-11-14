using System.Text.Json;
using ToSic.Sxc.Backend.App;
using Services_ServiceBase = ToSic.Sys.Services.ServiceBase;

#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppExtensionsControllerReal(
    LazySvc<ExtensionsBackend> extensionsBackendLazy,
    LazySvc<ExportExtension> exportExtensionLazy)
    : Services_ServiceBase($"Api.{LogSuffix}Rl", connect: [extensionsBackendLazy, exportExtensionLazy])
{
    public const string LogSuffix = "AppExt";

    // List all App Extensions and configuration
    public ExtensionsResultDto Extensions(int appId)
        => extensionsBackendLazy.Value.GetExtensions(appId);

    // Create or update configuration for a specific extension
    public bool Extension(int zoneId, int appId, string name, JsonElement configuration)
        => extensionsBackendLazy.Value.SaveExtension(zoneId, appId, name, configuration);

    /// <summary>
    /// Install an extension ZIP into /extensions.
    /// </summary>
    public bool InstallExtensionZip(HttpUploadedFile uploadInfo, int zoneId, int appId, bool overwrite = false)
    {
        var l = Log.Fn<bool>($"z:{zoneId}, a:{appId}, overwrite:{overwrite}");

        if (!uploadInfo.HasFiles())
            return l.ReturnFalse("no file uploaded");

        var (fileName, stream) = uploadInfo.GetStream(0);
        if (stream == null!)
            throw new NullReferenceException("File Stream is null, upload canceled");

        var ok = extensionsBackendLazy.Value.InstallExtensionZip(zoneId, appId, stream, overwrite, originalZipFileName: fileName);
        return l.ReturnAsOk(ok);
    }

    /// <summary>
    /// Export an extension as a ZIP file
    /// </summary>
    public THttpResponseType Download(int zoneId, int appId, string name)
        => exportExtensionLazy.Value.Export(zoneId, appId, name);
}
