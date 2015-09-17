angular.module("SxcServices")//, ['ng', 'eavNgSvcs', "EavConfiguration"])
    .factory("templatesSvc", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/template/getall", { params: { appId: svc.appId } });
            }));

            // delete, then reload
            svc.delete = function del(id) {
                return $http.delete("app/templates/delete", {params: {appId: svc.appId, id: id }})
                    .then(svc.liveListReload);
            };
            return svc;
        };
    });