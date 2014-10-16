<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.View" Codebehind="View.ascx.cs" %>
<asp:Panel runat="server" ID="pnlTemplateChooser" Visible="false" CssClass="dnnFormMessage dnnFormInfo sc-choosetemplate">
    <%--<div>
        <asp:DropDownList runat="server" ID="ddlContentType" AppendDataBoundItems="true" DataTextField="Name" DataValueField="AttributeSetId" OnSelectedIndexChanged="ddlContentType_SelectedIndexChanged" AutoPostBack="true" CssClass="sc-contenttype-selector">
            <asp:ListItem Value="0" ResourceKey="ddlContentTypeDefaultItem"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div>
        <asp:DropDownList runat="server" ID="ddlTemplate" DataTextField="Name" DataValueField="TemplateID" CssClass="sc-template-selector">
        </asp:DropDownList>
    </div>--%>
    
    
    <%-- New AngularJS template chooser --%>
    <div ng-controller="TemplateSelectorCtrl" data-moduleid="<%= ModuleId %>" ng-visible="manageInfo.templateChooserVisible">
        
        <div>
            <select ng-model="contentTypeId" ng-options="c.AttributeSetID as c.Name for c in contentTypes" class="sc-contenttype-selector" ng-disabled="manageInfo.hasContent">
                <option value=""><%= HttpUtility.HtmlEncode(LocalizeString("ddlContentTypeDefaultItem.Text")) %></option>
            </select>
        </div>
        
        <div>
            <select ng-model="templateId" class="sc-template-selector" ng-options="t.TemplateID as t.Name for t in filteredTemplates()">
                <option value="" ng-disabled="manageInfo.hasContent"><%= HttpUtility.HtmlEncode(LocalizeString("ddlTemplateDefaultItem.Text")) %></option>
            </select>
        </div>
        
        <a ng-visible="selectedTemplate != null" class="sc-choosetemplate-close" ng-click="setTemplateChooserState(false);">Close</a>
        <a ng-click="saveTemplateId(templateId)">Save Template</a>
    </div>

</asp:Panel>

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