$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $('.sc-menu').click(function (e) {
        e.stopPropagation();
    });

    var modules = $('.DnnModule-2sxc .Mod2sxcC[data-2sxc], .DnnModule-2sxc-app .Mod2sxcappC[data-2sxc]');

    modules.each(function () {
        var moduleId = $(this).data("2sxc").moduleId;
        $2sxc(moduleId).manage._processToolbars();

        if (window.angular != null) {
            angular.bootstrap($(this).find('.sc-selector-wrapper').get(0), ['2sxc.view']);
        }
    });


    window.EavEditDialogs = [];

});

// ToDo: Make sure AngularJS is loaded when needed (prevents loading AngularJS twice if it is already loaded by another module)
//if (modules.length > 0 && angular == null) {
//    $.getScript(modules[0].);
//}

