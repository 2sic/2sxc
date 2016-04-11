(function() {
    "use strict";

    angular.module("sxcFieldTemplates")
        .config(function(formlyConfigProvider) {

            formlyConfigProvider.setType({
                name: "entity-content-blocks",
                templateUrl: "fields/entity/entity-default.html",
                wrapper: ["eavLabel", "bootstrapHasError", "collapsible"],
                controller: "FieldTemplate-EntityContentBlockCtrl"
            });
        })
        .controller("FieldTemplate-EntityContentBlockCtrl", function($controller, $scope, $http, $filter, $translate, $modal, appId, eavAdminDialogs, eavDefaultValueService) {
            // use "inherited" controller just like described in http://stackoverflow.com/questions/18461263/can-an-angularjs-controller-inherit-from-another-controller-in-the-same-module
            $controller('FieldTemplate-EntityCtrl', { $scope: $scope });

            $scope.to.settings.merged.EnableRemove = true;
            $scope.to.settings.merged.AllowMultiValue = true;
            $scope.to.enableCollapseField = true;   // ui option to allow collapsing
            $scope.to.collapseField = true;   // ui option to allow collapsing
        });
})();