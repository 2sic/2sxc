using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            // TODO: EnsureSchemaOperation that errors are displayed just to super user... norma user should het simple message ...
            
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            Response.ContentType = "application/json";

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }
    }
}
