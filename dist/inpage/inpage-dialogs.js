
// note: this is code which still uses jQuery etc., so it's not really clean
// because of this we're including it as simple code and not packaging it as a service quite yet...

function processInstallMessage(event, modId) {
    var regExToCheckOrigin = /^(http|https):\/\/((gettingstarted|[a-z]*)\.)?(2sexycontent|2sxc)\.org(\/.*)?$/gi;
    if (!regExToCheckOrigin.test(event.origin)) {
        console.error("can't execute, wrong source domain");
        return;
    }

    // Data is sent as text because IE8 and 9 cannot send objects through postMessage
    var data = JSON.parse(event.data);

    // If message does not belong to this module, return
    if (data.moduleId !== modId)
        return;

    if (data.action === "install") {
        var sf = $.ServicesFramework(modId);

        var packages = data.packages;
        var packagesDisplayNames = "";

        // Loop all packages to install
        for (var i = 0; i < packages.length; i++) {
            packagesDisplayNames += "- " + packages[i].displayName + "\n";
        }

        if (confirm("Do you want to install these packages?\n\n"
            + packagesDisplayNames + "\nThis could take 10 to 60 seconds per package, "
            + "please don't reload the page while it's installing. "
            + "You will see a message once it's done and progess is logged to the JS-console.")) {
            $(".DnnModule-" + modId + " #pnlLoading").show();
            var label = $(".DnnModule-" + modId + " #packageName");

            label.html("...");

            runOneInstallJob(packages, 0, sf, label);
        }

    }
    else if (data.action === "resize")
        resizeIFrame(modId, data.height);
}

function resizeIFrame(modId, height) {
    $(".DnnModule-" + modId + " #frGettingStarted").height(height);
}

function runOneInstallJob(packages, i, sf, label) {
    var currentPackage = packages[i];
    console.log(currentPackage.displayName + "(" + i + ") started");
    label.html(currentPackage.displayName);
    return $.ajax({
        type: "GET",
        dataType: "json",
        async: true,
        url: sf.getServiceRoot('2sxc') + "Installer/" + "InstallPackage",
        data: "packageUrl=" + currentPackage.url,
        beforeSend: sf.setModuleHeaders
    })
    .complete(function (jqXHR, textStatus) {
        console.log(currentPackage.displayName + "(" + i + ") completed");
        if (i + 1 < packages.length) {
            runOneInstallJob(packages, i + 1, sf, label);
        } else {
            alert("Done installing. If you saw no errors, everything worked.");
            window.location.reload();
        }
    })
    .error(function (xhr, result, status) {
        var errorMessage = "Something went wrong while installing '" + currentPackage.displayName + "': " + status;
        if (xhr.responseText && xhr.responseText !== "") {
            var response = $.parseJSON(xhr.responseText);
            if (response.messages)
                errorMessage = errorMessage + " - " + response.messages[0].Message;
            else if (response.Message)
                errorMessage = errorMessage + " - " + response.Message;
        }
        errorMessage += " (you might find more informations about the error in the DNN event log).";
        alert(errorMessage);
    });
}
angular.module('SxcInpageTemplates', []).run(['$templateCache', function($templateCache) {
  'use strict';

  $templateCache.put('template-selector/template-selector.html',
    "<div class=\"dnnFormMessage dnnFormInfo\"><tabset><tab select=\"vm.mode='content'\"><tab-heading><span><i icon=icon-ok></i> C</span></tab-heading></tab><tab select=\"vm.mode='app'\"><tab-heading><span><i icon=icon-remove></i> A</span></tab-heading></tab></tabset><div class=sc-selectors><div ng-show=!vm.isContentApp><select ng-model=vm.appId class=\"sc-selector-app input-lg\" ng-options=\"a.AppId as (a.Name.indexOf('TemplatePicker.') === 0 ? '[+] ' + (a.Name | translate) : a.Name) for a in vm.apps\" ng-disabled=\"vm.dashInfo.hasContent || vm.dashInfo.isList\"><option value=\"\" ng-disabled=\"vm.appId != null\" translate=TemplatePicker.AppPickerDefault></option></select><span><span ng-if=\"vm.showAdvanced && !vm.isContentApp\"><button type=button class=\"btn btn-default btn-square\" ng-if=\"vm.appId != null\" ng-click=vm.appSettings(); title=\"{{ 'Toolbar.App' | translate }}\"><i class=icon-settings></i></button> <button type=button class=\"btn btn-default btn-square\" ng-click=vm.installApp(); title=\"{{ 'TemplatePicker.GetMoreApps' | translate }}\"><i class=icon-plus></i></button> <button type=button class=\"btn btn-default btn-square\" ng-click=vm.manageApps(); title=\"{{ 'Toolbar.Zone' | translate }}\"><i class=icon-manage></i></button></span></span></div><select ng-show=vm.isContentApp ng-model=vm.contentTypeId class=input-lg ng-options=\"c.StaticName as c.Name for c in vm.contentTypes\" ng-disabled=\"vm.dashInfo.hasContent || vm.dashInfo.isList\"><option ng-disabled=\"vm.contentTypeId != ''\" value=\"\" translate=TemplatePicker.ContentTypePickerDefault></option></select><div><select ng-show=\"vm.isContentApp ? vm.contentTypeId != 0 : (vm.savedAppId != null &&  vm.filteredTemplates().length > 1)\" class=input-lg ng-model=vm.templateId ng-options=\"t.TemplateId as t.Name for t in vm.filteredTemplates(vm.contentTypeId)\"></select><button ng-show=\"vm.templateId != null && vm.savedTemplateId != vm.templateId\" class=\"btn btn-primary btn-square\" ng-click=\"vm.persistTemplate(false, false);\" title=\"{{ 'TemplatePicker.Save' | translate }}\" type=button><i class=icon-ok></i></button> <button ng-show=\"vm.undoTemplateId != null\" class=\"btn btn-default btn-square\" ng-click=vm.cancelTemplateChange(); type=button title=\"{{ 'TemplatePicker.' + (vm.isContentApp ? 'Cancel' : 'Close') | translate }}\"><i class=icon-cancel></i></button></div></div><div class=sc-loading ng-show=vm.loading><i class=\"icon-sxc-spinner fa-spin\"></i></div><div style=\"position: relative\" ng-if=vm.showRemoteInstaller><iframe id=frGettingStarted ng-src={{vm.remoteInstallerUrl}} width=100% height=300px></iframe><div class=sc-loading id=pnlLoading style=\"display: none\"><i class=\"icon-sxc-spinner animate-spin\"></i><br><br><span class=sc-loading-label>installing <span id=packageName>.</span></span></div></div></div>"
  );

}]);

(function () {
    var module = angular.module("2sxc.view", [
        "2sxc4ng",
        "pascalprecht.translate",
        "SxcInpageTemplates",
        "EavConfiguration",
        "ui.bootstrap"
    ]);

    module.config(["$translateProvider", "AppInstanceId", "$translatePartialLoaderProvider", "languages", function ($translateProvider, AppInstanceId, $translatePartialLoaderProvider, languages) {
        
        // var globals = $2sxc(AppInstanceId).manage._manageInfo;
        
        // add translation table
        $translateProvider
            .preferredLanguage(languages.currentLanguage.split("-")[0])
            .useSanitizeValueStrategy("escapeParameters")   // this is very important to allow html in the JSON files
            .fallbackLanguage(languages.fallbackLanguage)
            .useLoader("$translatePartialLoader", {
                urlTemplate: languages.i18nRoot + "{part}-{lang}.js"
            })
            .useLoaderCache(true);

        $translatePartialLoaderProvider.addPart("inpage");
    }]);


})();
(function () {
    var module = angular.module("2sxc.view");

    module.factory("moduleApiService", ["$http", function($http) {
        return {

            //saveTemplate: function (templateId, forceCreateContentGroup, newTemplateChooserState) {
            //    return $http.get("View/Module/SaveTemplateId", { params: {
            //        templateId: templateId,
            //        forceCreateContentGroup: forceCreateContentGroup,
            //        newTemplateChooserState: newTemplateChooserState
            //    } });
            //},

            //addItem: function(sortOrder) {
            //    return $http.get("View/Module/AddItem", { params: { sortOrder: sortOrder } });
            //},

            getSelectableApps: function() {
                return $http.get("View/Module/GetSelectableApps");
            },

            setAppId: function(appId) {
                return $http.get("View/Module/SetAppId", { params: { appId: appId } });
            },

            getSelectableContentTypes: function() {
                return $http.get("View/Module/GetSelectableContentTypes");
            },

            getSelectableTemplates: function() {
                return $http.get("View/Module/GetSelectableTemplates");
            },

            //setTemplateChooserState: function(state) {
            //    return $http.get("View/Module/SetTemplateChooserState", { params: { state: state } });
            //},

            //renderTemplate: function(templateId, lang) {
            //    return $http.get("View/Module/RenderTemplate", { params: { templateId: templateId, lang: lang } });
            //},

            //changeOrder: function(sortOrder, destinationSortOrder) {
            //    return $http.get("View/Module/ChangeOrder",
            //    { params: { sortOrder: sortOrder, destinationSortOrder: destinationSortOrder } });
            //},

            //publish: function(part, sortOrder) {
            //    return $http.get("view/module/publish", { params: { part: part, sortOrder: sortOrder } });
            //},

            //removeFromList: function(sortOrder) {
            //    return $http.get("View/Module/RemoveFromList", { params: { sortOrder: sortOrder } });
            //},

            gettingStartedUrl: function() {
                return $http.get("View/Module/RemoteInstallDialogUrl", { params: { dialog: "gettingstarted"} });
            }
        };
    }]);

})();
(function () {
    var module = angular.module("2sxc.view");

    module.factory("contentBlockLink", function () {
        return function (vm) {
            // will generate an object necessary to communicate with the outer system
            var iframe = window.frameElement;
            iframe.vm = vm;

            return {
                dialogContainer: iframe,
                window: window.parent, 
                sxc: iframe.sxc,
                contentBlock: iframe.sxc.manage.contentBlock,
                getManageInfo: iframe.getManageInfo,
                dashInfo: iframe.getAdditionalDashboardConfig,
            };
        };
    });

    module.controller("TemplateSelectorCtrl", ["$scope", "moduleApiService", "AppInstanceId", "sxc", "$filter", "$q", "$window", "$translate", "$sce", "contentBlockLink", function ($scope, /* $attrs, */ moduleApiService, AppInstanceId, sxc, $filter, $q, $window, $translate, $sce, contentBlockLink) {
        //#region constants
        var cViewWithoutContent = "_LayoutElement"; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty
        //#endregion

        var realScope = $scope;
        var svc = moduleApiService;
        var importCommand = null; // todo! // $attrs.importappdialog; // note: ugly dependency, should find a way to remove

        //#region vm and basic values / variables attached to vm
        var vm = this;
        vm.apps = [];
        vm.contentTypes = [];
        vm.templates = [];
        var wrapper = contentBlockLink(vm);

        var di = vm.dashInfo = wrapper.dashInfo();
        vm.isContentApp = di.isContent;
        vm.showAdvanced = di.user.canDesign;
        vm.templateId = di.templateId;
        vm.undoTemplateId = di.templateId;
        vm.contentTypeId = di.contentTypeId;
        // (vm.manageInfo.contentTypeId === "" && vm.manageInfo.templateId !== null)
            //? cViewWithoutContent // has template but no content, use placeholder
            //: di.contentTypeId;// vm.manageInfo.contentTypeId;
        vm.undoContentTypeId = vm.contentTypeId;

        vm.appId = vm.dashInfo.appId;
        vm.savedAppId = vm.dashInfo.appId;


        vm.showRemoteInstaller = false;
        vm.remoteInstallerUrl = "";

        vm.loading = 0;
        //#endregion


        vm.filteredTemplates = function (contentTypeId) {
            // Don't filter on App - so just return all
            if (!vm.isContentApp)
                return vm.templates;

            var condition = { ContentTypeStaticName: (contentTypeId === cViewWithoutContent) ? "" : contentTypeId };
            return $filter("filter")(vm.templates, condition, true);
        };


        vm.reloadTemplates = function() {

            vm.loading++;
            var getContentTypes = svc.getSelectableContentTypes();
            var getTemplates = svc.getSelectableTemplates();

            return $q.all([getContentTypes, getTemplates])
                .then(function(res) {
                    vm.contentTypes = res[0].data || [];
                    vm.templates = res[1].data || [];

                    // Add option for no content type if there are templates without
                    if ($filter("filter")(vm.templates, { ContentTypeStaticName: "" }, true).length > 0) {
                        vm.contentTypes.push({ StaticName: cViewWithoutContent, Name: $translate.instant("TemplatePicker.LayoutElement") }); 
                        vm.contentTypes = $filter("orderBy")(vm.contentTypes, "Name");
                    }

                    vm.loading--;
                });
        };

        realScope.$watch("vm.templateId", function (newTemplateId, oldTemplateId) {
            if (newTemplateId === oldTemplateId)
                return;

            // Content (ajax, don't save the changed value)
            if (vm.isContentApp)
                return vm.renderTemplate(newTemplateId);

            // App
            vm.persistTemplate(false)
                .then(function() { wrapper.window.location.reload(); }); //note: must be in a function, as the reload is a method of the location object
        });

        // Auto-set view-dropdown if content-type changed
        realScope.$watch("vm.contentTypeId", function (newContentTypeId, oldContentTypeId) {
        	if (newContentTypeId == oldContentTypeId)
        		return;
        	// Select first template if contentType changed
        	var firstTemplateId = vm.filteredTemplates(newContentTypeId)[0].TemplateId; 
        	if (vm.templateId !== firstTemplateId && firstTemplateId !== null)
        		vm.templateId = firstTemplateId;
        });

        // Save/reload on app-change or show import-window
        realScope.$watch("vm.appId", function (newAppId, oldAppId) {
            if (newAppId === oldAppId || newAppId === null)
                return;

            svc.setAppId(newAppId)
                .then(function () { wrapper.window.location.reload(); });
        });

        vm.manageApps = function() {    wrapper.sxc.manage.action({ "action": "zone" });    };
        vm.appSettings = function() {   wrapper.sxc.manage.action({ "action": "app" });     };

        // this is the one we must refactor out, as the import-command doesn't exist here
        vm.installApp = function () {
            wrapper.window.location = importCommand; // actually does a dnnModal.show...
            return;
        };

        // Cancel and reset back to original state
        vm.cancelTemplateChange = wrapper.contentBlock.cancelTemplateChange;

        // store the template state to the server, optionally force create of content, and hide the selector
        vm.persistTemplate = wrapper.contentBlock.persistTemplate;
        vm.renderTemplate = wrapper.contentBlock.reload;  // just map to that method


        // Optionally change the show state, then 
        // check if it should be shown and load/show
        vm.show = function (stateChange) {
            // todo 8.4 disabled this, as this info should never be set from here again...
            if (stateChange !== undefined)  // optionally change the show-state
                vm.dashInfo.templateChooserVisible = stateChange;

            if (vm.dashInfo.templateChooserVisible) {
                var promises = [];
                if (vm.appId !== null) // if an app had already been chosen OR the content-app (always chosen)
                    promises.push(vm.reloadTemplates()); 

                // if it's the app-dialog and the app's haven't been loaded yet...
                if (!vm.isContentApp && vm.apps.length === 0)
                    promises.push(vm.loadApps());
                $q.all(promises).then(vm.externalInstaller.showIfConfigIsEmpty);
            }
        };

        // some helpers to show the i-frame and link up the ablity to then install stuff
        vm.externalInstaller = {
            // based on situation, decide if we should show the auto-install IFrame
            showIfConfigIsEmpty: function () {
                var showAutoInstaller = (vm.isContentApp) 
                    ? vm.templates.length === 0 
                    : vm.appCount === 0;

                if (showAutoInstaller)
                    vm.externalInstaller.setup();
            },

            configureCallback: function setupCallback() {
                window.addEventListener("message", function forwardMessage(event) {
                    processInstallMessage(event, AppInstanceId); // this calls an external, non-angular method to handle resizing & installation...
                }, false);
            },

            setup: function() {
                svc.gettingStartedUrl().then(function(result) {
                    vm.externalInstaller.configureCallback();
                    vm.showRemoteInstaller = true;
                    vm.remoteInstallerUrl = $sce.trustAsResourceUrl(result.data);
                    console.log(result.data);
                });
            }
        };

        // todo 8.4 - this should re-load state if re-shown
        vm.toggle = function () {
            if (vm.dashInfo.templateChooserVisible)
                vm.cancelTemplateChange();
            else {
                vm.show(true);
            }
        };

        vm.loadApps = function() {
            return svc.getSelectableApps()
                .then(function(data) {
                    vm.apps = data.data;
                    vm.appCount = data.data.length; // needed in the future to check if it shows getting started
                });
        };

        //#region initialize this

        vm.activate = function() {
            vm.show(true); 
        };

        vm.activate();

        //#endregion

    }]);


})();