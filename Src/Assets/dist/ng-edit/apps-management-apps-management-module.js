(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["apps-management-apps-management-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.html":
/*!*******************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.html ***!
  \*******************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matTooltip=\"Flush cache\" (click)=\"flushCache()\">\r\n    <mat-icon>cached</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple *ngIf=\"app.IsApp\" matTooltip=\"Delete\" (click)=\"deleteApp()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n  <div class=\"like-button disabled\" *ngIf=\"!app.IsApp\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.html":
/*!*************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.html ***!
  \*************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\">\r\n  <mat-icon matTooltip=\"Show this app to users\" *ngIf=\"value\">visibility</mat-icon>\r\n  <mat-icon matTooltip=\"Don't show this app to users\" *ngIf=\"!value\">visibility_off</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.html":
/*!*******************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.html ***!
  \*******************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"cell-box\">\r\n  <mat-slide-toggle [checked]=\"value\" (change)=\"toggleLanguage()\"></mat-slide-toggle>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.html":
/*!***************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.html ***!
  \***************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\">\r\n  <mat-icon matTooltip=\"Feature is enabled\" *ngIf=\"value\">toggle_on</mat-icon>\r\n  <mat-icon matTooltip=\"Feature is disabled\" *ngIf=\"!value\">toggle_off</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.html":
/*!*************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.html ***!
  \*************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\">\r\n  <mat-icon *ngIf=\"value\">person</mat-icon>\r\n  <mat-icon *ngIf=\"!value\">remove</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.html":
/*!*****************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.html ***!
  \*****************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\">\r\n  <mat-icon matTooltip=\"Security Status still work-in progress\" class=\"feature-security-icon\">help</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.html":
/*!*****************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.html ***!
  \*****************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\">\r\n  <mat-icon *ngIf=\"value\">visibility</mat-icon>\r\n  <mat-icon *ngIf=\"!value\">remove</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/apps-list/apps-list.component.html":
/*!********************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/apps-list/apps-list.component.html ***!
  \********************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"grid-wrapper mat-tab-grid-wrapper\">\r\n  <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"apps\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n  </ag-grid-angular>\r\n\r\n  <eco-fab-speed-dial class=\"grid-fab\">\r\n    <eco-fab-speed-dial-trigger spin=\"true\">\r\n      <button mat-fab>\r\n        <mat-icon class=\"spin180\">add</mat-icon>\r\n      </button>\r\n    </eco-fab-speed-dial-trigger>\r\n\r\n    <eco-fab-speed-dial-actions>\r\n      <button mat-mini-fab matTooltip=\"Find more apps\" (click)=\"browseCatalog()\">\r\n        <mat-icon>search</mat-icon>\r\n      </button>\r\n      <button mat-mini-fab matTooltip=\"Import app\" (click)=\"importApp()\">\r\n        <mat-icon>cloud_upload</mat-icon>\r\n      </button>\r\n      <button mat-mini-fab matTooltip=\"Create app\" (click)=\"createApp()\">\r\n        <mat-icon>add</mat-icon>\r\n      </button>\r\n    </eco-fab-speed-dial-actions>\r\n  </eco-fab-speed-dial>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/apps-management-nav/apps-management-nav.component.html":
/*!****************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/apps-management-nav/apps-management-nav.component.html ***!
  \****************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"nav-component-wrapper\">\r\n  <div mat-dialog-title>\r\n    <div class=\"dialog-title-box\">\r\n      <div>Manage Apps in Zone {{ zoneId }}</div>\r\n      <button mat-icon-button matTooltip=\"Close dialog\" (click)=\"closeDialog()\">\r\n        <mat-icon>close</mat-icon>\r\n      </button>\r\n    </div>\r\n  </div>\r\n\r\n  <router-outlet></router-outlet>\r\n\r\n  <!-- spm NOTE: we use mat-tab-group because mat-tab-nav-bar doesn't have animations and doesn't look pretty -->\r\n  <mat-tab-group dynamicHeight color=\"accent\" (selectedTabChange)=\"changeTab($event)\" [selectedIndex]=\"tabIndex\">\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"Apps\">\r\n        <mat-icon>star_border</mat-icon>\r\n        <span>Apps</span>\r\n      </div>\r\n      <app-apps-list *matTabContent></app-apps-list>\r\n    </mat-tab>\r\n\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"Languages\">\r\n        <mat-icon>translate</mat-icon>\r\n        <span>Languages</span>\r\n      </div>\r\n      <app-enable-languages *matTabContent></app-enable-languages>\r\n    </mat-tab>\r\n\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"These settings apply to all zones/portals\">\r\n        <mat-icon>tune</mat-icon>\r\n        <span>Features</span>\r\n      </div>\r\n      <app-manage-features *matTabContent></app-manage-features>\r\n    </mat-tab>\r\n\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"Insights\">\r\n        <mat-icon>speed</mat-icon>\r\n        <span>2sxc Insights</span>\r\n      </div>\r\n      <app-sxc-insights *matTabContent></app-sxc-insights>\r\n    </mat-tab>\r\n  </mat-tab-group>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/enable-languages/enable-languages.component.html":
/*!**********************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/enable-languages/enable-languages.component.html ***!
  \**********************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"grid-wrapper mat-tab-grid-wrapper\">\r\n  <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"languages\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n  </ag-grid-angular>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/manage-features/manage-features.component.html":
/*!********************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/manage-features/manage-features.component.html ***!
  \********************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"grid-wrapper mat-tab-grid-wrapper\" [ngClass]=\"{ 'iframe-wrapper': showManagement }\">\r\n  <ag-grid-angular class=\"ag-theme-material\" [ngClass]=\"{ 'force-hide': showManagement }\" [rowData]=\"features\"\r\n    [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n  </ag-grid-angular>\r\n\r\n  <mat-spinner class=\"spinner\" *ngIf=\"showSpinner\" mode=\"indeterminate\" diameter=\"20\" color=\"accent\"></mat-spinner>\r\n\r\n  <iframe class=\"iframe\" *ngIf=\"showManagement\" [src]=\"managementUrl\"></iframe>\r\n\r\n  <button mat-fab mat-elevation-z24 class=\"grid-fab\" matTooltip=\"Manage features\" (click)=\"toggleManagement()\">\r\n    <mat-icon>tune</mat-icon>\r\n  </button>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/sxc-insights/sxc-insights.component.html":
/*!**************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/sxc-insights/sxc-insights.component.html ***!
  \**************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"cards-box\">\r\n  <mat-card class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>2sxc Insights for Super Users</mat-card-title>\r\n      <div class=\"actions-box\">\r\n        <button mat-icon-button matTooltip=\"Open 2sxc Insights\" (click)=\"openInsights()\">\r\n          <mat-icon>speed</mat-icon>\r\n        </button>\r\n      </div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      This is to access a special section to see what is really in the server memory. It's intended for extensive\r\n      debugging\r\n      - see also <a href=\"https://2sxc.org/en/blog/post/using-2sxc-insights\" target=\"_blank\">this blog post</a>.\r\n    </mat-card-content>\r\n  </mat-card>\r\n\r\n  <mat-card class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>Activate Page Level Logging</mat-card-title>\r\n      <div class=\"actions-box\"></div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      <div>\r\n        This will place insights-logs in the HTML of the user output for users with ?debug=true in the url. It can only\r\n        be activated for short periods of time.\r\n      </div>\r\n      <div class=\"activate-log-form\">\r\n        <mat-form-field appearance=\"standard\" color=\"accent\">\r\n          <mat-label>Duration in Minutes</mat-label>\r\n          <input matInput type=\"number\" min=\"0\" [pattern]=\"positiveWholeNumber\" [(ngModel)]=\"pageLogDuration\"\r\n            name=\"Duration\" #duration=\"ngModel\">\r\n        </mat-form-field>\r\n        <ng-container *ngIf=\"duration.touched && duration.errors\">\r\n          <app-field-hint *ngIf=\"duration.errors.pattern\" [isError]=\"true\">Only positive whole numbers</app-field-hint>\r\n        </ng-container>\r\n        <div class=\"actions\">\r\n          <button mat-raised-button color=\"accent\" (click)=\"activatePageLog()\"\r\n            [disabled]=\"actionsDiabled || !pageLogDuration || pageLogDuration < 0\">\r\n            Activate\r\n          </button>\r\n        </div>\r\n      </div>\r\n    </mat-card-content>\r\n  </mat-card>\r\n</div>\r\n");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.scss":
/*!*******************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.scss ***!
  \*******************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2FnLWdyaWQtY29tcG9uZW50cy9hcHBzLWxpc3QtYWN0aW9ucy9hcHBzLWxpc3QtYWN0aW9ucy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.ts ***!
  \*****************************************************************************************************/
/*! exports provided: AppsListActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppsListActionsComponent", function() { return AppsListActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var AppsListActionsComponent = /** @class */ (function () {
    function AppsListActionsComponent() {
    }
    AppsListActionsComponent.prototype.agInit = function (params) {
        this.params = params;
        this.app = params.data;
    };
    AppsListActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    AppsListActionsComponent.prototype.deleteApp = function () {
        this.params.onDelete(this.app);
    };
    AppsListActionsComponent.prototype.flushCache = function () {
        this.params.onFlush(this.app);
    };
    AppsListActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-apps-list-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./apps-list-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./apps-list-actions.component.scss */ "./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.scss")).default]
        })
    ], AppsListActionsComponent);
    return AppsListActionsComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.scss":
/*!*************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.scss ***!
  \*************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2FnLWdyaWQtY29tcG9uZW50cy9hcHBzLWxpc3Qtc2hvdy9hcHBzLWxpc3Qtc2hvdy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.ts":
/*!***********************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.ts ***!
  \***********************************************************************************************/
/*! exports provided: AppsListShowComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppsListShowComponent", function() { return AppsListShowComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var AppsListShowComponent = /** @class */ (function () {
    function AppsListShowComponent() {
    }
    AppsListShowComponent.prototype.agInit = function (params) {
        this.value = params.value;
    };
    AppsListShowComponent.prototype.refresh = function (params) {
        return true;
    };
    AppsListShowComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-apps-list-show',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./apps-list-show.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./apps-list-show.component.scss */ "./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.scss")).default]
        })
    ], AppsListShowComponent);
    return AppsListShowComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.scss":
/*!*******************************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.scss ***!
  \*******************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".cell-box {\n  padding-left: 8px;\n  padding-right: 8px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHBzLW1hbmFnZW1lbnQvYWctZ3JpZC1jb21wb25lbnRzL2VuYWJsZS1sYW5ndWFnZXMtc3RhdHVzL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxhcHBzLW1hbmFnZW1lbnRcXGFnLWdyaWQtY29tcG9uZW50c1xcZW5hYmxlLWxhbmd1YWdlcy1zdGF0dXNcXGVuYWJsZS1sYW5ndWFnZXMtc3RhdHVzLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcHMtbWFuYWdlbWVudC9hZy1ncmlkLWNvbXBvbmVudHMvZW5hYmxlLWxhbmd1YWdlcy1zdGF0dXMvZW5hYmxlLWxhbmd1YWdlcy1zdGF0dXMuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxpQkFBQTtFQUNBLGtCQUFBO0FDQ0YiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcHMtbWFuYWdlbWVudC9hZy1ncmlkLWNvbXBvbmVudHMvZW5hYmxlLWxhbmd1YWdlcy1zdGF0dXMvZW5hYmxlLWxhbmd1YWdlcy1zdGF0dXMuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuY2VsbC1ib3gge1xyXG4gIHBhZGRpbmctbGVmdDogOHB4O1xyXG4gIHBhZGRpbmctcmlnaHQ6IDhweDtcclxufVxyXG4iLCIuY2VsbC1ib3gge1xuICBwYWRkaW5nLWxlZnQ6IDhweDtcbiAgcGFkZGluZy1yaWdodDogOHB4O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.ts":
/*!*****************************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.ts ***!
  \*****************************************************************************************************************/
/*! exports provided: EnableLanguagesStatusComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EnableLanguagesStatusComponent", function() { return EnableLanguagesStatusComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var EnableLanguagesStatusComponent = /** @class */ (function () {
    function EnableLanguagesStatusComponent() {
    }
    EnableLanguagesStatusComponent.prototype.agInit = function (params) {
        this.params = params;
        this.value = params.value;
    };
    EnableLanguagesStatusComponent.prototype.refresh = function (params) {
        return true;
    };
    EnableLanguagesStatusComponent.prototype.toggleLanguage = function () {
        var language = this.params.data;
        this.params.onEnabledToggle(language);
    };
    EnableLanguagesStatusComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-enable-languages-status',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./enable-languages-status.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./enable-languages-status.component.scss */ "./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.scss")).default]
        })
    ], EnableLanguagesStatusComponent);
    return EnableLanguagesStatusComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.scss":
/*!***************************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.scss ***!
  \***************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2FnLWdyaWQtY29tcG9uZW50cy9mZWF0dXJlcy1saXN0LWVuYWJsZWQvZmVhdHVyZXMtbGlzdC1lbmFibGVkLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.ts":
/*!*************************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.ts ***!
  \*************************************************************************************************************/
/*! exports provided: FeaturesListEnabledComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FeaturesListEnabledComponent", function() { return FeaturesListEnabledComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var FeaturesListEnabledComponent = /** @class */ (function () {
    function FeaturesListEnabledComponent() {
    }
    FeaturesListEnabledComponent.prototype.agInit = function (params) {
        this.value = params.value;
    };
    FeaturesListEnabledComponent.prototype.refresh = function (params) {
        return true;
    };
    FeaturesListEnabledComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-features-list-enabled',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./features-list-enabled.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./features-list-enabled.component.scss */ "./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.scss")).default]
        })
    ], FeaturesListEnabledComponent);
    return FeaturesListEnabledComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.scss":
/*!*************************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.scss ***!
  \*************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2FnLWdyaWQtY29tcG9uZW50cy9mZWF0dXJlcy1saXN0LXB1YmxpYy9mZWF0dXJlcy1saXN0LXB1YmxpYy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.ts":
/*!***********************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.ts ***!
  \***********************************************************************************************************/
/*! exports provided: FeaturesListPublicComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FeaturesListPublicComponent", function() { return FeaturesListPublicComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var FeaturesListPublicComponent = /** @class */ (function () {
    function FeaturesListPublicComponent() {
    }
    FeaturesListPublicComponent.prototype.agInit = function (params) {
        this.value = params.value;
    };
    FeaturesListPublicComponent.prototype.refresh = function (params) {
        return true;
    };
    FeaturesListPublicComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-features-list-public',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./features-list-public.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./features-list-public.component.scss */ "./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.scss")).default]
        })
    ], FeaturesListPublicComponent);
    return FeaturesListPublicComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.scss":
/*!*****************************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.scss ***!
  \*****************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".feature-security-icon {\n  color: green;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHBzLW1hbmFnZW1lbnQvYWctZ3JpZC1jb21wb25lbnRzL2ZlYXR1cmVzLWxpc3Qtc2VjdXJpdHkvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXGFwcHMtbWFuYWdlbWVudFxcYWctZ3JpZC1jb21wb25lbnRzXFxmZWF0dXJlcy1saXN0LXNlY3VyaXR5XFxmZWF0dXJlcy1saXN0LXNlY3VyaXR5LmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcHMtbWFuYWdlbWVudC9hZy1ncmlkLWNvbXBvbmVudHMvZmVhdHVyZXMtbGlzdC1zZWN1cml0eS9mZWF0dXJlcy1saXN0LXNlY3VyaXR5LmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsWUFBQTtBQ0NGIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHBzLW1hbmFnZW1lbnQvYWctZ3JpZC1jb21wb25lbnRzL2ZlYXR1cmVzLWxpc3Qtc2VjdXJpdHkvZmVhdHVyZXMtbGlzdC1zZWN1cml0eS5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5mZWF0dXJlLXNlY3VyaXR5LWljb24ge1xyXG4gIGNvbG9yOiBncmVlbjtcclxufVxyXG4iLCIuZmVhdHVyZS1zZWN1cml0eS1pY29uIHtcbiAgY29sb3I6IGdyZWVuO1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.ts":
/*!***************************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.ts ***!
  \***************************************************************************************************************/
/*! exports provided: FeaturesListSecurityComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FeaturesListSecurityComponent", function() { return FeaturesListSecurityComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var FeaturesListSecurityComponent = /** @class */ (function () {
    function FeaturesListSecurityComponent() {
    }
    FeaturesListSecurityComponent.prototype.agInit = function (params) {
    };
    FeaturesListSecurityComponent.prototype.refresh = function (params) {
        return true;
    };
    FeaturesListSecurityComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-features-list-security',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./features-list-security.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./features-list-security.component.scss */ "./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.scss")).default]
        })
    ], FeaturesListSecurityComponent);
    return FeaturesListSecurityComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.scss":
/*!*****************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.scss ***!
  \*****************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2FnLWdyaWQtY29tcG9uZW50cy9mZWF0dXJlcy1saXN0LXVpL2ZlYXR1cmVzLWxpc3QtdWkuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.ts":
/*!***************************************************************************************************!*\
  !*** ./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.ts ***!
  \***************************************************************************************************/
/*! exports provided: FeaturesListUiComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FeaturesListUiComponent", function() { return FeaturesListUiComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var FeaturesListUiComponent = /** @class */ (function () {
    function FeaturesListUiComponent() {
    }
    FeaturesListUiComponent.prototype.agInit = function (params) {
        this.value = params.value;
    };
    FeaturesListUiComponent.prototype.refresh = function (params) {
        return true;
    };
    FeaturesListUiComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-features-list-ui',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./features-list-ui.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./features-list-ui.component.scss */ "./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.scss")).default]
        })
    ], FeaturesListUiComponent);
    return FeaturesListUiComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/apps-list/apps-list.component.scss":
/*!********************************************************************!*\
  !*** ./src/app/apps-management/apps-list/apps-list.component.scss ***!
  \********************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2FwcHMtbGlzdC9hcHBzLWxpc3QuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/apps-management/apps-list/apps-list.component.ts":
/*!******************************************************************!*\
  !*** ./src/app/apps-management/apps-list/apps-list.component.ts ***!
  \******************************************************************/
/*! exports provided: AppsListComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppsListComponent", function() { return AppsListComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _services_apps_list_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../services/apps-list.service */ "./src/app/apps-management/services/apps-list.service.ts");
/* harmony import */ var _ag_grid_components_apps_list_show_apps_list_show_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../ag-grid-components/apps-list-show/apps-list-show.component */ "./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.ts");
/* harmony import */ var _ag_grid_components_apps_list_actions_apps_list_actions_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../ag-grid-components/apps-list-actions/apps-list-actions.component */ "./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.ts");
/* harmony import */ var _constants_app_patterns__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../constants/app.patterns */ "./src/app/apps-management/constants/app.patterns.ts");
/* harmony import */ var _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../../shared/components/boolean-filter/boolean-filter.component */ "./src/app/shared/components/boolean-filter/boolean-filter.component.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");














var AppsListComponent = /** @class */ (function () {
    function AppsListComponent(router, route, appsListService, snackBar) {
        var _this = this;
        this.router = router;
        this.route = route;
        this.appsListService = appsListService;
        this.snackBar = snackBar;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_5__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_13__["defaultGridOptions"]), { frameworkComponents: {
                booleanFilterComponent: _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_11__["BooleanFilterComponent"],
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_12__["IdFieldComponent"],
                appsListShowComponent: _ag_grid_components_apps_list_show_apps_list_show_component__WEBPACK_IMPORTED_MODULE_8__["AppsListShowComponent"],
                appsListActionsComponent: _ag_grid_components_apps_list_actions_apps_list_actions_component__WEBPACK_IMPORTED_MODULE_9__["AppsListActionsComponent"],
            }, columnDefs: [
                {
                    headerName: 'ID', field: 'Id', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
                },
                {
                    headerName: 'Show', field: 'IsHidden', width: 70, headerClass: 'dense', cellClass: 'icons no-outline', sortable: true,
                    filter: 'booleanFilterComponent', cellRenderer: 'appsListShowComponent', valueGetter: this.showValueGetter,
                },
                {
                    headerName: 'Name', field: 'Name', flex: 2, minWidth: 250, cellClass: 'apps-list-primary-action highlight', sortable: true,
                    filter: 'agTextColumnFilter', onCellClicked: this.openApp.bind(this), cellRenderer: function (params) {
                        var app = params.data;
                        if (app.Thumbnail != null) {
                            return "\n            <div class=\"container\">\n              <img class=\"image logo\" src=\"" + app.Thumbnail + "?w=40&h=40&mode=crop\"></img>\n              <div class=\"text\">" + params.value + "</div>\n            </div>";
                        }
                        else {
                            return "\n            <div class=\"container\">\n              <div class=\"image logo\">\n                <span class=\"material-icons-outlined\">star_border</span>\n              </div>\n              <div class=\"text\">" + params.value + "</div>\n            </div>";
                        }
                    },
                },
                {
                    width: 80, cellClass: 'secondary-action no-padding', cellRenderer: 'appsListActionsComponent',
                    cellRendererParams: {
                        onDelete: this.deleteApp.bind(this),
                        onFlush: function (app) { _this.flushApp(app); },
                    },
                },
                {
                    headerName: 'Folder', field: 'Folder', flex: 2, minWidth: 250, cellClass: 'no-outline', sortable: true,
                    filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Version', field: 'Version', width: 78, headerClass: 'dense', cellClass: 'no-outline', sortable: true,
                    filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Items', field: 'Items', width: 70, headerClass: 'dense', cellClass: 'number-cell no-outline', sortable: true,
                    filter: 'agNumberColumnFilter',
                },
            ] });
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_3__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild.firstChild;
    }
    AppsListComponent.prototype.ngOnInit = function () {
        this.fetchAppsList();
        this.refreshOnChildClosed();
    };
    AppsListComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
        this.subscription = null;
    };
    AppsListComponent.prototype.browseCatalog = function () {
        window.open('http://2sxc.org/apps');
    };
    AppsListComponent.prototype.createApp = function () {
        var _this = this;
        var name = prompt('Enter App Name (will also be used for folder)');
        if (name === null) {
            return;
        }
        name = name.trim().replace(/\s\s+/g, ' '); // remove multiple white spaces and tabs
        while (!name.match(_constants_app_patterns__WEBPACK_IMPORTED_MODULE_10__["appNamePattern"])) {
            name = prompt("Enter App Name (will also be used for folder)\n" + _constants_app_patterns__WEBPACK_IMPORTED_MODULE_10__["appNameError"], name);
            if (name === null) {
                return;
            }
            name = name.trim().replace(/\s\s+/g, ' '); // remove multiple white spaces and tabs
        }
        this.snackBar.open('Saving...');
        this.appsListService.create(name).subscribe(function () {
            _this.snackBar.open('Saved', null, { duration: 2000 });
            _this.fetchAppsList();
        });
    };
    AppsListComponent.prototype.importApp = function () {
        this.router.navigate(['import'], { relativeTo: this.route.firstChild });
    };
    AppsListComponent.prototype.fetchAppsList = function () {
        var _this = this;
        this.appsListService.getAll().subscribe(function (apps) {
            _this.apps = apps;
        });
    };
    AppsListComponent.prototype.idValueGetter = function (params) {
        var app = params.data;
        return "ID: " + app.Id + "\nGUID: " + app.Guid;
    };
    AppsListComponent.prototype.showValueGetter = function (params) {
        var app = params.data;
        return !app.IsHidden;
    };
    AppsListComponent.prototype.deleteApp = function (app) {
        var _this = this;
        var result = prompt("This cannot be undone. To really delete this app, type 'yes!' or type/paste the app-name here. Are you sure want to delete '" + app.Name + "' (" + app.Id + ")?");
        if (result === null) {
            return;
        }
        else if (result === app.Name || result === 'yes!') {
            this.snackBar.open('Deleting...');
            this.appsListService.delete(app.Id).subscribe(function () {
                _this.snackBar.open('Deleted', null, { duration: 2000 });
                _this.fetchAppsList();
            });
        }
        else {
            alert('Input did not match - will not delete');
        }
    };
    AppsListComponent.prototype.flushApp = function (app) {
        var _this = this;
        if (!confirm("Flush the App Cache for " + app.Name + " (" + app.Id + ")?")) {
            return;
        }
        this.snackBar.open('Flushing cache...');
        this.appsListService.flushCache(app.Id).subscribe(function () {
            _this.snackBar.open('Cache flushed', null, { duration: 2000 });
        });
    };
    AppsListComponent.prototype.openApp = function (params) {
        var appId = params.data.Id;
        this.router.navigate([appId.toString()], { relativeTo: this.route.parent });
    };
    AppsListComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchAppsList();
            }
        }));
    };
    AppsListComponent.ctorParameters = function () { return [
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_apps_list_service__WEBPACK_IMPORTED_MODULE_7__["AppsListService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_6__["MatSnackBar"] }
    ]; };
    AppsListComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-apps-list',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./apps-list.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/apps-list/apps-list.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./apps-list.component.scss */ "./src/app/apps-management/apps-list/apps-list.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_apps_list_service__WEBPACK_IMPORTED_MODULE_7__["AppsListService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_6__["MatSnackBar"]])
    ], AppsListComponent);
    return AppsListComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/apps-management-nav/apps-management-dialog.config.ts":
/*!**************************************************************************************!*\
  !*** ./src/app/apps-management/apps-management-nav/apps-management-dialog.config.ts ***!
  \**************************************************************************************/
/*! exports provided: appsManagementDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "appsManagementDialog", function() { return appsManagementDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var appsManagementDialog = {
    name: 'APPS_MANAGEMENT_DIALOG',
    initContext: true,
    panelSize: 'large',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var AppsManagementNavComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./apps-management-nav.component */ "./src/app/apps-management/apps-management-nav/apps-management-nav.component.ts"))];
                    case 1:
                        AppsManagementNavComponent = (_a.sent()).AppsManagementNavComponent;
                        return [2 /*return*/, AppsManagementNavComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/apps-management/apps-management-nav/apps-management-nav.component.scss":
/*!****************************************************************************************!*\
  !*** ./src/app/apps-management/apps-management-nav/apps-management-nav.component.scss ***!
  \****************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2FwcHMtbWFuYWdlbWVudC1uYXYvYXBwcy1tYW5hZ2VtZW50LW5hdi5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/apps-management/apps-management-nav/apps-management-nav.component.ts":
/*!**************************************************************************************!*\
  !*** ./src/app/apps-management/apps-management-nav/apps-management-nav.component.ts ***!
  \**************************************************************************************/
/*! exports provided: AppsManagementNavComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppsManagementNavComponent", function() { return AppsManagementNavComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");







var AppsManagementNavComponent = /** @class */ (function () {
    function AppsManagementNavComponent(dialogRef, router, route, context) {
        this.dialogRef = dialogRef;
        this.router = router;
        this.route = route;
        this.context = context;
        this.tabs = ['list', 'languages', 'features', 'sxc-insights']; // tabs order has to match template
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_4__["Subscription"]();
        this.zoneId = this.context.zoneId;
    }
    AppsManagementNavComponent.prototype.ngOnInit = function () {
        var _this = this;
        // set tab initially
        this.tabIndex = this.tabs.indexOf(this.route.snapshot.firstChild.url[0].path);
        this.subscription.add(
        // change tab when route changed
        this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_5__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_3__["NavigationEnd"]; })).subscribe(function (event) {
            _this.tabIndex = _this.tabs.indexOf(_this.route.snapshot.firstChild.url[0].path);
        }));
    };
    AppsManagementNavComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
        this.subscription = null;
    };
    AppsManagementNavComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    AppsManagementNavComponent.prototype.changeTab = function (event) {
        var path = this.tabs[event.index];
        this.router.navigate([path], { relativeTo: this.route });
    };
    AppsManagementNavComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_3__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_6__["Context"] }
    ]; };
    AppsManagementNavComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-apps-management-nav',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./apps-management-nav.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/apps-management-nav/apps-management-nav.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./apps-management-nav.component.scss */ "./src/app/apps-management/apps-management-nav/apps-management-nav.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_3__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"],
            _shared_services_context__WEBPACK_IMPORTED_MODULE_6__["Context"]])
    ], AppsManagementNavComponent);
    return AppsManagementNavComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/apps-management-routing.module.ts":
/*!*******************************************************************!*\
  !*** ./src/app/apps-management/apps-management-routing.module.ts ***!
  \*******************************************************************/
/*! exports provided: AppsManagementRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppsManagementRoutingModule", function() { return AppsManagementRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../shared/components/empty-route/empty-route.component */ "./src/app/shared/components/empty-route/empty-route.component.ts");
/* harmony import */ var _apps_management_nav_apps_management_dialog_config__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./apps-management-nav/apps-management-dialog.config */ "./src/app/apps-management/apps-management-nav/apps-management-dialog.config.ts");






var appsManagementRoutes = [
    {
        path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _apps_management_nav_apps_management_dialog_config__WEBPACK_IMPORTED_MODULE_5__["appsManagementDialog"] }, children: [
            { path: '', redirectTo: 'list', pathMatch: 'full' },
            {
                path: 'list', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], children: [
                    {
                        path: 'import',
                        loadChildren: function () { return Promise.all(/*! import() | import-app-import-app-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("import-app-import-app-module")]).then(__webpack_require__.bind(null, /*! ../import-app/import-app.module */ "./src/app/import-app/import-app.module.ts")).then(function (m) { return m.ImportAppModule; }); }
                    },
                ],
                data: { title: 'Apps in this Zone' },
            },
            { path: 'languages', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], data: { title: 'Zone Languages' } },
            { path: 'features', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], data: { title: 'Zone Features' } },
            { path: 'sxc-insights', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], data: { title: 'Debug Insights' } },
            {
                path: ':appId',
                loadChildren: function () { return Promise.all(/*! import() | app-administration-app-administration-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~33e631e1"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("app-administration-app-administration-module")]).then(__webpack_require__.bind(null, /*! ../app-administration/app-administration.module */ "./src/app/app-administration/app-administration.module.ts")).then(function (m) { return m.AppAdministrationModule; }); }
            },
        ]
    },
];
var AppsManagementRoutingModule = /** @class */ (function () {
    function AppsManagementRoutingModule() {
    }
    AppsManagementRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(appsManagementRoutes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], AppsManagementRoutingModule);
    return AppsManagementRoutingModule;
}());



/***/ }),

/***/ "./src/app/apps-management/apps-management.module.ts":
/*!***********************************************************!*\
  !*** ./src/app/apps-management/apps-management.module.ts ***!
  \***********************************************************/
/*! exports provided: AppsManagementModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppsManagementModule", function() { return AppsManagementModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @ag-grid-community/angular */ "../../node_modules/@ag-grid-community/angular/__ivy_ngcc__/fesm5/ag-grid-community-angular.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/progress-spinner */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/progress-spinner.js");
/* harmony import */ var _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/slide-toggle */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/slide-toggle.js");
/* harmony import */ var _angular_material_tabs__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/tabs */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tabs.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/core */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_card__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/material/card */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/card.js");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! @angular/material/input */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/input.js");
/* harmony import */ var _ecodev_fab_speed_dial__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @ecodev/fab-speed-dial */ "../../node_modules/@ecodev/fab-speed-dial/__ivy_ngcc__/fesm5/ecodev-fab-speed-dial.js");
/* harmony import */ var _apps_management_routing_module__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./apps-management-routing.module */ "./src/app/apps-management/apps-management-routing.module.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _apps_management_nav_apps_management_nav_component__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ./apps-management-nav/apps-management-nav.component */ "./src/app/apps-management/apps-management-nav/apps-management-nav.component.ts");
/* harmony import */ var _apps_list_apps_list_component__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ./apps-list/apps-list.component */ "./src/app/apps-management/apps-list/apps-list.component.ts");
/* harmony import */ var _manage_features_manage_features_component__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./manage-features/manage-features.component */ "./src/app/apps-management/manage-features/manage-features.component.ts");
/* harmony import */ var _sxc_insights_sxc_insights_component__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ./sxc-insights/sxc-insights.component */ "./src/app/apps-management/sxc-insights/sxc-insights.component.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _ag_grid_components_apps_list_show_apps_list_show_component__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ./ag-grid-components/apps-list-show/apps-list-show.component */ "./src/app/apps-management/ag-grid-components/apps-list-show/apps-list-show.component.ts");
/* harmony import */ var _ag_grid_components_apps_list_actions_apps_list_actions_component__WEBPACK_IMPORTED_MODULE_24__ = __webpack_require__(/*! ./ag-grid-components/apps-list-actions/apps-list-actions.component */ "./src/app/apps-management/ag-grid-components/apps-list-actions/apps-list-actions.component.ts");
/* harmony import */ var _services_apps_list_service__WEBPACK_IMPORTED_MODULE_25__ = __webpack_require__(/*! ./services/apps-list.service */ "./src/app/apps-management/services/apps-list.service.ts");
/* harmony import */ var _ag_grid_components_features_list_enabled_features_list_enabled_component__WEBPACK_IMPORTED_MODULE_26__ = __webpack_require__(/*! ./ag-grid-components/features-list-enabled/features-list-enabled.component */ "./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.ts");
/* harmony import */ var _ag_grid_components_features_list_ui_features_list_ui_component__WEBPACK_IMPORTED_MODULE_27__ = __webpack_require__(/*! ./ag-grid-components/features-list-ui/features-list-ui.component */ "./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.ts");
/* harmony import */ var _ag_grid_components_features_list_public_features_list_public_component__WEBPACK_IMPORTED_MODULE_28__ = __webpack_require__(/*! ./ag-grid-components/features-list-public/features-list-public.component */ "./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.ts");
/* harmony import */ var _ag_grid_components_features_list_security_features_list_security_component__WEBPACK_IMPORTED_MODULE_29__ = __webpack_require__(/*! ./ag-grid-components/features-list-security/features-list-security.component */ "./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.ts");
/* harmony import */ var _enable_languages_enable_languages_component__WEBPACK_IMPORTED_MODULE_30__ = __webpack_require__(/*! ./enable-languages/enable-languages.component */ "./src/app/apps-management/enable-languages/enable-languages.component.ts");
/* harmony import */ var _services_enable_languages_service__WEBPACK_IMPORTED_MODULE_31__ = __webpack_require__(/*! ./services/enable-languages.service */ "./src/app/apps-management/services/enable-languages.service.ts");
/* harmony import */ var _ag_grid_components_enable_languages_status_enable_languages_status_component__WEBPACK_IMPORTED_MODULE_32__ = __webpack_require__(/*! ./ag-grid-components/enable-languages-status/enable-languages-status.component */ "./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.ts");
/* harmony import */ var _services_features_config_service__WEBPACK_IMPORTED_MODULE_33__ = __webpack_require__(/*! ./services/features-config.service */ "./src/app/apps-management/services/features-config.service.ts");
/* harmony import */ var _services_sxc_insights_service__WEBPACK_IMPORTED_MODULE_34__ = __webpack_require__(/*! ./services/sxc-insights.service */ "./src/app/apps-management/services/sxc-insights.service.ts");



































var AppsManagementModule = /** @class */ (function () {
    function AppsManagementModule() {
    }
    AppsManagementModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _apps_management_nav_apps_management_nav_component__WEBPACK_IMPORTED_MODULE_18__["AppsManagementNavComponent"],
                _apps_list_apps_list_component__WEBPACK_IMPORTED_MODULE_19__["AppsListComponent"],
                _manage_features_manage_features_component__WEBPACK_IMPORTED_MODULE_20__["ManageFeaturesComponent"],
                _sxc_insights_sxc_insights_component__WEBPACK_IMPORTED_MODULE_21__["SxcInsightsComponent"],
                _ag_grid_components_apps_list_show_apps_list_show_component__WEBPACK_IMPORTED_MODULE_23__["AppsListShowComponent"],
                _ag_grid_components_apps_list_actions_apps_list_actions_component__WEBPACK_IMPORTED_MODULE_24__["AppsListActionsComponent"],
                _ag_grid_components_features_list_enabled_features_list_enabled_component__WEBPACK_IMPORTED_MODULE_26__["FeaturesListEnabledComponent"],
                _ag_grid_components_features_list_ui_features_list_ui_component__WEBPACK_IMPORTED_MODULE_27__["FeaturesListUiComponent"],
                _ag_grid_components_features_list_public_features_list_public_component__WEBPACK_IMPORTED_MODULE_28__["FeaturesListPublicComponent"],
                _ag_grid_components_features_list_security_features_list_security_component__WEBPACK_IMPORTED_MODULE_29__["FeaturesListSecurityComponent"],
                _enable_languages_enable_languages_component__WEBPACK_IMPORTED_MODULE_30__["EnableLanguagesComponent"],
                _ag_grid_components_enable_languages_status_enable_languages_status_component__WEBPACK_IMPORTED_MODULE_32__["EnableLanguagesStatusComponent"],
            ],
            entryComponents: [
                _apps_management_nav_apps_management_nav_component__WEBPACK_IMPORTED_MODULE_18__["AppsManagementNavComponent"],
                _ag_grid_components_apps_list_show_apps_list_show_component__WEBPACK_IMPORTED_MODULE_23__["AppsListShowComponent"],
                _ag_grid_components_apps_list_actions_apps_list_actions_component__WEBPACK_IMPORTED_MODULE_24__["AppsListActionsComponent"],
                _ag_grid_components_features_list_enabled_features_list_enabled_component__WEBPACK_IMPORTED_MODULE_26__["FeaturesListEnabledComponent"],
                _ag_grid_components_features_list_ui_features_list_ui_component__WEBPACK_IMPORTED_MODULE_27__["FeaturesListUiComponent"],
                _ag_grid_components_features_list_public_features_list_public_component__WEBPACK_IMPORTED_MODULE_28__["FeaturesListPublicComponent"],
                _ag_grid_components_features_list_security_features_list_security_component__WEBPACK_IMPORTED_MODULE_29__["FeaturesListSecurityComponent"],
                _ag_grid_components_enable_languages_status_enable_languages_status_component__WEBPACK_IMPORTED_MODULE_32__["EnableLanguagesStatusComponent"],
            ],
            imports: [
                _apps_management_routing_module__WEBPACK_IMPORTED_MODULE_16__["AppsManagementRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_17__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_11__["MatDialogModule"],
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_4__["AgGridModule"].withComponents([]),
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"],
                _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_8__["MatProgressSpinnerModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__["MatTooltipModule"],
                _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_9__["MatSlideToggleModule"],
                _angular_material_tabs__WEBPACK_IMPORTED_MODULE_10__["MatTabsModule"],
                _angular_material_core__WEBPACK_IMPORTED_MODULE_12__["MatRippleModule"],
                _ecodev_fab_speed_dial__WEBPACK_IMPORTED_MODULE_15__["EcoFabSpeedDialModule"],
                _angular_material_card__WEBPACK_IMPORTED_MODULE_13__["MatCardModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_3__["FormsModule"],
                _angular_material_input__WEBPACK_IMPORTED_MODULE_14__["MatInputModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_22__["Context"],
                _services_apps_list_service__WEBPACK_IMPORTED_MODULE_25__["AppsListService"],
                _services_enable_languages_service__WEBPACK_IMPORTED_MODULE_31__["EnableLanguagesService"],
                _services_features_config_service__WEBPACK_IMPORTED_MODULE_33__["FeaturesConfigService"],
                _services_sxc_insights_service__WEBPACK_IMPORTED_MODULE_34__["SxcInsightsService"],
            ]
        })
    ], AppsManagementModule);
    return AppsManagementModule;
}());



/***/ }),

/***/ "./src/app/apps-management/constants/app.patterns.ts":
/*!***********************************************************!*\
  !*** ./src/app/apps-management/constants/app.patterns.ts ***!
  \***********************************************************/
/*! exports provided: appNamePattern, appNameError */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "appNamePattern", function() { return appNamePattern; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "appNameError", function() { return appNameError; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var appNamePattern = /^[A-Za-z](?:[A-Za-z0-9\s\(\)-]+)*$/;
var appNameError = 'Standard letters, numbers, spaces, hyphens and round brackets are allowed. Must start with a letter.';


/***/ }),

/***/ "./src/app/apps-management/enable-languages/enable-languages.component.scss":
/*!**********************************************************************************!*\
  !*** ./src/app/apps-management/enable-languages/enable-languages.component.scss ***!
  \**********************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwcy1tYW5hZ2VtZW50L2VuYWJsZS1sYW5ndWFnZXMvZW5hYmxlLWxhbmd1YWdlcy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/apps-management/enable-languages/enable-languages.component.ts":
/*!********************************************************************************!*\
  !*** ./src/app/apps-management/enable-languages/enable-languages.component.ts ***!
  \********************************************************************************/
/*! exports provided: EnableLanguagesComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EnableLanguagesComponent", function() { return EnableLanguagesComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _services_enable_languages_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../services/enable-languages.service */ "./src/app/apps-management/services/enable-languages.service.ts");
/* harmony import */ var _ag_grid_components_enable_languages_status_enable_languages_status_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../ag-grid-components/enable-languages-status/enable-languages-status.component */ "./src/app/apps-management/ag-grid-components/enable-languages-status/enable-languages-status.component.ts");
/* harmony import */ var _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/components/boolean-filter/boolean-filter.component */ "./src/app/shared/components/boolean-filter/boolean-filter.component.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");








var EnableLanguagesComponent = /** @class */ (function () {
    function EnableLanguagesComponent(languagesService) {
        this.languagesService = languagesService;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_2__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_7__["defaultGridOptions"]), { frameworkComponents: {
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_6__["IdFieldComponent"],
                booleanFilterComponent: _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_5__["BooleanFilterComponent"],
                enableLanguagesStatusComponent: _ag_grid_components_enable_languages_status_enable_languages_status_component__WEBPACK_IMPORTED_MODULE_4__["EnableLanguagesStatusComponent"],
            }, columnDefs: [
                {
                    headerName: 'ID', field: 'Code', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
                },
                {
                    headerName: 'Name', field: 'Culture', flex: 2, minWidth: 250, cellClass: 'primary-action highlight no-outline', sortable: true,
                    filter: 'agTextColumnFilter', onCellClicked: this.handleNameClicked.bind(this),
                },
                {
                    headerName: 'Status', field: 'IsEnabled', width: 72, headerClass: 'dense', cellClass: 'no-padding no-outline',
                    cellRenderer: 'enableLanguagesStatusComponent', sortable: true, filter: 'booleanFilterComponent',
                    cellRendererParams: {
                        onEnabledToggle: this.toggleLanguage.bind(this),
                    },
                },
            ] });
    }
    EnableLanguagesComponent.prototype.ngOnInit = function () {
        this.fetchLanguages();
    };
    EnableLanguagesComponent.prototype.idValueGetter = function (params) {
        var language = params.data;
        return "ID: " + language.Code;
    };
    EnableLanguagesComponent.prototype.handleNameClicked = function (params) {
        var language = params.data;
        this.toggleLanguage(language);
    };
    EnableLanguagesComponent.prototype.toggleLanguage = function (language) {
        var _this = this;
        this.languagesService.save(language.Code, !language.IsEnabled).subscribe(function () {
            _this.fetchLanguages();
        });
    };
    EnableLanguagesComponent.prototype.fetchLanguages = function () {
        var _this = this;
        this.languagesService.getAll().subscribe(function (languages) {
            _this.languages = languages;
        });
    };
    EnableLanguagesComponent.ctorParameters = function () { return [
        { type: _services_enable_languages_service__WEBPACK_IMPORTED_MODULE_3__["EnableLanguagesService"] }
    ]; };
    EnableLanguagesComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-enable-languages',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./enable-languages.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/enable-languages/enable-languages.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./enable-languages.component.scss */ "./src/app/apps-management/enable-languages/enable-languages.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_services_enable_languages_service__WEBPACK_IMPORTED_MODULE_3__["EnableLanguagesService"]])
    ], EnableLanguagesComponent);
    return EnableLanguagesComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/manage-features/manage-features.component.scss":
/*!********************************************************************************!*\
  !*** ./src/app/apps-management/manage-features/manage-features.component.scss ***!
  \********************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".iframe-wrapper {\n  padding-top: 20px;\n}\n\n.spinner {\n  position: absolute;\n  top: 40px;\n  left: 20px;\n}\n\n.iframe {\n  border: none;\n  width: 100%;\n  height: 100%;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHBzLW1hbmFnZW1lbnQvbWFuYWdlLWZlYXR1cmVzL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxhcHBzLW1hbmFnZW1lbnRcXG1hbmFnZS1mZWF0dXJlc1xcbWFuYWdlLWZlYXR1cmVzLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcHMtbWFuYWdlbWVudC9tYW5hZ2UtZmVhdHVyZXMvbWFuYWdlLWZlYXR1cmVzLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsaUJBQUE7QUNDRjs7QURFQTtFQUNFLGtCQUFBO0VBQ0EsU0FBQTtFQUNBLFVBQUE7QUNDRjs7QURFQTtFQUNFLFlBQUE7RUFDQSxXQUFBO0VBQ0EsWUFBQTtBQ0NGIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHBzLW1hbmFnZW1lbnQvbWFuYWdlLWZlYXR1cmVzL21hbmFnZS1mZWF0dXJlcy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5pZnJhbWUtd3JhcHBlciB7XHJcbiAgcGFkZGluZy10b3A6IDIwcHg7XHJcbn1cclxuXHJcbi5zcGlubmVyIHtcclxuICBwb3NpdGlvbjogYWJzb2x1dGU7XHJcbiAgdG9wOiA0MHB4O1xyXG4gIGxlZnQ6IDIwcHg7XHJcbn1cclxuXHJcbi5pZnJhbWUge1xyXG4gIGJvcmRlcjogbm9uZTtcclxuICB3aWR0aDogMTAwJTtcclxuICBoZWlnaHQ6IDEwMCU7XHJcbn1cclxuIiwiLmlmcmFtZS13cmFwcGVyIHtcbiAgcGFkZGluZy10b3A6IDIwcHg7XG59XG5cbi5zcGlubmVyIHtcbiAgcG9zaXRpb246IGFic29sdXRlO1xuICB0b3A6IDQwcHg7XG4gIGxlZnQ6IDIwcHg7XG59XG5cbi5pZnJhbWUge1xuICBib3JkZXI6IG5vbmU7XG4gIHdpZHRoOiAxMDAlO1xuICBoZWlnaHQ6IDEwMCU7XG59Il19 */");

/***/ }),

/***/ "./src/app/apps-management/manage-features/manage-features.component.ts":
/*!******************************************************************************!*\
  !*** ./src/app/apps-management/manage-features/manage-features.component.ts ***!
  \******************************************************************************/
/*! exports provided: ManageFeaturesComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ManageFeaturesComponent", function() { return ManageFeaturesComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/platform-browser.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _ag_grid_components_features_list_enabled_features_list_enabled_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../ag-grid-components/features-list-enabled/features-list-enabled.component */ "./src/app/apps-management/ag-grid-components/features-list-enabled/features-list-enabled.component.ts");
/* harmony import */ var _ag_grid_components_features_list_ui_features_list_ui_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../ag-grid-components/features-list-ui/features-list-ui.component */ "./src/app/apps-management/ag-grid-components/features-list-ui/features-list-ui.component.ts");
/* harmony import */ var _ag_grid_components_features_list_public_features_list_public_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../ag-grid-components/features-list-public/features-list-public.component */ "./src/app/apps-management/ag-grid-components/features-list-public/features-list-public.component.ts");
/* harmony import */ var _ag_grid_components_features_list_security_features_list_security_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../ag-grid-components/features-list-security/features-list-security.component */ "./src/app/apps-management/ag-grid-components/features-list-security/features-list-security.component.ts");
/* harmony import */ var _services_features_config_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../services/features-config.service */ "./src/app/apps-management/services/features-config.service.ts");
/* harmony import */ var _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../../shared/components/boolean-filter/boolean-filter.component */ "./src/app/shared/components/boolean-filter/boolean-filter.component.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");












var ManageFeaturesComponent = /** @class */ (function () {
    function ManageFeaturesComponent(sanitizer, featuresConfigService) {
        this.sanitizer = sanitizer;
        this.featuresConfigService = featuresConfigService;
        this.showManagement = false;
        this.showSpinner = false;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_3__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_11__["defaultGridOptions"]), { frameworkComponents: {
                booleanFilterComponent: _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_9__["BooleanFilterComponent"],
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_10__["IdFieldComponent"],
                featuresListEnabledComponent: _ag_grid_components_features_list_enabled_features_list_enabled_component__WEBPACK_IMPORTED_MODULE_4__["FeaturesListEnabledComponent"],
                featuresListUiComponent: _ag_grid_components_features_list_ui_features_list_ui_component__WEBPACK_IMPORTED_MODULE_5__["FeaturesListUiComponent"],
                featuresListPublicComponent: _ag_grid_components_features_list_public_features_list_public_component__WEBPACK_IMPORTED_MODULE_6__["FeaturesListPublicComponent"],
                featuresListSecurityComponent: _ag_grid_components_features_list_security_features_list_security_component__WEBPACK_IMPORTED_MODULE_7__["FeaturesListSecurityComponent"],
            }, columnDefs: [
                {
                    headerName: 'ID', field: 'id', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
                },
                {
                    headerName: 'Enabled', field: 'enabled', width: 80, headerClass: 'dense', cellClass: 'no-outline',
                    sortable: true, filter: 'booleanFilterComponent', cellRenderer: 'featuresListEnabledComponent',
                },
                {
                    headerName: 'UI', field: 'ui', width: 70, headerClass: 'dense', cellClass: 'no-outline',
                    sortable: true, filter: 'booleanFilterComponent', cellRenderer: 'featuresListUiComponent',
                },
                {
                    headerName: 'Public', field: 'public', width: 70, headerClass: 'dense', cellClass: 'no-outline',
                    sortable: true, filter: 'booleanFilterComponent', cellRenderer: 'featuresListPublicComponent'
                },
                {
                    headerName: 'Name', field: 'id', flex: 2, minWidth: 250, cellClass: 'primary-action highlight', sortable: true,
                    filter: 'agTextColumnFilter', onCellClicked: this.openFeature,
                    cellRenderer: function (params) { return 'details (name lookup still WIP)'; },
                },
                {
                    headerName: 'Expires', field: 'expires', flex: 1, minWidth: 200, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter', valueGetter: this.valueGetterDateTime,
                },
                { headerName: 'Security', width: 70, cellClass: 'no-outline', cellRenderer: 'featuresListSecurityComponent' },
            ] });
    }
    ManageFeaturesComponent.prototype.ngOnInit = function () {
        this.fetchFeatures();
    };
    ManageFeaturesComponent.prototype.ngOnDestroy = function () {
        this.destroyManagementListener();
    };
    ManageFeaturesComponent.prototype.toggleManagement = function () {
        this.showManagement = !this.showManagement;
        this.destroyManagementListener();
        if (this.showManagement) {
            this.openManagement();
        }
    };
    ManageFeaturesComponent.prototype.idValueGetter = function (params) {
        var feature = params.data;
        return "GUID: " + feature.id;
    };
    ManageFeaturesComponent.prototype.openFeature = function (params) {
        window.open("https://2sxc.org/r/f/" + params.value, '_blank');
    };
    ManageFeaturesComponent.prototype.fetchFeatures = function () {
        var _this = this;
        this.featuresConfigService.getAll().subscribe(function (features) {
            _this.features = features;
        });
    };
    ManageFeaturesComponent.prototype.openManagement = function () {
        var _this = this;
        this.showSpinner = true;
        this.managementUrl = this.sanitizer.bypassSecurityTrustResourceUrl(''); // reset url
        this.featuresConfigService.getManageFeaturesUrl().subscribe(function (url) {
            _this.showSpinner = false;
            if (url.indexOf('error: user needs host permissions') > -1) {
                _this.showManagement = false;
                throw new Error('User needs host permissions!');
            }
            _this.managementUrl = _this.sanitizer.bypassSecurityTrustResourceUrl(url);
            var managementCallbackBound = _this.managementCallback.bind(_this);
            // event to receive message from iframe
            window.addEventListener('message', managementCallbackBound);
            _this.managementListener = { element: window, type: 'message', listener: managementCallbackBound };
        });
    };
    /** This should await callbacks from the iframe and if it gets a valid callback containing a json, it should send it to the server */
    ManageFeaturesComponent.prototype.managementCallback = function (event) {
        var _this = this;
        this.destroyManagementListener();
        if (typeof (event.data) === 'undefined') {
            return;
        }
        if (event.origin.endsWith('2sxc.org') === false) {
            return;
        } // something from an unknown domain, let's ignore it
        try {
            var features = event.data;
            var featuresString = JSON.stringify(features);
            this.featuresConfigService.saveFeatures(featuresString).subscribe(function (result) {
                _this.showManagement = false;
                _this.fetchFeatures();
            });
        }
        catch (e) { }
    };
    ManageFeaturesComponent.prototype.destroyManagementListener = function () {
        if (this.managementListener) {
            this.managementListener.element.removeEventListener(this.managementListener.type, this.managementListener.listener);
            this.managementListener = null;
        }
    };
    ManageFeaturesComponent.prototype.valueGetterDateTime = function (params) {
        var rawValue = params.data[params.colDef.field];
        if (!rawValue) {
            return null;
        }
        // remove 'Z' and replace 'T'
        return rawValue.substring(0, 19).replace('T', ' ');
    };
    ManageFeaturesComponent.ctorParameters = function () { return [
        { type: _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["DomSanitizer"] },
        { type: _services_features_config_service__WEBPACK_IMPORTED_MODULE_8__["FeaturesConfigService"] }
    ]; };
    ManageFeaturesComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-manage-features',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./manage-features.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/manage-features/manage-features.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./manage-features.component.scss */ "./src/app/apps-management/manage-features/manage-features.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["DomSanitizer"], _services_features_config_service__WEBPACK_IMPORTED_MODULE_8__["FeaturesConfigService"]])
    ], ManageFeaturesComponent);
    return ManageFeaturesComponent;
}());



/***/ }),

/***/ "./src/app/apps-management/services/enable-languages.service.ts":
/*!**********************************************************************!*\
  !*** ./src/app/apps-management/services/enable-languages.service.ts ***!
  \**********************************************************************/
/*! exports provided: EnableLanguagesService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EnableLanguagesService", function() { return EnableLanguagesService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");




var EnableLanguagesService = /** @class */ (function () {
    function EnableLanguagesService(http, dnnContext) {
        this.http = http;
        this.dnnContext = dnnContext;
    }
    EnableLanguagesService.prototype.getAll = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/getlanguages'));
    };
    EnableLanguagesService.prototype.toggle = function (code, enable) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/switchlanguage'), {
            params: { cultureCode: code, enable: enable.toString() },
        });
    };
    EnableLanguagesService.prototype.save = function (code, enable) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/switchlanguage'), {
            params: { cultureCode: code, enable: enable.toString() },
        });
    };
    EnableLanguagesService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    EnableLanguagesService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], EnableLanguagesService);
    return EnableLanguagesService;
}());



/***/ }),

/***/ "./src/app/apps-management/services/features-config.service.ts":
/*!*********************************************************************!*\
  !*** ./src/app/apps-management/services/features-config.service.ts ***!
  \*********************************************************************/
/*! exports provided: FeaturesConfigService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FeaturesConfigService", function() { return FeaturesConfigService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");




var FeaturesConfigService = /** @class */ (function () {
    function FeaturesConfigService(http, dnnContext) {
        this.http = http;
        this.dnnContext = dnnContext;
    }
    FeaturesConfigService.prototype.getAll = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/features'));
    };
    FeaturesConfigService.prototype.getManageFeaturesUrl = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/managefeaturesurl'));
    };
    FeaturesConfigService.prototype.saveFeatures = function (featuresString) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/SaveFeatures'), featuresString);
    };
    FeaturesConfigService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    FeaturesConfigService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], FeaturesConfigService);
    return FeaturesConfigService;
}());



/***/ }),

/***/ "./src/app/apps-management/services/sxc-insights.service.ts":
/*!******************************************************************!*\
  !*** ./src/app/apps-management/services/sxc-insights.service.ts ***!
  \******************************************************************/
/*! exports provided: SxcInsightsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SxcInsightsService", function() { return SxcInsightsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");




var SxcInsightsService = /** @class */ (function () {
    function SxcInsightsService(http, dnnContext) {
        this.http = http;
        this.dnnContext = dnnContext;
    }
    SxcInsightsService.prototype.activatePageLog = function (duration) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/extendedlogging'), {
            params: { duration: duration.toString() }
        });
    };
    SxcInsightsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    SxcInsightsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], SxcInsightsService);
    return SxcInsightsService;
}());



/***/ }),

/***/ "./src/app/apps-management/sxc-insights/sxc-insights.component.scss":
/*!**************************************************************************!*\
  !*** ./src/app/apps-management/sxc-insights/sxc-insights.component.scss ***!
  \**************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".activate-log-form .mat-form-field {\n  height: auto;\n}\n.activate-log-form .actions {\n  margin-top: 8px;\n  display: flex;\n  justify-content: flex-end;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHBzLW1hbmFnZW1lbnQvc3hjLWluc2lnaHRzL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxhcHBzLW1hbmFnZW1lbnRcXHN4Yy1pbnNpZ2h0c1xcc3hjLWluc2lnaHRzLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcHMtbWFuYWdlbWVudC9zeGMtaW5zaWdodHMvc3hjLWluc2lnaHRzLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUNFO0VBQ0UsWUFBQTtBQ0FKO0FER0U7RUFDRSxlQUFBO0VBQ0EsYUFBQTtFQUNBLHlCQUFBO0FDREoiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcHMtbWFuYWdlbWVudC9zeGMtaW5zaWdodHMvc3hjLWluc2lnaHRzLmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLmFjdGl2YXRlLWxvZy1mb3JtIHtcclxuICAubWF0LWZvcm0tZmllbGQge1xyXG4gICAgaGVpZ2h0OiBhdXRvO1xyXG4gIH1cclxuXHJcbiAgLmFjdGlvbnMge1xyXG4gICAgbWFyZ2luLXRvcDogOHB4O1xyXG4gICAgZGlzcGxheTogZmxleDtcclxuICAgIGp1c3RpZnktY29udGVudDogZmxleC1lbmQ7XHJcbiAgfVxyXG59XHJcbiIsIi5hY3RpdmF0ZS1sb2ctZm9ybSAubWF0LWZvcm0tZmllbGQge1xuICBoZWlnaHQ6IGF1dG87XG59XG4uYWN0aXZhdGUtbG9nLWZvcm0gLmFjdGlvbnMge1xuICBtYXJnaW4tdG9wOiA4cHg7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGp1c3RpZnktY29udGVudDogZmxleC1lbmQ7XG59Il19 */");

/***/ }),

/***/ "./src/app/apps-management/sxc-insights/sxc-insights.component.ts":
/*!************************************************************************!*\
  !*** ./src/app/apps-management/sxc-insights/sxc-insights.component.ts ***!
  \************************************************************************/
/*! exports provided: SxcInsightsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SxcInsightsComponent", function() { return SxcInsightsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _services_sxc_insights_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../services/sxc-insights.service */ "./src/app/apps-management/services/sxc-insights.service.ts");




var SxcInsightsComponent = /** @class */ (function () {
    function SxcInsightsComponent(sxcInsightsService, snackBar) {
        this.sxcInsightsService = sxcInsightsService;
        this.snackBar = snackBar;
        this.positiveWholeNumber = /^[^-]\d*$/;
        this.actionsDiabled = false;
    }
    SxcInsightsComponent.prototype.ngOnInit = function () {
    };
    SxcInsightsComponent.prototype.openInsights = function () {
        window.open('/desktopmodules/2sxc/api/sys/insights/help');
    };
    SxcInsightsComponent.prototype.activatePageLog = function () {
        var _this = this;
        this.actionsDiabled = true;
        this.snackBar.open('Activating...');
        this.sxcInsightsService.activatePageLog(this.pageLogDuration).subscribe(function (res) {
            _this.pageLogDuration = undefined;
            _this.actionsDiabled = false;
            _this.snackBar.open(res, null, { duration: 4000 });
        });
    };
    SxcInsightsComponent.ctorParameters = function () { return [
        { type: _services_sxc_insights_service__WEBPACK_IMPORTED_MODULE_3__["SxcInsightsService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"] }
    ]; };
    SxcInsightsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-sxc-insights',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./sxc-insights.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/apps-management/sxc-insights/sxc-insights.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./sxc-insights.component.scss */ "./src/app/apps-management/sxc-insights/sxc-insights.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_services_sxc_insights_service__WEBPACK_IMPORTED_MODULE_3__["SxcInsightsService"], _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"]])
    ], SxcInsightsComponent);
    return SxcInsightsComponent;
}());



/***/ })

}]);
//# sourceMappingURL=apps-management-apps-management-module.js.map