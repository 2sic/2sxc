using System;
using System.Web.UI;
using DotNetNuke.UI.UserControls;

namespace ToSic.Eav.ManagementUI
{
	public partial class DateTime_EditCustom : FieldTemplateUserControl
	{
        protected LabelControl FieldLabel;

        Boolean _useTimePicker;
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
