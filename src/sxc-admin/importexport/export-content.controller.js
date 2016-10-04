(function () {

    angular.module("ImportExport")
        .controller("ExportContent", ExportContentController)
        ;


    /*@ngInject*/
    function ExportContentController(ExportContentService, eavAdminDialogs, eavConfig, debugState, $uibModalInstance, $filter) {
        var vm = this;

        vm.debug = debugState;

        vm.IsExporting = false;

        vm.ExportScope = "2SexyContent";

        vm.ContentInfo = null;

        vm.getContentInfo = getContentInfo;
        vm.exportContent = exportContent;
        vm.changeExportScope = changeExportScope;

        vm.close = close;


        activate();

        function activate() {
            getContentInfo();
        }


        function getContentInfo() {
            return ExportContentService.getContentInfo(vm.ExportScope).then(function (result) { vm.ContentInfo = result; });
        }

        function exportContent() {     
            var contentTypeIds = selectedContentTypes().map(function (item) { return item.Id; });
            var templateIds = selectedTemplates().map(function (item) { return item.Id; });
            var entityIds = selectedEntities().map(function (item) { return item.EntityId; });
            entityIds = entityIds.concat(templateIds);
            
            vm.IsExporting = true;
            return ExportContentService.exportContent(contentTypeIds, entityIds, templateIds).then(function () {
                vm.IsExporting = false;
            }).catch(function () {
                vm.IsExporting = false;
            });
        }


        function selectedContentTypes() {
            return $filter("filter")(vm.ContentInfo.ContentTypes, { _export: true });
        }

        function selectedEntities() {
            var entities = [];
            angular.forEach(vm.ContentInfo.ContentTypes, function (item) {
                entities = entities.concat(
                    $filter("filter")(item.Entities, { _export: true })
                );
            });
            return entities;
        }

        function selectedTemplates() {
            // The ones with...
            var templates = [];
            angular.forEach(vm.ContentInfo.ContentTypes, function (item) {
                templates = templates.concat(
                    $filter("filter")(item.Templates, { _export: true })
                );
            });
            // ...and without content types
            templates = templates.concat($filter("filter")(vm.ContentInfo.TemplatesWithoutContentTypes, { _export: true }));
            return templates;
        }

        function changeExportScope() {
            var newExportScope = prompt("Enter an new dcope for export");
            if (newExportScope) {
                vm.ExportScope = newExportScope;
            }
            return getContentInfo();
        }

        function close() {
            $uibModalInstance.dismiss("cancel");
        }
    }

}());