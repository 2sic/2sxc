
(function () {
	'use strict';

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    var app = angular.module('sxcFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap', 'ui.tree', '2sxc4ng', 'SxcEditTemplates'], function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: 'string-wysiwyg',
			templateUrl: 'fieldtemplates/templates/string-wysiwyg.html',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			controller: 'FieldTemplate-WysiwygCtrl as vm'
		});

		formlyConfigProvider.setType({
			name: 'hyperlink-default',
			templateUrl: 'fieldtemplates/templates/hyperlink-default.html',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			controller: 'FieldTemplate-HyperlinkCtrl as vm'
		});

		formlyConfigProvider.setType({
			name: 'entity-default',
			templateUrl: 'fieldtemplates/templates/entity-default.html',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			controller: 'FieldTemplate-EntityCtrl'
		});

	});

	app.controller('FieldTemplate-HyperlinkCtrl', function ($modal, $scope, $http, sxc) {

		var vm = this;
		vm.modalInstance = null;
		vm.testLink = "";
		
		vm.bridge = {
			valueChanged: function(value, type) {
				$scope.$apply(function () {

					// Convert file path to file ID if type file is specified
					if (value) {
						$scope.value.Value = value;

						if (type == "file") {
							$http.get('dnn/Hyperlink/GetFileByPath?relativePath=' + encodeURIComponent(value)).then(function (result) {
								if(result.data)
									$scope.value.Value = "File:" + result.data.FileId;
							});
						}
					}
					vm.modalInstance.close();
				});
			},
			params: {
				Paths: $scope.to.settings.Hyperlink.Paths,
				FileFilter: $scope.to.settings.Hyperlink.FileFilter
			}
		};

		// Update test-link if necessary
		$scope.$watch('value.Value', function (newValue, oldValue) {
			if (!newValue)
				return;

			if (newValue.indexOf("File") != -1 || newValue.indexOf("Page") != -1) {
				$http.get('eav/FieldTemplateHyperlink/ResolveHyperlink?hyperlink=' + encodeURIComponent(newValue)).then(function (result) {
					if(result.data)
						vm.testLink = result.data;
				});
			}
		});

		vm.openDialog = function (type, options) {

			var template = type == 'pagepicker' ? 'pagepicker' : 'filemanager';
			vm.bridge.dialogType = type;
			vm.bridge.params.CurrentValue = $scope.value.Value;

			vm.modalInstance = $modal.open({
				templateUrl: 'fieldtemplates/templates/hyperlink-default-' + template + '.html',
				resolve: {
					bridge: function() {
						return vm.bridge;
					}
				},
				controller: function($scope, bridge) {
					$scope.bridge = bridge;
				},
				windowClass: 'sxc-dialog-filemanager'
			});
		};

	});

	app.controller('FieldTemplate-WysiwygCtrl', function ($scope) {

		var vm = this;

		// Everything the WebForms bridge (iFrame) should have access to
		vm.bridge = {
			onChanged: function (newValue) {
				$scope.$apply(function () {
					$scope.value.Value = newValue;
				});
			},
			setValue: function () { console.log('Error: setValue has no override'); },
			setReadOnly: function() { console.log('Error: setReadOnly has no override'); }
		};

		$scope.$watch('value.Value', function (newValue, oldValue) {
			if (newValue != oldValue)
				vm.bridge.setValue(newValue);
		});

		$scope.$watch('to.disabled', function (newValue, oldValue) {
			if (newValue != oldValue)
				vm.bridge.setReadOnly(newValue);
		});

	});

	app.controller('FieldTemplate-EntityCtrl', function($scope, $http, $filter, $modal) {

		$scope.availableEntities = [];

		if ($scope.model[$scope.options.key] === null)
			$scope.model[$scope.options.key] = [];

		$scope.addEntity = function() {
			if ($scope.selectedEntity == "new")
				$scope.openNewEntityDialog();
			else
				$scope.model[$scope.options.key].push(parseInt($scope.selectedEntity));
			$scope.selectedEntity = "";
		};

		$scope.createEntityAllowed = function () {
			return $scope.to.settings.Entity.EntityType !== null && $scope.to.settings.Entity.EntityType !== "";
		};

		$scope.openNewEntityDialog = function () {

			var modalInstance = $modal.open({
				template: '<div style="padding:20px;"><edit-content-group edit="vm.edit"></edit-content-group></div>',
				controller: function(entityType) {
					var vm = this;
					vm.edit = { contentTypeName: entityType };
				},
				controllerAs: 'vm',
				resolve: {
					entityType: function() {
						return $scope.to.settings.Entity.EntityType;
					}
				}
			});

			modalInstance.result.then(function() {
				$scope.getAvailableEntities();
			});

		};

		$scope.getAvailableEntities = function () {
			$http({
				method: 'GET',
				url: 'eav/EntityPicker/getavailableentities',
				params: {
					entityType: $scope.to.settings.Entity.EntityType,
					// ToDo: dimensionId: $scope.configuration.DimensionId
				}
			}).then(function(data) {
				$scope.availableEntities = data.data;
			});
		};

		$scope.getEntityText = function (entityId) {
		    var entities = $filter('filter')($scope.availableEntities, { Value: entityId });
		    return entities.length > 0 ? entities[0].Text : "(Entity not found)";
		};

		// Initialize entities
		$scope.getAvailableEntities();

	});

	app.directive('webFormsBridge', function (sxc) {
		var webFormsBridgeUrl = sxc._editContentGroupConfig.tabBaseUrl + "?ctl=webformsbridge&mid=" + sxc.id + "&popUp=true";

		return {
			restrict: 'A',
			scope: {
				type: "@bridgeType",
				bridge: "=webFormsBridge",
				bridgeSyncHeight: "@bridgeSyncHeight",
			},
			link: function (scope, elem, attrs) {
				elem[0].src = webFormsBridgeUrl + '&type=' + scope.type + (scope.bridge.params ? "&" + $.param(scope.bridge.params) : "");
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