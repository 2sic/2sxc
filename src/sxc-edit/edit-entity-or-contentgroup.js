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
	    var typeName = $scope.edit.typeName;
	    var groupGuid = $scope.edit.groupGuid;
	    //var mode = $scope.edit.mode;
	    var groupIndex = $scope.edit.groupIndex;

	    if (groupGuid) {
	        vm.editPackageRequest = {
	            type: 'group',
	            groupGuid: groupGuid,
                groupSet: ['content', 'presentation'],
                groupIndex: groupIndex
	        };
	    }
	    else {
	        vm.editPackageRequest = {
	            type: 'entities',
	            entities: [{
	                contentTypeName: typeName,
                    entityId: entityId
	            }]
	        };
	    }

	});

})();
