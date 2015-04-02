using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Security.Permissions;
using Telerik.Web.UI;
using DotNetNuke.Web.UI.WebControls;
using ToSic.Eav;
using ToSic.SexyContent;

namespace ToSic.SexyContent
{
    public partial class EditList : SexyControlEditBase
    {
        
        #region Private Properties

	    private ContentGroup _contentGroup;
	    private ContentGroup ContentGroup
	    {
		    get
		    {
			    if (_contentGroup == null)
				    _contentGroup = Sexy.ContentGroups.GetContentGroup(Sexy.GetContentGroupIdFromModule(this.ModuleId));
			    return _contentGroup;
		    }
	    }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId, "", null);

	        pnlEditListHeader.Visible = !String.IsNullOrEmpty(ContentGroup.Template.ListContentTypeStaticName);

            if(!IsPostBack)
                grdEntities.DataBind();
        }

        protected void grdEntities_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
			int sortOrder = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["SortOrder"]);

			switch (e.CommandName)
			{
				case "add":
					ContentGroup.AddContentAndPresentationEntity(sortOrder + 1);
					Response.Redirect(Request.RawUrl);
					break;
				case "addwithedit":
					Response.Redirect(Sexy.GetElementAddWithEditLink(ContentGroup.ContentGroupGuid, sortOrder + 1, ModuleId, TabId, Request.RawUrl));
					break;
			}
        }

        protected void grdEntities_RowDrop(object sender, GridDragDropEventArgs e)
        {
	        if (e.DestDataItem == null)
				return;

	        var sortOrder = (int)e.DraggedItems[0].GetDataKeyValue("SortOrder");
	        var destinationSortOrder = (int) e.DestDataItem.GetDataKeyValue("SortOrder");

	        ContentGroup.ReorderEntities(sortOrder, destinationSortOrder);

			// Refresh cached contentgroup
	        _contentGroup = null;

	        grdEntities.Rebind();
	        grdEntities.Items[destinationSortOrder].Selected = true;
        }

        protected void grdEntities_NeedDatasource(object sender, EventArgs e)
        {
			var entities = ContentGroup.Content;
			grdEntities.DataSource = entities.Select((en, i) =>
			{
				var entity = en != null ? new DynamicEntity(en, new[] { CultureInfo.CurrentCulture.Name }, Sexy) : null;
				return new
				{
					EntityId = en == null ? new int?() : en.EntityId,
					EntityTitle = entity != null ? String.IsNullOrEmpty(entity.EntityTitle.ToString()) ? "(empty)" : entity.EntityTitle : "(demo row)",
					SortOrder = i
				};
			});
        }

        protected string GetEditUrl(int sortOrder)
        {
            return Sexy.GetElementEditLink(ContentGroup.ContentGroupGuid, sortOrder, ModuleId, TabId, Request.RawUrl);
        }

        /// <summary>
        /// Returns the Settings URL
        /// </summary>
        /// <param name="ContentGroupItemID"></param>
        /// <returns></returns>
        protected string GetSettingsUrl(int sortOrder)
        {
			return Sexy.GetElementSettingsLink(ContentGroup.ContentGroupGuid, sortOrder, ModuleId, TabId, Request.RawUrl);
        }

        protected void btnEditListHeader_Click(object sender, EventArgs e)
        {
			Response.Redirect(Sexy.GetElementEditLink(ContentGroup.ContentGroupGuid, -1, ModuleId, TabId, Request.RawUrl));
        }
    }
}