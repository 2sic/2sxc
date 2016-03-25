(function () {
    var module = angular.module("2sxc.view");

    module.factory("inpageElement", function () {
        return function () {
            // will generate an object necessary to communicate with the outer system
            var iframe = window.frameElement;
            var contentBlock = null;
            var instanceBlock = null;
            return {
                dialogContainer: iframe,
                getManageInfo: iframe ? iframe.getManageInfo : null,
                //instanceContext: instanceBlock,
                //contentBlock: contentBlock
            };
        };
    });

    module.controller("TemplateSelectorCtrl", function ($scope, /* $attrs, */ moduleApiService, AppInstanceId, sxc, $filter, $q, $window, $translate, $sce, inpageElement) {
        //#region constants
        var cViewWithoutContent = "_LayoutElement"; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty
        var cAppActionManage = -2, cAppActionImport = -1, cAppActionCreate = -3;
        var viewPortSelector = ".DnnModule-" + AppInstanceId + " .sc-viewport";
        //#endregion

        var realScope = $scope;
        var svc = moduleApiService;
        var importCommand = null; // todo! // $attrs.importappdialog; // note: ugly dependency, should find a way to remove

        //#region vm and basic values / variables attached to vm
        var vm = this;
        vm.apps = [];
        vm.contentTypes = [];
        vm.templates = [];
        var inpagePartner = inpageElement();

        // the sxc.manage is just to keep the old version running for now
        vm.manageInfo = sxc.manage ? sxc.manage._manageInfo : inpagePartner.getManageInfo();
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


            // special case: add app
            if (newAppId === cAppActionImport) {
                window.location = importCommand; // actually does a dnnModal.show...
                return;
            }

            svc.setAppId(newAppId).then(function() {
                $window.location.reload();
            });
        });

        vm.manageApps = function() {
            sxc.manage.action({ "action": "zone" });
        };
        vm.appSettings = function() {
            sxc.manage.action({ "action": "app" });
        };
        vm.installApp = function() {
            window.location = importCommand; // actually does a dnnModal.show...
            return;
        };

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
                    vm.appCount = data.data.length; // needed in the future to check if it shows getting started
                    //if (vm.manageInfo.user.canDesign) {
                    //    vm.apps.push({ Name: "TemplatePicker.GetMoreApps", AppId: cAppActionImport });
                    //}
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
    });


})();