using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;

namespace ToSic.Eav.ManagementUI
{
    public partial class Number_EditCustom : ManagementUI.FieldTemplateUserControl
    {
        protected global::DotNetNuke.UI.UserControls.LabelControl FieldLabel;

        // Meta Data Keys
        private string MetaDataDefaultValueKey = "DefaultValue";
        private string MetaDataIsRequiredKey = "IsRequired";
        private string MetaDataLengthKey = "Length";
        private string MetaDataNotesKey = "Notes";
        private string MetaDataNameKey = "Name";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (FieldValueEditString == null)
                TextBox1.Text = GetMetaDataValue<string>(MetaDataDefaultValueKey, "");
            else
                TextBox1.Text = FieldValueEditString;

            if (GetMetaDataValue<bool>(MetaDataIsRequiredKey))
                TextBox1.CssClass += " dnnFormRequired";

            if (!String.IsNullOrEmpty(GetMetaDataValue<string>(MetaDataLengthKey)))
                TextBox1.MaxLength = GetMetaDataValue<int>(MetaDataLengthKey);

            TextBox1.ToolTip = GetMetaDataValue<string>(MetaDataNotesKey, "");

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
            get { return TextBox1.Text; }
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
