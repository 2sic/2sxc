(function() { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("MainSxcApp", [
            "EavConfiguration", // config
            "SxcTemplates", // inline templates
            "EavAdminUi", // dialog (modal) controller
            "EavServices", // multi-language stuff
            "SxcFilters", // for inline unsafe urls
            "ContentTypesApp",
            "PipelineManagement",
            "TemplatesApp",
            "ImportExportApp",
            "AppSettingsApp",
            "SystemSettingsApp",
            "WebApiApp"
        ])
        .config(["$translatePartialLoaderProvider", function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        }])
        .controller("AppMain", MainController)
        .factory("appDialogConfigSvc", ["appId", "$http", function(appId, $http) {
            var svc = {};

            // this will retrieve an advanced getting-started url to use in an the iframe
            svc.getDialogSettings = function gettingStartedUrl() {
                return $http.get("app/system/dialogsettings", { params: { appId: appId } });
            };
            return svc;
        }]);

    function MainController(eavAdminDialogs, eavConfig, appId, debugState, appDialogConfigSvc, $modalInstance) {
        var vm = this;
        vm.debug = debugState;
        vm.view = "start";

        appDialogConfigSvc.getDialogSettings().then(function (result) {
            vm.config = result.data;
        });

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    MainController.$inject = ["eavAdminDialogs", "eavConfig", "appId", "debugState", "appDialogConfigSvc", "$modalInstance"];

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("AppSettingsApp", [
        "EavConfiguration",     // 
        "EavServices",
        "SxcServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
    ])
        .controller("AppSettings", AppSettingsController)
        ;

    function AppSettingsController(appSettings, appId) {
        var vm = this;
        var svc = appSettings(appId);
        vm.items = svc.liveList();

        vm.ready = function ready() {
            return vm.items.length > 0;
        };

        /// Open a content-type configuration dialog for a type (for settins / resources) 
        vm.config = function openConf(staticName) {
            return svc.openConfig(staticName);
        };

        vm.edit = function edit(staticName) {
            return svc.edit(staticName);
        };

        vm.editPackage = svc.editPackage;

        //vm.export = function exp() {
        //    oldDialogs.appExport(appId, svc.liveListReload);
        //};

        //vm.importParts = function() {
        //    // probably afterwards
        //    var resolve = eavAdminDialogs.CreateResolve({
        //        appId: appId
        //    });
        //    return eavAdminDialogs.OpenModal(
        //        "importexport/import.html",
        //        "Import as vm",
        //        "lg",
        //        resolve);
        //};

        //vm.exportParts = function() {
        //    // probably afterwards
        //    var resolve = eavAdminDialogs.CreateResolve({
        //        appId: appId
        //    });
        //    return eavAdminDialogs.OpenModal(
        //        "importexport/export.html",
        //        "Export as vm",
        //        "lg",
        //        resolve);
        //};

    }
    AppSettingsController.$inject = ["appSettings", "appId"];

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("AppsManagementApp", [
        "EavServices",
        "EavConfiguration",
        "SxcServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "SxcAdminUi"
    ])
        .config(["$translatePartialLoaderProvider", function ($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        }])

        .controller("AppList", AppListController)
        ;

    function AppListController(appsSvc, eavAdminDialogs, sxcDialogs, eavConfig, appSettings, zoneId, oldDialogs, $modalInstance, $translate) {
        var vm = this;

        var svc = appsSvc(zoneId);
        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        // config from here fails, because it has to open the full dialog in another app
        //vm.config = function config(item) {
        //    var settings = appSettings(item.Id);
        //    settings.editPackage(svc.liveListReload);
        //    //alert("known bug: atm in this beta this feature has a bug - it only works for the app, in which you opened this dialog. ");
        //    //eavAdminDialogs.openItemEditWithEntityId(item.ConfigurationId, svc.liveListReload);
        //};

        vm.add = function add() {
            var result = prompt($translate.instant("AppManagement.Prompt.NewApp"));
            if (result)
                svc.create(result);
        };

        
        vm.tryToDelete = function tryToDelete(item) {
            var result = prompt($translate.instant("AppManagement.Prompt.DeleteApp", { name: item.Name, id: item.Id}));
                //prompt("This cannot be undone. To really delete this app, type (or copy/past) the app-name here: Delete '" + item.Name + "' (" + item.Id + ") ?");
            if (result === null)
                return;
            if(result === item.Name)
                svc.delete(item.Id);
            else 
                alert($translate.instant("AppManagement.Prompt.FailedDelete"));
        };

        // note that manage MUST open in a new iframe, to give the entire application 
        // a new initial context. otherwise we get problems with AppId and similar
        vm.manage = function manage(item) {
            var url = window.location.href;
            url = url
                .replace(new RegExp("appid=[0-9]*", "i"), "appid=" + item.Id) // note: sometimes it doesn't have an appid, so it's [0-9]* instead of [0-9]+
                .replace(/approot=[^&]*/, "approot=" + item.AppRoot + "/")
                .replace("dialog=zone", "dialog=app");

            sxcDialogs.openTotal(url, svc.liveListReload);
        };


        vm.browseCatalog = function() {
            window.open("http://2sxc.org/apps");
        };

        vm.import = function imp() {
            oldDialogs.appImport(svc.liveListReload);
        };

        vm.export = function exp(item)
        {
            oldDialogs.appExport(item.Id, svc.liveListReload);
        };

        vm.languages = function languages() {
            sxcDialogs.openLanguages(zoneId, vm.refresh);
        };

        vm.close = function () { $modalInstance.dismiss("cancel");};
    }
    AppListController.$inject = ["appsSvc", "eavAdminDialogs", "sxcDialogs", "eavConfig", "appSettings", "zoneId", "oldDialogs", "$modalInstance", "$translate"];

} ());
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
            case "develop":
                // lazy load this to ensure the module is "registered" inside 2sxc
                $ocLazyLoad.load([
                        //"../lib/angular-ui-ace/ui-ace.min.js",
                        $2sxc.debug.renameScript("../sxc-develop/sxc-develop.min.js")
                    ])
                    .then(function() {
                        sxcDialogs.openDevelop(items[0], vm.close);
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
    DialogHostController.$inject = ["zoneId", "appId", "items", "$2sxc", "dialog", "sxcDialogs", "eavAdminDialogs", "$ocLazyLoad"];

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("SxcFilters", [])
        .constant("createdBy", "2sic") // just a demo how to use constant or value configs in AngularJS
        .constant("license", "MIT") // these wouldn't be necessary, just added for learning exprience
        .filter('trustAsResourceUrl', ["$sce", function($sce) {
            return function(val) {
                return $sce.trustAsResourceUrl(val);
            };
        }])
        .filter('trustHtml', ["$sce", function($sce) {
            return function(text) {
                return $sce.trustAsHtml(text);
            };
        }]);

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExportApp", [
        "EavConfiguration",     // config
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "EavServices",              // multi-language stuff
        "SxcServices"
        //"SxcFilters",           // for inline unsafe urls
    ])
        .controller("ImportExportIntro", IntroController)
        .controller("Import", ImportController)
        .controller("Export", ExportController)
        ;

    function IntroController(eavAdminDialogs, eavConfig, oldDialogs, appId) {
        var vm = this;
        function blankCallback() { }

        vm.exportAll = function exp() {
            oldDialogs.appExport(appId, blankCallback);
        };

        vm.import = function () {
            oldDialogs.importPartial(appId, blankCallback);

            // probably afterwards
            //var resolve = eavAdminDialogs.CreateResolve({
            //    appId: appId
            //});
            //return eavAdminDialogs.OpenModal(
            //    "importexport/import.html",
            //    "Import as vm",
            //    "lg",
            //    resolve, blankCallback);
        };

        vm.export = function () {
            oldDialogs.exportPartial(appId, blankCallback);

            // probably afterwards
            //var resolve = eavAdminDialogs.CreateResolve({
            //    appId: appId
            //});
            //return eavAdminDialogs.OpenModal(
            //    "importexport/export.html",
            //    "Export as vm",
            //    "lg",
            //    resolve, blankCallback);
        };
    }
    IntroController.$inject = ["eavAdminDialogs", "eavConfig", "oldDialogs", "appId"];

    function ImportController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    ImportController.$inject = ["eavAdminDialogs", "eavConfig", "appId", "$modalInstance"];

    function ExportController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    ExportController.$inject = ["eavAdminDialogs", "eavConfig", "appId", "$modalInstance"];
} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("SystemSettingsApp", [
        "EavConfiguration",     // 
        "EavServices",
        "SxcServices",
        "SxcTemplates",         // inline templates
//        "EavAdminUi",           // dialog (modal) controller
    ])

        .controller("LanguageSettings", LanguagesSettingsController)
        ;

    function LanguagesSettingsController(languagesSvc, eavConfig, appId) {
        var vm = this;
        var svc = languagesSvc();
        vm.items = svc.liveList();

        // vm.refresh = 
        vm.ready = function ready() {
            return vm.items.length > 0;
        };

        vm.toggle = svc.toggle;

        vm.save = svc.save;
    }
    LanguagesSettingsController.$inject = ["languagesSvc", "eavConfig", "appId"];

} ());
(function () {

    angular.module("ReorderContentApp", [
            "SxcServices",
            "EavAdminUi" // dialog (modal) controller
    ])

        .config(["$translatePartialLoaderProvider", function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("inpage");
        }])

        .controller("ManageContentList", ManageContentController);

    function ManageContentController(appId, item, contentGroupSvc, eavAdminDialogs, $modalInstance, $translate) {
        var vm = this;
        vm.items = [];
        vm.header = {};
        vm.contentGroup = {
            id: item.EntityId,
            guid: item.Group.Guid,
            part: item.Group.Part,
            index: item.Group.Index
        };

        var svc = contentGroupSvc(appId);

        vm.reload = function () {
            return svc.getList(vm.contentGroup).then(function (result) {
                vm.items = result.data;
            });
        };
        vm.reload();

        vm.reloadHeader = function() {
            return svc.getHeader(vm.contentGroup).then(function(result) {
                vm.header = result.data;
            });
        };
        vm.reloadHeader();

        vm.ok = function ok() {
            svc.saveList(vm.contentGroup, vm.items).then(vm.close);
        };
        
        // note: not perfect yet - won't edit presentation of header
        vm.editHeader = function editHeader() {
            var items = [];
            items.push({
                Group: {
                    Guid: vm.contentGroup.guid,
                    Index: 0,
                    Part: "listcontent",
                    Add: vm.header.Id === "0"
                },
                Title: $translate.instant("EditFormTitle.ListContent")
            });
            items.push({
                Group: {
                    Guid: vm.contentGroup.guid,
                    Index: 0,
                    Part: "listpresentation",
                    Add: vm.header.Id === "0"
                },
                Title: $translate.instant("EditFormTitle.ListPresentation")
            });
            eavAdminDialogs.openEditItems(items, vm.reloadHeader);

        };

        vm.close = function () { $modalInstance.dismiss("cancel"); };

    }
    ManageContentController.$inject = ["appId", "item", "contentGroupSvc", "eavAdminDialogs", "$modalInstance", "$translate"];

} ());
(function () { 

    angular.module("ReplaceContentApp", [
            "SxcServices",
            "EavAdminUi"         // dialog (modal) controller
        ])
        .controller("ReplaceDialog", ReplaceContentController);

    function ReplaceContentController(appId, item, contentGroupSvc, eavAdminDialogs, $modalInstance, $filter) {
        var vm = this;
        vm.options = [];
        vm.item = {
            id: item.EntityId,
            guid: item.Group.Guid,
            part: item.Group.Part,
            index: item.Group.Index
        };

        var svc = contentGroupSvc(appId);

        vm.reload = function() {
            return svc.getItems(vm.item).then(function(result) {
                vm.options = result.data.Items;
                vm.item.id = result.data.SelectedId;
            });
        };
        vm.reload();

        vm.ok = function ok() {
            svc.saveItem(vm.item).then(vm.close);
        };
        
        vm.close = function () { $modalInstance.dismiss("cancel"); };

        vm.convertToInt = function (id) {
            return parseInt(id);
        };

        vm.copySelected = function copySelected() {
            var selectedId = vm.item.id;
            var items = [
                {
                    //ContentTypeName: contentType,
                    DuplicateEntity: vm.item.id
                }
            ];
            eavAdminDialogs.openEditItems(items, vm.reloadAfterCopy);
            // todo: on re-load a select would be nice
        };

        vm.reloadAfterCopy = function reloadAfterCopy(result) {
            var copy = result.data;
            vm.reload().then(function() {
                vm.item.id = copy[Object.keys(copy)[0]]; // get id of first item
            });
        };
    }
    ReplaceContentController.$inject = ["appId", "item", "contentGroupSvc", "eavAdminDialogs", "$modalInstance", "$filter"];

} ());
// Init the main eav services module
angular.module("SxcServices", [
    "ng",                   // Angular for $http etc.
    "EavConfiguration",     // global configuration
	"EavServices"
//    "pascalprecht.translate",
]);
angular.module("SxcServices")
    .factory("appSettings", ["$http", "eavConfig", "svcCreator", "contentTypeSvc", "contentItemsSvc", "eavAdminDialogs", "$filter", function ($http, eavConfig, svcCreator, contentTypeSvc, contentItemsSvc, eavAdminDialogs, $filter) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = contentTypeSvc(appId, "2SexyContent-App");
            //svc.appId = appId;
            svc.promise = svc.liveListReload(); // try to load the data..

            svc.openConfig = function openConf(staticName, afterEvent) {
                return svc.promise.then(function() {
                    var items = svc.liveList();
                    var found = $filter("filter")(items, { StaticName: staticName }, true);
                    if (found.length !== 1)
                        throw "Found too many settings for the type " + staticName;
                    var item = found[0];
                    return eavAdminDialogs.openContentTypeFields(item, afterEvent);
                });
            };

            svc.edit = function edit(staticName, afterEvent) {
                return svc.promise.then(function() {
                    var contentSvc = contentItemsSvc(svc.appId, staticName);
                    return contentSvc.liveListReload().then(function(result) {
                        var found = result.data;
                        if (found.length !== 1)
                            throw "Found too many settings for the type " + staticName;
                        var item = found[0];
                        return eavAdminDialogs.openItemEditWithEntityId(item.Id, afterEvent);
                    });
                });
            };

            svc.editPackage = function editPackage(callback) {
                return svc.edit("2SexyContent-App", callback);
            };

            return svc;
        };
    }]);
angular.module("SxcServices")
    .factory("appsSvc", ["$http", "eavConfig", "svcCreator", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(zoneId) {
            var svc = {
                zoneId: zoneId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/system/apps", { params: { zoneId: svc.zoneId } });
            }));

            svc.create = function create(name) {
                return $http.post("app/system/app", {}, { params: { zoneId: svc.zoneId, name: name } })
                    .then(svc.liveListReload);
            };

            // delete, then reload
            // for unclear reason, the verb DELETE fails, so I'm using get for now
            svc.delete = function del(appId) {
                return $http.get("app/system/deleteapp", {params: { zoneId: svc.zoneId, appId: appId } })
                    .then(svc.liveListReload);
            };

            return svc;
        };
    }]);

angular.module("SxcServices")
    .factory("contentGroupSvc", ["$http", "eavConfig", "svcCreator", "$resource", function($http, eavConfig, svcCreator, $resource) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                getItems: function(item) {
                    return $http.get('app/contentgroup/replace', { params: { appId: appId, guid: item.guid, part: item.part, index: item.index } });
                },
                saveItem: function(item) {
                    return $http.post('app/contentgroup/replace', {}, { params: { guid: item.guid, part: item.part, index: item.index, entityId: item.id } });
                },

                getList: function (contentGroup) {
                    return $http.get('app/contentgroup/itemlist', { params: { appId: appId, guid: contentGroup.guid } });
                },

                saveList: function (contentGroup, resortedList) {
                    return $http.post('app/contentgroup/itemlist', resortedList, { params: { appId: appId, guid: contentGroup.guid } });
                },

                getHeader: function (contentGroup) {
                    return $http.get('app/contentgroup/header', { params: { appId: appId, guid: contentGroup.guid } });
                }


            };

            return svc;
        };
    }]);
angular.module("SxcServices")//, ['ng', 'eavNgSvcs', "EavConfiguration"])
    .factory("importExportSvc", ["$http", "eavConfig", "svcCreator", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            // todo: 2tk - everything here is only demo code

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/template/getall", { params: { appId: svc.appId } });
            }));

            // delete, then reload
            svc.delete = function del(id) {
                return $http.delete("sxc/templates/delete", {params: {appId: svc.appId, id: id }})
                    .then(svc.liveListReload);
            };
            return svc;
        };
    }]);
// By default, eav-controls assume that all their parameters (appId, etc.) are instantiated by the bootstrapper
// but the "root" component must get it from the url
// Since different objects could be the root object (this depends on the initial loader), the root-one must have
// a connection to the Url, but only when it is the root
// So the trick is to just include this file - which will provide values for the important attribute
//
// As of now, it supplies
// - dialog -> which dialog to show
// - tabid -> the url tab id
// - items - list of items to edit

//(function () {
    angular.module("InitSxcParametersFromUrl", ["2sxc4ng"])
        .factory("dialog", ["$2sxc", function($2sxc) {
            return $2sxc.urlParams.get("dialog");
        }])
        .factory("tabId", ["$2sxc", function($2sxc) {
            return $2sxc.urlParams.get("tid");
        }])

        .factory("websiteRoot", ["$2sxc", function ($2sxc) {
            return $2sxc.urlParams.get("websiteroot");
        }])
        .factory("systemRoot", ["websiteRoot", function (websiteRoot) {
            return websiteRoot + "desktopmodules/tosic_sexycontent/";
        }])
        .factory("portalRoot", ["$2sxc", function ($2sxc) {
            return $2sxc.urlParams.get("portalroot");
        }])
        .factory("appRoot", ["$2sxc", function ($2sxc) {
                return $2sxc.urlParams.get("appRoot");
        }])
        .factory("items", ["$2sxc", function ($2sxc) {
                var found = $2sxc.urlParams.get("items");
                if (found)
                    return (found) ? JSON.parse(found) : null;
        }])
    ;



// }());
angular.module("SxcServices")
    .factory("languagesSvc", ["$http", "eavConfig", "svcCreator", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/system/getlanguages");//, { params: { appId: svc.appId } });
            }));

            // delete, then reload
            svc.toggle = function toggle(item) {
                return $http.get("app/system/switchlanguage", {params: {cultureCode: item.Code, enable: !item.IsEnabled }})
                    .then(svc.liveListReload);
            };

            svc.save = function save(item) {
                return $http.get("app/system/switchlanguage", { params: { cultureCode: item.Code, enable: item.IsEnabled } })
                    .then(svc.liveListReload);
            };

            return svc;
        };
    }]);
/*  this file contains a service to handle open/close of dialogs
*/

angular.module("SxcAdminUi", [
    "ng",
    "ui.bootstrap", // for the $modal etc.
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
    .factory("oldDialogs", ["tabId", "AppInstanceId", "appId", "websiteRoot", "$q", function (tabId, AppInstanceId, appId, websiteRoot, $q) {
        var svc = {};

        // todo: maybe needs something to get the real root-address
        svc.oldRootUrl = websiteRoot + "Default.aspx?tabid={{tabid}}&mid={{mid}}&ctl={{ctl}}&appid={{appid}}&popUp=true"
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
                if(!window.Promise) // Special workaround to enable promiseWindow in IE without jQuery
                    PromiseWindow.defaultConfig.promiseProvider = PromiseWindow.getAPlusPromiseProvider($q);
                PromiseWindow.open(url).then(callback, callback);
            };

            svc.editTemplate = function edit(itemId, callback) {
                svc.showInfoOld();
                var url = svc.getUrl("edittemplate")
                    + ((itemId === 0) ? "" : "&templateid=" + itemId); // must leave parameter away if we want a new-dialog
                svc.openPromiseWindow(url, callback);
            };

            svc.appExport = function appExport(altAppId, callback) {
                svc.showInfoOld();
                var url = svc.getUrl("appexport", altAppId);
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
    }])

    .factory("sxcDialogs", ["$modal", "eavAdminDialogs", function ($modal, eavAdminDialogs) {
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
            return $2sxc.totalPopup.open(svc.browserFixUrlCaching(url), callback);
        };

            svc.browserFixUrlCaching = function(url) {
                // this fixes a caching issue on IE and FF - see https://github.com/2sic/2sxc/issues/444
                // by default I only need to do this on IE and FF, but to remain consistent, I always do it
                var urlCheck = /(\/ui.html(\?time=[0-9]*)*)#/gi;
                if (url.match(urlCheck)) 
                    url = url.replace(urlCheck, "/ui.html?time=" + new Date().getTime() + "#");
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
            var resolve = eavAdminDialogs.CreateResolve({ item: item });
            return eavAdminDialogs.OpenModal("source-editor/editor.html", "Editor as vm", "xlg", resolve, closeCallback);
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
    }])

;
angular.module("SxcServices")//, ['ng', 'eavNgSvcs', "EavConfiguration"])
    .factory("templatesSvc", ["$http", "eavConfig", "svcCreator", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/template/getall", { params: { appId: svc.appId } });
            }));

            // delete, then reload, for now must use httpget because delete sometimes causes issues
            svc.delete = function del(id) {
                return $http.get("app/template/delete", {params: {appId: svc.appId, Id: id }})
                    .then(svc.liveListReload);
            };
            return svc;
        };
    }]);
angular.module("SxcServices")
    .factory("webApiSvc", ["$http", "eavConfig", "svcCreator", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/system/webapifiles", { params: { appId: svc.appId } });
            }));

            return svc;
        };
    }]);
angular.module('SxcTemplates', []).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('app-main/app-main.html',
    "<div ng-click=vm.debug.autoEnableAsNeeded($event)><div class=modal-header><button class=\"btn btn-default btn-square btn-subtle pull-right\" type=button ng-click=vm.close()><i icon=remove></i></button><h3 class=modal-title translate=Main.Title></h3></div><div class=modal-body><div><tabset><tab><tab-heading><span tooltip=\"{{'Main.Tab.GettingStarted' | translate }}\"><i icon=home></i> {{'Main.Tab.GS' | translate | trustHtml }}</span></tab-heading><iframe ng-src=\"{{ vm.config.GettingStartedUrl | trustAsResourceUrl }}\" style=\"border: none; width: 100%; height: 500px\"></iframe><div ng-hide=true>original script, maybe we need it $(document).ready(function () { $(window).bind(\"resize\", function () { var GettingStartedFrame = $(\".sc-iframe-gettingstarted\"); GettingStartedFrame.height($(window).height() - 50); }); $(window).trigger(\"resize\"); });</div></tab><tab select=\"vm.view='content'\"><tab-heading><span icon=list tooltip=\"{{'Main.Tab.ContentData' | translate }}\"></span> {{'Main.Tab.CD' | translate }}</tab-heading><div ng-if=\"vm.view == 'content'\"><div ng-controller=\"List as vm\" ng-include=\"'content-types/content-types.html'\"></div></div></tab><tab select=\"vm.view='query'\" ng-if=!vm.config.IsContent><tab-heading><span icon=filter tooltip=\"{{'Main.Tab.Query' | translate }}\"></span> {{'Main.Tab.Q' | translate }}</tab-heading><div ng-if=\"vm.view == 'query'\"><div ng-controller=\"PipelineManagement as vm\" ng-include=\"'pipelines/pipelines.html'\"></div></div></tab><tab select=\"vm.view='view'\"><tab-heading><span icon=picture tooltip=\"{{'Main.Tab.ViewsTemplates' | translate }}\"></span> {{'Main.Tab.VT' | translate }}</tab-heading><div ng-if=\"vm.view == 'view'\"><div ng-controller=\"TemplateList as vm\" ng-include=\"'templates/templates.html'\"></div></div></tab><tab select=\"vm.view='webapi'\" ng-if=!vm.config.IsContent><tab-heading><span icon=flash tooltip=\"{{'Main.Tab.WebApi' | translate }}\"></span> {{'Main.Tab.WA' | translate }}</tab-heading><div ng-if=\"vm.view == 'webapi'\"><div ng-controller=\"WebApiMain as vm\" ng-include=\"'web-api/web-api.html'\"></div></div></tab><tab select=\"vm.view='app'\"><tab-heading><span icon=unchecked tooltip=\"{{'Main.Tab.AppSettings' | translate }}\"></span> {{'Main.Tab.AS' | translate }}</tab-heading><div ng-if=\"vm.view == 'app'\"><div ng-if=!vm.config.IsContent><div ng-controller=\"AppSettings as vm\" ng-include=\"'app-settings/app-settings.html'\"></div><br></div><div ng-controller=\"ImportExportIntro as vm\" ng-include=\"'importexport/intro.html'\"></div></div></tab><tab select=\"vm.view='portal'\"><tab-heading><span icon=globe tooltip=\"{{'Main.Tab.PortalLanguages' | translate }}\"></span> {{'Main.Tab.PL' | translate }}</tab-heading><div ng-if=\"vm.view == 'portal'\"><h3 translate=Main.Portal.Title></h3><div translate=Main.Portal.Intro></div><div ng-controller=\"LanguageSettings as vm\" ng-include=\"'language-settings/languages.html'\"></div></div></tab></tabset></div></div><show-debug-availability class=pull-right></show-debug-availability></div>"
  );


  $templateCache.put('app-settings/app-settings.html',
    "<div class=modal-body><h3 translate=AppConfig.Title></h3><div translate=AppConfig.Intro></div><div><div class=\"pull-left btn-group-vertical\"><button tooltip=\"{{ 'AppConfig.Settings.Edit' | translate }}\" ng-disabled=!vm.ready() ng-click=\"vm.edit('App-Settings')\" class=\"btn btn-primary btn-square\" type=button><i icon=pencil></i></button> <button tooltip=\"{{ 'AppConfig.Settings.Config' | translate }}\" ng-disabled=!vm.ready() ng-click=\"vm.config('App-Settings')\" class=\"btn btn-default btn-square\" type=button><i icon=cog></i></button></div><div style=\"margin-left: 50px\"><h4 class=modal-title translate=AppConfig.Settings.Title></h4><div translate=AppConfig.Settings.Intro></div></div></div><br><div><div class=\"pull-left btn-group-vertical\"><button tooltip=\"{{'AppConfig.Resources.Edit' | translate}}\" ng-disabled=!vm.ready() ng-click=\"vm.edit('App-Resources')\" class=\"btn btn-square btn-primary\" type=button><i icon=pencil></i></button> <button tooltip=\"{{'AppConfig.Resources.Config' | translate}}\" ng-disabled=!vm.ready() ng-click=\"vm.config('App-Resources')\" class=\"btn btn-square btn-default\" type=button><i icon=cog></i></button></div><div style=\"margin-left: 50px\"><h4 class=modal-title translate=AppConfig.Resources.Title></h4><div translate=AppConfig.Resources.Intro></div></div></div><br><div><div class=\"pull-left btn-group-vertical\"><button tooltip=\"{{'AppConfig.Definition.Edit' | translate}}\" ng-disabled=!vm.ready() ng-click=vm.editPackage() class=\"btn btn-primary btn-square\" type=button><i icon=cog></i></button></div><div style=\"margin-left: 50px\"><h4 class=modal-title translate=AppConfig.Definition.Title></h4><div translate=AppConfig.Definition.Intro></div></div></div></div>"
  );


  $templateCache.put('apps-management/apps.html',
    "<div><div class=modal-header><button icon=remove class=\"btn btn-default btn-square btn-subtle pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=AppManagement.Title></h3></div><div class=modal-body><span class=btn-group><button type=button class=\"btn btn-primary\" ng-click=vm.browseCatalog()><span icon=search></span> {{ 'AppManagement.Buttons.Browse' | translate }}</button> <button type=button class=btn ng-click=vm.import()><span icon=import tooltip=\"{{ 'AppManagement.Buttons.Import' | translate }}\"></span></button> <button type=button class=btn ng-click=vm.add()><span icon=plus tooltip=\"{{ 'AppManagement.Buttons.Create' | translate }}\"></span></button> <button type=button class=btn ng-click=vm.refresh()><span icon=repeat></span></button> <button type=button class=btn ng-click=vm.languages()><span icon=globe></span></button></span><table class=\"table table-hover\"><thead><tr><th translate=AppManagement.Table.Name></th><th translate=AppManagement.Table.Folder></th><th><span icon=eye-open tooltip=\"{{ 'AppManagement.Table.Show' | translate }}\"></span></th><th translate=AppManagement.Table.Actions></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:'Title'\" ng-click=vm.manage(item)><td class=clickable><span tooltip=\"\r" +
    "\n" +
    "Id: {{item.Id}}\r" +
    "\n" +
    "Guid: {{item.Guid}}\">{{item.Name}}</span></td><td class=clickable>{{item.Folder}}</td><td><span icon=\"{{ item.IsHidden ? 'eye-close' : 'eye-open' }}\"></span></td><td stop-event=click><button icon=export class=\"btn btn-xs\" type=button ng-click=vm.export(item)></button> <button icon=remove ng-disabled={{!item.IsApp}} type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div></div>"
  );


  $templateCache.put('importexport/export.html',
    "<div><div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=ImportExport.Export.Title></h3></div><div class=modal-body><div translate=ImportExport.Export.Intro></div><div translate=ImportExport.Export.FurtherHelp></div>todo: 2tk export stuff + messages, errors etc.</div></div>"
  );


  $templateCache.put('importexport/import.html',
    "<div><div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=ImportExport.Import.Title></h3></div><div class=modal-body>todo import<div translate=ImportExport.Import.Explanation></div>todo 2tk - upload, and following messages, errors etc.</div></div>"
  );


  $templateCache.put('importexport/intro.html',
    "<div class=modal-body><div><div class=\"pull-left btn-group-vertical\"><button tooltip=\"{{'AppConfig.Export.Button' | translate}}\" ng-click=vm.exportAll() class=\"btn btn-square btn-primary\" type=button><i icon=export></i></button></div><div style=\"margin-left: 50px\"><h4 class=modal-title translate=AppConfig.Export.Title></h4><div translate=AppConfig.Export.Intro></div></div></div><br><div><div class=\"pull-left btn-group-vertical\"><button tooltip=\"{{'ImportExport.Buttons.Export' | translate}}\" ng-click=vm.export() class=\"btn btn-square btn-primary\" type=button><i icon=export></i></button> <button tooltip=\"{{'ImportExport.Buttons.Import' | translate}}\" ng-click=vm.import() class=\"btn btn-square btn-primary\" type=button><i icon=import></i></button></div><div style=\"margin-left: 50px\"><h4 class=modal-title translate=ImportExport.Title></h4><div translate=ImportExport.Intro></div><div translate=ImportExport.FurtherHelp></div></div></div></div>"
  );


  $templateCache.put('language-settings/languages.html',
    "<div><div class=modal-header><h4 translate=Language.Title></h4></div><div class=modal-body><div translate=Language.Intro></div><table class=\"table table-hover\"><thead><tr><th translate=Language.Table.Code></th><th translate=Language.Table.Culture></th><th translate=Language.Table.Status></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:['ContentType.Name','Name']\" class=clickable-row ng-click=vm.toggle(item)><td class=clickable>{{item.Code}}</td><td class=clickable>{{item.Culture}}</td><td class=\"clickable text-nowrap\"><span ng-click=vm.save(item) stop-event=click><switch ng-model=item.IsEnabled></switch></span></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table>&nbsp;</div></div>"
  );


  $templateCache.put('manage-content-list/manage-content-list.html',
    "<div class=modal-header><button class=\"btn btn-default btn-square btn-subtle pull-right\" type=button ng-click=vm.close()><i icon=remove></i></button><h3 class=modal-title translate=ManageContentList.Title></h3></div><div><div class=modal-body><div><p translate=ManageContentList.HeaderIntro></p><div><a ng-if=vm.header.Type ng-click=vm.editHeader()>{{ vm.header.Title }} <i icon=pencil class=pull-right></i></a> <span ng-if=!vm.header.Type translate=ManageContentList.NoHeaderInThisList></span></div><br></div><div><p translate=ManageContentList.Intro></p><div ui-tree=options data-empty-placeholder-enabled=false><ol ui-tree-nodes ng-model=vm.items><li ng-repeat=\"item in vm.items\" ui-tree-node class=eav-entityselect-item style=\"width: 100%\"><div ui-tree-handle><i icon=move title=\"{{ 'FieldType.Entity.DragMove' | translate }}\" class=\"pull-left eav-entityselect-sort\"></i> &nbsp; {{item.Title}} ({{item.Id}})</div></li></ol></div></div></div></div><div class=modal-footer><button class=\"btn btn-primary btn-square btn-lg pull-left\" type=button ng-click=vm.ok()><i icon=ok></i></button></div>"
  );


  $templateCache.put('replace-content/replace-content.html',
    "<div class=modal-header><button class=\"btn btn-default btn-square btn-subtle pull-right\" type=button ng-click=vm.close()><i icon=remove></i></button><h3 class=modal-title translate=ReplaceContent.Title></h3></div><div><div class=modal-body><p translate=ReplaceContent.Intro></p><div translate=ReplaceContent.ChooseItem></div><div><select class=input-lg ng-model=vm.item.id ng-options=\"vm.convertToInt(key) as ((value || '[?]') + ' (' + key + ')') for (key,value) in vm.options\"></select>&nbsp;<button type=button class=\"btn btn-lg\" ng-click=vm.copySelected()><i icon=duplicate></i></button></div></div></div><div class=modal-footer><button class=\"btn btn-primary btn-square btn-lg pull-left\" type=button ng-click=vm.ok()><i icon=ok></i></button></div>"
  );


  $templateCache.put('templates/edit.html',
    "<div class=modal-header><button class=\"btn pull-right\" type=button ng-click=vm.close()><span class=\"glyphicon glyphicon-remove\"></span></button><h3 class=modal-title translate=TemplateEdit.Title></h3></div><div class=modal-body>For template metadata - not for source... todo todo todo</div><style>.tooltip-inner {\r" +
    "\n" +
    "    white-space:pre-wrap;\r" +
    "\n" +
    "}</style>"
  );


  $templateCache.put('templates/templates.html',
    "<div class=modal-body><button icon=plus type=button class=\"btn btn-primary btn-square\" ng-click=vm.add()></button> <span class=btn-group ng-if=vm.debug.on><button icon=repeat type=button class=\"btn btn-square\" ng-click=vm.refresh()></button></span><table class=\"table table-hover\"><thead><tr><th translate=Templates.Table.TName></th><th translate=Templates.Table.TPath></th><th translate=Templates.Table.CType></th><th translate=Templates.Table.DemoC></th><th><span tooltip=\"{{'Templates.Table.Show' | translate}}\"><i icon=eye-open></i></span></th><th translate=Templates.Table.UrlKey></th><th translate=Templates.Table.Actions></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:['ContentType.Name','Name']\" class=clickable-row ng-click=vm.edit(item)><td class=clickable>{{item.Name}}</td><td class=clickable><span tooltip={{item.TemplatePath}}>...{{item.TemplatePath.split(\"/\").pop()}}</span></td><td class=clickable><span tooltip=\"\r" +
    "\n" +
    "Cont: {{item.ContentType.Name}} ({{item.ContentType.Id}})\r" +
    "\n" +
    "Pres: {{item.PresentationType.Name}} ({{item.PresentationType.Id}})\r" +
    "\n" +
    "ListC: {{item.ListContentType.Name}} ({{item.ListContentType.Id}})\r" +
    "\n" +
    "ListP: {{item.ListPresentationType.Name}} ({{item.ListPresentationType.Id}})\r" +
    "\n" +
    "\">{{item.ContentType.Name}}</span></td><td class=clickable><span tooltip=\"\r" +
    "\n" +
    "Demo: {{item.ContentType.DemoTitle}} ({{item.ContentType.DemoId}})\r" +
    "\n" +
    "Pres: {{item.PresentationType.DemoTitle}} ({{item.PresentationType.DemoId}})\r" +
    "\n" +
    "ListC: {{item.ListContentType.DemoTitle}} ({{item.ListContentType.DemoId}})\r" +
    "\n" +
    "ListP: {{item.ListPresentationType.DemoTitle}} ({{item.ListPresentationType.DemoId}})\r" +
    "\n" +
    "\">{{item.ContentType.DemoId}}</span></td><td><span icon=\"{{ item.IsHidden ? 'close' : 'eye-open'}}\"></span></td><td class=clickable><span tooltip={{item.ViewNameInUrl}}>{{item.ViewNameInUrl}}</span></td><td class=text-nowrap stop-event=click><button type=button class=\"btn btn-xs btn-square\" ng-click=vm.permissions(item)><i icon=user></i></button> <button type=button class=\"btn btn-xs btn-square\" ng-click=vm.tryToDelete(item)><span icon=remove></span></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table><div translate=Templates.InfoHideAdvanced></div></div>"
  );


  $templateCache.put('web-api/web-api.html',
    "<div class=modal-header><h3 class=modal-title translate=WebApi.Title></h3></div><div class=modal-body><p translate=WebApi.Intro></p><button icon=plus type=button class=\"btn btn-square\" ng-click=vm.add()></button> <button icon=repeat type=button class=\"btn btn-square\" ng-click=vm.refresh()></button><table class=\"table table-hover\"><thead><tr><th translate=WebApi.ListTitle></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:['ContentType.Name','Name']\"><td><span tooltip={{item.TemplatePath}}>{{item}}</span></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table><p translate=WebApi.QuickStart></p></div>"
  );

}]);

(function () { 

    angular.module("TemplatesApp", [
        "SxcServices",
        "EavConfiguration",
        "EavAdminUi",
        "EavServices",
        "EavDirectives"
    ])
        .controller("TemplateList", TemplateListController)
        ;

    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, debugState, oldDialogs, $translate, $modalInstance, $sce) {
        var vm = this;
        vm.debug = debugState;

        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            oldDialogs.editTemplate(item.Id, svc.liveListReload);
            // eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.add = function add() {
            oldDialogs.editTemplate(0, svc.liveListReload);

            return;
            // templ till the edit dialog is JS-only
            //window.open(vm.getOldEditUrl());

            //// probably afterwards
            //var resolve = eavAdminDialogs.CreateResolve({
            //    appId: appId,
            //    svc: svc
            //});
            //return eavAdminDialogs.OpenModal(
            //    "templates/edit.html",
            //    "TemplateEdit as vm",
            //    "lg",
            //    resolve,
            //    svc.liveListReload);
        };

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.permissions = function permissions(item) {
            return eavAdminDialogs.openPermissionsForGuid(appId, item.Guid, svc.liveListReload);
        };

        vm.tryToDelete = function tryToDelete(item) {
            if (confirm($translate.instant("General.Questions.DeleteEntity", { title: item.Name, id: item.Id})))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    TemplateListController.$inject = ["templatesSvc", "eavAdminDialogs", "eavConfig", "appId", "debugState", "oldDialogs", "$translate", "$modalInstance", "$sce"];

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("TemplatesApp")
        .controller("TemplateEdit", TemplateEditController)
        ;

    function TemplateEditController(svc, eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;

        vm.items = svc.liveList();

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    TemplateEditController.$inject = ["svc", "eavAdminDialogs", "eavConfig", "appId", "$modalInstance"];

} ());
(function () { 

    angular.module("WebApiApp", [
        "SxcServices",
        //"EavConfiguration",
        "EavAdminUi",
        "EavServices",
        "EavDirectives"
    ])
        .controller("WebApiMain", WebApiMainController)
        ;

    function WebApiMainController(appId, webApiSvc, eavAdminDialogs, $modalInstance, $translate) {
        var vm = this;
        
        var svc = webApiSvc(appId);

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.add = function add() {
            alert($translate.instant("WebApi.AddDoesntExist"));
        };

        // not implemented yet...
        vm.tryToDelete = function tryToDelete(item) {
            if (confirm($translate.instant("General.Messages.DeleteEntity", { title: item.Title, id: item.Id})))   //"Delete '" + item.Title + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    WebApiMainController.$inject = ["appId", "webApiSvc", "eavAdminDialogs", "$modalInstance", "$translate"];

} ());