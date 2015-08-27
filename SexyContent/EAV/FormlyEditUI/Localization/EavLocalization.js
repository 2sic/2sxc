
(function () {
	'use strict';

	/* This app handles all aspectes of the multilanguage features of the field templates */

	var eavLocalization = angular.module('eavLocalization', ['formly'], function (formlyConfigProvider) {

		formlyConfigProvider.setWrapper([
			{
				name: 'eavLocalization',
				templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/Localization/FormlyLocalizationWrapper.html'
			}
		]);

	});

	eavLocalization.directive('eavLocalizationScopeControl', function () {
		return {
			restrict: 'E',
			transclude: true,
			template: '',
			link: function(scope, element, attrs) {

				// Set base value object
				if (!scope.model[scope.options.key])
					scope.model[scope.options.key] = { Values: [] };

				// If current language is the default language, create an empty value object
				// ToDo: Handle non-default language case
				if (true) {
					if (scope.model[scope.options.key].Values.length == 0)
						scope.model[scope.options.key].Values.push({ Value: null, Dimensions: [] });
				}

				// Decide which value to edit
				// ToDo: Must care for the current language
				scope.model[scope.options.key]._currentValue = scope.model[scope.options.key].Values[0];

				// Set scope variable 'value' to simplify binding
				scope.value = scope.model[scope.options.key]._currentValue;
			}
			//controller: function($scope) {
			//	$scope.value = $scope.model[$scope.options.key]._currentValue;
			//}
		};
	});

	eavLocalization.directive('eavLocalizationMenu', function() {
		return {
			restrict: 'E',
			transclude: true,
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/Localization/LocalizationMenu.html',
			link: function (scope, element, attrs) { }
		};
	});

})();