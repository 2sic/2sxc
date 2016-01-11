
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

    function FieldWysiwygTinyMceController($scope, dnnBridgeSvc) {
        var vm = this;

        vm.activate = function () {

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

                setup: function(editor) {
                    vm.editor = editor;
                    addTinyMceToolbarButtons(editor, vm);
                }
            };
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

        // group with adam-link, dnn-link
        editor.addButton("adamlink", {
            type: "splitbutton",
            icon: " icon-file-pdf",
            title: "Link using ADAM - just drop files",
            onclick: function() {
                vm.toggleAdam(false);
            },
            menu: [
                {
                    text: "link ADAM-file (recommended)",
                    tooltip: "link a file from the Automatic Digital Asset Manager",
                    icon: " icon-file-pdf",
                    onclick: function() {
                        vm.toggleAdam(false);
                    }
                }, {
                    text: "link DNN-file (slow)",
                    icon: " icon-file",
                    onclick: function() {
                        vm.openDnnDialog("documentmanager");
                    }
                }
            ]
        });

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
                text: "link to another page",
                icon: " icon-sitemap",
                onclick: function() {
                    vm.openDnnDialog("pagepicker");
                }
            },
            //{ icon: "unlink", text: "remove link", onclick: function() { editor.execCommand("unlink"); } },
        ]
        };
        var linkgroupPro = angular.copy(linkgroup);
        linkgroupPro.menu.push({ icon: " icon-anchor", text: "insert anchor (#-link target)", onclick: function() { editor.execCommand("mceAnchor"); } });
        editor.addButton("linkgroup", linkgroup);
        editor.addButton("linkgrouppro", linkgroupPro);
        //#endregion

        // group with images (adam)
        editor.addButton("images", {
            type: "splitbutton",
            text: "",
            icon: "image",
            onclick: function() {
                vm.toggleAdam(true);
            },
            menu: [
                {
                    text: "from ADAM (recommended)",
                    icon: "image",
                    onclick: function() {
                        vm.toggleAdam(true);
                    }
                }, {
                    text: "from DNN (all files in DNN, slower)",
                    icon: "image",
                    onclick: function() {
                        vm.openDnnDialog("imagemanager");
                    }
                }, {
                    text: "Insert with url / edit image",
                    icon: "image",
                    onclick: function () { editor.execCommand("mceImage"); }

                },
                { icon: "alignleft", onclick: function() { editor.execCommand("JustifyLeft"); } },
                { icon: "aligncenter", onclick: function() { editor.execCommand("JustifyCenter"); } },
                { icon: "alignright", onclick: function() { editor.execCommand("JustifyRight"); } }
            ]
        });

        editor.addButton("formatgroup", {
            type: "splitbutton",
            text: "",
            icon: "italic",
            cmd: "italic",
            onPostRender: initOnPostRender("italic"),
            menu: [
                { icon: "strikethrough", text: "strikethrough", onclick: function () { editor.execCommand("strikethrough"); } },
                {   icon: "superscript", text: "superscript", onclick: function() { editor.execCommand("superscript"); }  },
                {   icon: "subscript", text: "subscript", onclick: function() { editor.execCommand("subscript"); }  }
            ]

        });

        //#region mode switching and the buttons for it
        function switchModes(mode) {
            editor.settings.toolbar = editor.settings.modes[mode].toolbar;
            editor.settings.menubar = editor.settings.modes[mode].menubar;
            // editor.settings.contextmenu = editor.settings.modes[mode].contextmenu; - doesn't work at the moment
            
            editor.theme.panel.remove();    // kill current toolbar
            editor.theme.renderUI(editor);
            editor.execCommand("mceFocus");

            document.getElementById("dummyfocus").focus();

            setTimeout(function() {
                editor.focus();
            }, 100);
        }

        editor.addButton("modestandard", {
            icon: " icon-cancel",
            tooltip: "basic mode",
            onclick: function () { switchModes("standard"); }
        });

        editor.addButton("modeadvanced", {
            icon: " icon-pro",
            tooltip: "pro mode",
            onclick: function () {  switchModes("advanced");    }
        });
        //#endregion


        //#region h1, h2, etc. buttons, inspired by http://blog.ionelmc.ro/2013/10/17/tinymce-formatting-toolbar-buttons/
        ["pre", "p", "code", "h1", "h2", "h3", "h4", "h5", "h6"].forEach(function (name) {
            editor.addButton(name, {
                tooltip: "Toggle " + name,
                text: name.toUpperCase(),
                onclick: function() { editor.execCommand("mceToggleFormat", false, name); },
                onPostRender: function() {
                    var self = this,
                        setup = function() {
                            editor.formatter.formatChanged(name, function(state) {
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

        //#region image alignment / size buttons
        editor.addButton("alignimgleft", {icon: " icon-align-left", cmd: "JustifyLeft", onPostRender: initOnPostRender("alignleft")});
        editor.addButton("alignimgcenter", { icon: " icon-align-center", cmd: "justifycenter", onPostRender: initOnPostRender("aligncenter") });
        editor.addButton("alignimgright", { icon: " icon-align-right", cmd: "justifyright", onPostRender: initOnPostRender("alignright") });
        editor.addButton("alignimg100", {icon: " icon-resize-horizontal",
            onclick: function() {   editor.formatter.apply("imgwidth100");  },
            onPostRender: initOnPostRender("imgwidth100")
        });
        editor.addButton("alignimg50", {icon: " icon-resize-horizontal",
            onclick: function() {   editor.formatter.apply("imgwidth50");  },
            onPostRender: initOnPostRender("imgwidth50")
        });
        //#endregion

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


