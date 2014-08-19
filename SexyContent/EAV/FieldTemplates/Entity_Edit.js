(function() {
    angular.module('2sic-EAV')
        .controller('EntityEditCtrl', function($scope, eavDialogService, $rootElement) {
            $scope.configuration = {};
            $scope.selectedEntity = "";
            $scope.entityIds = function() {
                return $scope.configuration.SelectedEntities.join(',');
            };
            $scope.AddEntity = function() {
                $scope.configuration.SelectedEntities.push(parseInt($scope.selectedEntity));
                $scope.selectedEntity = "";
            };

            $scope.CreateEntityAllowed = function() { return $scope.configuration.AttributeSetId != null && $scope.configuration.AttributeSetId != 0; };

            $scope.OpenNewEntityDialog = function() {
                var url = $($rootElement).attr("data-newdialogurl") + "&PreventRedirect=true";
                url = url.replace("[AttributeSetId]", $scope.configuration.AttributeSetId);
                eavDialogService.open(url, 950, 600, function() {
                    __doPostBack();
                });
            };
        }).factory('eavDialogService', [
            function() {
                return {
                    open: function (url, width, height, callback) {
                        if (window.top.EavEditDialogs == null)
                            window.top.EavEditDialogs = [];

                        var dialogElement = "<div id='EavNewEditDialog" + window.top.EavEditDialogs.length + "'><iframe style='position:absolute; top:0; right:0; left:0; bottom:0; height:100%; width:100%; border:0;' src='" + url + "'></iframe></div>";

                        window.top.jQuery(dialogElement).dialog({
                            autoOpen: true,
                            modal: true,
                            width: width,
                            height: height,
                            dialogClass: "dnnFormPopup",
                            buttons: {},
                            close: function (event, ui) {
                                $(this).remove();
                                if(callback != null)
                                    callback();

                                window.top.EavEditDialogs.pop();
                            }
                        });

                        window.top.EavEditDialogs.push(dialogElement);
                    }
                };
            }
        ]);

})();