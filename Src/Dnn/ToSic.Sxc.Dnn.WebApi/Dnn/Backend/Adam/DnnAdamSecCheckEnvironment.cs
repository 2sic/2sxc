using DotNetNuke.Abstractions.Application;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using ToSic.Sxc.Adam.Sys.Security;
using IAsset = ToSic.Eav.Apps.Assets.IAsset;
using IFile = ToSic.Eav.Apps.Assets.IFile;
using IFolder = ToSic.Eav.Apps.Assets.IFolder;

namespace ToSic.Sxc.Dnn.Backend;

internal class DnnAdamSecCheckEnvironment(IHostSettings hostSettings)
    : ServiceBase("Dnn.AdmSCh"), IAdamSecurityCheckEnvironment
{
    /// <summary>
    /// Helper to check extension based on DNN settings
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// mostly a copy from https://github.com/dnnsoftware/Dnn.Platform/blob/115ae75da6b152f77ad36312eb76327cdc55edd7/DNN%20Platform/Modules/Journal/FileUploadController.cs#L72
    /// </remarks>
    public bool SiteAllowsExtension(IAdamSecurityCheckService security, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(extension)
               && hostSettings.AllowedExtensionAllowList.IsAllowedExtension(extension.ToLowerInvariant());
    }

    public bool CanEditFolder(IAdamSecurityCheckService security, IAsset item)
    {
        var id = (item as IFolder)?.Id
                 ?? (item as IFile)?.ParentId
                 ?? throw new ArgumentException("Should be a DNN asset", nameof(item));

        if (FolderManager.Instance.GetFolder(id) is not FolderInfo folderInfo)
            return false;

        return FolderPermissionController.CanAddFolder(folderInfo) // ex: "WRITE" core folder permission
            || FolderPermissionController.CanAdminFolder(folderInfo) // ex: "WRITE" core folder permission
            || FolderPermissionController.CanManageFolder(folderInfo) // ex: "WRITE" core folder permission
            || FolderPermissionController.CanDeleteFolder(folderInfo) // ex: "WRITE" core folder permission
            || FolderPermissionController.CanCopyFolder(folderInfo); // ex: "WRITE" core folder permission
    }
}
