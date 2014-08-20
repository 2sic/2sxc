// The EavApiService wraps $http while making sure the base url is
// correct and adding some default parameters to every web service call

(function () {
    angular.module('2sic-EAV')
        .factory('eavApiService', function (eavGlobalConfigurationProvider, $http) {
            return function(settings) {
                settings.url = eavGlobalConfigurationProvider.apiBaseUrl + settings.url;
                settings.params = $.extend({}, eavGlobalConfigurationProvider.defaultApiParams, settings.params);
                return $http(settings);
            }
        });
})();