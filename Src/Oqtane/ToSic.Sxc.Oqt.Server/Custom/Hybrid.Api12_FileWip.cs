using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12
    {
        #region Experimental

        public object Test()
        {
            return "test test test";
            // return File("filename",)
        }

        public object File(string dontRelyOnParameterOrder = ToSic.Eav.Constants.RandomProtectionParameter,
            // Important: the second parameter should _not_ be a string, otherwise the signature looks the same as the built-in File(...) method
            bool? download = null,
            string virtualPath = null, // important: this is the virtualPath, but it should not have the same name, to not confuse the compiler with same sounding param names
            string contentType = null,
            string fileDownloadName = null,
            Stream stream = null,
            string body = null
            )
        {
            ToSic.Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, nameof(File), "");

            // Set reallyForceDownload based on forceDownload and file name
            var reallyForceDownload = download == true || !string.IsNullOrWhiteSpace(fileDownloadName);

            // check if we should force a download, but maybe fileDownloadName is empty
            if (reallyForceDownload && string.IsNullOrWhiteSpace(fileDownloadName))
            {
                // try to guess name based on virtualPath name
                fileDownloadName = !string.IsNullOrWhiteSpace(virtualPath) ? Path.GetFileName(virtualPath) : null;
                if (string.IsNullOrWhiteSpace(fileDownloadName))
                    throw new ArgumentException($"Can't force download without a {nameof(fileDownloadName)} or a real {nameof(virtualPath)}");
            }

            // check if this may just be a call to the built in file, which has two strings
            // this can only be possible if only the virtualPath and contentType were set
            if (!string.IsNullOrWhiteSpace(virtualPath) && !string.IsNullOrWhiteSpace(contentType))
                return string.IsNullOrWhiteSpace(fileDownloadName)
                    ? base.File(virtualPath, contentType)
                    : base.File(virtualPath, contentType, fileDownloadName);

            return null;
        }

        #endregion

    }
}
