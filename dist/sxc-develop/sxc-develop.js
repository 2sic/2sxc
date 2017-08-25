(function () {

    angular.module("SourceEditor", [
            "EavConfiguration",
            "EavServices",
            "SxcServices",
            "SxcTemplates",
            "pascalprecht.translate",
            "ui.ace"
        ])
        /*@ngInject*/
        .config(["$translatePartialLoaderProvider", function ($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("source-editor-snippets");
        }]);

} ());


angular.module('SourceEditor').component('devFiles', {
    templateUrl: 'source-editor/dev-files.html',
    /*@ngInject*/
    controller: ["appAssetsSvc", "appId", function (appAssetsSvc, appId) {
        var vm = angular.extend(this, {
            show: false,
            svc: appAssetsSvc(appId),

            toggle: function() {
                vm.show = !vm.show;
                if (!vm.assets)
                    vm.assets = vm.svc.liveList();
            },

            editFile: function(filename) {
                window.open(vm.assembleUrl(filename));
                vm.toggle();
            },

            assembleUrl: function(newFileName) {
                // note that as of now, we'll just use the initial url and change the path
                // then open a new window
                var url = window.location.href;
                var newItems = JSON.stringify([{ Path: newFileName }]);
                return url.replace(new RegExp("items=.*?%5d", "i"), "items=" + encodeURI(newItems)); // note: sometimes it doesn't have an appid, so it's [0-9]* instead of [0-9]+
            },

            addFile: function() {
                // todo: i18n
                var result = prompt("please enter full file name"); // $translate.instant("AppManagement.Prompt.NewApp"));
                if (result)
                    vm.svc.create(result);

            }
        });

    }],
    controllerAs: "vm",
    bindings: {
        fileName: "<",
        type: "<"
    }
});
(function () {

EditorController.$inject = ["sourceSvc", "snippetSvc", "appAssetsSvc", "appId", "sxcDialogs", "items", "$uibModalInstance", "$window", "$scope", "$translate", "saveToastr", "ctrlS", "debugState"];
angular.module("SourceEditor").component("editor", {
    templateUrl: "source-editor/editor.html",
    controller: EditorController,
    controllerAs: "vm"
});

/*@ngInject*/
function EditorController(sourceSvc, snippetSvc, appAssetsSvc, appId, sxcDialogs, items, $uibModalInstance, $window, $scope, $translate, saveToastr, ctrlS, debugState) {
    // todo: must re-think this, nicer would be if it's a proper parameter
    var item = items[0];

    $translate.refresh();   // necessary to load stuff added in this lazy-loaded app

    var vm = this;
    vm.debug = debugState;

    // if item is an object with EntityId, it referrs to a template, otherwise it's a relative path

    var svc = sourceSvc(item.EntityId !== undefined ? item.EntityId : item.Path);

    vm.view = {};
    vm.tempCodeBecauseOfBug = "";
    vm.editor = null;

    svc.get().then(function (result) {
        vm.view = result.data;
        svc.initSnippets(vm.view);
    });

    // load appropriate snippets from the snippet service
    svc.initSnippets = function (template) {
        vm.snipSvc = snippetSvc(template, ace);
        vm.snipSvc.getSnippets().then(function (result) {
            vm.snippets = result;   // prep for binding to the snippet-selector

            // now register the snippets in the editor
            vm.registerSnippets();
        });
    };

    //#region close / prevent-close
    vm.close = function () {
        if (!confirm($translate.instant("Message.ExitOk")))
            return;
        window.close();
    };

    // prevent all kind of closing when accidentally just clicking on the side of the dialog
    $scope.$on("modal.closing", function (e) { e.preventDefault(); });

    $window.addEventListener("beforeunload", function (e) {
        var unsavedChangesText = $translate.instant("Message.ExitOk");
        (e || window.event).returnValue = unsavedChangesText; //Gecko + IE
        return unsavedChangesText; //Gecko + Webkit, Safari, Chrome etc.
    });

    //#endregion

    //#region save
    vm.save = function (autoClose) {
        var after = autoClose ? vm.close : function () { };

        //#region bugfix 607
        // check if there is still some temp-snippet which we must update first 
        // - because of issue https://github.com/2sic/2sxc/issues/607
        // it's very important that we place the text into a copy of the variable
        // and NOT in the view.Code, otherwise undo will stop working
        var latestCode = vm.editor.getValue();
        var savePackage = angular.copy(vm.view);
        if (savePackage.Code !== latestCode) //{
            savePackage.Code = latestCode;
        //#endregion

        // now save with appropriate toaster
        saveToastr(svc.save(savePackage)).then(after);
    };
    //#endregion

    activate();

    function activate() {
        // add ctrl+s to save
        ctrlS(function () { vm.save(false); });


    }

    //#region snippets
    vm.registerSnippets = function registerSnippets() {
        // ensure we have everything first (this may be called multiple times), then register them
        if (!(vm.snipSvc && vm.editor))
            return;
        vm.snipSvc.registerInEditor();
    };
    //#endregion

    // this event is called when the editor is ready
    vm.aceLoaded = function (_editor) {
        vm.editor = _editor;        // remember the editor for later actions
        vm.registerSnippets();      // try to register the snippets
    };

}


}());


angular.module('SourceEditor').component('snippetsLinks', {
    templateUrl: 'source-editor/snippets-links.html',
    /*@ngInject*/
    controller: function () {
        //var vm = this;
    },
    controllerAs: "vm",
    bindings: {
        links: "<"
    }
});


angular.module('SourceEditor').component('snippets', {
    templateUrl: 'source-editor/snippets.html',
    /*@ngInject*/
    controller: function () {
        var vm = this;

        // default set
        vm.snippetSet = "Content";

        vm.addSnippet = function addSnippet(snippet) {
            var snippetManager = ace.require("ace/snippets").snippetManager;
            snippetManager.insertSnippet(vm.editor, snippet);
            vm.editor.focus();
        };

        vm.$onInit = function () {
            console.log("component snip loading");
            console.log("def set" + vm.snippetSet);
        };

        vm.$onChanges = function() {
            console.log("def set" + vm.snippetSet);
        };

    },
    controllerAs: "vm",
    bindings: {
        snippets: "<",
        editor: "<"
    }
});
// This service delivers all snippets, translated etc. to the sourc-editor UI
angular.module("SourceEditor")
    /*@ngInject*/
    .factory("snippetSvc", ["$http", "eavConfig", "svcCreator", "$translate", "contentTypeFieldSvc", "$q", function ($http, eavConfig, svcCreator, $translate, contentTypeFieldSvc, $q) {

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
    }]);

angular.module("SourceEditor")
    /*@ngInject*/
    .factory("sourceSvc", ["$http", function ($http) {

        // Construct a service for this specific appId
        return function createSvc(key) {

            // if the key is a string, then it's to be used as a path, otherwise as a template-id
            var params = isNaN(key)
                ? { path: key }
                : { templateId: key };

            var svc = {
                get: function() {
                    return $http.get("app-sys/appassets/asset", { params: params })
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
                    return $http.post("app-sys/appassets/asset", item, { params: params });
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
angular.module("DevTemplates", []).run(["$templateCache", function($templateCache) {$templateCache.put("source-editor/dev-files.html","<div uib-tooltip=\"{{ vm.fileName }}\" ng-click=\"vm.toggle()\">\r\n    {{ vm.fileName.substr(vm.fileName.lastIndexOf(\"\\\\\") + 1) }}\r\n    ({{vm.type }})\r\n    <i ng-class=\'{\"eav-icon-plus-squared\": !vm.show, \"eav-icon-minus-squared\": vm.show}\'></i>\r\n</div>\r\n<div ng-if=\"vm.show\">\r\n    <h4>quick-pick another file</h4>\r\n    <ol>\r\n        <li ng-repeat=\"asset in vm.assets\" ng-click=\"vm.editFile(asset)\">\r\n            {{asset}}\r\n        </li>\r\n    </ol>\r\n    <ul>\r\n        <li ng-click=\"vm.addFile()\">create file <span class=\"eav-icon-plus-circled\"></span></li>\r\n        <li>for copy, rename etc. please use the dnn file manager</li>\r\n    </ul>\r\n</div>");
$templateCache.put("source-editor/editor.html","<div ng-click=\"vm.debug.autoEnableAsNeeded($event)\">\r\n    <div class=\"modal-header\">\r\n        <h3 class=\"modal-title\" translate=\"SourceEditor.Title\"></h3>\r\n    </div>\r\n\r\n    <div class=\"modal-body\">\r\n        <div class=\"row\">\r\n            <div class=\"col-md-8\">\r\n                <dev-files file-name=\"vm.view.FileName\" type=\"vm.view.Type\"></dev-files>\r\n\r\n                <div ng-model=\"vm.view.Code\" style=\"height: 600px;\" ui-ace=\"{\r\n                    useWrapMode : true,\r\n                    useSoftTabs: true,\r\n                    showGutter: true,\r\n                    theme:\'sqlserver\',\r\n                    mode: \'razor\',\r\n                    onLoad: vm.aceLoaded,\r\n                    require: [\'ace/ext/language_tools\', \'//xyz/something\'],\r\n                    advanced: {\r\n                        enableSnippets: true,\r\n                        enableBasicAutocompletion: true,\r\n                        enableLiveAutocompletion: true\r\n                    },\r\n                    rendererOptions: {\r\n                        fontSize: 16\r\n                    }\r\n                }\">\r\n                </div>\r\n            </div>\r\n            <div class=\"pull-right col-md-4\">\r\n                <!-- snippets wrapper - should ensure scrolling-->\r\n                <snippets ng-if=\"vm.snippets\" snippets=\"vm.snippets\" editor=\"vm.editor\"></snippets>\r\n            </div>\r\n        </div>\r\n\r\n    </div>\r\n    <div class=\"modal-footer\">\r\n        <div class=\"pull-left\">\r\n            <button class=\"btn btn-primary btn-lg xxbtn-square\" type=\"button\" ng-click=\"vm.save(false)\">\r\n                <span icon=\"check\" uib-tooltip=\"{{ \'Button.SaveAndKeepOpen\' | translate }}\"></span>\r\n                {{ \'Button.SaveAndKeepOpen\' | translate }}\r\n            </button>\r\n            also supports Ctrl+S\r\n        </div>\r\n    </div>\r\n    <show-debug-availability class=\"pull-right\"></show-debug-availability>\r\n    <div ng-if=\"vm.debug.on\">\r\n        <pre>{{vm.view.Code}}</pre>\r\n    </div>\r\n</div>\r\n\r\n<style>\r\n    /* helper to ensure that razor (which is correctly detected by ACE) is also highlighted */\r\n    .ace_razor {\r\n        background-color: yellow;\r\n    }\r\n\r\n    /* make sure the highlighted text is also black, otherwise it a kind of gray */\r\n    .ace_punctuation.ace_short.ace_razor {\r\n        color: black;\r\n    }\r\n\r\n    .ace_punctuation.ace_block.ace_razor {\r\n        color: black;\r\n    }\r\n</style>");
$templateCache.put("source-editor/snippets-links.html","<div>\r\n    <div ng-repeat=\"link in vm.links\">\r\n        &gt; <a href=\"{{link.url}}\" target=\"_blank\">{{link.name}}</a>\r\n    </div>\r\n</div>");
$templateCache.put("source-editor/snippets.html","<div>\r\n    <strong translate=\"SourceEditor.SnippetsSection.Title\"></strong> <i icon=\"question-sign\" style=\"opacity: 0.3\" ng-click=\"showSnippetInfo = !showSnippetInfo\"></i>\r\n    <div ng-if=\"showSnippetInfo\" translate=\"SourceEditor.SnippetsSection.Intro\"></div>\r\n</div>\r\n<select class=\"input-lg\"\r\n        style=\"width: 90%\"\r\n        ng-model=\"vm.snippetSet\"\r\n        ng-options=\"key as (\'SourceEditorSnippets.\' + key + \'.Title\' | translate) for (key , value) in vm.snippets\"\r\n        uib-tooltip=\"{{ \'SourceEditorSnippets.\' + vm.snippetSet + \'.Help\'  | translate}}\"></select>\r\n<div>&nbsp;</div>\r\n<div style=\"height: 500px; overflow: auto\">\r\n    <div ng-repeat=\"(subsetName, subsetValue) in vm.snippets[vm.snippetSet]\">\r\n        <strong uib-tooltip=\"{{ \'SourceEditorSnippets.\' + vm.snippetSet + \'.\' + subsetName + \'.Help\'  | translate}}\">{{ \'SourceEditorSnippets.\' + vm.snippetSet + \'.\' + subsetName + \'.Title\' | translate}}</strong>\r\n        <ul>\r\n            <li ng-repeat=\"value in subsetValue | toArray | orderBy: \'$key\'\" uib-tooltip=\"{{ value.snip }}\">\r\n                <span ng-mouseover=\"showAdd = true\" ng-click=\"show = !show\" ng-mouseout=\"showAdd = false\">\r\n                    <i class=\"eav-icon-plus-squared\"\r\n                       ng-click=\"vm.addSnippet(value.snip)\"\r\n                       ng-show=\"showAdd\"\r\n                       stop-event=\"click\"></i>\r\n                    {{value.label}}\r\n\r\n                    <i icon=\"info-sign\" style=\"opacity: 0.3\" ng-show=\"value.help\"></i>\r\n                    <a ng-show=\"value.more\"\r\n                       ng-click=\"showMore = !showMore\"\r\n                       stop-event=\"click\">\r\n                        <i icon=\"plus\"></i>more\r\n                    </a>\r\n                </span>\r\n                <div ng-if=\"show\">\r\n                    <em>{{value.help}}</em>\r\n                    <snippets-links links=\"value.links\" ng-if=\"value.links\"></snippets-links>\r\n                </div>\r\n                <ul ng-if=\"showMore\">\r\n                    <li ng-repeat=\"more in value.more | toArray | orderBy: \'$key\'\" uib-tooltip=\"{{ value.snip }}\">\r\n                        <span ng-click=\"vm.addSnippet(more.snip)\">{{more.label}}</span>\r\n                        <i icon=\"info-sign\" style=\"opacity: 0.3\" ng-click=\"show = !show\" ng-show=\"more.help\"></i>\r\n                        <div ng-if=\"show\">\r\n                            <em>{{more.help}}</em>\r\n                            <snippets-links links=\"more.links\" ng-if=\"more.links\"></snippets-links>\r\n                        </div>\r\n                </ul>\r\n            </li>\r\n        </ul>\r\n\r\n    </div>\r\n</div>\r\n");}]);