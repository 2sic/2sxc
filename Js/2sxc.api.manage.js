
$2sxc.getManageController = function(id) {

    var moduleElement = $('.DnnModule-' + id);
    var config = $.parseJSON(moduleElement.find('.Mod2sxcC').attr('data-2sxc')).manage.config;

    var manageController = {

        isEditMode: function() {
            return config.isEditMode;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        config: config,

        getLink: function (settings) {
            settings = $.extend(config, settings);

            var dialogParams = {
                //tabid: settings.tabId,
                ctl: 'editcontentgroup',
                mid: settings.moduleId,
                sortOrder: settings.sortOrder,
                contentGroupId: settings.contentGroupId,
                cultureDimension: settings.cultureDimension,
                returnUrl: ''
            };

            return settings.dialogUrl
                + (settings.dialogUrl.indexOf('?') == -1 ? '?' : '&')
                + $.param(dialogParams);
        },

        openDialog: function(settings) {

            var link = manageController.getLink(settings);

            if (window.dnnModal && dnnModal.show) {
                link += (link.indexOf('?') == -1 ? '?' : '&') + "popUp=true";
                dnnModal.show(link, /*showReturn*/true, 550, 950, true, '');
            } else {
                window.location = link;
            }
            
        },

        getButton: function (settings) {

            settings = $.extend(config, settings);
            var button = $('<a />', {
                'class': 'sc-' + settings.action
            });

            // Bind click action
            button.click(function () { manageController.openDialog(settings); });

            return button;
        },

        // Builds the toolbar and returns it as jQuery object
        getToolbar: function(entityId, editSettings, addSettings, newSettings) {

            var toolbar = $('<ul />', {
                'class': 'sc-menu'
            });

            editSettings = $.extend(editSettings, { action: 'edit', sortOrder: 0 });
            addSettings = $.extend(addSettings, { action: 'add' });
            newSettings = $.extend(newSettings, { action: 'new' });

            toolbar.append($('<li />').append(manageController.getButton(editSettings)));
            toolbar.append($('<li />').append(manageController.getButton(addSettings)));
            toolbar.append($('<li />').append(manageController.getButton(newSettings)));

            return toolbar;

        }

    };

    return manageController;
}