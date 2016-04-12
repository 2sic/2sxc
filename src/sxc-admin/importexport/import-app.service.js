(function () {

    angular.module("ImportExport")
        .factory("ImportAppService", ImportAppService)
    ;


    function ImportAppService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            importApp: importApp,
        };
        return srvc;


        function importApp(file) {
            return $http({
                method: "POST",
                url: "app/ImportExport/ImportApp",
                headers: { "Content-Type": undefined },
                transformRequest: function (data) {
                    var formData = new FormData();
                    formData.append("AppId", data.AppId);
                    formData.append("ZoneId", data.ZoneId);
                    formData.append("File", data.File);
                    return formData;
                },
                data: { AppId: appId, ZoneId: zoneId, File: file }
            });
        }
    }
}());