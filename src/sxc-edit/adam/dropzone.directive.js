/* js/fileAppDirectives */

angular.module("sxcFieldTemplates")
    .directive("dropzone", function (sxc, tabId, dragClass) {
        return {
            restrict: "C",
            link: function (scope, element, attrs) {
                var header = scope.$parent.to.header;
                var field = scope.$parent.options.key;
                var entityGuid = header.Guid; 
                var url = sxc.resolveServiceUrl("app-content/" + header.ContentTypeName + "/" + entityGuid + "/" + field);

                var config = {
                    url: url,
                    urlRoot: url,
                    maxFilesize: 100,
                    paramName: "uploadfile",
                    maxThumbnailFilesize: 10,

                    headers: {
                        "ModuleId": sxc.id,
                        "TabId": tabId
                    },

                    //previewTemplate: "<div></div>",
                    dictDefaultMessage: "",
                    addRemoveLinks: false,
                    previewsContainer: ".field-" + field.toLowerCase() +  " .dropzone-previews",
                    clickable: ".field-" + field.toLowerCase() +  " .dropzone-adam"
                };

                var eventHandlers = {
                    'addedfile': function(file) {
                        scope.$apply(function() {
                            // scope.fileAdded = true; // seems unused
                            scope.uploading = true;
                        });
                    },

                    "processing": function (file) {
                        this.options.url = (scope.adam.subFolder === "")
                            ? this.options.urlRoot
                            : this.options.urlRoot + "?subfolder=" + scope.adam.subFolder;
                    },

                    'success': function (file, response) {
                        if (response.Success) {
                            scope.$parent.value.Value = "File:" + response.FileId;
                            scope.$apply();
                        } else {
                            alert("Upload failed because: " + response.Error);
                        }
                    },

                    "queuecomplete": function (file) {
                        if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                            scope.uploading = false;
                            if (scope.adam && scope.adam.queueComplete)
                                scope.adam.queueComplete();
                        }
                    }
                };

                var dropzone = new Dropzone(element[0], config);

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