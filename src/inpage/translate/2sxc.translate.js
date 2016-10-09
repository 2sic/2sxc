// provide an official translate API for 2sxc - currently internally using a jQuery library, but this may change
(function () {

    $2sxc.translate = function(key) {
        return $.t(key);
    };
    
})();
