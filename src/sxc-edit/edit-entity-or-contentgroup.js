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

		// This array holds the entities to edit
		//vm.entitiesToEdit = [];

		//// Edit a content group - first load the contentgroup configuration
		//// Then add entities to edit from configuration
		//if (contentGroupGuid) {
		//	// ToDo: Refactor - move to service and move api
		//	sxcContentGroupService.get(contentGroupGuid).then(function (result) {
		//		var contentGroup = result.data;

		//		// Template must be set to edit something
		//		if(!contentGroup.Template)
		//			alert('No template defined');

		//		console.log(result);

		//		// Use either default or List types. Reset sortOrder to 0 for List types.
		//		var editTypes = sortOrder == -1 ? ['ListContent', 'ListPresentation'] : ['Content', 'Presentation'];
		//		if (sortOrder == -1)
		//			sortOrder = 0;

		//		angular.forEach(editTypes, function (editType, i) {
		//			var contentTypeName = contentGroup.Template[editType + 'TypeStaticName'];

		//			var entityToEdit = {
		//				contentTypeName: contentTypeName,
		//				entityId: contentGroup[editType][sortOrder],
		//				editControlTitle: editType
		//			};

		//			if (editType.indexOf("Presentation") != -1)
		//				entityToEdit.isPresentation = true;
		//			if (entityToEdit.isPresentation && !entityToEdit.entityId)
		//				entityToEdit.useDefaultValues = true;

		//			if (contentTypeName)
		//				vm.entitiesToEdit.push(entityToEdit);
		//		});

		//		console.log(vm.entitiesToEdit);
		//	});
		//}
		
		//if (entityId) // User wants to edit a single entity
		//	vm.entitiesToEdit.push({ entityId: entityId, editControlTitle: 'Entity' });
		//else if (contentTypeName && !contentGroupGuid) // EntityId not specified, but contentTypeName - user wants to add an item of this type
		//	vm.entitiesToEdit.push({ contentTypeName: contentTypeName, editControlTitle: 'Entity' });

		//vm.registeredControls = [];
		//vm.registerEditControl = function(control) {
		//	vm.registeredControls.push(control);
		//};

		//vm.isValid = function() {
		//	var valid = true;
		//	angular.forEach(vm.registeredControls, function(e, i) {
		//		if (!e.isValid())
		//			valid = false;
		//	});
		//	return valid;
		//};

		//vm.save = function() {
		//	var savePromises = [];
		//	angular.forEach(vm.registeredControls, function(e, i) {
		//		savePromises.push(e.save());
		//	});
		//	$q.all(savePromises).then(function() {
		//		alert("All save promises resolved!");
		//	});
		//};
	});

})();
