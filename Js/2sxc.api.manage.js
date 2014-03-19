
$2sxc.getManageController = function(id) {

    var moduleElement = $(".DnnModule-" + id);
    var config = $.parseJSON(moduleElement.attr("data-2sxc"));

    var manageController = {

        isEditMode: function() {
            // ToDo: Fix isEditMode
            return false;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        config: config,

        getLink: function(settings) {
            var dialogParams = {
                tabid: settings.tabId,
                ctl: 'editcontentgroupitem',
                mid: settings.moduleId,
                sortOrder: null,
                contentGroupId: null,
                cultureDimension: null,
                returnUrl: ''
            };

            return settings.dialogUrl
                + (settings.dialogUrl.indexOf('?') == -1 ? '?' : '&')
                + $.param(dialogParams);
        },

        openDialog: function(settings) {
            //dnnModal.open()
        },

        getButton: function (settings) {

            settings = $.extend(config, settings);
            var button = $('<a />', {
                'class': settings.action
            });

            // Bind click action
            button.on('click', function () { manageController.openDialog(settings); });

            return button;
        },

        getToolbar: function(entityId, editSettings, addSettings, newSettings) {

            return null;

        }

    };

    return manageController;
}