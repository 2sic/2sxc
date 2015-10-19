angular.module("SxcServices")
    .factory("languagesSvc", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = {
                appId: appId
            };

            svc = angular.extend(svc, svcCreator.implementLiveList(function getAll() {
                return $http.get("app/system/getlanguages");//, { params: { appId: svc.appId } });
            }));

            // delete, then reload
            svc.toggle = function toggle(item) {
                return $http.get("app/system/switchlanguage", {params: {cultureCode: item.Code, enable: !item.IsEnabled }})
                    .then(svc.liveListReload);
            };

            svc.save = function save(item) {
                return $http.get("app/system/switchlanguage", { params: { cultureCode: item.Code, enable: item.IsEnabled } })
                    .then(svc.liveListReload);
            };

            return svc;
        };
    });