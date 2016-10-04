(function () { 

    angular.module("ImportExport")
        .controller("ImportExportIntro", IntroController)
        ;

    /*@ngInject*/
    function IntroController(eavAdminDialogs, eavConfig, appId) {
        var vm = this;
        function blankCallback() { }

        vm.exportAll = function exp() {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/export-app.html",
                "ExportApp as vm",
                "lg",
                resolve, blankCallback);

        };

        vm.import = function () {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/import-content.html",
                "ImportContent as vm",
                "lg",
                resolve, blankCallback);
        };

        vm.export = function () {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/export-content.html",
                "ExportContent as vm",
                "lg",
                resolve, blankCallback);
        };
    }

} ());