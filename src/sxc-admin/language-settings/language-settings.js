(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("SystemSettingsApp", [
        "EavConfiguration",     // 
        "EavServices",
        "SxcServices",
        "SxcTemplates",         // inline templates
//        "EavAdminUi",           // dialog (modal) controller
    ])

        .controller("LanguageSettings", LanguagesSettingsController)
        ;

    /*@ngInject*/
    function LanguagesSettingsController(languagesSvc, eavConfig, appId) {
        var vm = this;
        var svc = languagesSvc();
        vm.items = svc.liveList();

        // vm.refresh = 
        vm.ready = function ready() {
            return vm.items.length > 0;
        };

        vm.toggle = svc.toggle;

        vm.save = svc.save;
    }

} ());