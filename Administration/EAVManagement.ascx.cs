using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav;

namespace ToSic.SexyContent
{
    public partial class EAVManagement : SexyControlAdminBase
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            // Register JavaScripts
            ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn);
            DotNetNuke.Framework.jQuery.RegisterJQuery(this.Page);
            DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration();

            base.Page_Init(sender, e);
        }

        /// <summary>
        /// Initialize EAVManagement Control and add it to the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ManagementMode"] == "NewItem" ||
                Request.QueryString["ManagementMode"] == "EditItem")
            {
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemForm.js", 100);
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavGlobalConfigurationProvider.js", 101);
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavApiService.js", 102);
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavDialogService.js", 103);

                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemFormEntityModelCreator.js", 200);
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/ItemForm.js", 300);
                ClientResourceManager.RegisterStyleSheet(this.Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemForm.css", 150);
            }


            // Register Stylesheet & Script
            ClientResourceManager.RegisterStyleSheet(this.Page, "~/DesktopModules/ToSIC_SexyContent/Styles/Edit.css", 200);

            // Add DNN Version to body class
            SexyContent.AddDNNVersionToBodyClass(this);

            var eavManagement = (ToSic.Eav.ManagementUI.EavManagement)Page.LoadControl(TemplateControl.TemplateSourceDirectory + "/../SexyContent/EAV/Controls/EAVManagement.ascx");
            eavManagement.BaseUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, SexyContent.ControlKeys.EavManagement, "mid=" + ModuleId.ToString() + "&popUp=true&" + "AppID=" + AppId.ToString());
            eavManagement.Scope = SexyContent.AttributeSetScope;
            eavManagement.AssignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDDefault;
            eavManagement.DefaultCultureDimension = SexyContent.GetLanguageId(ZoneId.Value, PortalSettings.DefaultLanguage);
            eavManagement.ZoneId = ZoneId;
            eavManagement.AppId = AppId;
            eavManagement.AddFormClientScriptAndCss = false;
            eavManagement.EntityDeleting += EavManagementEntityDeleting;
            pnlEAV.Controls.Add(eavManagement);
        }

        protected void EavManagementEntityDeleting(EntityDeletingEventArgs e)
        {
            if (!Sexy.CanDeleteEntity(e.EntityId))
            {
                e.Cancel = true;
                e.CancelMessage += " This entity cannot be deleted because it is in use (2SexyContent).";
            }
        }
    }
}