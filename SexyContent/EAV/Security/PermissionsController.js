(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("PermissionsApp", ['PermissionsServices'])
        .constant('createdBy', '2sic')          // just a demo how to use constant or value configs in AngularJS
        .constant('licence', 'MIT')             // these wouldn't be necessary, just added for learning exprience
        .controller("Admin", AdminController)
    ;



    function AdminController(permissionsSvc) {
        var vm = this;

        permissionsSvc.PermissionTargetGuid = $2sxc.ng.getParameterByName('Target');

        vm.getUrl = permissionsSvc.getUrl;

        vm.permissions = permissionsSvc.allLive();

        vm.tryToDelete = function tryToDelete(title, entityId) {
            var ok = confirm("Delete '" + title + "' (" + entityId + ") ?");
            if (ok)
                permissionsSvc.delete(entityId)
        };

        vm.refresh = function refresh() {
            permissionsSvc.getAll();
        }

        vm.create = function create() {
            alert('todo');
        }

    };

}());