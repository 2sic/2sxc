/* global angular */
(function () {
	'use strict';

	var app = angular.module('eavEditEntity', ['formly', 'eavFieldTemplates', 'sxcFieldTemplates', '2sxc4ng']);

	// Main directive that renders an entity edit form
	app.directive('eavEditEntity', function() {
		return {
			template: '<formly-form ng-submit="vm.onSubmit()" form="vm.form" model="vm.model" fields="vm.formFields"></formly-form><h3>Debug</h3><pre>{{vm.model | json}}</pre><!--<pre>{{vm.debug | json}}</pre><pre>{{vm.formFields | json}}</pre>-->',
			restrict: 'E',
			scope: {
				contentTypeName: '@contentTypeName',
				entityId: '@entityId',
				registerEditControl: '=registerEditControl'
			},
			controller: 'EditEntityCtrl',
			controllerAs: 'vm'
		};
	});

	// The controller for the main form directive
	app.controller('EditEntityCtrl', function editEntityCtrl($http, $scope, formlyConfig) {
		
		var vm = this;

		vm.save = function() {
			alert("Saving not implemented yet!");
			console.log(vm.model);
		};

		// The control object is available outside the directive
		// Place functions here that should be available from the parent of the directive
		vm.control = {
			isValid: function() { return vm.form.$valid; },
			save: vm.save
		};

		// Register this control in the parent control
		$scope.registerEditControl(vm.control);

		vm.model = {};

		vm.formFields = null;

		$http.get('eav/ContentType/GetContentTypeConfiguration?appId=1&zoneId=1&contentTypeName=' + encodeURIComponent($scope.contentTypeName))
		.then(function (result) {
			vm.debug = result;

			// Transform EAV content type configuration to formFields (formly configuration)
			angular.forEach(result.data, function (e, i) {
				vm.formFields.push({
					key: e.StaticName,
					type: getType(e),
					templateOptions: {
						required: !!e.MetaData.Required,
						label: e.MetaData.Name,
						description: e.MetaData.Notes,
						settings: e.MetaData
					},
					hide: (e.MetaData.VisibleInEditUI ? !e.MetaData.VisibleInEditUI : false),
					defaultValue: convertDefaultValue(e)
				});
			});

		});

		if ($scope.entityId) {
			$http.get('eav/Entity/GetEntity?appId=1&zoneId=1&entityId=' + $scope.entityId)
				.then(function(result) {
					vm.model = result.data;
				});
		}

		// Returns the field type for an attribute configuration
		function getType(attributeConfiguration) {
			var e = attributeConfiguration;
			var type = e.Type.toLowerCase();
			var subType = e.MetaData.InputType;

			subType = subType ? subType.toLowerCase() : null;

			// Special case: override subtype for string-textarea
			if (type == 'string' && e.MetaData.RowCount > 1)
				subType = 'textarea';

			// Use subtype 'default' if none is specified - or type does not exist
			if (!subType || !formlyConfig.getType(type + '-' + subType))
				subType = 'default';

			return (type + '-' + subType);
		}

		// Returns a typed default value from the string representation
		function convertDefaultValue(attributeConfiguration) {
			var e = attributeConfiguration;

			if (!e.MetaData.DefaultValue)
				return null;

			switch (e.Type.toLowerCase()) {
				case 'boolean':
					return e.MetaData.DefaultValue.toLowerCase() == 'true';
				case 'datetime':
					return new Date(e.MetaData.DefaultValue);
				default:
					return e.MetaData.DefaultValue;
			}
		}

	});

})();