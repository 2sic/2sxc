(function () {

    angular.module("ImportExport")
        .factory("ExportContentService", ExportContentService)
    ;


    /*@ngInject*/
    function ExportContentService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            getContentInfo: getContentInfo,
            exportContent: exportContent
        };
        return srvc;


        function getContentInfo(scope) {
            return $http.get(eavConfig.getUrlPrefix("api") + "/app/ImportExport/GetContentInfo", { params: { appId: appId, zoneId: zoneId, scope: scope || "2SexyContent" } }).then(function (result) { return result.data; });
        }

        function exportContent(contentTypeIds, entityIds, templateIds) {
            window.open(eavConfig.getUrlPrefix("api") + "/app/ImportExport/ExportContent?appId=" + appId + "&zoneId=" + zoneId + "&contentTypeIdsString=" + contentTypeIds.join(";") + "&entityIdsString=" + entityIds.join(";") + "&templateIdsString=" + templateIds.join(";"), "_self", "");
            return $q.when(true);
        }
    }

}());