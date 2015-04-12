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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIjJzeGMuVGVtcGxhdGVTZWxlY3Rvci5qcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxDQUFDLFlBQVk7SUFDVCxJQUFJLFNBQVMsUUFBUSxPQUFPLGFBQWEsQ0FBQzs7SUFFMUMsT0FBTyxXQUFXLDZGQUF3QixTQUFTLFFBQVEsUUFBUSxrQkFBa0IsU0FBUyxJQUFJLFNBQVM7O1FBRXZHLElBQUksV0FBVyxPQUFPOztRQUV0QixJQUFJLFlBQVksaUJBQWlCOztRQUVqQyxPQUFPLGFBQWEsTUFBTSxVQUFVLE9BQU87UUFDM0MsT0FBTyxPQUFPO1FBQ2QsT0FBTyxlQUFlO1FBQ3RCLE9BQU8sWUFBWTtRQUNuQixPQUFPLG9CQUFvQixVQUFVLGVBQWU7O1lBRWhELElBQUksQ0FBQyxPQUFPLFdBQVc7Z0JBQ25CLE9BQU8sT0FBTztZQUNsQixPQUFPLFFBQVEsVUFBVSxPQUFPLFdBQVcsaUJBQWlCLG1CQUFtQixFQUFFLHVCQUF1QixPQUFPLEVBQUUsdUJBQXVCLGlCQUFpQjs7UUFFN0osT0FBTyxnQkFBZ0IsT0FBTyxXQUFXO1FBQ3pDLE9BQU8sYUFBYSxPQUFPLFdBQVc7UUFDdEMsT0FBTyxrQkFBa0IsT0FBTyxXQUFXO1FBQzNDLE9BQU8sUUFBUSxPQUFPLFdBQVc7UUFDakMsT0FBTyxhQUFhLE9BQU8sV0FBVztRQUN0QyxPQUFPLFVBQVU7O1FBRWpCLE9BQU8sa0JBQWtCLFdBQVc7O1lBRWhDLE9BQU87WUFDUCxJQUFJLGtCQUFrQixVQUFVO1lBQ2hDLElBQUksZUFBZSxVQUFVOztZQUU3QixHQUFHLElBQUksQ0FBQyxpQkFBaUIsZUFBZSxLQUFLLFVBQVUsS0FBSztnQkFDeEQsT0FBTyxlQUFlLElBQUksR0FBRztnQkFDN0IsT0FBTyxZQUFZLElBQUksR0FBRzs7O2dCQUcxQixJQUFJLFFBQVEsVUFBVSxPQUFPLFdBQVcsRUFBRSx1QkFBdUIsTUFBTSxNQUFNLFNBQVMsR0FBRztpQkFDeEYsT0FBTyxhQUFhLEtBQUssRUFBRSxZQUFZLGtCQUFr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O2dCQUdyQixTQUFTLFNBQVMsV0FBVztvQkFDekIsT0FBTyxXQUFXLFVBQVU7d0JBQ3hCLEtBQUs7d0JBQ0wsUUFBUSxFQUFFLFdBQVc7OztnQkFHN0IsbUJBQW1CLFdBQVc7b0JBQzFCLE9BQU8sV0FBVyxVQUFVO3dCQUN4QixLQUFLOzs7Z0JBR2IsVUFBVSxTQUFTLE9BQU87b0JBQ3RCLE9BQU8sV0FBVyxVQUFVO3dCQUN4QixLQUFLO3dCQUNMLFFBQVEsRUFBRSxPQUFPOzs7Z0JBR3pCLDJCQUEyQixZQUFZO29CQUNuQyxPQUFPLFdBQVcsVUFBVTt3QkFDeEIsS0FBSzs7O2dCQUdiLHdCQUF3QixXQUFXO29CQUMvQixPQUFPLFdBQVcsVUFBVTt3QkFDeEIsS0FBSzs7O2dCQUdiLHlCQUF5QixTQUFTLE9BQU87b0JBQ3JDLE9BQU8sV0FBVyxVQUFVO3dCQUN4QixLQUFLO3dCQUNMLFFBQVEsRUFBRSxPQUFPOzs7Z0JBR3pCLGdCQUFnQixTQUFTLFlBQVk7b0JBQ2pDLE9BQU8sV0FBVyxVQUFVO3dCQUN4QixLQUFLO3dCQUNMLFFBQVEsRUFBRSxZQUFZOzs7Ozs7O0tBT3pDIiwic291cmNlc0NvbnRlbnQiOlsiKGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBtb2R1bGUgPSBhbmd1bGFyLm1vZHVsZSgnMnN4Yy52aWV3JywgW1wiMnN4Yy5hcGlcIl0pO1xyXG5cclxuICAgIG1vZHVsZS5jb250cm9sbGVyKCdUZW1wbGF0ZVNlbGVjdG9yQ3RybCcsIGZ1bmN0aW9uKCRzY29wZSwgJGF0dHJzLCBtb2R1bGVBcGlTZXJ2aWNlLCAkZmlsdGVyLCAkcSwgJHdpbmRvdykge1xyXG5cclxuICAgICAgICB2YXIgbW9kdWxlSWQgPSAkYXR0cnMubW9kdWxlaWQ7XHJcblxyXG4gICAgICAgIHZhciBtb2R1bGVBcGkgPSBtb2R1bGVBcGlTZXJ2aWNlKG1vZHVsZUlkKTtcclxuXHJcbiAgICAgICAgJHNjb3BlLm1hbmFnZUluZm8gPSAkMnN4Yyhtb2R1bGVJZCkubWFuYWdlLl9tYW5hZ2VJbmZvO1xyXG4gICAgICAgICRzY29wZS5hcHBzID0gW107XHJcbiAgICAgICAgJHNjb3BlLmNvbnRlbnRUeXBlcyA9IFtdO1xyXG4gICAgICAgICRzY29wZS50ZW1wbGF0ZXMgPSBbXTtcclxuICAgICAgICAkc2NvcGUuZmlsdGVyZWRUZW1wbGF0ZXMgPSBmdW5jdGlvbiAoY29udGVudFR5cGVJZCkge1xyXG4gICAgICAgICAgICAvLyBSZXR1cm4gYWxsIHRlbXBsYXRlcyBmb3IgQXBwXHJcbiAgICAgICAgICAgIGlmICghJHNjb3BlLm1hbmFnZUluZm8uaXNDb250ZW50QXBwKVxyXG4gICAgICAgICAgICAgICAgcmV0dXJuICRzY29wZS50ZW1wbGF0ZXM7XHJcbiAgICAgICAgICAgIHJldHVybiAkZmlsdGVyKCdmaWx0ZXInKSgkc2NvcGUudGVtcGxhdGVzLCBjb250ZW50VHlwZUlkID09IFwiX0xheW91dEVsZW1lbnRcIiA/IHsgQ29udGVudFR5cGVTdGF0aWNOYW1lOiBcIlwiIH0gOiB7IENvbnRlbnRUeXBlU3RhdGljTmFtZTogY29udGVudFR5cGVJZCB9LCB0cnVlKTtcclxuICAgICAgICB9O1xyXG4gICAgICAgICRzY29wZS5jb250ZW50VHlwZUlkID0gJHNjb3BlLm1hbmFnZUluZm8uY29udGVudFR5cGVJZDtcclxuICAgICAgICAkc2NvcGUudGVtcGxhdGVJZCA9ICRzY29wZS5tYW5hZ2VJbmZvLnRlbXBsYXRlSWQ7XHJcbiAgICAgICAgJHNjb3BlLnNhdmVkVGVtcGxhdGVJZCA9ICRzY29wZS5tYW5hZ2VJbmZvLnRlbXBsYXRlSWQ7XHJcbiAgICAgICAgJHNjb3BlLmFwcElkID0gJHNjb3BlLm1hbmFnZUluZm8uYXBwSWQ7XHJcbiAgICAgICAgJHNjb3BlLnNhdmVkQXBwSWQgPSAkc2NvcGUubWFuYWdlSW5mby5hcHBJZDtcclxuICAgICAgICAkc2NvcGUubG9hZGluZyA9IDA7XHJcblxyXG4gICAgICAgICRzY29wZS5yZWxvYWRUZW1wbGF0ZXMgPSBmdW5jdGlvbigpIHtcclxuXHJcbiAgICAgICAgICAgICRzY29wZS5sb2FkaW5nKys7XHJcbiAgICAgICAgICAgIHZhciBnZXRDb250ZW50VHlwZXMgPSBtb2R1bGVBcGkuZ2V0U2VsZWN0YWJsZUNvbnRlbnRUeXBlcygpO1xyXG4gICAgICAgICAgICB2YXIgZ2V0VGVtcGxhdGVzID0gbW9kdWxlQXBpLmdldFNlbGVjdGFibGVUZW1wbGF0ZXMoKTtcclxuXHJcbiAgICAgICAgICAgICRxLmFsbChbZ2V0Q29udGVudFR5cGVzLCBnZXRUZW1wbGF0ZXNdKS50aGVuKGZ1bmN0aW9uIChyZXMpIHtcclxuICAgICAgICAgICAgICAgICRzY29wZS5jb250ZW50VHlwZXMgPSByZXNbMF0uZGF0YTtcclxuICAgICAgICAgICAgICAgICRzY29wZS50ZW1wbGF0ZXMgPSByZXNbMV0uZGF0YTtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBBZGQgb3B0aW9uIGZvciBubyBjb250ZW50IHR5cGUgaWYgdGhlcmUgYXJlIHRlbXBsYXRlcyB3aXRob3V0XHJcbiAgICAgICAgICAgICAgICBpZiAoJGZpbHRlcignZmlsdGVyJykoJHNjb3BlLnRlbXBsYXRlcywgeyBDb250ZW50VHlwZVN0YXRpY05hbWU6IFwiXCIgfSwgdHJ1ZSkubGVuZ3RoID4gMCkge1xyXG4gICAgICAgICAgICAgICAgXHQkc2NvcGUuY29udGVudFR5cGVzLnB1c2goeyBTdGF0aWNOYW1lOiBcIl9MYXlvdXRFbGVtZW50XCIsIE5hbWU6IFwiTGF5b3V0IGVsZW1lbnRcIiB9KTtcclxuICAgICAgICAgICAgICAgICAgICAkc2NvcGUuY29udGVudFR5cGVzID0gJGZpbHRlcignb3JkZXJCeScpKCRzY29wZS5jb250ZW50VHlwZXMsICdOYW1lJyk7XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgJHNjb3BlLmxvYWRpbmctLTtcclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIH07XHJcblxyXG4gICAgICAgICRzY29wZS4kd2F0Y2goJ3RlbXBsYXRlSWQnLCBmdW5jdGlvbiAobmV3VGVtcGxhdGVJZCwgb2xkVGVtcGxhdGVJZCkge1xyXG4gICAgICAgIFx0aWYgKG5ld1RlbXBsYXRlSWQgIT0gb2xkVGVtcGxhdGVJZCkge1xyXG4gICAgICAgIFx0XHRpZiAoJHNjb3BlLm1hbmFnZUluZm8uaXNDb250ZW50QXBwKVxyXG4gICAgICAgIFx0XHRcdCRzY29wZS5yZW5kZXJUZW1wbGF0ZShuZXdUZW1wbGF0ZUlkKTtcclxuICAgICAgICBcdFx0ZWxzZSB7XHJcbiAgICAgICAgXHRcdFx0JHNjb3BlLmxvYWRpbmcrKztcclxuXHRcdFx0ICAgICAgICB2YXIgcHJvbWlzZTtcclxuXHRcdFx0ICAgICAgICBpZiAoJHNjb3BlLm1hbmFnZUluZm8uaGFzQ29udGVudClcclxuXHRcdFx0XHQgICAgICAgIHByb21pc2UgPSAkc2NvcGUuc2F2ZVRlbXBsYXRlSWQobmV3VGVtcGxhdGVJZCk7XHJcblx0XHRcdCAgICAgICAgZWxzZVxyXG5cdFx0XHRcdCAgICAgICAgcHJvbWlzZSA9ICRzY29wZS5zZXRQcmV2aWV3VGVtcGxhdGVJZChuZXdUZW1wbGF0ZUlkKTtcclxuXHRcdFx0ICAgICAgICBwcm9taXNlLnRoZW4oZnVuY3Rpb24oKSB7XHJcbiAgICAgICAgXHRcdFx0XHQkd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG5cdFx0XHQgICAgICAgIH0pO1xyXG4gICAgICAgIFx0XHR9XHJcbiAgICAgICAgXHR9XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICRzY29wZS4kd2F0Y2goJ2NvbnRlbnRUeXBlSWQnLCBmdW5jdGlvbiAobmV3Q29udGVudFR5cGVJZCwgb2xkQ29udGVudFR5cGVJZCkge1xyXG4gICAgICAgIFx0aWYgKG5ld0NvbnRlbnRUeXBlSWQgPT0gb2xkQ29udGVudFR5cGVJZClcclxuICAgICAgICBcdFx0cmV0dXJuO1xyXG4gICAgICAgIFx0Ly8gU2VsZWN0IGZpcnN0IHRlbXBsYXRlIGlmIGNvbnRlbnRUeXBlIGNoYW5nZWRcclxuICAgICAgICBcdHZhciBmaXJzdFRlbXBsYXRlSWQgPSAkc2NvcGUuZmlsdGVyZWRUZW1wbGF0ZXMobmV3Q29udGVudFR5cGVJZClbMF0uVGVtcGxhdGVJZDsgLy8gJGZpbHRlcignZmlsdGVyJykoJHNjb3BlLnRlbXBsYXRlcywgeyBBdHRyaWJ1dGVTZXRJZDogJHNjb3BlLmNvbnRlbnRUeXBlSWQgPT0gbnVsbCA/IFwiISFcIiA6ICRzY29wZS5jb250ZW50VHlwZUlkIH0pWzBdLlRlbXBsYXRlSUQ7XHJcbiAgICAgICAgXHRpZiAoJHNjb3BlLnRlbXBsYXRlSWQgIT0gZmlyc3RUZW1wbGF0ZUlkICYmIGZpcnN0VGVtcGxhdGVJZCAhPSBudWxsKVxyXG4gICAgICAgIFx0XHQkc2NvcGUudGVtcGxhdGVJZCA9IGZpcnN0VGVtcGxhdGVJZDtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgaWYgKCRzY29wZS5hcHBJZCAhPSBudWxsICYmICRzY29wZS5tYW5hZ2VJbmZvLnRlbXBsYXRlQ2hvb3NlclZpc2libGUpXHJcbiAgICAgICAgICAgICRzY29wZS5yZWxvYWRUZW1wbGF0ZXMoKTtcclxuXHJcbiAgICAgICAgJHNjb3BlLiR3YXRjaCgnbWFuYWdlSW5mby50ZW1wbGF0ZUNob29zZXJWaXNpYmxlJywgZnVuY3Rpb24odmlzaWJsZSwgb2xkVmlzaWJsZSkge1xyXG4gICAgICAgICAgICBpZiAodmlzaWJsZSAhPSBvbGRWaXNpYmxlICYmICRzY29wZS5hcHBJZCAhPSBudWxsICYmIHZpc2libGUpXHJcbiAgICAgICAgICAgICAgICAkc2NvcGUucmVsb2FkVGVtcGxhdGVzKCk7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICRzY29wZS4kd2F0Y2goJ2FwcElkJywgZnVuY3Rpb24gKG5ld0FwcElkLCBvbGRBcHBJZCkge1xyXG4gICAgICAgICAgICBpZiAobmV3QXBwSWQgPT0gb2xkQXBwSWQgfHwgbmV3QXBwSWQgPT0gbnVsbClcclxuICAgICAgICAgICAgICAgIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIGlmIChuZXdBcHBJZCA9PSAtMSkge1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uID0gJGF0dHJzLmltcG9ydGFwcGRpYWxvZztcclxuICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgbW9kdWxlQXBpLnNldEFwcElkKG5ld0FwcElkKS50aGVuKGZ1bmN0aW9uKCkge1xyXG4gICAgICAgICAgICAgICAgJHdpbmRvdy5sb2NhdGlvbi5yZWxvYWQoKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIGlmICghJHNjb3BlLm1hbmFnZUluZm8uaXNDb250ZW50QXBwKSB7XHJcbiAgICAgICAgICAgIG1vZHVsZUFwaS5nZXRTZWxlY3RhYmxlQXBwcygpLnRoZW4oZnVuY3Rpb24oZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgJHNjb3BlLmFwcHMgPSBkYXRhLmRhdGE7XHJcbiAgICAgICAgICAgICAgICAkc2NvcGUuYXBwcy5wdXNoKHsgTmFtZTogJGF0dHJzLmltcG9ydGFwcHRleHQsIEFwcElkOiAtMSB9KTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAkc2NvcGUuc2V0VGVtcGxhdGVDaG9vc2VyU3RhdGUgPSBmdW5jdGlvbiAoc3RhdGUpIHtcclxuICAgICAgICAgICAgLy8gUmVzZXQgdGVtcGxhdGVpZCAvIGNhbmNlbCB0ZW1wbGF0ZSBjaGFuZ2VcclxuICAgICAgICAgICAgaWYgKCFzdGF0ZSlcclxuICAgICAgICAgICAgICAgICRzY29wZS50ZW1wbGF0ZUlkID0gJHNjb3BlLnNhdmVkVGVtcGxhdGVJZDtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBtb2R1bGVBcGkuc2V0VGVtcGxhdGVDaG9vc2VyU3RhdGUoc3RhdGUpLnRoZW4oZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgJHNjb3BlLm1hbmFnZUluZm8udGVtcGxhdGVDaG9vc2VyVmlzaWJsZSA9IHN0YXRlO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9O1xyXG5cclxuICAgICAgICAkc2NvcGUuc2F2ZVRlbXBsYXRlSWQgPSBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgXHR2YXIgcHJvbWlzZXMgPSBbXTtcclxuXHJcbiAgICAgICAgICAgIHByb21pc2VzLnB1c2gobW9kdWxlQXBpLnNhdmVUZW1wbGF0ZUlkKCRzY29wZS50ZW1wbGF0ZUlkKSk7XHJcblxyXG4gICAgICAgICAgICAkc2NvcGUuc2F2ZWRUZW1wbGF0ZUlkID0gJHNjb3BlLnRlbXBsYXRlSWQ7XHJcbiAgICAgICAgICAgIHByb21pc2VzLnB1c2goJHNjb3BlLnNldFRlbXBsYXRlQ2hvb3NlclN0YXRlKGZhbHNlKSk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gJHEuYWxsKHByb21pc2VzKTtcclxuICAgICAgICB9O1xyXG5cclxuXHQgICAgJHNjb3BlLnNldFByZXZpZXdUZW1wbGF0ZUlkID0gZnVuY3Rpb24oKSB7XHJcblx0XHQgICAgcmV0dXJuIG1vZHVsZUFwaS5zZXRQcmV2aWV3VGVtcGxhdGVJZCgkc2NvcGUudGVtcGxhdGVJZCk7XHJcblx0ICAgIH07XHJcblxyXG4gICAgICAgICRzY29wZS5yZW5kZXJUZW1wbGF0ZSA9IGZ1bmN0aW9uICh0ZW1wbGF0ZUlkKSB7XHJcbiAgICAgICAgICAgICRzY29wZS5sb2FkaW5nKys7XHJcbiAgICAgICAgICAgIG1vZHVsZUFwaS5yZW5kZXJUZW1wbGF0ZSh0ZW1wbGF0ZUlkKS50aGVuKGZ1bmN0aW9uIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgICAgICAgICAkc2NvcGUuaW5zZXJ0UmVuZGVyZWRUZW1wbGF0ZShyZXNwb25zZS5kYXRhKTtcclxuICAgICAgICAgICAgICAgICAgICAkMnN4Yyhtb2R1bGVJZCkubWFuYWdlLl9wcm9jZXNzVG9vbGJhcnMoKTtcclxuICAgICAgICAgICAgICAgIH0gY2F0Y2ggKGUpIHtcclxuICAgICAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcIkVycm9yIHdoaWxlIHJlbmRlcmluZyB0ZW1wbGF0ZTpcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coZSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAkc2NvcGUubG9hZGluZy0tO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9O1xyXG5cclxuICAgICAgICAkc2NvcGUuaW5zZXJ0UmVuZGVyZWRUZW1wbGF0ZSA9IGZ1bmN0aW9uKHJlbmRlcmVkVGVtcGxhdGUpIHtcclxuICAgICAgICAgICAgJChcIi5Ebm5Nb2R1bGUtXCIgKyBtb2R1bGVJZCArIFwiIC5zYy12aWV3cG9ydFwiKS5odG1sKHJlbmRlcmVkVGVtcGxhdGUpO1xyXG4gICAgICAgIH07XHJcblxyXG4gICAgICAgICRzY29wZS5hZGRJdGVtID0gZnVuY3Rpb24oc29ydE9yZGVyKSB7XHJcbiAgICAgICAgICAgIG1vZHVsZUFwaS5hZGRJdGVtKHNvcnRPcmRlcikudGhlbihmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAkc2NvcGUucmVuZGVyVGVtcGxhdGUoJHNjb3BlLnRlbXBsYXRlSWQpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9O1xyXG5cclxuICAgIH0pO1xyXG5cclxuICAgIG1vZHVsZS5mYWN0b3J5KCdtb2R1bGVBcGlTZXJ2aWNlJywgZnVuY3Rpb24oYXBpU2VydmljZSkge1xyXG4gICAgICAgIHJldHVybiBmdW5jdGlvbihtb2R1bGVJZCkge1xyXG4gICAgICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICAgICAgc2F2ZVRlbXBsYXRlSWQ6IGZ1bmN0aW9uKHRlbXBsYXRlSWQpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1cmw6ICdWaWV3L01vZHVsZS9TYXZlVGVtcGxhdGVJZCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcmFtczogeyB0ZW1wbGF0ZUlkOiB0ZW1wbGF0ZUlkIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIFx0c2V0UHJldmlld1RlbXBsYXRlSWQ6IGZ1bmN0aW9uKHRlbXBsYXRlSWQpIHtcclxuICAgICAgICAgICAgXHRcdHJldHVybiBhcGlTZXJ2aWNlKG1vZHVsZUlkLCB7XHJcbiAgICAgICAgICAgIFx0XHRcdHVybDogJ1ZpZXcvTW9kdWxlL1NldFByZXZpZXdUZW1wbGF0ZUlkJyxcclxuICAgICAgICAgICAgXHRcdFx0cGFyYW1zOiB7IHRlbXBsYXRlSWQ6IHRlbXBsYXRlSWQgfVxyXG4gICAgICAgICAgICBcdFx0fSk7XHJcblx0ICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGFkZEl0ZW06IGZ1bmN0aW9uKHNvcnRPcmRlcikge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBhcGlTZXJ2aWNlKG1vZHVsZUlkLCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHVybDogJ1ZpZXcvTW9kdWxlL0FkZEl0ZW0nLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJhbXM6IHsgc29ydE9yZGVyOiBzb3J0T3JkZXIgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGdldFNlbGVjdGFibGVBcHBzOiBmdW5jdGlvbigpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1cmw6ICdWaWV3L01vZHVsZS9HZXRTZWxlY3RhYmxlQXBwcydcclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBzZXRBcHBJZDogZnVuY3Rpb24oYXBwSWQpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1cmw6ICdWaWV3L01vZHVsZS9TZXRBcHBJZCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcmFtczogeyBhcHBJZDogYXBwSWQgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGdldFNlbGVjdGFibGVDb250ZW50VHlwZXM6IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gYXBpU2VydmljZShtb2R1bGVJZCwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1cmw6ICdWaWV3L01vZHVsZS9HZXRTZWxlY3RhYmxlQ29udGVudFR5cGVzJ1xyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGdldFNlbGVjdGFibGVUZW1wbGF0ZXM6IGZ1bmN0aW9uKCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBhcGlTZXJ2aWNlKG1vZHVsZUlkLCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHVybDogJ1ZpZXcvTW9kdWxlL0dldFNlbGVjdGFibGVUZW1wbGF0ZXMnXHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgc2V0VGVtcGxhdGVDaG9vc2VyU3RhdGU6IGZ1bmN0aW9uKHN0YXRlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGFwaVNlcnZpY2UobW9kdWxlSWQsIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiAnVmlldy9Nb2R1bGUvU2V0VGVtcGxhdGVDaG9vc2VyU3RhdGUnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJhbXM6IHsgc3RhdGU6IHN0YXRlIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICByZW5kZXJUZW1wbGF0ZTogZnVuY3Rpb24odGVtcGxhdGVJZCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBhcGlTZXJ2aWNlKG1vZHVsZUlkLCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHVybDogJ1ZpZXcvTW9kdWxlL1JlbmRlclRlbXBsYXRlJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFyYW1zOiB7IHRlbXBsYXRlSWQ6IHRlbXBsYXRlSWQgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgIH07XHJcbiAgICB9KTtcclxuXHJcbn0pKCk7Il19