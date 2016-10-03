

angular.module('SourceEditor').component('snippetsLinks', {
    templateUrl: 'source-editor/snippets-links.html',
    /*@ngInject*/
    controller: function () {
        //var vm = this;
    },
    controllerAs: "vm",
    bindings: {
        links: "<"
    }
});