
(function () {
	'use strict';

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	var webFormsBridgeUrl = "/en-us/Homeö/ctl/webformsbridge/mid/4023/popUp/true";

	var app = angular.module('sxcFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap'], function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: 'string-wysiwyg',
			template: '<iframe style="width:100%;" web-forms-bridge="vm.bridge" bridge-type="wysiwyg" bridge-sync-height="true"></iframe>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			controller: 'FieldTemplate-WysiwygCtrl as vm'
		});

		formlyConfigProvider.setType({
			name: 'hyperlink-default',
			template: '<div><div class="input-group" dropdown>' +
				'<input type="text" class="form-control" ng-model="model[options.key]">' +
				'<span class="input-group-btn"><button type="button" id="single-button" class="btn btn-default dropdown-toggle" dropdown-toggle>' +
				'...' +
				'</button></span>' +
				'<ul class="dropdown-menu pull-right" role="menu">' +
					'<li role="menuitem"><a ng-click="vm.openDialog()" href="#">Page Picker</a></li>' +
				'</ul>' +
				'</div><div>Test: {{vm.test}}</div></div>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			controller: 'FieldTemplate-HyperlinkCtrl as vm'
		});

	});

	app.controller('FieldTemplate-HyperlinkCtrl', function ($modal) {

		var vm = this;
		vm.test = "...";

		vm.openDialog = function(type) {
			$modal.open({
				template: '<div><div class="modal-header">' +
            '<h3 class="modal-title">Select a page</h3>' +
			'</div>' +
			'<div class="modal-body" style="height:400px;"><iframe style="width:100%; height:400px" web-forms-bridge="vm.bridge" bridge-type="pagepicker" bridge-sync-height="true"></iframe></div>' +
			'<div class="modal-footer"></div>' +
			'</div>'
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
					if (scope.bridgeSyncHeight) {
						
						var resize = function () {
							elem.css('height', '');
							elem.css('height', w.document.body.scrollHeight + "px");
						};

						//w.$(w).resize('resize'); // Performance issues when uncommenting this line...
						resize();

					}
				});
			}
		};
	});

})();