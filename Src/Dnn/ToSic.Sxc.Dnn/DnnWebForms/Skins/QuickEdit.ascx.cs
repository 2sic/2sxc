using System;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.SexyContent.DnnWebForms.Skins
{
    public partial class QuickEdit : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT"))
                DnnStaticDi.StaticBuild<DnnClientResources>()
                    .Init(Page, null, null)
                    .RegisterClientDependencies(Page, true, true, true);
        }
    }
}