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

                // todo: change "scope" to bindToController whenever I have time - http://blog.thoughtram.io/angularjs/2015/01/02/exploring-angular-1.3-bindToController.html
                scope: {
                    // Identity fields
                    contentTypeName: "=",
                    entityGuid: "=",
                    fieldName: "=",

                    // configuration general
                    subFolder: "=",
                    folderDepth: "=", 
                    metadataContentTypes: "=",
                    allowAssetsInRoot: "=",
                    showImagesOnly: "=?",

                    // binding and cross-component communication
                    autoLoad: "=",
                    updateCallback: "=",
                    registerSelf: "=",

                    // basic functionality
                    enableSelect: "=",
                    ngDisabled: "="
                },
                controller: "BrowserController",
                controllerAs: "vm"

            };
        });
})();