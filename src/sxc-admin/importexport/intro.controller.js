(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExportApp")
        .controller("ImportExportIntro", IntroController)
        ;

    function IntroController(eavAdminDialogs, eavConfig, oldDialogs, appId) {
        var vm = this;
        function blankCallback() { }

        vm.exportAll = function exp() {
            oldDialogs.appExport(appId, blankCallback);

            // todo: 2tk probably afterwards
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
            //    "ExportContent as vm",
            //    "lg",
            //    resolve, blankCallback);
        };
    }

} ());