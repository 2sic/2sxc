// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it
// all in-page toolbars etc.
// if loaded, it's found under the $2sxc(module).manage
// it has commands to
// - getButton
// - getToolbar
// - action(...)

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


    $2sxc._getManageController = function (sxc) {
        var contentBlockTag = getContentBlockTag(sxc);
        var editContext = getContextInfo(contentBlockTag);

        // assemble all parameters needed for the dialogs if we open anything
        var ngDialogParams = {
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
            // todo: probably move the user into the dashboard info
            user: { canDesign: editContext.User.CanDesign, canDevelop: editContext.User.CanDesign },
            approot: editContext.ContentGroup.AppUrl || null // this is the only value which doesn't have a slash by default.  note that the app-root doesn't exist when opening "manage-app"
        };

        var dashConfig = {
            appId: editContext.ContentGroup.AppId,
            isContent: editContext.ContentGroup.IsContent,
            hasContent: editContext.ContentGroup.HasContent,
            isList: editContext.ContentGroup.IsList,
            templateId: editContext.ContentGroup.TemplateId,
            contentTypeId: editContext.ContentGroup.ContentTypeName,
            templateChooserVisible: editContext.ContentBlock.ShowTemplatePicker, // todo: maybe move to content-goup
            user: { canDesign: editContext.User.CanDesign, canDevelop: editContext.User.CanDesign },
            supportsAjax: editContext.ContentGroup.SupportsAjax
        };

        var toolsAndButtons = $2sxc._toolbarManager.create(sxc, editContext);
        var cmds = $2sxc._contentManagementCommands(sxc, contentBlockTag);

        var editManager = {
            // public method to find out if it's in edit-mode
            isEditMode: function () { return editContext.Environment.IsEditable; },
            reloadWithAjax: editContext.ContentGroup.SupportsAjax,  // for now, allow all content to use ajax, apps use page-reload

            dialogParameters: ngDialogParams, // used for various dialogs
            toolbarConfig: toolsAndButtons.config, // used to configure buttons / toolbars

            editContext: editContext, // metadata necessary to know what/how to edit
            dashboardConfig: dashConfig,
            _commands: cmds,

            // Perform a toolbar button-action - basically get the configuration and execute it's action
            action: cmds.executeAction,

            //#region toolbar quick-access commands - might be used by other scripts, so I'm keeping them here for the moment, but may just delete them later
            toolbar: toolsAndButtons, // should use this from now on when accessing from outside
            getButton: toolsAndButtons.getButton,
            // 2016-10-11 maybe breaking change, but shoudn't be exposed
            //createDefaultToolbar: toolsAndButtons.defaultButtonList,
            getToolbar: toolsAndButtons.getToolbar,
            //#endregion

            // init this object 
            init: function init() {
                // enhance UI in case there are known errors / issues
                if (editContext.error.type)
                    editManager._handleErrors(editContext.error.type, contentBlockTag);

                // finish init of sub-objects
                editManager._commands.init(editManager);
                editManager.contentBlock = $2sxc._contentBlock.create(sxc, editManager, contentBlockTag);

                // attach & open the mini-dashboard iframe
                if (!editContext.error.type && editContext.ContentBlock.ShowTemplatePicker)
                    editManager.action("layout");

            },

            // private: show error
            _handleErrors: function (errType, cbTag) {
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
                editContext.ContentGroup.Guid = newGuid;
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