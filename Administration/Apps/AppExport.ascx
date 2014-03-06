<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppExport.ascx.cs" Inherits="ToSic.SexyContent.Administration.Apps.AppExport" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="ToSic.Eav" %>
<%@ Import Namespace="ToSic.SexyContent" %>

Will Export: App-<%= Sexy.App.Name %>-<%= Sexy.App.Configuration.Version %>.zip<br/><br/>
Specs:
<ul>
    <li>
        Name: <%= Sexy.App.Name %>
    </li>
    <li>
        Guid:<%= Sexy.App.Name %><br />
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
        <%= Sexy.GetTemplates(PortalId) %>
    </li>
    <li>
        Tokens: <%= Sexy.GetTemplates(PortalId).Any(p => !p.IsRazor) %>
    </li>
    <li>
        Razor: <%= Sexy.GetTemplates(PortalId).Any(p => p.IsRazor) %>
    </li>
    <li>
        <%= new DirectoryInfo(Sexy.App.PhysicalPath).GetFiles("*.*", SearchOption.AllDirectories).Count()  %> files int the App folder
    </li>
    <li>
        ToDo: files in the portal folder (images/pdf etc.)
    </li>
</ul>
