(function () {

    angular.module("ImportExport")
        .factory("ImportContentService", ImportContentService)
    ;


    function ImportContentService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            importContent: importContent
        };
        return srvc;


        function importContent(fileName, fileData) {
            return $http.post("app/ImportExport/ImportContent", { AppId: appId, ZoneId: zoneId, FileName: fileName, FileData: fileData });
        }
    }

}());