(function () {
    angular.module("ContentExportApp", [
        "EavAdminUi",
        "EavDirectives",
        "EavConfiguration",
        "EavServices",
        "ContentHelperFilters",
        "ContentFormlyTypes"
    ]);
}());
(function () {

    angular.module("ContentExportApp")
        .controller("ContentExport", contentExportController);

    function contentExportController(appId, contentType, contentExportService, eavAdminDialogs, eavConfig, languages, $modalInstance, $filter) {
        var translate = $filter("translate");


        var vm = this;

        vm.formValues = {};

        vm.formFields = [{
            // Content type
            key: "AppId",
            type: "hidden",
            defaultValue: appId
        }, {
            // Default / fallback language
            key: "DefaultLanguage",
            type: "hidden",
            defaultValue: $filter("isoLangCode")(languages.defaultLanguage)
        }, {
            // Content type
            key: "ContentType",
            type: "hidden",
            defaultValue: contentType
        }, {
            key: "Language",
            type: "select",
            expressionProperties: {
                "templateOptions.label": "'Content.Export.Fields.Language.Label' | translate",
                "templateOptions.options": function () {
                    var options = [{
                        "name": translate("Content.Export.Fields.Language.Options.All"),
                        "value": ""
                    }];
                    angular.forEach(languages.languages, function (lang) {
                        var langCode = $filter("isoLangCode")(lang.key);
                        options.push({ "name": langCode, "value": langCode });
                    });
                    return options;
                }
            },
            defaultValue: ""
        }, {
            key: "RecordExport",
            type: "radio",
            expressionProperties: {
                "templateOptions.label": "'Content.Export.Fields.RecordExport.Label' | translate",
                "templateOptions.options": function () {
                    return [{
                        "name": translate("Content.Export.Fields.RecordExport.Options.Blank"),
                        "value": "Blank"
                    }, {
                        "name": translate("Content.Export.Fields.RecordExport.Options.All"),
                        "value": "All"
                    }];
                }
            },
            defaultValue: "All"
        }, {
            // Language references
            key: "LanguageReferences",
            type: "radio",
            expressionProperties: {
                "templateOptions.label": "'Content.Export.Fields.LanguageReferences.Label' | translate",
                "templateOptions.disabled": function () {
                    return vm.formValues.RecordExport == "Blank";
                },
                "templateOptions.options": function () {
                    return [{
                        "name": translate("Content.Export.Fields.LanguageReferences.Options.Link"),
                        "value": "Link"
                    }, {
                        "name": translate("Content.Export.Fields.LanguageReferences.Options.Resolve"),
                        "value": "Resolve"
                    }];
                }
            },
            defaultValue: "Link"
        }, {
            // File / page references
            key: "ResourcesReferences",
            type: "radio",
            expressionProperties: {
                "templateOptions.label": "'Content.Export.Fields.ResourcesReferences.Label' | translate",
                "templateOptions.disabled": function () {
                    return vm.formValues.RecordExport == "Blank";
                },
                "templateOptions.options": function () {
                    return [{
                        "name": translate("Content.Export.Fields.ResourcesReferences.Options.Link"),
                        "value": "Link"
                    }, {
                        "name": translate("Content.Export.Fields.ResourcesReferences.Options.Resolve"),
                        "value": "Resolve"
                    }];
                }
            },
            defaultValue: "Link"
        }];


        vm.exportContent = function exportContent() {
            contentExportService.exportContent(vm.formValues);
        };

        vm.close = function close() {
            $modalInstance.dismiss("cancel");
        };
    }
    contentExportController.$inject = ["appId", "contentType", "contentExportService", "eavAdminDialogs", "eavConfig", "languages", "$modalInstance", "$filter"];
}());
(function () {

    angular.module("ContentExportApp")
         .factory("contentExportService", contentExportService);


    function contentExportService() {
        var srvc = {
            exportContent: exportContent,
        };
        return srvc;

        function exportContent(args) {
            window.open("/api/eav/ContentExport/ExportContent?appId=" + args.AppId + "&language=" + args.Language + "&defaultLanguage=" + args.DefaultLanguage + "&contentType=" + args.ContentType + "&recordExport=" + args.RecordExport + "&resourcesReferences=" + args.ResourcesReferences + "&languageReferences=" + args.LanguageReferences, "_self", "");
        }
    }
}());
(function () {
    angular.module("ContentFormlyTypes", [
        "naif.base64",
        "formly",
        "formlyBootstrap",
        "ui.bootstrap"
    ]);
}());
(function () {

    angular.module("ContentFormlyTypes")

        .config(["formlyConfigProvider", function (formlyConfigProvider) {
            var formly = formlyConfigProvider;

            formly.setType({
                name: "file",
                template: "<span class='btn btn-default btn-file'><span class='glyphicon glyphicon-open'></span><input type='file' ng-model='model[options.key]' base-sixty-four-input /></span> <span ng-if='model[options.key]'>{{model[options.key].filename}}</span>",
                wrapper: ["bootstrapLabel", "bootstrapHasError"]
            });

            formly.setType({
                name: "hidden",
                template: "<input style='display:none' ng-model='model[options.key]' />",
                wrapper: ["bootstrapLabel", "bootstrapHasError"]
            });
    }]);
}());
(function () {
    angular.module("ContentHelperFilters", []);


    angular.module("ContentHelperFilters").filter("isoLangCode", function () {
        return function (str) {
            if (str.length != 5)
                return str;
            return str.substring(0, 2).toLowerCase() + "-" + str.substring(3, 5).toUpperCase();
        };
    });
}());
(function () {
    angular.module("ContentImportApp", [
        "EavAdminUi",
        "EavDirectives",
        "EavConfiguration",
        "EavServices",
        "ContentHelperFilters",
        "ContentFormlyTypes"
    ]);
}());
(function () {

    angular.module("ContentImportApp")
        .controller("ContentImport", contentImportController);

    function contentImportController(appId, contentType, contentImportService, eavAdminDialogs, eavConfig, languages, debugState, $modalInstance, $filter) {
        var translate = $filter("translate");

        var vm = this;
        vm.debug = debugState;

        vm.formValues = {};

        vm.formFields = [{
            // Content type
            key: "AppId",
            type: "hidden",
            defaultValue: appId
        }, {
            // Default / fallback language
            key: "DefaultLanguage",
            type: "hidden",
            defaultValue: $filter("isoLangCode")(languages.defaultLanguage)
        }, {
            // Content type
            key: "ContentType",
            type: "hidden",
            defaultValue: contentType
        }, {
            // File
            key: "File",
            type: "file",
            templateOptions: {
                required: true
            },
            expressionProperties: {
                "templateOptions.label": "'Content.Import.Fields.File.Label' | translate"
            }
        }, {
            // File / page references
            key: "ResourcesReferences",
            type: "radio",
            expressionProperties: {
                "templateOptions.label": "'Content.Import.Fields.ResourcesReferences.Label' | translate",
                "templateOptions.options": function () {
                    return [{
                        "name": translate("Content.Import.Fields.ResourcesReferences.Options.Keep"),
                        "value": "Keep"
                    }, {
                        "name": translate("Content.Import.Fields.ResourcesReferences.Options.Resolve"),
                        "value": "Resolve"
                    }];
                }
            },
            defaultValue: "Keep"
        }, {
            // Clear entities
            key: "ClearEntities",
            type: "radio",
            expressionProperties: {
                "templateOptions.label": "'Content.Import.Fields.ClearEntities.Label' | translate",
                "templateOptions.options": function () {
                    return [{
                        "name": translate("Content.Import.Fields.ClearEntities.Options.None"),
                        "value": "None"
                    }, {
                        "name": translate("Content.Import.Fields.ClearEntities.Options.All"),
                        "value": "All"
                    }];
                }
            },
            defaultValue: "None"
        }];

        vm.viewStates = {
            "Waiting":   0,
            "Default":   1,
            "Evaluated": 2,
            "Imported":  3
        };

        vm.viewStateSelected = vm.viewStates.Default;


        vm.evaluationResult = { };

        vm.importResult = { };


        vm.evaluateContent = function evaluateContent() {
            vm.viewStateSelected = vm.viewStates.Waiting;
            return contentImportService.evaluateContent(vm.formValues).then(function (result) {
                vm.evaluationResult = result.data;
                vm.viewStateSelected = vm.viewStates.Evaluated;
            });
        };

        vm.importContent = function importContent() {
            vm.viewStateSelected = vm.viewStates.Waiting;
            return contentImportService.importContent(vm.formValues).then(function (result) {
                vm.importResult = result.data;
                vm.viewStateSelected = vm.viewStates.Imported;
            });
        };

        vm.reset = function reset() {
            vm.formValues = { };
            vm.evaluationResult = { };
            vm.importResult = { };
        };

        vm.back = function back() {
            vm.viewStateSelected = vm.viewStates.Default;
        };

        vm.close = function close() {
            vm.viewStateSelected = vm.viewStates.Default;
            $modalInstance.dismiss("cancel");
        };
    }
    contentImportController.$inject = ["appId", "contentType", "contentImportService", "eavAdminDialogs", "eavConfig", "languages", "debugState", "$modalInstance", "$filter"];
}());
(function () {

    angular.module("ContentImportApp")
         .factory("contentImportService", contentImportService);


    function contentImportService($http) {
        var srvc = {
            evaluateContent: evaluateContent,
            importContent: importContent
        };
        return srvc;

        function evaluateContent(args) {
            return $http.post("eav/ContentImport/EvaluateContent", { AppId: args.AppId, DefaultLanguage: args.DefaultLanguage, ContentType: args.ContentType, ContentBase64: args.File.base64, ResourcesReferences: args.ResourcesReferences, ClearEntities: args.ClearEntities });
        }

        function importContent(args) {
            return $http.post("eav/ContentImport/ImportContent", { AppId: args.AppId, DefaultLanguage: args.DefaultLanguage, ContentType: args.ContentType, ContentBase64: args.File.base64, ResourcesReferences: args.ResourcesReferences, ClearEntities: args.ClearEntities });
        }
    }
    contentImportService.$inject = ["$http"];
}());
(function () { 

    angular.module("ContentEditApp", [
        "EavServices",
        "EavAdminUi"
    ])
        .controller("EditContentItem", EditContentItemController)
        ;

    function EditContentItemController(mode, entityId, contentType, eavAdminDialogs, $modalInstance) { //}, contentTypeId, eavAdminDialogs) {
        var vm = this;
        vm.mode = mode;
        vm.entityId = entityId;
        vm.contentType = contentType;
        vm.TestMessage = "Test message the controller is binding correctly...";

        vm.history = function history() {
            return eavAdminDialogs.openItemHistory(vm.entityId);
        };

        vm.close = function () { $modalInstance.dismiss("cancel"); };
    }
    EditContentItemController.$inject = ["mode", "entityId", "contentType", "eavAdminDialogs", "$modalInstance"];

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ContentItemsApp", [
        "EavConfiguration",
        "EavAdminUi",
        "EavServices"
    ])
        .controller("ContentItemsList", ContentItemsListController)
    ;

    function ContentItemsListController(contentItemsSvc, eavConfig, appId, contentType, /*contentTypeId, */ eavAdminDialogs, $modalInstance) {
        var vm = this;
        var svc = contentItemsSvc(appId, contentType); //, contentTypeId);

        vm.add = function add() {
            eavAdminDialogs.openItemNew(contentType, svc.liveListReload);
        };

        vm.edit = function(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.refresh = svc.liveListReload;

        vm.items = svc.liveList();

        vm.dynamicColumns = [];
        svc.getColumns().then(function (result) {
            var cols = result.data;
            for (var c = 0; c < cols.length; c++) {
                if (!cols[c].IsTitle)
                    vm.dynamicColumns.push(cols[c]);
            }
        });

        vm.tryToDelete = function tryToDelete(item) {
            if (confirm("Delete '" + "title-unknown-yet" + "' (" + item.RepositoryId + ") ?"))
                svc.delete(item.RepositoryId);
        };

        vm.openDuplicate = function openDuplicate(item) {
            var items = [
                {
                    ContentTypeName: contentType,
                    DuplicateEntity: item.Id
                }
            ];
            eavAdminDialogs.openEditItems(items, svc.liveListReload);

        };

        vm.close = function () { $modalInstance.dismiss("cancel"); };

    }
    ContentItemsListController.$inject = ["contentItemsSvc", "eavConfig", "appId", "contentType", "eavAdminDialogs", "$modalInstance"];

} ());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("HistoryApp", [
        "EavServices",
        "EavConfiguration",
        "eavTemplates",
    ])
        .controller("History", HistoryController)
        .controller("HistoryDetails", HistoryDetailsController)
        ;

    function HistoryController(appId, entityId, historySvc, $modalInstance, $modal) {
        var vm = this;
        var svc = historySvc(appId, entityId);
        vm.entityId = entityId;
        vm.items = svc.liveList();

        vm.close = function () { $modalInstance.dismiss("cancel"); };

        vm.details = function(item) {
            $modal.open({
                animation: true,
                templateUrl: "content-items/history-details.html",
                controller: "HistoryDetails",
                controllerAs: "vm",
                resolve: {
                    changeId: function() { return item.ChangeId; },
                    dataSvc: function() { return svc; }
                }
            });
        };
    }
    HistoryController.$inject = ["appId", "entityId", "historySvc", "$modalInstance", "$modal"];

    function HistoryDetailsController(changeId, dataSvc, $modalInstance) {
        var vm = this;
        alert("not implemented yet");
        var svc = dataSvc;

        svc.getVersionDetails(changeId).then(function(result) {
            alert(result.data);
            vm.items = result.data;
        });
        // vm.items = svc.liveList();

        vm.close = function () { $modalInstance.dismiss("cancel"); };
    }
    HistoryDetailsController.$inject = ["changeId", "dataSvc", "$modalInstance"];
} ());
// This is the main declaration for the app ContentTypesApp
(function () {

    angular.module("ContentTypesApp", [
        "EavServices",
        "EavAdminUi",
        "EavDirectives"
        ])
        .constant("license", {
            createdBy: "2sic internet solutions",
            license: "MIT"
            })             
    ;
}());
(function () { 

    angular.module("ContentTypesApp")
        .controller("List", contentTypeListController)
        .controller("Edit", contentTypeEditController)
    ;


    /// Manage the list of content-types
    function contentTypeListController(contentTypeSvc, eavAdminDialogs, appId, debugState, $translate) {
        var vm = this;
        var svc = contentTypeSvc(appId);

        vm.debug = debugState;

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.tryToDelete = function tryToDelete(item) {
            $translate("General.Questions.Delete", { target: "'" + item.Name + "' (" + item.Id + ")"}).then(function(msg) {
                if(confirm(msg))
                    svc.delete(item);
            });
        };

        vm.edit = function edit(item) {
            if (item === undefined)
                item = svc.newItem();

            eavAdminDialogs.openContentTypeEdit(item, vm.refresh);
        };

        vm.editFields = function editFields(item) {
            eavAdminDialogs.openContentTypeFields(item, vm.refresh);
        };

        vm.editItems = function editItems(item) {
            eavAdminDialogs.openContentItems(svc.appId, item.StaticName, item.Id, vm.refresh);
        };


        vm.liveEval = function admin() {
            $translate("General.Questions.SystemInput").then(function (msg) {
                var inp = prompt(msg);
                if(inp)
                    eval(inp); // jshint ignore:line
            });
        };

        // this is to change the scope of the items being shown
        vm.changeScope = function admin() {
            $translate("ContentTypes.Buttons.ChangeScopeQuestion").then(function (msg) {
                var inp = prompt(msg);
                if (inp)
                    svc.setScope(inp);
            });
        };

        vm.isGuid = function isGuid(txtToTest) {
            var patt = new RegExp(/[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i);
            return patt.test(txtToTest); // note: can't use the txtToTest.match because it causes infinite digest cycles
        };

        vm.permissions = function permissions(item) {
            return eavAdminDialogs.openPermissionsForGuid(svc.appId, item.StaticName, vm.refresh);
        };

        vm.openExport = function openExport(item) {
            return eavAdminDialogs.openContentExport(svc.appId, item.StaticName, vm.refresh);
        };

        vm.openImport = function openImport(item) {
            return eavAdminDialogs.openContentImport(svc.appId, item.StaticName, vm.refresh);
        };

    }
    contentTypeListController.$inject = ["contentTypeSvc", "eavAdminDialogs", "appId", "debugState", "$translate"];

    /// Edit or add a content-type
    /// Note that the svc can also be null if you don't already have it, the system will then create its own
    function contentTypeEditController(appId, item, contentTypeSvc, translate, $modalInstance) {
        var vm = this;
        var svc = contentTypeSvc(appId);

        vm.item = item;

        vm.ok = function () {
            svc.save(item).then(function() {
                $modalInstance.close(vm.item);              
            });
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    contentTypeEditController.$inject = ["appId", "item", "contentTypeSvc", "translate", "$modalInstance"];

}());
/*jshint laxbreak:true */
(function () {
    angular.module("ContentTypesApp")
        .controller("FieldList", contentTypeFieldListController)
        .controller("FieldsAdd", contentTypeFieldAddController)
    ;

    /// The controller to manage the fields-list
    function contentTypeFieldListController(appId, contentTypeFieldSvc, contentType, $modalInstance, $modal, eavAdminDialogs, $translate, eavConfig) {
        var vm = this;
        var svc = contentTypeFieldSvc(appId, contentType);

        // to close this dialog
        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };

        vm.items = svc.liveList();

        // Open an add-dialog, and add them if the dialog is closed
        vm.add = function add() {
            $modal.open({
                animation: true,
                templateUrl: "content-types/content-types-field-edit.html",
                controller: "FieldsAdd",
                controllerAs: "vm",
                resolve: {
                    svc: function() { return svc; }
                }
            });
        };


        // Actions like moveUp, Down, Delete, Title
        vm.moveUp = svc.moveUp;
        vm.moveDown = svc.moveDown;
        vm.setTitle = svc.setTitle;

        vm.tryToDelete = function tryToDelete(item) {
            if (item.IsTitle) 
                return $translate(["General.Messages.CantDelete", "General.Terms.Title"], {target:"{0}"}).then(function (translations) {
                    alert(translations["General.Messages.CantDelete"].replace("{0}", translations["General.Terms.Title"]));
                });

            $translate("General.Questions.Delete", { target: "'" + item.StaticName + "' (" + item.Id + ")" }).then(function(msg) {
                if (confirm(msg))
                    svc.delete(item);
            });
        };

        // Edit / Add metadata to a specific fields
        vm.createOrEditMetadata = function createOrEditMetadata(item, metadataType) {
            // assemble an array of 2 items for editing
            var items = [vm.createItemDefinition(item, "All"), vm.createItemDefinition(item, metadataType)];
            eavAdminDialogs.openEditItems(items, svc.liveListReload);
        };

        vm.createItemDefinition = function createItemDefinition(item, metadataType) {
            var title = metadataType == "All" ? "General" : metadataType;
            return item.Metadata[metadataType] !== undefined
                ? { EntityId: item.Metadata[metadataType].Id, Title: title }  // if defined, return the entity-number to edit
                : {
                    ContentTypeName: "@" + metadataType,        // otherwise the content type for new-assegnment
                    Metadata: {
                        Key: item.Id,
                        KeyType: "number",
                        TargetType: eavConfig.metadataOfAttribute
                    },
                    Title: title,
                    Prefill: { Name: item.StaticName }
                };      
        };
    }
    contentTypeFieldListController.$inject = ["appId", "contentTypeFieldSvc", "contentType", "$modalInstance", "$modal", "eavAdminDialogs", "$translate", "eavConfig"];

    /// This is the main controller for adding a field
    /// Add is a standalone dialog, showing 10 lines for new field names / types
    function contentTypeFieldAddController(svc, $modalInstance) {
        var vm = this;

        // prepare empty array of up to 10 new items to be added
        var nw = svc.newItem;
        vm.items = [nw(), nw(), nw(), nw(), nw(), nw(), nw(), nw(), nw(), nw()];

        vm.item = svc.newItem();
        vm.types = svc.types.liveList();

        vm.ok = function () {
            var items = vm.items;
            var newList = [];
            for (var c = 0; c < items.length; c++)
                if (items[c].StaticName)
                    newList.push(items[c]);
            svc.addMany(newList, 0);
            $modalInstance.close();
        };

        vm.close = function() { $modalInstance.dismiss("cancel"); };
    }
    contentTypeFieldAddController.$inject = ["svc", "$modalInstance"];
}());

angular.module("EavDirectives", [])

    .directive("icon", function () {
        return {
            restrict: "A",
            replace: false,
            transclude: false,
            link: function postLink(scope, elem, attrs) {
                var icn = attrs.icon;
                elem.addClass("glyphicon glyphicon-" + icn);
            }
        };
    })
;
angular.module('eavTemplates',[]).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('content-import-export/content-export.html',
    "<div class=modal-header><button class=\"btn pull-right\" type=button icon=remove ng-click=vm.close()></button><h3 class=modal-title translate=Content.Export.Title>cc</h3></div><div class=modal-body><div translate=Content.Export.Help></div><formly-form form=vm.form model=vm.formValues fields=vm.formFields></formly-form></div><div class=modal-footer><button type=button class=\"btn btn-default pull-left\" ng-click=vm.exportContent() translate=Content.Export.Commands.Export></button></div>"
  );


  $templateCache.put('content-import-export/content-import.html',
    "<div ng-click=vm.debug.autoEnableAsNeeded($event)><div class=modal-header><button class=\"btn pull-right\" type=button icon=remove ng-click=vm.close()></button><h3 class=modal-title><span translate=Content.Import.Title></span> <span ng-show=\"vm.viewStateSelected > 0\" translate=Content.Import.TitleSteps translate-values=\"{step: vm.viewStateSelected}\"></span></h3></div><div ng-switch=vm.viewStateSelected><div ng-switch-when=1><div class=modal-body><div translate=Content.Import.Help></div><formly-form form=vm.form model=vm.formValues fields=vm.formFields></formly-form><div class=text-warning translate=Content.Import.Messages.BackupContentBefore></div></div><div class=modal-footer><button type=button class=\"btn btn-default pull-left\" ng-click=vm.evaluateContent() ng-disabled=\"!vm.formValues.File || !vm.formValues.File.filename\" translate=Content.Import.Commands.Preview></button></div></div><div ng-switch-when=0><div class=modal-body>{{'Content.Import.Messages.WaitingForResponse' | translate}}</div></div><div ng-switch-when=2><div class=modal-body><div ng-if=vm.evaluationResult.Succeeded><h4 translate=Content.Import.Evaluation.Detail.Title translate-values=\"{filename: vm.formValues.File.filename}\"></h4><h5 translate=Content.Import.Evaluation.Detail.File.Title></h5><ul><li translate=Content.Import.Evaluation.Detail.File.ElementCount translate-values=\"{count: vm.evaluationResult.Detail.DocumentElementsCount}\"></li><li translate=Content.Import.Evaluation.Detail.File.LanguageCount translate-values=\"{count: vm.evaluationResult.Detail.LanguagesInDocumentCount}\"></li><li translate=Content.Import.Evaluation.Detail.File.Attributes translate-values=\"{count: vm.evaluationResult.Detail.AttributeNamesInDocument.length, attributes: vm.evaluationResult.Detail.AttributeNamesInDocument.join(', ')}\"></li></ul><h5 translate=Content.Import.Evaluation.Detail.Entities.Title></h5><ul><li translate=Content.Import.Evaluation.Detail.Entities.Create translate-values=\"{count: vm.evaluationResult.Detail.AmountOfEntitiesCreated}\"></li><li translate=Content.Import.Evaluation.Detail.Entities.Update translate-values=\"{count: vm.evaluationResult.Detail.AmountOfEntitiesUpdated}\"></li><li translate=Content.Import.Evaluation.Detail.Entities.Delete translate-values=\"{count: vm.evaluationResult.Detail.AmountOfEntitiesDeleted}\"></li><li translate=Content.Import.Evaluation.Detail.Entities.AttributesIgnored translate-values=\"{count: vm.evaluationResult.Detail.AttributeNamesNotImported.length, attributes: vm.evaluationResult.Detail.AttributeNamesNotImported.join(', ')}\"></li></ul><div class=text-warning translate=Content.Import.Messages.ImportCanTakeSomeTime></div></div><div ng-if=!vm.evaluationResult.Succeeded><h4 translate=Content.Import.Evaluation.Error.Title translate-values=\"{filename: vm.formValues.File.filename}\"></h4><ul><li ng-repeat=\"error in vm.evaluationResult.Detail\"><div><span translate=Content.Import.Evaluation.Error.Codes.{{error.ErrorCode}}></span></div><div ng-if=error.ErrorDetail><i translate=Content.Import.Evaluation.Error.Detail translate-values=\"{detail: error.ErrorDetail}\"></i></div><div ng-if=error.LineNumber><i translate=Content.Import.Evaluation.Error.LineNumber&quot; translate-values=\"{number: error.LineNumber}\"></i></div><div ng-if=error.LineDetail><i translate=Content.Import.Evaluation.Error.LineDetail translate-values=\"{detail: error.LineDetail}\"></i></div></li></ul></div></div><div class=modal-footer><button type=button class=\"btn pull-left\" ng-click=vm.back() icon=arrow-left></button> <button type=button class=\"btn btn-default pull-left\" ng-click=vm.importContent() translate=Content.Import.Commands.Import ng-disabled=!vm.evaluationResult.Succeeded></button></div></div><div ng-switch-when=3><div class=modal-body><span ng-show=vm.importResult.Succeeded translate=Content.Import.Messages.ImportSucceeded></span> <span ng-hide=vm.importResult.Succeeded translate=Content.Import.Messages.ImportFailed></span></div></div><div ng-if=vm.debug.on><h3>Debug infos</h3><pre>{{vm.formValues | json}}</pre></div></div></div>"
  );


  $templateCache.put('content-items/content-edit.html',
    "<div class=modal-header><button type=button class=\"btn btn-default\" ng-click=vm.history()><span class=\"glyphicon glyphicon-time\">history / todo</span></button><h3 class=modal-title>Edit / New Content</h3></div><div class=modal-body>this is where the edit appears. Would edit entity {{vm.entityId}} or add a {{vm.contentType}} - depending on the mode: {{vm.mode}}<h3>Use cases</h3><ol><li>Edit an existing entity with ID</li><li>Create a new entity of a certaint content-type, just save and done (like from a \"new\" button without content-group)</li><li>Create a new entity of a certain type and assign it to a metadata thing (guid, int, string)</li><li>Create a new entity and put it into a content-group at the right place</li><li>Edit content-group: item + presentation</li><li>Edit multiple IDs/or new/mix: Edit multiple items with IDs</li></ol>init of 1 edit - entity-id in storage - new-type + optional: assignment-id + assignment-type - array of the above --- [{id 17}, {type: \"person\"}, {type: person, asstype: 4, target: 0205}] - content-group</div>"
  );


  $templateCache.put('content-items/content-items.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Content.Manage.Title></h3></div><div class=modal-body><button icon=plus type=button class=\"btn btn-default\" ng-click=vm.add()></button> <button icon=repeat type=button class=btn ng-click=vm.refresh()></button><div style=\"overflow: auto\"><table class=\"table table-striped table-hover table-manage-eav\"><thead><tr><th translate=Content.Manage.Table.Id class=col-id></th><th translate=Content.Manage.Table.Status style=\"width: 60px\"></th><th translate=Content.Manage.Table.Title style=\"width: 200px\"></th><th translate=Content.Manage.Table.Actions class=mini-btn-2></th><th ng-repeat=\"col in vm.dynamicColumns\" style=\"width: 10%\">{{col.StaticName}}</th></tr></thead><tbody><tr ng-repeat=\"item in vm.items\"><td class=text-nowrap style=\"text-align: right\"><span tooltip=\"Id: {{item.Id}}\r" +
    "\n" +
    "RepoId: {{item.RepositoryId}}\r" +
    "\n" +
    "Guid: {{item.Guid}}\">{{item.RepositoryId}}</span></td><td class=text-nowrap><span class=glyphicon ng-class=\"{'glyphicon-ok-circle': item.IsPublished, 'glyphicon-ban-circle' : !item.IsPublished && !item.Published, 'glyphicon-record' : !item.IsPublished && item.Published }\" tooltip=\"{{ 'Content.Publish.' + (item.IsPublished ? 'PnV': item.Published ? 'DoP' : 'D') | translate }}\"></span> <span icon=\"{{ item.Draft ? 'paperclip' : item.Published ? 'export' : '' }}\" tooltip=\"{{ (item.Draft ? 'Content.Publish.HD' :'') | translate:'{ id: item.Draft.RepositoryId}' }}\r" +
    "\n" +
    "{{ (item.Published ? 'Content.Publish.HP' :'') | translate }} #{{ item.Published.RepositoryId }}\"></span> <span ng-if=item.Metadata tooltip=\"Metadata for type {{ item.Metadata.TargetType}}, id {{ item.Metadata.KeyNumber }}{{ item.Metadata.KeyString }}{{ item.Metadata.KeyGuid }}\" icon=tag></span></td><td class=text-nowrap><div class=hide-overflow-text style=\"height: 20px; width: 200px\" tooltip={{item.Title}}><a ng-click=vm.edit(item)>{{item.Title}}{{ (!item.Title ? 'Content.Manage.NoTitle':'') | translate }}</a></div></td><td class=text-nowrap><button icon=duplicate type=button class=\"btn btn-xs\" ng-click=vm.openDuplicate(item) tooltip=\"{{ 'General.Buttons.Copy' | translate }}\"></button> <button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item) tooltip=\"{{ 'General.Buttons.Delete' | translate }}\"></button></td><td ng-repeat=\"col in vm.dynamicColumns\" width=10%><div style=\"height: 20px; max-width: 300px\" class=hide-overflow-text tooltip={{item[col.StaticName]}}>{{item[col.StaticName].toString().substring(0,25)}}</div></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div></div>"
  );


  $templateCache.put('content-items/history-details.html',
    "<div><div class=modal-header><button class=\"btn btn-primary pull-right\" type=button ng-click=vm.close()><span class=\"glyphicon glyphicon-remove\"></span></button><h3 class=modal-title>History Details {{vm.ChangeId}} of {{vm.entityId}}</h3></div><div class=modal-body><h1>todo</h1><table class=\"table table-striped table-hover\"><thead><tr><th>Field</th><th>Language</th><th>Value</th><th>SharedWith</th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:SysCreatedDate:reverse\"><td>{{item.Field}}</td><td>{{item.Language}}</td><td>{{item.Value}}</td><td>{{item.SharedWith}}</td></tr><tr ng-if=!vm.items.length><td colspan=100>No History</td></tr></tbody></table><button class=\"btn btn-primary pull-right\" type=button ng-click=vm.restore()><span class=\"glyphicon glyphicon-ok\">todo restore</span></button></div></div>"
  );


  $templateCache.put('content-items/history.html',
    "<div><div class=modal-header><button class=\"btn btn-primary pull-right\" type=button ng-click=vm.close()><span class=\"glyphicon glyphicon-remove\"></span></button><h3 class=modal-title>{{ \"Content.History.Title\" | translate:'{ id:vm.entityId }' }}History of {{vm.entityId}}</h3></div><div class=modal-body><table class=\"table table-striped table-hover\"><thead><tr><th translate=Content.History.Table.Id></th><th translate=Content.History.Table.When></th><th translate=Content.History.Table.User></th><th translate=Content.History.Table.Action></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:SysCreatedDate:reverse\"><td><span tooltip=\"ChangeId: {{item.ChangeId}}\">{{item.VirtualVersion}}</span></td><td>{{item.SysCreatedDate.replace(\"T\", \" \")}}</td><td>{{item.User}}</td><td><button type=button class=\"btn btn-xs\" ng-click=vm.details(item)><span class=\"glyphicon glyphicon-search\"></span></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div></div>"
  );


  $templateCache.put('content-types/content-types-edit.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=ContentTypeEdit.Title></h3></div><div class=modal-body>{{ \"ContentTypeEdit.Name\" | translate }}:<br><input ng-model=\"vm.item.Name\"><br>{{ \"ContentTypeEdit.Description\" | translate }}:<br><input ng-model=\"vm.item.Description\"><br>{{ \"ContentTypeEdit.Scope\" | translate }}:<br><input disabled ng-model=\"vm.item.Scope\"></div><div class=modal-footer><button icon=ok class=\"btn btn-default pull-left\" type=button ng-click=vm.ok()></button></div>"
  );


  $templateCache.put('content-types/content-types-field-edit.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Fields.TitleEdit></h3></div><div class=modal-body><table class=\"table table-hover table-manage-eav\"><thead><tr><th translate=Fields.Table.Name style=\"width: 200px\"></th><th translate=Fields.Table.DataType style=\"width: 150px\">Data Type</th><th></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items\"><td><input ng-model=item.StaticName ng-required=\"true\"></td><td><select ng-model=item.Type ng-options=\"o as 'DataType.' + o + '.Choice' | translate for o in vm.types | orderBy: 'toString()' \"></select></td><td></td></tr></tbody></table></div><div class=modal-footer><button icon=ok class=\"btn btn-default pull-left\" type=button ng-click=vm.ok()></button></div>"
  );


  $templateCache.put('content-types/content-types-fields.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Fields.Title></h3></div><div class=modal-body><button icon=plus ng-click=vm.add() class=\"btn btn-default\"></button><table class=\"table table-hover table-manage-eav\"><thead><tr><th translate=Fields.Table.Title class=mini-btn-1></th><th translate=Fields.Table.Name style=\"width: 200px\"></th><th translate=Fields.Table.DataType style=\"width: 100px\"></th><th translate=Fields.Table.Label style=\"width: 150px\"></th><th translate=Fields.Table.Notes style=\"width: 250px\"></th><th translate=Fields.Table.Sort class=mini-btn-2></th><th translate=Fields.Table.Action class=mini-btn-1></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy: 'SortOrder'\"><td><button icon=\"{{item.IsTitle ? 'star' : 'star-empty'}}\" type=button class=\"btn btn-xs\" ng-style=\"(item.IsTitle ? '' : 'color: transparent !important')\" ng-click=vm.setTitle(item)></button></td><td><a tooltip=\"{{ 'Id: ' + item.Id}}\" ng-click=\"vm.createOrEditMetadata(item, item.Type)\">{{item.StaticName}}</a></td><td class=text-nowrap>{{item.Type}}</td><td class=text-nowrap>{{item.Metadata.All.Name}}</td><td class=text-nowrap>{{item.Metadata.All.Notes}}</td><td class=text-nowrap><button icon=arrow-up type=button class=\"btn btn-xs\" ng-disabled=$first ng-click=vm.moveUp(item)></button> <button icon=arrow-down type=button class=\"btn btn-xs\" ng-disabled=$last ng-click=vm.moveDown(item)></button></td><td><button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div>"
  );


  $templateCache.put('content-types/content-types.html',
    "<div ng-controller=\"List as vm\" ng-click=vm.debug.autoEnableAsNeeded($event)><div class=modal-header><h3 class=modal-title translate=ContentTypes.Title></h3></div><div class=modal-body><button icon=plus title=\"{{ 'General.Buttons.Add' | translate }}\" type=button class=\"btn btn-default\" ng-click=vm.edit()></button> <button icon=repeat title=\"{{ 'General.Buttons.Refresh' | translate }}\" type=button class=btn ng-click=vm.refresh()></button> <span ng-if=vm.debug.on><button icon=record title=\"{{ 'ContentTypes.Buttons.ChangeScope' | translate }}\" type=button class=btn ng-click=vm.changeScope()></button> <button icon=flash title=\"{{ 'General.Buttons.System' | translate }}\" type=button class=btn ng-click=vm.liveEval()></button></span><table class=\"table table-hover\" style=\"table-layout: fixed; width: 100%\"><thead><tr><th translate=ContentTypes.TypesTable.Items class=col-id></th><th translate=ContentTypes.TypesTable.Name style=\"width: 50%\"></th><th translate=ContentTypes.TypesTable.Description style=\"width: 50%\"></th><th translate=ContentTypes.TypesTable.Fields class=mini-btn-2></th><th translate=ContentTypes.TypesTable.Actions class=mini-btn-3></th><th style=\"width: 40px\"></th></tr></thead><tbody><tr ng-if=vm.items.isLoaded ng-repeat=\"item in vm.items | orderBy:'Name'\"><td style=\"text-align: center\">{{item.Items}}</td><td><a class=\"text-nowrap hide-overflow-text\" style=\"max-width: 400px\" ng-click=vm.editItems(item) target=_self tooltip={{item.Name}}>{{item.Name}}</a></td><td><div class=\"text-nowrap hide-overflow-text\" style=\"max-width: 500px\" tooltip={{item.Description}}>{{item.Description}}</div></td><td><button type=button class=\"btn btn-xs\" style=\"width: 60px\" ng-click=vm.editFields(item)><span icon=list>&nbsp;{{item.Fields}}</span></button></td><td class=text-nowrap><button icon=info-sign tooltip=\"{{ 'ContentTypes.Buttons.Rename' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.edit(item)></button> <button icon=export tooltip=\"{{ 'ContentTypes.Buttons.Export' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.openExport(item)></button> <button icon=import tooltip=\"{{ 'ContentTypes.Buttons.Import' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.openImport(item)></button> <button icon=user type=button class=\"btn btn-xs\" ng-click=vm.permissions(item) ng-if=vm.isGuid(item.StaticName)></button></td><td><button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td></tr><tr ng-if=!vm.items.length><td colspan=100>{{ 'General.Messages.Loading' | translate }} / {{ 'General.Messages.NothingFound' | translate }}</td></tr></tbody></table></div><div ng-if=vm.debug.on><h3>Notes / Debug / ToDo</h3><ol><li>get validators to work on all dialogs</li><li>this dialog doesn't refresh properly when I add/change stuff</li></ol></div></div>"
  );


  $templateCache.put('permissions/permissions.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Permissions.Title></h3></div><div class=modal-body><button icon=plus type=button class=\"btn btn-default\" ng-click=vm.add()></button> <button icon=repeat type=button class=btn ng-click=vm.refresh()></button><table class=\"table table-striped table-hover table-manage-eav\"><thead><tr><th translate=Permissions.Table.Id style=\"width: 60px\"></th><th translate=Permissions.Table.Name style=\"width: 33%\"></th><th translate=Permissions.Table.Condition style=\"width: 33%\"></th><th translate=Permissions.Table.Grant style=\"width: 33%\"></th><th translate=Permissions.Table.Actions style=\"width: 40px\"></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:'Title'\"><td>{{item.Id}}</td><td><a ng-click=vm.edit(item)>{{item.Title}}</a></td><td>{{item.Condition}}</td><td>{{item.Grant}}</td><td class=text-nowrap><button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div>"
  );


  $templateCache.put('pipelines/pipeline-designer.html',
    "<div class=ng-cloak><div ng-controller=PipelineDesignerController><div id=pipelineContainer><div ng-repeat=\"dataSource in pipelineData.DataSources\" datasource guid={{dataSource.EntityGuid}} id=dataSource_{{dataSource.EntityGuid}} class=dataSource ng-attr-style=\"top: {{dataSource.VisualDesignerData.Top}}px; left: {{dataSource.VisualDesignerData.Left}}px\"><div class=configure ng-click=configureDataSource(dataSource) title=\"Configure this DataSource\" ng-if=!dataSource.ReadOnly><span class=\"glyphicon glyphicon-list-alt\"></span></div><div class=name title=\"Click to edit the Name\" ng-click=editName(dataSource)>{{dataSource.Name || '(unnamed)'}}</div><br><div class=description title=\"Click to edit the Description\" ng-click=editDescription(dataSource)>{{dataSource.Description || '(no description)'}}</div><br><div class=typename ng-attr-title={{dataSource.PartAssemblyAndType}}>Type: {{dataSource.PartAssemblyAndType | typename: 'className'}}</div><div class=ep title=\"Drag a new Out-Connection from here\" ng-if=!dataSource.ReadOnly><span class=\"glyphicon glyphicon-plus-sign\"></span></div><div class=\"delete glyphicon glyphicon-remove\" title=\"Delete this DataSource\" ng-click=remove($index) ng-if=!dataSource.ReadOnly></div></div></div><div class=\"actions panel panel-default\"><div class=panel-heading><span class=pull-left>Actions</span> <a href=http://2sxc.org/help class=\"btn btn-info btn-xs pull-right\" target=_blank><span class=\"glyphicon glyphicon-question-sign\"></span> Help</a></div><div class=panel-body><button type=button class=\"btn btn-primary btn-block\" ng-disabled=readOnly ng-click=savePipeline()><span class=\"glyphicon glyphicon-floppy-save\"></span> Save</button><select class=form-control ng-model=addDataSourceType ng-disabled=readOnly ng-change=addDataSource() ng-options=\"d.ClassName for d in pipelineData.InstalledDataSources | filter: {allowNew: '!false'} | orderBy: 'ClassName'\"><option value=\"\">-- Add DataSource --</option></select><button type=button class=\"btn btn-default btn-sm\" title=\"Query the Data of this Pipeline\" ng-click=queryPipeline()><span class=\"glyphicon glyphicon-play\"></span> Query</button> <button type=button class=\"btn btn-default btn-sm\" title=\"Clone this Pipeline with all DataSources and Configurations\" ng-click=clonePipeline() ng-disabled=!PipelineEntityId><span class=\"glyphicon glyphicon-share-alt\"></span> Clone</button> <button type=button class=\"btn btn-default btn-sm\" ng-click=editPipelineEntity()><span class=\"glyphicon glyphicon-pencil\"></span> Test Parameters</button> <button type=button class=\"btn btn-info btn-xs\" ng-click=toggleEndpointOverlays()><span class=\"glyphicon glyphicon-info-sign\"></span> {{showEndpointOverlays ? 'Hide' : 'Show' }} Overlays</button> <button type=button class=\"btn btn-info btn-xs\" ng-click=repaint()><span class=\"glyphicon glyphicon-repeat\"></span> Repaint</button> <button type=button class=\"btn btn-info btn-xs\" ng-click=toogleDebug()><span class=\"glyphicon glyphicon-info-sign\"></span> {{debug ? 'Hide' : 'Show'}} Debug Info</button></div></div><toaster-container></toaster-container><pre ng-if=debug>{{pipelineData | json}}</pre></div></div>"
  );


  $templateCache.put('pipelines/pipelines.html',
    "<div class=modal-header><h3 class=modal-title translate=Pipeline.Manage.Title></h3></div><div class=\"modal-body ng-cloak\"><div translate=Pipeline.Manage.Intro></div><div><button icon=plus type=button class=\"btn btn-default\" ng-click=vm.add()></button> <button icon=repeat type=button class=btn ng-click=vm.refresh()></button> <button icon=flash type=button class=btn ng-click=vm.liveEval()></button><table class=\"table table-striped table-hover table-manage-eav\"><thead><tr><th translate=Pipeline.Manage.Table.Id class=col-id></th><th translate=Pipeline.Manage.Table.Name></th><th translate=Pipeline.Manage.Table.Description></th><th translate=Pipeline.Manage.Table.Actions class=mini-btn-4></th></tr></thead><tbody><tr ng-repeat=\"pipeline in vm.pipelines | orderBy:'Name'\"><td>{{pipeline.Id}}</td><td><a ng-click=vm.edit(pipeline)>{{pipeline.Name}}</a></td><td>{{pipeline.Description}}</td><td class=\"text-nowrap mini-btn-4\"><button icon=edit title=\"{{ 'General.Buttons.Edit' | translate }}\" class=\"btn btn-xs btn-default\" ng-click=vm.design(pipeline)></button> <button icon=user title=\"{{ 'General.Buttons.Permissions' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.permissions(pipeline)></button> <button icon=duplicate title=\"{{ 'General.Buttons.Copy' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.clone(pipeline)></button> <button icon=remove title=\"{{ 'General.Buttons.Delete' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.delete(pipeline)></button></td></tr><tr ng-if=!vm.pipelines.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div></div>"
  );


  $templateCache.put('pipelines/query-stats.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Pipeline.Stats.Title></h3></div><div class=modal-body><div translate=Pipeline.Stats.Intro></div><div><h3 translate=Pipeline.Stats.ParamTitle></h3><div translate=Pipeline.Stats.ExecutedIn translate-values=\"{ ms: vm.timeUsed, ticks: vm.ticksUsed }\"></div><div><ul><li ng-repeat=\"param in vm.testParameters\">{{param}}</li></ul></div></div><div><h3 translate=Pipeline.Stats.QueryTitle></h3><pre>{{vm.result | json}}</pre></div><div><h3 translate=Pipeline.Stats.SourcesAndStreamsTitle></h3><h4 translate=Pipeline.Stats.Sources.Title></h4><table><tr><th translate=Pipeline.Stats.Sources.Guid></th><th translate=Pipeline.Stats.Sources.Type></th><th translate=Pipeline.Stats.Sources.Config></th></tr><tr ng-repeat=\"s in vm.sources\"><td tooltip={{s}}><pre>{{s.Guid.substring(0, 13)}}...</pre></td><td>{{s.Type}}</td><td tooltip={{s.Configuration}}><ol><li ng-repeat=\"(key, value) in s.Configuration\"><b>{{key}}</b>=<em>{{value}}</em></li></ol></td></tr></table><h4 translate=Pipeline.Stats.Streams.Title></h4><table><tr><th translate=Pipeline.Stats.Streams.Source></th><th translate=Pipeline.Stats.Streams.Target></th><th translate=Pipeline.Stats.Streams.Items></th><th translate=Pipeline.Stats.Streams.Error></th></tr><tr ng-repeat=\"sr in vm.streams\"><td><pre>{{sr.Source.substring(0, 13) + \":\" + sr.SourceOut}}</pre></td><td><pre>{{sr.Target.substring(0, 13) + \":\" + sr.TargetIn}}</pre></td><td><span>{{sr.Count}}</span></td><td><span>{{sr.Error}}</span></td></tr></table></div></div>"
  );

}]);

(function () { 

    angular.module("PermissionsApp", [
        "EavServices",
        "EavConfiguration",
        "EavAdminUi"])
        .controller("PermissionList", permissionListController)
        ;

    function permissionListController(permissionsSvc, eavAdminDialogs, eavConfig, appId, targetGuid, $modalInstance /* $location */) {
        var vm = this;
        var svc = permissionsSvc(appId, targetGuid);

        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.add = function add() {
            eavAdminDialogs.openMetadataNew(appId, "entity", svc.PermissionTargetGuid, svc.ctName, svc.liveListReload);
        };

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;
        
        vm.tryToDelete = function tryToDelete(item) {
            if (confirm("Delete '" + item.Title + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
    permissionListController.$inject = ["permissionsSvc", "eavAdminDialogs", "eavConfig", "appId", "targetGuid", "$modalInstance"];

} ());
angular.module("PipelineDesigner", [
        "PipelineDesigner.filters",
        "ngResource",
        "EavConfiguration",
        "EavServices",
        "eavTemplates",
        "eavNgSvcs",
        "EavAdminUi"
    ])

// datasource directive makes an element a DataSource with jsPlumb
    .directive("datasource", ["$timeout", function($timeout) {
        return {
            restrict: "A",
            link: function(scope, element) {
                // make this a DataSource when the DOM is ready
                $timeout(function() {
                    scope.makeDataSource(scope.dataSource, element);
                });
                if (scope.$last === true) {
                    $timeout(function() {
                        scope.$emit("ngRepeatFinished");
                    });
                }
            }
        };
    }]);

// Filters for "ClassName, AssemblyName"
angular.module("PipelineDesigner.filters", []).filter("typename", function () {
	return function (input, format) {
		var globalParts = input.match(/[^,\s]+/g);

		switch (format) {
			case "classFullName":
				if (globalParts)
				    return globalParts[0];
			    break;
			case "className":
				if (globalParts) {
					var classFullName = globalParts[0].match(/[^\.]+/g);
					return classFullName[classFullName.length - 1];
				}
		}

		return input;
	};
});
// AngularJS Controller for the >>>> Pipeline Designer
// todo: refactor the pipeline designer to use the new eavAdminUi service

/*jshint laxbreak:true */
(function () {

    angular.module("PipelineDesigner")
        .controller("PipelineDesignerController",
            ["appId", "pipelineId", "$scope", "pipelineService", "$location", "$timeout", "$filter", "uiNotification", "eavAdminDialogs", "$log", "eavConfig", "$q", function (appId, pipelineId, $scope, pipelineService, $location, $timeout, $filter, uiNotification, eavAdminDialogs, $log, eavConfig, $q) {
                "use strict";

                // Init
                uiNotification.wait();
                $scope.readOnly = true;
                $scope.dataSourcesCount = 0;
                $scope.dataSourceIdPrefix = "dataSource_";
                $scope.debug = false;

                // Load Pipeline Data
                $scope.PipelineEntityId = pipelineId;

                pipelineService.setAppId(appId);

                $scope.findDataSourceOfElement = function fdsog(element) {
                    var guid = element.attributes.guid.value;
                    var list = $scope.pipelineData.DataSources;
                    var found = $filter("filter")(list, { EntityGuid: guid })[0];
                    return found;
                };

                // Get Data from PipelineService (Web API)
                pipelineService.getPipeline($scope.PipelineEntityId)
                    .then(function(success) {
                        $scope.pipelineData = success;

                        // If a new (empty) Pipeline is made, init new Pipeline
                        if (!$scope.PipelineEntityId || $scope.pipelineData.DataSources.length === 1) {
                            $scope.readOnly = false;
                            initNewPipeline();
                        } else {
                            // if read only, show message
                            $scope.readOnly = !success.Pipeline.AllowEdit;
                            uiNotification.note("Ready", $scope.readOnly ? "This pipeline is read only" : "You can now design the Pipeline. \nNote that there are still a few UI bugs.\nVisit 2sxc.org/help for more.", true);
                        }
                    }, function(reason) {
                        uiNotification.error("Loading Pipeline failed", reason);
                    });

                // init new jsPlumb Instance
                jsPlumb.ready(function() {
                    $scope.jsPlumbInstance = jsPlumb.getInstance({
                        Connector: ["Bezier", { curviness: 70 }],
                        HoverPaintStyle: {
                            lineWidth: 4,
                            strokeStyle: "#216477",
                            outlineWidth: 2,
                            outlineColor: "white"
                        },
                        PaintStyle: {
                            lineWidth: 4,
                            strokeStyle: "#61B7CF",
                            joinstyle: "round",
                            outlineColor: "white",
                            outlineWidth: 2
                        },
                        Container: "pipelineContainer"
                    });

                    // If connection on Out-DataSource was removed, remove custom Endpoint
                    $scope.jsPlumbInstance.bind("connectionDetached", function(info) {
                        if (info.targetId == $scope.dataSourceIdPrefix + "Out") {
                            var element = angular.element(info.target);
                            var fixedEndpoints = $scope.findDataSourceOfElement(element) /* element.scope() */.dataSource.Definition().In;
                            var label = info.targetEndpoint.getOverlay("endpointLabel").label;
                            if (fixedEndpoints.indexOf(label) == -1) {
                                $timeout(function() {
                                    $scope.jsPlumbInstance.deleteEndpoint(info.targetEndpoint);
                                });
                            }
                        }
                    });

                    // If a new connection is created, ask for a name of the In-Stream
                    $scope.jsPlumbInstance.bind("connection", function(info) {
                        if (!$scope.connectionsInitialized) return;

                        // Repeat until a valid Stream-Name is provided by the user
                        var repeatCount = 0;
                        var endpointHandling = function(endpoint) {
                            var label = endpoint.getOverlay("endpointLabel").getLabel();
                            if (label === labelPrompt && info.targetEndpoint.id !== endpoint.id && angular.element(endpoint.canvas).hasClass("targetEndpoint"))
                                targetEndpointHavingSameLabel = endpoint;
                        };
                        while (true) {
                            repeatCount++;

                            var promptMessage = "Please name the Stream";
                            if (repeatCount > 1)
                                promptMessage += ". Ensure the name is not used by any other Stream on this DataSource.";

                            var endpointLabel = info.targetEndpoint.getOverlay("endpointLabel");
                            var labelPrompt = prompt(promptMessage, endpointLabel.getLabel());
                            if (labelPrompt)
                                endpointLabel.setLabel(labelPrompt);
                            else
                                continue;

                            // Check if any other Target-Endpoint has the same Stream-Name (Label)
                            var endpoints = $scope.jsPlumbInstance.getEndpoints(info.target.id);
                            var targetEndpointHavingSameLabel = null;

                            angular.forEach(endpoints, endpointHandling);
                            if (targetEndpointHavingSameLabel)
                                continue;

                            break;
                        }
                    });
                });

                // #region jsPlumb Endpoint Definitions
                var getEndpointOverlays = function(isSource) {
                    return [
                        [
                            "Label", {
                                id: "endpointLabel",
                                location: [0.5, isSource ? -0.5 : 1.5],
                                label: "Default",
                                cssClass: isSource ? "endpointSourceLabel" : "endpointTargetLabel",
                                events: {
                                    dblclick: function(labelOverlay) {
                                        if ($scope.readOnly) return;

                                        var newLabel = prompt("Rename Stream", labelOverlay.label);
                                        if (newLabel)
                                            labelOverlay.setLabel(newLabel);
                                    }
                                }
                            }
                        ]
                    ];
                };

                // the definition of source endpoints (the small blue ones)
                var sourceEndpoint = {
                    paintStyle: { fillStyle: "transparent", radius: 10, lineWidth: 0 },
                    cssClass: "sourceEndpoint",
                    maxConnections: -1,
                    isSource: true,
                    anchor: ["Continuous", { faces: ["top"] }],
                    overlays: getEndpointOverlays(true)
                };

                // the definition of target endpoints (will appear when the user drags a connection) 
                var targetEndpoint = {
                    paintStyle: { fillStyle: "transparent", radius: 10, lineWidth: 0 },
                    cssClass: "targetEndpoint",
                    maxConnections: 1,
                    isTarget: true,
                    anchor: ["Continuous", { faces: ["bottom"] }],
                    overlays: getEndpointOverlays(false),
                    dropOptions: { hoverClass: "hover", activeClass: "active" }
                };
                // #endregion

                // make a DataSource with Endpoints, called by the datasource-Directive
                $scope.makeDataSource = function(dataSource, element) {
                    // suspend drawing and initialise
                    $scope.jsPlumbInstance.doWhileSuspended(function() {
                        // Add Out- and In-Endpoints from Definition
                        var dataSourceDefinition = dataSource.Definition();
                        if (dataSourceDefinition !== null) {
                            // Add Out-Endpoints
                            angular.forEach(dataSourceDefinition.Out, function(name) {
                                addEndpoint(element, name, false);
                            });
                            // Add In-Endpoints
                            angular.forEach(dataSourceDefinition.In, function(name) {
                                addEndpoint(element, name, true);
                            });
                            // make the DataSource a Target for new Endpoints (if .In is an Array)
                            if (dataSourceDefinition.In) {
                                var targetEndpointUnlimited = targetEndpoint;
                                targetEndpointUnlimited.maxConnections = -1;
                                $scope.jsPlumbInstance.makeTarget(element, targetEndpointUnlimited);
                            }

                            $scope.jsPlumbInstance.makeSource(element, sourceEndpoint, { filter: ".ep .glyphicon" });
                        }

                        // make DataSources draggable
                        if (!$scope.readOnly) {
                            $scope.jsPlumbInstance.draggable(element, {
                                grid: [20, 20],
                                drag: $scope.dataSourceDrag
                            });
                        }
                    });

                    $scope.dataSourcesCount++;
                };

                // Add a jsPlumb Endpoint to an Element
                var addEndpoint = function(element, name, isIn) {
                    if (!element.length) {
                        $log.error({ message: "Element not found", selector: element.selector });
                        return;
                    }
                    console.log(element);

                    var dataSource = $scope.findDataSourceOfElement(element[0]);
                    // old, using jQuery - var dataSource = element.scope().dataSource;


                    var uuid = element[0].id + (isIn ? "_in_" : "_out_") + name;
                    // old - using jQuery - var uuid = element.attr("id") + (isIn ? "_in_" : "_out_") + name;
                    var params = {
                        uuid: uuid,
                        enabled: !dataSource.ReadOnly || dataSource.EntityGuid == "Out" // Endpoints on Out-DataSource must be always enabled
                    };
                    var endPoint = $scope.jsPlumbInstance.addEndpoint(element, (isIn ? targetEndpoint : sourceEndpoint), params);
                    endPoint.getOverlay("endpointLabel").setLabel(name);
                };

                // Initialize jsPlumb Connections once after all DataSources were created in the DOM
                $scope.connectionsInitialized = false;
                $scope.$on("ngRepeatFinished", function() {
                    if ($scope.connectionsInitialized) return;

                    // suspend drawing and initialise
                    $scope.jsPlumbInstance.doWhileSuspended(function() {
                        initWirings($scope.pipelineData.Pipeline.StreamWiring);
                    });
                    $scope.repaint(); // repaint so continuous connections are aligned correctly

                    $scope.connectionsInitialized = true;
                });


                var initWirings = function(streamWiring) {
                    angular.forEach(streamWiring, function(wire) {
                        // read connections from Pipeline
                        var sourceElementId = $scope.dataSourceIdPrefix + wire.From;
                        var fromUuid = sourceElementId + "_out_" + wire.Out;
                        var targetElementId = $scope.dataSourceIdPrefix + wire.To;
                        var toUuid = targetElementId + "_in_" + wire.In;

                        // Ensure In- and Out-Endpoint exist
                        if (!$scope.jsPlumbInstance.getEndpoint(fromUuid))
                            addEndpoint(jsPlumb.getSelector("#" + sourceElementId), wire.Out, false);
                        if (!$scope.jsPlumbInstance.getEndpoint(toUuid))
                            addEndpoint(jsPlumb.getSelector("#" + targetElementId), wire.In, true);

                        try {
                            $scope.jsPlumbInstance.connect({ uuids: [fromUuid, toUuid] });
                        } catch (e) {
                            $log.error({ message: "Connection failed", from: fromUuid, to: toUuid });
                        }
                    });

                    // $scope.jsPlumbInstance.getConnections

                };

                // Init a new Pipeline with DataSources and Wirings from Configuration
                var initNewPipeline = function () {
                    var templateForNew = eavConfig.pipelineDesigner.defaultPipeline.dataSources;
                    angular.forEach(templateForNew, function(dataSource) {
                        $scope.addDataSource(dataSource.partAssemblyAndType, dataSource.visualDesignerData, false, dataSource.entityGuid);
                    });

                    // Wait until all DataSources were created
                    var initWiringsListener = $scope.$on("ngRepeatFinished", function() {
                        $scope.connectionsInitialized = false;
                        initWirings(eavConfig.pipelineDesigner.defaultPipeline.streamWiring);
                        $scope.connectionsInitialized = true;

                        initWiringsListener(); // unbind the Listener
                    });
                };

                // Add new DataSource
                $scope.addDataSource = function(partAssemblyAndType, visualDesignerData, autoSave, entityGuid) {
                    if (!partAssemblyAndType) {
                        partAssemblyAndType = $scope.addDataSourceType.PartAssemblyAndType;
                        $scope.addDataSourceType = null;
                    }
                    if (!visualDesignerData)
                        visualDesignerData = { Top: 100, Left: 100 };

                    var newDataSource = {
                        VisualDesignerData: visualDesignerData,
                        Name: $filter("typename")(partAssemblyAndType, "className"),
                        Description: "",
                        PartAssemblyAndType: partAssemblyAndType,
                        EntityGuid: entityGuid || "unsaved" + ($scope.dataSourcesCount + 1)
                    };
                    // Extend it with a Property to it's Definition
                    newDataSource = angular.extend(newDataSource, pipelineService.getNewDataSource($scope.pipelineData, newDataSource));

                    $scope.pipelineData.DataSources.push(newDataSource);

                    if (autoSave !== false)
                        $scope.savePipeline();
                };

                // Delete a DataSource
                $scope.remove = function(index) {
                    var dataSource = $scope.pipelineData.DataSources[index];
                    if (!confirm("Delete DataSource \"" + (dataSource.Name || "(unnamed)") + "\"?")) return;
                    var elementId = $scope.dataSourceIdPrefix + dataSource.EntityGuid;
                    $scope.jsPlumbInstance.selectEndpoints({ element: elementId }).remove();
                    $scope.pipelineData.DataSources.splice(index, 1);
                };

                // Edit name of a DataSource
                $scope.editName = function(dataSource) {
                    if (dataSource.ReadOnly) return;

                    var newName = prompt("Rename DataSource", dataSource.Name);
                    if (newName !== undefined && newName.trim())
                        dataSource.Name = newName;
                };

                // Edit Description of a DataSource
                $scope.editDescription = function(dataSource) {
                    if (dataSource.ReadOnly) return;

                    var newDescription = prompt("Edit Description", dataSource.Description);
                    if (newDescription !== undefined && newDescription.trim())
                        dataSource.Description = newDescription;
                };

                // Update DataSource Position on Drag
                $scope.dataSourceDrag = function() {
                    var $this = /* angular.element(this); /*/  $(this);
                    var offset = $this.offset();
                    var dataSource = $scope.findDataSourceOfElement($this).dataSource;// $this.scope().dataSource;
                    $scope.$apply(function() {
                        dataSource.VisualDesignerData.Top = Math.round(offset.top);
                        dataSource.VisualDesignerData.Left = Math.round(offset.left);
                    });
                };

                // Configure a DataSource
                $scope.configureDataSource = function(dataSource) {
                    if (dataSource.ReadOnly) return;

                    // Ensure dataSource Entity is saved
                    if (!dataSourceIsPersisted(dataSource)) {
                        $scope.savePipeline();
                        return;
                    }

                    pipelineService.editDataSourcePart(dataSource);

                };

                // Test wether a DataSource is persisted on the Server
                var dataSourceIsPersisted = function(dataSource) {
                    return dataSource.EntityGuid.indexOf("unsaved") === -1;
                };

                // Show/Hide Endpoint Overlays
                $scope.showEndpointOverlays = true;
                $scope.toggleEndpointOverlays = function() {
                    $scope.showEndpointOverlays = !$scope.showEndpointOverlays;

                    var endpoints = $scope.jsPlumbInstance.selectEndpoints();
                    if ($scope.showEndpointOverlays)
                        endpoints.showOverlays();
                    else
                        endpoints.hideOverlays();
                };

                // Edit Pipeline Entity
                $scope.editPipelineEntity = function() {
                    // save Pipeline, then open Edit Dialog
                    $scope.savePipeline().then(function() {

                        eavAdminDialogs.openEditItems([{ EntityId: $scope.PipelineEntityId }], function() {
                            pipelineService.getPipeline($scope.PipelineEntityId).then(pipelineSaved);
                        });

                    });
                };

                // Sync jsPlumb Connections and StreamsOut to the pipelineData-Object
                var syncPipelineData = function() {
                    var connectionInfos = [];
                    angular.forEach($scope.jsPlumbInstance.getAllConnections(), function(connection) {
                        connectionInfos.push({
                            From: connection.sourceId.substr($scope.dataSourceIdPrefix.length),
                            Out: connection.endpoints[0].getOverlay("endpointLabel").label,
                            To: connection.targetId.substr($scope.dataSourceIdPrefix.length),
                            In: connection.endpoints[1].getOverlay("endpointLabel").label
                        });
                    });
                    $scope.pipelineData.Pipeline.StreamWiring = connectionInfos;

                    var streamsOut = [];
                    $scope.jsPlumbInstance.selectEndpoints({ target: $scope.dataSourceIdPrefix + "Out" }).each(function(endpoint) {
                        streamsOut.push(endpoint.getOverlay("endpointLabel").label);
                    });
                    $scope.pipelineData.Pipeline.StreamsOut = streamsOut.join(",");
                };

                // #region Save Pipeline
                // Save Pipeline
                // returns a Promise about the saving state
                $scope.savePipeline = function(successHandler) {
                    uiNotification.wait("Saving...");
                    $scope.readOnly = true;

                    syncPipelineData();

                    var deferred = $q.defer();

                    if (typeof successHandler == "undefined") // set default success Handler
                        successHandler = pipelineSaved;

                    pipelineService.savePipeline($scope.pipelineData.Pipeline, $scope.pipelineData.DataSources).then(successHandler, function(reason) {
                        uiNotification.error("Save Pipeline failed", reason);
                        $scope.readOnly = false;
                        deferred.reject();
                    }).then(function() {
                        deferred.resolve();
                    });

                    return deferred.promise;
                };

                // Handle Pipeline Saved, success contains the updated Pipeline Data
                var pipelineSaved = function(success) {
                    // Update PipelineData with data retrieved from the Server
                    $scope.pipelineData.Pipeline = success.Pipeline;
                    $scope.PipelineEntityId = success.Pipeline.EntityId /*EntityId*/;
                    $location.search("PipelineId", success.Pipeline.EntityId /*EntityId*/);
                    $scope.readOnly = !success.Pipeline.AllowEdit;
                    $scope.pipelineData.DataSources = success.DataSources;
                    pipelineService.postProcessDataSources($scope.pipelineData);

                    uiNotification.success("Saved", "Pipeline " + success.Pipeline.EntityId /*EntityId*/ + " saved and loaded", true);

                    // Reset jsPlumb, re-Init Connections
                    $scope.jsPlumbInstance.reset();
                    $scope.connectionsInitialized = false;
                };
                // #endregion

                // Repaint jsPlumb
                $scope.repaint = function() {
                    $scope.jsPlumbInstance.repaintEverything();
                };

                // Show/Hide Debug info
                $scope.toogleDebug = function() {
                    $scope.debug = !$scope.debug;
                };

                // Query the Pipeline
                $scope.queryPipeline = function() {
                    var query = function() {
                        // Query pipelineService for the result...
                        uiNotification.wait("Running Query ...");

                        pipelineService.queryPipeline($scope.PipelineEntityId).then(function(success) {
                            // Show Result in a UI-Dialog
                            uiNotification.clear();

                            var resolve = eavAdminDialogs.CreateResolve({ testParams: $scope.pipelineData.Pipeline.TestParameters, result: success });
                            eavAdminDialogs.OpenModal("pipelines/query-stats.html", "QueryStats as vm", "lg", resolve);

                            $timeout(function() {
                                showEntityCountOnStreams(success);
                            });
                            $log.debug(success);
                        }, function(reason) {
                            uiNotification.error("Query failed", reason);
                        });
                    };


                    var showEntityCountOnStreams = function(result) {
                        angular.forEach(result.Streams, function(stream) {
                            // Find jsPlumb Connection for the current Stream
                            var sourceElementId = $scope.dataSourceIdPrefix + stream.Source;
                            var targetElementId = $scope.dataSourceIdPrefix + stream.Target;
                            if (stream.Target === "00000000-0000-0000-0000-000000000000")
                                targetElementId = $scope.dataSourceIdPrefix + "Out";

                            var fromUuid = sourceElementId + "_out_" + stream.SourceOut;
                            var toUuid = targetElementId + "_in_" + stream.TargetIn;

                            var sourceEndpoint = $scope.jsPlumbInstance.getEndpoint(fromUuid);
                            var streamFound = false;
                            if (sourceEndpoint) {
                                angular.forEach(sourceEndpoint.connections, function(connection) {
                                    if (connection.endpoints[1].getUuid() === toUuid) {
                                        // when connection found, update it's label with the Entities-Count
                                        connection.setLabel({
                                            label: stream.Count.toString(),
                                            cssClass: "streamEntitiesCount"
                                        });
                                        streamFound = true;
                                        return;
                                    }
                                });
                            }

                            if (!streamFound)
                                $log.error("Stream not found", stream, sourceEndpoint);
                        });
                    };

                    // Ensure the Pipeline is saved
                    $scope.savePipeline().then(query);
                };

                // Clone the Pipeline
                $scope.clonePipeline = function() {
                    if (!confirm("Clone Pipeline " + $scope.PipelineEntityId + "?")) return;

                    // Clone and get new PipelineEntityId
                    var clone = function() {
                        return pipelineService.clonePipeline($scope.PipelineEntityId);
                    };
                    // Get the new Pipeline (Pipeline and DataSources)
                    var getClonePipeline = function(success) {
                        return pipelineService.getPipeline(success.EntityId /*EntityId*/);
                    };

                    // Save, clone, get clone, load clone
                    $scope.savePipeline(null).then(clone).then(getClonePipeline).then(pipelineSaved);
                };
            }]);
})();
// Config and Controller for the Pipeline Management UI
angular.module("PipelineManagement", [
    "EavServices",
    "EavConfiguration",
    "eavNgSvcs",
    "EavAdminUi"
]).
	controller("PipelineManagement", ["$modalInstance", "appId", "pipelineService", "eavAdminDialogs", "eavConfig", function ($modalInstance, appId, pipelineService, eavAdminDialogs, eavConfig) {
	    var vm = this;
        vm.appId = appId;

	    pipelineService.setAppId(appId);
	    pipelineService.initContentTypes();
	    // Make URL-Provider available to the scope
	    vm.getPipelineUrl = pipelineService.getPipelineUrl;

	    // Refresh List of Pipelines
	    vm.refresh = function () {
	        vm.pipelines = pipelineService.getPipelines(appId);
	    };
	    vm.refresh();

	    // Delete a Pipeline
        vm.delete = function(pipeline) {
            if (!confirm("Delete Pipeline \"" + pipeline.Name + "\" (" + pipeline.Id + ")?"))
                return;

            pipelineService.deletePipeline(pipeline.Id).then(function() {
                vm.refresh();
            }, function(reason) {
                alert(reason);
            });
        };

	    // Clone a Pipeline
        vm.clone = function(pipeline) {
            pipelineService.clonePipeline(pipeline.Id).then(function() {
                vm.refresh();
            }, function(reason) {
                alert(reason);
            });
        };

        vm.permissions = function (item) {
            return eavAdminDialogs.openPermissionsForGuid(appId, item.Guid);
        };

        vm.add = function add() {
            var items = [{
                    ContentTypeName: "DataPipeline",
                    Prefill: { TestParameters: eavConfig.pipelineDesigner.testParameters }
                }];
            eavAdminDialogs.openEditItems(items, vm.refresh);
        };

        vm.edit = function edit(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.Id, vm.refresh);
        }; 

        vm.design = function design(item) {
            return eavAdminDialogs.editPipeline(vm.appId, item.Id, vm.refresh);
        };
        vm.liveEval = function admin() {
            var inp = prompt("This is for very advanced operations. Only use this if you know what you're doing. \n\n Enter admin commands:");
            if (inp)
                eval(inp); // jshint ignore:line
        };
        vm.close = function () { $modalInstance.dismiss("cancel"); };
    }]);
/*jshint laxbreak:true */
(function() {

    angular.module("PipelineDesigner")
        .controller("QueryStats", ["testParams", "result", "$modalInstance", function (testParams, result, $modalInstance) {
                var vm = this;
                var success = result;
                vm.testParameters = testParams.split("\n");
                vm.timeUsed = success.QueryTimer.Milliseconds;
                vm.ticksUsed = success.QueryTimer.Ticks;
                vm.result = success.Query;

                vm.sources = success.Sources;
                vm.streams = success.Streams;

                vm.connections = "todo";


                vm.close = function () {
                    $modalInstance.dismiss("cancel");
                };



            }]
        );
})();
// Init the main eav services module
angular.module("EavServices", [
    "ng",                   // Angular for $http etc.
    "EavConfiguration",     // global configuration
    "pascalprecht.translate",
    "ngResource",           // only needed for the pipeline-service, maybe not necessary any more?
    "toaster"
]);

angular.module("EavServices")
    .factory("contentItemsSvc", ["$http", "entitiesSvc", "metadataSvc", "svcCreator", function($http, entitiesSvc, metadataSvc, svcCreator) {
            return function createContentItemsSvc(appId, contentType) {
                var svc = {};
                svc.contentType = contentType;

                svc.appId = appId;

                svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                    return $http.get("eav/entities/GetAllOfTypeForAdmin", { params: { appId: svc.appId, contentType: svc.contentType } });
                }));

                // delete, then reload
                svc.delete = function del(id) {
                    return entitiesSvc.delete(svc.contentType, id)
                        .then(svc.liveListReload);
                };

                // todo: should use the ContentTypeService instead
                svc.getColumns = function getColumns() {
                    return $http.get("eav/contenttype/getfields/", { params: { "appid": svc.appId, "staticName": svc.contentType } });
                };

                return svc;
            };
        }]
    );

angular.module("EavServices")
    .factory("contentTypeFieldSvc", ["$http", "eavConfig", "svcCreator", function($http, eavConfig, svcCreator) {
        return function createFieldsSvc(appId, contentType) {
            // start with a basic service which implement the live-list functionality
            var svc = {};
            svc.appId = appId;
            svc.contentType = contentType;

            svc.typeListRetrieve = function typeListRetrieve() {
                return $http.get("eav/contenttype/datatypes/", { params: { "appid": svc.appId } });
            };

	        svc.getFields = function getFields() {
		        return $http.get("eav/contenttype/getfields", { params: { "appid": svc.appId, "staticName": svc.contentType.StaticName } });
	        };

            svc = angular.extend(svc, svcCreator.implementLiveList(svc.getFields));

            svc.types = svcCreator.implementLiveList(svc.typeListRetrieve);


            svc.moveUp = function moveUp(item) {
                return $http.get("eav/contenttype/reorder", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id, direction: "up" } })
                    .then(svc.liveListReload);
            };
            svc.moveDown = function moveDown(item) {
                return $http.get("eav/contenttype/reorder", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id, direction: "down" } })
                    .then(svc.liveListReload);
            };

            svc.delete = function del(item) {
                return $http.delete("eav/contenttype/delete", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id } })
                    .then(svc.liveListReload);
            };

            svc.addMany = function add(items, count) {
                return $http.get("eav/contenttype/addfield/", { params: items[count] })
                    .then(function() {
                        if (items.length == ++count)
                            svc.liveListReload();
                        else
                            svc.addMany(items, count);
                    });
            };

            svc.add = function addOne(item) {
                return $http.get("eav/contenttype/addfield/", { params: item })
                    .then(svc.liveListReload);
            };

            svc.newItemCount = 0;
            svc.newItem = function newItem() {
                return {
                    AppId: svc.appId,
                    ContentTypeId: svc.contentType.Id,
                    Id: 0,
                    Type: "String",
                    StaticName: "",
                    IsTitle: svc.liveList().length === 0,
                    SortOrder: svc.liveList().length + svc.newItemCount++
                };
            };


            svc.delete = function del(item) {
                if (item.IsTitle)
                    throw "Can't delete Title";
                return $http.delete("eav/contenttype/deletefield", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id } })
                    .then(svc.liveListReload);
            };

            svc.setTitle = function setTitle(item) {
                return $http.get("eav/contenttype/setTitle", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id } })
                    .then(svc.liveListReload);
            };


            return svc;
        };
    }]);

angular.module("EavServices")
    .factory("contentTypeSvc", ["$http", "eavConfig", "svcCreator", function ($http, eavConfig, svcCreator) {
        return function appSpecificContentTypeSvc(appId, scope) {
            var svc = {};
            svc.scope = scope || eavConfig.contentType.defaultScope;
            svc.appId = appId;

            svc.retrieveContentTypes = function typeListRetrieve() {
                return $http.get("eav/contenttype/get/", { params: { "appid": svc.appId, "scope": svc.scope } });
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(svc.retrieveContentTypes));

            svc.getDetails = function getDetails(contentTypeName) {
                return $http.get("eav/contenttype/GetSingle", { params: { "appid": svc.appId, "contentTypeStaticName": contentTypeName } });
            };

            svc.newItem = function newItem() {
                return {
                    StaticName: "",
                    Name: "",
                    Description: "",
                    Scope: eavConfig.contentType.defaultScope
                };
            };

            svc.save = function save(item) {
                return $http.post("eav/contenttype/save/", item, { params: { appid: svc.appId } })
                    .then(svc.liveListReload);
            };

            svc.delete = function del(item) {
                return $http.delete("eav/contenttype/delete", { params: { appid: svc.appId, staticName: item.StaticName } })
                    .then(svc.liveListReload);
            };

            svc.setScope = function setScope(newScope) {
                svc.scope = newScope;
                svc.liveListReload();
            };
            return svc;
        };

    }]);
// uiDebug
// will add
// vm.debug -> shows if in debug mode - bind ng-ifs to this
// vm.maybeEnableDebug - a method which checks for ctrl+shift-click and if yes, changes debug state
//
// How to use
// 1. add uiDebug to your controller dependencies like:    contentImportController(appId, ..., debugState, $modalInstance, $filter)
// 2. add a line after creating your vm-object like:       vm.debug = debugState;
// 3. add a click event as far out as possible on html:    <div ng-click="vm.debug.autoEnableAsNeeded($event)">
// 4. wrap your hidden stuff in an ng-if:                  <div ng-if="vm.debug.on">

angular.module("EavServices")
    .factory("debugState", function () {
        var svc = {
            on: false
        };

        svc.autoEnableAsNeeded = function (evt) {
            evt = window.event || evt;
            var ctrlAndShiftPressed = evt.ctrlKey;
            if (ctrlAndShiftPressed)
                svc.on = !svc.on;
        };

        return svc;
    });
/*  this file contains a service to handle 
 * How it works
 * This service tries to open a modal dialog if it can, otherwise a new window returning a promise to allow
 * ...refresh when the window close. 
 * 
 * In most cases there is a nice command to open something, like openItemEditWithEntityId(id, callback)
 * ...and there is also a more advanced version where you could specify more closely what you wanted
 * ...usually ending with an X, so like openItemEditWithEntityIdX(resolve, callbacks)
 * 
 * the simple callback is 1 function (usually to refresh the main list), the complex callbacks have the following structure
 * 1. .success (optional)
 * 2. .error (optional) 
 * 3. .notify (optional)
 * 4. .close (optional) --> this one is attached to all events if no primary handler is defined 
 *  
 * How to use
 * 1. you must already include all js files in your main app - so the controllers you'll need must be preloaded
 * 2. Your main app must also  declare the other apps as dependencies, so angular.module('yourname', ['dialog 1', 'diolag 2'])
 * 3. your main app must also need this ['EavAdminUI']
 * 4. your controller must require eavAdminDialogs
 * 5. Then you can call such a dialog
 */


// Todo
// 1. Import / Export
// 2. Pipeline Designer

angular.module("EavAdminUi", ["ng",
    "ui.bootstrap",         // for the $modal etc.
    "EavServices",
    "eavTemplates",         // Provides all cached templates
    "PermissionsApp",       // Permissions dialogs to manage permissions
    "ContentItemsApp",      // Content-items dialog - not working atm?
    "PipelineManagement",   // Manage pipelines
    "ContentImportApp",
    "ContentExportApp",
    "HistoryApp",            // the item-history app
	"eavEditEntity"			// the edit-app
])
    .factory("eavAdminDialogs", ["$modal", "eavConfig", "contentTypeSvc", "$window", function ($modal, eavConfig, contentTypeSvc, $window) {

            var svc = {};

            //#region List of Content Items dialogs
            svc.openContentItems = function oci(appId, staticName, itemId, closeCallback) {
                var resolve = svc.CreateResolve({ appId: appId, contentType: staticName, contentTypeId: itemId });
                return svc.OpenModal("content-items/content-items.html", "ContentItemsList as vm", "xlg", resolve, closeCallback);
            };
            //#endregion

            //#region content import export
            svc.openContentImport = function ocimp(appId, staticName, closeCallback) {
                var resolve = svc.CreateResolve({ appId: appId, contentType: staticName });
                return svc.OpenModal("content-import-export/content-import.html", "ContentImport as vm", "lg", resolve, closeCallback);
            };

            svc.openContentExport = function ocexp(appId, staticName, closeCallback) {
                var resolve = svc.CreateResolve({ appId: appId, contentType: staticName });
                return svc.OpenModal("content-import-export/content-export.html", "ContentExport as vm", "lg", resolve, closeCallback);
            };

            //#endregion

            //#region ContentType dialogs

            svc.openContentTypeEdit = function octe(item, closeCallback) {
                var resolve = svc.CreateResolve({ item: item });
                return svc.OpenModal("content-types/content-types-edit.html", "Edit as vm", "sm", resolve, closeCallback);
            };

            svc.openContentTypeFields = function octf(item, closeCallback) {
                var resolve = svc.CreateResolve({ contentType: item });
                return svc.OpenModal("content-types/content-types-fields.html", "FieldList as vm", "lg", resolve, closeCallback);
            };
            //#endregion
        
            //#region Item - new, edit
            svc.openItemNew = function oin(contentTypeName, closeCallback) {
                return svc.openEditItems([{ ContentTypeName: contentTypeName }], closeCallback);
            };

            svc.openItemEditWithEntityId = function oie(entityId, closeCallback) {
                return svc.openEditItems([{ EntityId: entityId }], closeCallback);
            };

            svc.openEditItems = function oel(items, closeCallback) {
                var resolve = svc.CreateResolve({ items: items });
                return svc.OpenModal("form/main-form.html", "EditEntityWrapperCtrl as vm", "lg", resolve, closeCallback);
            };

            svc.openItemHistory = function ioh(entityId, closeCallback) {
                return svc.OpenModal("content-items/history.html", "History as vm", "lg",
                    svc.CreateResolve({ entityId: entityId }),
                    closeCallback);
            };
            //#endregion

            //#region Metadata - mainly new
            svc.openMetadataNew = function omdn(appId, targetType, targetId, metadataType, closeCallback) {
                var metadata = {};
                switch (targetType) {
                    case "entity":
                        metadata.Key = targetId;
                        metadata.KeyType = "guid";
                        metadata.TargetType = eavConfig.metadataOfEntity;
                        break;
                    case "attribute":
                        metadata.Key = targetId;
                        metadata.KeyType = "number";
                        metadata.TargetType = eavConfig.metadataOfAttribute;
                        break;
                    default: throw "targetType unknown, only accepts entity or attribute for now";
                }
                var items = [{
                    ContentTypeName: metadataType,
                    Metadata: metadata
                }];

                svc.openEditItems(items, closeCallback);
            };
            //#endregion

            //#region Permissions Dialog
            svc.openPermissionsForGuid = function opfg(appId, targetGuid, closeCallback) {
                var resolve = svc.CreateResolve({ appId: appId, targetGuid: targetGuid });
                return svc.OpenModal("permissions/permissions.html", "PermissionList as vm", "lg", resolve, closeCallback);
            };
            //#endregion

            //#region Pipeline Designer
            svc.editPipeline = function ep(appId, pipelineId, closeCallback) {
                var url = eavConfig.adminUrls.pipelineDesigner(appId, pipelineId);
                $window.open(url);
                return;
            };
            //#endregion



        //#region Internal helpers
            svc._attachCallbacks = function attachCallbacks(promise, callbacks) {
                if (typeof (callbacks) === "undefined")
                    return;
                if (typeof (callbacks) === "function") // if it's only one callback, use it for all close-cases
                    callbacks = { close: callbacks };
                return promise.result.then(callbacks.success || callbacks.close, callbacks.error || callbacks.close, callbacks.notify || callbacks.close);
            };

        // Will open a modal window. Has various specials, like
        // 1. If the templateUrl begins with "~/" - this will be re-mapped to the ng-app root. Only use this for not-inline stuff
        // 2. The controller can be written as "something as vm" and this will be split and configured corectly
            svc.OpenModal = function openModal(templateUrl, controller, size, resolveValues, callbacks) {
                var foundAs = controller.indexOf(" as ");
                var contAs = foundAs > 0 ?
                    controller.substring(foundAs + 4)
                    : null;
                if (foundAs > 0)
                    controller = controller.substring(0, foundAs);

                var modalInstance = $modal.open({
                    animation: true,
                    templateUrl: templateUrl,
                    controller: controller,
                    controllerAs: contAs,
                    size: size,
                    resolve: resolveValues
                });

                return svc._attachCallbacks(modalInstance, callbacks);
            };

        /// This will create a resolve-object containing return function()... for each property in the array
            svc.CreateResolve = function createResolve() {
                var fns = {}, list = arguments[0];
                for (var prop in list) 
                    if (list.hasOwnProperty(prop))
                        fns[prop] = svc._create1Resolve(list[prop]);
                return fns;
            };

            svc._create1Resolve = function (value) {
                return function () { return value; };
            };
        //#endregion


        return svc;
    }])

;
/*  this file contains various eav-angular services
 *  1. the basic configuration enforcing html5 mode
 */

angular.module("eavNgSvcs", ["ng"])

    /// Config to ensure that $location can work and give url-parameters
    .config(["$locationProvider", function ($locationProvider) {
            $locationProvider.html5Mode({
                enabled: true,
                requireBase: false
            });
        } ])


;


angular.module("EavServices")
    .factory("historySvc", ["$http", "svcCreator", function($http, svcCreator) { 
        //var eavConf = eavConfig;

        // Construct a service for this specific targetGuid
        return function createSvc(appId, entityId) {
            var svc = {
                appId: appId,
                entityId: entityId
            };

            // When we get the list, reverse-number the results to give it a nice version number
            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("eav/entities/history", { params: { "appId": svc.appId, "entityId": svc.entityId } })
                .then(function(result) {
                    var list = result.data;
                        for (var i = 0; i < list.length; i++)
                            list[i].VirtualVersion = list.length - i;
                        return result;
                    });
            }));

            svc.getVersionDetails = function getVersionDetails(changeId) {
                return $http.get("eav/entities/historydetails", { params: { "appId": svc.appId, "entityId": svc.entityId, "changeId": changeId } });
            };

            return svc;
        };
    }]);
/* The main component for language inclusion
 * Ensure the dependencies work, that the url-schema is prepared etc.
 * 
 */

(function () {
    angular.module("EavServices")

    .config(["$translateProvider", "languages", "$translatePartialLoaderProvider", function ($translateProvider, languages, $translatePartialLoaderProvider) {
            $translateProvider
                .preferredLanguage(languages.currentLanguage.split("-")[0])
                .useSanitizeValueStrategy("escapeParameters")   // this is very important to allow html in the JSON files
                .fallbackLanguage(languages.defaultLanguage.split("-")[0])

                .useLoader("$translatePartialLoader", {
                    urlTemplate: languages.i18nRoot + "{part}-{lang}.js" 
                })
                .useLoaderCache(true);              // should cache json
            $translatePartialLoaderProvider         // these parts are always required
                .addPart("admin")
                .addPart("edit");   
    }])

    // ensure that adding parts will load the missing files
    .run(["$rootScope", "$translate", function ($rootScope, $translate) {
        $rootScope.$on("$translatePartialLoaderStructureChanged", function () {
            $translate.refresh();
        });
    }])

    //.factory("")
    ;
})();
// By default, eav-controls assume that all their parameters (appId, etc.) are instantiated by the bootstrapper
// but the "root" component must get it from the url
// Since different objects could be the root object (this depends on the initial loader), the root-one must have
// a connection to the Url, but only when it is the root
// So the trick is to just include this file - which will provide values for the important attribute
//
// As of now, it only supplies
// * appId
(function () {
    angular.module("InitParametersFromUrl", [])
        //#region properties
        .factory("appId", function () {
            return getParameterByName("appId");
        })
        .factory("zoneId", function () {
            return getParameterByName("zoneId");
        })
        .factory("entityId", function () {
            return getParameterByName("entityid");
        })
        .factory("contentTypeName", function () {
            return getParameterByName("contenttypename");
        })

        .factory("pipelineId", function () {
            return getParameterByName("pipelineId");
        })
        .factory("dialog", function () {
            return getParameterByName("dialog");
        })
        //#endregion
        //#region helpers / dummy objects
        // This is a dummy object, because it's needed for dialogs
        .factory("$modalInstance", function () {
            return null;
        })
    //#endregion
    ;

    function getParameterByName(name) {
        if (window.$2sxc)
            return window.$2sxc.urlParams.get(name);
        return getParameterByNameDuplicate(name);
    }

    // this is a duplicate fn of the 2sxc-version, should only be used if 2sxc doesn't exist
    function getParameterByNameDuplicate(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var searchRx = new RegExp("[\\?&]" + name + "=([^&#]*)", "i");
        var results = searchRx.exec(location.search);

        if (results === null) {
            var hashRx = new RegExp("[#&]" + name + "=([^&#]*)", "i");
            results = hashRx.exec(location.hash);
        }

        // if nothing found, try normal URL because DNN places parameters in /key/value notation
        if (results === null) {
            // Otherwise try parts of the URL
            var matches = window.location.pathname.match(new RegExp("/" + name + "/([^/]+)", "i"));

            // Check if we found anything, if we do find it, we must reverse the results so we get the "last" one in case there are multiple hits
            if (matches !== null && matches.length > 1)
                results = matches.reverse()[0];
        } else
            results = results[1];

        return results === null ? "" : decodeURIComponent(results.replace(/\+/g, " "));
    }
}());
// metadata
// retrieves metadata for an entity or an attribute

angular.module("EavServices")
    /// Management actions which are rather advanced metadata kind of actions
    .factory("metadataSvc", ["$http", "appId", function($http, appId) {
        var svc = {};

        // Find all items assigned to a GUID
        svc.getMetadata = function getMetadata(assignedToId, keyGuid, contentTypeName) {
            return $http.get("eav/metadata/getassignedentities", {
                params: {
                    appId: appId,
                    assignmentObjectTypeId: assignedToId,
                    keyType: "guid",
                    key: keyGuid,
                    contentType: contentTypeName
                }
            });
        };
        return svc;
    }]);

angular.module("EavServices")
    .factory("permissionsSvc", ["$http", "eavConfig", "entitiesSvc", "metadataSvc", "svcCreator", "contentTypeSvc", function($http, eavConfig, entitiesSvc, metadataSvc, svcCreator, contentTypeSvc) {
        var eavConf = eavConfig;

        // Construct a service for this specific targetGuid
        return function createSvc(appId, permissionTargetGuid) {
            var svc = {
                PermissionTargetGuid: permissionTargetGuid,
                ctName: "PermissionConfiguration",
                ctId: 0,
                EntityAssignment: eavConf.metadataOfEntity,
                ctSvc: contentTypeSvc(appId)
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                // todo: refactor this - get out of the eavmanagemnetsvc
                return metadataSvc.getMetadata(svc.EntityAssignment, svc.PermissionTargetGuid, svc.ctName).then(svc.updateLiveAll);
            }));

            // Get ID of this content-type 
            svc.ctSvc.getDetails(svc.ctName).then(function (result) {
                svc.ctId = result.data.AttributeSetId;
            });

            // delete, then reload
            svc.delete = function del(id) {
                return entitiesSvc.delete(svc.ctName, id)
                    .then(svc.liveListReload);
            };
            return svc;
        };
    }]);
// PipelineService provides an interface to the Server Backend storing Pipelines and their Pipeline Parts

angular.module("EavServices")
    .factory("pipelineService", ["$resource", "$q", "$filter", "eavConfig", "$http", "contentTypeSvc", "metadataSvc", "eavAdminDialogs", function ($resource, $q, $filter, eavConfig, $http, contentTypeSvc, metadataSvc, eavAdminDialogs) {
        "use strict";
        var svc = {};
        // Web API Service
        svc.pipelineResource = $resource("eav/PipelineDesigner/:action");
        svc.entitiesResource = $resource("eav/Entities/:action");

        svc.dataPipelineAttributeSetId = 0;
        svc.appId = 0;

        // Get the Definition of a DataSource
        svc.getDataSourceDefinitionProperty = function (model, dataSource) {
        	var definition = $filter("filter")(model.InstalledDataSources, function (d) { return d.PartAssemblyAndType == dataSource.PartAssemblyAndType; })[0];
        	if (!definition)
        		throw "DataSource Definition not found: " + dataSource.PartAssemblyAndType;
        	return definition;
        };

        // todo refactor: why do we have 2 methods with same name?
        // Extend Pipeline-Model retrieved from the Server
        var postProcessDataSources = function(model) {
            // Append Out-DataSource for the UI
            model.DataSources.push({
                Name: eavConfig.pipelineDesigner.outDataSource.name,
                Description: eavConfig.pipelineDesigner.outDataSource.description,
                EntityGuid: "Out",
                PartAssemblyAndType: eavConfig.pipelineDesigner.outDataSource.className,
                VisualDesignerData: eavConfig.pipelineDesigner.outDataSource.visualDesignerData,
                ReadOnly: true
            });

            // Extend each DataSource with Definition-Property and ReadOnly Status
            angular.forEach(model.DataSources, function(dataSource) {
                dataSource.Definition = function() { return svc.getDataSourceDefinitionProperty(model, dataSource); };
                dataSource.ReadOnly = dataSource.ReadOnly || !model.Pipeline.AllowEdit;
            });
        };

        angular.extend(svc, { 
            
            // get a Pipeline with Pipeline Info with Pipeline Parts and Installed DataSources
            getPipeline: function(pipelineEntityId) {
                var deferred = $q.defer();

                var getPipeline = svc.pipelineResource.get({ action: "GetPipeline", id: pipelineEntityId, appId: svc.appId });
                var getInstalledDataSources = svc.pipelineResource.query({ action: "GetInstalledDataSources" });

                // Join and modify retrieved Data
                $q.all([getPipeline.$promise, getInstalledDataSources.$promise]).then(function(results) {
                    var model = JSON.parse(angular.toJson(results[0])); // workaround to remove AngularJS Promise from the result-Objects
                    model.InstalledDataSources = JSON.parse(angular.toJson(results[1]));

                    // Init new Pipeline Object
                    if (!pipelineEntityId) {
                        model.Pipeline = {
                            AllowEdit: "True"
                        };
                    }

                    // Add Out-DataSource for the UI
                    model.InstalledDataSources.push({
                        PartAssemblyAndType: eavConfig.pipelineDesigner.outDataSource.className,
                        ClassName: eavConfig.pipelineDesigner.outDataSource.className,
                        In: eavConfig.pipelineDesigner.outDataSource.in,
                        Out: null,
                        allowNew: false
                    });

                    postProcessDataSources(model);

                    deferred.resolve(model);
                }, function(reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },
            // Ensure Model has all DataSources and they're linked to their Definition-Object
            postProcessDataSources: function(model) {
                // stop Post-Process if the model already contains the Out-DataSource
                if ($filter("filter")(model.DataSources, function(d) { return d.EntityGuid == "Out"; })[0])
                    return;

                postProcessDataSources(model);
            },
            // Get a JSON for a DataSource with Definition-Property
            getNewDataSource: function(model, dataSourceBase) {
                return {
                    Definition: function() { return svc.getDataSourceDefinitionProperty(model, dataSourceBase); }
                };
            },
            // Save whole Pipline
            savePipeline: function(pipeline, dataSources) {
                if (!svc.appId)
                    return $q.reject("appId must be set to save a Pipeline");

                // Remove some Properties from the DataSource before Saving
                var dataSourcesPrepared = [];
                angular.forEach(dataSources, function(dataSource) {
                    var dataSourceClone = angular.copy(dataSource);
                    delete dataSourceClone.ReadOnly;
                    dataSourcesPrepared.push(dataSourceClone);
                });

                return svc.pipelineResource.save({
                    action: "SavePipeline",
                    appId: svc.appId,
                    Id: pipeline.EntityId /*id later EntityId */
                }, { pipeline: pipeline, dataSources: dataSourcesPrepared }).$promise;
            },
            // clone a whole Pipeline
            clonePipeline: function(pipelineEntityId) {
                return svc.pipelineResource.get({ action: "ClonePipeline", appId: svc.appId, Id: pipelineEntityId }).$promise;
            },


            // Get the URL to configure a DataSource
            editDataSourcePart: function(dataSource) {
                var dataSourceFullName = $filter("typename")(dataSource.PartAssemblyAndType, "classFullName");
                var contentTypeName = "|Config " + dataSourceFullName; // todo refactor centralize
                var assignmentObjectTypeId = 4; // todo refactor centralize
                var keyGuid = dataSource.EntityGuid;

                // Query for existing Entity
                metadataSvc.getMetadata(assignmentObjectTypeId, keyGuid, contentTypeName).then(function (result) { 
                    var success = result.data;
                    if (success.length) // Edit existing Entity
                        eavAdminDialogs.openItemEditWithEntityId(success[0].Id);
                    else { // Create new Entity
                        var items = [{
                                ContentTypeName: contentTypeName,
                                Metadata: {
                                    TargetType: assignmentObjectTypeId,
                                    KeyType: "guid",
                                    Key: keyGuid
                                }}];
                        eavAdminDialogs.openEditItems(items);
                    }
                });
            }

        });

        angular.extend(svc, {
            // Query the Data of a Pipeline
            queryPipeline: function (id) {
                return svc.pipelineResource.get({ action: "QueryPipeline", appId: svc.appId, id: id }).$promise;
            },
            // set appId and init some dynamic configurations
            setAppId: function (newAppId) {
                svc.appId = newAppId;
            },
            // Init some Content Types, currently only used for getPipelineUrl('new', ...)
            initContentTypes: function initContentTypes() {
                return contentTypeSvc(svc.appId).getDetails("DataPipeline").then(function (result) {
                    svc.dataPipelineAttributeSetId = result.data.AttributeSetId;
                });
            },
            // Get all Pipelines of current App
            getPipelines: function () {
                return svc.entitiesResource.query({ action: "GetEntities", appId: svc.appId, contentType: "DataPipeline" });
            },
            // Delete a Pipeline on current App
            deletePipeline: function (id) {
                return svc.pipelineResource.get({ action: "DeletePipeline", appId: svc.appId, id: id }).$promise;
            }
        });

        return svc;
    }]);
/*  this file contains the svcCreator - a helper to quickly create services
 */

angular.module("EavServices")
    // This is a helper-factory to create services which manage one live list
    // check examples with the permissions-service or the content-type-service how we use it
    .factory("svcCreator", function() {
        var creator = {};

        // construct a object which has liveListCache, liveListReload(), liveListReset(),  
        creator.implementLiveList = function(getLiveList) {
            var t = {};

            t.liveListCache = [];                   // this is the cached list
            t.liveListCache.isLoaded = false;

            t.liveList = function getAllLive() {
                if (t.liveListCache.length === 0)
                    t.liveListReload();
                return t.liveListCache;
            };

            // use a promise-result to re-fill the live list of all items, return the promise again
            t._liveListUpdateWithResult = function updateLiveAll(result) {
                t.liveListCache.length = 0; // clear
                for (var i = 0; i < result.data.length; i++)
                    t.liveListCache.push(result.data[i]);
                t.liveListCache.isLoaded = true;
                return result;
            };

            t.liveListSourceRead = getLiveList;

            t.liveListReload = function getAll() {
                return t.liveListSourceRead()
                    .then(t._liveListUpdateWithResult);
            };

            t.liveListReset = function resetList() {
                t.liveListCache = [];
            };

            return t;
        };
        return creator;

    })

;

angular.module("EavServices")
    .factory("uiNotification", ["toaster", function (toaster) {
            "use strict";

            var showNote = function (type, title, body, autoHide) {
                // wrap toaster in ready-Event because notes would't be show if teaster is used before
                angular.element(document).ready(function () {
                    toaster.clear();
                    toaster.pop(type, title, body, autoHide ? null : 0);
                });
            };

            return {
                clear: function () {
                    toaster.clear();
                },
                error: function (title, bodyOrError) {
                    var message;
                    // test whether bodyOrError is an Error from Web API
                    if (bodyOrError && bodyOrError.data && bodyOrError.data.Message) {
                        message = bodyOrError.data.Message;
                        if (bodyOrError.data.ExceptionMessage)
                            message += "\n" + bodyOrError.data.ExceptionMessage;
                    } else
                        message = bodyOrError;

                    showNote("error", title, message);
                },
                note: function (title, body, autoHide) {
                    showNote("note", title, body, autoHide);
                },
                success: function (title, body, autoHide) {
                    showNote("success", title, body, autoHide);
                },
                wait: function (title) {
                    showNote("note", title ? title : "Please wait ..", "This shouldn't take long", false);
                }
            };
        }]
    );