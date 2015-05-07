module.exports = function(grunt) {

  // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        jshint: {
            grunt: ['Gruntfile.js'],
            Sxc4ng: ['Js/AngularJS/2sxc4ng.js'],
            // Sxc: ['Js/2sxc.api.js'], // commented out for now, has about 10 suggestions but won't fix them yet
            // SxcManage: ['Js/2sxc.api.manage.js'], // 2015-05-07 has about 13 suggestions, won't fix yet
            // SxcTemplate: ['Js/2sxc.TemplateSelector.js'], // 2015-05-07 has about 4 suggestions for null-comparison, won't fix yet
        },

        ngAnnotate: {
            options: {
                // Task-specific options go here. 
                // disable sourceMap for now as we can't pass it through to uglify yet (don't know how) sourceMap: true
            },
            Sxc4ng: {
                files: {
                    'Js/AngularJS/2sxc4ng.annotated.js': ['Js/AngularJS/2sxc4ng.js']
                }
            },
            SxcModuleUi: {
                    files: {
                        'Js/2sxc.TemplateSelector.annotated.js': ['Js/2sxc.TemplateSelector.js']
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
                    'Js/AngularJS/2sxc4ng.min.js': ['Js/AngularJS/2sxc4ng.annotated.js']
                }
            }, 
            SxcCore: {
                files: {
                    'Js/2sxc.api.min.js': ['Js/2sxc.api.js'],
                    'Js/2sxc.api.manage.min.js': ['Js/2sxc.api.manage.js'],
                    'Js/2sxc.TemplateSelector.min.js': ['Js/2sxc.TemplateSelector.annotated.js']
                }
            },
            SxcModuleUi: {
                files: {
                    'Js/2sxc.TemplateSelector.min.js': ['Js/2sxc.TemplateSelector.annotated.js']
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