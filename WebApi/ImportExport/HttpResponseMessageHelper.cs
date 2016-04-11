using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpResponseMessageHelper
    {
        public static HttpResponseMessage GetAttachmentHttpResponseMessage(string fileName, string fileType, Stream fileContent)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(fileContent);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(fileType);
            response.Content.Headers.ContentLength = fileContent.Length;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            return response;
        }

        public static HttpResponseMessage GetAttachmentHttpResponseMessage(string fileName, string fileType, string fileContent)
        {
            var fileBytes = Encoding.UTF8.GetBytes(fileContent);
            return GetAttachmentHttpResponseMessage(fileName, fileType, new MemoryStream(fileBytes));
        }

    }
}