/* 
 * Field: String - Dropdown
 */

angular.module("sxcFieldTemplates")
    .config(function(formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-template-picker",
            templateUrl: "fields/string/string-template-picker.html",
            wrapper: defaultFieldWrappers,
            controller: "FieldTemplate-String-TemplatePicker"
        });

    })
    .controller("FieldTemplate-String-TemplatePicker", function ($scope, appAssetsSvc, appId) { //, $http, $filter, $translate, $modal, eavAdminDialogs, eavDefaultValueService) {
        // ensure settings are merged
        if (!$scope.to.settings.merged)
            $scope.to.settings.merged = {};

        // create initial list for binding
        $scope.templates = [];

        var svc = appAssetsSvc(appId);
        $scope.templates = svc.liveList();

    });