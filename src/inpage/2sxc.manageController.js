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
                params: { parent: parent, field: field, sortOrder: index, app: app }
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