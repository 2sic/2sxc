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

    var toolbarConfig = manageInfo.config;
    var enableTools = manageInfo.user.canDesign;
    var enableDevelop = manageInfo.user.canDevelop;
    var isContent = manageInfo.isContentApp;

    // all the standard buttons with the display configuration and click-action
    var actionButtonsConf = {
        'default': {
            hideFirst: true,
            action: function(settings, event) { alert("not implemented yet"); }
        },
        'edit': {
            title: "Toolbar.Edit",
            iclass: "icon-sxc-pencil",
            lightbox: true,
            hideFirst: true,
            showOn: "default",
            action: function(settings, event) {
                tbContr._openNgDialog(settings, event);
            }
        },
        'new': {
            title: "Toolbar.New",
            iclass: "icon-sxc-plus",
            lightbox: true,
            hideFirst: true,
            showOn: "default",
            action: function(settings, event) {
                tbContr._openNgDialog($.extend({}, settings, { sortOrder: settings.sortOrder + 1 }), event);
            }
        },
        'layout': {
            title: "Toolbar.ChangeLayout",
            iclass: "icon-sxc-glasses",
            lightbox: false,
            hideFirst: true,
            showOn: "default",
            uiActionOnly: true, // so it doesn't create the content when used
            action: function (settings, event) {
                tbContr._getSelectorScope().toggle();
            }
        },
        'add': {
            title: "Toolbar.AddDemo",
            iclass: "icon-sxc-plus-circled",
            lightbox: false,
            hideFirst: true,
            showOn: "edit",
            action: function(settings, event) {
                tbContr._getSelectorScope().addItem(settings.sortOrder + 1);
            }
        },
        'replace': {
            title: "Toolbar.Replace",
            iclass: "icon-sxc-replace",
            lightbox: true,
            hideFirst: true,
            showOn: "edit",
            action: function(settings, event) {
                tbContr._openNgDialog(settings, event);
            }
        },
        'publish': {
            title: "Toolbar.Published",
            iclass: "icon-sxc-eye",
            iclass2: "icon-sxc-eye-off",
            lightbox: false,
            hideFirst: true,
            disabled: true,
            showOn: "edit",
            action: function (settings, event) {
                if (settings.isPublished) {
                    alert(tbContr.translate("Toolbar.AlreadyPublished")); 
                    return;
                }
                var part = settings.sortOrder === -1 ? "listcontent" : "content";
                var index = settings.sortOrder === -1 ? 0 : settings.sortOrder;
                tbContr._getSelectorScope().publish(part, index);
            }
        },
        'moveup': {
            title: "Toolbar.MoveUp",
            iclass: "icon-sxc-move-up",
            lightbox: false,
            hideFirst: true,
            disabled: false,
            showOn: "edit",
            action: function(settings, event) {
                tbContr._getSelectorScope().changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
            }
        },
        'movedown': {
            title: "Toolbar.MoveDown",
            iclass: "icon-sxc-move-down",
            lightbox: false,
            hideFirst: true,
            disabled: false,
            showOn: "edit",
            action: function(settings, event) {
                tbContr._getSelectorScope().changeOrder(settings.sortOrder, settings.sortOrder + 1);
            }
        },
        'sort': {
            title: "Toolbar.Sort",
            iclass: "icon-sxc-list-numbered",
            lightbox: true,
            hideFirst: true,
            showOn: "design",
            action: function (settings, event) {
                tbContr._openNgDialog(settings, event);
            }
        },
        'remove': {
            title: "Toolbar.Remove",
            iclass: "icon-sxc-minus-circled",
            lightbox: false,
            hideFirst: true,
            disabled: true,
            showOn: "edit",
            action: function (settings, event) {
                if (confirm(tbContr.translate("Toolbar.ConfirmRemove"))) {
                    tbContr._getSelectorScope().removeFromList(settings.sortOrder);
                }
            }
        },
        'develop': {
            title: "Toolbar.EditView", // todo
            iclass: "icon-sxc-code",
            lightbox: true,
            hideFirst: true,
            showOn: "admin",
            uiActionOnly: true, // so it doesn't create the content when used
            action: function (settings, event) {
                tbContr._openNgDialog(settings, event);
            }
        },
        'app': {
            title: "Toolbar.App",
            iclass: "icon-sxc-settings",
            lightbox: true,
            hideFirst: true,
            showOn: "admin",
            uiActionOnly: true, // so it doesn't create the content when used
            action: function (settings, event) {
                tbContr._openNgDialog(settings, event);
            }
        },
        'manage-apps': {
            title: "Toolbar.ManageApps",
            iclass: "icon-sxc-manage",
            lightbox: true,
            hideFirst: true,
            showOn: "admin",
            uiActionOnly: true, // so it doesn't create the content when used
            action: function (settings, event) {
                tbContr._openNgDialog(settings, event);
            }
        },
        "more": {
            title: "Toolbar.MoreActions",
            iclass: "icon-sxc-options btn-mode",
            borlightboxder: false,
            hideFirst: false,
            showOn: "default,edit,design,admin",
            uiActionOnly: true, // so it doesn't create the content when clicked
            action: function (settings, event) {
                var fullMenu = $(event.target).closest("ul.sc-menu");
                var state = Number(fullMenu.attr("data-state") || 0);
                var newState = (state + 1) % (enableTools ? 4 : 3); // if tools are enabled, there are 4 states

                fullMenu.removeClass("show-set-" + state)
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

            if(["replace", "app", "zone", "sort", "develop"].indexOf(settings.action) !== -1)
                params.dialog = settings.action;

            if (settings.action === "develop")
                items = [{ EntityId: manageInfo.templateId }];

            // when doing new, there may be a prefill in the link to initialize the new item
            if (settings.prefill) {
                for (var i = 0; i < items.length; i++)
                    items[i].Prefill = settings.prefill;
            }

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
                tbContr._getSelectorScope().reload();
                closeCallback();
            };
            var link = tbContr.getNgLink(settings);

            if (event && event.shiftKey)
                window.open(link);
            else
                $2sxc.totalPopup.open(link, callback);
        },

        // Perform a toolbar button-action - basically get the configuration and execute it's action
        action: function(settings, event) {
            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            var conf = actionButtonsConf[settings.action] || actionButtonsConf.default;
            if (conf.uiActionOnly)
                return conf.action(settings, origEvent);
            else
                // if more than just a UI-action, then it needs to be sure the content-group is created first
                tbContr._getSelectorScope().prepareToAddContent().then(function () {
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
                    settings.useModuleList = false;
                }
                if (settings.entity._2sxcEditInformation.sortOrder) {
                    settings.sortOrder = settings.entity._2sxcEditInformation.sortOrder;
                    settings.useModuleList = true;
                }
                delete settings.entity;
            }

            // retrieve configuration for this button
            var conf = actionButtonsConf[settings.action] || actionButtonsConf.default;

            var showClasses = "";
            var classesList = conf.showOn.split(",");
            for (var c = 0; c < classesList.length; c++)
                showClasses += " show-" + classesList[c];
            var button = $("<a />", {
                'class': "sc-" + settings.action + " "
                    + " " + (conf.lightbox ? "box" : "")
                    + showClasses,
                'onclick': "javascript:$2sxc(" + id + ").manage.action(" + JSON.stringify(settings) + ", event);",
                'title': tbContr.translate(conf.title)
            });
            var box = $("<div/>");
            var symbol = $("<i class=\"" + conf.iclass + "\" aria-hidden=\"true\"></i>");

            // if publish-button and not published yet, show button (otherwise hidden) & change icon
            if (settings.action === "publish" && settings.isPublished === false) {
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
            var buttons = [];

            if (settings.action) {
                // if single item with specified action, use this as our button-list
                buttons = [settings];
            } else if ($.isArray(settings)) {
                // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                buttons = settings;
            } else {
                // Create a standard menu with all standard buttons
                // first button: edit
                buttons.push($.extend({}, settings, { action: "edit" }));

                // add applicable list buttons - add=add item below; new=lightbox-dialog
                if (toolbarConfig.isList && settings.sortOrder !== -1) { // if list and not the list-header
                    buttons.push($.extend({}, settings, { action: "new" }));
                    if (settings.useModuleList) 
                        buttons.push($.extend({}, settings, { action: "add" }));

                    // only provide remove on lists
                    buttons.push($.extend({}, settings, { action: "remove" }));

                    if (settings.useModuleList) {
                        if (settings.sortOrder !== 0)
                            buttons.push($.extend({}, settings, { action: "moveup" }));
                        buttons.push($.extend({}, settings, { action: "movedown" }));
                    }
                    buttons.push($.extend({}, settings, { action: "sort" }));
                }
                buttons.push($.extend({}, settings, { action: "publish" }));

                // the replace button only makes sense if it's a content-group
                if (settings.useModuleList)
                    buttons.push($.extend({}, settings, { action: "replace" }));
                
                buttons.push($.extend({}, settings, { action: "layout" }));
                if (enableTools) {
                    buttons.push($.extend({}, settings, { action: "app" }));
                    buttons.push($.extend({}, settings, { action: "develop" }));
                    if (!isContent) 
                        buttons.push($.extend({}, settings, { action: "manage-apps" }));
                }
                buttons.push($.extend({}, settings, { action: "more" }));
            }

            var tbClasses = "sc-menu show-set-0" + ((settings.sortOrder === -1) ? " listContent" : "");
            var toolbar = $("<ul />", { 'class': tbClasses, 'onclick': "javascript: var e = arguments[0] || window.event; e.stopPropagation();" });

            for (var i = 0; i < buttons.length; i++)
                toolbar.append($("<li />").append($(tbContr.getButton(buttons[i]))));

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
        },

        translate: function(key) {
            return tbContr._getSelectorScope().translate(key);
        }

    };

    return tbContr;
};