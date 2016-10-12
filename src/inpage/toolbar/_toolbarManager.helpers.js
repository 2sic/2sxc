// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.

(function () {
    var tools = $2sxc._toolbarManager.buttonHelpers = {

        createFlatList: function (unstructuredConfig, actions, itemSettings, config) {
            var realConfig = tools.ensureHierarchy(unstructuredConfig);

            var flat = tools.flattenList(realConfig);
            tools.warnAboutInexistingActions(flat, actions);

            tools.addCurrentItemSettings(flat, itemSettings);
            tools.fallbackAllSettings(flat, actions);

            tools.hideIfShowConditionNotMet(flat, itemSettings, config);
            return flat;
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
                    var current = btns[v];
                    
                    // if just a name, turn into a command
                    if(typeof current === "string")
                        current = { action: current };

                    // if it's a command w/action, wrap into command + trim
                    if (typeof current.action === "string")
                        $2sxc._lib.extend(current, { command: { action: current.action.trim() } });

                    // some clean-up
                    delete current.action;  // remove the action property
                    current.group = grp;    // attach group reference
                    btns[v] = current;  
                    flatList.push(btns[v]);
                }
                grp.buttons = btns; // ensure the internal def is also an array now
            }
            full.flat = flatList;
            return flatList;
        },

        // warn about buttons which don't have an action or an own click-event
        warnAboutInexistingActions: function (btnList, actions) {
            for(var i = 0; btnList[i]; i++) 
                if (!(btnList[i].onclick || actions[btnList[i].command.action]))
                    console.log("warning: toolbar-button without 'onclick' or known action-name: '" + btnList[i].action);
        },

        // remove buttons which are not valid based on add condition
        hideIfShowConditionNotMet: function(btnList, settings, config) {
            for (var i = 0; i < btnList.length; i++) {
                var add = btnList[i].showCondition;
                if (add !== undefined && (typeof (add) === "function"))
                    if (!add(settings, config)) {
                        btnList.splice(i, 1);
                        i--;
                    }
            }
        },

        // enhance the button with settings for this instance
        addCurrentItemSettings: function(btnList, settings) {
            for (var i = 0; i < btnList.length; i++) 
                $2sxc._lib.extend(btnList[i].command, settings);
        },

        btnProperties: [
            "classes",
            "icon",
            "title",
            "dynamicClasses",
            "showCondition"
        ],
        actProperties: [
            "params"    // todo: maybe different!
        ],

        // ensure all buttons have either own settings, or the fallbacks
        fallbackAllSettings: function(btnList, actions) {
            for (var i = 0; i < btnList.length; i++) {
                var btn = btnList[i];
                for (var d = 0; d < tools.btnProperties.length; d++)
                    tools.fallbackOneSetting(btn, actions, tools.btnProperties[d]);
            }
        },

        // configure missing button properties with various fallback options
        fallbackOneSetting: function(btn, actions, propName) {
            btn[propName] = btn[propName]   // by if already defined, use the already defined propery
                || (btn.group.defaults && btn.group.defaults[propName])     // if the group has defaults, try use use that property
                || (actions[btn.command.action] && actions[btn.command.action][propName]); // if there is an action, try to use that property name
        }
    };

})();