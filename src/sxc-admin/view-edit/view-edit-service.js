
angular.module("ViewEdit")
    .factory("viewEditSvc", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId, templateId) {
            var svc = {
                getView: function(item) {
                    return $http.get('app/todo/replace', { params: { appId: appId, guid: item.guid, part: item.part, index: item.index } });
                },
                saveView: function(item) {
                    return $http.post('app/todo/replace', {}, { params: { guid: item.guid, part: item.part, index: item.index, entityId: item.id } });
                }

            };

            return svc;
        };
    });