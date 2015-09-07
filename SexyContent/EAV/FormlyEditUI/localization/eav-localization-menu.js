
(function () {
	'use strict';

	angular.module('eavLocalization').directive('eavLocalizationMenu', function () {
		return {
			restrict: 'E',
			scope: {
				fieldModel: '=fieldModel',
				options: '=options'
			},
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/localization/localization-menu.html',
			link: function (scope, element, attrs) { },
			controllerAs: 'vm',
			controller: function ($scope, eavLanguageService) {
				var vm = this;
				var langConf = eavLanguageService;
				vm.fieldModel = $scope.fieldModel;
				vm.isDefaultLanguage = function () { return langConf.currentLanguage != langConf.defaultLanguage; };

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