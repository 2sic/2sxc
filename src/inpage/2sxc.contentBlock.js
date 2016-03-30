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

$2sxc.contentBlock = function(sxc, manage) {
	var viewPortSelector = ".DnnModule-" + sxc.id + " .sc-viewport";

	//#region loads of old stuff, should be cleaned, mostly just copied from the angulare coe

	var cViewWithoutContent = "_LayoutElement"; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty

    // var manageInfo = manage._manageInfo; // todo: not nice dependecy on internal variable
    var editContext = manage.editContext;
    var ctid = (editContext.ContentGroup.ContentTypeName === "" && editContext.ContentGroup.TemplateId !== null)
        ? cViewWithoutContent // has template but no content, use placeholder
        : editContext.ContentGroup.ContentTypeName;// manageInfo.contentTypeId;

	//#endregion

    var cb = {
        sxc: sxc,
        loading: 0, // counter for multiple ajax running, purpose not clear...
        editContext: editContext,    // todo: not ideal depedency, but ok...
        // minfo: manageInfo, // todo: not nice dependecy; will also need to reload...
        templateId: editContext.ContentGroup.TemplateId,// manageInfo.templateId,
        undoTemplateId: editContext.ContentGroup.TemplateId, //manageInfo.templateId,
        contentTypeId: ctid,
        undoContentTypeId: ctid,

        // ajax update/replace the content of the content-block
        replace: function(newContent) {
            try {
                $(viewPortSelector).html(newContent);
                manage._processToolbars();
            } catch (e) {
                console.log("Error while rendering template:");
                console.log(e);
            }
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

            console.log("new loading");
            var lang = cb.editContext.Language.Current; //.minfo.lang;

            // ajax-call, then replace
            cb.loading++;
            return sxc.webApi.get({
                    url: "view/module/rendertemplate",
                    params: { templateId: templateId, lang: lang },
                    dataType: "html"
                })
                .then(function(response) {
                    cb.replace(response);
                    cb.loading--;
                });
        },
        refresh: function() { cb.reload(); },

        // set a content-item in this block to published, then reload
        publish: function(part, sortOrder) {
            return sxc.webApi.get({
                url: "view/module/publish",
                params: { part: part, sortOrder: sortOrder }
            }).then(cb.refresh);
        },

        // remove an item from a list, then reload
        removeFromList: function(sortOrder) {
            return sxc.webApi.get({
                url: "view/module/removefromlist",
                params: { sortOrder: sortOrder }
            }).then(cb.refresh);
        },

        // change the order of an item in a list, then reload
        changeOrder: function(sortOrder, destinationSortOrder) {
            return sxc.webApi.get({
                url: "view/module/changeorder",
                params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder }
            }).then(cb.refresh);
        },

        setTemplateChooserState: function (state) {
        	return sxc.webApi.get({
        		url: "View/Module/SetTemplateChooserState",
        		params: { state: state }
        	});
        },

        addItem: function(sortOrder) {
        	return sxc.webApi.get({
        		url: "View/Module/AddItem",
        		params: { sortOrder: sortOrder }
        	}).then(cb.refresh);
        },

        _saveTemplate: function (templateId, forceCreateContentGroup, newTemplateChooserState) {
            return sxc.webApi.get({
                url: "View/Module/SaveTemplateId",
                params: {
                    templateId: templateId,
                    forceCreateContentGroup: forceCreateContentGroup,
                    newTemplateChooserState: newTemplateChooserState
                }
            });
        },

        // todo...
        prepareToAddContent: function() {
            return cb.persistTemplate(true, false);
        },

        persistTemplate: function(forceCreate, selectorVisibility) {
            // Save only if the currently saved is not the same as the new
            var groupExistsAndTemplateUnchanged = !!cb.editContext.ContentGroup.HasContent && (cb.undoTemplateId === cb.templateId);// !!cb.minfo.hasContent && (cb.undoTemplateId === cb.templateId);
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

                        // todo: will need more complexity / 2016-03-30 unsure if this is actually ever re-used...
                        // cb.minfo.config.contentGroupId = newGuid; // update internal ContentGroupGuid 
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