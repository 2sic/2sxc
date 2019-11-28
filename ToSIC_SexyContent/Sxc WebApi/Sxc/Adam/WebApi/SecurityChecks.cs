using System;
using System.IO;
using System.Web.Http;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using JetBrains.Annotations;
using ToSic.Eav.Security.Files;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.Sxc.Adam.WebApi
{
    internal class SecurityChecks
    {

        internal static bool DestinationIsInItem(Guid guid, string field, string path, out HttpResponseException preparedException)
        {
            var inAdam = Sxc.Adam.Security.PathIsInItemAdam(guid, field, path);
            preparedException = inAdam
                ? null
                : Http.PermissionDenied("Can't access a resource which is not part of this item.");
            return inAdam;
        }


        /// <summary>
        /// Helper to check extension based on DNN settings
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <remarks>
        /// mostly a copy from https://github.com/dnnsoftware/Dnn.Platform/blob/115ae75da6b152f77ad36312eb76327cdc55edd7/DNN%20Platform/Modules/Journal/FileUploadController.cs#L72
        /// </remarks>
        [AssertionMethod]
        internal static bool IsAllowedDnnExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                   && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLower());
        }




        [AssertionMethod]
        internal static bool IsKnownRiskyExtension(string fileName) 
            => FileNames.IsKnownRiskyExtension(fileName);

        [AssertionMethod]
        internal static void ThrowIfAccessingRootButNotAllowed(bool usePortalRoot, bool userIsRestricted)
        {
            if (usePortalRoot && userIsRestricted)
                throw Http.BadRequest("you may only create draft-data, so file operations outside of ADAM is not allowed");
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
