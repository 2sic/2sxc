/* global angular */
(function () {
	'use strict';

	var app = angular.module('eavEditEntity', ['formly', 'eavFieldTemplates', 'eavNgSvcs', "EavServices" /* 'ContentTypeFieldServices' */, 'eavEditTemplates']);

	// Main directive that renders an entity edit form
	app.directive('eavEditEntity', function() {
		return {
			templateUrl: 'edit-entity.html',
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
	app.controller('EditEntityCtrl', ["appId", "$http", "$scope", "formlyConfig", "contentTypeFieldSvc", "entitiesSvc", function editEntityCtrl(appId, $http, $scope, formlyConfig, contentTypeFieldSvc, entitiesSvc) {

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
		if($scope.registerEditControl)
			$scope.registerEditControl(vm.control);

		vm.model = null;
		vm.entity = null;

		vm.formFields = null;


		var loadContentType = function () {

		    contentTypeFieldSvc(appId, { StaticName: vm.entity.Type.Name }).getFields()
			.then(function (result) {
			    vm.debug = result;

			    // Transform EAV content type configuration to formFields (formly configuration)
			    angular.forEach(result.data, function (e, i) {

			        if (e.Metadata.All === undefined)
			            e.Metadata.All = {};

			        vm.formFields.push({
			            key: e.StaticName,
			            type: getType(e),
			            templateOptions: {
			                required: !!e.Metadata.All.Required,
			                label: e.Metadata.All.Name === undefined ? e.StaticName : e.Metadata.All.Name,
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
		};

	    // Load existing entity if defined
		
		if (!!$scope.entityId) {
		    entitiesSvc.getMultiLanguage(appId, $scope.contentTypeName, $scope.entityId)
                .then(function (result) {
                    vm.entity = result.data;
                    loadContentType();
                });
		} else {
		    vm.entity = entitiesSvc.newEntity($scope.contentTypeName);
		    loadContentType();
		}

		// Returns the field type for an attribute configuration
		var getType = function(attributeConfiguration) {
			var e = attributeConfiguration;
			var type = e.Type.toLowerCase();
			var subType = e.Metadata.String !== undefined ? e.Metadata.String.InputType : null;

			subType = subType ? subType.toLowerCase() : null;

			// Special case: override subtype for string-textarea
			if (type === 'string' && e.Metadata.String !== undefined && e.Metadata.String.RowCount > 1)
				subType = 'textarea';

			// Use subtype 'default' if none is specified - or type does not exist
			if (!subType || !formlyConfig.getType(type + '-' + subType))
				subType = 'default';

			return (type + '-' + subType);
		};
	}]);
    
	

})();

(function() {
	'use strict';

	/* This app registers all field templates for EAV in the angularjs eavFieldTemplates app */

	var eavFieldTemplates = angular.module('eavFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap', 'eavLocalization'], ["formlyConfigProvider", function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: 'string-default',
			template: '<input class="form-control" ng-model="value.Value">',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization']
		});

		formlyConfigProvider.setType({
			name: 'string-dropdown',
			template: '<select class="form-control" ng-model="value.Value"></select>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			defaultOptions: function defaultOptions(options) {
				
				// DropDown field: Convert string configuration for dropdown values to object, which will be bound to the select
				if (!options.templateOptions.options && options.templateOptions.settings.String.DropdownValues) {
					var o = options.templateOptions.settings.String.DropdownValues;
					o = o.replace('\r', '').split('\n');
					o = o.map(function (e, i) {
						var s = e.split(':');
						return {
							name: s[0],
							value: s[1] ? s[1] : s[0]
						};
					});
					options.templateOptions.options = o;
				}

				function _defineProperty(obj, key, value) { return Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); }

				var ngOptions = options.templateOptions.ngOptions || 'option[to.valueProp || \'value\'] as option[to.labelProp || \'name\'] group by option[to.groupProp || \'group\'] for option in to.options';
				return {
					ngModelAttrs: _defineProperty({}, ngOptions, {
						value: 'ng-options'
					})
				};

			}
		});

		formlyConfigProvider.setType({
			name: 'string-textarea',
			template: '<textarea class="form-control" ng-model="value.Value"></textarea>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			defaultOptions: {
				ngModelAttrs: {
					'{{to.settings.String.RowCount}}': { value: 'rows' },
					cols: { attribute: 'cols' }
				}
			}
		});

		formlyConfigProvider.setType({
			name: 'number-default',
			template: '<input type="number" class="form-control" ng-model="value.Value">{{vm.isGoogleMap}}',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			defaultOptions: {
				ngModelAttrs: {
					'{{to.settings.Number.Min}}': { value: 'min' },
					'{{to.settings.Number.Max}}': { value: 'max' },
					'{{to.settings.Number.Decimals ? "^[0-9]+(\.[0-9]{1," + to.settings.Number.Decimals + "})?$" : null}}': { value: 'pattern' }
				}
			},
			controller: 'FieldTemplate-NumberCtrl as vm'
		});

		formlyConfigProvider.setType({
			name: 'boolean-default',
			template: "<div class=\"checkbox\">\n\t<label>\n\t\t<input type=\"checkbox\"\n           class=\"formly-field-checkbox\"\n\t\t       ng-model=\"value.Value\">\n\t\t{{to.label}}\n\t\t{{to.required ? '*' : ''}}\n\t</label>\n</div>\n",
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization']
		});

		formlyConfigProvider.setType({
			name: 'datetime-default',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			template: '<div><div class="input-group"><div class="input-group-addon" style="cursor:pointer;" ng-click="to.isOpen = true;"><i class="glyphicon glyphicon-calendar"></i></div><input class="form-control" ng-model="value.Value" is-open="to.isOpen" datepicker-options="to.datepickerOptions" datepicker-popup /></div>' +
				 '<timepicker ng-show="to.settings.DateTime.UseTimePicker" ng-model="value.Value" show-meridian="ismeridian"></timepicker></div>',
			defaultOptions: {
				templateOptions: {
					datepickerOptions: {},
					datepickerPopup: 'dd.MM.yyyy'
				}
			}
		});
	}]);

	eavFieldTemplates.controller('FieldTemplate-NumberCtrl', function () {
		var vm = this;
		vm.isGoogleMap = "test";
	});

})();
angular.module('eavEditTemplates',[]).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('edit-entity.html',
    "<div ng-show=vm.editInDefaultLanguageFirst()>Please edit this in the default language first.</div><div ng-show=!vm.editInDefaultLanguageFirst()><formly-form ng-submit=vm.onSubmit() form=vm.form model=vm.entity.Attributes fields=vm.formFields></formly-form><a ng-click=\"vm.showDebug = !vm.showDebug;\">Debug</a><div ng-if=vm.showDebug><h3>Debug</h3><pre>{{vm.entity | json}}</pre><pre>{{vm.debug | json}}</pre><pre>{{vm.formFields | json}}</pre></div></div>"
  );


  $templateCache.put('fields/templates/entity-default.html',
    "<div class=eav-entityselect><div ui-tree=options data-empty-place-holder-enabled=false><ol ui-tree-nodes ng-model=model[options.key]><li ng-repeat=\"item in model[options.key]\" ui-tree-node class=eav-entityselect-item><div ui-tree-handle><span title=\"{{getEntityText(item) + ' (' + item + ')'}}\">{{getEntityText(item)}}</span> <a data-nodrag title=\"Remove this item\" ng-click=remove(this) class=eav-entityselect-item-remove>[remove]</a></div></li></ol></div><select class=\"eav-entityselect-selector form-control\" ng-model=selectedEntity ng-change=addEntity() ng-show=\"to.settings.Entity.AllowMultiValue || model[options.key].length < 1\"><option value=\"\">-- choose --</option><option value=new ng-if=createEntityAllowed()>-- new --</option><option ng-repeat=\"item in availableEntities\" ng-disabled=\"model[options.key].indexOf(item.Value) != -1\" value={{item.Value}}>{{item.Text}}</option></select></div>"
  );


  $templateCache.put('localization/formly-localization-wrapper.html',
    "<eav-localization-scope-control></eav-localization-scope-control><div ng-if=value><eav-localization-menu field-model=model[options.key] options=options></eav-localization-menu><formly-transclude></formly-transclude></div><p class=bg-info style=padding:12px ng-if=!value>Please create the value for <i>'{{to.label}}'</i> in the default language before translating it.</p>"
  );


  $templateCache.put('localization/language-switcher.html',
    "<ul class=\"nav nav-pills\" style=margin-left:0><li ng-repeat=\"l in languages.languages\" ng-class=\"{ active: languages.currentLanguage == l.key }\"><a ng-click=\"languages.currentLanguage = l.key;\" href=javascript:void(0);>{{l.name}}</a></li></ul>"
  );


  $templateCache.put('localization/localization-menu.html',
    "<div dropdown is-open=status.isopen class=eav-localization><a class=eav-localization-lock ng-if=vm.isDefaultLanguage() ng-class=\"{ 'eav-localization-lock-open': !options.templateOptions.disabled }\" dropdown-toggle href=javascript:void(0)><i class=\"glyphicon glyphicon-globe\"></i></a><ul class=\"dropdown-menu multi-level pull-right eav-localization-dropdown\" role=menu aria-labelledby=single-button><li role=menuitem><a ng-click=vm.actions.translate()>Translate</a></li><li role=menuitem><a href=#>Another action</a></li><li role=menuitem class=dropdown-submenu><a href=#>Something else here</a><ul class=dropdown-menu><li role=menuitem><a href=#>Action</a></li><li role=menuitem><a href=#>Another action</a></li></ul></li><li class=divider></li><li role=menuitem><a href=#>Separated link</a></li></ul></div>"
  );


  $templateCache.put('wrappers/edit-entity-wrapper.html',
    "<div class=modal-header><h3 class=modal-title>Edit entity</h3></div><div class=modal-body><div xng-controller=\"EditEntityWrapperCtrl as vm\"><eav-language-switcher></eav-language-switcher><eav-edit-entity content-type-name={{vm.contentTypeName}} entity-id={{vm.entityId}} register-edit-control=vm.registerEditControl></eav-edit-entity><button ng-disabled=!vm.isValid() ng-click=vm.save() class=\"btn btn-primary submit-button\">Save</button></div></div>"
  );

}]);


(function () {
	'use strict';


	/* This app handles all aspectes of the multilanguage features of the field templates */

	var eavLocalization = angular.module('eavLocalization', ['formly', "EavConfiguration"], ["formlyConfigProvider", function (formlyConfigProvider) {

		// Field templates that use this wrapper must bind to value.Value instead of model[...]
		formlyConfigProvider.setWrapper([
			{
				name: 'eavLocalization',
				templateUrl: 'localization/formly-localization-wrapper.html'
			}
		]);

	}]);

	eavLocalization.directive('eavLanguageSwitcher', function () {
		return {
			restrict: 'E',
			templateUrl: 'localization/language-switcher.html',
			controller: ["$scope", "languages", function($scope, languages) {
				$scope.languages = languages;
			}]
		};
	});

	//eavLocalization.factory('eavLanguageService', function (languages) {
		//return {
		//	languages: [{ key: 'en-us', name: 'English (United States)' }],
		//	defaultLanguage: 'en-us',
		//	currentLanguage: 'en-us'
		//};
	//});

	eavLocalization.directive('eavLocalizationScopeControl', function () {
		return {
			restrict: 'E',
			transclude: true,
			template: '',
			link: function (scope, element, attrs) {
			},
			controller: ["$scope", "$filter", "eavDefaultValueService", "languages", function ($scope, $filter, eavDefaultValueService, languages) { // Can't use controllerAs because of transcluded scope

				var scope = $scope;
				var langConf = languages;

				var initCurrentValue = function() {

					// Set base value object if not defined
					if (!scope.model[scope.options.key])
						scope.model[scope.options.key] = { Values: [] };

					var fieldModel = scope.model[scope.options.key];

					// If current language = default language and there are no values, create an empty value object
					if (langConf.currentLanguage == langConf.defaultLanguage) {
						if (fieldModel.Values.length === 0) {
							var defaultValue = eavDefaultValueService(scope.options);
							fieldModel.Values.push({ Value: defaultValue, Dimensions: {} });
							fieldModel.Values[0].Dimensions[langConf.currentLanguage] = true; // Assign default language dimension
						}
					}

					var valueToEdit;

					// Decide which value to edit:
					// 1. If there is a value with current dimension on it, use it
					valueToEdit = $filter('filter')(fieldModel.Values, function(v, i) {
						return v.Dimensions[langConf.currentLanguage] !== undefined;
					})[0];

					// 2. Use default language value
					if (valueToEdit === undefined)
						valueToEdit = $filter('filter')(fieldModel.Values, function(v, i) {
							return v.Dimensions[langConf.defaultLanguage] !== undefined;
						})[0];

					// 3. Use the first value if there is only one
					if (valueToEdit === undefined) {
						if (fieldModel.Values.length > 1)
							throw "Default language value not found, but found multiple values - can't handle editing";
						// Use the first value
						valueToEdit = fieldModel.Values[0];
					}

					fieldModel._currentValue = valueToEdit;

					// Set scope variable 'value' to simplify binding
					scope.value = fieldModel._currentValue;

					// Decide whether the value is writable or not
					var writable = (langConf.currentLanguage == langConf.defaultLanguage) ||
					(scope.value && scope.value.Dimensions[langConf.currentLanguage]);

					scope.to.disabled = !writable;
				};

				initCurrentValue();

				// Handle language switch
				scope.langConf = langConf; // Watch does only work on scope variables
				scope.$watch('langConf.currentLanguage', function (newValue, oldValue) {
					if (oldValue === undefined || newValue == oldValue)
						return;
					initCurrentValue();
					console.log('switched language from ' + oldValue + ' to ' + newValue);
				});

				// ToDo: Could cause performance issues (deep watch array)...
				scope.$watch('model[options.key].Values', function(newValue, oldValue) {
					initCurrentValue();
				}, true);

				// The language menu must be able to trigger an update of the _currentValue property
				scope.model[scope.options.key]._initCurrentValue = initCurrentValue;
			}]
		};
	});

	eavLocalization.directive('eavLocalizationMenu', function() {
		return {
			restrict: 'E',
			scope: {
				fieldModel: '=fieldModel',
				options: '=options'
			},
			templateUrl: 'localization/localization-menu.html',
			link: function (scope, element, attrs) { },
			controllerAs: 'vm',
			controller: ["$scope", "languages", function ($scope, languages) {
				var vm = this;
				vm.fieldModel = $scope.fieldModel;
				vm.isDefaultLanguage = function () { return languages.currentLanguage != languages.defaultLanguage; };

				vm.actions = {
					translate: function () {
						var value = { Value: 'New translated value!', Dimensions: {} };
						value.Dimensions[languages.currentLanguage] = true;
						vm.fieldModel.Values.push(value);
					}
				};

			}]
		};
	});

})();
/* global angular */
(function () {
	'use strict';

	angular.module('eavEditEntity').service('eavDefaultValueService', function () {
		// Returns a typed default value from the string representation
		return function parseDefaultValue(fieldConfig) {
			var e = fieldConfig;
			var d = e.templateOptions.settings.DefaultValue;

			switch (e.type.split('-')[0]) {
				case 'boolean':
					return d !== undefined ? d.toLowerCase() == 'true' : false;
				case 'datetime':
					return d !== undefined ? new Date(d) : null;
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
/* global angular */
(function () {
	'use strict';

	var app = angular.module('eavEditEntity');

	// The controller for the main form directive
	app.controller('EditEntityWrapperCtrl', ["$http", "$scope", "contentTypeName", "entityId", function editEntityCtrl($http, $scope, contentTypeName, entityId) {

		var vm = this;
		vm.contentTypeName = contentTypeName;
		vm.entityId = entityId;
		
		vm.registeredControls = [];
		vm.registerEditControl = function (control) {
			vm.registeredControls.push(control);
		};

		vm.isValid = function () {
			var valid = true;
			angular.forEach(vm.registeredControls, function (e, i) {
				if (!e.isValid())
					valid = false;
			});
			return valid;
		};

		vm.save = function () {
			var savePromises = [];
			angular.forEach(vm.registeredControls, function (e, i) {
				savePromises.push(e.save());
			});
			$q.all(savePromises).then(function () {
				alert("All save promises resolved!");
			});
		};
	}]);

})();