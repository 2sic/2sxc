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
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__Stats__ = __webpack_require__(7);




var sxcVersion = '10.24.01';
function SxcController(id, cbid) {
    var $2sxc = window.$2sxc;
    if (!$2sxc._controllers)
        throw new Error('$2sxc not initialized yet');
    if (typeof id === 'object') {
        var idTuple = autoFind(id);
        id = idTuple[0];
        cbid = idTuple[1];
    }
    if (!cbid)
        cbid = id;
    var cacheKey = id + ':' + cbid;
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
        load: (urlManager.get('debug') === 'true'),
        uncache: urlManager.get('sxcver'),
    };
    var stats = new __WEBPACK_IMPORTED_MODULE_3__Stats__["a" /* Stats */]();
    var addOn = {
        _controllers: {},
        sysinfo: {
            version: sxcVersion,
            description: 'The 2sxc Controller object - read more about it on docs.2sxc.org',
        },
        beta: {},
        _data: {},
        totalPopup: new __WEBPACK_IMPORTED_MODULE_1__ToSic_Sxc_TotalPopup__["a" /* TotalPopup */](),
        urlParams: urlManager,
        debug: debug,
        stats: stats,
        parts: {
            getUrl: function (url, preventUnmin) {
                var r = (preventUnmin || !debug.load) ? url : url.replace('.min', '');
                if (debug.uncache && r.indexOf('sxcver') === -1)
                    r = r + ((r.indexOf('?') === -1) ? '?' : '&') + 'sxcver=' + debug.uncache;
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
    var containerTag = $(domElement).closest('.sc-content-block')[0];
    if (!containerTag)
        return null;
    var iid = containerTag.getAttribute('data-cb-instance');
    var cbid = containerTag.getAttribute('data-cb-id');
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
        this.serviceScopes = ['app', 'app-sys', 'app-api', 'app-query', 'app-content', 'eav', 'view', 'dnn'];
        this.serviceRoot = dnnSf(id).getServiceRoot('2sxc');
        this.webApi = new __WEBPACK_IMPORTED_MODULE_1__ToSic_Sxc_WebApi__["a" /* SxcWebApiWithInternals */](this, id, cbid);
    }
    SxcInstance.prototype.resolveServiceUrl = function (virtualPath) {
        var scope = virtualPath.split('/')[0].toLowerCase();
        if (this.serviceScopes.indexOf(scope) === -1)
            return virtualPath;
        return this.serviceRoot + scope + '/' + virtualPath.substring(virtualPath.indexOf('/') + 1);
    };
    SxcInstance.prototype.showDetailedHttpError = function (result) {
        if (window.console)
            console.log(result);
        if (result.status === 404 &&
            result.config &&
            result.config.url &&
            result.config.url.indexOf('/dist/i18n/') > -1) {
            if (window.console)
                console.log('just fyi: failed to load language resource; will have to use default');
            return result;
        }
        if (result.status === 0 || result.status === -1)
            return result;
        var infoText = 'Had an error talking to the server (status ' + result.status + ').';
        var srvResp = result.responseText
            ? JSON.parse(result.responseText)
            : result.data;
        if (srvResp) {
            var msg = srvResp.Message;
            if (msg)
                infoText += '\nMessage: ' + msg;
            var msgDet = srvResp.MessageDetail || srvResp.ExceptionMessage;
            if (msgDet)
                infoText += '\nDetail: ' + msgDet;
            if (msgDet && msgDet.indexOf('No action was found') === 0)
                if (msgDet.indexOf('that matches the name') > 0)
                    infoText += '\n\nTip from 2sxc: you probably got the action-name wrong in your JS.';
                else if (msgDet.indexOf('that matches the request.') > 0)
                    infoText += '\n\nTip from 2sxc: Seems like the parameters are the wrong amount or type.';
            if (msg && msg.indexOf('Controller') === 0 && msg.indexOf('not found') > 0)
                infoText +=
                    "\n\nTip from 2sxc: you probably spelled the controller name wrong or forgot to remove the word 'controller' from the call in JS. To call a controller called 'DemoController' only use 'Demo'.";
        }
        infoText += '\n\nif you are an advanced user you can learn more about what went wrong - discover how on 2sxc.org/help?tag=debug';
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
        var url = this.controller.resolveServiceUrl('app-sys/appcontent/GetContentBlockData');
        if (typeof params === 'string')
            url += '&' + params;
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
        return $(this).bind('2scLoad', callback)[0]._triggerLoaded();
    };
    SxcDataWithInternals.prototype._triggerLoaded = function () {
        return this.controller.isLoaded
            ? $(this).trigger('2scLoad', [this])[0]
            : this;
    };
    SxcDataWithInternals.prototype.one = function (events, callback) {
        if (!this.controller.isLoaded)
            return $(this).one('2scLoad', callback)[0];
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
        return this.request(settingsOrUrl, params, data, preventAutoFail, 'GET');
    };
    SxcWebApiWithInternals.prototype.post = function (settingsOrUrl, params, data, preventAutoFail) {
        return this.request(settingsOrUrl, params, data, preventAutoFail, 'POST');
    };
    SxcWebApiWithInternals.prototype.delete = function (settingsOrUrl, params, data, preventAutoFail) {
        return this.request(settingsOrUrl, params, data, preventAutoFail, 'DELETE');
    };
    SxcWebApiWithInternals.prototype.put = function (settingsOrUrl, params, data, preventAutoFail) {
        return this.request(settingsOrUrl, params, data, preventAutoFail, 'PUT');
    };
    SxcWebApiWithInternals.prototype.request = function (settings, params, data, preventAutoFail, method) {
        if (typeof params !== 'object' && typeof params !== 'undefined')
            params = { id: params };
        if (typeof settings === 'string') {
            var controllerAction = settings.split('/');
            var controllerName = controllerAction[0];
            var actionName = controllerAction[1];
            if (controllerName === '' || actionName === '')
                alert('Error: controller or action not defined. Will continue with likely errors.');
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
            method: method === null ? 'POST' : method,
            params: null,
            preventAutoFail: false,
        };
        settings = $.extend({}, defaults, settings);
        var sf = $.ServicesFramework(this.id);
        var cbid = this.cbid;
        var promise = $.ajax({
            async: true,
            dataType: settings.dataType || 'json',
            data: JSON.stringify(settings.data),
            contentType: 'application/json',
            type: settings.method,
            url: this.getActionUrl(settings),
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ContentBlockId', cbid);
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
            : sf.getServiceRoot('2sxc') + 'app/auto/api/' + settings.controller + '/' + settings.action;
        return base + (settings.params === null ? '' : ('?' + $.param(settings.params)));
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
        var wrapper = document.createElement('div');
        wrapper.setAttribute('style', ' top: 0;left: 0;width: 100%;height: 100%; position:fixed; z-index:' + z);
        document.body.appendChild(wrapper);
        var ifrm = document.createElement('iframe');
        ifrm.setAttribute('allowtransparency', 'true');
        ifrm.setAttribute('style', 'top: 0;left: 0;width: 100%;height: 100%;');
        ifrm.setAttribute('src', url);
        wrapper.appendChild(ifrm);
        document.body.className += ' sxc-popup-open';
        this.frame = ifrm;
        this.callback = callback;
    };
    TotalPopup.prototype.close = function () {
        if (this.frame) {
            document.body.className = document.body.className.replace('sxc-popup-open', '');
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
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var searchRx = new RegExp('[\\?&]' + name + '=([^&#]*)', 'i');
        var results = searchRx.exec(location.search);
        var strResult;
        if (results === null) {
            var hashRx = new RegExp('[#&]' + name + '=([^&#]*)', 'i');
            results = hashRx.exec(location.hash);
        }
        if (results === null) {
            var matches = window.location.pathname.match(new RegExp('/' + name + '/([^/]+)', 'i'));
            if (matches && matches.length > 1)
                strResult = matches.reverse()[0];
        }
        else
            strResult = results[1];
        return strResult === null || strResult === undefined
            ? ''
            : decodeURIComponent(strResult.replace(/\+/g, ' '));
    };
    UrlParamManager.prototype.require = function (name) {
        var found = this.get(name);
        if (found === '') {
            var message = "Required parameter (" + name + ") missing from url - cannot continue";
            alert(message);
            throw message;
        }
        return found;
    };
    return UrlParamManager;
}());



/***/ }),
/* 7 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Stats; });
var Stats = (function () {
    function Stats() {
        this.watchDomChanges = 0;
    }
    return Stats;
}());



/***/ })
/******/ ]);
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAgODNhNGYyZmYyYzQyNmE3MDhkOTgiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvMnN4Yy5hcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkNvbnRyb2xsZXIudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkluc3RhbmNlLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5EYXRhLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5XZWJBcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlRvdGFsUG9wdXAudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlVybC50cyIsIndlYnBhY2s6Ly8vLi8yc3hjLWFwaS9qcy9TdGF0cy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUE7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7OztBQUdBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLGFBQUs7QUFDTDtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBLG1DQUEyQiwwQkFBMEIsRUFBRTtBQUN2RCx5Q0FBaUMsZUFBZTtBQUNoRDtBQUNBO0FBQ0E7O0FBRUE7QUFDQSw4REFBc0QsK0RBQStEOztBQUVySDtBQUNBOztBQUVBO0FBQ0E7Ozs7Ozs7Ozs7QUN4RG9FO0FBS3BFLElBQUksQ0FBQyxNQUFNLENBQUMsS0FBSztJQUNiLE1BQU0sQ0FBQyxLQUFLLEdBQUcseUZBQWtCLEVBQUUsQ0FBQzs7Ozs7Ozs7Ozs7OztBQ1Q2RDtBQUNqRDtBQUNGO0FBQ2xCO0FB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QUVsQixJQUFNLE9BQU8sR0FBRyxNQUFNLENBQUMsUUFBUSxDQUFDLFFBQVEsQ0FBQyxLQUFLLENBQUMsSUFBSSxNQUFNLENBQUMsR0FBRyxHQUFHLElBQUksR0FBRyxVQUFVLEVBQUUsR0FBRyxDQUFDLENBQUMsQ0FBQztZQUl6RixJQUFJLE9BQU8sSUFBSSxPQUFPLENBQUMsTUFBTSxHQUFHLENBQUM7Z0JBQzdCLFNBQVMsR0FBRyxPQUFPLENBQUMsT0FBTyxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUM7U0FDeEM7O1lBQ0csU0FBUyxHQUFHLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUUzQixPQUFPLFNBQVMsS0FBSyxJQUFJLElBQUksU0FBUyxLQUFLLFNBQVM7WUFDaEQsQ0FBQyxDQUFDLEVBQUU7WUFDSixDQUFDLENBQUMsa0JBQWtCLENBQUMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxLQUFLLEVBQUUsR0FBRyxDQUFDLENBQUMsQ0FBQztJQUM1RCxDQUFDO0lBRUQsaUNBQU8sR0FBUCxVQUFRLElBQVk7UUFDaEIsSUFBTSxLQUFLLEdBQUcsSUFBSSxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsQ0FBQztRQUM3QixJQUFJLEtBQUssS0FBSyxFQUFFLEVBQUU7WUFDZCxJQUFNLE9BQU8sR0FBRyx5QkFBdUIsSUFBSSx5Q0FBc0MsQ0FBQztZQUNsRixLQUFLLENBQUMsT0FBTyxDQUFDLENBQUM7WUFDZixNQUFNLE9BQU8sQ0FBQztTQUNqQjtRQUNELE9BQU8sS0FBSyxDQUFDO0lBQ2pCLENBQUM7SUFDTCxzQkFBQztBQUFELENBQUM7Ozs7Ozs7OztBQ3pDTDtBQUFBO0lBQUE7UUFDSSxvQkFBZSxHQUFHLENBQUMsQ0FBQztJQUN4QixDQUFDO0lBQUQsWUFBQztBQUFELENBQUMiLCJmaWxlIjoiMnN4Yy5hcGkuanMiLCJzb3VyY2VzQ29udGVudCI6WyIgXHQvLyBUaGUgbW9kdWxlIGNhY2hlXG4gXHR2YXIgaW5zdGFsbGVkTW9kdWxlcyA9IHt9O1xuXG4gXHQvLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuIFx0ZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXG4gXHRcdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuIFx0XHRpZihpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSkge1xuIFx0XHRcdHJldHVybiBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXS5leHBvcnRzO1xuIFx0XHR9XG4gXHRcdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG4gXHRcdHZhciBtb2R1bGUgPSBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSA9IHtcbiBcdFx0XHRpOiBtb2R1bGVJZCxcbiBcdFx0XHRsOiBmYWxzZSxcbiBcdFx0XHRleHBvcnRzOiB7fVxuIFx0XHR9O1xuXG4gXHRcdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuIFx0XHRtb2R1bGVzW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuIFx0XHQvLyBGbGFnIHRoZSBtb2R1bGUgYXMgbG9hZGVkXG4gXHRcdG1vZHVsZS5sID0gdHJ1ZTtcblxuIFx0XHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuIFx0XHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG4gXHR9XG5cblxuIFx0Ly8gZXhwb3NlIHRoZSBtb2R1bGVzIG9iamVjdCAoX193ZWJwYWNrX21vZHVsZXNfXylcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubSA9IG1vZHVsZXM7XG5cbiBcdC8vIGV4cG9zZSB0aGUgbW9kdWxlIGNhY2hlXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLmMgPSBpbnN0YWxsZWRNb2R1bGVzO1xuXG4gXHQvLyBkZWZpbmUgZ2V0dGVyIGZ1bmN0aW9uIGZvciBoYXJtb255IGV4cG9ydHNcbiBcdF9fd2VicGFja19yZXF1aXJlX18uZCA9IGZ1bmN0aW9uKGV4cG9ydHMsIG5hbWUsIGdldHRlcikge1xuIFx0XHRpZighX193ZWJwYWNrX3JlcXVpcmVfXy5vKGV4cG9ydHMsIG5hbWUpKSB7XG4gXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIG5hbWUsIHtcbiBcdFx0XHRcdGNvbmZpZ3VyYWJsZTogZmFsc2UsXG4gXHRcdFx0XHRlbnVtZXJhYmxlOiB0cnVlLFxuIFx0XHRcdFx0Z2V0OiBnZXR0ZXJcbiBcdFx0XHR9KTtcbiBcdFx0fVxuIFx0fTtcblxuIFx0Ly8gZ2V0RGVmYXVsdEV4cG9ydCBmdW5jdGlvbiBmb3IgY29tcGF0aWJpbGl0eSB3aXRoIG5vbi1oYXJtb255IG1vZHVsZXNcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubiA9IGZ1bmN0aW9uKG1vZHVsZSkge1xuIFx0XHR2YXIgZ2V0dGVyID0gbW9kdWxlICYmIG1vZHVsZS5fX2VzTW9kdWxlID9cbiBcdFx0XHRmdW5jdGlvbiBnZXREZWZhdWx0KCkgeyByZXR1cm4gbW9kdWxlWydkZWZhdWx0J107IH0gOlxuIFx0XHRcdGZ1bmN0aW9uIGdldE1vZHVsZUV4cG9ydHMoKSB7IHJldHVybiBtb2R1bGU7IH07XG4gXHRcdF9fd2VicGFja19yZXF1aXJlX18uZChnZXR0ZXIsICdhJywgZ2V0dGVyKTtcbiBcdFx0cmV0dXJuIGdldHRlcjtcbiBcdH07XG5cbiBcdC8vIE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbFxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5vID0gZnVuY3Rpb24ob2JqZWN0LCBwcm9wZXJ0eSkgeyByZXR1cm4gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eS5jYWxsKG9iamVjdCwgcHJvcGVydHkpOyB9O1xuXG4gXHQvLyBfX3dlYnBhY2tfcHVibGljX3BhdGhfX1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5wID0gXCJcIjtcblxuIFx0Ly8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4gXHRyZXR1cm4gX193ZWJwYWNrX3JlcXVpcmVfXyhfX3dlYnBhY2tfcmVxdWlyZV9fLnMgPSAwKTtcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyB3ZWJwYWNrL2Jvb3RzdHJhcCA4M2E0ZjJmZjJjNDI2YTcwOGQ5OCIsIi8vIHRoaXMgaXMgdGhlIDJzeGMtamF2YXNjcmlwdCBBUElcclxuLy8gMnN4YyB3aWxsIGluY2x1ZGUgdGhpcyBhdXRvbWF0aWNhbGx5IHdoZW4gYSB1c2VyIGhhcyBlZGl0LXJpZ2h0c1xyXG4vLyBhIHRlbXBsYXRlIGRldmVsb3BlciB3aWxsIHR5cGljYWxseSB1c2UgdGhpcyB0byB1c2UgdGhlIGRhdGEtYXBpIHRvIHJlYWQgMnN4Yy1kYXRhIGZyb20gdGhlIHNlcnZlclxyXG4vLyByZWFkIG1vcmUgYWJvdXQgdGhpcyBpbiB0aGUgd2lraTogaHR0cHM6Ly9naXRodWIuY29tLzJzaWMvMnN4Yy93aWtpL0phdmFTY3JpcHQtJTI0MnN4Y1xyXG5cclxuaW1wb3J0IHsgYnVpbGRTeGNDb250cm9sbGVyLCBXaW5kb3cgfSBmcm9tIFwiLi9Ub1NpYy5TeGMuQ29udHJvbGxlclwiO1xyXG5cclxuLy8gUmVTaGFycGVyIGRpc2FibGUgSW5jb25zaXN0ZW50TmFtaW5nXHJcbmRlY2xhcmUgY29uc3Qgd2luZG93OiBXaW5kb3c7XHJcblxyXG5pZiAoIXdpbmRvdy4kMnN4YykgLy8gcHJldmVudCBkb3VibGUgZXhlY3V0aW9uXHJcbiAgICB3aW5kb3cuJDJzeGMgPSBidWlsZFN4Y0NvbnRyb2xsZXIoKTtcclxuXHJcbi8vIFJlU2hhcnBlciByZXN0b3JlIEluY29uc2lzdGVudE5hbWluZ1xyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi8yc3hjLWFwaS9qcy8yc3hjLmFwaS50cyIsIi8vIFJlU2hhcnBlciBkaXNhYmxlIEluY29uc2lzdGVudE5hbWluZ1xyXG5cclxuaW1wb3J0IHsgU3hjSW5zdGFuY2UsIFN4Y0luc3RhbmNlV2l0aEVkaXRpbmcsIFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB9IGZyb20gJy4vVG9TaWMuU3hjLkluc3RhbmNlJztcclxuaW1wb3J0IHsgVG90YWxQb3B1cCB9IGZyb20gJy4vVG9TaWMuU3hjLlRvdGFsUG9wdXAnO1xyXG5pbXBvcnQgeyBVcmxQYXJhbU1hbmFnZXIgfSBmcm9tICcuL1RvU2ljLlN4Yy5VcmwnO1xyXG5pbXBvcnQgeyBTdGF0cyB9IGZyb20gJy4vU3RhdHMnO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBXaW5kb3cgeyAkMnN4YzogU3hjQ29udHJvbGxlciB8IFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzOyB9XHJcblxyXG5kZWNsYXJlIGNvbnN0ICQ6IGFueTtcclxuZGVjbGFyZSBjb25zdCB3aW5kb3c6IFdpbmRvdztcclxuY29uc3Qgc3hjVmVyc2lvbiA9ICcxMC4yNC4wMSc7XHJcblxyXG4vKipcclxuICogVGhpcyBpcyB0aGUgaW50ZXJmYWNlIGZvciB0aGUgbWFpbiAkMnN4YyBvYmplY3Qgb24gdGhlIHdpbmRvd1xyXG4gKi9cclxuZXhwb3J0IGludGVyZmFjZSBTeGNDb250cm9sbGVyIHtcclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhIDJzeGMtaW5zdGFuY2Ugb2YgdGhlIGlkIG9yIGh0bWwtdGFnIHBhc3NlZCBpblxyXG4gICAgICogQHBhcmFtIGlkXHJcbiAgICAgKiBAcGFyYW0gY2JpZFxyXG4gICAgICogQHJldHVybnMge31cclxuICAgICAqL1xyXG4gICAgKGlkOiBudW1iZXIgfCBIVE1MRWxlbWVudCwgY2JpZD86IG51bWJlcik6IFN4Y0luc3RhbmNlIHwgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzLFxyXG5cclxuICAgIC8qKlxyXG4gICAgICogc3lzdGVtIGluZm9ybWF0aW9uLCBtYWlubHkgZm9yIGNoZWNraW5nIHdoaWNoIHZlcnNpb24gb2YgMnN4YyBpcyBydW5uaW5nXHJcbiAgICAgKiBub3RlOiBpdCdzIG5vdCBhbHdheXMgdXBkYXRlZCByZWxpYWJseSwgYnV0IGl0IGhlbHBzIHdoZW4gZGVidWdnaW5nXHJcbiAgICAgKi9cclxuICAgIHN5c2luZm86IHtcclxuICAgICAgICAvKipcclxuICAgICAgICAgKiB0aGUgdmVyc2lvbiB1c2luZyB0aGUgIyMuIyMuIyMgc3ludGF4XHJcbiAgICAgICAgICovXHJcbiAgICAgICAgdmVyc2lvbjogc3RyaW5nLFxyXG5cclxuICAgICAgICAvKipcclxuICAgICAgICAgKiBhIHNob3J0IHRleHQgZGVzY3JpcHRpb24sIGZvciBwZW9wbGUgd2hvIGhhdmUgbm8gaWRlYSB3aGF0IHRoaXMgaXNcclxuICAgICAgICAgKi9cclxuICAgICAgICBkZXNjcmlwdGlvbjogc3RyaW5nLFxyXG4gICAgfTtcclxufVxyXG5cclxuLyoqXHJcbiAqIHJldHVybnMgYSAyc3hjLWluc3RhbmNlIG9mIHRoZSBpZCBvciBodG1sLXRhZyBwYXNzZWQgaW5cclxuICogQHBhcmFtIGlkXHJcbiAqIEBwYXJhbSBjYmlkXHJcbiAqIEByZXR1cm5zIHt9XHJcbiAqL1xyXG5mdW5jdGlvbiBTeGNDb250cm9sbGVyKGlkOiBudW1iZXIgfCBIVE1MRWxlbWVudCwgY2JpZD86IG51bWJlcik6IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB7XHJcbiAgICBjb25zdCAkMnN4YyA9IHdpbmRvdy4kMnN4YyBhcyBTeGNDb250cm9sbGVyV2l0aEludGVybmFscztcclxuICAgIGlmICghJDJzeGMuX2NvbnRyb2xsZXJzKVxyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcignJDJzeGMgbm90IGluaXRpYWxpemVkIHlldCcpO1xyXG5cclxuICAgIC8vIGlmIGl0J3MgYSBkb20tZWxlbWVudCwgdXNlIGF1dG8tZmluZFxyXG4gICAgaWYgKHR5cGVvZiBpZCA9PT0gJ29iamVjdCcpIHtcclxuICAgICAgICBjb25zdCBpZFR1cGxlID0gYXV0b0ZpbmQoaWQpO1xyXG4gICAgICAgIGlkID0gaWRUdXBsZVswXTtcclxuICAgICAgICBjYmlkID0gaWRUdXBsZVsxXTtcclxuICAgIH1cclxuXHJcbiAgICBpZiAoIWNiaWQpIGNiaWQgPSBpZDsgICAgICAgICAgIC8vIGlmIGNvbnRlbnQtYmxvY2sgaXMgdW5rbm93biwgdXNlIGlkIG9mIG1vZHVsZVxyXG4gICAgY29uc3QgY2FjaGVLZXkgPSBpZCArICc6JyArIGNiaWQ7IC8vIG5ldXRyYWxpemUgdGhlIGlkIGZyb20gb2xkIFwiMzRcIiBmb3JtYXQgdG8gdGhlIG5ldyBcIjM1OjM1M1wiIGZvcm1hdFxyXG5cclxuICAgIC8vIGVpdGhlciBnZXQgdGhlIGNhY2hlZCBjb250cm9sbGVyIGZyb20gcHJldmlvdXMgY2FsbHMsIG9yIGNyZWF0ZSBhIG5ldyBvbmVcclxuICAgIGlmICgkMnN4Yy5fY29udHJvbGxlcnNbY2FjaGVLZXldKSByZXR1cm4gJDJzeGMuX2NvbnRyb2xsZXJzW2NhY2hlS2V5XTtcclxuXHJcbiAgICAvLyBhbHNvIGluaXQgdGhlIGRhdGEtY2FjaGUgaW4gY2FzZSBpdCdzIGV2ZXIgbmVlZGVkXHJcbiAgICBpZiAoISQyc3hjLl9kYXRhW2NhY2hlS2V5XSkgJDJzeGMuX2RhdGFbY2FjaGVLZXldID0ge307XHJcblxyXG4gICAgcmV0dXJuICgkMnN4Yy5fY29udHJvbGxlcnNbY2FjaGVLZXldXHJcbiAgICAgICAgPSBuZXcgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzKGlkLCBjYmlkLCBjYWNoZUtleSwgJDJzeGMsICQuU2VydmljZXNGcmFtZXdvcmspKTtcclxufVxyXG5cclxuLyoqXHJcbiAqIEJ1aWxkIGEgU1hDIENvbnRyb2xsZXIgZm9yIHRoZSBwYWdlLiBTaG91bGQgb25seSBldmVyIGJlIGV4ZWN1dGVkIG9uY2VcclxuICovXHJcbmV4cG9ydCBmdW5jdGlvbiBidWlsZFN4Y0NvbnRyb2xsZXIoKTogU3hjQ29udHJvbGxlciB8IFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzIHtcclxuICAgIGNvbnN0IHVybE1hbmFnZXIgPSBuZXcgVXJsUGFyYW1NYW5hZ2VyKCk7XHJcbiAgICBjb25zdCBkZWJ1ZyA9IHtcclxuICAgICAgICBsb2FkOiAodXJsTWFuYWdlci5nZXQoJ2RlYnVnJykgPT09ICd0cnVlJyksXHJcbiAgICAgICAgdW5jYWNoZTogdXJsTWFuYWdlci5nZXQoJ3N4Y3ZlcicpLFxyXG4gICAgfTtcclxuICAgIGNvbnN0IHN0YXRzID0gbmV3IFN0YXRzKCk7XHJcblxyXG4gICAgY29uc3QgYWRkT246IGFueSA9IHtcclxuICAgICAgICBfY29udHJvbGxlcnM6IHt9IGFzIGFueSxcclxuICAgICAgICBzeXNpbmZvOiB7XHJcbiAgICAgICAgICAgIHZlcnNpb246IHN4Y1ZlcnNpb24sXHJcbiAgICAgICAgICAgIGRlc2NyaXB0aW9uOiAnVGhlIDJzeGMgQ29udHJvbGxlciBvYmplY3QgLSByZWFkIG1vcmUgYWJvdXQgaXQgb24gZG9jcy4yc3hjLm9yZycsXHJcbiAgICAgICAgfSxcclxuICAgICAgICBiZXRhOiB7fSxcclxuICAgICAgICBfZGF0YToge30sXHJcbiAgICAgICAgLy8gdGhpcyBjcmVhdGVzIGEgZnVsbC1zY3JlZW4gaWZyYW1lLXBvcHVwIGFuZCBwcm92aWRlcyBhIGNsb3NlLWNvbW1hbmQgdG8gZmluaXNoIHRoZSBkaWFsb2cgYXMgbmVlZGVkXHJcbiAgICAgICAgdG90YWxQb3B1cDogbmV3IFRvdGFsUG9wdXAoKSxcclxuICAgICAgICB1cmxQYXJhbXM6IHVybE1hbmFnZXIsXHJcbiAgICAgICAgLy8gbm90ZTogSSB3b3VsZCBsaWtlIHRvIHJlbW92ZSB0aGlzIGZyb20gJDJzeGMsIGJ1dCBpdCdzIGN1cnJlbnRseVxyXG4gICAgICAgIC8vIHVzZWQgYm90aCBpbiB0aGUgaW5wYWdlLWVkaXQgYW5kIGluIHRoZSBkaWFsb2dzXHJcbiAgICAgICAgLy8gZGVidWcgc3RhdGUgd2hpY2ggaXMgbmVlZGVkIGluIHZhcmlvdXMgcGxhY2VzXHJcbiAgICAgICAgZGVidWcsXHJcbiAgICAgICAgc3RhdHM6IHN0YXRzLFxyXG4gICAgICAgIC8vIG1pbmktaGVscGVycyB0byBtYW5hZ2UgMnN4YyBwYXJ0cywgYSBiaXQgbGlrZSBhIGRlcGVuZGVuY3kgbG9hZGVyXHJcbiAgICAgICAgLy8gd2hpY2ggd2lsbCBvcHRpbWl6ZSB0byBsb2FkIG1pbi9tYXggZGVwZW5kaW5nIG9uIGRlYnVnIHN0YXRlXHJcbiAgICAgICAgcGFydHM6IHtcclxuICAgICAgICAgICAgZ2V0VXJsKHVybDogc3RyaW5nLCBwcmV2ZW50VW5taW46IGJvb2xlYW4pIHtcclxuICAgICAgICAgICAgICAgIGxldCByID0gKHByZXZlbnRVbm1pbiB8fCAhZGVidWcubG9hZCkgPyB1cmwgOiB1cmwucmVwbGFjZSgnLm1pbicsICcnKTsgLy8gdXNlIG1pbiBvciBub3RcclxuICAgICAgICAgICAgICAgIGlmIChkZWJ1Zy51bmNhY2hlICYmIHIuaW5kZXhPZignc3hjdmVyJykgPT09IC0xKVxyXG4gICAgICAgICAgICAgICAgICAgIHIgPSByICsgKChyLmluZGV4T2YoJz8nKSA9PT0gLTEpID8gJz8nIDogJyYnKSArICdzeGN2ZXI9JyArIGRlYnVnLnVuY2FjaGU7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gcjtcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICB9LFxyXG4gICAgfTtcclxuICAgIGZvciAoY29uc3QgcHJvcGVydHkgaW4gYWRkT24pXHJcbiAgICAgICAgaWYgKGFkZE9uLmhhc093blByb3BlcnR5KHByb3BlcnR5KSlcclxuICAgICAgICAgICAgU3hjQ29udHJvbGxlcltwcm9wZXJ0eV0gPSBhZGRPbltwcm9wZXJ0eV0gYXMgYW55O1xyXG4gICAgcmV0dXJuIFN4Y0NvbnRyb2xsZXIgYXMgYW55IGFzIFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzO1xyXG59XHJcblxyXG5mdW5jdGlvbiBhdXRvRmluZChkb21FbGVtZW50OiBIVE1MRWxlbWVudCk6IFtudW1iZXIsIG51bWJlcl0ge1xyXG4gICAgY29uc3QgY29udGFpbmVyVGFnID0gJChkb21FbGVtZW50KS5jbG9zZXN0KCcuc2MtY29udGVudC1ibG9jaycpWzBdO1xyXG4gICAgaWYgKCFjb250YWluZXJUYWcpIHJldHVybiBudWxsO1xyXG4gICAgY29uc3QgaWlkID0gY29udGFpbmVyVGFnLmdldEF0dHJpYnV0ZSgnZGF0YS1jYi1pbnN0YW5jZScpO1xyXG4gICAgY29uc3QgY2JpZCA9IGNvbnRhaW5lclRhZy5nZXRBdHRyaWJ1dGUoJ2RhdGEtY2ItaWQnKTtcclxuICAgIGlmICghaWlkIHx8ICFjYmlkKSByZXR1cm4gbnVsbDtcclxuICAgIHJldHVybiBbaWlkLCBjYmlkXTtcclxufVxyXG5cclxuZXhwb3J0IGludGVyZmFjZSBTeGNDb250cm9sbGVyV2l0aEludGVybmFscyBleHRlbmRzIFN4Y0NvbnRyb2xsZXIge1xyXG4gICAgKGlkOiBudW1iZXIgfCBIVE1MRWxlbWVudCwgY2JpZD86IG51bWJlcik6IFN4Y0luc3RhbmNlIHwgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzO1xyXG4gICAgdG90YWxQb3B1cDogVG90YWxQb3B1cDtcclxuICAgIHVybFBhcmFtczogVXJsUGFyYW1NYW5hZ2VyO1xyXG4gICAgYmV0YTogYW55O1xyXG4gICAgX2NvbnRyb2xsZXJzOiBhbnk7XHJcbiAgICBfZGF0YTogYW55O1xyXG4gICAgX21hbmFnZTogYW55O1xyXG4gICAgX3RyYW5zbGF0ZUluaXQ6IGFueTtcclxuICAgIGRlYnVnOiBhbnk7XHJcbiAgICBwYXJ0czogYW55O1xyXG5cclxufVxyXG5cclxuLy8gUmVTaGFycGVyIHJlc3RvcmUgSW5jb25zaXN0ZW50TmFtaW5nXHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5Db250cm9sbGVyLnRzIiwiXHJcbmltcG9ydCB7IFN4Y0NvbnRyb2xsZXIsIFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzIH0gZnJvbSAnLi9Ub1NpYy5TeGMuQ29udHJvbGxlcic7XHJcbmltcG9ydCB7IFN4Y0RhdGFXaXRoSW50ZXJuYWxzIH0gZnJvbSAnLi9Ub1NpYy5TeGMuRGF0YSc7XHJcbmltcG9ydCB7IFN4Y1dlYkFwaVdpdGhJbnRlcm5hbHMgfSBmcm9tICcuL1RvU2ljLlN4Yy5XZWJBcGknO1xyXG4vKipcclxuICogVGhlIHR5cGljYWwgc3hjLWluc3RhbmNlIG9iamVjdCBmb3IgYSBzcGVjaWZpYyBETk4gbW9kdWxlIG9yIGNvbnRlbnQtYmxvY2tcclxuICovXHJcbmV4cG9ydCBjbGFzcyBTeGNJbnN0YW5jZSB7XHJcbiAgICAvKipcclxuICAgICAqIGhlbHBlcnMgZm9yIGFqYXggY2FsbHNcclxuICAgICAqL1xyXG4gICAgd2ViQXBpOiBTeGNXZWJBcGlXaXRoSW50ZXJuYWxzO1xyXG4gICAgcHJvdGVjdGVkIHNlcnZpY2VSb290OiBzdHJpbmc7XHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IHNlcnZpY2VTY29wZXMgPSBbJ2FwcCcsICdhcHAtc3lzJywgJ2FwcC1hcGknLCAnYXBwLXF1ZXJ5JywgJ2FwcC1jb250ZW50JywgJ2VhdicsICd2aWV3JywgJ2RubiddO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICAqIHRoZSBzeGMtaW5zdGFuY2UgSUQsIHdoaWNoIGlzIHVzdWFsbHkgdGhlIEROTiBNb2R1bGUgSWRcclxuICAgICAgICAgKi9cclxuICAgICAgICBwdWJsaWMgaWQ6IG51bWJlcixcclxuXHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgICogY29udGVudC1ibG9jayBJRCwgd2hpY2ggaXMgZWl0aGVyIHRoZSBtb2R1bGUgSUQsIG9yIHRoZSBjb250ZW50LWJsb2NrIGRlZmluaXRpaW9uIGVudGl0eSBJRFxyXG4gICAgICAgICAqIHRoaXMgaXMgYW4gYWR2YW5jZWQgY29uY2VwdCB5b3UgdXN1YWxseSBkb24ndCBjYXJlIGFib3V0LCBvdGhlcndpc2UgeW91IHNob3VsZCByZXNlYXJjaCBpdFxyXG4gICAgICAgICAqL1xyXG4gICAgICAgIHB1YmxpYyBjYmlkOiBudW1iZXIsXHJcbiAgICAgICAgcHJvdGVjdGVkIHJlYWRvbmx5IGRublNmOiBhbnksXHJcbiAgICApIHtcclxuICAgICAgICB0aGlzLnNlcnZpY2VSb290ID0gZG5uU2YoaWQpLmdldFNlcnZpY2VSb290KCcyc3hjJyk7XHJcbiAgICAgICAgdGhpcy53ZWJBcGkgPSBuZXcgU3hjV2ViQXBpV2l0aEludGVybmFscyh0aGlzLCBpZCwgY2JpZCk7XHJcbiAgICB9XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBjb252ZXJ0cyBhIHNob3J0IGFwaS1jYWxsIHBhdGggbGlrZSBcIi9hcHAvQmxvZy9xdWVyeS94eXpcIiB0byB0aGUgRE5OIGZ1bGwgcGF0aFxyXG4gICAgICogd2hpY2ggdmFyaWVzIGZyb20gaW5zdGFsbGF0aW9uIHRvIGluc3RhbGxhdGlvbiBsaWtlIFwiL2Rlc2t0b3Btb2R1bGVzL2FwaS8yc3hjL2FwcC8uLi5cIlxyXG4gICAgICogQHBhcmFtIHZpcnR1YWxQYXRoXHJcbiAgICAgKiBAcmV0dXJucyBtYXBwZWQgcGF0aFxyXG4gICAgICovXHJcbiAgICByZXNvbHZlU2VydmljZVVybCh2aXJ0dWFsUGF0aDogc3RyaW5nKSB7XHJcbiAgICAgICAgY29uc3Qgc2NvcGUgPSB2aXJ0dWFsUGF0aC5zcGxpdCgnLycpWzBdLnRvTG93ZXJDYXNlKCk7XHJcblxyXG4gICAgICAgIC8vIHN0b3AgaWYgaXQncyBub3Qgb25lIG9mIG91ciBzcGVjaWFsIHBhdGhzXHJcbiAgICAgICAgaWYgKHRoaXMuc2VydmljZVNjb3Blcy5pbmRleE9mKHNjb3BlKSA9PT0gLTEpXHJcbiAgICAgICAgICAgIHJldHVybiB2aXJ0dWFsUGF0aDtcclxuXHJcbiAgICAgICAgcmV0dXJuIHRoaXMuc2VydmljZVJvb3QgKyBzY29wZSArICcvJyArIHZpcnR1YWxQYXRoLnN1YnN0cmluZyh2aXJ0dWFsUGF0aC5pbmRleE9mKCcvJykgKyAxKTtcclxuICAgIH1cclxuXHJcblxyXG4gICAgLy8gU2hvdyBhIG5pY2UgZXJyb3Igd2l0aCBtb3JlIGluZm9zIGFyb3VuZCAyc3hjXHJcbiAgICBzaG93RGV0YWlsZWRIdHRwRXJyb3IocmVzdWx0OiBhbnkpOiBhbnkge1xyXG4gICAgICAgIGlmICh3aW5kb3cuY29uc29sZSlcclxuICAgICAgICAgICAgY29uc29sZS5sb2cocmVzdWx0KTtcclxuXHJcbiAgICAgICAgaWYgKHJlc3VsdC5zdGF0dXMgPT09IDQwNCAmJlxyXG4gICAgICAgICAgICByZXN1bHQuY29uZmlnICYmXHJcbiAgICAgICAgICAgIHJlc3VsdC5jb25maWcudXJsICYmXHJcbiAgICAgICAgICAgIHJlc3VsdC5jb25maWcudXJsLmluZGV4T2YoJy9kaXN0L2kxOG4vJykgPiAtMSkge1xyXG4gICAgICAgICAgICBpZiAod2luZG93LmNvbnNvbGUpXHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZygnanVzdCBmeWk6IGZhaWxlZCB0byBsb2FkIGxhbmd1YWdlIHJlc291cmNlOyB3aWxsIGhhdmUgdG8gdXNlIGRlZmF1bHQnKTtcclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcclxuICAgICAgICB9XHJcblxyXG5cclxuICAgICAgICAvLyBpZiBpdCdzIGFuIHVuc3BlY2lmaWVkIDAtZXJyb3IsIGl0J3MgcHJvYmFibHkgbm90IGFuIGVycm9yIGJ1dCBhIGNhbmNlbGxlZCByZXF1ZXN0LFxyXG4gICAgICAgIC8vIChoYXBwZW5zIHdoZW4gY2xvc2luZyBwb3B1cHMgY29udGFpbmluZyBhbmd1bGFySlMpXHJcbiAgICAgICAgaWYgKHJlc3VsdC5zdGF0dXMgPT09IDAgfHwgcmVzdWx0LnN0YXR1cyA9PT0gLTEpXHJcbiAgICAgICAgICAgIHJldHVybiByZXN1bHQ7XHJcblxyXG4gICAgICAgIC8vIGxldCdzIHRyeSB0byBzaG93IGdvb2QgbWVzc2FnZXMgaW4gbW9zdCBjYXNlc1xyXG4gICAgICAgIGxldCBpbmZvVGV4dCA9ICdIYWQgYW4gZXJyb3IgdGFsa2luZyB0byB0aGUgc2VydmVyIChzdGF0dXMgJyArIHJlc3VsdC5zdGF0dXMgKyAnKS4nO1xyXG4gICAgICAgIGNvbnN0IHNydlJlc3AgPSByZXN1bHQucmVzcG9uc2VUZXh0XHJcbiAgICAgICAgICAgID8gSlNPTi5wYXJzZShyZXN1bHQucmVzcG9uc2VUZXh0KSAvLyBmb3IganF1ZXJ5IGFqYXggZXJyb3JzXHJcbiAgICAgICAgICAgIDogcmVzdWx0LmRhdGE7IC8vIGZvciBhbmd1bGFyICRodHRwXHJcbiAgICAgICAgaWYgKHNydlJlc3ApIHtcclxuICAgICAgICAgICAgY29uc3QgbXNnID0gc3J2UmVzcC5NZXNzYWdlO1xyXG4gICAgICAgICAgICBpZiAobXNnKSBpbmZvVGV4dCArPSAnXFxuTWVzc2FnZTogJyArIG1zZztcclxuICAgICAgICAgICAgY29uc3QgbXNnRGV0ID0gc3J2UmVzcC5NZXNzYWdlRGV0YWlsIHx8IHNydlJlc3AuRXhjZXB0aW9uTWVzc2FnZTtcclxuICAgICAgICAgICAgaWYgKG1zZ0RldCkgaW5mb1RleHQgKz0gJ1xcbkRldGFpbDogJyArIG1zZ0RldDtcclxuXHJcblxyXG4gICAgICAgICAgICBpZiAobXNnRGV0ICYmIG1zZ0RldC5pbmRleE9mKCdObyBhY3Rpb24gd2FzIGZvdW5kJykgPT09IDApXHJcbiAgICAgICAgICAgICAgICBpZiAobXNnRGV0LmluZGV4T2YoJ3RoYXQgbWF0Y2hlcyB0aGUgbmFtZScpID4gMClcclxuICAgICAgICAgICAgICAgICAgICBpbmZvVGV4dCArPSAnXFxuXFxuVGlwIGZyb20gMnN4YzogeW91IHByb2JhYmx5IGdvdCB0aGUgYWN0aW9uLW5hbWUgd3JvbmcgaW4geW91ciBKUy4nO1xyXG4gICAgICAgICAgICAgICAgZWxzZSBpZiAobXNnRGV0LmluZGV4T2YoJ3RoYXQgbWF0Y2hlcyB0aGUgcmVxdWVzdC4nKSA+IDApXHJcbiAgICAgICAgICAgICAgICAgICAgaW5mb1RleHQgKz0gJ1xcblxcblRpcCBmcm9tIDJzeGM6IFNlZW1zIGxpa2UgdGhlIHBhcmFtZXRlcnMgYXJlIHRoZSB3cm9uZyBhbW91bnQgb3IgdHlwZS4nO1xyXG5cclxuICAgICAgICAgICAgaWYgKG1zZyAmJiBtc2cuaW5kZXhPZignQ29udHJvbGxlcicpID09PSAwICYmIG1zZy5pbmRleE9mKCdub3QgZm91bmQnKSA+IDApXHJcbiAgICAgICAgICAgICAgICBpbmZvVGV4dCArPVxyXG4gICAgICAgICAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTptYXgtbGluZS1sZW5ndGhcclxuICAgICAgICAgICAgICAgICAgICBcIlxcblxcblRpcCBmcm9tIDJzeGM6IHlvdSBwcm9iYWJseSBzcGVsbGVkIHRoZSBjb250cm9sbGVyIG5hbWUgd3Jvbmcgb3IgZm9yZ290IHRvIHJlbW92ZSB0aGUgd29yZCAnY29udHJvbGxlcicgZnJvbSB0aGUgY2FsbCBpbiBKUy4gVG8gY2FsbCBhIGNvbnRyb2xsZXIgY2FsbGVkICdEZW1vQ29udHJvbGxlcicgb25seSB1c2UgJ0RlbW8nLlwiO1xyXG5cclxuICAgICAgICB9XHJcbiAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm1heC1saW5lLWxlbmd0aFxyXG4gICAgICAgIGluZm9UZXh0ICs9ICdcXG5cXG5pZiB5b3UgYXJlIGFuIGFkdmFuY2VkIHVzZXIgeW91IGNhbiBsZWFybiBtb3JlIGFib3V0IHdoYXQgd2VudCB3cm9uZyAtIGRpc2NvdmVyIGhvdyBvbiAyc3hjLm9yZy9oZWxwP3RhZz1kZWJ1Zyc7XHJcbiAgICAgICAgYWxlcnQoaW5mb1RleHQpO1xyXG5cclxuICAgICAgICByZXR1cm4gcmVzdWx0O1xyXG4gICAgfVxyXG59XHJcblxyXG4vKipcclxuICogRW5oYW5jZWQgc3hjIGluc3RhbmNlIHdpdGggYWRkaXRpb25hbCBlZGl0aW5nIGZ1bmN0aW9uYWxpdHlcclxuICogVXNlIHRoaXMsIGlmIHlvdSBpbnRlbmQgdG8gcnVuIGNvbnRlbnQtbWFuYWdlbWVudCBjb21tYW5kcyBsaWtlIFwiZWRpdFwiIGZyb20geW91ciBKUyBkaXJlY3RseVxyXG4gKi9cclxuZXhwb3J0IGNsYXNzIFN4Y0luc3RhbmNlV2l0aEVkaXRpbmcgZXh0ZW5kcyBTeGNJbnN0YW5jZSB7XHJcbiAgICAvKipcclxuICAgICAqIG1hbmFnZSBvYmplY3Qgd2hpY2ggcHJvdmlkZXMgYWNjZXNzIHRvIGFkZGl0aW9uYWwgY29udGVudC1tYW5hZ2VtZW50IGZlYXR1cmVzXHJcbiAgICAgKiBpdCBvbmx5IGV4aXN0cyBpZiAyc3hjIGlzIGluIGVkaXQgbW9kZSAob3RoZXJ3aXNlIHRoZSBKUyBhcmUgbm90IGluY2x1ZGVkIGZvciB0aGVzZSBmZWF0dXJlcylcclxuICAgICAqL1xyXG4gICAgbWFuYWdlOiBhbnkgPSBudWxsOyAvLyBpbml0aWFsaXplIGNvcnJlY3RseSBsYXRlciBvblxyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIHB1YmxpYyBpZDogbnVtYmVyLFxyXG4gICAgICAgIHB1YmxpYyBjYmlkOiBudW1iZXIsXHJcbi8vIFJlU2hhcnBlciBkaXNhYmxlIG9uY2UgSW5jb25zaXN0ZW50TmFtaW5nXHJcbiAgICAgICAgcHJvdGVjdGVkICQyc3hjOiBTeGNDb250cm9sbGVyV2l0aEludGVybmFscyxcclxuICAgICAgICBwcm90ZWN0ZWQgcmVhZG9ubHkgZG5uU2Y6IGFueSxcclxuICAgICkge1xyXG4gICAgICAgIHN1cGVyKGlkLCBjYmlkLCBkbm5TZik7XHJcblxyXG4gICAgICAgIC8vIGFkZCBtYW5hZ2UgcHJvcGVydHksIGJ1dCBub3Qgd2l0aGluIGluaXRpYWxpemVyLCBiZWNhdXNlIGluc2lkZSB0aGUgbWFuYWdlLWluaXRpYWxpemVyIGl0IG1heSByZWZlcmVuY2UgMnN4YyBhZ2FpblxyXG4gICAgICAgIHRyeSB7IC8vIHNvbWV0aW1lcyB0aGUgbWFuYWdlIGNhbid0IGJlIGJ1aWx0LCBsaWtlIGJlZm9yZSBpbnN0YWxsaW5nXHJcbiAgICAgICAgICAgIGlmICgkMnN4Yy5fbWFuYWdlKSAkMnN4Yy5fbWFuYWdlLmluaXRJbnN0YW5jZSh0aGlzKTtcclxuICAgICAgICB9IGNhdGNoIChlKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUuZXJyb3IoJ2Vycm9yIGluIDJzeGMgLSB3aWxsIG9ubHkgbG9nIGJ1dCBub3QgdGhyb3cnLCBlKTtcclxuICAgICAgICAgICAgLy8gdGhyb3cgZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIHRoaXMgb25seSB3b3JrcyB3aGVuIG1hbmFnZSBleGlzdHMgKG5vdCBpbnN0YWxsaW5nKSBhbmQgdHJhbnNsYXRvciBleGlzdHMgdG9vXHJcbiAgICAgICAgaWYgKCQyc3hjLl90cmFuc2xhdGVJbml0ICYmIHRoaXMubWFuYWdlKSAkMnN4Yy5fdHJhbnNsYXRlSW5pdCh0aGlzLm1hbmFnZSk7ICAgIC8vIGluaXQgdHJhbnNsYXRlLCBub3QgcmVhbGx5IG5pY2UsIGJ1dCBvayBmb3Igbm93XHJcblxyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogY2hlY2tzIGlmIHdlJ3JlIGN1cnJlbnRseSBpbiBlZGl0IG1vZGVcclxuICAgICAqIEByZXR1cm5zIHtib29sZWFufVxyXG4gICAgICovXHJcbiAgICBpc0VkaXRNb2RlKCkge1xyXG4gICAgICAgIHJldHVybiB0aGlzLm1hbmFnZSAmJiB0aGlzLm1hbmFnZS5faXNFZGl0TW9kZSgpO1xyXG4gICAgfVxyXG5cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFN4Y0luc3RhbmNlV2l0aEludGVybmFscyBleHRlbmRzIFN4Y0luc3RhbmNlV2l0aEVkaXRpbmcge1xyXG4gICAgZGF0YTogU3hjRGF0YVdpdGhJbnRlcm5hbHM7XHJcbiAgICBzb3VyY2U6IGFueSA9IG51bGw7XHJcbiAgICBpc0xvYWRlZCA9IGZhbHNlO1xyXG4gICAgbGFzdFJlZnJlc2g6IERhdGUgPSBudWxsO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIHB1YmxpYyBpZDogbnVtYmVyLFxyXG4gICAgICAgIHB1YmxpYyBjYmlkOiBudW1iZXIsXHJcbiAgICAgICAgcHJpdmF0ZSBjYWNoZUtleTogc3RyaW5nLFxyXG4vLyBSZVNoYXJwZXIgZGlzYWJsZSBvbmNlIEluY29uc2lzdGVudE5hbWluZ1xyXG4gICAgICAgIHByb3RlY3RlZCAkMnN4YzogU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHMsXHJcbiAgICAgICAgcHJvdGVjdGVkIHJlYWRvbmx5IGRublNmOiBhbnksXHJcbiAgICApIHtcclxuICAgICAgICBzdXBlcihpZCwgY2JpZCwgJDJzeGMsIGRublNmKTtcclxuICAgICAgICB0aGlzLmRhdGEgPSBuZXcgU3hjRGF0YVdpdGhJbnRlcm5hbHModGhpcyk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVjcmVhdGUocmVzZXRDYWNoZTogYm9vbGVhbik6IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB7XHJcbiAgICAgICAgaWYgKHJlc2V0Q2FjaGUpIGRlbGV0ZSB0aGlzLiQyc3hjLl9jb250cm9sbGVyc1t0aGlzLmNhY2hlS2V5XTsgLy8gY2xlYXIgY2FjaGVcclxuICAgICAgICByZXR1cm4gdGhpcy4kMnN4Yyh0aGlzLmlkLCB0aGlzLmNiaWQpIGFzIGFueSBhcyBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHM7IC8vIGdlbmVyYXRlIG5ld1xyXG4gICAgfVxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5JbnN0YW5jZS50cyIsImltcG9ydCB7IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyB9IGZyb20gJy4vVG9TaWMuU3hjLkluc3RhbmNlJztcclxuXHJcbmRlY2xhcmUgY29uc3QgJDogYW55O1xyXG5cclxuXHJcbmV4cG9ydCBjbGFzcyBTeGNEYXRhV2l0aEludGVybmFscyB7XHJcbiAgICBzb3VyY2U6IGFueSA9IHVuZGVmaW5lZDtcclxuXHJcbiAgICAvLyBpbi1zdHJlYW1zXHJcbiAgICBcImluXCI6IGFueSA9IHt9O1xyXG5cclxuICAgIC8vIHdpbGwgaG9sZCB0aGUgZGVmYXVsdCBzdHJlYW0gKFtcImluXCJdW1wiRGVmYXVsdFwiXS5MaXN0XHJcbi8vIFJlU2hhcnBlciBkaXNhYmxlIG9uY2UgSW5jb25zaXN0ZW50TmFtaW5nXHJcbiAgICBMaXN0OiBhbnkgPSBbXTtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihcclxuICAgICAgICBwcml2YXRlIGNvbnRyb2xsZXI6IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyxcclxuICAgICkge1xyXG5cclxuICAgIH1cclxuXHJcbiAgICAvLyBzb3VyY2UgcGF0aCBkZWZhdWx0aW5nIHRvIGN1cnJlbnQgcGFnZSArIG9wdGlvbmFsIHBhcmFtc1xyXG4gICAgc291cmNlVXJsKHBhcmFtcz86IHN0cmluZyk6IHN0cmluZyB7XHJcbiAgICAgICAgbGV0IHVybCA9IHRoaXMuY29udHJvbGxlci5yZXNvbHZlU2VydmljZVVybCgnYXBwLXN5cy9hcHBjb250ZW50L0dldENvbnRlbnRCbG9ja0RhdGEnKTtcclxuICAgICAgICBpZiAodHlwZW9mIHBhcmFtcyA9PT0gJ3N0cmluZycpIC8vIHRleHQgbGlrZSAnaWQ9NydcclxuICAgICAgICAgICAgdXJsICs9ICcmJyArIHBhcmFtcztcclxuICAgICAgICByZXR1cm4gdXJsO1xyXG4gICAgfVxyXG5cclxuXHJcbiAgICAvLyBsb2FkIGRhdGEgdmlhIGFqYXhcclxuICAgIGxvYWQoc291cmNlPzogYW55KSB7XHJcbiAgICAgICAgLy8gaWYgc291cmNlIGlzIGFscmVhZHkgdGhlIGRhdGEsIHNldCBpdFxyXG4gICAgICAgIGlmIChzb3VyY2UgJiYgc291cmNlLkxpc3QpIHtcclxuICAgICAgICAgICAgLy8gMjAxNy0wOS0wNSAyZG06IGRpc2NvdmVyZCBhIGNhbGwgdG8gYW4gaW5leGlzdGluZyBmdW5jdGlvblxyXG4gICAgICAgICAgICAvLyBzaW5jZSB0aGlzIGlzIGFuIG9sZCBBUEkgd2hpY2ggaXMgYmVpbmcgZGVwcmVjYXRlZCwgcGxlYXNlIGRvbid0IGZpeCB1bmxlc3Mgd2UgZ2V0IGFjdGl2ZSBmZWVkYmFja1xyXG4gICAgICAgICAgICAvLyBjb250cm9sbGVyLmRhdGEuc2V0RGF0YShzb3VyY2UpO1xyXG4gICAgICAgICAgICByZXR1cm4gdGhpcy5jb250cm9sbGVyLmRhdGE7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgaWYgKCFzb3VyY2UpXHJcbiAgICAgICAgICAgICAgICBzb3VyY2UgPSB7fTtcclxuICAgICAgICAgICAgaWYgKCFzb3VyY2UudXJsKVxyXG4gICAgICAgICAgICAgICAgc291cmNlLnVybCA9IHRoaXMuY29udHJvbGxlci5kYXRhLnNvdXJjZVVybCgpO1xyXG4gICAgICAgICAgICBzb3VyY2Uub3JpZ1N1Y2Nlc3MgPSBzb3VyY2Uuc3VjY2VzcztcclxuICAgICAgICAgICAgc291cmNlLnN1Y2Nlc3MgPSAoZGF0YTogYW55KSA9PiB7XHJcblxyXG4gICAgICAgICAgICAgICAgZm9yIChjb25zdCBkYXRhU2V0TmFtZSBpbiBkYXRhKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGRhdGEuaGFzT3duUHJvcGVydHkoZGF0YVNldE5hbWUpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoZGF0YVtkYXRhU2V0TmFtZV0uTGlzdCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5jb250cm9sbGVyLmRhdGEuaW5bZGF0YVNldE5hbWVdID0gZGF0YVtkYXRhU2V0TmFtZV07XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmNvbnRyb2xsZXIuZGF0YS5pbltkYXRhU2V0TmFtZV0ubmFtZSA9IGRhdGFTZXROYW1lO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuY29udHJvbGxlci5kYXRhLmluLkRlZmF1bHQpXHJcbiAgICAgICAgICAgICAgICAgICAgLy8gMjAxNy0wOS0wNSAyZG06IHByZXZpb3VzbHkgd3JvdGUgaXQgdG8gY29udHJvbGxlci5MaXN0LCBidXQgdGhpcyBpcyBhbG1vc3QgY2VydGFpbmx5IGEgbWlzdGFrZVxyXG4gICAgICAgICAgICAgICAgICAgIC8vIHNpbmNlIGl0J3MgYW4gb2xkIEFQSSB3aGljaCBpcyBiZWluZyBkZXByZWNhdGVkLCB3ZSB3b24ndCBmaXggaXRcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkxpc3QgPSB0aGlzLmluLkRlZmF1bHQuTGlzdDtcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAoc291cmNlLm9yaWdTdWNjZXNzKVxyXG4gICAgICAgICAgICAgICAgICAgIHNvdXJjZS5vcmlnU3VjY2Vzcyh0aGlzKTtcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLmNvbnRyb2xsZXIuaXNMb2FkZWQgPSB0cnVlO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5jb250cm9sbGVyLmxhc3RSZWZyZXNoID0gbmV3IERhdGUoKTtcclxuICAgICAgICAgICAgICAgICh0aGlzIGFzIGFueSkuX3RyaWdnZXJMb2FkZWQoKTtcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICAgICAgc291cmNlLmVycm9yID0gKHJlcXVlc3Q6IGFueSkgPT4geyBhbGVydChyZXF1ZXN0LnN0YXR1c1RleHQpOyB9O1xyXG4gICAgICAgICAgICBzb3VyY2UucHJldmVudEF1dG9GYWlsID0gdHJ1ZTsgLy8gdXNlIG91ciBmYWlsIG1lc3NhZ2VcclxuICAgICAgICAgICAgdGhpcy5zb3VyY2UgPSBzb3VyY2U7XHJcbiAgICAgICAgICAgIHJldHVybiB0aGlzLnJlbG9hZCgpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICByZWxvYWQoKTogU3hjRGF0YVdpdGhJbnRlcm5hbHMge1xyXG4gICAgICAgIHRoaXMuY29udHJvbGxlci53ZWJBcGkuZ2V0KHRoaXMuc291cmNlKVxyXG4gICAgICAgICAgICAudGhlbih0aGlzLnNvdXJjZS5zdWNjZXNzLCB0aGlzLnNvdXJjZS5lcnJvcik7XHJcbiAgICAgICAgcmV0dXJuIHRoaXM7XHJcbiAgICB9XHJcblxyXG4gICAgb24oZXZlbnRzOiBFdmVudCwgY2FsbGJhY2s6ICgpID0+IHZvaWQpOiBQcm9taXNlPGFueT4ge1xyXG4gICAgICAgIHJldHVybiAkKHRoaXMpLmJpbmQoJzJzY0xvYWQnLCBjYWxsYmFjaylbMF0uX3RyaWdnZXJMb2FkZWQoKTtcclxuICAgIH1cclxuXHJcbi8vIFJlU2hhcnBlciBkaXNhYmxlIG9uY2UgSW5jb25zaXN0ZW50TmFtaW5nXHJcbiAgICBfdHJpZ2dlckxvYWRlZCgpOiBQcm9taXNlPGFueT4ge1xyXG4gICAgICAgIHJldHVybiB0aGlzLmNvbnRyb2xsZXIuaXNMb2FkZWRcclxuICAgICAgICAgICAgPyAkKHRoaXMpLnRyaWdnZXIoJzJzY0xvYWQnLCBbdGhpc10pWzBdXHJcbiAgICAgICAgICAgIDogdGhpcztcclxuICAgIH1cclxuXHJcbiAgICBvbmUoZXZlbnRzOiBFdmVudCwgY2FsbGJhY2s6ICh4OiBhbnksIHk6IGFueSkgPT4gdm9pZCk6IFN4Y0RhdGFXaXRoSW50ZXJuYWxzIHtcclxuICAgICAgICBpZiAoIXRoaXMuY29udHJvbGxlci5pc0xvYWRlZClcclxuICAgICAgICAgICAgcmV0dXJuICQodGhpcykub25lKCcyc2NMb2FkJywgY2FsbGJhY2spWzBdO1xyXG4gICAgICAgIGNhbGxiYWNrKHt9LCB0aGlzKTtcclxuICAgICAgICByZXR1cm4gdGhpcztcclxuICAgIH1cclxufVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi8yc3hjLWFwaS9qcy9Ub1NpYy5TeGMuRGF0YS50cyIsIlxyXG5kZWNsYXJlIGNvbnN0ICQ6IGFueTtcclxuaW1wb3J0IHsgU3hjSW5zdGFuY2UgfSBmcm9tICcuL1RvU2ljLlN4Yy5JbnN0YW5jZSc7XHJcblxyXG4vKipcclxuICogaGVscGVyIEFQSSB0byBydW4gYWpheCAvIFJFU1QgY2FsbHMgdG8gdGhlIHNlcnZlclxyXG4gKiBpdCB3aWxsIGVuc3VyZSB0aGF0IHRoZSBoZWFkZXJzIGV0Yy4gYXJlIHNldCBjb3JyZWN0bHlcclxuICogYW5kIHRoYXQgdXJscyBhcmUgcmV3cml0dGVuXHJcbiAqL1xyXG5leHBvcnQgY2xhc3MgU3hjV2ViQXBpV2l0aEludGVybmFscyB7XHJcbiAgICBjb25zdHJ1Y3RvcihcclxuICAgICAgICBwcml2YXRlIHJlYWRvbmx5IGNvbnRyb2xsZXI6IFN4Y0luc3RhbmNlLFxyXG4gICAgICAgIHByaXZhdGUgcmVhZG9ubHkgaWQ6IG51bWJlcixcclxuICAgICAgICBwcml2YXRlIHJlYWRvbmx5IGNiaWQ6IG51bWJlcixcclxuICAgICkge1xyXG5cclxuICAgIH1cclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhbiBodHRwLWdldCBwcm9taXNlXHJcbiAgICAgKiBAcGFyYW0gc2V0dGluZ3NPclVybCB0aGUgdXJsIHRvIGdldFxyXG4gICAgICogQHBhcmFtIHBhcmFtcyBqUXVlcnkgc3R5bGUgYWpheCBwYXJhbWV0ZXJzXHJcbiAgICAgKiBAcGFyYW0gZGF0YSBqUXVlcnkgc3R5bGUgZGF0YSBmb3IgcG9zdC9wdXQgcmVxdWVzdHNcclxuICAgICAqIEBwYXJhbSBwcmV2ZW50QXV0b0ZhaWxcclxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBqUXVlcnkgYWpheCBwcm9taXNlIG9iamVjdFxyXG4gICAgICovXHJcbiAgICBnZXQoc2V0dGluZ3NPclVybDogc3RyaW5nIHwgYW55LCBwYXJhbXM/OiBhbnksIGRhdGE/OiBhbnksIHByZXZlbnRBdXRvRmFpbD86IGJvb2xlYW4pOiBhbnkge1xyXG4gICAgICAgIHJldHVybiB0aGlzLnJlcXVlc3Qoc2V0dGluZ3NPclVybCwgcGFyYW1zLCBkYXRhLCBwcmV2ZW50QXV0b0ZhaWwsICdHRVQnKTtcclxuICAgIH1cclxuXHJcbiAgICAvKipcclxuICAgICAqIHJldHVybnMgYW4gaHR0cC1nZXQgcHJvbWlzZVxyXG4gICAgICogQHBhcmFtIHNldHRpbmdzT3JVcmwgdGhlIHVybCB0byBnZXRcclxuICAgICAqIEBwYXJhbSBwYXJhbXMgalF1ZXJ5IHN0eWxlIGFqYXggcGFyYW1ldGVyc1xyXG4gICAgICogQHBhcmFtIGRhdGEgalF1ZXJ5IHN0eWxlIGRhdGEgZm9yIHBvc3QvcHV0IHJlcXVlc3RzXHJcbiAgICAgKiBAcGFyYW0gcHJldmVudEF1dG9GYWlsXHJcbiAgICAgKiBAcmV0dXJucyB7UHJvbWlzZX0galF1ZXJ5IGFqYXggcHJvbWlzZSBvYmplY3RcclxuICAgICAqL1xyXG4gICAgcG9zdChzZXR0aW5nc09yVXJsOiBzdHJpbmcgfCBhbnksIHBhcmFtcz86IGFueSwgZGF0YT86IGFueSwgcHJldmVudEF1dG9GYWlsPzogYm9vbGVhbik6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMucmVxdWVzdChzZXR0aW5nc09yVXJsLCBwYXJhbXMsIGRhdGEsIHByZXZlbnRBdXRvRmFpbCwgJ1BPU1QnKTtcclxuICAgIH1cclxuXHJcbiAgICAvKipcclxuICAgICAqIHJldHVybnMgYW4gaHR0cC1nZXQgcHJvbWlzZVxyXG4gICAgICogQHBhcmFtIHNldHRpbmdzT3JVcmwgdGhlIHVybCB0byBnZXRcclxuICAgICAqIEBwYXJhbSBwYXJhbXMgalF1ZXJ5IHN0eWxlIGFqYXggcGFyYW1ldGVyc1xyXG4gICAgICogQHBhcmFtIGRhdGEgalF1ZXJ5IHN0eWxlIGRhdGEgZm9yIHBvc3QvcHV0IHJlcXVlc3RzXHJcbiAgICAgKiBAcGFyYW0gcHJldmVudEF1dG9GYWlsXHJcbiAgICAgKiBAcmV0dXJucyB7UHJvbWlzZX0galF1ZXJ5IGFqYXggcHJvbWlzZSBvYmplY3RcclxuICAgICAqL1xyXG4gICAgZGVsZXRlKHNldHRpbmdzT3JVcmw6IHN0cmluZyB8IGFueSwgcGFyYW1zPzogYW55LCBkYXRhPzogYW55LCBwcmV2ZW50QXV0b0ZhaWw/OiBib29sZWFuKTogYW55IHtcclxuICAgICAgICByZXR1cm4gdGhpcy5yZXF1ZXN0KHNldHRpbmdzT3JVcmwsIHBhcmFtcywgZGF0YSwgcHJldmVudEF1dG9GYWlsLCAnREVMRVRFJyk7XHJcbiAgICB9XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiByZXR1cm5zIGFuIGh0dHAtZ2V0IHByb21pc2VcclxuICAgICAqIEBwYXJhbSBzZXR0aW5nc09yVXJsIHRoZSB1cmwgdG8gZ2V0XHJcbiAgICAgKiBAcGFyYW0gcGFyYW1zIGpRdWVyeSBzdHlsZSBhamF4IHBhcmFtZXRlcnNcclxuICAgICAqIEBwYXJhbSBkYXRhIGpRdWVyeSBzdHlsZSBkYXRhIGZvciBwb3N0L3B1dCByZXF1ZXN0c1xyXG4gICAgICogQHBhcmFtIHByZXZlbnRBdXRvRmFpbFxyXG4gICAgICogQHJldHVybnMge1Byb21pc2V9IGpRdWVyeSBhamF4IHByb21pc2Ugb2JqZWN0XHJcbiAgICAgKi9cclxuICAgIHB1dChzZXR0aW5nc09yVXJsOiBzdHJpbmcgfCBhbnksIHBhcmFtcz86IGFueSwgZGF0YT86IGFueSwgcHJldmVudEF1dG9GYWlsPzogYm9vbGVhbik6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMucmVxdWVzdChzZXR0aW5nc09yVXJsLCBwYXJhbXMsIGRhdGEsIHByZXZlbnRBdXRvRmFpbCwgJ1BVVCcpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVxdWVzdChzZXR0aW5nczogc3RyaW5nIHwgYW55LCBwYXJhbXM6IGFueSwgZGF0YTogYW55LCBwcmV2ZW50QXV0b0ZhaWw6IGJvb2xlYW4sIG1ldGhvZDogc3RyaW5nKTogYW55IHtcclxuXHJcbiAgICAgICAgLy8gdXJsIHBhcmFtZXRlcjogYXV0byBjb252ZXJ0IGEgc2luZ2xlIHZhbHVlIChpbnN0ZWFkIG9mIG9iamVjdCBvZiB2YWx1ZXMpIHRvIGFuIGlkPS4uLiBwYXJhbWV0ZXJcclxuICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6Y3VybHlcclxuICAgICAgICBpZiAodHlwZW9mIHBhcmFtcyAhPT0gJ29iamVjdCcgJiYgdHlwZW9mIHBhcmFtcyAhPT0gJ3VuZGVmaW5lZCcpXHJcbiAgICAgICAgICAgIHBhcmFtcyA9IHsgaWQ6IHBhcmFtcyB9O1xyXG5cclxuICAgICAgICAvLyBpZiB0aGUgZmlyc3QgcGFyYW1ldGVyIGlzIGEgc3RyaW5nLCByZXNvbHZlIHNldHRpbmdzXHJcbiAgICAgICAgaWYgKHR5cGVvZiBzZXR0aW5ncyA9PT0gJ3N0cmluZycpIHtcclxuICAgICAgICAgICAgY29uc3QgY29udHJvbGxlckFjdGlvbiA9IHNldHRpbmdzLnNwbGl0KCcvJyk7XHJcbiAgICAgICAgICAgIGNvbnN0IGNvbnRyb2xsZXJOYW1lID0gY29udHJvbGxlckFjdGlvblswXTtcclxuICAgICAgICAgICAgY29uc3QgYWN0aW9uTmFtZSA9IGNvbnRyb2xsZXJBY3Rpb25bMV07XHJcblxyXG4gICAgICAgICAgICBpZiAoY29udHJvbGxlck5hbWUgPT09ICcnIHx8IGFjdGlvbk5hbWUgPT09ICcnKVxyXG4gICAgICAgICAgICAgICAgYWxlcnQoJ0Vycm9yOiBjb250cm9sbGVyIG9yIGFjdGlvbiBub3QgZGVmaW5lZC4gV2lsbCBjb250aW51ZSB3aXRoIGxpa2VseSBlcnJvcnMuJyk7XHJcblxyXG4gICAgICAgICAgICBzZXR0aW5ncyA9IHtcclxuICAgICAgICAgICAgICAgIGNvbnRyb2xsZXI6IGNvbnRyb2xsZXJOYW1lLFxyXG4gICAgICAgICAgICAgICAgYWN0aW9uOiBhY3Rpb25OYW1lLFxyXG4gICAgICAgICAgICAgICAgcGFyYW1zLFxyXG4gICAgICAgICAgICAgICAgZGF0YSxcclxuICAgICAgICAgICAgICAgIHVybDogY29udHJvbGxlckFjdGlvbi5sZW5ndGggPiAyID8gc2V0dGluZ3MgOiBudWxsLFxyXG4gICAgICAgICAgICAgICAgcHJldmVudEF1dG9GYWlsLFxyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgZGVmYXVsdHMgPSB7XHJcbiAgICAgICAgICAgIG1ldGhvZDogbWV0aG9kID09PSBudWxsID8gJ1BPU1QnIDogbWV0aG9kLFxyXG4gICAgICAgICAgICBwYXJhbXM6IG51bGwgYXMgYW55LFxyXG4gICAgICAgICAgICBwcmV2ZW50QXV0b0ZhaWw6IGZhbHNlLFxyXG4gICAgICAgIH07XHJcbiAgICAgICAgc2V0dGluZ3MgPSAkLmV4dGVuZCh7fSwgZGVmYXVsdHMsIHNldHRpbmdzKTtcclxuICAgICAgICBjb25zdCBzZiA9ICQuU2VydmljZXNGcmFtZXdvcmsodGhpcy5pZCk7XHJcbiAgICAgICAgY29uc3QgY2JpZCA9IHRoaXMuY2JpZDsgLy8gbXVzdCByZWFkIGhlcmUsIGFzIHRoZSBcInRoaXNcIiB3aWxsIGNoYW5nZSBpbnNpZGUgdGhlIG1ldGhvZFxyXG5cclxuICAgICAgICBjb25zdCBwcm9taXNlID0gJC5hamF4KHtcclxuICAgICAgICAgICAgYXN5bmM6IHRydWUsXHJcbiAgICAgICAgICAgIGRhdGFUeXBlOiBzZXR0aW5ncy5kYXRhVHlwZSB8fCAnanNvbicsIC8vIGRlZmF1bHQgaXMganNvbiBpZiBub3Qgc3BlY2lmaWVkXHJcbiAgICAgICAgICAgIGRhdGE6IEpTT04uc3RyaW5naWZ5KHNldHRpbmdzLmRhdGEpLFxyXG4gICAgICAgICAgICBjb250ZW50VHlwZTogJ2FwcGxpY2F0aW9uL2pzb24nLFxyXG4gICAgICAgICAgICB0eXBlOiBzZXR0aW5ncy5tZXRob2QsXHJcbiAgICAgICAgICAgIHVybDogdGhpcy5nZXRBY3Rpb25Vcmwoc2V0dGluZ3MpLFxyXG4gICAgICAgICAgICBiZWZvcmVTZW5kKHhocjogYW55KSB7XHJcbiAgICAgICAgICAgICAgICB4aHIuc2V0UmVxdWVzdEhlYWRlcignQ29udGVudEJsb2NrSWQnLCBjYmlkKTtcclxuICAgICAgICAgICAgICAgIHNmLnNldE1vZHVsZUhlYWRlcnMoeGhyKTtcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgaWYgKCFzZXR0aW5ncy5wcmV2ZW50QXV0b0ZhaWwpXHJcbiAgICAgICAgICAgIHByb21pc2UuZmFpbCh0aGlzLmNvbnRyb2xsZXIuc2hvd0RldGFpbGVkSHR0cEVycm9yKTtcclxuXHJcbiAgICAgICAgcmV0dXJuIHByb21pc2U7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBnZXRBY3Rpb25Vcmwoc2V0dGluZ3M6IGFueSk6IHN0cmluZyB7XHJcbiAgICAgICAgY29uc3Qgc2YgPSAkLlNlcnZpY2VzRnJhbWV3b3JrKHRoaXMuaWQpO1xyXG4gICAgICAgIGNvbnN0IGJhc2UgPSAoc2V0dGluZ3MudXJsKVxyXG4gICAgICAgICAgICA/IHRoaXMuY29udHJvbGxlci5yZXNvbHZlU2VydmljZVVybChzZXR0aW5ncy51cmwpXHJcbiAgICAgICAgICAgIDogc2YuZ2V0U2VydmljZVJvb3QoJzJzeGMnKSArICdhcHAvYXV0by9hcGkvJyArIHNldHRpbmdzLmNvbnRyb2xsZXIgKyAnLycgKyBzZXR0aW5ncy5hY3Rpb247XHJcbiAgICAgICAgcmV0dXJuIGJhc2UgKyAoc2V0dGluZ3MucGFyYW1zID09PSBudWxsID8gJycgOiAoJz8nICsgJC5wYXJhbShzZXR0aW5ncy5wYXJhbXMpKSk7XHJcbiAgICB9XHJcblxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5XZWJBcGkudHMiLCJcclxuZXhwb3J0IGNsYXNzIFRvdGFsUG9wdXAge1xyXG4gICAgZnJhbWU6IGFueSA9IHVuZGVmaW5lZDtcclxuICAgIGNhbGxiYWNrOiBhbnkgPSB1bmRlZmluZWQ7XHJcblxyXG4gICAgb3Blbih1cmw6IHN0cmluZywgY2FsbGJhY2s6ICgpID0+IHZvaWQpOiB2b2lkIHtcclxuICAgICAgICAvLyBjb3VudCBwYXJlbnRzIHRvIHNlZSBob3cgaGlnaCB0aGUgei1pbmRleCBuZWVkcyB0byBiZVxyXG4gICAgICAgIGxldCB6ID0gMTAwMDAwMTA7IC8vIE5lZWRzIGF0IGxlYXN0IDEwMDAwMDAwIHRvIGJlIG9uIHRvcCBvZiB0aGUgRE5OOSBiYXJcclxuICAgICAgICBsZXQgcCA9IHdpbmRvdztcclxuICAgICAgICB3aGlsZSAocCAhPT0gd2luZG93LnRvcCAmJiB6IDwgMTAwMDAxMDApIHtcclxuICAgICAgICAgICAgeisrO1xyXG4gICAgICAgICAgICBwID0gcC5wYXJlbnQ7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCB3cmFwcGVyID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnZGl2Jyk7XHJcbiAgICAgICAgd3JhcHBlci5zZXRBdHRyaWJ1dGUoJ3N0eWxlJywgJyB0b3A6IDA7bGVmdDogMDt3aWR0aDogMTAwJTtoZWlnaHQ6IDEwMCU7IHBvc2l0aW9uOmZpeGVkOyB6LWluZGV4OicgKyB6KTtcclxuICAgICAgICBkb2N1bWVudC5ib2R5LmFwcGVuZENoaWxkKHdyYXBwZXIpO1xyXG5cclxuICAgICAgICBjb25zdCBpZnJtID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnaWZyYW1lJyk7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoJ2FsbG93dHJhbnNwYXJlbmN5JywgJ3RydWUnKTtcclxuICAgICAgICBpZnJtLnNldEF0dHJpYnV0ZSgnc3R5bGUnLCAndG9wOiAwO2xlZnQ6IDA7d2lkdGg6IDEwMCU7aGVpZ2h0OiAxMDAlOycpO1xyXG4gICAgICAgIGlmcm0uc2V0QXR0cmlidXRlKCdzcmMnLCB1cmwpO1xyXG4gICAgICAgIHdyYXBwZXIuYXBwZW5kQ2hpbGQoaWZybSk7XHJcbiAgICAgICAgZG9jdW1lbnQuYm9keS5jbGFzc05hbWUgKz0gJyBzeGMtcG9wdXAtb3Blbic7XHJcbiAgICAgICAgdGhpcy5mcmFtZSA9IGlmcm07XHJcbiAgICAgICAgdGhpcy5jYWxsYmFjayA9IGNhbGxiYWNrO1xyXG4gICAgfVxyXG5cclxuICAgIGNsb3NlKCk6IHZvaWQge1xyXG4gICAgICAgIGlmICh0aGlzLmZyYW1lKSB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmJvZHkuY2xhc3NOYW1lID0gZG9jdW1lbnQuYm9keS5jbGFzc05hbWUucmVwbGFjZSgnc3hjLXBvcHVwLW9wZW4nLCAnJyk7XHJcbiAgICAgICAgICAgIGNvbnN0IGZybSA9IHRoaXMuZnJhbWU7XHJcbiAgICAgICAgICAgIGZybS5wYXJlbnROb2RlLnBhcmVudE5vZGUucmVtb3ZlQ2hpbGQoZnJtLnBhcmVudE5vZGUpO1xyXG4gICAgICAgICAgICB0aGlzLmNhbGxiYWNrKCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGNsb3NlVGhpcygpOiB2b2lkIHtcclxuICAgICAgICAod2luZG93LnBhcmVudCBhcyBhbnkpLiQyc3hjLnRvdGFsUG9wdXAuY2xvc2UoKTtcclxuICAgIH1cclxuXHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlRvdGFsUG9wdXAudHMiLCJcclxuICAgIGV4cG9ydCBjbGFzcyBVcmxQYXJhbU1hbmFnZXIge1xyXG4gICAgICAgIGdldChuYW1lOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgLy8gd2FybmluZzogdGhpcyBtZXRob2QgaXMgZHVwbGljYXRlZCBpbiAyIHBsYWNlcyAtIGtlZXAgdGhlbSBpbiBzeW5jLlxyXG4gICAgICAgICAgICAvLyBsb2NhdGlvbnMgYXJlIGVhdiBhbmQgMnN4YzRuZ1xyXG4gICAgICAgICAgICBuYW1lID0gbmFtZS5yZXBsYWNlKC9bXFxbXS8sICdcXFxcWycpLnJlcGxhY2UoL1tcXF1dLywgJ1xcXFxdJyk7XHJcbiAgICAgICAgICAgIGNvbnN0IHNlYXJjaFJ4ID0gbmV3IFJlZ0V4cCgnW1xcXFw/Jl0nICsgbmFtZSArICc9KFteJiNdKiknLCAnaScpO1xyXG4gICAgICAgICAgICBsZXQgcmVzdWx0cyA9IHNlYXJjaFJ4LmV4ZWMobG9jYXRpb24uc2VhcmNoKTtcclxuICAgICAgICAgICAgbGV0IHN0clJlc3VsdDogc3RyaW5nO1xyXG5cclxuICAgICAgICAgICAgaWYgKHJlc3VsdHMgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IGhhc2hSeCA9IG5ldyBSZWdFeHAoJ1sjJl0nICsgbmFtZSArICc9KFteJiNdKiknLCAnaScpO1xyXG4gICAgICAgICAgICAgICAgcmVzdWx0cyA9IGhhc2hSeC5leGVjKGxvY2F0aW9uLmhhc2gpO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAvLyBpZiBub3RoaW5nIGZvdW5kLCB0cnkgbm9ybWFsIFVSTCBiZWNhdXNlIEROTiBwbGFjZXMgcGFyYW1ldGVycyBpbiAva2V5L3ZhbHVlIG5vdGF0aW9uXHJcbiAgICAgICAgICAgIGlmIChyZXN1bHRzID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAvLyBPdGhlcndpc2UgdHJ5IHBhcnRzIG9mIHRoZSBVUkxcclxuICAgICAgICAgICAgICAgIGNvbnN0IG1hdGNoZXMgPSB3aW5kb3cubG9jYXRpb24ucGF0aG5hbWUubWF0Y2gobmV3IFJlZ0V4cCgnLycgKyBuYW1lICsgJy8oW14vXSspJywgJ2knKSk7XHJcblxyXG4gICAgICAgICAgICAgICAgLy8gQ2hlY2sgaWYgd2UgZm91bmQgYW55dGhpbmcsIGlmIHdlIGRvIGZpbmQgaXQsIHdlIG11c3QgcmV2ZXJzZSB0aGVcclxuICAgICAgICAgICAgICAgIC8vIHJlc3VsdHMgc28gd2UgZ2V0IHRoZSBcImxhc3RcIiBvbmUgaW4gY2FzZSB0aGVyZSBhcmUgbXVsdGlwbGUgaGl0c1xyXG4gICAgICAgICAgICAgICAgaWYgKG1hdGNoZXMgJiYgbWF0Y2hlcy5sZW5ndGggPiAxKVxyXG4gICAgICAgICAgICAgICAgICAgIHN0clJlc3VsdCA9IG1hdGNoZXMucmV2ZXJzZSgpWzBdO1xyXG4gICAgICAgICAgICB9IGVsc2VcclxuICAgICAgICAgICAgICAgIHN0clJlc3VsdCA9IHJlc3VsdHNbMV07XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gc3RyUmVzdWx0ID09PSBudWxsIHx8IHN0clJlc3VsdCA9PT0gdW5kZWZpbmVkXHJcbiAgICAgICAgICAgICAgICA/ICcnXHJcbiAgICAgICAgICAgICAgICA6IGRlY29kZVVSSUNvbXBvbmVudChzdHJSZXN1bHQucmVwbGFjZSgvXFwrL2csICcgJykpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVxdWlyZShuYW1lOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgY29uc3QgZm91bmQgPSB0aGlzLmdldChuYW1lKTtcclxuICAgICAgICAgICAgaWYgKGZvdW5kID09PSAnJykge1xyXG4gICAgICAgICAgICAgICAgY29uc3QgbWVzc2FnZSA9IGBSZXF1aXJlZCBwYXJhbWV0ZXIgKCR7bmFtZX0pIG1pc3NpbmcgZnJvbSB1cmwgLSBjYW5ub3QgY29udGludWVgO1xyXG4gICAgICAgICAgICAgICAgYWxlcnQobWVzc2FnZSk7XHJcbiAgICAgICAgICAgICAgICB0aHJvdyBtZXNzYWdlO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIHJldHVybiBmb3VuZDtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5VcmwudHMiLCJleHBvcnQgY2xhc3MgU3RhdHMge1xyXG4gICAgd2F0Y2hEb21DaGFuZ2VzID0gMDtcclxufVxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1N0YXRzLnRzIl0sInNvdXJjZVJvb3QiOiIifQ==