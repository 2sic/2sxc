(function () {

    angular.module("ImportExport")
        .controller("ImportContent", ImportContentController)
    ;


    function ImportContentController(ImportContentService, eavAdminDialogs, eavConfig, $modalInstance) {
        var vm = this;

        vm.ImportFile = {};
        vm.ImportResult = {};

        vm.importContent = importContent;

        vm.close = close;


        function importContent() {
            return ImportContentService.importContent(vm.ImportFile.Name, vm.ImportFile.Data).then(function (result) {
                vm.ImportResult = result.data;
            });
        }

        function close() {
            $modalInstance.dismiss("cancel");
        }
    }
}());