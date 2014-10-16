<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.ViewApp" Codebehind="ViewApp.ascx.cs" %>
<asp:Placeholder runat="server" ID="pnlTemplateChooser" Visible="false">
    
    <%-- New AngularJS template chooser --%>
    <div ng-controller="TemplateSelectorCtrl" data-moduleid="<%= ModuleId %>" class="sc-selector-wrapper">
        <div ng-show="manageInfo.templateChooserVisible" class="dnnFormMessage dnnFormInfo">
            <div class="sc-selectors">
                <select ng-show="!manageInfo.isContentApp" ng-model="appId" class="sc-selector-app">
                    
                </select>

                <select ng-if="manageInfo.isContentApp" ng-model="contentTypeId" ng-options="c.AttributeSetID as c.Name for c in contentTypes" class="sc-selector-contenttype" ng-disabled="manageInfo.hasContent">
                    <option value=""><%= HttpUtility.HtmlEncode(LocalizeString("ddlContentTypeDefaultItem.Text")) %></option>
                </select>
        
                <select ng-show="contentTypeId != null" ng-model="templateId" class="sc-selector-template" ng-options="t.TemplateID as t.Name for t in filteredTemplates()">
                </select>
            </div>
        
            <a ng-show="templateId != null && savedTemplateId != templateId" ng-click="saveTemplateId(templateId);" class="sc-selector-save" title="Save Template">Save Template</a>
            <a ng-show="savedTemplateId != null" class="sc-selector-close" ng-click="setTemplateChooserState(false);" title="Cancel">Cancel</a>
        </div>
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

<asp:Panel runat="server" ID="pnlOpenCatalog" Visible="False">
    <script type="text/javascript">
        window.location = "<%= EditUrl("", "", ToSic.SexyContent.SexyContent.ControlKeys.AppImport) %>";
    </script>
</asp:Panel>
