using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToSic.SexyContent.Administration
{
    public partial class AppManagement : SexyControlAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdApps_NeedDataSource(object sender, EventArgs e)
        {
            grdApps.DataSource = Sexy.ContentContext.Apps.Where(p => p.ZoneID == ZoneId);
        }
    }
}