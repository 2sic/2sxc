/*
    Extending 2sxc with angular capabilities
    In general, this should automatically take care of everything just by including it in your sources. 
    Make sure it's added after AngularJS and after the 2sxc.api.js
    It will then look for all sxc-apps and initialize them, ensuring that $http is pre-configured to work with DNN
*/
$2sxc.ng = {
    appAttribute: "sxc-app",
    ngAttrPrefixes: ["ng-", "data-ng-", "ng:", "x-ng-"],
    iidAttrNames: ["app-instanceid", "data-instanceid", "id"],
    ngAttrDependencies: "dependencies", 

    // bootstrap: an App-Start-Help; normally you won't call this manually as it will be auto-bootstrapped. 
    // All params optional except for 'element'
    bootstrap: function (element, ngModName, iid, dependencies, config) {
        // first, try to get moduleId from function-param or from from URL
        iid = iid || $2sxc.ng.findInstanceId(element) || $2sxc.urlParams.get("mid"); 

        // then provide access to the dnn-services framework (or a fake thereof)
        var sf = $.ServicesFramework(iid);

        // create a micro-module to configure sxc-init parameters, add to dependencies. Note that the order is important!
        angular.module("confSxcApp" + iid, [])
            .constant("AppInstanceId", iid)
            .constant("AppServiceFramework", sf)
            .constant("HttpHeaders", { "ModuleId": iid, "TabId": sf.getTabId(), "RequestVerificationToken": sf.getAntiForgeryValue() });
        var allDependencies = ["confSxcApp" + iid, "2sxc4ng"].concat(dependencies || [ngModName]);

        angular.element(document).ready(function () {
			try {
				angular.bootstrap(element, allDependencies, config); // start the app
			} catch (e) { // Make sure that if one app breaks, others continue to work
				if (console && console.error)
				    console.error(e);
			}
        });
    },

    // find instance Id in an attribute of the tag - typically with id="app-700" or something and use the number as IID
    findInstanceId: function findInstanceId(element) {
        var attrib, ngElement = angular.element(element);
        for (var i = 0; i < $2sxc.ng.iidAttrNames.length; i++)
            attrib = ngElement.attr($2sxc.ng.iidAttrNames[i]);
        if (attrib) {
            var iid = parseInt(attrib.toString().replace(/\D/g, "")); // filter all characters if necessary
            if (!iid) throw "iid or instanceId (the DNN moduleid) not supplied and automatic lookup failed. Please set app-tag attribute iid or give id in bootstrap call";
            return iid;
        }
    },

    // Auto-bootstrap all sub-tags having an 'sxc-app' attribute - for Multiple-Apps-per-Page
    bootstrapAll: function bootstrapAll(element) {
        element = element || document;
        var allAppTags = element.querySelectorAll("[" + $2sxc.ng.appAttribute + "]");
        angular.forEach(allAppTags, function (appTag) {
		    var ngModName = appTag.getAttribute($2sxc.ng.appAttribute);
		    var configDependencyInjection = { strictDi: $2sxc.ng.getNgAttribute(appTag, "strict-di") !== null };
            // new 2015-09-05
		    var dependencies = $2sxc.ng.getNgAttribute(appTag, $2sxc.ng.ngAttrDependencies);
		    if (dependencies) dependencies = dependencies.split(",");
		    $2sxc.ng.bootstrap(appTag, ngModName, null, dependencies, configDependencyInjection);
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
            if (typeof (attr = element.attr(attr)) == "string")
                return attr;
        }
        return null;
    },
};
$2sxc.ng.autoRunBootstrap();

angular.module("2sxc4ng", ["ng"])
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

    // provide the global $2sxc object to angular modules as a clear, clean dependency
    .factory("$2sxc", function () {
        if (!$2sxc) throw "the Angular service 'sxc' can't find the global $2sxc controller";
        return $2sxc;
    })

    // Provide the app-specific sxc helper for this module
    .factory("sxc", function (AppInstanceId, $2sxc) {
        if(window.console) console.log("creating sxc service for id: " + AppInstanceId);
        var ngSxc = $2sxc(AppInstanceId);    // make this service be the 2sxc-controller for this module
        return ngSxc;
    })


    /// Standard entity commands like get one, many etc.
    .factory("content", function ($http) {
        // construct a service just for this content-type
        return function (contentType) {
            var oneType = {};
            oneType.contentType = contentType;
            oneType.root = "app-content/" + contentType;

            // will get one or all of a content-type, depending on if an id was supplied
            oneType.get = oneType.read = function get(id) {
                return $http.get(oneType.root + (id ? "/" + id : ""));
            };
            oneType.create = function create(values) {
                return $http.post(oneType.root, values);
            };
            oneType.update = oneType.patch = function update(values, id) {
                var realId = id || values.Id || values.id;  // automatically use the correct Id
                return $http.post(oneType.root + "/" + realId, values);
            };
            oneType.delete = function del(id) {
                return $http.delete(oneType.root + "/" + id);
            };
            return oneType;
        };
    })

    /// simple helper service which will call a query
    .factory("query", function ($http) {
        return function (name) {
            var qry = {};
            qry.root = "app-query/" + name;

            qry.get = function (config) {
                return $http.get(qry.root, config);
            };
            return qry;
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