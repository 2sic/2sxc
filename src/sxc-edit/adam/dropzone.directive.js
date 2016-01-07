/* js/fileAppDirectives */

angular.module("Adam")
    .directive("dropzone", function (sxc, tabId, dragClass, adamSvc) {
        return {
            restrict: "C",
            link: function(scope, element, attrs, controller) {
                var header = scope.$parent.to.header;
                var field = scope.$parent.options.key;
                var entityGuid = header.Guid;
                var svc = adamSvc(header.ContentTypeName, entityGuid, field, "");
                var url = svc.url;

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

                    dictDefaultMessage: "",
                    addRemoveLinks: false,
                    previewsContainer: ".field-" + field.toLowerCase() + " .dropzone-previews",
                    // we need a clickable, because otherwise the entire area is clickable. so i'm just making the preview clickable, as it's not important
                    clickable: ".field-" + field.toLowerCase() + " .invisible-clickable" // " .dropzone-adam"
                };



                var eventHandlers = {
                    'addedfile': function(file) {
                        scope.$apply(function() {
                            scope.uploading = true;
                        });
                    },

                    "processing": function(file) {
                        this.options.url = svc.uploadUrl(controller.adam.subFolder);
                    },

                    'success': function(file, response) {
                        if (response.Success) {
                            svc.addFullPath(response);  // calculate additional infos
                            scope.$parent.afterUpload(response);
                        } else {
                            alert("Upload failed because: " + response.Error);
                        }
                    },

                    "queuecomplete": function(file) {
                        if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                            scope.uploading = false;
                            controller.adam.refresh();
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

                controller.openUpload = function() {
                    dropzone.hiddenFileInput.click();
                };
            },

            // This controller is needed, because it needs an API which can talk to other directives
            controller: function() {
                var vm = this;
                vm.adam = {
                    show: false,
                    subFolder: "",
                    refresh: function () { }
                };
            }
        };
    });