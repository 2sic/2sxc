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
        /// <summary>
        /// Returns the ContentGroupID from QueryString
        /// </summary>
        private Guid ContentGroupID
        {
            get
            {
                return Guid.Parse(Request.QueryString["ContentGroupID"]);
            }
        }

	    private ContentGroup _contentGroup;
	    private ContentGroup ContentGroup
	    {
		    get
		    {
			    if (_contentGroup == null)
				    _contentGroup = Sexy.ContentGroups.GetContentGroup(ContentGroupID);
			    return _contentGroup;
		    }
	    }

		//private List<ContentGroupItem> _Items;
		//private List<ContentGroupItem> Items
		//{
		//	get
		//	{
		//		if (_Items == null)
		//		{
		//			_Items = Sexy.Templates.GetContentGroupItems(ContentGroupID).ToList();
		//		}
		//		return _Items;
		//	}
		//}

		//private Template Template
		//{
		//	get
		//	{
		//		return Sexy.Templates.GetTemplate(Sexy.Templates.GetContentGroupItems(ContentGroupID).First().TemplateID.Value);
		//	}
		//}
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
			// ToDo: implement grid actions
			throw new Exception("ToDo: implement grid actions");
			//int ContentGroupItemID = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ID"]);
			//var item = Items.First(p => p.ContentGroupItemID == ContentGroupItemID);

			//switch (e.CommandName)
			//{
			//	case "add":
			//		SexyUncached.AddContentGroupItem(ContentGroupID, UserId, item.TemplateID.Value, null, item.SortOrder + 1, true, ContentGroupItemType.Content, false);
			//		// Refresh page
			//		Response.Redirect(Request.RawUrl);
			//		break;
			//	case "addwithedit":
			//		Response.Redirect(Sexy.GetElementAddWithEditLink(ContentGroupID, item.SortOrder + 1, ModuleId, TabId, Request.RawUrl));
			//		break;
			//}
        }

        protected void grdEntities_RowDrop(object sender, GridDragDropEventArgs e)
        {
			// ToDo: implement rowdrop
	        throw new Exception("ToDo: implement rowdrop");
	        //if (e.DestDataItem != null)
	        //{
	        //	var UncachedSexyContent = SexyUncached;

	        //	ContentGroupItem DestItem = UncachedSexyContent.Templates.GetContentGroupItem(((int)e.DestDataItem.GetDataKeyValue("ID")));
	        //	ContentGroupItem Item = UncachedSexyContent.Templates.GetContentGroupItem(((int)e.DraggedItems[0].GetDataKeyValue("ID")));
	        //	int DestinationSortOrder = DestItem.SortOrder;

	        //	if (e.DropPosition == GridItemDropPosition.Below)
	        //		DestinationSortOrder++;

	        //	if (Item.SortOrder < DestinationSortOrder)
	        //		DestinationSortOrder--;

	        //	UncachedSexyContent.Templates.ReorderContentGroupItem(Item, DestinationSortOrder, true);

	        //	grdEntities.Rebind();
	        //	grdEntities.Items[DestinationSortOrder].Selected = true;
	        //}
        }

        protected void grdEntities_NeedDatasource(object sender, EventArgs e)
        {
			// ToDo: Fix needdatasource
			throw new Exception("Fix Needdatasource");
			//_Items = null;
			//var entities = DataSource.GetInitialDataSource(this.ZoneId.Value, this.AppId.Value);
			//grdEntities.DataSource = Items.Select(i =>
			//{
			//	var entity = i.EntityID.HasValue ? new DynamicEntity(entities.List.FirstOrDefault(en => en.Key == i.EntityID.Value).Value, new [] { CultureInfo.CurrentCulture.Name }, Sexy) : null;
			//	return new
			//	{
			//		EntityID = i.EntityID,
			//		EntityTitle = entity != null ? String.IsNullOrEmpty(entity.EntityTitle.ToString()) ? "(empty)" : entity.EntityTitle : "(no demo row)",
			//		ID = i.ContentGroupItemID
			//	};
			//});

        }

        protected string GetEditUrl(int sortOrder)
        {
            return Sexy.GetElementEditLink(ContentGroupID, sortOrder, ModuleId, TabId, Request.RawUrl);
        }

        /// <summary>
        /// Returns the Settings URL
        /// </summary>
        /// <param name="ContentGroupItemID"></param>
        /// <returns></returns>
        protected string GetSettingsUrl(int sortOrder)
        {
			return Sexy.GetElementSettingsLink(ContentGroupID, sortOrder, ModuleId, TabId, Request.RawUrl);
        }

        protected void btnEditListHeader_Click(object sender, EventArgs e)
        {
            Response.Redirect(Sexy.GetElementEditLink(ContentGroupID, -1, ModuleId, TabId, Request.RawUrl));
        }
    }
}