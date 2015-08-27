
(function () {
    if (window.$2sxc)
        return;

    window.$2sxc = function (id) {

        if (!$2sxc._data[id])
            $2sxc._data[id] = {};

        var controller = $2sxc._controllers[id] ? $2sxc._controllers[id] : $2sxc._controllers[id] = {
            // <NewIn7>
            serviceScopes: ['app-api', 'app-query', 'app-content', 'eav', 'view'], // todo7: should probably deprecate "app"
            serviceRoot: $.ServicesFramework(id).getServiceRoot('2sxc'),
            resolveServiceUrl: function resolveServiceUrl(virtualPath) {
            	var scope = virtualPath.split('/')[0].toLowerCase();

            	// stop if it's not one of our special paths
                if (controller.serviceScopes.indexOf(scope) == -1)
                	return virtualPath;

                return controller.serviceRoot + scope + '/' + virtualPath.substring(virtualPath.indexOf('/') + 1);
            },
            // </NewIn7>
            data: {
                // source path defaulting to current page + optional params
                sourceUrl: function (params) {
                    var url = window.location.href;
                    if (url.indexOf("#"))
                        url = url.substr(0, url.indexOf("#"));
                    url += (window.location.href.indexOf("?") != -1 ? "&" : "?") + "mid=" + id + "&standalone=true&popUp=true&type=data";
                    if (typeof params == 'string') // text like 'id=7'
                        url += "&" + params;
                    return url;
                },

                // in-streams
                "in": {},

                // Will hold the default stream (["in"]["Default"].List
                List: [],

                controller: null,

                // Load data via ajax
                load: function (source) {
                    // If source is already the data, set it
                    if (source && source.List) {
                        controller.data.setData(source);
                        return controller.data;
                    } else {
                        if (!source)
                            source = {};
                        if (!source.url)
                            source.url = controller.data.sourceUrl();
                        source.origSuccess = source.success;
                        source.success = function (data) {

                            for (var dataSetName in data) {
                                if (data[dataSetName].List != null) {
                                    controller.data["in"][dataSetName] = data[dataSetName];
                                    controller.data["in"][dataSetName].name = dataSetName;
                                }
                            }

                            if (controller.data["in"]["Default"])
                                controller.List = controller.data["in"]["Default"].List;

                            if (source.origSuccess)
                                source.origSuccess(controller.data);

                            controller.isLoaded = true;
                            controller.lastRefresh = new Date();
                            controller.data._triggerLoaded();
                        };
                        source.error = function (request) {
                            alert(JSON.parse(request.responseText).error);
                        };
                        controller.data.source = source;
                        return controller.data.reload();
                    }
                },

                reload: function (optionalCallback) {
					if (optionalCallback)
                        controller.data.source.success = optionalCallback;

                    $.ajax(controller.data.source);
                    return controller.data;
                },

                on: function (events, callback) {
                    return $(controller.data).bind("2scLoad", callback)[0]._triggerLoaded();
                },

                _triggerLoaded: function () {
                    return controller.isLoaded ? $(controller.data).trigger("2scLoad", [controller.data])[0] : controller.data;
                },

                one: function (events, callback) {
                    if (!controller.isLoaded)
                        return $(controller.data).one("2scLoad", callback)[0];
                    callback({}, controller.data);
                    return controller.data;
                }
            },

            id: id,
            source: null,
            isLoaded: false,
            lastRefresh: null,
            manage: $2sxc.getManageController ? $2sxc.getManageController(id) : null,
            isEditMode: function () {
                return controller.manage && controller.manage.isEditMode();
            },
            webApi: {
                get: function (controllerAction, params, data, preventAutoFail) {
                    return controller.webApi._action(controllerAction, params, data, preventAutoFail, "GET");
                },
                post: function (controllerAction, params, data, preventAutoFail) {
                    return controller.webApi._action(controllerAction, params, data, preventAutoFail, "POST");
                },
                "delete": function (controllerAction, params, data, preventAutoFail) {
                    return controller.webApi._action(controllerAction, params, data, preventAutoFail, "DELETE");
                },
                put: function (controllerAction, params, data, preventAutoFail) {
                    return controller.webApi._action(controllerAction, params, data, preventAutoFail, "PUT");
                },
                _action: function (settings, params, data, preventAutoFail, method) {

                    // Url parameter: autoconvert a single value (instead of object of values) to an id=... parameter
                    if (typeof params != "object" && typeof params != "undefined")
                        params = { id: params };

                    // If the first parameter is a string, resolve settings
                    if (typeof settings == 'string') {
                        var controllerAction = settings.split('/');
                        var controllerName = controllerAction[0];
                        var actionName = controllerAction[1];

                        if (controllerName == '' || actionName == '')
                            alert('Error: controller or action not defined. Will continue with likely errors.');

                        settings = {
                            controller: controllerName,
                            action: actionName,
                            params: params,
                            data: data,
                            preventAutoFail: preventAutoFail
                        };
                    }

                    var defaults = {
                        method: method == null ? 'POST' : method,
                        params: null,
                        preventAutoFail: false
                    };
                    settings = $.extend({}, defaults, settings);
                    var sf = $.ServicesFramework(id);

                    var promise = $.ajax({
                        type: settings.method,
                        dataType: "json",
                        async: true,
                        data: JSON.stringify(settings.data),
                        contentType: "application/json",
                        url: controller.webApi.getActionUrl(settings),
                        beforeSend: sf.setModuleHeaders
                    });

                    if (!settings.preventAutoFail)
                        promise.fail(controller.showDetailedHttpError);

                    return promise;
                },
                getActionUrl: function (settings) {
                    var sf = $.ServicesFramework(id);
                    return sf.getServiceRoot('2sxc') + "app-api/" + settings.controller + "/" + settings.action + (settings.params == null ? "" : ("?" + $.param(settings.params)));
                }
            },

            // Show a nice error with more infos around 2sxc
            showDetailedHttpError: function showDetailedHttpError(result) {
                            if (window.console)
                                console.log(result);

                // if it's an unspecified 0-error, it's probably not an error but a cancelled request, (happens when closing popups containing angularJS)
                if (result.status == 0)
                    return;

                            // let's try to show good messages in most cases
                            var infoText = "Had an error talking to the server (status " + result.status + ").";
                var srvResp = result.responseText ?
                    JSON.parse(result.responseText) // for jquery ajax errors
                    : result.data;                  // for angular $http
                            if (srvResp) {
                                var msg = srvResp.Message;
                                if (msg) infoText += "\n\nMessage: " + msg;
                                var msgDet = srvResp.MessageDetail;
                                if (msgDet) infoText += "\n\nDetail: " + msgDet;


                                if (msgDet && msgDet.indexOf("No action was found") == 0)
                                    if (msgDet.indexOf("that matches the name") > 0)
                                        infoText += "\n\nTip from 2sxc: you probably got the action-name wrong in your JS.";
                                    else if (msgDet.indexOf("that matches the request.") > 0)
                                        infoText += "\n\nTip from 2sxc: Seems like the parameters are the wrong amount or type.";

                                if (msg.indexOf("Controller") == 0 && msg.indexOf("not found") > 0)
                                    infoText += "\n\nTip from 2sxc: you probably spelled the controller name wrong or forgot to remove the word 'controller' from the call in JS. To call a controller called 'DemoController' only use 'Demo'.";

                            }
                            infoText += "\n\nFor further debugging view the JS-console or use fiddler. ";
                            alert(infoText);
            }
        };

        // Make sure back-reference to controller is set
        controller.data.controller = controller;

        return controller;
    };

    $2sxc._controllers = {};
    $2sxc.metaName = "The 2sxc Controller object";
    $2sxc.metaVersion = "07.00.03";
    $2sxc.beta = {};
    $2sxc._data = {};
    
    // New helper method - it's needed to get 
    //$2sxc.getParamFromUrl = function getParamFromUrl(paramName) {
    //    var result = null;
    //    // First, try to get it from URL-Query
    //    var query = window.location.search.substring(1);
    //    var vars = query.split("&");
    //    for (var i = 0; i < vars.length; i++) {
    //        var pair = vars[i].split("=");
    //        if (pair[0] == paramName) { result = pair[1]; }
    //    }

    //    if (result)
    //        return result;

    //    // Otherwise try parts of the URL
    //    var matches = window.location.pathname.match("/" + paramName + "/([^/]+)", 'i');

    //    // Check if we found anything
    //    if (matches != null && matches.length > 1) {
    //        matches = matches.reverse(); // must reverse, because it may have found more than one and then we want the one at the end of the URL
    //        result = matches[0];
    //    }

    //    if (result)
    //        return result;

    //}
})();