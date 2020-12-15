using System.IO;
using JetBrains.Annotations;
using ToSic.Sxc.WebApi.Adam;

// #Todo must really finish this - ATM still just for fun / experimental

namespace IntegrationSamples.SxcEdit01.Adam
{
    public class IntAdamSecurityChecks: SecurityChecksBase
    {
        /// <summary>
        /// Helper to check extension based on DNN settings
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [AssertionMethod]
        public override bool SiteAllowsExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                   /*&& Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLowerInvariant())*/;
        }

        public override bool CanEditFolder(ToSic.Eav.Apps.Assets.IAsset item)
        {
            //var folder = FolderManager.Instance.GetFolder(folderId);
            //return folder != null && FolderPermissionController.CanAddFolder(folder as FolderInfo);
            return true;
        }
    }
}
