
// contains all the grunt config for the source-editor and it's data

module.exports = function (grunt) {

    grunt.config.merge({
        copy: {
            "source-editor-libs": {
                files: [
                    {
                        expand: true,
                        flatten: true,
                        cwd: "<%= paths.bower %>/angular-ui-ace/",
                        src: ["**/*.js"],
                        dest: "<%= paths.dist %>/lib/angular-ui-ace/"
                    }
                ]
            }
        },
        go: {
            options: {
                excel: "<%= paths.src %>/sxc-develop/source-editor/snippets.xlsx",
                json: "<%= paths.dist %>/sxc-develop/snippets.json.js",
                to: "json",
                formating: true
            },
            dist: {}
        },
        watch: {
            options: {
                atBegin: true
            },
            snippets: {
                files: ["<%= paths.src %>/sxc-develop/source-editor/snippets.xlsx"],
                tasks: ["go"]
            }
        }

    });

    grunt.loadNpmTasks("json.excel");

    grunt.registerTask("build-snippets-auto", ["watch:snippets"]);
};