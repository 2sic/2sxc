(function() { // TN: this is a helper construct, research iife or read https://github.com/johnpapa/angularjs-styleguide#iife

    angular.module("Adam", [
        "SxcServices",
        "EavConfiguration", // config
        "EavServices" // multi-language stuff
            //"SxcTemplates", // inline templates
            //"EavAdminUi", // dialog (modal) controller
            //"SxcFilters", // for inline unsafe urls
            //"ContentTypesApp",
            //"PipelineManagement",
            //"TemplatesApp",
            //"ImportExportApp",
            //"AppSettingsApp",
            //"SystemSettingsApp",
            //"WebApiApp"
        ])
        //.config(function($translatePartialLoaderProvider) {
        //    // ensure the language pack is loaded
        //    $translatePartialLoaderProvider.addPart("sxc-admin");
        //})
        //.controller("AppMain", MainController);
        ;

} ());