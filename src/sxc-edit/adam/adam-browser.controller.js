(function () {
    /* jshint laxbreak:true */
    "use strict";

    var app = angular.module("Adam"); 

    // The controller for the main form directive
    app.controller("BrowserController", BrowserController);
    
    function BrowserController($scope, adamSvc, debugState) {
        var vm = this;
        vm.debug = debugState;
        vm.contentTypeName = $scope.contentTypeName;
        vm.entityGuid = $scope.entityGuid;
        vm.fieldName = $scope.fieldName;
        vm.show = false;
        vm.showFolders = false;
        vm.subFolder = $scope.subFolder || "";

        vm.activate = function () {
            if($scope.autoLoad)
                vm.toggle();
        };

        // load svc...
        vm.svc = adamSvc(vm.contentTypeName, vm.entityGuid, vm.fieldName, vm.subFolder);

        // refresh - also used by callback after an upload completed
        vm.refresh = vm.svc.liveListReload;

        vm.get = function () {
            vm.items = vm.svc.liveList();
            vm.folders = vm.svc.folders;
        };

        vm.toggle = function toggle() {
            vm.show = !vm.show;
            if (vm.show)
                vm.get();
        };

        vm.select = function (fileItem) {
            if (!fileItem.IsFolder)
                $scope.updateCallback("File:" + fileItem.Id);
            else
                vm.goIntoFolder(fileItem);
        };

        vm.addFolder = function () {
            var folderName = window.prompt("Folder Name?");
            vm.svc.addFolder(folderName)
                .then(vm.refresh);
        };

        vm.del = function del(item) {
            var ok = window.confirm("delete ok?");
            if (ok)
                vm.svc.delete(item);
        };

        //#region Folder Navigation
        vm.goIntoFolder = function (folder) {
            var subFolder = vm.svc.goIntoFolder(folder);
            vm.subFolder = subFolder;
        };

        vm.goUp = function () {
            vm.subFolder = vm.svc.goUp();
        };
        //#endregion

        vm.activate();
    }

})();
