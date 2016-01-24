
(function() {
    "use strict";

    angular.module("sxcFieldTemplates")
        .config(function(formlyConfigProvider) {

            formlyConfigProvider.setType({
                name: "hyperlink-library",
                templateUrl: "fields/hyperlink/hyperlink-library.html",
                wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
                controller: "FieldTemplate-Library as vm"
            });

        })
        .controller("FieldTemplate-Library", function($modal, $scope, $http, sxc, adamSvc, debugState) {

            var vm = this;
            vm.debug = debugState;
            vm.modalInstance = null;
            vm.testLink = "";

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


        });


})();