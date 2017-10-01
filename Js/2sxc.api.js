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
        var promise = $.ajax({
            async: true,
            dataType: settings.dataType || "json",
            data: JSON.stringify(settings.data),
            contentType: "application/json",
            type: settings.method,
            url: this.getActionUrl(settings),
            beforeSend: function (xhr) {
                xhr.setRequestHeader("ContentBlockId", this.cbid);
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAgMTFkODQ2YWM0MjE2ZDQzNjVhZTUiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvMnN4Yy5hcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkNvbnRyb2xsZXIudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkluc3RhbmNlLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5EYXRhLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5XZWJBcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlRvdGFsUG9wdXAudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlVybC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUE7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7OztBQUdBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLGFBQUs7QUFDTDtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBLG1DQUEyQiwwQkFBMEIsRUFBRTtBQUN2RCx5Q0FBaUMsZUFBZTtBQUNoRDtBQUNBO0FBQ0E7O0FBRUE7QUFDQSw4REFBc0QsK0RBQStEOztBQUVySDtBQUNBOztBQUVBO0FBQ0E7Ozs7Ozs7Ozs7QUN4RG9FO0FBS3BFLEVBQUUsQ0FBQyxDQUFDLENBQUMsTUFBTSxDQUFDLEtBQUssQ0FBQztJQUNkLE1BQU0sQ0FBQyxLQUFLLEdBQUcseUZBQWtCLEVBQUUsQ0FBQzs7Ozs7Ozs7Ozs7O0FDVDZEO0FBQ2pEO0FBQ0Y7QUEwQ2xELHVCQUF1QixFQUF3QixFQUFFLElBQWE7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UyxHQUFHLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUUzQixNQUFNLENBQUMsU0FBUyxLQUFLLElBQUksSUFBSSxTQUFTLEtBQUssU0FBUztjQUM5QyxFQUFFO2NBQ0Ysa0JBQWtCLENBQUMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxLQUFLLEVBQUUsR0FBRyxDQUFDLENBQUMsQ0FBQztJQUM1RCxDQUFDO0lBRUQsaUNBQU8sR0FBUCxVQUFRLElBQVk7UUFDaEIsSUFBTSxLQUFLLEdBQUcsSUFBSSxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsQ0FBQztRQUM3QixFQUFFLENBQUMsQ0FBQyxLQUFLLEtBQUssRUFBRSxDQUFDLENBQUMsQ0FBQztZQUNmLElBQU0sT0FBTyxHQUFHLHlCQUF1QixJQUFJLHlDQUFzQyxDQUFDO1lBQ2xGLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQztZQUNmLE1BQU0sT0FBTyxDQUFDO1FBQ2xCLENBQUM7UUFDRCxNQUFNLENBQUMsS0FBSyxDQUFDO0lBQ2pCLENBQUM7SUFDTCxzQkFBQztBQUFELENBQUMiLCJmaWxlIjoiMnN4Yy5hcGkuanMiLCJzb3VyY2VzQ29udGVudCI6WyIgXHQvLyBUaGUgbW9kdWxlIGNhY2hlXG4gXHR2YXIgaW5zdGFsbGVkTW9kdWxlcyA9IHt9O1xuXG4gXHQvLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuIFx0ZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXG4gXHRcdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuIFx0XHRpZihpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSkge1xuIFx0XHRcdHJldHVybiBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXS5leHBvcnRzO1xuIFx0XHR9XG4gXHRcdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG4gXHRcdHZhciBtb2R1bGUgPSBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSA9IHtcbiBcdFx0XHRpOiBtb2R1bGVJZCxcbiBcdFx0XHRsOiBmYWxzZSxcbiBcdFx0XHRleHBvcnRzOiB7fVxuIFx0XHR9O1xuXG4gXHRcdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuIFx0XHRtb2R1bGVzW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuIFx0XHQvLyBGbGFnIHRoZSBtb2R1bGUgYXMgbG9hZGVkXG4gXHRcdG1vZHVsZS5sID0gdHJ1ZTtcblxuIFx0XHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuIFx0XHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG4gXHR9XG5cblxuIFx0Ly8gZXhwb3NlIHRoZSBtb2R1bGVzIG9iamVjdCAoX193ZWJwYWNrX21vZHVsZXNfXylcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubSA9IG1vZHVsZXM7XG5cbiBcdC8vIGV4cG9zZSB0aGUgbW9kdWxlIGNhY2hlXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLmMgPSBpbnN0YWxsZWRNb2R1bGVzO1xuXG4gXHQvLyBkZWZpbmUgZ2V0dGVyIGZ1bmN0aW9uIGZvciBoYXJtb255IGV4cG9ydHNcbiBcdF9fd2VicGFja19yZXF1aXJlX18uZCA9IGZ1bmN0aW9uKGV4cG9ydHMsIG5hbWUsIGdldHRlcikge1xuIFx0XHRpZighX193ZWJwYWNrX3JlcXVpcmVfXy5vKGV4cG9ydHMsIG5hbWUpKSB7XG4gXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIG5hbWUsIHtcbiBcdFx0XHRcdGNvbmZpZ3VyYWJsZTogZmFsc2UsXG4gXHRcdFx0XHRlbnVtZXJhYmxlOiB0cnVlLFxuIFx0XHRcdFx0Z2V0OiBnZXR0ZXJcbiBcdFx0XHR9KTtcbiBcdFx0fVxuIFx0fTtcblxuIFx0Ly8gZ2V0RGVmYXVsdEV4cG9ydCBmdW5jdGlvbiBmb3IgY29tcGF0aWJpbGl0eSB3aXRoIG5vbi1oYXJtb255IG1vZHVsZXNcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubiA9IGZ1bmN0aW9uKG1vZHVsZSkge1xuIFx0XHR2YXIgZ2V0dGVyID0gbW9kdWxlICYmIG1vZHVsZS5fX2VzTW9kdWxlID9cbiBcdFx0XHRmdW5jdGlvbiBnZXREZWZhdWx0KCkgeyByZXR1cm4gbW9kdWxlWydkZWZhdWx0J107IH0gOlxuIFx0XHRcdGZ1bmN0aW9uIGdldE1vZHVsZUV4cG9ydHMoKSB7IHJldHVybiBtb2R1bGU7IH07XG4gXHRcdF9fd2VicGFja19yZXF1aXJlX18uZChnZXR0ZXIsICdhJywgZ2V0dGVyKTtcbiBcdFx0cmV0dXJuIGdldHRlcjtcbiBcdH07XG5cbiBcdC8vIE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbFxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5vID0gZnVuY3Rpb24ob2JqZWN0LCBwcm9wZXJ0eSkgeyByZXR1cm4gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eS5jYWxsKG9iamVjdCwgcHJvcGVydHkpOyB9O1xuXG4gXHQvLyBfX3dlYnBhY2tfcHVibGljX3BhdGhfX1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5wID0gXCJcIjtcblxuIFx0Ly8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4gXHRyZXR1cm4gX193ZWJwYWNrX3JlcXVpcmVfXyhfX3dlYnBhY2tfcmVxdWlyZV9fLnMgPSAwKTtcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyB3ZWJwYWNrL2Jvb3RzdHJhcCAxMWQ4NDZhYzQyMTZkNDM2NWFlNSIsIi8vIHRoaXMgaXMgdGhlIDJzeGMtamF2YXNjcmlwdCBBUElcclxuLy8gMnN4YyB3aWxsIGluY2x1ZGUgdGhpcyBhdXRvbWF0aWNhbGx5IHdoZW4gYSB1c2VyIGhhcyBlZGl0LXJpZ2h0c1xyXG4vLyBhIHRlbXBsYXRlIGRldmVsb3BlciB3aWxsIHR5cGljYWxseSB1c2UgdGhpcyB0byB1c2UgdGhlIGRhdGEtYXBpIHRvIHJlYWQgMnN4Yy1kYXRhIGZyb20gdGhlIHNlcnZlclxyXG4vLyByZWFkIG1vcmUgYWJvdXQgdGhpcyBpbiB0aGUgd2lraTogaHR0cHM6Ly9naXRodWIuY29tLzJzaWMvMnN4Yy93aWtpL0phdmFTY3JpcHQtJTI0MnN4Y1xyXG5cclxuaW1wb3J0IHsgYnVpbGRTeGNDb250cm9sbGVyLCBXaW5kb3cgfSBmcm9tIFwiLi9Ub1NpYy5TeGMuQ29udHJvbGxlclwiO1xyXG5cclxuLy8gUmVTaGFycGVyIGRpc2FibGUgSW5jb25zaXN0ZW50TmFtaW5nXHJcbmRlY2xhcmUgY29uc3Qgd2luZG93OiBXaW5kb3c7XHJcblxyXG5pZiAoIXdpbmRvdy4kMnN4YykgLy8gcHJldmVudCBkb3VibGUgZXhlY3V0aW9uXHJcbiAgICB3aW5kb3cuJDJzeGMgPSBidWlsZFN4Y0NvbnRyb2xsZXIoKTtcclxuXHJcbi8vIFJlU2hhcnBlciByZXN0b3JlIEluY29uc2lzdGVudE5hbWluZ1xyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi8yc3hjLWFwaS9qcy8yc3hjLmFwaS50cyIsIi8vIFJlU2hhcnBlciBkaXNhYmxlIEluY29uc2lzdGVudE5hbWluZ1xyXG5cclxuaW1wb3J0IHsgU3hjSW5zdGFuY2UsIFN4Y0luc3RhbmNlV2l0aEVkaXRpbmcsIFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB9IGZyb20gXCIuL1RvU2ljLlN4Yy5JbnN0YW5jZVwiO1xyXG5pbXBvcnQgeyBUb3RhbFBvcHVwIH0gZnJvbSBcIi4vVG9TaWMuU3hjLlRvdGFsUG9wdXBcIjtcclxuaW1wb3J0IHsgVXJsUGFyYW1NYW5hZ2VyIH0gZnJvbSBcIi4vVG9TaWMuU3hjLlVybFwiO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBXaW5kb3cgeyAkMnN4YzogU3hjQ29udHJvbGxlciB8IFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzOyB9XHJcblxyXG5kZWNsYXJlIGNvbnN0ICQ6IGFueTtcclxuZGVjbGFyZSBjb25zdCB3aW5kb3c6IFdpbmRvdztcclxuXHJcbi8qKlxyXG4gKiBUaGlzIGlzIHRoZSBpbnRlcmZhY2UgZm9yIHRoZSBtYWluICQyc3hjIG9iamVjdCBvbiB0aGUgd2luZG93XHJcbiAqL1xyXG5leHBvcnQgaW50ZXJmYWNlIFN4Y0NvbnRyb2xsZXIge1xyXG4gICAgLyoqXHJcbiAgICAgKiByZXR1cm5zIGEgMnN4Yy1pbnN0YW5jZSBvZiB0aGUgaWQgb3IgaHRtbC10YWcgcGFzc2VkIGluXHJcbiAgICAgKiBAcGFyYW0gaWRcclxuICAgICAqIEBwYXJhbSBjYmlkXHJcbiAgICAgKiBAcmV0dXJucyB7fVxyXG4gICAgICovXHJcbiAgICAoaWQ6IG51bWJlciB8IEhUTUxFbGVtZW50LCBjYmlkPzogbnVtYmVyKTogU3hjSW5zdGFuY2UgfCBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHMsXHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBzeXN0ZW0gaW5mb3JtYXRpb24sIG1haW5seSBmb3IgY2hlY2tpbmcgd2hpY2ggdmVyc2lvbiBvZiAyc3hjIGlzIHJ1bm5pbmdcclxuICAgICAqIG5vdGU6IGl0J3Mgbm90IGFsd2F5cyB1cGRhdGVkIHJlbGlhYmx5LCBidXQgaXQgaGVscHMgd2hlbiBkZWJ1Z2dpbmdcclxuICAgICAqL1xyXG4gICAgc3lzaW5mbzoge1xyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICAqIHRoZSB2ZXJzaW9uIHVzaW5nIHRoZSAjIy4jIy4jIyBzeW50YXhcclxuICAgICAgICAgKi9cclxuICAgICAgICB2ZXJzaW9uOiBzdHJpbmcsXHJcblxyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICAqIGEgc2hvcnQgdGV4dCBkZXNjcmlwdGlvbiwgZm9yIHBlb3BsZSB3aG8gaGF2ZSBubyBpZGVhIHdoYXQgdGhpcyBpc1xyXG4gICAgICAgICAqL1xyXG4gICAgICAgIGRlc2NyaXB0aW9uOiBzdHJpbmcsXHJcbiAgICB9O1xyXG59XHJcblxyXG4vKipcclxuICogcmV0dXJucyBhIDJzeGMtaW5zdGFuY2Ugb2YgdGhlIGlkIG9yIGh0bWwtdGFnIHBhc3NlZCBpblxyXG4gKiBAcGFyYW0gaWRcclxuICogQHBhcmFtIGNiaWRcclxuICogQHJldHVybnMge31cclxuICovXHJcbmZ1bmN0aW9uIFN4Y0NvbnRyb2xsZXIoaWQ6IG51bWJlciB8IEhUTUxFbGVtZW50LCBjYmlkPzogbnVtYmVyKTogU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIHtcclxuICAgIGNvbnN0ICQyc3hjID0gd2luZG93LiQyc3hjIGFzIFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzO1xyXG4gICAgaWYgKCEkMnN4Yy5fY29udHJvbGxlcnMpXHJcbiAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiJDJzeGMgbm90IGluaXRpYWxpemVkIHlldFwiKTtcclxuXHJcbiAgICAvLyBpZiBpdCdzIGEgZG9tLWVsZW1lbnQsIHVzZSBhdXRvLWZpbmRcclxuICAgIGlmICh0eXBlb2YgaWQgPT09IFwib2JqZWN0XCIpIHtcclxuICAgICAgICBjb25zdCBpZFR1cGxlID0gYXV0b0ZpbmQoaWQpO1xyXG4gICAgICAgIGlkID0gaWRUdXBsZVswXTtcclxuICAgICAgICBjYmlkID0gaWRUdXBsZVsxXTtcclxuICAgIH1cclxuXHJcbiAgICBpZiAoIWNiaWQpIGNiaWQgPSBpZDsgICAgICAgICAgIC8vIGlmIGNvbnRlbnQtYmxvY2sgaXMgdW5rbm93biwgdXNlIGlkIG9mIG1vZHVsZVxyXG4gICAgY29uc3QgY2FjaGVLZXkgPSBpZCArIFwiOlwiICsgY2JpZDsgLy8gbmV1dHJhbGl6ZSB0aGUgaWQgZnJvbSBvbGQgXCIzNFwiIGZvcm1hdCB0byB0aGUgbmV3IFwiMzU6MzUzXCIgZm9ybWF0XHJcblxyXG4gICAgLy8gZWl0aGVyIGdldCB0aGUgY2FjaGVkIGNvbnRyb2xsZXIgZnJvbSBwcmV2aW91cyBjYWxscywgb3IgY3JlYXRlIGEgbmV3IG9uZVxyXG4gICAgaWYgKCQyc3hjLl9jb250cm9sbGVyc1tjYWNoZUtleV0pIHJldHVybiAkMnN4Yy5fY29udHJvbGxlcnNbY2FjaGVLZXldO1xyXG5cclxuICAgIC8vIGFsc28gaW5pdCB0aGUgZGF0YS1jYWNoZSBpbiBjYXNlIGl0J3MgZXZlciBuZWVkZWRcclxuICAgIGlmICghJDJzeGMuX2RhdGFbY2FjaGVLZXldKSAkMnN4Yy5fZGF0YVtjYWNoZUtleV0gPSB7fTtcclxuXHJcbiAgICByZXR1cm4gKCQyc3hjLl9jb250cm9sbGVyc1tjYWNoZUtleV1cclxuICAgICAgICA9IG5ldyBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHMoaWQsIGNiaWQsIGNhY2hlS2V5LCAkMnN4YywgJC5TZXJ2aWNlc0ZyYW1ld29yaykpO1xyXG59XHJcblxyXG4vKipcclxuICogQnVpbGQgYSBTWEMgQ29udHJvbGxlciBmb3IgdGhlIHBhZ2UuIFNob3VsZCBvbmx5IGV2ZXIgYmUgZXhlY3V0ZWQgb25jZVxyXG4gKi9cclxuZXhwb3J0IGZ1bmN0aW9uIGJ1aWxkU3hjQ29udHJvbGxlcigpOiBTeGNDb250cm9sbGVyIHwgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHMge1xyXG4gICAgY29uc3QgdXJsTWFuYWdlciA9IG5ldyBVcmxQYXJhbU1hbmFnZXIoKTtcclxuICAgIGNvbnN0IGRlYnVnID0ge1xyXG4gICAgICAgIGxvYWQ6ICh1cmxNYW5hZ2VyLmdldChcImRlYnVnXCIpID09PSBcInRydWVcIiksXHJcbiAgICAgICAgdW5jYWNoZTogdXJsTWFuYWdlci5nZXQoXCJzeGN2ZXJcIiksXHJcbiAgICB9O1xyXG5cclxuICAgIGNvbnN0IGFkZE9uOiBhbnkgPSB7XHJcbiAgICAgICAgX2NvbnRyb2xsZXJzOiB7fSBhcyBhbnksXHJcbiAgICAgICAgc3lzaW5mbzoge1xyXG4gICAgICAgICAgICB2ZXJzaW9uOiBcIjA5LjA1LjAyXCIsXHJcbiAgICAgICAgICAgIGRlc2NyaXB0aW9uOiBcIlRoZSAyc3hjIENvbnRyb2xsZXIgb2JqZWN0IC0gcmVhZCBtb3JlIGFib3V0IGl0IG9uIDJzeGMub3JnXCIsXHJcbiAgICAgICAgfSxcclxuICAgICAgICBiZXRhOiB7fSxcclxuICAgICAgICBfZGF0YToge30sXHJcbiAgICAgICAgLy8gdGhpcyBjcmVhdGVzIGEgZnVsbC1zY3JlZW4gaWZyYW1lLXBvcHVwIGFuZCBwcm92aWRlcyBhIGNsb3NlLWNvbW1hbmQgdG8gZmluaXNoIHRoZSBkaWFsb2cgYXMgbmVlZGVkXHJcbiAgICAgICAgdG90YWxQb3B1cDogbmV3IFRvdGFsUG9wdXAoKSxcclxuICAgICAgICB1cmxQYXJhbXM6IHVybE1hbmFnZXIsXHJcbiAgICAgICAgLy8gbm90ZTogSSB3b3VsZCBsaWtlIHRvIHJlbW92ZSB0aGlzIGZyb20gJDJzeGMsIGJ1dCBpdCdzIGN1cnJlbnRseVxyXG4gICAgICAgIC8vIHVzZWQgYm90aCBpbiB0aGUgaW5wYWdlLWVkaXQgYW5kIGluIHRoZSBkaWFsb2dzXHJcbiAgICAgICAgLy8gZGVidWcgc3RhdGUgd2hpY2ggaXMgbmVlZGVkIGluIHZhcmlvdXMgcGxhY2VzXHJcbiAgICAgICAgZGVidWcsXHJcbiAgICAgICAgLy8gbWluaS1oZWxwZXJzIHRvIG1hbmFnZSAyc3hjIHBhcnRzLCBhIGJpdCBsaWtlIGEgZGVwZW5kZW5jeSBsb2FkZXJcclxuICAgICAgICAvLyB3aGljaCB3aWxsIG9wdGltaXplIHRvIGxvYWQgbWluL21heCBkZXBlbmRpbmcgb24gZGVidWcgc3RhdGVcclxuICAgICAgICBwYXJ0czoge1xyXG4gICAgICAgICAgICBnZXRVcmwodXJsOiBzdHJpbmcsIHByZXZlbnRVbm1pbjogYm9vbGVhbikge1xyXG4gICAgICAgICAgICAgICAgbGV0IHIgPSAocHJldmVudFVubWluIHx8ICFkZWJ1Zy5sb2FkKSA/IHVybCA6IHVybC5yZXBsYWNlKFwiLm1pblwiLCBcIlwiKTsgLy8gdXNlIG1pbiBvciBub3RcclxuICAgICAgICAgICAgICAgIGlmIChkZWJ1Zy51bmNhY2hlICYmIHIuaW5kZXhPZihcInN4Y3ZlclwiKSA9PT0gLTEpXHJcbiAgICAgICAgICAgICAgICAgICAgciA9IHIgKyAoKHIuaW5kZXhPZihcIj9cIikgPT09IC0xKSA/IFwiP1wiIDogXCImXCIpICsgXCJzeGN2ZXI9XCIgKyBkZWJ1Zy51bmNhY2hlO1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIHI7XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgfSxcclxuICAgIH07XHJcbiAgICBmb3IgKGNvbnN0IHByb3BlcnR5IGluIGFkZE9uKVxyXG4gICAgICAgIGlmIChhZGRPbi5oYXNPd25Qcm9wZXJ0eShwcm9wZXJ0eSkpXHJcbiAgICAgICAgICAgIFN4Y0NvbnRyb2xsZXJbcHJvcGVydHldID0gYWRkT25bcHJvcGVydHldIGFzIGFueTtcclxuICAgIHJldHVybiBTeGNDb250cm9sbGVyIGFzIGFueSBhcyBTeGNDb250cm9sbGVyV2l0aEludGVybmFscztcclxufVxyXG5cclxuZnVuY3Rpb24gYXV0b0ZpbmQoZG9tRWxlbWVudDogSFRNTEVsZW1lbnQpOiBbbnVtYmVyLCBudW1iZXJdIHtcclxuICAgIGNvbnN0IGNvbnRhaW5lclRhZyA9ICQoZG9tRWxlbWVudCkuY2xvc2VzdChcIi5zYy1jb250ZW50LWJsb2NrXCIpWzBdO1xyXG4gICAgaWYgKCFjb250YWluZXJUYWcpIHJldHVybiBudWxsO1xyXG4gICAgY29uc3QgaWlkID0gY29udGFpbmVyVGFnLmdldEF0dHJpYnV0ZShcImRhdGEtY2ItaW5zdGFuY2VcIik7XHJcbiAgICBjb25zdCBjYmlkID0gY29udGFpbmVyVGFnLmdldEF0dHJpYnV0ZShcImRhdGEtY2ItaWRcIik7XHJcbiAgICBpZiAoIWlpZCB8fCAhY2JpZCkgcmV0dXJuIG51bGw7XHJcbiAgICByZXR1cm4gW2lpZCwgY2JpZF07XHJcbn1cclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHMgZXh0ZW5kcyBTeGNDb250cm9sbGVyIHtcclxuICAgIChpZDogbnVtYmVyIHwgSFRNTEVsZW1lbnQsIGNiaWQ/OiBudW1iZXIpOiBTeGNJbnN0YW5jZSB8IFN4Y0luc3RhbmNlV2l0aEludGVybmFscztcclxuICAgIHRvdGFsUG9wdXA6IFRvdGFsUG9wdXA7XHJcbiAgICB1cmxQYXJhbXM6IFVybFBhcmFtTWFuYWdlcjtcclxuICAgIGJldGE6IGFueTtcclxuICAgIF9jb250cm9sbGVyczogYW55O1xyXG4gICAgX2RhdGE6IGFueTtcclxuICAgIF9tYW5hZ2U6IGFueTtcclxuICAgIF90cmFuc2xhdGVJbml0OiBhbnk7XHJcbiAgICBkZWJ1ZzogYW55O1xyXG4gICAgcGFydHM6IGFueTtcclxuXHJcbn1cclxuXHJcbi8vIFJlU2hhcnBlciByZXN0b3JlIEluY29uc2lzdGVudE5hbWluZ1xyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi8yc3hjLWFwaS9qcy9Ub1NpYy5TeGMuQ29udHJvbGxlci50cyIsIlxyXG5pbXBvcnQgeyBTeGNDb250cm9sbGVyLCBTeGNDb250cm9sbGVyV2l0aEludGVybmFscyB9IGZyb20gXCIuL1RvU2ljLlN4Yy5Db250cm9sbGVyXCI7XHJcbmltcG9ydCB7IFN4Y0RhdGFXaXRoSW50ZXJuYWxzIH0gZnJvbSBcIi4vVG9TaWMuU3hjLkRhdGFcIjtcclxuaW1wb3J0IHsgU3hjV2ViQXBpV2l0aEludGVybmFscyB9IGZyb20gXCIuL1RvU2ljLlN4Yy5XZWJBcGlcIjtcclxuLyoqXHJcbiAqIFRoZSB0eXBpY2FsIHN4Yy1pbnN0YW5jZSBvYmplY3QgZm9yIGEgc3BlY2lmaWMgRE5OIG1vZHVsZSBvciBjb250ZW50LWJsb2NrXHJcbiAqL1xyXG5leHBvcnQgY2xhc3MgU3hjSW5zdGFuY2Uge1xyXG4gICAgLyoqXHJcbiAgICAgKiBoZWxwZXJzIGZvciBhamF4IGNhbGxzXHJcbiAgICAgKi9cclxuICAgIHdlYkFwaTogU3hjV2ViQXBpV2l0aEludGVybmFscztcclxuICAgIHByb3RlY3RlZCBzZXJ2aWNlUm9vdDogc3RyaW5nO1xyXG4gICAgcHJpdmF0ZSByZWFkb25seSBzZXJ2aWNlU2NvcGVzID0gW1wiYXBwXCIsIFwiYXBwLXN5c1wiLCBcImFwcC1hcGlcIiwgXCJhcHAtcXVlcnlcIiwgXCJhcHAtY29udGVudFwiLCBcImVhdlwiLCBcInZpZXdcIiwgXCJkbm5cIl07XHJcblxyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgICogdGhlIHN4Yy1pbnN0YW5jZSBJRCwgd2hpY2ggaXMgdXN1YWxseSB0aGUgRE5OIE1vZHVsZSBJZFxyXG4gICAgICAgICAqL1xyXG4gICAgICAgIHB1YmxpYyBpZDogbnVtYmVyLFxyXG5cclxuICAgICAgICAvKipcclxuICAgICAgICAgKiBjb250ZW50LWJsb2NrIElELCB3aGljaCBpcyBlaXRoZXIgdGhlIG1vZHVsZSBJRCwgb3IgdGhlIGNvbnRlbnQtYmxvY2sgZGVmaW5pdGlpb24gZW50aXR5IElEXHJcbiAgICAgICAgICogdGhpcyBpcyBhbiBhZHZhbmNlZCBjb25jZXB0IHlvdSB1c3VhbGx5IGRvbid0IGNhcmUgYWJvdXQsIG90aGVyd2lzZSB5b3Ugc2hvdWxkIHJlc2VhcmNoIGl0XHJcbiAgICAgICAgICovXHJcbiAgICAgICAgcHVibGljIGNiaWQ6IG51bWJlcixcclxuICAgICAgICBwcm90ZWN0ZWQgcmVhZG9ubHkgZG5uU2Y6IGFueSxcclxuICAgICkge1xyXG4gICAgICAgIHRoaXMuc2VydmljZVJvb3QgPSBkbm5TZihpZCkuZ2V0U2VydmljZVJvb3QoXCIyc3hjXCIpO1xyXG4gICAgICAgIHRoaXMud2ViQXBpID0gbmV3IFN4Y1dlYkFwaVdpdGhJbnRlcm5hbHModGhpcywgaWQsIGNiaWQpO1xyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogY29udmVydHMgYSBzaG9ydCBhcGktY2FsbCBwYXRoIGxpa2UgXCIvYXBwL0Jsb2cvcXVlcnkveHl6XCIgdG8gdGhlIEROTiBmdWxsIHBhdGhcclxuICAgICAqIHdoaWNoIHZhcmllcyBmcm9tIGluc3RhbGxhdGlvbiB0byBpbnN0YWxsYXRpb24gbGlrZSBcIi9kZXNrdG9wbW9kdWxlcy9hcGkvMnN4Yy9hcHAvLi4uXCJcclxuICAgICAqIEBwYXJhbSB2aXJ0dWFsUGF0aFxyXG4gICAgICogQHJldHVybnMgbWFwcGVkIHBhdGhcclxuICAgICAqL1xyXG4gICAgcmVzb2x2ZVNlcnZpY2VVcmwodmlydHVhbFBhdGg6IHN0cmluZykge1xyXG4gICAgICAgIGNvbnN0IHNjb3BlID0gdmlydHVhbFBhdGguc3BsaXQoXCIvXCIpWzBdLnRvTG93ZXJDYXNlKCk7XHJcblxyXG4gICAgICAgIC8vIHN0b3AgaWYgaXQncyBub3Qgb25lIG9mIG91ciBzcGVjaWFsIHBhdGhzXHJcbiAgICAgICAgaWYgKHRoaXMuc2VydmljZVNjb3Blcy5pbmRleE9mKHNjb3BlKSA9PT0gLTEpXHJcbiAgICAgICAgICAgIHJldHVybiB2aXJ0dWFsUGF0aDtcclxuXHJcbiAgICAgICAgcmV0dXJuIHRoaXMuc2VydmljZVJvb3QgKyBzY29wZSArIFwiL1wiICsgdmlydHVhbFBhdGguc3Vic3RyaW5nKHZpcnR1YWxQYXRoLmluZGV4T2YoXCIvXCIpICsgMSk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIC8vIFNob3cgYSBuaWNlIGVycm9yIHdpdGggbW9yZSBpbmZvcyBhcm91bmQgMnN4Y1xyXG4gICAgc2hvd0RldGFpbGVkSHR0cEVycm9yKHJlc3VsdDogYW55KTogYW55IHtcclxuICAgICAgICBpZiAod2luZG93LmNvbnNvbGUpXHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKHJlc3VsdCk7XHJcblxyXG4gICAgICAgIGlmIChyZXN1bHQuc3RhdHVzID09PSA0MDQgJiZcclxuICAgICAgICAgICAgcmVzdWx0LmNvbmZpZyAmJlxyXG4gICAgICAgICAgICByZXN1bHQuY29uZmlnLnVybCAmJlxyXG4gICAgICAgICAgICByZXN1bHQuY29uZmlnLnVybC5pbmRleE9mKFwiL2Rpc3QvaTE4bi9cIikgPiAtMSkge1xyXG4gICAgICAgICAgICBpZiAod2luZG93LmNvbnNvbGUpXHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcImp1c3QgZnlpOiBmYWlsZWQgdG8gbG9hZCBsYW5ndWFnZSByZXNvdXJjZTsgd2lsbCBoYXZlIHRvIHVzZSBkZWZhdWx0XCIpO1xyXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0O1xyXG4gICAgICAgIH1cclxuXHJcblxyXG4gICAgICAgIC8vIGlmIGl0J3MgYW4gdW5zcGVjaWZpZWQgMC1lcnJvciwgaXQncyBwcm9iYWJseSBub3QgYW4gZXJyb3IgYnV0IGEgY2FuY2VsbGVkIHJlcXVlc3QsXHJcbiAgICAgICAgLy8gKGhhcHBlbnMgd2hlbiBjbG9zaW5nIHBvcHVwcyBjb250YWluaW5nIGFuZ3VsYXJKUylcclxuICAgICAgICBpZiAocmVzdWx0LnN0YXR1cyA9PT0gMCB8fCByZXN1bHQuc3RhdHVzID09PSAtMSlcclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcclxuXHJcbiAgICAgICAgLy8gbGV0J3MgdHJ5IHRvIHNob3cgZ29vZCBtZXNzYWdlcyBpbiBtb3N0IGNhc2VzXHJcbiAgICAgICAgbGV0IGluZm9UZXh0ID0gXCJIYWQgYW4gZXJyb3IgdGFsa2luZyB0byB0aGUgc2VydmVyIChzdGF0dXMgXCIgKyByZXN1bHQuc3RhdHVzICsgXCIpLlwiO1xyXG4gICAgICAgIGNvbnN0IHNydlJlc3AgPSByZXN1bHQucmVzcG9uc2VUZXh0XHJcbiAgICAgICAgICAgID8gSlNPTi5wYXJzZShyZXN1bHQucmVzcG9uc2VUZXh0KSAvLyBmb3IganF1ZXJ5IGFqYXggZXJyb3JzXHJcbiAgICAgICAgICAgIDogcmVzdWx0LmRhdGE7IC8vIGZvciBhbmd1bGFyICRodHRwXHJcbiAgICAgICAgaWYgKHNydlJlc3ApIHtcclxuICAgICAgICAgICAgY29uc3QgbXNnID0gc3J2UmVzcC5NZXNzYWdlO1xyXG4gICAgICAgICAgICBpZiAobXNnKSBpbmZvVGV4dCArPSBcIlxcbk1lc3NhZ2U6IFwiICsgbXNnO1xyXG4gICAgICAgICAgICBjb25zdCBtc2dEZXQgPSBzcnZSZXNwLk1lc3NhZ2VEZXRhaWwgfHwgc3J2UmVzcC5FeGNlcHRpb25NZXNzYWdlO1xyXG4gICAgICAgICAgICBpZiAobXNnRGV0KSBpbmZvVGV4dCArPSBcIlxcbkRldGFpbDogXCIgKyBtc2dEZXQ7XHJcblxyXG5cclxuICAgICAgICAgICAgaWYgKG1zZ0RldCAmJiBtc2dEZXQuaW5kZXhPZihcIk5vIGFjdGlvbiB3YXMgZm91bmRcIikgPT09IDApXHJcbiAgICAgICAgICAgICAgICBpZiAobXNnRGV0LmluZGV4T2YoXCJ0aGF0IG1hdGNoZXMgdGhlIG5hbWVcIikgPiAwKVxyXG4gICAgICAgICAgICAgICAgICAgIGluZm9UZXh0ICs9IFwiXFxuXFxuVGlwIGZyb20gMnN4YzogeW91IHByb2JhYmx5IGdvdCB0aGUgYWN0aW9uLW5hbWUgd3JvbmcgaW4geW91ciBKUy5cIjtcclxuICAgICAgICAgICAgICAgIGVsc2UgaWYgKG1zZ0RldC5pbmRleE9mKFwidGhhdCBtYXRjaGVzIHRoZSByZXF1ZXN0LlwiKSA+IDApXHJcbiAgICAgICAgICAgICAgICAgICAgaW5mb1RleHQgKz0gXCJcXG5cXG5UaXAgZnJvbSAyc3hjOiBTZWVtcyBsaWtlIHRoZSBwYXJhbWV0ZXJzIGFyZSB0aGUgd3JvbmcgYW1vdW50IG9yIHR5cGUuXCI7XHJcblxyXG4gICAgICAgICAgICBpZiAobXNnICYmIG1zZy5pbmRleE9mKFwiQ29udHJvbGxlclwiKSA9PT0gMCAmJiBtc2cuaW5kZXhPZihcIm5vdCBmb3VuZFwiKSA+IDApXHJcbiAgICAgICAgICAgICAgICBpbmZvVGV4dCArPVxyXG4gICAgICAgICAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTptYXgtbGluZS1sZW5ndGhcclxuICAgICAgICAgICAgICAgICAgICBcIlxcblxcblRpcCBmcm9tIDJzeGM6IHlvdSBwcm9iYWJseSBzcGVsbGVkIHRoZSBjb250cm9sbGVyIG5hbWUgd3Jvbmcgb3IgZm9yZ290IHRvIHJlbW92ZSB0aGUgd29yZCAnY29udHJvbGxlcicgZnJvbSB0aGUgY2FsbCBpbiBKUy4gVG8gY2FsbCBhIGNvbnRyb2xsZXIgY2FsbGVkICdEZW1vQ29udHJvbGxlcicgb25seSB1c2UgJ0RlbW8nLlwiO1xyXG5cclxuICAgICAgICB9XHJcbiAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm1heC1saW5lLWxlbmd0aFxyXG4gICAgICAgIGluZm9UZXh0ICs9IFwiXFxuXFxuaWYgeW91IGFyZSBhbiBhZHZhbmNlZCB1c2VyIHlvdSBjYW4gbGVhcm4gbW9yZSBhYm91dCB3aGF0IHdlbnQgd3JvbmcgLSBkaXNjb3ZlciBob3cgb24gMnN4Yy5vcmcvaGVscD90YWc9ZGVidWdcIjtcclxuICAgICAgICBhbGVydChpbmZvVGV4dCk7XHJcblxyXG4gICAgICAgIHJldHVybiByZXN1bHQ7XHJcbiAgICB9XHJcbn1cclxuXHJcbi8qKlxyXG4gKiBFbmhhbmNlZCBzeGMgaW5zdGFuY2Ugd2l0aCBhZGRpdGlvbmFsIGVkaXRpbmcgZnVuY3Rpb25hbGl0eVxyXG4gKiBVc2UgdGhpcywgaWYgeW91IGludGVuZCB0byBydW4gY29udGVudC1tYW5hZ2VtZW50IGNvbW1hbmRzIGxpa2UgXCJlZGl0XCIgZnJvbSB5b3VyIEpTIGRpcmVjdGx5XHJcbiAqL1xyXG5leHBvcnQgY2xhc3MgU3hjSW5zdGFuY2VXaXRoRWRpdGluZyBleHRlbmRzIFN4Y0luc3RhbmNlIHtcclxuICAgIC8qKlxyXG4gICAgICogbWFuYWdlIG9iamVjdCB3aGljaCBwcm92aWRlcyBhY2Nlc3MgdG8gYWRkaXRpb25hbCBjb250ZW50LW1hbmFnZW1lbnQgZmVhdHVyZXNcclxuICAgICAqIGl0IG9ubHkgZXhpc3RzIGlmIDJzeGMgaXMgaW4gZWRpdCBtb2RlIChvdGhlcndpc2UgdGhlIEpTIGFyZSBub3QgaW5jbHVkZWQgZm9yIHRoZXNlIGZlYXR1cmVzKVxyXG4gICAgICovXHJcbiAgICBtYW5hZ2U6IGFueSA9IG51bGw7IC8vIGluaXRpYWxpemUgY29ycmVjdGx5IGxhdGVyIG9uXHJcblxyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgcHVibGljIGlkOiBudW1iZXIsXHJcbiAgICAgICAgcHVibGljIGNiaWQ6IG51bWJlcixcclxuLy8gUmVTaGFycGVyIGRpc2FibGUgb25jZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuICAgICAgICBwcm90ZWN0ZWQgJDJzeGM6IFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzLFxyXG4gICAgICAgIHByb3RlY3RlZCByZWFkb25seSBkbm5TZjogYW55LFxyXG4gICAgKSB7XHJcbiAgICAgICAgc3VwZXIoaWQsIGNiaWQsIGRublNmKTtcclxuXHJcbiAgICAgICAgLy8gYWRkIG1hbmFnZSBwcm9wZXJ0eSwgYnV0IG5vdCB3aXRoaW4gaW5pdGlhbGl6ZXIsIGJlY2F1c2UgaW5zaWRlIHRoZSBtYW5hZ2UtaW5pdGlhbGl6ZXIgaXQgbWF5IHJlZmVyZW5jZSAyc3hjIGFnYWluXHJcbiAgICAgICAgdHJ5IHsgLy8gc29tZXRpbWVzIHRoZSBtYW5hZ2UgY2FuJ3QgYmUgYnVpbHQsIGxpa2UgYmVmb3JlIGluc3RhbGxpbmdcclxuICAgICAgICAgICAgaWYgKCQyc3hjLl9tYW5hZ2UpICQyc3hjLl9tYW5hZ2UuaW5pdEluc3RhbmNlKHRoaXMpO1xyXG4gICAgICAgIH0gY2F0Y2ggKGUpIHtcclxuICAgICAgICAgICAgY29uc29sZS5lcnJvcignZXJyb3IgaW4gMnN4YyAtIHdpbGwgb25seSBsb2cgYnV0IG5vdCB0aHJvdycsIGUpO1xyXG4gICAgICAgICAgICAvLyB0aHJvdyBlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gdGhpcyBvbmx5IHdvcmtzIHdoZW4gbWFuYWdlIGV4aXN0cyAobm90IGluc3RhbGxpbmcpIGFuZCB0cmFuc2xhdG9yIGV4aXN0cyB0b29cclxuICAgICAgICBpZiAoJDJzeGMuX3RyYW5zbGF0ZUluaXQgJiYgdGhpcy5tYW5hZ2UpICQyc3hjLl90cmFuc2xhdGVJbml0KHRoaXMubWFuYWdlKTsgICAgLy8gaW5pdCB0cmFuc2xhdGUsIG5vdCByZWFsbHkgbmljZSwgYnV0IG9rIGZvciBub3dcclxuXHJcbiAgICB9XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBjaGVja3MgaWYgd2UncmUgY3VycmVudGx5IGluIGVkaXQgbW9kZVxyXG4gICAgICogQHJldHVybnMge2Jvb2xlYW59XHJcbiAgICAgKi9cclxuICAgIGlzRWRpdE1vZGUoKSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMubWFuYWdlICYmIHRoaXMubWFuYWdlLl9pc0VkaXRNb2RlKCk7XHJcbiAgICB9XHJcblxyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIGV4dGVuZHMgU3hjSW5zdGFuY2VXaXRoRWRpdGluZyB7XHJcbiAgICBkYXRhOiBTeGNEYXRhV2l0aEludGVybmFscztcclxuICAgIHNvdXJjZTogYW55ID0gbnVsbDtcclxuICAgIGlzTG9hZGVkID0gZmFsc2U7XHJcbiAgICBsYXN0UmVmcmVzaDogRGF0ZSA9IG51bGw7XHJcblxyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgcHVibGljIGlkOiBudW1iZXIsXHJcbiAgICAgICAgcHVibGljIGNiaWQ6IG51bWJlcixcclxuICAgICAgICBwcml2YXRlIGNhY2hlS2V5OiBzdHJpbmcsXHJcbi8vIFJlU2hhcnBlciBkaXNhYmxlIG9uY2UgSW5jb25zaXN0ZW50TmFtaW5nXHJcbiAgICAgICAgcHJvdGVjdGVkICQyc3hjOiBTeGNDb250cm9sbGVyV2l0aEludGVybmFscyxcclxuICAgICAgICBwcm90ZWN0ZWQgcmVhZG9ubHkgZG5uU2Y6IGFueSxcclxuICAgICkge1xyXG4gICAgICAgIHN1cGVyKGlkLCBjYmlkLCAkMnN4YywgZG5uU2YpO1xyXG4gICAgICAgIHRoaXMuZGF0YSA9IG5ldyBTeGNEYXRhV2l0aEludGVybmFscyh0aGlzKTtcclxuICAgIH1cclxuXHJcbiAgICByZWNyZWF0ZShyZXNldENhY2hlOiBib29sZWFuKTogU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIHtcclxuICAgICAgICBpZiAocmVzZXRDYWNoZSkgZGVsZXRlIHRoaXMuJDJzeGMuX2NvbnRyb2xsZXJzW3RoaXMuY2FjaGVLZXldOyAvLyBjbGVhciBjYWNoZVxyXG4gICAgICAgIHJldHVybiB0aGlzLiQyc3hjKHRoaXMuaWQsIHRoaXMuY2JpZCkgYXMgYW55IGFzIFN4Y0luc3RhbmNlV2l0aEludGVybmFsczsgLy8gZ2VuZXJhdGUgbmV3XHJcbiAgICB9XHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkluc3RhbmNlLnRzIiwiaW1wb3J0IHsgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIH0gZnJvbSBcIi4vVG9TaWMuU3hjLkluc3RhbmNlXCI7XHJcblxyXG5kZWNsYXJlIGNvbnN0ICQ6IGFueTtcclxuXHJcblxyXG5leHBvcnQgY2xhc3MgU3hjRGF0YVdpdGhJbnRlcm5hbHMge1xyXG4gICAgc291cmNlOiBhbnkgPSB1bmRlZmluZWQ7XHJcblxyXG4gICAgLy8gaW4tc3RyZWFtc1xyXG4gICAgXCJpblwiOiBhbnkgPSB7fTtcclxuXHJcbiAgICAvLyB3aWxsIGhvbGQgdGhlIGRlZmF1bHQgc3RyZWFtIChbXCJpblwiXVtcIkRlZmF1bHRcIl0uTGlzdFxyXG4gICAgTGlzdDogYW55ID0gW107XHJcblxyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgcHJpdmF0ZSBjb250cm9sbGVyOiBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHMsXHJcbiAgICApIHtcclxuXHJcbiAgICB9XHJcblxyXG4gICAgLy8gc291cmNlIHBhdGggZGVmYXVsdGluZyB0byBjdXJyZW50IHBhZ2UgKyBvcHRpb25hbCBwYXJhbXNcclxuICAgIHNvdXJjZVVybChwYXJhbXM/OiBzdHJpbmcpOiBzdHJpbmcge1xyXG4gICAgICAgIGxldCB1cmwgPSB0aGlzLmNvbnRyb2xsZXIucmVzb2x2ZVNlcnZpY2VVcmwoXCJhcHAtc3lzL2FwcGNvbnRlbnQvR2V0Q29udGVudEJsb2NrRGF0YVwiKTtcclxuICAgICAgICBpZiAodHlwZW9mIHBhcmFtcyA9PT0gXCJzdHJpbmdcIikgLy8gdGV4dCBsaWtlICdpZD03J1xyXG4gICAgICAgICAgICB1cmwgKz0gXCImXCIgKyBwYXJhbXM7XHJcbiAgICAgICAgcmV0dXJuIHVybDtcclxuICAgIH1cclxuXHJcblxyXG4gICAgLy8gbG9hZCBkYXRhIHZpYSBhamF4XHJcbiAgICBsb2FkKHNvdXJjZT86IGFueSkge1xyXG4gICAgICAgIC8vIGlmIHNvdXJjZSBpcyBhbHJlYWR5IHRoZSBkYXRhLCBzZXQgaXRcclxuICAgICAgICBpZiAoc291cmNlICYmIHNvdXJjZS5MaXN0KSB7XHJcbiAgICAgICAgICAgIC8vIDIwMTctMDktMDUgMmRtOiBkaXNjb3ZlcmQgYSBjYWxsIHRvIGFuIGluZXhpc3RpbmcgZnVuY3Rpb25cclxuICAgICAgICAgICAgLy8gc2luY2UgdGhpcyBpcyBhbiBvbGQgQVBJIHdoaWNoIGlzIGJlaW5nIGRlcHJlY2F0ZWQsIHBsZWFzZSBkb24ndCBmaXggdW5sZXNzIHdlIGdldCBhY3RpdmUgZmVlZGJhY2tcclxuICAgICAgICAgICAgLy8gY29udHJvbGxlci5kYXRhLnNldERhdGEoc291cmNlKTtcclxuICAgICAgICAgICAgcmV0dXJuIHRoaXMuY29udHJvbGxlci5kYXRhO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGlmICghc291cmNlKVxyXG4gICAgICAgICAgICAgICAgc291cmNlID0ge307XHJcbiAgICAgICAgICAgIGlmICghc291cmNlLnVybClcclxuICAgICAgICAgICAgICAgIHNvdXJjZS51cmwgPSB0aGlzLmNvbnRyb2xsZXIuZGF0YS5zb3VyY2VVcmwoKTtcclxuICAgICAgICAgICAgc291cmNlLm9yaWdTdWNjZXNzID0gc291cmNlLnN1Y2Nlc3M7XHJcbiAgICAgICAgICAgIHNvdXJjZS5zdWNjZXNzID0gKGRhdGE6IGFueSkgPT4ge1xyXG5cclxuICAgICAgICAgICAgICAgIGZvciAoY29uc3QgZGF0YVNldE5hbWUgaW4gZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChkYXRhLmhhc093blByb3BlcnR5KGRhdGFTZXROYW1lKSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGRhdGFbZGF0YVNldE5hbWVdLkxpc3QgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuY29udHJvbGxlci5kYXRhLmluW2RhdGFTZXROYW1lXSA9IGRhdGFbZGF0YVNldE5hbWVdO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5jb250cm9sbGVyLmRhdGEuaW5bZGF0YVNldE5hbWVdLm5hbWUgPSBkYXRhU2V0TmFtZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGlmICh0aGlzLmNvbnRyb2xsZXIuZGF0YS5pbi5EZWZhdWx0KVxyXG4gICAgICAgICAgICAgICAgICAgIC8vIDIwMTctMDktMDUgMmRtOiBwcmV2aW91c2x5IHdyb3RlIGl0IHRvIGNvbnRyb2xsZXIuTGlzdCwgYnV0IHRoaXMgaXMgYWxtb3N0IGNlcnRhaW5seSBhIG1pc3Rha2VcclxuICAgICAgICAgICAgICAgICAgICAvLyBzaW5jZSBpdCdzIGFuIG9sZCBBUEkgd2hpY2ggaXMgYmVpbmcgZGVwcmVjYXRlZCwgd2Ugd29uJ3QgZml4IGl0XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5MaXN0ID0gdGhpcy5pbi5EZWZhdWx0Lkxpc3Q7XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHNvdXJjZS5vcmlnU3VjY2VzcylcclxuICAgICAgICAgICAgICAgICAgICBzb3VyY2Uub3JpZ1N1Y2Nlc3ModGhpcyk7XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5jb250cm9sbGVyLmlzTG9hZGVkID0gdHJ1ZTtcclxuICAgICAgICAgICAgICAgIHRoaXMuY29udHJvbGxlci5sYXN0UmVmcmVzaCA9IG5ldyBEYXRlKCk7XHJcbiAgICAgICAgICAgICAgICAodGhpcyBhcyBhbnkpLl90cmlnZ2VyTG9hZGVkKCk7XHJcbiAgICAgICAgICAgIH07XHJcbiAgICAgICAgICAgIHNvdXJjZS5lcnJvciA9IChyZXF1ZXN0OiBhbnkpID0+IHsgYWxlcnQocmVxdWVzdC5zdGF0dXNUZXh0KTsgfTtcclxuICAgICAgICAgICAgc291cmNlLnByZXZlbnRBdXRvRmFpbCA9IHRydWU7IC8vIHVzZSBvdXIgZmFpbCBtZXNzYWdlXHJcbiAgICAgICAgICAgIHRoaXMuc291cmNlID0gc291cmNlO1xyXG4gICAgICAgICAgICByZXR1cm4gdGhpcy5yZWxvYWQoKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVsb2FkKCk6IFN4Y0RhdGFXaXRoSW50ZXJuYWxzIHtcclxuICAgICAgICB0aGlzLmNvbnRyb2xsZXIud2ViQXBpLmdldCh0aGlzLnNvdXJjZSlcclxuICAgICAgICAgICAgLnRoZW4odGhpcy5zb3VyY2Uuc3VjY2VzcywgdGhpcy5zb3VyY2UuZXJyb3IpO1xyXG4gICAgICAgIHJldHVybiB0aGlzO1xyXG4gICAgfVxyXG5cclxuICAgIG9uKGV2ZW50czogRXZlbnQsIGNhbGxiYWNrOiAoKSA9PiB2b2lkKTogUHJvbWlzZTxhbnk+IHtcclxuICAgICAgICByZXR1cm4gJCh0aGlzKS5iaW5kKFwiMnNjTG9hZFwiLCBjYWxsYmFjaylbMF0uX3RyaWdnZXJMb2FkZWQoKTtcclxuICAgIH1cclxuXHJcbiAgICBfdHJpZ2dlckxvYWRlZCgpOiBQcm9taXNlPGFueT4ge1xyXG4gICAgICAgIHJldHVybiB0aGlzLmNvbnRyb2xsZXIuaXNMb2FkZWRcclxuICAgICAgICAgICAgPyAkKHRoaXMpLnRyaWdnZXIoXCIyc2NMb2FkXCIsIFt0aGlzXSlbMF1cclxuICAgICAgICAgICAgOiB0aGlzO1xyXG4gICAgfVxyXG5cclxuICAgIG9uZShldmVudHM6IEV2ZW50LCBjYWxsYmFjazogKHg6IGFueSwgeTogYW55KSA9PiB2b2lkKTogU3hjRGF0YVdpdGhJbnRlcm5hbHMge1xyXG4gICAgICAgIGlmICghdGhpcy5jb250cm9sbGVyLmlzTG9hZGVkKVxyXG4gICAgICAgICAgICByZXR1cm4gJCh0aGlzKS5vbmUoXCIyc2NMb2FkXCIsIGNhbGxiYWNrKVswXTtcclxuICAgICAgICBjYWxsYmFjayh7fSwgdGhpcyk7XHJcbiAgICAgICAgcmV0dXJuIHRoaXM7XHJcbiAgICB9XHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkRhdGEudHMiLCJcclxuZGVjbGFyZSBjb25zdCAkOiBhbnk7XHJcbmltcG9ydCB7IFN4Y0luc3RhbmNlIH0gZnJvbSBcIi4vVG9TaWMuU3hjLkluc3RhbmNlXCI7XHJcblxyXG4vKipcclxuICogaGVscGVyIEFQSSB0byBydW4gYWpheCAvIFJFU1QgY2FsbHMgdG8gdGhlIHNlcnZlclxyXG4gKiBpdCB3aWxsIGVuc3VyZSB0aGF0IHRoZSBoZWFkZXJzIGV0Yy4gYXJlIHNldCBjb3JyZWN0bHlcclxuICogYW5kIHRoYXQgdXJscyBhcmUgcmV3cml0dGVuXHJcbiAqL1xyXG5leHBvcnQgY2xhc3MgU3hjV2ViQXBpV2l0aEludGVybmFscyB7XHJcbiAgICBjb25zdHJ1Y3RvcihcclxuICAgICAgICBwcml2YXRlIHJlYWRvbmx5IGNvbnRyb2xsZXI6IFN4Y0luc3RhbmNlLFxyXG4gICAgICAgIHByaXZhdGUgcmVhZG9ubHkgaWQ6IG51bWJlcixcclxuICAgICAgICBwcml2YXRlIHJlYWRvbmx5IGNiaWQ6IG51bWJlcixcclxuICAgICkge1xyXG5cclxuICAgIH1cclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhbiBodHRwLWdldCBwcm9taXNlXHJcbiAgICAgKiBAcGFyYW0gc2V0dGluZ3NPclVybCB0aGUgdXJsIHRvIGdldFxyXG4gICAgICogQHBhcmFtIHBhcmFtcyBqUXVlcnkgc3R5bGUgYWpheCBwYXJhbWV0ZXJzXHJcbiAgICAgKiBAcGFyYW0gZGF0YSBqUXVlcnkgc3R5bGUgZGF0YSBmb3IgcG9zdC9wdXQgcmVxdWVzdHNcclxuICAgICAqIEBwYXJhbSBwcmV2ZW50QXV0b0ZhaWxcclxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBqUXVlcnkgYWpheCBwcm9taXNlIG9iamVjdFxyXG4gICAgICovXHJcbiAgICBnZXQoc2V0dGluZ3NPclVybDogc3RyaW5nIHwgYW55LCBwYXJhbXM/OiBhbnksIGRhdGE/OiBhbnksIHByZXZlbnRBdXRvRmFpbD86IGJvb2xlYW4pOiBhbnkge1xyXG4gICAgICAgIHJldHVybiB0aGlzLnJlcXVlc3Qoc2V0dGluZ3NPclVybCwgcGFyYW1zLCBkYXRhLCBwcmV2ZW50QXV0b0ZhaWwsIFwiR0VUXCIpO1xyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhbiBodHRwLWdldCBwcm9taXNlXHJcbiAgICAgKiBAcGFyYW0gc2V0dGluZ3NPclVybCB0aGUgdXJsIHRvIGdldFxyXG4gICAgICogQHBhcmFtIHBhcmFtcyBqUXVlcnkgc3R5bGUgYWpheCBwYXJhbWV0ZXJzXHJcbiAgICAgKiBAcGFyYW0gZGF0YSBqUXVlcnkgc3R5bGUgZGF0YSBmb3IgcG9zdC9wdXQgcmVxdWVzdHNcclxuICAgICAqIEBwYXJhbSBwcmV2ZW50QXV0b0ZhaWxcclxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBqUXVlcnkgYWpheCBwcm9taXNlIG9iamVjdFxyXG4gICAgICovXHJcbiAgICBwb3N0KHNldHRpbmdzT3JVcmw6IHN0cmluZyB8IGFueSwgcGFyYW1zPzogYW55LCBkYXRhPzogYW55LCBwcmV2ZW50QXV0b0ZhaWw/OiBib29sZWFuKTogYW55IHtcclxuICAgICAgICByZXR1cm4gdGhpcy5yZXF1ZXN0KHNldHRpbmdzT3JVcmwsIHBhcmFtcywgZGF0YSwgcHJldmVudEF1dG9GYWlsLCBcIlBPU1RcIik7XHJcbiAgICB9XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiByZXR1cm5zIGFuIGh0dHAtZ2V0IHByb21pc2VcclxuICAgICAqIEBwYXJhbSBzZXR0aW5nc09yVXJsIHRoZSB1cmwgdG8gZ2V0XHJcbiAgICAgKiBAcGFyYW0gcGFyYW1zIGpRdWVyeSBzdHlsZSBhamF4IHBhcmFtZXRlcnNcclxuICAgICAqIEBwYXJhbSBkYXRhIGpRdWVyeSBzdHlsZSBkYXRhIGZvciBwb3N0L3B1dCByZXF1ZXN0c1xyXG4gICAgICogQHBhcmFtIHByZXZlbnRBdXRvRmFpbFxyXG4gICAgICogQHJldHVybnMge1Byb21pc2V9IGpRdWVyeSBhamF4IHByb21pc2Ugb2JqZWN0XHJcbiAgICAgKi9cclxuICAgIGRlbGV0ZShzZXR0aW5nc09yVXJsOiBzdHJpbmcgfCBhbnksIHBhcmFtcz86IGFueSwgZGF0YT86IGFueSwgcHJldmVudEF1dG9GYWlsPzogYm9vbGVhbik6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMucmVxdWVzdChzZXR0aW5nc09yVXJsLCBwYXJhbXMsIGRhdGEsIHByZXZlbnRBdXRvRmFpbCwgXCJERUxFVEVcIik7XHJcbiAgICB9XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiByZXR1cm5zIGFuIGh0dHAtZ2V0IHByb21pc2VcclxuICAgICAqIEBwYXJhbSBzZXR0aW5nc09yVXJsIHRoZSB1cmwgdG8gZ2V0XHJcbiAgICAgKiBAcGFyYW0gcGFyYW1zIGpRdWVyeSBzdHlsZSBhamF4IHBhcmFtZXRlcnNcclxuICAgICAqIEBwYXJhbSBkYXRhIGpRdWVyeSBzdHlsZSBkYXRhIGZvciBwb3N0L3B1dCByZXF1ZXN0c1xyXG4gICAgICogQHBhcmFtIHByZXZlbnRBdXRvRmFpbFxyXG4gICAgICogQHJldHVybnMge1Byb21pc2V9IGpRdWVyeSBhamF4IHByb21pc2Ugb2JqZWN0XHJcbiAgICAgKi9cclxuICAgIHB1dChzZXR0aW5nc09yVXJsOiBzdHJpbmcgfCBhbnksIHBhcmFtcz86IGFueSwgZGF0YT86IGFueSwgcHJldmVudEF1dG9GYWlsPzogYm9vbGVhbik6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMucmVxdWVzdChzZXR0aW5nc09yVXJsLCBwYXJhbXMsIGRhdGEsIHByZXZlbnRBdXRvRmFpbCwgXCJQVVRcIik7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZXF1ZXN0KHNldHRpbmdzOiBzdHJpbmcgfCBhbnksIHBhcmFtczogYW55LCBkYXRhOiBhbnksIHByZXZlbnRBdXRvRmFpbDogYm9vbGVhbiwgbWV0aG9kOiBzdHJpbmcpOiBhbnkge1xyXG5cclxuICAgICAgICAvLyB1cmwgcGFyYW1ldGVyOiBhdXRvY29udmVydCBhIHNpbmdsZSB2YWx1ZSAoaW5zdGVhZCBvZiBvYmplY3Qgb2YgdmFsdWVzKSB0byBhbiBpZD0uLi4gcGFyYW1ldGVyXHJcbiAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOmN1cmx5XHJcbiAgICAgICAgaWYgKHR5cGVvZiBwYXJhbXMgIT09IFwib2JqZWN0XCIgJiYgdHlwZW9mIHBhcmFtcyAhPT0gXCJ1bmRlZmluZWRcIilcclxuICAgICAgICAgICAgcGFyYW1zID0geyBpZDogcGFyYW1zIH07XHJcblxyXG4gICAgICAgIC8vIGlmIHRoZSBmaXJzdCBwYXJhbWV0ZXIgaXMgYSBzdHJpbmcsIHJlc29sdmUgc2V0dGluZ3NcclxuICAgICAgICBpZiAodHlwZW9mIHNldHRpbmdzID09PSBcInN0cmluZ1wiKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IGNvbnRyb2xsZXJBY3Rpb24gPSBzZXR0aW5ncy5zcGxpdChcIi9cIik7XHJcbiAgICAgICAgICAgIGNvbnN0IGNvbnRyb2xsZXJOYW1lID0gY29udHJvbGxlckFjdGlvblswXTtcclxuICAgICAgICAgICAgY29uc3QgYWN0aW9uTmFtZSA9IGNvbnRyb2xsZXJBY3Rpb25bMV07XHJcblxyXG4gICAgICAgICAgICBpZiAoY29udHJvbGxlck5hbWUgPT09IFwiXCIgfHwgYWN0aW9uTmFtZSA9PT0gXCJcIilcclxuICAgICAgICAgICAgICAgIGFsZXJ0KFwiRXJyb3I6IGNvbnRyb2xsZXIgb3IgYWN0aW9uIG5vdCBkZWZpbmVkLiBXaWxsIGNvbnRpbnVlIHdpdGggbGlrZWx5IGVycm9ycy5cIik7XHJcblxyXG4gICAgICAgICAgICBzZXR0aW5ncyA9IHtcclxuICAgICAgICAgICAgICAgIGNvbnRyb2xsZXI6IGNvbnRyb2xsZXJOYW1lLFxyXG4gICAgICAgICAgICAgICAgYWN0aW9uOiBhY3Rpb25OYW1lLFxyXG4gICAgICAgICAgICAgICAgcGFyYW1zLFxyXG4gICAgICAgICAgICAgICAgZGF0YSxcclxuICAgICAgICAgICAgICAgIHVybDogY29udHJvbGxlckFjdGlvbi5sZW5ndGggPiAyID8gc2V0dGluZ3MgOiBudWxsLFxyXG4gICAgICAgICAgICAgICAgcHJldmVudEF1dG9GYWlsLFxyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgZGVmYXVsdHMgPSB7XHJcbiAgICAgICAgICAgIG1ldGhvZDogbWV0aG9kID09PSBudWxsID8gXCJQT1NUXCIgOiBtZXRob2QsXHJcbiAgICAgICAgICAgIHBhcmFtczogbnVsbCBhcyBhbnksXHJcbiAgICAgICAgICAgIHByZXZlbnRBdXRvRmFpbDogZmFsc2UsXHJcbiAgICAgICAgfTtcclxuICAgICAgICBzZXR0aW5ncyA9ICQuZXh0ZW5kKHt9LCBkZWZhdWx0cywgc2V0dGluZ3MpO1xyXG4gICAgICAgIGNvbnN0IHNmID0gJC5TZXJ2aWNlc0ZyYW1ld29yayh0aGlzLmlkKTtcclxuXHJcbiAgICAgICAgY29uc3QgcHJvbWlzZSA9ICQuYWpheCh7XHJcbiAgICAgICAgICAgIGFzeW5jOiB0cnVlLFxyXG4gICAgICAgICAgICBkYXRhVHlwZTogc2V0dGluZ3MuZGF0YVR5cGUgfHwgXCJqc29uXCIsIC8vIGRlZmF1bHQgaXMganNvbiBpZiBub3Qgc3BlY2lmaWVkXHJcbiAgICAgICAgICAgIGRhdGE6IEpTT04uc3RyaW5naWZ5KHNldHRpbmdzLmRhdGEpLFxyXG4gICAgICAgICAgICBjb250ZW50VHlwZTogXCJhcHBsaWNhdGlvbi9qc29uXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IHNldHRpbmdzLm1ldGhvZCxcclxuICAgICAgICAgICAgdXJsOiB0aGlzLmdldEFjdGlvblVybChzZXR0aW5ncyksXHJcbiAgICAgICAgICAgIGJlZm9yZVNlbmQoeGhyOiBhbnkpIHtcclxuICAgICAgICAgICAgICAgIHhoci5zZXRSZXF1ZXN0SGVhZGVyKFwiQ29udGVudEJsb2NrSWRcIiwgdGhpcy5jYmlkKTtcclxuICAgICAgICAgICAgICAgIHNmLnNldE1vZHVsZUhlYWRlcnMoeGhyKTtcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgaWYgKCFzZXR0aW5ncy5wcmV2ZW50QXV0b0ZhaWwpXHJcbiAgICAgICAgICAgIHByb21pc2UuZmFpbCh0aGlzLmNvbnRyb2xsZXIuc2hvd0RldGFpbGVkSHR0cEVycm9yKTtcclxuXHJcbiAgICAgICAgcmV0dXJuIHByb21pc2U7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBnZXRBY3Rpb25Vcmwoc2V0dGluZ3M6IGFueSk6IHN0cmluZyB7XHJcbiAgICAgICAgY29uc3Qgc2YgPSAkLlNlcnZpY2VzRnJhbWV3b3JrKHRoaXMuaWQpO1xyXG4gICAgICAgIGNvbnN0IGJhc2UgPSAoc2V0dGluZ3MudXJsKVxyXG4gICAgICAgICAgICA/IHRoaXMuY29udHJvbGxlci5yZXNvbHZlU2VydmljZVVybChzZXR0aW5ncy51cmwpXHJcbiAgICAgICAgICAgIDogc2YuZ2V0U2VydmljZVJvb3QoXCIyc3hjXCIpICsgXCJhcHAvYXV0by9hcGkvXCIgKyBzZXR0aW5ncy5jb250cm9sbGVyICsgXCIvXCIgKyBzZXR0aW5ncy5hY3Rpb247XHJcbiAgICAgICAgcmV0dXJuIGJhc2UgKyAoc2V0dGluZ3MucGFyYW1zID09PSBudWxsID8gXCJcIiA6IChcIj9cIiArICQucGFyYW0oc2V0dGluZ3MucGFyYW1zKSkpO1xyXG4gICAgfVxyXG5cclxufVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi8yc3hjLWFwaS9qcy9Ub1NpYy5TeGMuV2ViQXBpLnRzIiwiXHJcbmV4cG9ydCBjbGFzcyBUb3RhbFBvcHVwIHtcclxuICAgIGZyYW1lOiBhbnkgPSB1bmRlZmluZWQ7XHJcbiAgICBjYWxsYmFjazogYW55ID0gdW5kZWZpbmVkO1xyXG5cclxuICAgIG9wZW4odXJsOiBzdHJpbmcsIGNhbGxiYWNrOiAoKSA9PiB2b2lkKTogdm9pZCB7XHJcbiAgICAgICAgLy8gY291bnQgcGFyZW50cyB0byBzZWUgaG93IGhpZ2ggdGhlIHotaW5kZXggbmVlZHMgdG8gYmVcclxuICAgICAgICBsZXQgeiA9IDEwMDAwMDEwOyAvLyBOZWVkcyBhdCBsZWFzdCAxMDAwMDAwMCB0byBiZSBvbiB0b3Agb2YgdGhlIEROTjkgYmFyXHJcbiAgICAgICAgbGV0IHAgPSB3aW5kb3c7XHJcbiAgICAgICAgd2hpbGUgKHAgIT09IHdpbmRvdy50b3AgJiYgeiA8IDEwMDAwMTAwKSB7XHJcbiAgICAgICAgICAgIHorKztcclxuICAgICAgICAgICAgcCA9IHAucGFyZW50O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3Qgd3JhcHBlciA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJkaXZcIik7XHJcbiAgICAgICAgd3JhcHBlci5zZXRBdHRyaWJ1dGUoXCJzdHlsZVwiLCBcIiB0b3A6IDA7bGVmdDogMDt3aWR0aDogMTAwJTtoZWlnaHQ6IDEwMCU7IHBvc2l0aW9uOmZpeGVkOyB6LWluZGV4OlwiICsgeik7XHJcbiAgICAgICAgZG9jdW1lbnQuYm9keS5hcHBlbmRDaGlsZCh3cmFwcGVyKTtcclxuXHJcbiAgICAgICAgY29uc3QgaWZybSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJpZnJhbWVcIik7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoXCJhbGxvd3RyYW5zcGFyZW5jeVwiLCBcInRydWVcIik7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoXCJzdHlsZVwiLCBcInRvcDogMDtsZWZ0OiAwO3dpZHRoOiAxMDAlO2hlaWdodDogMTAwJTtcIik7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoXCJzcmNcIiwgdXJsKTtcclxuICAgICAgICB3cmFwcGVyLmFwcGVuZENoaWxkKGlmcm0pO1xyXG4gICAgICAgIGRvY3VtZW50LmJvZHkuY2xhc3NOYW1lICs9IFwiIHN4Yy1wb3B1cC1vcGVuXCI7XHJcbiAgICAgICAgdGhpcy5mcmFtZSA9IGlmcm07XHJcbiAgICAgICAgdGhpcy5jYWxsYmFjayA9IGNhbGxiYWNrO1xyXG4gICAgfVxyXG5cclxuICAgIGNsb3NlKCk6IHZvaWQge1xyXG4gICAgICAgIGlmICh0aGlzLmZyYW1lKSB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmJvZHkuY2xhc3NOYW1lID0gZG9jdW1lbnQuYm9keS5jbGFzc05hbWUucmVwbGFjZShcInN4Yy1wb3B1cC1vcGVuXCIsIFwiXCIpO1xyXG4gICAgICAgICAgICBjb25zdCBmcm0gPSB0aGlzLmZyYW1lO1xyXG4gICAgICAgICAgICBmcm0ucGFyZW50Tm9kZS5wYXJlbnROb2RlLnJlbW92ZUNoaWxkKGZybS5wYXJlbnROb2RlKTtcclxuICAgICAgICAgICAgdGhpcy5jYWxsYmFjaygpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBjbG9zZVRoaXMoKTogdm9pZCB7XHJcbiAgICAgICAgKHdpbmRvdy5wYXJlbnQgYXMgYW55KS4kMnN4Yy50b3RhbFBvcHVwLmNsb3NlKCk7XHJcbiAgICB9XHJcblxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5Ub3RhbFBvcHVwLnRzIiwiXHJcbiAgICBleHBvcnQgY2xhc3MgVXJsUGFyYW1NYW5hZ2VyIHtcclxuICAgICAgICBnZXQobmFtZTogc3RyaW5nKSB7XHJcbiAgICAgICAgICAgIC8vIHdhcm5pbmc6IHRoaXMgbWV0aG9kIGlzIGR1cGxpY2F0ZWQgaW4gMiBwbGFjZXMgLSBrZWVwIHRoZW0gaW4gc3luYy5cclxuICAgICAgICAgICAgLy8gbG9jYXRpb25zIGFyZSBlYXYgYW5kIDJzeGM0bmdcclxuICAgICAgICAgICAgbmFtZSA9IG5hbWUucmVwbGFjZSgvW1xcW10vLCBcIlxcXFxbXCIpLnJlcGxhY2UoL1tcXF1dLywgXCJcXFxcXVwiKTtcclxuICAgICAgICAgICAgY29uc3Qgc2VhcmNoUnggPSBuZXcgUmVnRXhwKFwiW1xcXFw/Jl1cIiArIG5hbWUgKyBcIj0oW14mI10qKVwiLCBcImlcIik7XHJcbiAgICAgICAgICAgIGxldCByZXN1bHRzID0gc2VhcmNoUnguZXhlYyhsb2NhdGlvbi5zZWFyY2gpO1xyXG4gICAgICAgICAgICBsZXQgc3RyUmVzdWx0OiBzdHJpbmc7XHJcblxyXG4gICAgICAgICAgICBpZiAocmVzdWx0cyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgY29uc3QgaGFzaFJ4ID0gbmV3IFJlZ0V4cChcIlsjJl1cIiArIG5hbWUgKyBcIj0oW14mI10qKVwiLCBcImlcIik7XHJcbiAgICAgICAgICAgICAgICByZXN1bHRzID0gaGFzaFJ4LmV4ZWMobG9jYXRpb24uaGFzaCk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIC8vIGlmIG5vdGhpbmcgZm91bmQsIHRyeSBub3JtYWwgVVJMIGJlY2F1c2UgRE5OIHBsYWNlcyBwYXJhbWV0ZXJzIGluIC9rZXkvdmFsdWUgbm90YXRpb25cclxuICAgICAgICAgICAgaWYgKHJlc3VsdHMgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIC8vIE90aGVyd2lzZSB0cnkgcGFydHMgb2YgdGhlIFVSTFxyXG4gICAgICAgICAgICAgICAgY29uc3QgbWF0Y2hlcyA9IHdpbmRvdy5sb2NhdGlvbi5wYXRobmFtZS5tYXRjaChuZXcgUmVnRXhwKFwiL1wiICsgbmFtZSArIFwiLyhbXi9dKylcIiwgXCJpXCIpKTtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBDaGVjayBpZiB3ZSBmb3VuZCBhbnl0aGluZywgaWYgd2UgZG8gZmluZCBpdCwgd2UgbXVzdCByZXZlcnNlIHRoZVxyXG4gICAgICAgICAgICAgICAgLy8gcmVzdWx0cyBzbyB3ZSBnZXQgdGhlIFwibGFzdFwiIG9uZSBpbiBjYXNlIHRoZXJlIGFyZSBtdWx0aXBsZSBoaXRzXHJcbiAgICAgICAgICAgICAgICBpZiAobWF0Y2hlcyAmJiBtYXRjaGVzLmxlbmd0aCA+IDEpXHJcbiAgICAgICAgICAgICAgICAgICAgc3RyUmVzdWx0ID0gbWF0Y2hlcy5yZXZlcnNlKClbMF07XHJcbiAgICAgICAgICAgIH0gZWxzZVxyXG4gICAgICAgICAgICAgICAgc3RyUmVzdWx0ID0gcmVzdWx0c1sxXTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBzdHJSZXN1bHQgPT09IG51bGwgfHwgc3RyUmVzdWx0ID09PSB1bmRlZmluZWRcclxuICAgICAgICAgICAgICAgID8gXCJcIlxyXG4gICAgICAgICAgICAgICAgOiBkZWNvZGVVUklDb21wb25lbnQoc3RyUmVzdWx0LnJlcGxhY2UoL1xcKy9nLCBcIiBcIikpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVxdWlyZShuYW1lOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgY29uc3QgZm91bmQgPSB0aGlzLmdldChuYW1lKTtcclxuICAgICAgICAgICAgaWYgKGZvdW5kID09PSBcIlwiKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBtZXNzYWdlID0gYFJlcXVpcmVkIHBhcmFtZXRlciAoJHtuYW1lfSkgbWlzc2luZyBmcm9tIHVybCAtIGNhbm5vdCBjb250aW51ZWA7XHJcbiAgICAgICAgICAgICAgICBhbGVydChtZXNzYWdlKTtcclxuICAgICAgICAgICAgICAgIHRocm93IG1lc3NhZ2U7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcmV0dXJuIGZvdW5kO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlVybC50cyJdLCJzb3VyY2VSb290IjoiIn0=