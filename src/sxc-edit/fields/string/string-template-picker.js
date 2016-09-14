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
    .controller("FieldTemplate-String-TemplatePicker", function($scope, appAssetsSvc, appId, fieldMask) { //, $http, $filter, $translate, $modal, eavAdminDialogs, eavDefaultValueService) {

        function activate() {
            // ensure settings are merged
            if (!$scope.to.settings.merged)
                $scope.to.settings.merged = {};

            $scope.setFileConfig("Token"); // use token setting as default, till the UI tells us otherwise

            var scriptType = fieldMask("[Type]", $scope, $scope.onTypeChange);
            var location = fieldMask("[Location]", $scope, $scope.onLocationChange);

            // create initial list for binding
            $scope.templates = [];

            var svc = appAssetsSvc(appId);
            $scope.templates = svc.liveList();
        }

        $scope.setFileConfig = function(type) {
            switch (type) {
            case "Token":
                $scope.fileExt = "html";
                $scope.filePrefix = "";
                break;
            case "C# Razor":
                $scope.fileExt = "cshtml";
                $scope.filePrefix = "_";
                break;
            }
        };

        $scope.onTypeChange = function(currentType) {
            console.log("type changed");
            $scope.setFileConfig(currentType);
        };

        $scope.onLocationChange = function() {
            console.log("location changed");
        };

        activate();

    })

    // Setup the filter
    .filter("isValidFile", function() {

        // Create the return function
        // set the required parameter name to **number**
        return function(paths, extension) {
            var ext = "." + extension;
            var out = [];
            angular.forEach(paths, function(path) {
                if (path.slice(path.length - ext.length) === ext)
                    out.push(path);
            });
            return out;
        };
    });
