<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.EAVManagement" Codebehind="EAVManagement.ascx.cs" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-ui-tree.min.css" Priority="60" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js" Priority="60" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-ui-tree.min.js" Priority="61" />

<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<div class="dnnForm">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelEAVManagement">EAV Management</h2>
    <asp:Panel runat="server" id="pnlEAV"></asp:Panel>
</div>

<ul class="dnnActions">
	<li>
		<a href="<%= EditUrl(TabId, SexyContent.ControlKeys.DataExport, true, "mid=" + ModuleId + "&" +SexyContent.AppIDString + "=" + Request.QueryString[SexyContent.AppIDString]) %>" class="dnnSecondaryAction">Export Data (beta)</a>
	</li>
	<li>
		<a href="<%= EditUrl(TabId, SexyContent.ControlKeys.DataImport, true, "mid=" + ModuleId + "&" + SexyContent.AppIDString + "=" + Request.QueryString[SexyContent.AppIDString]) %>" class="dnnSecondaryAction">Import Data (beta)</a>
	</li>
</ul>