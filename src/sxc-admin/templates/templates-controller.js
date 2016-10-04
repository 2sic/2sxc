(function () { 

    angular.module("TemplatesApp", [
        "SxcServices",
        "EavConfiguration",
        "EavAdminUi",
        "EavServices",
        "EavDirectives"
    ])
        .controller("TemplateList", TemplateListController)
        ;

    /*@ngInject*/
    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, debugState, $translate, $uibModalInstance) {
        var vm = this;
        vm.debug = debugState;

        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };
        


        vm.add = function addNew() {
            eavAdminDialogs.openItemNew("2SexyContent-Template", svc.liveListReload);
        };

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.permissions = function permissions(item) {
            return eavAdminDialogs.openPermissionsForGuid(appId, item.Guid, svc.liveListReload);
        };

        vm.tryToDelete = function tryToDelete(item) {
            if (confirm($translate.instant("General.Questions.DeleteEntity", { title: item.Name, id: item.Id})))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $uibModalInstance.dismiss("cancel");
        };
    }

} ());