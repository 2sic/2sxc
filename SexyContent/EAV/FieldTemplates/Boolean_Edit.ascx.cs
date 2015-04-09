using System;
using System.Web.UI;
using DotNetNuke.UI.UserControls;

namespace ToSic.Eav.ManagementUI
{
	public partial class Boolean_EditCustom : FieldTemplateUserControl
	{
        protected LabelControl FieldLabel;

		protected void Page_Load(object sender, EventArgs e)
		{
            CheckBox1.ToolTip = GetMetaDataValue<string>("Notes");
            FieldLabel.Text = GetMetaDataValue("Name", Attribute.StaticName);

			if (ShowDataControlOnly)
				FieldLabel.Visible = false;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!IsPostBack)
			{
				DataBind();
			}
            FieldLabel.Text = GetMetaDataValue("Name", Attribute.StaticName);
            FieldLabel.HelpText = GetMetaDataValue<string>("Notes");

            var Value = false;
            if (Boolean.TryParse(FieldValueEditString, out Value))
                CheckBox1.Checked = Value;
            else if (MetaData.ContainsKey("DefaultValue") && !String.IsNullOrEmpty(((IAttribute<string>)MetaData["DefaultValue"]).Typed[DimensionIds]) && Boolean.TryParse(((IAttribute<string>)MetaData["DefaultValue"]).Typed[DimensionIds], out Value))
                CheckBox1.Checked = Value;
            else
                CheckBox1.Checked = false;

			base.OnPreRender(e);
		}

		public override object Value
		{
            get { return CheckBox1.Checked; }
		}


		public override Control DataControl
		{
			get { return CheckBox1; }
		}

	}
}
