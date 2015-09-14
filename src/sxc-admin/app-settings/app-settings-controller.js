(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("AppSettingsApp", [
        "EavConfiguration",     // config
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "Eavi18n"              // multi-language stuff
    ])
        .controller("AppSettings", AppSettingsController)
        ;

    function AppSettingsController(eavAdminDialogs, eavConfig, appId) {
        var vm = this;

        vm.editSettings = function() {
            //todo
        };

        vm.confSettings = function() {
            // todo
        };

        vm.editResources = function() {
            //todo
        };

        vm.confResources = function() {
            //todo
        };
    }

} ());