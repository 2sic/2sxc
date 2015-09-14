(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("MainSxcApp", [
        "EavConfiguration",     // config
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "Eavi18n",              // multi-language stuff
        "SxcFilters",           // for inline unsafe urls
        "ContentTypesApp",
        "TemplatesApp"
    ])
        .config(function($translatePartialLoaderProvider) {
                 $translatePartialLoaderProvider.addPart("sxc-admin");
            })

        .controller("Main", MainController)
        ;

    function MainController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;
        vm.view = "start";
        alert(window.mainConfig.gettingStartedUrl);
        vm.gettingStartedUrl = (window.mainConfig) ? window.mainConfig.gettingStartedUrl : "http://gettingstarted.2sxc.org";

        // var svc = templatesSvc(appId);

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());