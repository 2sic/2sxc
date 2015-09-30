/* global angular */
(function() {
	'use strict';

	var app = angular.module('sxcEditContentGroup', ['eavEditEntity', 'eavLocalization', 'sxcFieldTemplates', 'SxcEditTemplates', 'eavEditEntity']);
	app.directive('editContentGroup', function() {
		return {
		    templateUrl: 'edit-entity-or-contentgroup.html',
			restrict: 'E',
			scope: {
				edit: '=edit'
			},
			controller: 'editContentGroupCtrl',
			controllerAs: 'vm'
		};
	});

	app.controller('editContentGroupCtrl', function($q, $scope) {
		var vm = this;

	    // Prepare parameters
	    var entityId = $scope.edit.entityId;
	    var contentTypeName = $scope.edit.contentTypeName;
	    var contentGroupGuid = $scope.edit.contentGroupGuid;
	    //var mode = $scope.edit.mode;
	    var sortOrder = $scope.edit.sortOrder;

	    if (contentGroupGuid) {
	        vm.editPackageRequest = {
	            type: 'group',
	            groupGuid: contentGroupGuid,
                groupSet: ['content'],
                groupIndex: sortOrder
	        };
	    }
	    else {
	        vm.editPackageRequest = {
	            type: 'entities',
	            entities: [{
	                contentTypeName: contentTypeName,
                    entityId: entityId
	            }]
	        };
	    }

	});

})();
