angular.module('PermissionsServices', ['ng', 'eavNgSvcs', 'eavGlobalConfigurationProvider'])
    .factory('permissionsSvc', function ($http, eavGlobalConfigurationProvider, entitiesSvc, eavManagementSvc) {
        var svc = {};
        var eavConf = eavGlobalConfigurationProvider;
        svc.ctName = "PermissionConfiguration";
        svc.ctId = 0;
        svc.PermissionTargetGuid = '{00000000-0000-0000-0000-000000000000}';
        svc.EntityAssignment = eavGlobalConfigurationProvider.assignmentObjectTypeIdDataPipeline;

        // Cached list of all permissions
        svc._all = [];
        svc.allLive = function getAllLive(){
            if(svc._all.length == 0)
                svc.getAll();
            return svc._all;
        }

        // use a promise-result to re-fill the live list of all items, return the promise again
        svc.updateLiveAll = function updateLiveAll(result) {
            svc._all.length = 0; // clear
            for(i = 0; i < result.data.length; i++)
                svc._all.push(result.data[i]);
            return result;
        }

        svc.getAll = function getAll() {
            return eavManagementSvc.getAssignedItems(svc.EntityAssignment, svc.PermissionTargetGuid, svc.ctName).then(svc.updateLiveAll);
        };

        // Get ID of this content-type
        eavManagementSvc.getContentTypeDefinition(svc.ctName).then(function (result) {
            svc.ctId = result.data.AttributeSetId;
        });


        
        svc.delete = function del(id) {
            return entitiesSvc.delete(svc.ctName, id)
                .then(svc.getAll);
        };
        
        
        // Get New/Edit/Design URL for a Pipeline
        svc.getUrl = function (mode, id) {
            switch (mode) {
                case 'new':
                    return eavConf.itemForm.getNewItemUrl(svc.ctId, svc.EntityAssignment, { keyGuid: svc.PermissionTargetGuid }, false);
                case 'edit':
                    return eavGlobalConfigurationProvider.itemForm.getEditItemUrl(id, undefined, true);
                case 'design':
                    return eavGlobalConfigurationProvider.pipelineDesigner.getUrl(appId, id);
                default:
                    return null;
            }
        };

/*
        // load and return a promise, but also update internal cache whenever data is retrieved
        svc.load = function load() {
            return $http.get("app-query/Q1")
                .then(svc.updateLiveAll);

            //return $http.get("app-api/Query/GetQuery", { params: { "name": "Q1" }})
            //    .then(svc.updateLiveAll);
        }

        // auto-load when the service is started
        // probably not a good idea...svc.load();

        // use a promise-result to re-fill the live list of all items, return the promise again
        svc.updateLiveAll = function updateLiveAll(result) {
            svc._all.length = 0; // clear
            for (i = 0; i < result.data.length; i++)
                svc._all.push(result.data[i]);
            svc._all[0].isDefault = true;
            return result;
        }
*/
        return svc;
    })

;