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

    contentExportController.$inject = ["appId", "contentType", "itemIds", "contentExportService", "eavAdminDialogs", "eavConfig", "languages", "$uibModalInstance", "$filter", "$translate"];
    angular.module("ContentExportApp")
        .controller("ContentExport", contentExportController);

    function contentExportController(appId, contentType, itemIds, contentExportService, eavAdminDialogs, eavConfig, languages, $uibModalInstance, $filter, $translate) {

        var vm = this;

        vm.formValues = {};

        // check if we were given some IDs to export only that
        var hasIdList = (Array.isArray(itemIds) && itemIds.length > 0);
        var cSelection = "Selection";

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
                        "name": $translate.instant("Content.Export.Fields.Language.Options.All"),
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
                    var opts = [{
                        "name": $translate.instant("Content.Export.Fields.RecordExport.Options.Blank"),
                        "value": "Blank"
                    }, {
                        "name": $translate.instant("Content.Export.Fields.RecordExport.Options.All"),
                        "value": "All"
                    }];
                    if (hasIdList)
                        opts.push({
                            "name": $translate.instant("Content.Export.Fields.RecordExport.Options.Selection", { count: itemIds.length }), // "todo: selected " + itemIds.length + " items",
                            "value": cSelection
                        });
                    return opts;
                }
            },
            defaultValue: hasIdList ? cSelection : "All"
        }, {
            // Language references
            key: "LanguageReferences",
            type: "radio",
            expressionProperties: {
                "templateOptions.label": "'Content.Export.Fields.LanguageReferences.Label' | translate",
                "templateOptions.disabled": function () {
                    return vm.formValues.RecordExport === "Blank";
                },
                "templateOptions.options": function () {
                    return [{
                        "name": $translate.instant("Content.Export.Fields.LanguageReferences.Options.Link"),
                        "value": "Link"
                    }, {
                        "name": $translate.instant("Content.Export.Fields.LanguageReferences.Options.Resolve"),
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
                    return vm.formValues.RecordExport === "Blank";
                },
                "templateOptions.options": function () {
                    return [{
                        "name": $translate.instant("Content.Export.Fields.ResourcesReferences.Options.Link"),
                        "value": "Link"
                    }, {
                        "name": $translate.instant("Content.Export.Fields.ResourcesReferences.Options.Resolve"),
                        "value": "Resolve"
                    }];
                }
            },
            defaultValue: "Link"
        }];


        vm.exportContent = function exportContent() {
            contentExportService.exportContent(vm.formValues, hasIdList && vm.formValues.RecordExport === cSelection ? itemIds : null);
        };

        vm.close = function close() {
            $uibModalInstance.dismiss("cancel");
        };
    }
}());
(function () {

    contentExportService.$inject = ["$http", "eavConfig"];
    angular.module("ContentExportApp")
         .factory("contentExportService", contentExportService);


    function contentExportService($http, eavConfig) {
        var srvc = {
            exportContent: exportContent
        };
        return srvc;

        function exportContent(args, selectedIds) {
            var url = eavConfig.getUrlPrefix("api") + "/eav/ContentExport/ExportContent",
                addids = selectedIds ? "&selectedids=" + selectedIds.join() : "",
                fullUrl = url
                    + "?appId=" + args.AppId
                    + "&language=" + args.Language
                    + "&defaultLanguage=" + args.DefaultLanguage
                    + "&contentType=" + args.ContentType
                    + "&recordExport=" + args.RecordExport
                    + "&resourcesReferences=" + args.ResourcesReferences
                    + "&languageReferences=" + args.LanguageReferences
                    + addids;

            window.open(fullUrl, "_blank" /* "_self" */, "");
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
                template: "<span class='btn btn-default btn-square btn-file'><span class='glyphicon glyphicon-open'></span><input type='file' ng-model='model[options.key]' base-sixty-four-input /></span> <span ng-if='model[options.key]'>{{model[options.key].filename}}</span>",
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

    contentImportController.$inject = ["appId", "contentType", "contentImportService", "eavAdminDialogs", "eavConfig", "languages", "debugState", "$uibModalInstance", "$filter", "$translate"];
    angular.module("ContentImportApp")
        .controller("ContentImport", contentImportController);

    function contentImportController(appId, contentType, contentImportService, eavAdminDialogs, eavConfig, languages, debugState, $uibModalInstance, $filter, $translate) {

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
                        "name": $translate.instant("Content.Import.Fields.ResourcesReferences.Options.Keep"),
                        "value": "Keep"
                    }, {
                        "name": $translate.instant("Content.Import.Fields.ResourcesReferences.Options.Resolve"),
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
                        "name": $translate.instant("Content.Import.Fields.ClearEntities.Options.None"),
                        "value": "None"
                    }, {
                        "name": $translate.instant("Content.Import.Fields.ClearEntities.Options.All"),
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
            $uibModalInstance.dismiss("cancel");
        };
    }
}());
(function () {

    contentImportService.$inject = ["$http"];
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
}());
(function () { 

    EditContentItemController.$inject = ["mode", "entityId", "contentType", "eavAdminDialogs", "$uibModalInstance"];
    angular.module("ContentEditApp", [
        "EavServices",
        "EavAdminUi"
    ])
        .controller("EditContentItem", EditContentItemController)
        ;

    function EditContentItemController(mode, entityId, contentType, eavAdminDialogs, $uibModalInstance) { //}, contentTypeId, eavAdminDialogs) {
        var vm = this;
        vm.mode = mode;
        vm.entityId = entityId;
        vm.contentType = contentType;
        vm.TestMessage = "Test message the controller is binding correctly...";

        vm.history = function history() {
            return eavAdminDialogs.openItemHistory(vm.entityId);
        };

        vm.close = function () { $uibModalInstance.dismiss("cancel"); };
    }

} ());
(function () {
	'use strict';

	contentItemsListController.$inject = ["contentItemsSvc", "eavConfig", "appId", "contentType", "eavAdminDialogs", "toastr", "debugState", "$uibModalInstance", "$uibModalStack", "$q", "$translate", "entitiesSvc", "agGridFilters"];
	angular.module("ContentItemsAppAgnostic", [
		"EavConfiguration",
		"EavAdminUi",
		"EavServices"
		// "agGrid" // needs this, but can't hardwire the dependency as it would cause problems with lazy-loading
	])
		.controller("ContentItemsList", contentItemsListController)
		;

	function contentItemsListController(contentItemsSvc, eavConfig, appId, contentType, eavAdminDialogs, toastr, debugState, $uibModalInstance, $uibModalStack, $q, $translate, entitiesSvc, agGridFilters) {
		/* jshint validthis:true */
		var vm = angular.extend(this, {
			debug: debugState,
			gridOptions: {
				enableSorting: true,
				enableFilter: true,
				rowHeight: 39,
				colWidth: 155,
				headerHeight: 38,
				angularCompileRows: true
			},
			add: add,
			refresh: setRowData,
			openExport: openExport,
			tryToDelete: tryToDelete,
			openDuplicate: openDuplicate,
			close: close,
			debugFilter: showFilter
		});
		var svc;

		var staticColumns = [
			{
				headerName: "ID",
				field: "Id",
				width: 50,
				template: '<span tooltip-append-to-body="true" uib-tooltip="Id: {{data.Id}}\nRepoId: {{data._RepositoryId}}\nGuid: {{data.Guid}}" ng-bind="data.Id"></span>',
				cellClass: "clickable",
				filter: 'number',
				onCellClicked: openEditDialog
			},
			{
				headerName: "Status",
				field: "IsPublished",
				width: 75,
				suppressSorting: true,
				template: '<span class="glyphicon" '
				+ 'ng-class="{\'glyphicon-eye-open\': data.IsPublished, \'glyphicon-eye-close\' : !data.IsPublished}" '
				+ 'tooltip-append-to-body="true" uib-tooltip="{{ \'Content.Publish.\' + (data.IsPublished ? \'PnV\': data.IsPublishedEntity ? \'DoP\' : \'D\') | translate }}"></span>'

				+ ' <span icon="{{ data.DraftEntity || data.PublishedEntity ? \'link\' : \'\' }}" '
				+ 'tooltip-append-to-body="true" '
				+ 'uib-tooltip="{{ (data.DraftEntity ? \'Content.Publish.HD\' :\'\') | translate:\'{ id: data.DraftEntity._RepositoryId }\' }} {{ data.DraftEntity._RepositoryId }}\n{{ (data.PublishedEntity ? \'Content.Publish.HP\' :\'\') | translate }} {{ data.PublishedEntity._RepositoryId }}"></span> <span ng-if="data.Metadata" tooltip-append-to-body="true" uib-tooltip="Metadata for type {{ data.Metadata.TargetType}}, id {{ data.Metadata.KeyNumber }}{{ data.Metadata.KeyString }}{{ data.Metadata.KeyGuid }}" icon="tag"></span>',
				valueGetter: valueGetterStatusField
			},
			{
				headerName: "Title",
				field: "_Title", 
				width: 216,
				cellClass: "clickable",
				template: '<span tooltip-append-to-body="true" uib-tooltip="{{data._Title}}" ng-bind="data._Title + \' \' + ((!data._Title ? \'Content.Manage.NoTitle\':\'\') | translate)"></span>',
				filter: 'text',
				onCellClicked: openEditDialog
			},
			{
				headerName: "",
				width: 70,
				suppressSorting: true,
				suppressMenu: true,
				template: '<button type="button" class="btn btn-xs btn-square" ng-click="vm.openDuplicate(data)" tooltip-append-to-body="true" uib-tooltip="{{ \'General.Buttons.Copy\' | translate }}">'
				+ '<i icon="duplicate"></i>'
				+ '</button> '
				+ '<button type="button" class="btn btn-xs btn-square" ng-click="vm.tryToDelete(data, false)" tooltip-append-to-body="true" uib-tooltip="{{ \'General.Buttons.Delete\' | translate }}">'
				+ '<i icon="remove"></i> '
				+ '</button>'
			}
		];

		activate();

		function activate() {
			svc = contentItemsSvc(appId, contentType);

			// set RowData an Column Definitions
			$q.all([setRowData(), svc.getColumns()])
				.then(function (success) {
					var columnDefs = getColumnDefs(success[1].data);
					vm.gridOptions.api.setColumnDefs(columnDefs);

					// resize outer modal (if needed)
					var bodyWidth = vm.gridOptions.api.gridPanel.eBodyContainer.clientWidth;
					var viewportWidth = vm.gridOptions.api.gridPanel.eBodyViewport.clientWidth;
					if (bodyWidth < viewportWidth)
						setModalWidth(bodyWidth);

					// try to apply some initial filters...
					var filterModel = agGridFilters.get();//
					vm.gridOptions.api.setFilterModel(filterModel);
				});
		}

		function showFilter() {
			var savedModel = vm.gridOptions.api.getFilterModel();
			console.log("current filter: ", savedModel);
			alert('check console for filter information');
		}

		// set width of outer angular-ui-modal. This is a quick and dirty solution because there's no official way to do this.
		// $uibModalStack.getTop() might get a wrong modal Instance
		// setting the width with inline css in a controller should be avoided
		function setModalWidth(width) {
			var modalDomEl = $uibModalStack.getTop().value.modalDomEl;
			var modalDialog = modalDomEl.children();
			modalDialog.css("width", (width + 47) + "px");	// add some pixels for padding and scrollbars
		}

		function add() {
			eavAdminDialogs.openItemNew(contentType, setRowData);
		}

		function openExport() {
			// check if there is a filter attached
			var ids = null,
				hasFilters = false,
				mod = vm.gridOptions.api.getFilterModel();

			// check if any filters are applied
			for (var prop in mod)
				if (mod.hasOwnProperty(prop)) {
					hasFilters = true;
					break;
				}

			if (hasFilters) {
				ids = [];
				vm.gridOptions.api.forEachNodeAfterFilterAndSort(function (rowNode) {
					ids.push(rowNode.data.Id);
				});
				if (ids.length === 0)
					ids = null;
			}

			// open export but DONT do a refresh callback, because it delays working with the table even though this is export only
			return eavAdminDialogs.openContentExport(appId, contentType, null, ids);
		}

		function openEditDialog(params) {
			eavAdminDialogs.openItemEditWithEntityId(params.data.Id, setRowData);
		}

		// Get/Update Grid Row-Data
		function setRowData() {
			var sortModel = {};
			var filterModel = {};
			if (vm.gridOptions.api) {
				sortModel = vm.gridOptions.api.getSortModel();
				filterModel = vm.gridOptions.api.getFilterModel();
			}

			return svc.liveListSourceRead().then(function (success) {
				vm.gridOptions.api.setRowData(success.data);
				vm.gridOptions.api.setSortModel(sortModel);
				vm.gridOptions.api.setFilterModel(filterModel);
			});
		}

		// get Grid Column-Definitions from an Array of EAV-Attributes
		function getColumnDefs(eavAttributes) {
			var columnDefs = staticColumns;

			angular.forEach(eavAttributes, function (eavAttribute) {
				if (eavAttribute.IsTitle) {
					staticColumns[2].eavAttribute = eavAttribute;
					return;	// don't add Title-Field twice
				}

				var colDef = {
					eavAttribute: eavAttribute,
					headerName: eavAttribute.StaticName,
					field: eavAttribute.StaticName,
					cellRenderer: cellRendererDefault,
					filterParams: { cellRenderer: cellRendererDefaultFilter }
				};


				switch (eavAttribute.Type) {
					case "Entity":
						try {
							colDef.allowMultiValue = eavAttribute.Metadata.Entity.AllowMultiValue;
						} catch (e) {
							colDef.allowMultiValue = true;
						}

						colDef.cellRenderer = cellRendererEntity;
						colDef.valueGetter = valueGetterEntityField;
						break;
					case "DateTime":
						try {
							colDef.useTimePicker = eavAttribute.Metadata.DateTime.UseTimePicker;
						} catch (e) {
							colDef.useTimePicker = false;
						}
						colDef.valueGetter = valueGetterDateTime;
						break;
					case "Boolean":
						colDef.valueGetter = valueGetterBoolean;
						break;
					case "Number":
						colDef.filter = 'number';
						break;
				}

				columnDefs.push(colDef);
			});

			return columnDefs;
		}

		//#region Column Value-Getter and Cell Renderer
		function valueGetterEntityField(params) {
			var rawValue = params.data[params.colDef.field];
			if (rawValue.length === 0)
				return null;

			return rawValue.map(function (item) {
				return item._Title;
			});
		}

		function valueGetterStatusField(params) {
			return [
				params.data.IsPublished ? "is published" : "is not published",
				params.data.Metadata ? "is metadata" : "is not metadata"
			];
		}

		function valueGetterDateTime(params) {
			var rawValue = params.data[params.colDef.field];
			if (!rawValue)
				return null;

			// remove 'Z' and replace 'T'
			return params.colDef.useTimePicker ? rawValue.substr(0, 19).replace('T', ' ') : rawValue.substr(0, 10);
		}

		function valueGetterBoolean(params) {
			var rawValue = params.data[params.colDef.field];
			if (typeof rawValue != "boolean")
				return null;

			return rawValue.toString();
		}

		function cellRendererDefault(params) {
			if (typeof (params.value) != "string" || params.value === null)
				return params.value;

			var encodedValue = htmlEncode(params.value);
			return '<span ng-non-bindable><span title="' + encodedValue + '">' + encodedValue + '</span></span>';
		}

		function cellRendererDefaultFilter(params) {
			return cellRendererDefault(params) || "(empty)";
		}

		// htmlencode strings (source: http://stackoverflow.com/a/7124052)
		function htmlEncode(text) {
			return text.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
		}

		function cellRendererEntity(params) {
			if (!Array.isArray(params.value))
				return null;

			var encodedValue = htmlEncode(params.value.join(", "));
			var result = '<span title="' + encodedValue + '">';
			if (params.colDef.allowMultiValue)
				result += '<span class="badge badge-primary">' + params.value.length + '</span> ';
			result += encodedValue + '</span>';

			return result;
		}
		// #endregion

		function tryToDelete(item) {
			entitiesSvc.tryDeleteAndAskForce(contentType, item._RepositoryId, item._Title).then(setRowData);
		}

		function openDuplicate(item) {
			var items = [{
				ContentTypeName: contentType,
				DuplicateEntity: item.Id
			}];
			eavAdminDialogs.openEditItems(items, svc.liveListReload);
		}

		function close() {
			$uibModalInstance.dismiss("cancel");
		}
	}
}());
(function () {
    'use strict';

    angular.module("ContentItemsAppAgnostic")
        .factory("agGridFilters", function() {
            return {
                get: function() {
                    if (!window.$2sxc) return {};
                    var urlFilters = window.$2sxc.urlParams.get("filters"), filters = null;
                    if (!urlFilters) return {};

                    // special decode if parameter was passed as base64 - this is necessary for strings containing the "+" character
                    if (urlFilters.charAt(urlFilters.length - 1) === "=") 
                        urlFilters = atob(urlFilters);
                    
                    try {
                        filters = JSON.parse(urlFilters);
                        console.log("found filters for this list:", filters);
                    } catch (e) {
                        console.log("can't parse json with filters from url: ", urlFilters);
                    }
                    if (!filters)
                        return {};

                    // check if there is a IsPublished filter, handle the special cases
                    if (filters.IsPublished === true)
                        filters.IsPublished = ["is published"];
                    else if (filters.IsPublished === false)
                        console.warn("filter ispublished = false is not implemented yet");

                    if (typeof filters.IsMetadata !== "undefined") {
                        // ensure that IsPublished is an array, in case we add Metadata-filters
                        if (!Array.isArray(filters.IsPublished))
                            filters.IsPublished = [];
                        filters.IsPublished.push(filters.IsMetadata ? "is metadata" : "is not metadata");
                        delete filters.IsMetadata;
                    }

                    // catch simple number filters, convert into ag-grid format
                    for (var field in filters)
                        if (filters.hasOwnProperty(field) && typeof filters[field] === "number")
                            filters[field] = { filter: filters[field], type: 1 };
                        
                    

                    console.log("will try to apply filter: ", filters);
                    return filters;
                    //{
                    //    // IsPublished: ["is published"],
                    //    ImageFormat: "w"
                    //}
                }
            };
        });
}());
(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    HistoryController.$inject = ["appId", "entityId", "historySvc", "$uibModalInstance", "$uibModal"];
    HistoryDetailsController.$inject = ["changeId", "dataSvc", "$uibModalInstance"];
    angular.module("HistoryApp", [
        "EavServices",
        "EavConfiguration",
        "eavTemplates",
    ])
        .controller("History", HistoryController)
        .controller("HistoryDetails", HistoryDetailsController)
        ;

    function HistoryController(appId, entityId, historySvc, $uibModalInstance, $uibModal) {
        var vm = this;
        var svc = historySvc(appId, entityId);
        vm.entityId = entityId;
        vm.items = svc.liveList();

        vm.close = function () { $uibModalInstance.dismiss("cancel"); };

        vm.details = function(item) {
            $uibModal.open({
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

    function HistoryDetailsController(changeId, dataSvc, $uibModalInstance) {
        var vm = this;
        alert("not implemented yet");
        var svc = dataSvc;

        svc.getVersionDetails(changeId).then(function(result) {
            alert(result.data);
            vm.items = result.data;
        });
        // vm.items = svc.liveList();

        vm.close = function () { $uibModalInstance.dismiss("cancel"); };
    }
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
(function() {

    contentTypeListController.$inject = ["contentTypeSvc", "eavAdminDialogs", "appId", "debugState", "$translate", "eavConfig"];
    angular.module("ContentTypesApp")
        .controller("List", contentTypeListController);


    /// Manage the list of content-types
    function contentTypeListController(contentTypeSvc, eavAdminDialogs, appId, debugState, $translate, eavConfig) {
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

        vm.createGhost = function createGhost() {
            var sourceName = window.prompt("to create a ghost content-type enter source static name / id - this is a very advanced operation - read more about it on 2sxc.org/help?tag=ghost");
            if (!sourceName)
                return;
            svc.createGhost(sourceName);
        };

        vm.editFields = function editFields(item) {
            eavAdminDialogs.openContentTypeFields(item, vm.refresh);
        };

        vm.editItems = function editItems(item) {
            eavAdminDialogs.openContentItems(svc.appId, item.StaticName, item.Id, vm.refresh);
        };

        vm.addItem = function(contentType) {
            eavAdminDialogs.openItemNew(contentType, vm.refresh);
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

        //#region metadata for this type - new 2016-09-07

        // Edit / Add metadata to a specific field
        vm.createOrEditMetadata = function createOrEditMetadata(item) {
            // assemble an array of items for editing
            var items = [vm.createItemDefinition(item, "ContentType")];
            eavAdminDialogs.openEditItems(items, svc.liveListReload);
        };
        
        vm.createItemDefinition = function createItemDefinition(item, metadataType) {
            var title = "ContentType Metadata"; // todo: i18n
            return item.Metadata  // check if it already has metadata
                ? { EntityId: item.Metadata.Id, Title: title }  // if defined, return the entity-number to edit
                : {
                    ContentTypeName: metadataType,        // otherwise the content type for new-assegnment
                    Metadata: {
                        Key: item.StaticName,
                        KeyType: "string",
                        TargetType: eavConfig.metadataOfContentType
                    },
                    Title: title,
                    Prefill: { Label: item.Name, Description: item.Description }
                };
        };
        //#endregion
    }
}());
(function() {

    contentTypeEditController.$inject = ["appId", "item", "contentTypeSvc", "debugState", "$translate", "$uibModalInstance"];
    angular.module("ContentTypesApp")
        .controller("Edit", contentTypeEditController);

    /// Edit or add a content-type
    /// Note that the svc can also be null if you don't already have it, the system will then create its own
    function contentTypeEditController(appId, item, contentTypeSvc, debugState, $translate, $uibModalInstance) {
        var vm = this;
        var svc = contentTypeSvc(appId);

        vm.debug = debugState;

        vm.item = item;
        vm.item.ChangeStaticName = false;
        vm.item.NewStaticName = vm.item.StaticName; // in case you really, really want to change it

        vm.ok = function () {
            svc.save(item).then(function() {
                $uibModalInstance.close(vm.item);              
            });
        };

        vm.close = function () {
            $uibModalInstance.dismiss("cancel");
        };
    }

}());
(function () {
    /*jshint laxbreak:true */
    contentTypeFieldEditController.$inject = ["appId", "svc", "item", "$filter", "$uibModalInstance"];
    angular.module("ContentTypesApp")
        .controller("FieldEdit", contentTypeFieldEditController)
    ;

    /// This is the main controller for adding a field
    /// Add is a standalone dialog, showing 10 lines for new field names / types
    function contentTypeFieldEditController(appId, svc, item, $filter, $uibModalInstance) {
        var vm = this;

        vm.items = [item];

        vm.types = svc.types.liveList();

        vm.allInputTypes = svc.getInputTypesList();

        vm.resetSubTypes = function resetSubTypes(item) {
            item.InputType = item.Type.toLowerCase() + "-default";
        };

        vm.ok = function () {
            svc.updateInputType(vm.items[0]);
            $uibModalInstance.close();
        };

        vm.close = function() { $uibModalInstance.dismiss("cancel"); };
    }
}());
(function () {
    /*jshint laxbreak:true */
    contentTypeFieldsAddController.$inject = ["appId", "svc", "$filter", "$uibModalInstance"];
    angular.module("ContentTypesApp")
        .controller("FieldsAdd", contentTypeFieldsAddController)
    ;

    /// This is the main controller for adding a field
    /// Add is a standalone dialog, showing 10 lines for new field names / types
    function contentTypeFieldsAddController(appId, svc, $filter, $uibModalInstance) {
        var vm = this;

        // prepare empty array of up to 10 new items to be added
        var nw = svc.newItem;
        vm.items = [nw(), nw(), nw(), nw(), nw(), nw(), nw(), nw(), nw(), nw()];

        vm.item = svc.newItem();
        vm.types = svc.types.liveList();

        vm.allInputTypes = svc.getInputTypesList();
        //svc.getInputTypes().then(function (result) {
        //    function addToList(value, key) {
        //        var item = {
        //            dataType: value.Type.substring(0, value.Type.indexOf("-")),
        //            inputType: value.Type, 
        //            label: value.Label,
        //            description: value.Description
        //        };
        //        vm.allInputTypes.push(item);
        //    }

        //    angular.forEach(result.data, addToList);

        //    vm.allInputTypes = $filter("orderBy")(vm.allInputTypes, ["dataType", "inputType"]);
        //});

        vm.resetSubTypes = function resetSubTypes(item) {
            item.InputType = item.Type.toLowerCase() + "-default";
        };

        vm.ok = function () {
            var items = vm.items;
            var newList = [];
            for (var c = 0; c < items.length; c++)
                if (items[c].StaticName)
                    newList.push(items[c]);
            svc.addMany(newList, 0);
            $uibModalInstance.close();
        };

        vm.close = function() { $uibModalInstance.dismiss("cancel"); };
    }
}());
/*jshint laxbreak:true */
(function () {
    contentTypeFieldListController.$inject = ["appId", "contentTypeFieldSvc", "contentType", "$uibModalInstance", "$uibModal", "eavAdminDialogs", "$filter", "$translate", "eavConfig", "$scope"];
    angular.module("ContentTypesApp")
        .controller("FieldList", contentTypeFieldListController)
        ;

    /// The controller to manage the fields-list
    function contentTypeFieldListController(appId, contentTypeFieldSvc, contentType, $uibModalInstance, $uibModal, eavAdminDialogs, $filter, $translate, eavConfig, $scope) {
        var vm = this;
        var svc = contentTypeFieldSvc(appId, contentType);

        // to close this dialog
        vm.close = function () {
            $uibModalInstance.dismiss("cancel");
        };

        vm.items = svc.liveList();

        vm.orderList = function () {
            var orderList = [];
            vm.items.map(function (e, i) {
                orderList.push(e.Id);
            });
            return orderList;
        };

        vm.treeOptions = {
            dropped: function () {
                vm.dragEnabled = false; // Disable drag while updating (causes strange effects like duplicate items)
                svc.reOrder(vm.orderList()).then(function () {
                    vm.dragEnabled = true;
                });
            }
        };

        vm.dragEnabled = true;

        // Open an add-dialog, and add them if the dialog is closed
        vm.add = function add() {
            $uibModal.open({
                animation: true,
                templateUrl: "content-types/content-types-fields-add.html",
                controller: "FieldsAdd",
                controllerAs: "vm",
                size: "lg",
                resolve: {
                    svc: function () { return svc; }
                }
            });
        };

        vm.edit = function edit(item) {
            $uibModal.open({
                animation: true,
                templateUrl: "content-types/content-types-field-edit.html",
                controller: "FieldEdit",
                controllerAs: "vm",
                size: "lg",
                resolve: {
                    svc: function () { return svc; },
                    item: function () { return item; }
                }
            });

        };

        vm.inputTypeTooltip = function (inputType) {
            if (inputType !== "unknown")
                return inputType;

            return "unknown means it's using an old definition for input-types - edit it to use the new definition";
        };

        // Actions like moveUp, Down, Delete, Title
        //vm.moveUp = svc.moveUp;
        //vm.moveDown = svc.moveDown;
        vm.setTitle = svc.setTitle;

        vm.tryToDelete = function tryToDelete(item) {
            if (item.IsTitle)
                return $translate(["General.Messages.CantDelete", "General.Terms.Title"], { target: "{0}" }).then(function (translations) {
                    alert(translations["General.Messages.CantDelete"].replace("{0}", translations["General.Terms.Title"]));
                });

            return $translate("General.Questions.Delete", { target: "'" + item.StaticName + "' (" + item.Id + ")" }).then(function (msg) {
                if (confirm(msg))
                    svc.delete(item);
            });
        };

        vm.rename = function rename(item) {
            $translate("General.Questions.Rename", { target: "'" + item.StaticName + "' (" + item.Id + ")" }).then(function (msg) {
                var newName = prompt(msg);
                if (newName)
                    svc.rename(item, newName);
            });
        };

        // Edit / Add metadata to a specific fields
        vm.createOrEditMetadata = function createOrEditMetadata(item, metadataType) {
            // assemble an array of 2 items for editing
            var items = [
                vm.createItemDefinition(item, "All"),
                vm.createItemDefinition(item, metadataType),
                vm.createItemDefinition(item, item.InputType)
            ];
            eavAdminDialogs.openEditItems(items, svc.liveListReload);
        };

        vm.createItemDefinition = function createItemDefinition(item, metadataType) {
            var title = metadataType === "All" ? $translate.instant("DataType.All.Title") : metadataType;
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

}());

(function () {
    /* jshint laxbreak:true*/

angular.module("EavDirectives", [])
    .directive("icon", function() {
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
    .directive("stopEvent", function() {
        return {
            restrict: "A",
            link: function(scope, element, attr) {
                if (attr && attr.stopEvent)
                    element.bind(attr.stopEvent, function(e) {
                        e.stopPropagation();
                    });
            }
        };
    })
    .directive("showDebugAvailability", ["eavConfig", function (eavConfig) {
        return {
            restrict: "E",
            scope: {},
            template: "<span class=\"debug-indicator low-priority\" ng-class='{ \"debug-enabled\": debugState.on }' "
            + "uib-tooltip=\"{{ 'AdvancedMode.Info.Available' | translate }} \n" + eavConfig.versionInfo + "\" "
            + "ng-click='askForLogging()'>"
                + "&pi;"
            + "</span><br/>",
            controller: ["$scope", "debugState", "toastr", function ($scope, debugState, toastr) {
                $scope.debugState = debugState;

                function askLogging() {
                    var duration = prompt("enable extended logging? type desired duration in minutes:", 1);
                    if (duration === null || duration === undefined) return;
                    debugState.enableExtendedLogging(duration).then(function (res) {
                        console.log(res.data);
                        toastr.info(res.data, { timeOut: 1000 });
                    });
                }


                $scope.askForLogging = function () {
                    if (!debugState.on) return;
                    askLogging();
                };
            }]
        };
    }])

    ;


})();
(function () { 

    permissionListController.$inject = ["permissionsSvc", "eavAdminDialogs", "eavConfig", "appId", "targetGuid", "$uibModalInstance"];
    angular.module("PermissionsApp", [
        "EavServices",
        "EavConfiguration",
        "EavAdminUi"])
        .controller("PermissionList", permissionListController)
        ;

    function permissionListController(permissionsSvc, eavAdminDialogs, eavConfig, appId, targetGuid, $uibModalInstance /* $location */) {
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
            if (confirm("Delete '" + item.Title + "' (" + item.Id + ") ?")) // todo: probably change .Title to ._Title
                svc.delete(item.Id);
        };

        vm.close = function () {
            $uibModalInstance.dismiss("cancel");
        };
    }

} ());
angular.module("PipelineDesigner",
    [
        "PipelineDesigner.filters",
        "ngResource",
        "EavConfiguration",
        "EavServices",
        "eavTemplates",
        "eavNgSvcs",
        "EavAdminUi",
        "eavEditEntity"
    ]);



angular.module("PipelineDesigner")
    // datasource directive makes an element a DataSource with jsPlumb
    .directive('datasource', ["$timeout", function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element) {
                // make this a DataSource when the DOM is ready
                $timeout(function () {
                    scope.makeDataSource(scope.dataSource, element);
                });
                if (scope.$last === true) {
                    $timeout(function () {
                        scope.$emit("ngRepeatFinished");
                    });
                }
            }
        };
    }]);

// AngularJS Controller for the >>>> Pipeline Designer

(function() {
    /*jshint laxbreak:true */

    var editName = function(dataSource) {
        if (dataSource.ReadOnly) return;

        var newName = prompt("Rename DataSource", dataSource.Name);
        if (newName && newName.trim())
            dataSource.Name = newName.trim();
    };

    // Edit Description of a DataSource
    var editDescription = function(dataSource) {
        if (dataSource.ReadOnly) return;

        var newDescription = prompt("Edit Description", dataSource.Description);
        if (newDescription && newDescription.trim())
            dataSource.Description = newDescription.trim();
    };

    // helper method because we don't have jQuery any more to find the offset
    function getElementOffset(element) {
        var de = document.documentElement;
        var box = element.getBoundingClientRect();
        var top = box.top + window.pageYOffset - de.clientTop;
        var left = box.left + window.pageXOffset - de.clientLeft;
        return { top: top, left: left };
    }


    angular.module("PipelineDesigner")
        .controller("PipelineDesignerController",
            ["appId", "pipelineId", "$scope", "pipelineService", "$location", "debugState", "$timeout", "ctrlS", "$filter", "toastrWithHttpErrorHandling", "eavAdminDialogs", "$log", "eavConfig", "$q", "getUrlParamMustRefactor", "queryDef", "plumbGui", function(appId,
                pipelineId,
                $scope,
                pipelineService,
                $location,
                debugState,
                $timeout,
                ctrlS,
                $filter,
                toastrWithHttpErrorHandling,
                eavAdminDialogs,
                $log,
                eavConfig,
                $q,
                getUrlParamMustRefactor,
                queryDef,
                plumbGui) {


                "use strict";
                // Init
                var vm = this;
                vm.debug = debugState;
                vm.warnings = [];
                var toastr = toastrWithHttpErrorHandling;
                $scope.debug = false;
                $scope.queryDef = queryDef;
                pipelineService.setAppId(appId);

                // fully re-initialize a query (at start, or later re-load)
                vm.reInitQuery = function() {
                    // Get Data from PipelineService (Web API)
                    var waitMsg = toastr.info("This shouldn't take long", "Loading...");
                    return queryDef.loadQuery()
                        .then(function() {
                                toastr.clear(waitMsg);
                                refreshWarnings(queryDef.data, vm);
                            },
                            function(reason) {
                                toastr.error(reason, "Loading query failed");
                            });
                };

                function activate() {
                    // add ctrl+s to save
                    vm.saveShortcut = ctrlS(function() { vm.savePipeline(); }); 
                    vm.reInitQuery();
                }

                activate();


                // make a DataSource with Endpoints, called by the datasource-Directive (which uses a $timeout)
                $scope.makeDataSource = function (dataSource, element) {
                    plumbGui.makeSource(dataSource, element, $scope.dataSourceDrag);
                    queryDef.dsCount++; // unclear what this is for, probably to name/number new sources
                };



                // Initialize jsPlumb Connections once after all DataSources were created in the DOM
                $scope.$on("ngRepeatFinished",
                    function() {
                        if (plumbGui.connectionsInitialized) return;

                        plumbGui.instance.batch(plumbGui.initWirings); // suspend drawing and initialise
                        $scope.repaint(); // repaint so continuous connections are aligned correctly

                        plumbGui.connectionsInitialized = true;
                    });


                vm.addSelectedDataSource = function() {
                    var partAssemblyAndType = $scope.addDataSourceType.PartAssemblyAndType;
                    queryDef.addDataSource(partAssemblyAndType, null, null, $scope.addDataSourceType.Name);
                    $scope.addDataSourceType = null; // reset dropdown
                    $scope.savePipeline();
                };

                // Delete a DataSource
                vm.remove = function(index) {
                    var dataSource = queryDef.data.DataSources[index];
                    if (!confirm("Delete DataSource \"" + (dataSource.Name || "(unnamed)") + "\"?")) return;
                    var elementId = plumbGui.dataSrcIdPrefix + dataSource.EntityGuid;
                    plumbGui.instance.selectEndpoints({ element: elementId }).remove();
                    queryDef.data.DataSources.splice(index, 1);
                };

                // Edit name & description of a DataSource
                $scope.editName = editName;
                $scope.editDescription = editDescription;

                // Update DataSource Position on Drag
                $scope.dataSourceDrag = function(draggedWrapper) {
                    var offset = getElementOffset(draggedWrapper.el);
                    var dataSource = plumbGui.findDataSourceOfElement(draggedWrapper.el);
                    $scope.$apply(function() {
                        dataSource.VisualDesignerData.Top = Math.round(offset.top);
                        dataSource.VisualDesignerData.Left = Math.round(offset.left);
                    });
                };

                // Configure a DataSource
                $scope.configureDataSource = function(dataSource) {
                    if (dataSource.ReadOnly) return;

                    // Ensure dataSource Entity is saved
                    if (!queryDef.dataSourceIsPersisted(dataSource)) 
                        $scope.savePipeline();
                    else
                        pipelineService.editDataSourcePart(dataSource, queryDef.data.InstalledDataSources);
                };


                // Show/Hide Endpoint Overlays
                $scope.showEndpointOverlays = true;
                $scope.toggleEndpointOverlays = function() {
                    $scope.showEndpointOverlays = !$scope.showEndpointOverlays;

                    var endpoints = plumbGui.instance.selectEndpoints();
                    if ($scope.showEndpointOverlays)
                        endpoints.showOverlays();
                    else
                        endpoints.hideOverlays();
                };

                // Edit Pipeline Entity
                $scope.editPipelineEntity = function() {
                    // save Pipeline, then open Edit Dialog
                    $scope.savePipeline().then(function() {
                        vm.saveShortcut.unbind();// disable ctrl+s
                        eavAdminDialogs.openEditItems([{ EntityId: queryDef.id }],
                            function (success) {
                                console.log("testing", success);
                                vm.reInitQuery()
                                    .then(resetPlumbAndWarnings) // reset jsplumb
                                    .then(vm.saveShortcut.rebind);// re-enable ctrl+s
                            });

                    });
                };

                // #region Save Pipeline
                // Handle Pipeline Saved, success contains the updated Pipeline Data
                function resetPlumbAndWarnings(promise) {
                    // Reset jsPlumb, re-Init Connections
                    plumbGui.instance.reset();
                    plumbGui.connectionsInitialized = false;
                    refreshWarnings(queryDef.data, vm);
                    return promise;
                }


                // Save Pipeline
                // returns a Promise about the saving state
                vm.savePipeline = $scope.savePipeline = function savePipeline() {
                    toastr.info("This shouldn't take long", "Saving...");
                    plumbGui.pushPlumbConfigToQueryDef(plumbGui.instance);
                    return queryDef.save()
                        .then(resetPlumbAndWarnings);
                };


                // #endregion

                // Repaint jsPlumb
                $scope.repaint = function() { plumbGui.instance.repaintEverything(); };

                // Show/Hide Debug info
                $scope.toogleDebug = function() { $scope.debug = !$scope.debug; };

                // check if there are special warnings the developer should know
                // typically when the test-module-id is different from the one we're currently
                // working on, or if no test-module-id is provided
                // note: this should actually be external code, and injected later on
                // reason is that it's actually testing for a 2sxc-variable mid
                function refreshWarnings(pipelineData, vm) {
                    var regex = /^\[module:moduleid\]=([0-9]*)$/gmi; // capture the mod-id
                    var testParams, testMid;
                    var warnings = vm.warnings = [];
                    try { // catch various not-initialized errors
                        testParams = pipelineData.Pipeline.TestParameters;
                        var matches = regex.exec(testParams);
                        if (!matches || matches.length === 0)
                            warnings.push(
                                "Your test values has no moduleid specified. You probably want to check your test-parameters.");
                        testMid = matches[1];
                        var urlMid = getUrlParamMustRefactor("mid");
                        if (testMid !== urlMid)
                            warnings.push("Your test moduleid (" +
                                testMid +
                                ") is different from the current moduleid (" +
                                urlMid +
                                "). You probably want to check your test-values.");
                    } catch (ex) { }
                }

                // Query the Pipeline
                $scope.queryPipeline = function(saveFirst) {
                    function runQuery() {
                        // Query pipelineService for the result...
                        toastr.info("Running Query ...");

                        pipelineService.queryPipeline(queryDef.id).then(function(success) {
                                // Show Result in a UI-Dialog
                                toastr.clear();

                                var resolve = eavAdminDialogs.CreateResolve({
                                    testParams: queryDef.data.Pipeline.TestParameters,
                                    result: success
                                });
                                eavAdminDialogs.OpenModal("pipelines/query-stats.html",
                                    "QueryStats as vm",
                                    "lg",
                                    resolve);

                                $timeout(function() {
                                    plumbGui.putEntityCountOnConnection(success);
                                });
                                $log.debug(success);
                            },
                            function(reason) {
                                toastr.error(reason, "Query failed");
                            });
                    }

                    // Ensure the Pipeline is saved
                    if (saveFirst)
                        $scope.savePipeline().then(runQuery);
                    else
                        runQuery();
                };

                vm.typeInfo = queryDef.dsTypeInfo;
            }]);
})();
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
// Config and Controller for the Pipeline Management UI
angular.module("PipelineManagement", [
    "EavServices",
    "EavConfiguration",
    "eavNgSvcs",
    "EavAdminUi"
]).
    controller("PipelineManagement", ["$uibModalInstance", "appId", "pipelineService", "debugState", "eavAdminDialogs", "eavConfig", function ($uibModalInstance, appId, pipelineService, debugState, eavAdminDialogs, eavConfig) {
        var vm = this;
        vm.debug = debugState;
        vm.appId = appId;

        pipelineService.setAppId(appId);

        // Refresh List of Pipelines
        vm.refresh = function () {
            vm.pipelines = pipelineService.getPipelines(appId);
        };
        vm.refresh();

        // Delete a Pipeline
        vm.delete = function (pipeline) {
            if (!confirm("Delete Pipeline \"" + pipeline.Name + "\" (" + pipeline.Id + ")?"))
                return;

            pipelineService.deletePipeline(pipeline.Id).then(function () {
                vm.refresh();
            }, function (reason) {
                alert(reason);
            });
        };

        // Clone a Pipeline
        vm.clone = function (pipeline) {
            pipelineService.clonePipeline(pipeline.Id).then(function () {
                vm.refresh();
            }, function (reason) {
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
        vm.close = function () { $uibModalInstance.dismiss("cancel"); };
    }]);

(function () {
    var jsPlumb;    // needed, as we'll fill it later with the window value

    var linePaintDefault = {
        lineWidth: 4,
        strokeStyle: "#61B7CF",
        joinstyle: "round",
        outlineColor: "white",
        outlineWidth: 2
    };
    var lineCount = 0,
        lineColors = [
            "#009688", "#00bcd4", "#3f51b5", "#9c27b0", "#e91e63",
            "#db4437", "#ff9800", "#60a917", "#60a917", "#008a00",
            "#00aba9", "#1ba1e2", "#0050ef", "#6a00ff", "#aa00ff",
            "#f472d0", "#d80073", "#a20025", "#e51400", "#fa6800",
            "#f0a30a", "#e3c800", "#825a2c", "#6d8764", "#647687",
            "#76608a", "#a0522d"
        ],
        maxCols = lineColors.length - 1;
    function resetLineCount() { lineCount = 0; }
    function nextLinePaintStyle() {
        return Object.assign({}, linePaintDefault, { strokeStyle: lineColors[lineCount++ % maxCols] });
    }

    console.log(nextLinePaintStyle());
    
    var instanceTemplate = {
        Connector: ["Bezier", { curviness: 70 }],
        HoverPaintStyle: {
            lineWidth: 4,
            strokeStyle: "#216477",
            outlineWidth: 2,
            outlineColor: "white"
        },
        PaintStyle: nextLinePaintStyle(),// linePaintDefault,
        Container: "pipelineContainer"
    };



    angular.module("PipelineDesigner").factory("plumbGui",
        ["queryDef", "$filter", "$log", "$timeout", function(queryDef, $filter, $log, $timeout) {

            var plumbGui = {
                dataSrcIdPrefix: "dataSource_",
                connectionsInitialized: false
            };


            // the definition of source endpoints (the small blue ones)
            plumbGui.buildSourceEndpoint = function() {
                return {
                    paintStyle: { fillStyle: "transparent", radius: 10, lineWidth: 0 },
                    cssClass: "sourceEndpoint",
                    maxConnections: -1,
                    isSource: true,
                    anchor: ["Continuous", { faces: ["top"] }],
                    overlays: getEndpointOverlays(true, queryDef.readOnly)
                };
            };

            // the definition of target endpoints (will appear when the user drags a connection) 
            plumbGui.buildTargetEndpoint = function() {
                return {
                    paintStyle: { fillStyle: "transparent", radius: 10, lineWidth: 0 },
                    cssClass: "targetEndpoint",
                    maxConnections: 1,
                    isTarget: true,
                    anchor: ["Continuous", { faces: ["bottom"] }],
                    overlays: getEndpointOverlays(false, queryDef.readOnly),
                    dropOptions: { hoverClass: "hover", activeClass: "active" }
                };
            };

            // this will retrieve the dataSource info-object for a DOM element
            plumbGui.findDataSourceOfElement = function fdsog(element) {
                var guid = element.attributes.guid.value;
                var list = queryDef.data.DataSources;
                var found = $filter("filter")(list, { EntityGuid: guid })[0];
                return found;
            };

            // Sync jsPlumb Connections and StreamsOut to the pipelineData-Object
            plumbGui.pushPlumbConfigToQueryDef = function() {
                var connectionInfos = [];
                angular.forEach(plumbGui.instance.getAllConnections(),
                    function(connection) {
                        connectionInfos.push({
                            From: connection.sourceId.substr(plumbGui.dataSrcIdPrefix.length),
                            Out: connection.endpoints[0].getOverlay("endpointLabel").label,
                            To: connection.targetId.substr(plumbGui.dataSrcIdPrefix.length),
                            In: connection.endpoints[1].getOverlay("endpointLabel").label
                        });
                    });
                queryDef.data.Pipeline.StreamWiring = connectionInfos;

                var streamsOut = [];
                plumbGui.instance.selectEndpoints({ target: plumbGui.dataSrcIdPrefix + "Out" }).each(
                    function(endpoint) {
                        streamsOut.push(endpoint.getOverlay("endpointLabel").label);
                    });
                queryDef.data.Pipeline.StreamsOut = streamsOut.join(",");
            };


            // Add a jsPlumb Endpoint to an Element
            plumbGui.addEndpoint = function(element, name, isIn) {
                if (!element.length) {
                    $log.error({ message: "Element not found", selector: element.selector });
                    return;
                }
                //console.log(element);

                var dataSource = plumbGui.findDataSourceOfElement(element[0]);

                var uuid = element[0].id + (isIn ? "_in_" : "_out_") + name;
                var params = {
                    uuid: uuid,
                    enabled:
                        !dataSource.ReadOnly ||
                            dataSource.EntityGuid === "Out" // Endpoints on Out-DataSource must be always enabled
                };
                var endPoint = plumbGui.instance.addEndpoint(element,
                    (isIn ? plumbGui.buildTargetEndpoint() : plumbGui.buildSourceEndpoint()),
                    params);
                endPoint.getOverlay("endpointLabel").setLabel(name);
            };

            plumbGui.initWirings = function () {
                angular.forEach(queryDef.data.Pipeline.StreamWiring,
                    function(wire) {
                        // read connections from Pipeline
                        var sourceElementId = plumbGui.dataSrcIdPrefix + wire.From;
                        var fromUuid = sourceElementId + "_out_" + wire.Out;
                        var targetElementId = plumbGui.dataSrcIdPrefix + wire.To;
                        var toUuid = targetElementId + "_in_" + wire.In;

                        // Ensure In- and Out-Endpoint exist
                        if (!plumbGui.instance.getEndpoint(fromUuid))
                            plumbGui.addEndpoint(jsPlumb.getSelector("#" + sourceElementId), wire.Out, false);
                        if (!plumbGui.instance.getEndpoint(toUuid))
                            plumbGui.addEndpoint(jsPlumb.getSelector("#" + targetElementId), wire.In, true);

                        try {
                            plumbGui.instance.connect({
                                uuids: [fromUuid, toUuid],
                                //PaintStyle: nextLinePaintStyle(),// { strokeWidth: 15, stroke: 'rgba(0, 243,230,18)' }
                                paintStyle: nextLinePaintStyle()
                            });
                        } catch (e) {
                            $log.error({ message: "Connection failed", from: fromUuid, to: toUuid });
                        }
                    });
            };

            plumbGui.putEntityCountOnConnection = function (result) {
                angular.forEach(result.Streams, function (stream) {
                    // Find jsPlumb Connection for the current Stream
                    var sourceElementId = plumbGui.dataSrcIdPrefix + stream.Source;
                    var targetElementId = plumbGui.dataSrcIdPrefix + stream.Target;
                    if (stream.Target === "00000000-0000-0000-0000-000000000000"
                        || stream.Target === queryDef.data.Pipeline.EntityGuid)
                        targetElementId = plumbGui.dataSrcIdPrefix + "Out";

                    var fromUuid = sourceElementId + "_out_" + stream.SourceOut;
                    var toUuid = targetElementId + "_in_" + stream.TargetIn;

                    var sEndp = plumbGui.instance.getEndpoint(fromUuid);
                    var streamFound = false;
                    if (sEndp) {
                        angular.forEach(sEndp.connections, function (connection) {
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
                        $log.error("Stream not found", stream, sEndp);
                });
            };


            plumbGui.makeSource = function(dataSource, element, dragHandler) {
                // suspend drawing and initialise
                plumbGui.instance.batch(function() {

                    // make DataSources draggable. Must happen before makeSource()!
                    if (!queryDef.readOnly)
                        plumbGui.instance.draggable(element,
                            {
                                grid: [20, 20],
                                drag: dragHandler
                            });

                    // Add Out- and In-Endpoints from Definition
                    var dataSourceDefinition = dataSource.Definition();
                    if (dataSourceDefinition) {
                        // Add Out-Endpoints
                        angular.forEach(dataSourceDefinition.Out,
                            function(name) {
                                plumbGui.addEndpoint(element, name, false);
                            });
                        // Add In-Endpoints
                        angular.forEach(dataSourceDefinition.In,
                            function(name) {
                                plumbGui.addEndpoint(element, name, true);
                            });
                        // make the DataSource a Target for new Endpoints (if .In is an Array)
                        if (dataSourceDefinition.In) {
                            var targetEndpointUnlimited = plumbGui.buildTargetEndpoint();
                            targetEndpointUnlimited.maxConnections = -1;
                            plumbGui.instance.makeTarget(element, targetEndpointUnlimited);
                        }

                        if (dataSourceDefinition.DynamicOut)
                            plumbGui.instance.makeSource(element,
                                plumbGui.buildSourceEndpoint(),
                                { filter: ".add-endpoint .new-connection" });
                    }
                });
            };



            plumbGui.buildInstance = function() {
                plumbGui.instance = jsPlumb.getInstance(instanceTemplate);

                // If connection on Out-DataSource was removed, remove custom Endpoint
                plumbGui.instance.bind("connectionDetached",
                    function(info) {
                        if (info.targetId === plumbGui.dataSrcIdPrefix + "Out") {
                            var element = angular.element(info.target);
                            var fixedEndpoints = plumbGui.findDataSourceOfElement(element).dataSource.Definition().In;
                            var label = info.targetEndpoint.getOverlay("endpointLabel").label;
                            if (fixedEndpoints.indexOf(label) === -1) {
                                $timeout(function() {
                                    plumbGui.instance.deleteEndpoint(info.targetEndpoint);
                                });
                            }
                        }
                    });


                // If a new connection is created, ask for a name of the In-Stream
                plumbGui.instance.bind("connection", function (info) {
                    if (!plumbGui.connectionsInitialized) return;

                    // Repeat until a valid Stream-Name is provided by the user
                    var repeatCount = 0;
                    var labelPrompt,
                        targetEndpointHavingSameLabel;

                    var endpointHandling = function (endpoint) {
                        var label = endpoint.getOverlay("endpointLabel").getLabel();
                        if (label === labelPrompt &&
                            info.targetEndpoint.id !== endpoint.id &&
                            angular.element(endpoint.canvas).hasClass("targetEndpoint"))
                            targetEndpointHavingSameLabel = endpoint;
                    };

                    while (true) {
                        repeatCount++;

                        var promptMessage = "Please name the Stream";
                        if (repeatCount > 1)
                            promptMessage += ". Ensure the name is not used by any other Stream on this DataSource.";

                        var endpointLabel = info.targetEndpoint.getOverlay("endpointLabel");
                        labelPrompt = prompt(promptMessage, endpointLabel.getLabel());
                        if (labelPrompt)
                            endpointLabel.setLabel(labelPrompt);
                        else
                            continue;

                        // Check if any other Target-Endpoint has the same Stream-Name (Label)
                        var endpoints = plumbGui.instance.getEndpoints(info.target.id);
                        targetEndpointHavingSameLabel = null; // reset...

                        angular.forEach(endpoints, endpointHandling);
                        if (targetEndpointHavingSameLabel)
                            continue;

                        break;
                    }
                });
            };

            // init new jsPlumb Instance
            window.jsPlumb.ready(function () {
                jsPlumb = window.jsPlumb; // re-set local short-name, as now it's initialized & ready
                plumbGui.buildInstance();// can't do this before jsplumb is ready...
            });

            return plumbGui;

        }]);


    // #region jsPlumb Endpoint Definitions
    function getEndpointOverlays(isSource, readOnlyMode) {
        return [
            [
                "Label", {
                    id: "endpointLabel",
                    //location: [0.5, isSource ? -0.5 : 1.5],
                    location: [0.5, isSource ? 0 : 1],
                    label: "Default",
                    cssClass: "noselect " + (isSource ? "endpointSourceLabel" : "endpointTargetLabel"),
                    events: {
                        dblclick: function (labelOverlay) {
                            if (readOnlyMode) return;

                            var newLabel = prompt("Rename Stream", labelOverlay.label);
                            if (newLabel)
                                labelOverlay.setLabel(newLabel);
                        }
                    }
                }
            ]
        ];
    }


})();

(function() {

    var guiTypes = buildGuiTypes();

    /*
        shared data state across various components
    */
    angular.module("PipelineDesigner").factory("queryDef",
        ["pipelineId", "pipelineService", "$q", "$location", "toastr", "$filter", "eavConfig", function (pipelineId, pipelineService, $q, $location, toastr, $filter, eavConfig) {

            var queryDef = {
                id: pipelineId, // injected from URL
                dsCount: 0,
                readOnly: true,
                data: null,


                // Test wether a DataSource is persisted on the Server
                dataSourceIsPersisted: function(dataSource) {
                    return dataSource.EntityGuid.indexOf("unsaved") === -1;
                },

                addDataSource: function(partAssemblyAndType, visualDesignerData, entityGuid, name) {
                    if (!visualDesignerData)
                        visualDesignerData = { Top: 100, Left: 100 };

                    var newDataSource = {
                        VisualDesignerData: visualDesignerData,
                        Name: name || $filter("typename")(partAssemblyAndType, "className"),
                        Description: "",
                        PartAssemblyAndType: partAssemblyAndType,
                        EntityGuid: entityGuid || "unsaved" + (queryDef.dsCount + 1)
                    };
                    // Extend it with a Property to it's Definition
                    newDataSource = angular.extend(newDataSource,
                        pipelineService.getNewDataSource(queryDef.data, newDataSource));

                    queryDef.data.DataSources.push(newDataSource);
                },

                loadQuery: function() {
                    return pipelineService.getPipeline(queryDef.id)
                        .then(function (success) {
                            queryDef.data = success;

                            // If a new (empty) Pipeline is made, init new Pipeline
                            if (!queryDef.id || queryDef.data.DataSources.length === 1) {
                                queryDef.readOnly = false;
                                queryDef.loadQueryFromDefaultTemplate();
                            } else {
                                // if read only, show message
                                queryDef.readOnly = !success.Pipeline.AllowEdit;
                                toastr.info(queryDef.readOnly
                                    ? "This pipeline is read only"
                                    : "You can now design the Pipeline. \nVisit 2sxc.org/help for more.",
                                    "Ready",
                                    { autoDismiss: true });
                            }
                        });
                },


                // Init a new Pipeline with DataSources and Wirings from Configuration
                loadQueryFromDefaultTemplate: function() {
                    var templateForNew = eavConfig.pipelineDesigner.defaultPipeline.dataSources;
                    angular.forEach(templateForNew, function (dataSource) {
                        queryDef.addDataSource(dataSource.partAssemblyAndType, dataSource.visualDesignerData, dataSource.entityGuid);
                    });

                    // attach template wiring
                    queryDef.data.Pipeline.StreamWiring = eavConfig.pipelineDesigner.defaultPipeline.streamWiring;
                },

                // save the current query and reload entire definition as returned from server
                save: function() {
                    queryDef.readOnly = true;

                    return pipelineService.savePipeline(queryDef.data.Pipeline, queryDef.data.DataSources)
                        .then(function(success) {
                                // Update PipelineData with data retrieved from the Server
                                queryDef.data.Pipeline = success.Pipeline;
                                queryDef.data.TestParameters = success.TestParameters;
                                queryDef.id = success.Pipeline.EntityId;
                                $location.search("PipelineId", success.Pipeline.EntityId);
                                queryDef.readOnly = !success.Pipeline.AllowEdit;
                                queryDef.data.DataSources = success.DataSources;
                                pipelineService.postProcessDataSources(queryDef.data);

                                // communicate to the user...
                                toastr.clear();
                                toastr.success("Pipeline " + success.Pipeline.EntityId + " saved and loaded",
                                    "Saved", { autoDismiss: true });

                            },
                            function(reason) {
                                toastr.error(reason, "Save Pipeline failed");
                                queryDef.readOnly = false;
                            });
                },

                _typeInfos: {},
                dsTypeInfo: function (dataSource) {
                    // maybe we already retrieved it before...
                    var cacheKey = dataSource.EntityGuid;
                    if (queryDef._typeInfos[cacheKey]) return queryDef._typeInfos[cacheKey];

                    var typeInfo = null;
                    // try to find the type on the source
                    var found = $filter("filter")(queryDef.data.InstalledDataSources,
                        { PartAssemblyAndType: dataSource.PartAssemblyAndType });
                    if (found && found.length) {
                        var def = found[0], primType = def.PrimaryType;
                        typeInfo = Object.assign({}, primType ? guiTypes[primType] : guiTypes.Unknown);
                        if (def.Icon) typeInfo.icon = guiTypes.iconPrefix + def.Icon;
                        if (def.DynamicOut) typeInfo.dynamicOut = true;
                        if (def.HelpLink) typeInfo.helpLink = def.HelpLink;
                        if (def.EnableConfig) typeInfo.config = def.EnableConfig;
                    }
                    if (!typeInfo) typeInfo = guiTypes.Unknown;

                    queryDef._typeInfos[cacheKey] = typeInfo;
                    return typeInfo;
                }


            };


            return queryDef;
        }]);

function buildGuiTypes() {
    var guiTypes = {
        iconPref: "eav-icon-"
    };

    function addGuiType(name, icon, notes) { guiTypes[name] = { name: name, icon: guiTypes.iconPref + icon, notes: notes }; }

    addGuiType("Unknown", "circle", "unknown type");
    addGuiType("Cache", "history", "caching of data");
    addGuiType("Filter", "filter", "filter data - usually returning less items than came in");
    addGuiType("Logic", "fork", "logic operations - usually choosing between different streams");
    addGuiType("Lookup", "search", "lookup operation - usually looking for other data based on a criteria");
    addGuiType("Modify", "star-half-alt", "modify data - usually changing, adding or removing values"); // tod  o
    addGuiType("Security", "user", "security - usually limit what the user sees based on his identity");
    addGuiType("Sort", "sort-alt-up", "sort the items");
    addGuiType("Source", "export", "source of new data - usually SQL, CSV or similar");
    addGuiType("Target", "target", "target - usually just a destination of data");

    return guiTypes;
}


})();
/*jshint laxbreak:true */
(function() {

    angular.module("PipelineDesigner")
        .controller("QueryStats", ["testParams", "result", "$uibModalInstance", function (testParams, result, $uibModalInstance) {
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
                    $uibModalInstance.dismiss("cancel");
                };

            }]
        );
})();
if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.lastIndexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}
// Init the main eav services module
angular.module("EavServices", [
    "ng",                   // Angular for $http etc.
    "EavConfiguration",     // global configuration
    "pascalprecht.translate",
    "ngResource",           // only needed for the pipeline-service, maybe not necessary any more?
    "ngAnimate",
    "toastr"
]);

angular.module('EavServices')
    .factory('contentItemsSvc', ["$http", "entitiesSvc", "metadataSvc", "svcCreator", function ($http, entitiesSvc, metadataSvc, svcCreator) {
        return function (appId, contentType) {
            var svc = {};
            svc.contentType = contentType;
            svc.appId = appId;

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get('eav/entities/GetAllOfTypeForAdmin', { params: { appId: svc.appId, contentType: svc.contentType } });
            }));
            
            // delete, then reload
            svc.delete = function (id, tryForce) {
                return entitiesSvc.delete(svc.contentType, id, tryForce);
                //.then(svc.liveListReload, null);
                //});
            };
            
            // todo: should use the ContentTypeService instead
            svc.getColumns = function () {
                return $http.get("eav/contenttype/getfields/", { params: { appid: svc.appId, staticName: svc.contentType } });
            };

            return svc;
        };
    }]);

angular.module("EavServices")
    .factory("contentTypeSvc", ["$http", "eavConfig", "svcCreator", "$translatePartialLoader", "$translate", function ($http, eavConfig, svcCreator, $translatePartialLoader, $translate) {
        return function appSpecificContentTypeSvc(appId, scope) {
            var svc = {};
            svc.scope = scope || eavConfig.contentType.defaultScope;
            svc.appId = appId;

            svc.retrieveContentTypes = function typeListRetrieve() {
                return $http.get("eav/contenttype/get/", { params: { "appid": svc.appId, "scope": svc.scope } });
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(svc.retrieveContentTypes));

            svc.getDetails = function getDetails(contentTypeName, config) {
                return $http.get("eav/contenttype/GetSingle", angular.extend({}, config, {
                    params: { "appid": svc.appId, "contentTypeStaticName": contentTypeName }
                }))
                    .then(function (promise) {
                        // check if definition asks for external i18n, then add to loader
                        if (promise && promise.data && promise.data.I18nKey) {
                            $translatePartialLoader.addPart("content-types/" + promise.data.I18nKey);
                        }
                        return promise;
                    });
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
                return $http.get("eav/contenttype/delete", { params: { appid: svc.appId, staticName: item.StaticName } })
                    .then(svc.liveListReload);
            };

            svc.setScope = function setScope(newScope) {
                svc.scope = newScope;
                svc.liveListReload();
            };

            svc.createGhost = function createGhost(sourceStaticName) {
                return $http.get("eav/contenttype/createghost", { params: { appid: svc.appId, sourceStaticName: sourceStaticName } })
                    .then(svc.liveListReload);
            };
            return svc;
        };

    }]);

angular.module("EavServices")
    .factory("contentTypeFieldSvc", ["$http", "eavConfig", "svcCreator", "$filter", function($http, eavConfig, svcCreator, $filter) {
        return function createFieldsSvc(appId, contentType) {
            // start with a basic service which implement the live-list functionality
            var svc = {};
            svc.appId = appId;
            svc.contentType = contentType;

            svc.typeListRetrieve = function typeListRetrieve() {
                return $http.get("eav/contenttype/datatypes/", { params: { "appid": svc.appId } });
            };

            svc._inputTypesList = [];
            svc.getInputTypesList = function getInputTpes() {
                if (svc._inputTypesList.length > 0)
                    return svc._inputTypesList;
                $http.get("eav/contenttype/inputtypes/", { params: { "appid": svc.appId } })
                    .then(function(result) {
                        function addToList(value, key) {
                            var item = {
                                dataType: value.Type.substring(0, value.Type.indexOf("-")),
                                inputType: value.Type,
                                label: value.Label,
                                description: value.Description
                            };
                            svc._inputTypesList.push(item);
                        }

                        angular.forEach(result.data, addToList);

                        svc._inputTypesList = $filter("orderBy")(svc._inputTypesList, ["dataType", "inputType"]);
                    });
                return svc._inputTypesList;
            };

	        svc.getFields = function getFields() {
	            return $http.get("eav/contenttype/getfields", { params: { "appid": svc.appId, "staticName": svc.contentType.StaticName } })
	            .then(function(result) {
	                // merge the settings into one, with correct priority sequence
	                if (result.data ) {
	                    for (var i = 0; i < result.data.length; i++) {
	                        var fld = result.data[i];
	                        if(!fld.Metadata)
                                continue;
	                        var md = fld.Metadata;
	                        var allMd = md.All;
	                        var typeMd = md[fld.Type];
	                        var inputMd = md[fld.InputType];
	                        md.merged = angular.merge({}, allMd, typeMd, inputMd);
	                    }
	                }
	                    return result;
	                });
	        };

            svc = angular.extend(svc, svcCreator.implementLiveList(svc.getFields));

            svc.types = svcCreator.implementLiveList(svc.typeListRetrieve);


            svc.reOrder = function reOrder(idArray) {
                console.log(idArray);
                return $http.get("eav/contenttype/reorder", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, newSortOrder: JSON.stringify(idArray) } })
                    .then(svc.liveListReload);
            };

            svc.delete = function del(item) {
                return $http.get("eav/contenttype/delete", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id } })
                    .then(svc.liveListReload);
            };

            svc.addMany = function add(items, count) {
                return $http.get("eav/contenttype/addfield/", { params: items[count] })
                    .then(function() {
                        if (items.length === ++count)
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
                    InputType: "string-default",
                    StaticName: "",
                    IsTitle: svc.liveList().length === 0,
                    SortOrder: svc.liveList().length + svc.newItemCount++
                };
            };


            svc.delete = function del(item) {
                if (item.IsTitle)
                    throw "Can't delete Title";
                return $http.get("eav/contenttype/deletefield", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id } })
                    .then(svc.liveListReload);
            };

            svc.updateInputType = function updateInputType(item) {
                return $http.get("eav/contenttype/updateinputtype", { params: { appid: svc.appId, attributeId: item.Id, field: item.StaticName, inputType: item.InputType } })
                    .then(svc.liveListReload);
            };


            svc.setTitle = function setTitle(item) {
                return $http.get("eav/contenttype/setTitle", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id } })
                    .then(svc.liveListReload);
            };

            svc.rename = function rename(item, newName) {
                return $http.get("eav/contenttype/rename", { params: { appid: svc.appId, contentTypeId: svc.contentType.Id, attributeId: item.Id, newName: newName } })
                    .then(svc.liveListReload);
            };


            return svc;
        };
    }]);
/* 
 * 
 * Simple service which takes care of ctrl+S keyboard shortcuts. 
 * use it as a service for your controller, then add a line like 
         function activate() {
            // add ctrl+s to save
            ctrlS.bind(function() { vm.save(false); });
        }

 */

angular.module("EavServices")
    .factory("ctrlS", ["$window", function ($window) {

        // Create a capture Ctrl+S and execute action-object
        function createSave(action) {
            var save = {
                _event: null,
                _action: null,
                _isbound: false,

                // this will be called on each keydown, will check if it was a ctrl+S
                detectCtrlSAndExcecute: function (e) {
                    if (!save._isbound) return null; // special, in case unbinding didn't work 100% (can happen)
                    if (e.keyCode === 83 && (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey)) {
                        if (save._action === null)
                            return console.log("can't do anything on ctrl+S, no action registered");
                        e.preventDefault();
                        save._action();
                    }
                    return null;
                },

                bind: function(eventAction) {
                    save._action = eventAction;
                    save._isbound = true;
                    save._event = $window.addEventListener("keydown", save.detectCtrlSAndExcecute, false);
                },

                unbind: function() {
                    $window.removeEventListener("keydown", save.detectCtrlSAndExcecute);
                    save._isbound = false;
                },

                // re-attach Ctrl+S if it had already been attached previously
                rebind: function() {
                    if (save._action === null)
                        throw "can't rebind, as it was never initially bound";
                    if (save._isbound)
                        throw "can't rebind, as it's still bound";
                    save.bind(save._action);
                }
            };

            save.bind(action);

            return save;
        }

        return createSave;
    }]);
/* shared debugState = advancedMode
 * 
 * vm.debug -> shows if in debug mode - bind ng-ifs to this
 * vm.maybeEnableDebug - a method which checks for ctrl+shift-click and if yes, changes debug state
 *
 * How to use
 * 1. add uiDebug to your controller dependencies like:    contentImportController(appId, ..., debugState, $uibModalInstance, $filter)
 * 2. add a line after creating your vm-object like:       vm.debug = debugState;
 * 3. add a click event as far out as possible on html:    <div ng-click="vm.debug.autoEnableAsNeeded($event)">
 * 4. wrap your hidden stuff in an ng-if:                  <div ng-if="vm.debug.on">
 *
 * Note that if you're using it in a directive you'll use $scope instead of vm, so the binding is different.
 * For example, instead of <div ng-if="vm.debug.on"> you would write <div ng-if="debug.on">
 */

angular.module("EavServices")
    .factory("debugState", ["$translate", "toastr", "$http", function ($translate, toastr, $http) {
        var svc = {
            on: false
        };

        svc.toggle = function toggle() {
            svc.on = !svc.on;
            toastr.clear(svc.toast);
            svc.toast = toastr.info($translate.instant("AdvancedMode.Info.Turn" + (svc.on ? "On" : "Off")), { timeOut: 3000 });
        };

        svc.autoEnableAsNeeded = function (e) {
            e = window.event || e;
            var ctrlPressed = (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey);//evt.ctrlKey;
            if (ctrlPressed && !e.alreadySwitchedDebugState) {
                svc.toggle();
                e.alreadySwitchedDebugState = true;
            }
        };

        svc.enableExtendedLogging = function(duration) {
            return $http.get("app-sys/system/extendedlogging", { params: { "duration": duration } });
        };

        return svc;
    }]);
// This service adds CSS classes to body when something is dragged onto the page
angular.module("EavServices")
    .factory("dragClass", function () {

        document.addEventListener("dragover", function() {
            if(this === document)
                document.body.classList.add("eav-dragging");
        });
        document.addEventListener("dragleave", function() {
            if(this === document)
                document.body.classList.remove("eav-dragging");
        });

        return {};

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
 * 2. Your main app must also declare the other apps as dependencies, so angular.module('yourname', ['dialog 1', 'diolag 2'])
 * 3. your main app must also need this ['EavAdminUI']
 * 4. your controller must require eavAdminDialogs
 * 5. Then you can call such a dialog
 */

// Todo
// 1. Import / Export
// 2. Pipeline Designer

angular.module("EavAdminUi", ["ng",
    "ui.bootstrap",         // for the $uibModal etc.
    "EavServices",
    "eavTemplates",         // Provides all cached templates
    "PermissionsApp",       // Permissions dialogs to manage permissions
    "ContentItemsAppAgnostic", 
    "PipelineManagement",   // Manage pipelines
    "ContentImportApp",
    "ContentExportApp",
    "HistoryApp",            // the item-history app

    // big todo: currently removed dependency to eavEditentity (much faster) but it actually does...
    // ...need it to initialize this class, so ATM this only works in a system where the other dependency
    // is defined. very not clean :( 
    // but much faster for now
    // the correct clean up would be to create an edit-dialogs class or something (todo)
    // "eavEditEntity"			// the edit-app
])
    .factory("eavAdminDialogs", ["$uibModal", "eavConfig", "$window", "entitiesSvc", "contentTypeSvc", "appId", function ($uibModal, eavConfig, $window,
        // these are needed just for simple access to some dialogs
        entitiesSvc,    // warning: this only works ATM when called in 2sxc, because it needs the eavEditEntity dependency
        contentTypeSvc,
        appId) {
        /*jshint laxbreak:true */

        var svc = {};

        //#region List of Content Items dialogs
        svc.openContentItems = function oci(appId, staticName, itemId, closeCallback) {
            var resolve = svc.CreateResolve({ appId: appId, contentType: staticName, contentTypeId: itemId });
            return svc.OpenModal("content-items/content-items-agnostic.html", "ContentItemsList as vm", "fullscreen", resolve, closeCallback);
        };
        //#endregion

        //#region content import export
        svc.openContentImport = function ocimp(appId, staticName, closeCallback) {
            var resolve = svc.CreateResolve({ appId: appId, contentType: staticName });
            return svc.OpenModal("content-import-export/content-import.html", "ContentImport as vm", "lg", resolve, closeCallback);
        };

        svc.openContentExport = function ocexp(appId, staticName, closeCallback, optionalIds) {
            var resolve = svc.CreateResolve({ appId: appId, contentType: staticName, itemIds: optionalIds });
            return svc.OpenModal("content-import-export/content-export.html", "ContentExport as vm", "lg", resolve, closeCallback);
        };

        //#endregion

        //#region ContentType dialogs
        svc.openContentTypeEdit = function octe(item, closeCallback) {
            var resolve = svc.CreateResolve({ item: item });
            return svc.OpenModal("content-types/content-types-edit.html", "Edit as vm", "", resolve, closeCallback);
        };

        svc.openContentTypeFields = function octf(item, closeCallback) {
            var resolve = svc.CreateResolve({ contentType: item });
            return svc.OpenModal("content-types/content-types-fields.html", "FieldList as vm", "xlg", resolve, closeCallback);
        };

        // this one assumes we have a content-item, but must first retrieve content-type-infos
        svc.openContentTypeFieldsOfItems = function octf(item, closeCallback) {
            return entitiesSvc.getManyForEditing(appId, item)
                .then(function (result) {
                    var ctName = result.data[0].Header.ContentTypeName;
                    var svcForThis = contentTypeSvc(appId); // note: won't specify scope to fallback
                    return svcForThis.getDetails(ctName)
                        .then(function (result2) {
                            return svc.openContentTypeFields(result2.data, closeCallback);
                        });
                });
        };

        //#endregion
        //#region Item - new, edit
        svc.openItemNew = function oin(contentTypeName, closeCallback) {
            return svc.openEditItems([{ ContentTypeName: contentTypeName }], closeCallback, { partOfPage: false });
        };

        svc.openItemEditWithEntityId = function oie(entityId, closeCallback) {
            return svc.openEditItems([{ EntityId: entityId }], closeCallback, { partOfPage: false });
        };

        svc.openEditItems = function oel(items, closeCallback, moreResolves) {
            var merged = angular.extend({ items: items }, moreResolves || {});
            merged.partOfPage = Boolean(merged.partOfPage);
            merged.publishing = merged.publishing || null;
            console.log("openEditItems: partOfPage: " + merged.partOfPage, " publishing: " + merged.publishing);
            var resolve = svc.CreateResolve(merged);
            return svc.OpenModal("form/main-form.html", "EditEntityWrapperCtrl as vm", "ent-edit", resolve, closeCallback);
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

            svc.openEditItems(items, closeCallback, { partOfPage: false });
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
            var url = svc.derivedUrl({
                dialog: "pipeline-designer",
                pipelineId: pipelineId
            });
            $window.open(url);
            return;
        };
        //#endregion

        //#region GenerateUrlBasedOnCurrent
        svc.derivedUrl = function derivedUrl(varsToReplace) {
            var url = window.location.href;
            for (var prop in varsToReplace)
                if (varsToReplace.hasOwnProperty(prop))
                    url = svc.replaceOrAddOneParam(url, prop, varsToReplace[prop]);

            return url;
            //url = url
            //    .replace(new RegExp("appid=[0-9]*", "i"), "appid=" + item.Id) // note: sometimes it doesn't have an appid, so it's [0-9]* instead of [0-9]+
            //    .replace(/approot=[^&]*/, "approot=" + item.AppRoot + "/")
            //    .replace("dialog=zone", "dialog=app");
        };

        svc.replaceOrAddOneParam = function replaceOneParam(original, param, value) {
            var rule = new RegExp("(" + param + "=).*?(&)", "i");
            var newText = rule.test(original)
                ? original.replace(rule, "$1" + value + "$2")
                : original + "&" + param + "=" + value;
            return newText;
        };
        //#endregion

        //#region Internal helpers
        svc._attachCallbacks = function attachCallbacks(promise, callbacks) {
            if (typeof (callbacks) === "undefined")
                return null;
            if (typeof (callbacks) === "function") // if it's only one callback, use it for all close-cases
                callbacks = { close: callbacks };
            return promise.result.then(callbacks.success || callbacks.close, callbacks.error || callbacks.close, callbacks.notify || callbacks.close);
        };

        // Will open a modal window. Has various specials, like
        // 1. If the templateUrl begins with "~/" - this will be re-mapped to the ng-app root. Only use this for not-inline stuff
        // 2. The controller can be written as "something as vm" and this will be split and configured corectly
        svc.openModalComponent = function (componentName, size, values, callbacks) {
            var modalInstance = $uibModal.open({
                component: componentName,
                resolve: svc.CreateResolve(values),
                size: size,
            });
            return svc._attachCallbacks(modalInstance, callbacks);
        };

        svc.OpenModal = function openModal(templateUrl, controller, size, resolveValues, callbacks) {
            var foundAs = controller.indexOf(" as ");
            var contAs = foundAs > 0 ?
                controller.substring(foundAs + 4)
                : null;

            if (foundAs > 0) controller = controller.substring(0, foundAs);
            var modalInstance = $uibModal.open({
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


/* File Type Services
 * Helps check if something is an image (then the UI usually wants a thumbnail)
 * ...or if it has an icon in the font-library - then it can provide the class name for the icon
 */
angular.module("EavServices").service("fileType", function () {
    var svc = {};
    svc.iconPrefix = "eav-icon-";
    svc.defaultIcon = "file";
    svc.checkImgRegEx = /(?:([^:\/?#]+):)?(?:\/\/([^\/?#]*))?([^?#]*\.(?:jpg|jpeg|gif|png))(?:\?([^#]*))?(?:#(.*))?/i;
    svc.extensions = {
        doc: "file-word",
        docx: "file-word",
        xls: "file-excel",
        xlsx: "file-excel",
        ppt: "file-powerpoint",
        pptx: "file-powerpoint",
        pdf: "file-pdf",
        mp3: "file-audio",
        avi: "file-video",
        mpg: "file-video",
        mpeg: "file-video",
        mov: "file-video",
        mp4: "file-video",
        zip: "file-archive",
        rar: "file-archive",
        txt: "file-text",
        html: "file-code",
        css: "file-code",
        xml: "file-code",
        xsl: "file-code",
        vcf: "user"
    };

    svc.getExtension = function(filename) {
        return filename.substr(filename.lastIndexOf(".") + 1).toLowerCase();
    };

    svc.getIconClass = function getClass(filename) {
        return svc.iconPrefix + (svc.extensions[svc.getExtension(filename)] || svc.defaultIcon);
    };
    
    svc.isKnownType = function(filename) {
        return svc.extensions.indexOf[svc.getExtension(filename)] !== -1;
    };

    svc.isImage = function(filename) {
        return svc.checkImgRegEx.test(filename);
    };

    // not used yet, so commented out
    //svc.type = function(url) {
    //    if (svc.isImage(url))
    //        return "image";
    //    return "file";
    //};

    return svc;
});

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
        .config(["$translateProvider", "languages", "$translatePartialLoaderProvider", function($translateProvider, languages, $translatePartialLoaderProvider) {
            $translateProvider
                .preferredLanguage(languages.currentLanguage.split("-")[0])
                .useSanitizeValueStrategy("escapeParameters") // this is very important to allow html in the JSON files
                .fallbackLanguage(languages.fallbackLanguage)
                .useLoader("$translatePartialLoader", {
                    urlTemplate: languages.i18nRoot + "{part}-{lang}.js"
                })
                .useLoaderCache(true); // should cache json
            $translatePartialLoaderProvider // these parts are always required
                .addPart("admin")
                .addPart("edit");
        }])

        // ensure that adding parts will load the missing files
        .run(["$rootScope", "$translate", function($rootScope, $translate) {
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
        .factory("$uibModalInstance", function () {
            return null;
        })

        // helper, currently only used by pipeline designer, to get url parameter
        // will provide a get-url-param command
        .factory("getUrlParamMustRefactor", function() {
                return getParameterByName;
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
/*
 * Cleans up all kinds of texts containing non-latin characters like umlauts or romanian characters etc.
 * 
 * based on http://stackoverflow.com/questions/286921/efficiently-replace-all-accented-characters-in-a-string
 * 
 */

angular.module("EavServices")
    .factory("latinizeText", function() {
        return function(input) {
            var latinMap = { "Á": "A", "Ă": "A", "Ắ": "A", "Ặ": "A", "Ằ": "A", "Ẳ": "A", "Ẵ": "A", "Ǎ": "A", "Â": "A", "Ấ": "A", "Ậ": "A", "Ầ": "A", "Ẩ": "A", "Ẫ": "A", "Ä": "Ae", "Ǟ": "A", "Ȧ": "A", "Ǡ": "A", "Ạ": "A", "Ȁ": "A", "À": "A", "Ả": "A", "Ȃ": "A", "Ā": "A", "Ą": "A", "Å": "A", "Ǻ": "A", "Ḁ": "A", "Ⱥ": "A", "Ã": "A", "Ꜳ": "AA", "Æ": "AE", "Ǽ": "AE", "Ǣ": "AE", "Ꜵ": "AO", "Ꜷ": "AU", "Ꜹ": "AV", "Ꜻ": "AV", "Ꜽ": "AY", "Ḃ": "B", "Ḅ": "B", "Ɓ": "B", "Ḇ": "B", "Ƀ": "B", "Ƃ": "B", "Ć": "C", "Č": "C", "Ç": "C", "Ḉ": "C", "Ĉ": "C", "Ċ": "C", "Ƈ": "C", "Ȼ": "C", "Ď": "D", "Ḑ": "D", "Ḓ": "D", "Ḋ": "D", "Ḍ": "D", "Ɗ": "D", "Ḏ": "D", "ǲ": "D", "ǅ": "D", "Đ": "D", "Ƌ": "D", "Ǳ": "DZ", "Ǆ": "DZ", "É": "E", "Ĕ": "E", "Ě": "E", "Ȩ": "E", "Ḝ": "E", "Ê": "E", "Ế": "E", "Ệ": "E", "Ề": "E", "Ể": "E", "Ễ": "E", "Ḙ": "E", "Ë": "E", "Ė": "E", "Ẹ": "E", "Ȅ": "E", "È": "E", "Ẻ": "E", "Ȇ": "E", "Ē": "E", "Ḗ": "E", "Ḕ": "E", "Ę": "E", "Ɇ": "E", "Ẽ": "E", "Ḛ": "E", "Ꝫ": "ET", "Ḟ": "F", "Ƒ": "F", "Ǵ": "G", "Ğ": "G", "Ǧ": "G", "Ģ": "G", "Ĝ": "G", "Ġ": "G", "Ɠ": "G", "Ḡ": "G", "Ǥ": "G", "Ḫ": "H", "Ȟ": "H", "Ḩ": "H", "Ĥ": "H", "Ⱨ": "H", "Ḧ": "H", "Ḣ": "H", "Ḥ": "H", "Ħ": "H", "Í": "I", "Ĭ": "I", "Ǐ": "I", "Î": "I", "Ï": "I", "Ḯ": "I", "İ": "I", "Ị": "I", "Ȉ": "I", "Ì": "I", "Ỉ": "I", "Ȋ": "I", "Ī": "I", "Į": "I", "Ɨ": "I", "Ĩ": "I", "Ḭ": "I", "Ꝺ": "D", "Ꝼ": "F", "Ᵹ": "G", "Ꞃ": "R", "Ꞅ": "S", "Ꞇ": "T", "Ꝭ": "IS", "Ĵ": "J", "Ɉ": "J", "Ḱ": "K", "Ǩ": "K", "Ķ": "K", "Ⱪ": "K", "Ꝃ": "K", "Ḳ": "K", "Ƙ": "K", "Ḵ": "K", "Ꝁ": "K", "Ꝅ": "K", "Ĺ": "L", "Ƚ": "L", "Ľ": "L", "Ļ": "L", "Ḽ": "L", "Ḷ": "L", "Ḹ": "L", "Ⱡ": "L", "Ꝉ": "L", "Ḻ": "L", "Ŀ": "L", "Ɫ": "L", "ǈ": "L", "Ł": "L", "Ǉ": "LJ", "Ḿ": "M", "Ṁ": "M", "Ṃ": "M", "Ɱ": "M", "Ń": "N", "Ň": "N", "Ņ": "N", "Ṋ": "N", "Ṅ": "N", "Ṇ": "N", "Ǹ": "N", "Ɲ": "N", "Ṉ": "N", "Ƞ": "N", "ǋ": "N", "Ñ": "N", "Ǌ": "NJ", "Ó": "O", "Ŏ": "O", "Ǒ": "O", "Ô": "O", "Ố": "O", "Ộ": "O", "Ồ": "O", "Ổ": "O", "Ỗ": "O", "Öe": "O", "Ȫ": "O", "Ȯ": "O", "Ȱ": "O", "Ọ": "O", "Ő": "O", "Ȍ": "O", "Ò": "O", "Ỏ": "O", "Ơ": "O", "Ớ": "O", "Ợ": "O", "Ờ": "O", "Ở": "O", "Ỡ": "O", "Ȏ": "O", "Ꝋ": "O", "Ꝍ": "O", "Ō": "O", "Ṓ": "O", "Ṑ": "O", "Ɵ": "O", "Ǫ": "O", "Ǭ": "O", "Ø": "O", "Ǿ": "O", "Õ": "O", "Ṍ": "O", "Ṏ": "O", "Ȭ": "O", "Ƣ": "OI", "Ꝏ": "OO", "Ɛ": "E", "Ɔ": "O", "Ȣ": "OU", "Ṕ": "P", "Ṗ": "P", "Ꝓ": "P", "Ƥ": "P", "Ꝕ": "P", "Ᵽ": "P", "Ꝑ": "P", "Ꝙ": "Q", "Ꝗ": "Q", "Ŕ": "R", "Ř": "R", "Ŗ": "R", "Ṙ": "R", "Ṛ": "R", "Ṝ": "R", "Ȑ": "R", "Ȓ": "R", "Ṟ": "R", "Ɍ": "R", "Ɽ": "R", "Ꜿ": "C", "Ǝ": "E", "Ś": "S", "Ṥ": "S", "Š": "S", "Ṧ": "S", "Ş": "S", "Ŝ": "S", "Ș": "S", "Ṡ": "S", "Ṣ": "S", "Ṩ": "S", "Ť": "T", "Ţ": "T", "Ṱ": "T", "Ț": "T", "Ⱦ": "T", "Ṫ": "T", "Ṭ": "T", "Ƭ": "T", "Ṯ": "T", "Ʈ": "T", "Ŧ": "T", "Ɐ": "A", "Ꞁ": "L", "Ɯ": "M", "Ʌ": "V", "Ꜩ": "TZ", "Ú": "U", "Ŭ": "U", "Ǔ": "U", "Û": "U", "Ṷ": "U", "Ü": "Ue", "Ǘ": "U", "Ǚ": "U", "Ǜ": "U", "Ǖ": "U", "Ṳ": "U", "Ụ": "U", "Ű": "U", "Ȕ": "U", "Ù": "U", "Ủ": "U", "Ư": "U", "Ứ": "U", "Ự": "U", "Ừ": "U", "Ử": "U", "Ữ": "U", "Ȗ": "U", "Ū": "U", "Ṻ": "U", "Ų": "U", "Ů": "U", "Ũ": "U", "Ṹ": "U", "Ṵ": "U", "Ꝟ": "V", "Ṿ": "V", "Ʋ": "V", "Ṽ": "V", "Ꝡ": "VY", "Ẃ": "W", "Ŵ": "W", "Ẅ": "W", "Ẇ": "W", "Ẉ": "W", "Ẁ": "W", "Ⱳ": "W", "Ẍ": "X", "Ẋ": "X", "Ý": "Y", "Ŷ": "Y", "Ÿ": "Y", "Ẏ": "Y", "Ỵ": "Y", "Ỳ": "Y", "Ƴ": "Y", "Ỷ": "Y", "Ỿ": "Y", "Ȳ": "Y", "Ɏ": "Y", "Ỹ": "Y", "Ź": "Z", "Ž": "Z", "Ẑ": "Z", "Ⱬ": "Z", "Ż": "Z", "Ẓ": "Z", "Ȥ": "Z", "Ẕ": "Z", "Ƶ": "Z", "Ĳ": "IJ", "Œ": "OE", "ᴀ": "A", "ᴁ": "AE", "ʙ": "B", "ᴃ": "B", "ᴄ": "C", "ᴅ": "D", "ᴇ": "E", "ꜰ": "F", "ɢ": "G", "ʛ": "G", "ʜ": "H", "ɪ": "I", "ʁ": "R", "ᴊ": "J", "ᴋ": "K", "ʟ": "L", "ᴌ": "L", "ᴍ": "M", "ɴ": "N", "ᴏ": "O", "ɶ": "OE", "ᴐ": "O", "ᴕ": "OU", "ᴘ": "P", "ʀ": "R", "ᴎ": "N", "ᴙ": "R", "ꜱ": "S", "ᴛ": "T", "ⱻ": "E", "ᴚ": "R", "ᴜ": "U", "ᴠ": "V", "ᴡ": "W", "ʏ": "Y", "ᴢ": "Z", "á": "a", "ă": "a", "ắ": "a", "ặ": "a", "ằ": "a", "ẳ": "a", "ẵ": "a", "ǎ": "a", "â": "a", "ấ": "a", "ậ": "a", "ầ": "a", "ẩ": "a", "ẫ": "a", "ä": "ae", "ǟ": "a", "ȧ": "a", "ǡ": "a", "ạ": "a", "ȁ": "a", "à": "a", "ả": "a", "ȃ": "a", "ā": "a", "ą": "a", "ᶏ": "a", "ẚ": "a", "å": "a", "ǻ": "a", "ḁ": "a", "ⱥ": "a", "ã": "a", "ꜳ": "aa", "æ": "ae", "ǽ": "ae", "ǣ": "ae", "ꜵ": "ao", "ꜷ": "au", "ꜹ": "av", "ꜻ": "av", "ꜽ": "ay", "ḃ": "b", "ḅ": "b", "ɓ": "b", "ḇ": "b", "ᵬ": "b", "ᶀ": "b", "ƀ": "b", "ƃ": "b", "ɵ": "o", "ć": "c", "č": "c", "ç": "c", "ḉ": "c", "ĉ": "c", "ɕ": "c", "ċ": "c", "ƈ": "c", "ȼ": "c", "ď": "d", "ḑ": "d", "ḓ": "d", "ȡ": "d", "ḋ": "d", "ḍ": "d", "ɗ": "d", "ᶑ": "d", "ḏ": "d", "ᵭ": "d", "ᶁ": "d", "đ": "d", "ɖ": "d", "ƌ": "d", "ı": "i", "ȷ": "j", "ɟ": "j", "ʄ": "j", "ǳ": "dz", "ǆ": "dz", "é": "e", "ĕ": "e", "ě": "e", "ȩ": "e", "ḝ": "e", "ê": "e", "ế": "e", "ệ": "e", "ề": "e", "ể": "e", "ễ": "e", "ḙ": "e", "ë": "e", "ė": "e", "ẹ": "e", "ȅ": "e", "è": "e", "ẻ": "e", "ȇ": "e", "ē": "e", "ḗ": "e", "ḕ": "e", "ⱸ": "e", "ę": "e", "ᶒ": "e", "ɇ": "e", "ẽ": "e", "ḛ": "e", "ꝫ": "et", "ḟ": "f", "ƒ": "f", "ᵮ": "f", "ᶂ": "f", "ǵ": "g", "ğ": "g", "ǧ": "g", "ģ": "g", "ĝ": "g", "ġ": "g", "ɠ": "g", "ḡ": "g", "ᶃ": "g", "ǥ": "g", "ḫ": "h", "ȟ": "h", "ḩ": "h", "ĥ": "h", "ⱨ": "h", "ḧ": "h", "ḣ": "h", "ḥ": "h", "ɦ": "h", "ẖ": "h", "ħ": "h", "ƕ": "hv", "í": "i", "ĭ": "i", "ǐ": "i", "î": "i", "ï": "i", "ḯ": "i", "ị": "i", "ȉ": "i", "ì": "i", "ỉ": "i", "ȋ": "i", "ī": "i", "į": "i", "ᶖ": "i", "ɨ": "i", "ĩ": "i", "ḭ": "i", "ꝺ": "d", "ꝼ": "f", "ᵹ": "g", "ꞃ": "r", "ꞅ": "s", "ꞇ": "t", "ꝭ": "is", "ǰ": "j", "ĵ": "j", "ʝ": "j", "ɉ": "j", "ḱ": "k", "ǩ": "k", "ķ": "k", "ⱪ": "k", "ꝃ": "k", "ḳ": "k", "ƙ": "k", "ḵ": "k", "ᶄ": "k", "ꝁ": "k", "ꝅ": "k", "ĺ": "l", "ƚ": "l", "ɬ": "l", "ľ": "l", "ļ": "l", "ḽ": "l", "ȴ": "l", "ḷ": "l", "ḹ": "l", "ⱡ": "l", "ꝉ": "l", "ḻ": "l", "ŀ": "l", "ɫ": "l", "ᶅ": "l", "ɭ": "l", "ł": "l", "ǉ": "lj", "ſ": "s", "ẜ": "s", "ẛ": "s", "ẝ": "s", "ḿ": "m", "ṁ": "m", "ṃ": "m", "ɱ": "m", "ᵯ": "m", "ᶆ": "m", "ń": "n", "ň": "n", "ņ": "n", "ṋ": "n", "ȵ": "n", "ṅ": "n", "ṇ": "n", "ǹ": "n", "ɲ": "n", "ṉ": "n", "ƞ": "n", "ᵰ": "n", "ᶇ": "n", "ɳ": "n", "ñ": "n", "ǌ": "nj", "ó": "o", "ŏ": "o", "ǒ": "o", "ô": "o", "ố": "o", "ộ": "o", "ồ": "o", "ổ": "o", "ỗ": "o", "ö": "oe", "ȫ": "o", "ȯ": "o", "ȱ": "o", "ọ": "o", "ő": "o", "ȍ": "o", "ò": "o", "ỏ": "o", "ơ": "o", "ớ": "o", "ợ": "o", "ờ": "o", "ở": "o", "ỡ": "o", "ȏ": "o", "ꝋ": "o", "ꝍ": "o", "ⱺ": "o", "ō": "o", "ṓ": "o", "ṑ": "o", "ǫ": "o", "ǭ": "o", "ø": "o", "ǿ": "o", "õ": "o", "ṍ": "o", "ṏ": "o", "ȭ": "o", "ƣ": "oi", "ꝏ": "oo", "ɛ": "e", "ᶓ": "e", "ɔ": "o", "ᶗ": "o", "ȣ": "ou", "ṕ": "p", "ṗ": "p", "ꝓ": "p", "ƥ": "p", "ᵱ": "p", "ᶈ": "p", "ꝕ": "p", "ᵽ": "p", "ꝑ": "p", "ꝙ": "q", "ʠ": "q", "ɋ": "q", "ꝗ": "q", "ŕ": "r", "ř": "r", "ŗ": "r", "ṙ": "r", "ṛ": "r", "ṝ": "r", "ȑ": "r", "ɾ": "r", "ᵳ": "r", "ȓ": "r", "ṟ": "r", "ɼ": "r", "ᵲ": "r", "ᶉ": "r", "ɍ": "r", "ɽ": "r", "ↄ": "c", "ꜿ": "c", "ɘ": "e", "ɿ": "r", "ß": "ss", "ś": "s", "ṥ": "s", "š": "s", "ṧ": "s", "ş": "s", "ŝ": "s", "ș": "s", "ṡ": "s", "ṣ": "s", "ṩ": "s", "ʂ": "s", "ᵴ": "s", "ᶊ": "s", "ȿ": "s", "ɡ": "g", "ᴑ": "o", "ᴓ": "o", "ᴝ": "u", "ť": "t", "ţ": "t", "ṱ": "t", "ț": "t", "ȶ": "t", "ẗ": "t", "ⱦ": "t", "ṫ": "t", "ṭ": "t", "ƭ": "t", "ṯ": "t", "ᵵ": "t", "ƫ": "t", "ʈ": "t", "ŧ": "t", "ᵺ": "th", "ɐ": "a", "ᴂ": "ae", "ǝ": "e", "ᵷ": "g", "ɥ": "h", "ʮ": "h", "ʯ": "h", "ᴉ": "i", "ʞ": "k", "ꞁ": "l", "ɯ": "m", "ɰ": "m", "ᴔ": "oe", "ɹ": "r", "ɻ": "r", "ɺ": "r", "ⱹ": "r", "ʇ": "t", "ʌ": "v", "ʍ": "w", "ʎ": "y", "ꜩ": "tz", "ú": "u", "ŭ": "u", "ǔ": "u", "û": "u", "ṷ": "u", "ü": "ue", "ǘ": "u", "ǚ": "u", "ǜ": "u", "ǖ": "u", "ṳ": "u", "ụ": "u", "ű": "u", "ȕ": "u", "ù": "u", "ủ": "u", "ư": "u", "ứ": "u", "ự": "u", "ừ": "u", "ử": "u", "ữ": "u", "ȗ": "u", "ū": "u", "ṻ": "u", "ų": "u", "ᶙ": "u", "ů": "u", "ũ": "u", "ṹ": "u", "ṵ": "u", "ᵫ": "ue", "ꝸ": "um", "ⱴ": "v", "ꝟ": "v", "ṿ": "v", "ʋ": "v", "ᶌ": "v", "ⱱ": "v", "ṽ": "v", "ꝡ": "vy", "ẃ": "w", "ŵ": "w", "ẅ": "w", "ẇ": "w", "ẉ": "w", "ẁ": "w", "ⱳ": "w", "ẘ": "w", "ẍ": "x", "ẋ": "x", "ᶍ": "x", "ý": "y", "ŷ": "y", "ÿ": "y", "ẏ": "y", "ỵ": "y", "ỳ": "y", "ƴ": "y", "ỷ": "y", "ỿ": "y", "ȳ": "y", "ẙ": "y", "ɏ": "y", "ỹ": "y", "ź": "z", "ž": "z", "ẑ": "z", "ʑ": "z", "ⱬ": "z", "ż": "z", "ẓ": "z", "ȥ": "z", "ẕ": "z", "ᵶ": "z", "ᶎ": "z", "ʐ": "z", "ƶ": "z", "ɀ": "z", "ﬀ": "ff", "ﬃ": "ffi", "ﬄ": "ffl", "ﬁ": "fi", "ﬂ": "fl", "ĳ": "ij", "œ": "oe", "ﬆ": "st", "ₐ": "a", "ₑ": "e", "ᵢ": "i", "ⱼ": "j", "ₒ": "o", "ᵣ": "r", "ᵤ": "u", "ᵥ": "v", "ₓ": "x" };
            return input.replace(/[^A-Za-z0-9\[\] ]/g, function(a) { return latinMap[a] || a; });
        };
    });

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

            // 2016-02-14 2dm commented out, don't think the ctId is ever used...
            // Get ID of this content-type 
            svc.ctSvc.getDetails(svc.ctName).then(function (result) {
                svc.ctId = result.data.Id; // 2016-02-14 previously AttributeSetId;
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
    .factory("pipelineService", ["$resource", "$q", "$filter", "eavConfig", "$http", "contentTypeSvc", "metadataSvc", "eavAdminDialogs", "appId", function ($resource, $q, $filter, eavConfig, $http, contentTypeSvc, metadataSvc, eavAdminDialogs, appId) {
        "use strict";
        var svc = {};
        // Web API Service
        svc.pipelineResource = $resource("eav/PipelineDesigner/:action");
        svc.entitiesResource = $resource("eav/Entities/:action");

        svc.appId = 0;

        // Get the Definition of a DataSource
        svc.getDataSourceDefinitionProperty = function (model, dataSource) {
        	var definition = $filter("filter")(model.InstalledDataSources, function(d) {
	            return d.PartAssemblyAndType === dataSource.PartAssemblyAndType;
	        })[0];
        	if (!definition)
        		throw "DataSource Definition not found: " + dataSource.PartAssemblyAndType;
        	return definition;
        };

        // todo refactor: why do we have 2 methods with same name?
        // Extend Pipeline-Model retrieved from the Server
        var postProcessDataSources = function (model) {
            var outDs = eavConfig.pipelineDesigner.outDataSource;
            // Append Out-DataSource for the UI
            model.DataSources.push({
                Name: outDs.name,
                Description: outDs.description,
                EntityGuid: "Out",
                PartAssemblyAndType: outDs.className,
                VisualDesignerData: outDs.visualDesignerData, 
                ReadOnly: true
            });

            // Extend each DataSource with Definition-Property and ReadOnly Status
            angular.forEach(model.DataSources, function(dataSource) {
                dataSource.Definition = function() { return svc.getDataSourceDefinitionProperty(model, dataSource); };
                dataSource.ReadOnly = dataSource.ReadOnly || !model.Pipeline.AllowEdit;
                dataSource.VisualDesignerData = dataSource.VisualDesignerData || { Top: 50, Left: 50 }; // in case server returns null, use a default setting
            });
        };

        angular.extend(svc, { 
            
            // get a Pipeline with Pipeline Info with Pipeline Parts and Installed DataSources
            getPipeline: function(pipelineEntityId) {
                var deferred = $q.defer();

                var httpGetPipe = svc.pipelineResource.get({ action: "GetPipeline", id: pipelineEntityId, appId: svc.appId });
                var getInstalledDataSources = svc.pipelineResource.query({ action: "GetInstalledDataSources" });

                // Join and modify retrieved Data
                $q.all([httpGetPipe.$promise, getInstalledDataSources.$promise]).then(function(results) {
                    var model = JSON.parse(angular.toJson(results[0])); // workaround to remove AngularJS Promise from the result-Objects
                    model.InstalledDataSources = JSON.parse(angular.toJson(results[1]));

                    // Init new Pipeline Object
                    if (!pipelineEntityId) {
                        model.Pipeline = {
                            AllowEdit: "True"
                        };
                    }

                    var outDs = eavConfig.pipelineDesigner.outDataSource;
                    // Add Out-DataSource for the UI
                    model.InstalledDataSources.push({
                        PartAssemblyAndType: outDs.className,
                        Name: outDs.name || outDs.className,
                        In: outDs.in,
                        Out: null,
                        allowNew: false,
                        PrimaryType: "Target",
                        DynamicOut: false
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
                if ($filter("filter")(model.DataSources, function(d) { return d.EntityGuid === "Out"; })[0])
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
            editDataSourcePart: function (dataSource, allSources) {
                var sourceDef = $filter("filter")(allSources, { PartAssemblyAndType: dataSource.PartAssemblyAndType }, true)[0];
                console.log(sourceDef);
                var contentTypeName = (sourceDef && sourceDef.ContentType)
                    ? sourceDef.ContentType
                    : "|Config " + $filter("typename")(dataSource.PartAssemblyAndType, "classFullName"); // todo refactor centralize

                var assignmentObjectTypeId = 4; // todo refactor centralize this constant
                var keyGuid = dataSource.EntityGuid;

                // Query for existing Entity
                metadataSvc.getMetadata(assignmentObjectTypeId, keyGuid, contentTypeName).then(function (result) { 
                    var success = result.data;
                    if (success.length) // Edit existing Entity
                        eavAdminDialogs.openItemEditWithEntityId(success[0].Id);
                    else { // Check if the type exists, and if yes, create new Entity

                        contentTypeSvc(appId, "System").getDetails(contentTypeName, { ignoreErrors: true })
                            .then(function() {
                                var items = [
                                    {
                                        ContentTypeName: contentTypeName,
                                        Metadata: {
                                            TargetType: assignmentObjectTypeId,
                                            KeyType: "guid",
                                            Key: keyGuid
                                        }
                                    }
                                ];
                                eavAdminDialogs.openEditItems(items);
                            },
                            function () {
                                alert("Server reports error - this usually means that this data-source doesn't have any configuration");
                            });



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
    .factory("svcCreator", ["toastr", "$translate", "$timeout", function (toastr, $translate, $timeout) {
        var creator = {};

        // construct a object which has liveListCache, liveListReload(), liveListReset(),  
        creator.implementLiveList = function (getLiveList, disableToastr) {
            var t = {};
            t.disableToastr = !!disableToastr;
            t.liveListCache = [];                   // this is the cached list
            t.liveListCache.isLoaded = false;

            t.liveList = function getAllLive() {
                if (t.liveListCache.length === 0 && !t.liveListCache.isLoaded)
                    t.liveListReload();
                return t.liveListCache;
            };

            // use a promise-result to re-fill the live list of all items, return the promise again
            t._liveListUpdateWithResult = function updateLiveAll(result) {
                if (t.msg.isOpened)
                    toastr.clear(t.msg);
                else {
                    $timeout(300).then(function() {
                            toastr.clear(t.msg);
                        }
                    );
                }
                t.liveListCache.length = 0; // clear
                for (var i = 0; i < result.data.length; i++)
                    t.liveListCache.push(result.data[i]);
                t.liveListCache.isLoaded = true;
                return result;
            };

            t.liveListSourceRead = getLiveList;

            t.liveListReload = function getAll() {
                // show loading - must use the promise-mode because this may be used early before the language has arrived
                $translate("General.Messages.Loading").then(function(msg) {
                    t.msg = toastr.info(msg);
                });
                return t.liveListSourceRead()
                    .then(t._liveListUpdateWithResult);
            };

            t.liveListReset = function resetList() {
                t.liveListCache = [];
            };

            return t;
        };
        return creator;

    }])

;

angular.module("EavServices")
    // the config is important to ensure our toaster has a common setup
    .config(["toastrConfig", function(toastrConfig) {
        angular.extend(toastrConfig, {
            autoDismiss: false,
            containerId: "toast-container",
            maxOpened: 5, // def is 0    
            newestOnTop: true,
            positionClass: "toast-top-right",
            preventDuplicates: false,
            preventOpenDuplicates: false,
            target: "body"
        });
    }])

    .factory("toastrWithHttpErrorHandling", ["toastr", function (toastr) {
        toastr.originalError = toastr.error;
        toastr.error = function errorWithHttpErrorDisplay(messageOrHttpError, title, optionsOverride) {
            var message;
            // test whether bodyOrError is an Error from Web API
            if (messageOrHttpError && messageOrHttpError.data && messageOrHttpError.data.Message) {
                message = messageOrHttpError.data.Message;
                if (messageOrHttpError.data.ExceptionMessage)
                    message += "\n" + messageOrHttpError.data.ExceptionMessage;
            } else
                message = messageOrHttpError;

            toastr.originalError(message, title, optionsOverride);
        };
        return toastr;
    }])

    
    .factory("saveToastr", ["toastr", "$translate", function (toastr, $translate) {
        function saveWithToaster(promise) {
            // todo: replace all this with a single-line calling the promise-toaster below...
            // ? return saveWithToaster(promise, "Message.Saving", "Message.Saved", "Message.ErrorWhileSaving", null, 3000, null);
                var saving = toastr.info($translate.instant("Message.Saving"));
                return promise.then(function(result) {
                    toastr.clear(saving);
                    toastr.success($translate.instant("Message.Saved"), { timeOut: 3000 });
                    return result;
                }, function errorWhileSaving(result) {
                    toastr.clear(saving);
                    toastr.error($translate.instant("Message.ErrorWhileSaving"));
                    return result;
                });
            }

            return saveWithToaster;
    }])

    .factory("promiseToastr", ["toastrWithHttpErrorHandling", "$translate", function (toastrWithHttpErrorHandling, $translate) {
        function saveWithToaster(promise, keyInfo, keyOk, keyError, durInfo, durOk, durError) {
            var toastr = toastrWithHttpErrorHandling;
            var saving = toastr.info($translate.instant(keyInfo));
            return promise.then(function (result) {
                toastr.clear(saving);
                toastr.success($translate.instant(keyOk), { timeOut: durOk || 1000 });
                return result;
            }, function errorWhileSaving(result) {
                toastr.clear(saving);
                toastr.error(result, $translate.instant(keyError));
                return result;
            });
        }

        return saveWithToaster;
    }])
;
angular.module("eavTemplates", []).run(["$templateCache", function($templateCache) {$templateCache.put("content-import-export/content-export.html","\r\n<div class=\"modal-header\">\r\n    <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" icon=\"remove\" ng-click=\"vm.close()\"></button>\r\n    <h3 class=\"modal-title\" translate=\"Content.Export.Title\">cc</h3>\r\n</div>\r\n\r\n<div class=\"modal-body\">\r\n    <div translate=\"Content.Export.Help\"></div>\r\n    <formly-form form=\"vm.form\" model=\"vm.formValues\" fields=\"vm.formFields\">\r\n    </formly-form>\r\n</div>\r\n\r\n<div class=\"modal-footer\">\r\n    <button type=\"button\" class=\"btn btn-primary pull-left\" ng-click=\"vm.exportContent()\" translate=\"Content.Export.Commands.Export\"></button>\r\n</div>");
$templateCache.put("content-import-export/content-import.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n    <!-- HEADER -->\r\n    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" icon=\"remove\" ng-click=\"vm.close()\"></button>\r\n        <h3 class=\"modal-title\"><span  translate=\"Content.Import.Title\"></span> <span ng-show=\"vm.viewStateSelected > 0\" translate=\"Content.Import.TitleSteps\" translate-values=\"{step: vm.viewStateSelected}\"></span></h3>\r\n    </div>\r\n    <!-- END HEADER -->\r\n\r\n    <div ng-switch=\"vm.viewStateSelected\">\r\n\r\n        <!-- FORM -->\r\n        <div ng-switch-when=\"1\">\r\n            <div class=\"modal-body\">\r\n                <div translate=\"Content.Import.Help\"></div>\r\n                <formly-form form=\"vm.form\" model=\"vm.formValues\" fields=\"vm.formFields\"></formly-form>\r\n                <div class=\"text-warning\" translate=\"Content.Import.Messages.BackupContentBefore\"></div>\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <button type=\"button\" class=\"btn btn-primary pull-left\" ng-click=\"vm.evaluateContent()\" ng-disabled=\"!vm.formValues.File || !vm.formValues.File.filename\" translate=\"Content.Import.Commands.Preview\"></button>\r\n            </div>\r\n        </div>\r\n        <!-- END FORM -->\r\n\r\n\r\n        <!-- WAITING -->\r\n        <div ng-switch-when=\"0\">\r\n            <div class=\"modal-body\"> {{\'Content.Import.Messages.WaitingForResponse\' | translate}}\r\n            </div>\r\n        </div>\r\n        <!-- END WAITING -->\r\n\r\n\r\n        <!-- EVALUATION RESULT -->\r\n        <div ng-switch-when=\"2\">\r\n            <div class=\"modal-body\">\r\n                <!-- DETAILS / STATISTICS -->\r\n                <div ng-if=\"vm.evaluationResult.Succeeded\">\r\n                    <h4 translate=\"Content.Import.Evaluation.Detail.Title\" translate-values=\"{filename: vm.formValues.File.filename}\"></h4>\r\n                    <h5 translate=\"Content.Import.Evaluation.Detail.File.Title\"></h5>\r\n                    <ul>\r\n                        <li translate=\"Content.Import.Evaluation.Detail.File.ElementCount\" translate-values=\"{count: vm.evaluationResult.Detail.DocumentElementsCount}\"></li>\r\n                        <li translate=\"Content.Import.Evaluation.Detail.File.LanguageCount\" translate-values=\"{count: vm.evaluationResult.Detail.LanguagesInDocumentCount}\"></li>\r\n                        <li translate=\"Content.Import.Evaluation.Detail.File.Attributes\" translate-values=\"{count: vm.evaluationResult.Detail.AttributeNamesInDocument.length, attributes: vm.evaluationResult.Detail.AttributeNamesInDocument.join(\', \')}\"></li>\r\n                    </ul>\r\n                    <h5 translate=\"Content.Import.Evaluation.Detail.Entities.Title\"></h5>\r\n                    <ul>\r\n                        <li translate=\"Content.Import.Evaluation.Detail.Entities.Create\" translate-values=\"{count: vm.evaluationResult.Detail.AmountOfEntitiesCreated}\"></li>\r\n                        <li translate=\"Content.Import.Evaluation.Detail.Entities.Update\" translate-values=\"{count: vm.evaluationResult.Detail.AmountOfEntitiesUpdated}\"></li>\r\n                        <li translate=\"Content.Import.Evaluation.Detail.Entities.Delete\" translate-values=\"{count: vm.evaluationResult.Detail.AmountOfEntitiesDeleted}\"></li>\r\n                        <li translate=\"Content.Import.Evaluation.Detail.Entities.AttributesIgnored\" translate-values=\"{count: vm.evaluationResult.Detail.AttributeNamesNotImported.length, attributes: vm.evaluationResult.Detail.AttributeNamesNotImported.join(\', \')}\"></li>\r\n                    </ul>\r\n                    <div class=\"text-warning\" translate=\"Content.Import.Messages.ImportCanTakeSomeTime\"></div>\r\n                </div>\r\n                <!-- END DETAILS / STATISTICS -->\r\n                <!-- ERRORS -->\r\n                <div ng-if=\"!vm.evaluationResult.Succeeded\">\r\n                    <h4 translate=\"Content.Import.Evaluation.Error.Title\" translate-values=\"{filename: vm.formValues.File.filename}\"></h4>\r\n                    <ul>\r\n                        <li ng-repeat=\"error in vm.evaluationResult.Detail\">\r\n                            <div><span translate=\"Content.Import.Evaluation.Error.Codes.{{error.ErrorCode}}\"></span></div>\r\n                            <div ng-if=\"error.ErrorDetail\"><i translate=\"Content.Import.Evaluation.Error.Detail\" translate-values=\"{detail: error.ErrorDetail}\"></i>\r\n                            </div>\r\n                            <div ng-if=\"error.LineNumber\"><i translate=Content.Import.Evaluation.Error.LineNumber\" translate-values=\"{number: error.LineNumber}\"></i>\r\n                            </div>\r\n                            <div ng-if=\"error.LineDetail\"><i translate=\"Content.Import.Evaluation.Error.LineDetail\" translate-values=\"{detail: error.LineDetail}\"></i>\r\n                            </div>\r\n                        </li>\r\n                    </ul>\r\n                </div>\r\n                <!-- END ERRORS -->\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <button type=\"button\" class=\"btn pull-left\" ng-click=\"vm.back()\" icon=\"arrow-left\"></button>\r\n                <button type=\"button\" class=\"btn btn-default pull-left\" ng-click=\"vm.importContent()\" translate=\"Content.Import.Commands.Import\" ng-disabled=\"!vm.evaluationResult.Succeeded\"></button>\r\n            </div>\r\n        </div>\r\n        <!-- END EVALUATION RESULT -->\r\n\r\n\r\n        <!-- IMPORT RESULT -->\r\n        <div ng-switch-when=\"3\">\r\n            <div class=\"modal-body\">\r\n                <span ng-show=\"vm.importResult.Succeeded\" translate=\"Content.Import.Messages.ImportSucceeded\"></span>\r\n                <span ng-hide=\"vm.importResult.Succeeded\" translate=\"Content.Import.Messages.ImportFailed\"></span>\r\n            </div>\r\n        </div>\r\n        <!-- END IMPORT RESULT -->\r\n\r\n        <div ng-if=\"vm.debug.on\">\r\n            <h3>Debug infos</h3>\r\n            <pre>{{vm.formValues | json}}</pre>\r\n        </div>\r\n    </div>\r\n</div>");
$templateCache.put("content-items/content-edit.html","<div class=\"modal-header\">\r\n    <button type=\"button\" class=\"btn btn-default btn-subtle\" ng-click=\"vm.history()\">\r\n        <span class=\"glyphicon glyphicon-time\"> history / todo </span>\r\n    </button>\r\n    <h3 class=\"modal-title\">Edit / New Content</h3>\r\n</div>\r\n\r\n<div class=\"modal-body\">\r\n    this is where the edit appears. Would edit entity {{vm.entityId}} or add a {{vm.contentType}} - depending on the mode: {{vm.mode}}\r\n    <h3>Use cases</h3>\r\n    <ol>\r\n        <li>Edit an existing entity with ID</li>\r\n        <li>Create a new entity of a certaint content-type, just save and done (like from a \"new\" button without content-group)</li>\r\n        <li>Create a new entity of a certain type and assign it to a metadata thing (guid, int, string)</li>\r\n\r\n        <li>Create a new entity and put it into a content-group at the right place</li>\r\n        <li>Edit content-group: item + presentation </li>\r\n        <li>Edit multiple IDs/or new/mix: Edit multiple items with IDs</li>\r\n    </ol>\r\n\r\n    init of 1 edit\r\n    - entity-id in storage\r\n    - new-type + optional: assignment-id + assignment-type\r\n\r\n    - array of the above\r\n    --- [{id 17}, {type: \"person\"}, {type: person, asstype: 4, target: 0205}]\r\n\r\n    - content-group\r\n</div>\r\n");
$templateCache.put("content-items/content-items-agnostic.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\" class=\"content-items-agnostic\">\r\n    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\"><i icon=\"remove\"></i></button>\r\n        <h3 class=\"modal-title\" translate=\"Content.Manage.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <button type=\"button\" class=\"btn btn-primary btn-square\" ng-click=\"vm.add()\"><i icon=\"plus\"></i></button>\r\n        <button type=\"button\"\r\n                class=\"btn btn-default btn-square\"\r\n                uib-tooltip=\"{{ \'ContentTypes.Buttons.Export\' | translate }}\"\r\n                ng-click=\"vm.openExport()\">\r\n            <i icon=\"export\"></i>\r\n        </button>\r\n	    <button ng-if=\"vm.debug.on\" type=\"button\" class=\"btn btn-warning btn-square\" ng-click=\"vm.refresh()\"><i icon=\"repeat\"></i></button>\r\n        <button ng-if=\"vm.debug.on\" type=\"button\" class=\"btn btn-warning btn-square\" ng-click=\"vm.debugFilter()\"><i icon=\"filter\"></i></button>\r\n\r\n		<div ag-grid=\"vm.gridOptions\" class=\"ag-grid-wrapper\"></div>\r\n	    \r\n        <show-debug-availability class=\"pull-right\" ></show-debug-availability>\r\n    </div>\r\n</div>");
$templateCache.put("content-items/history-details.html","<div>\r\n    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-subtle btn-square pull-right\" type=\"button\" ng-click=\"vm.close()\">\r\n            <span class=\"glyphicon glyphicon-remove\"></span>\r\n        </button>\r\n        <h3 class=\"modal-title\">History Details {{vm.ChangeId}} of {{vm.entityId}}</h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <h1>todo</h1>\r\n        <table class=\"table table-striped table-hover\">\r\n            <thead>\r\n            <tr>\r\n                <th>Field</th>\r\n                <th>Language</th>\r\n                <th>Value</th>\r\n                <th>SharedWith</th>\r\n            </tr>\r\n            </thead>\r\n            <tbody>\r\n            <tr ng-repeat=\"item in vm.items | orderBy:SysCreatedDate:reverse\">\r\n                <td>{{item.Field}}</td>\r\n                <td>{{item.Language}}</td>\r\n                <td>{{item.Value}}</td>\r\n                <td>{{item.SharedWith}}</td>\r\n\r\n            </tr>\r\n            <tr ng-if=\"!vm.items.length\">\r\n                <td colspan=\"100\">No History</td>\r\n            </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n        <button class=\"btn btn-primary pull-right\" type=\"button\" ng-click=\"vm.restore()\">\r\n            <span class=\"glyphicon glyphicon-ok\">todo restore</span>\r\n        </button>    </div>\r\n\r\n</div>");
$templateCache.put("content-items/history.html","<div>\r\n    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\">\r\n            <span class=\"glyphicon glyphicon-remove\"></span>\r\n        </button>\r\n        <h3 class=\"modal-title\">{{ \"Content.History.Title\" | translate:\'{ id:vm.entityId }\' }}History of {{vm.entityId}}</h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n\r\n        <table class=\"table table-striped table-hover\">\r\n            <thead>\r\n            <tr>\r\n                <th translate=\"Content.History.Table.Id\"></th>\r\n                <th translate=\"Content.History.Table.When\"></th>\r\n                <th translate=\"Content.History.Table.User\"></th>\r\n                <th translate=\"Content.History.Table.Action\"></th>\r\n            </tr>\r\n            </thead>\r\n            <tbody>\r\n            <tr ng-repeat=\"item in vm.items | orderBy:SysCreatedDate:reverse\">\r\n                <td><span uib-tooltip=\"ChangeId: {{item.ChangeId}}\">{{item.VirtualVersion}}</span></td>\r\n                <td>{{item.SysCreatedDate.replace(\"T\", \" \")}}</td>\r\n                <td>{{item.User}}</td>\r\n                <td>\r\n                    <button type=\"button\" class=\"btn btn-xs\" ng-click=\"vm.details(item)\">\r\n                        <span class=\"glyphicon glyphicon-search\"></span>\r\n                    </button>\r\n                </td>\r\n            </tr>\r\n                <tr ng-if=\"!vm.items.length\">\r\n                    <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n\r\n</div>");
$templateCache.put("content-types/content-types-edit.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\"><i icon=\"remove\"></i></button>\r\n        <h3 class=\"modal-title\" translate=\"ContentTypeEdit.Title\"></h3>\r\n    </div>\r\n\r\n    <div class=\"modal-body\">\r\n        {{ \"ContentTypeEdit.Name\" | translate }}: <br />\r\n        <input ng-model=\"vm.item.Name\" class=\"input-lg\" />\r\n        <br />\r\n        {{ \"ContentTypeEdit.Description\" | translate }}: <br />\r\n        <input ng-model=\"vm.item.Description\" class=\"input-lg\"/><br/>\r\n\r\n        <div>\r\n            {{ \"ContentTypeEdit.Scope\" | translate }}: <br />\r\n            <span ng-if=\"vm.debug.on\">\r\n                <div class=\"alert alert-danger\">the scope should almost never be changed - <a href=\"http://2sxc.org/help?tag=scope\" _target=\"_blank\">see help</a></div>\r\n            </span>\r\n            <input ng-disabled=\"!vm.debug.on\" ng-model=\"vm.item.Scope\" class=\"input-lg\" />\r\n        </div>\r\n\r\n        <div ng-if=\"vm.debug.on\" class=\"alert-danger\">\r\n            <h3>Static Name</h3>\r\n            <input type=\"checkbox\" class=\"input-lg\" ng-model=\"vm.item.ChangeStaticName\"/> Really edit StaticName??? - this is usually a very bad idea\r\n            <br/>\r\n            <input ng-model=\"vm.item.NewStaticName\" ng-disabled=\"!vm.item.ChangeStaticName\" class=\"input-lg\"/>\r\n        </div>\r\n        <div ng-if=\"vm.debug.on\" class=\"alert-danger\">\r\n            <h3>Shared Content Type (Ghost)</h3>\r\n            <div>Note: this can\'t be edited in the UI, for now if you really know what you\'re doing, do it in the DB</div>\r\n            <div>Uses Type Definition of: {{vm.item.SharedDefId}}</div> \r\n        </div>\r\n    </div>\r\n    <div class=\"modal-footer\">\r\n        <button class=\"btn btn-primary btn-square pull-left btn-lg\" type=\"button\" ng-click=\"vm.ok()\"><i icon=\"ok\"></i></button>\r\n        <show-debug-availability class=\"pull-right\" style=\"margin-top: 20px;\"></show-debug-availability>\r\n    </div>\r\n</div>");
$templateCache.put("content-types/content-types-field-edit.html","<div class=\"modal-header\">\r\n    <button icon=\"remove\" class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n    <h3 class=\"modal-title\" translate=\"Fields.TitleEdit\"></h3>\r\n</div>\r\n<div class=\"modal-body\">\r\n    <table class=\"table table-hover table-manage-eav\">\r\n        <thead>\r\n        <tr>\r\n            <th translate=\"Fields.Table.Name\" style=\"width: 33%\"></th>\r\n            <th translate=\"Fields.Table.DataType\" style=\"width: 33%\">Data Type</th>\r\n            <th translate=\"Fields.Table.InputType\" style=\"width: 33%\">Input Type</th>\r\n        </tr>\r\n        </thead>\r\n        <tbody>\r\n        <tr ng-repeat=\"item in vm.items\">\r\n            <td>\r\n                <input ng-model=\"item.StaticName\" ng-required=\"true\" class=\"input-lg\" style=\"width: 100%\" disabled=\"disabled\"/>\r\n            </td>\r\n            <td>\r\n                <input ng-model=\"item.Type\" disabled=\"disabled\" class=\"input-lg\" style=\"width: 100%\"/>\r\n            </td>\r\n            <td>\r\n                <select class=\"input-lg\" ng-model=\"item.InputType\" style=\"width: 100%\"\r\n                        uib-tooltip=\"{{ (vm.allInputTypes | filter: { inputType: item.InputType})[0].description }}\"\r\n                        ng-options=\"o.inputType as o.label for o in vm.allInputTypes | filter: {dataType: item.Type.toLowerCase() } \">\r\n                </select>\r\n\r\n            </td>\r\n        </tr>\r\n        </tbody>\r\n    </table>\r\n</div>\r\n<div class=\"modal-footer\">\r\n    <button icon=\"ok\" class=\"btn btn-lg btn-primary btn-square pull-left\" type=\"button\" ng-click=\"vm.ok()\"></button>\r\n</div>");
$templateCache.put("content-types/content-types-fields-add.html","<div class=\"modal-header\">\r\n    <button icon=\"remove\" class=\"btn btn-default btn-square btn-subtle pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n    <h3 class=\"modal-title\" translate=\"Fields.TitleEdit\"></h3>\r\n</div>\r\n<div class=\"modal-body\">\r\n    <table class=\"table table-hover table-manage-eav\">\r\n        <thead>\r\n        <tr>\r\n            <th translate=\"Fields.Table.Name\" style=\"width: 33%\"></th>\r\n            <th translate=\"Fields.Table.DataType\" style=\"width: 33%\">Data Type</th>\r\n            <th translate=\"Fields.Table.InputType\" style=\"width: 33%\">Input Type</th>\r\n        </tr>\r\n        </thead>\r\n        <tbody>\r\n        <tr ng-repeat=\"item in vm.items\">\r\n            <td>\r\n                <input ng-model=\"item.StaticName\" ng-required=\"true\" class=\"input-lg\" style=\"width: 100%\"/>\r\n            </td>\r\n            <td>\r\n                <select class=\"input-lg\" ng-model=\"item.Type\" style=\"width: 100%\"\r\n                        uib-tooltip=\"{{ \'DataType.\' + item.Type + \'.Explanation\' | translate }}\"\r\n                        ng-options=\"o as \'DataType.\' + o + \'.Choice\' | translate for o in vm.types | orderBy: \'toString()\' \"\r\n                        ng-change=\"vm.resetSubTypes(item)\">\r\n                    <option>-- select --</option>\r\n                </select>\r\n            </td>\r\n            <td>\r\n                <select class=\"input-lg\" ng-model=\"item.InputType\" style=\"width: 100%\"\r\n                        uib-tooltip=\"{{ (vm.allInputTypes | filter: { inputType: item.InputType})[0].description }}\"\r\n                        ng-options=\"o.inputType as o.label for o in vm.allInputTypes | filter: {dataType: item.Type.toLowerCase() } \">\r\n                </select>\r\n\r\n            </td>\r\n        </tr>\r\n        </tbody>\r\n    </table>\r\n</div>\r\n<div class=\"modal-footer\">\r\n    <button icon=\"ok\" class=\"btn btn-lg btn-primary btn-square pull-left\" type=\"button\" ng-click=\"vm.ok()\"></button>\r\n</div>");
$templateCache.put("content-types/content-types-fields.html","<div>\r\n\r\n    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-subtle btn-square pull-right\" type=\"button\" ng-click=\"vm.close()\"><i icon=\"remove\"></i></button>\r\n        <h3 class=\"modal-title\" translate=\"Fields.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <button icon=\"plus\" ng-click=\"vm.add()\" class=\"btn btn-primary btn-square\"></button>\r\n\r\n        <!-- Table of content types for editing -->\r\n        <table ui-tree=\"vm.treeOptions\" data-drag-enabled=\"vm.dragEnabled\" ui-tree-nodes ng-model=\"vm.items\" class=\"table table-hover table-manage-eav eav-admin-field-list\">\r\n            <thead>\r\n                <tr>\r\n                    <th class=\"mini-btn-1\"></th>\r\n                    <th translate=\"Fields.Table.Title\" class=\"mini-btn-1\"></th>\r\n                    <th translate=\"Fields.Table.Name\" style=\"width: 35%\"></th>\r\n                    <th translate=\"Fields.Table.DataType\" style=\"width: 20%\"></th>\r\n                    <th translate=\"Fields.Table.InputType\" style=\"width: 20%\"></th>\r\n                    <th translate=\"Fields.Table.Label\" style=\"width: 30%\"></th>\r\n                    <th translate=\"Fields.Table.Notes\" style=\"width: 50%\"></th>\r\n                    <!--<th translate=\"Fields.Table.Sort\" class=\"mini-btn-2\"></th>-->\r\n                    <th translate=\"Fields.Table.Action\" class=\"mini-btn-2\"></th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                <tr ng-repeat=\"item in vm.items track by $id(item)\" ui-tree-node class=\"clickable-row\" ng-click=\"vm.createOrEditMetadata(item, item.Type)\" ng-class=\"[ \'type-\' + item.Type.toLowerCase(), \'subtype-\' + item.InputType.substring(item.InputType.indexOf(\'-\') + 1, 100)]\">\r\n                    <td ui-tree-handle><i class=\"glyphicon glyphicon-sort\"></i></td>\r\n                    <td stop-event=\"click\">\r\n                        <button type=\"button\" class=\"btn btn-xs btn-square\" ng-style=\"(item.IsTitle ? \'\' : \'color: transparent !important\')\" ng-click=\"vm.setTitle(item)\">\r\n                            <i icon=\"{{item.IsTitle ? \'star\' : \'star-empty\'}}\"></i>\r\n                        </button>\r\n                    </td>\r\n                    <td class=\"clickable\"><span uib-tooltip=\"{{ \'Id: \' + item.Id}}\">{{item.StaticName}}</span></td>\r\n                    <td class=\"text-nowrap clickable\">\r\n                        {{item.Type}}\r\n                    </td>\r\n                    <td class=\"text-nowrap InputType\" stop-event=\"click\">\r\n                        <span class=\"clickable\" uib-tooltip=\"{{ vm.inputTypeTooltip(item.InputType) }}\" ng-click=\"vm.edit(item)\">\r\n                            <i icon=\"pencil\"></i>\r\n                            {{item.InputType.substring(item.InputType.indexOf(\'-\') + 1, 100)}}\r\n                        </span>\r\n                    </td>\r\n                    <td class=\"text-nowrap clickable\">\r\n                        {{item.Metadata.All.Name}}\r\n                    </td>\r\n                    <!--<td>-</td>-->\r\n                    <td class=\"text-nowrap clickable\">\r\n                        <div class=\"hide-overflow-text\">\r\n                            {{item.Metadata.All.Notes}}\r\n                        </div>\r\n                    </td>\r\n\r\n                    <!--<td class=\"text-nowrap\" stop-event=\"click\">\r\n                        <button icon=\"arrow-up\" type=\"button\" class=\"btn btn-xs btn-square\" ng-disabled=\"$first\" ng-click=\"vm.moveUp(item)\"></button>\r\n                        <button icon=\"arrow-down\" type=\"button\" class=\"btn btn-xs btn-square\" ng-disabled=\"$last\" ng-click=\"vm.moveDown(item)\"></button>\r\n                    </td>-->\r\n                    <td stop-event=\"click\">\r\n                        <button icon=\"cog\" type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.rename(item)\"></button>\r\n                        <button icon=\"remove\" type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.tryToDelete(item)\"></button>\r\n                    </td>\r\n                </tr>\r\n                <tr ng-if=\"!vm.items.length\">\r\n                    <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n        <!--Ordered  {{vm.orderList()}} Test-->\r\n    </div>\r\n\r\n\r\n</div>");
$templateCache.put("content-types/content-types.html","<div ng-controller=\"List as vm\" ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n    <div class=\"modal-header\">\r\n        <h3 class=\"modal-title\" translate=\"ContentTypes.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n        <!-- Buttons on top -->\r\n        <button title=\"{{ \'General.Buttons.Add\' | translate }}\" type=\"button\" class=\"btn btn-primary btn-square\" ng-click=\"vm.edit()\"><i icon=\"plus\"></i></button>\r\n\r\n\r\n        <span class=\"btn-group\" ng-if=\"vm.debug.on\">\r\n            <button title=\"{{ \'General.Buttons.Refresh\' | translate }}\" type=\"button\" class=\"btn btn-warning btn-square\" ng-click=\"vm.refresh()\"><i icon=\"repeat\"></i></button>\r\n            <button title=\"todo\" type=\"button\" class=\"btn btn-warning btn-icon\" ng-click=\"vm.createGhost()\"><i class=\"eav-icon-ghost\"></i></button>\r\n            <button title=\"{{ \'ContentTypes.Buttons.ChangeScope\' | translate }}\" type=\"button\" class=\"btn btn-warning btn-square\" ng-click=\"vm.changeScope()\"><i icon=\"record\"></i></button>\r\n            <button title=\"{{ \'General.Buttons.System\' | translate }}\" type=\"button\" class=\"btn btn-warning btn-square\" ng-click=\"vm.liveEval()\"><i icon=\"flash\"></i></button>\r\n        </span>\r\n        <!-- Table of content types for editing -->\r\n        <table class=\"table table-hover\" style=\"table-layout: fixed; width: 100%\">\r\n            <thead>\r\n            <tr>\r\n                <!--<th translate=\"ContentTypes.TypesTable.Items\" class=\"col-id\"></th>-->\r\n                <th translate=\"ContentTypes.TypesTable.Name\" style=\"width: 50%\"></th>\r\n                <th class=\"mini-btn-1\"></th>\r\n                <th translate=\"ContentTypes.TypesTable.Description\" style=\"width: 50%\"></th>\r\n                <th translate=\"ContentTypes.TypesTable.Fields\" class=\"mini-btn-2\"></th>\r\n                <th translate=\"ContentTypes.TypesTable.Actions\" class=\"mini-btn-5\"></th>\r\n                <th class=\"mini-btn-1\"> </th>\r\n            </tr>\r\n            </thead>\r\n            <tbody>\r\n            <tr ng-if=\"vm.items.isLoaded\" ng-repeat=\"item in vm.items | orderBy:\'Name\'\" class=\"clickable-row\" ng-click=\"vm.editItems(item)\">\r\n                <!--<td style=\"text-align: center\" class=\"clickable\"> {{item.Items}} </td>-->\r\n                <td class=\"clickable\">\r\n                    <span class=\"text-nowrap hide-overflow-text\" style=\"max-width: 400px\" uib-tooltip=\"{{item.Label}} ({{item.Name}})\">{{item.Label}} <span ng-if=\"item.Name != item.Label\">({{item.Name}})</span></span>\r\n                </td>\r\n                <td class=\"clickable\" style=\"text-align: right\">\r\n                    <div class=\"badge pull-right badge-primary hover-pair\" stop-event=\"click\" ng-click=\"vm.addItem(item.StaticName)\"><span class=\"hover-default\">{{item.Items}}</span><span class=\"hover-hover eav-icon-plus\"></span></div></td>\r\n                <td class=\"clickable\">\r\n                    <div class=\"text-nowrap hide-overflow-text\" style=\"max-width: 500px\" uib-tooltip=\"{{item.Description}}\">{{item.Description}}</div>\r\n                </td>\r\n                <td stop-event=\"click\">\r\n                    <button ng-if=\"!item.UsesSharedDef\" type=\"button\" class=\"btn btn-xs\" style=\"width: 60px\" ng-click=\"vm.editFields(item)\">\r\n                        <i class=\"eav-icon-fields\"></i>&nbsp;<span style=\"width: 22px; text-align: right\">{{item.Fields}}</span>\r\n                    </button>\r\n                    <button ng-if=\"item.UsesSharedDef\" uib-tooltip=\"{{ \'ContentTypes.Messages.SharedDefinition\' | translate:item }}\" type=\"button\" class=\"btn btn-default btn-xs\" style=\"width: 60px\">\r\n                        <i class=\"eav-icon-ghost\"></i>&nbsp;<span style=\"width: 22px; text-align: right\">{{item.Fields}}</span>\r\n                    </button>\r\n                </td>\r\n\r\n                <td class=\"text-nowrap\" stop-event=\"click\">\r\n                    <span class=\"btn-group\">\r\n                        <button uib-tooltip=\"{{ \'General.Buttons.Rename\' | translate }} - {{  \'ContentTypes.Messages.Type\' + (item.UsesSharedDef ? \'Shared\' : \'Own\')  | translate:item }}\" type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.edit(item)\">\r\n                            <i icon=\"heart{{ (item.UsesSharedDef ? \'-empty\' : \'\') }}\"></i>\r\n                        </button>\r\n                        <button uib-tooltip=\"{{ \'General.Buttons.Metadata\' | translate }}\" type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.createOrEditMetadata(item)\">\r\n                            <i class=\"eav-icon-tag\"></i>\r\n                        </button>\r\n                        <button uib-tooltip=\"{{ \'ContentTypes.Buttons.Export\' | translate }}\" type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.openExport(item)\">\r\n                            <i icon=\"export\"></i>\r\n                        </button>\r\n                        <button uib-tooltip=\"{{ \'ContentTypes.Buttons.Import\' | translate }}\" type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.openImport(item)\">\r\n                            <i icon=\"import\"></i>\r\n                        </button>\r\n                        <button type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.permissions(item)\" ng-if=\"vm.isGuid(item.StaticName)\">\r\n                            <i icon=\"user\"></i>\r\n                        </button>\r\n                    </span>\r\n                </td>\r\n                <td stop-event=\"click\">\r\n                    <button icon=\"remove\" type=\"button\" class=\"btn btn-xs\" ng-click=\"vm.tryToDelete(item)\"></button>\r\n                </td>\r\n            </tr>\r\n            <tr ng-if=\"!vm.items.length\">\r\n                <td colspan=\"100\">{{ \'General.Messages.Loading\' | translate }} / {{ \'General.Messages.NothingFound\' | translate }}</td>\r\n            </tr>\r\n            </tbody>\r\n        </table>\r\n        <show-debug-availability class=\"pull-right\"></show-debug-availability>\r\n    </div>\r\n    <div ng-if=\"vm.debug.on\">\r\n\r\n        <h3>Notes / Debug / ToDo</h3>\r\n        <ol>\r\n            <li>get validators to work on all dialogs</li>\r\n        </ol>\r\n    </div>\r\n</div>\r\n");
$templateCache.put("permissions/permissions.html","    <div class=\"modal-header\">\r\n        <button class=\"btn btn-default btn-square pull-right\" type=\"button\" ng-click=\"vm.close()\"><i icon=\"remove\"></i></button>\r\n        <h3 class=\"modal-title\" translate=\"Permissions.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body\">\r\n\r\n        <button type=\"button\" class=\"btn btn-primar btn-square\" ng-click=\"vm.add()\"><i icon=\"plus\"></i></button>\r\n        <button ng-if=\"vm.debug.on\" type=\"button\" class=\"btn btn-square\" ng-click=\"vm.refresh()\"><i icon=\"repeat\"></i></button>\r\n\r\n        <table class=\"table table-striped table-hover table-manage-eav\">\r\n            <thead>\r\n            <tr>\r\n                <th translate=\"Permissions.Table.Id\" style=\"width: 60px\"></th>\r\n                <th translate=\"Permissions.Table.Name\" style=\"width: 33%\"></th>\r\n                <th translate=\"Permissions.Table.Condition\" style=\"width: 33%\"></th>\r\n                <th translate=\"Permissions.Table.Grant\" style=\"width: 33%\"></th>\r\n                <th translate=\"Permissions.Table.Actions\" style=\"width: 40px\"></th>\r\n            </tr>\r\n            </thead>\r\n            <tbody>\r\n            <tr ng-repeat=\"item in vm.items | orderBy:\'Title\'\" class=\"clickable-row\" ng-click=\"vm.edit(item)\">\r\n                <td class=\"clickable\">{{item.Id}}</td>\r\n                <td class=\"clickable\">{{item.Title}}</td>\r\n                <td class=\"clickable\">{{item.Condition}}</td>\r\n                <td class=\"clickable\">{{item.Grant}}</td>\r\n                <td class=\"text-nowrap\" stop-event=\"click\">\r\n                    <button icon=\"remove\" type=\"button\" class=\"btn btn-xs btn-square\" ng-click=\"vm.tryToDelete(item)\"></button>\r\n                </td>\r\n            </tr>\r\n            <tr ng-if=\"!vm.items.length\">\r\n                <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n            </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>");
$templateCache.put("pipelines/pipeline-designer.html","<div class=\"ng-cloak\">\r\n    <div ng-controller=\"PipelineDesignerController as vm\" ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n        <div id=\"pipelineContainer\">\r\n            <div ng-repeat=\"dataSource in queryDef.data.DataSources\"\r\n                 datasource\r\n                 guid=\"{{dataSource.EntityGuid}}\"\r\n                 id=\"dataSource_{{dataSource.EntityGuid}}\"\r\n                 class=\"dataSource\"\r\n                 ng-attr-style=\"{{ \'top: \' + dataSource.VisualDesignerData.Top +\'px; left: \' + dataSource.VisualDesignerData.Left + \'px; \' + (dataSource.VisualDesignerData.Width ? \'min-width: \' + dataSource.VisualDesignerData.Width + \'px\' : \'\') }}\">\r\n                <div class=\"configure\" ng-click=\"configureDataSource(dataSource)\" title=\"Configure this DataSource\" \r\n                     ng-if=\"!dataSource.ReadOnly && vm.typeInfo(dataSource).config\">\r\n                    <i class=\"eav-icon-settings\"></i>\r\n                </div>\r\n                <i class=\"type-info {{vm.typeInfo(dataSource).icon}}\" title=\"{{vm.typeInfo(dataSource).notes}}\"></i>\r\n                <div class=\"name noselect\" title=\"Click to edit the Name\" ng-click=\"editName(dataSource)\">\r\n                    {{dataSource.Name || \'(no name)\'}}<i class=\"show-hover-inline eav-icon-pencil\"></i>\r\n                </div><br />\r\n                <div class=\"description noselect\" title=\"Click to edit the Description\" ng-click=\"editDescription(dataSource)\">\r\n                    {{dataSource.Description}}<i class=\"show-hover-inline eav-icon-pencil\"></i>\r\n                </div><br />\r\n                <div class=\"typename\" ng-attr-title=\"{{dataSource.PartAssemblyAndType}}\">\r\n                    Type: {{dataSource.PartAssemblyAndType | typename: \'className\'}}\r\n                </div>\r\n                <div class=\"add-endpoint\" title=\"Drag a new Out-Connection from here\" \r\n                     ng-if=\"!dataSource.ReadOnly && vm.typeInfo(dataSource).dynamicOut\">\r\n                    <i class=\"new-connection eav-icon-up-dir\"></i>\r\n                </div>\r\n                <div class=\"delete eav-icon-cancel\" title=\"delete this\" ng-click=\"vm.remove($index)\" ng-if=\"!dataSource.ReadOnly\"></div>\r\n                <a class=\"help eav-icon-help-circled\" title=\"help for this data source\" \r\n                   href=\"{{vm.typeInfo(dataSource).helpLink}}\" target=\"_blank\"\r\n                   ng-if=\"vm.typeInfo(dataSource).helpLink\"></a>\r\n            </div>\r\n        </div>\r\n        <div class=\"actions panel panel-default\">\r\n            <div class=\"panel-heading\">\r\n                <span class=\"pull-left\">Actions</span>\r\n                <a href=\"http://2sxc.org/help\" class=\"btn btn-info btn-xs pull-right\" target=\"_blank\"><i class=\"eav-icon-info-circled\"></i> Help</a>\r\n            </div>\r\n            <div class=\"panel-body\">\r\n                <div class=\"btn-group\" role=\"group\" style=\"width: 100%\">\r\n                    <button type=\"button\" class=\"btn btn-primary btn-block\"\r\n                            title=\"Query the Data of this Pipeline. Note that it doesn\'t save changes - so if you have unexpected behaviour after rewiring - save first\"\r\n                            ng-click=\"queryPipeline(true)\"\r\n                            style=\"width: 75%\">\r\n                        <i class=\"eav-icon-ok\"></i> &amp;\r\n                        <i class=\"eav-icon-play\"></i>Query\r\n                    </button>\r\n                    <button type=\"button\" class=\"btn btn-primary btn-block\" title=\"Quick Query without saving (using server definition)\"\r\n                            ng-click=\"queryPipeline(false)\"\r\n                            style=\"width: 25%; margin-top: 4px\">\r\n                        <i class=\"eav-icon-play\"></i>\r\n                    </button>\r\n                </div>\r\n\r\n                <select class=\"form-control\" ng-model=\"addDataSourceType\" \r\n                        ng-disabled=\"queryDef.readOnly\" \r\n                        ng-change=\"vm.addSelectedDataSource()\" \r\n                        ng-options=\"d.Name for d in queryDef.data.InstalledDataSources | filter: {allowNew: \'!false\'} | orderBy: \'Name\'\">\r\n                    <option value=\"\">-- Add DataSource --</option>\r\n                </select>\r\n                <button type=\"button\" class=\"btn btn-primary btn-block\" ng-disabled=\"queryDef.readOnly\" ng-click=\"savePipeline()\">\r\n                    <i class=\"eav-icon-ok\"></i> Save\r\n                </button>\r\n                <br/>\r\n                \r\n                <!-- test parameters -->\r\n                <div>\r\n                    <div>\r\n                        <strong>Test Parameters</strong>\r\n                        <i class=\"eav-icon-pencil\" ng-click=\"editPipelineEntity()\"></i>\r\n                    </div>\r\n                    <div><ul><li ng-repeat=\"param in queryDef.data.Pipeline.TestParameters.split(\'\\n\')\">\r\n                                {{param}}\r\n                            </li>\r\n                        </ul>\r\n                    </div>\r\n                </div>\r\n\r\n\r\n                <!-- show warnings if detected -->\r\n                <div ng-if=\"vm.warnings.length\">\r\n                    <div><i class=\"eav-icon-attention\" style=\"color: red\"></i><strong>Warnings</strong></div>\r\n                    <ol>\r\n                        <li ng-repeat=\"warn in vm.warnings\">{{warn}}</li>\r\n                    </ol>\r\n                    <br />\r\n                </div>\r\n                \r\n                <!-- show description if available -->\r\n                <div ng-if=\"queryDef.data.Pipeline.Description\">\r\n                    <div><strong>Query Description</strong></div>\r\n                    <div>{{queryDef.data.Pipeline.Description}}</div>\r\n                </div>\r\n                <br/>\r\n                <button type=\"button\" class=\"btn btn-info btn-xs\" ng-click=\"toggleEndpointOverlays()\"><i class=\"eav-icon-info-circled\"></i> {{showEndpointOverlays ? \'Hide\' : \'Show\' }} Overlays</button>\r\n                <button type=\"button\" class=\"btn btn-info btn-xs\" ng-click=\"repaint()\"><i class=\"eav-icon-reload\"></i> Repaint</button>\r\n                <button type=\"button\" class=\"btn btn-info btn-xs\" ng-if=\"vm.debug.on\" ng-click=\"toogleDebug()\"><i class=\"eav-icon-info-circled\"></i> {{debug ? \'Hide\' : \'Show\'}} Debug Info</button>\r\n\r\n                <show-debug-availability class=\"pull-right\"></show-debug-availability>\r\n            </div>\r\n        </div>\r\n        <toaster-container></toaster-container>\r\n        <pre ng-if=\"debug\">{{queryDef.data | json}}</pre>\r\n    </div>\r\n</div>\r\n");
$templateCache.put("pipelines/pipelines.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n    <div class=\"modal-header\">\r\n        <h3 class=\"modal-title\" translate=\"Pipeline.Manage.Title\"></h3>\r\n    </div>\r\n    <div class=\"modal-body ng-cloak\">\r\n        <div translate=\"Pipeline.Manage.Intro\"></div>\r\n        <div>\r\n            <button icon=\"plus\" type=\"button\" class=\"btn btn-primary btn-square\" ng-click=\"vm.add()\"></button>\r\n            <span class=\"btn-group\" ng-if=\"vm.debug.on\">\r\n                <button type=\"button\" class=\"btn btn-warning btn-square\" ng-click=\"vm.refresh()\"><i icon=\"repeat\"></i></button>\r\n                <button type=\"button\" class=\"btn btn-warning btn-square\" ng-click=\"vm.liveEval()\"><i icon=\"flash\"></i></button>\r\n            </span>\r\n            <table class=\"table table-hover table-manage-eav\">\r\n                <thead>\r\n                <tr>\r\n                    <th translate=\"Pipeline.Manage.Table.Id\" class=\"col-id\"></th>\r\n                    <th translate=\"Pipeline.Manage.Table.Name\"></th>\r\n                    <th translate=\"Pipeline.Manage.Table.Description\"></th>\r\n                    <th translate=\"Pipeline.Manage.Table.Actions\" class=\"mini-btn-4\"></th>\r\n                </tr>\r\n                </thead>\r\n                <tbody>\r\n                <tr ng-repeat=\"pipeline in vm.pipelines | orderBy:\'Name\'\" class=\"clickable-row\" ng-click=\"vm.design(pipeline)\">\r\n                    <td class=\"clickable\">{{pipeline.Id}}</td>\r\n                    <td class=\"clickable\">{{pipeline.Name}}</td>\r\n                    <td class=\"clickable\">{{pipeline.Description}}</td>\r\n                    <td class=\"text-nowrap mini-btn-4\" stop-event=\'click\'>\r\n                        <span class=\"btn-group\">\r\n                            <button title=\"{{ \'General.Buttons.Edit\' | translate }}\" class=\"btn btn-xs\" ng-click=\"vm.edit(pipeline)\"><i icon=\"cog\"></i></button>\r\n                            <button title=\"{{ \'General.Buttons.Copy\' | translate }}\" type=\"button\" class=\"btn btn-xs\" ng-click=\"vm.clone(pipeline)\"><i icon=\"duplicate\"></i></button>\r\n                            <button title=\"{{ \'General.Buttons.Permissions\' | translate }}\" type=\"button\" class=\"btn btn-xs\" ng-click=\"vm.permissions(pipeline)\"><i icon=\"user\"></i></button>\r\n                        </span>\r\n                        <button title=\"{{ \'General.Buttons.Delete\' | translate }}\" type=\"button\" class=\"btn btn-xs\" ng-click=\"vm.delete(pipeline)\"><i icon=\"remove\"></i></button>\r\n                    </td>\r\n                </tr>\r\n                <tr ng-if=\"!vm.pipelines.length\">\r\n                    <td colspan=\"100\" translate=\"General.Messages.NothingFound\"></td>\r\n                </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n\r\n        <show-debug-availability class=\"pull-right\"></show-debug-availability>\r\n    </div>\r\n\r\n</div>");
$templateCache.put("pipelines/query-stats.html","<div class=\"modal-header\">\r\n    <button icon=\"remove\" class=\"btn pull-right\" type=\"button\" ng-click=\"vm.close()\"></button>\r\n    <h3 class=\"modal-title\" translate=\"Pipeline.Stats.Title\"></h3>\r\n</div>\r\n\r\n<div class=\"modal-body\">\r\n    <div translate=\"Pipeline.Stats.Intro\"></div>\r\n\r\n    <div>\r\n        <h3 translate=\"Pipeline.Stats.ParamTitle\"></h3>\r\n        <div translate=\"Pipeline.Stats.ExecutedIn\" translate-values=\"{ ms: vm.timeUsed, ticks: vm.ticksUsed }\"></div>\r\n        <div>\r\n            <ul>\r\n                <li ng-repeat=\"param in vm.testParameters\">{{param}}</li>\r\n            </ul>\r\n        </div>\r\n    </div>\r\n\r\n    <div>\r\n        <h3 translate=\"Pipeline.Stats.QueryTitle\"></h3>\r\n        <pre>{{vm.result | json}}</pre>\r\n    </div>\r\n    <div>\r\n        <h3 translate=\"Pipeline.Stats.SourcesAndStreamsTitle\"></h3>\r\n        <h4 translate=\"Pipeline.Stats.Sources.Title\"></h4>\r\n        <table>\r\n            <tr>\r\n                <th translate=\"Pipeline.Stats.Sources.Guid\"></th>\r\n                <th translate=\"Pipeline.Stats.Sources.Type\"></th>\r\n                <th translate=\"Pipeline.Stats.Sources.Config\"></th>\r\n            </tr>\r\n            <tr ng-repeat=\"s in vm.sources\">\r\n                <td uib-tooltip=\"{{s}}\"><pre>{{s.Guid.substring(0, 13)}}...</pre></td>\r\n                <td>{{s.Type}}</td>\r\n                <td uib-tooltip=\"{{s.Configuration}}\">\r\n                    <ol>\r\n                        <li ng-repeat=\"(key, value) in s.Configuration\"><b>{{key}}</b>=<em>{{value}}</em></li>\r\n                    </ol>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n\r\n        <h4 translate=\"Pipeline.Stats.Streams.Title\"></h4>\r\n        <table>\r\n            <tr>\r\n                <th translate=\"Pipeline.Stats.Streams.Source\"></th>\r\n                <th translate=\"Pipeline.Stats.Streams.Target\"></th>\r\n                <th translate=\"Pipeline.Stats.Streams.Items\"></th>\r\n                <th translate=\"Pipeline.Stats.Streams.Error\"></th>\r\n            </tr>\r\n            <tr ng-repeat=\"sr in vm.streams\">\r\n                <td><pre>{{sr.Source.substring(0, 13) + \":\" + sr.SourceOut}}</pre></td>\r\n                <td><pre>{{sr.Target.substring(0, 13) + \":\" + sr.TargetIn}}</pre></td>\r\n                <td><span>{{sr.Count}}</span></td>\r\n                <td><span>{{sr.Error}}</span></td>\r\n            </tr>\r\n        </table>\r\n\r\n    </div>\r\n</div>");}]);