/* global angular */
(function () {
	'use strict';

	var app = angular.module('SxcEditContentGroupDnnWrapper', ['sxcEditContentGroup']);
	app.controller('editContentGroupDnnWrapperCtrl', ["entityId", "typeName", "groupGuid", "groupIndex", function (entityId, typeName, groupGuid, groupIndex) {
		var vm = this;
		// Prepare URL parameters, which are passed to edit directive
		vm.edit = {
			entityId: entityId,
			contentTypeName: typeName,
			contentGroupGuid: groupGuid,
			sortOrder: groupIndex
		};
	}]);

})();

/* global angular */
(function() {
	'use strict';

	var app = angular.module('sxcEditContentGroup', ['eavEditEntity', 'eavLocalization', 'sxcFieldTemplates', 'SxcEditTemplates', 'sxcEditContentGroupSvc']);
	app.directive('editContentGroup', function() {
		return {
		    templateUrl: 'edit-entity-or-contentgroup.html',
			restrict: 'E',
			scope: {
				edit: '=edit'
			},
			controller: 'editContentGroupCtrl',
			controllerAs: 'vm'
		};
	});

	app.controller('editContentGroupCtrl', ["$q", "$scope", "sxcContentGroupService", function($q, $scope, sxcContentGroupService) {
		var vm = this;

		// This array holds the entities to edit
		vm.entitiesToEdit = [];

		// Prepare parameters
		var entityId = $scope.edit.entityId;
		var contentTypeName = $scope.edit.contentTypeName;
		var contentGroupGuid = $scope.edit.contentGroupGuid;
		//var mode = $scope.edit.mode;
		var sortOrder = $scope.edit.sortOrder;

		// Edit a content group - first load the contentgroup configuration
		// Then add entities to edit from configuration
		if (contentGroupGuid) {
			// ToDo: Refactor - move to service and move api
			sxcContentGroupService.get(contentGroupGuid).then(function (result) {
				var contentGroup = result.data;

				// Template must be set to edit something
				if(!contentGroup.Template)
					alert('No template defined');

				console.log(result);

				// Use either default or List types. Reset sortOrder to 0 for List types.
				var editTypes = sortOrder == -1 ? ['ListContent', 'ListPresentation'] : ['Content', 'Presentation'];
				if (sortOrder == -1)
					sortOrder = 0;

				angular.forEach(editTypes, function (editType, i) {
					var contentTypeName = contentGroup.Template[editType + 'TypeStaticName'];

					var entityToEdit = {
						contentTypeName: contentTypeName,
						entityId: contentGroup[editType][sortOrder],
						editControlTitle: editType
					};

					if (editType.indexOf("Presentation") != -1)
						entityToEdit.isPresentation = true;
					if (entityToEdit.isPresentation && !entityToEdit.entityId)
						entityToEdit.useDefaultValues = true;

					if (contentTypeName)
						vm.entitiesToEdit.push(entityToEdit);
				});

				console.log(vm.entitiesToEdit);
			});
		}
		
		if (entityId) // User wants to edit a single entity
			vm.entitiesToEdit.push({ entityId: entityId, editControlTitle: 'Entity' });
		else if (contentTypeName && !contentGroupGuid) // EntityId not specified, but contentTypeName - user wants to add an item of this type
			vm.entitiesToEdit.push({ contentTypeName: contentTypeName, editControlTitle: 'Entity' });

		vm.registeredControls = [];
		vm.registerEditControl = function(control) {
			vm.registeredControls.push(control);
		};

		vm.isValid = function() {
			var valid = true;
			angular.forEach(vm.registeredControls, function(e, i) {
				if (!e.isValid())
					valid = false;
			});
			return valid;
		};

		vm.save = function() {
			var savePromises = [];
			angular.forEach(vm.registeredControls, function(e, i) {
				savePromises.push(e.save());
			});
			$q.all(savePromises).then(function() {
				alert("All save promises resolved!");
			});
		};
	}]);

})();


(function () {
	'use strict';

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    var app = angular.module('sxcFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap', 'ui.tree', '2sxc4ng', 'SxcEditTemplates'], ["formlyConfigProvider", function (formlyConfigProvider) {

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

	}]);

	app.controller('FieldTemplate-HyperlinkCtrl', ["$modal", "$scope", "$http", "sxc", function ($modal, $scope, $http, sxc) {

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
				controller: ["$scope", "bridge", function($scope, bridge) {
					$scope.bridge = bridge;
				}],
				windowClass: 'sxc-dialog-filemanager'
			});
		};

	}]);

	app.controller('FieldTemplate-WysiwygCtrl', ["$scope", function ($scope) {

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

	}]);

	app.controller('FieldTemplate-EntityCtrl', ["$scope", "$http", "$filter", "$modal", function($scope, $http, $filter, $modal) {

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
				controller: ["entityType", function(entityType) {
					var vm = this;
					vm.edit = { contentTypeName: entityType };
				}],
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

	}]);

	app.directive('webFormsBridge', ["sxc", function (sxc) {
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
	}]);

})();
/* global angular */

// TODO - merge this into the contentGroup controller - it shouldn't need an own controller for this in Formly


(function () {
	'use strict';

	angular.module('sxcEditContentGroupSvc', []).factory('sxcContentGroupService', ["$http", function ($http) {

		return {
			get: function (contentGroupGuid) {
				// Returns a typed default value from the string representation
				return $http.get('app/ContentGroup/Get?contentGroupGuid=' + contentGroupGuid);
			}
		};

	}]);

})();
angular.module('SxcEditTemplates',[]).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('edit-contentgroup-dnnwrapper.html',
    "<edit-content-group edit=vm.edit></edit-content-group>"
  );


  $templateCache.put('edit-entity-or-contentgroup.html',
    "<div><eav-language-switcher></eav-language-switcher><div ng-repeat=\"entityToEdit in vm.entitiesToEdit\"><h2 ng-if=entityToEdit.editControlTitle>{{entityToEdit.editControlTitle}}</h2><div class=bg-info style=padding:12px ng-if=\"entityToEdit.isPresentation && entityToEdit.useDefaultValues\">The default values are used currently. <a class=\"btn btn-default\" ng-click=\"entityToEdit.useDefaultValues = false;\">Create</a></div><div ng-if=!entityToEdit.useDefaultValues><div class=pull-right ng-if=entityToEdit.isPresentation><a class=\"btn btn-default\" ng-click=\"entityToEdit.useDefaultValues = true;\">Use default values</a></div><eav-edit-entity content-type-name={{entityToEdit.contentTypeName}} entity-id={{entityToEdit.entityId}} register-edit-control=vm.registerEditControl></eav-edit-entity></div></div><button ng-disabled=!vm.isValid() ng-click=vm.save() class=\"btn btn-primary submit-button\">Save</button></div>"
  );


  $templateCache.put('fieldtemplates/templates/entity-default.html',
    "<div class=eav-entityselect><div ui-tree=options data-empty-place-holder-enabled=false><ol ui-tree-nodes ng-model=model[options.key]><li ng-repeat=\"item in model[options.key]\" ui-tree-node class=eav-entityselect-item><div ui-tree-handle><span title=\"{{getEntityText(item) + ' (' + item + ')'}}\">{{getEntityText(item)}}</span> <a data-nodrag title=\"Remove this item\" ng-click=remove(this) class=eav-entityselect-item-remove>[remove]</a></div></li></ol></div><select class=\"eav-entityselect-selector form-control\" ng-model=selectedEntity ng-change=addEntity() ng-show=\"to.settings.Entity.AllowMultiValue || model[options.key].length < 1\"><option value=\"\">-- choose --</option><option value=new ng-if=createEntityAllowed()>-- new --</option><option ng-repeat=\"item in availableEntities\" ng-disabled=\"model[options.key].indexOf(item.Value) != -1\" value={{item.Value}}>{{item.Text}}</option></select></div>"
  );


  $templateCache.put('fieldtemplates/templates/hyperlink-default-filemanager.html',
    "<div><iframe class=sxc-dialog-filemanager-iframe style=\"width:100%; height:100%; overflow:hidden\" scrolling=no web-forms-bridge=bridge bridge-type=filemanager bridge-sync-height=false></iframe></div><style>.sxc-dialog-filemanager .modal-dialog { width: 100%;height: 100%;margin: 0; }\r" +
    "\n" +
    "\t.sxc-dialog-filemanager .modal-content { background: none;height: 100%; }\r" +
    "\n" +
    "\t.sxc-dialog-filemanager-iframe { position: absolute;top: 0;left: 0;right: 0;bottom: 0; }</style>"
  );


  $templateCache.put('fieldtemplates/templates/hyperlink-default-pagepicker.html',
    "<div><div class=modal-header><h3 class=modal-title>Select a page</h3></div><div class=modal-body style=\"height:370px; width:600px\"><iframe style=\"width:100%; height:350px\" web-forms-bridge=bridge bridge-type=pagepicker bridge-sync-height=false></iframe></div><div class=modal-footer></div></div>"
  );


  $templateCache.put('fieldtemplates/templates/hyperlink-default.html',
    "<div><div class=input-group dropdown><input type=text class=form-control ng-model=value.Value> <span class=input-group-btn><button type=button id=single-button class=\"btn btn-default dropdown-toggle\" dropdown-toggle ng-disabled=to.disabled>...</button></span><ul class=\"dropdown-menu pull-right\" role=menu><li role=menuitem><a ng-click=\"vm.openDialog('pagepicker')\" href=javascript:void(0)>Page Picker</a></li><li role=menuitem><a ng-click=\"vm.openDialog('imagemanager')\" href=javascript:void(0)>Image Manager</a></li><li role=menuitem><a ng-click=\"vm.openDialog('documentmanager')\" href=javascript:void(0)>Document Manager</a></li></ul></div><div class=small>Test: <a href={{vm.testLink}} target=_blank>{{vm.testLink}}</a></div></div>"
  );


  $templateCache.put('fieldtemplates/templates/string-wysiwyg.html',
    "<iframe style=width:100% web-forms-bridge=vm.bridge bridge-type=wysiwyg bridge-sync-height=true></iframe>"
  );

}]);
