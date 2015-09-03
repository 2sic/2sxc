/* global angular */
(function() {
	'use strict';

	var app = angular.module('sxcEditContentGroup', ['eavEditEntity', 'eavLocalization']);
	app.directive('editContentGroup', function() {
		return {
			templateUrl: '/DesktopModules/ToSIC_SexyContent/SexyContent/EAV/FormlyEditUI/EditContentGroup.html',
			restrict: 'E',
			scope: {
				edit: '=edit'
			},
			controller: 'editContentGroupCtrl',
			controllerAs: 'vm'
		};
	});
	app.controller('editContentGroupCtrl', function($q, $http, $scope) {
		var vm = this;

		// This array holds the entities to edit
		vm.entitiesToEdit = [];

		// Prepare parameters
		var entityId = $scope.edit.entityId;
		var contentTypeName = $scope.edit.contentTypeName;
		var contentGroupGuid = $scope.edit.contentGroupGuid;
		//var mode = $scope.edit.mode;
		var sortOrder = $scope.edit.sortOrder;

		// Edit a content group - first load the contentgroup configuration
		// Then add entities to edit from configuration
		if (contentGroupGuid) {
			$http.get('view/ContentGroup/Get?contentGroupGuid=' + contentGroupGuid).then(function (result) {
				var contentGroup = result.data;

				// Template must be set to edit something
				if(!contentGroup.Template)
					alert('No template defined');

				// Use either default or List types. Reset sortOrder to 0 for List types.
				var editTypes = sortOrder == -1 ? ['ListContent', 'ListPresentation'] : ['Content', 'Presentation'];
				if (sortOrder == -1)
					sortOrder = 0;	

				angular.forEach(editTypes, function (editType, i) {
					var contentTypeName = contentGroup.Template[editType + 'TypeStaticName'];

					var entityToEdit = {
						contentTypeName: contentTypeName,
						entityId: contentGroup[editType][sortOrder],
						editControlTitle: editType
					};

					if (editType.indexOf("Presentation") != -1)
						entityToEdit.isPresentation = true;
					if (entityToEdit.isPresentation && !entityToEdit.entityId)
						entityToEdit.useDefaultValues = true;

					if (contentTypeName)
						vm.entitiesToEdit.push(entityToEdit);
				});

				console.log(vm.entitiesToEdit);
			});
		}
		
		if (entityId) // User wants to edit a single entity
			vm.entitiesToEdit.push({ entityId: entityId, editControlTitle: 'Entity' });
		else if (contentTypeName && !contentGroupGuid) // EntityId not specified, but contentTypeName - user wants to add an item of this type
			vm.entitiesToEdit.push({ contentTypeName: contentTypeName, editControlTitle: 'Entity' });

		vm.registeredControls = [];
		vm.registerEditControl = function(control) {
			vm.registeredControls.push(control);
		};

		vm.isValid = function() {
			var valid = true;
			angular.forEach(vm.registeredControls, function(e, i) {
				if (!e.isValid())
					valid = false;
			});
			return valid;
		};

		vm.save = function() {
			var savePromises = [];
			angular.forEach(vm.registeredControls, function(e, i) {
				savePromises.push(e.save());
			});
			$q.all(savePromises).then(function() {
				alert("All save promises resolved!");
			});
		};
	});

})();
