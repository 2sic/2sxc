// this enhances the $2sxc client controller with stuff only needed when logged in
(function () {
    if (!window.$2sxc || window.$2sxc.consts) return false;
    $2sxc.c = $2sxc.consts = {
        // classes
        cls: {
            scMenu: "sc-menu",
            scCb: "sc-content-block",
            scElm: "sc-element"
        },
        
        // attribs
        attr: {
            toolbar: "toolbar",
            toolbarData: "data-toolbar",
            settings: "settings",
            settingsData: "data-settings"
        },

        publishAllowed: "DraftOptional"

    };

    // selectors
    var sel = $2sxc.c.sel = {};
    Object.keys($2sxc.c.cls).forEach(function (key, index) {
        sel[key] = "." + $2sxc.c.cls[key];
    });

    /*
    ToDo: functional programming
    $2sxc.c.sel = Object.entries($2sxc.c.cls).reduce((res, current) => {
        res[entry[0]] = entry[1];
        return t;
    }, {});
    */
})();
// this enhances the $2sxc client controller with stuff only needed when logged in
(function () {
    if (!window.$2sxc || window.$2sxc.system) return;

    $2sxc.system = {
        finishUpgrade: finishUpgrade
    };

    // upgrade command - started when an error contains a link to start this
    function finishUpgrade(domElement) {
        var mc = $2sxc(domElement);
        $.ajax({
            type: "get",
            url: mc.resolveServiceUrl("view/module/finishinstallation"),
            beforeSend: $.ServicesFramework(mc.id).setModuleHeaders
        }).success(function () {
            alert("Upgrade ok, restarting the CMS and reloading...");
            location.reload();
        });
        alert("starting upgrade. This could take a few minutes. You'll see an 'ok' when it's done. Please wait...");
    }
})();
(function() {
    $2sxc._commands = {};
})();
/*
 * Actions of 2sxc - mostly used in toolbars
 * 
 * Minimal documentation regarding a button
 * the button can have the following properties / methods
 * - the indexer in the array (usually the same as the name)
 * - name (created in the buttonConfig)
 * - title - actually the translation key to retrieve the title (buttonConfig)
 * - icon - the icon-class
 * - uiActionOnly - true/false if this is just something visual; otherwise a webservice will ensure that a content-group exists (for editing etc.)
 * - showCondition(settings, moduleConfiguration) - would conditionally prevent adding this button by default
 * - code(settings, event) - the code executed on click, if it's not the default action
 * - dynamicClasses(settings) - can conditionally add more css-class names to add to the button, like the "empty" added if something doesn't have metadata
 * - disabled (new!)
 * - params - ...
 */

(function () {
    // helper function to create the configuration object
    function makeDef(name, translateKey, icon, uiOnly, partOfPage, more) {
        if (typeof (partOfPage) !== "boolean")
            throw "partOfPage in commands not provided, order will be wrong!";

        return $2sxc._lib.extend({
            name: name,
            title: "Toolbar." + translateKey,
            icon: "icon-sxc-" + icon,
            uiActionOnly: uiOnly,
            partOfPage: partOfPage
        }, more);
    }

    $2sxc._commands.definitions = {};
    $2sxc._commands.definitions.create = function (cmdSpecs) {
        var enableTools = cmdSpecs.canDesign;
        var isContent = cmdSpecs.isContent;

        var act = {
            // show the basic dashboard which allows view-changing
            // 2017-09-06 2dm "dash-view" deprecated - old name for now "layout" - should not be used any more!
            //"dash-view": makeDef("dash-view", "Dashboard", "", true, { inlineWindow: true }),

            // open the import dialog
            "app-import": makeDef("app-import", "Dashboard", "", true, false, {}),

            // open an edit-item dialog
            'edit': makeDef("edit", "Edit", "pencil", false, true, {
                params: { mode: "edit" },
                showCondition: function (settings, modConfig) {
                    return settings.entityId || settings.useModuleList; // need ID or a "slot", otherwise edit won't work
                }
            }),

            // new is a dialog to add something, and will not add if cancelled
            // new can also be used for mini-toolbars which just add an entity not attached to a module
            // in that case it's essential to add a contentType like 
            // <ul class="sc-menu" data-toolbar='{"action":"new", "contentType": "Category"}'></ul>
            'new': makeDef("new", "New", "plus", false, true, {
                params: { mode: "new" },
                dialog: "edit", // don't use "new" (default) but use "edit"
                showCondition: function (settings, modConfig) {
                    return settings.contentType || modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; // don't provide new on the header-item
                },
                code: function (settings, event, sxc) {
                    // todo - should refactor this to be a toolbarManager.contentBlock command
                    sxc.manage._commands._openNgDialog($2sxc._lib.extend({}, settings, { sortOrder: settings.sortOrder + 1 }), event, sxc);
                }
            }),

            // add brings no dialog, just add an empty item
            'add': makeDef("add", "AddDemo", "plus-circled", false, true, {
                showCondition: function (settings, modConfig) {
                    return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1;
                },
                code: function (settings, event, sxc) {
                    $2sxc._contentBlock.addItem(sxc, settings.sortOrder + 1);
                }
            }),

            // create a metadata toolbar
            "metadata": makeDef("metadata", "Metadata", "tag", false, false, {
                params: { mode: "new" },
                dialog: "edit", // don't use "new" (default) but use "edit"
                dynamicClasses: function (settings) {
                    // if it doesn't have data yet, make it less strong
                    return settings.entityId ? "" : "empty";
                    // return settings.items && settings.items[0].entityId ? "" : "empty";
                },
                showCondition: function (settings) {
                    return !!settings.metadata;
                }, // only add a metadata-button if it has metadata-infos
                configureCommand: function (cmd) {
                    var itm = {
                        Title: "EditFormTitle.Metadata",
                        Metadata: $2sxc._lib.extend({ keyType: "string", targetType: 10 }, cmd.settings.metadata)
                    };
                    $2sxc._lib.extend(cmd.items[0], itm);
                }
            }),

            // remove an item from the placeholder (usually for lists)
            'remove': makeDef("remove", "Remove", "minus-circled", false, true, {
                showCondition: function (settings, modConfig) {
                    return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1;
                },
                code: function (settings, event, sxc) {
                    if (confirm($2sxc.translate("Toolbar.ConfirmRemove"))) {
                        $2sxc._contentBlock.removeFromList(sxc, settings.sortOrder);
                        //sxc.manage.contentBlock
                        //    .removeFromList(settings.sortOrder);
                    }
                }
            }),

            // todo: work in progress related to https://github.com/2sic/2sxc/issues/618
            'delete': makeDef("deleteItem", "Delete", "cancel", true, false, {
                // disabled: true,
                showCondition: function (settings) {
                    // can never be used for a modulelist item, as it is always in use somewhere
                    if (settings.useModuleList)
                        return false;
                    
                    // check if all data exists required for deleting
                    return settings.entityId && settings.entityGuid && settings.entityTitle;
                },
                code: function (settings, event, sxc) {
                    $2sxc.contentItems.delete(sxc, settings.entityId, settings.entityGuid, settings.entityTitle);
                }
            }),

            'moveup': makeDef("moveup", "MoveUp", "move-up", false, true, {
                showCondition: function (settings, modConfig) {
                    return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1 && settings.sortOrder !== 0;
                },
                code: function (settings, event, sxc) {
                    $2sxc._contentBlock.changeOrder(sxc, settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
                }
            }),

            'movedown': makeDef("movedown", "MoveDown", "move-down", false, true, {
                showCondition: function (settings, modConfig) {
                    return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1;
                },
                code: function (settings, event, sxc) {
                    $2sxc._contentBlock.changeOrder(sxc, settings.sortOrder, settings.sortOrder + 1);
                }
            }),

            'instance-list': makeDef("instance-list", "Sort", "list-numbered", false, true, {
                showCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; }
            }),

            // todo: shouldn't be available if changes are not allowed
            'publish': makeDef("publish", "Unpublished", "eye-off", false, false, {
                showCondition: function (settings, modConfig) {
                    return settings.isPublished === false;
                },
                disabled: function(settings, modConfig) {
                    return !cmdSpecs.allowPublish;
                },
                code: function (settings, event, sxc) {
                    if (settings.isPublished) return alert($2sxc.translate("Toolbar.AlreadyPublished"));

                    // if we have an entity-id, publish based on that
                    if (settings.entityId) return $2sxc._contentBlock.publishId(sxc, settings.entityId);

                    var part = settings.sortOrder === -1 ? "listcontent" : "content";
                    var index = settings.sortOrder === -1 ? 0 : settings.sortOrder;
                    return $2sxc._contentBlock.publish(sxc, part, index);
                }
            }),

            'replace': makeDef("replace", "Replace", "replace", false, true, {
                showCondition: function (settings) { return settings.useModuleList; }
            }),

            //#region app-actions: app-settings, app-resources
            'app-settings': makeDef("app-settings", "AppSettings", "sliders", true, false, {
                dialog: "edit",
                disabled: cmdSpecs.appSettingsId === null,
                title: "Toolbar.AppSettings" + (cmdSpecs.appSettingsId === null ? "Disabled" : ""),
                showCondition: function (settings, modConfig) {
                    return enableTools && !isContent; // only if settings exist, or are 0 (to be created)
                },
                configureCommand: function (cmd) {
                    cmd.items = [{ EntityId: cmdSpecs.appSettingsId }];
                },
                dynamicClasses: function (settings) {
                    return cmdSpecs.appSettingsId !== null ? "" : "empty";  // if it doesn't have a query, make it less strong
                }
            }),

            'app-resources': makeDef("app-resources", "AppResources", "language", true, false, {
                dialog: "edit",
                disabled: cmdSpecs.appResourcesId === null,
                title: "Toolbar.AppResources" + (cmdSpecs.appResourcesId === null ? "Disabled" : ""),
                showCondition: function (settings, modConfig) {
                    return enableTools && !isContent; // only if resources exist or are 0 (to be created)...
                },
                configureCommand: function (cmd) {
                    cmd.items = [{ EntityId: cmdSpecs.appResourcesId }];
                },
                dynamicClasses: function (settings) {
                    return cmdSpecs.appResourcesId !== null ? "" : "empty";  // if it doesn't have a query, make it less strong
                }
            }),
            //#endregion

            //#region app & zone
            'app': makeDef("app", "App", "settings", true, false, {
                showCondition: enableTools
            }),

            'zone': makeDef("zone", "Zone", "manage", true, false, {
                showCondition: enableTools
            })
            //#endregion
        };

        // quick helper so we can better debug the creation of definitions
        function addDef(def) {
            act[def.name] = def;
        }

        //#region template commands: contenttype, contentitems, template-query, template-develop, template-settings
        addDef(makeDef("contenttype", "ContentType", "fields", true, false, {
            showCondition: enableTools
        }));

        addDef(makeDef("contentitems", "ContentItems", "table", true, false, {
            params: { contentTypeName: cmdSpecs.contentTypeId },
            showCondition: function (settings, modConfig) {
                return enableTools && (settings.contentType || cmdSpecs.contentTypeId);
            },
            configureCommand: function (cmd) {
                if (cmd.settings.contentType) // optionally override with custom type
                    cmd.params.contentTypeName = cmd.settings.contentType;
                // maybe: if item doesn't have a type, use that of template
                // else if (cmdSpecs.contentTypeId)
                //    cmd.params.contentTypeName = cmdSpecs.contentTypeId;
                if (cmd.settings.filters) {
                    var enc = JSON.stringify(cmd.settings.filters);
                    
                    // special case - if it contains a "+" character, this won't survive 
                    // encoding through the hash as it's always replaced with a space, even if it would be preconverted to %2b
                    // so we're base64 encoding it - see https://github.com/2sic/2sxc/issues/1061
                    if (enc.indexOf("+") > -1)
                        enc = btoa(enc);
                    cmd.params.filters = enc;
                }
            }
        }));

        addDef(makeDef("template-develop", "Develop", "code", true, false, {
            newWindow: true,
            dialog: "develop",
            showCondition: enableTools,
            configureCommand: function (cmd) {
                cmd.items = [{ EntityId: cmdSpecs.templateId }];
            }
        }));

        addDef(makeDef("template-query", "QueryEdit", "filter", true, false, {
            dialog: "pipeline-designer",
            params: { pipelineId: cmdSpecs.queryId },
            newWindow: true,
            disabled: cmdSpecs.appSettingsId === null,
            title: "Toolbar.QueryEdit" + (cmdSpecs.queryId === null ? "Disabled" : ""),
            showCondition: function (settings, modConfig) {
                return enableTools && !isContent;
            },
            dynamicClasses: function (settings) {
                return cmdSpecs.queryId ? "" : "empty"; // if it doesn't have a query, make it less strong
            }
        }));

        addDef(makeDef("template-settings", "TemplateSettings", "sliders", true, false, {
            dialog: "edit",
            showCondition: enableTools,
            configureCommand: function (cmd) {
                cmd.items = [{ EntityId: cmdSpecs.templateId }];
            }

        }));
        //#endregion template commands

        //#region custom code buttons
        addDef(makeDef("custom", "Custom", "bomb", true, false, {
            code: function (settings, event, sxc) {
                var fn;
                console.log("custom action with code - BETA feature, may change");
                if (!settings.customCode) {
                    console.warn("custom code action, but no onclick found to run", settings);
                    return;
                }
                try {
                    fn = new Function("settings", "event", "sxc", settings.customCode); // jshint ignore:line
                    fn(settings, event, sxc);
                } catch (err) {
                    console.error("error in custom button-code: ", settings);
                }
            }
        }));
        //#endregion

        addDef(makeDef("layout", "ChangeLayout", "glasses", true, true, {
             inlineWindow: true 
        }));

        addDef(makeDef("more", "MoreActions", "options btn-mode", true, false, {
            code: function (settings, event) {
                var btn = $(event.target),
                    fullMenu = btn.closest("ul.sc-menu"),
                    oldState = Number(fullMenu.attr("data-state") || 0),
                    max = Number(fullMenu.attr("group-count")),
                    newState = (oldState + 1) % max;

                fullMenu.removeClass("group-" + oldState)
                    .addClass("group-" + newState)
                    .attr("data-state", newState);
            }
        }));
        
        // show the version dialog
        addDef(makeDef("item-history", "ItemHistory", "clock", true, false, {
            inlineWindow: true,
            fullScreen: true
        }));

        return act;
    };
})();

(function () {
    $2sxc._commands.instanceEngine = function (sxc, editContext) {
        var engine = {
            commands: $2sxc._commands.initializeInstanceCommands(editContext),

            // assemble an object which will store the configuration and execute it
            create: function (specialSettings) {
                var settings = $2sxc._lib.extend({}, sxc.manage._instanceConfig, specialSettings); // merge button with general toolbar-settings
                var ngDialogUrl = sxc.manage._editContext.Environment.SxcRootUrl +
                    "desktopmodules/tosic_sexycontent/dist/dnn/ui.html?sxcver=" +
                    sxc.manage._editContext.Environment.SxcVersion;
                var isDebug = $2sxc.urlParams.get("debug") ? "&debug=true" : "";

                var cmd = {
                    settings: settings,
                    items: settings.items || [], // use predefined or create empty array
                    params: $2sxc._lib.extend({
                        dialog: settings.dialog || settings.action // the variable used to name the dialog changed in the history of 2sxc from action to dialog
                    }, settings.params),

                    addSimpleItem: function () {
                        var itm = {},
                            ct = cmd.settings.contentType || cmd.settings.attributeSetName; // two ways to name the content-type-name this, v 7.2+ and older
                        if (cmd.settings.entityId) itm.EntityId = cmd.settings.entityId;
                        if (ct) itm.ContentTypeName = ct;

                        // only add if there was stuff to add
                        if (itm.EntityId || itm.ContentTypeName) cmd.items.push(itm);
                    },

                    // this adds an item of the content-group, based on the group GUID and the sequence number
                    addContentGroupItem: function (guid, index, part, isAdd, isEntity, cbid, sectionLanguageKey) {
                        cmd.items.push({
                            Group: {
                                Guid: guid,
                                Index: index,
                                Part: part,
                                Add: isAdd
                            },
                            Title: $2sxc.translate(sectionLanguageKey)
                        });
                    },

                    // this will tell the command to edit a item from the sorted list in the group, optionally together with the presentation item
                    addContentGroupItemSetsToEditList: function (withPresentation) {
                        var isContentAndNotHeader = (cmd.settings.sortOrder !== -1),
                            index = isContentAndNotHeader ? cmd.settings.sortOrder : 0,
                            prefix = isContentAndNotHeader ? "" : "List",
                            cTerm = prefix + "Content",
                            pTerm = prefix + "Presentation",
                            isAdd = cmd.settings.action === "new",
                            groupId = cmd.settings.contentGroupId;
                        cmd.addContentGroupItem(groupId, index, cTerm.toLowerCase(), isAdd, cmd.settings.cbIsEntity, cmd.settings.cbId, "EditFormTitle." + cTerm);

                        if (withPresentation) cmd.addContentGroupItem(groupId, index, pTerm.toLowerCase(), isAdd, cmd.settings.cbIsEntity, cmd.settings.cbId, "EditFormTitle." + pTerm);
                    },

                    // build the link, combining specific params with global ones and put all in the url
                    generateLink: function () {
                        // if there is no items-array, create an empty one (it's required later on)
                        if (!cmd.settings.items) cmd.settings.items = [];
                        //#region steps for all actions: prefill, serialize, open-dialog
                        // when doing new, there may be a prefill in the link to initialize the new item
                        if (cmd.settings.prefill) {
                            for (var i = 0; i < cmd.items.length; i++) {
                                cmd.items[i].Prefill = cmd.settings.prefill;
                            }
                        }
                        cmd.params.items = JSON.stringify(cmd.items); // Serialize/json-ify the complex items-list

                        // clone the params and adjust parts based on partOfPage settings...
                        var sharedParams = $2sxc._lib.extend({}, sxc.manage._dialogParameters);
                        if (!cmd.settings.partOfPage) {
                            delete sharedParams.versioningRequirements;
                            delete sharedParams.publishing;
                            sharedParams.partOfPage = false;
                        }

                        return ngDialogUrl +
                            "#" + $.param(sharedParams) +
                            "&" + $.param(cmd.params) +
                            isDebug;
                        //#endregion
                    }
                };
                return cmd;
            },

            // create a dialog link
            _linkToNgDialog: function (specialSettings) {
                var cmd = sxc.manage._commands.create(specialSettings);

                if (cmd.settings.useModuleList) cmd.addContentGroupItemSetsToEditList(true);
                else cmd.addSimpleItem();

                // if the command has own configuration stuff, do that now
                if (cmd.settings.configureCommand) cmd.settings.configureCommand(cmd);

                return cmd.generateLink();
            },

            // open a new dialog of the angular-ui
            _openNgDialog: function (settings, event, sxc /*, closeCallback*/) {
                // the callback will handle events after closing the dialog
                // and reload the in-page view w/ajax or page reload
                var callback = function () {
                    $2sxc._contentBlock.reloadAndReInitialize(sxc);
                    // 2017-09-29 2dm: no call of _openNgDialog seems to give a callback ATM closeCallback();
                };
                var link = engine._linkToNgDialog(settings); // the link contains everything to open a full dialog (lots of params added)
                if (settings.inlineWindow)
                    return $2sxc._quickDialog.showOrToggle(sxc, link, callback, settings.fullScreen /* settings.dialog === "item-history"*/ , settings.dialog);
                if (settings.newWindow || (event && event.shiftKey))
                    return window.open(link);
                return $2sxc.totalPopup.open(link, callback);
            },

            // ToDo: remove dead code
            executeAction: function (nameOrSettings, settings, event) {

                // cycle parameters, in case it was called with 2 params only
                if (!event && settings && typeof settings.altKey !== "undefined") { // no event param, but settings contains the event-object
                    event = settings; // move it to the correct variable
                    settings = {}; // clear the settings variable, as none was provided
                }

                // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
                var origEvent = event || window.event;

                // check if name is name (string) or object (settings)
                settings = (typeof nameOrSettings === "string") ?
                    $2sxc._lib.extend(settings || {}, {
                        "action": nameOrSettings
                    }) // place the name as an action-name into a command-object
                    :
                    nameOrSettings;

                var conf = engine.commands[settings.action];
                settings = $2sxc._lib.extend({}, conf, settings); // merge conf & settings, but settings has higher priority

                if (!settings.dialog) settings.dialog = settings.action; // old code uses "action" as the parameter, now use verb ? dialog
                if (!settings.code) settings.code = engine._openNgDialog; // decide what action to perform

                if (conf.uiActionOnly) return settings.code(settings, origEvent, sxc);

                // if more than just a UI-action, then it needs to be sure the content-group is created first
                return $2sxc._contentBlock.prepareToAddContent(sxc, settings.useModuleList)
                    .then(function () {
                        return settings.code(settings, origEvent, sxc);
                    });
            }
        };

        return engine;
    };
})();
(function() {
    $2sxc._commands.initializeInstanceCommands = function(editContext) {
        var cg = editContext.ContentGroup;
        return $2sxc._commands.definitions.create({
            canDesign: editContext.User.CanDesign,
            templateId: cg.TemplateId,
            contentTypeId: cg.ContentTypeName,
            isContent: cg.IsContent,
            queryId: cg.QueryId,
            appResourcesId: cg.AppResourcesId,
            appSettingsId: cg.AppSettingsId,
            allowPublish: editContext.ContentBlock.VersioningRequirements === $2sxc.c.publishAllowed
        });
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
 * 
 * Maybe ToDo 2cb:
 * 2sxc should have one entry point (interface to browser context) only.
 * Otherwise, we cannot know, when which part will be executed and debugging becomes very difficult.
 * 
 */
(function () {

    /**
     * The main content-block manager
     */
    $2sxc._contentBlock = {

        // constants
        cViewWithoutContent: '_LayoutElement', // needed to differentiate the "select item" from the "empty-is-selected" which are both empty
        cUseExistingTemplate: -1
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
(function () {

    var cbm = $2sxc._contentBlock;

    // internal helper, to do something and reload the content block
    cbm.getAndReload = function (sxc, url, params) {
        return sxc.webApi.get({
            url: url,
            params: params
        }).then(function () { cbm.reloadAndReInitialize(sxc); });
    };

    /**
     * remove an item from a list, then reload
     * @param {} sxc 
     * @param {} sortOrder 
     * @returns {} 
     */
    cbm.removeFromList = function(sxc, sortOrder) {
        return cbm.getAndReload(sxc, "view/module/removefromlist", { sortOrder: sortOrder });
    };

    /**
     * change the order of an item in a list, then reload
     * @param {} sxc 
     * @param {} initOrder 
     * @param {} newOrder 
     * @returns {} 
     */
    cbm.changeOrder = function(sxc, initOrder, newOrder) {
        return cbm.getAndReload(sxc, "view/module/changeorder",
            { sortOrder: initOrder, destinationSortOrder: newOrder });
    };

    /**
     * add an item to the list at this position
     * @param {} sxc 
     * @param {} sortOrder 
     * @returns {} 
     */
    cbm.addItem = function(sxc, sortOrder) {
        return cbm.getAndReload(sxc, "view/module/additem", { sortOrder: sortOrder });
    };

    /**
     * set a content-item in this block to published, then reload
     * @param {} sxc 
     * @param {} part 
     * @param {} sortOrder 
     * @returns {} 
     */
    cbm.publish = function(sxc, part, sortOrder) {
        return cbm.getAndReload(sxc, "view/module/publish", { part: part, sortOrder: sortOrder });
    };

    /**
     * publish an item using it's ID
     * @param {} sxc 
     * @param {} entityId 
     * @returns {} 
     */
    cbm.publishId = function(sxc, entityId) {
        return cbm.getAndReload(sxc, "view/module/publish", { id: entityId });
    };


})();
// contains commands to create/move/delete a contentBlock in a page

$2sxc._contentBlock.manipulator = function (sxc) {
    return {
        create: create,
        move: move,
        delete: remove
    };

    function create(parentId, fieldName, index, appName, container, newGuid) {
        // the wrapper, into which this will be placed and the list of pre-existing blocks
        var listTag = container;
        if (listTag.length === 0) return alert('can\'t add content-block as we couldn\'t find the list');
        var cblockList = listTag.find('div.sc-content-block');
        if (index > cblockList.length) index = cblockList.length; // make sure index is never greater than the amount of items

        var params = {
            parentId: parentId,
            field: fieldName,
            sortOrder: index,
            app: appName,
            guid: newGuid,
        };
        return sxc.webApi.get({ url: 'view/module/generatecontentblock', params: params })
            .then(function (result) {
                var newTag = $(result); // prepare tag for inserting

                // should I add it to a specific position...
                if (cblockList.length > 0 && index > 0)
                    $(cblockList[cblockList.length > index - 1 ? index - 1 : cblockList.length - 1])
                        .after(newTag);
                else //...or just at the beginning?
                    listTag.prepend(newTag);

                var sxcNew = $2sxc(newTag);
                $2sxc._toolbarManager.buildToolbars(newTag);
            });
    }

    function move(parentId, field, indexFrom, indexTo) {
        var params = {
            parentId: parentId,
            field: field,
            indexFrom: indexFrom,
            indexTo: indexTo,
        };

        // todo: need sxc!
        return sxc.webApi.get({ url: 'view/module/moveiteminlist', params: params })
            .then(function () {
                console.log("done moving!");
                window.location.reload();
            });
    }

    // delete a content-block inside a list of content-blocks
    function remove(parentId, field, index) {
        if (!confirm($2sxc.translate('QuickInsertMenu.ConfirmDelete'))) return null;
        var params = {
            parentId: parentId,
            field: field,
            index: index,
        };
        return sxc.webApi.get({ url: 'view/module/RemoveItemInList', params: params })
            .then(function () {
                console.log('done deleting!');
                window.location.reload();
            });
    }
};
/* 
 * this is the content block manager in the browser
 * 
 * A Content Block is a standalone unit of content, with it's own definition of
 * 1. content items
 * 2. template
 * + some other stuff
 *
 * it should be able to render itself
 */
(function () {
    var cbm = $2sxc._contentBlock;

    /**
     * ajax update/replace the content of the content-block
     * optionally also initialze the toolbar (if not just preview)
     * @param {Object<>} sxc 
     * @param {string} newContent 
     * @param {boolean} justPreview 
     * @returns {} 
     */
    cbm.replaceCb = function (sxc, newContent, justPreview) {
        try {
            var newStuff = $(newContent);

            // Must disable toolbar before we attach to DOM
            if (justPreview) $2sxc._toolbarManager.disable(newStuff);

            $($2sxc._manage.getTag(sxc)).replaceWith(newStuff);

            // reset the cache, so the sxc-object is refreshed
            sxc.recreate(true);
        } catch (e) {
            console.log("Error while rendering template:", e);
        }
    };

    /**
     * Show a message where the content of a module should be - usually as placeholder till something else happens
     * @param {object} sxc 
     * @param {string} newContent 
     * @returns {} - nothing
     */
    cbm.showMessage = function (sxc, newContent) {
        $($2sxc._manage.getTag(sxc)).html(newContent);
    };

    cbm.ajaxLoad = function (sxc, alternateTemplateId, justPreview) {
        // ajax-call, then replace
        return cbm.getPreviewWithTemplate(sxc, alternateTemplateId)
            .then(function (result) {
                return cbm.replaceCb(sxc, result, justPreview);
            })
            .then($quickE.reset); // reset quick-edit, because the config could have changed
    };

    // this one assumes a replace / change has already happened, but now must be finalized...
    cbm.reloadAndReInitialize = function (sxc, forceAjax, preview) {
        var manage = sxc.manage;

        // if ajax is not supported, we must reload the whole page
        if (!forceAjax && !manage._reloadWithAjax) return window.location.reload();

        return cbm.ajaxLoad(sxc, cbm.cUseExistingTemplate, !!preview)
            .then(function () {
                
                if (dnn_tabVersioningEnabled) {
                    dnn.ContentEditorManager.triggerChangeOnPageContentEvent();
                }

                // ToDo: tell Evoq that page has changed if it has changed (Ajax call)
                // maybe check if already publish
                // compare to HTML module
                // if (publishing is required (FROM CONTENT BLOCK) and publish button not visible) show publish button

                // 2017-09-02 2dm - believe this was meant to re-init the dialog manager, but it doesn't actually work
                // must check for side-effects, which would need the manager to re-build the configuration
                $2sxc._quickDialog.hide();
            });
    };

})();

/* 
 * this is part of the content block manager
 */
(function () {
    var cbm = $2sxc._contentBlock;

    Object.assign(cbm, {
        prepareToAddContent: prepareToAddContent,
        updateTemplateFromDia: updateTemplateFromDia
    });

    return;

    /**
     * prepare the instance so content can be added
     * this ensure the content-group has been created, which is required to add content
     * @param {} sxc 
     * @returns {} 
     */
    function prepareToAddContent(sxc, useModuleList) {
        var isCreated = sxc.manage._editContext.ContentGroup.IsCreated;
        if (isCreated || !useModuleList) return $.when(null);
        // return persistTemplate(sxc, null);
        // var manage = sxc.manage;
        // var contentGroup = manage._editContext.ContentGroup;
        // var showingAjaxPreview = $2sxc._toolbarManager.isDisabled(sxc);
        // var groupExistsAndTemplateUnchanged = !!contentGroup.HasContent; // && !showingAjaxPreview;

        var templateId = /* templateId || */ sxc.manage._editContext.ContentGroup.TemplateId;

        // template has not changed
        // if (groupExistsAndTemplateUnchanged) return $.when(null);

        // persist the template
        return updateTemplate(sxc, templateId, true);
    }

    /**
     * Update the template and adjust UI accordingly.
     * @param {*} sxc 
     * @param {*} templateId 
     * @param {*} forceCreate 
     */
    function updateTemplateFromDia(sxc, templateId, forceCreate) {
        var contentGroup = sxc.manage._editContext.ContentGroup;
        var showingAjaxPreview = $2sxc._toolbarManager.isDisabled(sxc);

        // todo: should move things like remembering undo etc. back into the contentBlock state manager
        // or just reset it, so it picks up the right values again ?
        return updateTemplate(sxc, templateId, forceCreate)
            .then(function () {
                $2sxc._quickDialog.hide();

                // if it didn't have content, then it only has now...
                if (!contentGroup.HasContent) contentGroup.HasContent = forceCreate;

                // only reload on ajax, not on app as that was already re-loaded on the preview
                // necessary to show the original template again
                if (showingAjaxPreview) cbm.reloadAndReInitialize(sxc);
            });
    }

    /**
     * Update the template.
     */
    function updateTemplate(sxc, templateId, forceCreate) {
        return cbm.saveTemplate(sxc, templateId, forceCreate)
            .then(function (data, textStatus, xhr) {

                // error handling
                if (xhr.status !== 200) return alert('error - result not ok, was not able to create ContentGroup');

                if (!data) return;

                // fixes a special case where the guid is given with quotes (dependes on version of angularjs) issue #532
                newGuid = data.replace(/[\",\']/g, '');

                if (console) console.log('created content group {' + newGuid + '}');
                sxc.manage._updateContentGroupGuid(newGuid);
            });
    }
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
(function () {

    var cbm = $2sxc._contentBlock;

    //#region functions working only with what they are given
    // 2017-08-27 2dm: I'm working on cleaning up this code, and an important part 
    // is to have code which doesn't use old state (like object-properties initialized earlier)
    // extracting these methods is part of the work

    /**
     * TODO - unclear if still in use
     * @param {object} sxc 
     * @param {boolean} state 
     * @returns {promise} 
     */
    // 2017-09-02 2dm removed, deprecated, it's not stored on the server any more
    //cbm.setTemplateChooserState = function(sxc, state) {
    //    return sxc.webApi.get({
    //        url: "view/module/SetTemplateChooserState",
    //        params: { state: state }
    //    });
    //};

    /**
     * Save the template configuration for this instance
     * @param {object} sxc 
     * @param {int} templateId 
     * @param {boolean} [forceCreateContentGroup]
     * @returns {promise} 
     */
    cbm.saveTemplate = function(sxc, templateId, forceCreateContentGroup) {
        return sxc.webApi.get({
            url: "view/module/savetemplateid",
            params: {
                templateId: templateId,
                forceCreateContentGroup: forceCreateContentGroup,
                newTemplateChooserState: false
            }
        });
    };

    /**
     * Retrieve the preview from the web-api
     * @param {object} sxc 
     * @param {int} templateId 
     * @returns {promise} promise with the html in the result
     */
    cbm.getPreviewWithTemplate = function(sxc, templateId) {
        var ec = sxc.manage._editContext;
        templateId = templateId || -1; // fallback, meaning use saved ID
        return sxc.webApi.get({
            url: "view/module/rendertemplate",
            params: {
                templateId: templateId,
                lang: ec.Language.Current,
                cbisentity: ec.ContentBlock.IsEntity,
                cbid: ec.ContentBlock.Id,
                originalparameters: JSON.stringify(ec.Environment.parameters)
            },
            dataType: "html"
        });
    };
    //#endregion


})();
// Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
var $2sxcActionMenuMapper = function (moduleId) {
    var run = $2sxc(moduleId).manage.run;
    return {
        changeLayoutOrContent: function () { run("layout"); },
        addItem: function () { run("add", { useModuleList: true, sortOrder: 0 }); },
        edit: function () { run("edit", { useModuleList: true, sortOrder: 0 }); },
        adminApp: function () { run("app"); },
        adminZone: function () { run("zone"); },
        develop: function () { run("template-develop"); },
    };
};
//# sourceMappingURL=dnn-inpage-edit.js.map
// The following script fixes a bug in DNN 08.00.04
// the bug tries to detect a module-ID based on classes in a tag, 
// but uses a bad regex and captures the number 2 on all 2sxc-modules 
// instead of the real id
// this patch changes the order of the className of 2sxc modules when
// they are accessed through '$.fn.attr'
// 'DnnModule-2sxc DnnModule-xxx' -> DNN thinks the mod id is 2 (false)
// 'DnnModule-xxx DnnModule-2sxc' -> DNN thinks the mod id is xxx (correct)
// documented here https://github.com/2sic/2sxc/issues/986
(function () {
    var fn = $.fn.attr;
    $.fn.attr = function () {
        var val = fn.apply(this, arguments);
        if (arguments[0] !== "class"
            || typeof val !== "string"
            || val.search("DnnModule-2sxc ") === -1)
            return val;
        return val.replace("DnnModule-2sxc ", "") + " DnnModule-2sxc";
    };
})();
//# sourceMappingURL=dnn-08.00.04.js.map
// this enhances the $2sxc client controller with stuff only needed when logged in
(function() {
    if (window.$2sxc) {
        
        //#region contentItem Commands
        $2sxc.contentItems = {
            // delete command - try to really delete a content-item
            "delete": function (sxc, itemId, itemGuid, itemTitle) {
                // first show main warning / get ok
                var ok = confirm($2sxc.translate("Delete.Confirm")
                    .replace("{id}", itemId)
                    .replace("{title}", itemTitle));
                if (!ok) return;

                sxc.webApi.delete("app-content/any/" + itemGuid, null, null, true)
                    .success(function () {
                        location.reload();
                    }).error(function (error) {
                        var msgJs = $2sxc.translate("Delete.ErrCheckConsole");
                        console.log(error);

                        // check if it's a permission config problem
                        if (error.status === 401) alert($2sxc.translate("Delete.ErrPermission") + msgJs);
                        if (error.status === 400) alert($2sxc.translate("Delete.ErrInUse") + msgJs);
                    });
            }
        };
        //#endregion

    }
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

// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it
// all in-page toolbars etc.
// if loaded, it's found under the $2sxc(module).manage
// it has commands to
// - getButton
// - getToolbar
// - run(...)
// - isEditMode

(function () {
    $2sxc._manage = {};
})();

(function() {
    var mngApi = $2sxc._manage;

    /**
     * Get a html tag of the current sxc instance
     * @param {any} sxci
     * @return {jquery} - resulting html
     */
    $2sxc._manage.getTag = function(sxci) {
        return $("div[data-cb-id='" + sxci.cbid + "']")[0];
    };



    /**
     * get the edit-context object (a json object) of the current tag/sxc-instance
     * @param {any} htmlTag
     * @return {any} edit-context object
     */
    $2sxc._manage.getEditContextOfTag = function getEditContextOfTag(htmlTag) {
        var attr = htmlTag.getAttribute("data-edit-context");
        return JSON.parse(attr || "");
    };



    /**
     * get edit-context info of an sxc-object
     * @param {any} sxc
     * @return {any} edit context info
     */
    $2sxc._manage.getEditContext = function getEditContext(sxc) {
        return $2sxc._manage.getEditContextOfTag(mngApi.getTag(sxc));
    };


    /**
     * builds a config object used in the toolbar system
     * @param {any} editContext 
     * @returns {any} object containing various properties for this current sxc-instance
     */
    $2sxc._manage.buildInstanceConfig = function(editContext) {
        var ce = editContext.Environment, cg = editContext.ContentGroup, cb = editContext.ContentBlock;
        return {
            portalId: ce.WebsiteId,
            tabId: ce.PageId,
            moduleId: ce.InstanceId,
            version: ce.SxcVersion,

            contentGroupId: cg.Guid,
            cbIsEntity: cb.IsEntity,
            cbId: cb.Id,
            appPath: cg.AppUrl,
            isList: cg.IsList
        };
    };


    $2sxc._manage.getUserOfEditContext = function getUserOfEditContext(editContext) {
        return { canDesign: editContext.User.CanDesign, canDevelop: editContext.User.CanDesign };
    };



    /**
     * create a config-object for the quick-dialog, with all settings which the quick-dialog will need
     * @param {any} editContext
     * @returns {any} 
     */

    $2sxc._manage.buildQuickDialogConfig = function buildQuickDialogConfig(editContext) {
        return {
            appId: editContext.ContentGroup.AppId,
            isContent: editContext.ContentGroup.IsContent,
            hasContent: editContext.ContentGroup.HasContent,
            isList: editContext.ContentGroup.IsList,
            templateId: editContext.ContentGroup.TemplateId,
            contentTypeId: editContext.ContentGroup.ContentTypeName,
            templateChooserVisible: editContext.ContentBlock.ShowTemplatePicker, // todo: maybe move to content-goup
            user: $2sxc._manage.getUserOfEditContext(editContext),
            supportsAjax: editContext.ContentGroup.SupportsAjax
        };
    };

    /**
        * get all parameters needed by NG dialogs from an sxc
        * @param {any} sxc
        * @param {any} [editContext]
        * @return {any} special object containing the ng-dialog parameters
        */
    $2sxc._manage.buildNgDialogParams = function buildNgDialogParams(sxc, editContext) {
        if (!editContext) editContext = mngApi.getEditContext(sxc);
        return {
            zoneId: editContext.ContentGroup.ZoneId,
            appId: editContext.ContentGroup.AppId,
            tid: editContext.Environment.PageId,
            mid: editContext.Environment.InstanceId,
            cbid: sxc.cbid,
            lang: editContext.Language.Current,
            langpri: editContext.Language.Primary,
            langs: JSON.stringify(editContext.Language.All),
            portalroot: editContext.Environment.WebsiteUrl,
            websiteroot: editContext.Environment.SxcRootUrl,
            partOfPage: editContext.ContentBlock.PartOfPage,
            //versioningRequirements: editContext.ContentBlock.VersioningRequirements,
            publishing: editContext.ContentBlock.VersioningRequirements,

            // todo: probably move the user into the dashboard info
            user: $2sxc._manage.getUserOfEditContext(editContext),
            approot: editContext.ContentGroup.AppUrl || null // this is the only value which doesn't have a slash by default.  note that the app-root doesn't exist when opening "manage-app"
        };
    };
})();
// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it
// all in-page toolbars etc.
// if loaded, it's found under the $2sxc(module).manage
// it has commands to
// - getButton
// - getToolbar
// - run(...)
// - isEditMode

(function () {
    $2sxc._manage.initInstance = function(sxc) {
        try {
            initInstance(sxc);
        } catch (e) {
            console.error("error in 2sxc - will log but not throw", e);
        }
    };

    var mngApi = $2sxc._manage;
    function initInstance(sxc) {
        var editContext = mngApi.getEditContext(sxc);
        var userInfo = mngApi.getUserOfEditContext(editContext);

        var cmdEngine = $2sxc._commands.instanceEngine(sxc, editContext);

        var editManager = sxc.manage = {
            //#region Official, public properties and commands, which are stable for use from the outside
            /**
             * run a command - often used in toolbars and custom buttons
             */
            run: cmdEngine.executeAction,

            /**
             * Generate a button (an <a>-tag) for one specific toolbar-action.
             * @param {Object<any>} actDef - settings, an object containing the specs for the expected buton
             * @param {int} groupIndex - number what button-group it's in'
             * @returns {string} html of a button
             */
            getButton: function (actDef, groupIndex) {
                return $2sxc._toolbarManager.generateButtonHtml(sxc, actDef, groupIndex);
            },

            /**
             * Builds the toolbar and returns it as HTML
             * @param {Object<any>} tbConfig - general toolbar config
             * @param {Object<any>} moreSettings - additional / override settings
             * @returns {string} html of the current toolbar
             */
            getToolbar: function (tbConfig, moreSettings) {
                return $2sxc._toolbarManager.generateToolbarHtml(sxc, tbConfig, moreSettings);
            },
            //#endregion official, public properties - everything below this can change at any time

            // internal method to find out if it's in edit-mode
            _isEditMode: function () { return editContext.Environment.IsEditable; },
            _reloadWithAjax: editContext.ContentGroup.SupportsAjax,
            _dialogParameters: mngApi.buildNgDialogParams(sxc, editContext),      // used for various dialogs
            _instanceConfig: mngApi.buildInstanceConfig(editContext), // used to configure buttons / toolbars
            _editContext: editContext,              // metadata necessary to know what/how to edit
            _quickDialogConfig: mngApi.buildQuickDialogConfig(editContext),           // used for in-page dialogs
            _commands: cmdEngine,                        // used to handle the commands for this content-block
            _user: userInfo,

            // init this object 
            init: function init() {
                // enhance UI in case there are known errors / issues
                if (editContext.error.type)
                    editManager._handleErrors(editContext.error.type, $2sxc._manage.getTag(sxc));

                // todo: move this to dialog-handling
                // display the dialog
                var openDialogId = sessionStorage.getItem("dia-cbid");
                if (editContext.error.type || !openDialogId || openDialogId !== sxc.cbid) return false;
                sessionStorage.removeItem("dia-cbid");
                editManager.run("layout");
            },

            // private: show error when the app-data hasn't been installed yet for this imported-module
            _handleErrors: function (errType, cbTag) {
                var errWrapper = $("<div class=\"dnnFormMessage dnnFormWarning sc-element\"></div>");
                var msg = "";
                var toolbar = $("<ul class='sc-menu'></ul>");
                if (errType === "DataIsMissing") {
                    msg = "Error: System.Exception: Data is missing - usually when a site is copied but the content / apps have not been imported yet - check 2sxc.org/help?tag=export-import";
                    toolbar.attr("data-toolbar", '[{\"action\": \"zone\"}, {\"action\": \"more\"}]');
                }
                errWrapper.append(msg);
                errWrapper.append(toolbar);
                $(cbTag).append(errWrapper);
            },

            // change config by replacing the guid, and refreshing dependend sub-objects
            _updateContentGroupGuid: function (newGuid) {
                editContext.ContentGroup.Guid = newGuid;
                editManager._instanceConfig = mngApi.buildInstanceConfig(editContext);
            },

            _getCbManipulator: function () {
                return $2sxc._contentBlock.manipulator(sxc);
            }
        };

        editManager.init();
        return editManager;
    }
})();
// https://tc39.github.io/ecma262/#sec-array.prototype.find
if (!Array.prototype.find) {
    Object.defineProperty(Array.prototype, 'find', {
        value: function (predicate) {
            // 1. Let O be ? ToObject(this value).
            if (this == null) { // jshint ignore:line
                throw new TypeError('"this" is null or not defined');
            }

            var o = Object(this);

            // 2. Let len be ? ToLength(? Get(O, "length")).
            var len = o.length >>> 0;

            // 3. If IsCallable(predicate) is false, throw a TypeError exception.
            if (typeof predicate !== 'function') {
                throw new TypeError('predicate must be a function');
            }

            // 4. If thisArg was supplied, let T be thisArg; else let T be undefined.
            var thisArg = arguments[1];

            // 5. Let k be 0.
            var k = 0;

            // 6. Repeat, while k < len
            while (k < len) {
                // a. Let Pk be ! ToString(k).
                // b. Let kValue be ? Get(O, Pk).
                // c. Let testResult be ToBoolean(? Call(predicate, T, « kValue, k, O »)).
                // d. If testResult is true, return kValue.
                var kValue = o[k];
                if (predicate.call(thisArg, kValue, k, o)) {
                    return kValue;
                }
                // e. Increase k by 1.
                k++;
            }

            // 7. Return undefined.
            return undefined;
        }
    });
}
if (typeof Object.assign != 'function') {
    Object.assign = function (target, varArgs) { // .length of function is 2
        'use strict';
        if (target === null) { // TypeError if undefined or null
            throw new TypeError('Cannot convert undefined or null to object');
        }

        var to = Object(target);

        for (var index = 1; index < arguments.length; index++) {
            var nextSource = arguments[index];

            if (nextSource !== null) { // Skip over if undefined or null
                for (var nextKey in nextSource) {
                    // Avoid bugs when hasOwnProperty is shadowed
                    if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
                        to[nextKey] = nextSource[nextKey];
                    }
                }
            }
        }
        return to;
    };
}
// this is a dialog manager which is in charge of all
// quick-dialogs. 
// it always has a reference to the latest dialog created by any module instance

(function () {
    var resizeInterval = 200;
    var scrollTopOffset = 80;
    var resizeWatcher = null;
    var diagShowClass = "dia-select";
    var isFullscreen = false;

    /**
     * dialog manager - the currently active dialog object
     */
    var diagManager = $2sxc._quickDialog = {
        current: null,

        /**
         * toggle visibility
         * @param {boolean} [show] true/false optional
         */
        toggle: function (show) {
            var cont = $(diagManager.getContainer());
            if (show === undefined)
                show = !cont.hasClass(diagShowClass);
            // show/hide visually
            cont.toggleClass(diagShowClass, show);
            diagManager.current = show ? diagManager.getIFrame() : null;
        },

        hide: function () {
            if (diagManager.current) diagManager.toggle(false);
        },

        /**
         * cancel the current dialog
         */
        cancel: function () {
            if (diagManager.current) diagManager.current.cancel(); // cancel & hide
        },

        /**
         * Remember dialog state across page-reload
         * @param {Object<any>} sxc - the sxc which is persisted for
         */
        persistDialog: function (sxc) {
            sessionStorage.setItem("dia-cbid", sxc.cbid);
        },

        /**
         * get the current container
         * @returns {element} html element of the div
         */
        getContainer: function () {
            var container = $(".inpage-frame-wrapper");
            return container.length > 0 ? container : buildContainerAndIFrame();
        },

        /**
         * find the iframe which hosts the dialog
         * @param {html} [container] - html-container as jQuery object
         * @returns {html} iframe object
         */
        getIFrame: function (container) {
            if (!container) container = diagManager.getContainer();
            return container.find("iframe")[0];
        },

        /**
         * check if the dialog is showing for the current sxc-instance
         * @param {Object<any>} sxc - sxc object
         * @param {string} dialogName - name of dialog
         * @returns {boolean} true if it's currently showing for this sxc-instance
         */
        isShowing: function (sxc, dialogName) {
            return diagManager.current // there is a current dialog
                &&
                diagManager.current.sxcCacheKey === sxc.cacheKey // the iframe is showing for the current sxc
                &&
                diagManager.current.dialogName === dialogName; // the view is the same as previously
        },

        /**
         * show / reset the current iframe to use new url and callback
         * @param {any} sxc - sxc object
         * @param {string} url - url to show
         * @param {function()} closeCallback - callback event
         * @param {boolean} fullScreen - if it should open full screen
         * @param {string} [dialogName] - optional name of dialog, to check if it's already open
         * @returns {any} jquery object of the iframe
         */
        showOrToggle: function (sxc, url, closeCallback, fullScreen, dialogName) {
            setSize(fullScreen);
            var iFrame = diagManager.getIFrame();

            // in case it's a toggle
            if (dialogName && diagManager.isShowing(sxc, dialogName))
                return diagManager.hide();

            iFrame.rewire(sxc, closeCallback, dialogName);
            iFrame.setAttribute("src", rewriteUrl(url));
            // if the window had already been loaded, re-init
            if (iFrame.contentWindow && iFrame.contentWindow.reboot)
                iFrame.contentWindow.reboot();

            // make sure it's visible'
            iFrame.toggle(true);
            return iFrame;
        }
    };

    /**
     * build the container in the dom w/iframe for re-use
     * @return {jquery} jquery dom-object 
     */
    function buildContainerAndIFrame() {
        var container = $('<div class="inpage-frame-wrapper"><div class="inpage-frame"></div></div>');
        var newIFrame = document.createElement("iframe");
        newIFrame = extendIFrameWithSxcState(newIFrame);
        container.find(".inpage-frame").html(newIFrame);
        $("body").append(container);

        watchForResize();
        return container;
    }

    function setSize(fullScreen) {
        var container = diagManager.getContainer();
        // set container height
        container.css("min-height", fullScreen ? "100%" : "225px");
        isFullscreen = fullScreen;
    }

    function extendIFrameWithSxcState(iFrame) {
        var hiddenSxc = null;
        var cbApi = $2sxc._contentBlock;
        var tagModule = null;

        /**
         * get the sxc-object of this iframe
         * @returns {Object<any>} refreshed sxc-object
         */
        function reSxc() {
            if (!hiddenSxc) throw "can't find sxc-instance of IFrame, probably it wasn't initialized yet";
            return hiddenSxc.recreate();
        }

        var newFrm = Object.assign(iFrame, {
            closeCallback: null,
            rewire: function (sxc, callback, dialogName) {
                hiddenSxc = sxc;
                tagModule = $($($2sxc._manage.getTag(sxc)).parent().eq(0));
                newFrm.sxcCacheKey = sxc.cacheKey;
                newFrm.closeCallback = callback;
                if (dialogName) newFrm.dialogName = dialogName;
            },
            getManageInfo: function () {
                return reSxc().manage._dialogParameters;
            },
            getAdditionalDashboardConfig: function () {
                return reSxc().manage._quickDialogConfig;
            },
            persistDia: function () {
                diagManager.persistDialog(reSxc());
            },
            scrollToTarget: function () {
                $("body").animate({
                    scrollTop: tagModule.offset().top - scrollTopOffset
                });
            },
            toggle: function (show) {
                diagManager.toggle(show);
            },
            cancel: function () {
                newFrm.toggle(false);
                //todo: only re-init if something was changed?
                // return cbApi.reloadAndReInitialize(reSxc());

                // cancel the dialog
                localStorage.setItem('cancelled-dialog', true);
                return newFrm.closeCallback();
            },
            run: function (verb) {
                reSxc().manage.run(verb);
            },
            showMessage: function (message) {
                cbApi.showMessage(reSxc(), '<p class="no-live-preview-available">' + message + "</p>");
            },
            reloadAndReInit: function () {
                return cbApi.reloadAndReInitialize(reSxc(), true, true);
            },
            saveTemplate: function (templateId) {
                return cbApi.updateTemplateFromDia(reSxc(), templateId, false);
            },
            previewTemplate: function (templateId) {
                return cbApi.ajaxLoad(reSxc(), templateId, true);
            }
        });
        return newFrm;
    }

    /**
     * rewrite the url to fit the quick-dialog situation
     * optionally with a live-compiled version from ng-serve
     * @param {string} url - original url pointing to the "wrong" dialog
     * @returns {string} new url
     */
    function rewriteUrl(url) {
        // change default url-schema from the primary angular-app to the quick-dialog
        url = url.replace("dist/dnn/ui.html?", "dist/ng/ui.html?");

        // special debug-code when running on local ng-serve
        // this is only activated if the developer manually sets a value in the localStorage
        try {
            var devMode = localStorage.getItem("devMode");
            if (devMode && ~~devMode)
                url = url.replace("/desktopmodules/tosic_sexycontent/dist/ng/ui.html", "http://localhost:4200");
        } catch (e) {
            // ignore
        }
        return url;
    }

    /**
     * create watcher which monitors the iframe size and adjusts the container as needed
     * @param {boolean} [keepWatching] optional true/false to start/stop the watcher
     * @returns {null} nothing
     */
    function watchForResize(keepWatching) {
        if (keepWatching === false && resizeWatcher) {
            clearInterval(resizeWatcher);
            resizeWatcher = null;
            return null;
        }

        var cont = diagManager.getContainer();
        if (!resizeWatcher) // only add a timer if not already running
            resizeWatcher = setInterval(function () {
                try {
                    var frm = diagManager.getIFrame(cont);
                    if (!frm) return;
                    var height = frm.contentDocument.body.offsetHeight;
                    if (frm.previousHeight === height) return;
                    frm.style.minHeight = cont.css("min-height");
                    frm.style.height = height + "px";
                    frm.previousHeight = height;
                    if (isFullscreen) {
                        frm.style.height = "100%";
                        frm.style.position = "absolute";
                    }
                } catch (e) {
                    // ignore
                }
            }, resizeInterval);
        return resizeWatcher;
    }
})();
$(function () {
    "use strict";

    // the quick-edit object
    var $quickE = window.$quickE = {};

    // selectors used all over the in-page-editing, centralized to ensure consistency
    $quickE.selectors = {
        cb: {
            id: "cb",
            "class": "sc-content-block",
            selector: ".sc-content-block",
            listSelector: ".sc-content-block-list",
            context: "data-list-context",
            singleItem: "single-item"
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


    $quickE.btn = function(action, icon, i18N, invisible, unavailable, classes) {
        return "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-" + icon + " "
            + (invisible ? " sc-invisible " : "")
            + (unavailable ? " sc-unavailable " : "")
            + classes + "' data-action='" + action + "' data-i18n='[title]QuickInsertMenu." + i18N + "'></a>";
    };

    // the quick-insert object
    $.extend($quickE, {
        body: $("body"),
        win: $(window),
        main: $("<div class='sc-content-block-menu sc-content-block-quick-insert sc-i18n'></div>"),
        template: "<a class='sc-content-block-menu-addcontent sc-invisible' data-type='Default' data-i18n='[titleTemplate]QuickInsertMenu.AddBlockContent'>x</a>"
            + "<a class='sc-content-block-menu-addapp sc-invisible' data-type='' data-i18n='[titleTemplate]QuickInsertMenu.AddBlockApp'>x</a>"
            + $quickE.btn("select", "ok", "Select", true)
            + $quickE.btn("paste", "paste", "Paste", true, true),
        selected: $("<div class='sc-content-block-menu sc-content-block-selected-menu sc-i18n'></div>")
            .append(
                $quickE.btn("delete", "trash-empty", "Delete"),
                $quickE.btn("sendToPane", "export", "Move", null, null, "sc-cb-mod-only"),
                "<div id='paneList'></div>"
            ),
        contentBlocks: null,
        cachedPanes: null,
        modules: null,
        nearestCb: null, 
        nearestMod: null,
        modManage: null // will be populated later in the module section
    });

    // add stuff which dependes on other values to create
    $.extend($quickE, {
        cbActions: $($quickE.template),
        modActions: $($quickE.template.replace(/QuickInsertMenu.AddBlock/g, "QuickInsertMenu.AddModule"))
            .attr("data-context", "module")
            .addClass("sc-content-block-menu-module")
    });

    // build the toolbar (hidden, but ready to show)
    $quickE.prepareToolbarInDom = function() {
        $quickE.body.append($quickE.main)
            .append($quickE.selected);
        $quickE.main.append($quickE.cbActions)
            .append($quickE.modActions);
    };

});
// add a clipboard to the quick edit
$(function () {

    // perform copy and paste commands - needs the clipboard
    $quickE.copyPasteInPage = function (cbAction, list, index, type) {
        var newClip = $quickE.clipboard.createSpecs(type, list, index);

        // action!
        switch (cbAction) {
            case "select":
                $quickE.clipboard.mark(newClip);
                break;
            case "paste":
                var from = $quickE.clipboard.data.index, to = newClip.index;
                // check that we only move block-to-block or module to module
                if ($quickE.clipboard.data.type !== newClip.type)
                    return alert("can't move module-to-block; move only works from module-to-module or block-to-block");

                if (isNaN(from) || isNaN(to) || from === to) // || from + 1 === to) // this moves it to the same spot, so ignore
                    return $quickE.clipboard.clear(); // don't do anything

                // cb-numbering is a bit different, because the selector is at the bottom
                // only there we should also skip on +1;
                if(newClip.type === $quickE.selectors.cb.id && from + 1 === to)
                    return $quickE.clipboard.clear(); // don't do anything

                if (type === $quickE.selectors.cb.type) {
                    $2sxc(list).manage._getCbManipulator().move(newClip.parent, newClip.field, from, to);
                } else {
                    $quickE.cmds.mod.move($quickE.clipboard.data, newClip, from, to);
                }
                $quickE.clipboard.clear();
                break;
            default:
        }
        return null;
    };

    // clipboard object - remembers what module (or content-block) was previously copied / needs to be pasted
    $quickE.clipboard = {
        data: {},
        mark: function (newData) {
            if (newData) {
                // if it was already selected with the same thing, then release it
                if ($quickE.clipboard.data && $quickE.clipboard.data.item === newData.item)
                    return $quickE.clipboard.clear();
                $quickE.clipboard.data = newData;
            }
            $("." + $quickE.selectors.selected).removeClass($quickE.selectors.selected); // clear previous markings
            var cb = $($quickE.clipboard.data.item);
            cb.addClass($quickE.selectors.selected);
            if (cb.prev().is("iframe"))
                cb.prev().addClass($quickE.selectors.selected);
            $quickE.setSecondaryActionsState(true);
            $quickE.selected.toggle(cb, $quickE.clipboard.data.type);
        },
        clear: function () {
            $("." + $quickE.selectors.selected).removeClass($quickE.selectors.selected);
            $quickE.clipboard.data = null;
            $quickE.setSecondaryActionsState(false);
            $quickE.selected.toggle(false);
        },

        createSpecs: function (type, list, index) {
            var listItems = list.find($quickE.selectors[type].selector);
            if (index >= listItems.length) index = listItems.length - 1; // sometimes the index is 1 larger than the length, then select last
            var currentItem = listItems[index];
            var editContext = JSON.parse(list.attr($quickE.selectors.cb.context) || null) || { parent: "dnn", field: list.id };
            return { parent: editContext.parent, field: editContext.field, list: list, item: currentItem, index: index, type: type };
        }
    };


    $quickE.setSecondaryActionsState = function (state) {
        var btns = $("a.sc-content-block-menu-btn");
        btns = btns.filter(".icon-sxc-paste");
        btns.toggleClass("sc-unavailable", !state);
    };


    $quickE.selected.toggle = function (target) {
        if (!target)
            return $quickE.selected.hide();

        var coords = $quickE.getCoordinates(target);
        coords.yh = coords.y + 20;
        $quickE.positionAndAlign($quickE.selected, coords);
        $quickE.selected.target = target;
    };

    // bind clipboard actions 
    $("a", $quickE.selected).click(function () {
        var action = $(this).data("action");
        var clip = $quickE.clipboard.data;
        switch (action) {
            case "delete":
                return $quickE.cmds[clip.type].delete(clip);
            case "sendToPane":
                return $quickE.cmds.mod.sendToPane(clip);
        }
    });

});
// extend the quick edit with the core commands
$(function () {
    $quickE.cmds = {
        cb: {
            "delete": function (clip) {
                return $2sxc(clip.list).manage._getCbManipulator().delete(clip.parent, clip.field, clip.index);
            },
            "create": function(parent, field, index, appOrContent, list, newGuid) {
                return $2sxc(list).manage._getCbManipulator().create(parent, field, index, appOrContent, list, newGuid);
            }
        },
        mod: {
            "delete": function (clip) {
                if (!confirm("are you sure?")) return;
                var modId = $quickE.modManage.getModuleId(clip.item.className);
                $quickE.modManage.delete(modId);
            },
            // todo: unsure if this is a good place for this bit of code...
            move: function (oldClip, newClip, from, to) {
                var modId = $quickE.modManage.getModuleId(oldClip.item.className);
                var pane = $quickE.modManage.getPaneName(newClip.list);
                $quickE.modManage.move(modId, pane, to);
            },
            sendToPane: function() {
                var pane = $quickE.main.actionsForModule.closest($quickE.selectors.mod.listSelector);

                // show the pane-options
                var pl = $quickE.selected.find("#paneList");
                if (!pl.is(":empty"))
                    pl.empty();
                pl.append($quickE.modManage.getMoveButtons($quickE.modManage.getPaneName(pane)));

            }
        }
    };



});
$(function () {
    var configAttr = "quick-edit-config";

    // the initial configuration
    var conf = $quickE.config = {
        enable: true,
        innerBlocks: {
            enable: null    // default: auto-detect
        },
        modules: {
            enable: null    // default: auto-detect
        }
    };
    
    $quickE._readPageConfig = function () {
        var configs = $("[" + configAttr + "]"), finalConfig = {}, confJ, confO;

        // any inner blocks found? will currently affect if modules can be inserted...
        var hasInnerCBs = ($($quickE.selectors.cb.listSelector).length > 0);

        if (configs.length > 0) {
            // go through reverse list, as the last is the most important...
            for (var c = configs.length; c >= 0; c--) {
                confJ = configs[0].getAttribute(configAttr);
                try {
                    confO = JSON.parse(confJ);
                    $.extend(finalConfig, confO);
                } catch (e) {
                    console.warn('had trouble with json', e);
                }
            }
            $.extend(conf, finalConfig);
        }

        // re-check "auto" or "null"
        // if it has inner-content, then it's probably a details page, where quickly adding modules would be a problem, so for now, disable modules in this case
        if (conf.modules.enable === null || conf.modules.enable === 'auto') conf.modules.enable = !hasInnerCBs;

        // for now, ContentBlocks are only enabled if they exist on the page
        if (conf.innerBlocks.enable === null || conf.innerBlocks.enable === 'auto') conf.innerBlocks.enable = hasInnerCBs;  
    };

});
// content-block specific stuff like actions
$(function () {

    function onCbButtonClick () {
        var list = $quickE.main.actionsForCb.closest($quickE.selectors.cb.listSelector),
            listItems = list.find($quickE.selectors.cb.selector),
            actionConfig = JSON.parse(list.attr($quickE.selectors.cb.context)),
            index = 0,
            newGuid = actionConfig.guid || null;

        if ($quickE.main.actionsForCb.hasClass($quickE.selectors.cb.class))
            index = listItems.index($quickE.main.actionsForCb[0]) + 1;

        // check cut/paste
        var cbAction = $(this).data("action");
        if (cbAction)
            // this is a cut/paste action
            return $quickE.copyPasteInPage(cbAction, list, index, $quickE.selectors.cb.id);
        else {
            var appOrContent = $(this).data("type");
            return $quickE.cmds.cb.create(actionConfig.parent, actionConfig.field, index, appOrContent, list, newGuid);
        } 
    }

    $quickE.cbActions.click(onCbButtonClick);
});
// module specific stuff
$(function () {
    "use strict";

    $quickE.modManage = {
        "delete": deleteMod,
        create: createModWithTypeName,
        move: moveMod,
        getPaneName: getPaneName,
        getModuleId: getModuleId,
        getMoveButtons: generatePaneMoveButtons
    };


    function getPaneName(pane) {
        return $(pane).attr("id").replace("dnn_", "");
    }

    // find the correct module id from a list of classes - used on the module-wrapper
    function getModuleId(classes) {
        var result = classes.match(/DnnModule-([0-9]+)(?:\W|$)/);
        return (result && result.length === 2) ? result[1] : null;
    }

    // show an error when an xhr error occurs
    function xhrError (xhr, optionalMessage) {
        alert(optionalMessage || "Error while talking to server.");
        console.log(xhr);
    }

    // service calls we'll need
    function createModWithTypeName(paneName, index, type) {
        return sendDnnAjax(null, "controlbar/GetPortalDesktopModules", {
            data: "category=All&loadingStartIndex=0&loadingPageSize=100&searchTerm=",
            success: function (desktopModules) {
                var moduleToFind = type === "Default" ? " Content" : " App";
                var module = null;

                desktopModules.forEach(function (e, i) {
                    if (e.ModuleName === moduleToFind)
                        module = e;
                });

                return (!module)
                    ? alert(moduleToFind + " module not found.")
                    : createMod(paneName, index, module.ModuleID);
            }
        });
    }

    // move a dnn module
    function moveMod(modId, pane, order) {
        var service = $.dnnSF(modId);
        var tabId = service.getTabId();
        var dataVar = {
            TabId: tabId,
            ModuleId: modId,
            Pane: pane,
            ModuleOrder: (2 * order + 4) // strange formula, copied from DNN https://github.com/dnnsoftware/Dnn.Platform/blob/fd225b8de07042837f7473cd49fba13de42a3cc0/Website/admin/Menus/ModuleActions/ModuleActions.js#L70
        };

        sendDnnAjax(modId, "ModuleService/MoveModule", {
            type: "POST",
            data: dataVar,
            success: function () {
                window.location.reload();
            }
        });

        //fire window resize to reposition action menus
        $(window).resize();
    }
    
    // delete a module
    function deleteMod(modId) {
        var service = $.dnnSF(modId);
        var tabId = service.getTabId();
        return sendDnnAjax(modId, "2sxc/dnn/module/delete", {
            url: $.dnnSF().getServiceRoot("2sxc") + "dnn/module/delete",
            type: "GET",
            data: {
                tabId: tabId,
                modId: modId
            },
            success: function(d) {
                window.location.reload();
            }
        });
    }

    // call an api on dnn
    function sendDnnAjax(modId, serviceName, options) {
        var service = $.dnnSF(modId);

        return $.ajax($.extend( {
            type: "GET",
            url: service.getServiceRoot("internalservices") + serviceName,
            beforeSend: service.setModuleHeaders,
            error: xhrError
        }, options));
    }

    // create / insert a new module
    function createMod(paneName, position, modId) {
        var postData = {
            Module: modId,
            Page: "",
            Pane: paneName,
            Position: -1,
            Sort: position,
            Visibility: 0,
            AddExistingModule: false,
            CopyModule: false
        };
        return sendDnnAjax(null, "controlbar/AddModule", {
            type: "POST",
            data: postData,
            success: function (d) {
                window.location.reload();
            }
        });
    }


    function generatePaneMoveButtons(current) {
        var pns = $quickE.cachedPanes;
        // generate list of panes as links
        var targets = $("<div>");
        for (var p = 0; p < pns.length; p++) {
            var pName = $quickE.modManage.getPaneName(pns[p]),
                selected = (current === pName) ? " selected " : "";
            if (!selected)
                targets.append("<a data='" + pName + "'>" + pName + "</a>");
        }

        // attach click event...
        targets.find("a").click(function (d) {
            var link = $(this),
                clip = $quickE.clipboard.data,
                modId = $quickE.modManage.getModuleId(clip.item.className),
                newPane = link.attr("data");

            $quickE.modManage.move(modId, newPane, 0);
        });

        return targets;
    }

});
// module specific stuff
$(function () {

    function onModuleButtonClick() {
        var type = $(this).data("type"),
            dnnMod = $quickE.main.actionsForModule,
            pane = dnnMod.closest($quickE.selectors.mod.listSelector),
            index = 0;

        if (dnnMod.hasClass("DnnModule"))
            index = pane.find(".DnnModule").index(dnnMod[0]) + 1;

        var cbAction = $(this).data("action");
        if (cbAction)  // copy/paste
            return $quickE.copyPasteInPage(cbAction, pane, index, $quickE.selectors.mod.id);

        return $quickE.modManage.create($quickE.modManage.getPaneName(pane), index, type);
    }

    // bind module actions click
    $quickE.modActions.click(onModuleButtonClick);
});
// everything related to positioning the quick-edit in-page editing
$(function () {


    // Prepare offset calculation based on body positioning
    $quickE.getBodyPosition = function () {
        var bodyPos = $quickE.body.css("position");
        return bodyPos === "relative" || bodyPos === "absolute"
            ? { x: $quickE.body.offset().left, y: $quickE.body.offset().top }
            : { x: 0, y: 0 };
    };

    // Refresh content block and modules elements
    $quickE.refreshDomObjects = function () {
        $quickE.bodyOffset = $quickE.getBodyPosition(); // must update this, as sometimes after finishing page load the position changes, like when dnn adds the toolbar

        //// Cache the panes (because panes can't change dynamically)
        //if (!$quickE.cachedPanes)
        //    $quickE.cachedPanes = $($quickE.selectors.mod.listSelector);
        
        if ($quickE.config.innerBlocks.enable) {
            // get all content-block lists which are empty, or which allow multiple child-items
            var lists = $($quickE.selectors.cb.listSelector)
                .filter(":not(." + $quickE.selectors.cb.singleItem + "), :empty");
            $quickE.contentBlocks = lists // $($quickE.selectors.cb.listSelector)
                .find($quickE.selectors.cb.selector)
                .add(lists);// $quickE.selectors.cb.listSelector);
        }
        if ($quickE.config.modules.enable)
            $quickE.modules = $quickE.cachedPanes
                .find($quickE.selectors.mod.selector)
                .add($quickE.cachedPanes);
    };

    // position, align and show a menu linked to another item
    $quickE.positionAndAlign = function (element, coords) {
        return element.css({
            left: coords.x - $quickE.bodyOffset.x,
            top: coords.yh - $quickE.bodyOffset.y,
            width: coords.element.width()
        }).show();
    };

    // Refresh positioning / visibility of the quick-insert bar
    $quickE.refresh = function (e) {
        var highlightClass = "sc-cb-highlight-for-insert";

        if (!$quickE.refreshDomObjects.lastCall || (new Date() - $quickE.refreshDomObjects.lastCall > 1000)) {
            // console.log('refreshed contentblock and modules');
            $quickE.refreshDomObjects.lastCall = new Date();
            $quickE.refreshDomObjects();
        }

        if ($quickE.config.innerBlocks.enable && $quickE.contentBlocks) {
            $quickE.nearestCb = $quickE.findNearest($quickE.contentBlocks, { x: e.clientX, y: e.clientY }, $quickE.selectors.cb.selector);
        }

        if ($quickE.config.modules.enable && $quickE.modules) {
            $quickE.nearestMod = $quickE.findNearest($quickE.modules, { x: e.clientX, y: e.clientY }, $quickE.selectors.mod.selector);
        }

        $quickE.modActions.toggleClass("sc-invisible", $quickE.nearestMod === null);
        $quickE.cbActions.toggleClass("sc-invisible", $quickE.nearestCb === null);

        var oldParent = $quickE.main.parentContainer;

        if ($quickE.nearestCb !== null || $quickE.nearestMod !== null) {
            var alignTo = $quickE.nearestCb || $quickE.nearestMod;

            // find parent pane to highlight
            var parentPane = $(alignTo.element).closest($quickE.selectors.mod.listSelector);
            var parentCbList = $(alignTo.element).closest($quickE.selectors.cb.listSelector);
            var parentContainer = (parentCbList.length ? parentCbList : parentPane)[0];

            // put part of the pane-name into the button-labels
            if (parentPane.length > 0) {
                var paneName = parentPane.attr("id") || "";
                if (paneName.length > 4) paneName = paneName.substr(4);
                $quickE.modActions.filter("[titleTemplate]").each(function () {
                    var t = $(this);
                    t.attr("title", t.attr("titleTemplate").replace("{0}", paneName));
                });
            }

            $quickE.positionAndAlign($quickE.main, alignTo, true);

            // Keep current block as current on menu
            $quickE.main.actionsForCb = $quickE.nearestCb ? $quickE.nearestCb.element : null;
            $quickE.main.actionsForModule = $quickE.nearestMod ? $quickE.nearestMod.element : null;
            $quickE.main.parentContainer = parentContainer;
            $(parentContainer).addClass(highlightClass);
        } else {
            $quickE.main.parentContainer = null;
            $quickE.main.hide();
        }

        // if previously a parent-pane was highlighted, un-highlight it now
        if (oldParent && oldParent !== $quickE.main.parentContainer)
            $(oldParent).removeClass(highlightClass);
    };

    // Return the nearest element to the mouse cursor from elements (jQuery elements)
    $quickE.findNearest = function (elements, position) {
        var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed

        var nearestItem = null;
        var nearestDistance = maxDistance;

        var posX = position.x + $quickE.win.scrollLeft();
        var posY = position.y + $quickE.win.scrollTop();

        // Find nearest element
        elements.each(function () {
            var e = $quickE.getCoordinates($(this));

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

    $quickE.getCoordinates = function (element) {
        return {
            element: element,
            x: element.offset().left,
            w: element.width(),
            y: element.offset().top,
            // For content-block ITEMS, the menu must be visible at the end
            // For content-block-LISTS, the menu must be at top
            yh: element.offset().top + (element.is($quickE.selectors.eitherCbOrMod) ? element.height() : 0)
        };
    };

});
$(function () {
    $quickE.enable = function () {
        // build all toolbar html-elements
        $quickE.prepareToolbarInDom();

        // Cache the panes (because panes can't change dynamically)
        $quickE.initPanes();
    };

    // start watching for mouse-move
    $quickE.watchMouse = function() {
        var refreshTimeout = null;
        $("body").on("mousemove", function (e) {
            if (refreshTimeout === null)
                refreshTimeout = window.setTimeout(function() {
                    requestAnimationFrame(function() {
                        $quickE.refresh(e);
                        refreshTimeout = null;
                    });
                }, 20);
        });
    };

    $quickE.start = function() {
        try {
            $quickE._readPageConfig();
            if ($quickE.config.enable) {
                // initialize first body-offset
                $quickE.bodyOffset = $quickE.getBodyPosition();

                $quickE.enable();

                $quickE.toggleParts();

                $quickE.watchMouse();
            }
        } catch (e) {
            console.error("couldn't start quick-edit", e);
        }
    };

    // cache the panes which can contain modules
    $quickE.initPanes = function () {
        $quickE.cachedPanes = $($quickE.selectors.mod.listSelector);
        $quickE.cachedPanes.addClass("sc-cb-pane-glow");
    };

    // enable/disable module/content-blocks as configured
    $quickE.toggleParts = function () {
        //// content blocks actions
        //$quickE.cbActions.toggle($quickE.config.innerBlocks.enable);

        //// module actions
        //$quickE.modActions.hide($quickE.config.modules.enable);
    };

    // reset the quick-edit
    // for example after ajax-loading a content-block, which may cause changed configurations
    $quickE.reset = function() {
        $quickE._readPageConfig();
        $quickE.toggleParts();
    };

    // run on-load
    $($quickE.start);
});
/*
 * Author: Alex Gibson
 * https://github.com/alexgibson/shake.js
 * License: MIT license
 */

(function(global, factory) {
    if (typeof define === 'function' && define.amd) {
        define(function() {
            return factory(global, global.document);
        });
    } else if (typeof module !== 'undefined' && module.exports) {
        module.exports = factory(global, global.document);
    } else {
        global.Shake = factory(global, global.document);
    }
} (typeof window !== 'undefined' ? window : this, function (window, document) {

    'use strict';

    function Shake(options) {
        //feature detect
        this.hasDeviceMotion = 'ondevicemotion' in window;

        this.options = {
            threshold: 15, //default velocity threshold for shake to register
            timeout: 1000,
            callback: null // callback - will only be used if provided, otherwise generate event // function() {}//default interval between events
        };

        if (typeof options === 'object') {
            for (var i in options) {
                if (options.hasOwnProperty(i)) {
                    this.options[i] = options[i];
                }
            }
        }

        //use date to prevent multiple shakes firing
        this.lastTime = new Date();

        //accelerometer values
        this.lastX = null;
        this.lastY = null;
        this.lastZ = null;
    }

    //reset timer values
    Shake.prototype.reset = function () {
        this.lastTime = new Date();
        this.lastX = null;
        this.lastY = null;
        this.lastZ = null;
    };

    //start listening for devicemotion
    Shake.prototype.start = function () {
        this.reset();
        if (this.hasDeviceMotion) {
            window.addEventListener('devicemotion', this, false);
        }
    };

    //stop listening for devicemotion
    Shake.prototype.stop = function () {
        if (this.hasDeviceMotion) {
            window.removeEventListener('devicemotion', this, false);
        }
        this.reset();
    };

    //calculates if shake did occur
    Shake.prototype.devicemotion = function (e) {
        var current = e.accelerationIncludingGravity;
        var currentTime;
        var timeDifference;
        var deltaX = 0;
        var deltaY = 0;
        var deltaZ = 0;

        if ((this.lastX === null) && (this.lastY === null) && (this.lastZ === null)) {
            this.lastX = current.x;
            this.lastY = current.y;
            this.lastZ = current.z;
            return;
        }

        deltaX = Math.abs(this.lastX - current.x);
        deltaY = Math.abs(this.lastY - current.y);
        deltaZ = Math.abs(this.lastZ - current.z);

        if (((deltaX > this.options.threshold) && (deltaY > this.options.threshold)) || ((deltaX > this.options.threshold) && (deltaZ > this.options.threshold)) || ((deltaY > this.options.threshold) && (deltaZ > this.options.threshold))) {
            //calculate time in milliseconds since last shake registered
            currentTime = new Date();
            timeDifference = currentTime.getTime() - this.lastTime.getTime();
            
            if (timeDifference > this.options.timeout) {
                // once triggered, execute  the callback
                if( typeof this.options.callback === 'function' ) {
                    this.options.callback();
                }
                else
                    console.log("shake event without callback detected");
                this.lastTime = new Date();
            }
        }

        this.lastX = current.x;
        this.lastY = current.y;
        this.lastZ = current.z;
    };

    //event handler
    Shake.prototype.handleEvent = function (e) {
        if (typeof (this[e.type]) === 'function') {
            return this[e.type](e);
        }
    };

    return Shake;
}));

(function () {
    
    // prevent propagation of the click (if menu was clicked)
    $($2sxc.c.sel.scMenu /*".sc-menu"*/).click(function (e) {
        e.stopPropagation();
    });
})();
// enable shake detection on all toolbars
$(function () {

    // this will add a css-class to auto-show all toolbars (or remove it again)
    function toggleAllToolbars() {
        $(document.body).toggleClass('sc-tb-show-all');
    }
    
    // start shake-event monitoring, which will then generate a window-event
    (new Shake({ callback: toggleAllToolbars })).start();
});
// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.
(function () {
    /**
     * Toolbar manager for the whole page - basically a set of APIs
     */
    $2sxc._toolbarManager = {

        // internal constants
        cDisableAttrName: "data-disable-toolbar"
    };
})();
(function () {
    // quick debug - set to false if not needed for production
    var dbg = false;

    // default / fallback settings for toolbars when nothings is specified
    var settingsForEmptyToolbar = {
        hover: 'left',
        autoAddMore: 'left'
    };

    Object.assign($2sxc._toolbarManager, {
        buildToolbars: buildToolbars,
        disable: disable,
        isDisabled: isDisabled
    });

    return;

    // generate an empty / fallback toolbar tag
    function generateFallbackToolbar() {
        var settingsString = JSON.stringify(settingsForEmptyToolbar);
        return $("<ul class='sc-menu' toolbar='' settings='" + settingsString + "'/>");
    }

    // find current toolbars inside this wrapper-tag
    function getToolbarTags(parentTag) {
        var allInner = $(".sc-menu[toolbar],.sc-menu[data-toolbar]", parentTag);

        // return only those, which don't belong to a sub-item
        var res = allInner.filter(function (i, e) {
            return $(e).closest(".sc-content-block")[0] === parentTag[0];
        });
        if (dbg) console.log("found toolbars for parent", parentTag, res);
        return res;
    }

    // create a process-toolbar command to generate toolbars inside a tag
    function buildToolbars(parentTag, optionalId) {
        parentTag = $(parentTag || ".DnnModule-" + optionalId);

        // if something says the toolbars are disabled, then skip
        if (parentTag.attr($2sxc._toolbarManager.cDisableAttrName)) return;

        // todo: change mechanism to not render toolbar, this uses a secret class name which the toolbar shouldn't know
        // don't add, if it is has un-initialized content
        // 2017-09-08 2dm disabled this, I believe the bootstrapping should never call this any more, if sc-uninitialized. if ok, then delete this in a few days
        //var disableAutoAdd = $(".sc-uninitialized", parentTag).length !== 0;

        var toolbars = getToolbarTags(parentTag);

        // no toolbars found, must help a bit because otherwise editing is hard
        if (toolbars.length === 0) { // && !disableAutoAdd) {
            if (dbg) console.log("didn't find toolbar, so will auto-create", parentTag);

            var outsideCb = !parentTag.hasClass($2sxc.c.cls.scCb); // "sc-content-block");
            var contentTag = outsideCb ? parentTag.find("div.sc-content-block") : parentTag;
            contentTag.addClass($2sxc.c.cls.scElm); // "sc-element");

            contentTag.prepend(generateFallbackToolbar());
            toolbars = getToolbarTags(parentTag);
        }

        toolbars.each(function initToolbar() {
            var tag = $(this),
                data = null,
                toolbarConfig, toolbarSettings, at = $2sxc.c.attr;

            try {
                data = tag.attr(at.toolbar) || tag.attr(at.toolbarData) || "{}";
                toolbarConfig = JSON.parse(data);
                data = tag.attr(at.settings) || tag.attr(at.settingsData) || "{}";
                toolbarSettings = JSON.parse(data);
                if (toolbarConfig === {} && toolbarSettings === {})
                    toolbarSettings = settingsForEmptyToolbar;
            } catch (err) {
                console
                    .error("error in settings JSON - probably invalid - make sure you also quote your properties like \"name\": ...", data, err);
                return;
            }

            try {
                tag.replaceWith($2sxc(tag).manage.getToolbar(toolbarConfig, toolbarSettings));
            } catch (err2) {
                // note: errors happen a lot on custom toolbars, amke sure the others are still rendered
                console.error("error creating toolbar - will skip this one", err2);
            }
        });
    }
    
    function disable(tag) {
        tag = $(tag);
        tag.attr($2sxc._toolbarManager.cDisableAttrName, true);
    }

    function isDisabled(sxc) {
        var tag = $($2sxc._manage.getTag(sxc));
        return !!tag.attr($2sxc._toolbarManager.cDisableAttrName);
    }
})();
(function () {
    $2sxc._toolbarManager.generateButtonHtml = generateButtonHtml;
    return;

    // does some clean-up work on a button-definition object
    // because the target item could be specified directly, or in a complex internal object called entity
    function flattenActionDefinition(actDef) {
        if (!actDef.entity || !actDef.entity._2sxcEditInformation) return;

        var editInfo = actDef.entity._2sxcEditInformation;
        actDef.useModuleList = (editInfo.sortOrder !== undefined); // has sort-order, so use list
        if (editInfo.entityId !== undefined) actDef.entityId = editInfo.entityId;
        if (editInfo.sortOrder !== undefined) actDef.sortOrder = editInfo.sortOrder;
        delete actDef.entity; // clean up edit-info
    }

    // generate the html for a button
    // Expects: instance sxc, action-definition, + group-index in which the button is shown
    function generateButtonHtml(sxc, actDef, groupIndex) {

        // if the button belongs to a content-item, move the specs up to the item into the settings-object
        flattenActionDefinition(actDef);

        // retrieve configuration for this button
        var showClasses = "group-" + groupIndex + (actDef.disabled ? " disabled" : ""),
            classesList = (actDef.classes || "").split(","),
            box = $("<div/>"),
            symbol = $("<i class=\"" + actDef.icon + "\" aria-hidden=\"true\"></i>"),
            onclick = actDef.disabled ?
            "" :
            "$2sxc(" + sxc.id + ", " + sxc.cbid + ").manage.run(" + JSON.stringify(actDef.command) + ", event);";

        for (var c = 0; c < classesList.length; c++) showClasses += " " + classesList[c];

        var button = $("<a />", {
            'class': "sc-" + actDef.action + " " + showClasses +
                (actDef.dynamicClasses ? " " + actDef.dynamicClasses(actDef) : ""),
            'onclick': onclick,
            'data-i18n': "[title]" + actDef.title
        });
        button.html(box.html(symbol));
        return button[0].outerHTML;
    }
})();
(function () {
    var tbManager = $2sxc._toolbarManager;
    $2sxc._toolbarManager.generateToolbarHtml = generateToolbarHtml;
    return;

    function generateToolbarHtml(sxc, tbConfig, moreSettings) {
        // if it has an action or is an array, keep that. Otherwise get standard buttons
        tbConfig = tbConfig || {}; // if null/undefined, use empty object
        var btnList = tbConfig;
        if (!tbConfig.action && !tbConfig.groups && !tbConfig.buttons && !Array.isArray(tbConfig))
            btnList = tbManager.standardButtons(sxc.manage._user.canDesign /* editContext.User.CanDesign */ , tbConfig);

        // whatever we had, if more settings were provided, override with these...
        var tlbDef = tbManager.buttonHelpers.buildFullDefinition(btnList, sxc.manage._commands.commands, sxc.manage._instanceConfig /* tb.config */ , moreSettings);
        var btnGroups = tlbDef.groups;
        var behaviourClasses = " sc-tb-hover-" + tlbDef.settings.hover + " sc-tb-show-" + tlbDef.settings.show;

        // todo: these settings assume it's not in an array...
        var tbClasses = "sc-menu group-0 " + behaviourClasses + " " +
            ((tbConfig.sortOrder === -1) ? " listContent" : "") +
            (tlbDef.settings.classes ? " " + tlbDef.settings.classes : "");
        var toolbar = $("<ul />", {
            'class': tbClasses,
            'onclick': "var e = arguments[0] || window.event; e.stopPropagation();"
        });

        for (var i = 0; i < btnGroups.length; i++) {
            var btns = btnGroups[i].buttons;
            for (var h = 0; h < btns.length; h++)
                toolbar.append($("<li />").append($(tbManager.generateButtonHtml(sxc, btns[h], i))));
        }

        toolbar.attr("group-count", btnGroups.length);
        return toolbar[0].outerHTML;
    }
})();
// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.
(function () {

    // ToDo: refactor to avoid side-effects
    var tools = $2sxc._toolbarManager.buttonHelpers = {

        defaultSettings: {
            autoAddMore: null,     // null | "right" | "start" | true
            hover: "right",         // right | left | none
            show: "hover",          // always | hover
            // order or reverse, still thinking about this --> order: "default"    // default | reverse
        },

        // take any common input format and convert it to a full toolbar-structure definition
        // can handle the following input formats (the param unstructuredConfig):
        // complete tree (detected by "groups): { groups: [ {}, {}], name: ..., defaults: {...} } 
        // group of buttons (detected by "buttons): { buttons: "..." | [], name: ..., ... }
        // list of buttons (detected by IsArray with action): [ { action: "..." | []}, { action: ""|[]} ]
        // button (detected by "command"): { command: ""|[], icon: "..", ... }
        // just a command (detected by "action"): { entityId: 17, action: "edit" }
        // array of commands: [{entityId: 17, action: "edit"}, {contentType: "blog", action: "new"}]
        buildFullDefinition: function (unstructuredConfig, allActions, instanceConfig, moreSettings) {
            var fullConfig = tools.ensureDefinitionTree(unstructuredConfig, moreSettings);

            // ToDo: don't use console.log in production
            if (unstructuredConfig.debug) console.log("toolbar: detailed debug on; start build full Def");
            tools.expandButtonGroups(fullConfig, allActions);
            tools.removeDisableButtons(fullConfig, instanceConfig);
            if (fullConfig.debug) console.log("after remove: ", fullConfig);

            tools.customize(fullConfig);

            return fullConfig;
        },

        //#region build initial toolbar object
        // this will take an input which could already be a tree, but it could also be a 
        // button-definition, or just a string, and make sure that afterwards it's a tree with groups
        // the groups could still be in compact form, or already expanded, dependending on the input
        // output is object with:
        // - groups containing buttons[], but buttons could still be very flat
        // - defaults, already officially formatted
        // - params, officially formatted 
        ensureDefinitionTree: function (original, moreSettings) {
            // original is null/undefined, just return empty set
            if (!original) throw ("preparing toolbar, with nothing to work on: " + original);

            // ensure that if it's just actions or buttons, they are then processed as arrays with 1 entry
            if (!Array.isArray(original) && (original.action || original.buttons)) original = [original];

            // ensure that arrays of actions or buttons are re-mapped to the right structure node
            if (Array.isArray(original) && original.length) {
                // an array of items having buttons, so it must be button-groups
                if (original[0].buttons)
                    original.groups = original; // move "down"

                // array of items having an action, so these are buttons
                else if (original[0].command || original[0].action)
                    original = { groups: [{ buttons: original }] };
                else
                    console.warn("toolbar tried to build toolbar but couldn't detect type of this:", original);
            }

            // build an object with this structure
            return {
                name: original.name || "toolbar", // name, no real use
                debug: original.debug || false, // show more debug info
                groups: original.groups || [], // the groups of buttons
                defaults: original.defaults || {}, // the button defaults like icon, etc.
                params: original.params || {}, // these are the default command parameters
                settings: $2sxc._lib.extend({}, tools.defaultSettings, original.settings, moreSettings)
            };
        },
        //#endregion inital toolbar object

        // this will traverse a groups-tree and expand each group
        // so if groups were just strings like "edit,new" or compact buttons, they will be expanded afterwards
        expandButtonGroups: function (fullSet, actions) { //, itemSettings) {
            // by now we should have a structure, let's check/fix the buttons
            for (var g = 0; g < fullSet.groups.length; g++) {
                // expand a verb-list like "edit,new" into objects like [{ action: "edit" }, {action: "new"}]
                tools.expandButtonList(fullSet.groups[g], fullSet.settings);

                // fix all the buttons
                var btns = fullSet.groups[g].buttons;
                if (Array.isArray(btns))
                    for (var b = 0; b < btns.length; b++) {
                        var btn = btns[b];
                        if (!(actions[btn.command.action]))
                            console.warn("warning: toolbar-button with unknown action-name:", btn.command.action);
                        $2sxc._lib.extend(btn.command, fullSet.params); // enhance the button with settings for this instance
                        // tools.addCommandParams(fullSet, btn);
                        tools.addDefaultBtnSettings(btn, fullSet.groups[g], fullSet, actions);      // ensure all buttons have either own settings, or the fallbacks
                    }
            }
        },

        // take a list of buttons (objects OR strings)
        // and convert to proper array of buttons with actions
        // on the in is a object with buttons, which are either:
        // - a string like "edit" or multi-value "layout,more"
        // - an array of such strings incl. optional complex objects which are
        expandButtonList: function (root, settings) {
            // var root = grp; // the root object which has all params of the command
            var btns = [], sharedProperties = null;

            // convert compact buttons (with multi-verb action objects) into own button-objects
            // important because an older syntax allowed {action: "new,edit", entityId: 17}
            if (Array.isArray(root.buttons)) {
                for (var b = 0; b < root.buttons.length; b++) {
                    var btn = root.buttons[b];
                    if (typeof btn.action === "string" && btn.action.indexOf(",") > -1) { // if btns. is neither array nor string, it's a short-hand with action names
                        var acts = btn.action.split(",");
                        for (var a = 0; a < acts.length; a++) {
                            btns.push($.extend(true, {}, btn, { action: acts[a] }));
                        }
                    } else
                        btns.push(btn);
                }
            } else if (typeof root.buttons === "string") {
                btns = root.buttons.split(",");
                sharedProperties = $.extend({}, root); // inherit all fields used in the button
                delete sharedProperties.buttons; // this one's not needed
                delete sharedProperties.name; // this one's not needed
                delete sharedProperties.action; //
            } else {
                btns = root.buttons;
            }

            // optionally add a more-button in each group
            if (settings.autoAddMore) {
                if (settings.autoAddMore === "right")
                    btns.push("more");
                else {
                    btns.unshift("more");
                }
            }

            // add each button - check if it's already an object or just the string
            for (var v = 0; v < btns.length; v++) {
                btns[v] = tools.expandButtonConfig(btns[v], sharedProperties);
                // todo: refactor this out, not needed any more as they are all together now
                // btns[v].group = root;// grp;    // attach group reference, needed for fallback etc.
            }
            root.buttons = btns; // ensure the internal def is also an array now
        },

        // takes an object like "actionname" or { action: "actionname", ... } and changes it to a { command: { action: "actionname" }, ... }
        expandButtonConfig: function (original, sharedProps) {
            // prevent multiple inits
            if (original._expanded || original.command)
                return original;

            // if just a name, turn into a command
            if (typeof original === "string")
                original = { action: original };

            // if it's a command w/action, wrap into command + trim
            if (typeof original.action === "string") {
                original.action = original.action.trim();
                original = { command: original };
            }
            // some clean-up
            delete original.action;  // remove the action property
            original._expanded = true;
            return original;
        },

        // remove buttons which are not valid based on add condition
        removeDisableButtons: function (full, config) {
            var btnGroups = full.groups;
            for (var g = 0; g < btnGroups.length; g++) {
                var btns = btnGroups[g].buttons;
                removeUnfitButtons(btns, config);
                disableButtons(btns, config);

                // remove the group, if no buttons left, or only "more"
                if (btns.length === 0 || (btns.length === 1 && btns[0].command.action === "more"))
                    btnGroups.splice(g--, 1);   // remove, and decrement counter
            }

            function removeUnfitButtons(btns, config) {
                for (var i = 0; i < btns.length; i++) {
                    //var add = btns[i].showCondition;
                    //if (add !== undefined)
                    //    if (typeof (add) === "function" ? !add(btns[i].command, config) : !add)
                    if (!tools.evalPropOrFunction(btns[i].showCondition, btns[i].command, config, true))
                        btns.splice(i--, 1);
                }
            }

            function disableButtons(btns, config) {
                for (var i = 0; i < btns.length; i++) 
                    btns[i].disabled = tools.evalPropOrFunction(btns[i].disabled, btns[i].command, config, false);
            }

        },

        btnProperties: [
            "classes",
            "icon",
            "title",
            "dynamicClasses",
            "showCondition",
            "disabled"
        ],
        prvProperties: [
            "defaults",
            "params",
            "name"
        ],

        // enhance button-object with default icons, etc.
        addDefaultBtnSettings: function (btn, group, groups, actions) {
            for (var d = 0; d < tools.btnProperties.length; d++)
                fallbackBtnSetting(btn, actions, tools.btnProperties[d]);

            // configure missing button properties with various fallback options
            function fallbackBtnSetting(btn, actions, propName) {
                btn[propName] = btn[propName]   // by if already defined, use the already defined propery
                    || (group.defaults && group.defaults[propName])     // if the group has defaults, try use use that property
                    || (groups && groups.defaults && groups.defaults[propName])     // if the group has defaults, try use use that property
                    || (actions[btn.command.action] && actions[btn.command.action][propName]); // if there is an action, try to use that property name
            }
        },

        customize: function (toolbar) {
            //if (!toolbar.settings) return;
            //var set = toolbar.settings;
            //if (set.autoAddMore) {
            //    console.log("auto-more");
            //    var grps = toolbar.groups;
            //    for (var g = 0; g < grps.length; g++) {
            //        var btns = grps[g];
            //        for (var i = 0; i < btns.length; i++) {
            //        }
            //    }
            //}
        },

        evalPropOrFunction: function(propOrFunction, settings, config, fallback) {
            if (propOrFunction === undefined || propOrFunction === null)
                return fallback;
            return typeof (propOrFunction) === "function" ? propOrFunction(settings, config) : propOrFunction;
        }
    };

})();
// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.
(function () {
    $2sxc._toolbarManager.standardButtons = standardButtons;
    return;
    
    function standardButtons(canDesign, sharedParameters) {
        // create a deep-copy of the original object
        var btns = $.extend(true, {}, $2sxc._toolbarManager.toolbarTemplate);
        btns.params = sharedParameters && (Array.isArray(sharedParameters) && sharedParameters[0]) || sharedParameters;
        if (!canDesign) btns.groups.splice(2, 1); // remove this menu
        return btns;
    }
})();
// the default / initial buttons in a standard toolbar
(function () {

    // ToDo: refactor to avoid side-effects
    $2sxc._toolbarManager.toolbarTemplate = {
        groups: [
            // ToDo: remove dead code
            //{
            //    name: "test",
            //    buttons: [
            //        {
            //            action: "edit",
            //            icon: "icon-sxc-code",
            //            title: "just quick edit!"
            //        },
            //        "inexisting-action",
            //        "edit",
            //        {
            //            action: "publish",
            //            showCondition: true,
            //            title: "forced publish button"
            //        },
            //        {
            //            command: {
            //                action: "custom",
            //                customCode: "alert('custom button!')"
            //            }
            //        },
            //        "more"
            //    ]
            //},
            {
                name: "default",
                buttons: "edit,new,metadata,publish,layout"
            }, {
                name: "list",
                buttons: "add,remove,moveup,movedown,instance-list,replace,item-history"
            }, {
                name: "data",
                buttons: "delete"
            }, {
                name: "instance",
                buttons: "template-develop,template-settings,contentitems,template-query,contenttype",
                defaults: {
                    classes: "group-pro"
                }
            }, {
                name: "app",
                buttons: "app,app-settings,app-resources,zone",
                defaults: {
                    classes: "group-pro"
                }
            }
        ],
        defaults: {},
        params: {},
        settings: {
            autoAddMore: "right",
            // these are defaults, don't set again
            // hover: "right",
        }
    };
})();
// initialize the translation system; ensure toolbars etc. are translated
(function () {
    var initialized = false;

    $2sxc._translateInit = function (manage) {
        if (initialized) return;
        window.i18next
            .use(window.i18nextXHRBackend)
            .init({
                lng: manage._editContext.Language.Current.substr(0,2), // "en",
                fallbackLng: "en",
                whitelist: ["en", "de", "fr", "it", "uk", "nl"],
                preload: ["en"],
                backend: {
                    loadPath: manage._editContext.Environment.SxcRootUrl + "desktopmodules/tosic_sexycontent/dist/i18n/inpage-{{lng}}.js"
                }
            }, function (err, t) {
                // for options see
                // https://github.com/i18next/jquery-i18next#initialize-the-plugin
                jqueryI18next.init(i18next, $);
                // start localizing, details:
                // https://github.com/i18next/jquery-i18next#usage-of-selector-function
                $("ul.sc-menu").localize(); // inline toolbars
                $(".sc-i18n").localize();   // quick-insert menus
            });
        initialized = true;
    };
})();

// provide an official translate API for 2sxc - currently internally using a jQuery library, but this may change
(function () {
    $2sxc.translate = function(key) {
        // return key;
        return ($.t && $.t(key)) || key;
    };
})();
!function(e,t){"object"==typeof exports&&"undefined"!=typeof module?module.exports=t():"function"==typeof define&&define.amd?define("i18next",t):e.i18next=t()}(this,function(){"use strict";function e(e){return null==e?"":""+e}function t(e,t,n){e.forEach(function(e){t[e]&&(n[e]=t[e])})}function n(e,t,n){function o(e){return e&&e.indexOf("###")>-1?e.replace(/###/g,"."):e}for(var r="string"!=typeof t?[].concat(t):t.split(".");r.length>1;){if(!e)return{};var i=o(r.shift());!e[i]&&n&&(e[i]=new n),e=e[i]}return e?{obj:e,k:o(r.shift())}:{}}function o(e,t,o){var r=n(e,t,Object),i=r.obj,s=r.k;i[s]=o}function r(e,t,o,r){var i=n(e,t,Object),s=i.obj,a=i.k;s[a]=s[a]||[],r&&(s[a]=s[a].concat(o)),r||s[a].push(o)}function i(e,t){var o=n(e,t),r=o.obj,i=o.k;return r?r[i]:void 0}function s(e,t,n){for(var o in t)o in e?"string"==typeof e[o]||e[o]instanceof String||"string"==typeof t[o]||t[o]instanceof String?n&&(e[o]=t[o]):s(e[o],t[o],n):e[o]=t[o];return e}function a(e){return e.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g,"\\$&")}function l(e){return"string"==typeof e?e.replace(/[&<>"'\/]/g,function(e){return C[e]}):e}function u(e){return e.interpolation={unescapeSuffix:"HTML"},e.interpolation.prefix=e.interpolationPrefix||"__",e.interpolation.suffix=e.interpolationSuffix||"__",e.interpolation.escapeValue=e.escapeInterpolation||!1,e.interpolation.nestingPrefix=e.reusePrefix||"$t(",e.interpolation.nestingSuffix=e.reuseSuffix||")",e}function c(e){return e.resStore&&(e.resources=e.resStore),e.ns&&e.ns.defaultNs?(e.defaultNS=e.ns.defaultNs,e.ns=e.ns.namespaces):e.defaultNS=e.ns||"translation",e.fallbackToDefaultNS&&e.defaultNS&&(e.fallbackNS=e.defaultNS),e.saveMissing=e.sendMissing,e.saveMissingTo=e.sendMissingTo||"current",e.returnNull=!e.fallbackOnNull,e.returnEmptyString=!e.fallbackOnEmpty,e.returnObjects=e.returnObjectTrees,e.joinArrays="\n",e.returnedObjectHandler=e.objectTreeKeyHandler,e.parseMissingKeyHandler=e.parseMissingKey,e.appendNamespaceToMissingKey=!0,e.nsSeparator=e.nsseparator,e.keySeparator=e.keyseparator,"sprintf"===e.shortcutFunction&&(e.overloadTranslationOptionHandler=function(e){for(var t=[],n=1;n<e.length;n++)t.push(e[n]);return{postProcess:"sprintf",sprintf:t}}),e.whitelist=e.lngWhitelist,e.preload=e.preload,"current"===e.load&&(e.load="currentOnly"),"unspecific"===e.load&&(e.load="languageOnly"),e.backend=e.backend||{},e.backend.loadPath=e.resGetPath||"locales/__lng__/__ns__.json",e.backend.addPath=e.resPostPath||"locales/add/__lng__/__ns__",e.backend.allowMultiLoading=e.dynamicLoad,e.cache=e.cache||{},e.cache.prefix="res_",e.cache.expirationTime=6048e5,e.cache.enabled=!!e.useLocalStorage,e=u(e),e.defaultVariables&&(e.interpolation.defaultVariables=e.defaultVariables),e}function p(e){return e=u(e),e.joinArrays="\n",e}function f(e){return(e.interpolationPrefix||e.interpolationSuffix||e.escapeInterpolation)&&(e=u(e)),e.nsSeparator=e.nsseparator,e.keySeparator=e.keyseparator,e.returnObjects=e.returnObjectTrees,e}function h(e){e.lng=function(){return S.deprecate("i18next.lng() can be replaced by i18next.language for detected language or i18next.languages for languages ordered by translation lookup."),e.services.languageUtils.toResolveHierarchy(e.language)[0]},e.preload=function(t,n){S.deprecate("i18next.preload() can be replaced with i18next.loadLanguages()"),e.loadLanguages(t,n)},e.setLng=function(t,n,o){return S.deprecate("i18next.setLng() can be replaced with i18next.changeLanguage() or i18next.getFixedT() to get a translation function with fixed language or namespace."),"function"==typeof n&&(o=n,n={}),n||(n={}),n.fixLng===!0&&o?o(null,e.getFixedT(t)):void e.changeLanguage(t,o)},e.addPostProcessor=function(t,n){S.deprecate("i18next.addPostProcessor() can be replaced by i18next.use({ type: 'postProcessor', name: 'name', process: fc })"),e.use({type:"postProcessor",name:t,process:n})}}function g(e){return e.charAt(0).toUpperCase()+e.slice(1)}function d(){var e={};return R.forEach(function(t){t.lngs.forEach(function(n){return e[n]={numbers:t.nr,plurals:P[t.fc]}})}),e}function v(e,t){for(var n=e.indexOf(t);-1!==n;)e.splice(n,1),n=e.indexOf(t)}function y(){return{debug:!1,ns:["translation"],defaultNS:["translation"],fallbackLng:["dev"],fallbackNS:!1,whitelist:!1,load:"all",preload:!1,keySeparator:".",nsSeparator:":",pluralSeparator:"_",contextSeparator:"_",saveMissing:!1,saveMissingTo:"fallback",missingKeyHandler:!1,postProcess:!1,returnNull:!0,returnEmptyString:!0,returnObjects:!1,joinArrays:!1,returnedObjectHandler:function(){},parseMissingKeyHandler:!1,appendNamespaceToMissingKey:!1,overloadTranslationOptionHandler:function(e){return{defaultValue:e[1]}},interpolation:{escapeValue:!0,prefix:"{{",suffix:"}}",unescapePrefix:"-",nestingPrefix:"$t(",nestingSuffix:")",defaultVariables:void 0}}}function b(e){return"string"==typeof e.ns&&(e.ns=[e.ns]),"string"==typeof e.fallbackLng&&(e.fallbackLng=[e.fallbackLng]),"string"==typeof e.fallbackNS&&(e.fallbackNS=[e.fallbackNS]),e.whitelist&&e.whitelist.indexOf("cimode")<0&&e.whitelist.push("cimode"),e}var m={};m["typeof"]="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(e){return typeof e}:function(e){return e&&"function"==typeof Symbol&&e.constructor===Symbol?"symbol":typeof e},m.classCallCheck=function(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")},m["extends"]=Object.assign||function(e){for(var t=1;t<arguments.length;t++){var n=arguments[t];for(var o in n)Object.prototype.hasOwnProperty.call(n,o)&&(e[o]=n[o])}return e},m.inherits=function(e,t){if("function"!=typeof t&&null!==t)throw new TypeError("Super expression must either be null or a function, not "+typeof t);e.prototype=Object.create(t&&t.prototype,{constructor:{value:e,enumerable:!1,writable:!0,configurable:!0}}),t&&(Object.setPrototypeOf?Object.setPrototypeOf(e,t):e.__proto__=t)},m.possibleConstructorReturn=function(e,t){if(!e)throw new ReferenceError("this hasn't been initialised - super() hasn't been called");return!t||"object"!=typeof t&&"function"!=typeof t?e:t},m.slicedToArray=function(){function e(e,t){var n=[],o=!0,r=!1,i=void 0;try{for(var s,a=e[Symbol.iterator]();!(o=(s=a.next()).done)&&(n.push(s.value),!t||n.length!==t);o=!0);}catch(l){r=!0,i=l}finally{try{!o&&a["return"]&&a["return"]()}finally{if(r)throw i}}return n}return function(t,n){if(Array.isArray(t))return t;if(Symbol.iterator in Object(t))return e(t,n);throw new TypeError("Invalid attempt to destructure non-iterable instance")}}();var x={type:"logger",log:function(e){this._output("log",e)},warn:function(e){this._output("warn",e)},error:function(e){this._output("error",e)},_output:function(e,t){console&&console[e]&&console[e].apply(console,Array.prototype.slice.call(t))}},k=function(){function e(t){var n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];m.classCallCheck(this,e),this.subs=[],this.init(t,n)}return e.prototype.init=function(e){var t=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];this.prefix=t.prefix||"i18next:",this.logger=e||x,this.options=t,this.debug=t.debug!==!1},e.prototype.setDebug=function(e){this.debug=e,this.subs.forEach(function(t){t.setDebug(e)})},e.prototype.log=function(){this.forward(arguments,"log","",!0)},e.prototype.warn=function(){this.forward(arguments,"warn","",!0)},e.prototype.error=function(){this.forward(arguments,"error","")},e.prototype.deprecate=function(){this.forward(arguments,"warn","WARNING DEPRECATED: ",!0)},e.prototype.forward=function(e,t,n,o){o&&!this.debug||("string"==typeof e[0]&&(e[0]=n+this.prefix+" "+e[0]),this.logger[t](e))},e.prototype.create=function(t){var n=new e(this.logger,m["extends"]({prefix:this.prefix+":"+t+":"},this.options));return this.subs.push(n),n},e}(),S=new k,w=function(){function e(){m.classCallCheck(this,e),this.observers={}}return e.prototype.on=function(e,t){var n=this;e.split(" ").forEach(function(e){n.observers[e]=n.observers[e]||[],n.observers[e].push(t)})},e.prototype.off=function(e,t){var n=this;this.observers[e]&&this.observers[e].forEach(function(){if(t){var o=n.observers[e].indexOf(t);o>-1&&n.observers[e].splice(o,1)}else delete n.observers[e]})},e.prototype.emit=function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),o=1;t>o;o++)n[o-1]=arguments[o];this.observers[e]&&this.observers[e].forEach(function(e){e.apply(void 0,n)}),this.observers["*"]&&this.observers["*"].forEach(function(t){var o;t.apply(t,(o=[e]).concat.apply(o,n))})},e}(),C={"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;","/":"&#x2F;"},L=function(e){function t(){var n=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],o=arguments.length<=1||void 0===arguments[1]?{ns:["translation"],defaultNS:"translation"}:arguments[1];m.classCallCheck(this,t);var r=m.possibleConstructorReturn(this,e.call(this));return r.data=n,r.options=o,r}return m.inherits(t,e),t.prototype.addNamespaces=function(e){this.options.ns.indexOf(e)<0&&this.options.ns.push(e)},t.prototype.removeNamespaces=function(e){var t=this.options.ns.indexOf(e);t>-1&&this.options.ns.splice(t,1)},t.prototype.getResource=function(e,t,n){var o=arguments.length<=3||void 0===arguments[3]?{}:arguments[3],r=o.keySeparator||this.options.keySeparator;void 0===r&&(r=".");var s=[e,t];return n&&"string"!=typeof n&&(s=s.concat(n)),n&&"string"==typeof n&&(s=s.concat(r?n.split(r):n)),e.indexOf(".")>-1&&(s=e.split(".")),i(this.data,s)},t.prototype.addResource=function(e,t,n,r){var i=arguments.length<=4||void 0===arguments[4]?{silent:!1}:arguments[4],s=this.options.keySeparator;void 0===s&&(s=".");var a=[e,t];n&&(a=a.concat(s?n.split(s):n)),e.indexOf(".")>-1&&(a=e.split("."),r=t,t=a[1]),this.addNamespaces(t),o(this.data,a,r),i.silent||this.emit("added",e,t,n,r)},t.prototype.addResources=function(e,t,n){for(var o in n)"string"==typeof n[o]&&this.addResource(e,t,o,n[o],{silent:!0});this.emit("added",e,t,n)},t.prototype.addResourceBundle=function(e,t,n,r,a){var l=[e,t];e.indexOf(".")>-1&&(l=e.split("."),r=n,n=t,t=l[1]),this.addNamespaces(t);var u=i(this.data,l)||{};r?s(u,n,a):u=m["extends"]({},u,n),o(this.data,l,u),this.emit("added",e,t,n)},t.prototype.removeResourceBundle=function(e,t){this.hasResourceBundle(e,t)&&delete this.data[e][t],this.removeNamespaces(t),this.emit("removed",e,t)},t.prototype.hasResourceBundle=function(e,t){return void 0!==this.getResource(e,t)},t.prototype.getResourceBundle=function(e,t){return t||(t=this.options.defaultNS),"v1"===this.options.compatibilityAPI?m["extends"]({},this.getResource(e,t)):this.getResource(e,t)},t.prototype.toJSON=function(){return this.data},t}(w),N={processors:{},addPostProcessor:function(e){this.processors[e.name]=e},handle:function(e,t,n,o,r){var i=this;return e.forEach(function(e){i.processors[e]&&(t=i.processors[e].process(t,n,o,r))}),t}},O=function(e){function n(o){var r=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];m.classCallCheck(this,n);var i=m.possibleConstructorReturn(this,e.call(this));return t(["resourceStore","languageUtils","pluralResolver","interpolator","backendConnector"],o,i),i.options=r,i.logger=S.create("translator"),i}return m.inherits(n,e),n.prototype.changeLanguage=function(e){e&&(this.language=e)},n.prototype.exists=function(e){var t=arguments.length<=1||void 0===arguments[1]?{interpolation:{}}:arguments[1];return"v1"===this.options.compatibilityAPI&&(t=f(t)),void 0!==this.resolve(e,t)},n.prototype.extractFromKey=function(e,t){var n=t.nsSeparator||this.options.nsSeparator;void 0===n&&(n=":");var o=t.ns||this.options.defaultNS;if(n&&e.indexOf(n)>-1){var r=e.split(n);o=r[0],e=r[1]}return"string"==typeof o&&(o=[o]),{key:e,namespaces:o}},n.prototype.translate=function(e){var t=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];if("object"!==("undefined"==typeof t?"undefined":m["typeof"](t))?t=this.options.overloadTranslationOptionHandler(arguments):"v1"===this.options.compatibilityAPI&&(t=f(t)),void 0===e||null===e||""===e)return"";"number"==typeof e&&(e=String(e)),"string"==typeof e&&(e=[e]);var n=t.lng||this.language;if(n&&"cimode"===n.toLowerCase())return e[e.length-1];var o=t.keySeparator||this.options.keySeparator||".",r=this.extractFromKey(e[e.length-1],t),i=r.key,s=r.namespaces,a=s[s.length-1],l=this.resolve(e,t),u=Object.prototype.toString.apply(l),c=["[object Number]","[object Function]","[object RegExp]"],p=void 0!==t.joinArrays?t.joinArrays:this.options.joinArrays;if(l&&"string"!=typeof l&&c.indexOf(u)<0&&(!p||"[object Array]"!==u)){if(!t.returnObjects&&!this.options.returnObjects)return this.logger.warn("accessing an object - but returnObjects options is not enabled!"),this.options.returnedObjectHandler?this.options.returnedObjectHandler(i,l,t):"key '"+i+" ("+this.language+")' returned an object instead of string.";var h="[object Array]"===u?[]:{};for(var g in l)h[g]=this.translate(""+i+o+g,m["extends"]({joinArrays:!1,ns:s},t));l=h}else if(p&&"[object Array]"===u)l=l.join(p),l&&(l=this.extendTranslation(l,i,t));else{var d=!1,v=!1;if(!this.isValidLookup(l)&&t.defaultValue&&(d=!0,l=t.defaultValue),this.isValidLookup(l)||(v=!0,l=i),(v||d)&&(this.logger.log("missingKey",n,a,i,l),this.options.saveMissing)){var y=[];if("fallback"===this.options.saveMissingTo&&this.options.fallbackLng&&this.options.fallbackLng[0])for(var b=0;b<this.options.fallbackLng.length;b++)y.push(this.options.fallbackLng[b]);else"all"===this.options.saveMissingTo?y=this.languageUtils.toResolveHierarchy(t.lng||this.language):y.push(t.lng||this.language);this.options.missingKeyHandler?this.options.missingKeyHandler(y,a,i,l):this.backendConnector&&this.backendConnector.saveMissing&&this.backendConnector.saveMissing(y,a,i,l),this.emit("missingKey",y,a,i,l)}l=this.extendTranslation(l,i,t),v&&l===i&&this.options.appendNamespaceToMissingKey&&(l=a+":"+i),v&&this.options.parseMissingKeyHandler&&(l=this.options.parseMissingKeyHandler(l))}return l},n.prototype.extendTranslation=function(e,t,n){var o=this;n.interpolation&&this.interpolator.init(n);var r=n.replace&&"string"!=typeof n.replace?n.replace:n;this.options.interpolation.defaultVariables&&(r=m["extends"]({},this.options.interpolation.defaultVariables,r)),e=this.interpolator.interpolate(e,r),e=this.interpolator.nest(e,function(){for(var e=arguments.length,t=Array(e),n=0;e>n;n++)t[n]=arguments[n];return o.translate.apply(o,t)},n),n.interpolation&&this.interpolator.reset();var i=n.postProcess||this.options.postProcess,s="string"==typeof i?[i]:i;return void 0!==e&&s&&s.length&&n.applyPostProcessor!==!1&&(e=N.handle(s,e,t,n,this)),e},n.prototype.resolve=function(e){var t=this,n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1],o=void 0;return"string"==typeof e&&(e=[e]),e.forEach(function(e){if(!t.isValidLookup(o)){var r=t.extractFromKey(e,n),i=r.key,s=r.namespaces;t.options.fallbackNS&&(s=s.concat(t.options.fallbackNS));var a=void 0!==n.count&&"string"!=typeof n.count,l=void 0!==n.context&&"string"==typeof n.context&&""!==n.context,u=n.lngs?n.lngs:t.languageUtils.toResolveHierarchy(n.lng||t.language);s.forEach(function(e){t.isValidLookup(o)||u.forEach(function(r){if(!t.isValidLookup(o)){var s=i,u=[s],c=void 0;a&&(c=t.pluralResolver.getSuffix(r,n.count)),a&&l&&u.push(s+c),l&&u.push(s+=""+t.options.contextSeparator+n.context),a&&u.push(s+=c);for(var p=void 0;p=u.pop();)t.isValidLookup(o)||(o=t.getResource(r,e,p,n))}})})}}),o},n.prototype.isValidLookup=function(e){return!(void 0===e||!this.options.returnNull&&null===e||!this.options.returnEmptyString&&""===e)},n.prototype.getResource=function(e,t,n){var o=arguments.length<=3||void 0===arguments[3]?{}:arguments[3];return this.resourceStore.getResource(e,t,n,o)},n}(w),j=function(){function e(t){m.classCallCheck(this,e),this.options=t,this.whitelist=this.options.whitelist||!1,this.logger=S.create("languageUtils")}return e.prototype.getLanguagePartFromCode=function(e){if(e.indexOf("-")<0)return e;var t=["NB-NO","NN-NO","nb-NO","nn-NO","nb-no","nn-no"],n=e.split("-");return this.formatLanguageCode(t.indexOf(e)>-1?n[1].toLowerCase():n[0])},e.prototype.formatLanguageCode=function(e){if("string"==typeof e&&e.indexOf("-")>-1){var t=["hans","hant","latn","cyrl","cans","mong","arab"],n=e.split("-");return this.options.lowerCaseLng?n=n.map(function(e){return e.toLowerCase()}):2===n.length?(n[0]=n[0].toLowerCase(),n[1]=n[1].toUpperCase(),t.indexOf(n[1].toLowerCase())>-1&&(n[1]=g(n[1].toLowerCase()))):3===n.length&&(n[0]=n[0].toLowerCase(),2===n[1].length&&(n[1]=n[1].toUpperCase()),"sgn"!==n[0]&&2===n[2].length&&(n[2]=n[2].toUpperCase()),t.indexOf(n[1].toLowerCase())>-1&&(n[1]=g(n[1].toLowerCase())),t.indexOf(n[2].toLowerCase())>-1&&(n[2]=g(n[2].toLowerCase()))),n.join("-")}return this.options.cleanCode||this.options.lowerCaseLng?e.toLowerCase():e},e.prototype.isWhitelisted=function(e){return"languageOnly"===this.options.load&&(e=this.getLanguagePartFromCode(e)),!this.whitelist||!this.whitelist.length||this.whitelist.indexOf(e)>-1},e.prototype.toResolveHierarchy=function(e,t){var n=this;t=t||this.options.fallbackLng||[],"string"==typeof t&&(t=[t]);var o=[],r=function(e){n.isWhitelisted(e)?o.push(e):n.logger.warn("rejecting non-whitelisted language code: "+e)};return"string"==typeof e&&e.indexOf("-")>-1?("languageOnly"!==this.options.load&&r(this.formatLanguageCode(e)),"currentOnly"!==this.options.load&&r(this.getLanguagePartFromCode(e))):"string"==typeof e&&r(this.formatLanguageCode(e)),t.forEach(function(e){o.indexOf(e)<0&&r(n.formatLanguageCode(e))}),o},e}(),R=[{lngs:["ach","ak","am","arn","br","fil","gun","ln","mfe","mg","mi","oc","tg","ti","tr","uz","wa"],nr:[1,2],fc:1},{lngs:["af","an","ast","az","bg","bn","ca","da","de","dev","el","en","eo","es","es_ar","et","eu","fi","fo","fur","fy","gl","gu","ha","he","hi","hu","hy","ia","it","kn","ku","lb","mai","ml","mn","mr","nah","nap","nb","ne","nl","nn","no","nso","pa","pap","pms","ps","pt","pt_br","rm","sco","se","si","so","son","sq","sv","sw","ta","te","tk","ur","yo"],nr:[1,2],fc:2},{lngs:["ay","bo","cgg","fa","id","ja","jbo","ka","kk","km","ko","ky","lo","ms","sah","su","th","tt","ug","vi","wo","zh"],nr:[1],fc:3},{lngs:["be","bs","dz","hr","ru","sr","uk"],nr:[1,2,5],fc:4},{lngs:["ar"],nr:[0,1,2,3,11,100],fc:5},{lngs:["cs","sk"],nr:[1,2,5],fc:6},{lngs:["csb","pl"],nr:[1,2,5],fc:7},{lngs:["cy"],nr:[1,2,3,8],fc:8},{lngs:["fr"],nr:[1,2],fc:9},{lngs:["ga"],nr:[1,2,3,7,11],fc:10},{lngs:["gd"],nr:[1,2,3,20],fc:11},{lngs:["is"],nr:[1,2],fc:12},{lngs:["jv"],nr:[0,1],fc:13},{lngs:["kw"],nr:[1,2,3,4],fc:14},{lngs:["lt"],nr:[1,2,10],fc:15},{lngs:["lv"],nr:[1,2,0],fc:16},{lngs:["mk"],nr:[1,2],fc:17},{lngs:["mnk"],nr:[0,1,2],fc:18},{lngs:["mt"],nr:[1,2,11,20],fc:19},{lngs:["or"],nr:[2,1],fc:2},{lngs:["ro"],nr:[1,2,20],fc:20},{lngs:["sl"],nr:[5,1,2,3],fc:21}],P={1:function(e){return Number(e>1)},2:function(e){return Number(1!=e)},3:function(e){return 0},4:function(e){return Number(e%10==1&&e%100!=11?0:e%10>=2&&4>=e%10&&(10>e%100||e%100>=20)?1:2)},5:function(e){return Number(0===e?0:1==e?1:2==e?2:e%100>=3&&10>=e%100?3:e%100>=11?4:5)},6:function(e){return Number(1==e?0:e>=2&&4>=e?1:2)},7:function(e){return Number(1==e?0:e%10>=2&&4>=e%10&&(10>e%100||e%100>=20)?1:2)},8:function(e){return Number(1==e?0:2==e?1:8!=e&&11!=e?2:3)},9:function(e){return Number(e>=2)},10:function(e){return Number(1==e?0:2==e?1:7>e?2:11>e?3:4)},11:function(e){return Number(1==e||11==e?0:2==e||12==e?1:e>2&&20>e?2:3)},12:function(e){return Number(e%10!=1||e%100==11)},13:function(e){return Number(0!==e)},14:function(e){return Number(1==e?0:2==e?1:3==e?2:3)},15:function(e){return Number(e%10==1&&e%100!=11?0:e%10>=2&&(10>e%100||e%100>=20)?1:2)},16:function(e){return Number(e%10==1&&e%100!=11?0:0!==e?1:2)},17:function(e){return Number(1==e||e%10==1?0:1)},18:function(e){return Number(0==e?0:1==e?1:2)},19:function(e){return Number(1==e?0:0===e||e%100>1&&11>e%100?1:e%100>10&&20>e%100?2:3)},20:function(e){return Number(1==e?0:0===e||e%100>0&&20>e%100?1:2)},21:function(e){return Number(e%100==1?1:e%100==2?2:e%100==3||e%100==4?3:0)}},E=function(){function e(t){var n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];m.classCallCheck(this,e),this.languageUtils=t,this.options=n,this.logger=S.create("pluralResolver"),this.rules=d()}return e.prototype.addRule=function(e,t){this.rules[e]=t},e.prototype.getRule=function(e){return this.rules[this.languageUtils.getLanguagePartFromCode(e)]},e.prototype.needsPlural=function(e){var t=this.getRule(e);return!(t&&t.numbers.length<=1)},e.prototype.getSuffix=function(e,t){var n=this.getRule(e);if(n){if(1===n.numbers.length)return"";var o=n.noAbs?n.plurals(t):n.plurals(Math.abs(t)),r=n.numbers[o];if(2===n.numbers.length&&1===n.numbers[0]&&(2===r?r="plural":1===r&&(r="")),"v1"===this.options.compatibilityJSON){if(1===r)return"";if("number"==typeof r)return"_plural_"+r.toString()}return this.options.prepend&&r.toString()?this.options.prepend+r.toString():r.toString()}return this.logger.warn("no plural rule found for: "+e),""},e}(),_=function(){function t(){var e=arguments.length<=0||void 0===arguments[0]?{}:arguments[0];m.classCallCheck(this,t),this.logger=S.create("interpolator"),this.init(e,!0)}return t.prototype.init=function(){var e=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],t=arguments[1];t&&(this.options=e),e.interpolation||(e.interpolation={escapeValue:!0});var n=e.interpolation;this.escapeValue=n.escapeValue,this.prefix=n.prefix?a(n.prefix):n.prefixEscaped||"{{",this.suffix=n.suffix?a(n.suffix):n.suffixEscaped||"}}",this.unescapePrefix=n.unescapeSuffix?"":n.unescapePrefix||"-",this.unescapeSuffix=this.unescapePrefix?"":n.unescapeSuffix||"",this.nestingPrefix=n.nestingPrefix?a(n.nestingPrefix):n.nestingPrefixEscaped||a("$t("),this.nestingSuffix=n.nestingSuffix?a(n.nestingSuffix):n.nestingSuffixEscaped||a(")");var o=this.prefix+"(.+?)"+this.suffix;this.regexp=new RegExp(o,"g");var r=this.prefix+this.unescapePrefix+"(.+?)"+this.unescapeSuffix+this.suffix;this.regexpUnescape=new RegExp(r,"g");var i=this.nestingPrefix+"(.+?)"+this.nestingSuffix;this.nestingRegexp=new RegExp(i,"g")},t.prototype.reset=function(){this.options&&this.init(this.options)},t.prototype.interpolate=function(t,n){function o(e){return e.replace(/\$/g,"$$$$")}for(var r=void 0,s=void 0;r=this.regexpUnescape.exec(t);){var a=i(n,r[1].trim());t=t.replace(r[0],a)}for(;r=this.regexp.exec(t);)s=i(n,r[1].trim()),"string"!=typeof s&&(s=e(s)),s||(this.logger.warn("missed to pass in variable "+r[1]+" for interpolating "+t),s=""),s=o(this.escapeValue?l(s):s),t=t.replace(r[0],s),this.regexp.lastIndex=0;return t},t.prototype.nest=function(t,n){function o(e){return e.replace(/\$/g,"$$$$")}function r(e){if(e.indexOf(",")<0)return e;var t=e.split(",");e=t.shift();var n=t.join(",");n=this.interpolate(n,u);try{u=JSON.parse(n)}catch(o){this.logger.error("failed parsing options string in nesting for key "+e,o)}return e}var i=arguments.length<=2||void 0===arguments[2]?{}:arguments[2],s=void 0,a=void 0,u=JSON.parse(JSON.stringify(i));for(u.applyPostProcessor=!1;s=this.nestingRegexp.exec(t);)a=n(r.call(this,s[1].trim()),u),"string"!=typeof a&&(a=e(a)),a||(this.logger.warn("missed to pass in variable "+s[1]+" for interpolating "+t),a=""),a=o(this.escapeValue?l(a):a),t=t.replace(s[0],a),this.regexp.lastIndex=0;return t},t}(),T=function(e){function t(n,o,r){var i=arguments.length<=3||void 0===arguments[3]?{}:arguments[3];m.classCallCheck(this,t);var s=m.possibleConstructorReturn(this,e.call(this));return s.backend=n,s.store=o,s.services=r,s.options=i,s.logger=S.create("backendConnector"),s.state={},s.queue=[],s.backend&&s.backend.init&&s.backend.init(r,i.backend,i),s}return m.inherits(t,e),t.prototype.queueLoad=function(e,t,n){var o=this,r=[],i=[],s=[],a=[];return e.forEach(function(e){var n=!0;t.forEach(function(t){var s=e+"|"+t;o.store.hasResourceBundle(e,t)?o.state[s]=2:o.state[s]<0||(1===o.state[s]?i.indexOf(s)<0&&i.push(s):(o.state[s]=1,n=!1,i.indexOf(s)<0&&i.push(s),r.indexOf(s)<0&&r.push(s),a.indexOf(t)<0&&a.push(t)))}),n||s.push(e)}),(r.length||i.length)&&this.queue.push({pending:i,loaded:{},errors:[],callback:n}),{toLoad:r,pending:i,toLoadLanguages:s,toLoadNamespaces:a}},t.prototype.loaded=function(e,t,n){var o=this,i=e.split("|"),s=m.slicedToArray(i,2),a=s[0],l=s[1];t&&this.emit("failedLoading",a,l,t),n&&this.store.addResourceBundle(a,l,n),this.state[e]=t?-1:2,this.queue.forEach(function(n){r(n.loaded,[a],l),v(n.pending,e),t&&n.errors.push(t),0!==n.pending.length||n.done||(n.errors.length?n.callback(n.errors):n.callback(),o.emit("loaded",n.loaded),n.done=!0)}),this.queue=this.queue.filter(function(e){return!e.done})},t.prototype.read=function(e,t,n,o,r,i){var s=this;return o||(o=0),r||(r=250),e.length?void this.backend[n](e,t,function(a,l){return a&&l&&5>o?void setTimeout(function(){s.read.call(s,e,t,n,++o,2*r,i)},r):void i(a,l)}):i(null,{})},t.prototype.load=function(e,t,n){var o=this;if(!this.backend)return this.logger.warn("No backend was added via i18next.use. Will not load resources."),n&&n();var r=m["extends"]({},this.backend.options,this.options.backend);"string"==typeof e&&(e=this.services.languageUtils.toResolveHierarchy(e)),"string"==typeof t&&(t=[t]);var s=this.queueLoad(e,t,n);return s.toLoad.length?void(r.allowMultiLoading&&this.backend.readMulti?this.read(s.toLoadLanguages,s.toLoadNamespaces,"readMulti",null,null,function(e,t){e&&o.logger.warn("loading namespaces "+s.toLoadNamespaces.join(", ")+" for languages "+s.toLoadLanguages.join(", ")+" via multiloading failed",e),!e&&t&&o.logger.log("loaded namespaces "+s.toLoadNamespaces.join(", ")+" for languages "+s.toLoadLanguages.join(", ")+" via multiloading",t),s.toLoad.forEach(function(n){var r=n.split("|"),s=m.slicedToArray(r,2),a=s[0],l=s[1],u=i(t,[a,l]);if(u)o.loaded(n,e,u);else{var c="loading namespace "+l+" for language "+a+" via multiloading failed";o.loaded(n,c),o.logger.error(c)}})}):!function(){var e=function(e){var t=this,n=e.split("|"),o=m.slicedToArray(n,2),r=o[0],i=o[1];this.read(r,i,"read",null,null,function(n,o){n&&t.logger.warn("loading namespace "+i+" for language "+r+" failed",n),!n&&o&&t.logger.log("loaded namespace "+i+" for language "+r,o),t.loaded(e,n,o)})};s.toLoad.forEach(function(t){e.call(o,t)})}()):void(s.pending.length||n())},t.prototype.saveMissing=function(e,t,n,o){this.backend&&this.backend.create&&this.backend.create(e,t,n,o),this.store.addResource(e[0],t,n,o)},t}(w),A=function(e){function t(n,o,r){var i=arguments.length<=3||void 0===arguments[3]?{}:arguments[3];m.classCallCheck(this,t);var s=m.possibleConstructorReturn(this,e.call(this));return s.cache=n,s.store=o,s.services=r,s.options=i,s.logger=S.create("cacheConnector"),s.cache&&s.cache.init&&s.cache.init(r,i.cache,i),s}return m.inherits(t,e),t.prototype.load=function(e,t,n){var o=this;if(!this.cache)return n&&n();var r=m["extends"]({},this.cache.options,this.options.cache);"string"==typeof e&&(e=this.services.languageUtils.toResolveHierarchy(e)),"string"==typeof t&&(t=[t]),r.enabled?this.cache.load(e,function(t,r){if(t&&o.logger.error("loading languages "+e.join(", ")+" from cache failed",t),r)for(var i in r)for(var s in r[i])if("i18nStamp"!==s){var a=r[i][s];a&&o.store.addResourceBundle(i,s,a)}n&&n()}):n&&n()},t.prototype.save=function(){this.cache&&this.options.cache&&this.options.cache.enabled&&this.cache.save(this.store.data)},t}(w),M=function(e){function t(){var n=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],o=arguments[1];m.classCallCheck(this,t);var r=m.possibleConstructorReturn(this,e.call(this));return r.options=b(n),r.services={},r.logger=S,r.modules={},o&&!r.isInitialized&&r.init(n,o),r}return m.inherits(t,e),t.prototype.init=function(e,t){function n(e){return e?"function"==typeof e?new e:e:void 0}var o=this;if("function"==typeof e&&(t=e,e={}),e||(e={}),"v1"===e.compatibilityAPI?this.options=m["extends"]({},y(),b(c(e)),{}):"v1"===e.compatibilityJSON?this.options=m["extends"]({},y(),b(p(e)),{}):this.options=m["extends"]({},y(),this.options,b(e)),t||(t=function(){}),!this.options.isClone){this.modules.logger?S.init(n(this.modules.logger),this.options):S.init(null,this.options);var r=new j(this.options);this.store=new L(this.options.resources,this.options);var i=this.services;i.logger=S,i.resourceStore=this.store,i.resourceStore.on("added removed",function(e,t){i.cacheConnector.save()}),i.languageUtils=r,i.pluralResolver=new E(r,{prepend:this.options.pluralSeparator,compatibilityJSON:this.options.compatibilityJSON}),i.interpolator=new _(this.options),i.backendConnector=new T(n(this.modules.backend),i.resourceStore,i,this.options),i.backendConnector.on("*",function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),r=1;t>r;r++)n[r-1]=arguments[r];o.emit.apply(o,[e].concat(n))}),i.backendConnector.on("loaded",function(e){i.cacheConnector.save()}),i.cacheConnector=new A(n(this.modules.cache),i.resourceStore,i,this.options),i.cacheConnector.on("*",function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),r=1;t>r;r++)n[r-1]=arguments[r];o.emit.apply(o,[e].concat(n))}),this.modules.languageDetector&&(i.languageDetector=n(this.modules.languageDetector),i.languageDetector.init(i,this.options.detection,this.options)),this.translator=new O(this.services,this.options),this.translator.on("*",function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),r=1;t>r;r++)n[r-1]=arguments[r];o.emit.apply(o,[e].concat(n))})}var s=["getResource","addResource","addResources","addResourceBundle","removeResourceBundle","hasResourceBundle","getResourceBundle"];s.forEach(function(e){o[e]=function(){return this.store[e].apply(this.store,arguments)}}),"v1"===this.options.compatibilityAPI&&h(this);var a=function(){o.changeLanguage(o.options.lng,function(e,n){o.emit("initialized",o.options),o.logger.log("initialized",o.options),t(e,n)})};return this.options.resources?a():setTimeout(a,10),this},t.prototype.loadResources=function(e){var t=this;if(e||(e=function(){}),this.options.resources)e(null);else{var n=function(){if(t.language&&"cimode"===t.language.toLowerCase())return{v:e()};var n=[],o=function(e){var o=t.services.languageUtils.toResolveHierarchy(e);o.forEach(function(e){n.indexOf(e)<0&&n.push(e)})};o(t.language),t.options.preload&&t.options.preload.forEach(function(e){o(e)}),t.services.cacheConnector.load(n,t.options.ns,function(){t.services.backendConnector.load(n,t.options.ns,e)})}();if("object"===("undefined"==typeof n?"undefined":m["typeof"](n)))return n.v}},t.prototype.use=function(e){return"backend"===e.type&&(this.modules.backend=e),"cache"===e.type&&(this.modules.cache=e),("logger"===e.type||e.log&&e.warn&&e.warn)&&(this.modules.logger=e),"languageDetector"===e.type&&(this.modules.languageDetector=e),"postProcessor"===e.type&&N.addPostProcessor(e),this},t.prototype.changeLanguage=function(e,t){var n=this,o=function(o){e&&(n.emit("languageChanged",e),n.logger.log("languageChanged",e)),t&&t(o,function(){for(var e=arguments.length,t=Array(e),o=0;e>o;o++)t[o]=arguments[o];return n.t.apply(n,t)})};!e&&this.services.languageDetector&&(e=this.services.languageDetector.detect()),e&&(this.language=e,this.languages=this.services.languageUtils.toResolveHierarchy(e),this.translator.changeLanguage(e),this.services.languageDetector&&this.services.languageDetector.cacheUserLanguage(e)),this.loadResources(function(e){o(e)})},t.prototype.getFixedT=function(e,t){var n=this,o=function r(e,t){return t=t||{},t.lng=t.lng||r.lng,t.ns=t.ns||r.ns,n.t(e,t)};return o.lng=e,o.ns=t,o},t.prototype.t=function(){return this.translator&&this.translator.translate.apply(this.translator,arguments)},t.prototype.exists=function(){return this.translator&&this.translator.exists.apply(this.translator,arguments)},t.prototype.setDefaultNamespace=function(e){this.options.defaultNS=e},t.prototype.loadNamespaces=function(e,t){var n=this;return this.options.ns?("string"==typeof e&&(e=[e]),e.forEach(function(e){n.options.ns.indexOf(e)<0&&n.options.ns.push(e)}),void this.loadResources(t)):t&&t()},t.prototype.loadLanguages=function(e,t){"string"==typeof e&&(e=[e]);var n=this.options.preload||[],o=e.filter(function(e){return n.indexOf(e)<0});return o.length?(this.options.preload=n.concat(o),
void this.loadResources(t)):t()},t.prototype.dir=function(e){e||(e=this.language);var t=["ar","shu","sqr","ssh","xaa","yhd","yud","aao","abh","abv","acm","acq","acw","acx","acy","adf","ads","aeb","aec","afb","ajp","apc","apd","arb","arq","ars","ary","arz","auz","avl","ayh","ayl","ayn","ayp","bbz","pga","he","iw","ps","pbt","pbu","pst","prp","prd","ur","ydd","yds","yih","ji","yi","hbo","men","xmn","fa","jpr","peo","pes","prs","dv","sam"];return t.indexOf(this.services.languageUtils.getLanguagePartFromCode(e))?"ltr":"rtl"},t.prototype.createInstance=function(){var e=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],n=arguments[1];return new t(e,n)},t.prototype.cloneInstance=function(){var e=this,n=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],o=arguments[1],r=new t(m["extends"]({},n,this.options,{isClone:!0}),o),i=["store","translator","services","language"];return i.forEach(function(t){r[t]=e[t]}),r},t}(w),H=new M;return H});
!function(e,t){"object"==typeof exports&&"undefined"!=typeof module?module.exports=t():"function"==typeof define&&define.amd?define("i18nextXHRBackend",t):e.i18nextXHRBackend=t()}(this,function(){"use strict";function e(e){return a.call(r.call(arguments,1),function(t){if(t)for(var n in t)void 0===e[n]&&(e[n]=t[n])}),e}function t(e,t,n,i,a){if(i&&"object"===("undefined"==typeof i?"undefined":o["typeof"](i))){var r="",s=encodeURIComponent;for(var l in i)r+="&"+s(l)+"="+s(i[l]);i=r.slice(1)+(a?"":"&_t="+new Date)}try{var c=new(XMLHttpRequest||ActiveXObject)("MSXML2.XMLHTTP.3.0");c.open(i?"POST":"GET",e,1),t.crossDomain||c.setRequestHeader("X-Requested-With","XMLHttpRequest"),c.setRequestHeader("Content-type","application/x-www-form-urlencoded"),c.onreadystatechange=function(){c.readyState>3&&n&&n(c.responseText,c)},c.send(i)}catch(s){window.console&&console.log(s)}}function n(){return{loadPath:"/locales/{{lng}}/{{ns}}.json",addPath:"locales/add/{{lng}}/{{ns}}",allowMultiLoading:!1,parse:JSON.parse,crossDomain:!1,ajax:t}}var o={};o["typeof"]="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(e){return typeof e}:function(e){return e&&"function"==typeof Symbol&&e.constructor===Symbol?"symbol":typeof e},o.classCallCheck=function(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")},o.createClass=function(){function e(e,t){for(var n=0;n<t.length;n++){var o=t[n];o.enumerable=o.enumerable||!1,o.configurable=!0,"value"in o&&(o.writable=!0),Object.defineProperty(e,o.key,o)}}return function(t,n,o){return n&&e(t.prototype,n),o&&e(t,o),t}}();var i=[],a=i.forEach,r=i.slice,s=function(){function t(e){var n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];o.classCallCheck(this,t),this.init(e,n),this.type="backend"}return o.createClass(t,[{key:"init",value:function(t){var o=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];this.services=t,this.options=e(o,this.options||{},n())}},{key:"readMulti",value:function(e,t,n){var o=this.services.interpolator.interpolate(this.options.loadPath,{lng:e.join("+"),ns:t.join("+")});this.loadUrl(o,n)}},{key:"read",value:function(e,t,n){var o=this.services.interpolator.interpolate(this.options.loadPath,{lng:e,ns:t});this.loadUrl(o,n)}},{key:"loadUrl",value:function(e,t){var n=this;this.options.ajax(e,this.options,function(o,i){var a=i.status.toString();if(0===a.indexOf("5"))return t("failed loading "+e,!0);if(0===a.indexOf("4"))return t("failed loading "+e,!1);var r=void 0,s=void 0;try{r=n.options.parse(o)}catch(l){s="failed parsing "+e+" to json"}return s?t(s,!1):void t(null,r)})}},{key:"create",value:function(e,t,n,o){var i=this;"string"==typeof e&&(e=[e]);var a={};a[n]=o||"",e.forEach(function(e){var n=i.services.interpolator.interpolate(i.options.addPath,{lng:e,ns:t});i.options.ajax(n,i.options,function(e,t){},a)})}}]),t}();return s.type="backend",s});
!function(t,e){"object"==typeof exports&&"undefined"!=typeof module?module.exports=e():"function"==typeof define&&define.amd?define("jqueryI18next",e):t.jqueryI18next=e()}(this,function(){"use strict";function t(t,a){function r(n,a,r){function i(t,n){return s.parseDefaultValueFromContent?e["extends"]({},t,{defaultValue:n}):t}if(0!==a.length){var o="text";if(0===a.indexOf("[")){var f=a.split("]");a=f[1],o=f[0].substr(1,f[0].length-1)}if(a.indexOf(";")===a.length-1&&(a=a.substr(0,a.length-2)),"html"===o)n.html(t.t(a,i(r,n.html())));else if("text"===o)n.text(t.t(a,i(r,n.text())));else if("prepend"===o)n.prepend(t.t(a,i(r,n.html())));else if("append"===o)n.append(t.t(a,i(r,n.html())));else if(0===o.indexOf("data-")){var l=o.substr("data-".length),d=t.t(a,i(r,n.data(l)));n.data(l,d),n.attr(o,d)}else n.attr(o,t.t(a,i(r,n.attr(o))))}}function i(t,n){var i=t.attr(s.selectorAttr);if(i||"undefined"==typeof i||i===!1||(i=t.text()||t.val()),i){var o=t,f=t.data(s.targetAttr);if(f&&(o=t.find(f)||t),n||s.useOptionsAttr!==!0||(n=t.data(s.optionsAttr)),n=n||{},i.indexOf(";")>=0){var l=i.split(";");a.each(l,function(t,e){""!==e&&r(o,e,n)})}else r(o,i,n);if(s.useOptionsAttr===!0){var d={};d=e["extends"]({clone:d},n),delete d.lng,t.data(s.optionsAttr,d)}}}function o(t){return this.each(function(){i(a(this),t);var e=a(this).find("["+s.selectorAttr+"]");e.each(function(){i(a(this),t)})})}var s=arguments.length<=2||void 0===arguments[2]?{}:arguments[2];s=e["extends"]({},n,s),a[s.tName]=t.t.bind(t),a[s.i18nName]=t,a.fn[s.handleName]=o}var e={};e["extends"]=Object.assign||function(t){for(var e=1;e<arguments.length;e++){var n=arguments[e];for(var a in n)Object.prototype.hasOwnProperty.call(n,a)&&(t[a]=n[a])}return t};var n={tName:"t",i18nName:"i18n",handleName:"localize",selectorAttr:"data-i18n",targetAttr:"i18n-target",optionsAttr:"i18n-options",useOptionsAttr:!1,parseDefaultValueFromContent:!0},a={init:t};return a});
// module & toolbar bootstrapping (initialize all toolbars after loading page)
// this will run onReady...
$(function () {
    var initializedModules = [];
    var openedTemplatePickerOnce = false;
    var cancelledDialog = localStorage.getItem('cancelled-dialog');

    if (cancelledDialog) localStorage.removeItem('cancelled-dialog');

    initAllModules(true);

    // watch for ajax reloads on edit or view-changes, to re-init the toolbars etc.
    document.body.addEventListener("DOMSubtreeModified", function (event) {
        initAllModules(false);
    }, false);

    return; // avoid side-effects

    function initAllModules(isFirstRun) {
        $("div[data-edit-context]").each(function () {
            initModule(this, isFirstRun);
        });
        tryShowTemplatePicker();
    }

    /**
     * Show the template picker if
     * - template picker has not yet been opened
     * - dialog has not been cancelled
     * - only one uninitialized module on page
     * @returns
     */
    function tryShowTemplatePicker() {
        var uninitializedModules = $('.sc-uninitialized');

        if (cancelledDialog || openedTemplatePickerOnce) return false;

        // already showing a dialog
        if ($2sxc._quickDialog.current !== null) return false;

        // not exactly one uninitialized module
        if (uninitializedModules.length !== 1) return false;

        // show the template picker of this module
        var module = uninitializedModules.parent('div[data-edit-context]')[0];
        $2sxc(module).manage.run('layout');
        openedTemplatePickerOnce = true;
    }

    function initModule(module, isFirstRun) {

        // check if module is already in the list of initialized modules
        if (initializedModules.find(function (m) {
                return m === module;
            })) return false;
        
        // add to modules-list
        initializedModules.push(module);
        
        var sxc = $2sxc(module);
        
        // check if the sxc must be re-created. This is necessary when modules are dynamically changed
        // because the configuration may change, and that is cached otherwise, resulting in toolbars with wrong config
        if (!isFirstRun) sxc = sxc.recreate(true);
        
        // check if we must show the glasses
        // this must run even after first-run, because it can be added ajax-style
        var wasEmpty = showGlassesButtonIfUninitialized(sxc);
        
        if (isFirstRun || !wasEmpty) $2sxc._toolbarManager.buildToolbars(module);

        return true;
    }

    function showGlassesButtonIfUninitialized(sxc) {

        // already initialized
        if (sxc.manage._editContext.ContentGroup.TemplateId !== 0) return false;

        // already has a glasses button
        var tag = $($2sxc._manage.getTag(sxc));
        if (tag.find(".sc-uninitialized").length !== 0) return false;

        // note: title is added on mouseover, as the translation isn't ready at page-load
        var btn = $('<div class="sc-uninitialized" onmouseover="this.title = $2sxc.translate(this.title)" title="InPage.NewElement"><div class="icon-sxc-glasses"></div></div>');
        btn.on("click", function () {
            sxc.manage.run("layout");
        });

        tag.append(btn);
        return true;
    }
});