(function () {

    angular.module("ImportExport")
        .factory("ImportAppService", ImportAppService)
    ;


    function ImportAppService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            importApp: importApp
        };
        return srvc;


        function importApp(fileName, fileData) {
            return $http.post("app/ImportExport/ImportApp", { AppId: appId, ZoneId: zoneId, FileName: fileName, FileData: fileData });
        }
    }
}());