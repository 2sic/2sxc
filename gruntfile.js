module.exports = function(grunt) {

  // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

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

  // Default task(s).
  grunt.registerTask('default', ['ngAnnotate', 'uglify']);

};