using System.IO;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using JetBrains.Annotations;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Dnn.WebApi
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
            var folder = FolderManager.Instance.GetFolder(folderId);
            return folder != null && FolderPermissionController.CanAddFolder(folder as FolderInfo);
        }
    }
}
