
(function () {
	'use strict';

	// ToDo: Use correct language configuration from DNN / 2sxc
	var langConfig = {
		languages: ['en-us', 'de-de'],
		currentLanguage: 'de-de',
		defaultLanguage: 'en-us'
	};


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
	
	//eavLocalization.directive('eavLanguageSwitcher')

	eavLocalization.directive('eavLocalizationScopeControl', function () {
		return {
			restrict: 'E',
			transclude: true,
			template: '',
			link: function (scope, element, attrs) {
			},
			controller: function ($scope, $filter) { // Can't use controllerAs because of transcluded scope

				var scope = $scope;

				// Set base value object if not defined
				if (!scope.model[scope.options.key])
					scope.model[scope.options.key] = { Values: [] };

				var fieldModel = scope.model[scope.options.key];

				// If current language = default language and there are no values, create an empty value object
				if (langConfig.currentLanguage == langConfig.defaultLanguage) {
					if (fieldModel.Values.length == 0) {
						fieldModel.Values.push({ Value: null, Dimensions: {} });
						fieldModel.Values[0].Dimensions[langConfig.currentLanguage] = true; // Assign default language dimension
					}
				}

				var valueToEdit;

				// Decide which value to edit:
				// 1. If there is a value with current dimension on it, use it
				valueToEdit = $filter('filter')(fieldModel.Values, function (v, i) {
					return v.Dimensions[langConfig.currentLanguage] != null;
				})[0];

				// 2. Use default language value
				if(valueToEdit == null)
					valueToEdit = $filter('filter')(fieldModel.Values, function (v, i) {
						return v.Dimensions[langConfig.defaultLanguage] != null;
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
				var writable = (langConfig.currentLanguage == langConfig.defaultLanguage) ||
					 (scope.value && scope.value.Dimensions[langConfig.currentLanguage]);

				scope.disabled = !writable;
			}
		};
	});

	eavLocalization.directive('eavLocalizationMenu', function() {
		return {
			restrict: 'E',
			scope: {
				value: '=value'
			},
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/Localization/LocalizationMenu.html',
			link: function (scope, element, attrs) { },
			controllerAs: 'vm',
			controller: function ($scope) {
				var vm = this;
				vm.value = $scope.value;

				vm.state = {
					writable: function() {
						return true;
					}
				};

			}
		};
	});

})();