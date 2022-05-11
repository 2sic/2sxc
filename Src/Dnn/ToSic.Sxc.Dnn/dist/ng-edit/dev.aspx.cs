using DotNetNuke.Framework;
using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.dist.ng_edit
{
    public partial class Dev : CDefault
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Clear();
            Response.AppendHeader("test-dev", "2sxc");

            //var sp = DnnStaticDi.GetPageScopedServiceProvider();
            //var pageService = sp.GetRequiredService<IPageService>();
            //pageService.AddCsp("test-csp", "2sxc");

            ResponseIndex();
        }

        private void ResponseIndex()
        {
            var path = Server.MapPath("~/DesktopModules/ToSIC_SexyContent/dist/ng-edit/index.html");
            if (!System.IO.File.Exists(path)) throw new Exception("File not found: " + path);
            var html = System.IO.File.ReadAllText(path);
            Response.Write(html);
        }
    }
}