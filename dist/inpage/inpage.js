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
                code: function (settings, event, tbContr) {
                    tbContr.contentBlock 
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
                code: function (settings, event, tbContr) {
                    if (confirm($2sxc.translate("Toolbar.ConfirmRemove"))) {
                        tbContr.contentBlock
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
                code: function (settings, event, tbContr) {
                    tbContr.contentBlock
                        .changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
                }
            },
            'movedown': {
                title: "Toolbar.MoveDown",
                iclass: "icon-sxc-move-down",
                disabled: false,
                showOn: "edit",
                addCondition: function (settings, modConfig) { return modConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
                code: function (settings, event, tbContr) {
                    tbContr.contentBlock
                        .changeOrder(settings.sortOrder, settings.sortOrder + 1);
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
                code: function (settings, event, tbContr) {
                    if (settings.isPublished) {
                        alert($2sxc.translate("Toolbar.AlreadyPublished"));
                        return;
                    }
                    var part = settings.sortOrder === -1 ? "listcontent" : "content";
                    var index = settings.sortOrder === -1 ? 0 : settings.sortOrder;
                    tbContr.contentBlock
                        .publish(part, index);
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
                    //if (!manager.dialog)
                    //    manager.dialog = manager.action({ "action": "dash-view" });
                    //else
                    //    manager.dialog.toggle();
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
                        Group: { Guid: guid, Index: index, Part: part, Add: isAdd, ContentBlockIsEntity: isEntity, ContentBlockId: cbid },
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
                cmc.editManager.contentBlock.reload();
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
                var conf = allActions[btnSettings.action],
                    showClasses = "",
                    classesList = conf.showOn.split(","),
                    box = $("<div/>"),
                    symbol = $("<i class=\"" + conf.iclass + "\" aria-hidden=\"true\"></i>");

                for (var c = 0; c < classesList.length; c++)
                    showClasses += " show-" + classesList[c];

                var button = $("<a />", {
                    'class': "sc-" + btnSettings.action + " " + showClasses + (conf.dynamicClasses ? " " + conf.dynamicClasses(btnSettings) : ""),
                    'onclick': "javascript:$2sxc(" + id + ", " + cbid + ").manage.action(" + JSON.stringify(btnSettings) + ", event);",
                    'title': $2sxc.translate(conf.title)
                });

                // todo: move the following lines into the button-config and just call from here
                // if publish-button and not published yet, show button (otherwise hidden) & change icon
                if (btnSettings.action === "publish" && btnSettings.isPublished === false) {
                    button.addClass("show-default").removeClass("show-edit")
                        .attr("title", $2sxc.translate("Toolbar.Unpublished"));
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
                var buttons = settings.action
                    ? [settings] // if single item with specified action, use this as our button-list
                    : $.isArray(settings)
                    ? settings // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                    : tb.createDefaultToolbar(settings);

                var tbClasses = "sc-menu show-set-0" + ((settings.sortOrder === -1) ? " listContent" : "");
                var toolbar = $("<ul />", { 'class': tbClasses, 'onclick': "javascript: var e = arguments[0] || window.event; e.stopPropagation();" });

                for (var i = 0; i < buttons.length; i++)
                    toolbar.append($("<li />").append($(tb.getButton(buttons[i]))));

                return toolbar[0].outerHTML;
            },

            // find all toolbar-info-attributes in the HTML, convert to <ul><li> toolbar
            _processToolbars: function() {
                $(".sc-menu[data-toolbar]", $(".DnnModule-" + id)).each(function() {
                    var toolbarSettings = $.parseJSON($(this).attr("data-toolbar"));
                    var toolbarTag = $(this);
                    toolbarTag.replaceWith($2sxc(toolbarTag).manage.getToolbar(toolbarSettings));
                });
            },


        };
        return tb;
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

$2sxc.contentBlock = function(sxc, manage, cbTag) {
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

        // ajax update/replace the content of the content-block
        replace: function(newContent) {
            try {
                var newStuff = $(newContent);
                $(cbTag).replaceWith(newStuff);
                cbTag = newStuff;
            } catch (e) {
                console.log("Error while rendering template:");
                console.log(e);
            }
        },

        // this one assumes a replace / change has already happened, but now must be finalized...
        finalizeReplace: function() {
            // create new sxc-object
            cb.sxc = cb.sxc.recreate();
            cb.sxc.manage.toolbar._processToolbars(); // sub-optimal deep dependency
        },

        // retrieve new preview-content with alternate template and then show the result
        reload: function(templateId) {
            // if nothing specified, use stored id
            if (!templateId)
                templateId = cb.templateId;

            // if nothing specified / stored, cancel
            if (!templateId)
                return null;

            // if reloading a non-content-app, re-load the page
            if (!cb.editContext.ContentGroup.IsContent)
                return window.location.reload();

            // remember for future persist/save
            cb.templateId = templateId;

            var lang = cb.editContext.Language.Current; 

            // ajax-call, then replace
            return cb.sxc.webApi.get({
                    url: "view/module/rendertemplate",
                    params: { templateId: templateId, lang: lang, cbisentity: editContext.ContentBlock.IsEntity, cbid: editContext.ContentBlock.Id },
                    dataType: "html"
                })
                .then(cb.replace);
        },

        // set a content-item in this block to published, then reload
        publish: function(part, sortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/publish",
                params: { part: part, sortOrder: sortOrder }
            }).then(cb.reload);
        },

        // remove an item from a list, then reload
        removeFromList: function(sortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/removefromlist",
                params: { sortOrder: sortOrder }
            }).then(cb.reload);
        },

        // change the order of an item in a list, then reload
        changeOrder: function(sortOrder, destinationSortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/changeorder",
                params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder }
            }).then(cb.reload);
        },

        setTemplateChooserState: function (state) {
        	return cb.sxc.webApi.get({
        		url: "View/Module/SetTemplateChooserState",
        		params: { state: state }
        	});
        },

        addItem: function(sortOrder) {
        	return cb.sxc.webApi.get({
        		url: "View/Module/AddItem",
        		params: { sortOrder: sortOrder }
        	}).then(cb.reload);
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
        cancelTemplateChange: function() {
            cb.templateId = cb.undoTemplateId;
            cb.contentTypeId = cb.undoContentTypeId;

            // dialog...
            sxc.manage.dialog.justHide();
            cb.setTemplateChooserState(false);

            if (cb.editContext.ContentGroup.IsContent) // necessary to show the original template again
                cb.reload()
                    .then(cb.finalizeReplace);
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
                cb.setTemplateChooserState(isVisible)
                    .then(function() {
                        manage.editContext.ContentBlock.ShowTemplatePicker = isVisible;
                    });


        },


        // todo...
        prepareToAddContent: function() {
            return cb.persistTemplate(true, false);
        },

        persistTemplate: function(forceCreate, selectorVisibility) {
            // Save only if the currently saved is not the same as the new
            var groupExistsAndTemplateUnchanged = !!cb.editContext.ContentGroup.HasContent
                && (cb.undoTemplateId === cb.templateId);// !!cb.minfo.hasContent && (cb.undoTemplateId === cb.templateId);
            var promiseToSetState;
            if (groupExistsAndTemplateUnchanged)
                promiseToSetState = (cb.editContext.ContentBlock.ShowTemplatePicker)//.minfo.templateChooserVisible)
                    ? cb.setTemplateChooserState(false) // hide in case it was visible
                    : $.when(null); // all is ok, create empty promise to allow chaining the result
            else
                promiseToSetState = cb._saveTemplate(cb.templateId, forceCreate, selectorVisibility) 
                    .then(function (data, textStatus, xhr) {
                    	if (xhr.status !== 200) { // only continue if ok
                            alert("error - result not ok, was not able to create ContentGroup");
                            return;
                        }
                        var newGuid = data;
                        if (newGuid === null)
                            return;
                        newGuid = newGuid.replace(/[\",\']/g, ""); // fixes a special case where the guid is given with quotes (dependes on version of angularjs) issue #532
                        if (console)
                            console.log("created content group {" + newGuid + "}");

                        manage.updateContentGroupGuid(newGuid);
                    });

            var promiseToCorrectUi = promiseToSetState.then(function() {
                cb.undoTemplateId = cb.templateId; // remember for future undo
                cb.undoContentTypeId = cb.contentTypeId; // remember ...
                
                cb.editContext.ContentBlock.ShowTemplatePicker = false; // cb.minfo.templateChooserVisible = false;

                if (manage.dialog)
                	manage.dialog.justHide();

                if (!cb.editContext.ContentGroup.HasContent) // if it didn't have content, then it only has now...
                    cb.editContext.ContentGroup.HasContent = forceCreate;
                // if (!cb.minfo.hasContent) // if it didn't have content, then it only has now...
                //    cb.minfo.hasContent = forceCreate; // ...if we forced it to
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
        };

        var toolsAndButtons = $2sxc._toolbarManager(sxc, ec);

        var editManager = {
            // public method to find out if it's in edit-mode
            isEditMode: function () { return ec.Environment.IsEditable; },

            dialogParameters: ngDialogParams, // used for various dialogs
            toolbarConfig: toolsAndButtons.config, // used to configure buttons / toolbars
            updateContentGroupGuid: function(newGuid) {
                ec.ContentGroup.Guid = newGuid;
                toolsAndButtons.refreshConfig(); 
                editManager.toolbarConfig = toolsAndButtons.config;
            },

            editContext: ec, // metadata necessary to know what/how to edit
            dashboardConfig: dashConfig,
            commands: $2sxc._contentManagementCommands(sxc, cbTag),


            // Perform a toolbar button-action - basically get the configuration and execute it's action
            action: function(settings, event) {
                var conf = editManager.toolbar.actions[settings.action];
                settings = $2sxc._lib.extend({}, conf, settings); // merge conf & settings, but settings has higher priority
                if (!settings.dialog) settings.dialog = settings.action; // old code uses "action" as the parameter, now use verb ? dialog
                if (!settings.code) settings.code = editManager.commands._openNgDialog; // decide what action to perform

                var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
                if (conf.uiActionOnly)
                    return settings.code(settings, origEvent, editManager);

                // if more than just a UI-action, then it needs to be sure the content-group is created first
                editManager.contentBlock.prepareToAddContent()
                    .then(function() {
                        return settings.code(settings, origEvent, editManager);
                    });
            },

            //#region toolbar quick-access commands - might be used by other scripts, so I'm keeping them here for the moment, but may just delete them later
            toolbar: toolsAndButtons, // should use this from now on when accessing from outside
            getButton: toolsAndButtons.getButton,
            createDefaultToolbar: toolsAndButtons.createDefaultToolbar,
            getToolbar: toolsAndButtons.getToolbar,
            //_processToolbars: toolsAndButtons._processToolbars,
            //#endregion

            initContentBlocks: function() {
                // find the blocks / scope
            }
        };

        // finish init of sub-objects
        editManager.commands.init(editManager);
        editManager.contentBlock = $2sxc.contentBlock(sxc, editManager, cbTag);

        editManager.tempCreateCB = function(parent, field, index, app) {
            return sxc.webApi.get({
                url: "view/module/generatecontentblock",
                params: { parentId: parent, field: field, sortOrder: index, app: app }
            }).then(function(result) {
                console.log(result);
            });

        };

        // attach & open the mini-dashboard iframe
        if (ec.ContentBlock.ShowTemplatePicker)
            editManager.action({ "action": "layout" });


        return editManager;
    };


})();

    //#region translate - TODO re-enable translate
    $2sxc.translate = function(key) {
        return key;
    };
    //#endregion

(function () {

    var diag = $2sxc._dialog = {
        mode: "iframe",
        templates: {
            inline: "<iframe width='100%' height='200px' src='{{url}}'"
            // + "onload=\"this.syncHeight(this.contentWindow.document.body.scrollHeight);\""
            //+ "onload=\"this.style.height=this.contentWindow.document.body.scrollHeight + 'px';\""
            + "onresize=\"console.log('resize')\""
            + "></iframe>"
        }
    };

    diag.create = function (sxc, tag, url, closeCallback) {
        var diagBox = $(diag.templates.inline.replace("{{url}}", url))[0];    // build iframe tag

        diagBox.closeCallback = closeCallback;
        diagBox.sxc = sxc;

        //#region data bridge both ways
        diagBox.getManageInfo = function() {    return diagBox.sxc.manage.dialogParameters; };

        diagBox.getAdditionalDashboardConfig = function () {
            return diagBox.sxc.manage.dashboardConfig;
        };

        diagBox.getCommands = function() { return diagBox.vm;  };// created by inner code

        //#endregion

        //#region sync size - not completed yet
        // todo: sync sizes
        diagBox.syncHeight = function (height) {
            console.log("tried resize to " + height);
            diagBox.style.height = height + "px";
        };

        //#endregion

        //#region Visibility toggle & status

        diagBox.isVisible = function() { return diagBox.style.display !== "none";   };

        diagBox.toggle = function () { diagBox.style.display = diagBox.style.display === "none" ? "" : "none"; };

        diagBox.justHide = function () { diagBox.style.display = "none"; };
        //#endregion

        $(tag).before(diagBox);

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
                var moduleId = $(this).data("cb-instance");
                var cbid = $(this).data("cb-id");
                $2sxc(moduleId, cbid).manage.toolbar._processToolbars();
            } catch (e) { // Make sure that if one app breaks, others continue to work
                if (console && console.error)
                    console.error(e);
            }
        });
    }, 0);

    window.EavEditDialogs = [];
});
