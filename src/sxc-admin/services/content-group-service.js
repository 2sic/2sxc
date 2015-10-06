
angular.module("SxcServices")
    .factory("contentGroupSvc", function($http, eavConfig, svcCreator, $resource) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                getItems: function(item) {
                    return $http('app/contentgroup/replace', { appId: appId, guid: item.guid, part: item.part, index: item.index });
                },
                saveItem: function(item) {
                    return $http.post('app/contentgroup/replace', { guid: item.guid, part: item.part, index: item.index, entityId: item.id });
                }
            };

            //svc.replace = $resource("",
            //,
            //{
            //    get: { method: "GET", isArray:true },
            //    save: {method: "POST", params: { entityId: "@id" }}
            //});

            return svc;
        };
    });