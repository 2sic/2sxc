using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;

namespace ToSic.SexyContent.EAV.PipelineDesigner
{
	public partial class PipelineDesignerDnnWrapper : PortalModuleBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
		}
	}
}