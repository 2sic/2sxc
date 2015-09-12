(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("TemplatesApp", [
        "TemplatesServices",
        "EavConfiguration",
        "EavAdminUi",
        "Sxci18n"])
        .constant("createdBy", "2sic")          // just a demo how to use constant or value configs in AngularJS
        .constant("license", "MIT")             // these wouldn't be necessary, just added for learning exprience
        .controller("TemplateList", TemplateListController)
        ;

    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;
        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.add = function add() {
            alert('todo');
            // todo: JSRefactor continue here
            var resolve = eavAdminDialogs.CreateResolve({ appId: appId, contentType: eavConfig.contentType.template});
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