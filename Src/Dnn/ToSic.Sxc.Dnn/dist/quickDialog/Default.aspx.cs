using System;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.dist.quickDialog
{
    public class Default : CachedPageBase
    {
        // TODO: @STV - this is all duplicate code - pls move to function in the base class, which you call here with the path
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Clear();
            Response.AppendHeader("test-dev", "2sxc");

            //var sp = DnnStaticDi.GetPageScopedServiceProvider();
            //var pageService = sp.GetRequiredService<IPageService>();
            //pageService.AddCsp("test-csp", "2sxc");

            var pageIdString = Request.QueryString["pageId"];
            var pageId = pageIdString.HasValue() ? Convert.ToInt32(pageIdString) : -1;

            var html = PageOutputCached("~/DesktopModules/ToSIC_SexyContent/dist/quickDialog/index-raw.html");
            var content = DnnJsApi.GetJsApiJson(pageId);
            html = JsApi.UpdateMetaTagJsApi(html, content);
            Response.Write(html);
        }
    }
}