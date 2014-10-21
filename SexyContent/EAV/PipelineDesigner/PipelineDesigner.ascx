<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PipelineDesigner.ascx.cs" Inherits="ToSic.Eav.ManagementUI.EAV.PipelineDesigner.PipelineDesigner" %>
<div ng-app="pipelineDesinger" class="ng-cloak">
	<div ng-controller="pipelineDesignerController">
		<div id="pipelineContainer">
			<div
				ng-repeat="dataSource in pipelineData.DataSources"
				datasource
				id="dataSource_{{dataSource.EntityGuid}}"
				class="dataSource"
				ng-attr-style="top: {{dataSource.VisualDesignerData.Top}}px; left: {{dataSource.VisualDesignerData.Left}}px"
				title="Double click to edit the Configuration"
				ng-dblclick="configureDataSource(dataSource)">
				<div class="name" title="Click to edit the Name" ng-click="editName(dataSource)">{{dataSource.Name || '(unnamed)'}}</div><br/>
				<div class="description" title="Click to edit the Description" ng-click="editDescription(dataSource)">{{dataSource.Description || '(no description)'}}</div><br/>
				<div class="typename" ng-attr-title="{{dataSource.PartAssemblyAndType}}">Type: {{dataSource.PartAssemblyAndType | typename: 'className'}}</div>
				<div class="ep" title="Drag a new Out-Connection from here" ng-if="!dataSource.ReadOnly"></div>
				<div class="delete" title="Delete this DataSource" ng-click="remove($index)" ng-if="!dataSource.ReadOnly"></div>
			</div>
		</div>
		<div class="actions panel panel-default">
			<div class="panel-heading">Actions</div>
			<div class="panel-body">
				<button class="btn btn-primary btn-block" ng-disabled="readOnly" ng-click="savePipeline()">Save</button>
				<select class="form-control" ng-model="addDataSourceType" ng-disabled="readOnly" ng-change="addDataSource()" ng-options="d.ClassName for d in pipelineData.InstalledDataSources | filter: {allowNew: '!false'} | orderBy: 'ClassName'">
					<option value="">-- Add DataSource --</option>
				</select>
				<button class="btn btn-default btn-sm" title="Query the Data of this Pipeline" ng-click="queryPipeline()">Query</button>
				<button class="btn btn-default btn-sm" title="Clone this Pipeline with all DataSources and Configurations" ng-click="clonePipeline()" ng-disabled="!PipelineEntityId">Clone</button>
				<button class="btn btn-info btn-xs" ng-click="toggleEndpointOverlays()">{{showEndpointOverlays ? 'Hide' : 'Show' }} Overlays</button>
				<button class="btn btn-info btn-xs" ng-click="repaint()">Repaint</button>
				<button class="btn btn-info btn-xs" ng-click="toogleDebug()">{{debug ? 'Hide' : 'Show'}} Debug Info</button>
			</div>
		</div>
		<toaster-container></toaster-container>
		<pre ng-if="debug">{{pipelineData | json}}</pre>
	</div>
</div>
