
// contains all the grunt config for the in-page fonts

module.exports = function (grunt) {

    grunt.config.merge({
        fonts: {
            cwd: "<%= paths.bower %>/2sxc-icons/",
            dist: "<%= paths.dist %>/lib/fonts/",
            dev: "<%= paths.src %>/sxc-edit/",
            inpage: "<%= paths.src %>/inpage/"
        },

        copy: {
            fonts: {
                files: [
                    {
                        note: "font files",
                        expand: true,
                        flatten: true,
                        cwd: "<%= fonts.cwd %>",
                        src: ["**/*.woff"],
                        dest: "<%= fonts.dist %>"
                    },
                    {
                        note: "full system css definition for current characters",
                        expand: true,
                        flatten: true,
                        cwd: "<%= fonts.cwd %>",
                        src: ["full-system/css/app-icons-codes.css"],
                        dest: "<%= fonts.dev %>"
                    },
                    {
                        note: "full system css definition for inpage characters",
                        expand: true,
                        flatten: true,
                        cwd: "<%= fonts.cwd %>",
                        src: ["in-page-icons/css/inpage-icons-codes.css"],
                        dest: "<%= fonts.inpage %>"
                    }

                ]
            }
        }

    });

    grunt.registerTask("import-icons", ["copy:fonts"]);

};