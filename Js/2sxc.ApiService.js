(function () {
    
    var module = angular.module('2sxc.api', []);

    module.factory('apiService', function ($http, $window) {

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

            var apiPromise = $http(settings);

            // Catch, log and show errors
            return apiPromise.catch(function (reason) {
                if ($window.console)
                    $window.console.log(reason);
                var error = "Error: ";
                error += (reason.data && reason.data.ExceptionMessage) ? reason.data.ExceptionMessage : "Unkown";
                error += "\r\nThe error has also been logged to the console.";
                $window.alert(error);
            });
        }
    });

})();