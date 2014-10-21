<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PipelineDesignerDnnWrapper.ascx.cs" Inherits="ToSic.SexyContent.EAV.PipelineDesigner.PipelineDesignerDnnWrapper" %>
<%@ Register src="PipelineDesigner.ascx" tagname="PipelineDesigner" tagprefix="sxc" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" Priority="60" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/PipelineDesigner/PipelineDesigner.css" />
<dnn:DnnCssInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/toaster.css" />

<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css">
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap-theme.min.css">

<sxc:PipelineDesigner ID="PipelineDesigner1" runat="server" />

<dnn:DnnJsInclude runat="server" Priority="60" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js" />
<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-resource.min.js" />
<dnn:DnnJsInclude runat="server" Priority="62" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-animate.min.js" />
<dnn:DnnJsInclude runat="server" Priority="63" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/toaster.js" />
<dnn:DnnJsInclude runat="server" Priority="64" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/PipelineDesigner/assets/jquery.jsPlumb-1.6.4-min.js" />
<dnn:DnnJsInclude runat="server" Priority="65" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/PipelineDesigner/PipelineDesigner.js" />
<dnn:DnnJsInclude runat="server" Priority="66" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/PipelineDesigner/PipelineDesignerController.js" />
<dnn:DnnJsInclude runat="server" Priority="67" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/PipelineDesigner/PipelineFactory.js" />
<dnn:DnnJsInclude runat="server" Priority="69" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/NotificationService.js" />
<dnn:DnnJsInclude runat="server" Priority="70" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavGlobalConfigurationProvider.js" />
<dnn:DnnJsInclude runat="server" Priority="71" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavDialogService.js" />
<%--<dnn:DnnJsInclude runat="server" Priority="68" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavApiService.js" />--%>