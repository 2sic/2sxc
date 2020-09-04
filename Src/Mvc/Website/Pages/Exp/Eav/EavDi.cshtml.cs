using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToSic.Eav.Run;
using ToSic.Sxc.Web;

namespace Website.Pages
{
    public class EavDiModel : PageModel
    {
        public EavDiModel(ITenant tenant, IHttpContextAccessor httpC, IHttp http)
        {
            // itenant should exist
            var x = tenant.Id;
            var y = httpC.HttpContext.Request.Body;
            Http = http;
        }

        public IHttp Http;


        public void OnGet()
        {

        }
    }
}