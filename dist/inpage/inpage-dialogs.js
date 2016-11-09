
// note: this is code which still uses jQuery etc., so it's not really clean
// because of this we're including it as simple code and not packaging it as a service quite yet...

function processInstallMessage(event, modId, progressIndicator, $http) {
    var regExToCheckOrigin = /^(http|https):\/\/((gettingstarted|[a-z]*)\.)?(2sexycontent|2sxc)\.org(\/.*)?$/gi;
    if (!regExToCheckOrigin.test(event.origin)) {
        console.error("can't execute, wrong source domain");
        return;
    }

    // Data is sent as text because IE8 and 9 cannot send objects through postMessage
    var data = JSON.parse(event.data);

    modId = Number(modId);
    // If message does not belong to this module, return
    if (data.moduleId !== modId)
        return;

    if (data.action === "install") {
        // var sf = $.ServicesFramework(modId);

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

            progressIndicator.show = true;
            progressIndicator.label = ".....";

            runOneInstallJob(packages, 0, progressIndicator, $http);
        }

    }
    else if (data.action === "resize")
        resizeIFrame(modId, data.height);
}

function resizeIFrame(modId, height) {
    document.getElementById("frGettingStarted").style.height = (height + 10) + "px";
}

function runOneInstallJob(packages, i, progressIndicator, $http) {
    var currentPackage = packages[i];
    console.log(currentPackage.displayName + "(" + i + ") started");
    progressIndicator.label = currentPackage.displayName;
    return $http.get("app/installer/installpackage",
        { params: { "packageUrl": currentPackage.url } })


    .then(function (response) {
        console.log(currentPackage.displayName + "(" + i + ") completed");
        if (i + 1 < packages.length) {
            runOneInstallJob(packages, i + 1, progressIndicator, $http);
        } else {
            alert("Done installing. If you saw no errors, everything worked.");
            window.top.location.reload();
        }
    }, function (xhr) {
        var errorMessage = "Something went wrong while installing '" + currentPackage.displayName + "': " + status;
        if (xhr.responseText && xhr.responseText !== "") {
            var response = JSON.parse(xhr.responseText);
            if (response.messages)
                errorMessage = errorMessage + " - " + response.messages[0].Message;
            else if (response.Message)
                errorMessage = errorMessage + " - " + response.Message;
        }
        errorMessage += " (you might find more informations about the error in the DNN event log).";
        alert(errorMessage);
    });
}
(function () {
    var module = angular.module("2sxc.view", [
            "2sxc4ng",
            "EavAdminUi", // dialog (modal) controller
            "pascalprecht.translate",
            "SxcInpageTemplates",
            "EavConfiguration",
            "ui.bootstrap"
        ])
        /*@ngInject*/
        .config(["$translatePartialLoaderProvider", function ($translatePartialLoaderProvider) {
            $translatePartialLoaderProvider.addPart("sxc-admin");
        }])
    ;

})();
(function () {
    var module = angular.module("2sxc.view");

    /*@ngInject*/
    module.factory("moduleApiService", ["$http", function ($http) {
        return {

            //saveTemplate: function (templateId, forceCreateContentGroup, newTemplateChooserState) {
            //    return $http.get("View/Module/SaveTemplateId", { params: {
            //        templateId: templateId,
            //        forceCreateContentGroup: forceCreateContentGroup,
            //        newTemplateChooserState: newTemplateChooserState
            //    } });
            //},

            //addItem: function(sortOrder) {
            //    return $http.get("View/Module/AddItem", { params: { sortOrder: sortOrder } });
            //},

            getSelectableApps: function() {
                return $http.get("View/Module/GetSelectableApps");
            },

            setAppId: function(appId) {
                return $http.get("View/Module/SetAppId", { params: { appId: appId } });
            },

            getSelectableContentTypes: function() {
                return $http.get("View/Module/GetSelectableContentTypes")
                    .then(function(result) {
                        if (result.data && result.data.length) {
                            angular.forEach(result.data, function(value, key) {
                                value.Label = (value.Metadata && value.Metadata.Label)
                                    ? value.Metadata.Label
                                    : value.Name;
                            });
                        }
                        return result;
                    });
            },

            getSelectableTemplates: function() {
                return $http.get("View/Module/GetSelectableTemplates");
            },

            //setTemplateChooserState: function(state) {
            //    return $http.get("View/Module/SetTemplateChooserState", { params: { state: state } });
            //},

            //renderTemplate: function(templateId, lang) {
            //    return $http.get("View/Module/RenderTemplate", { params: { templateId: templateId, lang: lang } });
            //},

            //changeOrder: function(sortOrder, destinationSortOrder) {
            //    return $http.get("View/Module/ChangeOrder",
            //    { params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder } });
            //},

            //publish: function(part, sortOrder) {
            //    return $http.get("view/module/publish", { params: { part: part, sortOrder: sortOrder } });
            //},

            //removeFromList: function(sortOrder) {
            //    return $http.get("View/Module/RemoveFromList", { params: { sortOrder: sortOrder } });
            //},

            gettingStartedUrl: function() {
                return $http.get("View/Module/RemoteInstallDialogUrl", { params: { dialog: "gettingstarted"} });
            }
        };
    }]);

})();
(function () {
    var module = angular.module("2sxc.view");

    /*@ngInject*/
    module.factory("contentBlockLink", function () {
        return function (vm) {
            // will generate an object necessary to communicate with the outer system
            var iframe = window.frameElement;
            iframe.vm = vm;

            return {
                dialogContainer: iframe,
                window: window.parent, 
                sxc: iframe.sxc,
                contentBlock: iframe.sxc.manage.contentBlock,
                // getManageInfo: iframe.getManageInfo,
                dashInfo: iframe.getAdditionalDashboardConfig
            };
        };
    });

    /*@ngInject*/
    module.controller("TemplateSelectorCtrl", ["$scope", "$interval", "moduleApiService", "AppInstanceId", "sxc", "$filter", "$q", "$window", "$translate", "$sce", "contentBlockLink", "$http", function ($scope, $interval, moduleApiService, AppInstanceId, sxc, $filter, $q, $window, $translate, $sce, contentBlockLink, $http) {
        //#region constants
        var cViewWithoutContent = "_LayoutElement"; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty
        var cAppActionManage = -2, cAppActionImport = -1, cAppActionCreate = -3;
        //#endregion

        var realScope = $scope;
        var svc = moduleApiService;

        //#region vm and basic values / variables attached to vm
        var vm = this;
        vm.apps = [];
        vm.contentTypes = [];
        vm.templates = [];
        var wrapper = contentBlockLink(vm);

        var di = vm.dashInfo = wrapper.dashInfo();
        vm.isContentApp = di.isContent;
        vm.supportsAjax = vm.isContentApp || di.supportsAjax;
        vm.showAdvanced = di.user.canDesign;
        vm.templateId = di.templateId;
        vm.undoTemplateId = di.templateId;
        vm.contentTypeId = di.contentTypeId;
        vm.undoContentTypeId = vm.contentTypeId;

        vm.appId = vm.dashInfo.appId !== 0 ? vm.dashInfo.appId : null;
        vm.savedAppId = vm.dashInfo.appId;


        vm.showRemoteInstaller = false;
        vm.remoteInstallerUrl = "";

        vm.loading = 0;
        vm.progressIndicator = {
            show: false,
            label: "..."
        };
        //#endregion

        //#region installer
        function enableProgressIndicator() {
            vm.progressIndicator.updater = $interval(function() {
                // don't do anything, this is just to ensure the digest happens
            }, 200);
        }
        //#endregion


        vm.filteredTemplates = function (contentTypeId) {
            if (vm.templates.length === 0)  // skip any filters if we don't have anything to go on yet
                return vm.templates;

            // filters for "normal" content - applies to everything
            // note that if a template is hidden by config but is curretly used here, the UI thinks it's not hidden, so the filter won't break anything
            var condition = { IsHidden: false };

            // 2016-11-09 disable don't filter on app, because even in this case there may already be data stored which is viewspecific
            // so it should only allow switching to views which have the same data-type
            // Don't filter on App - so just return all
            //if (!vm.isContentApp)
            //    return vm.templates;
            
            // add more conditions if in Content-Mode (which has a type-selector)
            if (vm.isContentApp)
                condition = angular.extend(condition, {
                    ContentTypeStaticName: (contentTypeId === cViewWithoutContent) ? "" : contentTypeId
                });
            var result = $filter("filter")(vm.templates, condition, true);
            return result;
        };


        vm.reloadTemplatesAndContentTypes = function() {

            vm.loading++;
            var getContentTypes = svc.getSelectableContentTypes();
            var getTemplates = svc.getSelectableTemplates();

            return $q.all([getContentTypes, getTemplates])
                .then(function (res) {
                    // map to view-model
                    vm.contentTypes = res[0].data || [];
                    vm.templates = res[1].data || [];

                    // if the currently selected content-type/template is configured to hidden, 
                    // re-show it, so that it can be used in the selectors
                    function unhideUsedContentType(filter) {
                        var found = vm.contentTypes.filter(filter);
                        if (found && found[0] && found[0].IsHidden) found[0].IsHidden = false;
                    }
                    unhideUsedContentType(function (item) { return item.StaticName === vm.contentTypeId; });
                    unhideUsedContentType(function(item) { return item.TemplateId === vm.templateId; });

                    // unhide the currently used template
                    var tmpl = $filter("filter")(vm.templates, { TemplateId: vm.templateId }, true);
                    if (tmpl && tmpl[0]) tmpl[0].IsHidden = false;

                    // Add option for no content type if there are templates without
                    if ($filter("filter")(vm.templates, { ContentTypeStaticName: "" }, true).length > 0) {
                        var le = { StaticName: cViewWithoutContent, Name: $translate.instant("TemplatePicker.LayoutElement"), IsHidden: false };
                        le.Label = le.Name;
                        vm.contentTypes.push(le);
                    }

                    // sort them now
                    vm.contentTypes = $filter("orderBy")(vm.contentTypes, "Name");

                    vm.loading--;
                });
        };

        realScope.$watch("vm.templateId", function (newTemplateId, oldTemplateId) {
            if (newTemplateId === oldTemplateId)
                return;

            // Content (ajax, don't save the changed value)
            if (vm.supportsAjax)
                return vm.renderTemplate(newTemplateId);

            // ToDo: Not sure why we need to set this value before calling persistTemplate. Clean up
            wrapper.contentBlock.templateId = vm.templateId;

            // App
            vm.persistTemplate(false)
                .then(function() {
                    return wrapper.window.location.reload(); //note: must be in a function, as the reload is a method of the location object
                });
        });

        // Auto-set view-dropdown if content-type changed
        realScope.$watch("vm.contentTypeId", function (newContentTypeId, oldContentTypeId) {
        	if (newContentTypeId === oldContentTypeId)
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

            // special case: add app
            if (newAppId === cAppActionImport) {
                return vm.appImport();
            }

            // find new app specs
            var newApp = $filter('filter')(vm.apps, { AppId: newAppId })[0];

            svc.setAppId(newAppId)
                .then(function() {
                    if (newApp.SupportsAjaxReload) {
                        vm.reInitAll(true); // special code to force app-change/reload
                    } else
                        wrapper.window.location.reload();
                });
        });

        vm.manageApps = function() {    wrapper.sxc.manage.run("zone");    };
        vm.appSettings = function() {   wrapper.sxc.manage.run("app");     };
        vm.appImport = function() {   wrapper.sxc.manage.run("app-import");     };

        // Cancel and reset back to original state
        vm.cancelTemplateChange = wrapper.contentBlock._cancelTemplateChange;

        // store the template state to the server, optionally force create of content, and hide the selector
        vm.persistTemplate = wrapper.contentBlock.persistTemplate;
        vm.renderTemplate = wrapper.contentBlock.reload;  // just map to that method
        vm.reInitAll = wrapper.contentBlock.reloadAndReInitialize;  // just map to that method

        vm.appStore = function() {
            window.open("http://2sxc.org/en/apps");
        };

        // Optionally change the show state, then 
        // check if it should be shown and load/show
        vm.show = function (stateChange) {
            // todo 8.4 disabled this, as this info should never be set from here again...
            if (stateChange !== undefined)  // optionally change the show-state
                vm.dashInfo.templateChooserVisible = stateChange;

            if (vm.dashInfo.templateChooserVisible) {
                var promises = [];
                if (vm.appId !== null) // if an app had already been chosen OR the content-app (always chosen)
                    promises.push(vm.reloadTemplatesAndContentTypes()); 

                // if it's the app-dialog and the app's haven't been loaded yet...
                if (!vm.isContentApp && vm.apps.length === 0)
                    promises.push(vm.loadApps());
                $q.all(promises).then(vm.externalInstaller.showIfConfigIsEmpty);
            }
        };

        //#region some *Installer* helpers to show the i-frame and link up the ablity to then install stuff
        vm.externalInstaller = {
            // based on situation, decide if we should show the auto-install IFrame
            showIfConfigIsEmpty: function () {
                var showAutoInstaller = (vm.isContentApp) 
                    ? vm.templates.length === 0 
                    : vm.appCount === 0;

                if (showAutoInstaller)
                    vm.externalInstaller.setup();
            },

            configureCallback: function setupCallback() {
                window.addEventListener("message", function forwardMessage(event) {
                    processInstallMessage(event, AppInstanceId, vm.progressIndicator, $http); // this calls an external, non-angular method to handle resizing & installation...
                }, false);
            },

            setup: function() {
                svc.gettingStartedUrl().then(function (result) {
                    if (result.data) {  // only show getting started if it's really still a blank system, otherwise the server will return null, then don't do anything
                        vm.externalInstaller.configureCallback();
                        vm.showRemoteInstaller = true;
                        enableProgressIndicator();
                        vm.remoteInstallerUrl = $sce.trustAsResourceUrl(result.data);
                        console.log(result.data);
                    }
                });
            }
        };
        //#endregion

        // todo 8.4 - this should re-load state if re-shown
        vm.toggle = function () {
            if (vm.dashInfo.templateChooserVisible)
                vm.cancelTemplateChange();
            else {
                vm.show(true);
            }
        };

        vm.loadApps = function() {
            return svc.getSelectableApps()
                .then(function(data) {
                    vm.apps = data.data;
                    vm.appCount = data.data.length; // needed in the future to check if it shows getting started

                    if (vm.showAdvanced) {
                        vm.apps.push({ Name: "TemplatePicker.Install", AppId: cAppActionImport });
                        //vm.apps.push({ Name: "create your own app...", AppId: cAppActionCreate }); // todo: i18n
                        //vm.apps.push({ Name: "manage apps...", AppId: cAppActionManage }); // todo: i18n
                    }
                });
        };

        //#region initialize this

        vm.activate = function() {
            vm.show(true); 
        };

        vm.activate();

        //#endregion

    }]);


})();
angular.module("SxcInpageTemplates", []).run(["$templateCache", function($templateCache) {$templateCache.put("template-selector/template-selector.html","\r\n\r\n<div class=\"sc-selectors-wrapper\">\r\n    <div class=\"sc-selectors\">\r\n        <!-- App Selector - only relevant in App-Mode -->\r\n        <div ng-show=\"!vm.isContentApp\" style=\"overflow:hidden;\">\r\n            <select ng-model=\"vm.appId\" class=\"sc-selector-app input-lg pull-left\"\r\n                    ng-options=\"a.AppId as (a.Name.indexOf(\'TemplatePicker.\') === 0 ? \'[+] \' + (a.Name | translate) : a.Name) for a in vm.apps\"\r\n                    ng-disabled=\"vm.dashInfo.hasContent\">\r\n                <option value=\"\" ng-disabled=\"vm.appId != null\" translate=\"TemplatePicker.AppPickerDefault\"></option>\r\n            </select>\r\n            <span>\r\n                    <span ng-if=\"vm.showAdvanced && !vm.isContentApp\">\r\n                        <button type=\"button\" class=\"btn btn-default\"\r\n                                ng-show=\"vm.appId != null\"\r\n                                ng-click=\"vm.appSettings();\"\r\n                                title=\"{{ \'TemplatePicker.App\' | translate }}\">\r\n                            <i class=\"icon-eav-settings\"></i>\r\n                        </button>\r\n                        <button type=\"button\" class=\"btn btn-default\"\r\n                                ng-click=\"vm.appImport();\"\r\n                                title=\"{{ \'TemplatePicker.Install\' | translate }}\">\r\n                            <i class=\"icon-eav-plus\"></i>\r\n                        </button>\r\n                        <button type=\"button\" class=\"btn btn-default\"\r\n                                ng-click=\"vm.appStore();\"\r\n                                title=\"{{ \'TemplatePicker.Catalog\' | translate }}\">\r\n                            <i class=\"icon-eav-cart-arrow-down\"></i>\r\n                        </button>\r\n                        <button type=\"button\" class=\"btn btn-default\"\r\n                                ng-click=\"vm.manageApps();\"\r\n                                title=\"{{ \'TemplatePicker.Zone\' | translate }}\">\r\n                            <i class=\"icon-eav-manage\"></i>\r\n                        </button>\r\n\r\n                    </span>\r\n                </span>\r\n\r\n        </div>\r\n\r\n\r\n        <!-- Content Type selector, only for Content-Mode -->\r\n        <select ng-show=\"vm.isContentApp\" ng-model=\"vm.contentTypeId\"\r\n                class=\"input-lg\"\r\n                ng-options=\"c.StaticName as c.Label for c in vm.contentTypes | filter: { IsHidden : false } | orderBy: \'Label\'\"\r\n                ng-disabled=\"vm.dashInfo.hasContent || vm.dashInfo.isList\">\r\n            <option ng-disabled=\"vm.contentTypeId != \'\'\" value=\"\" translate=\"TemplatePicker.ContentTypePickerDefault\"></option>\r\n        </select>\r\n\r\n        <!-- View / template selector -->\r\n        <div>\r\n            <select ng-show=\"vm.isContentApp ? vm.contentTypeId != 0 : (vm.savedAppId != null)\" \r\n                    x=\"( && vm.filteredTemplates().length > 1)\"\r\n                    ng-disabled=\"vm.templateId && vm.filteredTemplates().length <= 1\"\r\n                    class=\"input-lg pull-left\"\r\n                    ng-model=\"vm.templateId\"\r\n                    ng-options=\"t.TemplateId as t.Name for t in vm.filteredTemplates(vm.contentTypeId)\"></select>\r\n\r\n            <button ng-show=\"vm.templateId != null && vm.savedTemplateId != vm.templateId\"\r\n                    class=\"btn btn-primary\"\r\n                    ng-click=\"vm.persistTemplate(false, false);\"\r\n                    title=\"{{ \'TemplatePicker.Save\' | translate }}\"\r\n                    type=\"button\">\r\n                <i class=\"icon-eav-ok\"></i>\r\n            </button>\r\n            <button ng-show=\"vm.undoTemplateId != null\"\r\n                    class=\"btn btn-default\"\r\n                    ng-click=\"vm.cancelTemplateChange();\"\r\n                    type=\"button\"\r\n                    title=\"{{ \'TemplatePicker.\' + (vm.isContentApp ? \'Cancel\' : \'Close\') | translate }}\">\r\n                <i class=\"icon-eav-cancel\"></i>\r\n            </button>\r\n        </div>\r\n    </div>\r\n\r\n\r\n\r\n    <div class=\"sc-loading\" ng-show=\"vm.loading\">\r\n        <i class=\"icon-eav-spinner animate-spin\"></i>\r\n    </div>\r\n\r\n    <!-- the auto-installer IFrame, with spinner and everything -->\r\n    <div style=\"position: relative;\" ng-if=\"vm.showRemoteInstaller\">\r\n        <iframe id=\"frGettingStarted\" ng-src=\"{{vm.remoteInstallerUrl}}\" width=\"100%\" height=\"300px\"></iframe>\r\n        <div class=\"sc-loading\" id=\"pnlLoading\" ng-if=\"vm.progressIndicator.show\">\r\n            <i class=\"icon-eav-spinner animate-spin\"></i>\r\n            <br/>\r\n            <br/>\r\n            <span class=\"sc-loading-label\">\r\n                    installing <span id=\"packageName\">{{vm.progressIndicator.label}}</span>\r\n                </span>\r\n        </div>\r\n    </div>\r\n</div>\r\n");}]);