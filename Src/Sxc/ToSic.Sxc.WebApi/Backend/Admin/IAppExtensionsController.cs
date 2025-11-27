using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Sxc.Backend.App;

namespace ToSic.Sxc.Backend.Admin;

/// <summary>
/// Controller interface for App Extensions endpoints separated from the main App controller.
/// </summary>
public interface IAppExtensionsController<out THttpResponse>
{
    /// <summary>
    /// Get all App Extensions and their configuration (if any).
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <returns>Object with property "extensions" containing an array of extensions</returns>
    ExtensionsResultDto Extensions(int appId);

    /// <summary>
    /// Create or replace the configuration of a specific App Extension.
    /// </summary>
    /// <param name="zoneId">Zone id (for permission/consistency)</param>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name under "/extensions"</param>
    /// <param name="configuration">JSON to write as App_Data/extension.json</param>
    /// <returns>true if saved</returns>
    bool Extension(int zoneId, int appId, string name, ExtensionManifest configuration);

    /// <summary>
    /// Install app extension zip.
    /// </summary>
    /// <param name="zoneId">Zone id (for permission/consistency)</param>
    /// <param name="appId">App identifier</param>
    /// <param name="overwrite">Overwrite existing files if true</param>
    /// <returns>true if installation succeeded</returns>
    bool Install(int zoneId, int appId, bool overwrite = false);

    /// <summary>
    /// Download (export) a specific extension as a ZIP file.
    /// </summary>
    /// <param name="zoneId">Zone id (for permission/consistency)</param>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <returns>HTTP response containing the file data.</returns>
    THttpResponse Download(int zoneId, int appId, string name);

    /// <summary>
    /// Inspect endpoint mirroring DNN behavior.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <param name="edition">Optional edition name</param>
    public ExtensionInspectResultDto Inspect(int appId, string name, string? edition = null);

    /// <summary>
    /// Delete an extension and optionally its data.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <param name="edition">Optional edition name</param>
    /// <param name="force">Force deletion even when files or data changed.</param>
    /// <param name="withData">Delete related data when true (requires force).</param>
    /// <returns>true if deleted</returns>
    bool Delete(int appId, string name, string? edition = null, bool force = false, bool withData = false);
}
