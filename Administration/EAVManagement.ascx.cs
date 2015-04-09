using System;
using DotNetNuke.Framework;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav;
using ToSic.Eav.ManagementUI;
using Globals = DotNetNuke.Common.Globals;

namespace ToSic.SexyContent
{
    public partial class EAVManagement : SexyControlAdminBase
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            // Register JavaScripts
            ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
            jQuery.RegisterJQuery(Page);
            jQuery.RequestDnnPluginsRegistration();

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
                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemForm.js", 100);
                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavGlobalConfigurationProvider.js", 101);
                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavApiService.js", 102);
                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavDialogService.js", 103);

                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemFormEntityModelCreator.js", 200);
                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/ToSIC_SexyContent/Js/ItemForm.js", 300);
                ClientResourceManager.RegisterStyleSheet(Page, "~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Controls/ItemForm.css", 150);
            }


            // Register Stylesheet & Script
            ClientResourceManager.RegisterStyleSheet(Page, "~/DesktopModules/ToSIC_SexyContent/Styles/Edit.css", 200);

            // Add DNN Version to body class
            SexyContent.AddDNNVersionToBodyClass(this);

            var eavManagement = (EavManagement)Page.LoadControl(TemplateControl.TemplateSourceDirectory + "/../SexyContent/EAV/Controls/EAVManagement.ascx");
            eavManagement.BaseUrl = Globals.NavigateURL(TabId, SexyContent.ControlKeys.EavManagement, "mid=" + ModuleId + "&popUp=true&" + "AppID=" + AppId);
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
			// This code is not needed anymore because EAV has the list management now.
			//if (!Sexy.CanDeleteEntity(e.EntityId))
			//{
			//	e.Cancel = true;
			//	e.CancelMessage += " This entity cannot be deleted because it is in use (2sxc).";
			//}
        }
    }
}