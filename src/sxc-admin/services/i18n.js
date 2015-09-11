(function () {
    angular.module("Sxci18n", [
        "pascalprecht.translate",
        "EavConfiguration"
    ])

    .config(function ($translateProvider, languages) {
        $translateProvider
          .preferredLanguage(languages.currentLanguage.split('-')[0])
          .useSanitizeValueStrategy("escape")
          .fallbackLanguage(languages.defaultLanguage.split('-')[0])
          .useStaticFilesLoader({
          	prefix: languages.i18nRoot + "sxc-admin-",
              suffix: ".js"
          });
    });
})();