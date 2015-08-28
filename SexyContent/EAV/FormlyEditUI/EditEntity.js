/* global angular */
(function () {
	'use strict';

	var app = angular.module('eavEditEntity', ['formly', 'eavFieldTemplates', 'sxcFieldTemplates', '2sxc4ng']);

	// Main directive that renders an entity edit form
	app.directive('eavEditEntity', function() {
		return {
			template: '<formly-form ng-submit="vm.onSubmit()" form="vm.form" model="vm.entity.Attributes" fields="vm.formFields"></formly-form><a ng-click="vm.showDebug = !vm.showDebug;">Debug</a><div ng-if="vm.showDebug"><h3>Debug</h3><pre>{{vm.entity | json}}</pre><pre>{{vm.debug | json}}</pre><pre>{{vm.formFields | json}}</pre></div>',
			restrict: 'E',
			scope: {
				contentTypeName: '@contentTypeName',
				entityId: '@entityId',
				registerEditControl: '=registerEditControl',
				langConf: '=langConf'
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
		};

		// The control object is available outside the directive
		// Place functions here that should be available from the parent of the directive
		vm.control = {
			isValid: function() { return vm.form.$valid; },
			save: vm.save
		};

		// Register this control in the parent control
		$scope.registerEditControl(vm.control);

		vm.model = null;
		vm.entity = null;

		vm.formFields = null;

		$http.get('eav/ContentType/GetContentTypeConfiguration?contentTypeName=' + encodeURIComponent($scope.contentTypeName))
			.then(function(result) {
				vm.debug = result;

				// Transform EAV content type configuration to formFields (formly configuration)
				angular.forEach(result.data, function(e, i) {
					vm.formFields.push({
						key: e.StaticName,
						type: getType(e),
						templateOptions: {
							required: !!e.MetaData.Required,
							label: e.MetaData.Name,
							description: e.MetaData.Notes,
							settings: e.MetaData,
							langConf: $scope.langConf
						},
						hide: (e.MetaData.VisibleInEditUI ? !e.MetaData.VisibleInEditUI : false),
						//defaultValue: parseDefaultValue(e)
						expressionProperties: {
							'templateOptions.disabled': function ($viewValue, $modelValue, scope) {
								return scope.disabled;
							}
						}
					});
				});

				if ($scope.entityId) {
					$http.get('eav/Entity/GetEntity?entityId=' + $scope.entityId)
						.then(function(result) {
							vm.entity = result.data;
						});
				} else {
					// ToDo: Create new / blank model should probably not be here
					vm.entity = {
						Id: null,
						Guid: null,
						Type: {
							Name: $scope.contentTypeName
						},
						Attributes: {}
					};
				}

			});

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
		function parseDefaultValue(attributeConfiguration) {
			var e = attributeConfiguration;
			var d = e.MetaData.DefaultValue;

			switch (e.Type.toLowerCase()) {
				case 'boolean':
					return d != null ? d.toLowerCase() == 'true' : false;
				case 'datetime':
					return d != null ? new Date(d) : null;
				case 'entity':
					return [];
				case 'number':
					return null;
				default:
					return d ? d : "";
			}
		}

	});

})();