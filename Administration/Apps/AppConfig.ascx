<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppConfig.ascx.cs" Inherits="ToSic.SexyContent.Administration.AppConfig" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<%@ Import Namespace="ToSic.SexyContent.EAV.PipelineDesigner" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="../Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<div class="dnnForm dnnSexyContentAppConfig dnnClear">
    <h2 class="dnnFormSectionHead">
        <span><%= LocalizeString("AppConfig.MainTitle") %></span>
    </h2>
    <%= LocalizeString("AppConfig.Intro") %>
    
    <div>
        <h5>App Settings</h5>
        <p>Settings are configurations used by the app - like SQL-connection strings, default "items-to-show" numbers and things like that. They can also be multi-language, so that a setting (like default RSS-Feed) could be different in each language.</p>
        <p>
            <a class="dnnPrimaryAction" href="<%= Sexy.GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameAppSettings, SexyContent.AssignmentObjectTypeIDSexyContentApp, AppId.Value) %>">
                Edit App Settings
            </a>
            <a title="configure which settings exist for this App" class="dnnSecondaryAction" href="<%= EditUrl("", "", SexyContent.ControlKeys.EavManagement, SexyContent.AppIDString + "=" + AppId.Value + "&ManagementMode=ContentTypeFields&AttributeSetId=" + SexyContent.GetAppSettingsAttributeSetId(ZoneId.Value, AppId.Value)) %>">
                Configure App Settings
            </a>
        </p>
    </div>
    <div>
        <h5>App Resources</h5>
        <p>Resources are used for labels and things like that in the App. They are usually needed to create multi-lingual views and such, and should not be used for App-Settings.</p>
        <p>
            <a class="dnnPrimaryAction" href="<%= Sexy.GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameAppResources, SexyContent.AssignmentObjectTypeIDSexyContentApp, AppId.Value) %>">
                Edit App Resources
            </a>
            <a title="configure which resources exist for this App" class="dnnSecondaryAction" href="<%= EditUrl("", "", SexyContent.ControlKeys.EavManagement, SexyContent.AppIDString + "=" + AppId.Value + "&ManagementMode=ContentTypeFields&AttributeSetId=" + SexyContent.GetAppResourcesAttributeSetId(ZoneId.Value, AppId.Value)) %>">
                Configure App Resources
            </a>
        </p>
    </div>
    <div>
        <h5>Other actions</h5>
        <p>
            <a class="dnnSecondaryAction" href="<%= Sexy.GetMetaDataEditUrl(TabId, ModuleId, Request.RawUrl, this, SexyContent.AttributeSetStaticNameApps, SexyContent.AssignmentObjectTypeIDSexyContentApp, AppId.Value) %>">Edit App Definition</a>
            <a class="dnnSecondaryAction" href="<%= EditUrl("", "", SexyContent.ControlKeys.AppExport, SexyContent.AppIDString + "=" + AppId.Value) %>">Export this App</a>
        </p>
    </div>
</div>

<style>
    .dnnSexyContentAppConfig h5 { margin-bottom:5px; }
</style>
