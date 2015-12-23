/// <binding />
module.exports = function (grunt) {
    "use strict";
    var distRoot = "dist/";
    var tmpRoot = "tmp/";

    var sxcadmin = {
        cwd: "src/sxc-admin/",
        cwdJs: ["src/sxc-admin/**/*.js"],
        tmp: "tmp/sxc-admin/",
        templates: "tmp/sxc-admin/sxc-templates.js",
        dist: "dist/sxc-admin/",
        concatFile: "dist/sxc-admin/sxc-admin.js",
        uglifyFile: "dist/sxc-admin/sxc-admin.min.js"
    };
    var sxcedit = {
        cwd: "src/sxc-edit/",
        cwdJs: ["src/sxc-edit/**/*.js"],
        tmp: "tmp/sxc-edit/",
        templates: "tmp/sxc-edit/sxc-templates.js",
        dist: "dist/sxc-edit/",
        concatFile: "dist/sxc-edit/sxc-edit.js",
        uglifyFile: "dist/sxc-edit/sxc-edit.min.js",
        concatCss: "dist/sxc-edit/sxc-edit.css",
        concatCssMin: "dist/admin/sxc-edit.min.css"
    };
    var inpage = {
        cwd: "src/inpage/",
        cwdJs: ["src/inpage/**/*.js"],
        tmp: "tmp/inpage/",
        templates: "tmp/inpage/inpage-templates.js",
        dist: "dist/admin/",
        concatFile: "dist/inpage/inpage.js",
        uglifyFile: "dist/inpage/inpage.min.js"
    };
    var eavconf = {
        cwd: "src/config/",
        cwdJs: ["src/config/**/*.js"],
        tmp: "tmp/config/",
        dist: "dist/config/",
        concatFile: "dist/config/config.js",
        uglifyFile: "dist/config/config.min.js"
    };
    var designer = {
        cwd: "src/sxc-designer/",
        cwdJs: ["src/sxc-designer/**/*.js"],
        tmp: "tmp/sxc-designer/",
        templates: "tmp/sxc-designer/sxc-templates.js",
        dist: "dist/sxc-designer/",
        concatFile: "dist/sxc-designer/sxc-designer.js",
        uglifyFile: "dist/sxc-designer/sxc-designer.min.js"
    };


  // Project configuration.
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
        },
        
        jshint: {
            options: {
                laxbreak: true,
                scripturl: true
            },
            all: ["gruntfile.js", sxcadmin.cwd, inpage.cwd, eavconf.cwd, sxcedit.cwd, designer.cwd]
        },

        clean: {
            dist: distRoot + "**/*", // only do this when you will re-copy the eav stuff into here
            tmp: tmpRoot + "**/*"
        },

        copy: {
            build: {
                files: [
                    {
                        expand: true,
                        cwd: "src",
                        src: ["*/**", "!**/*Spec.js"],
                        dest: "tmp"
                    }
                ]
            }
        },


        ngtemplates: {
            default: {
                options: {
                    module: "SxcTemplates",
                    append: true,
                    standalone: true,
                    htmlmin: {
                        collapseBooleanAttributes: true,
                        collapseWhitespace: true,
                        removeAttributeQuotes: true,
                        removeComments: true,
                        removeEmptyAttributes: true,
                        removeRedundantAttributes: false,
                        removeScriptTypeAttributes: true,
                        removeStyleLinkTypeAttributes: true
                    }
                },
                files: [
                    {
                        cwd: sxcadmin.tmp,
                        src: ["**/*.html"], 
                        dest: sxcadmin.templates
                    }
                ]
            },
            sxcedit: {
                options: {
                    module: "SxcEditTemplates",
                    append: true,
                    standalone: true,
                    htmlmin: {
                        collapseBooleanAttributes: true,
                        collapseWhitespace: true,
                        removeAttributeQuotes: true,
                        removeComments: true,
                        removeEmptyAttributes: true,
                        removeRedundantAttributes: false,
                        removeScriptTypeAttributes: true,
                        removeStyleLinkTypeAttributes: true
                    }
                },
                files: [
                    {
                        cwd: sxcedit.tmp,
                        src: ["**/*.html"],
                        dest: sxcedit.templates
                    }
                ]
            },
            designer: {
                options: {
                    module: "SourceEditor",
                    append: true,
                    standalone: false,
                    htmlmin: {
                        collapseBooleanAttributes: true,
                        collapseWhitespace: true,
                        removeAttributeQuotes: true,
                        removeComments: true,
                        removeEmptyAttributes: true,
                        removeRedundantAttributes: false,
                        removeScriptTypeAttributes: true,
                        removeStyleLinkTypeAttributes: true
                    }
                },
                files: [
                    {
                        cwd: designer.tmp,
                        src: ["**/*.html"],
                        dest: designer.templates
                    }
                ]
            },
            inpage: {
                options: {
                    module: "SxcInpageTemplates",
                    append: true,
                    standalone: true,
                    htmlmin: {
                        collapseBooleanAttributes: true,
                        collapseWhitespace: true,
                        removeAttributeQuotes: true,
                        removeComments: true,
                        removeEmptyAttributes: true,
                        removeRedundantAttributes: false,
                        removeScriptTypeAttributes: true,
                        removeStyleLinkTypeAttributes: true
                    }
                },
                files: [
                    {
                        cwd: inpage.tmp,
                        src: ["**/*.html"],
                        dest: inpage.templates
                    }
                ]
            }
        },

        concat: {
            default: {
                src: sxcadmin.tmp + "**/*.js",
                dest: sxcadmin.concatFile
            },
            sxcedit: {
                src: sxcedit.tmp + "**/*.js",
                dest: sxcedit.concatFile
            },
            inpage: {
                src: inpage.tmp + "**/*.js",
                dest: inpage.concatFile
            },
            eavconf: {
                src: eavconf.tmp + "**/*.js",
                dest: eavconf.concatFile
            },
            designer: {
                src: designer.tmp + "**/*.js",
                dest: designer.concatFile
            },
            adminCss: {
                src: sxcedit.tmp + "**/*.css",
                dest: sxcedit.concatCss
            }
        },


        ngAnnotate: {
            options: {
                // Task-specific options go here. 
                // don't enable sourceMap for now as we can't pass it through to uglify yet (don't know how) sourceMap: true
            },
            sxcadmin: {
                expand: true,
                src: [sxcadmin.concatFile, sxcedit.concatFile, inpage.concatFile, eavconf.concatFile, designer.concatFile],
                extDot: "last"          // Extensions in filenames begin after the last dot 
            }
        },

        uglify: {
            options: {
                banner: "/*! <%= pkg.name %> <%= grunt.template.today(\"yyyy-mm-dd\") %> */\n",
                sourceMap: true
            },
            sxcadmin: { src: sxcadmin.concatFile,   dest: sxcadmin.uglifyFile   },
            sxcedit: {  src: sxcedit.concatFile,    dest: sxcedit.uglifyFile    },
            designer: { src: designer.concatFile,   dest: designer.uglifyFile    },
            inpage: {   src: inpage.concatFile,     dest: inpage.uglifyFile     },
            eavconf: {  src: eavconf.concatFile,    dest: eavconf.uglifyFile    }
        },
        
        cssmin: {
            options: {
                shorthandCompacting: false,
                roundingPrecision: -1
            },
            target: {
                files: [{
                    expand: true,
                    cwd: distRoot,
                    src: ["**/*.css", "!**/*.min.css"],
                    dest: distRoot,
                    ext: ".min.css"
                }
                ]
            }
        },

        compress: {
            main: {
                options: {
                    mode: "gzip"
                },
                expand: true,
                cwd: distRoot,
                src: ["**/*.min.js"],
                dest: distRoot,
                ext: ".gz.js"
            }
        },

        // note: jasmine not in use yet
        jasmine: {
            
        },
        

        watch: {
            options : {
                atBegin: true
            },
            sxcbuild: {
                files: ["gruntfile.js", "src/**"],
                tasks: ["build"]
            }
        }


    });

    grunt.loadNpmTasks("grunt-contrib-uglify");
    grunt.loadNpmTasks("grunt-contrib-jshint");
    grunt.loadNpmTasks("grunt-contrib-jasmine");
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks("grunt-ng-annotate");
    grunt.loadNpmTasks("grunt-contrib-concat");
    grunt.loadNpmTasks("grunt-contrib-copy");
    grunt.loadNpmTasks("grunt-contrib-clean");
    //grunt.loadNpmTasks("grunt-ng-templates");
    grunt.loadNpmTasks("grunt-angular-templates");
    grunt.loadNpmTasks("grunt-contrib-compress");
    grunt.loadNpmTasks("grunt-contrib-cssmin");

    grunt.task.loadTasks("grunt-tasks");

    // Default task(s).
    grunt.registerTask("build", [
        "jshint",
        "clean:tmp",
        "copy",
        "ngtemplates",
        "concat",
        "ngAnnotate",
        "uglify",
        "cssmin"
    ]);

    grunt.registerTask("build-auto", [
        "watch:sxcbuild"
    ]);

  grunt.registerTask("default", ["jshint", "ngAnnotate", "uglify"]);

};