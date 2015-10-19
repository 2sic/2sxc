(function () { 

    angular.module("ReorderContentApp", [
            "SxcServices",
            "EavAdminUi"         // dialog (modal) controller
        ])
        .controller("ReorderContentList", ReorderContentController);

    function ReorderContentController(appId, item, contentGroupSvc, eavAdminDialogs, $modalInstance, $filter) {
        var vm = this;
        vm.items = [];
        vm.item = {
            id: item.EntityId,
            guid: item.Group.Guid,
            part: item.Group.Part,
            index: item.Group.Index
        };

        var svc = contentGroupSvc(appId);

        vm.reload = function() {
            return svc.getList(vm.item).then(function(result) {
                vm.items = result.data;
            });
        };
        vm.reload();

        vm.ok = function ok() {
            svc.saveList(vm.items).then(vm.close);
        };
        
        vm.close = function () { $modalInstance.dismiss("cancel"); };

        //vm.convertToInt = function (id) {
        //    return parseInt(id);
        //};


        //vm.reloadAfterCopy = function reloadAfterCopy(result) {
        //    var copy = result.data;
        //    vm.reload().then(function() {
        //        vm.item.id = copy[Object.keys(copy)[0]]; 
        //    });
        //};
    }

} ());