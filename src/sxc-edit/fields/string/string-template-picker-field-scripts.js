// This isn't fully nice, but it's the code used by the template picker elements
// it's outsourced here, to ensure that code revisions are clear and api consistent
// because the previous version had the code inside the field-config
// and was highly dependent on the angular/formly API, which changes a bit from time to time
// this makes it easier to keep it in sync

// I'll have to think of a better way to provide/inject this in the future...

// must find out if we're in App or Content before we do more...

(function () {
    var cs = {
        init: function(context) {
            cs.context = context;

            // get angular injector to get a service
            var inj = context.$injector;// angular.element(document).find("div").injector();

            var appDialogConfigSvc = inj.get("appDialogConfigSvc");

            appDialogConfigSvc.getDialogSettings().then(function(result) {
                var config = result.data;
                // if this is a content-app, disable two fields
                if (config.IsContent) {
                    //disable current field
                    context.formVm.formFields[context.field.SortOrder].templateOptions.disabled = true;
                    // do the same for the data-field (field 20)
                    context.formVm.formFields[20].templateOptions.disabled = true;
                }
            });

        }
    };

    window["2sxc-template-picker-custom-script-for-name-field"] = cs;
})();