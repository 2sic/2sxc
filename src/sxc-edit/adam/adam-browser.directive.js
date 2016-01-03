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
                    dropzoneCtrl.adam = scope.vm;
                },
                scope: {
                    contentTypeName: "=",
                    entityGuid: "=",
                    fieldName: "=",
                    subFolder: "=",
                    autoLoad: "=",
                    updateCallback: "="
                },
                controller: "BrowserController",
                controllerAs: "vm"
            };
        });
})();