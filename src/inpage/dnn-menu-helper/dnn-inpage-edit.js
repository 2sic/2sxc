


// Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
var $2sxcActionMenuMapper = function (moduleId) {
    return {
        changeLayoutOrContent: function () {
            $2sxc(moduleId).manage.action("layout");
        },
        addItem: function () {
            $2sxc(moduleId).manage.action("add", { "useModuleList": true, "sortOrder": 0 });
        },
        edit: function () {
            $2sxc(moduleId).manage.action("edit", { "useModuleList": true, "sortOrder": 0 });
        },
        adminApp: function () {
            $2sxc(moduleId).manage.action("app");
        },
        adminZone: function () {
            $2sxc(moduleId).manage.action("zone");
        },
        develop: function () {
            $2sxc(moduleId).manage.action("develop");
        }
    };
};