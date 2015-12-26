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
    EditorController.$inject = ["sourceSvc", "snippetSvc", "item", "$modalInstance", "$scope", "$translate"];

}());
(function () {
    /*jshint multistr: true */

    angular.module("SourceEditor")
        .constant("snippets", {
            "tokens":
                "# Some useful 2sxc tags / placeholders \n\
# toolbar\n\
snippet toolbar \n\
key Toolbar \n\
title Toolbar \n\
help Toolbar for inline editing with 2sxc. If used inside a <div class=\"sc-element\"> then the toolbar will automatically float \n\
	[${1:Content}:Toolbar]\n\
",


            "html": "",


            "razor": "# Some useful 2sxc tags / placeholders \n\
#######################\n\
### Razor App stuff\n\
# path\n\
set app\n\
title Path \n\
help returns the url to the current app, for integrating scripts, images etc. For example, use as ***\/scripts\/knockout.js\n\
snippet path \n\
	@App.Path\n\
# physical path\n\
set app\n\
title Physical path \n\
help physical path, in c:\\\n\
snippet physical path \n\
	@App.PhysicalPath\n\
# App Guid \n\
set app\n\
title App Guid \n\
help internal GUID - should stay the same across all systems for this specific App \n\
snippet app guid \n\
	@App.AppGuid\n\
# App Id \n\
set app\n\
title App Id \n\
help Id in the current data base. Is a different number in every App-Installation \n\
snippet app id \n\
	@App.AppId\n\
# App Name \n\
set app\n\
title App Name \n\
help internal name \n\
snippet app name \n\
	@App.Name\n\
# App Folder \n\
set app\n\
title App Folder \n\
help folder of the 2sxc-app, often used to create paths to scripts or join some values. if you only need to reference a script, please use App.Path \n\
snippet app folder \n\
	@App.Folder\n\
            ",
            "razor2": {
                "App": [
                    {
                        "key": "path",
                        "title": "Path",
                        "help": "returns the url to the current app, for integrating scripts, images etc. For example, use as ***\/scripts\/knockout.js",
                        "snippet": "path",
                        "content": "@App.Path"
                    },
                    {
                        "key": "physical path",
                        "title": "Physical path",
                        "help": "physical path, in c:\\",
                        "snippet": "physical path",
                        "content": "@App.PhysicalPath"
                    },
                    {
                        "key": "App Guid",
                        "title": "App Guid",
                        "help": "internal GUID - should stay the same across all systems for this specific App",
                        "snippet": "app guid",
                        "content": "@App.AppGuid"
                    },
                    {
                        "key": "App Id",
                        "title": "App Id",
                        "help": "Id in the current data base. Is a different number in every App-Installation",
                        "snippet": "app id",
                        "content": "@App.AppId"
                    },
                    {
                        "key": "App Name",
                        "title": "App Name",
                        "help": "internal name",
                        "snippet": "app name",
                        "content": "@App.Name"
                    },
                    {
                        "key": "App Folder",
                        "title": "App Folder",
                        "help": "folder of the 2sxc-app, often used to create paths to scripts or join some values. if you only need to reference a script, please use App.Path",
                        "snippet": "app folder",
                        "content": "@App.Folder"
                    }
                ]

            }
        });


} ());
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
