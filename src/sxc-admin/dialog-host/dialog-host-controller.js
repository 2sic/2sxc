(function () { 

    angular.module("DialogHost", [
        "SxcAdminUi",
        "EavAdminUi",
        "oc.lazyLoad"
    ])
         
        .controller("DialogHost", DialogHostController)
        ;

    function DialogHostController(zoneId, appId, items, $2sxc, dialog, sxcDialogs, eavAdminDialogs, $ocLazyLoad) {
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
                // designer, must first load more dependencies...
                //var cdn = $ocLazyLoad.load([
                //    "//cdn.jsdelivr.net/ace/1.2.2/noconflict/ace.js",
                //    "//cdn.jsdelivr.net/ace/1.2.2/noconflict/ext-language_tools.js"
                //]);
                //var locals =
                //    //cdn.then(function () {
                //    //return
                    $ocLazyLoad.load([
                        //"../lib/angular-ui-ace/ui-ace.min.js",
                        "../sxc-designer/sxc-designer.min.js"
                    ])
                ////});
                //locals
                        .then(function() {
                        sxcDialogs.openViewEdit(items[0], vm.close);
                    });
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