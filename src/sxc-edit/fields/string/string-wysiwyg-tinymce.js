
(function () {
	"use strict";

    // Register in Angular Formly
    angular.module("sxcFieldTemplates")
        .config(function(formlyConfigProvider) {
            formlyConfigProvider.setType({
                name: "string-wysiwyg-tinymce",
                templateUrl: "fields/string/string-wysiwyg-tinymce.html",
                wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
                controller: "FieldWysiwygTinyMce as vm"
            });
        })

        .controller("FieldWysiwygTinyMce", FieldWysiwygTinyMceController);

    var defLanguage = "en_US", // needed to check if the language changed
        labelsEn =
        {
            linkAdamFile: "Link ADAM-file (recommended)",
            linkAdamFileHover: "Link using ADAM - just drop files using the Automatic Digital Assets Manager",
            linkDnnFile: "Link DNN-file",
            linkDnnFileHover: "Link a DNN-file (all files, slow)",
            linkPage: "Link to another page",
            linkPageHover: "Link a page from the current site",
            linkAnchorHover: "Anchor to link to using .../page#anchorname",
            modePro: "Switch to advanced mode",
            modeStd: "Switch to standard mode",
            H1: "H1",
            H2: "H2",
            Remove: "Remove"
        },
        // todo: move this to i18n later on
        labelsDe = {
            linkAdamFile: "ADAM-Datei (empfohlen)",
            linkAdamFileHover: "ADAM-Dateien verlinken - Dateien einfach hierhin ziehen - verwendet den Automatic Digital Assets Manager",
            linkDnnFile: "DNN-Datei verlinken",
            linkDnnFileHover: "DNN-Datei verlinken (alle Dateien, langsam)",
            linkPage: "Seite verlinken",
            linkPageHover: "Eine Seite aus dieser Website verlinken",
            linkAnchorHover: "Texmarke (Anchor) für Verlinkung mit .../page#anchorname",
            modePro: "Zum Profi-Modus wechseln",
            modeStd: "Zum Standard-Modus wechseln",
            "H1": "Ü1",
            "H2": "Ü2",
            Remove: "Entfernen"
        };

    function FieldWysiwygTinyMceController($scope, dnnBridgeSvc, languages) {
        var vm = this;

        vm.activate = function () {
            var availableLanguages = "de,es,fr,it,uk".split(",");

            var plugins = [
                "code",         // allow view / edit source
                "contextmenu",  // right-click menu for things like insert, etc.
                "autolink",     // automatically convert www.xxx links to real links
                "tabfocus",     // get in an out of the editor with tab
                "image",        // image button and image-settings
                "link",         // link button + ctrl+k to add link
                // "autosave",     // temp-backups the content in case the browser crashes, allows restore
                "paste",        // enables paste as text from word etc. https://www.tinymce.com/docs/plugins/paste/
                "anchor",       // allows users to set an anchor inside the text
                "charmap",      // character map https://www.tinymce.com/docs/plugins/visualchars/
                "hr",           // hr
                "media",        // video embed
                "nonbreaking",  // add button to insert &nbsp; https://www.tinymce.com/docs/plugins/nonbreaking/
                "searchreplace",// search/replace https://www.tinymce.com/docs/plugins/searchreplace/
                "table",        // https://www.tinymce.com/docs/plugins/searchreplace/

            ];

            var modes = {
                standard: {
                    menubar: false,
                    toolbar: " undo redo removeformat "
                    + "| bold formatgroup "
                    + "| h1 h2 hgroup " 
                    + "| numlist "// not needed since now context senitive: " outdent indent "
                    + "| adamlink linkgroup "
                    + "| modeadvanced ",
                    contextmenu: "charmap hr",
                },
                advanced: {
                    menubar: true,
                    toolbar: " undo redo removeformat "
                    + "| styleselect "
                    + "| bold italic "
                    + "| h1 h2 hgroup "
                    + "| bullist numlist outdent indent "
                    + "| images linkgrouppro "
                    + "| code modestandard ",
                    contextmenu: "link image | charmap hr adamimage",
                }
            };

            $scope.tinymceOptions = {
                baseURL: "//cdn.tinymce.com/4",
                inline: true,               // use the div, not an iframe
                automatic_uploads: false,   // we're using our own upload mechanism
                modes: modes,               // for later switch to another mode
                menubar: modes.standard.menubar,    // basic menu (none)
                toolbar: modes.standard.toolbar,    // basic toolbar
                plugins: plugins.join(" "),
                contextmenu: modes.standard.contextmenu, //"link image | charmap hr adamimage",
                autosave_ask_before_unload: false,
                paste_as_text: true,
                
                // Url Rewriting in images and pages
                //convert_urls: false,  // don't use this, would keep the domain which is often a test-domain
                relative_urls: false, // keep urls with full path so starting with a "/" - otherwise it would rewrite them to a "../../.." syntax
                default_link_target: "_blank",  // auto-use blank as default link-target
                object_resizing: false, // don't allow manual scaling of images

                // General looks
                skin: "lightgray",
                theme: "modern",
                // statusbar: true,    // doesn't work in inline :(

                language: "en",

                setup: function(editor) {
                    vm.editor = editor;
                    if ($scope.tinymceOptions.language)
                        initLangResources(editor, $scope.tinymceOptions.language);
                    addTinyMceToolbarButtons(editor, vm);
                }
            };

            // check if it's an additionally translated language
            var lang2 = languages.currentLanguage.substr(0, 2);

            // test a specific language quickly
             // lang2 = "de";

            if (availableLanguages.indexOf(lang2) >= 0)
                angular.extend($scope.tinymceOptions, {
                    language: lang2,
                    language_url: "../i18n/lib/tinymce/" + lang2 + ".js"
                });

        };

        //#region new adam: callbacks only
        vm.registerAdam = function (adam) {
            vm.adam = adam;
        };


        vm.setValue = function (fileItem, modeImage) {
            if (modeImage === undefined)        // if not supplied, use the setting in the adam
                modeImage = vm.adamModeImage; 
            vm.editor.insertContent(modeImage
                ? "<img src=\"" + fileItem.fullPath + "\">"
                : "<a href=\"" + fileItem.fullPath + "\">" + fileItem.Name.substr(0, fileItem.Name.lastIndexOf(".")) + "</a>");
        };

        // this is the event called by dropzone as something is dropped
        $scope.afterUpload = function(fileItem) {   
            vm.setValue(fileItem, fileItem.Type === "image");
        };

        vm.toggleAdam = function toggle(imagesOnly) {
            vm.adamModeImage = imagesOnly;
            vm.adam.toggle({showImagesOnly: imagesOnly});
            $scope.$apply();
        };

        //#endregion

        //#region DNN stuff

        // open the dialog
        vm.openDnnDialog = function (type) {
            dnnBridgeSvc.open(type, "", { Paths: null, FileFilter: null }, vm.processResultOfDnnBridge);
        };

        // the callback when something was selected
        vm.processResultOfDnnBridge = function (value, type) {
            $scope.$apply(function () {
                if (!value) return;

                // Convert file path to file ID if type file is specified
                var promise = dnnBridgeSvc.getUrlOfId(type + ":" + (value.id || value.FileId)); // id on page, FileId on file
                if (promise)
                    promise.then(function (result) {
                        var previouslySelected = vm.editor.selection.getContent();

                        if (type === "file") {
                            var fileName = result.data.substr(result.data.lastIndexOf("/"));
                            fileName = fileName.substr(0, fileName.lastIndexOf("."));
                            vm.editor.insertContent("<a href=\"" + result.data + "\">" + (previouslySelected || fileName) + "</a>");
                        } else if (type === "image") {
                            vm.editor.insertContent("<img src=\"" + result.data + "\">");
                        } else { // page
                            vm.editor.insertContent("<a href=\"" + result.data + "\">" + (previouslySelected || value.name) + "</a>");
                        }
                    });

            });
        };

        //#endregion

        vm.activate();
    }

    // todo - later use i18n from angular-translate
    function initLangResources(editor, language) {
        tinymce.addI18n("de", labelsDe);
        tinymce.addI18n("en", labelsEn);
    }

    function addTinyMceToolbarButtons(editor, vm) {
        //#region helpers like initOnPostRender(name)

        // helper function to add activate/deactivate to buttons like alignleft, alignright etc.
        function initOnPostRender(name) { // copied from https://github.com/tinymce/tinymce/blob/ddfa0366fc700334f67b2c57f8c6e290abf0b222/js/tinymce/classes/ui/FormatControls.js#L232-L249
            return function () {
                var self = this;

                if (editor.formatter) {
                    editor.formatter.formatChanged(name, function (state) {
                        self.active(state);
                    });
                } else {
                    editor.on("init", function () {
                        editor.formatter.formatChanged(name, function (state) {
                            self.active(state);
                        });
                    });
                }
            };
        }

        //#endregion

        //#region register formats

        // the method that will register everything
        function registerTinyMceFormats(editor, vm) {
            editor.formatter.register({
                imgwidth100: [{ selector: "img", collapsed: false, styles: { 'width': "100%" } }],
                imgwidth50: [{ selector: "img", collapsed: false, styles: { 'width': "50%" } }],
                imgwidth33: [{ selector: "img", collapsed: false, styles: { 'width': "33%" } }],
                imgwidth25: [{ selector: "img", collapsed: false, styles: { 'width': "25%" } }]
            });
        }

        // call register once the editor-object is ready
        editor.on('init', function() {
            registerTinyMceFormats(editor, vm);
        });

        //#endregion

        // i18n ok
        // group with adam-link, dnn-link
        editor.addButton("adamlink", {
            type: "splitbutton",
            icon: " icon-file-pdf",
            title: "linkAdamFileHover",
            onclick: function() {
                vm.toggleAdam(false);
            },
            menu: [
                {
                    text: "linkAdamFile",
                    tooltip: "linkAdamFileHover",
                    icon: " icon-file-pdf",
                    onclick: function() {
                        vm.toggleAdam(false);
                    }
                }, {
                    text: "linkDnnFile",
                    tooltip: "linkDnnFileHover",
                    icon: " icon-file",
                    onclick: function () {
                        vm.openDnnDialog("documentmanager");
                    }
                }
            ]
        });

        // i18n ok
        //#region link group with web-link, page-link, unlink, anchor
        var linkgroup = {
            type: "splitbutton",
            icon: "link",
            title: "Link",
            onPostRender: initOnPostRender("link"),
            onclick: function() {
                editor.execCommand("mceLink");
            },
            
            menu: [
            { icon: "link", text: "Link", onclick: function() { editor.execCommand("mceLink"); } },
            {
                text: "linkPage",
                tooltip: "linkPageHover",
                icon: " icon-sitemap",
                onclick: function() {
                    vm.openDnnDialog("pagepicker");
                }
            }
        ]
        };
        var linkgroupPro = angular.copy(linkgroup);
        linkgroupPro.menu.push({ icon: " icon-anchor", text: "Anchor", tooltip: "linkAnchorHover", onclick: function() { editor.execCommand("mceAnchor"); } });
        editor.addButton("linkgroup", linkgroup);
        editor.addButton("linkgrouppro", linkgroupPro);
        //#endregion

        // i18n ok
        // group with images (adam) - only in PRO mode
        editor.addButton("images", {
            type: "splitbutton",
            text: "",
            icon: "image",
            onclick: function() {
                vm.toggleAdam(true);
            },
            menu: [
                {
                    text: "linkAdamFile", 
                    tooltip: "linkAdamFileHover",
                    icon: "image",
                    onclick: function() {
                        vm.toggleAdam(true);
                    }
                }, {
                    text: "linkDnnFile", 
                    tooltip: "linkDnnFileHover",
                    icon: "image",
                    onclick: function() {
                        vm.openDnnDialog("imagemanager");
                    }
                }, {
                    text: "Insert\/edit image", // i18n tinyMce standard
                    icon: "image",
                    onclick: function () { editor.execCommand("mceImage"); }

                },
                // note: all these use i18n from tinyMce standard
                { icon: "alignleft", tooltip:"Align left", onclick: function() { editor.execCommand("JustifyLeft"); } },
                { icon: "aligncenter", tooltip: "Align center", onclick: function() { editor.execCommand("JustifyCenter"); } },
                { icon: "alignright", tooltip: "Align right", onclick: function() { editor.execCommand("JustifyRight"); } }
            ]
        });

        // i18n ok
        editor.addButton("formatgroup", {
            type: "splitbutton",
            tooltip: "Italic",  // will be autotranslated
            text: "",
            icon: "italic",
            cmd: "italic",
            onPostRender: initOnPostRender("italic"),
            menu: [
                { icon: "strikethrough", text: "Strikethrough", onclick: function () { editor.execCommand("strikethrough"); } },
                {   icon: "superscript", text: "Superscript", onclick: function() { editor.execCommand("superscript"); }  },
                {   icon: "subscript", text: "Subscript", onclick: function() { editor.execCommand("subscript"); }  }
            ]

        });

        // i18n ok
        //#region mode switching and the buttons for it
        function switchModes(mode) {
            editor.settings.toolbar = editor.settings.modes[mode].toolbar;
            editor.settings.menubar = editor.settings.modes[mode].menubar;
            // editor.settings.contextmenu = editor.settings.modes[mode].contextmenu; - doesn't work at the moment
            
            editor.theme.panel.remove();    // kill current toolbar
            editor.theme.renderUI(editor);
            editor.execCommand("mceFocus");

            // focus away...
            document.getElementById("dummyfocus").focus();

            // ...and focus back a bit later
            setTimeout(function() {
                editor.focus();
            }, 100);
        }

        editor.addButton("modestandard", {
            icon: " icon-cancel",
            tooltip: "modeStd",
            onclick: function () { switchModes("standard"); }
        });

        editor.addButton("modeadvanced", {
            icon: " icon-pro",
            tooltip: "modeStd",
            onclick: function () {  switchModes("advanced");    }
        });
        //#endregion

        // i18n ok
        //#region h1, h2, etc. buttons, inspired by http://blog.ionelmc.ro/2013/10/17/tinymce-formatting-toolbar-buttons/
        // note that the complex array is needede because auto-translate only happens if the string is identical
        [["pre", "Preformatted", "Preformatted"],
            ["p", "Paragraph", "Paragraph"],
            ["code", "Code", "Code"],
            ["h1", "Heading 1", "H1"],
            ["h2", "Heading 2", "H2"],
            ["h3", "Heading 3", "H3"],
            ["h4", "Heading 4", "Heading 4"],
            ["h5", "Heading 5", "Heading 5"],
            ["h6", "Heading 6", "Heading 6"]].forEach(function (tag) {
            editor.addButton(tag[0], {
                tooltip: tag[1],
                text: tag[2],
                onclick: function() { editor.execCommand("mceToggleFormat", false, tag[0]); },
                onPostRender: function() {
                    var self = this,
                        setup = function() {
                            editor.formatter.formatChanged(tag[0], function(state) {
                                self.active(state);
                            });
                        };
                    var x = editor.formatter ? setup() : editor.on("init", setup);
                }
            });
        });

        // group of buttons with an h3 to start and showing h4-6 + p
        editor.addButton("hgroup", angular.extend({}, editor.buttons.h3,
        {
            type: "splitbutton",
            menu: [
                editor.buttons.h4,
                editor.buttons.h5,
                editor.buttons.h6,
                editor.buttons.p
            ]
        }));
        //#endregion

        // i18n ok
        //#region image alignment / size buttons
        editor.addButton("alignimgleft", { icon: " icon-align-left", tooltip: "Align left", cmd: "JustifyLeft", onPostRender: initOnPostRender("alignleft") });
        editor.addButton("alignimgcenter", { icon: " icon-align-center", tooltip: "Align center", cmd: "justifycenter", onPostRender: initOnPostRender("aligncenter") });
        editor.addButton("alignimgright", { icon: " icon-align-right", tooltip: "Align right", cmd: "justifyright", onPostRender: initOnPostRender("alignright") });
        editor.addButton("alignimg100", {icon: " icon-resize-horizontal", tooltip: "100%",
            onclick: function() {   editor.formatter.apply("imgwidth100");  },
            onPostRender: initOnPostRender("imgwidth100")
        });
        editor.addButton("alignimg50", {icon: " icon-resize-horizontal", tooltip: "50%",
            onclick: function() {   editor.formatter.apply("imgwidth50");  },
            onPostRender: initOnPostRender("imgwidth50")
        });
        //#endregion

        // i18n ok
        //#region my context toolbars for links, images and lists (ul/li)
        function makeTagDetector(tagWeNeedInTheTagPath) {
            return function tagDetector(currentElement) {
                // check if we are in a tag within a specific tag
                var selectorMatched = editor.dom.is(currentElement, tagWeNeedInTheTagPath) && editor.getBody().contains(currentElement);
                return selectorMatched;
            };
        }
        editor.addContextToolbar(makeTagDetector("a"), "link unlink");
        editor.addContextToolbar(makeTagDetector("img"), "image | alignimgleft alignimgcenter alignimgright alignimg100 alignimg50 | removeformat | remove");
        editor.addContextToolbar(makeTagDetector("li"), "bullist numlist | outdent indent");
        //#endregion
    }

})();


