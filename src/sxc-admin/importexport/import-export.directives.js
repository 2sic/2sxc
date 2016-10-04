(function () {

    angular.module("ImportExport")
        .directive("sxcFileRead", FileReadDirective)
        .directive("sxcFileInput", FileInputDirective)
        ;

    /*@ngInject*/
    function FileReadDirective() {
        return {
            scope: {
                sxcFileRead: "="
            },
            link: function (scope, element, attributes) {

                element.bind("change", function (e) {
                    var file = e.target.files[0];
                    var fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        scope.$apply(function () {
                            scope.sxcFileRead = {
                                Name: file.name,
                                Data: e.target.result
                            };
                        });
                    };
                    fileReader.readAsDataURL(file);
                });
            }
        };
    }


    function FileInputDirective() {
        return {
            scope: {
                sxcFileInput: "="
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (e) {
                    scope.sxcFileInput = e.target.files[0];
                    scope.$apply();
                });
            }
        };
    }
}());