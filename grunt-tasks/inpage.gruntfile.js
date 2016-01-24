
// contains all the grunt config for the i18n processes

module.exports = function (grunt) {
    grunt.config.merge({
        inpage: {
            cwd: "<%= paths.src %>/inpage/",
            cwdJs: ["<%= paths.src %>/inpage/**/*.js"],
            tmp: "<%= paths.temp %>/inpage/",
            templates: "<%= paths.temp %>/inpage/inpage-templates.js",
            dist: "<%= paths.dist %>/admin/",
            concatFile: "<%= paths.dist %>/inpage/inpage.js",
            uglifyFile: "<%= paths.dist %>/inpage/inpage.min.js"
        },


        copy: {
            testInpage: {
                files: [
                {
                    expand: true,
                    cwd: "<%= inpage.cwd %>",
                    src: ["**/*.*"],
                    dest: "<%= inpage.tmp %>"
                }
                ]
            }
        }

    });

    grunt.registerTask("build-inpageTest", ["copy:testInpage"]);
};