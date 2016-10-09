(function() {
    $2sxc._toolbarManager.create = function (sxc, editContext) {
        var id = sxc.id, cbid = sxc.cbid;
        var allActions = $2sxc._actions.create({
            canDesign: editContext.User.CanDesign,
            templateId: editContext.ContentGroup.TemplateId,
            contentTypeId: editContext.ContentGroup.ContentTypeName
        });

        // #region helper functions
        function createToolbarConfig(context) {
            return {
                portalId: context.Environment.WebsiteId,
                tabId: context.Environment.PageId,
                moduleId: context.Environment.InstanceId,
                version: context.Environment.SxcVersion,

                contentGroupId: context.ContentGroup.Guid, // todo 8.4
                cbIsEntity: context.ContentBlock.IsEntity,
                cbId: context.ContentBlock.Id,
                appPath: context.ContentGroup.AppUrl,
                isList: context.ContentGroup.IsList
            };
        }

        // does some clean-up work on a button-definition object
        // because the target item could be specified directly, or in a complex internal object called entity
        function flattenActionDefinition(actDef) {
            if (actDef.entity && actDef.entity._2sxcEditInformation) {
                var editInfo = actDef.entity._2sxcEditInformation;
                actDef.useModuleList = (editInfo.sortOrder !== undefined); // has sort-order, so use list
                if (editInfo.entityId !== undefined)
                    actDef.entityId = editInfo.entityId;
                if (editInfo.sortOrder !== undefined)
                    actDef.sortOrder = editInfo.sortOrder;
                delete actDef.entity;   // clean up edit-info
            }
        }


        //#endregion helper functions



        var tb = {
            config: createToolbarConfig(editContext),
            refreshConfig: function() { tb.config = createToolbarConfig(); },
            actions: allActions,
            // Generate a button (an <a>-tag) for one specific toolbar-action. 
            // Expects: settings, an object containing the specs for the expected buton
            getButton: function (actDef) {
                // if the button belongs to a content-item, move the specs up to the item into the settings-object
                flattenActionDefinition(actDef);

                // retrieve configuration for this button
                var conf = allActions[actDef.action],
                    groupId = actDef.groupId,
                    showClasses = "group-" + groupId,
                    classesList = conf.showOn.split(","),
                    box = $("<div/>"),
                    symbol = $("<i class=\"" + conf.icon + "\" aria-hidden=\"true\"></i>");

                //for (var c = 0; c < classesList.length; c++)
                //    showClasses += " show-" + classesList[c];

                var button = $("<a />", {
                    'class': "sc-" + actDef.action + " " + showClasses + (conf.dynamicClasses ? " " + conf.dynamicClasses(actDef) : ""),
                    'onclick': "$2sxc(" + id + ", " + cbid + ").manage.action(" + JSON.stringify(actDef, function (key,value) { return key === "group" ? undefined : value; }) + ", event);",
                    'data-i18n': "[title]" + conf.title
                });

                button.data("groups", actDef.groups);

                button.html(box.html(symbol));

                return button[0].outerHTML;
            },

            // Assemble a default toolbar instruction set
            createDefaultToolbar: function (settings) {
                var defTb = $2sxc._toolbarManager.standardButtons(editContext);
                return tb._buildGroupedToolbar(defTb, settings);
            },

            _buildGroupedToolbar: function (groups, settings) {
                return $2sxc._toolbarManager.buttonHelpers
                    .createFlatList(groups, allActions, settings, tb.config);

                //var flatList = [];

                //function addButton(verb, groupId, groups, group) {
                //    // if this action has an add-condition, check that first
                //    if (!allActions[verb])
                //        return console.log("can't add button for verb: '" + verb + "'. action not found.");
                //    var add = allActions[verb].addCondition;
                //    if (add === undefined || ((typeof (add) === "function") ? add(settings, tb.config) : add))
                //        flatList.push($2sxc._lib.extend({}, settings, { action: verb, groupId: groupId, groups: groups, group: group }));
                //}

                //for (var s = 0; s < groups.length; s++) {
                //    var bs = groups[s].buttons.split(",");
                //    for (var v = 0; v < bs.length; v++)
                //        addButton(bs[v].trim(), s, groups.length, groups[s].name);
                //}

                //return flatList;
            },
            // Builds the toolbar and returns it as HTML
            // expects settings - either for 1 button or for an array of buttons
            getToolbar: function(settings) {
                var actionList = settings.action
                    ? [settings] // if single item with specified action, use this as our button-list
                    : $.isArray(settings)
                        ? settings // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                        : tb.createDefaultToolbar(settings);

                var tbClasses = "sc-menu group-0 show-set-0" + ((settings.sortOrder === -1) ? " listContent" : "");
                var toolbar = $("<ul />", { 'class': tbClasses, 'onclick': "var e = arguments[0] || window.event; e.stopPropagation();" });

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