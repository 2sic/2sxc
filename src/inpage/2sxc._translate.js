(function() {
    var initialized = false;
    $2sxc.translate = function(key) {
        return $.t(key);
    };
    //#endregion

    $2sxc._initTranslate = function(manage) {
        if (!initialized) {
            window.i18next
                .use(window.i18nextXHRBackend)
                .init({
                    lng: "en", // todo: automate
                    fallbackLng: "en",
                    whitelist: ["en", "de", "fr", "it", "uk"],
                    preload: ["en"],
                    backend: {
                        // path where resources get loaded from
                        // loadPath: '/locales/{{lng}}/{{ns}}.json',
                        loadPath: manage.editContext.Environment.SxcRootUrl + "desktopmodules/tosic_sexycontent/dist/i18n/inpage-{{lng}}.js"
                    }
                }, function (err, t) {
                    // for options see
                    // https://github.com/i18next/jquery-i18next#initialize-the-plugin
                    jqueryI18next.init(i18next, $);
                    // start localizing, details:
                    // https://github.com/i18next/jquery-i18next#usage-of-selector-function
                    $('ul.sc-menu').localize();
                });
            initialized = true;
        }
    };
})();
