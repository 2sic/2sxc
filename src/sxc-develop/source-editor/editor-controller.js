(function () {

    angular.module("SourceEditor")

        .controller("Editor", EditorController)
    ;

    function EditorController(sourceSvc, snippetSvc, item, $modalInstance, $window, $scope, $translate, saveToastr, ctrlS) {
        $translate.refresh();   // necessary to load stuff added in this lazy-loaded app

        var vm = this;
        var svc = sourceSvc(item.EntityId);
        vm.view = {};
        vm.editor = null;

        svc.get().then(function (result) {
            vm.view = result.data;
            svc.initSnippets(vm.view);
        });

        // load appropriate snippets from the snippet service
        svc.initSnippets = function (template) {
            vm.snipSvc = snippetSvc(template, ace);
            vm.snipSvc.getSnippets().then(function (result) {
                vm.snippets = result;
                vm.snippetSet = "Content";    // select default
                vm.snippetHelp = vm.snipSvc.help;
                vm.snippetLabel = vm.snipSvc.label;

                // now register the snippets in the editor
                vm.registerSnippets();
            });
        };

        //#region close / prevent-close
        //vm.close = function () { $modalInstance.dismiss("cancel"); };

        vm.maybeLeave = function maybeLeave(e) {
            if (!confirm($translate.instant("Message.ExitOk")))
                e.preventDefault();
        };

        $scope.$on('modal.closing', vm.maybeLeave);

        $window.addEventListener('beforeunload', function (e) {
            var unsavedChangesText = $translate.instant("Message.ExitOk");
            (e || window.event).returnValue = unsavedChangesText; //Gecko + IE
            return unsavedChangesText; //Gecko + Webkit, Safari, Chrome etc.
        });

        //#endregion

        //#region save
        vm.save = function (autoClose) {
            var after = autoClose ? vm.close : function () { };
            saveToastr(svc.save(vm.view)).then(after);
        };
        //#endregion

        activate();

        function activate() {
            // add ctrl+s to save
            ctrlS.bind(function() { vm.save(false); });
        }



        //#region snippets
        vm.addSnippet = function addSnippet(snippet) {
            var snippetManager = ace.require("ace/snippets").snippetManager;
            snippetManager.insertSnippet(vm.editor, snippet);
            vm.editor.focus();
        };

        vm.registerSnippets = function registerSnippets() {
            // ensure we have everything first (this may be called multiple times), then register them
            if (!(vm.snipSvc && vm.editor))
                return;
            vm.snipSvc.registerInEditor();
        };
        //#endregion

        // this event is called when the editor is ready
        $scope.aceLoaded = function (_editor) {
            vm.editor = _editor;        // remember the editor for later actions
            vm.registerSnippets();      // try to register the snippets
        };

    }

}());