using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace Website.Pages
{
    public class EavDiModel : PageModel
    {
        public EavDiModel(ITenant tenant, IHttpContextAccessor httpC, IHttp http, IServerPaths serverPaths)
        {
            // itenant should exist
            var x = tenant.Id;
            var y = httpC.HttpContext.Request.Body;
            Http = http;
            ServerPaths = serverPaths;
        }

        public IHttp Http;
        public readonly IServerPaths ServerPaths;


        public void OnGet()
        {

        }
    }
}