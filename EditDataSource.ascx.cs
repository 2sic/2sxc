using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToSic.SexyContent
{
    public partial class EditDataSource : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Module ctl will go to the settings dialog
            lnkBack.NavigateUrl = EditUrl(TabId, "Module", false, "ModuleId", ModuleId.ToString());
        }
    }
}