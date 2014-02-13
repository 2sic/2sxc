using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace ToSic.Eav.ManagementUI
{
	public partial class DateTime_EditCustom : ManagementUI.FieldTemplateUserControl
	{
        protected global::DotNetNuke.UI.UserControls.LabelControl FieldLabel;

        Boolean UseTimePicker = false;
		protected void Page_Load(object sender, EventArgs e)
		{
            if (MetaData.ContainsKey("UseTimePicker") && ((IAttribute<bool?>)MetaData["UseTimePicker"]).Typed[DimensionIds].Value)
                UseTimePicker = true;

            TimePicker1.Visible = UseTimePicker;

            Calendar1.ToolTip = MetaData.ContainsKey("Notes") ? MetaData["Notes"][DimensionIds].ToString() : null;

			if (ShowDataControlOnly)
				FieldLabel.Visible = false;

            if (MetaData.ContainsKey("IsRequired") && ((IAttribute<bool?>)MetaData["IsRequired"]).Typed[DimensionIds].Value)
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
                if(UseTimePicker)
                    TimePicker1.DataBind();
			}
            FieldLabel.Text = MetaData.ContainsKey("Name") ? MetaData["Name"][DimensionIds].ToString() : Attribute.StaticName;
            FieldLabel.HelpText = MetaData.ContainsKey("Notes") ? MetaData["Notes"][DimensionIds].ToString() : "";
            valCalendar1.Enabled = MetaData.ContainsKey("IsRequired") && ((IAttribute<bool?>)MetaData["IsRequired"]).Typed[DimensionIds].Value;

			base.OnPreRender(e);
		}

	    public override object Value
	    {
	        get
	        {
                DateTime? SelectedDate = null;
                if (Calendar1.SelectedDate.HasValue)
                    SelectedDate = Calendar1.SelectedDate.Value;
                if (UseTimePicker)
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
