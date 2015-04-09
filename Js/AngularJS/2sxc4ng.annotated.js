/*
    Extending 2sxc with angular capabilities
    In general, this should automatically take care of everything just by including it in your sources. 
    Make sure it's added after AngularJS and after the 2sxc.api.js
    It will then look for all sxc-apps and initialize them, ensuring that $http is pre-configured to work with DNN
*/
$2sxc.ng = {
    appAttribute: 'sxc-app',
    ngAttrPrefixes: ['ng-', 'data-ng-', 'ng:', 'x-ng-'],
    iidAttrNames: ['app-instanceid','data-instanceid', 'id'],

    // bootstrap: an App-Start-Help; normally you won't call this manually as it will be auto-bootstrapped. 
    // All params optional except for 'element'
    bootstrap: function (element, ngModName, iid, dependencies, config) {
        iid = iid || $2sxc.ng.findInstanceId(element); 
        var sf = $.ServicesFramework(iid);

        // create a micro-module to configure sxc-init parameters, add to dependencies. Note that the order is important!
        angular.module('confSxcApp' + iid, [])
            // .constant('iid', iid)
            .constant('AppInstanceId', iid)
            .constant('AppServiceFramework', sf)
            .constant('HttpHeaders', { "ModuleId": iid, "TabId": sf.getTabId(), "RequestVerificationToken": sf.getAntiForgeryValue() });
        var allDependencies = ['confSxcApp' + iid, '2sxc4ng'].concat(dependencies || [ngModName]);

        angular.element(document).ready(function () {
            angular.bootstrap(element, allDependencies, config);      // start the app
        });
    },

    // find instance Id in an attribute of the tag - typically with id="app-700" or something and use the number as IID
    findInstanceId: function findInstanceId(element) {
        var attrib, ngElement = angular.element(element);
        for (var i = 0; i < $2sxc.ng.iidAttrNames.length; i++) 
            if(attrib = ngElement.attr($2sxc.ng.iidAttrNames[i])) {
                var iid = parseInt(attrib.toString().replace(/\D/g, ''));  // filter all characters if necessary
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
        })
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
    }
}
$2sxc.ng.autoRunBootstrap();

angular.module('2sxc4ng', ['ng'])
    // Configure $http for DNN web services (security tokens etc.)
    .config(["$httpProvider", "HttpHeaders", function ($httpProvider, HttpHeaders) {
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
                  sxc.showDetailedHttpError(rejection);
                  return $q.reject(rejection);
                }
            };
        }]);
        
    }])

    // Provide the sxc helper for this module
    .factory('sxc', ["AppInstanceId", function (AppInstanceId) {
        console.log('creating sxc service for id: ' + AppInstanceId);
        if (!$2sxc) throw "the Angular service 'sxc' can't find the global $2sxc controller";
        var ngSxc = $2sxc(AppInstanceId);    // make this service be the 2sxc-controller for this module
        return ngSxc;
    }])
    
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
//# sourceMappingURL=data:application/json;base64,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Y2FwYWJpbGl0aWVzXHJcbiAgICBJbiBnZW5lcmFsLCB0aGlzIHNob3VsZCBhdXRvbWF0aWNhbGx5IHRha2UgY2FyZSBvZiBldmVyeXRoaW5nIGp1c3QgYnkgaW5jbHVkaW5nIGl0IGluIHlvdXIgc291cmNlcy4gXHJcbiAgICBNYWtlIHN1cmUgaXQncyBhZGRlZCBhZnRlciBBbmd1bGFySlMgYW5kIGFmdGVyIHRoZSAyc3hjLmFwaS5qc1xyXG4gICAgSXQgd2lsbCB0aGVuIGxvb2sgZm9yIGFsbCBzeGMtYXBwcyBhbmQgaW5pdGlhbGl6ZSB0aGVtLCBlbnN1cmluZyB0aGF0ICRodHRwIGlzIHByZS1jb25maWd1cmVkIHRvIHdvcmsgd2l0aCBETk5cclxuKi9cclxuJDJzeGMubmcgPSB7XHJcbiAgICBhcHBBdHRyaWJ1dGU6ICdzeGMtYXBwJyxcclxuICAgIG5nQXR0clByZWZpeGVzOiBbJ25nLScsICdkYXRhLW5nLScsICduZzonLCAneC1uZy0nXSxcclxuICAgIGlpZEF0dHJOYW1lczogWydhcHAtaW5zdGFuY2VpZCcsJ2RhdGEtaW5zdGFuY2VpZCcsICdpZCddLFxyXG5cclxuICAgIC8vIGJvb3RzdHJhcDogYW4gQXBwLVN0YXJ0LUhlbHA7IG5vcm1hbGx5IHlvdSB3b24ndCBjYWxsIHRoaXMgbWFudWFsbHkgYXMgaXQgd2lsbCBiZSBhdXRvLWJvb3RzdHJhcHBlZC4gXHJcbiAgICAvLyBBbGwgcGFyYW1zIG9wdGlvbmFsIGV4Y2VwdCBmb3IgJ2VsZW1lbnQnXHJcbiAgICBib290c3RyYXA6IGZ1bmN0aW9uIChlbGVtZW50LCBuZ01vZE5hbWUsIGlpZCwgZGVwZW5kZW5jaWVzLCBjb25maWcpIHtcclxuICAgICAgICBpaWQgPSBpaWQgfHwgJDJzeGMubmcuZmluZEluc3RhbmNlSWQoZWxlbWVudCk7IFxyXG4gICAgICAgIHZhciBzZiA9ICQuU2VydmljZXNGcmFtZXdvcmsoaWlkKTtcclxuXHJcbiAgICAgICAgLy8gY3JlYXRlIGEgbWljcm8tbW9kdWxlIHRvIGNvbmZpZ3VyZSBzeGMtaW5pdCBwYXJhbWV0ZXJzLCBhZGQgdG8gZGVwZW5kZW5jaWVzLiBOb3RlIHRoYXQgdGhlIG9yZGVyIGlzIGltcG9ydGFudCFcclxuICAgICAgICBhbmd1bGFyLm1vZHVsZSgnY29uZlN4Y0FwcCcgKyBpaWQsIFtdKVxyXG4gICAgICAgICAgICAvLyAuY29uc3RhbnQoJ2lpZCcsIGlpZClcclxuICAgICAgICAgICAgLmNvbnN0YW50KCdBcHBJbnN0YW5jZUlkJywgaWlkKVxyXG4gICAgICAgICAgICAuY29uc3RhbnQoJ0FwcFNlcnZpY2VGcmFtZXdvcmsnLCBzZilcclxuICAgICAgICAgICAgLmNvbnN0YW50KCdIdHRwSGVhZGVycycsIHsgXCJNb2R1bGVJZFwiOiBpaWQsIFwiVGFiSWRcIjogc2YuZ2V0VGFiSWQoKSwgXCJSZXF1ZXN0VmVyaWZpY2F0aW9uVG9rZW5cIjogc2YuZ2V0QW50aUZvcmdlcnlWYWx1ZSgpIH0pO1xyXG4gICAgICAgIHZhciBhbGxEZXBlbmRlbmNpZXMgPSBbJ2NvbmZTeGNBcHAnICsgaWlkLCAnMnN4YzRuZyddLmNvbmNhdChkZXBlbmRlbmNpZXMgfHwgW25nTW9kTmFtZV0pO1xyXG5cclxuICAgICAgICBhbmd1bGFyLmVsZW1lbnQoZG9jdW1lbnQpLnJlYWR5KGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgYW5ndWxhci5ib290c3RyYXAoZWxlbWVudCwgYWxsRGVwZW5kZW5jaWVzLCBjb25maWcpOyAgICAgIC8vIHN0YXJ0IHRoZSBhcHBcclxuICAgICAgICB9KTtcclxuICAgIH0sXHJcblxyXG4gICAgLy8gZmluZCBpbnN0YW5jZSBJZCBpbiBhbiBhdHRyaWJ1dGUgb2YgdGhlIHRhZyAtIHR5cGljYWxseSB3aXRoIGlkPVwiYXBwLTcwMFwiIG9yIHNvbWV0aGluZyBhbmQgdXNlIHRoZSBudW1iZXIgYXMgSUlEXHJcbiAgICBmaW5kSW5zdGFuY2VJZDogZnVuY3Rpb24gZmluZEluc3RhbmNlSWQoZWxlbWVudCkge1xyXG4gICAgICAgIHZhciBhdHRyaWIsIG5nRWxlbWVudCA9IGFuZ3VsYXIuZWxlbWVudChlbGVtZW50KTtcclxuICAgICAgICBmb3IgKHZhciBpID0gMDsgaSA8ICQyc3hjLm5nLmlpZEF0dHJOYW1lcy5sZW5ndGg7IGkrKykgXHJcbiAgICAgICAgICAgIGlmKGF0dHJpYiA9IG5nRWxlbWVudC5hdHRyKCQyc3hjLm5nLmlpZEF0dHJOYW1lc1tpXSkpIHtcclxuICAgICAgICAgICAgICAgIHZhciBpaWQgPSBwYXJzZUludChhdHRyaWIudG9TdHJpbmcoKS5yZXBsYWNlKC9cXEQvZywgJycpKTsgIC8vIGZpbHRlciBhbGwgY2hhcmFjdGVycyBpZiBuZWNlc3NhcnlcclxuICAgICAgICAgICAgICAgIGlmICghaWlkKSB0aHJvdyBcImlpZCBvciBpbnN0YW5jZUlkICh0aGUgRE5OIG1vZHVsZWlkKSBub3Qgc3VwcGxpZWQgYW5kIGF1dG9tYXRpYyBsb29rdXAgZmFpbGVkLiBQbGVhc2Ugc2V0IGFwcC10YWcgYXR0cmlidXRlIGlpZCBvciBnaXZlIGlkIGluIGJvb3RzdHJhcCBjYWxsXCI7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gaWlkO1xyXG4gICAgICAgICAgICB9XHJcbiAgICB9LFxyXG5cclxuICAgIC8vIEF1dG8tYm9vdHN0cmFwIGFsbCBzdWItdGFncyBoYXZpbmcgYW4gJ3N4Yy1hcHAnIGF0dHJpYnV0ZSAtIGZvciBNdWx0aXBsZS1BcHBzLXBlci1QYWdlXHJcbiAgICBib290c3RyYXBBbGw6IGZ1bmN0aW9uIGJvb3RzdHJhcEFsbChlbGVtZW50KSB7XHJcbiAgICAgICAgZWxlbWVudCA9IGVsZW1lbnQgfHwgZG9jdW1lbnQ7XHJcbiAgICAgICAgdmFyIGFsbEFwcFRhZ3MgPSBlbGVtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoJ1snICsgJDJzeGMubmcuYXBwQXR0cmlidXRlICsgJ10nKTtcclxuICAgICAgICBhbmd1bGFyLmZvckVhY2goYWxsQXBwVGFncywgZnVuY3Rpb24gKGFwcFRhZykge1xyXG4gICAgICAgICAgICB2YXIgbmdNb2ROYW1lID0gYXBwVGFnLmdldEF0dHJpYnV0ZSgkMnN4Yy5uZy5hcHBBdHRyaWJ1dGUpO1xyXG4gICAgICAgICAgICB2YXIgY29uZmlnRGVwZW5kZW5jeUluamVjdGlvbiA9IHsgc3RyaWN0RGk6ICQyc3hjLm5nLmdldE5nQXR0cmlidXRlKGFwcFRhZywgXCJzdHJpY3QtZGlcIikgIT09IG51bGwgfTtcclxuICAgICAgICAgICAgJDJzeGMubmcuYm9vdHN0cmFwKGFwcFRhZywgbmdNb2ROYW1lLCBudWxsLCBudWxsLCBjb25maWdEZXBlbmRlbmN5SW5qZWN0aW9uKTtcclxuICAgICAgICB9KVxyXG4gICAgfSxcclxuXHJcbiAgICAvLyBpZiB0aGUgcGFnZSBjb250YWlucyBhbmd1bGFyLCBkbyBhdXRvLWJvb3RzdHJhcCBvZiBhbGwgMnN4YyBhcHBzXHJcbiAgICBhdXRvUnVuQm9vdHN0cmFwOiBmdW5jdGlvbiBhdXRvUnVuQm9vdHN0cmFwKCkge1xyXG4gICAgICAgIGlmIChhbmd1bGFyKVxyXG4gICAgICAgICAgICBhbmd1bGFyLmVsZW1lbnQoZG9jdW1lbnQpLnJlYWR5KGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICQyc3hjLm5nLmJvb3RzdHJhcEFsbCgpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgIH0sXHJcblxyXG4gICAgLy8gSGVscGVyIGZ1bmN0aW9uIHRvIHRyeSB2YXJpb3VzIGF0dHJpYnV0ZS1wcmVmaXhlc1xyXG4gICAgZ2V0TmdBdHRyaWJ1dGU6IGZ1bmN0aW9uIGdldE5nQXR0cmlidXRlKGVsZW1lbnQsIG5nQXR0cikge1xyXG4gICAgICAgIHZhciBhdHRyLCBpLCBpaSA9ICQyc3hjLm5nLm5nQXR0clByZWZpeGVzLmxlbmd0aDtcclxuICAgICAgICBlbGVtZW50ID0gYW5ndWxhci5lbGVtZW50KGVsZW1lbnQpO1xyXG4gICAgICAgIGZvciAoaSA9IDA7IGkgPCBpaTsgKytpKSB7XHJcbiAgICAgICAgICAgIGF0dHIgPSAkMnN4Yy5uZy5uZ0F0dHJQcmVmaXhlc1tpXSArIG5nQXR0cjtcclxuICAgICAgICAgICAgaWYgKHR5cGVvZiAoYXR0ciA9IGVsZW1lbnQuYXR0cihhdHRyKSkgPT0gJ3N0cmluZycpXHJcbiAgICAgICAgICAgICAgICByZXR1cm4gYXR0cjtcclxuICAgICAgICB9XHJcbiAgICAgICAgcmV0dXJuIG51bGw7XHJcbiAgICB9XHJcbn1cclxuJDJzeGMubmcuYXV0b1J1bkJvb3RzdHJhcCgpO1xyXG5cclxuYW5ndWxhci5tb2R1bGUoJzJzeGM0bmcnLCBbJ25nJ10pXHJcbiAgICAvLyBDb25maWd1cmUgJGh0dHAgZm9yIEROTiB3ZWIgc2VydmljZXMgKHNlY3VyaXR5IHRva2VucyBldGMuKVxyXG4gICAgLmNvbmZpZyhmdW5jdGlvbiAoJGh0dHBQcm92aWRlciwgSHR0cEhlYWRlcnMpIHtcclxuICAgICAgICBhbmd1bGFyLmV4dGVuZCgkaHR0cFByb3ZpZGVyLmRlZmF1bHRzLmhlYWRlcnMuY29tbW9uLCBIdHRwSGVhZGVycyk7XHJcbiAgICAgICAgJGh0dHBQcm92aWRlci5pbnRlcmNlcHRvcnMucHVzaChmdW5jdGlvbigkcSwgc3hjKSB7IFxyXG4gICAgICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICAgICAgLy8gUmV3cml0ZSAyc3hjLXVybHMgaWYgbmVjZXNzYXJ5XHJcbiAgICAgICAgICAgICAgICAncmVxdWVzdCc6IGZ1bmN0aW9uKGNvbmZpZykge1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbmZpZy51cmwgPSBzeGMucmVzb2x2ZVNlcnZpY2VVcmwoY29uZmlnLnVybCk7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGNvbmZpZztcclxuICAgICAgICAgICAgICAgIH0sXHJcblxyXG4gICAgICAgICAgICAgICAgLy8gU2hvdyB2ZXJ5IG5pY2UgZXJyb3IgaWYgbmVjZXNzYXJ5XHJcbiAgICAgICAgICAgICAgICdyZXNwb25zZUVycm9yJzogZnVuY3Rpb24ocmVqZWN0aW9uKSB7XHJcbiAgICAgICAgICAgICAgICAgIHN4Yy5zaG93RGV0YWlsZWRIdHRwRXJyb3IocmVqZWN0aW9uKTtcclxuICAgICAgICAgICAgICAgICAgcmV0dXJuICRxLnJlamVjdChyZWplY3Rpb24pO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgIH0pO1xyXG4gICAgICAgIFxyXG4gICAgfSlcclxuXHJcbiAgICAvLyBQcm92aWRlIHRoZSBzeGMgaGVscGVyIGZvciB0aGlzIG1vZHVsZVxyXG4gICAgLmZhY3RvcnkoJ3N4YycsIGZ1bmN0aW9uIChBcHBJbnN0YW5jZUlkKSB7XHJcbiAgICAgICAgY29uc29sZS5sb2coJ2NyZWF0aW5nIHN4YyBzZXJ2aWNlIGZvciBpZDogJyArIEFwcEluc3RhbmNlSWQpO1xyXG4gICAgICAgIGlmICghJDJzeGMpIHRocm93IFwidGhlIEFuZ3VsYXIgc2VydmljZSAnc3hjJyBjYW4ndCBmaW5kIHRoZSBnbG9iYWwgJDJzeGMgY29udHJvbGxlclwiO1xyXG4gICAgICAgIHZhciBuZ1N4YyA9ICQyc3hjKEFwcEluc3RhbmNlSWQpOyAgICAvLyBtYWtlIHRoaXMgc2VydmljZSBiZSB0aGUgMnN4Yy1jb250cm9sbGVyIGZvciB0aGlzIG1vZHVsZVxyXG4gICAgICAgIHJldHVybiBuZ1N4YztcclxuICAgIH0pXHJcbiAgICBcclxuICAgIC8qIFRvZG86IGZ1dHVyZSBmZWF0dXJlXHJcbiAgICAuZmFjdG9yeSgnc3hjUmVzb3VyY2UnLCBmdW5jdGlvbihpaWQsICRpbmplY3Rvcikge1xyXG4gICAgICAgIHJldHVybiBmdW5jdGlvbiAodXJsLCBwYXJhbURlZmF1bHRzLCBhY3Rpb25zLCBvcHRpb25zKSB7XHJcblxyXG4gICAgLy8gICAgICAgIC8vIHRvZG86IGNoZWNrIGlmIHJlc291cmNlIGlzIGxvYWRlZFxyXG4gICAgICAgICAgICAvLyBtYW51YWxseSBnZXQgaW5qZWN0b3IsIHRvIHByZXZlbnQgcmVxdWlyZWQgZGVwZW5kZW5jeSBhbmQgZ2l2ZSBuaWNlIGVycm9yXHJcbiAgICAgICAgICAgIHZhciBpbmogPSBhbmd1bGFyLmluamVjdG9yKFsnbmdSZXNvdXJjZSddKTtcclxuICAgICAgICAgICAgaWYgKCFpbmopXHJcbiAgICAgICAgICAgICAgICB0aHJvdyAnRXJyb3I6IHN4Y1Jlc291cmNlIG9ubHkgd29ya3MgaWYgdGhlIHBhZ2UgYWxzbyBpbmNsdWRlcyBhbmd1bGFyLXJlc291cmNlLiBTbyB5b3UgbXVzdCBlaXRoZXIgaW5jbHVkZSB0aGF0LCBvciB5b3Ugc2hvdWxkIHVzZSBqUXVlcnkgQUpBWCBpbnN0ZWFkLic7XHJcbiAgICAgICAgICAgIHZhciByZXMgPSBpbmouZ2V0KCckcmVzb3VyY2UnKVxyXG4gICAgICAgICAgICBcclxuICAgICAgICAgICAgXHJcbiAgICAgICAgICAgIHJldHVybiByZXModXJsLCBwYXJhbURlZmF1bHRzLCBhY3Rpb25zLCBvcHRpb24pO1xyXG4gICAgICAgIH1cclxuICAgIH0pXHJcbiAgICAqL1xyXG47Il19