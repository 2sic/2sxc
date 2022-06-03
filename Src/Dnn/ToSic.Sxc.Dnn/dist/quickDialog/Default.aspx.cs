using System;

namespace ToSic.Sxc.Dnn.dist.quickDialog
{
    public class Default : CachedPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Clear();
            Response.AppendHeader("test-dev", "2sxc");

            //var sp = DnnStaticDi.GetPageScopedServiceProvider();
            //var pageService = sp.GetRequiredService<IPageService>();
            //pageService.AddCsp("test-csp", "2sxc");

            Response.Write(PageOutputCached("~/DesktopModules/ToSIC_SexyContent/dist/quickDialog/index-raw.html"));
        }
    }
}