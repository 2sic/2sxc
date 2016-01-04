
(function() {
    "use strict";

    angular.module("sxcFieldTemplates")
        .config(function(formlyConfigProvider) {

            formlyConfigProvider.setType({
                name: "hyperlink-default",
                templateUrl: "fields/hyperlink/hyperlink-default.html",
                wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
                controller: "FieldTemplate-HyperlinkCtrl as vm"
            });
        })
        .controller("FieldTemplate-HyperlinkCtrl", function ($modal, $scope, $http, sxc, adamSvc, debugState, dnnBridgeSvc) {

            var vm = this;
            vm.debug = debugState;
            vm.testLink = "";
            vm.checkImgRegEx = /(?:([^:\/?#]+):)?(?:\/\/([^\/?#]*))?([^?#]*\.(?:jpg|jpeg|gif|png))(?:\?([^#]*))?(?:#(.*))?/i;

            vm.isImage = function() {
                var value = $scope.value;
                return vm.checkImgRegEx.test(vm.testLink);
            };
            vm.thumbnailUrl = function thumbnailUrl(size) {
                if (size === 1)
                    return vm.testLink + "?w=46&h=46&mode=crop";
                if (size === 2)
                    return vm.testLink + "?w=500&h=400&mode=max";
            };

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

            //#region dnn-bridge dialogs

            // the callback when something was selected
            vm.processResultOfDnnBridge = function(value, type) {
                $scope.$apply(function() {
                    if (!value) return;
                    
                    // Convert file path to file ID if type file is specified
                    $scope.value.Value = value;
                    if (type === "file") {
                        dnnBridgeSvc.convertPathToId(value, type)
                            .then(function(result) {
                                if (result.data)
                                    $scope.value.Value = "file:" + result.data.FileId;
                            });
                    }
                });
            };

            // open the dialog
            vm.openDialog = function (type) {
                dnnBridgeSvc.open(
                    type,
                    $scope.value.Value,
                    {
                        Paths: $scope.to.settings.merged ? $scope.to.settings.merged.Paths : "",
                        FileFilter: $scope.to.settings.merged ? $scope.to.settings.merged.FileFilter : ""
                    },
                    vm.processResultOfDnnBridge);
            };

            //#region new adam: callbacks only
            vm.registerAdam = function(adam) {
                vm.adam = adam;
            };
            vm.setValue = function(fileItem) {
                $scope.value.Value = "File:" + fileItem.Id;
            };
            $scope.afterUpload = vm.setValue;   // binding for dropzone
            vm.toggleAdam = function toggle() {
                vm.adam.toggle();
            };

            //#endregion


        });


})();