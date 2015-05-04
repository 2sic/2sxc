// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it
$2sxc.getManageController = function (id) {

    var moduleElement = $('.DnnModule-' + id);
    var manageInfo = $.parseJSON(moduleElement.find('.Mod2sxcC, .Mod2sxcappC').attr('data-2sxc')).manage;
    var toolbarConfig = manageInfo.config;
    toolbarConfig.returnUrl = window.location.href;
    var isEditMode = manageInfo.isEditMode;
    var sf = $.ServicesFramework(id);

    // all the standard buttons with the display configuration and click-action
    var actionButtonsConf = {
        'default': {
            icon: 'glyphicon-fire', hideFirst: true,
            action: function (event, settings) { alert('not implemented yet') }
        },
        'edit': {
            title: 'Edit', icon: 'glyphicon-pencil', lightbox: true, hideFirst: false,
            action: function (event, settings) {
                manageController._openDialog(settings);
            }
        },
        'editinline': {
            title: 'Edit inline', icon: 'glyphicon-pencil', lightbox: false,
            action: function (event, settings) { alert('not implemented yet') }
        },
        'new': {
            title: 'New', icon: 'glyphicon-plus', lightbox: true, hideFirst: false,
            action: function (event, settings) {
                manageController._openDialog($.extend({}, settings, { sortOrder: settings.sortOrder + 1 }));
            }
        },
        'add': {
            title: 'Add', icon: 'glyphicon-plus', lightbox: false, hideFirst: true,
            action: function (event, settings) {
                // ToDo: Remove dependency to AngularJS, should use 2sxc.api.js
                manageController._getSelectorScope().addItem(settings.sortOrder + 1);
            }
        },
        'replace': {
            title: 'Replace', icon: 'glyphicon-random', lightbox: false, hideFirst: true,
            action: function (event, settings) {
                manageController._openDialog(settings);
            }
        },
        'publish': {
            title: 'Published', icon: 'glyphicon-eye-open disabled', icon2: 'glyphicon-eye-close disabled', lightbox: false, hideFirst: true, disabled: true,
            action: function (event, settings) {
                alert('Status: ' + (settings.isPublished ? 'published' : 'not published'));
            }
        },
        'moveup': {
            title: 'Move up', icon: 'glyphicon-arrow-up', icon2: 'glyphicon-arrow-up', lightbox: false, hideFirst: true, disabled: false,
            action: function (event, settings) {
                manageController._getSelectorScope().changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
            }
        },
        'movedown': {
            title: 'Move down', icon: 'glyphicon-arrow-down', icon2: 'glyphicon-arrow-down', lightbox: false, hideFirst: true, disabled: false,
            action: function (event, settings) {
                manageController._getSelectorScope().changeOrder(settings.sortOrder, settings.sortOrder + 1);
            }
        },
        'remove': {
            title: 'remove', icon: 'glyphicon-remove-circle', lightbox: false, hideFirst: true, disabled: true,
            action: function (event, settings) {
                if (confirm("This will remove this content-item from this list, but not delete it (so you can add it again later). \nSee 2sxc.org/help for more. \n\nOk to remove?")) {
                    manageController._getSelectorScope().removeFromList(settings.sortOrder);
                }
            }
        },
        'more': {
            title: 'More', icon: 'glyphicon-option-horizontal', icon2: 'glyphicon-option-vertical', borlightboxder: false, hideFirst: false,
            action: function (event, settings) {
                $(event.target).toggleClass(this.icon).toggleClass(this.icon2).closest('ul.sc-menu').toggleClass('showAll');
            }
        }
    };

    var manageController = {

        isEditMode: function () {
            return isEditMode;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        _toolbarConfig: toolbarConfig,

        _manageInfo: manageInfo,

        getLink: function (settings) {
            settings = $.extend({}, toolbarConfig, settings);

            var params = {
                ctl: 'editcontentgroup',
                mid: settings.moduleId,
                returnUrl: settings.returnUrl
            };

            if (settings.cultureDimension && settings.cultureDimension != null)
                params.cultureDimension = settings.cultureDimension;

            if (settings.action == 'new')
                params.editMode = "New";

            if (!settings.useModuleList) {
                if (settings.action != 'new')
                    params.entityId = settings.entityId;
                if (settings.attributeSetName)
                    params.attributeSetName = settings.attributeSetName;
            } else {
                params.sortOrder = settings.sortOrder;
                params.contentGroupId = settings.contentGroupId;
            }

            if (settings.action == 'replace') {
                params.ctl = 'settingswrapper';
                params.ItemType = 'Content';
            }

            if (settings.prefill)
                params.prefill = JSON.stringify(settings.prefill);

            return settings.dialogUrl
                + (settings.dialogUrl.indexOf('?') == -1 ? '?' : '&')
                + $.param(params);
        },

        // Open a dialog within DNN as a lightbox or as a link, depending on DNN-config
        _openDialog: function (settings) {

            var link = manageController.getLink(settings);

            if (window.dnnModal && dnnModal.show) {
                link += (link.indexOf('?') == -1 ? '?' : '&') + "popUp=true";
                dnnModal.show(link, /*showReturn*/true, 550, 950, true, '');
            } else {
                window.location = link;
            }

        },

        // Perform a toolbar button-action - basically get the configuration and execute it's action
        action: function (event, settings) {
            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            var conf = actionButtonsConf[settings.action] || actionButtonsConf.default;
            manageController._getSelectorScope().saveTemplateId().then(function () {
                conf.action(origEvent, settings);
            });
        },

        // Generate a button (an <a>-tag) for one specific toolbar-action. 
        // Expects: settings, an object containing the specs for the expected buton
        getButton: function (settings) {
            // if the button belongs to a content-item, move the specs to the item into the settings-object
            if (settings.entity && settings.entity._2sxcEditInformation) {
                if (settings.entity._2sxcEditInformation.entityId) {
                    settings.entityId = settings.entity._2sxcEditInformation.entityId;
                }
                if (settings.entity._2sxcEditInformation.sortOrder != null) {
                    settings.sortOrder = settings.entity._2sxcEditInformation.sortOrder;
                    settings.useModuleList = true;
                }
                delete settings.entity;
            }

            // retrieve configuration for this button
            var conf = actionButtonsConf[settings.action] || actionButtonsConf.default;

            var button = $('<a />', {
                'class': 'sc-' + settings.action + ' '
                    + (settings.hideFirst || conf.hideFirst ? 'hideFirst' : '')
                    + ' ' + (conf.lightbox ? 'box' : ''),
                'onclick': 'javascript:$2sxc(' + id + ').manage.action(event, ' + JSON.stringify(settings) + ');',
                'title': conf.title
            });
            var symbol = $('<span class="glyphicon ' + conf.icon + '" aria-hidden="true"></span>');

            // if publish-button and not published yet, show button (otherwise hidden) & change icon
            if (settings.action == "publish" && settings.isPublished == false) {
                button.toggleClass('hideFirst', false)
                    .attr('title', "Unpublished");
                symbol.toggleClass(conf.icon, false)
                    .toggleClass(conf.icon2, true);
            }

            button.html(symbol);

            return button[0].outerHTML;
        },

        // Builds the toolbar and returns it as HTML
        // expects settings - either for 1 button or for an array of buttons
        getToolbar: function (settings) {
            var buttons = [];

            if (settings.action) {
                // if single item with specified action, use this as our button-list
                //settings = [settings];
                buttons = [settings];
            } else if ($.isArray(settings)) {
                // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                buttons = settings;
            } else {
                // Create a standard menu with all standard buttons
                // first button: edit
                buttons.push($.extend({}, settings, { action: 'edit' }));

                // add applicable list buttons - add=add item below; new=lightbox-dialog
                if (toolbarConfig.isList && settings.sortOrder != -1) {     // if list and not the list-header
                    buttons.push($.extend({}, settings, { action: 'new' }));
                    if (settings.useModuleList) {
                        buttons.push($.extend({}, settings, { action: 'add' }));
                        buttons.push($.extend({}, settings, { action: 'replace' }));
                        if (settings.sortOrder != 0)
                            buttons.push($.extend({}, settings, { action: 'moveup' }));
                        buttons.push($.extend({}, settings, { action: 'movedown' }));
                    }
                }
                buttons.push($.extend({}, settings, { action: 'publish' }));
                if (toolbarConfig.isList)
                    buttons.push($.extend({}, settings, { action: 'remove' })); // only provide remove on lists
                buttons.push($.extend({}, settings, { action: 'more' }));
            }

            var toolbar = $('<ul />', { 'class': 'sc-menu', 'onclick': 'javascript: var e = arguments[0] || window.event; e.stopPropagation();' });

            for (var i = 0; i < buttons.length; i++)
                toolbar.append($('<li />').append($(manageController.getButton(buttons[i]))));

            return toolbar[0].outerHTML;
        },

        // find all toolbar-info-attributes in the HTML, convert to <ul><li> toolbar
        _processToolbars: function () {
            $('.sc-menu[data-toolbar]', $(".DnnModule-" + id)).each(function () {
                var toolbarSettings = $.parseJSON($(this).attr('data-toolbar'));
                $(this).replaceWith($2sxc(id).manage.getToolbar(toolbarSettings));
            });
        },

        _getSelectorScope: function () {
            var selectorElement = document.querySelector('.DnnModule-' + id + ' .sc-selector-wrapper');
            return angular.element(selectorElement).scope();
        }

    };

    return manageController;
}