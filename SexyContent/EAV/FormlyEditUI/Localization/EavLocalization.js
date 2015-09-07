
(function () {
	'use strict';


	/* This app handles all aspectes of the multilanguage features of the field templates */

	var eavLocalization = angular.module('eavLocalization', ['formly'], function (formlyConfigProvider) {

		// Field templates that use this wrapper must bind to value.Value instead of model[...]
		formlyConfigProvider.setWrapper([
			{
				name: 'eavLocalization',
				templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/Localization/FormlyLocalizationWrapper.html'
			}
		]);

	});

	eavLocalization.directive('eavLanguageSwitcher', function () {
		return {
			restrict: 'E',
			template: '<ul class="nav nav-pills" style="margin-left:0;"><li ng-repeat="l in langConf.languages" ng-class="{ active: langConf.currentLanguage == l.key }" ><a ng-click="langConf.currentLanguage = l.key;" href="javascript:void(0);">{{l.name}}</a></li></ul>',
			controller: function($scope, eavLanguageService) {
				$scope.langConf = eavLanguageService;
			}
		};
	});

	eavLocalization.factory('eavLanguageService', function (sxc) {
		return sxc._editContentGroupConfig.langConf;
	});

	eavLocalization.directive('eavLocalizationScopeControl', function () {
		return {
			restrict: 'E',
			transclude: true,
			template: '',
			link: function (scope, element, attrs) {
			},
			controller: function ($scope, $filter, eavDefaultValueService, eavLanguageService) { // Can't use controllerAs because of transcluded scope

				var scope = $scope;
				var langConf = eavLanguageService;

				var initCurrentValue = function() {

					// Set base value object if not defined
					if (!scope.model[scope.options.key])
						scope.model[scope.options.key] = { Values: [] };

					var fieldModel = scope.model[scope.options.key];

					// If current language = default language and there are no values, create an empty value object
					if (langConf.currentLanguage == langConf.defaultLanguage) {
						if (fieldModel.Values.length == 0) {
							var defaultValue = eavDefaultValueService(scope.options);
							fieldModel.Values.push({ Value: defaultValue, Dimensions: {} });
							fieldModel.Values[0].Dimensions[langConf.currentLanguage] = true; // Assign default language dimension
						}
					}

					var valueToEdit;

					// Decide which value to edit:
					// 1. If there is a value with current dimension on it, use it
					valueToEdit = $filter('filter')(fieldModel.Values, function(v, i) {
						return v.Dimensions[langConf.currentLanguage] != null;
					})[0];

					// 2. Use default language value
					if (valueToEdit == null)
						valueToEdit = $filter('filter')(fieldModel.Values, function(v, i) {
							return v.Dimensions[langConf.defaultLanguage] != null;
						})[0];

					// 3. Use the first value if there is only one
					if (valueToEdit == null) {
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
					if (oldValue == null || newValue == oldValue)
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
			}
		};
	});

	eavLocalization.directive('eavLocalizationMenu', function() {
		return {
			restrict: 'E',
			scope: {
				fieldModel: '=fieldModel',
				options: '=options'
			},
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/Localization/LocalizationMenu.html',
			link: function (scope, element, attrs) { },
			controllerAs: 'vm',
			controller: function ($scope, eavLanguageService) {
				var vm = this;
				var langConf = eavLanguageService;
				vm.fieldModel = $scope.fieldModel;
				vm.isDefaultLanguage = function() { return langConf.currentLanguage != langConf.defaultLanguage; };

				vm.actions = {
					translate: function () {
						var value = { Value: 'New translated value!', Dimensions: {} };
						value.Dimensions[langConf.currentLanguage] = true;
						vm.fieldModel.Values.push(value);
					}
				};

			}
		};
	});

})();