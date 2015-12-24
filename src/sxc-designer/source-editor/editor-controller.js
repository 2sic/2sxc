(function () { 

    angular.module("SourceEditor")

        .controller("Editor", EditorController)
        ;

    function EditorController(sourceSvc, snippetSvc, item, $modalInstance, $scope, $translate) {
        $translate.refresh();   // necessary to load stuff added in this lazy-loaded app

        var vm = this;
        var svc = sourceSvc(item.EntityId);
        vm.view = null;
        vm.editor = null;

        activate();

        function activate() {
            // get started...
            svc.get().then(function(result) {
                vm.view = result.data;
                vm.registerSnippets();
                //svc.initSnippets(vm.view);
            });            
        }



        // load appropriate snippets from the snippet service
        svc.initSnippets = function() {
            vm.snipSvc.getSnippets().then(function(result) {
                vm.snippets = result;
                vm.snippetSet = "Content";    // select default
                vm.snippetHelp = vm.snipSvc.help;
                vm.snippetLabel = vm.snipSvc.label;

                // now register the snippets in the editor
                vm.registerSnippets();
            });
        };

        vm.close = function () { $modalInstance.dismiss("cancel"); };

        vm.save = function() {
            svc.save(vm.view).then(vm.close);
        };

        vm.addSnippet = function addSnippet(snippet) {
            var snippetManager = ace.require("ace/snippets").snippetManager;
            snippetManager.insertSnippet(vm.editor, snippet);
            vm.editor.focus();
        };

        vm.registerSnippets = function registerSnippets() {
            // ensure we have everything
            if (!(vm.view && vm.editor))
                return;

            vm.snipSvc = snippetSvc(vm.view, ace);
            vm.snipSvc.registerSnippets("razor");

            //svc.initSnippets();
        };

        // this event is called when the editor is ready
        $scope.aceLoaded = function (_editor) {
            vm.editor = _editor;        // remember the editor for later actions
            vm.registerSnippets();      // try to register the snippets
        };

    }

} ());