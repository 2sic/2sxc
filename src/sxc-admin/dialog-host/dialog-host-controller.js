(function () { 

    angular.module("DialogHost", [
        "SxcAdminUi",
        "EavAdminUi"
    ])
         
        .controller("DialogHost", DialogHostController)
        ;

    function DialogHostController(zoneId, appId, entityId, $2sxc, dialog, sxcDialogs, eavAdminDialogs) {
        var vm = this;
        vm.dialog = dialog;
        var initialDialog = dialog;

        vm.close = function close() {
            sxcDialogs.closeThis();
        };

        switch (initialDialog) {
            case "edit":
                // todo: editor
                eavAdminDialogs.openItemEditWithEntityId(entityId, vm.close);
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
                var groupGuid = $2sxc.urlParams.get("groupguid");
                var groupPart = $2sxc.urlParams.get("grouppart");
                var groupIndex = $2sxc.urlParams.get("groupindex");
                sxcDialogs.openReplaceContent(appId, groupGuid, groupPart, groupIndex, vm.close);
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