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
			Register2sxcGlobals(this, Request);
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
		}


		private static void Register2sxcGlobals(PortalModuleBase module, HttpRequest request)
		{
			// Add some required variables to module host div
			((ModuleHost)module.Parent).Attributes.Add("data-2sxc-globals", (new
			{
				ModuleContext = new
				{
					module.ModuleContext.PortalId,
					module.ModuleContext.TabId,
					module.ModuleContext.ModuleId
				},
				ApplicationPath = (request.IsSecureConnection ? "https://" : "http://") + module.PortalAlias.HTTPAlias + "/"
			}).ToJson());
		}
	}
}