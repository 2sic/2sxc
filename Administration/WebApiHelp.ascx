<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebApiHelp.ascx.cs" Inherits="ToSic.SexyContent.Administration.WebApiHelp" %>
<%@ Import Namespace="System.IO" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<asp:Panel runat="server" class="dnnForm dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelWebApiHelp">
        <span><%= LocalizeString("Heading.Text") %></span>
    </h2>
    <fieldset>
        <%= LocalizeString("WebApiHelp.Text") %>
        The following list shows the .cs files in the App-API folder:<br/>
        <% if (Directory.Exists(Path.Combine(this.Sexy.App.PhysicalPath, "Api"))) { %>
        <ul>
            <% foreach (var file in Directory.GetFiles(Path.Combine(this.Sexy.App.PhysicalPath, "Api"))) { %>
            <li>
                <%= Path.GetFileName(file) %>
            </li>
            <% } %>
        </ul>
        <% } else { %>
            <p>(the directory does not exist)</p>
        <% } %>

        <br/><br/>
        <hr/>
        <p>For a quick start, we recommend that you install the WebApi demo-app. It contains some WebAPI controllers with various actions and some example views to use these controllers. Download the App here in the App-Catalog.</p>
        <a href="http://2sxc.org/en/Apps/Details?AppGuid=40a89dd5-30af-43fe-bc92-00b3bf3d09ab" target="_blank" class="dnnPrimaryAction">Download WebApi Demo</a>
    </fieldset>
</asp:Panel>