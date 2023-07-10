#if !NETFRAMEWORK
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Infrastructure;

namespace ToSic.Sxc.WebApi.Infrastructure
{
    public class NetCoreResponseMaker: ResponseMaker<IActionResult>
    {
        public void Init(ControllerBase apiController) => _apiController = apiController;

        private ControllerBase _apiController;

        public ControllerBase ApiController => _apiController ??
                                           throw new(
                                               $"Accessing the {nameof(ApiController)} in the {nameof(NetCoreResponseMaker)} requires it to be Init first.");

        public override IActionResult Error(int statusCode, string message)
            => ApiController.Problem(message, null, statusCode); 

        public override IActionResult Error(int statusCode, Exception exception)
            => ApiController.Problem(exception.Message, null, statusCode);

        public override IActionResult Json(object json) => new JsonResult(json);

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
#endif