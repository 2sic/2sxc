module.exports = function(grunt) {

  // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        jshint: {
            grunt: ['Gruntfile.js'],
            Sxc4ng: ['js/AngularJS/2sxc4ng.js'],
            // Sxc: ['js/2sxc.api.js'], // commented out for now, has about 10 suggestions but won't fix them yet
            // SxcManage: ['js/2sxc.api.manage.js'], // 2015-05-07 has about 13 suggestions, won't fix yet
            // SxcTemplate: ['js/template-selector/template-selector.js'], // 2015-05-07 has about 4 suggestions for null-comparison, won't fix yet
        },

        ngAnnotate: {
            options: {
                // Task-specific options go here. 
                // disable sourceMap for now as we can't pass it through to uglify yet (don't know how) sourceMap: true
            },
            Sxc4ng: {
                files: {
                    'js/AngularJS/2sxc4ng.annotated.js': ['js/AngularJS/2sxc4ng.js']
                }
            },
            SxcModuleUi: {
                    files: {
                        'js/template-selector/template-selector.annotated.js': ['js/template-selector/template-selector.js']
                    }
            }
        },

        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> */\n',
                sourceMap: true
            },
            Sxc4ng: {
                files: {
                    'js/AngularJS/2sxc4ng.min.js': ['js/AngularJS/2sxc4ng.annotated.js']
                }
            }, 
            SxcCore: {
                files: {
                    'js/2sxc.api.min.js': ['js/2sxc.api.js'],
                    'js/2sxc.api.manage.min.js': ['js/2sxc.api.manage.js'],
                    'js/template-selector/template-selector.min.js': ['js/template-selector/template-selector.annotated.js'],
                    'js/dnn-view-edit.min.js': ['js/dnn-view-edit.api.js']
    }
            },
            SxcModuleUi: {
                files: {
                    'js/template-selector/template-selector.min.js': ['js/template-selector/template-selector.annotated.js']
                }

            }
        }
    });

  grunt.loadNpmTasks('grunt-ng-annotate');
  
  // Load the plugin that provides the "uglify" task.
  grunt.loadNpmTasks('grunt-contrib-uglify');

  grunt.loadNpmTasks('grunt-contrib-jshint');

// Default task(s).
  grunt.registerTask('default', ['jshint', 'ngAnnotate', 'uglify']);

};