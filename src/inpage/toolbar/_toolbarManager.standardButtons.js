// the toolbar manager is an internal helper
// taking care of toolbars, buttons etc.

(function () {

    $2sxc._toolbarManager.standardButtons = function(canDesign) {
        var btns = $2sxc._toolbarManager.toolbarTemplate;
        if (!canDesign)
            btns.groups.splice(2, 1); // remove this menu
        return btns;
    };

})();