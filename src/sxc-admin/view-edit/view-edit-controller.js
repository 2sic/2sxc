(function () { 

    angular.module("ViewEdit")

        .controller("ViewEdit", ViewEditController)
        ;

    function ViewEditController(viewEditSvc, $modalInstance) { // }, eavConfig, appId) {
        var vm = this;
        var svc = viewEditSvc();
        vm.view = { source: 1 };//svc.getView();

        vm.close = function () { $modalInstance.dismiss("cancel"); };

        vm.save = function() {
            alert('todo');
            svc.saveView(vm.view);
        };
    }

} ());