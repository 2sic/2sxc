using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Custom.Hybrid;
using ToSic.Sxc.Code;
using ToSic.Sxc.Mvc;
using ToSic.Sxc.Mvc.Web;
using ToSic.Sxc.Razor;
using ToSic.Sxc.Razor.Internal;

namespace Website.Pages.RenderPartial
{
    public class Render2Model : PageModel
    {
        private readonly IRazorRenderer _renderer;
        private readonly SxcMvc _sxcMvc;
        public Render2Model(IRazorRenderer renderer, SxcMvc sxcMvc)
        {
            _renderer = renderer;
            _sxcMvc = sxcMvc;
        }

        public async Task OnGetAsync()
        {
            DynamicCodeRoot dynCode = null;// _sxcMvc.CreateDynCode(TestIds.Blog, null);

            var path = $"/{MvcConstants.WwwRoot}2sxc/Blog App/_1 Main blog view.cshtml";

            InnerRender = await _renderer.RenderToStringAsync(
                path,
                new ContactForm
                {
                    Email = "something@somewhere",
                    Message = "Hello message!",
                    Name = "The Dude",
                    Priority = Priority.Medium,
                    Subject = "This is the subject"
                }, rzv =>
                {
                    if (rzv.RazorPage is IRazor12 asSxc)
                    {
                        asSxc._DynCodeRoot = dynCode;
                        //asSxc.VirtualPath = path;
                        //asSxc.Purpose = Purpose.WebView;
                    }
                });
        }


        public string InnerRender;
    }

}