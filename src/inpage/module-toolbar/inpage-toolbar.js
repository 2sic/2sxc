// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it
// all in-page toolbars etc.
(function () {
    //#region helper functions
    function getContentBlockTag(sxci) {
         return $("div[data-cb-id='" + sxci.cbid + "']")[0];
    }

    function getContextInfo(sxci) {
        var attr = getContentBlockTag(sxci).getAttribute("data-edit-context");
        return $.parseJSON(attr || "");
    }
    //#endregion


$2sxc.getManageController = function (id, sxc) {

    var editContext = getContextInfo(sxc),
        ecEnv = editContext.Environment,
        ecCb = editContext.ContentBlock,
        ecCg = editContext.ContentGroup,
        ecLang = editContext.Language;

    var moduleElement = $(".DnnModule-" + id);

    // assemble all parameters needed for the dialogs if we open anything
    var ngDialogParams = {
        zoneId: ecCg.ZoneId,
        appId: ecCg.AppId,
        tid: ecEnv.PageId,
        mid: ecEnv.InstanceId, 
        lang: ecLang.Current,
        langpri: ecLang.Primary, 
        langs:  JSON.stringify(ecLang.All),
        portalroot: ecEnv.WebsiteUrl,
        websiteroot: ecEnv.SxcRootUrl,
        user: { canDesign: editContext.User.CanDesign, canDevelop: editContext.User.CanDesign } ,
        // note that the app-root doesn't exist when opening "manage-app"
        approot: ecCg.AppUrl || null // this is the only value which doesn't have a slash by default
    };
    var actionParams = {
        canDesign: editContext.User.CanDesign,
        templateId: ecCg.TemplateId,
        contentTypeId: ecCg.ContentTypeName
    };
    var toolbarConfig = {
        portalId: ecEnv.WebsiteId,
        tabId: ecEnv.PageId,
        moduleId: ecEnv.InstanceId,
        contentGroupId: ecCg.Guid,
        appPath: ecCg.AppUrl,
        isList: ecCg.IsList,
        version: ecEnv.SxcVersion
    };


    // 2016-03-30 2dm: I think this isn't ever used, that it's a leftover of something previous, because neither values seem to hold anything
    // manageInfo.config.contentType = manageInfo.config.contentType || manageInfo.config.attributeSetName; // still support the old name...there was a good reason but I don't know it any more...

    var actionButtonsConf = $2sxc._actions.create(actionParams);
    var toolsAndButtons = $2sxc._toolbarManager(id, actionButtonsConf, toolbarConfig);

    var editor = {
        // public method to find out if it's in edit-mode
        isEditMode: function() { return ecEnv.IsEditable; },

        dialogParameters: ngDialogParams,   // used for various dialogs
        toolbarConfig: toolbarConfig,       // used to configure buttons / toolbars
        editContext: editContext,           // metadata necessary to know what/how to edit

        // create a dialog link
        linkToNgDialog: function(specialSettings) {
            var cmd = createCommandObject(editor, specialSettings);

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
        openNgDialog: function(settings, event, closeCallback) {

            var callback = function() {
                editor.rootCB.reload();
                closeCallback();
            };
            var link = editor.linkToNgDialog(settings);

            if (settings.newWindow || (event && event.shiftKey))
                return window.open(link);
            else {
                if (settings.inlineWindow)
                //window.open(link);
                    return $2sxc.dialog.create(id, moduleElement, link, callback);
                else
                    return $2sxc.totalPopup.open(link, callback);
            }
        },

        // Perform a toolbar button-action - basically get the configuration and execute it's action
        action: function(settings, event) {
            var conf = actionButtonsConf[settings.action];
            settings = $2sxc._lib.extend({}, conf, settings); // merge conf & settings, but settings has higher priority
            if (!settings.dialog) settings.dialog = settings.action; // old code uses "action" as the parameter, now use verb ? dialog
            if (!settings.code) settings.code = editor.openNgDialog; // decide what action to perform

            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            if (conf.uiActionOnly)
                return settings.code(settings, origEvent, editor);

            // if more than just a UI-action, then it needs to be sure the content-group is created first
            //manage._getAngularVm().prepareToAddContent()
            editor.rootCB.prepareToAddContent()
                .then(function() {
                    return settings.code(settings, origEvent, editor);
                });
        },

        // toolbar quick-access commands
        getButton: toolsAndButtons.getButton,
        createDefaultToolbar: toolsAndButtons.createDefaultToolbar,
        getToolbar: toolsAndButtons.getToolbar,
        _processToolbars: toolsAndButtons._processToolbars
    };

    // attach & open the mini-dashboard iframe
    if (ecCb.ShowTemplatePicker)
        editor.action({ "action": "layout" });

    editor.rootCB = $2sxc.contentBlock(sxc, editor);

    return editor;
};


    // assemble an object which will store the configuration and execute it
    function createCommandObject(editManager, specialSettings) {
        var settings = $2sxc._lib.extend({}, editManager.toolbarConfig, specialSettings); // merge button with general toolbar-settings
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
                    Title: $2sxc.translate(sectionLanguageKey)
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

                cmd.params.items = JSON.stringify(cmd.items);// Serialize/json-ify the complex items-list

                var ngDialogUrl = editManager.editContext.Environment.SxcRootUrl + "desktopmodules/tosic_sexycontent/dist/dnn/ui.html?sxcver="
                    + editManager.editContext.Environment.SxcVersion;

                var isDebug = $2sxc.urlParams.get("debug") ? "&debug=true" : "";
                return ngDialogUrl
                    + "#" + $.param(editManager.dialogParameters)
                    + "&" + $.param(cmd.params)
                    + isDebug;
                //#endregion
            }
        };
        return cmd;
    }

})();