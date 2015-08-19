/* global angular */
(function() {
	'use strict';

	var outerApp = angular.module('testModule', ['eavEditEntity']);
	outerApp.controller('outerAppController', function($q) {
		var vm = this;

		// This array holds the entities to edit
		vm.entitiesToEdit = [];

		// Prepare URL parameters
		var entityId = $2sxc.ng.getParameterByName('entityId');
		var contentTypeName = $2sxc.ng.getParameterByName('contentTypeName');
		var contentGroupId = $2sxc.ng.getParameterByName('contentGroupId');
		var mode = $2sxc.ng.getParameterByName('mode');
		var sortOrder = $2sxc.ng.getParameterByName('sortOrder');


		// A content group is defined - first load the contentgroup configuration
		if (contentGroupId) {
			vm.entitiesToEdit.push({ contentTypeName: 'Test', entityId: '3942' });
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
