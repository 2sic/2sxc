(function () { 

    angular.module("TemplatesApp", [
        "TemplatesServices",
        "EavConfiguration",
        "EavAdminUi",
        "Eavi18n",
        "EavDirectives"
    ])
        .controller("TemplateList", TemplateListController)
        ;

    function TemplateListController(templatesSvc, eavAdminDialogs, eavConfig, appId, $modalInstance, $sce) {
        var vm = this;
        var svc = templatesSvc(appId);

        vm.edit = function edit(item) {
            var url = vm.getOldEditUrl();
            url += (url.indexOf("?") == -1) ? "?" : "&";
            url += "templateid=" + item.Id;
            window.open(url);
            // eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
        };

        vm.getOldEditUrl = function() {
            var myUrl = window.location.href;
            var newUrl = myUrl.replace("/ctl/content/", "/ctl/edittemplate/")
                .replace("ctl=content", "ctl=edittemplate");

            return newUrl;
        };

        vm.add = function add() {
            // templ till the edit dialog is JS-only
            window.open(vm.getOldEditUrl());

            // probably afterwards
            var resolve = eavAdminDialogs.CreateResolve({
                appId: appId,
                svc: svc
            });
            return eavAdminDialogs.OpenModal(
                "templates/edit.html",
                "TemplateEdit as vm",
                "lg",
                resolve,
                svc.liveListReload);
        };

        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.permissions = function permissions(item) {
            return eavAdminDialogs.openPermissionsForGuid(appId, item.Guid, svc.liveListReload);
        };

        vm.tryToDelete = function tryToDelete(item) {
            if (confirm("Delete '" + item.Title + "' (" + item.Id + ") ?"))
                svc.delete(item.Id);
        };

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }

} ());