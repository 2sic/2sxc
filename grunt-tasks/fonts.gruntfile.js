
// contains all the grunt config for the in-page fonts

module.exports = function (grunt) {

    grunt.config.merge({
        fonts: {
            cwd: "<%= paths.bower %>/2sxc-icons/in-page-icons/",
            dist: "<%= paths.dist %>/lib/fonts/"
        },

        copy: {
            fonts: {
                files: [
                    {
                        expand: true,
                        flatten: true,
                        cwd: "<%= fonts.cwd %>",
                        src: ["**/*.woff"],
                        dest: "<%= fonts.dist %>"
                    }

                ]
            }
        }

    });

    grunt.registerTask("import-icons", ["copy:fonts"]);

};