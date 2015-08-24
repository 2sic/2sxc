
(function () {
	'use strict';

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	var webFormsBridgeUrl = "/en-us/Homeö/ctl/webformsbridge/mid/4023/popUp/true";

	var app = angular.module('sxcFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap'], function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: 'string-wysiwyg',
			template: '<iframe style="height:500px; width:100%;" web-forms-bridge="bridge" bridge-type="wysiwyg"></iframe>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			controller: 'FieldTemplate-WysiwygCtrl as vm'
		});

		formlyConfigProvider.setType({
			name: 'hyperlink-default',
			template: '<div><input type="text" class="form-control" ng-model="model[options.key]"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
				'<span class="caret"></span>' +
				'<span class="sr-only">Toggle Dropdown</span>' +
				'</button>Test: {{vm.test}}</div>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			controller: 'FieldTemplate-HyperlinkCtrl as vm'
		});

	});

	app.controller('FieldTemplate-HyperlinkCtrl', function () {
		var vm = this;
		vm.test = "...";
	});

	app.controller('FieldTemplate-WysiwygCtrl', function($scope) {
		
		// Everything the WebForms bridge (iFrame) should have access to
		$scope.bridge = {
			onChanged: function (newValue) {
				alert(newValue);
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
				bridge: "=webFormsBridge"
			},
			link: function (scope, elem, attrs) {
				elem[0].src = webFormsBridgeUrl + '?type=' + scope.type;
				elem.on('load', function() {
					var w = elem[0].contentWindow || elem[0];
					w.bridge = scope.bridge;
				});
			}
		};
	});

})();