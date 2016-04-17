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
            $translatePartialLoaderProvider.addPart("sxc-admin");
        })
    ;

})();