angular.module("SxcServices")
    .factory("adamSvc", function($http, eavConfig, sxc, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(contentType, entityGuid, field, subfolder) {
            var svc = {
                url: sxc.resolveServiceUrl("app-content/" + contentType + "/" + entityGuid + "/" + field + "/"),
                subfolder: subfolder
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get(url + "items", { params: { subfolder: svc.subfolder } });
            }));

            // create folder
            svc.add = function add(newfolder) {
                return $http.post(url + "folder", {}, { params: { subfolder: svc.subfolder, newFolder: newfolder } })
                    .then(svc.liveListReload);
            };


            // delete, then reload
            // IF verb DELETE fails, so I'm using get for now
            svc.delete = function del(isFolder, id) {
                return $http.delete(url + "delete", {}, { params: { subfolder: svc.subfolder, isFolder: isFolder, id: id } })
                    .then(svc.liveListReload);
            };

            return svc;
        };
    });