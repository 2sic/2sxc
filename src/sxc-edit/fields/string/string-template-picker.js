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
            return $scope.typeWatcher.current && $scope.locWatcher.current; // check if these have real values inside
        };

        $scope.setFileConfig = function(type) {
            switch (type) {
            case "Token":
                $scope.fileExt = ".html";
                $scope.filePrefix = "";
                break;
            case "C# Razor":
                $scope.fileExt = ".cshtml";
                $scope.filePrefix = "_";
                break;
            }
        };

        //$scope.onTypeChange = function(currentType) {
        //    $scope.setFileConfig(currentType);
        //};

        $scope.onLocationChange = function(loc) {
            $scope.svcCurrent = (loc === "Host File System") 
                ? $scope.svcGlobal
                : $scope.svcApp;

            $scope.templates = $scope.svcCurrent.liveList();
        };

        $scope.add = function() {
            var fileName = prompt("enter new file name"); // todo: i18n

            if (!fileName)
                return;

            // 1. check for folders
            var path = "";
            fileName = fileName.replace("/", "\\");
            var foundSlash = fileName.lastIndexOf("\\");
            if (foundSlash > -1) {
                path = fileName.substring(0, foundSlash);
                fileName = fileName.substring(foundSlash + 1);
            }

            // 2. check if extension already provided, otherwise or if not perfect, just attach default
            if (fileName.indexOf($scope.fileExt) !== fileName.length - $scope.fileExt)
                fileName += $scope.fileExt;

            // todo: check if cshtmls have a "_" in the file name (not folder, must be the file name part)
            if ($scope.fileExt === ".cshtml" && fileName[0] !== "_")
                fileName = "_" + fileName;

            var result = path + fileName;
            console.log(result);
            // todo: tell service to create
            $scope.svcCurrent.create(result, "test content");
        };

        activate();

    })

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
