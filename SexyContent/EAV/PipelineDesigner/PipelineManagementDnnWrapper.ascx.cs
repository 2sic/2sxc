using System;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;

namespace ToSic.SexyContent.EAV.PipelineDesigner
{
	public partial class PipelineManagementDnnWrapper : SexyControlEditBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
		}

		public static string GetEditUrl(PortalModuleBase caller, int appId)
		{
			return caller.EditUrl(caller.PortalSettings.ActiveTab.TabID, SexyContent.ControlKeys.PipelineManagement, true,
					"mid=" + caller.ModuleId + "&ReturnUrl=" + HttpUtility.UrlEncode(caller.Request.RawUrl)) + "&AppId=" + appId;
		}
	}
}