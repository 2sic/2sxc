(function () {

    angular.module("ImportExport")
        .controller("ImportContent", ImportContentController)
    ;


    function ImportContentController(ImportContentService, eavAdminDialogs, eavConfig, $modalInstance) {
        var vm = this;

        vm.IsImporting = false;

        vm.ImportFile = {};
        vm.ImportResult = {};

        vm.importContent = importContent;

        vm.close = close;


        function importContent() {
            vm.IsImporting = true;
            return ImportContentService.importContent(vm.ImportFile).then(function (result) {
                vm.ImportResult = result.data;
                vm.IsImporting = false;
            }).catch(function (error) {
                console.log(error);
                vm.IsImporting = false;
            });
        }

        function close() {
            $modalInstance.dismiss("cancel");
        }
    }
}());