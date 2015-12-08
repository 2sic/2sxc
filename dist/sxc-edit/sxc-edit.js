
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
        "EavConfiguration",
        "SxcServices"
    ]);

})();
/* js/fileAppDirectives */

angular.module("sxcFieldTemplates")
    .directive("dropzone", ["sxc", "tabId", "dragClass", function (sxc, tabId, dragClass) {
        return {
            restrict: "C",
            link: function (scope, element, attrs) {
                var header = scope.$parent.to.header;
                var field = scope.$parent.options.key;
                var entityGuid = header.Guid; 
                var url = sxc.resolveServiceUrl("app-content/" + header.ContentTypeName + "/" + entityGuid + "/" + field);

                var config = {
                    url: url,
                    maxFilesize: 100,
                    paramName: "uploadfile",
                    maxThumbnailFilesize: 10,

                    headers: {
                        "ModuleId": sxc.id,
                        "TabId": tabId
                    },

                    //previewTemplate: "<div></div>",
                    dictDefaultMessage: "",
                    addRemoveLinks: false,
                    previewsContainer: ".field-" + field.toLowerCase() +  " .dropzone-previews",
                    clickable: ".field-" + field.toLowerCase() +  " .dropzone-adam"
                };

                var eventHandlers = {
                    'addedfile': function(file) {
                        //scope.file = file;
                        //if (this.files[1] !== null) {
                        //    this.removeFile(this.files[0]);
                        //}
                        scope.$apply(function() {
                            scope.fileAdded = true;
                        });
                    },

                    'success': function (file, response) {
                        if (response.Success) {
                            scope.$parent.value.Value = "File:" + response.FileId;
                            scope.$apply();
                        } else {
                            alert("Upload failed because: " + response.Error);
                        }
                    }
                };

                var dropzone = new Dropzone(element[0], config);

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

	angular.module("sxcFieldTemplates")


	.directive("webFormsBridge", ["sxc", "portalRoot", function (sxc, portalRoot) {
	    var webFormsBridgeUrl = portalRoot + "Default.aspx?tabid=" + $2sxc.urlParams.require("tid") + "&ctl=webformsbridge&mid=" + sxc.id + "&popUp=true";

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

(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	angular.module("sxcFieldTemplates")

    .config(["formlyConfigProvider", function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: "hyperlink-default",
			templateUrl: "fieldtemplates/templates/hyperlink-default.html",
			wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
			controller: "FieldTemplate-HyperlinkCtrl as vm"
		});
        
	}])

	.controller("FieldTemplate-HyperlinkCtrl", ["$modal", "$scope", "$http", "sxc", "adamSvc", "debugState", function ($modal, $scope, $http, sxc, adamSvc, debugState) {

	    var vm = this;
	    vm.debug = debugState;
		vm.modalInstance = null;
		vm.testLink = "";
		vm.checkImgRegEx = /(?:([^:\/?#]+):)?(?:\/\/([^\/?#]*))?([^?#]*\.(?:jpg|jpeg|gif|png))(?:\?([^#]*))?(?:#(.*))?/i;

		vm.isImage = function () {
		    var value = $scope.value;
		    return vm.checkImgRegEx.test(vm.testLink);
		};
		vm.thumbnailUrl = function thumbnailUrl(size) {
	        if (size === 1)
	            return vm.testLink + "?w=46&h=46&mode=crop";
	        if(size===2)
	            return vm.testLink + "?w=500&h=400&mode=max";
	    };

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
			    Paths: $scope.to.settings.merged ? $scope.to.settings.merged.Paths : "",
			    FileFilter: $scope.to.settings.merged ? $scope.to.settings.merged.FileFilter : ""
			}
		};

		// Update test-link if necessary
		$scope.$watch("value.Value", function (newValue, oldValue) {
			if (!newValue)
				return;

		    // handle short-ID links like file:17
		    var linkLowered = newValue.toLowerCase();
		    if (linkLowered.indexOf("file") !== -1 || linkLowered.indexOf("page") !== -1) {
				$http.get("dnn/Hyperlink/ResolveHyperlink?hyperlink=" + encodeURIComponent(newValue)).then(function (result) {
					if(result.data)
						vm.testLink = result.data;
				});
			} else {
			    vm.testLink = newValue;
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

		vm.getExistingFiles = function () {
		    var header = $scope.to.header;
		    var field = $scope.options.key;
		    var entityGuid = header.Guid;

		    var adam = adamSvc(header.ContentTypeName, entityGuid, field, "");
	        vm.items = adam.liveList();
	        vm.refresh = adam.liveListReload;
		};

	    vm.setToExisting = function(fileItem) {
	        if (!fileItem.IsFolder)
	            $scope.value.Value = "File:" + fileItem.Id;
	    };

	    }]);


})();

(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	angular.module("sxcFieldTemplates")

    .config(["formlyConfigProvider", function (formlyConfigProvider) {

        // for now identical with -adv, but later will change
		formlyConfigProvider.setType({
			name: "string-wysiwyg",
			templateUrl: "fieldtemplates/templates/string-wysiwyg.html",
			wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
			controller: "FieldTemplate-WysiwygCtrl as vm"
		});

        // for now identical with -adv, but later will change
		formlyConfigProvider.setType({
			name: "string-wysiwyg-adv",
			templateUrl: "fieldtemplates/templates/string-wysiwyg.html",
			wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
			controller: "FieldTemplate-WysiwygCtrl as vm"
		});

        
	}])


	.controller("FieldTemplate-WysiwygCtrl", ["$scope", function ($scope) {

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
    "<div><div class=modal-header><h3 class=modal-title translate=Edit.Fields.Hyperlink.PagePicker.Title></h3></div><div class=modal-body style=\"height:370px; width:600px\"><iframe style=\"width:100%; height: 350px; border: 0\" web-forms-bridge=bridge bridge-type=pagepicker bridge-sync-height=false></iframe></div><div class=modal-footer></div></div>"
  );


  $templateCache.put('fieldtemplates/templates/hyperlink-default.html',
    "<div><div class=dropzone><div class=input-group dropdown><div ng-if=\"value.Value && vm.isImage()\" class=\"input-group-addon btn-default\" style=\"width: 46px; padding-top: 0px; padding-bottom: 0px; border-top-width: 0px; padding-left: 0px; padding-right: 0px; border-left-width: 0px; border-bottom-width: 0px; background-color: transparent; background-image: url('{{vm.thumbnailUrl(1)}}')\" ng-mouseover=\"vm.showPreview = true\" ng-mouseleave=\"vm.showPreview = false\"></div><input type=text class=\"form-control input-lg\" ng-model=value.Value tooltip=\"{{'Edit.Fields.Hyperlink.Default.Tooltip1' | translate }}\r" +
    "\n" +
    "{{'Edit.Fields.Hyperlink.Default.Tooltip2' | translate }}\r" +
    "\n" +
    "ADAM - sponsored with ♥ by 2sic.com\"> <span class=input-group-btn style=\"vertical-align: top\"><button type=button class=\"btn btn-primary dropzone-adam btn-lg\" ng-disabled=to.disabled tooltip=\"{{'Edit.Fields.Hyperlink.Default.AdamUploadLabel' | translate }}\"><i icon=upload></i> <i icon=apple></i></button> <button tabindex=-1 type=button class=\"btn btn-default dropdown-toggle btn-lg btn-square\" dropdown-toggle ng-disabled=to.disabled><i icon=option-horizontal></i></button></span><ul class=\"dropdown-menu pull-right\" role=menu><li role=menuitem><a class=dropzone-adam href=javascript:void(0);><i icon=apple></i> <span translate=Edit.Fields.Hyperlink.Default.MenuAdam></span></a></li><li role=menuitem ng-if=\"to.settings['merged'].ShowPagePicker\"><a ng-click=\"vm.openDialog('pagepicker')\" href=javascript:void(0)><i icon=home></i> <span translate=Edit.Fields.Hyperlink.Default.MenuPage></span></a></li><li role=menuitem ng-if=\"to.settings['merged'].ShowImageManager\"><a ng-click=\"vm.openDialog('imagemanager')\" href=javascript:void(0)><i icon=picture></i> <span translate=Edit.Fields.Hyperlink.Default.MenuImage></span></a></li><li role=menuitem ng-if=\"to.settings['merged'].ShowFileManager\"><a ng-click=\"vm.openDialog('documentmanager')\" href=javascript:void(0)><i icon=file></i> <span translate=Edit.Fields.Hyperlink.Default.MenuDocs></span></a></li></ul></div><div ng-if=vm.showPreview style=\"position: relative\"><div style=\"position: absolute;z-index: 100;background: white;top: 10px;text-align: center;left: 0;right: 0\"><img ng-src=\"{{vm.thumbnailUrl(2)}}\"></div></div><div ng-if=value.Value><a href={{vm.testLink}} target=_blank tabindex=-1 tooltip={{vm.testLink}}><i icon=new-window></i> <span>&nbsp;... {{vm.testLink.substr(vm.testLink.lastIndexOf(\"/\"), 100)}}</span></a></div><div class=dropzone-previews></div><div class=small ng-show=fileAdded><a href=\"http://2sxc.org/help?tag=adam\" target=_blank tooltip=\"ADAM is the Automatic Digital Assets Manager - click to discover more\"><i icon=apple></i> Adam</a> is sponsored with ♥ by <a tabindex=-1 href=\"http://2sic.com/\" target=_blank>2sic.com</a></div><div ng-if=vm.debug.on><span ng-click=vm.getExistingFiles()>show existing (beta) :)</span><div><div class=dz-preview><div class=dz-details><div class=dz-size><span data-dz-size=\"\"><strong>+</strong></span></div><div class=dz-filename><span data-dz-name=\"\">drag file</span></div><div class=dz-filename><span data-dz-name=\"\">or click</span></div></div><div class=dz-success-mark><svg width=54px height=54px viewbox=\"0 0 54 54\" version=1.1 xmlns=http://www.w3.org/2000/svg xmlns:xlink=http://www.w3.org/1999/xlink xmlns:sketch=http://www.bohemiancoding.com/sketch/ns><title>Check</title><defs></defs><g id=Page-1 stroke=none stroke-width=1 fill=none fill-rule=evenodd sketch:type=MSPage><path d=\"M23.5,31.8431458 L17.5852419,25.9283877 C16.0248253,24.3679711 13.4910294,24.366835 11.9289322,25.9289322 C10.3700136,27.4878508 10.3665912,30.0234455 11.9283877,31.5852419 L20.4147581,40.0716123 C20.5133999,40.1702541 20.6159315,40.2626649 20.7218615,40.3488435 C22.2835669,41.8725651 24.794234,41.8626202 26.3461564,40.3106978 L43.3106978,23.3461564 C44.8771021,21.7797521 44.8758057,19.2483887 43.3137085,17.6862915 C41.7547899,16.1273729 39.2176035,16.1255422 37.6538436,17.6893022 L23.5,31.8431458 Z M27,53 C41.3594035,53 53,41.3594035 53,27 C53,12.6405965 41.3594035,1 27,1 C12.6405965,1 1,12.6405965 1,27 C1,41.3594035 12.6405965,53 27,53 Z\" id=Oval-2 stroke-opacity=0.198794158 stroke=#747474 fill-opacity=0.816519475 fill=#FFFFFF sketch:type=MSShapeGroup></path></g></svg></div></div><div class=dz-preview ng-class=\"{ 'dz-success': value.Value.toLowerCase() == 'file:' + item.Id }\" ng-repeat=\"item in vm.items | filter: { IsFolder : false } | orderBy:'-IsFolder'\" ng-click=vm.setToExisting(item)><div class=dz-image><img ng-if=\"item.Type == 'image'\" data-dz-thumbnail=\"\" alt=\"{{ item.Id + ':' + item.Name }}\" ng-src=\"{{ '/portals/0/' + item.Path + '?w=120&h=120&mode=crop' }}\"></div><div class=dz-details><div class=dz-size><span data-dz-size=\"\"><strong>{{ item.Id }}</strong></span></div><div class=dz-filename><span data-dz-name=\"\">{{ item.Name }}</span></div><div class=dz-filename><span ng-if=!item.IsFolder data-dz-name=\"\">{{ (item.Size / 1024).toFixed(0) }} kb</span></div></div><div class=dz-error-message><span data-dz-errormessage=\"\"></span></div><div class=dz-success-mark><svg width=54px height=54px viewbox=\"0 0 54 54\" version=1.1 xmlns=http://www.w3.org/2000/svg xmlns:xlink=http://www.w3.org/1999/xlink xmlns:sketch=http://www.bohemiancoding.com/sketch/ns><title>Check</title><defs></defs><g id=Page-1 stroke=none stroke-width=1 fill=none fill-rule=evenodd sketch:type=MSPage><path d=\"M23.5,31.8431458 L17.5852419,25.9283877 C16.0248253,24.3679711 13.4910294,24.366835 11.9289322,25.9289322 C10.3700136,27.4878508 10.3665912,30.0234455 11.9283877,31.5852419 L20.4147581,40.0716123 C20.5133999,40.1702541 20.6159315,40.2626649 20.7218615,40.3488435 C22.2835669,41.8725651 24.794234,41.8626202 26.3461564,40.3106978 L43.3106978,23.3461564 C44.8771021,21.7797521 44.8758057,19.2483887 43.3137085,17.6862915 C41.7547899,16.1273729 39.2176035,16.1255422 37.6538436,17.6893022 L23.5,31.8431458 Z M27,53 C41.3594035,53 53,41.3594035 53,27 C53,12.6405965 41.3594035,1 27,1 C12.6405965,1 1,12.6405965 1,27 C1,41.3594035 12.6405965,53 27,53 Z\" id=Oval-2 stroke-opacity=0.198794158 stroke=#747474 fill-opacity=0.816519475 fill=#FFFFFF sketch:type=MSShapeGroup></path></g></svg></div><div class=dz-error-mark><svg width=54px height=54px viewbox=\"0 0 54 54\" version=1.1 xmlns=http://www.w3.org/2000/svg xmlns:xlink=http://www.w3.org/1999/xlink xmlns:sketch=http://www.bohemiancoding.com/sketch/ns><title>Error</title><defs></defs><g id=Page-1 stroke=none stroke-width=1 fill=none fill-rule=evenodd sketch:type=MSPage><g id=Check-+-Oval-2 sketch:type=MSLayerGroup stroke=#747474 stroke-opacity=0.198794158 fill=#FFFFFF fill-opacity=0.816519475><path d=\"M32.6568542,29 L38.3106978,23.3461564 C39.8771021,21.7797521 39.8758057,19.2483887 38.3137085,17.6862915 C36.7547899,16.1273729 34.2176035,16.1255422 32.6538436,17.6893022 L27,23.3431458 L21.3461564,17.6893022 C19.7823965,16.1255422 17.2452101,16.1273729 15.6862915,17.6862915 C14.1241943,19.2483887 14.1228979,21.7797521 15.6893022,23.3461564 L21.3431458,29 L15.6893022,34.6538436 C14.1228979,36.2202479 14.1241943,38.7516113 15.6862915,40.3137085 C17.2452101,41.8726271 19.7823965,41.8744578 21.3461564,40.3106978 L27,34.6568542 L32.6538436,40.3106978 C34.2176035,41.8744578 36.7547899,41.8726271 38.3137085,40.3137085 C39.8758057,38.7516113 39.8771021,36.2202479 38.3106978,34.6538436 L32.6568542,29 Z M27,53 C41.3594035,53 53,41.3594035 53,27 C53,12.6405965 41.3594035,1 27,1 C12.6405965,1 1,12.6405965 1,27 C1,41.3594035 12.6405965,53 27,53 Z\" id=Oval-2 sketch:type=MSShapeGroup></path></g></g></svg></div></div></div></div></div></div>"
  );


  $templateCache.put('fieldtemplates/templates/string-wysiwyg.html',
    "<iframe style=\"width:100%; border: 0\" web-forms-bridge=vm.bridge bridge-type=wysiwyg bridge-sync-height=true></iframe>"
  );

}]);
