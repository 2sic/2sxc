(function() {
    angular.module('2sic-EAV').factory('eavDialogService', [ "eavGlobalConfigurationProvider",
        function(configProvider) {
            return {
                open: function(params) {

                    params = $.extend({
                        url: "",
                        width: 950,
                        height: 600,
                        onClose: function() {}
                    }, params);

                    if (window.top.EavEditDialogs == null)
                        window.top.EavEditDialogs = [];

                    var dialogElement = "<div id='EavNewEditDialog" + window.top.EavEditDialogs.length + "'><iframe style='position:absolute; top:0; right:0; left:0; bottom:0; height:100%; width:100%; border:0;' src='" + params.url + "'></iframe></div>";

                    window.top.jQuery(dialogElement).dialog({
                        autoOpen: true,
                        modal: true,
                        width: params.width,
                        dialogClass: configProvider.dialogClass,
                        height: params.height,
                        close: function(event, ui) {
                            $(this).remove();
                            params.onClose();
                            window.top.EavEditDialogs.pop();
                        }
                    });

                    window.top.EavEditDialogs.push(dialogElement);
                }
            };
        }
    ]);
})();