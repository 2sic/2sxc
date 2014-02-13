<%@ Control Language="C#" Inherits="ToSic.Eav.ManagementUI.Boolean_EditCustom" Codebehind="Boolean_Edit.ascx.cs" AutoEventWireup="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register tagPrefix="SexyContent" tagName="DimensionMenu" src="../Controls/DimensionMenu.ascx" %>

<dnn:Label ID="FieldLabel" runat="server" Suffix=":" />    
<SexyContent:DimensionMenu runat="server"></SexyContent:DimensionMenu>
<div class="eav-field-control">
<asp:CheckBox ID="CheckBox1" runat="server" CssClass="DDCheckBox" EnableViewState="true" />
</div>