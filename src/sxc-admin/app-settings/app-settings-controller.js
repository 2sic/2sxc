(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("AppSettingsApp", [
        "EavConfiguration",     // 
        "EavServices",
        "SxcServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
    ])
        .controller("AppSettings", AppSettingsController)
        ;

    function AppSettingsController(appSettings, appId) {
        var vm = this;
        var svc = appSettings(appId);
        vm.items = svc.liveList();

        vm.ready = function ready() {
            return vm.items.length > 0;
        };

        /// Open a content-type configuration dialog for a type (for settins / resources) 
        vm.config = function openConf(staticName) {
            return svc.openConfig(staticName);
        };

        vm.edit = function edit(staticName) {
            return svc.edit(staticName);
        };

        vm.editPackage = svc.editPackage;

        //vm.export = function exp() {
        //    oldDialogs.appExport(appId, svc.liveListReload);
        //};

        //vm.importParts = function() {
        //    // probably afterwards
        //    var resolve = eavAdminDialogs.CreateResolve({
        //        appId: appId
        //    });
        //    return eavAdminDialogs.OpenModal(
        //        "importexport/import.html",
        //        "Import as vm",
        //        "lg",
        //        resolve);
        //};

        //vm.exportParts = function() {
        //    // probably afterwards
        //    var resolve = eavAdminDialogs.CreateResolve({
        //        appId: appId
        //    });
        //    return eavAdminDialogs.OpenModal(
        //        "importexport/export.html",
        //        "Export as vm",
        //        "lg",
        //        resolve);
        //};

    }

} ());