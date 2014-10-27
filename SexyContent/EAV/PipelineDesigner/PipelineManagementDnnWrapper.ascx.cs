using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;

namespace ToSic.SexyContent.EAV.PipelineDesigner
{
	public partial class PipelineManagementDnnWrapper : PortalModuleBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			PipelineDesignerDnnWrapper.Register2SxcGlobals(this, Request, int.Parse(Request.QueryString["AppId"]));
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

		}
	}
}