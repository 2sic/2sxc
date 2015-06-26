(function () {
    var module = angular.module('2sxc.view', ["2sxc.api", "2sxc4ng", "pascalprecht.translate"]);

    module.config(["$translateProvider", "HttpHeaders", "AppInstanceId", function ($translateProvider, HttpHeaders, AppInstanceId) {
        //alert(sxc);
        alert(AppInstanceId);
        var ngSxc = $2sxc(AppInstanceId);
        alert(ngSxc);
        // add translation table
        $translateProvider
          .preferredLanguage('en')
          .useSanitizeValueStrategy('escape')
          .fallbackLanguage('en')
          .useStaticFilesLoader({
              // todo: path...
              prefix: '/desktopmodules/tosic_sexycontent/i18n/inpage-',
              suffix: '.js'
          });
    }]);

    module.controller('TemplateSelectorCtrl', ["$scope", "$attrs", "moduleApiService", "$filter", "$q", "$window", function($scope, $attrs, moduleApiService, $filter, $q, $window) {
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
            return $filter('filter')(vm.templates, contentTypeId == "_LayoutElement" ? { ContentTypeStaticName: "" } : { ContentTypeStaticName: contentTypeId }, true);
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
                if ($filter('filter')(vm.templates, { ContentTypeStaticName: "" }, true).length > 0) {
                	vm.contentTypes.push({ StaticName: "_LayoutElement", Name: "Layout element" });
                    vm.contentTypes = $filter('orderBy')(vm.contentTypes, 'Name');
                }

                vm.loading--;
            });

        };

        realScope.$watch('vm.templateId', function (newTemplateId, oldTemplateId) {
        	if (newTemplateId != oldTemplateId) {
        		//alert("templateId changed");
        		if (vm.manageInfo.isContentApp)
        			vm.renderTemplate(newTemplateId);
        		else {
        			vm.loading++;
			        var promise;
			        if (vm.manageInfo.hasContent)
				        promise = vm.saveTemplateId(newTemplateId);
			        else
				        promise = vm.setPreviewTemplateId(newTemplateId);
			        promise.then(function() {
        				$window.location.reload();
			        });
        		}
        	}
        });

        realScope.$watch('vm.contentTypeId', function (newContentTypeId, oldContentTypeId) {
        	if (newContentTypeId == oldContentTypeId)
        		return;
        	// Select first template if contentType changed
        	var firstTemplateId = vm.filteredTemplates(newContentTypeId)[0].TemplateId; // $filter('filter')(vm.templates, { AttributeSetId: vm.contentTypeId == null ? "!!" : vm.contentTypeId })[0].TemplateID;
        	if (vm.templateId != firstTemplateId && firstTemplateId != null)
        		vm.templateId = firstTemplateId;
        });

        if (vm.appId != null && vm.manageInfo.templateChooserVisible)
            vm.reloadTemplates();

        realScope.$watch('vm.manageInfo.templateChooserVisible', function(visible, oldVisible) {
            if (visible != oldVisible && vm.appId != null && visible)
                vm.reloadTemplates();
        });

        realScope.$watch('vm.appId', function (newAppId, oldAppId) {
            if (newAppId == oldAppId || newAppId == null)
                return;

            if (newAppId == -1) {
                window.location = $attrs.importappdialog;
                return;
            }

            moduleApi.setAppId(newAppId).then(function() {
                $window.location.reload();
            });
        });

        if (!vm.manageInfo.isContentApp) {
            moduleApi.getSelectableApps().then(function(data) {
                vm.apps = data.data;
                vm.apps.push({ Name: $attrs.importapptext, AppId: -1 });
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

        vm.saveTemplateId = function () {
        	var promises = [];

			// Save only if the currently saved is not the same as the new
        	if (!vm.manageInfo.hasContent || vm.savedTemplateId != vm.templateId) {
        		promises.push(moduleApi.saveTemplateId(vm.templateId));
        		vm.savedTemplateId = vm.templateId;
        		promises.push(vm.setTemplateChooserState(false));
	        }

            return $q.all(promises);
        };

	    vm.setPreviewTemplateId = function() {
		    return moduleApi.setPreviewTemplateId(vm.templateId);
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

    }]);

    module.factory('moduleApiService', ["sxc", "$http", function(sxc, $http) {
        return function (moduleId) {
            function apiService(modId, settings) {
                return $http(settings);
            }
            return {
                saveTemplateId: function(templateId) {
                    return apiService(moduleId, {
                        url: 'View/Module/SaveTemplateId',
                        params: { templateId: templateId }
                    });
                },
            	setPreviewTemplateId: function(templateId) {
            		return apiService(moduleId, {
            			url: 'View/Module/SetPreviewTemplateId',
            			params: { templateId: templateId }
            		});
	            },
                addItem: function(sortOrder) {
                    return apiService(moduleId, {
                        url: 'View/Module/AddItem',
                        params: { sortOrder: sortOrder }
                    });
                },
                getSelectableApps: function() {
                    return apiService(moduleId, {
                        url: 'View/Module/GetSelectableApps'
                    });
                },
                setAppId: function(appId) {
                    return apiService(moduleId, {
                        url: 'View/Module/SetAppId',
                        params: { appId: appId }
                    });
                },
                getSelectableContentTypes: function () {
                    return apiService(moduleId, {
                        url: 'View/Module/GetSelectableContentTypes'
                    });
                },
                getSelectableTemplates: function() {
                    return apiService(moduleId, {
                        url: 'View/Module/GetSelectableTemplates'
                    });
                },
                setTemplateChooserState: function(state) {
                    return apiService(moduleId, {
                        url: 'View/Module/SetTemplateChooserState',
                        params: { state: state }
                    });
                },
                renderTemplate: function(templateId) {
                    return apiService(moduleId, {
                        url: 'View/Module/RenderTemplate',
                        params: { templateId: templateId }
                    });
                },
                changeOrder: function (sortOrder, destinationSortOrder) {
                	return apiService(moduleId, {
                		url: 'View/Module/ChangeOrder',
                		params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder }
                	});
                },
                removeFromList: function (sortOrder) {
                	return apiService(moduleId, {
                		url: 'View/Module/RemoveFromList',
                		params: { sortOrder: sortOrder }
                	});
                }
            };
        };
    }]);

})();