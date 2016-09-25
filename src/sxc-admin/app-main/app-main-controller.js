(function() { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("MainSxcApp", [
            "EavConfiguration", // config
            "SxcTemplates", // inline templates
            "EavAdminUi", // dialog (modal) controller
            "EavServices", // multi-language stuff
            "SxcFilters", // for inline unsafe urls
            "ContentTypesApp",
            "PipelineManagement",
            "TemplatesApp",
            "ImportExport",
            "AppSettingsApp",
            "SystemSettingsApp",
            "WebApiApp",
            "SxcServices"
        ])
        .config(function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        })
        .controller("AppMain", MainController)

        //.factory("appDialogConfigSvc", function(appId, $http) {
        //    var svc = {};

        //    // this will retrieve an advanced getting-started url to use in an the iframe
        //    svc.getDialogSettings = function gettingStartedUrl() {
        //        return $http.get("app/system/dialogsettings", { params: { appId: appId } });
        //    };
        //    return svc;
        //})
    ;

    function MainController(eavAdminDialogs, eavConfig, appId, debugState, appDialogConfigSvc, $uibModalInstance) {
        var vm = this;
        vm.debug = debugState;
        vm.view = "start";

        appDialogConfigSvc.getDialogSettings().then(function (result) {
            vm.config = result.data;
        });

        vm.close = function () {
            $uibModalInstance.dismiss("cancel");
        };
    }

} ());