// EavGlobalConfigurationProvider providers default global values for the EAV angular system
// The ConfigurationProvider in 2sxc is not the same as in the EAV project.

(function () {
    angular.module('2sic-EAV')
        .factory('eavGlobalConfigurationProvider', function () {

            // Get needed moduleContext variables from parent node
            var globals = $.parseJSON($("[data-2sxc-globals]").attr("data-2sxc-globals"));

            return {
                apiBaseUrl: globals.ApplicationPath + "DesktopModules/2sxc/API",
                defaultApiParams: {
                    portalId: globals.ModuleContext.PortalId,
                    moduleId: globals.ModuleContext.ModuleId,
                    tabId: globals.ModuleContext.TabId
                },
                dialogClass: "dnnFormPopup"
            };
        });
})();