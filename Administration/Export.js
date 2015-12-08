(function() {
    angular.module('2sxc-Export', [])
        .controller('ExportController', function($scope, $filter) {
            //$scope.data = [];
            $scope.contentTypes = [];
            $scope.templatesWithoutContentType = [];

            $scope.init = function (data) {
                $scope.contentTypes = data.contentTypes;
                $scope.templatesWithoutContentType = data.templatesWithoutContentType;
            };

            $scope.selectedContentTypesString = function() {
                var selectedContentTypes = $filter('filter')($scope.contentTypes, { _2sxcExport: true });
                return $.map(selectedContentTypes, function(e, i) { return e.Id; }).join(",");
            };

            $scope.selectedTemplatesString = function () {
                var selectedTemplates = [];
                $.each($scope.contentTypes, function (i, e) {
                    selectedTemplates = selectedTemplates.concat($filter('filter')(e.Templates, { _2sxcExport: true }));
                });
                selectedTemplates = selectedTemplates.concat($filter('filter')($scope.templatesWithoutContentType, { _2sxcExport: true }));
                return $.map(selectedTemplates, function (e, i) { return e.TemplateId; }).join(",");
            };

            $scope.selectedEntitiesString = function() {
                var selectedEntities = [];
                $.each($scope.contentTypes, function (i, e) {
                    selectedEntities = selectedEntities.concat($filter('filter')(e.Entities, { _2sxcExport: true }));
                });
                return $.map(selectedEntities, function (e, i) { return e.EntityId; }).join(",");
            };

            // Returns only templateDefaults with a DemoEntityId
            $scope.templateDefaultFilter = function (templateDefaults) {
                return $filter('filter')(templateDefaults, function(templateDefault) {
                    return templateDefault.DemoEntityID != 0 && templateDefault.DemoEntityID != null;
                });
            };
    });
})();