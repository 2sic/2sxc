
(function () {
    if (window.$2sxc)
        return;

    window.$2sxc = function (id) {

        if (!$2sxc._data[id])
            $2sxc._data[id] = {};

        // either get the cached controller from previous calls, or create a new one
        var controller = $2sxc._controllers[id] ? $2sxc._controllers[id] : $2sxc._controllers[id] = {
            // <NewIn7>
            serviceScopes: ["app", "app-api", "app-query", "app-content", "eav", "view", "dnn"],
            serviceRoot: $.ServicesFramework(id).getServiceRoot("2sxc"),
            resolveServiceUrl: function resolveServiceUrl(virtualPath) {
            	var scope = virtualPath.split("/")[0].toLowerCase();

            	// stop if it's not one of our special paths
                if (controller.serviceScopes.indexOf(scope) == -1)
                	return virtualPath;

                return controller.serviceRoot + scope + "/" + virtualPath.substring(virtualPath.indexOf("/") + 1);
            },
            // </NewIn7>
            data: {
                // source path defaulting to current page + optional params
                sourceUrl: function (params) {
                    var url = window.location.href;
                    if (url.indexOf("#"))
                        url = url.substr(0, url.indexOf("#"));
                    url += (window.location.href.indexOf("?") != -1 ? "&" : "?") + "mid=" + id + "&standalone=true&popUp=true&type=data";
                    if (typeof params == "string") // text like 'id=7'
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
                                if (data[dataSetName].List !== null) {
                                    controller.data["in"][dataSetName] = data[dataSetName];
                                    controller.data["in"][dataSetName].name = dataSetName;
                                }
                            }

                            if (controller.data["in"].Default)
                                controller.List = controller.data["in"].Default.List;

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
                    if (typeof settings == "string") {
                        var controllerAction = settings.split("/");
                        var controllerName = controllerAction[0];
                        var actionName = controllerAction[1];

                        if (controllerName === "" || actionName === "")
                            alert("Error: controller or action not defined. Will continue with likely errors.");

                        settings = {
                            controller: controllerName,
                            action: actionName,
                            params: params,
                            data: data,
                            preventAutoFail: preventAutoFail
                        };
                    }

                    var defaults = {
                        method: method === null ? "POST" : method,
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
                    return sf.getServiceRoot("2sxc") + "app-api/" + settings.controller + "/" + settings.action + (settings.params === null ? "" : ("?" + $.param(settings.params)));
                }
            },

            // Show a nice error with more infos around 2sxc
            showDetailedHttpError: function showDetailedHttpError(result) {
                if (window.console)
                    console.log(result);

                if (result.status === 404 && result.config && result.config.url && result.config.url.indexOf("/dist/i18n/") > -1) {
                    if (window.console)
                        console.log("just fyi: failed to load language resource; will have to use default");
                    return result;
                }


                // if it's an unspecified 0-error, it's probably not an error but a cancelled request, (happens when closing popups containing angularJS)
                if (result.status === 0)
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


                                if (msgDet && msgDet.indexOf("No action was found") === 0)
                                    if (msgDet.indexOf("that matches the name") > 0)
                                        infoText += "\n\nTip from 2sxc: you probably got the action-name wrong in your JS.";
                                    else if (msgDet.indexOf("that matches the request.") > 0)
                                        infoText += "\n\nTip from 2sxc: Seems like the parameters are the wrong amount or type.";
                                
                                if (msg && msg.indexOf("Controller") === 0 && msg.indexOf("not found") > 0)
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
    $2sxc.metaVersion = "08.01.00";
    $2sxc.beta = {};
    $2sxc._data = {};
    

    // this creates a full-screen iframe-popup and provides a close-command to finish the dialog as needed
    $2sxc.totalPopup = {
        open: function openTotalPopup(url, callback) {
            // count parents to see how high the z-index needs to be
            var z = 1500, p = window;
            while (p !== window.top && z < 1600) {
                z++;
                p = p.parent;
            }

            var wrapper = document.createElement("div");
            wrapper.setAttribute("style", " top: 0;left: 0;width: 100%;height: 100%; position:fixed; z-index:" + z);
            document.body.appendChild(wrapper);

            var ifrm = document.createElement("iframe");
            ifrm.setAttribute("allowtransparency", "true");
            ifrm.setAttribute("style", "top: 0;left: 0;width: 100%;height: 100%;");
            ifrm.setAttribute("src", url);
            wrapper.appendChild(ifrm);
            document.body.className += ' sxc-popup-open';
            $2sxc.totalPopup.frame = ifrm;
            $2sxc.totalPopup.callback = callback;
        },
        close: function closeTotalPopup() {
            if ($2sxc.totalPopup.frame) {
                document.body.className = document.body.className.replace('sxc-popup-open', '');
                var frm = $2sxc.totalPopup.frame;
                frm.parentNode.parentNode.removeChild(frm.parentNode);
                $2sxc.totalPopup.callback();
            }
        },
        closeThis: function closeThis() {
            window.parent.$2sxc.totalPopup.close();
        }
    };

    $2sxc.urlParams = {
        get: function getParameterByName(name) {
            // warning: this method is duplicated in 3 places - keep them in sync. 
            // locations are eav, 2sxc4ng and ui.html
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var searchRx = new RegExp("[\\?&]" + name + "=([^&#]*)", "i");
            var results = searchRx.exec(location.search);

            if (results === null) {
                var hashRx = new RegExp("[#&]" + name + "=([^&#]*)", "i");
                results = hashRx.exec(location.hash);
            }

            // if nothing found, try normal URL because DNN places parameters in /key/value notation
            if (results === null) {
                // Otherwise try parts of the URL
                var matches = window.location.pathname.match(new RegExp("/" + name + "/([^/]+)", "i"));

                // Check if we found anything, if we do find it, we must reverse the results so we get the "last" one in case there are multiple hits
                if (matches !== null && matches.length > 1)
                    results = matches.reverse()[0];
            } else
                results = results[1];

            return results === null ? "" : decodeURIComponent(results.replace(/\+/g, " "));
        },

        require: function getRequiredParameter(name) {
            var found = $2sxc.urlParams.get(name);
            if (found === "") {
                var message = "Required parameter (" + name + ") missing from url - cannot continue";
                alert(message);
                throw message;
            }
            return found;
        }
    };

    // debug state which is needed in various places
    $2sxc.debug = {
        load: ($2sxc.urlParams.get("debug") === "true"),
        renameScript: function toMinOrNotToMin(url) {
            return (!$2sxc.debug.load) ? url : url.replace(".min", "");
        }
    };
})();