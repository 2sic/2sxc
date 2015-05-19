<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PermissionsDnnWrapper.ascx.cs" Inherits="ToSic.SexyContent.EAV.Security.PermissionsDnnWrapper" %>
<%@ Register Src="Permissions.ascx" TagName="Permissions" TagPrefix="sxc"  %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css">
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap-theme.min.css">
<style type="text/css">
	a.btn-default { color: #333 }	/* overwrite DNN Button-Color */
</style>

<sxc:permissions runat="server" id="Permissions" />

<dnn:DnnJsInclude runat="server" Priority="60" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js" />
<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular-resource.min.js" />

<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/js/2sxc.api.min.js" />
<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/2sxc4ng.min.js" />

<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/eavNgSvcs.js" />


<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Security/PermissionsServices.js" />
<dnn:DnnJsInclude runat="server" Priority="61" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/Security/PermissionsController.js" />


<dnn:DnnJsInclude runat="server" Priority="70" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/AngularServices/EavGlobalConfigurationProvider.js" />
