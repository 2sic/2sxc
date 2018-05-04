using System;
using System.IO;
using DotNetNuke.Entities.Host;
using JetBrains.Annotations;
using ToSic.Eav.Identity;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Adam
{
    internal class SecurityChecks
    {




        [AssertionMethod]
        internal static void ThrowIfDestNotInItem(Guid guid, string field, string path)
        {
            var shortGuid = Mapper.GuidCompress(guid);
            // will do check, case-sensitive because the compressed guid is case-sensitive
            if (!path.Replace('\\', '/').Contains(shortGuid + "/" + field)) 
                throw Http.PermissionDenied("Can't access a resource which is not part of this item.");
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
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                   && Sxc.Adam.Security.BadExtensions.IsMatch(extension);
        }

        [AssertionMethod]
        internal static bool ThrowIfUserMayNotWriteEverywhere(bool usePortalRoot,
            PermissionCheckBase permCheck)
        {
            var everywhereOk = permCheck.UserMay(GrantSets.WritePublished);
            if (usePortalRoot && !everywhereOk)
                throw Http.BadRequest("you may only create draft-data, so file operations outside of ADAM is not allowed");
            return everywhereOk;
        }



    }
}
