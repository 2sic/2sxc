using System.IO;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using ToSic.Sxc.Adam.Internal;
using IAsset = ToSic.Eav.Apps.Assets.IAsset;
using IFile = ToSic.Eav.Apps.Assets.IFile;
using IFolder = ToSic.Eav.Apps.Assets.IFolder;

namespace ToSic.Sxc.Dnn.Backend;

internal class DnnAdamSecurityChecks(AdamSecurityChecksBase.MyServices services)
    : AdamSecurityChecksBase(services, DnnConstants.LogName)
{
    /// <summary>
    /// Helper to check extension based on DNN settings
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    /// <remarks>
    /// mostly a copy from https://github.com/dnnsoftware/Dnn.Platform/blob/115ae75da6b152f77ad36312eb76327cdc55edd7/DNN%20Platform/Modules/Journal/FileUploadController.cs#L72
    /// </remarks>
    public override bool SiteAllowsExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(extension)
               && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLowerInvariant());
    }

    public override bool CanEditFolder(IAsset item)
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