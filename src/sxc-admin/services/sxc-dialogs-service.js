/*  this file contains a service to handle 
*/

angular.module("SxcAdminUi", [
    "ng",
    "ui.bootstrap",         // for the $modal etc.
    //"EavConfiguration",
    "MainSxcApp",
    "AppsManagementApp",
    "ReplaceContentApp",
    "EavAdminUi",           // dialog (modal) controller
])
    .factory("sxcDialogs", function ($modal, eavAdminDialogs) {
        var svc = {};

        svc.openAppMain = function oam(appId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ appId: appId });
            return eavAdminDialogs.OpenModal("app-main/app-main.html", "AppMain as vm", "xlg", resolve, closeCallback);
        };

        svc.openTotal = function openTotal(url, callback) {
            $2sxc.totalPopup.open(url, callback);
        };

        svc.closeThis = function closeThisTotalPopup() {
            $2sxc.totalPopup.closeThis();
        };
        //svc.reopenAppMain = function roam(appId, closeCallback) {

        //};

        svc.openZoneMain = function ozm(zoneId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ zoneId: zoneId });
            return eavAdminDialogs.OpenModal("apps-management/apps.html", "AppList as vm", "xlg", resolve, closeCallback);
        };

        svc.openReplaceContent = function orc(groupId, groupSet, groupIndex, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ groupId: groupId, groupSet : groupSet, groupIndex:groupIndex});
            return eavAdminDialogs.OpenModal("replace-content/replace-content.html", "ReplaceDialog as vm", "xlg", resolve, closeCallback);
        };

        return svc;
    })

;