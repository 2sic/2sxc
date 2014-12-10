var pipelineDesigner = angular.module('pipelineDesinger', ['pipelineDesinger.filters', 'ngResource', 'toaster', 'eavGlobalConfigurationProvider', 'eavDialogService', 'pipelineFactory']);

pipelineDesigner.config(['$locationProvider', function ($locationProvider) {
	$locationProvider.html5Mode(true);
}]);

// datasource directive makes an element a DataSource with jsPlumb
pipelineDesigner.directive('datasource', function ($timeout) {
	return {
		restrict: 'A',
		link: function (scope, element) {
			// make this a DataSource when the DOM is ready
			$timeout(function () {
				scope.makeDataSource(scope.dataSource, element);
			});
			if (scope.$last === true) {
				$timeout(function () {
					scope.$emit('ngRepeatFinished');
				});
			}
		}
	}
});

// Filters for "ClassName, AssemblyName"
angular.module('pipelineDesinger.filters', []).filter('typename', function () {
	return function (input, format) {
		var globalParts = input.match(/[^,\s]+/g);

		switch (format) {
			case 'classFullName':
				if (globalParts)
					return globalParts[0];
			case 'className':
				if (globalParts) {
					var classFullName = globalParts[0].match(/[^\.]+/g);
					return classFullName[classFullName.length - 1];
				}
		}

		return input;
	};
});