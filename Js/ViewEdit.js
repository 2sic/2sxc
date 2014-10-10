$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $('.sc-menu').click(function (e) {
        e.stopPropagation();
    });

    $('.DnnModule-2sxc .Mod2sxcC, .DnnModule-2sxc-app .Mod2sxcappC').each(function () {
        if (!$(this).is("[data-2sxc]"))
            return;

        var moduleId = $.parseJSON($(this).attr('data-2sxc')).moduleId;

        $('.sc-menu[data-toolbar]', this).each(function () {
            var toolbarSettings = $.parseJSON($(this).attr('data-toolbar'));
            $(this).replaceWith($2sxc(moduleId).manage.getToolbar(toolbarSettings));
        });

        $('.sc-template-selector', this).change(function() {
            $2sxc(moduleId).manage._changeTemplate($(this).val());
        });
    });


    window.EavEditDialogs = [];
});
