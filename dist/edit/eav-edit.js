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
        "ui.tree"
    ]
)
    // important: order of use is backwards, so the last is around the second-last, etc.
    .constant("defaultFieldWrappers", [
        "eavLabel",
        "bootstrapHasError",
        "disablevisually",
        "eavLocalization",
        "collapsible"
    ])
;
/* 
 * Field: Boolean - Default
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", function(formlyConfigProvider) {
        formlyConfigProvider.setType({
            name: "boolean-default",
            templateUrl: "fields/boolean/boolean-default.html",
            wrapper: ["bootstrapHasError", "disablevisually", "eavLocalization", "collapsible"]
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
            template: "<div>" +
                "<div class=\"input-group\">" +
                    "<div class=\"input-group-addon\" style=\"cursor:pointer;\" ng-click=\"to.isOpen = true;\">" +
                        "<i class=\"glyphicon glyphicon-calendar\"></i>" +
                    "</div>" +
                    "<input class=\"form-control input-lg\" ng-model=\"value.Value\" is-open=\"to.isOpen\" datepicker-options=\"to.datepickerOptions\" datepicker-popup />" +
				    "<timepicker ng-show=\"to.settings.merged.UseTimePicker\" ng-model=\"value.Value\" show-meridian=\"ismeridian\"></timepicker>" +
                "</div>",
            defaultOptions: {
                templateOptions: {
                    datepickerOptions: {},
                    datepickerPopup: "dd.MM.yyyy"
                }
            },
            link: function (scope, el, attrs) {
                // Server delivers value as string, so convert it to UTC date
                function convertDateToUTC(date) { return new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds()); }
                if (scope.value && scope.value.Value && typeof (scope.value.Value) === 'string')
                    scope.value.Value = convertDateToUTC(new Date(scope.value.Value));
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
            wrapper: ["fieldGroup"]
        });
    }]);
/* 
 * Field: Entity - Default
 * Also contains much business logic and the necessary controller
 * 
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", function(formlyConfigProvider) {

        formlyConfigProvider.setType({
            name: "entity-default",
            templateUrl: "fields/entity/entity-default.html",
            wrapper: ["eavLabel", "bootstrapHasError", "collapsible"],
            controller: "FieldTemplate-EntityCtrl"
        });

    }])
    .controller("FieldTemplate-EntityCtrl", ["$scope", "$http", "$filter", "$translate", "$modal", "appId", "eavAdminDialogs", "eavDefaultValueService", function ($scope, $http, $filter, $translate, $modal, appId, eavAdminDialogs, eavDefaultValueService) {
        if (!$scope.to.settings.merged)
            $scope.to.settings.merged = {};

        $scope.availableEntities = [];

        if ($scope.model[$scope.options.key] === undefined || $scope.model[$scope.options.key].Values[0].Value === "")
            $scope.model[$scope.options.key] = { Values: [{ Value: eavDefaultValueService($scope.options), Dimensions: {} }] };

        $scope.chosenEntities = $scope.model[$scope.options.key].Values[0].Value;

        $scope.addEntity = function() {
            if ($scope.selectedEntity === "new")
                $scope.openNewEntityDialog();
            else
                $scope.chosenEntities.push($scope.selectedEntity);
            $scope.selectedEntity = "";
        };

        $scope.createEntityAllowed = function() {
            return $scope.to.settings.merged.EntityType !== null && $scope.to.settings.merged.EntityType !== "";
        };

        $scope.openNewEntityDialog = function() {
            function reload(result) {
                if (result.data === null || result.data === undefined)
                    return;

                $scope.getAvailableEntities().then(function () {
                    $scope.chosenEntities.push(Object.keys(result.data)[0]);
                });
            }

            eavAdminDialogs.openItemNew($scope.to.settings.merged.EntityType, reload);

        };

        $scope.getAvailableEntities = function() {
            return $http({
                method: "GET",
                url: "eav/EntityPicker/getavailableentities",
                params: {
                    contentTypeName: $scope.to.settings.merged.EntityType,
                    appId: appId
                    // ToDo: dimensionId: $scope.configuration.DimensionId
                }
            }).then(function(data) {
                $scope.availableEntities = data.data;
            });
        };

        $scope.getEntityText = function (entityId) {
            if (entityId === null)
                return "empty slot"; // todo: i18n
            var entities = $filter("filter")($scope.availableEntities, { Value: entityId });
            return entities.length > 0 ? entities[0].Text : $translate.instant("FieldType.Entity.EntityNotFound"); 
        };

        // remove needs the index --> don't name "remove" - causes problems
        $scope.removeSlot = function remove(itemGuid, index) {
            $scope.chosenEntities.splice(index, 1);
        };

        // edit needs the Guid - the index isn't important
        $scope.edit = function (itemGuid, index) {
            if (itemGuid === null)
                return alert('no can do'); // todo: i18n
            var entities = $filter("filter")($scope.availableEntities, { Value: itemGuid });
            var id = entities[0].Id;

            eavAdminDialogs.openItemEditWithEntityId(id, $scope.getAvailableEntities);
        };

        // Initialize entities
        $scope.getAvailableEntities();

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
 * Field: String - Default
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function (formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-default",
            template: "<div><input class=\"form-control input-lg\" ng-if=\"!(options.templateOptions.settings.merged.RowCount > 1)\" ng-pattern=\"vm.regexPattern\" ng-model=\"value.Value\">" +
                "<textarea ng-if=\"options.templateOptions.settings.merged.RowCount > 1\" rows=\"{{options.templateOptions.settings.merged.RowCount}}\" class=\"form-control input-lg\" ng-model=\"value.Value\"></textarea></div>",
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
            template: "<select class=\"form-control input-lg\" ng-model=\"value.Value\"></select>",
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
/* global angular */
(function () {
    /* jshint laxbreak:true*/
    "use strict";

    var app = angular.module("eavEditEntity");

    // The controller for the main form directive
    app.controller("EditEntities", ["appId", "$http", "$scope", "entitiesSvc", "toastr", "saveToastr", "$translate", "debugState", "ctrlS", function editEntityCtrl(appId, $http, $scope, entitiesSvc, toastr, saveToastr, $translate, debugState, ctrlS) {

        var vm = this;
        vm.debug = debugState;
        vm.isWorking = 0;           // isWorking is > 0 when any $http request runs
        vm.registeredControls = []; // array of input-type controls used in these forms
        vm.items = null;            // array of items to edit
        vm.willPublish = false;     // default is won't publish, but will usually be overridden

        //#region activate / deactivate + bindings
        // the activate-command, to intialize everything. Must be called later, when all methods have been attached
        function activate() {
            // bind ctrl+S
            vm.saveShortcut = ctrlS(function () { vm.save(); });

            // load all data
            vm.loadAll();
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
        vm.loadAll = function() {
            entitiesSvc.getManyForEditing(appId, $scope.itemList)
                .then(function(result) {
                    vm.items = result.data;
                    angular.forEach(vm.items, function(v, i) {

                        // If the entity is null, it does not exist yet. Create a new one
                        if (!vm.items[i].Entity && !!vm.items[i].Header.ContentTypeName)
                            vm.items[i].Entity = entitiesSvc.newEntity(vm.items[i].Header);

                        vm.items[i].Entity = enhanceEntity(vm.items[i].Entity);

                        // set slot value - must be inverte for boolean-switch
                        var grp = vm.items[i].Header.Group;
                        vm.items[i].slotIsUsed = (grp === null
                            || grp.SlotIsEmpty !== true);
                    });
                    vm.willPublish = vm.items[0].Entity.IsPublished;
                });
        };

        vm.showFormErrors = function() {
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
            saveToastr(entitiesSvc.saveMany(appId, vm.items)).then(function (result) {
                $scope.state.setPristine();
                if (close)
                    vm.afterSaveEvent(result);
                vm.isWorking--;
            }, function errorWhileSaving(response) {
                vm.isWorking--;
            });
        };

        // things to do after saving
        vm.afterSaveEvent = $scope.afterSaveEvent;

        //#endregion

        //#region state check/set for valid/dirty/pristine
        // check if form is valid
        vm.isValid = function () {
            var valid = true;
            angular.forEach(vm.registeredControls, function (e, i) {
                if (!e.isValid())
                    valid = false;
            });
            return valid;
        };

        vm.formErrors = function () {
            var list = [];
            angular.forEach(vm.registeredControls, function (e, i) {
                if (!e.isValid())
                    list.push(e.error());
            });
            return list;
        };

        // check if dirty
        $scope.state.isDirty = function() {
            var dirty = false;
            angular.forEach(vm.registeredControls, function(e, i) {
                if (e.isDirty())
                    dirty = true;
            });
            return dirty;
        };

        // set to not-dirty (pristine)
        $scope.state.setPristine = function() {
            angular.forEach(vm.registeredControls, function(e, i) {
                e.setPristine();
            });
        };
        //#endregion

        // monitor for changes in publish-state and set it for all items being edited
        $scope.$watch('vm.willPublish', function (newValue, oldValue) {
            angular.forEach(vm.items, function (v, i) {
                vm.items[i].Entity.IsPublished = vm.willPublish;
            });
        });

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
                state: "="
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
	app.controller("EditEntityFormCtrl", ["appId", "$http", "$scope", "formlyConfig", "contentTypeFieldSvc", "$sce", "debugState", "customInputTypes", "eavConfig", function editEntityCtrl(appId, $http, $scope, formlyConfig, contentTypeFieldSvc, $sce, debugState, customInputTypes, eavConfig) {

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

	    vm.registerAllFieldsFromReturnedDefinition = function raffrd(result) {
	        var lastGroupHeadingId = 0;
	        angular.forEach(result.data, function (e, i) {

	            if (e.Metadata.All === undefined)
	                e.Metadata.All = {};

	            var fieldType = e.InputType;

	            // always remember the last heading so all the following fields know to look there for collapse-setting
	            var isFieldHeading = (fieldType === "empty-default");
	            if (isFieldHeading)
	                lastGroupHeadingId = i;

	            var nextField = {
	                key: e.StaticName,
	                type: fieldType,
	                templateOptions: {
	                    required: !!e.Metadata.All.Required,
	                    label: e.Metadata.All.Name === undefined ? e.StaticName : e.Metadata.All.Name,
	                    description: $sce.trustAsHtml(e.Metadata.All.Notes),
	                    settings: e.Metadata,
	                    header: $scope.header,
	                    canCollapse: lastGroupHeadingId > 0 && !isFieldHeading,
	                    fieldGroup: vm.formFields[lastGroupHeadingId],
	                    disabled: e.Metadata.All.Disabled,
	                    langReadOnly: false // Will be set by the language directive to override the disabled state
	                },
	                className: "type-" + e.Type.toLowerCase() + " input-" + fieldType + " field-" + e.StaticName.toLowerCase(),
	                hide: (e.Metadata.All.VisibleInEditUI === false ? !debugState.on : false),
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
	    };


	    // Load existing entity if defined
		if (vm.entity !== null)
		    loadContentType();


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
	app.controller("EditEntityWrapperCtrl", ["$q", "$http", "$scope", "items", "$modalInstance", "$window", "$translate", function editEntityCtrl($q, $http, $scope, items, $modalInstance, $window, $translate) {

	    var vm = this;
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
		    $modalInstance.close(result);
		};


	    vm.maybeLeave = function maybeLeave(e) {
	        var unsavedChangesText = $translate.instant("Errors.UnsavedChanges");
	        if (vm.state.isDirty() && !confirm(unsavedChangesText + " " + $translate.instant("Message.ExitOk")))
	            e.preventDefault();
	    };

	    $scope.$on('modal.closing', vm.maybeLeave);
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
angular.module('eavEditTemplates', []).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('fields/boolean/boolean-default.html',
    "<div class=\"checkbox checkbox-labeled\"><switch class=\"tosic-green pull-left\" ng-model=value.Value></switch><div ng-include=\"'wrappers/eav-label.html'\"></div></div>"
  );


  $templateCache.put('fields/custom/custom-default.html',
    "<div class=\"alert alert-danger\">ERROR - This is a custom field, you shouldn't see this. You only see this because the custom-dialog is missing.</div><input class=\"form-control input-lg\" ng-pattern=vm.regexPattern ng-model=value.Value>"
  );


  $templateCache.put('fields/empty/empty-default.html',
    "<span></span>"
  );


  $templateCache.put('fields/entity/entity-default.html',
    "<div class=eav-entityselect><div ui-tree=options data-empty-placeholder-enabled=false><ol ui-tree-nodes ng-model=chosenEntities><li ng-repeat=\"item in chosenEntities track by $index\" ui-tree-node class=eav-entityselect-item><div ui-tree-handle><i icon=move title=\"{{ 'FieldType.Entity.DragMove' | translate }}\" class=\"pull-left eav-entityselect-sort\" ng-show=to.settings.Entity.AllowMultiValue></i> <span title=\"{{getEntityText(item) + ' (' + item + ')'}}\">{{getEntityText(item)}}</span> <span class=eav-entityselect-item-actions><span data-nodrag title=\"{{ 'FieldType.Entity.Edit' | translate }}\" ng-click=\"edit(item, index)\"><i icon=pencil></i></span> <span data-nodrag title=\"{{ 'FieldType.Entity.Remove' | translate }}\" ng-click=\"removeSlot(item, $index)\" class=eav-entityselect-item-remove><i icon=minus></i></span></span></div></li></ol></div><select class=\"eav-entityselect-selector form-control input-lg\" ng-model=selectedEntity ng-change=addEntity() ng-show=\"to.settings.merged.AllowMultiValue || chosenEntities.length < 1\"><option value=\"\" translate=FieldType.Entity.Choose></option><option value=new ng-if=createEntityAllowed() translate=FieldType.Entity.New></option><option ng-repeat=\"item in availableEntities\" ng-disabled=\"chosenEntities.indexOf(item.Value) != -1\" value={{item.Value}}>{{item.Text}}</option></select></div>"
  );


  $templateCache.put('form/edit-many-entities.html',
    "<div ng-if=\"vm.items != null\" ng-click=vm.debug.autoEnableAsNeeded($event)><eav-language-switcher is-disabled=!vm.isValid()></eav-language-switcher><div ng-repeat=\"p in vm.items\" class=group-entity><h3 class=clickable ng-click=\"p.collapse = !p.collapse\">{{p.Header.Title ? p.Header.Title : 'EditEntity.DefaultTitle' | translate }}&nbsp; <span ng-if=p.Header.Group.SlotCanBeEmpty ng-click=vm.toggleSlotIsEmpty(p) stop-event=click><switch ng-model=p.slotIsUsed class=tosic-blue style=\"top: 6px\" tooltip=\"{{'EditEntity.SlotUsed' + p.slotIsUsed | translate}}\"></switch></span> <span class=\"pull-right clickable\" style=\"font-size: smaller\"><span class=\"low-priority collapse-entity-button\" ng-if=p.collapse icon=plus-sign></span> <span class=collapse-entity-button ng-if=!p.collapse icon=minus-sign></span></span></h3><eav-edit-entity-form entity=p.Entity header=p.Header register-edit-control=vm.registerEditControl ng-hide=p.collapse></eav-edit-entity-form></div><div><button ng-class=\"{ 'disabled': vm.isValid() || !vm.isWorking}\" ng-click=vm.save(true) type=button class=\"btn btn-primary btn-lg submit-button\"><span icon=ok tooltip=\"{{ 'Button.Save' | translate }}\"></span> &nbsp;<span translate=Button.Save></span></button> &nbsp; <button ng-class=\"{ 'disabled': vm.isValid() || !vm.isWorking}\" class=\"btn btn-default btn-lg btn-square\" type=button ng-click=vm.save(false)><span icon=check tooltip=\"{{ 'Button.SaveAndKeepOpen' | translate }}\"></span></button> &nbsp;<switch ng-model=vm.willPublish class=tosic-blue style=\"top: 13px\"></switch>&nbsp; <span ng-click=\"vm.willPublish = !vm.willPublish;\" class=save-published-icon><i ng-if=vm.willPublish icon=eye-open tooltip=\"{{ 'Status.Published' | translate }} - {{ 'Message.WillPublish' | translate }}\"></i> <i ng-if=!vm.willPublish icon=eye-close tooltip=\"{{ 'Status.Unpublished' | translate }} - {{ 'Message.WontPublish' | translate }}\"></i></span> <span ng-if=vm.debug.on><button tooltip=debug icon=zoom-in class=btn ng-click=\"vm.showDebugItems = !vm.showDebugItems\"></button></span><show-debug-availability class=pull-right style=\"margin-top: 20px\"></show-debug-availability></div><div ng-if=\"vm.debug.on && vm.showDebugItems\"><pre>{{ vm.items | json }}</pre></div></div>"
  );


  $templateCache.put('form/edit-single-entity.html',
    "<div ng-show=vm.editInDefaultLanguageFirst() translate=Message.PleaseCreateDefLang></div><div ng-show=!vm.editInDefaultLanguageFirst()><formly-form ng-if=\"vm.formFields && vm.formFields.length\" ng-submit=vm.onSubmit() form=vm.form model=vm.entity.Attributes fields=vm.formFields></formly-form></div>"
  );


  $templateCache.put('form/main-form.html',
    "<div class=modal-body><span class=pull-right><span style=\"display: inline-block; position: relative; left:15px\"><button class=\"btn btn-default btn-square btn-subtle\" type=button ng-click=vm.close()><i icon=remove></i></button></span></span><eav-edit-entities item-list=vm.itemList after-save-event=vm.afterSave state=vm.state></eav-edit-entities></div>"
  );


  $templateCache.put('localization/formly-localization-wrapper.html',
    "<eav-localization-scope-control></eav-localization-scope-control><div ng-if=!!value><eav-localization-menu field-model=model[options.key] options=options value=value index=index></eav-localization-menu><formly-transclude></formly-transclude></div><p class=bg-info style=padding:12px ng-if=!value translate=LangWrapper.CreateValueInDefFirst translate-values=\"{ fieldname: '{{to.label}}' }\">Please... <i>'{{to.label}}'</i> in the def...</p>"
  );


  $templateCache.put('localization/language-switcher.html',
    "<tabset><tab ng-repeat=\"l in languages.languages\" heading=\"{{ l.name.substring(0, l.name.indexOf('(') > 0 ? l.name.indexOf('(') - 1 : 100 ) }}\" ng-click=\"!isDisabled ? languages.currentLanguage = l.key : false;\" disable=isDisabled active=\"languages.currentLanguage == l.key\" tooltip={{l.name}}></tab></tabset>"
  );


  $templateCache.put('localization/localization-menu.html',
    "<div dropdown is-open=status.isopen class=eav-localization style=\"z-index:{{1000 - index}}\"><a class=eav-localization-lock ng-click=vm.actions.toggleTranslate(); ng-if=vm.isDefaultLanguage() title={{vm.tooltip()}} ng-class=\"{ 'eav-localization-lock-open': !options.templateOptions.disabled }\" dropdown-toggle>{{vm.infoMessage()}} <i class=\"glyphicon glyphicon-globe\"></i></a><ul class=\"dropdown-menu multi-level pull-right eav-localization-dropdown\" role=menu aria-labelledby=single-button><li role=menuitem><a ng-disabled=vm.enableTranslate() ng-click=vm.actions.translate() translate=LangMenu.Unlink></a></li><li role=menuitem><a ng-click=vm.actions.linkDefault() translate=LangMenu.LinkDefault></a></li><li role=menuitem class=dropdown-submenu><a href=# translate=LangMenu.GoogleTranslate></a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" class=disabled role=menuitem><a ng-click=vm.actions.autoTranslate(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li><li role=menuitem class=dropdown-submenu><a href=# translate=LangMenu.Copy></a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: options.templateOptions.disabled || !vm.hasLanguage(language.key) }\" role=menuitem><a ng-click=vm.actions.copyFrom(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li><li role=menuitem class=dropdown-submenu><a href=# translate=LangMenu.Use></a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: !vm.hasLanguage(language.key) }\" role=menuitem><a ng-click=vm.actions.useFrom(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li><li role=menuitem class=dropdown-submenu><a href=# translate=LangMenu.Share></a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: !vm.hasLanguage(language.key) }\" role=menuitem><a ng-click=vm.actions.shareFrom(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li></ul></div>"
  );


  $templateCache.put('wrappers/collapsible.html',
    "<div ng-show=!to.collapse class=group-field-set><formly-transclude></formly-transclude></div>"
  );


  $templateCache.put('wrappers/disablevisually.html',
    "<div visually-disabled={{to.disabled}}><formly-transclude></formly-transclude></div>"
  );


  $templateCache.put('wrappers/eav-label.html',
    "<div><label for={{id}} class=\"control-label {{to.labelSrOnly ? 'sr-only' : ''}} {{to.type}}\" ng-if=to.label>{{to.label}} {{to.required ? '*' : ''}} <a tabindex=-1 ng-click=\"to.showDescription = !to.showDescription\" href=javascript:void(0); ng-if=\"to.description && to.description != ''\"><i icon=info-sign class=low-priority></i></a></label><p ng-if=to.showDescription class=bg-info style=\"padding: 5px\" ng-bind-html=to.description></p><formly-transclude></formly-transclude></div>"
  );


  $templateCache.put('wrappers/field-group.html',
    "<div><h4 class=clickable ng-click=\"to.collapseGroup = !to.collapseGroup\">{{to.label}} <span class=\"pull-right btn-sm\"><span ng-if=to.collapseGroup class=\"low-priority collapse-fieldgroup-button\" icon=plus-sign></span> <span ng-if=!to.collapseGroup class=\"low-priority collapse-fieldgroup-button\" icon=minus-sign></span></span></h4><div ng-if=!to.collapseGroup style=\"padding: 5px\" ng-bind-html=to.description></div><formly-transclude></formly-transclude></div>"
  );

}]);


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
					console.log("switched language from " + oldValue + " to " + newValue);
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

	eavLocalization.directive("eavLocalizationMenu", function() {
		return {
			restrict: "E",
			scope: {
				fieldModel: "=fieldModel",
				options: "=options",
				value: "=value",
                index: "=index"
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
				    translate: function trnslt() {
				        if (vm.enableTranslate()) {
				            vm.fieldModel.removeLanguage(languages.currentLanguage);
				            vm.fieldModel.addVs($scope.value.Value, languages.currentLanguage, false);
				        }
				    },
				    linkDefault: function linkDefault() {
				        vm.fieldModel.removeLanguage(languages.currentLanguage);
				    },
				    autoTranslate: function(languageKey) {
				        alert(translate("LangMenu.NotImplemented"));
				    },
				    copyFrom: function (languageKey) {
				        if ($scope.options.templateOptions.disabled)
				            alert(translate("LangMenu.CopyNotPossible"));
				        var value = vm.fieldModel.getVsWithLanguage(languageKey).Value;
				        $scope.value.Value = value;
				    },
				    useFrom: function (languageKey) {
				        vm.fieldModel.removeLanguage(languages.currentLanguage);
				        var vs = vm.fieldModel.getVsWithLanguage(languageKey);
				        vs.setLanguage(languages.currentLanguage, true);
				    },
				    shareFrom: function (languageKey) {
				        vm.fieldModel.removeLanguage(languages.currentLanguage);
				        var vs = vm.fieldModel.getVsWithLanguage(languageKey);
				        vs.setLanguage(languages.currentLanguage, false);
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
				    return d !== undefined && d !== null ? d : []; 
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
        .factory("entitiesSvc", ["$http", "appId", "toastrWithHttpErrorHandling", "promiseToastr", function ($http, appId, toastrWithHttpErrorHandling, promiseToastr) {
            var svc = {
                toastr: toastrWithHttpErrorHandling
            };

            svc.getManyForEditing = function(appId, items) {
                return $http.post("eav/entities/getmanyforediting", items, { params: { appId: appId } });
            };

            svc.saveMany = function(appId, items) {
                // first clean up unnecessary nodes - just to make sure we don't miss-read the JSONs transferred
                var removeTempValue = function(value, key) { delete value._currentValue; };
                var itmCopy = angular.copy(items);
                for (var ei = 0; ei < itmCopy.length; ei++)
                    angular.forEach(itmCopy[ei].Entity.Attributes, removeTempValue);

                return $http.post("eav/entities/savemany", itmCopy, { params: { appId: appId } }).then(function (serverKeys) {
                    var syncUpdatedKeys = function(value, key) {
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

            svc.delete = function del(type, id) {
                console.log("try to delete");

                var delPromise = $http.get("eav/entities/delete", {
                    ignoreErrors: true,
                    params: {
                        'contentType': type,
                        'id': id,
                        'appId': appId
                    }
                });
                return promiseToastr(delPromise, "Message.Deleting", "Message.Ok", "Message.Error");
            };

            svc.newEntity = function(header) {
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