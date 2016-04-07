(function () {
    var module = angular.module("2sxc.view", [
            "2sxc4ng",
            "EavAdminUi", // dialog (modal) controller
            "pascalprecht.translate",
            "SxcInpageTemplates",
            "EavConfiguration",
            "ui.bootstrap"
        ])
        .config(function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("sxc-admin");
        })
    ;

    //module.config(function ($translateProvider, AppInstanceId, $translatePartialLoaderProvider, languages) {

    //    // var globals = $2sxc(AppInstanceId).manage._manageInfo;

    //    // add translation table
    //    $translateProvider
    //        .preferredLanguage(languages.currentLanguage.split("-")[0])
    //        .useSanitizeValueStrategy("escapeParameters")   // this is very important to allow html in the JSON files
    //        .fallbackLanguage(languages.fallbackLanguage)
    //        .useLoader("$translatePartialLoader", {
    //            urlTemplate: languages.i18nRoot + "{part}-{lang}.js"
    //        })
    //        .useLoaderCache(true);

    //    $translatePartialLoaderProvider.addPart("inpage");
    //});


})();