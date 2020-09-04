using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Engines;
using ToSic.Sxc.Mvc.RazorPages;

namespace Website.Pages.RenderPartial
{
    public class Render2Model : PageModel
    {
        private readonly IRenderRazor _renderer;
        public Render2Model(IRenderRazor renderer)
        {
            _renderer = renderer;
        }

        public async Task OnGetAsync()
        {
            var dynCode = SxcMvc.CreateDynCode(TestIds.Blog, null);

            var path = "/wwwroot/2sxc/Blog App/_1 Main blog view.cshtml";

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
                    if (rzv.RazorPage is IIsSxcRazorPage asSxc)
                    {
                        asSxc.DynCode = dynCode;
                        asSxc.VirtualPath = path;
                        asSxc.Purpose = Purpose.WebView;
                    }
                });
        }


        public string InnerRender;
    }
    
}