(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExportApp", [
        "EavConfiguration",     // config
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "Eavi18n",              // multi-language stuff
        "ImportExportServices"
        //"SxcFilters",           // for inline unsafe urls
    ])
        .controller("ImportExportIntro", IntroController)
        .controller("Import", ImportController)
        .controller("Export", ExportController)
        ;

    function IntroController(eavAdminDialogs, eavConfig, appId) {
        var vm = this;

        vm.import = function() {
            // probably afterwards
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/import.html",
                "Import as vm",
                "lg",
                resolve);
        };

        vm.export = function() {
            // probably afterwards
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/export.html",
                "Export as vm",
                "lg",
                resolve);
        };
    }

    function ImportController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

    function ExportController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
} ());