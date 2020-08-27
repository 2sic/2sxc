(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["content-export-content-export-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-export/content-export.component.html":
/*!**************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/content-export/content-export.component.html ***!
  \**************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Export Content / Data</div>\r\n</div>\r\n\r\n<p class=\"dialog-description\">\r\n  This will generate an XML file which you can edit in Excel. If you just want to import new data, use this to\r\n  export the schema that you can then fill in using Excel. Please visit\r\n  <a href=\"http://2sxc.org/help\" target=\"_blank\">http://2sxc.org/help</a> for more instructions.\r\n</p>\r\n\r\n<form #ngForm=\"ngForm\" class=\"dialog-form\">\r\n  <div class=\"dialog-form-content fancy-scrollbar-light\">\r\n    <div>\r\n      <mat-form-field appearance=\"standard\" color=\"accent\">\r\n        <mat-label>Languages</mat-label>\r\n        <mat-select [(ngModel)]=\"formValues.language\" name=\"Language\">\r\n          <mat-option value=\"\">All</mat-option>\r\n          <mat-option *ngFor=\"let lang of languages\" [value]=\"lang.key\">{{ lang.key }}</mat-option>\r\n        </mat-select>\r\n      </mat-form-field>\r\n    </div>\r\n\r\n    <div>\r\n      <p class=\"field-label\">Export data</p>\r\n      <mat-radio-group [(ngModel)]=\"formValues.recordExport\" name=\"RecordExport\">\r\n        <mat-radio-button value=\"Blank\">No, just export blank data schema (for new data import)\r\n        </mat-radio-button>\r\n        <mat-radio-button value=\"All\">Yes, export all content-items</mat-radio-button>\r\n        <mat-radio-button *ngIf=\"hasIdList\" value=\"Selection\">Export selected {{itemIds.length}}\r\n          items</mat-radio-button>\r\n      </mat-radio-group>\r\n    </div>\r\n\r\n    <div>\r\n      <p class=\"field-label\">Value references to other languages</p>\r\n      <mat-radio-group [(ngModel)]=\"formValues.languageReferences\" name=\"LanguageReferences\"\r\n        [disabled]=\"formValues.recordExport === 'Blank'\">\r\n        <mat-radio-button value=\"Link\">Keep references to other languages (for re-import)\r\n        </mat-radio-button>\r\n        <mat-radio-button value=\"Resolve\">Replace references with values</mat-radio-button>\r\n      </mat-radio-group>\r\n    </div>\r\n\r\n    <div>\r\n      <p class=\"field-label\">File / page references</p>\r\n      <mat-radio-group [(ngModel)]=\"formValues.resourcesReferences\" name=\"ResourcesReferences\"\r\n        [disabled]=\"formValues.recordExport === 'Blank'\">\r\n        <mat-radio-button value=\"Link\">Keep references (for re-import, for example Page:4711)\r\n        </mat-radio-button>\r\n        <mat-radio-button value=\"Resolve\">Replace references with real URLs (for example\r\n          /Portals/0...)\r\n        </mat-radio-button>\r\n      </mat-radio-group>\r\n    </div>\r\n  </div>\r\n\r\n  <div class=\"dialog-form-actions\">\r\n    <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n    <button mat-raised-button [disabled]=\"!ngForm.form.valid\" (click)=\"exportJson()\">\r\n      Export Type Definition as Json (for developers)\r\n    </button>\r\n    <button mat-raised-button color=\"accent\" [disabled]=\"!ngForm.form.valid\" (click)=\"exportContent()\">\r\n      Export\r\n    </button>\r\n  </div>\r\n</form>\r\n");

/***/ }),

/***/ "./src/app/content-export/content-export-dialog.config.ts":
/*!****************************************************************!*\
  !*** ./src/app/content-export/content-export-dialog.config.ts ***!
  \****************************************************************/
/*! exports provided: contentExportDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentExportDialog", function() { return contentExportDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var contentExportDialog = {
    name: 'EXPORT_CONTENT_TYPE_DIALOG',
    initContext: true,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ContentExportComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./content-export.component */ "./src/app/content-export/content-export.component.ts"))];
                    case 1:
                        ContentExportComponent = (_a.sent()).ContentExportComponent;
                        return [2 /*return*/, ContentExportComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/content-export/content-export-routing.module.ts":
/*!*****************************************************************!*\
  !*** ./src/app/content-export/content-export-routing.module.ts ***!
  \*****************************************************************/
/*! exports provided: ContentExportRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentExportRoutingModule", function() { return ContentExportRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _content_export_dialog_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./content-export-dialog.config */ "./src/app/content-export/content-export-dialog.config.ts");





var routes = [
    { path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _content_export_dialog_config__WEBPACK_IMPORTED_MODULE_4__["contentExportDialog"] } },
];
var ContentExportRoutingModule = /** @class */ (function () {
    function ContentExportRoutingModule() {
    }
    ContentExportRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], ContentExportRoutingModule);
    return ContentExportRoutingModule;
}());



/***/ }),

/***/ "./src/app/content-export/content-export.component.scss":
/*!**************************************************************!*\
  !*** ./src/app/content-export/content-export.component.scss ***!
  \**************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".field-label {\n  font-size: 18px;\n  margin: 24px 0 0;\n}\n\n.mat-radio-group {\n  display: flex;\n  flex-direction: column;\n  margin: 8px 0;\n}\n\n.mat-radio-button {\n  margin: 5px;\n  font-size: 14px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWV4cG9ydC9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcY29udGVudC1leHBvcnRcXGNvbnRlbnQtZXhwb3J0LmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2NvbnRlbnQtZXhwb3J0L2NvbnRlbnQtZXhwb3J0LmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsZUFBQTtFQUNBLGdCQUFBO0FDQ0Y7O0FERUE7RUFDRSxhQUFBO0VBQ0Esc0JBQUE7RUFDQSxhQUFBO0FDQ0Y7O0FERUE7RUFDRSxXQUFBO0VBQ0EsZUFBQTtBQ0NGIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb250ZW50LWV4cG9ydC9jb250ZW50LWV4cG9ydC5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5maWVsZC1sYWJlbCB7XHJcbiAgZm9udC1zaXplOiAxOHB4O1xyXG4gIG1hcmdpbjogMjRweCAwIDA7XHJcbn1cclxuXHJcbi5tYXQtcmFkaW8tZ3JvdXAge1xyXG4gIGRpc3BsYXk6IGZsZXg7XHJcbiAgZmxleC1kaXJlY3Rpb246IGNvbHVtbjtcclxuICBtYXJnaW46IDhweCAwO1xyXG59XHJcblxyXG4ubWF0LXJhZGlvLWJ1dHRvbiB7XHJcbiAgbWFyZ2luOiA1cHg7XHJcbiAgZm9udC1zaXplOiAxNHB4O1xyXG59XHJcbiIsIi5maWVsZC1sYWJlbCB7XG4gIGZvbnQtc2l6ZTogMThweDtcbiAgbWFyZ2luOiAyNHB4IDAgMDtcbn1cblxuLm1hdC1yYWRpby1ncm91cCB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XG4gIG1hcmdpbjogOHB4IDA7XG59XG5cbi5tYXQtcmFkaW8tYnV0dG9uIHtcbiAgbWFyZ2luOiA1cHg7XG4gIGZvbnQtc2l6ZTogMTRweDtcbn0iXX0= */");

/***/ }),

/***/ "./src/app/content-export/content-export.component.ts":
/*!************************************************************!*\
  !*** ./src/app/content-export/content-export.component.ts ***!
  \************************************************************/
/*! exports provided: ContentExportComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentExportComponent", function() { return ContentExportComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../app-administration/services/content-export.service */ "./src/app/app-administration/services/content-export.service.ts");
/* harmony import */ var _shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");






var ContentExportComponent = /** @class */ (function () {
    function ContentExportComponent(dialogRef, route, contentExportService) {
        this.dialogRef = dialogRef;
        this.route = route;
        this.contentExportService = contentExportService;
        this.hostClass = 'dialog-component';
        this.languages = JSON.parse(sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_5__["keyLangs"]));
        this.hasIdList = false;
        var selectedIds = this.route.snapshot.paramMap.get('selectedIds');
        this.hasIdList = !!selectedIds;
        if (this.hasIdList) {
            this.itemIds = selectedIds.split(',').map(function (id) { return parseInt(id, 10); });
        }
        this.formValues = {
            defaultLanguage: sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_5__["keyLangPri"]),
            contentTypeStaticName: this.route.snapshot.paramMap.get('contentTypeStaticName'),
            language: '',
            recordExport: this.hasIdList ? 'Selection' : 'All',
            languageReferences: 'Link',
            resourcesReferences: 'Link',
        };
    }
    ContentExportComponent.prototype.ngOnInit = function () {
    };
    ContentExportComponent.prototype.exportContent = function () {
        this.contentExportService.exportContent(this.formValues, this.hasIdList && this.formValues.recordExport === 'Selection' ? this.itemIds : null);
    };
    ContentExportComponent.prototype.exportJson = function () {
        this.contentExportService.exportJson(this.formValues.contentTypeStaticName);
    };
    ContentExportComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ContentExportComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"] },
        { type: _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_4__["ContentExportService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ContentExportComponent.prototype, "hostClass", void 0);
    ContentExportComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-export',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-export.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/content-export/content-export.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-export.component.scss */ "./src/app/content-export/content-export.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"],
            _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_4__["ContentExportService"]])
    ], ContentExportComponent);
    return ContentExportComponent;
}());



/***/ }),

/***/ "./src/app/content-export/content-export.module.ts":
/*!*********************************************************!*\
  !*** ./src/app/content-export/content-export.module.ts ***!
  \*********************************************************/
/*! exports provided: ContentExportModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentExportModule", function() { return ContentExportModule; });
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
/* harmony import */ var _angular_material_radio__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/radio */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/radio.js");
/* harmony import */ var _content_export_routing_module__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./content-export-routing.module */ "./src/app/content-export/content-export-routing.module.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _content_export_component__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./content-export.component */ "./src/app/content-export/content-export.component.ts");
/* harmony import */ var _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../app-administration/services/content-export.service */ "./src/app/app-administration/services/content-export.service.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
















var ContentExportModule = /** @class */ (function () {
    function ContentExportModule() {
    }
    ContentExportModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _content_export_component__WEBPACK_IMPORTED_MODULE_13__["ContentExportComponent"],
            ],
            entryComponents: [
                _content_export_component__WEBPACK_IMPORTED_MODULE_13__["ContentExportComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _content_export_routing_module__WEBPACK_IMPORTED_MODULE_11__["ContentExportRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_12__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__["MatTooltipModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_3__["FormsModule"],
                _angular_material_input__WEBPACK_IMPORTED_MODULE_8__["MatInputModule"],
                _angular_material_select__WEBPACK_IMPORTED_MODULE_9__["MatSelectModule"],
                _angular_material_radio__WEBPACK_IMPORTED_MODULE_10__["MatRadioModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_15__["Context"],
                _app_administration_services_content_export_service__WEBPACK_IMPORTED_MODULE_14__["ContentExportService"],
            ]
        })
    ], ContentExportModule);
    return ContentExportModule;
}());



/***/ })

}]);
//# sourceMappingURL=content-export-content-export-module.js.map