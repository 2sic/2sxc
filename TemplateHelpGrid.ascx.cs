using System;
using System.Web.UI;
using DotNetNuke.Web.UI.WebControls;

namespace ToSic.SexyContent
{
    public partial class TemplateHelpGrid : UserControl
    {
        public DnnGrid Grid { get { return grdFields; } }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}