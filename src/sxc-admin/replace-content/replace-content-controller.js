(function () { 

    angular.module("ReplaceContentApp", [
            "SxcServices",
            "EavAdminUi"         // dialog (modal) controller
        ])
        .controller("ReplaceDialog", ReplaceContentController);

    function ReplaceContentController(appId, entityId, groupGuid, groupPart, groupIndex, contentGroupSvc, $modalInstance) {
        var vm = this;
        vm.item = {
            id: entityId,
            guid: groupGuid,
            part: groupPart,
            index: groupIndex
        };

        var svc = contentGroupSvc(appId);
        var res = svc.replace;

        vm.options = res.get(vm.item);

        vm.ok = function ok() {
            res.save(vm.item).$promise.then(function() {
                $modalInstance.dismiss("cancel");
            });
        };
        
        vm.close = function () { $modalInstance.dismiss("cancel"); };

    }

} ());