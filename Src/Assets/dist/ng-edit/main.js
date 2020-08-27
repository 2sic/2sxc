(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app.component.html":
/*!************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app.component.html ***!
  \************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<router-outlet></router-outlet>\r\n");

/***/ }),

/***/ "../../node_modules/tslib/tslib.es6.js":
/*!***************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/tslib/tslib.es6.js ***!
  \***************************************************************************/
/*! exports provided: __extends, __assign, __rest, __decorate, __param, __metadata, __awaiter, __generator, __exportStar, __values, __read, __spread, __spreadArrays, __await, __asyncGenerator, __asyncDelegator, __asyncValues, __makeTemplateObject, __importStar, __importDefault, __classPrivateFieldGet, __classPrivateFieldSet */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__extends", function() { return __extends; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__assign", function() { return __assign; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__rest", function() { return __rest; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__decorate", function() { return __decorate; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__param", function() { return __param; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__metadata", function() { return __metadata; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__awaiter", function() { return __awaiter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__generator", function() { return __generator; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__exportStar", function() { return __exportStar; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__values", function() { return __values; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__read", function() { return __read; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__spread", function() { return __spread; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__spreadArrays", function() { return __spreadArrays; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__await", function() { return __await; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__asyncGenerator", function() { return __asyncGenerator; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__asyncDelegator", function() { return __asyncDelegator; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__asyncValues", function() { return __asyncValues; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__makeTemplateObject", function() { return __makeTemplateObject; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__importStar", function() { return __importStar; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__importDefault", function() { return __importDefault; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__classPrivateFieldGet", function() { return __classPrivateFieldGet; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "__classPrivateFieldSet", function() { return __classPrivateFieldSet; });
/*! *****************************************************************************
Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the Apache License, Version 2.0 (the "License"); you may not use
this file except in compliance with the License. You may obtain a copy of the
License at http://www.apache.org/licenses/LICENSE-2.0

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
MERCHANTABLITY OR NON-INFRINGEMENT.

See the Apache Version 2.0 License for specific language governing permissions
and limitations under the License.
***************************************************************************** */
/* global Reflect, Promise */

var extendStatics = function(d, b) {
    extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return extendStatics(d, b);
};

function __extends(d, b) {
    extendStatics(d, b);
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
}

var __assign = function() {
    __assign = Object.assign || function __assign(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p)) t[p] = s[p];
        }
        return t;
    }
    return __assign.apply(this, arguments);
}

function __rest(s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) {
            if (e.indexOf(p[i]) < 0 && Object.prototype.propertyIsEnumerable.call(s, p[i]))
                t[p[i]] = s[p[i]];
        }
    return t;
}

function __decorate(decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
}

function __param(paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
}

function __metadata(metadataKey, metadataValue) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(metadataKey, metadataValue);
}

function __awaiter(thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
}

function __generator(thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
}

function __exportStar(m, exports) {
    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
}

function __values(o) {
    var s = typeof Symbol === "function" && Symbol.iterator, m = s && o[s], i = 0;
    if (m) return m.call(o);
    if (o && typeof o.length === "number") return {
        next: function () {
            if (o && i >= o.length) o = void 0;
            return { value: o && o[i++], done: !o };
        }
    };
    throw new TypeError(s ? "Object is not iterable." : "Symbol.iterator is not defined.");
}

function __read(o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
}

function __spread() {
    for (var ar = [], i = 0; i < arguments.length; i++)
        ar = ar.concat(__read(arguments[i]));
    return ar;
}

function __spreadArrays() {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};

function __await(v) {
    return this instanceof __await ? (this.v = v, this) : new __await(v);
}

function __asyncGenerator(thisArg, _arguments, generator) {
    if (!Symbol.asyncIterator) throw new TypeError("Symbol.asyncIterator is not defined.");
    var g = generator.apply(thisArg, _arguments || []), i, q = [];
    return i = {}, verb("next"), verb("throw"), verb("return"), i[Symbol.asyncIterator] = function () { return this; }, i;
    function verb(n) { if (g[n]) i[n] = function (v) { return new Promise(function (a, b) { q.push([n, v, a, b]) > 1 || resume(n, v); }); }; }
    function resume(n, v) { try { step(g[n](v)); } catch (e) { settle(q[0][3], e); } }
    function step(r) { r.value instanceof __await ? Promise.resolve(r.value.v).then(fulfill, reject) : settle(q[0][2], r); }
    function fulfill(value) { resume("next", value); }
    function reject(value) { resume("throw", value); }
    function settle(f, v) { if (f(v), q.shift(), q.length) resume(q[0][0], q[0][1]); }
}

function __asyncDelegator(o) {
    var i, p;
    return i = {}, verb("next"), verb("throw", function (e) { throw e; }), verb("return"), i[Symbol.iterator] = function () { return this; }, i;
    function verb(n, f) { i[n] = o[n] ? function (v) { return (p = !p) ? { value: __await(o[n](v)), done: n === "return" } : f ? f(v) : v; } : f; }
}

function __asyncValues(o) {
    if (!Symbol.asyncIterator) throw new TypeError("Symbol.asyncIterator is not defined.");
    var m = o[Symbol.asyncIterator], i;
    return m ? m.call(o) : (o = typeof __values === "function" ? __values(o) : o[Symbol.iterator](), i = {}, verb("next"), verb("throw"), verb("return"), i[Symbol.asyncIterator] = function () { return this; }, i);
    function verb(n) { i[n] = o[n] && function (v) { return new Promise(function (resolve, reject) { v = o[n](v), settle(resolve, reject, v.done, v.value); }); }; }
    function settle(resolve, reject, d, v) { Promise.resolve(v).then(function(v) { resolve({ value: v, done: d }); }, reject); }
}

function __makeTemplateObject(cooked, raw) {
    if (Object.defineProperty) { Object.defineProperty(cooked, "raw", { value: raw }); } else { cooked.raw = raw; }
    return cooked;
};

function __importStar(mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result.default = mod;
    return result;
}

function __importDefault(mod) {
    return (mod && mod.__esModule) ? mod : { default: mod };
}

function __classPrivateFieldGet(receiver, privateMap) {
    if (!privateMap.has(receiver)) {
        throw new TypeError("attempted to get private field on non-instance");
    }
    return privateMap.get(receiver);
}

function __classPrivateFieldSet(receiver, privateMap, value) {
    if (!privateMap.has(receiver)) {
        throw new TypeError("attempted to set private field on non-instance");
    }
    privateMap.set(receiver, value);
    return value;
}


/***/ }),

/***/ "../edit/edit.matcher.ts":
/*!*******************************!*\
  !*** ../edit/edit.matcher.ts ***!
  \*******************************/
/*! exports provided: editRoot, edit */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "editRoot", function() { return editRoot; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "edit", function() { return edit; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

/**
 * ':zoneId/:appId/edit/:items'
 * ':zoneId/:appId/edit/:items/details/:detailsEntityGuid/:detailsFieldId'
 * ':zoneId/:appId/edit/:items/update/:updateEntityGuid/:updateFieldId'
 */
function editRoot(url) {
    if (url.length < 4) {
        return null;
    }
    if (url[2].path !== 'edit') {
        return null;
    }
    var hasDetails = url[4] != null && url[4].path === 'details' && url[5] != null && url[6] != null;
    var hasUpdate = url[4] != null && url[4].path === 'update' && url[5] != null && url[6] != null;
    var posParams = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({ zoneId: url[0], appId: url[1], items: url[3] }, (hasDetails && { detailsEntityGuid: url[5], detailsFieldId: url[6] })), (hasUpdate && { updateEntityGuid: url[5], updateFieldId: url[6] }));
    var match = {
        consumed: url.slice(0, (hasDetails || hasUpdate) ? 7 : 4),
        posParams: posParams
    };
    return match;
}
/**
 * 'edit/:items'
 * 'edit/:items/details/:detailsEntityGuid/:detailsFieldId'
 * 'edit/:items/update/:updateEntityGuid/:updateFieldId'
 */
function edit(url) {
    if (url.length < 2) {
        return null;
    }
    if (url[0].path !== 'edit') {
        return null;
    }
    var hasDetails = url[2] != null && url[2].path === 'details' && url[3] != null && url[4] != null;
    var hasUpdate = url[2] != null && url[2].path === 'update' && url[3] != null && url[4] != null;
    var posParams = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({ items: url[1] }, (hasDetails && { detailsEntityGuid: url[3], detailsFieldId: url[4] })), (hasUpdate && { updateEntityGuid: url[3], updateFieldId: url[4] }));
    var match = {
        consumed: url.slice(0, (hasDetails || hasUpdate) ? 5 : 2),
        posParams: posParams
    };
    return match;
}


/***/ }),

/***/ "../edit/shared/helpers/url-helper.ts":
/*!********************************************!*\
  !*** ../edit/shared/helpers/url-helper.ts ***!
  \********************************************/
/*! exports provided: UrlHelper */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UrlHelper", function() { return UrlHelper; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _models_eav_configuration__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../models/eav-configuration */ "../edit/shared/models/eav-configuration.ts");
/* harmony import */ var _ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../ng-dialogs/src/app/shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");
/* harmony import */ var _ng_dialogs_src_app_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../ng-dialogs/src/app/shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");




var UrlHelper = /** @class */ (function () {
    function UrlHelper() {
    }
    UrlHelper.readQueryStringParameters = function (url) {
        var queryParams = {};
        url.split('&').forEach(function (f) {
            if (f.split('=').length === 2) {
                queryParams[f.split('=')[0]] = decodeURIComponent(f.split('=')[1].replace(/\+/g, ' '));
            }
        });
        return queryParams;
    };
    /** Create EavConfiguration from sessionStorage */
    UrlHelper.getEavConfiguration = function (route, context) {
        var form = Object(_ng_dialogs_src_app_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_3__["convertUrlToForm"])(route.snapshot.params.items);
        var editItems = JSON.stringify(form.items);
        return new _models_eav_configuration__WEBPACK_IMPORTED_MODULE_1__["EavConfiguration"](context.zoneId.toString(), context.appId.toString(), context.appRoot, context.contentBlockId.toString(), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyDebug"]), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyDialog"]), editItems, sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyLang"]), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyLangPri"]), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyLangs"]), context.moduleId.toString(), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyMode"]), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyPartOfPage"]), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyPortalRoot"]), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyPublishing"]), context.tabId.toString(), context.requestToken.toString(), sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyWebsiteRoot"]), UrlHelper.getVersioningOptions(sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyPartOfPage"]) === 'true', sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyPublishing"])));
    };
    UrlHelper.getVersioningOptions = function (partOfPage, publishing) {
        if (!partOfPage) {
            return { show: true, hide: true, branch: true };
        }
        var req = publishing || '';
        switch (req) {
            case '':
            case 'DraftOptional': return { show: true, hide: true, branch: true };
            case 'DraftRequired': return { branch: true, hide: true };
            default: {
                console.error('invalid versioning requiremenets: ' + req.toString());
                return {};
            }
        }
    };
    /** https://stackoverflow.com/questions/979975/how-to-get-the-value-from-the-get-parameters/1099670#1099670 */
    UrlHelper.getUrlParams = function (qs) {
        qs = qs.split('+').join(' ');
        var params = {};
        var tokens;
        var re = /[?&]?([^=]+)=([^&]*)/g;
        // tslint:disable-next-line:no-conditional-assignment
        while (tokens = re.exec(qs)) {
            params[decodeURIComponent(tokens[1])] = decodeURIComponent(tokens[2]);
        }
        return params;
    };
    UrlHelper.replaceUrlParam = function (url, paramName, paramValue) {
        if (paramValue === null) {
            paramValue = '';
        }
        var pattern = new RegExp('\\b(' + paramName + '=).*?(&|#|$)');
        if (url.search(pattern) >= 0) {
            return url.replace(pattern, '$1' + paramValue + '$2');
        }
        url = url.replace(/[?#]$/, '');
        return url + (url.indexOf('?') > 0 ? '&' : '?') + paramName + '=' + paramValue;
    };
    UrlHelper.getUrlPrefix = function (area, eavConfig) {
        var result = '';
        if (area === 'system') {
            result = eavConfig.systemroot;
        } // used to link to JS-stuff and similar
        if (area === 'zone') {
            result = eavConfig.portalroot;
        } // used to link to the site-root (like an image)
        if (area === 'app') {
            result = eavConfig.approot;
        } // used to find the app-root of something inside an app
        if (result.endsWith('/')) {
            result = result.substring(0, result.length - 1);
        }
        return result;
    };
    return UrlHelper;
}());



/***/ }),

/***/ "../edit/shared/models/eav-configuration.ts":
/*!**************************************************!*\
  !*** ../edit/shared/models/eav-configuration.ts ***!
  \**************************************************/
/*! exports provided: EavConfiguration */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavConfiguration", function() { return EavConfiguration; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var EavConfiguration = /** @class */ (function () {
    function EavConfiguration(zoneId, appId, approot, cbid, debug, dialog, items, lang, langpri, langs, mid, mode, partOfPage, portalroot, publishing, tid, rvt, websiteroot, versioningOptions) {
        this.zoneId = zoneId;
        this.appId = appId;
        this.approot = approot;
        this.cbid = cbid;
        this.debug = debug;
        this.dialog = dialog;
        this.items = items;
        this.lang = lang;
        this.langpri = langpri;
        this.langs = langs;
        this.mid = mid;
        this.mode = mode;
        this.partOfPage = partOfPage;
        this.portalroot = portalroot;
        this.publishing = publishing;
        this.tid = tid;
        this.rvt = rvt;
        this.websiteroot = websiteroot;
        this.versioningOptions = versioningOptions;
        this.systemroot = websiteroot + 'desktopmodules/tosic_sexycontent/';
    }
    return EavConfiguration;
}());



/***/ }),

/***/ "../edit/shared/store/actions/global-configuration.actions.ts":
/*!********************************************************************!*\
  !*** ../edit/shared/store/actions/global-configuration.actions.ts ***!
  \********************************************************************/
/*! exports provided: loadDebugEnabled, toggleDebugEnabled */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "loadDebugEnabled", function() { return loadDebugEnabled; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "toggleDebugEnabled", function() { return toggleDebugEnabled; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngrx/store */ "../../node_modules/@ngrx/store/__ivy_ngcc__/fesm5/store.js");


var loadDebugEnabled = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["createAction"])('[GlobalConfiguration] LOAD_DEBUG_ENABLED', Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["props"])());
var toggleDebugEnabled = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["createAction"])('[GlobalConfiguration] TOGGLE_DEBUG_ENABLED');


/***/ }),

/***/ "../edit/shared/store/index.ts":
/*!*************************************!*\
  !*** ../edit/shared/store/index.ts ***!
  \*************************************/
/*! exports provided: logger, metaReducers, reducers, selectGlobalConfiguration, selectDebugEnabled */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _reducers__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./reducers */ "../edit/shared/store/reducers/index.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "logger", function() { return _reducers__WEBPACK_IMPORTED_MODULE_1__["logger"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "metaReducers", function() { return _reducers__WEBPACK_IMPORTED_MODULE_1__["metaReducers"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "reducers", function() { return _reducers__WEBPACK_IMPORTED_MODULE_1__["reducers"]; });

/* harmony import */ var _selectors__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./selectors */ "../edit/shared/store/selectors/index.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "selectGlobalConfiguration", function() { return _selectors__WEBPACK_IMPORTED_MODULE_2__["selectGlobalConfiguration"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "selectDebugEnabled", function() { return _selectors__WEBPACK_IMPORTED_MODULE_2__["selectDebugEnabled"]; });






/***/ }),

/***/ "../edit/shared/store/ngrx-data/entity-metadata.ts":
/*!*********************************************************!*\
  !*** ../edit/shared/store/ngrx-data/entity-metadata.ts ***!
  \*********************************************************/
/*! exports provided: entityMetadata, pluralNames, entityConfig, itemSelectId, languageSelectId, languageInstanceSelectId, contentTypeSelectId, InputTypeSelectId */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "entityMetadata", function() { return entityMetadata; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "pluralNames", function() { return pluralNames; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "entityConfig", function() { return entityConfig; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "itemSelectId", function() { return itemSelectId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "languageSelectId", function() { return languageSelectId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "languageInstanceSelectId", function() { return languageInstanceSelectId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentTypeSelectId", function() { return contentTypeSelectId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InputTypeSelectId", function() { return InputTypeSelectId; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var entityMetadata = {
    Item: {
        selectId: itemSelectId,
    },
    Feature: {},
    Language: {
        selectId: languageSelectId,
    },
    LanguageInstance: {
        selectId: languageInstanceSelectId,
    },
    ContentType: {
        selectId: contentTypeSelectId,
    },
    InputType: {
        selectId: InputTypeSelectId,
    },
};
var pluralNames = {
    Feature: 'Features',
};
var entityConfig = {
    entityMetadata: entityMetadata,
    pluralNames: pluralNames,
};
function itemSelectId(entity) {
    return entity === null ? undefined : entity.entity.guid;
}
function languageSelectId(entity) {
    return entity === null ? undefined : entity.key;
}
function languageInstanceSelectId(entity) {
    return entity === null ? undefined : entity.formId;
}
function contentTypeSelectId(entity) {
    return entity === null ? undefined : entity.contentType.id;
}
function InputTypeSelectId(entity) {
    return entity === null ? undefined : entity.Type;
}


/***/ }),

/***/ "../edit/shared/store/reducers/global-configuration.reducer.ts":
/*!*********************************************************************!*\
  !*** ../edit/shared/store/reducers/global-configuration.reducer.ts ***!
  \*********************************************************************/
/*! exports provided: initialState, reducer */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "initialState", function() { return initialState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "reducer", function() { return reducer; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngrx/store */ "../../node_modules/@ngrx/store/__ivy_ngcc__/fesm5/store.js");
/* harmony import */ var _ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../ng-dialogs/src/app/shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");
/* harmony import */ var _actions_global_configuration_actions__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../actions/global-configuration.actions */ "../edit/shared/store/actions/global-configuration.actions.ts");




var initialState = {
    debugEnabled: sessionStorage.getItem(_ng_dialogs_src_app_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyDebug"]) === 'true',
};
var globalConfigurationReducer = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["createReducer"])(initialState, Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["on"])(_actions_global_configuration_actions__WEBPACK_IMPORTED_MODULE_3__["loadDebugEnabled"], function (state, _a) {
    var debugEnabled = _a.debugEnabled;
    return (Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, state), { debugEnabled: debugEnabled }));
}), Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["on"])(_actions_global_configuration_actions__WEBPACK_IMPORTED_MODULE_3__["toggleDebugEnabled"], function (state) { return (Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, state), { debugEnabled: !state.debugEnabled })); }));
function reducer(state, action) {
    return globalConfigurationReducer(state, action);
}


/***/ }),

/***/ "../edit/shared/store/reducers/index.ts":
/*!**********************************************!*\
  !*** ../edit/shared/store/reducers/index.ts ***!
  \**********************************************/
/*! exports provided: logger, metaReducers, reducers */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "logger", function() { return logger; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "metaReducers", function() { return metaReducers; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "reducers", function() { return reducers; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _ng_dialogs_src_environments_environment__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../ng-dialogs/src/environments/environment */ "./src/environments/environment.ts");
/* harmony import */ var _global_configuration_reducer__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./global-configuration.reducer */ "../edit/shared/store/reducers/global-configuration.reducer.ts");
/* harmony import */ var _ng_dialogs_src_app_shared_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../../ng-dialogs/src/app/shared/helpers/angular-console-log.helper */ "./src/app/shared/helpers/angular-console-log.helper.ts");




/** Console log all actions */
function logger(reducer) {
    return function (state, action) {
        Object(_ng_dialogs_src_app_shared_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_3__["angularConsoleLog"])('[STORE] state', state);
        Object(_ng_dialogs_src_app_shared_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_3__["angularConsoleLog"])('[STORE] action', action);
        return reducer(state, action);
    };
}
/**
 * By default, @ngrx/store uses combineReducers with the reducer map to compose
 * the root meta-reducer. To add more meta-reducers, provide an array of meta-reducers
 * that will be composed to form the root meta-reducer.
 */
var metaReducers = !_ng_dialogs_src_environments_environment__WEBPACK_IMPORTED_MODULE_1__["environment"].production
    ? [logger]
    : [];
var reducers = {
    globalConfiguration: _global_configuration_reducer__WEBPACK_IMPORTED_MODULE_2__["reducer"],
};


/***/ }),

/***/ "../edit/shared/store/selectors/global-configuration.selectors.ts":
/*!************************************************************************!*\
  !*** ../edit/shared/store/selectors/global-configuration.selectors.ts ***!
  \************************************************************************/
/*! exports provided: selectGlobalConfiguration, selectDebugEnabled */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "selectGlobalConfiguration", function() { return selectGlobalConfiguration; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "selectDebugEnabled", function() { return selectDebugEnabled; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngrx/store */ "../../node_modules/@ngrx/store/__ivy_ngcc__/fesm5/store.js");


var selectGlobalConfiguration = function (state) { return state.globalConfiguration; };
var selectDebugEnabled = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["createSelector"])(selectGlobalConfiguration, function (state) { return state.debugEnabled; });


/***/ }),

/***/ "../edit/shared/store/selectors/index.ts":
/*!***********************************************!*\
  !*** ../edit/shared/store/selectors/index.ts ***!
  \***********************************************/
/*! exports provided: selectGlobalConfiguration, selectDebugEnabled */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _global_configuration_selectors__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./global-configuration.selectors */ "../edit/shared/store/selectors/global-configuration.selectors.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "selectGlobalConfiguration", function() { return _global_configuration_selectors__WEBPACK_IMPORTED_MODULE_1__["selectGlobalConfiguration"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "selectDebugEnabled", function() { return _global_configuration_selectors__WEBPACK_IMPORTED_MODULE_1__["selectDebugEnabled"]; });





/***/ }),

/***/ "./$$_lazy_route_resource lazy recursive":
/*!******************************************************!*\
  !*** ./$$_lazy_route_resource lazy namespace object ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncaught exception popping up in devtools
	return Promise.resolve().then(function() {
		var e = new Error("Cannot find module '" + req + "'");
		e.code = 'MODULE_NOT_FOUND';
		throw e;
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "./$$_lazy_route_resource lazy recursive";

/***/ }),

/***/ "./src/app/app-routing.module.ts":
/*!***************************************!*\
  !*** ./src/app/app-routing.module.ts ***!
  \***************************************/
/*! exports provided: AppRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppRoutingModule", function() { return AppRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../edit/edit.matcher */ "../edit/edit.matcher.ts");




var appRoutes = [
    {
        path: ':zoneId/apps',
        loadChildren: function () { return Promise.all(/*! import() | apps-management-apps-management-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~33e631e1"), __webpack_require__.e("common"), __webpack_require__.e("apps-management-apps-management-module")]).then(__webpack_require__.bind(null, /*! ./apps-management/apps-management.module */ "./src/app/apps-management/apps-management.module.ts")).then(function (m) { return m.AppsManagementModule; }); },
        data: { title: 'Apps' },
    },
    {
        path: ':zoneId/import',
        loadChildren: function () { return Promise.all(/*! import() | import-app-import-app-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("import-app-import-app-module")]).then(__webpack_require__.bind(null, /*! ./import-app/import-app.module */ "./src/app/import-app/import-app.module.ts")).then(function (m) { return m.ImportAppModule; }); },
        data: { title: 'Import App' },
    },
    {
        path: ':zoneId/:appId/app',
        loadChildren: function () { return Promise.all(/*! import() | app-administration-app-administration-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~33e631e1"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("app-administration-app-administration-module")]).then(__webpack_require__.bind(null, /*! ./app-administration/app-administration.module */ "./src/app/app-administration/app-administration.module.ts")).then(function (m) { return m.AppAdministrationModule; }); },
        data: { title: 'App' },
    },
    {
        path: ':zoneId/:appId/code',
        loadChildren: function () { return Promise.all(/*! import() | code-editor-code-editor-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~code-editor-code-editor-module~visual-query-visual-query-module"), __webpack_require__.e("common"), __webpack_require__.e("code-editor-code-editor-module")]).then(__webpack_require__.bind(null, /*! ./code-editor/code-editor.module */ "./src/app/code-editor/code-editor.module.ts")).then(function (m) { return m.CodeEditorModule; }); },
        data: { title: 'Code Editor' },
    },
    {
        path: ':zoneId/:appId/query/:pipelineId',
        loadChildren: function () { return Promise.all(/*! import() | visual-query-visual-query-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~code-editor-code-editor-module~visual-query-visual-query-module"), __webpack_require__.e("common"), __webpack_require__.e("visual-query-visual-query-module")]).then(__webpack_require__.bind(null, /*! ./visual-query/visual-query.module */ "./src/app/visual-query/visual-query.module.ts")).then(function (m) { return m.VisualQueryModule; }); },
        data: { title: 'Visual Query' },
    },
    {
        path: ':zoneId/:appId/:guid/:part/:index/replace',
        loadChildren: function () { return Promise.all(/*! import() | replace-content-replace-content-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("common"), __webpack_require__.e("replace-content-replace-content-module")]).then(__webpack_require__.bind(null, /*! ./replace-content/replace-content.module */ "./src/app/replace-content/replace-content.module.ts")).then(function (m) { return m.ReplaceContentModule; }); },
        data: { title: 'Apps' },
    },
    {
        path: ':zoneId/:appId/:guid/:part/:index/reorder',
        loadChildren: function () { return Promise.all(/*! import() | manage-content-list-manage-content-list-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("common"), __webpack_require__.e("manage-content-list-manage-content-list-module")]).then(__webpack_require__.bind(null, /*! ./manage-content-list/manage-content-list.module */ "./src/app/manage-content-list/manage-content-list.module.ts")).then(function (m) { return m.ManageContentListModule; }); },
        data: { title: 'Reorder Items' },
    },
    {
        path: ':zoneId/:appId/items/:contentTypeStaticName',
        loadChildren: function () { return Promise.all(/*! import() | content-items-content-items-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~33e631e1"), __webpack_require__.e("common"), __webpack_require__.e("content-items-content-items-module")]).then(__webpack_require__.bind(null, /*! ./content-items/content-items.module */ "./src/app/content-items/content-items.module.ts")).then(function (m) { return m.ContentItemsModule; }); },
        data: { title: 'Items' },
    },
    {
        path: ':zoneId/:appId/fields/:contentTypeStaticName',
        loadChildren: function () { return Promise.all(/*! import() | content-type-fields-content-type-fields-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("content-type-fields-content-type-fields-module")]).then(__webpack_require__.bind(null, /*! ./content-type-fields/content-type-fields.module */ "./src/app/content-type-fields/content-type-fields.module.ts")).then(function (m) { return m.ContentTypeFieldsModule; }); },
        data: { title: 'Fields' },
    },
    {
        matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_3__["editRoot"],
        loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); },
        data: { title: 'Edit Item' },
    },
    // routes below are not linked directly from the initializer and are used for testing
    // to make sure each module contains enough data to be self sustainable
    {
        path: ':zoneId/:appId/export/:contentTypeStaticName',
        loadChildren: function () { return Promise.all(/*! import() | content-export-content-export-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("common"), __webpack_require__.e("content-export-content-export-module")]).then(__webpack_require__.bind(null, /*! ./content-export/content-export.module */ "./src/app/content-export/content-export.module.ts")).then(function (m) { return m.ContentExportModule; }); },
        data: { title: 'Export Items' },
    },
    {
        path: ':zoneId/:appId/export/:contentTypeStaticName/:selectedIds',
        loadChildren: function () { return Promise.all(/*! import() | content-export-content-export-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("common"), __webpack_require__.e("content-export-content-export-module")]).then(__webpack_require__.bind(null, /*! ./content-export/content-export.module */ "./src/app/content-export/content-export.module.ts")).then(function (m) { return m.ContentExportModule; }); },
        data: { title: 'Export Items' },
    },
    {
        path: ':zoneId/:appId/permissions/:type/:keyType/:key',
        loadChildren: function () { return Promise.all(/*! import() | permissions-permissions-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("permissions-permissions-module")]).then(__webpack_require__.bind(null, /*! ./permissions/permissions.module */ "./src/app/permissions/permissions.module.ts")).then(function (m) { return m.PermissionsModule; }); },
        data: { title: 'Permissions' },
    },
];
var AppRoutingModule = /** @class */ (function () {
    function AppRoutingModule() {
    }
    AppRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forRoot(appRoutes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], AppRoutingModule);
    return AppRoutingModule;
}());



/***/ }),

/***/ "./src/app/app.component.scss":
/*!************************************!*\
  !*** ./src/app/app.component.scss ***!
  \************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/app.component.ts":
/*!**********************************!*\
  !*** ./src/app/app.component.ts ***!
  \**********************************/
/*! exports provided: AppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppComponent", function() { return AppComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/platform-browser.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");










var AppComponent = /** @class */ (function (_super) {
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__extends"])(AppComponent, _super);
    function AppComponent(el, dnnContext, context, titleService, router, activatedRoute, matIconRegistry) {
        var _this = _super.call(this, el, dnnContext.preConfigure({
            moduleId: parseInt(sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_9__["keyModuleId"]), 10),
            contentBlockId: parseInt(sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_9__["keyContentBlockId"]), 10),
        })) || this;
        _this.context = context;
        _this.titleService = titleService;
        _this.router = router;
        _this.activatedRoute = activatedRoute;
        _this.matIconRegistry = matIconRegistry;
        _this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_5__["Subscription"]();
        _this.context.initRoot();
        _this.matIconRegistry.setDefaultFontSetClass('material-icons-outlined');
        return _this;
    }
    AppComponent.prototype.ngOnInit = function () {
        var _this = this;
        // Mostly copied from https://blog.bitsrc.io/dynamic-page-titles-in-angular-98ce20b5c334
        // Routes need a data: { title: '...' } for this to work
        var appTitle = this.titleService.getTitle(); // initial title when loading the page
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_6__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_6__["map"])(function () {
            var child = _this.activatedRoute.firstChild;
            while (child.firstChild) {
                child = child.firstChild;
            }
            if (child.snapshot.data['title']) {
                return child.snapshot.data['title'];
            }
            return appTitle;
        })).subscribe(function (title) {
            _this.titleService.setTitle(title);
        }));
    };
    AppComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    AppComponent.ctorParameters = function () { return [
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_7__["Context"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_8__["Context"] },
        { type: _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__["Title"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _angular_material_icon__WEBPACK_IMPORTED_MODULE_4__["MatIconRegistry"] }
    ]; };
    AppComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-root',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./app.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./app.component.scss */ "./src/app/app.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"],
            _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_7__["Context"],
            _shared_services_context__WEBPACK_IMPORTED_MODULE_8__["Context"],
            _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__["Title"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _angular_material_icon__WEBPACK_IMPORTED_MODULE_4__["MatIconRegistry"]])
    ], AppComponent);
    return AppComponent;
}(_2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_7__["DnnAppComponent"]));



/***/ }),

/***/ "./src/app/app.module.ts":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/platform-browser.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/platform-browser/animations */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/animations.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _ngrx_data__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @ngrx/data */ "../../node_modules/@ngrx/data/__ivy_ngcc__/fesm5/data.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @ngrx/store */ "../../node_modules/@ngrx/store/__ivy_ngcc__/fesm5/store.js");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @ngrx/effects */ "../../node_modules/@ngrx/effects/__ivy_ngcc__/fesm5/effects.js");
/* harmony import */ var _ngrx_store_devtools__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @ngrx/store-devtools */ "../../node_modules/@ngrx/store-devtools/__ivy_ngcc__/fesm5/store-devtools.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @ngx-translate/core */ "../../node_modules/@ngx-translate/core/__ivy_ngcc__/fesm5/ngx-translate-core.js");
/* harmony import */ var _app_routing_module__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./app-routing.module */ "./src/app/app-routing.module.ts");
/* harmony import */ var _app_component__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./app.component */ "./src/app/app.component.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _params_init_factory__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./params-init.factory */ "./src/app/params-init.factory.ts");
/* harmony import */ var _edit_shared_store_ngrx_data_entity_metadata__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../../../edit/shared/store/ngrx-data/entity-metadata */ "../edit/shared/store/ngrx-data/entity-metadata.ts");
/* harmony import */ var _edit_shared_store__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ../../../edit/shared/store */ "../edit/shared/store/index.ts");
/* harmony import */ var _shared_interceptors_set_headers_interceptor__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ./shared/interceptors/set-headers.interceptor */ "./src/app/shared/interceptors/set-headers.interceptor.ts");
/* harmony import */ var _shared_interceptors_handle_errors_interceptor__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./shared/interceptors/handle-errors.interceptor */ "./src/app/shared/interceptors/handle-errors.interceptor.ts");





















var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["NgModule"])({
            declarations: [
                _app_component__WEBPACK_IMPORTED_MODULE_14__["AppComponent"],
            ],
            entryComponents: [],
            imports: [
                _app_routing_module__WEBPACK_IMPORTED_MODULE_13__["AppRoutingModule"],
                _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__["BrowserModule"],
                _angular_common_http__WEBPACK_IMPORTED_MODULE_5__["HttpClientModule"],
                _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_4__["BrowserAnimationsModule"],
                _ngrx_store__WEBPACK_IMPORTED_MODULE_8__["StoreModule"].forRoot(_edit_shared_store__WEBPACK_IMPORTED_MODULE_18__["reducers"], { metaReducers: _edit_shared_store__WEBPACK_IMPORTED_MODULE_18__["metaReducers"], runtimeChecks: { strictStateImmutability: true, strictActionImmutability: true } }),
                _ngrx_effects__WEBPACK_IMPORTED_MODULE_9__["EffectsModule"].forRoot([]),
                _ngrx_store_devtools__WEBPACK_IMPORTED_MODULE_10__["StoreDevtoolsModule"].instrument({ maxAge: 25 }),
                _ngrx_data__WEBPACK_IMPORTED_MODULE_7__["EntityDataModule"].forRoot(_edit_shared_store_ngrx_data_entity_metadata__WEBPACK_IMPORTED_MODULE_17__["entityConfig"]),
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_12__["TranslateModule"].forRoot(),
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_6__["MatSnackBarModule"],
            ],
            providers: [
                { provide: _angular_core__WEBPACK_IMPORTED_MODULE_2__["APP_INITIALIZER"], useFactory: _params_init_factory__WEBPACK_IMPORTED_MODULE_16__["paramsInitFactory"], deps: [_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injector"]], multi: true },
                { provide: _angular_common__WEBPACK_IMPORTED_MODULE_3__["LocationStrategy"], useClass: _angular_common__WEBPACK_IMPORTED_MODULE_3__["HashLocationStrategy"] },
                _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_11__["DnnInterceptor"],
                { provide: _angular_common_http__WEBPACK_IMPORTED_MODULE_5__["HTTP_INTERCEPTORS"], useClass: _shared_interceptors_set_headers_interceptor__WEBPACK_IMPORTED_MODULE_19__["SetHeadersInterceptor"], multi: true },
                { provide: _angular_common_http__WEBPACK_IMPORTED_MODULE_5__["HTTP_INTERCEPTORS"], useClass: _shared_interceptors_handle_errors_interceptor__WEBPACK_IMPORTED_MODULE_20__["HandleErrorsInterceptor"], multi: true },
                _shared_services_context__WEBPACK_IMPORTED_MODULE_15__["Context"],
                _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__["Title"]
            ],
            bootstrap: [_app_component__WEBPACK_IMPORTED_MODULE_14__["AppComponent"]]
        })
    ], AppModule);
    return AppModule;
}());



/***/ }),

/***/ "./src/app/params-init.factory.ts":
/*!****************************************!*\
  !*** ./src/app/params-init.factory.ts ***!
  \****************************************/
/*! exports provided: paramsInitFactory */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "paramsInitFactory", function() { return paramsInitFactory; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _edit_shared_helpers_url_helper__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../edit/shared/helpers/url-helper */ "../edit/shared/helpers/url-helper.ts");
/* harmony import */ var _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./shared/constants/dialog-types.constants */ "./src/app/shared/constants/dialog-types.constants.ts");
/* harmony import */ var _shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");




// tslint:disable-next-line:max-line-length


function paramsInitFactory(injector) {
    return function () {
        var e_1, _a, e_2, _b;
        console.log('Setting parameters config and clearing route');
        var eavKeys = Object.keys(sessionStorage).filter(function (key) { return key.startsWith(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["prefix"]); });
        var isParamsRoute = !window.location.hash.startsWith('#/');
        if (isParamsRoute) {
            console.log('Initial route:', window.location.href);
            try {
                // clear our part of the session
                for (var eavKeys_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(eavKeys), eavKeys_1_1 = eavKeys_1.next(); !eavKeys_1_1.done; eavKeys_1_1 = eavKeys_1.next()) {
                    var key = eavKeys_1_1.value;
                    sessionStorage.removeItem(key);
                }
            }
            catch (e_1_1) { e_1 = { error: e_1_1 }; }
            finally {
                try {
                    if (eavKeys_1_1 && !eavKeys_1_1.done && (_a = eavKeys_1.return)) _a.call(eavKeys_1);
                }
                finally { if (e_1) throw e_1.error; }
            }
            sessionStorage.setItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUrl"], window.location.href); // save url which opened the dialog
            sessionStorage.setItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDialog"], _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].Edit); // set edit dialog as the default
            // save params
            var urlHash = window.location.hash.substring(1); // substring removes first # char
            var queryParametersFromUrl = _edit_shared_helpers_url_helper__WEBPACK_IMPORTED_MODULE_2__["UrlHelper"].readQueryStringParameters(urlHash);
            var paramKeys = Object.keys(queryParametersFromUrl);
            try {
                for (var paramKeys_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(paramKeys), paramKeys_1_1 = paramKeys_1.next(); !paramKeys_1_1.done; paramKeys_1_1 = paramKeys_1.next()) {
                    var key = paramKeys_1_1.value;
                    var value = queryParametersFromUrl[key];
                    if (value === undefined || value === null) {
                        continue;
                    }
                    sessionStorage.setItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["prefix"] + key, value);
                }
            }
            catch (e_2_1) { e_2 = { error: e_2_1 }; }
            finally {
                try {
                    if (paramKeys_1_1 && !paramKeys_1_1.done && (_b = paramKeys_1.return)) _b.call(paramKeys_1);
                }
                finally { if (e_2) throw e_2.error; }
            }
            // redirect
            var router = injector.get(_angular_router__WEBPACK_IMPORTED_MODULE_1__["Router"]);
            var zoneId = sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyZoneId"]);
            var appId = sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyAppId"]);
            var dialog = sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDialog"]);
            var contentType = sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyContentType"]);
            var items = sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyItems"]);
            switch (dialog) {
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].Zone:
                    router.navigate([zoneId + "/apps"]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].AppImport:
                    router.navigate([zoneId + "/import"]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].App:
                    router.navigate([zoneId + "/" + appId + "/app"]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].ContentType:
                    router.navigate([zoneId + "/" + appId + "/fields/" + contentType]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].ContentItems:
                    router.navigate([zoneId + "/" + appId + "/items/" + contentType]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].Edit:
                    var editItems = JSON.parse(items);
                    var form = { items: editItems };
                    var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_5__["convertFormToUrl"])(form);
                    router.navigate([zoneId + "/" + appId + "/edit/" + formUrl]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].Develop:
                    router.navigate([zoneId + "/" + appId + "/code"]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].PipelineDesigner:
                    var pipelineId = sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPipelineId"]);
                    router.navigate([zoneId + "/" + appId + "/query/" + pipelineId]);
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].Replace:
                    var replaceItems = JSON.parse(items);
                    var rGuid = replaceItems[0].Group.Guid;
                    var rPart = replaceItems[0].Group.Part;
                    var rIndex = replaceItems[0].Group.Index;
                    var add = replaceItems[0].Group.Add;
                    var queryParams = add ? { add: true } : {};
                    router.navigate([zoneId + "/" + appId + "/" + rGuid + "/" + rPart + "/" + rIndex + "/replace"], { queryParams: queryParams });
                    break;
                case _shared_constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].InstanceList:
                    var groupItems = JSON.parse(items);
                    var gGuid = groupItems[0].Group.Guid;
                    var gPart = groupItems[0].Group.Part;
                    var gIndex = groupItems[0].Group.Index;
                    router.navigate([zoneId + "/" + appId + "/" + gGuid + "/" + gPart + "/" + gIndex + "/reorder"]);
                    break;
                default:
                    alert("Cannot open unknown dialog \"" + dialog + "\"");
                    try {
                        window.parent.$2sxc.totalPopup.close();
                    }
                    catch (error) { }
            }
        }
        else if (eavKeys.length === 0) {
            // if not params route and no params are saved, e.g. browser was reopened, throw error
            alert('Missing required url parameters. Please reopen dialog.');
            throw new Error('Missing required url parameters. Please reopen dialog.');
        }
        else {
            console.log('Initial route:', sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUrl"]));
        }
        loadEnvironment();
    };
}
function loadEnvironment() {
    $2sxc.env.load({
        page: parseInt(sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyTabId"]), 10),
        rvt: sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyRequestToken"]),
        root: sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPortalRoot"]),
        api: sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPortalRoot"]) + 'desktopmodules/2sxc/api/',
    });
}


/***/ }),

/***/ "./src/app/shared/constants/dialog-types.constants.ts":
/*!************************************************************!*\
  !*** ./src/app/shared/constants/dialog-types.constants.ts ***!
  \************************************************************/
/*! exports provided: DialogTypeConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DialogTypeConstants", function() { return DialogTypeConstants; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var DialogTypeConstants = /** @class */ (function () {
    function DialogTypeConstants() {
    }
    DialogTypeConstants.Zone = 'zone';
    DialogTypeConstants.AppImport = 'app-import';
    DialogTypeConstants.App = 'app';
    DialogTypeConstants.ContentType = 'contenttype';
    DialogTypeConstants.ContentItems = 'contentitems';
    DialogTypeConstants.Edit = 'edit';
    DialogTypeConstants.Develop = 'develop';
    DialogTypeConstants.PipelineDesigner = 'pipeline-designer';
    DialogTypeConstants.Replace = 'replace';
    DialogTypeConstants.InstanceList = 'instance-list';
    return DialogTypeConstants;
}());



/***/ }),

/***/ "./src/app/shared/constants/session.constants.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/constants/session.constants.ts ***!
  \*******************************************************/
/*! exports provided: prefix, keyZoneId, keyRequestToken, keyTabId, keyContentBlockId, keyModuleId, keyAppId, keyAppRoot, keyDebug, keyDialog, keyContentType, keyFa, keyItems, keyLang, keyLangPri, keyLangs, keyMode, keyPartOfPage, keyPortalRoot, keyPublishing, keyWebsiteRoot, keyFilters, keyUserCanDesign, keyUserCanDevelop, keyPipelineId, keyUrl */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "prefix", function() { return prefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyZoneId", function() { return keyZoneId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyRequestToken", function() { return keyRequestToken; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyTabId", function() { return keyTabId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyContentBlockId", function() { return keyContentBlockId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyModuleId", function() { return keyModuleId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyAppId", function() { return keyAppId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyAppRoot", function() { return keyAppRoot; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyDebug", function() { return keyDebug; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyDialog", function() { return keyDialog; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyContentType", function() { return keyContentType; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyFa", function() { return keyFa; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyItems", function() { return keyItems; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyLang", function() { return keyLang; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyLangPri", function() { return keyLangPri; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyLangs", function() { return keyLangs; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyMode", function() { return keyMode; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyPartOfPage", function() { return keyPartOfPage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyPortalRoot", function() { return keyPortalRoot; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyPublishing", function() { return keyPublishing; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyWebsiteRoot", function() { return keyWebsiteRoot; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyFilters", function() { return keyFilters; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyUserCanDesign", function() { return keyUserCanDesign; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyUserCanDevelop", function() { return keyUserCanDevelop; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyPipelineId", function() { return keyPipelineId; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "keyUrl", function() { return keyUrl; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var prefix = 'eav-';
var keyZoneId = prefix + 'zoneId';
var keyRequestToken = prefix + 'rvt';
var keyTabId = prefix + 'tid';
var keyContentBlockId = prefix + 'cbid';
var keyModuleId = prefix + 'mid';
var keyAppId = prefix + 'appId';
var keyAppRoot = prefix + 'approot';
var keyDebug = prefix + 'debug';
var keyDialog = prefix + 'dialog';
var keyContentType = prefix + 'contentType';
var keyFa = prefix + 'fa';
var keyItems = prefix + 'items';
var keyLang = prefix + 'lang';
var keyLangPri = prefix + 'langpri';
var keyLangs = prefix + 'langs';
var keyMode = prefix + 'mode';
var keyPartOfPage = prefix + 'partOfPage';
var keyPortalRoot = prefix + 'portalroot';
var keyPublishing = prefix + 'publishing';
var keyWebsiteRoot = prefix + 'websiteroot';
var keyFilters = prefix + 'filters';
var keyUserCanDesign = prefix + 'user%5BcanDesign%5D';
var keyUserCanDevelop = prefix + 'user%5BcanDevelop%5D';
var keyPipelineId = prefix + 'pipelineId';
/** Url which opened the dialog. Used for debugging */
var keyUrl = prefix + 'url';


/***/ }),

/***/ "./src/app/shared/helpers/angular-console-log.helper.ts":
/*!**************************************************************!*\
  !*** ./src/app/shared/helpers/angular-console-log.helper.ts ***!
  \**************************************************************/
/*! exports provided: angularConsoleLog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "angularConsoleLog", function() { return angularConsoleLog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../environments/environment */ "./src/environments/environment.ts");


function angularConsoleLog(message) {
    var optionalParams = [];
    for (var _i = 1; _i < arguments.length; _i++) {
        optionalParams[_i - 1] = arguments[_i];
    }
    if (_environments_environment__WEBPACK_IMPORTED_MODULE_1__["environment"].production) {
        return;
    }
    console.groupCollapsed.apply(console, Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__spread"])([message], optionalParams));
    // tslint:disable-next-line:no-console
    console.trace();
    console.groupEnd();
}


/***/ }),

/***/ "./src/app/shared/helpers/url-prep.helper.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/helpers/url-prep.helper.ts ***!
  \***************************************************/
/*! exports provided: convertFormToUrl, convertUrlToForm */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "convertFormToUrl", function() { return convertFormToUrl; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "convertUrlToForm", function() { return convertUrlToForm; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function convertFormToUrl(form) {
    var e_1, _a, e_2, _b;
    var _c, _d, _e;
    var formUrl = '';
    try {
        for (var _f = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(form.items), _g = _f.next(); !_g.done; _g = _f.next()) {
            var item = _g.value;
            if (formUrl) {
                formUrl += ',';
            }
            if (item.EntityId) {
                // Edit Item
                var editItem = item;
                formUrl += editItem.EntityId;
            }
            else if (item.ContentTypeName) {
                // Add Item
                var addItem = item;
                formUrl += 'new:' + addItem.ContentTypeName;
                if ((_c = addItem.For) === null || _c === void 0 ? void 0 : _c.String) {
                    formUrl += '&for:s~' + paramEncode(addItem.For.String) + ':' + addItem.For.Target;
                }
                else if ((_d = addItem.For) === null || _d === void 0 ? void 0 : _d.Number) {
                    formUrl += '&for:n~' + addItem.For.Number + ':' + addItem.For.Target;
                }
                else if ((_e = addItem.For) === null || _e === void 0 ? void 0 : _e.Guid) {
                    formUrl += '&for:g~' + addItem.For.Guid + ':' + addItem.For.Target;
                }
                if (addItem.Prefill) {
                    var keys = Object.keys(addItem.Prefill);
                    try {
                        for (var keys_1 = (e_2 = void 0, Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(keys)), keys_1_1 = keys_1.next(); !keys_1_1.done; keys_1_1 = keys_1.next()) {
                            var key = keys_1_1.value;
                            formUrl += '&prefill:' + key + '~' + paramEncode(addItem.Prefill[key]);
                        }
                    }
                    catch (e_2_1) { e_2 = { error: e_2_1 }; }
                    finally {
                        try {
                            if (keys_1_1 && !keys_1_1.done && (_b = keys_1.return)) _b.call(keys_1);
                        }
                        finally { if (e_2) throw e_2.error; }
                    }
                }
                if (addItem.DuplicateEntity) {
                    formUrl += '&copy:' + addItem.DuplicateEntity;
                }
            }
            else if (item.Group) {
                // Group Item
                var groupItem = item;
                formUrl += 'group:' + groupItem.Group.Guid + ':' + groupItem.Group.Index + ':' + groupItem.Group.Part + ':' + groupItem.Group.Add;
            }
        }
    }
    catch (e_1_1) { e_1 = { error: e_1_1 }; }
    finally {
        try {
            if (_g && !_g.done && (_a = _f.return)) _a.call(_f);
        }
        finally { if (e_1) throw e_1.error; }
    }
    return formUrl;
}
function convertUrlToForm(formUrl) {
    var e_3, _a, e_4, _b;
    var form = { items: [] };
    var items = formUrl.split(',');
    var isNumber = /^[0-9]*$/g;
    try {
        for (var items_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(items), items_1_1 = items_1.next(); !items_1_1.done; items_1_1 = items_1.next()) {
            var item = items_1_1.value;
            if (isNumber.test(item)) {
                // Edit Item
                var editItem = { EntityId: parseInt(item, 10) };
                form.items.push(editItem);
            }
            else if (item.startsWith('new:')) {
                // Add Item
                var addItem = {};
                var options = item.split('&');
                try {
                    for (var options_1 = (e_4 = void 0, Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(options)), options_1_1 = options_1.next(); !options_1_1.done; options_1_1 = options_1.next()) {
                        var option = options_1_1.value;
                        if (option.startsWith('new:')) {
                            // Add Item ContentType
                            var newParams = option.split(':');
                            addItem.ContentTypeName = newParams[1];
                        }
                        else if (option.startsWith('for:')) {
                            // Add Item For
                            addItem.For = {};
                            var forParams = option.split(':');
                            var forType = forParams[1].split('~')[0];
                            var forValue = forParams[1].split('~')[1];
                            var forTarget = forParams[2];
                            switch (forType) {
                                case 's':
                                    addItem.For.String = paramDecode(forValue);
                                    break;
                                case 'n':
                                    addItem.For.Number = parseInt(forValue, 10);
                                    break;
                                case 'g':
                                    addItem.For.Guid = forValue;
                                    break;
                            }
                            addItem.For.Target = forTarget;
                        }
                        else if (option.startsWith('prefill:')) {
                            // Add Item Prefill
                            if (addItem.Prefill == null) {
                                addItem.Prefill = {};
                            }
                            var prefillParams = option.split(':');
                            var key = prefillParams[1].split('~')[0];
                            var value = paramDecode(prefillParams[1].split('~')[1]);
                            addItem.Prefill[key] = value;
                        }
                        else if (option.startsWith('copy:')) {
                            // Add Item Copy
                            var copyParams = option.split(':');
                            addItem.DuplicateEntity = parseInt(copyParams[1], 10);
                        }
                    }
                }
                catch (e_4_1) { e_4 = { error: e_4_1 }; }
                finally {
                    try {
                        if (options_1_1 && !options_1_1.done && (_b = options_1.return)) _b.call(options_1);
                    }
                    finally { if (e_4) throw e_4.error; }
                }
                form.items.push(addItem);
            }
            else if (item.startsWith('group:')) {
                // Group Item
                var groupParams = item.split(':');
                var groupItem = {
                    Group: {
                        Guid: groupParams[1],
                        Index: parseInt(groupParams[2], 10),
                        Part: groupParams[3],
                        Add: groupParams[4] === 'true',
                    }
                };
                form.items.push(groupItem);
            }
        }
    }
    catch (e_3_1) { e_3 = { error: e_3_1 }; }
    finally {
        try {
            if (items_1_1 && !items_1_1.done && (_a = items_1.return)) _a.call(items_1);
        }
        finally { if (e_3) throw e_3.error; }
    }
    return form;
}
/** Encodes characters in URL parameter to not mess up routing. Don't forget to decode it! :) */
function paramEncode(text) {
    text = text.replace(/\//g, '%2F');
    text = text.replace(/\:/g, '%3A');
    text = text.replace(/\&/g, '%26');
    text = text.replace(/\~/g, '%7E');
    return text;
}
/** Decodes characters in URL parameter */
function paramDecode(text) {
    text = text.replace(/%2F/g, '/');
    text = text.replace(/%3A/g, ':');
    text = text.replace(/%26/g, '&');
    text = text.replace(/%7E/g, '~');
    return text;
}


/***/ }),

/***/ "./src/app/shared/interceptors/handle-errors.interceptor.ts":
/*!******************************************************************!*\
  !*** ./src/app/shared/interceptors/handle-errors.interceptor.ts ***!
  \******************************************************************/
/*! exports provided: HandleErrorsInterceptor */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HandleErrorsInterceptor", function() { return HandleErrorsInterceptor; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");




var HandleErrorsInterceptor = /** @class */ (function () {
    function HandleErrorsInterceptor() {
        /** URLs excluded from detailed error alert  */
        this.excludedUrls = [
            'dist/ng-edit/i18n',
        ];
    }
    HandleErrorsInterceptor.prototype.intercept = function (req, next) {
        var _this = this;
        return next.handle(req).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["catchError"])(function (error) {
            if (!_this.checkIfExcluded(error.url)) {
                _this.showDetailedHttpError(error);
            }
            return Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["throwError"])(error);
        }));
    };
    HandleErrorsInterceptor.prototype.checkIfExcluded = function (url) {
        var e_1, _a;
        try {
            for (var _b = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(this.excludedUrls), _c = _b.next(); !_c.done; _c = _b.next()) {
                var excludedUrl = _c.value;
                if (url.includes(excludedUrl)) {
                    return true;
                }
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (_c && !_c.done && (_a = _b.return)) _a.call(_b);
            }
            finally { if (e_1) throw e_1.error; }
        }
        return false;
    };
    HandleErrorsInterceptor.prototype.showDetailedHttpError = function (error) {
        var infoText = 'Had an error talking to the server (status ' + error.status + ').';
        var srvResp = error.error;
        if (srvResp) {
            var msg = srvResp.Message;
            if (msg) {
                infoText += '\nMessage: ' + msg;
            }
            var msgDet = srvResp.MessageDetail || srvResp.ExceptionMessage;
            if (msgDet) {
                infoText += '\nDetail: ' + msgDet;
            }
            if (msgDet && msgDet.indexOf('No action was found') === 0) {
                if (msgDet.indexOf('that matches the name') > 0) {
                    infoText += '\n\nTip from 2sxc: you probably got the action-name wrong in your JS.';
                }
                else if (msgDet.indexOf('that matches the request.') > 0) {
                    infoText += '\n\nTip from 2sxc: Seems like the parameters are the wrong amount or type.';
                }
            }
            if (msg && msg.indexOf('Controller') === 0 && msg.indexOf('not found') > 0) {
                infoText += '\n\nTip from 2sxc: you probably spelled the controller name wrong or forgot to remove the word \'controller\' from the call in JS. To call a controller called \'DemoController\' only use \'Demo\'.';
            }
            infoText += '\n\nif you are an advanced user you can learn more about what went wrong - discover how on 2sxc.org/help?tag=debug';
        }
        alert(infoText);
    };
    HandleErrorsInterceptor = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [])
    ], HandleErrorsInterceptor);
    return HandleErrorsInterceptor;
}());



/***/ }),

/***/ "./src/app/shared/interceptors/set-headers.interceptor.ts":
/*!****************************************************************!*\
  !*** ./src/app/shared/interceptors/set-headers.interceptor.ts ***!
  \****************************************************************/
/*! exports provided: SetHeadersInterceptor */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SetHeadersInterceptor", function() { return SetHeadersInterceptor; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var SetHeadersInterceptor = /** @class */ (function () {
    function SetHeadersInterceptor() {
    }
    SetHeadersInterceptor.prototype.intercept = function (req, next) {
        var modified;
        if (req.body instanceof FormData) {
            // sending files. Do not set content type so browser can add delimiter boundary automatically
            return next.handle(req);
        }
        else {
            modified = req.clone({
                setHeaders: {
                    'Content-Type': 'application/json;charset=UTF-8',
                }
            });
            return next.handle(modified);
        }
    };
    SetHeadersInterceptor = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [])
    ], SetHeadersInterceptor);
    return SetHeadersInterceptor;
}());



/***/ }),

/***/ "./src/app/shared/services/context.ts":
/*!********************************************!*\
  !*** ./src/app/shared/services/context.ts ***!
  \********************************************/
/*! exports provided: Context */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Context", function() { return Context; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _constants_session_constants__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../constants/session.constants */ "./src/app/shared/constants/session.constants.ts");
/* harmony import */ var _helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../helpers/angular-console-log.helper */ "./src/app/shared/helpers/angular-console-log.helper.ts");




/** The context provides information */
var Context = /** @class */ (function () {
    function Context(parentContext) {
        /** Determines if the context is ready to use, and everything is initialized */
        this.ready = false;
        this.parent = parentContext;
        // spm NOTE: I've given id to every context to make it easier to follow how things work
        var globalWindow = window;
        if (!globalWindow.contextId) {
            globalWindow.contextId = 0;
        }
        this.id = globalWindow.contextId++;
        Object(_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_3__["angularConsoleLog"])('Context.constructor', this);
    }
    Object.defineProperty(Context.prototype, "zoneId", {
        /** The current Zone ID */
        get: function () {
            return this._zoneId || (this._zoneId = this.routeNum(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyZoneId"]) || this.parent.zoneId);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Context.prototype, "appId", {
        /** The current App ID */
        get: function () {
            return (this._appId != null) ? this._appId : (this._appId = this.routeNum(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyAppId"]) || this.parent.appId);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Context.prototype, "appRoot", {
        /** Root of the current App */
        get: function () {
            return (this._appRoot != null) ? this._appRoot : (this._appRoot = this.parent.appRoot);
        },
        set: function (path) {
            this._appRoot = path;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Context.prototype, "requestToken", {
        /**
         * The request verification token for http requests.
         * It's only loaded from the root, never from sub-contexts
         */
        get: function () { return this._rvt || (this._rvt = this.parent.requestToken); },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Context.prototype, "tabId", {
        /** Tab Id is global */
        get: function () {
            return this._tabId || (this._tabId = this.routeNum(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyTabId"]) || this.parent.tabId);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Context.prototype, "contentBlockId", {
        /** Content Block Id is global */
        get: function () {
            return this._contentBlockId || (this._contentBlockId = this.routeNum(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyContentBlockId"]) || this.parent.contentBlockId);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Context.prototype, "moduleId", {
        /** Module Id is global */
        get: function () {
            return this._moduleId || (this._moduleId = this.routeNum(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyModuleId"]) || this.parent.moduleId);
        },
        enumerable: true,
        configurable: true
    });
    /**
     * This is the initializer at entry-componets of modules.
     * It ensures that within that module, the context has the values given by the route
     */
    Context.prototype.init = function (route) {
        this.routeSnapshot = route && route.snapshot;
        this.clearCachedValues();
        this.ready = route != null;
        Object(_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_3__["angularConsoleLog"])('Context.init', this, route);
    };
    Context.prototype.initRoot = function () {
        // required, global things
        this._rvt = sessionStorage.getItem(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyRequestToken"]);
        this._zoneId = this.sessionNumber(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyZoneId"]);
        this._tabId = this.sessionNumber(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyTabId"]);
        this._contentBlockId = this.sessionNumber(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyContentBlockId"]);
        this._moduleId = this.sessionNumber(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyModuleId"]);
        if (!this._rvt || !this._zoneId || !this._tabId || !this._contentBlockId || !this._moduleId) {
            throw new Error('Context is missing some of the required parameters');
        }
        // optional global things
        this._appId = this.sessionNumber(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyAppId"]);
        this._appRoot = sessionStorage.getItem(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["keyAppRoot"]);
        this.ready = true;
        Object(_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_3__["angularConsoleLog"])('Context.initRoot', this);
    };
    Context.prototype.sessionNumber = function (name) {
        var result = sessionStorage.getItem(name);
        if (result !== null) {
            var num = parseInt(result, 10);
            return isNaN(num) ? null : num;
        }
        return null;
    };
    /**
     * Get a number from the route, or optionally its parents.
     * Returns value in route or null
     */
    Context.prototype.routeNum = function (name) {
        // catch case where state is null, like when the recursive parent is in use
        if (this.routeSnapshot == null) {
            return null;
        }
        var paramName = name.substring(_constants_session_constants__WEBPACK_IMPORTED_MODULE_2__["prefix"].length);
        var result = this.routeSnapshot.paramMap.get(paramName);
        if (result !== null) {
            var num = parseInt(result, 10);
            return isNaN(num) ? null : num;
        }
    };
    /**
     * Clears cached values. Required when one module instance is opened multiple times,
     * e.g. Apps Management -> App Admin for appId 2 -> back -> App Admin for appId 17.
     * Module is reused, and so is context and it contains values for previous appId.
     */
    Context.prototype.clearCachedValues = function () {
        this._zoneId = null;
        this._appId = null;
        this._appRoot = null;
        this._rvt = null;
        this._tabId = null;
        this._contentBlockId = null;
        this._moduleId = null;
    };
    Context.ctorParameters = function () { return [
        { type: Context, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Optional"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["SkipSelf"] }] }
    ]; };
    Context = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__param"])(0, Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Optional"])()), Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__param"])(0, Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["SkipSelf"])()),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [Context])
    ], Context);
    return Context;
}());



/***/ }),

/***/ "./src/environments/environment.ts":
/*!*****************************************!*\
  !*** ./src/environments/environment.ts ***!
  \*****************************************/
/*! exports provided: environment */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "environment", function() { return environment; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

var environment = {
    production: false
};
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.


/***/ }),

/***/ "./src/main.ts":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser-dynamic */ "../../node_modules/@angular/platform-browser-dynamic/__ivy_ngcc__/fesm5/platform-browser-dynamic.js");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./app/app.module */ "./src/app/app.module.ts");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./environments/environment */ "./src/environments/environment.ts");





if (_environments_environment__WEBPACK_IMPORTED_MODULE_4__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["enableProdMode"])();
}
Object(_angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__["platformBrowserDynamic"])().bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_3__["AppModule"])
    .catch(function (err) { return console.error(err); });


/***/ }),

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! C:\Projects\eav-item-dialog-angular\projects\ng-dialogs\src\main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map