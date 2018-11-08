(function () {

    class simpleInput {

        constructor(name, host, options) {
            this.name = name;
            this.host = host;
            this.options = options;
        }

        initialize(host, options) {
            this.host = host;
            // this.options = somethingWithCallbacks.options;
            console.log('myComponent initialize', this.host);
            this.host.update('update value');
        }

        render(container) {
            container.innerHTML = '<input id="myfield" type="text">';

            // container.getElementsByTagName('input').onchange(this.changeCheck);
            console.log('myComponent render', container);

            var elements = container.getElementsByTagName('input');
            console.log('elem', container.getElementsByTagName('input'));

            elements[0].addEventListener('change', () => {
                this.changeCheck(event, elements[0].value);
            })
        }

        changeCheck(event, value) {
            console.log('changeCheck event', event);
            console.log('changeCheck value', value);
            // do validity checks
            var ok = value.length > 7;
            if (ok)
                this.host.update(value);
        }

        externalChange(container, newValue) {
            var elements = container.getElementsByTagName('input');

            if (elements[0].value !== newValue)
                elements[0].value = newValue;
        }
    }

    function externalComponentFactory(name) {
        console.log('simpeInput');
        return new simpleInput(name, null, null);
    }

    window.addOn.register(externalComponentFactory('simple-input'));
})();