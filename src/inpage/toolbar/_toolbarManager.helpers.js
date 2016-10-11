// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.

(function () {
    var tools = $2sxc._toolbarManager.buttonHelpers = {

        createFlatList: function(groups, actions, itemSettings, config) {
            var flat = tools.flattenList(groups);
            tools.removeInexistingActions(flat, actions);

            tools.addSettings(flat, itemSettings);
            tools.fallbackAllSettings(flat, actions);

            tools.removeInvalidButtons(flat, actions, itemSettings, config);
            return flat;
        },

        // change a hierarchy of buttons into a flat, simpler list
        flattenList: function(btnGroups) {
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
                    // replace the item on the index with a correct object
                    if(typeof btns[v] === "string")
                        btns[v] = { action: btns[v].trim() };
                    btns[v].group = grp;    // attach group reference
                    flatList.push(btns[v]);
                }
                grp.buttons = btns; // ensure the internal def is also an array now
            }
            return flatList;
        },

        // filter out buttons whose actions don't exist
        removeInexistingActions: function(btnList, actions) {
            for (var i = 0; i < btnList.length; i) 
                if (!actions[btnList[i].action]) {
                    console.log("can't add button for action: '" + btnList[i].action + "'. action not found.");
                    btnList.splice(i, 1);
                }
                else i++;
        },

        // remove buttons which are not valid based on add condition
        removeInvalidButtons: function(btnList, actions, settings, config) {
            for (var i = 0; i < btnList.length; i++) {
                var add = btnList[i].addCondition;
                if (add !== undefined && (typeof (add) === "function"))
                    if (!add(settings, config)) {
                        btnList.splice(i, 1);
                        i--;
                    }
            }
        },

        // enhance the button with settings for this instance
        addSettings: function(btnList, settings) {
            for (var i = 0; i < btnList.length; i++) {
                var btn = btnList[i];

                $2sxc._lib.extend(btn, settings);
            }
            return btnList;
        },

        properties: [
            "classes",
            "icon",
            "title",
            "dynamicClasses",
            "addCondition"
        ],

        fallbackAllSettings: function(btnList, actions) {
            for (var i = 0; i < btnList.length; i++) {
                var btn = btnList[i];
                for (var d = 0; d < tools.properties.length; d++)
                    tools.fallbackOneSetting(btn, actions, tools.properties[d]);
            }
        },

        // 
        fallbackOneSetting: function(btn, actions, propName) {//}, groupProp, actions, actProp) {
            btn[propName] = btn[propName]
                || (btn.group.defaults && btn.group.defaults[propName])
                || actions[btn.action][propName];
        }
    };

})();