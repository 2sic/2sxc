(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExportApp", [
        "EavConfiguration",     // config
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "EavServices",              // multi-language stuff
        "SxcServices"
        //"SxcFilters",           // for inline unsafe urls
    ])
        .controller("ImportExportIntro", IntroController)
        .controller("Import", ImportController)
        .controller("Export", ExportController)
        ;

    function IntroController(eavAdminDialogs, eavConfig, oldDialogs, appId) {
        var vm = this;
        function blankCallback() { }

        vm.exportAll = function exp() {
            oldDialogs.appExport(appId, blankCallback);
        };

        vm.import = function () {
            oldDialogs.importPartial(appId, blankCallback);

            // probably afterwards
            //var resolve = eavAdminDialogs.CreateResolve({
            //    appId: appId
            //});
            //return eavAdminDialogs.OpenModal(
            //    "importexport/import.html",
            //    "Import as vm",
            //    "lg",
            //    resolve, blankCallback);
        };

        vm.export = function () {
            oldDialogs.exportPartial(appId, blankCallback);

            // probably afterwards
            //var resolve = eavAdminDialogs.CreateResolve({
            //    appId: appId
            //});
            //return eavAdminDialogs.OpenModal(
            //    "importexport/export.html",
            //    "Export as vm",
            //    "lg",
            //    resolve, blankCallback);
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