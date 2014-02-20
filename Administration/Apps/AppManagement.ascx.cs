using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ToSic.SexyContent.Administration
{
    public partial class AppManagement : SexyControlAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void grdApps_NeedDataSource(object sender, EventArgs e)
        {
            grdApps.DataSource = Sexy.GetApps();
        }

        protected void btnCreateApp_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(hfNewAppName.Value))
            {
                Sexy.AddApp(hfNewAppName.Value);
                Response.Redirect(Request.RawUrl);
            }
        }

        protected void grdApps_ItemDataBound(object sender, GridItemEventArgs e)
        {
            // Disable Content-App
            if (e.Item is GridDataItem)
            {
                var item = e.Item as GridDataItem;
                if (((App)item.DataItem).Name == "Content")
                    item.Controls.Cast<GridTableCell>().ToList().ForEach(c => c.Enabled = false);
            }
        }
    }
}