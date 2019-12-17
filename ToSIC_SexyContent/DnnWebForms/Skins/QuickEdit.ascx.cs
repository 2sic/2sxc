using System;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.SexyContent.DnnWebForms.Skins
{
    public partial class QuickEdit : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT"))
                new DnnRenderingHelpers(null, null).RegisterClientDependencies(Page, true, true, true);
        }
    }
}