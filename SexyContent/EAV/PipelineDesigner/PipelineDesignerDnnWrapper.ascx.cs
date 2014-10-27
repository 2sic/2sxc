using System;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.UI.Modules;

namespace ToSic.SexyContent.EAV.PipelineDesigner
{
	public partial class PipelineDesignerDnnWrapper : PortalModuleBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Register2SxcGlobals(this, Request, int.Parse(Request.QueryString["AppId"]));
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
		}


		// ToDo: Refactor, move to correct Class
		public static void Register2SxcGlobals(PortalModuleBase module, HttpRequest request, int appId)
		{
			// Add some required variables to module host div
			((ModuleHost)module.Parent).Attributes.Add("data-2sxc-globals", (new
			{
				ModuleContext = new
				{
					module.ModuleContext.PortalId,
					module.ModuleContext.TabId,
					module.ModuleContext.ModuleId,
					AppId = appId
				},
				ApplicationPath = (request.IsSecureConnection ? "https://" : "http://") + module.PortalAlias.HTTPAlias + "/",
				module.PortalSettings.ActiveTab.FullUrl
			}).ToJson());
		}
	}
}