using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
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
            responseMessage.Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public override HttpResponseMessage Ok() 
            => ApiController.Request.CreateResponse(HttpStatusCode.OK);
    }
}