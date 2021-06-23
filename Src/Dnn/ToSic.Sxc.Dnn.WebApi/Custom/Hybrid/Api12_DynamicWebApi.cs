using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12: IDynamicWebApi
    {
        /// <inheritdoc />
        public dynamic File(string dontRelyOnParameterOrder = ToSic.Eav.Parameters.Protector,
            // Important: the second parameter should _not_ be a string, otherwise the signature looks the same as the built-in File(...) method
            bool? download = null,
            string virtualPath = null, // important: this is the virtualPath, but it should not have the same name, to not confuse the compiler with same sounding param names
            string contentType = null,
            string fileDownloadName = null,
            object contents = null // can be stream, string or byte[]
            )
        {
            fileDownloadName = CustomApiHelpers.FileParamsInitialCheck(dontRelyOnParameterOrder, download, virtualPath, fileDownloadName, contents);

            // Try to figure out file mime type as needed
            if (string.IsNullOrWhiteSpace(contentType))
                contentType = MimeMapping.GetMimeMapping(fileDownloadName ?? virtualPath);

            HttpContent content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(string.Empty));

            // check if this may just be a call to the built in file, which has two strings
            // this can only be possible if only the virtualPath and contentType were set
            if (!string.IsNullOrWhiteSpace(virtualPath))
                content = new StreamContent(new FileStream(HttpContext.Current.Server.MapPath(virtualPath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            var isValidXml = false;
            switch (contents)
            {
                case string stringBody:
                    content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(stringBody));
                    isValidXml = CustomApiHelpers.IsValidXml(stringBody);
                    break;
                case Stream streamBody:
                    content = new StreamContent(streamBody);
                    isValidXml = CustomApiHelpers.IsValidXml(streamBody);
                    break;
                case byte[] charBody:
                    content = new ByteArrayContent(charBody);
                    isValidXml = CustomApiHelpers.IsValidXml(charBody);
                    break;
            }
            contentType = CustomApiHelpers.XmlContentTypeFromContent(isValidXml, contentType);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = content;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            // TODO: STV - make sure this is the same in Oqtane
            if (download == false)
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            else
            {
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileDownloadName;
            }

            return response;
        }


    }
}
