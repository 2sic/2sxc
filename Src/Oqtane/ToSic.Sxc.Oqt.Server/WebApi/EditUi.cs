using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public class EditUi
    {
        public static Task HandleEditUi(HttpContext context, IWebHostEnvironment env)
        {
            var path = Path.Combine(env.WebRootPath, @"Modules\ToSic.Sxc\dist\ng-edit\index.html");
            if (!File.Exists(path)) throw new FileNotFoundException("File not found: " + path);

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.Headers.Add("test-dev", "2sxc");
            context.Response.ContentType = "text/html";
            context.Response.Body.WriteAsync(File.ReadAllBytes(path));
            
            return Task.CompletedTask;
        }
    }
}
