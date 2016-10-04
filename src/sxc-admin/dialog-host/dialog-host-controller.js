(function () { 

    angular.module("DialogHost", [
        "SxcAdminUi",
        "EavAdminUi",
        "oc.lazyLoad",

        "eavEditEntity" // new it must be added here, so it's available in the entire application - not good architecture, must fix someday
    ])
         
        .controller("DialogHost", DialogHostController)
        ;

    function preLoadAgGrid($ocLazyLoad) {
        return $ocLazyLoad.load([
            "../lib/ag-grid/ag-grid.min.js",
            "../lib/ag-grid/ag-grid.min.css"
        ]);

    }

    /*@ngInject*/
    function DialogHostController(zoneId, appId, items, $2sxc, dialog, sxcDialogs, contentTypeName, eavAdminDialogs, $ocLazyLoad) {
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
                preLoadAgGrid($ocLazyLoad).then(function() {
                    sxcDialogs.openAppMain(appId, vm.close);
                });
                break;
            case "app-import":
                // this is the zone-config dialog showing mainly all the apps
                sxcDialogs.openAppImport(vm.close);
                break;
            case "replace":
                // this is the "replace item in a list" dialog
                sxcDialogs.openReplaceContent(items[0], vm.close);
                break;
            case "sort":
                sxcDialogs.openManageContentList(items[0], vm.close);
                break;
            case "develop":
                // lazy load this to ensure the module is "registered" inside 2sxc
                $ocLazyLoad.load([
                        $2sxc.parts.getUrl("../sxc-develop/sxc-develop.min.js")
                    ])
                    .then(function() {
                        sxcDialogs.openDevelop(items[0], vm.close);
                    });
                break;
            case "contenttype":
                eavAdminDialogs.openContentTypeFieldsOfItems(items, vm.close);
                break;
            case "contentitems":
                preLoadAgGrid($ocLazyLoad).then(function() {
                    eavAdminDialogs.openContentItems(appId, contentTypeName, contentTypeName, vm.close);
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