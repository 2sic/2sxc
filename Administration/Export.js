(function() {
    angular.module('2sxc-Export', [])
        .controller('ExportController', function($scope, $filter) {
            $scope.data = [];
            $scope.init = function(data) {
                $scope.data = data;
            };

            $scope.selectedContentTypesString = function() {
                var selectedContentTypes = $filter('filter')($scope.data, { _2sxcExport: true });
                return $.map(selectedContentTypes, function(e, i) { return e.Id; }).join(",");
            };

            $scope.selectedTemplatesString = function () {
                var selectedTemplates = [];
                $.each($scope.data, function(i,e) {
                    selectedTemplates = selectedTemplates.concat($filter('filter')(e.Templates, { _2sxcExport: true }));
                });
                return $.map(selectedTemplates, function (e, i) { return e.TemplateID; }).join(",");
            };

            $scope.selectedEntitiesString = function() {
                var selectedEntities = [];
                $.each($scope.data, function (i, e) {
                    selectedEntities = selectedEntities.concat($filter('filter')(e.Entities, { _2sxcExport: true }));
                });
                return $.map(selectedEntities, function (e, i) { return e.EntityId; }).join(",");
            };
    });
})();