/// <binding />
module.exports = function (grunt) {
    "use strict";

    grunt.initConfig();
    
    grunt.config.merge({
        pkg: grunt.file.readJSON("package.json"),

        paths: {
            bower: "bower_components",
            dist: "dist",
            libs: "libs",
            publish: "publish",
            src: "src",
            temp: "tmp",
            tests: "tests"
        }


    });

    grunt.loadNpmTasks("grunt-contrib-watch");

    grunt.task.loadTasks("grunt-tasks");


};