// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.

(function () {
    var tools = $2sxc._toolbarManager.buttonHelpers = {


        createDef: function(action, icon, label, decorations, group) {
            return {
                action: action,
                icon: icon,
                label: label,
                decorations: decorations,
                group: group
            };
        },

        expandDef: function(groups) {
            for (var i = 0; i < groups.length; i++) {
                var group = groups[i];

                // first expand buttons if it's a string
                if (typeof group.buttons === "string") 
                    group.buttons = group.buttons.split(",");

                for (var b = 0; b < group.buttons.length; b++) {
                    var btn = group.buttons[b];
                    if (typeof btn === "string")
                        btn = tools.createDef(btn);

                    if (group.defaults)
                        $2sxc._lib.extend(btn, group.defaults);

                    group.buttons[b] = btn;
                }
            }
        },

        createFlatList: function(groups, actions, itemSettings, config) {
            var flat = tools.flattenList(groups);
            tools.removeInvalidButtons(flat, actions, itemSettings, config);
            tools.addSettings(flat, itemSettings);
            tools.fallbackAllSettings(flat, actions);
            return flat;
        },

        // change a hierarchy of buttons into a flat, simpler list
        flattenList: function(btnGroups) {
            var flatList = [];
            for (var s = 0; s < btnGroups.length; s++) {
                // first, enrich the set so it knows about it's context
                var grp = $2sxc._lib.extend(btnGroups[s], { index: s, groups: btnGroups});

                // now process the butons
                var bs = grp.buttons.split(",");
                for (var v = 0; v < bs.length; v++)
                    flatList.push({ action: bs[v].trim(), group: grp });
            }
            return flatList;
        },

        // remove buttons which are not valid based on add condition
        removeInvalidButtons: function(btnList, actions, settings, config) {

            for (var i = 0; i < btnList.length; i++) {
                var btn = btnList[i];
                // if this action has an add-condition, check that first
                if (!actions[btn.action]) {
                    console.log("can't add button for action: '" + btn.action + "'. action not found.");
                    btnList.splice(i, 1);
                    i--;
                    continue;
                }
                var add = actions[btn.action].addCondition;
                if (add !== undefined && (typeof (add) === "function"))
                    if (!add(settings, config)) {
                        btnList.splice(i, 1);
                        i--;
                    }
            }
            return btnList;
        },

        // enhance the button with settings for this instance
        addSettings: function(btnList, settings) {
            for (var i = 0; i < btnList.length; i++) {
                var btn = btnList[i];

                $2sxc._lib.extend(btn, settings);
            }
            return btnList;
        },

        properties: ["decorations"],

        fallbackAllSettings: function(btnList, actions) {
            for (var i = 0; i < btnList.length; i++) {
                var btn = btnList[i];
                for (var d = 0; d < tools.properties.length; d++)
                    tools.fallbackOneSetting(btn, actions, tools.properties[d]);
            }
        },

        fallbackOneSetting: function(btn, actions, propName) {//}, groupProp, actions, actProp) {
            btn[propName] = btn[propName] || (btn.group.defaults && btn.group.defaults[propName]) || actions[propName];
        }
    };

})();