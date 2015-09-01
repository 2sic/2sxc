using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using Telerik.Web.UI;
using ToSic.Eav;

namespace ToSic.SexyContent.Configuration
{
    public partial class PortalConfiguration : SexyControlAdminBase
    {
        
        #region Properties

        private int? CurrentZoneID {
            get { return SexyContent.GetZoneID(PortalId); }
        }
        private int? SelectedZoneID {
            get {
                if (ddlZones.SelectedValue != "-1")
                    return int.Parse(ddlZones.SelectedValue);
                return null;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindZones();

            // Localize Zone Infotext
            litZoneInfo.Text = LocalizeString("litZoneInfo.Text");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Renaming a zone is not possible until one is selected
            btnRenameZone.Visible = SelectedZoneID.HasValue;

            grdCultures.Visible = SelectedZoneID.HasValue;
            pnlSpecifyZoneFirst.Visible = !SelectedZoneID.HasValue;
        }

        /// <summary>
        /// Bind the Zones Dropdown
        /// </summary>
        private void BindZones()
        {
            var DefaultText = LocalizeString("ZoneDefault.Text");
            ddlZones.Items.Clear();
            ddlZones.Items.Add(new ListItem(DefaultText, "-1"));

            // Bind Zones in DropDown
            ddlZones.DataSource = GetAvailableZones();
            ddlZones.DataBind();
            ddlZones.Enabled = !CurrentZoneID.HasValue;
            btnCreateZone.Visible = !CurrentZoneID.HasValue;

            // Set Current ZoneID in DropDown, if specified
            if (CurrentZoneID.HasValue && ddlZones.Items.FindByValue(CurrentZoneID.Value.ToString()) != null)
                ddlZones.SelectedValue = CurrentZoneID.Value.ToString();
        }

        /// <summary>
        /// Returns the available Zones (VDB's) for the current user.
        /// Host users can see all zones, while Administrators may only
        /// see the current zone, if specified.
        /// </summary>
        private List<Zone> GetAvailableZones()
        {
            if (PortalSettings.UserInfo.IsSuperUser && !CurrentZoneID.HasValue)
                return SexyContent.GetZones();

            if(CurrentZoneID.HasValue)
                return new List<Zone> { SexyContent.GetZones().Single(z => z.ZoneID == CurrentZoneID.Value) };

            return new List<Zone>();
        }

        /// <summary>
        /// Gets called when doing any action in the Cultures grid (activate or deactivate)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdCultures_ItemCommand(object sender, GridCommandEventArgs e)
        {
            var Item = e.Item as GridEditableItem;
            var CultureCode = Item.GetDataKeyValue("Code").ToString();

            // Activate or Deactivate the Culture
            Sexy.SetCultureState(CultureCode, e.CommandName == "Activate", PortalId);

            // Re-bind the grid
            grdCultures.Rebind();
        }

        protected void grdCultures_NeedDatasource(object sender, GridNeedDataSourceEventArgs e)
        {
            // Set DataSource of the Cultures grid
            grdCultures.DataSource = SexyContent.GetCulturesWithActiveState(PortalId, SelectedZoneID.HasValue ? SelectedZoneID.Value : 1);
        }

        protected string GetTooltipMessage(string Code, bool AllowStateChange)
        {
            if (AllowStateChange)
                return "";
            if (Code == PortalSettings.DefaultLanguage)
                return LocalizeString("DefaultLanguageCannotBeDisabled.Text");
            return String.Format(LocalizeString("EnableDefaultLanguageFirst.Text"), PortalSettings.DefaultLanguage);
        }

        protected void btnRenameZone_Click(object sender, EventArgs e)
        {
            var SelectedZoneIDBefore = SelectedZoneID;
            Sexy.ContentContext.Zone.UpdateZone(SelectedZoneID.Value, hfZoneName.Value);
            BindZones();
            ddlZones.SelectedValue = SelectedZoneIDBefore.Value.ToString();
        }

        protected void btnCreateZone_Click(object sender, EventArgs e)
        {
            var NewZone = SexyContent.AddZone(hfZoneName.Value);
            SexyContent.SetZoneID(NewZone.ZoneID, PortalId);
            BindZones();
            ddlZones.SelectedValue = NewZone.ZoneID.ToString();
            grdCultures.Rebind();
        }

        /// <summary>
        /// Save and redirect on Save Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void hlkSave_Click(object sender, EventArgs e)
        {
            SexyContent.SetZoneID(SelectedZoneID, PortalId);
            RedirectBack();
        }

        protected void RedirectBack()
        {
            if (!String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                Response.Redirect(Request.QueryString["ReturnUrl"], true);
            else
                Response.Redirect(Globals.NavigateURL(TabId), true);
        }

        protected void ddlZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Re-bind the cultures grid, because Zone has changed
            grdCultures.Rebind();
        }
    }
}