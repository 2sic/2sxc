
(function () {
	'use strict';

    angular.module('SxcEditContentGroupDnnWrapper', [
        'eavEditEntity',
        'eavLocalization',
        'sxcFieldTemplates',
        'SxcEditTemplates',
        'eavEditEntity'
    ])

	.controller('EditInDnn', function (items, /*entityId, typeName, groupGuid, groupSet, groupIndex, */ $modalInstance) {
		var vm = this;

            vm.editPackageRequest = items;

	    // this is the callback after saving - needed to close everything
		vm.afterSave = function (result) {
		    if (result.status === 200)
		        vm.close();
		    else {
		        alert("Something went wrong - maybe parts worked, maybe not. Sorry :(");
		    }

		};

		vm.close = $modalInstance.close;
	});

})();
