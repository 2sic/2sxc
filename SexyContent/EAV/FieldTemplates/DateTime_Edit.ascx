<%@ Control Language="C#" Inherits="ToSic.Eav.ManagementUI.DateTime_EditCustom" Codebehind="DateTime_Edit.ascx.cs" AutoEventWireup="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register tagPrefix="SexyContent" tagName="DimensionMenu" src="../Controls/DimensionMenu.ascx" %>

<dnn:Label ID="FieldLabel" runat="server" Suffix=":" />

<SexyContent:DimensionMenu runat="server"></SexyContent:DimensionMenu>
<div class="eav-field-control">
    <dnnweb:DnnDatePicker runat="server" ID="Calendar1" MinDate="1000-01-01" MaxDate="9999-01-01" />
    <dnnweb:DnnTimePicker runat="server" ID="TimePicker1" Visible="false" MinDate="1000-01-01" MaxDate="9999-01-01" />
</div>
<asp:RequiredFieldValidator ID="valCalendar1" runat="server" ControlToValidate="Calendar1"
    ErrorMessage="Please select or enter a date." Display="Dynamic" CssClass="dnnFormMessage dnnFormError"></asp:RequiredFieldValidator>