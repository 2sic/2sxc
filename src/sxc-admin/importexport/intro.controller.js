(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExport")
        .controller("ImportExportIntro", IntroController)
        ;

    function IntroController(eavAdminDialogs, eavConfig, oldDialogs, appId) {
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