<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebApiHelp.ascx.cs" Inherits="ToSic.SexyContent.Administration.WebApiHelp" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<asp:Panel runat="server" class="dnnForm dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelWebApiHelp">
        <span><%= LocalizeString("Heading.Text") %></span>
    </h2>
    <fieldset>
        <%= LocalizeString("WebApiHelp.Text") %>
        <br/><br/>
        The following table shows the .cs files in the App-API folder:<br/>
        [todo: list here]<br/><br/>
        For a quick start, the following button will auto-generate a simple WebAPI-Controller for you and create the JavaScript code to access the API. <br/>
        [todo:button]
    </fieldset>
</asp:Panel>