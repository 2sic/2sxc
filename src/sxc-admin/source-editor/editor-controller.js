(function () { 

    angular.module("SourceEditor")

        .controller("Editor", EditorController)
        ;

    function EditorController(sourceSvc, helpSvc, item, $modalInstance, $scope) {
        var vm = this;
        var svc = sourceSvc(item.EntityId);
        var help = helpSvc(item.EntiyId);
        vm.view = {};
        vm.editor = null;

        svc.get().then(function(result) {
            vm.view = result.data;
        });

        vm.snippets = help.getSnippets();

        vm.close = function () { $modalInstance.dismiss("cancel"); };

        vm.save = function() {
            svc.save(vm.view).then(vm.close);
        };

        vm.addSnippet = function addSnippet(snippet) {
            var snippetManager = ace.require("ace/snippets").snippetManager;
            snippetManager.insertSnippet(vm.editor, snippet);
        };

        $scope.aceLoaded = function(_editor) {
            // Options
            vm.editor = _editor;
        };

    }

} ());