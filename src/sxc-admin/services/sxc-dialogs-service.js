/*  this file contains a service to handle open/close of dialogs
*/

angular.module("SxcAdminUi", [
    "ng",
    "ui.bootstrap", // for the $modal etc.
    "MainSxcApp",
    "AppsManagementApp",
    "ReplaceContentApp",
    "SystemSettingsApp",
    "SxcTemplates",
    "EavAdminUi", // dialog (modal) controller
])
    .factory("oldDialogs", function (tabId, AppInstanceId, appId) {
        var svc = {};

        // todo: maybe needs something to get the real root-address
        svc.oldRootUrl = "/Default.aspx?tabid={{tabid}}&mid={{mid}}&ctl={{ctl}}&appid={{appid}}&popUp=true"
            .replace("{{tabid}}", tabId)
            .replace("{{mid}}", AppInstanceId);

            svc.getUrl = function getUrl(ctlName, alternateAppId) {
                return svc.oldRootUrl.replace("{{appid}}", alternateAppId || appId).replace("{{ctl}}", ctlName);
            };

            svc.showInfoOld = function showInfoOld() {
                // alert("Info! \n\n This dialog still uses the old DNN-dialogs. It will open in a new window. After saving/closing that, please refresh this page to see changes made.");
            };

            
        // this will open a browser-window as a modal-promise dialog
        // this is needed for all older, not-yet-migrated ascx-parts
            svc.openPromiseWindow = function opw(url, callback) {
                // note that Success & error both should do the callback, mostly a list-refresh
                PromiseWindow.open(url).then(callback, callback);
            };

            svc.editTemplate = function edit(itemId, callback) {
                svc.showInfoOld();
                var url = svc.getUrl("edittemplate")
                    + ((itemId === 0) ? "" : "&templateid=" + itemId); // must leave parameter away if we want a new-dialog
                svc.openPromiseWindow(url, callback);
            };

            svc.appExport = function appExport(callback) {
                svc.showInfoOld();
                var url = svc.getUrl("appexport", 0);
                svc.openPromiseWindow(url, callback);
            };

            svc.appImport = function appImport(altAppId, callback) {
                svc.showInfoOld();
                var url = svc.getUrl("appimport", altAppId);
                svc.openPromiseWindow(url, callback);
            };

            svc.exportPartial = function exportPartial(callback) {
                svc.showInfoOld();
                var url = svc.getUrl("export", 0);
                svc.openPromiseWindow(url, callback);
            };

            svc.importPartial = function importPartial(altAppId, callback) {
                svc.showInfoOld();
                var url = svc.getUrl("import", altAppId);
                svc.openPromiseWindow(url, callback);
            };
            return svc;
    })

    .factory("sxcDialogs", function ($modal, eavAdminDialogs) {
        var svc = {};

        // the portal-level dialog showing all apps
        svc.openZoneMain = function ozm(zoneId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ zoneId: zoneId });
            return eavAdminDialogs.OpenModal("apps-management/apps.html", "AppList as vm", "xlg", resolve, closeCallback);
        };

        // the app-level dialog showing all app content-items, templates, web-api etc.
        svc.openAppMain = function oam(appId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ appId: appId });
            return eavAdminDialogs.OpenModal("app-main/app-main.html", "AppMain as vm", "xlg", resolve, closeCallback);
        };

        //#region Total-Popup open / close
        svc.openTotal = function openTotal(url, callback) {
            $2sxc.totalPopup.open(url, callback);
        };

        svc.closeThis = function closeThisTotalPopup() {
            $2sxc.totalPopup.closeThis();
        };
        //#endregion

        // the replace-content item
        svc.openReplaceContent = function orc(appId, groupGuid, groupPart, groupIndex, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ groupGuid: groupGuid, groupPart: groupPart, groupIndex: groupIndex });
            return eavAdminDialogs.OpenModal("replace-content/replace-content.html", "ReplaceDialog as vm", "xlg", resolve, closeCallback);
        };

        svc.openLanguages = function orc(zoneId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ zoneId: zoneId });
            return eavAdminDialogs.OpenModal("language-settings/languages.html", "LanguageSettings as vm", "lg", resolve, closeCallback);
        };

        return svc;
    })

;