(function () { 

    angular.module("TemplatesApp", [
        "TemplatesServices",
        "EavConfiguration",
        "EavAdminUi",
        "Eavi18n",
        "EavDirectives"
    ])
        .controller("TemplateList", TemplateListController)
        ;

    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, $modalInstance, $sce) {
        var vm = this;
        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.add = function add() {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId,
                svc: svc
            });
            return eavAdminDialogs.OpenModal(
                "templates/edit.html",
                "TemplateEdit as vm",
                "lg",
                resolve,
                svc.liveListReload);
        };

        vm.items = svc.liveList();

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