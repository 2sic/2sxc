angular.module("SxcServices")
    /*@ngInject*/
    .factory("appDialogConfigSvc", function (appId, $http) {
        var svc = {};

        // this will retrieve an advanced getting-started url to use in an the iframe
        svc.getDialogSettings = function gettingStartedUrl() {
            return $http.get("app/system/dialogsettings", { params: { appId: appId } });
        };
        return svc;
    });