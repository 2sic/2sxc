
angular.module("SourceEditor")
    .factory("snippetSvc", function($http, eavConfig, svcCreator, $translate) {

        // Construct a service for this specific appId
        return function createSvc(templateConfiguration) {

            var sets = {
                "Content": {
                    "General": 
                        {
                            "Toolbar": "[Content:Toolbar]"
                        },
                    "Fields": {
                        
                    }
                },
                "Presentation": {
                    "Fields": {
                        
                    }
                },
                "List": {
                    "Header": {
                        "[ListContent:Toolbar]": null
                    },
                    "Repeaters": {
                        "<repeat... tag": "<repeat repeat=\"Employee in Data:Default\">...[Employee:Title]...</repeat>",
                        "Index0": "[Content:Repeater:Index]",
                        "Index1": "[Content:Repeater:Index1]",
                        "Count": "[Content:Repeater:Count]",
                        "IsFirst": "[Content:Repeater:IsFirst]",
                        "IsLast": "[Content:Repeater:IsLast]",
                        "Alternator2": "[Content:Repeater:Alternator2]",
                        "Alternator3": "[Content:Repeater:Alternator3]",
                        "Alternator4": "[Content:Repeater:Alternator4]",
                        "Alternator5":"[Content:Repeater:Alternator5]"
                    },
                    "Fields": {
                        
                    }
                },
                "App": {
                    "General": {
                        "[App:Path]": null,
                        "[App:PhysicalPath]": null,
                        "[App:AppGuid]": null,
                        "[App:AppId]": null,
                        "[App:Name]": null,
                        "[App:Folder]": null
                    },
                    "Resources": {
                        
                    },
                    "Settings": {
                        
                    }
                }
            };

            var svc = {
                getSnippets: function () {
                    if (!templateConfiguration.HasList)
                        delete sets.List;

                    if (!templateConfiguration.HasApp)
                        delete sets.App;

                    return sets;
                },

                help: function help(set, subset, snip) {
                    var key = svc.getHelpKey(set, subset, snip, "Help");

                    var result = $translate.instant(key);
                    if (result === key)
                        result = "";
                    return result;
                },

                label: function label(set, subset, snip) {
                    var key = svc.getHelpKey(set, subset, snip, "Key");

                    var result = $translate.instant(key);
                    if (result === key)
                        result = snip;
                    return result;
                },

                getHelpKey: function(set, subset, snip, addition) {
                    var root = "SourceEditorSnippets";
                    var key = root + "." + set + "." + subset + "." + snip;
                    key += addition;
                    return key;
                }
            };


            return svc;
        };
    });