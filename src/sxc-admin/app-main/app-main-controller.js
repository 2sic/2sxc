(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("MainSxcApp", [
            "EavConfiguration", // config
            "SxcTemplates", // inline templates
            "EavAdminUi", // dialog (modal) controller
            "EavServices", // multi-language stuff
            "SxcFilters", // for inline unsafe urls
            "ContentTypesApp",
            "PipelineManagement",
            "TemplatesApp",
            "ImportExportApp",
            "AppSettingsApp",
            "SystemSettingsApp"
        ])
        .config(function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        })
        .controller("AppMain", MainController)
        .factory("appDialogConfigSvc", function(appId, $http) {
            var svc = {};
                alert('in svc' + appId);
            // this will retrieve an advanced getting-started url to use in an the iframe
            svc.getDialogSettings = function gettingStartedUrl() {
                return $http.get("app/system/dialogsettings", { params: { appId: appId } });
            };
            return svc;
        })
        ;

    function MainController(eavAdminDialogs, eavConfig, appId, appDialogConfigSvc, $modalInstance) {
        var vm = this;
        vm.view = "start";
        alert('in app' + appId);
        appDialogConfigSvc.getDialogSettings().then(function(result) {
            vm.config = result.data;//.GettingStartedUrl;
        });

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());