(function () {
    var module = angular.module("2sxc.view");

    module.factory("moduleApiService", function($http) {
        return function (moduleId) {

            function apiService(modId, settings) {
                return $http(settings);
            }

            return {
                saveTemplateId: function(templateId) {
                    return apiService(moduleId, {
                        url: "View/Module/SaveTemplateId",
                        params: { templateId: templateId }
                    });
                },
            	setPreviewTemplateId: function(templateId) {
            		return apiService(moduleId, {
            			url: "View/Module/SetPreviewTemplateId",
            			params: { templateId: templateId }
            		});
	            },
                addItem: function(sortOrder) {
                    return apiService(moduleId, {
                        url: "View/Module/AddItem",
                        params: { sortOrder: sortOrder }
                    });
                },
                getSelectableApps: function() {
                    return apiService(moduleId, {
                        url: "View/Module/GetSelectableApps"
                    });
                },
                setAppId: function(appId) {
                    return apiService(moduleId, {
                        url: "View/Module/SetAppId",
                        params: { appId: appId }
                    });
                },
                getSelectableContentTypes: function () {
                    return apiService(moduleId, {
                        url: "View/Module/GetSelectableContentTypes"
                    });
                },
                getSelectableTemplates: function() {
                    return apiService(moduleId, {
                        url: "View/Module/GetSelectableTemplates"
                    });
                },
                setTemplateChooserState: function(state) {
                    return apiService(moduleId, {
                        url: "View/Module/SetTemplateChooserState",
                        params: { state: state }
                    });
                },
                renderTemplate: function(templateId) {
                    return apiService(moduleId, {
                        url: "View/Module/RenderTemplate",
                        params: { templateId: templateId }
                    });
                },
                changeOrder: function (sortOrder, destinationSortOrder) {
                	return apiService(moduleId, {
                		url: "View/Module/ChangeOrder",
                		params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder }
                	});
                },
                publish: function(part, sortOrder) {
                 	return apiService(moduleId, {
                		url: "view/module/publish",
                		params: { part: part, sortOrder: sortOrder }
                	});
                },
                removeFromList: function (sortOrder) {
                	return apiService(moduleId, {
                		url: "View/Module/RemoveFromList",
                		params: { sortOrder: sortOrder }
                	});
                }
            };
        };
    });

})();