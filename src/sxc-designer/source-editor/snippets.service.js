// This service delivers all snippets, translated etc. to the sourc-editor UI
angular.module("SourceEditor")
    .factory("snippetSvc", function ($http, eavConfig, svcCreator, $translate, contentTypeFieldSvc, $q) {

        // Construct a service for this specific appId
        return function createSvc(templateConfiguration, ace) {

            var svc = {
                cachedSnippets: {},
                loaded: false,
                list: null, // snippets as list in a format for the editor
                tree: null, // snippets as tree for the drop-down tool
                ace: ace,   // source editor object

                /// Main function, loads all snippets, translates
                /// returns the object tree as a promise
                getSnippets: function () {
                    if (svc.loaded)
                        return $q(function (resolve, reject) { resolve(svc.cachedSnippets); });

                    return svc.loadTable().then(function (result) {
                        // filter for token/razor snippets
                        svc.list = svc.filterAwayNotNeededSnippetsList(result.data.snippets);

                        var sets = svc.initSnippetsWithConfig(svc.list);
                        for (var x in sets)
                            svc.cachedSnippets[x] = sets[x];
                        svc.loaded = true;
                        return (svc.cachedSnippets);
                    });
                },

                initSnippetsWithConfig: function (sets) {
                    sets = svc.tree = svc.makeTree(sets);

                    sets.Content = { Fields: {}, PresentationFields: {} };
                    // maybe remove list-infos
                    if (templateConfiguration.HasList)
                        sets.List = { Fields: {}, PresentationFields: {} };

                    // maybe remove App-infos
                    if (templateConfiguration.HasApp)
                        sets.App = { Resources: {}, Settings: {} };

                    // filter for token/razor snippets
                    // svc.traverse(sets, svc.filterAwayNotNeededSnippetsTree);

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

                loadTable: function () {
                    return $http.get("../sxc-designer/snippets.json.js");
                },
                makeTree: function(list) {
                    var tree = {};
                    for (var i = 0; i < list.length; i++) {
                        var o = list[i];
                        if (tree[o.set] === undefined)
                            tree[o.set] = {};
                        if (tree[o.set][o.subset] === undefined)
                            tree[o.set][o.subset] = [];
                        var reformatted = { "key": o.name, "label": svc.label(o.set, o.subset, o.name), "snip": o.content, "help": o.help || svc.help(o.set, o.subset, o.name) };

                        tree[o.set][o.subset].push(reformatted);
                    }
                    return tree;
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

                getHelpKey: function (set, subset, snip, addition) {
                    var root = "SourceEditorSnippets";
                    var key = root + "." + set + "." + subset + "." + snip;
                    key += addition;
                    return key;
                },

                //#endregion

                //#region scan the configuration and filter unneeded snippets
                traverse: function (o, func) {
                    for (var i in o) {
                        if (!o.hasOwnProperty(i))
                            continue;
                        func.apply(this, [o, i, o[i]]);
                        //going on step down in the object tree!!
                        if (o[i] !== null && typeof (o[i]) == "object")
                            svc.traverse(o[i], func);
                    }
                },

                filterAwayNotNeededSnippetsList: function (list) {
                    var newList = [];
                    for (var i = 0; i < list.length; i++) {
                        var itm = list[i];
                        var setHasPrefix = svc.keyPrefixes.indexOf(itm.set[0]);
                        if (setHasPrefix === -1 || (setHasPrefix === svc.keyPrefixIndex)) {
                            newList.push(itm);
                            // if necessary, remove first char
                            if(setHasPrefix===svc.keyPrefixIndex)
                                itm.set = itm.set.substr(1);
                        }
                    }
                    return newList;
                },
                filterAwayNotNeededSnippetsTree: function (parent, key, value) {
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
                keyPrefixIndex: (templateConfiguration.Type.indexOf("Razor") > -1) ? 0 : 1,
                allowedKeyPrefix: (templateConfiguration.Type.indexOf("Razor") > -1) ? "@" : "[",
                //#endregion

                //#region get fields in content types
                loadContentType: function (target, type, prefix) {
                    contentTypeFieldSvc(templateConfiguration.AppId, { StaticName: type }).getFields()
                        .then(function (result) {
                            // first add common items if the content-type actually exists
                            angular.forEach(result.data, function (value) {
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

                valuePlaceholder: function (obj, val) {
                    return (templateConfiguration.Type.indexOf("Razor") > -1)
                        ? "@" + obj + "." + val
                        : "[" + obj.replace(".", ":") + ":" + val + "]";
                },

                //#endregion

                registerInEditor: function() {
                    // try to add my snippets
                    var snippetManager = ace.require("ace/snippets").snippetManager;
                    //svc.parsed = snippetManager.parseSnippetFile(svc.list, "_");
                    snippetManager.register(svc.list);
                }
            };


            return svc;
        };
    });