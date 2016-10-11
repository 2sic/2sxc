// the default / initial buttons in a standard toolbar

(function () {
    $2sxc._toolbarManager.toolbarTemplate = [
        //{
        //    name: "test",
        //    buttons: [
        //        {
        //            action: "edit",
        //            icon: "icon-sxc-code",
        //            title: "just quick edit!"
        //        },
        //        "inexisting-action",
        //        {
        //            action: "something fake"
        //        },
        //        "edit",
        //        {
        //            action: "publish-auto",
        //            addCondition: true
        //        },
        //        "more"
        //    ]
        //},
        {
            name: "default",
            buttons: "edit,new,metadata,publish,more"
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
                classes: "group-pro"
            }
        },
        {
            name: "app",
            // todo: add multilanguage-resources & settings
            buttons: "app,zone,more",
            defaults: {
                classes: "group-pro"
            }
        }
    ];
})();