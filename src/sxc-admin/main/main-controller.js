(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("MainSxcApp", [
        "EavConfiguration",     // config
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "Sxci18n",              // multi-language stuff
        "SxcFilters",           // for inline unsafe urls
        "ContentTypesApp",
        "TemplatesApp"
    ])
        .constant("createdBy", "2sic")          // just a demo how to use constant or value configs in AngularJS
        .constant("license", "MIT")             // these wouldn't be necessary, just added for learning exprience
        .controller("Main", MainController)
        ;

    function MainController(eavAdminDialogs, eavConfig, appId, $modalInstance) {
        var vm = this;
        vm.view = "start";
        alert(window.mainConfig.gettingStartedUrl);
        vm.gettingStartedUrl = (window.mainConfig) ? window.mainConfig.gettingStartedUrl : "http://gettingstarted.2sxc.org";

        // var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            alert('todo');
        }; 

        vm.add = function add() {
            alert('todo');
        };
        
        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());