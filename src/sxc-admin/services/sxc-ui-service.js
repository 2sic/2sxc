/*  this file contains a service to handle 
*/

angular.module("SxcAdminUi", [
    "ng",
    "ui.bootstrap",         // for the $modal etc.
    "EavConfiguration",
    "MainSxcApp",
    "EavAdminUi",           // dialog (modal) controller
])
    .factory("sxcDialogs", function ($modal, eavAdminDialogs, eavConfig) {

        var svc = {};

        //#region Content Items dialogs
        svc.openAppMain = function oam(appId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ appId: appId });
            var callbacks = { close: closeCallback };
            return eavAdminDialogs.OpenModal("main/main.html", "Main as vm", "lg", resolve, callbacks);
        };
        //#endregion

        return svc;
    })

;