// the default / initial buttons in a standard toolbar

(function () {
    $2sxc._toolbarManager.toolbarTemplate = [
        {
            name: "default",
            buttons: "edit,new,metadata,publish-auto,more"
        },
        {
            name: "list",
            buttons: "add,remove,moveup,movedown,sort,replace,more"
        },
        {
            name: "instance",
            // todo: add templatesettings, query
            buttons: "develop,contenttype,contentitems,more",
            defaults: {
                decorations: "group-pro"
            }
        },
        {
            name: "app",
            // todo: add multilanguage-resources & settings
            buttons: "app,zone,more",
            defaults: {
                decorations: "group-pro"
            }
        }
    ];
})();