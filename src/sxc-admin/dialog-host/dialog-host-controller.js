(function () { 

    angular.module("DialogHost", [
        "SxcAdminUi",
        "EavAdminUi"
    ])
         
        .controller("DialogHost", DialogHostController)
        ;

    function DialogHostController(zoneId, appId, items, $2sxc, dialog, sxcDialogs, eavAdminDialogs) {
        var vm = this;
        vm.dialog = dialog;
        var initialDialog = dialog;

        vm.close = function close() {
            sxcDialogs.closeThis();
        };

        switch (initialDialog) {
            case "edit":
                eavAdminDialogs.openEditItems(items, vm.close);
                break;
            case "zone":
                // this is the zone-config dialog showing mainly all the apps
                sxcDialogs.openZoneMain(zoneId, vm.close);
                break;
            case "app":
                // this opens the manage-an-app with content-types, views, etc.
                sxcDialogs.openAppMain(appId, vm.close);
                break;
            case "replace":
                // this is the "replace item in a list" dialog
                sxcDialogs.openReplaceContent(items[0], vm.close);
                break;
            case "sort":
                sxcDialogs.openManageContentList(items[0], vm.close);
                break;
            case "template":
                sxcDialogs.openViewEdit(items[0], vm.close);
                break;
            case "pipeline-designer":
                // Don't do anything, as the template already loads the app in fullscreen-mode
                // eavDialogs.editPipeline(appId, pipelineId, closeCallback);
                break;
            default:
                alert("Trying to open an unknown dialog (" + initialDialog + "). Will close again.");
                vm.close();
                throw "Trying to open a dialog, don't know which one";
        }
    }

} ());