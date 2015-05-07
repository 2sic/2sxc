<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Export.ascx.cs" Inherits="ToSic.SexyContent.Export" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js" Priority="60" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/Administration/Export.js" Priority="65" />

<asp:Panel runat="server" class="dnnForm dnnSexyContentExport dnnClear" id="pnlChoose" ng-app="2sxc-Export">
    <h2 class="dnnFormSectionHead" runat="server" id="dnnSitePanelExport"><asp:Label runat="server" ID="lblExportHeading" ResourceKey="lblExportHeading"></asp:Label></h2>
    <div class="dnnFormInfo">
        <asp:Label runat="server" ID="Label2" ResourceKey="lblPartialExportIntro"></asp:Label>
    </div>

    <fieldset>
        <div id="pnlExportView" ng-controller="ExportController" runat="server">
            
            <div style="display:none;">
                <asp:TextBox runat="server" ID="txtSelectedContentTypes" ng-value="selectedContentTypesString()"></asp:TextBox>
                <asp:TextBox runat="server" ID="txtSelectedTemplates" ng-value="selectedTemplatesString()"></asp:TextBox>
                <asp:TextBox runat="server" ID="txtSelectedEntities" ng-value="selectedEntitiesString()"></asp:TextBox>
            </div>

            <ul class="sc-export-list">
                <li ng-repeat="contentType in contentTypes">
                    <a ng-click="contentType._2sxcExport = !contentType._2sxcExport;" ng-class="{active: contentType._2sxcExport}">
                        <input type="checkbox" ng-model="contentType._2sxcExport" ng-click="$event.stopPropagation()" />
                        ContentType: {{contentType.Name}} ({{contentType.Id}})
                    </a>
                    
                    <div class="sc-export-list-inner" ng-if="contentType.Templates.length">
                        <strong>Templates</strong>
                        <ul>
                            <li ng-repeat="template in contentType.Templates">
                                <a ng-click="template._2sxcExport = !template._2sxcExport;" ng-class="{active: template._2sxcExport}">
                                    <input type="checkbox" ng-model="template._2sxcExport" ng-click="$event.stopPropagation()" />
                                    {{template.Name}} ({{template.TemplateID}})

                                    <i ng-init="currentTemplateDefaults = templateDefaultFilter(template.TemplateDefaults);">
                                        <span ng-if="currentTemplateDefaults.length > 0" style="margin-left:15px;">Demo-Entities: </span>
                                        <span ng-repeat="templateDefault in currentTemplateDefaults">
                                            {{templateDefault.ItemType}} {{templateDefault.DemoEntityID}}, 
                                        </span>
                                    </i>
                                </a>
                            </li>
                        </ul>
                    </div>
                    <div class="sc-export-list-inner" ng-if="contentType.Entities.length">
                        <strong>Entities</strong>
                        <ul>
                            <li ng-repeat="entity in contentType.Entities">                                
                                <a ng-click="entity._2sxcExport = !entity._2sxcExport;" ng-class="{active: entity._2sxcExport}">
                                    <input type="checkbox" ng-model="entity._2sxcExport" ng-click="$event.stopPropagation()" />
                                    {{entity._2sxcEditInformation.title}} ({{entity.EntityId}})
                                </a>
                            </li>
                        </ul>
                    </div>
                </li>
            </ul>
            <br /><br />
            <strong>Templates without content type</strong>
            <ul class="sc-export-list">
                <li ng-repeat="template in templatesWithoutContentType">
                    <a ng-click="template._2sxcExport = !template._2sxcExport;" ng-class="{active: template._2sxcExport}">
                        <input type="checkbox" ng-model="template._2sxcExport" ng-click="$event.stopPropagation()" />
                        {{template.Name}} ({{template.TemplateID}})
                    </a>
                </li>
            </ul>
        </div>
        
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton ID="btnExport" runat="server" CssClass="dnnPrimaryAction" ResourceKey="hlkExport" OnClick="btnExport_Click"></asp:LinkButton></li>
            <li><asp:HyperLink ID="hlkCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="hlkCancel"></asp:HyperLink></li>
        </ul>
    </fieldset>
</asp:Panel>


<style>
    .sc-export-list { list-style-type: none;margin: 20px 0;padding: 0;border-top: 1px solid #DDD; }
    .sc-export-list-inner { padding: 3px 0px 7px 40px; }
    .sc-export-list > li { margin: 0;border-bottom: 1px solid #DDD;font-weight: bold; }
    .sc-export-list > li > a { padding: 12px;font-size: 16px; }
    .sc-export-list > li a { color: #333;text-decoration: none;cursor: pointer;display: block; }
    .sc-export-list > li a:hover { background: #EEE; }
    .sc-export-list > li a.active { background: #E6F7E7; }
    .sc-export-list > li ul { list-style-type: none;margin: 0;padding: 0;  }
    .sc-export-list > li li { font-weight: normal;padding: 0;margin: 0; }
    .sc-export-list > li li a { padding: 5px; }
</style>