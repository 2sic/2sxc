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
    cbidAttrName: "data-cb-id",
    ngAttrDependencies: "dependencies", 

    // bootstrap: an App-Start-Help; normally you won't call this manually as it will be auto-bootstrapped. 
    // All params optional except for 'element'
    bootstrap: function (element, ngModName, iid, dependencies, config) {
        // first, try to get moduleId from function-param or from from URL
        iid = iid || $2sxc.ng.findInstanceId(element) || $2sxc.urlParams.get("mid");

        var cbid = $2sxc.ng.findContentBlockId(element) || $2sxc.urlParams.get("cbid") || iid;
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
        for (var i = 0; i < $2sxc.ng.iidAttrNames.length; i++) {
            attrib = ngElement.attr($2sxc.ng.iidAttrNames[i]);
            if (attrib) {
                var iid = parseInt(attrib.toString().replace(/\D/g, "")); // filter all characters if necessary
                if (!iid) throw "iid or instanceId (the DNN moduleid) not supplied and automatic lookup failed. Please set app-tag attribute iid or give id in bootstrap call";
                return iid;
            }
        }
    },

    findContentBlockId: function (el) {
        var cbid;
        while (el.getAttribute) { // loop as long as it knows this command
            if ((cbid = el.getAttribute($2sxc.ng.cbidAttrName))) return cbid;
            el = el.parentNode;
        }
        return null;
    },
    _closest: function(el, fn) {
        while (el) {
            if (fn(el)) return el; 
            el = el.parentNode;
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
        // prevent multiple bootstrapping in case this file was included multiple times
        if (window.bootstrappingAlreadyStarted)
            return;
        window.bootstrappingAlreadyStarted = true;

        // bootstrap, if it has angular
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
    .config(["$httpProvider", "HttpHeaders", function ($httpProvider, HttpHeaders) {
        angular.extend($httpProvider.defaults.headers.common, HttpHeaders);
        $httpProvider.interceptors.push(["$q", "sxc", function ($q, sxc) {
            return {
                // Rewrite 2sxc-urls if necessary
                'request': function (config) {
                    config.url = sxc.resolveServiceUrl(config.url);
                    return config;
                },

                // Show very nice error if necessary
                'responseError': function (rejection) {
                    if(!rejection.config.ignoreErrors)
                        sxc.showDetailedHttpError(rejection);
                    return $q.reject(rejection);
                }
            };
        }]);

    }])

    // provide the global $2sxc object to angular modules as a clear, clean dependency
    .factory("$2sxc", function () {
        if (!$2sxc) throw "the Angular service 'sxc' can't find the global $2sxc controller";
        return $2sxc;
    })

    // Provide the app-specific sxc helper for this module
    .factory("sxc", ["AppInstanceId", "$2sxc", function (AppInstanceId, $2sxc) {
        if(window.console) console.log("creating sxc service for id: " + AppInstanceId);
        var ngSxc = $2sxc(AppInstanceId);    // make this service be the 2sxc-controller for this module
        return ngSxc;
    }])


    /// Standard entity commands like get one, many etc.
    .factory("content", ["$http", function ($http) {
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
    }])

    /// simple helper service which will call a query
    .factory("query", ["$http", function ($http) {
        return function (name) {
            var qry = {};
            qry.root = "app-query/" + name;

            qry.get = function (config) {
                return $http.get(qry.root, config);
            };
            return qry;
        };
    }])


    // BETA - not final. SXC-Toolbar, not ready for production use
    .directive('sxcToolbar', ["AppInstanceId", function SxcToolbar(AppInstanceId) {
        return {
            restrict: 'E',
            scope: {
                entity: '&for',
                entityId: '&forId',
                actions: '&custom',
                forContentType: '&forContentType'
            },
            link: function (scope, element, attrs) {
                var manageCtrl = $2sxc(AppInstanceId).manage;
                var toolbar = '';

                if(manageCtrl)
                {
                    
                    if (scope.entity() !== undefined)
                        toolbar = manageCtrl.getToolbar([{ "entity": scope.entity(), "action": "edit" }]);
                    else if (scope.entityId() !== undefined)
                        toolbar = manageCtrl.getToolbar([{ "entityId": scope.entityId(), "action": "edit" }]);
                    else if (scope.actions() !== undefined)
                        toolbar = manageCtrl.getToolbar(scope.actions());
                    else if (scope.forContentType() !== undefined)
                        toolbar = manageCtrl.getToolbar([{ "action": "new", contentType: scope.forContentType() }]);

                }

                element.html(toolbar);
            }
        };
    }])
;