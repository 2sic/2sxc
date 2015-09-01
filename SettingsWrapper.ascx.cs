using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Change Layout or Content
    /// </summary>
    public partial class SettingsWrapper : SexyControlEditBase
    {

	    public string ItemType
	    {
			get { return Request.QueryString["ItemType"]; }
	    }

	    public int SortOrder
	    {
	        get
	        {
	            if (ItemType == "ListContent")
	                return 0; // not -1 any more, that was the old way of defining the list-header
	            return int.Parse(Request.QueryString["SortOrder"]); 
	        }
	    }

		/// <summary>
		/// Page init for the settingswrapper control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Init(object sender, EventArgs e)
		{
			// Nothing to do if PostBack
			if (Page.IsPostBack)
				return;

			// Break if there is no ContentGroupItem
			if (!ContentGroup.Exists || ContentGroup[ItemType] == null || ContentGroup.Template == null)
				throw new Exception("Cannot find content group");

            // try to get the entityId. Sometimes it will try to get #0 which doesn't exist yet, that's why it has these checks
		    var itemList = ContentGroup[ItemType];
			var entity =  (itemList.Count > SortOrder) ? itemList[SortOrder] : null;
			var entityID = entity == null ? new int?() : entity.EntityId;

			var attributeSetName = ItemType == "Content" ? ContentGroup.Template.ContentTypeStaticName : ContentGroup.Template.ListContentTypeStaticName;

			if (!String.IsNullOrEmpty(attributeSetName))
			{
				var dataSource = DataSource.GetInitialDataSource(ZoneId.Value, AppId.Value);
				dataSource = DataSource.GetDataSource<EntityTypeFilter>(ZoneId.Value, AppId.Value, dataSource);
				((EntityTypeFilter) dataSource).TypeName = attributeSetName;

				ddlEntities.DataSource =
					dataSource.List.Select(p => new
					{
						EntityTitle = (p.Value.Title != null ? p.Value.Title[0] : "[?]") + " (" + p.Value.EntityId + ")",
						EntityID = p.Value.EntityId
					});
				ddlEntities.DataBind();
			}

			if (entityID.HasValue)
				ddlEntities.SelectedValue = entityID.Value.ToString();

		}

		/// <summary>
		/// Call "UpdateSettings" on the settings control and close the modal window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSave_Click(object sender, EventArgs e)
		{
			var entityId = ddlEntities.SelectedValue != "-1" ? int.Parse(ddlEntities.SelectedValue) : new int?();
            ContentGroup.UpdateEntity(ItemType, SortOrder, entityId);
			RedirectReturn();
		}

		/// <summary>
		/// Close the modal window without saving the settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCancel_Click(object sender, EventArgs e)
		{
			RedirectReturn();
		}

		private void RedirectReturn()
		{
			if (String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
				Response.Redirect(ModuleContext.NavigateUrl(TabId, "", true));
			else
				Response.Redirect(Request.QueryString["ReturnUrl"]);
		}
    }
}