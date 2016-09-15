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

    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, debugState, oldDialogs, $translate, $modalInstance, $sce) {
        var vm = this;
        vm.debug = debugState;

        var svc = templatesSvc(appId);

        vm.editOld = function editOld(item) {
            oldDialogs.editTemplate(item.Id, svc.liveListReload);
        };
        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };
        


        vm.addOld = function add() {
            oldDialogs.editTemplate(0, svc.liveListReload);
            return;
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
            $modalInstance.dismiss("cancel");
        };
    }

} ());