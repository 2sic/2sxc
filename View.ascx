<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.View" Codebehind="View.ascx.cs" %>
<asp:Panel runat="server" ID="pnlTemplateChooser" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <div>
        <asp:DropDownList runat="server" ID="ddlContentType" AppendDataBoundItems="true" DataTextField="Name" DataValueField="AttributeSetId" OnSelectedIndexChanged="ddlContentType_SelectedIndexChanged" AutoPostBack="true" CssClass="sc-contenttype-selector">
            <asp:ListItem Value="0" ResourceKey="ddlContentTypeDefaultItem"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div>
        <asp:DropDownList runat="server" ID="ddlApp" AppendDataBoundItems="true" CssClass="sc-app-selector" DataTextField="Name" DataValueField="AppId" OnSelectedIndexChanged="ddlApp_SelectedIndexChanged" AutoPostBack="true">
            <asp:ListItem Value="0" Text="<Choose App>"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div>
        <asp:DropDownList runat="server" ID="ddlTemplate" DataTextField="Name" DataValueField="TemplateID" OnSelectedIndexChanged="ddlTemplate_SelectedIndexChanged" AutoPostBack="true" CssClass="sc-template-selector">
        </asp:DropDownList>
    </div>
</asp:Panel>
<asp:Panel runat="server" Visible="False" class="dnnFormMessage dnnFormInfo" ID="pnlGetStarted">
    <%= LocalizeString("GetStarted.Text") %>
</asp:Panel>

<asp:Panel runat="server" ID="pnlMissingConfiguration" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" ID="lblMissingConfiguration" ResourceKey="lblMissingConfiguration"></asp:Label>
    <asp:LinkButton runat="server" ID="hlkConfigurePortal" 
        CssClass="dnnSecondaryAction" ResourceKey="hlkConfigurePortal" 
        onclick="hlkConfigurePortal_Click"></asp:LinkButton>
</asp:Panel>

<asp:Panel runat="server" ID="pnlZoneConfigurationMissing" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" ID="lblMissingZoneConfiguration" ResourceKey="ZoneConfigurationMissing"></asp:Label>
    <asp:HyperLink runat="server" ID="hlkConfigureZone" 
        CssClass="dnnSecondaryAction" ResourceKey="hlkConfigureZone"></asp:HyperLink>
</asp:Panel>

<asp:Panel runat="server" ID="pnlError" CssClass="dnnFormMessage dnnFormWarning" Visible="false"></asp:Panel>
<asp:Panel runat="server" ID="pnlMessage" CssClass="dnnFormMessage dnnFormInfo" Visible="false"></asp:Panel>
<asp:PlaceHolder runat="server" ID="phOutput"></asp:PlaceHolder>

<asp:HiddenField runat="server" ID="hfContentGroupItemID" Value="" Visible="false" />
<asp:HiddenField runat="server" ID="hfContentGroupItemAction" Value="" Visible="false" OnValueChanged="hfContentGroupItemAction_ValueChanged" />