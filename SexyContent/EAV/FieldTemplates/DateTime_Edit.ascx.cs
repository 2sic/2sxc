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
            TimePicker1.Visible = _useTimePicker;

            Calendar1.ToolTip = GetMetaDataValue<string>("Notes");

			if (ShowDataControlOnly)
				FieldLabel.Visible = false;

            if (GetMetaDataValue<bool>("Required"))
            {
                Calendar1.CssClass += " dnnFormRequired";
                TimePicker1.CssClass += " dnnFormRequired";
            }
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!IsPostBack)
			{
				
                if (((DateTime?)FieldValue).HasValue)
                {
                    Calendar1.SelectedDate = ((DateTime?)FieldValue).Value;
                    TimePicker1.SelectedDate = ((DateTime?)FieldValue).Value;
                }

				Calendar1.DataBind();
                if(_useTimePicker)
                    TimePicker1.DataBind();
			}
            FieldLabel.Text = GetMetaDataValue("Name", Attribute.StaticName);
            FieldLabel.HelpText = GetMetaDataValue<string>("Notes");
            valCalendar1.Enabled = GetMetaDataValue<bool>("Required");

			base.OnPreRender(e);
		}

	    public override object Value
	    {
	        get
	        {
                DateTime? SelectedDate = null;
                if (Calendar1.SelectedDate.HasValue)
                    SelectedDate = Calendar1.SelectedDate.Value;
                if (_useTimePicker)
                {
                    if (TimePicker1.SelectedDate.HasValue)
                        SelectedDate += TimePicker1.SelectedDate.Value.TimeOfDay;
                }

                if (SelectedDate.HasValue)
                    return SelectedDate.Value;
                else
                    return "";
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
