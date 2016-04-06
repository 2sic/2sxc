(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExportApp", [
        "EavConfiguration", // config
        "SxcTemplates", // inline templates
        "EavAdminUi", // dialog (modal) controller
        "EavServices", // multi-language stuff
        "SxcServices"
    ]);
} ());