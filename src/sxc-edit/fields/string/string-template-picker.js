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
    .controller("FieldTemplate-String-TemplatePicker", function($scope, appAssetsSvc, appId, fieldMask) { 

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
