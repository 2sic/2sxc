// EavGlobalConfigurationProvider providers default global values for the EAV angular system
// The ConfigurationProvider in 2sxc is not the same as in the EAV project.

// the following config-stuff is not in angular, because some settings are needed in dialogs which are not built with angularJS yet.
// they are included in the same file for conveniance and to motivate the remaining dialogs to get migrated to AngularJS

/*jshint laxbreak:true */
(function () {
    var runningInDnn = location.href.indexOf("ui.html") === -1;

    // will contain some settings which are filled in different ways
    window.$eavUIConfig = {
        languages: {
            fallbackLanguage: "en" // just for the resources files of eav/2sxc
        }
    };

    if (/* window.jQuery !== undefined && */ runningInDnn) { // in dnn-page there is a jquery, which also allows us to find attributes
        // in jQuery-Mode I have to wait till the document is ready
        $(function() {
            var moduleElement = $(document);
            // var manageInfo = $.parseJSON($(moduleElement.find(".Mod2sxcC, .Mod2sxcappC")[0]).attr("data-2sxc")).manage;
            var manageInfo = $.parseJSON($(moduleElement.find("div[data-2sxc]")[0]).attr("data-2sxc")).manage;
            var lng = window.$eavUIConfig.languages;
            lng.i18nRoot = manageInfo.applicationRoot + "desktopmodules/tosic_sexycontent/dist/i18n/";
            lng.defaultLanguage = manageInfo.langPrimary;
            lng.currentLanguage = manageInfo.lang;
            lng.languages = manageInfo.languages;
       });
    }
    else {
        var lng = window.$eavUIConfig.languages;
        lng.i18nRoot = location.href.substring(0, location.href.indexOf("dist/dnn/ui.html")) + "dist/i18n/";
        lng.defaultLanguage = $2sxc.urlParams.require("langpri");
        lng.currentLanguage = $2sxc.urlParams.require("lang");
        lng.languages = JSON.parse($2sxc.urlParams.require("langs"));
    }

})();

if (window.angular) // needed because the file is also included in older non-angular dialogs
    angular.module("EavConfiguration", [])
        .constant("languages", window.$eavUIConfig.languages)
        .factory("eavConfig", ["$location", "sxc", "portalRoot", "systemRoot", "appRoot", "AppInstanceId", function ($location, sxc, portalRoot, systemRoot, appRoot, AppInstanceId) {

            // var dnnModuleId = $location.search().mid;

            return {
                dialogClass: "dnnFormPopup",

                getUrlPrefix: function (area) {
                    var result = "";
                    if (area === "api") {
                        var url = sxc.resolveServiceUrl("eav/");
                        result = url.substr(0, url.length - 5);
                    }

                    if (area === "system")      result = systemRoot;                    // used to link to JS-stuff and similar
                    if (area === "zone")        result = portalRoot;                    // used to link to the site-root (like an image)
                    if (area === "app")         result = appRoot;                       // used to find the app-root of something inside an app
                    if (area === "dialog")      result = systemRoot + "dnn";            // note: not tested yet
                    if (area === "dialog-page") result = systemRoot + "dnn/ui.html";    // note: not tested yet
                    
                    if (result.endsWith("/")) 
                        result = result.substring(0, result.length - 1);
                    return result;
                },
                pipelineDesigner: {
                    outDataSource: {
                        className: "SexyContentTemplate",
                        in: ["ListContent", "Default"],

                        name: "2sxc Target (View or API)",
                        description: "The template/script which will show this data",
                        visualDesignerData: { Top: 20, Left: 420 }
                    },
                    defaultPipeline: {
                        dataSources: [
                            {
                                entityGuid: "unsaved1",
                                partAssemblyAndType: "ToSic.Eav.DataSources.Caches.ICache, ToSic.Eav.DataSources",
                                visualDesignerData: { Top: 800, Left: 440 }
                            },
                            {
                                entityGuid: "unsaved2",
                                partAssemblyAndType: "ToSic.Eav.DataSources.PublishingFilter, ToSic.Eav.DataSources",
                                visualDesignerData: { Top: 620, Left: 440 }
                            },
                            {
                                entityGuid: "unsaved3",
                                partAssemblyAndType: "ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent",
                                visualDesignerData: { Top: 440, Left: 440 }
                            }
                        ],
                        streamWiring: [
                            { From: "unsaved1", Out: "Default", To: "unsaved2", In: "Default" },
                            { From: "unsaved1", Out: "Drafts", To: "unsaved2", In: "Drafts" },
                            { From: "unsaved1", Out: "Published", To: "unsaved2", In: "Published" },
                            { From: "unsaved2", Out: "Default", To: "unsaved3", In: "Default" },
                            { From: "unsaved3", Out: "ListContent", To: "Out", In: "ListContent" },
                            { From: "unsaved3", Out: "Default", To: "Out", In: "Default" }
                        ]
                    },
                    testParameters: "[Module:ModuleID]=" + AppInstanceId // dnnModuleId 
                },
                metadataOfEntity: 4,
                metadataOfAttribute: 2,

                contentType: {
                    defaultScope: "2SexyContent",
                    template: "2SexyContent-Template"
                }

            };
        }]);