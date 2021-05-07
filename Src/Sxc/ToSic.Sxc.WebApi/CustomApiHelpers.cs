using System;
using System.IO;
using System.Net;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi
{
    public class CustomApiHelpers
    {
        public static string FileParamsInitialCheck(string dontRelyOnParameterOrder, bool? download, string virtualPath,
            string fileDownloadName, object contents)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, nameof(File), "");

            // Check initial conflicting values
            CheckInitialConflictingValues(virtualPath, contents);

            return CheckForceDownload(download, virtualPath, fileDownloadName);
        }

        /**
         * check initial conflicting value
         */
        public static void CheckInitialConflictingValues(string virtualPath, object contents)
        {
            var contentCount = (contents != null ? 1 : 0) + (virtualPath != null ? 1 : 0);
            if (contentCount == 0)
                throw new ArgumentException("None of the provided parameters give content for the file to return.");
            if (contentCount > 1)
                throw new ArgumentException(
                    $"Multiple file setting properties like '{nameof(contents)}' or '{nameof(virtualPath)}' have a value - only one can be provided.");
        }

        public static string CheckForceDownload(bool? download, string virtualPath, string fileDownloadName)
        {
            // Set reallyForceDownload based on forceDownload and file name
            var reallyForceDownload = download == true || !string.IsNullOrWhiteSpace(fileDownloadName);

            // check if we should force a download, but maybe fileDownloadName is empty
            if (reallyForceDownload && string.IsNullOrWhiteSpace(fileDownloadName))
            {
                // try to guess name based on virtualPath name
                fileDownloadName = !string.IsNullOrWhiteSpace(virtualPath) ? Path.GetFileName(virtualPath) : null;
                if (string.IsNullOrWhiteSpace(fileDownloadName))
                    throw new HttpExceptionAbstraction(HttpStatusCode.NotFound,
                        $"Can't force download without a {nameof(fileDownloadName)} or a real {nameof(virtualPath)}",
                        "Not Found");
            }

            return fileDownloadName;
        }
    }
}
