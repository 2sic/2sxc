<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppExport.ascx.cs" Inherits="ToSic.SexyContent.Administration.Apps.AppExport" %>
<%@ Import Namespace="ToSic.Eav" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<%@ Import Namespace="System.IO" %>

<%
	var version = "";
	if (Sexy.App.Configuration != null)
		version = Sexy.App.Configuration.Version;
%>

<h2>Will Export: App-<%= Sexy.App.Name %>-<%= version %>.zip</h2>
Specs:
<ul>
    <li>
        Name: <%= Sexy.App.Name %>
    </li>
    <li>
        Guid:<%= Sexy.App.AppGuid %>
    </li>
    <li>
        Version: <%= version %>
    </li>
</ul>
<br /><br />
Contains:<br/>
<ul>
    <li>
        <%= DataSource.GetInitialDataSource(ZoneId.Value, AppId.Value).Out["Default"].List.Count %> Entities
    </li>
    <li>
        <%= SexyContent.GetCulturesWithActiveState(PortalId, ZoneId.Value).Count(p => p.Active) %> Languages
    </li>
    <li>
        <%= Sexy.Templates.GetAllTemplates().Count() %> templates
    </li>
    <li>
        Tokens: <%= Sexy.Templates.GetAllTemplates().Any(p => !p.IsRazor) %>
    </li>
    <li>
        Razor: <%= Sexy.Templates.GetAllTemplates().Any(p => p.IsRazor) %>
    </li>
    <li>
        <% if (Directory.Exists(Sexy.App.PhysicalPath))
           { %>
            <%= Exporter.FileManager.AllTransferableFiles.Count() %> files in the App folder to export of a total of 
            <%= Exporter.FileManager.AllFiles.Count() %> files in the App folder
        <% }
           else
           { %>
            no files
        <% } %>

    </li>
    <%--<li>
        ToDo: files in the portal folder (images/pdf etc.)
    </li>--%>
</ul>
<br/>
<br/>
<asp:CheckBox runat="server" Checked="False" ID="chkIncludeContentGroups" /> Include all Content-Groups for re-import in copies of this exact site (only select this when you want to copy the entire DNN-site) - <a href="http://2sxc.org/en/help?tag=export-app" target="_blank">more help</a>
<br/>
<asp:CheckBox runat="server" Checked="False" ID="chkResetApGuid" /> Reset the App-Guid to 0000. You only need this in special tutorial-Apps, usually you should leave this blank. Read <a href="http://2sxc.org/en/help?tag=export-app" target="_blank">more</a>.
<br/>


<ul class="dnnActions">
    <li>
        <asp:LinkButton runat="server" ID="btnExportApp" OnClick="btnExportApp_OnClick" Text="Export" CssClass="dnnPrimaryAction"></asp:LinkButton>
            <a href="javascript:window.close();" class="dnnPrimaryAction">close</a>
    </li>
</ul>