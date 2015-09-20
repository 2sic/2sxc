

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
                template: "<span class='btn btn-default btn-file'><span class='glyphicon glyphicon-open'></span><input type='file' ng-model='model[options.key]' base-sixty-four-input /></span><br /><span ng-if='model[options.key]'>{{model[options.key].filename}}</span>",
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
    angular.module("ContentImportApp", [
        "EavAdminUi",
        "EavDirectives",
        "EavServices",
        "ContentFormlyTypes"
    ]);
}());
(function () {

    angular.module("ContentImportApp")
        .controller("ContentImport", contentImportController);


    function contentImportController(appId, contentType, contentImportService, eavAdminDialogs, eavConfig, $modalInstance, $filter) {
        var translate = $filter("translate");

        var vm = this;

        vm.debug = {};

        vm.formValues = { };

        vm.formFields = [{
            // Content type
            key: "AppId",
            type: "hidden",
            defaultValue: appId
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
            // File references
            key: "FileReferences",
            type: "radio",
            expressionProperties: {
                "templateOptions.label": "'Content.Import.Fields.FileReferences.Label' | translate",
                "templateOptions.options": function () {
                    return [{
                        "name": translate("Content.Import.Fields.FileReferences.Options.Keep"),
                        "value": "Keep"
                    }, {
                        "name": translate("Content.Import.Fields.FileReferences.Options.Resolve"),
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

        vm.currentStep = "1"; // 1, 2, 3...

        vm.evaluateContent = function previewContent() {
            contentImportService.evaluateContent(vm.formValues).then(function (result) { vm.debug = result; });
        };

        vm.importContent = function importContent() {

        };

        vm.close = function close() {
            $modalInstance.dismiss("cancel");
        };
    }
    contentImportController.$inject = ["appId", "contentType", "contentImportService", "eavAdminDialogs", "eavConfig", "$modalInstance", "$filter"];
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

            return $http.post("eav/ContentImport/EvaluateContent", { AppId: args.AppId, ContentType: args.ContentType, ContentBase64: args.File.base64 });
        }

        function importContent(args) {

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
            if (confirm("Delete '" + "title-unknown-yet" + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
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
    function contentTypeListController(contentTypeSvc, eavAdminDialogs, appId, $translate) {
        var vm = this;
        var svc = contentTypeSvc(appId);

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

        vm.isGuid = function isGuid(txtToTest) {
            var patt = new RegExp(/[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i);
            return patt.test(txtToTest); // note: can't use the txtToTest.match because it causes infinite digest cycles
        };

        vm.permissions = function permissions(item) {
            return eavAdminDialogs.openPermissionsForGuid(svc.appId, item.StaticName, vm.refresh);
        };

        vm.openExport = function openExport() {
            return eavAdminDialogs.openContentExport(svc.appId);
        };

        vm.openImport = function openImport(item) {
            return eavAdminDialogs.openContentImport(svc.appId, item.StaticName, vm.refresh);
        };
    }
    contentTypeListController.$inject = ["contentTypeSvc", "eavAdminDialogs", "appId", "$translate"];

    /// Edit or add a content-type
    /// Note that the svc can also be null if you don't already have it, the system will then create its own
    function contentTypeEditController(appId, item, contentTypeSvc, $modalInstance) {
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
    contentTypeEditController.$inject = ["appId", "item", "contentTypeSvc", "$modalInstance"];

}());
(function () { 
    angular.module("ContentTypesApp")
        .controller("FieldList", contentTypeFieldListController)
        .controller("FieldsAdd", contentTypeFieldAddController)
    ;

    /// The controller to manage the fields-list
    function contentTypeFieldListController(appId, contentTypeFieldSvc, contentType, $modalInstance, $modal, eavAdminDialogs, $translate) {
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
            var exists = item.Metadata[metadataType] !== undefined;

            if (exists) {
                eavAdminDialogs.openItemEditWithEntityId(
                    item.Metadata[metadataType].Id,
                    svc.liveListReload);
            } else {
                eavAdminDialogs.openMetadataNew(appId, "attribute", item.Id, '@' + metadataType,
                    svc.liveListReload);
            }
        };
    }
    contentTypeFieldListController.$inject = ["appId", "contentTypeFieldSvc", "contentType", "$modalInstance", "$modal", "eavAdminDialogs", "$translate"];

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
    ""
  );


  $templateCache.put('content-import-export/content-import.html',
    "<div class=modal-header><button class=\"btn pull-right\" type=button icon=remove ng-click=vm.close()></button><h3 class=modal-title translate=Content.Import.Title translate-values=\"{currentStep: vm.currentStep}\"></h3></div><div class=modal-body><div ng-switch=vm.currentStep><div ng-switch-when=1><p translate=Content.Import.Help></p><formly-form form=vm.form model=vm.formValues fields=vm.formFields><p class=text-warning translate=Content.Import.Messages.BackupContentBefore></p><button type=button class=\"btn btn-primary\" ng-click=\"vm.evaluateContent(); vm.currentStep = 2\" ng-disabled=!vm.form.$valid translate=Content.Import.Commands.Preview></button> <button type=button class=\"btn btn-default\" ng-click=vm.close() translate=Content.Import.Commands.Close></button></formly-form></div><div ng-switch-when=2><p>TODO: Show the evaluation of the content</p><button type=button class=\"btn btn-primary\" ng-click=\"vm.importContent(); vm.currentStep = 3\" translate=Content.Import.Commands.Import></button> <button type=button class=\"btn btn-default\" ng-click=\"vm.currentStep = 1\" translate=Content.Import.Commands.Back></button> <button type=button class=\"btn btn-default\" ng-click=vm.close() translate=Content.Import.Commands.Close></button></div><div ng-switch-when=3><p>TODO: Show the result of import</p><button type=button class=\"btn btn-primary\" ng-click=\"vm.currentStep = 1\" translate=Content.Import.Commands.Back></button> <button type=button class=\"btn btn-default\" ng-click=vm.close() translate=Content.Import.Commands.Close></button></div></div><hr><pre>{{vm.debug | json}}</pre></div>"
  );


  $templateCache.put('content-items/content-edit.html',
    "<div class=modal-header><button type=button class=\"btn btn-default\" ng-click=vm.history()><span class=\"glyphicon glyphicon-time\">history / todo</span></button><h3 class=modal-title>Edit / New Content</h3></div><div class=modal-body>this is where the edit appears. Would edit entity {{vm.entityId}} or add a {{vm.contentType}} - depending on the mode: {{vm.mode}}<h3>Use cases</h3><ol><li>Edit an existing entity with ID</li><li>Create a new entity of a certaint content-type, just save and done (like from a \"new\" button without content-group)</li><li>Create a new entity of a certain type and assign it to a metadata thing (guid, int, string)</li><li>Create a new entity and put it into a content-group at the right place</li><li>Edit content-group: item + presentation</li><li>Edit multiple IDs/or new/mix: Edit multiple items with IDs</li></ol>init of 1 edit - entity-id in storage - new-type + optional: assignment-id + assignment-type - array of the above --- [{id 17}, {type: \"person\"}, {type: person, asstype: 4, target: 0205}] - content-group</div>"
  );


  $templateCache.put('content-items/content-items.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Content.Manage.Title></h3></div><div class=modal-body><button icon=plus type=button class=\"btn btn-default\" ng-click=vm.add()></button> <button icon=repeat type=button class=btn ng-click=vm.refresh()></button><div style=\"overflow: auto\"><table class=\"table table-striped table-hover\"><thead><tr><th translate=Content.Manage.Table.Id></th><th translate=Content.Manage.Table.Published></th><th translate=Content.Manage.Table.Title style=\"width: 200px\"></th><th translate=Content.Manage.Table.Actions></th><th ng-repeat=\"col in vm.dynamicColumns\">{{col.StaticName}}</th></tr></thead><tbody><tr ng-repeat=\"item in vm.items\"><td class=text-nowrap><span>{{item.Id}}</span></td><td class=text-nowrap><span class=glyphicon ng-class=\"{'glyphicon-ok-circle': item.IsPublished, 'glyphicon-ban-circle' : !item.IsPublished && !item.Published, 'glyphicon-record' : !item.IsPublished && item.Published }\" tooltip=\"{{ 'Content.Publish.' + (item.IsPublished ? 'PnV': item.Published ? 'DoP' : 'D') | translate }}\"></span> <span icon=\"{{ item.Draft ? 'paperclip' : item.Published ? 'export' : '' }}\" tooltip=\"{{ (item.Draft ? 'Content.Publish.HD' :'') | translate:'{ id: item.Draft.RepositoryId}' }}{{ (item.Published ? 'Content.Publish.HP' :'') | translate:'{ id: item.Published.RepositoryId}' }}\"></span></td><td><div style=\"height: 20px; width: 200px; position: relative; overflow: hidden; white-space: nowrap; text-overflow: ellipsis\" tooltip={{item.Title}}><a ng-click=vm.edit(item)>{{item.Title}}{{ (!item.Title ? 'Content.Manage.NoTitle':'') | translate }}</a></div></td><td><button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td><td ng-repeat=\"col in vm.dynamicColumns\"><div style=\"height: 20px; max-width: 100px; position: relative; overflow: hidden; text-overflow: ellipsis\" tooltip={{item[col.StaticName]}}>{{item[col.StaticName].toString().substring(0,25)}}</div></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div></div>"
  );


  $templateCache.put('content-items/history-details.html',
    "<div><div class=modal-header><button class=\"btn btn-primary pull-right\" type=button ng-click=vm.close()><span class=\"glyphicon glyphicon-remove\"></span></button><h3 class=modal-title>History Details {{vm.ChangeId}} of {{vm.entityId}}</h3></div><div class=modal-body><h1>todo</h1><table class=\"table table-striped table-hover\"><thead><tr><th>Field</th><th>Language</th><th>Value</th><th>SharedWith</th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:SysCreatedDate:reverse\"><td>{{item.Field}}</td><td>{{item.Language}}</td><td>{{item.Value}}</td><td>{{item.SharedWith}}</td></tr><tr ng-if=!vm.items.length><td colspan=100>No History</td></tr></tbody></table><button class=\"btn btn-primary pull-right\" type=button ng-click=vm.restore()><span class=\"glyphicon glyphicon-ok\">todo restore</span></button></div></div>"
  );


  $templateCache.put('content-items/history.html',
    "<div><div class=modal-header><button class=\"btn btn-primary pull-right\" type=button ng-click=vm.close()><span class=\"glyphicon glyphicon-remove\"></span></button><h3 class=modal-title>{{ \"Content.History.Title\" | translate:'{ id:vm.entityId }' }}History of {{vm.entityId}}</h3></div><div class=modal-body><table class=\"table table-striped table-hover\"><thead><tr><th translate=Content.History.Table.Id></th><th translate=Content.History.Table.When></th><th translate=Content.History.Table.User></th><th translate=Content.History.Table.Action></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:SysCreatedDate:reverse\"><td><span tooltip=\"ChangeId: {{item.ChangeId}}\">{{item.VirtualVersion}}</span></td><td>{{item.SysCreatedDate.replace(\"T\", \" \")}}</td><td>{{item.User}}</td><td><button type=button class=\"btn btn-xs\" ng-click=vm.details(item)><span class=\"glyphicon glyphicon-search\"></span></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div></div>"
  );


  $templateCache.put('content-types/content-types-edit.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=ContentTypeEdit.Title></h3></div><div class=modal-body>{{ \"ContentTypeEdit.Name\" | translate }}:<br><input ng-model=\"vm.item.Name\"><br>{{ \"ContentTypeEdit.Description\" | translate }}:<br><input ng-model=\"vm.item.Description\"><br>{{ \"ContentTypeEdit.Scope\" | translate }}:<br><input disabled ng-model=\"vm.item.Scope\"></div><div class=modal-footer><button icon=ok class=\"btn btn-primary\" type=button ng-click=vm.ok()></button></div>"
  );


  $templateCache.put('content-types/content-types-field-edit.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Fields.TitleEdit></h3></div><div class=modal-body><table class=\"table table-hover\"><thead><tr><th translate=Fields.Table.Name></th><th translate=Fields.Table.DataType>Data Type</th></tr></thead><tbody><tr ng-repeat=\"item in vm.items\"><td><input ng-model=item.StaticName ng-required=\"true\"></td><td><select ng-model=item.Type ng-options=\"o for o in vm.types track by o\"></select></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div><div class=modal-footer><button icon=ok class=\"btn btn-primary\" type=button ng-click=vm.ok()></button></div>"
  );


  $templateCache.put('content-types/content-types-fields.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Fields.Title></h3></div><div class=modal-body><button icon=plus ng-click=vm.add() class=\"btn btn-default\"></button><table class=\"table table-hover\"><thead><tr><th translate=Fields.Table.Title></th><th translate=Fields.Table.Name></th><th translate=Fields.Table.Edit></th><th translate=Fields.Table.Label></th><th translate=Fields.Table.Notes></th><th translate=Fields.Table.Sort></th><th translate=Fields.Table.Action></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy: 'SortOrder'\"><td><button icon=\"{{item.IsTitle ? 'star' : 'star-empty'}}\" type=button class=\"btn btn-xs\" ng-style=\"(item.IsTitle ? '' : 'color: transparent !important')\" ng-click=vm.setTitle(item)></button></td><td><span tooltip=\"{{ 'Id: ' + item.Id}}\">{{item.StaticName}}</span></td><td class=text-nowrap><button icon=pencil type=button class=\"btn btn-xs\" ng-click=\"vm.createOrEditMetadata(item, 'All')\" translate=Fields.General></button> <button icon=pencil type=button class=\"btn btn-xs\" ng-click=\"vm.createOrEditMetadata(item, item.Type)\">{{item.Type}}</button></td><td>{{item.Metadata['@All'].Attributes.Name.DefaultValue.TypedContents}}</td><td>{{item.Metadata['@All'].Attributes.Notes.DefaultValue.TypedContents}}</td><td class=text-nowrap><button icon=arrow-up type=button class=\"btn btn-xs\" ng-disabled=$first ng-click=vm.moveUp(item)></button> <button icon=arrow-down type=button class=\"btn btn-xs\" ng-disabled=$last ng-click=vm.moveDown(item)></button></td><td><button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div>"
  );


  $templateCache.put('content-types/content-types.html',
    "<div class=modal-header><h3 class=modal-title translate=ContentTypes.Title></h3></div><div class=modal-body><div ng-controller=\"List as vm\" class=ng-cloak><button icon=plus title=\"{{ 'General.Buttons.Add' | translate }}\" type=button class=\"btn btn-default\" ng-click=vm.edit()></button> <button icon=repeat title=\"{{ 'General.Buttons.Refresh' | translate }}\" type=button class=btn ng-click=vm.refresh()></button> <button icon=flash title=\"{{ 'General.Buttons.System' | translate }}\" type=button class=btn ng-click=vm.liveEval()></button><table class=\"table table-hover\"><thead><tr><th translate=ContentTypes.TypesTable.Name></th><th translate=ContentTypes.TypesTable.Description></th><th translate=ContentTypes.TypesTable.Fields></th><th translate=ContentTypes.TypesTable.Items></th><th translate=ContentTypes.TypesTable.Actions></th><th></th></tr></thead><tbody><tr ng-if=vm.items.isLoaded ng-repeat=\"item in vm.items | orderBy:'Name'\"><td><a ng-click=vm.edit(item) target=_self>{{item.Name}}</a></td><td>{{item.Description}}</td><td><button type=button class=\"btn btn-xs btn-default\" ng-click=vm.editFields(item)><span icon=cog>&nbsp;{{item.Fields}}</span></button></td><td><button type=button class=\"btn btn-xs btn-default\" ng-click=vm.editItems(item)><span icon=list>&nbsp;{{item.Items}}</span></button></td><td class=text-nowrap><button icon=export tooltip=\"{{ 'ContentTypes.Buttons.Export' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.openExport(item)></button> <button icon=import tooltip=\"{{ 'ContentTypes.Buttons.Import' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.openImport(item)></button> <button icon=user type=button class=\"btn btn-xs\" ng-click=vm.permissions(item) ng-if=vm.isGuid(item.StaticName)></button></td><td><button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td></tr><tr ng-if=!vm.items.length><td colspan=100>{{ 'General.Messages.Loading' | translate }} / {{ 'General.Messages.NothingFound' | translate }}</td></tr></tbody></table></div></div><div><h3>todo</h3><ol><li>get validators to work on all dialogs</li><li>this dialog doesn't refresh properly when I add/change stuff</li></ol></div>"
  );


  $templateCache.put('permissions/permissions.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Permissions.Title></h3></div><div class=modal-body><button icon=plus type=button class=\"btn btn-default\" ng-click=vm.add()></button> <button icon=repeat type=button class=btn ng-click=vm.refresh()></button><table class=\"table table-striped table-hover\"><thead><tr><th translate=Permissions.Table.Name></th><th translate=Permissions.Table.Id></th><th translate=Permissions.Table.Condition></th><th translate=Permissions.Table.Grant></th><th translate=Permissions.Table.Actions></th></tr></thead><tbody><tr ng-repeat=\"item in vm.items | orderBy:'Title'\"><td><a ng-click=vm.edit(item)>{{item.Title}}</a></td><td>{{item.Id}}</td><td>{{item.Condition}}</td><td>{{item.Grant}}</td><td><button icon=remove type=button class=\"btn btn-xs\" ng-click=vm.tryToDelete(item)></button></td></tr><tr ng-if=!vm.items.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div>"
  );


  $templateCache.put('pipelines/pipeline-designer.html',
    "<div class=ng-cloak><div ng-controller=PipelineDesignerController><div id=pipelineContainer><div ng-repeat=\"dataSource in pipelineData.DataSources\" datasource id=dataSource_{{dataSource.EntityGuid}} class=dataSource ng-attr-style=\"top: {{dataSource.VisualDesignerData.Top}}px; left: {{dataSource.VisualDesignerData.Left}}px\"><div class=configure ng-click=configureDataSource(dataSource) title=\"Configure this DataSource\" ng-if=!dataSource.ReadOnly><span class=\"glyphicon glyphicon-list-alt\"></span></div><div class=name title=\"Click to edit the Name\" ng-click=editName(dataSource)>{{dataSource.Name || '(unnamed)'}}</div><br><div class=description title=\"Click to edit the Description\" ng-click=editDescription(dataSource)>{{dataSource.Description || '(no description)'}}</div><br><div class=typename ng-attr-title={{dataSource.PartAssemblyAndType}}>Type: {{dataSource.PartAssemblyAndType | typename: 'className'}}</div><div class=ep title=\"Drag a new Out-Connection from here\" ng-if=!dataSource.ReadOnly><span class=\"glyphicon glyphicon-plus-sign\"></span></div><div class=\"delete glyphicon glyphicon-remove\" title=\"Delete this DataSource\" ng-click=remove($index) ng-if=!dataSource.ReadOnly></div></div></div><div class=\"actions panel panel-default\"><div class=panel-heading><span class=pull-left>Actions</span> <a href=http://2sxc.org/help class=\"btn btn-info btn-xs pull-right\" target=_blank><span class=\"glyphicon glyphicon-question-sign\"></span> Help</a></div><div class=panel-body><button type=button class=\"btn btn-primary btn-block\" ng-disabled=readOnly ng-click=savePipeline()><span class=\"glyphicon glyphicon-floppy-save\"></span> Save</button><select class=form-control ng-model=addDataSourceType ng-disabled=readOnly ng-change=addDataSource() ng-options=\"d.ClassName for d in pipelineData.InstalledDataSources | filter: {allowNew: '!false'} | orderBy: 'ClassName'\"><option value=\"\">-- Add DataSource --</option></select><button type=button class=\"btn btn-default btn-sm\" title=\"Query the Data of this Pipeline\" ng-click=queryPipeline()><span class=\"glyphicon glyphicon-play\"></span> Query</button> <button type=button class=\"btn btn-default btn-sm\" title=\"Clone this Pipeline with all DataSources and Configurations\" ng-click=clonePipeline() ng-disabled=!PipelineEntityId><span class=\"glyphicon glyphicon-share-alt\"></span> Clone</button> <button type=button class=\"btn btn-default btn-sm\" ng-click=editPipelineEntity()><span class=\"glyphicon glyphicon-pencil\"></span> Test Parameters</button> <button type=button class=\"btn btn-info btn-xs\" ng-click=toggleEndpointOverlays()><span class=\"glyphicon glyphicon-info-sign\"></span> {{showEndpointOverlays ? 'Hide' : 'Show' }} Overlays</button> <button type=button class=\"btn btn-info btn-xs\" ng-click=repaint()><span class=\"glyphicon glyphicon-repeat\"></span> Repaint</button> <button type=button class=\"btn btn-info btn-xs\" ng-click=toogleDebug()><span class=\"glyphicon glyphicon-info-sign\"></span> {{debug ? 'Hide' : 'Show'}} Debug Info</button></div></div><toaster-container></toaster-container><pre ng-if=debug>{{pipelineData | json}}</pre></div></div>"
  );


  $templateCache.put('pipelines/pipelines.html',
    "<div class=modal-header><button icon=remove class=\"btn pull-right\" type=button ng-click=vm.close()></button><h3 class=modal-title translate=Pipeline.Manage.Title></h3></div><div class=\"modal-body ng-cloak\"><div translate=Pipeline.Manage.Intro></div><div><button icon=plus type=button class=\"btn btn-default\" ng-click=vm.add()></button> <button icon=repeat type=button class=btn ng-click=vm.refresh()></button> <button icon=flash type=button class=btn ng-click=vm.liveEval()></button><table class=\"table table-striped table-hover\"><thead><tr><th translate=Pipeline.Manage.Table.Id></th><th translate=Pipeline.Manage.Table.Name></th><th translate=Pipeline.Manage.Table.Description></th><th translate=Pipeline.Manage.Table.Actions></th></tr></thead><tbody><tr ng-repeat=\"pipeline in vm.pipelines | orderBy:'Name'\"><td>{{pipeline.Id}}</td><td><a ng-click=vm.edit(pipeline)>{{pipeline.Name}}</a></td><td>{{pipeline.Description}}</td><td class=text-nowrap><button icon=edit title=\"{{ 'General.Buttons.Edit' | translate }}\" class=\"btn btn-xs btn-default\" ng-click=vm.design(pipeline)></button> <button icon=user title=\"{{ 'General.Buttons.Permissions' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.permissions(pipeline)></button> <button icon=duplicate title=\"{{ 'General.Buttons.Copy' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.clone(pipeline)></button> <button icon=remove title=\"{{ 'General.Buttons.Delete' | translate }}\" type=button class=\"btn btn-xs\" ng-click=vm.delete(pipeline)></button></td></tr><tr ng-if=!vm.pipelines.length><td colspan=100 translate=General.Messages.NothingFound></td></tr></tbody></table></div></div>"
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
    "toaster",
    "EavConfiguration",
    "eavDialogService",
    "EavServices",
    "eavTemplates"])

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
    }])

// Show Notifications using toaster
    .factory("uiNotification", [
        "toaster", function(toaster) {
            "use strict";

            var showNote = function(type, title, body, autoHide) {
                // wrap toaster in ready-Event because notes would't be show if teaster is used before
                angular.element(document).ready(function() {
                    toaster.clear();
                    toaster.pop(type, title, body, autoHide ? null : 0);
                });
            };

            return {
                clear: function() {
                    toaster.clear();
                },
                error: function(title, bodyOrError) {
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
                note: function(title, body, autoHide) {
                    showNote("note", title, body, autoHide);
                },
                success: function(title, body, autoHide) {
                    showNote("success", title, body, autoHide);
                },
                wait: function(title) {
                    showNote("note", title ? title : "Please wait ..", "This shouldn't take long", false);
                }
            };
        }
    ]);

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
angular.module("PipelineDesigner")

    .controller("PipelineDesignerController",
			
	["appId", "pipelineId", "$scope", "pipelineService", "$location", "$timeout", "$filter", "uiNotification", "eavDialogService", "$log", "eavConfig", "$q", function (appId, pipelineId, $scope, pipelineService, $location, $timeout, $filter, uiNotification, eavDialogService, $log, eavConfig, $q) {
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

		// Get Data from PipelineService (Web API)
		pipelineService.getPipeline($scope.PipelineEntityId).then(function (success) {
			$scope.pipelineData = success;
			$scope.readOnly = !success.Pipeline.AllowEdit;
			uiNotification.note("Ready", $scope.readOnly ? "This pipeline is read only" : "You can now design the Pipeline. \nNote that there are still a few UI bugs.\nVisit 2sxc.org/help for more.", true);

			// If a new Pipeline is made, init new Pipeline
			if (!$scope.PipelineEntityId || $scope.pipelineData.DataSources.length === 1)
				initNewPipeline();
		}, function (reason) {
			uiNotification.error("Loading Pipeline failed", reason);
		});

		// init new jsPlumb Instance
		jsPlumb.ready(function () {
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
			$scope.jsPlumbInstance.bind("connectionDetached", function (info) {
				if (info.targetId == $scope.dataSourceIdPrefix + "Out") {
					var fixedEndpoints = angular.element(info.target).scope().dataSource.Definition().In;
					var label = info.targetEndpoint.getOverlay("endpointLabel").label;
					if (fixedEndpoints.indexOf(label) == -1) {
						$timeout(function () {
							$scope.jsPlumbInstance.deleteEndpoint(info.targetEndpoint);
						});
					}
				}
			});
			// If a new connection is created, ask for a name of the In-Stream
			$scope.jsPlumbInstance.bind("connection", function (info) {
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

	        var dataSource = element.scope().dataSource;
	        var uuid = element.attr("id") + (isIn ? "_in_" : "_out_") + name;
	        var params = {
	            uuid: uuid,
	            enabled: !dataSource.ReadOnly || dataSource.EntityGuid == "Out" // Endpoints on Out-DataSource must be always enabled
	        };
	        var endPoint = $scope.jsPlumbInstance.addEndpoint(element, (isIn ? targetEndpoint : sourceEndpoint), params);
	        endPoint.getOverlay("endpointLabel").setLabel(name);
	    };

		// Initialize jsPlumb Connections once after all DataSources were created in the DOM
		$scope.connectionsInitialized = false;
		$scope.$on("ngRepeatFinished", function () {
			if ($scope.connectionsInitialized) return;

			// suspend drawing and initialise
			$scope.jsPlumbInstance.doWhileSuspended(function () {
				initWirings($scope.pipelineData.Pipeline.StreamWiring);
			});
			$scope.repaint();	// repaint so continuous connections are aligned correctly

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
	    };

		// Init a new Pipeline with DataSources and Wirings from Configuration
	    var initNewPipeline = function() {
	        angular.forEach(eavConfig.pipelineDesigner.defaultPipeline.dataSources, function(dataSource) {
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
	        var $this = $(this);
	        var offset = $this.offset();
	        var dataSource = $this.scope().dataSource;
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

	        uiNotification.wait();

	        pipelineService.getDataSourceConfigurationUrl(dataSource).then(function(url) {
	            uiNotification.clear();
	            eavDialogService.open({ url: url, title: "Configure DataSource " + dataSource.Name });
	        }, function(error) {
	            uiNotification.error("Open Configuration UI failed", error);
	        });
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
	            eavDialogService.open({
	                url: pipelineService.getPipelineUrl("edit", $scope.PipelineEntityId),
	                title: "Edit Test Values",
	                onClose: function() {
	                    pipelineService.getPipeline($scope.PipelineEntityId).then(pipelineSaved);
	                }
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
		var pipelineSaved = function (success) {
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
	                eavDialogService.open({
	                    title: "Query result",
	                    content: "<div><div>The Full result was logged to the Browser Console. Further down you'll find more debug-infos. </div>"
	                        + "<h3>Parameters used</h3><div>" + ($scope.pipelineData.Pipeline.TestParameters.length > 5 ? $scope.pipelineData.Pipeline.TestParameters.replace("\n", "<br>") : "no test params specified") + "</div> "
	                        + "<h3>Query result - executed in " + success.QueryTimer.Milliseconds + "ms (" + success.QueryTimer.Ticks + "tx)</h3><div> <pre id=\"pipelineQueryResult\">" + $filter("json")(success.Query) + "</pre>" + showConnectionTable(success) + "</div>"
	                        + "</div"
	                });
	                $timeout(function() {
	                    showEntityCountOnStreams(success);
	                });
	                $log.debug(success);
	            }, function(reason) {
	                uiNotification.error("Query failed", reason);
	            });
	        };

	        // Create html-table with connection debug-info
	        var showConnectionTable = function(result) {
	            var srcTbl = "<h3>Sources</h3>" +
	                "<table><tr><th>Guid</th><th>Type</th><th>Config</th></tr>";
	            var src = result.Sources;
	            for (var s in src) {
	                if (s[0] != "$") {
	                    srcTbl += "<tr><td><pre>" + s.substring(0, 13) + "...</pre></td><td>" + src[s].Type + "</td><td>";
	                    var cnf = src[s].Configuration;
	                    for (var c in cnf)
	                        if (c[0] != "$")
	                            srcTbl += "<b>" + c + "</b>" + "=" + cnf[c] + "</br>";
	                    srcTbl += "</td></tr>";
	                }
	            }
	            srcTbl += "</table>";


	            srcTbl += "<h3>Streams</h3>" +
	                "<table><tr><th>Source</th><th>Target</th><th>Items</th><th>Err</th></tr>";
	            src = result.Streams;
	            for (var sr in src) {
	                if (sr[0] != "$") {
	                    srcTbl += "<tr><td><pre>"
	                        + src[sr].Source.substring(0, 13) + ":" + src[sr].SourceOut + "</pre></td><td><pre>"
	                        + src[sr].Target.substring(0, 13) + ":" + src[sr].TargetIn + "</pre></td><td>"
	                        + src[sr].Count + "</td><td>"
	                        + src[sr].Error + "</td></tr>";
	                }
	            }
	            srcTbl += "</table>";

	            return srcTbl;
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

// Config and Controller for the Pipeline Management UI
angular.module("PipelineManagement", [
    "EavServices",
    "EavConfiguration",
    "eavNgSvcs",
    "EavAdminUi"
]).
	controller("PipelineManagement", ["$modalInstance", "appId", "pipelineService", "eavAdminDialogs", function ($modalInstance, appId, pipelineService, eavAdminDialogs) {
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
            eavAdminDialogs.openItemNew(pipelineService.dataPipelineAttributeSetId, vm.refresh);
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
// todo: get rid of this

// it's only used in the pipeline-designer-controller, and seems to be to open modal-dialogs to edit something

// won't be necessary any more when Formly works

angular.module('eavDialogService', ["EavConfiguration"])
    .factory('eavDialogService', ["eavConfig", function (eavConfig) {
		return {
			open: function (params) {

				params = $.extend({
					url: "",
					width: 950,
					height: 600,
					onClose: function () { },
					title: null
				}, params);

				if (window.top.EavEditDialogs === null)
					window.top.EavEditDialogs = [];

				var dialogElement;
				if (params.url)
					dialogElement = '<div id="EavNewEditDialog"' + window.top.EavEditDialogs.length + '"><iframe style="position:absolute; top:0; right:0; left:0; bottom:0; height:100%; width:100%; border:0" src="' + params.url + '"></iframe></div>';
				else if (params.content)
					dialogElement = params.content;
				else
					dialogElement = '<div>no url and no content specified</div>';

				window.top.jQuery(dialogElement).dialog({
					title: params.title,
					autoOpen: true,
					modal: true,
					width: params.width,
					dialogClass: eavConfig.dialogClass,
					height: params.height,
					close: function (event, ui) {
						$(this).remove();
						params.onClose();
						window.top.EavEditDialogs.pop();
					}
				});

				window.top.EavEditDialogs.push(dialogElement);
			}
		};
	}]
);
// Init the main eav services module
angular.module("EavServices", [
    "ng",                   // Angular for $http etc.
    "EavConfiguration",     // global configuration
    "pascalprecht.translate",
    "ngResource",           // only needed for the pipeline-service, maybe not necessary any more?
]);

angular.module("EavServices")
    .factory("contentItemsSvc", ["$http", "entitiesSvc", "eavManagementSvc", "svcCreator", function($http, entitiesSvc, eavManagementSvc, svcCreator) {
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
    "HistoryApp",            // the item-history app
	"eavEditEntity"			// the edit-app
])
    .factory("eavAdminDialogs", ["$modal", "eavConfig", "eavManagementSvc", "contentTypeSvc", "$window", function ($modal, eavConfig, eavManagementSvc, contentTypeSvc, $window) {

        var svc = {};

        //#region Content Items dialogs
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
                //if (useDummyContentEditor) {
                    return svc.openItemEditWithEntityIdX(svc.CreateResolve({ mode: "new", entityId: null, contentTypeName: contentTypeId}), closeCallback );
                //} else {
                //    var url = eavConfig.itemForm.getNewItemUrl(contentTypeId);
                //    return PromiseWindow.open(url).then(null, function(error) { if (error === "closed") closeCallback(); });
                //}
            };

            svc.openItemEditWithEntityId = function oie(entityId, closeCallback) {
                //if (useDummyContentEditor) {
                    return svc.openItemEditWithEntityIdX(svc.CreateResolve({ mode: "edit", entityId: entityId, contentTypeName:null }), closeCallback );
                //} else {
                //    var url = eavConfig.itemForm.getEditItemUrl(entityId, undefined, true);
                //    return PromiseWindow.open(url).then(null, function(error) { if (error == "closed") closeCallback(); });
                //}
            };

            svc.openItemEditWithEntityIdX = function oieweix(resolve, callbacks) {
            	return svc.OpenModal("wrappers/edit-entity-wrapper.html", "EditEntityWrapperCtrl as vm", "lg", resolve, callbacks);
            };

            svc.openItemHistory = function ioh(entityId, closeCallback) {
                return svc.OpenModal("content-items/history.html", "History as vm", "lg",
                    svc.CreateResolve({ entityId: entityId }),
                    closeCallback);
            };
            

        //#endregion

        //#region Metadata - mainly new
            svc.openMetadataNew = function omdn(appId, targetType, targetId, metadataType, closeCallback) {
                var key = {};//, assignmentType;
                switch (targetType) {
                    case "entity":
                        key.keyGuid = targetId;
                        key.assignmentType = eavConfig.metadataOfEntity;
                        break;
                    case "attribute":
                        key.keyNumber = targetId;
                        key.assignmentType = eavConfig.metadataOfAttribute;
                        break;
                    default: throw "targetType unknown, only accepts entity or attribute";
                }
                // return eavManagementSvc.getContentTypeDefinition(metadataType)
                return contentTypeSvc(appId).getDetails(metadataType)
                    .then(function (result) {
                    //if (useDummyContentEditor) {
                        var resolve = svc.CreateResolve({ mode: "new", entityId: null, contentTypeName: metadataType });
                        alert(metadataType);
                        resolve = angular.extend(resolve, svc.CreateResolve(key));
                        return svc.openItemEditWithEntityIdX(resolve, { close: closeCallback });
                    //} else {

                    //    var attSetId = result.data.AttributeSetId;
                    //    var url = eavConfig.itemForm
                    //        .getNewItemUrl(attSetId, assignmentType, key, false);

                    //    return PromiseWindow.open(url).then(null, function(error) { if (error == "closed") closeCallback(); });
                    //}
                });
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
 *  2. a management-dialog which simply gets the appid if in the url
 *  3. eavManagementSvc - provides some services to retrieve metadata and similar for eav-management dialogs
 *  4. svcCreator - a helper to quickly create services
 *  5. entitiesSvc - a service to get/delete entities
 */

angular.module("eavNgSvcs", ["ng"])

    /// Config to ensure that $location can work and give url-parameters
    .config(["$locationProvider", function ($locationProvider) {
            $locationProvider.html5Mode({
                enabled: true,
                requireBase: false
            });
        } ])

    /// Provide state-information related to the current open dialog
    .factory("eavManagementDialog", ["$location", function($location){
        var result = {};
        var srch = $location.search();
        result.appId = srch.AppId || srch.appId || srch.appid;
        return result;
    }])

    /// Management actions which are rather advanced metadata kind of actions
    .factory("eavManagementSvc", ["$http", "eavManagementDialog", function($http, eavManagementDialog) {
        var svc = {};

        // Retrieve extra content-type info
        svc.getContentTypeDefinition = function getContentTypeDefinition(contentTypeName) {
            alert("using the wrong method - should use the content-type controller. Will work for now, change code please");
            return $http.get("eav/contenttype/get", { params: { appId: eavManagementDialog.appId, contentTypeId: contentTypeName } });
        };

        // Find all items assigned to a GUID
        svc.getAssignedItems = function getAssignedItems(assignedToId, keyGuid, contentTypeName) {
            return $http.get("eav/metadata/getassignedentities", {
                params: {
                    appId: eavManagementDialog.appId,
                    assignmentObjectTypeId: assignedToId,
                    keyType: "guid",
                    key: keyGuid,
                    contentType: contentTypeName
                }
            });
        };
        return svc;
    }])

    /// Standard entity commands like get one, many etc.
    .factory("entitiesSvc", ["$http", "eavManagementDialog", function ($http, eavManagementDialog) {
        var svc = {};

        svc.get = function get(contentType, id) {
            return id ?
                $http.get("eav/entities/getone", { params: { 'contentType': contentType, 'id': id, 'appId': eavManagementDialog.appId } })
                : $http.get("eav/entities/getentities", { params: { 'contentType': contentType, 'appId': eavManagementDialog.appId }});
        };

		svc.getMultiLanguage = function getMultiLanguage(appId, contentType, id) {
			return $http.get("eav/entities/getone", { params: { contentType: contentType, id: id, appId: appId, format: "multi-language" } });
		};

        svc.delete = function del(type, id) {
            return $http.delete("eav/entities/delete", {
                params: {
                    'contentType': type,
                    'id': id,
                    'appId': eavManagementDialog.appId
                }
            });
        };

		svc.newEntity = function(contentTypeName) {
			return {
				Id: null,
				Guid: null,
				Type: {
					Name: contentTypeName
				},
				Attributes: {}
			};
		};
        
        return svc;
    }])

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
    }]);
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

angular.module("EavServices")
    .factory("permissionsSvc", ["$http", "eavConfig", "entitiesSvc", "eavManagementSvc", "svcCreator", "contentTypeSvc", function($http, eavConfig, entitiesSvc, eavManagementSvc, svcCreator, contentTypeSvc) {
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
                return eavManagementSvc.getAssignedItems(svc.EntityAssignment, svc.PermissionTargetGuid, svc.ctName).then(svc.updateLiveAll);
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
    .factory("pipelineService", ["$resource", "$q", "$filter", "eavConfig", "$http", "contentTypeSvc", function($resource, $q, $filter, eavConfig, $http, contentTypeSvc) {
        "use strict";
        var svc = {};
        // Web API Service
        svc.pipelineResource = $resource("eav/PipelineDesigner/:action");
        svc.entitiesResource = $resource("eav/Entities/:action");

        svc.dataPipelineAttributeSetId = 0;
        svc.appId = 0;

        // Get the Definition of a DataSource
        svc.getDataSourceDefinitionProperty = function(model, dataSource) {
            return $filter("filter")(model.InstalledDataSources, function(d) { return d.PartAssemblyAndType == dataSource.PartAssemblyAndType; })[0];
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
            getDataSourceConfigurationUrl: function(dataSource) {
                var dataSourceFullName = $filter("typename")(dataSource.PartAssemblyAndType, "classFullName");
                var contentTypeName = "|Config " + dataSourceFullName; // todo refactor centralize
                var assignmentObjectTypeId = 4; // todo refactor centralize
                var keyGuid = dataSource.EntityGuid;
                var preventRedirect = true;

                var deferred = $q.defer();

                // Query for existing Entity
                svc.entitiesResource.query({ action: "GetAssignedEntities", appId: svc.appId, assignmentObjectTypeId: assignmentObjectTypeId, keyType: "guid", key: keyGuid, contentType: contentTypeName }, function (success) {
                    if (success.length) // Edit existing Entity
                        deferred.resolve(eavConfig.itemForm.getEditItemUrl(success[0].Id /*EntityId*/, null, preventRedirect));
                    else { // Create new Entity
                        // todo: this is a get-content-type, it shouldn't be using the entitiesResource
                        // todo: but I'm not sure when it is being used
                        svc.entitiesResource.get({ action: "GetContentType", appId: svc.appId, contentType: contentTypeName }, function (contentType) {
                            // test for "null"-response
                            if (contentType[0] == "n" && contentType[1] == "u" && contentType[2] == "l" && contentType[3] == "l")
                                deferred.reject("Content Type " + contentTypeName + " not found.");
                            else
                                deferred.resolve(eavConfig.itemForm.getNewItemUrl(contentType.AttributeSetId, assignmentObjectTypeId, { KeyGuid: keyGuid, ReturnUrl: null }, preventRedirect));
                        }, function(reason) {
                            deferred.reject(reason);
                        });
                    }
                }, function(reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

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