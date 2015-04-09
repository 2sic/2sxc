(function () {
    var module = angular.module('2sxc.view', ["2sxc.api"]);

    module.controller('TemplateSelectorCtrl', ["$scope", "$attrs", "moduleApiService", "$filter", "$q", "$window", function($scope, $attrs, moduleApiService, $filter, $q, $window) {

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
            return $filter('filter')($scope.templates, contentTypeId == 0 ? { ContentTypeStaticName: null } : { ContentTypeStaticName: contentTypeId }, true);
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
                if ($filter('filter')($scope.templates, { ContentTypeStaticName: null }, true).length > 0) {
                	$scope.contentTypes.push({ StaticName: null, Name: "Layout element" });
                    $scope.contentTypes = $filter('orderBy')($scope.contentTypes, 'Name');
                }

                $scope.loading--;
            });

        };

        $scope.$watch('templateId', function (newTemplateId, oldTemplateId) {
        	if (newTemplateId != oldTemplateId) {
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

            promises.push(moduleApi.saveTemplateId($scope.templateId));

            $scope.savedTemplateId = $scope.templateId;
            promises.push($scope.setTemplateChooserState(false));

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

        $scope.addItem = function(sortOrder) {
            moduleApi.addItem(sortOrder).then(function () {
                $scope.renderTemplate($scope.templateId);
            });
        };

    }]);

    module.factory('moduleApiService', ["apiService", function(apiService) {
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
                }
            };
        };
    }]);

})();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIjJzeGMuVGVtcGxhdGVTZWxlY3Rvci5qcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxDQUFDLFlBQVk7SUFDVCxJQUFJLFNBQVMsUUFBUSxPQUFPLGFBQWEsQ0FBQzs7SUFFMUMsT0FBTyxXQUFXLDZGQUF3QixTQUFTLFFBQVEsUUFBUSxrQkFBa0IsU0FBUyxJQUFJLFNBQVM7O1FBRXZHLElBQUksV0FBVyxPQUFPOztRQUV0QixJQUFJLFlBQVksaUJBQWlCOztRQUVqQyxPQUFPLGFBQWEsTUFBTSxVQUFVLE9BQU87UUFDM0MsT0FBTyxPQUFPO1FBQ2QsT0FBTyxlQUFlO1FBQ3RCLE9BQU8sWUFBWTtRQUNuQixPQUFPLG9CQUFvQixVQUFVLGVBQWU7O1lBRWhELElBQUksQ0FBQyxPQUFPLFdBQVc7Z0JBQ25CLE9BQU8sT0FBTztZQUNsQixPQUFPLFFBQVEsVUFBVSxPQUFPLFdBQVcsaUJBQWlCLElBQUksRUFBRSx1QkFBdUIsU0FBUyxFQUFFLHVCQUF1QixpQkFBaUI7O1FBRWhKLE9BQU8sZ0JBQWdCLE9BQU8sV0FBVztRQUN6QyxPQUFPLGFBQWEsT0FBTyxXQUFXO1FBQ3RDLE9BQU8sa0JBQWtCLE9BQU8sV0FBVztRQUMzQyxPQUFPLFFBQVEsT0FBTyxXQUFXO1FBQ2pDLE9BQU8sYUFBYSxPQUFPLFdBQVc7UUFDdEMsT0FBTyxVQUFVOztRQUVqQixPQUFPLGtCQUFrQixXQUFXOztZQUVoQyxPQUFPO1lBQ1AsSUFBSSxrQkFBa0IsVUFBVTtZQUNoQyxJQUFJLGVBQWUsVUFBVTs7WUFFN0IsR0FBRyxJQUFJLENBQUMsaUJBQWlCLGVBQWUsS0FBSyxVQUFVLEtBQUs7Z0JBQ3hELE9BQU8sZUFBZSxJQUFJLEdBQUc7Z0JBQzdCLE9BQU8sWUFBWSxJQUFJLEdBQUc7OztnQkFHMUIsSUFBSSxRQUFRLFVBQVUsT0FBTyxXQUFXLEVBQUUsdUJBQXVCLFFBQVEsTUFBTSxTQUFTLEdBQUc7aUJBQzFGLE9BQU8sYUFBYSxLQUFLLEVBQUUsWUFBWSxNQUFNLE1B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R3JCLFNBQVMsU0FBUyxXQUFXO29CQUN6QixPQUFPLFdBQVcsVUFBVTt3QkFDeEIsS0FBSzt3QkFDTCxRQUFRLEVBQUUsV0FBVzs7O2dCQUc3QixtQkFBbUIsV0FBVztvQkFDMUIsT0FBTyxXQUFXLFVBQVU7d0JBQ3hCLEtBQUs7OztnQkFHYixVQUFVLFNBQVMsT0FBTztvQkFDdEIsT0FBTyxXQUFXLFVBQVU7d0JBQ3hCLEtBQUs7d0JBQ0wsUUFBUSxFQUFFLE9BQU87OztnQkFHekIsMkJBQTJCLFlBQVk7b0JBQ25DLE9BQU8sV0FBVyxVQUFVO3dCQUN4QixLQUFLOzs7Z0JBR2Isd0JBQXdCLFdBQVc7b0JBQy9CLE9BQU8sV0FBVyxVQUFVO3dCQUN4QixLQUFLOzs7Z0JBR2IseUJBQXlCLFNBQVMsT0FBTztvQkFDckMsT0FBTyxXQUFXLFVBQVU7d0JBQ3hCLEtBQUs7d0JBQ0wsUUFBUSxFQUFFLE9BQU87OztnQkFHekIsZ0JBQWdCLFNBQVMsWUFBWTtvQkFDakMsT0FBTyxXQUFXLFVBQVU7d0JBQ3hCLEtBQUs7d0JBQ0wsUUFBUSxFQUFFLFlBQVk7Ozs7Ozs7S0FPekMiLCJzb3VyY2VzQ29udGVudCI6WyIoZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIG1vZHVsZSA9IGFuZ3VsYXIubW9kdWxlKCcyc3hjLnZpZXcnLCBbXCIyc3hjLmFwaVwiXSk7XHJcblxyXG4gICAgbW9kdWxlLmNvbnRyb2xsZXIoJ1RlbXBsYXRlU2VsZWN0b3JDdHJsJywgZnVuY3Rpb24oJHNjb3BlLCAkYXR0cnMsIG1vZHVsZUFwaVNlcnZpY2UsICRmaWx0ZXIsICRxLCAkd2luZG93KSB7XHJcblxyXG4gICAgICAgIHZhciBtb2R1bGVJZCA9ICRhdHRycy5tb2R1bGVpZDtcclxuXHJcbiAgICAgICAgdmFyIG1vZHVsZUFwaSA9IG1vZHVsZUFwaVNlcnZpY2UobW9kdWxlSWQpO1xyXG5cclxuICAgICAgICAkc2NvcGUubWFuYWdlSW5mbyA9ICQyc3hjKG1vZHVsZUlkKS5tYW5hZ2UuX21hbmFnZUluZm87XHJcbiAgICAgICAgJHNjb3BlLmFwcHMgPSBbXTtcclxuICAgICAgICAkc2NvcGUuY29udGVudFR5cGVzID0gW107XHJcbiAgICAgICAgJHNjb3BlLnRlbXBsYXRlcyA9IFtdO1xyXG4gICAgICAgICRzY29wZS5maWx0ZXJlZFRlbXBsYXRlcyA9IGZ1bmN0aW9uIChjb250ZW50VHlwZUlkKSB7XHJcbiAgICAgICAgICAgIC8vIFJldHVybiBhbGwgdGVtcGxhdGVzIGZvciBBcHBcclxuICAgICAgICAgICAgaWYgKCEkc2NvcGUubWFuYWdlSW5mby5pc0NvbnRlbnRBcHApXHJcbiAgICAgICAgICAgICAgICByZXR1cm4gJHNjb3BlLnRlbXBsYXRlcztcclxuICAgICAgICAgICAgcmV0dXJuICRmaWx0ZXIoJ2ZpbHRlcicpKCRzY29wZS50ZW1wbGF0ZXMsIGNvbnRlbnRUeXBlSWQgPT0gMCA/IHsgQ29udGVudFR5cGVTdGF0aWNOYW1lOiBudWxsIH0gOiB7IENvbnRlbnRUeXBlU3RhdGljTmFtZTogY29udGVudFR5cGVJZCB9LCB0cnVlKTtcclxuICAgICAgICB9O1xyXG4gICAgICAgICRzY29wZS5jb250ZW50VHlwZUlkID0gJHNjb3BlLm1hbmFnZUluZm8uY29udGVudFR5cGVJZDtcclxuICAgICAgICAkc2NvcGUudGVtcGxhdGVJZCA9ICRzY29wZS5tYW5hZ2VJbmZvLnRlbXBsYXRlSWQ7XHJcbiAgICAgICAgJHNjb3BlLnNhdmVkVGVtcGxhdGVJZCA9ICRzY29wZS5tYW5hZ2VJbmZvLnRlbXBsYXRlSWQ7XHJcbiAgICAgICAgJHNjb3BlLmFwcElkID0gJHNjb3BlLm1hbmFnZUluZm8uYXBwSWQ7XHJcbiAgICAgICAgJHNjb3BlLnNhdmVkQXBwSWQgPSAkc2NvcGUubWFuYWdlSW5mby5hcHBJZDtcclxuICAgICAgICAkc2NvcGUubG9hZGluZyA9IDA7XHJcblxyXG4gICAgICAgICRzY29wZS5yZWxvYWRUZW1wbGF0ZXMgPSBmdW5jdGlvbigpIHtcclxuXHJcbiAgICAgICAgICAgICRzY29wZS5sb2FkaW5nKys7XHJcbiAgICAgICAgICAgIHZhciBnZXRDb250ZW50VHlwZXMgPSBtb2R1bGVBcGkuZ2V0U2VsZWN0YWJsZUNvbnRlbnRUeXBlcygpO1xyXG4gICAgICAgICAgICB2YXIgZ2V0VGVtcGxhdGVzID0gbW9kdWxlQXBpLmdldFNlbGVjdGFibGVUZW1wbGF0ZXMoKTtcclxuXHJcbiAgICAgICAgICAgICRxLmFsbChbZ2V0Q29udGVudFR5cGVzLCBnZXRUZW1wbGF0ZXNdKS50aGVuKGZ1bmN0aW9uIChyZXMpIHtcclxuICAgICAgICAgICAgICAgICRzY29wZS5jb250ZW50VHlwZXMgPSByZXNbMF0uZGF0YTtcclxuICAgICAgICAgICAgICAgICRzY29wZS50ZW1wbGF0ZXMgPSByZXNbMV0uZGF0YTtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBBZGQgb3B0aW9uIGZvciBubyBjb250ZW50IHR5cGUgaWYgdGhlcmUgYXJlIHRlbXBsYXRlcyB3aXRob3V0XHJcbiAgICAgICAgICAgICAgICBpZiAoJGZpbHRlcignZmlsdGVyJykoJHNjb3BlLnRlbXBsYXRlcywgeyBDb250ZW50VHlwZVN0YXRpY05hbWU6IG51bGwgfSwgdHJ1ZSkubGVuZ3RoID4gMCkge1xyXG4gICAgICAgICAgICAgICAgXHQkc2NvcGUuY29udGVudFR5cGVzLnB1c2goeyBTdGF0aWNOYW1lOiBudWxsLCBOYW1lOiBcIkxheW91dCBlbGVtZW50XCIgfSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJHNjb3BlLmNvbnRlbnRUeXBlcyA9ICRmaWx0ZXIoJ29yZGVyQnknKSgkc2NvcGUuY29udGVudFR5cGVzLCAnTmFtZScpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICRzY29wZS5sb2FkaW5nLS07XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICB9O1xyXG5cclxuICAgICAgICAkc2NvcGUuJHdhdGNoKCd0ZW1wbGF0ZUlkJywgZnVuY3Rpb24gKG5ld1RlbXBsYXRlSWQsIG9sZFRlbXBsYXRlSWQpIHtcclxuICAgICAgICBcdGlmIChuZXdUZW1wbGF0ZUlkICE9IG9sZFRlbXBsYXRlSWQpIHtcclxuICAgICAgICBcdFx0aWYgKCRzY29wZS5tYW5hZ2VJbmZvLmlzQ29udGVudEFwcClcclxuICAgICAgICBcdFx0XHQkc2NvcGUucmVuZGVyVGVtcGxhdGUobmV3VGVtcGxhdGVJZCk7XHJcbiAgICAgICAgXHRcdGVsc2Uge1xyXG4gICAgICAgIFx0XHRcdCRzY29wZS5sb2FkaW5nKys7XHJcblx0XHRcdCAgICAgICAgdmFyIHByb21pc2U7XHJcblx0XHRcdCAgICAgICAgaWYgKCRzY29wZS5tYW5hZ2VJbmZvLmhhc0NvbnRlbnQpXHJcblx0XHRcdFx0ICAgICAgICBwcm9taXNlID0gJHNjb3BlLnNhdmVUZW1wbGF0ZUlkKG5ld1RlbXBsYXRlSWQpO1xyXG5cdFx0XHQgICAgICAgIGVsc2VcclxuXHRcdFx0XHQgICAgICAgIHByb21pc2UgPSAkc2NvcGUuc2V0UHJldmlld1RlbXBsYXRlSWQobmV3VGVtcGxhdGVJZCk7XHJcblx0XHRcdCAgICAgICAgcHJvbWlzZS50aGVuKGZ1bmN0aW9uKCkge1xyXG4gICAgICAgIFx0XHRcdFx0JHdpbmRvdy5sb2NhdGlvbi5yZWxvYWQoKTtcclxuXHRcdFx0ICAgICAgICB9KTtcclxuICAgICAgICBcdFx0fVxyXG4gICAgICAgIFx0fVxyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICAkc2NvcGUuJHdhdGNoKCdjb250ZW50VHlwZUlkJywgZnVuY3Rpb24gKG5ld0NvbnRlbnRUeXBlSWQsIG9sZENvbnRlbnRUeXBlSWQpIHtcclxuICAgICAgICBcdGlmIChuZXdDb250ZW50VHlwZUlkID09IG9sZENvbnRlbnRUeXBlSWQpXHJcbiAgICAgICAgXHRcdHJldHVybjtcclxuICAgICAgICBcdC8vIFNlbGVjdCBmaXJzdCB0ZW1wbGF0ZSBpZiBjb250ZW50VHlwZSBjaGFuZ2VkXHJcbiAgICAgICAgXHR2YXIgZmlyc3RUZW1wbGF0ZUlkID0gJHNjb3BlLmZpbHRlcmVkVGVtcGxhdGVzKG5ld0NvbnRlbnRUeXBlSWQpWzBdLlRlbXBsYXRlSWQ7IC8vICRmaWx0ZXIoJ2ZpbHRlcicpKCRzY29wZS50ZW1wbGF0ZXMsIHsgQXR0cmlidXRlU2V0SWQ6ICRzY29wZS5jb250ZW50VHlwZUlkID09IG51bGwgPyBcIiEhXCIgOiAkc2NvcGUuY29udGVudFR5cGVJZCB9KVswXS5UZW1wbGF0ZUlEO1xyXG4gICAgICAgIFx0aWYgKCRzY29wZS50ZW1wbGF0ZUlkICE9IGZpcnN0VGVtcGxhdGVJZCAmJiBmaXJzdFRlbXBsYXRlSWQgIT0gbnVsbClcclxuICAgICAgICBcdFx0JHNjb3BlLnRlbXBsYXRlSWQgPSBmaXJzdFRlbXBsYXRlSWQ7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIGlmICgkc2NvcGUuYXBwSWQgIT0gbnVsbCAmJiAkc2NvcGUubWFuYWdlSW5mby50ZW1wbGF0ZUNob29zZXJWaXNpYmxlKVxyXG4gICAgICAgICAgICAkc2NvcGUucmVsb2FkVGVtcGxhdGVzKCk7XHJcblxyXG4gICAgICAgICRzY29wZS4kd2F0Y2goJ21hbmFnZUluZm8udGVtcGxhdGVDaG9vc2VyVmlzaWJsZScsIGZ1bmN0aW9uKHZpc2libGUsIG9sZFZpc2libGUpIHtcclxuICAgICAgICAgICAgaWYgKHZpc2libGUgIT0gb2xkVmlzaWJsZSAmJiAkc2NvcGUuYXBwSWQgIT0gbnVsbCAmJiB2aXNpYmxlKVxyXG4gICAgICAgICAgICAgICAgJHNjb3BlLnJlbG9hZFRlbXBsYXRlcygpO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICAkc2NvcGUuJHdhdGNoKCdhcHBJZCcsIGZ1bmN0aW9uIChuZXdBcHBJZCwgb2xkQXBwSWQpIHtcclxuICAgICAgICAgICAgaWYgKG5ld0FwcElkID09IG9sZEFwcElkIHx8IG5ld0FwcElkID09IG51bGwpXHJcbiAgICAgICAgICAgICAgICByZXR1cm47XHJcblxyXG4gICAgICAgICAgICBpZiAobmV3QXBwSWQgPT0gLTEpIHtcclxuICAgICAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbiA9ICRhdHRycy5pbXBvcnRhcHBkaWFsb2c7XHJcbiAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIG1vZHVsZUFwaS5zZXRBcHBJZChuZXdBcHBJZCkudGhlbihmdW5jdGlvbigpIHtcclxuICAgICAgICAgICAgICAgICR3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBpZiAoISRzY29wZS5tYW5hZ2VJbmZvLmlzQ29udGVudEFwcCkge1xyXG4gICAgICAgICAgICBtb2R1bGVBcGkuZ2V0U2VsZWN0YWJsZUFwcHMoKS50aGVuKGZ1bmN0aW9uKGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICRzY29wZS5hcHBzID0gZGF0YS5kYXRhO1xyXG4gICAgICAgICAgICAgICAgJHNjb3BlLmFwcHMucHVzaCh7IE5hbWU6ICRhdHRycy5pbXBvcnRhcHB0ZXh0LCBBcHBJZDogLTEgfSk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgJHNjb3BlLnNldFRlbXBsYXRlQ2hvb3NlclN0YXRlID0gZnVuY3Rpb24gKHN0YXRlKSB7XHJcbiAgICAgICAgICAgIC8vIFJlc2V0IHRlbXBsYXRlaWQgLyBjYW5jZWwgdGVtcGxhdGUgY2hhbmdlXHJcbiAgICAgICAgICAgIGlmICghc3RhdGUpXHJcbiAgICAgICAgICAgICAgICAkc2NvcGUudGVtcGxhdGVJZCA9ICRzY29wZS5zYXZlZFRlbXBsYXRlSWQ7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gbW9kdWxlQXBpLnNldFRlbXBsYXRlQ2hvb3NlclN0YXRlKHN0YXRlKS50aGVuKGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICRzY29wZS5tYW5hZ2VJbmZvLnRlbXBsYXRlQ2hvb3NlclZpc2libGUgPSBzdGF0ZTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfTtcclxuXHJcbiAgICAgICAgJHNjb3BlLnNhdmVUZW1wbGF0ZUlkID0gZnVuY3Rpb24gKCkge1xyXG4gICAgICAgIFx0dmFyIHByb21pc2VzID0gW107XHJcblxyXG4gICAgICAgICAgICBwcm9taXNlcy5wdXNoKG1vZHVsZUFwaS5zYXZlVGVtcGxhdGVJZCgkc2NvcGUudGVtcGxhdGVJZCkpO1xyXG5cclxuICAgICAgICAgICAgJHNjb3BlLnNhdmVkVGVtcGxhdGVJZCA9ICRzY29wZS50ZW1wbGF0ZUlkO1xyXG4gICAgICAgICAgICBwcm9taXNlcy5wdXNoKCRzY29wZS5zZXRUZW1wbGF0ZUNob29zZXJTdGF0ZShmYWxzZSkpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuICRxLmFsbChwcm9taXNlcyk7XHJcbiAgICAgICAgfTtcclxuXHJcblx0ICAgICRzY29wZS5zZXRQcmV2aWV3VGVtcGxhdGVJZCA9IGZ1bmN0aW9uKCkge1xyXG5cdFx0ICAgIHJldHVybiBtb2R1bGVBcGkuc2V0UHJldmlld1RlbXBsYXRlSWQoJHNjb3BlLnRlbXBsYXRlSWQpO1xyXG5cdCAgICB9O1xyXG5cclxuICAgICAgICAkc2NvcGUucmVuZGVyVGVtcGxhdGUgPSBmdW5jdGlvbiAodGVtcGxhdGVJZCkge1xyXG4gICAgICAgICAgICAkc2NvcGUubG9hZGluZysrO1xyXG4gICAgICAgICAgICBtb2R1bGVBcGkucmVuZGVyVGVtcGxhdGUodGVtcGxhdGVJZCkudGhlbihmdW5jdGlvbiAocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJHNjb3BlLmluc2VydFJlbmRlcmVkVGVtcGxhdGUocmVzcG9uc2UuZGF0YSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJDJzeGMobW9kdWxlSWQpLm1hbmFnZS5fcHJvY2Vzc1Rvb2xiYXJzKCk7XHJcbiAgICAgICAgICAgICAgICB9IGNhdGNoIChlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coXCJFcnJvciB3aGlsZSByZW5kZXJpbmcgdGVtcGxhdGU6XCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKGUpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgJHNjb3BlLmxvYWRpbmctLTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfTtcclxuXHJcbiAgICAgICAgJHNjb3BlLmluc2VydFJlbmRlcmVkVGVtcGxhdGUgPSBmdW5jdGlvbihyZW5kZXJlZFRlbXBsYXRlKSB7XHJcbiAgICAgICAgICAgICQoXCIuRG5uTW9kdWxlLVwiICsgbW9kdWxlSWQgKyBcIiAuc2Mtdmlld3BvcnRcIikuaHRtbChyZW5kZXJlZFRlbXBsYXRlKTtcclxuICAgICAgICB9O1xyXG5cclxuICAgICAgICAkc2NvcGUuYWRkSXRlbSA9IGZ1bmN0aW9uKHNvcnRPcmRlcikge1xyXG4gICAgICAgICAgICBtb2R1bGVBcGkuYWRkSXRlbShzb3J0T3JkZXIpLnRoZW4oZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgJHNjb3BlLnJlbmRlclRlbXBsYXRlKCRzY29wZS50ZW1wbGF0ZUlkKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfTtcclxuXHJcbiAgICB9KTtcclxuXHJcbiAgICBtb2R1bGUuZmFjdG9yeSgnbW9kdWxlQXBpU2VydmljZScsIGZ1bmN0aW9uKGFwaVNlcnZpY2UpIHtcclxuICAgICAgICByZXR1cm4gZnVuY3Rpb24obW9kdWxlSWQpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgICAgIHNhdmVUZW1wbGF0ZUlkOiBmdW5jdGlvbih0ZW1wbGF0ZUlkKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGFwaVNlcnZpY2UobW9kdWxlSWQsIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiAnVmlldy9Nb2R1bGUvU2F2ZVRlbXBsYXRlSWQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJhbXM6IHsgdGVtcGxhdGVJZDogdGVtcGxhdGVJZCB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBcdHNldFByZXZpZXdUZW1wbGF0ZUlkOiBmdW5jdGlvbih0ZW1wbGF0ZUlkKSB7XHJcbiAgICAgICAgICAgIFx0XHRyZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICBcdFx0XHR1cmw6ICdWaWV3L01vZHVsZS9TZXRQcmV2aWV3VGVtcGxhdGVJZCcsXHJcbiAgICAgICAgICAgIFx0XHRcdHBhcmFtczogeyB0ZW1wbGF0ZUlkOiB0ZW1wbGF0ZUlkIH1cclxuICAgICAgICAgICAgXHRcdH0pO1xyXG5cdCAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBhZGRJdGVtOiBmdW5jdGlvbihzb3J0T3JkZXIpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1cmw6ICdWaWV3L01vZHVsZS9BZGRJdGVtJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFyYW1zOiB7IHNvcnRPcmRlcjogc29ydE9yZGVyIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBnZXRTZWxlY3RhYmxlQXBwczogZnVuY3Rpb24oKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGFwaVNlcnZpY2UobW9kdWxlSWQsIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiAnVmlldy9Nb2R1bGUvR2V0U2VsZWN0YWJsZUFwcHMnXHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgc2V0QXBwSWQ6IGZ1bmN0aW9uKGFwcElkKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGFwaVNlcnZpY2UobW9kdWxlSWQsIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiAnVmlldy9Nb2R1bGUvU2V0QXBwSWQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJhbXM6IHsgYXBwSWQ6IGFwcElkIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBnZXRTZWxlY3RhYmxlQ29udGVudFR5cGVzOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGFwaVNlcnZpY2UobW9kdWxlSWQsIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiAnVmlldy9Nb2R1bGUvR2V0U2VsZWN0YWJsZUNvbnRlbnRUeXBlcydcclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBnZXRTZWxlY3RhYmxlVGVtcGxhdGVzOiBmdW5jdGlvbigpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1cmw6ICdWaWV3L01vZHVsZS9HZXRTZWxlY3RhYmxlVGVtcGxhdGVzJ1xyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIHNldFRlbXBsYXRlQ2hvb3NlclN0YXRlOiBmdW5jdGlvbihzdGF0ZSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBhcGlTZXJ2aWNlKG1vZHVsZUlkLCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHVybDogJ1ZpZXcvTW9kdWxlL1NldFRlbXBsYXRlQ2hvb3NlclN0YXRlJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFyYW1zOiB7IHN0YXRlOiBzdGF0ZSB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgcmVuZGVyVGVtcGxhdGU6IGZ1bmN0aW9uKHRlbXBsYXRlSWQpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1cmw6ICdWaWV3L01vZHVsZS9SZW5kZXJUZW1wbGF0ZScsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcmFtczogeyB0ZW1wbGF0ZUlkOiB0ZW1wbGF0ZUlkIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfTtcclxuICAgICAgICB9O1xyXG4gICAgfSk7XHJcblxyXG59KSgpOyJdfQ==