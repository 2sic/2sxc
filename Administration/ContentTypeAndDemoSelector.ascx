<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentTypeAndDemoSelector.ascx.cs" Inherits="ToSic.SexyContent.ContentTypeAndDemoSelector" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
    <dnn:Label ID="lblContentType" runat="server" Suffix=":" />
    <asp:Panel runat="server" ID="pnlLocked" Visible="False">
        <asp:Label runat="server" ID="lblLocked" resourcekey="lblLocked"></asp:Label><br />
    </asp:Panel>
    <asp:DropDownList ID="ddlContentTypes" DataTextField="Name" 
        DataValueField="StaticName" runat="server" AppendDataBoundItems="true" 
        AutoPostBack="true" 
        onselectedindexchanged="ddlContentTypes_SelectedIndexChanged">
        <asp:ListItem ResourceKey="ddlContentTypesDefaultItem" Value="0"></asp:ListItem>
    </asp:DropDownList>
    <asp:RequiredFieldValidator ID="valContentType" runat="server" InitialValue="" ControlToValidate="ddlContentTypes" CssClass="dnnFormError" Display="Dynamic"></asp:RequiredFieldValidator>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblDemoRow" runat="server" Suffix=":" />
    <asp:DropDownList ID="ddlDemoRows" runat="server" DataTextField="EntityTitle" DataValueField="EntityId" AppendDataBoundItems="true">
        <asp:ListItem ResourceKey="ddlDemoRowsDefaultItem" Value="0"></asp:ListItem>
    </asp:DropDownList>
</div>