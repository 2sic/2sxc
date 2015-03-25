using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Modules;
using ToSic.Eav;
using ToSic.SexyContent;
using DotNetNuke.Entities.Modules;

namespace ToSic.SexyContent
{
	public partial class ContentTypeAndDemoSelector : PortalModuleBase
	{

		public bool ContentTypeRequired { get; set; }
		public bool EnableNoContentTypeOption { get; set; }

		public int? _ContentTypeID;
		public int? ContentTypeID
		{
			get { return int.Parse(ddlContentTypes.SelectedValue); }
			set { _ContentTypeID = value; }
		}

		public int? _DemoEntityID;
		public int? DemoEntityID
		{
			get
			{
				return Convert.ToInt32(ddlDemoRows.SelectedValue) > 0 ? Convert.ToInt32(ddlDemoRows.SelectedValue) : new int?();
			}
			set
			{
				_DemoEntityID = value;
			}
		}

		public string ItemType { get; set; }

		private bool _Enabled { get; set; }
		public bool Enabled
		{
			set
			{
				_Enabled = value;
				ddlContentTypes.Enabled = value;
				//ddlDemoRows.Enabled = value;
				pnlLocked.Visible = !value;
			}
		}

		public int ZoneId { get; set; }
		public int AppId { get; set; }

		private SexyContent _sexy;
		public SexyContent Sexy
		{
			get
			{
				if (_sexy == null)
				{
					if (ZoneId == 0 || AppId == 0)
						throw new ArgumentNullException("ZoneId and AppId must be set.");
					_sexy = new SexyContent(ZoneId, AppId);
				}
				return _sexy;
			}
		}
		private SexyContent _sexyUncached;
		public SexyContent SexyUncached
		{
			get
			{
				if (_sexyUncached == null)
					_sexyUncached = new SexyContent(ZoneId, AppId, false);
				return _sexyUncached;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Make sure the correct Resource file is loaded
			LocalResourceFile = LocalResourceFile.Replace(ID, "ContentTypeAndDemoSelector");

			// Set localized Error Messages for validator
			valContentType.ErrorMessage = LocalizeString("valContentType.ErrorMessage");
			valContentType.Enabled = ContentTypeRequired;
			((DotNetNuke.UI.UserControls.LabelControl)lblContentType).Text = LocalizeString("lblContentType" + ItemType + ".Text");
			((DotNetNuke.UI.UserControls.LabelControl)lblDemoRow).Text = LocalizeString("lblDemoRow" + ItemType + ".Text");

			if (!IsPostBack)
			{
				if (EnableNoContentTypeOption)
					ddlContentTypes.Items.Add(new ListItem("< no ContentType >", "-1"));

				// DataBind Content Types
				var AttributeSets = Sexy.GetAvailableAttributeSets(SexyContent.AttributeSetScope);
				ddlContentTypes.DataSource = AttributeSets;

				if (AttributeSets.Any(a => a.AttributeSetId == _ContentTypeID))
					ddlContentTypes.SelectedValue = _ContentTypeID.ToString();

				ddlContentTypes.DataBind();

				BindDemoContentDropdown(Convert.ToInt32(ddlContentTypes.SelectedValue), ddlDemoRows);
				if (_DemoEntityID.HasValue && _DemoEntityID != 0 && ddlDemoRows.Items.FindByValue(_DemoEntityID.Value.ToString()) != null)
					ddlDemoRows.SelectedValue = _DemoEntityID.Value.ToString();
			}
		}

		/// <summary>
		/// Bind democontent dropdown if the content type dropdown changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ddlContentTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindDemoContentDropdown(Convert.ToInt32(ddlContentTypes.SelectedValue), ddlDemoRows);
		}

		/// <summary>
		/// Bind the demo dropdown
		/// </summary>
		/// <param name="ContentType"></param>
		/// <param name="DemoContentChooser"></param>
		protected void BindDemoContentDropdown(int ContentType, DropDownList DemoContentChooser)
        {
            if (ContentType > 0)
            {
                DemoContentChooser.Items.Clear();
                DemoContentChooser.Items.Add(new ListItem(LocalizeString("ddlDemoRowsDefaultItem.Text"), "0"));

	            var dataSource = DataSource.GetInitialDataSource(ZoneId, AppId);
	            var data = dataSource.List.Where(p => p.Value.Type.AttributeSetId == ContentType).Select(p => new { p.Value.EntityId, EntityTitle = p.Value.Title != null ? p.Value.Title[CultureInfo.CurrentCulture.Name].ToString() : "(no title)" });
					
				DemoContentChooser.DataSource = data;
                DemoContentChooser.Enabled = true;
                DemoContentChooser.DataBind();
            }
            else
            {
                DemoContentChooser.Enabled = false;
            }
        }
	}
}