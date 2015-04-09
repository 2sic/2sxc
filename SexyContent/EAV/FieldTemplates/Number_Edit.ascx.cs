using System;
using System.Web.UI;
using DotNetNuke.UI.UserControls;

namespace ToSic.Eav.ManagementUI
{
    public partial class Number_EditCustom : FieldTemplateUserControl
    {
        protected LabelControl FieldLabel;

        // Meta Data Keys
        private string MetaDataDefaultValueKey = "DefaultValue";
        private string MetaDataIsRequiredKey = "Required";
        private string MetaDataLengthKey = "Length";
        private string MetaDataNotesKey = "Notes";
        private string MetaDataNameKey = "Name";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (FieldValueEditString == null)
                TextBox1.Text = GetMetaDataValue(MetaDataDefaultValueKey, "");
            else
                TextBox1.Text = (FieldValue as decimal?).ToString();

            if (GetMetaDataValue<bool>(MetaDataIsRequiredKey))
                TextBox1.CssClass += " dnnFormRequired";

            if (!String.IsNullOrEmpty(GetMetaDataValue<string>(MetaDataLengthKey)))
                TextBox1.MaxLength = GetMetaDataValue<int>(MetaDataLengthKey);

            TextBox1.ToolTip = GetMetaDataValue(MetaDataNotesKey, "");

            if (ShowDataControlOnly)
                FieldLabel.Visible = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }

            FieldLabel.Text = GetMetaDataValue(MetaDataNameKey, Attribute.StaticName);
            FieldLabel.HelpText = GetMetaDataValue<string>(MetaDataNotesKey);
            valFieldValue.Enabled = GetMetaDataValue<bool>(MetaDataIsRequiredKey);
            base.OnPreRender(e);
        }

        public override object Value
        {
            get {
                if(!String.IsNullOrEmpty(TextBox1.Text))
                {
                    return decimal.Parse(TextBox1.Text);
                }
                return new decimal?();

            }
        }


        public override Control DataControl
        {
            get
            {
                return TextBox1;
            }
        }

    }
}
