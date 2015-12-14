(function () { 

    angular.module("ViewEdit")

        .controller("HelpController", HelpControllerController)
        ;

    function HelpControllerController(languagesSvc, eavConfig, appId) {
        var vm = this;
        var svc = languagesSvc();
        vm.items = svc.liveList();

        // vm.refresh = 
        vm.ready = function ready() {
            return vm.items.length > 0;
        };

    }

} ());