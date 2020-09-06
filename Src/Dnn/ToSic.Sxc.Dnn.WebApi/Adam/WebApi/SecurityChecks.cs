using System.IO;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using JetBrains.Annotations;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Adam.WebApi
{
    public class DnnAdamSecurityChecks: SecurityChecksBase
    {
        /// <summary>
        /// Helper to check extension based on DNN settings
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <remarks>
        /// mostly a copy from https://github.com/dnnsoftware/Dnn.Platform/blob/115ae75da6b152f77ad36312eb76327cdc55edd7/DNN%20Platform/Modules/Journal/FileUploadController.cs#L72
        /// </remarks>
        [AssertionMethod]
        internal override bool TenantAllowsExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                   && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLower());
        }

        internal override bool CanEditFolder(int folderId)
        {
            var dnnFolder = FolderManager.Instance.GetFolder(folderId);
            return CanEdit(dnnFolder);
        }

        internal static bool CanEdit(IFileInfo file)
        {
            if (file == null) return false;
            var folder = (FolderInfo)FolderManager.Instance.GetFolder(file.FolderId);
            return CanEdit(folder);
        }

        internal static bool CanEdit(IFolderInfo folder) 
            => folder != null && FolderPermissionController.CanAddFolder(folder as FolderInfo);
    }
}
