using ToSic.Sxc.Web.Internal.EditUi;

namespace ToSic.Sxc.Dnn.dist.quick_dialog;

public class Default : CachedPageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.AppendHeader("test-dev", "2sxc");

        Response.Write(PageOutputCached("~/DesktopModules/ToSIC_SexyContent/dist/quick-dialog/index-raw.html", EditUiResourceSettings.QuickDialog));
    }
}