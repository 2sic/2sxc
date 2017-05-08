webpackJsonp([1,4],{

/***/ 114:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(27);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_map__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_map__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__ = __webpack_require__(39);
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
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};




var ModuleApiService = (function () {
    function ModuleApiService(http, sxc) {
        this.http = http;
        this.sxc = sxc;
        this.base = 'http://2sxc.dev/desktopmodules/2sxc/api/';
    }
    ModuleApiService.prototype.getSelectableApps = function () {
        return this.http.get('View/Module/GetSelectableApps')
            .map(function (response) { return response.json(); });
    };
    ModuleApiService.prototype.setAppId = function (appId) {
        return this.http.get("view/Module/SetAppId?appId=" + appId);
    };
    ModuleApiService.prototype.getSelectableContentTypes = function () {
        return this.http.get('View/Module/GetSelectableContentTypes')
            .map(function (response) { return (response.json() || []).map(function (x) {
            x.Label = (x.Metadata && x.Metadata.Label)
                ? x.Metadata.Label
                : x.Name;
            return x;
        }); });
    };
    ModuleApiService.prototype.getSelectableTemplates = function () {
        return this.http.get('View/Module/GetSelectableTemplates')
            .map(function (response) { return response.json(); });
    };
    ModuleApiService.prototype.gettingStartedUrl = function () {
        return this.http.get('View/Module/RemoteInstallDialogUrl?dialog=gettingstarted');
    };
    return ModuleApiService;
}());
ModuleApiService = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __param(0, __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["g" /* Inject */])(__WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */])),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _b || Object])
], ModuleApiService);

var _a, _b;
//# sourceMappingURL=module-api.service.js.map

/***/ }),

/***/ 115:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(27);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__ = __webpack_require__(130);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return GettingStartedService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var GettingStartedService = (function () {
    function GettingStartedService(http) {
        this.http = http;
    }
    GettingStartedService.prototype.processInstallMessage = function (event, modId) {
        var regExToCheckOrigin = /^(http|https):\/\/((gettingstarted|[a-z]*)\.)?(2sexycontent|2sxc)\.org(\/.*)?$/gi;
        if (!regExToCheckOrigin.test(event.origin))
            throw 'Cannot execute. Wrong source domain.';
        var data = JSON.parse(event.data);
        modId = Number(modId);
        if (data.moduleId !== modId)
            return;
        if (data.action === "install") {
            var packages = data.packages;
            var packagesDisplayNames = "";
            for (var i = 0; i < packages.length; i++) {
                packagesDisplayNames += "- " + packages[i].displayName + "\n";
            }
            if (confirm("\n          Do you want to install these packages?\n\n\n          " + packagesDisplayNames + "\nThis could take 10 to 60 seconds per package, \n          please don't reload the page while it's installing.\n          You will see a message once it's done and progess is logged to the JS-console.")) {
                return this.runOneInstallJob(packages, 0);
            }
        }
        else if (data.action === "resize")
            this.resizeIFrame(modId, data.height);
        return __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__["Observable"].empty();
    };
    GettingStartedService.prototype.resizeIFrame = function (modId, height) {
        document.getElementById("frGettingStarted").style.height = (height + 10) + "px";
    };
    GettingStartedService.prototype.runOneInstallJob = function (packages, i) {
        var _this = this;
        var currentPackage = packages[i];
        console.log(currentPackage.displayName + " (" + i + ") started");
        return this.http.get("app-sys/installer/installpackage?packageUrl=" + currentPackage.url)
            .map(function (response) {
            console.log(currentPackage.displayName + " (" + i + ") completed");
            if (i + 1 < packages.length) {
                _this.runOneInstallJob(packages, i + 1)
                    .subscribe(function (res) { });
            }
            else {
                alert("Done installing. If you saw no errors, everything worked.");
                window.top.location.reload();
            }
            return response;
        })
            .catch(function (e) {
            console.error(e);
            var errorMessage = "Something went wrong while installing '" + currentPackage.displayName + "': " + status;
            if (e.responseText && e.responseText !== "") {
                var response = JSON.parse(e.responseText);
                if (response.messages)
                    errorMessage = errorMessage + " - " + response.messages[0].Message;
                else if (response.Message)
                    errorMessage = errorMessage + " - " + response.Message;
            }
            errorMessage += " (you might find more informations about the error in the DNN event log).";
            alert(errorMessage);
            return __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__["Observable"].throw(e);
        });
    };
    return GettingStartedService;
}());
GettingStartedService = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */]) === "function" && _a || Object])
], GettingStartedService);

var _a;
//# sourceMappingURL=getting-started.service.js.map

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
            .filter(function (t) { return !t.IsHidden && (!args.isContentApp || t.ContentTypeStaticName === (args.contentTypeId || '')); });
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
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_app_core_module_api_service__ = __webpack_require__(114);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__ = __webpack_require__(130);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_router__ = __webpack_require__(54);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__ = __webpack_require__(116);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_http__ = __webpack_require__(27);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_app_getting_started_service__ = __webpack_require__(115);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__angular_platform_browser__ = __webpack_require__(18);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_app_core_2sxc_service__ = __webpack_require__(39);
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
    function TemplatePickerComponent(api, route, http, gettingStarted, templateFilter, sanitizer, sxc) {
        var _this = this;
        this.api = api;
        this.route = route;
        this.http = http;
        this.gettingStarted = gettingStarted;
        this.templateFilter = templateFilter;
        this.sanitizer = sanitizer;
        this.sxc = sxc;
        this.cViewWithoutContent = '_LayoutElement';
        this.cAppActionManage = -2;
        this.cAppActionImport = -1;
        this.cAppActionCreate = -3;
        this.apps = [];
        this.contentTypes = [];
        this.templates = [];
        this.showProgress = false;
        this.showRemoteInstaller = false;
        this.remoteInstallerUrl = '';
        this.isLoading = false;
        this.externalInstaller = {
            showIfConfigIsEmpty: function () {
                var showAutoInstaller = _this.isContentApp
                    ? _this.templates.length === 0
                    : _this.apps.length === 0;
                if (showAutoInstaller)
                    _this.externalInstaller.setup();
            },
            configureCallback: function () {
                window.addEventListener("message", function (evt) {
                    _this.showProgress = true;
                    // TODO: mid into service
                    _this.gettingStarted.processInstallMessage(evt, $2sxc.urlParams.require('mid'))
                        .subscribe(function () { return _this.showProgress = false; });
                }, false);
            },
            setup: function () {
                _this.api.gettingStartedUrl()
                    .subscribe(function (response) {
                    // only show getting started if it's really still a blank system, otherwise the server will return null, then don't do anything
                    if (!response.json())
                        return;
                    _this.externalInstaller.configureCallback();
                    _this.showRemoteInstaller = true;
                    _this.remoteInstallerUrl = _this.sanitizer.bypassSecurityTrustResourceUrl(response.json());
                });
            }
        };
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
                return _this.frame.sxc.manage.contentBlock.reloadAndReInitialize(true);
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
                .subscribe(this.externalInstaller.showIfConfigIsEmpty);
        }
    };
    TemplatePickerComponent.prototype.loadApps = function () {
        var _this = this;
        var obs = this.api.getSelectableApps();
        obs.subscribe(function (apps) {
            _this.apps = apps;
            if (_this.showAdvanced)
                _this.apps.push({ Name: "TemplatePicker.Install", AppId: _this.cAppActionImport });
        });
        return obs;
    };
    ;
    return TemplatePickerComponent;
}());
TemplatePickerComponent = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_13" /* Component */])({
        selector: 'app-template-picker',
        template: __webpack_require__(271),
        styles: [__webpack_require__(267)]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1_app_core_module_api_service__["a" /* ModuleApiService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1_app_core_module_api_service__["a" /* ModuleApiService */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* ActivatedRoute */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* ActivatedRoute */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5__angular_http__["b" /* Http */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_5__angular_http__["b" /* Http */]) === "function" && _c || Object, typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_6_app_getting_started_service__["a" /* GettingStartedService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_6_app_getting_started_service__["a" /* GettingStartedService */]) === "function" && _d || Object, typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_4_app_template_picker_template_filter_pipe__["a" /* TemplateFilterPipe */]) === "function" && _e || Object, typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_7__angular_platform_browser__["c" /* DomSanitizer */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_7__angular_platform_browser__["c" /* DomSanitizer */]) === "function" && _f || Object, typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_8_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_8_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _g || Object])
], TemplatePickerComponent);

var _a, _b, _c, _d, _e, _f, _g;
//# sourceMappingURL=template-picker.component.js.map

/***/ }),

/***/ 181:
/***/ (function(module, exports) {

function webpackEmptyContext(req) {
	throw new Error("Cannot find module '" + req + "'.");
}
webpackEmptyContext.keys = function() { return []; };
webpackEmptyContext.resolve = webpackEmptyContext;
module.exports = webpackEmptyContext;
webpackEmptyContext.id = 181;


/***/ }),

/***/ 182:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__ = __webpack_require__(204);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__app_app_module__ = __webpack_require__(207);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__environments_environment__ = __webpack_require__(212);




if (__WEBPACK_IMPORTED_MODULE_3__environments_environment__["a" /* environment */].production) {
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["a" /* enableProdMode */])();
}
__webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])().bootstrapModule(__WEBPACK_IMPORTED_MODULE_2__app_app_module__["a" /* AppModule */]);
//# sourceMappingURL=main.js.map

/***/ }),

/***/ 206:
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
        template: __webpack_require__(270),
        styles: [__webpack_require__(268)]
    }),
    __metadata("design:paramtypes", [])
], AppComponent);

//# sourceMappingURL=app.component.js.map

/***/ }),

/***/ 207:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser__ = __webpack_require__(18);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__(75);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_http__ = __webpack_require__(27);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__app_component__ = __webpack_require__(206);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_app_template_picker_template_picker_module__ = __webpack_require__(211);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_app_template_picker_template_picker_component__ = __webpack_require__(117);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__angular_router__ = __webpack_require__(54);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_app_getting_started_service__ = __webpack_require__(115);
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
            // { provide: "windowObject", useValue: window },
            __WEBPACK_IMPORTED_MODULE_8_app_getting_started_service__["a" /* GettingStartedService */]
            // { provide: APP_BASE_HREF, useValue: window['_app_base'] || '/' }
        ],
        bootstrap: [__WEBPACK_IMPORTED_MODULE_4__app_component__["a" /* AppComponent */]]
    })
], AppModule);

//# sourceMappingURL=app.module.js.map

/***/ }),

/***/ 208:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__(25);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(54);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_app_core_module_api_service__ = __webpack_require__(114);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_app_core_2sxc_service__ = __webpack_require__(39);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_http__ = __webpack_require__(27);
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
            __WEBPACK_IMPORTED_MODULE_2__angular_router__["a" /* RouterModule */]
        ],
        declarations: [],
        providers: [
            __WEBPACK_IMPORTED_MODULE_3_app_core_module_api_service__["a" /* ModuleApiService */],
            __WEBPACK_IMPORTED_MODULE_4_app_core_2sxc_service__["a" /* $2sxcService */],
            __WEBPACK_IMPORTED_MODULE_5__angular_http__["b" /* Http */]
        ]
    })
], CoreModule);

//# sourceMappingURL=core.module.js.map

/***/ }),

/***/ 209:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(27);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__ = __webpack_require__(39);
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["f" /* ConnectionBackend */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["f" /* ConnectionBackend */]) === "function" && _a || Object, typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */]) === "function" && _b || Object, typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_2_app_core_2sxc_service__["a" /* $2sxcService */]) === "function" && _c || Object])
], HttpInterceptorService);

var _a, _b, _c;
//# sourceMappingURL=http-interceptor.service.js.map

/***/ }),

/***/ 210:
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

/***/ 211:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_forms__ = __webpack_require__(75);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_material__ = __webpack_require__(203);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_common__ = __webpack_require__(25);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_platform_browser_animations__ = __webpack_require__(205);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__template_picker_component__ = __webpack_require__(117);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__template_filter_pipe__ = __webpack_require__(116);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_app_core_core_module__ = __webpack_require__(208);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__content_type_filter_pipe__ = __webpack_require__(210);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__angular_flex_layout__ = __webpack_require__(200);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__angular_http__ = __webpack_require__(27);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11_app_http_interceptor_service__ = __webpack_require__(209);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12_app_core_2sxc_service__ = __webpack_require__(39);
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["b" /* NgModule */])({
        exports: [
            __WEBPACK_IMPORTED_MODULE_5__template_picker_component__["a" /* TemplatePickerComponent */]
        ],
        imports: [
            __WEBPACK_IMPORTED_MODULE_3__angular_common__["c" /* CommonModule */],
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
        // { provide: appId, useValue: appId }],
        declarations: [__WEBPACK_IMPORTED_MODULE_5__template_picker_component__["a" /* TemplatePickerComponent */], __WEBPACK_IMPORTED_MODULE_6__template_filter_pipe__["a" /* TemplateFilterPipe */], __WEBPACK_IMPORTED_MODULE_8__content_type_filter_pipe__["a" /* ContentTypeFilterPipe */]]
    })
], TemplatePickerModule);

//# sourceMappingURL=template-picker.module.js.map

/***/ }),

/***/ 212:
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

/***/ 267:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(45)();
// imports


// module
exports.push([module.i, ":host md-card {\n  background: #fff; }\n  :host md-card md-select {\n    width: 320px; }\n  :host md-card .row {\n    margin: 8px 0; }\n  :host md-card button {\n    margin: 0 0 0 8px;\n    float: left;\n    background: #0088f4; }\n  :host md-card .fr-getting-started {\n    border: none; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 268:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(45)();
// imports


// module
exports.push([module.i, ":host {\r\n    display: block;\r\n    padding: 0 0 180px;\r\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 270:
/***/ (function(module, exports) {

module.exports = "<router-outlet></router-outlet>"

/***/ }),

/***/ 271:
/***/ (function(module, exports) {

module.exports = "<md-card>\r\n    <div *ngIf=\"!isContentApp\" fxLayout=\"row\" class=\"row\">\r\n        <md-select [(ngModel)]=\"appId\" (change)=\"updateAppId($event)\" [disabled]=\"dashInfo.hasContent\">\r\n            <md-option *ngFor=\"let app of apps\" [value]=\"app.AppId\">\r\n                {{ app.Name.indexOf('TemplatePicker.') === 0 ? '[+] ' + app.Name : app.Name }}\r\n            </md-option>\r\n        </md-select>\r\n        <div *ngIf=\"showAdvanced && !isContentApp\">\r\n            <button md-mini-fab *ngIf=\"appId !== null\" (click)=\"frame.sxc.manage.run('app')\" title=\"{{ 'TemplatePicker.App' }}\">\r\n                <md-icon>settings</md-icon>\r\n            </button>\r\n            <button md-mini-fab (click)=\"frame.sxc.manage.run('app-import')\" title=\"{{ 'TemplatePicker.Install' }}\">\r\n                <md-icon>add</md-icon>\r\n            </button>\r\n            <button md-mini-fab (click)=\"appStore()\" title=\"{{ 'TemplatePicker.Catalog' }}\">\r\n                <md-icon>add_shopping_cart</md-icon>\r\n            </button>\r\n            <button md-mini-fab (click)=\"frame.sxc.manage.run('zone')\" title=\"{{ 'TemplatePicker.Zone' }}\">\r\n                <md-icon>extension</md-icon>\r\n            </button>\r\n        </div>\r\n    </div>\r\n    <div *ngIf=\"isContentApp\">\r\n        <md-select [(ngModel)]=\"contentTypeId\" (change)=\"updateContentTypeId($event)\" [disabled]=\"dashInfo.hasContent || dashInfo.isList\">\r\n            <md-option *ngFor=\"let type of contentTypes | contentTypeFilter\" [value]=\"type.StaticName\">\r\n                {{ type.Label }}\r\n            </md-option>\r\n        </md-select>\r\n    </div>\r\n    <div class=\"row\" fxLayout=\"row\" *ngIf=\"isContentApp ? contentTypeId != 0 : (savedAppId != 0)\">\r\n        <md-select [(ngModel)]=\"templateId\" (change)=\"updateTemplateId($event)\" [disabled]=\"templates.length == 0 && templateId\">\r\n            <md-option *ngFor=\"let template of templates | templateFilter:{ contentTypeId: contentTypeId, isContentApp: isContentApp }\" [value]=\"template.TemplateId\">\r\n                {{ template.Name }}\r\n            </md-option>\r\n        </md-select>\r\n        <button md-mini-fab *ngIf=\"templateId !== null\" (click)=\"frame.sxc.manage.contentBlock.persistTemplate(false, false)\" title=\"{{ 'TemplatePicker.Save' }}\">\r\n        <md-icon>check</md-icon>\r\n    </button>\r\n        <button md-mini-fab *ngIf=\"undoTemplateId !== null\" (click)=\"frame.sxc.manage.contentBlock._cancelTemplateChange()\" title=\"{{ 'TemplatePicker.' + (isContentApp ? 'Cancel' : 'Close') }}\">\r\n        <md-icon>close</md-icon>\r\n    </button>\r\n    </div>\r\n    <div *ngIf=\"showRemoteInstaller\">\r\n        <md-progress-bar *ngIf=\"showProgress\"></md-progress-bar>\r\n        <iframe class=\"fr-getting-started\" id=\"frGettingStarted\" [src]=\"remoteInstallerUrl\" width=\"100%\" height=\"300px\"></iframe>\r\n        <!--<div class=\"sc-loading\" id=\"pnlLoading\" *ngIf=\"progressIndicator.show\">\r\n            <i class=\"icon-eav-spinner animate-spin\"></i>\r\n            <span class=\"sc-loading-label\">\r\n                installing <span id=\"packageName\">{{ progressIndicator.label }}</span>\r\n            </span>\r\n        </div>-->\r\n    </div>\r\n</md-card>"

/***/ }),

/***/ 39:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_router__ = __webpack_require__(54);
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* ActivatedRoute */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* ActivatedRoute */]) === "function" && _a || Object])
], $2sxcService);

var _a;
//# sourceMappingURL=$2sxc.service.js.map

/***/ }),

/***/ 520:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(182);


/***/ })

},[520]);
//# sourceMappingURL=main.bundle.js.map