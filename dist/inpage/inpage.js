


// Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
var $2sxcActionMenuMapper = function (moduleId) {
    return {
        changeLayoutOrContent: function () {
            $2sxc(moduleId).manage._getSelectorScope().setTemplateChooserState(true);
        },
        addItem: function () {
            $2sxc(moduleId).manage.action({ 'action': 'add', 'useModuleList': true });
        },
        edit: function () {
            $2sxc(moduleId).manage.action({ 'action': 'edit', 'useModuleList': true, 'sortOrder': 0 });
        },
        adminApp: function () {
            $2sxc(moduleId).manage._openNgDialog({ 'action': 'app' });
        },
        adminZone: function () {
            $2sxc(moduleId).manage._openNgDialog({ 'action': 'zone' });
        }
    };
};
angular.module('SxcInpageTemplates',[]).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('template-selector/template-selector-app.html',
    "<div ng-cloak ng-show=vm.manageInfo.templateChooserVisible class=\"dnnFormMessage dnnFormInfo\"><div class=sc-selectors><select ng-show=!vm.manageInfo.isContentApp ng-model=vm.appId class=sc-selector-app ng-options=\"a.AppId as a.Name for a in vm.apps\" ng-disabled=\"vm.manageInfo.hasContent || vm.manageInfo.isList\"><option value=\"\" ng-disabled=\"vm.appId != null\" translate-attr-value=TemplatePicker.AppPickerDefault></option></select><select ng-show=vm.manageInfo.isContentApp ng-model=vm.contentTypeId ng-options=\"c.StaticName as c.Name for c in vm.contentTypes\" class=sc-selector-contenttype ng-disabled=\"vm.manageInfo.hasContent || vm.manageInfo.isList\"><option ng-disabled=\"vm.contentTypeId != ''\" value=\"\" translate=TemplatePicker.ContentTypePickerDefault></option></select><select ng-show=\"vm.manageInfo.isContentApp ? vm.contentTypeId != 0 : (vm.savedAppId != null &&  vm.filteredTemplates().length > 1)\" ng-model=vm.templateId class=sc-selector-template ng-options=\"t.TemplateId as t.Name for t in vm.filteredTemplates(vm.contentTypeId)\"></select></div><div class=sc-selector-actions><a ng-show=\"vm.templateId != null && vm.savedTemplateId != vm.templateId\" ng-click=vm.saveTemplateId(); class=sc-selector-save title=\"{{ 'TemplatePicker.Save' | translate }}\">{{ 'TemplatePicker.Save' | translate }}</a> <a ng-show=\"vm.savedTemplateId != null\" class=sc-selector-close ng-click=vm.setTemplateChooserState(false); title=\"{{ 'TemplatePicker.Close' | translate }}\">{{ 'TemplatePicker.Close' | translate }}</a></div><div class=\"sc-loading sc-loading-nobg\" ng-show=vm.loading></div></div>"
  );


  $templateCache.put('template-selector/template-selector-view.html',
    "<div ng-cloak ng-show=vm.manageInfo.templateChooserVisible class=\"dnnFormMessage dnnFormInfo\"><div class=sc-selectors><select ng-show=!vm.manageInfo.isContentApp ng-model=vm.appId class=sc-selector-app ng-options=\"a.AppId as a.Name for a in vm.apps\"><option value=\"\" translate=TemplatePicker.AppPickerDefault></option></select><select ng-show=vm.manageInfo.isContentApp ng-model=vm.contentTypeId ng-options=\"c.StaticName as c.Name for c in vm.contentTypes\" class=sc-selector-contenttype ng-disabled=\"vm.manageInfo.hasContent || vm.manageInfo.isList\"><option ng-disabled=\"vm.contentTypeId != ''\" value=\"\" translate=TemplatePicker.ContentTypePickerDefault></option></select><select ng-show=\"vm.manageInfo.isContentApp ? vm.contentTypeId != 0 : vm.savedAppId != null\" ng-model=vm.templateId class=sc-selector-template ng-options=\"t.TemplateId as t.Name for t in vm.filteredTemplates(vm.contentTypeId)\"></select></div><div class=sc-selector-actions><a ng-show=\"vm.templateId != null && vm.savedTemplateId != vm.templateId\" ng-click=vm.saveTemplateId(); class=sc-selector-save title=\"{{ 'TemplatePicker.Save' | translate }}\">{{ 'TemplatePicker.Save' | translate }}</a> <a ng-show=\"vm.savedTemplateId != null\" class=sc-selector-close ng-click=vm.setTemplateChooserState(false); title=\"{{ 'TemplatePicker.Cancel' | translate }}\">{{ 'TemplatePicker.Cancel' | translate }}</a></div><div class=\"sc-loading sc-loading-nobg\" ng-show=loading></div></div>"
  );

}]);

// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it

// all in-page toolbars etc.
$2sxc.getManageController = function (id) {

    var moduleElement = $(".DnnModule-" + id);
    var manageInfo = $.parseJSON(moduleElement.find(".Mod2sxcC, .Mod2sxcappC").attr("data-2sxc")).manage;
    var sxcGlobals = $.parseJSON(moduleElement.find(".Mod2sxcC, .Mod2sxcappC").attr("data-2sxc-globals"));
    manageInfo.ngDialogUrl = manageInfo.applicationRoot + "desktopmodules/tosic_sexycontent/dist/dnn/ui.html";

    manageInfo.ngDialogParams = {
        zoneId: manageInfo.zoneId,
        appId: manageInfo.appId,
        tid: manageInfo.config.tabId,
        mid: manageInfo.config.moduleId,
        lang: manageInfo.lang,
        langpri: manageInfo.langPrimary,
        langs: JSON.stringify(manageInfo.languages),
        portalroot: sxcGlobals.PortalRoot
    };

    var toolbarConfig = manageInfo.config;
    toolbarConfig.returnUrl = window.location.href;

    // all the standard buttons with the display configuration and click-action
    var actionButtonsConf = {
        'default': {
            icon: "glyphicon-fire",
            hideFirst: true,
            action: function(settings, event) { alert("not implemented yet"); }
        },
        'edit': {
            title: "Edit",
            icon: "glyphicon-pencil",
            lightbox: true,
            hideFirst: false,
            action: function(settings, event) {
                manageController._openNgDialog(settings, event);
            }
        },
        'new': {
            title: "New",
            icon: "glyphicon-plus",
            lightbox: true,
            hideFirst: false,
            action: function(settings, event) {
                manageController._openNgDialog($.extend({}, settings, { sortOrder: settings.sortOrder + 1 }), event);
            }
        },
        'add': {
            title: "Add",
            icon: "glyphicon-plus",
            lightbox: false,
            hideFirst: true,
            action: function(settings, event) {
                // ToDo: Remove dependency to AngularJS, should use 2sxc.api.js
                manageController._getSelectorScope().addItem(settings.sortOrder + 1);
            }
        },
        'replace': {
            title: "Replace",
            icon: "glyphicon-random",
            lightbox: true,
            hideFirst: true,
            action: function(settings, event) {
                manageController._openNgDialog(settings, event);
            }
        },
        'publish': {
            title: "Published",
            icon: "glyphicon-eye-open",
            icon2: "glyphicon-eye-close",
            lightbox: false,
            hideFirst: true,
            disabled: true,
            action: function (settings, event) {
                if (settings.isPublished) {
                    alert("already published");
                    return;
                }
                var part = settings.sortOrder == -1 ? "listcontent" : "content";
                var index = settings.sortOrder == -1 ? 0 : settings.sortOrder;
                manageController._getSelectorScope().publish(part, index);

                //alert("Status: " + (settings.isPublished ? "published" : "not published"));
            }
        },
        'moveup': {
            title: "Move up",
            icon: "glyphicon-arrow-up",
            icon2: "glyphicon-arrow-up",
            lightbox: false,
            hideFirst: true,
            disabled: false,
            action: function(settings, event) {
                manageController._getSelectorScope().changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
            }
        },
        'movedown': {
            title: "Move down",
            icon: "glyphicon-arrow-down",
            icon2: "glyphicon-arrow-down",
            lightbox: false,
            hideFirst: true,
            disabled: false,
            action: function(settings, event) {
                manageController._getSelectorScope().changeOrder(settings.sortOrder, settings.sortOrder + 1);
            }
        },
        'remove': {
            title: "remove",
            icon: "glyphicon-minus",
            lightbox: false,
            hideFirst: true,
            disabled: true,
            action: function(settings, event) {
                if (confirm("This will remove this content-item from this list, but not delete it (so you can add it again later). \nSee 2sxc.org/help for more. \n\nOk to remove?")) {
                    manageController._getSelectorScope().removeFromList(settings.sortOrder);
                }
            }
        },
        'more': {
            title: "More",
            icon: "glyphicon-option-horizontal",
            icon2: "glyphicon-option-vertical",
            borlightboxder: false,
            hideFirst: false,
            action: function(settings, event) {
                $(event.target).toggleClass(this.icon).toggleClass(this.icon2).closest("ul.sc-menu").toggleClass("showAll");
            }
        }
    };

    var manageController = {
        isEditMode: function() {
            return manageInfo.isEditMode;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        _toolbarConfig: toolbarConfig,

        _manageInfo: manageInfo,


        // create an edit-dialog link
        // needs the followings data:
        // zoneid, tid (tabid), mid (moduleid), appid
        // dialog=[zone|app|...]
        // lang=..., flang=
        getNgLink: function(settings) {
            settings = $.extend({}, toolbarConfig, settings);

            var params = {
                dialog: "edit",
                mode: (settings.action === "new") ? "new" : "edit"
            };
            var items = [];

            // when not using a content-group list, ...
            if (!settings.useModuleList) {
                if (settings.action !== "new")
                    items.push({ EntityId: settings.entityId });
                if (settings.contentType || settings.attributeSetName)
                    items.push({ ContentTypeName: settings.contentType || settings.attributeSetName });
            }
            // when using a list, the sort-order is important to find the right item
            if (settings.useModuleList || settings.action === "replace") {
                var normalContent = (settings.sortOrder !== -1);
                var index = normalContent ? settings.sortOrder : 0;
                items.push({
                    Group: {
                        Guid: settings.contentGroupId,
                        Index: index,
                        Part: normalContent ? "content" : "listcontent",
                        Add: settings.action === "new"
                    },
                    Title: normalContent ? "Content" : "List Content"
                });
                if (settings.action !== "replace") // if not replace, also add the presentation
                    items.push({
                        Group: {
                            Guid: settings.contentGroupId,
                            Index: index,
                            Part: normalContent ? "presentation" : "listpresentation",
                            Add: settings.action === "new"
                        },
                        Title: normalContent ? "Presentation" : "List Presentation"
                    });
            }

            if (settings.action === "replace" || settings.action === "app" || settings.action === "zone") 
                params.dialog = settings.action;
            
            // Serialize/json-ify the complex items-list
            if (items.length)
                params.items = JSON.stringify(items);

            // when doing new, there may be a prefill in the link to initialize the new item
            if (settings.prefill) {
                var prefill = JSON.stringify(settings.prefill);
                for (var i = 0; i < items.length; i++)
                    items[i].Prefill = prefill;
            }

            return manageInfo.ngDialogUrl
                + "#" + $.param(manageInfo.ngDialogParams)
                + "&" + $.param(params);
        },

        // open a new dialog of the angular-ui
        _openNgDialog: function(settings, event, closeCallback) {
            
            var callback = function () {
                manageController._getSelectorScope().reload();
                closeCallback();
            };
            var link = manageController.getNgLink(settings);

            if (event && event.shiftKey)
                window.open(link);
            else
                $2sxc.totalPopup.open(link, callback);
        },

        // Perform a toolbar button-action - basically get the configuration and execute it's action
        action: function(settings, event) {
            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            var conf = actionButtonsConf[settings.action] || actionButtonsConf.default;
            manageController._getSelectorScope().saveTemplateId().then(function() {
                conf.action(settings, origEvent);
            });
        },

        // Generate a button (an <a>-tag) for one specific toolbar-action. 
        // Expects: settings, an object containing the specs for the expected buton
        getButton: function(settings) {
            // if the button belongs to a content-item, move the specs to the item into the settings-object
            if (settings.entity && settings.entity._2sxcEditInformation) {
                if (settings.entity._2sxcEditInformation.entityId) {
                    settings.entityId = settings.entity._2sxcEditInformation.entityId;
                }
                if (settings.entity._2sxcEditInformation.sortOrder !== null) {
                    settings.sortOrder = settings.entity._2sxcEditInformation.sortOrder;
                    settings.useModuleList = true;
                }
                delete settings.entity;
            }

            // retrieve configuration for this button
            var conf = actionButtonsConf[settings.action] || actionButtonsConf.default;

            var button = $("<a />", {
                'class': "sc-" + settings.action + " "
                    + (settings.hideFirst || conf.hideFirst ? "hideFirst" : "")
                    + " " + (conf.lightbox ? "box" : ""),
                'onclick': "javascript:$2sxc(" + id + ").manage.action(" + JSON.stringify(settings) + ", event);",
                'title': conf.title
            });
            var symbol = $("<span class=\"glyphicon " + conf.icon + "\" aria-hidden=\"true\"></span>");

            // if publish-button and not published yet, show button (otherwise hidden) & change icon
            if (settings.action === "publish" && settings.isPublished === false) {
                button.toggleClass("hideFirst", false)
                    .attr("title", "Unpublished");
                symbol.toggleClass(conf.icon, false)
                    .toggleClass(conf.icon2, true);
            }

            button.html(symbol);

            return button[0].outerHTML;
        },

        // Builds the toolbar and returns it as HTML
        // expects settings - either for 1 button or for an array of buttons
        getToolbar: function(settings) {
            var buttons = [];

            if (settings.action) {
                // if single item with specified action, use this as our button-list
                //settings = [settings];
                buttons = [settings];
            } else if ($.isArray(settings)) {
                // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                buttons = settings;
            } else {
                // Create a standard menu with all standard buttons
                // first button: edit
                buttons.push($.extend({}, settings, { action: "edit" }));

                // add applicable list buttons - add=add item below; new=lightbox-dialog
                if (toolbarConfig.isList && settings.sortOrder != -1) { // if list and not the list-header
                    buttons.push($.extend({}, settings, { action: "new" }));
                    if (settings.useModuleList) {
                        buttons.push($.extend({}, settings, { action: "add" }));
                        if (settings.sortOrder !== 0)
                            buttons.push($.extend({}, settings, { action: "moveup" }));
                        buttons.push($.extend({}, settings, { action: "movedown" }));
                    }
                }
                buttons.push($.extend({}, settings, { action: "publish" }));

                // the replace button only makes sense if it's a content-group
                if (settings.useModuleList)
                    buttons.push($.extend({}, settings, { action: "replace" }));

                // only provide remove on lists
                if (toolbarConfig.isList) 
                    buttons.push($.extend({}, settings, { action: "remove" })); 
                
                buttons.push($.extend({}, settings, { action: "more" }));
            }

            var tbClasses = "sc-menu" + ((settings.sortOrder == -1) ? " listContent" : "");
            var toolbar = $("<ul />", { 'class': tbClasses, 'onclick': "javascript: var e = arguments[0] || window.event; e.stopPropagation();" });

            for (var i = 0; i < buttons.length; i++)
                toolbar.append($("<li />").append($(manageController.getButton(buttons[i]))));

            return toolbar[0].outerHTML;
        },

        // find all toolbar-info-attributes in the HTML, convert to <ul><li> toolbar
        _processToolbars: function() {
            $(".sc-menu[data-toolbar]", $(".DnnModule-" + id)).each(function() {
                var toolbarSettings = $.parseJSON($(this).attr("data-toolbar"));
                $(this).replaceWith($2sxc(id).manage.getToolbar(toolbarSettings));
            });
        },

        _getSelectorScope: function() {
            var selectorElement = document.querySelector(".DnnModule-" + id + " .sc-selector-wrapper");
            return angular.element(selectorElement).scope().vm;
        }

    };

    return manageController;
};

// Toolbar bootstrapping (initialize all toolbars after loading page)
$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $(".sc-menu").click(function (e) {
        e.stopPropagation();
    });

    var modules = $(".DnnModule-2sxc .Mod2sxcC[data-2sxc], .DnnModule-2sxc-app .Mod2sxcappC[data-2sxc]");

    modules.each(function () {
        var moduleId = $(this).data("2sxc").moduleId;
        $2sxc(moduleId).manage._processToolbars();
    });

    window.EavEditDialogs = [];
});

(function () {
    var module = angular.module("2sxc.view", [
        "2sxc4ng",
        "pascalprecht.translate",
        "SxcInpageTemplates",
        "EavConfiguration"
    ]);

    module.config(["$translateProvider", "AppInstanceId", "$translatePartialLoaderProvider", "languages", function ($translateProvider, AppInstanceId, $translatePartialLoaderProvider, languages) {
        
        var globals = $2sxc(AppInstanceId).manage._manageInfo;
        
        // add translation table
        $translateProvider
            .preferredLanguage(languages.currentLanguage.split("-")[0])
            .useSanitizeValueStrategy("escapeParameters")   // this is very important to allow html in the JSON files
            .fallbackLanguage(languages.defaultLanguage.split("-")[0])
            .useLoader("$translatePartialLoader", {
                urlTemplate: languages.i18nRoot + "{part}-{lang}.js"
            })
            .useLoaderCache(true);

        $translatePartialLoaderProvider.addPart("inpage");
    }]);

    module.controller("TemplateSelectorCtrl", ["$scope", "$attrs", "moduleApiService", "$filter", "$q", "$window", function($scope, $attrs, moduleApiService, $filter, $q, $window) {
        var vm = this;
        var realScope = $scope;

        var moduleId = $attrs.moduleid;
        var moduleApi = moduleApiService(moduleId);

        vm.manageInfo = $2sxc(moduleId).manage._manageInfo;
        vm.apps = [];
        vm.contentTypes = [];
        vm.templates = [];
        vm.filteredTemplates = function (contentTypeId) {
            // Return all templates for App
            if (!vm.manageInfo.isContentApp)
                return vm.templates;
            return $filter("filter")(vm.templates, contentTypeId == "_LayoutElement" ? { ContentTypeStaticName: "" } : { ContentTypeStaticName: contentTypeId }, true);
        };
        vm.contentTypeId = vm.manageInfo.contentTypeId;
        vm.templateId = vm.manageInfo.templateId;
        vm.savedTemplateId = vm.manageInfo.templateId;
        vm.appId = vm.manageInfo.appId;
        vm.savedAppId = vm.manageInfo.appId;
        vm.loading = 0;

        vm.reloadTemplates = function() {

            vm.loading++;
            var getContentTypes = moduleApi.getSelectableContentTypes();
            var getTemplates = moduleApi.getSelectableTemplates();

            $q.all([getContentTypes, getTemplates]).then(function (res) {
                vm.contentTypes = res[0].data;
                vm.templates = res[1].data;

                // Add option for no content type if there are templates without
                if ($filter("filter")(vm.templates, { ContentTypeStaticName: "" }, true).length > 0) {
                	vm.contentTypes.push({ StaticName: "_LayoutElement", Name: "Layout element" });
                    vm.contentTypes = $filter("orderBy")(vm.contentTypes, "Name");
                }

                vm.loading--;
            });

        };

        vm.reload = function () {
            if (vm.manageInfo.isContentApp)
                vm.renderTemplate(vm.templateId);
            else
                $window.location.reload();
        };

        realScope.$watch("vm.templateId", function (newTemplateId, oldTemplateId) {
        	if (newTemplateId != oldTemplateId) {
        		//alert("templateId changed");
        		if (vm.manageInfo.isContentApp)
        			vm.renderTemplate(newTemplateId);
        		else {
        			vm.loading++;
			        var promise;
			        if (vm.manageInfo.hasContent)
				        promise = vm.saveTemplateId(newTemplateId);
			        else
				        promise = vm.setPreviewTemplateId(newTemplateId);
			        promise.then(function() {
        				$window.location.reload();
			        });
        		}
        	}
        });

        realScope.$watch("vm.contentTypeId", function (newContentTypeId, oldContentTypeId) {
        	if (newContentTypeId == oldContentTypeId)
        		return;
        	// Select first template if contentType changed
        	var firstTemplateId = vm.filteredTemplates(newContentTypeId)[0].TemplateId; // $filter('filter')(vm.templates, { AttributeSetId: vm.contentTypeId == null ? "!!" : vm.contentTypeId })[0].TemplateID;
        	if (vm.templateId !== firstTemplateId && firstTemplateId !== null)
        		vm.templateId = firstTemplateId;
        });

        if (vm.appId !== null && vm.manageInfo.templateChooserVisible)
            vm.reloadTemplates();

        realScope.$watch("vm.manageInfo.templateChooserVisible", function(visible, oldVisible) {
            if (visible !== oldVisible && vm.appId !== null && visible)
                vm.reloadTemplates();
        });

        realScope.$watch("vm.appId", function (newAppId, oldAppId) {
            if (newAppId === oldAppId || newAppId === null)
                return;

            if (newAppId == -1) {
                window.location = $attrs.importappdialog;
                return;
            }

            moduleApi.setAppId(newAppId).then(function() {
                $window.location.reload();
            });
        });

        if (!vm.manageInfo.isContentApp) {
            moduleApi.getSelectableApps().then(function(data) {
                vm.apps = data.data;
                vm.apps.push({ Name: $attrs.importapptext, AppId: -1 });
            });
        }

        vm.setTemplateChooserState = function (state) {
            // Reset templateid / cancel template change
            if (!state)
                vm.templateId = vm.savedTemplateId;

            return moduleApi.setTemplateChooserState(state).then(function () {
                vm.manageInfo.templateChooserVisible = state;
            });
        };

        vm.saveTemplateId = function () {
        	var promises = [];

			// Save only if the currently saved is not the same as the new
        	if (!vm.manageInfo.hasContent || vm.savedTemplateId != vm.templateId) {
        		promises.push(moduleApi.saveTemplateId(vm.templateId).then(function(result) {
        		    // Make sure that ContentGroupGuid is updated accordingly
        		    var guid = result.data;
		            $2sxc(moduleId).manage._manageInfo.config.contentGroupId = guid;
		        }));
        		vm.savedTemplateId = vm.templateId;
        		promises.push(vm.setTemplateChooserState(false));
	        }

            return $q.all(promises);
        };

	    vm.setPreviewTemplateId = function() {
		    return moduleApi.setPreviewTemplateId(vm.templateId);
	    };

        vm.renderTemplate = function (templateId) {
            vm.loading++;
            moduleApi.renderTemplate(templateId).then(function (response) {
                try {
                    vm.insertRenderedTemplate(response.data);
                    $2sxc(moduleId).manage._processToolbars();
                } catch (e) {
                    console.log("Error while rendering template:");
                    console.log(e);
                }
                vm.loading--;
            });
        };

        vm.insertRenderedTemplate = function(renderedTemplate) {
            $(".DnnModule-" + moduleId + " .sc-viewport").html(renderedTemplate);
        };

		// ToDo: Remove this here, as it's not used in TemplateSelector - should move to 2sxc.api.manage.js
        vm.addItem = function(sortOrder) {
            moduleApi.addItem(sortOrder).then(function () {
                vm.renderTemplate(vm.templateId);
            });
        };
        vm.removeFromList = function (sortOrder) {
        	moduleApi.removeFromList(sortOrder).then(function () {
        		vm.renderTemplate(vm.templateId);
        	});
        };
        vm.changeOrder = function (sortOrder, desintationSortOrder) {
        	moduleApi.changeOrder(sortOrder, desintationSortOrder).then(function () {
        		vm.renderTemplate(vm.templateId);
        	});
        };
        vm.publish = function(part, sortOrder) {
            moduleApi.publish(part, sortOrder).then(function() {
                vm.renderTemplate(vm.templateId);
            });
        };

    }]);

    module.factory("moduleApiService", ["$http", function($http) {
        return function (moduleId) {
            function apiService(modId, settings) {
                return $http(settings);
            }
            return {
                saveTemplateId: function(templateId) {
                    return apiService(moduleId, {
                        url: "View/Module/SaveTemplateId",
                        params: { templateId: templateId }
                    });
                },
            	setPreviewTemplateId: function(templateId) {
            		return apiService(moduleId, {
            			url: "View/Module/SetPreviewTemplateId",
            			params: { templateId: templateId }
            		});
	            },
                addItem: function(sortOrder) {
                    return apiService(moduleId, {
                        url: "View/Module/AddItem",
                        params: { sortOrder: sortOrder }
                    });
                },
                getSelectableApps: function() {
                    return apiService(moduleId, {
                        url: "View/Module/GetSelectableApps"
                    });
                },
                setAppId: function(appId) {
                    return apiService(moduleId, {
                        url: "View/Module/SetAppId",
                        params: { appId: appId }
                    });
                },
                getSelectableContentTypes: function () {
                    return apiService(moduleId, {
                        url: "View/Module/GetSelectableContentTypes"
                    });
                },
                getSelectableTemplates: function() {
                    return apiService(moduleId, {
                        url: "View/Module/GetSelectableTemplates"
                    });
                },
                setTemplateChooserState: function(state) {
                    return apiService(moduleId, {
                        url: "View/Module/SetTemplateChooserState",
                        params: { state: state }
                    });
                },
                renderTemplate: function(templateId) {
                    return apiService(moduleId, {
                        url: "View/Module/RenderTemplate",
                        params: { templateId: templateId }
                    });
                },
                changeOrder: function (sortOrder, destinationSortOrder) {
                	return apiService(moduleId, {
                		url: "View/Module/ChangeOrder",
                		params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder }
                	});
                },
                publish: function(part, sortOrder) {
                 	return apiService(moduleId, {
                		url: "view/module/publish",
                		params: { part: part, sortOrder: sortOrder }
                	});
                },
                removeFromList: function (sortOrder) {
                	return apiService(moduleId, {
                		url: "View/Module/RemoveFromList",
                		params: { sortOrder: sortOrder }
                	});
                }
            };
        };
    }]);

})();