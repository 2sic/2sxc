using System;
using System.IO;
using System.Web.Http;
using DotNetNuke.Entities.Host;
using JetBrains.Annotations;
using ToSic.Eav.Identity;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Adam
{
    internal class SecurityChecks
    {




        internal static bool DestinationIsInItem(Guid guid, string field, string path, out HttpResponseException preparedException)
        {
            var shortGuid = Mapper.GuidCompress(guid);
            // will do check, case-sensitive because the compressed guid is case-sensitive
            if (!path.Replace('\\', '/').Contains(shortGuid + "/" + field))
            {
                preparedException = Http.PermissionDenied("Can't access a resource which is not part of this item.");
                return false;
            }
            preparedException = null;
            return true;
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
        internal static void ThrowIfAccessingRootButNotAllowed(bool usePortalRoot, bool userIsRestricted)
            //IPermissionCheck permCheck)
        {
            //var everywhereOk = permCheck.UserMay(GrantSets.WritePublished);
            if (usePortalRoot && userIsRestricted)// !everywhereOk)
                throw Http.BadRequest("you may only create draft-data, so file operations outside of ADAM is not allowed");
            //return everywhereOk;
        }



    }
}
