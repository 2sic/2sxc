angular.module('eavDialogService', []).factory('eavDialogService', ["eavGlobalConfigurationProvider",
    function (configProvider) {
    	return {
    		open: function (params) {

    			params = $.extend({
    				url: "",
    				width: 950,
    				height: 600,
    				onClose: function () { },
    				title: null
    			}, params);

    			// ToDo: Should be here, not in ViewEdit.js
    			//if (window.top.EavEditDialogs == null)
    			//    window.top.EavEditDialogs = [];

    			var dialogElement;
    			if (params.url)
    				dialogElement = '<div id="EavNewEditDialog"' + window.top.EavEditDialogs.length + '"><iframe style="position:absolute; top:0; right:0; left:0; bottom:0; height:100%; width:100%; border:0" src="' + params.url + '"></iframe></div>';
    			else if (params.content)
    				dialogElement = params.content;
    			else
    				dialogElement = '<div>no url and no content specified</div>';

    			window.top.jQuery(dialogElement).dialog({
    				title: params.title,
    				autoOpen: true,
    				modal: true,
    				width: params.width,
    				dialogClass: configProvider.dialogClass,
    				height: params.height,
    				close: function (event, ui) {
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