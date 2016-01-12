(function () {

    angular.module("SourceEditor", [
            "EavConfiguration",
            "EavServices",
            "SxcServices",
            "SxcTemplates",
            "pascalprecht.translate",
            "ui.ace"
        ])
        .config(["$translatePartialLoaderProvider", function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("source-editor-snippets");
        }]);

} ());
(function () {

    EditorController.$inject = ["sourceSvc", "snippetSvc", "item", "$modalInstance", "$scope", "$translate"];
    angular.module("SourceEditor")

        .controller("Editor", EditorController)
    ;

    function EditorController(sourceSvc, snippetSvc, item, $modalInstance, $scope, $translate) {
        $translate.refresh();   // necessary to load stuff added in this lazy-loaded app

        var vm = this;
        var svc = sourceSvc(item.EntityId);
        vm.view = {};
        vm.editor = null;

        svc.get().then(function (result) {
            vm.view = result.data;
            svc.initSnippets(vm.view);
        });

        // load appropriate snippets from the snippet service
        svc.initSnippets = function (template) {
            vm.snipSvc = snippetSvc(template, ace);
            vm.snipSvc.getSnippets().then(function (result) {
                vm.snippets = result;
                vm.snippetSet = "Content";    // select default
                vm.snippetHelp = vm.snipSvc.help;
                vm.snippetLabel = vm.snipSvc.label;

                // now register the snippets in the editor
                vm.registerSnippets();
            });
        };

        vm.close = function () { $modalInstance.dismiss("cancel"); };

        vm.save = function () {
            svc.save(vm.view).then(vm.close);
        };

        vm.addSnippet = function addSnippet(snippet) {
            var snippetManager = ace.require("ace/snippets").snippetManager;
            snippetManager.insertSnippet(vm.editor, snippet);
            vm.editor.focus();
        };

        vm.registerSnippets = function registerSnippets() {
            // ensure we have everything
            if (!(vm.snipSvc && vm.editor))
                return;
            // try to add my snippets
            vm.snipSvc.registerInEditor();
            //var snippetManager = ace.require("ace/snippets").snippetManager;
            //var snippets = vm.snipSvc.snippetsToRegister();
            //var parsed = snippetManager.parseSnippetFile(snippets.snippetText, snippets.scope);
            //snippetManager.register(parsed);
        };

        // this event is called when the editor is ready
        $scope.aceLoaded = function (_editor) {
            vm.editor = _editor;        // remember the editor for later actions
            vm.registerSnippets();      // try to register the snippets
        };

    }

}());
// This service delivers all snippets, translated etc. to the sourc-editor UI
angular.module("SourceEditor")
    .factory("snippetSvc", ["$http", "eavConfig", "svcCreator", "$translate", "contentTypeFieldSvc", "$q", function ($http, eavConfig, svcCreator, $translate, contentTypeFieldSvc, $q) {

        // Construct a service for this specific appId
        return function createSvc(templateConfiguration, ace) {

            var svc = {
                cachedSnippets: {},
                loaded: false,
                list: null, // snippets as list in a format for the editor
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
                        svc.list = svc.filterAwayNotNeededSnippetsList(result.data.snippets);

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
                    sets.Content = { Fields: {}, PresentationFields: {} };
                    if (templateConfiguration.TypeContent)
                        svc.loadContentType(sets.Content.Fields, templateConfiguration.TypeContent, "Content");
                    if (templateConfiguration.TypeContentPresentation)
                        svc.loadContentType(sets.Content.PresentationFields, templateConfiguration.TypeContentPresentation, "Content.Presentation");

                    if (templateConfiguration.HasList) {
                        sets.List = { Fields: {}, PresentationFields: {} };
                        if (templateConfiguration.TypeList)
                            svc.loadContentType(sets.List.Fields, templateConfiguration.TypeList, "List");
                        if (templateConfiguration.TypeListPresentation)
                            svc.loadContentType(sets.List.PresentationFields, templateConfiguration.TypeListPresentation, "List.Presentation");
                    }

                    // maybe App-infos
                    if (templateConfiguration.HasApp) {
                        sets.App = { Resources: {}, Settings: {} };
                        svc.loadContentType(sets.App.Resources, "App-Resources", "App.Resources");
                        svc.loadContentType(sets.App.Settings, "App-Settings", "App.Settings");
                    }
                    //#endregion

                    svc.cachedSnippets = sets;
                    return sets;
                },

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
                        var reformatted = { "key": o.name, "label": svc.label(o.set, o.subset, o.name), "snip": o.content, "help": o.help || svc.help(o.set, o.subset, o.name) };

                        tree[o.set][o.subset].push(reformatted);
                    }
                    return tree;
                },
                // #endregion

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
                            newList.push(itm);
                            // if necessary, remove first char
                            if (setHasPrefix === svc.keyPrefixIndex)
                                itm.set = itm.set.substr(1);
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
                                var description = value.Metadata.merged.Notes || "" + " (" + value.Type.toLowerCase() + ") ";
                                var placeholder = svc.valuePlaceholder(prefix, fieldname);
                                var snipDef = target[fieldname] = {
                                    key: fieldname,
                                    label: fieldname,
                                    snip: placeholder,
                                    help: description
                                };
                                // add more snippets if it could be an image
                                if (value.Type === "Hyperlink") {
                                    target[fieldname + "-tu"] = angular.extend({}, snipDef, {
                                        key: fieldname + " - thumb url",
                                        label: fieldname + " - thumb url",
                                        snip: svc.imageResizeSnippet(snipDef.snip, false)
                                    });
                                    target[fieldname + "-ti"] = angular.extend({}, snipDef, {
                                        key: fieldname + " - thumb img",
                                        label: fieldname + " - thumb img",
                                        snip: svc.imageResizeSnippet(snipDef.snip, true)
                                    });
                                }
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

                imageResizeSnippet: function(basicPlaceholder, addImgTag) {
                    var snip = basicPlaceholder + "?w=${1:200}&h=${2:200}&mode=${3:crop}";
                    if (addImgTag)
                        snip = "<img src=\"" + snip + "\">";
                    return snip;
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
    }]);

angular.module("SourceEditor")
    .factory("sourceSvc", ["$http", function($http) {

        // Construct a service for this specific appId
        return function createSvc(templateId) {
            var svc = {
                get: function() {
                    return $http.get('view/template/template', { params: { templateId: templateId } });
                },

                save: function(item) {
                    return $http.post('view/template/template', item, { params: { templateId: templateId} });
                }

            };

            return svc;
        };
    }]);
(function () {

    angular.module("SourceEditor")

        // helps convert an object with keys to an array to allow sorting
        // from https://github.com/petebacondarwin/angular-toArrayFilter
        .filter('toArray', function() {
            return function(obj, addKey) {
                if (!angular.isObject(obj)) return obj;
                if (addKey === false) {
                    return Object.keys(obj).map(function(key) {
                        return obj[key];
                    });
                } else {
                    return Object.keys(obj).map(function(key) {
                        var value = obj[key];
                        return angular.isObject(value) ?
                            Object.defineProperty(value, '$key', { enumerable: false, value: key }) :
                            { $key: key, $value: value };
                    });
                }
            };
        });


} ());
angular.module('SourceEditor').run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('source-editor/editor.html',
    "<div ng-click=vm.debug.autoEnableAsNeeded($event)><div class=modal-header><button class=\"btn btn-default btn-square btn-subtle pull-right\" type=button ng-click=vm.close()><i icon=remove></i></button><h3 class=modal-title translate=SourceEditor.Title></h3></div><div class=modal-body><div class=row><div class=col-md-8><div tooltip=\"{{ vm.view.FileName }}\">{{ vm.view.FileName.substr(vm.view.FileName.lastIndexOf(\"\\\\\") + 1) }} ({{vm.view.Type }})</div><div ng-model=vm.view.Code style=\"height: 400px\" ui-ace=\"{\r" +
    "\n" +
    "                    useWrapMode : true,\r" +
    "\n" +
    "                    useSoftTabs: true,\r" +
    "\n" +
    "                    showGutter: true,\r" +
    "\n" +
    "                    theme:'twilight',\r" +
    "\n" +
    "                    mode: 'html',\r" +
    "\n" +
    "                    onLoad: aceLoaded,\r" +
    "\n" +
    "                    require: ['ace/ext/language_tools'],\r" +
    "\n" +
    "                    advanced: {\r" +
    "\n" +
    "                        enableSnippets: true,\r" +
    "\n" +
    "                        enableBasicAutocompletion: true,\r" +
    "\n" +
    "                        enableLiveAutocompletion: true\r" +
    "\n" +
    "                    }}\"></div></div><div class=\"pull-right col-md-4\"><div><strong translate=SourceEditor.SnippetsSection.Title></strong> <i icon=question-sign style=\"opacity: 0.3\" ng-click=\"showSnippetInfo = !showSnippetInfo\"></i><div ng-if=showSnippetInfo translate=SourceEditor.SnippetsSection.Intro></div></div><select class=input-lg width=100% ng-model=vm.snippetSet ng-options=\"key as ('SourceEditorSnippets.' + key + '.Title' | translate) for (key , value) in vm.snippets\" tooltip=\"{{ 'SourceEditorSnippets.' + vm.snippetSet + '.Help'  | translate}}\"></select><div>&nbsp;</div><div ng-repeat=\"(subsetName, subsetValue) in vm.snippets[vm.snippetSet]\"><strong tooltip=\"{{ 'SourceEditorSnippets.' + vm.snippetSet + '.' + subsetName + '.Help'  | translate}}\">{{ 'SourceEditorSnippets.' + vm.snippetSet + '.' + subsetName + '.Title' | translate}}</strong><ul><li ng-repeat=\"value in subsetValue | toArray | orderBy: '$key'\" tooltip=\"{{ value.snip }}\"><span ng-click=vm.addSnippet(value.snip)>{{value.label}}</span> <i icon=info-sign style=\"opacity: 0.3\" ng-click=\"show = !show\" ng-show=value.help></i><div ng-if=show><em>{{value.help}}</em></div></li></ul></div></div></div></div><div class=modal-footer><button class=\"btn btn-primary btn-square btn-lg pull-left\" type=button ng-click=vm.save()><i icon=ok></i></button></div><show-debug-availability class=pull-right></show-debug-availability></div>"
  );

}]);
