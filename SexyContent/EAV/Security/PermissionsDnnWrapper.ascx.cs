using System;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;

namespace ToSic.SexyContent.EAV.Security
{
	public partial class PermissionsDnnWrapper : SexyControlEditBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
		}
	}
}