<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.ViewApp" Codebehind="ViewApp.ascx.cs" %>
<%@ Import Namespace="ToSic.SexyContent" %>
<asp:Panel runat="server" ID="pnlTemplateChooser" Visible="false" CssClass="dnnFormMessage dnnFormInfo sc-choosetemplate">
    <div>
        <asp:DropDownList runat="server" ID="ddlApp" Visible="False" AppendDataBoundItems="true" CssClass="sc-app-selector" DataTextField="Name" DataValueField="AppId" OnSelectedIndexChanged="ddlApp_SelectedIndexChanged" AutoPostBack="true">
            <asp:ListItem Value="0" Text="<Choose App>" resourcekey="ddlAppDefaultItem"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div>
        <asp:DropDownList runat="server" ID="ddlTemplate" DataTextField="Name" DataValueField="TemplateID" OnSelectedIndexChanged="ddlTemplate_SelectedIndexChanged" AutoPostBack="true" CssClass="sc-template-selector">
        </asp:DropDownList>
    </div>
    <% if (Template != null) { %>
        <a class="sc-choosetemplate-close" href="javascript:$2sxc(<%= ModuleId %>).manage._setTemplateChooserState(false);">Close</a>
    <% } %>
</asp:Panel>
<asp:Panel runat="server" Visible="False" class="dnnFormMessage dnnFormInfo" ID="pnlGetStarted"></asp:Panel>

<asp:Panel runat="server" ID="pnlZoneConfigurationMissing" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" ID="lblMissingZoneConfiguration" ResourceKey="ZoneConfigurationMissing"></asp:Label>
    <asp:HyperLink runat="server" ID="hlkConfigureZone" 
        CssClass="dnnSecondaryAction" ResourceKey="hlkConfigureZone"></asp:HyperLink>
</asp:Panel>

<asp:Panel runat="server" ID="pnlError" CssClass="dnnFormMessage dnnFormWarning" Visible="false"></asp:Panel>
<asp:Panel runat="server" ID="pnlMessage" CssClass="dnnFormMessage dnnFormInfo" Visible="false"></asp:Panel>
<asp:PlaceHolder runat="server" ID="phOutput"></asp:PlaceHolder>

<asp:Panel runat="server" ID="pnlOpenCatalog" Visible="False">
    <script type="text/javascript">
        window.location = "<%= EditUrl("", "", SexyContent.ControlKeys.AppImport) %>";
    </script>
</asp:Panel>
