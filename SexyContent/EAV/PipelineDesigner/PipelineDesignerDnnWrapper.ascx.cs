using System;
using DotNetNuke.Framework;

namespace ToSic.SexyContent.EAV.PipelineDesigner
{
	public partial class PipelineDesignerDnnWrapper : SexyControlEditBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
		}
	}
}