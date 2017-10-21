/* 
 * Custom Fields skelleton to allow later, lazy loaded fields
 */
(function() {
    "use strict";
    var module = angular.module("eavCustomFields", ["oc.lazyLoad"])

    .config(["$ocLazyLoadProvider", function ($ocLazyLoadProvider) {
        $ocLazyLoadProvider.config({
            debug: true
        });
    }]);

})();
/* Main object with dependencies, used in wrappers and other places */
(function () {
	"use strict";

    angular.module("eavEditEntity", [
        "formly",
        "ui.bootstrap",
        "uiSwitch",
        "toastr",
        "ngAnimate",
        "EavServices",
        "eavEditTemplates",
        "eavFieldTemplates",
        "eavCustomFields",

        // new...
        "oc.lazyLoad"
    ]);



})();
/* 
 * Field-Templates app initializer
 */

angular.module("eavFieldTemplates",
    [
        "formly",
        "formlyBootstrap",
        "ui.bootstrap",
        "eavLocalization",
        "eavEditTemplates",
        "ui.tree",

        // testing for the entity-picker dropdown
        "ui.select",
        "ngSanitize"
    ]
)
    // important: order of use is backwards, so the last is around the second-last, etc.
    .constant("defaultFieldWrappers", [
        "eavLabel",
        "float-label",
        "bootstrapHasError",
        "disablevisually",
        "eavLocalization",
        "responsive",
        "collapsible",
        "hiddenIfNeeded"
    ])

    .constant("fieldWrappersWithPreview", [
        "eavLabel",
        "float-label",
        "bootstrapHasError",
        "disablevisually",
        "eavLocalization",
        "preview-default",
        "responsive",
        "collapsible",
        "hiddenIfNeeded"
    ])

    .constant("defaultFieldWrappersNoFloat", [
        "eavLabel",
        //"float-label",
        "bootstrapHasError",
        "disablevisually",
        "eavLocalization",
        //"preview-default",
        "responsive",
        "collapsible",
        "hiddenIfNeeded"
    ])

    .constant("fieldWrappersNoLabel", [
        //"eavLabel",
        //"float-label",
        "bootstrapHasError",
        "disablevisually",
        "eavLocalization",
        //"preview-default",
        "responsive",
        "no-label-space",
        "collapsible",
        "hiddenIfNeeded"
    ])
;
/* 
 * Field: Boolean - Default
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "fieldWrappersNoLabel", function (formlyConfigProvider, fieldWrappersNoLabel) {
        formlyConfigProvider.setType({
            name: "boolean-default",
            templateUrl: "fields/boolean/boolean-default.html",
            wrapper: fieldWrappersNoLabel // ["bootstrapHasError", "disablevisually", "eavLocalization", "responsive", "collapsible"]
        });
    }]);
/* 
 * Field: Custom - Default (basically something you should never see)
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "custom-default",
            templateUrl: "fields/custom/custom-default.html",
            wrapper: defaultFieldWrappers
        });

    }]);
// this changes JSON-serialization for dates, 
// because we usually want the time to be the same across time zones and NOT keeping the same moment
Date.prototype.toJSON = function() {
    var x = new Date(this);
    x.setHours(x.getHours() - x.getTimezoneOffset() / 60);
    return x.toISOString();
};
/* 
 * Field: DateTime - Default
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "datetime-default",
            wrapper: defaultFieldWrappers,
            templateUrl: "fields/datetime/datetime-default.html",
            defaultOptions: {
                templateOptions: {
                    datepickerOptions: {}
                }
            },
            controller: ['$scope', '$locale', '$translate', function($scope, $locale, $translate) {
                $scope.format = $locale.DATETIME_FORMATS.mediumDate;
                $scope.datepickerPopup = {
                    clearText: $translate.instant("CalendarPopup.ClearButton"),
                    closeText: $translate.instant("CalendarPopup.CloseButton"),
                    currentText: $translate.instant("CalendarPopup.CurrentButton")
                };
            }],
            link: function (scope, el, attrs) {

                // Server delivers value as string, so convert it to UTC date
                scope.$watch('value', function (value) {
                    if (value && value.Value && !(value.Value instanceof Date)) {
                        scope.value.Value = convertDateToUTC(new Date(value.Value));
                    }
                });

                function convertDateToUTC(date) { return new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds()); }
                //if (scope.value && scope.value.Value && typeof (scope.value.Value) === 'string')
                //    scope.value.Value = convertDateToUTC(new Date(scope.value.Value));
            }
        });

    }]);
/* 
 * Field: Empty - Default: this is usually a title/group section
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", function(formlyConfigProvider) {
        formlyConfigProvider.setType({
            name: "empty-default",
            templateUrl: "fields/empty/empty-default.html",
            wrapper: ["fieldGroup"],
            controller: "FieldTemplate-TitleController"
        });
    }])
    .controller("FieldTemplate-TitleController", ["$scope", "debugState", function($scope, debugState) {
        if (!$scope.to.settings.merged)
            $scope.to.settings.merged = {};

        //$scope.to.settings.merged.DefaultCollapsed = true;// = "show";


        $scope.set = function(newState) {
            $scope.to.collapseGroup = newState;
        };

        $scope.toggle = function() {
            $scope.to.collapseGroup = !$scope.to.collapseGroup;
        };

        if ($scope.to.settings.merged.DefaultCollapsed === true) 
            $scope.set(true);

    }]);
/* 
 * Field: Entity - Default
 * Also contains much business logic and the necessary controller
 * 
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {

        var wrappers = defaultFieldWrappers.slice(0); // copy the array
        wrappers.splice(defaultFieldWrappers.indexOf("eavLocalization"), 1); // remove the localization...

        formlyConfigProvider.setType({
            name: "entity-default",
            templateUrl: "fields/entity/entity-default.html",
            wrapper: wrappers,
            controller: "FieldTemplate-EntityCtrl"
        });
    }])
    .controller("FieldTemplate-EntityCtrl", ["$scope", "$http", "$filter", "$translate", "$uibModal", "appId", "eavAdminDialogs", "eavDefaultValueService", "fieldMask", "$q", "$timeout", "entitiesSvc", "debugState", function ($scope, $http, $filter, $translate, $uibModal, appId, eavAdminDialogs, eavDefaultValueService, fieldMask, $q, $timeout, entitiesSvc, debugState) {
        var contentType, lastContentType;

        function activate() {
            // ensure settings are merged
            if (!$scope.to.settings.merged)
                $scope.to.settings.merged = {};

            // of no real data-model exists yet for this value (list of chosen entities), then create a blank
            if ($scope.model[$scope.options.key] === undefined || $scope.model[$scope.options.key].Values[0].Value === "") {
                var initVal = eavDefaultValueService($scope.options);   // note: works for simple entries as well as multiple, then it has to be an array though
                $scope.model[$scope.options.key] = { Values: [{ Value: initVal, Dimensions: {} }]};
            }

            // create short names for template
            var valList = $scope.model[$scope.options.key].Values[0].Value;
            //var showList = new [];
            //for (var i = 0; i < valList.length; i++)
            //    showList.push({ "title": valList[i] || "null", realId: valList[i], tempId: i });

            $scope.chosenEntities = valList;
            $scope.selectedEntity = null;

            // Initialize entities
            var sourceMask = $scope.to.settings.merged.EntityType || null;
            contentType = fieldMask(sourceMask, $scope, $scope.maybeReload, null);// this will contain the auto-resolve type (based on other contentType-field)
            // don't get it, it must be blank to start with, so it will be loaded at least 1x lastContentType = contentType.resolve();

            $scope.availableEntities = [];
        }

        $scope.debug = debugState;

        // add an just-picked entity to the selected list
        $scope.addEntity = function(item) {
            if (item === null) return false;
            $scope.chosenEntities.push(item);
            $scope.selectedEntity = null;
            return true;
        };

        // open the dialog for a new item
        $scope.openNewEntityDialog = function() {
            function reloadAfterAdd(result) {
                if (!result || result.data === null || result.data === undefined)
                    return;

                $scope.maybeReload(true).then(function () {
                    $scope.chosenEntities.push(Object.keys(result.data)[0]);
                    setDirty();
                });
            }
            eavAdminDialogs.openItemNew(contentType.resolve(), reloadAfterAdd);
        };

        // ajax call to get all entities
        // todo: move to a service some time + enhance to provide more fields if needed
        $scope.getAvailableEntities = function () {
            //if (!ctName)
            var ctName = contentType.resolve(); // always get the latest definition, possibly from another drop-down

            // check if we should get all or only the selected ones...
            // if we can't add, then we only need one...
            var itemFilter = null;
            try {
                itemFilter = $scope.to.settings.merged.EnableAddExisting
                    ? null
                    : $scope.model[$scope.options.key].Values[0].Value;
            }
            catch(err) {}

            return $http.post("eav/EntityPicker/getavailableentities", itemFilter, {
                params: {
                    contentTypeName: ctName,
                    appId: appId
                    // ToDo: dimensionId: $scope.configuration.DimensionId
                }
            }).then(function(data) {
                $scope.availableEntities = data.data;
            });
        };

        $scope.maybeReload = function (force) {
            var newMask = contentType.resolve();
            if (lastContentType !== newMask || force) {
                lastContentType = newMask;
                return $scope.getAvailableEntities();
            }
            return $q.when();
        };


        // get a nice label for any entity, including non-existing ones
        $scope.getEntityText = function (entityId) {
            if (entityId === null)
                return "empty slot"; // todo: i18n
            var entities = $filter("filter")($scope.availableEntities, { Value: entityId });
            return entities.length > 0 ? entities[0].Text : $translate.instant("FieldType.Entity.EntityNotFound"); 
        };

        // remove needs the index --> don't name "remove" - causes problems
        $scope.removeSlot = function(itemGuid, index) {
            $scope.chosenEntities.splice(index, 1);
            setDirty();
        };

        $scope.deleteItemInSlot = function (itemGuid, index) {
            if ($scope.to.settings.merged.EntityType === '') {
                alert('delete not possible - no type specified in entity field configuration');
                return;
            }

            var entities = $filter("filter")($scope.availableEntities, { Value: itemGuid });
            var id = entities[0].Id;

            entitiesSvc.tryDeleteAndAskForce(contentType.resolve(), id, entities[0].Text).then(function () {
                $scope.chosenEntities.splice(index, 1);
                $scope.maybeReload(true);
            });
        };

        // edit needs the Guid - the index isn't important
        $scope.edit = function (itemGuid, index) {
            if (itemGuid === null)
                return alert("no can do"); // todo: i18n
            var entities = $filter("filter")($scope.availableEntities, { Value: itemGuid });
            var id = entities[0].Id;

            return eavAdminDialogs.openItemEditWithEntityId(id, $scope.getAvailableEntities);
        };

        $scope.insertNull = function() {
            $scope.chosenEntities.push(null);
        };

        function setDirty() {
            $scope.form.$setDirty();
        }

        activate();
    }])

    .directive("entityValidation", [function () {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function(scope, element, attrs, ngModel) {
                if (!ngModel) return;

                ngModel.$validators.required = function (modelValue, viewValue) {
                    var value;

                    if (!scope.$parent.$parent.to.required) return true;

                    value = modelValue || viewValue;
                    if (!value || !Array.isArray(value)) return true;
                    return value.length > 0;
                };

                scope.$watch(function () {
                    return ngModel.$viewValue;
                }, function (newValue) {
                    ngModel.$validate();
                }, true);
            }
        };
    }]);

/* 
 * Field: Number - Default
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {
        formlyConfigProvider.setType({
            name: "number-default",
            template: "<input type=\"number\" class=\"form-control input-lg\" ng-model=\"value.Value\">{{vm.isGoogleMap}}",
            wrapper: defaultFieldWrappers,
            defaultOptions: {
                ngModelAttrs: {
                    '{{to.settings.merged.Min}}': { value: "min" },
                    '{{to.settings.merged.Max}}': { value: "max" },
                    '{{to.settings.merged.Decimals ? "^[0-9]+(\.[0-9]{1," + to.settings.merged.Decimals + "})?$" : null}}': { value: "pattern" }
                }
            },
            controller: "FieldTemplate-NumberCtrl as vm"
        });
    }]).controller("FieldTemplate-NumberCtrl", function () {
        var vm = this;
    });
/* 
 * Field: String - Dropdown
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function(formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-contenttype",
            templateUrl: "fields/string/string-contenttype.html",
            wrapper: defaultFieldWrappers,
            controller: "FieldTemplate-String-ContentType"
        });

    }])
    .controller("FieldTemplate-String-ContentType", ["$scope", "contentTypeSvc", "appId", function($scope, contentTypeSvc, appId) { //, $http, $filter, $translate, $uibModal, eavAdminDialogs, eavDefaultValueService) {
        // ensure settings are merged
        if (!$scope.to.settings.merged)
            $scope.to.settings.merged = {};

        // create initial list for binding
        $scope.contentTypes = [];

        var svc = contentTypeSvc(appId);
        svc.retrieveContentTypes().then(function(result) {
            $scope.contentTypes = result.data;
        });

    }]);
/* 
 * Field: String - Default
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-default",
            template: "<div><input class=\"form-control material\" ng-if=\"!(options.templateOptions.settings.merged.RowCount > 1)\" ng-pattern=\"vm.regexPattern\" ng-model=\"value.Value\">" +
                "<textarea ng-if=\"options.templateOptions.settings.merged.RowCount > 1\" rows=\"{{options.templateOptions.settings.merged.RowCount}}\" class=\"form-control material\" ng-model=\"value.Value\"></textarea></div>",
            wrapper: defaultFieldWrappers, 
            controller: "FieldTemplate-StringCtrl as vm"
        });

    }]).controller("FieldTemplate-StringCtrl", ["$scope", function ($scope) {
        var vm = this;
        var validationRegexString = ".*";
        var stringSettings = $scope.options.templateOptions.settings.merged;
        if (stringSettings && stringSettings.ValidationRegExJavaScript)
            validationRegexString = stringSettings.ValidationRegExJavaScript;
        vm.regexPattern = new RegExp(validationRegexString, 'i');

        console.log($scope.options.templateOptions);
    }]);
/* 
 * Field: String - Dropdown
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-dropdown",
            template: "<select class=\"form-control input-material material\" ng-model=\"value.Value\"></select>",
            wrapper: defaultFieldWrappers,
            defaultOptions: function defaultOptions(options) {

                // DropDown field: Convert string configuration for dropdown values to object, which will be bound to the select
                if (options.templateOptions.settings && options.templateOptions.settings.merged && options.templateOptions.settings.merged.DropdownValues) {
                    var o = options.templateOptions.settings.merged.DropdownValues;
                    o = o.replace(/\r/g, "").split("\n");
                    o = o.map(function (e, i) {
                        var s = e.split(":");
                        return {
                            name: s[0],
                            value: (s[1] || s[1] === '') ? s[1] : s[0]
                        };
                    });
                    options.templateOptions.options = o;
                }

                function _defineProperty(obj, key, value) { return Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); }

                var ngOptions = options.templateOptions.ngOptions || "option[to.valueProp || 'value'] as option[to.labelProp || 'name'] group by option[to.groupProp || 'group'] for option in to.options";
                return {
                    ngModelAttrs: _defineProperty({}, ngOptions, {
                        value: "ng-options"
                    })
                };

            }
        });
    }]);
/* 
 * Field: String - url-path
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function(formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-url-path",
            template: "<div><input class=\"form-control material\" only-simple-url-chars ng-pattern=\"vm.regexPattern\" ng-model=\"value.Value\" ng-blur=\"vm.finalClean()\"></div>",
            wrapper: defaultFieldWrappers,
            controller: "FieldTemplate-String-Url-Path-Ctrl as vm"
        });

    }])
    .controller("FieldTemplate-String-Url-Path-Ctrl", ["$scope", "debugState", "stripNonUrlCharacters", "fieldMask", function($scope, debugState, stripNonUrlCharacters, fieldMask) {
        var vm = this;

        // get configured
        var controlSettings = $scope.to.settings["string-url-path"];
        var sourceMask = (controlSettings) ? controlSettings.AutoGenerateMask || null : null;

        // todo: change to include the change-detection
        var mask = fieldMask(sourceMask, $scope, null, function preCleane(key, value) {
            return value.replace("/", "-").replace("\\", "-"); // this will remove slashes which could look like path-parts
        });

        // test values
        //sourceMask = "[Name]";
        //autoGenerateLink = true;
        var enableSlashes = true;
        $scope.enablePath = enableSlashes;

        //#region Field-Mask handling 
        vm.lastAutoCopy = "";
        vm.sourcesChangedTryToUpdate = function sourcesChangedTryToUpdate() {
            // don't do anything if the current field is not empty and doesn't have the last copy of the stripped value
            if ($scope.value && $scope.value.Value && $scope.value.Value !== vm.lastAutoCopy)
                return;

            var orig = mask.resolve(); // vm.getFieldContentBasedOnMask(sourceMask);
            //orig = orig.replace("/", "-").replace("\\", "-"); // when auto-importing, remove slashes as they shouldn't feel like paths afterwards
            var cleaned = stripNonUrlCharacters(orig, enableSlashes, true);
            if (cleaned && $scope.value) {
                vm.lastAutoCopy = cleaned;
                $scope.value.Value = cleaned;
            }
        };

        //#region enforce custom regex - copied from string-default
        var validationRegexString = ".*";
        var stringSettings = $scope.options.templateOptions.settings.merged;
        if (stringSettings && stringSettings.ValidationRegExJavaScript)
            validationRegexString = stringSettings.ValidationRegExJavaScript;
        vm.regexPattern = new RegExp(validationRegexString, "i");

        //#endregion

        //#region do final cleaning on blur / leave-field; mainly remove trailing "/"
        vm.finalClean = function() {
            var orig = $scope.value.Value;
            var cleaned = stripNonUrlCharacters(orig, enableSlashes, true);
            if (orig !== cleaned)
                $scope.value.Value = cleaned;
        };
        //#endregion


        vm.activate = function () {
            // TODO: use new functionality on the fieldMask instead!
            // add a watch for each field in the field-mask
            angular.forEach(mask.fieldList() /* vm.getFieldsOfMask(sourceMask)*/, function(e, i) {
                $scope.$watch("model." + e + "._currentValue.Value", function() {
                    if (debugState.on) console.log("url-path: " + e + " changed...");
                    vm.sourcesChangedTryToUpdate(sourceMask);
                });
            });

            $scope.debug = debugState;
            if (debugState.on) console.log($scope.options.templateOptions);
        };
        vm.activate();

    }])


// this is a helper which cleans up the url and is used in various places
    .factory("stripNonUrlCharacters", ["latinizeText", function(latinizeText) {
        return function(inputValue, allowPath, trimEnd) {
            if (!inputValue) return "";
            var rexSeparators = allowPath ? /[^a-z0-9-_/]+/gi : /[^a-z0-9-_]+/gi;

            // allow only lower-case, numbers and _/- characters
            var latinized = latinizeText(inputValue.toLowerCase());
            var cleanInputValue = latinized
                .replace("'s ", "s ") // neutralize it's, daniel's etc. but only if followed by a space, to ensure we don't kill quotes
                .replace("\\", "/") // neutralize slash representation
                .replace(rexSeparators, "-") // replace everything we don't want with a -
                .replace(/-+/gi, "-") // reduce multiple "-"
                .replace(/\/+/gi, "/") // reduce multiple slashes
                .replace(/-*\/-*/gi, "/") // reduce "-/" or "/-" combinations to a simple "/"
                .replace(trimEnd ? /^-|-+$/gi : /^-/gi, ""); // trim front and maybe end "-"
            return cleanInputValue;
        };
    }])


    // this monitors an input-field and ensures that only allowed characters are typed
    .directive("onlySimpleUrlChars", ["stripNonUrlCharacters", function(stripNonUrlCharacters) {
        return {
            require: "ngModel",
            restrict: "A",
            link: function(scope, element, attrs, modelCtrl) {
                modelCtrl.$parsers.push(function(inputValue) {
                    if (inputValue === null)
                        return "";
                    var cleanInputValue = stripNonUrlCharacters(inputValue, scope.enablePath, false);

                    if (cleanInputValue !== inputValue) {
                        modelCtrl.$setViewValue(cleanInputValue);
                        modelCtrl.$render();
                    }
                    return cleanInputValue;
                });
            }
        };
    }]);

/* global angular */
(function () {
    "use strict";

    var app = angular.module("eavEditEntity");

    // The controller for the main form directive
    app.controller("EditEntities", ["appId", "$http", "$scope", "entitiesSvc", "contentTypeSvc", "$sce", "toastr", "saveToastr", "$translate", "debugState", "ctrlS", function editEntityCtrl(appId, $http, $scope, entitiesSvc, contentTypeSvc, $sce, toastr, saveToastr, $translate, debugState, ctrlS) {
        var detailedLogging = false;
        var clog = detailedLogging
            ? function () { for (var i = 0; i < arguments.length; i++) console.log(arguments[i]); }
            : function () { };

        var vm = this;
        vm.debug = debugState;
        vm.isWorking = 0;           // isWorking is > 0 when any $http request runs
        vm.registeredControls = []; // array of input-type controls used in these forms
        vm.items = null;            // array of items to edit
        vm.itemsHelp = [];
        vm.willPublish = false;     // default is won't publish, but will usually be overridden
        vm.publishMode = "hide";    // has 3 modes: show, hide, branch (where branch is a hidden, linked clone)
        vm.enableDraft = false;
        vm.typeI18n = [];

        var ctSvc = contentTypeSvc(appId);

        //#region activate / deactivate + bindings
        // the activate-command, to intialize everything. Must be called later, when all methods have been attached
        function activate() {
            // bind ctrl+S
            vm.saveShortcut = ctrlS(vm.save);

            // load all data
            vm.loadAll();
            vm.versioningOptions = getVersioningOptions();
        }

        function getVersioningOptions() {
            if (!$scope.partOfPage)
                return { show: true, hide: true, branch: true };
            var req = $2sxc.urlParams.get("publishing") || "";
            switch (req) {
                case "":
                case "DraftOptional": return { show: true, hide: true, branch: true };
                case "DraftRequired": return { branch: true, hide: true };
                default: throw "invalid versioning requiremenets: " + req.toString();
            }
        }

        // clean-up call when the dialog is closed
        function deactivate() {
            vm.saveShortcut.unbind();
        }

        // bind the clean-up call to when the dialog is removed
        $scope.$on("$destroy", function () {
            deactivate();
        });
        //#endregion

        // add an additional input-type control for lazy-loading etc.
        vm.registerEditControl = function (control) {
            vm.registeredControls.push(control);
        };
        //#region load / save

        // load all data
        vm.loadAll = function () {
            entitiesSvc.getManyForEditing(appId, $scope.itemList)
                .then(function (result) {
                    vm.items = result.data;
                    angular.forEach(vm.items, function (v, i) {

                        // If the entity is null, it does not exist yet. Create a new one
                        if (!vm.items[i].Entity && !!vm.items[i].Header.ContentTypeName)
                            vm.items[i].Entity = entitiesSvc.newEntity(vm.items[i].Header);

                        vm.items[i].Entity = enhanceEntity(vm.items[i].Entity);

                        //// load more content-type metadata to show
                        //vm.items[i].ContentType = contentTypeSvc.getDetails(vm.items[i].Header.ContentTypeName);
                        // set slot value - must be inverte for boolean-switch
                        var grp = vm.items[i].Header.Group;
                        vm.items[i].slotIsUsed = (grp === null || grp === undefined || grp.SlotIsEmpty !== true);
                    });
                    vm.willPublish = vm.items[0].Entity.IsPublished;
                    vm.enableDraft = vm.items[0].Header.EntityId !== 0; // it already exists, so enable draft
                    vm.publishMode = vm.items[0].Entity.IsBranch
                        ? "branch" // it's a branch, so it must have been saved as a draft-branch
                        : vm.items[0].Entity.IsPublished ? "show" : "hide";

                    // if publis mode is prohibited, revert to default
                    if (!vm.versioningOptions[vm.publishMode]) vm.publishMode = Object.keys(vm.versioningOptions)[0];
                    return result;
                }).then(function (result) {
                    angular.forEach(vm.items, function (v, i) {
                        // load more content-type metadata to show
                        ctSvc.getDetails(vm.items[i].Header.ContentTypeName).then(function (ct) {
                            if (ct.data) {
                                // first, check for i18n

                                if (ct.data.I18nKey) {
                                    console.log("has i18n");
                                    vm.typeI18n[i] = "ContentTypes." + ct.data.I18nKey;
                                }
                                // check for included instructions
                                if (ct.data.Metadata && ct.data.Metadata.EditInstructions)
                                    vm.itemsHelp[i] = $sce.trustAsHtml(ct.data.Metadata.EditInstructions);
                                translateIfNecessary(ct.data, i);
                            }
                        });
                    });
                });
        };

        /**
         * translate all content-type labels - if there is a key to do so
         */
        function translateIfNecessary(data, index) {
            try {
                var i18nKey = data.I18nKey;
                if (!i18nKey) return;
                var rootKey = "ContentTypes." + i18nKey + ".Metadata";
                // this must happen in a refresh-promise, as the resources are lazy-loaded
                $translate.refresh().then(function () {
                    var keylbl = rootKey + ".Label",
                        //keyDsc = rootKey + ".Description",
                        keyEdt = rootKey + ".EditInstructions",
                        txtLbl = $translate.instant(keylbl),
                        //txtDsc = $translate.instant(keyDsc),
                        txtEdt = $translate.instant(keyEdt);
                    if (txtLbl !== keylbl) vm.items[index].Header.Title = txtLbl;
                    //if (txtLbl !== keylbl) vm.items[index].Header.??? = txtDsc;
                    if (txtEdt !== keyEdt) vm.itemsHelp[index] = $sce.trustAsHtml(txtEdt);
                });
            }
            catch (e) { /* ignore */ }
        }


        vm.showFormErrors = function () {
            var errors = vm.formErrors();
            var msgs = [], msgTemplate = $translate.instant("Message.FieldErrorList");
            for (var set = 0; set < errors.length; set++) {
                if (errors[set].required) {
                    var req = errors[set].required.map(function (itm) { return { field: itm.$name, error: "required" }; });
                    msgs = msgs.concat(req);
                }
            }
            var nice = msgs.map(function (err) {
                var specs = err.field.split("_");

                return msgTemplate.replace("{form}", specs[1])
                    .replace("{field}", specs[3])
                    .replace("{error}", err.error);
            });
            var msg = nice.join("<br/>");
            return toastr.error($translate.instant("Message.CantSaveInvalid").replace("{0}", msg),
                $translate.instant("Message.Error"), { allowHtml: true });
        };

        // the save-call
        vm.save = function (close) {
            // check if saving is allowed
            if (!vm.isValid())
                return vm.showFormErrors();

            if (vm.isWorking > 0)
                return toastr.error($translate.instant("Message.CantSaveProcessing")); // todo: i18n

            // save
            vm.isWorking++;
            saveToastr(entitiesSvc.saveMany(appId, vm.items, $scope.partOfPage))
                .then(function (result) {
                    $scope.state.setPristine();
                    if (close) {
                        vm.allowCloseWithoutAsking = true;
                        vm.afterSaveEvent(result);
                    }
                    vm.enableDraft = true;  // after saving, we can re-save as draft
                    vm.isWorking--;
                }, function errorWhileSaving() {
                    vm.isWorking--;
                });
            return null;
        };

        // things to do after saving
        vm.afterSaveEvent = $scope.afterSaveEvent;

        //#endregion

        //#region state check/set for valid/dirty/pristine
        // check if form is valid
        vm.isValid = function () {
            var valid = true;
            angular.forEach(vm.registeredControls, function (e) {
                if (!e.isValid())
                    valid = false;
            });
            return valid;
        };

        vm.formErrors = function () {
            var list = [];
            angular.forEach(vm.registeredControls, function (e) {
                if (!e.isValid())
                    list.push(e.error());
            });
            return list;
        };

        // check if dirty
        $scope.state.isDirty = function () {
            var dirty = false;
            angular.forEach(vm.registeredControls, function (e) {
                if (e.isDirty())
                    dirty = true;
            });
            return dirty;
        };

        // set to not-dirty (pristine)
        $scope.state.setPristine = function () {
            angular.forEach(vm.registeredControls, function (e) {
                e.setPristine();
            });
        };
        //#endregion

        // monitor for changes in publish-state and set it for all items being edited
        $scope.$watch("vm.willPublish", function () {   // ToDO Todo
            angular.forEach(vm.items, function (v, i) {
                vm.items[i].Entity.IsPublished = vm.willPublish;
            });
        });

        $scope.$watch("vm.publishMode", function () {   // ToDO Todo
            var publish = vm.publishMode === "show"; // all other cases are hide
            var branch = vm.publishMode === "branch"; // all other cases are no-branch
            angular.forEach(vm.items, function (v, i) {
                vm.items[i].Entity.IsPublished = publish;
                vm.items[i].Entity.IsBranch = branch;
            });
        });

        // handle maybe-leave
        vm.maybeLeave = {
            save: function () { vm.save(true); },
            quit: $scope.close,
            handleClick: function (event) {
                clog("handleClick", event);
                var target = event.target || event.srcElement;
                if (target.nodeName === "I") target = target.parentNode;
                if (target.id === "save" || target.id === "quit") {
                    clog("for " + target.id);
                    vm.allowCloseWithoutAsking = true;
                    vm.maybeLeave[target.id]();
                }
            },
            ask: function (e) {
                if (!$scope.state.isDirty() || vm.allowCloseWithoutAsking)
                    return;
                var template = "<div>"  // note: this variable must be inside this method, to ensure that translate is pre-loaded before we call it
                    + $translate.instant("Errors.UnsavedChanges") + "<br>"
                    + "<button type='button' id='save' class='btn btn-primary' ><i class='eav-icon-ok'></i>" + $translate.instant("General.Buttons.Save") + "</button> &nbsp;"
                    + "<button type='button' id='quit' class='btn btn-default' ><i class= 'eav-icon-cancel'></i>" + $translate.instant("General.Buttons.NotSave") + "</button>"
                    + "</div>";
                if (vm.dialog && vm.dialog.isOpened)
                    toastr.clear(vm.dialog);
                vm.dialog = toastr.warning(template, {
                    allowHtml: true,
                    timeOut: 3000,
                    onShown: function (toast) {
                        toast.el[0].onclick = vm.maybeLeave.handleClick;
                    }
                });
                e.preventDefault();
            }
        };

        $scope.$on("modal.closing", vm.maybeLeave.ask);



        /// toggle / change if a section (slot) is in use or not (like an unused presentation)
        vm.toggleSlotIsEmpty = function (item) {
            if (!item.Header.Group)
                item.Header.Group = {};
            item.Header.Group.SlotIsEmpty = !item.Header.Group.SlotIsEmpty;
            item.slotIsUsed = !item.Header.Group.SlotIsEmpty;
        };

        activate();

    }]);
})();

(function () {
    "use strict";

    var app = angular.module("eavEditEntity");
    
    app.directive("eavEditEntities", function () {
        return {
            templateUrl: "form/edit-many-entities.html",
            restrict: "E",
            scope: {
                itemList: "=",
                afterSaveEvent: "=",
                state: "=",
                close: "=",
                partOfPage: "=",
                publishing: "="
            },
            controller: "EditEntities",
            controllerAs: "vm"
        };
    });
})();

(function () {
    /* jshint laxbreak:true */
	"use strict";

	var app = angular.module("eavEditEntity"); 
	
	// The controller for the main form directive
    app.controller("EditEntityFormCtrl", ["appId", "$http", "$scope", "$rootScope", "$translate", "formlyConfig", "contentTypeFieldSvc", "$sce", "debugState", "customInputTypes", "eavConfig", "$injector", function editEntityCtrl(appId, $http, $scope,
        $rootScope, // needed to handle translation-loading events
        $translate,  // needed to translate i18n on labels etc.
        formlyConfig, contentTypeFieldSvc, $sce, debugState, customInputTypes, eavConfig, $injector) {

		var vm = this;
		vm.editInDefaultLanguageFirst = function () {
			return false; // ToDo: Use correct language information, e.g. eavLanguageService.currentLanguage != eavLanguageService.defaultLanguage && !$scope.entityId;
		};

		// The control object is available outside the directive
		// Place functions here that should be available from the parent of the directive
		vm.control = {
		    isValid: function () { return vm.formFields.length === 0 || vm.form && vm.form.$valid; },
		    isDirty: function () { return (vm.form && vm.form.$dirty); },
		    setPristine: function () { if (vm.form) vm.form.$setPristine(); },
            error: function () { return vm.form.$error; }
		};

		// Register this control in the parent control
		if($scope.registerEditControl)
			$scope.registerEditControl(vm.control);

		vm.model = null;
		vm.entity = $scope.entity;

		vm.formFields = [];


		var loadContentType = function () {

		    contentTypeFieldSvc(appId, { StaticName: vm.entity.Type.StaticName }).getFields()
		        .then(function(result) {
		            vm.debug = result;

		            // Transform EAV content type configuration to formFields (formly configuration)

                    // first: add all custom types to re-load these scripts and styles
		            angular.forEach(result.data, function (e, i) {
		                // check in config input-type replacement map if the specified type should be replaced by another
		                //if (e.InputType && eavConfig.formly.inputTypeReplacementMap[e.InputType]) 
		                //    e.InputType = eavConfig.formly.inputTypeReplacementMap[e.InputType];

		                // review type and get additional configs!
		                e.InputType = vm.getType(e);
		                eavConfig.formly.inputTypeReconfig(e);  // provide custom overrides etc. if necessary

		                if (e.InputTypeConfig)
		                    customInputTypes.addInputType(e);
		            });

		            // load all assets before continuing with formly-binding
		            var promiseToLoad = customInputTypes.loadWithPromise();
		            promiseToLoad.then(function(dependencyResult) {
		                vm.registerAllFieldsFromReturnedDefinition(result);
		            });
		        });
		};

	    vm.initCustomJavaScript = function(field) {
	        var jsobject,
                cjs = field.Metadata.merged.CustomJavaScript;
	        if (!cjs) return;
	        if (cjs.indexOf("/* compatibility: 1.0 */") < 0) {
	            console.log("found custom js for field '" + field.StaticName + "', but didn't find correct version support; ignore");
	            return;
	        }

	        try {
	            console.log("fyi: will run custom js for " + field.StaticName);
	            var fn = new Function(cjs); // jshint ignore:line
	            jsobject = fn();
	        }
            catch (ex) {
                console.log("wasn't able to process the custom javascript for field '" + field.StaticName + "'. tried: " + cjs);
            }
	        if (jsobject === undefined || jsobject === null)
	            return;

	        var context = {
	            field: field,   // current field object
	            formVm: vm,     // the entire form view model
	            formlyConfig: formlyConfig, // form configuration
	            appId: appId,   // appId - in case needed for service calls or similar
	            module: app, // pass in this current module in case something complex is wanted
	            $injector: $injector,   // to get $http or similar
	        };

	        // now cjs should be the initiliazed object...
	        if (jsobject && jsobject.init) {
	            console.log("fyi: will init on custom js for " + field.StaticName);
	            try {
	                jsobject.init(context);
	            } catch (ex) {
	                console.log("init custom js failed with error - will ignore this");
	                console.log(ex);
	            }
	        }
	    };

	    vm.registerAllFieldsFromReturnedDefinition = function raffrd(result) {
	        var lastGroupHeadingId = null,
	            rawFields = result.data;
	        angular.forEach(rawFields, function (e, i) {

                // make sure there is a definition object for the "All" settings
	            if (e.Metadata.All === undefined)
	                e.Metadata.All = {};

	            if (e.Metadata.All.VisibleInEditUI !== false) // for null or undefined
	                e.Metadata.All.VisibleInEditUI = true;

	            vm.initCustomJavaScript(e);

	            var fieldType = e.InputType;

	            // always remember the last heading so all the following fields know to look there for collapse-setting
	            var isFieldHeading = (fieldType === "empty-default");
	            if (isFieldHeading) lastGroupHeadingId = i;

	            var nextField = {
	                key: e.StaticName,
                    type: fieldType,
	                templateOptions: {
	                    required: !!e.Metadata.All.Required,
	                    label: e.Metadata.All.Name === undefined ? e.StaticName : e.Metadata.All.Name,
	                    // i18nKey: e.I18nKey ? e.I18nKey + ".Attributes." + e.StaticName : null, // optional translation key, if resources are provided in i18n
	                    description: $sce.trustAsHtml(e.Metadata.All.Notes),
	                    settings: e.Metadata,
	                    header: $scope.header,
	                    canCollapse: (lastGroupHeadingId !== null) && !isFieldHeading,
	                    fieldGroup: vm.formFields[lastGroupHeadingId || 0],
	                    disabled: e.Metadata.All.Disabled,
	                    langReadOnly: false, // Will be set by the language directive to override the disabled state

                        // test to discover focused for floating labels
	                    onBlur: 'to.focused=false',
	                    onFocus: 'to.focused=true',
                        focused: false,
                        debug: debugState.on
	                },
                    className: "type-" + e.Type.toLowerCase() + " input-" + fieldType + " field-" + e.StaticName.toLowerCase(),
	                
	                expressionProperties: {
	                    // Needed for dynamic update of the disabled property
	                    'templateOptions.disabled': 'options.templateOptions.disabled' // doesn't set anything, just here to ensure formly causes update-binding
	                },
	                watcher: [
                        {
                            // changes when a entity becomes enabled / disabled
                            expression: function (field, scope, stop) {
                                return e.Metadata.All.Disabled ||
                                    (field.templateOptions.header.Group && field.templateOptions.header.Group.SlotIsEmpty) ||
                                    field.templateOptions.langReadOnly;
                            },
                            listener: function (field, newValue, oldValue, scope, stopWatching) {
                                field.templateOptions.disabled = newValue;
                            }
                        },
                        {
                            // handle collapse / open
                            expression: function (field, scope, stop) {
                                // only change values if it can collapse...
                                return (field.templateOptions.canCollapse) ? field.templateOptions.fieldGroup.templateOptions.collapseGroup : null;
                            },
                            listener: function (field, newValue, oldValue, scope, stopWatching) {
                                if (field.templateOptions.canCollapse)
                                    field.templateOptions.collapse = newValue;
                            }
                        }
	                ]
                };

	            vm.formFields.push(nextField);
            });
	        translateFieldsIfNecessary(rawFields, vm.formFields);
	    };

        /**
         * translate all content-type labels - if there is a key to do so
         */
        function translateFieldsIfNecessary(rawFields, fieldList) {
            try {
                var i18nKey = rawFields[0].I18nKey;
                if (!i18nKey) return;
                var rootKey = "ContentTypes." + i18nKey + ".Attributes.";
                // this must happen in a refresh-promise, as the resources are lazy-loaded
                $translate.refresh().then(function() {
                    angular.forEach(fieldList,
                        function(field, i) {
                            var fieldKey = rootKey + field.key,
                                keylbl = fieldKey + ".Label",
                                keyDsc = fieldKey + ".Description",
                                txtLbl = $translate.instant(keylbl),
                                txtDsc = $translate.instant(keyDsc);
                            if (txtLbl !== keylbl) field.templateOptions.label = txtLbl;
                            if (txtDsc !== keyDsc) field.templateOptions.description = $sce.trustAsHtml(txtDsc);
                        });
                });
            }
            catch (e) { /* ignore */ }
        }

	    // Load existing entity if defined
		if (vm.entity !== null) loadContentType();

	    // Returns the field type for an attribute configuration
		vm.getType = function (attributeConfiguration) {
		    var e = attributeConfiguration;
		    var type = e.Type.toLowerCase();
		    var inputType = "";

		    // new: the All can - and should - have an input-type which doesn't change
		    // First look in Metadata.All if an InputType is defined (All should override the setting, which is not the case when using only merged)
		    if (e.InputType !== "unknown") // the input type of @All is here from the web service // Metadata.All && e.Metadata.All.InputType)
		        inputType = e.InputType;
		        // If not, look in merged
		    else if (e.Metadata.merged && e.Metadata.merged.InputType)
		        inputType = e.Metadata.merged.InputType;

		    if (inputType && inputType.indexOf("-") === -1) // has input-type, but missing main type, this happens with old types like string wysiyg
		        inputType = type + "-" + inputType;

		    var willBeRewrittenByConfig = (inputType && eavConfig.formly.inputTypeReplacementMap[inputType]);
		    if (!willBeRewrittenByConfig) {
		        // this type may have assets, so the definition may be late-loaded
		        var typeAlreadyRegistered = formlyConfig.getType(inputType);    // check if this input-type actually exists - so "string-i-made-this-up" will return undefined
		        var typeWillRegisterLaterWithAssets = (e.InputTypeConfig ? !!e.InputTypeConfig.Assets : false); // if it will load assets later, then it may still be defined then

		        // Use subtype 'default' if none is specified - or type does not exist
		        if (!inputType || (!typeAlreadyRegistered && !typeWillRegisterLaterWithAssets))
		            inputType = type + "-default";

		        // but re-check if it's in the config! since the name might have changed
		        willBeRewrittenByConfig = (inputType && eavConfig.formly.inputTypeReplacementMap[inputType]);
		    }

		    // check in config input-type replacement map if the specified type should be replaced by another
		    // like "string-wysiwyg" replaced by "string-wysiwyg-tinymce"
		    if (willBeRewrittenByConfig)
		        inputType = eavConfig.formly.inputTypeReplacementMap[inputType];

		    return (inputType);
		};
	}]);
})();

(function () {
	"use strict";

	angular.module("eavEditEntity")
        .directive("eavEditEntityForm", function () {
		return {
		    templateUrl: "form/edit-single-entity.html",
			restrict: "E",
			scope: {
			    entity: "=",
                header: "=",
                registerEditControl: "="
			},
			controller: "EditEntityFormCtrl",
			controllerAs: "vm"
		};
	});
})();
/* global angular */
(function () {
	"use strict";

	var app = angular.module("eavEditEntity");

	// The controller for the main form directive
	app.controller("EditEntityWrapperCtrl", ["$q", "$http", "$scope", "items", "$uibModalInstance", "$window", "$translate", "toastr", "partOfPage", "publishing", function editEntityCtrl($q, $http, $scope, items, $uibModalInstance, $window, $translate, toastr, partOfPage, publishing) {
		var vm = this;
		
        vm.partOfPage = partOfPage;
	    vm.publishing = publishing;
		vm.itemList = items;

	    // this is the callback after saving - needed to close everything
	    vm.afterSave = function(result) {
	        if (result.status === 200)
	            vm.close(result);
	        else {
	            alert($translate.instant("Errors.UnclearError"));
	        }
	    };
		
	    vm.state = {
	        isDirty: function() {
	            throw $translate.instant("Errors.InnerControlMustOverride");
	        }
	    };

	    vm.close = function (result) {
		    $uibModalInstance.close(result);
		};

	    $window.addEventListener('beforeunload', function (e) {
	        var unsavedChangesText = $translate.instant("Errors.UnsavedChanges");
	        if (vm.state.isDirty()) {
	            (e || window.event).returnValue = unsavedChangesText; //Gecko + IE
	            return unsavedChangesText; //Gecko + Webkit, Safari, Chrome etc.
	        }
	        return null;
	    });
	}]);
})();

(function () {
	"use strict";


	/* This app handles all aspectes of the multilanguage features of the field templates */

	var eavLocalization = angular.module("eavLocalization", ["formly", "EavConfiguration"], ["formlyConfigProvider", function (formlyConfigProvider) {

		// Field templates that use this wrapper must bind to value.Value instead of model[...]
		formlyConfigProvider.setWrapper([
			{
				name: "eavLocalization",
				templateUrl: "localization/formly-localization-wrapper.html"
			}
		]);

	}]);

	eavLocalization.directive("eavLanguageSwitcher", function () {
		return {
			restrict: "E",
			templateUrl: "localization/language-switcher.html",
			controller: ["$scope", "languages", function($scope, languages) {
				$scope.languages = languages;
			}],
			scope: {
			    isDisabled: "=isDisabled"
			}
		};
	});

	eavLocalization.directive("eavLocalizationScopeControl", function () {
		return {
			restrict: "E",
			transclude: true,
			template: "",
			link: function (scope, element, attrs) {
			},
			controller: ["$scope", "$filter", "$translate", "eavDefaultValueService", "languages", function ($scope, $filter, $translate, eavDefaultValueService, languages) { // Can't use controllerAs because of transcluded scope

				var scope = $scope;
				var langConf = languages;

				var initCurrentValue = function() {

					// Set base value object if not defined
					if (!scope.model[scope.options.key])
						scope.model.addAttribute(scope.options.key);

					var fieldModel = scope.model[scope.options.key];

					// If current language = default language and there are no values, create an empty value object
					if (fieldModel.Values.length === 0) {
					    if (langConf.currentLanguage == langConf.defaultLanguage) {
					        var defaultValue = eavDefaultValueService(scope.options);
                            // Add default language if we are in a ml environment, else don't add any
					        var languageToAdd = langConf.languages.length > 0 ? langConf.currentLanguage : null;
					        fieldModel.addVs(defaultValue, languageToAdd);
					    }
					    else { // There are no values - value must be edited in default language first
					        return;
					    }
					}

				    // Assign default language if no dimension is set - new: if multiple languages are in use!!!
					if (Object.keys(fieldModel.Values[0].Dimensions).length === 0)
                        if(langConf.languages.length > 0)
				            fieldModel.Values[0].Dimensions[langConf.defaultLanguage] = false; // set to "not-read-only"

					var valueToEdit;

					// Decide which value to edit:
					// 1. If there is a value with current dimension on it, use it
					valueToEdit = $filter("filter")(fieldModel.Values, function(v, i) {
						return v.Dimensions[langConf.currentLanguage] !== undefined;
					})[0];

					// 2. Use default language value
					if (valueToEdit === undefined)
						valueToEdit = $filter("filter")(fieldModel.Values, function(v, i) {
							return v.Dimensions[langConf.defaultLanguage] !== undefined;
						})[0];

					// 3. Use the first value if there is only one
					if (valueToEdit === undefined) {
						if (fieldModel.Values.length > 1)
						    throw $translate.instant("Errors.DefLangNotFound") + " " + $scope.options.key;
						// Use the first value
						valueToEdit = fieldModel.Values[0];
					}

					fieldModel._currentValue = valueToEdit;

					// Set scope variable 'value' to simplify binding
					scope.value = fieldModel._currentValue;

				    // Decide whether the value is writable or not
					var writable = (langConf.currentLanguage == langConf.defaultLanguage) ||
                        (scope.value && scope.value.Dimensions[langConf.currentLanguage] === false);

					scope.to.langReadOnly = !writable;
				};

				initCurrentValue();

				// Handle language switch
				scope.langConf = langConf; // Watch does only work on scope variables
				scope.$watch("langConf.currentLanguage", function (newValue, oldValue) {
					if (oldValue === undefined || newValue == oldValue)
						return;
					initCurrentValue();
				});

				// ToDo: Could cause performance issues (deep watch array)...
				scope.$watch("model[options.key].Values", function(newValue, oldValue) {
					initCurrentValue();
				}, true);

				// The language menu must be able to trigger an update of the _currentValue property
				scope.model[scope.options.key]._initCurrentValue = initCurrentValue;
			}]
		};
	});

	eavLocalization.directive("eavLocalizationMenu", function () {
		return {
			restrict: "E",
			scope: {
				fieldModel: "=fieldModel",
				options: "=options",
				value: "=value",
				index: "=index",
                formModel: "=formModel"
			},
			templateUrl: "localization/localization-menu.html",
			link: function (scope, element, attrs) { },
			controllerAs: "vm",
			controller: ["$scope", "languages", "$translate", function ($scope, languages, $translate) {

			    var vm = this;
			    var lblDefault = $translate.instant("LangMenu.UseDefault");
			    var lblIn = $translate.instant("LangMenu.In");

				vm.fieldModel = $scope.fieldModel;
				vm.languages = languages;
				vm.hasLanguage = function(languageKey) {
				    return vm.fieldModel.getVsWithLanguage(languageKey) !== null;
				};

				vm.isDefaultLanguage = function () { return languages.currentLanguage != languages.defaultLanguage; };
				vm.enableTranslate = function () { return vm.fieldModel.getVsWithLanguage(languages.currentLanguage) === null; };

				vm.infoMessage = function () {
				    if (Object.keys($scope.value.Dimensions).length === 1 && $scope.value.Dimensions[languages.defaultLanguage] === false)
				        return lblDefault;
				    if (Object.keys($scope.value.Dimensions).length === 1 && $scope.value.Dimensions[languages.currentLanguage] === false)
				        return "";
				    return $translate.instant("LangMenu.In", { languages: Object.keys($scope.value.Dimensions).join(", ") });
				    // "in " + Object.keys($scope.value.Dimensions).join(", ");
				};

				vm.tooltip = function () {
				    var editableIn = [];
				    var usedIn = [];
				    angular.forEach($scope.value.Dimensions, function (value, key) {
				        (value ? usedIn : editableIn).push(key);
				    });
				    var tooltip = $translate.instant("LangMenu.EditableIn", { languages: editableIn.join(", ") }); // "editable in " + editableIn.join(", ");
				    if (usedIn.length > 0)
				        tooltip += $translate.instant("LangMenu.AlsoUsedIn", { languages: usedIn.join(", ") });// ", also used in " + usedIn.join(", ");
				    return tooltip;
				};

				vm.actions = {
				    toggleTranslate: function toggleTranslate() {
				        if (vm.enableTranslate())
				            vm.actions.translate();
				        else
				            vm.actions.linkDefault();
				    },
				    translate: function translate() {
				        if (vm.enableTranslate()) {
				            vm.fieldModel.removeLanguage(languages.currentLanguage);
				            vm.fieldModel.addVs($scope.value.Value, languages.currentLanguage, false);
				        }
				    },
				    linkDefault: function linkDefault() {
				        vm.fieldModel.removeLanguage(languages.currentLanguage);
				    },
				    autoTranslate: function (languageKey) {
				        // Google translate is not implemented yet, because
                        // there is no longer a free api.
				        alert($translate.instant("LangMenu.NotImplemented"));
				    },
				    copyFrom: function (languageKey) {
				        if ($scope.options.templateOptions.disabled)
				            alert($translate.instant("LangMenu.CopyNotPossible"));
				        else {
				            var value = vm.fieldModel.getVsWithLanguage(languageKey).Value;
                            if (value === null || value === undefined)
				                console.log($scope.options.key + ": Can't copy value from " + languageKey + ' because that value does not exist.');
				            else
				                $scope.value.Value = value;
				        }
				    },
				    useFrom: function (languageKey) {
				        var vs = vm.fieldModel.getVsWithLanguage(languageKey);
				        if (vs === null || vs === undefined)
				            console.log($scope.options.key + ": Can't use value from " + languageKey + ' because that value does not exist.');
				        else {
				            vm.fieldModel.removeLanguage(languages.currentLanguage);
				            vs.setLanguage(languages.currentLanguage, true);
				        }
				    },
				    shareFrom: function (languageKey) {
				        var vs = vm.fieldModel.getVsWithLanguage(languageKey);
				        if (vs === null || vs === undefined)
				            console.log($scope.options.key + ": Can't share value from " + languageKey + ' because that value does not exist.');
				        else {
				            vm.fieldModel.removeLanguage(languages.currentLanguage);
				            vs.setLanguage(languages.currentLanguage, false);
				        }
				    },
				    all: {
				        translate: function translate() {
				            forAllMenus('translate');
				        },
				        linkDefault: function linkDefault() {
				            forAllMenus('linkDefault');
				        },
				        copyFrom: function (languageKey) {
				            forAllMenus('copyFrom', languageKey);
				        },
				        useFrom: function (languageKey) {
				            forAllMenus('useFrom', languageKey);
				        },
				        shareFrom: function (languageKey) {
				            forAllMenus('shareFrom', languageKey);
				        }
				    }
				};

			    // Collect all localizationMenus (to enable "all" actions)
				if ($scope.formModel.localizationMenus === undefined)
				    $scope.formModel.localizationMenus = [];
				$scope.formModel.localizationMenus.push(vm.actions);

				var forAllMenus = function (action, languageKey) {
				    for (var i = 0; i < $scope.formModel.localizationMenus.length; i++) {
				        $scope.formModel.localizationMenus[i][action](languageKey);
				    }
				};
			}]
		};
	});

	eavLocalization.directive("eavTreatTimeUtc", function () {
	    var directive = {
	        restrict: "A",
	        require: ["ngModel"],
            compile: compile,
	        link: link
	    };
	    return directive;

	    function compile(element, attributes) {

	    }

	    function link(scope, element, attributes, modelController) {     
	        modelController[0].$formatters.push(function (modelValue) {

	            return modelValue;
	        });

	        modelController[0].parsers.push(function (viewValue) {

	            return viewValue;
	        });
	    }
	});
})();


// Note: the entity-reader is meant for admin-purposes. 
// It does not try to do fallback, because the admin-UI MUST know the real data
function enhanceEntity(entity) {
    var enhancer = this; 

    enhancer.enhanceWithCount = function (obj) {
        obj.count = function () {
            var key, count = 0;
            for (key in this)
                if (this.hasOwnProperty(key) && typeof this[key] != 'function')
                    count++;
            return count;
        };
    }; 

    // this will enhance a Values with necessary methods
    enhancer.enhanceVs = function (vs) {
        vs.hasLanguage = function (language) { return this.Dimensions.hasOwnProperty(language); };
        vs.setLanguage = function (language, shareMode) { this.Dimensions[language] = shareMode; };
        vs.languageMode = function(language) { return (this.hasLanguage(language)) ? this.Dimensions[language] : null; };

        // ToDo: Fix enhance dimensions - or use alternative Object.keys(vs.Dimensions).length
        //if(typeof vs.Dimensions != "undefined")
            //enhancer.enhanceWithCount(vs.Dimensions);
        return vs;
    };

    // this will enhance an attribute
    enhancer.enhanceAtt = function(att) {
        att.getVsWithLanguage = function(language) {
            // try to find it based on the language - it then has a property matching the language
            for (var v = 0; v < this.Values.length; v++)
                if (this.Values[v].hasLanguage(language))
                    return this.Values[v];

            // if we don't find it, we must report it back as such
            return null;
        };

        att.setLanguageToVs = function(vs, language, shareMode) {
            // check if it's already there, if yes, just ensure shareMode, then done
            if (vs.hasLanguage(language))
                return vs.setLanguage(language, shareMode);

            // otherwise find the language if it's anywhere else and remove that first; 
            // note that this might delete a value set, so we should only do it after checking if it wasn't already right
            this.removeLanguage(language);

            // now set it anew
            return vs.setLanguage(language, shareMode);
        };

        att.removeLanguage = function(language) {
            var value = this.getVsWithLanguage(language);
            if (value === null)
                return;
            delete value.Dimensions[language];

            // check if the vs still has any properties left, if not, remove it entirely - unless it's the last one...
            if (Object.keys(value.Dimensions).length === 0 && this.Values.length > 0)
                this.removeVs(value);
        };

        att.removeVs = function(vs) {
            for (var v = 0; v < this.Values.length; v++)
                if (this.Values[v] === vs)
                    this.Values.splice(v, 1);
        };

        // todo: when adding VS - ensure the events are added too...
        att.addVs = function(value, language, shareMode) {
            var dimensions = {};
            if(language !== null)
                dimensions[language] = ((shareMode === null || shareMode === undefined) ? false : shareMode);
            var newVs = { "Value": value, "Dimensions": dimensions };
            // ToDo: enhancer.enhanceWithCount(newVs.Dimensions);
            this.Values.push(enhancer.enhanceVs(newVs));
        };

        // Now go through the Values and give them more commands
        for (var v = 0; v < att.Values.length; v++)
            enhancer.enhanceVs(att.Values[v]);

        return att;
    };

    // this will enhance an entity
    enhancer.enhanceEntity = function(ent) {
        ent.getTitle = function() {
            ent.getAttribute(ent.TitleAttributeName);
        };

        ent.hasAttribute = function(attrName) {
            return ent.Attributes[attrName] !== undefined;
        };

        ent.getAttribute = function(attrName) {
            return ent.Attributes[attrName];
        };

        // ToDo: Discuss with 2dm 
        ent.Attributes.addAttribute = function (attrName) {
            ent.Attributes[attrName] = { Values: [] };
            enhancer.enhanceAtt(ent.Attributes[attrName]);
        };

        for (var attKey in ent.Attributes)
            if(ent.Attributes.hasOwnProperty(attKey) && typeof(ent.Attributes[attKey]) != 'function')
                enhancer.enhanceAtt(ent.Attributes[attKey]);

        return ent;
    };

    return enhancer.enhanceEntity(entity);
}
/* service to manage input types */
(function () {
	"use strict";

    // notes: this has not been tested extensively
    // I'm guessing that it's not optimal yet, and I'm guessing that if the dialog is opened multiple times, that the list of dependencies just
    // keeps on growing and the UI might just get heavier with time ... must test once we have a few custom input types

	angular.module("eavEditEntity")
        .service("customInputTypes", ["eavConfig", "toastr", "formlyConfig", "$q", "$interval", "$ocLazyLoad", function (eavConfig, toastr, formlyConfig, $q, $interval, $ocLazyLoad) {
            // Returns a typed default value from the string representation
            var svc = {};
            svc.inputTypesOnPage = {};
            svc.allLoaded = true;
            svc.assetsToLoad = [];

	        svc.addInputType = function addInputType(field) {
	            var config = field.InputTypeConfig;
                // check if anything defined - older configurations don't have anything and will default to string-default anyhow
	            if (config === undefined || config === null)
	                return;

	            svc.inputTypesOnPage[config.Type] = config;

	            svc.addToLoadQueue(config);
	        };

	        svc.addToLoadQueue = function loadNewAssets(config) {
	            if (config.Assets === undefined || config.Assets === null || !config.Assets) {
	                config.assetsLoaded = true;
	                return;
	            }

	            // split by new-line, ensuring nothing blank
	            var list = config.Assets.split("\n");

	            for (var a = 0; a < list.length; a++) {
	                var asset = list[a].trim();
	                if (asset.length > 5) { // ensure we skip empty lines etc.
	                    svc.assetsToLoad.push(svc.resolveSpecialPaths(asset));
	                }
	            }
	        };

	        // now create promise and wait for everything to load
	        svc.loadWithPromise = function loadWithPromise() {
	            return $ocLazyLoad.load(svc.assetsToLoad);

	        };

	        svc.resolveSpecialPaths = function resolve(url) {
	            url = url.replace(/\[System:Path\]/i, eavConfig.getUrlPrefix("system"))
	                .replace(/\[Zone:Path\]/i, eavConfig.getUrlPrefix("zone"))
	                .replace(/\[App:Path\]/i, eavConfig.getUrlPrefix("app"));
	            return url;
	        };

	        svc.checkDependencyArrival = function cda(typeName) {
	            return !!formlyConfig.getType(typeName);
	        };

	        return svc;
	    }]);

})();
/* global angular */
(function () {
	"use strict";

	angular.module("eavEditEntity")
        .service("eavDefaultValueService", function () {
		// Returns a typed default value from the string representation
		return function parseDefaultValue(fieldConfig) {
			var e = fieldConfig;
			var d = e.templateOptions.settings.All.DefaultValue;

		    if (e.templateOptions.header.Prefill && e.templateOptions.header.Prefill[e.key]) {
			    d = e.templateOptions.header.Prefill[e.key];
			}

			switch (e.type.split("-")[0]) {
				case "boolean":
					return d !== undefined && d !== null ? d.toLowerCase() === "true" : false;
				case "datetime":
					return d !== undefined && d !== null && d !== "" ? new Date(d) : null;
			    case "entity":
			        if (!(d !== undefined && d !== null && d !== ""))
			            return []; // no default value

			        // 3 possibilities
			        if (d.constructor === Array) return d;  // possibility 1) an array

                    // for possibility 2 & 3, do some variation checking
			        if (d.indexOf("{") > -1) // string has { } characters, we must switch them to quotes
			            d = d.replace(/[\{\}]/g, "\"");

			        if (d.indexOf(",") !== -1 && d.indexOf("[") === -1) // list but no array, add brackets
			            d = "[" + d + "]";

			        return (d.indexOf("[") === 0) // possibility 2) an array with guid strings
			            ? JSON.parse(d) // if it's a string containing an array
			            : [d.replace(/"/g, "")]; //  possibility 3) just a guid string, but might have quotes
                        
				case "number":
				    return d !== undefined && d !== null && d !== "" ? Number(d) : "";
                default:
					return d ? d : "";
			}
		};
	});

})();
/* global angular */
(function () {
    "use strict";

    angular.module("eavEditEntity")
        /// Standard entity commands like get one, many etc.
        .factory('entitiesSvc', ["$http", "appId", "toastrWithHttpErrorHandling", "promiseToastr", "$q", "$translate", "toastr", function ($http, appId, toastrWithHttpErrorHandling, promiseToastr, $q, $translate, toastr) {
            var svc = {
                toastr: toastrWithHttpErrorHandling
            };

            svc.getManyForEditing = function (appId, items) {
                return $http.post('eav/entities/getmanyforediting', items, { params: { appId: appId } });
            };
            
            svc.saveMany = function (appId, items, partOfPage) {

                // first clean up unnecessary nodes - just to make sure we don't miss-read the JSONs transferred
                var removeTempValue = function (value, key) { delete value._currentValue; };
                var itmCopy = angular.copy(items);
                for (var ei = 0; ei < itmCopy.length; ei++) angular.forEach(itmCopy[ei].Entity.Attributes, removeTempValue);

                return $http.post("eav/entities/savemany", itmCopy, { params: { appId: appId, partOfPage: partOfPage || false } })
                    .then(function (serverKeys) {
                        var syncUpdatedKeys = function (value, key) {
                            // first ensure we don't break something
                            var ent = value.Entity;
                            if ((ent.Id === null || ent.Id === 0) && (ent.Guid !== null || typeof (ent.Guid) !== "undefined" || ent.Guid !== "00000000-0000-0000-0000-000000000000")) {
                                // try to find it in the return material to re-assign it
                                var newId = serverKeys.data[ent.Guid];
                                value.Entity.Id = newId;
                                value.Header.ID = newId;
                            }
                        };
                        angular.forEach(items, syncUpdatedKeys);

                        return serverKeys;
                    });
            };

            svc.tryDeleteAndAskForce = function tryDeleteAndAskForce(type, id, itemTitle) {
                var deferred = $q.defer();

                // todo: i18n
                var msg = $translate.instant("General.Questions.DeleteEntity", { title: itemTitle, id: id });
                if (!confirm(msg))
                    deferred.reject("Delete aborted by user");
                else {
                    svc.delete(type, id, false).then(function (result) {

                        if (result.status >= 200 && result.status < 300) {
                            deferred.resolve(result);
                        }
                        else {
                            // if delete failed, ask to force-delete in a toaster
                            var msg = "<div>" + $translate.instant("General.Questions.ForceDelete", { title: itemTitle, id: id }) + "<br/>"
                                + "<button type='button' id='del' class='btn btn-default' ><i class= 'eav-icon-ok'></i>" + $translate.instant("General.Buttons.ForceDelete") + "</button>"
                                + "</div>";

                            toastr.warning(msg, {
                                allowHtml: true,
                                timeOut: 5000,
                                onShown: function (toast) {
                                    // this checks for the click on the button in the toaster
                                    toast.el[0].onclick = function (event) {
                                        var target = event.target || event.srcElement;
                                        if (target.id === "del")
                                            svc.delete(type, id, true)
                                                .then(deferred.resolve);
                                    };
                                }
                            });
                        }
                    });
                }
                return deferred.promise;
            };

            svc.delete = function del(type, id, tryForce) {
                console.log("try to delete");

                var delPromise = $http.get("eav/entities/delete", {
                    ignoreErrors: true,
                    params: {
                        'contentType': type,
                        'id': id,
                        'appId': appId,
                        'force': tryForce
                    }
                });

                return promiseToastr(delPromise, "Message.Deleting", "Message.Ok", "Message.Error");
            };

            svc.newEntity = function (header) {
                return {
                    Id: null,
                    Guid: header.Guid,
                    Type: {
                        StaticName: header.ContentTypeName // contentTypeName
                    },
                    Attributes: {},
                    IsPublished: true
                };
            };

            svc.save = function save(appId, newData) {
                return $http.post("eav/entities/save", newData, { params: { appId: appId } });
            };

            return svc;
        }]);


})();
/*
 * This is a special service which uses a field mask 
 * 
 * like "[Title] - [Subtitle]" 
 * 
 * and will then provide a list of fields which were used, as well as a resolved value if needed
 * 
 */

angular.module("eavFieldTemplates")
    .factory("fieldMask", ["debugState", function (debugState) {
        // mask: a string like "[FirstName] [LastName]"
        // model: usually the $scope.model, passed into here
        // overloadPreCleanValues: a function which will "scrub" the found field-values
        // 
        // use: first create an object like mask = fieldMask.createFieldMask("[FirstName]", $scope.model);
        //      then: access result in your timer or whatever with mask.resolve();
        function createFieldMask(mask, $scope, changeEvent, overloadPreCleanValues) {
            var srv = {
                mask: mask,
                model: $scope.model,
                fields: [],
                value: undefined,
                findFields: /\[.*?\]/ig,
                unwrapField: /[\[\]]/ig
            };

            // resolves a mask to the final value
            srv.resolve = function getNewAutoValue() {
                var value = srv.mask;
                angular.forEach(srv.fields, function(e, i) {
                    var replaceValue = (srv.model.hasOwnProperty(e) && srv.model[e] && srv.model[e]._currentValue && srv.model[e]._currentValue.Value)
                        ? srv.model[e]._currentValue.Value : "";
                    var cleaned = srv.preClean(e, replaceValue);
                    value = value.replace("[" + e + "]", cleaned);
                });

                return value;
            };

            // retrieves a list of all fields used in the mask
            srv.fieldList = function() {
                var result = [];
                if (!srv.mask) return result;
                var matches = srv.mask.match(srv.findFields);
                angular.forEach(matches, function(e, i) {
                    var staticName = e.replace(srv.unwrapField, "");
                    result.push(staticName);
                });
                return result;
            };

            srv.preClean = function(key, value) {
                return value;
            };

            // change-event - will only fire if it really changes
            srv.onChange = function() {
                var maybeNew = srv.resolve();
                if (srv.value !== maybeNew)
                    changeEvent(maybeNew);
                srv.value = maybeNew;
            };

            // add watcher and execute onChange
            srv.watchAllFields = function() {
                // add a watch for each field in the field-mask
                angular.forEach(srv.fields, function (e, i) {
                    $scope.$watch("model." + e + "._currentValue.Value", function () {
                        if (debugState.on) console.log("url-path: " + e + " changed...");
                        srv.onChange();
                    });
                });
            };

            function activate() {
                srv.fields = srv.fieldList();

                if (overloadPreCleanValues) // got an overload...
                    srv.preClean = overloadPreCleanValues;

                // bind auto-watch only if needed...
                if ($scope && changeEvent)
                    srv.watchAllFields();
            }

            activate();

            return srv;
        }

        return createFieldMask;
    }]);
angular.module("eavEditTemplates", []).run(["$templateCache", function($templateCache) {$templateCache.put("fields/boolean/boolean-default.html","<div class=\"checkbox checkbox-labeled\">\r\n    <!--<label>-->\r\n        <switch class=\"tosic-green pull-left\" ng-model=\"value.Value\"></switch>\r\n    <!-- maybe need the (hidden) input to ensure the label actually switches the boolean -->\r\n        <!--<input type=\"checkbox\" class=\"formly-field-checkbox\" ng-model=\"value.Value\" style=\"display: none\">-->\r\n        <div ng-include=\"\'wrappers/eav-label.html\'\"></div>\r\n        <!--{{to.label}} {{to.required ? \'*\' : \'\'}}-->\r\n    <!--</label>-->\r\n</div>");
$templateCache.put("fields/custom/custom-default.html","<div class=\"alert alert-danger\">\r\n    ERROR - This is a custom field, you shouldn\'t see this. You only see this because the custom-dialog is missing.\r\n</div>\r\n<input class=\"form-control input-lg\" ng-pattern=\"vm.regexPattern\" ng-model=\"value.Value\">");
$templateCache.put("fields/datetime/datetime-default.html","<div>\r\n    <div class=\"input-group\" style=\"width: 100%\">\r\n        <input class=\"form-control input-lg\" ng-model=\"value.Value\" is-open=\"to.isOpen\" uib-datepicker-popup=\"{{format}}\" close-text=\"{{datepickerPopup.closeText}}\" clear-text=\"{{datepickerPopup.clearText}}\" current-text=\"{{datepickerPopup.currentText}}\" datepicker-options=\"to.datepickerOptions\" />\r\n\r\n        <span class=\"input-group-btn\" style=\"vertical-align: top;\">\r\n            <button type=\"button\" class=\"btn btn-default eav-icon-field-button pull-right icon-field-button icon-field-button-calendar\"\r\n                    ng-disabled=\"to.disabled\"\r\n                    ng-click=\"to.isOpen = true;\">\r\n                <i class=\"eav-icon-calendar\"></i>\r\n            </button>\r\n        </span>\r\n        <!--<div class=\"input-group-addon\" style=\"cursor: pointer;\" ng-click=\"to.isOpen = true;\">\r\n            <i class=\"glyphicon glyphicon-calendar\"></i>\r\n        </div>-->\r\n        <div uib-timepicker ng-show=\"to.settings.merged.UseTimePicker\" ng-model=\"value.Value\" show-meridian=\"ismeridian\" style=\"display:table-row;\"></div>\r\n    </div>\r\n</div>\r\n");
$templateCache.put("fields/empty/empty-default.html","<span></span>");
$templateCache.put("fields/entity/entity-default.html","<div class=\"eav-entityselect\">\r\n    <div>\r\n        <div ui-tree=\"options\" data-empty-placeholder-enabled=\"false\" ng-show=\"to.settings.merged.EnableCreate || chosenEntities.length > 0\">\r\n            <table ui-tree-nodes ng-model=\"chosenEntities\" entity-validation ng-required=\"false\" class=\"eav-entityselect-table\" style=\"table-layout: fixed;\">\r\n                <thead>\r\n                <tr>\r\n                    <th></th>\r\n                    <th></th>\r\n                    <th></th>\r\n                </tr>\r\n                </thead>\r\n                <!-- important note - track by $index very important for multiple null-values in list -->\r\n                <tr ng-repeat=\"item in chosenEntities track by $index\" ui-tree-node class=\"eav-entityselect-item\" >\r\n                    <td ui-tree-handle>\r\n                        <i title=\"{{ \'FieldType.Entity.DragMove\' | translate }}\" class=\"eav-icon-link pull-left eav-entityselect-icon\" ng-show=\"to.settings.Entity.AllowMultiValue\"></i>\r\n                        <i title=\"\" class=\"eav-icon-link pull-left eav-entityselect-icon\" ng-show=\"!to.settings.Entity.AllowMultiValue\"></i>\r\n                    </td>\r\n                    <td ui-tree-handle>\r\n                        <span class=\"eav-entityselect-item-title\" title=\"{{getEntityText(item) + \' (\' + item + \')\'}}\">{{getEntityText(item)}}</span>\r\n                    </td>\r\n                    <td style=\"text-align: right;\">\r\n                        <ul class=\"eav-entityselect-item-actions\">\r\n                            <li>\r\n                                <a title=\"{{ \'FieldType.Entity.Edit\' | translate }}\" ng-click=\"edit(item, index)\" ng-show=\"to.settings.merged.EnableEdit\" data-nodrag>\r\n                                    <i class=\"eav-icon-pencil\"></i>\r\n                                </a>\r\n                            </li>\r\n                            <li>\r\n                                <a title=\"{{ \'FieldType.Entity.Remove\' | translate }}\" ng-click=\"removeSlot(item, $index)\" class=\"eav-entityselect-item-remove\" ng-show=\"to.settings.merged.EnableRemove\" data-nodrag>\r\n                                    <i ng-class=\"{ \'eav-icon-minus-circled\': to.settings.merged.AllowMultiValue, \'eav-icon-down-dir\': !to.settings.merged.AllowMultiValue  }\"></i>\r\n                                </a>\r\n                            </li>\r\n                            <li>\r\n                                <!-- todo: i18n, code in action, eav-icon-visiblity/alignment -->\r\n                                <a title=\"{{ \'FieldType.Entity.Delete\' | translate }}\" ng-click=\"deleteItemInSlot(item, $index)\" class=\"eav-entityselect-item-remove\" ng-show=\"to.settings.merged.EnableDelete\" data-nodrag>\r\n                                    <i class=\"eav-icon-cancel\"></i>\r\n                                </a>\r\n                            </li>\r\n                        </ul>\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n        </div>\r\n\r\n        <div class=\"eav-entityselect-actions\" ng-class=\"{ \'no-add\': (!to.settings.merged.AllowMultiValue && chosenEntities.length > 0) }\">\r\n\r\n            <!-- pick existing entity -->\r\n            <div class=\"eav-entityselect-action-addexisting\"\r\n                 ng-show=\"to.settings.merged.EnableAddExisting && (to.settings.merged.AllowMultiValue || chosenEntities.length < 1)\">\r\n                <div class=\"eav-entityselect-selector-wrapper\">\r\n                    <div class=\"eav-entityselect-action-addexisting-close\" ng-show=\"$select.open\">Close</div>\r\n                    <ui-select theme=\"bootstrap\"\r\n                               ng-model=\"selectedEntity\"\r\n                               on-highlight=\"maybeReload()\"\r\n                               ng-required=\"false\"\r\n                               on-select=\"addEntity($select.selected.Value)\">\r\n                        <ui-select-match placeholder=\"{{ $select.open ? \'search\' : \'FieldType.Entity.Choose\' | translate }}\">\r\n                            <!--<div ng-bind=\"$select.selected.Text\"></div>-->\r\n                            {{ \'FieldType.Entity.Choose\' | translate }}\r\n                        </ui-select-match>\r\n                        <ui-select-choices ng-style=\"{opacity: $select.open ? 1 : 0 }\"  repeat=\"item in availableEntities | filter: { Text : $select.search } track by item.Value\"\r\n                                           refresh=\"maybeReload()\"\r\n                                           refresh-delay=\"0\"\r\n                                           minimum-input-length=\"0\"\r\n                                           ui-disable-choice=\"chosenEntities.indexOf(item.Value) != -1\">\r\n                            <span ng-bind=\"item.Text\"></span>\r\n                        </ui-select-choices>\r\n                    </ui-select>\r\n                </div>\r\n            </div>\r\n\r\n            <!-- create new entity to add to this list -->\r\n            <button ng-if=\"to.settings.merged.EnableCreate && to.settings.merged.EntityType !== \'\' && (to.settings.merged.AllowMultiValue || chosenEntities.length < 1)\"\r\n                    class=\"eav-entityselect-action-create icon-field-button\"\r\n                    ng-click=\"openNewEntityDialog()\">\r\n                <i class=\"eav-icon-plus\"></i>\r\n            </button>\r\n        </div>\r\n        \r\n        <div ng-if=\"debug.on\">\r\n            debug: <span ng-click=\"insertNull()\">add null-item</span>\r\n        </div>\r\n\r\n        <!-- test - want to re-align how two add-scenarios work; ideally side-by side\r\n        <div class=\"subtle-till-mouseover\"\r\n             ng-show=\"(to.settings.merged.EnableAddExisting && (to.settings.merged.AllowMultiValue || chosenEntities.length < 1)) || (to.settings.merged.EnableCreate && (to.settings.merged.AllowMultiValue || chosenEntities.length < 1))\">\r\n        </div>-->\r\n    </div>\r\n</div>");
$templateCache.put("fields/string/string-contenttype.html","<div>\r\n    <select class=\"form-control input-material material\"\r\n            ng-model=\"value.Value\">\r\n        <option value=\"\">(none)</option>\r\n        <option\r\n            ng-repeat=\"item in contentTypes\"\r\n            ng-selected=\"{{item.StaticName == value.Value}}\"\r\n            value=\"{{item.StaticName}}\">\r\n            {{item.Label}}\r\n        </option>\r\n    </select>\r\n</div>");
$templateCache.put("form/edit-many-entities.html","<div ng-if=\"vm.items != null\" ng-click=\"vm.debug.autoEnableAsNeeded($event)\" class=\"form-container-multi\">\r\n    <eav-language-switcher is-disabled=\"!vm.isValid()\"></eav-language-switcher>\r\n    <div ng-repeat=\"p in vm.items\" class=\"group-entity\">\r\n        <div class=\"form-ci-title unhide-area\" ng-click=\"p.collapse = !p.collapse\">\r\n            <span style=\"position: relative\">\r\n                <i class=\"decoration eav-icon-side-marker\"></i>\r\n                <i class=\"decoration state eav-icon-minus collapse-entity-button hide-till-mouseover\" ng-if=\"!p.collapse\"></i>\r\n                <i class=\"decoration state eav-icon-plus collapse-entity-button\" ng-if=\"p.collapse\"></i>\r\n            </span>\r\n            {{p.Header.Title ? p.Header.Title : \'EditEntity.DefaultTitle\' | translate }}&nbsp;\r\n            <span ng-if=\"p.Header.Group.SlotCanBeEmpty\" ng-click=\"vm.toggleSlotIsEmpty(p)\" stop-event=\"click\">\r\n                <i class=\"eav-icon-toggle-off\" ng-class=\" p.slotIsUsed ? \'eav-icon-toggle-on\' : \'eav-icon-toggle-off\' \" ng-click=\"p.slotIsUsed = !p.slotIsUsed\" uib-tooltip=\"{{\'EditEntity.SlotUsed\' + p.slotIsUsed | translate}}\"></i>\r\n            </span>\r\n        </div>\r\n        <div ng-if=\"vm.itemsHelp[$index]\" ng-bind-html=\"vm.itemsHelp[$index]\"></div>\r\n        <eav-edit-entity-form entity=\"p.Entity\" header=\"p.Header\" register-edit-control=\"vm.registerEditControl\" ng-hide=\"p.collapse\"></eav-edit-entity-form>\r\n    </div>\r\n    <div>\r\n        <!-- note: the buttons are not really disabled, because we want to be able to click them and see the error message -->\r\n        <div class=\"btn-group\" uib-dropdown>\r\n            <button ng-class=\"{ \'disabled\': !vm.isValid() || vm.isWorking > 0}\" ng-click=\"vm.save(true)\" type=\"button\" class=\"btn btn-primary btn-lg submit-button\">\r\n                <span class=\"eav-icon-ok\" uib-tooltip=\"{{ \'Button.Save\' | translate }}\"></span> &nbsp;<span translate=\"Button.Save\"></span>\r\n            </button>\r\n            <button class=\"dropdown-toggle btn btn-primary btn-lg\" ng-class=\"{ \'disabled\': !vm.isValid() || vm.isWorking > 0}\" uib-dropdown-toggle><i class=\"caret\"></i></button>\r\n            <ul class=\"dropdown-menu\" role=\"menu\">\r\n                <li><a ng-click=\"vm.save(true)\"><i class=\"eav-icon-ok\"></i> {{ \'Button.Save\' | translate }}</a></li>\r\n                <li><a ng-click=\"vm.save(false)\"><i class=\"eav-icon-ok-circled2\"></i> {{ \'Button.SaveAndKeepOpen\' | translate }}</a></li>\r\n            </ul>\r\n        </div>\r\n\r\n        &nbsp;\r\n        <!-- note: published status will apply to all - so the first is taken for identification if published -->\r\n        &nbsp;\r\n        <div class=\"btn-group\" uib-dropdown>\r\n            <a class=\"dropdown-toggle\" uib-dropdown-toggle><i ng-class=\"{\'eav-icon-eye\': vm.publishMode === \'show\', \'eav-icon-eye-close\': vm.publishMode === \'hide\', \'eav-icon-git-branch\': vm.publishMode === \'branch\'}\"></i> {{ \'SaveMode.\' + vm.publishMode |translate }}<i class=\"caret\"></i></a>\r\n            <ul class=\"dropdown-menu\" role=\"menu\">\r\n                <li ng-if=\"vm.versioningOptions.show\"><a ng-click=\"vm.publishMode = \'show\'\"><i class=\"eav-icon-eye\"></i> {{ \'SaveMode.show\' |translate }}</a></li>\r\n                <li ng-if=\"vm.versioningOptions.hide\"><a ng-click=\"vm.publishMode = \'hide\'\"><i class=\"eav-icon-eye-close\"></i> {{ \'SaveMode.hide\' |translate }}</a></li>\r\n                <li ng-if=\"vm.versioningOptions.branch\" ng-show=\"vm.enableDraft\"><a ng-click=\"vm.publishMode = \'branch\'\"><i class=\"eav-icon-git-branch\"></i> {{ \'SaveMode.branch\' |translate }}</a></li>\r\n            </ul>\r\n        </div>\r\n        <span ng-if=\"vm.debug.on\">\r\n            <button class=\"eav-icon-flash btn\" uib-tooltip=\"debug\" ng-click=\"vm.showDebugItems = !vm.showDebugItems\"></button>\r\n        </span>\r\n        <show-debug-availability class=\"pull-right\" style=\"margin-top: 20px;\"></show-debug-availability>\r\n    </div>\r\n    <div ng-if=\"vm.debug.on && vm.showDebugItems\">\r\n        <div>\r\n            isValid: {{vm.isValid()}}<br />\r\n            isWorking: {{vm.isWorking}}\r\n        </div>\r\n        <pre>{{ vm.items | json }}</pre>\r\n    </div>\r\n</div>");
$templateCache.put("form/edit-single-entity.html","<div ng-show=\"vm.editInDefaultLanguageFirst()\" translate=\"Message.PleaseCreateDefLang\">\r\n	\r\n</div>\r\n<div ng-show=\"!vm.editInDefaultLanguageFirst()\">\r\n    <formly-form ng-if=\"vm.formFields && vm.formFields.length\" ng-submit=\"vm.onSubmit()\" form=\"vm.form\" model=\"vm.entity.Attributes\" fields=\"vm.formFields\"></formly-form>\r\n</div>\r\n");
$templateCache.put("form/main-form.html","<div class=\"modal-body-disabled\">\r\n    <span class=\"pull-right\">\r\n        <span style=\"display: inline-block; position: relative; left:0px\">\r\n            <button class=\"btn btn-default btn-icon-square btn-subtle\" type=\"button\" ng-click=\"vm.close()\">\r\n                <i class=\"eav-icon-cancel\"></i>\r\n            </button>\r\n        </span>\r\n    </span>\r\n    <eav-edit-entities part-of-page=\"vm.partOfPage\" publishing=\"vm.publishing\" item-list=\"vm.itemList\" after-save-event=\"vm.afterSave\" state=\"vm.state\" close=\"vm.close\"></eav-edit-entities>\r\n</div>");
$templateCache.put("localization/formly-localization-wrapper.html","<eav-localization-scope-control></eav-localization-scope-control>\r\n<div ng-if=\"!!value\">\r\n    <formly-transclude></formly-transclude>\r\n    <eav-localization-menu form-model=\"model\" field-model=\"model[options.key]\" options=\"options\" value=\"value\" index=\"index\"></eav-localization-menu>\r\n</div>\r\n<p class=\"bg-info\" style=\"padding:12px;\" ng-if=\"!value\" translate=\"LangWrapper.CreateValueInDefFirst\" translate-values=\"{ fieldname: \'{{to.label}}\' }\">Please... <i>\'{{to.label}}\'</i> in the def...</p>");
$templateCache.put("localization/language-switcher.html","<uib-tabset ng-init=\"activeLang = languages.currentLanguage;\" active=\"activeLang\">\r\n    <uib-tab ng-repeat=\"l in languages.languages\" index=\"l.key\" heading=\"{{ l.name.substring(0, l.name.indexOf(\'(\') > 0 ? l.name.indexOf(\'(\') - 1 : 100 ) }}\" ng-click=\"!isDisabled ? languages.currentLanguage = l.key : false;\" disable=\"isDisabled\" uib-tooltip=\"{{l.name}}\"></uib-tab><!-- -->\r\n</uib-tabset>");
$templateCache.put("localization/localization-menu.html","<div uib-dropdown is-open=\"status.isopen\" class=\"eav-localization\"> <!--style=\"z-index:{{1000 - index}};\"-->\r\n	<a class=\"eav-localization-lock\" ng-click=\"vm.actions.toggleTranslate();\" ng-if=\"vm.isDefaultLanguage()\" title=\"{{vm.tooltip()}}\" ng-class=\"{ \'eav-localization-lock-open\': !options.templateOptions.disabled }\" uib-dropdown-toggle>\r\n        {{vm.infoMessage()}} <i class=\"glyphicon glyphicon-globe\"></i>\r\n	</a>\r\n    <ul class=\"dropdown-menu multi-level pull-right eav-localization-dropdown\" role=\"menu\" aria-labelledby=\"single-button\">\r\n        <li role=\"menuitem\"><a ng-click=\"vm.actions.translate()\" translate=\"LangMenu.Unlink\"></a></li>\r\n        <li role=\"menuitem\"><a ng-click=\"vm.actions.linkDefault()\" translate=\"LangMenu.LinkDefault\"></a></li>\r\n        <!-- Google translate is disabled because there is no longer a free version\r\n            <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n            <a href=\"#\" translate=\"LangMenu.GoogleTranslate\"></a>\r\n            <ul class=\"dropdown-menu\">\r\n                <li ng-repeat=\"language in vm.languages.languages\" role=\"menuitem\">\r\n                    <a ng-click=\"vm.actions.autoTranslate(language.key)\" title=\"{{language.name}}\" href=\"#\">{{language.key}}</a>\r\n                </li>\r\n            </ul>\r\n        </li>-->\r\n        <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n            <a href=\"#\" translate=\"LangMenu.Copy\"></a>\r\n            <ul class=\"dropdown-menu\">\r\n                <li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: options.templateOptions.disabled || !vm.hasLanguage(language.key) }\" role=\"menuitem\">\r\n                    <a ng-click=\"vm.actions.copyFrom(language.key)\" title=\"{{language.name}}\" href=\"#\">{{language.key}}</a>\r\n                </li>\r\n            </ul>\r\n        </li>\r\n        <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n            <a href=\"#\" translate=\"LangMenu.Use\"></a>\r\n            <ul class=\"dropdown-menu\">\r\n                <li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: !vm.hasLanguage(language.key) }\" role=\"menuitem\">\r\n                    <a ng-click=\"vm.actions.useFrom(language.key)\" title=\"{{language.name}}\" href=\"#\">{{language.key}}</a>\r\n                </li>\r\n            </ul>\r\n        </li>\r\n        <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n            <a href=\"#\" translate=\"LangMenu.Share\"></a>\r\n            <ul class=\"dropdown-menu\">\r\n                <li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: !vm.hasLanguage(language.key) }\" role=\"menuitem\">\r\n                    <a ng-click=\"vm.actions.shareFrom(language.key)\" title=\"{{language.name}}\" href=\"#\">{{language.key}}</a>\r\n                </li>\r\n            </ul>\r\n        </li>\r\n        <!-- All fields -->\r\n        <li class=\"divider\"></li>\r\n        <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n            <a href=\"#\" translate=\"LangMenu.AllFields\"></a>\r\n            <ul class=\"dropdown-menu\">\r\n                <li role=\"menuitem\"><a ng-click=\"vm.actions.all.translate()\" translate=\"LangMenu.Unlink\"></a></li>\r\n                <li role=\"menuitem\"><a ng-click=\"vm.actions.all.linkDefault()\" translate=\"LangMenu.LinkDefault\"></a></li>\r\n                <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n                    <a href=\"#\" translate=\"LangMenu.Copy\"></a>\r\n                    <ul class=\"dropdown-menu\">\r\n                        <li ng-repeat=\"language in vm.languages.languages\" role=\"menuitem\">\r\n                            <a ng-click=\"vm.actions.all.copyFrom(language.key)\" title=\"{{language.name}}\" href=\"#\">{{language.key}}</a>\r\n                        </li>\r\n                    </ul>\r\n                </li>\r\n                <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n                    <a href=\"#\" translate=\"LangMenu.Use\"></a>\r\n                    <ul class=\"dropdown-menu\">\r\n                        <li ng-repeat=\"language in vm.languages.languages\" role=\"menuitem\">\r\n                            <a ng-click=\"vm.actions.all.useFrom(language.key)\" title=\"{{language.name}}\" href=\"#\">{{language.key}}</a>\r\n                        </li>\r\n                    </ul>\r\n                </li>\r\n                <li role=\"menuitem\" class=\"dropdown-submenu\">\r\n                    <a href=\"#\" translate=\"LangMenu.Share\"></a>\r\n                    <ul class=\"dropdown-menu\">\r\n                        <li ng-repeat=\"language in vm.languages.languages\" role=\"menuitem\">\r\n                            <a ng-click=\"vm.actions.all.shareFrom(language.key)\" title=\"{{language.name}}\" href=\"#\">{{language.key}}</a>\r\n                        </li>\r\n                    </ul>\r\n                </li>\r\n            </ul>\r\n        </li>\r\n    </ul>\r\n</div>");
$templateCache.put("ml-entities/tests/SpecRunner.html","<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <meta charset=\"utf-8\">\r\n  <title>Jasmine Spec Runner v2.3.4</title>\r\n    <!--\r\n  <link rel=\"shortcut icon\" type=\"image/png\" href=\"lib/jasmine-2.3.4/jasmine_favicon.png\">\r\n  <link rel=\"stylesheet\" href=\"lib/jasmine-2.3.4/jasmine.css\">\r\n\r\n  <script src=\"lib/jasmine-2.3.4/jasmine.js\"></script>\r\n  <script src=\"lib/jasmine-2.3.4/jasmine-html.js\"></script>\r\n  <script src=\"lib/jasmine-2.3.4/boot.js\"></script>\r\n        -->\r\n\r\n    <link rel=\"stylesheet\" href=\"../../../../node_modules\\grunt-contrib-jasmine\\node_modules\\jasmine-core/lib/jasmine-core/jasmine.css\">\r\n    <script src=\"../../../../node_modules\\grunt-contrib-jasmine\\node_modules\\jasmine-core/lib/jasmine-core/jasmine.js\"></script>\r\n    <script src=\"../../../../node_modules\\grunt-contrib-jasmine\\node_modules\\jasmine-core/lib/jasmine-core/jasmine-html.js\"></script>\r\n    <script src=\"../../../../node_modules\\grunt-contrib-jasmine\\node_modules\\jasmine-core/lib/jasmine-core/boot.js\"></script>\r\n  <!-- include source files here... -->\r\n    <!--\r\n  <script src=\"src/Player.js\"></script>\r\n  <script src=\"src/Song.js\"></script>\r\n    -->\r\n    <script src=\"../entity-enhancer.js\"></script>\r\n\r\n\r\n  <!-- include spec files here... -->\r\n    <!--\r\n  <script src=\"spec/SpecHelper.js\"></script>\r\n  <script src=\"spec/PlayerSpec.js\"></script>\r\n        -->\r\n    <script src=\"../specs/eav-content-ml.spec.js\"></script>\r\n\r\n</head>\r\n\r\n<body>\r\n</body>\r\n</html>\r\n");
$templateCache.put("wrappers/collapsible.html","<!-- hide entire field if necessary-->\r\n<div ng-show=\"!to.collapse\" class=\"group-field-set form-field-grid-keeper\">\r\n    <formly-transclude></formly-transclude>\r\n</div>");
$templateCache.put("wrappers/disablevisually.html","<div visually-disabled=\"{{to.disabled}}\">\r\n    <formly-transclude></formly-transclude>\r\n</div>");
$templateCache.put("wrappers/eav-label-inside.html","<label for=\"{{id}}\" class=\"control-label eav-label {{to.labelSrOnly ? \'sr-only\' : \'\'}} {{to.type}}\"\r\n       ng-if=\"to.label\"\r\n       ng-click=\"to.collapseField = !to.collapseField\">\r\n    {{to.label}}\r\n    {{to.required ? \'*\' : \'\'}}\r\n    <a tabindex=\"-1\" ng-click=\"to.showDescription = !to.showDescription\" href=\"javascript:void(0);\" ng-if=\"to.description && to.description != \'\'\">\r\n        <i class=\"eav-icon-info-circled low-priority\"></i>\r\n    </a>\r\n    <span class=\"btn-sm\" ng-if=\"to.enableCollapseField\">\r\n        <span ng-if=\"to.collapseField\" class=\"eav-icon-plus-circled low-priority collapse-fieldgroup-button\"></span>\r\n        <span ng-if=\"!to.collapseField\" class=\"eav-icon-minus-circled low-priority collapse-fieldgroup-button\"></span>\r\n    </span>\r\n</label>");
$templateCache.put("wrappers/eav-label.html","<div>\r\n    <!-- just fyi: the ng-class adds a \"float-away\" if the notes are shown or if the field has content -->\r\n    <div>\r\n        <div class=\"inside\" ng-include=\"\'wrappers/eav-label-inside.html\'\"></div>\r\n        <div ng-if=\"to.showDescription\" class=\"info-wrapper\">\r\n            <p class=\"bg-info\" style=\"padding: 5px;\" ng-bind-html=\"to.description\">\r\n            </p>\r\n        </div>\r\n\r\n        <div ng-show=\"!(to.collapseField && to.enableCollapseField)\">\r\n            <formly-transclude></formly-transclude>\r\n        </div>\r\n        \r\n        <!-- special debug layer, only enable during advanced debugging, usually never needed -->\r\n        <div ng-if=\"false && debug.on\">\r\n            Field-Debug: {{fc}}\r\n        </div>\r\n    </div>\r\n</div>");
$templateCache.put("wrappers/field-group.html","<div>\r\n    <div class=\"form-ci-subtitle unhide-area\" ng-click=\"toggle()\">\r\n        <span style=\"position: relative\">\r\n            <i class=\"eav-icon-side-marker decoration\"></i>\r\n            <span ng-if=\"to.collapseGroup\" class=\"decoration state eav-icon-plus-circled low-priority collapse-fieldgroup-button\"></span>\r\n            <span ng-if=\"!to.collapseGroup\" class=\"decoration state eav-icon-minus-circled low-priority collapse-fieldgroup-button hide-till-mouseover\"></span>\r\n        </span>\r\n        {{to.label}}\r\n    </div>\r\n    <div ng-if=\"!to.collapseGroup\" style=\"padding: 5px;\" ng-bind-html=\"to.description\">\r\n    </div>\r\n    <formly-transclude></formly-transclude>\r\n</div>");
$templateCache.put("wrappers/float-label.html","<div class=\"wrap-float-label\"\r\n     ng-class=\"[\r\n     {\'float-disabled\': value.Value || (fc[0] || fc).$modelValue || to.showDescription || to.focused || ( (fc[0] || fc).$invalid && (fc[0] || fc).$touched ) },\r\n     {focused: to.focused},\r\n     {\'ng-touched\': (fc[0] || fc).$touched},\r\n     {\'ng-invalid\' : (fc[0] || fc).$invalid}\r\n     ]\">\r\n    <formly-transclude></formly-transclude>\r\n</div>");
$templateCache.put("wrappers/hidden.html","<div ng-show=\"{{to.settings.All.VisibleInEditUI || to.debug}}\">\r\n    <formly-transclude></formly-transclude>\r\n</div>");
$templateCache.put("wrappers/no-label-space.html","<div class=\"no-label-space\">\r\n    <formly-transclude></formly-transclude>\r\n</div>");
$templateCache.put("wrappers/preview-default.html","<div class=\"preview-default\">\r\n    <div class=\"preview-area\"></div>\r\n    <div>\r\n        <formly-transclude></formly-transclude>\r\n    </div>\r\n</div>");
$templateCache.put("wrappers/responsive.html","<div class=\"clearfix\">\r\n    <div class=\"responsive-optional\" >\r\n        <div ng-include=\"\'wrappers/eav-label-inside.html\'\"></div>\r\n    </div>\r\n    <div class=\"responsive-priority\">\r\n        <formly-transclude></formly-transclude>\r\n    </div>\r\n</div>");}]);
/*
 * This wrapper should be around all fields, so that they can collapse 
 * when a field-group-title requires collapsing
 */
(function () {
    "use strict";

    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function (formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'collapsible',
                templateUrl: "wrappers/collapsible.html"
            });
        }]);
})();

(function() {
	"use strict";

    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function(formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'disablevisually',
                templateUrl: "wrappers/disablevisually.html"
            });
        }]);
})();

(function() {
	"use strict";

    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function(formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'eavLabel',
                templateUrl: "wrappers/eav-label.html"
            });
        }]);
})();

/*
 * This is the label-wrapper of a group-title, 
 * and in the html allows show/hide of the entire group
 * show-hide works over the options property to.collapseGroup
 */
(function() {
	"use strict";

    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function(formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'fieldGroup',
                templateUrl: "wrappers/field-group.html"
            });
        }]);
})();
/*
 * This wrapper should be around all fields, so that they can float the label 
 */
(function () {
    "use strict";
    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function (formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'float-label',
                templateUrl: "wrappers/float-label.html"
            });
        }]);
})();

(function() {
	"use strict";

    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function(formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'hiddenIfNeeded',
                templateUrl: "wrappers/hidden.html"
            });
        }]);
})();
/*
 * This wrapper should be around all fields, so that they can float the label 
 */
(function () {
    "use strict";
    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function (formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'no-label-space',
                templateUrl: "wrappers/no-label-space.html"
            });
        }]);
})();
/*
 * This wrapper should be around all fields, so that they can float the label 
 */
(function () {
    "use strict";
    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function (formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'preview-default',
                templateUrl: "wrappers/preview-default.html"
            });
        }]);
})();
/*
 * This wrapper should be around all fields, so that they can float the label 
 */
(function () {
    "use strict";
    angular.module("eavFieldTemplates")
        .config(["formlyConfigProvider", function (formlyConfigProvider) {
            formlyConfigProvider.setWrapper({
                name: 'responsive',
                templateUrl: "wrappers/responsive.html"
            });
        }]);
})();