webpackJsonp([1,4],{

/***/ 115:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_Rx__ = __webpack_require__(133);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_rxjs_Rx__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_http__ = __webpack_require__(29);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__ = __webpack_require__(6);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InstallerService; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__angular_http__["a" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2__angular_http__["a" /* Http */]) === "function" && _a || Object])
], InstallerService);

var _a;
//# sourceMappingURL=installer.service.js.map

/***/ }),

/***/ 116:
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
    TemplateFilterPipe.prototype.transform = function (templates, args) {
        return templates
            .filter(function (t) { return !t.IsHidden && (!args.isContentApp || t.ContentTypeStaticName === (args.contentTypeId === '_LayoutElement' ? '' : (args.contentTypeId || ''))); });
    };
    return TemplateFilterPipe;
}());
TemplateFilterPipe = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["W" /* Pipe */])({
        name: 'templateFilter'
    })
], TemplateFilterPipe);

//# sourceMappingURL=template-filter.pipe.js.map

/***/ }),

/***/ 117:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__ = __webpack_require__(76);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__ = __webpack_require__(75);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Rx__ = __webpack_require__(133);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_rxjs_Rx__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_router__ = __webpack_require__(53);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_app_template_picker_template_filter_pipe__ = __webpack_require__(116);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_rxjs_Subject__ = __webpack_require__(6);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_6_rxjs_Subject__);
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
    function TemplatePickerComponent(api, route, templateFilter, appRef, translate) {
        var _this = this;
        this.api = api;
        this.route = route;
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
        this.updateTemplateSubject = new __WEBPACK_IMPORTED_MODULE_6_rxjs_Subject__["Subject"]();
        this.updateAppSubject = new __WEBPACK_IMPORTED_MODULE_6_rxjs_Subject__["Subject"]();
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
                    return _this.frame.sxc.manage.contentBlock.reloadAndReInitialize(true, true)
                        .then(function () {
                        return _this.api.loadTemplates().toPromise()
                            .then(function () {
                            _this.loadingTemplates = false;
                            _this.template = _this.templates[0];
                            _this.appRef.tick();
                        })
                            .then(function () {
                            _this.loading = false;
                            _this.frame.scrollToTarget();
                            _this.appRef.tick();
                        });
                    });
                _this.frame.sxc.manage.contentBlock.reloadNoLivePreview("<p class=\"no-live-preview-available\">Reloading App. Please wait.</p>")
                    .then(function () {
                    _this.loading = false;
                    _this.frame.scrollToTarget();
                    _this.appRef.tick();
                });
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
                return _this.frame.sxc.manage.contentBlock.reload(template.TemplateId)
                    .then(function () {
                    _this.loading = false;
                    _this.frame.scrollToTarget();
                    _this.appRef.tick();
                });
            _this.frame.sxc.manage.contentBlock.reloadNoLivePreview("<p class=\"no-live-preview-available\">Reloading content type <b>" + template.Name + "</b>. Please wait.</p>")
                .then(function () {
                _this.loading = false;
                _this.frame.scrollToTarget();
                _this.appRef.tick();
            });
            // TODO: Not sure why we need to set this value before calling persistTemplate. Clean up!
            _this.frame.sxc.manage.contentBlock.templateId = _this.template.TemplateId;
            return _this.frame.sxc.manage.contentBlock.persistTemplate(false)
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
        var cb = this.frame.sxc.manage.contentBlock;
        cb.templateId = this.template.TemplateId;
        cb.persistTemplate(false, false);
    };
    TemplatePickerComponent.prototype.appStore = function () {
        win.open("http://2sxc.org/en/apps");
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
        if (selectedContentTypeId)
            this.contentType = contentTypes.find(function (c) { return c.StaticName === selectedContentTypeId; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["_11" /* Component */])({
        selector: 'app-template-picker',
        template: __webpack_require__(282),
        styles: [__webpack_require__(277)],
        providers: [__WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["d" /* TranslatePipe */]],
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_4__angular_router__["b" /* ActivatedRoute */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_4__angular_router__["b" /* ActivatedRoute */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_5_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */]) === "function" && _c || Object, typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_1__angular_core__["j" /* ApplicationRef */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_core__["j" /* ApplicationRef */]) === "function" && _d || Object, typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["d" /* TranslatePipe */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["d" /* TranslatePipe */]) === "function" && _e || Object])
], TemplatePickerComponent);

var _a, _b, _c, _d, _e;
//# sourceMappingURL=template-picker.component.js.map

/***/ }),

/***/ 185:
/***/ (function(module, exports) {

function webpackEmptyContext(req) {
	throw new Error("Cannot find module '" + req + "'.");
}
webpackEmptyContext.keys = function() { return []; };
webpackEmptyContext.resolve = webpackEmptyContext;
module.exports = webpackEmptyContext;
webpackEmptyContext.id = 185;


/***/ }),

/***/ 186:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__ = __webpack_require__(208);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__app_app_module__ = __webpack_require__(211);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__environments_environment__ = __webpack_require__(219);




if (__WEBPACK_IMPORTED_MODULE_3__environments_environment__["a" /* environment */].production) {
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["a" /* enableProdMode */])();
}
__webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])().bootstrapModule(__WEBPACK_IMPORTED_MODULE_2__app_app_module__["a" /* AppModule */]);
//# sourceMappingURL=main.js.map

/***/ }),

/***/ 210:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__ = __webpack_require__(76);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(2);
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
    function AppComponent(translate) {
        this.translate = translate;
        translate.addLangs(["en", "fr"]);
        translate.setDefaultLang('en');
        var browserLang = translate.getBrowserLang();
        translate.use(browserLang.match(/en|fr/) ? browserLang : 'en');
    }
    return AppComponent;
}());
AppComponent = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["_11" /* Component */])({
        selector: 'app-root',
        template: __webpack_require__(280),
        styles: [__webpack_require__(278)]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["c" /* TranslateService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["c" /* TranslateService */]) === "function" && _a || Object])
], AppComponent);

var _a;
//# sourceMappingURL=app.component.js.map

/***/ }),

/***/ 211:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_http_loader__ = __webpack_require__(220);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(29);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__ngx_translate_core__ = __webpack_require__(76);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__ = __webpack_require__(25);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_forms__ = __webpack_require__(74);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__app_component__ = __webpack_require__(210);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_app_template_picker_template_picker_module__ = __webpack_require__(218);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_app_template_picker_template_picker_component__ = __webpack_require__(117);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__angular_router__ = __webpack_require__(53);
/* unused harmony export HttpLoaderFactory */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppModule; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};










function HttpLoaderFactory(http) {
    return new __WEBPACK_IMPORTED_MODULE_0__ngx_translate_http_loader__["a" /* TranslateHttpLoader */](http, "../i18n/sxc-admin-", ".js");
}
var appRoutes = [
    {
        path: '',
        component: __WEBPACK_IMPORTED_MODULE_8_app_template_picker_template_picker_component__["a" /* TemplatePickerComponent */]
    },
    {
        path: 'index.html',
        component: __WEBPACK_IMPORTED_MODULE_8_app_template_picker_template_picker_component__["a" /* TemplatePickerComponent */]
    }
];
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_4__angular_core__["b" /* NgModule */])({
        declarations: [
            __WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */]
        ],
        exports: [],
        imports: [
            __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__["a" /* BrowserModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_forms__["a" /* FormsModule */],
            __WEBPACK_IMPORTED_MODULE_7_app_template_picker_template_picker_module__["a" /* TemplatePickerModule */],
            __WEBPACK_IMPORTED_MODULE_9__angular_router__["a" /* RouterModule */],
            __WEBPACK_IMPORTED_MODULE_9__angular_router__["a" /* RouterModule */].forRoot(appRoutes),
            __WEBPACK_IMPORTED_MODULE_2__ngx_translate_core__["a" /* TranslateModule */].forRoot({
                loader: {
                    provide: __WEBPACK_IMPORTED_MODULE_2__ngx_translate_core__["b" /* TranslateLoader */],
                    useFactory: HttpLoaderFactory,
                    deps: [__WEBPACK_IMPORTED_MODULE_1__angular_http__["a" /* Http */]]
                }
            })
        ],
        providers: [],
        bootstrap: [__WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */]]
    })
], AppModule);

//# sourceMappingURL=app.module.js.map

/***/ }),

/***/ 212:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__(19);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(53);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_core_module_api_service__ = __webpack_require__(75);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_app_core_2sxc_service__ = __webpack_require__(54);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_http__ = __webpack_require__(29);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_app_core_http_interceptor_service_provider__ = __webpack_require__(213);
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
            __WEBPACK_IMPORTED_MODULE_1__angular_common__["c" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_2__angular_router__["a" /* RouterModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_http__["b" /* HttpModule */]
        ],
        declarations: [],
        providers: [
            __WEBPACK_IMPORTED_MODULE_3_app_core_module_api_service__["a" /* ModuleApiService */],
            __WEBPACK_IMPORTED_MODULE_4_app_core_2sxc_service__["a" /* $2sxcService */],
            __WEBPACK_IMPORTED_MODULE_6_app_core_http_interceptor_service_provider__["a" /* Http2SxcHttpProvider */],
        ]
    })
], CoreModule);

//# sourceMappingURL=core.module.js.map

/***/ }),

/***/ 213:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__http_interceptor_service__ = __webpack_require__(214);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(29);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__(54);
/* unused harmony export Http2SxcProviderFactory */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Http2SxcHttpProvider; });



function Http2SxcProviderFactory(backend, defaultOptions, sxc) {
    return new __WEBPACK_IMPORTED_MODULE_0__http_interceptor_service__["a" /* Http2sxc */](backend, defaultOptions, sxc);
}
var Http2SxcHttpProvider = {
    provide: __WEBPACK_IMPORTED_MODULE_1__angular_http__["a" /* Http */],
    useFactory: Http2SxcProviderFactory,
    deps: [__WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* XHRBackend */], __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */], __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]]
};
//# sourceMappingURL=http-interceptor.service.provider.js.map

/***/ }),

/***/ 214:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(29);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__(54);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Http2sxc; });
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
        if (options === void 0) { options = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */](); }
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
            options.headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["e" /* Headers */]();
        options.headers.set('ModuleId', mid);
        options.headers.set('TabId', tid);
        options.headers.set('ContentBlockId', cbid);
        options.headers.set('RequestVerificationToken', window.$.ServicesFramework(mid).getAntiForgeryValue());
        options.headers.set('X-Debugging-Hint', 'bootstrapped by 2sxc4ng');
        return options;
    };
    return Http2sxc;
}(__WEBPACK_IMPORTED_MODULE_1__angular_http__["a" /* Http */]));
Http2sxc = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["f" /* ConnectionBackend */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["f" /* ConnectionBackend */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _c || Object])
], Http2sxc);

var _a, _b, _c;
//# sourceMappingURL=http-interceptor.service.js.map

/***/ }),

/***/ 215:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_app_installer_installer_service__ = __webpack_require__(115);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__ = __webpack_require__(75);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__ = __webpack_require__(25);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InstallerComponent; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["M" /* Input */])(),
    __metadata("design:type", Boolean)
], InstallerComponent.prototype, "isContentApp", void 0);
InstallerComponent = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_11" /* Component */])({
        selector: 'app-installer',
        template: __webpack_require__(281),
        styles: [__webpack_require__(276)]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1_app_installer_installer_service__["a" /* InstallerService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1_app_installer_installer_service__["a" /* InstallerService */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_module_api_service__["a" /* ModuleApiService */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__["c" /* DomSanitizer */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3__angular_platform_browser__["c" /* DomSanitizer */]) === "function" && _c || Object])
], InstallerComponent);

var _a, _b, _c;
//# sourceMappingURL=installer.component.js.map

/***/ }),

/***/ 216:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__(19);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__installer_component__ = __webpack_require__(215);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_installer_installer_service__ = __webpack_require__(115);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_material__ = __webpack_require__(114);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InstallerModule; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["b" /* NgModule */])({
        imports: [
            __WEBPACK_IMPORTED_MODULE_1__angular_common__["c" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_4__angular_material__["e" /* MdProgressSpinnerModule */],
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

/***/ 217:
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["W" /* Pipe */])({
        name: 'contentTypeFilter'
    })
], ContentTypeFilterPipe);

//# sourceMappingURL=content-type-filter.pipe.js.map

/***/ }),

/***/ 218:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__ = __webpack_require__(76);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__(74);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_material__ = __webpack_require__(114);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_common__ = __webpack_require__(19);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_platform_browser_animations__ = __webpack_require__(209);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__template_picker_component__ = __webpack_require__(117);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__template_filter_pipe__ = __webpack_require__(116);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_app_core_core_module__ = __webpack_require__(212);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__content_type_filter_pipe__ = __webpack_require__(217);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__angular_flex_layout__ = __webpack_require__(204);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11_app_installer_installer_module__ = __webpack_require__(216);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TemplatePickerModule; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};












var appId = $2sxc.urlParams.require('appId');
var TemplatePickerModule = (function () {
    function TemplatePickerModule() {
    }
    return TemplatePickerModule;
}());
TemplatePickerModule = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["b" /* NgModule */])({
        exports: [
            __WEBPACK_IMPORTED_MODULE_6__template_picker_component__["a" /* TemplatePickerComponent */]
        ],
        imports: [
            __WEBPACK_IMPORTED_MODULE_4__angular_common__["c" /* CommonModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["a" /* MdMenuModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["b" /* MdTabsModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["c" /* MaterialModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_platform_browser_animations__["a" /* BrowserAnimationsModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_material__["d" /* MdProgressBarModule */],
            __WEBPACK_IMPORTED_MODULE_8_app_core_core_module__["a" /* CoreModule */],
            __WEBPACK_IMPORTED_MODULE_2__angular_forms__["a" /* FormsModule */],
            __WEBPACK_IMPORTED_MODULE_10__angular_flex_layout__["FlexLayoutModule"],
            __WEBPACK_IMPORTED_MODULE_11_app_installer_installer_module__["a" /* InstallerModule */],
            __WEBPACK_IMPORTED_MODULE_0__ngx_translate_core__["a" /* TranslateModule */]
        ],
        providers: [
            __WEBPACK_IMPORTED_MODULE_7__template_filter_pipe__["a" /* TemplateFilterPipe */]
        ],
        declarations: [__WEBPACK_IMPORTED_MODULE_6__template_picker_component__["a" /* TemplatePickerComponent */], __WEBPACK_IMPORTED_MODULE_7__template_filter_pipe__["a" /* TemplateFilterPipe */], __WEBPACK_IMPORTED_MODULE_9__content_type_filter_pipe__["a" /* ContentTypeFilterPipe */]]
    })
], TemplatePickerModule);

//# sourceMappingURL=template-picker.module.js.map

/***/ }),

/***/ 219:
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

/***/ 276:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(35)();
// imports


// module
exports.push([module.i, ":host iframe {\n  border: none;\n  height: 500px; }\n\n:host .progress {\n  position: absolute;\n  left: 0;\n  top: 0;\n  height: 100%;\n  width: 100%;\n  background: rgba(255, 255, 255, 0.8);\n  display: -webkit-box;\n  display: -ms-flexbox;\n  display: flex;\n  -webkit-box-pack: center;\n      -ms-flex-pack: center;\n          justify-content: center;\n  -webkit-box-orient: vertical;\n  -webkit-box-direction: normal;\n      -ms-flex-direction: column;\n          flex-direction: column;\n  text-align: center; }\n  :host .progress md-progress-spinner {\n    margin: 0 auto; }\n  :host .progress span {\n    line-height: 48px; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 277:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(35)();
// imports


// module
exports.push([module.i, "/**\n * Applies styles for users in high contrast mode. Note that this only applies\n * to Microsoft browsers. Chrome can be included by checking for the `html[hc]`\n * attribute, however Chrome handles high contrast differently.\n */\n/* Theme for the ripple elements.*/\n/** The mixins below are shared between md-menu and md-select */\n/**\n * This mixin adds the correct panel transform styles based\n * on the direction that the menu panel opens.\n */\n/* stylelint-disable material/no-prefixes */\n/* stylelint-enable */\n/**\n * Applies styles for users in high contrast mode. Note that this only applies\n * to Microsoft browsers. Chrome can be included by checking for the `html[hc]`\n * attribute, however Chrome handles high contrast differently.\n */\n/**\n * This mixin contains shared option styles between the select and\n * autocomplete components.\n */\n.mat-elevation-z0 {\n  box-shadow: 0px 0px 0px 0px rgba(0, 0, 0, 0.2), 0px 0px 0px 0px rgba(0, 0, 0, 0.14), 0px 0px 0px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z1 {\n  box-shadow: 0px 2px 1px -1px rgba(0, 0, 0, 0.2), 0px 1px 1px 0px rgba(0, 0, 0, 0.14), 0px 1px 3px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z2 {\n  box-shadow: 0px 3px 1px -2px rgba(0, 0, 0, 0.2), 0px 2px 2px 0px rgba(0, 0, 0, 0.14), 0px 1px 5px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z3 {\n  box-shadow: 0px 3px 3px -2px rgba(0, 0, 0, 0.2), 0px 3px 4px 0px rgba(0, 0, 0, 0.14), 0px 1px 8px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z4 {\n  box-shadow: 0px 2px 4px -1px rgba(0, 0, 0, 0.2), 0px 4px 5px 0px rgba(0, 0, 0, 0.14), 0px 1px 10px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z5 {\n  box-shadow: 0px 3px 5px -1px rgba(0, 0, 0, 0.2), 0px 5px 8px 0px rgba(0, 0, 0, 0.14), 0px 1px 14px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z6 {\n  box-shadow: 0px 3px 5px -1px rgba(0, 0, 0, 0.2), 0px 6px 10px 0px rgba(0, 0, 0, 0.14), 0px 1px 18px 0px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z7 {\n  box-shadow: 0px 4px 5px -2px rgba(0, 0, 0, 0.2), 0px 7px 10px 1px rgba(0, 0, 0, 0.14), 0px 2px 16px 1px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z8 {\n  box-shadow: 0px 5px 5px -3px rgba(0, 0, 0, 0.2), 0px 8px 10px 1px rgba(0, 0, 0, 0.14), 0px 3px 14px 2px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z9 {\n  box-shadow: 0px 5px 6px -3px rgba(0, 0, 0, 0.2), 0px 9px 12px 1px rgba(0, 0, 0, 0.14), 0px 3px 16px 2px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z10 {\n  box-shadow: 0px 6px 6px -3px rgba(0, 0, 0, 0.2), 0px 10px 14px 1px rgba(0, 0, 0, 0.14), 0px 4px 18px 3px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z11 {\n  box-shadow: 0px 6px 7px -4px rgba(0, 0, 0, 0.2), 0px 11px 15px 1px rgba(0, 0, 0, 0.14), 0px 4px 20px 3px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z12 {\n  box-shadow: 0px 7px 8px -4px rgba(0, 0, 0, 0.2), 0px 12px 17px 2px rgba(0, 0, 0, 0.14), 0px 5px 22px 4px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z13 {\n  box-shadow: 0px 7px 8px -4px rgba(0, 0, 0, 0.2), 0px 13px 19px 2px rgba(0, 0, 0, 0.14), 0px 5px 24px 4px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z14 {\n  box-shadow: 0px 7px 9px -4px rgba(0, 0, 0, 0.2), 0px 14px 21px 2px rgba(0, 0, 0, 0.14), 0px 5px 26px 4px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z15 {\n  box-shadow: 0px 8px 9px -5px rgba(0, 0, 0, 0.2), 0px 15px 22px 2px rgba(0, 0, 0, 0.14), 0px 6px 28px 5px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z16 {\n  box-shadow: 0px 8px 10px -5px rgba(0, 0, 0, 0.2), 0px 16px 24px 2px rgba(0, 0, 0, 0.14), 0px 6px 30px 5px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z17 {\n  box-shadow: 0px 8px 11px -5px rgba(0, 0, 0, 0.2), 0px 17px 26px 2px rgba(0, 0, 0, 0.14), 0px 6px 32px 5px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z18 {\n  box-shadow: 0px 9px 11px -5px rgba(0, 0, 0, 0.2), 0px 18px 28px 2px rgba(0, 0, 0, 0.14), 0px 7px 34px 6px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z19 {\n  box-shadow: 0px 9px 12px -6px rgba(0, 0, 0, 0.2), 0px 19px 29px 2px rgba(0, 0, 0, 0.14), 0px 7px 36px 6px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z20 {\n  box-shadow: 0px 10px 13px -6px rgba(0, 0, 0, 0.2), 0px 20px 31px 3px rgba(0, 0, 0, 0.14), 0px 8px 38px 7px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z21 {\n  box-shadow: 0px 10px 13px -6px rgba(0, 0, 0, 0.2), 0px 21px 33px 3px rgba(0, 0, 0, 0.14), 0px 8px 40px 7px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z22 {\n  box-shadow: 0px 10px 14px -6px rgba(0, 0, 0, 0.2), 0px 22px 35px 3px rgba(0, 0, 0, 0.14), 0px 8px 42px 7px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z23 {\n  box-shadow: 0px 11px 14px -7px rgba(0, 0, 0, 0.2), 0px 23px 36px 3px rgba(0, 0, 0, 0.14), 0px 9px 44px 8px rgba(0, 0, 0, 0.12); }\n\n.mat-elevation-z24 {\n  box-shadow: 0px 11px 15px -7px rgba(0, 0, 0, 0.2), 0px 24px 38px 3px rgba(0, 0, 0, 0.14), 0px 9px 46px 8px rgba(0, 0, 0, 0.12); }\n\n.mat-ripple {\n  overflow: hidden; }\n\n.mat-ripple.mat-ripple-unbounded {\n  overflow: visible; }\n\n.mat-ripple-element {\n  position: absolute;\n  border-radius: 50%;\n  pointer-events: none;\n  transition: opacity, -webkit-transform 0ms cubic-bezier(0, 0, 0.2, 1);\n  transition: opacity, transform 0ms cubic-bezier(0, 0, 0.2, 1);\n  transition: opacity, transform 0ms cubic-bezier(0, 0, 0.2, 1), -webkit-transform 0ms cubic-bezier(0, 0, 0.2, 1);\n  -webkit-transform: scale(0);\n          transform: scale(0); }\n\n.mat-option {\n  white-space: nowrap;\n  overflow: hidden;\n  text-overflow: ellipsis;\n  display: block;\n  line-height: 48px;\n  height: 48px;\n  padding: 0 16px;\n  font-size: 16px;\n  font-family: Roboto, \"Helvetica Neue\", sans-serif;\n  text-align: left;\n  text-decoration: none;\n  position: relative;\n  cursor: pointer;\n  outline: none; }\n  .mat-option[disabled] {\n    cursor: default; }\n  [dir='rtl'] .mat-option {\n    text-align: right; }\n  .mat-option .mat-icon {\n    margin-right: 16px; }\n    [dir='rtl'] .mat-option .mat-icon {\n      margin-left: 16px; }\n  .mat-option[aria-disabled='true'] {\n    -webkit-user-select: none;\n    -moz-user-select: none;\n    -ms-user-select: none;\n    user-select: none;\n    cursor: default; }\n\n.mat-option-ripple {\n  position: absolute;\n  top: 0;\n  left: 0;\n  bottom: 0;\n  right: 0; }\n  @media screen and (-ms-high-contrast: active) {\n    .mat-option-ripple {\n      opacity: 0.5; } }\n\n.mat-option-pseudo-checkbox {\n  margin-right: 8px; }\n  [dir='rtl'] .mat-option-pseudo-checkbox {\n    margin-left: 8px;\n    margin-right: 0; }\n\n.cdk-visually-hidden {\n  border: 0;\n  clip: rect(0 0 0 0);\n  height: 1px;\n  margin: -1px;\n  overflow: hidden;\n  padding: 0;\n  position: absolute;\n  text-transform: none;\n  width: 1px; }\n\n.cdk-overlay-container, .cdk-global-overlay-wrapper {\n  pointer-events: none;\n  top: 0;\n  left: 0;\n  height: 100%;\n  width: 100%; }\n\n.cdk-overlay-container {\n  position: fixed;\n  z-index: 1000; }\n\n.cdk-global-overlay-wrapper {\n  display: -webkit-box;\n  display: -ms-flexbox;\n  display: flex;\n  position: absolute;\n  z-index: 1000; }\n\n.cdk-overlay-pane {\n  position: absolute;\n  pointer-events: auto;\n  box-sizing: border-box;\n  z-index: 1000; }\n\n.cdk-overlay-backdrop {\n  position: absolute;\n  top: 0;\n  bottom: 0;\n  left: 0;\n  right: 0;\n  z-index: 1000;\n  pointer-events: auto;\n  transition: opacity 400ms cubic-bezier(0.25, 0.8, 0.25, 1);\n  opacity: 0; }\n  .cdk-overlay-backdrop.cdk-overlay-backdrop-showing {\n    opacity: 0.48; }\n\n.cdk-overlay-dark-backdrop {\n  background: rgba(0, 0, 0, 0.6); }\n\n.cdk-overlay-transparent-backdrop {\n  background: none; }\n\n.mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.1); }\n\n.mat-option:hover:not(.mat-option-disabled), .mat-option:focus:not(.mat-option-disabled) {\n  background: rgba(0, 0, 0, 0.04); }\n\n.mat-option.mat-selected {\n  color: #00838f; }\n  .mat-option.mat-selected:not(.mat-option-multiple) {\n    background: rgba(0, 0, 0, 0.04); }\n\n.mat-option.mat-active {\n  background: rgba(0, 0, 0, 0.04);\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-option.mat-option-disabled {\n  color: rgba(0, 0, 0, 0.38); }\n\n.mat-pseudo-checkbox {\n  color: rgba(0, 0, 0, 0.54); }\n  .mat-pseudo-checkbox::after {\n    color: #fafafa; }\n\n.mat-pseudo-checkbox-checked.mat-primary, .mat-pseudo-checkbox-indeterminate.mat-primary {\n  background: #00bcd4; }\n\n.mat-pseudo-checkbox-checked.mat-accent, .mat-pseudo-checkbox-indeterminate.mat-accent {\n  background: #03a9f4; }\n\n.mat-pseudo-checkbox-checked.mat-warn, .mat-pseudo-checkbox-indeterminate.mat-warn {\n  background: #f44336; }\n\n.mat-pseudo-checkbox-checked.mat-pseudo-checkbox-disabled, .mat-pseudo-checkbox-indeterminate.mat-pseudo-checkbox-disabled {\n  background: #b0b0b0; }\n\n.mat-app-background {\n  background-color: #fafafa; }\n\n.mat-theme-loaded-marker {\n  display: none; }\n\n.mat-autocomplete-panel {\n  background: white;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-autocomplete-panel .mat-option.mat-selected:not(.mat-active) {\n    background: white;\n    color: rgba(0, 0, 0, 0.87); }\n\n.mat-button.mat-primary .mat-button-focus-overlay, .mat-icon-button.mat-primary .mat-button-focus-overlay, .mat-raised-button.mat-primary .mat-button-focus-overlay, .mat-fab.mat-primary .mat-button-focus-overlay, .mat-mini-fab.mat-primary .mat-button-focus-overlay {\n  background-color: rgba(0, 131, 143, 0.12); }\n\n.mat-button.mat-accent .mat-button-focus-overlay, .mat-icon-button.mat-accent .mat-button-focus-overlay, .mat-raised-button.mat-accent .mat-button-focus-overlay, .mat-fab.mat-accent .mat-button-focus-overlay, .mat-mini-fab.mat-accent .mat-button-focus-overlay {\n  background-color: rgba(3, 155, 229, 0.12); }\n\n.mat-button.mat-warn .mat-button-focus-overlay, .mat-icon-button.mat-warn .mat-button-focus-overlay, .mat-raised-button.mat-warn .mat-button-focus-overlay, .mat-fab.mat-warn .mat-button-focus-overlay, .mat-mini-fab.mat-warn .mat-button-focus-overlay {\n  background-color: rgba(229, 57, 53, 0.12); }\n\n.mat-button[disabled] .mat-button-focus-overlay, .mat-icon-button[disabled] .mat-button-focus-overlay, .mat-raised-button[disabled] .mat-button-focus-overlay, .mat-fab[disabled] .mat-button-focus-overlay, .mat-mini-fab[disabled] .mat-button-focus-overlay {\n  background-color: transparent; }\n\n.mat-button, .mat-icon-button {\n  background: transparent; }\n  .mat-button.mat-primary, .mat-icon-button.mat-primary {\n    color: #00838f; }\n  .mat-button.mat-accent, .mat-icon-button.mat-accent {\n    color: #039be5; }\n  .mat-button.mat-warn, .mat-icon-button.mat-warn {\n    color: #e53935; }\n  .mat-button.mat-primary[disabled], .mat-button.mat-accent[disabled], .mat-button.mat-warn[disabled], .mat-button[disabled][disabled], .mat-icon-button.mat-primary[disabled], .mat-icon-button.mat-accent[disabled], .mat-icon-button.mat-warn[disabled], .mat-icon-button[disabled][disabled] {\n    color: rgba(0, 0, 0, 0.38); }\n\n.mat-icon-button.mat-primary .mat-ripple-element {\n  background-color: rgba(0, 131, 143, 0.26); }\n\n.mat-icon-button.mat-accent .mat-ripple-element {\n  background-color: rgba(3, 155, 229, 0.26); }\n\n.mat-icon-button.mat-warn .mat-ripple-element {\n  background-color: rgba(229, 57, 53, 0.26); }\n\n.mat-raised-button, .mat-fab, .mat-mini-fab {\n  color: rgba(0, 0, 0, 0.87);\n  background-color: white; }\n  .mat-raised-button.mat-primary, .mat-fab.mat-primary, .mat-mini-fab.mat-primary {\n    color: white; }\n  .mat-raised-button.mat-accent, .mat-fab.mat-accent, .mat-mini-fab.mat-accent {\n    color: white; }\n  .mat-raised-button.mat-warn, .mat-fab.mat-warn, .mat-mini-fab.mat-warn {\n    color: white; }\n  .mat-raised-button.mat-primary[disabled], .mat-raised-button.mat-accent[disabled], .mat-raised-button.mat-warn[disabled], .mat-raised-button[disabled][disabled], .mat-fab.mat-primary[disabled], .mat-fab.mat-accent[disabled], .mat-fab.mat-warn[disabled], .mat-fab[disabled][disabled], .mat-mini-fab.mat-primary[disabled], .mat-mini-fab.mat-accent[disabled], .mat-mini-fab.mat-warn[disabled], .mat-mini-fab[disabled][disabled] {\n    color: rgba(0, 0, 0, 0.38); }\n  .mat-raised-button.mat-primary, .mat-fab.mat-primary, .mat-mini-fab.mat-primary {\n    background-color: #00838f; }\n  .mat-raised-button.mat-accent, .mat-fab.mat-accent, .mat-mini-fab.mat-accent {\n    background-color: #039be5; }\n  .mat-raised-button.mat-warn, .mat-fab.mat-warn, .mat-mini-fab.mat-warn {\n    background-color: #e53935; }\n  .mat-raised-button.mat-primary[disabled], .mat-raised-button.mat-accent[disabled], .mat-raised-button.mat-warn[disabled], .mat-raised-button[disabled][disabled], .mat-fab.mat-primary[disabled], .mat-fab.mat-accent[disabled], .mat-fab.mat-warn[disabled], .mat-fab[disabled][disabled], .mat-mini-fab.mat-primary[disabled], .mat-mini-fab.mat-accent[disabled], .mat-mini-fab.mat-warn[disabled], .mat-mini-fab[disabled][disabled] {\n    background-color: rgba(0, 0, 0, 0.12); }\n\n.mat-fab, .mat-mini-fab {\n  background-color: #039be5;\n  color: white; }\n\n.mat-button-toggle {\n  color: rgba(0, 0, 0, 0.38); }\n  .mat-button-toggle.cdk-focused .mat-button-toggle-focus-overlay {\n    background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-button-toggle-checked {\n  background-color: #e0e0e0;\n  color: black; }\n\n.mat-button-toggle-disabled {\n  background-color: #eeeeee;\n  color: rgba(0, 0, 0, 0.38); }\n  .mat-button-toggle-disabled.mat-button-toggle-checked {\n    background-color: #bdbdbd; }\n\n.mat-card {\n  background: white;\n  color: black; }\n\n.mat-card-subtitle {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-checkbox-frame {\n  border-color: rgba(0, 0, 0, 0.54); }\n\n.mat-checkbox-checkmark {\n  fill: #fafafa; }\n\n.mat-checkbox-checkmark-path {\n  stroke: #fafafa !important; }\n\n.mat-checkbox-mixedmark {\n  background-color: #fafafa; }\n\n.mat-checkbox-indeterminate.mat-primary .mat-checkbox-background, .mat-checkbox-checked.mat-primary .mat-checkbox-background {\n  background-color: #00bcd4; }\n\n.mat-checkbox-indeterminate.mat-accent .mat-checkbox-background, .mat-checkbox-checked.mat-accent .mat-checkbox-background {\n  background-color: #03a9f4; }\n\n.mat-checkbox-indeterminate.mat-warn .mat-checkbox-background, .mat-checkbox-checked.mat-warn .mat-checkbox-background {\n  background-color: #f44336; }\n\n.mat-checkbox-disabled.mat-checkbox-checked .mat-checkbox-background, .mat-checkbox-disabled.mat-checkbox-indeterminate .mat-checkbox-background {\n  background-color: #b0b0b0; }\n\n.mat-checkbox-disabled:not(.mat-checkbox-checked) .mat-checkbox-frame {\n  border-color: #b0b0b0; }\n\n.mat-checkbox:not(.mat-checkbox-disabled).mat-primary .mat-checkbox-ripple .mat-ripple-element {\n  background-color: rgba(0, 131, 143, 0.26); }\n\n.mat-checkbox:not(.mat-checkbox-disabled).mat-accent .mat-checkbox-ripple .mat-ripple-element {\n  background-color: rgba(3, 155, 229, 0.26); }\n\n.mat-checkbox:not(.mat-checkbox-disabled).mat-warn .mat-checkbox-ripple .mat-ripple-element {\n  background-color: rgba(229, 57, 53, 0.26); }\n\n.mat-chip:not(.mat-basic-chip) {\n  background-color: #e0e0e0;\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-chip.mat-chip-selected:not(.mat-basic-chip) {\n  background-color: #808080;\n  color: rgba(255, 255, 255, 0.87); }\n  .mat-chip.mat-chip-selected:not(.mat-basic-chip).mat-primary {\n    background-color: #00bcd4;\n    color: white; }\n  .mat-chip.mat-chip-selected:not(.mat-basic-chip).mat-accent {\n    background-color: #03a9f4;\n    color: white; }\n  .mat-chip.mat-chip-selected:not(.mat-basic-chip).mat-warn {\n    background-color: #f44336;\n    color: white; }\n\n.mat-dialog-container {\n  background: white; }\n\n.mat-icon.mat-primary {\n  color: #00838f; }\n\n.mat-icon.mat-accent {\n  color: #039be5; }\n\n.mat-icon.mat-warn {\n  color: #e53935; }\n\n.mat-input-placeholder {\n  color: rgba(0, 0, 0, 0.38); }\n  .mat-focused .mat-input-placeholder {\n    color: #00838f; }\n    .mat-focused .mat-input-placeholder.mat-accent {\n      color: #039be5; }\n    .mat-focused .mat-input-placeholder.mat-warn {\n      color: #e53935; }\n\n.mat-input-element:disabled {\n  color: rgba(0, 0, 0, 0.38); }\n\ninput.mat-input-element:-webkit-autofill + .mat-input-placeholder .mat-placeholder-required,\n.mat-focused .mat-input-placeholder.mat-float .mat-placeholder-required {\n  color: #039be5; }\n\n.mat-input-underline {\n  border-color: rgba(0, 0, 0, 0.12); }\n  .mat-input-underline .mat-input-ripple {\n    background-color: #00838f; }\n    .mat-input-underline .mat-input-ripple.mat-accent {\n      background-color: #039be5; }\n    .mat-input-underline .mat-input-ripple.mat-warn {\n      background-color: #e53935; }\n\n.mat-input-invalid .mat-input-placeholder,\n.mat-input-invalid .mat-placeholder-required {\n  color: #e53935; }\n\n.mat-input-invalid .mat-input-underline {\n  border-color: #e53935; }\n\n.mat-input-invalid .mat-input-ripple {\n  background-color: #e53935; }\n\n.mat-input-error {\n  color: #e53935; }\n\n.mat-list .mat-list-item, .mat-nav-list .mat-list-item {\n  color: black; }\n\n.mat-list .mat-subheader, .mat-nav-list .mat-subheader {\n  color: rgba(0, 0, 0, 0.54); }\n\n.mat-divider {\n  border-top-color: rgba(0, 0, 0, 0.12); }\n\n.mat-nav-list .mat-list-item-content:hover, .mat-nav-list .mat-list-item-content.mat-list-item-focus {\n  background: rgba(0, 0, 0, 0.04); }\n\n.mat-menu-content {\n  background: white; }\n\n.mat-menu-item {\n  background: transparent;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-menu-item[disabled] {\n    color: rgba(0, 0, 0, 0.38); }\n  .mat-menu-item .mat-icon {\n    color: rgba(0, 0, 0, 0.54);\n    vertical-align: middle; }\n  .mat-menu-item:hover:not([disabled]), .mat-menu-item:focus:not([disabled]) {\n    background: rgba(0, 0, 0, 0.04); }\n\n.mat-progress-bar-background {\n  background: url(\"data:image/svg+xml;charset=UTF-8,%3Csvg%20version%3D%271.1%27%20xmlns%3D%27http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%27%20xmlns%3Axlink%3D%27http%3A%2F%2Fwww.w3.org%2F1999%2Fxlink%27%20x%3D%270px%27%20y%3D%270px%27%20enable-background%3D%27new%200%200%205%202%27%20xml%3Aspace%3D%27preserve%27%20viewBox%3D%270%200%205%202%27%20preserveAspectRatio%3D%27none%20slice%27%3E%3Ccircle%20cx%3D%271%27%20cy%3D%271%27%20r%3D%271%27%20fill%3D%27%23b2ebf2%27%2F%3E%3C%2Fsvg%3E\"); }\n\n.mat-progress-bar-buffer {\n  background-color: #b2ebf2; }\n\n.mat-progress-bar-fill::after {\n  background-color: #00acc1; }\n\n.mat-progress-bar.mat-accent .mat-progress-bar-background {\n  background: url(\"data:image/svg+xml;charset=UTF-8,%3Csvg%20version%3D%271.1%27%20xmlns%3D%27http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%27%20xmlns%3Axlink%3D%27http%3A%2F%2Fwww.w3.org%2F1999%2Fxlink%27%20x%3D%270px%27%20y%3D%270px%27%20enable-background%3D%27new%200%200%205%202%27%20xml%3Aspace%3D%27preserve%27%20viewBox%3D%270%200%205%202%27%20preserveAspectRatio%3D%27none%20slice%27%3E%3Ccircle%20cx%3D%271%27%20cy%3D%271%27%20r%3D%271%27%20fill%3D%27%23b3e5fc%27%2F%3E%3C%2Fsvg%3E\"); }\n\n.mat-progress-bar.mat-accent .mat-progress-bar-buffer {\n  background-color: #b3e5fc; }\n\n.mat-progress-bar.mat-accent .mat-progress-bar-fill::after {\n  background-color: #039be5; }\n\n.mat-progress-bar.mat-warn .mat-progress-bar-background {\n  background: url(\"data:image/svg+xml;charset=UTF-8,%3Csvg%20version%3D%271.1%27%20xmlns%3D%27http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%27%20xmlns%3Axlink%3D%27http%3A%2F%2Fwww.w3.org%2F1999%2Fxlink%27%20x%3D%270px%27%20y%3D%270px%27%20enable-background%3D%27new%200%200%205%202%27%20xml%3Aspace%3D%27preserve%27%20viewBox%3D%270%200%205%202%27%20preserveAspectRatio%3D%27none%20slice%27%3E%3Ccircle%20cx%3D%271%27%20cy%3D%271%27%20r%3D%271%27%20fill%3D%27%23ffcdd2%27%2F%3E%3C%2Fsvg%3E\"); }\n\n.mat-progress-bar.mat-warn .mat-progress-bar-buffer {\n  background-color: #ffcdd2; }\n\n.mat-progress-bar.mat-warn .mat-progress-bar-fill::after {\n  background-color: #e53935; }\n\n.mat-progress-spinner path, .mat-spinner path {\n  stroke: #00acc1; }\n\n.mat-progress-spinner.mat-accent path, .mat-spinner.mat-accent path {\n  stroke: #039be5; }\n\n.mat-progress-spinner.mat-warn path, .mat-spinner.mat-warn path {\n  stroke: #e53935; }\n\n.mat-radio-outer-circle {\n  border-color: rgba(0, 0, 0, 0.54); }\n  .mat-radio-checked .mat-radio-outer-circle {\n    border-color: #039be5; }\n  .mat-radio-disabled .mat-radio-outer-circle {\n    border-color: rgba(0, 0, 0, 0.38); }\n\n.mat-radio-inner-circle {\n  background-color: #039be5; }\n  .mat-radio-disabled .mat-radio-inner-circle {\n    background-color: rgba(0, 0, 0, 0.38); }\n\n.mat-radio-ripple .mat-ripple-element {\n  background-color: rgba(3, 155, 229, 0.26); }\n  .mat-radio-disabled .mat-radio-ripple .mat-ripple-element {\n    background-color: rgba(0, 0, 0, 0.38); }\n\n.mat-select-trigger {\n  color: rgba(0, 0, 0, 0.38); }\n  .mat-select:focus:not(.mat-select-disabled) .mat-select-trigger {\n    color: #00838f; }\n  .mat-select:not(:focus).ng-invalid.ng-touched:not(.mat-select-disabled) .mat-select-trigger {\n    color: #e53935; }\n\n.mat-select-underline {\n  background-color: rgba(0, 0, 0, 0.12); }\n  .mat-select:focus:not(.mat-select-disabled) .mat-select-underline {\n    background-color: #00838f; }\n  .mat-select:not(:focus).ng-invalid.ng-touched:not(.mat-select-disabled) .mat-select-underline {\n    background-color: #e53935; }\n\n.mat-select-arrow {\n  color: rgba(0, 0, 0, 0.38); }\n  .mat-select:focus:not(.mat-select-disabled) .mat-select-arrow {\n    color: #00838f; }\n  .mat-select:not(:focus).ng-invalid.ng-touched:not(.mat-select-disabled) .mat-select-arrow {\n    color: #e53935; }\n\n.mat-select-content, .mat-select-panel-done-animating {\n  background: white; }\n\n.mat-select-value {\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-select-disabled .mat-select-value {\n    color: rgba(0, 0, 0, 0.38); }\n\n.mat-sidenav-container {\n  background-color: #fafafa;\n  color: rgba(0, 0, 0, 0.87); }\n\n.mat-sidenav {\n  background-color: white;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-sidenav.mat-sidenav-push {\n    background-color: white; }\n\n.mat-sidenav-backdrop.mat-sidenav-shown {\n  background-color: rgba(0, 0, 0, 0.6); }\n\n.mat-slide-toggle.mat-checked:not(.mat-disabled) .mat-slide-toggle-thumb {\n  background-color: #03a9f4; }\n\n.mat-slide-toggle.mat-checked:not(.mat-disabled) .mat-slide-toggle-bar {\n  background-color: rgba(3, 169, 244, 0.5); }\n\n.mat-slide-toggle:not(.mat-checked) .mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-slide-toggle .mat-ripple-element {\n  background-color: rgba(3, 169, 244, 0.12); }\n\n.mat-slide-toggle.mat-primary.mat-checked:not(.mat-disabled) .mat-slide-toggle-thumb {\n  background-color: #00bcd4; }\n\n.mat-slide-toggle.mat-primary.mat-checked:not(.mat-disabled) .mat-slide-toggle-bar {\n  background-color: rgba(0, 188, 212, 0.5); }\n\n.mat-slide-toggle.mat-primary:not(.mat-checked) .mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-slide-toggle.mat-primary .mat-ripple-element {\n  background-color: rgba(0, 188, 212, 0.12); }\n\n.mat-slide-toggle.mat-warn.mat-checked:not(.mat-disabled) .mat-slide-toggle-thumb {\n  background-color: #f44336; }\n\n.mat-slide-toggle.mat-warn.mat-checked:not(.mat-disabled) .mat-slide-toggle-bar {\n  background-color: rgba(244, 67, 54, 0.5); }\n\n.mat-slide-toggle.mat-warn:not(.mat-checked) .mat-ripple-element {\n  background-color: rgba(0, 0, 0, 0.06); }\n\n.mat-slide-toggle.mat-warn .mat-ripple-element {\n  background-color: rgba(244, 67, 54, 0.12); }\n\n.mat-disabled .mat-slide-toggle-thumb {\n  background-color: #bdbdbd; }\n\n.mat-disabled .mat-slide-toggle-bar {\n  background-color: rgba(0, 0, 0, 0.1); }\n\n.mat-slide-toggle-thumb {\n  background-color: #fafafa; }\n\n.mat-slide-toggle-bar {\n  background-color: rgba(0, 0, 0, 0.38); }\n\n.mat-slider-track-background {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-primary .mat-slider-track-fill, .mat-primary\n.mat-slider-thumb, .mat-primary\n.mat-slider-thumb-label {\n  background-color: #00838f; }\n\n.mat-accent .mat-slider-track-fill, .mat-accent\n.mat-slider-thumb, .mat-accent\n.mat-slider-thumb-label {\n  background-color: #039be5; }\n\n.mat-warn .mat-slider-track-fill, .mat-warn\n.mat-slider-thumb, .mat-warn\n.mat-slider-thumb-label {\n  background-color: #e53935; }\n\n.mat-slider-focus-ring {\n  background-color: rgba(3, 155, 229, 0.2); }\n\n.mat-primary .mat-slider-thumb-label-text {\n  color: white; }\n\n.mat-accent .mat-slider-thumb-label-text {\n  color: white; }\n\n.mat-warn .mat-slider-thumb-label-text {\n  color: white; }\n\n.mat-slider:hover .mat-slider-track-background,\n.cdk-focused .mat-slider-track-background {\n  background-color: rgba(0, 0, 0, 0.38); }\n\n.mat-slider-disabled .mat-slider-track-background,\n.mat-slider-disabled .mat-slider-track-fill,\n.mat-slider-disabled .mat-slider-thumb {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-slider-disabled:hover .mat-slider-track-background {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-slider-min-value .mat-slider-focus-ring {\n  background-color: rgba(0, 0, 0, 0.12); }\n\n.mat-slider-min-value.mat-slider-thumb-label-showing .mat-slider-thumb,\n.mat-slider-min-value.mat-slider-thumb-label-showing .mat-slider-thumb-label {\n  background-color: black; }\n\n.mat-slider-min-value.mat-slider-thumb-label-showing.cdk-focused .mat-slider-thumb,\n.mat-slider-min-value.mat-slider-thumb-label-showing.cdk-focused .mat-slider-thumb-label {\n  background-color: rgba(0, 0, 0, 0.26); }\n\n.mat-slider-min-value:not(.mat-slider-thumb-label-showing) .mat-slider-thumb {\n  border-color: rgba(0, 0, 0, 0.26);\n  background-color: transparent; }\n\n.mat-slider-min-value:not(.mat-slider-thumb-label-showing):hover .mat-slider-thumb, .mat-slider-min-value:not(.mat-slider-thumb-label-showing).cdk-focused .mat-slider-thumb {\n  border-color: rgba(0, 0, 0, 0.38); }\n\n.mat-slider-min-value:not(.mat-slider-thumb-label-showing):hover.mat-slider-disabled .mat-slider-thumb, .mat-slider-min-value:not(.mat-slider-thumb-label-showing).cdk-focused.mat-slider-disabled .mat-slider-thumb {\n  border-color: rgba(0, 0, 0, 0.26); }\n\n.mat-tab-nav-bar,\n.mat-tab-header {\n  border-bottom: 1px solid #e0e0e0; }\n  .mat-tab-group-inverted-header .mat-tab-nav-bar, .mat-tab-group-inverted-header\n  .mat-tab-header {\n    border-top: 1px solid #e0e0e0;\n    border-bottom: none; }\n\n.mat-tab-label:focus {\n  background-color: rgba(178, 235, 242, 0.3); }\n\n.mat-ink-bar {\n  background-color: #00bcd4; }\n\n.mat-tab-label, .mat-tab-link {\n  color: currentColor; }\n  .mat-tab-label.mat-tab-disabled, .mat-tab-link.mat-tab-disabled {\n    color: rgba(0, 0, 0, 0.38); }\n\n.mat-toolbar {\n  background: whitesmoke;\n  color: rgba(0, 0, 0, 0.87); }\n  .mat-toolbar.mat-primary {\n    background: #00838f;\n    color: white; }\n  .mat-toolbar.mat-accent {\n    background: #039be5;\n    color: white; }\n  .mat-toolbar.mat-warn {\n    background: #e53935;\n    color: white; }\n\n.mat-tooltip {\n  background: rgba(97, 97, 97, 0.9); }\n\n:host {\n  display: block;\n  padding-top: 56px; }\n  :host .content {\n    background: #fafafa;\n    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.3);\n    position: relative; }\n    :host .content .card {\n      max-width: 1200px;\n      margin: 0 auto; }\n      :host .content .card .top-controls {\n        position: absolute;\n        height: 56px;\n        right: 4px;\n        top: -56px;\n        width: 100%;\n        text-align: center;\n        z-index: 2;\n        display: -webkit-box;\n        display: -ms-flexbox;\n        display: flex;\n        -webkit-box-orient: horizontal;\n        -webkit-box-direction: normal;\n            -ms-flex-direction: row;\n                flex-direction: row;\n        -webkit-box-align: center;\n            -ms-flex-align: center;\n                align-items: center; }\n        :host .content .card .top-controls button {\n          display: inline-block;\n          float: none;\n          position: relative;\n          top: 28px; }\n        :host .content .card .top-controls button.secondary {\n          background: #fafafa !important;\n          color: rgba(0, 0, 0, 0.8); }\n      :host .content .card .tiles {\n        white-space: nowrap;\n        overflow-x: auto; }\n        :host .content .card .tiles .tile {\n          box-sizing: border-box;\n          background: #fff;\n          display: inline-block;\n          margin: 12px 0 12px 12px;\n          position: relative;\n          font-size: 12px;\n          border-radius: 22px 0 22px 0;\n          height: 88px;\n          width: 88px;\n          line-height: 88px;\n          text-align: center;\n          overflow: hidden;\n          cursor: pointer;\n          color: rgba(0, 0, 0, 0.8);\n          transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);\n          box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24); }\n          :host .content .card .tiles .tile:hover {\n            box-shadow: 0 3px 6px rgba(0, 0, 0, 0.16), 0 3px 6px rgba(0, 0, 0, 0.23); }\n            :host .content .card .tiles .tile:hover .title {\n              opacity: 1; }\n              :host .content .card .tiles .tile:hover .title span {\n                -webkit-transform: translate(0, 0);\n                        transform: translate(0, 0); }\n          :host .content .card .tiles .tile:active, :host .content .card .tiles .tile.active {\n            box-shadow: 0 14px 28px rgba(0, 120, 220, 0.25), 0 10px 10px rgba(0, 120, 220, 0.22); }\n          :host .content .card .tiles .tile.active, :host .content .card .tiles .tile:focus {\n            color: #fff; }\n          :host .content .card .tiles .tile.config {\n            box-shadow: none;\n            border: 1px dashed rgba(0, 0, 0, 0.2);\n            background: transparent; }\n          :host .content .card .tiles .tile .bg {\n            position: absolute;\n            left: 0;\n            top: 0;\n            width: 100%;\n            height: 100%; }\n            :host .content .card .tiles .tile .bg img {\n              width: 100%;\n              height: 100%; }\n          :host .content .card .tiles .tile .title {\n            display: inline-block;\n            position: absolute;\n            box-sizing: border-box;\n            padding: 0 4px;\n            left: 0;\n            bottom: 0;\n            width: 100%;\n            height: 24px;\n            line-height: 24px;\n            background: white;\n            color: rgba(0, 0, 0, 0.8);\n            letter-spacing: .1pt;\n            font-size: 11px;\n            text-overflow: ellipsis;\n            overflow: hidden;\n            white-space: nowrap;\n            opacity: 0;\n            font-weight: bold;\n            transition: opacity .4s ease; }\n            :host .content .card .tiles .tile .title.show {\n              opacity: 1; }\n              :host .content .card .tiles .tile .title.show span {\n                -webkit-transform: translate(0, 0);\n                        transform: translate(0, 0); }\n            :host .content .card .tiles .tile .title span {\n              display: inline-block;\n              -webkit-transform: translate(0, 24px);\n                      transform: translate(0, 24px);\n              transition: -webkit-transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);\n              transition: transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);\n              transition: transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55), -webkit-transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55); }\n      :host .content .card .templates-spinner {\n        width: 48px;\n        margin: 12px 0 12px 12px;\n        height: 88px;\n        display: inline-block; }\n      :host .content .card md-select {\n        width: 320px; }\n      :host .content .card .row {\n        margin: 8px 0; }\n      :host .content .card button {\n        margin: 0 0 0 8px;\n        float: left;\n        background: #0088f4; }\n      :host .content .card .fr-getting-started {\n        border: none; }\n\n/deep/ md-tab-group md-tab-header {\n  border-bottom: none !important; }\n  /deep/ md-tab-group md-tab-header md-ink-bar {\n    display: none !important; }\n  /deep/ md-tab-group md-tab-header .mat-tab-labels {\n    display: block; }\n    /deep/ md-tab-group md-tab-header .mat-tab-labels .mat-tab-label {\n      min-width: 0 !important;\n      display: inline-block; }\n      /deep/ md-tab-group md-tab-header .mat-tab-labels .mat-tab-label.mat-tab-label-active {\n        opacity: 1; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 278:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(35)();
// imports


// module
exports.push([module.i, ":host {\r\n    display: block;\r\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 280:
/***/ (function(module, exports) {

module.exports = "<router-outlet></router-outlet>"

/***/ }),

/***/ 281:
/***/ (function(module, exports) {

module.exports = "<div class=\"progress\" *ngIf=\"showProgress\">\r\n  <md-progress-spinner [mode]=\"'indeterminate'\"></md-progress-spinner>\r\n  <span>Installing {{ currentPackage?.displayName }}..</span>\r\n</div>\r\n<div *ngIf=\"ready\">\r\n  <iframe class=\"fr-getting-started\" id=\"frGettingStarted\" [src]=\"remoteInstallerUrl\" width=\"100%\" height=\"300px\"></iframe>\r\n</div>"

/***/ }),

/***/ 282:
/***/ (function(module, exports) {

module.exports = "<div class=\"content\">\r\n    <md-progress-bar [ngStyle]=\"{ opacity: (!ready || loading) ? 1 : 0 }\" [mode]=\"'indeterminate'\"></md-progress-bar>\r\n    <div *ngIf=\"ready\" class=\"card\">\r\n        <div class=\"top-controls\" fxLayout=\"row\" fxLayoutAlign=\"center center\">\r\n            <button md-fab *ngIf=\"template\" (click)=\"persistTemplate()\" [attr.title]=\"'TemplatePicker.Save' | translate\">\r\n                <md-icon>check</md-icon>\r\n            </button>\r\n            <button md-mini-fab class=\"secondary\" *ngIf=\"undoTemplateId !== null\" (click)=\"frame.sxc.manage.contentBlock._cancelTemplateChange()\"\r\n                [attr.title]=\"('TemplatePicker.' + (isContentApp ? 'Cancel' : 'Close')) | translate\">\r\n                <md-icon>close</md-icon>\r\n            </button>\r\n        </div>\r\n        <md-tab-group [(selectedIndex)]=\"tabIndex\">\r\n            <md-tab [label]=\"(isContentApp ? (contentType?.Name || ('TemplatePicker.ContentTypePickerDefault' | translate)) : (app?.name || ('TemplatePicker.AppPickerDefault' | translate)))\">\r\n                <div *ngIf=\"!isContentApp; else contentApp\" class=\"tiles\">\r\n                    <div class=\"tile\" [ngClass]=\"{ active: app?.appId === a.appId }\" [attr.title]=\"a.name\" (click)=\"app?.appId === a.appId ? switchTab() : updateApp(a)\"\r\n                        (dblclick)=\"switchTab()\" *ngFor=\"let a of apps\">\r\n                        <div class=\"bg\">\r\n                            <img *ngIf=\"a.thumbnail !== null && a.thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"a.thumbnail + '?w=176&h=176'\">\r\n                            <div *ngIf=\"a.thumbnail === null || a.thumbnail === ''\" class=\"bg-icon\">\r\n                                <md-icon>star</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title\" [ngClass]=\"{ show: a.thumbnail === null || a.thumbnail === '' }\">\r\n                            <span>{{a.name}}</span>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"tile config\" *ngIf=\"showAdvanced && !isContentApp\" (click)=\"frame.sxc.manage.run('app-import')\" [attr.title]=\"'TemplatePicker.Install' | translate\">\r\n                        <div class=\"bg\">\r\n                            <div class=\"bg-icon\">\r\n                                <md-icon>get_app</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title show\">\r\n                            <span>{{\"TemplatePicker.Install\" | translate}}</span>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"tile config\" *ngIf=\"showAdvanced && !isContentApp\" (click)=\"frame.sxc.manage.run('zone')\" [attr.title]=\"'TemplatePicker.Zone' | translate\">\r\n                        <div class=\"bg\">\r\n                            <div class=\"bg-icon\">\r\n                                <md-icon>apps</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title show\">\r\n                            <span>{{\"TemplatePicker.Zone\" | translate}}</span>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n                <ng-template #contentApp>\r\n                    <div class=\"tiles\">\r\n                        <div class=\"tile\" [ngClass]=\"{ active: contentType?.StaticName === c.StaticName, blocked: !allowContentTypeChange }\" [attr.title]=\"c.Label\"\r\n                            (click)=\"contentType?.StaticName === c.StaticName ? switchTab() : updateContentType(c)\" (dblclick)=\"switchTab()\"\r\n                            *ngFor=\"let c of contentTypes\">\r\n                            <div class=\"bg\">\r\n                                <img *ngIf=\"c.Thumbnail !== null && c.Thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"c.Thumbnail + '?w=176&h=176'\">\r\n                                <div *ngIf=\"c.Thumbnail === null || c.Thumbnail === ''\" class=\"bg-icon\">\r\n                                    <md-icon>bubble_chart</md-icon>\r\n                                </div>\r\n                            </div>\r\n                            <div class=\"title\" [ngClass]=\"{ show: c.Thumbnail === null || c.Thumbnail === '' }\">\r\n                                <span>{{c.Label}}</span>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                </ng-template>\r\n            </md-tab>\r\n            <md-tab *ngIf=\"isContentApp ? contentType : app\" [label]=\"('TemplatePicker.ChangeView' | translate) + '(' + templates.length + ')'\">\r\n                <div class=\"tiles\">\r\n                    <md-spinner class=\"templates-spinner\" *ngIf=\"loadingTemplates\"></md-spinner>\r\n                    <div class=\"tile\" [ngClass]=\"{ active: template?.TemplateId === t.TemplateId }\" [attr.title]=\"t.Name\" (click)=\"updateTemplateSubject.next({ template: t })\"\r\n                        *ngFor=\"let t of templates\">\r\n                        <div class=\"bg\">\r\n                            <img *ngIf=\"t.Thumbnail !== null && t.Thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"t.Thumbnail + '?w=176&h=176'\">\r\n                            <div *ngIf=\"t.Thumbnail === null || t.Thumbnail === ''\" class=\"bg-icon\">\r\n                                <md-icon *ngIf=\"isContentApp\">view_carousel</md-icon>\r\n                                <md-icon *ngIf=\"!isContentApp\">view_quilt</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title\" [ngClass]=\"{ show: t.Thumbnail === null || t.Thumbnail === '' }\">\r\n                            <span>{{t.Name}}</span>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"tile config\" *ngIf=\"showAdvanced && !isContentApp && app?.appId !== null\" (click)=\"frame.sxc.manage.run('app')\"\r\n                        [attr.title]=\"'TemplatePicker.App' | translate\">\r\n                        <div class=\"bg\">\r\n                            <div class=\"bg-icon\">\r\n                                <md-icon>settings</md-icon>\r\n                            </div>\r\n                        </div>\r\n                        <div class=\"title show\">\r\n                            <span>{{\"TemplatePicker.App\" | translate}}</span>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </md-tab>\r\n        </md-tab-group>\r\n        <app-installer *ngIf=\"showInstaller\" [isContentApp]=\"isContentApp\"></app-installer>\r\n    </div>\r\n</div>"

/***/ }),

/***/ 528:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(186);


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
        var params = route.snapshot.queryParams;
        this.sxc = $2sxc(params['mid'], params['cbid']);
    }
    return $2sxcService;
}());
$2sxcService = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* ActivatedRoute */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* ActivatedRoute */]) === "function" && _a || Object])
], $2sxcService);

var _a;
//# sourceMappingURL=$2sxc.service.js.map

/***/ }),

/***/ 75:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_add_operator_map__ = __webpack_require__(18);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_add_operator_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_rxjs_add_operator_map__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__(54);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_http__ = __webpack_require__(29);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__ = __webpack_require__(6);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_rxjs_Subject__);
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__angular_http__["a" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3__angular_http__["a" /* Http */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _b || Object])
], ModuleApiService);

var _a, _b;
//# sourceMappingURL=module-api.service.js.map

/***/ })

},[528]);
//# sourceMappingURL=main.bundle.js.map