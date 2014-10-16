<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.View" Codebehind="View.ascx.cs" %>
<asp:Placeholder runat="server" ID="pnlTemplateChooser" Visible="false">
    <%--<div>
        <asp:DropDownList runat="server" ID="ddlContentType" AppendDataBoundItems="true" DataTextField="Name" DataValueField="AttributeSetId" OnSelectedIndexChanged="ddlContentType_SelectedIndexChanged" AutoPostBack="true" CssClass="sc-contenttype-selector">
            <asp:ListItem Value="0" ResourceKey="ddlContentTypeDefaultItem"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div>
        <asp:DropDownList runat="server" ID="ddlTemplate" DataTextField="Name" DataValueField="TemplateID" CssClass="sc-template-selector">
        </asp:DropDownList>
    </div>--%>
    
    <%--CssClass="dnnFormMessage dnnFormInfo sc-choosetemplate"--%>
    
    <%-- New AngularJS template chooser --%>
    <div ng-controller="TemplateSelectorCtrl" data-moduleid="<%= ModuleId %>" ng-visible="manageInfo.templateChooserVisible" class="dnnFormMessage dnnFormInfo sc-selector-wrapper">
        
        <div class="sc-selectors">
            <select ng-model="contentTypeId" ng-options="c.AttributeSetID as c.Name for c in contentTypes" class="sc-selector-contenttype" ng-disabled="manageInfo.hasContent">
                <option value=""><%= HttpUtility.HtmlEncode(LocalizeString("ddlContentTypeDefaultItem.Text")) %></option>
            </select>
        
            <select ng-model="templateId" class="sc-selector-template" ng-options="t.TemplateID as t.Name for t in filteredTemplates()">
                <option value="" ng-disabled="manageInfo.hasContent"><%= HttpUtility.HtmlEncode(LocalizeString("ddlTemplateDefaultItem.Text")) %></option>
            </select>
        </div>
        
        <a ng-visible="selectedTemplate != null" class="sc-selector-close" ng-click="setTemplateChooserState(false);">Close</a>
        <a ng-click="saveTemplateId(templateId)" class="sc-selector-save">Save Template</a>
    </div>

</asp:Placeholder>

<asp:Panel runat="server" Visible="False" class="dnnFormMessage dnnFormInfo" ID="pnlGetStarted"></asp:Panel>

<asp:Panel runat="server" ID="pnlZoneConfigurationMissing" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" ID="lblMissingZoneConfiguration" ResourceKey="ZoneConfigurationMissing"></asp:Label>
    <asp:HyperLink runat="server" ID="hlkConfigureZone" 
        CssClass="dnnSecondaryAction" ResourceKey="hlkConfigureZone"></asp:HyperLink>
</asp:Panel>

<asp:Panel runat="server" ID="pnlError" CssClass="dnnFormMessage dnnFormWarning" Visible="false"></asp:Panel>
<asp:Panel runat="server" ID="pnlMessage" CssClass="dnnFormMessage dnnFormInfo" Visible="false"></asp:Panel>

<div class="sc-viewport">
    <asp:PlaceHolder runat="server" ID="phOutput"></asp:PlaceHolder>
</div>