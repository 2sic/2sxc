using DotNetNuke.Entities.Modules;
using System;

namespace ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms
{
	public partial class Wysiwyg : PortalModuleBase
	{
		public new int PortalId { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Texteditor1.ChooseMode = false;
		}
	}
}