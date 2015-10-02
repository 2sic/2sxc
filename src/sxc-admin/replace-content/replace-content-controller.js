(function () { 

    angular.module("ReplaceContentApp", [
            "SxcServices",
            "EavAdminUi"         // dialog (modal) controller
        ])
        .controller("ReplaceDialog", ReplaceContentController);

    function ReplaceContentController(appId, item, contentGroupSvc, $modalInstance) {
        var vm = this;
        vm.item = {
            id: item.EntityId,
            guid: item.Group.Guid,
            part: item.Group.Part,
            index: item.Group.Index
        };

        var svc = contentGroupSvc(appId);
        var res = svc.replace;

        vm.options = res.get(vm.item);

        vm.ok = function ok() {
            res.save(vm.item).$promise.then(vm.close);
        };
        
        vm.close = function () { $modalInstance.dismiss("cancel"); };

    }

} ());