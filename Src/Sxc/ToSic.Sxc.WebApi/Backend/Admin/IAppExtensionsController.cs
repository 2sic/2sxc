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
    /// Preflight installation of an extension zip to report current state and options.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="editions">Optional comma-delimited list of editions to install into (empty or null = root).</param>
    PreflightResultDto InstallPreflight(int appId, string editions = "");

    /// <summary>
    /// Install app extension zip.
    /// </summary>
    /// <param name="zoneId">Zone identifier</param>
    /// <param name="appId">App identifier</param>
    /// <param name="editions">Optional comma-delimited list of editions to install into (empty or null = root).</param>
    /// <param name="overwrite">Overwrite existing files if true</param>
    /// <returns>true if installation succeeded</returns>
    bool Install(int zoneId, int appId, string editions = "", bool overwrite = false);

    /// <summary>
    /// Preflight installation of an extension zip downloaded from a URL.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="urls">Remote URLs to extension packages (first is used)</param>
    /// <param name="editions">Optional comma-delimited list of editions to install into (empty or null = root).</param>
    PreflightResultDto InstallPreflightFrom(int appId, string[] urls, string editions = "");

    /// <summary>
    /// Install app extension zip downloaded from a URL.
    /// </summary>
    /// <param name="zoneId">Zone identifier</param>
    /// <param name="appId">App identifier</param>
    /// <param name="urls">Remote URLs to extension packages (first is used)</param>
    /// <param name="editions">Optional comma-delimited list of editions to install into (empty or null = root).</param>
    /// <param name="overwrite">Overwrite existing files if true</param>
    /// <returns>true if installation succeeded</returns>
    bool InstallFrom(int zoneId, int appId, string[] urls, string editions = "", bool overwrite = false);

    /// <summary>
    /// Inspect endpoint.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <param name="edition">Optional edition name</param>
    ExtensionInspectResultDto Inspect(int appId, string name, string? edition = null);

    /// <summary>
    /// Create or replace the configuration of a specific App Extension.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name under "/extensions"</param>
    /// <param name="configuration">JSON to write as App_Data/extension.json</param>
    /// <returns>true if saved</returns>
    bool Configuration(int appId, string name, ExtensionManifest configuration);

    /// <summary>
    /// Download (export) a specific extension as a ZIP file.
    /// </summary>
    /// <param name="appId">App identifier</param>
    /// <param name="name">Extension folder name</param>
    /// <returns>HTTP response containing the file data.</returns>
    THttpResponse Download(int appId, string name);

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
