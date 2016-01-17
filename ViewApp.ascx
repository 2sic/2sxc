<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.View" Codebehind="View.ascx.cs" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<asp:Placeholder runat="server" ID="pnlTemplateChooser" Visible="false" EnableViewState="False">
    
    <%-- todo:2rm - auto-select template doesn't quite work when not inline-template --%>
    <%-- note: 95% identical with ViewApp.ascx - merge as soon as possible --%>
    <div sxc-app="2sxc.view" id="tselector<%= ModuleId %>" ng-controller="TemplateSelectorCtrl as vm" 
        data-moduleid="<%= ModuleId %>" class="sc-selector-wrapper" 
        ng-include="'template-selector/template-selector.html'" 
        data-importAppDialog="<%= EditUrl("", "", SexyContent.ControlKeys.AppImport) %>"
        >
    </div>
</asp:Placeholder>

<asp:Panel runat="server" Visible="False" class="dnnFormMessage dnnFormInfo" ID="pnlGetStarted"></asp:Panel>

<asp:Panel runat="server" ID="pnlZoneConfigurationMissing" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" ID="lblMissingZoneConfiguration" ResourceKey="ZoneConfigurationMissing"></asp:Label>
    <asp:HyperLink runat="server" ID="hlkConfigureZone" 
        CssClass="dnnSecondaryAction" ResourceKey="hlkConfigureZone"></asp:HyperLink>
</asp:Panel>

<asp:Panel runat="server" ID="pnlError" CssClass="dnnFormMessage dnnFormWarning" Visible="false"></asp:Panel>
<asp:Panel runat="server" ID="pnlMessage" CssClass="dnnFormMessage dnnFormInfo" Visible="false"></asp:Panel>

<div class="sc-viewport">
    <asp:PlaceHolder runat="server" ID="phOutput"></asp:PlaceHolder>
</div>