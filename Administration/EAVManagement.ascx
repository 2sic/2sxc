<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.EAVManagement" Codebehind="EAVManagement.ascx.cs" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>

<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<div class="dnnForm">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelEAVManagement">EAV Management</h2>
    <%-- Optional use the BaseUrl-Property to specify a URL that this Wrapper Module will use --%>
    <asp:Panel runat="server" id="pnlEAV"></asp:Panel>
</div>