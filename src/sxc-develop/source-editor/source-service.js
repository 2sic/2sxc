
angular.module("SourceEditor")
    .factory("sourceSvc", function($http) {

        // Construct a service for this specific appId
        return function createSvc(templateId) {
            var svc = {
                get: function() {
                    return $http.get("app/appassets/template", { params: { templateId: templateId } });
                },

                save: function(item) {
                    return $http.post("app/appassets/template", item, { params: { templateId: templateId } });
                }

            };

            return svc;
        };
    });