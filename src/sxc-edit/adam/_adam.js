(function() { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("Adam", [
        "SxcServices",
        "EavConfiguration", // config
        "EavServices", // multi-language stuff
        ])
        ;

} ());