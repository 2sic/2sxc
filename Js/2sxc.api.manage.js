
$2sxc.getManageController = function(id) {

    var moduleElement = $('.DnnModule-' + id);
    var config = $.parseJSON(moduleElement.find('.Mod2sxcC, .Mod2sxcappC').attr('data-2sxc')).manage.config;

    var manageController = {

        isEditMode: function() {
            return config.isEditMode;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        config: config,

        getLink: function (settings) {
            settings = $.extend({}, config, settings);

            var params = {
                ctl: 'editcontentgroup',
                mid: settings.moduleId
            };

            if (settings.cultureDimension && settings.cultureDimension != null)
                params.cultureDimension = settings.cultureDimension;

            if (settings.action == 'new')
                params.editMode = "New";

            if (!settings.useModuleList) {
                if (settings.action != 'new')
                    params.entityId = settings.entityId;
                if(settings.attributeSetName)
                    params.attributeSetName = settings.attributeSetName;
            } else {
                params.sortOrder = settings.sortOrder;
                params.contentGroupId = settings.contentGroupId;
            }

            return settings.dialogUrl
                + (settings.dialogUrl.indexOf('?') == -1 ? '?' : '&')
                + $.param(params);
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

        action: function(settings) {

            if (settings.action == 'edit' || settings.action == 'new')
                manageController.openDialog(settings);
            else if (settings.action == 'add') {
                moduleElement.find("input[id$=hfContentGroupItemSortOrder]:first").val(settings.sortOrder);
                moduleElement.find("input[id$=hfContentGroupItemAction]:first").val("add");

                $("form").submit();
            } else {
                throw "Action " + settings.action + " not known.";
            }
        },

        getButton: function (settings) {

            settings = $.extend({}, config, settings);
            var button = $('<a />', {
                'class': 'sc-' + settings.action,
                'href': 'javascript:$2sxc(' + settings.moduleId + ').manage.action(' + JSON.stringify(settings) + ')'
            });

            // Bind click action
            //button.click(function () { manageController.openDialog(settings); });

            return button[0].outerHTML; 
        },

        // Builds the toolbar and returns it as HTML
        getToolbar: function (settings) {

            var buttons = [];

            if ($.isArray(settings)) {
                buttons = settings;
            } else {
                buttons = [
                    $.extend({ action: 'edit' }, settings)
                ];

                if (config.isList && settings.sortOrder != -1) {
                    buttons.push($.extend({ action: 'add' }, settings));
                    buttons.push($.extend({ action: 'new' }, settings));
                }
            }

            var toolbar = $('<ul />', { 'class': 'sc-menu' });

            for (var i = 0; i < buttons.length; i++)
                toolbar.append($('<li />').append($(manageController.getButton(buttons[i]))));

            return toolbar[0].outerHTML;
        }

    };

    return manageController;
}