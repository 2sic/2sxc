using ToSic.Sxc.Backend.App;
using ToSic.Eav.Apps.Sys.FileSystemState;

#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppExtensionsControllerReal(ExtensionsBackend backend,
    LazySvc<ExportExtension> exportExtensionLazy)
    : ServiceBase("Api.ExtsRl", connect: [backend, exportExtensionLazy])
{
    public const string LogSuffix = "ApiExts";

    // List all App Extensions and configuration
    public ExtensionsResultDto Extensions(int appId)
        => backend.GetExtensions(appId);

    // Create or update configuration for a specific extension
    public bool Extension(int zoneId, int appId, string name, ExtensionManifest manifest)
        => backend.SaveExtension(zoneId, appId, name, manifest);

    /// <summary>
    /// Install an extension ZIP into /extensions.
    /// </summary>
    public bool Install(HttpUploadedFile uploadInfo, int zoneId, int appId, bool overwrite = false)
    {
        var l = Log.Fn<bool>($"z:{zoneId}, a:{appId}, overwrite:{overwrite}");

        if (!uploadInfo.HasFiles())
            return l.ReturnFalse("no file uploaded");

        var (fileName, stream) = uploadInfo.GetStream();
        if (stream == null!)
            throw new NullReferenceException("File Stream is null, upload canceled");

        var ok = backend.InstallExtensionZip(zoneId, appId, stream, overwrite, originalZipFileName: fileName);
        return l.ReturnAsOk(ok);
    }

    /// <summary>
    /// Export an extension as a ZIP file
    /// </summary>
    public THttpResponseType Download(int zoneId, int appId, string name)
        => exportExtensionLazy.Value.Export(zoneId, appId, name);

    /// <summary>
    /// Inspect an extension for changes compared to its lock file.
    /// </summary>
    public ExtensionInspectResultDto Inspect(int appId, string name, string? edition = null)
        => backend.InspectExtension(appId, name, edition);

    /// <summary>
    /// Delete an extension and optionally its data.
    /// </summary>
    public bool Delete(int appId, string name, string? edition = null, bool force = false, bool withData = false)
        => backend.DeleteExtension(appId, name, edition, force, withData);
}
