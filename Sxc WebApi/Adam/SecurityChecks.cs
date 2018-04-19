using System;
using System.Collections.Generic;
using System.IO;
using DotNetNuke.Entities.Host;
using JetBrains.Annotations;
using ToSic.Eav.Identity;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Adam
{
    internal class SecurityChecks
    {




        [AssertionMethod]
        internal static void ThrowIfDestNotInItem(Guid guid, string field, string path)
        {
            var shortGuid = Mapper.GuidCompress(guid);
            var expectedPathPart = shortGuid + "\\" + field;
            if (path.IndexOf(expectedPathPart, StringComparison.Ordinal) == -1)
                throw new AccessViolationException(
                    "Trying to access a file/folder in a path which is not part of this item - access denied.");
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


        /// <summary>
        /// This will check if the field-definition grants additional rights
        /// Should only be called if the user doesn't have full edit-rights
        /// </summary>

        [AssertionMethod]
        internal static void CheckFieldPermissions(IAttributeDefinition fieldDef, List<Grants> requiredGrant, SxcInstance sxcInstance, Log log)
        {
            var fieldPermissions = new DnnPermissionCheck(log,
                instance: sxcInstance.EnvInstance,
                meta1: fieldDef.Metadata);

            if (!fieldPermissions.UserMay(requiredGrant))
                throw Http.PermissionDenied("this field is not configured to allow uploads by the current user");
        }
    }
}
