(function () {
    var module = angular.module('2sxc.view', ["2sxc.api", "pascalprecht.translate"]);

    module.config(['$translateProvider', function($translateProvider) {
 
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

    module.controller('TemplateSelectorCtrl', function($scope, $attrs, moduleApiService, $filter, $q, $window) {

        var moduleId = $attrs.moduleid;
        var moduleApi = moduleApiService(moduleId);

        $scope.manageInfo = $2sxc(moduleId).manage._manageInfo;
        $scope.apps = [];
        $scope.contentTypes = [];
        $scope.templates = [];
        $scope.filteredTemplates = function (contentTypeId) {
            // Return all templates for App
            if (!$scope.manageInfo.isContentApp)
                return $scope.templates;
            return $filter('filter')($scope.templates, contentTypeId == "_LayoutElement" ? { ContentTypeStaticName: "" } : { ContentTypeStaticName: contentTypeId }, true);
        };
        $scope.contentTypeId = $scope.manageInfo.contentTypeId;
        $scope.templateId = $scope.manageInfo.templateId;
        $scope.savedTemplateId = $scope.manageInfo.templateId;
        $scope.appId = $scope.manageInfo.appId;
        $scope.savedAppId = $scope.manageInfo.appId;
        $scope.loading = 0;

        $scope.reloadTemplates = function() {

            $scope.loading++;
            var getContentTypes = moduleApi.getSelectableContentTypes();
            var getTemplates = moduleApi.getSelectableTemplates();

            $q.all([getContentTypes, getTemplates]).then(function (res) {
                $scope.contentTypes = res[0].data;
                $scope.templates = res[1].data;

                // Add option for no content type if there are templates without
                if ($filter('filter')($scope.templates, { ContentTypeStaticName: "" }, true).length > 0) {
                	$scope.contentTypes.push({ StaticName: "_LayoutElement", Name: "Layout element" });
                    $scope.contentTypes = $filter('orderBy')($scope.contentTypes, 'Name');
                }

                $scope.loading--;
            });

        };

        $scope.$watch('templateId', function (newTemplateId, oldTemplateId) {
        	if (newTemplateId != oldTemplateId) {
        		alert("templateId changed");
        		if ($scope.manageInfo.isContentApp)
        			$scope.renderTemplate(newTemplateId);
        		else {
        			$scope.loading++;
			        var promise;
			        if ($scope.manageInfo.hasContent)
				        promise = $scope.saveTemplateId(newTemplateId);
			        else
				        promise = $scope.setPreviewTemplateId(newTemplateId);
			        promise.then(function() {
        				$window.location.reload();
			        });
        		}
        	}
        });

        $scope.$watch('contentTypeId', function (newContentTypeId, oldContentTypeId) {
        	if (newContentTypeId == oldContentTypeId)
        		return;
        	// Select first template if contentType changed
        	var firstTemplateId = $scope.filteredTemplates(newContentTypeId)[0].TemplateId; // $filter('filter')($scope.templates, { AttributeSetId: $scope.contentTypeId == null ? "!!" : $scope.contentTypeId })[0].TemplateID;
        	if ($scope.templateId != firstTemplateId && firstTemplateId != null)
        		$scope.templateId = firstTemplateId;
        });

        if ($scope.appId != null && $scope.manageInfo.templateChooserVisible)
            $scope.reloadTemplates();

        $scope.$watch('manageInfo.templateChooserVisible', function(visible, oldVisible) {
            if (visible != oldVisible && $scope.appId != null && visible)
                $scope.reloadTemplates();
        });

        $scope.$watch('appId', function (newAppId, oldAppId) {
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

        if (!$scope.manageInfo.isContentApp) {
            moduleApi.getSelectableApps().then(function(data) {
                $scope.apps = data.data;
                $scope.apps.push({ Name: $attrs.importapptext, AppId: -1 });
            });
        }

        $scope.setTemplateChooserState = function (state) {
            // Reset templateid / cancel template change
            if (!state)
                $scope.templateId = $scope.savedTemplateId;

            return moduleApi.setTemplateChooserState(state).then(function () {
                $scope.manageInfo.templateChooserVisible = state;
            });
        };

        $scope.saveTemplateId = function () {
        	var promises = [];

			// Save only if the currently saved is not the same as the new
        	if (!$scope.manageInfo.hasContent || $scope.savedTemplateId != $scope.templateId) {
        		promises.push(moduleApi.saveTemplateId($scope.templateId));
        		$scope.savedTemplateId = $scope.templateId;
        		promises.push($scope.setTemplateChooserState(false));
	        }

            return $q.all(promises);
        };

	    $scope.setPreviewTemplateId = function() {
		    return moduleApi.setPreviewTemplateId($scope.templateId);
	    };

        $scope.renderTemplate = function (templateId) {
            $scope.loading++;
            moduleApi.renderTemplate(templateId).then(function (response) {
                try {
                    $scope.insertRenderedTemplate(response.data);
                    $2sxc(moduleId).manage._processToolbars();
                } catch (e) {
                    console.log("Error while rendering template:");
                    console.log(e);
                }
                $scope.loading--;
            });
        };

        $scope.insertRenderedTemplate = function(renderedTemplate) {
            $(".DnnModule-" + moduleId + " .sc-viewport").html(renderedTemplate);
        };

		// ToDo: Remove this here, as it's not used in TemplateSelector - should move to 2sxc.api.manage.js
        $scope.addItem = function(sortOrder) {
            moduleApi.addItem(sortOrder).then(function () {
                $scope.renderTemplate($scope.templateId);
            });
        };
        $scope.removeFromList = function (sortOrder) {
        	moduleApi.removeFromList(sortOrder).then(function () {
        		$scope.renderTemplate($scope.templateId);
        	});
        };
        $scope.changeOrder = function (sortOrder, desintationSortOrder) {
        	moduleApi.changeOrder(sortOrder, desintationSortOrder).then(function () {
        		$scope.renderTemplate($scope.templateId);
        	});
        };

    });

    module.factory('moduleApiService', function(apiService) {
        return function(moduleId) {
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
    });

})();