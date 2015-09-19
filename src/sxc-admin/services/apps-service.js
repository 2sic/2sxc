angular.module("SxcServices")
    .factory("appsSvc", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(zoneId) {
            var svc = {
                zoneId: zoneId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/system/apps", { params: { zoneId: svc.zoneId } });
            }));

            svc.create = function create(name) {
                return $http.post("app/system/app", {}, { params: { zoneId: svc.zoneId, name: name } })
                    .then(svc.liveListReload);
            };

            // delete, then reload
            // for unclear reason, the verb DELETE fails, so I'm using get for now
            svc.delete = function del(appId) {
                return $http.get("app/system/deleteapp", {params: { zoneId: svc.zoneId, appId: appId } })
                    .then(svc.liveListReload);
            };

            return svc;
        };
    });