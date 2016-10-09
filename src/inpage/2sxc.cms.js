// this is the edit-API which is included when content can be edited
(function() {
    $2sxc._cms = function extendWithCms(sxc) {
        sxc.cms = {
            exec: function (actionName, config, event) {
                // todo: do some checks to confirm that this action is currently possible (prevent just throwing errors)
                // ...

                // execute the action
                sxc.manage.action(actionName, config, event);
            }

        };

        sxc.inlineUi = {
            // config: {},
            getButton: sxc.manage.getButton,
            getToolbar: sxc.manage.getToolbar
        };
    };
    console.log('cms loaded');

})();
