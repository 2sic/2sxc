
(function () {
	"use strict";

	/* This app registers all field templates for 2sxc in the angularjs sxcFieldTemplates app */

    angular.module("sxcFieldTemplates", [
        "formly",
        "formlyBootstrap",
        "ui.bootstrap",
        "ui.tree",
        "2sxc4ng",
        "SxcEditTemplates",     // temp - was because of bad template-converter, remove once I update grunt
        "EavConfiguration",
        "SxcServices",
        "Adam",
        //"ui.tinymce",   // connector to tiny-mce for angular
        "oc.lazyLoad"   // needed to lazy-load the MCE editor from the cloud
    ]);

})();