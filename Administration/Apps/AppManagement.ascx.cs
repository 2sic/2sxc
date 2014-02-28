using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using ToSic.Eav;

namespace ToSic.SexyContent.Administration
{
    public partial class AppManagement : SexyControlAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void grdApps_NeedDataSource(object sender, EventArgs e)
        {
            grdApps.DataSource = SexyContent.GetApps(ZoneId.Value, true);
        }

        protected void btnCreateApp_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(hfNewAppName.Value))
            {
                SexyContent.AddApp(ZoneId.Value, hfNewAppName.Value);
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

        /// <summary>
        /// GridView DeleteCommand, deletes the app that caused the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdApps_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            int appId = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["AppID"]);
            new SexyContent(ZoneId.Value, appId, false).RemoveApp(appId, UserId);
            grdApps.Rebind();
        }

        
    }
}