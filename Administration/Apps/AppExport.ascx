<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppExport.ascx.cs" Inherits="ToSic.SexyContent.Administration.Apps.AppExport" %>
<%@ Import Namespace="ToSic.Eav" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="ToSic.SexyContent.Internal" %>

<%
	var version = "";
	if (_sxcInstance.App.Configuration != null)
		version = _sxcInstance.App.Configuration.Version;
%>

<h2>Will Export: App-<%= _sxcInstance.App.Name %>-<%= version %>.zip</h2>
Specs:
<ul>
    <li>
        Name: <%= _sxcInstance.App.Name %>
    </li>
    <li>
        Guid:<%= _sxcInstance.App.AppGuid %>
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
        <%= ZoneHelpers.GetCulturesWithActiveState(PortalId, ZoneId.Value).Count(p => p.Active) %> Languages
    </li>
    <li>
        <%= _sxcInstance.AppTemplates.GetAllTemplates().Count() %> templates
    </li>
    <li>
        Tokens: <%= _sxcInstance.AppTemplates.GetAllTemplates().Any(p => !p.IsRazor) %>
    </li>
    <li>
        Razor: <%= _sxcInstance.AppTemplates.GetAllTemplates().Any(p => p.IsRazor) %>
    </li>
    <li>
        <% if (Directory.Exists(_sxcInstance.App.PhysicalPath))
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