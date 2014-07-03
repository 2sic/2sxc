<%@ Control Language="C#" Inherits="ToSic.Eav.ManagementUI.Entity_EditCustom" Codebehind="Entity_Edit.ascx.cs" AutoEventWireup="True" %>
<%@ Register src="../Controls/DimensionMenu.ascx" tagPrefix="Eav" tagName="DimensionMenu" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FieldTemplates/Entity_Edit.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FieldTemplates/Entity_Edit.js" />

<dnn:Label ID="FieldLabel" runat="server" Suffix=":" />
<Eav:DimensionMenu ID="DimensionMenu1" runat="server" />

<div class="eav-field-control dnnLeft" ng-controller="EntityEditCtrl">
    
    <input style="display:none;" type="text" runat="server" id="hfConfiguration" ng-value="configuration | json" />
    <input style="display:none;" type="text" runat="server" id="hfEntityIds" ng-value="entityIds()" />

    <div ui-tree="options" data-empty-place-holder-enabled="false">
        <ol ui-tree-nodes ng-model="configuration.SelectedEntities">
            <li ng-repeat="item in configuration.SelectedEntities" ui-tree-node class="eav-entityselect-item">
                <div ui-tree-handle ng-init="itemText = (configuration.Entities | filter:{Value: item})[0].Text">
                    <span title="{{itemText}}">{{itemText}}</span>
                    <a data-nodrag title="Remove this item" ng-click="remove(this)" class="eav-entityselect-item-remove">[remove]</a>
                </div>
            </li>
        </ol>
    </div>
        
    <select class="eav-entityselect-selector" ng-model="selectedEntity" ng-change="AddEntity()" ng-show="configuration.AllowMultiValue || configuration.SelectedEntities.length < 1">
        <option value="">--add more--</option>
        <option ng-repeat="item in configuration.Entities" ng-disabled="configuration.SelectedEntities.indexOf(item.Value) != -1" value="{{item.Value}}">{{item.Text}}</option>
    </select>

</div>


<%--<div style="overflow:hidden; margin-bottom: 13px">
    <div class="eav-field-control dnnLeft">
	    <asp:DropDownList ID="DropDownList1" runat="server" DataTextField="Text" DataValueField="Value" AppendDataBoundItems="True" OnDataBound="DropDownList1_DataBound" CssClass="dnnFormInput eav-entity-selector">
		    <Items>
			    <asp:ListItem Text="(none)" Value="" />
		    </Items>
	    </asp:DropDownList>
	    <asp:HiddenField ID="hfEntityIds" runat="server" Visible="False" />
	    <asp:Panel runat="server" ID="pnlMultiValues" CssClass="MultiValuesWrapper" Visible="False"/>
	    <asp:PlaceHolder runat="server" ID="phAddMultiValue" Visible="False">
		    <a href="javascript:void(0)" class="AddValue dnnSecondaryAction">Add Value</a>
	    </asp:PlaceHolder>
    </div>
</div>--%>