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
    var i18n = {
        cwd: "src/i18n/",
        dist: "dist/i18n/"
    };
    var languagePacks = {
        cwd: "bower_components/2sxc-eav-languages/dist/i18n/",
        dist: "dist/i18n/",
        filter: ["**/*.js", "!**-en.js"]
    };
    var sxc4ng = "js/AngularJS/2sxc4ng.js";



  // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON("package.json"),

        jshint: {
            options: {
                laxbreak: true,
                scripturl: true
            },
            all: ["gruntfile.js", sxcadmin.cwd, inpage.cwd, eavconf.cwd, sxcedit.cwd, sxc4ng],
            Sxc: ["js/2sxc.api.js"]
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
                        cwd: sxcadmin.cwd,
                        src: ["**", "!**/*Spec.js"],
                        dest: sxcadmin.tmp
                    },
                    {
                        expand: true,
                        cwd: sxcedit.cwd,
                        src: ["**", "!**/*Spec.js"],
                        dest: sxcedit.tmp
                    },
                    {
                        expand: true,
                        cwd: inpage.cwd,
                        src: ["**/*.*"],
                        dest: inpage.tmp
                    },
                    {
                        expand: true,
                        cwd: eavconf.cwd,
                        src: ["**/*.*"],
                        dest: eavconf.tmp
                    },
                    {
                        expand: true,
                        cwd: "src/dnn/",
                        src: ["**/*.*"],
                        dest: "dist/dnn/"
                    }
                ]
            },
            i18n: {
                files: [
                    {
                        expand: true,
                        cwd: "src/i18n/", 
                        src: ["**/*.json"],
                        dest: "dist/i18n/", 
                        rename: function (dest, src) {
                            return dest + src.replace(".json", ".js");
                        }
                    }

                ]
            },
            data: { // currently only used for source-editor-snippets
                files: [
                    {
                        expand: true,
                        flatten: true,
                        cwd: "src/sxc-admin/", 
                        src: ["**/*.json"],
                        dest: "dist/sxc-admin/", 
                        rename: function (dest, src) {
                            return dest + src.replace(".json", ".js");
                        }
                    }

                ]
            },
            languagePacks: {
                files: [
                    {
                        expand: true,
                        cwd: languagePacks.cwd,
                        src: languagePacks.filter,
                        dest: languagePacks.dist
                    }
                ]
            }
        },


        ngtemplates: {
            default: {
                options: {
                    module: "SxcTemplates",
                    append: true,
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
            inpage: {
                options: {
                    module: "SxcInpageTemplates",
                    append: true,
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
            adminCss: {
                src: sxcedit.tmp + "**/*.css",
                dest: sxcedit.concatCss
            },
        },


        ngAnnotate: {
            options: {
                // Task-specific options go here. 
                // disable sourceMap for now as we can't pass it through to uglify yet (don't know how) sourceMap: true
            },
            sxcadmin: {
                expand: true,
                src: sxcadmin.concatFile,
                extDot: "last"          // Extensions in filenames begin after the last dot 
            },
            sxcedit: {
                expand: true,
                src: sxcedit.concatFile,
                extDot: "last"          // Extensions in filenames begin after the last dot 
            },
            inpage: {
                expand: true,
                src: inpage.concatFile,
                extDot: "last"          // Extensions in filenames begin after the last dot 
            },
            eavconf: {
                expand: true,
                src: eavconf.concatFile,
                extDot: "last"          // Extensions in filenames begin after the last dot 
            },
            Sxc4ng: {
                files: {
                    'js/AngularJS/2sxc4ng.annotated.js': ["js/AngularJS/2sxc4ng.js"]
                }
            }
        },

        uglify: {
            options: {
                banner: "/*! <%= pkg.name %> <%= grunt.template.today(\"yyyy-mm-dd\") %> */\n",
                sourceMap: true
            },
            sxcadmin: {
                src: sxcadmin.concatFile,
                dest: sxcadmin.uglifyFile
            },
            sxcedit: {
                src: sxcedit.concatFile,
                dest: sxcedit.uglifyFile
            },
            inpage: {
                src: inpage.concatFile,
                dest: inpage.uglifyFile
            },
            eavconf: {
                src: eavconf.concatFile,
                dest: eavconf.uglifyFile
            },
            Sxc4ng: {
                files: {
                    'js/AngularJS/2sxc4ng.min.js': ["js/AngularJS/2sxc4ng.annotated.js"]
                }
            }, 
            SxcCore: {
                files: {
                    'js/2sxc.api.min.js': ["js/2sxc.api.js"],
                    //'js/dnn-inpage-edit.min.js': ["js/dnn-inpage-edit.js"]
                }
            }
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
    grunt.loadNpmTasks("grunt-ng-templates");
    grunt.loadNpmTasks("grunt-contrib-compress");
    grunt.loadNpmTasks("grunt-contrib-cssmin");

    // Default task(s).
    grunt.registerTask("build", [
        "jshint",
        "clean:tmp",
        "copy",
        "ngtemplates",
        "concat",
        "ngAnnotate",
        "uglify",
        "cssmin",
        //"clean:tmp",
    ]);

    grunt.registerTask("build-auto", [
        "watch:sxcbuild"
    ]);

  grunt.registerTask("default", ["jshint", "ngAnnotate", "uglify"]);

};