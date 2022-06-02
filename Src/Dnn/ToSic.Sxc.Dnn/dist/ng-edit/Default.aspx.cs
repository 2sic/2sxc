using DotNetNuke.Framework;
using System;
using System.IO;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.dist.ng_edit
{
    public partial class Default : CachedPageBase
    {
        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Clear();
            Response.AppendHeader("test-dev", "2sxc");

            //var sp = DnnStaticDi.GetPageScopedServiceProvider();
            //var pageService = sp.GetRequiredService<IPageService>();
            //pageService.AddCsp("test-csp", "2sxc");

            var html = PageOutputCached("~/DesktopModules/ToSIC_SexyContent/dist/ng-edit/eav-ui.html");
            var content = DnnJsApi.GetJsApiJson(-1);
            html = JsApi.UpdateMetaTagJsApi(html, content);
            Response.Write(html);
        }
    }
}