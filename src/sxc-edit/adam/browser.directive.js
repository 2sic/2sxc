(function () {
    /* jshint laxbreak:true*/

    angular.module("Adam")
        .directive("adamBrowser", function() {
            return {
                restrict: "E",
                templateUrl: "adam/browser.html",

                //replace: true,
                transclude: false,
                require: "^dropzone",
                link: function postLink(scope, elem, attrs, dropzoneCtrl) {
                    // connect this adam to the dropzone
                    dropzoneCtrl.adam = scope.vm;       // so the dropzone controller knows what path etc.
                    scope.vm.dropzone = dropzoneCtrl;   // so we can require an "open file browse" dialog
                },
                scope: {
                    contentTypeName: "=",
                    entityGuid: "=",
                    fieldName: "=",
                    subFolder: "=",
                    showFolders: "=",
                    autoLoad: "=",
                    updateCallback: "=",
                    registerSelf: "=",
                    enableSelect: "=",
                    ngDisabled: "="
                },
                controller: "BrowserController",
                controllerAs: "vm"
            };
        });
})();