(function() { 

    MainController.$inject = ["eavAdminDialogs", "eavConfig", "appId", "debugState", "appDialogConfigSvc", "$uibModalInstance"];
    angular.module("MainSxcApp", [
            "EavConfiguration", // config
            "SxcTemplates", // inline templates
            "EavAdminUi", // dialog (modal) controller
            "EavServices", // multi-language stuff
            "SxcFilters", // for inline unsafe urls
            "ContentTypesApp",
            "PipelineManagement",
            "TemplatesApp",
            "ImportExport",
            "AppSettingsApp",
            "SystemSettingsApp",
            "WebApiApp",
            "SxcServices"
        ])
        /*@ngInject*/
        .config(["$translatePartialLoaderProvider", function ($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        }])
        .controller("AppMain", MainController)
    ;

    /*@ngInject*/
    function MainController(eavAdminDialogs, eavConfig, appId, debugState, appDialogConfigSvc, $uibModalInstance) {
        var vm = this;
        vm.debug = debugState;
        vm.view = "start";

        appDialogConfigSvc.getDialogSettings().then(function (result) {
            vm.config = result.data;
        });

        vm.close = function () {
            $uibModalInstance.dismiss("cancel");
        };
    }

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    AppSettingsController.$inject = ["appSettings", "appId"];
    angular.module("AppSettingsApp", [
        "EavConfiguration",     // 
        "EavServices",
        "SxcServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
    ])
        .controller("AppSettings", AppSettingsController)
        ;

    /*@ngInject*/
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

    }

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    AppListController.$inject = ["appsSvc", "eavAdminDialogs", "sxcDialogs", "eavConfig", "appSettings", "appId", "zoneId", "$uibModalInstance", "$translate"];
    angular.module("AppsManagementApp", [
        "EavServices",
        "EavConfiguration",
        "SxcServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "SxcAdminUi"
    ])
        /*@ngInject*/
        .config(["$translatePartialLoaderProvider", function ($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        }])

        .controller("AppList", AppListController)
        ;

    /*@ngInject*/
    function AppListController(appsSvc, eavAdminDialogs, sxcDialogs, eavConfig, appSettings, appId, zoneId, $uibModalInstance, $translate) {
        var vm = this;

        function blankCallback() { }

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
            sxcDialogs.openAppImport(vm.refresh);
        };

        // export is disabled for now, because it actually doesn't use the id given here, but the 
        // original appId - part of #887
        vm.export = function exp(item) {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: item.Id
            });
            return eavAdminDialogs.OpenModal(
                "importexport/export-app.html",
                "ExportApp as vm",
                "lg",
                resolve, blankCallback);
        };

        vm.languages = function languages() {
            sxcDialogs.openLanguages(zoneId, vm.refresh);
        };

        vm.close = function () { $uibModalInstance.dismiss("cancel");};
    }

} ());
(function () {
    DialogHostController.$inject = ["zoneId", "appId", "items", "$2sxc", "dialog", "sxcDialogs", "contentTypeName", "eavAdminDialogs", "$ocLazyLoad"];
    angular
        .module("DialogHost", [
            "SxcAdminUi",
            "EavAdminUi",
            "oc.lazyLoad",

            "eavEditEntity" // new it must be added here, so it's available in the entire application - not good architecture, must fix someday
        ])
        .controller("DialogHost", DialogHostController);

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
                eavAdminDialogs.openEditItems(items, vm.close, {
                    partOfPage: $2sxc.urlParams.get('partOfPage'),
                    publishing: $2sxc.urlParams.get('publishing')
                });
                break;
            case "zone":
                // this is the zone-config dialog showing mainly all the apps
                sxcDialogs.openZoneMain(zoneId, vm.close);
                break;
            case "app":
                // this opens the manage-an-app with content-types, views, etc.
                preLoadAgGrid($ocLazyLoad).then(function () {
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
            case "instance-list":
                sxcDialogs.openManageContentList(items[0], vm.close);
                break;
            case "develop":
                // lazy load this to ensure the module is "registered" inside 2sxc
                $ocLazyLoad.load([
                        $2sxc.parts.getUrl("../sxc-develop/sxc-develop.min.js")
                    ])
                    .then(function () {
                        sxcDialogs.openDevelop(items[0], vm.close);
                    });
                break;
            case "contenttype":
                eavAdminDialogs.openContentTypeFieldsOfItems(items, vm.close);
                break;
            case "contentitems":
                preLoadAgGrid($ocLazyLoad).then(function () {
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

}());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("SxcFilters", [])
        .constant("createdBy", "2sic") // just a demo how to use constant or value configs in AngularJS
        .constant("license", "MIT") // these wouldn't be necessary, just added for learning exprience
        /*@ngInject*/
        .filter('trustAsResourceUrl', ["$sce", function ($sce) {
            return function(val) {
                return $sce.trustAsResourceUrl(val);
            };
        }])
        /*@ngInject*/
        .filter('trustHtml', ["$sce", function ($sce) {
            return function(text) {
                return $sce.trustAsHtml(text);
            };
        }]);

} ());
(function () { 

    angular.module("ImportExport", [
        "EavConfiguration", // Config
        "SxcTemplates",     // Inline templates
        "EavAdminUi",       // Dialog (modal) controller
        "EavServices",      // Multi-language stuff
        "SxcServices"
    ]);
} ());
(function () {

    ExportAppController.$inject = ["ExportAppService", "eavAdminDialogs", "debugState", "eavConfig", "$uibModalInstance"];
    angular.module("ImportExport")
        .controller("ExportApp", ExportAppController)
        ;
  

    /*@ngInject*/
    function ExportAppController(ExportAppService, eavAdminDialogs, debugState, eavConfig, $uibModalInstance) {
        var vm = this;
        vm.debug = debugState;

        vm.IsExporting = false;

        vm.IncludeContentGroups = false;
        vm.ResetAppGuid = false;

        vm.AppInfo = {};

        vm.getAppInfo = getAppInfo;
        vm.exportApp = exportApp;
        vm.exportGit = exportGit;
        vm.close = close;


        activate();

        function activate() {
            getAppInfo();
        }

        // retrieve additional statistics & metadata about this app
        function getAppInfo() {
            return ExportAppService.getAppInfo().then(function (result) {
                vm.AppInfo = result;
            });
        }

        // this will call the export-app on the server
        function exportApp() {
            vm.IsExporting = true;
            return ExportAppService.exportApp(vm.IncludeContentGroups, vm.ResetAppGuid).then(function () {
                vm.IsExporting = false;
            }).catch(function () {
                vm.IsExporting = false;
            });
        }

        // this will tell the server to export the data in the DB so it can be used in version control
        function exportGit() {
            vm.IsExporting = true;
            return ExportAppService.exportForVersionControl(vm.IncludeContentGroups, vm.ResetAppGuid).then(function () {
                vm.IsExporting = false;
                alert("done - please check you '.data' folder");
            }).catch(function () {
                vm.IsExporting = false;
            });
        }

        function close() {
            $uibModalInstance.dismiss("cancel");
        }
    }
}());
(function () {
    ExportAppService.$inject = ["appId", "zoneId", "eavConfig", "$http", "$q"];
    angular.module("ImportExport")
        .factory("ExportAppService", ExportAppService)
    ;


    /*@ngInject*/
    function ExportAppService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            getAppInfo: getAppInfo,
            exportApp: exportApp,
            exportForVersionControl: exportForVersionControl
        };
        return srvc;

        function getAppInfo() {
            return $http.get(eavConfig.getUrlPrefix("api") + "/app-sys/ImportExport/GetAppInfo", { params: { appId: appId, zoneId: zoneId } }).then(function (result) { return result.data; });
        }

        function exportApp(includeContentGroups, resetAppGuid) {
            window.open(eavConfig.getUrlPrefix("api") + "/app-sys/ImportExport/ExportApp?appId=" + appId + "&zoneId=" + zoneId + "&includeContentGroups=" + includeContentGroups + "&resetAppGuid=" + resetAppGuid, "_self", "");
            return $q.when(true);
        }

        function exportForVersionControl(includeContentGroups, resetAppGuid) {
            // todo: put params in nice params object
            return $http.get("app-sys/ImportExport/ExportForVersionControl?appId=" + appId + "&zoneId=" + zoneId + "&includeContentGroups=" + includeContentGroups + "&resetAppGuid=" + resetAppGuid);

        }
    }
}());
(function () {

    ExportContentController.$inject = ["ExportContentService", "eavAdminDialogs", "eavConfig", "debugState", "$uibModalInstance", "$filter"];
    angular.module("ImportExport")
        .controller("ExportContent", ExportContentController)
        ;


    /*@ngInject*/
    function ExportContentController(ExportContentService, eavAdminDialogs, eavConfig, debugState, $uibModalInstance, $filter) {
        var vm = this;

        vm.debug = debugState;

        vm.IsExporting = false;

        vm.ExportScope = "2SexyContent";

        vm.ContentInfo = null;

        vm.getContentInfo = getContentInfo;
        vm.exportContent = exportContent;
        vm.changeExportScope = changeExportScope;

        vm.close = close;


        activate();

        function activate() {
            getContentInfo();
        }


        function getContentInfo() {
            return ExportContentService.getContentInfo(vm.ExportScope).then(function (result) { vm.ContentInfo = result; });
        }

        function exportContent() {     
            var contentTypeIds = selectedContentTypes().map(function (item) { return item.Id; });
            var templateIds = selectedTemplates().map(function (item) { return item.Id; });
            var entityIds = selectedEntities().map(function (item) { return item.Id; });
            entityIds = entityIds.concat(templateIds);
            
            vm.IsExporting = true;
            return ExportContentService.exportContent(contentTypeIds, entityIds, templateIds).then(function () {
                vm.IsExporting = false;
            }).catch(function () {
                vm.IsExporting = false;
            });
        }


        function selectedContentTypes() {
            return $filter("filter")(vm.ContentInfo.ContentTypes, { _export: true });
        }

        function selectedEntities() {
            var entities = [];
            angular.forEach(vm.ContentInfo.ContentTypes, function (item) {
                entities = entities.concat(
                    $filter("filter")(item.Entities, { _export: true })
                );
            });
            return entities;
        }

        function selectedTemplates() {
            // The ones with...
            var templates = [];
            angular.forEach(vm.ContentInfo.ContentTypes, function (item) {
                templates = templates.concat(
                    $filter("filter")(item.Templates, { _export: true })
                );
            });
            // ...and without content types
            templates = templates.concat($filter("filter")(vm.ContentInfo.TemplatesWithoutContentTypes, { _export: true }));
            return templates;
        }

        function changeExportScope() {
            var newExportScope = prompt("Enter an new dcope for export");
            if (newExportScope) {
                vm.ExportScope = newExportScope;
            }
            return getContentInfo();
        }

        function close() {
            $uibModalInstance.dismiss("cancel");
        }
    }

}());
(function () {

    ExportContentService.$inject = ["appId", "zoneId", "eavConfig", "$http", "$q"];
    angular.module("ImportExport")
        .factory("ExportContentService", ExportContentService)
    ;


    /*@ngInject*/
    function ExportContentService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            getContentInfo: getContentInfo,
            exportContent: exportContent
        };
        return srvc;


        function getContentInfo(scope) {
            return $http.get(eavConfig.getUrlPrefix("api") + "/app-sys/ImportExport/GetContentInfo", { params: { appId: appId, zoneId: zoneId, scope: scope || "2SexyContent" } }).then(function (result) { return result.data; });
        }

        function exportContent(contentTypeIds, entityIds, templateIds) {
            window.open(eavConfig.getUrlPrefix("api") + "/app-sys/ImportExport/ExportContent?appId=" + appId + "&zoneId=" + zoneId + "&contentTypeIdsString=" + contentTypeIds.join(";") + "&entityIdsString=" + entityIds.join(";") + "&templateIdsString=" + templateIds.join(";"), "_self", "");
            return $q.when(true);
        }
    }

}());
(function () {

    ImportAppController.$inject = ["ImportAppService", "eavAdminDialogs", "eavConfig", "$uibModalInstance"];
    angular.module("ImportExport")
        .controller("ImportApp", ImportAppController)
    ;

    /*@ngInject*/
    function ImportAppController(ImportAppService, eavAdminDialogs, eavConfig, $uibModalInstance) {
        var vm = this;

        vm.IsImporting = false;

        vm.ImportFile = {};
        vm.ImportResult = {};

        vm.importApp = importApp;

        vm.close = close;


        function importApp() {
            vm.IsImporting = true;
            return ImportAppService.importApp(vm.ImportFile).then(function (result) {
                vm.ImportResult = result.data;
                vm.IsImporting = false;
            }).catch(function (error) {
                vm.IsImporting = false;
            });
        }

        function close() {
            $uibModalInstance.dismiss("cancel");
        }
    }
}());
(function () {

    ImportAppService.$inject = ["appId", "zoneId", "eavConfig", "$http", "$q"];
    angular.module("ImportExport")
        .factory("ImportAppService", ImportAppService)
    ;


    function ImportAppService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            importApp: importApp,
        };
        return srvc;


        function importApp(file) {
            return $http({
                method: "POST",
                url: "app-sys/ImportExport/ImportApp",
                headers: { "Content-Type": undefined },
                transformRequest: function (data) {
                    var formData = new FormData();
                    formData.append("AppId", data.AppId);
                    formData.append("ZoneId", data.ZoneId);
                    formData.append("File", data.File);
                    return formData;
                },
                data: { AppId: appId, ZoneId: zoneId, File: file }
            });
        }
    }
}());
(function () {

    ImportContentController.$inject = ["ImportContentService", "eavAdminDialogs", "eavConfig", "$uibModalInstance"];
    angular.module("ImportExport")
        .controller("ImportContent", ImportContentController)
    ;


    /*@ngInject*/
    function ImportContentController(ImportContentService, eavAdminDialogs, eavConfig, $uibModalInstance) {
        var vm = this;

        vm.IsImporting = false;

        vm.ImportFile = {};
        vm.ImportResult = {};

        vm.importContent = importContent;

        vm.close = close;


        function importContent() {
            vm.IsImporting = true;
            return ImportContentService.importContent(vm.ImportFile).then(function (result) {
                vm.ImportResult = result.data;
                vm.IsImporting = false;
            }).catch(function (error) {
                console.log(error);
                vm.IsImporting = false;
            });
        }

        function close() {
            $uibModalInstance.dismiss("cancel");
        }
    }
}());
(function () {

    ImportContentService.$inject = ["appId", "zoneId", "eavConfig", "$http", "$q"];
    angular.module("ImportExport")
        .factory("ImportContentService", ImportContentService)
    ;


    /*@ngInject*/
    function ImportContentService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            importContent: importContent
        };
        return srvc;


        function importContent(file) {
            return $http({
                method: "POST",
                url: "app-sys/ImportExport/ImportContent",
                headers: { "Content-Type": undefined },
                transformRequest: function (data) {
                    var formData = new FormData();
                    formData.append("AppId", data.AppId);
                    formData.append("ZoneId", data.ZoneId);
                    formData.append("File", data.File);
                    return formData;
                },
                data: { AppId: appId, ZoneId: zoneId, File: file }
            });
        }
    }

}());
(function () {

    angular.module("ImportExport")
        .directive("sxcFileRead", FileReadDirective)
        .directive("sxcFileInput", FileInputDirective)
        ;

    /*@ngInject*/
    function FileReadDirective() {
        return {
            scope: {
                sxcFileRead: "="
            },
            link: function (scope, element, attributes) {

                element.bind("change", function (e) {
                    var file = e.target.files[0];
                    var fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        scope.$apply(function () {
                            scope.sxcFileRead = {
                                Name: file.name,
                                Data: e.target.result
                            };
                        });
                    };
                    fileReader.readAsDataURL(file);
                });
            }
        };
    }


    function FileInputDirective() {
        return {
            scope: {
                sxcFileInput: "="
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (e) {
                    scope.sxcFileInput = e.target.files[0];
                    scope.$apply();
                });
            }
        };
    }
}());
(function () { 

    IntroController.$inject = ["eavAdminDialogs", "eavConfig", "appId"];
    angular.module("ImportExport")
        .controller("ImportExportIntro", IntroController)
        ;

    /*@ngInject*/
    function IntroController(eavAdminDialogs, eavConfig, appId) {
        var vm = this;
        function blankCallback() { }

        vm.exportAll = function exp() {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/export-app.html",
                "ExportApp as vm",
                "lg",
                resolve, blankCallback);

        };

        vm.import = function () {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/import-content.html",
                "ImportContent as vm",
                "lg",
                resolve, blankCallback);
        };

        vm.export = function () {
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId
            });
            return eavAdminDialogs.OpenModal(
                "importexport/export-content.html",
                "ExportContent as vm",
                "lg",
                resolve, blankCallback);
        };
    }

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    LanguagesSettingsController.$inject = ["languagesSvc", "eavConfig", "appId"];
    angular.module("SystemSettingsApp", [
        "EavConfiguration",     // 
        "EavServices",
        "SxcServices",
        "SxcTemplates",         // inline templates
//        "EavAdminUi",           // dialog (modal) controller
    ])

        .controller("LanguageSettings", LanguagesSettingsController)
        ;

    /*@ngInject*/
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

} ());
(function () {

    ManageContentController.$inject = ["appId", "item", "contentGroupSvc", "eavAdminDialogs", "$uibModalInstance", "$translate"];
    angular.module("ReorderContentApp", [
            "SxcServices",
            "EavAdminUi" // dialog (modal) controller
    ])

        /*@ngInject*/
        .config(["$translatePartialLoaderProvider", function ($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("inpage");
        }])

        .controller("ManageContentList", ManageContentController);

    /*@ngInject*/
    function ManageContentController(appId, item, contentGroupSvc, eavAdminDialogs, $uibModalInstance, $translate) {
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
            eavAdminDialogs.openEditItems(items, vm.reloadHeader, { partOfPage: $2sxc.urlParams.get('partOfPage'), publishing: $2sxc.urlParams.get('publishing') });

        };

        vm.edit = function (id) {
            if (id === null || id === 0)
                return alert('no can do'); // todo: i18n
            //var entities = $filter("filter")($scope.availableEntities, { Value: itemGuid });
            //var id = entities[0].Id;

            eavAdminDialogs.openItemEditWithEntityId(id, vm.reload);
        };

        vm.close = function () { $uibModalInstance.dismiss("cancel"); };

    }

} ());
(function () { 

    ReplaceContentController.$inject = ["appId", "item", "contentGroupSvc", "eavAdminDialogs", "$uibModalInstance", "$filter"];
    angular.module("ReplaceContentApp", [
            "SxcServices",
            "EavAdminUi"         // dialog (modal) controller
        ])
        .controller("ReplaceDialog", ReplaceContentController);

    /*@ngInject*/
    function ReplaceContentController(appId, item, contentGroupSvc, eavAdminDialogs, $uibModalInstance, $filter) {
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
        
        vm.close = function () { $uibModalInstance.dismiss("cancel"); };

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

} ());
// Init the main eav services module
angular.module("SxcServices", [
    "ng",                   // Angular for $http etc.
    "EavConfiguration",     // global configuration
	"EavServices",
    "InitSxcParametersFromUrl",
    "InitParametersFromUrl"
//    "pascalprecht.translate",
]);
angular.module("SxcServices")
    /*@ngInject*/
    .factory("appDialogConfigSvc", ["appId", "$http", function (appId, $http) {
        var svc = {};

        // this will retrieve an advanced getting-started url to use in an the iframe
        svc.getDialogSettings = function gettingStartedUrl() {
            return $http.get("app-sys/system/dialogsettings", { params: { appId: appId } });
        };
        return svc;
    }]);
angular.module("SxcServices")
    /*@ngInject*/
    .factory("appAssetsSvc", ["$http", "eavConfig", "svcCreator", function ($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId, global) {
            var svc = {
                params: {
                    appId: appId,
                    global: global || false
                }
            };

            // ReSharper disable once UseOfImplicitGlobalInFunctionScope
            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app-sys/appassets/list", { params: angular.extend({}, svc.params, { withSubfolders: true }) });
            }));

            svc.create = function create(path, content) {
                return $http.post("app-sys/appassets/create", { content: content || "" }, { params: angular.extend({}, svc.params, { path: path }) })
                    .then(function(result) {
                        if (result.data === false) // must check for an explicit false, to avoid undefineds
                            alert("server reported that create failed - the file probably already exists"); // todo: i18n
                        return result;
                    })
                    .then(svc.liveListReload);
            };

            //// delete, then reload, for now must use httpget because delete sometimes causes issues
            //svc.delete = function del(id) {
            //    return $http.get("app-sys/template/delete", {params: {appId: svc.appId, Id: id }})
            //        .then(svc.liveListReload);
            //};
            return svc;
        };
    }]);
angular.module("SxcServices")
    /*@ngInject*/
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
    /*@ngInject*/
    .factory("appsSvc", ["$http", "eavConfig", "svcCreator", function ($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(zoneId) {
            var svc = {
                zoneId: zoneId
            };
            
            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app-sys/system/apps", { params: { zoneId: svc.zoneId } });
            }));

            svc.create = function create(name) {
                return $http.post("app-sys/system/app", {}, { params: { zoneId: svc.zoneId, name: name } })
                    .then(svc.liveListReload);
            };

            // delete, then reload
            // for unclear reason, the verb DELETE fails, so I'm using get for now
            svc.delete = function del(appId) {
                return $http.get("app-sys/system/deleteapp", {params: { zoneId: svc.zoneId, appId: appId } })
                    .then(svc.liveListReload);
            };

            return svc;
        };
    }]);

angular.module("SxcServices")
    /*@ngInject*/
    .factory("contentGroupSvc", ["$http", function ($http) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                getItems: function(item) {
                    return $http.get("app-sys/contentgroup/replace", { params: { appId: appId, guid: item.guid, part: item.part, index: item.index } });
                },
                saveItem: function(item) {
                    return $http.post("app-sys/contentgroup/replace", {}, { params: { guid: item.guid, part: item.part, index: item.index, entityId: item.id } });
                },

                getList: function (contentGroup) {
                    return $http.get("app-sys/contentgroup/itemlist", { params: { appId: appId, guid: contentGroup.guid } });
                },

                saveList: function (contentGroup, resortedList) {
                    return $http.post("app-sys/contentgroup/itemlist", resortedList, { params: { appId: appId, guid: contentGroup.guid } });
                },

                getHeader: function (contentGroup) {
                    return $http.get("app-sys/contentgroup/header", { params: { appId: appId, guid: contentGroup.guid } });
                }


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
        /*@ngInject*/
        .factory("dialog", ["$2sxc", function ($2sxc) {
            return $2sxc.urlParams.get("dialog");
        }])
        /*@ngInject*/
        .factory("tabId", ["$2sxc", function ($2sxc) {
            return $2sxc.urlParams.get("tid");
        }])
        /*@ngInject*/
        .factory("websiteRoot", ["$2sxc", function ($2sxc) {
            return $2sxc.urlParams.get("websiteroot");
        }])
        /*@ngInject*/
        .factory("systemRoot", ["websiteRoot", function (websiteRoot) {
            return websiteRoot + "desktopmodules/tosic_sexycontent/";
        }])
        /*@ngInject*/
        .factory("portalRoot", ["$2sxc", function ($2sxc) {
            return $2sxc.urlParams.get("portalroot");
        }])
        /*@ngInject*/
        .factory("appRoot", ["$2sxc", function ($2sxc) {
                return $2sxc.urlParams.get("appRoot");
        }])
        /*@ngInject*/
        .factory("items", ["$2sxc", function ($2sxc) {
                var found = $2sxc.urlParams.get("items");
                if (found)
                    return (found) ? JSON.parse(found) : null;
        }])
        /*@ngInject*/
        .factory("beta", ["$2sxc", function ($2sxc) {
            return $2sxc.urlParams.get("beta");
        }])
    ;



// }());
angular.module("SxcServices")
    /*@ngInject*/
    .factory("languagesSvc", ["$http", "eavConfig", "svcCreator", function ($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app-sys/system/getlanguages");//, { params: { appId: svc.appId } });
            }));

            // delete, then reload
            svc.toggle = function toggle(item) {
                return $http.get("app-sys/system/switchlanguage", {params: {cultureCode: item.Code, enable: !item.IsEnabled }})
                    .then(svc.liveListReload);
            };

            svc.save = function save(item) {
                return $http.get("app-sys/system/switchlanguage", { params: { cultureCode: item.Code, enable: item.IsEnabled } })
                    .then(svc.liveListReload);
            };

            return svc;
        };
    }]);
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

    /*@ngInject*/
    .factory("sxcDialogs", ["$uibModal", "eavAdminDialogs", function ($uibModal, eavAdminDialogs) {
        var service = {
            openZoneMain: openZoneMain,
            openAppMain: openAppMain,
            openAppImport: openAppImport,
            openTotal: openTotal,
            browserFixUrlCaching: browserFixUrlCaching,
            closeThis: closeThis,
            openReplaceContent: openReplaceContent,
            openManageContentList: openManageContentList,
            openDevelop: openDevelop,
            openLanguages: openLanguages
        };
        
        return service;
        
        // the portal-level dialog showing all apps
        function openZoneMain(zoneId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ zoneId: zoneId });
            return eavAdminDialogs.OpenModal("apps-management/apps.html", "AppList as vm", "xlg", resolve, closeCallback);
        }

        // the app-level dialog showing all app content-items, templates, web-api etc.
        function openAppMain(appId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ appId: appId });
            return eavAdminDialogs.OpenModal("app-main/app-main.html", "AppMain as vm", "xlg", resolve, closeCallback);
        }

        // the app-level dialog showing all app content-items, templates, web-api etc.
        function openAppImport(closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({}); // { appId: appId }});
            return eavAdminDialogs.OpenModal("importexport/import-app.html", "ImportApp as vm", "lg", resolve, closeCallback);
        }

        //#region Total-Popup open / close
        function openTotal(url, callback) {
            return $2sxc.totalPopup.open(service.browserFixUrlCaching(url), callback);
        }

        function browserFixUrlCaching(url) {
            // this fixes a caching issue on IE and FF - see https://github.com/2sic/2sxc/issues/444
            // by default I only need to do this on IE and FF, but to remain consistent, I always do it
            var urlCheck = /(\/ui.html\?sxcver=[0-9\.]*)((&time=)([0-9]*))*/gi;
            if (url.match(urlCheck))
                url = url.replace(urlCheck, "$1&time=" + new Date().getTime());
            return url;
        }

        function closeThis() {
            return $2sxc.totalPopup.closeThis();
        }
        //#endregion

        // the replace-content item
        function openReplaceContent(item, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ item: item });
            return eavAdminDialogs.OpenModal("replace-content/replace-content.html", "ReplaceDialog as vm", "lg", resolve, closeCallback);
        }

        function openManageContentList(item, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ item: item });
            return eavAdminDialogs.OpenModal("manage-content-list/manage-content-list.html", "ManageContentList as vm", "", resolve, closeCallback);
        }

        function openDevelop(item, closeCallback) {
            eavAdminDialogs.openModalComponent("editor", "max", { item: item }, closeCallback);
            //var resolve = eavAdminDialogs.CreateResolve({ item: item });
            //return eavAdminDialogs.OpenModal("source-editor/editor.html", "Editor as vm", "max", resolve, closeCallback);
        }

        function openLanguages(zoneId, closeCallback) {
            var resolve = eavAdminDialogs.CreateResolve({ zoneId: zoneId });
            return eavAdminDialogs.OpenModal("language-settings/languages.html", "LanguageSettings as vm", "lg", resolve, closeCallback);
        }
    }]);
angular.module("SxcServices")
    /*@ngInject*/
    .factory("templatesSvc", ["$http", "eavConfig", "svcCreator", function ($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app-sys/template/getall", { params: { appId: svc.appId } });
            }));
            
            // delete, then reload, for now must use httpget because delete sometimes causes issues
            svc.delete = function del(id) {
                return $http.get("app-sys/template/delete", {params: {appId: svc.appId, Id: id }})
                    .then(svc.liveListReload);
            };
            return svc;
        };
    }]);
angular.module("SxcServices")
    /*@ngInject*/
    .factory("webApiSvc", ["$http", "eavConfig", "svcCreator", function ($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app-sys/appassets/list", { params: { appId: svc.appId, path: "api", mask: "*.cs" } });
            }));

            return svc;
        };
    }]);
angular.module("SxcTemplates", []).run(["$templateCache", function($templateCache) {$templateCache.put("app-main/app-main.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\">\r\n            <i icon=\"remove\"></i>\r\n        </button>\r\n        <h3 class=\"modal-title\" translate=\"Main.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <div>\r\n            <uib-tabset>\r\n                <uib-tab>\r\n                    <uib-tab-heading>\r\n                        <span uib-tooltip=\"{{\'Main.Tab.GettingStarted\' | translate }}\">\r\n                            <i icon=\"home\"></i> {{\'Main.Tab.GS\' | translate | trustHtml }}\r\n                        </span>\r\n                    </uib-tab-heading>\r\n\r\n                    <iframe ng-src=\"{{ vm.config.GettingStartedUrl | trustAsResourceUrl }}\" style=\"border: none; width: 100%; height: 500px\"></iframe>\r\n                    \r\n                    <div ng-hide=\"true\">\r\n                        original script, maybe we need it\r\n                        $(document).ready(function () {\r\n                        $(window).bind(\"resize\", function () {\r\n                        var GettingStartedFrame = $(\".sc-iframe-gettingstarted\");\r\n                        GettingStartedFrame.height($(window).height() - 50);\r\n                        });\r\n\r\n                        $(window).trigger(\"resize\");\r\n                        });\r\n                    </div>\r\n                </uib-tab>\r\n\r\n                <uib-tab select=\"vm.view=\'content\'\">\r\n                    <uib-tab-heading>\r\n                        <span icon=\"list\" uib-tooltip=\"{{\'Main.Tab.ContentData\' | translate }}\"></span> {{\'Main.Tab.CD\' | translate }}\r\n                    </uib-tab-heading>\r\n                    <div ng-if=\"vm.view == \'content\'\">\r\n                        <div ng-controller=\"List as vm\" ng-include=\"\'content-types/content-types.html\'\"></div>\r\n                    </div>\r\n                </uib-tab>\r\n\r\n\r\n                <uib-tab select=\"vm.view=\'query\'\" ng-if=\"!vm.config.IsContent\">\r\n                    <uib-tab-heading>\r\n                        <span icon=\"filter\" uib-tooltip=\"{{\'Main.Tab.Query\' | translate }}\"></span> {{\'Main.Tab.Q\' | translate }}\r\n                    </uib-tab-heading>\r\n                    <div ng-if=\"vm.view == \'query\'\">\r\n                        <div ng-controller=\"PipelineManagement as vm\" ng-include=\"\'pipelines/pipelines.html\'\"></div>\r\n\r\n                    </div>\r\n                </uib-tab>\r\n\r\n                <uib-tab select=\"vm.view=\'view\'\">\r\n                    <uib-tab-heading>\r\n                        <span icon=\"picture\" uib-tooltip=\"{{\'Main.Tab.ViewsTemplates\' | translate }}\"></span> {{\'Main.Tab.VT\' | translate }}\r\n                    </uib-tab-heading>\r\n                    <div ng-if=\"vm.view == \'view\'\">\r\n                        <div ng-controller=\"TemplateList as vm\" ng-include=\"\'templates/templates.html\'\"></div>\r\n\r\n                    </div>\r\n                </uib-tab>\r\n\r\n\r\n\r\n\r\n                <uib-tab select=\"vm.view=\'webapi\'\" ng-if=\"!vm.config.IsContent\">\r\n                    <uib-tab-heading>\r\n                        <span icon=\"flash\" uib-tooltip=\"{{\'Main.Tab.WebApi\' | translate }}\"></span> {{\'Main.Tab.WA\' | translate }}\r\n                    </uib-tab-heading>\r\n                    <div ng-if=\"vm.view == \'webapi\'\">\r\n                        <div ng-controller=\"WebApiMain as vm\" ng-include=\"\'web-api/web-api.html\'\"></div>\r\n                    </div>\r\n                </uib-tab>\r\n\r\n\r\n                <uib-tab select=\"vm.view=\'app\'\">\r\n                    <uib-tab-heading>\r\n                        <span icon=\"unchecked\" uib-tooltip=\"{{\'Main.Tab.AppSettings\' | translate }}\"></span> {{\'Main.Tab.AS\' | translate }}\r\n                    </uib-tab-heading>\r\n                    <div ng-if=\"vm.view == \'app\'\">\r\n                        <div ng-if=\"!vm.config.IsContent\">\r\n                            <div ng-controller=\"AppSettings as vm\" ng-include=\"\'app-settings/app-settings.html\'\"></div>\r\n                            <br />                            \r\n                        </div>\r\n                        <div ng-controller=\"ImportExportIntro as vm\" ng-include=\"\'importexport/intro.html\'\"></div>\r\n                    </div>\r\n                </uib-tab>\r\n\r\n                <uib-tab select=\"vm.view=\'portal\'\">\r\n                    <uib-tab-heading>\r\n                        <span icon=\"globe\" uib-tooltip=\"{{\'Main.Tab.PortalLanguages\' | translate }}\"></span> {{\'Main.Tab.PL\' | translate }}\r\n                    </uib-tab-heading>\r\n                    <div ng-if=\"vm.view == \'portal\'\">\r\n                        <h3 translate=\"Main.Portal.Title\"></h3>\r\n                        <div translate=\"Main.Portal.Intro\"></div>\r\n                        <div ng-controller=\"LanguageSettings as vm\" ng-include=\"\'language-settings/languages.html\'\"></div>\r\n\r\n                    </div>\r\n                </uib-tab>\r\n\r\n            </uib-tabset>\r\n\r\n        </div>\r\n\r\n\r\n    </div>\r\n    <show-debug-availability class=\"pull-right\"></show-debug-availability>\r\n</div>");
$templateCache.put("app-settings/app-settings.html","    \r\n<div class=\"modal-body\">\r\n    <h3 translate=\"AppConfig.Title\"></h3>\r\n    <div translate=\"AppConfig.Intro\"></div>\r\n\r\n    <div class=\"clearfix\">\r\n        <div class=\"pull-left btn-group-vertical\">\r\n            <button uib-tooltip=\"{{ \'AppConfig.Settings.Edit\' | translate }}\" ng-disabled=\"!vm.ready()\" ng-click=\"vm.edit(\'App-Settings\')\" class=\"btn btn-primary btn-square\" type=\"button\">\r\n                <i icon=\"pencil\"></i>\r\n            </button>\r\n            <button uib-tooltip=\"{{ \'AppConfig.Settings.Config\' | translate }}\" ng-disabled=\"!vm.ready()\" ng-click=\"vm.config(\'App-Settings\')\" class=\"btn btn-default btn-square\" type=\"button\">\r\n                <i icon=\"cog\"></i>\r\n            </button>\r\n        </div>\r\n        <div style=\"margin-left: 50px\">\r\n            <h4 class=\"modal-title\" translate=\"AppConfig.Settings.Title\"></h4>\r\n            <div translate=\"AppConfig.Settings.Intro\"></div>\r\n        </div>\r\n    </div>\r\n    <br/>\r\n    <div class=\"clearfix\">\r\n        <div class=\"pull-left btn-group-vertical\">\r\n            <button uib-tooltip=\"{{\'AppConfig.Resources.Edit\' | translate}}\" ng-disabled=\"!vm.ready()\" ng-click=\"vm.edit(\'App-Resources\')\" class=\"btn btn-square btn-primary\" type=\"button\">\r\n                <i icon=\"pencil\"></i>\r\n            </button>\r\n\r\n            <button uib-tooltip=\"{{\'AppConfig.Resources.Config\' | translate}}\" ng-disabled=\"!vm.ready()\" ng-click=\"vm.config(\'App-Resources\')\" class=\"btn btn-square btn-default\" type=\"button\">\r\n                <i icon=\"cog\"></i>\r\n            </button>\r\n        </div>\r\n        <div style=\"margin-left: 50px\">\r\n            <h4 class=\"modal-title\" translate=\"AppConfig.Resources.Title\"></h4>\r\n            <div translate=\"AppConfig.Resources.Intro\"></div>\r\n        </div>\r\n    </div>\r\n    <br/>\r\n    <div>\r\n        <div class=\"pull-left btn-group-vertical\">\r\n            <button uib-tooltip=\"{{\'AppConfig.Definition.Edit\' | translate}}\" ng-disabled=\"!vm.ready()\" ng-click=\"vm.editPackage()\" class=\"btn btn-primary btn-square\" type=\"button\">\r\n                <i icon=\"cog\"></i>\r\n            </button>\r\n        </div>\r\n        <div style=\"margin-left: 50px\">\r\n\r\n            <h4 class=\"modal-title\" translate=\"AppConfig.Definition.Title\"></h4>\r\n            <div translate=\"AppConfig.Definition.Intro\"></div>\r\n        </div>\r\n    </div>\r\n\r\n</div>");
$templateCache.put("apps-management/apps.html","<div>\r\n    <div class=\"modal-header\">\r\n        <button icon=\"remove\" class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n        <h3 class=\"modal-title\" translate=\"AppManagement.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <span class=\"btn-group\">\r\n            <button type=\"button\" class=\"btn btn-primary\" ng-click=\"vm.browseCatalog()\">\r\n                <span icon=\"search\"></span> {{ \'AppManagement.Buttons.Browse\' | translate }}\r\n            </button>\r\n            <button type=\"button\" class=\"btn\" ng-click=\"vm.import()\">\r\n                <span icon=\"import\" uib-tooltip=\"{{ \'AppManagement.Buttons.Import\' | translate }}\"></span>\r\n            </button>\r\n\r\n            <button type=\"button\" class=\"btn\" ng-click=\"vm.add()\">\r\n                <span icon=\"plus\" uib-tooltip=\"{{ \'AppManagement.Buttons.Create\' | translate }}\"></span>\r\n            </button>\r\n            <button type=\"button\" class=\"btn\" ng-click=\"vm.refresh()\">\r\n                <span icon=\"repeat\"></span>\r\n            </button>\r\n            <button type=\"button\" class=\"btn\" ng-click=\"vm.languages()\">\r\n                <span icon=\"globe\"></span>\r\n            </button>\r\n        </span>\r\n        <table class=\"table table-hover\">\r\n            <thead>\r\n            <tr>\r\n                <th translate=\"AppManagement.Table.Name\"></th>\r\n                <th translate=\"AppManagement.Table.Folder\"></th>\r\n                <!--<th translate=\"AppManagement.Table.Templates\"></th>-->\r\n                <th> <span icon=\"eye-open\" uib-tooltip=\"{{ \'AppManagement.Table.Show\' | translate }}\"></span></th>\r\n                <th translate=\"AppManagement.Table.Actions\"></th>\r\n            </tr>\r\n            </thead>\r\n            <tbody>\r\n            <tr ng-repeat=\"item in vm.items | orderBy:\'Title\'\" ng-click=\"vm.manage(item)\">\r\n                <td class=\"clickable\">\r\n                    <span uib-tooltip=\"\r\nId: {{item.Id}}\r\nGuid: {{item.Guid}}\">{{item.Name}}</span>\r\n                </td>\r\n                <td class=\"clickable\">{{item.Folder}}</td>\r\n                <!--<td></td>-->\r\n                <td><span icon=\"{{ item.IsHidden ? \'eye-close\' : \'eye-open\' }}\"></span> </td>\r\n                <td stop-event=\"click\">\r\n                    <!--<button icon=\"cog\" ng-disabled=\"!item.IsApp\" type=\"button\" class=\"btn btn-xs\" ng-click=\"vm.config(item)\"></button>-->\r\n                    <!-- disabled export because there\'s no good way to pass the app-id to the dialog with dependency injection - part of #887 -->\r\n                    <!--<button icon=\"export\" class=\"btn btn-xs\" type=\"button\" ng-click=\"vm.export(item)\"></button>-->\r\n                    <button icon=\"remove\" ng-disabled=\"{{!item.IsApp}}\" type=\"button\" class=\"btn btn-xs\" ng-click=\"vm.tryToDelete(item)\"></button>\r\n                </td>\r\n            </tr>\r\n            <tr ng-if=\"!vm.items.length\">\r\n                <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n            </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>");
$templateCache.put("importexport/export-app.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n    <div class=\"modal-header\">\r\n        <button icon=\"remove\" class=\"btn pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n        <h3 class=\"modal-title\" translate=\"ImportExport.ExportApp.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <div translate=\"ImportExport.ExportApp.Intro\"></div>\r\n        <div translate=\"ImportExport.ExportApp.FurtherHelp\"></div>\r\n\r\n        <h5>{{\"ImportExport.ExportApp.Specifications.Title\" | translate}}</h5>\r\n        <ul>\r\n            <li>{{\"ImportExport.ExportApp.Specifications.AppName\" | translate}} {{vm.AppInfo.Name}}</li>\r\n            <li>{{\"ImportExport.ExportApp.Specifications.AppGuid\" | translate}} {{vm.AppInfo.Guid}}</li>\r\n            <li>{{\"ImportExport.ExportApp.Specifications.AppVersion\" | translate}} {{vm.AppInfo.Version}}</li>\r\n        </ul>\r\n\r\n        <h5>{{\"ImportExport.ExportApp.Content.Title\" | translate}}</h5>\r\n        <ul>\r\n            <li>{{vm.AppInfo.EntitiesCount}} {{\"ImportExport.ExportApp.Content.EntitiesCount\" | translate}}</li>\r\n            <li>{{vm.AppInfo.LanguagesCount}} {{\"ImportExport.ExportApp.Content.LanguagesCount\" | translate}}</li>\r\n            <li>{{vm.AppInfo.TemplatesCount}} {{\"ImportExport.ExportApp.Content.TemplatesCount\" | translate}} ({{\"ImportExport.ExportApp.Content.TokenTemplates\" | translate}} {{vm.AppInfo.HasTokenTemplates}}, {{\"ImportExport.ExportApp.Content.RazorTemplates\" | translate}} {{vm.AppInfo.HasRazorTemplates}})</li>\r\n            <li>{{vm.AppInfo.TransferableFilesCount}} {{\"ImportExport.ExportApp.Content.TransferableFilesCount\" | translate}}</li>\r\n            <li>{{vm.AppInfo.FilesCount}} {{\"ImportExport.ExportApp.Content.FilesCount\" | translate}}</li>\r\n        </ul>\r\n\r\n        <div>\r\n            <input ng-model=\"vm.IncludeContentGroups\" type=\"checkbox\" ng-disabled=\"vm.ResetAppGuid\" /> {{\"ImportExport.ExportApp.Options.IncludeContentGroups\" | translate}}\r\n        </div>\r\n        <div>\r\n            <input ng-model=\"vm.ResetAppGuid\" type=\"checkbox\" ng-disabled=\"vm.IncludeContentGroups\"/> {{\"ImportExport.ExportApp.Options.ResetAppGuid\" | translate}}\r\n        </div>\r\n    </div>\r\n\r\n    <div class=\"modal-footer\">\r\n        <button type=\"button\" class=\"btn btn-primary pull-left\" ng-click=\"vm.exportApp()\" translate=\"ImportExport.ExportApp.Commands.Export\" ng-disabled=\"vm.IsExporting\"></button>\r\n        <button type=\"button\" class=\"btn btn-default pull-left\" ng-click=\"vm.exportGit()\"\r\n                translate=\"ImportExport.ExportApp.Commands.ExportForVersionControl\"\r\n                ng-disabled=\"vm.IsExporting\"\r\n                xxxng-show=\"vm.debug.on\"\r\n                ></button>\r\n    </div>\r\n    <show-debug-availability class=\"pull-right\"></show-debug-availability>\r\n</div>\r\n");
$templateCache.put("importexport/export-content.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n\r\n    <div class=\"modal-header\">\r\n        <button icon=\"remove\" class=\"btn pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n        <h3 class=\"modal-title\" translate=\"ImportExport.ExportContent.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n         \r\n        <span class=\"btn-group\" ng-if=\"vm.debug.on\">\r\n            <button icon=\"record\" type=\"button\" class=\"btn btn-square\" ng-click=\"vm.changeExportScope()\"></button>\r\n        </span>\r\n\r\n        <div translate=\"ImportExport.ExportContent.Intro\"></div>\r\n        <div translate=\"ImportExport.ExportContent.FurtherHelp\"></div>\r\n\r\n        <h5 translate=\"ImportExport.ExportContent.ContentTypes.Title\"></h5>\r\n        <ul class=\"sc-export-content-list\">\r\n            <li ng-repeat=\"contentType in vm.ContentInfo.ContentTypes\">\r\n                <label ng-class=\"{ \'active\': contentType._export }\"><input type=\"checkbox\" ng-model=\"contentType._export\" /> {{contentType.Name}} ({{contentType.Id}})</label>\r\n                <div ng-if=\"contentType.Templates.length\" class=\"sc-export-content-list-inner\">\r\n                    <h6 translate=\"ImportExport.ExportContent.ContentTypes.Templates\"></h6>\r\n                    <ul>\r\n                        <li ng-repeat=\"template in contentType.Templates\">\r\n                            <label ng-class=\"{ \'active\': template._export }\"><input type=\"checkbox\" ng-model=\"template._export\" /> {{template.Name}} ({{template.Id}})</label>\r\n                        </li>\r\n                    </ul>\r\n                </div>\r\n                <div ng-if=\"contentType.Entities.length\" class=\"sc-export-content-list-inner\">\r\n                    <h6 translate=\"ImportExport.ExportContent.ContentTypes.Entities\"></h6>\r\n                    <ul>\r\n                        <li ng-repeat=\"entity in contentType.Entities\">\r\n                            <label ng-class=\"{ \'active\': entity._export }\"><input type=\"checkbox\" ng-model=\"entity._export\" /> {{entity.Title}} ({{entity.Id}})</label>\r\n                        </li>\r\n                    </ul>\r\n                </div>\r\n\r\n            </li>\r\n        </ul>\r\n\r\n        <h5 translate=\"ImportExport.ExportContent.TemplatesWithoutContentTypes.Title\"></h5>\r\n        <ul class=\"sc-export-content-list\">\r\n            <li ng-repeat=\"template in vm.ContentInfo.TemplatesWithoutContentTypes\">\r\n                <label ng-class=\"{ \'active\': template._export }\"><input type=\"checkbox\" ng-model=\"template._export\" /> {{template.Name}} ({{template.Id}})</label>\r\n            </li>\r\n        </ul>\r\n    </div>\r\n\r\n    <div class=\"modal-footer\">\r\n        <button type=\"button\" class=\"btn btn-primary pull-left\" ng-click=\"vm.exportContent()\" translate=\"ImportExport.ExportContent.Commands.Export\" ng-disabled=\"vm.IsExporting\"></button>\r\n    </div>\r\n\r\n    <show-debug-availability class=\"pull-right\"></show-debug-availability>\r\n</div>\r\n\r\n\r\n\r\n<style>\r\n    .sc-export-content-list { list-style-type: none; margin: 0; padding: 0; border-top: 1px solid #DDD; }\r\n\r\n    .sc-export-content-list-inner { padding: 0 0 0 40px; }\r\n\r\n    .sc-export-content-list > li { margin: 0; padding: 0; border-bottom: 1px solid #DDD; font-weight: bold; }\r\n\r\n    .sc-export-content-list h6 { font-size: inherit; font-weight: bold; margin: 0; padding: 5px; }\r\n\r\n    .sc-export-content-list label { display: block; padding: 10px; margin: 0; font-size: inherit; font-weight: normal; }\r\n\r\n    .sc-export-content-list label:hover { background: #EEE; }\r\n    .sc-export-content-list label.active { background: #E6F7E7; }\r\n\r\n    .sc-export-content-list > li ul { list-style-type: none; margin: 0; padding: 0; }\r\n\r\n    .sc-export-content-list > li li { font-weight: normal; padding: 0; margin: 0; }\r\n</style>");
$templateCache.put("importexport/import-app.html","<div>\r\n    <div class=\"modal-header\">\r\n        <button icon=\"remove\" class=\"btn pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n        <h3 class=\"modal-title\" translate=\"ImportExport.ImportApp.Title\"></h3>\r\n    </div>\r\n\r\n    <div class=\"sxc-spinner\" ng-show=\"vm.IsImporting\" class=\"sxc-import-spinner\"><i class=\"eav-icon-spinner animate-spin\"></i></div>\r\n\r\n    <!-- IMPORT -->\r\n    <div ng-if=\"!vm.ImportResult.Messages\">\r\n\r\n        <div class=\"modal-body\">\r\n\r\n            <div translate=\"ImportExport.ImportApp.Intro\"></div>\r\n            <div translate=\"ImportExport.ImportApp.FurtherHelp\"></div>\r\n            <br />\r\n\r\n            <span class=\"btn btn-default btn-file\">\r\n                <span ng-hide=\"vm.ImportFile.name\">{{\"ImportExport.ImportApp.Commands.SelectFile\" | translate}}</span>\r\n                <span ng-show=\"vm.ImportFile.name\">{{vm.ImportFile.name}}</span>\r\n                <input type=\"file\" sxc-file-input=\"vm.ImportFile\" />\r\n            </span>\r\n            <br />\r\n        </div>\r\n\r\n        <div class=\"modal-footer\">\r\n            <button type=\"button\" class=\"btn btn-primary pull-left\" ng-click=\"vm.importApp()\" ng-disabled=\"!vm.ImportFile.name || vm.IsImporting\" translate=\"ImportExport.ImportApp.Commands.Import\"></button>\r\n        </div>\r\n    </div>\r\n    <!-- END IMPORT -->\r\n\r\n\r\n    <!-- RESULT -->\r\n    <div ng-if=\"vm.ImportResult.Messages\">\r\n        <div class=\"modal-body\">\r\n            <div ng-if=\"vm.ImportResult.Succeeded\" class=\"sxc-message sxc-message-info\">\r\n                {{\"ImportExport.ImportContent.Messages.ImportSucceeded\" | translate}} <!-- (<a ng-click=\"vm.ImportResult._hideSuccessMessages = !vm.ImportResult._hideSuccessMessages\">{{\"ImportExport.ImportContent.Commands.ToggleSuccessMessages\" | translate}}</a>) -->\r\n            </div>\r\n            <div ng-if=\"!vm.ImportResult.Succeeded\" class=\"sxc-message sxc-message-error\">\r\n                {{\"ImportExport.ImportContent.Messages.ImportFailed\" | translate}}\r\n            </div>\r\n            <div ng-repeat=\"message in vm.ImportResult.Messages\" class=\"sxc-message\" ng-class=\"{ \'sxc-message-warning\': message.MessageType == 0, \'sxc-message-success\': message.MessageType == 1, \'sxc-message-error\': message.MessageType == 2, \'sxc-message-success-hidden\': vm.ImportResult._hideSuccessMessages }\">\r\n                {{message.Text}}\r\n            </div>\r\n        </div>\r\n        <div class=\"modal-footer\">\r\n        </div>\r\n    </div>\r\n    <!-- END RESULT -->\r\n\r\n\r\n</div>\r\n\r\n<style>\r\n    .sxc-message { display: block; padding: 18px 18px; margin-bottom: 18px; border: 1px solid rgba(2, 139, 255, 0.2); border-radius: 3px; background: rgba(2,139,255,0.15); max-width: 980px; }\r\n\r\n    .sxc-message-success.sxc-message { background-color: rgba(0,255,0,0.15); border-color: rgba(0, 255, 0, 0.5); }\r\n    \r\n    .sxc-message-success.sxc-message-success-hidden { display: none; }\r\n    \r\n    .sxc-message-warning.sxc-message { background-color: rgba(255,255,0,0.15); border-color: #CDB21F; }\r\n\r\n    .sxc-message-error.sxc-message { background-color: rgba(255,0,0,0.15); border-color: rgba(255, 0, 0, 0.2); }\r\n</style>");
$templateCache.put("importexport/import-content.html","<div>\r\n    <div class=\"modal-header\">\r\n        <button icon=\"remove\" class=\"btn pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n        <h3 class=\"modal-title\" translate=\"ImportExport.ImportContent.Title\"></h3>\r\n    </div>\r\n\r\n    <div ng-show=\"true || vm.IsImporting\" class=\"sxc-import-spinner\"><i class=\"eav-icon-spinner animate-spin\"></i></div>\r\n\r\n    <!-- IMPORT -->\r\n    <div ng-if=\"!vm.ImportResult.Messages\">\r\n\r\n        <div class=\"modal-body\">\r\n            <div translate=\"ImportExport.ImportContent.Intro\"></div>\r\n            <div translate=\"ImportExport.ImportContent.FurtherHelp\"></div>\r\n            <br />\r\n            <span class=\"btn btn-default btn-file\">\r\n                <span ng-hide=\"vm.ImportFile.name\">{{\"ImportExport.ImportContent.Commands.SelectFile\" | translate}}</span>\r\n                <span ng-show=\"vm.ImportFile.name\">{{vm.ImportFile.name}}</span>\r\n                <input type=\"file\" sxc-file-input=\"vm.ImportFile\" />\r\n            </span>\r\n            <br />\r\n        </div>\r\n\r\n        <div class=\"modal-footer\">\r\n            <button type=\"button\" class=\"btn btn-primary pull-left\" ng-click=\"vm.importContent()\" ng-disabled=\"!vm.ImportFile.name || vm.IsImporting\" translate=\"ImportExport.ImportContent.Commands.Import\"></button>\r\n        </div>\r\n    </div>\r\n    <!-- END IMPORT -->\r\n\r\n    <!-- RESULT -->\r\n    <div ng-if=\"vm.ImportResult.Messages\">\r\n        <div class=\"modal-body\">\r\n            <div ng-if=\"vm.ImportResult.Succeeded\" class=\"sxc-message sxc-message-info\">\r\n                {{\"ImportExport.ImportContent.Messages.ImportSucceeded\" | translate}} (<a ng-click=\"vm.ImportResult._hideSuccessMessages = !vm.ImportResult._hideSuccessMessages\">{{\"ImportExport.ImportContent.Commands.ToggleSuccessMessages\" | translate}}</a>)\r\n            </div>\r\n            <div ng-if=\"!vm.ImportResult.Succeeded\" class=\"sxc-message sxc-message-error\">\r\n                {{\"ImportExport.ImportContent.Messages.ImportFailed\" | translate}}\r\n            </div>\r\n            <div ng-repeat=\"message in vm.ImportResult.Messages\" class=\"sxc-message\" ng-class=\"{ \'sxc-message-warning\': message.MessageType == 0, \'sxc-message-success\': message.MessageType == 1, \'sxc-message-error\': message.MessageType == 2, \'sxc-message-success-hidden\': vm.ImportResult._hideSuccessMessages }\">\r\n                {{message.Message}}\r\n            </div>\r\n        </div>\r\n        <div class=\"modal-footer\">\r\n        </div>\r\n    </div>\r\n    <!-- END RESULT -->\r\n</div>");
$templateCache.put("importexport/intro.html","<div class=\"modal-body\">\r\n    <div>\r\n        <div class=\"pull-left btn-group-vertical\">\r\n            <button uib-tooltip=\"{{\'AppConfig.Export.Button\' | translate}}\" ng-click=\"vm.exportAll()\" class=\"btn btn-square btn-primary\" type=\"button\">\r\n                <i icon=\"export\"></i>\r\n            </button>\r\n        </div>\r\n        <div style=\"margin-left: 50px\">\r\n            <h4 class=\"modal-title\" translate=\"AppConfig.Export.Title\"></h4>\r\n            <div translate=\"AppConfig.Export.Intro\"></div>\r\n        </div>\r\n    </div>\r\n\r\n    <br />\r\n\r\n    <div>\r\n        <div class=\"pull-left btn-group-vertical\">\r\n            <button uib-tooltip=\"{{\'ImportExport.Buttons.Export\' | translate}}\" ng-click=\"vm.export()\" class=\"btn btn-square btn-primary\" type=\"button\">\r\n                <i icon=\"export\"></i>\r\n            </button>\r\n\r\n            <button uib-tooltip=\"{{\'ImportExport.Buttons.Import\' | translate}}\" ng-click=\"vm.import()\" class=\"btn btn-square btn-primary\" type=\"button\">\r\n                <i icon=\"import\"></i>\r\n            </button>\r\n        </div>\r\n        <div style=\"margin-left: 50px\">\r\n            <h4 class=\"modal-title\" translate=\"ImportExport.Title\"></h4>\r\n            <div translate=\"ImportExport.Intro\"></div>\r\n        </div>\r\n    </div>\r\n\r\n    <br />\r\n\r\n</div>");
$templateCache.put("language-settings/languages.html","<div>\r\n    <div class=\"modal-header\">\r\n        <h4 translate=\"Language.Title\"></h4>        \r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <div translate=\"Language.Intro\"></div>\r\n\r\n        <table class=\"table table-hover\">\r\n            <thead>\r\n                <tr>\r\n                    <th translate=\"Language.Table.Code\"></th>\r\n                    <th translate=\"Language.Table.Culture\"></th>\r\n                    <th translate=\"Language.Table.Status\"></th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                <tr ng-repeat=\"item in vm.items | orderBy:[\'ContentType.Name\',\'Name\']\" class=\"clickable-row\" ng-click=\"vm.toggle(item)\">\r\n                    <td class=\"clickable\">{{item.Code}}</td>\r\n                    <td class=\"clickable\">{{item.Culture}}</span></td>\r\n                    <td class=\"clickable text-nowrap\">\r\n                        <span ng-click=\"vm.save(item)\" stop-event=\"click\">\r\n                            <switch ng-model=\"item.IsEnabled\"></switch>\r\n                        </span>\r\n                    </td>\r\n                </tr>\r\n                <tr ng-if=\"!vm.items.length\">\r\n                    <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n        &nbsp;\r\n    </div>\r\n</div>");
$templateCache.put("manage-content-list/manage-content-list.html","<div class=\"modal-header\">\r\n    <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\"><i icon=\"remove\"></i></button>\r\n    <h3 class=\"modal-title\" translate=\"ManageContentList.Title\"></h3>\r\n</div>\r\n<div>\r\n    <div class=\"modal-body\">\r\n        <div>\r\n            <p translate=\"ManageContentList.HeaderIntro\"></p>\r\n            <div>\r\n                <a ng-if=\"vm.header.Type\" ng-click=\"vm.editHeader()\"> {{ vm.header.Title }} <i icon=\"pencil\" class=\"pull-right\"></i></a>\r\n                <span ng-if=\"!vm.header.Type\" translate=\"ManageContentList.NoHeaderInThisList\"></span>\r\n            </div>\r\n            <br/>\r\n        </div>\r\n        <div>\r\n            <p translate=\"ManageContentList.Intro\"></p>\r\n            <div ui-tree=\"options\" data-empty-placeholder-enabled=\"false\">\r\n                <ol ui-tree-nodes ng-model=\"vm.items\">\r\n                    <li ng-repeat=\"item in vm.items\" ui-tree-node class=\"eav-entityselect-item\" style=\"width: 100%\">\r\n                        <div ui-tree-handle>\r\n                            <i icon=\"move\" title=\"{{ \'FieldType.Entity.DragMove\' | translate }}\" class=\"pull-left eav-entityselect-sort\"></i>\r\n                            <span>&nbsp;{{item.Title}} ({{item.Id}})</span>\r\n                            <span class=\"eav-entityselect-item-actions\">\r\n                                <span ng-if=\"item.Id !== 0\" data-nodrag title=\"{{ \'FieldType.Entity.Edit\' | translate }}\" ng-click=\"vm.edit(item.Id)\">\r\n                                    <i icon=\"pencil\"></i>\r\n                                </span>\r\n                            </span>\r\n                        </div>\r\n                    </li>\r\n                </ol>\r\n\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"modal-footer\">\r\n    <button class=\"btn btn-primary btn-square btn-lg pull-left\" type=\"button\" ng-click=\"vm.ok()\"><i icon=\"ok\"></i></button>\r\n</div>");
$templateCache.put("replace-content/replace-content.html","<div class=\"modal-header\">\r\n    <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\"><i icon=\"remove\"></i></button>\r\n    <h3 class=\"modal-title\" translate=\"ReplaceContent.Title\"></h3>\r\n</div>\r\n<div>\r\n    <div class=\"modal-body\">\r\n        <p translate=\"ReplaceContent.Intro\"></p>\r\n        <div translate=\"ReplaceContent.ChooseItem\"></div>\r\n        <div>\r\n            <select class=\"input-lg\"\r\n                    ng-model=\"vm.item.id\"\r\n                    ng-options=\"vm.convertToInt(key) as ((value || \'[?]\') + \' (\' + key + \')\') for (key,value) in vm.options\"\r\n                    style=\"max-width: 100%\"></select>\r\n            &nbsp;<button type=\"button\" class=\"btn btn-lg\" ng-click=\"vm.copySelected()\">\r\n                <i icon=\"duplicate\"></i>\r\n            </button>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"modal-footer\">\r\n    <button class=\"btn btn-primary btn-square btn-lg pull-left\" type=\"button\" ng-click=\"vm.ok()\"><i icon=\"ok\"></i></button>\r\n</div>");
$templateCache.put("templates/templates.html","\r\n\r\n<div class=\"modal-body\">\r\n    <button icon=\"plus\" type=\"button\" class=\"btn btn-primary btn-square\" ng-click=\"vm.add()\"> </button>\r\n    <span class=\"btn-group\" ng-if=\"vm.debug.on\">\r\n        <button icon=\"repeat\" type=\"button\" class=\"btn btn-square\" ng-click=\"vm.refresh()\"></button>\r\n    </span>\r\n    <table class=\"table table-hover\">\r\n        <thead>\r\n        <tr>\r\n            <th translate=\"Templates.Table.TName\"></th>\r\n            <th translate=\"Templates.Table.TPath\"></th>\r\n            <th translate=\"Templates.Table.CType\"></th>\r\n            <th translate=\"Templates.Table.DemoC\"></th>\r\n            <th>\r\n                <span uib-tooltip=\"{{\'Templates.Table.Show\' | translate}}\"><i icon=\"eye-open\"></i></span>\r\n            </th>\r\n            <th translate=\"Templates.Table.UrlKey\"></th>\r\n            <th translate=\"Templates.Table.Actions\"></th>\r\n        </tr>\r\n        </thead>\r\n        <tbody>\r\n        <tr ng-repeat=\"item in vm.items | orderBy:[\'ContentType.Name\',\'Name\']\" class=\"clickable-row\" ng-click=\"vm.edit(item)\">\r\n            <td class=\"clickable\">{{item.Name}}</td>\r\n            <td class=\"clickable\"><span uib-tooltip=\"{{item.TemplatePath}}\">...{{item.TemplatePath.split(\"/\").pop()}}</span></td>\r\n            <td class=\"clickable\">\r\n                <span uib-tooltip=\"\r\nCont: {{item.ContentType.Name}} ({{item.ContentType.Id}})\r\nPres: {{item.PresentationType.Name}} ({{item.PresentationType.Id}})\r\nListC: {{item.ListContentType.Name}} ({{item.ListContentType.Id}})\r\nListP: {{item.ListPresentationType.Name}} ({{item.ListPresentationType.Id}})\r\n\">{{item.ContentType.Name}}</span>\r\n            </td>\r\n            <td class=\"clickable\">\r\n                <span uib-tooltip=\"\r\nDemo: {{item.ContentType.DemoTitle}} ({{item.ContentType.DemoId}})\r\nPres: {{item.PresentationType.DemoTitle}} ({{item.PresentationType.DemoId}})\r\nListC: {{item.ListContentType.DemoTitle}} ({{item.ListContentType.DemoId}})\r\nListP: {{item.ListPresentationType.DemoTitle}} ({{item.ListPresentationType.DemoId}})\r\n\">{{item.ContentType.DemoId}}</span>\r\n            </td>\r\n            <td>\r\n                <span icon=\"{{ item.IsHidden ? \'close\' : \'eye-open\'}}\"></span>\r\n            </td>\r\n            <td class=\"clickable\"><span uib-tooltip=\"{{item.ViewNameInUrl}}\">{{item.ViewNameInUrl}}</span></td>\r\n            <td class=\"text-nowrap\" stop-event=\"click\">\r\n                <button type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.permissions(item)\">\r\n                    <i icon=\"user\"></i>\r\n                </button>\r\n\r\n                <button type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.tryToDelete(item)\">\r\n                    <span icon=\"remove\"></span>\r\n                </button>\r\n            </td>\r\n        </tr>\r\n        <tr ng-if=\"!vm.items.length\">\r\n            <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n        </tr>\r\n        </tbody>\r\n    </table>\r\n\r\n    <div class=\"\" translate=\"Templates.InfoHideAdvanced\"></div>\r\n</div>");
$templateCache.put("web-api/web-api.html","<div class=\"modal-header\">\r\n    <h3 class=\"modal-title\" translate=\"WebApi.Title\"></h3>\r\n</div>\r\n\r\n<div class=\"modal-body\">\r\n    <p translate=\"WebApi.Intro\"></p>\r\n    <button icon=\"plus\" type=\"button\" class=\"btn btn-square\" ng-click=\"vm.add()\"> </button>\r\n    <button icon=\"repeat\" type=\"button\" class=\"btn btn-square\" ng-click=\"vm.refresh()\"></button>\r\n\r\n    <table class=\"table table-hover\">\r\n        <thead>\r\n            <tr>\r\n                <th translate=\"WebApi.ListTitle\"></th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n            <tr ng-repeat=\"item in vm.items | orderBy:[\'ContentType.Name\',\'Name\']\">\r\n                <td><span uib-tooltip=\"{{item.TemplatePath}}\">{{item}}</span></td>\r\n            </tr>\r\n            <tr ng-if=\"!vm.items.length\">\r\n                <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n    <p translate=\"WebApi.QuickStart\"></p>\r\n\r\n</div>\r\n\r\n");}]);
(function () { 

    TemplateListController.$inject = ["templatesSvc", "eavAdminDialogs", "eavConfig", "appId", "debugState", "$translate", "$uibModalInstance"];
    angular.module("TemplatesApp", [
        "SxcServices",
        "EavConfiguration",
        "EavAdminUi",
        "EavServices",
        "EavDirectives"
    ])
        .controller("TemplateList", TemplateListController)
        ;

    /*@ngInject*/
    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, debugState, $translate, $uibModalInstance) {
        var vm = this;
        vm.debug = debugState;

        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };
        


        vm.add = function addNew() {
            eavAdminDialogs.openItemNew("2SexyContent-Template", svc.liveListReload);
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
            $uibModalInstance.dismiss("cancel");
        };
    }

} ());
(function () { 

    WebApiMainController.$inject = ["appId", "webApiSvc", "eavAdminDialogs", "$uibModalInstance", "$translate"];
    angular.module("WebApiApp", [
        "SxcServices",
        //"EavConfiguration",
        "EavAdminUi",
        "EavServices",
        "EavDirectives"
    ])
        .controller("WebApiMain", WebApiMainController)
        ;

    /*@ngInject*/
    function WebApiMainController(appId, webApiSvc, eavAdminDialogs, $uibModalInstance, $translate) {
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
            $uibModalInstance.dismiss("cancel");
        };
    }

} ());