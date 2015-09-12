(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("TemplatesApp")
        .constant("createdBy", "2sic")          // just a demo how to use constant or value configs in AngularJS
        .constant("license", "MIT")             // these wouldn't be necessary, just added for learning exprience
        .controller("TemplateEdit", TemplateEditController)
        ;

    function TemplateEditController(svc, eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;

        vm.items = svc.liveList();


        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());