<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PipelineManagement.ascx.cs" Inherits="ToSic.Eav.ManagementUI.EAV.PipelineManagement" %>
<div ng-app="pipelineManagement" class="ng-cloak">
	<div ng-controller="pipelineManagementController">
		<a ng-href="{{getPipelineUrl('new')}}" target="_self" class="btn btn-default">
			<span class="glyphicon glyphicon-plus"></span> New
		</a>
		<button type="button" class="btn btn-default" ng-click="refresh()">
			<span class="glyphicon glyphicon-repeat"></span> Refresh
		</button>
		<table class="table table-striped table-hover">
			<thead>
				<tr>
					<th>ID</th>
					<th>Name</th>
					<th>Description</th>
					<th>Actions</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="pipeline in pipelines | orderBy:'Name'">
					<td>{{pipeline.EntityId}}</td>
					<td>{{pipeline.Name}}</td>
					<td>{{pipeline.Description}}</td>
					<td>
						<a class="btn btn-xs btn-default" target="_blank" ng-href="{{getPipelineUrl('design', pipeline)}}">
							<span class="glyphicon glyphicon-random"></span> Open Designer
						</a>
						<a class="btn btn-xs btn-default" target="_self" ng-href="{{getPipelineUrl('edit', pipeline)}}">
							<span class="glyphicon glyphicon-pencil"></span> Edit
						</a>
					</td>
				</tr>
				<tr ng-if="!pipelines.length">
					<td colspan="100">No Items</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>
