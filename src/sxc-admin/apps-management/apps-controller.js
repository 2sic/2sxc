(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("AppsManagementApp", [
        "EavServices",
        "EavConfiguration",
        "SxcServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "SxcAdminUi"
    ])
        .config(function ($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        })

        .controller("AppList", AppListController)
        ;

    function AppListController(appsSvc, eavAdminDialogs, sxcDialogs, eavConfig, zoneId, $modalInstance) {
        var vm = this;

        var svc = appsSvc(zoneId);
        vm.items = svc.liveList();
        vm.refresh = svc.liveListReload;

        vm.config = function config(item) {
            eavAdminDialogs.openItemEditWithEntityId(item.ConfigurationId, svc.liveListReload);
        };

        vm.add = function add() {
            // todo: ask
            var result = confirm("Enter App Name (will also be used for folder)");
            if (result)

            // todo: create
            svc.create(result);
        };

        
        vm.tryToDelete = function tryToDelete(item) {
            var result = input("This cannot be undone. To really delete this app, type (or copy/past) the app-name here: Delete '" + item.Name + "' (" + item.Id + ") ?");
            if(result === item.Name)
                svc.delete(item.Id);
        };

        // todo: make this open an i-frame lightbox instead of a new window
        vm.manage = function manage(item) {
            var url = window.location.href;
            url = url
                .replace(new RegExp("appid=[0-9]+", "i"), "appid=" + item.Id)
                .replace("dialog=zone", "dialog=app");

            sxcDialogs.openTotal(url, svc.liveListReload);
            //window.open(url);
            //sxcDialogs.openAppMain(item.Id, svc.liveListReload);
        };

        vm.export = function exp(item)
        {
            alert("todo");
        };

        vm.close = function () { $modalInstance.dismiss("cancel");};
    }

} ());