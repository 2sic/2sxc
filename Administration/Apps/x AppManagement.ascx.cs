using System;
using System.Linq;
using DotNetNuke.Entities.Portals;
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
            grdApps.DataSource = SexyContent.GetApps(ZoneId.Value, true, new PortalSettings(ModuleConfiguration.OwnerPortalID));
        }

        protected void btnCreateApp_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(hfNewAppName.Value))
            {
                SexyContent.AddApp(ZoneId.Value, hfNewAppName.Value, new PortalSettings(ModuleConfiguration.OwnerPortalID));
                Response.Redirect(Request.RawUrl);
            }
        }

        protected void grdApps_ItemDataBound(object sender, GridItemEventArgs e)
        {
            // Disable Content-App
            if (e.Item is GridDataItem)
            {
                var item = e.Item as GridDataItem;
                if (((App) item.DataItem).Name == "Content")
                {
                    item.Controls.Cast<GridTableCell>().ToList().ForEach(c => c.Enabled = false);
                }
            }
        }

        /// <summary>
        /// GridView DeleteCommand, deletes the app that caused the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdApps_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (hfAppNameToDelete.Value == "")
                return;

            var item = grdApps.Items[e.CommandArgument.ToString()];
            var appId = (int) item.GetDataKeyValue("AppId");

            //int appId = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["AppId"]);
            var sexy = new SexyContent(ZoneId.Value, appId, false);

            if (sexy.App.Name == hfAppNameToDelete.Value)
            {
                sexy.RemoveApp(appId, UserId);
                grdApps.Rebind();
            }

            hfAppNameToDelete.Value = "";
        }

        
    }
}