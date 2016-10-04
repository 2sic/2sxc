(function () {

    angular.module("ImportExport")
        .controller("ExportApp", ExportAppController)
        ;
  

    /*@ngInject*/
    function ExportAppController(ExportAppService, eavAdminDialogs, debugState, eavConfig, $uibModalInstance) {
        var vm = this;
        vm.debug = debugState;

        vm.IsExporting = false;

        vm.IncludeContentGroups = false;
        vm.ResetAppGuid = false;

        vm.AppInfo = {};

        vm.getAppInfo = getAppInfo;
        vm.exportApp = exportApp;
        vm.exportGit = exportGit;
        vm.close = close;


        activate();

        function activate() {
            getAppInfo();
        }

        // retrieve additional statistics & metadata about this app
        function getAppInfo() {
            return ExportAppService.getAppInfo().then(function (result) {
                vm.AppInfo = result;
            });
        }

        // this will call the export-app on the server
        function exportApp() {
            vm.IsExporting = true;
            return ExportAppService.exportApp(vm.IncludeContentGroups, vm.ResetAppGuid).then(function () {
                vm.IsExporting = false;
            }).catch(function () {
                vm.IsExporting = false;
            });
        }

        // this will tell the server to export the data in the DB so it can be used in version control
        function exportGit() {
            vm.IsExporting = true;
            return ExportAppService.exportForVersionControl(vm.IncludeContentGroups, vm.ResetAppGuid).then(function () {
                vm.IsExporting = false;
                alert("done - please check you '.data' folder");
            }).catch(function () {
                vm.IsExporting = false;
            });
        }

        function close() {
            $uibModalInstance.dismiss("cancel");
        }
    }
}());