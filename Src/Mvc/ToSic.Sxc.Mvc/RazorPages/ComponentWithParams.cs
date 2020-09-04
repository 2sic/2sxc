using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ToSic.Sxc.Mvc.RazorPages
{
    public class ComponentWithParams: ViewComponent
    {
        public int PageId;
        public int InstanceId;


        public async Task<IViewComponentResult> InvokeAsync(int pageId, int instanceId)
        {
            PageId = pageId;
            InstanceId = instanceId;

            var items = new List<string>
            {
                "Hello",
                "There",
                "something"
            };
            return View(); //items, model);
        }
    }
}
