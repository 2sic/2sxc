using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;

namespace ToSic.Eav.ManagementUI
{
    public partial class Text_EditCustom : ManagementUI.FieldTemplateUserControl
    {
        protected DotNetNuke.UI.UserControls.LabelControl FieldLabel;
        protected DotNetNuke.UI.UserControls.TextEditor Texteditor1;
        protected DotNetNuke.UI.UserControls.UrlControl DnnUrl1;

        private const string MetaDataDefaultValueKey = "DefaultValue";
        private const string MetaDataInputTypeKey = "InputType";
        private const string MetaDataDrowdownValuesKey = "DropdownValues";
        private const string MetaDataRowCountKey = "RowCount";
        private const string MetaDataIsRequiredKey = "Required";
        private const string MetaDataLengthKey = "Length";
        private const string MetaDataNotesKey = "Notes";
        private const string MetaDataNameKey = "Name";
        private const string MetaDataWysiwygHeightKey = "WysiwygHeight";
        private const string MetaDataWysiwygWidthKey = "WysiwygWidth";
        private const string MetaDataRegularExpression = "ValidationRegEx";
        private const string MetaDataRegularExpressionJavaScript = "ValidationRegExJavaScript";

        private enum InputTypes { Textbox, Wysiwyg, DropDown, Link }

        /// <summary>
        /// Gets the InputType of the current Control.
        /// </summary>
        private InputTypes InputType
        {
            get
            {
                if (ViewState[MetaDataInputTypeKey] == null)
                {
                    switch (GetMetaDataValue(MetaDataInputTypeKey, "").ToLower())
                    {
                        case "wysiwyg":
                            ViewState[MetaDataInputTypeKey] = InputTypes.Wysiwyg;
                            break;
                        case "dropdown":
                            ViewState[MetaDataInputTypeKey] = InputTypes.DropDown;
                            break;
                        case "link":
                            ViewState[MetaDataInputTypeKey] = InputTypes.Link;
                            break;
                        default:
                            ViewState[MetaDataInputTypeKey] = InputTypes.Textbox;
                            break;
                    }
                }

                return (InputTypes)(ViewState[MetaDataInputTypeKey]);
            }
        }

        /// <summary>
        /// Inform if this control was officially initialized. Used for example to preload information (but not on certain postbacks)
        /// Necessary because we can't rely on IsPostback because sometimes our form will load inside a post back
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return (bool)(ViewState["IsInitialized"] ?? false);
            }
            set
            {
                ViewState["IsInitialized"] = value;
            }
        }

        /// <summary>
        /// The code that should run once when a control is loaded
        /// Base code should have "virtual"
        /// Derived classes should have "override"
        /// </summary>
        public virtual void InitializeFieldTemplate()
        {
            string DefaultValue = GetMetaDataValue(MetaDataDefaultValueKey, "");

            // Do things depending which InputType this control has set
            if (InputType == InputTypes.Wysiwyg)
            {
                TextBox1.Visible = false;
                Texteditor1.Visible = true;

                //if (MetaData.ContainsKey(MetaDataWysiwygHeightKey))
                //{
                    var WysiwygHeight = GetMetaDataValue(MetaDataWysiwygHeightKey, new decimal?()); // (((IAttribute<decimal?>)(MetaData[MetaDataWysiwygHeightKey]))).Typed[DimensionIds];
                    if(WysiwygHeight.HasValue)
                        Texteditor1.Height = new Unit(Convert.ToInt32(WysiwygHeight));
                //}
                if (MetaData.ContainsKey(MetaDataWysiwygWidthKey))
                {
                    var WysiwygWidth = (((IAttribute<decimal?>)(MetaData[MetaDataWysiwygWidthKey]))).Typed[DimensionIds];
                    if(WysiwygWidth.HasValue)
                        Texteditor1.Width = new Unit(Convert.ToInt32(WysiwygWidth));
                }
                
                if (FieldValueEditString != null)
                    Texteditor1.Text = FieldValueEditString;
                else
                    Texteditor1.Text = DefaultValue;

                Texteditor1.ChooseMode = false;
            }
            else if(InputType == InputTypes.DropDown)
            {
                TextBox1.Visible = false;
                DropDown1.Visible = true;
                if (!String.IsNullOrEmpty(GetMetaDataValue<string>(MetaDataDrowdownValuesKey)))
                    DropDown1.DataSource = (from c in GetMetaDataValue<string>(MetaDataDrowdownValuesKey).Replace("\r", "").Split('\n')
                                            select new
                                            {
                                                Text = c.Contains(':') ? (c.Split(':'))[0] : c,
                                                Value = c.Contains(':') ? (c.Split(':'))[1] : c
                                            }).ToList();
                DropDown1.DataBind();

                if (!String.IsNullOrEmpty(FieldValueEditString))
                {
                    if (DropDown1.Items.Cast<ListItem>().All(i => i.Value != FieldValueEditString))
                        DropDown1.Items.Insert(0, new ListItem(FieldValueEditString + " (keep old value)", FieldValueEditString));
                    DropDown1.SelectedValue = FieldValueEditString;
                }

                if(FieldValueEditString == null)
                {
                    DropDown1.SelectedValue = DefaultValue;
                }
            }
            else if(InputType == InputTypes.Link)
            {
                TextBox1.Visible = false;
                DnnUrl1.Visible = true;

                DnnUrl1.Url = FieldValueEditString;
            }
            else
            {
                // Row Count of multiline field
                var metaDataRowCount = GetMetaDataValue<decimal?>(MetaDataRowCountKey);
                if (metaDataRowCount.HasValue && metaDataRowCount > 0)
                {
                    TextBox1.Rows = Convert.ToInt32(metaDataRowCount.Value.ToString());
                    if(TextBox1.Rows > 1)
                        TextBox1.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                }

                if (GetMetaDataValue<bool>(MetaDataIsRequiredKey))
                    TextBox1.CssClass += " dnnFormRequired";

                var metaDataLength = GetMetaDataValue<int?>(MetaDataLengthKey);
                if (metaDataLength.HasValue && metaDataLength > 0)
                    TextBox1.MaxLength = metaDataLength.Value;

                TextBox1.ToolTip = GetMetaDataValue<string>(MetaDataNotesKey);

                if (FieldValueEditString != null)
                    TextBox1.Text = FieldValueEditString;
                else
                    TextBox1.Text = DefaultValue;
            }

            if (ShowDataControlOnly)
                FieldLabel.Visible = false;

            valFieldValue.DataBind();

            var metaRegularExpressions = GetMetaDataValue<string>(MetaDataRegularExpression);
            if (!String.IsNullOrEmpty(metaRegularExpressions))
                valRegularExpression.ValidationExpression = metaRegularExpressions;
            valRegularExpression.DataBind();
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            // this should be in the base class, so always used
            if (!IsInitialized)
            {
                InitializeFieldTemplate();
                IsInitialized = true;
            }

            FieldLabel.Text = GetMetaDataValue<string>(MetaDataNameKey, Attribute.StaticName);
            FieldLabel.HelpText = GetMetaDataValue<string>(MetaDataNotesKey);
            valFieldValue.Enabled = GetMetaDataValue<bool>(MetaDataIsRequiredKey);
            valRegularExpression.Enabled = !String.IsNullOrEmpty(GetMetaDataValue<string>(MetaDataRegularExpression));
            
            // base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        public override object Value
        {
            get
            {
                string Value;

                switch (InputType)
                {
                    case InputTypes.Wysiwyg:
                        Value = Texteditor1.Text;
                        break;
                    case InputTypes.DropDown:
                        Value = DropDown1.SelectedValue;
                        break;
                    case InputTypes.Link:
                        if (DnnUrl1.UrlType == "T")
                            Value = Regex.Replace(DotNetNuke.Common.Globals.NavigateURL(int.Parse(DnnUrl1.Url)), "^https?://(.*?)/", "/");
                        else
                            Value = DnnUrl1.Url;
                        break;
                    default:
                        Value = TextBox1.Text;
                        break;
                }

                return Value;
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
