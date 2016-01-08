
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
                "paste",        // enables paste as text
                "anchor",       // allows users to set an anchor inside the text
            ];

            var modes = {
                standard: {
                    menubar: false,
                    toolbar: " undo redo removeformat | styleselect | bold italic | h1 h2 hgroup | bullist numlist outdent indent "
                    + "| adamlink linkgroup "
                    + "| modeadvanced ",
                },
                advanced: {
                    menubar: true,
                    toolbar: " undo redo removeformat | styleselect | bold italic | h1 h2 hgroup | bullist numlist outdent indent "
                    + "| images linkgroup "
                    + "| code modestandard ",
                }
            };

            $scope.tinymceOptions = {
                baseURL: "//cdn.tinymce.com/4",
                //onChange: function (e) {
                //    // put logic here for keypress and cut/paste changes
                //},
                inline: true, // use the div, not an iframe
                automatic_uploads: false, // we're using our own upload mechanism
                modes: modes,            // for later switch to another mode
                menubar: modes.standard.menubar,    // basic menu (none)
                toolbar: modes.standard.toolbar,    // basic toolbar
                plugins: plugins.join(" "),
                contextmenu: "link image adamimage",
                autosave_ask_before_unload: false,
                paste_as_text: true,
                

                // Url Rewriting in images and pages
                //convert_urls: false,  // don't use this, would keep the domain which is often a test-domain
                relative_urls: false, // keep urls with full path so starting with a "/" - otherwise it would rewrite them to a "../../.." syntax
                object_resizing: false, // don't allow manual scaling of images

                skin: "lightgray",
                theme: "modern",
                statusbar: true,    // doesn't work in inline :(
                setup: function(editor) {
                    vm.editor = editor;
                    addTinyMceToolbarButtons(editor, vm);

                    // make sure the model isn't dirty
                    var x = $scope.value.Value;

                    editor.on("init", function() {
                        var y = $scope.value.Value;
                    });
                    editor.on("dirty", function() {
                        var y = $scope.value.Value;
                    });
                    editor.on("loadcontent", function() {
                        var y = $scope.value.Value;
                    });
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

        // group with web-link, page-link, unlink, anchor
        editor.addButton("linkgroup", {
            type: "splitbutton",
            icon: "link",
            title: "Link",
            onclick: function() {
                editor.execCommand("mceLink");
            },
            menu: [
                { icon: "link", text: "Link", onclick: function () { editor.execCommand("mceLink"); } },
                {
                    text: "link to another page",
                    icon: " icon-sitemap",
                    onclick: function() {
                        vm.openDnnDialog("pagepicker");
                    }
                },
                { icon: "unlink", text: "remove link", onclick: function () { editor.execCommand("unlink"); } },
                { icon: " icon-anchor", text: "insert anchor (#-link target)", onclick: function () { editor.execCommand("mceAnchor"); } },

            ]
        });

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
                { icon: "alignright", onclick: function() { editor.execCommand("JustifyRight"); } },
            ]
        });

        function switchModes(mode) {
            editor.settings.toolbar = editor.settings.modes[mode].toolbar;
            editor.settings.menubar = editor.settings.modes[mode].menubar;
            
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


        // h1, h2, etc. buttons, inspired by http://blog.ionelmc.ro/2013/10/17/tinymce-formatting-toolbar-buttons/
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
    }
})();