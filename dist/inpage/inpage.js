


// Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
var $2sxcActionMenuMapper = function (moduleId) {
    return {
        changeLayoutOrContent: function () {
            $2sxc(moduleId).manage.action({ "action": "layout" });
        },
        addItem: function () {
            $2sxc(moduleId).manage.action({ "action": "add", "useModuleList": true, "sortOrder": 0 });
        },
        edit: function () {
            $2sxc(moduleId).manage.action({ "action": "edit", "useModuleList": true, "sortOrder": 0 });
        },
        adminApp: function () {
            $2sxc(moduleId).manage.action({ "action": "app" });
        },
        adminZone: function () {
            $2sxc(moduleId).manage.action({ "action": "zone" });
        },
        develop: function () {
            $2sxc(moduleId).manage.action({ "action": "develop" });
        }
    };
};

// note: this is code which still uses jQuery etc., so it's not really clean
// because of this we're including it as simple code and not packaging it as a service quite yet...

function processInstallMessage(event, modId) {
    var regExToCheckOrigin = /^(http|https):\/\/((gettingstarted|[a-z]*)\.)?(2sexycontent|2sxc)\.org(\/.*)?$/gi;
    if (!regExToCheckOrigin.test(event.origin)) {
        console.error("can't execute, wrong source domain");
        return;
    }

    // Data is sent as text because IE8 and 9 cannot send objects through postMessage
    var data = JSON.parse(event.data);

    // If message does not belong to this module, return
    if (data.moduleId !== modId)
        return;

    if (data.action === "install") {
        var sf = $.ServicesFramework(modId);

        var packages = data.packages;
        var packagesDisplayNames = "";

        // Loop all packages to install
        for (var i = 0; i < packages.length; i++) {
            packagesDisplayNames += "- " + packages[i].displayName + "\n";
        }

        if (confirm("Do you want to install these packages?\n\n"
            + packagesDisplayNames + "\nThis could take 10 to 60 seconds per package, "
            + "please don't reload the page while it's installing. "
            + "You will see a message once it's done and progess is logged to the JS-console.")) {
            $(".DnnModule-" + modId + " #pnlLoading").show();
            var label = $(".DnnModule-" + modId + " #packageName");

            label.html("...");

            runOneInstallJob(packages, 0, sf, label);
        }

    }
    else if (data.action === "resize")
        resizeIFrame(modId, data.height);
}

function resizeIFrame(modId, height) {
    $(".DnnModule-" + modId + " #frGettingStarted").height(height);
}

function runOneInstallJob(packages, i, sf, label) {
    var currentPackage = packages[i];
    console.log(currentPackage.displayName + "(" + i + ") started");
    label.html(currentPackage.displayName);
    return $.ajax({
        type: "GET",
        dataType: "json",
        async: true,
        url: sf.getServiceRoot('2sxc') + "Installer/" + "InstallPackage",
        data: "packageUrl=" + currentPackage.url,
        beforeSend: sf.setModuleHeaders
    })
    .complete(function (jqXHR, textStatus) {
        console.log(currentPackage.displayName + "(" + i + ") completed");
        if (i + 1 < packages.length) {
            runOneInstallJob(packages, i + 1, sf, label);
        } else {
            alert("Done installing. If you saw no errors, everything worked.");
            window.location.reload();
        }
    })
    .error(function (xhr, result, status) {
        var errorMessage = "Something went wrong while installing '" + currentPackage.displayName + "': " + status;
        if (xhr.responseText && xhr.responseText !== "") {
            var response = $.parseJSON(xhr.responseText);
            if (response.messages)
                errorMessage = errorMessage + " - " + response.messages[0].Message;
            else if (response.Message)
                errorMessage = errorMessage + " - " + response.Message;
        }
        errorMessage += " (you might find more informations about the error in the DNN event log).";
        alert(errorMessage);
    });
}
angular.module('SxcInpageTemplates', []).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('template-selector/template-selector.html',
    "<div ng-cloak ng-show=vm.manageInfo.templateChooserVisible class=\"dnnFormMessage dnnFormInfo\"><div class=sc-selectors><select ng-show=!vm.manageInfo.isContentApp ng-model=vm.appId class=sc-selector-app ng-options=\"a.AppId as (a.Name.indexOf('TemplatePicker.') === 0 ? (a.Name | translate) : a.Name) for a in vm.apps\" ng-disabled=\"vm.manageInfo.hasContent || vm.manageInfo.isList\"><option value=\"\" ng-disabled=\"vm.appId != null\" translate=TemplatePicker.AppPickerDefault></option></select><select ng-show=vm.manageInfo.isContentApp ng-model=vm.contentTypeId class=sc-selector-contenttype ng-options=\"c.StaticName as c.Name for c in vm.contentTypes\" ng-disabled=\"vm.manageInfo.hasContent || vm.manageInfo.isList\"><option ng-disabled=\"vm.contentTypeId != ''\" value=\"\" translate=TemplatePicker.ContentTypePickerDefault></option></select><select ng-show=\"vm.manageInfo.isContentApp ? vm.contentTypeId != 0 : (vm.savedAppId != null &&  vm.filteredTemplates().length > 1)\" ng-model=vm.templateId class=sc-selector-template ng-options=\"t.TemplateId as t.Name for t in vm.filteredTemplates(vm.contentTypeId)\"></select></div><div class=sc-selector-actions>&nbsp; <a ng-show=\"vm.templateId != null && vm.savedTemplateId != vm.templateId\" class=sc-selector-save ng-click=\"vm.persistTemplate(false, false);\" title=\"{{ 'TemplatePicker.Save' | translate }}\"><div><i class=icon-sxc-ok></i></div></a> &nbsp; <a ng-show=\"vm.undoTemplateId != null\" class=sc-selector-close ng-click=vm.cancelTemplateChange(); title=\"{{ 'TemplatePicker.' + (vm.manageInfo.isContentApp ? 'Cancel' : 'Close') | translate }}\"><div><i class=icon-sxc-cancel></i></div></a></div><div class=sc-loading ng-show=vm.loading><i class=\"icon-sxc-spinner fa-spin\"></i></div><div style=\"position: relative\" ng-if=vm.showRemoteInstaller><iframe id=frGettingStarted ng-src={{vm.remoteInstallerUrl}} width=100% height=300px></iframe><div class=sc-loading id=pnlLoading style=display:none><i class=\"icon-sxc-spinner animate-spin\"></i><br><br><span class=sc-loading-label>installing <span id=packageName>.</span></span></div></div></div>"
  );

}]);

// A helper-controller in charge of opening edit-dialogs + creating the toolbars for it

// all in-page toolbars etc.
$2sxc.getManageController = function (id) {
    //#region helper functions
    function extend() { // same as angular.extend or jquery.extend, but without that additional dependency
        for (var i = 1; i < arguments.length; i++)
            for (var key in arguments[i])
                if (arguments[i].hasOwnProperty(key))
                    arguments[0][key] = arguments[i][key];
        return arguments[0];
    }
    //#endregion
    var moduleElement = $(".DnnModule-" + id);
    var manageInfo = $.parseJSON(moduleElement.find("div[data-2sxc]").attr("data-2sxc")).manage;
    var sxcGlobals = $.parseJSON(moduleElement.find("div[data-2sxc-globals]").attr("data-2sxc-globals"));
    manageInfo.ngDialogUrl = manageInfo.applicationRoot + "desktopmodules/tosic_sexycontent/dist/dnn/ui.html?sxcver="
        + manageInfo.config.version;


    manageInfo.ngDialogParams = {
        zoneId: manageInfo.zoneId,
        appId: manageInfo.appId,
        tid: manageInfo.config.tabId,
        mid: manageInfo.config.moduleId,
        lang: manageInfo.lang,
        langpri: manageInfo.langPrimary,
        langs: JSON.stringify(manageInfo.languages),
        portalroot: sxcGlobals.PortalRoot,
        websiteroot: manageInfo.applicationRoot,
        user: manageInfo.user,
        // note that the app-root doesn't exist when opening "manage-app"
        approot: (manageInfo.config && manageInfo.config.appPath) ? manageInfo.config.appPath : null // this is the only value which doesn't have a slash by default
    };

    var contentTypeName = manageInfo.contentTypeId; // note: only exists if the template has a content-type
    var toolbarConfig = manageInfo.config;
    toolbarConfig.contentType = toolbarConfig.contentType || toolbarConfig.attributeSetName;
    var enableTools = manageInfo.user.canDesign;
    var enableDevelop = manageInfo.user.canDevelop;
    var isContent = manageInfo.isContentApp;
    var isDebug = $2sxc.urlParams.get("debug") ? "&debug=true" : "";

    function buttonConfig(name, translateKey, icon, show,  uiOnly, more) {
        return extend({
            name: name,
            title: "Toolbar." + translateKey,
            iclass: "icon-sxc-" + icon,
            showOn: show,
            uiActionOnly: uiOnly
        }, more);
    }

    // minimal documentation regarding a button
    // the button can have the following properties / methods
    // - the indexer in the array (usually the same as the name)
    // - name (created in the buttonConfig)
    // - title - actually the translation key to retrieve the title (buttonConfig)
    // - iclass - the icon-class
    // - showOn - comma separated list of values on which toolbar state to show this on
    // - uiActionOnly - true/false if this is just something visual; otherwise a webservice will ensure that a content-group exists (for editing etc.)
    // - addCondition(settings) - would conditionally prevent adding this button by default
    // - code(settings, event) - the code executed on click, if it's not the default action
    // - dynamicClasses(settings) - can conditionally add more css-class names to add to the button, like the "empty" added if something doesn't have metadata
    // - params - ...
    // - 

    var actionButtonsConf = {
        'edit': buttonConfig("edit", "Edit", "pencil", "default", false, { params: { mode: "edit" } }),
        // new is a dialog to add something, and will not add if cancelled
        // new can also be used for mini-toolbars which just add an entity not attached to a module
        // in that case it's essential to add a contentType like 
        // <ul class="sc-menu" data-toolbar='{"action":"new", "contentType": "Category"}'></ul>
        'new': buttonConfig("new", "New", "plus", "default", false, { params: { mode: "new" },
            dialog: "edit", // don't use "new" (default) but use "edit"
            addCondition: function(settings) {
                return toolbarConfig.isList && settings.useModuleList && settings.sortOrder !== -1; // don't provide new on the header-item
            },
            code: function (settings, event) {
                tbContr._openNgDialog(extend({}, settings, { sortOrder: settings.sortOrder + 1 }), event);
            }
        }),
        // add brings no dialog, just add an empty item
        'add': buttonConfig("add", "AddDemo", "plus-circled", "edit", false, {
            addCondition: function (settings) { return toolbarConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
            code: function(settings, event) {
                tbContr._getAngularVm().addItem(settings.sortOrder + 1);
            }
        }),
        "metadata" : buttonConfig("metadata", "Metadata", "tag", "default", false, { params: { mode: "new" },
            dialog: "edit", // don't use "new" (default) but use "edit"
            dynamicClasses: function (settings) {
                // if it doesn't have data yet, make it less strong
                return settings.entityId ? "" : "empty";
                // return settings.items && settings.items[0].entityId ? "" : "empty";
            },
            addCondition: function (settings) {
                return !!settings.metadata; // only add a metadata-button if it has metadata-infos
                // return settings.items && (settings.items[0].metadata || settings.items[0].entityId); // only add a metadata-button if there is an items-list
            },
            configureCommand: function(cmd) {
                var itm = {
                    Title: "EditFormTitle.Metadata",
                    Metadata: extend({ keyType: "string", targetType: 10 }, cmd.settings.metadata)
                };
                extend(cmd.items[0], itm);
            }
        }),
        'remove': {
            title: "Toolbar.Remove",
            iclass: "icon-sxc-minus-circled",
            disabled: true,
            showOn: "edit",
            addCondition: function (settings) { return toolbarConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
            code: function(settings, event) {
                if (confirm(tbContr.translate("Toolbar.ConfirmRemove"))) {
                    tbContr._getAngularVm().removeFromList(settings.sortOrder);
                }
            }
        },

        // todo: work in progress related to https://github.com/2sic/2sxc/issues/618
        //'delete': {
        //    title: "Toolbar.Delete",
        //    iclass: "icon-sxc-cancel",
        //    disabled: true,
        //    showOn: "edit",
        //    addCondition: function (settings) { return !settings.useModuleList; },
        //    code: function (settings, event) {
        //        if (confirm(tbContr.translate("Toolbar.ReallyDelete"))) {
        //            tbContr._getAngularVm().reallyDelete(settings.entityId);
        //        }
        //    }
        //},

        'moveup': {
            title: "Toolbar.MoveUp",
            iclass: "icon-sxc-move-up",
            disabled: false,
            showOn: "edit",
            addCondition: function (settings) { return toolbarConfig.isList && settings.useModuleList && settings.sortOrder !== -1 && settings.sortOrder !== 0; },
            code: function(settings, event) {
                tbContr._getAngularVm().changeOrder(settings.sortOrder, Math.max(settings.sortOrder - 1, 0));
            }
        },
        'movedown': {
            title: "Toolbar.MoveDown",
            iclass: "icon-sxc-move-down",
            disabled: false,
            showOn: "edit",
            addCondition: function (settings) { return toolbarConfig.isList && settings.useModuleList && settings.sortOrder !== -1; },
            code: function(settings, event) {
                tbContr._getAngularVm().changeOrder(settings.sortOrder, settings.sortOrder + 1);
            }
        },
        'sort': {
            title: "Toolbar.Sort",
            iclass: "icon-sxc-list-numbered",
            showOn: "edit",
            addCondition: function (settings) { return toolbarConfig.isList && settings.useModuleList && settings.sortOrder !== -1; }
        },
        'publish': buttonConfig("publish", "Published", "eye", "edit", false, {
            iclass2: "icon-sxc-eye-off",
            disabled: true,
            code: function(settings, event) {
                if (settings.isPublished) {
                    alert(tbContr.translate("Toolbar.AlreadyPublished"));
                    return;
                }
                var part = settings.sortOrder === -1 ? "listcontent" : "content";
                var index = settings.sortOrder === -1 ? 0 : settings.sortOrder;
                tbContr._getAngularVm().publish(part, index);
            }
        }),
        'replace': buttonConfig("replace", "Replace", "replace", "edit", false, {
            addCondition: function(settings) { return settings.useModuleList; },
        }),
        'layout': {
            title: "Toolbar.ChangeLayout",
            iclass: "icon-sxc-glasses",
            showOn: "default",
            uiActionOnly: true, // so it doesn't create the content when used
            code: function(settings, event) {
                tbContr._getAngularVm().toggle();
            }
        },
        'develop': buttonConfig("develop", "Develop", "code", "admin", true, {
            newWindow: true,
            addCondition: enableTools,
            configureCommand: function(cmd) {
                cmd.items = [{ EntityId: manageInfo.templateId }];
            }
        }),
        'contenttype': {
            title: "Toolbar.ContentType",
            iclass: "icon-sxc-fields",
            showOn: "admin",
            uiActionOnly: true,
            addCondition: enableTools,
        },
        'contentitems': {
            title: "Toolbar.ContentItems",
            iclass: "icon-sxc-table",
            showOn: "admin",
            params: { contentTypeName: contentTypeName },
            uiActionOnly: true, // so it doesn't create the content when used
            addCondition: enableTools && contentTypeName,
        },
        'app': {
            title: "Toolbar.App",
            iclass: "icon-sxc-settings",
            showOn: "admin",
            uiActionOnly: true, // so it doesn't create the content when used
            addCondition: enableTools,
        },
        'zone': {
            title: "Toolbar.Zone",
            iclass: "icon-sxc-manage",
            showOn: "admin",
            uiActionOnly: true, // so it doesn't create the content when used
            addCondition: enableTools,
        },
        "more": {
            title: "Toolbar.MoreActions",
            iclass: "icon-sxc-options btn-mode",
            showOn: "default,edit,design,admin",
            uiActionOnly: true, // so it doesn't create the content when clicked
            code: function(settings, event) {
                var fullMenu = $(event.target).closest("ul.sc-menu");
                var oldState = Number(fullMenu.attr("data-state") || 0);
                var newState = oldState + 1;
                if (newState === 2) newState = 3; // state 1 doesn't exist yet - skip
                newState = newState % (enableTools ? 4 : 3); // if tools are enabled, there are 4 states
                fullMenu.removeClass("show-set-" + oldState)
                    .addClass("show-set-" + newState)
                    .attr("data-state", newState);
            }
        }
    };

    var tbContr = {
        isEditMode: function() {
            return manageInfo.isEditMode;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        _toolbarConfig: toolbarConfig,

        _manageInfo: manageInfo,

        // assemble an object which will store the configuration and execute it
        createCommandObject: function(specialSettings) {
            var settings = extend({}, toolbarConfig, specialSettings); // merge button with general toolbar-settings
            var cmd = {
                settings: settings,
                items: settings.items || [],                            // use predefined or create empty array
                params: extend({
                    dialog: settings.dialog || settings.action          // the variable used to name the dialog changed in the history of 2sxc from action to dialog
                }, settings.params),

                addSimpleItem: function() {
                    var itm = {}, ct = cmd.settings.contentType || cmd.settings.attributeSetName; // two ways to name the content-type-name this, v 7.2+ and older
                    if (cmd.settings.entityId) itm.EntityId = cmd.settings.entityId;
                    if (ct) itm.ContentTypeName = ct;
                    if (itm.EntityId || itm.ContentTypeName)    // only add if there was stuff to add
                        cmd.items.push(itm);
                },

                // this adds an item of the content-group, based on the group GUID and the sequence number
                addContentGroupItem: function(guid, index, part, isAdd, sectionLanguageKey) {
                    cmd.items.push({
                        Group: { Guid: guid, Index: index, Part: part, Add: isAdd },
                        Title: tbContr.translate(sectionLanguageKey)
                    });
                },

                // this will tell the command to edit a item from the sorted list in the group, optionally together with the presentation item
                addContentGroupItemSetsToEditList: function (withPresentation) {
                    var isContentAndNotHeader = (cmd.settings.sortOrder !== -1);
                    var index = isContentAndNotHeader ? cmd.settings.sortOrder : 0;
                    var prefix = isContentAndNotHeader ? "" : "List";
                    var cTerm = prefix + "Content";
                    var pTerm = prefix + "Presentation";
                    var isAdd = cmd.settings.action === "new";
                    var groupId = cmd.settings.contentGroupId;
                    cmd.addContentGroupItem(groupId, index, cTerm.toLowerCase(), isAdd, "EditFormTitle." + cTerm);

                    if (withPresentation) 
                        cmd.addContentGroupItem(groupId, index, pTerm.toLowerCase(), isAdd, "EditFormTitle." + pTerm);
                    
                },

                generateLink: function () {
                    // if there is no items-array, create an empty one (it's required later on)
                    if (!cmd.settings.items) cmd.settings.items = [];
                    //#region steps for all actions: prefill, serialize, open-dialog
                    // when doing new, there may be a prefill in the link to initialize the new item
                    if (cmd.settings.prefill)
                        for (var i = 0; i < cmd.items.length; i++)
                            cmd.items[i].Prefill = cmd.settings.prefill;

                    // Serialize/json-ify the complex items-list
                    // if (cmd.items.length)
                        cmd.params.items = JSON.stringify(cmd.items);

                    return manageInfo.ngDialogUrl
                        + "#" + $.param(manageInfo.ngDialogParams)
                        + "&" + $.param(cmd.params)
                        + isDebug;
                    //#endregion
                }
            };
            return cmd;
        },
        // create a dialog link
        getNgLink: function (specialSettings) {
            var cmd = tbContr.createCommandObject(specialSettings);

            if (cmd.settings.useModuleList)
                cmd.addContentGroupItemSetsToEditList(true);
            else
                cmd.addSimpleItem();            

            // if the command has own configuration stuff, do that now
            if (cmd.settings.configureCommand)
                cmd.settings.configureCommand(cmd);

            return cmd.generateLink();
        },

        // open a new dialog of the angular-ui
        _openNgDialog: function(settings, event, closeCallback) {
            
            var callback = function () {
                tbContr._getAngularVm().reload();
                closeCallback();
            };
            var link = tbContr.getNgLink(settings);

            if (settings.newWindow || (event && event.shiftKey))
                window.open(link);
            else
                $2sxc.totalPopup.open(link, callback);
        },

        // Perform a toolbar button-action - basically get the configuration and execute it's action
        action: function(settings, event) {
            var conf = actionButtonsConf[settings.action];
            settings = extend({}, conf, settings);              // merge conf & settings, but settings has higher priority
            if (!settings.dialog) settings.dialog = settings.action;    // old code uses "action" as the parameter, now use verb ? dialog
            if (!settings.code) settings.code = tbContr._openNgDialog;  // decide what action to perform

            var origEvent = event || window.event; // pre-save event because afterwards we have a promise, so the event-object changes; funky syntax is because of browser differences
            if (conf.uiActionOnly)
                return settings.code(settings, origEvent);
            
            // if more than just a UI-action, then it needs to be sure the content-group is created first
            tbContr._getAngularVm().prepareToAddContent().then(function() {
                return settings.code(settings, origEvent);
            });
        },

        // Generate a button (an <a>-tag) for one specific toolbar-action. 
        // Expects: settings, an object containing the specs for the expected buton
        getButton: function(btnSettings) {
            // if the button belongs to a content-item, move the specs up to the item into the settings-object
            if (btnSettings.entity && btnSettings.entity._2sxcEditInformation) {
                if (btnSettings.entity._2sxcEditInformation.entityId) {
                    btnSettings.entityId = btnSettings.entity._2sxcEditInformation.entityId;
                    btnSettings.useModuleList = false;
                }
                if (btnSettings.entity._2sxcEditInformation.sortOrder) {
                    btnSettings.sortOrder = btnSettings.entity._2sxcEditInformation.sortOrder;
                    btnSettings.useModuleList = true;
                }
                delete btnSettings.entity;
            }

            // retrieve configuration for this button
            var conf = actionButtonsConf[btnSettings.action];

            var showClasses = "";
            var classesList = conf.showOn.split(",");
            for (var c = 0; c < classesList.length; c++)
                showClasses += " show-" + classesList[c];
            var button = $("<a />", {
                'class': "sc-" + btnSettings.action + " " + showClasses + (conf.dynamicClasses ? " " + conf.dynamicClasses(btnSettings) : ""),
                'onclick': "javascript:$2sxc(" + id + ").manage.action(" + JSON.stringify(btnSettings) + ", event);",
                'title': tbContr.translate(conf.title)
            });
            var box = $("<div/>");
            var symbol = $("<i class=\"" + conf.iclass + "\" aria-hidden=\"true\"></i>");

            // todo: move the following lines into the button-config and just call from here
            // if publish-button and not published yet, show button (otherwise hidden) & change icon
            if (btnSettings.action === "publish" && btnSettings.isPublished === false) {
                button.addClass("show-default").removeClass("show-edit")
                    .attr("title", tbContr.translate("Toolbar.Unpublished")); 
                symbol.removeClass(conf.iclass).addClass(conf.iclass2);
            }

            button.html(box.html(symbol));

            return button[0].outerHTML;
        },

        // Builds the toolbar and returns it as HTML
        // expects settings - either for 1 button or for an array of buttons
        getToolbar: function(settings) {
            var buttons = settings.action
                ? [settings] // if single item with specified action, use this as our button-list
                : $.isArray(settings)
                ? settings // if it is an array, use that. Otherwise assume that we auto-generate all buttons with supplied settings
                : tbContr.createDefaultToolbar(settings);

            var tbClasses = "sc-menu show-set-0" + ((settings.sortOrder === -1) ? " listContent" : "");
            var toolbar = $("<ul />", { 'class': tbClasses, 'onclick': "javascript: var e = arguments[0] || window.event; e.stopPropagation();" });

            for (var i = 0; i < buttons.length; i++)
                toolbar.append($("<li />").append($(tbContr.getButton(buttons[i]))));

            return toolbar[0].outerHTML;
        },

        // Assemble a default toolbar instruction set
        createDefaultToolbar: function (settings) {
            // Create a standard menu with all standard buttons
            var buttons = [];

            buttons.add = function (verb) {
                var add = actionButtonsConf[verb].addCondition;
                if (add === undefined || ((typeof (add) === "function") ? add(settings) : add))
                    buttons.push(extend({}, settings, { action: verb }));
            };

            for (var btn in actionButtonsConf) 
                if (actionButtonsConf.hasOwnProperty(btn))
                    buttons.add(btn);

            return buttons;
        },

        // find all toolbar-info-attributes in the HTML, convert to <ul><li> toolbar
        _processToolbars: function() {
            $(".sc-menu[data-toolbar]", $(".DnnModule-" + id)).each(function() {
                var toolbarSettings = $.parseJSON($(this).attr("data-toolbar"));
                $(this).replaceWith($2sxc(id).manage.getToolbar(toolbarSettings));
            });
        },

        _getAngularVm: function() {
            var selectorElement = document.querySelector(".DnnModule-" + id + " .sc-selector-wrapper");
            return angular.element(selectorElement).scope().vm;
        },

        translate: function(key) {
            return tbContr._getAngularVm().translate(key);
        }

    };

    return tbContr;
};

// Toolbar bootstrapping (initialize all toolbars after loading page)
$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $(".sc-menu").click(function (e) {
        e.stopPropagation();
    });

    //var modules = $(".DnnModule-2sxc .Mod2sxcC[data-2sxc], .DnnModule-2sxc-app .Mod2sxcappC[data-2sxc]");
    var modules = $("div[data-2sxc]");

    // Ensure the _processToolbar is called after the next event cycle to make sure that the Angular app (template selector) is loaded first
    window.setTimeout(function () {
        modules.each(function () {
            try {
                var moduleId = $(this).data("2sxc").moduleId;
                $2sxc(moduleId).manage._processToolbars();
            } catch (e) { // Make sure that if one app breaks, others continue to work
                if (console && console.error)
                    console.error(e);
            }
        });
    }, 0);

    window.EavEditDialogs = [];
});

(function () {
    var module = angular.module("2sxc.view", [
        "2sxc4ng",
        "pascalprecht.translate",
        "SxcInpageTemplates",
        "EavConfiguration"
    ]);

    module.config(["$translateProvider", "AppInstanceId", "$translatePartialLoaderProvider", "languages", function ($translateProvider, AppInstanceId, $translatePartialLoaderProvider, languages) {
        
        var globals = $2sxc(AppInstanceId).manage._manageInfo;
        
        // add translation table
        $translateProvider
            .preferredLanguage(languages.currentLanguage.split("-")[0])
            .useSanitizeValueStrategy("escapeParameters")   // this is very important to allow html in the JSON files
            .fallbackLanguage(languages.fallbackLanguage)
            .useLoader("$translatePartialLoader", {
                urlTemplate: languages.i18nRoot + "{part}-{lang}.js"
            })
            .useLoaderCache(true);

        $translatePartialLoaderProvider.addPart("inpage");
    }]);


})();
(function () {
    var module = angular.module("2sxc.view");

    module.factory("moduleApiService", ["$http", function($http) {
        return {

            saveTemplate: function (templateId, forceCreateContentGroup, newTemplateChooserState) {
                return $http.get("View/Module/SaveTemplateId", { params: {
                    templateId: templateId,
                    forceCreateContentGroup: forceCreateContentGroup,
                    newTemplateChooserState: newTemplateChooserState
                } });
            },

            addItem: function(sortOrder) {
                return $http.get("View/Module/AddItem", { params: { sortOrder: sortOrder } });
            },

            getSelectableApps: function() {
                return $http.get("View/Module/GetSelectableApps");
            },

            setAppId: function(appId) {
                return $http.get("View/Module/SetAppId", { params: { appId: appId } });
            },

            getSelectableContentTypes: function() {
                return $http.get("View/Module/GetSelectableContentTypes");
            },

            getSelectableTemplates: function() {
                return $http.get("View/Module/GetSelectableTemplates");
            },

            setTemplateChooserState: function(state) {
                return $http.get("View/Module/SetTemplateChooserState", { params: { state: state } });
            },

            renderTemplate: function(templateId, lang) {
                return $http.get("View/Module/RenderTemplate", { params: { templateId: templateId, lang: lang } });
            },

            changeOrder: function(sortOrder, destinationSortOrder) {
                return $http.get("View/Module/ChangeOrder",
                { params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder } });
            },

            publish: function(part, sortOrder) {
                return $http.get("view/module/publish", { params: { part: part, sortOrder: sortOrder } });
            },

            removeFromList: function(sortOrder) {
                return $http.get("View/Module/RemoveFromList", { params: { sortOrder: sortOrder } });
            },

            gettingStartedUrl: function() {
                return $http.get("View/Module/RemoteInstallDialogUrl", { params: { dialog: "gettingstarted"} });
            }
        };
    }]);

})();
(function () {
    var module = angular.module("2sxc.view");

    module.controller("TemplateSelectorCtrl", ["$scope", "$attrs", "moduleApiService", "AppInstanceId", "sxc", "$filter", "$q", "$window", "$translate", "$sce", function ($scope, $attrs, moduleApiService, AppInstanceId, sxc, $filter, $q, $window, $translate, $sce) {
        //#region constants
        var cViewWithoutContent = "_LayoutElement"; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty

        //#endregion

        var realScope = $scope;
        var svc = moduleApiService;
        var importCommand = $attrs.importappdialog; // note: ugly dependency, should find a way to remove
        var viewPortSelector = ".DnnModule-" + AppInstanceId + " .sc-viewport";

        //#region vm and basic values / variables attached to vm
        var vm = this;
        vm.apps = [];
        vm.contentTypes = [];
        vm.templates = [];

        vm.manageInfo = sxc.manage._manageInfo;
        vm.templateId = vm.manageInfo.templateId;
        vm.undoTemplateId = vm.templateId;
        vm.contentTypeId = (vm.manageInfo.contentTypeId === "" && vm.manageInfo.templateId !== null)
            ? cViewWithoutContent           // has template but no content, use placeholder
            : vm.manageInfo.contentTypeId;
        vm.undoContentTypeId = vm.contentTypeId;

        vm.appId = vm.manageInfo.appId;
        vm.savedAppId = vm.manageInfo.appId;


        vm.showRemoteInstaller = false;
        vm.remoteInstallerUrl = "";

        vm.loading = 0;
        //#endregion


        vm.filteredTemplates = function (contentTypeId) {
            // Don't filter on App - so just return all
            if (!vm.manageInfo.isContentApp)
                return vm.templates;

            var condition = { ContentTypeStaticName: (contentTypeId === cViewWithoutContent) ? "" : contentTypeId };
            return $filter("filter")(vm.templates, condition, true);
        };


        vm.reloadTemplates = function() {

            vm.loading++;
            var getContentTypes = svc.getSelectableContentTypes();
            var getTemplates = svc.getSelectableTemplates();

            return $q.all([getContentTypes, getTemplates])
                .then(function(res) {
                    vm.contentTypes = res[0].data;
                    vm.templates = res[1].data;

                    // Add option for no content type if there are templates without
                    if ($filter("filter")(vm.templates, { ContentTypeStaticName: "" }, true).length > 0) {
                        vm.contentTypes.push({ StaticName: cViewWithoutContent, Name: $translate.instant("TemplatePicker.LayoutElement") }); 
                        vm.contentTypes = $filter("orderBy")(vm.contentTypes, "Name");
                    }

                    vm.loading--;
                });
        };

        realScope.$watch("vm.templateId", function (newTemplateId, oldTemplateId) {
            if (newTemplateId === oldTemplateId)
                return;

            // Content (ajax, don't save the changed value)
            if (vm.manageInfo.isContentApp)
                return vm.renderTemplate(newTemplateId);

            // App
            vm.loading++;
            vm.persistTemplate(false)
                .then(function() { $window.location.reload(); });
        });

        // Auto-set view-dropdown if content-type changed
        realScope.$watch("vm.contentTypeId", function (newContentTypeId, oldContentTypeId) {
        	if (newContentTypeId == oldContentTypeId)
        		return;
        	// Select first template if contentType changed
        	var firstTemplateId = vm.filteredTemplates(newContentTypeId)[0].TemplateId; 
        	if (vm.templateId !== firstTemplateId && firstTemplateId !== null)
        		vm.templateId = firstTemplateId;
        });

        // Save/reload on app-change or show import-window
        realScope.$watch("vm.appId", function (newAppId, oldAppId) {
            if (newAppId === oldAppId || newAppId === null)
                return;

            // special case: manage apps
            if (newAppId === -2) {
                alert('not implemented yet, because it would have to call the toolbar, which is bad architecture');
                return;
            }

            // special case: add app
            if (newAppId === -1) {
                window.location = importCommand; // actually does a dnnModal.show...
                return;
            }

            svc.setAppId(newAppId).then(function() {
                $window.location.reload();
            });
        });

        // Cancel and reset back to original state
        vm.cancelTemplateChange = function() {
            vm.templateId = vm.undoTemplateId;
            vm.contentTypeId = vm.undoContentTypeId;
            vm.manageInfo.templateChooserVisible = false;
            svc.setTemplateChooserState(false);
            if (vm.manageInfo.isContentApp) // necessary to show the original template again
                vm.reloadTemplates();
        };

        // store the template state to the server, optionally force create of content, and hide the selector
        vm.persistTemplate = function(forceCreate, selectorVisibility) {
            // Save only if the currently saved is not the same as the new
            var groupExistsAndTemplateUnchanged = !!vm.manageInfo.hasContent && (vm.undoTemplateId === vm.templateId);
            var promiseToSetState;
            if (groupExistsAndTemplateUnchanged)
                promiseToSetState = (vm.manageInfo.templateChooserVisible)
                    ? svc.setTemplateChooserState(false) // hide in case it was visible
                    : $q.when(null); // all is ok, create empty promise to allow chaining the result
            else
                promiseToSetState = svc.saveTemplate(vm.templateId, forceCreate, selectorVisibility)
                    .then(function(result) {
                        if (result.status !== 200) { // only continue if ok
                            alert("error - result not ok, was not able to create ContentGroup");
                            return;
                        }
                        var newGuid = result.data;
                        if (newGuid === null)
                            return;
                        newGuid = newGuid.replace(/[\",\']/g, ""); // fixes a special case where the guid is given with quotes (dependes on version of angularjs) issue #532
                        if (console)
                            console.log("created content group {" + newGuid + "}");
                        sxc.manage._manageInfo.config.contentGroupId = newGuid; // update internal ContentGroupGuid 
                    });
            
            var promiseToCorrectUi = promiseToSetState.then(function() {
                    vm.undoTemplateId = vm.templateId;          // remember for future undo
                    vm.undoContentTypeId = vm.contentTypeId;    // remember ...
                    vm.manageInfo.templateChooserVisible = false;
                    if(!vm.manageInfo.hasContent)               // if it didn't have content, then it only has now...
                        vm.manageInfo.hasContent = forceCreate; // ...if we forced it to
            });

            return promiseToCorrectUi;
        };

        vm.renderTemplate = function (templateId) {
            vm.loading++;
            svc.renderTemplate(templateId, vm.manageInfo.lang).then(function (response) {
                try {
                    $(viewPortSelector).html(response.data);
                    sxc.manage._processToolbars();
                } catch (e) {
                    console.log("Error while rendering template:");
                    console.log(e);
                }
                vm.loading--;
            });
        };

        // Optioally change the show state, then 
        // check if it should be shown and load/show
        vm.show = function(stateChange) {
            if (stateChange !== undefined)  // optionally change the show-state
                vm.manageInfo.templateChooserVisible = stateChange;

            if (vm.manageInfo.templateChooserVisible) {
                var promises = [];
                if (vm.appId !== null) // if an app had already been chosen OR the content-app (always chosen)
                    promises.push(vm.reloadTemplates()); 

                // if it's the app-dialog and the app's haven't been loaded yet...
                if (!vm.manageInfo.isContentApp && vm.apps.length === 0)
                    promises.push(vm.loadApps());
                $q.all(promises).then(vm.externalInstaller.showIfConfigIsEmpty);
            }
        };

        // some helpers to show the i-frame and link up the ablity to then install stuff
        vm.externalInstaller = {
            // based on situation, decide if we should show the auto-install IFrame
            showIfConfigIsEmpty: function () {
                var showAutoInstaller = (vm.manageInfo.isContentApp) 
                    ? vm.templates.length === 0 
                    : vm.apps.length <= 1;

                if (showAutoInstaller)
                    vm.externalInstaller.setup();
            },

            configureCallback: function setupCallback() {
                window.addEventListener("message", function forwardMessage(event) {
                    processInstallMessage(event, AppInstanceId); // this calls an external, non-angular method to handle resizing & installation...
                }, false);
            },

            setup: function() {
                svc.gettingStartedUrl().then(function(result) {
                    vm.externalInstaller.configureCallback();
                    vm.showRemoteInstaller = true;
                    vm.remoteInstallerUrl = $sce.trustAsResourceUrl(result.data);
                    console.log(result.data);
                });
            }
        };


        vm.toggle = function () {
            vm.manageInfo.someTest = "a value";
            if (vm.manageInfo.templateChooserVisible)
                vm.cancelTemplateChange();
            else {
                vm.show(true);
                svc.setTemplateChooserState(true);
            }
        };

        // reload by ajax or page, depeding on mode (used in toolbar)
        vm.reload = function () {
            if (!vm.templateId)
                return;

            if (vm.manageInfo.isContentApp)
                vm.renderTemplate(vm.templateId);
            else
                $window.location.reload();
        };

        vm.loadApps = function() {
            return svc.getSelectableApps()
                .then(function(data) {
                    vm.apps = data.data;
                    vm.apps.push({ Name: "TemplatePicker.GetMoreApps", AppId: -1 });
                });
        };

        //#region initialize this
        vm.activate = function() {
            vm.show(); // show if it has to, or not

            // Init App-Dropdown if it's an app-selector
            //if (!vm.manageInfo.isContentApp) 
            //    vm.loadApps().then(function() {
            //        vm.externalInstaller.showIfConfigIsEmpty();
            //    });
        };

        vm.activate();

        //#endregion


        //#region commands for the toolbar like add, remove, publish, translate, ..

        vm.prepareToAddContent = function () {
            return vm.persistTemplate(true, false);
        };

        vm.addItem = function(sortOrder) {
            svc.addItem(sortOrder).then(function () {
                vm.renderTemplate(vm.templateId);
            });
        };
        vm.removeFromList = function (sortOrder) {
        	svc.removeFromList(sortOrder).then(function () {
        		vm.renderTemplate(vm.templateId);
        	});
        };

        // todo: work in progress related to https://github.com/2sic/2sxc/issues/618
        vm.reallyDelete = function(itemId) {
            alert("Really delete not implemented yet - would delete: " + itemId);
        };

        vm.changeOrder = function (sortOrder, desintationSortOrder) {
        	svc.changeOrder(sortOrder, desintationSortOrder).then(function () {
        		vm.renderTemplate(vm.templateId);
        	});
        };
        vm.publish = function(part, sortOrder) {
            svc.publish(part, sortOrder).then(function() {
                vm.renderTemplate(vm.templateId);
            });
        };
        vm.translate = function (key) { return $translate.instant(key); };
        //#endregion
    }]);


})();