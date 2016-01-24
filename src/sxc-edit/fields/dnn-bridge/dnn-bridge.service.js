// This is the service which allows opening dnn-bridge dialogs and processes the results

angular.module("sxcFieldTemplates")
    .factory("dnnBridgeSvc", function($modal, $http, eavConfig, sxc) {
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
            var promise = $http.get("dnn/Hyperlink/GetFileByPath?relativePath=" + encodeURIComponent(pathWithoutVersion));
            return promise;
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