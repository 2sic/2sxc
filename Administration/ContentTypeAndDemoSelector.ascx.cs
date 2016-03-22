using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;

namespace ToSic.SexyContent
{
	public partial class ContentTypeAndDemoSelector : PortalModuleBase
	{

		public bool ContentTypeRequired { get; set; }
		public bool EnableNoContentTypeOption { get; set; }

		public string _ContentTypeStaticName;
		public string ContentTypeStaticName
		{
			get { return ddlContentTypes.SelectedValue; }
			set { _ContentTypeStaticName = value; }
		}

		public int? _DemoEntityID;
		public int? DemoEntityID
		{
			get
			{
				return ddlDemoRows.SelectedValue != "" && Convert.ToInt32(ddlDemoRows.SelectedValue) > 0 ? Convert.ToInt32(ddlDemoRows.SelectedValue) : new int?();
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

		private SxcInstance _sexy;
		public SxcInstance Sexy
		{
			get
			{
				if (_sexy == null)
				{
					if (ZoneId == 0 || AppId == 0)
						throw new ArgumentNullException("ZoneId and AppId must be set.");
					_sexy = new SxcInstance(ZoneId, AppId);
				}
				return _sexy;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Make sure the correct Resource file is loaded
			LocalResourceFile = LocalResourceFile.Replace(ID, "ContentTypeAndDemoSelector");

			// Set localized Error Messages for validator
			valContentType.ErrorMessage = LocalizeString("valContentType.ErrorMessage");
			valContentType.Enabled = ContentTypeRequired;
			lblContentType.Text = LocalizeString("lblContentType" + ItemType + ".Text");
			lblDemoRow.Text = LocalizeString("lblDemoRow" + ItemType + ".Text");

			if (!IsPostBack)
			{
				if (EnableNoContentTypeOption)
					ddlContentTypes.Items.Add(new ListItem("< no ContentType >", "-1"));

				// DataBind Content Types
				var AttributeSets = Sexy.AppTemplates.GetAvailableContentTypes(ToSic.SexyContent.Settings.AttributeSetScope);
				ddlContentTypes.DataSource = AttributeSets;

				if (AttributeSets.Any(a => a.StaticName == _ContentTypeStaticName))
					ddlContentTypes.SelectedValue = _ContentTypeStaticName;

				ddlContentTypes.DataBind();

				BindDemoContentDropdown(ddlContentTypes.SelectedValue, ddlDemoRows);
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
			BindDemoContentDropdown(ddlContentTypes.SelectedValue, ddlDemoRows);
		}

		/// <summary>
		/// Bind the demo dropdown
		/// </summary>
		/// <param name="contentType"></param>
		/// <param name="demoContentChooser"></param>
		protected void BindDemoContentDropdown(string contentType, DropDownList demoContentChooser)
        {
            if (!String.IsNullOrEmpty(contentType))
            {
                demoContentChooser.Items.Clear();
                demoContentChooser.Items.Add(new ListItem(LocalizeString("ddlDemoRowsDefaultItem.Text"), ""));

	            var dataSource = DataSource.GetInitialDataSource(ZoneId, AppId);
	            var data = dataSource.List.Where(p => p.Value.Type.StaticName == contentType).Select(p => new { p.Value.EntityId, EntityTitle = p.Value.Title != null && p.Value.Title[CultureInfo.CurrentCulture.Name] != null ? p.Value.Title[CultureInfo.CurrentCulture.Name].ToString() : "(no title)" });
					
				demoContentChooser.DataSource = data;
                demoContentChooser.Enabled = true;
                demoContentChooser.DataBind();
            }
            else
            {
                demoContentChooser.Enabled = false;
            }
        }
	}
}