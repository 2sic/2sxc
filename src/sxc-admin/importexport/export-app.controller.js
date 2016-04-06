(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExportApp")
        .controller("ExportApp", ExportController)
        ;

    function ExportController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
} ());