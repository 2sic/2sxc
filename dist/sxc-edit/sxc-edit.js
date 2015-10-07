
(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    angular.module("sxcFieldTemplates", [
        "formly",
        "formlyBootstrap",
        "ui.bootstrap",
        "ui.tree",
        "2sxc4ng",
        "SxcEditTemplates",
        "EavConfiguration"
        //"ngDropzone"
    ]);

})();
/* js/fileAppDirectives */

angular.module("sxcFieldTemplates")
    .directive("dropzone", ["sxc", "tabId", function (sxc, tabId) {
        return {
            restrict: "C",
            link: function (scope, element, attrs) {
                var header = scope.$parent.to.header;
                var field = scope.$parent.options.key;
                var tempGuid = header.ContentTypeName; // todo: this is wrong, it should have the entityguid, but I don't know where to find it
                var url = sxc.resolveServiceUrl("app-content/" + header.ContentTypeName + "/" + tempGuid + "/" + field);

                var config = {
                    url: url,// 'http://localhost:8080/upload',
                    maxFilesize: 100,
                    paramName: "uploadfile",
                    maxThumbnailFilesize: 10,
                    //parallelUploads: 1,
                    //autoProcessQueue: true, // false
                    
                    headers: {
                        "ModuleId": sxc.id,
                        "TabId": tabId
                    },

                    previewTemplate: "<span></span>",
                    dictDefaultMessage: ""

                };

                var eventHandlers = {
                    //'addedfile': function(file) {
                    //    scope.file = file;
                    //    if (this.files[1] !== null) {
                    //        this.removeFile(this.files[0]);
                    //    }
                    //    scope.$apply(function() {
                    //        scope.fileAdded = true;
                    //    });
                    //},

                    'success': function (file, response) {
                        if (response.Success) {
                            scope.$parent.value.Value = "File:" + response.FileId;
                            scope.$apply();
                        } else {
                            alert("Upload failed because: " + response.Error);
                        }
                    }

                };

                dropzone = new Dropzone(element[0], config);

                angular.forEach(eventHandlers, function(handler, event) {
                    dropzone.on(event, handler);
                });

                scope.processDropzone = function() {
                    dropzone.processQueue();
                };

                scope.resetDropzone = function() {
                    dropzone.removeAllFiles();
                };
            }
        };
    }]);

(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	var app = angular.module("sxcFieldTemplates")

    .config(["formlyConfigProvider", function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: "string-wysiwyg",
			templateUrl: "fieldtemplates/templates/string-wysiwyg.html",
			wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
			controller: "FieldTemplate-WysiwygCtrl as vm"
		});

		formlyConfigProvider.setType({
			name: "hyperlink-default",
			templateUrl: "fieldtemplates/templates/hyperlink-default.html",
			wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
			controller: "FieldTemplate-HyperlinkCtrl as vm"
		});
        
	}]);

	app.controller("FieldTemplate-HyperlinkCtrl", ["$modal", "$scope", "$http", "sxc", function ($modal, $scope, $http, sxc) {

		var vm = this;
		vm.modalInstance = null;
		vm.testLink = "";
		
		vm.bridge = { 
			valueChanged: function(value, type) {
				$scope.$apply(function () {

					// Convert file path to file ID if type file is specified
					if (value) {
						$scope.value.Value = value;


						if (type === "file") {
					        var valueWithoutVersion = value.replace(/\?ver=[0-9\-]*$/gi, "");
					        $http.get("dnn/Hyperlink/GetFileByPath?relativePath=" + encodeURIComponent(valueWithoutVersion)).then(function (result) {
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
		$scope.$watch("value.Value", function (newValue, oldValue) {
			if (!newValue)
				return;

			if (newValue.indexOf("File") !== -1 || newValue.indexOf("Page") !== -1) {
				$http.get("dnn/Hyperlink/ResolveHyperlink?hyperlink=" + encodeURIComponent(newValue)).then(function (result) {
					if(result.data)
						vm.testLink = result.data;
				});
			}
		});

		vm.openDialog = function (type, options) {

			var template = type === "pagepicker" ? "pagepicker" : "filemanager";
			vm.bridge.dialogType = type;
			vm.bridge.params.CurrentValue = $scope.value.Value;

			vm.modalInstance = $modal.open({
				templateUrl: "fieldtemplates/templates/hyperlink-default-" + template + ".html",
				resolve: {
					bridge: function() {
						return vm.bridge;
					}
				},
				controller: ["$scope", "bridge", function($scope, bridge) {
					$scope.bridge = bridge;
				}],
				windowClass: "sxc-dialog-filemanager"
			});
		};

	}]);

	app.controller("FieldTemplate-WysiwygCtrl", ["$scope", function ($scope) {

		var vm = this;

		// Everything the WebForms bridge (iFrame) should have access to
		vm.bridge = {
		    initialValue: "",
            initialReadOnly: false,
		    onChanged: function (newValue) {
				$scope.$apply(function () {
					$scope.value.Value = newValue;
				});
			},
			setValue: function (value) { vm.bridge.initialValue = value; },
			setReadOnly: function(readOnly) { vm.bridge.initialReadOnly = readOnly; }
		};

		$scope.$watch("value.Value", function (newValue, oldValue) {
			if (newValue !== oldValue)
				vm.bridge.setValue(newValue);
		});

		$scope.$watch("to.disabled", function (newValue, oldValue) {
			if (newValue !== oldValue)
				vm.bridge.setReadOnly(newValue);
		});

	}]);

	app.directive("webFormsBridge", ["sxc", "webRoot", function (sxc, webRoot) {
	    var webFormsBridgeUrl = webRoot + "?tabid=" + $2sxc.urlParams.require("tid") + "&ctl=webformsbridge&mid=" + sxc.id + "&popUp=true";

		return {
			restrict: "A",
			scope: {
				type: "@bridgeType",
				bridge: "=webFormsBridge",
				bridgeSyncHeight: "@bridgeSyncHeight"
			},
			link: function (scope, elem, attrs) {

			    var params = "";
			    if (scope.bridge.params) {
			        params = Object.keys(scope.bridge.params).map(function (prop) {
			            if (scope.bridge.params[prop] === null || scope.bridge.params[prop] === '')
			                return;
			            return [prop, scope.bridge.params[prop]].map(encodeURIComponent).join("=");
			        }).join("&");
			    }

			    elem[0].src = webFormsBridgeUrl + "&type=" + scope.type + (scope.bridge.params ? "&" + params : "");
				elem.on("load", function () {					
					var w = elem[0].contentWindow || elem[0];
					w.connectBridge(scope.bridge);

					// Sync height
					if (scope.bridgeSyncHeight === "true") {
						
						var resize = function () {
							elem.css("height", "");
							elem.css("height", w.document.body.scrollHeight + "px");
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


//(function () {
//	'use strict';

//	angular.module('sxcEditContentGroupSvc', []).factory('sxcContentGroupService', function ($http) {

//		return {
//			get: function (contentGroupGuid) {
//				// Returns a typed default value from the string representation
//				return $http.get('app/ContentGroup/Get?contentGroupGuid=' + contentGroupGuid);
//			}
//		};

//	});

//})();
angular.module('SxcEditTemplates',[]).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('fieldtemplates/templates/hyperlink-default-filemanager.html',
    "<div><iframe class=sxc-dialog-filemanager-iframe style=\"width:100%; height:100%; overflow:hidden; border: 0\" scrolling=no web-forms-bridge=bridge bridge-type=filemanager bridge-sync-height=false></iframe></div><style>.sxc-dialog-filemanager .modal-dialog { width: 100%;height: 100%;margin: 0; }\r" +
    "\n" +
    "\t.sxc-dialog-filemanager .modal-content { background: none;height: 100%; }\r" +
    "\n" +
    "\t.sxc-dialog-filemanager-iframe { position: absolute;top: 0;left: 0;right: 0;bottom: 0; }</style>"
  );


  $templateCache.put('fieldtemplates/templates/hyperlink-default-pagepicker.html',
    "<div><div class=modal-header><h3 class=modal-title>Select a page</h3></div><div class=modal-body style=\"height:370px; width:600px\"><iframe style=\"width:100%; height: 350px; border: 0\" web-forms-bridge=bridge bridge-type=pagepicker bridge-sync-height=false></iframe></div><div class=modal-footer></div></div>"
  );


  $templateCache.put('fieldtemplates/templates/hyperlink-default.html',
    "<div><div class=\"input-group dropzone\" dropdown><input type=text class=form-control ng-model=value.Value tooltip=\"drop file here to auto-upload\r" +
    "\n" +
    "for help see 2sxc.org/help?tag=adam\r" +
    "\n" +
    "ADAM - sponsored with love by 2sic.com\"> <span class=input-group-btn><button type=button id=single-button class=\"btn btn-default dropdown-toggle\" dropdown-toggle ng-disabled=to.disabled><span icon=option-horizontal></span></button></span><ul class=\"dropdown-menu pull-right\" role=menu><li role=menuitem><a ng-click=\"vm.openDialog('pagepicker')\" href=javascript:void(0)>Page Picker</a></li><li role=menuitem><a ng-click=\"vm.openDialog('imagemanager')\" href=javascript:void(0)>Image Manager</a></li><li role=menuitem><a ng-click=\"vm.openDialog('documentmanager')\" href=javascript:void(0)>Document Manager</a></li></ul></div><div ng-if=value.Value class=small>Test: <a href={{vm.testLink}} target=_blank>{{vm.testLink}}</a></div></div>"
  );


  $templateCache.put('fieldtemplates/templates/string-wysiwyg.html',
    "<iframe style=\"width:100%; border: 0\" web-forms-bridge=vm.bridge bridge-type=wysiwyg bridge-sync-height=true></iframe>"
  );

}]);
