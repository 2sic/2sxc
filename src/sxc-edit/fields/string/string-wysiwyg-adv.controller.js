
(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    angular.module("sxcFieldTemplates")
        .config(function(formlyConfigProvider) {

            // for now identical with -adv, but later will change
            formlyConfigProvider.setType({
                name: "string-wysiwyg-adv",
                templateUrl: "fields/string/string-wysiwyg-adv.html",
                wrapper: ["eavLabel", "bootstrapHasError", "eavLocalization"]
            });


        });

})();