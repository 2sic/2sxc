
(function () {
	"use strict";

    angular.module("sxcFieldTemplates")
        .config(function(formlyConfigProvider) {
            formlyConfigProvider.setType({
                name: "string-wysiwyg-tinymce",
                templateUrl: "fields/string/string-wysiwyg-tinymce.html",
                wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
                controller: "FieldWysiwygTinyMce as vm"
            });
        })
        .directive('test', function($compile) {
            return {
                restrict: 'A',
                //transclude: true,
                //template: '<span>loading editor...</p>',
                link: function(scope, element, attrs, ctrl, transclude) {
                    console.log("content was:" + transclude);
                },

                controller: function ($scope, $element, $interval, $window) {
                    var checkIfTinyMceLoaded = $interval(function() {
                        if ($window.tinymce) {
                            $window.tinymce.baseURL = "//cdn.tinymce.com/4";
                            var orig = $element[0].innerHTML.replace(/lazy-/g, "");
                            //var orig2 = '<div ui-tinymce="tinymceOptions" ng-model="value.Value" class="field-string-wysiwyg-mce-box"></div>';
                            //var el2 = $compile(orig2)($scope);
                            var el = $compile(orig)($scope);
                            $interval.cancel(checkIfTinyMceLoaded);
                            $element.replaceWith(el);//.parent().append(el);
                        } else {
                            console.log("waiting to load TinyMCE...");
                        }
                    }, 500);

                }
            };
        })


        .controller("FieldWysiwygTinyMce", FieldWysiwygTinyMceController);

    function FieldWysiwygTinyMceController($scope, dnnBridgeSvc, $ocLazyLoad, $interval) {
        var vm = this;

        var interv = $interval(function () {
            console.log("found tiny: " + window.tinymce + "; element:" + $scope);
            if (window.tinymce)
                $interval.cancel(interv);
        }, 100);

        vm.activate = function () {

            var plugins = [
                "code",     // allow view / edit source
                "contextmenu",  // right-click menu for things like insert, etc.
                "autolink", // automatically convert www.xxx links to real links
                "tabfocus", // get in an out of the editor with tab
                "image",    // image button and image-settings
                "link",     // link button + ctrl+k to add link
                "autosave", // temp-backups the content in case the browser crashes, allows restore
                "paste",    // enables paste as text
                "anchor",   // allows users to set an anchor inside the text
            ];
            $scope.tinymceOptions = {
                baseURL: "//cdn.tinymce.com/4",
                //onChange: function (e) {
                //    // put logic here for keypress and cut/paste changes
                //},
                inline: true, // use the div, not an iframe
                automatic_uploads: false, // we're using our own upload mechanism
                menubar: true, // don't add a second row of menus
                toolbar: " undo redo removeformat | styleselect | bold italic | h1 h2 hgroup | bullist numlist outdent indent "
                    + "| images linkgroup "
                    + "| code",
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
        vm.setValue = function (fileItem) {
            vm.editor.insertContent("<img src=\"" + fileItem.fullPath + "\">");
        };
        $scope.afterUpload = vm.setValue;

        vm.toggleAdam = function toggle() {
            vm.adam.toggle();
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

        $ocLazyLoad.load({
            serie: true,
            files: [
                "//cdn.tinymce.com/4/tinymce.min.js",
                "../../bower_components/angular-ui-tinymce/src/tinymce.js"
            ]
        }).then(function () {
            //window.tinymce.init();

            console.log('lazy loaded');
            console.log("found after lazy: " + window.tinymce);
            //$scope.$apply();
            vm.activate();

        });
        //vm.activate();
    }

    function addTinyMceToolbarButtons(editor, vm) {

        editor.addButton("linkgroup",
        {
            type: "splitbutton",
            icon: "link",
            title: "Link",
            onclick: function() {
                editor.execCommand("mceLink");
            },
            menu: [
                { icon: "link", text: "web link", onclick: function() { editor.execCommand("mceLink"); } },
                { icon: "unlink", text: "remove link", onclick: function() { editor.execCommand("unlink"); } },
                { icon: "anchor", text: "set anchor", onclick: function() { editor.execCommand("mceAnchor"); } },
                {
                    text: "file in ADAM (recommended)",
                    icon: "newdocument",
                    onclick: function() {
                        vm.toggleAdam();
                    }
                }, {
                    text: "file in DNN",
                    icon: "newdocument",
                    onclick: function() {
                        vm.openDnnDialog("documentmanager");
                    }
                }, {
                    text: "page in DNN",
                    icon: "copy",
                    onclick: function() {
                        vm.openDnnDialog("pagepicker");
                    }
                }
            ]
        });

        editor.addButton("images", {
            type: "splitbutton",
            text: "",
            icon: "image",
            onclick: function() {
                vm.toggleAdam();
            },
            menu: [
                {
                    text: "from ADAM (recommended)",
                    icon: "image",
                    onclick: function() {
                        vm.toggleAdam();
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


        editor.addButton("adamimage", {
            text: "image from ADAM",
            icon: "image",
            onclick: function () {
                vm.toggleAdam();
            }
        });
        //editor.addButton("dnn", {
        //    type: "menubutton",
        //    text: "DNN",
        //    icon: "link",
        //    menu: [
        //        {
        //            text: "file from ADAM (automatic, recommended)",
        //            icon: "newdocument",
        //            onclick: function() {
        //                vm.toggleAdam();
        //            }
        //        }, {
        //            text: "file from DNN",
        //            icon: "newdocument custom glyphicon glyphicon-apple",
        //            onclick: function() {
        //                vm.openDnnDialog("documentmanager");
        //            }
        //        }, {
        //            text: "page in DNN",
        //            icon: "copy",
        //            onclick: function() {
        //                vm.openDnnDialog("pagepicker");
        //            }
        //        }
        //    ]
        //});

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