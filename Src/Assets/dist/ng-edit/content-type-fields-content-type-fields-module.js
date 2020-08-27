(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["content-type-fields-content-type-fields-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.html":
/*!*******************************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.html ***!
  \*******************************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Rename\" (click)=\"rename()\">\r\n    <mat-icon>settings</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Delete\" *ngIf=\"!field.IsTitle\" (click)=\"delete()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n  <div class=\"like-button disabled\" *ngIf=\"field.IsTitle\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Permissions\" *ngIf=\"showPermissions\"\r\n    (click)=\"openPermissions()\">\r\n    <mat-icon>person</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.html":
/*!*************************************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.html ***!
  \*************************************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div matRipple class=\"input-component highlight\">\r\n  <div class=\"text\">{{ value }}</div>\r\n  <div class=\"like-button\">\r\n    <mat-icon>arrow_drop_down</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.html":
/*!***************************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.html ***!
  \***************************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Use as title field\">\r\n    <mat-icon>{{ icon }}</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.html":
/*!*************************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.html ***!
  \*************************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\" [matTooltip]=\"value\">\r\n  <mat-icon>{{ icon }}</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/content-type-fields.component.html":
/*!************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/content-type-fields.component.html ***!
  \************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"nav-component-wrapper\">\r\n  <div mat-dialog-title>\r\n    <div class=\"dialog-title-box\">\r\n      <div>{{ contentType?.Name }} Fields</div>\r\n      <button mat-icon-button matTooltip=\"Close dialog\" (click)=\"closeDialog()\">\r\n        <mat-icon>close</mat-icon>\r\n      </button>\r\n    </div>\r\n  </div>\r\n\r\n  <router-outlet></router-outlet>\r\n\r\n  <div class=\"grid-wrapper\">\r\n    <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"fields\" [modules]=\"modules\" [gridOptions]=\"gridOptions\"\r\n      (rowDragEnter)=\"onRowDragEnter($event)\" (rowDragEnd)=\"onRowDragEnd($event)\" (rowDragMove)=\"onRowDragMove($event)\"\r\n      (gridReady)=\"onGridReady($event)\" (sortChanged)=\"onSortChanged($event)\" (filterChanged)=\"onFilterChanged($event)\">\r\n    </ag-grid-angular>\r\n\r\n    <button mat-fab mat-elevation-z24 class=\"grid-fab\" matTooltip=\"Add fields\" (click)=\"add()\">\r\n      <mat-icon>add</mat-icon>\r\n    </button>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.html":
/*!******************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.html ***!
  \******************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\" [ngSwitch]=\"editMode\">\r\n    <ng-container *ngSwitchCase=\"undefined\">Fields</ng-container>\r\n    <ng-container *ngSwitchCase=\"true\">Edit Field</ng-container>\r\n    <ng-container *ngSwitchCase=\"false\">Add Fields</ng-container>\r\n  </div>\r\n</div>\r\n\r\n<form #ngForm=\"ngForm\" class=\"dialog-form\">\r\n  <div class=\"dialog-form-content fancy-scrollbar-light\">\r\n    <div class=\"row-container\" *ngFor=\"let field of fields; index as i\">\r\n      <div class=\"edit-input\">\r\n        <mat-form-field appearance=\"standard\" color=\"accent\">\r\n          <mat-label>Name</mat-label>\r\n          <input matInput [pattern]=\"fieldNamePattern\" [(ngModel)]=\"field.StaticName\" [name]=\"'StaticName' + i\"\r\n            [disabled]=\"editMode\" #staticName=\"ngModel\">\r\n        </mat-form-field>\r\n        <ng-container *ngIf=\"staticName.touched && staticName.errors\">\r\n          <app-field-hint *ngIf=\"staticName.errors.pattern\" [isError]=\"true\">{{ fieldNameError }}</app-field-hint>\r\n        </ng-container>\r\n      </div>\r\n\r\n      <div class=\"edit-input\">\r\n        <mat-form-field appearance=\"standard\" color=\"accent\">\r\n          <mat-label>Data Type</mat-label>\r\n          <mat-select (selectionChange)=\"resetInputType(i); calculateInputTypeOptions(i); calculateHints(i)\"\r\n            [(ngModel)]=\"field.Type\" [name]=\"'Type' + i\" [disabled]=\"editMode\">\r\n            <mat-select-trigger>\r\n              <mat-icon class=\"type-icon\">{{ findIcon(field.Type) }}</mat-icon>\r\n              <span>{{ field.Type }}</span>\r\n            </mat-select-trigger>\r\n            <mat-option *ngFor=\"let dataType of dataTypes\" [value]=\"dataType.name\">\r\n              <mat-icon>{{ dataType.icon }}</mat-icon>\r\n              <span>{{ dataType.label }}</span>\r\n            </mat-option>\r\n          </mat-select>\r\n        </mat-form-field>\r\n        <app-field-hint>{{ dataTypeHints[i] }}</app-field-hint>\r\n      </div>\r\n\r\n      <div class=\"edit-input\">\r\n        <mat-form-field appearance=\"standard\" color=\"accent\">\r\n          <mat-label>Input Type</mat-label>\r\n          <mat-select (selectionChange)=\"calculateHints(i)\" [(ngModel)]=\"field.InputType\" [name]=\"'InputType' + i\">\r\n            <mat-option *ngFor=\"let option of filteredInputTypeOptions[i]\" [value]=\"option.inputType\">\r\n              {{ option.label }}</mat-option>\r\n          </mat-select>\r\n        </mat-form-field>\r\n        <app-field-hint>{{ inputTypeHints[i] }}</app-field-hint>\r\n      </div>\r\n    </div>\r\n  </div>\r\n\r\n  <div class=\"dialog-form-actions\">\r\n    <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n    <button mat-raised-button color=\"accent\" [disabled]=\"!ngForm.form.valid\" (click)=\"save()\">Save</button>\r\n  </div>\r\n</form>\r\n");

/***/ }),

/***/ "./src/app/app-administration/constants/field-name.patterns.ts":
/*!*********************************************************************!*\
  !*** ./src/app/app-administration/constants/field-name.patterns.ts ***!
  \*********************************************************************/
/*! exports provided: fieldNamePattern, fieldNameError */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "fieldNamePattern", function() { return fieldNamePattern; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "fieldNameError", function() { return fieldNameError; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var fieldNamePattern = /^[A-Za-z][A-Za-z0-9-]+$/;
var fieldNameError = 'Standard letters, numbers and hyphens are allowed. Must start with a letter.';


/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.scss":
/*!*******************************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.scss ***!
  \*******************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC10eXBlLWZpZWxkcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC10eXBlLWZpZWxkcy1hY3Rpb25zL2NvbnRlbnQtdHlwZS1maWVsZHMtYWN0aW9ucy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.ts":
/*!*****************************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.ts ***!
  \*****************************************************************************************************************************/
/*! exports provided: ContentTypeFieldsActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeFieldsActionsComponent", function() { return ContentTypeFieldsActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _constants_input_type_constants__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../constants/input-type.constants */ "./src/app/content-type-fields/constants/input-type.constants.ts");
/* harmony import */ var _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../constants/data-type.constants */ "./src/app/content-type-fields/constants/data-type.constants.ts");




var ContentTypeFieldsActionsComponent = /** @class */ (function () {
    function ContentTypeFieldsActionsComponent() {
    }
    ContentTypeFieldsActionsComponent.prototype.agInit = function (params) {
        this.params = params;
        this.field = params.data;
        this.showPermissions = this.field.InputType === _constants_input_type_constants__WEBPACK_IMPORTED_MODULE_2__["InputTypeConstants"].StringWysiwyg || this.field.Type === _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_3__["DataTypeConstants"].Hyperlink;
    };
    ContentTypeFieldsActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    ContentTypeFieldsActionsComponent.prototype.rename = function () {
        this.params.onRename(this.field);
    };
    ContentTypeFieldsActionsComponent.prototype.delete = function () {
        this.params.onDelete(this.field);
    };
    ContentTypeFieldsActionsComponent.prototype.openPermissions = function () {
        this.params.onOpenPermissions(this.field);
    };
    ContentTypeFieldsActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-type-fields-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-type-fields-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-type-fields-actions.component.scss */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.scss")).default]
        })
    ], ContentTypeFieldsActionsComponent);
    return ContentTypeFieldsActionsComponent;
}());



/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.scss":
/*!*************************************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.scss ***!
  \*************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC10eXBlLWZpZWxkcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC10eXBlLWZpZWxkcy1pbnB1dC10eXBlL2NvbnRlbnQtdHlwZS1maWVsZHMtaW5wdXQtdHlwZS5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.ts":
/*!***********************************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.ts ***!
  \***********************************************************************************************************************************/
/*! exports provided: ContentTypeFieldsInputTypeComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeFieldsInputTypeComponent", function() { return ContentTypeFieldsInputTypeComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ContentTypeFieldsInputTypeComponent = /** @class */ (function () {
    function ContentTypeFieldsInputTypeComponent() {
    }
    ContentTypeFieldsInputTypeComponent.prototype.agInit = function (params) {
        this.value = params.value;
    };
    ContentTypeFieldsInputTypeComponent.prototype.refresh = function (params) {
        return true;
    };
    ContentTypeFieldsInputTypeComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-type-fields-input-type',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-type-fields-input-type.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-type-fields-input-type.component.scss */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.scss")).default]
        })
    ], ContentTypeFieldsInputTypeComponent);
    return ContentTypeFieldsInputTypeComponent;
}());



/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.scss":
/*!***************************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.scss ***!
  \***************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC10eXBlLWZpZWxkcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC10eXBlLWZpZWxkcy10aXRsZS9jb250ZW50LXR5cGUtZmllbGRzLXRpdGxlLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.ts":
/*!*************************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.ts ***!
  \*************************************************************************************************************************/
/*! exports provided: ContentTypeFieldsTitleComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeFieldsTitleComponent", function() { return ContentTypeFieldsTitleComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ContentTypeFieldsTitleComponent = /** @class */ (function () {
    function ContentTypeFieldsTitleComponent() {
    }
    ContentTypeFieldsTitleComponent.prototype.agInit = function (params) {
        var value = params.value;
        this.icon = value ? 'star' : 'star_border';
    };
    ContentTypeFieldsTitleComponent.prototype.refresh = function (params) {
        return true;
    };
    ContentTypeFieldsTitleComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-type-fields-title',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-type-fields-title.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-type-fields-title.component.scss */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.scss")).default]
        })
    ], ContentTypeFieldsTitleComponent);
    return ContentTypeFieldsTitleComponent;
}());



/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.scss":
/*!*************************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.scss ***!
  \*************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC10eXBlLWZpZWxkcy9hZy1ncmlkLWNvbXBvbmVudHMvY29udGVudC10eXBlLWZpZWxkcy10eXBlL2NvbnRlbnQtdHlwZS1maWVsZHMtdHlwZS5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.ts":
/*!***********************************************************************************************************************!*\
  !*** ./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.ts ***!
  \***********************************************************************************************************************/
/*! exports provided: ContentTypeFieldsTypeComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeFieldsTypeComponent", function() { return ContentTypeFieldsTypeComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../content-type-fields.helpers */ "./src/app/content-type-fields/content-type-fields.helpers.ts");



var ContentTypeFieldsTypeComponent = /** @class */ (function () {
    function ContentTypeFieldsTypeComponent() {
    }
    ContentTypeFieldsTypeComponent.prototype.agInit = function (params) {
        this.value = params.value;
        this.icon = Object(_content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_2__["calculateTypeIcon"])(this.value);
    };
    ContentTypeFieldsTypeComponent.prototype.refresh = function (params) {
        return true;
    };
    ContentTypeFieldsTypeComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-type-fields-type',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-type-fields-type.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-type-fields-type.component.scss */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.scss")).default]
        })
    ], ContentTypeFieldsTypeComponent);
    return ContentTypeFieldsTypeComponent;
}());



/***/ }),

/***/ "./src/app/content-type-fields/content-type-fields-dialog.config.ts":
/*!**************************************************************************!*\
  !*** ./src/app/content-type-fields/content-type-fields-dialog.config.ts ***!
  \**************************************************************************/
/*! exports provided: contentTypeFieldsDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentTypeFieldsDialog", function() { return contentTypeFieldsDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var contentTypeFieldsDialog = {
    name: 'CONTENT_TYPE_FIELDS_DIALOG',
    initContext: true,
    panelSize: 'large',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ContentTypeFieldsComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./content-type-fields.component */ "./src/app/content-type-fields/content-type-fields.component.ts"))];
                    case 1:
                        ContentTypeFieldsComponent = (_a.sent()).ContentTypeFieldsComponent;
                        return [2 /*return*/, ContentTypeFieldsComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/content-type-fields/content-type-fields-routing.module.ts":
/*!***************************************************************************!*\
  !*** ./src/app/content-type-fields/content-type-fields-routing.module.ts ***!
  \***************************************************************************/
/*! exports provided: ContentTypeFieldsRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeFieldsRoutingModule", function() { return ContentTypeFieldsRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _content_type_fields_dialog_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./content-type-fields-dialog.config */ "./src/app/content-type-fields/content-type-fields-dialog.config.ts");
/* harmony import */ var _edit_content_type_fields_edit_content_type_fields_dialog_config__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./edit-content-type-fields/edit-content-type-fields-dialog.config */ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields-dialog.config.ts");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../../../edit/edit.matcher */ "../edit/edit.matcher.ts");







var routes = [
    {
        path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _content_type_fields_dialog_config__WEBPACK_IMPORTED_MODULE_4__["contentTypeFieldsDialog"] }, children: [
            { path: 'add/:contentTypeStaticName', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _edit_content_type_fields_edit_content_type_fields_dialog_config__WEBPACK_IMPORTED_MODULE_5__["editContentTypeFieldsDialog"] } },
            { path: 'update/:contentTypeStaticName/:id', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _edit_content_type_fields_edit_content_type_fields_dialog_config__WEBPACK_IMPORTED_MODULE_5__["editContentTypeFieldsDialog"] } },
            {
                path: 'permissions/:type/:keyType/:key',
                loadChildren: function () { return Promise.all(/*! import() | permissions-permissions-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("permissions-permissions-module")]).then(__webpack_require__.bind(null, /*! ../permissions/permissions.module */ "./src/app/permissions/permissions.module.ts")).then(function (m) { return m.PermissionsModule; }); }
            },
            {
                matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_6__["edit"],
                loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); }
            },
        ]
    }
];
var ContentTypeFieldsRoutingModule = /** @class */ (function () {
    function ContentTypeFieldsRoutingModule() {
    }
    ContentTypeFieldsRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], ContentTypeFieldsRoutingModule);
    return ContentTypeFieldsRoutingModule;
}());



/***/ }),

/***/ "./src/app/content-type-fields/content-type-fields.component.scss":
/*!************************************************************************!*\
  !*** ./src/app/content-type-fields/content-type-fields.component.scss ***!
  \************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC10eXBlLWZpZWxkcy9jb250ZW50LXR5cGUtZmllbGRzLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/content-type-fields/content-type-fields.component.ts":
/*!**********************************************************************!*\
  !*** ./src/app/content-type-fields/content-type-fields.component.ts ***!
  \**********************************************************************/
/*! exports provided: ContentTypeFieldsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeFieldsComponent", function() { return ContentTypeFieldsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../app-administration/services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./services/content-types-fields.service */ "./src/app/content-type-fields/services/content-types-fields.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _app_administration_constants_field_name_patterns__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../app-administration/constants/field-name.patterns */ "./src/app/app-administration/constants/field-name.patterns.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_title_content_type_fields_title_component__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-title/content-type-fields-title.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_input_type_content_type_fields_input_type_component__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_actions_content_type_fields_actions_component__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-actions/content-type-fields-actions.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_type_content_type_fields_type_component__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-type/content-type-fields-type.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.ts");
/* harmony import */ var _constants_input_type_constants__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./constants/input-type.constants */ "./src/app/content-type-fields/constants/input-type.constants.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");







// tslint:disable-next-line:max-line-length












var ContentTypeFieldsComponent = /** @class */ (function () {
    function ContentTypeFieldsComponent(dialogRef, route, router, contentTypesService, contentTypesFieldsService, snackBar, changeDetectorRef) {
        this.dialogRef = dialogRef;
        this.route = route;
        this.router = router;
        this.contentTypesService = contentTypesService;
        this.contentTypesFieldsService = contentTypesFieldsService;
        this.snackBar = snackBar;
        this.changeDetectorRef = changeDetectorRef;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_7__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_17__["defaultGridOptions"]), { getRowClass: function (params) {
                var field = params.data;
                return field.InputType === _constants_input_type_constants__WEBPACK_IMPORTED_MODULE_16__["InputTypeConstants"].EmptyDefault ? 'group-row' : '';
            }, frameworkComponents: {
                contentTypeFieldsTitleComponent: _ag_grid_components_content_type_fields_title_content_type_fields_title_component__WEBPACK_IMPORTED_MODULE_12__["ContentTypeFieldsTitleComponent"],
                contentTypeFieldsTypeComponent: _ag_grid_components_content_type_fields_type_content_type_fields_type_component__WEBPACK_IMPORTED_MODULE_15__["ContentTypeFieldsTypeComponent"],
                contentTypeFieldsInputTypeComponent: _ag_grid_components_content_type_fields_input_type_content_type_fields_input_type_component__WEBPACK_IMPORTED_MODULE_13__["ContentTypeFieldsInputTypeComponent"],
                contentTypeFieldsActionsComponent: _ag_grid_components_content_type_fields_actions_content_type_fields_actions_component__WEBPACK_IMPORTED_MODULE_14__["ContentTypeFieldsActionsComponent"],
            }, columnDefs: [
                { rowDrag: true, width: 18, cellClass: 'no-select no-padding no-outline' },
                {
                    headerName: 'Title', field: 'IsTitle', width: 42, cellClass: 'secondary-action no-padding no-outline',
                    cellRenderer: 'contentTypeFieldsTitleComponent', onCellClicked: this.setTitle.bind(this),
                },
                {
                    headerName: 'Name', field: 'StaticName', flex: 2, minWidth: 250, cellClass: 'primary-action highlight',
                    sortable: true, filter: 'agTextColumnFilter', onCellClicked: this.editFieldMetadata.bind(this),
                },
                {
                    headerName: 'Type', field: 'Type', width: 70, headerClass: 'dense', cellClass: 'no-outline', sortable: true,
                    filter: 'agTextColumnFilter', cellRenderer: 'contentTypeFieldsTypeComponent',
                },
                {
                    headerName: 'Input', field: 'InputType', width: 160, cellClass: 'secondary-action no-padding',
                    sortable: true, filter: 'agTextColumnFilter', cellRenderer: 'contentTypeFieldsInputTypeComponent',
                    onCellClicked: this.changeInputType.bind(this), valueGetter: this.inputTypeValueGetter,
                },
                {
                    width: 120, cellClass: 'secondary-action no-padding', cellRenderer: 'contentTypeFieldsActionsComponent',
                    cellRendererParams: {
                        onRename: this.rename.bind(this),
                        onDelete: this.delete.bind(this),
                        onOpenPermissions: this.openPermissions.bind(this),
                    },
                },
                {
                    headerName: 'Label', field: 'Metadata.All.Name', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Notes', field: 'Metadata.All.Notes', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
            ] });
        this.sortApplied = false;
        this.filterApplied = false;
        this.rowDragSuppressed = false;
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_5__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild;
        this.contentTypeStaticName = this.route.snapshot.paramMap.get('contentTypeStaticName');
    }
    ContentTypeFieldsComponent.prototype.ngOnInit = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _a;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _a = this;
                        return [4 /*yield*/, this.contentTypesService.retrieveContentType(this.contentTypeStaticName).toPromise()];
                    case 1:
                        _a.contentType = _b.sent();
                        return [4 /*yield*/, this.fetchFields()];
                    case 2:
                        _b.sent();
                        this.refreshOnChildClosed();
                        return [2 /*return*/];
                }
            });
        });
    };
    ContentTypeFieldsComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ContentTypeFieldsComponent.prototype.onGridReady = function (params) {
        this.gridApi = params.api;
    };
    ContentTypeFieldsComponent.prototype.onRowDragEnter = function (event) {
        this.gridApi.setEnableCellTextSelection(false);
    };
    ContentTypeFieldsComponent.prototype.onRowDragEnd = function (event) {
        var _this = this;
        this.gridApi.setSuppressRowDrag(true);
        var idArray = this.fields.map(function (field) { return field.Id; });
        this.contentTypesFieldsService.reOrder(idArray, this.contentType).subscribe(function (res) { return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(_this, void 0, void 0, function () {
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.fetchFields()];
                    case 1:
                        _a.sent();
                        this.changeDetectorRef.detectChanges();
                        this.gridApi.setEnableCellTextSelection(true);
                        this.gridApi.setSuppressRowDrag(false);
                        return [2 /*return*/];
                }
            });
        }); });
    };
    ContentTypeFieldsComponent.prototype.onRowDragMove = function (event) {
        var movingNode = event.node;
        var overNode = event.overNode;
        if (!overNode) {
            return;
        }
        var rowNeedsToMove = movingNode !== overNode;
        if (rowNeedsToMove) {
            var movingData = movingNode.data;
            var overData = overNode.data;
            var fromIndex = this.fields.indexOf(movingData);
            var toIndex = this.fields.indexOf(overData);
            this.moveInArray(this.fields, fromIndex, toIndex);
            this.gridApi.setRowData(this.fields);
            this.gridApi.clearFocusedCell();
        }
    };
    ContentTypeFieldsComponent.prototype.moveInArray = function (arr, fromIndex, toIndex) {
        var element = arr[fromIndex];
        arr.splice(fromIndex, 1);
        arr.splice(toIndex, 0, element);
    };
    ContentTypeFieldsComponent.prototype.onSortChanged = function (params) {
        var sortModel = this.gridApi.getSortModel();
        this.sortApplied = sortModel.length > 0;
        this.suppressRowDrag();
    };
    ContentTypeFieldsComponent.prototype.onFilterChanged = function (params) {
        var filterModel = this.gridApi.getFilterModel();
        var fieldsFiltered = Object.keys(filterModel);
        this.filterApplied = fieldsFiltered.length > 0;
        this.suppressRowDrag();
    };
    ContentTypeFieldsComponent.prototype.suppressRowDrag = function () {
        var shouldSuppress = this.sortApplied || this.filterApplied;
        if (shouldSuppress && !this.rowDragSuppressed) {
            this.rowDragSuppressed = true;
            this.gridApi.setSuppressRowDrag(true);
        }
        else if (!shouldSuppress && this.rowDragSuppressed) {
            this.rowDragSuppressed = false;
            this.gridApi.setSuppressRowDrag(false);
        }
    };
    ContentTypeFieldsComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ContentTypeFieldsComponent.prototype.add = function () {
        this.router.navigate(["add/" + this.contentTypeStaticName], { relativeTo: this.route });
    };
    ContentTypeFieldsComponent.prototype.inputTypeValueGetter = function (params) {
        var field = params.data;
        var inputType = field.InputType.substring(field.InputType.indexOf('-') + 1);
        return inputType;
    };
    ContentTypeFieldsComponent.prototype.fetchFields = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _a;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _a = this;
                        return [4 /*yield*/, this.contentTypesFieldsService.getFields(this.contentType).toPromise()];
                    case 1:
                        _a.fields = _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    ContentTypeFieldsComponent.prototype.editFieldMetadata = function (params) {
        var field = params.data;
        var form = {
            items: [
                this.createItemDefinition(field, 'All'),
                this.createItemDefinition(field, field.Type),
                this.createItemDefinition(field, field.InputType)
            ],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_18__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    ContentTypeFieldsComponent.prototype.createItemDefinition = function (field, metadataType) {
        return field.Metadata[metadataType] != null
            ? { EntityId: field.Metadata[metadataType].Id } // if defined, return the entity-number to edit
            : {
                ContentTypeName: '@' + metadataType,
                For: {
                    Target: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].metadata.attribute.target,
                    Number: field.Id,
                },
                Prefill: { Name: field.StaticName },
            };
    };
    ContentTypeFieldsComponent.prototype.setTitle = function (params) {
        var _this = this;
        var field = params.data;
        this.snackBar.open('Setting title...');
        this.contentTypesFieldsService.setTitle(field, this.contentType).subscribe(function () {
            _this.snackBar.open('Title set', null, { duration: 2000 });
            _this.fetchFields();
        });
    };
    ContentTypeFieldsComponent.prototype.changeInputType = function (params) {
        var field = params.data;
        this.router.navigate(["update/" + this.contentTypeStaticName + "/" + field.Id], { relativeTo: this.route });
    };
    ContentTypeFieldsComponent.prototype.rename = function (field) {
        var _this = this;
        var newName = prompt("What new name would you like for '" + field.StaticName + "' (" + field.Id + ")?", field.StaticName);
        if (newName === null) {
            return;
        }
        newName = newName.trim().replace(/\s\s+/g, ' '); // remove multiple white spaces and tabs
        if (newName === field.StaticName) {
            return;
        }
        while (!newName.match(_app_administration_constants_field_name_patterns__WEBPACK_IMPORTED_MODULE_11__["fieldNamePattern"])) {
            newName = prompt("What new name would you like for '" + field.StaticName + "' (" + field.Id + ")?\n" + _app_administration_constants_field_name_patterns__WEBPACK_IMPORTED_MODULE_11__["fieldNameError"], newName);
            if (newName === null) {
                return;
            }
            newName = newName.trim().replace(/\s\s+/g, ' '); // remove multiple white spaces and tabs
            if (newName === field.StaticName) {
                return;
            }
        }
        this.snackBar.open('Saving...');
        this.contentTypesFieldsService.rename(field, this.contentType, newName).subscribe(function () {
            _this.snackBar.open('Saved', null, { duration: 2000 });
            _this.fetchFields();
        });
    };
    ContentTypeFieldsComponent.prototype.delete = function (field) {
        var _this = this;
        if (!confirm("Are you sure you want to delete '" + field.StaticName + "' (" + field.Id + ")?")) {
            return;
        }
        this.snackBar.open('Deleting...');
        this.contentTypesFieldsService.delete(field, this.contentType).subscribe(function (res) {
            _this.snackBar.open('Deleted', null, { duration: 2000 });
            _this.fetchFields();
        });
    };
    ContentTypeFieldsComponent.prototype.openPermissions = function (field) {
        this.router.navigate(["permissions/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].metadata.attribute.type + "/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].keyTypes.number + "/" + field.Id], { relativeTo: this.route });
    };
    ContentTypeFieldsComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_6__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchFields();
            }
        }));
    };
    ContentTypeFieldsComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__["ContentTypesService"] },
        { type: _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_9__["ContentTypesFieldsService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectorRef"] }
    ]; };
    ContentTypeFieldsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-type-fields',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-type-fields.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/content-type-fields.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-type-fields.component.scss */ "./src/app/content-type-fields/content-type-fields.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__["ContentTypesService"],
            _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_9__["ContentTypesFieldsService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"],
            _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectorRef"]])
    ], ContentTypeFieldsComponent);
    return ContentTypeFieldsComponent;
}());



/***/ }),

/***/ "./src/app/content-type-fields/content-type-fields.helpers.ts":
/*!********************************************************************!*\
  !*** ./src/app/content-type-fields/content-type-fields.helpers.ts ***!
  \********************************************************************/
/*! exports provided: calculateTypeIcon */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "calculateTypeIcon", function() { return calculateTypeIcon; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./constants/data-type.constants */ "./src/app/content-type-fields/constants/data-type.constants.ts");


function calculateTypeIcon(typeName) {
    switch (typeName) {
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].String:
            return 'text_fields';
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].Entity:
            return 'share';
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].Boolean:
            return 'toggle_on';
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].Number:
            return 'dialpad';
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].Custom:
            return 'extension';
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].DateTime:
            return 'today';
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].Hyperlink:
            return 'link';
        case _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_1__["DataTypeConstants"].Empty:
            return 'crop_free';
        default:
            return 'device_unknown';
    }
}


/***/ }),

/***/ "./src/app/content-type-fields/content-type-fields.module.ts":
/*!*******************************************************************!*\
  !*** ./src/app/content-type-fields/content-type-fields.module.ts ***!
  \*******************************************************************/
/*! exports provided: ContentTypeFieldsModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeFieldsModule", function() { return ContentTypeFieldsModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/input */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/input.js");
/* harmony import */ var _angular_material_select__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/select */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/select.js");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/core */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @ag-grid-community/angular */ "../../node_modules/@ag-grid-community/angular/__ivy_ngcc__/fesm5/ag-grid-community-angular.js");
/* harmony import */ var _content_type_fields_routing_module__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./content-type-fields-routing.module */ "./src/app/content-type-fields/content-type-fields-routing.module.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _content_type_fields_component__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./content-type-fields.component */ "./src/app/content-type-fields/content-type-fields.component.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_title_content_type_fields_title_component__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-title/content-type-fields-title.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-title/content-type-fields-title.component.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_input_type_content_type_fields_input_type_component__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-input-type/content-type-fields-input-type.component.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_actions_content_type_fields_actions_component__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-actions/content-type-fields-actions.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-actions/content-type-fields-actions.component.ts");
/* harmony import */ var _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ../app-administration/services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./services/content-types-fields.service */ "./src/app/content-type-fields/services/content-types-fields.service.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _edit_content_type_fields_edit_content_type_fields_component__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ./edit-content-type-fields/edit-content-type-fields.component */ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.ts");
/* harmony import */ var _ag_grid_components_content_type_fields_type_content_type_fields_type_component__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ./ag-grid-components/content-type-fields-type/content-type-fields-type.component */ "./src/app/content-type-fields/ag-grid-components/content-type-fields-type/content-type-fields-type.component.ts");
























var ContentTypeFieldsModule = /** @class */ (function () {
    function ContentTypeFieldsModule() {
    }
    ContentTypeFieldsModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _content_type_fields_component__WEBPACK_IMPORTED_MODULE_15__["ContentTypeFieldsComponent"],
                _ag_grid_components_content_type_fields_title_content_type_fields_title_component__WEBPACK_IMPORTED_MODULE_16__["ContentTypeFieldsTitleComponent"],
                _ag_grid_components_content_type_fields_input_type_content_type_fields_input_type_component__WEBPACK_IMPORTED_MODULE_17__["ContentTypeFieldsInputTypeComponent"],
                _ag_grid_components_content_type_fields_actions_content_type_fields_actions_component__WEBPACK_IMPORTED_MODULE_18__["ContentTypeFieldsActionsComponent"],
                _edit_content_type_fields_edit_content_type_fields_component__WEBPACK_IMPORTED_MODULE_22__["EditContentTypeFieldsComponent"],
                _ag_grid_components_content_type_fields_type_content_type_fields_type_component__WEBPACK_IMPORTED_MODULE_23__["ContentTypeFieldsTypeComponent"],
            ],
            entryComponents: [
                _content_type_fields_component__WEBPACK_IMPORTED_MODULE_15__["ContentTypeFieldsComponent"],
                _ag_grid_components_content_type_fields_title_content_type_fields_title_component__WEBPACK_IMPORTED_MODULE_16__["ContentTypeFieldsTitleComponent"],
                _ag_grid_components_content_type_fields_input_type_content_type_fields_input_type_component__WEBPACK_IMPORTED_MODULE_17__["ContentTypeFieldsInputTypeComponent"],
                _ag_grid_components_content_type_fields_actions_content_type_fields_actions_component__WEBPACK_IMPORTED_MODULE_18__["ContentTypeFieldsActionsComponent"],
                _edit_content_type_fields_edit_content_type_fields_component__WEBPACK_IMPORTED_MODULE_22__["EditContentTypeFieldsComponent"],
                _ag_grid_components_content_type_fields_type_content_type_fields_type_component__WEBPACK_IMPORTED_MODULE_23__["ContentTypeFieldsTypeComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _content_type_fields_routing_module__WEBPACK_IMPORTED_MODULE_13__["ContentTypeFieldsRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_21__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__["MatTooltipModule"],
                _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_12__["AgGridModule"].withComponents([]),
                _angular_forms__WEBPACK_IMPORTED_MODULE_3__["FormsModule"],
                _angular_material_input__WEBPACK_IMPORTED_MODULE_8__["MatInputModule"],
                _angular_material_select__WEBPACK_IMPORTED_MODULE_9__["MatSelectModule"],
                _angular_material_core__WEBPACK_IMPORTED_MODULE_10__["MatRippleModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_11__["MatSnackBarModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_14__["Context"],
                _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_19__["ContentTypesService"],
                _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_20__["ContentTypesFieldsService"],
            ]
        })
    ], ContentTypeFieldsModule);
    return ContentTypeFieldsModule;
}());



/***/ }),

/***/ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields-dialog.config.ts":
/*!********************************************************************************************************!*\
  !*** ./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields-dialog.config.ts ***!
  \********************************************************************************************************/
/*! exports provided: editContentTypeFieldsDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "editContentTypeFieldsDialog", function() { return editContentTypeFieldsDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var editContentTypeFieldsDialog = {
    name: 'EDIT_CONTENT_TYPE_FIELDS_DIALOG',
    initContext: false,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var EditContentTypeFieldsComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./edit-content-type-fields.component */ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.ts"))];
                    case 1:
                        EditContentTypeFieldsComponent = (_a.sent()).EditContentTypeFieldsComponent;
                        return [2 /*return*/, EditContentTypeFieldsComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.scss":
/*!******************************************************************************************************!*\
  !*** ./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.scss ***!
  \******************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".row-container {\n  display: flex;\n  justify-content: space-between;\n}\n\n.edit-input {\n  padding-bottom: 8px;\n  width: 30%;\n}\n\n.type-icon {\n  width: inherit;\n  height: inherit;\n  margin-right: 8px;\n  font-size: inherit;\n  vertical-align: top;\n  line-height: inherit;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LXR5cGUtZmllbGRzL2VkaXQtY29udGVudC10eXBlLWZpZWxkcy9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcY29udGVudC10eXBlLWZpZWxkc1xcZWRpdC1jb250ZW50LXR5cGUtZmllbGRzXFxlZGl0LWNvbnRlbnQtdHlwZS1maWVsZHMuY29tcG9uZW50LnNjc3MiLCJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC10eXBlLWZpZWxkcy9lZGl0LWNvbnRlbnQtdHlwZS1maWVsZHMvZWRpdC1jb250ZW50LXR5cGUtZmllbGRzLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsYUFBQTtFQUNBLDhCQUFBO0FDQ0Y7O0FERUE7RUFDRSxtQkFBQTtFQUNBLFVBQUE7QUNDRjs7QURFQTtFQUNFLGNBQUE7RUFDQSxlQUFBO0VBQ0EsaUJBQUE7RUFDQSxrQkFBQTtFQUNBLG1CQUFBO0VBQ0Esb0JBQUE7QUNDRiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29udGVudC10eXBlLWZpZWxkcy9lZGl0LWNvbnRlbnQtdHlwZS1maWVsZHMvZWRpdC1jb250ZW50LXR5cGUtZmllbGRzLmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLnJvdy1jb250YWluZXIge1xyXG4gIGRpc3BsYXk6IGZsZXg7XHJcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xyXG59XHJcblxyXG4uZWRpdC1pbnB1dCB7XHJcbiAgcGFkZGluZy1ib3R0b206IDhweDtcclxuICB3aWR0aDogMzAlO1xyXG59XHJcblxyXG4udHlwZS1pY29uIHtcclxuICB3aWR0aDogaW5oZXJpdDtcclxuICBoZWlnaHQ6IGluaGVyaXQ7XHJcbiAgbWFyZ2luLXJpZ2h0OiA4cHg7XHJcbiAgZm9udC1zaXplOiBpbmhlcml0O1xyXG4gIHZlcnRpY2FsLWFsaWduOiB0b3A7XHJcbiAgbGluZS1oZWlnaHQ6IGluaGVyaXQ7XHJcbn1cclxuIiwiLnJvdy1jb250YWluZXIge1xuICBkaXNwbGF5OiBmbGV4O1xuICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWJldHdlZW47XG59XG5cbi5lZGl0LWlucHV0IHtcbiAgcGFkZGluZy1ib3R0b206IDhweDtcbiAgd2lkdGg6IDMwJTtcbn1cblxuLnR5cGUtaWNvbiB7XG4gIHdpZHRoOiBpbmhlcml0O1xuICBoZWlnaHQ6IGluaGVyaXQ7XG4gIG1hcmdpbi1yaWdodDogOHB4O1xuICBmb250LXNpemU6IGluaGVyaXQ7XG4gIHZlcnRpY2FsLWFsaWduOiB0b3A7XG4gIGxpbmUtaGVpZ2h0OiBpbmhlcml0O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.ts":
/*!****************************************************************************************************!*\
  !*** ./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.ts ***!
  \****************************************************************************************************/
/*! exports provided: EditContentTypeFieldsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EditContentTypeFieldsComponent", function() { return EditContentTypeFieldsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../../app-administration/services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../services/content-types-fields.service */ "./src/app/content-type-fields/services/content-types-fields.service.ts");
/* harmony import */ var _edit_content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./edit-content-type-fields.helpers */ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.helpers.ts");
/* harmony import */ var _app_administration_constants_field_name_patterns__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../../app-administration/constants/field-name.patterns */ "./src/app/app-administration/constants/field-name.patterns.ts");
/* harmony import */ var _content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../content-type-fields.helpers */ "./src/app/content-type-fields/content-type-fields.helpers.ts");
/* harmony import */ var _constants_input_type_constants__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../constants/input-type.constants */ "./src/app/content-type-fields/constants/input-type.constants.ts");
/* harmony import */ var _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../constants/data-type.constants */ "./src/app/content-type-fields/constants/data-type.constants.ts");















var EditContentTypeFieldsComponent = /** @class */ (function () {
    function EditContentTypeFieldsComponent(dialogRef, route, contentTypesService, contentTypesFieldsService, snackBar) {
        var _this = this;
        this.dialogRef = dialogRef;
        this.route = route;
        this.contentTypesService = contentTypesService;
        this.contentTypesFieldsService = contentTypesFieldsService;
        this.snackBar = snackBar;
        this.hostClass = 'dialog-component';
        this.fields = [];
        this.filteredInputTypeOptions = [];
        this.dataTypeHints = [];
        this.inputTypeHints = [];
        this.fieldNamePattern = _app_administration_constants_field_name_patterns__WEBPACK_IMPORTED_MODULE_11__["fieldNamePattern"];
        this.fieldNameError = _app_administration_constants_field_name_patterns__WEBPACK_IMPORTED_MODULE_11__["fieldNameError"];
        this.findIcon = _content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_12__["calculateTypeIcon"];
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_6__["Subscription"]();
        this.dialogRef.disableClose = true;
        this.subscription.add(this.dialogRef.backdropClick().subscribe(function (event) {
            if (_this.form.dirty) {
                var confirmed = confirm('You have unsaved changes. Are you sure you want to close this dialog?');
                if (!confirmed) {
                    return;
                }
            }
            _this.closeDialog();
        }));
    }
    EditContentTypeFieldsComponent.prototype.ngOnInit = function () {
        var _this = this;
        var contentTypeStaticName = this.route.snapshot.paramMap.get('contentTypeStaticName');
        var editFieldId = this.route.snapshot.paramMap.get('id') ? parseInt(this.route.snapshot.paramMap.get('id'), 10) : null;
        this.editMode = (editFieldId !== null);
        var contentType$ = this.contentTypesService.retrieveContentType(contentTypeStaticName).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["share"])());
        var fields$ = contentType$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["mergeMap"])(function (contentType) { return _this.contentTypesFieldsService.getFields(contentType); }));
        var dataTypes$ = this.contentTypesFieldsService.typeListRetrieve().pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["map"])(function (rawDataTypes) { return Object(_edit_content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_10__["calculateDataTypes"])(rawDataTypes); }));
        var inputTypes$ = this.contentTypesFieldsService.getInputTypesList();
        Object(rxjs__WEBPACK_IMPORTED_MODULE_6__["forkJoin"])([contentType$, fields$, dataTypes$, inputTypes$]).subscribe(function (joined) {
            _this.contentType = joined[0];
            var allFields = joined[1];
            _this.dataTypes = joined[2];
            _this.inputTypeOptions = joined[3];
            if (_this.editMode) {
                var editField = allFields.find(function (field) { return field.Id === editFieldId; });
                _this.fields.push(editField);
            }
            else {
                for (var i = 1; i <= 8; i++) {
                    _this.fields.push({
                        Id: 0,
                        Type: _constants_data_type_constants__WEBPACK_IMPORTED_MODULE_14__["DataTypeConstants"].String,
                        InputType: _constants_input_type_constants__WEBPACK_IMPORTED_MODULE_13__["InputTypeConstants"].StringDefault,
                        StaticName: '',
                        IsTitle: allFields.length === 0,
                        SortOrder: allFields.length + i,
                    });
                }
            }
            for (var i = 0; i < _this.fields.length; i++) {
                _this.calculateInputTypeOptions(i);
                _this.calculateHints(i);
            }
        });
    };
    EditContentTypeFieldsComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    EditContentTypeFieldsComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    EditContentTypeFieldsComponent.prototype.resetInputType = function (index) {
        this.fields[index].InputType = this.fields[index].Type.toLowerCase() + _constants_input_type_constants__WEBPACK_IMPORTED_MODULE_13__["InputTypeConstants"].DefaultSuffix;
    };
    EditContentTypeFieldsComponent.prototype.calculateInputTypeOptions = function (index) {
        var _this = this;
        this.filteredInputTypeOptions[index] = this.inputTypeOptions
            .filter(function (option) { return option.dataType === _this.fields[index].Type.toLowerCase(); });
    };
    EditContentTypeFieldsComponent.prototype.calculateHints = function (index) {
        var _this = this;
        var selectedDataType = this.dataTypes.find(function (dataType) { return dataType.name === _this.fields[index].Type; });
        var selectedInputType = this.inputTypeOptions.find(function (inputTypeOption) { return inputTypeOption.inputType === _this.fields[index].InputType; });
        this.dataTypeHints[index] = selectedDataType ? selectedDataType.description : '';
        this.inputTypeHints[index] = selectedInputType ? selectedInputType.description : '';
    };
    EditContentTypeFieldsComponent.prototype.save = function () {
        var _this = this;
        this.snackBar.open('Saving...');
        if (this.editMode) {
            var field = this.fields[0];
            this.contentTypesFieldsService.updateInputType(field.Id, field.StaticName, field.InputType).subscribe(function (res) {
                _this.snackBar.open('Saved', null, { duration: 2000 });
                _this.closeDialog();
            });
        }
        else {
            rxjs__WEBPACK_IMPORTED_MODULE_6__["of"].apply(void 0, Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__spread"])(this.fields)).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["filter"])(function (field) { return !!field.StaticName; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["concatMap"])(function (field) {
                return _this.contentTypesFieldsService.add(field, _this.contentType.Id).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["catchError"])(function (error) { return Object(rxjs__WEBPACK_IMPORTED_MODULE_6__["of"])(null); }));
            }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["toArray"])()).subscribe(function (responses) {
                _this.snackBar.open('Saved', null, { duration: 2000 });
                _this.closeDialog();
            });
        }
    };
    EditContentTypeFieldsComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"] },
        { type: _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__["ContentTypesService"] },
        { type: _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_9__["ContentTypesFieldsService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBar"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], EditContentTypeFieldsComponent.prototype, "hostClass", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ViewChild"])('ngForm', { read: _angular_forms__WEBPACK_IMPORTED_MODULE_2__["NgForm"] }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", _angular_forms__WEBPACK_IMPORTED_MODULE_2__["NgForm"])
    ], EditContentTypeFieldsComponent.prototype, "form", void 0);
    EditContentTypeFieldsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-edit-content-type-fields',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./edit-content-type-fields.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./edit-content-type-fields.component.scss */ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"],
            _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_8__["ContentTypesService"],
            _services_content_types_fields_service__WEBPACK_IMPORTED_MODULE_9__["ContentTypesFieldsService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBar"]])
    ], EditContentTypeFieldsComponent);
    return EditContentTypeFieldsComponent;
}());



/***/ }),

/***/ "./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.helpers.ts":
/*!**************************************************************************************************!*\
  !*** ./src/app/content-type-fields/edit-content-type-fields/edit-content-type-fields.helpers.ts ***!
  \**************************************************************************************************/
/*! exports provided: DataType, calculateDataTypes */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DataType", function() { return DataType; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "calculateDataTypes", function() { return calculateDataTypes; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../content-type-fields.helpers */ "./src/app/content-type-fields/content-type-fields.helpers.ts");


// tslint:disable:max-line-length
var dataTypeLabels = {
    Boolean: { label: 'Boolean (yes/no)', description: 'Yes/no or true/false values' },
    Custom: { label: 'Custom - ui-tools or custom types', description: 'Use for things like gps-pickers (which writes into multiple fields) or for custom-data which serializes something exotic into the db like an array, a custom json or anything' },
    DateTime: { label: 'Date and/or time', description: 'For date, time or combined values' },
    Empty: { label: 'Empty - for form-titles etc.', description: 'Use to structure your form' },
    Entity: { label: 'Entity (other content-items)', description: 'One or more other content-items' },
    Hyperlink: { label: 'Link / file reference', description: 'Hyperlink or reference to a picture / file' },
    Number: { label: 'Number', description: 'Any kind of number' },
    String: { label: 'Text / string', description: 'Any kind of text' },
};
// tslint:enable:max-line-length
var DataType = /** @class */ (function () {
    function DataType() {
    }
    return DataType;
}());

function calculateDataTypes(rawDataTypes) {
    var e_1, _a;
    var dataTypes = [];
    try {
        for (var rawDataTypes_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(rawDataTypes), rawDataTypes_1_1 = rawDataTypes_1.next(); !rawDataTypes_1_1.done; rawDataTypes_1_1 = rawDataTypes_1.next()) {
            var rawDataType = rawDataTypes_1_1.value;
            dataTypes.push({
                name: rawDataType,
                label: dataTypeLabels[rawDataType].label,
                icon: Object(_content_type_fields_helpers__WEBPACK_IMPORTED_MODULE_1__["calculateTypeIcon"])(rawDataType),
                description: dataTypeLabels[rawDataType].description,
            });
        }
    }
    catch (e_1_1) { e_1 = { error: e_1_1 }; }
    finally {
        try {
            if (rawDataTypes_1_1 && !rawDataTypes_1_1.done && (_a = rawDataTypes_1.return)) _a.call(rawDataTypes_1);
        }
        finally { if (e_1) throw e_1.error; }
    }
    return dataTypes;
}


/***/ }),

/***/ "./src/app/content-type-fields/services/content-types-fields.service.ts":
/*!******************************************************************************!*\
  !*** ./src/app/content-type-fields/services/content-types-fields.service.ts ***!
  \******************************************************************************/
/*! exports provided: ContentTypesFieldsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypesFieldsService", function() { return ContentTypesFieldsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");






var ContentTypesFieldsService = /** @class */ (function () {
    function ContentTypesFieldsService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ContentTypesFieldsService.prototype.typeListRetrieve = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/datatypes'), {
            params: { appid: this.context.appId.toString() }
        });
    };
    ContentTypesFieldsService.prototype.getInputTypesList = function () {
        return this.http
            .get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/inputtypes'), { params: { appid: this.context.appId.toString() } })
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (inputConfigs) {
            var inputTypeOptions = inputConfigs.map(function (config) {
                var option = {
                    dataType: config.Type.substring(0, config.Type.indexOf('-')),
                    inputType: config.Type,
                    label: config.Label,
                    description: config.Description,
                };
                return option;
            });
            return inputTypeOptions;
        }));
    };
    ContentTypesFieldsService.prototype.getFields = function (contentType) {
        return this.http
            .get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/getfields'), {
            params: { appid: this.context.appId.toString(), staticName: contentType.StaticName },
        })
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (fields) {
            var e_1, _a;
            if (fields) {
                try {
                    for (var fields_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(fields), fields_1_1 = fields_1.next(); !fields_1_1.done; fields_1_1 = fields_1.next()) {
                        var fld = fields_1_1.value;
                        if (!fld.Metadata) {
                            continue;
                        }
                        var md = fld.Metadata;
                        var allMd = md.All;
                        var typeMd = md[fld.Type];
                        var inputMd = md[fld.InputType];
                        md.merged = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, allMd), typeMd), inputMd);
                    }
                }
                catch (e_1_1) { e_1 = { error: e_1_1 }; }
                finally {
                    try {
                        if (fields_1_1 && !fields_1_1.done && (_a = fields_1.return)) _a.call(fields_1);
                    }
                    finally { if (e_1) throw e_1.error; }
                }
            }
            return fields;
        }));
    };
    ContentTypesFieldsService.prototype.reOrder = function (idArray, contentType) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/reorder'), {
            params: {
                appid: this.context.appId.toString(),
                contentTypeId: contentType.Id.toString(),
                newSortOrder: JSON.stringify(idArray),
            },
        });
    };
    ContentTypesFieldsService.prototype.setTitle = function (item, contentType) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/setTitle'), {
            params: {
                appid: this.context.appId.toString(),
                contentTypeId: contentType.Id.toString(),
                attributeId: item.Id.toString(),
            },
        });
    };
    ContentTypesFieldsService.prototype.rename = function (item, contentType, newName) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/rename'), {
            params: {
                appid: this.context.appId.toString(),
                contentTypeId: contentType.Id.toString(),
                attributeId: item.Id.toString(),
                newName: newName,
            },
        });
    };
    ContentTypesFieldsService.prototype.delete = function (item, contentType) {
        if (item.IsTitle) {
            throw new Error('Can\'t delete Title');
        }
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/deletefield'), {
            params: {
                appid: this.context.appId.toString(),
                contentTypeId: contentType.Id.toString(),
                attributeId: item.Id.toString(),
            },
        });
    };
    ContentTypesFieldsService.prototype.add = function (newField, contentTypeId) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/addfield'), {
            params: {
                AppId: this.context.appId.toString(),
                ContentTypeId: contentTypeId.toString(),
                Id: newField.Id.toString(),
                Type: newField.Type,
                InputType: newField.InputType,
                StaticName: newField.StaticName,
                IsTitle: newField.IsTitle.toString(),
                SortOrder: newField.SortOrder.toString(),
            }
        });
    };
    ContentTypesFieldsService.prototype.updateInputType = function (id, staticName, inputType) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/updateinputtype'), {
            params: { appId: this.context.appId.toString(), attributeId: id.toString(), field: staticName, inputType: inputType }
        });
    };
    ContentTypesFieldsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"] }
    ]; };
    ContentTypesFieldsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"]])
    ], ContentTypesFieldsService);
    return ContentTypesFieldsService;
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
//# sourceMappingURL=content-type-fields-content-type-fields-module.js.map