$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $('.sc-menu').click(function (e) {
        e.stopPropagation();
    });

    var modules = $('.DnnModule-2sxc .Mod2sxcC[data-2sxc], .DnnModule-2sxc-app .Mod2sxcappC[data-2sxc]');

    modules.each(function () {
        var moduleId = $(this).data("2sxc").moduleId;
        $2sxc(moduleId).manage._processToolbars();
    });

    window.EavEditDialogs = [];
});


// Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
var $2sxcActionMenuMapper = function (moduleId) {
    return {
        changeLayoutOrContent: function () {
            $2sxc(moduleId).manage._getSelectorScope().setTemplateChooserState(true);
        },
        addItem: function () {
            $2sxc(moduleId).manage.action({ 'action': 'add', 'useModuleList': true });
        },
        edit: function () {
            $2sxc(moduleId).manage.action({ 'action': 'edit', 'useModuleList': true, 'sortOrder': 0 });
        }
    };
};