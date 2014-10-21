// Show Notifications using toaster
pipelineDesigner.factory('uiNotification', ['toaster', function (toaster) {
	'use strict';

	var showNote = function (type, title, body, autoHide) {
		// wrap toaster in ready-Event because notes would't be show if teaster is used before
		angular.element(document).ready(function () {
			toaster.clear();
			toaster.pop(type, title, body, autoHide ? null : 0);
		});
	};

	return {
		clear: function () {
			toaster.clear();
		},
		error: function (title, bodyOrError) {
			var message;
			// test whether bodyOrError is an Error from Web API
			if (bodyOrError && bodyOrError.data && bodyOrError.data.Message) {
				message = bodyOrError.data.Message;
				if (bodyOrError.data.ExceptionMessage)
					message += '\n' + bodyOrError.data.ExceptionMessage;
			} else
				message = bodyOrError;

			showNote('error', title, message);
		},
		note: function (title, body, autoHide) {
			showNote('note', title, body, autoHide);
		},
		success: function (title, body, autoHide) {
			showNote('success', title, body, autoHide);
		},
		wait: function (title) {
			showNote('note', title ? title : 'Please wait ..', "This shouldn't take long", false);
		}
	};
}]);