angular.module('ImportExportServices', ['ng', 'eavNgSvcs', "EavConfiguration"])
    .factory('importExportSvc', function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            // todo: 2tk - everything here is only demo code

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get('app/template/getall', { params: { appId: svc.appId } });
            }));

            // delete, then reload
            svc.delete = function del(id) {
                return $http.delete('sxc/templates/delete', {params: {appId: svc.appId, id: id }})
                    .then(svc.liveListReload);
            };
            return svc;
        };
    });