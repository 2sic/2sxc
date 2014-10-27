<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PipelineManagementDnnWrapper.ascx.cs" Inherits="ToSic.SexyContent.EAV.PipelineDesigner.PipelineManagementDnnWrapper" %>
<%@ Register Src="PipelineManagement.ascx" TagName="PipelineManagement" TagPrefix="sxc"  %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css">
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap-theme.min.css">

<sxc:PipelineManagement runat="server" id="PipelineManagement" />

<dnn:DnnJsInclude runat="server" Priority="60" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js" />
<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-resource.min.js" />
<dnn:DnnJsInclude runat="server" Priority="62" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/PipelineDesigner/PipelineManagement.js" />
<dnn:DnnJsInclude runat="server" Priority="70" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavGlobalConfigurationProvider.js" />
