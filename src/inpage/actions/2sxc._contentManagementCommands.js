
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
            var cmd = cmc.editManager._commands.create(specialSettings);

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

        executeAction: function (optionalName, settings, event) {
            // check if name is name (string) or object (settings)
            if (typeof (optionalName) === "string")
                settings = $2sxc._lib.extend(settings || {}, { "action": optionalName });
            else { event = settings; settings = optionalName; }

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
        }
    };

    return cmc;
    };


})();