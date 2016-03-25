/*
 * Actions of 2sxc - mostly used in toolbars
 * 
 * Minimal documentation regarding a button
 * the button can have the following properties / methods
 * - the indexer in the array (usually the same as the name)
 * - name (created in the buttonConfig)
 * - title - actually the translation key to retrieve the title (buttonConfig)
 * - iclass - the icon-class
 * - showOn - comma separated list of values on which toolbar state to show this on
 * - uiActionOnly - true/false if this is just something visual; otherwise a webservice will ensure that a content-group exists (for editing etc.)
 * - addCondition(settings, moduleConfiguration) - would conditionally prevent adding this button by default
 * - code(settings, event) - the code executed on click, if it's not the default action
 * - dynamicClasses(settings) - can conditionally add more css-class names to add to the button, like the "empty" added if something doesn't have metadata
 * - params - ...
 */

(function () {
    // helper function to create the configuration object
    function createActionConfig(name, translateKey, icon, show, uiOnly, more) {
        return $2sxc._lib.extend({
            name: name,
            title: "Toolbar." + translateKey,
            iclass: "icon-sxc-" + icon,
            showOn: show,
            uiActionOnly: uiOnly
        }, more);
    }

    $2sxc._actions = {};
    var thisObj = $2sxc._actions.create = function (manageInfo) {
        var enableTools = manageInfo.user.canDesign;

        var act = {
            "dash-view": createActionConfig("dash", "Dashboard", "", "", true, {
                inlineWindow: true
            }),
            'edit': createActionConfig("edit", "Edit", "pencil", "default", false, { params: { mode: "edit" } }),
            // new is a dialog to add something, and will not add if cancelled
            // new can also be used for mini-toolbars which just add an entity not attached to a module
            // in that case it's essential to add a contentType like 
            // <ul class="sc-menu" data-toolbar='{"action":"new", "contentType": "Category"}'></ul>
            'new': createActionConfig("new", "New", "plus", "default", false, {
                params: { mode: "new" },
                dialog: "edit", // don't use "new" (default) but use "edit"
                addCondition: function (settings, modConfig) {
                    return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; // don't provide new on the header-item
                },
                code: function (settings, event, toolbarManager) {
                    toolbarManager._openNgDialog($2sxc._lib.extend({}, settings, { sortOrder: settings.sortOrder + 1 }), event);
                }
            }),
            // add brings no dialog, just add an empty item
            'add': createActionConfig("add", "AddDemo", "plus-circled", "edit", false, {
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
                code: function (settings, event, toolbarManager) {
                    toolbarManager._getAngularVm().addItem(settings.sortOrder + 1);
                }
            }),
            "metadata": createActionConfig("metadata", "Metadata", "tag", "default", false, {
                params: { mode: "new" },
                dialog: "edit", // don't use "new" (default) but use "edit"
                dynamicClasses: function (settings) {
                    // if it doesn't have data yet, make it less strong
                    return settings.entityId ? "" : "empty";
                    // return settings.items && settings.items[0].entityId ? "" : "empty";
                },
                addCondition: function (settings) { return !!settings.metadata; }, // only add a metadata-button if it has metadata-infos
                configureCommand: function (cmd) {
                    var itm = {
                        Title: "EditFormTitle.Metadata",
                        Metadata: $2sxc._lib.extend({ keyType: "string", targetType: 10 }, cmd.settings.metadata)
                    };
                    $2sxc._lib.extend(cmd.items[0], itm);
                }
            }),
            'remove': {
                title: "Toolbar.Remove",
                iclass: "icon-sxc-minus-circled",
                disabled: true,
                showOn: "edit",
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
                code: function (settings, event) {
                    if (confirm(tbContr.translate("Toolbar.ConfirmRemove"))) {
                        tbContr._getAngularVm().removeFromList(settings.sortOrder);
                    }
                }
            },

            // todo: work in progress related to https://github.com/2sic/2sxc/issues/618
            //'delete': {
            //    title: "Toolbar.Delete",
            //    iclass: "icon-sxc-cancel",
            //    disabled: true,
            //    showOn: "edit",
            //    addCondition: function (settings) { return !settings.useModuleList; },
            //    code: function (settings, event) {
            //        if (confirm(tbContr.translate("Toolbar.ReallyDelete"))) {
            //            tbContr._getAngularVm().reallyDelete(settings.entityId);
            //        }
            //    }
            //},

            'moveup': {
                title: "Toolbar.MoveUp",
                iclass: "icon-sxc-move-up",
                disabled: false,
                showOn: "edit",
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1 && settings.sortOrder !== 0; },
                code: function (settings, event, toolbarManager) {
                    toolbarManager._getAngularVm().changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
                }
            },
            'movedown': {
                title: "Toolbar.MoveDown",
                iclass: "icon-sxc-move-down",
                disabled: false,
                showOn: "edit",
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
                code: function (settings, event, toolbarManager) {
                    toolbarManager._getAngularVm().changeOrder(settings.sortOrder, settings.sortOrder + 1);
                }
            },
            'sort': {
                title: "Toolbar.Sort",
                iclass: "icon-sxc-list-numbered",
                showOn: "edit",
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; }
            },
            'publish': createActionConfig("publish", "Published", "eye", "edit", false, {
                iclass2: "icon-sxc-eye-off",
                disabled: true,
                code: function (settings, event, toolbarManager) {
                    if (settings.isPublished) {
                        alert(toolbarManager.translate("Toolbar.AlreadyPublished"));
                        return;
                    }
                    var part = settings.sortOrder === -1 ? "listcontent" : "content";
                    var index = settings.sortOrder === -1 ? 0 : settings.sortOrder;
                    toolbarManager._getAngularVm().publish(part, index);
                }
            }),
            'replace': createActionConfig("replace", "Replace", "replace", "edit", false, {
                addCondition: function (settings) { return settings.useModuleList; },
            }),
            'layout': {
                title: "Toolbar.ChangeLayout",
                iclass: "icon-sxc-glasses",
                showOn: "default",
                uiActionOnly: true, // so it doesn't create the content when used
                code: function (settings, event, toolbarManager) {
                    toolbarManager._getAngularVm().toggle();
                }
            },
            'develop': createActionConfig("develop", "Develop", "code", "admin", true, {
                newWindow: true,
                addCondition: enableTools,
                configureCommand: function (cmd) {
                    cmd.items = [{ EntityId: manageInfo.templateId }];
                }
            }),
            'contenttype': {
                title: "Toolbar.ContentType",
                iclass: "icon-sxc-fields",
                showOn: "admin",
                uiActionOnly: true,
                addCondition: enableTools,
            },
            'contentitems': {
                title: "Toolbar.ContentItems",
                iclass: "icon-sxc-table",
                showOn: "admin",
                params: { contentTypeName: manageInfo.contentTypeId },
                uiActionOnly: true, // so it doesn't create the content when used
                addCondition: enableTools && manageInfo.contentTypeId,
            },
            'app': {
                title: "Toolbar.App",
                iclass: "icon-sxc-settings",
                showOn: "admin",
                uiActionOnly: true, // so it doesn't create the content when used
                addCondition: enableTools,
            },
            'zone': {
                title: "Toolbar.Zone",
                iclass: "icon-sxc-manage",
                showOn: "admin",
                uiActionOnly: true, // so it doesn't create the content when used
                addCondition: enableTools,
            },
            "more": {
                title: "Toolbar.MoreActions",
                iclass: "icon-sxc-options btn-mode",
                showOn: "default,edit,design,admin",
                uiActionOnly: true, // so it doesn't create the content when clicked
                code: function (settings, event, toolbarManager) {
                    var fullMenu = $(event.target).closest("ul.sc-menu"); // todo: slightly nasty dependency...
                    var oldState = Number(fullMenu.attr("data-state") || 0);
                    var newState = oldState + 1;
                    if (newState === 2) newState = 3; // state 1 doesn't exist yet - skip
                    newState = newState % (enableTools ? 4 : 3); // if tools are enabled, there are 4 states
                    fullMenu.removeClass("show-set-" + oldState)
                        .addClass("show-set-" + newState)
                        .attr("data-state", newState);
                }
            }
        };
        return act;
    };

})();


(function () {
	$2sxc._lib = {
		extend:
            function extend() { // same as angular.extend or jquery.extend, but without that additional dependency
            	for (var i = 1; i < arguments.length; i++)
            		for (var key in arguments[i])
            			if (arguments[i].hasOwnProperty(key))
            				arguments[0][key] = arguments[i][key];
            	return arguments[0];
            }
	};
})();




// Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
var $2sxcActionMenuMapper = function (moduleId) {
    return {
        changeLayoutOrContent: function () {
            $2sxc(moduleId).manage.action({ "action": "layout" });
        },
        addItem: function () {
            $2sxc(moduleId).manage.action({ "action": "add", "useModuleList": true, "sortOrder": 0 });
        },
        edit: function () {
            $2sxc(moduleId).manage.action({ "action": "edit", "useModuleList": true, "sortOrder": 0 });
        },
        adminApp: function () {
            $2sxc(moduleId).manage.action({ "action": "app" });
        },
        adminZone: function () {
            $2sxc(moduleId).manage.action({ "action": "zone" });
        },
        develop: function () {
            $2sxc(moduleId).manage.action({ "action": "develop" });
        }
    };
};
// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it
// all in-page toolbars etc.

$2sxc.getManageController = function (id) {
    var moduleElement = $(".DnnModule-" + id);
    var manageInfo = $.parseJSON(moduleElement.find("div[data-2sxc]").attr("data-2sxc")).manage;
    var sxcGlobals = $.parseJSON(moduleElement.find("div[data-2sxc-globals]").attr("data-2sxc-globals"));
    manageInfo.ngDialogUrl = manageInfo.applicationRoot + "desktopmodules/tosic_sexycontent/dist/dnn/ui.html?sxcver="
        + manageInfo.config.version;

    // assemble all parameters needed for the dialogs if we open anything
    manageInfo.ngDialogParams = {
        zoneId: manageInfo.zoneId,
        appId: manageInfo.appId,
        tid: manageInfo.config.tabId,
        mid: manageInfo.config.moduleId,
        lang: manageInfo.lang,
        langpri: manageInfo.langPrimary,
        langs: JSON.stringify(manageInfo.languages),
        portalroot: sxcGlobals.PortalRoot,
        websiteroot: manageInfo.applicationRoot,
        user: manageInfo.user,
        // note that the app-root doesn't exist when opening "manage-app"
        approot: (manageInfo.config && manageInfo.config.appPath) ? manageInfo.config.appPath : null // this is the only value which doesn't have a slash by default
    };

    //var contentTypeName = manageInfo.contentTypeId; // note: only exists if the template has a content-type
    //var toolbarConfig = manageInfo.config;
    manageInfo.config.contentType = manageInfo.config.contentType || manageInfo.config.attributeSetName;
    // var enableTools = manageInfo.user.canDesign;
    // var enableDevelop = manageInfo.user.canDevelop;
    // var isContent = manageInfo.isContentApp;
    var isDebug = $2sxc.urlParams.get("debug") ? "&debug=true" : "",
        actionButtonsConf = $2sxc._actions.create(manageInfo);

    var tbContr = {
        // public method to find out if it's in edit-mode
        isEditMode: function() {    return manageInfo.isEditMode;   },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        _manageInfo: manageInfo,
        _toolbarConfig: manageInfo.config,

        // assemble an object which will store the configuration and execute it
        createCommandObject: function(specialSettings) {
            var settings = $2sxc._lib.extend({}, tbContr._toolbarConfig, specialSettings); // merge button with general toolbar-settings
            var cmd = {
                settings: settings,
                items: settings.items || [],                            // use predefined or create empty array
                params: $2sxc._lib.extend({
                    dialog: settings.dialog || settings.action          // the variable used to name the dialog changed in the history of 2sxc from action to dialog
                }, settings.params),

                addSimpleItem: function() {
                    var itm = {}, ct = cmd.settings.contentType || cmd.settings.attributeSetName; // two ways to name the content-type-name this, v 7.2+ and older
                    if (cmd.settings.entityId) itm.EntityId = cmd.settings.entityId;
                    if (ct) itm.ContentTypeName = ct;
                    if (itm.EntityId || itm.ContentTypeName)    // only add if there was stuff to add
                        cmd.items.push(itm);
                },

                // this adds an item of the content-group, based on the group GUID and the sequence number
                addContentGroupItem: function(guid, index, part, isAdd, sectionLanguageKey) {
                    cmd.items.push({
                        Group: { Guid: guid, Index: index, Part: part, Add: isAdd },
                        Title: tbContr.translate(sectionLanguageKey)
                    });
                },

                // this will tell the command to edit a item from the sorted list in the group, optionally together with the presentation item
                addContentGroupItemSetsToEditList: function (withPresentation) {
                    var isContentAndNotHeader = (cmd.settings.sortOrder !== -1);
                    var index = isContentAndNotHeader ? cmd.settings.sortOrder : 0;
                    var prefix = isContentAndNotHeader ? "" : "List";
                    var cTerm = prefix + "Content";
                    var pTerm = prefix + "Presentation";
                    var isAdd = cmd.settings.action === "new";
                    var groupId = cmd.settings.contentGroupId;
                    cmd.addContentGroupItem(groupId, index, cTerm.toLowerCase(), isAdd, "EditFormTitle." + cTerm);

                    if (withPresentation) 
                        cmd.addContentGroupItem(groupId, index, pTerm.toLowerCase(), isAdd, "EditFormTitle." + pTerm);
                    
                },

                generateLink: function () {
                    // if there is no items-array, create an empty one (it's required later on)
                    if (!cmd.settings.items) cmd.settings.items = [];
                    //#region steps for all actions: prefill, serialize, open-dialog
                    // when doing new, there may be a prefill in the link to initialize the new item
                    if (cmd.settings.prefill)
                        for (var i = 0; i < cmd.items.length; i++)
                            cmd.items[i].Prefill = cmd.settings.prefill;

                    // Serialize/json-ify the complex items-list
                    // if (cmd.items.length)
                        cmd.params.items = JSON.stringify(cmd.items);

                    return manageInfo.ngDialogUrl
                        + "#" + $.param(manageInfo.ngDialogParams)
                        + "&" + $.param(cmd.params)
                        + isDebug;
                    //#endregion
                }
            };
            return cmd;
        },

        // create a dialog link
        getNgLink: function (specialSettings) {
            var cmd = tbContr.createCommandObject(specialSettings);

            if (cmd.settings.useModuleList)
                cmd.addContentGroupItemSetsToEditList(true);
            else
                cmd.addSimpleItem();            

            // if the command has own configuration stuff, do that now
            if (cmd.settings.configureCommand)
                cmd.settings.configureCommand(cmd);

            return cmd.generateLink();
        },

        // open a new dialog of the angular-ui
        _openNgDialog: function(settings, event, closeCallback) {
            
            var callback = function () {
                tbContr._getAngularVm().reload();
                closeCallback();
            };
            var link = tbContr.getNgLink(settings);

            if (settings.newWindow || (event && event.shiftKey))
                window.open(link);
            else {
                if (settings.inlineWindow)
                    //window.open(link);
                    $2sxc.dialog.create(id, moduleElement, link, callback);
                else
                    $2sxc.totalPopup.open(link, callback);
            }
        },

        // Perform a toolbar button-action - basically get the configuration and execute it's action
        action: function(settings, event) {
            var conf = actionButtonsConf[settings.action];
            settings = $2sxc._lib.extend({}, conf, settings);              // merge conf & settings, but settings has higher priority
            if (!settings.dialog) settings.dialog = settings.action;    // old code uses "action" as the parameter, now use verb ? dialog
            if (!settings.code) settings.code = tbContr._openNgDialog;  // decide what action to perform

            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            if (conf.uiActionOnly)
                return settings.code(settings, origEvent, tbContr);
            
            // if more than just a UI-action, then it needs to be sure the content-group is created first
            tbContr._getAngularVm().prepareToAddContent().then(function() {
                return settings.code(settings, origEvent, tbContr);
            });
        },

        // Generate a button (an <a>-tag) for one specific toolbar-action. 
        // Expects: settings, an object containing the specs for the expected buton
        getButton: function(btnSettings) {
            // if the button belongs to a content-item, move the specs up to the item into the settings-object
            if (btnSettings.entity && btnSettings.entity._2sxcEditInformation) {
                if (btnSettings.entity._2sxcEditInformation.entityId) {
                    btnSettings.entityId = btnSettings.entity._2sxcEditInformation.entityId;
                    btnSettings.useModuleList = false;
                }
                if (btnSettings.entity._2sxcEditInformation.sortOrder) {
                    btnSettings.sortOrder = btnSettings.entity._2sxcEditInformation.sortOrder;
                    btnSettings.useModuleList = true;
                }
                delete btnSettings.entity;
            }

            // retrieve configuration for this button
            var conf = actionButtonsConf[btnSettings.action],
                showClasses = "",
                classesList = conf.showOn.split(","),
                box = $("<div/>"),
                symbol = $("<i class=\"" + conf.iclass + "\" aria-hidden=\"true\"></i>");

            for (var c = 0; c < classesList.length; c++) 
                showClasses += " show-" + classesList[c];

            var button = $("<a />", {
                'class': "sc-" + btnSettings.action + " " + showClasses + (conf.dynamicClasses ? " " + conf.dynamicClasses(btnSettings) : ""),
                'onclick': "javascript:$2sxc(" + id + ").manage.action(" + JSON.stringify(btnSettings) + ", event);",
                'title': tbContr.translate(conf.title)
            });

            // todo: move the following lines into the button-config and just call from here
            // if publish-button and not published yet, show button (otherwise hidden) & change icon
            if (btnSettings.action === "publish" && btnSettings.isPublished === false) {
                button.addClass("show-default").removeClass("show-edit")
                    .attr("title", tbContr.translate("Toolbar.Unpublished")); 
                symbol.removeClass(conf.iclass).addClass(conf.iclass2);
            }

            button.html(box.html(symbol));

            return button[0].outerHTML;
        },

        // Builds the toolbar and returns it as HTML
        // expects settings - either for 1 button or for an array of buttons
        getToolbar: function(settings) {
            var buttons = settings.action
                ? [settings] // if single item with specified action, use this as our button-list
                : $.isArray(settings)
                ? settings // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                : tbContr.createDefaultToolbar(settings);

            var tbClasses = "sc-menu show-set-0" + ((settings.sortOrder === -1) ? " listContent" : "");
            var toolbar = $("<ul />", { 'class': tbClasses, 'onclick': "javascript: var e = arguments[0] || window.event; e.stopPropagation();" });

            for (var i = 0; i < buttons.length; i++)
                toolbar.append($("<li />").append($(tbContr.getButton(buttons[i]))));

            return toolbar[0].outerHTML;
        },

        // Assemble a default toolbar instruction set
        createDefaultToolbar: function (settings) {
            // Create a standard menu with all standard buttons
            var buttons = [];

            buttons.add = function (verb) {
                var add = actionButtonsConf[verb].addCondition;
                if (add === undefined || ((typeof (add) === "function") ? add(settings, tbContr._toolbarConfig) : add))
                    buttons.push($2sxc._lib.extend({}, settings, { action: verb }));
            };

            for (var btn in actionButtonsConf) 
                if (actionButtonsConf.hasOwnProperty(btn))
                    buttons.add(btn);

            return buttons;
        },

        // find all toolbar-info-attributes in the HTML, convert to <ul><li> toolbar
        _processToolbars: function() {
            $(".sc-menu[data-toolbar]", $(".DnnModule-" + id)).each(function() {
                var toolbarSettings = $.parseJSON($(this).attr("data-toolbar"));
                $(this).replaceWith($2sxc(id).manage.getToolbar(toolbarSettings));
            });
        },

        _getAngularVm: function() {
            var selectorElement = document.querySelector(".DnnModule-" + id + " .sc-selector-wrapper");
            return angular.element(selectorElement).scope().vm;
        },

        translate: function (key) {
            // todo: re-enable translate
            // return key;
            return tbContr._getAngularVm().translate(key);
        }

    };

    return tbContr;
};

// Toolbar bootstrapping (initialize all toolbars after loading page)
$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $(".sc-menu").click(function (e) {
        e.stopPropagation();
    });

    //var modules = $(".DnnModule-2sxc .Mod2sxcC[data-2sxc], .DnnModule-2sxc-app .Mod2sxcappC[data-2sxc]");
    var modules = $("div[data-2sxc]");

    // Ensure the _processToolbar is called after the next event cycle to make sure that the Angular app (template selector) is loaded first
    window.setTimeout(function () {
        modules.each(function () {
            try {
                var moduleId = $(this).data("2sxc").moduleId;
                $2sxc(moduleId).manage._processToolbars();
            } catch (e) { // Make sure that if one app breaks, others continue to work
                if (console && console.error)
                    console.error(e);
            }
        });
    }, 0);

    window.EavEditDialogs = [];
});
