(function () { 

    angular.module("SourceEditor")

        .controller("Help", HelpController)
        ;

    function HelpController(viewHelpSvc) { // }, eavConfig, appId) {
        var vm = this;
        var svc = viewHelpSvc;
        vm.items = svc.liveList();

        // vm.refresh = 
        vm.ready = function ready() {
            return vm.items.length > 0;
        };

    }

} ());