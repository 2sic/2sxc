using System;
using DotNetNuke.Entities.Modules;

namespace ToSic.SexyContent
{
    public partial class EditDataSource : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Module ctl will go to the settings dialog
            lnkBack.NavigateUrl = EditUrl(TabId, "Module", false, "ModuleId", ModuleId.ToString());
        }
    }
}