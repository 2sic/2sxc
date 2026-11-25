using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using ToSic.Eav.Apps.Assets;
using ToSic.Sxc.Adam.Sys.Security;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam;

/// <summary>
/// Oqtane specific parts of the ADAM security checks.
/// </summary>
internal class OqtAdamSecCheckEnvironment(
    ISettingRepository settingRepository,
    IFolderRepository folderRepository,
    IUserPermissions userPermissions,
    IHttpContextAccessor httpContextAccessor)
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
    /// Check native Edit permissions on the folder, or on the containing folder for a file.
    /// </summary>
    public bool CanEditFolder(IAdamSecurityCheckService security, IAsset item)
    {
        var folderId = item switch
        {
            IFolder folder => folder.Id,
            IFile file => file.ParentId,
            _ => 0
        };
        var httpContext = httpContextAccessor.HttpContext;
        if (folderId <= 0 || httpContext == null)
            return false;

        var nativeFolder = folderRepository.GetFolder(folderId);
        return nativeFolder != null
               && nativeFolder.SiteId == security.AdamContext.Context.Site.Id
               && userPermissions.IsAuthorized(httpContext.User, PermissionNames.Edit, nativeFolder.PermissionList);
    }
}
