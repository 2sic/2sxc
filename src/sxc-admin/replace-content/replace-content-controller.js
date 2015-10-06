(function () { 

    angular.module("ReplaceContentApp", [
            "SxcServices",
            "EavAdminUi"         // dialog (modal) controller
        ])
        .controller("ReplaceDialog", ReplaceContentController);

    function ReplaceContentController(appId, item, contentGroupSvc, $modalInstance, $filter) {
        var vm = this;
        vm.options = [];
        vm.item = {
            id: item.EntityId,
            guid: item.Group.Guid,
            part: item.Group.Part,
            index: item.Group.Index
        };

        var svc = contentGroupSvc(appId);

        svc.getItems(vm.item).then(function (result) {
            vm.options = result.data.Items;
            vm.item.id = result.data.SelectedId;
        });

        vm.ok = function ok() {
            svc.saveItem(vm.item).then(vm.close);
        };
        
        vm.close = function () { $modalInstance.dismiss("cancel"); };

    }

} ());