using System;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent
{
    public partial class QuickInsert : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderingHelpers.RegisterClientDependencies(Page);
        }
    }
}