(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("TemplatesApp", [
        "TemplatesServices",
        "EavConfiguration",
        "EavAdminUi",
        "Sxci18n"])
        .constant("createdBy", "2sic")          // just a demo how to use constant or value configs in AngularJS
        .constant("license", "MIT")             // these wouldn't be necessary, just added for learning exprience
        .controller("PermissionList", PermissionListController)
        ;

    function PermissionListController(templatesSvc, eavAdminDialogs, eavConfig, appId, targetGuid, $modalInstance) {
        var vm = this;
        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            alert('todo');
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.add = function add() {
            alert('todo');
            eavAdminDialogs.openMetadataNew("entity", svc.PermissionTargetGuid, svc.ctName, svc.liveListReload);
        };

        vm.items = svc.liveList();
        
        vm.tryToDelete = function tryToDelete(item) {
            if (confirm("Delete '" + item.Title + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());