
// contains all the grunt config for importing languages

module.exports = function (grunt) {

    grunt.config.merge({
        languagePacks: {
            cwd: "<%= paths.bower %>/2sxc-eav-languages/dist/i18n/",
            dist: "<%= paths.dist %>/i18n/",
            filter: ["**/*.js", "!**-en.js"]
        },


        copy: {
            languagePacks: {
                files: [
                    {
                        expand: true,
                        cwd: "<%= languagePacks.cwd %>",
                        src: "<%= languagePacks.filter %>",
                        dest: "<%= languagePacks.dist %>"
                    }
                ]
            }

        }

    });

    grunt.registerTask("import-languages", ["copy:languagePacks"]);
};