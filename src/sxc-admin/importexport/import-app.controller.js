(function () {

    angular.module("ImportExport")
        .controller("ImportApp", ImportAppController)
    ;

    function ImportAppController(ImportAppService, eavAdminDialogs, eavConfig, $modalInstance) {
        var vm = this;

        vm.ImportFile = {};
        vm.ImportResult = {};

        vm.importApp = importApp;

        vm.close = close;


        function importApp() {
            return ImportAppService.importApp(vm.ImportFile.Name, vm.ImportFile.Data).then(function (result) {
                vm.ImportResult = result.data;
            });
        }

        function close() {
            $modalInstance.dismiss("cancel");
        }
    }
}());