using ToSic.Sxc.Backend.App;
using ToSic.Eav.Apps.Sys.FileSystemState;


namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppExtensionsControllerReal(
    LazySvc<ExtensionReaderBackend> readerLazy,
    LazySvc<ExtensionWriterBackend> writerLazy,
    LazySvc<ExtensionInstallBackend> zipLazy,
    LazySvc<ExtensionInspectBackend> inspectorLazy,
    LazySvc<ExtensionDeleteBackend> deleteLazy,
    LazySvc<ExtensionExportService> exportExtensionLazy)
    : ServiceBase("Api.ExtsRl", connect: [readerLazy, writerLazy, zipLazy, inspectorLazy, deleteLazy, exportExtensionLazy])
{
    public const string LogSuffix = "ApiExts";

    /// <summary>
    /// Get all App Extensions and their configuration (if any).
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <returns>Object with property "extensions" containing an array of extensions</returns>
    public ExtensionsResultDto Extensions(int appId)
        => readerLazy.Value.GetExtensions(appId);

    /// <summary>
    /// Preflight install of an extension zip to report current state and options.
    /// </summary>
    /// <param name="uploadInfo">Uploaded ZIP file containing the extension package</param>
    /// <param name="appId">App identifier</param>
    /// <param name="editions">Optional list of editions to install into (empty or null = root)</param>
    /// <returns>Preflight result describing detected state and installation options</returns>
    public PreflightResultDto InstallPreflight(HttpUploadedFile uploadInfo, int appId, string[]? editions = null)
    {
        var l = Log.Fn<PreflightResultDto>($"a:{appId}, editions:'{string.Join(",", editions ?? [])}'");

        if (!uploadInfo.HasFiles())
            throw l.Ex(new ArgumentException("no file uploaded", nameof(uploadInfo)));

        var (fileName, stream) = uploadInfo.GetStream();
        if (stream == null!)
            throw l.Ex(new NullReferenceException("File Stream is null, upload canceled"));

        var result = zipLazy.Value.InstallPreflight(appId, stream, originalZipFileName: fileName, editions: editions);
        return l.Return(result, "ok");
    }

    /// <summary>
    /// Install app extension zip.
    /// </summary>
    /// <param name="uploadInfo">Uploaded ZIP file containing the extension package</param>
    /// <param name="appId">App identifier</param>
    /// <param name="editions">Optional list of editions to install into (empty or null = root)</param>
    /// <param name="overwrite">Overwrite existing files if true</param>
    /// <returns>true if installation succeeded</returns>
    public bool Install(HttpUploadedFile uploadInfo, int appId, string[]? editions = null, bool overwrite = false)
    {
        var l = Log.Fn<bool>($"a:{appId}, editions:'{string.Join(",", editions ?? [])}', overwrite:{overwrite}");

        if (!uploadInfo.HasFiles())
            throw l.Ex(new ArgumentException("no file uploaded", nameof(uploadInfo)));

        var (fileName, stream) = uploadInfo.GetStream();
        if (stream == null!)
            throw l.Ex(new NullReferenceException("File Stream is null, upload canceled"));

        var ok = zipLazy.Value.InstallExtensionZip(appId, stream, overwrite, originalZipFileName: fileName, editions: editions);
        return l.ReturnAsOk(ok);
    }

    /// <summary>
    /// Inspect endpoint mirroring DNN behavior.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <param name="edition">Optional edition name</param>
    public ExtensionInspectResultDto Inspect(int appId, string name, string? edition = null)
        => inspectorLazy.Value.Inspect(appId, name, edition);

    /// <summary>
    /// Create or replace the configuration of a specific App Extension.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name under "/extensions"</param>
    /// <param name="manifest">JSON to write as App_Data/extension.json</param>
    /// <returns>true if saved</returns>
    public bool Configuration(int appId, string name, ExtensionManifest manifest)
        => writerLazy.Value.SaveConfiguration(appId, name, manifest);

    /// <summary>
    /// Download (export) a specific extension as a ZIP file.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <returns>HTTP response containing the file data.</returns>
    public THttpResponseType Download(int appId, string name)
        => exportExtensionLazy.Value.Export(appId, name);
    
    /// <summary>
    /// Delete an extension and optionally its data.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <param name="edition">Optional edition name</param>
    /// <param name="force">Force deletion even when files or data changed.</param>
    /// <param name="withData">Delete related data when true (requires force).</param>
    /// <returns>true if deleted</returns>
    public bool Delete(int appId, string name, string? edition = null, bool force = false, bool withData = false)
        => deleteLazy.Value.DeleteExtension(appId, name, edition, force, withData);
}
