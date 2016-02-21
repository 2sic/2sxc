// This is the service which allows opening dnn-bridge dialogs and processes the results

angular.module("sxcFieldTemplates")
    .factory("dnnBridgeSvc", function ($modal, $http, promiseToastr, eavConfig, sxc) {
        var svc = {};
        svc.open = function open(type, oldValue, params, callback) {
            var template = type === "pagepicker" ? "pagepicker" : "filemanager";

            var connector = {
                params: params,
                valueChanged: callback,
                dialogType: type
            };

            connector.valueChanged = function valueChanged(value, type) {
                connector.modalInstance.close();
                callback(value, type);
            };

            connector.params.CurrentValue = oldValue;

            connector.modalInstance = $modal.open({
                templateUrl: "fields/dnn-bridge/hyperlink-default-" + template + ".html",
                resolve: {
                    bridge: function () {
                        return connector;
                    }
                },
                controller: function ($scope, bridge) {
                    $scope.bridge = bridge;
                },
                windowClass: "sxc-dialog-filemanager"
            });

            return connector.modalInstance;
        };

        // convert the url to a Id-code
        svc.convertPathToId = function(path, type) {
            var pathWithoutVersion = path.replace(/\?ver=[0-9\-]*$/gi, "");
            // todo: working on https://github.com/2sic/2sxc/issues/656 but can't reproduce error
            // this is why I tried ignoreErrors and promisetoaster, but atm there is nothing to work on...
            var promise = $http.get("dnn/Hyperlink/GetFileByPath?relativePath=" + encodeURIComponent(pathWithoutVersion),
            {
                //ignoreErrors: true
            });
            return promiseToastr(promise, "Edit.Field.Hyperlink.Message.Loading", "Edit.Field.Hyperlink.Message.Ok", "Edit.Field.Hyperlink.Message.Error", 0, 0, 1000);
        };

        // handle short-ID links like file:17
        svc.getUrlOfId = function(idCode) {
            var linkLowered = idCode.toLowerCase();
            if (linkLowered.indexOf("file:") !== -1 || linkLowered.indexOf("page:") !== -1)
                return $http.get("dnn/Hyperlink/ResolveHyperlink?hyperlink=" + encodeURIComponent(idCode));
            return null;
        };

        return svc;

    });