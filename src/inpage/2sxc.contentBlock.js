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
        reloadAndReInitialize: function () {
            if (manage.reloadWithAjax) // necessary to show the original template again
                return cb.reload()
                    .then(function () {
                        // create new sxc-object
                        cb.sxc = cb.sxc.recreate();
                        cb.sxc.manage.toolbar._processToolbars(); // sub-optimal deep dependency
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
            if (!manage.reloadWithAjax)
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
            }).then(cb.reload);
        },

        // remove an item from a list, then reload
        removeFromList: function (sortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/removefromlist",
                params: { sortOrder: sortOrder }
            }).then(cb.reload);
        },

        // change the order of an item in a list, then reload
        changeOrder: function (sortOrder, destinationSortOrder) {
            return cb.sxc.webApi.get({
                url: "view/module/changeorder",
                params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder }
            }).then(cb.reload);
        },


        addItem: function (sortOrder) {
            return cb.sxc.webApi.get({
                url: "View/Module/AddItem",
                params: { sortOrder: sortOrder }
            }).then(cb.reload);
        },
        //#endregion

        _getPreviewWithTemplate: function (templateId) {
            return cb.sxc.webApi.get({
                url: "view/module/rendertemplate",
                params: {
                    templateId: templateId,
                    lang: cb.editContext.Language.Current,
                    cbisentity: editContext.ContentBlock.IsEntity,
                    cbid: editContext.ContentBlock.Id
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
                && (cb.undoTemplateId === cb.templateId);// !!cb.minfo.hasContent && (cb.undoTemplateId === cb.templateId);
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
                if (!groupExistsAndTemplateUnchanged && manage.reloadWithAjax)      // necessary to show the original template again
                    cb.reloadAndReInitialize();
            });

            return promiseToCorrectUi;
        }


    };

    return cb;
};


// ToDo: Move this to correct location (just poc-code)
var newBlockMenu = $("<div class='sc-content-block-menu'><a class='sc-content-block-menu-addcontent'>Add Content</a><a class='sc-content-block-menu-addapp'>Add app</a></div>");
$("body").append(newBlockMenu);

newBlockMenu.find('a').click(function () {
    var list = newBlockMenu.actionsFor.closest('.sc-content-block-list');

    var actionConfig = JSON.parse(list.attr('data-sc-list'));

    $2sxc(newBlockMenu.actionsFor).manage.createContentBlock(actionConfig.id, actionConfig.field, 0, 'Default');
});

$("body").on('mousemove', function (e) {
    // e.clientX, e.clientY holds the coordinates of the mouse
    var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed

    var nearest = null;
    var nearestDistance = maxDistance;

    var contentBlocks = $(".sc-content-block").add(".sc-content-block-list");

    // Find nearest content block
    contentBlocks.each(function () {
        var block = $(this);
        var x = block.offset().left;
        var w = block.width();
        var y = block.offset().top;
        var h = block.height();

        var mouseX = e.clientX + $(window).scrollLeft();
        var mouseY = e.clientY + $(window).scrollTop();

        // First check x coordinates - must be within container
        if (mouseX < x || mouseX > x + w)
            return;

        // For content-block elements, the menu must be visible at the end
        // For content-block-lists, the menu must be at top
        var cmpHeight = block.hasClass('sc-content-block') ? y + h : y;

        // Check if y coordinates are within boundaries
        var distance = Math.abs(mouseY - cmpHeight);

        if (distance < maxDistance && distance < nearestDistance) {

            newBlockMenu.css({
                'left': x,
                'top': cmpHeight,
                'width': w
            }).show();

            // Add class if menu creates modules
            //newBlockMenu.toggleClass('sc-content-block-menu-module', block.hasClass('sc-viewport'));

            // Keep current block as current on menu
            newBlockMenu.actionsFor = nearest = block;

            nearestDistance = distance;
        }
    });

    if (nearest === null)
        newBlockMenu.hide();
});

