/*
    Extending 2sxc with angular capabilities
    In general, this should automatically take care of everything just by including it in your sources. 
    Make sure it's added after AngularJS and after the 2sxc.api.js
    It will then look for all sxc-apps and initialize them, ensuring that $http is pre-configured to work with DNN
*/
$2sxc.ng = {
    appAttribute: 'sxc-app',
    ngAttrPrefixes: ['ng-', 'data-ng-', 'ng:', 'x-ng-'],
    iidAttrNames: ['app-instanceid', 'data-instanceid', 'id'],

    // bootstrap: an App-Start-Help; normally you won't call this manually as it will be auto-bootstrapped. 
    // All params optional except for 'element'
    bootstrap: function (element, ngModName, iid, dependencies, config) {
        // first, try to get moduleId from URL
        
        iid = iid || $2sxc.ng.findInstanceId(element) || $2sxc.ng.getParameterByName('mid'); // use fn-param, or get from DOM, or get url-param
        var sf = $.ServicesFramework(iid);

        // create a micro-module to configure sxc-init parameters, add to dependencies. Note that the order is important!
        angular.module('confSxcApp' + iid, [])
            // .constant('iid', iid)
            .constant('AppInstanceId', iid)
            .constant('AppServiceFramework', sf)
            .constant('HttpHeaders', { "ModuleId": iid, "TabId": sf.getTabId(), "RequestVerificationToken": sf.getAntiForgeryValue() });
        var allDependencies = ['confSxcApp' + iid, '2sxc4ng'].concat(dependencies || [ngModName]);

        angular.element(document).ready(function () {
            angular.bootstrap(element, allDependencies, config); // start the app
        });
    },

    // find instance Id in an attribute of the tag - typically with id="app-700" or something and use the number as IID
    findInstanceId: function findInstanceId(element) {
        var attrib, ngElement = angular.element(element);
        for (var i = 0; i < $2sxc.ng.iidAttrNames.length; i++)
            attrib = ngElement.attr($2sxc.ng.iidAttrNames[i]);
        if (attrib) {
            var iid = parseInt(attrib.toString().replace(/\D/g, '')); // filter all characters if necessary
            if (!iid) throw "iid or instanceId (the DNN moduleid) not supplied and automatic lookup failed. Please set app-tag attribute iid or give id in bootstrap call";
            return iid;
        }
    },

    // Auto-bootstrap all sub-tags having an 'sxc-app' attribute - for Multiple-Apps-per-Page
    bootstrapAll: function bootstrapAll(element) {
        element = element || document;
        var allAppTags = element.querySelectorAll('[' + $2sxc.ng.appAttribute + ']');
        angular.forEach(allAppTags, function (appTag) {
            var ngModName = appTag.getAttribute($2sxc.ng.appAttribute);
            var configDependencyInjection = { strictDi: $2sxc.ng.getNgAttribute(appTag, "strict-di") !== null };
            $2sxc.ng.bootstrap(appTag, ngModName, null, null, configDependencyInjection);
        });
    },

    // if the page contains angular, do auto-bootstrap of all 2sxc apps
    autoRunBootstrap: function autoRunBootstrap() {
        if (angular)
            angular.element(document).ready(function () {
                $2sxc.ng.bootstrapAll();
            });
    },

    // Helper function to try various attribute-prefixes
    getNgAttribute: function getNgAttribute(element, ngAttr) {
        var attr, i, ii = $2sxc.ng.ngAttrPrefixes.length;
        element = angular.element(element);
        for (i = 0; i < ii; ++i) {
            attr = $2sxc.ng.ngAttrPrefixes[i] + ngAttr;
            if (typeof (attr = element.attr(attr)) == 'string')
                return attr;
        }
        return null;
    },

    // get url param - mainly needed for mid=### in admin-dialogs
    getParameterByName: function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);

        // if nothing found, try normal URL because DNN places parameters in /key/value notation
        if (results === null) {
            // Otherwise try parts of the URL
            var matches = window.location.pathname.match("/" + name + "/([^/]+)", 'i');

            // Check if we found anything, if we do find it, we must reverse the results so we get the "last" one in case there are multiple hits
            if (matches !== null && matches.length > 1) 
                results = matches.reverse()[0]; 
        } else 
            results = results[1];
        
        return results === null ? "" : decodeURIComponent(results.replace(/\+/g, " "));
    }

};
$2sxc.ng.autoRunBootstrap();

angular.module('2sxc4ng', ['ng'])
    // Configure $http for DNN web services (security tokens etc.)
    .config(function ($httpProvider, HttpHeaders) {
        angular.extend($httpProvider.defaults.headers.common, HttpHeaders);
        $httpProvider.interceptors.push(function ($q, sxc) {
            return {
                // Rewrite 2sxc-urls if necessary
                'request': function (config) {
                    config.url = sxc.resolveServiceUrl(config.url);
                    return config;
                },

                // Show very nice error if necessary
                'responseError': function (rejection) {
                    sxc.showDetailedHttpError(rejection);
                    return $q.reject(rejection);
                }
            };
        });

    })

    // Provide the sxc helper for this module
    .factory('sxc', function (AppInstanceId) {
        console.log('creating sxc service for id: ' + AppInstanceId);
        if (!$2sxc) throw "the Angular service 'sxc' can't find the global $2sxc controller";
        var ngSxc = $2sxc(AppInstanceId);    // make this service be the 2sxc-controller for this module
        return ngSxc;
    })

    /// Standard entity commands like get one, many etc.
    .factory('content', function ($http) {
        // construct a service just for this content-type
            return function(contentType) {
                var oneType = {};
                oneType.contentType = contentType;
                oneType.root = 'app-content/' + contentType;

                // will get one or all of a content-type, depending on if an id was supplied
                oneType.get = oneType.read = function get(id) {
                    return $http.get(oneType.root + (id ? '/' + id : ''));
                };
                oneType.create = function create(values) {
                    return $http.post(oneType.root, values);
                };
                oneType.update = function update(values, id) {
                    var realId = id || values.Id || values.id;  // automatically use the correct Id
                    return $http.post(oneType.root + '/' + realId, values);
                };
                oneType.delete = function del(id) {
                    return $http.delete(oneType.root + '/' + id);
                };
                return oneType;
            };
        })

    .factory('query', function ($http) {
        return function(name) {
            var qry = this;
            qry.root = 'app-query/' + name;
            qry.get = function() {
                return $http.get(qry.root);
            };
        };
    })

/* Todo: future feature
.factory('sxcResource', function(iid, $injector) {
    return function (url, paramDefaults, actions, options) {

//        // todo: check if resource is loaded
        // manually get injector, to prevent required dependency and give nice error
        var inj = angular.injector(['ngResource']);
        if (!inj)
            throw 'Error: sxcResource only works if the page also includes angular-resource. So you must either include that, or you should use jQuery AJAX instead.';
        var res = inj.get('$resource')
        
        
        return res(url, paramDefaults, actions, option);
    }
})
*/
;