// todo
// finish toolbar based on https://www.tinymce.com/docs/advanced/editor-control-identifiers/#toolbarcontrols
// - more buttons
// - more unimportant buttons, hidden in a sub-menu
//
// finish right-click actions
// 
// review further plugins/functionalty
(function () {
	"use strict";

    angular.module("sxcFieldTemplates")
        .config(function(formlyConfigProvider) {

            // for now identical with -adv, but later will change
            formlyConfigProvider.setType({
                name: "string-wysiwyg",
                templateUrl: "fields/string/string-wysiwyg-tinymce.html",
                wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"],
                controller: "FieldWysiwygTinyMce as vm"
            });


        })
        .controller("FieldWysiwygTinyMce", FieldWysiwygTinyMceController);

    function FieldWysiwygTinyMceController($scope, dnnBridgeSvc) {
            var vm = this;

        vm.activate = function() {
            $scope.tinymceOptions = {
                //onChange: function (e) {
                //    // put logic here for keypress and cut/paste changes
                //},
                inline: true,               // use the div, not an iframe
                automatic_uploads: false,   // we're using our own upload mechanism
                menubar: true,              // don't add a second row of menus
                toolbar: "assets dnn | undo redo removeformat | styleselect | bold italic | bullist numlist outdent indent | alignleft aligncenter alignright | link image"
                    + "| code",
                plugins: "code contextmenu autolink tabfocus",
                contextmenu: "link image",
                // plugins: 'advlist autolink link image lists charmap print preview',

                // Url Rewriting in images and pages
                //convert_urls: false,  // don't use this, would keep the domain which is often a test-domain
                relative_urls: false,   // keep urls with full path so starting with a "/" - otherwise it would rewrite them to a "../../.." syntax
                object_resizing: false, // don't allow manual scaling of images

                skin: "lightgray",
                theme: "modern",
                statusbar: true,
                setup: function (editor) {
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
                // $scope.value.Value = value;
                var promise = dnnBridgeSvc.getUrlOfId(type + ":" + (value.id || value.FileId)); // id on page, FileId on file
                if (promise)
                    promise.then(function(result) {
                        if (type === "file") {
                            alert("todo: add link to file or img tag");
                        } else { // page
                            vm.editor.insertContent("<a href=\"" + result.data + "\">" + value.name + "</a>");
                        }
                    });

            });
        };

        //#endregion

        vm.activate();        
    }

    function addTinyMceToolbarButtons(editor, vm) {
        editor.addButton("assets", {
            type: "splitbutton",
            text: "Assets",
            icon: "code custom glyphicon glyphicon-apple",
            onclick: function () {
                vm.toggleAdam();
            },
            menu: [
            {
                text: "Adam",
                icon: "code custom glyphicon glyphicon-apple",
                onclick: function() {
                    vm.toggleAdam();
                }
            },{
                    text: "Page",
                    icon: "code custom glyphicon glyphicon-apple",
                    onclick: function () {
                        vm.openDnnDialog("pagepicker");
                    }
                }, {
                    text: "Image",
                    icon: "code custom glyphicon glyphicon-apple",
                    onclick: function () {
                        vm.openDnnDialog("documentmanager");
                    }
                }, {
                    text: "File",
                    icon: "code custom glyphicon glyphicon-apple",
                    onclick: function () {
                        vm.openDnnDialog("imagemanager");
                    }
                }
            ]
        });


        editor.addButton("adam", {
            text: "Adam",
            icon: "code custom glyphicon glyphicon-apple",
            onclick: function () {
                vm.toggleAdam();
            }
        });
        editor.addButton("dnn", {
            type: "menubutton",
            text: "DNN",
            icon: "code custom glyphicon glyphicon-apple",
            menu: [
                {
                    text: "Page",
                    icon: "code custom glyphicon glyphicon-apple",
                    onclick: function() {
                        vm.openDnnDialog("pagepicker");
                    }
                }, {
                    text: "Image",
                    icon: "code custom glyphicon glyphicon-apple",
                    onclick: function() {
                        vm.openDnnDialog("documentmanager");
                    }
                }, {
                    text: "File",
                    icon: "code custom glyphicon glyphicon-apple",
                    onclick: function() {
                        vm.openDnnDialog("imagemanager");
                    }
                }
            ]
        });

    }
})();