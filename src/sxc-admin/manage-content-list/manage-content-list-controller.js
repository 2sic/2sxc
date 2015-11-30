(function () {

    angular.module("ReorderContentApp", [
            "SxcServices",
            "EavAdminUi" // dialog (modal) controller
    ])

        .config(function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("inpage");
        })

        .controller("ManageContentList", ManageContentController);

    function ManageContentController(appId, item, contentGroupSvc, eavAdminDialogs, $modalInstance, $translate) {
        var vm = this;
        vm.items = [];
        vm.header = {};
        vm.contentGroup = {
            id: item.EntityId,
            guid: item.Group.Guid,
            part: item.Group.Part,
            index: item.Group.Index
        };

        var svc = contentGroupSvc(appId);

        vm.reload = function () {
            return svc.getList(vm.contentGroup).then(function (result) {
                vm.items = result.data;
            });
        };
        vm.reload();

        vm.reloadHeader = function() {
            return svc.getHeader(vm.contentGroup).then(function(result) {
                vm.header = result.data;
            });
        };
        vm.reloadHeader();

        vm.ok = function ok() {
            svc.saveList(vm.contentGroup, vm.items).then(vm.close);
        };
        
        // note: not perfect yet - won't edit presentation of header
        vm.editHeader = function editHeader() {
            var items = [];
            items.push({
                Group: {
                    Guid: vm.contentGroup.guid,
                    Index: 0,
                    Part: "listcontent",
                    Add: vm.header.Id === "0"
                },
                Title: $translate.instant("EditFormTitle.ListContent")
            });
            items.push({
                Group: {
                    Guid: vm.contentGroup.guid,
                    Index: 0,
                    Part: "listpresentation",
                    Add: vm.header.Id === "0"
                },
                Title: $translate.instant("EditFormTitle.ListPresentation")
            });
            eavAdminDialogs.openEditItems(items, vm.reloadHeader);

        };

        vm.close = function () { $modalInstance.dismiss("cancel"); };

    }

} ());