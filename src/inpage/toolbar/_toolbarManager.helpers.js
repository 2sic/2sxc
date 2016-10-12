// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.

(function () {
    var tools = $2sxc._toolbarManager.buttonHelpers = {

        createFlatList: function (unstructuredConfig, actions, itemSettings, config) {
            var realConfig = tools.ensureHierarchy(unstructuredConfig);

            var btnList = tools.flattenList(realConfig);
            for (var i = 0; i < btnList.length; i++) 
                tools.btnCleanVariousInputFormats(btnList[i], actions, itemSettings);

            tools.removeButtonsWithUnmetConditions(btnList, itemSettings, config);
            return btnList;
        },

        btnCleanVariousInputFormats: function(btn, actions, itemSettings) {
            // warn about buttons which don't have an action or an own click-event
            tools.btnWarnUnknownAction(btn, actions);

            // enhance the button with settings for this instance
            tools.btnAddItemSettings(btn, itemSettings);

            // ensure all buttons have either own settings, or the fallbacks
            tools.btnAttachMissingSettings(btn, actions);            
        },

        ensureHierarchy: function (original) {
            // goal: return an object with this structure
            var fullSet = {
                name: "my toolbar",
                groups: [],
                defaults: {}
            };

            // the second simplest case: just an array of buttons, each configured
            if (Array.isArray(original)) {
                fullSet.groups = original;
                return fullSet;
            }
            return original;
        },



        // change a hierarchy of buttons into a flat, simpler list
        flattenList: function (full) {
            var btnGroups = full.groups;
            var flatList = [];
            for (var s = 0; s < btnGroups.length; s++) {
                // first, enrich the set so it knows about it's context
                var grp = $2sxc._lib.extend(btnGroups[s], { index: s, groups: btnGroups});

                // now process the buttons if string-format
                var btns = grp.buttons;
                if (typeof btns === "string")
                    btns = btns.split(",");

                // add each button - check if it's already an object or just the string
                for (var v = 0; v < btns.length; v++) {
                    btns[v] = tools.expandButtonConfig(btns[v]);
                    btns[v].group = grp;    // attach group reference
                    flatList.push(btns[v]);
                }
                grp.buttons = btns; // ensure the internal def is also an array now
            }
            full.flat = flatList;
            return flatList;
        },

        // takes an object like "actionname" or { action: "actionname", ... } and changes it to a { command: { action: "actionname" }, ... }
        expandButtonConfig: function (original) {
            if (original._expanded)
                return original;

            // if just a name, turn into a command
            if (typeof original === "string")
                original = { action: original };

            // if it's a command w/action, wrap into command + trim
            if (typeof original.action === "string")
                $2sxc._lib.extend(original, { command: { action: original.action.trim() } });

            // some clean-up
            delete original.action;  // remove the action property
            original._expanded = true;
            return original;
        },

        btnWarnUnknownAction: function(btn, actions) {
            if (!(actions[btn.command.action]))
                console.log("warning: toolbar-button with unknown action-name: '" + btn.command.action);
        },

        // remove buttons which are not valid based on add condition
        removeButtonsWithUnmetConditions: function(btnList, settings, config) {
            for (var i = 0; i < btnList.length; i++) {
                var add = btnList[i].showCondition;
                if (add !== undefined && (typeof (add) === "function"))
                    if (!add(settings, config)) {
                        btnList.splice(i, 1);
                        i--;
                    }
            }
        },

        btnAddItemSettings: function(btn, itemSettings) {
            $2sxc._lib.extend(btn.command, itemSettings);
        },

        btnProperties: [
            "classes",
            "icon",
            "title",
            "dynamicClasses",
            "showCondition"
        ],
        actProperties: [
            "params"    // todo: maybe different! DOESN'T WORK YET - MUST IMPLEMENT
        ],

        btnAttachMissingSettings: function(btn, actions) {
            for (var d = 0; d < tools.btnProperties.length; d++)
                tools.fallbackOneSetting(btn, actions, tools.btnProperties[d]);
        },

        // configure missing button properties with various fallback options
        fallbackOneSetting: function(btn, actions, propName) {
            btn[propName] = btn[propName]   // by if already defined, use the already defined propery
                || (btn.group.defaults && btn.group.defaults[propName])     // if the group has defaults, try use use that property
                || (actions[btn.command.action] && actions[btn.command.action][propName]); // if there is an action, try to use that property name
        }
    };

})();