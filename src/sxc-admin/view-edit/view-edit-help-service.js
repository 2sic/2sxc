
angular.module("ViewEdit")
    .factory("helpSvc", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId, templateId) {
            var svc = {
                getItems: function(item) {
                    return $http.get('app/contentgroup/replace', { params: { appId: appId, guid: item.guid, part: item.part, index: item.index } });
                },
                saveItem: function(item) {
                    return $http.post('app/contentgroup/replace', {}, { params: { guid: item.guid, part: item.part, index: item.index, entityId: item.id } });
                },

                getList: function (contentGroup) {
                    return $http.get('app/contentgroup/itemlist', { params: { appId: appId, guid: contentGroup.guid } });
                },

                saveList: function (contentGroup, resortedList) {
                    return $http.post('app/contentgroup/itemlist', resortedList, { params: { appId: appId, guid: contentGroup.guid } });
                },

                getHeader: function (contentGroup) {
                    return $http.get('app/contentgroup/header', { params: { appId: appId, guid: contentGroup.guid } });
                }


            };

            return svc;
        };
    });