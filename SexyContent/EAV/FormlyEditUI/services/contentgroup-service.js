/* global angular */
(function () {
	'use strict';

	angular.module('sxcEditContentGroup').factory('sxcContentGroupService', function ($http) {

		return {
			get: function (contentGroupGuid) {
				// Returns a typed default value from the string representation
				return $http.get('app/ContentGroup/Get?contentGroupGuid=' + contentGroupGuid);
			}
		};

	});

})();