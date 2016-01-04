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

    function FieldWysiwygTinyMceController($scope) {
            var vm = this;

        vm.activate = function() {
            $scope.tinymceOptions = {
                //onChange: function (e) {
                //    // put logic here for keypress and cut/paste changes
                //},
                inline: true, // use the div, not an iframe
                automatic_uploads: false, // we're using our own upload mechanism
                menubar: true, // don't add a second row of menus
                toolbar: "mybutton | undo redo removeformat | styleselect | bold italic | bullist numlist outdent indent | alignleft aligncenter alignright | link image |"
                    + "code",
                plugins: "code contextmenu autolink tabfocus",
                contextmenu: "link image inserttable | cell row column deletetable",
                // plugins: 'advlist autolink link image lists charmap print preview',
                skin: "lightgray",
                theme: "modern",
                statusbar: true,
                setup: function (editor) {
                    vm.editor = editor;
                    addTinyMceToolbarButtons(editor, vm);
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
        vm.toggleAdam = function toggle() {
            vm.adam.toggle();
        };

        //#endregion

        vm.activate();        
    }

    function addTinyMceToolbarButtons(editor, vm) {
        editor.addButton("mybutton", {
            text: "My button",
            icon: "code custom glyphicon glyphicon-apple",
            onclick: function () {
                vm.toggleAdam();
            }
        });
    }
})();