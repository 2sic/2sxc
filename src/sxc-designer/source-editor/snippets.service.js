// This service delivers all snippets, translated etc. to the sourc-editor UI
angular.module("SourceEditor")
    .factory("snippetSvc", function($http, eavConfig, svcCreator, $translate, contentTypeFieldSvc, $q) {

        // Construct a service for this specific appId
        return function createSvc(templateConfiguration) {

            var svc = {
                cachedSnippets: {},
                loaded: false,

                /// Main function, loads all snippets, translates
                /// returns the object tree as a promise
                getSnippets: function() {
                    if (svc.loaded)
                        return $q(function(resolve, reject) { resolve(svc.cachedSnippets); });

                    return svc.loadSnippets().then(function(result) {
                        var sets = svc.initSnippetsWithConfig(result.data.Snippets);
                        for (var x in sets)
                            svc.cachedSnippets[x] = sets[x];
                        svc.loaded = true;
                        return (svc.cachedSnippets);
                    });
                },

                initSnippetsWithConfig: function(sets) {

                    // maybe remove list-infos
                    if (!templateConfiguration.HasList)
                        delete sets.List;

                    // maybe remove App-infos
                    if (!templateConfiguration.HasApp)
                        delete sets.App;

                    // filter for token/razor snippets
                    svc.traverse(sets, svc.filterAwayNotNeededSnippets);

                    angular.forEach(sets, function(setValue, setKey) {
                        angular.forEach(setValue, function(subSetValue, subSetKey) {
                            angular.forEach(subSetValue, function(itemValue, itemKey) {
                                svc.expandSnippetInfo(subSetValue, setKey, subSetKey, itemKey, itemValue);
                            });
                        });
                    });

                    //#region Retrieve all relevant content-types and infos
                    if (templateConfiguration.TypeContent)
                        svc.loadContentType(sets.Content.Fields, templateConfiguration.TypeContent, "Content");
                    if (templateConfiguration.TypeContentPresentation)
                        svc.loadContentType(sets.Content.PresentationFields, templateConfiguration.TypeContentPresentation, "Content.Presentation");
                    if (templateConfiguration.TypeList)
                        svc.loadContentType(sets.List.Fields, templateConfiguration.TypeList, "List");
                    if (templateConfiguration.TypeListPresentation)
                        svc.loadContentType(sets.List.PresentationFields, templateConfiguration.TypeListPresentation, "List.Presentation");

                    if (templateConfiguration.HasApp) {
                        svc.loadContentType(sets.App.Resources, "App-Resources", "App.Resources");
                        svc.loadContentType(sets.App.Settings, "App-Settings", "App.Settings");
                    }
                    //#endregion

                    svc.cachedSnippets = sets;
                    return sets;
                },

                loadSnippets: function() {
                    return $http.get("../sxc-admin/source-editor-snippets.js");
                },

                //#region help / translate
                help: function help(set, subset, snip) {
                    var key = svc.getHelpKey(set, subset, snip, ".Help");

                    var result = $translate.instant(key);
                    if (result === key)
                        result = "";
                    return result;
                },

                label: function label(set, subset, snip) {
                    var key = svc.getHelpKey(set, subset, snip, ".Key");

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
                },

                //#endregion

                //#region scan the configuration and filter unneeded snippets
                traverse: function(o, func) {
                    for (var i in o) {
                        if (!o.hasOwnProperty(i))
                            continue;
                        func.apply(this, [o, i, o[i]]);
                        //going on step down in the object tree!!
                        if (o[i] !== null && typeof (o[i]) == "object")
                            svc.traverse(o[i], func);
                    }
                },

                filterAwayNotNeededSnippets: function(parent, key, value) {
                    // check if we have a special prefix
                    var prefix = key[0];
                    var found = svc.keyPrefixes.indexOf(prefix);

                    // always remove the original, even if not necessary, to preserve order
                    delete parent[key];

                    if (found !== -1) {
                        if (prefix !== svc.allowedKeyPrefix)
                            return; // don't even add it any more
                        key = key.substr(1);
                    }

                    parent[key] = value;
                },

                keyPrefixes: ["@", "["],
                allowedKeyPrefix: (templateConfiguration.Type.indexOf("Razor") > -1) ? "@" : "[",
                //#endregion

                expandSnippetInfo: function(target, setName, subsetName, key, value) {
                    if (value instanceof Object)
                        return;
                    target[key] = { "key": key, "label": svc.label(setName, subsetName, key), "snip": value, "help": svc.help(setName, subsetName, key) };
                },

                //#region get fields in content types
                loadContentType: function(target, type, prefix) {
                    contentTypeFieldSvc(templateConfiguration.AppId, { StaticName: type }).getFields()
                        .then(function(result) {
                            // first add common items if the content-type actually exists
                            angular.forEach(result.data, function(value) {
                                var fieldname = value.StaticName;
                                var description = value.Metadata.merged.Notes || "" + " (" + value.Type.toLowerCase() + ") ";
                                var placeholder = svc.valuePlaceholder(prefix, fieldname);
                                target[fieldname] = {
                                    key: fieldname,
                                    label: fieldname,
                                    snip: placeholder,
                                    help: description
                                };
                            });

                            var std = ["EntityId", "EntityTitle", "EntityGuid", "EntityType", "IsPublished", "Modified"];
                            if (result.data.length)
                                for (var i = 0; i < std.length; i++)
                                    target[std[i]] = {
                                        key: std[i],
                                        label: std[i],
                                        snip: svc.valuePlaceholder(prefix, std[i]),
                                        help: $translate.instant("SourceEditorSnippets.StandardFields." + std[i] + ".Help")
                                    };

                        });
                },

                valuePlaceholder: function(obj, val) {
                    return (templateConfiguration.Type.indexOf("Razor") > -1)
                        ? "@" + obj + "." + val
                        : "[" + obj.replace(".", ":") + ":" + val + "]";
                },

                //#endregion

                /*jshint multistr: true */

                snippetsToRegister: function() {
                    var testSnippets = {};
                    testSnippets.snippetText = "# Some useful 2sxc tags / placeholders \n\
# toolbar\n\
snippet toolbar \n\
	[${1:Content}:Toolbar]\n\
";
                    testSnippets.scope = "_";// "html";
                    return testSnippets;
                }

            };


            return svc;
        };
    });