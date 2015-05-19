angular.module('eavNgSvcs', ['ng'])

    /// Config to ensure that $location can work and give url-parameters
    .config(['$locationProvider', function ($locationProvider) {
        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        })
    } ])

    /// Provide state-information related to the current open dialog
    .factory('eavManagementDialog', function($location){
    	var result = {};
		// ToDo: Nicer solution for getting the appId
		var appId = $location.url().match(/\/AppId\/([0-9]+)/i);
		if (appId != null && appId.length > 1)
        //if (appId.length > 1)
			result.appId = appId[1];
		else
			result.appId = $location.search().AppId;
        return result;
    })

    /// Management actions which are rather advanced metadata kind of actions
    .factory('eavManagementSvc', function($http, eavManagementDialog) {
        var svc = {};

        // Retrieve extra content-type info
        svc.getContentTypeDefinition = function getContentTypeDefinition(contentTypeName) {
            return $http.get('eav/Entities/GetContentType', { params: { appId: eavManagementDialog.appId, contentType: contentTypeName} });
        }

        // Find all items assigned to a GUID
        svc.getAssignedItems = function getAssignedItems(assignedToId, keyGuid, contentTypeName) {
            return $http.get('eav/entities/GetAssignedEntities', { params: {
                appId: eavManagementDialog.appId,
                assignmentObjectTypeId: assignedToId,
                keyGuid: keyGuid,
                contentType: contentTypeName
            }
            });
        }
        return svc;
    })


    /// Standard entity commands like get one, many etc.
    .factory('entitiesSvc', function ($http, eavManagementDialog) {
        var svc = {};

        svc.get = function get(contentType, id) {
            return id 
                ? $http.get("eav/entities/GetOne", { params: { 'contentType': contentType, 'id': id, 'appId': eavManagementDialog.appId }})
                : $http.get("eav/entities/GetEntities", { params: { 'contentType': contentType, 'appId': eavManagementDialog.appId }});
        };

        svc.delete = function del(type, id) {
            return $http.delete('eav/Entities/Delete', { params: { 
            'contentType': type, 
            'id': id,
            'appId': eavManagementDialog.appId }});
        }
        
        return svc;
    })


    ///// Standard entity commands like get one, many etc.
    //.factory('contentSvc', function ($http) {
    //    var svc = {};

    //    svc.get = function get(contentType, id) {
    //        return id 
    //            ? $http.get("app-content/" + contentType + '/' + id)
    //            : $http.get("app-content/" + contentType);
    //    };

    //    // todo
    //    svc.update = function update(contentType, id, values) {
            
    //    }

    //    svc.delete = function del(contentType, id) {
    //        return $http.delete('app-content/' + contentType + '/' + id);
    //    }

    //    svc.getQuery = function getQuery(name) {
    //        return $http.get("app-query/" + name)
    //    }
        
    //    return svc;
    //})

;