(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("AppSettingsApp", [
        "EavConfiguration",     // 
        "ContentTypeServices",
        "ContentItemsServices",
        "SxcTemplates",         // inline templates
        "EavAdminUi",           // dialog (modal) controller
        "Eavi18n"              // multi-language stuff
    ])
        .controller("AppSettings", AppSettingsController)
        ;

    function AppSettingsController(eavAdminDialogs, contentTypeSvc, contentItemsSvc, eavConfig, appId, $filter) {
        var vm = this;
        var svc = contentTypeSvc(appId, "2SexyContent-App");
        vm.items = svc.liveList();

        vm.ready = function ready() {
            return vm.items.length > 0;
        };

        /// Open a content-type configuration dialog for a type (for settins / resources) 
        vm.config = function openConf(staticName) {
            var found = $filter("filter")(vm.items, { StaticName: staticName }, true);
            if (found.length !== 1)
                throw "Found too many settings for the type " + staticName;
            var item = found[0];
            return eavAdminDialogs.openContentTypeFields(item, svc.liveListReload);
        };

        vm.edit = function edit(staticName) {
            var contentSvc = contentItemsSvc(appId, staticName);
            return contentSvc.liveListReload().then(function(result) {
                var found = result.data;
                if (found.length !== 1)
                    throw "Found too many settings for the type " + staticName;
                var item = found[0];
                return eavAdminDialogs.openItemEditWithEntityId(item.Id, svc.liveListReload);
            });
        };

    }

} ());