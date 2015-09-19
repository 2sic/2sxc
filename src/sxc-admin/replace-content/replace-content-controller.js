(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ReplaceContentApp", [
        "EavConfiguration",     // 
        //"EavServices",
        "SxcServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
    ])

        .controller("ReplaceDialog", ReplaceContentController)
        ;

    function ReplaceContentController(appId, groupId, groupSet, groupIndex) {
        var vm = this;
        var svc = {};
        vm.items = svc.liveList();
        
        vm.toggle = svc.toggle;
    }

} ());