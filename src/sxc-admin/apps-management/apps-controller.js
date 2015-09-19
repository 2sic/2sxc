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
            var result = prompt("Enter App Name (will also be used for folder)");
            if (result)
                svc.create(result);
        };

        
        vm.tryToDelete = function tryToDelete(item) {
            var result = prompt("This cannot be undone. To really delete this app, type (or copy/past) the app-name here: Delete '" + item.Name + "' (" + item.Id + ") ?");
            if(result === item.Name)
                svc.delete(item.Id);
        };

        // note that manage MUST open in a new iframe, to give the entire application 
        // a new initial context. otherwise we get problems with AppId and similar
        vm.manage = function manage(item) {
            var url = window.location.href;
            url = url
                .replace(new RegExp("appid=[0-9]+", "i"), "appid=" + item.Id)
                .replace("dialog=zone", "dialog=app");

            sxcDialogs.openTotal(url, svc.liveListReload);
        };


        vm.browseCatalog = function() {
            window.open("http://2sxc.org/apps");
        };

        vm.import = function imp() {
            alert('todo');
        };

        vm.export = function exp(item)
        {
            alert("todo");
        };

        vm.languages = function languages() {
            sxcDialogs.openLanguages(zoneId, vm.refresh);
        };

        vm.close = function () { $modalInstance.dismiss("cancel");};
    }

} ());