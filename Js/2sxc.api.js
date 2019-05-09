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
            version: '09.43.00',
            description: 'The 2sxc Controller object - read more about it on 2sxc.org',
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAgNjRjMjA2MzZmZDNhZDIyZTM1ZDciLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvMnN4Yy5hcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkNvbnRyb2xsZXIudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkluc3RhbmNlLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5EYXRhLnRzIiwid2VicGFjazovLy8uLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5XZWJBcGkudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlRvdGFsUG9wdXAudHMiLCJ3ZWJwYWNrOi8vLy4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlVybC50cyIsIndlYnBhY2s6Ly8vLi8yc3hjLWFwaS9qcy9TdGF0cy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUE7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7OztBQUdBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLGFBQUs7QUFDTDtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBLG1DQUEyQiwwQkFBMEIsRUFBRTtBQUN2RCx5Q0FBaUMsZUFBZTtBQUNoRDtBQUNBO0FBQ0E7O0FBRUE7QUFDQSw4REFBc0QsK0RBQStEOztBQUVySDtBQUNBOztBQUVBO0FBQ0E7Ozs7Ozs7Ozs7QUN4RG9FO0FBS3BFLElBQUksQ0FBQyxNQUFNLENBQUMsS0FBSztJQUNiLE1BQU0sQ0FBQyxLQUFLLEdBQUcseUZBQWtCLEVBQUUsQ0FBQzs7Ozs7Ozs7Ozs7OztBQ1Q2RDtBQUNqRDtBQUNGO0FBQ2xCO0FB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QUFRLENBQUMsUUFBUSxDQUFDLEtBQUssQ0FBQyxJQUFJLE1BQU0sQ0FBQyxHQUFHLEdBQUcsSUFBSSxHQUFHLFVBQVUsRUFBRSxHQUFHLENBQUMsQ0FBQyxDQUFDO1lBSXpGLElBQUksT0FBTyxJQUFJLE9BQU8sQ0FBQyxNQUFNLEdBQUcsQ0FBQztnQkFDN0IsU0FBUyxHQUFHLE9BQU8sQ0FBQyxPQUFPLEVBQUUsQ0FBQyxDQUFDLENBQUMsQ0FBQztTQUN4Qzs7WUFDRyxTQUFTLEdBQUcsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBRTNCLE9BQU8sU0FBUyxLQUFLLElBQUksSUFBSSxTQUFTLEtBQUssU0FBUztZQUNoRCxDQUFDLENBQUMsRUFBRTtZQUNKLENBQUMsQ0FBQyxrQkFBa0IsQ0FBQyxTQUFTLENBQUMsT0FBTyxDQUFDLEtBQUssRUFBRSxHQUFHLENBQUMsQ0FBQyxDQUFDO0lBQzVELENBQUM7SUFFRCxpQ0FBTyxHQUFQLFVBQVEsSUFBWTtRQUNoQixJQUFNLEtBQUssR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLElBQUksQ0FBQyxDQUFDO1FBQzdCLElBQUksS0FBSyxLQUFLLEVBQUUsRUFBRTtZQUNkLElBQU0sT0FBTyxHQUFHLHlCQUF1QixJQUFJLHlDQUFzQyxDQUFDO1lBQ2xGLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQztZQUNmLE1BQU0sT0FBTyxDQUFDO1NBQ2pCO1FBQ0QsT0FBTyxLQUFLLENBQUM7SUFDakIsQ0FBQztJQUNMLHNCQUFDO0FBQUQsQ0FBQzs7Ozs7Ozs7O0FDekNMO0FBQUE7SUFBQTtRQUNJLG9CQUFlLEdBQUcsQ0FBQyxDQUFDO0lBQ3hCLENBQUM7SUFBRCxZQUFDO0FBQUQsQ0FBQyIsImZpbGUiOiIyc3hjLmFwaS5qcyIsInNvdXJjZXNDb250ZW50IjpbIiBcdC8vIFRoZSBtb2R1bGUgY2FjaGVcbiBcdHZhciBpbnN0YWxsZWRNb2R1bGVzID0ge307XG5cbiBcdC8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG4gXHRmdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cbiBcdFx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG4gXHRcdGlmKGluc3RhbGxlZE1vZHVsZXNbbW9kdWxlSWRdKSB7XG4gXHRcdFx0cmV0dXJuIGluc3RhbGxlZE1vZHVsZXNbbW9kdWxlSWRdLmV4cG9ydHM7XG4gXHRcdH1cbiBcdFx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcbiBcdFx0dmFyIG1vZHVsZSA9IGluc3RhbGxlZE1vZHVsZXNbbW9kdWxlSWRdID0ge1xuIFx0XHRcdGk6IG1vZHVsZUlkLFxuIFx0XHRcdGw6IGZhbHNlLFxuIFx0XHRcdGV4cG9ydHM6IHt9XG4gXHRcdH07XG5cbiBcdFx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG4gXHRcdG1vZHVsZXNbbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG4gXHRcdC8vIEZsYWcgdGhlIG1vZHVsZSBhcyBsb2FkZWRcbiBcdFx0bW9kdWxlLmwgPSB0cnVlO1xuXG4gXHRcdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG4gXHRcdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbiBcdH1cblxuXG4gXHQvLyBleHBvc2UgdGhlIG1vZHVsZXMgb2JqZWN0IChfX3dlYnBhY2tfbW9kdWxlc19fKVxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5tID0gbW9kdWxlcztcblxuIFx0Ly8gZXhwb3NlIHRoZSBtb2R1bGUgY2FjaGVcbiBcdF9fd2VicGFja19yZXF1aXJlX18uYyA9IGluc3RhbGxlZE1vZHVsZXM7XG5cbiBcdC8vIGRlZmluZSBnZXR0ZXIgZnVuY3Rpb24gZm9yIGhhcm1vbnkgZXhwb3J0c1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5kID0gZnVuY3Rpb24oZXhwb3J0cywgbmFtZSwgZ2V0dGVyKSB7XG4gXHRcdGlmKCFfX3dlYnBhY2tfcmVxdWlyZV9fLm8oZXhwb3J0cywgbmFtZSkpIHtcbiBcdFx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkoZXhwb3J0cywgbmFtZSwge1xuIFx0XHRcdFx0Y29uZmlndXJhYmxlOiBmYWxzZSxcbiBcdFx0XHRcdGVudW1lcmFibGU6IHRydWUsXG4gXHRcdFx0XHRnZXQ6IGdldHRlclxuIFx0XHRcdH0pO1xuIFx0XHR9XG4gXHR9O1xuXG4gXHQvLyBnZXREZWZhdWx0RXhwb3J0IGZ1bmN0aW9uIGZvciBjb21wYXRpYmlsaXR5IHdpdGggbm9uLWhhcm1vbnkgbW9kdWxlc1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5uID0gZnVuY3Rpb24obW9kdWxlKSB7XG4gXHRcdHZhciBnZXR0ZXIgPSBtb2R1bGUgJiYgbW9kdWxlLl9fZXNNb2R1bGUgP1xuIFx0XHRcdGZ1bmN0aW9uIGdldERlZmF1bHQoKSB7IHJldHVybiBtb2R1bGVbJ2RlZmF1bHQnXTsgfSA6XG4gXHRcdFx0ZnVuY3Rpb24gZ2V0TW9kdWxlRXhwb3J0cygpIHsgcmV0dXJuIG1vZHVsZTsgfTtcbiBcdFx0X193ZWJwYWNrX3JlcXVpcmVfXy5kKGdldHRlciwgJ2EnLCBnZXR0ZXIpO1xuIFx0XHRyZXR1cm4gZ2V0dGVyO1xuIFx0fTtcblxuIFx0Ly8gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eS5jYWxsXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLm8gPSBmdW5jdGlvbihvYmplY3QsIHByb3BlcnR5KSB7IHJldHVybiBPYmplY3QucHJvdG90eXBlLmhhc093blByb3BlcnR5LmNhbGwob2JqZWN0LCBwcm9wZXJ0eSk7IH07XG5cbiBcdC8vIF9fd2VicGFja19wdWJsaWNfcGF0aF9fXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLnAgPSBcIlwiO1xuXG4gXHQvLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbiBcdHJldHVybiBfX3dlYnBhY2tfcmVxdWlyZV9fKF9fd2VicGFja19yZXF1aXJlX18ucyA9IDApO1xuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIHdlYnBhY2svYm9vdHN0cmFwIDY0YzIwNjM2ZmQzYWQyMmUzNWQ3IiwiLy8gdGhpcyBpcyB0aGUgMnN4Yy1qYXZhc2NyaXB0IEFQSVxyXG4vLyAyc3hjIHdpbGwgaW5jbHVkZSB0aGlzIGF1dG9tYXRpY2FsbHkgd2hlbiBhIHVzZXIgaGFzIGVkaXQtcmlnaHRzXHJcbi8vIGEgdGVtcGxhdGUgZGV2ZWxvcGVyIHdpbGwgdHlwaWNhbGx5IHVzZSB0aGlzIHRvIHVzZSB0aGUgZGF0YS1hcGkgdG8gcmVhZCAyc3hjLWRhdGEgZnJvbSB0aGUgc2VydmVyXHJcbi8vIHJlYWQgbW9yZSBhYm91dCB0aGlzIGluIHRoZSB3aWtpOiBodHRwczovL2dpdGh1Yi5jb20vMnNpYy8yc3hjL3dpa2kvSmF2YVNjcmlwdC0lMjQyc3hjXHJcblxyXG5pbXBvcnQgeyBidWlsZFN4Y0NvbnRyb2xsZXIsIFdpbmRvdyB9IGZyb20gXCIuL1RvU2ljLlN4Yy5Db250cm9sbGVyXCI7XHJcblxyXG4vLyBSZVNoYXJwZXIgZGlzYWJsZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuZGVjbGFyZSBjb25zdCB3aW5kb3c6IFdpbmRvdztcclxuXHJcbmlmICghd2luZG93LiQyc3hjKSAvLyBwcmV2ZW50IGRvdWJsZSBleGVjdXRpb25cclxuICAgIHdpbmRvdy4kMnN4YyA9IGJ1aWxkU3hjQ29udHJvbGxlcigpO1xyXG5cclxuLy8gUmVTaGFycGVyIHJlc3RvcmUgSW5jb25zaXN0ZW50TmFtaW5nXHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzLzJzeGMuYXBpLnRzIiwiLy8gUmVTaGFycGVyIGRpc2FibGUgSW5jb25zaXN0ZW50TmFtaW5nXHJcblxyXG5pbXBvcnQgeyBTeGNJbnN0YW5jZSwgU3hjSW5zdGFuY2VXaXRoRWRpdGluZywgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIH0gZnJvbSAnLi9Ub1NpYy5TeGMuSW5zdGFuY2UnO1xyXG5pbXBvcnQgeyBUb3RhbFBvcHVwIH0gZnJvbSAnLi9Ub1NpYy5TeGMuVG90YWxQb3B1cCc7XHJcbmltcG9ydCB7IFVybFBhcmFtTWFuYWdlciB9IGZyb20gJy4vVG9TaWMuU3hjLlVybCc7XHJcbmltcG9ydCB7IFN0YXRzIH0gZnJvbSAnLi9TdGF0cyc7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFdpbmRvdyB7ICQyc3hjOiBTeGNDb250cm9sbGVyIHwgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHM7IH1cclxuXHJcbmRlY2xhcmUgY29uc3QgJDogYW55O1xyXG5kZWNsYXJlIGNvbnN0IHdpbmRvdzogV2luZG93O1xyXG5cclxuLyoqXHJcbiAqIFRoaXMgaXMgdGhlIGludGVyZmFjZSBmb3IgdGhlIG1haW4gJDJzeGMgb2JqZWN0IG9uIHRoZSB3aW5kb3dcclxuICovXHJcbmV4cG9ydCBpbnRlcmZhY2UgU3hjQ29udHJvbGxlciB7XHJcbiAgICAvKipcclxuICAgICAqIHJldHVybnMgYSAyc3hjLWluc3RhbmNlIG9mIHRoZSBpZCBvciBodG1sLXRhZyBwYXNzZWQgaW5cclxuICAgICAqIEBwYXJhbSBpZFxyXG4gICAgICogQHBhcmFtIGNiaWRcclxuICAgICAqIEByZXR1cm5zIHt9XHJcbiAgICAgKi9cclxuICAgIChpZDogbnVtYmVyIHwgSFRNTEVsZW1lbnQsIGNiaWQ/OiBudW1iZXIpOiBTeGNJbnN0YW5jZSB8IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyxcclxuXHJcbiAgICAvKipcclxuICAgICAqIHN5c3RlbSBpbmZvcm1hdGlvbiwgbWFpbmx5IGZvciBjaGVja2luZyB3aGljaCB2ZXJzaW9uIG9mIDJzeGMgaXMgcnVubmluZ1xyXG4gICAgICogbm90ZTogaXQncyBub3QgYWx3YXlzIHVwZGF0ZWQgcmVsaWFibHksIGJ1dCBpdCBoZWxwcyB3aGVuIGRlYnVnZ2luZ1xyXG4gICAgICovXHJcbiAgICBzeXNpbmZvOiB7XHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgICogdGhlIHZlcnNpb24gdXNpbmcgdGhlICMjLiMjLiMjIHN5bnRheFxyXG4gICAgICAgICAqL1xyXG4gICAgICAgIHZlcnNpb246IHN0cmluZyxcclxuXHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgICogYSBzaG9ydCB0ZXh0IGRlc2NyaXB0aW9uLCBmb3IgcGVvcGxlIHdobyBoYXZlIG5vIGlkZWEgd2hhdCB0aGlzIGlzXHJcbiAgICAgICAgICovXHJcbiAgICAgICAgZGVzY3JpcHRpb246IHN0cmluZyxcclxuICAgIH07XHJcbn1cclxuXHJcbi8qKlxyXG4gKiByZXR1cm5zIGEgMnN4Yy1pbnN0YW5jZSBvZiB0aGUgaWQgb3IgaHRtbC10YWcgcGFzc2VkIGluXHJcbiAqIEBwYXJhbSBpZFxyXG4gKiBAcGFyYW0gY2JpZFxyXG4gKiBAcmV0dXJucyB7fVxyXG4gKi9cclxuZnVuY3Rpb24gU3hjQ29udHJvbGxlcihpZDogbnVtYmVyIHwgSFRNTEVsZW1lbnQsIGNiaWQ/OiBudW1iZXIpOiBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHMge1xyXG4gICAgY29uc3QgJDJzeGMgPSB3aW5kb3cuJDJzeGMgYXMgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHM7XHJcbiAgICBpZiAoISQyc3hjLl9jb250cm9sbGVycylcclxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoJyQyc3hjIG5vdCBpbml0aWFsaXplZCB5ZXQnKTtcclxuXHJcbiAgICAvLyBpZiBpdCdzIGEgZG9tLWVsZW1lbnQsIHVzZSBhdXRvLWZpbmRcclxuICAgIGlmICh0eXBlb2YgaWQgPT09ICdvYmplY3QnKSB7XHJcbiAgICAgICAgY29uc3QgaWRUdXBsZSA9IGF1dG9GaW5kKGlkKTtcclxuICAgICAgICBpZCA9IGlkVHVwbGVbMF07XHJcbiAgICAgICAgY2JpZCA9IGlkVHVwbGVbMV07XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKCFjYmlkKSBjYmlkID0gaWQ7ICAgICAgICAgICAvLyBpZiBjb250ZW50LWJsb2NrIGlzIHVua25vd24sIHVzZSBpZCBvZiBtb2R1bGVcclxuICAgIGNvbnN0IGNhY2hlS2V5ID0gaWQgKyAnOicgKyBjYmlkOyAvLyBuZXV0cmFsaXplIHRoZSBpZCBmcm9tIG9sZCBcIjM0XCIgZm9ybWF0IHRvIHRoZSBuZXcgXCIzNTozNTNcIiBmb3JtYXRcclxuXHJcbiAgICAvLyBlaXRoZXIgZ2V0IHRoZSBjYWNoZWQgY29udHJvbGxlciBmcm9tIHByZXZpb3VzIGNhbGxzLCBvciBjcmVhdGUgYSBuZXcgb25lXHJcbiAgICBpZiAoJDJzeGMuX2NvbnRyb2xsZXJzW2NhY2hlS2V5XSkgcmV0dXJuICQyc3hjLl9jb250cm9sbGVyc1tjYWNoZUtleV07XHJcblxyXG4gICAgLy8gYWxzbyBpbml0IHRoZSBkYXRhLWNhY2hlIGluIGNhc2UgaXQncyBldmVyIG5lZWRlZFxyXG4gICAgaWYgKCEkMnN4Yy5fZGF0YVtjYWNoZUtleV0pICQyc3hjLl9kYXRhW2NhY2hlS2V5XSA9IHt9O1xyXG5cclxuICAgIHJldHVybiAoJDJzeGMuX2NvbnRyb2xsZXJzW2NhY2hlS2V5XVxyXG4gICAgICAgID0gbmV3IFN4Y0luc3RhbmNlV2l0aEludGVybmFscyhpZCwgY2JpZCwgY2FjaGVLZXksICQyc3hjLCAkLlNlcnZpY2VzRnJhbWV3b3JrKSk7XHJcbn1cclxuXHJcbi8qKlxyXG4gKiBCdWlsZCBhIFNYQyBDb250cm9sbGVyIGZvciB0aGUgcGFnZS4gU2hvdWxkIG9ubHkgZXZlciBiZSBleGVjdXRlZCBvbmNlXHJcbiAqL1xyXG5leHBvcnQgZnVuY3Rpb24gYnVpbGRTeGNDb250cm9sbGVyKCk6IFN4Y0NvbnRyb2xsZXIgfCBTeGNDb250cm9sbGVyV2l0aEludGVybmFscyB7XHJcbiAgICBjb25zdCB1cmxNYW5hZ2VyID0gbmV3IFVybFBhcmFtTWFuYWdlcigpO1xyXG4gICAgY29uc3QgZGVidWcgPSB7XHJcbiAgICAgICAgbG9hZDogKHVybE1hbmFnZXIuZ2V0KCdkZWJ1ZycpID09PSAndHJ1ZScpLFxyXG4gICAgICAgIHVuY2FjaGU6IHVybE1hbmFnZXIuZ2V0KCdzeGN2ZXInKSxcclxuICAgIH07XHJcbiAgICBjb25zdCBzdGF0cyA9IG5ldyBTdGF0cygpO1xyXG5cclxuICAgIGNvbnN0IGFkZE9uOiBhbnkgPSB7XHJcbiAgICAgICAgX2NvbnRyb2xsZXJzOiB7fSBhcyBhbnksXHJcbiAgICAgICAgc3lzaW5mbzoge1xyXG4gICAgICAgICAgICB2ZXJzaW9uOiAnMDkuNDMuMDAnLFxyXG4gICAgICAgICAgICBkZXNjcmlwdGlvbjogJ1RoZSAyc3hjIENvbnRyb2xsZXIgb2JqZWN0IC0gcmVhZCBtb3JlIGFib3V0IGl0IG9uIDJzeGMub3JnJyxcclxuICAgICAgICB9LFxyXG4gICAgICAgIGJldGE6IHt9LFxyXG4gICAgICAgIF9kYXRhOiB7fSxcclxuICAgICAgICAvLyB0aGlzIGNyZWF0ZXMgYSBmdWxsLXNjcmVlbiBpZnJhbWUtcG9wdXAgYW5kIHByb3ZpZGVzIGEgY2xvc2UtY29tbWFuZCB0byBmaW5pc2ggdGhlIGRpYWxvZyBhcyBuZWVkZWRcclxuICAgICAgICB0b3RhbFBvcHVwOiBuZXcgVG90YWxQb3B1cCgpLFxyXG4gICAgICAgIHVybFBhcmFtczogdXJsTWFuYWdlcixcclxuICAgICAgICAvLyBub3RlOiBJIHdvdWxkIGxpa2UgdG8gcmVtb3ZlIHRoaXMgZnJvbSAkMnN4YywgYnV0IGl0J3MgY3VycmVudGx5XHJcbiAgICAgICAgLy8gdXNlZCBib3RoIGluIHRoZSBpbnBhZ2UtZWRpdCBhbmQgaW4gdGhlIGRpYWxvZ3NcclxuICAgICAgICAvLyBkZWJ1ZyBzdGF0ZSB3aGljaCBpcyBuZWVkZWQgaW4gdmFyaW91cyBwbGFjZXNcclxuICAgICAgICBkZWJ1ZyxcclxuICAgICAgICBzdGF0czogc3RhdHMsXHJcbiAgICAgICAgLy8gbWluaS1oZWxwZXJzIHRvIG1hbmFnZSAyc3hjIHBhcnRzLCBhIGJpdCBsaWtlIGEgZGVwZW5kZW5jeSBsb2FkZXJcclxuICAgICAgICAvLyB3aGljaCB3aWxsIG9wdGltaXplIHRvIGxvYWQgbWluL21heCBkZXBlbmRpbmcgb24gZGVidWcgc3RhdGVcclxuICAgICAgICBwYXJ0czoge1xyXG4gICAgICAgICAgICBnZXRVcmwodXJsOiBzdHJpbmcsIHByZXZlbnRVbm1pbjogYm9vbGVhbikge1xyXG4gICAgICAgICAgICAgICAgbGV0IHIgPSAocHJldmVudFVubWluIHx8ICFkZWJ1Zy5sb2FkKSA/IHVybCA6IHVybC5yZXBsYWNlKCcubWluJywgJycpOyAvLyB1c2UgbWluIG9yIG5vdFxyXG4gICAgICAgICAgICAgICAgaWYgKGRlYnVnLnVuY2FjaGUgJiYgci5pbmRleE9mKCdzeGN2ZXInKSA9PT0gLTEpXHJcbiAgICAgICAgICAgICAgICAgICAgciA9IHIgKyAoKHIuaW5kZXhPZignPycpID09PSAtMSkgPyAnPycgOiAnJicpICsgJ3N4Y3Zlcj0nICsgZGVidWcudW5jYWNoZTtcclxuICAgICAgICAgICAgICAgIHJldHVybiByO1xyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgIH0sXHJcbiAgICB9O1xyXG4gICAgZm9yIChjb25zdCBwcm9wZXJ0eSBpbiBhZGRPbilcclxuICAgICAgICBpZiAoYWRkT24uaGFzT3duUHJvcGVydHkocHJvcGVydHkpKVxyXG4gICAgICAgICAgICBTeGNDb250cm9sbGVyW3Byb3BlcnR5XSA9IGFkZE9uW3Byb3BlcnR5XSBhcyBhbnk7XHJcbiAgICByZXR1cm4gU3hjQ29udHJvbGxlciBhcyBhbnkgYXMgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHM7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGF1dG9GaW5kKGRvbUVsZW1lbnQ6IEhUTUxFbGVtZW50KTogW251bWJlciwgbnVtYmVyXSB7XHJcbiAgICBjb25zdCBjb250YWluZXJUYWcgPSAkKGRvbUVsZW1lbnQpLmNsb3Nlc3QoJy5zYy1jb250ZW50LWJsb2NrJylbMF07XHJcbiAgICBpZiAoIWNvbnRhaW5lclRhZykgcmV0dXJuIG51bGw7XHJcbiAgICBjb25zdCBpaWQgPSBjb250YWluZXJUYWcuZ2V0QXR0cmlidXRlKCdkYXRhLWNiLWluc3RhbmNlJyk7XHJcbiAgICBjb25zdCBjYmlkID0gY29udGFpbmVyVGFnLmdldEF0dHJpYnV0ZSgnZGF0YS1jYi1pZCcpO1xyXG4gICAgaWYgKCFpaWQgfHwgIWNiaWQpIHJldHVybiBudWxsO1xyXG4gICAgcmV0dXJuIFtpaWQsIGNiaWRdO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzIGV4dGVuZHMgU3hjQ29udHJvbGxlciB7XHJcbiAgICAoaWQ6IG51bWJlciB8IEhUTUxFbGVtZW50LCBjYmlkPzogbnVtYmVyKTogU3hjSW5zdGFuY2UgfCBTeGNJbnN0YW5jZVdpdGhJbnRlcm5hbHM7XHJcbiAgICB0b3RhbFBvcHVwOiBUb3RhbFBvcHVwO1xyXG4gICAgdXJsUGFyYW1zOiBVcmxQYXJhbU1hbmFnZXI7XHJcbiAgICBiZXRhOiBhbnk7XHJcbiAgICBfY29udHJvbGxlcnM6IGFueTtcclxuICAgIF9kYXRhOiBhbnk7XHJcbiAgICBfbWFuYWdlOiBhbnk7XHJcbiAgICBfdHJhbnNsYXRlSW5pdDogYW55O1xyXG4gICAgZGVidWc6IGFueTtcclxuICAgIHBhcnRzOiBhbnk7XHJcblxyXG59XHJcblxyXG4vLyBSZVNoYXJwZXIgcmVzdG9yZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkNvbnRyb2xsZXIudHMiLCJcclxuaW1wb3J0IHsgU3hjQ29udHJvbGxlciwgU3hjQ29udHJvbGxlcldpdGhJbnRlcm5hbHMgfSBmcm9tICcuL1RvU2ljLlN4Yy5Db250cm9sbGVyJztcclxuaW1wb3J0IHsgU3hjRGF0YVdpdGhJbnRlcm5hbHMgfSBmcm9tICcuL1RvU2ljLlN4Yy5EYXRhJztcclxuaW1wb3J0IHsgU3hjV2ViQXBpV2l0aEludGVybmFscyB9IGZyb20gJy4vVG9TaWMuU3hjLldlYkFwaSc7XHJcbi8qKlxyXG4gKiBUaGUgdHlwaWNhbCBzeGMtaW5zdGFuY2Ugb2JqZWN0IGZvciBhIHNwZWNpZmljIEROTiBtb2R1bGUgb3IgY29udGVudC1ibG9ja1xyXG4gKi9cclxuZXhwb3J0IGNsYXNzIFN4Y0luc3RhbmNlIHtcclxuICAgIC8qKlxyXG4gICAgICogaGVscGVycyBmb3IgYWpheCBjYWxsc1xyXG4gICAgICovXHJcbiAgICB3ZWJBcGk6IFN4Y1dlYkFwaVdpdGhJbnRlcm5hbHM7XHJcbiAgICBwcm90ZWN0ZWQgc2VydmljZVJvb3Q6IHN0cmluZztcclxuICAgIHByaXZhdGUgcmVhZG9ubHkgc2VydmljZVNjb3BlcyA9IFsnYXBwJywgJ2FwcC1zeXMnLCAnYXBwLWFwaScsICdhcHAtcXVlcnknLCAnYXBwLWNvbnRlbnQnLCAnZWF2JywgJ3ZpZXcnLCAnZG5uJ107XHJcblxyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgICogdGhlIHN4Yy1pbnN0YW5jZSBJRCwgd2hpY2ggaXMgdXN1YWxseSB0aGUgRE5OIE1vZHVsZSBJZFxyXG4gICAgICAgICAqL1xyXG4gICAgICAgIHB1YmxpYyBpZDogbnVtYmVyLFxyXG5cclxuICAgICAgICAvKipcclxuICAgICAgICAgKiBjb250ZW50LWJsb2NrIElELCB3aGljaCBpcyBlaXRoZXIgdGhlIG1vZHVsZSBJRCwgb3IgdGhlIGNvbnRlbnQtYmxvY2sgZGVmaW5pdGlpb24gZW50aXR5IElEXHJcbiAgICAgICAgICogdGhpcyBpcyBhbiBhZHZhbmNlZCBjb25jZXB0IHlvdSB1c3VhbGx5IGRvbid0IGNhcmUgYWJvdXQsIG90aGVyd2lzZSB5b3Ugc2hvdWxkIHJlc2VhcmNoIGl0XHJcbiAgICAgICAgICovXHJcbiAgICAgICAgcHVibGljIGNiaWQ6IG51bWJlcixcclxuICAgICAgICBwcm90ZWN0ZWQgcmVhZG9ubHkgZG5uU2Y6IGFueSxcclxuICAgICkge1xyXG4gICAgICAgIHRoaXMuc2VydmljZVJvb3QgPSBkbm5TZihpZCkuZ2V0U2VydmljZVJvb3QoJzJzeGMnKTtcclxuICAgICAgICB0aGlzLndlYkFwaSA9IG5ldyBTeGNXZWJBcGlXaXRoSW50ZXJuYWxzKHRoaXMsIGlkLCBjYmlkKTtcclxuICAgIH1cclxuXHJcbiAgICAvKipcclxuICAgICAqIGNvbnZlcnRzIGEgc2hvcnQgYXBpLWNhbGwgcGF0aCBsaWtlIFwiL2FwcC9CbG9nL3F1ZXJ5L3h5elwiIHRvIHRoZSBETk4gZnVsbCBwYXRoXHJcbiAgICAgKiB3aGljaCB2YXJpZXMgZnJvbSBpbnN0YWxsYXRpb24gdG8gaW5zdGFsbGF0aW9uIGxpa2UgXCIvZGVza3RvcG1vZHVsZXMvYXBpLzJzeGMvYXBwLy4uLlwiXHJcbiAgICAgKiBAcGFyYW0gdmlydHVhbFBhdGhcclxuICAgICAqIEByZXR1cm5zIG1hcHBlZCBwYXRoXHJcbiAgICAgKi9cclxuICAgIHJlc29sdmVTZXJ2aWNlVXJsKHZpcnR1YWxQYXRoOiBzdHJpbmcpIHtcclxuICAgICAgICBjb25zdCBzY29wZSA9IHZpcnR1YWxQYXRoLnNwbGl0KCcvJylbMF0udG9Mb3dlckNhc2UoKTtcclxuXHJcbiAgICAgICAgLy8gc3RvcCBpZiBpdCdzIG5vdCBvbmUgb2Ygb3VyIHNwZWNpYWwgcGF0aHNcclxuICAgICAgICBpZiAodGhpcy5zZXJ2aWNlU2NvcGVzLmluZGV4T2Yoc2NvcGUpID09PSAtMSlcclxuICAgICAgICAgICAgcmV0dXJuIHZpcnR1YWxQYXRoO1xyXG5cclxuICAgICAgICByZXR1cm4gdGhpcy5zZXJ2aWNlUm9vdCArIHNjb3BlICsgJy8nICsgdmlydHVhbFBhdGguc3Vic3RyaW5nKHZpcnR1YWxQYXRoLmluZGV4T2YoJy8nKSArIDEpO1xyXG4gICAgfVxyXG5cclxuXHJcbiAgICAvLyBTaG93IGEgbmljZSBlcnJvciB3aXRoIG1vcmUgaW5mb3MgYXJvdW5kIDJzeGNcclxuICAgIHNob3dEZXRhaWxlZEh0dHBFcnJvcihyZXN1bHQ6IGFueSk6IGFueSB7XHJcbiAgICAgICAgaWYgKHdpbmRvdy5jb25zb2xlKVxyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhyZXN1bHQpO1xyXG5cclxuICAgICAgICBpZiAocmVzdWx0LnN0YXR1cyA9PT0gNDA0ICYmXHJcbiAgICAgICAgICAgIHJlc3VsdC5jb25maWcgJiZcclxuICAgICAgICAgICAgcmVzdWx0LmNvbmZpZy51cmwgJiZcclxuICAgICAgICAgICAgcmVzdWx0LmNvbmZpZy51cmwuaW5kZXhPZignL2Rpc3QvaTE4bi8nKSA+IC0xKSB7XHJcbiAgICAgICAgICAgIGlmICh3aW5kb3cuY29uc29sZSlcclxuICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKCdqdXN0IGZ5aTogZmFpbGVkIHRvIGxvYWQgbGFuZ3VhZ2UgcmVzb3VyY2U7IHdpbGwgaGF2ZSB0byB1c2UgZGVmYXVsdCcpO1xyXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0O1xyXG4gICAgICAgIH1cclxuXHJcblxyXG4gICAgICAgIC8vIGlmIGl0J3MgYW4gdW5zcGVjaWZpZWQgMC1lcnJvciwgaXQncyBwcm9iYWJseSBub3QgYW4gZXJyb3IgYnV0IGEgY2FuY2VsbGVkIHJlcXVlc3QsXHJcbiAgICAgICAgLy8gKGhhcHBlbnMgd2hlbiBjbG9zaW5nIHBvcHVwcyBjb250YWluaW5nIGFuZ3VsYXJKUylcclxuICAgICAgICBpZiAocmVzdWx0LnN0YXR1cyA9PT0gMCB8fCByZXN1bHQuc3RhdHVzID09PSAtMSlcclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcclxuXHJcbiAgICAgICAgLy8gbGV0J3MgdHJ5IHRvIHNob3cgZ29vZCBtZXNzYWdlcyBpbiBtb3N0IGNhc2VzXHJcbiAgICAgICAgbGV0IGluZm9UZXh0ID0gJ0hhZCBhbiBlcnJvciB0YWxraW5nIHRvIHRoZSBzZXJ2ZXIgKHN0YXR1cyAnICsgcmVzdWx0LnN0YXR1cyArICcpLic7XHJcbiAgICAgICAgY29uc3Qgc3J2UmVzcCA9IHJlc3VsdC5yZXNwb25zZVRleHRcclxuICAgICAgICAgICAgPyBKU09OLnBhcnNlKHJlc3VsdC5yZXNwb25zZVRleHQpIC8vIGZvciBqcXVlcnkgYWpheCBlcnJvcnNcclxuICAgICAgICAgICAgOiByZXN1bHQuZGF0YTsgLy8gZm9yIGFuZ3VsYXIgJGh0dHBcclxuICAgICAgICBpZiAoc3J2UmVzcCkge1xyXG4gICAgICAgICAgICBjb25zdCBtc2cgPSBzcnZSZXNwLk1lc3NhZ2U7XHJcbiAgICAgICAgICAgIGlmIChtc2cpIGluZm9UZXh0ICs9ICdcXG5NZXNzYWdlOiAnICsgbXNnO1xyXG4gICAgICAgICAgICBjb25zdCBtc2dEZXQgPSBzcnZSZXNwLk1lc3NhZ2VEZXRhaWwgfHwgc3J2UmVzcC5FeGNlcHRpb25NZXNzYWdlO1xyXG4gICAgICAgICAgICBpZiAobXNnRGV0KSBpbmZvVGV4dCArPSAnXFxuRGV0YWlsOiAnICsgbXNnRGV0O1xyXG5cclxuXHJcbiAgICAgICAgICAgIGlmIChtc2dEZXQgJiYgbXNnRGV0LmluZGV4T2YoJ05vIGFjdGlvbiB3YXMgZm91bmQnKSA9PT0gMClcclxuICAgICAgICAgICAgICAgIGlmIChtc2dEZXQuaW5kZXhPZigndGhhdCBtYXRjaGVzIHRoZSBuYW1lJykgPiAwKVxyXG4gICAgICAgICAgICAgICAgICAgIGluZm9UZXh0ICs9ICdcXG5cXG5UaXAgZnJvbSAyc3hjOiB5b3UgcHJvYmFibHkgZ290IHRoZSBhY3Rpb24tbmFtZSB3cm9uZyBpbiB5b3VyIEpTLic7XHJcbiAgICAgICAgICAgICAgICBlbHNlIGlmIChtc2dEZXQuaW5kZXhPZigndGhhdCBtYXRjaGVzIHRoZSByZXF1ZXN0LicpID4gMClcclxuICAgICAgICAgICAgICAgICAgICBpbmZvVGV4dCArPSAnXFxuXFxuVGlwIGZyb20gMnN4YzogU2VlbXMgbGlrZSB0aGUgcGFyYW1ldGVycyBhcmUgdGhlIHdyb25nIGFtb3VudCBvciB0eXBlLic7XHJcblxyXG4gICAgICAgICAgICBpZiAobXNnICYmIG1zZy5pbmRleE9mKCdDb250cm9sbGVyJykgPT09IDAgJiYgbXNnLmluZGV4T2YoJ25vdCBmb3VuZCcpID4gMClcclxuICAgICAgICAgICAgICAgIGluZm9UZXh0ICs9XHJcbiAgICAgICAgICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm1heC1saW5lLWxlbmd0aFxyXG4gICAgICAgICAgICAgICAgICAgIFwiXFxuXFxuVGlwIGZyb20gMnN4YzogeW91IHByb2JhYmx5IHNwZWxsZWQgdGhlIGNvbnRyb2xsZXIgbmFtZSB3cm9uZyBvciBmb3Jnb3QgdG8gcmVtb3ZlIHRoZSB3b3JkICdjb250cm9sbGVyJyBmcm9tIHRoZSBjYWxsIGluIEpTLiBUbyBjYWxsIGEgY29udHJvbGxlciBjYWxsZWQgJ0RlbW9Db250cm9sbGVyJyBvbmx5IHVzZSAnRGVtbycuXCI7XHJcblxyXG4gICAgICAgIH1cclxuICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bWF4LWxpbmUtbGVuZ3RoXHJcbiAgICAgICAgaW5mb1RleHQgKz0gJ1xcblxcbmlmIHlvdSBhcmUgYW4gYWR2YW5jZWQgdXNlciB5b3UgY2FuIGxlYXJuIG1vcmUgYWJvdXQgd2hhdCB3ZW50IHdyb25nIC0gZGlzY292ZXIgaG93IG9uIDJzeGMub3JnL2hlbHA/dGFnPWRlYnVnJztcclxuICAgICAgICBhbGVydChpbmZvVGV4dCk7XHJcblxyXG4gICAgICAgIHJldHVybiByZXN1bHQ7XHJcbiAgICB9XHJcbn1cclxuXHJcbi8qKlxyXG4gKiBFbmhhbmNlZCBzeGMgaW5zdGFuY2Ugd2l0aCBhZGRpdGlvbmFsIGVkaXRpbmcgZnVuY3Rpb25hbGl0eVxyXG4gKiBVc2UgdGhpcywgaWYgeW91IGludGVuZCB0byBydW4gY29udGVudC1tYW5hZ2VtZW50IGNvbW1hbmRzIGxpa2UgXCJlZGl0XCIgZnJvbSB5b3VyIEpTIGRpcmVjdGx5XHJcbiAqL1xyXG5leHBvcnQgY2xhc3MgU3hjSW5zdGFuY2VXaXRoRWRpdGluZyBleHRlbmRzIFN4Y0luc3RhbmNlIHtcclxuICAgIC8qKlxyXG4gICAgICogbWFuYWdlIG9iamVjdCB3aGljaCBwcm92aWRlcyBhY2Nlc3MgdG8gYWRkaXRpb25hbCBjb250ZW50LW1hbmFnZW1lbnQgZmVhdHVyZXNcclxuICAgICAqIGl0IG9ubHkgZXhpc3RzIGlmIDJzeGMgaXMgaW4gZWRpdCBtb2RlIChvdGhlcndpc2UgdGhlIEpTIGFyZSBub3QgaW5jbHVkZWQgZm9yIHRoZXNlIGZlYXR1cmVzKVxyXG4gICAgICovXHJcbiAgICBtYW5hZ2U6IGFueSA9IG51bGw7IC8vIGluaXRpYWxpemUgY29ycmVjdGx5IGxhdGVyIG9uXHJcblxyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgcHVibGljIGlkOiBudW1iZXIsXHJcbiAgICAgICAgcHVibGljIGNiaWQ6IG51bWJlcixcclxuLy8gUmVTaGFycGVyIGRpc2FibGUgb25jZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuICAgICAgICBwcm90ZWN0ZWQgJDJzeGM6IFN4Y0NvbnRyb2xsZXJXaXRoSW50ZXJuYWxzLFxyXG4gICAgICAgIHByb3RlY3RlZCByZWFkb25seSBkbm5TZjogYW55LFxyXG4gICAgKSB7XHJcbiAgICAgICAgc3VwZXIoaWQsIGNiaWQsIGRublNmKTtcclxuXHJcbiAgICAgICAgLy8gYWRkIG1hbmFnZSBwcm9wZXJ0eSwgYnV0IG5vdCB3aXRoaW4gaW5pdGlhbGl6ZXIsIGJlY2F1c2UgaW5zaWRlIHRoZSBtYW5hZ2UtaW5pdGlhbGl6ZXIgaXQgbWF5IHJlZmVyZW5jZSAyc3hjIGFnYWluXHJcbiAgICAgICAgdHJ5IHsgLy8gc29tZXRpbWVzIHRoZSBtYW5hZ2UgY2FuJ3QgYmUgYnVpbHQsIGxpa2UgYmVmb3JlIGluc3RhbGxpbmdcclxuICAgICAgICAgICAgaWYgKCQyc3hjLl9tYW5hZ2UpICQyc3hjLl9tYW5hZ2UuaW5pdEluc3RhbmNlKHRoaXMpO1xyXG4gICAgICAgIH0gY2F0Y2ggKGUpIHtcclxuICAgICAgICAgICAgY29uc29sZS5lcnJvcignZXJyb3IgaW4gMnN4YyAtIHdpbGwgb25seSBsb2cgYnV0IG5vdCB0aHJvdycsIGUpO1xyXG4gICAgICAgICAgICAvLyB0aHJvdyBlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gdGhpcyBvbmx5IHdvcmtzIHdoZW4gbWFuYWdlIGV4aXN0cyAobm90IGluc3RhbGxpbmcpIGFuZCB0cmFuc2xhdG9yIGV4aXN0cyB0b29cclxuICAgICAgICBpZiAoJDJzeGMuX3RyYW5zbGF0ZUluaXQgJiYgdGhpcy5tYW5hZ2UpICQyc3hjLl90cmFuc2xhdGVJbml0KHRoaXMubWFuYWdlKTsgICAgLy8gaW5pdCB0cmFuc2xhdGUsIG5vdCByZWFsbHkgbmljZSwgYnV0IG9rIGZvciBub3dcclxuXHJcbiAgICB9XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBjaGVja3MgaWYgd2UncmUgY3VycmVudGx5IGluIGVkaXQgbW9kZVxyXG4gICAgICogQHJldHVybnMge2Jvb2xlYW59XHJcbiAgICAgKi9cclxuICAgIGlzRWRpdE1vZGUoKSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMubWFuYWdlICYmIHRoaXMubWFuYWdlLl9pc0VkaXRNb2RlKCk7XHJcbiAgICB9XHJcblxyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIGV4dGVuZHMgU3hjSW5zdGFuY2VXaXRoRWRpdGluZyB7XHJcbiAgICBkYXRhOiBTeGNEYXRhV2l0aEludGVybmFscztcclxuICAgIHNvdXJjZTogYW55ID0gbnVsbDtcclxuICAgIGlzTG9hZGVkID0gZmFsc2U7XHJcbiAgICBsYXN0UmVmcmVzaDogRGF0ZSA9IG51bGw7XHJcblxyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgICAgcHVibGljIGlkOiBudW1iZXIsXHJcbiAgICAgICAgcHVibGljIGNiaWQ6IG51bWJlcixcclxuICAgICAgICBwcml2YXRlIGNhY2hlS2V5OiBzdHJpbmcsXHJcbi8vIFJlU2hhcnBlciBkaXNhYmxlIG9uY2UgSW5jb25zaXN0ZW50TmFtaW5nXHJcbiAgICAgICAgcHJvdGVjdGVkICQyc3hjOiBTeGNDb250cm9sbGVyV2l0aEludGVybmFscyxcclxuICAgICAgICBwcm90ZWN0ZWQgcmVhZG9ubHkgZG5uU2Y6IGFueSxcclxuICAgICkge1xyXG4gICAgICAgIHN1cGVyKGlkLCBjYmlkLCAkMnN4YywgZG5uU2YpO1xyXG4gICAgICAgIHRoaXMuZGF0YSA9IG5ldyBTeGNEYXRhV2l0aEludGVybmFscyh0aGlzKTtcclxuICAgIH1cclxuXHJcbiAgICByZWNyZWF0ZShyZXNldENhY2hlOiBib29sZWFuKTogU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIHtcclxuICAgICAgICBpZiAocmVzZXRDYWNoZSkgZGVsZXRlIHRoaXMuJDJzeGMuX2NvbnRyb2xsZXJzW3RoaXMuY2FjaGVLZXldOyAvLyBjbGVhciBjYWNoZVxyXG4gICAgICAgIHJldHVybiB0aGlzLiQyc3hjKHRoaXMuaWQsIHRoaXMuY2JpZCkgYXMgYW55IGFzIFN4Y0luc3RhbmNlV2l0aEludGVybmFsczsgLy8gZ2VuZXJhdGUgbmV3XHJcbiAgICB9XHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLkluc3RhbmNlLnRzIiwiaW1wb3J0IHsgU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzIH0gZnJvbSAnLi9Ub1NpYy5TeGMuSW5zdGFuY2UnO1xyXG5cclxuZGVjbGFyZSBjb25zdCAkOiBhbnk7XHJcblxyXG5cclxuZXhwb3J0IGNsYXNzIFN4Y0RhdGFXaXRoSW50ZXJuYWxzIHtcclxuICAgIHNvdXJjZTogYW55ID0gdW5kZWZpbmVkO1xyXG5cclxuICAgIC8vIGluLXN0cmVhbXNcclxuICAgIFwiaW5cIjogYW55ID0ge307XHJcblxyXG4gICAgLy8gd2lsbCBob2xkIHRoZSBkZWZhdWx0IHN0cmVhbSAoW1wiaW5cIl1bXCJEZWZhdWx0XCJdLkxpc3RcclxuLy8gUmVTaGFycGVyIGRpc2FibGUgb25jZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuICAgIExpc3Q6IGFueSA9IFtdO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIHByaXZhdGUgY29udHJvbGxlcjogU3hjSW5zdGFuY2VXaXRoSW50ZXJuYWxzLFxyXG4gICAgKSB7XHJcblxyXG4gICAgfVxyXG5cclxuICAgIC8vIHNvdXJjZSBwYXRoIGRlZmF1bHRpbmcgdG8gY3VycmVudCBwYWdlICsgb3B0aW9uYWwgcGFyYW1zXHJcbiAgICBzb3VyY2VVcmwocGFyYW1zPzogc3RyaW5nKTogc3RyaW5nIHtcclxuICAgICAgICBsZXQgdXJsID0gdGhpcy5jb250cm9sbGVyLnJlc29sdmVTZXJ2aWNlVXJsKCdhcHAtc3lzL2FwcGNvbnRlbnQvR2V0Q29udGVudEJsb2NrRGF0YScpO1xyXG4gICAgICAgIGlmICh0eXBlb2YgcGFyYW1zID09PSAnc3RyaW5nJykgLy8gdGV4dCBsaWtlICdpZD03J1xyXG4gICAgICAgICAgICB1cmwgKz0gJyYnICsgcGFyYW1zO1xyXG4gICAgICAgIHJldHVybiB1cmw7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIC8vIGxvYWQgZGF0YSB2aWEgYWpheFxyXG4gICAgbG9hZChzb3VyY2U/OiBhbnkpIHtcclxuICAgICAgICAvLyBpZiBzb3VyY2UgaXMgYWxyZWFkeSB0aGUgZGF0YSwgc2V0IGl0XHJcbiAgICAgICAgaWYgKHNvdXJjZSAmJiBzb3VyY2UuTGlzdCkge1xyXG4gICAgICAgICAgICAvLyAyMDE3LTA5LTA1IDJkbTogZGlzY292ZXJkIGEgY2FsbCB0byBhbiBpbmV4aXN0aW5nIGZ1bmN0aW9uXHJcbiAgICAgICAgICAgIC8vIHNpbmNlIHRoaXMgaXMgYW4gb2xkIEFQSSB3aGljaCBpcyBiZWluZyBkZXByZWNhdGVkLCBwbGVhc2UgZG9uJ3QgZml4IHVubGVzcyB3ZSBnZXQgYWN0aXZlIGZlZWRiYWNrXHJcbiAgICAgICAgICAgIC8vIGNvbnRyb2xsZXIuZGF0YS5zZXREYXRhKHNvdXJjZSk7XHJcbiAgICAgICAgICAgIHJldHVybiB0aGlzLmNvbnRyb2xsZXIuZGF0YTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBpZiAoIXNvdXJjZSlcclxuICAgICAgICAgICAgICAgIHNvdXJjZSA9IHt9O1xyXG4gICAgICAgICAgICBpZiAoIXNvdXJjZS51cmwpXHJcbiAgICAgICAgICAgICAgICBzb3VyY2UudXJsID0gdGhpcy5jb250cm9sbGVyLmRhdGEuc291cmNlVXJsKCk7XHJcbiAgICAgICAgICAgIHNvdXJjZS5vcmlnU3VjY2VzcyA9IHNvdXJjZS5zdWNjZXNzO1xyXG4gICAgICAgICAgICBzb3VyY2Uuc3VjY2VzcyA9IChkYXRhOiBhbnkpID0+IHtcclxuXHJcbiAgICAgICAgICAgICAgICBmb3IgKGNvbnN0IGRhdGFTZXROYW1lIGluIGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZGF0YS5oYXNPd25Qcm9wZXJ0eShkYXRhU2V0TmFtZSkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChkYXRhW2RhdGFTZXROYW1lXS5MaXN0ICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmNvbnRyb2xsZXIuZGF0YS5pbltkYXRhU2V0TmFtZV0gPSBkYXRhW2RhdGFTZXROYW1lXTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuY29udHJvbGxlci5kYXRhLmluW2RhdGFTZXROYW1lXS5uYW1lID0gZGF0YVNldE5hbWU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5jb250cm9sbGVyLmRhdGEuaW4uRGVmYXVsdClcclxuICAgICAgICAgICAgICAgICAgICAvLyAyMDE3LTA5LTA1IDJkbTogcHJldmlvdXNseSB3cm90ZSBpdCB0byBjb250cm9sbGVyLkxpc3QsIGJ1dCB0aGlzIGlzIGFsbW9zdCBjZXJ0YWlubHkgYSBtaXN0YWtlXHJcbiAgICAgICAgICAgICAgICAgICAgLy8gc2luY2UgaXQncyBhbiBvbGQgQVBJIHdoaWNoIGlzIGJlaW5nIGRlcHJlY2F0ZWQsIHdlIHdvbid0IGZpeCBpdFxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuTGlzdCA9IHRoaXMuaW4uRGVmYXVsdC5MaXN0O1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChzb3VyY2Uub3JpZ1N1Y2Nlc3MpXHJcbiAgICAgICAgICAgICAgICAgICAgc291cmNlLm9yaWdTdWNjZXNzKHRoaXMpO1xyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuY29udHJvbGxlci5pc0xvYWRlZCA9IHRydWU7XHJcbiAgICAgICAgICAgICAgICB0aGlzLmNvbnRyb2xsZXIubGFzdFJlZnJlc2ggPSBuZXcgRGF0ZSgpO1xyXG4gICAgICAgICAgICAgICAgKHRoaXMgYXMgYW55KS5fdHJpZ2dlckxvYWRlZCgpO1xyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICBzb3VyY2UuZXJyb3IgPSAocmVxdWVzdDogYW55KSA9PiB7IGFsZXJ0KHJlcXVlc3Quc3RhdHVzVGV4dCk7IH07XHJcbiAgICAgICAgICAgIHNvdXJjZS5wcmV2ZW50QXV0b0ZhaWwgPSB0cnVlOyAvLyB1c2Ugb3VyIGZhaWwgbWVzc2FnZVxyXG4gICAgICAgICAgICB0aGlzLnNvdXJjZSA9IHNvdXJjZTtcclxuICAgICAgICAgICAgcmV0dXJuIHRoaXMucmVsb2FkKCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHJlbG9hZCgpOiBTeGNEYXRhV2l0aEludGVybmFscyB7XHJcbiAgICAgICAgdGhpcy5jb250cm9sbGVyLndlYkFwaS5nZXQodGhpcy5zb3VyY2UpXHJcbiAgICAgICAgICAgIC50aGVuKHRoaXMuc291cmNlLnN1Y2Nlc3MsIHRoaXMuc291cmNlLmVycm9yKTtcclxuICAgICAgICByZXR1cm4gdGhpcztcclxuICAgIH1cclxuXHJcbiAgICBvbihldmVudHM6IEV2ZW50LCBjYWxsYmFjazogKCkgPT4gdm9pZCk6IFByb21pc2U8YW55PiB7XHJcbiAgICAgICAgcmV0dXJuICQodGhpcykuYmluZCgnMnNjTG9hZCcsIGNhbGxiYWNrKVswXS5fdHJpZ2dlckxvYWRlZCgpO1xyXG4gICAgfVxyXG5cclxuLy8gUmVTaGFycGVyIGRpc2FibGUgb25jZSBJbmNvbnNpc3RlbnROYW1pbmdcclxuICAgIF90cmlnZ2VyTG9hZGVkKCk6IFByb21pc2U8YW55PiB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuY29udHJvbGxlci5pc0xvYWRlZFxyXG4gICAgICAgICAgICA/ICQodGhpcykudHJpZ2dlcignMnNjTG9hZCcsIFt0aGlzXSlbMF1cclxuICAgICAgICAgICAgOiB0aGlzO1xyXG4gICAgfVxyXG5cclxuICAgIG9uZShldmVudHM6IEV2ZW50LCBjYWxsYmFjazogKHg6IGFueSwgeTogYW55KSA9PiB2b2lkKTogU3hjRGF0YVdpdGhJbnRlcm5hbHMge1xyXG4gICAgICAgIGlmICghdGhpcy5jb250cm9sbGVyLmlzTG9hZGVkKVxyXG4gICAgICAgICAgICByZXR1cm4gJCh0aGlzKS5vbmUoJzJzY0xvYWQnLCBjYWxsYmFjaylbMF07XHJcbiAgICAgICAgY2FsbGJhY2soe30sIHRoaXMpO1xyXG4gICAgICAgIHJldHVybiB0aGlzO1xyXG4gICAgfVxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuLzJzeGMtYXBpL2pzL1RvU2ljLlN4Yy5EYXRhLnRzIiwiXHJcbmRlY2xhcmUgY29uc3QgJDogYW55O1xyXG5pbXBvcnQgeyBTeGNJbnN0YW5jZSB9IGZyb20gJy4vVG9TaWMuU3hjLkluc3RhbmNlJztcclxuXHJcbi8qKlxyXG4gKiBoZWxwZXIgQVBJIHRvIHJ1biBhamF4IC8gUkVTVCBjYWxscyB0byB0aGUgc2VydmVyXHJcbiAqIGl0IHdpbGwgZW5zdXJlIHRoYXQgdGhlIGhlYWRlcnMgZXRjLiBhcmUgc2V0IGNvcnJlY3RseVxyXG4gKiBhbmQgdGhhdCB1cmxzIGFyZSByZXdyaXR0ZW5cclxuICovXHJcbmV4cG9ydCBjbGFzcyBTeGNXZWJBcGlXaXRoSW50ZXJuYWxzIHtcclxuICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgIHByaXZhdGUgcmVhZG9ubHkgY29udHJvbGxlcjogU3hjSW5zdGFuY2UsXHJcbiAgICAgICAgcHJpdmF0ZSByZWFkb25seSBpZDogbnVtYmVyLFxyXG4gICAgICAgIHByaXZhdGUgcmVhZG9ubHkgY2JpZDogbnVtYmVyLFxyXG4gICAgKSB7XHJcblxyXG4gICAgfVxyXG4gICAgLyoqXHJcbiAgICAgKiByZXR1cm5zIGFuIGh0dHAtZ2V0IHByb21pc2VcclxuICAgICAqIEBwYXJhbSBzZXR0aW5nc09yVXJsIHRoZSB1cmwgdG8gZ2V0XHJcbiAgICAgKiBAcGFyYW0gcGFyYW1zIGpRdWVyeSBzdHlsZSBhamF4IHBhcmFtZXRlcnNcclxuICAgICAqIEBwYXJhbSBkYXRhIGpRdWVyeSBzdHlsZSBkYXRhIGZvciBwb3N0L3B1dCByZXF1ZXN0c1xyXG4gICAgICogQHBhcmFtIHByZXZlbnRBdXRvRmFpbFxyXG4gICAgICogQHJldHVybnMge1Byb21pc2V9IGpRdWVyeSBhamF4IHByb21pc2Ugb2JqZWN0XHJcbiAgICAgKi9cclxuICAgIGdldChzZXR0aW5nc09yVXJsOiBzdHJpbmcgfCBhbnksIHBhcmFtcz86IGFueSwgZGF0YT86IGFueSwgcHJldmVudEF1dG9GYWlsPzogYm9vbGVhbik6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMucmVxdWVzdChzZXR0aW5nc09yVXJsLCBwYXJhbXMsIGRhdGEsIHByZXZlbnRBdXRvRmFpbCwgJ0dFVCcpO1xyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhbiBodHRwLWdldCBwcm9taXNlXHJcbiAgICAgKiBAcGFyYW0gc2V0dGluZ3NPclVybCB0aGUgdXJsIHRvIGdldFxyXG4gICAgICogQHBhcmFtIHBhcmFtcyBqUXVlcnkgc3R5bGUgYWpheCBwYXJhbWV0ZXJzXHJcbiAgICAgKiBAcGFyYW0gZGF0YSBqUXVlcnkgc3R5bGUgZGF0YSBmb3IgcG9zdC9wdXQgcmVxdWVzdHNcclxuICAgICAqIEBwYXJhbSBwcmV2ZW50QXV0b0ZhaWxcclxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBqUXVlcnkgYWpheCBwcm9taXNlIG9iamVjdFxyXG4gICAgICovXHJcbiAgICBwb3N0KHNldHRpbmdzT3JVcmw6IHN0cmluZyB8IGFueSwgcGFyYW1zPzogYW55LCBkYXRhPzogYW55LCBwcmV2ZW50QXV0b0ZhaWw/OiBib29sZWFuKTogYW55IHtcclxuICAgICAgICByZXR1cm4gdGhpcy5yZXF1ZXN0KHNldHRpbmdzT3JVcmwsIHBhcmFtcywgZGF0YSwgcHJldmVudEF1dG9GYWlsLCAnUE9TVCcpO1xyXG4gICAgfVxyXG5cclxuICAgIC8qKlxyXG4gICAgICogcmV0dXJucyBhbiBodHRwLWdldCBwcm9taXNlXHJcbiAgICAgKiBAcGFyYW0gc2V0dGluZ3NPclVybCB0aGUgdXJsIHRvIGdldFxyXG4gICAgICogQHBhcmFtIHBhcmFtcyBqUXVlcnkgc3R5bGUgYWpheCBwYXJhbWV0ZXJzXHJcbiAgICAgKiBAcGFyYW0gZGF0YSBqUXVlcnkgc3R5bGUgZGF0YSBmb3IgcG9zdC9wdXQgcmVxdWVzdHNcclxuICAgICAqIEBwYXJhbSBwcmV2ZW50QXV0b0ZhaWxcclxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBqUXVlcnkgYWpheCBwcm9taXNlIG9iamVjdFxyXG4gICAgICovXHJcbiAgICBkZWxldGUoc2V0dGluZ3NPclVybDogc3RyaW5nIHwgYW55LCBwYXJhbXM/OiBhbnksIGRhdGE/OiBhbnksIHByZXZlbnRBdXRvRmFpbD86IGJvb2xlYW4pOiBhbnkge1xyXG4gICAgICAgIHJldHVybiB0aGlzLnJlcXVlc3Qoc2V0dGluZ3NPclVybCwgcGFyYW1zLCBkYXRhLCBwcmV2ZW50QXV0b0ZhaWwsICdERUxFVEUnKTtcclxuICAgIH1cclxuXHJcbiAgICAvKipcclxuICAgICAqIHJldHVybnMgYW4gaHR0cC1nZXQgcHJvbWlzZVxyXG4gICAgICogQHBhcmFtIHNldHRpbmdzT3JVcmwgdGhlIHVybCB0byBnZXRcclxuICAgICAqIEBwYXJhbSBwYXJhbXMgalF1ZXJ5IHN0eWxlIGFqYXggcGFyYW1ldGVyc1xyXG4gICAgICogQHBhcmFtIGRhdGEgalF1ZXJ5IHN0eWxlIGRhdGEgZm9yIHBvc3QvcHV0IHJlcXVlc3RzXHJcbiAgICAgKiBAcGFyYW0gcHJldmVudEF1dG9GYWlsXHJcbiAgICAgKiBAcmV0dXJucyB7UHJvbWlzZX0galF1ZXJ5IGFqYXggcHJvbWlzZSBvYmplY3RcclxuICAgICAqL1xyXG4gICAgcHV0KHNldHRpbmdzT3JVcmw6IHN0cmluZyB8IGFueSwgcGFyYW1zPzogYW55LCBkYXRhPzogYW55LCBwcmV2ZW50QXV0b0ZhaWw/OiBib29sZWFuKTogYW55IHtcclxuICAgICAgICByZXR1cm4gdGhpcy5yZXF1ZXN0KHNldHRpbmdzT3JVcmwsIHBhcmFtcywgZGF0YSwgcHJldmVudEF1dG9GYWlsLCAnUFVUJyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZXF1ZXN0KHNldHRpbmdzOiBzdHJpbmcgfCBhbnksIHBhcmFtczogYW55LCBkYXRhOiBhbnksIHByZXZlbnRBdXRvRmFpbDogYm9vbGVhbiwgbWV0aG9kOiBzdHJpbmcpOiBhbnkge1xyXG5cclxuICAgICAgICAvLyB1cmwgcGFyYW1ldGVyOiBhdXRvIGNvbnZlcnQgYSBzaW5nbGUgdmFsdWUgKGluc3RlYWQgb2Ygb2JqZWN0IG9mIHZhbHVlcykgdG8gYW4gaWQ9Li4uIHBhcmFtZXRlclxyXG4gICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpjdXJseVxyXG4gICAgICAgIGlmICh0eXBlb2YgcGFyYW1zICE9PSAnb2JqZWN0JyAmJiB0eXBlb2YgcGFyYW1zICE9PSAndW5kZWZpbmVkJylcclxuICAgICAgICAgICAgcGFyYW1zID0geyBpZDogcGFyYW1zIH07XHJcblxyXG4gICAgICAgIC8vIGlmIHRoZSBmaXJzdCBwYXJhbWV0ZXIgaXMgYSBzdHJpbmcsIHJlc29sdmUgc2V0dGluZ3NcclxuICAgICAgICBpZiAodHlwZW9mIHNldHRpbmdzID09PSAnc3RyaW5nJykge1xyXG4gICAgICAgICAgICBjb25zdCBjb250cm9sbGVyQWN0aW9uID0gc2V0dGluZ3Muc3BsaXQoJy8nKTtcclxuICAgICAgICAgICAgY29uc3QgY29udHJvbGxlck5hbWUgPSBjb250cm9sbGVyQWN0aW9uWzBdO1xyXG4gICAgICAgICAgICBjb25zdCBhY3Rpb25OYW1lID0gY29udHJvbGxlckFjdGlvblsxXTtcclxuXHJcbiAgICAgICAgICAgIGlmIChjb250cm9sbGVyTmFtZSA9PT0gJycgfHwgYWN0aW9uTmFtZSA9PT0gJycpXHJcbiAgICAgICAgICAgICAgICBhbGVydCgnRXJyb3I6IGNvbnRyb2xsZXIgb3IgYWN0aW9uIG5vdCBkZWZpbmVkLiBXaWxsIGNvbnRpbnVlIHdpdGggbGlrZWx5IGVycm9ycy4nKTtcclxuXHJcbiAgICAgICAgICAgIHNldHRpbmdzID0ge1xyXG4gICAgICAgICAgICAgICAgY29udHJvbGxlcjogY29udHJvbGxlck5hbWUsXHJcbiAgICAgICAgICAgICAgICBhY3Rpb246IGFjdGlvbk5hbWUsXHJcbiAgICAgICAgICAgICAgICBwYXJhbXMsXHJcbiAgICAgICAgICAgICAgICBkYXRhLFxyXG4gICAgICAgICAgICAgICAgdXJsOiBjb250cm9sbGVyQWN0aW9uLmxlbmd0aCA+IDIgPyBzZXR0aW5ncyA6IG51bGwsXHJcbiAgICAgICAgICAgICAgICBwcmV2ZW50QXV0b0ZhaWwsXHJcbiAgICAgICAgICAgIH07XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBkZWZhdWx0cyA9IHtcclxuICAgICAgICAgICAgbWV0aG9kOiBtZXRob2QgPT09IG51bGwgPyAnUE9TVCcgOiBtZXRob2QsXHJcbiAgICAgICAgICAgIHBhcmFtczogbnVsbCBhcyBhbnksXHJcbiAgICAgICAgICAgIHByZXZlbnRBdXRvRmFpbDogZmFsc2UsXHJcbiAgICAgICAgfTtcclxuICAgICAgICBzZXR0aW5ncyA9ICQuZXh0ZW5kKHt9LCBkZWZhdWx0cywgc2V0dGluZ3MpO1xyXG4gICAgICAgIGNvbnN0IHNmID0gJC5TZXJ2aWNlc0ZyYW1ld29yayh0aGlzLmlkKTtcclxuICAgICAgICBjb25zdCBjYmlkID0gdGhpcy5jYmlkOyAvLyBtdXN0IHJlYWQgaGVyZSwgYXMgdGhlIFwidGhpc1wiIHdpbGwgY2hhbmdlIGluc2lkZSB0aGUgbWV0aG9kXHJcblxyXG4gICAgICAgIGNvbnN0IHByb21pc2UgPSAkLmFqYXgoe1xyXG4gICAgICAgICAgICBhc3luYzogdHJ1ZSxcclxuICAgICAgICAgICAgZGF0YVR5cGU6IHNldHRpbmdzLmRhdGFUeXBlIHx8ICdqc29uJywgLy8gZGVmYXVsdCBpcyBqc29uIGlmIG5vdCBzcGVjaWZpZWRcclxuICAgICAgICAgICAgZGF0YTogSlNPTi5zdHJpbmdpZnkoc2V0dGluZ3MuZGF0YSksXHJcbiAgICAgICAgICAgIGNvbnRlbnRUeXBlOiAnYXBwbGljYXRpb24vanNvbicsXHJcbiAgICAgICAgICAgIHR5cGU6IHNldHRpbmdzLm1ldGhvZCxcclxuICAgICAgICAgICAgdXJsOiB0aGlzLmdldEFjdGlvblVybChzZXR0aW5ncyksXHJcbiAgICAgICAgICAgIGJlZm9yZVNlbmQoeGhyOiBhbnkpIHtcclxuICAgICAgICAgICAgICAgIHhoci5zZXRSZXF1ZXN0SGVhZGVyKCdDb250ZW50QmxvY2tJZCcsIGNiaWQpO1xyXG4gICAgICAgICAgICAgICAgc2Yuc2V0TW9kdWxlSGVhZGVycyh4aHIpO1xyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBpZiAoIXNldHRpbmdzLnByZXZlbnRBdXRvRmFpbClcclxuICAgICAgICAgICAgcHJvbWlzZS5mYWlsKHRoaXMuY29udHJvbGxlci5zaG93RGV0YWlsZWRIdHRwRXJyb3IpO1xyXG5cclxuICAgICAgICByZXR1cm4gcHJvbWlzZTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGdldEFjdGlvblVybChzZXR0aW5nczogYW55KTogc3RyaW5nIHtcclxuICAgICAgICBjb25zdCBzZiA9ICQuU2VydmljZXNGcmFtZXdvcmsodGhpcy5pZCk7XHJcbiAgICAgICAgY29uc3QgYmFzZSA9IChzZXR0aW5ncy51cmwpXHJcbiAgICAgICAgICAgID8gdGhpcy5jb250cm9sbGVyLnJlc29sdmVTZXJ2aWNlVXJsKHNldHRpbmdzLnVybClcclxuICAgICAgICAgICAgOiBzZi5nZXRTZXJ2aWNlUm9vdCgnMnN4YycpICsgJ2FwcC9hdXRvL2FwaS8nICsgc2V0dGluZ3MuY29udHJvbGxlciArICcvJyArIHNldHRpbmdzLmFjdGlvbjtcclxuICAgICAgICByZXR1cm4gYmFzZSArIChzZXR0aW5ncy5wYXJhbXMgPT09IG51bGwgPyAnJyA6ICgnPycgKyAkLnBhcmFtKHNldHRpbmdzLnBhcmFtcykpKTtcclxuICAgIH1cclxuXHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLldlYkFwaS50cyIsIlxyXG5leHBvcnQgY2xhc3MgVG90YWxQb3B1cCB7XHJcbiAgICBmcmFtZTogYW55ID0gdW5kZWZpbmVkO1xyXG4gICAgY2FsbGJhY2s6IGFueSA9IHVuZGVmaW5lZDtcclxuXHJcbiAgICBvcGVuKHVybDogc3RyaW5nLCBjYWxsYmFjazogKCkgPT4gdm9pZCk6IHZvaWQge1xyXG4gICAgICAgIC8vIGNvdW50IHBhcmVudHMgdG8gc2VlIGhvdyBoaWdoIHRoZSB6LWluZGV4IG5lZWRzIHRvIGJlXHJcbiAgICAgICAgbGV0IHogPSAxMDAwMDAxMDsgLy8gTmVlZHMgYXQgbGVhc3QgMTAwMDAwMDAgdG8gYmUgb24gdG9wIG9mIHRoZSBETk45IGJhclxyXG4gICAgICAgIGxldCBwID0gd2luZG93O1xyXG4gICAgICAgIHdoaWxlIChwICE9PSB3aW5kb3cudG9wICYmIHogPCAxMDAwMDEwMCkge1xyXG4gICAgICAgICAgICB6Kys7XHJcbiAgICAgICAgICAgIHAgPSBwLnBhcmVudDtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGNvbnN0IHdyYXBwZXIgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdkaXYnKTtcclxuICAgICAgICB3cmFwcGVyLnNldEF0dHJpYnV0ZSgnc3R5bGUnLCAnIHRvcDogMDtsZWZ0OiAwO3dpZHRoOiAxMDAlO2hlaWdodDogMTAwJTsgcG9zaXRpb246Zml4ZWQ7IHotaW5kZXg6JyArIHopO1xyXG4gICAgICAgIGRvY3VtZW50LmJvZHkuYXBwZW5kQ2hpbGQod3JhcHBlcik7XHJcblxyXG4gICAgICAgIGNvbnN0IGlmcm0gPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdpZnJhbWUnKTtcclxuICAgICAgICBpZnJtLnNldEF0dHJpYnV0ZSgnYWxsb3d0cmFuc3BhcmVuY3knLCAndHJ1ZScpO1xyXG4gICAgICAgIGlmcm0uc2V0QXR0cmlidXRlKCdzdHlsZScsICd0b3A6IDA7bGVmdDogMDt3aWR0aDogMTAwJTtoZWlnaHQ6IDEwMCU7Jyk7XHJcbiAgICAgICAgaWZybS5zZXRBdHRyaWJ1dGUoJ3NyYycsIHVybCk7XHJcbiAgICAgICAgd3JhcHBlci5hcHBlbmRDaGlsZChpZnJtKTtcclxuICAgICAgICBkb2N1bWVudC5ib2R5LmNsYXNzTmFtZSArPSAnIHN4Yy1wb3B1cC1vcGVuJztcclxuICAgICAgICB0aGlzLmZyYW1lID0gaWZybTtcclxuICAgICAgICB0aGlzLmNhbGxiYWNrID0gY2FsbGJhY2s7XHJcbiAgICB9XHJcblxyXG4gICAgY2xvc2UoKTogdm9pZCB7XHJcbiAgICAgICAgaWYgKHRoaXMuZnJhbWUpIHtcclxuICAgICAgICAgICAgZG9jdW1lbnQuYm9keS5jbGFzc05hbWUgPSBkb2N1bWVudC5ib2R5LmNsYXNzTmFtZS5yZXBsYWNlKCdzeGMtcG9wdXAtb3BlbicsICcnKTtcclxuICAgICAgICAgICAgY29uc3QgZnJtID0gdGhpcy5mcmFtZTtcclxuICAgICAgICAgICAgZnJtLnBhcmVudE5vZGUucGFyZW50Tm9kZS5yZW1vdmVDaGlsZChmcm0ucGFyZW50Tm9kZSk7XHJcbiAgICAgICAgICAgIHRoaXMuY2FsbGJhY2soKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgY2xvc2VUaGlzKCk6IHZvaWQge1xyXG4gICAgICAgICh3aW5kb3cucGFyZW50IGFzIGFueSkuJDJzeGMudG90YWxQb3B1cC5jbG9zZSgpO1xyXG4gICAgfVxyXG5cclxufVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi8yc3hjLWFwaS9qcy9Ub1NpYy5TeGMuVG90YWxQb3B1cC50cyIsIlxyXG4gICAgZXhwb3J0IGNsYXNzIFVybFBhcmFtTWFuYWdlciB7XHJcbiAgICAgICAgZ2V0KG5hbWU6IHN0cmluZykge1xyXG4gICAgICAgICAgICAvLyB3YXJuaW5nOiB0aGlzIG1ldGhvZCBpcyBkdXBsaWNhdGVkIGluIDIgcGxhY2VzIC0ga2VlcCB0aGVtIGluIHN5bmMuXHJcbiAgICAgICAgICAgIC8vIGxvY2F0aW9ucyBhcmUgZWF2IGFuZCAyc3hjNG5nXHJcbiAgICAgICAgICAgIG5hbWUgPSBuYW1lLnJlcGxhY2UoL1tcXFtdLywgJ1xcXFxbJykucmVwbGFjZSgvW1xcXV0vLCAnXFxcXF0nKTtcclxuICAgICAgICAgICAgY29uc3Qgc2VhcmNoUnggPSBuZXcgUmVnRXhwKCdbXFxcXD8mXScgKyBuYW1lICsgJz0oW14mI10qKScsICdpJyk7XHJcbiAgICAgICAgICAgIGxldCByZXN1bHRzID0gc2VhcmNoUnguZXhlYyhsb2NhdGlvbi5zZWFyY2gpO1xyXG4gICAgICAgICAgICBsZXQgc3RyUmVzdWx0OiBzdHJpbmc7XHJcblxyXG4gICAgICAgICAgICBpZiAocmVzdWx0cyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgY29uc3QgaGFzaFJ4ID0gbmV3IFJlZ0V4cCgnWyMmXScgKyBuYW1lICsgJz0oW14mI10qKScsICdpJyk7XHJcbiAgICAgICAgICAgICAgICByZXN1bHRzID0gaGFzaFJ4LmV4ZWMobG9jYXRpb24uaGFzaCk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIC8vIGlmIG5vdGhpbmcgZm91bmQsIHRyeSBub3JtYWwgVVJMIGJlY2F1c2UgRE5OIHBsYWNlcyBwYXJhbWV0ZXJzIGluIC9rZXkvdmFsdWUgbm90YXRpb25cclxuICAgICAgICAgICAgaWYgKHJlc3VsdHMgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIC8vIE90aGVyd2lzZSB0cnkgcGFydHMgb2YgdGhlIFVSTFxyXG4gICAgICAgICAgICAgICAgY29uc3QgbWF0Y2hlcyA9IHdpbmRvdy5sb2NhdGlvbi5wYXRobmFtZS5tYXRjaChuZXcgUmVnRXhwKCcvJyArIG5hbWUgKyAnLyhbXi9dKyknLCAnaScpKTtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBDaGVjayBpZiB3ZSBmb3VuZCBhbnl0aGluZywgaWYgd2UgZG8gZmluZCBpdCwgd2UgbXVzdCByZXZlcnNlIHRoZVxyXG4gICAgICAgICAgICAgICAgLy8gcmVzdWx0cyBzbyB3ZSBnZXQgdGhlIFwibGFzdFwiIG9uZSBpbiBjYXNlIHRoZXJlIGFyZSBtdWx0aXBsZSBoaXRzXHJcbiAgICAgICAgICAgICAgICBpZiAobWF0Y2hlcyAmJiBtYXRjaGVzLmxlbmd0aCA+IDEpXHJcbiAgICAgICAgICAgICAgICAgICAgc3RyUmVzdWx0ID0gbWF0Y2hlcy5yZXZlcnNlKClbMF07XHJcbiAgICAgICAgICAgIH0gZWxzZVxyXG4gICAgICAgICAgICAgICAgc3RyUmVzdWx0ID0gcmVzdWx0c1sxXTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBzdHJSZXN1bHQgPT09IG51bGwgfHwgc3RyUmVzdWx0ID09PSB1bmRlZmluZWRcclxuICAgICAgICAgICAgICAgID8gJydcclxuICAgICAgICAgICAgICAgIDogZGVjb2RlVVJJQ29tcG9uZW50KHN0clJlc3VsdC5yZXBsYWNlKC9cXCsvZywgJyAnKSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXF1aXJlKG5hbWU6IHN0cmluZykge1xyXG4gICAgICAgICAgICBjb25zdCBmb3VuZCA9IHRoaXMuZ2V0KG5hbWUpO1xyXG4gICAgICAgICAgICBpZiAoZm91bmQgPT09ICcnKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBtZXNzYWdlID0gYFJlcXVpcmVkIHBhcmFtZXRlciAoJHtuYW1lfSkgbWlzc2luZyBmcm9tIHVybCAtIGNhbm5vdCBjb250aW51ZWA7XHJcbiAgICAgICAgICAgICAgICBhbGVydChtZXNzYWdlKTtcclxuICAgICAgICAgICAgICAgIHRocm93IG1lc3NhZ2U7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcmV0dXJuIGZvdW5kO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvVG9TaWMuU3hjLlVybC50cyIsImV4cG9ydCBjbGFzcyBTdGF0cyB7XHJcbiAgICB3YXRjaERvbUNoYW5nZXMgPSAwO1xyXG59XG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vMnN4Yy1hcGkvanMvU3RhdHMudHMiXSwic291cmNlUm9vdCI6IiJ9