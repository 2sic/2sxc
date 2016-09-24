/* js/fileAppDirectives */
(function() {
    angular.module("Adam")
        .directive("dropzone", function (sxc, tabId, AppInstanceId, ContentBlockId, dragClass, adamSvc, $timeout, $translate) {

            return {
                restrict: "C",
                link: postLink,

                // This controller is needed, because it needs an API which can talk to other directives
                controller: controller
            };


            // this is the method called after linking the directive, which initializes Dropzone
            function postLink(scope, element, attrs, controller) {
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
                        "ModuleId": AppInstanceId,
                        "TabId": tabId,
                        "ContentBlockId": ContentBlockId
                    },

                    dictDefaultMessage: "",
                    addRemoveLinks: false,
                    previewsContainer: ".field-" + field.toLowerCase() + " .dropzone-previews",
                    // we need a clickable, because otherwise the entire area is clickable. so i'm just making the preview clickable, as it's not important
                    clickable: ".field-" + field.toLowerCase() + " .invisible-clickable" // " .dropzone-adam"
                };


                var eventHandlers = {
                    'addedfile': function (file) {
                        $timeout(function () {
                            // anything you want can go here and will safely be run on the next digest.
                            scope.$apply(function () { // this must run in a timeout
                                scope.uploading = true;
                            });
                        });
                    },

                    "processing": function (file) {
                        this.options.url = svc.uploadUrl(controller.adam.subFolder);
                    },

                    'success': function (file, response) {
                        if (response.Success) {
                            svc.addFullPath(response); // calculate additional infos
                            scope.$parent.afterUpload(response);
                        } else {
                            alert("Upload failed because: " + response.Error);
                        }
                    },
                    'error': function (file, error, xhr) {
                        alert($translate.instant("Errors.AdamUploadError"));
                    },

                    "queuecomplete": function (file) {
                        if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                            scope.uploading = false;
                            controller.adam.refresh();
                        }
                    }
                };

                // delay building the dropszone till the DOM is ready
                $timeout(function() {
                    var dropzone = new Dropzone(element[0], config);

                    angular.forEach(eventHandlers, function(handler, event) {
                        dropzone.on(event, handler);
                    });

                    scope.processDropzone = function() { dropzone.processQueue(); };
                    scope.resetDropzone = function() { dropzone.removeAllFiles(); };
                    controller.openUpload = function() { dropzone.hiddenFileInput.click(); };

                }, 0);
            }


            function controller() {
                var vm = this;
                vm.adam = {
                    show: false,
                    subFolder: "",
                    refresh: function () { }
                };

            }

        });


})();