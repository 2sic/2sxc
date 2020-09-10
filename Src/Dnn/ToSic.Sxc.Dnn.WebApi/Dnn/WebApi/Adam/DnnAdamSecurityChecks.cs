using System;
using System.IO;
using DotNetNuke.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using JetBrains.Annotations;
using ToSic.Eav.Apps.Assets;
using ToSic.Sxc.WebApi.Adam;
using IAsset = ToSic.Sxc.Adam.IAsset;

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

        internal override bool CanEditFolder(Eav.Apps.Assets.IAsset item)
        {
            var id = (item as IFolder)?.Id
                     ?? (item as IFile)?.ParentId
                     ?? throw new ArgumentException("Should be a DNN asset", nameof(item));
            
            var folder = FolderManager.Instance.GetFolder(id /*folderId*/);
            return folder != null && FolderPermissionController.CanAddFolder(folder as FolderInfo);
        }
    }
}
