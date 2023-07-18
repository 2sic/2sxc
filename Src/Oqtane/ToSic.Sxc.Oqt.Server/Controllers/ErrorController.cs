//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Oqtane.Shared;
//using ToSic.Sxc.Oqt.Shared.Helpers;

//namespace ToSic.Sxc.Oqt.Server.Controllers
//{
//    [ApiController]
//    [AllowAnonymous]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class ErrorController : ControllerBase
//    {
//        [Route("/error")]
//        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
//        {
//            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

//            // super user and admins gets all details in error
//            if (User.IsInRole(RoleNames.Host) || User.IsInRole(RoleNames.Admin))
//                return Problem(
//                    detail: ErrorHelper.ErrorMessage(context.Error, true),
//                    title: context.Error.Message);

//            // normal users gets minimal error message.
//            return Problem(
//                title: context.Error.Message);
//        }
//    }
//}
