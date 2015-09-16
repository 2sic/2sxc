/*  this file contains a service to handle 
*/

angular.module("SxcAdminUi", [
    "ng",
    "ui.bootstrap",         // for the $modal etc.
    //"EavConfiguration",
    "MainSxcApp",
    "AppsManagementApp",
    "EavAdminUi",           // dialog (modal) controller
])
    .factory("sxcDialogs", function ($modal, eavAdminDialogs) {
        var svc = {};

        svc.openAppMain = function oam(appId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ appId: appId });
            return eavAdminDialogs.OpenModal("main/main.html", "Main as vm", "xlg", resolve, closeCallback);
        };

        //svc.reopenAppMain = function roam(appId, closeCallback) {

        //};

        svc.openZoneMain = function ozm(zoneId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ zoneId: zoneId });
            return eavAdminDialogs.OpenModal("apps-management/apps.html", "AppList as vm", "xlg", resolve, closeCallback);
        };

        return svc;
    })

;