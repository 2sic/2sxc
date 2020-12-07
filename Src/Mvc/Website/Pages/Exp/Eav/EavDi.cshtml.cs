using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace Website.Pages
{
    public class EavDiModel : PageModel
    {
        public EavDiModel(ISite site, IHttpContextAccessor httpC, IHttp http, IServerPaths serverPaths, ILinkPaths linkPaths)
        {
            // itenant should exist
            var x = site.Id;
            var y = httpC.HttpContext.Request.Body;
            Http = http;
            ServerPaths = serverPaths;
            LinkPaths = linkPaths;
        }

        public IHttp Http;
        public readonly IServerPaths ServerPaths;
        public readonly ILinkPaths LinkPaths;


        public void OnGet()
        {

        }
    }
}