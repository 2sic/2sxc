angular.module("SxcServices")
    .factory("appSettings", function ($http, eavConfig, svcCreator, contentTypeSvc, contentItemsSvc, eavAdminDialogs, $filter) {

        // Construct a service for this specific appId
        return function createSvc(appId) {
            var svc = contentTypeSvc(appId, "2SexyContent-App");
            //svc.appId = appId;
            svc.promise = svc.liveListReload(); // try to load the data..

            svc.openConfig = function openConf(staticName, afterEvent) {
                return svc.promise.then(function() {
                    var items = svc.liveList();
                    var found = $filter("filter")(items, { StaticName: staticName }, true);
                    if (found.length !== 1)
                        throw "Found too many settings for the type " + staticName;
                    var item = found[0];
                    return eavAdminDialogs.openContentTypeFields(item, afterEvent);
                });
            };

            svc.edit = function edit(staticName, afterEvent) {
                return svc.promise.then(function() {
                    var contentSvc = contentItemsSvc(svc.appId, staticName);
                    return contentSvc.liveListReload().then(function(result) {
                        var found = result.data;
                        if (found.length !== 1)
                            throw "Found too many settings for the type " + staticName;
                        var item = found[0];
                        return eavAdminDialogs.openItemEditWithEntityId(item.Id, afterEvent);
                    });
                });
            };

            svc.editPackage = function editPackage(callback) {
                return svc.edit("2SexyContent-App", callback);
            };

            return svc;
        };
    });