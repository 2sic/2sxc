/* global angular */
(function () {
	'use strict';

	var app = angular.module('eavEditEntity', ['formly', 'eavFieldTemplates', 'sxcFieldTemplates', '2sxc4ng']);

	// Main directive that renders an entity edit form
	app.directive('eavEditEntity', function() {
		return {
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/EditEntity.html',
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
	app.controller('EditEntityCtrl', function editEntityCtrl($http, $scope, formlyConfig, eavLanguageService) {
		
		var vm = this;
		vm.editInDefaultLanguageFirst = function() {
			return eavLanguageService.currentLanguage != eavLanguageService.defaultLanguage && !$scope.entityId;
		};

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
						//defaultValue: parseDefaultValue(e)
						expressionProperties: {
							'templateOptions.disabled': 'options.templateOptions.disabled' // Needed for dynamic update of the disabled property
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

	});

	app.service('eavDefaultValueService', function () {
		// Returns a typed default value from the string representation
		return function parseDefaultValue(fieldConfig) {
			var e = fieldConfig;
			var d = e.templateOptions.settings.DefaultValue;

			switch (e.type.split('-')[0]) {
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
		};
	});

})();