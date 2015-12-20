
angular.module("SourceEditor")
    .factory("helpSvc", function($http, eavConfig, svcCreator) {

        // Construct a service for this specific appId
        return function createSvc(templateId) {

            var sets = {
                "Main": [
                    { "Stuff": "[Content:stuff]" },
                    { "Other": "[Content:other]" }
                ],
                "List": [
                    { "Title": "[List:Something]" },
                    { "Other": "[List:Other]" },
                    { "then": "[List:then]" }
                ]
            };

            var svc = {
                getSnippets: function() {
                    return sets;
                }


            };

            return svc;
        };
    });