(function() {
    /* jshint laxbreak:true*/
    angular.module("Adam")
        .directive("dropzoneUploadPreview", function() {
            return {
                restrict: "E",
                templateUrl: "adam/dropzone-upload-preview.html",
                replace: true,
                transclude: false
            };
        });
})();