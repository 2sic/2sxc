using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Plumbing;

namespace ToSic.Sxc.Dnn
{
    public class ResponseMakerNetFramework: ResponseMaker<HttpResponseMessage>
    {
        public void Init(System.Web.Http.ApiController apiController) => _apiController = apiController;

        private System.Web.Http.ApiController _apiController;

        private System.Web.Http.ApiController ApiController
            => _apiController ?? throw new Exception(
                $"Accessing the {nameof(ApiController)} in the {nameof(ResponseMakerNetFramework)} requires it to be Init first.");

        public override HttpResponseMessage InternalServerError(string message) 
            => Error((int)HttpStatusCode.InternalServerError, message);

        public override HttpResponseMessage InternalServerError(Exception exception)
            => Error((int)HttpStatusCode.InternalServerError, exception);

        public override HttpResponseMessage Error(int statusCode, string message) 
            => ApiController.Request.CreateErrorResponse((HttpStatusCode)statusCode, message);

        public override HttpResponseMessage Error(int statusCode, Exception exception)
            => ApiController.Request.CreateErrorResponse((HttpStatusCode)statusCode, exception);

        public override HttpResponseMessage Json(object json)
        {
            var responseMessage = ApiController.Request.CreateResponse(HttpStatusCode.OK);
            responseMessage.Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, MimeHelper.Json);
            return responseMessage;
        }

        public override HttpResponseMessage Ok() 
            => ApiController.Request.CreateResponse(HttpStatusCode.OK);

        public override HttpResponseMessage GetAttachmentHttpResponseMessage(string fileName, string fileType, Stream fileContent)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(fileContent) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(fileType);
            response.Content.Headers.ContentLength = fileContent.Length;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return response;
        }

        public override HttpResponseMessage GetAttachmentHttpResponseMessage(string fileName, string fileType, string fileContent)
        {
            var fileBytes = Encoding.UTF8.GetBytes(fileContent);
            return GetAttachmentHttpResponseMessage(fileName, fileType, new MemoryStream(fileBytes));
        }

        public override HttpResponseMessage BuildDownload(string content, string fileName)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            if (fileName.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeHelper.Json);
            else if (fileName.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeHelper.Xml);

            return response;
        }
    }
}