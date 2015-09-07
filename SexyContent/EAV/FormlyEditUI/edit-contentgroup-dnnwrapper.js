/* global angular */
(function () {
	'use strict';

	var app = angular.module('sxcEditContentGroupDnnWrapper', ['sxcEditContentGroup']);
	app.controller('editContentGroupDnnWrapperCtrl', function () {
		var vm = this;
		// Prepare URL parameters, which are passed to edit directive
		vm.edit = {
			entityId: $2sxc.ng.getParameterByName('entityId'),
			contentTypeName: $2sxc.ng.getParameterByName('contentTypeName'),
			contentGroupGuid: $2sxc.ng.getParameterByName('contentGroupGuid'),
			//mode: $2sxc.ng.getParameterByName('mode'),
			sortOrder: $2sxc.ng.getParameterByName('sortOrder')
		};
	});

})();
