using System;
using System.Globalization;
using System.Linq;
using DotNetNuke.Common;
using Telerik.Web.UI;

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
				    _contentGroup = Sexy.ContentGroups.GetContentGroupForModule(ModuleId);
			    return _contentGroup;
		    }
	    }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkCancel.NavigateUrl = Globals.NavigateURL(TabId, "", null);

	        pnlEditListHeader.Visible = !String.IsNullOrEmpty(ContentGroup.Template.ListContentTypeStaticName);

            if(!IsPostBack)
                grdEntities.DataBind();
        }

        protected void grdEntities_ItemCommand(object sender, GridCommandEventArgs e)
        {
			var sortOrder = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["SortOrder"]);

			switch (e.CommandName)
			{
				case "add":
					ContentGroup.AddContentAndPresentationEntity("content", sortOrder + 1, null, null);
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

		///// <summary>
		///// Returns the Settings URL
		///// </summary>
		///// <param name="ContentGroupItemID"></param>
		///// <returns></returns>
		//protected string GetSettingsUrl(string itemType, int sortOrder)
		//{
		//	return Sexy.GetElementSettingsLink(ContentGroup.ContentGroupGuid, sortOrder, ModuleId, TabId, Request.RawUrl);
		//}

        protected void btnEditListHeader_Click(object sender, EventArgs e)
        {
			Response.Redirect(Sexy.GetElementEditLink(ContentGroup.ContentGroupGuid, -1, ModuleId, TabId, Request.RawUrl));
        }
    }
}