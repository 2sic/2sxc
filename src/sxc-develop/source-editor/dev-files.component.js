

angular.module('SourceEditor').component('devFiles', {
    templateUrl: 'source-editor/dev-files.html',
    controller: function (appAssetsSvc, appId) {
        var vm = angular.extend(this, {
            show: false,
            svc: appAssetsSvc(appId),

            toggle: function() {
                vm.show = !vm.show;
                if (!vm.assets)
                    vm.assets = vm.svc.liveList();
            },

            editFile: function(filename) {
                window.open(vm.assembleUrl(filename));
                vm.toggle();
            },

            assembleUrl: function(newFileName) {
                // note that as of now, we'll just use the initial url and change the path
                // then open a new window
                var url = window.location.href;
                var newItems = JSON.stringify([{ Path: newFileName }]);
                return url.replace(new RegExp("items=.*?%5d", "i"), "items=" + encodeURI(newItems)); // note: sometimes it doesn't have an appid, so it's [0-9]* instead of [0-9]+
            },

            addFile: function() {
                // todo: i18n
                var result = prompt("please enter full file name"); // $translate.instant("AppManagement.Prompt.NewApp"));
                if (result)
                    vm.svc.create(result);

            }
        });

    },
    controllerAs: "vm",
    bindings: {
        fileName: "<",
        type: "<"
    }
});