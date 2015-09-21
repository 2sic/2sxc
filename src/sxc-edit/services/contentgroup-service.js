/* global angular */

// TODO - merge this into the contentGroup controller - it shouldn't need an own controller for this in Formly


(function () {
	'use strict';

	angular.module('sxcEditContentGroupSvc', []).factory('sxcContentGroupService', function ($http) {

		return {
			get: function (contentGroupGuid) {
				// Returns a typed default value from the string representation
				return $http.get('app/ContentGroup/Get?contentGroupGuid=' + contentGroupGuid);
			}
		};

	});

})();