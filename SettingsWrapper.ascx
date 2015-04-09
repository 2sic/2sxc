<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.SettingsWrapper" Codebehind="SettingsWrapper.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelSexyContentSettings">
        <a href="#"><asp:Label runat="server" ID="lblSettingsHeading" ResourceKey="lblSettingsHeading"></asp:Label></a></h2>
    <fieldset>
        <%--<div class="dnnFormItem">
            <dnn:Label ID="lblContentType" runat="server" ControlName="lblContentType" Suffix=":" />
            <asp:Label ID="lblContentTypeDefaultText" runat="server" ResourceKey="lblContentTypeDefaultText"></asp:Label>
            <asp:Label ID="lblContentTypeText" Visible="false" runat="server"></asp:Label>
        </div>--%>
        <div class="dnnFormItem" runat="server" id="pnlEntities">
            <dnn:Label ID="lblEntityID" runat="server" ControlName="EntityID" Suffix=":" />
            <asp:DropDownList ID="ddlEntities" runat="server" DataTextField="EntityTitle" DataValueField="EntityID"
                AppendDataBoundItems="true">
                <asp:ListItem ResourceKey="ddlEntitiesDefaultItem" Value="-1"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </fieldset>
    <ul class="dnnActions">
        <li>
            <asp:LinkButton runat="server" ID="btnSave" class="dnnPrimaryAction" onclick="btnSave_Click" ResourceKey="btnSave"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnCancel" class="dnnSecondaryAction" onclick="btnCancel_Click" ResourceKey="btnCancel"></asp:LinkButton>
        </li>
    </ul>
</div>