using System.IO;
using JetBrains.Annotations;
using ToSic.Sxc.WebApi.Adam;

// #Todo must really finish this - ATM still just for fun / experimental

namespace ToSic.Sxc.Mvc.WebApi.Adam
{
    public class MvcAdamSecurityChecks: SecurityChecksBase
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
                   /*&& Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLowerInvariant())*/;
        }

        internal override bool CanEditFolder(Eav.Apps.Assets.IAsset item)
        {
            //var folder = FolderManager.Instance.GetFolder(folderId);
            //return folder != null && FolderPermissionController.CanAddFolder(folder as FolderInfo);
            return true;
        }
    }
}
