


// Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
var $2sxcActionMenuMapper = function (moduleId) {
    return {
        changeLayoutOrContent: function () {
            $2sxc(moduleId).manage.action({ 'action': 'layout' });
        },
        addItem: function () {
            $2sxc(moduleId).manage.action({ 'action': 'add', 'useModuleList': true });
        },
        edit: function () {
            $2sxc(moduleId).manage.action({ 'action': 'edit', 'useModuleList': true, 'sortOrder': 0 });
        },
        adminApp: function () {
            $2sxc(moduleId).manage.action({ 'action': 'app' });
        },
        adminZone: function () {
            $2sxc(moduleId).manage.action({ 'action': 'zone' });
        },
        develop: function () {
            $2sxc(moduleId).manage.action({ 'action': 'develop' });
        }
    };
};