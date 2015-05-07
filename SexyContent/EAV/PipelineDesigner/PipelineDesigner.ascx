<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PipelineDesigner.ascx.cs" Inherits="ToSic.Eav.ManagementUI.EAV.PipelineDesigner.PipelineDesigner" %>
<div ng-app="pipelineDesinger" class="ng-cloak">
	<div ng-controller="PipelineDesignerController">
		<div id="pipelineContainer">
			<div
				ng-repeat="dataSource in pipelineData.DataSources"
				datasource
				id="dataSource_{{dataSource.EntityGuid}}"
				class="dataSource"
				ng-attr-style="top: {{dataSource.VisualDesignerData.Top}}px; left: {{dataSource.VisualDesignerData.Left}}px">
				<div class="configure" ng-click="configureDataSource(dataSource)" title="Configure this DataSource" ng-if="!dataSource.ReadOnly">
					<span class="glyphicon glyphicon-list-alt"></span>
				</div>
				<div class="name" title="Click to edit the Name" ng-click="editName(dataSource)">{{dataSource.Name || '(unnamed)'}}</div><br/>
				<div class="description" title="Click to edit the Description" ng-click="editDescription(dataSource)">{{dataSource.Description || '(no description)'}}</div><br/>
				<div class="typename" ng-attr-title="{{dataSource.PartAssemblyAndType}}">Type: {{dataSource.PartAssemblyAndType | typename: 'className'}}</div>
				<div class="ep" title="Drag a new Out-Connection from here" ng-if="!dataSource.ReadOnly">
					<span class="glyphicon glyphicon-plus-sign"></span>
				</div>
				<div class="delete glyphicon glyphicon-remove" title="Delete this DataSource" ng-click="remove($index)" ng-if="!dataSource.ReadOnly"></div>
			</div>
		</div>
		<div class="actions panel panel-default">
			<div class="panel-heading">
				<span class="pull-left">Actions</span>
				<a href="http://2sxc.org/help" class="btn btn-info btn-xs pull-right" target="_blank"><span class="glyphicon glyphicon-question-sign"></span> Help</a>
			</div>
			<div class="panel-body">
				<button type="button" class="btn btn-primary btn-block" ng-disabled="readOnly" ng-click="savePipeline()"><span class="glyphicon glyphicon-floppy-save"></span> Save</button>
				<select class="form-control" ng-model="addDataSourceType" ng-disabled="readOnly" ng-change="addDataSource()" ng-options="d.ClassName for d in pipelineData.InstalledDataSources | filter: {allowNew: '!false'} | orderBy: 'ClassName'">
					<option value="">-- Add DataSource --</option>
				</select>
				<button type="button" class="btn btn-default btn-sm" title="Query the Data of this Pipeline" ng-click="queryPipeline()"><span class="glyphicon glyphicon-play"></span> Query</button>
				<button type="button" class="btn btn-default btn-sm" title="Clone this Pipeline with all DataSources and Configurations" ng-click="clonePipeline()" ng-disabled="!PipelineEntityId"><span class="glyphicon glyphicon-share-alt"></span> Clone</button>
				<button type="button" class="btn btn-default btn-sm" ng-click="editPipelineEntity()"><span class="glyphicon glyphicon-pencil"></span> Test Parameters</button>
				<button type="button" class="btn btn-info btn-xs" ng-click="toggleEndpointOverlays()"><span class="glyphicon glyphicon-info-sign"></span> {{showEndpointOverlays ? 'Hide' : 'Show' }} Overlays</button>
				<button type="button" class="btn btn-info btn-xs" ng-click="repaint()"><span class="glyphicon glyphicon-repeat"></span> Repaint</button>
				<button type="button" class="btn btn-info btn-xs" ng-click="toogleDebug()"><span class="glyphicon glyphicon-info-sign"></span> {{debug ? 'Hide' : 'Show'}} Debug Info</button>
			</div>
		</div>
		<toaster-container></toaster-container>
		<pre ng-if="debug">{{pipelineData | json}}</pre>
	</div>
</div>
