(function () { 

    angular.module("DialogHost", [
        "SxcAdminUi",
        "EavAdminUi"
    ])
         
        .controller("DialogHost", DialogHostController)
        ;

    function DialogHostController(zoneId, appId, dialog, sxcDialogs, eavAdminDialogs) {
        var vm = this;
        vm.dialog = dialog;
        var initialDialog = dialog;

        vm.close = function close() {
            //todo: close window, pass message on to top-frame
            alert('close');
            // window.close();
        };

        switch (initialDialog) {
            case "edit":
                // todo: editor
                break;
            case "zone":
                sxcDialogs.openZoneMain(zoneId, vm.close);
                break;
            case "app":
                sxcDialogs.openAppMain(appId, vm.close);
                break;
            case "pipeline-designer":
                // Don't do anything, as the template already loads the app in fullscreen-mode
                //eavDialogs.editPipeline(appId, pipelineId, closeCallback);
                break;
            default:
                throw "Trying to open a dialog, don't know which one";
        }
    }

} ());