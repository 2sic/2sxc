<%@ Control Language="C#" Inherits="ToSic.Eav.ManagementUI.Number_EditCustom" Codebehind="Number_Edit.ascx.cs" AutoEventWireup="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register tagPrefix="SexyContent" tagName="DimensionMenu" src="../Controls/DimensionMenu.ascx" %>

<dnn:Label ID="FieldLabel" runat="server" Suffix=":" />

<SexyContent:DimensionMenu runat="server"></SexyContent:DimensionMenu>
<div class="eav-field-control">
    <asp:TextBox ID="TextBox1" runat="server" CssClass="DDTextBox" EnableViewState="true" />
</div>
<asp:RequiredFieldValidator ID="valFieldValue" runat="server" CssClass="dnnFormMessage dnnFormError" ControlToValidate="TextBox1" Display="Dynamic" EnableClientScript="true" ErrorMessage="Please enter a value."></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator ID="valFieldValue2" runat="server" CssClass="dnnFormMessage dnnFormError" ControlToValidate="TextBox1" Display="Dynamic" EnableClientScript="true" ErrorMessage="Please enter numeric value." Enabled="true" ValidationExpression="^[\d\.,]*$"></asp:RegularExpressionValidator>