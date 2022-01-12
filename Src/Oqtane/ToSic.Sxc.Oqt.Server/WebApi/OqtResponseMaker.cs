using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using ToSic.Sxc.WebApi.Plumbing;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtResponseMaker: ResponseMaker
    {
        public void Init(Controller apiController) => _apiController = apiController;

        private Controller _apiController;

        public Controller ApiController => _apiController ??
                                           throw new Exception(
                                               $"Accessing the {nameof(ApiController)} in the {nameof(OqtResponseMaker)} requires it to be Init first.");

        public override IActionResult InternalServerError(string message) 
            => Error((int)HttpStatusCode.InternalServerError, message);

        public override IActionResult InternalServerError(Exception exception)
            => Error((int)HttpStatusCode.InternalServerError, exception);

        public override IActionResult Error(int statusCode, string message)
            => ApiController.Problem(message, null, statusCode); 

        public override IActionResult Error(int statusCode, Exception exception)
            => ApiController.Problem(exception.Message, null, statusCode);

        public override IActionResult Json(object json)
        {
            return ApiController.Json(json);
        }
    }
}
