/* global angular */
(function() {
	'use strict';

	var outerApp = angular.module('testModule', ['eavEditEntity']);
	outerApp.controller('outerAppController', function($q) {
		var vm = this;
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