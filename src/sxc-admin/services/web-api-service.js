angular.module("SxcServices")
    .factory("webApiSvc", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/system/webapifiles", { params: { appId: svc.appId } });
            }));

            return svc;
        };
    });