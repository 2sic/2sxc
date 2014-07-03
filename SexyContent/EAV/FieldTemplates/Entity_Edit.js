(function() {
    angular.module('2sic-EAV', ['ui.tree'])
        .controller('EntityEditCtrl', function($scope) {
            $scope.configuration = {};
            $scope.selectedEntity = "";
            $scope.entityIds = function () {
                return $scope.configuration.SelectedEntities.join(',');
            };
            $scope.AddEntity = function () {
                $scope.configuration.SelectedEntities.push(parseInt($scope.selectedEntity));
                $scope.selectedEntity = "";
            };
    });

})();