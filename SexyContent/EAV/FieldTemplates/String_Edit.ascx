<%@ Control Language="C#" Inherits="ToSic.Eav.ManagementUI.Text_EditCustom" Codebehind="String_Edit.ascx.cs" AutoEventWireup="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register tagPrefix="SexyContent" tagName="DimensionMenu" src="../Controls/DimensionMenu.ascx" %>

<dnn:Label ID="FieldLabel" runat="server" Suffix=":" />
<SexyContent:DimensionMenu runat="server"></SexyContent:DimensionMenu>

<div class="eav-field-control">
    <asp:TextBox ID="TextBox1" runat="server" CssClass="DDTextBox" EnableViewState="true" />
    <asp:DropDownList DataTextField="Text" DataValueField="Value" Visible="false" runat="server" ID="DropDown1" CssClass="dnnFormInput"></asp:DropDownList>
    <%--<dnn:DnnComboBox DataTextField="Text" DataValueField="Value" Visible="false" runat="server" ID="DropDown1" CssClass="dnnFormInput"></dnn:DnnComboBox>--%>
    <dnn:texteditor Visible="false" Height="400" Width="100%" ID="Texteditor1" runat="server" EnableViewState="true" HtmlEncode="false" />
    <div class="dnnLeft">
        <dnn:URL ID="DnnUrl1" runat="server" Visible="false" ShowFiles="false" ShowTrack="false" ShowLog="false" ShowTabs="true" ShowUrls="true"></dnn:URL>
    </div>
    <asp:RequiredFieldValidator ID="valFieldValue" runat="server" CssClass="dnnFormMessage dnnFormError" ControlToValidate="TextBox1" Display="Dynamic" EnableClientScript="true" ErrorMessage="Please enter a value."></asp:RequiredFieldValidator>
    <asp:RegularExpressionValidator ID="valRegularExpression" runat="server" CssClass="dnnFormMessage dnnFormError" ControlToValidate="TextBox1" Display="Dynamic" EnableClientScript="true" ErrorMessage="Please enter a valid value."></asp:RegularExpressionValidator>
</div>