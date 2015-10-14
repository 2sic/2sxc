(function () { 

    angular.module("WebApiApp", [
        "SxcServices",
        //"EavConfiguration",
        "EavAdminUi",
        "EavServices",
        "EavDirectives"
    ])
        .controller("WebApiMain", WebApiMainController)
        ;

    function WebApiMainController(appId, webApiSvc, eavAdminDialogs, $modalInstance, $filter) {
        var vm = this;
        var translate = $filter('translate');
        
        var svc = webApiSvc(appId);

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.add = function add() {
            alert(translate("WebApi.AddDoesntExist"));
        };

        // not implemented yet...
        vm.tryToDelete = function tryToDelete(item) {
            if (confirm(translate("General.Messages.DeleteEntity", { title: item.Title, id: item.Id})))   //"Delete '" + item.Title + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());