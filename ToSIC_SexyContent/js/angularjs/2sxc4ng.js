/*
    Extending 2sxc with angular capabilities
    This is an angular module which provides all 2sxc-information and services to an angular app
    It will be auto-added to your app when you use the bootstrapping
*/
(function (angular) {
    angular.module("2sxc4ng", ["ng"])
        // Configure $http for DNN web services (security tokens etc.)
        // ReSharper disable InconsistentNaming
        .config(["$httpProvider", "HttpHeaders", function($httpProvider, HttpHeaders) {
        // ReSharper restore InconsistentNaming
            angular.extend($httpProvider.defaults.headers.common, HttpHeaders);
            $httpProvider.interceptors.push(["$q", "sxc", function($q, sxc) {
                return {
                    // Rewrite 2sxc-urls if necessary
                    'request': function(config) {
                        config.url = sxc.resolveServiceUrl(config.url);
                        return config;
                    },

                    // Show very nice error if necessary
                    'responseError': function(rejection) {
                        if (!rejection.config.ignoreErrors)
                            sxc.showDetailedHttpError(rejection);
                        return $q.reject(rejection);
                    }
                };
            }]);

        }])

        // provide the global $2sxc object to angular modules as a clear, clean dependency
        .factory("$2sxc", function() {
            if (!$2sxc) throw "the Angular service '$2sxc' in module '2sxc4ng' can't find the global $2sxc controller";
            return $2sxc;
        })

        // Provide the app-specific sxc helper for this module
        // ReSharper disable InconsistentNaming
        .factory("sxc", ["AppInstanceId", "$2sxc", function(AppInstanceId, $2sxc) {
        // ReSharper restore InconsistentNaming
            // if (window.console) console.log("creating sxc service for id: " + AppInstanceId);
            var ngSxc = $2sxc(AppInstanceId); // make this service be the 2sxc-controller for this module
            return ngSxc;
        }])


        /// Standard entity commands like get one, many etc.
        .factory("content", ["$http", function($http) {
            // construct a service just for this content-type
            return function(contentType) {
                var oneType = {};
                oneType.contentType = contentType;
                oneType.root = "app/auto/content/" + contentType;

                // will get one or all of a content-type, depending on if an id was supplied
                oneType.get = oneType.read = function get(id) {
                    return $http.get(oneType.root + (id ? "/" + id : ""));
                };
                oneType.create = function create(values) {
                    return $http.post(oneType.root, values);
                };
                oneType.update = oneType.patch = function update(values, id) {
                    var realId = id || values.Id || values.id; // automatically use the correct Id
                    return $http.post(oneType.root + "/" + realId, values);
                };
                oneType.delete = function del(id) {
                    return $http.delete(oneType.root + "/" + id);
                };
                return oneType;
            };
        }])

        /// simple helper service which will call a query
        .factory("query", ["$http", "sxc", function($http, sxc) {
            var createQueryObj = function(name) {
                var qry = {
                    root: "app/auto/query/" + name
                };

                qry.get = function(config) {
                    return !name
                        ? $http(angular.extend({ url: sxc.data.sourceUrl() }, config)) // otherwise use the current-view stuff
                        : $http.get(qry.root, config); // if it has a name, call that specific query
                };

                return qry;
            };

            return createQueryObj;
        }])

        .directive("sxcToolbar", ["sxc", function(sxc) {
            return {
                restrict: "E",
                scope: {
                    // official syntax uses the toolbar configuration and the settings as standardized in 2sxc 8.6
                    // read about it in https://github.com/2sic/2sxc/wiki/AngularJs-1-Overview
                    toolbar: "&toolbar",
                    settings: "&settings"
                },
                link: function(scope, element, attrs) {
                    var manageCtrl = sxc.manage;
                    var toolbar = "";

                    // the manage only exists when in edit mode
                    if (manageCtrl) {
                        // run in new mode as defined in 2sxc 8.6
                        var tbObj = scope.toolbar();
                        if (tbObj !== undefined) {
                            var setObj = scope.settings();
                            toolbar = manageCtrl.getToolbar(tbObj, setObj);
                        }

                    }

                    element.html(toolbar);
                }
            };
        }]);
})(angular);
/*
    Extending 2sxc with angular capabilities
    In general, this should automatically take care of everything just by including it in your sources. 
    Make sure it's added after AngularJS and after the 2sxc.api.js
    It will then look for all sxc-apps and initialize them, ensuring that $http is pre-configured to work with DNN

    Required HTML Attributes 
    * sxc-app="MyAppNameWhatever" - required for auto-bootstrapping
    
    Optional HTML Attributes
    * ng-controller="AngularControllerName" - required for auto-bootstrapping
    * dependencies="" - here you can add additional dependencies if you need them
    * id, data-instanceid, app-instanceid - would tell 2sxc what module-id to use, for example when doing server requests
    *   Note that the id etc. are optional, because since 2sxc 8.x it can be auto-detected
    * data-cb-id - the content-block id, only used for advanced use cases and is normally auto-detected
    
    Angular Constants / Variables added
    * AppInstanceId - the module-id for accessing the HTML-block or for server-requests
    * ContentBlockId - advanced use case, not explained here
    * AppServiceFramework - a real or fake DNN sf-object
    * HttpHeaders - the headers which we use to initialize the $http to ensure it works / authenticates correctly
    
    Angular Modules added
    * 2sxc4ng
    * all the dependencies listed in the dependencies attribute
*/

(function(angular) {
    var ng = $2sxc.ng = {
        appAttribute: "sxc-app",
        ngAttrPrefixes: ["ng-", "data-ng-", "ng:", "x-ng-"],
        iidAttrNames: ["app-instanceid", "data-instanceid", "id"],
        cbidAttrName: "data-cb-id",
        ngAttrDependencies: "dependencies",

        // bootstrap: an App-Start-Help; normally you won't call this manually as it will be auto-bootstrapped. 
        // All params optional except for 'element'
        bootstrap: function(element, ngModName, iid, dependencies, config) {
            // first, try to get moduleId from function-param or from from URL
            iid = iid || findInstanceId(element) || $2sxc.urlParams.get("mid") || autoFindId(element);

            var cbid = findContentBlockId(element) || $2sxc.urlParams.get("cbid") || iid;
            // then provide access to the dnn-services framework (or a fake thereof)
            var sf = $.ServicesFramework(iid);

            // create a micro-module to configure sxc-init parameters, add to dependencies. Note that the order is important!
            var confMod = angular.module("confSxcApp" + iid + "-" + cbid, [])
                .constant("AppInstanceId", iid)
                .constant("ContentBlockId", cbid)
                .constant("AppServiceFramework", sf)
                .constant("HttpHeaders", {
                    "ModuleId": iid,
                    "ContentBlockId": cbid,
                    "TabId": sf.getTabId(),
                    "RequestVerificationToken": sf.getAntiForgeryValue(),
                    "Debugging-Hint": "bootstrapped by 2sxc4ng",
                    "Cache-Control": "no-cache", // had to add because of browser ajax caching issue #437
                    "Pragma": "no-cache"
                });
            var allDependencies = [confMod.name, "2sxc4ng"].concat(dependencies || [ngModName]);

            angular.element(document).ready(function() {
                try {
                    angular.bootstrap(element, allDependencies, config); // start the app
                } catch (e) { // Make sure that if one app breaks, others continue to work
                    if (console && console.error)
                        console.error(e);
                }
            });
        },

        // Auto-bootstrap all sub-tags having an 'sxc-app' attribute - for Multiple-Apps-per-Page
        bootstrapAll: function bootstrapAll(element) {
            element = element || document;
            var allAppTags = element.querySelectorAll("[" + ng.appAttribute + "]");
            angular.forEach(allAppTags, function(appTag) {
                var ngModName = appTag.getAttribute(ng.appAttribute);
                var configDependencyInjection = { strictDi: getNgAttribute(appTag, "strict-di") !== null };

                var dependencies = getNgAttribute(appTag, ng.ngAttrDependencies);
                if (dependencies) dependencies = dependencies.split(",");
                ng.bootstrap(appTag, ngModName, null, dependencies, configDependencyInjection);
            });
        },

        // if the page contains angular, do auto-bootstrap of all 2sxc apps
        autoRunBootstrap: function autoRunBootstrap() {
            // prevent multiple bootstrapping in case this file was included multiple times
            if (window.bootstrappingAlreadyStarted)
                return;
            window.bootstrappingAlreadyStarted = true;

            // bootstrap, if it has angular
            if (angular)
                angular.element(document).ready(function() {
                    ng.bootstrapAll();
                });
        },

    };


    // Helper function to try various attribute-prefixes
    function getNgAttribute(element, ngAttr) {
        var attr, i, ii = ng.ngAttrPrefixes.length;
        element = angular.element(element);
        for (i = 0; i < ii; ++i) {
            attr = ng.ngAttrPrefixes[i] + ngAttr;
            if (typeof (attr = element.attr(attr)) == "string")
                return attr;
        }
        return null;
    }

    // find instance Id in an attribute of the tag - typically with id="app-700" or something and use the number as IID
    function findInstanceId(element) {
        var attrib, ngElement = angular.element(element);
        for (var i = 0; i < ng.iidAttrNames.length; i++) {
            attrib = ngElement.attr(ng.iidAttrNames[i]);
            if (attrib) {
                var iid = parseInt(attrib.toString().replace(/\D/g, "")); // filter all characters if necessary
                if(iid) return iid; // if it contained a number, use it, otherwise continue
            }
        }
        return null;
    }

    function findContentBlockId(el) {
        var cbid;
        while (el.getAttribute) { // loop as long as it knows this command
            if ((cbid = el.getAttribute(ng.cbidAttrName))) return cbid;
            el = el.parentNode;
        }
        return null;
    }

    // this can't be in the find-instance-id, because it needs jQuery internally and only works on the real page
    function autoFindId(element) {
        if (!$) return null;
        var sxc = $2sxc(element);
        return sxc && sxc.id;
    }


    ng.autoRunBootstrap();

})(angular);
