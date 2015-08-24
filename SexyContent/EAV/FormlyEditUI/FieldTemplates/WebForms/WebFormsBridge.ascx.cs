using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;

namespace ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms
{
	/// <summary>
	/// This control is a bridge for DNN components that do not work in plain JavaScript yet (File Picker, Page Picker, etc.)
	/// </summary>
	public partial class WebFormsBridge : PortalModuleBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			
			switch (Request.QueryString["type"].ToLower())
			{
				case "pagepicker":
					var controlPagePicker = (PagePicker)LoadControl("~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/FieldTemplates/WebForms/PagePicker.ascx");
					controlPagePicker.PortalId = this.PortalId;
					pnlBridgeContent.Controls.Add(controlPagePicker);
					break;
				case "wysiwyg":
					var controlWysiwyg = (Wysiwyg)LoadControl("~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/FieldTemplates/WebForms/Wysiwyg.ascx");
					controlWysiwyg.PortalId = this.PortalId;
					pnlBridgeContent.Controls.Add(controlWysiwyg);
					break;
			}

			JavaScript.RequestRegistration(CommonJs.DnnPlugins);
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
		}
	}
}