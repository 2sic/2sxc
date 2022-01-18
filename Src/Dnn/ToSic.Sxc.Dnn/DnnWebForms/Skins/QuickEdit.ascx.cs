using System;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.Sxc.Dnn.DnnWebForms.Skins
{
    public partial class QuickEdit : System.Web.UI.UserControl
    {
        private bool isEdit = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            isEdit = DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT");
            if (isEdit)
                DnnStaticDi.GetServiceProvider().Build<DnnClientResources>()
                    .Init(Page, null, null)
                    .RegisterClientDependencies(Page, true, true, true);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // this is temp solution, because it was required for 2sxc module instance created bin skin
            if (isEdit)
                new DnnJsApiHeader(null).AddHeaders();
        }
    }
}