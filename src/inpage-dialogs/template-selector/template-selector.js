(function () {
    var module = angular.module("2sxc.view");

    module.factory("contentBlockLink", function () {
        return function (vm) {
            // will generate an object necessary to communicate with the outer system
            var iframe = window.frameElement;
            iframe.vm = vm;

            return {
                dialogContainer: iframe,
                window: window.parent, //iframe.parent,
                sxc: iframe.sxc,
                contentBlock: iframe.sxc.manage.rootCB,
                getManageInfo: iframe.getManageInfo,
                hide: iframe.justHide
            };
        };
    });

    module.controller("TemplateSelectorCtrl", function ($scope, /* $attrs, */ moduleApiService, AppInstanceId, sxc, $filter, $q, $window, $translate, $sce, contentBlockLink) {
        //#region constants
        var cViewWithoutContent = "_LayoutElement"; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty
        //#endregion

        var realScope = $scope;
        var svc = moduleApiService;
        var importCommand = null; // todo! // $attrs.importappdialog; // note: ugly dependency, should find a way to remove

        //#region vm and basic values / variables attached to vm
        var vm = this;
        vm.apps = [];
        vm.contentTypes = [];
        vm.templates = [];
        var wrapper = contentBlockLink(vm);

        // the sxc.manage is just to keep the old version running for now
        vm.manageInfo = wrapper.getManageInfo();
        vm.showAdvanced = vm.manageInfo.user.canDesign;
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
            vm.persistTemplate(false)
                .then(wrapper.window.location.reload); 
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

            svc.setAppId(newAppId)
                .then(wrapper.window.location.reload);
        });

        vm.manageApps = function() {    wrapper.sxc.manage.action({ "action": "zone" });    };
        vm.appSettings = function() {   wrapper.sxc.manage.action({ "action": "app" });     };

        // this is the one we must refactor out, as the import-command doesn't exist here
        vm.installApp = function () {
            wrapper.window.location = importCommand; // actually does a dnnModal.show...
            return;
        };

        // todo: move to content-block
        // Cancel and reset back to original state
        vm.cancelTemplateChange = function() {
            vm.templateId = vm.undoTemplateId;
            vm.contentTypeId = vm.undoContentTypeId;
            vm.manageInfo.templateChooserVisible = false;
            wrapper.hide();
            wrapper.contentBlock.setTemplateChooserState(false);
            if (vm.manageInfo.isContentApp) // necessary to show the original template again
                vm.reloadTemplates();
        };

        // store the template state to the server, optionally force create of content, and hide the selector
        vm.persistTemplate = wrapper.contentBlock.persistTemplate;
        vm.renderTemplate = wrapper.contentBlock.reload;  // just map to that method


        // Optionally change the show state, then 
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
                    : vm.appCount === 0;

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
                wrapper.contentBlock.setTemplateChooserState(true);
            }
        };

        vm.loadApps = function() {
            return svc.getSelectableApps()
                .then(function(data) {
                    vm.apps = data.data;
                    vm.appCount = data.data.length; // needed in the future to check if it shows getting started
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