(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["import-app-import-app-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/import-app/import-app.component.html":
/*!******************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/import-app/import-app.component.html ***!
  \******************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Import App</div>\r\n</div>\r\n\r\n<mat-spinner *ngIf=\"isImporting\" mode=\"indeterminate\" diameter=\"20\" color=\"accent\"></mat-spinner>\r\n\r\n<div *ngIf=\"!importResult || (importResult && !importResult.Messages)\">\r\n  <p class=\"dialog-description\">\r\n    Select an app package (zip) from your computer to import an app. New apps can be downloaded on\r\n    <a href=\"http://2sxc.org/apps\">http://2sxc.org/apps</a>.\r\n    For further help visit <a href=\"http://2sxc.org/en/help?tag=import-app\" target=\"_blank\">2sxc Help</a>.\r\n  </p>\r\n\r\n  <div>\r\n    <button mat-raised-button matTooltip=\"Open file browser\" (click)=\"fileInput.click()\">\r\n      <span *ngIf=\"!importFile\">Select file</span>\r\n      <span *ngIf=\"importFile\">{{ importFile.name }}</span>\r\n    </button>\r\n    <input #fileInput type=\"file\" (change)=\"fileChange($event)\" class=\"hide\" />\r\n  </div>\r\n\r\n  <div class=\"dialog-actions-box\">\r\n    <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n    <button mat-raised-button color=\"accent\" [disabled]=\"!importFile || isImporting\"\r\n      (click)=\"importApp()\">Import</button>\r\n  </div>\r\n</div>\r\n\r\n<div *ngIf=\"importResult && importResult.Messages && !isImporting\">\r\n  <div *ngIf=\"importResult.Succeeded\" class=\"sxc-message sxc-message-info\">\r\n    The import has been done. See the messages below for more information.\r\n  </div>\r\n  <div *ngIf=\"!importResult.Succeeded\" class=\"sxc-message sxc-message-error\">\r\n    The import failed. See the messages below for more information.\r\n  </div>\r\n  <div *ngFor=\"let message of importResult.Messages\" class=\"sxc-message\" [ngClass]=\"{\r\n    'sxc-message-warning': message.MessageType == 0,\r\n    'sxc-message-success': message.MessageType == 1,\r\n    'sxc-message-error': message.MessageType == 2\r\n  }\">\r\n    {{ message.Text }}\r\n  </div>\r\n  <div class=\"dialog-actions-box\">\r\n    <button mat-raised-button color=\"accent\" (click)=\"closeDialog()\">Close</button>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "./src/app/import-app/import-app-dialog.config.ts":
/*!********************************************************!*\
  !*** ./src/app/import-app/import-app-dialog.config.ts ***!
  \********************************************************/
/*! exports provided: importAppDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "importAppDialog", function() { return importAppDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var importAppDialog = {
    name: 'IMPORT_APP_DIALOG',
    initContext: true,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ImportAppComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./import-app.component */ "./src/app/import-app/import-app.component.ts"))];
                    case 1:
                        ImportAppComponent = (_a.sent()).ImportAppComponent;
                        return [2 /*return*/, ImportAppComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/import-app/import-app-routing.module.ts":
/*!*********************************************************!*\
  !*** ./src/app/import-app/import-app-routing.module.ts ***!
  \*********************************************************/
/*! exports provided: ImportAppRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ImportAppRoutingModule", function() { return ImportAppRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _import_app_dialog_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./import-app-dialog.config */ "./src/app/import-app/import-app-dialog.config.ts");





var routes = [
    { path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _import_app_dialog_config__WEBPACK_IMPORTED_MODULE_4__["importAppDialog"] } }
];
var ImportAppRoutingModule = /** @class */ (function () {
    function ImportAppRoutingModule() {
    }
    ImportAppRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], ImportAppRoutingModule);
    return ImportAppRoutingModule;
}());



/***/ }),

/***/ "./src/app/import-app/import-app.component.scss":
/*!******************************************************!*\
  !*** ./src/app/import-app/import-app.component.scss ***!
  \******************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvaW1wb3J0LWFwcC9pbXBvcnQtYXBwLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/import-app/import-app.component.ts":
/*!****************************************************!*\
  !*** ./src/app/import-app/import-app.component.ts ***!
  \****************************************************/
/*! exports provided: ImportAppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ImportAppComponent", function() { return ImportAppComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _services_import_app_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./services/import-app.service */ "./src/app/import-app/services/import-app.service.ts");




var ImportAppComponent = /** @class */ (function () {
    function ImportAppComponent(dialogRef, importAppService) {
        this.dialogRef = dialogRef;
        this.importAppService = importAppService;
        this.isImporting = false;
    }
    ImportAppComponent.prototype.ngOnInit = function () {
    };
    ImportAppComponent.prototype.importApp = function (changedName) {
        var _this = this;
        this.isImporting = true;
        this.importAppService.importApp(this.importFile, changedName).subscribe({
            next: function (result) {
                _this.isImporting = false;
                _this.importResult = result;
                // The app could not be installed because the app-folder already exists. Install app in a different folder?
                if (_this.importResult && _this.importResult.Messages && _this.importResult.Messages[0]
                    && _this.importResult.Messages[0].MessageType === 0) {
                    var folderName = prompt(_this.importResult.Messages[0].Text + ' Would you like to install it using another folder name?');
                    if (folderName) {
                        _this.importApp(folderName);
                    }
                }
            },
            error: function (error) {
                _this.isImporting = false;
            },
        });
    };
    ImportAppComponent.prototype.fileChange = function (event) {
        this.importFile = event.target.files[0];
    };
    ImportAppComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ImportAppComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _services_import_app_service__WEBPACK_IMPORTED_MODULE_3__["ImportAppService"] }
    ]; };
    ImportAppComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-import-app',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./import-app.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/import-app/import-app.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./import-app.component.scss */ "./src/app/import-app/import-app.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"], _services_import_app_service__WEBPACK_IMPORTED_MODULE_3__["ImportAppService"]])
    ], ImportAppComponent);
    return ImportAppComponent;
}());



/***/ }),

/***/ "./src/app/import-app/import-app.module.ts":
/*!*************************************************!*\
  !*** ./src/app/import-app/import-app.module.ts ***!
  \*************************************************/
/*! exports provided: ImportAppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ImportAppModule", function() { return ImportAppModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/progress-spinner */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/progress-spinner.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _import_app_routing_module__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./import-app-routing.module */ "./src/app/import-app/import-app-routing.module.ts");
/* harmony import */ var _import_app_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./import-app.component */ "./src/app/import-app/import-app.component.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _services_import_app_service__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./services/import-app.service */ "./src/app/import-app/services/import-app.service.ts");












var ImportAppModule = /** @class */ (function () {
    function ImportAppModule() {
    }
    ImportAppModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _import_app_component__WEBPACK_IMPORTED_MODULE_8__["ImportAppComponent"],
            ],
            entryComponents: [
                _import_app_component__WEBPACK_IMPORTED_MODULE_8__["ImportAppComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _import_app_routing_module__WEBPACK_IMPORTED_MODULE_7__["ImportAppRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_10__["SharedComponentsModule"],
                _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_3__["MatProgressSpinnerModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_6__["MatTooltipModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_9__["Context"],
                _services_import_app_service__WEBPACK_IMPORTED_MODULE_11__["ImportAppService"],
            ]
        })
    ], ImportAppModule);
    return ImportAppModule;
}());



/***/ }),

/***/ "./src/app/import-app/services/import-app.service.ts":
/*!***********************************************************!*\
  !*** ./src/app/import-app/services/import-app.service.ts ***!
  \***********************************************************/
/*! exports provided: ImportAppService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ImportAppService", function() { return ImportAppService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var ImportAppService = /** @class */ (function () {
    function ImportAppService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ImportAppService.prototype.importApp = function (file, changedName) {
        var formData = new FormData();
        formData.append('AppId', this.context.appId.toString());
        formData.append('ZoneId', this.context.zoneId.toString());
        formData.append('File', file);
        formData.append('Name', changedName ? changedName : '');
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/ImportExport/ImportApp'), formData);
    };
    ImportAppService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ImportAppService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ImportAppService);
    return ImportAppService;
}());



/***/ })

}]);
//# sourceMappingURL=import-app-import-app-module.js.map