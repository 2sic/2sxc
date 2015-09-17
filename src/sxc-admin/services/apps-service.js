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
                return $http.post("app/system/app", { name: name })
                    .then(svc.liveListReload);
            };

            // delete, then reload
            svc.delete = function del(appId) {
                return $http.delete("app/system/app", {params: {appId: appId }})
                    .then(svc.liveListReload);
            };

            return svc;
        };
    });