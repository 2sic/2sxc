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
            user: { canDesign: ec.User.CanDesign, canDevelop: ec.User.CanDesign }
        };

        var toolsAndButtons = $2sxc._toolbarManager(sxc, ec);
        var cmds = $2sxc._contentManagementCommands(sxc, cbTag);

        var editManager = {
            // public method to find out if it's in edit-mode
            isEditMode: function () { return ec.Environment.IsEditable; },
            reloadWithAjax: ec.ContentGroup.IsContent,  // for now, allow all content to use ajax, apps use page-reload

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
                // finish init of sub-objects
                editManager.commands.init(editManager);
                editManager.contentBlock = $2sxc.contentBlock(sxc, editManager, cbTag);

                // attach & open the mini-dashboard iframe
                if (ec.ContentBlock.ShowTemplatePicker)
                    editManager.action({ "action": "layout" });

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
                sxc.webApi.get({
                    url: "view/module/movecontentblock",
                    params: { parentId: parentId, field: field, indexFrom: indexFrom, indexTo: indexTo }
                }).then(function(result) {
                    console.log("done moving!");
                    window.location.reload();
                    
                });

            }


        };

        editManager.init();
        return editManager;
    };


})();