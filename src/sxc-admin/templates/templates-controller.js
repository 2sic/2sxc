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

    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, oldDialogs, $modalInstance, $sce) {
        var vm = this;
        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            oldDialogs.editTemplate(item.Id, svc.liveListReload);
            // eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.add = function add() {
            oldDialogs.editTemplate(0, svc.liveListReload);

            return;
            // templ till the edit dialog is JS-only
            //window.open(vm.getOldEditUrl());

            //// probably afterwards
            //var resolve = eavAdminDialogs.CreateResolve({
            //    appId: appId,
            //    svc: svc
            //});
            //return eavAdminDialogs.OpenModal(
            //    "templates/edit.html",
            //    "TemplateEdit as vm",
            //    "lg",
            //    resolve,
            //    svc.liveListReload);
        };

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.permissions = function permissions(item) {
            return eavAdminDialogs.openPermissionsForGuid(appId, item.Guid, svc.liveListReload);
        };

        vm.tryToDelete = function tryToDelete(item) {
            if (confirm("Delete '" + item.Title + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());