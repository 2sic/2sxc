/* global angular */
(function () {
	'use strict';

	var app = angular.module('SxcEditContentGroupDnnWrapper', ['sxcEditContentGroup']);
	app.controller('editContentGroupDnnWrapperCtrl', function (entityId, typeName, groupGuid, groupSet, groupIndex, $modalInstance) {
		var vm = this;
		// Prepare URL parameters, which are passed to edit directive
		vm.edit = {
			entityId: entityId,
			typeName: typeName,
			groupGuid: groupGuid,
			groupIndex: groupIndex,
            groupSet: groupSet
		};

		vm.close = $modalInstance.close;
	});

})();
