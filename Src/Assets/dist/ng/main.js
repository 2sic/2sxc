(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app.component.html":
/*!*********************************************************************************************************************!*\
  !*** ./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app.component.html ***!
  \*********************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<app-template-picker *ngIf=\"name === 'dash-view' || name === 'layout'\"></app-template-picker>");

/***/ }),

/***/ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/history/history.component.html":
/*!*********************************************************************************************************************************!*\
  !*** ./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/history/history.component.html ***!
  \*********************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<!-- dialog header -->\r\n<mat-toolbar color=\"primary\">\r\n  <span class=\"title\">{{\"ItemHistory.Title\" | translate}}</span>\r\n  <span class=\"spacer\"></span>\r\n  <button mat-dialog-close mat-icon-button>\r\n    <mat-icon class=\"example-icon\">close</mat-icon>\r\n  </button>\r\n</mat-toolbar>\r\n\r\n<!-- list of versions -->\r\n<div class=\"table\">\r\n  <div class=\"no-items\" *ngIf=\"sxcVersion.error | async\">{{'ItemHistory.NoHistory' | translate}}</div>\r\n  <div class=\"no-items\" *ngIf=\"(sxcVersion.versions | async)?.length === 0\">{{'ItemHistory.NoHistory' | translate}}</div>\r\n  <div class=\"record\" *ngFor=\"let version of sxcVersion.versions | async\">\r\n    <mat-expansion-panel>\r\n      <!-- version header -->\r\n      <mat-expansion-panel-header>\r\n        <mat-panel-title fxFlex=\"108px\">{{\"ItemHistory.Version\" | translate:({version:version.VersionNumber})}}</mat-panel-title>\r\n        <mat-panel-description>{{version.TimeStamp}}</mat-panel-description>\r\n      </mat-expansion-panel-header>\r\n\r\n      <!-- version body -->\r\n      <!-- use ng-template to ensure lazy initialization, otherwise it's slow with a lot of data -->\r\n      <ng-template matExpansionPanelContent>\r\n        <div class=\"detail\">\r\n          <div fxLayout=\"row\" *ngFor=\"let data of version.Data\" [class.changed]=\"data.hasChanged\">\r\n            <div fxFlex=\"160px\" class=\"label\">{{data.key}}:</div>\r\n            <!-- expandable value details -->\r\n            <div fxFlex [class.expand]=\"data.expand\" class=\"value\" title=\"expand content\" (click)=\"data.expand=!data.expand\">\r\n              <div class=\"lang-wrapper\" *ngFor=\"let val of data.value\">\r\n                <div *ngIf=\"data.value.length > 0\" class=\"lang\">{{val[0]}}</div>\r\n                <div [innerHTML]=\"val[1]\"></div>\r\n              </div>\r\n            </div>\r\n            <div flex=\"nogrow\" *ngIf=\"data.value.length > 1\">\r\n              <i *ngFor=\"let val of data.value\">[{{val[0]}}]&nbsp;</i>\r\n            </div>\r\n            <i flex=\"nogrow\">[{{data.type}}]</i>\r\n          </div>\r\n        </div>\r\n        <mat-action-row>\r\n          <button mat-button (click)=\"restoreLive(version)\">{{'ItemHistory.Buttons.RestoreLive' | translate}}</button>\r\n        </mat-action-row>\r\n      </ng-template>\r\n    </mat-expansion-panel>\r\n  </div>\r\n</div>\r\n\r\n");

/***/ }),

/***/ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/installer/installer.component.html":
/*!*************************************************************************************************************************************!*\
  !*** ./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/installer/installer.component.html ***!
  \*************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"progress\" *ngIf=\"showProgress\">\r\n  <mat-progress-spinner [mode]=\"'indeterminate'\"></mat-progress-spinner>\r\n  <span>Installing {{ currentPackage?.displayName }}..</span>\r\n</div>\r\n<div *ngIf=\"ready\">\r\n  <iframe class=\"fr-getting-started\" id=\"frGettingStarted\" [src]=\"remoteInstallerUrl\" width=\"100%\" height=\"300px\"></iframe>\r\n</div>\r\n");

/***/ }),

/***/ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/template-picker/template-picker.component.html":
/*!*************************************************************************************************************************************************!*\
  !*** ./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/template-picker/template-picker.component.html ***!
  \*************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\r\n<div class=\"content\">\r\n\r\n  <!-- debug info section -->\r\n  <div *ngIf=\"showDebug\">\r\n    <h4>Debug</h4>\r\n    <ul>\r\n      <li>Types: {{ types?.length }}, current: {{contentType?.StaticName}}</li>\r\n      <li>Apps: {{ (apps$ | async)?.length }}, current: {{app?.AppId }} </li>\r\n      <li>Templates: {{templates?.length}}, current: {{template?.TemplateId}}</li>\r\n      <li>Config: isContent='{{isContent}}' Ready: {{ready}} </li>\r\n      <li>Tab to show: '{{tabIndex}}' / preventTypeSwitch '{{preventTypeSwitch}}' / preventAppSwitch '{{preventAppSwich}}'</li>\r\n    </ul>\r\n  </div>\r\n\r\n  <!-- loading indicator -->\r\n  <mat-progress-bar [ngStyle]=\"{ opacity: ready ? 0 : 1 }\" [mode]=\"'indeterminate'\"></mat-progress-bar>\r\n\r\n  <!-- main dialog, starting with save/cancel button -->\r\n  <div class=\"card\"\r\n    [ngClass]=\"{ blocked: !ready }\"\r\n  >\r\n    <div class=\"top-controls\" fxLayout=\"row\" fxLayoutAlign=\"center center\">\r\n      <button mat-fab *ngIf=\"template\" (click)=\"persistTemplate(template)\" [attr.title]=\"'TemplatePicker.Save' | translate\">\r\n        <mat-icon>check</mat-icon>\r\n      </button>\r\n      <button mat-mini-fab class=\"secondary\" *ngIf=\"showCancel\" (click)=\"cancel()\" [attr.title]=\"('TemplatePicker.' + (isContent ? 'Cancel' : 'Close')) | translate\">\r\n        <mat-icon>close</mat-icon>\r\n      </button>\r\n    </div>\r\n\r\n    <!-- tabs -->\r\n    <mat-tab-group [(selectedIndex)]=\"tabIndex\">\r\n      <mat-tab>\r\n        <ng-template mat-tab-label>\r\n          {{(isContent \r\n            ? (contentType?.Name || ('TemplatePicker.ContentTypePickerDefault' | translate)) \r\n            : (app?.Name || ('TemplatePicker.AppPickerDefault' | translate)))}}\r\n        </ng-template>\r\n\r\n        <!-- App Selector -->\r\n        <div *ngIf=\"!isContent; else contentApp\" class=\"tiles\">\r\n\r\n          <div class=\"tile\" \r\n            [ngClass]=\"{ active: app?.AppId === a.AppId, blocked: preventTypeSwitch }\" \r\n            [attr.title]=\"a.Name\" \r\n            (click)=\"selectApp(app, a)\"\r\n            (dblclick)=\"switchTab()\" *ngFor=\"let a of apps$ | async\">\r\n            <div class=\"bg\">\r\n              <img *ngIf=\"a.Thumbnail !== null && a.Thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"a.Thumbnail + '?w=176&h=176'\">\r\n              <div *ngIf=\"a.Thumbnail === null || a.Thumbnail === ''\" class=\"bg-icon\">\r\n                <mat-icon>star</mat-icon>\r\n              </div>\r\n            </div>\r\n            <div class=\"version\"><span>v{{a.VersionMain}}</span></div>\r\n\r\n            <div class=\"title\" [ngClass]=\"{ show: a.Thumbnail === null || a.Thumbnail === '' }\">\r\n              <span>{{a.Name}}</span>\r\n            </div>\r\n          </div>\r\n\r\n          <!-- install and manage buttons -->\r\n          <div class=\"tile config\" *ngIf=\"showAdvanced\" (click)=\"run('app-import')\" [attr.title]=\"'TemplatePicker.Install' | translate\">\r\n            <div class=\"bg\">\r\n              <div class=\"bg-icon\">\r\n                <mat-icon>get_app</mat-icon>\r\n              </div>\r\n            </div>\r\n            <div class=\"title show\">\r\n              <span>{{\"TemplatePicker.Install\" | translate}}</span>\r\n            </div>\r\n          </div>\r\n          <div class=\"tile config\" *ngIf=\"showAdvanced\" (click)=\"run('zone')\" [attr.title]=\"'TemplatePicker.Zone' | translate\">\r\n            <div class=\"bg\">\r\n              <div class=\"bg-icon\">\r\n                <mat-icon>apps</mat-icon>\r\n              </div>\r\n            </div>\r\n            <div class=\"title show\">\r\n              <span>{{\"TemplatePicker.Zone\" | translate}}</span>\r\n            </div>\r\n          </div>\r\n        </div>\r\n\r\n        <!-- Content-Type selection (when not a generic app, but the default content-app -->\r\n        <ng-template #contentApp>\r\n          <div class=\"tiles\">\r\n            <div mat-button class=\"tile\" \r\n              [ngClass]=\"{ active: contentType?.StaticName === c.StaticName, blocked: preventTypeSwitch }\"\r\n              [attr.title]=\"(c.Label | translate) + (showDebug ? ' (' + c.StaticName + ')' : '')\" \r\n              (click)=\"selectContentType(contentType, c)\"\r\n              (dblclick)=\"switchTab()\" \r\n              *ngFor=\"let c of types\"\r\n            >\r\n              <div class=\"bg\">\r\n                <img *ngIf=\"c.Thumbnail !== null && c.Thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"c.Thumbnail + '?w=176&h=176'\">\r\n                <div *ngIf=\"c.Thumbnail === null || c.Thumbnail === ''\" class=\"bg-icon\">\r\n                  <mat-icon>bubble_chart</mat-icon>\r\n                </div>\r\n              </div>\r\n              <div class=\"title\" [ngClass]=\"{ show: c.Thumbnail === null || c.Thumbnail === '' }\">\r\n                <span>{{c.Label}}</span>\r\n              </div>\r\n            </div>\r\n          </div>\r\n        </ng-template>\r\n      </mat-tab>\r\n\r\n      <!-- template selection after app/content-type selection -->\r\n      <mat-tab *ngIf=\"isContent ? contentType : app\" [label]=\"('TemplatePicker.ChangeView' | translate) + '(' + templates.length + ')'\">\r\n        <div class=\"tiles\">\r\n          <mat-spinner class=\"templates-spinner\" *ngIf=\"templatesLoading$ | async\"></mat-spinner>\r\n          <div class=\"tile\" \r\n            [ngClass]=\"{ active: template?.TemplateId === t.TemplateId }\" \r\n            [attr.title]=\"t.Name + (showDebug ? ' (' + t.TemplateId + ')' : '')\" \r\n            (click)=\"selectTemplate(t)\"\r\n            *ngFor=\"let t of templates\">\r\n            <div class=\"bg\">\r\n              <img *ngIf=\"t.Thumbnail !== null && t.Thumbnail !== ''\" class=\"bg-img\" [attr.src]=\"t.Thumbnail + '?w=176&h=176'\">\r\n              <div *ngIf=\"t.Thumbnail === null || t.Thumbnail === ''\" class=\"bg-icon\">\r\n                <mat-icon *ngIf=\"isContent\">view_carousel</mat-icon>\r\n                <mat-icon *ngIf=\"!isContent\">view_quilt</mat-icon>\r\n              </div>\r\n            </div>\r\n            <div class=\"title\" [ngClass]=\"{ show: t.Thumbnail === null || t.Thumbnail === '' }\">\r\n              <span>{{t.Name}}</span>\r\n            </div>\r\n          </div>\r\n          <div class=\"tile config\" *ngIf=\"showAdvanced && !isContent && app?.AppId !== null\" (click)=\"run('app')\" [attr.title]=\"'TemplatePicker.App' | translate\">\r\n            <div class=\"bg\">\r\n              <div class=\"bg-icon\">\r\n                <mat-icon>settings</mat-icon>\r\n              </div>\r\n            </div>\r\n            <div class=\"title show\">\r\n              <span>{{\"TemplatePicker.App\" | translate}}</span>\r\n            </div>\r\n          </div>\r\n        </div>\r\n      </mat-tab>\r\n    </mat-tab-group>\r\n\r\n    <span class=\"no-install-allowed\" *ngIf=\"isBadContextForInstaller && showInstaller\">No {{isContent ? 'Content Apps' : 'Apps'}} installed yet. Please persue the installation by creating a new {{isContent ? 'Content' : 'App'}} in the root of your website.</span>\r\n    <app-installer *ngIf=\"!isBadContextForInstaller && showInstaller\" [isContentApp]=\"isContent\"></app-installer>\r\n  </div>\r\n</div>\r\n");

/***/ }),

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
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (":host {\r\n    display: block;\r\n}\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvYXBwLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxjQUFjO0FBQ2xCIiwiZmlsZSI6InNyYy9hcHAvYXBwLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyI6aG9zdCB7XHJcbiAgICBkaXNwbGF5OiBibG9jaztcclxufVxyXG4iXX0= */");

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
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm2015/ngx-translate-core.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var app_history_history_component__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! app/history/history.component */ "./src/app/history/history.component.ts");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "./node_modules/@angular/material/fesm2015/dialog.js");
/* harmony import */ var _core_log__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./core/log */ "./src/app/core/log.ts");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "./node_modules/@2sic.com/dnn-sxc-angular/fesm2015/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _config__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./config */ "./src/app/config.ts");
/* harmony import */ var _i18n__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./i18n */ "./src/app/i18n/index.ts");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
/* harmony import */ var app_core_constants__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! app/core/constants */ "./src/app/core/constants.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __importDefault = (undefined && undefined.__importDefault) || function (mod) {
  return (mod && mod.__esModule) ? mod : { "default": mod };
};










var AppComponent = /** @class */ (function (_super) {
    __extends(AppComponent, _super);
    function AppComponent(translate, dialog, el, context, http) {
        var _this = _super.call(this, el, context.preConfigure({ sxc: _config__WEBPACK_IMPORTED_MODULE_6__["Config"].getSxcInstance() }), false) || this;
        _this.translate = translate;
        _this.dialog = dialog;
        translate.addLangs(_i18n__WEBPACK_IMPORTED_MODULE_7__["SupportedLanguages"]);
        http.get(app_core_constants__WEBPACK_IMPORTED_MODULE_9__["Constants"].webApiDialogContext + "?appId=" + _config__WEBPACK_IMPORTED_MODULE_6__["Config"].appId())
            .subscribe(function (ctxDto) {
            var lang = ctxDto.Context.Language;
            translate.setDefaultLang(_i18n__WEBPACK_IMPORTED_MODULE_7__["PrimaryUiLanguage"]);
            translate.use(Object(_i18n__WEBPACK_IMPORTED_MODULE_7__["langCode2"])(lang.Current));
            _this.showDialog();
        });
        return _this;
    }
    AppComponent.prototype.showDialog = function () {
        this.name = _config__WEBPACK_IMPORTED_MODULE_6__["Config"].dialog();
        _core_log__WEBPACK_IMPORTED_MODULE_4__["log"].add("loading '" + this.name + "'");
        var frame = window.frameElement;
        if (this.name === 'item-history') {
            this.dialog.open(app_history_history_component__WEBPACK_IMPORTED_MODULE_2__["HistoryComponent"]).afterClosed()
                .subscribe(function () { return frame.bridge.hide(); });
        }
    };
    AppComponent.ctorParameters = function () { return [
        { type: _ngx_translate_core__WEBPACK_IMPORTED_MODULE_0__["TranslateService"] },
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialog"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_5__["Context"] },
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_8__["HttpClient"] }
    ]; };
    AppComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-root',
            template: __importDefault(__webpack_require__(/*! raw-loader!./app.component.html */ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app.component.html")).default,
            styles: [__importDefault(__webpack_require__(/*! ./app.component.css */ "./src/app/app.component.css")).default]
        }),
        __metadata("design:paramtypes", [_ngx_translate_core__WEBPACK_IMPORTED_MODULE_0__["TranslateService"],
            _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialog"],
            _angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"],
            _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_5__["Context"],
            _angular_common_http__WEBPACK_IMPORTED_MODULE_8__["HttpClient"]])
    ], AppComponent);
    return AppComponent;
}(_2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_5__["DnnAppComponent"]));



/***/ }),

/***/ "./src/app/app.module.ts":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: HttpLoaderFactory, AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HttpLoaderFactory", function() { return HttpLoaderFactory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var _ngx_translate_http_loader__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngx-translate/http-loader */ "./node_modules/@ngx-translate/http-loader/fesm2015/ngx-translate-http-loader.js");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm2015/ngx-translate-core.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser */ "./node_modules/@angular/platform-browser/fesm2015/platform-browser.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm2015/forms.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "./node_modules/@2sic.com/dnn-sxc-angular/fesm2015/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _app_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./app.component */ "./src/app/app.component.ts");
/* harmony import */ var app_template_picker_template_picker_module__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! app/template-picker/template-picker.module */ "./src/app/template-picker/template-picker.module.ts");
/* harmony import */ var app_history_version_dialog_module__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! app/history/version-dialog.module */ "./src/app/history/version-dialog.module.ts");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
/* harmony import */ var _material_module__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./material-module */ "./src/app/material-module.ts");
/* harmony import */ var _core_log__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./core/log */ "./src/app/core/log.ts");
/* harmony import */ var _i18n__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./i18n */ "./src/app/i18n/index.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};













function HttpLoaderFactory(http) {
    var loader = new _ngx_translate_http_loader__WEBPACK_IMPORTED_MODULE_0__["TranslateHttpLoader"](http, _i18n__WEBPACK_IMPORTED_MODULE_12__["pathToI18n"], _i18n__WEBPACK_IMPORTED_MODULE_12__["i18nExtension"]);
    _core_log__WEBPACK_IMPORTED_MODULE_11__["log"].add('created translate-loader', loader);
    return loader;
}
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_3__["NgModule"])({
            declarations: [
                _app_component__WEBPACK_IMPORTED_MODULE_6__["AppComponent"]
            ],
            exports: [],
            imports: [
                _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["BrowserModule"],
                _angular_common_http__WEBPACK_IMPORTED_MODULE_9__["HttpClientModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_4__["FormsModule"],
                app_template_picker_template_picker_module__WEBPACK_IMPORTED_MODULE_7__["TemplatePickerModule"],
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_1__["TranslateModule"].forRoot({
                    loader: {
                        provide: _ngx_translate_core__WEBPACK_IMPORTED_MODULE_1__["TranslateLoader"],
                        useFactory: HttpLoaderFactory,
                        deps: [_angular_common_http__WEBPACK_IMPORTED_MODULE_9__["HttpClient"]]
                    }
                }),
                _material_module__WEBPACK_IMPORTED_MODULE_10__["MaterialModule"],
                app_history_version_dialog_module__WEBPACK_IMPORTED_MODULE_8__["VersionDialogModule"],
            ],
            providers: [_2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_5__["DnnInterceptor"]
            ],
            bootstrap: [_app_component__WEBPACK_IMPORTED_MODULE_6__["AppComponent"]]
        })
    ], AppModule);
    return AppModule;
}());



/***/ }),

/***/ "./src/app/config.ts":
/*!***************************!*\
  !*** ./src/app/config.ts ***!
  \***************************/
/*! exports provided: Config */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Config", function() { return Config; });
var Config = /** @class */ (function () {
    function Config() {
    }
    Config.appId = function () { return get('appId'); };
    Config.apps = function () { return get('apps'); };
    Config.item = function () { return JSON.parse(req('items'))[0]; };
    Config.moduleId = function () { return Number(req('mid')); };
    Config.cbId = function () { return Number(req('cbid')); };
    Config.dialog = function () { return req('dialog'); };
    Config.getSxcInstance = function () { return $2sxc(Config.moduleId(), Config.cbId()); };
    return Config;
}());

function req(key) { return $2sxc.urlParams.require(key); }
function get(key) { return $2sxc.urlParams.get(key); }


/***/ }),

/***/ "./src/app/core/app.ts":
/*!*****************************!*\
  !*** ./src/app/core/app.ts ***!
  \*****************************/
/*! exports provided: App */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "App", function() { return App; });
var App = /** @class */ (function () {
    function App(json) {
        Object.assign(this, json);
        try {
            this.VersionMain = parseInt(this.Version.substr(0, 2));
        }
        catch (e) { /* ignore */ }
    }
    return App;
}());



/***/ }),

/***/ "./src/app/core/behavior-observable.ts":
/*!*********************************************!*\
  !*** ./src/app/core/behavior-observable.ts ***!
  \*********************************************/
/*! exports provided: BehaviorObservable */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BehaviorObservable", function() { return BehaviorObservable; });
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();

// Todo: probably not needed any more, since now Subjects are automatically observables
var BehaviorObservable = /** @class */ (function (_super) {
    __extends(BehaviorObservable, _super);
    function BehaviorObservable() {
        return _super.call(this) || this;
    }
    BehaviorObservable.create = function (initialValue) {
        var subj = new rxjs__WEBPACK_IMPORTED_MODULE_0__["BehaviorSubject"](initialValue);
        var obs = subj.asObservable();
        obs.initialValue = initialValue;
        obs.subject = subj;
        obs.reset = function () {
            obs.subject.next(obs.initialValue);
        };
        obs.isInitial = function () {
            return obs.subject.value === obs.initialValue;
        };
        obs.next = function (value) { return obs.subject.next(value); };
        return obs;
    };
    return BehaviorObservable;
}(rxjs__WEBPACK_IMPORTED_MODULE_0__["Observable"]));



/***/ }),

/***/ "./src/app/core/boot-control.ts":
/*!**************************************!*\
  !*** ./src/app/core/boot-control.ts ***!
  \**************************************/
/*! exports provided: BootController */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BootController", function() { return BootController; });
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var _log__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./log */ "./src/app/core/log.ts");


var log = _log__WEBPACK_IMPORTED_MODULE_1__["log"].subLog('boot-controller');
/**
 * Special reboot controller, to restart the angular app
 * when critical parameters were changed
 */
var BootController = /** @class */ (function () {
    function BootController() {
        this._reboot = new rxjs__WEBPACK_IMPORTED_MODULE_0__["Subject"]();
        this.rebootRequest$ = this._reboot.asObservable();
    }
    BootController.getRebootController = function () {
        log.add('getRebootController()');
        if (!BootController.instance) {
            BootController.instance = new BootController();
        }
        return BootController.instance;
    };
    BootController.prototype.reboot = function () {
        log.add('restarting...');
        this._reboot.next(true);
    };
    return BootController;
}());



/***/ }),

/***/ "./src/app/core/constants.ts":
/*!***********************************!*\
  !*** ./src/app/core/constants.ts ***!
  \***********************************/
/*! exports provided: Constants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Constants", function() { return Constants; });
var Constants = {
    logName: 'quick-edit',
    //#region WebApi Endpoints used: 2sxc
    webApiDialogContext: 'app-sys/system/dialogsettings',
    webApiInstallPackage: 'app-sys/installer/installpackage',
    webApiRemoteInstaller: 'view/Module/RemoteInstallDialogUrl',
    webApiGetTemplates: 'view/Module/GetSelectableTemplates',
    webApiGetTypes: 'view/Module/GetSelectableContentTypes',
    webApiGetApps: 'view/Module/GetSelectableApps',
    webApiSetApp: 'view/Module/SetAppId',
};


/***/ }),

/***/ "./src/app/core/core.module.ts":
/*!*************************************!*\
  !*** ./src/app/core/core.module.ts ***!
  \*************************************/
/*! exports provided: CoreModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CoreModule", function() { return CoreModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm2015/common.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
/* harmony import */ var app_installer_getting_started_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! app/installer/getting-started.service */ "./src/app/installer/getting-started.service.ts");
/* harmony import */ var app_template_picker_picker_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! app/template-picker/picker.service */ "./src/app/template-picker/picker.service.ts");
/* harmony import */ var app_template_picker_current_data_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! app/template-picker/current-data.service */ "./src/app/template-picker/current-data.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};






var CoreModule = /** @class */ (function () {
    function CoreModule() {
    }
    CoreModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_1__["CommonModule"],
                _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClientModule"]
            ],
            declarations: [],
            providers: [
                app_installer_getting_started_service__WEBPACK_IMPORTED_MODULE_3__["GettingStartedService"],
                app_template_picker_picker_service__WEBPACK_IMPORTED_MODULE_4__["PickerService"],
                app_template_picker_current_data_service__WEBPACK_IMPORTED_MODULE_5__["CurrentDataService"],
            ]
        })
    ], CoreModule);
    return CoreModule;
}());



/***/ }),

/***/ "./src/app/core/log.ts":
/*!*****************************!*\
  !*** ./src/app/core/log.ts ***!
  \*****************************/
/*! exports provided: Log, log */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Log", function() { return Log; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "log", function() { return log; });
/* harmony import */ var _constants__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./constants */ "./src/app/core/constants.ts");
/* harmony import */ var app_debug_config__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! app/debug-config */ "./src/app/debug-config.ts");


var Log = /** @class */ (function () {
    function Log(name, parent) {
        this.name = '';
        this.autoDump = app_debug_config__WEBPACK_IMPORTED_MODULE_1__["DebugConfig"].logger.logToConsole;
        this.logs = new Array();
        this.loggers = new Object();
        this.name = name;
        this.parent = parent;
    }
    Log.prototype.add = function (msg) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        this.logs.push(msg);
        if (this.name)
            msg = this.name + ':' + msg;
        if (this.parent)
            this.parent.logs.push(msg);
        if (this.autoDump || Log.forceLogToConsole) {
            arguments[0] = _constants__WEBPACK_IMPORTED_MODULE_0__["Constants"].logName + ': ' + msg;
            console.log.apply(null, arguments);
        }
    };
    Log.prototype.subLog = function (name, autoDump) {
        var newLog = new Log(name, this);
        this.loggers[name] = newLog;
        newLog.autoDump = typeof (autoDump) === 'boolean'
            ? autoDump : this.autoDump;
        if (app_debug_config__WEBPACK_IMPORTED_MODULE_1__["DebugConfig"].logger.internals)
            this.add("logger: subLog(" + name + ", " + autoDump + ") resulting in autoDump=" + newLog.autoDump);
        return newLog;
    };
    Log.prototype.dump = function () {
        this.add('dumping to console');
        console.log("Log dump for '" + this.name + "'", this);
    };
    Log.configureRuntimeLogging = function (state) {
        if (app_debug_config__WEBPACK_IMPORTED_MODULE_1__["DebugConfig"].logger.urlDebugActivatesLive)
            Log.forceLogToConsole = state;
    };
    /** global state to determine if logging to console should be done or not */
    Log.forceLogToConsole = false;
    return Log;
}());

var log = new Log();
window['logger'] = log;


/***/ }),

/***/ "./src/app/debug-config.ts":
/*!*********************************!*\
  !*** ./src/app/debug-config.ts ***!
  \*********************************/
/*! exports provided: DebugConfig */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DebugConfig", function() { return DebugConfig; });
/** configuration what to debug directly to the screen and what to keep secret */
var DebugConfig = {
    /** logger configuration */
    logger: {
        /** should we stream messages directly to console */
        logToConsole: false,
        /** should we also log internal events */
        internals: false,
        /** allow url param ?debug=true to turn on logging */
        urlDebugEnablesAll: true,
        /** if url param ?debug=true also enables all live-logging */
        urlDebugActivatesLive: true
    },
    /** api debugging */
    api: {
        enabled: false,
        streams: false,
    },
    /** template picker */
    picker: {
        enabled: false,
        streams: false,
        showDebugPanel: false
    },
    /** template state */
    state: {
        enabled: false,
        streams: false,
        inits: false,
    },
    /** content-type processor */
    typeProcessor: false,
    /** template processor */
    templateProcessor: false,
};


/***/ }),

/***/ "./src/app/history/history.component.scss":
/*!************************************************!*\
  !*** ./src/app/history/history.component.scss ***!
  \************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (":host {\n  width: 1000px;\n  max-width: 100%;\n  display: block;\n}\n\n:host mat-toolbar {\n  background: transparent;\n  color: black;\n}\n\n:host mat-toolbar .spacer {\n  flex: 1 1 auto;\n}\n\n:host mat-toolbar .title {\n  font-weight: 300;\n}\n\n:host .table {\n  padding: 8px;\n}\n\n:host .table .no-items {\n  color: rgba(0, 0, 0, 0.6);\n  text-align: center;\n  margin: 16px;\n}\n\n:host .table .header {\n  line-height: 48px;\n  font-weight: 400;\n  padding: 0 22px;\n  color: rgba(0, 0, 0, 0.6);\n}\n\n:host .table .record mat-expansion-panel {\n  transition: box-shadow 280ms cubic-bezier(0.4, 0, 0.2, 1), margin 280ms ease;\n}\n\n:host .table .record mat-expansion-panel.mat-expanded {\n  margin: 16px 0 !important;\n}\n\n:host .table .record mat-expansion-panel .detail {\n  line-height: 28px;\n  box-sizing: border-box;\n}\n\n:host .table .record mat-expansion-panel .detail > div {\n  border-bottom: 1px solid rgba(0, 0, 0, 0.1);\n  margin-bottom: 4px;\n  padding: 4px 0;\n}\n\n:host .table .record mat-expansion-panel .detail > div.changed .label,\n:host .table .record mat-expansion-panel .detail > div.changed .value {\n  color: #2196F3;\n}\n\n:host .table .record mat-expansion-panel .detail > div:last-of-type {\n  border-bottom: none;\n}\n\n:host .table .record mat-expansion-panel .detail > div .label {\n  vertical-align: top;\n  color: rgba(0, 0, 0, 0.6);\n  height: 28px;\n}\n\n:host .table .record mat-expansion-panel .detail > div .value {\n  cursor: pointer;\n  vertical-align: top;\n  height: 28px;\n  overflow: hidden;\n  display: inline-block;\n  white-space: nowrap;\n  text-overflow: ellipsis;\n}\n\n:host .table .record mat-expansion-panel .detail > div .value .lang {\n  display: none;\n}\n\n:host .table .record mat-expansion-panel .detail > div .value.expand {\n  height: auto;\n}\n\n:host .table .record mat-expansion-panel .detail > div .value.expand .lang-wrapper {\n  position: relative;\n  padding: 8px 0;\n}\n\n:host .table .record mat-expansion-panel .detail > div .value.expand .lang-wrapper .lang {\n  display: block;\n  position: absolute;\n  font-size: 10pt;\n  left: 0;\n  top: 0;\n  color: rgba(0, 0, 0, 0.6);\n  line-height: 12px;\n  font-style: italic;\n}\n\n:host .table .record mat-expansion-panel .detail > div .value ::ng-deep * {\n  margin: 0;\n}\n\n:host .table .record mat-expansion-panel .detail > div i {\n  vertical-align: top;\n  height: 28px;\n  color: rgba(0, 0, 0, 0.6);\n  text-align: right;\n  font-size: 8pt;\n}\n\n:host .table .record mat-expansion-panel mat-action-row button {\n  margin-left: 8px;\n  text-transform: uppercase;\n}\n\n:host footer {\n  padding: 16px 22px;\n}\n\n:host footer button {\n  margin-left: 8px;\n}\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvaGlzdG9yeS9oaXN0b3J5LmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0ksYUFBYTtFQUNiLGVBQWU7RUFDZixjQUFjO0FBQ2xCOztBQUpBO0VBS1EsdUJBQXVCO0VBQ3ZCLFlBQXVCO0FBRy9COztBQVRBO0VBUVksY0FBYztBQUsxQjs7QUFiQTtFQVdZLGdCQUFnQjtBQU01Qjs7QUFqQkE7RUFlUSxZQUFZO0FBTXBCOztBQXJCQTtFQWlCWSx5QkFBd0I7RUFDeEIsa0JBQWtCO0VBQ2xCLFlBQVk7QUFReEI7O0FBM0JBO0VBc0JZLGlCQUFpQjtFQUNqQixnQkFBZ0I7RUFDaEIsZUFBZTtFQUNmLHlCQUF3QjtBQVNwQzs7QUFsQ0E7RUE2QmdCLDRFQUEwRTtBQVMxRjs7QUF0Q0E7RUErQm9CLHlCQUF5QjtBQVc3Qzs7QUExQ0E7RUFrQ29CLGlCQUFpQjtFQUNqQixzQkFBc0I7QUFZMUM7O0FBL0NBO0VBcUN3QiwyQ0FBMEM7RUFDMUMsa0JBQWtCO0VBQ2xCLGNBQWM7QUFjdEM7O0FBckRBOztFQTJDZ0MsY0FBYztBQWU5Qzs7QUExREE7RUErQzRCLG1CQUFtQjtBQWUvQzs7QUE5REE7RUFrRDRCLG1CQUFtQjtFQUNuQix5QkFBd0I7RUFDeEIsWUFBWTtBQWdCeEM7O0FBcEVBO0VBdUQ0QixlQUFlO0VBQ2YsbUJBQW1CO0VBQ25CLFlBQVk7RUFDWixnQkFBZ0I7RUFDaEIscUJBQXFCO0VBQ3JCLG1CQUFtQjtFQUNuQix1QkFBdUI7QUFpQm5EOztBQTlFQTtFQStEZ0MsYUFBYTtBQW1CN0M7O0FBbEZBO0VBa0VnQyxZQUFZO0FBb0I1Qzs7QUF0RkE7RUFvRW9DLGtCQUFrQjtFQUNsQixjQUFjO0FBc0JsRDs7QUEzRkE7RUF1RXdDLGNBQWM7RUFDZCxrQkFBa0I7RUFDbEIsZUFBZTtFQUNmLE9BQU87RUFDUCxNQUFNO0VBQ04seUJBQXdCO0VBQ3hCLGlCQUFpQjtFQUNqQixrQkFBa0I7QUF3QjFEOztBQXRHQTtFQW1GZ0MsU0FBUztBQXVCekM7O0FBMUdBO0VBdUY0QixtQkFBbUI7RUFDbkIsWUFBWTtFQUNaLHlCQUF3QjtFQUN4QixpQkFBaUI7RUFDakIsY0FBYztBQXVCMUM7O0FBbEhBO0VBaUd3QixnQkFBZ0I7RUFDaEIseUJBQXlCO0FBcUJqRDs7QUF2SEE7RUF5R1Esa0JBQWtCO0FBa0IxQjs7QUEzSEE7RUEyR1ksZ0JBQWdCO0FBb0I1QiIsImZpbGUiOiJzcmMvYXBwL2hpc3RvcnkvaGlzdG9yeS5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIjpob3N0IHtcclxuICAgIHdpZHRoOiAxMDAwcHg7XHJcbiAgICBtYXgtd2lkdGg6IDEwMCU7XHJcbiAgICBkaXNwbGF5OiBibG9jaztcclxuICAgIG1hdC10b29sYmFyIHtcclxuICAgICAgICBiYWNrZ3JvdW5kOiB0cmFuc3BhcmVudDtcclxuICAgICAgICBjb2xvcjogcmdiYSgwLCAwLCAwLCAxKTtcclxuICAgICAgICAuc3BhY2VyIHtcclxuICAgICAgICAgICAgZmxleDogMSAxIGF1dG87XHJcbiAgICAgICAgfVxyXG4gICAgICAgIC50aXRsZSB7XHJcbiAgICAgICAgICAgIGZvbnQtd2VpZ2h0OiAzMDA7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgLnRhYmxlIHtcclxuICAgICAgICBwYWRkaW5nOiA4cHg7XHJcbiAgICAgICAgLm5vLWl0ZW1zIHtcclxuICAgICAgICAgICAgY29sb3I6IHJnYmEoMCwgMCAsMCwgLjYpO1xyXG4gICAgICAgICAgICB0ZXh0LWFsaWduOiBjZW50ZXI7XHJcbiAgICAgICAgICAgIG1hcmdpbjogMTZweDtcclxuICAgICAgICB9XHJcbiAgICAgICAgLmhlYWRlciB7XHJcbiAgICAgICAgICAgIGxpbmUtaGVpZ2h0OiA0OHB4O1xyXG4gICAgICAgICAgICBmb250LXdlaWdodDogNDAwO1xyXG4gICAgICAgICAgICBwYWRkaW5nOiAwIDIycHg7XHJcbiAgICAgICAgICAgIGNvbG9yOiByZ2JhKDAsIDAsIDAsIC42KTtcclxuICAgICAgICB9XHJcbiAgICAgICAgLnJlY29yZCB7XHJcbiAgICAgICAgICAgIG1hdC1leHBhbnNpb24tcGFuZWwge1xyXG4gICAgICAgICAgICAgICAgdHJhbnNpdGlvbjogYm94LXNoYWRvdyAyODBtcyBjdWJpYy1iZXppZXIoLjQsIDAsIC4yLCAxKSwgbWFyZ2luIDI4MG1zIGVhc2U7XHJcbiAgICAgICAgICAgICAgICAmLm1hdC1leHBhbmRlZCB7XHJcbiAgICAgICAgICAgICAgICAgICAgbWFyZ2luOiAxNnB4IDAgIWltcG9ydGFudDtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIC5kZXRhaWwge1xyXG4gICAgICAgICAgICAgICAgICAgIGxpbmUtaGVpZ2h0OiAyOHB4O1xyXG4gICAgICAgICAgICAgICAgICAgIGJveC1zaXppbmc6IGJvcmRlci1ib3g7XHJcbiAgICAgICAgICAgICAgICAgICAgPmRpdiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGJvcmRlci1ib3R0b206IDFweCBzb2xpZCByZ2JhKDAsIDAsIDAsIC4xKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbWFyZ2luLWJvdHRvbTogNHB4O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYWRkaW5nOiA0cHggMDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgJi5jaGFuZ2VkIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC5sYWJlbCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC52YWx1ZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgY29sb3I6ICMyMTk2RjM7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgJjpsYXN0LW9mLXR5cGUge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYm9yZGVyLWJvdHRvbTogbm9uZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAubGFiZWwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdmVydGljYWwtYWxpZ246IHRvcDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiByZ2JhKDAsIDAsIDAsIC42KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlaWdodDogMjhweDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAudmFsdWUge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY3Vyc29yOiBwb2ludGVyO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdmVydGljYWwtYWxpZ246IHRvcDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlaWdodDogMjhweDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG92ZXJmbG93OiBoaWRkZW47XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBkaXNwbGF5OiBpbmxpbmUtYmxvY2s7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aGl0ZS1zcGFjZTogbm93cmFwO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGV4dC1vdmVyZmxvdzogZWxsaXBzaXM7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAubGFuZyB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGlzcGxheTogbm9uZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICYuZXhwYW5kIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWlnaHQ6IGF1dG87XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgLmxhbmctd3JhcHBlciB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGFkZGluZzogOHB4IDA7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIC5sYW5nIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRpc3BsYXk6IGJsb2NrO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcG9zaXRpb246IGFic29sdXRlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZm9udC1zaXplOiAxMHB0O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgbGVmdDogMDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRvcDogMDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiByZ2JhKDAsIDAsIDAsIC42KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxpbmUtaGVpZ2h0OiAxMnB4O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZm9udC1zdHlsZTogaXRhbGljO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgOjpuZy1kZWVwICoge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1hcmdpbjogMDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZlcnRpY2FsLWFsaWduOiB0b3A7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWlnaHQ6IDI4cHg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb2xvcjogcmdiYSgwLCAwLCAwLCAuNik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZXh0LWFsaWduOiByaWdodDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZvbnQtc2l6ZTogOHB0O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgbWF0LWFjdGlvbi1yb3cge1xyXG4gICAgICAgICAgICAgICAgICAgIGJ1dHRvbiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1hcmdpbi1sZWZ0OiA4cHg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRleHQtdHJhbnNmb3JtOiB1cHBlcmNhc2U7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgZm9vdGVyIHtcclxuICAgICAgICBwYWRkaW5nOiAxNnB4IDIycHg7XHJcbiAgICAgICAgYnV0dG9uIHtcclxuICAgICAgICAgICAgbWFyZ2luLWxlZnQ6IDhweDtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn1cclxuIl19 */");

/***/ }),

/***/ "./src/app/history/history.component.ts":
/*!**********************************************!*\
  !*** ./src/app/history/history.component.ts ***!
  \**********************************************/
/*! exports provided: HistoryComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "HistoryComponent", function() { return HistoryComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _sxc_versions_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./sxc-versions.service */ "./src/app/history/sxc-versions.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __importDefault = (undefined && undefined.__importDefault) || function (mod) {
  return (mod && mod.__esModule) ? mod : { "default": mod };
};


var HistoryComponent = /** @class */ (function () {
    // versions: Version[] = [];
    // versionParam: any;
    function HistoryComponent(sxcVersion) {
        this.sxcVersion = sxcVersion;
    }
    HistoryComponent.prototype.restoreLive = function (version) {
        this.sxcVersion.restore(version.ChangeSetId)
            .subscribe(function (_) { return window.parent.location.reload(); });
    };
    HistoryComponent.ctorParameters = function () { return [
        { type: _sxc_versions_service__WEBPACK_IMPORTED_MODULE_1__["SxcVersionsService"] }
    ]; };
    HistoryComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-history',
            template: __importDefault(__webpack_require__(/*! raw-loader!./history.component.html */ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/history/history.component.html")).default,
            styles: [__importDefault(__webpack_require__(/*! ./history.component.scss */ "./src/app/history/history.component.scss")).default]
        }),
        __metadata("design:paramtypes", [_sxc_versions_service__WEBPACK_IMPORTED_MODULE_1__["SxcVersionsService"]])
    ], HistoryComponent);
    return HistoryComponent;
}());

// 2020-07-28 2dm turned off, not used anywhere ATM
// Might be activated some time, but not now
// restoreDraft(version) {
//   this.dialog.open(ConfirmRestoreDialog, {
//     data: { version, isDraft: true },
//   }).afterClosed()
//     .subscribe(res => res ? alert('restoring draft') : undefined);
// }
// }
//
// @Component({
//   selector: 'confirm-restore-dialog',
//   template: `
//     <div class="content">
//       <div class="title">Restoring {{data.isDraft ? 'draft' : 'live'}} to version <b>{{data.version.ChangeSetId}}</b>.</div>
//       <div fxLayout="row">
//         <button mat-button [mat-dialog-close]="false">abort</button>
//         <span fxFlex></span>
//         <button mat-raised-button [mat-dialog-close]="true">proceed</button>
//       </div>
//     </div>
//   `,
// })
// export class ConfirmRestoreDialog {
//   constructor(
//     public dialogRef: MatDialogRef<ConfirmRestoreDialog>,
//     @Inject(MAT_DIALOG_DATA) public data: any
//   ) { }
// }


/***/ }),

/***/ "./src/app/history/index.ts":
/*!**********************************!*\
  !*** ./src/app/history/index.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "webApiHistory", function() { return webApiHistory; });
/* harmony import */ var _version_dto__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./version-dto */ "./src/app/history/version-dto.ts");
/* harmony import */ var _version_dto__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(_version_dto__WEBPACK_IMPORTED_MODULE_0__);
/* harmony reexport (unknown) */ for(var __WEBPACK_IMPORT_KEY__ in _version_dto__WEBPACK_IMPORTED_MODULE_0__) if(["webApiHistory","default"].indexOf(__WEBPACK_IMPORT_KEY__) < 0) (function(key) { __webpack_require__.d(__webpack_exports__, key, function() { return _version_dto__WEBPACK_IMPORTED_MODULE_0__[key]; }) }(__WEBPACK_IMPORT_KEY__));
/* harmony import */ var _version__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./version */ "./src/app/history/version.ts");
/* harmony import */ var _version__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_version__WEBPACK_IMPORTED_MODULE_1__);
/* harmony reexport (unknown) */ for(var __WEBPACK_IMPORT_KEY__ in _version__WEBPACK_IMPORTED_MODULE_1__) if(["webApiHistory","default"].indexOf(__WEBPACK_IMPORT_KEY__) < 0) (function(key) { __webpack_require__.d(__webpack_exports__, key, function() { return _version__WEBPACK_IMPORTED_MODULE_1__[key]; }) }(__WEBPACK_IMPORT_KEY__));
var webApiHistory = "eav/entities";




/***/ }),

/***/ "./src/app/history/sxc-versions.service.ts":
/*!*************************************************!*\
  !*** ./src/app/history/sxc-versions.service.ts ***!
  \*************************************************/
/*! exports provided: SxcVersionsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SxcVersionsService", function() { return SxcVersionsService; });
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var _config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../config */ "./src/app/config.ts");
/* harmony import */ var ___WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! . */ "./src/app/history/index.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var SxcVersionsService = /** @class */ (function () {
    function SxcVersionsService(http) {
        this.http = http;
        this.versionsSubject = new rxjs__WEBPACK_IMPORTED_MODULE_3__["ReplaySubject"](1);
        this.versions = this.versionsSubject.asObservable();
        this.errorSubject = new rxjs__WEBPACK_IMPORTED_MODULE_3__["ReplaySubject"](1);
        this.error = this.errorSubject.asObservable();
        this.loadVersions();
    }
    SxcVersionsService.prototype.restore = function (changeId) {
        var appId = _config__WEBPACK_IMPORTED_MODULE_4__["Config"].appId();
        var item = _config__WEBPACK_IMPORTED_MODULE_4__["Config"].item();
        var url = ___WEBPACK_IMPORTED_MODULE_5__["webApiHistory"] + "/restore?appId=" + appId + "&changeId=" + changeId;
        return this.http.post(url, item);
    };
    SxcVersionsService.prototype.loadVersions = function () {
        var _this = this;
        var appId = _config__WEBPACK_IMPORTED_MODULE_4__["Config"].appId();
        var item = _config__WEBPACK_IMPORTED_MODULE_4__["Config"].item();
        var url = ___WEBPACK_IMPORTED_MODULE_5__["webApiHistory"] + "/history?appId=" + appId;
        this.http.post(url, item)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function (all) {
            return all.map(function (ver) { return ({
                ChangeSetId: ver.ChangeSetId,
                HistoryId: ver.HistoryId,
                Data: convertVersionJsonToData(ver, findPrevious(all, ver)),
                TimeStamp: formatTimestamp(ver.TimeStamp),
                User: ver.User,
                VersionNumber: ver.VersionNumber,
            }); });
        }))
            .subscribe(function (v) { return _this.versionsSubject.next(v); }, function () { _this.errorSubject.next('Could not load versions.'); });
    };
    SxcVersionsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] }
    ]; };
    SxcVersionsService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"]])
    ], SxcVersionsService);
    return SxcVersionsService;
}());

/** Take the multi-level Attributes and flatten for use in the UI */
function convertVersionJsonToData(v, prevAttrs) {
    var attrs = JSON.parse(v.Json).Entity.Attributes;
    return Object.entries(attrs)
        .reduce(function (t, c) { return Array.prototype.concat(t, Object.entries(c[1])
        .map(function (_a) {
        var key = _a[0], value = _a[1];
        return ({
            key: key,
            value: Object.entries(value),
            type: c[0],
            hasChanged: prevAttrs && JSON.stringify(prevAttrs[c[0]][key]) !== JSON.stringify(value),
        });
    })); }, []);
}
function findPrevious(all, v) {
    var prevVersion = all.find(function (v2) { return v2.VersionNumber === v.VersionNumber - 1; });
    var prevVerAttrs = prevVersion && JSON.parse(prevVersion.Json).Entity.Attributes;
    return prevVerAttrs;
}
function formatTimestamp(timestamp) {
    var date = new Date(timestamp);
    var y = date.getFullYear();
    var m = date.getUTCMonth() + 1;
    var d = date.getDate();
    var h = date.getHours();
    var min = date.getMinutes();
    return y + "-" + (m < 10 ? '0' : '') + m + "-" + (d < 10 ? '0' : '') + d + " " + (h < 10 ? '0' : '') + h + ":" + (min < 10 ? '0' : '') + min;
}


/***/ }),

/***/ "./src/app/history/version-dialog.module.ts":
/*!**************************************************!*\
  !*** ./src/app/history/version-dialog.module.ts ***!
  \**************************************************/
/*! exports provided: VersionDialogModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "VersionDialogModule", function() { return VersionDialogModule; });
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm2015/ngx-translate-core.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm2015/common.js");
/* harmony import */ var _sxc_versions_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./sxc-versions.service */ "./src/app/history/sxc-versions.service.ts");
/* harmony import */ var _history_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./history.component */ "./src/app/history/history.component.ts");
/* harmony import */ var _angular_flex_layout__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/flex-layout */ "./node_modules/@angular/flex-layout/esm2015/flex-layout.js");
/* harmony import */ var app_material_module__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! app/material-module */ "./src/app/material-module.ts");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/platform-browser/animations */ "./node_modules/@angular/platform-browser/fesm2015/animations.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};









var VersionDialogModule = /** @class */ (function () {
    function VersionDialogModule() {
    }
    VersionDialogModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            exports: [
                _history_component__WEBPACK_IMPORTED_MODULE_4__["HistoryComponent"]
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _angular_common_http__WEBPACK_IMPORTED_MODULE_8__["HttpClientModule"],
                _angular_flex_layout__WEBPACK_IMPORTED_MODULE_5__["FlexLayoutModule"],
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_0__["TranslateModule"],
                _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_7__["BrowserAnimationsModule"],
                app_material_module__WEBPACK_IMPORTED_MODULE_6__["MaterialModule"],
            ],
            providers: [
                _sxc_versions_service__WEBPACK_IMPORTED_MODULE_3__["SxcVersionsService"],
            ],
            declarations: [
                _history_component__WEBPACK_IMPORTED_MODULE_4__["HistoryComponent"],
            ],
        })
    ], VersionDialogModule);
    return VersionDialogModule;
}());



/***/ }),

/***/ "./src/app/history/version-dto.ts":
/*!****************************************!*\
  !*** ./src/app/history/version-dto.ts ***!
  \****************************************/
/*! no static exports found */
/***/ (function(module, exports) {



/***/ }),

/***/ "./src/app/history/version.ts":
/*!************************************!*\
  !*** ./src/app/history/version.ts ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports) {



/***/ }),

/***/ "./src/app/i18n/constants.ts":
/*!***********************************!*\
  !*** ./src/app/i18n/constants.ts ***!
  \***********************************/
/*! exports provided: PrimaryUiLanguage, SupportedLanguages, i18nPrefix, pathToI18n, i18nExtension */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PrimaryUiLanguage", function() { return PrimaryUiLanguage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SupportedLanguages", function() { return SupportedLanguages; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "i18nPrefix", function() { return i18nPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "pathToI18n", function() { return pathToI18n; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "i18nExtension", function() { return i18nExtension; });
/* Constants for i18n */
var PrimaryUiLanguage = 'en';
var SupportedLanguages = ['en', 'de', 'es', 'fr', 'it', 'uk'];
var i18nPrefix = 'TemplatePicker.LayoutElement';
// todo: note there is another prefix ItemHistory... which will be removed soon
var pathToI18n = './i18n/';
var i18nExtension = '.js';


/***/ }),

/***/ "./src/app/i18n/index.ts":
/*!*******************************!*\
  !*** ./src/app/i18n/index.ts ***!
  \*******************************/
/*! exports provided: PrimaryUiLanguage, SupportedLanguages, i18nPrefix, pathToI18n, i18nExtension, langCode2 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "langCode2", function() { return langCode2; });
/* harmony import */ var _constants__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./constants */ "./src/app/i18n/constants.ts");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "PrimaryUiLanguage", function() { return _constants__WEBPACK_IMPORTED_MODULE_0__["PrimaryUiLanguage"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "SupportedLanguages", function() { return _constants__WEBPACK_IMPORTED_MODULE_0__["SupportedLanguages"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "i18nPrefix", function() { return _constants__WEBPACK_IMPORTED_MODULE_0__["i18nPrefix"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "pathToI18n", function() { return _constants__WEBPACK_IMPORTED_MODULE_0__["pathToI18n"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "i18nExtension", function() { return _constants__WEBPACK_IMPORTED_MODULE_0__["i18nExtension"]; });


function langCode2(langCode5) {
    return langCode5.split('-')[0];
}


/***/ }),

/***/ "./src/app/installer/getting-started.service.ts":
/*!******************************************************!*\
  !*** ./src/app/installer/getting-started.service.ts ***!
  \******************************************************/
/*! exports provided: GettingStartedService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "GettingStartedService", function() { return GettingStartedService; });
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var app_core_log__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! app/core/log */ "./src/app/core/log.ts");
/* harmony import */ var app_core_constants__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! app/core/constants */ "./src/app/core/constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var GettingStartedService = /** @class */ (function () {
    function GettingStartedService(http) {
        this.http = http;
        this.ready$ = new rxjs__WEBPACK_IMPORTED_MODULE_3__["Observable"]();
        this.gettingStartedSubject = new rxjs__WEBPACK_IMPORTED_MODULE_3__["Subject"]();
        this.gettingStarted$ = this.gettingStartedSubject.asObservable();
        this.ready$ = this.gettingStarted$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function () { return true; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(false));
        this.ready$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["tap"])(function (r) { return app_core_log__WEBPACK_IMPORTED_MODULE_4__["log"].add("ready getting started:" + r); })).subscribe();
    }
    GettingStartedService.prototype.loadGettingStarted = function (isContentApp) {
        var _this = this;
        this.http.get(app_core_constants__WEBPACK_IMPORTED_MODULE_5__["Constants"].webApiRemoteInstaller + "?dialog=gettingstarted&isContentApp=" + isContentApp)
            .subscribe(function (json) { return _this.gettingStartedSubject.next(json); });
    };
    GettingStartedService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] }
    ]; };
    GettingStartedService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"]])
    ], GettingStartedService);
    return GettingStartedService;
}());



/***/ }),

/***/ "./src/app/installer/installer.component.scss":
/*!****************************************************!*\
  !*** ./src/app/installer/installer.component.scss ***!
  \****************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (":host iframe {\n  border: none;\n  height: 500px;\n}\n\n:host .progress {\n  position: absolute;\n  left: 0;\n  top: 0;\n  height: 100%;\n  width: 100%;\n  background: rgba(255, 255, 255, 0.8);\n  display: flex;\n  justify-content: center;\n  flex-direction: column;\n  text-align: center;\n}\n\n:host .progress mat-progress-spinner {\n  margin: 0 auto;\n}\n\n:host .progress span {\n  line-height: 48px;\n}\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvaW5zdGFsbGVyL2luc3RhbGxlci5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUVRLFlBQVk7RUFDWixhQUFhO0FBQXJCOztBQUhBO0VBTVEsa0JBQWtCO0VBQ2xCLE9BQU87RUFDUCxNQUFNO0VBQ04sWUFBWTtFQUNaLFdBQVc7RUFDWCxvQ0FBbUM7RUFDbkMsYUFBYTtFQUNiLHVCQUF1QjtFQUN2QixzQkFBc0I7RUFDdEIsa0JBQWtCO0FBQzFCOztBQWhCQTtFQWlCWSxjQUFjO0FBRzFCOztBQXBCQTtFQW9CWSxpQkFBaUI7QUFJN0IiLCJmaWxlIjoic3JjL2FwcC9pbnN0YWxsZXIvaW5zdGFsbGVyLmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiOmhvc3Qge1xyXG4gICAgaWZyYW1lIHtcclxuICAgICAgICBib3JkZXI6IG5vbmU7XHJcbiAgICAgICAgaGVpZ2h0OiA1MDBweDtcclxuICAgIH1cclxuICAgIC5wcm9ncmVzcyB7XHJcbiAgICAgICAgcG9zaXRpb246IGFic29sdXRlO1xyXG4gICAgICAgIGxlZnQ6IDA7XHJcbiAgICAgICAgdG9wOiAwO1xyXG4gICAgICAgIGhlaWdodDogMTAwJTtcclxuICAgICAgICB3aWR0aDogMTAwJTtcclxuICAgICAgICBiYWNrZ3JvdW5kOiByZ2JhKDI1NSwgMjU1LCAyNTUsIC44KTtcclxuICAgICAgICBkaXNwbGF5OiBmbGV4O1xyXG4gICAgICAgIGp1c3RpZnktY29udGVudDogY2VudGVyO1xyXG4gICAgICAgIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XHJcbiAgICAgICAgdGV4dC1hbGlnbjogY2VudGVyO1xyXG4gICAgICAgIG1hdC1wcm9ncmVzcy1zcGlubmVyIHtcclxuICAgICAgICAgICAgbWFyZ2luOiAwIGF1dG87XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHNwYW4ge1xyXG4gICAgICAgICAgICBsaW5lLWhlaWdodDogNDhweDtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iXX0= */");

/***/ }),

/***/ "./src/app/installer/installer.component.ts":
/*!**************************************************!*\
  !*** ./src/app/installer/installer.component.ts ***!
  \**************************************************/
/*! exports provided: InstallerComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InstallerComponent", function() { return InstallerComponent; });
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var app_installer_installer_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! app/installer/installer.service */ "./src/app/installer/installer.service.ts");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/platform-browser */ "./node_modules/@angular/platform-browser/fesm2015/platform-browser.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var _getting_started_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./getting-started.service */ "./src/app/installer/getting-started.service.ts");
/* harmony import */ var _config__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../config */ "./src/app/config.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __importDefault = (undefined && undefined.__importDefault) || function (mod) {
  return (mod && mod.__esModule) ? mod : { "default": mod };
};







var InstallerComponent = /** @class */ (function () {
    function InstallerComponent(installer, api, sanitizer) {
        var _this = this;
        this.installer = installer;
        this.api = api;
        this.sanitizer = sanitizer;
        this.remoteInstallerUrl = '';
        this.ready = false;
        this.subscriptions = [];
        this.subscriptions.push(this.api.gettingStarted$.subscribe(function (url) {
            _this.remoteInstallerUrl = _this.sanitizer.bypassSecurityTrustResourceUrl(url);
            _this.ready = true;
        }));
        window.bootController.rebootRequest$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["debounceTime"])(1000))
            .subscribe(function () { return _this.destroy(); });
    }
    InstallerComponent.prototype.destroy = function () {
        this.subscriptions
            .forEach(function (sub) { return sub.unsubscribe(); });
        console.log('destroy subs', this.subscriptions);
    };
    InstallerComponent.prototype.ngOnInit = function () {
        var _this = this;
        var alreadyProcessing = false;
        this.api.loadGettingStarted(this.isContentApp);
        this.subscriptions.push(Object(rxjs__WEBPACK_IMPORTED_MODULE_4__["fromEvent"])(window, 'message').pipe(
        // Ensure only one installation is processed.
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["filter"])(function () { return !alreadyProcessing; }), 
        // Get data from event.
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function (evt) {
            try {
                return JSON.parse(evt.data);
            }
            catch (e) {
                return void 0;
            }
        }), 
        // Check if data is correct.
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["filter"])(function (data) { return data
            && Number(data.moduleId) === _config__WEBPACK_IMPORTED_MODULE_6__["Config"].moduleId()
            && data.action === 'install'; }), 
        // Get packages from data.
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function (data) { return Object.values(data.packages); }), 
        // Show confirm dialog.
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["filter"])(function (packages) {
            var packagesDisplayNames = packages
                .reduce(function (t, c) { return t + " - " + c.displayName + "\n"; }, '');
            var msg = "Do you want to install these packages?\n\n" + packagesDisplayNames + "\nThis takes 5 - 30 seconds per package. Don't reload the page while it's installing.";
            return confirm(msg);
        }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["switchMap"])(function (packages) {
            alreadyProcessing = true;
            _this.showProgress = true;
            return _this.installer.installPackages(packages, function (p) { return _this.currentPackage = p; });
        }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["tap"])(function () {
            _this.showProgress = false;
            alert('Installation complete 👍');
            window.top.location.reload();
        }))
            .subscribe(null, function () {
            _this.showProgress = false;
            alert('An error occurred.');
            alreadyProcessing = false;
        }));
    };
    InstallerComponent.ctorParameters = function () { return [
        { type: app_installer_installer_service__WEBPACK_IMPORTED_MODULE_2__["InstallerService"] },
        { type: _getting_started_service__WEBPACK_IMPORTED_MODULE_5__["GettingStartedService"] },
        { type: _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__["DomSanitizer"] }
    ]; };
    InstallerComponent.propDecorators = {
        isContentApp: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"] }]
    };
    InstallerComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-installer',
            template: __importDefault(__webpack_require__(/*! raw-loader!./installer.component.html */ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/installer/installer.component.html")).default,
            styles: [__importDefault(__webpack_require__(/*! ./installer.component.scss */ "./src/app/installer/installer.component.scss")).default]
        }),
        __metadata("design:paramtypes", [app_installer_installer_service__WEBPACK_IMPORTED_MODULE_2__["InstallerService"],
            _getting_started_service__WEBPACK_IMPORTED_MODULE_5__["GettingStartedService"],
            _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__["DomSanitizer"]])
    ], InstallerComponent);
    return InstallerComponent;
}());



/***/ }),

/***/ "./src/app/installer/installer.module.ts":
/*!***********************************************!*\
  !*** ./src/app/installer/installer.module.ts ***!
  \***********************************************/
/*! exports provided: InstallerModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InstallerModule", function() { return InstallerModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm2015/common.js");
/* harmony import */ var _installer_component__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./installer.component */ "./src/app/installer/installer.component.ts");
/* harmony import */ var app_installer_installer_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! app/installer/installer.service */ "./src/app/installer/installer.service.ts");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/progress-spinner */ "./node_modules/@angular/material/fesm2015/progress-spinner.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};





var InstallerModule = /** @class */ (function () {
    function InstallerModule() {
    }
    InstallerModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_1__["CommonModule"],
                _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_4__["MatProgressSpinnerModule"],
            ],
            exports: [
                _installer_component__WEBPACK_IMPORTED_MODULE_2__["InstallerComponent"]
            ],
            declarations: [
                _installer_component__WEBPACK_IMPORTED_MODULE_2__["InstallerComponent"]
            ],
            providers: [
                app_installer_installer_service__WEBPACK_IMPORTED_MODULE_3__["InstallerService"]
            ]
        })
    ], InstallerModule);
    return InstallerModule;
}());



/***/ }),

/***/ "./src/app/installer/installer.service.ts":
/*!************************************************!*\
  !*** ./src/app/installer/installer.service.ts ***!
  \************************************************/
/*! exports provided: InstallerService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InstallerService", function() { return InstallerService; });
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
/* harmony import */ var app_core_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! app/core/constants */ "./src/app/core/constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var InstallerService = /** @class */ (function () {
    function InstallerService(http) {
        this.http = http;
    }
    InstallerService.prototype.installPackages = function (packages, step) {
        var _this = this;
        return packages.reduce(function (t, c) { return t.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["switchMap"])(function () {
            if (!c.url)
                return Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["of"])(true);
            step(c);
            return _this.http.get(app_core_constants__WEBPACK_IMPORTED_MODULE_4__["Constants"].webApiInstallPackage + "?packageUrl=" + c.url);
        })); }, Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["of"])(true));
    };
    InstallerService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_3__["HttpClient"] }
    ]; };
    InstallerService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_3__["HttpClient"]])
    ], InstallerService);
    return InstallerService;
}());



/***/ }),

/***/ "./src/app/material-module.ts":
/*!************************************!*\
  !*** ./src/app/material-module.ts ***!
  \************************************/
/*! exports provided: MaterialModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MaterialModule", function() { return MaterialModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material/button */ "./node_modules/@angular/material/fesm2015/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/icon */ "./node_modules/@angular/material/fesm2015/icon.js");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/progress-spinner */ "./node_modules/@angular/material/fesm2015/progress-spinner.js");
/* harmony import */ var _angular_material_progress_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/progress-bar */ "./node_modules/@angular/material/fesm2015/progress-bar.js");
/* harmony import */ var _angular_material_expansion__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/expansion */ "./node_modules/@angular/material/fesm2015/expansion.js");
/* harmony import */ var _angular_material_tabs__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/tabs */ "./node_modules/@angular/material/fesm2015/tabs.js");
/* harmony import */ var _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/toolbar */ "./node_modules/@angular/material/fesm2015/toolbar.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/dialog */ "./node_modules/@angular/material/fesm2015/dialog.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};









var MATERIAL_MODULES = [
    _angular_material_button__WEBPACK_IMPORTED_MODULE_1__["MatButtonModule"],
    _angular_material_dialog__WEBPACK_IMPORTED_MODULE_8__["MatDialogModule"],
    _angular_material_expansion__WEBPACK_IMPORTED_MODULE_5__["MatExpansionModule"],
    _angular_material_icon__WEBPACK_IMPORTED_MODULE_2__["MatIconModule"],
    _angular_material_progress_bar__WEBPACK_IMPORTED_MODULE_4__["MatProgressBarModule"],
    _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_3__["MatProgressSpinnerModule"],
    _angular_material_tabs__WEBPACK_IMPORTED_MODULE_6__["MatTabsModule"],
    _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_7__["MatToolbarModule"],
];
var MaterialModule = /** @class */ (function () {
    function MaterialModule() {
    }
    MaterialModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            imports: MATERIAL_MODULES,
            exports: MATERIAL_MODULES,
        })
    ], MaterialModule);
    return MaterialModule;
}());



/***/ }),

/***/ "./src/app/template-picker/constants.ts":
/*!**********************************************!*\
  !*** ./src/app/template-picker/constants.ts ***!
  \**********************************************/
/*! exports provided: cViewWithoutContent, cAppActionImport */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "cViewWithoutContent", function() { return cViewWithoutContent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "cAppActionImport", function() { return cAppActionImport; });
var cViewWithoutContent = '_LayoutElement';
var cAppActionImport = -1;


/***/ }),

/***/ "./src/app/template-picker/current-data.service.ts":
/*!*********************************************************!*\
  !*** ./src/app/template-picker/current-data.service.ts ***!
  \*********************************************************/
/*! exports provided: CurrentDataService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CurrentDataService", function() { return CurrentDataService; });
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var _picker_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./picker.service */ "./src/app/template-picker/picker.service.ts");
/* harmony import */ var _template_filter_pipe__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./template-filter.pipe */ "./src/app/template-picker/template-filter.pipe.ts");
/* harmony import */ var app_core_log__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! app/core/log */ "./src/app/core/log.ts");
/* harmony import */ var _data_content_types_processor_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./data/content-types-processor.service */ "./src/app/template-picker/data/content-types-processor.service.ts");
/* harmony import */ var _data_template_processor__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./data/template-processor */ "./src/app/template-picker/data/template-processor.ts");
/* harmony import */ var app_debug_config__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! app/debug-config */ "./src/app/debug-config.ts");
/* harmony import */ var app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! app/core/behavior-observable */ "./src/app/core/behavior-observable.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
// #region imports










// #endregion
var log = app_core_log__WEBPACK_IMPORTED_MODULE_5__["log"].subLog('state', app_debug_config__WEBPACK_IMPORTED_MODULE_8__["DebugConfig"].state.enabled);
var CurrentDataService = /** @class */ (function () {
    function CurrentDataService(api, templateFilter, ctProcessor) {
        this.api = api;
        this.templateFilter = templateFilter;
        this.ctProcessor = ctProcessor;
        this.appId$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_9__["BehaviorObservable"].create(null);
        this.initialTypeId$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_9__["BehaviorObservable"].create(null);
        this.initialTemplateId$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_9__["BehaviorObservable"].create(null);
        this.selectedType$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_9__["BehaviorObservable"].create(null);
        this.selectedTemplate$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_9__["BehaviorObservable"].create(null);
        this.buildBasicObservables();
    }
    CurrentDataService.prototype.buildBasicObservables = function () {
        var _this = this;
        // app-stream should contain selected app, once the ID is known - or null
        this.app$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["combineLatest"])(this.api.apps$, this.appId$, function (apps, appId) { return apps.find(function (a) { return a.AppId === appId; }); });
        // current type should be either the initial type, or a manually selected type
        var initialType$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["combineLatest"])(this.initialTypeId$, this.api.contentTypes$, function (typeId, all) { return _data_content_types_processor_service__WEBPACK_IMPORTED_MODULE_6__["ContentTypesProcessor"].findContentTypesById(all, typeId); });
        this.type$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["merge"])(initialType$, this.selectedType$).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(null), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["share"])());
        // the templates-list is always filtered by the currently selected type
        this.templates$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["combineLatest"])(this.api.templates$, this.type$, function (all, current) { return _this.findTemplatesForTypeOrAll(all, current); })
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(new Array()));
        // the current template is either the last selected, or auto-selected when conditions change
        var initialTemplate$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["combineLatest"])(this.initialTemplateId$, this.api.templates$, function (id, templates) { return templates.find(function (t) { return t.TemplateId === id; }); }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["filter"])(function (t) { return t != null; }), // only allow new values which are not null, to guarantee later template$ updates don't affect this
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(null), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["share"])());
        var selected$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["merge"])(initialTemplate$, this.selectedTemplate$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["filter"])(function (t) { return t !== null; })));
        this.template$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["combineLatest"])(selected$, this.templates$, this.type$, this.app$, function (selected, templates, type, app) { return _data_template_processor__WEBPACK_IMPORTED_MODULE_7__["TemplateProcessor"].pickSelected(selected, templates, type, app); })
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(null), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["share"])());
        // construct list of relevant types for the UI
        this.types$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["combineLatest"])(this.api.contentTypes$, this.type$, this.api.templates$, this.template$, function (types, type, templates, template) { return _this.ctProcessor.buildList(types, type, templates, template); });
    };
    CurrentDataService.prototype.init = function (config) {
        this.config = config;
        // app-init is ready, if it has an app or doesn't need to init one
        log.add("initializing with config:" + JSON.stringify(config), config);
        var appReady$ = this.app$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function (a) { return config.isContent || !!a; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(config.isContent || !config.appId));
        var typeReady$ = this.type$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function (t) { return !!t; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["scan"])(function (acc, value) { return acc || value; }, !config.contentTypeId));
        var templReady$ = this.template$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function (t) { return !!t; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["debounceTime"])(100), // need to debounce, because the template might have a value and change again
        Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(!config.templateId));
        var loadAll$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_2__["combineLatest"])(appReady$, templReady$, typeReady$)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["map"])(function (set) { return set[0] && set[1] && set[2]; }));
        this.initLogging(appReady$, typeReady$, templReady$, loadAll$);
        // automatically set the app, type and template
        this.activateCurrentApp(config.appId);
        this.initialTypeId$.next(config.contentTypeId);
        this.initialTemplateId$.next(config.templateId);
        return loadAll$;
    };
    CurrentDataService.prototype.initLogging = function (inita$, inittyp$, initt$, initAll$) {
        var slog = log.subLog('stream', app_debug_config__WEBPACK_IMPORTED_MODULE_8__["DebugConfig"].state.streams);
        this.type$.subscribe(function (t) { return slog.add("type$ update:'" + (t && t.Label) + "'", t); });
        this.app$.subscribe(function (a) { return slog.add("app$ update:'" + (a && a.AppId) + "'", a); });
        this.template$.subscribe(function (t) { return slog.add("template$ update:'" + (t && t.TemplateId) + "'", t); });
        this.templates$.subscribe(function (t) { return slog.add("templates$ count:'" + (t && t.length) + "'", t); });
        this.types$.subscribe(function (t) { return slog.add("types$ count:'" + (t && t.length) + "'", t); });
        this.selectedTemplate$.subscribe(function (t) { return slog.add("selectedTemplate$: " + (t && t.TemplateId)); });
        var initLog = log.subLog('stream-init', app_debug_config__WEBPACK_IMPORTED_MODULE_8__["DebugConfig"].state.inits);
        this.initialTypeId$.subscribe(function (t) { return initLog.add("initial TypeId:'" + t + "'", t); });
        this.initialTemplateId$.subscribe(function (t) { return initLog.add("initial TemplateId:'" + t + "'", t); });
        inita$.subscribe(function (t) { return initLog.add("init app$", t); });
        inittyp$.subscribe(function (t) { return initLog.add("init type$", t); });
        initt$.subscribe(function (t) { return initLog.add("init temp$", t); });
        initAll$.subscribe(function (t) { return initLog.add("init all$", t); });
    };
    //#region activate calls from outside
    CurrentDataService.prototype.activateCurrentApp = function (appId) {
        log.add("activateApp(" + appId + ")");
        this.appId$.next(appId);
    };
    CurrentDataService.prototype.activateType = function (contentType) {
        log.add("activateType(" + contentType.Name + ")");
        this.selectedType$.next(contentType);
    };
    CurrentDataService.prototype.activateTemplate = function (template) {
        log.add("activateTemplate(" + template.TemplateId + ")");
        this.selectedTemplate$.next(template);
    };
    //#endregion
    CurrentDataService.prototype.findTemplatesForTypeOrAll = function (allTemplates, contentType) {
        return this.templateFilter.transform(allTemplates, { contentType: contentType, isContent: this.config.isContent });
    };
    CurrentDataService.ctorParameters = function () { return [
        { type: _picker_service__WEBPACK_IMPORTED_MODULE_3__["PickerService"] },
        { type: _template_filter_pipe__WEBPACK_IMPORTED_MODULE_4__["TemplateFilterPipe"] },
        { type: _data_content_types_processor_service__WEBPACK_IMPORTED_MODULE_6__["ContentTypesProcessor"] }
    ]; };
    CurrentDataService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        __metadata("design:paramtypes", [_picker_service__WEBPACK_IMPORTED_MODULE_3__["PickerService"],
            _template_filter_pipe__WEBPACK_IMPORTED_MODULE_4__["TemplateFilterPipe"],
            _data_content_types_processor_service__WEBPACK_IMPORTED_MODULE_6__["ContentTypesProcessor"]])
    ], CurrentDataService);
    return CurrentDataService;
}());



/***/ }),

/***/ "./src/app/template-picker/data/content-types-processor.service.ts":
/*!*************************************************************************!*\
  !*** ./src/app/template-picker/data/content-types-processor.service.ts ***!
  \*************************************************************************/
/*! exports provided: ContentTypesProcessor */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypesProcessor", function() { return ContentTypesProcessor; });
/* harmony import */ var _constants__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../constants */ "./src/app/template-picker/constants.ts");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm2015/ngx-translate-core.js");
/* harmony import */ var app_core_log__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! app/core/log */ "./src/app/core/log.ts");
/* harmony import */ var app_debug_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! app/debug-config */ "./src/app/debug-config.ts");
/* harmony import */ var app_i18n__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! app/i18n */ "./src/app/i18n/index.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






// const debug = true;
var log = app_core_log__WEBPACK_IMPORTED_MODULE_3__["log"].subLog('ct-processor', app_debug_config__WEBPACK_IMPORTED_MODULE_4__["DebugConfig"].typeProcessor);
/**
 * This is a helper to do various transformations for the list of content-types
 */
var ContentTypesProcessor = /** @class */ (function () {
    function ContentTypesProcessor(translate) {
        this.translate = translate;
    }
    ContentTypesProcessor.prototype.buildList = function (allTypes, type, allTemplates, template) {
        log.add('buildList(...) of content-types to show');
        var unhide = this.unhideSelectedType(allTypes, type, template);
        unhide = this.addEmptyTypeIfNeeded(unhide, allTemplates);
        var filtered = this.hideNecessaryTypes(unhide);
        return this.sortTypes(filtered);
    };
    ContentTypesProcessor.prototype.hideNecessaryTypes = function (types) {
        return types.filter(function (t) { return !t.IsHidden; });
    };
    /**
     * Ensure current content-type is visible, just in case it's configured as hidden
     */
    ContentTypesProcessor.prototype.unhideSelectedType = function (contentTypes, currentType, currentTemplate) {
        contentTypes
            .filter(function (c) { return ((currentTemplate === null || currentTemplate === void 0 ? void 0 : currentTemplate.TemplateId) === c.TemplateId) || (c.StaticName === (currentType === null || currentType === void 0 ? void 0 : currentType.StaticName)); })
            .forEach(function (c) { return c.IsHidden = false; });
        return contentTypes;
    };
    /**
     * add an empty content-type for UI selection if any template would support "no content-type"
     */
    ContentTypesProcessor.prototype.addEmptyTypeIfNeeded = function (contentTypes, templates) {
        var layoutElementLabel = (this.translate && this.translate.instant(app_i18n__WEBPACK_IMPORTED_MODULE_5__["i18nPrefix"]))
            || _constants__WEBPACK_IMPORTED_MODULE_0__["cViewWithoutContent"]; // if translate is not ready, use the nicer label
        // add option for empty content type
        if (templates && templates.find(function (t) { return t.ContentTypeStaticName === ''; })) {
            contentTypes = contentTypes.slice(); // copy it first to not change original
            contentTypes.push({
                StaticName: _constants__WEBPACK_IMPORTED_MODULE_0__["cViewWithoutContent"],
                Name: app_i18n__WEBPACK_IMPORTED_MODULE_5__["i18nPrefix"],
                Thumbnail: null,
                Label: layoutElementLabel,
                IsHidden: false,
            });
        }
        return contentTypes;
    };
    /**
     * Sort the types by label
     */
    ContentTypesProcessor.prototype.sortTypes = function (contentTypes) {
        // https://stackoverflow.com/questions/51165/how-to-sort-strings-in-javascript
        return contentTypes.sort(function (a, b) { return ('' + a.Label).localeCompare(b.Label); });
    };
    // tslint:disable-next-line:member-ordering
    ContentTypesProcessor.findContentTypesById = function (contentTypes, selectedContentTypeId) {
        log.add("findContentTypesById(..., " + selectedContentTypeId);
        return selectedContentTypeId
            ? contentTypes.find(function (c) { return c.StaticName === selectedContentTypeId; })
            : null;
    };
    ContentTypesProcessor.ctorParameters = function () { return [
        { type: _ngx_translate_core__WEBPACK_IMPORTED_MODULE_2__["TranslateService"] }
    ]; };
    ContentTypesProcessor = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        __metadata("design:paramtypes", [_ngx_translate_core__WEBPACK_IMPORTED_MODULE_2__["TranslateService"]])
    ], ContentTypesProcessor);
    return ContentTypesProcessor;
}());



/***/ }),

/***/ "./src/app/template-picker/data/template-processor.ts":
/*!************************************************************!*\
  !*** ./src/app/template-picker/data/template-processor.ts ***!
  \************************************************************/
/*! exports provided: TemplateProcessor */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TemplateProcessor", function() { return TemplateProcessor; });
/* harmony import */ var app_core_log__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! app/core/log */ "./src/app/core/log.ts");
/* harmony import */ var app_debug_config__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! app/debug-config */ "./src/app/debug-config.ts");


var log = app_core_log__WEBPACK_IMPORTED_MODULE_0__["log"].subLog('TemplateProcessor', app_debug_config__WEBPACK_IMPORTED_MODULE_1__["DebugConfig"].templateProcessor);
var TemplateProcessor = /** @class */ (function () {
    function TemplateProcessor() {
    }
    TemplateProcessor.pickSelected = function (selected, templates, type, app) {
        log.add("pickSelected(selected: " + (selected && selected.TemplateId) + ", templates: " + templates.length + ")");
        // if one is selected, return that; but only if it's in the list of possible templates
        if (selected && templates.find(function (t) { return t.TemplateId === selected.TemplateId; }))
            return selected;
        // if none is selected, return the first; assuming a type or app has been selected
        if ((type || app) && templates && templates.length)
            return templates[0];
        // nothing valid
        return null;
    };
    return TemplateProcessor;
}());



/***/ }),

/***/ "./src/app/template-picker/debug.pipe.ts":
/*!***********************************************!*\
  !*** ./src/app/template-picker/debug.pipe.ts ***!
  \***********************************************/
/*! exports provided: DebugPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DebugPipe", function() { return DebugPipe; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var DebugPipe = /** @class */ (function () {
    function DebugPipe() {
    }
    DebugPipe.prototype.transform = function (obj, note) {
        console.log("pd:" + note + " (" + typeof (obj) + ")", obj);
        return obj;
    };
    DebugPipe = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Pipe"])({ name: 'debug', pure: true })
    ], DebugPipe);
    return DebugPipe;
}());



/***/ }),

/***/ "./src/app/template-picker/picker.service.ts":
/*!***************************************************!*\
  !*** ./src/app/template-picker/picker.service.ts ***!
  \***************************************************/
/*! exports provided: PickerService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PickerService", function() { return PickerService; });
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm2015/http.js");
/* harmony import */ var app_core_app__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! app/core/app */ "./src/app/core/app.ts");
/* harmony import */ var app_core_log__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! app/core/log */ "./src/app/core/log.ts");
/* harmony import */ var app_core_constants__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! app/core/constants */ "./src/app/core/constants.ts");
/* harmony import */ var app_debug_config__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! app/debug-config */ "./src/app/debug-config.ts");
/* harmony import */ var app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! app/core/behavior-observable */ "./src/app/core/behavior-observable.ts");
/* harmony import */ var _config__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../config */ "./src/app/config.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};











var log = app_core_log__WEBPACK_IMPORTED_MODULE_5__["log"].subLog('api', app_debug_config__WEBPACK_IMPORTED_MODULE_7__["DebugConfig"].api.enabled);
var uninitializedList = []; // this must be created as a variable, so we can check later if it's still the original or a new empty list
var PickerService = /** @class */ (function () {
    // all the subjects - these are all multi-cast, so don't use share!
    // #endregion
    function PickerService(http) {
        this.http = http;
        // #region public properties
        /** all apps of the zone */
        this.apps$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_8__["BehaviorObservable"].create(uninitializedList);
        /** all types of this app */
        this.contentTypes$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_8__["BehaviorObservable"].create(uninitializedList);
        /** templates/views of this app */
        this.templates$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_8__["BehaviorObservable"].create(uninitializedList);
        /**
         * ready is true when all necessary data is loaded
         * note that apps are not loaded if not needed */
        this.ready$ = new rxjs__WEBPACK_IMPORTED_MODULE_0__["Observable"]();
        // #endregion
        // #region private properties
        this.mustLoadApps = false;
        log.add('constructor()');
        this.buildObservables();
        this.enableLogging();
    }
    PickerService.prototype.buildObservables = function () {
        var _this = this;
        log.add("buildObservables()");
        // ready requires all to have data, but app can be skipped if not required
        this.ready$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["combineLatest"])(this.apps$, this.contentTypes$, this.templates$, function (a, ct, t) { return ({ apps: a, types: ct, templates: t }); })
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(function (set) { return set.templates !== uninitializedList
            && set.types !== uninitializedList
            && (!_this.mustLoadApps || !!(set.apps && set.apps !== uninitializedList)); }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["startWith"])(false), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["share"])());
    };
    PickerService.prototype.saveAppId = function (appId, reloadParts) {
        log.add("saveAppId(" + appId + ", " + reloadParts + ")");
        // skip doing anything here, if we're in content-mode (which doesn't use/change apps)
        if (!this.loadApps)
            throw new Error("can't save app, as we're not in app-mode");
        return this.http.get(app_core_constants__WEBPACK_IMPORTED_MODULE_6__["Constants"].webApiSetApp + "?appId=" + appId).toPromise();
    };
    PickerService.prototype.initLoading = function (requireApps) {
        log.add("initLoading(requireApps: " + requireApps + ")");
        this.mustLoadApps = requireApps;
        if (requireApps)
            this.loadApps();
        return this.reloadAppParts();
    };
    PickerService.prototype.reloadAppParts = function () {
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["combineLatest"])(this.loadTemplates(), this.loadContentTypes());
    };
    /**
     * load templates - is sometimes repeated if the app changes
     */
    PickerService.prototype.loadTemplates = function () {
        var _this = this;
        log.add('loadTemplates()');
        this.templates$.reset();
        var obs = this.http.get(app_core_constants__WEBPACK_IMPORTED_MODULE_6__["Constants"].webApiGetTemplates)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["share"])()); // ensure it's only run once
        obs.subscribe(function (response) { return _this.templates$.next(response || []); });
        return obs;
    };
    /**
     * Load the ContentTypes - only needed on first initialization
     */
    PickerService.prototype.loadContentTypes = function () {
        var _this = this;
        log.add("loadContentTypes()");
        this.contentTypes$.reset();
        var obs = this.http.get(app_core_constants__WEBPACK_IMPORTED_MODULE_6__["Constants"].webApiGetTypes)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["share"])()); // ensure it's only run once
        obs.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["map"])(function (response) { return (response || []).map(function (ct) {
            ct.Label = (ct.Metadata && ct.Metadata.Label)
                ? ct.Metadata.Label
                : ct.Name;
            return ct;
        }); }))
            .subscribe(function (json) { return _this.contentTypes$.next(json); });
        return obs;
    };
    /**
     * Load all Apps, only needed on first initialization
     */
    PickerService.prototype.loadApps = function () {
        var _this = this;
        var alreadyLoaded = !this.apps$.isInitial();
        log.add("loadApps() - skip:" + alreadyLoaded);
        if (alreadyLoaded)
            return;
        var appsFilter = _config__WEBPACK_IMPORTED_MODULE_9__["Config"].apps();
        var obs = this.http.get(app_core_constants__WEBPACK_IMPORTED_MODULE_6__["Constants"].webApiGetApps + "?apps=" + appsFilter)
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["share"])()); // ensure it's only run once
        obs.subscribe(function (response) { return _this.apps$.subject.next(response.map(function (a) { return new app_core_app__WEBPACK_IMPORTED_MODULE_4__["App"](a); })); });
        return obs;
    };
    PickerService.prototype.enableLogging = function () {
        var streamLog = app_core_log__WEBPACK_IMPORTED_MODULE_5__["log"].subLog('api-streams', app_debug_config__WEBPACK_IMPORTED_MODULE_7__["DebugConfig"].api.streams);
        this.apps$.subscribe(function (a) { return streamLog.add("app$:" + (a && a.length)); });
        this.contentTypes$.subscribe(function (ct) { return streamLog.add("contentTypes$:" + (ct && ct.length)); });
        this.templates$.subscribe(function (t) { return streamLog.add("templates$:" + (t && t.length)); });
        this.ready$.subscribe(function (r) { return streamLog.add("ready$:" + r); });
    };
    PickerService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_3__["HttpClient"] }
    ]; };
    PickerService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])(),
        __metadata("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_3__["HttpClient"]])
    ], PickerService);
    return PickerService;
}());



/***/ }),

/***/ "./src/app/template-picker/template-filter.pipe.ts":
/*!*********************************************************!*\
  !*** ./src/app/template-picker/template-filter.pipe.ts ***!
  \*********************************************************/
/*! exports provided: TemplateFilterPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TemplateFilterPipe", function() { return TemplateFilterPipe; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _constants__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./constants */ "./src/app/template-picker/constants.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};


var TemplateFilterPipe = /** @class */ (function () {
    function TemplateFilterPipe() {
    }
    TemplateFilterPipe.prototype.transform = function (templates, args) {
        var typeId = args.contentType ? args.contentType.StaticName : undefined;
        // in case we're filtering for the special "empty" code, use empty in the filter
        var typeNameFilter = typeId === _constants__WEBPACK_IMPORTED_MODULE_1__["cViewWithoutContent"]
            ? ''
            : (typeId || '');
        return templates
            .filter(function (t) { return !t.IsHidden; })
            .filter(function (t) { return !args.isContent || t.ContentTypeStaticName === typeNameFilter; });
    };
    TemplateFilterPipe = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Pipe"])({
            name: 'templateFilter'
        })
    ], TemplateFilterPipe);
    return TemplateFilterPipe;
}());



/***/ }),

/***/ "./src/app/template-picker/template-picker.component.scss":
/*!****************************************************************!*\
  !*** ./src/app/template-picker/template-picker.component.scss ***!
  \****************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (":host {\n  display: block;\n  padding-top: 56px;\n}\n\n:host .content {\n  background: #fafafa;\n  box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.3);\n  position: relative;\n}\n\n:host .content .card {\n  max-width: 1200px;\n  margin: 0 auto;\n  min-height: 164px;\n}\n\n:host .content .card .top-controls {\n  position: absolute;\n  height: 56px;\n  right: 4px;\n  top: -56px;\n  width: 100%;\n  text-align: center;\n  z-index: 2;\n  display: flex;\n  flex-direction: row;\n  align-items: center;\n}\n\n:host .content .card .top-controls button {\n  display: inline-block;\n  float: none;\n  position: relative;\n  top: 28px;\n}\n\n:host .content .card .top-controls button.secondary {\n  background: #fafafa !important;\n  color: rgba(0, 0, 0, 0.8);\n}\n\n:host .content .card .tiles {\n  white-space: nowrap;\n  overflow-x: auto;\n}\n\n:host .content .card .tiles .tile {\n  box-sizing: border-box;\n  background: #fff;\n  display: inline-block;\n  margin: 12px 0 12px 12px;\n  position: relative;\n  font-size: 12px;\n  border-radius: 22px 0 22px 0;\n  height: 88px;\n  width: 88px;\n  line-height: 88px;\n  text-align: center;\n  overflow: hidden;\n  cursor: pointer;\n  color: rgba(0, 0, 0, 0.8);\n  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);\n  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24);\n}\n\n:host .content .card .tiles .tile:hover {\n  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.16), 0 3px 6px rgba(0, 0, 0, 0.23);\n}\n\n:host .content .card .tiles .tile:hover .title {\n  opacity: 1;\n  text-overflow: initial;\n  height: auto;\n  min-height: 24px;\n  padding: 4px;\n  white-space: pre-wrap;\n  line-height: normal;\n}\n\n:host .content .card .tiles .tile:hover .title span {\n  transform: translate(0, 0);\n}\n\n:host .content .card .tiles .tile:hover .version {\n  opacity: 1;\n}\n\n:host .content .card .tiles .tile.blocked {\n  cursor: not-allowed;\n  opacity: 0.5;\n}\n\n:host .content .card .tiles .tile:active, :host .content .card .tiles .tile.active {\n  cursor: pointer;\n  box-shadow: 0 14px 28px rgba(0, 120, 220, 0.25), 0 10px 10px rgba(0, 120, 220, 0.22);\n  opacity: 1;\n}\n\n:host .content .card .tiles .tile.active, :host .content .card .tiles .tile:focus {\n  color: #0078dc;\n}\n\n:host .content .card .tiles .tile.config {\n  box-shadow: none;\n  border: 1px dashed rgba(0, 0, 0, 0.2);\n  background: transparent;\n}\n\n:host .content .card .tiles .tile .bg {\n  position: absolute;\n  left: 0;\n  top: 0;\n  width: 100%;\n  height: 100%;\n}\n\n:host .content .card .tiles .tile .bg img {\n  width: 100%;\n  height: 100%;\n}\n\n:host .content .card .tiles .tile .title {\n  display: inline-block;\n  position: absolute;\n  box-sizing: border-box;\n  padding: 0 4px;\n  left: 0;\n  bottom: 0;\n  width: 100%;\n  height: 24px;\n  line-height: 24px;\n  background: white;\n  color: rgba(0, 0, 0, 0.8);\n  letter-spacing: .1pt;\n  font-size: 11px;\n  text-overflow: ellipsis;\n  overflow: hidden;\n  white-space: nowrap;\n  opacity: 0;\n  font-weight: bold;\n  transition: opacity .4s ease;\n}\n\n:host .content .card .tiles .tile .title.show {\n  opacity: 1;\n}\n\n:host .content .card .tiles .tile .title.show span {\n  transform: translate(0, 0);\n}\n\n:host .content .card .tiles .tile .title span {\n  display: inline-block;\n  transform: translate(0, 24px);\n  transition: transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);\n}\n\n:host .content .card .tiles .tile .version {\n  display: inline-block;\n  position: absolute;\n  box-sizing: border-box;\n  padding: 0 4px;\n  right: 0;\n  top: 0;\n  width: 24px;\n  height: 24px;\n  line-height: 24px;\n  color: #fff;\n  letter-spacing: .1pt;\n  font-size: 11px;\n  font-weight: bold;\n  white-space: nowrap;\n  opacity: 0;\n  transition: opacity .4s ease;\n}\n\n:host .content .card .tiles .tile .version span {\n  transform: translate(0, 24px);\n  transition: transform 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);\n  text-shadow: 0px 0px 5px #000;\n}\n\n:host .content .card .templates-spinner {\n  width: 48px;\n  margin: 12px 0 12px 12px;\n  height: 88px;\n  display: inline-block;\n}\n\n:host .content .card mat-select {\n  width: 320px;\n}\n\n:host .content .card .row {\n  margin: 8px 0;\n}\n\n:host .content .card button {\n  margin: 0 0 0 8px;\n  float: left;\n  background: #0088f4;\n}\n\n:host .content .card .fr-getting-started {\n  border: none;\n}\n\n:host .content .no-install-allowed {\n  background: #F44336;\n  display: block;\n  padding: 16px;\n  border-radius: 2px;\n  color: #fff;\n  line-height: 24px;\n  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.19), 0 6px 6px rgba(0, 0, 0, 0.23);\n  text-align: center;\n}\n\n::ng-deep mat-tab-group mat-tab-header {\n  border-bottom: none !important;\n}\n\n::ng-deep mat-tab-group mat-tab-header mat-ink-bar {\n  display: none !important;\n}\n\n::ng-deep mat-tab-group mat-tab-header .mat-tab-labels {\n  display: block;\n}\n\n::ng-deep mat-tab-group mat-tab-header .mat-tab-labels .mat-tab-label {\n  min-width: 0 !important;\n}\n\n::ng-deep mat-tab-group mat-tab-header .mat-tab-labels .mat-tab-label.mat-tab-label-active {\n  opacity: 1;\n}\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvdGVtcGxhdGUtcGlja2VyL3RlbXBsYXRlLXBpY2tlci5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNJLGNBQWM7RUFDZCxpQkFBaUI7QUFDckI7O0FBSEE7RUFJUSxtQkFBbUI7RUFDbkIseUNBQXdDO0VBQ3hDLGtCQUFrQjtBQUcxQjs7QUFUQTtFQVFZLGlCQUFpQjtFQUNqQixjQUFjO0VBQ2QsaUJBQWlCO0FBSzdCOztBQWZBO0VBWWdCLGtCQUFrQjtFQUNsQixZQUFZO0VBQ1osVUFBVTtFQUNWLFVBQVU7RUFDVixXQUFXO0VBQ1gsa0JBQWtCO0VBQ2xCLFVBQVU7RUFDVixhQUFhO0VBQ2IsbUJBQW1CO0VBQ25CLG1CQUFtQjtBQU9uQzs7QUE1QkE7RUF1Qm9CLHFCQUFxQjtFQUNyQixXQUFXO0VBQ1gsa0JBQWtCO0VBQ2xCLFNBQVM7QUFTN0I7O0FBbkNBO0VBNkJvQiw4QkFBOEI7RUFDOUIseUJBQXdCO0FBVTVDOztBQXhDQTtFQWtDZ0IsbUJBQW1CO0VBQ25CLGdCQUFnQjtBQVVoQzs7QUE3Q0E7RUFxQ29CLHNCQUFzQjtFQUN0QixnQkFBZ0I7RUFDaEIscUJBQXFCO0VBQ3JCLHdCQUF3QjtFQUN4QixrQkFBa0I7RUFDbEIsZUFBZTtFQUNmLDRCQUE0QjtFQUM1QixZQUFZO0VBQ1osV0FBVztFQUNYLGlCQUFpQjtFQUNqQixrQkFBa0I7RUFDbEIsZ0JBQWdCO0VBQ2hCLGVBQWU7RUFDZix5QkFBd0I7RUFDeEIscURBQWtEO0VBQ2xELHdFQUFzRTtBQVkxRjs7QUFoRUE7RUFzRHdCLHdFQUF3RTtBQWNoRzs7QUFwRUE7RUF3RDRCLFVBQVU7RUFDVixzQkFBc0I7RUFDdEIsWUFBWTtFQUNaLGdCQUFnQjtFQUNoQixZQUFZO0VBQ1oscUJBQXFCO0VBQ3JCLG1CQUFtQjtBQWdCL0M7O0FBOUVBO0VBZ0VnQywwQkFBMEI7QUFrQjFEOztBQWxGQTtFQW9FNEIsVUFBVTtBQWtCdEM7O0FBdEZBO0VBd0V3QixtQkFBbUI7RUFDbkIsWUFBWTtBQWtCcEM7O0FBM0ZBO0VBNkV3QixlQUFlO0VBQ2Ysb0ZBQW9GO0VBQ3BGLFVBQVU7QUFrQmxDOztBQWpHQTtFQW9Gd0IsY0FBdUI7QUFpQi9DOztBQXJHQTtFQXVGd0IsZ0JBQWdCO0VBQ2hCLHFDQUFvQztFQUNwQyx1QkFBdUI7QUFrQi9DOztBQTNHQTtFQTRGd0Isa0JBQWtCO0VBQ2xCLE9BQU87RUFDUCxNQUFNO0VBQ04sV0FBVztFQUNYLFlBQVk7QUFtQnBDOztBQW5IQTtFQWtHNEIsV0FBVztFQUNYLFlBQVk7QUFxQnhDOztBQXhIQTtFQXVHd0IscUJBQXFCO0VBQ3JCLGtCQUFrQjtFQUNsQixzQkFBc0I7RUFDdEIsY0FBYztFQUNkLE9BQU87RUFDUCxTQUFTO0VBQ1QsV0FBVztFQUNYLFlBQVk7RUFDWixpQkFBaUI7RUFDakIsaUJBQWtDO0VBQ2xDLHlCQUF3QjtFQUN4QixvQkFBb0I7RUFDcEIsZUFBZTtFQUNmLHVCQUF1QjtFQUN2QixnQkFBZ0I7RUFDaEIsbUJBQW1CO0VBQ25CLFVBQVU7RUFDVixpQkFBaUI7RUFDakIsNEJBQTRCO0FBcUJwRDs7QUE5SUE7RUEySDRCLFVBQVU7QUF1QnRDOztBQWxKQTtFQTZIZ0MsMEJBQTBCO0FBeUIxRDs7QUF0SkE7RUFpSTRCLHFCQUFxQjtFQUNyQiw2QkFBNkI7RUFDN0IsaUVBQWdFO0FBeUI1Rjs7QUE1SkE7RUF1SXdCLHFCQUFxQjtFQUNyQixrQkFBa0I7RUFDbEIsc0JBQXNCO0VBQ3RCLGNBQWM7RUFDZCxRQUFRO0VBQ1IsTUFBTTtFQUNOLFdBQVc7RUFDWCxZQUFZO0VBQ1osaUJBQWlCO0VBRWpCLFdBQVc7RUFDWCxvQkFBb0I7RUFDcEIsZUFBZTtFQUNmLGlCQUFpQjtFQUNqQixtQkFBbUI7RUFDbkIsVUFBVTtFQUNWLDRCQUE0QjtBQXdCcEQ7O0FBL0tBO0VBMEo0Qiw2QkFBNkI7RUFDN0IsaUVBQWdFO0VBQ2hFLDZCQUNKO0FBd0J4Qjs7QUFyTEE7RUFtS2dCLFdBQVc7RUFDWCx3QkFBd0I7RUFDeEIsWUFBWTtFQUNaLHFCQUFxQjtBQXNCckM7O0FBNUxBO0VBeUtnQixZQUFZO0FBdUI1Qjs7QUFoTUE7RUE0S2dCLGFBQWE7QUF3QjdCOztBQXBNQTtFQStLZ0IsaUJBQWlCO0VBQ2pCLFdBQVc7RUFDWCxtQkFBbUI7QUF5Qm5DOztBQTFNQTtFQW9MZ0IsWUFBWTtBQTBCNUI7O0FBOU1BO0VBeUxZLG1CQUFtQjtFQUNuQixjQUFjO0VBQ2QsYUFBYTtFQUNiLGtCQUFrQjtFQUNsQixXQUFXO0VBQ1gsaUJBQWlCO0VBQ2pCLDBFQUFvRTtFQUNwRSxrQkFBa0I7QUF5QjlCOztBQXBCQTtFQUVRLDhCQUE4QjtBQXNCdEM7O0FBeEJBO0VBSVksd0JBQXdCO0FBd0JwQzs7QUE1QkE7RUFPWSxjQUFjO0FBeUIxQjs7QUFoQ0E7RUFTZ0IsdUJBQXVCO0FBMkJ2Qzs7QUFwQ0E7RUFXb0IsVUFBVTtBQTZCOUIiLCJmaWxlIjoic3JjL2FwcC90ZW1wbGF0ZS1waWNrZXIvdGVtcGxhdGUtcGlja2VyLmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiOmhvc3Qge1xyXG4gICAgZGlzcGxheTogYmxvY2s7XHJcbiAgICBwYWRkaW5nLXRvcDogNTZweDtcclxuICAgIC5jb250ZW50IHtcclxuICAgICAgICBiYWNrZ3JvdW5kOiAjZmFmYWZhO1xyXG4gICAgICAgIGJveC1zaGFkb3c6IDAgLTFweCAycHggcmdiYSgwLCAwLCAwLCAuMyk7XHJcbiAgICAgICAgcG9zaXRpb246IHJlbGF0aXZlO1xyXG4gICAgICAgIC5jYXJkIHtcclxuICAgICAgICAgICAgbWF4LXdpZHRoOiAxMjAwcHg7XHJcbiAgICAgICAgICAgIG1hcmdpbjogMCBhdXRvO1xyXG4gICAgICAgICAgICBtaW4taGVpZ2h0OiAxNjRweDtcclxuICAgICAgICAgICAgLnRvcC1jb250cm9scyB7XHJcbiAgICAgICAgICAgICAgICBwb3NpdGlvbjogYWJzb2x1dGU7XHJcbiAgICAgICAgICAgICAgICBoZWlnaHQ6IDU2cHg7XHJcbiAgICAgICAgICAgICAgICByaWdodDogNHB4O1xyXG4gICAgICAgICAgICAgICAgdG9wOiAtNTZweDtcclxuICAgICAgICAgICAgICAgIHdpZHRoOiAxMDAlO1xyXG4gICAgICAgICAgICAgICAgdGV4dC1hbGlnbjogY2VudGVyO1xyXG4gICAgICAgICAgICAgICAgei1pbmRleDogMjtcclxuICAgICAgICAgICAgICAgIGRpc3BsYXk6IGZsZXg7XHJcbiAgICAgICAgICAgICAgICBmbGV4LWRpcmVjdGlvbjogcm93O1xyXG4gICAgICAgICAgICAgICAgYWxpZ24taXRlbXM6IGNlbnRlcjtcclxuICAgICAgICAgICAgICAgIGJ1dHRvbiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xyXG4gICAgICAgICAgICAgICAgICAgIGZsb2F0OiBub25lO1xyXG4gICAgICAgICAgICAgICAgICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgICAgICAgICAgICAgICAgICB0b3A6IDI4cHg7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBidXR0b24uc2Vjb25kYXJ5IHtcclxuICAgICAgICAgICAgICAgICAgICBiYWNrZ3JvdW5kOiAjZmFmYWZhICFpbXBvcnRhbnQ7XHJcbiAgICAgICAgICAgICAgICAgICAgY29sb3I6IHJnYmEoMCwgMCwgMCwgLjgpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIC50aWxlcyB7XHJcbiAgICAgICAgICAgICAgICB3aGl0ZS1zcGFjZTogbm93cmFwO1xyXG4gICAgICAgICAgICAgICAgb3ZlcmZsb3cteDogYXV0bztcclxuICAgICAgICAgICAgICAgIC50aWxlIHtcclxuICAgICAgICAgICAgICAgICAgICBib3gtc2l6aW5nOiBib3JkZXItYm94O1xyXG4gICAgICAgICAgICAgICAgICAgIGJhY2tncm91bmQ6ICNmZmY7XHJcbiAgICAgICAgICAgICAgICAgICAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xyXG4gICAgICAgICAgICAgICAgICAgIG1hcmdpbjogMTJweCAwIDEycHggMTJweDtcclxuICAgICAgICAgICAgICAgICAgICBwb3NpdGlvbjogcmVsYXRpdmU7XHJcbiAgICAgICAgICAgICAgICAgICAgZm9udC1zaXplOiAxMnB4O1xyXG4gICAgICAgICAgICAgICAgICAgIGJvcmRlci1yYWRpdXM6IDIycHggMCAyMnB4IDA7XHJcbiAgICAgICAgICAgICAgICAgICAgaGVpZ2h0OiA4OHB4O1xyXG4gICAgICAgICAgICAgICAgICAgIHdpZHRoOiA4OHB4O1xyXG4gICAgICAgICAgICAgICAgICAgIGxpbmUtaGVpZ2h0OiA4OHB4O1xyXG4gICAgICAgICAgICAgICAgICAgIHRleHQtYWxpZ246IGNlbnRlcjtcclxuICAgICAgICAgICAgICAgICAgICBvdmVyZmxvdzogaGlkZGVuO1xyXG4gICAgICAgICAgICAgICAgICAgIGN1cnNvcjogcG9pbnRlcjtcclxuICAgICAgICAgICAgICAgICAgICBjb2xvcjogcmdiYSgwLCAwLCAwLCAuOCk7XHJcbiAgICAgICAgICAgICAgICAgICAgdHJhbnNpdGlvbjogYWxsIDAuM3MgY3ViaWMtYmV6aWVyKC4yNSwgLjgsIC4yNSwgMSk7XHJcbiAgICAgICAgICAgICAgICAgICAgYm94LXNoYWRvdzogMCAxcHggM3B4IHJnYmEoMCwgMCwgMCwgLjEyKSwgMCAxcHggMnB4IHJnYmEoMCwgMCwgMCwgLjI0KTtcclxuICAgICAgICAgICAgICAgICAgICAmOmhvdmVyIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYm94LXNoYWRvdzogMCAzcHggNnB4IHJnYmEoMCwgMCwgMCwgMC4xNiksIDAgM3B4IDZweCByZ2JhKDAsIDAsIDAsIDAuMjMpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAudGl0bGUge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgb3BhY2l0eTogMTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRleHQtb3ZlcmZsb3c6IGluaXRpYWw7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWlnaHQ6IGF1dG87XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtaW4taGVpZ2h0OiAyNHB4O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcGFkZGluZzogNHB4O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2hpdGUtc3BhY2U6IHByZS13cmFwO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbGluZS1oZWlnaHQ6IG5vcm1hbDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHNwYW4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRyYW5zZm9ybTogdHJhbnNsYXRlKDAsIDApO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC52ZXJzaW9uIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wYWNpdHk6IDE7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgJi5ibG9ja2VkIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgY3Vyc29yOiBub3QtYWxsb3dlZDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgb3BhY2l0eTogMC41O1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAmOmFjdGl2ZSxcclxuICAgICAgICAgICAgICAgICAgICAmLmFjdGl2ZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGN1cnNvcjogcG9pbnRlcjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYm94LXNoYWRvdzogMCAxNHB4IDI4cHggcmdiYSgwLCAxMjAsIDIyMCwgMC4yNSksIDAgMTBweCAxMHB4IHJnYmEoMCwgMTIwLCAyMjAsIDAuMjIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcGFjaXR5OiAxO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAmLmFjdGl2ZSxcclxuICAgICAgICAgICAgICAgICAgICAmOmZvY3VzIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gY29sb3I6ICNmZmY7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiByZ2IoMCwgMTIwLCAyMjApO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAmLmNvbmZpZyB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGJveC1zaGFkb3c6IG5vbmU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGJvcmRlcjogMXB4IGRhc2hlZCByZ2JhKDAsIDAsIDAsIC4yKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYmFja2dyb3VuZDogdHJhbnNwYXJlbnQ7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIC5iZyB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBvc2l0aW9uOiBhYnNvbHV0ZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbGVmdDogMDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdG9wOiAwO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogMTAwJTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaGVpZ2h0OiAxMDAlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpbWcge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IDEwMCU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWlnaHQ6IDEwMCU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgLnRpdGxlIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBwb3NpdGlvbjogYWJzb2x1dGU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGJveC1zaXppbmc6IGJvcmRlci1ib3g7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhZGRpbmc6IDAgNHB4O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBsZWZ0OiAwO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBib3R0b206IDA7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiAxMDAlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBoZWlnaHQ6IDI0cHg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGxpbmUtaGVpZ2h0OiAyNHB4O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBiYWNrZ3JvdW5kOiByZ2JhKDI1NSwgMjU1LCAyNTUsIDEpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb2xvcjogcmdiYSgwLCAwLCAwLCAuOCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGxldHRlci1zcGFjaW5nOiAuMXB0O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBmb250LXNpemU6IDExcHg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRleHQtb3ZlcmZsb3c6IGVsbGlwc2lzO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvdmVyZmxvdzogaGlkZGVuO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB3aGl0ZS1zcGFjZTogbm93cmFwO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcGFjaXR5OiAwO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBmb250LXdlaWdodDogYm9sZDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHJhbnNpdGlvbjogb3BhY2l0eSAuNHMgZWFzZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgJi5zaG93IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wYWNpdHk6IDE7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBzcGFuIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0cmFuc2Zvcm06IHRyYW5zbGF0ZSgwLCAwKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBzcGFuIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRpc3BsYXk6IGlubGluZS1ibG9jaztcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRyYW5zZm9ybTogdHJhbnNsYXRlKDAsIDI0cHgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdHJhbnNpdGlvbjogdHJhbnNmb3JtIC40cyBjdWJpYy1iZXppZXIoMC42OCwgLTAuNTUsIDAuMjY1LCAxLjU1KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAudmVyc2lvbiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRpc3BsYXk6IGlubGluZS1ibG9jaztcclxuICAgICAgICAgICAgICAgICAgICAgICAgcG9zaXRpb246IGFic29sdXRlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBib3gtc2l6aW5nOiBib3JkZXItYm94O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYWRkaW5nOiAwIDRweDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmlnaHQ6IDA7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRvcDogMDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IDI0cHg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGhlaWdodDogMjRweDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbGluZS1oZWlnaHQ6IDI0cHg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIGJhY2tncm91bmQ6IHJnYmEoMjU1LCAyNTUsIDI1NSwgMSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiAjZmZmO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBsZXR0ZXItc3BhY2luZzogLjFwdDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9udC1zaXplOiAxMXB4O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBmb250LXdlaWdodDogYm9sZDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgd2hpdGUtc3BhY2U6IG5vd3JhcDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgb3BhY2l0eTogMDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHJhbnNpdGlvbjogb3BhY2l0eSAuNHMgZWFzZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgc3BhbiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBkaXNwbGF5OiBpbmxpbmUtYmxvY2s7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0cmFuc2Zvcm06IHRyYW5zbGF0ZSgwLCAyNHB4KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRyYW5zaXRpb246IHRyYW5zZm9ybSAuNHMgY3ViaWMtYmV6aWVyKDAuNjgsIC0wLjU1LCAwLjI2NSwgMS41NSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZXh0LXNoYWRvdzogMHB4IDBweCA1cHggIzAwMFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAudGVtcGxhdGVzLXNwaW5uZXIge1xyXG4gICAgICAgICAgICAgICAgd2lkdGg6IDQ4cHg7XHJcbiAgICAgICAgICAgICAgICBtYXJnaW46IDEycHggMCAxMnB4IDEycHg7XHJcbiAgICAgICAgICAgICAgICBoZWlnaHQ6IDg4cHg7XHJcbiAgICAgICAgICAgICAgICBkaXNwbGF5OiBpbmxpbmUtYmxvY2s7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgbWF0LXNlbGVjdCB7XHJcbiAgICAgICAgICAgICAgICB3aWR0aDogMzIwcHg7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgLnJvdyB7XHJcbiAgICAgICAgICAgICAgICBtYXJnaW46IDhweCAwO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGJ1dHRvbiB7XHJcbiAgICAgICAgICAgICAgICBtYXJnaW46IDAgMCAwIDhweDtcclxuICAgICAgICAgICAgICAgIGZsb2F0OiBsZWZ0O1xyXG4gICAgICAgICAgICAgICAgYmFja2dyb3VuZDogIzAwODhmNDtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAuZnItZ2V0dGluZy1zdGFydGVkIHtcclxuICAgICAgICAgICAgICAgIGJvcmRlcjogbm9uZTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLm5vLWluc3RhbGwtYWxsb3dlZCB7XHJcbiAgICAgICAgICAgIGJhY2tncm91bmQ6ICNGNDQzMzY7XHJcbiAgICAgICAgICAgIGRpc3BsYXk6IGJsb2NrO1xyXG4gICAgICAgICAgICBwYWRkaW5nOiAxNnB4O1xyXG4gICAgICAgICAgICBib3JkZXItcmFkaXVzOiAycHg7XHJcbiAgICAgICAgICAgIGNvbG9yOiAjZmZmO1xyXG4gICAgICAgICAgICBsaW5lLWhlaWdodDogMjRweDtcclxuICAgICAgICAgICAgYm94LXNoYWRvdzogMCAxMHB4IDIwcHggcmdiYSgwLDAsMCwwLjE5KSwgMCA2cHggNnB4IHJnYmEoMCwwLDAsMC4yMyk7XHJcbiAgICAgICAgICAgIHRleHQtYWxpZ246IGNlbnRlcjtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn1cclxuXHJcbjo6bmctZGVlcCBtYXQtdGFiLWdyb3VwIHtcclxuICAgIG1hdC10YWItaGVhZGVyIHtcclxuICAgICAgICBib3JkZXItYm90dG9tOiBub25lICFpbXBvcnRhbnQ7XHJcbiAgICAgICAgbWF0LWluay1iYXIge1xyXG4gICAgICAgICAgICBkaXNwbGF5OiBub25lICFpbXBvcnRhbnQ7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIC5tYXQtdGFiLWxhYmVscyB7XHJcbiAgICAgICAgICAgIGRpc3BsYXk6IGJsb2NrO1xyXG4gICAgICAgICAgICAubWF0LXRhYi1sYWJlbCB7XHJcbiAgICAgICAgICAgICAgICBtaW4td2lkdGg6IDAgIWltcG9ydGFudDtcclxuICAgICAgICAgICAgICAgICYubWF0LXRhYi1sYWJlbC1hY3RpdmUge1xyXG4gICAgICAgICAgICAgICAgICAgIG9wYWNpdHk6IDE7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcbn1cclxuIl19 */");

/***/ }),

/***/ "./src/app/template-picker/template-picker.component.ts":
/*!**************************************************************!*\
  !*** ./src/app/template-picker/template-picker.component.ts ***!
  \**************************************************************/
/*! exports provided: TemplatePickerComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TemplatePickerComponent", function() { return TemplatePickerComponent; });
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs */ "./node_modules/rxjs/_esm2015/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _constants__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./constants */ "./src/app/template-picker/constants.ts");
/* harmony import */ var app_core_log__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! app/core/log */ "./src/app/core/log.ts");
/* harmony import */ var _picker_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./picker.service */ "./src/app/template-picker/picker.service.ts");
/* harmony import */ var _current_data_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./current-data.service */ "./src/app/template-picker/current-data.service.ts");
/* harmony import */ var app_debug_config__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! app/debug-config */ "./src/app/debug-config.ts");
/* harmony import */ var app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! app/core/behavior-observable */ "./src/app/core/behavior-observable.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __importDefault = (undefined && undefined.__importDefault) || function (mod) {
  return (mod && mod.__esModule) ? mod : { "default": mod };
};









var log = app_core_log__WEBPACK_IMPORTED_MODULE_4__["log"].subLog('picker', app_debug_config__WEBPACK_IMPORTED_MODULE_7__["DebugConfig"].picker.enabled);
var TemplatePickerComponent = /** @class */ (function () {
    //#endregion
    function TemplatePickerComponent(api, state, cdRef) {
        this.api = api;
        this.state = state;
        this.cdRef = cdRef;
        /** is cancelling possible */
        this.showCancel = true;
        /** show advanced features (admin/host only) */
        this.showAdvanced = false;
        /** show the installer */
        this.showInstaller = false;
        /** Tab-id, when we set it, the tab switches */
        this.tabIndex = 0;
        /** Indicates whether the installer can be shown in this dialog or not */
        this.isBadContextForInstaller = false;
        /** internal loading state */
        this.loading$ = app_core_behavior_observable__WEBPACK_IMPORTED_MODULE_8__["BehaviorObservable"].create(false);
        this.preventAppSwich = false;
        this.showDebug = app_debug_config__WEBPACK_IMPORTED_MODULE_7__["DebugConfig"].picker.showDebugPanel;
        this.ready = false;
        // get configuration from iframe-bridge and set everything
        this.bridge = window.frameElement.bridge;
        var dashInfo = this.bridge.getAdditionalDashboardConfig();
        this.boot(dashInfo);
        this.debugObservables();
    }
    TemplatePickerComponent.prototype.ngOnInit = function () {
        this.autosyncObservablesToEnsureUiUpdates();
    };
    TemplatePickerComponent.prototype.boot = function (dashInfo) {
        this.showDebug = dashInfo.debug;
        app_core_log__WEBPACK_IMPORTED_MODULE_4__["Log"].configureRuntimeLogging(dashInfo.debug);
        // start data-loading
        this.api.initLoading(!dashInfo.isContent);
        // init parts, variables, observables
        var initDone$ = this.state.init(dashInfo);
        this.initObservables(initDone$);
        this.initValuesFromBridge(dashInfo);
        this.loading$.next(false);
    };
    TemplatePickerComponent.prototype.debugObservables = function () {
        if (!app_debug_config__WEBPACK_IMPORTED_MODULE_7__["DebugConfig"].picker.streams)
            return;
        this.loading$.subscribe(function (l) { return log.add("loading$:" + l); });
        this.ready$.subscribe(function (r) { return log.add("ready$:" + r); });
    };
    /**
     * wire up observables for this component
     */
    TemplatePickerComponent.prototype.initObservables = function (initDone$) {
        var _this = this;
        var initTrue$ = initDone$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["filter"])(function (t) { return !!t; }));
        // wire up basic observables
        this.ready$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["combineLatest"])(this.api.ready$, this.loading$, function (r, l) { return r && !l; });
        // all apps are the same as provided by the api
        this.apps$ = this.api.apps$;
        // if the content-type or app is set, switch tabs (ignore null/empty states)
        var typeOrAppReady = Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["merge"])(this.state.type$, this.state.app$).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["filter"])(function (t) { return !!t; }));
        Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["combineLatest"])(typeOrAppReady, initTrue$).subscribe(function (_) { return _this.switchTab(); });
        // once the data is known, check if installer is needed
        Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["combineLatest"])(this.api.templates$, this.api.contentTypes$, this.api.apps$, this.api.ready$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["filter"])(function (r) { return !!r; })), function (templates, _, apps) {
            log.add('apps/templates loaded, will check if we should show installer');
            _this.showInstaller = _this.isContent
                ? templates.length === 0
                : apps.filter(function (a) { return a.AppId !== _constants__WEBPACK_IMPORTED_MODULE_3__["cAppActionImport"]; }).length === 0;
        }).subscribe();
        // template loading is true, when the template-list or selected template are not ready
        this.templatesLoading$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["combineLatest"])(this.state.templates$, this.state.template$, function (all, selected) { return !(all && selected); }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["startWith"])(false));
        // whenever the template changes, ensure the preview reloads
        // but don't do this when initializing, that's why we listen to initDone$
        this.state.template$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["filter"])(function (t) { return !!t; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_1__["skipUntil"])(initTrue$))
            .subscribe(function (t) { return _this.previewTemplate(t); });
    };
    /** The UI doesn't update reliably :(, so we copy the data to local variables */
    TemplatePickerComponent.prototype.autosyncObservablesToEnsureUiUpdates = function () {
        var _this = this;
        this.state.app$.subscribe(function (a) { return _this.app = a; });
        this.state.templates$.subscribe(function (t) { return _this.templates = t; });
        this.state.template$.subscribe(function (t) { return _this.template = t; });
        this.state.types$.subscribe(function (t) { return _this.types = t; });
        this.state.type$.subscribe(function (t) { return _this.contentType = t; });
        this.ready$.subscribe(function (r) { return _this.ready = r; });
        Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["merge"])(this.ready$, this.state.app$, this.state.type$, this.state.types$, this.state.template$, this.state.templates$).subscribe(function () { return _this.cdRef.detectChanges(); });
    };
    TemplatePickerComponent.prototype.initValuesFromBridge = function (config) {
        this.preventTypeSwitch = config.hasContent;
        this.isBadContextForInstaller = config.isInnerContent;
        this.isContent = config.isContent;
        this.supportsAjax = this.isContent || config.supportsAjax;
        this.showAdvanced = config.user.canDesign;
        this.preventAppSwich = config.hasContent;
        this.showCancel = config.templateId != null;
    };
    //#region basic UI action binding
    TemplatePickerComponent.prototype.cancel = function () { this.bridge.cancel(); };
    TemplatePickerComponent.prototype.run = function (action) { this.bridge.run(action); };
    TemplatePickerComponent.prototype.persistTemplate = function (template) { this.bridge.setTemplate(template.TemplateId, template.Name, true); };
    /**
     * app selection from UI
     */
    TemplatePickerComponent.prototype.selectApp = function (before, after) {
        console.log('selectApp()');
        if (before && before.AppId === after.AppId)
            this.switchTab();
        else
            this.updateApp(after);
    };
    /**
     * content-type selection from UI
     */
    TemplatePickerComponent.prototype.selectContentType = function (before, after) {
        if (before && before.StaticName === after.StaticName)
            this.switchTab();
        else
            this.setContentType(after);
    };
    /**
     * activate a template from the UI
     */
    TemplatePickerComponent.prototype.selectTemplate = function (template) {
        this.state.activateTemplate(template);
    };
    //#endregion
    TemplatePickerComponent.prototype.setContentType = function (contentType) {
        log.add("select content-type '" + contentType.Name + "'; prevent: " + this.preventTypeSwitch);
        if (this.preventTypeSwitch)
            return;
        this.state.activateType(contentType);
    };
    TemplatePickerComponent.prototype.switchTab = function () {
        var _this = this;
        log.add('switchTab()');
        // must delay change because of a bug in the tabs-updating
        Object(rxjs__WEBPACK_IMPORTED_MODULE_0__["timer"])(100).toPromise().then(function (_) { return _this.tabIndex = 1; });
    };
    TemplatePickerComponent.prototype.updateApp = function (newApp) {
        var _this = this;
        // ajax-support can change as apps are changed; for ajax, maybe both the previous and new must support it
        // or just new? still WIP
        var ajax = newApp.SupportsAjaxReload;
        log.add("changing app to " + newApp.AppId + "; prevent-switch: " + this.preventAppSwich + " use-ajax:" + ajax);
        if (this.preventAppSwich)
            return;
        this.loading$.next(true);
        this.bridge.showMessage('loading App...');
        var savePromise = this.api.saveAppId(newApp.AppId.toString(), ajax);
        if (ajax) {
            savePromise.then(function () {
                log.add('saved app, will reset some stuff');
                // do this after save completed, to ensure that the module is ready on the server
                log.add('calling reloadAndReInit()');
                _this.bridge.reloadAndReInit()
                    .then(function (newConfig) { return _this.boot(newConfig); });
            });
        }
        else {
            savePromise.then(function () { return window.parent.location.reload(); });
        }
    };
    TemplatePickerComponent.prototype.previewTemplate = function (t) {
        var _this = this;
        log.add("previewTemplate(" + t.TemplateId + "), ajax is " + this.supportsAjax);
        this.loading$.next(true);
        this.bridge
            .setTemplate(t.TemplateId, t.Name, false)
            .then(function (_) { return _this.loading$.next(false); });
    };
    TemplatePickerComponent.ctorParameters = function () { return [
        { type: _picker_service__WEBPACK_IMPORTED_MODULE_5__["PickerService"] },
        { type: _current_data_service__WEBPACK_IMPORTED_MODULE_6__["CurrentDataService"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_2__["ChangeDetectorRef"] }
    ]; };
    TemplatePickerComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Component"])({
            selector: 'app-template-picker',
            template: __importDefault(__webpack_require__(/*! raw-loader!./template-picker.component.html */ "./node_modules/@angular-devkit/build-angular/node_modules/raw-loader/dist/cjs.js!./src/app/template-picker/template-picker.component.html")).default,
            styles: [__importDefault(__webpack_require__(/*! ./template-picker.component.scss */ "./src/app/template-picker/template-picker.component.scss")).default]
        }),
        __metadata("design:paramtypes", [_picker_service__WEBPACK_IMPORTED_MODULE_5__["PickerService"],
            _current_data_service__WEBPACK_IMPORTED_MODULE_6__["CurrentDataService"],
            _angular_core__WEBPACK_IMPORTED_MODULE_2__["ChangeDetectorRef"]])
    ], TemplatePickerComponent);
    return TemplatePickerComponent;
}());



/***/ }),

/***/ "./src/app/template-picker/template-picker.module.ts":
/*!***********************************************************!*\
  !*** ./src/app/template-picker/template-picker.module.ts ***!
  \***********************************************************/
/*! exports provided: TemplatePickerModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TemplatePickerModule", function() { return TemplatePickerModule; });
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @ngx-translate/core */ "./node_modules/@ngx-translate/core/fesm2015/ngx-translate-core.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm2015/forms.js");
/* harmony import */ var _angular_material_menu__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/menu */ "./node_modules/@angular/material/fesm2015/menu.js");
/* harmony import */ var _angular_material_progress_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/progress-bar */ "./node_modules/@angular/material/fesm2015/progress-bar.js");
/* harmony import */ var _angular_material_tabs__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/tabs */ "./node_modules/@angular/material/fesm2015/tabs.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/common */ "./node_modules/@angular/common/fesm2015/common.js");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/platform-browser/animations */ "./node_modules/@angular/platform-browser/fesm2015/animations.js");
/* harmony import */ var _template_picker_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./template-picker.component */ "./src/app/template-picker/template-picker.component.ts");
/* harmony import */ var _template_filter_pipe__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./template-filter.pipe */ "./src/app/template-picker/template-filter.pipe.ts");
/* harmony import */ var app_core_core_module__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! app/core/core.module */ "./src/app/core/core.module.ts");
/* harmony import */ var _angular_flex_layout__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/flex-layout */ "./node_modules/@angular/flex-layout/esm2015/flex-layout.js");
/* harmony import */ var app_installer_installer_module__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! app/installer/installer.module */ "./src/app/installer/installer.module.ts");
/* harmony import */ var _debug_pipe__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./debug.pipe */ "./src/app/template-picker/debug.pipe.ts");
/* harmony import */ var _data_content_types_processor_service__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./data/content-types-processor.service */ "./src/app/template-picker/data/content-types-processor.service.ts");
/* harmony import */ var app_material_module__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! app/material-module */ "./src/app/material-module.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
















var TemplatePickerModule = /** @class */ (function () {
    function TemplatePickerModule() {
    }
    TemplatePickerModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            exports: [
                _template_picker_component__WEBPACK_IMPORTED_MODULE_8__["TemplatePickerComponent"]
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_6__["CommonModule"],
                _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_7__["BrowserAnimationsModule"],
                _angular_material_menu__WEBPACK_IMPORTED_MODULE_3__["MatMenuModule"],
                _angular_material_tabs__WEBPACK_IMPORTED_MODULE_5__["MatTabsModule"],
                app_material_module__WEBPACK_IMPORTED_MODULE_15__["MaterialModule"],
                _angular_material_progress_bar__WEBPACK_IMPORTED_MODULE_4__["MatProgressBarModule"],
                app_core_core_module__WEBPACK_IMPORTED_MODULE_10__["CoreModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_2__["FormsModule"],
                _angular_flex_layout__WEBPACK_IMPORTED_MODULE_11__["FlexLayoutModule"],
                app_installer_installer_module__WEBPACK_IMPORTED_MODULE_12__["InstallerModule"],
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_0__["TranslateModule"]
            ],
            providers: [
                _template_filter_pipe__WEBPACK_IMPORTED_MODULE_9__["TemplateFilterPipe"],
                _data_content_types_processor_service__WEBPACK_IMPORTED_MODULE_14__["ContentTypesProcessor"]
            ],
            declarations: [
                _template_picker_component__WEBPACK_IMPORTED_MODULE_8__["TemplatePickerComponent"],
                _template_filter_pipe__WEBPACK_IMPORTED_MODULE_9__["TemplateFilterPipe"],
                _debug_pipe__WEBPACK_IMPORTED_MODULE_13__["DebugPipe"],
            ]
        })
    ], TemplatePickerModule);
    return TemplatePickerModule;
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
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! rxjs/operators */ "./node_modules/rxjs/_esm2015/operators/index.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm2015/core.js");
/* harmony import */ var _angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser-dynamic */ "./node_modules/@angular/platform-browser-dynamic/fesm2015/platform-browser-dynamic.js");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./app/app.module */ "./src/app/app.module.ts");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./environments/environment */ "./src/environments/environment.ts");
/* harmony import */ var _app_core_boot_control__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./app/core/boot-control */ "./src/app/core/boot-control.ts");
/* harmony import */ var app_core_log__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! app/core/log */ "./src/app/core/log.ts");







if (_environments_environment__WEBPACK_IMPORTED_MODULE_4__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["enableProdMode"])();
}
app_core_log__WEBPACK_IMPORTED_MODULE_6__["log"].add('loading main.ts');
var platform = Object(_angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__["platformBrowserDynamic"])();
function init() {
    app_core_log__WEBPACK_IMPORTED_MODULE_6__["log"].add('init()');
    try {
        // kill listeners
        if (!platform.destroyed)
            platform.destroy();
    }
    catch (e) {
        console.log('platform destroy error', e);
    }
    // must re-create the object here, otherwise AOT compiler optimizations
    // break these lines of code
    Object(_angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__["platformBrowserDynamic"])().bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_3__["AppModule"])
        .then(function () { return window.appBootstrap && window.appBootstrap(); })
        .catch(function (err) { return console.error('NG Bootstrap Error =>', err); });
}
// provide hook for outside reboot calls
var bootController = window.bootController = _app_core_boot_control__WEBPACK_IMPORTED_MODULE_5__["BootController"].getRebootController();
// Init on reboot request.
bootController.rebootRequest$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_0__["startWith"])(true)) // Init on first load.
    .subscribe(function () { return init(); });


/***/ }),

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! C:\Projects\2sxc-ui\projects\quick-dialog\src\main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map