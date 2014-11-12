using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace ToSic.Eav.ManagementUI
{
	public partial class DateTime_EditCustom : ManagementUI.FieldTemplateUserControl
	{
        protected global::DotNetNuke.UI.UserControls.LabelControl FieldLabel;

        Boolean _useTimePicker = false;
		protected void Page_Load(object sender, EventArgs e)
		{
            _useTimePicker = GetMetaDataValue<bool>("UseTimePicker");
            DateTimePicker.Visible = _useTimePicker;
            Calendar1.Visible = !_useTimePicker;

            Calendar1.ToolTip = GetMetaDataValue<string>("Notes");
            DateTimePicker.ToolTip = GetMetaDataValue<string>("Notes");

			if (ShowDataControlOnly)
				FieldLabel.Visible = false;

            if (GetMetaDataValue<bool>("Required"))
            {
                Calendar1.CssClass += " dnnFormRequired";
                DateTimePicker.CssClass += " dnnFormRequired";
            }
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!IsPostBack)
			{
				
                if (((DateTime?)FieldValue).HasValue)
                {
                    Calendar1.SelectedDate = ((DateTime?)FieldValue).Value;
                    DateTimePicker.SelectedDate = ((DateTime?)FieldValue).Value;
                }

                if(!_useTimePicker)
				    Calendar1.DataBind();
                else
                    DateTimePicker.DataBind();
			}
            FieldLabel.Text = GetMetaDataValue("Name", Attribute.StaticName);
            FieldLabel.HelpText = GetMetaDataValue<string>("Notes");
            valCalendar1.Enabled = GetMetaDataValue<bool>("Required");
            valDateTimePicker.Enabled = GetMetaDataValue<bool>("Required");

			base.OnPreRender(e);
		}

	    public override object Value
	    {
	        get
	        {
                if (!_useTimePicker)
                    return Calendar1.SelectedDate;
                else
                    return DateTimePicker.SelectedDate;
	        }
	    }


	    public override Control DataControl
		{
			get
			{
				return Calendar1;
			}
		}

	}
}
