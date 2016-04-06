
// contains all the grunt config for the i18n processes

module.exports = function (grunt) {

    grunt.config.merge({
        i18n: {
            cwd: "<%= paths.src %>/i18n/",
            dist: "<%= paths.dist %>/i18n/"
        },

        copy: {
            i18n: {
                files: [
                    {
                        expand: true,
                        cwd: "<%= i18n.cwd %>",
                        src: ["**/*.json"],
                        dest: "<%= i18n.dist %>",
                        rename: function (dest, src) {
                            return dest + src.replace(".json", ".js");
                        }
                    }

                ]
            }
        }

    });
};