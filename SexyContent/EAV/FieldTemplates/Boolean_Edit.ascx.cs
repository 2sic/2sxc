using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace ToSic.Eav.ManagementUI
{
	public partial class Boolean_EditCustom : ManagementUI.FieldTemplateUserControl
	{
        protected global::DotNetNuke.UI.UserControls.LabelControl FieldLabel;

		protected void Page_Load(object sender, EventArgs e)
		{
            CheckBox1.ToolTip = MetaData.ContainsKey("Notes") ? MetaData["Notes"][DimensionIds].ToString() : null;
            FieldLabel.Text = MetaData.ContainsKey("Name") ? MetaData["Name"][DimensionIds].ToString() : Attribute.StaticName;

			if (ShowDataControlOnly)
				FieldLabel.Visible = false;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!IsPostBack)
			{
				DataBind();
			}
            FieldLabel.Text = MetaData.ContainsKey("Name") ? MetaData["Name"][DimensionIds].ToString() : Attribute.StaticName;
            FieldLabel.HelpText = MetaData.ContainsKey("Notes") ? MetaData["Notes"][DimensionIds].ToString() : "";

            bool Value = false;
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
