
(function () {
	$2sxc._lib = {
		extend:
            function extend() { // same as angular.extend or jquery.extend, but without that additional dependency
            	for (var i = 1; i < arguments.length; i++)
            		for (var key in arguments[i])
            			if (arguments[i].hasOwnProperty(key))
            				arguments[0][key] = arguments[i][key];
            	return arguments[0];
            }
	};
})();
