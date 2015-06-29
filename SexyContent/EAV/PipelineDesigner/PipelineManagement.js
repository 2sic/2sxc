// Config and Controller for the Pipeline Management UI
angular.module('pipelineManagement', ['pipelineService', 'eavGlobalConfigurationProvider', 'ngResource']).
	config(function ($locationProvider) {
	    $locationProvider.html5Mode({
	        enabled: true,
	        requireBase: false
	    });
	}).
	controller('PipelineManagementController', function ($rootScope, $scope, $location, $window, pipelineService) {
	    // Init
	    $scope.AppId = $location.search().AppId;
	    if (!$scope.AppId)
	        throw 'Please specify an AppId';
	    pipelineService.setAppId($scope.AppId);
	    pipelineService.initContentTypes();
	    // Make URL-Provider available to the scope
	    $scope.getPipelineUrl = pipelineService.getPipelineUrl;

	    // Refresh List of Pipelines
	    $scope.refresh = function () {
	        $scope.pipelines = pipelineService.getPipelines($scope.AppId);
	    };
	    $scope.refresh();

	    // Set Return URL
	    if ($location.search().ReturnUrl)
	        $scope.returnUrl = decodeURIComponent($location.search().ReturnUrl);

	    // Delete a Pipeline
	    $scope.delete = function (pipeline) {
	        if (!confirm('Delete Pipeline "' + pipeline.Name + '" (' + pipeline.Id + ')?'))
	            return;

	        pipelineService.deletePipeline(pipeline.Id).then(function () {
	            $scope.refresh();
	        }, function (reason) {
	            alert(reason);
	        });
	    }

	    // Clone a Pipeline
	    $scope.clone = function (pipeline) {
	        pipelineService.clonePipeline(pipeline.Id).then(function () {
	            $scope.refresh();
	        }, function (reason) {
	            alert(reason);
	        });
	    }
	});