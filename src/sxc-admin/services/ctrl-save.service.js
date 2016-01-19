//angular.module("SxcServices")
//    .factory("ctrlS", function($window) {
//        var save = {
//            _event: null,

//            bind: function bind(action) {
//                save._event = window.addEventListener("keydown", function(e) {
//                    if (e.keyCode === 83 && (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey)) {
//                        e.preventDefault();
//                        action();
//                    }
//                }, false);

//            },

//            unbind: function unbind() {
//                window.removeEventListener(save._event);
//            }
//        };

//        return save;
//    });