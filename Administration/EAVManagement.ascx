<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.EAVManagement" Codebehind="EAVManagement.ascx.cs" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-ui-tree.min.css" Priority="60" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js" Priority="60" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-ui-tree.min.js" Priority="61" />

<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<div class="dnnForm">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelEAVManagement">EAV Management</h2>
    <%-- Optional use the BaseUrl-Property to specify a URL that this Wrapper Module will use --%>
    <asp:Panel runat="server" id="pnlEAV"></asp:Panel>
</div>