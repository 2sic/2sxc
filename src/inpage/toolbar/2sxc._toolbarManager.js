(function() {
    $2sxc._toolbarManager = function (sxc, editContext) {
        var id = sxc.id, cbid = sxc.cbid;
        var actionParams = {
            canDesign: editContext.User.CanDesign,
            templateId: editContext.ContentGroup.TemplateId,
            contentTypeId: editContext.ContentGroup.ContentTypeName
        };

        // #region helper functions
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
                isList: editContext.ContentGroup.IsList
            };
            return toolbarConfig;
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

        var allActions = $2sxc._actions.create(actionParams);

        var tb = {
            config: createToolbarConfig(),
            refreshConfig: function() { tb.config = createToolbarConfig(); },
            actions: allActions,
            // Generate a button (an <a>-tag) for one specific toolbar-action. 
            // Expects: settings, an object containing the specs for the expected buton
            getButton: function (actDef) {
                // if the button belongs to a content-item, move the specs up to the item into the settings-object
                flattenActionDefinition(actDef);

                // retrieve configuration for this button
                var conf = allActions[actDef.action],
                    showClasses = "",
                    classesList = conf.showOn.split(","),
                    box = $("<div/>"),
                    symbol = $("<i class=\"" + conf.icon + "\" aria-hidden=\"true\"></i>");

                for (var c = 0; c < classesList.length; c++)
                    showClasses += " show-" + classesList[c];

                var button = $("<a />", {
                    'class': "sc-" + actDef.action + " " + showClasses + (conf.dynamicClasses ? " " + conf.dynamicClasses(actDef) : ""),
                    'onclick': "$2sxc(" + id + ", " + cbid + ").manage.action(" + JSON.stringify(actDef) + ", event);",
                    'data-i18n': "[title]" + conf.title
                });

                // 2016-10-09 2dm moved to the unpublish-auto action
                //// todo: move the following lines into the button-config and just call from here
                //// if publish-button and not published yet, show button (otherwise hidden) & change icon
                //if (actDef.action === "publish" && actDef.isPublished === false) {
                //    button.addClass("show-default").removeClass("show-edit")
                //        .attr("data-i18n", "[title]Toolbar.Unpublished");
                //        //.attr("title", $2sxc.translate("Toolbar.Unpublished"));
                //    symbol.removeClass(conf.icon).addClass(conf.icon);
                //}

                button.html(box.html(symbol));

                return button[0].outerHTML;
            },

            // Assemble a default toolbar instruction set
            createDefaultToolbar: function (settings) {
                var defTb = [
                    {
                        name: "default",
                        buttons: "edit,new,metadata,unpublish-auto,more"
                    },
                    {
                        name: "list",
                        buttons: "add,remove,moveup,movedown,sort,replace,more"
                    },
                    {
                        name: "instance",
                        buttons: "develop,contenttype,contentitems,more" // todo: add templatesettings, query
                    },
                    {
                        name: "app",
                        buttons: "app,zone,more" // todo: add multilanguage-resources & settings
                    }
                ];

                // Create a standard menu with all standard buttons
                var buttons = [], buttons2 = [];

                buttons.add = function (verb) {
                    // if this action has an add-condition, check that first
                    var add = allActions[verb].addCondition;
                    if (add === undefined || ((typeof (add) === "function") ? add(settings, tb.config) : add))
                        buttons.push($2sxc._lib.extend({}, settings, { action: verb }));
                };

                buttons2.add = function (verb, group) {
                    // if this action has an add-condition, check that first
                    if (!allActions[verb])
                        return console.log("can't add button for verb: '" + verb + "'. action not found.");
                    var add = allActions[verb].addCondition;
                    if (add === undefined || ((typeof (add) === "function") ? add(settings, tb.config) : add))
                        buttons2.push($2sxc._lib.extend({}, settings, { action: verb, group: group }));
                };

                for (var btn in allActions)
                    if (allActions.hasOwnProperty(btn))
                        buttons.add(btn);

                for (var s = 0; s < defTb.length; s++) {
                    var bs = defTb[s].buttons.split(",");
                    for (var v = 0; v < bs.length; v++)
                        buttons2.add(bs[v].trim(), defTb[s].name);
                }

                return buttons2;
            },

            // Builds the toolbar and returns it as HTML
            // expects settings - either for 1 button or for an array of buttons
            getToolbar: function(settings) {
                var actionList = settings.action
                    ? [settings] // if single item with specified action, use this as our button-list
                    : $.isArray(settings)
                        ? settings // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                        : tb.createDefaultToolbar(settings);

                var tbClasses = "sc-menu show-set-0" + ((settings.sortOrder === -1) ? " listContent" : "");
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