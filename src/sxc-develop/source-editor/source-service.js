
angular.module("SourceEditor")
    .factory("sourceSvc", function($http) {

        // Construct a service for this specific appId
        return function createSvc(key) {

            // if the key is a string, then it's to be used as a path, otherwise as a template-id
            var params = isNaN(key)
                ? { path: key }
                : { templateId: key };

            var svc = {
                get: function() {
                    return $http.get("app/appassets/asset", { params: params })
                        .then(function(result) {
                            var data = result.data;
                            if (data.Type.toLowerCase() === "auto") {
                                switch(data.Extension.toLowerCase()) {
                                    case ".cs":
                                    case ".cshtml":
                                        data.Type = "Razor";
                                        break;
                                    case ".html":
                                    case ".css":
                                    case ".js":
                                        data.Type = "Token";
                                        break;
                                }
                            }
                            return result;
                        });
                },

                save: function(item) {
                    return $http.post("app/appassets/asset", item, { params: params });
                }
            };

            return svc;
        };
    });