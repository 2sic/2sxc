using System;
using System.IO;
using JetBrains.Annotations;
using ToSic.Eav.Apps.Assets;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Oqt.Server.Controllers.Adam
{
    public class OqtAdamSecurityChecks: SecurityChecksBase
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
        internal override bool SiteAllowsExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                ;
            //&& Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLower());
        }

        internal override bool CanEditFolder(IAsset item)
        {
            var id = (item as IFolder)?.Id
                     ?? (item as IFile)?.ParentId
                     ?? throw new ArgumentException("Should be a DNN asset", nameof(item));

            WipConstants.DontDoAnythingImplementLater();
            return true;
            //var folder = FolderManager.Instance.GetFolder(id);
            //return folder != null && FolderPermissionController.CanAddFolder(folder as FolderInfo);
        }
    }
}
