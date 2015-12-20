(function () { 

    angular.module("SourceEditor")

        .controller("Editor", EditorController)
        ;

    function EditorController(sourceSvc, snippetSvc, item, $modalInstance, $scope, $sce) {
        var vm = this;
        var svc = sourceSvc(item.EntityId);
        vm.view = {};
        vm.editor = null;

        svc.get().then(function(result) {
            vm.view = result.data;
            svc.initSnippets(vm.view);
        });

        svc.initSnippets = function(template) {
            vm.snipSvc = snippetSvc(template);
            vm.snippets = vm.snipSvc.getSnippets();
            vm.snippetSet = "Content";    // select default
        };

        vm.snippetLabel = function(set, subset, key) {
            return vm.snipSvc.label(set, subset, key);
        };
        vm.snippetHelp = function(set, subset, key) {
            return vm.snipSvc.help(set, subset, key);
        };



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