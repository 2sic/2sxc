(function () {
    var module = angular.module('2sxc.view', []);

    module.controller('TemplateSelectorCtrl', function ($scope, $attrs, apiService, $filter) {

        var moduleId = $attrs.moduleid;
        $scope.manageInfo = $2sxc(moduleId).manage._manageInfo;

        $scope.contentTypes = [];
        $scope.templates = [];
        $scope.filteredTemplates = function() {
            return $filter('filter')($scope.templates, { AttributeSetID: $scope.contentTypeId });
        };
        $scope.contentTypeId = null;
        $scope.templateId = $scope.manageInfo.templateId;

        $scope.$watch('templateId', function (newTemplateId, oldTemplateId) {
            if (newTemplateId == null) {
                var firstTemplateId = $filter('filter')($scope.templates, { AttributeSetID: $scope.contentTypeId })[0].TemplateID;
                if ($scope.templateId != firstTemplateId)
                    $scope.templateId = firstTemplateId;
            }
            else if (newTemplateId != oldTemplateId) {
                $scope.renderTemplate(newTemplateId);
            }
        });

        $scope.$watch('contentTypeId', function (newContentTypeId, oldContentTypeId) {
            if (newContentTypeId != null) {
                $scope.templateId = null;
            }
        });

        apiService(moduleId, {
            url: 'View/Module/GetSelectableContentTypes'
        }).then(function(data) {
            $scope.contentTypes = data.data;
        });

        apiService(moduleId, {
            url: 'View/Module/GetSelectableTemplates'
        }).then(function(data) {
            $scope.templates = data.data;
            var template = $filter('filter')($scope.templates, { TemplateID: $scope.templateId });
            if (template[0] != null && $scope.contentTypeId == null)
                $scope.contentTypeId = template[0].AttributeSetID;
        });


        $scope.setTemplateChooserState = function (state) {
            apiService(moduleId, {
                url: 'View/Module/SetTemplateChooserState',
                params: { state: state }
            });
        };

        $scope.saveTemplateId = function(templateId) {
            apiService(moduleId, {
                url: 'View/ContentGroup/SaveTemplateId',
                params: { templateId: templateId }
            });
        };

        $scope.renderTemplate = function(templateId) {
            apiService(moduleId, {
                url: 'View/Module/RenderTemplate',
                params: { templateId: templateId }
            }).then(function (response) {
                $scope.insertRenderedTemplate(response.data);
                $2sxc(moduleId).manage._processToolbars();
            });
        };

        $scope.insertRenderedTemplate = function(renderedTemplate) {
            $(".DnnModule-" + moduleId + " .sc-viewport").html(renderedTemplate);
        };

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


