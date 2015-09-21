/* global angular */
(function () {
	'use strict';

	var app = angular.module('SxcEditContentGroupDnnWrapper', ['sxcEditContentGroup']);
	app.controller('editContentGroupDnnWrapperCtrl', function (entityId, typeName, groupGuid, groupIndex) {
		var vm = this;
		// Prepare URL parameters, which are passed to edit directive
		vm.edit = {
			entityId: entityId,
			contentTypeName: typeName,
			contentGroupGuid: groupGuid,
			sortOrder: groupIndex
		};
	});

})();
