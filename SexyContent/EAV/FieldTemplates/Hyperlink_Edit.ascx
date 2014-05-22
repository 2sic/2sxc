<%@ Control Language="C#" Inherits="ToSic.Eav.ManagementUI.Hyperlink_EditCustom" AutoEventWireup="True" CodeBehind="Hyperlink_Edit.ascx.cs" %>
<%@ Import Namespace="DotNetNuke.Entities.Portals" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="SexyContent" TagName="DimensionMenu" Src="../Controls/DimensionMenu.ascx" %>


<dnn:Label ID="FieldLabel" runat="server" Suffix=":" />
<SexyContent:DimensionMenu runat="server" />
<div class="eav-field-control" data-homedirectory="<%# PortalSettings.Current.HomeDirectory %>">
	<asp:TextBox runat="server" ID="txtFilePath" />
    <asp:RequiredFieldValidator ID="valFieldValue" runat="server" CssClass="dnnFormMessage dnnFormError" ControlToValidate="txtFilePath" Display="Dynamic" EnableClientScript="true" ErrorMessage="Please enter a value."></asp:RequiredFieldValidator>
	<div class="sc-hyperlink-picker-wrapper">
		<div class="eav-contextmenu">
			<asp:HyperLink ID="hlkFileBrowse" runat="server" NavigateUrl='<%# "javascript:" + GetClientOpenDialogCommand() %>' Text="..." CssClass="sc-hyperlink-browse" />
			<ul class="sc-hyperlink-pickermenu eav-contextmenu-actions" id="ulPickerMenu" runat="server">
				<li id="liPagePicker" runat="server"><a href="javascript:void(0)">Page</a></li>
				<li id="liImageManager" runat="server"><a href="javascript:void(0)">Image</a></li>
				<li id="liDocumentManager" runat="server"><a href="javascript:void(0)">File</a></li>
			</ul>
		</div>
	</div>
	<div class="sc-hyperlink-testlink-wrapper">
		Test: <a href="javascript:void(0)" class="sc-hyperlink-testlink" target="_blank"></a>
	</div>
</div>
<telerik:DialogOpener runat="server" ID="DialogOpener1" />
<asp:Panel runat="server" ID="pnlDnnPageDropDownList" CssClass="sc-hyperlink-pagepicker" style="display: none"/>