(function () {
    /* jshint laxbreak:true*/

    angular.module("Adam")
        .directive("adamBrowser", function() {
            return {
                restrict: "E",
                templateUrl: "adam/browser.html",

                //replace: true,
                transclude: false,
                //link: function postLink(scope, elem, attrs) {
                //    var icn = attrs.icon;
                //    elem.addClass("glyphicon glyphicon-" + icn);
                //},
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