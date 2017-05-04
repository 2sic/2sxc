webpackJsonp([1,4],{

/***/ 114:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_map__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_map__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__ = __webpack_require__(54);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ModuleApiService; });
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
        this.base = 'http://2sxc.dev/desktopmodules/2sxc/api/';
    }
    ModuleApiService.prototype.getSelectableApps = function () {
        return this.http.get(this.base + "View/Module/GetSelectableApps")
            .map(function (response) { return response.json(); });
    };
    ModuleApiService.prototype.setAppId = function (appId) {
        return this.http.get(this.base + "view/Module/SetAppId?appId=" + appId);
    };
    ModuleApiService.prototype.getSelectableContentTypes = function () {
        return this.http.get(this.base + "View/Module/GetSelectableContentTypes")
            .map(function (response) { return (response.json() || []).map(function (x) {
            x.Label = (x.Metadata && x.Metadata.Label)
                ? x.Metadata.Label
                : x.Name;
            return x;
        }); });
    };
    ModuleApiService.prototype.getSelectableTemplates = function () {
        return this.http.get(this.base + "View/Module/GetSelectableTemplates")
            .map(function (response) { return response.json(); });
    };
    ModuleApiService.prototype.gettingStartedUrl = function () {
        var params = new URLSearchParams();
        params.set('dialog', 'gettingstarted');
        return this.http.get(this.base + "View/Module/RemoteInstallDialogUrl", { search: params });
    };
    return ModuleApiService;
}());
ModuleApiService = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["e" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _b || Object])
], ModuleApiService);

var _a, _b;
//# sourceMappingURL=module-api.service.js.map

/***/ }),

/***/ 115:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TemplateFilterPipe; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var TemplateFilterPipe = (function () {
    function TemplateFilterPipe() {
    }
    TemplateFilterPipe.prototype.transform = function (templates, contentTypeId) {
        return templates
            .filter(function (t) { return !t.IsHidden && t.ContentTypeStaticName === (contentTypeId || ''); });
    };
    return TemplateFilterPipe;
}());
TemplateFilterPipe = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["A" /* Pipe */])({
        name: 'templateFilter'
    })
], TemplateFilterPipe);

//# sourceMappingURL=template-filter.pipe.js.map

/***/ }),

/***/ 116:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_app_core_module_api_service__ = __webpack_require__(114);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__ = __webpack_require__(271);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_router__ = __webpack_require__(53);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__ = __webpack_require__(115);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_http__ = __webpack_require__(37);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TemplatePickerComponent; });
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
    function TemplatePickerComponent(api, route, http, templateFilter) {
        this.api = api;
        this.route = route;
        this.http = http;
        this.templateFilter = templateFilter;
        this.cViewWithoutContent = '_LayoutElement';
        this.cAppActionManage = -2;
        this.cAppActionImport = -1;
        this.cAppActionCreate = -3;
        this.apps = [];
        this.contentTypes = [];
        this.templates = [];
        this.showRemoteInstaller = false;
        this.remoteInstallerUrl = '';
        this.isLoading = false;
        this.frame = win.frameElement;
    }
    TemplatePickerComponent.prototype.isDirty = function () {
        return false;
    };
    TemplatePickerComponent.prototype.appStore = function () {
        win.open("http://2sxc.org/en/apps");
    };
    TemplatePickerComponent.prototype.updateTemplateId = function (evt) {
        var id = evt.value;
        if (this.supportsAjax)
            return this.frame.sxc.manage.contentBlock.reload(id);
        // TODO: Not sure why we need to set this value before calling persistTemplate. Clean up!
        this.frame.sxc.manage.contentBlock.templateId = this.templateId;
        // app
        this.frame.sxc.manage.contentBlock.persistTemplate(false)
            .then(function () { return win.parent.location.reload(); });
    };
    TemplatePickerComponent.prototype.updateContentTypeId = function (evt) {
        var id = evt.value;
        // select first template
        var firstTemplateId = this.templateFilter.transform(this.templates, id)[0].TemplateId;
        if (firstTemplateId === null)
            return false;
        this.templateId = firstTemplateId;
    };
    TemplatePickerComponent.prototype.updateAppId = function (evt) {
        var _this = this;
        var id = evt.value;
        // add app
        if (id === this.cAppActionImport)
            return this.frame.sxc.manage.run('app-import');
        // find new app specs
        var newApp = this.apps.find(function (a) { return a.AppId === id; });
        this.api.setAppId(id)
            .subscribe(function (res) {
            if (newApp.SupportsAjaxReload)
                return _this.frame.sxc.manage.reloadAndReInitialize(true);
            win.parent.location.reload();
        });
    };
    TemplatePickerComponent.prototype.ngOnInit = function () {
        this.dashInfo = this.frame.getAdditionalDashboardConfig();
        this.isContentApp = this.dashInfo.isContent;
        this.supportsAjax = this.isContentApp || this.dashInfo.supportsAjax;
        this.showAdvanced = this.dashInfo.user.canDesign;
        this.templateId = this.dashInfo.templateId;
        this.undoTemplateId = this.dashInfo.templateId;
        this.contentTypeId = this.dashInfo.contentTypeId;
        this.undoContentTypeId = this.contentTypeId;
        this.appId = this.dashInfo.appId || null;
        this.savedAppId = this.dashInfo.appId;
        this.frame.isDirty = this.isDirty;
        this.reloadTemplatesAndContentTypes();
        this.show(true);
    };
    TemplatePickerComponent.prototype.reloadTemplatesAndContentTypes = function () {
        var _this = this;
        this.isLoading = true;
        var obs = __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__["Observable"].forkJoin([
            this.api.getSelectableContentTypes(),
            this.api.getSelectableTemplates()
        ]);
        obs.subscribe(function (res) {
            _this.contentTypes = res[0] || [];
            _this.templates = res[1] || [];
            // ensure current content type is visible
            _this.contentTypes
                .filter(function (c) { return c.StaticName === _this.contentTypeId || c.TemplateId === _this.templateId; })
                .forEach(function (c) { return c.IsHidden = false; });
            // add option for no content type if there are templates without
            if (_this.templates.find(function (t) { return t.ContentTypeStaticName === ''; })) {
                // TODO: i18n
                var name = "TemplatePicker.LayoutElement";
                _this.contentTypes.push({
                    StaticName: _this.cViewWithoutContent,
                    Name: name,
                    Label: name,
                    IsHidden: false
                });
            }
            // sort the content types
            _this.contentTypes = _this.contentTypes
                .sort(function (a, b) {
                if (a.Name > b.Name)
                    return 1;
                if (a.Name < b.Name)
                    return -1;
                return 0;
            });
            _this.isLoading = false;
        });
        return obs;
    };
    TemplatePickerComponent.prototype.toggle = function () {
        if (this.dashInfo.templateChooserVisible)
            return this.frame.sxc.manage.contentBlock._cancelTemplateChange();
        this.show(true);
    };
    TemplatePickerComponent.prototype.show = function (stateChange) {
        // todo 8.4 disabled this, as this info should never be set from here again...
        if (stateChange !== undefined)
            this.dashInfo.templateChooserVisible = stateChange;
        if (this.dashInfo.templateChooserVisible) {
            var observables = [];
            if (this.appId !== null)
                observables.push(this.reloadTemplatesAndContentTypes());
            // if it's the app-dialog and the app's haven't been loaded yet...
            if (!this.isContentApp && this.apps.length === 0)
                observables.push(this.loadApps());
            return __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__["Observable"].forkJoin(observables)
                .subscribe(function () {
                // TODO: implement external installer
                // this.externalInstaller.showIfConfigIsEmpty
            });
        }
    };
    TemplatePickerComponent.prototype.loadApps = function () {
        var _this = this;
        var obs = this.api.getSelectableApps();
        obs.subscribe(function (apps) {
            _this.apps = apps;
            _this.appCount = apps.length; // needed in the future to check if it shows getting started
            if (_this.showAdvanced) {
                _this.apps.push({ Name: "TemplatePicker.Install", AppId: _this.cAppActionImport });
            }
        });
        return obs;
    };
    ;
    return TemplatePickerComponent;
}());
TemplatePickerComponent = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_13" /* Component */])({
        selector: 'app-template-picker',
        template: __webpack_require__(269),
        styles: [__webpack_require__(265)]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1_app_core_module_api_service__["a" /* ModuleApiService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1_app_core_module_api_service__["a" /* ModuleApiService */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* ActivatedRoute */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* ActivatedRoute */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5__angular_http__["b" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_5__angular_http__["b" /* Http */]) === "function" && _c || Object, typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */]) === "function" && _d || Object])
], TemplatePickerComponent);

var _a, _b, _c, _d;
//# sourceMappingURL=template-picker.component.js.map

/***/ }),

/***/ 179:
/***/ (function(module, exports) {

function webpackEmptyContext(req) {
	throw new Error("Cannot find module '" + req + "'.");
}
webpackEmptyContext.keys = function() { return []; };
webpackEmptyContext.resolve = webpackEmptyContext;
module.exports = webpackEmptyContext;
webpackEmptyContext.id = 179;


/***/ }),

/***/ 180:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__ = __webpack_require__(202);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__app_app_module__ = __webpack_require__(205);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__environments_environment__ = __webpack_require__(210);




if (__WEBPACK_IMPORTED_MODULE_3__environments_environment__["a" /* environment */].production) {
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["a" /* enableProdMode */])();
}
__webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])().bootstrapModule(__WEBPACK_IMPORTED_MODULE_2__app_app_module__["a" /* AppModule */]);
//# sourceMappingURL=main.js.map

/***/ }),

/***/ 204:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppComponent; });
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
    function AppComponent() {
    }
    return AppComponent;
}());
AppComponent = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_13" /* Component */])({
        selector: 'app-root',
        template: __webpack_require__(268),
        styles: [__webpack_require__(266)]
    }),
    __metadata("design:paramtypes", [])
], AppComponent);

//# sourceMappingURL=app.component.js.map

/***/ }),

/***/ 205:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__(75);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_http__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__app_component__ = __webpack_require__(204);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_app_template_picker_template_picker_module__ = __webpack_require__(209);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_app_template_picker_template_picker_component__ = __webpack_require__(116);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__angular_router__ = __webpack_require__(53);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__angular_common__ = __webpack_require__(18);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppModule; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};









var appRoutes = [
    {
        path: '',
        component: __WEBPACK_IMPORTED_MODULE_6_app_template_picker_template_picker_component__["a" /* TemplatePickerComponent */]
    }
];
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["b" /* NgModule */])({
        declarations: [
            __WEBPACK_IMPORTED_MODULE_4__app_component__["a" /* AppComponent */]
        ],
        imports: [
            __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser__["a" /* BrowserModule */],
            __WEBPACK_IMPORTED_MODULE_2__angular_forms__["a" /* FormsModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_http__["a" /* HttpModule */],
            __WEBPACK_IMPORTED_MODULE_5_app_template_picker_template_picker_module__["a" /* TemplatePickerModule */],
            __WEBPACK_IMPORTED_MODULE_7__angular_router__["a" /* RouterModule */].forRoot(appRoutes),
        ],
        providers: [
            { provide: "windowObject", useValue: window },
            { provide: __WEBPACK_IMPORTED_MODULE_8__angular_common__["a" /* APP_BASE_HREF */], useValue: window['_app_base'] || '/' }
        ],
        bootstrap: [__WEBPACK_IMPORTED_MODULE_4__app_component__["a" /* AppComponent */]]
    })
], AppModule);

//# sourceMappingURL=app.module.js.map

/***/ }),

/***/ 206:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__(18);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(53);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_core_module_api_service__ = __webpack_require__(114);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_app_core_2sxc_service__ = __webpack_require__(54);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CoreModule; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["b" /* NgModule */])({
        imports: [
            __WEBPACK_IMPORTED_MODULE_1__angular_common__["i" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_2__angular_router__["a" /* RouterModule */]
        ],
        declarations: [],
        providers: [
            __WEBPACK_IMPORTED_MODULE_3_app_core_module_api_service__["a" /* ModuleApiService */],
            __WEBPACK_IMPORTED_MODULE_4_app_core_2sxc_service__["a" /* $2sxcService */]
        ]
    })
], CoreModule);

//# sourceMappingURL=core.module.js.map

/***/ }),

/***/ 207:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__(54);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return HttpInterceptorService; });
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



var HttpInterceptorService = (function (_super) {
    __extends(HttpInterceptorService, _super);
    function HttpInterceptorService(backend, defaultOptions, sxc) {
        var _this = _super.call(this, backend, defaultOptions) || this;
        _this.sxc = sxc;
        _this.configure(defaultOptions);
        return _this;
    }
    HttpInterceptorService.prototype.request = function (url, options) {
        if (options === void 0) { options = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */](); }
        this.configure(options);
        if (typeof url === 'string')
            url = this.sxc.sxc.resolveServiceUrl(url);
        else
            url.url = this.sxc.sxc.resolveServiceUrl(url.url);
        return _super.prototype.request.call(this, url, options);
    };
    HttpInterceptorService.prototype.configure = function (options) {
        var mid = $2sxc.urlParams.require('mid'), tid = $2sxc.urlParams.require('tid');
        if (!options.headers)
            options.headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* Headers */]();
        options.headers.set('moduleId', mid);
        options.headers.set('tabId', tid);
        return options;
    };
    return HttpInterceptorService;
}(__WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */]));
HttpInterceptorService = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["e" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["f" /* ConnectionBackend */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["f" /* ConnectionBackend */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _c || Object])
], HttpInterceptorService);

var _a, _b, _c;
//# sourceMappingURL=http-interceptor.service.js.map

/***/ }),

/***/ 208:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ContentTypeFilterPipe; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["A" /* Pipe */])({
        name: 'contentTypeFilter'
    })
], ContentTypeFilterPipe);

//# sourceMappingURL=content-type-filter.pipe.js.map

/***/ }),

/***/ 209:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_forms__ = __webpack_require__(75);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_material__ = __webpack_require__(201);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_common__ = __webpack_require__(18);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_platform_browser_animations__ = __webpack_require__(203);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__template_picker_component__ = __webpack_require__(116);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__template_filter_pipe__ = __webpack_require__(115);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_app_core_core_module__ = __webpack_require__(206);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__content_type_filter_pipe__ = __webpack_require__(208);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__angular_flex_layout__ = __webpack_require__(198);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__angular_http__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11_app_http_interceptor_service__ = __webpack_require__(207);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12_app_core_2sxc_service__ = __webpack_require__(54);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TemplatePickerModule; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["b" /* NgModule */])({
        exports: [
            __WEBPACK_IMPORTED_MODULE_5__template_picker_component__["a" /* TemplatePickerComponent */]
        ],
        imports: [
            __WEBPACK_IMPORTED_MODULE_3__angular_common__["i" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_2__angular_material__["a" /* MaterialModule */],
            __WEBPACK_IMPORTED_MODULE_4__angular_platform_browser_animations__["a" /* BrowserAnimationsModule */],
            __WEBPACK_IMPORTED_MODULE_7_app_core_core_module__["a" /* CoreModule */],
            __WEBPACK_IMPORTED_MODULE_1__angular_forms__["a" /* FormsModule */],
            __WEBPACK_IMPORTED_MODULE_9__angular_flex_layout__["a" /* FlexLayoutModule */]
        ],
        providers: [
            __WEBPACK_IMPORTED_MODULE_6__template_filter_pipe__["a" /* TemplateFilterPipe */],
            {
                provide: __WEBPACK_IMPORTED_MODULE_10__angular_http__["b" /* Http */],
                useFactory: function (backend, options, sxc) {
                    return new __WEBPACK_IMPORTED_MODULE_11_app_http_interceptor_service__["a" /* HttpInterceptorService */](backend, options, sxc);
                },
                deps: [__WEBPACK_IMPORTED_MODULE_10__angular_http__["c" /* XHRBackend */], __WEBPACK_IMPORTED_MODULE_10__angular_http__["d" /* RequestOptions */], __WEBPACK_IMPORTED_MODULE_12_app_core_2sxc_service__["a" /* $2sxcService */]]
            }
        ],
        declarations: [__WEBPACK_IMPORTED_MODULE_5__template_picker_component__["a" /* TemplatePickerComponent */], __WEBPACK_IMPORTED_MODULE_6__template_filter_pipe__["a" /* TemplateFilterPipe */], __WEBPACK_IMPORTED_MODULE_8__content_type_filter_pipe__["a" /* ContentTypeFilterPipe */]]
    })
], TemplatePickerModule);

//# sourceMappingURL=template-picker.module.js.map

/***/ }),

/***/ 210:
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

/***/ 265:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(44)();
// imports


// module
exports.push([module.i, ":host md-card {\n  background: #028bff; }\n  :host md-card md-select {\n    width: 220px; }\n  :host md-card .row {\n    margin: 8px 0; }\n  :host md-card button {\n    margin: 0 4px;\n    float: left;\n    background: rgba(0, 0, 0, 0.1); }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 266:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(44)();
// imports


// module
exports.push([module.i, ":host {\r\n    display: block;\r\n    padding: 0 0 180px;\r\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 268:
/***/ (function(module, exports) {

module.exports = "<router-outlet></router-outlet>"

/***/ }),

/***/ 269:
/***/ (function(module, exports) {

module.exports = "<md-card>\n    <div *ngIf=\"!isContentApp\" fxLayout=\"row\" class=\"row\">\n        <md-select [(ngModel)]=\"appId\" (change)=\"updateAppId($event)\" [disabled]=\"dashInfo.hasContent\">\n            <md-option *ngFor=\"let app of apps\" [value]=\"app.AppId\">\n                {{ app.Name.indexOf('TemplatePicker.') === 0 ? '[+] ' + app.Name : app.Name }}\n            </md-option>\n        </md-select>\n        <div *ngIf=\"showAdvanced && !isContentApp\">\n            <button md-mini-fab *ngIf=\"appId !== null\" (click)=\"frame.sxc.manage.run('app')\" title=\"{{ 'TemplatePicker.App' }}\">\n                <md-icon>settings</md-icon>\n            </button>\n            <button md-mini-fab (click)=\"frame.sxc.manage.run('app-import')\" title=\"{{ 'TemplatePicker.Install' }}\">\n                <md-icon>add</md-icon>\n            </button>\n            <button md-mini-fab (click)=\"appStore()\" title=\"{{ 'TemplatePicker.Catalog' }}\">\n                <md-icon>add_shopping_cart</md-icon>\n            </button>\n            <button md-mini-fab (click)=\"frame.sxc.manage.run('zone')\" title=\"{{ 'TemplatePicker.Zone' }}\">\n                <md-icon>settings</md-icon>\n            </button>\n        </div>\n    </div>\n    <div *ngIf=\"isContentApp\">\n        <md-select [(ngModel)]=\"contentTypeId\" (change)=\"updateContentTypeId($event)\" [disabled]=\"dashInfo.hasContent || dashInfo.isList\">\n            <md-option *ngFor=\"let type of contentTypes | contentTypeFilter\" [value]=\"type.StaticName\">\n                {{ type.Label }}\n            </md-option>\n        </md-select>\n    </div>\n    <div class=\"row\" fxLayout=\"row\" *ngIf=\"isContentApp ? contentTypeId != 0 : (savedAppId != 0)\">\n        <md-select [(ngModel)]=\"templateId\" (change)=\"updateTemplateId($event)\" [disabled]=\"templates.length == 0 && templateId\">\n            <md-option *ngFor=\"let template of templates | templateFilter:contentTypeId\" [value]=\"template.TemplateId\">\n                {{ template.Name }}\n            </md-option>\n        </md-select>\n        <button md-mini-fab *ngIf=\"templateId !== null\" (click)=\"frame.sxc.manage.contentBlock.persistTemplate(false, false)\" title=\"{{ 'TemplatePicker.Save' }}\">\n        <md-icon>check</md-icon>\n    </button>\n        <button md-mini-fab *ngIf=\"undoTemplateId !== null\" (click)=\"frame.sxc.manage.contentBlock._cancelTemplateChange()\" title=\"{{ 'TemplatePicker.' + (isContentApp ? 'Cancel' : 'Close') }}\">\n        <md-icon>close</md-icon>\n    </button>\n    </div>\n    <div *ngIf=\"showRemoteInstaller\">\n        <iframe id=\"frGettingStarted\" src=\"{{ remoteInstallerUrl }}\" width=\"100%\" height=\"300px\"></iframe>\n        <div class=\"sc-loading\" id=\"pnlLoading\" *ngIf=\"progressIndicator.show\">\n            <i class=\"icon-eav-spinner animate-spin\"></i>\n            <span class=\"sc-loading-label\">\n                installing <span id=\"packageName\">{{ progressIndicator.label }}</span>\n            </span>\n        </div>\n    </div>\n</md-card>"

/***/ }),

/***/ 519:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(180);


/***/ }),

/***/ 54:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_router__ = __webpack_require__(53);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return $2sxcService; });
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
    function $2sxcService(route) {
        this.route = route;
        this.sxc = $2sxc(route.snapshot.queryParams['appId']);
    }
    return $2sxcService;
}());
$2sxcService = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["e" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* ActivatedRoute */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* ActivatedRoute */]) === "function" && _a || Object])
], $2sxcService);

var _a;
//# sourceMappingURL=$2sxc.service.js.map

/***/ })

},[519]);
//# sourceMappingURL=main.bundle.js.map