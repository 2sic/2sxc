<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PipelineManagement.ascx.cs" Inherits="ToSic.Eav.ManagementUI.EAV.PipelineManagement" %>
<div ng-app="pipelineManagement" class="ng-cloak">
	<div ng-controller="PipelineManagementController">
		<a ng-href="{{getPipelineUrl('new')}}" target="_self" class="btn btn-default">
			<span class="glyphicon glyphicon-plus"></span> New
		</a>
		<button type="button" class="btn btn-default" ng-click="refresh()">
			<span class="glyphicon glyphicon-repeat"></span> Refresh
		</button>
		<a target="_self" class="btn btn-default" ng-if="returnUrl" ng-href="{{returnUrl}}">
			<span class="glyphicon glyphicon-arrow-left"></span> Back
		</a>
		<table class="table table-striped table-hover">
			<thead>
				<tr>
					<th>Name</th>
					<th>ID</th>
					<th>Description</th>
					<th>Actions</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="pipeline in pipelines | orderBy:'Name'">
					<td>{{pipeline.Name}}</td>
					<td>{{pipeline.Id}}</td>
					<td>{{pipeline.Description}}</td>
					<td>
						<a class="btn btn-xs btn-default" target="_blank" ng-href="{{getPipelineUrl('design', pipeline.Id)}}">
							<span class="glyphicon glyphicon-random"></span> Open Designer
						</a>
						<a class="btn btn-xs btn-default" target="_blank" ng-href="{{getPipelineUrl('permissions', pipeline.Guid)}}">
							<span class="glyphicon glyphicon-lock"></span> Permissions
						</a>
						<button type="button" class="btn btn-xs btn-default" ng-click="clone(pipeline)">
							<span class="glyphicon glyphicon-share-alt"></span> Clone
						</button>
						<button type="button" class="btn btn-xs btn-danger" ng-click="delete(pipeline)">
							<span class="glyphicon glyphicon-remove"></span> Delete
						</button>
					</td>
				</tr>
				<tr ng-if="!pipelines.length">
					<td colspan="100">No Items</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>
