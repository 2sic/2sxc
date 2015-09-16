(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("MainSxcApp", [
        "EavConfiguration",     // config
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "EavServices",          // multi-language stuff
        "SxcFilters",           // for inline unsafe urls
        "ContentTypesApp",
        "PipelineManagement",
        "TemplatesApp",
        "ImportExportApp",
        "AppSettingsApp",
        "SystemSettingsApp"
    ])
        .config(function ($translatePartialLoaderProvider) {
                // ensure the language pack is loaded
                 $translatePartialLoaderProvider.addPart("sxc-admin");
            })

        .controller("Main", MainController)
        ;

    function MainController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;
        vm.view = "start";

        vm.gettingStartedUrl = (window.mainConfig) ? window.mainConfig.gettingStartedUrl : "http://gettingstarted.2sxc.org";

        //vm.gettingStartedUrl = function() {
        //    return (window.mainConfig) ?
        //        window.mainConfig.gettingStartedUrl
        //        : "http://gettingstarted.2sxc.org";
        //};
        // var svc = templatesSvc(appId);

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());