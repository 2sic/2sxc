using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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

        private List<Element> _Elements;
        private List<Element> Elements
        {
            get
            {
                if (_Elements == null)
                {
                    _Elements = Sexy.GetContentElements(ModuleId, Sexy.GetCurrentLanguageName(), ContentGroupID, PortalId).ToList();
                }
                return _Elements;
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
            Element Element = Elements.First(p => p.ID == ContentGroupItemID);

            switch (e.CommandName)
            {
                case "add":
                    Sexy.AddContentGroupItem(ContentGroupID, UserId, Element.TemplateId.Value, null, Element.SortOrder + 1, true, ContentGroupItemType.Content, false);
                    grdEntities.Rebind();
                    grdEntities.Items[Element.SortOrder].Selected = true;
                    break;
                case "addwithedit":
                    Response.Redirect(Sexy.GetElementAddWithEditLink(ContentGroupID, Element.SortOrder + 1, ModuleId, TabId, Request.RawUrl));
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
            _Elements = null;
            grdEntities.DataSource = from c in Elements
                                     select new
                                     {
                                         EntityID = c.EntityId,
                                         EntityTitle = c.Content != null ? String.IsNullOrEmpty(c.Content.EntityTitle.ToString()) ? "(empty)" : c.Content.EntityTitle : "(no demo row)",
                                         ID = c.ID
                                     };
        }

        protected string GetEditUrl(int ContentGroupItemID)
        {
            Element Item = Elements.Where(p => p.ID == ContentGroupItemID).Single();
            return Sexy.GetElementEditLink(Item.GroupID, Item.SortOrder, ModuleId, TabId, Request.RawUrl);
        }

        /// <summary>
        /// Returns the Settings URL
        /// </summary>
        /// <param name="ContentGroupItemID"></param>
        /// <returns></returns>
        protected string GetSettingsUrl(int ContentGroupItemID)
        {
            Element Item = Elements.Where(p => p.ID == ContentGroupItemID).Single();
            return Sexy.GetElementSettingsLink(Item.ID, ModuleId, TabId, Request.RawUrl);
        }

        protected void btnEditListHeader_Click(object sender, EventArgs e)
        {
            Response.Redirect(Sexy.GetElementEditLink(ContentGroupID, -1, ModuleId, TabId, Request.RawUrl));
        }
    }
}