(function() {
    /* jshint laxbreak:true*/

    angular.module("Adam", [])
        /*@ngInject*/
        .directive("adamHint", function () {
            return {
                restrict: "E",
                replace: false,
                transclude: false,
                templateUrl: "adam/adam-hint.html"
            };
        });
})();