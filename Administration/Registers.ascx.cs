using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToSic.SexyContent.Administration
{
    public partial class Registers : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Correct Local Resource File
            var ResourceFile = DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/ToSIC_SexyContent/Administration/App_LocalResources/Registers.ascx.resx");

            var ParentModule = (SexyControlAdminBase)Parent;

            var Registers = new List<string>();
            Registers.Add(SexyContent.ControlKeys.GettingStarted);

            // Add Buttons if ZoneID is set
            if (SexyContent.GetZoneID(ParentModule.PortalId).HasValue)
            {
                Registers.Add(SexyContent.ControlKeys.EavManagement);
                Registers.Add(SexyContent.ControlKeys.ManageTemplates);

                if(!ParentModule.IsContentApp)
                    Registers.Add(SexyContent.ControlKeys.WebApiHelp);

                Registers.Add(SexyContent.ControlKeys.Import);
            }

            if (ParentModule.IsContentApp)
                Registers.Add(SexyContent.ControlKeys.PortalConfiguration);
            else
                Registers.Add(SexyContent.ControlKeys.AppConfig);

            rptRegisters.DataSource = from c in Registers
                                      select new {
                                        Name = DotNetNuke.Services.Localization.Localization.GetString(c + ".Text",  ResourceFile),
                                        Key = c,
                                        Url = ParentModule.EditUrl(ParentModule.TabId, c, true, "mid=" + ParentModule.ModuleId +
                                            (String.IsNullOrEmpty(Request.QueryString[SexyContent.AppIDString]) ? "" : "&" + SexyContent.AppIDString + "=" + Request.QueryString[SexyContent.AppIDString])),
                                        Active = Request.QueryString["ctl"].ToLower() == c.ToLower()
                                      };
            rptRegisters.DataBind();
        }
    }
}