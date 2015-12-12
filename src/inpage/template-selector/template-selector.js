(function () {
    var module = angular.module("2sxc.view");

    module.controller("TemplateSelectorCtrl", function($scope, $attrs, moduleApiService, $filter, $q, $window, $translate) {
        var vm = this;
        var realScope = $scope;

        var moduleId = $attrs.moduleid;
        var moduleApi = moduleApiService(moduleId);

        vm.manageInfo = $2sxc(moduleId).manage._manageInfo;
        vm.apps = [];
        vm.contentTypes = [];
        vm.templates = [];
        vm.filteredTemplates = function (contentTypeId) {
            // Return all templates for App
            if (!vm.manageInfo.isContentApp)
                return vm.templates;
            return $filter("filter")(vm.templates, contentTypeId == "_LayoutElement" ? { ContentTypeStaticName: "" } : { ContentTypeStaticName: contentTypeId }, true);
        };
        vm.contentTypeId = vm.manageInfo.contentTypeId;
        vm.templateId = vm.manageInfo.templateId;
        vm.savedTemplateId = vm.manageInfo.templateId;
        vm.appId = vm.manageInfo.appId;
        vm.savedAppId = vm.manageInfo.appId;
        vm.loading = 0;

        vm.reloadTemplates = function() {

            vm.loading++;
            var getContentTypes = moduleApi.getSelectableContentTypes();
            var getTemplates = moduleApi.getSelectableTemplates();

            $q.all([getContentTypes, getTemplates]).then(function (res) {
                vm.contentTypes = res[0].data;
                vm.templates = res[1].data;

                // Add option for no content type if there are templates without
                if ($filter("filter")(vm.templates, { ContentTypeStaticName: "" }, true).length > 0) {
                	vm.contentTypes.push({ StaticName: "_LayoutElement", Name: "Layout element" });
                    vm.contentTypes = $filter("orderBy")(vm.contentTypes, "Name");
                }

                vm.loading--;
            });

        };

        vm.reload = function () {
            if (!vm.templateId)
                return;
            
            if (vm.manageInfo.isContentApp)
                vm.renderTemplate(vm.templateId);
            else
                $window.location.reload();
        };

        if (vm.appId !== null && vm.manageInfo.templateChooserVisible)
            vm.reloadTemplates();

        // todo: 2dm working on this
        realScope.$watch("vm.templateId", function (newTemplateId, oldTemplateId) {
        	if (newTemplateId !== oldTemplateId) {
        		//alert("templateId changed");
        		if (vm.manageInfo.isContentApp)
        			vm.renderTemplate(newTemplateId);
        		else {
        			vm.loading++;
        			//var promise = (vm.manageInfo.hasContent)
                    //    ? vm.saveTemplateId(newTemplateId)
                    //    : vm.setPreviewTemplateId(newTemplateId);

        			vm.saveTemplateIdButDontAddGroup.then(function () {
        				$window.location.reload();
			        });
        		}
        	}
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

        // Save/reload on app-change
        realScope.$watch("vm.appId", function (newAppId, oldAppId) {
            if (newAppId === oldAppId || newAppId === null)
                return;

            if (newAppId == -1) {
                window.location = $attrs.importappdialog;
                return;
            }

            moduleApi.setAppId(newAppId).then(function() {
                $window.location.reload();
            });
        });

        // Init App-Dropdown if it's an app-selector
        if (!vm.manageInfo.isContentApp) {
            moduleApi.getSelectableApps().then(function(data) {
                vm.apps = data.data;
                vm.apps.push({ Name: $translate.instant('TemplatePicker.GetMoreApps'), AppId: -1 });
            });
        }

        vm.setTemplateChooserState = function (state) {
            // Reset templateid / cancel template change
            if (!state)
                vm.templateId = vm.savedTemplateId;

            return moduleApi.setTemplateChooserState(state).then(function () {
                vm.manageInfo.templateChooserVisible = state;
            });
        };

        vm.saveTemplateIdButDontAddGroup = function ecgic(newTemplateId) {
            var promise = (vm.manageInfo.hasContent)
                ? vm.saveTemplateId(newTemplateId)
                : vm.setPreviewTemplateId(newTemplateId);
            return promise;
        };

        vm.saveTemplateId = function (newTemplateId) {
            newTemplateId = newTemplateId || vm.templateId;
        	var promises = [];

            // todo: this condition could be part of the 500 problem?
			// Save only if the currently saved is not the same as the new
        	if (!vm.manageInfo.hasContent || vm.savedTemplateId !== newTemplateId) {
	            promises.push(moduleApi.saveTemplateId(newTemplateId)
	                .then(function(result) {
	                    // Make sure that ContentGroupGuid is updated accordingly
	                    var guid = result.data;
	                    $2sxc(moduleId).manage._manageInfo.config.contentGroupId = guid;
	                }));
        		vm.savedTemplateId = newTemplateId;
        		promises.push(vm.setTemplateChooserState(false));
	        }

            // todo: this doesn't enforce a sequence - maybe a problem?
            return $q.all(promises);
        };

	    vm.setPreviewTemplateId = function(newTemplateId) {
	        return moduleApi.setPreviewTemplateId(newTemplateId);
	    };

        vm.renderTemplate = function (templateId) {
            vm.loading++;
            moduleApi.renderTemplate(templateId).then(function (response) {
                try {
                    vm.insertRenderedTemplate(response.data);
                    $2sxc(moduleId).manage._processToolbars();
                } catch (e) {
                    console.log("Error while rendering template:");
                    console.log(e);
                }
                vm.loading--;
            });
        };

        vm.insertRenderedTemplate = function(renderedTemplate) {
            $(".DnnModule-" + moduleId + " .sc-viewport").html(renderedTemplate);
        };

		// ToDo: Remove this here, as it's not used in TemplateSelector - should move to 2sxc.api.manage.js
        vm.addItem = function(sortOrder) {
            moduleApi.addItem(sortOrder).then(function () {
                vm.renderTemplate(vm.templateId);
            });
        };
        vm.removeFromList = function (sortOrder) {
        	moduleApi.removeFromList(sortOrder).then(function () {
        		vm.renderTemplate(vm.templateId);
        	});
        };
        vm.changeOrder = function (sortOrder, desintationSortOrder) {
        	moduleApi.changeOrder(sortOrder, desintationSortOrder).then(function () {
        		vm.renderTemplate(vm.templateId);
        	});
        };
        vm.publish = function(part, sortOrder) {
            moduleApi.publish(part, sortOrder).then(function() {
                vm.renderTemplate(vm.templateId);
            });
        };
        vm.translate = function (key) { return $translate.instant(key); };

    });


})();