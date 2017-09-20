(function() { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("Adam", [
        "SxcServices",
        "EavConfiguration", // config
        "EavServices", // multi-language stuff
        ])
        ;

} ());
(function() {
    /* jshint laxbreak:true*/

    angular.module("Adam", [])
        /*@ngInject*/
        .directive("adamHint", function () {
            return {
                restrict: "E",
                replace: false,
                transclude: false,
                templateUrl: "adam/adam-hint.html"
            };
        });
})();
angular.module("Adam")
    /*@ngInject*/
    .factory("adamSvc", ["$http", "eavConfig", "sxc", "svcCreator", "appRoot", function ($http, eavConfig, sxc, svcCreator, appRoot) {

        // Construct a service for this specific appId
        return function createSvc(contentType, entityGuid, field, subfolder, serviceConfig) {
            var svc = {
                url: sxc.resolveServiceUrl("app-content/" + contentType + "/" + entityGuid + "/" + field),
                subfolder: subfolder,
                folders: [],
                adamRoot: appRoot.substr(0, appRoot.indexOf("2sxc"))
            };

            // get the correct url for uploading as it is needed by external services (dropzone)
            svc.uploadUrl = function(targetSubfolder) {
                var url = (targetSubfolder === "")
                    ? svc.url
                    : svc.url + "?subfolder=" + targetSubfolder;
                url += (url.indexOf("?") == -1 ? "?" : "&") + "usePortalRoot=" + serviceConfig.usePortalRoot;
                return url;
            };

            // extend a json-response with a path (based on the adam-root) to also have a fullPath
            svc.addFullPath = function addFullPath(value, key) {
                value.fullPath = svc.adamRoot + value.Path;
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get(svc.url + "/items", { params: { subfolder: svc.subfolder, usePortalRoot: serviceConfig.usePortalRoot } })
                    .then(function (result) {
                        angular.forEach(result.data, svc.addFullPath);
                        return result;
                    });
            }));

            // create folder
            svc.addFolder = function add(newfolder) {
                return $http.post(svc.url + "/folder", {}, { params: { subfolder: svc.subfolder, newFolder: newfolder, usePortalRoot: serviceConfig.usePortalRoot } })
                    .then(svc.liveListReload);
            };

            svc.goIntoFolder = function(childFolder) {
                svc.folders.push(childFolder);
                var pathParts = childFolder.Path.split("/");
                var subPath = "";
                for (var c = 0; c < svc.folders.length; c++)
                    subPath = pathParts[pathParts.length - c - 2] + "/" + subPath;

                subPath = subPath.replace("//", "/");
                if (subPath[subPath.length - 1] === "/")
                    subPath = subPath.substr(0, subPath.length - 1);

                childFolder.Subfolder = subPath;

                // now assemble the correct subfolder based on the folders-array
                svc.subfolder = subPath;
                svc.liveListReload();
                return subPath;
            };

            svc.goUp = function() {
                if (svc.folders.length > 0)
                    svc.folders.pop();
                if (svc.folders.length > 0) {
                    svc.subfolder = svc.folders[svc.folders.length - 1].Subfolder;
                } else {
                    svc.subfolder = "";
                }
                svc.liveListReload();
                return svc.subfolder;
            };

            // delete, then reload
            // IF verb DELETE fails, so I'm using get for now
            svc.delete = function del(item) {
                return $http.get(svc.url + "/delete", { params: { subfolder: svc.subfolder, isFolder: item.IsFolder, id: item.Id, usePortalRoot: serviceConfig.usePortalRoot  } })
                    .then(svc.liveListReload);
            };

            // rename, then reload
            svc.rename = function rename(item, newName) {
                return $http.get(svc.url + "/rename", { params: { subfolder: svc.subfolder, isFolder: item.IsFolder, id: item.Id, usePortalRoot: serviceConfig.usePortalRoot, newName: newName } })
                    .then(svc.liveListReload);
            };
            
            svc.reload = svc.liveListReload;

            return svc;
        };
    }]);
(function () {
    /* jshint laxbreak:true */
    "use strict";

    BrowserController.$inject = ["$scope", "adamSvc", "debugState", "eavConfig", "eavAdminDialogs", "appRoot", "fileType"];
    var app = angular.module("Adam"); 

    // The controller for the main form directive
    app.controller("BrowserController", BrowserController);
    
    /*@ngInject*/
    function BrowserController($scope, adamSvc, debugState, eavConfig, eavAdminDialogs, appRoot, fileType) {
        var vm = this;
        vm.debug = debugState;

        var initConfig = function initConfig() {
            vm.contentTypeName = $scope.contentTypeName;
            vm.entityGuid = $scope.entityGuid;
            vm.fieldName = $scope.fieldName;
            vm.subFolder = $scope.subFolder || "";
            vm.showImagesOnly = $scope.showImagesOnly = $scope.showImagesOnly || false;

            vm.folderDepth = (typeof $scope.folderDepth !== 'undefined' && $scope.folderDepth !== null)
                ? $scope.folderDepth
                : 2;
            vm.showFolders = !!vm.folderDepth;
            vm.allowAssetsInRoot = $scope.allowAssetsInRoot || true;    // if true, the initial folder can have files, otherwise only subfolders
            vm.metadataContentTypes = $scope.metadataContentTypes || "";
        };

        initConfig();
        
        vm.show = false;
        vm.appRoot = appRoot;        
        vm.adamModeConfig = $scope.adamModeConfig;

        vm.disabled = $scope.ngDisabled;
        vm.enableSelect = ($scope.enableSelect === false) ? false : true; // must do it like this, $scope.enableSelect || true will not work

        vm.activate = function () {
            if($scope.autoLoad)
                vm.toggle();
            if ($scope.registerSelf)
                $scope.registerSelf(vm);
        };

        // load svc...
        vm.svc = adamSvc(vm.contentTypeName, vm.entityGuid, vm.fieldName, vm.subFolder, $scope.adamModeConfig);

        // refresh - also used by callback after an upload completed
        vm.refresh = vm.svc.liveListReload;

        vm.get = function () {
            vm.items = vm.svc.liveList();
            vm.folders = vm.svc.folders;
            vm.svc.liveListReload();
        };

        vm.toggle = function toggle(newConfig) {
            // Reload configuration
            initConfig();
            var configChanged = false;
            if (newConfig) {
                // Detect changes in config, allows correct toggle behaviour
                if (JSON.stringify(newConfig) !== vm.oldConfig)
                    configChanged = true;
                vm.oldConfig = JSON.stringify(newConfig);

                vm.showImagesOnly = newConfig.showImagesOnly;
                $scope.adamModeConfig.usePortalRoot = !!(newConfig.usePortalRoot);
            }

            vm.show = configChanged || !vm.show;
            
            if (!vm.show)
                $scope.adamModeConfig.usePortalRoot = false;

            // Override configuration in portal mode
            if ($scope.adamModeConfig.usePortalRoot) {
                vm.showFolders = true;
                vm.folderDepth = 99;
            }

            if (vm.show)
                vm.get();
        };

        vm.openUpload = function() {
            vm.dropzone.openUpload();
        };

        vm.select = function (fileItem) {
            if (vm.disabled || !vm.enableSelect)
                return;
            $scope.updateCallback(fileItem);
        };

        vm.addFolder = function () {
            if (vm.disabled)
                return;
            var folderName = window.prompt("Please enter a folder name"); // todo i18n
            if (folderName)
                vm.svc.addFolder(folderName)
                    .then(vm.refresh);
        };

        vm.del = function del(item) {
            if (vm.disabled)
                return;
            var ok = window.confirm("Are you sure you want to delete this item?"); // todo i18n
            if (ok)
                vm.svc.delete(item);
        };

        vm.rename = function rename(item) {
            var newName = window.prompt('Rename the file / folder to: ', item.Name);
            if (newName)
                vm.svc.rename(item, newName);
        };

        //#region Folder Navigation
        vm.goIntoFolder = function (folder) {
            var subFolder = vm.svc.goIntoFolder(folder);
            vm.subFolder = subFolder;
        };

        vm.goUp = function () {
            vm.subFolder = vm.svc.goUp();
        };

        vm.currentFolderDepth = function() {
            return vm.svc.folders.length;
        };

        vm.allowCreateFolder = function() {
            return vm.svc.folders.length < vm.folderDepth;
        };
        //#endregion

        //#region Metadata
        vm.editMetadata = function(item) {
            var items = [
                vm._itemDefinition(item, vm.getMetadataType(item))
            ];

            eavAdminDialogs.openEditItems(items, vm.refresh);
        };

        vm.getMetadataType = function(item) {
            var found;

            // check if it's a folder and if this has a special registration
            if (item.Type === "folder") {
                found = vm.metadataContentTypes.match(/^(folder)(:)([^\n]*)/im);
                if (found)
                    return found[3];
                else 
                    return null;
            }

            // check if the extension has a special registration
            // -- not implemented yet

            // check if the type "image" or "document" has a special registration
            // -- not implemneted yet


            // nothing found so far, go for the default with nothing as the prefix 
            found = vm.metadataContentTypes.match(/^([^:\n]*)(\n|$)/im);
            if (found)
                return found[1];

            // this is if we don't find anything
            return null;
        };

        // todo: move to service, shouldn't be part of the application
        vm._itemDefinition = function (item, metadataType) {
            var title = "EditFormTitle.Metadata"; // todo: i18n
            return item.MetadataId !== 0
                ? { EntityId: item.MetadataId, Title: title } // if defined, return the entity-number to edit
                : {
                    ContentTypeName: metadataType, // otherwise the content type for new-assegnment
                    Metadata: {
                        Key: (item.Type === "folder" ? "folder" : "file") + ":" + item.Id,
                        KeyType: "string",
                        TargetType: eavConfig.metadataOfCmsObject
                    },
                    Title: title,
                    Prefill: { EntityTitle: item.Name } // possibly prefill the entity title 
                };

        };

        //#endregion

        //#region icons
        vm.icon = function (item) {
            return fileType.getIconClass(item.Name);
        };
        //#endregion

        vm.allowedFileTypes = [];
        if ($scope.fileFilter) {
            vm.allowedFileTypes = $scope.fileFilter.split(',').map(function (i) {
                return i.replace('*', '').trim();
            });
        }

        vm.fileEndingFilter = function (item) {
            if (vm.allowedFileTypes.length === 0)
                return true;
            var extension = item.Name.match(/(?:\.([^.]+))?$/)[0];
            return vm.allowedFileTypes.indexOf(extension) != -1;
        };

        vm.activate();
    }

})();

(function () {
    /* jshint laxbreak:true*/

    angular.module("Adam")
        /*@ngInject*/
        .directive("adamBrowser", function () {
            return {
                restrict: "E",
                templateUrl: "adam/browser.html",

                //replace: true,
                transclude: false,
                require: "^dropzone",
                link: function postLink(scope, elem, attrs, dropzoneCtrl) {
                    // connect this adam to the dropzone
                    dropzoneCtrl.adam = scope.vm;       // so the dropzone controller knows what path etc.
                    scope.vm.dropzone = dropzoneCtrl;   // so we can require an "open file browse" dialog
                },

                // todo: change "scope" to bindToController whenever I have time - http://blog.thoughtram.io/angularjs/2015/01/02/exploring-angular-1.3-bindToController.html
                scope: {
                    // Identity fields
                    contentTypeName: "=",
                    entityGuid: "=",
                    fieldName: "=",

                    // configuration general
                    subFolder: "=",
                    folderDepth: "=", 
                    metadataContentTypes: "=",
                    allowAssetsInRoot: "=",
                    showImagesOnly: "=?",
                    adamModeConfig: "=",
                    fileFilter: "=?",

                    // binding and cross-component communication
                    autoLoad: "=",
                    updateCallback: "=",
                    registerSelf: "=",

                    // basic functionality
                    enableSelect: "=",
                    ngDisabled: "="
                },
                controller: "BrowserController",
                controllerAs: "vm"

            };
        });
})();
(function() {
    /* jshint laxbreak:true*/
    angular.module("Adam")
        /*@ngInject*/
        .directive("dropzoneUploadPreview", function () {
            return {
                restrict: "E",
                templateUrl: "adam/dropzone-upload-preview.html",
                replace: true,
                transclude: false
            };
        });
})();
/* js/fileAppDirectives */
(function() {
    angular.module("Adam")
        /*@ngInject*/
        .directive("dropzone", ["sxc", "tabId", "AppInstanceId", "ContentBlockId", "dragClass", "adamSvc", "$timeout", "$translate", function (sxc, tabId, AppInstanceId, ContentBlockId, dragClass, adamSvc, $timeout, $translate) {

            return {
                restrict: "C",
                link: postLink,

                // This controller is needed, because it needs an API which can talk to other directives
                controller: controller
            };


            // this is the method called after linking the directive, which initializes Dropzone
            function postLink(scope, element, attrs, controller) {
                var header = scope.$parent.to.header;
                var field = scope.$parent.options.key;
                var entityGuid = header.Guid;
                var svc = adamSvc(header.ContentTypeName, entityGuid, field, "", scope.$parent.vm.adamModeConfig);
                var url = svc.url;

                var config = {
                    url: url,
                    urlRoot: url,
                    maxFilesize: 10000, // 10'000 MB = 10 GB, note that it will also be stopped on the server if it's larger than the really allowed sized
                    paramName: "uploadfile",
                    maxThumbnailFilesize: 10,

                    headers: {
                        "ModuleId": AppInstanceId,
                        "TabId": tabId,
                        "ContentBlockId": ContentBlockId
                    },

                    dictDefaultMessage: "",
                    addRemoveLinks: false,
                    previewsContainer: ".field-" + field.toLowerCase() + " .dropzone-previews",
                    // we need a clickable, because otherwise the entire area is clickable. so i'm just making the preview clickable, as it's not important
                    clickable: ".field-" + field.toLowerCase() + " .invisible-clickable" // " .dropzone-adam"
                };


                var eventHandlers = {
                    'addedfile': function (file) {
                        $timeout(function () {
                            // anything you want can go here and will safely be run on the next digest.
                            scope.$apply(function () { // this must run in a timeout
                                scope.uploading = true;
                            });
                        });
                    },

                    "processing": function (file) {
                        this.options.url = svc.uploadUrl(controller.adam.subFolder);
                    },

                    'success': function (file, response) {
                        if (response.Success) {
                            svc.addFullPath(response); // calculate additional infos
                            scope.$parent.afterUpload(response);
                        } else {
                            alert("Upload failed because: " + response.Error);
                        }
                    },
                    'error': function (file, error, xhr) {
                        alert($translate.instant("Errors.AdamUploadError"));
                    },

                    "queuecomplete": function (file) {
                        if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                            scope.uploading = false;
                            controller.adam.refresh();
                        }
                    }
                };

                // delay building the dropszone till the DOM is ready
                $timeout(function () {
                    var dropzone = new Dropzone(element[0], config);

                    angular.forEach(eventHandlers, function(handler, event) {
                        dropzone.on(event, handler);
                    });

                    scope.processDropzone = function() { dropzone.processQueue(); };
                    scope.resetDropzone = function() { dropzone.removeAllFiles(); };
                    controller.openUpload = function() { dropzone.hiddenFileInput.click(); };

                }, 0);
            }

            /*@ngInject*/
            function controller() {
                var vm = this;
                vm.adam = {
                    show: false,
                    subFolder: "",
                    refresh: function () { }
                };

            }

        }]);


})();

(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    angular.module("sxcFieldTemplates", [
        "formly",
        "formlyBootstrap",
        "ui.bootstrap",
        "ui.tree",
        "2sxc4ng",
        "SxcEditTemplates",     // temp - was because of bad template-converter, remove once I update grunt
        "EavConfiguration",
        "SxcServices",
        "eavFieldTemplates",
        "Adam",
        //"ui.tinymce",   // connector to tiny-mce for angular
        "oc.lazyLoad"   // needed to lazy-load the MCE editor from the cloud
    ]);

})();
// This is the service which allows opening dnn-bridge dialogs and processes the results

angular.module("sxcFieldTemplates")
    /*@ngInject*/
    .factory("dnnBridgeSvc", ["$uibModal", "$http", "promiseToastr", function ($uibModal, $http, promiseToastr) {
        var svc = {};
        svc.open = function open(oldValue, params, callback) {
            var type = "pagepicker";

            var connector = {
                params: params,
                valueChanged: callback,
                dialogType: type
            };

            connector.valueChanged = function valueChanged(value) {
                connector.modalInstance.close();
                callback(value);
            };

            connector.params.CurrentValue = oldValue;

            console.log("before open page picker");
            console.log($uibModal);
            connector.modalInstance = $uibModal.open({
                templateUrl: "fields/dnn-bridge/hyperlink-default-pagepicker.html",
                resolve: {
                    bridge: function () {
                        return connector;
                    }
                },
                /*@ngInject*/
                controller: ["$scope", "bridge", function ($scope, bridge) {
                    $scope.bridge = bridge;
                }],
                windowClass: "sxc-dialog-filemanager"
            });
            console.log("after open page picker");

            return connector.modalInstance;
        };

        // 2017-08-12 2dm looks unused now
        // convert the url to a Id-code
        //svc.convertPathToId = function(path, type) {
        //    var pathWithoutVersion = path.replace(/\?ver=[0-9\-]*$/gi, "");
        //    // todo: working on https://github.com/2sic/2sxc/issues/656 but can't reproduce error
        //    // this is why I tried ignoreErrors and promisetoaster, but atm there is nothing to work on...
        //    var promise = $http.get("dnn/Hyperlink/GetFileByPath?relativePath=" + encodeURIComponent(pathWithoutVersion),
        //    {
        //        //ignoreErrors: true
        //    });
        //    return promiseToastr(promise, "Edit.Field.Hyperlink.Message.Loading", "Edit.Field.Hyperlink.Message.Ok", "Edit.Field.Hyperlink.Message.Error", 0, 0, 1000);
        //};

        // handle short-ID links like file:17
        svc.getUrlOfId = function(idCode) {
            var linkLowered = idCode.toLowerCase();
            if (linkLowered.indexOf("file:") !== -1 || linkLowered.indexOf("page:") !== -1)
                return $http.get("dnn/Hyperlink/ResolveHyperlink?hyperlink=" + encodeURIComponent(idCode));
            return null;
        };

        return svc;

    }]);

// this is in charge of the iframe which shows the dnn-bridge components

(function () {
	"use strict";

	angular.module("sxcFieldTemplates")


    /*@ngInject*/
	.directive("webFormsBridge", ["sxc", "portalRoot", function (sxc, portalRoot) {
	    var webFormsBridgeUrl = portalRoot + "Default.aspx?tabid=" + $2sxc.urlParams.require("tid") + "&ctl=webformsbridge&mid=" + sxc.id + "&dnnprintmode=true&SkinSrc=%5bG%5dSkins%2f_default%2fNo+Skin&ContainerSrc=%5bG%5dContainers%2f_default%2fNo+Container"; //"&popUp=true";

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
                    // test if the connectBridge works, if not, it's usually a telerik-not-installed issue
				    if (!w.connectBridge)
				        return alert("can't connect to the dialog - you are probably running a new DNN (v.8+) and didn't activate the old Telerik components. Please install these in the host > extensions to get this to work");
				    
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
						w.$(w.document).on('triggerbridgeresize', function () {
						    window.setTimeout(resize, 0);
						});

					}
				});
			}
		};
	}]);

})();
(function() {
    "use strict";

    angular.module("sxcFieldTemplates")
        .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {

            var wrappers = defaultFieldWrappers.slice(0); // copy the array
            wrappers.splice(defaultFieldWrappers.indexOf("eavLocalization"), 1); // remove the localization...

            formlyConfigProvider.setType({
                name: "entity-content-blocks",
                templateUrl: "fields/entity/entity-default.html",
                wrapper: wrappers, // ["eavLabel", "bootstrapHasError", "collapsible"],
                controller: "FieldTemplate-EntityContentBlockCtrl"
            });
        }])
        /*@ngInject*/
        .controller("FieldTemplate-EntityContentBlockCtrl", ["$controller", "$scope", "$http", "$filter", "$translate", "$uibModal", "appId", "eavAdminDialogs", "eavDefaultValueService", function($controller, $scope, $http, $filter, $translate, $uibModal, appId, eavAdminDialogs, eavDefaultValueService) {
            $scope.to.settings.merged.EnableRemove = true;
            $scope.to.settings.merged.AllowMultiValue = true; // for correct UI showing "remove"
            $scope.to.settings.merged.EnableAddExisting = false; // disable manual select existing
            $scope.to.settings.merged.EnableCreate = false;         // disable manual create
            $scope.to.settings.merged.EnableEdit = false;
            $scope.to.settings.merged.EntityType = "ContentGroupReference";
            $scope.to.enableCollapseField = true;   // ui option to allow collapsing
            $scope.to.collapseField = true;   // ui option to allow collapsing


            // use "inherited" controller just like described in http://stackoverflow.com/questions/18461263/can-an-angularjs-controller-inherit-from-another-controller-in-the-same-module
            $controller("FieldTemplate-EntityCtrl", { $scope: $scope });

            // do something with the values...
            var vals = $scope.model[$scope.options.key].Values[0].Value;

            //addCSSRule("div", "background-color: pink");
        }]);

    function addCSSRule(selector, rules, index) {
        var sheet = document.styleSheets[0];
        if ("insertRule" in sheet) {
            sheet.insertRule(selector + "{" + rules + "}", index);
        }
        else if ("addRule" in sheet) {
            sheet.addRule(selector, rules, index);
        }
    }

})();

(function() {
    "use strict";

    angular.module("sxcFieldTemplates")
        .config(["formlyConfigProvider", "fieldWrappersWithPreview", function (formlyConfigProvider, fieldWrappersWithPreview) {

            formlyConfigProvider.setType({
                name: "hyperlink-default",
                templateUrl: "fields/hyperlink/hyperlink-default.html",
                wrapper: fieldWrappersWithPreview,
                controller: "FieldTemplate-HyperlinkCtrl as vm"
            });
        }])
        /*@ngInject*/
        .controller("FieldTemplate-HyperlinkCtrl", ["$uibModal", "$scope", "$http", "sxc", "adamSvc", "debugState", "dnnBridgeSvc", "fileType", function ($uibModal, $scope, $http, sxc, adamSvc, debugState, dnnBridgeSvc, fileType) {

            var vm = this;
            vm.debug = debugState;
            vm.testLink = "";

            vm.isImage = function () { return fileType.isImage(vm.testLink); };
            vm.thumbnailUrl = function thumbnailUrl(size, quote) {
                var result = vm.testLink;
                if (size === 1)
                    result = result + "?w=64&h=64&mode=crop";
                if (size === 2)
                    result = result + "?w=500&h=400&mode=max";
                var qt = quote ? "\"" : "";
                return qt + result + qt;
            };

            vm.icon = function () { return fileType.getIconClass(vm.testLink); };
            vm.tooltipUrl = function (str) { return str.replace(/\//g, "/&#8203;"); };
            vm.adamModeConfig = {
                usePortalRoot: false
            };

            function ensureDefaultConfig() {
                var merged = $scope.to.settings.merged;
                if (merged.ShowAdam === undefined || merged.ShowAdam === null) merged.ShowAdam = true;
                if (merged.Buttons === undefined || merged.Buttons === null) merged.Buttons = "adam,more";
            }

            ensureDefaultConfig();

            // Update test-link if necessary - both when typing or if link was set by dialogs
            $scope.$watch("value.Value", function(newValue, oldValue) {
                if (!newValue)
                    return;

                // handle short-ID links like file:17
                var promise = dnnBridgeSvc.getUrlOfId(newValue);
                if(promise)
                    promise.then(function (result) {
                        if (result.data) 
                            vm.testLink = result.data;
                    });
                else 
                    vm.testLink = newValue;
            });

            //#region dnn-page picker dialog

            // the callback when something was selected
            vm.processResultOfPagePicker = function (value) {
                $scope.$apply(function() {
                    // Convert to page:xyz format (if it wasn't cancelled)
                    if (value)
                        $scope.value.Value = "page:" + value.id;
                });
            };

            // open the dialog
            vm.openPageDialog = function () {
                dnnBridgeSvc.open(
                    $scope.value.Value,
                    {
                        Paths: $scope.to.settings.merged ? $scope.to.settings.merged.Paths : "",
                        FileFilter: $scope.to.settings.merged ? $scope.to.settings.merged.FileFilter : ""
                    },
                    vm.processResultOfPagePicker);
            };
            //#endregion dnn page picker

            //#region new adam: callbacks only
            vm.registerAdam = function(adam) {
                vm.adam = adam;
            };
            vm.setValue = function(fileItem) {
                $scope.value.Value = "File:" + fileItem.Id;
            };

            $scope.afterUpload = vm.setValue;   // binding for dropzone

            vm.toggleAdam = function toggle(usePortalRoot, imagesOnly) {
                
                vm.adam.toggle({
                    showImagesOnly: imagesOnly,
                    usePortalRoot: usePortalRoot
                });
            };

            //#endregion


        }]);


})();

(function() {
    "use strict";

    angular.module("sxcFieldTemplates")
        .config(["formlyConfigProvider", "defaultFieldWrappersNoFloat", function (formlyConfigProvider, defaultFieldWrappersNoFloat) {

            formlyConfigProvider.setType({
                name: "hyperlink-library",
                templateUrl: "fields/hyperlink/hyperlink-library.html",
                // todo: check if we could use the defaultFieldWrappers instead
                wrapper: defaultFieldWrappersNoFloat,// ["eavLabel", "bootstrapHasError", "eavLocalization", "collapsible"],
                controller: "FieldTemplate-Library as vm"
            });

        }])
        /*@ngInject*/
        .controller("FieldTemplate-Library", ["$uibModal", "$scope", "$http", "sxc", "adamSvc", "debugState", function ($uibModal, $scope, $http, sxc, adamSvc, debugState) {

            var vm = this;
            vm.debug = debugState;
            vm.modalInstance = null;
            vm.testLink = "";

            vm.adamModeConfig = {
                usePortalRoot: false
            };

            //#region new adam: callbacks only
            vm.registerAdam = function(adam) {
                vm.adam = adam;
            };
            //vm.setValue = function(url) {
            //    $scope.value.Value = url;
            //};
            $scope.afterUpload = function(fileItem) {};

            vm.toggleAdam = function toggle() {
                vm.adam.toggle();
            };

            //#endregion


        }]);


})();
/*!
Math.uuid.js (v1.4)
http://www.broofa.com
mailto:robert@broofa.com

Copyright (c) 2010 Robert Kieffer
Dual licensed under the MIT and GPL licenses.
*/

/*
 * Generate a random uuid.
 *
 * USAGE: Math.uuid(length, radix)
 *   length - the desired number of characters
 *   radix  - the number of allowable values for each character.
 *
 * EXAMPLES:
 *   // No arguments  - returns RFC4122, version 4 ID
 *   >>> Math.uuid()
 *   "92329D39-6F5C-4520-ABFC-AAB64544E172"
 *
 *   // One argument - returns ID of the specified length
 *   >>> Math.uuid(15)     // 15 character ID (default base=62)
 *   "VcydxgltxrVZSTV"
 *
 *   // Two arguments - returns ID of the specified length, and radix. (Radix must be <= 62)
 *   >>> Math.uuid(8, 2)  // 8 character ID (base=2)
 *   "01001010"
 *   >>> Math.uuid(8, 10) // 8 character ID (base=10)
 *   "47473046"
 *   >>> Math.uuid(8, 16) // 8 character ID (base=16)
 *   "098F4D35"
 */
(function () {
    // Private array of chars to use
    var CHARS = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'.split('');

    Math.uuid = function (len, radix) {
        var chars = CHARS, uuid = [], i;
        radix = radix || chars.length;

        if (len) {
            // Compact form
            for (i = 0; i < len; i++) uuid[i] = chars[0 | Math.random() * radix];
        } else {
            // rfc4122, version 4 form
            var r;

            // rfc4122 requires these characters
            uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
            uuid[14] = '4';

            // Fill in random data.  At i==19 set the high bits of clock sequence as
            // per rfc4122, sec. 4.1.5
            for (i = 0; i < 36; i++) {
                if (!uuid[i]) {
                    r = 0 | Math.random() * 16;
                    uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r];
                }
            }
        }

        return uuid.join('');
    };

    // A more performant, but slightly bulkier, RFC4122v4 solution.  We boost performance
    // by minimizing calls to random()
    Math.uuidFast = function () {
        var chars = CHARS, uuid = new Array(36), rnd = 0, r;
        for (var i = 0; i < 36; i++) {
            if (i == 8 || i == 13 || i == 18 || i == 23) {
                uuid[i] = '-';
            } else if (i == 14) {
                uuid[i] = '4';
            } else {
                if (rnd <= 0x02) rnd = 0x2000000 + (Math.random() * 0x1000000) | 0;
                r = rnd & 0xf;
                rnd = rnd >> 4;
                uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r];
            }
        }
        return uuid.join('');
    };

    // A more compact, but less performant, RFC4122v4 solution:
    Math.uuidCompact = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    };
})();
/* 
 * Field: String - font-icon picker
 */

angular.module("sxcFieldTemplates")
    .config(["formlyConfigProvider", "fieldWrappersWithPreview", function (formlyConfigProvider, fieldWrappersWithPreview) {

        formlyConfigProvider.setType({
            name: "string-font-icon-picker",
            templateUrl: "fields/string/string-font-icon-picker.html",
            wrapper: fieldWrappersWithPreview,
            controller: "FieldTemplate-String-Font-Icon-Picker as vm"
        });

    }])
    /*@ngInject*/
    .controller("FieldTemplate-String-Font-Icon-Picker", ["$scope", "debugState", "$ocLazyLoad", "appRoot", function ($scope, debugState, $ocLazyLoad, appRoot) {
        var vm = angular.extend(this, {
            iconFilter: "", // used for in-line search
            prefix: "", // used to find the right css-classes
            previewPrefix: "", // used to preview the icon, in addition to the built-in class
            icons: [], // list of icons, to be filled
            useTestValues: false, // to prefill with test-values, in case needed
            selectorIsOpen: false
    });


        //#region icon css-class-methods
        function getIconClasses(className) {
            var charcount = className.length, foundList = [], duplicateDetector = {};
            if (!className) return foundList;
            for (var ssSet = 0; ssSet < document.styleSheets.length; ssSet++) {
                var classes = document.styleSheets[ssSet].rules || document.styleSheets[ssSet].cssRules;
                if(classes)
                    for (var x = 0; x < classes.length; x++)
                        if (classes[x].selectorText && classes[x].selectorText.substring(0, charcount) === className) {
                            // prevent duplicate-add...
                            var txt = classes[x].selectorText,
                                icnClass = txt.substring(0, txt.indexOf(":")).replace(".", "");
                            if (!duplicateDetector[icnClass]) {
                                foundList.push({ rule: classes[x], 'class': icnClass });
                                duplicateDetector[icnClass] = true;
                            }
                        }
            }
            return foundList;
        }

//#endregion

        //#region load additional resources
        function loadAdditionalResources(files) {
            files = files || "";
            var mapped = files.replace("[App:Path]", appRoot)
                .replace(/([\w])\/\/([\w])/g,   // match any double // but not if part of https or just "//" at the beginning
                "$1/$2");
            var fileList = mapped ? mapped.split("\n") : [];
            return $ocLazyLoad.load(fileList);
        }
        //#endregion

        vm.setIcon = function(newValue) {
            $scope.value.Value = newValue;
            vm.selectorIsOpen = false;
            //$scope.status.isopen = false;
            $scope.form.$setDirty();
        };

        vm.activate = function() {
            // get configured
            var controlSettings = $scope.to.settings["string-font-icon-picker"];
            vm.files = (controlSettings) ? controlSettings.Files || "" : "";
            vm.prefix = (controlSettings) ? controlSettings.CssPrefix || "" : "";
            vm.previewPrefix = (controlSettings) ? controlSettings.PreviewCss || "" : "";

            if (vm.useTestValues)
                angular.extend(vm, {
                    iconFilter: "",
                    prefix: ".glyphicon-",
                    previewPrefix: "glyphicon",
                });

            // load all additional css, THEN load the icons
            loadAdditionalResources(vm.files).then(function() {
                // load the icons
                vm.icons = getIconClasses(vm.prefix);

            });

            vm.debug = debugState;
            if (debugState.on) console.log($scope.options.templateOptions);
        };

        vm.activate();
    }]);
// This isn't fully nice, but it's the code used by the template picker elements
// it's outsourced here, to ensure that code revisions are clear and api consistent
// because the previous version had the code inside the field-config
// and was highly dependent on the angular/formly API, which changes a bit from time to time
// this makes it easier to keep it in sync

// I'll have to think of a better way to provide/inject this in the future...

// must find out if we're in App or Content before we do more...

(function () {
    var cs = {
        init: function(context) {
            cs.context = context;

            // get angular injector to get a service
            var inj = context.$injector;

            var appDialogConfigSvc = inj.get("appDialogConfigSvc");

            appDialogConfigSvc.getDialogSettings().then(function(result) {
                var config = result.data;
                // if this is a content-app, disable two fields
                if (config.IsContent) {
                    //disable current field
                    context.formVm.formFields[context.field.SortOrder].templateOptions.disabled = true;
                    // do the same for the data-field (field 20)
                    context.formVm.formFields[20].templateOptions.disabled = true;
                }
            });

        }
    };

    window["2sxc-template-picker-custom-script-for-name-field"] = cs;
})();
/* 
 * Field: String - Dropdown
 */

angular.module("sxcFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function(formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-template-picker",
            templateUrl: "fields/string/string-template-picker.html",
            wrapper: defaultFieldWrappers,
            controller: "FieldTemplate-String-TemplatePicker"
        });

    }])
    /*@ngInject*/
    .controller("FieldTemplate-String-TemplatePicker", ["$scope", "appAssetsSvc", "appId", "fieldMask", function ($scope, appAssetsSvc, appId, fieldMask) {

        function activate() {
            // ensure settings are merged
            if (!$scope.to.settings.merged)
                $scope.to.settings.merged = {};

            $scope.setFileConfig("Token"); // use token setting as default, till the UI tells us otherwise

            // clean up existing paths, because some used "/" and some "\" for paths, so it wouldn't match in the drop-down
            if ($scope.options && $scope.options.value())
                angular.forEach($scope.options.value().Values, function(v, i) {
                    v.Value = v.Value.replace("\\", "/");
                });
            

            // set change-watchers to the other values
            $scope.typeWatcher = fieldMask("[Type]", $scope, $scope.setFileConfig);
            $scope.locWatcher = fieldMask("[Location]", $scope, $scope.onLocationChange);

            // create initial list for binding
            $scope.templates = [];

            $scope.svcApp = appAssetsSvc(appId, false);
            $scope.svcGlobal = appAssetsSvc(appId, true);

            $scope.onLocationChange(); // set initial file list
            //$scope.templates = $scope.svcApp.liveList();
        }

        $scope.readyToUse = function() {
            return $scope.typeWatcher.value && $scope.locWatcher.value; // check if these have real values inside
        };

        $scope.setFileConfig = function (type) {
            var specs = {
                "Token": { ext: ".html", prefix: "", suggestion: "yourfile.html", body: "<p>You successfully created your own template. Start editing it by hovering the \"Manage\" button and opening the \"Edit Template\" dialog.</p>" },
                "C# Razor": { ext: ".cshtml", prefix: "_", suggestion: "_yourfile.cshtml", body: "<p>You successfully created your own template. Start editing it by hovering the \"Manage\" button and opening the \"Edit Template\" dialog.</p>" }
            };
            $scope.file = specs[type];

        };

        // when the watcher says the location changed, reset stuff
        $scope.onLocationChange = function(loc) {
            $scope.svcCurrent = (loc === "Host File System") 
                ? $scope.svcGlobal
                : $scope.svcApp;

            $scope.templates = $scope.svcCurrent.liveList();
        };

        // ask for a new file name and 
        $scope.add = function() {
            var fileName = prompt("enter new file name", $scope.file.suggestion); // todo: i18n

            if (!fileName)
                return;

            // 1. check for folders
            var path = "";
            fileName = fileName.replace("\\", "/");
            var foundSlash = fileName.lastIndexOf("/");
            if (foundSlash > -1) {
                path = fileName.substring(0, foundSlash + 1); // path with slash
                fileName = fileName.substring(foundSlash + 1);
            }

            // 2. check if extension already provided, otherwise or if not perfect, just attach default
            if (!fileName.endsWith($scope.file.ext))// fileName.indexOf($scope.fileExt) !== fileName.length - $scope.fileExt.length)
                fileName += $scope.file.ext;

            // 3. check if cshtmls have a "_" in the file name (not folder, must be the file name part)
            if ($scope.file.prefix !== "" && fileName[0] !== $scope.file.prefix)
                fileName = $scope.file.prefix + fileName;

            var fullPath = path + fileName;
            console.log(fullPath);

            // 4. tell service to create it
            $scope.svcCurrent.create(fullPath, $scope.file.body)
                .then(function() {
                    $scope.value.Value = fullPath; // set the dropdown to the new file
                });
        };

        activate();

    }])

    // filter to only show files which are applicable to this
    .filter("isValidFile", function() {

        // Create the return function
        // set the required parameter name to **number**
        return function(paths, ext) {
            var out = [];
            angular.forEach(paths, function(path) {
                if (path.slice(path.length - ext.length) === ext)
                    out.push(path);
            });
            return out;
        };
    });


(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    angular.module("sxcFieldTemplates")
        .config(["formlyConfigProvider", function(formlyConfigProvider) {

            // for now identical with -adv, but later will change
            formlyConfigProvider.setType({
                name: "string-wysiwyg-adv",
                templateUrl: "fields/string/string-wysiwyg-adv.html",
                // todo: check if we could use the defaultFieldWrappers instead
                wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization", "collapsible"]
            });


        }]);

})();

(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	angular.module("sxcFieldTemplates")

    .config(["formlyConfigProvider", function (formlyConfigProvider) {

        // for now identical with -adv, but later will change
		formlyConfigProvider.setType({
			name: "string-wysiwyg-dnn",
			templateUrl: "fields/string/string-wysiwyg-dnn.html",
		    // todo: check if we could use the defaultFieldWrappers instead
			wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization", "collapsible"],
			controller: "FieldTemplate-WysiwygCtrl as vm"
		});

        
	}])


    /*@ngInject*/
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

(function () {
	"use strict";

    // Register in Angular Formly
    FieldWysiwygTinyMceController.$inject = ["$scope", "languages", "tinyMceHelpers", "tinyMceToolbars", "tinyMceConfig", "tinyMceAdam", "tinyMceDnnBridge"];
    angular.module("sxcFieldTemplates")
        .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {
            formlyConfigProvider.setType({
                name: "string-wysiwyg-tinymce",
                templateUrl: "fields/string/string-wysiwyg-tinymce.html",
                wrapper: defaultFieldWrappers, 
                controller: "FieldWysiwygTinyMce as vm"
            });
        }])

        .controller("FieldWysiwygTinyMce", FieldWysiwygTinyMceController);

    /*@ngInject*/
    function FieldWysiwygTinyMceController($scope, languages, tinyMceHelpers, tinyMceToolbars, tinyMceConfig, tinyMceAdam, tinyMceDnnBridge) {
        var vm = this;
        vm.enableContentBlocks = true;

        var settings = {
            enableContentBlocks : false
        };

        vm.adamModeConfig = {
            usePortalRoot: false
        };

        vm.activate = function () {

            enableContentBlocksIfPossible(settings);

            // initialize options and wire-up init-callback
            $scope.tinymceOptions = angular.extend(tinyMceConfig.getDefaultOptions(settings), {
                setup: tinyMceInitCallback
            });

            // add ADAM definition, so that the callback will be able to link up to this
            tinyMceAdam.attachAdam(vm, $scope);

            // add DNN Bridge, needed for webforms dnn-dialogs
            tinyMceDnnBridge.attach(vm, $scope);

            // check if it's an additionally translated language and load the translations
            var lang2 = /* "de" */ languages.currentLanguage.substr(0, 2);
            if (tinyMceConfig.languages.indexOf(lang2) >= 0)
                angular.extend($scope.tinymceOptions, {
                    language: lang2,
                    language_url: "../i18n/lib/tinymce/" + lang2 + ".js"
                });

            watchDisabled($scope);
        };
        vm.activate();

        // callback event which tinyMce will execute when it's built the editor
        function tinyMceInitCallback(editor) {
            vm.editor = editor;
            if ($scope.tinymceOptions.language)
                tinyMceHelpers.addTranslations(editor, $scope.tinymceOptions.language);

            tinyMceToolbars.addButtons(vm);
            tinyMceAdam.addButtons(vm);

            enableContentBlocksIfPossible(editor);
        }

        function watchDisabled(ngscope) {
            // Monitor for changes on Disabled
            ngscope.$watch("to.disabled", function(newValue, oldValue) {
                if (newValue !== oldValue && vm.editor !== null) {
                    ngscope.tinymceOptions.readonly = newValue;
                    ngscope.$broadcast("$tinymce:refresh"); // Refresh tinymce instance to pick-up new readonly value
                }
            });
        }

        function enableContentBlocksIfPossible(settings) {
            // quit if there are no following fields
            if ($scope.fields.length === $scope.index + 1)
                return;

            var nextField = $scope.fields[$scope.index + 1];
            if (nextField.type === "entity-content-blocks")
                settings.enableContentBlocks = true;
        }
    }



})();



angular.module("sxcFieldTemplates")
    /*@ngInject*/
    .factory("tinyMceAdam", function () {
        return {
            attachAdam: attachAdam,
            addButtons: addAdamButtons
        };

        function attachAdam(vm, $scope) {
            vm.registerAdam = function (adam) {
                vm.adam = adam;
            };

            vm.setValue = function (fileItem, modeImage) {
                if (modeImage === undefined)        // if not supplied, use the setting in the adam
                    modeImage = vm.adamModeImage;
                vm.editor.insertContent(modeImage
                    ? "<img src=\"" + fileItem.fullPath + "\">"
                    : "<a href=\"" + fileItem.fullPath + "\">" + fileItem.Name.substr(0, fileItem.Name.lastIndexOf(".")) + "</a>");
            };

            // this is the event called by dropzone as something is dropped
            $scope.afterUpload = function (fileItem) {
                vm.setValue(fileItem, fileItem.Type === "image");
            };

            vm.toggleAdam = function toggle(imagesOnly, usePortalRoot) {
                vm.adamModeImage = imagesOnly;
                //vm.adamModeConfig.usePortalRoot = !!usePortalRoot;
                vm.adam.toggle({
                    showImagesOnly: imagesOnly,
                    usePortalRoot: usePortalRoot
                });
                $scope.$apply();
            };
        }

        function addAdamButtons(vm) {
            var e = vm.editor;
            // group with adam-link, dnn-link
            e.addButton("linkfiles", {
                type: "splitbutton",
                icon: " eav-icon-file-pdf",
                title: "Link.AdamFile.Tooltip",
                onclick: function () {
                    vm.toggleAdam(false);
                },
                menu: [
                    {
                        text: "Link.AdamFile",
                        tooltip: "Link.AdamFile.Tooltip",
                        icon: " eav-icon-file-pdf",
                        onclick: function () {
                            vm.toggleAdam(false, false);
                        }
                    }, {
                        text: "Link.DnnFile",
                        tooltip: "Link.DnnFile.Tooltip",
                        icon: " eav-icon-file",
                        onclick: function () {
                            vm.toggleAdam(false, true);
                        }
                    }
                ]
            });


            // group with images (adam) - only in PRO mode
            e.addButton("images", {
                type: "splitbutton",
                text: "",
                icon: "image",
                onclick: function () {
                    vm.toggleAdam(true);
                },
                menu: [
                    {
                        text: "Image.AdamImage",
                        tooltip: "Image.AdamImage.Tooltip",
                        icon: "image",
                        onclick: function () { vm.toggleAdam(true); }
                    }, {
                        text: "Image.DnnImage",
                        tooltip: "Image.DnnImage.Tooltip",
                        icon: "image",
                        onclick: function () { vm.toggleAdam(true, true); }
                    }, {
                        text: "Insert\/edit image", // i18n tinyMce standard
                        icon: "image",
                        onclick: function () { e.execCommand("mceImage"); }

                    },
                    // note: all these use i18n from tinyMce standard
                    { icon: "alignleft", tooltip: "Align left", onclick: function () { e.execCommand("JustifyLeft"); } },
                    { icon: "aligncenter", tooltip: "Align center", onclick: function () { e.execCommand("JustifyCenter"); } },
                    { icon: "alignright", tooltip: "Align right", onclick: function () { e.execCommand("JustifyRight"); } }
                ]
            });

        }
    });
angular.module("sxcFieldTemplates")
    /*@ngInject*/
    .factory("tinyMceConfig", ["beta", function (beta) {
        var svc = {
            // cdn root
            cdnRoot: "//cdn.tinymce.com/4",
            // these are the sizes we can auto-resize to
            imgSizes: [100, 75, 70, 66, 60, 50, 40, 33, 30, 25, 10],

            // the default language, in which we have all labels/translations
            defaultLanguage: "en",

            // all other languages
            languages: "de,es,fr,it,uk,nl".split(","),

            // tinyMCE plugins we're using
            plugins: [
                "code", // allow view / edit source
                "contextmenu", // right-click menu for things like insert, etc.
                "autolink", // automatically convert www.xxx links to real links
                "tabfocus", // get in an out of the editor with tab
                "image", // image button and image-settings
                "link", // link button + ctrl+k to add link
                // "autosave",     // temp-backups the content in case the browser crashes, allows restore
                "paste", // enables paste as text from word etc. https://www.tinymce.com/docs/plugins/paste/
                "anchor", // allows users to set an anchor inside the text
                "charmap", // character map https://www.tinymce.com/docs/plugins/visualchars/
                "hr", // hr
                "media", // video embed
                "nonbreaking", // add button to insert &nbsp; https://www.tinymce.com/docs/plugins/nonbreaking/
                "searchreplace", // search/replace https://www.tinymce.com/docs/plugins/searchreplace/
                "table", // https://www.tinymce.com/docs/plugins/searchreplace/
                "lists", // should fix bug with fonts in list-items (https://github.com/tinymce/tinymce/issues/2330),
                "textpattern" // enable typing like "1. text" to create lists etc.
            ],
            
            validateAlso: '@[class]' // allow classes on all elements, 
                    + ',i' // allow i elements (allows icon-font tags like <i class="fa fa-...">)
                    + ",hr[sxc|guid]" // experimental: allow inline content-blocks
        };

        function buildModes(settings) {
            // the WYSIWYG-modes we offer, standard with simple toolbar and advanced with much more
            modes = {
                standard: {
                    menubar: false,
                    toolbar: " undo redo removeformat "
                        + "| bold formatgroup "
                        + "| h1 h2 hgroup "
                        + "| listgroup "
                        + "| linkfiles linkgroup "
                        + "| " + (settings.enableContentBlocks ? " addcontentblock " : "")+ "modeadvanced ",
                    contextmenu: "charmap hr" + (settings.enableContentBlocks ? " addcontentblock" : "")
                },
                advanced: {
                    menubar: true,
                    toolbar: " undo redo removeformat "
                        + "| styleselect "
                        + "| bold italic "
                        + "| h1 h2 hgroup "
                        + "| bullist numlist outdent indent "
                        + "| images linkfiles linkgrouppro "
                        + "| code modestandard ",
                    contextmenu: "link image | charmap hr adamimage"
                }
            };
            return modes;
        }

        svc.getDefaultOptions = function (settings) {
            var modes = buildModes(settings);
            return {
                baseURL: svc.cdnRoot,
                inline: true, // use the div, not an iframe
                automatic_uploads: false, // we're using our own upload mechanism
                modes: modes, // for later switch to another mode
                menubar: modes.standard.menubar, // basic menu (none)
                toolbar: modes.standard.toolbar, // basic toolbar
                plugins: svc.plugins.join(" "),
                contextmenu: modes.standard.contextmenu, //"link image | charmap hr adamimage",
                autosave_ask_before_unload: false,
                paste_as_text: true,
                extended_valid_elements: svc.validateAlso,
                //'@[class]' // allow classes on all elements, 
                //+ ',i' // allow i elements (allows icon-font tags like <i class="fa fa-...">)
                //+ ",hr[sxc|guid]", // experimental: allow inline content-blocks
                custom_elements: "hr",

                // Url Rewriting in images and pages
                //convert_urls: false,  // don't use this, would keep the domain which is often a test-domain
                relative_urls: false, // keep urls with full path so starting with a "/" - otherwise it would rewrite them to a "../../.." syntax
                default_link_target: "_blank", // auto-use blank as default link-target
                object_resizing: false, // don't allow manual scaling of images

                // General looks
                skin: "lightgray",
                theme: "modern",
                // statusbar: true,    // doesn't work in inline :(

                language: svc.defaultLanguage,

                debounce: false // DONT slow-down model updates - otherwise we sometimes miss the last changes
            };
        };

        return svc;
    }]);
angular.module("sxcFieldTemplates")
    /*@ngInject*/
    .factory("tinyMceDnnBridge", ["dnnBridgeSvc", function (dnnBridgeSvc) {
        return {
            attach: attach
        };

        function attach(vm, $scope) {

            // the callback when something was selected
            vm.processResultOfDnnBridge = function (value) {
                $scope.$apply(function () {
                    if (!value) return;

                    var previouslySelected = vm.editor.selection.getContent();
                    
                    var promise = dnnBridgeSvc.getUrlOfId("page:" + (value.id || value.FileId)); // id on page, FileId on file
                    return promise.then(function (result) {
                        vm.editor.insertContent("<a href=\"" + result.data + "\">" + (previouslySelected || value.name) + "</a>");
                    });

                });
            };

            // open the dialog - note: strong dependency on the buttons, not perfect here
            vm.openDnnDialog = function (type) {
                dnnBridgeSvc.open("", { Paths: null, FileFilter: null }, vm.processResultOfDnnBridge);
            };

        }

    }]);
angular.module("sxcFieldTemplates")
    /*@ngInject*/
    .factory("tinyMceHelpers", ["$translate", "tinyMceConfig", function ($translate, tinyMceConfig) {
        var svc = {
            addTranslations: initLangResources
        };

        // Initialize the tinymce resources which we translate ourselves
        function initLangResources(editor, language) {
            var primaryLan = tinyMceConfig.defaultLanguage;
            var keys = [], mceTranslations = {}, prefix = "Extension.TinyMce.", pLen = prefix.length;

            // find all relevant keys by querying the primary language
            var all = $translate.getTranslationTable(primaryLan);
            // ReSharper disable once MissingHasOwnPropertyInForeach
            for (var key in all)
                if (key.indexOf(prefix) === 0)
                    keys.push(key);

            var translations = $translate.instant(keys);

            for (var k = 0; k < keys.length; k++)
                mceTranslations[keys[k].substring(pLen)] = translations[keys[k]];
            tinymce.addI18n(language, mceTranslations);
        }
        return svc;
    }]);
angular.module("sxcFieldTemplates")
    /*@ngInject*/
    .factory("tinyMceToolbars", ["tinyMceConfig", function (tinyMceConfig) {
        var svc = {
            addButtons: addTinyMceToolbarButtons
        };

        function addTinyMceToolbarButtons(vm) {
            var editor = vm.editor;
            //#region helpers like initOnPostRender(name)

            // helper function to add activate/deactivate to buttons like alignleft, alignright etc.
            function initOnPostRender(name) { // copied/modified from https://github.com/tinymce/tinymce/blob/ddfa0366fc700334f67b2c57f8c6e290abf0b222/js/tinymce/classes/ui/FormatControls.js#L232-L249
                return function () {
                    var self = this; // keep ref to the current button?

                    function watchChange() {
                        editor.formatter.formatChanged(name, function (state) {
                            self.active(state);
                        });
                    }

                    if (editor.formatter)
                        watchChange();
                    else
                        editor.on("init", watchChange());
                };
            }

            //#endregion

            //#region register formats

            // the method that will register all formats - like img-sizes
            function registerTinyMceFormats(editor, vm) {
                var imgformats = {};
                for (var is = 0; is < tinyMceConfig.imgSizes.length; is++)
                    imgformats["imgwidth" + tinyMceConfig.imgSizes[is]] = [{ selector: "img", collapsed: false, styles: { 'width': tinyMceConfig.imgSizes[is] + "%" } }];
                editor.formatter.register(imgformats);
            }

            // call register once the editor-object is ready
            editor.on('init', function () {
                registerTinyMceFormats(editor, vm);
            });

            //#endregion

            //// group with adam-link, dnn-link
            //editor.addButton("linkfiles", {
            //    type: "splitbutton",
            //    icon: " eav-icon-file-pdf",
            //    title: "Link.AdamFile.Tooltip",
            //    onclick: function () {
            //        vm.toggleAdam(false);
            //    },
            //    menu: [
            //        {
            //            text: "Link.AdamFile",
            //            tooltip: "Link.AdamFile.Tooltip",
            //            icon: " eav-icon-file-pdf",
            //            onclick: function () {
            //                vm.toggleAdam(false);
            //            }
            //        }, {
            //            text: "Link.DnnFile",
            //            tooltip: "Link.DnnFile.Tooltip",
            //            icon: " eav-icon-file",
            //            onclick: function () {
            //                vm.openDnnDialog("documentmanager");
            //            }
            //        }
            //    ]
            //});

            //#region link group with web-link, page-link, unlink, anchor
            var linkgroup = {
                type: "splitbutton",
                icon: "link",
                title: "Link",
                onPostRender: initOnPostRender("link"),
                onclick: function () {
                    editor.execCommand("mceLink");
                },

                menu: [
                { icon: "link", text: "Link", onclick: function () { editor.execCommand("mceLink"); } },
                {
                    text: "Link.Page",
                    tooltip: "Link.Page.Tooltip",
                    icon: " eav-icon-sitemap",
                    onclick: function () {
                        vm.openDnnDialog("pagepicker");
                    }
                }
                ]
            };
            var linkgroupPro = angular.copy(linkgroup);
            linkgroupPro.menu.push({ icon: " eav-icon-anchor", text: "Anchor", tooltip: "Link.Anchor.Tooltip", onclick: function () { editor.execCommand("mceAnchor"); } });
            editor.addButton("linkgroup", linkgroup);
            editor.addButton("linkgrouppro", linkgroupPro);
            //#endregion

            // group with images (adam) - only in PRO mode
            editor.addButton("images", {
                type: "splitbutton",
                text: "",
                icon: "image",
                onclick: function () {
                    vm.toggleAdam(true);
                },
                menu: [
                    {
                        text: "Image.AdamImage",
                        tooltip: "Image.AdamImage.Tooltip",
                        icon: "image",
                        onclick: function () { vm.toggleAdam(true); }
                    }, {
                        text: "Image.DnnImage",
                        tooltip: "Image.DnnImage.Tooltip",
                        icon: "image",
                        onclick: function () { vm.toggleAdam(true, true); }
                    }, {
                        text: "Insert\/edit image", // i18n tinyMce standard
                        icon: "image",
                        onclick: function () { editor.execCommand("mceImage"); }

                    },
                    // note: all these use i18n from tinyMce standard
                    { icon: "alignleft", tooltip: "Align left", onclick: function () { editor.execCommand("JustifyLeft"); } },
                    { icon: "aligncenter", tooltip: "Align center", onclick: function () { editor.execCommand("JustifyCenter"); } },
                    { icon: "alignright", tooltip: "Align right", onclick: function () { editor.execCommand("JustifyRight"); } }
                ]
            });

            // drop-down with italic, strikethrough, ...
            editor.addButton("formatgroup", {
                type: "splitbutton",
                tooltip: "Italic",  // will be autotranslated
                text: "",
                icon: "italic",
                cmd: "italic",
                onPostRender: initOnPostRender("italic"),
                menu: [
                    { icon: "strikethrough", text: "Strikethrough", onclick: function () { editor.execCommand("strikethrough"); } },
                    { icon: "superscript", text: "Superscript", onclick: function () { editor.execCommand("superscript"); } },
                    { icon: "subscript", text: "Subscript", onclick: function () { editor.execCommand("subscript"); } }
                ]

            });

            // drop-down with italic, strikethrough, ...
            editor.addButton("listgroup", {
                type: "splitbutton",
                tooltip: "Numbered list",  // official tinymce key
                text: "",
                icon: "numlist",
                cmd: "InsertOrderedList",
                onPostRender: initOnPostRender("numlist"),  // for unknown reasons, this just doesn't activate correctly :( - neither does the bullist
                menu: [
                    { icon: "bullist", text: "Bullet list", onPostRender: initOnPostRender("bullist"), onclick: function () { editor.execCommand("InsertUnorderedList"); } },
                    { icon: "outdent", text: "Outdent", onclick: function () { editor.execCommand("Outdent"); } },
                    { icon: "indent", text: "Indent", onclick: function () { editor.execCommand("Indent"); } }
                ]

            });

            //#region mode switching and the buttons for it
            function switchModes(mode) {
                editor.settings.toolbar = editor.settings.modes[mode].toolbar;
                editor.settings.menubar = editor.settings.modes[mode].menubar;
                // editor.settings.contextmenu = editor.settings.modes[mode].contextmenu; - doesn't work at the moment

                editor.theme.panel.remove();    // kill current toolbar
                editor.theme.renderUI(editor);
                editor.execCommand("mceFocus");

                // focus away...
                document.getElementById("dummyfocus").focus();

                // ...and focus back a bit later
                setTimeout(function () {
                    editor.focus();
                }, 100);
            }

            editor.addButton("modestandard", {
                icon: " eav-icon-cancel",
                tooltip: "SwitchMode.Standard",
                onclick: function () { switchModes("standard"); }
            });

            editor.addButton("modeadvanced", {
                icon: " eav-icon-pro",
                tooltip: "SwitchMode.Pro",
                onclick: function () { switchModes("advanced"); }
            });
            //#endregion

            //#region h1, h2, etc. buttons, inspired by http://blog.ionelmc.ro/2013/10/17/tinymce-formatting-toolbar-buttons/
            // note that the complex array is needede because auto-translate only happens if the string is identical
            [["pre", "Preformatted", "Preformatted"],
                ["p", "Paragraph", "Paragraph"],
                ["code", "Code", "Code"],
                ["h1", "Heading 1", "H1"],
                ["h2", "Heading 2", "H2"],
                ["h3", "Heading 3", "H3"],
                ["h4", "Heading 4", "Heading 4"],
                ["h5", "Heading 5", "Heading 5"],
                ["h6", "Heading 6", "Heading 6"]].forEach(function (tag) {
                    editor.addButton(tag[0], {
                        tooltip: tag[1],
                        text: tag[2],
                        onclick: function () { editor.execCommand("mceToggleFormat", false, tag[0]); },
                        onPostRender: function () {
                            var self = this,
                                setup = function () {
                                    editor.formatter.formatChanged(tag[0], function (state) {
                                        self.active(state);
                                    });
                                };
                            var x = editor.formatter ? setup() : editor.on("init", setup);
                        }
                    });
                });

            // group of buttons with an h3 to start and showing h4-6 + p
            editor.addButton("hgroup", angular.extend({}, editor.buttons.h3,
            {
                type: "splitbutton",
                menu: [
                    editor.buttons.h4,
                    editor.buttons.h5,
                    editor.buttons.h6,
                    editor.buttons.p
                ]
            }));
            //#endregion

            // #region inside content
            editor.addButton("addcontentblock", {
                icon: " eav-icon-content-block",
                classes: "btn-addcontentblock",
                tooltip: "ContentBlock.Add",
                onclick: function() {
                    var guid = Math.uuid().toLowerCase(); // requires the uuid-generator to be included
                    
                    vm.editor.insertContent("<hr sxc=\"sxc-content-block\" guid=\"" + guid + "\" />");
                }
            });
            // #endregion

            //#region image alignment / size buttons
            editor.addButton("alignimgleft", { icon: " eav-icon-align-left", tooltip: "Align left", cmd: "JustifyLeft", onPostRender: initOnPostRender("alignleft") });
            editor.addButton("alignimgcenter", { icon: " eav-icon-align-center", tooltip: "Align center", cmd: "justifycenter", onPostRender: initOnPostRender("aligncenter") });
            editor.addButton("alignimgright", { icon: " eav-icon-align-right", tooltip: "Align right", cmd: "justifyright", onPostRender: initOnPostRender("alignright") });

            var imgMenuArray = [];
            function makeImgFormatCall(size) { return function () { editor.formatter.apply("imgwidth" + size); }; }
            for (var is = 0; is < tinyMceConfig.imgSizes.length; is++) {
                var config = {
                    icon: " eav-icon-resize-horizontal",
                    tooltip: tinyMceConfig.imgSizes[is] + "%",
                    text: tinyMceConfig.imgSizes[is] + "%",
                    onclick: makeImgFormatCall(tinyMceConfig.imgSizes[is]),
                    onPostRender: initOnPostRender("imgwidth" + tinyMceConfig.imgSizes[is])
                };
                editor.addButton("imgresize" + tinyMceConfig.imgSizes[is], config);
                imgMenuArray.push(config);
            }

            editor.addButton("resizeimg100", {
                icon: " eav-icon-resize-horizontal", tooltip: "100%",
                onclick: function () { editor.formatter.apply("imgwidth100"); },
                onPostRender: initOnPostRender("imgwidth100")
            });

            // group of buttons to resize an image 100%, 50%, etc.
            editor.addButton("imgresponsive", angular.extend({}, editor.buttons.resizeimg100,
            { type: "splitbutton", menu: imgMenuArray }));
            //#endregion

            //#region my context toolbars for links, images and lists (ul/li)
            function makeTagDetector(tagWeNeedInTheTagPath) {
                return function tagDetector(currentElement) {
                    // check if we are in a tag within a specific tag
                    var selectorMatched = editor.dom.is(currentElement, tagWeNeedInTheTagPath) && editor.getBody().contains(currentElement);
                    return selectorMatched;
                };
            }

            editor.addContextToolbar(makeTagDetector("a"), "link unlink");
            editor.addContextToolbar(makeTagDetector("img"), "image | alignimgleft alignimgcenter alignimgright imgresponsive | removeformat | remove");
            editor.addContextToolbar(makeTagDetector("li,ol,ul"), "bullist numlist | outdent indent");
            //#endregion
        }

        return svc;
    }]);
angular.module("SxcEditTemplates", []).run(["$templateCache", function($templateCache) {$templateCache.put("adam/adam-hint.html","<div class=\"small pull-right\">\r\n    <span style=\"opacity: 0.5\">drop files here -</span>\r\n    <a href=\"http://2sxc.org/help?tag=adam\" target=\"_blank\" uib-tooltip=\"ADAM is the Automatic Digital Assets Manager - click to discover more\">\r\n        <i class=\"eav-icon-apple\"></i>\r\n        Adam\r\n    </a>\r\n    <span style=\"opacity: 0.5\"> is sponsored with\r\n    <i class=\"eav-icon-heart\"></i> by\r\n    <a tabindex=\"-1\" href=\"http://2sic.com/\" target=\"_blank\">\r\n        2sic.com\r\n    </a>\r\n    </span>\r\n</div>\r\n");
$templateCache.put("adam/browser.html","<div ng-if=\"vm.show\" ng-class=\"\'adam-scope-\' + (vm.adamModeConfig.usePortalRoot ? \'site\' : field)\">\r\n    <!-- info for dropping stuff here -->\r\n    <div class=\"dz-preview dropzone-adam\" ng-disabled=\"vm.disabled\" uib-tooltip=\"{{\'Edit.Fields.Hyperlink.Default.AdamUploadLabel\' | translate }}\" ng-click=\"vm.openUpload()\">\r\n        <div class=\"dz-image adam-browse-background-icon adam-browse-background\" xstyle=\"background-color: whitesmoke\">\r\n            <i class=\"eav-icon-up-circled2\"></i>\r\n            <div class=\"adam-short-label\">upload to<i ng-class=\"vm.adamModeConfig.usePortalRoot ? \'eav-icon-globe\' : \'eav-icon-apple\'\" style=\"font-size: larger\"></i></div>\r\n        </div>\r\n    </div>\r\n\r\n    <!-- add folder - not always shown -->\r\n    <div ng-show=\"vm.allowCreateFolder() || vm.debug.on\" class=\"dz-preview\" ng-disabled=\"vm.disabled\" ng-click=\"vm.addFolder()\">\r\n        <div class=\"dz-image adam-browse-background-icon adam-browse-background\">\r\n            <div class=\"\">\r\n                <i class=\"eav-icon-folder-empty\"></i>\r\n                <div class=\"adam-short-label\">new folder</div>\r\n            </div>\r\n        </div>\r\n        <div class=\"adam-background adam-browse-background-icon\">\r\n            <i class=\"eav-icon-plus\" style=\"font-size: 2em; top: 13px; position: relative;\"></i>\r\n        </div>\r\n        <div class=\"dz-details\" style=\"opacity: 1\">\r\n\r\n        </div>\r\n    </div>\r\n\r\n    <!-- browse up a folder - not always shown -->\r\n    <div ng-show=\"vm.showFolders || vm.debug.on\" class=\"dz-preview\" ng-disabled=\"vm.disabled\" ng-if=\"vm.folders.length > 0\" ng-click=\"vm.goUp()\">\r\n        <div class=\"dz-image  adam-browse-background-icon adam-browse-background\">\r\n            <i class=\"eav-icon-folder-empty\"></i>\r\n            <div class=\"adam-short-label\">up</div>\r\n        </div>\r\n        <div class=\"adam-background adam-browse-background-icon\">\r\n            <i class=\"eav-icon-level-up\" style=\"font-size: 2em; top: 13px; position: relative;\"></i>\r\n        </div>\r\n    </div>\r\n\r\n    <!-- folder list - not always shown -->\r\n    <div ng-show=\"vm.showFolders || vm.debug.on\" class=\"dz-preview\" ng-repeat=\"item in vm.items | filter: { IsFolder: true } | filter: { Name: \'!2sxc\' } | filter: { Name: \'!adam\' } | orderBy:\'Name\'\"\r\n         ng-click=\"vm.goIntoFolder(item)\">\r\n        <div class=\"dz-image adam-blur adam-browse-background-icon adam-browse-background\">\r\n            <i class=\"eav-icon-folder-empty\"></i>\r\n            <div class=\"short-label\">{{ item.Name }}</div>\r\n        </div>\r\n\r\n        <div class=\"dz-details file-type-{{item.Type}}\">\r\n            <span ng-click=\"vm.del(item)\" stop-event=\"click\" class=\"adam-delete-button\"><i class=\"eav-icon-cancel\"></i></span>\r\n            <span ng-click=\"vm.rename(item)\" stop-event=\"click\" class=\"adam-rename-button\"><i class=\"eav-icon-pencil\"></i></span>\r\n            <div class=\"adam-full-name-area\">\r\n                <div class=\"adam-full-name\">{{ item.Name }}</div>\r\n            </div>\r\n        </div>\r\n\r\n        <span class=\"adam-tag\" ng-class=\"{\'metadata-exists\': item.MetadataId > 0}\"\r\n              ng-click=\"vm.editMetadata(item)\"\r\n              ng-if=\"vm.getMetadataType(item)\"\r\n              stop-event=\"click\"\r\n              uib-tooltip=\"{{vm.getMetadataType(item)}}:{{item.MetadataId}}\">\r\n            <i class=\"eav-icon-tag\" style=\"font-size: larger\"></i>\r\n        </span>\r\n    </div>\r\n\r\n\r\n    <!-- files -->\r\n    <div class=\"dz-preview\" ng-class=\"{ \'dz-success\': value.Value.toLowerCase() == \'file:\' + item.Id }\" ng-repeat=\"item in (vm.items | filter: vm.fileEndingFilter | filter: { IsFolder: false }) | filter: (vm.showImagesOnly ? {Type: \'image\'} : {})  | orderBy:\'Name\'\" ng-click=\"vm.select(item)\" ng-disabled=\"vm.disabled || !vm.enableSelect\">\r\n        <div ng-if=\"item.Type !== \'image\'\" class=\"dz-image adam-blur  adam-browse-background-icon adam-browse-background\">\r\n            <i ng-class=\"vm.icon(item)\"></i>\r\n            <div class=\"adam-short-label\">{{ item.Name }}</div>\r\n        </div>\r\n        <div ng-if=\"item.Type === \'image\'\" class=\"dz-image\">\r\n            <img data-dz-thumbnail=\"\" alt=\"{{ item.Id + \':\' + item.Name\r\n}}\" ng-src=\"{{ item.fullPath + \'?w=120&h=120&mode=crop\' }}\">\r\n        </div>\r\n\r\n\r\n\r\n        <div class=\"dz-details file-type-{{item.Type}}\">\r\n            <span ng-click=\"vm.del(item)\" stop-event=\"click\" class=\"adam-delete-button\"><i class=\"eav-icon-cancel\"></i></span>\r\n            <span ng-click=\"vm.rename(item)\" stop-event=\"click\" class=\"adam-rename-button\"><i class=\"eav-icon-pencil\"></i></span>\r\n            <div class=\"adam-full-name-area\">\r\n                <div class=\"adam-full-name\">{{ item.Name }}</div>\r\n            </div>\r\n            <div class=\"dz-filename adam-short-label\">\r\n                <span>#{{ item.Id }} - {{ (item.Size / 1024).toFixed(0) }} kb</span>\r\n            </div>\r\n            <a class=\"adam-link-button\" target=\"_blank\" ng-href=\"{{ item.fullPath }}\">\r\n                <i class=\"eav-icon-link-ext\" style=\"font-size: larger\"></i>\r\n            </a>\r\n        </div>\r\n\r\n        <span class=\"adam-tag\" ng-class=\"{\'metadata-exists\': item.MetadataId > 0}\"\r\n              ng-click=\"vm.editMetadata(item)\"\r\n              ng-if=\"vm.getMetadataType(item)\"\r\n              stop-event=\"click\"\r\n              uib-tooltip=\"{{vm.getMetadataType(item)}}:{{item.MetadataId}}\">\r\n            <i class=\"eav-icon-tag\" style=\"font-size: larger\"></i>\r\n        </span>\r\n\r\n\r\n        <div class=\"dz-success-mark\">\r\n            <svg width=\"54px\" height=\"54px\" viewBox=\"0 0 54 54\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:sketch=\"http://www.bohemiancoding.com/sketch/ns\">\r\n                <title>Check</title>\r\n                <defs></defs>\r\n                <g id=\"Page-1\" stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\" sketch:type=\"MSPage\">\r\n                    <path d=\"M23.5,31.8431458 L17.5852419,25.9283877 C16.0248253,24.3679711 13.4910294,24.366835 11.9289322,25.9289322 C10.3700136,27.4878508 10.3665912,30.0234455 11.9283877,31.5852419 L20.4147581,40.0716123 C20.5133999,40.1702541 20.6159315,40.2626649 20.7218615,40.3488435 C22.2835669,41.8725651 24.794234,41.8626202 26.3461564,40.3106978 L43.3106978,23.3461564 C44.8771021,21.7797521 44.8758057,19.2483887 43.3137085,17.6862915 C41.7547899,16.1273729 39.2176035,16.1255422 37.6538436,17.6893022 L23.5,31.8431458 Z M27,53 C41.3594035,53 53,41.3594035 53,27 C53,12.6405965 41.3594035,1 27,1 C12.6405965,1 1,12.6405965 1,27 C1,41.3594035 12.6405965,53 27,53 Z\" id=\"Oval-2\" stroke-opacity=\"0.198794158\" stroke=\"#747474\" fill-opacity=\"0.816519475\" fill=\"#FFFFFF\" sketch:type=\"MSShapeGroup\"></path>\r\n                </g>\r\n            </svg>\r\n        </div>\r\n    </div>\r\n\r\n</div>");
$templateCache.put("adam/dropzone-upload-preview.html","<div ng-show=\"uploading\">\r\n    <div class=\"dropzone-previews\">\r\n    </div>\r\n    <span class=\"invisible-clickable\" data-note=\"just a fake, invisible area for dropzone\"></span>\r\n</div>");
$templateCache.put("fields/dnn-bridge/hyperlink-default-pagepicker.html","<div>\r\n	<div class=\"modal-header\">\r\n		<h3 class=\"modal-title\" translate=\"Edit.Fields.Hyperlink.PagePicker.Title\"></h3>\r\n	</div>\r\n	<div class=\"modal-body\" style=\"height:370px; width:600px\">\r\n		<iframe style=\"width:100%; height: 350px; border: 0;\" web-forms-bridge=\"bridge\" bridge-type=\"pagepicker\" bridge-sync-height=\"false\"></iframe>\r\n	</div>\r\n	<div class=\"modal-footer\"></div>\r\n</div>");
$templateCache.put("fields/hyperlink/hyperlink-default.html","<div class=\"dropzone\">\r\n    <div class=\"clearfix\">\r\n        <div ng-if=\"value.Value && vm.isImage()\"\r\n             class=\"thumbnail-before-input\"\r\n             ng-style=\"{ \'background-image\': \'url(\' + vm.thumbnailUrl(1, true) + \')\' }\"\r\n             ng-mouseover=\"vm.showPreview = true\"\r\n             ng-mouseleave=\"vm.showPreview = false\">\r\n        </div>\r\n\r\n        <div ng-if=\"value.Value && !vm.isImage()\"\r\n           class=\"thumbnail-before-input icon-before-input\">\r\n            <a href=\"{{vm.testLink}}\"\r\n               target=\"_blank\" tabindex=\"-1\"\r\n               tooltip-html=\"{{vm.tooltipUrl(vm.testLink)}}\"\r\n               tooltip-placement=\"right\"\r\n               ng-class=\"vm.icon()\">\r\n            </a>            \r\n        </div>\r\n        <div ng-if=\"!value.Value\"\r\n             class=\"thumbnail-before-input empty-placeholder\">\r\n        </div>\r\n        <div class=\"after-preview\">\r\n            <div class=\"input-group\" uib-dropdown>\r\n                <input type=\"text\" class=\"form-control input-lg\" ng-model=\"value.Value\" uib-tooltip=\"{{\'Edit.Fields.Hyperlink.Default.Tooltip1\' | translate }}\r\n{{\'Edit.Fields.Hyperlink.Default.Tooltip2\' | translate }}\r\nADAM - sponsored with ♥ by 2sic.com\">\r\n                <span class=\"input-group-btn\" style=\"vertical-align: top;\">\r\n                    <div style=\"width: 6px;\"></div>\r\n                    <button ng-if=\"to.settings[\'merged\'].Buttons.indexOf(\'adam\') > -1\" type=\"button\" class=\"btn btn-default icon-field-button\" ng-disabled=\"to.disabled\" uib-tooltip=\"{{\'Edit.Fields.Hyperlink.Default.AdamUploadLabel\' | translate }}\" ng-click=\"vm.toggleAdam()\">\r\n                        <i class=\"eav-icon-apple\"></i>\r\n                    </button>\r\n                    <button ng-if=\"to.settings[\'merged\'].Buttons.indexOf(\'page\') > -1\" type=\"button\" class=\"btn btn-default icon-field-button\" ng-disabled=\"to.disabled\" uib-tooltip=\"{{\'Edit.Fields.Hyperlink.Default.PageLabel\' | translate }}\" ng-click=\"vm.openPageDialog()\">\r\n                        <i class=\"eav-icon-sitemap\"></i>\r\n                    </button>\r\n                    <button ng-if=\"to.settings[\'merged\'].Buttons.indexOf(\'more\') > -1\" tabindex=\"-1\" type=\"button\" class=\"btn btn-default uib-dropdown-toggle icon-field-button\" uib-dropdown-toggle ng-disabled=\"to.disabled\">\r\n                        <i class=\"eav-icon-options\"></i>\r\n                    </button>\r\n                </span>\r\n                <ul class=\"dropdown-menu pull-right\" uib-dropdown-menu role=\"menu\">\r\n                    <li role=\"menuitem\" ng-if=\"to.settings[\'merged\'].ShowAdam\"><a class=\"dropzone-adam\" ng-click=\"vm.toggleAdam(false)\" href=\"javascript:void(0);\"><i class=\"eav-icon-apple\"></i> <span translate=\"Edit.Fields.Hyperlink.Default.MenuAdam\"></span></a></li>\r\n                    <li role=\"menuitem\" ng-if=\"to.settings[\'merged\'].ShowPagePicker\"><a ng-click=\"vm.openPageDialog()\" href=\"javascript:void(0)\"><i class=\"eav-icon-sitemap\" xicon=\"home\"></i> <span translate=\"Edit.Fields.Hyperlink.Default.MenuPage\"></span></a></li>\r\n                    <li role=\"menuitem\" ng-if=\"to.settings[\'merged\'].ShowImageManager\"><a ng-click=\"vm.toggleAdam(true, true)\" href=\"javascript:void(0)\"><i class=\"eav-icon-file-image\" xicon=\"picture\"></i> <span translate=\"Edit.Fields.Hyperlink.Default.MenuImage\"></span></a></li>\r\n                    <li role=\"menuitem\" ng-if=\"to.settings[\'merged\'].ShowFileManager\"><a ng-click=\"vm.toggleAdam(true, false)\" href=\"javascript:void(0)\"><i class=\"eav-icon-file\" xicon=\"file\"></i> <span translate=\"Edit.Fields.Hyperlink.Default.MenuDocs\"></span></a></li>\r\n                </ul>\r\n            </div>\r\n            <div ng-if=\"vm.showPreview\" style=\"position: relative\">\r\n                <div style=\"position: absolute; z-index: 100; background: white; top: 10px; text-align: center; left: 0; right: 0;\">\r\n                    <img ng-src=\"{{vm.thumbnailUrl(2)}}\" />\r\n                </div>\r\n            </div>\r\n\r\n            <adam-hint class=\"field-hints\"></adam-hint>\r\n            <div ng-if=\"value.Value\" class=\"field-hints\">\r\n                <a href=\"{{vm.testLink}}\" target=\"_blank\" tabindex=\"-1\" tooltip-html=\"{{vm.tooltipUrl(vm.testLink)}}\">\r\n                    <span>&nbsp;... {{vm.testLink.substr(vm.testLink.lastIndexOf(\"/\"), 100)}}</span>\r\n                </a>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n    <div>\r\n        <!-- The ADAM file browser, requires the uploader wrapped around it -->\r\n        <adam-browser content-type-name=\"to.header.ContentTypeName\"\r\n                      entity-guid=\"to.header.Guid\"\r\n                      field-name=\"options.key\"\r\n                      auto-load=\"false\"\r\n                      folder-depth=\"0\"\r\n                      sub-folder=\"\"\r\n                      update-callback=\"vm.setValue\"\r\n                      register-self=\"vm.registerAdam\"\r\n                      adam-mode-config=\"vm.adamModeConfig\"\r\n                      ng-disabled=\"to.disabled\"\r\n                      file-filter=\"to.settings[\'merged\'].FileFilter\"></adam-browser>\r\n\r\n        <!-- the preview of the uploader -->\r\n        <dropzone-upload-preview></dropzone-upload-preview>\r\n\r\n    </div>\r\n</div>");
$templateCache.put("fields/hyperlink/hyperlink-library.html","<div>\r\n    <div class=\"dropzone\">\r\n        <!-- The ADAM file browser, requires the uploader wrapped around it -->\r\n        <adam-browser\r\n            content-type-name=\"to.header.ContentTypeName\"\r\n            entity-guid=\"to.header.Guid\"\r\n            field-name=\"options.key\"\r\n            auto-load=\"true\"\r\n            sub-folder=\"\"\r\n            folder-depth=\"to.settings.merged.FolderDepth\"\r\n            metadata-content-types=\"to.settings.merged.MetadataContentTypes\"\r\n            allow-assets-in-root=\"to.settings.merged.allowAssetsInRoot\"\r\n            enable-select=\"false\"\r\n            update-callback=\"vm.setValue\"\r\n            register-self=\"vm.registerAdam\"\r\n            adam-mode-config=\"vm.adamModeConfig\">\r\n        </adam-browser>\r\n\r\n\r\n        <!-- the preview of the uploader -->\r\n        <dropzone-upload-preview></dropzone-upload-preview>\r\n\r\n    </div>\r\n</div>");
$templateCache.put("fields/string/string-font-icon-picker.html","<div>\r\n    <div uib-dropdown uib-keyboard-nav auto-close=\"outsideClick\" is-open=\"vm.selectorIsOpen\">\r\n        <div class=\"thumbnail-before-input icon-preview\">\r\n            <button type=\"button\" class=\"\" uib-tooltip=\"{{value.Value}}\" uib-dropdown-toggle>\r\n                <i class=\"{{vm.previewPrefix}} {{value.Value}}\" ng-show=\"value.Value\"></i>\r\n                <span ng-show=\"!value.Value\">&nbsp;&nbsp;</span>\r\n            </button>\r\n        </div>\r\n        <div class=\"input-group\" uib-dropdown-toggle>\r\n            <input type=\"text\" class=\"form-control input-lg\" ng-model=\"value.Value\" ng-disabled=\"false\" >\r\n        </div>\r\n        <!-- the search ui -->\r\n        <ul class=\"dropdown-menu icons-menu-columns\" role=\"menu\" uib-dropdown-menu>\r\n            <li class=\"input-group\">\r\n                <span class=\"input-group-addon btn-default btn\" ng-click=\"vm.selectorIsOpen = false; value.Value = \'\'\">\r\n                    <i class=\"eav-icon-cancel\"></i>\r\n                </span>\r\n                <input type=\"search\" ng-model=\"vm.iconFilter\" class=\"form-control input-lg\" placeholder=\"search...\" />\r\n            </li>\r\n\r\n            <li ng-repeat=\"icn in vm.icons\" role=\"menuitem\"\r\n                ng-show=\"icn.class.indexOf(vm.iconFilter) !== -1\">\r\n                <a ng-click=\"vm.setIcon(icn.class)\" xng-click=\"value.Value = icn.class; vm.selectorIsOpen = false;\">\r\n                    <i class=\"{{vm.previewPrefix}} {{icn.class}}\"></i> <span uib-tooltip=\"{{icn.class}}\">...{{icn.class.substring(vm.prefix.length-1,25)}}</span>\r\n                </a>\r\n            </li>\r\n        </ul>\r\n    </div>\r\n    <div ng-if=\"vm.debug.on\">Infos: found {{vm.icons.length}} items for prefix \"{{vm.prefix}}\" and will use \"{{vm.previewPrefix}}\" as a preview class. Selected is \"{{value.Value}}\" and files are: {{vm.files}}</div>\r\n</div> ");
$templateCache.put("fields/string/string-template-picker.html","<div>\r\n    <div class=\"input-group\">\r\n\r\n        <select class=\"form-control input-material material\"\r\n                ng-model=\"value.Value\"\r\n                ng-disabled=\"!readyToUse()\">\r\n            <option value=\"\">(no file selected)</option>\r\n            <option ng-repeat=\"item in templates | isValidFile:file.ext\"\r\n                    ng-selected=\"{{item == value.Value}}\"\r\n                    value=\"{{item}}\">\r\n                {{item}}\r\n            </option>\r\n        </select>\r\n        <span class=\"input-group-btn\" style=\"vertical-align: top;\">\r\n            <button class=\"btn btn-default icon-field-button\" type=\"button\" ng-click=\"add()\" ng-disabled=\"!readyToUse()\">\r\n                <span class=\"eav-icon-plus\"></span>\r\n            </button>\r\n        </span>\r\n    </div>\r\n</div>");
$templateCache.put("fields/string/string-wysiwyg-adv.html","<div>\r\n    this would be an advanced, configurable WYSIWYG. It does not exist yet :). \r\n</div>");
$templateCache.put("fields/string/string-wysiwyg-dnn.html","<iframe style=\"width:100%; border: 0;\" web-forms-bridge=\"vm.bridge\" bridge-type=\"wysiwyg\" bridge-sync-height=\"true\"></iframe>");
$templateCache.put("fields/string/string-wysiwyg-tinymce.html","<div>\r\n    <div class=\"dropzone\">\r\n        <div> <!-- needed because after a refresh, the editor can\'t be placed at the same location -->\r\n            <div ui-tinymce=\"tinymceOptions\" ng-model=\"value.Value\" class=\"field-string-wysiwyg-mce-box\"></div>\r\n        </div>\r\n\r\n        <!-- the ADAM file browser requires the uploader wrapped around it -->\r\n        <adam-browser content-type-name=\"to.header.ContentTypeName\"\r\n                      entity-guid=\"to.header.Guid\"\r\n                      field-name=\"options.key\"\r\n                      auto-load=\"false\"\r\n                      folder-depth=\"0\"\r\n                      sub-folder=\"\"\r\n                      update-callback=\"vm.setValue\"\r\n                      register-self=\"vm.registerAdam\"\r\n                      show-images-only=\"vm.adamModeImage\"\r\n                      ng-disabled=\"to.disabled\" adam-mode-config=\"vm.adamModeConfig\"></adam-browser>\r\n\r\n        <!-- dummy item to set focus on in script-->\r\n        <span id=\"dummyfocus\" tabindex=\"-1\"></span>\r\n\r\n        <!-- the preview of the uploader -->\r\n        <dropzone-upload-preview></dropzone-upload-preview>\r\n        <adam-hint class=\"field-hints\"></adam-hint>\r\n\r\n    </div>\r\n</div>");}]);