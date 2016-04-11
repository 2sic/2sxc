(function () { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("ImportExport", [
        "EavConfiguration", // Config
        "SxcTemplates",     // Inline templates
        "EavAdminUi",       // Dialog (modal) controller
        "EavServices",      // Multi-language stuff
        "SxcServices"
    ]);
} ());