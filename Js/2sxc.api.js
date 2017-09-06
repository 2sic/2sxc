(function () {
    if (window.$2sxc)
        return;
    var $2sxc = window.$2sxc = getInstance;
    function getInstance(id, cbid) {
        if (typeof id === "object")
            return autoFind(id);
        if (!cbid)
            cbid = id;
        var cacheKey = id + ":" + cbid;
        if ($2sxc._controllers[cacheKey])
            return $2sxc._controllers[cacheKey];
        if (!$2sxc._data[cacheKey])
            $2sxc._data[cacheKey] = {};
        var controller = $2sxc._controllers[cacheKey] = {
            serviceScopes: ["app", "app-sys", "app-api", "app-query", "app-content", "eav", "view", "dnn"],
            serviceRoot: $.ServicesFramework(id).getServiceRoot("2sxc"),
            resolveServiceUrl: function (virtualPath) {
                var scope = virtualPath.split("/")[0].toLowerCase();
                if (controller.serviceScopes.indexOf(scope) === -1)
                    return virtualPath;
                return controller.serviceRoot + scope + "/" + virtualPath.substring(virtualPath.indexOf("/") + 1);
            },
            data: {
                sourceUrl: function (params) {
                    var url = controller.resolveServiceUrl("app-sys/appcontent/GetContentBlockData");
                    if (typeof params == "string")
                        url += "&" + params;
                    return url;
                },
                source: undefined,
                "in": {},
                List: [],
                load: function (source) {
                    if (source && source.List) {
                        return controller.data;
                    }
                    else {
                        if (!source)
                            source = {};
                        if (!source.url)
                            source.url = controller.data.sourceUrl();
                        source.origSuccess = source.success;
                        source.success = function (data) {
                            for (var dataSetName in data) {
                                if (data.hasOwnProperty(dataSetName))
                                    if (data[dataSetName].List !== null) {
                                        controller.data["in"][dataSetName] = data[dataSetName];
                                        controller.data["in"][dataSetName].name = dataSetName;
                                    }
                            }
                            if (controller.data["in"].Default)
                                controller.data.List = controller.data["in"].Default.List;
                            if (source.origSuccess)
                                source.origSuccess(controller.data);
                            controller.isLoaded = true;
                            controller.lastRefresh = new Date();
                            controller.data._triggerLoaded();
                        };
                        source.error = function (request) { alert(request.statusText); };
                        source.preventAutoFail = true;
                        controller.data.source = source;
                        return controller.data.reload();
                    }
                },
                reload: function () {
                    controller.webApi.get(controller.data.source)
                        .then(controller.data.source.success, controller.data.source.error);
                    return controller.data;
                },
                on: function (events, callback) {
                    return $(controller.data).bind("2scLoad", callback)[0]._triggerLoaded();
                },
                _triggerLoaded: function () {
                    return controller.isLoaded
                        ? $(controller.data).trigger("2scLoad", [controller.data])[0]
                        : controller.data;
                },
                one: function (events, callback) {
                    if (!controller.isLoaded)
                        return $(controller.data).one("2scLoad", callback)[0];
                    callback({}, controller.data);
                    return controller.data;
                }
            },
            id: id,
            cbid: cbid,
            cacheKey: cacheKey,
            source: null,
            isLoaded: false,
            lastRefresh: null,
            manage: null,
            isEditMode: function () {
                return controller.manage && controller.manage._isEditMode();
            },
            recreate: function (resetCache) {
                if (resetCache)
                    delete $2sxc._controllers[cacheKey];
                return $2sxc(controller.id, controller.cbid);
            },
            webApi: {
                get: function (s, p, d, paf) { return controller.webApi._action(s, p, d, paf, "GET"); },
                post: function (s, p, d, paf) { return controller.webApi._action(s, p, d, paf, "POST"); },
                "delete": function (s, p, d, paf) { return controller.webApi._action(s, p, d, paf, "DELETE"); },
                put: function (s, p, d, paf) { return controller.webApi._action(s, p, d, paf, "PUT"); },
                _action: function (settings, params, data, preventAutoFail, method) {
                    if (typeof params != "object" && typeof params != "undefined")
                        params = { id: params };
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
                            url: controllerAction.length > 2 ? settings : null,
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
                        dataType: settings.dataType || "json",
                        async: true,
                        data: JSON.stringify(settings.data),
                        contentType: "application/json",
                        url: controller.webApi.getActionUrl(settings),
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("ContentBlockId", cbid);
                            sf.setModuleHeaders(xhr);
                        }
                    });
                    if (!settings.preventAutoFail)
                        promise.fail(controller.showDetailedHttpError);
                    return promise;
                },
                getActionUrl: function (settings) {
                    var sf = $.ServicesFramework(id);
                    var base = (settings.url)
                        ? controller.resolveServiceUrl(settings.url)
                        : sf.getServiceRoot("2sxc") + "app/auto/api/" + settings.controller + "/" + settings.action;
                    return base + (settings.params === null ? "" : ("?" + $.param(settings.params)));
                }
            },
            showDetailedHttpError: function (result) {
                if (window.console)
                    console.log(result);
                if (result.status === 404 &&
                    result.config &&
                    result.config.url &&
                    result.config.url.indexOf("/dist/i18n/") > -1) {
                    if (window.console)
                        console.log("just fyi: failed to load language resource; will have to use default");
                    return result;
                }
                if (result.status === 0 || result.status === -1)
                    return result;
                var infoText = "Had an error talking to the server (status " + result.status + ").";
                var srvResp = result.responseText
                    ? JSON.parse(result.responseText)
                    : result.data;
                if (srvResp) {
                    var msg = srvResp.Message;
                    if (msg)
                        infoText += "\nMessage: " + msg;
                    var msgDet = srvResp.MessageDetail || srvResp.ExceptionMessage;
                    if (msgDet)
                        infoText += "\nDetail: " + msgDet;
                    if (msgDet && msgDet.indexOf("No action was found") === 0)
                        if (msgDet.indexOf("that matches the name") > 0)
                            infoText += "\n\nTip from 2sxc: you probably got the action-name wrong in your JS.";
                        else if (msgDet.indexOf("that matches the request.") > 0)
                            infoText += "\n\nTip from 2sxc: Seems like the parameters are the wrong amount or type.";
                    if (msg && msg.indexOf("Controller") === 0 && msg.indexOf("not found") > 0)
                        infoText +=
                            "\n\nTip from 2sxc: you probably spelled the controller name wrong or forgot to remove the word 'controller' from the call in JS. To call a controller called 'DemoController' only use 'Demo'.";
                }
                infoText += "\n\nif you are an advanced user you can learn more about what went wrong - discover how on 2sxc.org/help?tag=debug";
                alert(infoText);
                return result;
            }
        };
        try {
            controller.manage = null;
            if ($2sxc._manage)
                $2sxc._manage.initInstance(controller);
        }
        catch (e) {
            throw e;
        }
        if ($2sxc._translateInit && controller.manage)
            $2sxc._translateInit(controller.manage);
        return controller;
    }
    ;
    $2sxc._controllers = {};
    $2sxc.sysinfo = {
        version: "09.05.00",
        description: "The 2sxc Controller object - read more about it on 2sxc.org"
    };
    $2sxc.beta = {};
    $2sxc._data = {};
    $2sxc.totalPopup = {
        open: function (url, callback) {
            var z = 10000010, p = window;
            while (p !== window.top && z < 10000100) {
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
            document.body.className += " sxc-popup-open";
            $2sxc.totalPopup.frame = ifrm;
            $2sxc.totalPopup.callback = callback;
        },
        close: function () {
            if ($2sxc.totalPopup.frame) {
                document.body.className = document.body.className.replace("sxc-popup-open", "");
                var frm = $2sxc.totalPopup.frame;
                frm.parentNode.parentNode.removeChild(frm.parentNode);
                $2sxc.totalPopup.callback();
            }
        },
        closeThis: function () {
            window.parent.$2sxc.totalPopup.close();
        },
        frame: undefined,
        callback: undefined
    };
    $2sxc.urlParams = {
        get: function (name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var searchRx = new RegExp("[\\?&]" + name + "=([^&#]*)", "i");
            var results = searchRx.exec(location.search), strResult;
            if (results === null) {
                var hashRx = new RegExp("[#&]" + name + "=([^&#]*)", "i");
                results = hashRx.exec(location.hash);
            }
            if (results === null) {
                var matches = window.location.pathname.match(new RegExp("/" + name + "/([^/]+)", "i"));
                if (matches && matches.length > 1)
                    strResult = matches.reverse()[0];
            }
            else
                strResult = results[1];
            return strResult === null || strResult === undefined ? "" : decodeURIComponent(strResult.replace(/\+/g, " "));
        },
        require: function (name) {
            var found = $2sxc.urlParams.get(name);
            if (found === "") {
                var message = "Required parameter (" + name + ") missing from url - cannot continue";
                alert(message);
                throw message;
            }
            return found;
        }
    };
    function autoFind(domElement) {
        var containerTag = $(domElement).closest(".sc-content-block")[0];
        if (!containerTag)
            return null;
        var iid = containerTag.getAttribute("data-cb-instance"), cbid = containerTag.getAttribute("data-cb-id");
        if (!iid || !cbid)
            return null;
        return $2sxc(iid, cbid);
    }
    ;
    $2sxc.debug = {
        load: ($2sxc.urlParams.get("debug") === "true"),
        uncache: $2sxc.urlParams.get("sxcver")
    };
    $2sxc.parts = {
        getUrl: function (url, preventUnmin) {
            var r = (preventUnmin || !$2sxc.debug.load) ? url : url.replace(".min", "");
            if ($2sxc.debug.uncache && r.indexOf("sxcver") === -1)
                r = r + ((r.indexOf("?") === -1) ? "?" : "&") + "sxcver=" + $2sxc.debug.uncache;
            return r;
        }
    };
})();
//# sourceMappingURL=2sxc.api.js.map