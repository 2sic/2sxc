using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms
{
	public partial class PagePicker : System.Web.UI.UserControl
	{
		public int PortalId { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			ctlPagePicker.PortalId = PortalId;
			ctlPagePicker.OnClientSelectionChanged.Add("SelectionChanged");
		}
	}
}