using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;

namespace ToSic.SexyContent
{
    public partial class EAVManagement : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        SexyContent Sexy = new SexyContent();

        protected void Page_Init(object sender, EventArgs e)
        {
            // Register JavaScripts
            ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn);
            DotNetNuke.Framework.jQuery.RegisterJQuery(this.Page);
            DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration();
        }

        /// <summary>
        /// Initialize EAVManagement Control and add it to the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Register Stylesheet & Script
            litAssetsInclude.Text = "<link rel='stylesheet' href='" + ResolveClientUrl("../Styles/Edit.css") + "'/>";

            if (Request.QueryString["ManagementMode"] == "NewItem" || Request.QueryString["ManagementMode"] == "EditItem")
                litAssetsInclude.Text += "<script src='" + ResolveClientUrl("../Js/ItemForm.js") + "' type='text/javascript' />";

            // Add DNN Version to body class
            Sexy.AddDNNVersionToBodyClass(this);

            var appId = (Request.QueryString.AllKeys.Contains("AppID")
                             ? int.Parse(Request.QueryString["AppID"])
                             : Sexy.GetDefaultAppID());

            var eavManagement = (ToSic.Eav.ManagementUI.EavManagement)Page.LoadControl(TemplateControl.TemplateSourceDirectory + "/../SexyContent/EAV/Controls/EAVManagement.ascx");
            eavManagement.BaseUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, SexyContent.ControlKeys.EavManagement, "mid=" + ModuleId.ToString() + "&popUp=true&" + "AppID=" + appId.ToString());
            eavManagement.Scope = SexyContent.AttributeSetScope;
            eavManagement.AssignmentObjectTypeId = Sexy.DefaultAssignmentObjectTypeID;
            eavManagement.DefaultCultureDimension = Sexy.ContentContext.GetLanguageId(PortalSettings.DefaultLanguage);
            eavManagement.ZoneId = Sexy.GetZoneID(PortalId);
            eavManagement.AppId = appId;
            pnlEAV.Controls.Add(eavManagement);
        }
    }
}