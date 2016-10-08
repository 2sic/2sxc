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
    var thisObj = $2sxc._actions.create = function (actionParams) {
        var enableTools = actionParams.canDesign;

        var act = {
            // show the basic dashboard which allows view-changing
            "dash-view": createActionConfig("dash", "Dashboard", "", "", true, { inlineWindow: true }),
            "app-import": createActionConfig("app-import", "Dashboard", "", "", true, {}),
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
                code: function (settings, event, manager) {
                    // todo - should refactor this to be a toolbarManager.contentBlock command
                    manager.commands._openNgDialog($2sxc._lib.extend({}, settings, { sortOrder: settings.sortOrder + 1 }), event);
                }
            }),
            // add brings no dialog, just add an empty item
            'add': createActionConfig("add", "AddDemo", "plus-circled", "edit", false, {
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
                code: function (settings, event, manager) {
                    manager.contentBlock 
                        .addItem(settings.sortOrder + 1);
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
                code: function (settings, event, manager) {
                    if (confirm($2sxc.translate("Toolbar.ConfirmRemove"))) {
                        manager.contentBlock
                            .removeFromList(settings.sortOrder);
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
                code: function (settings, event, manager) {
                    manager.contentBlock
                        .changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
                }
            },
            'movedown': {
                title: "Toolbar.MoveDown",
                iclass: "icon-sxc-move-down",
                disabled: false,
                showOn: "edit",
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
                code: function (settings, event, manager) {
                    manager.contentBlock.changeOrder(settings.sortOrder, settings.sortOrder + 1);
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
                code: function (settings, event, manager) {
                    if (settings.isPublished) {
                        alert($2sxc.translate("Toolbar.AlreadyPublished"));
                        return;
                    }
                    var part = settings.sortOrder === -1 ? "listcontent" : "content";
                    var index = settings.sortOrder === -1 ? 0 : settings.sortOrder;
                    manager.contentBlock.publish(part, index);
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
                code: function (settings, event, manager) {
                    manager.contentBlock.dialogToggle();
                }
            },
            'develop': createActionConfig("develop", "Develop", "code", "admin", true, {
                newWindow: true,
                addCondition: enableTools,
                configureCommand: function (cmd) {
                    cmd.items = [{ EntityId: actionParams.templateId }];
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
                params: { contentTypeName: actionParams.contentTypeId },
                uiActionOnly: true, // so it doesn't create the content when used
                addCondition: enableTools && actionParams.contentTypeId,
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
                code: function (settings, event) {
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


(function(){
$2sxc._contentManagementCommands = function (sxc, targetTag) {
    var cmc = {
        editManager: "must-be-added-after-initialization",
        init: function(editor) {
            cmc.editManager = editor;
        },

        // assemble an object which will store the configuration and execute it
        create: function(specialSettings) {
            var settings = $2sxc._lib.extend({}, cmc.editManager.toolbarConfig, specialSettings); // merge button with general toolbar-settings
            var ngDialogUrl = cmc.editManager.editContext.Environment.SxcRootUrl + "desktopmodules/tosic_sexycontent/dist/dnn/ui.html?sxcver="
                + cmc.editManager.editContext.Environment.SxcVersion;
            var isDebug = $2sxc.urlParams.get("debug") ? "&debug=true" : "";

            var cmd = {
                settings: settings,
                items: settings.items || [], // use predefined or create empty array
                params: $2sxc._lib.extend({
                    dialog: settings.dialog || settings.action // the variable used to name the dialog changed in the history of 2sxc from action to dialog
                }, settings.params),

                addSimpleItem: function() {
                    var itm = {}, ct = cmd.settings.contentType || cmd.settings.attributeSetName; // two ways to name the content-type-name this, v 7.2+ and older
                    if (cmd.settings.entityId) itm.EntityId = cmd.settings.entityId;
                    if (ct) itm.ContentTypeName = ct;
                    if (itm.EntityId || itm.ContentTypeName) // only add if there was stuff to add
                        cmd.items.push(itm);
                },

                // this adds an item of the content-group, based on the group GUID and the sequence number
                addContentGroupItem: function(guid, index, part, isAdd, isEntity, cbid, sectionLanguageKey) {
                    cmd.items.push({
                        Group: { Guid: guid, Index: index, Part: part, Add: isAdd },
                        Title: $2sxc.translate(sectionLanguageKey)
                    });
                },

                // this will tell the command to edit a item from the sorted list in the group, optionally together with the presentation item
                addContentGroupItemSetsToEditList: function(withPresentation) {
                    var isContentAndNotHeader = (cmd.settings.sortOrder !== -1);
                    var index = isContentAndNotHeader ? cmd.settings.sortOrder : 0;
                    var prefix = isContentAndNotHeader ? "" : "List";
                    var cTerm = prefix + "Content";
                    var pTerm = prefix + "Presentation";
                    var isAdd = cmd.settings.action === "new";
                    var groupId = cmd.settings.contentGroupId;
                    cmd.addContentGroupItem(groupId, index, cTerm.toLowerCase(), isAdd, cmd.settings.cbIsEntity, cmd.settings.cbId, "EditFormTitle." + cTerm);

                    if (withPresentation)
                        cmd.addContentGroupItem(groupId, index, pTerm.toLowerCase(), isAdd, cmd.settings.cbIsEntity, cmd.settings.cbId, "EditFormTitle." + pTerm);

                },

                generateLink: function() {
                    // if there is no items-array, create an empty one (it's required later on)
                    if (!cmd.settings.items) cmd.settings.items = [];
                    //#region steps for all actions: prefill, serialize, open-dialog
                    // when doing new, there may be a prefill in the link to initialize the new item
                    if (cmd.settings.prefill)
                        for (var i = 0; i < cmd.items.length; i++)
                            cmd.items[i].Prefill = cmd.settings.prefill;

                    cmd.params.items = JSON.stringify(cmd.items); // Serialize/json-ify the complex items-list

                    return ngDialogUrl
                        + "#" + $.param(cmc.editManager.dialogParameters)
                        + "&" + $.param(cmd.params)
                        + isDebug;
                    //#endregion
                }
            };
            return cmd;
        },

        // create a dialog link
        _linkToNgDialog: function(specialSettings) {
            var cmd = cmc.editManager.commands.create(specialSettings);

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
        _openNgDialog: function (settings, event, closeCallback) {

            var callback = function () {
                cmc.editManager.contentBlock.reloadAndReInitialize();
                closeCallback();
            };
            var link = cmc._linkToNgDialog(settings);

            if (settings.newWindow || (event && event.shiftKey))
                return window.open(link);
            else {
                if (settings.inlineWindow)
                    return $2sxc._dialog.create(sxc, targetTag, link, callback);
                else
                    return $2sxc.totalPopup.open(link, callback);
            }
        },

        executeAction: function (settings, event) {
            var conf = cmc.editManager.toolbar.actions[settings.action];
            settings = $2sxc._lib.extend({}, conf, settings); // merge conf & settings, but settings has higher priority
            if (!settings.dialog) settings.dialog = settings.action; // old code uses "action" as the parameter, now use verb ? dialog
            if (!settings.code) settings.code = cmc._openNgDialog; // decide what action to perform

            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            if (conf.uiActionOnly)
                return settings.code(settings, origEvent, cmc.editManager);

            // if more than just a UI-action, then it needs to be sure the content-group is created first
            cmc.editManager.contentBlock.prepareToAddContent()
                .then(function () {
                    return settings.code(settings, origEvent, cmc.editManager);
                });
        },
    };

    return cmc;
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

(function() {
    $2sxc._toolbarManager = function (sxc, editContext) {
        var id = sxc.id, cbid = sxc.cbid;
        var actionParams = {
            canDesign: editContext.User.CanDesign,
            templateId: editContext.ContentGroup.TemplateId,
            contentTypeId: editContext.ContentGroup.ContentTypeName
        };

        function createToolbarConfig() {
            var toolbarConfig = {
                portalId: editContext.Environment.WebsiteId,
                tabId: editContext.Environment.PageId,
                moduleId: editContext.Environment.InstanceId,
                version: editContext.Environment.SxcVersion,

                contentGroupId: editContext.ContentGroup.Guid, // todo 8.4
                cbIsEntity: editContext.ContentBlock.IsEntity,
                cbId: editContext.ContentBlock.Id,
                appPath: editContext.ContentGroup.AppUrl,
                isList: editContext.ContentGroup.IsList,
            };
            return toolbarConfig;
        }

        var allActions = $2sxc._actions.create(actionParams);

        var tb = {
            config: createToolbarConfig(),
            refreshConfig: function() { tb.config = createToolbarConfig(); },
            actions: allActions,
            // Generate a button (an <a>-tag) for one specific toolbar-action. 
            // Expects: settings, an object containing the specs for the expected buton
            getButton: function(actDef) {
                // if the button belongs to a content-item, move the specs up to the item into the settings-object
                if (actDef.entity && actDef.entity._2sxcEditInformation) {
                    var editInfo = actDef.entity._2sxcEditInformation;
                    actDef.useModuleList = (editInfo.sortOrder !== undefined); // has sort-order, so use list
                    if (editInfo.entityId !== undefined)
                        actDef.entityId = editInfo.entityId;
                    if (editInfo.sortOrder !== undefined)
                        actDef.sortOrder = editInfo.sortOrder;
                    delete actDef.entity;   // clean up edit-info
                }

                // retrieve configuration for this button
                var conf = allActions[actDef.action],
                    showClasses = "",
                    classesList = conf.showOn.split(","),
                    box = $("<div/>"),
                    symbol = $("<i class=\"" + conf.iclass + "\" aria-hidden=\"true\"></i>");

                for (var c = 0; c < classesList.length; c++)
                    showClasses += " show-" + classesList[c];

                var button = $("<a />", {
                    'class': "sc-" + actDef.action + " " + showClasses + (conf.dynamicClasses ? " " + conf.dynamicClasses(actDef) : ""),
                    'onclick': "javascript:$2sxc(" + id + ", " + cbid + ").manage.action(" + JSON.stringify(actDef) + ", event);",
                    'data-i18n': "[title]" + conf.title
                    //'title': $2sxc.translate(conf.title)
                });

                // todo: move the following lines into the button-config and just call from here
                // if publish-button and not published yet, show button (otherwise hidden) & change icon
                if (actDef.action === "publish" && actDef.isPublished === false) {
                    button.addClass("show-default").removeClass("show-edit")
                        .attr("data-i18n", "[title]Toolbar.Unpublished");
                        //.attr("title", $2sxc.translate("Toolbar.Unpublished"));
                    symbol.removeClass(conf.iclass).addClass(conf.iclass2);
                }

                button.html(box.html(symbol));

                return button[0].outerHTML;
            },

            // Assemble a default toolbar instruction set
            createDefaultToolbar: function(settings) {
                // Create a standard menu with all standard buttons
                var buttons = [];

                buttons.add = function(verb) {
                    var add = allActions[verb].addCondition;
                    if (add === undefined || ((typeof (add) === "function") ? add(settings, tb.config) : add))
                        buttons.push($2sxc._lib.extend({}, settings, { action: verb }));
                };

                for (var btn in allActions)
                    if (allActions.hasOwnProperty(btn))
                        buttons.add(btn);

                return buttons;
            },

            // Builds the toolbar and returns it as HTML
            // expects settings - either for 1 button or for an array of buttons
            getToolbar: function(settings) {
                var actionList = settings.action
                    ? [settings] // if single item with specified action, use this as our button-list
                    : $.isArray(settings)
                        ? settings // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                        : tb.createDefaultToolbar(settings);

                var tbClasses = "sc-menu show-set-0" + ((settings.sortOrder === -1) ? " listContent" : "");
                var toolbar = $("<ul />", { 'class': tbClasses, 'onclick': "javascript: var e = arguments[0] || window.event; e.stopPropagation();" });

                for (var i = 0; i < actionList.length; i++)
                    toolbar.append($("<li />").append($(tb.getButton(actionList[i]))));

                return toolbar[0].outerHTML;
            },

            // find all toolbar-info-attributes in the HTML, convert to <ul><li> toolbar
            _processToolbars: function (parentTag) {
                parentTag = parentTag ? $(parentTag) : $(".DnnModule-" + id);
                $(".sc-menu[data-toolbar]", parentTag).each(function() {
                    var toolbarSettings = $.parseJSON($(this).attr("data-toolbar"));
                    var toolbarTag = $(this);
                    toolbarTag.replaceWith($2sxc(toolbarTag).manage.getToolbar(toolbarSettings));
                });
            },


        };
        return tb;
    };
})();
(function() {
    var initialized = false;
    $2sxc.translate = function(key) {
        return $.t(key);
    };
    //#endregion

    $2sxc._initTranslate = function(manage) {
        if (!initialized) {
            window.i18next
                .use(window.i18nextXHRBackend)
                .init({
                    lng: manage.editContext.Language.Current.substr(0,2), // "en",
                    fallbackLng: "en",
                    whitelist: ["en", "de", "fr", "it", "uk", "nl"],
                    preload: ["en"],
                    backend: {
                        loadPath: manage.editContext.Environment.SxcRootUrl + "desktopmodules/tosic_sexycontent/dist/i18n/inpage-{{lng}}.js"
                    }
                }, function (err, t) {
                    // for options see
                    // https://github.com/i18next/jquery-i18next#initialize-the-plugin
                    jqueryI18next.init(i18next, $);
                    // start localizing, details:
                    // https://github.com/i18next/jquery-i18next#usage-of-selector-function
                    $('ul.sc-menu').localize(); // inline toolbars
                    $('.sc-i18n').localize();   // quick-insert menus
                });
            initialized = true;
        }
    };
})();

/* 
 * this is a content block in the browser
 * 
 * A Content Block is a standalone unit of content, with it's own definition of
 * 1. content items
 * 2. template
 * + some other stuff
 *
 * it should be able to render itself
 */

$2sxc.contentBlock = function (sxc, manage, cbTag) {
    //#region loads of old stuff, should be cleaned, mostly just copied from the angulare coe

    var cViewWithoutContent = "_LayoutElement"; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty
    var editContext = manage.editContext;
    var ctid = (editContext.ContentGroup.ContentTypeName === "" && editContext.ContentGroup.TemplateId !== null)
        ? cViewWithoutContent // has template but no content, use placeholder
        : editContext.ContentGroup.ContentTypeName;// manageInfo.contentTypeId;

    //#endregion

    var cb = {
        sxc: sxc,
        editContext: editContext,    // todo: not ideal depedency, but ok...

        templateId: editContext.ContentGroup.TemplateId,
        undoTemplateId: editContext.ContentGroup.TemplateId,
        contentTypeId: ctid,
        undoContentTypeId: ctid,
        buttonsAreLoaded: true,

        // ajax update/replace the content of the content-block
        replace: function (newContent, justPreview) {
            try {
                var newStuff = $(newContent);
                // don't do this yet, too many side-effects
                //if (justPreview) {    
                //    newStuff.attr("data-cb-id", "preview" + newStuff.attr("data-cb-id"));
                //    newStuff.Attr("data-cb-preview", true);
                //}
                $(cbTag).replaceWith(newStuff);
                cbTag = newStuff;
                cb.buttonsAreLoaded = false;
                //$2sxc(newStuff).manage.toolbar._processToolbars(newStuff); // init it...
            } catch (e) {
                console.log("Error while rendering template:");
                console.log(e);
            }
        },
        replacePreview: function (newContent) {
            cb.replace(newContent, true);
        },

        // this one assumes a replace / change has already happened, but now must be finalized...
        reloadAndReInitialize: function (forceAjax) {
            // force ajax is set when a new app was chosen, and the new app supports ajax
            // this value can only be true, or not exist at all
            if (forceAjax)
                manage.reloadWithAjax = true;

            if (manage.reloadWithAjax) // necessary to show the original template again
                return (forceAjax
                    ? cb.reload(-1) // -1 is important to it doesn't try to use the old templateid
                    : cb.reload())
                    .then(function () {
                        if (manage.reloadWithAjax && sxc.manage.dialog) sxc.manage.dialog.destroy(); // only remove on force, which is an app-change
                        // create new sxc-object
                        cb.sxc = cb.sxc.recreate();
                        cb.sxc.manage.toolbar._processToolbars(); // sub-optimal deep dependency
                        cb.buttonsAreLoaded = true;
                    });
            else
                return window.location.reload();

        },

        // retrieve new preview-content with alternate template and then show the result
        reload: function (templateId) {
            // if nothing specified, use stored id
            if (!templateId)
                templateId = cb.templateId;

            // if nothing specified / stored, cancel
            if (!templateId)
                return null;

            // if reloading a non-content-app, re-load the page
            if (!manage.reloadWithAjax) // special code to force ajax-app-change
                return window.location.reload();

            // remember for future persist/save/undo
            cb.templateId = templateId;

            // ajax-call, then replace
            return cb._getPreviewWithTemplate(templateId)
                .then(cb.replace);
        },

        //#region simple item commands like publish, remove, add, re-order
        // set a content-item in this block to published, then reload
        publish: function (part, sortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/publish",
                params: { part: part, sortOrder: sortOrder }
            }).then(cb.reloadAndReInitialize);
        },

        // remove an item from a list, then reload
        removeFromList: function (sortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/removefromlist",
                params: { sortOrder: sortOrder }
            }).then(cb.reloadAndReInitialize);
        },

        // change the order of an item in a list, then reload
        changeOrder: function (sortOrder, destinationSortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/changeorder",
                params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder }
            }).then(cb.reloadAndReInitialize);
        },


        addItem: function (sortOrder) {
            return cb.sxc.webApi.get({
                url: "View/Module/AddItem",
                params: { sortOrder: sortOrder }
            }).then(cb.reloadAndReInitialize);
        },
        //#endregion

        _getPreviewWithTemplate: function (templateId) {
            return cb.sxc.webApi.get({
                url: "view/module/rendertemplate",
                params: {
                    templateId: templateId,
                    lang: cb.editContext.Language.Current,
                    cbisentity: editContext.ContentBlock.IsEntity,
                    cbid: editContext.ContentBlock.Id,
                    originalparameters: JSON.stringify(editContext.Environment.parameters)
        },
                dataType: "html"
            });
        },

        _setTemplateChooserState: function (state) {
            return cb.sxc.webApi.get({
                url: "view/module/SetTemplateChooserState",
                params: { state: state }
            });
        },

        _saveTemplate: function (templateId, forceCreateContentGroup, newTemplateChooserState) {
            return cb.sxc.webApi.get({
                url: "view/module/savetemplateid",
                params: {
                    templateId: templateId,
                    forceCreateContentGroup: forceCreateContentGroup,
                    newTemplateChooserState: newTemplateChooserState
                }
            });
        },

        // Cancel and reset back to original state
        _cancelTemplateChange: function () {
            cb.templateId = cb.undoTemplateId;
            cb.contentTypeId = cb.undoContentTypeId;

            // dialog...
            sxc.manage.dialog.justHide();
            cb._setTemplateChooserState(false)
                .then(cb.reloadAndReInitialize);
        },

        dialogToggle: function () {
            // check if the dialog already exists, if yes, use that
            // it can already exist as part of the manage-object, 
            // ...or if the manage object was reset, we must find it in the DOM

            var diag = manage.dialog;
            if (!diag) {
                // todo: look for it in the dom
            }
            if (!diag) {
                // still not found, create it
                diag = manage.dialog = manage.action({ "action": "dash-view" }); // not ideal, must improve

            } else {
                diag.toggle();
            }

            var isVisible = diag.isVisible();
            if (manage.editContext.ContentBlock.ShowTemplatePicker !== isVisible)
                cb._setTemplateChooserState(isVisible)
                    .then(function () {
                        manage.editContext.ContentBlock.ShowTemplatePicker = isVisible;
                    });

        },


        prepareToAddContent: function () {
            return cb.persistTemplate(true, false);
        },

        persistTemplate: function (forceCreate, selectorVisibility) {
            // Save only if the currently saved is not the same as the new
            var groupExistsAndTemplateUnchanged = !!cb.editContext.ContentGroup.HasContent
                && (cb.undoTemplateId === cb.templateId);
            var promiseToSetState;
            if (groupExistsAndTemplateUnchanged)
                promiseToSetState = (cb.editContext.ContentBlock.ShowTemplatePicker)//.minfo.templateChooserVisible)
                    ? cb._setTemplateChooserState(false) // hide in case it was visible
                    : $.when(null); // all is ok, create empty promise to allow chaining the result
            else
                promiseToSetState = cb._saveTemplate(cb.templateId, forceCreate, selectorVisibility)
                    .then(function (data, textStatus, xhr) {
                        if (xhr.status !== 200) { // only continue if ok
                            alert("error - result not ok, was not able to create ContentGroup");
                            return;
                        }
                        var newGuid = data;
                        if (!newGuid) return;
                        newGuid = newGuid.replace(/[\",\']/g, ""); // fixes a special case where the guid is given with quotes (dependes on version of angularjs) issue #532
                        if (console) console.log("created content group {" + newGuid + "}");

                        manage.updateContentGroupGuid(newGuid);
                    });

            var promiseToCorrectUi = promiseToSetState.then(function () {
                cb.undoTemplateId = cb.templateId; // remember for future undo
                cb.undoContentTypeId = cb.contentTypeId; // remember ...

                cb.editContext.ContentBlock.ShowTemplatePicker = false; // cb.minfo.templateChooserVisible = false;

                if (manage.dialog)
                    manage.dialog.justHide();

                if (!cb.editContext.ContentGroup.HasContent) // if it didn't have content, then it only has now...
                    cb.editContext.ContentGroup.HasContent = forceCreate;

                // only re-load on content, not on app as that was already re-loaded on the preview
                if (!cb.buttonsAreLoaded || (!groupExistsAndTemplateUnchanged && manage.reloadWithAjax))      // necessary to show the original template again
                    cb.reloadAndReInitialize();
            });

            return promiseToCorrectUi;
        }


    };

    return cb;
};


// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it
// all in-page toolbars etc.
(function () {
    //#region helper functions
    function getContentBlockTag(sxci) {
         return $("div[data-cb-id='" + sxci.cbid + "']")[0];
    }

    function getContextInfo(cb) {
        var attr = cb.getAttribute("data-edit-context");
        return $.parseJSON(attr || "");
    }
    //#endregion


    $2sxc.getManageController = function (sxc) {
        var cbTag = getContentBlockTag(sxc);
        var ec = getContextInfo(cbTag);

        // assemble all parameters needed for the dialogs if we open anything
        var ngDialogParams = {
            zoneId: ec.ContentGroup.ZoneId,
            appId: ec.ContentGroup.AppId,
            tid: ec.Environment.PageId,
            mid: ec.Environment.InstanceId,
            cbid: sxc.cbid,
            lang: ec.Language.Current,
            langpri: ec.Language.Primary,
            langs: JSON.stringify(ec.Language.All),
            portalroot: ec.Environment.WebsiteUrl,
            websiteroot: ec.Environment.SxcRootUrl,
            // todo: probably move the user into the dashboard info
            user: { canDesign: ec.User.CanDesign, canDevelop: ec.User.CanDesign },
            approot: ec.ContentGroup.AppUrl || null // this is the only value which doesn't have a slash by default.  note that the app-root doesn't exist when opening "manage-app"
        };

        var dashConfig = {
            appId: ec.ContentGroup.AppId,
            isContent: ec.ContentGroup.IsContent,
            hasContent: ec.ContentGroup.HasContent,
            isList: ec.ContentGroup.IsList,
            templateId: ec.ContentGroup.TemplateId,
            contentTypeId: ec.ContentGroup.ContentTypeName,
            templateChooserVisible: ec.ContentBlock.ShowTemplatePicker, // todo: maybe move to content-goup
            user: { canDesign: ec.User.CanDesign, canDevelop: ec.User.CanDesign },
            supportsAjax: ec.ContentGroup.SupportsAjax
        };

        var toolsAndButtons = $2sxc._toolbarManager(sxc, ec);
        var cmds = $2sxc._contentManagementCommands(sxc, cbTag);

        var editManager = {
            // public method to find out if it's in edit-mode
            isEditMode: function () { return ec.Environment.IsEditable; },
            reloadWithAjax: ec.ContentGroup.SupportsAjax,  // for now, allow all content to use ajax, apps use page-reload

            dialogParameters: ngDialogParams, // used for various dialogs
            toolbarConfig: toolsAndButtons.config, // used to configure buttons / toolbars

            editContext: ec, // metadata necessary to know what/how to edit
            dashboardConfig: dashConfig,
            commands: cmds,

            // Perform a toolbar button-action - basically get the configuration and execute it's action
            action: cmds.executeAction,

            //#region toolbar quick-access commands - might be used by other scripts, so I'm keeping them here for the moment, but may just delete them later
            toolbar: toolsAndButtons, // should use this from now on when accessing from outside
            getButton: toolsAndButtons.getButton,
            createDefaultToolbar: toolsAndButtons.createDefaultToolbar,
            getToolbar: toolsAndButtons.getToolbar,
            //#endregion

            // init this object 
            init: function init() {
                // enhance UI in case there are known errors / issues
                if (ec.error.type)
                    editManager.handleErrors(ec.error.type, cbTag);

                // finish init of sub-objects
                editManager.commands.init(editManager);
                editManager.contentBlock = $2sxc.contentBlock(sxc, editManager, cbTag);

                // attach & open the mini-dashboard iframe
                if (!ec.error.type && ec.ContentBlock.ShowTemplatePicker)
                    editManager.action({ "action": "layout" });

            },
            handleErrors: function (errType, cbTag) {
                var errWrapper = $("<div class=\"dnnFormMessage dnnFormWarning sc-element\"></div>");
                var msg = "";
                var toolbar = $("<ul class='sc-menu'></ul>");
                var actions = [];
                if (errType === "DataIsMissing") {
                    msg = "Error: System.Exception: Data is missing - usually when a site is copied but the content / apps have not been imported yet - check 2sxc.org/help?tag=export-import";
                    actions = ["zone", "more"];
                    toolbar.attr("data-toolbar", '[{\"action\": \"zone\"}, {\"action\": \"more\"}]');
                }
                errWrapper.append(msg);
                errWrapper.append(toolbar);
                $(cbTag).append(errWrapper);
            },
            // change config by replacing the guid, and refreshing dependend sub-objects
            updateContentGroupGuid: function (newGuid) {
                ec.ContentGroup.Guid = newGuid;
                toolsAndButtons.refreshConfig(); 
                editManager.toolbarConfig = toolsAndButtons.config;
            },

            createContentBlock: function (parentId, fieldName, index, appName, container) {
                // the wrapper, into which this will be placed and the list of pre-existing blocks
                var listTag = container;
                if (listTag.length === 0) return alert("can't add content-block as we couldn't find the list");
                var cblockList = listTag.find("div.sc-content-block");
                if (index > cblockList.length)
                    index = cblockList.length; // make sure index is never greater than the amount of items
                return sxc.webApi.get({
                    url: "view/module/generatecontentblock",
                    params: { parentId: parentId, field: fieldName, sortOrder: index, app: appName }
                }).then(function (result) {
                    var newTag = $(result); // prepare tag for inserting

                    // should I add it to a specific position...
                    if (cblockList.length > 0 && index > 0) 
                        $(cblockList[cblockList.length > index - 1 ? index - 1: cblockList.length - 1])
                            .after(newTag);
                    else    //...or just at the beginning?
                        listTag.prepend(newTag);
                

                    var sxcNew = $2sxc(newTag);
                    sxcNew.manage.toolbar._processToolbars(newTag);

                });
            },

            moveContentBlock: function(parentId, field, indexFrom, indexTo) {
                return sxc.webApi.get({
                    url: "view/module/MoveItemInList",
                    params: { parentId: parentId, field: field, indexFrom: indexFrom, indexTo: indexTo }
                }).then(function() {
                    console.log("done moving!");
                    window.location.reload();
                });
            },

            // delete a content-block inside a list of content-blocks
            deleteContentBlock: function (parentId, field, index) {
                if (confirm($2sxc.translate("QuickInsertMenu.ConfirmDelete")))
                    return sxc.webApi.get({
                        url: "view/module/RemoveItemInList",
                        params: { parentId: parentId, field: field, index: index }
                    }).then(function() {
                        console.log("done deleting!");
                        window.location.reload();
                    });
                return null;
            }


        };

        editManager.init();
        return editManager;
    };


})();
(function () {

    var diag = $2sxc._dialog = {
        mode: "iframe",
        templates: {
            inline: "<iframe width='100%' height='100px' src='{{url}}'"
            // + "onload=\"this.syncHeight(this.contentWindow.document.body.scrollHeight);\""
            //+ "onload=\"this.style.height=this.contentWindow.document.body.scrollHeight + 'px';\""
            + "onresize=\"console.log('resize')\""
            + "></iframe>"
        }
    };

    diag.create = function (sxc, wrapperTag, url, closeCallback) {
        var diagBox = $(diag.templates.inline.replace("{{url}}", url))[0];    // build iframe tag

        diagBox.closeCallback = closeCallback;
        diagBox.sxc = sxc;
        // diagBox.attr("data-for-manual-debug", "app: " + sxc.manage.ContentGroup.AppUrl);

        //#region data bridge both ways
        diagBox.getManageInfo = function() {    return diagBox.sxc.manage.dialogParameters; };

        diagBox.getAdditionalDashboardConfig = function () {
            return diagBox.sxc.manage.dashboardConfig;
        };

        diagBox.getCommands = function() { return diagBox.vm;  };// created by inner code

        //#endregion

        //#region sync size
        diagBox.syncHeight = function () {
            var height = diagBox.contentDocument.body.offsetHeight;
            if (diagBox.previousHeight === height)
                return;
            window.diagBox = diagBox;
            diagBox.height = height + 'px';
            diagBox.previousHeight = height;
        };

        function loadEventListener()  {
            diagBox.syncHeight();
            diagBox.resizeInterval = window.setInterval(diagBox.syncHeight, 200); // Not ideal - polling the document height may cause performance issues
            //diagBox.contentDocument.body.addEventListener('resize', function () { diagBox.syncHeight(); }, true); // The resize event is not called reliable when the iframe content changes
        }
        diagBox.addEventListener('load', loadEventListener);

        //#endregion

        //#region Visibility toggle & status

        diagBox.isVisible = function() { return diagBox.style.display !== "none";   };

        diagBox.toggle = function () { diagBox.style.display = diagBox.style.display === "none" ? "" : "none"; };

        diagBox.justHide = function () { diagBox.style.display = "none"; };
        //#endregion

        // remove the diagBox - important when replacing the app with ajax, and the diag needs to be re-initialized
        diagBox.destroy = function () {
            window.clearInterval(diagBox.resizeInterval);   // clear this first, to prevent errors
            diagBox.remove(); // use the jquery remove for this
        };

        $(wrapperTag).before(diagBox);

        return diagBox;
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

// Toolbar bootstrapping (initialize all toolbars after loading page)
$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $(".sc-menu").click(function (e) {
        e.stopPropagation();
    });

    var modules = $("div[data-edit-context]");

    if (console) console.log("found " + modules.length + " content blocks");

    // Ensure the _processToolbar is called after the next event cycle to make sure that the Angular app (template selector) is loaded first
    window.setTimeout(function () {
        modules.each(function () {
            try {
                $2sxc(this).manage.toolbar._processToolbars(this);
            } catch (e) { // Make sure that if one app breaks, others continue to work
                if (console && console.error) console.error(e);
            }
        });
    }, 0);

    window.EavEditDialogs = [];
});

$(function () {
    "use strict";

    //var enableModuleMove = false; // not implemented yet
    var selectors = {
        cb: {
            id: "cb",
            "class": "sc-content-block",
            selector: ".sc-content-block",
            listSelector: ".sc-content-block-list",
            context: "data-list-context"
        },
        mod: {
            id: "mod",
            "class": "DnnModule",
            selector: ".DnnModule",
            listSelector: ".DNNEmptyPane, .dnnDropEmptyPanes, :has(>.DnnModule)", // Found no better way to get all panes - the hidden variable does not exist when not in edit page mode
            context: null
        },
        eitherCbOrMod: ".DnnModule, .sc-content-block",
        selected: "sc-cb-is-selected"
    };

    var hasContentBlocks = ($(selectors.cb.listSelector).length > 0);

    function btn(action, icon, i18N, invisible, unavailable, classes) {
        return "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-" + icon + " " 
            + (invisible ? " sc-invisible " : "") 
            + (unavailable ? " sc-unavailable " : "")
            + classes + "' data-action='" + action + "' data-i18n='[title]QuickInsertMenu." + i18N + "'></a>";
    }

    // the quick-insert object
    var qi = {
        enableCb: hasContentBlocks, // for now, ContentBlocks are only enabled if they exist on the page
        enableMod: !hasContentBlocks,   // if it has inner-content, then it's probably a details page, where quickly adding modules would be a problem, so for now, disable modules in this case
        body: $("body"),
        win: $(window),
        main: $("<div class='sc-content-block-menu sc-content-block-quick-insert sc-i18n'></div>"),
        template: "<a class='sc-content-block-menu-addcontent sc-invisible' data-type='Default' data-i18n='[titleTemplate]QuickInsertMenu.AddBlockContent'>x</a>"
            + "<a class='sc-content-block-menu-addapp sc-invisible' data-type='' data-i18n='[titleTemplate]QuickInsertMenu.AddBlockApp'>x</a>"
            + btn("select", "ok", "Select", true)
            + btn("paste", "paste", "Paste", true, true),
        selected: $("<div class='sc-content-block-menu sc-content-block-selected-menu sc-i18n'></div>")
            .append(/*btn("cancel", "ok", "Cancel") + */ btn("delete", "trash-empty", "Delete")),
        contentBlocks: null,
        modules: null,
        nearestCb: null, 
        nearestMod: null
    };

    // add stuff which must be added in a second run
    $2sxc._lib.extend(qi, {
        cbActions: $(qi.template),
        modActions: $(qi.template.replace(/QuickInsertMenu.AddBlock/g, "QuickInsertMenu.AddModule")).attr("data-context", "module").addClass("sc-content-block-menu-module")
    });

    qi.init = function() {
        qi.body.append(qi.main);
        qi.body.append(qi.selected);

        // content blocks actions
        if (qi.enableCb)
            qi.main.append(qi.cbActions);

        // module actions
        if (qi.enableMod)
            qi.main.append(qi.modActions);
    };

    qi.init();

    qi.selected.toggle = function(target) {
        if (!target)
            return qi.selected.hide();

        var coords = qi.getCoordinates(target);
        coords.yh = coords.y + 20;
        qi.positionAndAlign(qi.selected, coords);
        qi.selected.target = target;
    };
    
    // give all actions
    $("a", qi.selected).click(function () {
        var action = $(this).data("action");
        var clip = qi.clipboard.data;
        switch(action) {
            case "cancel":
                return qi.clipboard.clear();
            case "delete":
                qi.cmds[clip.type].delete(clip);
        }
    });

    qi.cmds = {
        cb: {
            "delete": function(clip) {
                return $2sxc(clip.list).manage.deleteContentBlock(clip.parent, clip.field, clip.index);
            }
        },
        mod: {
            "delete": function (clip) {
                alert("module delete not implemented yet");
                // todo: get tabid and mod id, then call delete
                //if (confirm("delete?")) { // todo i18n
                //    var apiCmd = { url: "dnn/module/delete", params: { tabId: 0, modId: 17 } };
                //    var sxc = $2sxc(0).webApi.get(apiCmd)
                //}
            },
            move: function(clip, etc) {
                // todo
            }
        }
    };

    qi.cbActions.click(function () {
        var list = qi.main.actionsForCb.closest(selectors.cb.listSelector);
        var listItems = list.find(selectors.cb.selector);
        var actionConfig = JSON.parse(list.attr(selectors.cb.context));
        var index = 0;

        if (qi.main.actionsForCb.hasClass(selectors.cb.class))
            index = listItems.index(qi.main.actionsForCb[0]) + 1;

        // check cut/paste
        var cbAction = $(this).data("action");
        if (!cbAction) {
            var appOrContent = $(this).data("type");
            return $2sxc(list).manage.createContentBlock(actionConfig.parent, actionConfig.field, index, appOrContent, list);
        } else
            // this is a cut/paste action
            return qi.copyPasteInPage(cbAction, list, index, selectors.cb.id);
    });

    qi.copyPasteInPage = function (cbAction, list, index, type) {
        var clip = qi.clipboard.createSpecs(type, list, index);

        // action!
        if (cbAction === "select") {
            qi.clipboard.mark(clip); 
        } else if (cbAction === "paste") {
            var from = qi.clipboard.data.index, to = clip.index;
            if (isNaN(from) || isNaN(to) || from === to || from + 1 === to) // this moves it to the same spot, so ignore
                return qi.clipboard.clear(); // don't do anything

            $2sxc(list).manage.moveContentBlock(clip.parent, clip.field, from, to);
            qi.clipboard.clear();
        } 
    };

    qi.clipboard = {
        data: {},
        mark: function (newData) {
            if (newData) {
                // if it was already selected with the same thing, then release it
                if (qi.clipboard.data && qi.clipboard.data.item === newData.item)
                    return qi.clipboard.clear();
                qi.clipboard.data = newData;
            }
            $("." + selectors.selected).removeClass(selectors.selected); // clear previous markings
            var cb = $(qi.clipboard.data.item);
            cb.addClass(selectors.selected);
            if (cb.prev().is("iframe"))
                cb.prev().addClass(selectors.selected);
            qi.setSecondaryActionsState(true);
            qi.selected.toggle(cb);
        },
        clear: function() {
            $("." + selectors.selected).removeClass(selectors.selected);
            qi.clipboard.data = null;
            qi.setSecondaryActionsState(false);
            qi.selected.toggle(false);
        },
        createSpecs: function (type, list, index) {
            var listItems = list.find(selectors[type].selector);
            if (index >= listItems.length) index = listItems.length - 1; // sometimes the index is 1 larger than the length, then select last
            var currentItem = listItems[index];
            var editContext = JSON.parse(list.attr(selectors.cb.context) || null) || { parent: "dnn", field: list.id };
            return { parent: editContext.parent, field: editContext.field, list: list, item: currentItem, index: index, type: type };
        }
    };

    qi.setSecondaryActionsState = function(state) {
        var btns = $("a.sc-content-block-menu-btn");
        btns = btns.filter(".icon-sxc-paste"); 
        btns.toggleClass("sc-unavailable", !state);
    };

    qi.modActions.click(function () {
        var type = $(this).data("type");
        var pane = qi.main.actionsForModule.closest(selectors.mod.listSelector);
        var paneName = pane.attr("id").replace("dnn_", "");

        var index = 0;
        if (qi.main.actionsForModule.hasClass("DnnModule"))
            index = pane.find(".DnnModule").index(qi.main.actionsForModule[0]) + 1;

        var cbAction = $(this).data("action");
        if (cbAction)
            return qi.copyPasteInPage(cbAction, pane, index, selectors.mod.id);

        // todo: try to use $2sxc(...).webApi instead of custom re-assembling these common build-up things
        // how: create a object containing the url, data, then just use the sxc.webApi(yourobject)
        var service = $.dnnSF();
        var serviceUrl = service.getServiceRoot("internalservices") + "controlbar/";

        var xhrError = function(xhr) {
            alert("Error while adding module.");
            console.log(xhr);
        };
        $.ajax({
            url: serviceUrl + "GetPortalDesktopModules",
            type: "GET",
            data: "category=All&loadingStartIndex=0&loadingPageSize=100&searchTerm=",
            beforeSend: service.setModuleHeaders,
            success: function (desktopModules) {
                var moduleToFind = type === "Default" ? " Content" : " App";
                var module = null;
                
                desktopModules.forEach(function (e,i) {
                    if (e.ModuleName === moduleToFind)
                        module = e;
                });

                if (!module)
                    return alert(moduleToFind + " module not found.");

                var postData = {
                    Module: module.ModuleID,
                    Page: "",
                    Pane: paneName,
                    Position: -1,
                    Sort: index,
                    Visibility: 0,
                    AddExistingModule: false,
                    CopyModule: false
                };



                $.ajax({
                    url: serviceUrl + "AddModule",
                    type: "POST",
                    data: postData,
                    beforeSend: service.setModuleHeaders,
                    success: function (d) {
                        window.location.reload();
                    },
                    error: xhrError
                });
            },
            error: xhrError
        });

        

    });

    var refreshTimeout = null;
    $("body").on("mousemove", function (e) {
        
        if (refreshTimeout === null)
            refreshTimeout = window.setTimeout(function () {
                requestAnimationFrame(function () {
                    qi.refresh(e);
                    refreshTimeout = null;
                });
            }, 20);

    });
    
    // Prepare offset calculation based on body positioning
    qi.getBodyPosition = function() {
        var bodyPos = qi.body.css("position");
        return bodyPos === "relative" || bodyPos === "absolute"
            ? { x: qi.body.offset().left, y: qi.body.offset().top }
            : { x: 0, y: 0 };
    };

    qi.bodyOffset = qi.getBodyPosition();

    // Refresh content block and modules elements
    qi.refreshDomObjects = function() {
        qi.bodyOffset = qi.getBodyPosition(); // must update this, as sometimes after finishing page load the position changes, like when dnn adds the toolbar

        // Cache the panes (because panes can't change dynamically)
        if (!qi.cachedPanes)
            qi.cachedPanes = $(selectors.mod.listSelector);

        if (qi.enableCb)
            qi.contentBlocks = $(selectors.cb.listSelector).find(selectors.cb.selector).add(selectors.cb.listSelector);
        if (qi.enableMod)
            qi.modules = qi.cachedPanes.find(selectors.mod.selector).add(qi.cachedPanes);
    };

    // position, align and show a menu linked to another item
    qi.positionAndAlign = function(element, coords) {
        return element.css({
            'left': coords.x - qi.bodyOffset.x,
            'top': coords.yh - qi.bodyOffset.y,
            'width': coords.element.width()
        }).show();
    };

    // Refresh positioning / visibility of the quick-insert bar
    qi.refresh = function(e) {

        if (!qi.refreshDomObjects.lastCall || (new Date() - qi.refreshDomObjects.lastCall > 1000)) {
            // console.log('refreshed contentblock and modules');
            qi.refreshDomObjects.lastCall = new Date();
            qi.refreshDomObjects();
        }

        if (qi.enableCb && qi.contentBlocks) {
            qi.nearestCb = qi.findNearest(qi.contentBlocks, { x: e.clientX, y: e.clientY }, selectors.cb.selector);
        }

        if (qi.enableMod && qi.modules) {
            qi.nearestMod = qi.findNearest(qi.modules, { x: e.clientX, y: e.clientY }, selectors.mod.selector);
        }

        qi.modActions.toggleClass("sc-invisible", qi.nearestMod === null);
        qi.cbActions.toggleClass("sc-invisible", qi.nearestCb === null);

        // if previously a parent-pane was highlighted, un-highlight it now
        if (qi.main.parentContainer)
            $(qi.main.parentContainer).removeClass("sc-cb-highlight-for-insert");

        if (qi.nearestCb !== null || qi.nearestMod !== null) {
            var alignTo = qi.nearestCb || qi.nearestMod;

            // find parent pane to highlight
            var parentPane = $(alignTo.element).closest(selectors.mod.listSelector);
            var parentCbList = $(alignTo.element).closest(selectors.cb.listSelector);
            var parentContainer = (parentCbList.length ? parentCbList : parentPane)[0];

            if (parentPane.length > 0) {
                var paneName = parentPane.attr("id") || "";
                if (paneName.length > 4) paneName = paneName.substr(4);
                qi.modActions.filter("[titleTemplate]").each(function() {
                    var t = $(this);
                    t.attr("title", t.attr("titleTemplate").replace("{0}", paneName));
                });
            }

            qi.positionAndAlign(qi.main, alignTo, true);

            // Keep current block as current on menu
            qi.main.actionsForCb = qi.nearestCb ? qi.nearestCb.element : null;
            qi.main.actionsForModule = qi.nearestMod ? qi.nearestMod.element : null;
            qi.main.parentContainer = parentContainer;
            $(parentContainer).addClass("sc-cb-highlight-for-insert");
        } else {
            qi.main.hide();
        }
    };

    // Return the nearest element to the mouse cursor from elements (jQuery elements)
    qi.findNearest = function (elements, position) {
        var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed

        var nearestItem = null;
        var nearestDistance = maxDistance;

        var posX = position.x + qi.win.scrollLeft();
        var posY = position.y + qi.win.scrollTop();

        // Find nearest element
        elements.each(function () {
            var e = qi.getCoordinates($(this));

            // First check x coordinates - must be within container
            if (posX < e.x || posX > e.x + e.w)
                return;

            // Check if y coordinates are within boundaries
            var distance = Math.abs(posY - e.yh);

            if (distance < maxDistance && distance < nearestDistance) {
                nearestItem = e;
                nearestDistance = distance;
            }
        });


        return nearestItem;
    };

    qi.getCoordinates = function (element) {
        return {
            element: element,
            x: element.offset().left,
            w: element.width(),
            y: element.offset().top,
            // For content-block ITEMS, the menu must be visible at the end
            // For content-block-LISTS, the menu must be at top
            yh: element.offset().top + (element.is(selectors.eitherCbOrMod) ? element.height() : 0)
        };
    };
});