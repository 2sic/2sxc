
// contains all the grunt config for the sxc-api

module.exports = function (grunt) {
    var sxc4ng = "js/AngularJS/2sxc4ng.js";

    grunt.config.merge({


        jshint: {
            Sxc: [sxc4ng, "js/2sxc.api.js"]
        },

        ngAnnotate: {
            Sxc4ng: {
                files: {
                    'js/AngularJS/2sxc4ng.annotated.js': ["js/AngularJS/2sxc4ng.js"]
                }
            }
        },

        uglify: {
            Sxc4ng: {
                files: {
                    'js/AngularJS/2sxc4ng.min.js': ["js/AngularJS/2sxc4ng.annotated.js"]
                }
            },
            SxcCore: {
                files: {
                    'js/2sxc.api.min.js': ["js/2sxc.api.js"],
                }
            }
        },

    });
};