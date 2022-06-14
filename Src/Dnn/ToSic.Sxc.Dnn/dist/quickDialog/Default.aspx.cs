using System;

namespace ToSic.Sxc.Dnn.dist.quickDialog
{
    public class Default : CachedPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AppendHeader("test-dev", "2sxc");

            Response.Write(PageOutputCached("~/DesktopModules/ToSIC_SexyContent/dist/quickDialog/index-raw.html"));
        }
    }
}