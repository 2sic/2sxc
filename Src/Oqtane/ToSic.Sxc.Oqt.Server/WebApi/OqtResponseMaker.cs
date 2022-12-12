using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Plumbing;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public class OqtResponseMaker: ResponseMaker<IActionResult>
    {
        public void Init(Controller apiController) => _apiController = apiController;

        private Controller _apiController;

        public Controller ApiController => _apiController ??
                                           throw new(
                                               $"Accessing the {nameof(ApiController)} in the {nameof(OqtResponseMaker)} requires it to be Init first.");

        public override IActionResult Error(int statusCode, string message)
            => ApiController.Problem(message, null, statusCode); 

        public override IActionResult Error(int statusCode, Exception exception)
            => ApiController.Problem(exception.Message, null, statusCode);

        public override IActionResult Json(object json) => ApiController.Json(json);

        public override IActionResult Ok() => ApiController.Ok();

        public override IActionResult File(Stream fileContent, string fileName, string fileType)
        {
            using var memoryStream = new MemoryStream();
            fileContent.CopyTo(memoryStream);
            return new FileContentResult(memoryStream.ToArray(), fileType) { FileDownloadName = fileName };
        }

        public override IActionResult File(string fileContent, string fileName)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
            return File(fileContent, fileName, contentType ?? MimeHelper.FallbackType);
        }
    }
}
