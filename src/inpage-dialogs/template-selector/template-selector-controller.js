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
    module.controller("TemplateSelectorCtrl", function ($scope, $interval, moduleApiService, AppInstanceId, sxc, $filter, $q, $window, $translate, $sce, contentBlockLink, $http) {
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
            // Don't filter on App - so just return all
            if (!vm.isContentApp)
                return vm.templates;

            var condition = { ContentTypeStaticName: (contentTypeId === cViewWithoutContent) ? "" : contentTypeId };
            return $filter("filter")(vm.templates, condition, true);
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
                    function findAndUnhide(filter) {
                        var found = vm.contentTypes.filter(filter);
                        if (found && found[0] && found[0].IsHidden) found[0].IsHidden = false;                        
                    }
                    findAndUnhide(function (item) { return item.StaticName === vm.contentTypeId; });
                    findAndUnhide(function(item) { return item.TemplateId === vm.templateId; });

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

        vm.manageApps = function() {    wrapper.sxc.manage.action({ "action": "zone" });    };
        vm.appSettings = function() {   wrapper.sxc.manage.action({ "action": "app" });     };
        vm.appImport = function() {   wrapper.sxc.manage.action({ "action": "app-import" });     };

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

    });


})();