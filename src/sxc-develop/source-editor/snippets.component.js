

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