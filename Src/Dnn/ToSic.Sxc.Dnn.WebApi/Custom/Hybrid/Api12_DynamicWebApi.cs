using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using ToSic.Eav;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.WebApi;
using System.Web;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12: IDynamicWebApi
    {
        /// <inheritdoc />
        public dynamic File(string dontRelyOnParameterOrder = ToSic.Eav.Constants.RandomProtectionParameter,
            // Important: the second parameter should _not_ be a string, otherwise the signature looks the same as the built-in File(...) method
            bool? download = null,
            string virtualPath = null, // important: this is the virtualPath, but it should not have the same name, to not confuse the compiler with same sounding param names
            string contentType = null,
            string fileDownloadName = null,
            object contents = null // can be stream, string or byte[]
            )
        {
            ToSic.Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, nameof(File), "");

            // Check initial conflicting values
            var contentCount = (contents != null ? 1 : 0) + (virtualPath != null ? 1 : 0);
            if (contentCount == 0)
                throw new ArgumentException("None of the provided parameters give content for the file to return.");
            if (contentCount > 1)
                throw new ArgumentException($"Multiple file setting properties like '{nameof(contents)}' or '{nameof(virtualPath)}' have a value - only one can be provided.");
            
            // Set reallyForceDownload based on forceDownload and file name
            var reallyForceDownload = download == true || !string.IsNullOrWhiteSpace(fileDownloadName);

            // check if we should force a download, but maybe fileDownloadName is empty
            if (reallyForceDownload && string.IsNullOrWhiteSpace(fileDownloadName))
            {
                // try to guess name based on virtualPath name
                fileDownloadName = !string.IsNullOrWhiteSpace(virtualPath) ? Path.GetFileName(virtualPath) : null;
                if (string.IsNullOrWhiteSpace(fileDownloadName))
                    throw new HttpExceptionAbstraction(HttpStatusCode.NotFound, $"Can't force download without a {nameof(fileDownloadName)} or a real {nameof(virtualPath)}", "Not Found");
            }

            // Try to figure out file mime type as needed
            if (string.IsNullOrWhiteSpace(contentType))
                contentType = MimeMapping.GetMimeMapping(fileDownloadName ?? virtualPath);

            HttpContent content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(string.Empty));

            // check if this may just be a call to the built in file, which has two strings
            // this can only be possible if only the virtualPath and contentType were set
            if (!string.IsNullOrWhiteSpace(virtualPath))
                content = new StreamContent(new FileStream(HttpContext.Current.Server.MapPath(virtualPath), FileMode.Open));

            if (contents is Stream streamBody)
                content = new StreamContent(streamBody);
            
            if(contents is string stringBody) 
                content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(stringBody));

            if (contents is byte[] charBody)
                content = new ByteArrayContent(charBody);

            //var stream = new MemoryStream().
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = content;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileDownloadName;

            return response;
        }
    }
}
