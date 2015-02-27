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
        private int ContentGroupID
        {
            get
            {
                return int.Parse(Request.QueryString[SexyContent.ContentGroupIDString]);
            }
        }

		private List<ContentGroupItem> _Items;
		private List<ContentGroupItem> Items
		{
			get
			{
				if (_Items == null)
				{
					_Items = Sexy.TemplateContext.GetContentGroupItems(ContentGroupID).ToList();
				}
				return _Items;
			}
		}

        private Template Template
        {
            get
            {
                return Sexy.TemplateContext.GetTemplate(Sexy.TemplateContext.GetContentGroupItems(ContentGroupID).First().TemplateID.Value);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId, "", null);
            var ListContentTemplateDefault = Sexy.GetTemplateDefault(Template.TemplateID, ContentGroupItemType.ListContent);
            pnlEditListHeader.Visible = (ListContentTemplateDefault != null && ListContentTemplateDefault.ContentTypeID.HasValue);            

            if(!IsPostBack)
                grdEntities.DataBind();
        }

        protected void grdEntities_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            int ContentGroupItemID = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ID"]);
            var item = Items.First(p => p.ContentGroupItemID == ContentGroupItemID);

            switch (e.CommandName)
            {
                case "add":
                    SexyUncached.AddContentGroupItem(ContentGroupID, UserId, item.TemplateID.Value, null, item.SortOrder + 1, true, ContentGroupItemType.Content, false);
                    // Refresh page
                    Response.Redirect(Request.RawUrl);
                    break;
                case "addwithedit":
                    Response.Redirect(Sexy.GetElementAddWithEditLink(ContentGroupID, item.SortOrder + 1, ModuleId, TabId, Request.RawUrl));
                    break;
            }
        }

        protected void grdEntities_RowDrop(object sender, GridDragDropEventArgs e)
        {
            if (e.DestDataItem != null)
            {
                var UncachedSexyContent = SexyUncached;

                ContentGroupItem DestItem = UncachedSexyContent.TemplateContext.GetContentGroupItem(((int)e.DestDataItem.GetDataKeyValue("ID")));
                ContentGroupItem Item = UncachedSexyContent.TemplateContext.GetContentGroupItem(((int)e.DraggedItems[0].GetDataKeyValue("ID")));
                int DestinationSortOrder = DestItem.SortOrder;

                if (e.DropPosition == GridItemDropPosition.Below)
                    DestinationSortOrder++;

                if (Item.SortOrder < DestinationSortOrder)
                    DestinationSortOrder--;

                UncachedSexyContent.TemplateContext.ReorderContentGroupItem(Item, DestinationSortOrder, true);

                grdEntities.Rebind();
                grdEntities.Items[DestinationSortOrder].Selected = true;
            }
        }

        protected void grdEntities_NeedDatasource(object sender, EventArgs e)
        {
            _Items = null;
	        var entities = DataSource.GetInitialDataSource(this.ZoneId.Value, this.AppId.Value);
	        grdEntities.DataSource = Items.Select(i =>
	        {
		        var entity = i.EntityID.HasValue ? new DynamicEntity(entities.List.FirstOrDefault(en => en.Key == i.EntityID.Value).Value, new [] { CultureInfo.CurrentCulture.Name }, Sexy) : null;
		        return new
		        {
			        EntityID = i.EntityID,
			        EntityTitle = entity != null ? String.IsNullOrEmpty(entity.EntityTitle.ToString()) ? "(empty)" : entity.EntityTitle : "(no demo row)",
			        ID = i.ContentGroupItemID
		        };
	        });

        }

        protected string GetEditUrl(int ContentGroupItemID)
        {
            var item = Items.Single(p => p.ContentGroupItemID == ContentGroupItemID);
            return Sexy.GetElementEditLink(item.ContentGroupID, item.SortOrder, ModuleId, TabId, Request.RawUrl);
        }

        /// <summary>
        /// Returns the Settings URL
        /// </summary>
        /// <param name="ContentGroupItemID"></param>
        /// <returns></returns>
        protected string GetSettingsUrl(int ContentGroupItemID)
        {
            var item = Items.Single(p => p.ContentGroupItemID == ContentGroupItemID);
            return Sexy.GetElementSettingsLink(item.ContentGroupItemID, ModuleId, TabId, Request.RawUrl);
        }

        protected void btnEditListHeader_Click(object sender, EventArgs e)
        {
            Response.Redirect(Sexy.GetElementEditLink(ContentGroupID, -1, ModuleId, TabId, Request.RawUrl));
        }
    }
}