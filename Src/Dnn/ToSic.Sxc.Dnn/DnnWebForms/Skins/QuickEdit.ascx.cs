using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Services;
// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.Dnn.DnnWebForms.Skins;

public partial class QuickEdit : System.Web.UI.UserControl
{
    private bool _isEdit;
    protected void Page_Load(object sender, EventArgs e)
    {
        _isEdit = DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT");
        if (!_isEdit)
            return;

        this.GetScopedService<DnnClientResources>()
            // #RemovedV20 #OldDnnAntiForgery
            .Init(Page, /*false,*/ null)
            .RegisterClientDependencies(Page, true, true, true);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        // this is temp solution, because it was required for 2sxc module instance created in skin
        if (!_isEdit)
            return;
        this.GetScopedService<DnnJsApiHeader>().AddHeaders();
    }
}