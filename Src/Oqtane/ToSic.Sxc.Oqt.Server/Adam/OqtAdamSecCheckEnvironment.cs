using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps.Assets;
using ToSic.Sxc.Adam.Sys.Security;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam;

/// <summary>
/// Oqtane specific parts of the ADAM security checks.
/// </summary>
internal class OqtAdamSecCheckEnvironment(ISettingRepository settingRepository)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.AdmSCh"), IAdamSecurityCheckEnvironment
{
    /// <summary>
    /// Name of the Oqtane site setting which lists the extensions a site accepts for uploads.
    /// </summary>
    internal const string UploadableFilesSettingName = "UploadableFiles";

    /// <summary>
    /// Check the extension against the "Uploadable Files" of the site, as Oqtane does for its own uploads.
    /// </summary>
    /// <remarks>
    /// Same rule as HasValidFileExtension in the Oqtane.Server FileController.
    /// </remarks>
    public bool SiteAllowsExtension(IAdamSecurityCheckService security, string fileName)
    {
        var siteId = security.AdamContext.Context.Site.Id;
        var uploadableFiles = settingRepository.GetSetting(EntityNames.Site, siteId, UploadableFilesSettingName)?.SettingValue;
        return IsUploadableExtension(uploadableFiles, fileName);
    }

    /// <summary>
    /// Check if the extension of the file is in the comma separated list of uploadable extensions.
    /// An empty list means the site has no own setting, so the Oqtane default applies.
    /// </summary>
    internal static bool IsUploadableExtension(string uploadableFiles, string fileName)
    {
        var extension = Path.GetExtension(fileName)?.TrimStart('.');
        if (string.IsNullOrEmpty(extension))
            return false;

        var allowed = string.IsNullOrEmpty(uploadableFiles)
            ? Constants.UploadableFiles
            : uploadableFiles;

        return allowed
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Same rule as the platform-neutral default, which is internal to ToSic.Sxc.Adam.
    /// </summary>
    public bool CanEditFolder(IAdamSecurityCheckService security, IAsset item)
    {
        var permissions = security.AdamContext.Context.Permissions;
        return permissions.IsSiteAdmin
               || permissions.IsContentAdmin
               || permissions.IsContentEditor;
    }
}
