using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace ToSic.SexyContent
{
    public partial class TemplateHelpGrid : System.Web.UI.UserControl
    {
        public DnnGrid Grid { get { return grdFields; } }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}