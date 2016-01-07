
// contains all the grunt config for the field string-wysiwyg-tinymce

module.exports = function (grunt) {
    var stringWysiwygTinyMce = {
        src: "<%= paths.bower %>/angular-ui-tinymce/src/*.js",
        concatFile: "<%= paths.dist %>/edit/extensions/field-string-wysiwyg-tinymce/set.js"
    };

    grunt.config.merge({
        concat: {
            stringWysiwygTinyMce: {
                src: stringWysiwygTinyMce.src,
                dest: stringWysiwygTinyMce.concatFile
            }
        },
    });

    grunt.registerTask("import-tinymce", ["concat:stringWysiwygTinyMce"]);

};