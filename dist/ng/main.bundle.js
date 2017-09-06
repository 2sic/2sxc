webpackJsonp(["main"],{

/***/ "../../../../../src/$$_gendir lazy recursive":
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	return new Promise(function(resolve, reject) { reject(new Error("Cannot find module '" + req + "'.")); });
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "../../../../../src/$$_gendir lazy recursive";

/***/ }),

/***/ "../../../../../src/app/app.component.css":
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__("../../../../css-loader/lib/css-base.js")(false);
// imports


// module
exports.push([module.i, ":host {\r\n    display: block;\r\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ "../../../../../src/app/app.component.html":
/***/ (function(module, exports) {

module.exports = "<app-template-picker *ngIf=\"name === 'dash-view' || name === 'layout'\"></app-template-picker>"

/***/ }),

/***/ "../../../../../src/app/app.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__ = __webpack_require__("../../../../@ngx-translate/core/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_version_dialog_dialog_component__ = __webpack_require__("../../../../../src/app/version-dialog/dialog.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_material__ = __webpack_require__("../../../material/@angular/material.es5.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var AppComponent = (function () {
    function AppComponent(translate, dialog) {
        this.translate = translate;
        this.dialog = dialog;
        var langs = ['en', 'de', 'es', 'fr', 'it', 'nl', 'uk'];
        translate.addLangs(langs);
        translate.setDefaultLang($2sxc.urlParams.require('langpri').split('-')[0]);
        translate.use($2sxc.urlParams.require('lang').split('-')[0]);
        this.name = $2sxc.urlParams.require('dialog');
        var frame = window.frameElement;
        if (this.name === 'item-history') {
            this.dialog.open(__WEBPACK_IMPORTED_MODULE_2_app_version_dialog_dialog_component__["b" /* DialogComponent */]).afterClosed()
                .subscribe(function () { return frame.toggle(false); });
        }
    }
    return AppComponent;
}());
AppComponent = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_1__angular_core__["n" /* Component */])({
        selector: 'app-root',
        template: __webpack_require__("../../../../../src/app/app.component.html"),
        styles: [__webpack_require__("../../../../../src/app/app.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["d" /* TranslateService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["d" /* TranslateService */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__angular_material__["c" /* MdDialog */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3__angular_material__["c" /* MdDialog */]) === "function" && _b || Object])
], AppComponent);

var _a, _b;
//# sourceMappingURL=app.component.js.map

/***/ }),

/***/ "../../../../../src/app/app.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* unused harmony export HttpLoaderFactory */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppModule; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_http_loader__ = __webpack_require__("../../../../@ngx-translate/http-loader/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__("../../../http/@angular/http.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__ngx_translate_core__ = __webpack_require__("../../../../@ngx-translate/core/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__ = __webpack_require__("../../../platform-browser/@angular/platform-browser.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_forms__ = __webpack_require__("../../../forms/@angular/forms.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__app_component__ = __webpack_require__("../../../../../src/app/app.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_app_template_picker_template_picker_module__ = __webpack_require__("../../../../../src/app/template-picker/template-picker.module.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_app_version_dialog_version_dialog_module__ = __webpack_require__("../../../../../src/app/version-dialog/version-dialog.module.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};









function HttpLoaderFactory(http) {
    return new __WEBPACK_IMPORTED_MODULE_0__ngx_translate_http_loader__["a" /* TranslateHttpLoader */](http, "../i18n/sxc-admin-", ".js");
}
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_4__angular_core__["L" /* NgModule */])({
        declarations: [
            __WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */]
        ],
        exports: [],
        imports: [
            __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__["a" /* BrowserModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_forms__["b" /* FormsModule */],
            __WEBPACK_IMPORTED_MODULE_7_app_template_picker_template_picker_module__["a" /* TemplatePickerModule */],
            __WEBPACK_IMPORTED_MODULE_2__ngx_translate_core__["b" /* TranslateModule */].forRoot({
                loader: {
                    provide: __WEBPACK_IMPORTED_MODULE_2__ngx_translate_core__["a" /* TranslateLoader */],
                    useFactory: HttpLoaderFactory,
                    deps: [__WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* Http */]]
                }
            }),
            __WEBPACK_IMPORTED_MODULE_8_app_version_dialog_version_dialog_module__["a" /* VersionDialogModule */],
        ],
        providers: [],
        bootstrap: [__WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */]]
    })
], AppModule);

//# sourceMappingURL=app.module.js.map

/***/ }),

/***/ "../../../../../src/app/core/$2sxc.service.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return $2sxcService; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var $2sxcService = (function () {
    function $2sxcService() {
        this.sxc = $2sxc($2sxc.urlParams.require('mid'), $2sxc.urlParams.require('cbid'));
    }
    return $2sxcService;
}());
$2sxcService = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["B" /* Injectable */])(),
    __metadata("design:paramtypes", [])
], $2sxcService);

//# sourceMappingURL=$2sxc.service.js.map

/***/ }),

/***/ "../../../../../src/app/core/boot-control.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return BootController; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_rxjs_Subject__ = __webpack_require__("../../../../rxjs/Subject.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_rxjs_Subject__);

/**
 * Special reboot controller, to restart the angular app
 * when critical parameters were changes
 */
var BootController = (function () {
    function BootController() {
        this._reboot = new __WEBPACK_IMPORTED_MODULE_0_rxjs_Subject__["Subject"]();
        this.reboot$ = this._reboot.asObservable();
    }
    BootController.getbootControl = function () {
        if (!BootController.instance) {
            BootController.instance = new BootController();
        }
        return BootController.instance;
    };
    BootController.prototype.watchReboot = function () {
        return this.reboot$;
    };
    BootController.prototype.restart = function () {
        console.log("restarting...");
        this._reboot.next(true);
    };
    return BootController;
}());

//# sourceMappingURL=boot-control.js.map

/***/ }),

/***/ "../../../../../src/app/core/core.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CoreModule; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__("../../../common/@angular/common.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__ = __webpack_require__("../../../../../src/app/core/module-api.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__ = __webpack_require__("../../../../../src/app/core/$2sxc.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_http__ = __webpack_require__("../../../http/@angular/http.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_app_core_http_interceptor_service_provider__ = __webpack_require__("../../../../../src/app/core/http-interceptor.service.provider.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};






var CoreModule = (function () {
    function CoreModule() {
    }
    return CoreModule;
}());
CoreModule = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["L" /* NgModule */])({
        imports: [
            __WEBPACK_IMPORTED_MODULE_1__angular_common__["a" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_4__angular_http__["d" /* HttpModule */]
        ],
        declarations: [],
        providers: [
            __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */],
            __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__["a" /* $2sxcService */],
            __WEBPACK_IMPORTED_MODULE_5_app_core_http_interceptor_service_provider__["a" /* Http2SxcHttpProvider */],
        ]
    })
], CoreModule);

//# sourceMappingURL=core.module.js.map

/***/ }),

/***/ "../../../../../src/app/core/http-interceptor.service.provider.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* unused harmony export Http2SxcProviderFactory */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Http2SxcHttpProvider; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__http_interceptor_service__ = __webpack_require__("../../../../../src/app/core/http-interceptor.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__("../../../http/@angular/http.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__("../../../../../src/app/core/$2sxc.service.ts");



function Http2SxcProviderFactory(backend, defaultOptions, sxc) {
    return new __WEBPACK_IMPORTED_MODULE_0__http_interceptor_service__["a" /* Http2sxc */](backend, defaultOptions, sxc);
}
var Http2SxcHttpProvider = {
    provide: __WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* Http */],
    useFactory: Http2SxcProviderFactory,
    deps: [__WEBPACK_IMPORTED_MODULE_1__angular_http__["f" /* XHRBackend */], __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* RequestOptions */], __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]]
};
//# sourceMappingURL=http-interceptor.service.provider.js.map

/***/ }),

/***/ "../../../../../src/app/core/http-interceptor.service.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Http2sxc; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__("../../../http/@angular/http.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__("../../../../../src/app/core/$2sxc.service.ts");
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
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var Http2sxc = (function (_super) {
    __extends(Http2sxc, _super);
    function Http2sxc(backend, defaultOptions, sxc) {
        var _this = _super.call(this, backend, defaultOptions) || this;
        _this.sxc = sxc;
        _this.configure(defaultOptions);
        return _this;
    }
    Http2sxc.prototype.request = function (url, options) {
        if (options === void 0) { options = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* RequestOptions */](); }
        var isDevMode = window.location.hostname === 'localhost';
        this.configure(options);
        if (typeof url === 'string')
            url = this.sxc.sxc.resolveServiceUrl(url);
        else
            url.url = this.sxc.sxc.resolveServiceUrl(url.url);
        return _super.prototype.request.call(this, url, options);
    };
    Http2sxc.prototype.configure = function (options) {
        var mid = $2sxc.urlParams.require('mid'), tid = $2sxc.urlParams.require('tid'), cbid = $2sxc.urlParams.require('cbid');
        if (!options.headers)
            options.headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Headers */]();
        options.headers.set('ModuleId', mid);
        options.headers.set('TabId', tid);
        options.headers.set('ContentBlockId', cbid);
        options.headers.set('RequestVerificationToken', window.$.ServicesFramework(mid).getAntiForgeryValue());
        options.headers.set('X-Debugging-Hint', 'bootstrapped by 2sxc4ng');
        return options;
    };
    return Http2sxc;
}(__WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* Http */]));
Http2sxc = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["B" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["a" /* ConnectionBackend */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["a" /* ConnectionBackend */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* RequestOptions */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* RequestOptions */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _c || Object])
], Http2sxc);

var _a, _b, _c;
//# sourceMappingURL=http-interceptor.service.js.map

/***/ }),

/***/ "../../../../../src/app/core/module-api.service.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ModuleApiService; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_add_operator_map__ = __webpack_require__("../../../../rxjs/add/operator/map.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_add_operator_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_rxjs_add_operator_map__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__("../../../../../src/app/core/$2sxc.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_http__ = __webpack_require__("../../../http/@angular/http.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__ = __webpack_require__("../../../../rxjs/Subject.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__);
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var ModuleApiService = (function () {
    function ModuleApiService(http, sxc) {
        this.http = http;
        this.sxc = sxc;
        this.appSubject = new __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__["Subject"]();
        this.contentTypeSubject = new __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__["Subject"]();
        this.gettingStartedSubject = new __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__["Subject"]();
        this.templateSubject = new __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__["Subject"]();
        this.apps = this.appSubject.asObservable();
        this.contentTypes = this.contentTypeSubject.asObservable();
        this.gettingStarted = this.gettingStartedSubject.asObservable();
        this.templates = this.templateSubject.asObservable();
    }
    ModuleApiService.prototype.setAppId = function (appId) {
        return this.http.get("view/Module/SetAppId?appId=" + appId);
    };
    ModuleApiService.prototype.loadGettingStarted = function (isContentApp) {
        var _this = this;
        var obs = this.http.get("View/Module/RemoteInstallDialogUrl?dialog=gettingstarted&isContentApp=" + isContentApp)
            .map(function (response) { return response.json(); });
        obs.subscribe(function (json) { return _this.gettingStartedSubject.next(json); });
        return obs;
    };
    ModuleApiService.prototype.loadTemplates = function () {
        var _this = this;
        var obs = this.http.get('View/Module/GetSelectableTemplates')
            .map(function (response) { return response.json() || []; });
        obs.subscribe(function (json) { return _this.templateSubject.next(json); });
        return obs;
    };
    ModuleApiService.prototype.loadContentTypes = function () {
        var _this = this;
        var obs = this.http.get('View/Module/GetSelectableContentTypes')
            .map(function (response) { return (response.json() || []).map(function (x) {
            x.Label = (x.Metadata && x.Metadata.Label)
                ? x.Metadata.Label
                : x.Name;
            return x;
        }); });
        obs.subscribe(function (json) { return _this.contentTypeSubject.next(json); });
        return obs;
    };
    ModuleApiService.prototype.loadApps = function () {
        var _this = this;
        var obs = this.http.get('View/Module/GetSelectableApps')
            .map(function (response) { return response.json().map(_this.parseResultObject); });
        obs.subscribe(function (json) { return _this.appSubject.next(json); });
        return obs;
    };
    ModuleApiService.prototype.parseResultObject = function (obj) {
        return Object.keys(obj)
            .reduce(function (t, v, k) {
            t[v.split('').reduce(function (t, v, k) { return t + (k === 0 ? v.toLowerCase() : v); }, '')] = obj[v];
            return t;
        }, {});
    };
    return ModuleApiService;
}());
ModuleApiService = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["B" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__angular_http__["c" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3__angular_http__["c" /* Http */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _b || Object])
], ModuleApiService);

var _a, _b;
//# sourceMappingURL=module-api.service.js.map

/***/ }),

/***/ "../../../../../src/app/installer/installer.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"progress\" *ngIf=\"showProgress\">\r\n  <md-progress-spinner [mode]=\"'indeterminate'\"></md-progress-spinner>\r\n  <span>Installing {{ currentPackage?.displayName }}..</span>\r\n</div>\r\n<div *ngIf=\"ready\">\r\n  <iframe class=\"fr-getting-started\" id=\"frGettingStarted\" [src]=\"remoteInstallerUrl\" width=\"100%\" height=\"300px\"></iframe>\r\n</div>"

/***/ }),

/***/ "../../../../../src/app/installer/installer.component.scss":
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__("../../../../css-loader/lib/css-base.js")(false);
// imports


// module
exports.push([module.i, ":host iframe {\n  border: none;\n  height: 500px; }\n\n:host .progress {\n  position: absolute;\n  left: 0;\n  top: 0;\n  height: 100%;\n  width: 100%;\n  background: rgba(255, 255, 255, 0.8);\n  display: -webkit-box;\n  display: -ms-flexbox;\n  display: flex;\n  -webkit-box-pack: center;\n      -ms-flex-pack: center;\n          justify-content: center;\n  -webkit-box-orient: vertical;\n  -webkit-box-direction: normal;\n      -ms-flex-direction: column;\n          flex-direction: column;\n  text-align: center; }\n  :host .progress md-progress-spinner {\n    margin: 0 auto; }\n  :host .progress span {\n    line-height: 48px; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ "../../../../../src/app/installer/installer.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InstallerComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_app_installer_installer_service__ = __webpack_require__("../../../../../src/app/installer/installer.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__ = __webpack_require__("../../../../../src/app/core/module-api.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__ = __webpack_require__("../../../platform-browser/@angular/platform-browser.es5.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var InstallerComponent = (function () {
    function InstallerComponent(installer, api, sanitizer) {
        var _this = this;
        this.installer = installer;
        this.api = api;
        this.sanitizer = sanitizer;
        this.remoteInstallerUrl = '';
        this.ready = false;
        this.api.gettingStarted
            .subscribe(function (url) {
            _this.remoteInstallerUrl = _this.sanitizer.bypassSecurityTrustResourceUrl(url);
            _this.ready = true;
        });
    }
    InstallerComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.api.loadGettingStarted(this.isContentApp);
        window.addEventListener('message', function (evt) {
            var data;
            try {
                data = JSON.parse(evt.data);
            }
            catch (e) {
                return false;
            }
            if (~~data.moduleId !== ~~$2sxc.urlParams.require('mid'))
                return;
            if (data.action !== 'install')
                return;
            var packages = Object.values(data.packages), packagesDisplayNames = packages.reduce(function (t, c) { return t + " - " + c.displayName + "\n"; }, '');
            if (!confirm("\n          Do you want to install these packages?\n\n\n          " + packagesDisplayNames + "\nThis could take 10 to 60 seconds per package, \n          please don't reload the page while it's installing."))
                return;
            _this.showProgress = true;
            _this.installer.installPackages(packages)
                .subscribe(function (p) { return _this.currentPackage = p; }, function (e) {
                _this.showProgress = false;
                alert('An error occurred.');
            }, function () {
                _this.showProgress = false;
                alert('Installation complete. If you saw no errors, everything worked.');
                window.top.location.reload();
            });
        }, false);
    };
    return InstallerComponent;
}());
__decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["E" /* Input */])(),
    __metadata("design:type", Boolean)
], InstallerComponent.prototype, "isContentApp", void 0);
InstallerComponent = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["n" /* Component */])({
        selector: 'app-installer',
        template: __webpack_require__("../../../../../src/app/installer/installer.component.html"),
        styles: [__webpack_require__("../../../../../src/app/installer/installer.component.scss")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1_app_installer_installer_service__["a" /* InstallerService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1_app_installer_installer_service__["a" /* InstallerService */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__["c" /* DomSanitizer */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__["c" /* DomSanitizer */]) === "function" && _c || Object])
], InstallerComponent);

var _a, _b, _c;
//# sourceMappingURL=installer.component.js.map

/***/ }),

/***/ "../../../../../src/app/installer/installer.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InstallerModule; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__("../../../common/@angular/common.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__installer_component__ = __webpack_require__("../../../../../src/app/installer/installer.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_installer_installer_service__ = __webpack_require__("../../../../../src/app/installer/installer.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_material__ = __webpack_require__("../../../material/@angular/material.es5.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};





var InstallerModule = (function () {
    function InstallerModule() {
    }
    return InstallerModule;
}());
InstallerModule = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["L" /* NgModule */])({
        imports: [
            __WEBPACK_IMPORTED_MODULE_1__angular_common__["a" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_4__angular_material__["g" /* MdProgressSpinnerModule */],
        ],
        exports: [
            __WEBPACK_IMPORTED_MODULE_2__installer_component__["a" /* InstallerComponent */]
        ],
        declarations: [
            __WEBPACK_IMPORTED_MODULE_2__installer_component__["a" /* InstallerComponent */]
        ],
        providers: [
            __WEBPACK_IMPORTED_MODULE_3_app_installer_installer_service__["a" /* InstallerService */]
        ]
    })
], InstallerModule);

//# sourceMappingURL=installer.module.js.map

/***/ }),

/***/ "../../../../../src/app/installer/installer.service.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InstallerService; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_Rx__ = __webpack_require__("../../../../rxjs/Rx.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_rxjs_Rx__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_http__ = __webpack_require__("../../../http/@angular/http.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__ = __webpack_require__("../../../../rxjs/Subject.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__);
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var InstallerService = (function () {
    function InstallerService(http) {
        this.http = http;
    }
    InstallerService.prototype.installPackages = function (packages) {
        var _this = this;
        var subject = new __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__["Subject"](), res = packages.reduce(function (t, c) { return t
            .switchMap(function () {
            if (!c.url)
                return __WEBPACK_IMPORTED_MODULE_1_rxjs_Rx__["Observable"].from([true]);
            subject.next(c);
            return _this.http.get("app-sys/installer/installpackage?packageUrl=" + c.url);
        }); }, __WEBPACK_IMPORTED_MODULE_1_rxjs_Rx__["Observable"].from([true]))
            .subscribe(function () { return subject.complete(); }, function (e) { return subject.error(e); });
        return subject.asObservable();
    };
    return InstallerService;
}());
InstallerService = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["B" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__angular_http__["c" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2__angular_http__["c" /* Http */]) === "function" && _a || Object])
], InstallerService);

var _a;
//# sourceMappingURL=installer.service.js.map

/***/ }),

/***/ "../../../../../src/app/template-picker/content-type-filter.pipe.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ContentTypeFilterPipe; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var ContentTypeFilterPipe = (function () {
    function ContentTypeFilterPipe() {
    }
    ContentTypeFilterPipe.prototype.transform = function (templates) {
        return templates
            .filter(function (t) { return !t.IsHidden; });
    };
    return ContentTypeFilterPipe;
}());
ContentTypeFilterPipe = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["W" /* Pipe */])({
        name: 'contentTypeFilter'
    })
], ContentTypeFilterPipe);

//# sourceMappingURL=content-type-filter.pipe.js.map

/***/ }),

/***/ "../../../../../src/app/template-picker/template-filter.pipe.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TemplateFilterPipe; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var TemplateFilterPipe = (function () {
    function TemplateFilterPipe() {
    }
    TemplateFilterPipe.prototype.transform = function (templates, args) {
        return templates
            .filter(function (t) { return !t.IsHidden && (!args.isContentApp || t.ContentTypeStaticName === (args.contentTypeId === '_LayoutElement' ? '' : (args.contentTypeId || ''))); });
    };
    return TemplateFilterPipe;
}());
TemplateFilterPipe = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["W" /* Pipe */])({
        name: 'templateFilter'
    })
], TemplateFilterPipe);

//# sourceMappingURL=template-filter.pipe.js.map

/***/ }),

/***/ "../../../../../src/app/template-picker/template-picker.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"content\">\r\n    <md-progress-bar [ngStyle]=\"{ opacity: (!ready || loading) ? 1 : 0 }\" [mode]=\"'indeterminate'\"></md-progress-bar>\r\n    <div *ngIf=\"ready\" class=\"card\">\r\n        <div class=\"top-controls\" fxLayout=\"row\" fxLayoutAlign=\"center center\">\r\n            <button md-fab *ngIf=\"template\" (click)=\"persistTemplate()\" [attr.title]=\"'TemplatePicker.Save' | translate\">\r\n                <md-icon>check</md-icon>\r\n            </button>\r\n            <button md-mini-fab class=\"secondary\" *ngIf=\"undoTemplateId !== null\" (click)=\"frame.cancel()\"\r\n                [attr.title]=\"('TemplatePicker.' + (isContentApp ? 'Cancel' : 'Close')) | translate\">\r\n                <md-icon>close</md-icon>\r\n            </button>\r\n        </div>\r\n        <md-tab-group [(selectedIndex)]=\"tabIndex\">\r\n            <md-tab [label]=\"(isContentApp ? (contentType?.Name || ('TemplatePicker.ContentTypePickerDefault' | translate)) : (app?.name || ('TemplatePicker.AppPickerDefault' | translate)))\">\r\n                <div *ngIf=\"!isContentApp; else contentApp\" class=\"tiles\">\r\n                    <div class=\"tile\" [ngClass]=\"{ active: app?.appId === a.appId }\" [attr.title]=\"a.name\" (click)=\"app?.appId === a.appId ? switchTab() : updateApp(a)\"\r\n                        (dblclick)=\"switchTab()\" *ngFor=\"let a of apps\">\r\n                        <div class=\"bg\">\r\n                            <img *ngIf=\"a.thumbnail !== null && a.thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"a.thumbnail + '?w=176&h=176'\">\r\n                            <div *ngIf=\"a.thumbnail === null || a.thumbnail === ''\" class=\"bg-icon\">\r\n                                <md-icon>star</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title\" [ngClass]=\"{ show: a.thumbnail === null || a.thumbnail === '' }\">\r\n                            <span>{{a.name}}</span>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"tile config\" *ngIf=\"showAdvanced && !isContentApp\" (click)=\"frame.run('app-import')\" [attr.title]=\"'TemplatePicker.Install' | translate\">\r\n                        <div class=\"bg\">\r\n                            <div class=\"bg-icon\">\r\n                                <md-icon>get_app</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title show\">\r\n                            <span>{{\"TemplatePicker.Install\" | translate}}</span>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"tile config\" *ngIf=\"showAdvanced && !isContentApp\" (click)=\"frame.run('zone')\" [attr.title]=\"'TemplatePicker.Zone' | translate\">\r\n                        <div class=\"bg\">\r\n                            <div class=\"bg-icon\">\r\n                                <md-icon>apps</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title show\">\r\n                            <span>{{\"TemplatePicker.Zone\" | translate}}</span>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n                <ng-template #contentApp>\r\n                    <div class=\"tiles\">\r\n                        <div class=\"tile\" [ngClass]=\"{ active: contentType?.StaticName === c.StaticName, blocked: !allowContentTypeChange }\" [attr.title]=\"c.Label\"\r\n                            (click)=\"contentType?.StaticName === c.StaticName ? switchTab() : updateContentType(c)\" (dblclick)=\"switchTab()\"\r\n                            *ngFor=\"let c of contentTypes\">\r\n                            <div class=\"bg\">\r\n                                <img *ngIf=\"c.Thumbnail !== null && c.Thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"c.Thumbnail + '?w=176&h=176'\">\r\n                                <div *ngIf=\"c.Thumbnail === null || c.Thumbnail === ''\" class=\"bg-icon\">\r\n                                    <md-icon>bubble_chart</md-icon>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"title\" [ngClass]=\"{ show: c.Thumbnail === null || c.Thumbnail === '' }\">\r\n                                <span>{{c.Label}}</span>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                </ng-template>\r\n            </md-tab>\r\n            <md-tab *ngIf=\"isContentApp ? contentType : app\" [label]=\"('TemplatePicker.ChangeView' | translate) + '(' + templates.length + ')'\">\r\n                <div class=\"tiles\">\r\n                    <md-spinner class=\"templates-spinner\" *ngIf=\"loadingTemplates\"></md-spinner>\r\n                    <div class=\"tile\" [ngClass]=\"{ active: template?.TemplateId === t.TemplateId }\" [attr.title]=\"t.Name\" (click)=\"updateTemplateSubject.next({ template: t })\"\r\n                        *ngFor=\"let t of templates\">\r\n                        <div class=\"bg\">\r\n                            <img *ngIf=\"t.Thumbnail !== null && t.Thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"t.Thumbnail + '?w=176&h=176'\">\r\n                            <div *ngIf=\"t.Thumbnail === null || t.Thumbnail === ''\" class=\"bg-icon\">\r\n                                <md-icon *ngIf=\"isContentApp\">view_carousel</md-icon>\r\n                                <md-icon *ngIf=\"!isContentApp\">view_quilt</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title\" [ngClass]=\"{ show: t.Thumbnail === null || t.Thumbnail === '' }\">\r\n                            <span>{{t.Name}}</span>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"tile config\" *ngIf=\"showAdvanced && !isContentApp && app?.appId !== null\" (click)=\"frame.run('app')\"\r\n                        [attr.title]=\"'TemplatePicker.App' | translate\">\r\n                        <div class=\"bg\">\r\n                            <div class=\"bg-icon\">\r\n                                <md-icon>settings</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title show\">\r\n                            <span>{{\"TemplatePicker.App\" | translate}}</span>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </md-tab>\r\n        </md-tab-group>\r\n        <app-installer *ngIf=\"showInstaller\" [isContentApp]=\"isContentApp\"></app-installer>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "../../../../../src/app/template-picker/template-picker.component.scss":
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__("../../../../css-loader/lib/css-base.js")(false);
// imports


// module
exports.push([module.i, "/**\n * Applies styles for users in high contrast mode. Note that this only applies\n * to Microsoft browsers. Chrome can be included by checking for the `html[hc]`\n * attribute, however Chrome handles high contrast differently.\n */\n/* Theme for the ripple elements.*/\n/** The mixins below are shared between md-menu and md-select */\n/**\n * This mixin adds the correct panel transform styles based\n * on the direction that the menu panel opens.\n */\n/* stylelint-disable material/no-prefixes */\n/* stylelint-enable */\n/**\n * This mixin contains shared option styles between the select and\n * autocomplete components.\n */\n.mat-elevation-z0 {\n  box-shadow: 0px 0px 0px 0px rgba(0, 0, 0, 0.2), 0px 0px 0px 0px rgba(0, 0, 0, 0.14), 0px 0px 0px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z1 {\n  box-shadow: 0px 2px 1px -1px rgba(0, 0, 0, 0.2), 0px 1px 1px 0px rgba(0, 0, 0, 0.14), 0px 1px 3px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z2 {\n  box-shadow: 0px 3px 1px -2px rgba(0, 0, 0, 0.2), 0px 2px 2px 0px rgba(0, 0, 0, 0.14), 0px 1px 5px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z3 {\n  box-shadow: 0px 3px 3px -2px rgba(0, 0, 0, 0.2), 0px 3px 4px 0px rgba(0, 0, 0, 0.14), 0px 1px 8px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z4 {\n  box-shadow: 0px 2px 4px -1px rgba(0, 0, 0, 0.2), 0px 4px 5px 0px rgba(0, 0, 0, 0.14), 0px 1px 10px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z5 {\n  box-shadow: 0px 3px 5px -1px rgba(0, 0, 0, 0.2), 0px 5px 8px 0px rgba(0, 0, 0, 0.14), 0px 1px 14px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z6 {\n  box-shadow: 0px 3px 5px -1px rgba(0, 0, 0, 0.2), 0px 6px 10px 0px rgba(0, 0, 0, 0.14), 0px 1px 18px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z7 {\n  box-shadow: 0px 4px 5px -2px rgba(0, 0, 0, 0.2), 0px 7px 10px 1px rgba(0, 0, 0, 0.14), 0px 2px 16px 1px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z8 {\n  box-shadow: 0px 5px 5px -3px rgba(0, 0, 0, 0.2), 0px 8px 10px 1px rgba(0, 0, 0, 0.14), 0px 3px 14px 2px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z9 {\n  box-shadow: 0px 5px 6px -3px rgba(0, 0, 0, 0.2), 0px 9px 12px 1px rgba(0, 0, 0, 0.14), 0px 3px 16px 2px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z10 {\n  box-shadow: 0px 6px 6px -3px rgba(0, 0, 0, 0.2), 0px 10px 14px 1px rgba(0, 0, 0, 0.14), 0px 4px 18px 3px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z11 {\n  box-shadow: 0px 6px 7px -4px rgba(0, 0, 0, 0.2), 0px 11px 15px 1px rgba(0, 0, 0, 0.14), 0px 4px 20px 3px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z12 {\n  box-shadow: 0px 7px 8px -4px rgba(0, 0, 0, 0.2), 0px 12px 17px 2px rgba(0, 0, 0, 0.14), 0px 5px 22px 4px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z13 {\n  box-shadow: 0px 7px 8px -4px rgba(0, 0, 0, 0.2), 0px 13px 19px 2px rgba(0, 0, 0, 0.14), 0px 5px 24px 4px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z14 {\n  box-shadow: 0px 7px 9px -4px rgba(0, 0, 0, 0.2), 0px 14px 21px 2px rgba(0, 0, 0, 0.14), 0px 5px 26px 4px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z15 {\n  box-shadow: 0px 8px 9px -5px rgba(0, 0, 0, 0.2), 0px 15px 22px 2px rgba(0, 0, 0, 0.14), 0px 6px 28px 5px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z16 {\n  box-shadow: 0px 8px 10px -5px rgba(0, 0, 0, 0.2), 0px 16px 24px 2px rgba(0, 0, 0, 0.14), 0px 6px 30px 5px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z17 {\n  box-shadow: 0px 8px 11px -5px rgba(0, 0, 0, 0.2), 0px 17px 26px 2px rgba(0, 0, 0, 0.14), 0px 6px 32px 5px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z18 {\n  box-shadow: 0px 9px 11px -5px rgba(0, 0, 0, 0.2), 0px 18px 28px 2px rgba(0, 0, 0, 0.14), 0px 7px 34px 6px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z19 {\n  box-shadow: 0px 9px 12px -6px rgba(0, 0, 0, 0.2), 0px 19px 29px 2px rgba(0, 0, 0, 0.14), 0px 7px 36px 6px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z20 {\n  box-shadow: 0px 10px 13px -6px rgba(0, 0, 0, 0.2), 0px 20px 31px 3px rgba(0, 0, 0, 0.14), 0px 8px 38px 7px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z21 {\n  box-shadow: 0px 10px 13px -6px rgba(0, 0, 0, 0.2), 0px 21px 33px 3px rgba(0, 0, 0, 0.14), 0px 8px 40px 7px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z22 {\n  box-shadow: 0px 10px 14px -6px rgba(0, 0, 0, 0.2), 0px 22px 35px 3px rgba(0, 0, 0, 0.14), 0px 8px 42px 7px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z23 {\n  box-shadow: 0px 11px 14px -7px rgba(0, 0, 0, 0.2), 0px 23px 36px 3px rgba(0, 0, 0, 0.14), 0px 9px 44px 8px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z24 {\n  box-shadow: 0px 11px 15px -7px rgba(0, 0, 0, 0.2), 0px 24px 38px 3px rgba(0, 0, 0, 0.14), 0px 9px 46px 8px rgba(0, 0, 0, 0.12); }\n\n.mat-h1, .mat-headline, .mat-typography h1 {\n  font: 400 24px/32px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 16px; }\n\n.mat-h2, .mat-title, .mat-typography h2 {\n  font: 500 20px/32px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 16px; }\n\n.mat-h3, .mat-subheading-2, .mat-typography h3 {\n  font: 400 16px/28px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 16px; }\n\n.mat-h4, .mat-subheading-1, .mat-typography h4 {\n  font: 400 15px/24px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 16px; }\n\n.mat-h5, .mat-typography h5 {\n  font-size: 11.62px;\n  font-weight: 400;\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  line-height: 20px;\n  margin: 0 0 12px; }\n\n.mat-h6, .mat-typography h6 {\n  font-size: 9.38px;\n  font-weight: 400;\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  line-height: 20px;\n  margin: 0 0 12px; }\n\n.mat-body-strong, .mat-body-2 {\n  font: 500 14px/24px Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-body, .mat-body-1, .mat-typography {\n  font: 400 14px/20px Roboto, \"Helvetica Neue\", sans-serif; }\n  .mat-body p, .mat-body-1 p, .mat-typography p {\n    margin: 0 0 12px; }\n\n.mat-small, .mat-caption {\n  font: 400 12px/20px Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-display-4, .mat-typography .mat-display-4 {\n  font: 300 112px/112px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 56px;\n  letter-spacing: -0.05em; }\n\n.mat-display-3, .mat-typography .mat-display-3 {\n  font: 400 56px/56px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 64px;\n  letter-spacing: -0.02em; }\n\n.mat-display-2, .mat-typography .mat-display-2 {\n  font: 400 45px/48px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 64px;\n  letter-spacing: -0.005em; }\n\n.mat-display-1, .mat-typography .mat-display-1 {\n  font: 400 34px/40px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0 0 64px; }\n\n.mat-button, .mat-raised-button, .mat-icon-button {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 14px;\n  font-weight: 500; }\n\n.mat-button-toggle {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-card {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-card-title {\n  font-size: 24px;\n  font-weight: 400; }\n\n.mat-card-subtitle,\n.mat-card-content,\n.mat-card-header .mat-card-title {\n  font-size: 14px; }\n\n.mat-checkbox {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-checkbox-layout .mat-checkbox-label {\n  line-height: 24px; }\n\n.mat-chip:not(.mat-basic-chip) {\n  font-size: 13px;\n  line-height: 16px; }\n\n.mat-header-cell {\n  font-size: 12px;\n  font-weight: 500; }\n\n.mat-cell {\n  font-size: 14px; }\n\n.mat-calendar {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-calendar-body {\n  font-size: 13px; }\n\n.mat-calendar-body-label,\n.mat-calendar-period-button {\n  font-size: 14px;\n  font-weight: 500; }\n\n.mat-calendar-table-header th {\n  font-size: 11px;\n  font-weight: 400; }\n\n.mat-dialog-title {\n  font: 500 20px/32px Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-grid-tile-header,\n.mat-grid-tile-footer {\n  font-size: 14px; }\n  .mat-grid-tile-header .mat-line,\n  .mat-grid-tile-footer .mat-line {\n    white-space: nowrap;\n    overflow: hidden;\n    text-overflow: ellipsis;\n    display: block;\n    box-sizing: border-box; }\n    .mat-grid-tile-header .mat-line:nth-child(n+2),\n    .mat-grid-tile-footer .mat-line:nth-child(n+2) {\n      font-size: 12px; }\n\n.mat-input-container {\n  font: 400 inherit/1.125 Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-input-wrapper {\n  padding-bottom: 1.296875em; }\n\n.mat-input-prefix .mat-icon,\n.mat-input-prefix .mat-datepicker-toggle,\n.mat-input-suffix .mat-icon,\n.mat-input-suffix .mat-datepicker-toggle {\n  font-size: 150%; }\n\n.mat-input-prefix .mat-icon-button,\n.mat-input-suffix .mat-icon-button {\n  height: 1.5em;\n  width: 1.5em; }\n  .mat-input-prefix .mat-icon-button .mat-icon,\n  .mat-input-suffix .mat-icon-button .mat-icon {\n    line-height: 1.5; }\n\n.mat-input-infix {\n  padding: 0.4375em 0;\n  border-top: 0.84375em solid transparent; }\n\n.mat-input-element:-webkit-autofill + .mat-input-placeholder-wrapper .mat-float {\n  -webkit-transform: translateY(-1.28125em) scale(0.75) perspective(100px) translateZ(0.001px);\n          transform: translateY(-1.28125em) scale(0.75) perspective(100px) translateZ(0.001px);\n  -ms-transform: translateY(-1.28125em) scale(0.75);\n  width: 133.33333333%; }\n\n.mat-input-placeholder-wrapper {\n  top: -0.84375em;\n  padding-top: 0.84375em; }\n\n.mat-input-placeholder {\n  top: 1.28125em; }\n  .mat-input-placeholder.mat-float:not(.mat-empty), .mat-focused .mat-input-placeholder.mat-float {\n    -webkit-transform: translateY(-1.28125em) scale(0.75) perspective(100px) translateZ(0.001px);\n            transform: translateY(-1.28125em) scale(0.75) perspective(100px) translateZ(0.001px);\n    -ms-transform: translateY(-1.28125em) scale(0.75);\n    width: 133.33333333%; }\n\n.mat-input-underline {\n  bottom: 1.296875em; }\n\n.mat-input-subscript-wrapper {\n  font-size: 75%;\n  margin-top: 0.60416667em;\n  top: calc(100% - 1.72916667em); }\n\n.mat-menu-item {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 16px; }\n\n.mat-paginator {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 12px; }\n\n.mat-paginator-page-size .mat-select-trigger {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 12px; }\n\n.mat-radio-button {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-select {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-select-trigger {\n  font-size: 16px; }\n\n.mat-slide-toggle-content {\n  font: 400 14px/20px Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-slider-thumb-label-text {\n  font-size: 12px;\n  font-weight: 500; }\n\n.mat-tab-group {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-tab-label, .mat-tab-link {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 14px;\n  font-weight: 500; }\n\n.mat-toolbar,\n.mat-toolbar h1,\n.mat-toolbar h2,\n.mat-toolbar h3,\n.mat-toolbar h4,\n.mat-toolbar h5,\n.mat-toolbar h6 {\n  font: 500 20px/32px Roboto, \"Helvetica Neue\", sans-serif;\n  margin: 0; }\n\n.mat-tooltip {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 10px;\n  padding-top: 6px;\n  padding-bottom: 6px; }\n\n.mat-list-item {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-list .mat-list-item, .mat-nav-list .mat-list-item {\n  font-size: 16px; }\n  .mat-list .mat-list-item .mat-line, .mat-nav-list .mat-list-item .mat-line {\n    white-space: nowrap;\n    overflow: hidden;\n    text-overflow: ellipsis;\n    display: block;\n    box-sizing: border-box; }\n    .mat-list .mat-list-item .mat-line:nth-child(n+2), .mat-nav-list .mat-list-item .mat-line:nth-child(n+2) {\n      font-size: 14px; }\n\n.mat-list .mat-subheader, .mat-nav-list .mat-subheader {\n  font: 500 14px/24px Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-list[dense] .mat-list-item, .mat-nav-list[dense] .mat-list-item {\n  font-size: 12px; }\n  .mat-list[dense] .mat-list-item .mat-line, .mat-nav-list[dense] .mat-list-item .mat-line {\n    white-space: nowrap;\n    overflow: hidden;\n    text-overflow: ellipsis;\n    display: block;\n    box-sizing: border-box; }\n    .mat-list[dense] .mat-list-item .mat-line:nth-child(n+2), .mat-nav-list[dense] .mat-list-item .mat-line:nth-child(n+2) {\n      font-size: 12px; }\n\n.mat-list[dense] .mat-subheader, .mat-nav-list[dense] .mat-subheader {\n  font: 500 12px Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-option {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 16px; }\n\n.mat-optgroup-label {\n  font: 500 14px/24px Roboto, \"Helvetica Neue\", sans-serif; }\n\n.mat-simple-snackbar {\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  font-size: 14px; }\n\n.mat-simple-snackbar-action {\n  line-height: 1;\n  font-family: inherit;\n  font-size: inherit;\n  font-weight: 500; }\n\n.mat-ripple {\n  overflow: hidden; }\n\n.mat-ripple.mat-ripple-unbounded {\n  overflow: visible; }\n\n.mat-ripple-element {\n  position: absolute;\n  border-radius: 50%;\n  pointer-events: none;\n  transition: opacity, -webkit-transform 0ms cubic-bezier(0, 0, 0.2, 1);\n  transition: opacity, transform 0ms cubic-bezier(0, 0, 0.2, 1);\n  transition: opacity, transform 0ms cubic-bezier(0, 0, 0.2, 1), -webkit-transform 0ms cubic-bezier(0, 0, 0.2, 1);\n  -webkit-transform: scale(0);\n          transform: scale(0); }\n\n.mat-option {\n  white-space: nowrap;\n  overflow: hidden;\n  text-overflow: ellipsis;\n  display: block;\n  line-height: 48px;\n  height: 48px;\n  padding: 0 16px;\n  text-align: left;\n  text-decoration: none;\n  position: relative;\n  cursor: pointer;\n  outline: none; }\n  .mat-option[disabled] {\n    cursor: default; }\n  [dir='rtl'] .mat-option {\n    text-align: right; }\n  .mat-option .mat-icon {\n    margin-right: 16px; }\n    [dir='rtl'] .mat-option .mat-icon {\n      margin-left: 16px;\n      margin-right: 0; }\n  .mat-option[aria-disabled='true'] {\n    -webkit-user-select: none;\n    -moz-user-select: none;\n    -ms-user-select: none;\n    user-select: none;\n    cursor: default; }\n  .mat-optgroup .mat-option:not(.mat-option-multiple) {\n    padding-left: 32px; }\n    [dir='rtl'] .mat-optgroup .mat-option:not(.mat-option-multiple) {\n      padding-left: 16px;\n      padding-right: 32px; }\n\n.mat-option-ripple {\n  position: absolute;\n  top: 0;\n  left: 0;\n  bottom: 0;\n  right: 0;\n  pointer-events: none; }\n  @media screen and (-ms-high-contrast: active) {\n    .mat-option-ripple {\n      opacity: 0.5; } }\n\n.mat-option-pseudo-checkbox {\n  margin-right: 8px; }\n  [dir='rtl'] .mat-option-pseudo-checkbox {\n    margin-left: 8px;\n    margin-right: 0; }\n\n.mat-optgroup-label {\n  white-space: nowrap;\n  overflow: hidden;\n  text-overflow: ellipsis;\n  display: block;\n  line-height: 48px;\n  height: 48px;\n  padding: 0 16px;\n  text-align: left;\n  text-decoration: none;\n  -webkit-user-select: none;\n  -moz-user-select: none;\n  -ms-user-select: none;\n  user-select: none;\n  cursor: default; }\n  .mat-optgroup-label[disabled] {\n    cursor: default; }\n  [dir='rtl'] .mat-optgroup-label {\n    text-align: right; }\n  .mat-optgroup-label .mat-icon {\n    margin-right: 16px; }\n    [dir='rtl'] .mat-optgroup-label .mat-icon {\n      margin-left: 16px;\n      margin-right: 0; }\n\n.cdk-visually-hidden {\n  border: 0;\n  clip: rect(0 0 0 0);\n  height: 1px;\n  margin: -1px;\n  overflow: hidden;\n  padding: 0;\n  position: absolute;\n  text-transform: none;\n  width: 1px; }\n\n.cdk-overlay-container, .cdk-global-overlay-wrapper {\n  pointer-events: none;\n  top: 0;\n  left: 0;\n  height: 100%;\n  width: 100%; }\n\n.cdk-overlay-container {\n  position: fixed;\n  z-index: 1000; }\n\n.cdk-global-overlay-wrapper {\n  display: -webkit-box;\n  display: -ms-flexbox;\n  display: flex;\n  position: absolute;\n  z-index: 1000; }\n\n.cdk-overlay-pane {\n  position: absolute;\n  pointer-events: auto;\n  box-sizing: border-box;\n  z-index: 1000; }\n\n.cdk-overlay-backdrop {\n  position: absolute;\n  top: 0;\n  bottom: 0;\n  left: 0;\n  right: 0;\n  z-index: 1000;\n  pointer-events: auto;\n  -webkit-tap-highlight-color: transparent;\n  transition: opacity 400ms cubic-bezier(0.25, 0.8, 0.25, 1);\n  opacity: 0; }\n  .cdk-overlay-backdrop.cdk-overlay-backdrop-showing {\n    opacity: 0.48; }\n\n.cdk-overlay-dark-backdrop {\n  background: rgba(0, 0, 0, 0.6); }\n\n.cdk-overlay-transparent-backdrop {\n  background: none; }\n\n.cdk-global-scrollblock {\n  position: fixed;\n  width: 100%;\n  overflow-y: scroll; }\n\n.mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.1); }\n\n.mat-option {\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-option:hover:not(.mat-option-disabled), .mat-option:focus:not(.mat-option-disabled) {\n    background: rgba(0, 0, 0, 0.04); }\n  .mat-option.mat-selected.mat-primary, .mat-primary .mat-option.mat-selected {\n    color: #00838f; }\n  .mat-option.mat-selected.mat-accent, .mat-accent .mat-option.mat-selected {\n    color: #039be5; }\n  .mat-option.mat-selected.mat-warn, .mat-warn .mat-option.mat-selected {\n    color: #e53935; }\n  .mat-option.mat-selected:not(.mat-option-multiple) {\n    background: rgba(0, 0, 0, 0.04); }\n  .mat-option.mat-active {\n    background: rgba(0, 0, 0, 0.04);\n    color: rgba(0, 0, 0, 0.87); }\n  .mat-option.mat-option-disabled {\n    color: rgba(0, 0, 0, 0.38); }\n\n.mat-optgroup-label {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-optgroup-disabled .mat-optgroup-label {\n  color: rgba(0, 0, 0, 0.38); }\n\n.mat-pseudo-checkbox {\n  color: rgba(0, 0, 0, 0.54); }\n  .mat-pseudo-checkbox::after {\n    color: #fafafa; }\n\n.mat-pseudo-checkbox-checked.mat-primary,\n.mat-pseudo-checkbox-indeterminate.mat-primary,\n.mat-primary .mat-pseudo-checkbox-checked,\n.mat-primary .mat-pseudo-checkbox-indeterminate {\n  background: #00838f; }\n\n.mat-pseudo-checkbox-checked.mat-accent,\n.mat-pseudo-checkbox-indeterminate.mat-accent,\n.mat-accent .mat-pseudo-checkbox-checked,\n.mat-accent .mat-pseudo-checkbox-indeterminate {\n  background: #039be5; }\n\n.mat-pseudo-checkbox-checked.mat-warn,\n.mat-pseudo-checkbox-indeterminate.mat-warn,\n.mat-warn .mat-pseudo-checkbox-checked,\n.mat-warn .mat-pseudo-checkbox-indeterminate {\n  background: #e53935; }\n\n.mat-pseudo-checkbox-checked.mat-pseudo-checkbox-disabled, .mat-pseudo-checkbox-indeterminate.mat-pseudo-checkbox-disabled {\n  background: #b0b0b0; }\n\n.mat-app-background {\n  background-color: #fafafa; }\n\n.mat-theme-loaded-marker {\n  display: none; }\n\n.mat-autocomplete-panel {\n  background: white;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-autocomplete-panel .mat-option.mat-selected:not(.mat-active):not(:hover) {\n    background: white;\n    color: rgba(0, 0, 0, 0.87); }\n\n.mat-button, .mat-icon-button {\n  background: transparent; }\n  .mat-button.mat-primary .mat-button-focus-overlay, .mat-icon-button.mat-primary .mat-button-focus-overlay {\n    background-color: rgba(0, 131, 143, 0.12); }\n  .mat-button.mat-accent .mat-button-focus-overlay, .mat-icon-button.mat-accent .mat-button-focus-overlay {\n    background-color: rgba(3, 155, 229, 0.12); }\n  .mat-button.mat-warn .mat-button-focus-overlay, .mat-icon-button.mat-warn .mat-button-focus-overlay {\n    background-color: rgba(229, 57, 53, 0.12); }\n  .mat-button[disabled] .mat-button-focus-overlay, .mat-icon-button[disabled] .mat-button-focus-overlay {\n    background-color: transparent; }\n  .mat-button.mat-primary, .mat-icon-button.mat-primary {\n    color: #00838f; }\n  .mat-button.mat-accent, .mat-icon-button.mat-accent {\n    color: #039be5; }\n  .mat-button.mat-warn, .mat-icon-button.mat-warn {\n    color: #e53935; }\n  .mat-button.mat-primary[disabled], .mat-button.mat-accent[disabled], .mat-button.mat-warn[disabled], .mat-button[disabled][disabled], .mat-icon-button.mat-primary[disabled], .mat-icon-button.mat-accent[disabled], .mat-icon-button.mat-warn[disabled], .mat-icon-button[disabled][disabled] {\n    color: rgba(0, 0, 0, 0.38); }\n\n.mat-raised-button, .mat-fab, .mat-mini-fab {\n  color: rgba(0, 0, 0, 0.87);\n  background-color: white; }\n  .mat-raised-button.mat-primary, .mat-fab.mat-primary, .mat-mini-fab.mat-primary {\n    color: white; }\n  .mat-raised-button.mat-accent, .mat-fab.mat-accent, .mat-mini-fab.mat-accent {\n    color: white; }\n  .mat-raised-button.mat-warn, .mat-fab.mat-warn, .mat-mini-fab.mat-warn {\n    color: white; }\n  .mat-raised-button.mat-primary[disabled], .mat-raised-button.mat-accent[disabled], .mat-raised-button.mat-warn[disabled], .mat-raised-button[disabled][disabled], .mat-fab.mat-primary[disabled], .mat-fab.mat-accent[disabled], .mat-fab.mat-warn[disabled], .mat-fab[disabled][disabled], .mat-mini-fab.mat-primary[disabled], .mat-mini-fab.mat-accent[disabled], .mat-mini-fab.mat-warn[disabled], .mat-mini-fab[disabled][disabled] {\n    color: rgba(0, 0, 0, 0.38); }\n  .mat-raised-button.mat-primary, .mat-fab.mat-primary, .mat-mini-fab.mat-primary {\n    background-color: #00838f; }\n  .mat-raised-button.mat-accent, .mat-fab.mat-accent, .mat-mini-fab.mat-accent {\n    background-color: #039be5; }\n  .mat-raised-button.mat-warn, .mat-fab.mat-warn, .mat-mini-fab.mat-warn {\n    background-color: #e53935; }\n  .mat-raised-button.mat-primary[disabled], .mat-raised-button.mat-accent[disabled], .mat-raised-button.mat-warn[disabled], .mat-raised-button[disabled][disabled], .mat-fab.mat-primary[disabled], .mat-fab.mat-accent[disabled], .mat-fab.mat-warn[disabled], .mat-fab[disabled][disabled], .mat-mini-fab.mat-primary[disabled], .mat-mini-fab.mat-accent[disabled], .mat-mini-fab.mat-warn[disabled], .mat-mini-fab[disabled][disabled] {\n    background-color: rgba(0, 0, 0, 0.12); }\n  .mat-raised-button.mat-primary .mat-ripple-element, .mat-fab.mat-primary .mat-ripple-element, .mat-mini-fab.mat-primary .mat-ripple-element {\n    background-color: rgba(255, 255, 255, 0.2); }\n  .mat-raised-button.mat-accent .mat-ripple-element, .mat-fab.mat-accent .mat-ripple-element, .mat-mini-fab.mat-accent .mat-ripple-element {\n    background-color: rgba(255, 255, 255, 0.2); }\n  .mat-raised-button.mat-warn .mat-ripple-element, .mat-fab.mat-warn .mat-ripple-element, .mat-mini-fab.mat-warn .mat-ripple-element {\n    background-color: rgba(255, 255, 255, 0.2); }\n\n.mat-button.mat-primary .mat-ripple-element {\n  background-color: rgba(0, 131, 143, 0.1); }\n\n.mat-button.mat-accent .mat-ripple-element {\n  background-color: rgba(3, 155, 229, 0.1); }\n\n.mat-button.mat-warn .mat-ripple-element {\n  background-color: rgba(229, 57, 53, 0.1); }\n\n.mat-icon-button.mat-primary .mat-ripple-element {\n  background-color: rgba(0, 131, 143, 0.2); }\n\n.mat-icon-button.mat-accent .mat-ripple-element {\n  background-color: rgba(3, 155, 229, 0.2); }\n\n.mat-icon-button.mat-warn .mat-ripple-element {\n  background-color: rgba(229, 57, 53, 0.2); }\n\n.mat-button-toggle {\n  color: rgba(0, 0, 0, 0.38); }\n  .mat-button-toggle.cdk-focused .mat-button-toggle-focus-overlay {\n    background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-button-toggle-checked {\n  background-color: #e0e0e0;\n  color: black; }\n\n.mat-button-toggle-disabled {\n  background-color: #eeeeee;\n  color: rgba(0, 0, 0, 0.38); }\n  .mat-button-toggle-disabled.mat-button-toggle-checked {\n    background-color: #bdbdbd; }\n\n.mat-card {\n  background: white;\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-card-subtitle {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-checkbox-frame {\n  border-color: rgba(0, 0, 0, 0.54); }\n\n.mat-checkbox-checkmark {\n  fill: #fafafa; }\n\n.mat-checkbox-checkmark-path {\n  stroke: #fafafa !important; }\n\n.mat-checkbox-mixedmark {\n  background-color: #fafafa; }\n\n.mat-checkbox-indeterminate.mat-primary .mat-checkbox-background, .mat-checkbox-checked.mat-primary .mat-checkbox-background {\n  background-color: #00838f; }\n\n.mat-checkbox-indeterminate.mat-accent .mat-checkbox-background, .mat-checkbox-checked.mat-accent .mat-checkbox-background {\n  background-color: #039be5; }\n\n.mat-checkbox-indeterminate.mat-warn .mat-checkbox-background, .mat-checkbox-checked.mat-warn .mat-checkbox-background {\n  background-color: #e53935; }\n\n.mat-checkbox-disabled.mat-checkbox-checked .mat-checkbox-background, .mat-checkbox-disabled.mat-checkbox-indeterminate .mat-checkbox-background {\n  background-color: #b0b0b0; }\n\n.mat-checkbox-disabled:not(.mat-checkbox-checked) .mat-checkbox-frame {\n  border-color: #b0b0b0; }\n\n.mat-checkbox-disabled .mat-checkbox-label {\n  color: #b0b0b0; }\n\n.mat-checkbox:not(.mat-checkbox-disabled).mat-primary .mat-checkbox-ripple .mat-ripple-element {\n  background-color: rgba(0, 131, 143, 0.26); }\n\n.mat-checkbox:not(.mat-checkbox-disabled).mat-accent .mat-checkbox-ripple .mat-ripple-element {\n  background-color: rgba(3, 155, 229, 0.26); }\n\n.mat-checkbox:not(.mat-checkbox-disabled).mat-warn .mat-checkbox-ripple .mat-ripple-element {\n  background-color: rgba(229, 57, 53, 0.26); }\n\n.mat-chip:not(.mat-basic-chip) {\n  background-color: #e0e0e0;\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-chip.mat-chip-selected:not(.mat-basic-chip) {\n  background-color: #808080;\n  color: rgba(255, 255, 255, 0.87); }\n  .mat-chip.mat-chip-selected:not(.mat-basic-chip).mat-primary {\n    background-color: #00838f;\n    color: white; }\n  .mat-chip.mat-chip-selected:not(.mat-basic-chip).mat-accent {\n    background-color: #039be5;\n    color: white; }\n  .mat-chip.mat-chip-selected:not(.mat-basic-chip).mat-warn {\n    background-color: #e53935;\n    color: white; }\n\n.mat-table {\n  background: white; }\n\n.mat-row, .mat-header-row {\n  border-bottom-color: rgba(0, 0, 0, 0.12); }\n\n.mat-header-cell {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-cell {\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-datepicker-content {\n  background-color: white;\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-calendar-arrow {\n  border-top-color: rgba(0, 0, 0, 0.54); }\n\n.mat-calendar-next-button,\n.mat-calendar-previous-button {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-calendar-table-header {\n  color: rgba(0, 0, 0, 0.38); }\n\n.mat-calendar-table-header-divider::after {\n  background: rgba(0, 0, 0, 0.12); }\n\n.mat-calendar-body-label {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-calendar-body-cell-content {\n  color: rgba(0, 0, 0, 0.87);\n  border-color: transparent; }\n  .mat-calendar-body-disabled > .mat-calendar-body-cell-content:not(.mat-calendar-body-selected) {\n    color: rgba(0, 0, 0, 0.38); }\n\n:not(.mat-calendar-body-disabled):hover > .mat-calendar-body-cell-content:not(.mat-calendar-body-selected),\n.cdk-keyboard-focused .mat-calendar-body-active > .mat-calendar-body-cell-content:not(.mat-calendar-body-selected) {\n  background-color: rgba(0, 0, 0, 0.04); }\n\n.mat-calendar-body-selected {\n  background-color: #00838f;\n  color: white; }\n\n.mat-calendar-body-disabled > .mat-calendar-body-selected {\n  background-color: rgba(0, 131, 143, 0.4); }\n\n.mat-calendar-body-today:not(.mat-calendar-body-selected) {\n  border-color: rgba(0, 0, 0, 0.38); }\n\n.mat-calendar-body-today.mat-calendar-body-selected {\n  box-shadow: inset 0 0 0 1px white; }\n\n.mat-calendar-body-disabled > .mat-calendar-body-today:not(.mat-calendar-body-selected) {\n  border-color: rgba(0, 0, 0, 0.18); }\n\n.mat-dialog-container {\n  background: white;\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-expansion-panel {\n  background: white;\n  color: black; }\n\n.mat-action-row {\n  border-top-color: rgba(0, 0, 0, 0.12); }\n\n.mat-expansion-panel-header:focus,\n.mat-expansion-panel-header:hover {\n  background: rgba(0, 0, 0, 0.04); }\n\n.mat-expansion-panel-header-title {\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-expansion-panel-header-description {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-expansion-indicator::after {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-icon.mat-primary {\n  color: #00838f; }\n\n.mat-icon.mat-accent {\n  color: #039be5; }\n\n.mat-icon.mat-warn {\n  color: #e53935; }\n\n.mat-input-placeholder {\n  color: rgba(0, 0, 0, 0.38); }\n\n.mat-focused .mat-input-placeholder {\n  color: #00838f; }\n  .mat-focused .mat-input-placeholder.mat-accent {\n    color: #039be5; }\n  .mat-focused .mat-input-placeholder.mat-warn {\n    color: #e53935; }\n\n.mat-input-element:disabled {\n  color: rgba(0, 0, 0, 0.38); }\n\ninput.mat-input-element:-webkit-autofill + .mat-input-placeholder .mat-placeholder-required,\n.mat-focused .mat-input-placeholder.mat-float .mat-placeholder-required {\n  color: #039be5; }\n\n.mat-input-underline {\n  background-color: rgba(0, 0, 0, 0.12); }\n\n.mat-input-ripple {\n  background-color: #00838f; }\n  .mat-input-ripple.mat-accent {\n    background-color: #039be5; }\n  .mat-input-ripple.mat-warn {\n    background-color: #e53935; }\n\n.mat-input-invalid .mat-input-placeholder {\n  color: #e53935; }\n  .mat-input-invalid .mat-input-placeholder.mat-accent,\n  .mat-input-invalid .mat-input-placeholder.mat-float .mat-placeholder-required {\n    color: #e53935; }\n\n.mat-input-invalid .mat-input-ripple {\n  background-color: #e53935; }\n\n.mat-input-error {\n  color: #e53935; }\n\n.mat-list .mat-list-item, .mat-nav-list .mat-list-item {\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-list .mat-subheader, .mat-nav-list .mat-subheader {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-divider {\n  border-top-color: rgba(0, 0, 0, 0.12); }\n\n.mat-nav-list .mat-list-item {\n  outline: none; }\n  .mat-nav-list .mat-list-item:hover, .mat-nav-list .mat-list-item.mat-list-item-focus {\n    background: rgba(0, 0, 0, 0.04); }\n\n.mat-menu-content {\n  background: white; }\n\n.mat-menu-item {\n  background: transparent;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-menu-item[disabled] {\n    color: rgba(0, 0, 0, 0.38); }\n  .mat-menu-item .mat-icon {\n    color: rgba(0, 0, 0, 0.54);\n    vertical-align: middle; }\n  .mat-menu-item:hover:not([disabled]), .mat-menu-item:focus:not([disabled]) {\n    background: rgba(0, 0, 0, 0.04); }\n\n.mat-paginator {\n  background: white; }\n\n.mat-paginator,\n.mat-paginator-page-size .mat-select-trigger {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-paginator-increment,\n.mat-paginator-decrement {\n  border-top: 2px solid rgba(0, 0, 0, 0.54);\n  border-right: 2px solid rgba(0, 0, 0, 0.54); }\n\n.mat-icon-button[disabled] .mat-paginator-increment,\n.mat-icon-button[disabled] .mat-paginator-decrement {\n  border-color: rgba(0, 0, 0, 0.38); }\n\n.mat-progress-bar-background {\n  background-image: url(\"data:image/svg+xml;charset=UTF-8,%3Csvg%20version%3D%271.1%27%20xmlns%3D%27http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%27%20xmlns%3Axlink%3D%27http%3A%2F%2Fwww.w3.org%2F1999%2Fxlink%27%20x%3D%270px%27%20y%3D%270px%27%20enable-background%3D%27new%200%200%205%202%27%20xml%3Aspace%3D%27preserve%27%20viewBox%3D%270%200%205%202%27%20preserveAspectRatio%3D%27none%20slice%27%3E%3Ccircle%20cx%3D%271%27%20cy%3D%271%27%20r%3D%271%27%20fill%3D%27%23b2ebf2%27%2F%3E%3C%2Fsvg%3E\"); }\n\n.mat-progress-bar-buffer {\n  background-color: #b2ebf2; }\n\n.mat-progress-bar-fill::after {\n  background-color: #00838f; }\n\n.mat-progress-bar.mat-accent .mat-progress-bar-background {\n  background-image: url(\"data:image/svg+xml;charset=UTF-8,%3Csvg%20version%3D%271.1%27%20xmlns%3D%27http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%27%20xmlns%3Axlink%3D%27http%3A%2F%2Fwww.w3.org%2F1999%2Fxlink%27%20x%3D%270px%27%20y%3D%270px%27%20enable-background%3D%27new%200%200%205%202%27%20xml%3Aspace%3D%27preserve%27%20viewBox%3D%270%200%205%202%27%20preserveAspectRatio%3D%27none%20slice%27%3E%3Ccircle%20cx%3D%271%27%20cy%3D%271%27%20r%3D%271%27%20fill%3D%27%2380d8ff%27%2F%3E%3C%2Fsvg%3E\"); }\n\n.mat-progress-bar.mat-accent .mat-progress-bar-buffer {\n  background-color: #80d8ff; }\n\n.mat-progress-bar.mat-accent .mat-progress-bar-fill::after {\n  background-color: #039be5; }\n\n.mat-progress-bar.mat-warn .mat-progress-bar-background {\n  background-image: url(\"data:image/svg+xml;charset=UTF-8,%3Csvg%20version%3D%271.1%27%20xmlns%3D%27http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%27%20xmlns%3Axlink%3D%27http%3A%2F%2Fwww.w3.org%2F1999%2Fxlink%27%20x%3D%270px%27%20y%3D%270px%27%20enable-background%3D%27new%200%200%205%202%27%20xml%3Aspace%3D%27preserve%27%20viewBox%3D%270%200%205%202%27%20preserveAspectRatio%3D%27none%20slice%27%3E%3Ccircle%20cx%3D%271%27%20cy%3D%271%27%20r%3D%271%27%20fill%3D%27%23ffcdd2%27%2F%3E%3C%2Fsvg%3E\"); }\n\n.mat-progress-bar.mat-warn .mat-progress-bar-buffer {\n  background-color: #ffcdd2; }\n\n.mat-progress-bar.mat-warn .mat-progress-bar-fill::after {\n  background-color: #e53935; }\n\n.mat-progress-spinner path, .mat-spinner path {\n  stroke: #00838f; }\n\n.mat-progress-spinner.mat-accent path, .mat-spinner.mat-accent path {\n  stroke: #039be5; }\n\n.mat-progress-spinner.mat-warn path, .mat-spinner.mat-warn path {\n  stroke: #e53935; }\n\n.mat-radio-outer-circle {\n  border-color: rgba(0, 0, 0, 0.54); }\n\n.mat-radio-disabled .mat-radio-outer-circle {\n  border-color: rgba(0, 0, 0, 0.38); }\n\n.mat-radio-disabled .mat-radio-ripple .mat-ripple-element, .mat-radio-disabled .mat-radio-inner-circle {\n  background-color: rgba(0, 0, 0, 0.38); }\n\n.mat-radio-disabled .mat-radio-label-content {\n  color: rgba(0, 0, 0, 0.38); }\n\n.mat-radio-button.mat-primary.mat-radio-checked .mat-radio-outer-circle {\n  border-color: #00838f; }\n\n.mat-radio-button.mat-primary .mat-radio-inner-circle {\n  background-color: #00838f; }\n\n.mat-radio-button.mat-primary .mat-radio-ripple .mat-ripple-element {\n  background-color: rgba(0, 131, 143, 0.26); }\n\n.mat-radio-button.mat-accent.mat-radio-checked .mat-radio-outer-circle {\n  border-color: #039be5; }\n\n.mat-radio-button.mat-accent .mat-radio-inner-circle {\n  background-color: #039be5; }\n\n.mat-radio-button.mat-accent .mat-radio-ripple .mat-ripple-element {\n  background-color: rgba(3, 155, 229, 0.26); }\n\n.mat-radio-button.mat-warn.mat-radio-checked .mat-radio-outer-circle {\n  border-color: #e53935; }\n\n.mat-radio-button.mat-warn .mat-radio-inner-circle {\n  background-color: #e53935; }\n\n.mat-radio-button.mat-warn .mat-radio-ripple .mat-ripple-element {\n  background-color: rgba(229, 57, 53, 0.26); }\n\n.mat-select-trigger,\n.mat-select-arrow {\n  color: rgba(0, 0, 0, 0.38); }\n\n.mat-select-underline {\n  background-color: rgba(0, 0, 0, 0.12); }\n\n.mat-select-disabled .mat-select-value,\n.mat-select-arrow,\n.mat-select-trigger {\n  color: rgba(0, 0, 0, 0.38); }\n\n.mat-select-content, .mat-select-panel-done-animating {\n  background: white; }\n\n.mat-select-value {\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-select:focus:not(.mat-select-disabled).mat-primary .mat-select-trigger, .mat-select:focus:not(.mat-select-disabled).mat-primary .mat-select-arrow {\n  color: #00838f; }\n\n.mat-select:focus:not(.mat-select-disabled).mat-primary .mat-select-underline {\n  background-color: #00838f; }\n\n.mat-select:focus:not(.mat-select-disabled).mat-accent .mat-select-trigger, .mat-select:focus:not(.mat-select-disabled).mat-accent .mat-select-arrow {\n  color: #039be5; }\n\n.mat-select:focus:not(.mat-select-disabled).mat-accent .mat-select-underline {\n  background-color: #039be5; }\n\n.mat-select:focus:not(.mat-select-disabled).mat-warn .mat-select-trigger, .mat-select:focus:not(.mat-select-disabled).mat-warn .mat-select-arrow,\n.mat-select:not(:focus).ng-invalid.ng-touched:not(.mat-select-disabled) .mat-select-trigger,\n.mat-select:not(:focus).ng-invalid.ng-touched:not(.mat-select-disabled) .mat-select-arrow {\n  color: #e53935; }\n\n.mat-select:focus:not(.mat-select-disabled).mat-warn .mat-select-underline,\n.mat-select:not(:focus).ng-invalid.ng-touched:not(.mat-select-disabled) .mat-select-underline {\n  background-color: #e53935; }\n\n.mat-sidenav-container {\n  background-color: #fafafa;\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-sidenav {\n  background-color: white;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-sidenav.mat-sidenav-push {\n    background-color: white; }\n\n.mat-sidenav-backdrop.mat-sidenav-shown {\n  background-color: rgba(0, 0, 0, 0.6); }\n\n.mat-slide-toggle.mat-checked:not(.mat-disabled) .mat-slide-toggle-thumb {\n  background-color: #03a9f4; }\n\n.mat-slide-toggle.mat-checked:not(.mat-disabled) .mat-slide-toggle-bar {\n  background-color: rgba(3, 169, 244, 0.5); }\n\n.mat-slide-toggle:not(.mat-checked) .mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-slide-toggle .mat-ripple-element {\n  background-color: rgba(3, 169, 244, 0.12); }\n\n.mat-slide-toggle.mat-primary.mat-checked:not(.mat-disabled) .mat-slide-toggle-thumb {\n  background-color: #00bcd4; }\n\n.mat-slide-toggle.mat-primary.mat-checked:not(.mat-disabled) .mat-slide-toggle-bar {\n  background-color: rgba(0, 188, 212, 0.5); }\n\n.mat-slide-toggle.mat-primary:not(.mat-checked) .mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-slide-toggle.mat-primary .mat-ripple-element {\n  background-color: rgba(0, 188, 212, 0.12); }\n\n.mat-slide-toggle.mat-warn.mat-checked:not(.mat-disabled) .mat-slide-toggle-thumb {\n  background-color: #f44336; }\n\n.mat-slide-toggle.mat-warn.mat-checked:not(.mat-disabled) .mat-slide-toggle-bar {\n  background-color: rgba(244, 67, 54, 0.5); }\n\n.mat-slide-toggle.mat-warn:not(.mat-checked) .mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-slide-toggle.mat-warn .mat-ripple-element {\n  background-color: rgba(244, 67, 54, 0.12); }\n\n.mat-disabled .mat-slide-toggle-thumb {\n  background-color: #bdbdbd; }\n\n.mat-disabled .mat-slide-toggle-bar {\n  background-color: rgba(0, 0, 0, 0.1); }\n\n.mat-slide-toggle-thumb {\n  background-color: #fafafa; }\n\n.mat-slide-toggle-bar {\n  background-color: rgba(0, 0, 0, 0.38); }\n\n.mat-slider-track-background {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-primary .mat-slider-track-fill,\n.mat-primary .mat-slider-thumb,\n.mat-primary .mat-slider-thumb-label {\n  background-color: #00838f; }\n\n.mat-primary .mat-slider-thumb-label-text {\n  color: white; }\n\n.mat-accent .mat-slider-track-fill,\n.mat-accent .mat-slider-thumb,\n.mat-accent .mat-slider-thumb-label {\n  background-color: #039be5; }\n\n.mat-accent .mat-slider-thumb-label-text {\n  color: white; }\n\n.mat-warn .mat-slider-track-fill,\n.mat-warn .mat-slider-thumb,\n.mat-warn .mat-slider-thumb-label {\n  background-color: #e53935; }\n\n.mat-warn .mat-slider-thumb-label-text {\n  color: white; }\n\n.mat-slider-focus-ring {\n  background-color: rgba(3, 155, 229, 0.2); }\n\n.mat-slider:hover .mat-slider-track-background,\n.cdk-focused .mat-slider-track-background {\n  background-color: rgba(0, 0, 0, 0.38); }\n\n.mat-slider-disabled .mat-slider-track-background,\n.mat-slider-disabled .mat-slider-track-fill,\n.mat-slider-disabled .mat-slider-thumb {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-slider-disabled:hover .mat-slider-track-background {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-slider-min-value .mat-slider-focus-ring {\n  background-color: rgba(0, 0, 0, 0.12); }\n\n.mat-slider-min-value.mat-slider-thumb-label-showing .mat-slider-thumb,\n.mat-slider-min-value.mat-slider-thumb-label-showing .mat-slider-thumb-label {\n  background-color: black; }\n\n.mat-slider-min-value.mat-slider-thumb-label-showing.cdk-focused .mat-slider-thumb,\n.mat-slider-min-value.mat-slider-thumb-label-showing.cdk-focused .mat-slider-thumb-label {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-slider-min-value:not(.mat-slider-thumb-label-showing) .mat-slider-thumb {\n  border-color: rgba(0, 0, 0, 0.26);\n  background-color: transparent; }\n\n.mat-slider-min-value:not(.mat-slider-thumb-label-showing):hover .mat-slider-thumb, .mat-slider-min-value:not(.mat-slider-thumb-label-showing).cdk-focused .mat-slider-thumb {\n  border-color: rgba(0, 0, 0, 0.38); }\n\n.mat-slider-min-value:not(.mat-slider-thumb-label-showing):hover.mat-slider-disabled .mat-slider-thumb, .mat-slider-min-value:not(.mat-slider-thumb-label-showing).cdk-focused.mat-slider-disabled .mat-slider-thumb {\n  border-color: rgba(0, 0, 0, 0.26); }\n\n.mat-slider-has-ticks .mat-slider-wrapper::after {\n  border-color: rgba(0, 0, 0, 0.7); }\n\n.mat-slider-horizontal .mat-slider-ticks {\n  background-image: repeating-linear-gradient(to right, rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.7) 2px, transparent 0, transparent);\n  background-image: -moz-repeating-linear-gradient(0.0001deg, rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.7) 2px, transparent 0, transparent); }\n\n.mat-slider-vertical .mat-slider-ticks {\n  background-image: repeating-linear-gradient(to bottom, rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.7) 2px, transparent 0, transparent); }\n\n.mat-tab-nav-bar,\n.mat-tab-header {\n  border-bottom: 1px solid rgba(0, 0, 0, 0.12); }\n\n.mat-tab-group-inverted-header .mat-tab-nav-bar,\n.mat-tab-group-inverted-header .mat-tab-header {\n  border-top: 1px solid rgba(0, 0, 0, 0.12);\n  border-bottom: none; }\n\n.mat-tab-label:focus {\n  background-color: rgba(178, 235, 242, 0.3); }\n\n.mat-ink-bar {\n  background-color: #00838f; }\n\n.mat-tab-label, .mat-tab-link {\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-tab-label.mat-tab-disabled, .mat-tab-link.mat-tab-disabled {\n    color: rgba(0, 0, 0, 0.38); }\n\n.mat-toolbar {\n  background: whitesmoke;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-toolbar.mat-primary {\n    background: #00838f;\n    color: white; }\n  .mat-toolbar.mat-accent {\n    background: #039be5;\n    color: white; }\n  .mat-toolbar.mat-warn {\n    background: #e53935;\n    color: white; }\n\n.mat-tooltip {\n  background: rgba(97, 97, 97, 0.9); }\n\n:host {\n  display: block;\n  padding-top: 56px; }\n  :host .content {\n    background: #fafafa;\n    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.3);\n    position: relative; }\n    :host .content .card {\n      max-width: 1200px;\n      margin: 0 auto; }\n      :host .content .card .top-controls {\n        position: absolute;\n        height: 56px;\n        right: 4px;\n        top: -56px;\n        width: 100%;\n        text-align: center;\n        z-index: 2;\n        display: -webkit-box;\n        display: -ms-flexbox;\n        display: flex;\n        -webkit-box-orient: horizontal;\n        -webkit-box-direction: normal;\n            -ms-flex-direction: row;\n                flex-direction: row;\n        -webkit-box-align: center;\n            -ms-flex-align: center;\n                align-items: center; }\n        :host .content .card .top-controls button {\n          display: inline-block;\n          float: none;\n          position: relative;\n          top: 28px; }\n        :host .content .card .top-controls button.secondary {\n          background: #fafafa !important;\n          color: rgba(0, 0, 0, 0.8); }\n      :host .content .card .tiles {\n        white-space: nowrap;\n        overflow-x: auto; }\n        :host .content .card .tiles .tile {\n          box-sizing: border-box;\n          background: #fff;\n          display: inline-block;\n          margin: 12px 0 12px 12px;\n          position: relative;\n          font-size: 12px;\n          border-radius: 22px 0 22px 0;\n          height: 88px;\n          width: 88px;\n          line-height: 88px;\n          text-align: center;\n          overflow: hidden;\n          cursor: pointer;\n          color: rgba(0, 0, 0, 0.8);\n          transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);\n          box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24); }\n          :host .content .card .tiles .tile:hover {\n            box-shadow: 0 3px 6px rgba(0, 0, 0, 0.16), 0 3px 6px rgba(0, 0, 0, 0.23); }\n            :host .content .card .tiles .tile:hover .title {\n              opacity: 1; }\n              :host .content .card .tiles .tile:hover .title span {\n                -webkit-transform: translate(0, 0);\n                        transform: translate(0, 0); }\n          :host .content .card .tiles .tile:active, :host .content .card .tiles .tile.active {\n            box-shadow: 0 14px 28px rgba(0, 120, 220, 0.25), 0 10px 10px rgba(0, 120, 220, 0.22); }\n          :host .content .card .tiles .tile.active, :host .content .card .tiles .tile:focus {\n            color: #fff; }\n          :host .content .card .tiles .tile.config {\n            box-shadow: none;\n            border: 1px dashed rgba(0, 0, 0, 0.2);\n            background: transparent; }\n          :host .content .card .tiles .tile .bg {\n            position: absolute;\n            left: 0;\n            top: 0;\n            width: 100%;\n            height: 100%; }\n            :host .content .card .tiles .tile .bg img {\n              width: 100%;\n              height: 100%; }\n          :host .content .card .tiles .tile .title {\n            display: inline-block;\n            position: absolute;\n            box-sizing: border-box;\n            padding: 0 4px;\n            left: 0;\n            bottom: 0;\n            width: 100%;\n            height: 24px;\n            line-height: 24px;\n            background: white;\n            color: rgba(0, 0, 0, 0.8);\n            letter-spacing: .1pt;\n            font-size: 11px;\n            text-overflow: ellipsis;\n            overflow: hidden;\n            white-space: nowrap;\n            opacity: 0;\n            font-weight: bold;\n            transition: opacity .4s ease; }\n            :host .content .card .tiles .tile .title.show {\n              opacity: 1; }\n              :host .content .card .tiles .tile .title.show span {\n                -webkit-transform: translate(0, 0);\n                        transform: translate(0, 0); }\n            :host .content .card .tiles .tile .title span {\n              display: inline-block;\n              -webkit-transform: translate(0, 24px);\n                      transform: translate(0, 24px);\n              transition: -webkit-transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);\n              transition: transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);\n              transition: transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55), -webkit-transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55); }\n      :host .content .card .templates-spinner {\n        width: 48px;\n        margin: 12px 0 12px 12px;\n        height: 88px;\n        display: inline-block; }\n      :host .content .card md-select {\n        width: 320px; }\n      :host .content .card .row {\n        margin: 8px 0; }\n      :host .content .card button {\n        margin: 0 0 0 8px;\n        float: left;\n        background: #0088f4; }\n      :host .content .card .fr-getting-started {\n        border: none; }\n\n/deep/ md-tab-group md-tab-header {\n  border-bottom: none !important; }\n  /deep/ md-tab-group md-tab-header md-ink-bar {\n    display: none !important; }\n  /deep/ md-tab-group md-tab-header .mat-tab-labels {\n    display: block; }\n    /deep/ md-tab-group md-tab-header .mat-tab-labels .mat-tab-label {\n      min-width: 0 !important;\n      display: inline-block; }\n      /deep/ md-tab-group md-tab-header .mat-tab-labels .mat-tab-label.mat-tab-label-active {\n        opacity: 1; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ "../../../../../src/app/template-picker/template-picker.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TemplatePickerComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__ = __webpack_require__("../../../../@ngx-translate/core/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__ = __webpack_require__("../../../../../src/app/core/module-api.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Rx__ = __webpack_require__("../../../../rxjs/Rx.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_rxjs_Rx__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__ = __webpack_require__("../../../../../src/app/template-picker/template-filter.pipe.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_rxjs_Subject__ = __webpack_require__("../../../../rxjs/Subject.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5_rxjs_Subject__);
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var win = window;
var TemplatePickerComponent = (function () {
    function TemplatePickerComponent(api, templateFilter, appRef, translate) {
        var _this = this;
        this.api = api;
        this.templateFilter = templateFilter;
        this.appRef = appRef;
        this.translate = translate;
        this.apps = [];
        this.templates = [];
        this.contentTypes = [];
        this.showProgress = false;
        this.showInstaller = false;
        this.loading = false;
        this.loadingTemplates = false;
        this.ready = false;
        this.tabIndex = 0;
        this.updateTemplateSubject = new __WEBPACK_IMPORTED_MODULE_5_rxjs_Subject__["Subject"]();
        this.updateAppSubject = new __WEBPACK_IMPORTED_MODULE_5_rxjs_Subject__["Subject"]();
        this.allTemplates = [];
        this.cViewWithoutContent = '_LayoutElement';
        this.cAppActionImport = -1;
        this.frame = win.frameElement;
        this.dashInfo = this.frame.getAdditionalDashboardConfig();
        this.allowContentTypeChange = !(this.dashInfo.hasContent || this.dashInfo.isList);
        __WEBPACK_IMPORTED_MODULE_3_rxjs_Rx__["Observable"].merge(this.updateTemplateSubject.asObservable(), this.updateAppSubject.asObservable()).subscribe(function (res) {
            _this.loading = true;
        });
        this.updateAppSubject
            .debounceTime(400)
            .subscribe(function (_a) {
            var app = _a.app;
            _this.api.setAppId(app.appId.toString()).toPromise()
                .then(function (res) {
                if (app.supportsAjaxReload)
                    return _this.frame.reloadAndReInit()
                        .then(function () {
                        return _this.api.loadTemplates().toPromise()
                            .then(function () {
                            _this.loadingTemplates = false;
                            _this.template = _this.templates[0];
                            _this.appRef.tick();
                        })
                            .then(function () { doPostAjaxScrolling(_this); });
                    });
                _this.frame.showMessage("loading App...");
                doPostAjaxScrolling(_this);
                _this.frame.persistDia();
                win.parent.location.reload();
            });
        });
        this.updateTemplateSubject
            .debounceTime(400)
            .subscribe(function (_a) {
            var template = _a.template;
            _this.loadingTemplates = false;
            _this.template = template;
            _this.appRef.tick();
            if (_this.supportsAjax)
                return _this.frame.previewTemplate(template.TemplateId)
                    .then(function () { doPostAjaxScrolling(_this); });
            _this.frame.showMessage("refreshing <b>" + template.Name + "</b>...");
            doPostAjaxScrolling(_this);
            _this.frame.persistDia();
            return _this.frame.saveTemplate(_this.template.TemplateId)
                .then(function () { return win.parent.location.reload(); });
        });
        this.api.apps
            .subscribe(function (apps) {
            _this.app = apps.find(function (a) { return a.appId === _this.dashInfo.appId; });
            if (_this.app)
                _this.tabIndex = 1;
            _this.apps = apps;
        });
        this.api.templates
            .subscribe(function (templates) { return _this.setTemplates(templates, _this.dashInfo.templateId); });
        this.api.contentTypes
            .subscribe(function (contentTypes) { return _this.setContentTypes(contentTypes, _this.dashInfo.contentTypeId); });
        __WEBPACK_IMPORTED_MODULE_3_rxjs_Rx__["Observable"].combineLatest([
            this.api.templates,
            this.api.contentTypes,
            this.api.apps
        ]).subscribe(function (res) {
            _this.filterTemplates(_this.contentType);
            _this.ready = true;
            _this.showInstaller = _this.isContentApp
                ? res[0].length === 0
                : res[2].filter(function (a) { return a.appId !== _this.cAppActionImport; }).length === 0;
        });
    }
    TemplatePickerComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.isContentApp = this.dashInfo.isContent;
        this.supportsAjax = this.isContentApp || this.dashInfo.supportsAjax;
        this.showAdvanced = this.dashInfo.user.canDesign;
        this.undoTemplateId = this.dashInfo.templateId;
        this.undoContentTypeId = this.dashInfo.contentTypeId;
        this.savedAppId = this.dashInfo.appId;
        this.frame.isDirty = this.isDirty;
        this.dashInfo.templateChooserVisible = true;
        this.api.loadTemplates()
            .take(1)
            .subscribe(function (templates) { return _this.api.loadContentTypes(); });
        this.api.loadApps();
    };
    TemplatePickerComponent.prototype.isDirty = function () {
        return false;
    };
    TemplatePickerComponent.prototype.persistTemplate = function () {
        this.frame.saveTemplate(this.template.TemplateId);
    };
    TemplatePickerComponent.prototype.appStore = function () {
        win.open("https://2sxc.org/apps");
    };
    TemplatePickerComponent.prototype.filterTemplates = function (contentType) {
        this.templates = this.templateFilter.transform(this.allTemplates, {
            contentTypeId: contentType ? contentType.StaticName : undefined,
            isContentApp: this.isContentApp
        });
    };
    TemplatePickerComponent.prototype.setTemplates = function (templates, selectedTemplateId) {
        if (selectedTemplateId)
            this.template = templates.find(function (t) { return t.TemplateId === selectedTemplateId; });
        this.allTemplates = templates;
    };
    TemplatePickerComponent.prototype.setContentTypes = function (contentTypes, selectedContentTypeId) {
        var _this = this;
        if (selectedContentTypeId) {
            this.contentType = contentTypes.find(function (c) { return c.StaticName === selectedContentTypeId; });
            this.tabIndex = 1;
        }
        contentTypes
            .filter(function (c) { return (_this.template && c.TemplateId === _this.template.TemplateId) || (_this.contentType && c.StaticName === _this.contentType.StaticName); })
            .forEach(function (c) { return c.IsHidden = false; });
        // option for no content types
        if (this.allTemplates.find(function (t) { return t.ContentTypeStaticName === ''; })) {
            var name = "TemplatePicker.LayoutElement";
            contentTypes.push({
                StaticName: this.cViewWithoutContent,
                Name: name,
                Thumbnail: null,
                Label: this.translate.transform(name),
                IsHidden: false,
            });
        }
        this.contentTypes = contentTypes
            .sort(function (a, b) {
            if (a.Name > b.Name)
                return 1;
            if (a.Name < b.Name)
                return -1;
            return 0;
        });
    };
    TemplatePickerComponent.prototype.updateContentType = function (contentType, keepTemplate) {
        if (keepTemplate === void 0) { keepTemplate = false; }
        if (!this.allowContentTypeChange)
            return false;
        this.contentType = contentType;
        this.tabIndex = 1;
        this.templates = [];
        this.loadingTemplates = true;
        this.filterTemplates(contentType);
        if (this.templates.length === 0)
            return false;
        this.updateTemplateSubject.next({
            template: keepTemplate ? (this.template || this.templates[0]) : this.templates[0],
        });
        return true;
    };
    TemplatePickerComponent.prototype.reloadContentType = function () {
        this.updateContentType(this.contentType, true);
    };
    TemplatePickerComponent.prototype.switchTab = function () {
        this.tabIndex = 1;
    };
    TemplatePickerComponent.prototype.updateApp = function (app) {
        this.app = app;
        this.templates = [];
        this.loadingTemplates = true;
        this.updateAppSubject.next({
            app: app,
        });
    };
    return TemplatePickerComponent;
}());
TemplatePickerComponent = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_1__angular_core__["n" /* Component */])({
        selector: 'app-template-picker',
        template: __webpack_require__("../../../../../src/app/template-picker/template-picker.component.html"),
        styles: [__webpack_require__("../../../../../src/app/template-picker/template-picker.component.scss")],
        providers: [__WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["c" /* TranslatePipe */]],
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_1__angular_core__["f" /* ApplicationRef */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_core__["f" /* ApplicationRef */]) === "function" && _c || Object, typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["c" /* TranslatePipe */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["c" /* TranslatePipe */]) === "function" && _d || Object])
], TemplatePickerComponent);

function doPostAjaxScrolling(target) {
    target.loading = false;
    target.frame.scrollToTarget();
    target.appRef.tick();
}
var _a, _b, _c, _d;
//# sourceMappingURL=template-picker.component.js.map

/***/ }),

/***/ "../../../../../src/app/template-picker/template-picker.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TemplatePickerModule; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__ = __webpack_require__("../../../../@ngx-translate/core/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__("../../../forms/@angular/forms.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_material__ = __webpack_require__("../../../material/@angular/material.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_common__ = __webpack_require__("../../../common/@angular/common.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_platform_browser_animations__ = __webpack_require__("../../../platform-browser/@angular/platform-browser/animations.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__template_picker_component__ = __webpack_require__("../../../../../src/app/template-picker/template-picker.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__template_filter_pipe__ = __webpack_require__("../../../../../src/app/template-picker/template-filter.pipe.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_app_core_core_module__ = __webpack_require__("../../../../../src/app/core/core.module.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__content_type_filter_pipe__ = __webpack_require__("../../../../../src/app/template-picker/content-type-filter.pipe.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__angular_flex_layout__ = __webpack_require__("../../../flex-layout/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11_app_installer_installer_module__ = __webpack_require__("../../../../../src/app/installer/installer.module.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};












var TemplatePickerModule = (function () {
    function TemplatePickerModule() {
    }
    return TemplatePickerModule;
}());
TemplatePickerModule = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_1__angular_core__["L" /* NgModule */])({
        exports: [
            __WEBPACK_IMPORTED_MODULE_6__template_picker_component__["a" /* TemplatePickerComponent */]
        ],
        imports: [
            __WEBPACK_IMPORTED_MODULE_4__angular_common__["a" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["e" /* MdMenuModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["h" /* MdTabsModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["b" /* MaterialModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_platform_browser_animations__["a" /* BrowserAnimationsModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["f" /* MdProgressBarModule */],
            __WEBPACK_IMPORTED_MODULE_8_app_core_core_module__["a" /* CoreModule */],
            __WEBPACK_IMPORTED_MODULE_2__angular_forms__["b" /* FormsModule */],
            __WEBPACK_IMPORTED_MODULE_10__angular_flex_layout__["a" /* FlexLayoutModule */],
            __WEBPACK_IMPORTED_MODULE_11_app_installer_installer_module__["a" /* InstallerModule */],
            __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["b" /* TranslateModule */]
        ],
        providers: [
            __WEBPACK_IMPORTED_MODULE_7__template_filter_pipe__["a" /* TemplateFilterPipe */]
        ],
        declarations: [__WEBPACK_IMPORTED_MODULE_6__template_picker_component__["a" /* TemplatePickerComponent */], __WEBPACK_IMPORTED_MODULE_7__template_filter_pipe__["a" /* TemplateFilterPipe */], __WEBPACK_IMPORTED_MODULE_9__content_type_filter_pipe__["a" /* ContentTypeFilterPipe */]]
    })
], TemplatePickerModule);

//# sourceMappingURL=template-picker.module.js.map

/***/ }),

/***/ "../../../../../src/app/version-dialog/dialog.component.html":
/***/ (function(module, exports) {

module.exports = "<md-toolbar color=\"primary\">\r\n  <span class=\"title\">{{\"ItemHistory.Title\" | translate}}</span>\r\n  <span class=\"spacer\"></span>\r\n  <button md-dialog-close md-icon-button>\r\n    <md-icon class=\"example-icon\">close</md-icon>\r\n  </button>\r\n</md-toolbar>\r\n<div class=\"table\">\r\n  <div class=\"record\" *ngFor=\"let version of sxcVersion.versions | async\">\r\n    <md-expansion-panel>\r\n      <md-expansion-panel-header>\r\n        <md-panel-title fxFlex=\"108px\">{{\"ItemHistory.Version\" | translate:({version:version.VersionNumber})}}</md-panel-title>\r\n        <md-panel-description>{{version.TimeStamp}}</md-panel-description>\r\n      </md-expansion-panel-header>\r\n      <div class=\"detail\">\r\n        <div fxLayout=\"row\" [class.changed]=\"data.hasChanged\" *ngFor=\"let data of version.Data\">\r\n          <div fxFlex=\"160px\" class=\"label\">{{data.key}}:</div>\r\n          <div fxFlex [class.expand]=\"data.expand\" class=\"value\" title=\"expand content\" (click)=\"data.expand=!data.expand\">\r\n            <div class=\"lang-wrapper\" *ngFor=\"let val of data.value\">\r\n              <div *ngIf=\"data.value.length > 0\" class=\"lang\">{{val[0]}}</div>\r\n              <div [innerHTML]=\"val[1]\"></div>\r\n            </div>\r\n          </div>\r\n          <div flex=\"nogrow\" *ngIf=\"data.value.length > 1\">\r\n            <i *ngFor=\"let val of data.value\">[{{val[0]}}]&nbsp;</i>\r\n          </div>\r\n          <i flex=\"nogrow\">[{{data.type}}]</i>\r\n        </div>\r\n      </div>\r\n      <md-action-row>\r\n        <!--<button md-button (click)=\"restoreDraft(version)\">{{\"ItemHistory.Buttons.RestoreDraft\" | translate}}</button>-->\r\n        <button md-button (click)=\"restoreLive(version)\">{{\"ItemHistory.Buttons.RestoreLive\" | translate}}</button>\r\n      </md-action-row>\r\n    </md-expansion-panel>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "../../../../../src/app/version-dialog/dialog.component.scss":
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__("../../../../css-loader/lib/css-base.js")(false);
// imports


// module
exports.push([module.i, ":host {\n  width: 1000px;\n  max-width: 100%;\n  display: block; }\n  :host md-toolbar {\n    background: transparent;\n    color: black; }\n    :host md-toolbar .spacer {\n      -webkit-box-flex: 1;\n          -ms-flex: 1 1 auto;\n              flex: 1 1 auto; }\n    :host md-toolbar .title {\n      font-weight: 300; }\n  :host .table {\n    padding: 8px; }\n    :host .table .header {\n      line-height: 48px;\n      font-weight: 400;\n      padding: 0 22px;\n      color: rgba(0, 0, 0, 0.6); }\n    :host .table .record md-expansion-panel {\n      transition: box-shadow 280ms cubic-bezier(0.4, 0, 0.2, 1), margin 280ms ease; }\n      :host .table .record md-expansion-panel.mat-expanded {\n        margin: 16px 0 !important; }\n      :host .table .record md-expansion-panel .detail {\n        line-height: 28px;\n        box-sizing: border-box; }\n        :host .table .record md-expansion-panel .detail > div {\n          border-bottom: 1px solid rgba(0, 0, 0, 0.1);\n          margin-bottom: 4px;\n          padding: 4px 0; }\n          :host .table .record md-expansion-panel .detail > div.changed .label,\n          :host .table .record md-expansion-panel .detail > div.changed .value {\n            color: #2196F3; }\n          :host .table .record md-expansion-panel .detail > div:last-of-type {\n            border-bottom: none; }\n          :host .table .record md-expansion-panel .detail > div .label {\n            vertical-align: top;\n            color: rgba(0, 0, 0, 0.6);\n            height: 28px; }\n          :host .table .record md-expansion-panel .detail > div .value {\n            cursor: pointer;\n            vertical-align: top;\n            height: 28px;\n            overflow: hidden;\n            display: inline-block;\n            white-space: nowrap;\n            text-overflow: ellipsis; }\n            :host .table .record md-expansion-panel .detail > div .value .lang {\n              display: none; }\n            :host .table .record md-expansion-panel .detail > div .value.expand {\n              height: auto; }\n              :host .table .record md-expansion-panel .detail > div .value.expand .lang-wrapper {\n                position: relative;\n                padding: 8px 0; }\n                :host .table .record md-expansion-panel .detail > div .value.expand .lang-wrapper .lang {\n                  display: block;\n                  position: absolute;\n                  font-size: 10pt;\n                  left: 0;\n                  top: 0;\n                  color: rgba(0, 0, 0, 0.6);\n                  line-height: 12px;\n                  font-style: italic; }\n            :host .table .record md-expansion-panel .detail > div .value /deep/ * {\n              margin: 0; }\n          :host .table .record md-expansion-panel .detail > div i {\n            vertical-align: top;\n            height: 28px;\n            color: rgba(0, 0, 0, 0.6);\n            text-align: right;\n            font-size: 8pt; }\n      :host .table .record md-expansion-panel md-action-row button {\n        margin-left: 8px;\n        text-transform: uppercase; }\n  :host footer {\n    padding: 16px 22px; }\n    :host footer button {\n      margin-left: 8px; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ "../../../../../src/app/version-dialog/dialog.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return DialogComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ConfirmRestoreDialog; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_material__ = __webpack_require__("../../../material/@angular/material.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_version_dialog_sxc_versions_service__ = __webpack_require__("../../../../../src/app/version-dialog/sxc-versions.service.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};



var DialogComponent = (function () {
    function DialogComponent(dialog, sxcVersion) {
        this.dialog = dialog;
        this.sxcVersion = sxcVersion;
        this.versions = [];
    }
    DialogComponent.prototype.restoreLive = function (version) {
        this.sxcVersion.restore(version.ChangeSetId)
            .subscribe(function (res) { return res ? window.parent.location.reload() : alert('restore failed miserably'); });
    };
    DialogComponent.prototype.restoreDraft = function (version) {
        this.dialog.open(ConfirmRestoreDialog, {
            data: { version: version, isDraft: true },
        }).afterClosed()
            .subscribe(function (res) { return res ? alert('restoring draft') : undefined; });
    };
    return DialogComponent;
}());
DialogComponent = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["n" /* Component */])({
        selector: 'app-dialog',
        template: __webpack_require__("../../../../../src/app/version-dialog/dialog.component.html"),
        styles: [__webpack_require__("../../../../../src/app/version-dialog/dialog.component.scss")],
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_material__["c" /* MdDialog */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_material__["c" /* MdDialog */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2_app_version_dialog_sxc_versions_service__["a" /* SxcVersionsService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_version_dialog_sxc_versions_service__["a" /* SxcVersionsService */]) === "function" && _b || Object])
], DialogComponent);

var ConfirmRestoreDialog = (function () {
    function ConfirmRestoreDialog(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    return ConfirmRestoreDialog;
}());
ConfirmRestoreDialog = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["n" /* Component */])({
        selector: 'confirm-restore-dialog',
        template: "\n    <div class=\"content\">\n      <div class=\"title\">Restoring {{data.isDraft ? 'draft' : 'live'}} to version <b>{{data.version.ChangeSetId}}</b>.</div>\n      <div fxLayout=\"row\">\n        <button md-button [md-dialog-close]=\"false\">abort</button>\n        <span fxFlex></span>\n        <button md-raised-button [md-dialog-close]=\"true\">proceed</button>\n      </div>\n    </div>\n  ",
    }),
    __param(1, Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["A" /* Inject */])(__WEBPACK_IMPORTED_MODULE_1__angular_material__["a" /* MD_DIALOG_DATA */])),
    __metadata("design:paramtypes", [typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_1__angular_material__["d" /* MdDialogRef */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_material__["d" /* MdDialogRef */]) === "function" && _c || Object, Object])
], ConfirmRestoreDialog);

var _a, _b, _c;
//# sourceMappingURL=dialog.component.js.map

/***/ }),

/***/ "../../../../../src/app/version-dialog/sxc-versions.service.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SxcVersionsService; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__("../../../http/@angular/http.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_ReplaySubject__ = __webpack_require__("../../../../rxjs/ReplaySubject.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_ReplaySubject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_ReplaySubject__);
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var SxcVersionsService = (function () {
    function SxcVersionsService(http) {
        this.http = http;
        this.versionsSubject = new __WEBPACK_IMPORTED_MODULE_2_rxjs_ReplaySubject__["ReplaySubject"](1);
        this.versions = this.versionsSubject.asObservable();
        this.apiUrl = $2sxc.urlParams.require('portalroot') + 'desktopmodules/2sxc/api/';
        this.loadVersions();
    }
    SxcVersionsService.prototype.restore = function (changeId) {
        var appId = $2sxc.urlParams.require('appId');
        var tabId = $2sxc.urlParams.require('tid');
        var cbId = $2sxc.urlParams.require('cbid');
        var modId = $2sxc.urlParams.require('mid');
        var item = JSON.parse($2sxc.urlParams.require('items'))[0];
        var url = this.apiUrl + "eav/entities/restore?appId=" + appId + "&changeId=" + changeId;
        var headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Headers */]();
        headers.append('TabId', tabId);
        headers.append('ModuleId', modId);
        headers.append('ContentBlockId', cbId);
        var options = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* RequestOptions */]({ headers: headers });
        return this.http.post(url, item, options);
    };
    SxcVersionsService.prototype.loadVersions = function () {
        var _this = this;
        var appId = $2sxc.urlParams.require('appId');
        var tabId = $2sxc.urlParams.require('tid');
        var cbId = $2sxc.urlParams.require('cbid');
        var modId = $2sxc.urlParams.require('mid');
        var item = JSON.parse($2sxc.urlParams.require('items'))[0];
        var url = this.apiUrl + "eav/entities/history?appId=" + appId;
        var headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Headers */]();
        headers.append('TabId', tabId);
        headers.append('ModuleId', modId);
        headers.append('ContentBlockId', cbId);
        var options = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* RequestOptions */]({ headers: headers });
        this.http.post(url, item, options)
            .map(function (res) { return res.json()
            .map(function (v, i, all) { return Object.assign(v, {
            Data: (function () {
                var lastVersion = all.find(function (v2) { return v2.VersionNumber === v.VersionNumber - 1; });
                var attr = JSON.parse(v.Json).Entity.Attributes;
                if (lastVersion) {
                    lastVersion = JSON.parse(lastVersion.Json).Entity.Attributes;
                }
                return Object.entries(attr)
                    .reduce(function (t, c) { return Array.prototype.concat(t, Object.entries(c[1])
                    .map(function (_a, i2) {
                    var key = _a[0], value = _a[1];
                    return ({ key: key, value: Object.entries(value), type: c[0], hasChanged: lastVersion ? JSON.stringify(lastVersion[c[0]][key]) !== JSON.stringify(value) : false });
                })); }, []);
            })(),
            TimeStamp: (function (timestamp) {
                var date = new Date(timestamp), y = date.getFullYear(), m = date.getUTCMonth(), d = date.getDate(), h = date.getHours(), min = date.getMinutes();
                return y + "-" + (m < 10 ? '0' : '') + m + "-" + (d < 10 ? '0' : '') + d + " " + (h < 10 ? '0' : '') + h + ":" + (min < 10 ? '0' : '') + min;
            })(v.TimeStamp),
        }); }); })
            .subscribe(function (v) { return _this.versionsSubject.next(v); });
    };
    return SxcVersionsService;
}());
SxcVersionsService = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["B" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* Http */]) === "function" && _a || Object])
], SxcVersionsService);

var _a;
//# sourceMappingURL=sxc-versions.service.js.map

/***/ }),

/***/ "../../../../../src/app/version-dialog/version-dialog.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return VersionDialogModule; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__ = __webpack_require__("../../../../@ngx-translate/core/index.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_common__ = __webpack_require__("../../../common/@angular/common.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_version_dialog_sxc_versions_service__ = __webpack_require__("../../../../../src/app/version-dialog/sxc-versions.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_app_version_dialog_dialog_component__ = __webpack_require__("../../../../../src/app/version-dialog/dialog.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_material__ = __webpack_require__("../../../material/@angular/material.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__angular_flex_layout__ = __webpack_require__("../../../flex-layout/index.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};







var VersionDialogModule = (function () {
    function VersionDialogModule() {
    }
    return VersionDialogModule;
}());
VersionDialogModule = __decorate([
    Object(__WEBPACK_IMPORTED_MODULE_1__angular_core__["L" /* NgModule */])({
        exports: [
            __WEBPACK_IMPORTED_MODULE_4_app_version_dialog_dialog_component__["b" /* DialogComponent */]
        ],
        imports: [
            __WEBPACK_IMPORTED_MODULE_2__angular_common__["a" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_material__["b" /* MaterialModule */],
            __WEBPACK_IMPORTED_MODULE_6__angular_flex_layout__["a" /* FlexLayoutModule */],
            __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["b" /* TranslateModule */]
        ],
        providers: [
            __WEBPACK_IMPORTED_MODULE_3_app_version_dialog_sxc_versions_service__["a" /* SxcVersionsService */]
        ],
        declarations: [
            __WEBPACK_IMPORTED_MODULE_4_app_version_dialog_dialog_component__["b" /* DialogComponent */],
            __WEBPACK_IMPORTED_MODULE_4_app_version_dialog_dialog_component__["a" /* ConfirmRestoreDialog */]
        ],
        entryComponents: [
            __WEBPACK_IMPORTED_MODULE_4_app_version_dialog_dialog_component__["b" /* DialogComponent */],
            __WEBPACK_IMPORTED_MODULE_4_app_version_dialog_dialog_component__["a" /* ConfirmRestoreDialog */],
        ],
    })
], VersionDialogModule);

//# sourceMappingURL=version-dialog.module.js.map

/***/ }),

/***/ "../../../../../src/environments/environment.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return environment; });
// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.
// The file contents for the current environment will overwrite these during build.
var environment = {
    production: false
};
//# sourceMappingURL=environment.js.map

/***/ }),

/***/ "../../../../../src/main.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/@angular/core.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__ = __webpack_require__("../../../platform-browser-dynamic/@angular/platform-browser-dynamic.es5.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__app_app_module__ = __webpack_require__("../../../../../src/app/app.module.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__environments_environment__ = __webpack_require__("../../../../../src/environments/environment.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__app_core_boot_control__ = __webpack_require__("../../../../../src/app/core/boot-control.ts");





if (__WEBPACK_IMPORTED_MODULE_3__environments_environment__["a" /* environment */].production) {
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_21" /* enableProdMode */])();
}
var init = function () {
    Object(__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])()
        .bootstrapModule(__WEBPACK_IMPORTED_MODULE_2__app_app_module__["a" /* AppModule */])
        .then(function () { return (window).appBootstrap && (window).appBootstrap(); })
        .catch(function (err) { return console.error('NG Bootstrap Error =>', err); });
};
// Init on first load
init();
// provide hook for outside reboot calls
var bootController = window.bootController = __WEBPACK_IMPORTED_MODULE_4__app_core_boot_control__["a" /* BootController */].getbootControl();
// Init on reboot request
var boot = bootController.watchReboot().subscribe(function () { return init(); });
//# sourceMappingURL=main.js.map

/***/ }),

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__("../../../../../src/main.ts");


/***/ })

},[0]);
//# sourceMappingURL=main.bundle.js.map