/*  this file contains a service to handle open/close of dialogs
*/

angular.module("SxcAdminUi", [
    "ng",
    "ui.bootstrap", // for the $uibModal etc.
    "MainSxcApp",
    "AppsManagementApp",
    "ReplaceContentApp",
    "ReorderContentApp",
    "SystemSettingsApp",
    "SxcTemplates",
    "SxcEditTemplates",
    "sxcFieldTemplates",
    "EavAdminUi", // dialog (modal) controller
])

    .factory("sxcDialogs", function ($uibModal, eavAdminDialogs) {
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

        // the app-level dialog showing all app content-items, templates, web-api etc.
        svc.openAppImport = function oam(closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({}); // { appId: appId }});
            return eavAdminDialogs.OpenModal( "importexport/import-app.html", "ImportApp as vm", "lg", resolve, closeCallback);
        };

        //#region Total-Popup open / close
        svc.openTotal = function openTotal(url, callback) {
            return $2sxc.totalPopup.open(svc.browserFixUrlCaching(url), callback);
        };

            svc.browserFixUrlCaching = function(url) {
                // this fixes a caching issue on IE and FF - see https://github.com/2sic/2sxc/issues/444
                // by default I only need to do this on IE and FF, but to remain consistent, I always do it
                var urlCheck = /(\/ui.html\?sxcver=[0-9\.]*)((&time=)([0-9]*))*/gi;
                if (url.match(urlCheck)) 
                    url = url.replace(urlCheck, "$1&time=" + new Date().getTime());
                return url;
            };

        svc.closeThis = function closeThisTotalPopup() {
            return $2sxc.totalPopup.closeThis();
        };
        //#endregion

        // the replace-content item
        svc.openReplaceContent = function orc(item, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ item: item });
            return eavAdminDialogs.OpenModal("replace-content/replace-content.html", "ReplaceDialog as vm", "lg", resolve, closeCallback);
        };

        svc.openManageContentList = function orcl(item, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ item: item });
            return eavAdminDialogs.OpenModal("manage-content-list/manage-content-list.html", "ManageContentList as vm", "", resolve, closeCallback);
        };


        svc.openDevelop = function ove(item, closeCallback) {
            eavAdminDialogs.openModalComponent("editor", "max", { item: item }, closeCallback);
            //var resolve = eavAdminDialogs.CreateResolve({ item: item });
            //return eavAdminDialogs.OpenModal("source-editor/editor.html", "Editor as vm", "max", resolve, closeCallback);
        };

        // 2dm 2015-10-07 - don't think this is in use, remove
        //svc.openContentEdit = function oce(edit, closeCallback) {
        //    var resolve = eavAdminDialogs.CreateResolve(edit);
        //    return eavAdminDialogs.OpenModal("wrappers/dnn-wrapper.html", "EditInDnn as vm", "lg", resolve, closeCallback);
        //};

        svc.openLanguages = function orc(zoneId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ zoneId: zoneId });
            return eavAdminDialogs.OpenModal("language-settings/languages.html", "LanguageSettings as vm", "lg", resolve, closeCallback);
        };

        return svc;
    })

;