using ToSic.Sxc.Web.Internal.EditUi;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn.dist.eavUi;

public class Default : CachedPageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.AppendHeader("test-dev", "2sxc");

        Response.Write(PageOutputCached("~/DesktopModules/ToSIC_SexyContent/dist/ng-edit/index-raw.html",
            EditUiResourceSettings.EditUi));

        // HACK: opening editui will change user language in cookie, so disable that
        //if (Response.Cookies["language"] != null) Response.Cookies.Remove("language");
    }
}