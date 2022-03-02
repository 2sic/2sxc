using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Text;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Plumbing;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public class OqtResponseMaker: ResponseMaker<IActionResult>
    {
        public void Init(Controller apiController) => _apiController = apiController;

        private Controller _apiController;

        public Controller ApiController => _apiController ??
                                           throw new Exception(
                                               $"Accessing the {nameof(ApiController)} in the {nameof(OqtResponseMaker)} requires it to be Init first.");

        //public override IActionResult InternalServerError(string message) 
        //    => Error((int)HttpStatusCode.InternalServerError, message);

        //public override IActionResult InternalServerError(Exception exception)
        //    => Error((int)HttpStatusCode.InternalServerError, exception);

        public override IActionResult Error(int statusCode, string message)
            => ApiController.Problem(message, null, statusCode); 

        public override IActionResult Error(int statusCode, Exception exception)
            => ApiController.Problem(exception.Message, null, statusCode);

        public override IActionResult Json(object json) => ApiController.Json(json);

        public override IActionResult Ok() => ApiController.Ok();

        public override IActionResult GetAttachmentHttpResponseMessage(string fileName, string fileType, Stream fileContent)
        {
            using var memoryStream = new MemoryStream();
            fileContent.CopyTo(memoryStream);
            return new FileContentResult(memoryStream.ToArray(), fileType) { FileDownloadName = fileName };
        }

        //public override IActionResult GetAttachmentHttpResponseMessage(string fileName, string fileType, string fileContent)
        //{
        //    var fileBytes = Encoding.UTF8.GetBytes(fileContent);
        //    return GetAttachmentHttpResponseMessage(fileName, fileType, new MemoryStream(fileBytes));
        //}

        public override IActionResult BuildDownload(string content, string fileName)
        {
            var fileBytes = Encoding.UTF8.GetBytes(content);
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
            return new FileContentResult(fileBytes, contentType ?? MimeHelper.FallbackType) { FileDownloadName = fileName };
        }

    }
}
