(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("AppManagementApp", [
        "PermissionsServices",
        "EavConfiguration",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "Eavi18n"               // multi-language stuff
    ])
        .controller("AppList", AppListController)
        ;

    function AppListController(permissionsSvc, eavAdminDialogs, eavConfig, appId, targetGuid, $modalInstance /* $location */) {
        var vm = this;
        var svc = permissionsSvc(appId, targetGuid);

        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.add = function add() {
            eavAdminDialogs.openMetadataNew("entity", svc.PermissionTargetGuid, svc.ctName, svc.liveListReload);
        };

        vm.items = svc.liveList();
        
        vm.tryToDelete = function tryToDelete(item) {
            if (confirm("Delete '" + item.Title + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
        };

        vm.refresh = svc.liveListReload;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());