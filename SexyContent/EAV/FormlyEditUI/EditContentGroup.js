/* global angular */
(function() {
	'use strict';

	var outerApp = angular.module('testModule', ['eavEditEntity', 'eavLocalization']);
	outerApp.controller('outerAppController', function($q, $http) {
		var vm = this;

		// This array holds the entities to edit
		vm.entitiesToEdit = [];

		// Prepare URL parameters
		var entityId = $2sxc.ng.getParameterByName('entityId');
		var contentTypeName = $2sxc.ng.getParameterByName('contentTypeName');
		var contentGroupGuid = $2sxc.ng.getParameterByName('contentGroupGuid');
		var mode = $2sxc.ng.getParameterByName('mode');
		var sortOrder = $2sxc.ng.getParameterByName('sortOrder');

		// Edit a content group - first load the contentgroup configuration
		// Then add entities to edit from configuration
		if (contentGroupGuid) {
			$http.get('view/ContentGroup/Get?contentGroupGuid=' + contentGroupGuid).then(function (result) {
				var contentGroup = result.data;

				// Template must be set to edit something
				if(!contentGroup.Template)
					alert('No template defined');

				var editTypes = ['Content', 'Presentation'];
				angular.forEach(editTypes, function (editType, i) {
					var contentTypeName = contentGroup.Template[editType + 'TypeStaticName'];
					
					if (contentTypeName)
						vm.entitiesToEdit.push({ contentTypeName: contentTypeName, entityId: contentGroup[editType][sortOrder], editControlTitle: editType });
				});

				console.log(vm.entitiesToEdit);
			});
		}
		
		if (entityId) // User wants to edit a single entity
			vm.entitiesToEdit.push({ entityId: entityId, editControlTitle: 'Entity' });
		else if (contentTypeName && mode == 'add') // EntityId not specified, but contentTypeName - user wants to add an item of this type
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
