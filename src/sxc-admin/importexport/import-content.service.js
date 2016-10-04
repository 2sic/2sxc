(function () {

    angular.module("ImportExport")
        .factory("ImportContentService", ImportContentService)
    ;


    /*@ngInject*/
    function ImportContentService(appId, zoneId, eavConfig, $http, $q) {
        var srvc = {
            importContent: importContent
        };
        return srvc;


        function importContent(file) {
            return $http({
                method: "POST",
                url: "app/ImportExport/ImportContent",
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