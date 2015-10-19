/* 
 * Field: Custom - Default (basically something you should never see)
 */

angular.module("eavFieldTemplates")
    .config(["formlyConfigProvider", "defaultFieldWrappers", function(formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "custom-gps",
            templateUrl: "fields/custom-gps/custom-gps.html",
            wrapper: defaultFieldWrappers,
            controller: "FieldTemplate-CustomGpsController"
        });
    }])
    .controller("FieldTemplate-CustomGpsController", ["$scope", "$filter", "$modal", "appId", "debugState", "eavAdminDialogs", function ($scope, $filter, $modal, appId, debugState, eavAdminDialogs) {
        $scope.debug = debugState;
        // try to find the settings, where to copy the field to...
        $scope.latField = "";
        $scope.LongField = "";

        var controlSettings = $scope.to.settings["custom-gps"];
        if (controlSettings) {
            $scope.latField = controlSettings.LatField || null;
        }
        // alert('gps' + $scope.latField);

    }]);
angular.module('eavEditExtended',[]).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('fields/custom-gps/custom-gps.html',
    "<div><div class=\"alert alert-danger\">GPS-Picker 2 - not implemented yet <input class=\"form-control input-lg\" ng-pattern=vm.regexPattern ng-model=value.Value></div><div ng-if=debug.on><h4>debug info</h4><div>lat field name: '{{ latField}}' long-field name: '{{longField}}'</div></div></div>"
  );

}]);
