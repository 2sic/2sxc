<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppExport.ascx.cs" Inherits="ToSic.SexyContent.Administration.Apps.AppExport" %>
<%@ Import Namespace="ToSic.Eav" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<%@ Import Namespace="System.IO" %>

<h2>Will Export: App-<%= Sexy.App.Name %>-<%= Sexy.App.Configuration.Version %>.zip</h2>
Specs:
<ul>
    <li>
        Name: <%= Sexy.App.Name %>
    </li>
    <li>
        Guid:<%= Sexy.App.AppGuid %>
    </li>
    <li>
        Version: <%= Sexy.App.Configuration.Version %>
    </li>
</ul>
<br /><br />
Contains:<br/>
<ul>
    <li>
        <%= DataSource.GetInitialDataSource(ZoneId.Value, AppId.Value).Out["Default"].List.Count %> Entities
    </li>
    <li>
        <%= SexyContent.GetCulturesWithActiveState(PortalId, ZoneId.Value).Where(p => p.Active).Count() %> Languages
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
            <%= new DirectoryInfo(Sexy.App.PhysicalPath).GetFiles("*.*", SearchOption.AllDirectories).Count() %> files int the App folder
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


<ul class="dnnActions">
    <li>
        <asp:LinkButton runat="server" ID="btnExportApp" OnClick="btnExportApp_OnClick" Text="Export" CssClass="dnnPrimaryAction"></asp:LinkButton>
    </li>
</ul>