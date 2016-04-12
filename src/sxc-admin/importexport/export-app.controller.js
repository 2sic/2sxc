(function () {

    angular.module("ImportExport")
        .controller("ExportApp", ExportAppController)
        ;
  

    function ExportAppController(ExportAppService, eavAdminDialogs, eavConfig, $modalInstance) {
        var vm = this;

        vm.IsExporting = false;

        vm.IncludeContentGroups = false;
        vm.ResetAppGuid = false;

        vm.AppInfo = {};

        vm.getAppInfo = getAppInfo;
        vm.exportApp = exportApp;

        vm.close = close;


        activate();

        function activate() {
            getAppInfo();
        }

        function getAppInfo() {
            return ExportAppService.getAppInfo().then(function (result) {
                vm.AppInfo = result;
            });
        }

        function exportApp() {
            vm.IsExporting = true;
            return ExportAppService.exportApp(vm.IncludeContentGroups, vm.ResetAppGuid).then(function () {
                vm.IsExporting = false;
            }).catch(function () {
                vm.IsExporting = false;
            });
        }

        function close() {
            $modalInstance.dismiss("cancel");
        }
    }
}());