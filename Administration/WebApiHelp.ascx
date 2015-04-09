<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebApiHelp.ascx.cs" Inherits="ToSic.SexyContent.Administration.WebApiHelp" %>
<%@ Import Namespace="ToSic.SexyContent.EAV.PipelineDesigner" %>
<%@ Import Namespace="System.IO" %>
<%@ Register TagName="AdministrationRegisters" TagPrefix="SexyContent" Src="Registers.ascx" %>
<SexyContent:AdministrationRegisters runat="server"></SexyContent:AdministrationRegisters>

<asp:Panel runat="server" class="dnnForm dnnClear">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelWebApiHelp">
        <span><%= LocalizeString("Heading.Text") %></span>
    </h2>
    <fieldset>
        <%= LocalizeString("SxcDataHelp.Text") %><br /><br/>
		<%= LocalizeString("VisualDataPipelines.Text") %><br/><br/>
		<a href="<%= PipelineManagementDnnWrapper.GetEditUrl(this, AppId.Value) %>" class="dnnPrimaryAction">Visual Pipeline Designer</a>
		<br/><br/>
	    <%= LocalizeString("WebApi.Text") %><br/><br/>
        The following list shows the .cs files in the App-API folder:<br/>
        <% if (Directory.Exists(Path.Combine(Sexy.App.PhysicalPath, "Api"))) { %>
        <ul>
            <% foreach (var file in Directory.GetFiles(Path.Combine(Sexy.App.PhysicalPath, "Api"), "*.cs")) { %>
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
        <a href="http://2sxc.org/en/Apps/tag/WebApi" target="_blank" class="dnnPrimaryAction">Download WebApi Demo</a>
    </fieldset>
</asp:Panel>