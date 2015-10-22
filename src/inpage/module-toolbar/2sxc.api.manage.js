// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it

// all in-page toolbars etc.
$2sxc.getManageController = function (id) {

    var moduleElement = $(".DnnModule-" + id);
    //var manageInfo = $.parseJSON(moduleElement.find(".Mod2sxcC, .Mod2sxcappC").attr("data-2sxc")).manage;
    //var sxcGlobals = $.parseJSON(moduleElement.find(".Mod2sxcC, .Mod2sxcappC").attr("data-2sxc-globals"));
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
        approot: manageInfo.appPath + "/" // this is the only value which doesn't have a slash by default
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
        'sort': {
            title: "Sort",
            icon: "glyphicon-sort",
            lightbox: true,
            hideFirst: true,
            action: function (settings, event) {
                manageController._openNgDialog(settings, event);
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

            if (settings.action === "replace" || settings.action === "app" || settings.action === "zone" || settings.action === "sort")
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
                if (toolbarConfig.isList && settings.sortOrder !== -1) { // if list and not the list-header
                    buttons.push($.extend({}, settings, { action: "new" }));
                    if (settings.useModuleList) {
                        buttons.push($.extend({}, settings, { action: "add" }));
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