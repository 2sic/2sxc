/* global angular */
(function () {
	'use strict';

	var app = angular.module('eavEditEntity', ['formly', 'eavFieldTemplates', 'sxcFieldTemplates', '2sxc4ng']);

	// Main directive that renders an entity edit form
	app.directive('eavEditEntity', function() {
		return {
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/edit-entity.html',
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
		vm.editInDefaultLanguageFirst = function () {
			return false; //eavLanguageService.currentLanguage != eavLanguageService.defaultLanguage && !$scope.entityId;
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

		$http.get('eav/ContentType/GetFields?staticName=' + encodeURIComponent($scope.contentTypeName))
			.then(function(result) {
				vm.debug = result;

				// Transform EAV content type configuration to formFields (formly configuration)
				angular.forEach(result.data, function (e, i) {

					console.log(e);

					vm.formFields.push({
						key: e.StaticName,
						type: getType(e),
						templateOptions: {
							required: !!e.Metadata.All.Required,
							label: e.Metadata.All.Name,
							description: e.Metadata.All.Notes,
							settings: e.Metadata
						},
						hide: (e.Metadata.All.VisibleInEditUI ? !e.Metadata.All.VisibleInEditUI : false),
						//defaultValue: parseDefaultValue(e)
						expressionProperties: {
							'templateOptions.disabled': 'options.templateOptions.disabled' // Needed for dynamic update of the disabled property
						}
					});
				});
			});

		// Load existing entity if defined
		if ($scope.entityId) {
			$http.get('eav/Entity/GetEntity?entityId=' + $scope.entityId)
				.then(function (result) {
					vm.entity = result.data;
				});
		} else {
			// ToDo: Create new / blank model should probably not be here (EntityService?)
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
	var getType = function (attributeConfiguration) {
		var e = attributeConfiguration;
		var type = e.Type.toLowerCase();
		var subType = e.Metadata.All.InputType;

		subType = subType ? subType.toLowerCase() : null;

		// Special case: override subtype for string-textarea
		if (type == 'string' && e.Metadata.All.RowCount > 1)
			subType = 'textarea';

		// Use subtype 'default' if none is specified - or type does not exist
		if (!subType || !formlyConfig.getType(type + '-' + subType))
			subType = 'default';

		return (type + '-' + subType);
	}

})();