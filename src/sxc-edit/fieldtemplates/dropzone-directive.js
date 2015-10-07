/* js/fileAppDirectives */

angular.module('sxcFieldTemplates')
    .directive('dropzone', function (sxc, tabId) {
        return {
            restrict: 'C',
            link: function (scope, element, attrs) {
                var header = scope.$parent.to.header;
                var field = scope.$parent.options.key;
                var url = sxc.resolveServiceUrl('app-content/' + header.ContentTypeName + '/' + header.Guid + '/' + field);

                var config = {
                    url: url,// 'http://localhost:8080/upload',
                    maxFilesize: 100,
                    paramName: "uploadfile",
                    maxThumbnailFilesize: 10,
                    //parallelUploads: 1,
                    //autoProcessQueue: true, // false
                    
                    headers: {
                        "ModuleId": sxc.id,
                        "TabId": tabId
                    }

                };

                var eventHandlers = {
                    //'addedfile': function(file) {
                    //    scope.file = file;
                    //    if (this.files[1] !== null) {
                    //        this.removeFile(this.files[0]);
                    //    }
                    //    scope.$apply(function() {
                    //        scope.fileAdded = true;
                    //    });
                    //},

                    //'success': function(file, response) {
                    //}

                };

                dropzone = new Dropzone(element[0], config);

                angular.forEach(eventHandlers, function(handler, event) {
                    dropzone.on(event, handler);
                });

                scope.processDropzone = function() {
                    dropzone.processQueue();
                };

                scope.resetDropzone = function() {
                    dropzone.removeAllFiles();
                };
            }
        };
    });