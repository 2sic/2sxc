(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("SxcFilters", [])
        .constant("createdBy", "2sic") // just a demo how to use constant or value configs in AngularJS
        .constant("license", "MIT") // these wouldn't be necessary, just added for learning exprience
        .filter('trustAsResourceUrl', function($sce) {
            return function(val) {
                return $sce.trustAsResourceUrl(val);
            };
        })

        .filter('treatAsSafeHtml')
    ;

} ());