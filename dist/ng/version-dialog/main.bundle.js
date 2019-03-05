webpackJsonp([1,4],{

/***/ 113:
/***/ (function(module, exports) {

function webpackEmptyContext(req) {
	throw new Error("Cannot find module '" + req + "'.");
}
webpackEmptyContext.keys = function() { return []; };
webpackEmptyContext.resolve = webpackEmptyContext;
module.exports = webpackEmptyContext;
webpackEmptyContext.id = 113;


/***/ }),

/***/ 114:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__ = __webpack_require__(135);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__app_app_module__ = __webpack_require__(138);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__environments_environment__ = __webpack_require__(139);




if (__WEBPACK_IMPORTED_MODULE_3__environments_environment__["a" /* environment */].production) {
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["a" /* enableProdMode */])();
}
__webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])().bootstrapModule(__WEBPACK_IMPORTED_MODULE_2__app_app_module__["a" /* AppModule */]);
//# sourceMappingURL=main.js.map

/***/ }),

/***/ 137:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_material__ = __webpack_require__(51);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_app_dialog_component__ = __webpack_require__(86);
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
    function AppComponent(dialog) {
        this.dialog = dialog;
        this.dialog.open(__WEBPACK_IMPORTED_MODULE_2_app_dialog_component__["a" /* DialogComponent */]).afterClosed()
            .subscribe(function () { return window.parent.postMessage('closeFrame', window.origin); });
    }
    return AppComponent;
}());
AppComponent = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_4" /* Component */])({
        selector: 'app-root',
        template: '',
        styles: [__webpack_require__(194)]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_material__["d" /* MdDialog */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_material__["d" /* MdDialog */]) === "function" && _a || Object])
], AppComponent);

var _a;
//# sourceMappingURL=app.component.js.map

/***/ }),

/***/ 138:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser_animations__ = __webpack_require__(136);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser__ = __webpack_require__(18);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_forms__ = __webpack_require__(84);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_http__ = __webpack_require__(85);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_material__ = __webpack_require__(51);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__angular_flex_layout__ = __webpack_require__(132);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__app_component__ = __webpack_require__(137);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__dialog_component__ = __webpack_require__(86);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppModule; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};









var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_2__angular_core__["b" /* NgModule */])({
        declarations: [
            __WEBPACK_IMPORTED_MODULE_7__app_component__["a" /* AppComponent */],
            __WEBPACK_IMPORTED_MODULE_8__dialog_component__["a" /* DialogComponent */],
            __WEBPACK_IMPORTED_MODULE_8__dialog_component__["b" /* ConfirmRestoreDialog */],
        ],
        entryComponents: [
            __WEBPACK_IMPORTED_MODULE_8__dialog_component__["a" /* DialogComponent */],
            __WEBPACK_IMPORTED_MODULE_8__dialog_component__["b" /* ConfirmRestoreDialog */],
        ],
        imports: [
            __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser__["a" /* BrowserModule */],
            __WEBPACK_IMPORTED_MODULE_3__angular_forms__["a" /* FormsModule */],
            __WEBPACK_IMPORTED_MODULE_4__angular_http__["a" /* HttpModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_material__["a" /* MdDialogModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_material__["b" /* MdToolbarModule */],
            __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser_animations__["a" /* BrowserAnimationsModule */],
            __WEBPACK_IMPORTED_MODULE_5__angular_material__["c" /* MaterialModule */],
            __WEBPACK_IMPORTED_MODULE_6__angular_flex_layout__["a" /* FlexLayoutModule */],
        ],
        providers: [],
        bootstrap: [__WEBPACK_IMPORTED_MODULE_7__app_component__["a" /* AppComponent */]]
    })
], AppModule);

//# sourceMappingURL=app.module.js.map

/***/ }),

/***/ 139:
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

/***/ 194:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(33)();
// imports


// module
exports.push([module.i, ":host {\n  position: fixed;\n  left: 0;\n  top: 0;\n  right: 0;\n  bottom: 0; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 195:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(33)();
// imports


// module
exports.push([module.i, ":host {\n  width: 1000px;\n  max-width: 100%;\n  display: block;\n  background: #fafafa; }\n  :host md-toolbar {\n    background: transparent;\n    color: black; }\n    :host md-toolbar .spacer {\n      -webkit-box-flex: 1;\n          -ms-flex: 1 1 auto;\n              flex: 1 1 auto; }\n    :host md-toolbar .title {\n      font-weight: 300; }\n  :host .table {\n    padding: 8px; }\n    :host .table .header {\n      line-height: 48px;\n      font-weight: 400;\n      padding: 0 22px;\n      color: rgba(0, 0, 0, 0.6); }\n    :host .table .record md-expansion-panel.mat-expanded {\n      margin: 16px 0 !important; }\n    :host .table .record md-expansion-panel .detail {\n      padding: 8px 0;\n      line-height: 28px;\n      box-sizing: border-box; }\n      :host .table .record md-expansion-panel .detail .label {\n        color: rgba(0, 0, 0, 0.6); }\n    :host .table .record md-expansion-panel md-action-row button {\n      margin-left: 8px; }\n  :host footer {\n    padding: 16px 22px; }\n    :host footer button {\n      margin-left: 8px; }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 196:
/***/ (function(module, exports) {

module.exports = "<md-toolbar color=\"primary\">\n  <span class=\"title\">Versions of this item</span>\n  <span class=\"spacer\"></span>\n  <button md-dialog-close md-icon-button>\n    <md-icon class=\"example-icon\">close</md-icon>\n  </button>\n</md-toolbar>\n<div class=\"table\">\n  <div class=\"record\" *ngFor=\"let version of versions\">\n    <md-expansion-panel>\n      <md-expansion-panel-header>\n        <md-panel-title fxFlex=\"108px\">Version {{version.version}}</md-panel-title>\n        <md-panel-description>{{version.timestamp}}, by {{version.user}}</md-panel-description>\n      </md-expansion-panel-header>\n      <div class=\"detail\">\n        <div fxLayout=\"row\" *ngFor=\"let data of version.data\">\n          <div fxFlex=\"96px\" class=\"label\">{{data.label}}:</div>\n          <div fxFlex class=\"value\">{{data.value}}</div>\n        </div>\n      </div>\n      <md-action-row>\n        <button md-button (click)=\"restoreDraft(version)\">RESTORE AS DRAFT</button>\n        <button md-button (click)=\"restoreLive(version)\">RESTORE LIVE</button>\n      </md-action-row>\n    </md-expansion-panel>\n  </div>\n</div>"

/***/ }),

/***/ 245:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(114);


/***/ }),

/***/ 86:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_material__ = __webpack_require__(51);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DialogComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return ConfirmRestoreDialog; });
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
    function DialogComponent(dialog) {
        this.dialog = dialog;
        this.versions = [
            {
                version: 19,
                timestamp: '2017-04-02 12:42',
                user: 'Daniel01',
                data: [{
                        label: 'Title',
                        value: 'Blogging about 2017'
                    }, {
                        label: 'Released',
                        value: '2017-06-03'
                    }, {
                        label: 'Content',
                        value: 'lorem ipsum dolor sit..'
                    }]
            }, {
                version: 18,
                timestamp: '2017-03-28 17:04',
                user: 'Daniel01',
                data: [{
                        label: 'Title',
                        value: 'Blogging about 2017'
                    }, {
                        label: 'Released',
                        value: '2017-06-03'
                    }, {
                        label: 'Content',
                        value: 'lorem ipsum dolor sit..'
                    }]
            }, {
                version: 17,
                timestamp: '2017-03-26 08:09',
                user: 'Daniel01',
                data: [{
                        label: 'Title',
                        value: 'Blogging about 2017'
                    }, {
                        label: 'Released',
                        value: '2017-06-03'
                    }, {
                        label: 'Content',
                        value: 'lorem ipsum dolor sit..'
                    }]
            }, {
                version: 16,
                timestamp: '2017-03-25 10:25',
                user: 'Daniel01',
                data: [{
                        label: 'Title',
                        value: 'Blogging about 2017'
                    }, {
                        label: 'Released',
                        value: '2017-06-03'
                    }, {
                        label: 'Content',
                        value: 'lorem ipsum dolor sit..'
                    }]
            }
        ];
    }
    DialogComponent.prototype.restoreLive = function (version) {
        this.dialog
            .open(ConfirmRestoreDialog, {
            data: { version: version },
        }).afterClosed()
            .subscribe(function (res) { return res ? alert('restoring live') : undefined; });
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
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_4" /* Component */])({
        selector: 'app-dialog',
        template: __webpack_require__(196),
        styles: [__webpack_require__(195)],
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_material__["d" /* MdDialog */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_material__["d" /* MdDialog */]) === "function" && _a || Object])
], DialogComponent);

var ConfirmRestoreDialog = (function () {
    function ConfirmRestoreDialog(dialogRef, data) {
        this.dialogRef = dialogRef;
        this.data = data;
    }
    return ConfirmRestoreDialog;
}());
ConfirmRestoreDialog = __decorate([
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_4" /* Component */])({
        selector: 'confirm-restore-dialog',
        template: "\n    <div class=\"content\">\n      <div class=\"title\">Restoring {{data.isDraft ? 'draft' : 'live'}} to version <b>{{data.version.version}}</b>.</div>\n      <div fxLayout=\"row\">\n        <button md-button [md-dialog-close]=\"false\">abort</button>\n        <span fxFlex></span>\n        <button md-raised-button [md-dialog-close]=\"true\">proceed</button>\n      </div>\n    </div>\n  ",
    }),
    __param(1, __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["g" /* Inject */])(__WEBPACK_IMPORTED_MODULE_1__angular_material__["e" /* MD_DIALOG_DATA */])),
    __metadata("design:paramtypes", [typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_material__["f" /* MdDialogRef */] !== "undefined" && __WEBPACK_IMPORTED_MODULE_1__angular_material__["f" /* MdDialogRef */]) === "function" && _b || Object, Object])
], ConfirmRestoreDialog);

var Version = (function () {
    function Version() {
    }
    return Version;
}());
var _a, _b;
//# sourceMappingURL=dialog.component.js.map

/***/ })

},[245]);
//# sourceMappingURL=main.bundle.js.map