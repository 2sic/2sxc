/*
    This is the bootsrapper for the EAV system
    It auto-bootstraps things with an attribute eav-app
    And will auto-configure $http to work with eav-urls

    In the live system it should usually not be needed or be replaced with a bootstrapper optimized to the environment (like 2sxc)

*/

$eav4ng = {
    appAttribute: 'eav-app',
    ngAttrPrefixes: ['ng-', 'data-ng-', 'ng:', 'x-ng-'],

    // bootstrap: an App-Start-Help; normally you won't call this manually as it will be auto-bootstrapped. 
    // All params optional except for 'element'
    bootstrap: function (element, ngModName, iid, dependencies, config) {
        var allDependencies = ['eav4ng'].concat(dependencies || [ngModName]);

        angular.element(document).ready(function () {
            angular.bootstrap(element, allDependencies, config); // start the app
        });
    },

    // Auto-bootstrap all sub-tags having an 'sxc-app' attribute - for Multiple-Apps-per-Page
    bootstrapAll: function bootstrapAll(element) {
        element = element || document;
        var allAppTags = element.querySelectorAll('[' + $eav4ng.appAttribute + ']');
        angular.forEach(allAppTags, function (appTag) {
            var ngModName = appTag.getAttribute($eav4ng.appAttribute);
            var configDependencyInjection = { strictDi: $eav4ng.getNgAttribute(appTag, "strict-di") !== null };
            $eav4ng.bootstrap(appTag, ngModName, null, null, configDependencyInjection);
        });
    },

    // if the page contains angular, do auto-bootstrap of all 2sxc apps
    autoRunBootstrap: function autoRunBootstrap() {
        if (angular)
            angular.element(document).ready(function () {
                $eav4ng.bootstrapAll();
            });
    },

    // Helper function to try various attribute-prefixes
    getNgAttribute: function getNgAttribute(element, ngAttr) {
        var attr, i, ii = $eav4ng.ngAttrPrefixes.length;
        element = angular.element(element);
        for (i = 0; i < ii; ++i) {
            attr = $eav4ng.ngAttrPrefixes[i] + ngAttr;
            if (typeof (attr = element.attr(attr)) == 'string')
                return attr;
        }
        return null;
    }
};
$eav4ng.autoRunBootstrap();

angular.module('eav4ng', ['ng'])
    // Configure $http for DNN web services (security tokens etc.)
    .config(function ($httpProvider) {
        // angular.extend($httpProvider.defaults.headers.common, HttpHeaders);
        $httpProvider.interceptors.push(function ($q) {
            return {
                // Rewrite 2sxc-urls if necessary
                'request': function (config) {
                    function resolveServiceUrl(virtualPath) {
                        var serviceScopes = ['app-api', 'app-query', 'app-content', 'eav']; // todo7: should probably deprecate "app"
                        var serviceRoot = "/api/";// $.ServicesFramework(id).getServiceRoot('2sxc'),

                        var scope = virtualPath.split('/')[0].toLowerCase();
                        // stop if it's not one of our special paths
                        if (serviceScopes.indexOf(scope) == -1)
                            return virtualPath;

                        // if (scope.indexOf('app-api') > -1 /* && scope.indexOf('app-content')!=0 */)  scope += "/auto-detect-app";
                        return serviceRoot + virtualPath;// '/' + virtualPath.substring(virtualPath.indexOf('/') + 1);
                    }
                            
                    config.url = resolveServiceUrl(config.url);
                    return config;
                },

                // Show very nice error if necessary
                'responseError': function (rejection) {
                    alert('Error: ' + rejection);
                    return $q.reject(rejection);
                }
            };
        });

    })
