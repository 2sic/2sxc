using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
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

            // Set configuration on attribute
		    var configuration = new
		    {
                AllowMultiValue = GetMetaDataValue<bool?>("AllowMultiValue"),
                DimensionId = (DimensionIds != null && DimensionIds.Any()) ? DimensionIds.First() : new int?(),
                AppId = AppId,
                ZoneId = ZoneId,
                SelectedEntities = RelatedEntities != null ? RelatedEntities.EntityIds : new List<int>(),
                AttributeSetId = ContentType == null ? new int?() : ContentType.AttributeSetId
		    };
            hfConfiguration.Value = new JavaScriptSerializer().Serialize(configuration);
		}

	    private IContentType ContentType
	    {
	        get
	        {
	            var strEntityType = GetMetaDataValue<string>("EntityType");
                if (!string.IsNullOrWhiteSpace(strEntityType))
                    return DataSource.GetCache(ZoneId.Value, AppId).GetContentType(strEntityType);
	            return null;
	        }
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