<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.View" Codebehind="View.ascx.cs" %>
<asp:Placeholder runat="server" ID="pnlTemplateChooser" Visible="false" EnableViewState="False">
    
<%--    <div sxc-app="2sxc.view" id="tselector<%= ModuleId %>" ng-controller="TemplateSelectorCtrl as vm" 
        class="sc-selector-wrapper" 
        ng-include="'template-selector/template-selector.html'"
        <%-- note that the importappdialog is only needed, till import-app works in angular-only --%
        data-importAppDialog="<%= EditUrl("", "", "appimport") %>"
        > 
    </div>--%>
</asp:Placeholder>

<asp:Panel runat="server" ID="pnlError" CssClass="dnnFormMessage dnnFormWarning" Visible="false"></asp:Panel>

<div class="sc-viewport">
    <asp:PlaceHolder runat="server" ID="phOutput"></asp:PlaceHolder>
</div>