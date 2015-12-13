(function () {
    var module = angular.module("2sxc.view");

    module.factory("moduleApiService", function($http) {
        return {

            saveTemplate: function (templateId, forceCreateContentGroup, newTemplateChooserState) {
                return $http.get("View/Module/SaveTemplateId", { params: {
                    templateId: templateId,
                    forceCreateContentGroup: forceCreateContentGroup,
                    newTemplateChooserState: newTemplateChooserState
                } });
            },

            addItem: function(sortOrder) {
                return $http.get("View/Module/AddItem", { params: { sortOrder: sortOrder } });
            },

            getSelectableApps: function() {
                return $http.get("View/Module/GetSelectableApps");
            },

            setAppId: function(appId) {
                return $http.get("View/Module/SetAppId", { params: { appId: appId } });
            },

            getSelectableContentTypes: function() {
                return $http.get("View/Module/GetSelectableContentTypes");
            },

            getSelectableTemplates: function() {
                return $http.get("View/Module/GetSelectableTemplates");
            },

            setTemplateChooserState: function(state) {
                return $http.get("View/Module/SetTemplateChooserState", { params: { state: state } });
            },

            renderTemplate: function(templateId) {
                return $http.get("View/Module/RenderTemplate", { params: { templateId: templateId } });
            },

            changeOrder: function(sortOrder, destinationSortOrder) {
                return $http.get("View/Module/ChangeOrder",
                { params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder } });
            },

            publish: function(part, sortOrder) {
                return $http.get("view/module/publish", { params: { part: part, sortOrder: sortOrder } });
            },

            removeFromList: function(sortOrder) {
                return $http.get("View/Module/RemoveFromList", { params: { sortOrder: sortOrder } });
            }
        };
    });

})();