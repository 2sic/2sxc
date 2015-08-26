
(function () {
	'use strict';

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	var webFormsBridgeUrl = "/en-us/Homeö/ctl/webformsbridge/mid/4023/popUp/true";

	var app = angular.module('sxcFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap'], function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: 'string-wysiwyg',
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/FieldTemplates/Templates/string-wysiwyg.html', // ToDo: Use correct base path
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			controller: 'FieldTemplate-WysiwygCtrl as vm'
		});

		formlyConfigProvider.setType({
			name: 'hyperlink-default',
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/FieldTemplates/Templates/hyperlink-default.html',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			controller: 'FieldTemplate-HyperlinkCtrl as vm'
		});

	});

	app.controller('FieldTemplate-HyperlinkCtrl', function ($modal, $scope) {

		var vm = this;
		vm.modalInstance = null;
		vm.test = "...";
		vm.bridge = {
			valueChanged: function (value) {
				$scope.$apply(function() {
					$scope.model[$scope.options.key] = value;
					vm.modalInstance.close();
				});
			}
		};

		vm.openDialog = function(type) {
			vm.modalInstance = $modal.open({
				templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/FieldTemplates/Templates/hyperlink-default-' + type + '.html',
				resolve: {
					bridge: function() {
						return vm.bridge;
					}
				},
				controller: function($scope, bridge) {
					$scope.bridge = bridge;
				}
			});
		};

	});

	app.controller('FieldTemplate-WysiwygCtrl', function ($scope) {

		var vm = this;

		// Everything the WebForms bridge (iFrame) should have access to
		vm.bridge = {
			onChanged: function (newValue) {
				$scope.$apply(function () {
					$scope.model[$scope.options.key] = newValue;
				});
			},
			setValue: function() { alert('Error: setValue has no override'); }
		};

	});

	app.directive('webFormsBridge', function() {
		return {
			restrict: 'A',
			scope: {
				type: "@bridgeType",
				bridge: "=webFormsBridge",
				bridgeSyncHeight: "@bridgeSyncHeight"
			},
			link: function (scope, elem, attrs) {
				elem[0].src = webFormsBridgeUrl + '?type=' + scope.type;
				elem.on('load', function () {					
					var w = elem[0].contentWindow || elem[0];
					w.connectBridge(scope.bridge);

					// Sync height
					if (scope.bridgeSyncHeight == "true") {
						
						var resize = function () {
							elem.css('height', '');
							elem.css('height', w.document.body.scrollHeight + "px");
						};

						//w.$(w).resize(resize); // Performance issues when uncommenting this line...
						resize();
						w.$(w.document).ready(function() {
							resize();
						});

					}
				});
			}
		};
	});

})();