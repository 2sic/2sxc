// Config and Controller for the Pipeline Management UI
angular.module('pipelineManagement', ['pipelineManagementFactory']).
	config(function ($locationProvider) {
		$locationProvider.html5Mode(true);
	}).
	controller('pipelineManagementController', function ($rootScope, $scope, $location, $window, pipelineManagementFactory) {
		// Init
		$scope.AppId = $location.search().AppId;
		if (!$scope.AppId)
			throw 'Please specify an AppId';
		pipelineManagementFactory.setAppId($scope.AppId);
		// Make URL-Provider available to the scope
		$scope.getPipelineUrl = pipelineManagementFactory.getPipelineUrl;

		// Refresh List of Pipelines
		$scope.refresh = function () {
			$scope.pipelines = pipelineManagementFactory.getPipelines($scope.AppId);
		};
		$scope.refresh();

		// Set Return URL
		if ($location.search().ReturnUrl)
			$scope.returnUrl = decodeURIComponent($location.search().ReturnUrl);
	});


// PipelineManagementFactory provides an interface to the Server Backend storing Pipelines
angular.module('pipelineManagementFactory', ['ngResource', 'eavGlobalConfigurationProvider']).
	factory('pipelineManagementFactory', function ($resource, $http, eavGlobalConfigurationProvider) {
		'use strict';

		// Web API Service
		var entitiesResource = $resource(eavGlobalConfigurationProvider.api.baseUrl + '/EAV/Entities/:action');
		// Add additional Headers to each http-Request
		angular.extend($http.defaults.headers.common, eavGlobalConfigurationProvider.api.additionalHeaders);

		var dataPipelineAttributeSetId;
		var appId;

		return {
			// set AppId and init some dynamic configurations
			setAppId: function (newAppId) {
				appId = newAppId;
				entitiesResource.get({ action: 'GetContentType', appId: appId, name: 'DataPipeline' }, function (success) {
					dataPipelineAttributeSetId = success.AttributeSetId;
				});
			},
			getPipelines: function () {
				return entitiesResource.query({ action: 'GetEntities', appId: appId, typeName: 'DataPipeline' });
			},
			getPipelineUrl: function (mode, pipeline) {
				switch (mode) {
					case 'new':
						return eavGlobalConfigurationProvider.itemForm.getUrl('New', { AttributeSetId: dataPipelineAttributeSetId, AssignmentObjectTypeId: 4 });
					case 'edit':
						return eavGlobalConfigurationProvider.itemForm.getUrl('Edit', { EntityId: pipeline.EntityId });
					case 'design':
						return eavGlobalConfigurationProvider.pipelineDesigner.getUrl(appId, pipeline.EntityId);
				}
			}
		}
	});