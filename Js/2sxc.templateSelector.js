(function () {
    var module = angular.module('2sxc.view', []);

    module.controller('TemplateSelectorCtrl', function ($scope, $attrs, apiService) {

        var moduleId = $attrs.moduleid;
        
        $scope.contentTypes = [];
        $scope.templates = [];
        $scope.selectedContentType = null;
        $scope.selectedTemplate = null;

        apiService(moduleId, {
            url: 'View/Module/GetSelectableContentTypes'
        }).then(function(data) {
            $scope.contentTypes = data.data;
        });

        apiService(moduleId, {
            url: 'View/Module/GetSelectableTemplates',
            params: {
                attributeSetId: null
            }
        }).then(function(data) {
            $scope.templates = data.data;
        });

    });

    module.factory('apiService', function ($http) {

        return function (moduleId, settings) {

            var sf = $.ServicesFramework(moduleId);

            // Prepare HTTP headers for DNN Web API
            var headers = {
                ModuleId: sf.getModuleId(),
                TabId: sf.getTabId(),
                RequestVerificationToken: sf.getAntiForgeryValue()
            };

            settings.headers = $.extend({}, settings.headers, headers);
            settings.url = sf.getServiceRoot('2sxc') + settings.url;
            settings.params = $.extend({}, settings.params);
            return $http(settings);
        }
    });

})();


