<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebApiHelp.ascx.cs" Inherits="ToSic.SexyContent.Administration.WebApiHelp" %>

<asp:Panel runat="server" class="dnnForm dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelWebApiHelp"><%= LocalizeString("Heading.Text") %></h2>
    <fieldset>
        <%= LocalizeString("WebApiHelp.Text") %>
    </fieldset>
</asp:Panel>