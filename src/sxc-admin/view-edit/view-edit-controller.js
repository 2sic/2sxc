(function () { 

    angular.module("ViewEdit")

        .controller("ViewEdit", ViewEditController)
        ;

    function ViewEditController(viewSvc, item, $modalInstance, $scope) {
        var vm = this;
        var svc = viewSvc(item.EntityId);
        vm.view = {};
        vm.editor = null;

        svc.get().then(function(result) {
            vm.view = result.data;
        });

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