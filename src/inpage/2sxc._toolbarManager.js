(function() {
    $2sxc._toolbarManager = function(id, allActions, toolbarConfig) {
        var tb = {
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
                    'onclick': "javascript:$2sxc(" + id + ").manage.action(" + JSON.stringify(btnSettings) + ", event);",
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
                    if (add === undefined || ((typeof (add) === "function") ? add(settings, toolbarConfig) : add))
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
                    $(this).replaceWith($2sxc(id).manage.getToolbar(toolbarSettings));
                });
            },


        };
        return tb;
    };
})();