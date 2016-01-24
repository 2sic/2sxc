// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it

// all in-page toolbars etc.
$2sxc.getManageController = function (id) {

    var moduleElement = $(".DnnModule-" + id);
    var manageInfo = $.parseJSON(moduleElement.find("div[data-2sxc]").attr("data-2sxc")).manage;
    var sxcGlobals = $.parseJSON(moduleElement.find("div[data-2sxc-globals]").attr("data-2sxc-globals"));
    manageInfo.ngDialogUrl = manageInfo.applicationRoot + "desktopmodules/tosic_sexycontent/dist/dnn/ui.html";

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

    var contentTypeName = manageInfo.contentTypeId; // note: only exists if the template has a content-type
    var toolbarConfig = manageInfo.config;
    var enableTools = manageInfo.user.canDesign;
    var enableDevelop = manageInfo.user.canDevelop;
    var isContent = manageInfo.isContentApp;

    function buttonConfig(name, translateKey, icon, show,  uiOnly, more) {
        return angular.extend({
            title: "Toolbar." + translateKey,
            iclass: "icon-sxc-" + icon,
            showOn: show,
            uiActionOnly: uiOnly
        }, more);
    }

    var actionButtonsConf = {
        'edit': buttonConfig('edit', "Edit", "pencil", "default", false, { params: { mode: "edit" } }),
        'new': buttonConfig('new', "New", "plus", "default", false, { params: { mode: "new" },
            dialog: "edit", // don't use "new" (default) but use "edit"
            addCondition: function(settings) { return toolbarConfig.isList && settings.sortOrder !== -1; },
            code: function(settings, event) {
                tbContr._openNgDialog($.extend({}, settings, { sortOrder: settings.sortOrder + 1 }), event);
            }
        }),
        'add': buttonConfig('add', "AddDemo", "plus-circled", "edit", false, {
            addCondition: function(settings) { return toolbarConfig.isList && settings.sortOrder !== -1 && settings.useModuleList; },
            code: function(settings, event) {
                tbContr._getAngularVm().addItem(settings.sortOrder + 1);
            }
        }),
        'replace': buttonConfig('replace', "Replace", "replace", "edit", false, {
            addCondition: function(settings) { return settings.useModuleList; },
        }),
        'publish': buttonConfig('publish', "Published", "eye", "edit", false, {
            iclass2: "icon-sxc-eye-off",
            disabled: true,
            code: function(settings, event) {
                if (settings.isPublished) {
                    alert(tbContr.translate("Toolbar.AlreadyPublished"));
                    return;
                }
                var part = settings.sortOrder === -1 ? "listcontent" : "content";
                var index = settings.sortOrder === -1 ? 0 : settings.sortOrder;
                tbContr._getAngularVm().publish(part, index);
            }
        }),

        'moveup': {
            title: "Toolbar.MoveUp",
            iclass: "icon-sxc-move-up",
            disabled: false,
            showOn: "edit",
            addCondition: function(settings) { return toolbarConfig.isList && settings.sortOrder !== -1 && settings.useModuleList && settings.sortOrder !== 0; },
            code: function(settings, event) {
                tbContr._getAngularVm().changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
            }
        },
        'movedown': {
            title: "Toolbar.MoveDown",
            iclass: "icon-sxc-move-down",
            disabled: false,
            showOn: "edit",
            addCondition: function(settings) { return toolbarConfig.isList && settings.sortOrder !== -1 && settings.useModuleList; },
            code: function(settings, event) {
                tbContr._getAngularVm().changeOrder(settings.sortOrder, settings.sortOrder + 1);
            }
        },
        'sort': {
            title: "Toolbar.Sort",
            iclass: "icon-sxc-list-numbered",
            showOn: "edit",
            addCondition: function(settings) { return toolbarConfig.isList && settings.sortOrder !== -1; },
        },
        'remove': {
            title: "Toolbar.Remove",
            iclass: "icon-sxc-minus-circled",
            disabled: true,
            showOn: "edit",
            addCondition: function(settings) { return toolbarConfig.isList && settings.sortOrder !== -1; },
            code: function(settings, event) {
                if (confirm(tbContr.translate("Toolbar.ConfirmRemove"))) {
                    tbContr._getAngularVm().removeFromList(settings.sortOrder);
                }
            }
        },
        'layout': {
            title: "Toolbar.ChangeLayout",
            iclass: "icon-sxc-glasses",
            showOn: "default",
            uiActionOnly: true, // so it doesn't create the content when used
            code: function(settings, event) {
                tbContr._getAngularVm().toggle();
            }
        },
        'develop': buttonConfig("develop", "Develop", "code", "admin", true, {
            newWindow: true,
            addCondition: enableTools,         
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
            params: { contentTypeName: contentTypeName },
            uiActionOnly: true, // so it doesn't create the content when used
            addCondition: enableTools && contentTypeName,
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
            code: function(settings, event) {
                var fullMenu = $(event.target).closest("ul.sc-menu");
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

    var tbContr = {
        isEditMode: function() {
            return manageInfo.isEditMode;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        _toolbarConfig: toolbarConfig,

        _manageInfo: manageInfo,

        // create a dialog link
        getNgLink: function(settings) {
            settings = $.extend({}, toolbarConfig, settings); // merge button with toolbar-settings

            var params = {
                dialog: settings.dialog || settings.action,
            };
            angular.extend(params, settings.params);
            var items = [];

            // when not using a content-group list, ...
            if (!settings.useModuleList) {
                if (settings.action !== "new")
                    items.push({ EntityId: settings.entityId });
                else if (settings.contentType || settings.attributeSetName)
                    items.push({ ContentTypeName: settings.contentType || settings.attributeSetName });
            }
            // when using a list, the sort-order is important to find the right item
            if (settings.useModuleList || settings.action === "replace" || settings.action === "sort") {
                var normalContent = (settings.sortOrder !== -1);
                var index = normalContent ? settings.sortOrder : 0;
                items.push({
                    Group: {
                        Guid: settings.contentGroupId,
                        Index: index,
                        Part: normalContent ? "content" : "listcontent",
                        Add: settings.action === "new"
                    },
                    Title: tbContr.translate("EditFormTitle." + (normalContent ? "Content" : "ListContent"))
                });
                if (settings.action !== "replace") // if not replace, also add the presentation
                    items.push({
                        Group: {
                            Guid: settings.contentGroupId,
                            Index: index,
                            Part: normalContent ? "presentation" : "listpresentation",
                            Add: settings.action === "new"
                        },
                        Title: tbContr.translate("EditFormTitle." + (normalContent ? "Presentation" : "ListPresentation"))
                    });
            }

            if (settings.action === "develop")
                items = [{ EntityId: manageInfo.templateId }];

            // when doing new, there may be a prefill in the link to initialize the new item
            if (settings.prefill) 
                for (var i = 0; i < items.length; i++)
                    items[i].Prefill = settings.prefill;

            // Serialize/json-ify the complex items-list
            if (items.length)
                params.items = JSON.stringify(items);

            return manageInfo.ngDialogUrl
                + "#" + $.param(manageInfo.ngDialogParams)
                + "&" + $.param(params);
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
            else
                $2sxc.totalPopup.open(link, callback);
        },

        // Perform a toolbar button-action - basically get the configuration and execute it's action
        action: function(settings, event) {
            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            var conf = actionButtonsConf[settings.action];
            if (conf.newWindow) settings.newWindow = conf.newWindow;
            if (conf.params) settings.params = conf.params;
            if (conf.dialog) settings.dialog = conf.dialog;
            if (!settings.dialog) settings.dialog = settings.action; // old code uses "action" as the parameter, now use verb

            var fn = conf.code || tbContr._openNgDialog; // decide what action to perform

            if (conf.uiActionOnly)
                return fn(settings, origEvent);
            
            // if more than just a UI-action, then it needs to be sure the content-group is created first
            tbContr._getAngularVm().prepareToAddContent().then(function() {
                return fn(settings, origEvent);
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
            var conf = actionButtonsConf[btnSettings.action];

            var showClasses = "";
            var classesList = conf.showOn.split(",");
            for (var c = 0; c < classesList.length; c++)
                showClasses += " show-" + classesList[c];
            var button = $("<a />", {
                'class': "sc-" + btnSettings.action + " " + showClasses,
                'onclick': "javascript:$2sxc(" + id + ").manage.action(" + JSON.stringify(btnSettings) + ", event);",
                'title': tbContr.translate(conf.title)
            });
            var box = $("<div/>");
            var symbol = $("<i class=\"" + conf.iclass + "\" aria-hidden=\"true\"></i>");

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
                if (Array.isArray(verb))
                    return verb.forEach(function(val) { buttons.add(val); });
                var add = actionButtonsConf[verb].addCondition;
                if (add === undefined || ((typeof (add) === 'function') ? add(settings) : add))
                    buttons.push($.extend({}, settings, { action: verb }));
            };
            buttons.add(["edit", "new", "add", "remove", "moveup", "movedown", "sort", "publish", "replace", "layout", "develop", "contenttype", "contentitems", "app", "zone", "more"]);
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

        translate: function(key) {
            return tbContr._getAngularVm().translate(key);
        }

    };

    return tbContr;
};