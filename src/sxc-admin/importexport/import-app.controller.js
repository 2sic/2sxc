(function () {

    angular.module("ImportExport")
        .controller("ImportApp", ImportAppController)
    ;

    function ImportAppController(ImportAppService, eavAdminDialogs, eavConfig, $uibModalInstance) {
        var vm = this;

        vm.IsImporting = false;

        vm.ImportFile = {};
        vm.ImportResult = {};

        vm.importApp = importApp;

        vm.close = close;


        function importApp() {
            vm.IsImporting = true;
            return ImportAppService.importApp(vm.ImportFile).then(function (result) {
                vm.ImportResult = result.data;
                vm.IsImporting = false;
            }).catch(function (error) {
                vm.IsImporting = false;
            });
        }

        function close() {
            $uibModalInstance.dismiss("cancel");
        }
    }
}());