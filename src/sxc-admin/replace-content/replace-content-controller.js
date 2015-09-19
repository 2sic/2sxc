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

    function ReplaceContentController(appId, entityId, groupGuid, groupPart, groupIndex, contentGroupSvc, $modalInstance) {
        var vm = this;
        vm.item = {
            id: entityId,
            guid: groupGuid,
            part: groupPart,
            index: groupIndex
        };

        var svc = contentGroupSvc(appId).replaceContentItemResource;

        vm.options = svc.get(vm.item);

        vm.ok = function ok() {
            svc.save(vm.item).$promise.then(function() {
                $modalInstance.dismiss("cancel");
            });
        };
        
        vm.close = function () { $modalInstance.dismiss("cancel"); };

    }

} ());