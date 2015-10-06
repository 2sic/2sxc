/* Main object with dependencies, used in wrappers and other places */
(function () {
	"use strict";

	angular.module("eavEditEntity", [
        "formly",
        "ui.bootstrap",
        "eavFieldTemplates",
        "EavServices",
        "eavEditTemplates"
	]);


})();
// this changes JSON-serialization for dates, 
// because we usually want the time to be the same across time zones and NOT keeping the same moment
Date.prototype.toJSON = function() {
    var x = new Date(this);
    x.setHours(x.getHours() - x.getTimezoneOffset() / 60);
    return x.toISOString();
};

(function() {
	"use strict";

	/* This app registers all field templates for EAV in the angularjs eavFieldTemplates app */

	var eavFieldTemplates = angular.module("eavFieldTemplates", ["formly", "formlyBootstrap", "ui.bootstrap", "eavLocalization", "eavEditTemplates"])
        .config(["formlyConfigProvider", function (formlyConfigProvider) {

	    formlyConfigProvider.setType({
	        name: "string-default",
	        template: "<input class=\"form-control\" ng-model=\"value.Value\">",
	        wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"]
	    });

	    formlyConfigProvider.setType({ name: "string-dropdown",
	        template: "<select class=\"form-control\" ng-model=\"value.Value\"></select>",
	        wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
	        defaultOptions: function defaultOptions(options) {
				
	            // DropDown field: Convert string configuration for dropdown values to object, which will be bound to the select
	            if (!options.templateOptions.options && options.templateOptions.settings.String.DropdownValues) {
	                var o = options.templateOptions.settings.String.DropdownValues;
	                o = o.replace(/\r/g, "").split("\n");
	                o = o.map(function (e, i) {
	                    var s = e.split(":");
	                    return {
	                        name: s[0],
	                        value: s[1] ? s[1] : s[0]
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

	    formlyConfigProvider.setType({
	        name: "string-textarea",
	        template: "<textarea class=\"form-control\" ng-model=\"value.Value\"></textarea>",
	        wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
	        defaultOptions: {
	            ngModelAttrs: {
	                '{{to.settings.String.RowCount}}': { value: "rows" },
	                cols: { attribute: "cols" }
	            }
	        }
	    });

	    formlyConfigProvider.setType({
	        name: "number-default",
	        template: "<input type=\"number\" class=\"form-control\" ng-model=\"value.Value\">{{vm.isGoogleMap}}",
	        wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
	        defaultOptions: {
	            ngModelAttrs: {
	                '{{to.settings.Number.Min}}': { value: "min" },
	                '{{to.settings.Number.Max}}': { value: "max" },
	                '{{to.settings.Number.Decimals ? "^[0-9]+(\.[0-9]{1," + to.settings.Number.Decimals + "})?$" : null}}': { value: "pattern" }
	            }
	        },
	        controller: "FieldTemplate-NumberCtrl as vm"
	    });

	    formlyConfigProvider.setType({
	        name: "boolean-default",
	        template: "<div class=\"checkbox\">\n\t<label>\n\t\t<input type=\"checkbox\"\n           class=\"formly-field-checkbox\"\n\t\t       ng-model=\"value.Value\">\n\t\t{{to.label}}\n\t\t{{to.required ? '*' : ''}}\n\t</label>\n</div>\n",
	        wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"]
	    });

	    formlyConfigProvider.setType({
	        name: "datetime-default",
	        wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
	        template: "<div>" +
                "<div class=\"input-group\">" +
                    "<div class=\"input-group-addon\" style=\"cursor:pointer;\" ng-click=\"to.isOpen = true;\">" +
                        "<i class=\"glyphicon glyphicon-calendar\"></i>" +
                    "</div>" +
                    "<input class=\"form-control\" ng-model=\"value.Value\" is-open=\"to.isOpen\" datepicker-options=\"to.datepickerOptions\" datepicker-popup />" +
				    "<timepicker ng-show=\"to.settings.DateTime.UseTimePicker\" ng-model=\"value.Value\" show-meridian=\"ismeridian\"></timepicker>" +
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
                if (scope.value && scope.value.Value && typeof(scope.value.Value) === 'string')
                    scope.value.Value = convertDateToUTC(new Date(scope.value.Value));
            }
	    });

		formlyConfigProvider.setType({
		    name: "entity-default",
		    templateUrl: "fields/templates/entity-default.html",
		    wrapper: ["eavLabel", "bootstrapHasError"],
		    controller: "FieldTemplate-EntityCtrl"
		});

	    formlyConfigProvider.setWrapper({
	        name: 'eavLabel',
            templateUrl: "fields/eav-label.html"
	    });
	}]);

	eavFieldTemplates.controller("FieldTemplate-NumberCtrl", function () {
		var vm = this;
		// ToDo: Implement Google Map
	});


	eavFieldTemplates.controller("FieldTemplate-EntityCtrl", ["$scope", "$http", "$filter", "$modal", "appId", function ($scope, $http, $filter, $modal, appId) {

	    if (!$scope.to.settings.Entity)
	        $scope.to.settings.Entity = {};

	    $scope.availableEntities = [];

	    if ($scope.model[$scope.options.key] === undefined || $scope.model[$scope.options.key].Values[0].Value === "")
	        $scope.model[$scope.options.key] = { Values: [{ Value: [], Dimensions: {} }] };

	    $scope.chosenEntities = $scope.model[$scope.options.key].Values[0].Value;

	    $scope.addEntity = function () {
	        if ($scope.selectedEntity == "new")
	            $scope.openNewEntityDialog();
	        else
	            $scope.chosenEntities.push($scope.selectedEntity);
	        $scope.selectedEntity = "";
	    };

	    $scope.createEntityAllowed = function () {
	        return $scope.to.settings.Entity.EntityType !== null && $scope.to.settings.Entity.EntityType !== "";
	    };

	    $scope.openNewEntityDialog = function () {

	        var modalInstance = $modal.open({
	            template: "<div style=\"padding:20px;\"><edit-content-group edit=\"vm.edit\"></edit-content-group></div>",
	            controller: ["entityType", function (entityType) {
	                var vm = this;
	                vm.edit = { contentTypeName: entityType };
	            }],
	            controllerAs: "vm",
	            resolve: {
	                entityType: function () {
	                    return $scope.to.settings.Entity.EntityType;
	                }
	            }
	        });

	        modalInstance.result.then(function () {
	            $scope.getAvailableEntities();
	        });

	    };

	    $scope.getAvailableEntities = function () {
	        $http({
	            method: "GET",
	            url: "eav/EntityPicker/getavailableentities",
	            params: {
	                contentTypeName: $scope.to.settings.Entity.EntityType,
	                appId: appId
	                // ToDo: dimensionId: $scope.configuration.DimensionId
	            }
	        }).then(function (data) {
	            $scope.availableEntities = data.data;
	        });
	    };

	    $scope.getEntityText = function (entityId) {
	        var entities = $filter("filter")($scope.availableEntities, { Value: entityId });
	        return entities.length > 0 ? entities[0].Text : "(Entity not found)";
	    };

	    $scope.remove = function (item) {
	        var index = $scope.chosenEntities.indexOf(item);
	        $scope.chosenEntities.splice(index, 1);
	    };

	    // Initialize entities
	    $scope.getAvailableEntities();

	}]);

})();
/* global angular */
(function () {
    "use strict";

    var app = angular.module("eavEditEntity");

    // The controller for the main form directive
    app.controller("EditEntities", ["appId", "$http", "$scope", "entitiesSvc", "uiNotification", "debugState", function editEntityCtrl(appId, $http, $scope, entitiesSvc, uiNotification, debugState) {

        var vm = this;
        vm.debug = debugState;
        
        vm.registeredControls = [];
        vm.registerEditControl = function (control) {
            vm.registeredControls.push(control);
        };

        vm.afterSaveEvent = $scope.afterSaveEvent;

        vm.isValid = function () {
            var valid = true;
            angular.forEach(vm.registeredControls, function (e, i) {
                if (!e.isValid())
                    valid = false;
            });
            return valid;
        };

        $scope.state.isDirty = function() {
            var dirty = false;
            angular.forEach(vm.registeredControls, function(e, i) {
                if (e.isDirty())
                    dirty = true;
            });
            return dirty;
        };

        $scope.state.setPristine = function() {
            angular.forEach(vm.registeredControls, function(e, i) {
                e.setPristine();
            });
        };

        vm.save = function () {
            entitiesSvc.saveMany(appId, vm.items).then(function(result) {
                $scope.state.setPristine();
                vm.afterSaveEvent(result);
            });
        };

        // todo: get toaster to work
        // todo: translate
        vm.saveAndKeepOpen = function () {
            uiNotification.note("Saving", "", true);
            entitiesSvc.saveMany(appId, vm.items).then(function() {
                $scope.state.setPristine();
                uiNotification.note("Saved", "", true);
            });
        };
        vm.items = null;

        entitiesSvc.getManyForEditing(appId, $scope.itemList)
            .then(function (result) {
                vm.items = result.data;
                angular.forEach(vm.items, function (v, i) {

                    // If the entity is null, it does not exist yet. Create a new one
                    if (!vm.items[i].Entity && !!vm.items[i].Header.ContentTypeName)
                        vm.items[i].Entity = entitiesSvc.newEntity(vm.items[i].Header.ContentTypeName);

                    else {
                        entitiesSvc.ensureGuid(vm.items[i]);
                    }

                    vm.items[i].Entity = enhanceEntity(vm.items[i].Entity);
                });
                vm.willPublish = vm.items[0].Entity.IsPublished;
            });

        vm.willPublish = false;

        vm.togglePublish = function() {
            vm.willPublish = !vm.willPublish;
            angular.forEach(vm.items, function(v, i) {
                vm.items[i].Entity.IsPublished = vm.willPublish;
            });
        };

        vm.toggleSlotIsEmpty = function (item) {
            if (!item.Header.Group)
                item.Header.Group = {};
            item.Header.Group.SlotIsEmpty = !item.Header.Group.SlotIsEmpty;
        };

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
	"use strict";

	var app = angular.module("eavEditEntity"); 

	// The controller for the main form directive
    app.controller("EditEntityFormCtrl", ["appId", "$http", "$scope", "formlyConfig", "contentTypeFieldSvc", "$sce", "debugState", function editEntityCtrl(appId, $http, $scope, formlyConfig, contentTypeFieldSvc, $sce, debugState) {

		var vm = this;
		vm.editInDefaultLanguageFirst = function () {
			return false; // ToDo: Use correct language information, e.g. eavLanguageService.currentLanguage != eavLanguageService.defaultLanguage && !$scope.entityId;
		};

		// The control object is available outside the directive
		// Place functions here that should be available from the parent of the directive
		vm.control = {
		    isValid: function () { return vm.formFields.length === 0 || vm.form && vm.form.$valid; },
		    isDirty: function () { return (vm.form && vm.form.$dirty); },
		    setPristine: function () { if(vm.form) vm.form.$setPristine(); }
		};

		// Register this control in the parent control
		if($scope.registerEditControl)
			$scope.registerEditControl(vm.control);

		vm.model = null;
		vm.entity = $scope.entity;

		vm.formFields = [];


		var loadContentType = function () {

		    contentTypeFieldSvc(appId, { StaticName: vm.entity.Type.StaticName }).getFields()
			.then(function (result) {
			    vm.debug = result;

			    // Transform EAV content type configuration to formFields (formly configuration)
			    angular.forEach(result.data, function (e, i) {

			        if (e.Metadata.All === undefined)
			            e.Metadata.All = {};

			        vm.formFields.push({
			            key: e.StaticName,
			            type: getType(e),
			            templateOptions: {
			                required: !!e.Metadata.All.Required,
			                label: e.Metadata.All.Name === undefined ? e.StaticName : e.Metadata.All.Name,
			                description: $sce.trustAsHtml(e.Metadata.All.Notes),
			                settings: e.Metadata,
			                header: $scope.header,
                            langReadOnly: false // Will be set by the language directive to override the disabled state
			            },
			            hide: (e.Metadata.All.VisibleInEditUI === false ? !debugState.on : false),
			            expressionProperties: { // Needed for dynamic update of the disabled property
			                'templateOptions.disabled': 'options.templateOptions.disabled'
			            },
			            watcher: {
			                expression: function (field, scope, stop) {
			                    return (field.templateOptions.header.Group && field.templateOptions.header.Group.SlotIsEmpty) || field.templateOptions.langReadOnly;
			                },
			                listener: function(field, newValue, oldValue, scope, stopWatching) {
			                    field.templateOptions.disabled = newValue;
			                }
			            }
			        });
			    });

		    });
		};

	    // Load existing entity if defined
		if (vm.entity !== null)
		    loadContentType();


		// Returns the field type for an attribute configuration
		var getType = function(attributeConfiguration) {
			var e = attributeConfiguration;
			var type = e.Type.toLowerCase();
			var subType = e.Metadata.String ? e.Metadata.String.InputType : null;

			subType = subType ? subType.toLowerCase() : null;

			// Special case: override subtype for string-textarea
			if (type === "string" && e.Metadata.String && e.Metadata.String.RowCount > 1)
				subType = "textarea";

			// Use subtype 'default' if none is specified - or type does not exist
			if (!subType || !formlyConfig.getType(type + "-" + subType))
				subType = "default";

			return (type + "-" + subType);
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
angular.module('eavEditTemplates',[]).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('fields/eav-label.html',
    "<div><label for={{id}} class=\"control-label {{to.labelSrOnly ? 'sr-only' : ''}}\" ng-if=to.label>{{to.label}} {{to.required ? '*' : ''}} <a tabindex=-1 ng-click=\"to.showDescription = !to.showDescription\" href=javascript:void(0); ng-if=\"to.description && to.description != ''\" title={{to.description}}><i icon=info-sign></i></a></label><p ng-if=to.showDescription class=bg-info style=\"padding: 5px\" ng-bind-html=to.description></p><formly-transclude></formly-transclude></div>"
  );


  $templateCache.put('fields/templates/entity-default.html',
    "<div class=eav-entityselect><div ui-tree=options data-empty-place-holder-enabled=false><ol ui-tree-nodes ng-model=chosenEntities><li ng-repeat=\"item in chosenEntities\" ui-tree-node class=eav-entityselect-item><div ui-tree-handle><span title=\"{{getEntityText(item) + ' (' + item + ')'}}\">{{getEntityText(item)}}</span> <a data-nodrag title=\"Remove this item\" ng-click=remove(this) class=eav-entityselect-item-remove>[remove]</a></div></li></ol></div><select class=\"eav-entityselect-selector form-control\" ng-model=selectedEntity ng-change=addEntity() ng-show=\"to.settings.Entity.AllowMultiValue || chosenEntities.length < 1\"><option value=\"\">-- choose --</option><option value=new ng-if=createEntityAllowed()>-- new --</option><option ng-repeat=\"item in availableEntities\" ng-disabled=\"chosenEntities.indexOf(item.Value) != -1\" value={{item.Value}}>{{item.Text}}</option></select></div>"
  );


  $templateCache.put('form/edit-many-entities.html',
    "<div ng-if=\"vm.items != null\" ng-click=vm.debug.autoEnableAsNeeded($event)><eav-language-switcher></eav-language-switcher><div ng-repeat=\"p in vm.items\"><h4>{{p.Header.Title ? p.Header.Title : 'Edit'}} <span ng-if=p.Header.Group.SlotCanBeEmpty><span ng-if=p.Header.Group.SlotIsEmpty icon=ban-circle ng-click=vm.toggleSlotIsEmpty(p) tooltip=\"this item is locked and will stay empty/default. The values are shown for your convenience. Click here to unlock if needed.\"></span> <span ng-if=!p.Header.Group.SlotIsEmpty icon=ok-circle ng-click=vm.toggleSlotIsEmpty(p) tooltip=\"this item is open for editing. Click here to lock / remove it and revert to default.\"></span></span> <span class=\"pull-right btn-sm\" ng-click=\"p.collapse = !p.collapse\"><span ng-if=p.collapse icon=plus-sign></span> <span ng-if=!p.collapse icon=minus-sign></span></span></h4><eav-edit-entity-form entity=p.Entity header=p.Header register-edit-control=vm.registerEditControl ng-hide=p.collapse></eav-edit-entity-form></div><button ng-disabled=!vm.isValid() ng-click=vm.save() class=\"btn btn-primary submit-button\"><span icon=ok tooltip=\"{{ 'Button.Save' | translate }}\"></span></button> <button class=btn ng-click=vm.saveAndKeepOpen()><span icon=check tooltip=\"{{ 'Button.SaveAndKeepOpen' | translate }}\"></span></button> <span ng-if=vm.willPublish icon=eye-open tooltip=\"{{ 'Status.Published' | translate }} - {{ 'Message.WillPublish' | translate }}\" ng-click=vm.togglePublish()></span> <span ng-if=!vm.willPublish icon=eye-close tooltip=\"{{ 'Status.Unpublished' | translate }} - {{ 'Message.WontPublish' | translate }}\" ng-click=vm.togglePublish()></span> <span ng-if=vm.debug.on><button tooltip=debug icon=zoom-in class=btn ng-click=\"vm.showDebugItems = !vm.showDebugItems\"></button></span> <span class=pull-right ng-if=false>todo: show more buttons... <button class=btn><span icon=option-horizontal tooltip=\"{{ 'Button.MoreOptions' | translate }}\"></span></button></span><div ng-if=\"vm.debug.on && vm.showDebugItems\"><pre>{{ vm.items | json }}</pre></div></div>"
  );


  $templateCache.put('form/edit-single-entity.html',
    "<div ng-show=vm.editInDefaultLanguageFirst()>Please edit this in the default language first.</div><div ng-show=!vm.editInDefaultLanguageFirst()><formly-form ng-if=\"vm.formFields && vm.formFields.length\" ng-submit=vm.onSubmit() form=vm.form model=vm.entity.Attributes fields=vm.formFields></formly-form></div>"
  );


  $templateCache.put('localization/formly-localization-wrapper.html',
    "<eav-localization-scope-control></eav-localization-scope-control><div ng-if=!!value><eav-localization-menu field-model=model[options.key] options=options value=value></eav-localization-menu><formly-transclude></formly-transclude></div><p class=bg-info style=padding:12px ng-if=!value>Please create the value for <i>'{{to.label}}'</i> in the default language before translating it.</p>"
  );


  $templateCache.put('localization/language-switcher.html',
    "<ul class=\"nav nav-pills\" style=\"margin-left:0; margin-bottom: 15px\"><li ng-repeat=\"l in languages.languages\" ng-class=\"{ active: languages.currentLanguage == l.key }\"><a ng-click=\"languages.currentLanguage = l.key;\" href=javascript:void(0);>{{l.name}}</a></li></ul>"
  );


  $templateCache.put('localization/localization-menu.html',
    "<div dropdown is-open=status.isopen class=eav-localization><a class=eav-localization-lock ng-if=vm.isDefaultLanguage() title={{vm.tooltip()}} ng-class=\"{ 'eav-localization-lock-open': !options.templateOptions.disabled }\" dropdown-toggle href=javascript:void(0)>{{vm.infoMessage()}} <i class=\"glyphicon glyphicon-globe\"></i></a><ul class=\"dropdown-menu multi-level pull-right eav-localization-dropdown\" role=menu aria-labelledby=single-button><li role=menuitem><a ng-disabled=vm.enableTranslate() ng-click=vm.actions.translate()>Translate (unlink)</a></li><li role=menuitem><a ng-click=vm.actions.linkDefault()>Use default</a></li><li role=menuitem class=dropdown-submenu><a href=#>Google-Translate from</a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" class=disabled role=menuitem><a ng-click=vm.actions.autoTranslate(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li><li role=menuitem class=dropdown-submenu><a href=#>Copy from</a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: options.templateOptions.disabled || !vm.hasLanguage(language.key) }\" role=menuitem><a ng-click=vm.actions.copyFrom(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li><li role=menuitem class=dropdown-submenu><a href=#>Use from</a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: !vm.hasLanguage(language.key) }\" role=menuitem><a ng-click=vm.actions.useFrom(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li><li role=menuitem class=dropdown-submenu><a href=#>Share from</a><ul class=dropdown-menu><li ng-repeat=\"language in vm.languages.languages\" ng-class=\"{ disabled: !vm.hasLanguage(language.key) }\" role=menuitem><a ng-click=vm.actions.shareFrom(language.key) title={{language.name}} href=#>{{language.key}}</a></li></ul></li></ul></div>"
  );


  $templateCache.put('wrappers/edit-entity-wrapper.html',
    "<div class=modal-header><button class=\"btn pull-right\" type=button icon=remove ng-click=vm.close()></button><h3 class=modal-title translate=EditEntity.Title></h3></div><div class=modal-body><eav-edit-entities item-list=vm.itemList after-save-event=vm.afterSave state=vm.state></eav-edit-entities></div>"
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
			}]
		};
	});

	eavLocalization.directive("eavLocalizationScopeControl", function () {
		return {
			restrict: "E",
			transclude: true,
			template: "",
			link: function (scope, element, attrs) {
			},
			controller: ["$scope", "$filter", "eavDefaultValueService", "languages", function ($scope, $filter, eavDefaultValueService, languages) { // Can't use controllerAs because of transcluded scope

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
					        fieldModel.addVs(defaultValue, langConf.currentLanguage); // Assign default language dimension
					    }
					    else { // There are no values - value must be edited in default language first
					        return;
					    }
					}


                    // todo: discuss w/2rm 2dm changed this 2015-10-05 - I think the false was wrong
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
							throw "Default language value not found, but found multiple values - can't handle editing for " + $scope.options.key;
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
                value: "=value"
			},
			templateUrl: "localization/localization-menu.html",
			link: function (scope, element, attrs) { },
			controllerAs: "vm",
			controller: ["$scope", "languages", function ($scope, languages) {
				var vm = this;
				vm.fieldModel = $scope.fieldModel;
				vm.languages = languages;
				vm.hasLanguage = function(languageKey) {
				    return vm.fieldModel.getVsWithLanguage(languageKey) !== null;
				};

				vm.isDefaultLanguage = function () { return languages.currentLanguage != languages.defaultLanguage; };
				vm.enableTranslate = function () { return true; };

				vm.infoMessage = function () {
				    if (Object.keys($scope.value.Dimensions).length === 1 && $scope.value.Dimensions[languages.defaultLanguage] === false)
				        return "auto (default)";
				    if (Object.keys($scope.value.Dimensions).length === 1 && $scope.value.Dimensions[languages.currentLanguage] === false)
				        return "";
				    return "in " + Object.keys($scope.value.Dimensions).join(", ");
				};

				vm.tooltip = function () {
				    var editableIn = [];
				    var usedIn = [];
				    angular.forEach($scope.value.Dimensions, function (value, key) {
				        (value ? usedIn : editableIn).push(key);
				    });
				    var tooltip = "editable in " + editableIn.join(", ");
				    if (usedIn.length > 0)
				        tooltip += ", also used in " + usedIn.join(", ");
				    return tooltip;
				};

				vm.actions = {
				    translate: function translate() {
				        vm.fieldModel.removeLanguage(languages.currentLanguage);
				        vm.fieldModel.addVs($scope.value.Value, languages.currentLanguage, false);
				    },
				    linkDefault: function linkDefault() {
				        vm.fieldModel.removeLanguage(languages.currentLanguage);
				    },
				    autoTranslate: function(languageKey) {
				        alert("This action is not implemented yet.");
				    },
				    copyFrom: function (languageKey) {
				        if ($scope.options.templateOptions.disabled)
				            alert("Copy not possible: the field is disabled.");
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
            dimensions[language] = (shareMode === null ? true : shareMode);
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
/* global angular */
(function () {
	'use strict';

	angular.module('eavEditEntity')
        .service('eavDefaultValueService', function () {
		// Returns a typed default value from the string representation
		return function parseDefaultValue(fieldConfig) {
			var e = fieldConfig;
			var d = e.templateOptions.settings.All.DefaultValue;

		    if (e.templateOptions.header.Prefill && e.templateOptions.header.Prefill[e.key]) {
			    d = e.templateOptions.header.Prefill[e.key];
			}

			switch (e.type.split('-')[0]) {
				case 'boolean':
					return d !== undefined ? d.toLowerCase() == 'true' : false;
				case 'datetime':
					return d !== undefined ? new Date(d) : null;
				case 'entity':
					return []; 
				case 'number':
					return null;
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
        .factory("entitiesSvc", ["$http", "appId", function($http, appId) {
            var svc = {};

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
                return $http.delete("eav/entities/delete", {
                    params: {
                        'contentType': type,
                        'id': id,
                        'appId': appId
                    }
                });
            };

            svc.newEntity = function(contentTypeName) {
                return {
                    Id: null,
                    Guid: generateUuid(),
                    Type: {
                        StaticName: contentTypeName
                    },
                    Attributes: {},
                    IsPublished: true
                };
            };

            svc.ensureGuid = function ensureGuid(item) {
                var ent = item.Entity;
                if ((ent.Id === null || ent.Id === 0) && (ent.Guid === null || typeof (ent.Guid) === "undefined" || ent.Guid === "00000000-0000-0000-0000-000000000000")) {
                    item.Entity.Guid = generateUuid();
                    item.Header.Guid = item.Entity.Guid;
                }
            };

            svc.save = function save(appId, newData) {
                return $http.post("eav/entities/save", newData, { params: { appId: appId } });
            };

            return svc;
        }]);


    // Generate Guid - code from http://stackoverflow.com/a/8809472
    function generateUuid() {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    }
})();
/* global angular */
(function () {
	"use strict";

	var app = angular.module("eavEditEntity");

	// The controller for the main form directive
	app.controller("EditEntityWrapperCtrl", ["$q", "$http", "$scope", "items", "$modalInstance", function editEntityCtrl($q, $http, $scope, items, $modalInstance) {

	    var vm = this;
	    vm.itemList = items;

	    // this is the callback after saving - needed to close everything
	    vm.afterSave = function(result) {
	        if (result.status === 200)
	            vm.close();
	        else {
	            alert("Something went wrong - maybe parts worked, maybe not. Sorry :(");
	        }
	    };

	    vm.state = {
	        isDirty: function() {
	            throw "Inner control must override this function.";
	        }
	    };

	    vm.close = function () {
		    $modalInstance.dismiss("cancel");
		};

	    $scope.$on('modal.closing', function(e) {
	        if (vm.state.isDirty() && !confirm("You have unsaved changes. Do you really want to exit?"))
	            e.preventDefault();
	    });
	}]);

})();