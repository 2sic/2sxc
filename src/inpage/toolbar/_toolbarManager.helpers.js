// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.

(function () {
    var tools = $2sxc._toolbarManager.buttonHelpers = {


        createDef: function(verb, icon, label, decorations, group) {
            return {
                verb: verb,
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

        createFlatList: function(groups, allActions, settings, config) {
            //var tools = $2sxc._toolbarManager.buttonHelpers;
            //var flat = tools.flattenList(groups);
            //flat = tools.removeInvalidButtons(flat, allActions, settings, tb.config);
            //flat = tools.addSettings(flat, settings);
            //return flat;

            var flatList = [];

            function addButton(verb, groupId, groups, group) {
                // if this action has an add-condition, check that first
                if (!allActions[verb]) {
                    console.log("can't add button for verb: '" + verb + "'. action not found.");
                    return;
                }
                var add = allActions[verb].addCondition;
                if (add === undefined || ((typeof (add) === "function") ? add(settings, config) : add))
                    flatList.push($2sxc._lib.extend({}, settings, { action: verb, groupId: groupId, groups: groups, group: group }));
            }

            for (var s = 0; s < groups.length; s++) {
                var bs = groups[s].buttons.split(",");
                for (var v = 0; v < bs.length; v++)
                    addButton(bs[v].trim(), s, groups.length, groups[s].name);
            }

            return flatList;
        },


        flattenList: function(btnGroups) {
            var flatList = [];

            function addButton(verb, groupId, groups, group) {
                // var add = allActions[verb].addCondition;
                // if (add === undefined || ((typeof (add) === "function") ? add(settings, tb.config) : add))
                    flatList.push($2sxc._lib.extend({} /*, settings*/, { verb: verb, action: verb, groupId: groupId, groups: groups /*, group: group */ }));
            }

            for (var s = 0; s < btnGroups.length; s++) {
                // first, enrich the set so it knows about it's context
                var grp = btnGroups[s];
                grp.index = s;
                grp.groups = btnGroups;

                // now process the butons
                var bs = grp.buttons.split(",");
                for (var v = 0; v < bs.length; v++)
                    addButton(bs[v].trim(), btnGroups);
            }

            return flatList;
        },

        // remove buttons which are not valid based on a condition
        removeInvalidButtons: function(btnList, actions, settings, config) {

            for (var i = 0; i < btnList.length; i++) {
                var btn = btnList[i];
                // if this action has an add-condition, check that first
                if (!actions[btn.verb]) {
                    console.log("can't add button for verb: '" + verb + "'. action not found.");
                    btnList.splice(i, 1);
                    i--;
                    continue;
                }
                var add = actions[btn.verb].addCondition;
                if (add !== undefined && (typeof (add) === "function"))
                    if (add(settings, config)) {
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
        }
    };

})();