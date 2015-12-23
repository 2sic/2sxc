
// contains all the grunt config for the dnn processes

module.exports = function (grunt) {

    grunt.config.merge({
        copy: {
            dnn: {
                files: [
                    {
                        expand: true,
                        cwd: "<%= paths.src %>/dnn/",
                        src: ["**/*.*"],
                        dest: "<%= paths.dist %>/dnn/"
                    }
                ]
            }
        }
    });
};