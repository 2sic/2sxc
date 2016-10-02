// This service delivers all snippets, translated etc. to the sourc-editor UI
angular.module("SourceEditor")
    .factory("snippetSvc", function ($http, eavConfig, svcCreator, $translate, contentTypeFieldSvc, $q) {

        // Construct a service for this specific appId
        return function createSvc(templateConfiguration, ace) {

            var svc = {
                cachedSnippets: {},
                //inputTypeSnippets: {},
                inputTypes: InputTypeSnippetHandler(),
                loaded: false,
                list: [], // snippets as list in a format for the editor
                tree: null, // snippets as tree for the drop-down tool
                ace: ace, // source editor object


                // #region load jsons and prepare for binding as tree and for the editor
                /// Main function, loads all snippets, translates
                /// returns the object tree as a promise
                getSnippets: function() {
                    if (svc.loaded)
                        return $q(function(resolve, reject) { resolve(svc.cachedSnippets); });

                    return svc.loadTable().then(function(result) {
                        // filter for token/razor snippets
                        var relevant = svc.filterAwayNotNeededSnippetsList(result.data.snippets);
                        svc.inputTypes.extractInputTypeSnippets(relevant, svc.list);

                        var sets = svc.initSnippetsWithConfig(svc.list);
                        for (var x in sets)
                            svc.cachedSnippets[x] = sets[x];
                        svc.loaded = true;
                        return (svc.cachedSnippets);
                    });
                },

                initSnippetsWithConfig: function(sets) {
                    sets = svc.tree = svc.makeTree(sets);

                    //#region Retrieve all relevant content-types and infos
                    sets.Content = angular.extend({}, sets.Content, { Fields: {}, PresentationFields: {} });
                    if (templateConfiguration.TypeContent)
                        svc.loadContentType(sets.Content.Fields, templateConfiguration.TypeContent, "Content");
                    if (templateConfiguration.TypeContentPresentation)
                        svc.loadContentType(sets.Content.PresentationFields, templateConfiguration.TypeContentPresentation, "Content.Presentation");

                    if (templateConfiguration.HasList) {
                        sets.List = angular.extend({}, sets.List, { Fields: {}, PresentationFields: {} });
                        if (templateConfiguration.TypeList)
                            svc.loadContentType(sets.List.Fields, templateConfiguration.TypeList, "ListContent");
                        if (templateConfiguration.TypeListPresentation)
                            svc.loadContentType(sets.List.PresentationFields, templateConfiguration.TypeListPresentation, "ListContent.Presentation");
                    } else 
                        delete sets.List;

                    // maybe App-infos
                    if (templateConfiguration.HasApp) {
                        sets.App.Resources = {};
                        sets.App.Settings = {};
                        svc.loadContentType(sets.App.Resources, "App-Resources", "App.Resources");
                        svc.loadContentType(sets.App.Settings, "App-Settings", "App.Settings");
                    }
                    //#endregion

                    svc.cachedSnippets = sets;
                    return sets;
                },

                // load snippets from server
                loadTable: function() {
                    return $http.get("../sxc-develop/snippets.json.js");
                },

                // Convert the list into a tree with set/subset/item
                makeTree: function(list) {
                    var tree = {};
                    for (var i = 0; i < list.length; i++) {
                        var o = list[i];
                        if (tree[o.set] === undefined)
                            tree[o.set] = {};
                        if (tree[o.set][o.subset] === undefined)
                            tree[o.set][o.subset] = [];
                        var reformatted = {
                            "key": o.name,
                            "label": svc.label(o.set, o.subset, o.name),
                            "snip": o.content,
                            "help": o.help || svc.help(o.set, o.subset, o.name),
                            "links": svc.linksList(o.links)
                        };

                        tree[o.set][o.subset].push(reformatted);
                    }
                    return tree;
                },
                // #endregion

                // #region links
                linksList: function prepareLinks(linksString) {
                    if (!linksString)
                        return null;
                    var links = [];
                    var llist = linksString.split("n");
                    for (var i = 0; i < llist.length; i++) {
                        var pair = llist[i].split(":");
                        if (pair.length === 3) {
                            links.push({"name": pair[0].trim(), "url": pair[1].trim() + ":"+ pair[2].trim()});
                        }
                    }
                    if (links.length === 0) return null;
                    return links;
                },

                // #endregion links

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
                    return "SourceEditorSnippets" + "." + set + "." + subset + "." + snip + addition;
                },

                //#endregion

                //#region scan the configuration and filter unneeded snippets

                // scan the list for sets starting with @ or [ and filter if not needed right now
                filterAwayNotNeededSnippetsList: function(list) {
                    var newList = [];
                    for (var i = 0; i < list.length; i++) {
                        var itm = list[i];
                        var setHasPrefix = svc.keyPrefixes.indexOf(itm.set[0]);
                        if (setHasPrefix === -1 || (setHasPrefix === svc.keyPrefixIndex)) {
                            // if necessary, remove first char
                            if (setHasPrefix === svc.keyPrefixIndex)
                                itm.set = itm.set.substr(1);

                            newList.push(itm);
                        }
                    }
                    return newList;
                },


                keyPrefixes: ["@", "["],
                keyPrefixIndex: (templateConfiguration.Type.indexOf("Razor") > -1) ? 0 : 1,
                //#endregion

                //#region get fields in content types
                loadContentType: function(target, type, prefix) {
                    contentTypeFieldSvc(templateConfiguration.AppId, { StaticName: type }).getFields()
                        .then(function(result) {
                            // first add common items if the content-type actually exists
                            angular.forEach(result.data, function(value) {
                                var fieldname = value.StaticName;
                                target[fieldname] = {
                                    key: fieldname,
                                    label: fieldname,
                                    snip: svc.valuePlaceholder(prefix, fieldname),
                                    help: value.Metadata.merged.Notes || "" + " (" + value.Type.toLowerCase() + ") "
                                };
                        
                                // try to add generic snippets specific to this input-type
                                var snipDefaults = angular.copy(target[fieldname]); // must be a copy, because target[fieldname] will grow

                                svc.inputTypes.attachSnippets(target, prefix, fieldname, value.InputType, snipDefaults);
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

                /// add the list to the snippet manager so it works for typing
                registerInEditor: function () {
                    if (svc.list === null)
                        throw "can't register snippets because list is not loaded";
                    var snippetManager = ace.require("ace/snippets").snippetManager;
                    snippetManager.register(svc.list);
                }
            };


            return svc;
        };

        function InputTypeSnippetHandler() {
            var itsh = {
                inputTypeSnippets: {},

                extractInputTypeSnippets: function(list, standardArray) {
                    var inputTypeArray = [];

                    for (var i = 0; i < list.length; i++) {
                        var itm = list[i];
                        var systemSnippet = itm.set[0] === "\\";
                        if (!systemSnippet)
                            standardArray.push(itm);
                        else {
                            itm.set = itm.set.substr(1);
                            inputTypeArray.push(itm);
                        }
                    }
                    itsh.inputTypeSnippets = itsh.catalogInputTypeSnippets(inputTypeArray);
                },

                catalogInputTypeSnippets: function(list) {
                    var inputTypeList = {};
                    for (var i = 0; i < list.length; i++) {
                        if (inputTypeList[list[i].subset] === undefined)
                            inputTypeList[list[i].subset] = [];
                        inputTypeList[list[i].subset].push(list[i]);
                    }
                    return inputTypeList;
                },

                attachSnippets: function(target, prefix, fieldname, inputType, snipDefaults) {
                    var genericSnippet = itsh.inputTypeSnippets[inputType];
                    if (inputType.indexOf("-")) {   // if it's a sub-type, let's also get the master-type
                        var fieldType = inputType.substr(0, inputType.indexOf("-"));
                        if (fieldType) {
                            var typeSnips = itsh.inputTypeSnippets[fieldType];
                            if (typeSnips)
                                genericSnippet = genericSnippet ? genericSnippet.concat(typeSnips) : typeSnips;
                        }
                    }
                    if (!genericSnippet)
                        return;

                    if (target[fieldname].more === undefined)
                        target[fieldname].more = [];
                    var fieldSnips = target[fieldname].more;
                    for(var g = 0;g < genericSnippet.length;g++)
                        try {
                            fieldSnips[fieldname + "-" + genericSnippet[g].name] = angular.extend({}, snipDefaults, {
                                key: fieldname + " - " + genericSnippet[g].name,
                                label: genericSnippet[g].name,
                                snip: itsh.localizeGenericSnippet(genericSnippet[g].content, prefix, fieldname),
                                collapse: true
                            });
                        } finally {
                        }

                },

                localizeGenericSnippet: function(snip, objName, fieldName) {
                    snip = snip.replace(/(\$\{[0-9]+\:)var(\})/gi, "$1" + objName + "$2")
                        .replace(/(\$\{[0-9]+\:)prop(\})/gi, "$1" + fieldName + "$2");
                    return snip;
                }
            };
            return itsh;
        }
    });