
(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    angular.module("sxcFieldTemplates", [
        "formly",
        "formlyBootstrap",
        "ui.bootstrap",
        "ui.tree",
        "2sxc4ng",
        "SxcEditTemplates",
        "EavConfiguration",
        "SxcServices",
        "Adam"
    ]);

})();