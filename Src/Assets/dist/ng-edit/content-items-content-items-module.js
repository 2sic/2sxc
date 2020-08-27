(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["content-items-content-items-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.html":
/*!*********************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.html ***!
  \*********************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Import a single JSON Item</div>\r\n</div>\r\n\r\n<div [ngSwitch]=\"viewState\">\r\n  <div *ngSwitchCase=\"1\">\r\n    <div>\r\n      <button mat-raised-button matTooltip=\"Open file browser\" (click)=\"fileInput.click()\">\r\n        <span *ngIf=\"!importFile\">Select file</span>\r\n        <span *ngIf=\"importFile\">{{ importFile.name }}</span>\r\n      </button>\r\n      <input #fileInput type=\"file\" (change)=\"fileChange($event)\" class=\"hide\" />\r\n    </div>\r\n\r\n    <div class=\"dialog-component-actions\">\r\n      <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n      <button mat-raised-button color=\"accent\" [disabled]=\"!importFile\" (click)=\"importContentItem()\">Import</button>\r\n    </div>\r\n  </div>\r\n  <div *ngSwitchCase=\"2\">\r\n    <mat-spinner mode=\"indeterminate\" diameter=\"20\" color=\"accent\"></mat-spinner>\r\n    <div class=\"dialog-component-actions\">\r\n      <button mat-raised-button disabled>Cancel</button>\r\n      <button mat-raised-button color=\"accent\" disabled>Import</button>\r\n    </div>\r\n  </div>\r\n  <div *ngSwitchCase=\"3\">\r\n    <p>Import completed!</p>\r\n    <div class=\"dialog-component-actions\">\r\n      <button mat-raised-button color=\"accent\" (click)=\"closeDialog()\">Close</button>\r\n    </div>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.html":
/*!*************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.html ***!
  \*************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Copy\" (click)=\"clone()\">\r\n    <mat-icon>file_copy</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Export\" (click)=\"export()\">\r\n    <mat-icon>cloud_download</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Delete\" (click)=\"delete()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.html":
/*!***********************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.html ***!
  \***********************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div [matTooltip]=\"encodedValue\">\r\n  <span *ngIf=\"entities\" class=\"more-entities\">{{ entities }}</span>\r\n  {{ encodedValue }}\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.html":
/*!***********************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.html ***!
  \***********************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\">\r\n  <mat-icon *ngIf=\"value.published\" matTooltip=\"Published\">visibility</mat-icon>\r\n  <mat-icon *ngIf=\"!value.published\" matTooltip=\"Not published\">visibility_off</mat-icon>\r\n  <ng-container *ngIf=\"value.metadata\">\r\n    <mat-icon class=\"meta-icon\" [matTooltip]=\"metadataTooltip\">local_offer</mat-icon>\r\n  </ng-container>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.html":
/*!*************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.html ***!
  \*************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"title\">Published</div>\r\n<mat-radio-group [(ngModel)]=\"published\" (ngModelChange)=\"filterChanged()\">\r\n  <mat-radio-button value=\"\">All</mat-radio-button>\r\n  <mat-radio-button value=\"true\">Published</mat-radio-button>\r\n  <mat-radio-button value=\"false\">Not published</mat-radio-button>\r\n</mat-radio-group>\r\n\r\n<div class=\"title\">Metadata</div>\r\n<mat-radio-group [(ngModel)]=\"metadata\" (ngModelChange)=\"filterChanged()\">\r\n  <mat-radio-button value=\"\">All</mat-radio-button>\r\n  <mat-radio-button value=\"true\">Is metadata</mat-radio-button>\r\n  <mat-radio-button value=\"false\">Is not metadata</mat-radio-button>\r\n</mat-radio-group>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/content-items.component.html":
/*!************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-items/content-items.component.html ***!
  \************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"nav-component-wrapper\">\r\n  <div mat-dialog-title>\r\n    <div class=\"dialog-title-box\">\r\n      <div>{{ contentType?.Name }} Data</div>\r\n      <button mat-icon-button matTooltip=\"Close dialog\" (click)=\"closeDialog()\">\r\n        <mat-icon>close</mat-icon>\r\n      </button>\r\n    </div>\r\n  </div>\r\n\r\n  <router-outlet></router-outlet>\r\n\r\n  <div class=\"grid-wrapper\">\r\n    <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"items\" [modules]=\"modules\" [gridOptions]=\"gridOptions\"\r\n      (gridReady)=\"onGridReady($event)\">\r\n    </ag-grid-angular>\r\n\r\n    <div class=\"actions-box\">\r\n      <button mat-icon-button matTooltip=\"Export\" (click)=\"exportContent()\">\r\n        <mat-icon>cloud_download</mat-icon>\r\n      </button>\r\n      <button mat-icon-button matTooltip=\"Import\" (click)=\"importItem()\">\r\n        <mat-icon>cloud_upload</mat-icon>\r\n      </button>\r\n      <button mat-icon-button matTooltip=\"Add metadata\" (click)=\"addMetadata()\">\r\n        <mat-icon>local_offer</mat-icon>\r\n      </button>\r\n      <button mat-icon-button matTooltip=\"Debug filter\" (click)=\"debugFilter()\">\r\n        <mat-icon>filter_list</mat-icon>\r\n      </button>\r\n    </div>\r\n\r\n    <button mat-fab mat-elevation-z24 class=\"grid-fab\" matTooltip=\"Add item\" (click)=\"editItem(null)\">\r\n      <mat-icon>add</mat-icon>\r\n    </button>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-item-import/content-item-import-dialog.config.ts":
/*!***********************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-item-import/content-item-import-dialog.config.ts ***!
  \***********************************************************************************************************/
/*! exports provided: contentItemImportDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentItemImportDialog", function() { return contentItemImportDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var contentItemImportDialog = {
    name: 'IMPORT_CONTENT_ITEM_DIALOG',
    initContext: false,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ContentItemImportComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./content-item-import.component */ "./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.ts"))];
                    case 1:
                        ContentItemImportComponent = (_a.sent()).ContentItemImportComponent;
                        return [2 /*return*/, ContentItemImportComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.scss":
/*!*********************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.scss ***!
  \*********************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC1pdGVtcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC1pdGVtLWltcG9ydC9jb250ZW50LWl0ZW0taW1wb3J0LmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.ts":
/*!*******************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.ts ***!
  \*******************************************************************************************************/
/*! exports provided: ContentItemImportComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemImportComponent", function() { return ContentItemImportComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _services_content_items_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../services/content-items.service */ "./src/app/content-items/services/content-items.service.ts");




var ContentItemImportComponent = /** @class */ (function () {
    function ContentItemImportComponent(dialogRef, contentItemsService) {
        this.dialogRef = dialogRef;
        this.contentItemsService = contentItemsService;
        this.hostClass = 'dialog-component';
        this.viewStates = {
            Default: 1,
            Waiting: 2,
            Imported: 3
        };
        this.viewState = this.viewStates.Default;
    }
    ContentItemImportComponent.prototype.ngOnInit = function () {
    };
    ContentItemImportComponent.prototype.importContentItem = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _this = this;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.viewState = this.viewStates.Waiting;
                        return [4 /*yield*/, this.contentItemsService.importItem(this.importFile)];
                    case 1:
                        (_a.sent()).subscribe({
                            next: function (res) {
                                _this.viewState = _this.viewStates.Imported;
                            },
                            error: function () {
                                _this.viewState = _this.viewStates.Default;
                            }
                        });
                        return [2 /*return*/];
                }
            });
        });
    };
    ContentItemImportComponent.prototype.fileChange = function (event) {
        this.importFile = event.target.files[0];
    };
    ContentItemImportComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ContentItemImportComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _services_content_items_service__WEBPACK_IMPORTED_MODULE_3__["ContentItemsService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ContentItemImportComponent.prototype, "hostClass", void 0);
    ContentItemImportComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-item-import',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-item-import.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-item-import.component.scss */ "./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"], _services_content_items_service__WEBPACK_IMPORTED_MODULE_3__["ContentItemsService"]])
    ], ContentItemImportComponent);
    return ContentItemImportComponent;
}());



/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.scss":
/*!*************************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.scss ***!
  \*************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC1pdGVtcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC1pdGVtcy1hY3Rpb25zL2NvbnRlbnQtaXRlbXMtYWN0aW9ucy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.ts":
/*!***********************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.ts ***!
  \***********************************************************************************************************/
/*! exports provided: ContentItemsActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemsActionsComponent", function() { return ContentItemsActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ContentItemsActionsComponent = /** @class */ (function () {
    function ContentItemsActionsComponent() {
    }
    ContentItemsActionsComponent.prototype.agInit = function (params) {
        this.params = params;
        this.item = params.data;
    };
    ContentItemsActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    ContentItemsActionsComponent.prototype.clone = function () {
        this.params.onClone(this.item);
    };
    ContentItemsActionsComponent.prototype.export = function () {
        this.params.onExport(this.item);
    };
    ContentItemsActionsComponent.prototype.delete = function () {
        this.params.onDelete(this.item);
    };
    ContentItemsActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-items-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-items-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-items-actions.component.scss */ "./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.scss")).default]
        })
    ], ContentItemsActionsComponent);
    return ContentItemsActionsComponent;
}());



/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.scss":
/*!***********************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.scss ***!
  \***********************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".more-entities {\n  padding: 0px 8px;\n  border-radius: 10px;\n  border: 1px solid rgba(29, 39, 61, 0.44);\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWl0ZW1zL2FnLWdyaWQtY29tcG9uZW50cy9jb250ZW50LWl0ZW1zLWVudGl0eS9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcY29udGVudC1pdGVtc1xcYWctZ3JpZC1jb21wb25lbnRzXFxjb250ZW50LWl0ZW1zLWVudGl0eVxcY29udGVudC1pdGVtcy1lbnRpdHkuY29tcG9uZW50LnNjc3MiLCJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC1pdGVtcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC1pdGVtcy1lbnRpdHkvY29udGVudC1pdGVtcy1lbnRpdHkuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxnQkFBQTtFQUNBLG1CQUFBO0VBQ0Esd0NBQUE7RUFDQSx5QkFBQTtLQUFBLHNCQUFBO01BQUEscUJBQUE7VUFBQSxpQkFBQTtBQ0NGIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWl0ZW1zL2FnLWdyaWQtY29tcG9uZW50cy9jb250ZW50LWl0ZW1zLWVudGl0eS9jb250ZW50LWl0ZW1zLWVudGl0eS5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5tb3JlLWVudGl0aWVzIHtcclxuICBwYWRkaW5nOiAwcHggOHB4O1xyXG4gIGJvcmRlci1yYWRpdXM6IDEwcHg7XHJcbiAgYm9yZGVyOiAxcHggc29saWQgcmdiYSgyOSwgMzksIDYxLCAwLjQ0KTtcclxuICB1c2VyLXNlbGVjdDogbm9uZTtcclxufVxyXG4iLCIubW9yZS1lbnRpdGllcyB7XG4gIHBhZGRpbmc6IDBweCA4cHg7XG4gIGJvcmRlci1yYWRpdXM6IDEwcHg7XG4gIGJvcmRlcjogMXB4IHNvbGlkIHJnYmEoMjksIDM5LCA2MSwgMC40NCk7XG4gIHVzZXItc2VsZWN0OiBub25lO1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.ts":
/*!*********************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.ts ***!
  \*********************************************************************************************************/
/*! exports provided: ContentItemsEntityComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemsEntityComponent", function() { return ContentItemsEntityComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ContentItemsEntityComponent = /** @class */ (function () {
    function ContentItemsEntityComponent() {
    }
    ContentItemsEntityComponent.prototype.agInit = function (params) {
        this.params = params;
        if (!Array.isArray(params.value)) {
            return;
        }
        this.encodedValue = this.htmlEncode(params.value.join(', '));
        if (params.colDef.allowMultiValue) {
            this.entities = params.value.length;
        }
    };
    ContentItemsEntityComponent.prototype.refresh = function (params) {
        return true;
    };
    // htmlencode strings (source: http://stackoverflow.com/a/7124052)
    ContentItemsEntityComponent.prototype.htmlEncode = function (text) {
        return text.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    };
    ContentItemsEntityComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-items-entity',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-items-entity.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-items-entity.component.scss */ "./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.scss")).default]
        })
    ], ContentItemsEntityComponent);
    return ContentItemsEntityComponent;
}());



/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.scss":
/*!***********************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.scss ***!
  \***********************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".meta-icon {\n  margin-left: 8px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWl0ZW1zL2FnLWdyaWQtY29tcG9uZW50cy9jb250ZW50LWl0ZW1zLXN0YXR1cy9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcY29udGVudC1pdGVtc1xcYWctZ3JpZC1jb21wb25lbnRzXFxjb250ZW50LWl0ZW1zLXN0YXR1c1xcY29udGVudC1pdGVtcy1zdGF0dXMuY29tcG9uZW50LnNjc3MiLCJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC1pdGVtcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC1pdGVtcy1zdGF0dXMvY29udGVudC1pdGVtcy1zdGF0dXMuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxnQkFBQTtBQ0NGIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWl0ZW1zL2FnLWdyaWQtY29tcG9uZW50cy9jb250ZW50LWl0ZW1zLXN0YXR1cy9jb250ZW50LWl0ZW1zLXN0YXR1cy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5tZXRhLWljb24ge1xyXG4gIG1hcmdpbi1sZWZ0OiA4cHg7XHJcbn1cclxuIiwiLm1ldGEtaWNvbiB7XG4gIG1hcmdpbi1sZWZ0OiA4cHg7XG59Il19 */");

/***/ }),

/***/ "./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.ts":
/*!*********************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.ts ***!
  \*********************************************************************************************************/
/*! exports provided: ContentItemsStatusComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemsStatusComponent", function() { return ContentItemsStatusComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ContentItemsStatusComponent = /** @class */ (function () {
    function ContentItemsStatusComponent() {
    }
    ContentItemsStatusComponent.prototype.agInit = function (params) {
        // spm TODO: something about data.DraftEntity and data.PublishedEntity is missing. Search in eav-ui project
        this.value = params.value;
        var item = params.data;
        if (item.Metadata) {
            this.metadataTooltip = 'Metadata'
                + ("\nType: " + item.Metadata.TargetType)
                + (item.Metadata.KeyNumber ? "\nNumber: " + item.Metadata.KeyNumber : '')
                + (item.Metadata.KeyString ? "\nString: " + item.Metadata.KeyString : '')
                + (item.Metadata.KeyGuid ? "\nGuid: " + item.Metadata.KeyGuid : '');
        }
    };
    ContentItemsStatusComponent.prototype.refresh = function (params) {
        return true;
    };
    ContentItemsStatusComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-items-status',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-items-status.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-items-status.component.scss */ "./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.scss")).default]
        })
    ], ContentItemsStatusComponent);
    return ContentItemsStatusComponent;
}());



/***/ }),

/***/ "./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.scss":
/*!*************************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.scss ***!
  \*************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".title {\n  padding: 12px 12px 0;\n}\n\n.mat-radio-group {\n  display: flex;\n  flex-direction: column;\n  justify-content: space-between;\n  overflow: hidden;\n  padding: 12px;\n  width: 160px;\n  height: 104px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWl0ZW1zL2FnLWdyaWQtY29tcG9uZW50cy9wdWItbWV0YS1maWx0ZXIvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXGNvbnRlbnQtaXRlbXNcXGFnLWdyaWQtY29tcG9uZW50c1xccHViLW1ldGEtZmlsdGVyXFxwdWItbWV0YS1maWx0ZXIuY29tcG9uZW50LnNjc3MiLCJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC1pdGVtcy9hZy1ncmlkLWNvbXBvbmVudHMvcHViLW1ldGEtZmlsdGVyL3B1Yi1tZXRhLWZpbHRlci5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNFLG9CQUFBO0FDQ0Y7O0FERUE7RUFDRSxhQUFBO0VBQ0Esc0JBQUE7RUFDQSw4QkFBQTtFQUNBLGdCQUFBO0VBQ0EsYUFBQTtFQUNBLFlBQUE7RUFDQSxhQUFBO0FDQ0YiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2NvbnRlbnQtaXRlbXMvYWctZ3JpZC1jb21wb25lbnRzL3B1Yi1tZXRhLWZpbHRlci9wdWItbWV0YS1maWx0ZXIuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIudGl0bGUge1xyXG4gIHBhZGRpbmc6IDEycHggMTJweCAwO1xyXG59XHJcblxyXG4ubWF0LXJhZGlvLWdyb3VwIHtcclxuICBkaXNwbGF5OiBmbGV4O1xyXG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XHJcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xyXG4gIG92ZXJmbG93OiBoaWRkZW47XHJcbiAgcGFkZGluZzogMTJweDtcclxuICB3aWR0aDogMTYwcHg7XHJcbiAgaGVpZ2h0OiAxMDRweDtcclxufVxyXG4iLCIudGl0bGUge1xuICBwYWRkaW5nOiAxMnB4IDEycHggMDtcbn1cblxuLm1hdC1yYWRpby1ncm91cCB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XG4gIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2VlbjtcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcbiAgcGFkZGluZzogMTJweDtcbiAgd2lkdGg6IDE2MHB4O1xuICBoZWlnaHQ6IDEwNHB4O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.ts":
/*!***********************************************************************************************!*\
  !*** ./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.ts ***!
  \***********************************************************************************************/
/*! exports provided: PubMetaFilterComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PubMetaFilterComponent", function() { return PubMetaFilterComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var PubMetaFilterComponent = /** @class */ (function () {
    function PubMetaFilterComponent() {
        this.published = '';
        this.metadata = '';
    }
    PubMetaFilterComponent.prototype.agInit = function (params) {
        this.params = params;
    };
    PubMetaFilterComponent.prototype.isFilterActive = function () {
        return this.published !== '' || this.metadata !== '';
    };
    PubMetaFilterComponent.prototype.doesFilterPass = function (params) {
        var publishedPassed = false;
        var metadataPassed = false;
        var value = this.params.valueGetter(params.node);
        if (this.published !== '') {
            if (value.published === null || value.published === undefined) {
                publishedPassed = false;
            }
            else {
                publishedPassed = value.published.toString() === this.published;
            }
        }
        else {
            publishedPassed = true;
        }
        if (this.metadata !== '') {
            if (value.metadata === null || value.metadata === undefined) {
                metadataPassed = false;
            }
            else {
                metadataPassed = value.metadata.toString() === this.metadata;
            }
        }
        else {
            metadataPassed = true;
        }
        return publishedPassed && metadataPassed;
    };
    PubMetaFilterComponent.prototype.getModel = function () {
        if (!this.isFilterActive()) {
            return;
        }
        return {
            filterType: 'pub-meta',
            published: this.published,
            metadata: this.metadata,
        };
    };
    PubMetaFilterComponent.prototype.setModel = function (model) {
        this.published = model ? model.published : '';
        this.metadata = model ? model.metadata : '';
    };
    PubMetaFilterComponent.prototype.afterGuiAttached = function (params) {
    };
    PubMetaFilterComponent.prototype.filterChanged = function () {
        this.params.filterChangedCallback();
    };
    PubMetaFilterComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-pub-meta-filter',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./pub-meta-filter.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./pub-meta-filter.component.scss */ "./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.scss")).default]
        })
    ], PubMetaFilterComponent);
    return PubMetaFilterComponent;
}());



/***/ }),

/***/ "./src/app/content-items/content-items-dialog.config.ts":
/*!**************************************************************!*\
  !*** ./src/app/content-items/content-items-dialog.config.ts ***!
  \**************************************************************/
/*! exports provided: contentItemsDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentItemsDialog", function() { return contentItemsDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var contentItemsDialog = {
    name: 'CONTENT_ITEMS_DIALOG',
    initContext: true,
    panelSize: 'large',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ContentItemsComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./content-items.component */ "./src/app/content-items/content-items.component.ts"))];
                    case 1:
                        ContentItemsComponent = (_a.sent()).ContentItemsComponent;
                        return [2 /*return*/, ContentItemsComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/content-items/content-items-routing.module.ts":
/*!***************************************************************!*\
  !*** ./src/app/content-items/content-items-routing.module.ts ***!
  \***************************************************************/
/*! exports provided: ContentItemsRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemsRoutingModule", function() { return ContentItemsRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _content_items_dialog_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./content-items-dialog.config */ "./src/app/content-items/content-items-dialog.config.ts");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../edit/edit.matcher */ "../edit/edit.matcher.ts");
/* harmony import */ var _ag_grid_components_content_item_import_content_item_import_dialog_config__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./ag-grid-components/content-item-import/content-item-import-dialog.config */ "./src/app/content-items/ag-grid-components/content-item-import/content-item-import-dialog.config.ts");







var routes = [
    {
        path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _content_items_dialog_config__WEBPACK_IMPORTED_MODULE_4__["contentItemsDialog"] }, children: [
            {
                path: 'export/:contentTypeStaticName',
                loadChildren: function () { return Promise.all(/*! import() | content-export-content-export-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("common"), __webpack_require__.e("content-export-content-export-module")]).then(__webpack_require__.bind(null, /*! ../content-export/content-export.module */ "./src/app/content-export/content-export.module.ts")).then(function (m) { return m.ContentExportModule; }); }
            },
            {
                path: 'export/:contentTypeStaticName/:selectedIds',
                loadChildren: function () { return Promise.all(/*! import() | content-export-content-export-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("common"), __webpack_require__.e("content-export-content-export-module")]).then(__webpack_require__.bind(null, /*! ../content-export/content-export.module */ "./src/app/content-export/content-export.module.ts")).then(function (m) { return m.ContentExportModule; }); }
            },
            { path: 'import', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _ag_grid_components_content_item_import_content_item_import_dialog_config__WEBPACK_IMPORTED_MODULE_6__["contentItemImportDialog"] } },
            {
                matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); }
            },
        ]
    },
];
var ContentItemsRoutingModule = /** @class */ (function () {
    function ContentItemsRoutingModule() {
    }
    ContentItemsRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], ContentItemsRoutingModule);
    return ContentItemsRoutingModule;
}());



/***/ }),

/***/ "./src/app/content-items/content-items.component.scss":
/*!************************************************************!*\
  !*** ./src/app/content-items/content-items.component.scss ***!
  \************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".actions-box {\n  margin-right: 66px;\n  margin-left: 8px;\n  display: flex;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWl0ZW1zL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxjb250ZW50LWl0ZW1zXFxjb250ZW50LWl0ZW1zLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2NvbnRlbnQtaXRlbXMvY29udGVudC1pdGVtcy5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNFLGtCQUFBO0VBQ0EsZ0JBQUE7RUFDQSxhQUFBO0FDQ0YiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2NvbnRlbnQtaXRlbXMvY29udGVudC1pdGVtcy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5hY3Rpb25zLWJveCB7XHJcbiAgbWFyZ2luLXJpZ2h0OiA2NnB4O1xyXG4gIG1hcmdpbi1sZWZ0OiA4cHg7XHJcbiAgZGlzcGxheTogZmxleDtcclxufVxyXG4iLCIuYWN0aW9ucy1ib3gge1xuICBtYXJnaW4tcmlnaHQ6IDY2cHg7XG4gIG1hcmdpbi1sZWZ0OiA4cHg7XG4gIGRpc3BsYXk6IGZsZXg7XG59Il19 */");

/***/ }),

/***/ "./src/app/content-items/content-items.component.ts":
/*!**********************************************************!*\
  !*** ./src/app/content-items/content-items.component.ts ***!
  \**********************************************************/
/*! exports provided: ContentItemsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemsComponent", function() { return ContentItemsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../app-administration/services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _services_content_items_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./services/content-items.service */ "./src/app/content-items/services/content-items.service.ts");
/* harmony import */ var _services_entities_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./services/entities.service */ "./src/app/content-items/services/entities.service.ts");
/* harmony import */ var _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../app-administration/services/content-export.service */ "./src/app/app-administration/services/content-export.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _ag_grid_components_pub_meta_filter_pub_meta_filter_component__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./ag-grid-components/pub-meta-filter/pub-meta-filter.component */ "./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.ts");
/* harmony import */ var _ag_grid_components_content_items_status_content_items_status_component__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./ag-grid-components/content-items-status/content-items-status.component */ "./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.ts");
/* harmony import */ var _ag_grid_components_content_items_actions_content_items_actions_component__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./ag-grid-components/content-items-actions/content-items-actions.component */ "./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.ts");
/* harmony import */ var _ag_grid_components_content_items_entity_content_items_entity_component__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./ag-grid-components/content-items-entity/content-items-entity.component */ "./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.ts");
/* harmony import */ var _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../shared/components/boolean-filter/boolean-filter.component */ "./src/app/shared/components/boolean-filter/boolean-filter.component.ts");
/* harmony import */ var _shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ../shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");
/* harmony import */ var _content_items_helpers__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ./content-items.helpers */ "./src/app/content-items/content-items.helpers.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ../shared/helpers/angular-console-log.helper */ "./src/app/shared/helpers/angular-console-log.helper.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");
























var ContentItemsComponent = /** @class */ (function () {
    function ContentItemsComponent(dialogRef, contentTypesService, router, route, contentItemsService, entitiesService, contentExportService, snackBar) {
        this.dialogRef = dialogRef;
        this.contentTypesService = contentTypesService;
        this.router = router;
        this.route = route;
        this.contentItemsService = contentItemsService;
        this.entitiesService = entitiesService;
        this.contentExportService = contentExportService;
        this.snackBar = snackBar;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_7__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_22__["defaultGridOptions"]), { frameworkComponents: {
                pubMetaFilterComponent: _ag_grid_components_pub_meta_filter_pub_meta_filter_component__WEBPACK_IMPORTED_MODULE_13__["PubMetaFilterComponent"],
                booleanFilterComponent: _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_17__["BooleanFilterComponent"],
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_20__["IdFieldComponent"],
                contentItemsStatusComponent: _ag_grid_components_content_items_status_content_items_status_component__WEBPACK_IMPORTED_MODULE_14__["ContentItemsStatusComponent"],
                contentItemsActionsComponent: _ag_grid_components_content_items_actions_content_items_actions_component__WEBPACK_IMPORTED_MODULE_15__["ContentItemsActionsComponent"],
                contentItemsEntityComponent: _ag_grid_components_content_items_entity_content_items_entity_component__WEBPACK_IMPORTED_MODULE_16__["ContentItemsEntityComponent"],
            } });
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_5__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild;
        this.contentTypeStaticName = this.route.snapshot.paramMap.get('contentTypeStaticName');
    }
    ContentItemsComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.fetchContentType();
        this.fetchItems();
        this.refreshOnChildClosed();
        this.contentItemsService.getColumns(this.contentTypeStaticName).subscribe(function (columns) {
            var _a;
            _this.columnDefs = _this.buildColumnDefs(columns);
            (_a = _this.gridApi) === null || _a === void 0 ? void 0 : _a.setColumnDefs(_this.columnDefs);
            var filterModel = Object(_content_items_helpers__WEBPACK_IMPORTED_MODULE_19__["buildFilterModel"])(sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_18__["keyFilters"]));
            if (filterModel) {
                Object(_shared_helpers_angular_console_log_helper__WEBPACK_IMPORTED_MODULE_21__["angularConsoleLog"])('Will try to apply filter:', filterModel);
                _this.gridApi.setFilterModel(filterModel);
            }
        });
    };
    ContentItemsComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ContentItemsComponent.prototype.onGridReady = function (params) {
        this.gridApi = params.api;
        if (this.columnDefs) {
            this.gridApi.setColumnDefs(this.columnDefs);
        }
    };
    ContentItemsComponent.prototype.fetchContentType = function () {
        var _this = this;
        this.contentTypesService.retrieveContentType(this.contentTypeStaticName).subscribe(function (contentType) {
            _this.contentType = contentType;
        });
    };
    ContentItemsComponent.prototype.fetchItems = function () {
        var _this = this;
        this.contentItemsService.getAll(this.contentTypeStaticName).subscribe(function (items) {
            _this.items = items;
        });
    };
    ContentItemsComponent.prototype.editItem = function (params) {
        var item = params === null || params === void 0 ? void 0 : params.data;
        var form = {
            items: [
                item == null
                    ? { ContentTypeName: this.contentTypeStaticName }
                    : { EntityId: item.Id }
            ],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_23__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    ContentItemsComponent.prototype.exportContent = function () {
        var filterModel = this.gridApi.getFilterModel();
        var hasFilters = Object.keys(filterModel).length > 0;
        var ids = [];
        if (hasFilters) {
            this.gridApi.forEachNodeAfterFilterAndSort(function (rowNode) {
                ids.push(rowNode.data.Id);
            });
        }
        this.router.navigate(["export/" + this.contentTypeStaticName + (ids.length > 0 ? "/" + ids : '')], { relativeTo: this.route });
    };
    ContentItemsComponent.prototype.importItem = function () {
        this.router.navigate(['import'], { relativeTo: this.route });
    };
    ContentItemsComponent.prototype.addMetadata = function () {
        var e_1, _a;
        if (!confirm('This is a special operation to add an item which is metadata for another item.'
            + ' If you didn\'t understand that, this is not for you :). Continue?')) {
            return;
        }
        var metadataKeys = Object.keys(_shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].metadata);
        var validTargetTypes = metadataKeys.map(function (metaKey) { return _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].metadata[metaKey].type; });
        var targetType = parseInt(prompt('What kind of assignment do you want?'
            + metadataKeys.map(function (metaKey) { return "\n" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].metadata[metaKey].type + ": " + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].metadata[metaKey].target; }), _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].metadata.entity.type.toString()), 10);
        if (!targetType) {
            return alert('No target type entered. Cancelled');
        }
        if (!validTargetTypes.includes(targetType)) {
            return alert('Invalid target type. Cancelled');
        }
        var key = prompt('What key do you want?');
        if (!key) {
            return alert('No key entered. Cancelled');
        }
        var keyTypeKeys = Object.keys(_shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes);
        var validKeyTypes = keyTypeKeys.map(function (keyTypeKey) { return _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes[keyTypeKey]; });
        var keyType = prompt('What key type do you want?'
            + keyTypeKeys.map(function (keyTypeKey) { return "\n" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes[keyTypeKey]; }), _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes.number);
        if (!keyType) {
            return alert('No key type entered. Cancelled');
        }
        if (!validKeyTypes.includes(keyType)) {
            return alert('Invalid key type. Cancelled');
        }
        if (keyType === _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes.number && !parseInt(key, 10)) {
            return alert('Key type number and key don\'t match. Cancelled');
        }
        var target;
        try {
            for (var metadataKeys_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(metadataKeys), metadataKeys_1_1 = metadataKeys_1.next(); !metadataKeys_1_1.done; metadataKeys_1_1 = metadataKeys_1.next()) {
                var metaKey = metadataKeys_1_1.value;
                if (targetType !== _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].metadata[metaKey].type) {
                    continue;
                }
                target = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].metadata[metaKey].target;
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (metadataKeys_1_1 && !metadataKeys_1_1.done && (_a = metadataKeys_1.return)) _a.call(metadataKeys_1);
            }
            finally { if (e_1) throw e_1.error; }
        }
        var form = {
            items: [{
                    ContentTypeName: this.contentTypeStaticName,
                    For: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({ Target: target }, (keyType === _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes.guid && { Guid: key })), (keyType === _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes.number && { Number: parseInt(key, 10) })), (keyType === _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_12__["eavConstants"].keyTypes.string && { String: key })),
                }],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_23__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    ContentItemsComponent.prototype.debugFilter = function () {
        console.warn('Current filter:', this.gridApi.getFilterModel());
        alert('Check console for filter information');
    };
    ContentItemsComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ContentItemsComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_6__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchItems();
            }
        }));
    };
    ContentItemsComponent.prototype.buildColumnDefs = function (columns) {
        var e_2, _a;
        var columnDefs = [
            {
                headerName: 'ID', field: 'Id', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
            },
            {
                headerName: 'Status', field: 'Status', width: 80, headerClass: 'dense', cellClass: 'no-outline',
                filter: 'pubMetaFilterComponent', cellRenderer: 'contentItemsStatusComponent', valueGetter: this.valueGetterStatus,
            },
            {
                headerName: 'Item (Entity)', field: '_Title', flex: 2, minWidth: 250, cellClass: 'primary-action highlight',
                sortable: true, filter: 'agTextColumnFilter', onCellClicked: this.editItem.bind(this),
            },
            {
                cellClass: 'secondary-action no-padding', width: 120, cellRenderer: 'contentItemsActionsComponent',
                cellRendererParams: {
                    onClone: this.clone.bind(this),
                    onExport: this.export.bind(this),
                    onDelete: this.delete.bind(this),
                },
            },
            {
                headerName: 'Stats', headerTooltip: 'Used by others / uses others',
                field: '_Used', width: 70, headerClass: 'dense', cellClass: 'no-outline',
                sortable: true, filter: 'agTextColumnFilter', valueGetter: this.valueGetterUsage,
            },
        ];
        try {
            for (var columns_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(columns), columns_1_1 = columns_1.next(); !columns_1_1.done; columns_1_1 = columns_1.next()) {
                var column = columns_1_1.value;
                var colDef = {
                    headerName: column.StaticName, field: column.StaticName, flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true,
                };
                switch (column.Type) {
                    case 'Entity':
                        try {
                            colDef.allowMultiValue = column.Metadata.Entity.AllowMultiValue;
                        }
                        catch (e) {
                            colDef.allowMultiValue = true;
                        }
                        colDef.cellRenderer = 'contentItemsEntityComponent';
                        colDef.valueGetter = this.valueGetterEntityField;
                        colDef.filter = 'agTextColumnFilter';
                        break;
                    case 'DateTime':
                        try {
                            colDef.useTimePicker = column.Metadata.DateTime.UseTimePicker;
                        }
                        catch (e) {
                            colDef.useTimePicker = false;
                        }
                        colDef.valueGetter = this.valueGetterDateTime;
                        colDef.filter = 'agTextColumnFilter';
                        break;
                    case 'Boolean':
                        colDef.valueGetter = this.valueGetterBoolean;
                        colDef.filter = 'booleanFilterComponent';
                        break;
                    case 'Number':
                        colDef.filter = 'agNumberColumnFilter';
                        break;
                    default:
                        colDef.filter = 'agTextColumnFilter';
                        break;
                }
                columnDefs.push(colDef);
            }
        }
        catch (e_2_1) { e_2 = { error: e_2_1 }; }
        finally {
            try {
                if (columns_1_1 && !columns_1_1.done && (_a = columns_1.return)) _a.call(columns_1);
            }
            finally { if (e_2) throw e_2.error; }
        }
        return columnDefs;
    };
    ContentItemsComponent.prototype.clone = function (item) {
        var form = {
            items: [{ ContentTypeName: this.contentTypeStaticName, DuplicateEntity: item.Id }],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_23__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    ContentItemsComponent.prototype.export = function (item) {
        this.contentExportService.exportEntity(item.Id, this.contentTypeStaticName, true);
    };
    ContentItemsComponent.prototype.delete = function (item) {
        var _this = this;
        if (!confirm("Delete '" + item._Title + "' (" + item._RepositoryId + ")?")) {
            return;
        }
        this.snackBar.open('Deleting...');
        this.entitiesService.delete(this.contentTypeStaticName, item._RepositoryId, false).subscribe({
            next: function () {
                _this.snackBar.open('Deleted', null, { duration: 2000 });
                _this.fetchItems();
            },
            error: function (err) {
                _this.snackBar.dismiss();
                if (!confirm(err.error.ExceptionMessage + "\n\nDo you want to force delete '" + item._Title + "' (" + item._RepositoryId + ")?")) {
                    return;
                }
                _this.snackBar.open('Deleting...');
                _this.entitiesService.delete(_this.contentTypeStaticName, item._RepositoryId, true).subscribe(function () {
                    _this.snackBar.open('Deleted', null, { duration: 2000 });
                    _this.fetchItems();
                });
            }
        });
    };
    ContentItemsComponent.prototype.idValueGetter = function (params) {
        var item = params.data;
        return "ID: " + item.Id + "\nRepoID: " + item._RepositoryId + "\nGUID: " + item.Guid;
    };
    ContentItemsComponent.prototype.valueGetterStatus = function (params) {
        var item = params.data;
        var published = {
            published: item.IsPublished,
            metadata: !!item.Metadata,
        };
        return published;
    };
    ContentItemsComponent.prototype.valueGetterUsage = function (params) {
        var item = params.data;
        return item._Used + " / " + item._Uses;
    };
    ContentItemsComponent.prototype.valueGetterEntityField = function (params) {
        var rawValue = params.data[params.colDef.field];
        if (rawValue.length === 0) {
            return null;
        }
        return rawValue.map(function (item) { return item.Title; });
    };
    ContentItemsComponent.prototype.valueGetterDateTime = function (params) {
        var rawValue = params.data[params.colDef.field];
        if (!rawValue) {
            return null;
        }
        // remove 'Z' and replace 'T'
        return params.colDef.useTimePicker ? rawValue.substring(0, 19).replace('T', ' ') : rawValue.substring(0, 10);
    };
    ContentItemsComponent.prototype.valueGetterBoolean = function (params) {
        var rawValue = params.data[params.colDef.field];
        if (typeof rawValue !== typeof true) {
            return null;
        }
        return rawValue.toString();
    };
    ContentItemsComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__["ContentTypesService"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_content_items_service__WEBPACK_IMPORTED_MODULE_9__["ContentItemsService"] },
        { type: _services_entities_service__WEBPACK_IMPORTED_MODULE_10__["EntitiesService"] },
        { type: _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_11__["ContentExportService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"] }
    ]; };
    ContentItemsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-items',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-items.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-items/content-items.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-items.component.scss */ "./src/app/content-items/content-items.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__["ContentTypesService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_content_items_service__WEBPACK_IMPORTED_MODULE_9__["ContentItemsService"],
            _services_entities_service__WEBPACK_IMPORTED_MODULE_10__["EntitiesService"],
            _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_11__["ContentExportService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"]])
    ], ContentItemsComponent);
    return ContentItemsComponent;
}());



/***/ }),

/***/ "./src/app/content-items/content-items.helpers.ts":
/*!********************************************************!*\
  !*** ./src/app/content-items/content-items.helpers.ts ***!
  \********************************************************/
/*! exports provided: buildFilterModel */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "buildFilterModel", function() { return buildFilterModel; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function buildFilterModel(urlFilters) {
    var e_1, _a;
    if (!urlFilters) {
        return;
    }
    // special decode if parameter was passed as base64 - this is necessary for strings containing the "+" character
    if (urlFilters.charAt(urlFilters.length - 1) === '=') {
        urlFilters = atob(urlFilters);
    }
    var parsed;
    try {
        parsed = JSON.parse(urlFilters);
    }
    catch (error) {
        console.error('Can\'t parse JSON with filters from url:', urlFilters);
    }
    if (!parsed) {
        return;
    }
    // filters can be published, metadata, string, number and boolean
    var filterModel = {};
    if (parsed.IsPublished || parsed.IsMetadata) {
        var filter = {
            filterType: 'pub-meta',
            published: parsed.IsPublished ? parsed.IsPublished : '',
            metadata: parsed.IsMetadata ? parsed.IsMetadata : '',
        };
        filterModel.Status = filter;
    }
    var filterKeys = Object.keys(parsed);
    try {
        for (var filterKeys_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(filterKeys), filterKeys_1_1 = filterKeys_1.next(); !filterKeys_1_1.done; filterKeys_1_1 = filterKeys_1.next()) {
            var key = filterKeys_1_1.value;
            if (key === 'IsPublished' || key === 'IsMetadata') {
                continue;
            }
            var value = parsed[key];
            if (typeof value === typeof '') {
                var filter = { filterType: 'text', type: 'equals', filter: value };
                filterModel[key] = filter;
            }
            else if (typeof value === typeof 0) {
                var filter = { filterType: 'number', type: 'equals', filter: value, filterTo: null };
                filterModel[key] = filter;
            }
            else if (typeof value === typeof true) {
                var filter = { filterType: 'boolean', filter: value.toString() };
                filterModel[key] = filter;
            }
        }
    }
    catch (e_1_1) { e_1 = { error: e_1_1 }; }
    finally {
        try {
            if (filterKeys_1_1 && !filterKeys_1_1.done && (_a = filterKeys_1.return)) _a.call(filterKeys_1);
        }
        finally { if (e_1) throw e_1.error; }
    }
    return filterModel;
}


/***/ }),

/***/ "./src/app/content-items/content-items.module.ts":
/*!*******************************************************!*\
  !*** ./src/app/content-items/content-items.module.ts ***!
  \*******************************************************/
/*! exports provided: ContentItemsModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemsModule", function() { return ContentItemsModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_radio__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/radio */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/radio.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/input */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/input.js");
/* harmony import */ var _angular_material_select__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/select */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/select.js");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/progress-spinner */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/progress-spinner.js");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/core */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! @ag-grid-community/angular */ "../../node_modules/@ag-grid-community/angular/__ivy_ngcc__/fesm5/ag-grid-community-angular.js");
/* harmony import */ var _ecodev_fab_speed_dial__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @ecodev/fab-speed-dial */ "../../node_modules/@ecodev/fab-speed-dial/__ivy_ngcc__/fesm5/ecodev-fab-speed-dial.js");
/* harmony import */ var _content_items_routing_module__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./content-items-routing.module */ "./src/app/content-items/content-items-routing.module.ts");
/* harmony import */ var _content_items_component__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ./content-items.component */ "./src/app/content-items/content-items.component.ts");
/* harmony import */ var _ag_grid_components_pub_meta_filter_pub_meta_filter_component__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ./ag-grid-components/pub-meta-filter/pub-meta-filter.component */ "./src/app/content-items/ag-grid-components/pub-meta-filter/pub-meta-filter.component.ts");
/* harmony import */ var _ag_grid_components_content_items_status_content_items_status_component__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ./ag-grid-components/content-items-status/content-items-status.component */ "./src/app/content-items/ag-grid-components/content-items-status/content-items-status.component.ts");
/* harmony import */ var _ag_grid_components_content_items_actions_content_items_actions_component__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./ag-grid-components/content-items-actions/content-items-actions.component */ "./src/app/content-items/ag-grid-components/content-items-actions/content-items-actions.component.ts");
/* harmony import */ var _ag_grid_components_content_items_entity_content_items_entity_component__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ./ag-grid-components/content-items-entity/content-items-entity.component */ "./src/app/content-items/ag-grid-components/content-items-entity/content-items-entity.component.ts");
/* harmony import */ var _ag_grid_components_content_item_import_content_item_import_component__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ./ag-grid-components/content-item-import/content-item-import.component */ "./src/app/content-items/ag-grid-components/content-item-import/content-item-import.component.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_24__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _services_content_items_service__WEBPACK_IMPORTED_MODULE_25__ = __webpack_require__(/*! ./services/content-items.service */ "./src/app/content-items/services/content-items.service.ts");
/* harmony import */ var _services_entities_service__WEBPACK_IMPORTED_MODULE_26__ = __webpack_require__(/*! ./services/entities.service */ "./src/app/content-items/services/entities.service.ts");
/* harmony import */ var _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_27__ = __webpack_require__(/*! ../app-administration/services/content-export.service */ "./src/app/app-administration/services/content-export.service.ts");
/* harmony import */ var _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_28__ = __webpack_require__(/*! ../app-administration/services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");





























var ContentItemsModule = /** @class */ (function () {
    function ContentItemsModule() {
    }
    ContentItemsModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _content_items_component__WEBPACK_IMPORTED_MODULE_17__["ContentItemsComponent"],
                _ag_grid_components_pub_meta_filter_pub_meta_filter_component__WEBPACK_IMPORTED_MODULE_18__["PubMetaFilterComponent"],
                _ag_grid_components_content_items_status_content_items_status_component__WEBPACK_IMPORTED_MODULE_19__["ContentItemsStatusComponent"],
                _ag_grid_components_content_items_actions_content_items_actions_component__WEBPACK_IMPORTED_MODULE_20__["ContentItemsActionsComponent"],
                _ag_grid_components_content_items_entity_content_items_entity_component__WEBPACK_IMPORTED_MODULE_21__["ContentItemsEntityComponent"],
                _ag_grid_components_content_item_import_content_item_import_component__WEBPACK_IMPORTED_MODULE_22__["ContentItemImportComponent"],
            ],
            entryComponents: [
                _content_items_component__WEBPACK_IMPORTED_MODULE_17__["ContentItemsComponent"],
                _ag_grid_components_pub_meta_filter_pub_meta_filter_component__WEBPACK_IMPORTED_MODULE_18__["PubMetaFilterComponent"],
                _ag_grid_components_content_items_status_content_items_status_component__WEBPACK_IMPORTED_MODULE_19__["ContentItemsStatusComponent"],
                _ag_grid_components_content_items_actions_content_items_actions_component__WEBPACK_IMPORTED_MODULE_20__["ContentItemsActionsComponent"],
                _ag_grid_components_content_items_entity_content_items_entity_component__WEBPACK_IMPORTED_MODULE_21__["ContentItemsEntityComponent"],
                _ag_grid_components_content_item_import_content_item_import_component__WEBPACK_IMPORTED_MODULE_22__["ContentItemImportComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _content_items_routing_module__WEBPACK_IMPORTED_MODULE_16__["ContentItemsRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_23__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_6__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_5__["MatIconModule"],
                _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_14__["AgGridModule"].withComponents([]),
                _angular_forms__WEBPACK_IMPORTED_MODULE_3__["FormsModule"],
                _angular_material_radio__WEBPACK_IMPORTED_MODULE_7__["MatRadioModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_8__["MatTooltipModule"],
                _angular_material_input__WEBPACK_IMPORTED_MODULE_9__["MatInputModule"],
                _angular_material_select__WEBPACK_IMPORTED_MODULE_10__["MatSelectModule"],
                _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_11__["MatProgressSpinnerModule"],
                _angular_material_core__WEBPACK_IMPORTED_MODULE_12__["MatRippleModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_13__["MatSnackBarModule"],
                _ecodev_fab_speed_dial__WEBPACK_IMPORTED_MODULE_15__["EcoFabSpeedDialModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_24__["Context"],
                _services_content_items_service__WEBPACK_IMPORTED_MODULE_25__["ContentItemsService"],
                _services_entities_service__WEBPACK_IMPORTED_MODULE_26__["EntitiesService"],
                _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_27__["ContentExportService"],
                _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_28__["ContentTypesService"],
            ]
        })
    ], ContentItemsModule);
    return ContentItemsModule;
}());



/***/ }),

/***/ "./src/app/shared/constants/eav.constants.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/constants/eav.constants.ts ***!
  \***************************************************/
/*! exports provided: eavConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "eavConstants", function() { return eavConstants; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var eavConstants = {
    metadata: {
        /** metadataOfAttribute */
        attribute: { type: 2, target: 'EAV Field Properties' },
        /** metadataOfApp */
        app: { type: 3, target: 'App' },
        /** metadataOfEntity */
        entity: { type: 4, target: 'Entity' },
        /** metadataOfContentType */
        contentType: { type: 5, target: 'ContentType' },
        /** metadataOfZone */
        zone: { type: 6, target: 'Zone' },
        /** metadataOfCmsObject */
        cmsObject: { type: 10, target: 'CmsObject' },
    },
    /** Loopup type for the metadata, e.g. key=80adb152-efad-4aa4-855e-74c5ef230e1f is keyType=guid */
    keyTypes: {
        guid: 'guid',
        string: 'string',
        number: 'number',
    },
    /** Scopes */
    scopes: {
        /** This is the main schema and the data you usually see is from here */
        default: { name: 'Default', value: '2SexyContent' },
        /** This contains content-types for configuration, settings and resources of the app */
        app: { name: 'System: App', value: '2SexyContent-App' },
    },
    /** Content types where templates, permissions, etc. are stored */
    contentTypes: {
        /** Content type containing app templates (views) */
        template: '2SexyContent-Template',
        /** Content type containing permissions */
        permissions: 'PermissionConfiguration',
        /** Content type containing queries */
        query: 'DataPipeline',
        /** Content type containing content type metadata (app administration > data > metadata) */
        contentType: 'ContentType',
        /** Content type containing app settings */
        settings: 'App-Settings',
        /** Content type containing app resources */
        resources: 'App-Resources',
    },
    pipelineDesigner: {
        outDataSource: {
            className: 'SexyContentTemplate',
            in: ['ListContent', 'Default'],
            name: '2sxc Target (View or API)',
            description: 'The template/script which will show this data',
            visualDesignerData: { Top: 20, Left: 200, Width: 700 }
        },
        defaultPipeline: {
            dataSources: [
                {
                    entityGuid: 'unsaved1',
                    partAssemblyAndType: 'ToSic.Eav.DataSources.Caches.ICache, ToSic.Eav.DataSources',
                    visualDesignerData: { Top: 440, Left: 440 }
                }, {
                    entityGuid: 'unsaved2',
                    partAssemblyAndType: 'ToSic.Eav.DataSources.PublishingFilter, ToSic.Eav.DataSources',
                    visualDesignerData: { Top: 300, Left: 440 }
                }, {
                    entityGuid: 'unsaved3',
                    partAssemblyAndType: 'ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent',
                    visualDesignerData: { Top: 170, Left: 440 }
                }
            ],
            streamWiring: [
                { From: 'unsaved1', Out: 'Default', To: 'unsaved2', In: 'Default' },
                { From: 'unsaved1', Out: 'Drafts', To: 'unsaved2', In: 'Drafts' },
                { From: 'unsaved1', Out: 'Published', To: 'unsaved2', In: 'Published' },
                { From: 'unsaved2', Out: 'Default', To: 'unsaved3', In: 'Default' },
                { From: 'unsaved3', Out: 'ListContent', To: 'Out', In: 'ListContent' },
                { From: 'unsaved3', Out: 'Default', To: 'Out', In: 'Default' }
            ]
        },
        testParameters: '[Demo:Demo]=true',
    },
};


/***/ })

}]);
//# sourceMappingURL=content-items-content-items-module.js.map