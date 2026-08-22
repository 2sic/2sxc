using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Web.Sys.EditUi;

namespace ToSic.Sxc.Dnn.dist.quick_dialog;

public class Default(
    IPortalController portalController,
    IApplicationStatusInfo applicationStatusInfo,
    IHostSettings hostSettings)
    : CachedPageBase(portalController, applicationStatusInfo, hostSettings)
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.AppendHeader("test-dev", "2sxc");

        Response.Write(PageOutputCached("~/DesktopModules/ToSic.Sxc/dist/quick-dialog/index-raw.html", EditUiResourceSettings.QuickDialog));
    }
}