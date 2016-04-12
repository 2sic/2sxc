using System;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.DnnWebForms.Skins
{
    public partial class QuickInsert : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT"))
                new RenderingHelpers(null).RegisterClientDependencies(Page);
        }
    }
}