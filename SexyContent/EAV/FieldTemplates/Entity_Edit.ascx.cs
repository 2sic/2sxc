using System;
using System.Web.UI;
using System.Linq;

namespace ToSic.Eav.ManagementUI
{
	public partial class Entity_EditCustom : FieldTemplateUserControl
	{
        protected global::DotNetNuke.UI.UserControls.LabelControl FieldLabel;

		private EntityRelationshipModel RelatedEntities
		{
			get { return (EntityRelationshipModel)FieldValue; }
		}
		private bool AllowMultiValue
		{
			get { return MetaData.ContainsKey("AllowMultiValue") && (bool)MetaData["AllowMultiValue"][DimensionIds]; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			SetDataSource();

			if (!IsPostBack && AllowMultiValue)
			{
				hfEntityIds.Visible = true;
				pnlMultiValues.Visible = true;
				phAddMultiValue.Visible = true;
				if (RelatedEntities != null)
					hfEntityIds.Value = string.Join(",", RelatedEntities.EntityIds);
			}

			DropDownList1.ToolTip = MetaData.ContainsKey("Notes") ? MetaData["Notes"][DimensionIds].ToString() : null;
			FieldLabel.Text = MetaData.ContainsKey("Name") ? MetaData["Name"][DimensionIds].ToString() : Attribute.StaticName;
            FieldLabel.HelpText = MetaData.ContainsKey("Notes") ? MetaData["Notes"][DimensionIds].ToString() : "";

			if (ShowDataControlOnly)
				FieldLabel.Visible = false;
		}

		public void SetDataSource()
		{
			var dsrc = DataSource.GetInitialDataSource(ZoneId, AppId);
			IContentType foundType = null;
			if (MetaData.ContainsKey("EntityType") && !string.IsNullOrWhiteSpace(MetaData["EntityType"][DimensionIds].ToString()))
				foundType = DataSource.GetCache(dsrc.ZoneId, dsrc.AppId).GetContentType(MetaData["EntityType"][DimensionIds].ToString());

		    var entities = from l in dsrc["Default"].List
						   where l.Value.Type == foundType || foundType == null
						   select new { Value = l.Key, Text = l.Value.Title == null || l.Value.Title[DimensionIds] == null || string.IsNullOrWhiteSpace(l.Value.Title[DimensionIds].ToString()) ? "(no Title, " + l.Key + ")" : l.Value.Title[DimensionIds] };

			DropDownList1.DataSource = entities.OrderBy(p => p.Text);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!IsPostBack)
			{
				DataBind();
			}
			base.OnPreRender(e);
		}

		public override object Value
		{
			get
			{
				if (AllowMultiValue)
					return string.IsNullOrWhiteSpace(hfEntityIds.Value) ? new int[0] : hfEntityIds.Value.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

				return string.IsNullOrEmpty(DropDownList1.SelectedValue) ? new int[0] : new int[1] { int.Parse(DropDownList1.SelectedValue) };
			}
		}

		public override Control DataControl
		{
			get { return AllowMultiValue ? (Control)DropDownList1 : hfEntityIds; }
		}

		/// <summary>
		/// Mark Item as selected
		/// </summary>
		protected void DropDownList1_DataBound(object sender, EventArgs e)
		{
			DropDownList1.ClearSelection();
			if (AllowMultiValue || RelatedEntities == null)
				return;

			foreach (var entityId in RelatedEntities.EntityIds)
			{
				var item = DropDownList1.Items.FindByValue(entityId.ToString());
				if (item != null)
				{
					item.Selected = true;
					break;
				}
			}
		}
	}
}