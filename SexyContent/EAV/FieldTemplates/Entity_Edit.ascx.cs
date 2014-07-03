using System;
using System.Collections;
using System.Collections.Generic;
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
            get { return GetMetaDataValue<bool>("AllowMultiValue"); }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack && AllowMultiValue)
			{
                hfEntityIds.Visible = true;
                if(RelatedEntities != null)
                    hfEntityIds.Value = string.Join(",", RelatedEntities.EntityIds);
			}

            FieldLabel.Text = GetMetaDataValue("Name", Attribute.StaticName);
            FieldLabel.HelpText = GetMetaDataValue<string>("Notes");

			if (ShowDataControlOnly)
				FieldLabel.Visible = false;

            // Set configuration on hiddenfield
            var configurationObject = new
            {
                AllowMultiValue = GetMetaDataValue<bool?>("AllowMultiValue"),
                Entities = SelectableEntities(),
                SelectedEntities = RelatedEntities != null ? RelatedEntities.EntityIds : new List<int>()
            };
            hfConfiguration.Attributes.Add("ng-init", "configuration=" + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(configurationObject) + "");
		}

        protected IEnumerable SelectableEntities()
        {
            var dsrc = DataSource.GetInitialDataSource(ZoneId, AppId);
            IContentType foundType = null;
            var strEntityType = GetMetaDataValue<string>("EntityType");
            if (!string.IsNullOrWhiteSpace(strEntityType))
                foundType = DataSource.GetCache(dsrc.ZoneId, dsrc.AppId).GetContentType(strEntityType);

            var entities = from l in dsrc["Default"].List
                           where l.Value.Type == foundType || foundType == null
                           select new { Value = l.Key, Text = l.Value.Title == null || l.Value.Title[DimensionIds] == null || string.IsNullOrWhiteSpace(l.Value.Title[DimensionIds].ToString()) ? "(no Title, " + l.Key + ")" : l.Value.Title[DimensionIds] };

            return entities;
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
				return string.IsNullOrWhiteSpace(hfEntityIds.Value) ? new int[0] : hfEntityIds.Value.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
			}
		}

		public override Control DataControl
		{
			get { return hfEntityIds; }
		}

	}
}