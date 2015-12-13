(function () {
    var module = angular.module("2sxc.view");

    module.controller("TemplateSelectorCtrl", function($scope, $attrs, moduleApiService, AppInstanceId, sxc, $filter, $q, $window, $translate) {
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

            $q.all([getContentTypes, getTemplates])
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

        // 2015-12-13 2dm - doesn't seem used...
        //vm.reload = function () {
        //    if (!vm.templateId)
        //        return;
        //       
        //    if (vm.manageInfo.isContentApp)
        //        vm.renderTemplate(vm.templateId);
        //    else
        //        $window.location.reload();
        //};

        realScope.$watch("vm.templateId", function (newTemplateId, oldTemplateId) {
            if (newTemplateId === oldTemplateId)
                return;

            // Content (ajax, don't save the changed value)
            if (vm.manageInfo.isContentApp)
                return vm.renderTemplate(newTemplateId);

            // App
            vm.loading++;
            vm.saveTemplateIdButDontAddGroup(newTemplateId, false)
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

        // Show/hide this template chooser
        realScope.$watch("vm.manageInfo.templateChooserVisible", function(visible, oldVisible) {
            if (visible !== oldVisible && vm.appId !== null && visible)
                vm.reloadTemplates();
        });

        // Save/reload on app-change or show import-window
        realScope.$watch("vm.appId", function (newAppId, oldAppId) {
            if (newAppId === oldAppId || newAppId === null)
                return;

            if (newAppId === -1) {
                window.location = importCommand; // actually does a dnnModal.show...
                return;
            }

            svc.setAppId(newAppId).then(function() {
                $window.location.reload();
            });
        });


        vm.setTemplateChooserState = function (state) {
            // Reset templateid / cancel template change
            if (!state) {
                vm.templateId = vm.undoTemplateId;
                vm.contentTypeId = vm.undoContentTypeId;
            }

            return svc.setTemplateChooserState(state).then(function () {
                vm.manageInfo.templateChooserVisible = state;
            });
        };

        vm.saveTemplateIdButDontAddGroup = function ecgic(newTemplateId, close) {
            var promise = (vm.manageInfo.hasContent)
                ? vm.saveTemplateToContentGroup(newTemplateId)
                : vm.savePreviewTemplateId(newTemplateId);

            if (close)
                promise = promise.then(function() { vm.setTemplateChooserState(false); });
            return promise;
        };

        vm.saveTemplateToContentGroup = function (newTemplateId) {
            newTemplateId = newTemplateId || vm.templateId;
        	var promises = [];

            // todo: this condition could be part of the 500 problem?
			// Save only if the currently saved is not the same as the new
        	if (!vm.manageInfo.hasContent || vm.undoTemplateId !== newTemplateId) {
	            promises.push(svc.saveTemplateToContentGroup(newTemplateId)
	                .then(function(result) {
	                    // Make sure that ContentGroupGuid is updated accordingly
	                    var guid = result.data;
	                    sxc.manage._manageInfo.config.contentGroupId = guid;
	                }));
	            vm.undoTemplateId = newTemplateId;          // remember for future undo
	            vm.undoContentTypeId = vm.contentTypeId;    // remember ...
        		promises.push(vm.setTemplateChooserState(false));
	        }

            // todo: this doesn't enforce a sequence - maybe a problem?
            return $q.all(promises);
        };

	    vm.savePreviewTemplateId = svc.savePreviewTemplateId;

        vm.renderTemplate = function (templateId) {
            vm.loading++;
            svc.renderTemplate(templateId).then(function (response) {
                try {
                    $(viewPortSelector).html(response.data);
                    // vm.insertRenderedTemplate(response.data);
                    sxc.manage._processToolbars();
                } catch (e) {
                    console.log("Error while rendering template:");
                    console.log(e);
                }
                vm.loading--;
            });
        };

        // not used 2015-12-13
        //vm.insertRenderedTemplate = function(renderedTemplate) {
        //    $(viewPortSelector).html(renderedTemplate);
        //};


        //#region initialize this
        if (vm.appId !== null && vm.manageInfo.templateChooserVisible)
            vm.reloadTemplates();

        // Init App-Dropdown if it's an app-selector
        if (!vm.manageInfo.isContentApp) {
            svc.getSelectableApps()
                .then(function(data) {
                    vm.apps = data.data;
                    vm.apps.push({ Name: $translate.instant("TemplatePicker.GetMoreApps"), AppId: -1 });
                });
        }

        //#endregion


        //#region commands for the toolbar like add, remove, publish, translate, ..
		// ToDo: Remove this here, as it's not used in TemplateSelector - should move to 2sxc.api.manage.js
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