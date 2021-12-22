using System;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.SexyContent.DnnWebForms.Skins
{
    public partial class QuickEdit : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT"))
                DnnStaticDi.GetServiceProvider().Build<DnnClientResources>()
                    .Init(Page, null, null)
                    .RegisterClientDependencies(Page, true, true, true);
        }
    }
}