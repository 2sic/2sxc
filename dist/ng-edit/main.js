(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ "./src/$$_lazy_route_resource lazy recursive":
/*!**********************************************************!*\
  !*** ./src/$$_lazy_route_resource lazy namespace object ***!
  \**********************************************************/
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
webpackEmptyAsyncContext.id = "./src/$$_lazy_route_resource lazy recursive";

/***/ }),

/***/ "./src/app/app.component.css":
/*!***********************************!*\
  !*** ./src/app/app.component.css ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/app.component.html":
/*!************************************!*\
  !*** ./src/app/app.component.html ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<router-outlet></router-outlet>"

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
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var AppComponent = /** @class */ (function () {
    function AppComponent() {
        // constructor(translate: TranslateService) {
        // // this language will be used as a fallback when a translation isn't found in the current language
        // translate.setDefaultLang('en');
        this.title = 'app';
        // // the lang to use, if the lang isn't available, it will use the current loader to get them
        // translate.use('en');
        // this language will be used as a fallback when a translation isn't found in the current language
        // translate.setDefaultLang('en');
        // the lang to use, if the lang isn't available, it will use the current loader to get them
        // translate.use('en');
        // translate.use('fr');
    }
    AppComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-root',
            template: __webpack_require__(/*! ./app.component.html */ "./src/app/app.component.html"),
            styles: [__webpack_require__(/*! ./app.component.css */ "./src/app/app.component.css")]
        }),
        __metadata("design:paramtypes", [])
    ], AppComponent);
    return AppComponent;
}());



/***/ }),

/***/ "./src/app/app.module.ts":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: createTranslateLoader, AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "createTranslateLoader", function() { return createTranslateLoader; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/platform-browser */ "./node_modules/@angular/platform-browser/fesm5/platform-browser.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var _ngrx_store_devtools__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @ngrx/store-devtools */ "./node_modules/@ngrx/store-devtools/fesm5/store-devtools.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @ngrx/effects */ "./node_modules/@ngrx/effects/fesm5/effects.js");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/platform-browser/animations */ "./node_modules/@angular/platform-browser/fesm5/animations.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/router */ "./node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _app_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./app.component */ "./src/app/app.component.ts");
/* harmony import */ var _shared_services_item_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./shared/services/item.service */ "./src/app/shared/services/item.service.ts");
/* harmony import */ var _shared_services_content_type_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./shared/services/content-type.service */ "./src/app/shared/services/content-type.service.ts");
/* harmony import */ var _shared_store__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./shared/store */ "./src/app/shared/store/index.ts");
/* harmony import */ var _shared_services_language_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./shared/services/language.service */ "./src/app/shared/services/language.service.ts");
/* harmony import */ var _shared_services_script_service__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./shared/services/script.service */ "./src/app/shared/services/script.service.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm5/ngx-translate-core.js");
/* harmony import */ var _ngx_translate_http_loader__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! @ngx-translate/http-loader */ "./node_modules/@ngx-translate/http-loader/esm5/ngx-translate-http-loader.js");
/* harmony import */ var _eav_material_controls_adam_adam_service__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ./eav-material-controls/adam/adam.service */ "./src/app/eav-material-controls/adam/adam.service.ts");
/* harmony import */ var _shared_services_svc_creator_service__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ./shared/services/svc-creator.service */ "./src/app/shared/services/svc-creator.service.ts");
/* harmony import */ var _shared_services_feature_service__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ./shared/services/feature.service */ "./src/app/shared/services/feature.service.ts");
/* harmony import */ var _shared_services_dnn_bridge_service__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./shared/services/dnn-bridge.service */ "./src/app/shared/services/dnn-bridge.service.ts");
/* harmony import */ var _shared_services_entity_service__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ./shared/services/entity.service */ "./src/app/shared/services/entity.service.ts");
/* harmony import */ var _shared_interceptors_interceptors__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ./shared/interceptors/interceptors */ "./src/app/shared/interceptors/interceptors.ts");
/* harmony import */ var _shared_services_input_type_service__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ./shared/services/input-type.service */ "./src/app/shared/services/input-type.service.ts");
/* harmony import */ var _shared_services_eav_admin_ui_service__WEBPACK_IMPORTED_MODULE_24__ = __webpack_require__(/*! ./shared/services/eav-admin-ui.service */ "./src/app/shared/services/eav-admin-ui.service.ts");
/* harmony import */ var _eav_item_dialog_dialogs_open_multi_item_dialog_open_multi_item_dialog_component__WEBPACK_IMPORTED_MODULE_25__ = __webpack_require__(/*! ./eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component */ "./src/app/eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component.ts");
/* harmony import */ var _eav_item_dialog_eav_item_dialog_module__WEBPACK_IMPORTED_MODULE_26__ = __webpack_require__(/*! ./eav-item-dialog/eav-item-dialog.module */ "./src/app/eav-item-dialog/eav-item-dialog.module.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};



























var routes = [
    {
        path: '**',
        component: _eav_item_dialog_dialogs_open_multi_item_dialog_open_multi_item_dialog_component__WEBPACK_IMPORTED_MODULE_25__["OpenMultiItemDialogComponent"]
    }
];
function createTranslateLoader(http) {
    return new _ngx_translate_http_loader__WEBPACK_IMPORTED_MODULE_16__["TranslateHttpLoader"](http, './../i18n/ng-edit/', '.json');
}
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _app_component__WEBPACK_IMPORTED_MODULE_8__["AppComponent"]
            ],
            imports: [
                _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__["BrowserModule"],
                // DropzoneModule,
                _ngrx_store__WEBPACK_IMPORTED_MODULE_2__["StoreModule"].forRoot({}, { metaReducers: _shared_store__WEBPACK_IMPORTED_MODULE_11__["metaReducers"] }),
                _ngrx_effects__WEBPACK_IMPORTED_MODULE_5__["EffectsModule"].forRoot([]),
                _ngrx_store_devtools__WEBPACK_IMPORTED_MODULE_3__["StoreDevtoolsModule"].instrument({ maxAge: 25 }),
                _angular_common_http__WEBPACK_IMPORTED_MODULE_4__["HttpClientModule"],
                _angular_router__WEBPACK_IMPORTED_MODULE_7__["RouterModule"].forRoot(routes),
                _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_6__["BrowserAnimationsModule"],
                _eav_item_dialog_eav_item_dialog_module__WEBPACK_IMPORTED_MODULE_26__["EavItemDialogModule"],
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_15__["TranslateModule"].forRoot({
                    loader: {
                        provide: _ngx_translate_core__WEBPACK_IMPORTED_MODULE_15__["TranslateLoader"],
                        useFactory: (createTranslateLoader),
                        deps: [_angular_common_http__WEBPACK_IMPORTED_MODULE_4__["HttpClient"]]
                    }
                })
            ],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_7__["RouterModule"]],
            providers: [
                _shared_services_item_service__WEBPACK_IMPORTED_MODULE_9__["ItemService"],
                _shared_services_content_type_service__WEBPACK_IMPORTED_MODULE_10__["ContentTypeService"],
                _shared_services_input_type_service__WEBPACK_IMPORTED_MODULE_23__["InputTypeService"],
                _shared_services_language_service__WEBPACK_IMPORTED_MODULE_12__["LanguageService"],
                _shared_services_script_service__WEBPACK_IMPORTED_MODULE_13__["ScriptLoaderService"],
                _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_14__["EavService"],
                _eav_material_controls_adam_adam_service__WEBPACK_IMPORTED_MODULE_17__["AdamService"],
                _shared_services_svc_creator_service__WEBPACK_IMPORTED_MODULE_18__["SvcCreatorService"],
                _shared_services_feature_service__WEBPACK_IMPORTED_MODULE_19__["FeatureService"],
                _shared_services_dnn_bridge_service__WEBPACK_IMPORTED_MODULE_20__["DnnBridgeService"],
                _shared_services_entity_service__WEBPACK_IMPORTED_MODULE_21__["EntityService"],
                _shared_services_eav_admin_ui_service__WEBPACK_IMPORTED_MODULE_24__["EavAdminUiService"],
                {
                    provide: _angular_common_http__WEBPACK_IMPORTED_MODULE_4__["HTTP_INTERCEPTORS"],
                    useClass: _shared_interceptors_interceptors__WEBPACK_IMPORTED_MODULE_22__["HeaderInterceptor"],
                    multi: true
                },
            ],
            bootstrap: [_app_component__WEBPACK_IMPORTED_MODULE_8__["AppComponent"]]
        })
    ], AppModule);
    return AppModule;
}());



/***/ }),

/***/ "./src/app/eav-dynamic-form/components/eav-field/eav-field.directive.ts":
/*!******************************************************************************!*\
  !*** ./src/app/eav-dynamic-form/components/eav-field/eav-field.directive.ts ***!
  \******************************************************************************/
/*! exports provided: EavFieldDirective */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavFieldDirective", function() { return EavFieldDirective; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _shared_models__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../shared/models */ "./src/app/shared/models/index.ts");
/* harmony import */ var _shared_services_script_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../shared/services/script.service */ "./src/app/shared/services/script.service.ts");
/* harmony import */ var _shared_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../shared/constants */ "./src/app/shared/constants/index.ts");
/* harmony import */ var _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../shared/constants/type-constants */ "./src/app/shared/constants/type-constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var EavFieldDirective = /** @class */ (function () {
    function EavFieldDirective(resolver, container, scriptLoaderService) {
        this.resolver = resolver;
        this.container = container;
        this.scriptLoaderService = scriptLoaderService;
        this.window = window;
        this.addOnList = [];
        this.externalCommponentRefList = [];
    }
    EavFieldDirective.prototype.ngOnInit = function () {
        var _this = this;
        // Clear lists and container
        this.addOnList = [];
        this.externalCommponentRefList = [];
        this.container.clear();
        this.config.forEach(function (controlConfiguration) {
            console.log('create controlConfiguration', controlConfiguration);
            _this.createFieldOrGroup(_this.container, controlConfiguration);
        });
    };
    /**
     * create all child fields and groups from fieldConfig in container
     * @param container
     * @param fieldConfig
     */
    EavFieldDirective.prototype.createFieldOrGroup = function (container, fieldConfig) {
        if (fieldConfig.fieldGroup) {
            // this.createGroupComponents(container, fieldConfig, <FormGroup>group.controls[fieldConfig.name]);
            this.createGroupComponents(container, fieldConfig);
        }
        else {
            if (fieldConfig.type === _shared_constants__WEBPACK_IMPORTED_MODULE_4__["InputTypesConstants"].external) {
                console.log('create external');
                this.createExternalComponent(container, fieldConfig);
            }
            else {
                console.log('create non external', fieldConfig.type);
                // this.createFieldComponent(container, fieldConfig, group);
                this.createComponent(container, fieldConfig);
            }
        }
    };
    /**
     * Create group components with group wrappers in container
     * @param container
     * @param fieldConfig
     */
    EavFieldDirective.prototype.createGroupComponents = function (container, fieldConfig) {
        var _this = this;
        if (fieldConfig.wrappers) {
            container = this.createComponentWrappers(container, fieldConfig, fieldConfig.wrappers);
        }
        fieldConfig.fieldGroup.forEach(function (controlConfiguration) {
            _this.createFieldOrGroup(container, controlConfiguration);
        });
    };
    /**
     * Create component and component wrappers if component exist
     * @param container
     * @param fieldConfig
     */
    EavFieldDirective.prototype.createComponent = function (container, fieldConfig) {
        if (fieldConfig.wrappers) {
            container = this.createComponentWrappers(container, fieldConfig, fieldConfig.wrappers);
        }
        var componentType = this.readComponentType(fieldConfig.type);
        var inputTypeAnnotations = Reflect.getMetadata('inputTypeAnnotations', componentType);
        // console.log('reading wrapper:', inputTypeAnnotations);
        // if inputTypeAnnotations of componentType exist then create component
        if (inputTypeAnnotations) {
            if (inputTypeAnnotations.wrapper) {
                container = this.createComponentWrappers(container, fieldConfig, inputTypeAnnotations.wrapper);
            }
            var factory = this.resolver.resolveComponentFactory(componentType);
            var ref = container.createComponent(factory);
            Object.assign(ref.instance, {
                group: this.group,
                config: fieldConfig,
            });
            return ref;
        }
        return null;
    };
    /**
     * Create and register external commponent
     * @param container
     * @param fieldConfig
     */
    EavFieldDirective.prototype.createExternalComponent = function (container, fieldConfig) {
        // first create component container - then load script
        var externalComponentRef = this.createComponent(container, fieldConfig);
        this.externalCommponentRefList[fieldConfig.name] = externalComponentRef;
        if (this.window.addOn === undefined) {
            // this.window.addOn = [];
            this.window.addOn = new _shared_models__WEBPACK_IMPORTED_MODULE_2__["CustomInputType"](this.registerExternalComponent.bind(this));
        }
        // // TODO: read data from config
        // Start loading all external dependencies (start with css). This method recursively load all dependencies.
        this.loadExternalnputType(0, fieldConfig.name, 'tinymce-wysiwyg', ['assets/script/tinymce-wysiwyg/src/tinymce-wysiwyg.css'], ['http://cdn.tinymce.com/4.6/tinymce.min.js', 'assets/script/tinymce-wysiwyg/src/tinymce-wysiwyg.js'], 
        // ['http://cdn.tinymce.com/4.6/tinymce.min.js', 'assets/script/tinymce-wysiwyg/dist/tinymce-wysiwyg.min.js'],
        _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_5__["FileTypeConstants"].css);
    };
    EavFieldDirective.prototype.loadExternalnputType = function (increment, name, type, styles, scripts, fileType) {
        var _this = this;
        var scriptModel = {
            name: "" + fileType + name + increment,
            filePath: (fileType === _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_5__["FileTypeConstants"].css) ? styles[increment] : scripts[increment],
            loaded: false
        };
        this.scriptLoaderService.load(scriptModel, fileType).subscribe(function (s) {
            if (s.loaded) {
                increment++;
                var nextScript = (fileType === _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_5__["FileTypeConstants"].css) ? styles[increment] : scripts[increment];
                if (nextScript) {
                    console.log('nextScript', name);
                    _this.loadExternalnputType(increment, name, type, styles, scripts, fileType);
                }
                else if (fileType === _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_5__["FileTypeConstants"].css) {
                    console.log('nextScript css', name);
                    _this.loadExternalnputType(0, name, type, styles, scripts, _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_5__["FileTypeConstants"].javaScript);
                }
                else { // when scripts load is finish then call registered factory
                    console.log('nextScript facrory', type);
                    _this.loadExternalFactoryToComponent(name, type);
                }
            }
        });
    };
    /**
     * First read component reference with NAME and external component (factory) with TYPE,
     * and then add external component (factory) to component (input type) reference.
     * @param name
     * @param type
     */
    EavFieldDirective.prototype.loadExternalFactoryToComponent = function (name, type) {
        var externalCommponentRef = this.externalCommponentRefList[name];
        var factory = this.addOnList[type];
        console.log('loaded name factory', this.addOnList);
        if (externalCommponentRef && factory) {
            console.log('loaded name', name);
            console.log('loaded this.externalCommponentRefList[name]', this.externalCommponentRefList);
            console.log('loaded factory', factory);
            Object.assign(externalCommponentRef.instance, {
                factory: Object.create(factory)
            });
        }
    };
    /**
     * When external component is registered on load - this method add that component to list
     * @param factory
     */
    EavFieldDirective.prototype.registerExternalComponent = function (factory) {
        this.addOnList[factory.name] = factory;
    };
    /**
     * Read component type by selector with ComponentFactoryResolver
     * @param selector
     */
    EavFieldDirective.prototype.readComponentType = function (selector) {
        var factories = Array.from(this.resolver['_factories'].values());
        var componentType = factories.find(function (x) { return x.selector === selector; })['componentType'];
        return componentType;
    };
    /**
     * Create wrappers in container
     * @param container
     * @param fieldConfig
     * @param wrappers
     */
    EavFieldDirective.prototype.createComponentWrappers = function (container, fieldConfig, wrappers) {
        var _this = this;
        wrappers.forEach(function (wrapperName) {
            container = _this.createWrapper(container, fieldConfig, wrapperName);
        });
        return container;
    };
    /**
     * Create wrapper in container
     * @param container
     * @param fieldConfig
     * @param wrapper
     */
    EavFieldDirective.prototype.createWrapper = function (container, fieldConfig, wrapper) {
        var componentType = this.readComponentType(wrapper);
        // create component from component type
        var componentFactory = this.resolver.resolveComponentFactory(componentType);
        var ref = container.createComponent(componentFactory);
        Object.assign(ref.instance, {
            group: this.group,
            config: fieldConfig
        });
        return ref.instance.fieldComponent;
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Array)
    ], EavFieldDirective.prototype, "config", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", _angular_forms__WEBPACK_IMPORTED_MODULE_1__["FormGroup"])
    ], EavFieldDirective.prototype, "group", void 0);
    EavFieldDirective = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Directive"])({
            selector: '[appEavField]'
        }),
        __metadata("design:paramtypes", [_angular_core__WEBPACK_IMPORTED_MODULE_0__["ComponentFactoryResolver"],
            _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"],
            _shared_services_script_service__WEBPACK_IMPORTED_MODULE_3__["ScriptLoaderService"]])
    ], EavFieldDirective);
    return EavFieldDirective;
}());



/***/ }),

/***/ "./src/app/eav-dynamic-form/components/eav-form/eav-form.component.css":
/*!*****************************************************************************!*\
  !*** ./src/app/eav-dynamic-form/components/eav-form/eav-form.component.css ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-dynamic-form/components/eav-form/eav-form.component.html":
/*!******************************************************************************!*\
  !*** ./src/app/eav-dynamic-form/components/eav-form/eav-form.component.html ***!
  \******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form class=\"dynamic-form\" [formGroup]=\"form\" (ngSubmit)=\"save($event)\" #dynamicForm=\"ngForm\">\r\n  <ng-container appEavField [config]=\"config\" [group]=\"form\">\r\n  </ng-container>\r\n</form>\r\n\r\n<span *ngIf=\"form && debugEnviroment\">\r\n  <button mat-icon-button type=\"button\" (click)=\"showDebugItems = !showDebugItems\">\r\n    <i class=\"eav-icon-flash\"></i>\r\n  </button>\r\n</span>\r\n<div *ngIf=\"form && debugEnviroment && showDebugItems\">\r\n  <div>\r\n    isValid: {{ form.valid }}<br />\r\n  </div>\r\n  <pre>{{ form.value | json }}</pre>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-dynamic-form/components/eav-form/eav-form.component.ts":
/*!****************************************************************************!*\
  !*** ./src/app/eav-dynamic-form/components/eav-form/eav-form.component.ts ***!
  \****************************************************************************/
/*! exports provided: EavFormComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavFormComponent", function() { return EavFormComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../environments/environment */ "./src/environments/environment.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var EavFormComponent = /** @class */ (function () {
    function EavFormComponent(formBuilder) {
        var _this = this;
        this.formBuilder = formBuilder;
        this.config = [];
        this.submit = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        this.formValueChange = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        this.form = new _angular_forms__WEBPACK_IMPORTED_MODULE_1__["FormGroup"]({});
        this.showDebugItems = false;
        this.subscriptions = [];
        /**
         * Check is value in form changed
         *
        */
        this.valueIsChanged = function (values) {
            var valueIsChanged = false;
            console.log('[Test Disabled] VALUECHANGED values', values);
            console.log('[Test Disabled] VALUECHANGED form values', _this.form.value);
            Object.keys(_this.form.value).forEach(function (valueKey) {
                if (_this.form.value[valueKey] !== values[valueKey]) {
                    valueIsChanged = true;
                }
            });
            console.log('[Test Disabled] VALUECHANGED', valueIsChanged);
            return valueIsChanged;
        };
    }
    Object.defineProperty(EavFormComponent.prototype, "changes", {
        get: function () { return this.form.valueChanges; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EavFormComponent.prototype, "valid", {
        get: function () { return this.form.valid; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EavFormComponent.prototype, "value", {
        get: function () { return this.form.value; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EavFormComponent.prototype, "debugEnviroment", {
        get: function () {
            return !_environments_environment__WEBPACK_IMPORTED_MODULE_2__["environment"].production;
        },
        enumerable: true,
        configurable: true
    });
    EavFormComponent.prototype.ngOnInit = function () {
        var _this = this;
        // let group = this.formBuilder.group({});
        this.createControlsInFormGroup(this.config);
        this.subscriptions.push(this.form.valueChanges.subscribe(function (val) {
            // if (this.form.valid) {
            // this.formErrors = this.FormService.validateForm(this.form, this.formErrors, true);
            _this.formValueChange.emit(val);
            // }
        }));
    };
    EavFormComponent.prototype.ngOnChanges = function () {
        // console.log('ngOnChanges EavFormComponent');
    };
    EavFormComponent.prototype.ngOnDestroy = function () {
        this.subscriptions.forEach(function (subscriber) { return subscriber.unsubscribe(); });
    };
    /**
     * Create form from configuration
     * @param fieldConfigArray
     */
    EavFormComponent.prototype.createControlsInFormGroup = function (fieldConfigArray) {
        var _this = this;
        // const group = this.formBuilder.group({});
        fieldConfigArray.forEach(function (fieldConfig) {
            if (fieldConfig.fieldGroup) {
                _this.createControlsInFormGroup(fieldConfig.fieldGroup);
            }
            else {
                _this.form.addControl(fieldConfig.name, _this.createControl(fieldConfig));
            }
        });
        return this.form;
    };
    /**
     *  Create form control
     * @param config
     */
    EavFormComponent.prototype.createControl = function (config) {
        // tslint:disable-next-line:prefer-const
        var disabled = config.disabled, validation = config.validation, value = config.value;
        return this.formBuilder.control({ disabled: disabled, value: value }, validation);
    };
    // handleSubmit(event: Event) {
    //   console.log('Submit');
    //   event.preventDefault();
    //   event.stopPropagation();
    //   this.submit.emit(this.value);
    // }
    EavFormComponent.prototype.save = function (event) {
        console.log('form save', event);
        // Use this to emit value out
        this.submit.emit(this.value);
    };
    EavFormComponent.prototype.submitOutside = function () {
        this.dynamicForm.ngSubmit.emit(this.value);
    };
    EavFormComponent.prototype.setDisabled = function (name, disable, emitEvent) {
        if (this.form.controls[name]) {
            if (disable) {
                this.form.controls[name].disable({ emitEvent: emitEvent });
            }
            else {
                this.form.controls[name].enable({ emitEvent: emitEvent });
            }
        }
    };
    /**
     * Set form control value
     * @param name
     * @param value
     * @param emitEvent If emitEvent is true, this change will cause a valueChanges event on the FormControl
     * to be emitted. This defaults to true (as it falls through to updateValueAndValidity).
     */
    EavFormComponent.prototype.setValue = function (name, value, emitEvent) {
        if (value !== this.form.controls[name].value) {
            console.log('CHANGE' + name + ' from value: ' + this.form.controls[name].value + ' to ' + value);
            this.form.controls[name].setValue(value, { emitEvent: emitEvent });
        }
    };
    /**
     * Patch values to formGroup. It accepts an object with control names as keys, and will do it's best to
     * match the values to the correct controls in the group.
     * @param values
     * @param emitEvent If emitEvent is true, this change will cause a valueChanges event on the FormGroup
     * to be emitted. This defaults to true (as it falls through to updateValueAndValidity).
     */
    EavFormComponent.prototype.patchValue = function (values, emitEvent) {
        // if (this.valueIsChanged(values)) {
        // console.log('value patchValue');
        this.form.patchValue(values, { emitEvent: emitEvent });
        // }
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('dynamicForm'),
        __metadata("design:type", _angular_forms__WEBPACK_IMPORTED_MODULE_1__["FormGroupDirective"])
    ], EavFormComponent.prototype, "dynamicForm", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Array)
    ], EavFormComponent.prototype, "config", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"])(),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"])
    ], EavFormComponent.prototype, "submit", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"])(),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"])
    ], EavFormComponent.prototype, "formValueChange", void 0);
    EavFormComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            exportAs: 'appEavForm',
            template: __webpack_require__(/*! ./eav-form.component.html */ "./src/app/eav-dynamic-form/components/eav-form/eav-form.component.html"),
            selector: 'app-eav-form',
            styles: [__webpack_require__(/*! ./eav-form.component.css */ "./src/app/eav-dynamic-form/components/eav-form/eav-form.component.css")]
        }),
        __metadata("design:paramtypes", [_angular_forms__WEBPACK_IMPORTED_MODULE_1__["FormBuilder"]])
    ], EavFormComponent);
    return EavFormComponent;
}());



/***/ }),

/***/ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts":
/*!*********************************************************************!*\
  !*** ./src/app/eav-dynamic-form/decorators/input-type.decorator.ts ***!
  \*********************************************************************/
/*! exports provided: InputType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InputType", function() { return InputType; });
/* harmony import */ var reflect_metadata__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! reflect-metadata */ "./node_modules/reflect-metadata/Reflect.js");
/* harmony import */ var reflect_metadata__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(reflect_metadata__WEBPACK_IMPORTED_MODULE_0__);
// import 'zone.js';

function InputType(annotation) {
    return function (target) {
        // Object.defineProperty(target.prototype, 'wrapper', { value: () => annotation.wrapper });
        // const metadata = new Component(annotation);
        Reflect.defineMetadata('inputTypeAnnotations', annotation, target);
    };
}


/***/ }),

/***/ "./src/app/eav-dynamic-form/eav-dynamic-form.module.ts":
/*!*************************************************************!*\
  !*** ./src/app/eav-dynamic-form/eav-dynamic-form.module.ts ***!
  \*************************************************************/
/*! exports provided: EavDynamicFormModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavDynamicFormModule", function() { return EavDynamicFormModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm5/common.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _components_eav_field_eav_field_directive__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./components/eav-field/eav-field.directive */ "./src/app/eav-dynamic-form/components/eav-field/eav-field.directive.ts");
/* harmony import */ var _components_eav_form_eav_form_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./components/eav-form/eav-form.component */ "./src/app/eav-dynamic-form/components/eav-form/eav-form.component.ts");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};






var EavDynamicFormModule = /** @class */ (function () {
    function EavDynamicFormModule() {
    }
    EavDynamicFormModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_1__["CommonModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_2__["ReactiveFormsModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatFormFieldModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatCheckboxModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatInputModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatSelectModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatDatepickerModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatNativeDateModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatCardModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_5__["MatIconModule"],
            ],
            declarations: [
                _components_eav_field_eav_field_directive__WEBPACK_IMPORTED_MODULE_3__["EavFieldDirective"],
                _components_eav_form_eav_form_component__WEBPACK_IMPORTED_MODULE_4__["EavFormComponent"],
            ],
            exports: [
                _components_eav_form_eav_form_component__WEBPACK_IMPORTED_MODULE_4__["EavFormComponent"]
            ],
        })
    ], EavDynamicFormModule);
    return EavDynamicFormModule;
}());



/***/ }),

/***/ "./src/app/eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component.html":
/*!******************************************************************************************************!*\
  !*** ./src/app/eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component.html ***!
  \******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component.ts":
/*!****************************************************************************************************!*\
  !*** ./src/app/eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component.ts ***!
  \****************************************************************************************************/
/*! exports provided: OpenMultiItemDialogComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "OpenMultiItemDialogComponent", function() { return OpenMultiItemDialogComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _multi_item_edit_form_multi_item_edit_form_component__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../multi-item-edit-form/multi-item-edit-form.component */ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.ts");
/* harmony import */ var _shared_services_eav_admin_ui_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../shared/services/eav-admin-ui.service */ "./src/app/shared/services/eav-admin-ui.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




/**
 * This component only open multi-item-dailog component in mat-dialog window
 */
var OpenMultiItemDialogComponent = /** @class */ (function () {
    function OpenMultiItemDialogComponent(dialog, eavAdminUiService) {
        var _this = this;
        this.dialog = dialog;
        this.eavAdminUiService = eavAdminUiService;
        // Open dialog
        var dialogRef = this.eavAdminUiService.openItemEditWithContent(this.dialog, _multi_item_edit_form_multi_item_edit_form_component__WEBPACK_IMPORTED_MODULE_2__["MultiItemEditFormComponent"]);
        // Close dialog
        dialogRef.afterClosed().subscribe(function (result) {
            _this.afterClosedDialog();
        });
    }
    OpenMultiItemDialogComponent.prototype.ngOnInit = function () { };
    /**
     * Triggered after dialog is closed
     */
    OpenMultiItemDialogComponent.prototype.afterClosedDialog = function () {
        console.log('The dialog was closed');
        // find and remove iframe
        // TODO: this is not good - need to find better solution
        var iframes = window.parent.frames.document.getElementsByTagName('iframe');
        if (iframes[0] && iframes[0].parentElement) {
            iframes[0].parentElement.remove();
        }
    };
    OpenMultiItemDialogComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-open-multi-item-dialog',
            template: __webpack_require__(/*! ./open-multi-item-dialog.component.html */ "./src/app/eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component.html"),
        }),
        __metadata("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_1__["MatDialog"],
            _shared_services_eav_admin_ui_service__WEBPACK_IMPORTED_MODULE_3__["EavAdminUiService"]])
    ], OpenMultiItemDialogComponent);
    return OpenMultiItemDialogComponent;
}());



/***/ }),

/***/ "./src/app/eav-item-dialog/eav-item-dialog.module.ts":
/*!***********************************************************!*\
  !*** ./src/app/eav-item-dialog/eav-item-dialog.module.ts ***!
  \***********************************************************/
/*! exports provided: EavItemDialogModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavItemDialogModule", function() { return EavItemDialogModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm5/common.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/router */ "./node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @ngrx/effects */ "./node_modules/@ngrx/effects/fesm5/effects.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _multi_item_edit_form_multi_item_edit_form_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./multi-item-edit-form/multi-item-edit-form.component */ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.ts");
/* harmony import */ var _eav_dynamic_form_eav_dynamic_form_module__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../eav-dynamic-form/eav-dynamic-form.module */ "./src/app/eav-dynamic-form/eav-dynamic-form.module.ts");
/* harmony import */ var _item_edit_form_item_edit_form_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./item-edit-form/item-edit-form.component */ "./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.ts");
/* harmony import */ var _eav_material_controls_eav_material_controls_module__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../eav-material-controls/eav-material-controls.module */ "./src/app/eav-material-controls/eav-material-controls.module.ts");
/* harmony import */ var _shared_store__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../shared/store */ "./src/app/shared/store/index.ts");
/* harmony import */ var _shared_effects_item_effects__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../shared/effects/item.effects */ "./src/app/shared/effects/item.effects.ts");
/* harmony import */ var _shared_effects_content_type_effects__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../shared/effects/content-type.effects */ "./src/app/shared/effects/content-type.effects.ts");
/* harmony import */ var _shared_effects_eav_effects__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../shared/effects/eav.effects */ "./src/app/shared/effects/eav.effects.ts");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm5/ngx-translate-core.js");
/* harmony import */ var _dialogs_open_multi_item_dialog_open_multi_item_dialog_component__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./dialogs/open-multi-item-dialog/open-multi-item-dialog.component */ "./src/app/eav-item-dialog/dialogs/open-multi-item-dialog/open-multi-item-dialog.component.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

















var EavItemDialogModule = /** @class */ (function () {
    function EavItemDialogModule() {
    }
    EavItemDialogModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            declarations: [
                _multi_item_edit_form_multi_item_edit_form_component__WEBPACK_IMPORTED_MODULE_7__["MultiItemEditFormComponent"],
                _item_edit_form_item_edit_form_component__WEBPACK_IMPORTED_MODULE_9__["ItemEditFormComponent"],
                _dialogs_open_multi_item_dialog_open_multi_item_dialog_component__WEBPACK_IMPORTED_MODULE_16__["OpenMultiItemDialogComponent"]
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_1__["CommonModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatButtonModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatCheckboxModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatInputModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatSelectModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_5__["ReactiveFormsModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatDatepickerModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatNativeDateModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatCardModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatMenuModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatSnackBarModule"],
                _eav_dynamic_form_eav_dynamic_form_module__WEBPACK_IMPORTED_MODULE_8__["EavDynamicFormModule"],
                _eav_material_controls_eav_material_controls_module__WEBPACK_IMPORTED_MODULE_10__["EavMaterialControlsModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_6__["MatDialogModule"],
                _ngrx_store__WEBPACK_IMPORTED_MODULE_2__["StoreModule"].forFeature('eavItemDialog', _shared_store__WEBPACK_IMPORTED_MODULE_11__["reducers"]),
                _ngrx_effects__WEBPACK_IMPORTED_MODULE_4__["EffectsModule"].forFeature([_shared_effects_item_effects__WEBPACK_IMPORTED_MODULE_12__["ItemEffects"], _shared_effects_content_type_effects__WEBPACK_IMPORTED_MODULE_13__["ContentTypeEffects"], _shared_effects_eav_effects__WEBPACK_IMPORTED_MODULE_14__["EavEffects"]]),
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_15__["TranslateModule"].forChild()
            ],
            entryComponents: [
                _multi_item_edit_form_multi_item_edit_form_component__WEBPACK_IMPORTED_MODULE_7__["MultiItemEditFormComponent"]
            ],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_3__["RouterModule"]],
            providers: [],
        })
    ], EavItemDialogModule);
    return EavItemDialogModule;
}());



/***/ }),

/***/ "./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.css":
/*!*****************************************************************************!*\
  !*** ./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.css ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".button-top-right {\r\n\tposition: absolute;\r\n  right: 0;\r\n  top: 0;\r\n}"

/***/ }),

/***/ "./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.html":
/*!******************************************************************************!*\
  !*** ./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.html ***!
  \******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"(contentType$ | async)\">\r\n    <!-- <button mat-icon-button (click)=\"deleteItem()\" class=\"button-top-right\">\r\n        <mat-icon class=\"mat-24\">close</mat-icon>\r\n    </button> -->\r\n\r\n    <app-eav-form [config]=\"itemFields$ | async\" (submit)=\"submit($event)\" (formValueChange)=\"formValueChange($event)\">\r\n    </app-eav-form>\r\n    <!-- <button mat-raised-button type=\"submit\" class=\"btn btn-default submit-button\" (click)=\"changeThis()\">change</button> -->\r\n</div>"

/***/ }),

/***/ "./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.ts":
/*!****************************************************************************!*\
  !*** ./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.ts ***!
  \****************************************************************************/
/*! exports provided: ItemEditFormComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ItemEditFormComponent", function() { return ItemEditFormComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _eav_dynamic_form_components_eav_form_eav_form_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../eav-dynamic-form/components/eav-form/eav-form.component */ "./src/app/eav-dynamic-form/components/eav-form/eav-form.component.ts");
/* harmony import */ var _shared_models_eav__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/models/eav */ "./src/app/shared/models/eav/index.ts");
/* harmony import */ var _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/constants/input-types-constants */ "./src/app/shared/constants/input-types-constants.ts");
/* harmony import */ var _shared_services_item_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../shared/services/item.service */ "./src/app/shared/services/item.service.ts");
/* harmony import */ var _shared_services_content_type_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../shared/services/content-type.service */ "./src/app/shared/services/content-type.service.ts");
/* harmony import */ var _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../../shared/helpers/localization-helper */ "./src/app/shared/helpers/localization-helper.ts");
/* harmony import */ var _eav_material_controls_validators_validation_helper__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../../eav-material-controls/validators/validation-helper */ "./src/app/eav-material-controls/validators/validation-helper.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @ngrx/effects */ "./node_modules/@ngrx/effects/fesm5/effects.js");
/* harmony import */ var _shared_store_actions_item_actions__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../../shared/store/actions/item.actions */ "./src/app/shared/store/actions/item.actions.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



// TODO: fix this dependency










var ItemEditFormComponent = /** @class */ (function () {
    function ItemEditFormComponent(itemService, contentTypeService, eavService, actions$) {
        var _this = this;
        this.itemService = itemService;
        this.contentTypeService = contentTypeService;
        this.eavService = eavService;
        this.actions$ = actions$;
        this.itemFormValueChange = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        this.itemBehaviorSubject$ = new rxjs__WEBPACK_IMPORTED_MODULE_1__["BehaviorSubject"](null);
        this.formIsValid = false;
        // deleteItem() {
        //   this.itemService.deleteItem(this.item);
        // }
        this.setFormValues = function (item, emit) {
            if (_this.form) {
                var formValues_1 = {};
                Object.keys(item.entity.attributes).forEach(function (attributeKey) {
                    formValues_1[attributeKey] = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_8__["LocalizationHelper"].translate(_this.currentLanguage, _this.defaultLanguage, item.entity.attributes[attributeKey], null);
                });
                if (_this.form.valueIsChanged(formValues_1)) {
                    // set new values to form
                    _this.form.patchValue(formValues_1, emit);
                }
                // important to be after patchValue
                _this.eavService.triggerFormSetValueChange(formValues_1);
            }
        };
        /**
         * load content type attributes to Formly FormFields (formlyFieldConfigArray)
         */
        this.loadContentTypeFormFields = function () {
            return _this.contentType$
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["switchMap"])(function (data) {
                var parentFieldGroup = _this.createEmptyFieldGroup(null, false);
                var currentFieldGroup = parentFieldGroup;
                // loop through contentType attributes
                data.contentType.attributes.forEach(function (attribute, index) {
                    var formlyFieldConfig = _this.loadFieldFromDefinitionTest(attribute, index);
                    // if input type is empty-default create new field group and than continue to add fields to that group
                    if (attribute.settings.InputType.values[0].value === _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].emptyDefault) {
                        var collapsed = attribute.settings.DefaultCollapsed ? attribute.settings.DefaultCollapsed.values[0].value : false;
                        currentFieldGroup = _this.createEmptyFieldGroup(attribute, collapsed);
                        parentFieldGroup.fieldGroup.push(currentFieldGroup);
                    }
                    else {
                        currentFieldGroup.fieldGroup.push(formlyFieldConfig);
                    }
                });
                return Object(rxjs__WEBPACK_IMPORTED_MODULE_1__["of"])([parentFieldGroup]);
            }));
        };
        /**
         * Create title field group with collapsible wrapper
         */
        this.createEmptyFieldGroup = function (attribute, collapse) {
            var settingsTranslated = null;
            if (attribute) {
                settingsTranslated = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_8__["LocalizationHelper"].translateSettings(attribute.settings, _this.currentLanguage, _this.defaultLanguage);
            }
            return {
                collapse: collapse,
                fieldGroup: [],
                header: _this.item.header,
                label: attribute !== null
                    ? (settingsTranslated !== null && settingsTranslated.Name) ? settingsTranslated.Name : attribute.name
                    : 'Edit Item',
                name: attribute !== null ? attribute.name : 'Edit Item',
                settings: settingsTranslated,
                type: _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].emptyDefault,
                wrappers: ['app-collapsible-wrapper'],
            };
        };
        this.eavConfig = eavService.getEavConfiguration();
    }
    Object.defineProperty(ItemEditFormComponent.prototype, "currentLanguage", {
        get: function () {
            return this.currentLanguageValue;
        },
        set: function (value) {
            this.currentLanguageValue = value;
            this.setFormValues(this.item, false); // need set emit to true because of  external commponents
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ItemEditFormComponent.prototype, "item", {
        get: function () {
            return this.itemBehaviorSubject$.getValue();
        },
        set: function (value) {
            this.itemBehaviorSubject$.next(value);
        },
        enumerable: true,
        configurable: true
    });
    ItemEditFormComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.itemBehaviorSubject$.subscribe(function (item) {
            _this.setFormValues(item, false);
        });
        this.loadContentTypeFromStore();
    };
    ItemEditFormComponent.prototype.formSaveObservable = function () {
        var _this = this;
        return this.actions$
            .ofType(_shared_store_actions_item_actions__WEBPACK_IMPORTED_MODULE_12__["SAVE_ITEM_ATTRIBUTES_VALUES"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["filter"])(function (action) { return action.item.entity.id === _this.item.entity.id; }));
    };
    ItemEditFormComponent.prototype.ngOnDestroy = function () {
        this.itemBehaviorSubject$.unsubscribe();
        // this.formSuccess.unsubscribe();
        // this.formError.unsubscribe();
    };
    ItemEditFormComponent.prototype.ngOnChanges = function () {
        console.log('ItemEditFormComponent current change: ', this.currentLanguage);
        // this.formIsValid = this.form.form.valid;
    };
    /**
     * Update NGRX/store on form value change
     * @param values key:value list of fields from form
     */
    ItemEditFormComponent.prototype.formValueChange = function (values) {
        if (this.form.form.valid) {
            this.itemService.updateItemAttributesValues(this.item.entity.id, values, this.currentLanguage, this.defaultLanguage);
        }
        // emit event to perent
        this.itemFormValueChange.emit();
    };
    ItemEditFormComponent.prototype.submit = function (values) {
        console.log('submit item edit');
        console.log(values);
        if (this.form.form.valid) {
            // TODO create body for submit
            this.eavService.saveItem(this.eavConfig.appId, this.item, values, this.currentLanguage, this.defaultLanguage);
        }
    };
    ItemEditFormComponent.prototype.loadContentTypeFromStore = function () {
        // Load content type for item from store
        this.contentType$ = this.contentTypeService.getContentTypeById(this.item.entity.type.id);
        // create form fields from content type
        this.itemFields$ = this.loadContentTypeFormFields();
    };
    // TEST
    ItemEditFormComponent.prototype.loadFieldFromDefinitionTest = function (attribute, index) {
        if (attribute.settings.InputType.values[0].value === 'custom-gps') {
            console.log('loadFieldFromDefinitionTest attribute:', attribute);
        }
        console.log('loadFieldFromDefinitionTest', attribute.settings.InputType);
        if (attribute.settings.InputType) {
            // if (attribute.settings.InputType.values[0].value.startWith('custom')) {
            //   return this.loadFieldFromDefinition(attribute, InputTypesConstants.external, index);
            // } else {
            //   return this.loadFieldFromDefinition(attribute, attribute.settings.InputType.values[0].value, index);
            // }
            switch (attribute.settings.InputType.values[0].value) {
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringDefault:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringDefault, index);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringUrlPath:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringUrlPath, index);
                // return this.loadFieldFromDefinitionStringUrlPath(attribute);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].booleanDefault:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].booleanDefault, index);
                // return this.getStringIconFontPickerFormlyField(attribute);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringDropdown:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringDropdown, index);
                // return this.loadFieldFromDefinitionStringDropDown(attribute);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].datetimeDefault:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].datetimeDefault, index);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].numberDefault:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].numberDefault, index);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringFontIconPicker:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringFontIconPicker, index);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].entityDefault:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].entityDefault, index);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].hyperlinkDefault:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].hyperlinkDefault, index);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].hyperlinkLibrary:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].hyperlinkLibrary, index);
                case _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].external:
                case 'string-wysiwyg':
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].external, index);
                case 'custom-my-field-test':
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].external, index);
                default:
                    return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringDefault, index);
            }
        }
        else {
            return this.loadFieldFromDefinition(attribute, _shared_constants_input_types_constants__WEBPACK_IMPORTED_MODULE_5__["InputTypesConstants"].stringDefault, index);
        }
    };
    /**
     * Load formly field from AttributeDef
     * @param attribute
     */
    ItemEditFormComponent.prototype.loadFieldFromDefinition = function (attribute, inputType, index) {
        // const inputType = InputTypesConstants.stringDefault; // attribute.settings.InputType.values[0].value;
        var settingsTranslated = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_8__["LocalizationHelper"].translateSettings(attribute.settings, this.currentLanguage, this.defaultLanguage);
        // set validation for all input types
        var validationList = _eav_material_controls_validators_validation_helper__WEBPACK_IMPORTED_MODULE_9__["ValidationHelper"].setDefaultValidations(settingsTranslated);
        var required = settingsTranslated.Required ? settingsTranslated.Required : false;
        // LocalizationHelper.translate(this.currentLanguage, this.defaultLanguage, attribute.settings.Required, false);
        // attribute.settings.Required ? attribute.settings.Required.values[0].value : false;
        var value = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_8__["LocalizationHelper"].translate(this.currentLanguage, this.defaultLanguage, this.item.entity.attributes[attribute.name], null);
        var disabled = settingsTranslated.Disabled
            ? settingsTranslated.Disabled : (this.isControlDisabledForCurrentLanguage(this.currentLanguage, this.defaultLanguage, this.item.entity.attributes[attribute.name], attribute.name));
        // const label = settingsTranslated.Name ? settingsTranslated.Name : null;
        var label = attribute !== null
            ? (settingsTranslated !== null && settingsTranslated.Name) ? settingsTranslated.Name : attribute.name
            : null;
        // LocalizationHelper.translate(this.currentLanguage, this.defaultLanguage, attribute.settings.Name, null);
        return {
            // valueKey: `${attribute.name}.values[0].value`,
            // pattern: pattern,
            disabled: disabled,
            entityId: this.item.entity.id,
            fullSettings: attribute.settings,
            header: this.item.header,
            index: index,
            label: label,
            name: attribute.name,
            placeholder: "Enter " + attribute.name,
            required: required,
            settings: settingsTranslated,
            type: inputType,
            validation: validationList,
            value: value,
            wrappers: ['app-hidden-wrapper'],
        };
    };
    /**
     * Determines is control disabled
     * @param currentLanguage
     * @param defaultLanguage
     * @param attributeValues
     * @param attributeKey
     */
    ItemEditFormComponent.prototype.isControlDisabledForCurrentLanguage = function (currentLanguage, defaultLanguage, attributeValues, attributeKey) {
        if (_shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_8__["LocalizationHelper"].isEditableTranslationExist(attributeValues, currentLanguage, defaultLanguage)) {
            return false;
            // } else if (LocalizationHelper.isReadonlyTranslationExist(attributeValues, currentLanguage)) {
            //   return true;
        }
        else {
            return true;
        }
    };
    /**
     * Enables all controls in form
     * @param allAttributes
     */
    ItemEditFormComponent.prototype.enableAllControls = function (allAttributes) {
        var _this = this;
        Object.keys(allAttributes).forEach(function (attributeKey) {
            if (_this.form.value[attributeKey] === undefined) {
                _this.form.setDisabled(attributeKey, false, false);
            }
        });
    };
    /**
     * loop trough all controls and set disable control if needed
     * @param allAttributes
     * @param currentLanguage
     * @param defaultLanguage
     */
    ItemEditFormComponent.prototype.disableControlsForCurrentLanguage = function (allAttributes, currentLanguage, defaultLanguage) {
        var _this = this;
        Object.keys(this.item.entity.attributes).forEach(function (attributeKey) {
            var disabled = _this.isControlDisabledForCurrentLanguage(currentLanguage, defaultLanguage, _this.item.entity.attributes[attributeKey], attributeKey);
            _this.form.setDisabled(attributeKey, disabled, false);
        });
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_eav_dynamic_form_components_eav_form_eav_form_component__WEBPACK_IMPORTED_MODULE_3__["EavFormComponent"]),
        __metadata("design:type", _eav_dynamic_form_components_eav_form_eav_form_component__WEBPACK_IMPORTED_MODULE_3__["EavFormComponent"])
    ], ItemEditFormComponent.prototype, "form", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"])(),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"])
    ], ItemEditFormComponent.prototype, "itemFormValueChange", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", String)
    ], ItemEditFormComponent.prototype, "defaultLanguage", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", String),
        __metadata("design:paramtypes", [String])
    ], ItemEditFormComponent.prototype, "currentLanguage", null);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", _shared_models_eav__WEBPACK_IMPORTED_MODULE_4__["Item"]),
        __metadata("design:paramtypes", [_shared_models_eav__WEBPACK_IMPORTED_MODULE_4__["Item"]])
    ], ItemEditFormComponent.prototype, "item", null);
    ItemEditFormComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-item-edit-form',
            template: __webpack_require__(/*! ./item-edit-form.component.html */ "./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.html"),
            styles: [__webpack_require__(/*! ./item-edit-form.component.css */ "./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.css")]
        }),
        __metadata("design:paramtypes", [_shared_services_item_service__WEBPACK_IMPORTED_MODULE_6__["ItemService"],
            _shared_services_content_type_service__WEBPACK_IMPORTED_MODULE_7__["ContentTypeService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_10__["EavService"],
            _ngrx_effects__WEBPACK_IMPORTED_MODULE_11__["Actions"]])
    ], ItemEditFormComponent);
    return ItemEditFormComponent;
}());



/***/ }),

/***/ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.css":
/*!*****************************************************************************************!*\
  !*** ./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.css ***!
  \*****************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = " .example-card {\r\n    max-width: 600px;\r\n    margin: 0 auto; \r\n\r\n    /* position: fixed; \r\n    left: 50%;\r\n    margin-left: -400px; */\r\n  } \r\n\r\n  .button-top-right {\r\n    position: absolute;\r\n    right: 0;\r\n    top: 0;\r\n  } \r\n\r\n  .disabled {\r\n  color: rgba(0,0,0,.26);\r\n  background-color: rgba(0,0,0,.12);\r\n}"

/***/ }),

/***/ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.html":
/*!******************************************************************************************!*\
  !*** ./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.html ***!
  \******************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- <mat-card class=\"example-card\" *ngIf=\"(items$ | async) && (items$ | async).length\"> -->\r\n<!-- <mat-dialog-actions> -->\r\n<!-- class=\"button-top-right\" -->\r\n<!-- <button mat-icon-button (click)=\"close()\">\r\n    <mat-icon class=\"mat-24\">close</mat-icon>\r\n  </button>\r\n</mat-dialog-actions> -->\r\n<div mat-dialog-content *ngIf=\"(items$ | async) && (items$ | async).length\">\r\n  <!-- <button mat-icon-button (click)=\"close()\" class=\"button-top-right\">\r\n    <mat-icon class=\"mat-24\">close</mat-icon>\r\n  </button> -->\r\n\r\n  <app-eav-language-switcher *ngIf=\"(languages$ | async).length > 0\" [languages]=\"(languages$ | async)\" [currentLanguage]=\"(currentLanguage$ | async)\"></app-eav-language-switcher>\r\n\r\n  <mat-error *ngIf=\"!formsAreValid\">\r\n    <span *ngFor=\"let formError of formErrors;\">\r\n      <p *ngFor=\"let key of Object.keys(formError)\">{{key}}: {{ formError[key] | translate}}</p>\r\n    </span>\r\n  </mat-error>\r\n\r\n  <div *ngFor=\"let item of (items$ | async); trackBy:trackByFn\">\r\n    <app-item-edit-form [item]=\"item\" [currentLanguage]=\"(currentLanguage$ | async)\" [defaultLanguage]=\"(defaultLanguage$ | async)\"\r\n      (itemFormValueChange)=\"formValueChange($event)\"></app-item-edit-form>\r\n  </div>\r\n  <button mat-raised-button type=\"submit\" class=\"btn btn-default submit-button\" [ngClass]=\"{ 'disabled': !formsAreValid }\"\r\n    (click)=\"saveAll(true)\">{{ 'Button.Save' | translate}}</button>\r\n  <button mat-icon-button type=\"button\" [matMenuTriggerFor]=\"saveMenu\" [ngClass]=\"{ 'disabled': !formsAreValid }\">\r\n    <i class=\"eav-icon-down-dir\"></i>\r\n  </button>\r\n  <mat-menu #saveMenu=\"matMenu\" [overlapTrigger]=\"false\">\r\n    <button mat-menu-item type=\"button\" (click)=\"saveAll(true)\">{{ 'Button.Save' | translate}}</button>\r\n    <button mat-menu-item type=\"button\" (click)=\"saveAll(false)\">{{ 'Button.SaveAndKeepOpen' | translate}}</button>\r\n  </mat-menu>\r\n  <button mat-icon-button type=\"button\" [matMenuTriggerFor]=\"publishModeMenu\" [ngClass]=\"{ 'disabled': !formsAreValid }\">\r\n    <i [ngClass]=\"{'eav-icon-eye': publishMode === 'show', 'eav-icon-eye-close': publishMode === 'hide', 'eav-icon-git-branch': publishMode === 'branch'}\"></i> {{ 'SaveMode.' + publishMode | translate }}\r\n    <i class=\"eav-icon-down-dir\"></i>\r\n  </button>\r\n\r\n  <mat-menu #publishModeMenu=\"matMenu\" [overlapTrigger]=\"false\">\r\n    <button *ngIf=\"eavConfig.versioningOptions.show\" mat-menu-item type=\"button\" (click)=\"publishMode = 'show'\">\r\n      <i class=\"eav-icon-eye\"></i>{{ 'SaveMode.show' | translate }}</button>\r\n    <button *ngIf=\"eavConfig.versioningOptions.hide\" mat-menu-item type=\"button\" (click)=\"publishMode = 'hide'\">\r\n      <i class=\"eav-icon-eye-close\"></i>{{ 'SaveMode.hide' | translate }}</button>\r\n    <button *ngIf=\"eavConfig.versioningOptions.branch\" mat-menu-item type=\"button\" (click)=\"publishMode = 'branch'\">\r\n      <i class=\"eav-icon-git-branch\"></i>{{ 'SaveMode.branch' |translate }}</button>\r\n  </mat-menu>\r\n</div>\r\n<!-- </mat-card> -->"

/***/ }),

/***/ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.ts":
/*!****************************************************************************************!*\
  !*** ./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.ts ***!
  \****************************************************************************************/
/*! exports provided: MultiItemEditFormComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MultiItemEditFormComponent", function() { return MultiItemEditFormComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @ngrx/effects */ "./node_modules/@ngrx/effects/fesm5/effects.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var reflect_metadata__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! reflect-metadata */ "./node_modules/reflect-metadata/Reflect.js");
/* harmony import */ var reflect_metadata__WEBPACK_IMPORTED_MODULE_5___default = /*#__PURE__*/__webpack_require__.n(reflect_metadata__WEBPACK_IMPORTED_MODULE_5__);
/* harmony import */ var _shared_store_actions_item_actions__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../shared/store/actions/item.actions */ "./src/app/shared/store/actions/item.actions.ts");
/* harmony import */ var _shared_services_content_type_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../shared/services/content-type.service */ "./src/app/shared/services/content-type.service.ts");
/* harmony import */ var _item_edit_form_item_edit_form_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../item-edit-form/item-edit-form.component */ "./src/app/eav-item-dialog/item-edit-form/item-edit-form.component.ts");
/* harmony import */ var _shared_services_item_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../../shared/services/item.service */ "./src/app/shared/services/item.service.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
/* harmony import */ var _shared_services_language_service__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../../shared/services/language.service */ "./src/app/shared/services/language.service.ts");
/* harmony import */ var _eav_material_controls_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../../eav-material-controls/validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm5/ngx-translate-core.js");
/* harmony import */ var _shared_models_json_format_v1__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../../shared/models/json-format-v1 */ "./src/app/shared/models/json-format-v1/index.ts");
/* harmony import */ var _shared_services_input_type_service__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ../../shared/services/input-type.service */ "./src/app/shared/services/input-type.service.ts");
/* harmony import */ var _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ../../shared/constants/type-constants */ "./src/app/shared/constants/type-constants.ts");
/* harmony import */ var _shared_models_eav_admin_dialog_data__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../../shared/models/eav/admin-dialog-data */ "./src/app/shared/models/eav/admin-dialog-data.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (undefined && undefined.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};


















var MultiItemEditFormComponent = /** @class */ (function () {
    function MultiItemEditFormComponent(formDialogData, itemService, inputTypeService, contentTypeService, eavService, languageService, changeDetectorRef, actions$, snackBar, validationMessagesService, translate) {
        this.formDialogData = formDialogData;
        this.itemService = itemService;
        this.inputTypeService = inputTypeService;
        this.contentTypeService = contentTypeService;
        this.eavService = eavService;
        this.languageService = languageService;
        this.changeDetectorRef = changeDetectorRef;
        this.actions$ = actions$;
        this.snackBar = snackBar;
        this.validationMessagesService = validationMessagesService;
        this.translate = translate;
        this.formSaveAllObservables$ = [];
        this.formErrors = [];
        this.Object = Object;
        this.formsAreValid = false;
        this.closeWindow = false;
        this.willPublish = false; // default is won't publish, but will usually be overridden
        this.publishMode = 'hide'; // has 3 modes: show, hide, branch (where branch is a hidden, linked clone)
        this.enableDraft = false;
        this.subscriptions = [];
        this.currentLanguage$ = languageService.getCurrentLanguage();
        this.defaultLanguage$ = languageService.getDefaultLanguage();
        this.translate.setDefaultLang('en');
        this.translate.use('en');
        // Read configuration from queryString
        this.eavConfig = this.eavService.getEavConfiguration();
        this.languageService.loadLanguages(JSON.parse(this.eavConfig.langs), this.eavConfig.lang, this.eavConfig.langpri, 'en-us');
    }
    MultiItemEditFormComponent.prototype.ngOnInit = function () {
        this.loadItemsData();
        this.setLanguageConfig();
        // suscribe to form submit
        this.saveFormMessagesSubscribe();
    };
    MultiItemEditFormComponent.prototype.ngAfterContentChecked = function () {
        this.saveFormSuscribe();
        this.setFormsAreValid();
        // need this to detectChange this.formsAreValid after ViewChecked
        this.changeDetectorRef.detectChanges();
    };
    MultiItemEditFormComponent.prototype.ngOnDestroy = function () {
        this.subscriptions.forEach(function (subscriber) { return subscriber.unsubscribe(); });
    };
    /**
     * observe formValue changes from all child forms
     */
    MultiItemEditFormComponent.prototype.formValueChange = function () {
        this.setFormsAreValid();
        // reset form errors
        this.formErrors = [];
    };
    /**
   * save all forms
   */
    MultiItemEditFormComponent.prototype.saveAll = function (close) {
        if (this.formsAreValid) {
            this.itemEditFormComponentQueryList.forEach(function (itemEditFormComponent) {
                itemEditFormComponent.form.submitOutside();
            });
            if (close) {
                this.closeWindow = true;
                // this.close();
            }
        }
        else {
            this.displayAllValidationMessages();
        }
    };
    /**
     * close (remove) iframe window
     */
    MultiItemEditFormComponent.prototype.close = function () {
        // find and remove iframe
        // TODO: this is not good - need to find better solution
        var iframes = window.parent.frames.document.getElementsByTagName('iframe');
        if (iframes[0] && iframes[0].parentElement) {
            iframes[0].parentElement.remove();
        }
    };
    MultiItemEditFormComponent.prototype.trackByFn = function (index, item) {
        return item.entity.id;
    };
    /**
     * Load all data for forms
     */
    MultiItemEditFormComponent.prototype.loadItemsData = function () {
        var _this = this;
        var entityId = Number(this.formDialogData.id);
        // if dialog type load with entity ids (edit - entity)
        if (this.formDialogData.type === _shared_constants_type_constants__WEBPACK_IMPORTED_MODULE_16__["DialogTypeConstants"].itemEditWithEntityId && entityId) {
            this.eavService.loadAllDataForFormByEntity(this.eavConfig.appId, [{ 'EntityId': entityId }]).subscribe(function (data) {
                _this.afterLoadItemsData(data);
            });
            // this.items$ = this.itemService.selectItemsByIdList([entityId]);
        }
        else { // else dialog type load without entity ids. (edit - toolbar)
            this.eavService.loadAllDataForForm(this.eavConfig.appId, this.eavConfig.items).subscribe(function (data) {
                _this.afterLoadItemsData(data);
                _this.items$ = _this.itemService.selectItemsByIdList(data.Items.map(function (item) { return item.Entity.Id; }));
            });
        }
    };
    MultiItemEditFormComponent.prototype.afterLoadItemsData = function (data) {
        this.itemService.loadItems(data.Items);
        this.inputTypeService.loadInputTypes(data.InputTypes);
        this.contentTypeService.loadContentTypes(data.ContentTypes);
        this.setPublishMode(data.Items);
        this.items$ = this.itemService.selectItemsByIdList(data.Items.map(function (item) { return item.Entity.Id; }));
    };
    /**
     * Read Eav Configuration
     */
    // private setEavConfiguration() {
    //   const queryStringParameters = UrlHelper.readQueryStringParameters(this.route.snapshot.fragment);
    //   console.log('queryStringParameters', queryStringParameters);
    //   // const eavConfiguration: EavConfiguration = UrlHelper.getEavConfiguration(queryStringParameters);
    //   this.eavConfig = UrlHelper.getEavConfiguration(queryStringParameters);
    // }
    MultiItemEditFormComponent.prototype.setLanguageConfig = function () {
        var _this = this;
        this.setTranslateLanguage(this.eavConfig.lang);
        // UILanguage harcoded (for future usage)
        // this.languageService.loadLanguages(JSON.parse(this.eavConfig.langs), this.eavConfig.lang, this.eavConfig.langpri, 'en-us');
        this.languages$ = this.languageService.selectAllLanguages();
        // on current language change reset form errors
        this.subscriptions.push(this.currentLanguage$.subscribe(function (len) {
            _this.formErrors = [];
        }));
    };
    /**
     * Set translate language of all forms
     * @param language
     *
     */
    MultiItemEditFormComponent.prototype.setTranslateLanguage = function (language) {
        if (language) {
            // TODO: find better solution
            var isoLangCode = language.substring(0, language.indexOf('-') > 0 ? language.indexOf('-') : 2);
            this.translate.use(isoLangCode);
        }
    };
    /**
     * With zip function look all forms submit observables and when all finish save all data (call savemany service)
     */
    MultiItemEditFormComponent.prototype.saveFormSuscribe = function () {
        var _this = this;
        // important - only subscribe once
        if (this.formSaveAllObservables$.length === 0) {
            if (this.itemEditFormComponentQueryList && this.itemEditFormComponentQueryList.length > 0) {
                this.itemEditFormComponentQueryList.forEach(function (itemEditFormComponent) {
                    _this.formSaveAllObservables$.push(itemEditFormComponent.formSaveObservable());
                });
            }
            if (this.formSaveAllObservables$ && this.formSaveAllObservables$.length > 0) {
                this.subscriptions.push(rxjs__WEBPACK_IMPORTED_MODULE_1__["zip"].apply(void 0, this.formSaveAllObservables$).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["switchMap"])(function (actions) {
                    console.log('ZIP ACTIONS ITEM: ', _shared_models_json_format_v1__WEBPACK_IMPORTED_MODULE_14__["JsonItem1"].create(actions[0].item));
                    var allItems = [];
                    actions.forEach(function (action) {
                        allItems.push(_shared_models_json_format_v1__WEBPACK_IMPORTED_MODULE_14__["JsonItem1"].create(action.item));
                    });
                    var body = "{\"Items\": " + JSON.stringify(allItems) + "}";
                    return _this.eavService.savemany(_this.eavConfig.appId, _this.eavConfig.partOfPage, body)
                        .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["map"])(function (data) { return _this.eavService.saveItemSuccess(data); }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["tap"])(function (data) { return console.log('working'); }));
                }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["catchError"])(function (err) { return Object(rxjs__WEBPACK_IMPORTED_MODULE_1__["of"])(_this.eavService.saveItemError(err)); }))
                    .subscribe());
            }
        }
    };
    /**
     * display form messages on form success or form error
     */
    MultiItemEditFormComponent.prototype.saveFormMessagesSubscribe = function () {
        var _this = this;
        this.subscriptions.push(this.actions$
            .ofType(_shared_store_actions_item_actions__WEBPACK_IMPORTED_MODULE_6__["SAVE_ITEM_ATTRIBUTES_VALUES_SUCCESS"])
            .subscribe(function (action) {
            console.log('success END: ', action.data);
            // TODO show success message
            // this.snackBar.open('saved',);
            _this.snackBarOpen('saved', _this.closeWindow);
        }));
        this.subscriptions.push(this.actions$
            .ofType(_shared_store_actions_item_actions__WEBPACK_IMPORTED_MODULE_6__["SAVE_ITEM_ATTRIBUTES_VALUES_ERROR"])
            .subscribe(function (action) {
            console.log('error END', action.error);
            // TODO show error message
            _this.snackBarOpen('error', false);
        }));
    };
    /**
     * Open snackbar with message and after closed call function close
     * @param message
     * @param callClose
     */
    MultiItemEditFormComponent.prototype.snackBarOpen = function (message, callClose) {
        var _this = this;
        var snackBarRef = this.snackBar.open(message, '', {
            duration: 2000
        });
        if (callClose) {
            this.subscriptions.push(snackBarRef.afterDismissed().subscribe(null, null, function () {
                _this.close();
            }));
        }
    };
    /**
     * Determines whether all forms are valid and sets a this.formsAreValid depending on it
     */
    MultiItemEditFormComponent.prototype.setFormsAreValid = function () {
        var _this = this;
        this.formsAreValid = false;
        if (this.itemEditFormComponentQueryList && this.itemEditFormComponentQueryList.length > 0) {
            this.formsAreValid = true;
            this.itemEditFormComponentQueryList.forEach(function (itemEditFormComponent) {
                if (itemEditFormComponent.form.valid === false) {
                    _this.formsAreValid = false;
                }
            });
        }
    };
    /**
     * Fill in all error validation messages from all forms
     */
    MultiItemEditFormComponent.prototype.displayAllValidationMessages = function () {
        var _this = this;
        this.formErrors = [];
        if (this.itemEditFormComponentQueryList && this.itemEditFormComponentQueryList.length > 0) {
            this.itemEditFormComponentQueryList.forEach(function (itemEditFormComponent) {
                if (itemEditFormComponent.form.form.invalid) {
                    _this.formErrors.push(_this.validationMessagesService.validateForm(itemEditFormComponent.form.form, false));
                }
            });
        }
    };
    // TODO: finish group and new entity ?????
    MultiItemEditFormComponent.prototype.setPublishMode = function (items) {
        items.forEach(function (item) {
            // If the entity is null, it does not exist yet. Create a new one
            // TODO: do we need this ???
            // if (!item.entity && !!item.header.contentTypeName) {
            //   // TODO: do we need this ???
            //   item.entity = entitiesSvc.newEntity(item.header);
            // }
            // TODO: do we need this ???
            // item.entity = enhanceEntity(item.entity);
            ////// load more content-type metadata to show
            //// vm.items[i].ContentType = contentTypeSvc.getDetails(vm.items[i].Header.ContentTypeName);
            // set slot value - must be inverte for boolean-switch
            // const grp = item.header.group;
            // item.slotIsUsed = (grp === null || grp === undefined || grp.SlotIsEmpty !== true);
        });
        // this.willPublish = items[0].entity.IsPublished;
        // this.enableDraft = items[0].header.entityId !== 0; // it already exists, so enable draft
        // this.publishMode = items[0].entity.IsBranch
        //   ? 'branch' // it's a branch, so it must have been saved as a draft-branch
        //   : items[0].entity.IsPublished ? 'show' : 'hide';
        // if publish mode is prohibited, revert to default
        if (!this.eavConfig.versioningOptions[this.publishMode]) {
            this.publishMode = Object.keys(this.eavConfig.versioningOptions)[0];
        }
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChildren"])(_item_edit_form_item_edit_form_component__WEBPACK_IMPORTED_MODULE_8__["ItemEditFormComponent"]),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["QueryList"])
    ], MultiItemEditFormComponent.prototype, "itemEditFormComponentQueryList", void 0);
    MultiItemEditFormComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-multi-item-edit-form',
            template: __webpack_require__(/*! ./multi-item-edit-form.component.html */ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.html"),
            styles: [__webpack_require__(/*! ./multi-item-edit-form.component.css */ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.css")]
        }),
        __param(0, Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Inject"])(_angular_material__WEBPACK_IMPORTED_MODULE_4__["MAT_DIALOG_DATA"])),
        __metadata("design:paramtypes", [_shared_models_eav_admin_dialog_data__WEBPACK_IMPORTED_MODULE_17__["AdminDialogData"],
            _shared_services_item_service__WEBPACK_IMPORTED_MODULE_9__["ItemService"],
            _shared_services_input_type_service__WEBPACK_IMPORTED_MODULE_15__["InputTypeService"],
            _shared_services_content_type_service__WEBPACK_IMPORTED_MODULE_7__["ContentTypeService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_10__["EavService"],
            _shared_services_language_service__WEBPACK_IMPORTED_MODULE_11__["LanguageService"],
            _angular_core__WEBPACK_IMPORTED_MODULE_0__["ChangeDetectorRef"],
            _ngrx_effects__WEBPACK_IMPORTED_MODULE_3__["Actions"],
            _angular_material__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"],
            _eav_material_controls_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_12__["ValidationMessagesService"],
            _ngx_translate_core__WEBPACK_IMPORTED_MODULE_13__["TranslateService"]])
    ], MultiItemEditFormComponent);
    return MultiItemEditFormComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.css":
/*!******************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.css ***!
  \******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.html":
/*!*******************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.html ***!
  \*******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- This div is adam-hint -->\r\n<div class=\"small pull-right\">\r\n  <span style=\"opacity: 0.5\">drop files here -</span>\r\n  <a href=\"http://2sxc.org/help?tag=adam\" target=\"_blank\" matTooltip=\"ADAM is the Automatic Digital Assets Manager - click to discover more\">\r\n    <i class=\"eav-icon-apple\"></i>\r\n    Adam\r\n  </a>\r\n  <span style=\"opacity: 0.5\"> is sponsored with\r\n    <i class=\"eav-icon-heart\"></i> by\r\n    <a tabindex=\"-1\" href=\"http://2sic.com/\" target=\"_blank\">\r\n      2sic.com\r\n    </a>\r\n  </span>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.ts":
/*!*****************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.ts ***!
  \*****************************************************************************/
/*! exports provided: AdamHintComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AdamHintComponent", function() { return AdamHintComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var AdamHintComponent = /** @class */ (function () {
    function AdamHintComponent() {
    }
    AdamHintComponent.prototype.ngOnInit = function () {
    };
    AdamHintComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'adam-hint',
            template: __webpack_require__(/*! ./adam-hint.component.html */ "./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.html"),
            styles: [__webpack_require__(/*! ./adam-hint.component.css */ "./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.css")]
        }),
        __metadata("design:paramtypes", [])
    ], AdamHintComponent);
    return AdamHintComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/adam/adam.service.ts":
/*!************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/adam.service.ts ***!
  \************************************************************/
/*! exports provided: AdamService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AdamService", function() { return AdamService; });
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _shared_services_svc_creator_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/svc-creator.service */ "./src/app/shared/services/svc-creator.service.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var AdamService = /** @class */ (function () {
    function AdamService(httpClient, svcCreatorService, eavService) {
        this.httpClient = httpClient;
        this.svcCreatorService = svcCreatorService;
        this.eavService = eavService;
        this.eavConfig = this.eavService.getEavConfiguration();
    }
    AdamService.prototype.createSvc = function (subfolder, serviceConfig, url) {
        var _this = this;
        // TODO: find how to solve serviceRoot
        // const serviceRoot = 'http://2sxc-dnn742.dnndev.me/en-us/desktopmodules/2sxc/api/';
        // const url = url, //UrlHelper.resolveServiceUrl('app-content/' + contentType + '/' + entityGuid + '/' + field, serviceRoot);
        var folders = [];
        var adamRoot = this.eavConfig.approot.substr(0, this.eavConfig.approot.indexOf('2sxc'));
        // extend a json-response with a path (based on the adam-root) to also have a fullPath
        var addFullPath = function (value, key) {
            // 2dm 2018-03-29 special fix - sometimes the path already has the full path, sometimes not
            // it should actually be resolved properly, but because I don't have time
            // ATM (data comes from different web-services, which are also used in other places
            // I'll just check if it's already in there
            value.FullPath = value.Path;
            if (value.Path && value.Path.toLowerCase().indexOf(adamRoot.toLowerCase()) === -1) {
                value.FullPath = adamRoot + value.Path;
            }
        };
        // create folder
        var addFolder = function (newfolder) {
            // maybe create model for data
            return _this.httpClient.post(url + '/folder', {}, {
                params: {
                    subfolder: subfolder,
                    newFolder: newfolder,
                    usePortalRoot: serviceConfig.usePortalRoot,
                    appId: _this.eavConfig.appId
                }
            })
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(function (data) {
                reload();
                return data;
            }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["tap"])(function (data) { return console.log('addFolder: ', data); }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["catchError"])(function (error) { return _this.handleError(error); }));
        };
        var goIntoFolder = function (childFolder) {
            folders.push(childFolder);
            var pathParts = childFolder.Path.split('/');
            var subPath = '';
            for (var i = 0; i < folders.length; i++) {
                subPath = pathParts[pathParts.length - i - 2] + '/' + subPath;
            }
            subPath = subPath.replace('//', '/');
            if (subPath[subPath.length - 1] === '/') {
                subPath = subPath.substr(0, subPath.length - 1);
            }
            childFolder.Subfolder = subPath;
            // now assemble the correct subfolder based on the folders-array
            subfolder = subPath;
            reload();
            return subPath;
        };
        var goUp = function () {
            if (folders.length > 0) {
                folders.pop();
            }
            if (folders.length > 0) {
                subfolder = folders[folders.length - 1].Subfolder;
            }
            else {
                subfolder = '';
            }
            reload();
            return subfolder;
        };
        var getAll = function () {
            console.log('GET ALL subfolder:', subfolder);
            // maybe create model for data
            return _this.httpClient.get(url + '/items', {
                params: {
                    subfolder: subfolder,
                    usePortalRoot: serviceConfig.usePortalRoot,
                    appId: _this.eavConfig.appId
                }
            })
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(function (data) {
                data.forEach(addFullPath);
                return data;
            }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["tap"])(function (data) { return console.log('items subfolder: ', subfolder); }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["catchError"])(function (error) { return _this.handleError(error); }));
        };
        // delete, then reload
        // IF verb DELETE fails, so I'm using get for now
        var deleteItem = function (item) {
            return _this.httpClient.get(url + '/delete', {
                params: {
                    subfolder: subfolder,
                    isFolder: item.IsFolder,
                    id: item.Id,
                    usePortalRoot: serviceConfig.usePortalRoot,
                    appId: _this.eavConfig.appId
                }
            })
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(function (data) {
                reload();
                return data;
            }), 
            // tap(data => console.log('delete: ', data))),
            Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["catchError"])(function (error) { return _this.handleError(error); }));
        };
        // rename, then reload
        var rename = function (item, newName) {
            return _this.httpClient.get(url + '/rename', {
                params: {
                    subfolder: subfolder,
                    isFolder: item.IsFolder,
                    id: item.Id,
                    usePortalRoot: serviceConfig.usePortalRoot,
                    newName: newName,
                    appId: _this.eavConfig.appId
                }
            })
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(function (data) {
                reload();
                return data;
            }), 
            // tap(data => console.log('rename: ', data)),
            Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["catchError"])(function (error) { return _this.handleError(error); }));
        };
        // get the correct url for uploading as it is needed by external services (dropzone)
        var uploadUrl = function (targetSubfolder) {
            var urlUpl = (targetSubfolder === '')
                ? url
                : url + '?subfolder=' + targetSubfolder;
            urlUpl += (urlUpl.indexOf('?') === -1 ? '?' : '&')
                + 'usePortalRoot=' + serviceConfig.usePortalRoot
                + '&appId=' + _this.eavConfig.appId;
            return urlUpl;
        };
        var svc = {
            url: url,
            subfolder: subfolder,
            folders: folders,
            adamRoot: adamRoot,
            getAll: getAll,
            uploadUrl: uploadUrl,
            addFullPath: addFullPath,
            addFolder: addFolder,
            goIntoFolder: goIntoFolder,
            goUp: goUp,
            deleteItem: deleteItem,
            rename: rename,
            liveListReload: null,
        };
        svc = Object.assign(svc, this.svcCreatorService.implementLiveList(getAll, 'true'));
        var reload = function () { return svc.liveListReload(); };
        return svc;
    };
    AdamService.prototype.handleError = function (error) {
        // In a real world app, we might send the error to remote logging infrastructure
        var errMsg = error.message || 'Server error';
        console.error(errMsg);
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["throwError"])(errMsg);
    };
    AdamService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_3__["HttpClient"],
            _shared_services_svc_creator_service__WEBPACK_IMPORTED_MODULE_4__["SvcCreatorService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_5__["EavService"]])
    ], AdamService);
    return AdamService;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/adam/browser/adam-browser.component.css":
/*!*******************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/browser/adam-browser.component.css ***!
  \*******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".metadata-exists {\r\n    color: #0088f4\r\n}\r\n\r\n.adam-browse-background-icon {\r\n    padding-left: 5px;\r\n    padding-right: 5px;\r\n    padding-top: 10px;\r\n    padding-bottom: 10px;\r\n    min-width: 100%;\r\n    min-height: 100%;\r\n    text-align: center;\r\n}\r\n\r\n.adam-browse-background-icon i {\r\n    font-size: 4em;\r\n}\r\n\r\n.adam-background {\r\n    z-index: 20;\r\n    position: absolute;\r\n    top: 0;\r\n    left: 0;\r\n    font-size: 13px;\r\n    min-width: 100%;\r\n    max-width: 100%;\r\n    padding: 2em 1em;\r\n    text-align: center;\r\n    line-height: 150%;\r\n}\r\n\r\n.adam-tag {\r\n    cursor: pointer;\r\n    font-size: large;position: absolute;right: -15px;top: 50px;font-size: 1.8em;\r\n    z-index: 30;\r\n}\r\n\r\n.adam-link-button {\r\n    position: absolute;\r\n    left: 4px;\r\n    top: 4px;\r\n    font-size: 1.2em;\r\n    color: rgba(0, 0, 0, .9);\r\n}\r\n\r\n.adam-delete-button, .adam-rename-button {\r\n    position: absolute;\r\n    top: 4px;\r\n    right: 4px;\r\n    cursor: pointer;\r\n    font-size: 1.4em;\r\n}\r\n\r\n.adam-delete-button:hover, .adam-rename-button:hover {\r\n    color: #0088f4;\r\n}\r\n\r\n.adam-rename-button {\r\n    right: 26px;\r\n    font-size:14px;\r\n    top:5px;\r\n}\r\n\r\n.dz-details {\r\n    bottom: 0;\r\n}\r\n\r\n.dropzone .dz-preview:hover .adam-blur {\r\n    -webkit-transform:scale(1.05,1.05);\r\n    transform:scale(1.05,1.05);\r\n    -webkit-filter:blur(8px);\r\n    filter:blur(8px)\r\n}\r\n\r\n/*label under the icon*/\r\n\r\n.adam-short-label  {\r\n    text-overflow: ellipsis;\r\n    display: block;\r\n    overflow: hidden;\r\n    white-space: nowrap;\r\n\r\n    position: absolute;\r\n    left: 0;\r\n    right: 0;\r\n    bottom: 10px;\r\n}\r\n\r\n.adam-full-name {\r\n    background-color: rgba(255,255,255,.8);\r\n    border-radius: 3px;\r\n    word-break: break-all;\r\n    width: 80%;\r\n    max-width: 95%;\r\n    max-height: 57px;\r\n    overflow: hidden;\r\n\r\n    position: absolute;\r\n    top: 50%;\r\n    left: 50%;\r\n    -webkit-transform: translate(-50%, -50%);\r\n            transform: translate(-50%, -50%);\r\n\r\n}\r\n\r\n.adam-full-name-area {\r\n    height: 60px;\r\n    display: block;\r\n}\r\n\r\n.adam-browse-background {\r\n        background-color: whitesmoke;\r\n}\r\n\r\n.mce-ico[class^=\"icon-\"]:before, .mce-ico[class*=\" icon-\"]:before {\r\n    margin-left: 0;\r\n}\r\n\r\n/* TODO:  */\r\n\r\n/* FROM dropzone-custom - hyperlink */\r\n\r\n/*\r\n * The MIT License\r\n * Copyright (c) 2012 Matias Meno <m@tias.me>\r\n */\r\n\r\n/* @-webkit-keyframes passing-through {\r\n    0% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(40px);\r\n      -moz-transform: translateY(40px);\r\n      -ms-transform: translateY(40px);\r\n      -o-transform: translateY(40px);\r\n      transform: translateY(40px); }\r\n    30%, 70% {\r\n      opacity: 1;\r\n      -webkit-transform: translateY(0px);\r\n      -moz-transform: translateY(0px);\r\n      -ms-transform: translateY(0px);\r\n      -o-transform: translateY(0px);\r\n      transform: translateY(0px); }\r\n    100% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(-40px);\r\n      -moz-transform: translateY(-40px);\r\n      -ms-transform: translateY(-40px);\r\n      -o-transform: translateY(-40px);\r\n      transform: translateY(-40px); } }\r\n  @-moz-keyframes passing-through {\r\n    0% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(40px);\r\n      -moz-transform: translateY(40px);\r\n      -ms-transform: translateY(40px);\r\n      -o-transform: translateY(40px);\r\n      transform: translateY(40px); }\r\n    30%, 70% {\r\n      opacity: 1;\r\n      -webkit-transform: translateY(0px);\r\n      -moz-transform: translateY(0px);\r\n      -ms-transform: translateY(0px);\r\n      -o-transform: translateY(0px);\r\n      transform: translateY(0px); }\r\n    100% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(-40px);\r\n      -moz-transform: translateY(-40px);\r\n      -ms-transform: translateY(-40px);\r\n      -o-transform: translateY(-40px);\r\n      transform: translateY(-40px); } }\r\n  @keyframes passing-through {\r\n    0% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(40px);\r\n      -moz-transform: translateY(40px);\r\n      -ms-transform: translateY(40px);\r\n      -o-transform: translateY(40px);\r\n      transform: translateY(40px); }\r\n    30%, 70% {\r\n      opacity: 1;\r\n      -webkit-transform: translateY(0px);\r\n      -moz-transform: translateY(0px);\r\n      -ms-transform: translateY(0px);\r\n      -o-transform: translateY(0px);\r\n      transform: translateY(0px); }\r\n    100% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(-40px);\r\n      -moz-transform: translateY(-40px);\r\n      -ms-transform: translateY(-40px);\r\n      -o-transform: translateY(-40px);\r\n      transform: translateY(-40px); } }\r\n  @-webkit-keyframes slide-in {\r\n    0% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(40px);\r\n      -moz-transform: translateY(40px);\r\n      -ms-transform: translateY(40px);\r\n      -o-transform: translateY(40px);\r\n      transform: translateY(40px); }\r\n    30% {\r\n      opacity: 1;\r\n      -webkit-transform: translateY(0px);\r\n      -moz-transform: translateY(0px);\r\n      -ms-transform: translateY(0px);\r\n      -o-transform: translateY(0px);\r\n      transform: translateY(0px); } }\r\n  @-moz-keyframes slide-in {\r\n    0% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(40px);\r\n      -moz-transform: translateY(40px);\r\n      -ms-transform: translateY(40px);\r\n      -o-transform: translateY(40px);\r\n      transform: translateY(40px); }\r\n    30% {\r\n      opacity: 1;\r\n      -webkit-transform: translateY(0px);\r\n      -moz-transform: translateY(0px);\r\n      -ms-transform: translateY(0px);\r\n      -o-transform: translateY(0px);\r\n      transform: translateY(0px); } }\r\n  @keyframes slide-in {\r\n    0% {\r\n      opacity: 0;\r\n      -webkit-transform: translateY(40px);\r\n      -moz-transform: translateY(40px);\r\n      -ms-transform: translateY(40px);\r\n      -o-transform: translateY(40px);\r\n      transform: translateY(40px); }\r\n    30% {\r\n      opacity: 1;\r\n      -webkit-transform: translateY(0px);\r\n      -moz-transform: translateY(0px);\r\n      -ms-transform: translateY(0px);\r\n      -o-transform: translateY(0px);\r\n      transform: translateY(0px); } }\r\n  @-webkit-keyframes pulse {\r\n    0% {\r\n      -webkit-transform: scale(1);\r\n      -moz-transform: scale(1);\r\n      -ms-transform: scale(1);\r\n      -o-transform: scale(1);\r\n      transform: scale(1); }\r\n    10% {\r\n      -webkit-transform: scale(1.1);\r\n      -moz-transform: scale(1.1);\r\n      -ms-transform: scale(1.1);\r\n      -o-transform: scale(1.1);\r\n      transform: scale(1.1); }\r\n    20% {\r\n      -webkit-transform: scale(1);\r\n      -moz-transform: scale(1);\r\n      -ms-transform: scale(1);\r\n      -o-transform: scale(1);\r\n      transform: scale(1); } }\r\n  @-moz-keyframes pulse {\r\n    0% {\r\n      -webkit-transform: scale(1);\r\n      -moz-transform: scale(1);\r\n      -ms-transform: scale(1);\r\n      -o-transform: scale(1);\r\n      transform: scale(1); }\r\n    10% {\r\n      -webkit-transform: scale(1.1);\r\n      -moz-transform: scale(1.1);\r\n      -ms-transform: scale(1.1);\r\n      -o-transform: scale(1.1);\r\n      transform: scale(1.1); }\r\n    20% {\r\n      -webkit-transform: scale(1);\r\n      -moz-transform: scale(1);\r\n      -ms-transform: scale(1);\r\n      -o-transform: scale(1);\r\n      transform: scale(1); } }\r\n  @keyframes pulse {\r\n    0% {\r\n      -webkit-transform: scale(1);\r\n      -moz-transform: scale(1);\r\n      -ms-transform: scale(1);\r\n      -o-transform: scale(1);\r\n      transform: scale(1); }\r\n    10% {\r\n      -webkit-transform: scale(1.1);\r\n      -moz-transform: scale(1.1);\r\n      -ms-transform: scale(1.1);\r\n      -o-transform: scale(1.1);\r\n      transform: scale(1.1); }\r\n    20% {\r\n      -webkit-transform: scale(1);\r\n      -moz-transform: scale(1);\r\n      -ms-transform: scale(1);\r\n      -o-transform: scale(1);\r\n      transform: scale(1); } } */\r\n\r\n.dropzone, .dropzone * {\r\n        box-sizing: border-box;outline-color: #0069BF;  }\r\n\r\n.eav-dragging .dropzone, .dropzone.dz-drag-hover {\r\n        min-height: 0;\r\n        outline: 2px dashed #0069BF;\r\n        background: white;\r\n          padding: 0;\r\n          border-radius: 5px;\r\n          transition: outline-color 0.5s;\r\n      }\r\n\r\n.dropzone.dz-clickable {\r\n          cursor: pointer; }\r\n\r\n.dropzone.dz-clickable * {\r\n            cursor: default; }\r\n\r\n.dropzone.dz-clickable .dz-message, .dropzone.dz-clickable .dz-message * {\r\n            cursor: pointer; }\r\n\r\n.dropzone.dz-started .dz-message {\r\n          display: none; }\r\n\r\n.dropzone.dz-drag-hover {\r\n            outline-color:  #0087F7;\r\n      ;\r\n          }\r\n\r\n.dropzone.dz-drag-hover .dz-message {\r\n            opacity: 0.5; }\r\n\r\n.dropzone .dz-message {\r\n          text-align: center;\r\n          margin: 2em 0; }\r\n\r\n.dropzone-previews { margin: 5px -5px; }\r\n\r\n.dropzone .dz-preview {\r\n          position: relative;\r\n          display: inline-block;\r\n          vertical-align: top;\r\n          margin: 5px;\r\n          min-height: 100px; }\r\n\r\n.dropzone .dz-preview:hover {\r\n            z-index: 1000; }\r\n\r\n.dropzone .dz-preview:hover .dz-details {\r\n              opacity: 1; }\r\n\r\n.dropzone .dz-preview.dz-file-preview .dz-image {\r\n              border-radius: 6px; /* original before 2016-07-06 2dm: 20px;*/\r\n            background: #999;\r\n            background: linear-gradient(to bottom, #eee, #ddd); }\r\n\r\n.dropzone .dz-preview.dz-file-preview .dz-details {\r\n            opacity: 1; }\r\n\r\n.dropzone .dz-preview.dz-image-preview {\r\n            background: white; }\r\n\r\n.dropzone .dz-preview.dz-image-preview .dz-details {\r\n              transition: opacity 0.2s linear; }\r\n\r\n.dropzone .dz-preview .dz-remove {\r\n            font-size: 14px;\r\n            text-align: center;\r\n            display: block;\r\n            cursor: pointer;\r\n            border: none; }\r\n\r\n.dropzone .dz-preview .dz-remove:hover {\r\n              text-decoration: underline; }\r\n\r\n.dropzone .dz-preview:hover .dz-details {\r\n            opacity: 1; }\r\n\r\n.dropzone .dz-preview .dz-details {\r\n            z-index: 20;\r\n            position: absolute;\r\n            top: 0;\r\n            left: 0;\r\n            opacity: 0;\r\n            font-size: 13px;\r\n            min-width: 100%;\r\n            max-width: 100%;\r\n            padding: 2em 1em;\r\n            text-align: center;\r\n            color: rgba(0, 0, 0, 0.9);\r\n            line-height: 150%; }\r\n\r\n.dropzone .dz-preview .dz-details .dz-size {\r\n              margin-bottom: 1em;\r\n              font-size: 16px; }\r\n\r\n.dropzone .dz-preview .dz-details .dz-filename {\r\n              white-space: nowrap; }\r\n\r\n.dropzone .dz-preview .dz-details .dz-filename:hover span {\r\n                border: 1px solid rgba(200, 200, 200, 0.8);\r\n                background-color: rgba(255, 255, 255, 0.8); }\r\n\r\n.dropzone .dz-preview .dz-details .dz-filename:not(:hover) {\r\n                overflow: hidden;\r\n                text-overflow: ellipsis; }\r\n\r\n.dropzone .dz-preview .dz-details .dz-filename:not(:hover) span {\r\n                  border: 1px solid transparent; }\r\n\r\n.dropzone .dz-preview .dz-details .dz-filename span, .dropzone .dz-preview .dz-details .dz-size span {\r\n              background-color: rgba(255, 255, 255, 0.4);\r\n              padding: 0 0.4em;\r\n              border-radius: 3px; }\r\n\r\n.dropzone .dz-preview:hover .dz-image img {\r\n            -webkit-transform: scale(1.05, 1.05);\r\n            transform: scale(1.05, 1.05);\r\n            -webkit-filter: blur(8px);\r\n            filter: blur(8px); }\r\n\r\n.dropzone .dz-preview .dz-image {\r\n              border-radius: 6px; /* original before 2016-07-06 2dm: 20px;*/\r\n            overflow: hidden;\r\n            width: 120px;\r\n            height: 120px;\r\n            position: relative;\r\n            display: block;\r\n            z-index: 10; }\r\n\r\n.dropzone .dz-preview .dz-image img {\r\n              display: block; }\r\n\r\n.dropzone .dz-preview.dz-success .dz-success-mark {\r\n            -webkit-animation: passing-through 3s cubic-bezier(0.77, 0, 0.175, 1);\r\n            animation: passing-through 3s cubic-bezier(0.77, 0, 0.175, 1); }\r\n\r\n.dropzone .dz-preview.dz-error .dz-error-mark {\r\n            opacity: 1;\r\n            -webkit-animation: slide-in 3s cubic-bezier(0.77, 0, 0.175, 1);\r\n            animation: slide-in 3s cubic-bezier(0.77, 0, 0.175, 1); }\r\n\r\n.dropzone .dz-preview .dz-success-mark, .dropzone .dz-preview .dz-error-mark {\r\n            pointer-events: none;\r\n            opacity: 0;\r\n            z-index: 500;\r\n            position: absolute;\r\n            display: block;\r\n            top: 50%;\r\n            left: 50%;\r\n            margin-left: -27px;\r\n            margin-top: -27px; }\r\n\r\n.dropzone .dz-preview .dz-success-mark svg, .dropzone .dz-preview .dz-error-mark svg {\r\n              display: block;\r\n              width: 54px;\r\n              height: 54px; }\r\n\r\n.dropzone .dz-preview.dz-processing .dz-progress {\r\n            opacity: 1;\r\n            transition: all 0.2s linear; }\r\n\r\n.dropzone .dz-preview.dz-complete .dz-progress {\r\n            opacity: 0;\r\n            transition: opacity 0.4s ease-in; }\r\n\r\n.dropzone .dz-preview:not(.dz-processing) .dz-progress {\r\n            -webkit-animation: pulse 6s ease infinite;\r\n            animation: pulse 6s ease infinite; }\r\n\r\n.dropzone .dz-preview .dz-progress {\r\n            opacity: 1;\r\n            z-index: 1000;\r\n            pointer-events: none;\r\n            position: absolute;\r\n            height: 16px;\r\n            left: 50%;\r\n            top: 50%;\r\n            margin-top: -8px;\r\n            width: 80px;\r\n            margin-left: -40px;\r\n            background: rgba(255, 255, 255, 0.9);\r\n            -webkit-transform: scale(1);\r\n            border-radius: 8px;\r\n            overflow: hidden; }\r\n\r\n.dropzone .dz-preview .dz-progress .dz-upload {\r\n              background: #333;\r\n              background: linear-gradient(to bottom, #666, #444);\r\n              position: absolute;\r\n              top: 0;\r\n              left: 0;\r\n              bottom: 0;\r\n              width: 0;\r\n              transition: width 300ms ease-in-out; }\r\n\r\n.dropzone .dz-preview.dz-error .dz-error-message {\r\n            display: block; }\r\n\r\n.dropzone .dz-preview.dz-error:hover .dz-error-message {\r\n            opacity: 1;\r\n            pointer-events: auto; }\r\n\r\n.dropzone .dz-preview .dz-error-message {\r\n            pointer-events: none;\r\n            z-index: 1000;\r\n            position: absolute;\r\n            display: block;\r\n            display: none;\r\n            opacity: 0;\r\n            transition: opacity 0.3s ease;\r\n            border-radius: 8px;\r\n            font-size: 13px;\r\n            top: 130px;\r\n            left: -10px;\r\n            width: 140px;\r\n            background: #be2626;\r\n            background: linear-gradient(to bottom, #be2626, #a92222);\r\n            padding: 0.5em 1.2em;\r\n            color: white; }\r\n\r\n.dropzone .dz-preview .dz-error-message:after {\r\n              content: '';\r\n              position: absolute;\r\n              top: -6px;\r\n              left: 64px;\r\n              width: 0;\r\n              height: 0;\r\n              border-left: 6px solid transparent;\r\n              border-right: 6px solid transparent;\r\n              border-bottom: 6px solid #be2626; }\r\n\r\n"

/***/ }),

/***/ "./src/app/eav-material-controls/adam/browser/adam-browser.component.html":
/*!********************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/browser/adam-browser.component.html ***!
  \********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- TODO: this dropzone class only because css - need to change-->\r\n<!--  -->\r\n<div class=\"dropzone\" *ngIf=\"!disabled\">\r\n  <div *ngIf=\"show\" ngClass=\"{{'adam-scope-' + (adamModeConfig.usePortalRoot ? 'site' : field)}}\">\r\n\r\n    <!-- info for dropping stuff here -->\r\n    <!--  [disabled]=\"disabled\"-->\r\n    <div class=\"dz-preview dropzone-adam\" (click)=\"openUploadClick($event)\" matTooltip=\"{{'Edit.Fields.Hyperlink.Default.AdamUploadLabel' | translate }}\">\r\n      <div class=\"dz-image adam-browse-background-icon adam-browse-background\" xstyle=\"background-color: whitesmoke\">\r\n        <i class=\"eav-icon-up-circled2\"></i>\r\n        <div class=\"adam-short-label\">upload to\r\n          <i ngClass=\"{{adamModeConfig.usePortalRoot ? 'eav-icon-globe' : 'eav-icon-apple'}}\" style=\"font-size: larger\"></i>\r\n        </div>\r\n      </div>\r\n    </div>\r\n\r\n    <!-- info for paste image from clipboard here -->\r\n    <!-- TODO: disable -->\r\n    <div *ngIf=\"!clipboardPasteImageFunctionalityDisabled\" class=\"dz-preview dropzone-adam paste-image\" matTooltip=\"{{'Edit.Fields.Hyperlink.Default.AdamUploadPasteLabel' | translate }}\">\r\n      <div class=\"dz-image adam-browse-background-icon adam-browse-background\" xstyle=\"background-color: whitesmoke\">\r\n        <i class=\"eav-icon-file-image\"></i>\r\n        <div class=\"adam-short-label\">paste image</div>\r\n      </div>\r\n    </div>\r\n\r\n    <!-- add folder - not always shown -->\r\n    <!-- TODOD: disable -->\r\n    <div *ngIf=\"allowCreateFolder()\" class=\"dz-preview\" (click)=\"addFolder()\">\r\n      <div class=\"dz-image adam-browse-background-icon adam-browse-background\">\r\n        <div class=\"\">\r\n          <i class=\"eav-icon-folder-empty\"></i>\r\n          <div class=\"adam-short-label\">new folder</div>\r\n        </div>\r\n      </div>\r\n      <div class=\"adam-background adam-browse-background-icon\">\r\n        <i class=\"eav-icon-plus\" style=\"font-size: 2em; top: 13px; position: relative;\"></i>\r\n      </div>\r\n      <div class=\"dz-details\" style=\"opacity: 1\"></div>\r\n    </div>\r\n\r\n    <!-- browse up a folder - not always shown -->\r\n    <!-- <div  ng-disabled=\"vm.disabled\"> -->\r\n    <ng-container *ngIf=\"showFolders\">\r\n      <div *ngIf=\"folders.length > 0\" class=\"dz-preview\" (click)=\"goUp()\">\r\n        <div class=\"dz-image  adam-browse-background-icon adam-browse-background\">\r\n          <i class=\"eav-icon-folder-empty\"></i>\r\n          <div class=\"adam-short-label\">up</div>\r\n        </div>\r\n        <div class=\"adam-background adam-browse-background-icon\">\r\n          <i class=\"eav-icon-level-up\" style=\"font-size: 2em; top: 13px; position: relative;\"></i>\r\n        </div>\r\n      </div>\r\n    </ng-container>\r\n\r\n    <!-- folder list - not always shown -->\r\n    <ng-container *ngIf=\"showFolders\">\r\n      <div class=\"dz-preview\" *ngFor=\"let item of (items$ | async | filter: 'IsFolder' : true  | filter: 'Name' : '2sxc' : false | filter: 'Name' : 'adam' : false | orderby : 'Name')\"\r\n        (click)=\"goIntoFolder(item)\">\r\n        <div class=\"dz-image adam-blur adam-browse-background-icon adam-browse-background\">\r\n          <i class=\"eav-icon-folder-empty\"></i>\r\n          <div class=\"short-label\">{{ item.Name }}</div>\r\n        </div>\r\n        <div class=\"dz-details file-type-{{item.Type}}\">\r\n          <span (click)=\"del(item)\" appClickStopPropagation class=\"adam-delete-button\">\r\n            <i class=\"eav-icon-cancel\"></i>\r\n          </span>\r\n          <span (click)=\"rename(item)\" appClickStopPropagation class=\"adam-rename-button\">\r\n            <i class=\"eav-icon-pencil\"></i>\r\n          </span>\r\n          <div class=\"adam-full-name-area\">\r\n            <div class=\"adam-full-name\">{{ item.Name }}</div>\r\n          </div>\r\n        </div>\r\n        <span class=\"adam-tag\" [ngClass]=\"{'metadata-exists': item.MetadataId > 0}\" (click)=\"editMetadata(item)\" matTooltip=\"{{getMetadataType(item)}}:{{item.MetadataId}}\"\r\n          appClickStopPropagation *ngIf=\"getMetadataType(item)\">\r\n          <i class=\"eav-icon-tag\" style=\"font-size: larger\"></i>\r\n        </span>\r\n      </div>\r\n    </ng-container>\r\n\r\n    <!-- files -->\r\n    <!-- <div  ng-disabled=\"vm.disabled || !vm.enableSelect\"> -->\r\n    <div class=\"dz-preview\" *ngFor=\"let item of (items$ | async | filter: 'IsFolder' : false | filter: (showImagesOnly ? 'Type' : undefined) : (showImagesOnly ? 'image' : undefined) | fileEndingFilter: allowedFileTypes | orderby : 'Name')\"\r\n      (click)=\"select(item)\" [ngClass]=\"{ 'dz-success': getValueCallback && getValueCallback().toLowerCase() === 'file:' + item.Id }\">\r\n      <div *ngIf=\"item.Type !== 'image'\" class=\"dz-image adam-blur adam-browse-background-icon adam-browse-background\">\r\n        <i [ngClass]=\"icon(item)\"></i>\r\n        <div class=\"adam-short-label\">{{ item.Name }}</div>\r\n      </div>\r\n      <div *ngIf=\"item.Type === 'image'\" class=\"dz-image\">\r\n        <img data-dz-thumbnail=\"\" [alt]=\"item.Id + ':' + item.Name\" [src]=\"item.FullPath + '?w=120&h=120&mode=crop'\">\r\n      </div>\r\n\r\n      <div class=\"dz-details file-type-{{item.Type}}\">\r\n        <span (click)=\"del(item)\" appClickStopPropagation class=\"adam-delete-button\">\r\n          <i class=\"eav-icon-cancel\"></i>\r\n        </span>\r\n        <span (click)=\"rename(item)\" appClickStopPropagation class=\"adam-rename-button\">\r\n          <i class=\"eav-icon-pencil\"></i>\r\n        </span>\r\n        <div class=\"adam-full-name-area\">\r\n          <div class=\"adam-full-name\">{{ item.Name }}</div>\r\n        </div>\r\n        <div class=\"dz-filename adam-short-label\">\r\n          <span>#{{ item.Id }} - {{ (item.Size / 1024).toFixed(0) }} kb</span>\r\n        </div>\r\n        <a class=\"adam-link-button\" target=\"_blank\" [href]=\"item.FullPath\">\r\n          <i class=\"eav-icon-link-ext\" style=\"font-size: larger\"></i>\r\n        </a>\r\n      </div>\r\n      <span class=\"adam-tag\" [ngClass]=\"{'metadata-exists': item.MetadataId > 0}\" (click)=\"editMetadata(item)\" *ngIf=\"getMetadataType(item)\"\r\n        appClickStopPropagation matTooltip=\"{{getMetadataType(item)}}:{{item.MetadataId}}\">\r\n        <i class=\"eav-icon-tag\" style=\"font-size: larger\"></i>\r\n      </span>\r\n\r\n      <div class=\"dz-success-mark\">\r\n        <svg width=\"54px\" height=\"54px\" viewBox=\"0 0 54 54\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\"\r\n          xmlns:sketch=\"http://www.bohemiancoding.com/sketch/ns\">\r\n          <title>Check</title>\r\n          <defs></defs>\r\n          <g id=\"Page-1\" stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\" sketch:type=\"MSPage\">\r\n            <path d=\"M23.5,31.8431458 L17.5852419,25.9283877 C16.0248253,24.3679711 13.4910294,24.366835 11.9289322,25.9289322 C10.3700136,27.4878508 10.3665912,30.0234455 11.9283877,31.5852419 L20.4147581,40.0716123 C20.5133999,40.1702541 20.6159315,40.2626649 20.7218615,40.3488435 C22.2835669,41.8725651 24.794234,41.8626202 26.3461564,40.3106978 L43.3106978,23.3461564 C44.8771021,21.7797521 44.8758057,19.2483887 43.3137085,17.6862915 C41.7547899,16.1273729 39.2176035,16.1255422 37.6538436,17.6893022 L23.5,31.8431458 Z M27,53 C41.3594035,53 53,41.3594035 53,27 C53,12.6405965 41.3594035,1 27,1 C12.6405965,1 1,12.6405965 1,27 C1,41.3594035 12.6405965,53 27,53 Z\"\r\n              id=\"Oval-2\" stroke-opacity=\"0.198794158\" stroke=\"#747474\" fill-opacity=\"0.816519475\" fill=\"#FFFFFF\" sketch:type=\"MSShapeGroup\"></path>\r\n          </g>\r\n        </svg>\r\n      </div>\r\n    </div>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/adam/browser/adam-browser.component.ts":
/*!******************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/browser/adam-browser.component.ts ***!
  \******************************************************************************/
/*! exports provided: AdamBrowserComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AdamBrowserComponent", function() { return AdamBrowserComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _adam_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../adam.service */ "./src/app/eav-material-controls/adam/adam.service.ts");
/* harmony import */ var _shared_services_file_type_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../shared/services/file-type.service */ "./src/app/shared/services/file-type.service.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
/* harmony import */ var _shared_services_feature_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../shared/services/feature.service */ "./src/app/shared/services/feature.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var AdamBrowserComponent = /** @class */ (function () {
    function AdamBrowserComponent(adamService, fileTypeService, eavService, featureService) {
        var _this = this;
        this.adamService = adamService;
        this.fileTypeService = fileTypeService;
        this.eavService = eavService;
        this.featureService = featureService;
        // basic functionality
        this.disabled = false;
        this.show = false;
        this.openUpload = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        // Configuration
        this.adamModeConfig = { usePortalRoot: false };
        this.autoLoad = false;
        this.enableSelect = true;
        this.fileFilter = '';
        this.folderDepth = 0;
        this.subFolder = '';
        this.allowedFileTypes = [];
        this.clipboardPasteImageFunctionalityDisabled = true;
        this.goUp = function () {
            _this.subFolder = _this.svc.goUp();
        };
        this.getMetadataType = function (item) {
            var found;
            // check if it's a folder and if this has a special registration
            if (item.Type === 'folder') {
                found = this.metadataContentTypes.match(/^(folder)(:)([^\n]*)/im);
                if (found) {
                    return found[3];
                }
                else {
                    return null;
                }
            }
            // check if the extension has a special registration
            // -- not implemented yet
            // check if the type "image" or "document" has a special registration
            // -- not implemneted yet
            // nothing found so far, go for the default with nothing as the prefix
            found = this.metadataContentTypes.match(/^([^:\n]*)(\n|$)/im);
            if (found) {
                return found[1];
            }
            // this is if we don't find anything
            return null;
        };
        // load svc...
        // vm.svc = adamSvc(vm.contentTypeName, vm.entityGuid, vm.fieldName, vm.subFolder, $scope.adamModeConfig);
        this.openUploadClick = function (event) { return _this.openUpload.emit(); };
        this.refresh = function () { return _this.svc.liveListReload(); };
        this.itemDefinition = function (item, metadataType) {
            var title = 'EditFormTitle.Metadata'; // todo: i18n
            return item.MetadataId !== 0
                ? { EntityId: item.MetadataId, Title: title } // if defined, return the entity-number to edit
                : {
                    ContentTypeName: metadataType,
                    Metadata: {
                        Key: (item.Type === 'folder' ? 'folder' : 'file') + ':' + item.Id,
                        KeyType: 'string',
                        TargetType: this.metadataOfCmsObject
                    },
                    Title: title,
                    Prefill: { EntityTitle: item.Name } // possibly prefill the entity title
                };
        };
        this.loadFileList = function () { return _this.svc.liveListLoad(); };
        this.eavConfig = this.eavService.getEavConfiguration();
    }
    Object.defineProperty(AdamBrowserComponent.prototype, "folders", {
        get: function () {
            return this.svc ? this.svc.folders : [];
        },
        enumerable: true,
        configurable: true
    });
    AdamBrowserComponent.prototype.ngOnInit = function () {
        this.initConfig();
        // console.log('adam ngOnInit config:', this.config);
        this.svc = this.adamService.createSvc(this.subFolder, this.adamModeConfig, this.url);
        console.log('adam ngOnInit url:', this.url);
        this.setAllowedFileTypes();
        // TODO: when to load folders??? Before was toggle!!!
        this.items$ = this.svc.liveListCache$;
        this.loadFileList();
        // TODO: when set folders??? Before was toggle!!!
        // this.folders = this.svc.folders;
        if (this.autoLoad) {
            this.toggle(null);
        }
    };
    AdamBrowserComponent.prototype.initConfig = function () {
        var _this = this;
        this.subFolder = this.subFolder || '';
        this.showImagesOnly = this.showImagesOnly || false;
        this.folderDepth = (typeof this.folderDepth !== 'undefined' && this.folderDepth !== null) ? this.folderDepth : 2;
        this.showFolders = !!this.folderDepth;
        this.allowAssetsInRoot = this.allowAssetsInRoot || true; // if true, the initial folder can have files, otherwise only subfolders
        this.metadataContentTypes = this.metadataContentTypes || '';
        // TODO:
        // appRoot = read app root
        this.enableSelect = (this.enableSelect === false) ? false : true; // must do it like this, $scope.enableSelect || true will not work
        // if feature clipboardPasteImageFunctionality enabled
        this.featureService.enabled('f6b8d6da-4744-453b-9543-0de499aa2352', this.eavConfig.appId, this.url)
            .subscribe(function (enabled) {
            return _this.clipboardPasteImageFunctionalityDisabled = (enabled === false);
        });
    };
    AdamBrowserComponent.prototype.addFolder = function () {
        if (this.disabled) {
            return;
        }
        var folderName = window.prompt('Please enter a folder name'); // todo i18n
        if (folderName) {
            this.svc.addFolder(folderName).subscribe();
        }
    };
    AdamBrowserComponent.prototype.allowCreateFolder = function () {
        return this.svc.folders.length < this.folderDepth;
    };
    AdamBrowserComponent.prototype.del = function (item) {
        if (this.disabled) {
            return;
        }
        var ok = window.confirm('Are you sure you want to delete this item?'); // todo i18n
        if (ok) {
            this.svc.deleteItem(item).subscribe();
        }
    };
    AdamBrowserComponent.prototype.editMetadata = function (item) {
        var items = [
            this.itemDefinition(item, this.getMetadataType(item))
        ];
        // TODO:
        // eavAdminDialogs.openEditItems(items, vm.refresh);
    };
    //#region Folder Navigation
    AdamBrowserComponent.prototype.goIntoFolder = function (folder) {
        var subFolder = this.svc.goIntoFolder(folder);
        // this.refresh();
        this.subFolder = subFolder;
    };
    AdamBrowserComponent.prototype.icon = function (item) {
        return this.fileTypeService.getIconClass(item.Name);
    };
    AdamBrowserComponent.prototype.rename = function (item) {
        var newName = window.prompt('Rename the file / folder to: ', item.Name);
        if (newName) {
            this.svc.rename(item, newName).subscribe();
        }
    };
    AdamBrowserComponent.prototype.select = function (fileItem) {
        if (this.disabled || !this.enableSelect) {
            return;
        }
        this.updateCallback(fileItem);
    };
    AdamBrowserComponent.prototype.toggle = function (newConfig) {
        // Reload configuration
        this.initConfig();
        var configChanged = false;
        if (newConfig) {
            // Detect changes in config, allows correct toggle behaviour
            if (JSON.stringify(newConfig) !== this.oldConfig) {
                configChanged = true;
            }
            this.oldConfig = JSON.stringify(newConfig);
            this.showImagesOnly = newConfig.showImagesOnly;
            this.adamModeConfig.usePortalRoot = !!(newConfig.usePortalRoot);
        }
        this.show = configChanged || !this.show;
        if (!this.show) {
            this.adamModeConfig.usePortalRoot = false;
        }
        // Override configuration in portal mode
        if (this.adamModeConfig.usePortalRoot) {
            this.showFolders = true;
            this.folderDepth = 99;
        }
        if (this.show) {
            this.refresh();
        }
    };
    /**
     * set configuration (called from input type)
     * @param adamConfig
     */
    AdamBrowserComponent.prototype.setConfig = function (adamConfig) {
        console.log('adam browser setAdamConfig', adamConfig);
        this.allowAssetsInRoot = adamConfig.allowAssetsInRoot;
        this.autoLoad = adamConfig.autoLoad;
        this.enableSelect = adamConfig.enableSelect;
        this.fileFilter = adamConfig.fileFilter;
        this.folderDepth = adamConfig.folderDepth;
        this.metadataContentTypes = adamConfig.metadataContentTypes;
        this.showImagesOnly = adamConfig.showImagesOnly;
        this.subFolder = adamConfig.subFolder;
        // Reload configuration
        this.initConfig();
        this.show = this.autoLoad;
        if (this.show) {
            this.refresh();
        }
    };
    AdamBrowserComponent.prototype.setAllowedFileTypes = function () {
        if (this.fileFilter) {
            this.allowedFileTypes = this.fileFilter.split(',').map(function (i) {
                return i.replace('*', '').trim();
            });
        }
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], AdamBrowserComponent.prototype, "metadataOfCmsObject", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], AdamBrowserComponent.prototype, "url", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], AdamBrowserComponent.prototype, "disabled", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], AdamBrowserComponent.prototype, "show", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"])(),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"])
    ], AdamBrowserComponent.prototype, "openUpload", void 0);
    AdamBrowserComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'adam-browser',
            template: __webpack_require__(/*! ./adam-browser.component.html */ "./src/app/eav-material-controls/adam/browser/adam-browser.component.html"),
            styles: [__webpack_require__(/*! ./adam-browser.component.css */ "./src/app/eav-material-controls/adam/browser/adam-browser.component.css")]
        }),
        __metadata("design:paramtypes", [_adam_service__WEBPACK_IMPORTED_MODULE_1__["AdamService"],
            _shared_services_file_type_service__WEBPACK_IMPORTED_MODULE_2__["FileTypeService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_3__["EavService"],
            _shared_services_feature_service__WEBPACK_IMPORTED_MODULE_4__["FeatureService"]])
    ], AdamBrowserComponent);
    return AdamBrowserComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/adam/dropzone/dropzone.component.css":
/*!****************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/dropzone/dropzone.component.css ***!
  \****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "/* .field-hints {\r\n    visibility: hidden;\r\n    opacity: 0;\r\n    transition-duration: 200ms, 200ms;\r\n    transition-property: opacity, visibility;\r\n    transition-delay: 0, 200ms;\r\n  }\r\n  \r\n  div.mce-edit-focus .field-hints {\r\n    visibility: visible;\r\n    opacity:1;\r\n  } */"

/***/ }),

/***/ "./src/app/eav-material-controls/adam/dropzone/dropzone.component.html":
/*!*****************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/dropzone/dropzone.component.html ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"dropzone dropzone-container\" [dropzone]=\"dropzoneConfig\" [disabled]=\"disabled\" (error)=\"onUploadError($event)\"\r\n  (success)=\"onUploadSuccess($event)\" (drop)=\"onDrop($event)\">\r\n  <ng-container #fieldComponent></ng-container>\r\n  <!-- [autoLoad]=\"false\" [folderDepth]=\"0\" [subFolder]=\"\" fileFilter=\"*.jpg,*.pdf.,*.css\" [enableSelect]=\"true\"-->\r\n  <adam-browser (openUpload)=\"openUpload()\" [url]=\"url\" [metadataOfCmsObject]=\"\">\r\n  </adam-browser>\r\n  <!-- ng-show=\"uploading\" -->\r\n  <div class=\"{{'field-' + config.index}}\">\r\n    <div class=\"dropzone-previews\">\r\n    </div>\r\n    <span #invisibleClickable class=\"invisible-clickable\" data-note=\"just a fake, invisible area for dropzone\"></span>\r\n  </div>\r\n  <!-- todo: focus with css -->\r\n  <adam-hint class=\"field-hints\"></adam-hint>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/adam/dropzone/dropzone.component.ts":
/*!***************************************************************************!*\
  !*** ./src/app/eav-material-controls/adam/dropzone/dropzone.component.ts ***!
  \***************************************************************************/
/*! exports provided: DropzoneComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DropzoneComponent", function() { return DropzoneComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var ngx_dropzone_wrapper__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ngx-dropzone-wrapper */ "./node_modules/ngx-dropzone-wrapper/dist/ngx-dropzone-wrapper.es5.js");
/* harmony import */ var _browser_adam_browser_component__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../browser/adam-browser.component */ "./src/app/eav-material-controls/adam/browser/adam-browser.component.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
/* harmony import */ var _shared_helpers_url_helper__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../shared/helpers/url-helper */ "./src/app/shared/helpers/url-helper.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var DropzoneComponent = /** @class */ (function () {
    function DropzoneComponent(eavService) {
        this.eavService = eavService;
        this.tempFileFilter = '*.jpg,*.pdf';
        this.eavConfig = this.eavService.getEavConfiguration();
    }
    Object.defineProperty(DropzoneComponent.prototype, "disabled", {
        get: function () {
            return this.group.controls[this.config.name].disabled;
        },
        enumerable: true,
        configurable: true
    });
    DropzoneComponent.prototype.ngOnInit = function () {
        this.config.adam = this.adamRef;
        // const serviceRoot = 'http://2sxc-dnn742.dnndev.me/en-us/desktopmodules/2sxc/api/';
        var serviceRoot = this.eavConfig.portalroot + 'desktopmodules/2sxc/api/';
        // const url = UrlHelper.resolveServiceUrl('app-content/' + contentType + '/' + entityGuid + '/' + field, serviceRoot);
        var contentType = this.config.header.contentTypeName;
        // const contentType = '106ba6ed-f807-475a-b004-cd77e6b317bd';
        var entityGuid = this.config.header.guid;
        // const entityGuid = '386ec145-d884-4fea-935b-a4d8d0c68d8d';
        var field = this.config.name;
        // const field = 'HyperLinkStaticName';
        this.url = _shared_helpers_url_helper__WEBPACK_IMPORTED_MODULE_4__["UrlHelper"].resolveServiceUrl("app-content/" + contentType + "/" + entityGuid + "/" + field, serviceRoot);
        console.log('', this.url);
        this.dropzoneConfig = {
            url: this.url + ("?usePortalRoot=" + this.eavConfig.portalroot + "false&appId=" + this.eavConfig.appId),
            maxFiles: 1,
            autoReset: null,
            errorReset: null,
            cancelReset: null,
            // 'http://2sxc-dnn742.dnndev.me/en-us/desktopmodules/2sxc/api/app-content/106ba6ed-f807-475a-b004-cd77e6b317bd/
            // 386ec145-d884-4fea-935b-a4d8d0c68d8d/HyperLinkStaticName?usePortalRoot=false&appId=7',
            // urlRoot: 'http://2sxc-dnn742.dnndev.me/',
            maxFilesize: 10000,
            paramName: 'uploadfile',
            maxThumbnailFilesize: 10,
            headers: {
                'ModuleId': this.eavConfig.mid,
                'TabId': this.eavConfig.tid,
                'ContentBlockId': this.eavConfig.cbid
            },
            dictDefaultMessage: '',
            addRemoveLinks: false,
            // '.field-' + field.toLowerCase() + ' .dropzone-previews',
            previewsContainer: '.dropzone-previews',
            // we need a clickable, because otherwise the entire area is clickable.
            // so i'm just making the preview clickable, as it's not important
            clickable: '.dropzone-previews' // '.field-' + this.config.index + ' .invisible-clickable'  // " .dropzone-adam"
        };
    };
    DropzoneComponent.prototype.ngAfterViewInit = function () {
        this.dropzoneConfig.previewsContainer = '.field-' + this.config.index + ' .dropzone-previews';
        this.dropzoneConfig.clickable = '.field-' + this.config.index + ' .invisible-clickable';
        console.log('this.dropzoneConfig:', this.dropzoneConfig);
        console.log('config ddropzone wrapper:', this.config.index);
    };
    DropzoneComponent.prototype.onUploadError = function (args) {
        console.log('onUploadError:', args);
    };
    DropzoneComponent.prototype.onUploadSuccess = function (args) {
        console.log('onUploadSuccess:', args);
        var response = args[1]; // Gets the server response as second argument.
        if (response.Success) {
            this.adamRef.svc.addFullPath(response); // calculate additional infos
            this.adamRef.afterUploadCallback(response);
            // Reset dropzone
            this.dropzoneRef.reset();
            this.adamRef.refresh();
        }
        else {
            alert('Upload failed because: ' + response.Error);
        }
    };
    DropzoneComponent.prototype.onDrop = function (args) {
        // this.adamRef.updateCallback();
    };
    /**
     * triger click on clickable element for load open
     */
    DropzoneComponent.prototype.openUpload = function () {
        console.log('openUpload click');
        this.invisibleClickableReference.nativeElement.click();
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('fieldComponent', { read: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"] }),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"])
    ], DropzoneComponent.prototype, "fieldComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(ngx_dropzone_wrapper__WEBPACK_IMPORTED_MODULE_1__["DropzoneDirective"]),
        __metadata("design:type", ngx_dropzone_wrapper__WEBPACK_IMPORTED_MODULE_1__["DropzoneDirective"])
    ], DropzoneComponent.prototype, "dropzoneRef", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('invisibleClickable'),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ElementRef"])
    ], DropzoneComponent.prototype, "invisibleClickableReference", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_browser_adam_browser_component__WEBPACK_IMPORTED_MODULE_2__["AdamBrowserComponent"]),
        __metadata("design:type", _browser_adam_browser_component__WEBPACK_IMPORTED_MODULE_2__["AdamBrowserComponent"])
    ], DropzoneComponent.prototype, "adamRef", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], DropzoneComponent.prototype, "config", void 0);
    DropzoneComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-dropzone',
            template: __webpack_require__(/*! ./dropzone.component.html */ "./src/app/eav-material-controls/adam/dropzone/dropzone.component.html"),
            styles: [__webpack_require__(/*! ./dropzone.component.css */ "./src/app/eav-material-controls/adam/dropzone/dropzone.component.css")]
        }),
        __metadata("design:paramtypes", [_shared_services_eav_service__WEBPACK_IMPORTED_MODULE_3__["EavService"]])
    ], DropzoneComponent);
    return DropzoneComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/eav-material-controls.module.ts":
/*!***********************************************************************!*\
  !*** ./src/app/eav-material-controls/eav-material-controls.module.ts ***!
  \***********************************************************************/
/*! exports provided: EavMaterialControlsModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavMaterialControlsModule", function() { return EavMaterialControlsModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm5/common.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm5/ngx-translate-core.js");
/* harmony import */ var ng_pick_datetime__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ng-pick-datetime */ "./node_modules/ng-pick-datetime/picker.js");
/* harmony import */ var ngx_dropzone_wrapper__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ngx-dropzone-wrapper */ "./node_modules/ngx-dropzone-wrapper/dist/ngx-dropzone-wrapper.es5.js");
/* harmony import */ var _wrappers__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./wrappers */ "./src/app/eav-material-controls/wrappers/index.ts");
/* harmony import */ var _input_types__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./input-types */ "./src/app/eav-material-controls/input-types/index.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
/* harmony import */ var _wrappers_text_entry_wrapper_text_entry_wrapper_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./wrappers/text-entry-wrapper/text-entry-wrapper.component */ "./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.ts");
/* harmony import */ var _wrappers_field_parent_wrapper_error_wrapper_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./wrappers/field-parent-wrapper/error-wrapper.component */ "./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.ts");
/* harmony import */ var _wrappers_eav_localization_wrapper_eav_localization_wrapper_component__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./wrappers/eav-localization-wrapper/eav-localization-wrapper.component */ "./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.ts");
/* harmony import */ var _shared_services_file_type_service__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../shared/services/file-type.service */ "./src/app/shared/services/file-type.service.ts");
/* harmony import */ var _localization_eav_language_switcher_eav_language_switcher_component__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./localization/eav-language-switcher/eav-language-switcher.component */ "./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.ts");
/* harmony import */ var _adam_browser_adam_browser_component__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./adam/browser/adam-browser.component */ "./src/app/eav-material-controls/adam/browser/adam-browser.component.ts");
/* harmony import */ var _adam_adam_hint_adam_hint_component__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./adam/adam-hint/adam-hint.component */ "./src/app/eav-material-controls/adam/adam-hint/adam-hint.component.ts");
/* harmony import */ var _adam_dropzone_dropzone_component__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ./adam/dropzone/dropzone.component */ "./src/app/eav-material-controls/adam/dropzone/dropzone.component.ts");
/* harmony import */ var _shared_pipes_filter_pipe__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ../shared/pipes/filter.pipe */ "./src/app/shared/pipes/filter.pipe.ts");
/* harmony import */ var _shared_pipes_orderby_pipe__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ../shared/pipes/orderby.pipe */ "./src/app/shared/pipes/orderby.pipe.ts");
/* harmony import */ var _shared_directives_click_stop_propagination_directive__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ../shared/directives/click-stop-propagination.directive */ "./src/app/shared/directives/click-stop-propagination.directive.ts");
/* harmony import */ var _shared_pipes_file_ending_filter_pipe__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ../shared/pipes/file-ending-filter.pipe */ "./src/app/shared/pipes/file-ending-filter.pipe.ts");
/* harmony import */ var _input_types_hyperlink_hyperlink_library_hyperlink_library_component__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ./input-types/hyperlink/hyperlink-library/hyperlink-library.component */ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.ts");
/* harmony import */ var _wrappers_hidden_wrapper_hidden_wrapper_component__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ./wrappers/hidden-wrapper/hidden-wrapper.component */ "./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
























var EavMaterialControlsModule = /** @class */ (function () {
    function EavMaterialControlsModule() {
    }
    EavMaterialControlsModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            declarations: [
                // wrappers
                _wrappers__WEBPACK_IMPORTED_MODULE_7__["CollapsibleWrapperComponent"],
                _wrappers_field_parent_wrapper_error_wrapper_component__WEBPACK_IMPORTED_MODULE_11__["ErrorWrapperComponent"],
                _wrappers_text_entry_wrapper_text_entry_wrapper_component__WEBPACK_IMPORTED_MODULE_10__["TextEntryWrapperComponent"],
                // types
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringUrlPathComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringDropdownComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringDropdownQueryComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringFontIconPickerComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["BooleanDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["DatetimeDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["EmptyDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["NumberDefaultComponent"],
                _wrappers_eav_localization_wrapper_eav_localization_wrapper_component__WEBPACK_IMPORTED_MODULE_12__["EavLocalizationComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["EntityDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["HyperlinkDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["ExternalComponent"],
                _adam_browser_adam_browser_component__WEBPACK_IMPORTED_MODULE_15__["AdamBrowserComponent"],
                _adam_adam_hint_adam_hint_component__WEBPACK_IMPORTED_MODULE_16__["AdamHintComponent"],
                _adam_dropzone_dropzone_component__WEBPACK_IMPORTED_MODULE_17__["DropzoneComponent"],
                _input_types_hyperlink_hyperlink_library_hyperlink_library_component__WEBPACK_IMPORTED_MODULE_22__["HyperlinkLibraryComponent"],
                _localization_eav_language_switcher_eav_language_switcher_component__WEBPACK_IMPORTED_MODULE_14__["EavLanguageSwitcherComponent"],
                _shared_pipes_filter_pipe__WEBPACK_IMPORTED_MODULE_18__["FilterPipe"],
                _shared_pipes_orderby_pipe__WEBPACK_IMPORTED_MODULE_19__["OrderByPipe"],
                _shared_pipes_file_ending_filter_pipe__WEBPACK_IMPORTED_MODULE_21__["FileEndingFilterPipe"],
                _shared_directives_click_stop_propagination_directive__WEBPACK_IMPORTED_MODULE_20__["ClickStopPropagationDirective"],
                _wrappers_hidden_wrapper_hidden_wrapper_component__WEBPACK_IMPORTED_MODULE_23__["HiddenWrapperComponent"]
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_1__["CommonModule"],
                ngx_dropzone_wrapper__WEBPACK_IMPORTED_MODULE_6__["DropzoneModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_2__["ReactiveFormsModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatFormFieldModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatButtonModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatInputModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatCheckboxModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatSelectModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatDatepickerModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatNativeDateModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatCardModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatIconModule"],
                ng_pick_datetime__WEBPACK_IMPORTED_MODULE_5__["OwlDateTimeModule"],
                ng_pick_datetime__WEBPACK_IMPORTED_MODULE_5__["OwlNativeDateTimeModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatGridListModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatAutocompleteModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatListModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatMenuModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatTooltipModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatTabsModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatProgressSpinnerModule"],
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_4__["TranslateModule"].forChild()
            ],
            entryComponents: [
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringUrlPathComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringDropdownComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringDropdownQueryComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["StringFontIconPickerComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["BooleanDefaultComponent"],
                _wrappers_text_entry_wrapper_text_entry_wrapper_component__WEBPACK_IMPORTED_MODULE_10__["TextEntryWrapperComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["DatetimeDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["EmptyDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["NumberDefaultComponent"],
                _wrappers_eav_localization_wrapper_eav_localization_wrapper_component__WEBPACK_IMPORTED_MODULE_12__["EavLocalizationComponent"],
                _wrappers_field_parent_wrapper_error_wrapper_component__WEBPACK_IMPORTED_MODULE_11__["ErrorWrapperComponent"],
                _wrappers__WEBPACK_IMPORTED_MODULE_7__["CollapsibleWrapperComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["EntityDefaultComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["HyperlinkDefaultComponent"],
                _input_types_hyperlink_hyperlink_library_hyperlink_library_component__WEBPACK_IMPORTED_MODULE_22__["HyperlinkLibraryComponent"],
                _input_types__WEBPACK_IMPORTED_MODULE_8__["ExternalComponent"],
                _adam_dropzone_dropzone_component__WEBPACK_IMPORTED_MODULE_17__["DropzoneComponent"],
                _wrappers_hidden_wrapper_hidden_wrapper_component__WEBPACK_IMPORTED_MODULE_23__["HiddenWrapperComponent"]
            ],
            exports: [_localization_eav_language_switcher_eav_language_switcher_component__WEBPACK_IMPORTED_MODULE_14__["EavLanguageSwitcherComponent"]],
            providers: [_shared_services_file_type_service__WEBPACK_IMPORTED_MODULE_13__["FileTypeService"], _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_9__["ValidationMessagesService"]]
        })
    ], EavMaterialControlsModule);
    return EavMaterialControlsModule;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.css":
/*!*********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.css ***!
  \*********************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.html":
/*!**********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.html ***!
  \**********************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div [formGroup]=\"group\">\r\n  <!-- <mat-form-field > -->\r\n  <mat-checkbox [formControlName]=\"config.name\" labelPosition=\"start\">\r\n    {{config.label}}\r\n  </mat-checkbox>\r\n  <!-- </mat-form-field> -->\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.ts":
/*!********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.ts ***!
  \********************************************************************************************************/
/*! exports provided: BooleanDefaultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BooleanDefaultComponent", function() { return BooleanDefaultComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/checkbox */ "./node_modules/@angular/material/esm5/checkbox.es5.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var BooleanDefaultComponent = /** @class */ (function () {
    function BooleanDefaultComponent() {
    }
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_angular_material_checkbox__WEBPACK_IMPORTED_MODULE_1__["MatCheckbox"]),
        __metadata("design:type", _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_1__["MatCheckbox"])
    ], BooleanDefaultComponent.prototype, "matCheckbox", void 0);
    BooleanDefaultComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'boolean-default',
            template: __webpack_require__(/*! ./boolean-default.component.html */ "./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.html"),
            styles: [__webpack_require__(/*! ./boolean-default.component.css */ "./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_2__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        })
    ], BooleanDefaultComponent);
    return BooleanDefaultComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/custom/external/external.component.css":
/*!******************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/custom/external/external.component.css ***!
  \******************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/custom/external/external.component.html":
/*!*******************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/custom/external/external.component.html ***!
  \*******************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n    <input matInput type=\"text\" id=\"demo\" class=\"form-control material\" [formControlName]=\"config.name\" [placeholder]=\"config.label\"\r\n        [required]=\"config.required\" #someVar1>\r\n</mat-form-field> -->\r\n\r\n<!-- style=\"display:none;\" -->\r\n\r\n<mat-spinner *ngIf=\"loaded\"></mat-spinner>\r\n<div #container></div>\r\n\r\n<mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n<mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n\r\n\r\n<!-- <div [innerHtml]=\"html\"></div> -->"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/custom/external/external.component.ts":
/*!*****************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/custom/external/external.component.ts ***!
  \*****************************************************************************************/
/*! exports provided: ExternalComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExternalComponent", function() { return ExternalComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var ExternalComponent = /** @class */ (function () {
    function ExternalComponent(validationMessagesService, eavService) {
        var _this = this;
        this.validationMessagesService = validationMessagesService;
        this.eavService = eavService;
        this.subscriptions = [];
        this.loaded = true;
        /**
         * This is host methods which the external control sees
         */
        this.externalInputTypeHost = {
            update: function (value) { return _this.update(value); },
            setInitValues: function (value) { return _this.setInitValues(); },
            // toggleAdam: (value1, value2) => this.toggleAdam(value1, value2),
            // adamModeImage: () => (this.config && this.config.adam) ? this.config.adam.showImagesOnly : null,
            attachAdam: function () { return _this.attachAdam(); }
        };
    }
    Object.defineProperty(ExternalComponent.prototype, "factory", {
        set: function (value) {
            console.log('set factory', value);
            if (value) {
                this.renderExternalComponent(value);
                this.subscribeFormChange(value);
                this.externalFactory = value;
            }
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ExternalComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ExternalComponent.prototype, "id", {
        get: function () {
            return "" + this.config.entityId + this.config.index;
        },
        enumerable: true,
        configurable: true
    });
    // TODO: need to finish validation
    ExternalComponent.prototype.getErrorMessage = function () {
        // console.log('trigger getErrorMessage1:', this.config.name);
        // console.log('trigger getErrorMessage:',
        var _this = this;
        var formError = '';
        var control = this.group.controls[this.config.name];
        if (control) {
            var messages_1 = this.validationMessagesService.validationMessages();
            if (control && control.invalid) {
                // if ((control.dirty || control.touched)) {
                // if (this.externalFactory && this.externalFactory.isDirty) {
                Object.keys(control.errors).forEach(function (key) {
                    if (messages_1[key]) {
                        formError = messages_1[key](_this.config);
                    }
                });
                // }
                // }
            }
        }
        // console.log('control.dirty:', control.dirty);
        // console.log('control.touched:', control.touched);
        return formError;
        // this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config));
        // return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    ExternalComponent.prototype.ngOnInit = function () { };
    ExternalComponent.prototype.renderExternalComponent = function (factory) {
        console.log('this.customInputTypeHost', this.externalInputTypeHost);
        console.log('this.customInputTypeHost', this.elReference.nativeElement);
        factory.initialize(this.externalInputTypeHost, this.config, this.id);
        factory.render(this.elReference.nativeElement);
        console.log('factory.writeValue(', this.group.controls[this.config.name].value);
        // factory.writeValue(this.elReference.nativeElement, this.group.controls[this.config.name].value);
        // this.setExternalControlValues(factory, this.group.controls[this.config.name].value);
        this.suscribeValueChanges(factory);
        // this.subscribeToCurrentLanguageFromStore(factory);
        this.loaded = false;
    };
    ExternalComponent.prototype.update = function (value) {
        console.log('ExternalComponent update change', value);
        // TODO: validate value
        this.group.controls[this.config.name].patchValue(value);
    };
    /**
     * Set initial values when external component is initialized
     */
    ExternalComponent.prototype.setInitValues = function () {
        this.setExternalControlValues(this.externalFactory, this.group.controls[this.config.name].value);
    };
    ExternalComponent.prototype.attachAdam = function () {
        var _this = this;
        // TODO:
        // If adam registered then attach Adam
        console.log('setInitValues');
        if (this.config.adam) {
            console.log('adam is registered - adam attached updateCallback', this.externalFactory);
            // set update callback = external method setAdamValue
            // callbacks - functions called from adam
            this.config.adam.updateCallback = function (value) {
                return _this.externalFactory.adamSetValue
                    ? _this.externalFactory.adamSetValue(value)
                    : alert('adam attached but adamSetValue method not exist');
            };
            this.config.adam.afterUploadCallback = function (value) {
                return _this.externalFactory.adamAfterUpload
                    ? _this.externalFactory.adamAfterUpload(value)
                    : alert('adam attached but adamAfterUpload method not exist');
            };
            // return value from form
            this.config.adam.getValueCallback = function () { return _this.group.controls[_this.config.name].value; };
            return {
                toggleAdam: function (value1, value2) { return _this.config.adam.toggle(value1); },
                setAdamConfig: function (adamConfig) { return _this.config.adam.setConfig(adamConfig); },
                adamModeImage: function () { return (_this.config && _this.config.adam) ? _this.config.adam.showImagesOnly : null; },
            };
        }
    };
    /**
     * subscribe to form value changes for this field
     */
    ExternalComponent.prototype.suscribeValueChanges = function (factory) {
        var _this = this;
        this.subscriptions.push(this.group.controls[this.config.name].valueChanges.subscribe(function (item) {
            console.log('ExternalComponent suscribeValueChanges', item);
            _this.setExternalControlValues(factory, item);
        }));
    };
    /**
     * This is subscribe for all setforms - even if is not changing value.
     * @param factory
     */
    ExternalComponent.prototype.subscribeFormChange = function (factory) {
        var _this = this;
        this.subscriptions.push(this.eavService.formSetValueChange$.subscribe(function (item) {
            console.log('Formm CHANGEEEEEEEEEEEEEEEEEE', item);
            _this.setExternalControlValues(factory, item[_this.config.name]);
        }));
    };
    /**
     * write value from the form into the view in external component
     * @param factory
     * @param value
     */
    ExternalComponent.prototype.setExternalControlValues = function (factory, value) {
        // if container have value
        if (this.elReference.nativeElement.innerHTML) {
            if (value) {
                console.log('set valueeee', value);
                factory.setValue(this.elReference.nativeElement, value);
            }
            factory.setOptions(this.elReference.nativeElement, this.group.controls[this.config.name].disabled);
            // this.setAdamOptions();
        }
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('container'),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ElementRef"])
    ], ExternalComponent.prototype, "elReference", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], ExternalComponent.prototype, "config", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], ExternalComponent.prototype, "factory", null);
    ExternalComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'external',
            template: __webpack_require__(/*! ./external.component.html */ "./src/app/eav-material-controls/input-types/custom/external/external.component.html"),
            styles: [__webpack_require__(/*! ./external.component.css */ "./src/app/eav-material-controls/input-types/custom/external/external.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__["InputType"])({
            wrapper: ['app-dropzone', 'app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__["ValidationMessagesService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_3__["EavService"]])
    ], ExternalComponent);
    return ExternalComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.css":
/*!************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.css ***!
  \************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.html":
/*!*************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.html ***!
  \*************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"config.settings.UseTimePicker && config.settings.UseTimePicker === true; then useTimePickerTemplate else notUseTimePickerTemplate\"></div>\r\n\r\n<ng-template #notUseTimePickerTemplate>\r\n  <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n    <input matInput [matDatepicker]=\"picker\" class=\"form-control material\" [formControlName]=\"config.name\" [placeholder]=\"config.label\"\r\n      [required]='config.required'>\r\n    <!-- [matDatepickerFilter]=\"to.datepickerOptions.filter\" -->\r\n    <!-- (dateChange)=\"to.change(field, formControl)\" -->\r\n    <mat-datepicker-toggle matSuffix [for]=\"picker\"></mat-datepicker-toggle>\r\n    <mat-datepicker #picker></mat-datepicker>\r\n    <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n    <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n  </mat-form-field>\r\n</ng-template>\r\n\r\n<ng-template #useTimePickerTemplate>\r\n  <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n    <input matInput [formControlName]=\"config.name\" [owlDateTime]=\"picker\" [placeholder]=\"config.label\" [required]='config.required'>\r\n    <!-- (dateTimeChange)=\"to.change(field, formControl)\" -->\r\n    <!-- control from https://github.com/DanielYKPan/date-time-picker -->\r\n    <owl-date-time #picker></owl-date-time>\r\n\r\n    <button matSuffix type=\"button\" mat-icon-button [owlDateTimeTrigger]=\"picker\">\r\n      <mat-icon class=\"mat-24\">today</mat-icon>\r\n    </button>\r\n    <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n    <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n  </mat-form-field>\r\n</ng-template>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.ts":
/*!***********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.ts ***!
  \***********************************************************************************************************/
/*! exports provided: DatetimeDefaultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DatetimeDefaultComponent", function() { return DatetimeDefaultComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var DatetimeDefaultComponent = /** @class */ (function () {
    function DatetimeDefaultComponent(validationMessagesService) {
        this.validationMessagesService = validationMessagesService;
    }
    Object.defineProperty(DatetimeDefaultComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    DatetimeDefaultComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    DatetimeDefaultComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'datetime-default',
            template: __webpack_require__(/*! ./datetime-default.component.html */ "./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.html"),
            styles: [__webpack_require__(/*! ./datetime-default.component.css */ "./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__["ValidationMessagesService"]])
    ], DatetimeDefaultComponent);
    return DatetimeDefaultComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.css":
/*!***************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.css ***!
  \***************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.html":
/*!****************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.html ***!
  \****************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<span></span>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.ts":
/*!**************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.ts ***!
  \**************************************************************************************************/
/*! exports provided: EmptyDefaultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EmptyDefaultComponent", function() { return EmptyDefaultComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var EmptyDefaultComponent = /** @class */ (function () {
    function EmptyDefaultComponent() {
    }
    EmptyDefaultComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'empty-default',
            template: __webpack_require__(/*! ./empty-default.component.html */ "./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.html"),
            styles: [__webpack_require__(/*! ./empty-default.component.css */ "./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.css")]
        })
    ], EmptyDefaultComponent);
    return EmptyDefaultComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.css":
/*!******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.css ***!
  \******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.html":
/*!*******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.html ***!
  \*******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- !modes.freeTextMode && -->\r\n<mat-list *ngIf=\"enableCreate || (chosenEntities && chosenEntities.length > 0)\">\r\n  <mat-list-item *ngFor=\"let item of chosenEntities; let i = index\">\r\n    <p matLine>\r\n      <!-- ng-show=\"!allowMultiValue\" -->\r\n      <!-- <i class=\"material-icons\">check</i> -->\r\n      <button mat-icon-button *ngIf=\"allowMultiValue\" [disabled]=\"chosenEntities[0] === item || disabled\" type=\"button\" (click)=\"levelUp(item, i)\">\r\n        <mat-icon class=\"mat-24\">arrow_upward</mat-icon>\r\n      </button>\r\n      <button mat-icon-button *ngIf=\"allowMultiValue\" [disabled]=\"chosenEntities[chosenEntities.length - 1] === item || disabled\"\r\n        type=\"button\" (click)=\"levelDown(item, i)\">\r\n        <mat-icon class=\"mat-24\">arrow_downward</mat-icon>\r\n      </button>\r\n      <i title=\"\" class=\"eav-icon-link pull-left eav-entityselect-icon\" *ngIf=\"allowMultiValue\"></i>\r\n      <i title=\"\" class=\"eav-icon-link pull-left eav-entityselect-icon\" *ngIf=\"!allowMultiValue\"></i>\r\n      <span [title]=\"getEntityText(item) + ' (' + item + ')'\">{{ getEntityText(item) }}</span>\r\n      <button mat-icon-button *ngIf=\"enableEdit\" type=\"button\" (click)=\"edit(item)\" title=\"{{ 'FieldType.Entity.Edit' | translate }}\"\r\n        [disabled]=\"disabled\">\r\n        <i class=\"eav-icon-pencil\"></i>\r\n      </button>\r\n      <button mat-icon-button *ngIf=\"enableRemove\" type=\"button\" (click)=\"removeSlot(item, i)\" title=\"{{ 'FieldType.Entity.Remove' | translate }}\"\r\n        [disabled]=\"disabled\">\r\n        <i class=\"{{allowMultiValue ? 'eav-icon-minus-circled' : 'eav-icon-down-dir'}}\"></i>\r\n      </button>\r\n      <button mat-icon-button *ngIf=\"enableDelete\" type=\"button\" (click)=\"deleteItemInSlot(item)\" title=\"{{ 'FieldType.Entity.Delete' | translate }}\"\r\n        [disabled]=\"disabled\">\r\n        <i class=\"eav-icon-cancel\"></i>\r\n      </button>\r\n    </p>\r\n  </mat-list-item>\r\n</mat-list>\r\n\r\n<!-- pick existing entity -->\r\n<!-- ng-if=\"!modes.freeTextMode\" -->\r\n<div *ngIf=\"enableAddExisting && (allowMultiValue || (chosenEntities && chosenEntities.length < 1))\">\r\n  <mat-form-field [style.width]=\"'100%'\" [formGroup]=\"group\">\r\n\r\n    <!--(click)=\"setSelectEntitiesObservables()\"  -->\r\n    <input matInput type=\"text\" #autocompleteInput [id]=\"id\" [placeholder]=\"config.label\" [matAutocomplete]=\"auto\" [disabled]=\"disabled\"\r\n      [placeholder]=\"config.label\" [required]=\"config.required\">\r\n    <mat-autocomplete #auto=\"matAutocomplete\" (optionSelected)=\"optionSelected($event)\">\r\n      <mat-option *ngFor=\"let item of (selectEntities | async)\" [value]=\"item.Value\" [disabled]=\"isInChosenEntities(item.Value)\">\r\n        <span>{{item.Text ? item.Text: item.Value}} {{value}}</span>\r\n      </mat-option>\r\n    </mat-autocomplete>\r\n    <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n  </mat-form-field>\r\n  <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n  <!-- <span ng-if=\"!error\">{{ 'FieldType.EntityQuery.QueryNoItems' | translate }}</span> -->\r\n</div>\r\n<!-- create new entity to add to this list -->\r\n<button mat-icon-button *ngIf=\"enableCreate && entityType !== '' && (allowMultiValue || chosenEntities.length < 1)\" type=\"button\"\r\n  (click)=\"openNewEntityDialog()\" [disabled]=\"disabled\">\r\n  <i class=\"eav-icon-plus\"></i>\r\n</button>\r\n\r\n<!-- TODO: Do we need this ??? -->\r\n<!-- handle free text mode -->\r\n<!-- <input ng-show=\"modes.freeTextMode\" class=\"form-control input-material material\" ng-model=\"value.Value\" formly-skip-ng-model-attrs-manipulator\r\n  type=\"text\">\r\n<a ng-show=\"to.settings.merged.EnableTextEntry && (to.settings.merged.AllowMultiValue || chosenEntities.length < 1)\" class=\"freetext-toggle input-group-addon icon-field-button icon-field-button-small\"\r\n  ng-class=\"{'active': modes.freeTextMode}\" ng-click=\"modes.freeTextMode = !modes.freeTextMode\">\r\n\r\n  <span ng-show=\"modes.freeTextMode\" class=\"eav-icon-down-dir\"></span>\r\n  <span ng-show=\"!modes.freeTextMode\" class=\"eav-icon-i-cursor\"></span>\r\n</a> -->"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.ts ***!
  \*****************************************************************************************************/
/*! exports provided: EntityDefaultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EntityDefaultComponent", function() { return EntityDefaultComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
/* harmony import */ var _shared_services_entity_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../../../shared/services/entity.service */ "./src/app/shared/services/entity.service.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
/* harmony import */ var _eav_item_dialog_multi_item_edit_form_multi_item_edit_form_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../../../../eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component */ "./src/app/eav-item-dialog/multi-item-edit-form/multi-item-edit-form.component.ts");
/* harmony import */ var _shared_services_eav_admin_ui_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../../../../shared/services/eav-admin-ui.service */ "./src/app/shared/services/eav-admin-ui.service.ts");
/* harmony import */ var _shared_services_field_mask_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../../../../shared/services/field-mask.service */ "./src/app/shared/services/field-mask.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};











var EntityDefaultComponent = /** @class */ (function () {
    function EntityDefaultComponent(entityService, eavService, eavAdminUiService, validationMessagesService, dialog) {
        var _this = this;
        this.entityService = entityService;
        this.eavService = eavService;
        this.eavAdminUiService = eavAdminUiService;
        this.validationMessagesService = validationMessagesService;
        this.dialog = dialog;
        // options: Item[];
        this.selectEntities = null;
        this.availableEntities = [];
        this.entityTextDefault = 'Item not found'; // $translate.instant("FieldType.Entity.EntityNotFound");
        this.subscriptions = [];
        this.getEntityText = function (value) {
            if (value === null) {
                return 'empty slot';
            }
            var entities = _this.availableEntities.filter(function (f) { return f.Value === value; });
            if (entities.length > 0) {
                return entities.length > 0 ? entities[0].Text :
                    _this.entityTextDefault ? _this.entityTextDefault : value;
            }
            return value;
        };
        this.getEntityId = function (value) {
            if (value === null) {
                return 'empty slot';
            }
            var entities = _this.availableEntities.filter(function (f) { return f.Value === value; });
            if (entities.length > 0) {
                return entities.length > 0 ? entities[0].Id : value;
            }
            return value;
        };
        /**
         * Determine is entityID in chosenEntities
         */
        this.isInChosenEntities = function (value) {
            if (_this.chosenEntities.find(function (e) { return e === value; })) {
                return true;
            }
            return false;
        };
        this.eavConfig = this.eavService.getEavConfiguration();
    }
    Object.defineProperty(EntityDefaultComponent.prototype, "allowMultiValue", {
        get: function () {
            return this.config.settings.AllowMultiValue || false;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "entityType", {
        get: function () {
            return this.config.settings.EntityType || '';
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "enableAddExisting", {
        get: function () {
            return this.config.settings.EnableAddExisting || false;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "enableCreate", {
        get: function () {
            return this.config.settings.EnableCreate || false;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "enableEdit", {
        get: function () {
            return this.config.settings.EnableEdit || false;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "enableRemove", {
        get: function () {
            return this.config.settings.EnableRemove || false;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "enableDelete", {
        get: function () {
            return this.config.settings.EnableDelete || false;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "disabled", {
        get: function () {
            return this.group.controls[this.config.name].disabled;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EntityDefaultComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    EntityDefaultComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config, true);
    };
    EntityDefaultComponent.prototype.ngOnInit = function () {
        // Initialize entities
        var sourceMask = this.entityType || null;
        // this will contain the auto-resolve type (based on other contentType-field)
        this.contentType = new _shared_services_field_mask_service__WEBPACK_IMPORTED_MODULE_10__["FieldMaskService"](sourceMask, this.maybeReload, null, null);
        // don't get it, it must be blank to start with, so it will be loaded at least 1x lastContentType = contentType.resolve();
        console.log('contentType', this.contentType);
        console.log('[create]  ngOnInit EntityDefaultComponent', this.group.value);
        this.setChosenEntities();
        this.setAvailableEntities();
    };
    EntityDefaultComponent.prototype.ngAfterViewInit = function () {
        this.setSelectEntitiesObservables();
    };
    EntityDefaultComponent.prototype.ngOnDestroy = function () {
        this.subscriptions.forEach(function (subscriber) { return subscriber.unsubscribe(); });
    };
    EntityDefaultComponent.prototype.maybeReload = function () {
        console.log('call maybeReload');
    };
    EntityDefaultComponent.prototype.optionSelected = function (event) {
        this.addEntity(event.option.value);
        this.input.nativeElement.value = null;
    };
    /**
     * add entity to form
     * @param value
     */
    EntityDefaultComponent.prototype.addEntity = function (value) {
        if (value) {
            // this.selectedValue = null;
            var entityValues = this.group.controls[this.config.name].value.slice();
            entityValues.push(value);
            this.group.controls[this.config.name].patchValue(entityValues);
        }
    };
    /**
     *  open edit eav item dialog for item
     * @param value
     */
    EntityDefaultComponent.prototype.edit = function (value) {
        var dialogRef = this.eavAdminUiService.openItemEditWithEntityId(this.dialog, this.getEntityId(value), _eav_item_dialog_multi_item_edit_form_multi_item_edit_form_component__WEBPACK_IMPORTED_MODULE_8__["MultiItemEditFormComponent"]);
    };
    /**
     * remove entity value from form
     * @param value
     */
    EntityDefaultComponent.prototype.removeSlot = function (item, index) {
        var entityValues = this.group.controls[this.config.name].value.slice();
        entityValues.splice(index, 1);
        // this.group.patchValue({ [this.config.name]: entityValues.filter(entity => entity !== value) });
        this.group.controls[this.config.name].patchValue(entityValues);
    };
    /**
     * delete entity
     * @param value
     */
    EntityDefaultComponent.prototype.deleteItemInSlot = function (item, index) {
        var _this = this;
        if (this.entityType === '') {
            alert('delete not possible - no type specified in entity field configuration');
            return;
        }
        var entities = this.availableEntities.filter(function (f) { return f.Value === item; });
        var id = entities[0].Id;
        var text = entities[0].Text;
        // TODO:contentType.resolve()
        var contentTypeTemp = this.entityType; // contentType.resolve()
        // Then delete entity item:
        this.entityService.delete(this.eavConfig.appId, contentTypeTemp, id, false).subscribe(function (result) {
            if (result === null || result.status >= 200 && result.status < 300) {
                // TODO: make message
                _this.removeSlot(item, index);
            }
            else {
                // TODO: message success
                _this.entityService.delete(_this.eavConfig.appId, contentTypeTemp, id, true).subscribe(function (items) {
                    _this.removeSlot(item, index);
                    // TODO: refresh avalable entities
                });
            }
        });
    };
    EntityDefaultComponent.prototype.levelUp = function (value, index) {
        var entityValues = this.group.controls[this.config.name].value.slice();
        entityValues.splice(index, 1);
        entityValues.splice.apply(entityValues, [index - 1, 0].concat([value]));
        this.group.controls[this.config.name].patchValue(entityValues);
    };
    EntityDefaultComponent.prototype.levelDown = function (value, index) {
        var entityValues = this.group.controls[this.config.name].value.slice();
        entityValues.splice(index, 1);
        entityValues.splice.apply(entityValues, [index + 1, 0].concat([value]));
        this.group.controls[this.config.name].patchValue(entityValues);
    };
    EntityDefaultComponent.prototype.openNewEntityDialog = function () {
        console.log('TODO openNewEntityDialog');
        // open the dialog for a new item
        // TODO: finisih this when web services are completed
        // eavAdminDialogs.openItemNew(contentType.resolve(), reloadAfterAdd);
    };
    /**
     * set initial value and subscribe to form value changes
     */
    EntityDefaultComponent.prototype.setChosenEntities = function () {
        var _this = this;
        this.chosenEntities = this.group.controls[this.config.name].value || [];
        this.subscriptions.push(this.group.controls[this.config.name].valueChanges.subscribe(function (item) {
            _this.updateChosenEntities(item);
        }));
        this.subscriptions.push(this.eavService.formSetValueChange$.subscribe(function (item) {
            _this.updateChosenEntities(_this.group.controls[_this.config.name].value);
        }));
    };
    EntityDefaultComponent.prototype.updateChosenEntities = function (values) {
        if (this.chosenEntities !== values) {
            this.chosenEntities = values;
        }
    };
    /**
     * TODO: select all entities from app
     */
    EntityDefaultComponent.prototype.setAvailableEntities = function () {
        var _this = this;
        // TODO:
        // const ctName = this.contentType.resolve(); // always get the latest definition, possibly from another drop-down
        // TEMP: harcoded
        var ctName = this.entityType;
        // check if we should get all or only the selected ones...
        // if we can't add, then we only need one...
        var itemFilter = null;
        try {
            itemFilter = this.enableAddExisting
                ? null
                : this.group.controls[this.config.name].value;
        }
        catch (err) { }
        this.entityService.getAvailableEntities(this.eavConfig.appId, itemFilter, ctName).subscribe(function (items) {
            _this.availableEntities = items.slice();
        });
    };
    /**
     * selectEntities observe events from input autocomplete field
     */
    EntityDefaultComponent.prototype.setSelectEntitiesObservables = function () {
        var _this = this;
        if (this.input && this.selectEntities === null) {
            var eventNames = ['keyup', 'click'];
            // Merge all observables into one single stream:
            var eventStreams = eventNames.map(function (eventName) {
                // return Observable.fromEvent(this.input.nativeElement, eventName);
                return Object(rxjs__WEBPACK_IMPORTED_MODULE_1__["fromEvent"])(_this.input.nativeElement, eventName);
            });
            var allEvents$ = rxjs__WEBPACK_IMPORTED_MODULE_1__["merge"].apply(void 0, eventStreams);
            this.selectEntities = allEvents$
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["map"])(function (value) { return _this.filter(value.target.value); }));
            // .do(value => console.log('test selectEntities', value));
        }
        // clear this.selectEntities if input don't exist
        // this can happen when not allowMultiValue
        if (!this.input) {
            this.selectEntities = null;
        }
    };
    EntityDefaultComponent.prototype.filter = function (val) {
        if (val === '') {
            return this.availableEntities;
        }
        return this.availableEntities.filter(function (option) {
            return option.Text ?
                option.Text.toLowerCase().indexOf(val.toLowerCase()) === 0
                : option.Value.toLowerCase().indexOf(val.toLowerCase()) === 0;
        });
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('autocompleteInput'),
        __metadata("design:type", Object)
    ], EntityDefaultComponent.prototype, "input", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], EntityDefaultComponent.prototype, "config", void 0);
    EntityDefaultComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'entity-default',
            template: __webpack_require__(/*! ./entity-default.component.html */ "./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.html"),
            styles: [__webpack_require__(/*! ./entity-default.component.css */ "./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.css")],
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_4__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_shared_services_entity_service__WEBPACK_IMPORTED_MODULE_6__["EntityService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_5__["EavService"],
            _shared_services_eav_admin_ui_service__WEBPACK_IMPORTED_MODULE_9__["EavAdminUiService"],
            _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_7__["ValidationMessagesService"],
            _angular_material__WEBPACK_IMPORTED_MODULE_3__["MatDialog"]])
    ], EntityDefaultComponent);
    return EntityDefaultComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.css":
/*!***************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.css ***!
  \***************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".thumbnail-before-input.icon-before-input {\r\n    width: 46px;\r\n    font-size: 42px;\r\n    padding-left: 2px;\r\n    padding-right: 5px;\r\n    padding-top: 2px;\r\n    padding-bottom: 3px;\r\n}\r\n.icon-before-input a {\r\n    color: #444;\r\n}\r\n.thumbnail-before-input {\r\n    padding-top: 0; \r\n    padding-bottom: 0; \r\n    border-top-width: 0; \r\n    padding-left: 0; \r\n    padding-right: 0; \r\n    border-left-width: 0; \r\n    border-bottom-width: 0; \r\n    background-color: transparent;\r\n    /* transparent;  */\r\n\r\n    /* new */\r\n    width: 64px;\r\n    height: 64px;\r\n    /* position: absolute;\r\n    top: 0; */\r\n     /* important to add this, because of paddings which are auto-added */\r\n    border-radius: 0;\r\n}\r\n/* design/style the empty placeholder - for now don't do anything '*/\r\n/* .thumbnail-before-input.empty-placeholder { */\r\n/*background-color: pink;*/\r\n/* } */\r\ndiv.dropzone div.input-group div.tooltip-inner {\r\n    word-wrap: break-word;\r\n}\r\n.field-hints {\r\n    visibility: hidden;\r\n    opacity: 0;\r\n    transition-duration: 200ms, 200ms;\r\n    transition-property: opacity, visibility;\r\n    transition-delay: 0, 200ms;\r\n}\r\ndiv.focused .field-hints {\r\n    visibility: visible;\r\n    opacity:1;\r\n}"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.html":
/*!****************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.html ***!
  \****************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- <div (mouseleave)=\"showFieldHints = false\"> -->\r\n<div>\r\n  <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n    <div matPrefix *ngIf=\"value && isImage()\" class=\"thumbnail-before-input\" (mouseenter)=\"showPreview = true\" (mouseleave)=\"showPreview = false\"\r\n      [ngStyle]=\"{'background-image': 'url(' + thumbnailUrl(1, true) + ')'}\">\r\n    </div>\r\n    <div matPrefix *ngIf=\"value && !isImage()\" class=\"thumbnail-before-input icon-before-input\">\r\n      <a [href]=\"link\" target=\"_blank\" tabindex=\"-1\" [class]=\"icon()\" matTooltip=\"{{tooltipUrl(link)}}\" matTooltipPosition=\"right\">\r\n      </a>\r\n    </div>\r\n    <div matPrefix *ngIf=\"!value\" class=\"thumbnail-before-input empty-placeholder\">\r\n    </div>\r\n    <input matInput type=\"text\" [formControlName]=\"config.name\" [placeholder]=\"config.label\" [required]=\"config.required\" (click)=\"showFieldHints = true\"\r\n      [required]=\"config.required\" matTooltip=\"{{'Edit.Fields.Hyperlink.Default.Tooltip1' | translate }}\r\n      {{'Edit.Fields.Hyperlink.Default.Tooltip2' | translate }}\r\n      ADAM - sponsored with ♥ by 2sic.com\" matTooltipPosition=\"above\">\r\n    <!-- <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() }}</mat-error> -->\r\n    <span matSuffix class=\"input-group-btn\">\r\n      <div style=\"width: 6px;\"></div>\r\n      <button mat-icon-button *ngIf=\"buttons.indexOf('adam') > -1\" type=\"button\" [disabled]=\"disabled\" class=\"btn btn-default icon-field-button\"\r\n        (click)=\"toggleAdam()\" matTooltip=\"{{ 'Edit.Fields.Hyperlink.Default.AdamUploadLabel' | translate }}\">\r\n        <i class=\"eav-icon-apple\"></i>\r\n      </button>\r\n      <button mat-icon-button *ngIf=\"buttons.indexOf('page') > -1\" type=\"button\" [disabled]=\"disabled\" class=\"btn btn-default icon-field-button\"\r\n        (click)=\"openPageDialog()\" matTooltip=\"{{ 'Edit.Fields.Hyperlink.Default.PageLabel' | translate }}\">\r\n        <i class=\"eav-icon-sitemap\"></i>\r\n      </button>\r\n      <button mat-icon-button *ngIf=\"buttons.indexOf('more') > -1\" tabindex=\"-1\" [disabled]=\"disabled\" type=\"button\" class=\"btn btn-default icon-field-button\"\r\n        [matMenuTriggerFor]=\"menu\">\r\n        <i class=\"eav-icon-options\"></i>\r\n      </button>\r\n      <mat-menu #menu=\"matMenu\">\r\n        <button mat-menu-item *ngIf=\"showAdam\" (click)=\"toggleAdam(false)\">\r\n          <i class=\"eav-icon-apple\"></i>\r\n          <span>{{'Edit.Fields.Hyperlink.Default.MenuAdam' | translate }}</span>\r\n        </button>\r\n        <button mat-menu-item *ngIf=\"config.settings.ShowPagePicker\" (click)=\"openPageDialog()\">\r\n          <i class=\"eav-icon-sitemap\" xicon=\"home\"></i>\r\n          <span>{{'Edit.Fields.Hyperlink.Default.MenuPage' | translate }}</span>\r\n        </button>\r\n        <button mat-menu-item *ngIf=\"config.settings.ShowImageManager\" (click)=\"toggleAdam(true, true)\">\r\n          <i class=\"eav-icon-file-image\" xicon=\"picture\"></i>\r\n          <span>{{'Edit.Fields.Hyperlink.Default.MenuImage' | translate }}</span>\r\n        </button>\r\n        <button mat-menu-item *ngIf=\"config.settings.ShowFileManager\" (click)=\"toggleAdam(true, false)\">\r\n          <i class=\"eav-icon-file\" xicon=\"file\"></i>\r\n          <span>{{'Edit.Fields.Hyperlink.Default.MenuDocs' | translate }}</span>\r\n        </button>\r\n      </mat-menu>\r\n    </span>\r\n    <div *ngIf=\"showPreview\" style=\"position: relative\">\r\n      <div style=\"position: absolute; z-index: 100; background: white; top: 10px; text-align: center; left: 0; right: 0;\">\r\n        <img [src]=\"thumbnailUrl(2)\" />\r\n      </div>\r\n    </div>\r\n  </mat-form-field>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.ts":
/*!**************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.ts ***!
  \**************************************************************************************************************/
/*! exports provided: HyperlinkDefaultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HyperlinkDefaultComponent", function() { return HyperlinkDefaultComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _shared_services_file_type_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../shared/services/file-type.service */ "./src/app/shared/services/file-type.service.ts");
/* harmony import */ var _shared_services_dnn_bridge_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../../shared/services/dnn-bridge.service */ "./src/app/shared/services/dnn-bridge.service.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
/* harmony import */ var _shared_models_adam_adam_config__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../shared/models/adam/adam-config */ "./src/app/shared/models/adam/adam-config.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var HyperlinkDefaultComponent = /** @class */ (function () {
    function HyperlinkDefaultComponent(fileTypeService, dnnBridgeService, eavService) {
        var _this = this;
        this.fileTypeService = fileTypeService;
        this.dnnBridgeService = dnnBridgeService;
        this.eavService = eavService;
        this.toggleAdamValue = false;
        this.link = '';
        // adam: any;
        this.subscriptions = [];
        this.adamModeConfig = {
            usePortalRoot: false
        };
        this.isImage = function () { return _this.fileTypeService.isImage(_this.link); };
        this.icon = function () { return _this.fileTypeService.getIconClass(_this.link); };
        this.tooltipUrl = function (str) {
            if (!str) {
                return '';
            }
            return str.replace(/\//g, '/&#8203;');
        };
        this.eavConfig = this.eavService.getEavConfiguration();
    }
    Object.defineProperty(HyperlinkDefaultComponent.prototype, "value", {
        get: function () {
            return this.group.controls[this.config.name].value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(HyperlinkDefaultComponent.prototype, "disabled", {
        get: function () {
            return this.group.controls[this.config.name].disabled;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(HyperlinkDefaultComponent.prototype, "showAdam", {
        // ensureDefaultConfig();
        get: function () {
            // this.config.settings.ShowAdam.values.Where(v => v.Dimensions.Contains("en-en").value) or values[0]
            // then the wrapper will enable/disable the field, depending on the dimension state\
            // so if it's read-only sharing, the input-field is disabled till the globe is clicked to enable edit...
            return this.config.settings.ShowAdam ? this.config.settings.ShowAdam : true;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(HyperlinkDefaultComponent.prototype, "fileFilter", {
        get: function () {
            return this.config.settings.FileFilter || '';
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(HyperlinkDefaultComponent.prototype, "buttons", {
        get: function () {
            return this.config.settings.Buttons ? this.config.settings.Buttons : 'adam,more';
        },
        enumerable: true,
        configurable: true
    });
    HyperlinkDefaultComponent.prototype.ngOnInit = function () {
        this.attachAdam();
        this.setLink(this.value);
        this.suscribeValueChanges();
    };
    HyperlinkDefaultComponent.prototype.ngOnDestroy = function () {
        this.subscriptions.forEach(function (subscriber) { return subscriber.unsubscribe(); });
    };
    HyperlinkDefaultComponent.prototype.setFormValue = function (formControlName, value) {
        var _a;
        this.group.patchValue((_a = {}, _a[formControlName] = value, _a));
    };
    HyperlinkDefaultComponent.prototype.thumbnailUrl = function (size, quote) {
        var result = this.link;
        if (size === 1) {
            result = result + '?w=64&h=64&mode=crop';
        }
        if (size === 2) {
            result = result + '?w=500&h=400&mode=max';
        }
        var qt = quote ? '"' : '';
        return qt + result + qt;
    };
    //#region dnn-page picker dialog
    // the callback when something was selected
    HyperlinkDefaultComponent.prototype.processResultOfPagePicker = function (value) {
        // Convert to page:xyz format (if it wasn't cancelled)
        if (value) {
            this.setFormValue(this.config.name, "page:" + value.id);
        }
    };
    // open the dialog
    HyperlinkDefaultComponent.prototype.openPageDialog = function () {
        console.log('openPageDialog');
        // dnnBridgeSvc.open(
        //   this.value,
        //   {
        //     Paths: this.config.settings.Paths ? this.config.settings.Paths.values[0].value : '',
        //     FileFilter: this.config.settings.FileFilter ? this.config.settings.FileFilter : ''
        //   },
        //   this.processResultOfPagePicker);
    };
    //#endregion dnn page picker
    //#region new adam: callbacks only
    HyperlinkDefaultComponent.prototype.setValue = function (fileItem) {
        console.log('setValue fileItem :', fileItem);
        this.setFormValue(this.config.name, "File:" + fileItem.Id);
    };
    HyperlinkDefaultComponent.prototype.toggleAdam = function (usePortalRoot, showImagesOnly) {
        console.log('toggleAdam hdefault');
        this.config.adam.toggle({
            showImagesOnly: showImagesOnly,
            usePortalRoot: usePortalRoot
        });
    };
    /**
   * subscribe to form value changes. Only this field change
   *
   */
    HyperlinkDefaultComponent.prototype.suscribeValueChanges = function () {
        var _this = this;
        this.subscriptions.push(this.group.controls[this.config.name].valueChanges.subscribe(function (item) {
            console.log('suscribeValueChanges CHANGE');
            _this.setLink(item);
        }));
    };
    /**
     * Update test-link if necessary - both when typing or if link was set by dialogs
     * @param value
     */
    HyperlinkDefaultComponent.prototype.setLink = function (value) {
        var _this = this;
        // const oldValue = this.value;
        if (!value) {
            return null;
        }
        // handle short-ID links like file:17
        var urlFromId$ = this.dnnBridgeService.getUrlOfId(this.eavConfig.appId, value, this.config.header.contentTypeName, this.config.header.guid, this.config.name);
        if (urlFromId$) {
            this.subscriptions.push(urlFromId$.subscribe(function (data) {
                if (data) {
                    _this.link = data;
                }
            }));
        }
        else {
            this.link = value;
        }
    };
    HyperlinkDefaultComponent.prototype.attachAdam = function () {
        var _this = this;
        if (this.config.adam) {
            // callbacks - functions called from adam
            this.config.adam.updateCallback = function (value) { return _this.setValue(value); };
            // binding for dropzone
            this.config.adam.afterUploadCallback = function (value) { return _this.setValue(value); };
            // return value from form
            this.config.adam.getValueCallback = function () { return _this.group.controls[_this.config.name].value; };
            // set adam configuration (initial config)
            // this.config.adam.setConfig(
            //   new AdamConfig(this.adamModeConfig,
            //     true, // allowAssetsRoot
            //     false, // autoLoad
            //     true, // enableSelect
            //     this.fileFilter, // fileFilter
            //     0, // folderDepth
            //     '', // metadataContentTypes
            //     '', // subFolder
            //   )
            // );
            console.log('HyperDefault setConfig : ', Object.assign(new _shared_models_adam_adam_config__WEBPACK_IMPORTED_MODULE_5__["AdamConfig"](), {
                adamModeConfig: this.adamModeConfig,
                fileFilter: this.fileFilter
            }));
            this.config.adam.setConfig(Object.assign(new _shared_models_adam_adam_config__WEBPACK_IMPORTED_MODULE_5__["AdamConfig"](), {
                adamModeConfig: this.adamModeConfig,
                fileFilter: this.fileFilter
            }));
            //   new AdamConfig(this.adamModeConfig,
            //     true, // allowAssetsInRoot
            //     false, // autoLoad
            //     true, // enableSelect
            //     this.fileFilter, // fileFilter
            //     0, // folderDepth
            //     '', // metadataContentTypes
            //     '', // subFolder
        }
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], HyperlinkDefaultComponent.prototype, "config", void 0);
    HyperlinkDefaultComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'hyperlink-default',
            template: __webpack_require__(/*! ./hyperlink-default.component.html */ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.html"),
            styles: [__webpack_require__(/*! ./hyperlink-default.component.css */ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__["InputType"])({
            wrapper: ['app-dropzone', 'app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_shared_services_file_type_service__WEBPACK_IMPORTED_MODULE_2__["FileTypeService"],
            _shared_services_dnn_bridge_service__WEBPACK_IMPORTED_MODULE_3__["DnnBridgeService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_4__["EavService"]])
    ], HyperlinkDefaultComponent);
    return HyperlinkDefaultComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.css":
/*!***************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.css ***!
  \***************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.html":
/*!****************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.html ***!
  \****************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.ts":
/*!**************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.ts ***!
  \**************************************************************************************************************/
/*! exports provided: HyperlinkLibraryComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HyperlinkLibraryComponent", function() { return HyperlinkLibraryComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _shared_models_adam_adam_config__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../shared/models/adam/adam-config */ "./src/app/shared/models/adam/adam-config.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var HyperlinkLibraryComponent = /** @class */ (function () {
    function HyperlinkLibraryComponent() {
        this.adamModeConfig = {
            usePortalRoot: false
        };
    }
    Object.defineProperty(HyperlinkLibraryComponent.prototype, "folderDepth", {
        get: function () {
            console.log('this.config.settings.FolderDepth', this.config.settings.FolderDepth);
            return this.config.settings.FolderDepth || '';
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(HyperlinkLibraryComponent.prototype, "metadataContentTypes", {
        get: function () {
            return this.config.settings.MetadataContentTypes || '';
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(HyperlinkLibraryComponent.prototype, "allowAssetsInRoot", {
        get: function () {
            return this.config.settings.AllowAssetsInRoot || true;
        },
        enumerable: true,
        configurable: true
    });
    HyperlinkLibraryComponent.prototype.ngOnInit = function () {
        this.attachAdam();
    };
    HyperlinkLibraryComponent.prototype.attachAdam = function () {
        if (this.config.adam) {
            // callbacks - functions called from adam
            this.config.adam.updateCallback = function (fileItem) { };
            // binding for dropzone
            this.config.adam.afterUploadCallback = function (fileItem) { };
            // return value from form
            // this.config.adam.getValueCallback = () =>
            this.config.adam.afterUploadCallback = function (fileItem) { };
            console.log('HyperLibrary setConfig : ', Object.assign(new _shared_models_adam_adam_config__WEBPACK_IMPORTED_MODULE_2__["AdamConfig"](), {
                adamModeConfig: this.adamModeConfig,
                allowAssetsInRoot: this.allowAssetsInRoot,
                autoLoad: true,
                enableSelect: false,
                folderDepth: this.folderDepth,
                metadataContentTypes: this.metadataContentTypes
            }));
            // set adam configuration (initial config)
            this.config.adam.setConfig(Object.assign(new _shared_models_adam_adam_config__WEBPACK_IMPORTED_MODULE_2__["AdamConfig"](), {
                adamModeConfig: this.adamModeConfig,
                allowAssetsInRoot: this.allowAssetsInRoot,
                autoLoad: true,
                enableSelect: false,
                folderDepth: this.folderDepth,
                metadataContentTypes: this.metadataContentTypes
            }));
            // this.config.adam.setConfig(
            //   new AdamConfig(this.adamModeConfig,
            //     this.allowAssetsInRoot,
            //     true, // autoLoad
            //     false, // enableSelect
            //     '', // fileFilter
            //     this.folderDepth,
            //     this.metadataContentTypes,
            //     '', // subFolder
            //   )
            // );
        }
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], HyperlinkLibraryComponent.prototype, "config", void 0);
    HyperlinkLibraryComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'hyperlink-library',
            template: __webpack_require__(/*! ./hyperlink-library.component.html */ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.html"),
            styles: [__webpack_require__(/*! ./hyperlink-library.component.css */ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-library/hyperlink-library.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__["InputType"])({
            wrapper: ['app-dropzone', 'app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [])
    ], HyperlinkLibraryComponent);
    return HyperlinkLibraryComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/index.ts":
/*!************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/index.ts ***!
  \************************************************************/
/*! exports provided: StringDefaultComponent, StringUrlPathComponent, StringDropdownComponent, StringDropdownQueryComponent, StringFontIconPickerComponent, BooleanDefaultComponent, DatetimeDefaultComponent, EmptyDefaultComponent, NumberDefaultComponent, EntityDefaultComponent, HyperlinkDefaultComponent, ExternalComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _string_string_default_string_default_component__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./string/string-default/string-default.component */ "./src/app/eav-material-controls/input-types/string/string-default/string-default.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "StringDefaultComponent", function() { return _string_string_default_string_default_component__WEBPACK_IMPORTED_MODULE_0__["StringDefaultComponent"]; });

/* harmony import */ var _string_string_url_path_string_url_path_component__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./string/string-url-path/string-url-path.component */ "./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "StringUrlPathComponent", function() { return _string_string_url_path_string_url_path_component__WEBPACK_IMPORTED_MODULE_1__["StringUrlPathComponent"]; });

/* harmony import */ var _string_string_dropdown_string_dropdown_component__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./string/string-dropdown/string-dropdown.component */ "./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "StringDropdownComponent", function() { return _string_string_dropdown_string_dropdown_component__WEBPACK_IMPORTED_MODULE_2__["StringDropdownComponent"]; });

/* harmony import */ var _string_string_dropdown_query_string_dropdown_query_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./string/string-dropdown-query/string-dropdown-query.component */ "./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "StringDropdownQueryComponent", function() { return _string_string_dropdown_query_string_dropdown_query_component__WEBPACK_IMPORTED_MODULE_3__["StringDropdownQueryComponent"]; });

/* harmony import */ var _string_string_font_icon_picker_string_font_icon_picker_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./string/string-font-icon-picker/string-font-icon-picker.component */ "./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "StringFontIconPickerComponent", function() { return _string_string_font_icon_picker_string_font_icon_picker_component__WEBPACK_IMPORTED_MODULE_4__["StringFontIconPickerComponent"]; });

/* harmony import */ var _boolean_boolean_default_boolean_default_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./boolean/boolean-default/boolean-default.component */ "./src/app/eav-material-controls/input-types/boolean/boolean-default/boolean-default.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "BooleanDefaultComponent", function() { return _boolean_boolean_default_boolean_default_component__WEBPACK_IMPORTED_MODULE_5__["BooleanDefaultComponent"]; });

/* harmony import */ var _datetime_datetime_default_datetime_default_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./datetime/datetime-default/datetime-default.component */ "./src/app/eav-material-controls/input-types/datetime/datetime-default/datetime-default.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "DatetimeDefaultComponent", function() { return _datetime_datetime_default_datetime_default_component__WEBPACK_IMPORTED_MODULE_6__["DatetimeDefaultComponent"]; });

/* harmony import */ var _empty_empty_default_empty_default_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./empty/empty-default/empty-default.component */ "./src/app/eav-material-controls/input-types/empty/empty-default/empty-default.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EmptyDefaultComponent", function() { return _empty_empty_default_empty_default_component__WEBPACK_IMPORTED_MODULE_7__["EmptyDefaultComponent"]; });

/* harmony import */ var _number_number_default_number_default_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./number/number-default/number-default.component */ "./src/app/eav-material-controls/input-types/number/number-default/number-default.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "NumberDefaultComponent", function() { return _number_number_default_number_default_component__WEBPACK_IMPORTED_MODULE_8__["NumberDefaultComponent"]; });

/* harmony import */ var _entity_entity_default_entity_default_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./entity/entity-default/entity-default.component */ "./src/app/eav-material-controls/input-types/entity/entity-default/entity-default.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EntityDefaultComponent", function() { return _entity_entity_default_entity_default_component__WEBPACK_IMPORTED_MODULE_9__["EntityDefaultComponent"]; });

/* harmony import */ var _hyperlink_hyperlink_default_hyperlink_default_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./hyperlink/hyperlink-default/hyperlink-default.component */ "./src/app/eav-material-controls/input-types/hyperlink/hyperlink-default/hyperlink-default.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "HyperlinkDefaultComponent", function() { return _hyperlink_hyperlink_default_hyperlink_default_component__WEBPACK_IMPORTED_MODULE_10__["HyperlinkDefaultComponent"]; });

/* harmony import */ var _custom_external_external_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./custom/external/external.component */ "./src/app/eav-material-controls/input-types/custom/external/external.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "ExternalComponent", function() { return _custom_external_external_component__WEBPACK_IMPORTED_MODULE_11__["ExternalComponent"]; });















/***/ }),

/***/ "./src/app/eav-material-controls/input-types/number/number-default/number-default.component.css":
/*!******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/number/number-default/number-default.component.css ***!
  \******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/number/number-default/number-default.component.html":
/*!*******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/number/number-default/number-default.component.html ***!
  \*******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n  <input matInput type=\"number\" class=\"form-control material\" [formControlName]=\"config.name\" [placeholder]=\"config.label\"\r\n    [required]='config.required'>\r\n  <!-- [pattern]='decimal' [max]='max' [min]='min' -->\r\n  <mat-hint align=\"start\" *ngIf=\"config.settings.Notes && config.settings.Notes\">\r\n    {{config.settings.Notes}}\r\n  </mat-hint>\r\n  <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n</mat-form-field>\r\n\r\n<!-- :{ param: config.settings } -->"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/number/number-default/number-default.component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/number/number-default/number-default.component.ts ***!
  \*****************************************************************************************************/
/*! exports provided: NumberDefaultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "NumberDefaultComponent", function() { return NumberDefaultComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var NumberDefaultComponent = /** @class */ (function () {
    function NumberDefaultComponent(validationMessagesService) {
        this.validationMessagesService = validationMessagesService;
    }
    Object.defineProperty(NumberDefaultComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    NumberDefaultComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    NumberDefaultComponent.prototype.ngOnInit = function () {
        // this.decimal = this.config.settings.Decimals ? `^[0-9]+(\.[0-9]{1,${this.config.settings.Decimals}})?$` : null;
    };
    NumberDefaultComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'number-default',
            template: __webpack_require__(/*! ./number-default.component.html */ "./src/app/eav-material-controls/input-types/number/number-default/number-default.component.html"),
            styles: [__webpack_require__(/*! ./number-default.component.css */ "./src/app/eav-material-controls/input-types/number/number-default/number-default.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__["ValidationMessagesService"]])
    ], NumberDefaultComponent);
    return NumberDefaultComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-default/string-default.component.css":
/*!******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-default/string-default.component.css ***!
  \******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-default/string-default.component.html":
/*!*******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-default/string-default.component.html ***!
  \*******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"rowCount > 1; then textareaField else inputField\">\r\n</div>\r\n<ng-template #inputField>\r\n    <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n        <input matInput type=\"text\" [id]=\"id\" class=\"form-control material\" [formControlName]=\"config.name\" [placeholder]=\"config.label\"\r\n            [required]=\"config.required\">\r\n        <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n        <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n    </mat-form-field>\r\n</ng-template>\r\n\r\n<ng-template #textareaField>\r\n    <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n        <textarea matInput [rows]=\"rowCount\" type=\"text\" [id]=\"id\" class=\"form-control material\" [formControlName]=\"config.name\"\r\n            [placeholder]=\"config.label\" [required]=\"config.required\"></textarea>\r\n        <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n        <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n    </mat-form-field>\r\n</ng-template>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-default/string-default.component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-default/string-default.component.ts ***!
  \*****************************************************************************************************/
/*! exports provided: StringDefaultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "StringDefaultComponent", function() { return StringDefaultComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var StringDefaultComponent = /** @class */ (function () {
    function StringDefaultComponent(validationMessagesService) {
        this.validationMessagesService = validationMessagesService;
    }
    Object.defineProperty(StringDefaultComponent.prototype, "rowCount", {
        get: function () {
            return this.config.settings.RowCount ? this.config.settings.RowCount : 1;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StringDefaultComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    StringDefaultComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('errorComponent', { read: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"] }),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"])
    ], StringDefaultComponent.prototype, "errorComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], StringDefaultComponent.prototype, "config", void 0);
    StringDefaultComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'string-default',
            template: __webpack_require__(/*! ./string-default.component.html */ "./src/app/eav-material-controls/input-types/string/string-default/string-default.component.html"),
            styles: [__webpack_require__(/*! ./string-default.component.css */ "./src/app/eav-material-controls/input-types/string/string-default/string-default.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__["ValidationMessagesService"]])
    ], StringDefaultComponent);
    return StringDefaultComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.css":
/*!********************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.css ***!
  \********************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.html":
/*!*********************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.html ***!
  \*********************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- <div *ngIf=\"config.rowCount > 1; then textareaField else inputField\"></div>\r\n\r\n<ng-template #inputField>\r\n  <input matInput [type]=\"type\" [id]=\"id\" class=\"form-control material\" [errorStateMatcher]=\"errorStateMatcher\" [formControl]=\"formControl\">\r\n</ng-template>\r\n\r\n<ng-template #textareaField>\r\n  <textarea matInput [rows]=\"config.rowCount\" [type]=\"type\" [id]=\"id\" class=\"form-control material\" [errorStateMatcher]=\"errorStateMatcher\"\r\n    [formControl]=\"formControl\"></textarea>\r\n</ng-template> -->"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.ts":
/*!*******************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.ts ***!
  \*******************************************************************************************************************/
/*! exports provided: StringDropdownQueryComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "StringDropdownQueryComponent", function() { return StringDropdownQueryComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var StringDropdownQueryComponent = /** @class */ (function () {
    function StringDropdownQueryComponent() {
    }
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_angular_material__WEBPACK_IMPORTED_MODULE_1__["MatInput"]),
        __metadata("design:type", _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatInput"])
    ], StringDropdownQueryComponent.prototype, "matInput", void 0);
    StringDropdownQueryComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'string-dropdown-query',
            template: __webpack_require__(/*! ./string-dropdown-query.component.html */ "./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.html"),
            styles: [__webpack_require__(/*! ./string-dropdown-query.component.css */ "./src/app/eav-material-controls/input-types/string/string-dropdown-query/string-dropdown-query.component.css")]
        })
    ], StringDropdownQueryComponent);
    return StringDropdownQueryComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.css":
/*!********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.css ***!
  \********************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.html":
/*!*********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.html ***!
  \*********************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"freeTextMode; then inputField else selectField\"></div>\r\n\r\n<ng-template #selectField>\r\n  <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n    <mat-select placeholder=\"Favorite animal\" [formControlName]=\"config.name\" [required]='config.required' [placeholder]=\"config.placeholder\">\r\n      <mat-option>--</mat-option>\r\n      <mat-option *ngFor=\"let item of selectOptions\" [value]=\"item.value\">\r\n        {{item.label}}\r\n      </mat-option>\r\n    </mat-select>\r\n    <a matSuffix *ngIf=\"config.settings.EnableTextEntry && config.settings.EnableTextEntry\" [class]=\"'input-group-addon icon-field-button icon-field-button-small' + (freeTextMode ? ' active' : '')\"\r\n      (click)=\"freeTextMode= !freeTextMode \">\r\n      <i *ngIf=\"!freeTextMode\" class=\"eav-icon-i-cursor\"></i>\r\n    </a>\r\n    <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n    <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n  </mat-form-field>\r\n</ng-template>\r\n\r\n<ng-template #inputField>\r\n  <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n    <input matInput *ngIf=\"freeTextMode\" type=\"text\" [id]=\"id\" class=\"form-control input-material material\" [required]='config.required'\r\n      [formControlName]=\"config.name\" [placeholder]=\"config.placeholder\">\r\n    <a matSuffix *ngIf=\"config.settings.EnableTextEntry && config.settings.EnableTextEntry\" [class]=\"'input-group-addon icon-field-button icon-field-button-small' + (freeTextMode ? ' active' : '')\"\r\n      (click)=\"freeTextMode = !freeTextMode \">\r\n      <i *ngIf=\"freeTextMode\" class=\"eav-icon-down-dir\"></i>\r\n    </a>\r\n    <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n    <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n  </mat-form-field>\r\n</ng-template>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.ts":
/*!*******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.ts ***!
  \*******************************************************************************************************/
/*! exports provided: StringDropdownComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "StringDropdownComponent", function() { return StringDropdownComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var StringDropdownComponent = /** @class */ (function () {
    function StringDropdownComponent(validationMessagesService) {
        this.validationMessagesService = validationMessagesService;
        this.freeTextMode = false;
        this.selectOptions = [];
        this._selectOptions = [];
        this._oldOptions = [];
    }
    Object.defineProperty(StringDropdownComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    StringDropdownComponent.prototype.ngOnInit = function () {
        this.selectOptions = this.setOptionsFromDropdownValues();
        console.log('this.config.settings.DropdownValues:', this.config.settings.DropdownValues);
    };
    /**
    * Read settings Dropdown values
    */
    StringDropdownComponent.prototype.setOptionsFromDropdownValues = function () {
        var options = [];
        if (this.config.settings.DropdownValues) {
            var dropdownValues = this.config.settings.DropdownValues;
            options = dropdownValues.replace(/\r/g, '').split('\n');
            options = options.map(function (e) {
                var s = e.split(':');
                var maybeWantedEmptyVal = s[1];
                var key = s.shift(); // take first, shrink the array
                var val = s.join(':');
                return {
                    label: key,
                    value: (val) ? val : key
                };
            });
        }
        return options;
    };
    StringDropdownComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    StringDropdownComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'string-dropdown',
            template: __webpack_require__(/*! ./string-dropdown.component.html */ "./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.html"),
            styles: [__webpack_require__(/*! ./string-dropdown.component.css */ "./src/app/eav-material-controls/input-types/string/string-dropdown/string-dropdown.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_1__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_2__["ValidationMessagesService"]])
    ], StringDropdownComponent);
    return StringDropdownComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.css":
/*!************************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.css ***!
  \************************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.html":
/*!*************************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.html ***!
  \*************************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div>\r\n  <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n    <div matPrefix class=\"thumbnail-before-input icon-preview\">\r\n      <i class=\"glyphicon {{value}}\"></i>\r\n    </div>\r\n    <input matInput type=\"text\" class=\"form-control material\" [formControlName]=\"config.name\" [placeholder]=\"config.label\" [required]=\"config.required\"\r\n      [matAutocomplete]=\"auto\" (click)=\"update()\">\r\n    <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\">{{config.settings.Notes}}</mat-hint>\r\n    <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n  </mat-form-field>\r\n  <mat-autocomplete #auto=\"matAutocomplete\">\r\n    <mat-option *ngFor=\"let icn of filteredIcons | async\" [value]=\"icn.class\">\r\n      <a (click)=\"setIcon(icn.class, config.name)\">\r\n        <i class=\"{{config.settings.PreviewCss}} {{icn.class}}\"></i>\r\n        <span>{{icn.class}}</span>\r\n      </a>\r\n    </mat-option>\r\n  </mat-autocomplete>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.ts":
/*!***********************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.ts ***!
  \***********************************************************************************************************************/
/*! exports provided: StringFontIconPickerComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "StringFontIconPickerComponent", function() { return StringFontIconPickerComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _shared_services_script_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../../shared/services/script.service */ "./src/app/shared/services/script.service.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
/* harmony import */ var _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../shared/services/eav.service */ "./src/app/shared/services/eav.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var StringFontIconPickerComponent = /** @class */ (function () {
    function StringFontIconPickerComponent(scriptLoaderService, validationMessagesService, eavService) {
        var _this = this;
        this.scriptLoaderService = scriptLoaderService;
        this.validationMessagesService = validationMessagesService;
        this.eavService = eavService;
        this.icons = [];
        this.subscriptions = [];
        this.getFilteredIcons = function () {
            return _this.group.controls[_this.config.name].valueChanges
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["startWith"])(''), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(function (icon) { return icon ? _this.filterStates(icon) : _this.icons.slice(); }));
            // .map(state => state ? this.filterStates(state) : this.icons.slice());
        };
        this.eavConfig = this.eavService.getEavConfiguration();
    }
    Object.defineProperty(StringFontIconPickerComponent.prototype, "files", {
        get: function () {
            return this.config.settings.Files ? this.config.settings.Files : '';
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StringFontIconPickerComponent.prototype, "prefix", {
        get: function () {
            return this.config.settings.CssPrefix ? this.config.settings.CssPrefix : '';
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StringFontIconPickerComponent.prototype, "previewCss", {
        get: function () {
            return this.config.settings.PreviewCss ? this.config.settings.PreviewCss : '';
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StringFontIconPickerComponent.prototype, "value", {
        get: function () {
            return this.group.controls[this.config.name].value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StringFontIconPickerComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    StringFontIconPickerComponent.prototype.ngOnInit = function () {
        this.loadAdditionalResources(this.files);
        this.filteredIcons = this.getFilteredIcons();
    };
    StringFontIconPickerComponent.prototype.ngOnDestroy = function () {
        this.subscriptions.forEach(function (subscriber) { return subscriber.unsubscribe(); });
    };
    StringFontIconPickerComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    StringFontIconPickerComponent.prototype.getIconClasses = function (className) {
        var charcount = className.length, foundList = [], duplicateDetector = {};
        if (!className) {
            return foundList;
        }
        for (var ssSet = 0; ssSet < document.styleSheets.length; ssSet++) {
            try {
                var classes = document.styleSheets[ssSet].rules || document.styleSheets[ssSet].cssRules;
                if (classes) {
                    for (var x = 0; x < classes.length; x++) {
                        if (classes[x].selectorText && classes[x].selectorText.substring(0, charcount) === className) {
                            // prevent duplicate-add...
                            var txt = classes[x].selectorText, icnClass = txt.substring(0, txt.indexOf(':')).replace('.', '');
                            if (!duplicateDetector[icnClass]) {
                                foundList.push({ rule: classes[x], 'class': icnClass });
                                duplicateDetector[icnClass] = true;
                            }
                        }
                    }
                }
            }
            catch (error) {
                // try catch imortant because can't find CSSStyleSheet rules error
                console.log('Icon picker CSSStyleSheet error: ', error);
            }
            //   }
            // }
        }
        // this.icons$ = foundList;
        // this.icons.push(...foundList);
        return foundList;
    };
    StringFontIconPickerComponent.prototype.loadAdditionalResources = function (files) {
        var _this = this;
        // const mapped = files.replace('[App:Path]', appRoot)
        // TODO: test this replace
        var mapped = files.replace('[App:Path]', this.eavConfig.portalroot + this.eavConfig.approot)
            .replace(/([\w])\/\/([\w])/g, // match any double // but not if part of https or just "//" at the beginning
        '$1/$2');
        var fileList = mapped ? mapped.split('\n') : [];
        var scriptModelList = [];
        fileList.forEach(function (element, index) {
            var scriptModel = {
                name: element,
                filePath: element,
                loaded: false
            };
            scriptModelList.push(scriptModel);
        });
        this.scriptLoaderService.loadList(scriptModelList, 'css').subscribe(function (s) {
            if (s !== null) {
                _this.icons = _this.getIconClasses(_this.prefix);
            }
        });
    };
    StringFontIconPickerComponent.prototype.setIcon = function (iconClass, formControlName) {
        var _a;
        this.group.patchValue((_a = {}, _a[formControlName] = iconClass, _a));
    };
    /**
    *  with update on click trigger value change to open autocomplete
    */
    StringFontIconPickerComponent.prototype.update = function () {
        this.group.controls[this.config.name].patchValue(this.value);
    };
    StringFontIconPickerComponent.prototype.filterStates = function (value) {
        var filterValue = value.toLowerCase();
        return this.icons.filter(function (icon) { return icon.class.toLowerCase().indexOf(filterValue) === 0; });
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], StringFontIconPickerComponent.prototype, "config", void 0);
    StringFontIconPickerComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'string-font-icon-picker',
            template: __webpack_require__(/*! ./string-font-icon-picker.component.html */ "./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.html"),
            styles: [__webpack_require__(/*! ./string-font-icon-picker.component.css */ "./src/app/eav-material-controls/input-types/string/string-font-icon-picker/string-font-icon-picker.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_2__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_shared_services_script_service__WEBPACK_IMPORTED_MODULE_3__["ScriptLoaderService"],
            _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_4__["ValidationMessagesService"],
            _shared_services_eav_service__WEBPACK_IMPORTED_MODULE_5__["EavService"]])
    ], StringFontIconPickerComponent);
    return StringFontIconPickerComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.css":
/*!********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.css ***!
  \********************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.html":
/*!*********************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.html ***!
  \*********************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div>\r\n    <!-- directive : only-simple-url-chars -->\r\n    <mat-form-field [formGroup]=\"group\" [style.width]=\"'100%'\">\r\n        <!-- (keyup)=\"clean(config.name, false)\" -->\r\n        <input matInput type=\"text\" class=\"form-control material\" [formControlName]=\"config.name\" (blur)=\"clean(config.name, true)\"\r\n            [placeholder]=\"config.label\" [required]='config.required'>\r\n        <mat-hint align=\"start\" *ngIf=\"config.settings.Notes\"> {{config.settings.Notes}}</mat-hint>\r\n        <mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>\r\n    </mat-form-field>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.ts":
/*!*******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.ts ***!
  \*******************************************************************************************************/
/*! exports provided: StringUrlPathComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "StringUrlPathComponent", function() { return StringUrlPathComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _shared_helpers_helper__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../../shared/helpers/helper */ "./src/app/shared/helpers/helper.ts");
/* harmony import */ var _eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../eav-dynamic-form/decorators/input-type.decorator */ "./src/app/eav-dynamic-form/decorators/input-type.decorator.ts");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
/* harmony import */ var _shared_services_field_mask_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../../shared/services/field-mask.service */ "./src/app/shared/services/field-mask.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var StringUrlPathComponent = /** @class */ (function () {
    function StringUrlPathComponent(validationMessagesService) {
        this.validationMessagesService = validationMessagesService;
        this.enableSlashes = true;
        this.lastAutoCopy = '';
        // private sourceMask: string;
        this.subscriptions = [];
        this.preCleane = function (key, value) {
            return value.replace('/', '-').replace('\\', '-'); // this will remove slashes which could look like path-parts
        };
    }
    Object.defineProperty(StringUrlPathComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StringUrlPathComponent.prototype, "autoGenerateMask", {
        get: function () {
            return this.config.settings.AutoGenerateMask || null;
        },
        enumerable: true,
        configurable: true
    });
    StringUrlPathComponent.prototype.ngOnInit = function () {
        var _this = this;
        var sourceMask = this.autoGenerateMask;
        // this will contain the auto-resolve type (based on other contentType-field)
        this.fieldMaskService = new _shared_services_field_mask_service__WEBPACK_IMPORTED_MODULE_4__["FieldMaskService"](sourceMask, null, this.preCleane, this.group.controls);
        // set initial value
        this.sourcesChangedTryToUpdate(this.fieldMaskService);
        // get all mask field and subcribe to changes. On every change sourcesChangedTryToUpdate.
        this.fieldMaskService.fieldList().forEach(function (e, i) {
            if (_this.group.controls[e]) {
                _this.group.controls[e].valueChanges.subscribe(function (item) {
                    _this.sourcesChangedTryToUpdate(_this.fieldMaskService);
                });
            }
        });
        // clean on value change
        this.subscriptions.push(this.group.controls[this.config.name].valueChanges.subscribe(function (item) {
            _this.clean(_this.config.name, false);
        }));
    };
    StringUrlPathComponent.prototype.ngOnDestroy = function () {
        this.subscriptions.forEach(function (subscriber) { return subscriber.unsubscribe(); });
    };
    /**
     * Field-Mask handling
     * @param fieldMaskService
     */
    StringUrlPathComponent.prototype.sourcesChangedTryToUpdate = function (fieldMaskService) {
        var formControlValue = this.group.controls[this.config.name].value;
        // don't do anything if the current field is not empty and doesn't have the last copy of the stripped value
        if (formControlValue && formControlValue !== this.lastAutoCopy) {
            return;
        }
        var orig = fieldMaskService.resolve();
        var cleaned = _shared_helpers_helper__WEBPACK_IMPORTED_MODULE_1__["Helper"].stripNonUrlCharacters(orig, this.enableSlashes, true);
        if (cleaned) {
            this.lastAutoCopy = cleaned;
            this.group.controls[this.config.name].patchValue(cleaned, { emitEvent: false });
        }
    };
    StringUrlPathComponent.prototype.clean = function (formControlName, trimEnd) {
        var formControlValue = this.group.controls[formControlName].value;
        var cleaned = _shared_helpers_helper__WEBPACK_IMPORTED_MODULE_1__["Helper"].stripNonUrlCharacters(formControlValue, this.enableSlashes, trimEnd);
        if (formControlValue !== cleaned) {
            this.group.controls[formControlName].patchValue(cleaned, { emitEvent: false });
        }
    };
    StringUrlPathComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    StringUrlPathComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            // tslint:disable-next-line:component-selector
            selector: 'string-url-path',
            template: __webpack_require__(/*! ./string-url-path.component.html */ "./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.html"),
            styles: [__webpack_require__(/*! ./string-url-path.component.css */ "./src/app/eav-material-controls/input-types/string/string-url-path/string-url-path.component.css")]
        }),
        Object(_eav_dynamic_form_decorators_input_type_decorator__WEBPACK_IMPORTED_MODULE_2__["InputType"])({
            wrapper: ['app-eav-localization-wrapper'],
        }),
        __metadata("design:paramtypes", [_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_3__["ValidationMessagesService"]])
    ], StringUrlPathComponent);
    return StringUrlPathComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.css":
/*!**************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.css ***!
  \**************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.html":
/*!***************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.html ***!
  \***************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<mat-tab-group *ngIf=\"currentLanguage\" (selectedTabChange)=\"selectedTabChanged($event)\" [selectedIndex]=\"languages.indexOf(getLanguage(currentLanguage))\">\r\n  <mat-tab *ngFor=\"let language of languages\" [label]=\"language.name.substring(0, language.name.indexOf('(') > 0 ? language.name.indexOf('(') - 1 : 100 )\"></mat-tab>\r\n</mat-tab-group>"

/***/ }),

/***/ "./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.ts":
/*!*************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.ts ***!
  \*************************************************************************************************************/
/*! exports provided: EavLanguageSwitcherComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavLanguageSwitcherComponent", function() { return EavLanguageSwitcherComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _shared_services_language_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../../shared/services/language.service */ "./src/app/shared/services/language.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var EavLanguageSwitcherComponent = /** @class */ (function () {
    function EavLanguageSwitcherComponent(languageService) {
        var _this = this;
        this.languageService = languageService;
        this.getLanguageByName = function (name) {
            return _this.languages.find(function (d) { return d.name.startsWith(name); });
        };
        this.getLanguage = function (key) {
            return _this.languages.find(function (d) { return d.key === key; });
        };
        // this.currentLanguage$ = languageService.getCurrentLanguage();
    }
    /**
     * on select tab changed update current language in store
     * @param event
     */
    EavLanguageSwitcherComponent.prototype.selectedTabChanged = function (tabChangeEvent) {
        var language = this.getLanguageByName(tabChangeEvent.tab.textLabel);
        this.languageService.updateCurrentLanguage(language.key);
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Array)
    ], EavLanguageSwitcherComponent.prototype, "languages", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", String)
    ], EavLanguageSwitcherComponent.prototype, "currentLanguage", void 0);
    EavLanguageSwitcherComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-eav-language-switcher',
            template: __webpack_require__(/*! ./eav-language-switcher.component.html */ "./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.html"),
            styles: [__webpack_require__(/*! ./eav-language-switcher.component.css */ "./src/app/eav-material-controls/localization/eav-language-switcher/eav-language-switcher.component.css")]
        }),
        __metadata("design:paramtypes", [_shared_services_language_service__WEBPACK_IMPORTED_MODULE_1__["LanguageService"]])
    ], EavLanguageSwitcherComponent);
    return EavLanguageSwitcherComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/validators/custom-validators.ts":
/*!***********************************************************************!*\
  !*** ./src/app/eav-material-controls/validators/custom-validators.ts ***!
  \***********************************************************************/
/*! exports provided: CustomValidators */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CustomValidators", function() { return CustomValidators; });
/* harmony import */ var _shared_helpers_helper__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../../shared/helpers/helper */ "./src/app/shared/helpers/helper.ts");

var CustomValidators = /** @class */ (function () {
    function CustomValidators() {
    }
    /**
     * validate url chars
     *
     */
    CustomValidators.onlySimpleUrlChars = function (allowPath, trimEnd) {
        return function (control) {
            var cleanInputValue = _shared_helpers_helper__WEBPACK_IMPORTED_MODULE_0__["Helper"].stripNonUrlCharacters(control.value, allowPath, trimEnd);
            return (cleanInputValue === control.value) ? null : { 'onlySimpleUrlChars': true };
        };
    };
    // create a static method for your validation
    CustomValidators.validateDecimals = function (decimals) {
        return function (control) {
            // first check if the control has a value
            if (control.value) {
                // match the control value against the regular expression
                var matches = control.value.toString().match("^[0-9]+(.[0-9]{1," + decimals + "})?$");
                // if there are not matches return an object, else return null.
                return !matches ? { decimals: true } : null;
            }
            else {
                return null;
            }
        };
    };
    return CustomValidators;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/validators/validation-helper.ts":
/*!***********************************************************************!*\
  !*** ./src/app/eav-material-controls/validators/validation-helper.ts ***!
  \***********************************************************************/
/*! exports provided: ValidationHelper */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ValidationHelper", function() { return ValidationHelper; });
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _custom_validators__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./custom-validators */ "./src/app/eav-material-controls/validators/custom-validators.ts");


var ValidationHelper = /** @class */ (function () {
    function ValidationHelper() {
    }
    /**
    * TODO: see can i write this in module configuration ???
    * @param inputType
    */
    ValidationHelper.setDefaultValidations = function (settings) {
        var validation = [];
        var required = settings.Required ? settings.Required : false;
        if (required) {
            validation.push(_angular_forms__WEBPACK_IMPORTED_MODULE_0__["Validators"].required);
        }
        // const pattern = settings.ValidationRegex ? settings.ValidationRegex : '';
        // if (pattern) {
        //     validation.push(Validators.pattern(pattern));
        // }
        var pattern = settings.ValidationRegExJavaScript ? settings.ValidationRegExJavaScript : '';
        if (pattern) {
            validation.push(_angular_forms__WEBPACK_IMPORTED_MODULE_0__["Validators"].pattern(pattern));
        }
        // this.decimal = this.config.settings.Decimals ? `^[0-9]+(\.[0-9]{1,${this.config.settings.Decimals}})?$` : null;
        // const patternDecimals = settings.Decimals ? `^[0-9]+(\.[0-9]{1,${settings.Decimals}})?$` : '';
        // if (patternDecimals) {
        //     validation.push(Validators.pattern(patternDecimals));
        // }
        // const patternDecimals = settings.Decimals ? `^[0-9]+(\.[0-9]{1,${settings.Decimals}})?$` : '';
        if (settings.Decimals) {
            validation.push(_custom_validators__WEBPACK_IMPORTED_MODULE_1__["CustomValidators"].validateDecimals(settings.Decimals));
            console.log('settings validation: ', validation);
        }
        // TODO: See do we set this here or in control
        var max = settings.Max ? settings.Max : 0;
        if (max > 0) {
            validation.push(_angular_forms__WEBPACK_IMPORTED_MODULE_0__["Validators"].max(max));
        }
        // TODO: See do we set this here or in control
        var min = settings.Min ? settings.Min : 0;
        if (min > 0) {
            validation.push(_angular_forms__WEBPACK_IMPORTED_MODULE_0__["Validators"].min(min));
        }
        // if (inputType === InputTypesConstants.stringUrlPath) {
        //   validation = [...['onlySimpleUrlChars']];
        // }
        return validation;
    };
    return ValidationHelper;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/validators/validation-messages-service.ts":
/*!*********************************************************************************!*\
  !*** ./src/app/eav-material-controls/validators/validation-messages-service.ts ***!
  \*********************************************************************************/
/*! exports provided: ValidationMessagesService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ValidationMessagesService", function() { return ValidationMessagesService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var ValidationMessagesService = /** @class */ (function () {
    function ValidationMessagesService() {
    }
    // static onlySimpleUrlCharsValidatorMessage(err, field: FormlyFieldConfig) {
    //   return `"${field.formControl.value}" is not a valid URL`;
    // }
    // static requiredMessage(config) {
    //   return `You must enter a value`;
    // }
    // static minlengthValidationMessage(err, field) {
    //   return `Should have atleast ${field.templateOptions.minLength} characters`;
    // }
    // static maxlengthValidationMessage(err, field) {
    //   return `This value should be less than ${field.settings.templateOptions.maxLength} characters`;
    // }
    // static minValidationMessage(err, field) {
    //   return `This value should be more than ${field.templateOptions.min}`;
    // }
    // static maxValidationMessage(err, field) {
    //   return `This value should be less than ${field.templateOptions.max}`;
    // }
    // static patternValidationMessage(err, field) {
    //   return `"${field.formControl.value}" is not a valid`;
    // }
    // return list of error messages
    ValidationMessagesService.prototype.validationMessages = function () {
        // this.translateService.get('ValidationMessage').subscribe(trans => {
        //   this.messages['required'] = trans.Required;
        //   // console.log('VALIDATION MESSAGES:', data)
        // });
        var messages = {
            required: function (config) {
                return config ? 'ValidationMessage.Required' : "ValidationMessage.RequiredShort";
            },
            // minLength: (config: FieldConfig) => {
            //   return `Should have atleast ${config.settings.MinLength} characters`;
            // },
            // maxLength: (config: FieldConfig) => {
            //   return `This value should be less than ${config.settings.MaxLength} characters`;
            // },
            min: function (config) {
                // return config ? `This value should be more than ${config.settings.Min}` : `ValidationMessage.NotValid`;
                return config ? "ValidationMessage.Min" : "ValidationMessage.NotValid";
            },
            max: function (config) {
                return config ? "ValidationMessage.Max" : "ValidationMessage.NotValid";
            },
            pattern: function (config) {
                return config ? "ValidationMessage.Pattern" : "ValidationMessage.NotValid";
            },
            decimals: function (config) {
                return config ? "ValidationMessage.Decimals" : "ValidationMessage.NotValid";
            },
        };
        return messages;
    };
    // Validate form instance
    // check_dirty true will only emit errors if the field is touched
    // check_dirty false will check all fields independent of
    // being touched or not. Use this as the last check before submitting
    ValidationMessagesService.prototype.validateForm = function (formToValidate, checkDirty) {
        var _this = this;
        var form = formToValidate;
        var formErrors = {};
        Object.keys(form.controls).forEach(function (key) {
            // for (const control in form.controls) {
            var control = form.controls[key];
            if (control) {
                // const control = form.get(field);
                var messages_1 = _this.validationMessages();
                if (control && control.invalid) {
                    if (!checkDirty || (control.dirty || control.touched)) {
                        Object.keys(control.errors).forEach(function (keyError) {
                            console.log('error key', keyError);
                            formErrors[key] = formErrors[key] || messages_1[keyError](undefined);
                        });
                    }
                }
            }
        });
        return formErrors;
    };
    /**
     * get validation error for control
     * @param control
     */
    ValidationMessagesService.prototype.getErrorMessage = function (control, config, touched) {
        var formError = '';
        if (control) {
            var messages_2 = this.validationMessages();
            if (control && control.invalid) {
                if ((control.dirty || control.touched) || touched) {
                    Object.keys(control.errors).forEach(function (key) {
                        if (messages_2[key]) {
                            formError = messages_2[key](config);
                        }
                    });
                }
            }
        }
        return formError;
    };
    ValidationMessagesService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [])
    ], ValidationMessagesService);
    return ValidationMessagesService;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.css":
/*!******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.css ***!
  \******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.html":
/*!*******************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.html ***!
  \*******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div>\r\n    <div class=\"form-ci-subtitle unhide-area\" (click)=\"config.collapse = !config.collapse\">\r\n        <span style=\"position: relative\">\r\n            <i class=\"eav-icon-side-marker\"></i>\r\n            <i *ngIf=\"config.collapse\" class=\"eav-icon-minus\"></i>\r\n            <i *ngIf=\"!config.collapse\" class=\"eav-icon-plus\"></i>\r\n        </span>\r\n        {{config.label ? config.label : 'EditEntity.DefaultTitle' | translate }}\r\n    </div>\r\n    <div [hidden]=\"config.collapse\">\r\n        <div [innerHtml]='notes'></div>\r\n\r\n        <ng-container #fieldComponent></ng-container>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.ts ***!
  \*****************************************************************************************************/
/*! exports provided: CollapsibleWrapperComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CollapsibleWrapperComponent", function() { return CollapsibleWrapperComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var CollapsibleWrapperComponent = /** @class */ (function () {
    function CollapsibleWrapperComponent() {
    }
    Object.defineProperty(CollapsibleWrapperComponent.prototype, "notes", {
        get: function () {
            return this.config.settings ? (this.config.settings.Notes || '') : '';
        },
        enumerable: true,
        configurable: true
    });
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('fieldComponent', { read: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"] }),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"])
    ], CollapsibleWrapperComponent.prototype, "fieldComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], CollapsibleWrapperComponent.prototype, "config", void 0);
    CollapsibleWrapperComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-collapsible-wrapper',
            template: __webpack_require__(/*! ./collapsible-wrapper.component.html */ "./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.html"),
            styles: [__webpack_require__(/*! ./collapsible-wrapper.component.css */ "./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.css")]
        })
    ], CollapsibleWrapperComponent);
    return CollapsibleWrapperComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.css":
/*!****************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.css ***!
  \****************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".language-field {\r\n    float: left;\r\n    width: 100%;\r\n    min-height: 40px;\r\n}\r\n\r\n .eav-localization-lock {\r\n    position: absolute;\r\n    top: -5px;\r\n    right: 0;\r\n    z-index: 1;\r\n    width: auto;\r\n }\r\n\r\n .eav-localization-lock-open { \r\n     color: #228b22; \r\n}\r\n\r\n .language-icon {\r\n   /* margin-top: 12px; */\r\n    font-size: 12px;\r\n }\r\n\r\n .language-wrap {\r\n    overflow: hidden;\r\n    position: relative;\r\n}"

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.html":
/*!*****************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.html ***!
  \*****************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"language-wrap\">\r\n  <div class=\"language-field\" (dblclick)=\"toggleTranslate(currentLanguage !== defaultLanguage)\">\r\n    <ng-container #fieldComponent></ng-container>\r\n  </div>\r\n  <button *ngIf=\"(currentLanguage !== defaultLanguage)\" class=\"eav-localization-lock\" [ngClass]=\"{ 'eav-localization-lock-open': !inputDisabled }\"\r\n    mat-icon-button type=\"button\" [matMenuTriggerFor]=\"rootMenu\">\r\n    {{ infoMessageLabel ? (infoMessageLabel | translate:{ languages: infoMessage }) : '' }}\r\n    <mat-icon class=\"mat-18 language-icon\">language</mat-icon>\r\n  </button>\r\n  <mat-menu #rootMenu=\"matMenu\" [overlapTrigger]=\"true\">\r\n    <button mat-menu-item type=\"button\" (click)=\"translateUnlink(config.name)\">{{ 'LangMenu.Unlink'| translate }}</button>\r\n    <button mat-menu-item type=\"button\" (click)=\"linkToDefault(config.name)\">{{ 'LangMenu.LinkDefault' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" [matMenuTriggerFor]=\"copyFromMenu\">{{ 'LangMenu.Copy' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" [matMenuTriggerFor]=\"useFromMenu\">{{ 'LangMenu.Use' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" [matMenuTriggerFor]=\"shareWithMenu\">{{ 'LangMenu.Share' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" [matMenuTriggerFor]=\"allFieldsMenu\">{{ 'LangMenu.AllFields' | translate }}</button>\r\n  </mat-menu>\r\n\r\n  <mat-menu #copyFromMenu=\"matMenu\">\r\n    <button mat-menu-item type=\"button\" *ngFor=\"let language of languages\" (click)=\"onClickCopyFrom(language.key, config.name)\"\r\n      [title]=\"language.name\">{{language.key}}</button>\r\n  </mat-menu>\r\n\r\n  <mat-menu #useFromMenu=\"matMenu\">\r\n    <button mat-menu-item type=\"button\" *ngFor=\"let language of languages\" (click)=\"onClickUseFrom(language.key, config.name)\"\r\n      [disabled]=\"!hasLanguage(language.key)\" [title]=\"language.name\">{{language.key}}</button>\r\n  </mat-menu>\r\n\r\n  <mat-menu #shareWithMenu=\"matMenu\">\r\n    <button mat-menu-item type=\"button\" *ngFor=\"let language of languages\" (click)=\"onClickShareWith(language.key, config.name)\"\r\n      [disabled]=\"!hasLanguage(language.key)\" [title]=\"language.name\">{{language.key}}</button>\r\n  </mat-menu>\r\n\r\n  <mat-menu #allFieldsMenu=\"matMenu\" [overlapTrigger]=\"false\">\r\n    <button mat-menu-item type=\"button\" (click)=\"translateUnlinkAll()\">{{ 'LangMenu.Unlink' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" (click)=\"linkToDefaultAll()\">{{ 'LangMenu.LinkDefault' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" [matMenuTriggerFor]=\"copyFromAllMenu\">{{ 'LangMenu.Copy' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" [matMenuTriggerFor]=\"useFromAllMenu\">{{ 'LangMenu.Use' | translate }}</button>\r\n    <button mat-menu-item type=\"button\" [matMenuTriggerFor]=\"shareWithAllMenu\">{{ 'LangMenu.Share' | translate }}</button>\r\n  </mat-menu>\r\n\r\n  <mat-menu #copyFromAllMenu=\"matMenu\">\r\n    <button mat-menu-item type=\"button\" *ngFor=\"let language of languages\" (click)=\"onClickCopyFromAll(language.key)\" [title]=\"language.name\">{{language.key}}</button>\r\n  </mat-menu>\r\n\r\n  <mat-menu #useFromAllMenu=\"matMenu\">\r\n    <button mat-menu-item type=\"button\" *ngFor=\"let language of languages\" (click)=\"onClickUseFromAll(language.key)\" [title]=\"language.name\">{{language.key}}</button>\r\n  </mat-menu>\r\n\r\n  <mat-menu #shareWithAllMenu=\"matMenu\">\r\n    <button mat-menu-item type=\"button\" *ngFor=\"let language of languages\" (click)=\"onClickShareWithAll(language.key)\" [title]=\"language.name\">{{language.key}}</button>\r\n  </mat-menu>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.ts":
/*!***************************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.ts ***!
  \***************************************************************************************************************/
/*! exports provided: EavLocalizationComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavLocalizationComponent", function() { return EavLocalizationComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material_menu__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/menu */ "./node_modules/@angular/material/esm5/menu.es5.js");
/* harmony import */ var _shared_services_language_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../shared/services/language.service */ "./src/app/shared/services/language.service.ts");
/* harmony import */ var _shared_services_item_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../shared/services/item.service */ "./src/app/shared/services/item.service.ts");
/* harmony import */ var _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../shared/helpers/localization-helper */ "./src/app/shared/helpers/localization-helper.ts");
/* harmony import */ var _validators_validation_helper__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../validators/validation-helper */ "./src/app/eav-material-controls/validators/validation-helper.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var EavLocalizationComponent = /** @class */ (function () {
    function EavLocalizationComponent(languageService, itemService) {
        var _this = this;
        this.languageService = languageService;
        this.itemService = itemService;
        this.currentLanguage = '';
        this.defaultLanguage = '';
        this.infoMessage = '';
        this.infoMessageLabel = '';
        this.subscriptions = [];
        this.hasLanguage = function (languageKey) {
            return _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].isEditableOrReadonlyTranslationExist(_this.attributes[_this.config.name], languageKey, _this.defaultLanguage);
        };
        this.currentLanguage$ = this.languageService.getCurrentLanguage();
        this.defaultLanguage$ = this.languageService.getDefaultLanguage();
    }
    Object.defineProperty(EavLocalizationComponent.prototype, "inputDisabled", {
        get: function () {
            return this.group.controls[this.config.name].disabled;
        },
        enumerable: true,
        configurable: true
    });
    EavLocalizationComponent.prototype.ngOnInit = function () {
        this.attributes$ = this.itemService.selectAttributesByEntityId(this.config.entityId);
        this.subscribeToAttributeValues();
        this.subscribeMenuChange();
        // subscribe to language data
        this.subscribeToCurrentLanguageFromStore();
        this.subscribeToDefaultLanguageFromStore();
        this.loadlanguagesFromStore();
    };
    EavLocalizationComponent.prototype.ngOnDestroy = function () {
        this.subscriptions.forEach(function (subscriber) { return subscriber.unsubscribe(); });
    };
    EavLocalizationComponent.prototype.toggleTranslate = function (isToggleEnabled) {
        if (isToggleEnabled) {
            if (this.group.controls[this.config.name].disabled) {
                this.translateUnlink(this.config.name);
            }
            else {
                this.linkToDefault(this.config.name);
            }
        }
    };
    EavLocalizationComponent.prototype.translateUnlink = function (attributeKey) {
        this.itemService.removeItemAttributeDimension(this.config.entityId, attributeKey, this.currentLanguage);
        var defaultValue = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].getAttributeValueTranslation(this.attributes[attributeKey], this.defaultLanguage, this.defaultLanguage);
        if (defaultValue) {
            this.itemService.addAttributeValue(this.config.entityId, attributeKey, this.attributes[attributeKey], defaultValue.value, this.currentLanguage, false);
        }
        else {
            console.log(this.currentLanguage + ': Cant copy value from ' + this.defaultLanguage + ' because that value does not exist.');
        }
        this.refreshControlConfig(attributeKey);
    };
    EavLocalizationComponent.prototype.linkToDefault = function (attributeKey) {
        this.itemService.removeItemAttributeDimension(this.config.entityId, attributeKey, this.currentLanguage);
        this.refreshControlConfig(attributeKey);
    };
    /**
     * Copy value where language is copyFromLanguageKey to value where language is current language
     * If value of current language don't exist then add new value
     * @param copyFromLanguageKey
     */
    EavLocalizationComponent.prototype.onClickCopyFrom = function (copyFromLanguageKey, attributeKey) {
        var attributeValueTranslation = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].getAttributeValueTranslation(this.attributes[attributeKey], copyFromLanguageKey, this.defaultLanguage);
        if (attributeValueTranslation) {
            var valueAlreadyExist = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].isEditableOrReadonlyTranslationExist(this.attributes[attributeKey], this.currentLanguage, this.defaultLanguage);
            if (valueAlreadyExist) {
                // Copy attribute value where language is languageKey to value where language is current language
                this.itemService.updateItemAttributeValue(this.config.entityId, attributeKey, attributeValueTranslation.value, this.currentLanguage, this.defaultLanguage, false);
            }
            else {
                // Copy attribute value where language is languageKey to new attribute with current language
                this.itemService.addAttributeValue(this.config.entityId, attributeKey, this.attributes[attributeKey], attributeValueTranslation.value, this.currentLanguage, false);
            }
        }
        else {
            console.log(this.currentLanguage + ': Cant copy value from ' + copyFromLanguageKey + ' because that value does not exist.');
        }
        this.refreshControlConfig(attributeKey);
    };
    EavLocalizationComponent.prototype.onClickUseFrom = function (languageKey, attributeKey) {
        this.itemService.removeItemAttributeDimension(this.config.entityId, attributeKey, this.currentLanguage);
        this.itemService.addItemAttributeDimension(this.config.entityId, attributeKey, this.currentLanguage, languageKey, this.defaultLanguage, true);
        // TODO: investigate can only triger current language change to disable controls ???
        // this.languageService.updateCurrentLanguage(this.currentLanguage);
        this.refreshControlConfig(attributeKey);
    };
    EavLocalizationComponent.prototype.onClickShareWith = function (languageKey, attributeKey) {
        this.itemService.removeItemAttributeDimension(this.config.entityId, attributeKey, this.currentLanguage);
        this.itemService.addItemAttributeDimension(this.config.entityId, attributeKey, this.currentLanguage, languageKey, this.defaultLanguage, false);
        this.refreshControlConfig(attributeKey);
    };
    EavLocalizationComponent.prototype.translateUnlinkAll = function () {
        var _this = this;
        Object.keys(this.attributes).forEach(function (attributeKey) {
            _this.translateUnlink(attributeKey);
        });
        this.languageService.triggerLocalizationWrapperMenuChange();
    };
    EavLocalizationComponent.prototype.linkToDefaultAll = function () {
        var _this = this;
        Object.keys(this.attributes).forEach(function (attributeKey) {
            _this.linkToDefault(attributeKey);
        });
        this.languageService.triggerLocalizationWrapperMenuChange();
    };
    EavLocalizationComponent.prototype.onClickCopyFromAll = function (languageKey) {
        var _this = this;
        Object.keys(this.attributes).forEach(function (attributeKey) {
            _this.onClickCopyFrom(languageKey, attributeKey);
        });
        this.languageService.triggerLocalizationWrapperMenuChange();
    };
    EavLocalizationComponent.prototype.onClickUseFromAll = function (languageKey) {
        var _this = this;
        Object.keys(this.attributes).forEach(function (attributeKey) {
            _this.onClickUseFrom(languageKey, attributeKey);
        });
        this.languageService.triggerLocalizationWrapperMenuChange();
    };
    EavLocalizationComponent.prototype.onClickShareWithAll = function (languageKey) {
        var _this = this;
        Object.keys(this.attributes).forEach(function (attributeKey) {
            _this.onClickShareWith(languageKey, attributeKey);
        });
        this.languageService.triggerLocalizationWrapperMenuChange();
    };
    EavLocalizationComponent.prototype.openMenu = function () {
        this.trigger.openMenu();
    };
    EavLocalizationComponent.prototype.closeMenu = function () {
        this.trigger.closeMenu();
    };
    EavLocalizationComponent.prototype.refreshControlConfig = function (attributeKey) {
        this.setControlDisable(this.attributes[attributeKey], attributeKey, this.currentLanguage, this.defaultLanguage);
        this.setAdamDisable();
        this.setInfoMessage(this.attributes[this.config.name], this.currentLanguage, this.defaultLanguage);
    };
    /**
     * Subscribe triggered when changing all in menu (forAllFields)
     */
    EavLocalizationComponent.prototype.subscribeMenuChange = function () {
        var _this = this;
        this.subscriptions.push(this.languageService.localizationWrapperMenuChange$.subscribe(function (s) {
            _this.setInfoMessage(_this.attributes[_this.config.name], _this.currentLanguage, _this.defaultLanguage);
        }));
    };
    /**
    * Subscribe to item attribute values
    */
    EavLocalizationComponent.prototype.subscribeToAttributeValues = function () {
        var _this = this;
        this.subscriptions.push(this.attributes$.subscribe(function (attributes) {
            _this.attributes = attributes;
        }));
    };
    EavLocalizationComponent.prototype.subscribeToCurrentLanguageFromStore = function () {
        var _this = this;
        this.subscriptions.push(this.currentLanguage$.subscribe(function (currentLanguage) {
            _this.currentLanguage = currentLanguage;
            _this.translateAllConfiguration(_this.currentLanguage);
            _this.refreshControlConfig(_this.config.name);
        }));
    };
    EavLocalizationComponent.prototype.subscribeToDefaultLanguageFromStore = function () {
        var _this = this;
        this.subscriptions.push(this.defaultLanguage$.subscribe(function (defaultLanguage) {
            console.log('[create] read default language', defaultLanguage);
            _this.defaultLanguage = defaultLanguage;
            _this.translateAllConfiguration(_this.currentLanguage);
            _this.setControlDisable(_this.attributes[_this.config.name], _this.config.name, _this.currentLanguage, _this.defaultLanguage);
            _this.setAdamDisable();
            _this.setInfoMessage(_this.attributes[_this.config.name], _this.currentLanguage, _this.defaultLanguage);
        }));
    };
    /**
     * Load languages from store and subscribe to languages
     */
    EavLocalizationComponent.prototype.loadlanguagesFromStore = function () {
        var _this = this;
        this.languages$ = this.languageService.selectAllLanguages();
        this.subscriptions.push(this.languages$.subscribe(function (languages) {
            _this.languages = languages;
        }));
    };
    EavLocalizationComponent.prototype.translateAllConfiguration = function (currentLanguage) {
        this.config.settings = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].translateSettings(this.config.fullSettings, this.currentLanguage, this.defaultLanguage);
        this.config.label = this.config.settings.Name ? this.config.settings.Name : null;
        this.config.validation = _validators_validation_helper__WEBPACK_IMPORTED_MODULE_5__["ValidationHelper"].setDefaultValidations(this.config.settings);
        this.config.required = this.config.settings.Required ? this.config.settings.Required : false;
    };
    /**
     * Determine is control disabled or enabled
     * @param attributes
     * @param attributeKey
     * @param currentLanguage
     * @param defaultLanguage
     */
    EavLocalizationComponent.prototype.setControlDisable = function (attributes, attributeKey, currentLanguage, defaultLanguage) {
        if (!this.config.disabled) {
            if (_shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].isEditableTranslationExist(attributes, currentLanguage, defaultLanguage)) {
                this.group.controls[attributeKey].enable({ emitEvent: false });
            }
            else if (_shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].isReadonlyTranslationExist(attributes, currentLanguage)) {
                this.group.controls[attributeKey].disable({ emitEvent: false });
            }
            else {
                this.group.controls[attributeKey].disable({ emitEvent: false });
            }
        }
    };
    /**
     * set info message
     * @param attributes
     * @param currentLanguage
     * @param defaultLanguage
     */
    EavLocalizationComponent.prototype.setInfoMessage = function (attributes, currentLanguage, defaultLanguage) {
        // Determine is control disabled or enabled and info message
        if (_shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].isEditableTranslationExist(attributes, currentLanguage, defaultLanguage)) {
            this.infoMessage = '';
            this.infoMessageLabel = '';
        }
        else if (_shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].isReadonlyTranslationExist(attributes, currentLanguage)) {
            this.infoMessage = _shared_helpers_localization_helper__WEBPACK_IMPORTED_MODULE_4__["LocalizationHelper"].getAttributeValueTranslation(attributes, currentLanguage, defaultLanguage)
                .dimensions.map(function (d) { return d.value.replace('~', ''); })
                .join(', ');
            this.infoMessageLabel = 'LangMenu.In';
        }
        else {
            this.infoMessage = '';
            this.infoMessageLabel = 'LangMenu.UseDefault';
        }
    };
    /**
     * Change adam disable state
     * @param attributeKey
     */
    EavLocalizationComponent.prototype.setAdamDisable = function () {
        // set Adam disabled state
        if (this.config.adam) {
            this.config.adam.disabled = this.group.controls[this.config.name].disabled;
        }
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('fieldComponent', { read: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"] }),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"])
    ], EavLocalizationComponent.prototype, "fieldComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])(_angular_material_menu__WEBPACK_IMPORTED_MODULE_1__["MatMenuTrigger"]),
        __metadata("design:type", _angular_material_menu__WEBPACK_IMPORTED_MODULE_1__["MatMenuTrigger"])
    ], EavLocalizationComponent.prototype, "trigger", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], EavLocalizationComponent.prototype, "config", void 0);
    EavLocalizationComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-eav-localization-wrapper',
            template: __webpack_require__(/*! ./eav-localization-wrapper.component.html */ "./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.html"),
            styles: [__webpack_require__(/*! ./eav-localization-wrapper.component.css */ "./src/app/eav-material-controls/wrappers/eav-localization-wrapper/eav-localization-wrapper.component.css")]
        }),
        __metadata("design:paramtypes", [_shared_services_language_service__WEBPACK_IMPORTED_MODULE_2__["LanguageService"],
            _shared_services_item_service__WEBPACK_IMPORTED_MODULE_3__["ItemService"]])
    ], EavLocalizationComponent);
    return EavLocalizationComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.css":
/*!*************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.css ***!
  \*************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.html":
/*!**************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.html ***!
  \**************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<ng-container #fieldComponent></ng-container>\r\n<mat-hint align=\"start\" *ngIf=\"config.settings.Notes && config.settings.Notes\">\r\n  {{config.settings.Notes}}\r\n</mat-hint>\r\n<mat-error *ngIf=\"inputInvalid\">{{ getErrorMessage() | translate:{ param: config.settings } }}</mat-error>"

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.ts":
/*!************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.ts ***!
  \************************************************************************************************/
/*! exports provided: ErrorWrapperComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ErrorWrapperComponent", function() { return ErrorWrapperComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../validators/validation-messages-service */ "./src/app/eav-material-controls/validators/validation-messages-service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ErrorWrapperComponent = /** @class */ (function () {
    function ErrorWrapperComponent(validationMessagesService) {
        this.validationMessagesService = validationMessagesService;
    }
    Object.defineProperty(ErrorWrapperComponent.prototype, "inputInvalid", {
        get: function () {
            return this.group.controls[this.config.name].invalid;
        },
        enumerable: true,
        configurable: true
    });
    ErrorWrapperComponent.prototype.getErrorMessage = function () {
        return this.validationMessagesService.getErrorMessage(this.group.controls[this.config.name], this.config);
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('fieldComponent', { read: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"] }),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"])
    ], ErrorWrapperComponent.prototype, "fieldComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], ErrorWrapperComponent.prototype, "config", void 0);
    ErrorWrapperComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-error-wrapper',
            template: __webpack_require__(/*! ./error-wrapper.component.html */ "./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.html"),
            styles: [__webpack_require__(/*! ./error-wrapper.component.css */ "./src/app/eav-material-controls/wrappers/field-parent-wrapper/error-wrapper.component.css")]
        }),
        __metadata("design:paramtypes", [_validators_validation_messages_service__WEBPACK_IMPORTED_MODULE_1__["ValidationMessagesService"]])
    ], ErrorWrapperComponent);
    return ErrorWrapperComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.css":
/*!********************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.css ***!
  \********************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.html":
/*!*********************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.html ***!
  \*********************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div [hidden]=\"!visibleInEditUI\">\r\n  <ng-container #fieldComponent></ng-container>\r\n</div>"

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.ts":
/*!*******************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.ts ***!
  \*******************************************************************************************/
/*! exports provided: HiddenWrapperComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HiddenWrapperComponent", function() { return HiddenWrapperComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var HiddenWrapperComponent = /** @class */ (function () {
    function HiddenWrapperComponent() {
    }
    Object.defineProperty(HiddenWrapperComponent.prototype, "visibleInEditUI", {
        get: function () {
            return (this.config.settings.VisibleInEditUI === false) ? false : true;
        },
        enumerable: true,
        configurable: true
    });
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('fieldComponent', { read: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"] }),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"])
    ], HiddenWrapperComponent.prototype, "fieldComponent", void 0);
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", Object)
    ], HiddenWrapperComponent.prototype, "config", void 0);
    HiddenWrapperComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-hidden-wrapper',
            template: __webpack_require__(/*! ./hidden-wrapper.component.html */ "./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.html"),
            styles: [__webpack_require__(/*! ./hidden-wrapper.component.css */ "./src/app/eav-material-controls/wrappers/hidden-wrapper/hidden-wrapper.component.css")]
        })
    ], HiddenWrapperComponent);
    return HiddenWrapperComponent;
}());



/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/index.ts":
/*!*********************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/index.ts ***!
  \*********************************************************/
/*! exports provided: CollapsibleWrapperComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _collapsible_wrapper_collapsible_wrapper_component__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./collapsible-wrapper/collapsible-wrapper.component */ "./src/app/eav-material-controls/wrappers/collapsible-wrapper/collapsible-wrapper.component.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "CollapsibleWrapperComponent", function() { return _collapsible_wrapper_collapsible_wrapper_component__WEBPACK_IMPORTED_MODULE_0__["CollapsibleWrapperComponent"]; });


// export { FormFieldWrapperComponent } from './form-field-wrapper/form-field-wrapper.component';


/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.css":
/*!****************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.css ***!
  \****************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.html":
/*!*****************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.html ***!
  \*****************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- <div class=\"date-picker-wide-suffix-right\"> -->\r\n<ng-container #fieldComponent></ng-container>\r\n\r\n<!-- <div class=\"mat-form-field-suffix mat-form-field-suffix\"> -->\r\n<ng-template #matSuffix>\r\n  <a *ngIf=\"to.settings.EnableTextEntry && to.settings.EnableTextEntry\" [class]=\"'input-group-addon icon-field-button icon-field-button-small' + (to.freeTextMode ? ' active' : '')\"\r\n    (click)=\"to.freeTextMode=! to.freeTextMode \">\r\n    <mat-icon *ngIf=\"to.freeTextMode\" class=\"mat-24\">add</mat-icon>\r\n    <mat-icon *ngIf=\"!to.freeTextMode \" class=\"mat-24\">remove</mat-icon>\r\n  </a>\r\n</ng-template>\r\n<!-- </div>\r\n</div> -->"

/***/ }),

/***/ "./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.ts":
/*!***************************************************************************************************!*\
  !*** ./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.ts ***!
  \***************************************************************************************************/
/*! exports provided: TextEntryWrapperComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TextEntryWrapperComponent", function() { return TextEntryWrapperComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var TextEntryWrapperComponent = /** @class */ (function () {
    function TextEntryWrapperComponent() {
    }
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewChild"])('fieldComponent', { read: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"] }),
        __metadata("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_0__["ViewContainerRef"])
    ], TextEntryWrapperComponent.prototype, "fieldComponent", void 0);
    TextEntryWrapperComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-text-entry-wrapper',
            template: __webpack_require__(/*! ./text-entry-wrapper.component.html */ "./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.html"),
            styles: [__webpack_require__(/*! ./text-entry-wrapper.component.css */ "./src/app/eav-material-controls/wrappers/text-entry-wrapper/text-entry-wrapper.component.css")]
        })
    ], TextEntryWrapperComponent);
    return TextEntryWrapperComponent;
}());



/***/ }),

/***/ "./src/app/shared/constants/index.ts":
/*!*******************************************!*\
  !*** ./src/app/shared/constants/index.ts ***!
  \*******************************************/
/*! exports provided: InputTypesConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _input_types_constants__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./input-types-constants */ "./src/app/shared/constants/input-types-constants.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "InputTypesConstants", function() { return _input_types_constants__WEBPACK_IMPORTED_MODULE_0__["InputTypesConstants"]; });




/***/ }),

/***/ "./src/app/shared/constants/input-types-constants.ts":
/*!***********************************************************!*\
  !*** ./src/app/shared/constants/input-types-constants.ts ***!
  \***********************************************************/
/*! exports provided: InputTypesConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InputTypesConstants", function() { return InputTypesConstants; });
var InputTypesConstants = /** @class */ (function () {
    function InputTypesConstants() {
    }
    // string
    InputTypesConstants.stringDefault = 'string-default';
    InputTypesConstants.stringUrlPath = 'string-url-path';
    InputTypesConstants.stringDropdown = 'string-dropdown';
    InputTypesConstants.stringDropdownQuery = 'string-dropdown-query';
    InputTypesConstants.stringFontIconPicker = 'string-font-icon-picker';
    // boolean
    InputTypesConstants.booleanDefault = 'boolean-default';
    // datetime
    InputTypesConstants.datetimeDefault = 'datetime-default';
    // empty
    InputTypesConstants.emptyDefault = 'empty-default';
    // number
    InputTypesConstants.numberDefault = 'number-default';
    // entity
    InputTypesConstants.entityDefault = 'entity-default';
    // hyperlink/files
    InputTypesConstants.hyperlinkDefault = 'hyperlink-default';
    // hyperlink library
    InputTypesConstants.hyperlinkLibrary = 'hyperlink-library';
    // custom/files
    InputTypesConstants.external = 'external';
    return InputTypesConstants;
}());



/***/ }),

/***/ "./src/app/shared/constants/type-constants.ts":
/*!****************************************************!*\
  !*** ./src/app/shared/constants/type-constants.ts ***!
  \****************************************************/
/*! exports provided: FileTypeConstants, DialogTypeConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FileTypeConstants", function() { return FileTypeConstants; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DialogTypeConstants", function() { return DialogTypeConstants; });
var FileTypeConstants = /** @class */ (function () {
    function FileTypeConstants() {
    }
    // string
    FileTypeConstants.css = 'css';
    FileTypeConstants.javaScript = 'js';
    return FileTypeConstants;
}());

var DialogTypeConstants = /** @class */ (function () {
    function DialogTypeConstants() {
    }
    // string
    DialogTypeConstants.itemEditWithEntityId = 'itemEditWithEntityId';
    DialogTypeConstants.itemEditWithContent = 'itemEditWithContent';
    return DialogTypeConstants;
}());



/***/ }),

/***/ "./src/app/shared/constants/url-constants.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/constants/url-constants.ts ***!
  \***************************************************/
/*! exports provided: UrlConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UrlConstants", function() { return UrlConstants; });
var UrlConstants = /** @class */ (function () {
    function UrlConstants() {
    }
    UrlConstants.apiRoot = '/desktopmodules/2sxc/api/';
    return UrlConstants;
}());



/***/ }),

/***/ "./src/app/shared/directives/click-stop-propagination.directive.ts":
/*!*************************************************************************!*\
  !*** ./src/app/shared/directives/click-stop-propagination.directive.ts ***!
  \*************************************************************************/
/*! exports provided: ClickStopPropagationDirective */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ClickStopPropagationDirective", function() { return ClickStopPropagationDirective; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var ClickStopPropagationDirective = /** @class */ (function () {
    function ClickStopPropagationDirective() {
    }
    ClickStopPropagationDirective.prototype.onClick = function (event) {
        event.stopPropagation();
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["HostListener"])('click', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], ClickStopPropagationDirective.prototype, "onClick", null);
    ClickStopPropagationDirective = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Directive"])({
            selector: '[appClickStopPropagation]'
        })
    ], ClickStopPropagationDirective);
    return ClickStopPropagationDirective;
}());



/***/ }),

/***/ "./src/app/shared/effects/content-type.effects.ts":
/*!********************************************************!*\
  !*** ./src/app/shared/effects/content-type.effects.ts ***!
  \********************************************************/
/*! exports provided: ContentTypeEffects */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeEffects", function() { return ContentTypeEffects; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngrx/effects */ "./node_modules/@ngrx/effects/fesm5/effects.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _services_content_type_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../services/content-type.service */ "./src/app/shared/services/content-type.service.ts");
/* harmony import */ var _store_actions_content_type_actions__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../store/actions/content-type.actions */ "./src/app/shared/store/actions/content-type.actions.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var ContentTypeEffects = /** @class */ (function () {
    function ContentTypeEffects(actions$, contentTypeService) {
        var _this = this;
        this.actions$ = actions$;
        this.contentTypeService = contentTypeService;
        /**
         * Efect on Action (LOAD_EAV_CONTENTTYPES) load ContentType and sent it to store with action LoadContentTypesSuccessAction
         */
        this.loadContentType$ = this.actions$
            .ofType(_store_actions_content_type_actions__WEBPACK_IMPORTED_MODULE_4__["LOAD_CONTENT_TYPE"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["switchMap"])(function (action) {
            return _this.contentTypeService.getContentTypeFromJsonContentType1(action.path)
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["map"])(function (contentType) { return new _store_actions_content_type_actions__WEBPACK_IMPORTED_MODULE_4__["LoadContentTypeSuccessAction"](contentType); }));
        }));
    }
    __decorate([
        Object(_ngrx_effects__WEBPACK_IMPORTED_MODULE_1__["Effect"])(),
        __metadata("design:type", Object)
    ], ContentTypeEffects.prototype, "loadContentType$", void 0);
    ContentTypeEffects = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_ngrx_effects__WEBPACK_IMPORTED_MODULE_1__["Actions"],
            _services_content_type_service__WEBPACK_IMPORTED_MODULE_3__["ContentTypeService"]])
    ], ContentTypeEffects);
    return ContentTypeEffects;
}());



/***/ }),

/***/ "./src/app/shared/effects/eav.effects.ts":
/*!***********************************************!*\
  !*** ./src/app/shared/effects/eav.effects.ts ***!
  \***********************************************/
/*! exports provided: EavEffects */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavEffects", function() { return EavEffects; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngrx/effects */ "./node_modules/@ngrx/effects/fesm5/effects.js");
/* harmony import */ var _services_eav_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../services/eav.service */ "./src/app/shared/services/eav.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



// import * as itemActions from '../store/actions/item.actions';
var EavEffects = /** @class */ (function () {
    function EavEffects(actions$, eavService) {
        this.actions$ = actions$;
        this.eavService = eavService;
    }
    EavEffects = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_ngrx_effects__WEBPACK_IMPORTED_MODULE_1__["Actions"],
            _services_eav_service__WEBPACK_IMPORTED_MODULE_2__["EavService"]])
    ], EavEffects);
    return EavEffects;
}());



/***/ }),

/***/ "./src/app/shared/effects/item.effects.ts":
/*!************************************************!*\
  !*** ./src/app/shared/effects/item.effects.ts ***!
  \************************************************/
/*! exports provided: ItemEffects */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ItemEffects", function() { return ItemEffects; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _ngrx_effects__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngrx/effects */ "./node_modules/@ngrx/effects/fesm5/effects.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _services_item_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../services/item.service */ "./src/app/shared/services/item.service.ts");
/* harmony import */ var _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../store/actions/item.actions */ "./src/app/shared/store/actions/item.actions.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var ItemEffects = /** @class */ (function () {
    function ItemEffects(actions$, itemService) {
        var _this = this;
        this.actions$ = actions$;
        this.itemService = itemService;
        /**
         * Efect on Action (LOAD_EAV_ITEMS) load EavItem and sent it to store with action LoadEavItemsSuccessAction
         */
        this.loadItem$ = this.actions$
            .ofType(_store_actions_item_actions__WEBPACK_IMPORTED_MODULE_4__["LOAD_ITEM"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["switchMap"])(function (action) {
            return _this.itemService.getItemFromJsonItem1(action.path)
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["map"])(function (item) { return new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_4__["LoadItemSuccessAction"](item); }));
        }));
    }
    __decorate([
        Object(_ngrx_effects__WEBPACK_IMPORTED_MODULE_1__["Effect"])(),
        __metadata("design:type", Object)
    ], ItemEffects.prototype, "loadItem$", void 0);
    ItemEffects = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_ngrx_effects__WEBPACK_IMPORTED_MODULE_1__["Actions"],
            _services_item_service__WEBPACK_IMPORTED_MODULE_3__["ItemService"]])
    ], ItemEffects);
    return ItemEffects;
}());



/***/ }),

/***/ "./src/app/shared/helpers/helper.ts":
/*!******************************************!*\
  !*** ./src/app/shared/helpers/helper.ts ***!
  \******************************************/
/*! exports provided: Helper */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Helper", function() { return Helper; });
var Helper = /** @class */ (function () {
    function Helper() {
    }
    /**
     * this is a helper which cleans up the url and is used in various places
     *
     */
    Helper.stripNonUrlCharacters = function (controlValue, allowPath, trimEnd) {
        if (!controlValue) {
            return '';
        }
        var rexSeparators = allowPath ? /[^a-z0-9-_/]+/gi : /[^a-z0-9-_]+/gi;
        var latinized = this.latinizeText(controlValue.toLowerCase());
        var cleanInputValue = latinized
            .replace("'s ", 's ') // neutralize it's, daniel's etc. but only if followed by a space, to ensure we don't kill quotes
            .replace('\\', '/') // neutralize slash representation
            .replace(rexSeparators, '-') // replace everything we don't want with a -
            .replace(/-+/gi, '-') // reduce multiple '-'
            .replace(/\/+/gi, '/') // reduce multiple slashes
            .replace(/-*\/-*/gi, '/') // reduce '-/' or '/-' combinations to a simple '/'
            .replace(trimEnd ? /^-|-+$/gi : /^-/gi, ''); // trim front and maybe end '-'
        return cleanInputValue;
    };
    /**
     * latinize text input
     * @param input
     */
    Helper.latinizeText = function (input) {
        var latinMap = {
            'Á': 'A', 'Ă': 'A', 'Ắ': 'A', 'Ặ': 'A', 'Ằ': 'A', 'Ẳ': 'A', 'Ẵ': 'A', 'Ǎ': 'A', 'Â': 'A', 'Ấ': 'A', 'Ậ': 'A',
            'Ầ': 'A', 'Ẩ': 'A', 'Ẫ': 'A', 'Ä': 'Ae', 'Ǟ': 'A', 'Ȧ': 'A', 'Ǡ': 'A', 'Ạ': 'A', 'Ȁ': 'A', 'À': 'A', 'Ả': 'A', 'Ȃ': 'A',
            'Ā': 'A', 'Ą': 'A', 'Å': 'A', 'Ǻ': 'A', 'Ḁ': 'A', 'Ⱥ': 'A', 'Ã': 'A', 'Ꜳ': 'AA', 'Æ': 'AE', 'Ǽ': 'AE', 'Ǣ': 'AE',
            'Ꜵ': 'AO', 'Ꜷ': 'AU', 'Ꜹ': 'AV', 'Ꜻ': 'AV', 'Ꜽ': 'AY', 'Ḃ': 'B', 'Ḅ': 'B', 'Ɓ': 'B', 'Ḇ': 'B', 'Ƀ': 'B',
            'Ƃ': 'B', 'Ć': 'C', 'Č': 'C', 'Ç': 'C', 'Ḉ': 'C', 'Ĉ': 'C', 'Ċ': 'C', 'Ƈ': 'C', 'Ȼ': 'C', 'Ď': 'D', 'Ḑ': 'D',
            'Ḓ': 'D', 'Ḋ': 'D', 'Ḍ': 'D', 'Ɗ': 'D', 'Ḏ': 'D', 'ǲ': 'D', 'ǅ': 'D', 'Đ': 'D', 'Ƌ': 'D', 'Ǳ': 'DZ', 'Ǆ': 'DZ',
            'É': 'E', 'Ĕ': 'E', 'Ě': 'E', 'Ȩ': 'E', 'Ḝ': 'E', 'Ê': 'E', 'Ế': 'E', 'Ệ': 'E', 'Ề': 'E', 'Ể': 'E', 'Ễ': 'E', 'Ḙ': 'E',
            'Ë': 'E', 'Ė': 'E', 'Ẹ': 'E', 'Ȅ': 'E', 'È': 'E', 'Ẻ': 'E', 'Ȇ': 'E', 'Ē': 'E', 'Ḗ': 'E', 'Ḕ': 'E', 'Ę': 'E', 'Ɇ': 'E',
            'Ẽ': 'E', 'Ḛ': 'E', 'Ꝫ': 'ET', 'Ḟ': 'F', 'Ƒ': 'F', 'Ǵ': 'G', 'Ğ': 'G', 'Ǧ': 'G', 'Ģ': 'G', 'Ĝ': 'G', 'Ġ': 'G', 'Ɠ': 'G',
            'Ḡ': 'G', 'Ǥ': 'G', 'Ḫ': 'H', 'Ȟ': 'H', 'Ḩ': 'H', 'Ĥ': 'H', 'Ⱨ': 'H', 'Ḧ': 'H', 'Ḣ': 'H', 'Ḥ': 'H', 'Ħ': 'H', 'Í': 'I',
            'Ĭ': 'I', 'Ǐ': 'I', 'Î': 'I', 'Ï': 'I', 'Ḯ': 'I', 'İ': 'I', 'Ị': 'I', 'Ȉ': 'I', 'Ì': 'I', 'Ỉ': 'I', 'Ȋ': 'I', 'Ī': 'I',
            'Į': 'I', 'Ɨ': 'I', 'Ĩ': 'I', 'Ḭ': 'I', 'Ꝺ': 'D', 'Ꝼ': 'F', 'Ᵹ': 'G', 'Ꞃ': 'R', 'Ꞅ': 'S', 'Ꞇ': 'T', 'Ꝭ': 'IS', 'Ĵ': 'J',
            'Ɉ': 'J', 'Ḱ': 'K', 'Ǩ': 'K', 'Ķ': 'K', 'Ⱪ': 'K', 'Ꝃ': 'K', 'Ḳ': 'K', 'Ƙ': 'K', 'Ḵ': 'K', 'Ꝁ': 'K', 'Ꝅ': 'K', 'Ĺ': 'L',
            'Ƚ': 'L', 'Ľ': 'L', 'Ļ': 'L', 'Ḽ': 'L', 'Ḷ': 'L', 'Ḹ': 'L', 'Ⱡ': 'L', 'Ꝉ': 'L', 'Ḻ': 'L', 'Ŀ': 'L', 'Ɫ': 'L', 'ǈ': 'L',
            'Ł': 'L', 'Ǉ': 'LJ', 'Ḿ': 'M', 'Ṁ': 'M', 'Ṃ': 'M', 'Ɱ': 'M', 'Ń': 'N', 'Ň': 'N', 'Ņ': 'N', 'Ṋ': 'N', 'Ṅ': 'N', 'Ṇ': 'N',
            'Ǹ': 'N', 'Ɲ': 'N', 'Ṉ': 'N', 'Ƞ': 'N', 'ǋ': 'N', 'Ñ': 'N', 'Ǌ': 'NJ', 'Ó': 'O', 'Ŏ': 'O', 'Ǒ': 'O', 'Ô': 'O', 'Ố': 'O',
            'Ộ': 'O', 'Ồ': 'O', 'Ổ': 'O', 'Ỗ': 'O', 'Öe': 'O', 'Ȫ': 'O', 'Ȯ': 'O', 'Ȱ': 'O', 'Ọ': 'O', 'Ő': 'O', 'Ȍ': 'O', 'Ò': 'O',
            'Ỏ': 'O', 'Ơ': 'O', 'Ớ': 'O', 'Ợ': 'O', 'Ờ': 'O', 'Ở': 'O', 'Ỡ': 'O', 'Ȏ': 'O', 'Ꝋ': 'O', 'Ꝍ': 'O', 'Ō': 'O', 'Ṓ': 'O',
            'Ṑ': 'O', 'Ɵ': 'O', 'Ǫ': 'O', 'Ǭ': 'O', 'Ø': 'O', 'Ǿ': 'O', 'Õ': 'O', 'Ṍ': 'O', 'Ṏ': 'O', 'Ȭ': 'O', 'Ƣ': 'OI', 'Ꝏ': 'OO',
            'Ɛ': 'E', 'Ɔ': 'O', 'Ȣ': 'OU', 'Ṕ': 'P', 'Ṗ': 'P', 'Ꝓ': 'P', 'Ƥ': 'P', 'Ꝕ': 'P', 'Ᵽ': 'P', 'Ꝑ': 'P', 'Ꝙ': 'Q', 'Ꝗ': 'Q',
            'Ŕ': 'R', 'Ř': 'R', 'Ŗ': 'R', 'Ṙ': 'R', 'Ṛ': 'R', 'Ṝ': 'R', 'Ȑ': 'R', 'Ȓ': 'R', 'Ṟ': 'R', 'Ɍ': 'R', 'Ɽ': 'R', 'Ꜿ': 'C',
            'Ǝ': 'E', 'Ś': 'S', 'Ṥ': 'S', 'Š': 'S', 'Ṧ': 'S', 'Ş': 'S', 'Ŝ': 'S', 'Ș': 'S', 'Ṡ': 'S', 'Ṣ': 'S', 'Ṩ': 'S', 'Ť': 'T',
            'Ţ': 'T', 'Ṱ': 'T', 'Ț': 'T', 'Ⱦ': 'T', 'Ṫ': 'T', 'Ṭ': 'T', 'Ƭ': 'T', 'Ṯ': 'T', 'Ʈ': 'T', 'Ŧ': 'T', 'Ɐ': 'A', 'Ꞁ': 'L',
            'Ɯ': 'M', 'Ʌ': 'V', 'Ꜩ': 'TZ', 'Ú': 'U', 'Ŭ': 'U', 'Ǔ': 'U', 'Û': 'U', 'Ṷ': 'U', 'Ü': 'Ue', 'Ǘ': 'U', 'Ǚ': 'U', 'Ǜ': 'U',
            'Ǖ': 'U', 'Ṳ': 'U', 'Ụ': 'U', 'Ű': 'U', 'Ȕ': 'U', 'Ù': 'U', 'Ủ': 'U', 'Ư': 'U', 'Ứ': 'U', 'Ự': 'U', 'Ừ': 'U', 'Ử': 'U',
            'Ữ': 'U', 'Ȗ': 'U', 'Ū': 'U', 'Ṻ': 'U', 'Ų': 'U', 'Ů': 'U', 'Ũ': 'U', 'Ṹ': 'U', 'Ṵ': 'U', 'Ꝟ': 'V', 'Ṿ': 'V', 'Ʋ': 'V',
            'Ṽ': 'V', 'Ꝡ': 'VY', 'Ẃ': 'W', 'Ŵ': 'W', 'Ẅ': 'W', 'Ẇ': 'W', 'Ẉ': 'W', 'Ẁ': 'W', 'Ⱳ': 'W', 'Ẍ': 'X', 'Ẋ': 'X', 'Ý': 'Y',
            'Ŷ': 'Y', 'Ÿ': 'Y', 'Ẏ': 'Y', 'Ỵ': 'Y', 'Ỳ': 'Y', 'Ƴ': 'Y', 'Ỷ': 'Y', 'Ỿ': 'Y', 'Ȳ': 'Y', 'Ɏ': 'Y', 'Ỹ': 'Y', 'Ź': 'Z',
            'Ž': 'Z', 'Ẑ': 'Z', 'Ⱬ': 'Z', 'Ż': 'Z', 'Ẓ': 'Z', 'Ȥ': 'Z', 'Ẕ': 'Z', 'Ƶ': 'Z', 'Ĳ': 'IJ', 'Œ': 'OE', 'ᴀ': 'A', 'ᴁ': 'AE',
            'ʙ': 'B', 'ᴃ': 'B', 'ᴄ': 'C', 'ᴅ': 'D', 'ᴇ': 'E', 'ꜰ': 'F', 'ɢ': 'G', 'ʛ': 'G', 'ʜ': 'H', 'ɪ': 'I', 'ʁ': 'R', 'ᴊ': 'J',
            'ᴋ': 'K', 'ʟ': 'L', 'ᴌ': 'L', 'ᴍ': 'M', 'ɴ': 'N', 'ᴏ': 'O', 'ɶ': 'OE', 'ᴐ': 'O', 'ᴕ': 'OU', 'ᴘ': 'P', 'ʀ': 'R', 'ᴎ': 'N',
            'ᴙ': 'R', 'ꜱ': 'S', 'ᴛ': 'T', 'ⱻ': 'E', 'ᴚ': 'R', 'ᴜ': 'U', 'ᴠ': 'V', 'ᴡ': 'W', 'ʏ': 'Y', 'ᴢ': 'Z', 'á': 'a', 'ă': 'a',
            'ắ': 'a', 'ặ': 'a', 'ằ': 'a', 'ẳ': 'a', 'ẵ': 'a', 'ǎ': 'a', 'â': 'a', 'ấ': 'a', 'ậ': 'a', 'ầ': 'a', 'ẩ': 'a', 'ẫ': 'a',
            'ä': 'ae', 'ǟ': 'a', 'ȧ': 'a', 'ǡ': 'a', 'ạ': 'a', 'ȁ': 'a', 'à': 'a', 'ả': 'a', 'ȃ': 'a', 'ā': 'a', 'ą': 'a', 'ᶏ': 'a',
            'ẚ': 'a', 'å': 'a', 'ǻ': 'a',
            'ḁ': 'a', 'ⱥ': 'a', 'ã': 'a', 'ꜳ': 'aa', 'æ': 'ae', 'ǽ': 'ae', 'ǣ': 'ae', 'ꜵ': 'ao', 'ꜷ': 'au', 'ꜹ': 'av', 'ꜻ': 'av',
            'ꜽ': 'ay', 'ḃ': 'b', 'ḅ': 'b', 'ɓ': 'b', 'ḇ': 'b', 'ᵬ': 'b', 'ᶀ': 'b', 'ƀ': 'b', 'ƃ': 'b', 'ɵ': 'o', 'ć': 'c', 'č': 'c',
            'ç': 'c', 'ḉ': 'c', 'ĉ': 'c', 'ɕ': 'c', 'ċ': 'c', 'ƈ': 'c', 'ȼ': 'c', 'ď': 'd', 'ḑ': 'd', 'ḓ': 'd', 'ȡ': 'd', 'ḋ': 'd',
            'ḍ': 'd', 'ɗ': 'd', 'ᶑ': 'd', 'ḏ': 'd', 'ᵭ': 'd', 'ᶁ': 'd', 'đ': 'd', 'ɖ': 'd', 'ƌ': 'd', 'ı': 'i', 'ȷ': 'j', 'ɟ': 'j',
            'ʄ': 'j', 'ǳ': 'dz', 'ǆ': 'dz', 'é': 'e', 'ĕ': 'e', 'ě': 'e', 'ȩ': 'e', 'ḝ': 'e', 'ê': 'e', 'ế': 'e', 'ệ': 'e', 'ề': 'e',
            'ể': 'e', 'ễ': 'e', 'ḙ': 'e', 'ë': 'e', 'ė': 'e', 'ẹ': 'e', 'ȅ': 'e', 'è': 'e', 'ẻ': 'e', 'ȇ': 'e', 'ē': 'e', 'ḗ': 'e',
            'ḕ': 'e', 'ⱸ': 'e', 'ę': 'e', 'ᶒ': 'e', 'ɇ': 'e', 'ẽ': 'e', 'ḛ': 'e', 'ꝫ': 'et', 'ḟ': 'f', 'ƒ': 'f', 'ᵮ': 'f', 'ᶂ': 'f',
            'ǵ': 'g', 'ğ': 'g', 'ǧ': 'g', 'ģ': 'g', 'ĝ': 'g', 'ġ': 'g', 'ɠ': 'g', 'ḡ': 'g', 'ᶃ': 'g', 'ǥ': 'g', 'ḫ': 'h', 'ȟ': 'h',
            'ḩ': 'h', 'ĥ': 'h', 'ⱨ': 'h', 'ḧ': 'h', 'ḣ': 'h', 'ḥ': 'h', 'ɦ': 'h', 'ẖ': 'h', 'ħ': 'h', 'ƕ': 'hv', 'í': 'i', 'ĭ': 'i',
            'ǐ': 'i', 'î': 'i', 'ï': 'i', 'ḯ': 'i', 'ị': 'i', 'ȉ': 'i', 'ì': 'i', 'ỉ': 'i', 'ȋ': 'i', 'ī': 'i', 'į': 'i', 'ᶖ': 'i',
            'ɨ': 'i', 'ĩ': 'i', 'ḭ': 'i', 'ꝺ': 'd', 'ꝼ': 'f', 'ᵹ': 'g', 'ꞃ': 'r', 'ꞅ': 's', 'ꞇ': 't', 'ꝭ': 'is', 'ǰ': 'j', 'ĵ': 'j',
            'ʝ': 'j', 'ɉ': 'j', 'ḱ': 'k', 'ǩ': 'k', 'ķ': 'k', 'ⱪ': 'k', 'ꝃ': 'k', 'ḳ': 'k', 'ƙ': 'k', 'ḵ': 'k', 'ᶄ': 'k', 'ꝁ': 'k',
            'ꝅ': 'k', 'ĺ': 'l', 'ƚ': 'l', 'ɬ': 'l', 'ľ': 'l', 'ļ': 'l', 'ḽ': 'l', 'ȴ': 'l', 'ḷ': 'l', 'ḹ': 'l', 'ⱡ': 'l', 'ꝉ': 'l',
            'ḻ': 'l', 'ŀ': 'l', 'ɫ': 'l', 'ᶅ': 'l', 'ɭ': 'l', 'ł': 'l', 'ǉ': 'lj', 'ſ': 's', 'ẜ': 's', 'ẛ': 's', 'ẝ': 's', 'ḿ': 'm',
            'ṁ': 'm', 'ṃ': 'm', 'ɱ': 'm', 'ᵯ': 'm', 'ᶆ': 'm', 'ń': 'n', 'ň': 'n', 'ņ': 'n', 'ṋ': 'n', 'ȵ': 'n', 'ṅ': 'n', 'ṇ': 'n',
            'ǹ': 'n', 'ɲ': 'n', 'ṉ': 'n', 'ƞ': 'n', 'ᵰ': 'n', 'ᶇ': 'n', 'ɳ': 'n', 'ñ': 'n', 'ǌ': 'nj', 'ó': 'o', 'ŏ': 'o', 'ǒ': 'o',
            'ô': 'o', 'ố': 'o', 'ộ': 'o', 'ồ': 'o', 'ổ': 'o', 'ỗ': 'o', 'ö': 'oe', 'ȫ': 'o', 'ȯ': 'o', 'ȱ': 'o', 'ọ': 'o', 'ő': 'o',
            'ȍ': 'o', 'ò': 'o', 'ỏ': 'o', 'ơ': 'o', 'ớ': 'o', 'ợ': 'o', 'ờ': 'o', 'ở': 'o', 'ỡ': 'o', 'ȏ': 'o', 'ꝋ': 'o', 'ꝍ': 'o',
            'ⱺ': 'o', 'ō': 'o', 'ṓ': 'o', 'ṑ': 'o', 'ǫ': 'o', 'ǭ': 'o', 'ø': 'o', 'ǿ': 'o', 'õ': 'o', 'ṍ': 'o', 'ṏ': 'o', 'ȭ': 'o',
            'ƣ': 'oi', 'ꝏ': 'oo', 'ɛ': 'e', 'ᶓ': 'e', 'ɔ': 'o', 'ᶗ': 'o', 'ȣ': 'ou', 'ṕ': 'p', 'ṗ': 'p', 'ꝓ': 'p', 'ƥ': 'p', 'ᵱ': 'p',
            'ᶈ': 'p', 'ꝕ': 'p', 'ᵽ': 'p', 'ꝑ': 'p', 'ꝙ': 'q', 'ʠ': 'q', 'ɋ': 'q', 'ꝗ': 'q', 'ŕ': 'r', 'ř': 'r', 'ŗ': 'r', 'ṙ': 'r',
            'ṛ': 'r', 'ṝ': 'r', 'ȑ': 'r', 'ɾ': 'r', 'ᵳ': 'r', 'ȓ': 'r', 'ṟ': 'r', 'ɼ': 'r', 'ᵲ': 'r', 'ᶉ': 'r', 'ɍ': 'r', 'ɽ': 'r',
            'ↄ': 'c', 'ꜿ': 'c', 'ɘ': 'e', 'ɿ': 'r', 'ß': 'ss', 'ś': 's', 'ṥ': 's', 'š': 's', 'ṧ': 's', 'ş': 's', 'ŝ': 's', 'ș': 's',
            'ṡ': 's', 'ṣ': 's', 'ṩ': 's', 'ʂ': 's', 'ᵴ': 's', 'ᶊ': 's', 'ȿ': 's', 'ɡ': 'g', 'ᴑ': 'o', 'ᴓ': 'o', 'ᴝ': 'u', 'ť': 't',
            'ţ': 't', 'ṱ': 't', 'ț': 't', 'ȶ': 't', 'ẗ': 't', 'ⱦ': 't', 'ṫ': 't', 'ṭ': 't', 'ƭ': 't', 'ṯ': 't', 'ᵵ': 't', 'ƫ': 't',
            'ʈ': 't', 'ŧ': 't', 'ᵺ': 'th', 'ɐ': 'a', 'ᴂ': 'ae', 'ǝ': 'e', 'ᵷ': 'g', 'ɥ': 'h', 'ʮ': 'h', 'ʯ': 'h', 'ᴉ': 'i', 'ʞ': 'k',
            'ꞁ': 'l', 'ɯ': 'm', 'ɰ': 'm', 'ᴔ': 'oe', 'ɹ': 'r', 'ɻ': 'r', 'ɺ': 'r', 'ⱹ': 'r', 'ʇ': 't', 'ʌ': 'v', 'ʍ': 'w', 'ʎ': 'y',
            'ꜩ': 'tz', 'ú': 'u', 'ŭ': 'u', 'ǔ': 'u', 'û': 'u', 'ṷ': 'u', 'ü': 'ue', 'ǘ': 'u', 'ǚ': 'u', 'ǜ': 'u', 'ǖ': 'u', 'ṳ': 'u',
            'ụ': 'u', 'ű': 'u', 'ȕ': 'u', 'ù': 'u', 'ủ': 'u', 'ư': 'u', 'ứ': 'u', 'ự': 'u', 'ừ': 'u', 'ử': 'u', 'ữ': 'u', 'ȗ': 'u',
            'ū': 'u', 'ṻ': 'u', 'ų': 'u', 'ᶙ': 'u', 'ů': 'u', 'ũ': 'u', 'ṹ': 'u', 'ṵ': 'u', 'ᵫ': 'ue', 'ꝸ': 'um', 'ⱴ': 'v', 'ꝟ': 'v',
            'ṿ': 'v', 'ʋ': 'v', 'ᶌ': 'v', 'ⱱ': 'v', 'ṽ': 'v', 'ꝡ': 'vy', 'ẃ': 'w', 'ŵ': 'w', 'ẅ': 'w', 'ẇ': 'w', 'ẉ': 'w', 'ẁ': 'w',
            'ⱳ': 'w', 'ẘ': 'w', 'ẍ': 'x', 'ẋ': 'x', 'ᶍ': 'x', 'ý': 'y', 'ŷ': 'y', 'ÿ': 'y', 'ẏ': 'y', 'ỵ': 'y', 'ỳ': 'y', 'ƴ': 'y',
            'ỷ': 'y', 'ỿ': 'y', 'ȳ': 'y', 'ẙ': 'y', 'ɏ': 'y', 'ỹ': 'y', 'ź': 'z', 'ž': 'z', 'ẑ': 'z', 'ʑ': 'z', 'ⱬ': 'z', 'ż': 'z',
            'ẓ': 'z', 'ȥ': 'z', 'ẕ': 'z', 'ᵶ': 'z', 'ᶎ': 'z', 'ʐ': 'z', 'ƶ': 'z', 'ɀ': 'z', 'ﬀ': 'ff', 'ﬃ': 'ffi', 'ﬄ': 'ffl', 'ﬁ': 'fi',
            'ﬂ': 'fl', 'ĳ': 'ij', 'œ': 'oe', 'ﬆ': 'st', 'ₐ': 'a', 'ₑ': 'e', 'ᵢ': 'i', 'ⱼ': 'j', 'ₒ': 'o', 'ᵣ': 'r', 'ᵤ': 'u', 'ᵥ': 'v',
            'ₓ': 'x'
        };
        return input.replace(/[^A-Za-z0-9\[\] ]/g, function (a) { return (latinMap[a] || a); });
    };
    return Helper;
}());



/***/ }),

/***/ "./src/app/shared/helpers/localization-helper.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/helpers/localization-helper.ts ***!
  \*******************************************************/
/*! exports provided: LocalizationHelper */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LocalizationHelper", function() { return LocalizationHelper; });
/* harmony import */ var _models_eav__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../models/eav */ "./src/app/shared/models/eav/index.ts");
var __assign = (undefined && undefined.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};

var LocalizationHelper = /** @class */ (function () {
    function LocalizationHelper() {
    }
    // public static updateItemAttribute(item: Item, attributes: EavAttributes) {
    //     return {
    //         ...item,
    //         entity: {
    //             ...item.entity,
    //             attributes: attributes,
    //         }
    //     };
    // }
    /**
     * get translated value for currentLanguage,
     * if not exist return default language translation,
     * if default language also not exist return first value
     * @param currentLanguage
     * @param defaultLanguage
     * @param attributeValues
     */
    LocalizationHelper.translate = function (currentLanguage, defaultLanguage, attributeValues, defaultValue) {
        if (attributeValues) {
            var translation = this.getAttributeValueTranslation(attributeValues, currentLanguage, defaultValue);
            // if translation exist then return translation
            if (translation) {
                return translation.value;
                // return translations[0].value;
            }
            else {
                var translationDefault = this.getAttributeValueTranslation(attributeValues, defaultLanguage, defaultLanguage);
                // if default language translation exist then return translation
                if (translationDefault) {
                    return translationDefault.value;
                }
                else {
                    // else get first value
                    // TODO: maybe return value with *
                    return attributeValues.values[0] ? attributeValues.values[0].value : null;
                }
            }
        }
        else {
            return defaultValue;
        }
    };
    LocalizationHelper.updateAttribute = function (allAttributes, attribute, attributeKey) {
        // copy attributes from item
        var eavAttributes = new _models_eav__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"]();
        Object.keys(allAttributes).forEach(function (key) {
            // const eavValueList: EavValue<any>[] = [];
            if (key === attributeKey) {
                eavAttributes[key] = __assign({}, attribute);
            }
            else {
                eavAttributes[key] = __assign({}, allAttributes[key]);
            }
        });
        return eavAttributes;
    };
    /**
     * Update value for languageKey
     * @param allAttributes
     * @param updateValues
     * @param languageKey
     */
    LocalizationHelper.updateAttributesValues = function (allAttributes, updateValues, languageKey, defaultLanguage) {
        var _this = this;
        // copy attributes from item
        var eavAttributes = new _models_eav__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"]();
        console.log('saveAttributeValues attributes before ', allAttributes);
        Object.keys(allAttributes).forEach(function (attributeKey) {
            var newItemValue = updateValues[attributeKey];
            console.log('saveAttributeValues newItemValues ', newItemValue);
            // if new value exist update attribute for languageKey
            if (newItemValue !== null && newItemValue !== undefined) {
                var valueWithLanguageExist = _this.isEditableOrReadonlyTranslationExist(allAttributes[attributeKey], languageKey, defaultLanguage);
                // if valueWithLanguageExist update value for languageKey
                if (valueWithLanguageExist) {
                    console.log('saveAttributeValues update values ', newItemValue);
                    eavAttributes[attributeKey] = __assign({}, allAttributes[attributeKey], { values: allAttributes[attributeKey].values.map(function (eavValue) {
                            return eavValue.dimensions.find(function (d) { return d.value === languageKey
                                || d.value === "~" + languageKey
                                || (languageKey === defaultLanguage && d.value === '*'); })
                                // Update value for languageKey
                                ? __assign({}, eavValue, { value: newItemValue }) : eavValue;
                        }) });
                }
                else {
                    eavAttributes[attributeKey] = __assign({}, allAttributes[attributeKey]);
                }
                // else { // else add new value with dimension languageKey
                //     console.log('saveAttributeValues add values ', newItemValue);
                //     const newEavValue = new EavValue(newItemValue, [new EavDimensions(languageKey)]);
                //     eavAttributes[attributeKey] = {
                //         ...allAttributes[attributeKey],
                //         values: [...allAttributes[attributeKey].values, newEavValue]
                //     };
                // }
            }
            else { // else copy item attributes
                console.log('saveAttributeValues update values else ', newItemValue);
                eavAttributes[attributeKey] = __assign({}, allAttributes[attributeKey]);
            }
        });
        console.log('saveAttributeValues attributes after ', eavAttributes);
        return eavAttributes;
    };
    /**
     * update attribute value, and change language readonly state if needed
     * @param allAttributes
     * @param attributeKey
     * @param newValue
     * @param existingLanguageKey
     * @param isReadOnly
     */
    LocalizationHelper.updateAttributeValue = function (allAttributes, attributeKey, newValue, existingLanguageKey, defaultLanguage, isReadOnly) {
        // copy attributes from item
        var eavAttributes = new _models_eav__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"]();
        var newLanguageValue = existingLanguageKey;
        if (isReadOnly) {
            newLanguageValue = "~" + existingLanguageKey;
        }
        var attribute = __assign({}, allAttributes[attributeKey], { values: allAttributes[attributeKey].values.map(function (eavValue) {
                return eavValue.dimensions.find(function (d) { return d.value === existingLanguageKey
                    || d.value === "~" + existingLanguageKey
                    || (existingLanguageKey === defaultLanguage && d.value === '*'); })
                    // Update value and dimension
                    ? __assign({}, eavValue, { 
                        // update value
                        value: newValue, 
                        // update languageKey with newLanguageValue
                        dimensions: eavValue.dimensions.map(function (dimension) {
                            return (dimension.value === existingLanguageKey
                                || dimension.value === "~" + existingLanguageKey
                                || (existingLanguageKey === defaultLanguage && dimension.value === '*'))
                                ? { value: newLanguageValue }
                                : dimension;
                        }) }) : eavValue;
            }) });
        eavAttributes = this.updateAttribute(allAttributes, attribute, attributeKey);
        return eavAttributes;
    };
    LocalizationHelper.addAttributeValue = function (allAttributes, attributeValue, attributeKey) {
        // copy attributes from item
        var eavAttributes = new _models_eav__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"]();
        var attribute = __assign({}, allAttributes[attributeKey], { values: allAttributes[attributeKey].values.concat([attributeValue]) });
        eavAttributes = this.updateAttribute(allAttributes, attribute, attributeKey);
        return eavAttributes;
    };
    /**
     * Add dimension to value with existing dimension.
     * @param allAttributes
     * @param attributeKey
     * @param newValue
     * @param existingLanguageKey
     * @param isReadOnly
     */
    LocalizationHelper.addAttributeDimension = function (allAttributes, attributeKey, newDimensionValue, existingDimensionValue, defaultLanguage, isReadOnly) {
        // copy attributes from item
        var eavAttributes = new _models_eav__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"]();
        var newLanguageValue = newDimensionValue;
        if (isReadOnly) {
            newLanguageValue = "~" + newDimensionValue;
        }
        var attribute = __assign({}, allAttributes[attributeKey], { values: allAttributes[attributeKey].values.map(function (eavValue) {
                return eavValue.dimensions.find(function (d) { return d.value === existingDimensionValue
                    || (existingDimensionValue === defaultLanguage && d.value === '*'); })
                    // Update dimension for current language
                    ? __assign({}, eavValue, { 
                        // if languageKey already exist
                        dimensions: eavValue.dimensions.concat({ value: newLanguageValue }) }) : eavValue;
            }) });
        eavAttributes = this.updateAttribute(allAttributes, attribute, attributeKey);
        return eavAttributes;
    };
    /**
     * Remove language
     * if more dimension (languages) exist delete only dimension, else delete value and dimension
     * @param allAttributesValues
     * @param attributeKey
     * @param languageKey
     */
    LocalizationHelper.removeAttributeDimension = function (allAttributes, attributeKey, languageKey) {
        console.log('removeAttributeDimension: ', allAttributes);
        // copy attributes from item
        var eavAttributes = new _models_eav__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"]();
        var value = allAttributes[attributeKey].values.find(function (eavValue) {
            return eavValue.dimensions.find(function (d) { return d.value === languageKey
                || d.value === "~" + languageKey; }) !== undefined;
        });
        var attribute = null;
        if (!value) {
            return __assign({}, allAttributes);
        }
        // if more dimension exist delete only dimension
        if (value.dimensions.length > 1) {
            attribute = __assign({}, allAttributes[attributeKey], { values: allAttributes[attributeKey].values.map(function (eavValue) {
                    return eavValue.dimensions.find(function (d) { return d.value === languageKey || d.value === "~" + languageKey; })
                        ? __assign({}, eavValue, { 
                            // delete only dimension
                            dimensions: eavValue.dimensions.filter(function (dimension) {
                                return (dimension.value !== languageKey && dimension.value !== "~" + languageKey);
                            }) }) : eavValue;
                }) });
        }
        // if only one dimension exist delete value and dimension
        if (value.dimensions.length === 1) {
            attribute = __assign({}, allAttributes[attributeKey], { values: allAttributes[attributeKey].values.filter(function (eavValue) {
                    return eavValue.dimensions.find(function (d) { return d.value !== languageKey && d.value !== "~" + languageKey; });
                }) });
        }
        eavAttributes = this.updateAttribute(allAttributes, attribute, attributeKey);
        return eavAttributes;
    };
    LocalizationHelper.translateSettings = function (settings, currentLanguage, defaultLanguage) {
        var settingsTranslated = new _models_eav__WEBPACK_IMPORTED_MODULE_0__["EavAttributesTranslated"];
        Object.keys(settings).forEach(function (attributesKey) {
            settingsTranslated[attributesKey] = LocalizationHelper.translate(currentLanguage, defaultLanguage, settings[attributesKey], false);
        });
        return settingsTranslated;
    };
    LocalizationHelper.getAttributeValueTranslation = function (allAttributesValues, languageKey, defaultLanguage) {
        return allAttributesValues.values.find(function (eavValue) {
            return eavValue.dimensions.find(function (d) { return d.value === languageKey
                || d.value === "~" + languageKey
                || (languageKey === defaultLanguage && d.value === '*'); }) !== undefined;
        });
    };
    LocalizationHelper.isEditableOrReadonlyTranslationExist = function (allAttributesValues, languageKey, defaultLanguage) {
        return allAttributesValues.values.filter(function (c) {
            return c.dimensions.find(function (d) {
                return d.value === languageKey
                    || d.value === "~" + languageKey
                    || (languageKey === defaultLanguage && d.value === '*');
            });
        }).length > 0;
    };
    /**
     * Language is editable if langageKey exist or on default language * exist
     */
    LocalizationHelper.isEditableTranslationExist = function (allAttributesValues, languageKey, defaultLanguage) {
        return allAttributesValues ? allAttributesValues.values.filter(function (eavValue) {
            return eavValue.dimensions.find(function (d) { return (d.value === languageKey)
                || (languageKey === defaultLanguage && d.value === '*'); });
        }).length > 0 : false;
    };
    LocalizationHelper.isReadonlyTranslationExist = function (allAttributesValues, languageKey) {
        return allAttributesValues.values.filter(function (eavValue) {
            return eavValue.dimensions.find(function (d) { return d.value === "~" + languageKey; });
        }).length > 0;
    };
    return LocalizationHelper;
}());



/***/ }),

/***/ "./src/app/shared/helpers/url-helper.ts":
/*!**********************************************!*\
  !*** ./src/app/shared/helpers/url-helper.ts ***!
  \**********************************************/
/*! exports provided: UrlHelper */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UrlHelper", function() { return UrlHelper; });
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _models_eav_configuration__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../models/eav-configuration */ "./src/app/shared/models/eav-configuration.ts");


var UrlHelper = /** @class */ (function () {
    function UrlHelper() {
    }
    UrlHelper.readQueryStringParameters = function (url) {
        var queryParams = {};
        url.split('&').forEach(function (f) {
            if (f.split('=').length === 2) {
                queryParams[f.split('=')[0]] = f.split('=')[1];
            }
        });
        return queryParams;
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
    /**
 * converts a short api-call path like "/app/Blog/query/xyz" to the DNN full path
 * which varies from installation to installation like "/desktopmodules/api/2sxc/app/..."
 * @param virtualPath
 * @returns mapped path
 */
    UrlHelper.resolveServiceUrl = function (virtualPath, serviceRoot) {
        var scope = virtualPath.split('/')[0].toLowerCase();
        // stop if it's not one of our special paths
        if (this.serviceScopes.indexOf(scope) === -1) {
            return virtualPath;
        }
        return serviceRoot + scope + '/' + virtualPath.substring(virtualPath.indexOf('/') + 1);
    };
    UrlHelper.serviceScopes = ['app', 'app-sys', 'app-api', 'app-query', 'app-content', 'eav', 'view', 'dnn'];
    UrlHelper.createHeader = function (tabId, moduleId, contentBlockId) {
        return new _angular_common_http__WEBPACK_IMPORTED_MODULE_0__["HttpHeaders"]({
            'TabId': tabId,
            'ContentBlockId': moduleId,
            'ModuleId': contentBlockId,
            'Content-Type': 'application/json;charset=UTF-8',
            'RequestVerificationToken': 'abcdefgihjklmnop'
        });
    };
    /**
     * Create EavCongiguration from queryStringParams
     */
    UrlHelper.getEavConfiguration = function (queryParams) {
        return new _models_eav_configuration__WEBPACK_IMPORTED_MODULE_1__["EavConfiguration"](queryParams['zoneId'], queryParams['appId'], queryParams['approot'], queryParams['cbid'], queryParams['dialog'], queryParams['items'], queryParams['lang'], queryParams['langpri'], queryParams['langs'], queryParams['mid'], queryParams['mode'], queryParams['partOfPage'], queryParams['portalroot'], queryParams['publishing'], queryParams['tid'], queryParams['websiteroot'], UrlHelper.getVersioningOptions(queryParams['partOfPage'] === 'true', queryParams['publishing']));
    };
    return UrlHelper;
}());



/***/ }),

/***/ "./src/app/shared/interceptors/interceptors.ts":
/*!*****************************************************!*\
  !*** ./src/app/shared/interceptors/interceptors.ts ***!
  \*****************************************************/
/*! exports provided: HeaderInterceptor */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HeaderInterceptor", function() { return HeaderInterceptor; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _services_eav_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../services/eav.service */ "./src/app/shared/services/eav.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var HeaderInterceptor = /** @class */ (function () {
    function HeaderInterceptor(eavService) {
        this.eavService = eavService;
    }
    HeaderInterceptor.prototype.intercept = function (req, next) {
        if (!this.eavConfig) {
            this.eavConfig = this.eavService.getEavConfiguration();
        }
        var modified = req.clone({
            setHeaders: {
                'TabId': this.eavConfig.tid,
                'ContentBlockId': this.eavConfig.cbid,
                'ModuleId': this.eavConfig.mid,
                'Content-Type': 'application/json;charset=UTF-8',
                'RequestVerificationToken': 'abcdefgihjklmnop'
            }
        });
        return next.handle(modified);
    };
    HeaderInterceptor = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_services_eav_service__WEBPACK_IMPORTED_MODULE_1__["EavService"]])
    ], HeaderInterceptor);
    return HeaderInterceptor;
}());



/***/ }),

/***/ "./src/app/shared/models/adam/adam-config.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/models/adam/adam-config.ts ***!
  \***************************************************/
/*! exports provided: AdamConfig, AdamModeConfig */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AdamConfig", function() { return AdamConfig; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AdamModeConfig", function() { return AdamModeConfig; });
var AdamConfig = /** @class */ (function () {
    function AdamConfig(adamModeConfig, allowAssetsInRoot, autoLoad, enableSelect, fileFilter, folderDepth, metadataContentTypes, showImagesOnly, subFolder) {
        if (adamModeConfig === void 0) { adamModeConfig = { usePortalRoot: false }; }
        if (allowAssetsInRoot === void 0) { allowAssetsInRoot = true; }
        if (autoLoad === void 0) { autoLoad = false; }
        if (enableSelect === void 0) { enableSelect = true; }
        if (fileFilter === void 0) { fileFilter = ''; }
        if (folderDepth === void 0) { folderDepth = 0; }
        if (metadataContentTypes === void 0) { metadataContentTypes = ''; }
        if (showImagesOnly === void 0) { showImagesOnly = false; }
        if (subFolder === void 0) { subFolder = ''; }
        this.adamModeConfig = adamModeConfig;
        this.allowAssetsInRoot = allowAssetsInRoot;
        this.autoLoad = autoLoad;
        this.enableSelect = enableSelect;
        this.fileFilter = fileFilter;
        this.folderDepth = folderDepth;
        this.metadataContentTypes = metadataContentTypes;
        this.showImagesOnly = showImagesOnly;
        this.subFolder = subFolder;
        this.adamModeConfig = adamModeConfig;
        this.allowAssetsInRoot = allowAssetsInRoot;
        this.autoLoad = autoLoad;
        this.enableSelect = enableSelect;
        this.folderDepth = folderDepth;
        this.fileFilter = fileFilter;
        this.metadataContentTypes = metadataContentTypes;
        this.showImagesOnly = showImagesOnly;
        this.subFolder = subFolder;
    }
    return AdamConfig;
}());

var AdamModeConfig = /** @class */ (function () {
    function AdamModeConfig(usePortalRoot) {
        this.usePortalRoot = usePortalRoot;
        this.usePortalRoot = usePortalRoot;
    }
    return AdamModeConfig;
}());



/***/ }),

/***/ "./src/app/shared/models/app-state.ts":
/*!********************************************!*\
  !*** ./src/app/shared/models/app-state.ts ***!
  \********************************************/
/*! exports provided: AppState */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppState", function() { return AppState; });
var AppState = /** @class */ (function () {
    function AppState() {
    }
    return AppState;
}());



/***/ }),

/***/ "./src/app/shared/models/custom-input-type.ts":
/*!****************************************************!*\
  !*** ./src/app/shared/models/custom-input-type.ts ***!
  \****************************************************/
/*! exports provided: CustomInputType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CustomInputType", function() { return CustomInputType; });
var CustomInputType = /** @class */ (function () {
    function CustomInputType(register) {
        this.register = register;
    }
    return CustomInputType;
}());



/***/ }),

/***/ "./src/app/shared/models/eav-configuration.ts":
/*!****************************************************!*\
  !*** ./src/app/shared/models/eav-configuration.ts ***!
  \****************************************************/
/*! exports provided: EavConfiguration */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavConfiguration", function() { return EavConfiguration; });
var EavConfiguration = /** @class */ (function () {
    function EavConfiguration(zoneId, appId, approot, cbid, dialog, items, lang, langpri, langs, mid, mode, partOfPage, portalroot, publishing, tid, 
    // public user[canDesign]: string,
    // public user[canDevelop]: string,
    websiteroot, 
    // TODO: write type instead any
    versioningOptions) {
        this.zoneId = zoneId;
        this.appId = appId;
        this.approot = approot;
        this.cbid = cbid;
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
        this.websiteroot = websiteroot;
        this.versioningOptions = versioningOptions;
        this.appId = appId;
        this.approot = approot;
        this.cbid = cbid;
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
        // this.user[canDesign] = user[canDesign];
        // this.user[canDevelop] = user[canDevelop];
        this.websiteroot = websiteroot;
        this.versioningOptions = versioningOptions;
    }
    return EavConfiguration;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/admin-dialog-data.ts":
/*!********************************************************!*\
  !*** ./src/app/shared/models/eav/admin-dialog-data.ts ***!
  \********************************************************/
/*! exports provided: AdminDialogData */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AdminDialogData", function() { return AdminDialogData; });
var AdminDialogData = /** @class */ (function () {
    function AdminDialogData(id, type) {
        this.id = id;
        this.type = type;
        this.id = id;
        this.type = type;
    }
    return AdminDialogData;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/attribute-def.ts":
/*!****************************************************!*\
  !*** ./src/app/shared/models/eav/attribute-def.ts ***!
  \****************************************************/
/*! exports provided: AttributeDef */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AttributeDef", function() { return AttributeDef; });
/* harmony import */ var _eav_entity__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-entity */ "./src/app/shared/models/eav/eav-entity.ts");
/* harmony import */ var _eav_attributes__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-attributes */ "./src/app/shared/models/eav/eav-attributes.ts");


var AttributeDef = /** @class */ (function () {
    function AttributeDef(name, type, isTitle, metadata, settings) {
        this.name = name;
        this.type = type;
        this.isTitle = isTitle;
        this.settings = settings;
        this.metadata = metadata;
    }
    /**
     * Create new AttributeDef from json typed AttributeDef1
     * @param item
     */
    AttributeDef.create = function (item) {
        // console.log('AttributeDef1:', item);
        var metaDataArray = _eav_entity__WEBPACK_IMPORTED_MODULE_0__["EavEntity"].createArray(item.Metadata);
        var settings = _eav_attributes__WEBPACK_IMPORTED_MODULE_1__["EavAttributes"].getFromEavEntityArray(metaDataArray);
        return new AttributeDef(item.Name, item.Type, item.IsTitle, metaDataArray, settings);
    };
    /**
     * Create new AttributeDef[] from json typed AttributeDef1[]
     * @param item
     */
    AttributeDef.createArray = function (attributeDef1Array) {
        var attributeDefArray = [];
        if (attributeDef1Array !== undefined) {
            attributeDef1Array.forEach(function (attributeDef1) {
                attributeDefArray.push(AttributeDef.create(attributeDef1));
            });
        }
        return attributeDefArray;
    };
    return AttributeDef;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/content-type-def.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/models/eav/content-type-def.ts ***!
  \*******************************************************/
/*! exports provided: ContentTypeDef */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeDef", function() { return ContentTypeDef; });
/* harmony import */ var _attribute_def__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./attribute-def */ "./src/app/shared/models/eav/attribute-def.ts");
/* harmony import */ var _eav_entity__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-entity */ "./src/app/shared/models/eav/eav-entity.ts");


var ContentTypeDef = /** @class */ (function () {
    function ContentTypeDef(id, name, scope, description, attributes, metadata) {
        this.id = id;
        this.name = name;
        this.scope = scope;
        this.description = description;
        this.attributes = attributes;
    }
    /**
     * Create ContentTypeDef from json typed ContentType1
     * @param item
     */
    ContentTypeDef.create = function (item) {
        var attributeDefArray = _attribute_def__WEBPACK_IMPORTED_MODULE_0__["AttributeDef"].createArray(item.Attributes);
        var metaDataArray = _eav_entity__WEBPACK_IMPORTED_MODULE_1__["EavEntity"].createArray(item.Metadata);
        return new ContentTypeDef(item.Id, item.Name, item.Scope, item.Description, attributeDefArray, metaDataArray);
    };
    return ContentTypeDef;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/content-type.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/models/eav/content-type.ts ***!
  \***************************************************/
/*! exports provided: ContentType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentType", function() { return ContentType; });
/* harmony import */ var _content_type_def__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./content-type-def */ "./src/app/shared/models/eav/content-type-def.ts");
/* harmony import */ var _eav_header__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-header */ "./src/app/shared/models/eav/eav-header.ts");
/* harmony import */ var _json_format_v1__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../json-format-v1 */ "./src/app/shared/models/json-format-v1/index.ts");



var ContentType = /** @class */ (function () {
    function ContentType(header, contentType) {
        this.header = header;
        this.contentType = contentType;
    }
    /**
     * Create new ContentType from json typed JsonContentType
     * @param item
     */
    ContentType.create = function (contentType) {
        return new ContentType(
        // EavHeader.create(item._),
        // TODO: finish content type header from load
        _eav_header__WEBPACK_IMPORTED_MODULE_1__["EavHeader"].create(new _json_format_v1__WEBPACK_IMPORTED_MODULE_2__["JsonHeader1"](1, '', '', null, null, null, '', null)), _content_type_def__WEBPACK_IMPORTED_MODULE_0__["ContentTypeDef"].create(contentType));
    };
    return ContentType;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-attributes-translated.ts":
/*!****************************************************************!*\
  !*** ./src/app/shared/models/eav/eav-attributes-translated.ts ***!
  \****************************************************************/
/*! exports provided: EavAttributesTranslated */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavAttributesTranslated", function() { return EavAttributesTranslated; });
var EavAttributesTranslated = /** @class */ (function () {
    function EavAttributesTranslated() {
    }
    return EavAttributesTranslated;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-attributes.ts":
/*!*****************************************************!*\
  !*** ./src/app/shared/models/eav/eav-attributes.ts ***!
  \*****************************************************/
/*! exports provided: EavAttributes */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavAttributes", function() { return EavAttributes; });
/* harmony import */ var _eav_values__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-values */ "./src/app/shared/models/eav/eav-values.ts");
/* harmony import */ var _eav_value__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-value */ "./src/app/shared/models/eav/eav-value.ts");


var EavAttributes = /** @class */ (function () {
    function EavAttributes() {
    }
    /**
     * Create Eav Attributes from json typed Attributes1
     * @param attributes1
     */
    EavAttributes.create = function (attributes1) {
        var newEavAtribute = new EavAttributes();
        // Loop trough attributes types - String, Boolean ...
        Object.keys(attributes1).forEach(function (attributes1Key) {
            if (attributes1.hasOwnProperty(attributes1Key)) {
                var attribute1_1 = attributes1[attributes1Key];
                // Loop trough attribute - Description, Name ...
                Object.keys(attribute1_1).forEach(function (attribute1Key) {
                    if (attribute1_1.hasOwnProperty(attribute1Key)) {
                        // Creates new EavValue for specified type
                        newEavAtribute[attribute1Key] = _eav_values__WEBPACK_IMPORTED_MODULE_0__["EavValues"].create(attribute1_1[attribute1Key]);
                    }
                });
            }
        });
        console.log('created attributes: ', newEavAtribute);
        return newEavAtribute;
    };
    /**
     * Get all attributes (dictionary) from attributs in EavEntity array (all attributs from each entity in array)
     * Example: Settings from metadata array
     * @param entity1Array
     */
    EavAttributes.getFromEavEntityArray = function (eavEntityArray) {
        var newEavAtribute = new EavAttributes();
        if (eavEntityArray !== undefined) {
            // First read all metadata settings witch are not @All
            eavEntityArray.forEach(function (eavEntity) {
                if (eavEntity.type.id !== '@All') {
                    Object.keys(eavEntity.attributes).forEach(function (attributeKey) {
                        newEavAtribute[attributeKey] = Object.assign({}, eavEntity.attributes[attributeKey]);
                    });
                }
            });
            // Read @All metadata settings last (to rewrite attribute if attribute with same name exist)
            eavEntityArray.forEach(function (eavEntity) {
                if (eavEntity.type.id === '@All') {
                    Object.keys(eavEntity.attributes).forEach(function (attributeKey) {
                        newEavAtribute[attributeKey] = Object.assign({}, eavEntity.attributes[attributeKey]);
                    });
                }
            });
        }
        return newEavAtribute;
    };
    /**
     * Create EavAtributes from dictionary
     */
    EavAttributes.createFromDictionary = function (value) {
        var eavAttributes = new EavAttributes();
        Object.keys(value).forEach(function (valueKey) {
            var eavValues = [];
            eavAttributes[valueKey] = new _eav_values__WEBPACK_IMPORTED_MODULE_0__["EavValues"]([new _eav_value__WEBPACK_IMPORTED_MODULE_1__["EavValue"](value[valueKey], [])]);
        });
        return eavAttributes;
    };
    return EavAttributes;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-dimensions.ts":
/*!*****************************************************!*\
  !*** ./src/app/shared/models/eav/eav-dimensions.ts ***!
  \*****************************************************/
/*! exports provided: EavDimensions */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavDimensions", function() { return EavDimensions; });
var EavDimensions = /** @class */ (function () {
    function EavDimensions(value) {
        this.value = value;
    }
    /**
     * Create Eav Dimensions from typed json Value1
     * @param value
     */
    /* public static create<T>(value1: Value1<T>): EavDimensions<T>[] {

        const asd: EavDimensions<T> = new EavDimensions<T>();

        const asdarray: EavDimensions<T>[] = [];

        // Loop trough attribute - Description, Name ...
        Object.keys(value1).forEach(value1Key => {
            if (value1.hasOwnProperty(value1Key)) {
                // Creates new EavValue for specified type
                newEavAtribute[attribute1Key] = EavValue.create<any>(attribute1[attribute1Key]);
            }
        });

        asdarray.push(new )

        return new EavDimensions<T>('*', value['*']);
    } */
    /**
     * Get attribute dimensions for current language
     * @param item
     * @param attributeKey
     * @param currentLanguage
     */
    EavDimensions.getEavAttributeDimensionsForLanguage = function (attribute, attributeKey, currentLanguage) {
        var eavAttribute = attribute[attributeKey];
        var dimensions = eavAttribute.values.map(function (eavValue) {
            return eavValue.dimensions.find(function (d) { return d.value === currentLanguage; });
        });
        return dimensions;
    };
    EavDimensions.update = function () {
        console.log('zovni');
    };
    return EavDimensions;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-entity.ts":
/*!*************************************************!*\
  !*** ./src/app/shared/models/eav/eav-entity.ts ***!
  \*************************************************/
/*! exports provided: EavEntity */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavEntity", function() { return EavEntity; });
/* harmony import */ var _eav_attributes__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-attributes */ "./src/app/shared/models/eav/eav-attributes.ts");
/* harmony import */ var _eav_type__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-type */ "./src/app/shared/models/eav/eav-type.ts");

// import { EavAttributes } from './eav-attributes';

var EavEntity = /** @class */ (function () {
    function EavEntity(id, version, guid, type, attributes, owner, metadata) {
        this.id = id;
        this.version = version;
        this.guid = guid;
        this.type = type;
        this.attributes = attributes;
        this.owner = owner;
        this.metadata = metadata;
    }
    /**
     * Create new Eav Entity from typed json Entity1
     * @param item
     */
    EavEntity.create = function (item) {
        console.log('create item.Attributes:', item.Attributes);
        var eavAttributes = _eav_attributes__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"].create(item.Attributes);
        var eavMetaData = this.createArray(item.Metadata);
        return new EavEntity(item.Id, item.Version, item.Guid, new _eav_type__WEBPACK_IMPORTED_MODULE_1__["EavType"](item.Type.Id, item.Type.Name), eavAttributes, item.Owner, eavMetaData);
    };
    /**
    * Create new MetaData Entity Array from json typed metadataArray Entity1[]
    * @param item
    */
    EavEntity.createArray = function (entity1Array) {
        var eavMetaDataArray = new Array();
        if (entity1Array !== undefined && entity1Array !== null) {
            console.log('entity1Array:', entity1Array);
            entity1Array.forEach(function (entity1) {
                eavMetaDataArray.push(EavEntity.create(entity1));
            });
        }
        return eavMetaDataArray;
    };
    return EavEntity;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-group-assignment.ts":
/*!***********************************************************!*\
  !*** ./src/app/shared/models/eav/eav-group-assignment.ts ***!
  \***********************************************************/
/*! exports provided: EavGroupAssignment */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavGroupAssignment", function() { return EavGroupAssignment; });
var EavGroupAssignment = /** @class */ (function () {
    function EavGroupAssignment(guid, part, index, add, slotCanBeEmpty, slotIsEmpty, contentBlockAppId) {
        this.guid = guid;
        this.part = part;
        this.index = index;
        this.add = add;
        this.slotCanBeEmpty = slotCanBeEmpty;
        this.slotIsEmpty = slotIsEmpty;
        this.contentBlockAppId = contentBlockAppId;
    }
    EavGroupAssignment.create = function (groupAssignment1) {
        return groupAssignment1 ? new EavGroupAssignment(groupAssignment1.Guid, groupAssignment1.Part, groupAssignment1.Index, groupAssignment1.Add, groupAssignment1.SlotCanBeEmpty, groupAssignment1.SlotIsEmpty, groupAssignment1.ContentBlockAppId) : null;
    };
    return EavGroupAssignment;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-header.ts":
/*!*************************************************!*\
  !*** ./src/app/shared/models/eav/eav-header.ts ***!
  \*************************************************/
/*! exports provided: EavHeader */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavHeader", function() { return EavHeader; });
/* harmony import */ var _eav_group_assignment__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-group-assignment */ "./src/app/shared/models/eav/eav-group-assignment.ts");
/* harmony import */ var _eav_entity__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-entity */ "./src/app/shared/models/eav/eav-entity.ts");


var EavHeader = /** @class */ (function () {
    function EavHeader(v, entityId, guid, contentTypeName, metadata, group, prefill, title, duplicateEntity) {
        this.v = v;
        this.entityId = entityId;
        this.guid = guid;
        this.contentTypeName = contentTypeName;
        this.metadata = metadata;
        this.group = group;
        this.prefill = prefill;
        this.title = title;
        this.duplicateEntity = duplicateEntity;
    }
    /**
     * Create Eav Header from typed json JsonHeader1
     * @param item
     */
    EavHeader.create = function (item) {
        var metadataArray = _eav_entity__WEBPACK_IMPORTED_MODULE_1__["EavEntity"].createArray(item.Metadata);
        var eavGroupAssignment = _eav_group_assignment__WEBPACK_IMPORTED_MODULE_0__["EavGroupAssignment"].create(item.Group);
        return new EavHeader(1, item.EntityId, item.Guid, item.ContentTypeName, metadataArray, eavGroupAssignment, item.Prefill, item.Title, item.DuplicateEntity);
    };
    return EavHeader;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-type.ts":
/*!***********************************************!*\
  !*** ./src/app/shared/models/eav/eav-type.ts ***!
  \***********************************************/
/*! exports provided: EavType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavType", function() { return EavType; });
var EavType = /** @class */ (function () {
    function EavType(id, name) {
        this.id = id;
        this.name = name;
    }
    return EavType;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-value.ts":
/*!************************************************!*\
  !*** ./src/app/shared/models/eav/eav-value.ts ***!
  \************************************************/
/*! exports provided: EavValue */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavValue", function() { return EavValue; });
/* harmony import */ var _eav_dimensions__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-dimensions */ "./src/app/shared/models/eav/eav-dimensions.ts");

var EavValue = /** @class */ (function () {
    function EavValue(value, dimensions) {
        this.value = value;
        this.dimensions = dimensions;
    }
    /**
     * Create Eav Value from typed json Value1
     * @param value
     */
    EavValue.create = function (value1) {
        var newEavValueArray = []; // = new EavValue(value1,);
        // Loop trough value1 - {'*', 'value'} ...
        Object.keys(value1).forEach(function (value1Key) {
            if (value1.hasOwnProperty(value1Key)) {
                var dimensions_1 = [];
                value1Key.split(',').forEach(function (language) {
                    dimensions_1.push(new _eav_dimensions__WEBPACK_IMPORTED_MODULE_0__["EavDimensions"](language));
                });
                // Creates new EavValue for specified type and add to array
                newEavValueArray.push(new EavValue(value1[value1Key], dimensions_1));
            }
        });
        return newEavValueArray;
    };
    return EavValue;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/eav-values.ts":
/*!*************************************************!*\
  !*** ./src/app/shared/models/eav/eav-values.ts ***!
  \*************************************************/
/*! exports provided: EavValues */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavValues", function() { return EavValues; });
/* harmony import */ var _eav_value__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-value */ "./src/app/shared/models/eav/eav-value.ts");

var EavValues = /** @class */ (function () {
    function EavValues(values) {
        this.values = values;
    }
    /**
     * Create Eav Value from typed json Value1
     * @param value
     */
    EavValues.create = function (value1) {
        return new EavValues(_eav_value__WEBPACK_IMPORTED_MODULE_0__["EavValue"].create(value1));
    };
    return EavValues;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/index.ts":
/*!********************************************!*\
  !*** ./src/app/shared/models/eav/index.ts ***!
  \********************************************/
/*! exports provided: EavAttributes, EavEntity, Item, EavValue, EavValues, EavDimensions, ContentType, EavHeader, Language, EavAttributesTranslated, InputType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _eav_attributes__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-attributes */ "./src/app/shared/models/eav/eav-attributes.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EavAttributes", function() { return _eav_attributes__WEBPACK_IMPORTED_MODULE_0__["EavAttributes"]; });

/* harmony import */ var _eav_entity__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-entity */ "./src/app/shared/models/eav/eav-entity.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EavEntity", function() { return _eav_entity__WEBPACK_IMPORTED_MODULE_1__["EavEntity"]; });

/* harmony import */ var _item__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./item */ "./src/app/shared/models/eav/item.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "Item", function() { return _item__WEBPACK_IMPORTED_MODULE_2__["Item"]; });

/* harmony import */ var _eav_value__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./eav-value */ "./src/app/shared/models/eav/eav-value.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EavValue", function() { return _eav_value__WEBPACK_IMPORTED_MODULE_3__["EavValue"]; });

/* harmony import */ var _eav_values__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./eav-values */ "./src/app/shared/models/eav/eav-values.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EavValues", function() { return _eav_values__WEBPACK_IMPORTED_MODULE_4__["EavValues"]; });

/* harmony import */ var _eav_dimensions__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./eav-dimensions */ "./src/app/shared/models/eav/eav-dimensions.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EavDimensions", function() { return _eav_dimensions__WEBPACK_IMPORTED_MODULE_5__["EavDimensions"]; });

/* harmony import */ var _content_type__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./content-type */ "./src/app/shared/models/eav/content-type.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "ContentType", function() { return _content_type__WEBPACK_IMPORTED_MODULE_6__["ContentType"]; });

/* harmony import */ var _eav_header__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./eav-header */ "./src/app/shared/models/eav/eav-header.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EavHeader", function() { return _eav_header__WEBPACK_IMPORTED_MODULE_7__["EavHeader"]; });

/* harmony import */ var _language__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./language */ "./src/app/shared/models/eav/language.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "Language", function() { return _language__WEBPACK_IMPORTED_MODULE_8__["Language"]; });

/* harmony import */ var _eav_attributes_translated__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./eav-attributes-translated */ "./src/app/shared/models/eav/eav-attributes-translated.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EavAttributesTranslated", function() { return _eav_attributes_translated__WEBPACK_IMPORTED_MODULE_9__["EavAttributesTranslated"]; });

/* harmony import */ var _input_type__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./input-type */ "./src/app/shared/models/eav/input-type.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "InputType", function() { return _input_type__WEBPACK_IMPORTED_MODULE_10__["InputType"]; });














/***/ }),

/***/ "./src/app/shared/models/eav/input-type.ts":
/*!*************************************************!*\
  !*** ./src/app/shared/models/eav/input-type.ts ***!
  \*************************************************/
/*! exports provided: InputType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InputType", function() { return InputType; });
var InputType = /** @class */ (function () {
    function InputType(Assets, Description, Label, Type) {
        this.Assets = Assets;
        this.Description = Description;
        this.Label = Label;
        this.Type = Type;
        this.Assets = Assets;
        this.Description = Description;
        this.Label = Label;
        this.Type = Type;
    }
    return InputType;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/item.ts":
/*!*******************************************!*\
  !*** ./src/app/shared/models/eav/item.ts ***!
  \*******************************************/
/*! exports provided: Item */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Item", function() { return Item; });
/* harmony import */ var _eav_entity__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./eav-entity */ "./src/app/shared/models/eav/eav-entity.ts");
/* harmony import */ var _eav_header__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./eav-header */ "./src/app/shared/models/eav/eav-header.ts");


var Item = /** @class */ (function () {
    function Item(header, entity) {
        this.header = header;
        this.entity = entity;
    }
    /**
     * Create new Eav Item from json typed JsonItem1
     * @param item
     */
    Item.create = function (item) {
        console.log('create item.Entity:', item.Entity);
        return new Item(
        // EavHeader.create(item._),
        _eav_header__WEBPACK_IMPORTED_MODULE_1__["EavHeader"].create(item.Header), _eav_entity__WEBPACK_IMPORTED_MODULE_0__["EavEntity"].create(item.Entity));
    };
    return Item;
}());



/***/ }),

/***/ "./src/app/shared/models/eav/language.ts":
/*!***********************************************!*\
  !*** ./src/app/shared/models/eav/language.ts ***!
  \***********************************************/
/*! exports provided: Language */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Language", function() { return Language; });
var Language = /** @class */ (function () {
    function Language(key, name) {
        this.key = key;
        this.name = name;
    }
    return Language;
}());



/***/ }),

/***/ "./src/app/shared/models/index.ts":
/*!****************************************!*\
  !*** ./src/app/shared/models/index.ts ***!
  \****************************************/
/*! exports provided: AppState, CustomInputType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _app_state__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./app-state */ "./src/app/shared/models/app-state.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "AppState", function() { return _app_state__WEBPACK_IMPORTED_MODULE_0__["AppState"]; });

/* harmony import */ var _custom_input_type__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./custom-input-type */ "./src/app/shared/models/custom-input-type.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "CustomInputType", function() { return _custom_input_type__WEBPACK_IMPORTED_MODULE_1__["CustomInputType"]; });





/***/ }),

/***/ "./src/app/shared/models/json-format-v1/attribute-def1.ts":
/*!****************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/attribute-def1.ts ***!
  \****************************************************************/
/*! exports provided: AttributeDef1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AttributeDef1", function() { return AttributeDef1; });
var AttributeDef1 = /** @class */ (function () {
    function AttributeDef1(Name, Type, IsTitle, Metadata) {
        this.Name = Name;
        this.Type = Type;
        this.IsTitle = IsTitle;
        this.Metadata = Metadata;
    }
    return AttributeDef1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/attribute1.ts":
/*!************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/attribute1.ts ***!
  \************************************************************/
/*! exports provided: Attribute1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Attribute1", function() { return Attribute1; });
var Attribute1 = /** @class */ (function () {
    function Attribute1() {
    }
    return Attribute1;
}());

/* "Attributes": {
    "String": {
        "Description": {
            "*": "Retrieve full list of all zones"
        },
        "Name": {
            "*": "Zones"
        },
        "StreamsOut": {
            "*": "ListContent,Default"
        },
        "StreamWiring": {
            "*": "3cef3168-5fe8-4417-8ee0-c47642181a1e:Default>Out:Default"
        },
        "TestParameters": {
            "*": "[Module:ModuleID]=6936"
        }
    },
    "Boolean": {
        "AllowEdit": {
            "*": true
        }
    }
}, */


/***/ }),

/***/ "./src/app/shared/models/json-format-v1/attributes1.ts":
/*!*************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/attributes1.ts ***!
  \*************************************************************/
/*! exports provided: Attributes1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Attributes1", function() { return Attributes1; });
/* harmony import */ var _value1__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./value1 */ "./src/app/shared/models/json-format-v1/value1.ts");

var Attributes1 = /** @class */ (function () {
    function Attributes1() {
    }
    Attributes1.create = function (eavAttributes) {
        var newAttribute1 = new Attributes1();
        Object.keys(eavAttributes).forEach(function (eavAttributeKey) {
            if (eavAttributes.hasOwnProperty(eavAttributeKey)) {
                newAttribute1[eavAttributeKey] = _value1__WEBPACK_IMPORTED_MODULE_0__["Value1"].create(eavAttributes[eavAttributeKey]);
            }
        });
        return newAttribute1;
    };
    return Attributes1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/content-type-def1.ts":
/*!*******************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/content-type-def1.ts ***!
  \*******************************************************************/
/*! exports provided: ContentTypeDef1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeDef1", function() { return ContentTypeDef1; });
var ContentTypeDef1 = /** @class */ (function () {
    function ContentTypeDef1(Id, Name, Scope, Description, Attributes, Metadata) {
        this.Id = Id;
        this.Name = Name;
        this.Scope = Scope;
        this.Description = Description;
        this.Attributes = Attributes;
        this.Metadata = Metadata;
    }
    return ContentTypeDef1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/entity1.ts":
/*!*********************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/entity1.ts ***!
  \*********************************************************/
/*! exports provided: Entity1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Entity1", function() { return Entity1; });
/* harmony import */ var _attributes1__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./attributes1 */ "./src/app/shared/models/json-format-v1/attributes1.ts");
/* harmony import */ var _type1__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./type1 */ "./src/app/shared/models/json-format-v1/type1.ts");


var Entity1 = /** @class */ (function () {
    function Entity1(Id, Version, Guid, Type, Attributes, Owner, Metadata) {
        this.Id = Id;
        this.Version = Version;
        this.Guid = Guid;
        this.Type = Type;
        this.Attributes = Attributes;
        this.Owner = Owner;
        this.Metadata = Metadata;
    }
    /* public static create(item: Entity1): Entity1 {
        return new Entity1(item.Id,
            item.Version,
            item.Guid,
            item.Type,
            item.Attributes,
            item.Owner,
            item.Metadata);
    } */
    Entity1.create = function (entity) {
        var attributes1 = _attributes1__WEBPACK_IMPORTED_MODULE_0__["Attributes1"].create(entity.attributes);
        var metaData1 = this.createArray(entity.metadata);
        return new Entity1(entity.id, entity.version, entity.guid, new _type1__WEBPACK_IMPORTED_MODULE_1__["Type1"](entity.type.id, entity.type.name), attributes1, entity.owner, metaData1);
    };
    Entity1.createArray = function (eavEntityArray) {
        var metaData1Array = new Array();
        if (eavEntityArray !== undefined && eavEntityArray !== null) {
            eavEntityArray.forEach(function (eavEntity) {
                metaData1Array.push(Entity1.create(eavEntity));
            });
        }
        return metaData1Array;
    };
    return Entity1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/group-assignment1.ts":
/*!*******************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/group-assignment1.ts ***!
  \*******************************************************************/
/*! exports provided: GroupAssignment1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "GroupAssignment1", function() { return GroupAssignment1; });
var GroupAssignment1 = /** @class */ (function () {
    function GroupAssignment1(guid, part, index, add, slotCanBeEmpty, slotIsEmpty, contentBlockAppId) {
        this.Guid = guid;
        this.Part = part;
        this.Index = index;
        this.Add = add;
        this.SlotCanBeEmpty = slotCanBeEmpty;
        this.SlotIsEmpty = slotIsEmpty;
        this.ContentBlockAppId = contentBlockAppId;
    }
    GroupAssignment1.create = function (eavGroupAssignment) {
        return eavGroupAssignment ? new GroupAssignment1(eavGroupAssignment.guid, eavGroupAssignment.part, eavGroupAssignment.index, eavGroupAssignment.add, eavGroupAssignment.slotCanBeEmpty, eavGroupAssignment.slotIsEmpty, eavGroupAssignment.contentBlockAppId) : null;
    };
    return GroupAssignment1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/index.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/index.ts ***!
  \*******************************************************/
/*! exports provided: ContentTypeDef1, AttributeDef1, Entity1, JsonContentType1, JsonItem1, JsonPackage1, JsonHeader1, Attribute1, Type1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _content_type_def1__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./content-type-def1 */ "./src/app/shared/models/json-format-v1/content-type-def1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "ContentTypeDef1", function() { return _content_type_def1__WEBPACK_IMPORTED_MODULE_0__["ContentTypeDef1"]; });

/* harmony import */ var _attribute_def1__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./attribute-def1 */ "./src/app/shared/models/json-format-v1/attribute-def1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "AttributeDef1", function() { return _attribute_def1__WEBPACK_IMPORTED_MODULE_1__["AttributeDef1"]; });

/* harmony import */ var _entity1__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./entity1 */ "./src/app/shared/models/json-format-v1/entity1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "Entity1", function() { return _entity1__WEBPACK_IMPORTED_MODULE_2__["Entity1"]; });

/* harmony import */ var _json_content_type1__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./json-content-type1 */ "./src/app/shared/models/json-format-v1/json-content-type1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "JsonContentType1", function() { return _json_content_type1__WEBPACK_IMPORTED_MODULE_3__["JsonContentType1"]; });

/* harmony import */ var _json_item1__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./json-item1 */ "./src/app/shared/models/json-format-v1/json-item1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "JsonItem1", function() { return _json_item1__WEBPACK_IMPORTED_MODULE_4__["JsonItem1"]; });

/* harmony import */ var _json_package1__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./json-package1 */ "./src/app/shared/models/json-format-v1/json-package1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "JsonPackage1", function() { return _json_package1__WEBPACK_IMPORTED_MODULE_5__["JsonPackage1"]; });

/* harmony import */ var _json_header1__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./json-header1 */ "./src/app/shared/models/json-format-v1/json-header1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "JsonHeader1", function() { return _json_header1__WEBPACK_IMPORTED_MODULE_6__["JsonHeader1"]; });

/* harmony import */ var _attribute1__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./attribute1 */ "./src/app/shared/models/json-format-v1/attribute1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "Attribute1", function() { return _attribute1__WEBPACK_IMPORTED_MODULE_7__["Attribute1"]; });

/* harmony import */ var _type1__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./type1 */ "./src/app/shared/models/json-format-v1/type1.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "Type1", function() { return _type1__WEBPACK_IMPORTED_MODULE_8__["Type1"]; });












/***/ }),

/***/ "./src/app/shared/models/json-format-v1/json-content-type1.ts":
/*!********************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/json-content-type1.ts ***!
  \********************************************************************/
/*! exports provided: JsonContentType1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "JsonContentType1", function() { return JsonContentType1; });
var JsonContentType1 = /** @class */ (function () {
    function JsonContentType1(_, ContentType) {
        this._ = _;
        this.ContentType = ContentType;
    }
    return JsonContentType1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/json-header1.ts":
/*!**************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/json-header1.ts ***!
  \**************************************************************/
/*! exports provided: JsonHeader1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "JsonHeader1", function() { return JsonHeader1; });
/* harmony import */ var _group_assignment1__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./group-assignment1 */ "./src/app/shared/models/json-format-v1/group-assignment1.ts");
/* harmony import */ var _entity1__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./entity1 */ "./src/app/shared/models/json-format-v1/entity1.ts");


var JsonHeader1 = /** @class */ (function () {
    function JsonHeader1(entityId, guid, contentTypeName, metadata, group, prefill, title, duplicateEntity) {
        this.EntityId = entityId;
        this.Guid = guid;
        this.ContentTypeName = contentTypeName;
        this.Metadata = metadata;
        this.Group = group;
        this.Prefill = prefill;
        this.Title = title;
        this.DuplicateEntity = duplicateEntity;
    }
    /* public static create(item: JsonHeader1): JsonHeader1 {
        return new JsonHeader1(item.V);
    } */
    JsonHeader1.create = function (item) {
        var metaDataArray = _entity1__WEBPACK_IMPORTED_MODULE_1__["Entity1"].createArray(item.metadata);
        var groupAssignment1 = _group_assignment1__WEBPACK_IMPORTED_MODULE_0__["GroupAssignment1"].create(item.group);
        return new JsonHeader1(item.entityId, item.guid, item.contentTypeName, metaDataArray, groupAssignment1, item.prefill, item.title, item.duplicateEntity);
    };
    return JsonHeader1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/json-item1.ts":
/*!************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/json-item1.ts ***!
  \************************************************************/
/*! exports provided: JsonItem1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "JsonItem1", function() { return JsonItem1; });
/* harmony import */ var _entity1__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./entity1 */ "./src/app/shared/models/json-format-v1/entity1.ts");
/* harmony import */ var _json_header1__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./json-header1 */ "./src/app/shared/models/json-format-v1/json-header1.ts");


var JsonItem1 = /** @class */ (function () {
    function JsonItem1(Header, Entity) {
        this.Header = Header;
        this.Entity = Entity;
    }
    /* public static create(item: JsonItem1): JsonItem1 {
        item._ = JsonHeader1.create(item._);
        item.Entity = Entity1.create(item.Entity);
        return new JsonItem1(item._, item.Entity);
    } */
    JsonItem1.create = function (item) {
        return new JsonItem1(_json_header1__WEBPACK_IMPORTED_MODULE_1__["JsonHeader1"].create(item.header), _entity1__WEBPACK_IMPORTED_MODULE_0__["Entity1"].create(item.entity));
    };
    return JsonItem1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/json-package1.ts":
/*!***************************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/json-package1.ts ***!
  \***************************************************************/
/*! exports provided: JsonPackage1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "JsonPackage1", function() { return JsonPackage1; });
var JsonPackage1 = /** @class */ (function () {
    // EntityType?: Entity1;
    function JsonPackage1(_, ContentType) {
        this._ = _;
        this.ContentType = ContentType;
    }
    return JsonPackage1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/type1.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/type1.ts ***!
  \*******************************************************/
/*! exports provided: Type1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Type1", function() { return Type1; });
var Type1 = /** @class */ (function () {
    function Type1(Id, Name) {
        this.Id = Id;
        this.Name = Name;
    }
    return Type1;
}());



/***/ }),

/***/ "./src/app/shared/models/json-format-v1/value1.ts":
/*!********************************************************!*\
  !*** ./src/app/shared/models/json-format-v1/value1.ts ***!
  \********************************************************/
/*! exports provided: Value1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Value1", function() { return Value1; });
var Value1 = /** @class */ (function () {
    function Value1() {
    }
    Value1.create = function (eavValues) {
        var newValue1 = {};
        console.log('eavValues.values.forEach: ', eavValues.values);
        eavValues.values.forEach(function (eavValue) {
            var allDimensions = eavValue.dimensions.map(function (d) { return d.value; }).join();
            newValue1[allDimensions] = eavValue.value;
        });
        return newValue1;
    };
    return Value1;
}());

/* "Attributes": {
    "String": {
        "Description": {
            "*": "Retrieve full list of all zones"
        },
        "Name": {
            "*": "Zones"
        },
        "StreamsOut": {
            "*": "ListContent,Default"
        },
        "StreamWiring": {
            "*": "3cef3168-5fe8-4417-8ee0-c47642181a1e:Default>Out:Default"
        },
        "TestParameters": {
            "*": "[Module:ModuleID]=6936"
        }
    },
    "Boolean": {
        "AllowEdit": {
            "*": true
        }
    }
}, */


/***/ }),

/***/ "./src/app/shared/pipes/file-ending-filter.pipe.ts":
/*!*********************************************************!*\
  !*** ./src/app/shared/pipes/file-ending-filter.pipe.ts ***!
  \*********************************************************/
/*! exports provided: FileEndingFilterPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FileEndingFilterPipe", function() { return FileEndingFilterPipe; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var FileEndingFilterPipe = /** @class */ (function () {
    function FileEndingFilterPipe() {
    }
    FileEndingFilterPipe.prototype.transform = function (items, allowedFileTypes) {
        if (!items) {
            return [];
        }
        if (allowedFileTypes.length === 0) {
            return items;
        }
        return items.filter(function (it) { return allowedFileTypes.indexOf(it.Name.match(/(?:\.([^.]+))?$/)[0]) !== -1; });
    };
    FileEndingFilterPipe = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Pipe"])({ name: 'fileEndingFilter' })
    ], FileEndingFilterPipe);
    return FileEndingFilterPipe;
}());



/***/ }),

/***/ "./src/app/shared/pipes/filter.pipe.ts":
/*!*********************************************!*\
  !*** ./src/app/shared/pipes/filter.pipe.ts ***!
  \*********************************************/
/*! exports provided: FilterPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FilterPipe", function() { return FilterPipe; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var FilterPipe = /** @class */ (function () {
    function FilterPipe() {
    }
    FilterPipe.prototype.transform = function (items, field, value, isEqual) {
        if (isEqual === void 0) { isEqual = true; }
        if (!items) {
            return [];
        }
        if (isEqual) {
            return items.filter(function (it) { return it[field] === value; });
        }
        else {
            return items.filter(function (it) { return it[field] !== value; });
        }
    };
    FilterPipe = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Pipe"])({ name: 'filter' })
    ], FilterPipe);
    return FilterPipe;
}());



/***/ }),

/***/ "./src/app/shared/pipes/orderby.pipe.ts":
/*!**********************************************!*\
  !*** ./src/app/shared/pipes/orderby.pipe.ts ***!
  \**********************************************/
/*! exports provided: OrderByPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "OrderByPipe", function() { return OrderByPipe; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var OrderByPipe = /** @class */ (function () {
    function OrderByPipe() {
    }
    OrderByPipe.prototype.transform = function (array, field) {
        array.sort(function (a, b) {
            if (a[field] < b[field]) {
                return -1;
            }
            else if (a[field] > b[field]) {
                return 1;
            }
            else {
                return 0;
            }
        });
        return array;
    };
    OrderByPipe = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Pipe"])({
            name: 'orderby'
        })
    ], OrderByPipe);
    return OrderByPipe;
}());



/***/ }),

/***/ "./src/app/shared/services/content-type.service.ts":
/*!*********************************************************!*\
  !*** ./src/app/shared/services/content-type.service.ts ***!
  \*********************************************************/
/*! exports provided: ContentTypeService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeService", function() { return ContentTypeService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _models_eav_content_type__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../models/eav/content-type */ "./src/app/shared/models/eav/content-type.ts");
/* harmony import */ var _store_actions_content_type_actions__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../store/actions/content-type.actions */ "./src/app/shared/store/actions/content-type.actions.ts");
/* harmony import */ var _store__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../store */ "./src/app/shared/store/index.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};








var ContentTypeService = /** @class */ (function () {
    // public contentTypes$: Observable<ContentType[]>;
    function ContentTypeService(httpClient, store) {
        this.httpClient = httpClient;
        this.store = store;
    }
    /**
     * Dispatch LoadItemsAction to store
     * @param path
     */
    ContentTypeService.prototype.loadContentType = function (path) {
        this.store.dispatch(new _store_actions_content_type_actions__WEBPACK_IMPORTED_MODULE_6__["LoadContentTypeAction"](path));
    };
    ContentTypeService.prototype.loadContentTypes = function (contentTypes) {
        var _this = this;
        contentTypes.forEach(function (jsonContentType1) {
            var contentType = _models_eav_content_type__WEBPACK_IMPORTED_MODULE_5__["ContentType"].create(jsonContentType1);
            _this.store.dispatch(new _store_actions_content_type_actions__WEBPACK_IMPORTED_MODULE_6__["LoadContentTypeSuccessAction"](contentType));
        });
    };
    /**
     * Observe content type for item type from store
     * @param id
     */
    ContentTypeService.prototype.getContentTypeById = function (id) {
        return this.store
            .select(_store__WEBPACK_IMPORTED_MODULE_7__["getContentTypes"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (data) { return data.find(function (obj) { return obj.contentType.id === id; }); }));
    };
    /**
     * Observe content type for item type from store
     * @param id
     */
    ContentTypeService.prototype.getTitleAttribute = function (id) {
        return this.store
            .select(_store__WEBPACK_IMPORTED_MODULE_7__["getContentTypes"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (data) { return data.find(function (obj) { return obj.contentType.id === id; }).contentType.attributes.find(function (obj) { return obj.isTitle === true; }); }));
    };
    /**
     * Get Content Type from Json Content Type V1
     */
    ContentTypeService.prototype.getContentTypeFromJsonContentType1 = function (path) {
        var _this = this;
        return this.httpClient.get("/DesktopModules/ToSIC_SexyContent/dist/ng-edit/assets/data/item-edit-form/content-type/" + path)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (item) {
            return _models_eav_content_type__WEBPACK_IMPORTED_MODULE_5__["ContentType"].create(item);
        }), 
        // tap(data => console.log('getEavEntityFromJsonItem1: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    /**
     * Get Json Content Type V1
     */
    ContentTypeService.prototype.getJsonContentType1 = function (path) {
        var _this = this;
        return this.httpClient.get("../../../assets/data/json-to-class-test/content-type/" + path)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (item) {
            return item;
        }), 
        // tap(data => console.log('getEavEntityFromJsonItem1: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    ContentTypeService.prototype.handleError = function (error) {
        // In a real world app, we might send the error to remote logging infrastructure
        var errMsg = error.message || 'Server error';
        console.error(errMsg);
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_3__["throwError"])(errMsg);
    };
    ContentTypeService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"], _ngrx_store__WEBPACK_IMPORTED_MODULE_2__["Store"]])
    ], ContentTypeService);
    return ContentTypeService;
}());



/***/ }),

/***/ "./src/app/shared/services/dnn-bridge.service.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/services/dnn-bridge.service.ts ***!
  \*******************************************************/
/*! exports provided: DnnBridgeService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DnnBridgeService", function() { return DnnBridgeService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _constants_url_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../constants/url-constants */ "./src/app/shared/constants/url-constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var DnnBridgeService = /** @class */ (function () {
    function DnnBridgeService(httpClient) {
        this.httpClient = httpClient;
    }
    DnnBridgeService.prototype.getUrlOfId = function (appId, idCode, contentType, guid, field) {
        var _this = this;
        var linkLowered = idCode.toLowerCase();
        if (linkLowered.indexOf('file:') !== -1 || linkLowered.indexOf('page:') !== -1) {
            return this.httpClient.get(_constants_url_constants__WEBPACK_IMPORTED_MODULE_4__["UrlConstants"].apiRoot + 'dnn/Hyperlink/ResolveHyperlink?hyperlink='
                + encodeURIComponent(idCode)
                + (guid ? '&guid=' + guid : '')
                + (contentType ? '&contentType=' + contentType : '')
                + (field ? '&field=' + field : '')
                + '&appId=' + appId)
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (data) {
                return data;
            }), 
            // tap(data => console.log('Hyperlink data: ', data)),
            Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["catchError"])(function (error) { return _this.handleError(error); }));
            // .do(data => console.log('features: ', data))
            // .catch(this.handleError);
        }
        else {
            return null;
        }
    };
    DnnBridgeService.prototype.handleError = function (error) {
        // In a real world app, we might send the error to remote logging infrastructure
        var errMsg = error.message || 'Server error';
        console.error(errMsg);
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["throwError"])(errMsg);
    };
    DnnBridgeService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"]])
    ], DnnBridgeService);
    return DnnBridgeService;
}());



/***/ }),

/***/ "./src/app/shared/services/eav-admin-ui.service.ts":
/*!*********************************************************!*\
  !*** ./src/app/shared/services/eav-admin-ui.service.ts ***!
  \*********************************************************/
/*! exports provided: EavAdminUiService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavAdminUiService", function() { return EavAdminUiService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _constants_type_constants__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../constants/type-constants */ "./src/app/shared/constants/type-constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var EavAdminUiService = /** @class */ (function () {
    function EavAdminUiService() {
        /**
         * Open a modal dialog containing the given component. With EntityId.
         */
        this.openItemEditWithEntityId = function (dialog, entityId, component) {
            return dialog.open(component, {
                width: '650px',
                data: {
                    id: entityId,
                    type: _constants_type_constants__WEBPACK_IMPORTED_MODULE_1__["DialogTypeConstants"].itemEditWithEntityId
                }
            });
        };
        /**
         * Open a modal dialog containing the given component.
         */
        this.openItemEditWithContent = function (dialog, component) {
            return dialog.open(component, {
                width: '650px',
                // height: '90%',
                // disableClose = true,
                // autoFocus = true,
                data: {
                    id: null,
                    type: _constants_type_constants__WEBPACK_IMPORTED_MODULE_1__["DialogTypeConstants"].itemEditWithContent
                }
            });
        };
    }
    EavAdminUiService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [])
    ], EavAdminUiService);
    return EavAdminUiService;
}());



/***/ }),

/***/ "./src/app/shared/services/eav.service.ts":
/*!************************************************!*\
  !*** ./src/app/shared/services/eav.service.ts ***!
  \************************************************/
/*! exports provided: EavService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EavService", function() { return EavService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/router */ "./node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _item_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./item.service */ "./src/app/shared/services/item.service.ts");
/* harmony import */ var _content_type_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./content-type.service */ "./src/app/shared/services/content-type.service.ts");
/* harmony import */ var _helpers_url_helper__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../helpers/url-helper */ "./src/app/shared/helpers/url-helper.ts");
/* harmony import */ var _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../store/actions/item.actions */ "./src/app/shared/store/actions/item.actions.ts");
/* harmony import */ var _constants_url_constants__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../constants/url-constants */ "./src/app/shared/constants/url-constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};











var EavService = /** @class */ (function () {
    function EavService(httpClient, store, itemService, contentTypeService, route) {
        var _this = this;
        this.httpClient = httpClient;
        this.store = store;
        this.itemService = itemService;
        this.contentTypeService = contentTypeService;
        this.route = route;
        // this formSetValueChangeSource observable is using in external components
        this.formSetValueChangeSource = new rxjs__WEBPACK_IMPORTED_MODULE_3__["Subject"]();
        this.formSetValueChange$ = this.formSetValueChangeSource.asObservable();
        // public getAllData() {
        //   this.store.dispatch(new dataActions.LoadAllDataAction());
        // }
        this.getEavConfiguration = function () {
            if (!_this.eavConfig) {
                _this.setEavConfiguration(_this.route);
            }
            if (_this.eavConfig) {
                return _this.eavConfig;
            }
            else {
                console.log('Configuration data not set');
            }
        };
    }
    EavService.prototype.loadAllDataForForm = function (appId, items) {
        var _this = this;
        var body = items.replace(/"/g, '\'');
        // const body = JSON.stringify([{ 'EntityId': 2809 }]);
        // const body = JSON.stringify([{ 'EntityId': 1033 }]);
        // const body = JSON.stringify([{ 'EntityId': 3861 }]);
        // const body = JSON.stringify([{ 'EntityId': 3858 }]);
        // const body = JSON.stringify([{ 'EntityId': 3841 }]);
        // const body = JSON.stringify([{ 'EntityId': 3830 }]);
        // const body = JSON.stringify([{ 'EntityId': 1754 }, { 'EntityId': 1785 }]); // , { 'EntityId': 3824 }
        // const body = JSON.stringify([{ 'EntityId': 1034 }, { 'EntityId': 1035 }]);
        // maybe create model for data
        return this.httpClient.post(_constants_url_constants__WEBPACK_IMPORTED_MODULE_10__["UrlConstants"].apiRoot + "eav/ui/load?appId=" + appId, body)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (data) {
            return data;
        }), 
        // tap(data => console.log('getAllDataForForm: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    // TODO: create entityarray type
    EavService.prototype.loadAllDataForFormByEntity = function (appId, entityArray) {
        var _this = this;
        var body = JSON.stringify(entityArray);
        // maybe create model for data
        return this.httpClient.post(_constants_url_constants__WEBPACK_IMPORTED_MODULE_10__["UrlConstants"].apiRoot + "eav/ui/load?appId=" + appId, body)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (data) {
            return data;
        }), 
        // tap(data => console.log('getAllDataForForm: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    EavService.prototype.saveItem = function (appId, item, updateValues, existingLanguageKey, defaultLanguage) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_9__["SaveItemAttributesValuesAction"](appId, item, updateValues, existingLanguageKey, defaultLanguage));
    };
    EavService.prototype.saveItemSuccess = function (data) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_9__["SaveItemAttributesValuesSuccessAction"](data));
    };
    EavService.prototype.saveItemError = function (error) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_9__["SaveItemAttributesValuesErrorAction"](error));
    };
    // TODO: Finish return model and sent real body
    // public savemany(appId: number, tabId: string, moduleId: string, contentBlockId: string, body: string): Observable<any> {
    EavService.prototype.savemany = function (appId, partOfPage, body) {
        var _this = this;
        console.log('start submit');
        // tslint:disable-next-line:max-line-length
        // const bodyTemp = `[{"Header":{"EntityId":1722,"Guid":"07621ab2-4bdc-4fd2-9c9d-e9cc765f988c","ContentTypeName":"67a0b738-f1d0-4773-899d-c5bb04cfce2b","Metadata":null,"Group":null,"Prefill":null,"Title":null,"DuplicateEntity":null},"Entity":{"Id":1722,"Type":{"Name":"DirectoryItem","StaticName":"67a0b738-f1d0-4773-899d-c5bb04cfce2b"},"IsPublished":true,"IsBranch":false,"TitleAttributeName":"Title","Attributes":{"Title":{"Values":[{"Value":"2sic internet solutions","Dimensions":{"en-us":false}}]},"Industry":{"Values":[{"Value":["9e733bf4-8179-4add-a333-6cb6dbff38dc"],"Dimensions":{}}]},"Link":{"Values":[{"Value":"https://www.2sic.com","Dimensions":{"en-us":false}}]},"Logo":{"Values":[{"Value":"file:216","Dimensions":{"en-us":false}}]},"LinkText":{"Values":[{"Value":"www.2sic.com","Dimensions":{"en-us":false}}]},"Town":{"Values":[{"Value":"Buchs","Dimensions":{"en-us":false}}]},"localizationMenus":[{"all":{}},{"all":{}},{"all":{}},{"all":{}},{"all":{}}]},"AppId":15},"slotIsUsed":true}]`;
        //  const body = items;
        // const partOfPage = false;
        // TODO: create model for data
        return this.httpClient.post(_constants_url_constants__WEBPACK_IMPORTED_MODULE_10__["UrlConstants"].apiRoot + "eav/ui/save?appId=" + appId + "&partOfPage=" + partOfPage, body)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (data) {
            console.log('return data');
            return data;
        }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["tap"])(function (data) { return console.log('submit: ', data); }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    /**
    * Trigger on form change - this is using in external components
    */
    EavService.prototype.triggerFormSetValueChange = function (formValues) {
        this.formSetValueChangeSource.next(formValues);
    };
    /**
   * Set Eav Configuration
   */
    EavService.prototype.setEavConfiguration = function (route) {
        var queryStringParameters = _helpers_url_helper__WEBPACK_IMPORTED_MODULE_8__["UrlHelper"].readQueryStringParameters(route.snapshot.fragment);
        console.log('queryStringParameters', queryStringParameters);
        // const eavConfiguration: EavConfiguration = UrlHelper.getEavConfiguration(queryStringParameters);
        this.eavConfig = _helpers_url_helper__WEBPACK_IMPORTED_MODULE_8__["UrlHelper"].getEavConfiguration(queryStringParameters);
    };
    EavService.prototype.handleError = function (error) {
        // In a real world app, we might send the error to remote logging infrastructure
        var errMsg = error.message || 'Server error';
        console.error(errMsg);
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_3__["throwError"])(errMsg);
    };
    EavService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"],
            _ngrx_store__WEBPACK_IMPORTED_MODULE_2__["Store"],
            _item_service__WEBPACK_IMPORTED_MODULE_6__["ItemService"],
            _content_type_service__WEBPACK_IMPORTED_MODULE_7__["ContentTypeService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_5__["ActivatedRoute"]])
    ], EavService);
    return EavService;
}());



/***/ }),

/***/ "./src/app/shared/services/entity.service.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/services/entity.service.ts ***!
  \***************************************************/
/*! exports provided: EntityService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EntityService", function() { return EntityService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _constants_url_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../constants/url-constants */ "./src/app/shared/constants/url-constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var EntityService = /** @class */ (function () {
    function EntityService(httpClient) {
        this.httpClient = httpClient;
    }
    /**
     * get availableEntities - (used in entity-default input type)
     * @param apiId
     * @param body
     * @param ctName
     */
    EntityService.prototype.getAvailableEntities = function (apiId, body, ctName) {
        var _this = this;
        // maybe create model for data
        return this.httpClient.post(_constants_url_constants__WEBPACK_IMPORTED_MODULE_4__["UrlConstants"].apiRoot + "eav/EntityPicker/getavailableentities", body, {
            params: {
                contentTypeName: ctName,
                appId: apiId
            }
        }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (data) {
            return data;
        }), 
        // tap(data => console.log('getAvailableEntities: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    EntityService.prototype.delete = function (appId, type, id, tryForce) {
        console.log('GET delete method:');
        return this.httpClient.get(_constants_url_constants__WEBPACK_IMPORTED_MODULE_4__["UrlConstants"].apiRoot + "eav/entities/delete", {
            // ignoreErrors: 'true',
            params: {
                'contentType': type,
                'id': id,
                'appId': appId,
                'force': tryForce.toString()
            }
        })
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (data) {
            console.log('data retun', data);
            return data;
        }), 
        // tap(data => console.log('entity delete: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["catchError"])(function (error) { return Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["of"])(error); }));
        // return null;
    };
    EntityService.prototype.handleError = function (error) {
        // In a real world app, we might send the error to remote logging infrastructure
        var errMsg = error.message || 'Server error';
        console.error(errMsg);
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["throwError"])(errMsg);
    };
    EntityService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"]])
    ], EntityService);
    return EntityService;
}());



/***/ }),

/***/ "./src/app/shared/services/feature.service.ts":
/*!****************************************************!*\
  !*** ./src/app/shared/services/feature.service.ts ***!
  \****************************************************/
/*! exports provided: FeatureService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FeatureService", function() { return FeatureService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _constants_url_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../constants/url-constants */ "./src/app/shared/constants/url-constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var FeatureService = /** @class */ (function () {
    function FeatureService(httpClient) {
        var _this = this;
        this.httpClient = httpClient;
        this.enabled = function (guid, appId, url) {
            return _this.getFeatures(appId, url)
                .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["switchMap"])(function (s) { return _this.enabledNow(s, guid); }));
        };
        this.enabledNow = function (list, guid) {
            for (var i = 0; i < list.length; i++) {
                if (list[i].id === guid) {
                    return Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["of"])(list[i].enabled);
                }
            }
            return Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["of"])(false);
        };
    }
    FeatureService.prototype.getFeatures = function (appId, url) {
        var _this = this;
        return this.httpClient.get(_constants_url_constants__WEBPACK_IMPORTED_MODULE_4__["UrlConstants"].apiRoot + "eav/system/features", {
            params: {
                appId: appId
            }
        })
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (data) {
            return data;
        }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["tap"])(function (data) { return console.log('features: ', data); }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    FeatureService.prototype.handleError = function (error) {
        // In a real world app, we might send the error to remote logging infrastructure
        var errMsg = error.message || 'Server error';
        console.error(errMsg);
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["throwError"])(errMsg);
    };
    FeatureService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"]])
    ], FeatureService);
    return FeatureService;
}());



/***/ }),

/***/ "./src/app/shared/services/field-mask.service.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/services/field-mask.service.ts ***!
  \*******************************************************/
/*! exports provided: FieldMaskService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FieldMaskService", function() { return FieldMaskService; });
// @Injectable()
var FieldMaskService = /** @class */ (function () {
    function FieldMaskService(mask, changeEvent, overloadPreCleanValues, model) {
        var _this = this;
        this.mask = mask;
        this.changeEvent = changeEvent;
        this.overloadPreCleanValues = overloadPreCleanValues;
        this.model = model;
        // private mask: string;
        // model: $scope.model,
        this.fields = [];
        // value: undefined,
        this.findFields = /\[.*?\]/ig;
        this.unwrapField = /[\[\]]/ig;
        this.preClean = function (key, value) {
            return value;
        };
        // retrieves a list of all fields used in the mask
        this.fieldList = function () {
            var result = [];
            if (!_this.mask) {
                return result;
            }
            var matches = _this.mask.match(_this.findFields);
            if (matches) {
                matches.forEach(function (e, i) {
                    var staticName = e.replace(_this.unwrapField, '');
                    result.push(staticName);
                });
            }
            else { // TODO: ask is this good
                result.push(_this.mask);
            }
            return result;
        };
        // resolves a mask to the final value
        // getNewAutoValue()
        // this.model = this.group.controls (group is FormGroup)
        this.resolve = function () {
            var value = _this.mask;
            _this.fields.forEach(function (e, i) {
                var replaceValue = _this.model.hasOwnProperty(e) && _this.model[e] && _this.model[e].value ? _this.model[e].value : '';
                var cleaned = _this.preClean(e, replaceValue);
                value = value.replace('[' + e + ']', cleaned);
            });
            console.log('resolve:', value);
            return value;
        };
        this.mask = mask;
        this.model = model;
        this.fields = this.fieldList();
        if (overloadPreCleanValues) { // got an overload...
            this.preClean = overloadPreCleanValues;
        }
    }
    return FieldMaskService;
}());



/***/ }),

/***/ "./src/app/shared/services/file-type.service.ts":
/*!******************************************************!*\
  !*** ./src/app/shared/services/file-type.service.ts ***!
  \******************************************************/
/*! exports provided: FileTypeService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FileTypeService", function() { return FileTypeService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var FileTypeService = /** @class */ (function () {
    function FileTypeService() {
        var _this = this;
        this.iconPrefix = 'eav-icon-';
        this.defaultIcon = 'file';
        this.checkImgRegEx = /(?:([^:\/?#]+):)?(?:\/\/([^\/?#]*))?([^?#]*\.(?:jpg|jpeg|gif|png))(?:\?([^#]*))?(?:#(.*))?/i;
        this.extensions = {
            doc: 'file-word',
            docx: 'file-word',
            xls: 'file-excel',
            xlsx: 'file-excel',
            ppt: 'file-powerpoint',
            pptx: 'file-powerpoint',
            pdf: 'file-pdf',
            mp3: 'file-audio',
            avi: 'file-video',
            mpg: 'file-video',
            mpeg: 'file-video',
            mov: 'file-video',
            mp4: 'file-video',
            zip: 'file-archive',
            rar: 'file-archive',
            txt: 'file-text',
            html: 'file-code',
            css: 'file-code',
            xml: 'file-code',
            xsl: 'file-code',
            vcf: 'user'
        };
        this.getExtension = function (filename) {
            return filename.substr(filename.lastIndexOf('.') + 1).toLowerCase();
        };
        this.getIconClass = function (filename) {
            return _this.iconPrefix + (_this.extensions[_this.getExtension(filename)] || _this.defaultIcon);
        };
        this.isKnownType = function (filename) {
            return _this.extensions[_this.getExtension(filename)] !== undefined;
        };
        this.isImage = function (filename) {
            return _this.checkImgRegEx.test(filename);
        };
    }
    FileTypeService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [])
    ], FileTypeService);
    return FileTypeService;
}());



/***/ }),

/***/ "./src/app/shared/services/input-type.service.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/services/input-type.service.ts ***!
  \*******************************************************/
/*! exports provided: InputTypeService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InputTypeService", function() { return InputTypeService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _store_actions_input_type_actions__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../store/actions/input-type.actions */ "./src/app/shared/store/actions/input-type.actions.ts");
/* harmony import */ var _store__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../store */ "./src/app/shared/store/index.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var InputTypeService = /** @class */ (function () {
    function InputTypeService(store) {
        this.store = store;
    }
    /**
      * Load all inputtypes info
      */
    InputTypeService.prototype.loadInputTypes = function (inputTypes) {
        this.store.dispatch(new _store_actions_input_type_actions__WEBPACK_IMPORTED_MODULE_3__["LoadInputTypeSuccessAction"](inputTypes));
    };
    /**
    * Observe input type from store
    * @param type
    */
    InputTypeService.prototype.getContentTypeById = function (type) {
        return this.store
            .select(_store__WEBPACK_IMPORTED_MODULE_4__["getInputTypes"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_2__["map"])(function (data) { return data.find(function (obj) { return obj.Type === type; }); }));
    };
    InputTypeService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_ngrx_store__WEBPACK_IMPORTED_MODULE_1__["Store"]])
    ], InputTypeService);
    return InputTypeService;
}());



/***/ }),

/***/ "./src/app/shared/services/item.service.ts":
/*!*************************************************!*\
  !*** ./src/app/shared/services/item.service.ts ***!
  \*************************************************/
/*! exports provided: ItemService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ItemService", function() { return ItemService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _models_eav_item__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../models/eav/item */ "./src/app/shared/models/eav/item.ts");
/* harmony import */ var _models_eav__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../models/eav */ "./src/app/shared/models/eav/index.ts");
/* harmony import */ var _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../store/actions/item.actions */ "./src/app/shared/store/actions/item.actions.ts");
/* harmony import */ var _store__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../store */ "./src/app/shared/store/index.ts");
/* harmony import */ var _models_eav_eav_dimensions__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../models/eav/eav-dimensions */ "./src/app/shared/models/eav/eav-dimensions.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};










var ItemService = /** @class */ (function () {
    // public items$  Observable<Item[]>;
    function ItemService(httpClient, store) {
        this.httpClient = httpClient;
        this.store = store;
        // this.items$ = store.select(fromStore.getItems);
    }
    // public loadAllData(path: string) {
    //   this.store.dispatch(new itemActions.LoadDataAction(path));
    // }
    ItemService.prototype.loadItem = function (path) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["LoadItemAction"](path));
    };
    ItemService.prototype.loadItems = function (items) {
        var _this = this;
        console.log('start create item');
        items.forEach(function (jsonItem1) {
            var item = _models_eav_item__WEBPACK_IMPORTED_MODULE_5__["Item"].create(jsonItem1);
            _this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["LoadItemSuccessAction"](item));
        });
    };
    ItemService.prototype.updateItem = function (attributes, id) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["UpdateItemAction"](attributes, id));
    };
    ItemService.prototype.updateItemAttribute = function (entityId, newEavAttribute, attributeKey) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["UpdateItemAttributeAction"](entityId, newEavAttribute, attributeKey));
    };
    ItemService.prototype.addItemAttributeValue = function (entityId, newEavAttributeValue, attributeKey) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["AddItemAttributeValueAction"](entityId, newEavAttributeValue, attributeKey));
    };
    ItemService.prototype.updateItemAttributeValue = function (entityId, attributeKey, newEavAttributeValue, existingDimensionValue, defaultLanguage, isReadOnly) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["UpdateItemAttributeValueAction"](entityId, attributeKey, newEavAttributeValue, existingDimensionValue, defaultLanguage, isReadOnly));
    };
    ItemService.prototype.updateItemAttributesValues = function (entityId, updateValues, languageKey, defaultLanguage) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["UpdateItemAttributesValuesAction"](entityId, updateValues, languageKey, defaultLanguage));
    };
    /**
    * Update entity attribute dimension. Add readonly languageKey to existing useFromLanguageKey.
    * Example to useFrom en-us add fr-fr = "en-us,-fr-fr"
    * */
    ItemService.prototype.addItemAttributeDimension = function (entityId, attributeKey, dimensionValue, existingDimensionValue, defaultLanguage, isReadOnly) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["AddItemAttributeDimensionAction"](entityId, attributeKey, dimensionValue, existingDimensionValue, defaultLanguage, isReadOnly));
    };
    ItemService.prototype.removeItemAttributeDimension = function (entityId, attributeKey, dimensionValue) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["RemoveItemAttributeDimensionAction"](entityId, attributeKey, dimensionValue));
    };
    // public updateItem(attributes: EavAttributes, item: EavItem) {
    //   this.store.dispatch(new itemActions.UpdateItemAction(attributes, item));
    // }
    ItemService.prototype.addAttributeValue = function (entityId, attributeKey, oldAttributeValues, newValue, languageKey, isReadOnly) {
        var newLanguageValue = languageKey;
        if (isReadOnly) {
            newLanguageValue = "~" + languageKey;
        }
        var newEavValue = new _models_eav__WEBPACK_IMPORTED_MODULE_6__["EavValue"](newValue, [new _models_eav_eav_dimensions__WEBPACK_IMPORTED_MODULE_9__["EavDimensions"](newLanguageValue)]);
        this.addItemAttributeValue(entityId, newEavValue, attributeKey);
    };
    ItemService.prototype.deleteItem = function (item) {
        this.store.dispatch(new _store_actions_item_actions__WEBPACK_IMPORTED_MODULE_7__["DeleteItemAction"](item));
    };
    ItemService.prototype.selectAttributeByEntityId = function (entityId, attributeKey) {
        return this.store
            .select(_store__WEBPACK_IMPORTED_MODULE_8__["getItems"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (c) { return c.find(function (obj) { return obj.entity.id === entityId; })
            ? c.find(function (obj) { return obj.entity.id === entityId; }).entity.attributes[attributeKey]
            : null; }));
    };
    ItemService.prototype.selectAttributesByEntityId = function (entityId) {
        return this.store
            .select(_store__WEBPACK_IMPORTED_MODULE_8__["getItems"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (c) { return c.find(function (obj) { return obj.entity.id === entityId; })
            ? c.find(function (obj) { return obj.entity.id === entityId; }).entity.attributes
            : null; }));
    };
    ItemService.prototype.selectAllItems = function () {
        return this.store.select(_store__WEBPACK_IMPORTED_MODULE_8__["getItems"]);
    };
    ItemService.prototype.selectItemById = function (id) {
        return this.store
            .select(_store__WEBPACK_IMPORTED_MODULE_8__["getItems"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (data) { return data.find(function (obj) { return obj.entity.id === id; }); }));
    };
    /**
     * Select items from store by id array list
     * @param idsList
     */
    ItemService.prototype.selectItemsByIdList = function (idsList) {
        return this.store
            .select(_store__WEBPACK_IMPORTED_MODULE_8__["getItems"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (data) { return data.filter(function (obj) { return idsList.filter(function (id) { return id === obj.entity.id; }).length > 0; }); }));
    };
    // public selectItemById(id: number): Observable<Item> {
    //   return this.store.select(fromStore.getItemById(id));
    // }
    /**
     * Get Item from Json Entity V1
     */
    ItemService.prototype.getItemFromJsonItem1 = function (path) {
        var _this = this;
        // return this.httpClient.get<JsonItem1>('../../../assets/data/item-edit-form/item/json-item-v1-person.json')
        // return this.httpClient.get<JsonItem1>(`../../../assets/data/item-edit-form/item/json-item-v1-accordion.json`)
        return this.httpClient.get("/DesktopModules/ToSIC_SexyContent/dist/ng-edit/assets/data/item-edit-form/item/" + path)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (item) {
            return _models_eav_item__WEBPACK_IMPORTED_MODULE_5__["Item"].create(item);
        }), 
        // tap(data => console.log('getItemFromJsonItem1: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    /**
     * Get Json Entity V1
     */
    ItemService.prototype.getJsonItem1 = function (path) {
        var _this = this;
        return this.httpClient.get("../../../assets/data/json-to-class-test/item/" + path)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["map"])(function (item) {
            return item;
        }), 
        // tap(data => console.log('getItemFromJsonItem1: ', data)),
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["catchError"])(function (error) { return _this.handleError(error); }));
    };
    ItemService.prototype.handleError = function (error) {
        // In a real world app, we might send the error to remote logging infrastructure
        var errMsg = error.message || 'Server error';
        console.error(errMsg);
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_3__["throwError"])(errMsg);
    };
    ItemService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"], _ngrx_store__WEBPACK_IMPORTED_MODULE_2__["Store"]])
    ], ItemService);
    return ItemService;
}());



/***/ }),

/***/ "./src/app/shared/services/language.service.ts":
/*!*****************************************************!*\
  !*** ./src/app/shared/services/language.service.ts ***!
  \*****************************************************/
/*! exports provided: LanguageService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LanguageService", function() { return LanguageService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var _store_actions_language_actions__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../store/actions/language.actions */ "./src/app/shared/store/actions/language.actions.ts");
/* harmony import */ var _store__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../store */ "./src/app/shared/store/index.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};







var LanguageService = /** @class */ (function () {
    function LanguageService(httpClient, store) {
        this.httpClient = httpClient;
        this.store = store;
        this.localizationWrapperMenuChangeSource = new rxjs__WEBPACK_IMPORTED_MODULE_2__["Subject"]();
        this.localizationWrapperMenuChange$ = this.localizationWrapperMenuChangeSource.asObservable();
        // this.items$ = store.select(fromStore.getItems);
    }
    /**
     * Load all languages
     */
    LanguageService.prototype.loadLanguages = function (languages, currentLanguage, defaultLanguage, uiLanguage) {
        this.store.dispatch(new _store_actions_language_actions__WEBPACK_IMPORTED_MODULE_5__["LoadLanguagesAction"](languages, currentLanguage, defaultLanguage, uiLanguage));
    };
    LanguageService.prototype.selectAllLanguages = function () {
        return this.store.select(_store__WEBPACK_IMPORTED_MODULE_6__["getLanguages"]);
    };
    LanguageService.prototype.selectLanguage = function (name) {
        return this.store.select(_store__WEBPACK_IMPORTED_MODULE_6__["getLanguages"])
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (data) { return data.find(function (obj) { return obj.name === name; }); }));
    };
    LanguageService.prototype.getCurrentLanguage = function () {
        return this.store.select(_store__WEBPACK_IMPORTED_MODULE_6__["getCurrentLanguage"]);
    };
    LanguageService.prototype.getDefaultLanguage = function () {
        return this.store.select(_store__WEBPACK_IMPORTED_MODULE_6__["getDefaultLanguage"]);
    };
    LanguageService.prototype.updateCurrentLanguage = function (currentLanguage) {
        this.store.dispatch(new _store_actions_language_actions__WEBPACK_IMPORTED_MODULE_5__["UpdateCurrentLanguageAction"](currentLanguage));
    };
    LanguageService.prototype.updateDefaultLanguage = function (defaultLanguage) {
        this.store.dispatch(new _store_actions_language_actions__WEBPACK_IMPORTED_MODULE_5__["UpdateDefaultLanguageAction"](defaultLanguage));
    };
    /**
     * Trigger info message change on all form controls
     * @param infoMessage
     */
    LanguageService.prototype.triggerLocalizationWrapperMenuChange = function () {
        this.localizationWrapperMenuChangeSource.next();
    };
    LanguageService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"], _ngrx_store__WEBPACK_IMPORTED_MODULE_4__["Store"]])
    ], LanguageService);
    return LanguageService;
}());



/***/ }),

/***/ "./src/app/shared/services/script.service.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/services/script.service.ts ***!
  \***************************************************/
/*! exports provided: ScriptLoaderService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ScriptLoaderService", function() { return ScriptLoaderService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
/* harmony import */ var _constants_type_constants__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../constants/type-constants */ "./src/app/shared/constants/type-constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};



var ScriptLoaderService = /** @class */ (function () {
    function ScriptLoaderService() {
        this.scripts = [];
    }
    ScriptLoaderService.prototype.load = function (script, fileType) {
        var _this = this;
        return new rxjs__WEBPACK_IMPORTED_MODULE_1__["Observable"](function (observer) {
            var existingScript = _this.scripts.find(function (s) { return s.name === script.name; });
            // Complete if already loaded
            if (existingScript && existingScript.loaded) {
                observer.next(existingScript);
                observer.complete();
            }
            else {
                // Add the script
                _this.scripts = _this.scripts.concat([script]);
                // Load the script
                var scriptElement = void 0;
                switch (fileType) {
                    case _constants_type_constants__WEBPACK_IMPORTED_MODULE_2__["FileTypeConstants"].css:
                        // Load the style
                        scriptElement = document.createElement('link');
                        scriptElement.rel = 'stylesheet';
                        scriptElement.href = script.filePath;
                        break;
                    case _constants_type_constants__WEBPACK_IMPORTED_MODULE_2__["FileTypeConstants"].javaScript:
                        scriptElement = document.createElement('script');
                        scriptElement.type = 'module';
                        scriptElement.src = script.filePath;
                        break;
                    default:
                        console.log('wrong file type');
                        break;
                }
                scriptElement.onload = function () {
                    script.loaded = true;
                    // Settimeout for testing slow load of scripts
                    // setTimeout(() => {
                    observer.next(script);
                    observer.complete();
                    // }, 5000);
                };
                scriptElement.onerror = function (error) {
                    observer.error('Couldnt load script ' + script.filePath);
                };
                document.getElementsByTagName('head')[0].appendChild(scriptElement);
            }
        });
    };
    ScriptLoaderService.prototype.loadList = function (scriptList, fileType) {
        var _this = this;
        var allScripts$ = [];
        scriptList.forEach(function (scriptModel) {
            allScripts$.push(_this.load(scriptModel, fileType));
        });
        return allScripts$.length > 0
            ? rxjs__WEBPACK_IMPORTED_MODULE_1__["zip"].apply(void 0, allScripts$) : null;
    };
    ScriptLoaderService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])()
    ], ScriptLoaderService);
    return ScriptLoaderService;
}());



/***/ }),

/***/ "./src/app/shared/services/svc-creator.service.ts":
/*!********************************************************!*\
  !*** ./src/app/shared/services/svc-creator.service.ts ***!
  \********************************************************/
/*! exports provided: SvcCreatorService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SvcCreatorService", function() { return SvcCreatorService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm5/index.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var SvcCreatorService = /** @class */ (function () {
    // construct a object which has liveListCache, liveListReload(), liveListReset(),
    function SvcCreatorService() {
    }
    SvcCreatorService.prototype.implementLiveList = function (getLiveList$, disableToastr) {
        var disableToastrValue = !!disableToastr;
        var liveListCacheIsLoaded = false;
        var liveListSourceRead$ = getLiveList$;
        var liveListCacheBehaviorSubject = new rxjs__WEBPACK_IMPORTED_MODULE_1__["BehaviorSubject"]([]);
        var liveListCache$ = liveListCacheBehaviorSubject.asObservable();
        // use a promise-result to re-fill the live list of all items, return the promise again
        // const _liveListUpdateWithResult = function
        var updateLiveAll = function (result) {
            // TODO:
            // if (t.msg.isOpened) {
            //   toastr.clear(t.msg);
            // }
            // else {
            //   $timeout(300).then(function () {
            //     toastr.clear(t.msg);
            //   }
            //   );
            // }
            // liveListCache.length = 0; // clear
            // liveListCache = [];
            // for (let i = 0; i < result.length; i++) {
            //   liveListCache.push(result[i]);
            // }
            liveListCacheBehaviorSubject.next(result);
            liveListCacheIsLoaded = true;
            console.log('liveListCache after:', liveListCacheBehaviorSubject.getValue());
        };
        /**
         * Reload live list action
         */
        var liveListReload = function () {
            // show loading - must use the promise-mode because this may be used early before the language has arrived
            // return 'General.Messages.Loading';
            // $translate("General.Messages.Loading").then(function (msg) {
            //   t.msg = toastr.info(msg);
            // });
            liveListSourceRead$().subscribe(function (s) { return updateLiveAll(s); });
        };
        /**
         * Load live list action
         */
        var liveListLoad = function () {
            if (liveListCacheBehaviorSubject.getValue() && !liveListCacheIsLoaded) {
                liveListReload();
            }
        };
        /**
         * Clear list
         */
        var liveListReset = function () {
            // liveListCache = [];
            liveListCacheBehaviorSubject.next([]);
        };
        var svc = {
            disableToastrValue: disableToastrValue,
            liveListCache$: liveListCache$,
            liveListCacheIsLoaded: liveListCacheIsLoaded,
            liveListSourceRead$: liveListSourceRead$,
            liveListLoad: liveListLoad,
            // getAllLive,
            liveListReload: liveListReload,
            liveListReset: liveListReset,
            updateLiveAll: updateLiveAll
        };
        return svc;
    };
    SvcCreatorService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])(),
        __metadata("design:paramtypes", [])
    ], SvcCreatorService);
    return SvcCreatorService;
}());



/***/ }),

/***/ "./src/app/shared/store/actions/content-type.actions.ts":
/*!**************************************************************!*\
  !*** ./src/app/shared/store/actions/content-type.actions.ts ***!
  \**************************************************************/
/*! exports provided: LOAD_CONTENT_TYPE, LOAD_CONTENT_TYPE_SUCCESS, LoadContentTypeAction, LoadContentTypeSuccessAction */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LOAD_CONTENT_TYPE", function() { return LOAD_CONTENT_TYPE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LOAD_CONTENT_TYPE_SUCCESS", function() { return LOAD_CONTENT_TYPE_SUCCESS; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoadContentTypeAction", function() { return LoadContentTypeAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoadContentTypeSuccessAction", function() { return LoadContentTypeSuccessAction; });
var LOAD_CONTENT_TYPE = 'LOAD_CONTENT_TYPE';
var LOAD_CONTENT_TYPE_SUCCESS = 'LOAD_CONTENT_TYPE_SUCCESS';
var LoadContentTypeAction = /** @class */ (function () {
    function LoadContentTypeAction(path) {
        this.path = path;
        this.type = LOAD_CONTENT_TYPE;
    }
    return LoadContentTypeAction;
}());

var LoadContentTypeSuccessAction = /** @class */ (function () {
    function LoadContentTypeSuccessAction(newContentType) {
        this.newContentType = newContentType;
        this.type = LOAD_CONTENT_TYPE_SUCCESS;
    }
    return LoadContentTypeSuccessAction;
}());



/***/ }),

/***/ "./src/app/shared/store/actions/input-type.actions.ts":
/*!************************************************************!*\
  !*** ./src/app/shared/store/actions/input-type.actions.ts ***!
  \************************************************************/
/*! exports provided: LOAD_INPUT_TYPE_SUCCESS, LoadInputTypeSuccessAction */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LOAD_INPUT_TYPE_SUCCESS", function() { return LOAD_INPUT_TYPE_SUCCESS; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoadInputTypeSuccessAction", function() { return LoadInputTypeSuccessAction; });
// export const LOAD_INPUT_TYPE = 'LOAD_INPUT_TYPE';
var LOAD_INPUT_TYPE_SUCCESS = 'LOAD_INPUT_TYPE_SUCCESS';
var LoadInputTypeSuccessAction = /** @class */ (function () {
    function LoadInputTypeSuccessAction(newInputTypes) {
        this.newInputTypes = newInputTypes;
        this.type = LOAD_INPUT_TYPE_SUCCESS;
    }
    return LoadInputTypeSuccessAction;
}());



/***/ }),

/***/ "./src/app/shared/store/actions/item.actions.ts":
/*!******************************************************!*\
  !*** ./src/app/shared/store/actions/item.actions.ts ***!
  \******************************************************/
/*! exports provided: LOAD_ITEM, LOAD_ITEM_SUCCESS, UPDATE_ITEM, UPDATE_ITEM_SUCCESS, ADD_ITEM_ATTRIBUTE, UPDATE_ITEM_ATTRIBUTE, ADD_ITEM_ATTRIBUTE_VALUE, UPDATE_ITEM_ATTRIBUTE_VALUE, UPDATE_ITEM_ATTRIBUTES_VALUES, SAVE_ITEM_ATTRIBUTES_VALUES, SAVE_ITEM_ATTRIBUTES_VALUES_SUCCESS, SAVE_ITEM_ATTRIBUTES_VALUES_ERROR, ADD_ITEM_ATTRIBUTE_DIMENSION, UPDATE_ITEM_ATTRIBUTE_DIMENSION, REMOVE_ITEM_ATTRIBUTE_DIMENSION, DELETE_ITEM, LoadItemAction, LoadItemSuccessAction, AddItemAttributeAction, AddItemAttributeValueAction, AddItemAttributeDimensionAction, UpdateItemAction, UpdateItemSuccessAction, UpdateItemAttributeAction, UpdateItemAttributeValueAction, UpdateItemAttributesValuesAction, SaveItemAttributesValuesAction, SaveItemAttributesValuesSuccessAction, SaveItemAttributesValuesErrorAction, RemoveItemAttributeDimensionAction, DeleteItemAction */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LOAD_ITEM", function() { return LOAD_ITEM; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LOAD_ITEM_SUCCESS", function() { return LOAD_ITEM_SUCCESS; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_ITEM", function() { return UPDATE_ITEM; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_ITEM_SUCCESS", function() { return UPDATE_ITEM_SUCCESS; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ADD_ITEM_ATTRIBUTE", function() { return ADD_ITEM_ATTRIBUTE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_ITEM_ATTRIBUTE", function() { return UPDATE_ITEM_ATTRIBUTE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ADD_ITEM_ATTRIBUTE_VALUE", function() { return ADD_ITEM_ATTRIBUTE_VALUE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_ITEM_ATTRIBUTE_VALUE", function() { return UPDATE_ITEM_ATTRIBUTE_VALUE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_ITEM_ATTRIBUTES_VALUES", function() { return UPDATE_ITEM_ATTRIBUTES_VALUES; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SAVE_ITEM_ATTRIBUTES_VALUES", function() { return SAVE_ITEM_ATTRIBUTES_VALUES; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SAVE_ITEM_ATTRIBUTES_VALUES_SUCCESS", function() { return SAVE_ITEM_ATTRIBUTES_VALUES_SUCCESS; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SAVE_ITEM_ATTRIBUTES_VALUES_ERROR", function() { return SAVE_ITEM_ATTRIBUTES_VALUES_ERROR; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ADD_ITEM_ATTRIBUTE_DIMENSION", function() { return ADD_ITEM_ATTRIBUTE_DIMENSION; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_ITEM_ATTRIBUTE_DIMENSION", function() { return UPDATE_ITEM_ATTRIBUTE_DIMENSION; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "REMOVE_ITEM_ATTRIBUTE_DIMENSION", function() { return REMOVE_ITEM_ATTRIBUTE_DIMENSION; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DELETE_ITEM", function() { return DELETE_ITEM; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoadItemAction", function() { return LoadItemAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoadItemSuccessAction", function() { return LoadItemSuccessAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AddItemAttributeAction", function() { return AddItemAttributeAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AddItemAttributeValueAction", function() { return AddItemAttributeValueAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AddItemAttributeDimensionAction", function() { return AddItemAttributeDimensionAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateItemAction", function() { return UpdateItemAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateItemSuccessAction", function() { return UpdateItemSuccessAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateItemAttributeAction", function() { return UpdateItemAttributeAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateItemAttributeValueAction", function() { return UpdateItemAttributeValueAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateItemAttributesValuesAction", function() { return UpdateItemAttributesValuesAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SaveItemAttributesValuesAction", function() { return SaveItemAttributesValuesAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SaveItemAttributesValuesSuccessAction", function() { return SaveItemAttributesValuesSuccessAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SaveItemAttributesValuesErrorAction", function() { return SaveItemAttributesValuesErrorAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "RemoveItemAttributeDimensionAction", function() { return RemoveItemAttributeDimensionAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DeleteItemAction", function() { return DeleteItemAction; });
var LOAD_ITEM = '[Item] LOAD_ITEM';
var LOAD_ITEM_SUCCESS = '[Item] LOAD_ITEM_SUCCESS';
var UPDATE_ITEM = '[Item] UPDATE_ITEM';
var UPDATE_ITEM_SUCCESS = '[Item] UPDATE_ITEM_SUCCESS';
var ADD_ITEM_ATTRIBUTE = '[Item] ADD_ITEM_ATTRIBUTE';
var UPDATE_ITEM_ATTRIBUTE = '[Item] UPDATE_ITEM_ATTRIBUTE';
var ADD_ITEM_ATTRIBUTE_VALUE = '[Item] ADD_ITEM_ATTRIBUTE_VALUE';
var UPDATE_ITEM_ATTRIBUTE_VALUE = '[Item] UPDATE_ITEM_ATTRIBUTE_VALUE';
var UPDATE_ITEM_ATTRIBUTES_VALUES = '[Item] UPDATE_ITEM_ATTRIBUTES_VALUES';
var SAVE_ITEM_ATTRIBUTES_VALUES = '[Item] SAVE_ITEM_ATTRIBUTES_VALUES';
var SAVE_ITEM_ATTRIBUTES_VALUES_SUCCESS = '[Item] SAVE_ITEM_ATTRIBUTES_VALUES_SUCCESS';
var SAVE_ITEM_ATTRIBUTES_VALUES_ERROR = '[Item] SAVE_ITEM_ATTRIBUTES_VALUES_ERROR';
var ADD_ITEM_ATTRIBUTE_DIMENSION = '[Item] ADD_ITEM_ATTRIBUTE_DIMENSION';
var UPDATE_ITEM_ATTRIBUTE_DIMENSION = '[Item] UPDATE_ITEM_ATTRIBUTE_DIMENSION';
var REMOVE_ITEM_ATTRIBUTE_DIMENSION = '[Item] REMOVE_ITEM_ATTRIBUTE_DIMENSION';
var DELETE_ITEM = '[Item] DELETE_ITEM';
/**
 * Load
 */
var LoadItemAction = /** @class */ (function () {
    function LoadItemAction(path) {
        this.path = path;
        this.type = LOAD_ITEM;
    }
    return LoadItemAction;
}());

var LoadItemSuccessAction = /** @class */ (function () {
    function LoadItemSuccessAction(newItem) {
        this.newItem = newItem;
        this.type = LOAD_ITEM_SUCCESS;
    }
    return LoadItemSuccessAction;
}());

/**
 * Add
 */
var AddItemAttributeAction = /** @class */ (function () {
    function AddItemAttributeAction(id, attribute, attributeKey) {
        this.id = id;
        this.attribute = attribute;
        this.attributeKey = attributeKey;
        this.type = ADD_ITEM_ATTRIBUTE;
    }
    return AddItemAttributeAction;
}());

var AddItemAttributeValueAction = /** @class */ (function () {
    function AddItemAttributeValueAction(id, attributeValue, attributeKey) {
        this.id = id;
        this.attributeValue = attributeValue;
        this.attributeKey = attributeKey;
        this.type = ADD_ITEM_ATTRIBUTE_VALUE;
    }
    return AddItemAttributeValueAction;
}());

var AddItemAttributeDimensionAction = /** @class */ (function () {
    function AddItemAttributeDimensionAction(id, attributeKey, dimensionValue, existingDimensionValue, defaultLanguage, isReadOnly) {
        this.id = id;
        this.attributeKey = attributeKey;
        this.dimensionValue = dimensionValue;
        this.existingDimensionValue = existingDimensionValue;
        this.defaultLanguage = defaultLanguage;
        this.isReadOnly = isReadOnly;
        this.type = ADD_ITEM_ATTRIBUTE_DIMENSION;
    }
    return AddItemAttributeDimensionAction;
}());

/**
 * Update
 */
var UpdateItemAction = /** @class */ (function () {
    function UpdateItemAction(attributes, id) {
        this.attributes = attributes;
        this.id = id;
        this.type = UPDATE_ITEM;
    }
    return UpdateItemAction;
}());

var UpdateItemSuccessAction = /** @class */ (function () {
    function UpdateItemSuccessAction(item) {
        this.item = item;
        this.type = UPDATE_ITEM_SUCCESS;
    }
    return UpdateItemSuccessAction;
}());

var UpdateItemAttributeAction = /** @class */ (function () {
    function UpdateItemAttributeAction(id, attribute, attributeKey) {
        this.id = id;
        this.attribute = attribute;
        this.attributeKey = attributeKey;
        this.type = UPDATE_ITEM_ATTRIBUTE;
    }
    return UpdateItemAttributeAction;
}());

var UpdateItemAttributeValueAction = /** @class */ (function () {
    function UpdateItemAttributeValueAction(id, attributeKey, attributeValue, existingLanguageKey, defaultLanguage, isReadOnly) {
        this.id = id;
        this.attributeKey = attributeKey;
        this.attributeValue = attributeValue;
        this.existingLanguageKey = existingLanguageKey;
        this.defaultLanguage = defaultLanguage;
        this.isReadOnly = isReadOnly;
        this.type = UPDATE_ITEM_ATTRIBUTE_VALUE;
    }
    return UpdateItemAttributeValueAction;
}());

var UpdateItemAttributesValuesAction = /** @class */ (function () {
    function UpdateItemAttributesValuesAction(id, updateValues, existingLanguageKey, defaultLanguage) {
        this.id = id;
        this.updateValues = updateValues;
        this.existingLanguageKey = existingLanguageKey;
        this.defaultLanguage = defaultLanguage;
        this.type = UPDATE_ITEM_ATTRIBUTES_VALUES;
    }
    return UpdateItemAttributesValuesAction;
}());

/**
 * Save (submit)
 */
var SaveItemAttributesValuesAction = /** @class */ (function () {
    function SaveItemAttributesValuesAction(appId, item, updateValues, existingLanguageKey, defaultLanguage) {
        this.appId = appId;
        this.item = item;
        this.updateValues = updateValues;
        this.existingLanguageKey = existingLanguageKey;
        this.defaultLanguage = defaultLanguage;
        this.type = SAVE_ITEM_ATTRIBUTES_VALUES;
    }
    return SaveItemAttributesValuesAction;
}());

var SaveItemAttributesValuesSuccessAction = /** @class */ (function () {
    // TODO: finish this with true values
    function SaveItemAttributesValuesSuccessAction(data) {
        this.data = data;
        this.type = SAVE_ITEM_ATTRIBUTES_VALUES_SUCCESS;
    }
    return SaveItemAttributesValuesSuccessAction;
}());

var SaveItemAttributesValuesErrorAction = /** @class */ (function () {
    // TODO: finish this with true values
    function SaveItemAttributesValuesErrorAction(error) {
        this.error = error;
        this.type = SAVE_ITEM_ATTRIBUTES_VALUES_ERROR;
    }
    return SaveItemAttributesValuesErrorAction;
}());

// export class UpdateItemAttributeDimensionAction implements Action {
//     readonly type = UPDATE_ITEM_ATTRIBUTE_DIMENSION;
//     constructor(public id: number, public attributeKey: string, public dimensionValue: string,
//         public existingDimensionValue: string, public isReadOnly: boolean) { }
// }
var RemoveItemAttributeDimensionAction = /** @class */ (function () {
    function RemoveItemAttributeDimensionAction(id, attributeKey, dimensionValue) {
        this.id = id;
        this.attributeKey = attributeKey;
        this.dimensionValue = dimensionValue;
        this.type = REMOVE_ITEM_ATTRIBUTE_DIMENSION;
    }
    return RemoveItemAttributeDimensionAction;
}());

/**
 * Delete
 */
var DeleteItemAction = /** @class */ (function () {
    function DeleteItemAction(item) {
        this.item = item;
        this.type = DELETE_ITEM;
    }
    return DeleteItemAction;
}());



/***/ }),

/***/ "./src/app/shared/store/actions/language.actions.ts":
/*!**********************************************************!*\
  !*** ./src/app/shared/store/actions/language.actions.ts ***!
  \**********************************************************/
/*! exports provided: LOAD_LANGUAGES, UPDATE_CURRENT_LANGUAGE, UPDATE_DEFAULT_LANGUAGE, UPDATE_UI_LANGUAGE, LoadLanguagesAction, UpdateCurrentLanguageAction, UpdateDefaultLanguageAction, UpdateUILanguageAction */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LOAD_LANGUAGES", function() { return LOAD_LANGUAGES; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_CURRENT_LANGUAGE", function() { return UPDATE_CURRENT_LANGUAGE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_DEFAULT_LANGUAGE", function() { return UPDATE_DEFAULT_LANGUAGE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UPDATE_UI_LANGUAGE", function() { return UPDATE_UI_LANGUAGE; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoadLanguagesAction", function() { return LoadLanguagesAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateCurrentLanguageAction", function() { return UpdateCurrentLanguageAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateDefaultLanguageAction", function() { return UpdateDefaultLanguageAction; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdateUILanguageAction", function() { return UpdateUILanguageAction; });
var LOAD_LANGUAGES = '[Language] LOAD_LANGUAGES';
var UPDATE_CURRENT_LANGUAGE = '[Language] UPDATE_CURRENT_LANGUAGE';
var UPDATE_DEFAULT_LANGUAGE = '[Language] UPDATE_DEFAULT_LANGUAGE';
var UPDATE_UI_LANGUAGE = '[Language] UPDATE_UI_LANGUAGE';
var LoadLanguagesAction = /** @class */ (function () {
    function LoadLanguagesAction(newLanguage, currentLanguage, defaultLanguage, uiLanguage) {
        this.newLanguage = newLanguage;
        this.currentLanguage = currentLanguage;
        this.defaultLanguage = defaultLanguage;
        this.uiLanguage = uiLanguage;
        this.type = LOAD_LANGUAGES;
    }
    return LoadLanguagesAction;
}());

var UpdateCurrentLanguageAction = /** @class */ (function () {
    function UpdateCurrentLanguageAction(currentLanguage) {
        this.currentLanguage = currentLanguage;
        this.type = UPDATE_CURRENT_LANGUAGE;
    }
    return UpdateCurrentLanguageAction;
}());

var UpdateDefaultLanguageAction = /** @class */ (function () {
    function UpdateDefaultLanguageAction(defaultLanguage) {
        this.defaultLanguage = defaultLanguage;
        this.type = UPDATE_DEFAULT_LANGUAGE;
    }
    return UpdateDefaultLanguageAction;
}());

var UpdateUILanguageAction = /** @class */ (function () {
    function UpdateUILanguageAction(uiLanguage) {
        this.uiLanguage = uiLanguage;
        this.type = UPDATE_UI_LANGUAGE;
    }
    return UpdateUILanguageAction;
}());



/***/ }),

/***/ "./src/app/shared/store/index.ts":
/*!***************************************!*\
  !*** ./src/app/shared/store/index.ts ***!
  \***************************************/
/*! exports provided: logger, metaReducers, reducers, getEavState, getItemState, getItems, getInputTypeState, getInputTypes, getContentTypeState, getContentTypes, getLanguageState, getLanguages, getCurrentLanguage, getDefaultLanguage, getUILanguage */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _reducers__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./reducers */ "./src/app/shared/store/reducers/index.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "logger", function() { return _reducers__WEBPACK_IMPORTED_MODULE_0__["logger"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "metaReducers", function() { return _reducers__WEBPACK_IMPORTED_MODULE_0__["metaReducers"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "reducers", function() { return _reducers__WEBPACK_IMPORTED_MODULE_0__["reducers"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getEavState", function() { return _reducers__WEBPACK_IMPORTED_MODULE_0__["getEavState"]; });

/* harmony import */ var _selectors__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./selectors */ "./src/app/shared/store/selectors/index.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getItemState", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getItemState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getItems", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getItems"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getInputTypeState", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getInputTypeState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getInputTypes", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getInputTypes"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getContentTypeState", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getContentTypeState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getContentTypes", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getContentTypes"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getLanguageState", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getLanguageState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getLanguages", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getLanguages"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getCurrentLanguage", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getCurrentLanguage"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getDefaultLanguage", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getDefaultLanguage"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getUILanguage", function() { return _selectors__WEBPACK_IMPORTED_MODULE_1__["getUILanguage"]; });





/***/ }),

/***/ "./src/app/shared/store/reducers/content-type.reducer.ts":
/*!***************************************************************!*\
  !*** ./src/app/shared/store/reducers/content-type.reducer.ts ***!
  \***************************************************************/
/*! exports provided: initialState, contentTypeReducer, getContentTypes */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "initialState", function() { return initialState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentTypeReducer", function() { return contentTypeReducer; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getContentTypes", function() { return getContentTypes; });
/* harmony import */ var _actions_content_type_actions__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../actions/content-type.actions */ "./src/app/shared/store/actions/content-type.actions.ts");

var initialState = {
    contentTypes: []
};
function contentTypeReducer(state, action) {
    if (state === void 0) { state = initialState; }
    switch (action.type) {
        case _actions_content_type_actions__WEBPACK_IMPORTED_MODULE_0__["LOAD_CONTENT_TYPE_SUCCESS"]: {
            // if contentType with same id exist in store don't load content
            var contentTypes = state.contentTypes.filter(function (contentType) {
                return contentType.contentType.id === action.newContentType.contentType.id;
            });
            if (contentTypes.length === 0) {
                return { contentTypes: state.contentTypes.concat([action.newContentType]) };
            }
            else {
                return state;
            }
        }
        default: {
            return state;
        }
    }
}
var getContentTypes = function (state) { return state.contentTypes; };


/***/ }),

/***/ "./src/app/shared/store/reducers/index.ts":
/*!************************************************!*\
  !*** ./src/app/shared/store/reducers/index.ts ***!
  \************************************************/
/*! exports provided: logger, metaReducers, reducers, getEavState */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "logger", function() { return logger; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "metaReducers", function() { return metaReducers; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "reducers", function() { return reducers; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getEavState", function() { return getEavState; });
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var ngrx_store_freeze__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ngrx-store-freeze */ "./node_modules/ngrx-store-freeze/index.js");
/* harmony import */ var ngrx_store_freeze__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(ngrx_store_freeze__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../../../environments/environment */ "./src/environments/environment.ts");
/* harmony import */ var _item_reducer__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./item.reducer */ "./src/app/shared/store/reducers/item.reducer.ts");
/* harmony import */ var _input_type_reducer__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./input-type.reducer */ "./src/app/shared/store/reducers/input-type.reducer.ts");
/* harmony import */ var _content_type_reducer__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./content-type.reducer */ "./src/app/shared/store/reducers/content-type.reducer.ts");
/* harmony import */ var _language_reducer__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./language.reducer */ "./src/app/shared/store/reducers/language.reducer.ts");







// console.log all actions
function logger(reducer) {
    return function (state, action) {
        // console.log('[STORE] state', JSON.stringify(state));
        console.log('[STORE] state', state);
        console.log('[STORE] action', action);
        return reducer(state, action);
    };
}
/**
 * By default, @ngrx/store uses combineReducers with the reducer map to compose
 * the root meta-reducer. To add more meta-reducers, provide an array of meta-reducers
 * that will be composed to form the root meta-reducer.
 */
var metaReducers = !_environments_environment__WEBPACK_IMPORTED_MODULE_2__["environment"].production
    ? [logger, ngrx_store_freeze__WEBPACK_IMPORTED_MODULE_1__["storeFreeze"]]
    : [];
var reducers = {
    itemState: _item_reducer__WEBPACK_IMPORTED_MODULE_3__["itemReducer"],
    inputTypeState: _input_type_reducer__WEBPACK_IMPORTED_MODULE_4__["inputTypeReducer"],
    contentTypeState: _content_type_reducer__WEBPACK_IMPORTED_MODULE_5__["contentTypeReducer"],
    languages: _language_reducer__WEBPACK_IMPORTED_MODULE_6__["languageReducer"],
};
var getEavState = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createFeatureSelector"])('eavItemDialog');


/***/ }),

/***/ "./src/app/shared/store/reducers/input-type.reducer.ts":
/*!*************************************************************!*\
  !*** ./src/app/shared/store/reducers/input-type.reducer.ts ***!
  \*************************************************************/
/*! exports provided: initialState, inputTypeReducer, getInputTypes */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "initialState", function() { return initialState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "inputTypeReducer", function() { return inputTypeReducer; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getInputTypes", function() { return getInputTypes; });
/* harmony import */ var _actions_input_type_actions__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../actions/input-type.actions */ "./src/app/shared/store/actions/input-type.actions.ts");
var __assign = (undefined && undefined.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};

var initialState = {
    inputTypes: []
};
function inputTypeReducer(state, action) {
    if (state === void 0) { state = initialState; }
    switch (action.type) {
        case _actions_input_type_actions__WEBPACK_IMPORTED_MODULE_0__["LOAD_INPUT_TYPE_SUCCESS"]: {
            return __assign({}, state, {
                inputTypes: action.newInputTypes.slice()
            });
        }
        default: {
            return state;
        }
    }
}
var getInputTypes = function (state) { return state.inputTypes; };


/***/ }),

/***/ "./src/app/shared/store/reducers/item.reducer.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/store/reducers/item.reducer.ts ***!
  \*******************************************************/
/*! exports provided: initialState, itemReducer, getItems */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "initialState", function() { return initialState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "itemReducer", function() { return itemReducer; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getItems", function() { return getItems; });
/* harmony import */ var _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../actions/item.actions */ "./src/app/shared/store/actions/item.actions.ts");
/* harmony import */ var _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../helpers/localization-helper */ "./src/app/shared/helpers/localization-helper.ts");
var __assign = (undefined && undefined.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};


var initialState = {
    items: []
};
function itemReducer(state, action) {
    if (state === void 0) { state = initialState; }
    switch (action.type) {
        // case fromItems.LOAD_ITEM_SUCCESS: {
        //     console.log('LOAD_ITEM_SUCCESS', action);
        //     return {
        //         ...state,
        //         ...{ items: [...state.items, action.newItem] }
        //     };
        // }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["LOAD_ITEM_SUCCESS"]: {
            // if item with same id not exist in store add item else overwrite item
            var itemExist = state.items.filter(function (data) {
                return data.entity.id === action.newItem.entity.id;
            });
            if (itemExist.length === 0) {
                return __assign({}, state, { items: state.items.concat([action.newItem]) });
            }
            else {
                return __assign({}, state, {
                    items: state.items.map(function (item) {
                        return item.entity.id === action.newItem.entity.id
                            ? action.newItem
                            : item;
                    })
                });
            }
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["UPDATE_ITEM"]: {
            console.log('action.attributes', action.attributes);
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: __assign({}, item.entity.attributes, action.attributes) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["UPDATE_ITEM_ATTRIBUTE"]: {
            console.log('action.attribute', action.attribute);
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__["LocalizationHelper"].updateAttribute(item.entity.attributes, action.attribute, action.attributeKey) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["UPDATE_ITEM_ATTRIBUTE_VALUE"]: {
            // console.log('action.attribute', action.attribute);
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__["LocalizationHelper"].updateAttributeValue(item.entity.attributes, action.attributeKey, action.attributeValue, action.existingLanguageKey, action.defaultLanguage, action.isReadOnly) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["SAVE_ITEM_ATTRIBUTES_VALUES"]: {
            console.log('action.attribute', action);
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.item.entity.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__["LocalizationHelper"].updateAttributesValues(item.entity.attributes, action.updateValues, action.existingLanguageKey, action.defaultLanguage) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["UPDATE_ITEM_ATTRIBUTES_VALUES"]: {
            console.log('action.attribute', action);
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__["LocalizationHelper"].updateAttributesValues(item.entity.attributes, action.updateValues, action.existingLanguageKey, action.defaultLanguage) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["ADD_ITEM_ATTRIBUTE_VALUE"]: {
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__["LocalizationHelper"].addAttributeValue(item.entity.attributes, action.attributeValue, action.attributeKey) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["ADD_ITEM_ATTRIBUTE_DIMENSION"]: {
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__["LocalizationHelper"].addAttributeDimension(item.entity.attributes, action.attributeKey, action.dimensionValue, action.existingDimensionValue, action.defaultLanguage, action.isReadOnly) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["REMOVE_ITEM_ATTRIBUTE_DIMENSION"]: {
            return __assign({}, state, {
                items: state.items.map(function (item) {
                    return item.entity.id === action.id
                        ? __assign({}, item, { entity: __assign({}, item.entity, { attributes: _helpers_localization_helper__WEBPACK_IMPORTED_MODULE_1__["LocalizationHelper"].removeAttributeDimension(item.entity.attributes, action.attributeKey, action.dimensionValue) }) }) : item;
                })
            });
        }
        case _actions_item_actions__WEBPACK_IMPORTED_MODULE_0__["DELETE_ITEM"]:
            return __assign({}, state, {
                items: state.items.filter(function (item) { return item.entity.id !== action.item.entity.id; })
            });
        default: {
            return state;
        }
    }
}
var getItems = function (state) { return state.items; };


/***/ }),

/***/ "./src/app/shared/store/reducers/language.reducer.ts":
/*!***********************************************************!*\
  !*** ./src/app/shared/store/reducers/language.reducer.ts ***!
  \***********************************************************/
/*! exports provided: initialState, languageReducer, getLanguages, getCurrentLanguage, getDefaultLanguage, getUILanguage */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "initialState", function() { return initialState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "languageReducer", function() { return languageReducer; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getLanguages", function() { return getLanguages; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getCurrentLanguage", function() { return getCurrentLanguage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getDefaultLanguage", function() { return getDefaultLanguage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getUILanguage", function() { return getUILanguage; });
/* harmony import */ var _actions_language_actions__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../actions/language.actions */ "./src/app/shared/store/actions/language.actions.ts");
var __assign = (undefined && undefined.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};

var initialState = {
    languages: [],
    currentLanguage: 'en-us',
    defaultLanguage: 'en-us',
    uiLanguage: 'en-us',
};
function languageReducer(state, action) {
    if (state === void 0) { state = initialState; }
    switch (action.type) {
        case _actions_language_actions__WEBPACK_IMPORTED_MODULE_0__["LOAD_LANGUAGES"]: {
            // console.log('loadsuccess item: ', action.newItem);
            return __assign({}, state, {
                languages: action.newLanguage.slice(),
                currentLanguage: action.currentLanguage,
                defaultLanguage: action.defaultLanguage,
                uiLanguage: action.uiLanguage,
            });
        }
        case _actions_language_actions__WEBPACK_IMPORTED_MODULE_0__["UPDATE_CURRENT_LANGUAGE"]: {
            console.log('action.attributes', action.currentLanguage);
            return __assign({}, state, {
                currentLanguage: action.currentLanguage
            });
        }
        case _actions_language_actions__WEBPACK_IMPORTED_MODULE_0__["UPDATE_DEFAULT_LANGUAGE"]: {
            console.log('action.attributes', action.defaultLanguage);
            return __assign({}, state, {
                defaultLanguage: action.defaultLanguage
            });
        }
        case _actions_language_actions__WEBPACK_IMPORTED_MODULE_0__["UPDATE_UI_LANGUAGE"]: {
            console.log('action.attributes', action.uiLanguage);
            return __assign({}, state, {
                uiLanguage: action.uiLanguage
            });
        }
        // case fromItems.DELETE_ITEM:
        //     return {
        //         ...state,
        //         ...{
        //             items: state.items.filter(item => item.entity.id !== action.item.entity.id)
        //         }
        //     };
        default: {
            return state;
        }
    }
}
var getLanguages = function (state) { return state.languages; };
var getCurrentLanguage = function (state) { return state.currentLanguage; };
var getDefaultLanguage = function (state) { return state.defaultLanguage; };
var getUILanguage = function (state) { return state.uiLanguage; };


/***/ }),

/***/ "./src/app/shared/store/selectors/content-type.selectors.ts":
/*!******************************************************************!*\
  !*** ./src/app/shared/store/selectors/content-type.selectors.ts ***!
  \******************************************************************/
/*! exports provided: getContentTypeState, getContentTypes */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getContentTypeState", function() { return getContentTypeState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getContentTypes", function() { return getContentTypes; });
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var _reducers__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../reducers */ "./src/app/shared/store/reducers/index.ts");
/* harmony import */ var _reducers_content_type_reducer__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../reducers/content-type.reducer */ "./src/app/shared/store/reducers/content-type.reducer.ts");



var getContentTypeState = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(_reducers__WEBPACK_IMPORTED_MODULE_1__["getEavState"], function (state) { return state.contentTypeState; });
var getContentTypes = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(getContentTypeState, _reducers_content_type_reducer__WEBPACK_IMPORTED_MODULE_2__["getContentTypes"]);


/***/ }),

/***/ "./src/app/shared/store/selectors/index.ts":
/*!*************************************************!*\
  !*** ./src/app/shared/store/selectors/index.ts ***!
  \*************************************************/
/*! exports provided: getItemState, getItems, getInputTypeState, getInputTypes, getContentTypeState, getContentTypes, getLanguageState, getLanguages, getCurrentLanguage, getDefaultLanguage, getUILanguage */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _items_selectors__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./items.selectors */ "./src/app/shared/store/selectors/items.selectors.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getItemState", function() { return _items_selectors__WEBPACK_IMPORTED_MODULE_0__["getItemState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getItems", function() { return _items_selectors__WEBPACK_IMPORTED_MODULE_0__["getItems"]; });

/* harmony import */ var _input_type_selectors__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./input-type.selectors */ "./src/app/shared/store/selectors/input-type.selectors.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getInputTypeState", function() { return _input_type_selectors__WEBPACK_IMPORTED_MODULE_1__["getInputTypeState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getInputTypes", function() { return _input_type_selectors__WEBPACK_IMPORTED_MODULE_1__["getInputTypes"]; });

/* harmony import */ var _content_type_selectors__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./content-type.selectors */ "./src/app/shared/store/selectors/content-type.selectors.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getContentTypeState", function() { return _content_type_selectors__WEBPACK_IMPORTED_MODULE_2__["getContentTypeState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getContentTypes", function() { return _content_type_selectors__WEBPACK_IMPORTED_MODULE_2__["getContentTypes"]; });

/* harmony import */ var _language_selectors__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./language.selectors */ "./src/app/shared/store/selectors/language.selectors.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getLanguageState", function() { return _language_selectors__WEBPACK_IMPORTED_MODULE_3__["getLanguageState"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getLanguages", function() { return _language_selectors__WEBPACK_IMPORTED_MODULE_3__["getLanguages"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getCurrentLanguage", function() { return _language_selectors__WEBPACK_IMPORTED_MODULE_3__["getCurrentLanguage"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getDefaultLanguage", function() { return _language_selectors__WEBPACK_IMPORTED_MODULE_3__["getDefaultLanguage"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "getUILanguage", function() { return _language_selectors__WEBPACK_IMPORTED_MODULE_3__["getUILanguage"]; });







/***/ }),

/***/ "./src/app/shared/store/selectors/input-type.selectors.ts":
/*!****************************************************************!*\
  !*** ./src/app/shared/store/selectors/input-type.selectors.ts ***!
  \****************************************************************/
/*! exports provided: getInputTypeState, getInputTypes */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getInputTypeState", function() { return getInputTypeState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getInputTypes", function() { return getInputTypes; });
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var _reducers__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../reducers */ "./src/app/shared/store/reducers/index.ts");
/* harmony import */ var _reducers_input_type_reducer__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../reducers/input-type.reducer */ "./src/app/shared/store/reducers/input-type.reducer.ts");



var getInputTypeState = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(_reducers__WEBPACK_IMPORTED_MODULE_1__["getEavState"], function (state) { return state.inputTypeState; });
var getInputTypes = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(getInputTypeState, _reducers_input_type_reducer__WEBPACK_IMPORTED_MODULE_2__["getInputTypes"]);


/***/ }),

/***/ "./src/app/shared/store/selectors/items.selectors.ts":
/*!***********************************************************!*\
  !*** ./src/app/shared/store/selectors/items.selectors.ts ***!
  \***********************************************************/
/*! exports provided: getItemState, getItems */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getItemState", function() { return getItemState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getItems", function() { return getItems; });
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var _reducers__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../reducers */ "./src/app/shared/store/reducers/index.ts");
/* harmony import */ var _reducers_item_reducer__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../reducers/item.reducer */ "./src/app/shared/store/reducers/item.reducer.ts");



var getItemState = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(_reducers__WEBPACK_IMPORTED_MODULE_1__["getEavState"], function (state) { return state.itemState; });
var getItems = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(getItemState, _reducers_item_reducer__WEBPACK_IMPORTED_MODULE_2__["getItems"]);


/***/ }),

/***/ "./src/app/shared/store/selectors/language.selectors.ts":
/*!**************************************************************!*\
  !*** ./src/app/shared/store/selectors/language.selectors.ts ***!
  \**************************************************************/
/*! exports provided: getLanguageState, getLanguages, getCurrentLanguage, getDefaultLanguage, getUILanguage */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getLanguageState", function() { return getLanguageState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getLanguages", function() { return getLanguages; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getCurrentLanguage", function() { return getCurrentLanguage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getDefaultLanguage", function() { return getDefaultLanguage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getUILanguage", function() { return getUILanguage; });
/* harmony import */ var _ngrx_store__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngrx/store */ "./node_modules/@ngrx/store/fesm5/store.js");
/* harmony import */ var _reducers__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../reducers */ "./src/app/shared/store/reducers/index.ts");
/* harmony import */ var _reducers_language_reducer__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../reducers/language.reducer */ "./src/app/shared/store/reducers/language.reducer.ts");



var getLanguageState = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(_reducers__WEBPACK_IMPORTED_MODULE_1__["getEavState"], function (state) { return state.languages; });
var getLanguages = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(getLanguageState, _reducers_language_reducer__WEBPACK_IMPORTED_MODULE_2__["getLanguages"]);
var getCurrentLanguage = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(getLanguageState, _reducers_language_reducer__WEBPACK_IMPORTED_MODULE_2__["getCurrentLanguage"]);
var getDefaultLanguage = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(getLanguageState, _reducers_language_reducer__WEBPACK_IMPORTED_MODULE_2__["getDefaultLanguage"]);
var getUILanguage = Object(_ngrx_store__WEBPACK_IMPORTED_MODULE_0__["createSelector"])(getLanguageState, _reducers_language_reducer__WEBPACK_IMPORTED_MODULE_2__["getUILanguage"]);


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
// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.
var environment = {
    production: false
};


/***/ }),

/***/ "./src/main.ts":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/platform-browser-dynamic */ "./node_modules/@angular/platform-browser-dynamic/fesm5/platform-browser-dynamic.js");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./app/app.module */ "./src/app/app.module.ts");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./environments/environment */ "./src/environments/environment.ts");




if (_environments_environment__WEBPACK_IMPORTED_MODULE_3__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["enableProdMode"])();
    window.console.log = function () { };
}
Object(_angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_1__["platformBrowserDynamic"])().bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_2__["AppModule"])
    .catch(function (err) { return console.log(err); });


/***/ }),

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! C:\Projects\eav-item-dialog-angular\src\main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map