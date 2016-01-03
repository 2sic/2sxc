
(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

	angular.module("sxcFieldTemplates")

    .config(function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: "hyperlink-default",
			templateUrl: "fieldtemplates/templates/hyperlink-default.html",
			wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
			controller: "FieldTemplate-HyperlinkCtrl as vm"
		});

		formlyConfigProvider.setType({
		    name: "hyperlink-library",
		    templateUrl: "fieldtemplates/templates/hyperlink-default.html",
		    wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
		    controller: "FieldTemplate-HyperlinkCtrl as vm"
		});
        
	})

	.controller("FieldTemplate-HyperlinkCtrl", function ($modal, $scope, $http, sxc, adamSvc, debugState) {

	    var vm = this;
	    vm.debug = debugState;
		vm.modalInstance = null;
		vm.testLink = "";
		vm.checkImgRegEx = /(?:([^:\/?#]+):)?(?:\/\/([^\/?#]*))?([^?#]*\.(?:jpg|jpeg|gif|png))(?:\?([^#]*))?(?:#(.*))?/i;
		vm.asLibrary = $scope.to.type === "hyperlink-library";
	    vm.enableFolders = vm.asLibrary;

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
				controller: function($scope, bridge) {
					$scope.bridge = bridge;
				},
				windowClass: "sxc-dialog-filemanager"
			});
		};

		vm.getExistingFiles = function () {
		};

	    //#region new adam: callbacks only
	    vm.setValue = function(url) {
	        $scope.value.Value = url;
	    };
        //#endregion

	    //#region adam
		//var header = $scope.to.header;
		//var field = $scope.options.key;
		//var entityGuid = header.Guid;

        //    $scope.adam = vm.adam = {
        //        show: false,
        //        subFolder: ""
	    //    };
        //    vm.adam.svc = adamSvc(header.ContentTypeName, entityGuid, field, "");
        //    vm.adam.refresh = vm.adam.svc.liveListReload;

	    //    vm.adam.get = function() {
	    //        vm.items = vm.adam.svc.liveList();
	    //        vm.adam.folders = vm.adam.svc.folders;
	    //    };

	    //    vm.adam.toggle = function toggle() {
	    //        vm.adam.show = !vm.adam.show;
	    //        vm.adam.get();
	    //    };
	    //    vm.adam.select = function (fileItem) {
	    //        if (!fileItem.IsFolder)
	    //            $scope.value.Value = "File:" + fileItem.Id;
	    //        else 
	    //            vm.adam.goIntoFolder(fileItem);
	    //    };
	    //    vm.adam.queueComplete = function qC() {
	    //        vm.adam.refresh();
	    //    };

	    //    vm.adam.addFolder = function() {
	    //        var folderName = window.prompt("Folder Name?");
	    //        vm.adam.svc.addFolder(folderName)
	    //            .then(vm.adam.refresh);
	    //    };

	    //    vm.adam.del = function del(item) {
	    //        var ok = window.confirm("delete ok?");
	    //        if (ok)
	    //            vm.adam.svc.delete(item);
	    //    };

	    //    vm.adam.goIntoFolder = function(folder) {
	    //        var subFolder = vm.adam.svc.goIntoFolder(folder);
	    //        vm.adam.subFolder = subFolder;
	    //    };
	    //    vm.adam.goUp = function () {
	    //        vm.adam.subFolder = vm.adam.svc.goUp();
	    //    };
	        //#endregion
	    });


})();