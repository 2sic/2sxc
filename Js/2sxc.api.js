/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ToSic_Sxc_Controller__ = __webpack_require__(1);

if (!window.$2sxc)
    window.$2sxc = Object(__WEBPACK_IMPORTED_MODULE_0__ToSic_Sxc_Controller__["a" /* buildSxcController */])();


/***/ }),
/* 1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (immutable) */ __webpack_exports__["a"] = buildSxcController;
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ToSic_Sxc_Instance__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__ToSic_Sxc_TotalPopup__ = __webpack_require__(5);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__ToSic_Sxc_Url__ = __webpack_require__(6);



function SxcController(id, cbid) {
    var $2sxc = window.$2sxc;
    if (!$2sxc._controllers)
        throw new Error("$2sxc not initialized yet");
    if (typeof id === "object") {
        var idTuple = autoFind(id);
        id = idTuple[0];
        cbid = idTuple[1];
    }
    if (!cbid)
        cbid = id;
    var cacheKey = id + ":" + cbid;
    if ($2sxc._controllers[cacheKey])
        return $2sxc._controllers[cacheKey];
    if (!$2sxc._data[cacheKey])
        $2sxc._data[cacheKey] = {};
    return ($2sxc._controllers[cacheKey]
        = new __WEBPACK_IMPORTED_MODULE_0__ToSic_Sxc_Instance__["a" /* SxcInstanceWithInternals */](id, cbid, cacheKey, $2sxc, $.ServicesFramework));
}
function buildSxcController() {
    var urlManager = new __WEBPACK_IMPORTED_MODULE_2__ToSic_Sxc_Url__["a" /* UrlParamManager */]();
    var debug = {
        load: (urlManager.get("debug") === "true"),
        uncache: urlManager.get("sxcver"),
    };
    var addOn = {
        _controllers: {},
        sysinfo: {
            version: "09.05.02",
            description: "The 2sxc Controller object - read more about it on 2sxc.org",
        },
        beta: {},
        _data: {},
        totalPopup: new __WEBPACK_IMPORTED_MODULE_1__ToSic_Sxc_TotalPopup__["a" /* TotalPopup */](),
        urlParams: urlManager,
        debug: debug,
        parts: {
            getUrl: function (url, preventUnmin) {
                var r = (preventUnmin || !debug.load) ? url : url.replace(".min", "");
                if (debug.uncache && r.indexOf("sxcver") === -1)
                    r = r + ((r.indexOf("?") === -1) ? "?" : "&") + "sxcver=" + debug.uncache;
                return r;
            },
        },
    };
    for (var property in addOn)
        if (addOn.hasOwnProperty(property))
            SxcController[property] = addOn[property];
    return SxcController;
}
function autoFind(domElement) {
    var containerTag = $(domElement).closest(".sc-content-block")[0];
    if (!containerTag)
        return null;
    var iid = containerTag.getAttribute("data-cb-instance");
    var cbid = containerTag.getAttribute("data-cb-id");
    if (!iid || !cbid)
        return null;
    return [iid, cbid];
}


/***/ }),
/* 2 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* unused harmony export SxcInstance */
/* unused harmony export SxcInstanceWithEditing */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SxcInstanceWithInternals; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ToSic_Sxc_Data__ = __webpack_require__(3);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__ToSic_Sxc_WebApi__ = __webpack_require__(4);
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();


var SxcInstance = (function () {
    function SxcInstance(id, cbid, dnnSf) {
        this.id = id;
        this.cbid = cbid;
        this.dnnSf = dnnSf;
        this.serviceScopes = ["app", "app-sys", "app-api", "app-query", "app-content", "eav", "view", "dnn"];
        this.serviceRoot = dnnSf(id).getServiceRoot("2sxc");
        this.webApi = new __WEBPACK_IMPORTED_MODULE_1__ToSic_Sxc_WebApi__["a" /* SxcWebApiWithInternals */](this, id, cbid);
    }
    SxcInstance.prototype.resolveServiceUrl = function (virtualPath) {
        var scope = virtualPath.split("/")[0].toLowerCase();
        if (this.serviceScopes.indexOf(scope) === -1)
            return virtualPath;
        return this.serviceRoot + scope + "/" + virtualPath.substring(virtualPath.indexOf("/") + 1);
    };
    SxcInstance.prototype.showDetailedHttpError = function (result) {
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
    };
    return SxcInstance;
}());

var SxcInstanceWithEditing = (function (_super) {
    __extends(SxcInstanceWithEditing, _super);
    function SxcInstanceWithEditing(id, cbid, $2sxc, dnnSf) {
        var _this = _super.call(this, id, cbid, dnnSf) || this;
        _this.id = id;
        _this.cbid = cbid;
        _this.$2sxc = $2sxc;
        _this.dnnSf = dnnSf;
        _this.manage = null;
        try {
            if ($2sxc._manage)
                $2sxc._manage.initInstance(_this);
        }
        catch (e) {
            console.error('error in 2sxc - will only log but not throw', e);
        }
        if ($2sxc._translateInit && _this.manage)
            $2sxc._translateInit(_this.manage);
        return _this;
    }
    SxcInstanceWithEditing.prototype.isEditMode = function () {
        return this.manage && this.manage._isEditMode();
    };
    return SxcInstanceWithEditing;
}(SxcInstance));

var SxcInstanceWithInternals = (function (_super) {
    __extends(SxcInstanceWithInternals, _super);
    function SxcInstanceWithInternals(id, cbid, cacheKey, $2sxc, dnnSf) {
        var _this = _super.call(this, id, cbid, $2sxc, dnnSf) || this;
        _this.id = id;
        _this.cbid = cbid;
        _this.cacheKey = cacheKey;
        _this.$2sxc = $2sxc;
        _this.dnnSf = dnnSf;
        _this.source = null;
        _this.isLoaded = false;
        _this.lastRefresh = null;
        _this.data = new __WEBPACK_IMPORTED_MODULE_0__ToSic_Sxc_Data__["a" /* SxcDataWithInternals */](_this);
        return _this;
    }
    SxcInstanceWithInternals.prototype.recreate = function (resetCache) {
        if (resetCache)
            delete this.$2sxc._controllers[this.cacheKey];
        return this.$2sxc(this.id, this.cbid);
    };
    return SxcInstanceWithInternals;
}(SxcInstanceWithEditing));



/***/ }),
/* 3 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SxcDataWithInternals; });
var SxcDataWithInternals = (function () {
    function SxcDataWithInternals(controller) {
        this.controller = controller;
        this.source = undefined;
        this["in"] = {};
        this.List = [];
    }
    SxcDataWithInternals.prototype.sourceUrl = function (params) {
        var url = this.controller.resolveServiceUrl("app-sys/appcontent/GetContentBlockData");
        if (typeof params === "string")
            url += "&" + params;
        return url;
    };
    SxcDataWithInternals.prototype.load = function (source) {
        var _this = this;
        if (source && source.List) {
            return this.controller.data;
        }
        else {
            if (!source)
                source = {};
            if (!source.url)
                source.url = this.controller.data.sourceUrl();
            source.origSuccess = source.success;
            source.success = function (data) {
                for (var dataSetName in data) {
                    if (data.hasOwnProperty(dataSetName))
                        if (data[dataSetName].List !== null) {
                            _this.controller.data.in[dataSetName] = data[dataSetName];
                            _this.controller.data.in[dataSetName].name = dataSetName;
                        }
                }
                if (_this.controller.data.in.Default)
                    _this.List = _this.in.Default.List;
                if (source.origSuccess)
                    source.origSuccess(_this);
                _this.controller.isLoaded = true;
                _this.controller.lastRefresh = new Date();
                _this._triggerLoaded();
            };
            source.error = function (request) { alert(request.statusText); };
            source.preventAutoFail = true;
            this.source = source;
            return this.reload();
        }
    };
    SxcDataWithInternals.prototype.reload = function () {
        this.controller.webApi.get(this.source)
            .then(this.source.success, this.source.error);
        return this;
    };
    SxcDataWithInternals.prototype.on = function (events, callback) {
        return $(this).bind("2scLoad", callback)[0]._triggerLoaded();
    };
    SxcDataWithInternals.prototype._triggerLoaded = function () {
        return this.controller.isLoaded
            ? $(this).trigger("2scLoad", [this])[0]
            : this;
    };
    SxcDataWithInternals.prototype.one = function (events, callback) {
        if (!this.controller.isLoaded)
            return $(this).one("2scLoad", callback)[0];
        callback({}, this);
        return this;
    };
    return SxcDataWithInternals;
}());



/***/ }),
/* 4 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SxcWebApiWithInternals; });
var SxcWebApiWithInternals = (function () {
    function SxcWebApiWithInternals(controller, id, cbid) {
        this.controller = controller;
        this.id = id;
        this.cbid = cbid;
    }
    SxcWebApiWithInternals.prototype.get = function (settingsOrUrl, params, data, preventAutoFail) {
        return this.request(settingsOrUrl, params, data, preventAutoFail, "GET");
    };
    SxcWebApiWithInternals.prototype.post = function (settingsOrUrl, params, data, preventAutoFail) {
        return this.request(settingsOrUrl, params, data, preventAutoFail, "POST");
    };
    SxcWebApiWithInternals.prototype.delete = function (settingsOrUrl, params, data, preventAutoFail) {
        return this.request(settingsOrUrl, params, data, preventAutoFail, "DELETE");
    };
    SxcWebApiWithInternals.prototype.put = function (settingsOrUrl, params, data, preventAutoFail) {
        return this.request(settingsOrUrl, params, data, preventAutoFail, "PUT");
    };
    SxcWebApiWithInternals.prototype.request = function (settings, params, data, preventAutoFail, method) {
        if (typeof params !== "object" && typeof params !== "undefined")
            params = { id: params };
        if (typeof settings === "string") {
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
                preventAutoFail: preventAutoFail,
            };
        }
        var defaults = {
            method: method === null ? "POST" : method,
            params: null,
            preventAutoFail: false,
        };
        settings = $.extend({}, defaults, settings);
        var sf = $.ServicesFramework(this.id);
        var cbid = this.cbid;
        var promise = $.ajax({
            async: true,
            dataType: settings.dataType || "json",
            data: JSON.stringify(settings.data),
            contentType: "application/json",
            type: settings.method,
            url: this.getActionUrl(settings),
            beforeSend: function (xhr) {
                xhr.setRequestHeader("ContentBlockId", cbid);
                sf.setModuleHeaders(xhr);
            },
        });
        if (!settings.preventAutoFail)
            promise.fail(this.controller.showDetailedHttpError);
        return promise;
    };
    SxcWebApiWithInternals.prototype.getActionUrl = function (settings) {
        var sf = $.ServicesFramework(this.id);
        var base = (settings.url)
            ? this.controller.resolveServiceUrl(settings.url)
            : sf.getServiceRoot("2sxc") + "app/auto/api/" + settings.controller + "/" + settings.action;
        return base + (settings.params === null ? "" : ("?" + $.param(settings.params)));
    };
    return SxcWebApiWithInternals;
}());



/***/ }),
/* 5 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TotalPopup; });
var TotalPopup = (function () {
    function TotalPopup() {
        this.frame = undefined;
        this.callback = undefined;
    }
    TotalPopup.prototype.open = function (url, callback) {
        var z = 10000010;
        var p = window;
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
        this.frame = ifrm;
        this.callback = callback;
    };
    TotalPopup.prototype.close = function () {
        if (this.frame) {
            document.body.className = document.body.className.replace("sxc-popup-open", "");
            var frm = this.frame;
            frm.parentNode.parentNode.removeChild(frm.parentNode);
            this.callback();
        }
    };
    TotalPopup.prototype.closeThis = function () {
        window.parent.$2sxc.totalPopup.close();
    };
    return TotalPopup;
}());



/***/ }),
/* 6 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return UrlParamManager; });
var UrlParamManager = (function () {
    function UrlParamManager() {
    }
    UrlParamManager.prototype.get = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var searchRx = new RegExp("[\\?&]" + name + "=([^&#]*)", "i");
        var results = searchRx.exec(location.search);
        var strResult;
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
        return strResult === null || strResult === undefined
            ? ""
            : decodeURIComponent(strResult.replace(/\+/g, " "));
    };
    UrlParamManager.prototype.require = function (name) {
        var found = this.get(name);
        if (found === "") {
            var message = "Required parameter (" + name + ") missing from url - cannot continue";
            alert(message);
            throw message;
        }
        return found;
    };
    return UrlParamManager;
}());



/***/ })
/******/ ]);
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAgMWU0ZTVjY2FkNTliNTk1MGYyYTUiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvMnN4Yy5hcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkNvbnRyb2xsZXIudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkluc3RhbmNlLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5EYXRhLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5XZWJBcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlRvdGFsUG9wdXAudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlVybC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUE7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7OztBQUdBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLGFBQUs7QUFDTDtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBLG1DQUEyQiwwQkFBMEIsRUFBRTtBQUN2RCx5Q0FBaUMsZUFBZTtBQUNoRDtBQUNBO0FBQ0E7O0FBRUE7QUFDQSw4REFBc0QsK0RBQStEOztBQUVySDtBQUNBOztBQUVBO0FBQ0E7Ozs7Ozs7Ozs7QUN4RG9FO0FBS3BFLEVBQUUsQ0FBQyxDQUFDLENBQUMsTUFBTSxDQUFDLEtBQUssQ0FBQztJQUNkLE1BQU0sQ0FBQyxLQUFLLEdBQUcseUZBQWtCLEVBQUUsQ0FBQzs7Ozs7Ozs7Ozs7O0FDVDZEO0FBQ2pEO0FBQ0Y7QUEwQ2xELHVCQUF1QixFQUF3QixFQUFFLElBQWE7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QztRQUN6QyxDQUFDO1FBQUMsSUFBSTtZQUNGLFNBQVMsR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFFM0IsTUFBTSxDQUFDLFNBQVMsS0FBSyxJQUFJLElBQUksU0FBUyxLQUFLLFNBQVM7Y0FDOUMsRUFBRTtjQUNGLGtCQUFrQixDQUFDLFNBQVMsQ0FBQyxPQUFPLENBQUMsS0FBSyxFQUFFLEdBQUcsQ0FBQyxDQUFDLENBQUM7SUFDNUQsQ0FBQztJQUVELGlDQUFPLEdBQVAsVUFBUSxJQUFZO1FBQ2hCLElBQU0sS0FBSyxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsSUFBSSxDQUFDLENBQUM7UUFDN0IsRUFBRSxDQUFDLENBQUMsS0FBSyxLQUFLLEVBQUUsQ0FBQyxDQUFDLENBQUM7WUFDZixJQUFNLE9BQU8sR0FBRyx5QkFBdUIsSUFBSSx5Q0FBc0MsQ0FBQztZQUNsRixLQUFLLENBQUMsT0FBTyxDQUFDLENBQUM7WUFDZixNQUFNLE9BQU8sQ0FBQztRQUNsQixDQUFDO1FBQ0QsTUFBTSxDQUFDLEtBQUssQ0FBQztJQUNqQixDQUFDO0lBQ0wsc0JBQUM7QUFBRCxDQUFDIiwiZmlsZSI6IjJzeGMuYXBpLmpzIiwic291cmNlc0NvbnRlbnQiOlsiIFx0Ly8gVGhlIG1vZHVsZSBjYWNoZVxuIFx0dmFyIGluc3RhbGxlZE1vZHVsZXMgPSB7fTtcblxuIFx0Ly8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbiBcdGZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblxuIFx0XHQvLyBDaGVjayBpZiBtb2R1bGUgaXMgaW4gY2FjaGVcbiBcdFx0aWYoaW5zdGFsbGVkTW9kdWxlc1ttb2R1bGVJZF0pIHtcbiBcdFx0XHRyZXR1cm4gaW5zdGFsbGVkTW9kdWxlc1ttb2R1bGVJZF0uZXhwb3J0cztcbiBcdFx0fVxuIFx0XHQvLyBDcmVhdGUgYSBuZXcgbW9kdWxlIChhbmQgcHV0IGl0IGludG8gdGhlIGNhY2hlKVxuIFx0XHR2YXIgbW9kdWxlID0gaW5zdGFsbGVkTW9kdWxlc1ttb2R1bGVJZF0gPSB7XG4gXHRcdFx0aTogbW9kdWxlSWQsXG4gXHRcdFx0bDogZmFsc2UsXG4gXHRcdFx0ZXhwb3J0czoge31cbiBcdFx0fTtcblxuIFx0XHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cbiBcdFx0bW9kdWxlc1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cbiBcdFx0Ly8gRmxhZyB0aGUgbW9kdWxlIGFzIGxvYWRlZFxuIFx0XHRtb2R1bGUubCA9IHRydWU7XG5cbiBcdFx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcbiBcdFx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xuIFx0fVxuXG5cbiBcdC8vIGV4cG9zZSB0aGUgbW9kdWxlcyBvYmplY3QgKF9fd2VicGFja19tb2R1bGVzX18pXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLm0gPSBtb2R1bGVzO1xuXG4gXHQvLyBleHBvc2UgdGhlIG1vZHVsZSBjYWNoZVxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5jID0gaW5zdGFsbGVkTW9kdWxlcztcblxuIFx0Ly8gZGVmaW5lIGdldHRlciBmdW5jdGlvbiBmb3IgaGFybW9ueSBleHBvcnRzXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLmQgPSBmdW5jdGlvbihleHBvcnRzLCBuYW1lLCBnZXR0ZXIpIHtcbiBcdFx0aWYoIV9fd2VicGFja19yZXF1aXJlX18ubyhleHBvcnRzLCBuYW1lKSkge1xuIFx0XHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBuYW1lLCB7XG4gXHRcdFx0XHRjb25maWd1cmFibGU6IGZhbHNlLFxuIFx0XHRcdFx0ZW51bWVyYWJsZTogdHJ1ZSxcbiBcdFx0XHRcdGdldDogZ2V0dGVyXG4gXHRcdFx0fSk7XG4gXHRcdH1cbiBcdH07XG5cbiBcdC8vIGdldERlZmF1bHRFeHBvcnQgZnVuY3Rpb24gZm9yIGNvbXBhdGliaWxpdHkgd2l0aCBub24taGFybW9ueSBtb2R1bGVzXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLm4gPSBmdW5jdGlvbihtb2R1bGUpIHtcbiBcdFx0dmFyIGdldHRlciA9IG1vZHVsZSAmJiBtb2R1bGUuX19lc01vZHVsZSA/XG4gXHRcdFx0ZnVuY3Rpb24gZ2V0RGVmYXVsdCgpIHsgcmV0dXJuIG1vZHVsZVsnZGVmYXVsdCddOyB9IDpcbiBcdFx0XHRmdW5jdGlvbiBnZXRNb2R1bGVFeHBvcnRzKCkgeyByZXR1cm4gbW9kdWxlOyB9O1xuIFx0XHRfX3dlYnBhY2tfcmVxdWlyZV9fLmQoZ2V0dGVyLCAnYScsIGdldHRlcik7XG4gXHRcdHJldHVybiBnZXR0ZXI7XG4gXHR9O1xuXG4gXHQvLyBPYmplY3QucHJvdG90eXBlLmhhc093blByb3BlcnR5LmNhbGxcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubyA9IGZ1bmN0aW9uKG9iamVjdCwgcHJvcGVydHkpIHsgcmV0dXJuIE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbChvYmplY3QsIHByb3BlcnR5KTsgfTtcblxuIFx0Ly8gX193ZWJwYWNrX3B1YmxpY19wYXRoX19cbiBcdF9fd2VicGFja19yZXF1aXJlX18ucCA9IFwiXCI7XG5cbiBcdC8vIExvYWQgZW50cnkgbW9kdWxlIGFuZCByZXR1cm4gZXhwb3J0c1xuIFx0cmV0dXJuIF9fd2VicGFja19yZXF1aXJlX18oX193ZWJwYWNrX3JlcXVpcmVfXy5zID0gMCk7XG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gd2VicGFjay9ib290c3RyYXAgMWU0ZTVjY2FkNTliNTk1MGYyYTUiLCIvLyB0aGlzIGlzIHRoZSAyc3hjLWphdmFzY3JpcHQgQVBJXHJcbi8vIDJzeGMgd2lsbCBpbmNsdWRlIHRoaXMgYXV0b21hdGljYWxseSB3aGVuIGEgdXNlciBoYXMgZWRpdC1yaWdodHNcclxuLy8gYSB0ZW1wbGF0ZSBkZXZlbG9wZXIgd2lsbCB0eXBpY2FsbHkgdXNlIHRoaXMgdG8gdXNlIHRoZSBkYXRhLWFwaSB0byByZWFkIDJzeGMtZGF0YSBmcm9tIHRoZSBzZXJ2ZXJcclxuLy8gcmVhZCBtb3JlIGFib3V0IHRoaXMgaW4gdGhlIHdpa2k6IGh0dHBzOi8vZ2l0aHViLmNvbS8yc2ljLzJzeGMvd2lraS9KYXZhU2NyaXB0LSUyNDJzeGNcclxuXHJcbmltcG9ydCB7IGJ1aWxkU3hjQ29udHJvbGxlciwgV2luZG93IH0gZnJvbSBcIi4vVG9TaWMuU3hjLkNvbnRyb2xsZXJcIjtcclxuXHJcbi8vIFJlU2hhcnBlciBkaXNhYmxlIEluY29uc2lzdGVudE5hbWluZ1xyXG5kZWNsYXJlIGNvbnN0IHdpbmRvdzogV2luZG93O1xyXG5cclxuaWYgKCF3aW5kb3cuJDJzeGMpIC8vIHByZXZlbnQgZG91YmxlIGV4ZWN1dGlvblxyXG4gICAgd2luZG93LiQyc3hjID0gYnVpbGRTeGNDb250cm9sbGVyKCk7XHJcblxyXG4vLyBSZVNoYXJwZXIgcmVzdG9yZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvMnN4Yy5hcGkudHMiLCIvLyBSZVNoYXJwZXIgZGlzYWJsZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuXHJcbmltcG9ydCB7IFN4Y0luc3RhbmNlLCBTeGNJbnN0YW5jZVdpdGhFZGl0aW5nLCBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHMgfSBmcm9tIFwiLi9Ub1NpYy5TeGMuSW5zdGFuY2VcIjtcclxuaW1wb3J0IHsgVG90YWxQb3B1cCB9IGZyb20gXCIuL1RvU2ljLlN4Yy5Ub3RhbFBvcHVwXCI7XHJcbmltcG9ydCB7IFVybFBhcmFtTWFuYWdlciB9IGZyb20gXCIuL1RvU2ljLlN4Yy5VcmxcIjtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgV2luZG93IHsgJDJzeGM6IFN4Y0NvbnRyb2xsZXIgfCBTeGNDb250cm9sbGVyV2l0aEludGVybmFsczsgfVxyXG5cclxuZGVjbGFyZSBjb25zdCAkOiBhbnk7XHJcbmRlY2xhcmUgY29uc3Qgd2luZG93OiBXaW5kb3c7XHJcblxyXG4vKipcclxuICogVGhpcyBpcyB0aGUgaW50ZXJmYWNlIGZvciB0aGUgbWFpbiAkMnN4YyBvYmplY3Qgb24gdGhlIHdpbmRvd1xyXG4gKi9cclxuZXhwb3J0IGludGVyZmFjZSBTeGNDb250cm9sbGVyIHtcclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhIDJzeGMtaW5zdGFuY2Ugb2YgdGhlIGlkIG9yIGh0bWwtdGFnIHBhc3NlZCBpblxyXG4gICAgICogQHBhcmFtIGlkXHJcbiAgICAgKiBAcGFyYW0gY2JpZFxyXG4gICAgICogQHJldHVybnMge31cclxuICAgICAqL1xyXG4gICAgKGlkOiBudW1iZXIgfCBIVE1MRWxlbWVudCwgY2JpZD86IG51bWJlcik6IFN4Y0luc3RhbmNlIHwgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzLFxyXG5cclxuICAgIC8qKlxyXG4gICAgICogc3lzdGVtIGluZm9ybWF0aW9uLCBtYWlubHkgZm9yIGNoZWNraW5nIHdoaWNoIHZlcnNpb24gb2YgMnN4YyBpcyBydW5uaW5nXHJcbiAgICAgKiBub3RlOiBpdCdzIG5vdCBhbHdheXMgdXBkYXRlZCByZWxpYWJseSwgYnV0IGl0IGhlbHBzIHdoZW4gZGVidWdnaW5nXHJcbiAgICAgKi9cclxuICAgIHN5c2luZm86IHtcclxuICAgICAgICAvKipcclxuICAgICAgICAgKiB0aGUgdmVyc2lvbiB1c2luZyB0aGUgIyMuIyMuIyMgc3ludGF4XHJcbiAgICAgICAgICovXHJcbiAgICAgICAgdmVyc2lvbjogc3RyaW5nLFxyXG5cclxuICAgICAgICAvKipcclxuICAgICAgICAgKiBhIHNob3J0IHRleHQgZGVzY3JpcHRpb24sIGZvciBwZW9wbGUgd2hvIGhhdmUgbm8gaWRlYSB3aGF0IHRoaXMgaXNcclxuICAgICAgICAgKi9cclxuICAgICAgICBkZXNjcmlwdGlvbjogc3RyaW5nLFxyXG4gICAgfTtcclxufVxyXG5cclxuLyoqXHJcbiAqIHJldHVybnMgYSAyc3hjLWluc3RhbmNlIG9mIHRoZSBpZCBvciBodG1sLXRhZyBwYXNzZWQgaW5cclxuICogQHBhcmFtIGlkXHJcbiAqIEBwYXJhbSBjYmlkXHJcbiAqIEByZXR1cm5zIHt9XHJcbiAqL1xyXG5mdW5jdGlvbiBTeGNDb250cm9sbGVyKGlkOiBudW1iZXIgfCBIVE1MRWxlbWVudCwgY2JpZD86IG51bWJlcik6IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB7XHJcbiAgICBjb25zdCAkMnN4YyA9IHdpbmRvdy4kMnN4YyBhcyBTeGNDb250cm9sbGVyV2l0aEludGVybmFscztcclxuICAgIGlmICghJDJzeGMuX2NvbnRyb2xsZXJzKVxyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcihcIiQyc3hjIG5vdCBpbml0aWFsaXplZCB5ZXRcIik7XHJcblxyXG4gICAgLy8gaWYgaXQncyBhIGRvbS1lbGVtZW50LCB1c2UgYXV0by1maW5kXHJcbiAgICBpZiAodHlwZW9mIGlkID09PSBcIm9iamVjdFwiKSB7XHJcbiAgICAgICAgY29uc3QgaWRUdXBsZSA9IGF1dG9GaW5kKGlkKTtcclxuICAgICAgICBpZCA9IGlkVHVwbGVbMF07XHJcbiAgICAgICAgY2JpZCA9IGlkVHVwbGVbMV07XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKCFjYmlkKSBjYmlkID0gaWQ7ICAgICAgICAgICAvLyBpZiBjb250ZW50LWJsb2NrIGlzIHVua25vd24sIHVzZSBpZCBvZiBtb2R1bGVcclxuICAgIGNvbnN0IGNhY2hlS2V5ID0gaWQgKyBcIjpcIiArIGNiaWQ7IC8vIG5ldXRyYWxpemUgdGhlIGlkIGZyb20gb2xkIFwiMzRcIiBmb3JtYXQgdG8gdGhlIG5ldyBcIjM1OjM1M1wiIGZvcm1hdFxyXG5cclxuICAgIC8vIGVpdGhlciBnZXQgdGhlIGNhY2hlZCBjb250cm9sbGVyIGZyb20gcHJldmlvdXMgY2FsbHMsIG9yIGNyZWF0ZSBhIG5ldyBvbmVcclxuICAgIGlmICgkMnN4Yy5fY29udHJvbGxlcnNbY2FjaGVLZXldKSByZXR1cm4gJDJzeGMuX2NvbnRyb2xsZXJzW2NhY2hlS2V5XTtcclxuXHJcbiAgICAvLyBhbHNvIGluaXQgdGhlIGRhdGEtY2FjaGUgaW4gY2FzZSBpdCdzIGV2ZXIgbmVlZGVkXHJcbiAgICBpZiAoISQyc3hjLl9kYXRhW2NhY2hlS2V5XSkgJDJzeGMuX2RhdGFbY2FjaGVLZXldID0ge307XHJcblxyXG4gICAgcmV0dXJuICgkMnN4Yy5fY29udHJvbGxlcnNbY2FjaGVLZXldXHJcbiAgICAgICAgPSBuZXcgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzKGlkLCBjYmlkLCBjYWNoZUtleSwgJDJzeGMsICQuU2VydmljZXNGcmFtZXdvcmspKTtcclxufVxyXG5cclxuLyoqXHJcbiAqIEJ1aWxkIGEgU1hDIENvbnRyb2xsZXIgZm9yIHRoZSBwYWdlLiBTaG91bGQgb25seSBldmVyIGJlIGV4ZWN1dGVkIG9uY2VcclxuICovXHJcbmV4cG9ydCBmdW5jdGlvbiBidWlsZFN4Y0NvbnRyb2xsZXIoKTogU3hjQ29udHJvbGxlciB8IFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzIHtcclxuICAgIGNvbnN0IHVybE1hbmFnZXIgPSBuZXcgVXJsUGFyYW1NYW5hZ2VyKCk7XHJcbiAgICBjb25zdCBkZWJ1ZyA9IHtcclxuICAgICAgICBsb2FkOiAodXJsTWFuYWdlci5nZXQoXCJkZWJ1Z1wiKSA9PT0gXCJ0cnVlXCIpLFxyXG4gICAgICAgIHVuY2FjaGU6IHVybE1hbmFnZXIuZ2V0KFwic3hjdmVyXCIpLFxyXG4gICAgfTtcclxuXHJcbiAgICBjb25zdCBhZGRPbjogYW55ID0ge1xyXG4gICAgICAgIF9jb250cm9sbGVyczoge30gYXMgYW55LFxyXG4gICAgICAgIHN5c2luZm86IHtcclxuICAgICAgICAgICAgdmVyc2lvbjogXCIwOS4wNS4wMlwiLFxyXG4gICAgICAgICAgICBkZXNjcmlwdGlvbjogXCJUaGUgMnN4YyBDb250cm9sbGVyIG9iamVjdCAtIHJlYWQgbW9yZSBhYm91dCBpdCBvbiAyc3hjLm9yZ1wiLFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgYmV0YToge30sXHJcbiAgICAgICAgX2RhdGE6IHt9LFxyXG4gICAgICAgIC8vIHRoaXMgY3JlYXRlcyBhIGZ1bGwtc2NyZWVuIGlmcmFtZS1wb3B1cCBhbmQgcHJvdmlkZXMgYSBjbG9zZS1jb21tYW5kIHRvIGZpbmlzaCB0aGUgZGlhbG9nIGFzIG5lZWRlZFxyXG4gICAgICAgIHRvdGFsUG9wdXA6IG5ldyBUb3RhbFBvcHVwKCksXHJcbiAgICAgICAgdXJsUGFyYW1zOiB1cmxNYW5hZ2VyLFxyXG4gICAgICAgIC8vIG5vdGU6IEkgd291bGQgbGlrZSB0byByZW1vdmUgdGhpcyBmcm9tICQyc3hjLCBidXQgaXQncyBjdXJyZW50bHlcclxuICAgICAgICAvLyB1c2VkIGJvdGggaW4gdGhlIGlucGFnZS1lZGl0IGFuZCBpbiB0aGUgZGlhbG9nc1xyXG4gICAgICAgIC8vIGRlYnVnIHN0YXRlIHdoaWNoIGlzIG5lZWRlZCBpbiB2YXJpb3VzIHBsYWNlc1xyXG4gICAgICAgIGRlYnVnLFxyXG4gICAgICAgIC8vIG1pbmktaGVscGVycyB0byBtYW5hZ2UgMnN4YyBwYXJ0cywgYSBiaXQgbGlrZSBhIGRlcGVuZGVuY3kgbG9hZGVyXHJcbiAgICAgICAgLy8gd2hpY2ggd2lsbCBvcHRpbWl6ZSB0byBsb2FkIG1pbi9tYXggZGVwZW5kaW5nIG9uIGRlYnVnIHN0YXRlXHJcbiAgICAgICAgcGFydHM6IHtcclxuICAgICAgICAgICAgZ2V0VXJsKHVybDogc3RyaW5nLCBwcmV2ZW50VW5taW46IGJvb2xlYW4pIHtcclxuICAgICAgICAgICAgICAgIGxldCByID0gKHByZXZlbnRVbm1pbiB8fCAhZGVidWcubG9hZCkgPyB1cmwgOiB1cmwucmVwbGFjZShcIi5taW5cIiwgXCJcIik7IC8vIHVzZSBtaW4gb3Igbm90XHJcbiAgICAgICAgICAgICAgICBpZiAoZGVidWcudW5jYWNoZSAmJiByLmluZGV4T2YoXCJzeGN2ZXJcIikgPT09IC0xKVxyXG4gICAgICAgICAgICAgICAgICAgIHIgPSByICsgKChyLmluZGV4T2YoXCI/XCIpID09PSAtMSkgPyBcIj9cIiA6IFwiJlwiKSArIFwic3hjdmVyPVwiICsgZGVidWcudW5jYWNoZTtcclxuICAgICAgICAgICAgICAgIHJldHVybiByO1xyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgIH0sXHJcbiAgICB9O1xyXG4gICAgZm9yIChjb25zdCBwcm9wZXJ0eSBpbiBhZGRPbilcclxuICAgICAgICBpZiAoYWRkT24uaGFzT3duUHJvcGVydHkocHJvcGVydHkpKVxyXG4gICAgICAgICAgICBTeGNDb250cm9sbGVyW3Byb3BlcnR5XSA9IGFkZE9uW3Byb3BlcnR5XSBhcyBhbnk7XHJcbiAgICByZXR1cm4gU3hjQ29udHJvbGxlciBhcyBhbnkgYXMgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHM7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGF1dG9GaW5kKGRvbUVsZW1lbnQ6IEhUTUxFbGVtZW50KTogW251bWJlciwgbnVtYmVyXSB7XHJcbiAgICBjb25zdCBjb250YWluZXJUYWcgPSAkKGRvbUVsZW1lbnQpLmNsb3Nlc3QoXCIuc2MtY29udGVudC1ibG9ja1wiKVswXTtcclxuICAgIGlmICghY29udGFpbmVyVGFnKSByZXR1cm4gbnVsbDtcclxuICAgIGNvbnN0IGlpZCA9IGNvbnRhaW5lclRhZy5nZXRBdHRyaWJ1dGUoXCJkYXRhLWNiLWluc3RhbmNlXCIpO1xyXG4gICAgY29uc3QgY2JpZCA9IGNvbnRhaW5lclRhZy5nZXRBdHRyaWJ1dGUoXCJkYXRhLWNiLWlkXCIpO1xyXG4gICAgaWYgKCFpaWQgfHwgIWNiaWQpIHJldHVybiBudWxsO1xyXG4gICAgcmV0dXJuIFtpaWQsIGNiaWRdO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzIGV4dGVuZHMgU3hjQ29udHJvbGxlciB7XHJcbiAgICAoaWQ6IG51bWJlciB8IEhUTUxFbGVtZW50LCBjYmlkPzogbnVtYmVyKTogU3hjSW5zdGFuY2UgfCBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHM7XHJcbiAgICB0b3RhbFBvcHVwOiBUb3RhbFBvcHVwO1xyXG4gICAgdXJsUGFyYW1zOiBVcmxQYXJhbU1hbmFnZXI7XHJcbiAgICBiZXRhOiBhbnk7XHJcbiAgICBfY29udHJvbGxlcnM6IGFueTtcclxuICAgIF9kYXRhOiBhbnk7XHJcbiAgICBfbWFuYWdlOiBhbnk7XHJcbiAgICBfdHJhbnNsYXRlSW5pdDogYW55O1xyXG4gICAgZGVidWc6IGFueTtcclxuICAgIHBhcnRzOiBhbnk7XHJcblxyXG59XHJcblxyXG4vLyBSZVNoYXJwZXIgcmVzdG9yZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkNvbnRyb2xsZXIudHMiLCJcclxuaW1wb3J0IHsgU3hjQ29udHJvbGxlciwgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHMgfSBmcm9tIFwiLi9Ub1NpYy5TeGMuQ29udHJvbGxlclwiO1xyXG5pbXBvcnQgeyBTeGNEYXRhV2l0aEludGVybmFscyB9IGZyb20gXCIuL1RvU2ljLlN4Yy5EYXRhXCI7XHJcbmltcG9ydCB7IFN4Y1dlYkFwaVdpdGhJbnRlcm5hbHMgfSBmcm9tIFwiLi9Ub1NpYy5TeGMuV2ViQXBpXCI7XHJcbi8qKlxyXG4gKiBUaGUgdHlwaWNhbCBzeGMtaW5zdGFuY2Ugb2JqZWN0IGZvciBhIHNwZWNpZmljIEROTiBtb2R1bGUgb3IgY29udGVudC1ibG9ja1xyXG4gKi9cclxuZXhwb3J0IGNsYXNzIFN4Y0luc3RhbmNlIHtcclxuICAgIC8qKlxyXG4gICAgICogaGVscGVycyBmb3IgYWpheCBjYWxsc1xyXG4gICAgICovXHJcbiAgICB3ZWJBcGk6IFN4Y1dlYkFwaVdpdGhJbnRlcm5hbHM7XHJcbiAgICBwcm90ZWN0ZWQgc2VydmljZVJvb3Q6IHN0cmluZztcclxuICAgIHByaXZhdGUgcmVhZG9ubHkgc2VydmljZVNjb3BlcyA9IFtcImFwcFwiLCBcImFwcC1zeXNcIiwgXCJhcHAtYXBpXCIsIFwiYXBwLXF1ZXJ5XCIsIFwiYXBwLWNvbnRlbnRcIiwgXCJlYXZcIiwgXCJ2aWV3XCIsIFwiZG5uXCJdO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICAqIHRoZSBzeGMtaW5zdGFuY2UgSUQsIHdoaWNoIGlzIHVzdWFsbHkgdGhlIEROTiBNb2R1bGUgSWRcclxuICAgICAgICAgKi9cclxuICAgICAgICBwdWJsaWMgaWQ6IG51bWJlcixcclxuXHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgICogY29udGVudC1ibG9jayBJRCwgd2hpY2ggaXMgZWl0aGVyIHRoZSBtb2R1bGUgSUQsIG9yIHRoZSBjb250ZW50LWJsb2NrIGRlZmluaXRpaW9uIGVudGl0eSBJRFxyXG4gICAgICAgICAqIHRoaXMgaXMgYW4gYWR2YW5jZWQgY29uY2VwdCB5b3UgdXN1YWxseSBkb24ndCBjYXJlIGFib3V0LCBvdGhlcndpc2UgeW91IHNob3VsZCByZXNlYXJjaCBpdFxyXG4gICAgICAgICAqL1xyXG4gICAgICAgIHB1YmxpYyBjYmlkOiBudW1iZXIsXHJcbiAgICAgICAgcHJvdGVjdGVkIHJlYWRvbmx5IGRublNmOiBhbnksXHJcbiAgICApIHtcclxuICAgICAgICB0aGlzLnNlcnZpY2VSb290ID0gZG5uU2YoaWQpLmdldFNlcnZpY2VSb290KFwiMnN4Y1wiKTtcclxuICAgICAgICB0aGlzLndlYkFwaSA9IG5ldyBTeGNXZWJBcGlXaXRoSW50ZXJuYWxzKHRoaXMsIGlkLCBjYmlkKTtcclxuICAgIH1cclxuXHJcbiAgICAvKipcclxuICAgICAqIGNvbnZlcnRzIGEgc2hvcnQgYXBpLWNhbGwgcGF0aCBsaWtlIFwiL2FwcC9CbG9nL3F1ZXJ5L3h5elwiIHRvIHRoZSBETk4gZnVsbCBwYXRoXHJcbiAgICAgKiB3aGljaCB2YXJpZXMgZnJvbSBpbnN0YWxsYXRpb24gdG8gaW5zdGFsbGF0aW9uIGxpa2UgXCIvZGVza3RvcG1vZHVsZXMvYXBpLzJzeGMvYXBwLy4uLlwiXHJcbiAgICAgKiBAcGFyYW0gdmlydHVhbFBhdGhcclxuICAgICAqIEByZXR1cm5zIG1hcHBlZCBwYXRoXHJcbiAgICAgKi9cclxuICAgIHJlc29sdmVTZXJ2aWNlVXJsKHZpcnR1YWxQYXRoOiBzdHJpbmcpIHtcclxuICAgICAgICBjb25zdCBzY29wZSA9IHZpcnR1YWxQYXRoLnNwbGl0KFwiL1wiKVswXS50b0xvd2VyQ2FzZSgpO1xyXG5cclxuICAgICAgICAvLyBzdG9wIGlmIGl0J3Mgbm90IG9uZSBvZiBvdXIgc3BlY2lhbCBwYXRoc1xyXG4gICAgICAgIGlmICh0aGlzLnNlcnZpY2VTY29wZXMuaW5kZXhPZihzY29wZSkgPT09IC0xKVxyXG4gICAgICAgICAgICByZXR1cm4gdmlydHVhbFBhdGg7XHJcblxyXG4gICAgICAgIHJldHVybiB0aGlzLnNlcnZpY2VSb290ICsgc2NvcGUgKyBcIi9cIiArIHZpcnR1YWxQYXRoLnN1YnN0cmluZyh2aXJ0dWFsUGF0aC5pbmRleE9mKFwiL1wiKSArIDEpO1xyXG4gICAgfVxyXG5cclxuXHJcbiAgICAvLyBTaG93IGEgbmljZSBlcnJvciB3aXRoIG1vcmUgaW5mb3MgYXJvdW5kIDJzeGNcclxuICAgIHNob3dEZXRhaWxlZEh0dHBFcnJvcihyZXN1bHQ6IGFueSk6IGFueSB7XHJcbiAgICAgICAgaWYgKHdpbmRvdy5jb25zb2xlKVxyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhyZXN1bHQpO1xyXG5cclxuICAgICAgICBpZiAocmVzdWx0LnN0YXR1cyA9PT0gNDA0ICYmXHJcbiAgICAgICAgICAgIHJlc3VsdC5jb25maWcgJiZcclxuICAgICAgICAgICAgcmVzdWx0LmNvbmZpZy51cmwgJiZcclxuICAgICAgICAgICAgcmVzdWx0LmNvbmZpZy51cmwuaW5kZXhPZihcIi9kaXN0L2kxOG4vXCIpID4gLTEpIHtcclxuICAgICAgICAgICAgaWYgKHdpbmRvdy5jb25zb2xlKVxyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coXCJqdXN0IGZ5aTogZmFpbGVkIHRvIGxvYWQgbGFuZ3VhZ2UgcmVzb3VyY2U7IHdpbGwgaGF2ZSB0byB1c2UgZGVmYXVsdFwiKTtcclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcclxuICAgICAgICB9XHJcblxyXG5cclxuICAgICAgICAvLyBpZiBpdCdzIGFuIHVuc3BlY2lmaWVkIDAtZXJyb3IsIGl0J3MgcHJvYmFibHkgbm90IGFuIGVycm9yIGJ1dCBhIGNhbmNlbGxlZCByZXF1ZXN0LFxyXG4gICAgICAgIC8vIChoYXBwZW5zIHdoZW4gY2xvc2luZyBwb3B1cHMgY29udGFpbmluZyBhbmd1bGFySlMpXHJcbiAgICAgICAgaWYgKHJlc3VsdC5zdGF0dXMgPT09IDAgfHwgcmVzdWx0LnN0YXR1cyA9PT0gLTEpXHJcbiAgICAgICAgICAgIHJldHVybiByZXN1bHQ7XHJcblxyXG4gICAgICAgIC8vIGxldCdzIHRyeSB0byBzaG93IGdvb2QgbWVzc2FnZXMgaW4gbW9zdCBjYXNlc1xyXG4gICAgICAgIGxldCBpbmZvVGV4dCA9IFwiSGFkIGFuIGVycm9yIHRhbGtpbmcgdG8gdGhlIHNlcnZlciAoc3RhdHVzIFwiICsgcmVzdWx0LnN0YXR1cyArIFwiKS5cIjtcclxuICAgICAgICBjb25zdCBzcnZSZXNwID0gcmVzdWx0LnJlc3BvbnNlVGV4dFxyXG4gICAgICAgICAgICA/IEpTT04ucGFyc2UocmVzdWx0LnJlc3BvbnNlVGV4dCkgLy8gZm9yIGpxdWVyeSBhamF4IGVycm9yc1xyXG4gICAgICAgICAgICA6IHJlc3VsdC5kYXRhOyAvLyBmb3IgYW5ndWxhciAkaHR0cFxyXG4gICAgICAgIGlmIChzcnZSZXNwKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IG1zZyA9IHNydlJlc3AuTWVzc2FnZTtcclxuICAgICAgICAgICAgaWYgKG1zZykgaW5mb1RleHQgKz0gXCJcXG5NZXNzYWdlOiBcIiArIG1zZztcclxuICAgICAgICAgICAgY29uc3QgbXNnRGV0ID0gc3J2UmVzcC5NZXNzYWdlRGV0YWlsIHx8IHNydlJlc3AuRXhjZXB0aW9uTWVzc2FnZTtcclxuICAgICAgICAgICAgaWYgKG1zZ0RldCkgaW5mb1RleHQgKz0gXCJcXG5EZXRhaWw6IFwiICsgbXNnRGV0O1xyXG5cclxuXHJcbiAgICAgICAgICAgIGlmIChtc2dEZXQgJiYgbXNnRGV0LmluZGV4T2YoXCJObyBhY3Rpb24gd2FzIGZvdW5kXCIpID09PSAwKVxyXG4gICAgICAgICAgICAgICAgaWYgKG1zZ0RldC5pbmRleE9mKFwidGhhdCBtYXRjaGVzIHRoZSBuYW1lXCIpID4gMClcclxuICAgICAgICAgICAgICAgICAgICBpbmZvVGV4dCArPSBcIlxcblxcblRpcCBmcm9tIDJzeGM6IHlvdSBwcm9iYWJseSBnb3QgdGhlIGFjdGlvbi1uYW1lIHdyb25nIGluIHlvdXIgSlMuXCI7XHJcbiAgICAgICAgICAgICAgICBlbHNlIGlmIChtc2dEZXQuaW5kZXhPZihcInRoYXQgbWF0Y2hlcyB0aGUgcmVxdWVzdC5cIikgPiAwKVxyXG4gICAgICAgICAgICAgICAgICAgIGluZm9UZXh0ICs9IFwiXFxuXFxuVGlwIGZyb20gMnN4YzogU2VlbXMgbGlrZSB0aGUgcGFyYW1ldGVycyBhcmUgdGhlIHdyb25nIGFtb3VudCBvciB0eXBlLlwiO1xyXG5cclxuICAgICAgICAgICAgaWYgKG1zZyAmJiBtc2cuaW5kZXhPZihcIkNvbnRyb2xsZXJcIikgPT09IDAgJiYgbXNnLmluZGV4T2YoXCJub3QgZm91bmRcIikgPiAwKVxyXG4gICAgICAgICAgICAgICAgaW5mb1RleHQgKz1cclxuICAgICAgICAgICAgICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bWF4LWxpbmUtbGVuZ3RoXHJcbiAgICAgICAgICAgICAgICAgICAgXCJcXG5cXG5UaXAgZnJvbSAyc3hjOiB5b3UgcHJvYmFibHkgc3BlbGxlZCB0aGUgY29udHJvbGxlciBuYW1lIHdyb25nIG9yIGZvcmdvdCB0byByZW1vdmUgdGhlIHdvcmQgJ2NvbnRyb2xsZXInIGZyb20gdGhlIGNhbGwgaW4gSlMuIFRvIGNhbGwgYSBjb250cm9sbGVyIGNhbGxlZCAnRGVtb0NvbnRyb2xsZXInIG9ubHkgdXNlICdEZW1vJy5cIjtcclxuXHJcbiAgICAgICAgfVxyXG4gICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTptYXgtbGluZS1sZW5ndGhcclxuICAgICAgICBpbmZvVGV4dCArPSBcIlxcblxcbmlmIHlvdSBhcmUgYW4gYWR2YW5jZWQgdXNlciB5b3UgY2FuIGxlYXJuIG1vcmUgYWJvdXQgd2hhdCB3ZW50IHdyb25nIC0gZGlzY292ZXIgaG93IG9uIDJzeGMub3JnL2hlbHA/dGFnPWRlYnVnXCI7XHJcbiAgICAgICAgYWxlcnQoaW5mb1RleHQpO1xyXG5cclxuICAgICAgICByZXR1cm4gcmVzdWx0O1xyXG4gICAgfVxyXG59XHJcblxyXG4vKipcclxuICogRW5oYW5jZWQgc3hjIGluc3RhbmNlIHdpdGggYWRkaXRpb25hbCBlZGl0aW5nIGZ1bmN0aW9uYWxpdHlcclxuICogVXNlIHRoaXMsIGlmIHlvdSBpbnRlbmQgdG8gcnVuIGNvbnRlbnQtbWFuYWdlbWVudCBjb21tYW5kcyBsaWtlIFwiZWRpdFwiIGZyb20geW91ciBKUyBkaXJlY3RseVxyXG4gKi9cclxuZXhwb3J0IGNsYXNzIFN4Y0luc3RhbmNlV2l0aEVkaXRpbmcgZXh0ZW5kcyBTeGNJbnN0YW5jZSB7XHJcbiAgICAvKipcclxuICAgICAqIG1hbmFnZSBvYmplY3Qgd2hpY2ggcHJvdmlkZXMgYWNjZXNzIHRvIGFkZGl0aW9uYWwgY29udGVudC1tYW5hZ2VtZW50IGZlYXR1cmVzXHJcbiAgICAgKiBpdCBvbmx5IGV4aXN0cyBpZiAyc3hjIGlzIGluIGVkaXQgbW9kZSAob3RoZXJ3aXNlIHRoZSBKUyBhcmUgbm90IGluY2x1ZGVkIGZvciB0aGVzZSBmZWF0dXJlcylcclxuICAgICAqL1xyXG4gICAgbWFuYWdlOiBhbnkgPSBudWxsOyAvLyBpbml0aWFsaXplIGNvcnJlY3RseSBsYXRlciBvblxyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIHB1YmxpYyBpZDogbnVtYmVyLFxyXG4gICAgICAgIHB1YmxpYyBjYmlkOiBudW1iZXIsXHJcbi8vIFJlU2hhcnBlciBkaXNhYmxlIG9uY2UgSW5jb25zaXN0ZW50TmFtaW5nXHJcbiAgICAgICAgcHJvdGVjdGVkICQyc3hjOiBTeGNDb250cm9sbGVyV2l0aEludGVybmFscyxcclxuICAgICAgICBwcm90ZWN0ZWQgcmVhZG9ubHkgZG5uU2Y6IGFueSxcclxuICAgICkge1xyXG4gICAgICAgIHN1cGVyKGlkLCBjYmlkLCBkbm5TZik7XHJcblxyXG4gICAgICAgIC8vIGFkZCBtYW5hZ2UgcHJvcGVydHksIGJ1dCBub3Qgd2l0aGluIGluaXRpYWxpemVyLCBiZWNhdXNlIGluc2lkZSB0aGUgbWFuYWdlLWluaXRpYWxpemVyIGl0IG1heSByZWZlcmVuY2UgMnN4YyBhZ2FpblxyXG4gICAgICAgIHRyeSB7IC8vIHNvbWV0aW1lcyB0aGUgbWFuYWdlIGNhbid0IGJlIGJ1aWx0LCBsaWtlIGJlZm9yZSBpbnN0YWxsaW5nXHJcbiAgICAgICAgICAgIGlmICgkMnN4Yy5fbWFuYWdlKSAkMnN4Yy5fbWFuYWdlLmluaXRJbnN0YW5jZSh0aGlzKTtcclxuICAgICAgICB9IGNhdGNoIChlKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUuZXJyb3IoJ2Vycm9yIGluIDJzeGMgLSB3aWxsIG9ubHkgbG9nIGJ1dCBub3QgdGhyb3cnLCBlKTtcclxuICAgICAgICAgICAgLy8gdGhyb3cgZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIHRoaXMgb25seSB3b3JrcyB3aGVuIG1hbmFnZSBleGlzdHMgKG5vdCBpbnN0YWxsaW5nKSBhbmQgdHJhbnNsYXRvciBleGlzdHMgdG9vXHJcbiAgICAgICAgaWYgKCQyc3hjLl90cmFuc2xhdGVJbml0ICYmIHRoaXMubWFuYWdlKSAkMnN4Yy5fdHJhbnNsYXRlSW5pdCh0aGlzLm1hbmFnZSk7ICAgIC8vIGluaXQgdHJhbnNsYXRlLCBub3QgcmVhbGx5IG5pY2UsIGJ1dCBvayBmb3Igbm93XHJcblxyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogY2hlY2tzIGlmIHdlJ3JlIGN1cnJlbnRseSBpbiBlZGl0IG1vZGVcclxuICAgICAqIEByZXR1cm5zIHtib29sZWFufVxyXG4gICAgICovXHJcbiAgICBpc0VkaXRNb2RlKCkge1xyXG4gICAgICAgIHJldHVybiB0aGlzLm1hbmFnZSAmJiB0aGlzLm1hbmFnZS5faXNFZGl0TW9kZSgpO1xyXG4gICAgfVxyXG5cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFN4Y0luc3RhbmNlV2l0aEludGVybmFscyBleHRlbmRzIFN4Y0luc3RhbmNlV2l0aEVkaXRpbmcge1xyXG4gICAgZGF0YTogU3hjRGF0YVdpdGhJbnRlcm5hbHM7XHJcbiAgICBzb3VyY2U6IGFueSA9IG51bGw7XHJcbiAgICBpc0xvYWRlZCA9IGZhbHNlO1xyXG4gICAgbGFzdFJlZnJlc2g6IERhdGUgPSBudWxsO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIHB1YmxpYyBpZDogbnVtYmVyLFxyXG4gICAgICAgIHB1YmxpYyBjYmlkOiBudW1iZXIsXHJcbiAgICAgICAgcHJpdmF0ZSBjYWNoZUtleTogc3RyaW5nLFxyXG4vLyBSZVNoYXJwZXIgZGlzYWJsZSBvbmNlIEluY29uc2lzdGVudE5hbWluZ1xyXG4gICAgICAgIHByb3RlY3RlZCAkMnN4YzogU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHMsXHJcbiAgICAgICAgcHJvdGVjdGVkIHJlYWRvbmx5IGRublNmOiBhbnksXHJcbiAgICApIHtcclxuICAgICAgICBzdXBlcihpZCwgY2JpZCwgJDJzeGMsIGRublNmKTtcclxuICAgICAgICB0aGlzLmRhdGEgPSBuZXcgU3hjRGF0YVdpdGhJbnRlcm5hbHModGhpcyk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVjcmVhdGUocmVzZXRDYWNoZTogYm9vbGVhbik6IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB7XHJcbiAgICAgICAgaWYgKHJlc2V0Q2FjaGUpIGRlbGV0ZSB0aGlzLiQyc3hjLl9jb250cm9sbGVyc1t0aGlzLmNhY2hlS2V5XTsgLy8gY2xlYXIgY2FjaGVcclxuICAgICAgICByZXR1cm4gdGhpcy4kMnN4Yyh0aGlzLmlkLCB0aGlzLmNiaWQpIGFzIGFueSBhcyBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHM7IC8vIGdlbmVyYXRlIG5ld1xyXG4gICAgfVxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5JbnN0YW5jZS50cyIsImltcG9ydCB7IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB9IGZyb20gXCIuL1RvU2ljLlN4Yy5JbnN0YW5jZVwiO1xyXG5cclxuZGVjbGFyZSBjb25zdCAkOiBhbnk7XHJcblxyXG5cclxuZXhwb3J0IGNsYXNzIFN4Y0RhdGFXaXRoSW50ZXJuYWxzIHtcclxuICAgIHNvdXJjZTogYW55ID0gdW5kZWZpbmVkO1xyXG5cclxuICAgIC8vIGluLXN0cmVhbXNcclxuICAgIFwiaW5cIjogYW55ID0ge307XHJcblxyXG4gICAgLy8gd2lsbCBob2xkIHRoZSBkZWZhdWx0IHN0cmVhbSAoW1wiaW5cIl1bXCJEZWZhdWx0XCJdLkxpc3RcclxuICAgIExpc3Q6IGFueSA9IFtdO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIHByaXZhdGUgY29udHJvbGxlcjogU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzLFxyXG4gICAgKSB7XHJcblxyXG4gICAgfVxyXG5cclxuICAgIC8vIHNvdXJjZSBwYXRoIGRlZmF1bHRpbmcgdG8gY3VycmVudCBwYWdlICsgb3B0aW9uYWwgcGFyYW1zXHJcbiAgICBzb3VyY2VVcmwocGFyYW1zPzogc3RyaW5nKTogc3RyaW5nIHtcclxuICAgICAgICBsZXQgdXJsID0gdGhpcy5jb250cm9sbGVyLnJlc29sdmVTZXJ2aWNlVXJsKFwiYXBwLXN5cy9hcHBjb250ZW50L0dldENvbnRlbnRCbG9ja0RhdGFcIik7XHJcbiAgICAgICAgaWYgKHR5cGVvZiBwYXJhbXMgPT09IFwic3RyaW5nXCIpIC8vIHRleHQgbGlrZSAnaWQ9NydcclxuICAgICAgICAgICAgdXJsICs9IFwiJlwiICsgcGFyYW1zO1xyXG4gICAgICAgIHJldHVybiB1cmw7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIC8vIGxvYWQgZGF0YSB2aWEgYWpheFxyXG4gICAgbG9hZChzb3VyY2U/OiBhbnkpIHtcclxuICAgICAgICAvLyBpZiBzb3VyY2UgaXMgYWxyZWFkeSB0aGUgZGF0YSwgc2V0IGl0XHJcbiAgICAgICAgaWYgKHNvdXJjZSAmJiBzb3VyY2UuTGlzdCkge1xyXG4gICAgICAgICAgICAvLyAyMDE3LTA5LTA1IDJkbTogZGlzY292ZXJkIGEgY2FsbCB0byBhbiBpbmV4aXN0aW5nIGZ1bmN0aW9uXHJcbiAgICAgICAgICAgIC8vIHNpbmNlIHRoaXMgaXMgYW4gb2xkIEFQSSB3aGljaCBpcyBiZWluZyBkZXByZWNhdGVkLCBwbGVhc2UgZG9uJ3QgZml4IHVubGVzcyB3ZSBnZXQgYWN0aXZlIGZlZWRiYWNrXHJcbiAgICAgICAgICAgIC8vIGNvbnRyb2xsZXIuZGF0YS5zZXREYXRhKHNvdXJjZSk7XHJcbiAgICAgICAgICAgIHJldHVybiB0aGlzLmNvbnRyb2xsZXIuZGF0YTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBpZiAoIXNvdXJjZSlcclxuICAgICAgICAgICAgICAgIHNvdXJjZSA9IHt9O1xyXG4gICAgICAgICAgICBpZiAoIXNvdXJjZS51cmwpXHJcbiAgICAgICAgICAgICAgICBzb3VyY2UudXJsID0gdGhpcy5jb250cm9sbGVyLmRhdGEuc291cmNlVXJsKCk7XHJcbiAgICAgICAgICAgIHNvdXJjZS5vcmlnU3VjY2VzcyA9IHNvdXJjZS5zdWNjZXNzO1xyXG4gICAgICAgICAgICBzb3VyY2Uuc3VjY2VzcyA9IChkYXRhOiBhbnkpID0+IHtcclxuXHJcbiAgICAgICAgICAgICAgICBmb3IgKGNvbnN0IGRhdGFTZXROYW1lIGluIGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZGF0YS5oYXNPd25Qcm9wZXJ0eShkYXRhU2V0TmFtZSkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChkYXRhW2RhdGFTZXROYW1lXS5MaXN0ICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmNvbnRyb2xsZXIuZGF0YS5pbltkYXRhU2V0TmFtZV0gPSBkYXRhW2RhdGFTZXROYW1lXTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuY29udHJvbGxlci5kYXRhLmluW2RhdGFTZXROYW1lXS5uYW1lID0gZGF0YVNldE5hbWU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5jb250cm9sbGVyLmRhdGEuaW4uRGVmYXVsdClcclxuICAgICAgICAgICAgICAgICAgICAvLyAyMDE3LTA5LTA1IDJkbTogcHJldmlvdXNseSB3cm90ZSBpdCB0byBjb250cm9sbGVyLkxpc3QsIGJ1dCB0aGlzIGlzIGFsbW9zdCBjZXJ0YWlubHkgYSBtaXN0YWtlXHJcbiAgICAgICAgICAgICAgICAgICAgLy8gc2luY2UgaXQncyBhbiBvbGQgQVBJIHdoaWNoIGlzIGJlaW5nIGRlcHJlY2F0ZWQsIHdlIHdvbid0IGZpeCBpdFxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuTGlzdCA9IHRoaXMuaW4uRGVmYXVsdC5MaXN0O1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChzb3VyY2Uub3JpZ1N1Y2Nlc3MpXHJcbiAgICAgICAgICAgICAgICAgICAgc291cmNlLm9yaWdTdWNjZXNzKHRoaXMpO1xyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuY29udHJvbGxlci5pc0xvYWRlZCA9IHRydWU7XHJcbiAgICAgICAgICAgICAgICB0aGlzLmNvbnRyb2xsZXIubGFzdFJlZnJlc2ggPSBuZXcgRGF0ZSgpO1xyXG4gICAgICAgICAgICAgICAgKHRoaXMgYXMgYW55KS5fdHJpZ2dlckxvYWRlZCgpO1xyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICBzb3VyY2UuZXJyb3IgPSAocmVxdWVzdDogYW55KSA9PiB7IGFsZXJ0KHJlcXVlc3Quc3RhdHVzVGV4dCk7IH07XHJcbiAgICAgICAgICAgIHNvdXJjZS5wcmV2ZW50QXV0b0ZhaWwgPSB0cnVlOyAvLyB1c2Ugb3VyIGZhaWwgbWVzc2FnZVxyXG4gICAgICAgICAgICB0aGlzLnNvdXJjZSA9IHNvdXJjZTtcclxuICAgICAgICAgICAgcmV0dXJuIHRoaXMucmVsb2FkKCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHJlbG9hZCgpOiBTeGNEYXRhV2l0aEludGVybmFscyB7XHJcbiAgICAgICAgdGhpcy5jb250cm9sbGVyLndlYkFwaS5nZXQodGhpcy5zb3VyY2UpXHJcbiAgICAgICAgICAgIC50aGVuKHRoaXMuc291cmNlLnN1Y2Nlc3MsIHRoaXMuc291cmNlLmVycm9yKTtcclxuICAgICAgICByZXR1cm4gdGhpcztcclxuICAgIH1cclxuXHJcbiAgICBvbihldmVudHM6IEV2ZW50LCBjYWxsYmFjazogKCkgPT4gdm9pZCk6IFByb21pc2U8YW55PiB7XHJcbiAgICAgICAgcmV0dXJuICQodGhpcykuYmluZChcIjJzY0xvYWRcIiwgY2FsbGJhY2spWzBdLl90cmlnZ2VyTG9hZGVkKCk7XHJcbiAgICB9XHJcblxyXG4gICAgX3RyaWdnZXJMb2FkZWQoKTogUHJvbWlzZTxhbnk+IHtcclxuICAgICAgICByZXR1cm4gdGhpcy5jb250cm9sbGVyLmlzTG9hZGVkXHJcbiAgICAgICAgICAgID8gJCh0aGlzKS50cmlnZ2VyKFwiMnNjTG9hZFwiLCBbdGhpc10pWzBdXHJcbiAgICAgICAgICAgIDogdGhpcztcclxuICAgIH1cclxuXHJcbiAgICBvbmUoZXZlbnRzOiBFdmVudCwgY2FsbGJhY2s6ICh4OiBhbnksIHk6IGFueSkgPT4gdm9pZCk6IFN4Y0RhdGFXaXRoSW50ZXJuYWxzIHtcclxuICAgICAgICBpZiAoIXRoaXMuY29udHJvbGxlci5pc0xvYWRlZClcclxuICAgICAgICAgICAgcmV0dXJuICQodGhpcykub25lKFwiMnNjTG9hZFwiLCBjYWxsYmFjaylbMF07XHJcbiAgICAgICAgY2FsbGJhY2soe30sIHRoaXMpO1xyXG4gICAgICAgIHJldHVybiB0aGlzO1xyXG4gICAgfVxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5EYXRhLnRzIiwiXHJcbmRlY2xhcmUgY29uc3QgJDogYW55O1xyXG5pbXBvcnQgeyBTeGNJbnN0YW5jZSB9IGZyb20gXCIuL1RvU2ljLlN4Yy5JbnN0YW5jZVwiO1xyXG5cclxuLyoqXHJcbiAqIGhlbHBlciBBUEkgdG8gcnVuIGFqYXggLyBSRVNUIGNhbGxzIHRvIHRoZSBzZXJ2ZXJcclxuICogaXQgd2lsbCBlbnN1cmUgdGhhdCB0aGUgaGVhZGVycyBldGMuIGFyZSBzZXQgY29ycmVjdGx5XHJcbiAqIGFuZCB0aGF0IHVybHMgYXJlIHJld3JpdHRlblxyXG4gKi9cclxuZXhwb3J0IGNsYXNzIFN4Y1dlYkFwaVdpdGhJbnRlcm5hbHMge1xyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgcHJpdmF0ZSByZWFkb25seSBjb250cm9sbGVyOiBTeGNJbnN0YW5jZSxcclxuICAgICAgICBwcml2YXRlIHJlYWRvbmx5IGlkOiBudW1iZXIsXHJcbiAgICAgICAgcHJpdmF0ZSByZWFkb25seSBjYmlkOiBudW1iZXIsXHJcbiAgICApIHtcclxuXHJcbiAgICB9XHJcbiAgICAvKipcclxuICAgICAqIHJldHVybnMgYW4gaHR0cC1nZXQgcHJvbWlzZVxyXG4gICAgICogQHBhcmFtIHNldHRpbmdzT3JVcmwgdGhlIHVybCB0byBnZXRcclxuICAgICAqIEBwYXJhbSBwYXJhbXMgalF1ZXJ5IHN0eWxlIGFqYXggcGFyYW1ldGVyc1xyXG4gICAgICogQHBhcmFtIGRhdGEgalF1ZXJ5IHN0eWxlIGRhdGEgZm9yIHBvc3QvcHV0IHJlcXVlc3RzXHJcbiAgICAgKiBAcGFyYW0gcHJldmVudEF1dG9GYWlsXHJcbiAgICAgKiBAcmV0dXJucyB7UHJvbWlzZX0galF1ZXJ5IGFqYXggcHJvbWlzZSBvYmplY3RcclxuICAgICAqL1xyXG4gICAgZ2V0KHNldHRpbmdzT3JVcmw6IHN0cmluZyB8IGFueSwgcGFyYW1zPzogYW55LCBkYXRhPzogYW55LCBwcmV2ZW50QXV0b0ZhaWw/OiBib29sZWFuKTogYW55IHtcclxuICAgICAgICByZXR1cm4gdGhpcy5yZXF1ZXN0KHNldHRpbmdzT3JVcmwsIHBhcmFtcywgZGF0YSwgcHJldmVudEF1dG9GYWlsLCBcIkdFVFwiKTtcclxuICAgIH1cclxuXHJcbiAgICAvKipcclxuICAgICAqIHJldHVybnMgYW4gaHR0cC1nZXQgcHJvbWlzZVxyXG4gICAgICogQHBhcmFtIHNldHRpbmdzT3JVcmwgdGhlIHVybCB0byBnZXRcclxuICAgICAqIEBwYXJhbSBwYXJhbXMgalF1ZXJ5IHN0eWxlIGFqYXggcGFyYW1ldGVyc1xyXG4gICAgICogQHBhcmFtIGRhdGEgalF1ZXJ5IHN0eWxlIGRhdGEgZm9yIHBvc3QvcHV0IHJlcXVlc3RzXHJcbiAgICAgKiBAcGFyYW0gcHJldmVudEF1dG9GYWlsXHJcbiAgICAgKiBAcmV0dXJucyB7UHJvbWlzZX0galF1ZXJ5IGFqYXggcHJvbWlzZSBvYmplY3RcclxuICAgICAqL1xyXG4gICAgcG9zdChzZXR0aW5nc09yVXJsOiBzdHJpbmcgfCBhbnksIHBhcmFtcz86IGFueSwgZGF0YT86IGFueSwgcHJldmVudEF1dG9GYWlsPzogYm9vbGVhbik6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMucmVxdWVzdChzZXR0aW5nc09yVXJsLCBwYXJhbXMsIGRhdGEsIHByZXZlbnRBdXRvRmFpbCwgXCJQT1NUXCIpO1xyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhbiBodHRwLWdldCBwcm9taXNlXHJcbiAgICAgKiBAcGFyYW0gc2V0dGluZ3NPclVybCB0aGUgdXJsIHRvIGdldFxyXG4gICAgICogQHBhcmFtIHBhcmFtcyBqUXVlcnkgc3R5bGUgYWpheCBwYXJhbWV0ZXJzXHJcbiAgICAgKiBAcGFyYW0gZGF0YSBqUXVlcnkgc3R5bGUgZGF0YSBmb3IgcG9zdC9wdXQgcmVxdWVzdHNcclxuICAgICAqIEBwYXJhbSBwcmV2ZW50QXV0b0ZhaWxcclxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBqUXVlcnkgYWpheCBwcm9taXNlIG9iamVjdFxyXG4gICAgICovXHJcbiAgICBkZWxldGUoc2V0dGluZ3NPclVybDogc3RyaW5nIHwgYW55LCBwYXJhbXM/OiBhbnksIGRhdGE/OiBhbnksIHByZXZlbnRBdXRvRmFpbD86IGJvb2xlYW4pOiBhbnkge1xyXG4gICAgICAgIHJldHVybiB0aGlzLnJlcXVlc3Qoc2V0dGluZ3NPclVybCwgcGFyYW1zLCBkYXRhLCBwcmV2ZW50QXV0b0ZhaWwsIFwiREVMRVRFXCIpO1xyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhbiBodHRwLWdldCBwcm9taXNlXHJcbiAgICAgKiBAcGFyYW0gc2V0dGluZ3NPclVybCB0aGUgdXJsIHRvIGdldFxyXG4gICAgICogQHBhcmFtIHBhcmFtcyBqUXVlcnkgc3R5bGUgYWpheCBwYXJhbWV0ZXJzXHJcbiAgICAgKiBAcGFyYW0gZGF0YSBqUXVlcnkgc3R5bGUgZGF0YSBmb3IgcG9zdC9wdXQgcmVxdWVzdHNcclxuICAgICAqIEBwYXJhbSBwcmV2ZW50QXV0b0ZhaWxcclxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBqUXVlcnkgYWpheCBwcm9taXNlIG9iamVjdFxyXG4gICAgICovXHJcbiAgICBwdXQoc2V0dGluZ3NPclVybDogc3RyaW5nIHwgYW55LCBwYXJhbXM/OiBhbnksIGRhdGE/OiBhbnksIHByZXZlbnRBdXRvRmFpbD86IGJvb2xlYW4pOiBhbnkge1xyXG4gICAgICAgIHJldHVybiB0aGlzLnJlcXVlc3Qoc2V0dGluZ3NPclVybCwgcGFyYW1zLCBkYXRhLCBwcmV2ZW50QXV0b0ZhaWwsIFwiUFVUXCIpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVxdWVzdChzZXR0aW5nczogc3RyaW5nIHwgYW55LCBwYXJhbXM6IGFueSwgZGF0YTogYW55LCBwcmV2ZW50QXV0b0ZhaWw6IGJvb2xlYW4sIG1ldGhvZDogc3RyaW5nKTogYW55IHtcclxuXHJcbiAgICAgICAgLy8gdXJsIHBhcmFtZXRlcjogYXV0b2NvbnZlcnQgYSBzaW5nbGUgdmFsdWUgKGluc3RlYWQgb2Ygb2JqZWN0IG9mIHZhbHVlcykgdG8gYW4gaWQ9Li4uIHBhcmFtZXRlclxyXG4gICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpjdXJseVxyXG4gICAgICAgIGlmICh0eXBlb2YgcGFyYW1zICE9PSBcIm9iamVjdFwiICYmIHR5cGVvZiBwYXJhbXMgIT09IFwidW5kZWZpbmVkXCIpXHJcbiAgICAgICAgICAgIHBhcmFtcyA9IHsgaWQ6IHBhcmFtcyB9O1xyXG5cclxuICAgICAgICAvLyBpZiB0aGUgZmlyc3QgcGFyYW1ldGVyIGlzIGEgc3RyaW5nLCByZXNvbHZlIHNldHRpbmdzXHJcbiAgICAgICAgaWYgKHR5cGVvZiBzZXR0aW5ncyA9PT0gXCJzdHJpbmdcIikge1xyXG4gICAgICAgICAgICBjb25zdCBjb250cm9sbGVyQWN0aW9uID0gc2V0dGluZ3Muc3BsaXQoXCIvXCIpO1xyXG4gICAgICAgICAgICBjb25zdCBjb250cm9sbGVyTmFtZSA9IGNvbnRyb2xsZXJBY3Rpb25bMF07XHJcbiAgICAgICAgICAgIGNvbnN0IGFjdGlvbk5hbWUgPSBjb250cm9sbGVyQWN0aW9uWzFdO1xyXG5cclxuICAgICAgICAgICAgaWYgKGNvbnRyb2xsZXJOYW1lID09PSBcIlwiIHx8IGFjdGlvbk5hbWUgPT09IFwiXCIpXHJcbiAgICAgICAgICAgICAgICBhbGVydChcIkVycm9yOiBjb250cm9sbGVyIG9yIGFjdGlvbiBub3QgZGVmaW5lZC4gV2lsbCBjb250aW51ZSB3aXRoIGxpa2VseSBlcnJvcnMuXCIpO1xyXG5cclxuICAgICAgICAgICAgc2V0dGluZ3MgPSB7XHJcbiAgICAgICAgICAgICAgICBjb250cm9sbGVyOiBjb250cm9sbGVyTmFtZSxcclxuICAgICAgICAgICAgICAgIGFjdGlvbjogYWN0aW9uTmFtZSxcclxuICAgICAgICAgICAgICAgIHBhcmFtcyxcclxuICAgICAgICAgICAgICAgIGRhdGEsXHJcbiAgICAgICAgICAgICAgICB1cmw6IGNvbnRyb2xsZXJBY3Rpb24ubGVuZ3RoID4gMiA/IHNldHRpbmdzIDogbnVsbCxcclxuICAgICAgICAgICAgICAgIHByZXZlbnRBdXRvRmFpbCxcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGNvbnN0IGRlZmF1bHRzID0ge1xyXG4gICAgICAgICAgICBtZXRob2Q6IG1ldGhvZCA9PT0gbnVsbCA/IFwiUE9TVFwiIDogbWV0aG9kLFxyXG4gICAgICAgICAgICBwYXJhbXM6IG51bGwgYXMgYW55LFxyXG4gICAgICAgICAgICBwcmV2ZW50QXV0b0ZhaWw6IGZhbHNlLFxyXG4gICAgICAgIH07XHJcbiAgICAgICAgc2V0dGluZ3MgPSAkLmV4dGVuZCh7fSwgZGVmYXVsdHMsIHNldHRpbmdzKTtcclxuICAgICAgICBjb25zdCBzZiA9ICQuU2VydmljZXNGcmFtZXdvcmsodGhpcy5pZCk7XHJcbiAgICAgICAgY29uc3QgY2JpZCA9IHRoaXMuY2JpZDsgLy8gbXVzdCByZWFkIGhlcmUsIGFzIHRoZSBcInRoaXNcIiB3aWxsIGNoYW5nZSBpbnNpZGUgdGhlIG1ldGhvZFxyXG5cclxuICAgICAgICBjb25zdCBwcm9taXNlID0gJC5hamF4KHtcclxuICAgICAgICAgICAgYXN5bmM6IHRydWUsXHJcbiAgICAgICAgICAgIGRhdGFUeXBlOiBzZXR0aW5ncy5kYXRhVHlwZSB8fCBcImpzb25cIiwgLy8gZGVmYXVsdCBpcyBqc29uIGlmIG5vdCBzcGVjaWZpZWRcclxuICAgICAgICAgICAgZGF0YTogSlNPTi5zdHJpbmdpZnkoc2V0dGluZ3MuZGF0YSksXHJcbiAgICAgICAgICAgIGNvbnRlbnRUeXBlOiBcImFwcGxpY2F0aW9uL2pzb25cIixcclxuICAgICAgICAgICAgdHlwZTogc2V0dGluZ3MubWV0aG9kLFxyXG4gICAgICAgICAgICB1cmw6IHRoaXMuZ2V0QWN0aW9uVXJsKHNldHRpbmdzKSxcclxuICAgICAgICAgICAgYmVmb3JlU2VuZCh4aHI6IGFueSkge1xyXG4gICAgICAgICAgICAgICAgeGhyLnNldFJlcXVlc3RIZWFkZXIoXCJDb250ZW50QmxvY2tJZFwiLCBjYmlkKTtcclxuICAgICAgICAgICAgICAgIHNmLnNldE1vZHVsZUhlYWRlcnMoeGhyKTtcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgaWYgKCFzZXR0aW5ncy5wcmV2ZW50QXV0b0ZhaWwpXHJcbiAgICAgICAgICAgIHByb21pc2UuZmFpbCh0aGlzLmNvbnRyb2xsZXIuc2hvd0RldGFpbGVkSHR0cEVycm9yKTtcclxuXHJcbiAgICAgICAgcmV0dXJuIHByb21pc2U7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBnZXRBY3Rpb25Vcmwoc2V0dGluZ3M6IGFueSk6IHN0cmluZyB7XHJcbiAgICAgICAgY29uc3Qgc2YgPSAkLlNlcnZpY2VzRnJhbWV3b3JrKHRoaXMuaWQpO1xyXG4gICAgICAgIGNvbnN0IGJhc2UgPSAoc2V0dGluZ3MudXJsKVxyXG4gICAgICAgICAgICA/IHRoaXMuY29udHJvbGxlci5yZXNvbHZlU2VydmljZVVybChzZXR0aW5ncy51cmwpXHJcbiAgICAgICAgICAgIDogc2YuZ2V0U2VydmljZVJvb3QoXCIyc3hjXCIpICsgXCJhcHAvYXV0by9hcGkvXCIgKyBzZXR0aW5ncy5jb250cm9sbGVyICsgXCIvXCIgKyBzZXR0aW5ncy5hY3Rpb247XHJcbiAgICAgICAgcmV0dXJuIGJhc2UgKyAoc2V0dGluZ3MucGFyYW1zID09PSBudWxsID8gXCJcIiA6IChcIj9cIiArICQucGFyYW0oc2V0dGluZ3MucGFyYW1zKSkpO1xyXG4gICAgfVxyXG5cclxufVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi8yc3hjLWFwaS9qcy9Ub1NpYy5TeGMuV2ViQXBpLnRzIiwiXHJcbmV4cG9ydCBjbGFzcyBUb3RhbFBvcHVwIHtcclxuICAgIGZyYW1lOiBhbnkgPSB1bmRlZmluZWQ7XHJcbiAgICBjYWxsYmFjazogYW55ID0gdW5kZWZpbmVkO1xyXG5cclxuICAgIG9wZW4odXJsOiBzdHJpbmcsIGNhbGxiYWNrOiAoKSA9PiB2b2lkKTogdm9pZCB7XHJcbiAgICAgICAgLy8gY291bnQgcGFyZW50cyB0byBzZWUgaG93IGhpZ2ggdGhlIHotaW5kZXggbmVlZHMgdG8gYmVcclxuICAgICAgICBsZXQgeiA9IDEwMDAwMDEwOyAvLyBOZWVkcyBhdCBsZWFzdCAxMDAwMDAwMCB0byBiZSBvbiB0b3Agb2YgdGhlIEROTjkgYmFyXHJcbiAgICAgICAgbGV0IHAgPSB3aW5kb3c7XHJcbiAgICAgICAgd2hpbGUgKHAgIT09IHdpbmRvdy50b3AgJiYgeiA8IDEwMDAwMTAwKSB7XHJcbiAgICAgICAgICAgIHorKztcclxuICAgICAgICAgICAgcCA9IHAucGFyZW50O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3Qgd3JhcHBlciA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJkaXZcIik7XHJcbiAgICAgICAgd3JhcHBlci5zZXRBdHRyaWJ1dGUoXCJzdHlsZVwiLCBcIiB0b3A6IDA7bGVmdDogMDt3aWR0aDogMTAwJTtoZWlnaHQ6IDEwMCU7IHBvc2l0aW9uOmZpeGVkOyB6LWluZGV4OlwiICsgeik7XHJcbiAgICAgICAgZG9jdW1lbnQuYm9keS5hcHBlbmRDaGlsZCh3cmFwcGVyKTtcclxuXHJcbiAgICAgICAgY29uc3QgaWZybSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJpZnJhbWVcIik7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoXCJhbGxvd3RyYW5zcGFyZW5jeVwiLCBcInRydWVcIik7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoXCJzdHlsZVwiLCBcInRvcDogMDtsZWZ0OiAwO3dpZHRoOiAxMDAlO2hlaWdodDogMTAwJTtcIik7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoXCJzcmNcIiwgdXJsKTtcclxuICAgICAgICB3cmFwcGVyLmFwcGVuZENoaWxkKGlmcm0pO1xyXG4gICAgICAgIGRvY3VtZW50LmJvZHkuY2xhc3NOYW1lICs9IFwiIHN4Yy1wb3B1cC1vcGVuXCI7XHJcbiAgICAgICAgdGhpcy5mcmFtZSA9IGlmcm07XHJcbiAgICAgICAgdGhpcy5jYWxsYmFjayA9IGNhbGxiYWNrO1xyXG4gICAgfVxyXG5cclxuICAgIGNsb3NlKCk6IHZvaWQge1xyXG4gICAgICAgIGlmICh0aGlzLmZyYW1lKSB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmJvZHkuY2xhc3NOYW1lID0gZG9jdW1lbnQuYm9keS5jbGFzc05hbWUucmVwbGFjZShcInN4Yy1wb3B1cC1vcGVuXCIsIFwiXCIpO1xyXG4gICAgICAgICAgICBjb25zdCBmcm0gPSB0aGlzLmZyYW1lO1xyXG4gICAgICAgICAgICBmcm0ucGFyZW50Tm9kZS5wYXJlbnROb2RlLnJlbW92ZUNoaWxkKGZybS5wYXJlbnROb2RlKTtcclxuICAgICAgICAgICAgdGhpcy5jYWxsYmFjaygpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBjbG9zZVRoaXMoKTogdm9pZCB7XHJcbiAgICAgICAgKHdpbmRvdy5wYXJlbnQgYXMgYW55KS4kMnN4Yy50b3RhbFBvcHVwLmNsb3NlKCk7XHJcbiAgICB9XHJcblxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5Ub3RhbFBvcHVwLnRzIiwiXHJcbiAgICBleHBvcnQgY2xhc3MgVXJsUGFyYW1NYW5hZ2VyIHtcclxuICAgICAgICBnZXQobmFtZTogc3RyaW5nKSB7XHJcbiAgICAgICAgICAgIC8vIHdhcm5pbmc6IHRoaXMgbWV0aG9kIGlzIGR1cGxpY2F0ZWQgaW4gMiBwbGFjZXMgLSBrZWVwIHRoZW0gaW4gc3luYy5cclxuICAgICAgICAgICAgLy8gbG9jYXRpb25zIGFyZSBlYXYgYW5kIDJzeGM0bmdcclxuICAgICAgICAgICAgbmFtZSA9IG5hbWUucmVwbGFjZSgvW1xcW10vLCBcIlxcXFxbXCIpLnJlcGxhY2UoL1tcXF1dLywgXCJcXFxcXVwiKTtcclxuICAgICAgICAgICAgY29uc3Qgc2VhcmNoUnggPSBuZXcgUmVnRXhwKFwiW1xcXFw/Jl1cIiArIG5hbWUgKyBcIj0oW14mI10qKVwiLCBcImlcIik7XHJcbiAgICAgICAgICAgIGxldCByZXN1bHRzID0gc2VhcmNoUnguZXhlYyhsb2NhdGlvbi5zZWFyY2gpO1xyXG4gICAgICAgICAgICBsZXQgc3RyUmVzdWx0OiBzdHJpbmc7XHJcblxyXG4gICAgICAgICAgICBpZiAocmVzdWx0cyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgY29uc3QgaGFzaFJ4ID0gbmV3IFJlZ0V4cChcIlsjJl1cIiArIG5hbWUgKyBcIj0oW14mI10qKVwiLCBcImlcIik7XHJcbiAgICAgICAgICAgICAgICByZXN1bHRzID0gaGFzaFJ4LmV4ZWMobG9jYXRpb24uaGFzaCk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIC8vIGlmIG5vdGhpbmcgZm91bmQsIHRyeSBub3JtYWwgVVJMIGJlY2F1c2UgRE5OIHBsYWNlcyBwYXJhbWV0ZXJzIGluIC9rZXkvdmFsdWUgbm90YXRpb25cclxuICAgICAgICAgICAgaWYgKHJlc3VsdHMgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIC8vIE90aGVyd2lzZSB0cnkgcGFydHMgb2YgdGhlIFVSTFxyXG4gICAgICAgICAgICAgICAgY29uc3QgbWF0Y2hlcyA9IHdpbmRvdy5sb2NhdGlvbi5wYXRobmFtZS5tYXRjaChuZXcgUmVnRXhwKFwiL1wiICsgbmFtZSArIFwiLyhbXi9dKylcIiwgXCJpXCIpKTtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBDaGVjayBpZiB3ZSBmb3VuZCBhbnl0aGluZywgaWYgd2UgZG8gZmluZCBpdCwgd2UgbXVzdCByZXZlcnNlIHRoZVxyXG4gICAgICAgICAgICAgICAgLy8gcmVzdWx0cyBzbyB3ZSBnZXQgdGhlIFwibGFzdFwiIG9uZSBpbiBjYXNlIHRoZXJlIGFyZSBtdWx0aXBsZSBoaXRzXHJcbiAgICAgICAgICAgICAgICBpZiAobWF0Y2hlcyAmJiBtYXRjaGVzLmxlbmd0aCA+IDEpXHJcbiAgICAgICAgICAgICAgICAgICAgc3RyUmVzdWx0ID0gbWF0Y2hlcy5yZXZlcnNlKClbMF07XHJcbiAgICAgICAgICAgIH0gZWxzZVxyXG4gICAgICAgICAgICAgICAgc3RyUmVzdWx0ID0gcmVzdWx0c1sxXTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBzdHJSZXN1bHQgPT09IG51bGwgfHwgc3RyUmVzdWx0ID09PSB1bmRlZmluZWRcclxuICAgICAgICAgICAgICAgID8gXCJcIlxyXG4gICAgICAgICAgICAgICAgOiBkZWNvZGVVUklDb21wb25lbnQoc3RyUmVzdWx0LnJlcGxhY2UoL1xcKy9nLCBcIiBcIikpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVxdWlyZShuYW1lOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgY29uc3QgZm91bmQgPSB0aGlzLmdldChuYW1lKTtcclxuICAgICAgICAgICAgaWYgKGZvdW5kID09PSBcIlwiKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBtZXNzYWdlID0gYFJlcXVpcmVkIHBhcmFtZXRlciAoJHtuYW1lfSkgbWlzc2luZyBmcm9tIHVybCAtIGNhbm5vdCBjb250aW51ZWA7XHJcbiAgICAgICAgICAgICAgICBhbGVydChtZXNzYWdlKTtcclxuICAgICAgICAgICAgICAgIHRocm93IG1lc3NhZ2U7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcmV0dXJuIGZvdW5kO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlVybC50cyJdLCJzb3VyY2VSb290IjoiIn0=