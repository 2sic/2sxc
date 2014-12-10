// Config and Controller for the Pipeline Management UI
angular.module('pipelineManagement', ['pipelineFactory', 'eavGlobalConfigurationProvider', 'ngResource']).
	config(function ($locationProvider) {
		$locationProvider.html5Mode(true);
	}).
	controller('PipelineManagementController', function ($rootScope, $scope, $location, $window, pipelineFactory) {
		// Init
		$scope.AppId = $location.search().AppId;
		if (!$scope.AppId)
			throw 'Please specify an AppId';
		pipelineFactory.setAppId($scope.AppId);
		pipelineFactory.initContentTypes();
		// Make URL-Provider available to the scope
		$scope.getPipelineUrl = pipelineFactory.getPipelineUrl;

		// Refresh List of Pipelines
		$scope.refresh = function () {
			$scope.pipelines = pipelineFactory.getPipelines($scope.AppId);
		};
		$scope.refresh();

		// Set Return URL
		if ($location.search().ReturnUrl)
			$scope.returnUrl = decodeURIComponent($location.search().ReturnUrl);

		// Delete a Pipeline
		$scope.delete = function (pipeline) {
			if (!confirm('Delete Pipeline "' + pipeline.Name + '" (' + pipeline.EntityId + ')?'))
				return;

			pipelineFactory.deletePipeline(pipeline.EntityId).then(function () {
				$scope.refresh();
			}, function (reason) {
				alert(reason);
			});
		}

		// Clone a Pipeline
		$scope.clone = function (pipeline) {
			pipelineFactory.clonePipeline(pipeline.EntityId).then(function () {
				$scope.refresh();
			}, function (reason) {
				alert(reason);
			});
		}
	});