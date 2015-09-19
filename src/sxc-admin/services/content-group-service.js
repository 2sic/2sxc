
angular.module("SxcServices")
    .factory("contentGroupSvc", function($http, eavConfig, svcCreator, $resource) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {};


            svc.replace = $resource("app/contentgroup/replace",
            { appId: appId, guid: "@guid", part: "@part", index: "@index" },
            {
                get: { method: "GET", isArray:true },
                save: {method: "POST", params: { entityId: "@id" }}
            });

            return svc;
        };
    });