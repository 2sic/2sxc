(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["manage-content-list-manage-content-list-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/manage-content-list/manage-content-list.component.html":
/*!************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/manage-content-list/manage-content-list.component.html ***!
  \************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Manage content-item lists</div>\r\n</div>\r\n\r\n<router-outlet></router-outlet>\r\n\r\n<div class=\"dialog-component-content fancy-scrollbar-light\">\r\n  <ng-container *ngIf=\"header\">\r\n    <p class=\"dialog-description\">\r\n      You can manage the list header here (if it is defined):\r\n    </p>\r\n\r\n    <p class=\"dialog-description\">\r\n      <ng-container *ngIf=\"header.Type\">\r\n        <span>{{ header.Title }}</span>\r\n        <button mat-icon-button matTooltip=\"Edit header\" (click)=\"editHeader()\">\r\n          <mat-icon>edit</mat-icon>\r\n        </button>\r\n      </ng-container>\r\n      <ng-container *ngIf=\"!header.Type\">(this list has no header)</ng-container>\r\n    </p>\r\n  </ng-container>\r\n\r\n  <p class=\"dialog-description sort-title\">Sort the items by dragging as you need, then save:</p>\r\n\r\n  <div class=\"dnd-list\" cdkDropList (cdkDropListDropped)=\"drop($event)\">\r\n    <div *ngFor=\"let item of items\" class=\"dnd-item\" cdkDrag>\r\n      <div class=\"dnd-item__title\">\r\n        <mat-icon class=\"dnd-item__title-icon\" matTooltip=\"Drag to reorder the list\">drag_handle</mat-icon>\r\n        <span class=\"dnd-item__title-text\">{{ item.Title }} ({{ item.Id }})</span>\r\n      </div>\r\n      <div *ngIf=\"item.Id !== 0\">\r\n        <button mat-icon-button matTooltip=\"Edit item\" appMousedownStopPropagation (click)=\"editItem(item.Id)\">\r\n          <mat-icon>edit</mat-icon>\r\n        </button>\r\n      </div>\r\n    </div>\r\n  </div>\r\n</div>\r\n\r\n<div class=\"dialog-component-actions\">\r\n  <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n  <button mat-raised-button color=\"accent\" (click)=\"saveList()\">Save</button>\r\n</div>\r\n");

/***/ }),

/***/ "./src/app/manage-content-list/manage-content-list-dialog.config.ts":
/*!**************************************************************************!*\
  !*** ./src/app/manage-content-list/manage-content-list-dialog.config.ts ***!
  \**************************************************************************/
/*! exports provided: manageContentListDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "manageContentListDialog", function() { return manageContentListDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var manageContentListDialog = {
    name: 'MANAGE_CONTENT_LIST_DIALOG',
    initContext: true,
    panelSize: 'small',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ManageContentListComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./manage-content-list.component */ "./src/app/manage-content-list/manage-content-list.component.ts"))];
                    case 1:
                        ManageContentListComponent = (_a.sent()).ManageContentListComponent;
                        return [2 /*return*/, ManageContentListComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/manage-content-list/manage-content-list-routing.module.ts":
/*!***************************************************************************!*\
  !*** ./src/app/manage-content-list/manage-content-list-routing.module.ts ***!
  \***************************************************************************/
/*! exports provided: ManageContentListRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ManageContentListRoutingModule", function() { return ManageContentListRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _manage_content_list_dialog_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./manage-content-list-dialog.config */ "./src/app/manage-content-list/manage-content-list-dialog.config.ts");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../edit/edit.matcher */ "../edit/edit.matcher.ts");






var routes = [
    {
        path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _manage_content_list_dialog_config__WEBPACK_IMPORTED_MODULE_4__["manageContentListDialog"] }, children: [
            {
                matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); }
            },
        ]
    },
];
var ManageContentListRoutingModule = /** @class */ (function () {
    function ManageContentListRoutingModule() {
    }
    ManageContentListRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], ManageContentListRoutingModule);
    return ManageContentListRoutingModule;
}());



/***/ }),

/***/ "./src/app/manage-content-list/manage-content-list.component.scss":
/*!************************************************************************!*\
  !*** ./src/app/manage-content-list/manage-content-list.component.scss ***!
  \************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".sort-title {\n  padding-top: 16px;\n}\n\n.dnd-list {\n  margin: 16px 0;\n  border: 1px solid #ccc;\n  border-radius: 4px;\n  overflow: hidden;\n}\n\n.dnd-item {\n  cursor: move;\n  height: 40px;\n  border-bottom: 1px solid #ccc;\n  padding: 0 8px;\n  display: flex;\n  align-items: center;\n  justify-content: space-between;\n  background: white;\n  box-sizing: border-box;\n  font-size: 14px;\n}\n\n.dnd-item:last-child {\n  border: none;\n}\n\n.dnd-item__title {\n  display: flex;\n  align-items: center;\n  overflow: hidden;\n}\n\n.dnd-item__title-icon {\n  margin-right: 8px;\n  opacity: 0.4;\n}\n\n.dnd-item__title-text {\n  overflow: hidden;\n  text-overflow: ellipsis;\n  white-space: nowrap;\n}\n\n.cdk-drag-preview {\n  box-sizing: border-box;\n  border-radius: 4px;\n  box-shadow: 0 5px 5px -3px rgba(0, 0, 0, 0.2), 0 8px 10px 1px rgba(0, 0, 0, 0.14), 0 3px 14px 2px rgba(0, 0, 0, 0.12);\n}\n\n.cdk-drag-placeholder {\n  opacity: 0;\n}\n\n.cdk-drag-animating {\n  transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);\n}\n\n.dnd-list.cdk-drop-list-dragging .dnd-item:not(.cdk-drag-placeholder) {\n  transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9tYW5hZ2UtY29udGVudC1saXN0L0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxtYW5hZ2UtY29udGVudC1saXN0XFxtYW5hZ2UtY29udGVudC1saXN0LmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL21hbmFnZS1jb250ZW50LWxpc3QvbWFuYWdlLWNvbnRlbnQtbGlzdC5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNFLGlCQUFBO0FDQ0Y7O0FERUE7RUFDRSxjQUFBO0VBQ0Esc0JBQUE7RUFDQSxrQkFBQTtFQUNBLGdCQUFBO0FDQ0Y7O0FERUE7RUFDRSxZQUFBO0VBQ0EsWUFBQTtFQUNBLDZCQUFBO0VBQ0EsY0FBQTtFQUNBLGFBQUE7RUFDQSxtQkFBQTtFQUNBLDhCQUFBO0VBQ0EsaUJBQUE7RUFDQSxzQkFBQTtFQUNBLGVBQUE7QUNDRjs7QURDRTtFQUNFLFlBQUE7QUNDSjs7QURFRTtFQUNFLGFBQUE7RUFDQSxtQkFBQTtFQUNBLGdCQUFBO0FDQUo7O0FER0U7RUFDRSxpQkFBQTtFQUNBLFlBQUE7QUNESjs7QURJRTtFQUNFLGdCQUFBO0VBQ0EsdUJBQUE7RUFDQSxtQkFBQTtBQ0ZKOztBRE1BO0VBQ0Usc0JBQUE7RUFDQSxrQkFBQTtFQUNBLHFIQUFBO0FDSEY7O0FEUUE7RUFDRSxVQUFBO0FDTEY7O0FEUUE7RUFDRSxzREFBQTtBQ0xGOztBRFFBO0VBQ0Usc0RBQUE7QUNMRiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvbWFuYWdlLWNvbnRlbnQtbGlzdC9tYW5hZ2UtY29udGVudC1saXN0LmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLnNvcnQtdGl0bGUge1xyXG4gIHBhZGRpbmctdG9wOiAxNnB4O1xyXG59XHJcblxyXG4uZG5kLWxpc3Qge1xyXG4gIG1hcmdpbjogMTZweCAwO1xyXG4gIGJvcmRlcjogMXB4IHNvbGlkICNjY2M7XHJcbiAgYm9yZGVyLXJhZGl1czogNHB4O1xyXG4gIG92ZXJmbG93OiBoaWRkZW47XHJcbn1cclxuXHJcbi5kbmQtaXRlbSB7XHJcbiAgY3Vyc29yOiBtb3ZlO1xyXG4gIGhlaWdodDogNDBweDtcclxuICBib3JkZXItYm90dG9tOiAxcHggc29saWQgI2NjYztcclxuICBwYWRkaW5nOiAwIDhweDtcclxuICBkaXNwbGF5OiBmbGV4O1xyXG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XHJcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xyXG4gIGJhY2tncm91bmQ6IHdoaXRlO1xyXG4gIGJveC1zaXppbmc6IGJvcmRlci1ib3g7XHJcbiAgZm9udC1zaXplOiAxNHB4O1xyXG5cclxuICAmOmxhc3QtY2hpbGQge1xyXG4gICAgYm9yZGVyOiBub25lO1xyXG4gIH1cclxuXHJcbiAgJl9fdGl0bGUge1xyXG4gICAgZGlzcGxheTogZmxleDtcclxuICAgIGFsaWduLWl0ZW1zOiBjZW50ZXI7XHJcbiAgICBvdmVyZmxvdzogaGlkZGVuO1xyXG4gIH1cclxuXHJcbiAgJl9fdGl0bGUtaWNvbiB7XHJcbiAgICBtYXJnaW4tcmlnaHQ6IDhweDtcclxuICAgIG9wYWNpdHk6IDAuNDtcclxuICB9XHJcblxyXG4gICZfX3RpdGxlLXRleHQge1xyXG4gICAgb3ZlcmZsb3c6IGhpZGRlbjtcclxuICAgIHRleHQtb3ZlcmZsb3c6IGVsbGlwc2lzO1xyXG4gICAgd2hpdGUtc3BhY2U6IG5vd3JhcDtcclxuICB9XHJcbn1cclxuXHJcbi5jZGstZHJhZy1wcmV2aWV3IHtcclxuICBib3gtc2l6aW5nOiBib3JkZXItYm94O1xyXG4gIGJvcmRlci1yYWRpdXM6IDRweDtcclxuICBib3gtc2hhZG93OiAwIDVweCA1cHggLTNweCByZ2JhKDAsIDAsIDAsIDAuMiksXHJcbiAgICAwIDhweCAxMHB4IDFweCByZ2JhKDAsIDAsIDAsIDAuMTQpLFxyXG4gICAgMCAzcHggMTRweCAycHggcmdiYSgwLCAwLCAwLCAwLjEyKTtcclxufVxyXG5cclxuLmNkay1kcmFnLXBsYWNlaG9sZGVyIHtcclxuICBvcGFjaXR5OiAwO1xyXG59XHJcblxyXG4uY2RrLWRyYWctYW5pbWF0aW5nIHtcclxuICB0cmFuc2l0aW9uOiB0cmFuc2Zvcm0gMjUwbXMgY3ViaWMtYmV6aWVyKDAsIDAsIDAuMiwgMSk7XHJcbn1cclxuXHJcbi5kbmQtbGlzdC5jZGstZHJvcC1saXN0LWRyYWdnaW5nIC5kbmQtaXRlbTpub3QoLmNkay1kcmFnLXBsYWNlaG9sZGVyKSB7XHJcbiAgdHJhbnNpdGlvbjogdHJhbnNmb3JtIDI1MG1zIGN1YmljLWJlemllcigwLCAwLCAwLjIsIDEpO1xyXG59XHJcbiIsIi5zb3J0LXRpdGxlIHtcbiAgcGFkZGluZy10b3A6IDE2cHg7XG59XG5cbi5kbmQtbGlzdCB7XG4gIG1hcmdpbjogMTZweCAwO1xuICBib3JkZXI6IDFweCBzb2xpZCAjY2NjO1xuICBib3JkZXItcmFkaXVzOiA0cHg7XG4gIG92ZXJmbG93OiBoaWRkZW47XG59XG5cbi5kbmQtaXRlbSB7XG4gIGN1cnNvcjogbW92ZTtcbiAgaGVpZ2h0OiA0MHB4O1xuICBib3JkZXItYm90dG9tOiAxcHggc29saWQgI2NjYztcbiAgcGFkZGluZzogMCA4cHg7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XG4gIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2VlbjtcbiAgYmFja2dyb3VuZDogd2hpdGU7XG4gIGJveC1zaXppbmc6IGJvcmRlci1ib3g7XG4gIGZvbnQtc2l6ZTogMTRweDtcbn1cbi5kbmQtaXRlbTpsYXN0LWNoaWxkIHtcbiAgYm9yZGVyOiBub25lO1xufVxuLmRuZC1pdGVtX190aXRsZSB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XG4gIG92ZXJmbG93OiBoaWRkZW47XG59XG4uZG5kLWl0ZW1fX3RpdGxlLWljb24ge1xuICBtYXJnaW4tcmlnaHQ6IDhweDtcbiAgb3BhY2l0eTogMC40O1xufVxuLmRuZC1pdGVtX190aXRsZS10ZXh0IHtcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcbiAgdGV4dC1vdmVyZmxvdzogZWxsaXBzaXM7XG4gIHdoaXRlLXNwYWNlOiBub3dyYXA7XG59XG5cbi5jZGstZHJhZy1wcmV2aWV3IHtcbiAgYm94LXNpemluZzogYm9yZGVyLWJveDtcbiAgYm9yZGVyLXJhZGl1czogNHB4O1xuICBib3gtc2hhZG93OiAwIDVweCA1cHggLTNweCByZ2JhKDAsIDAsIDAsIDAuMiksIDAgOHB4IDEwcHggMXB4IHJnYmEoMCwgMCwgMCwgMC4xNCksIDAgM3B4IDE0cHggMnB4IHJnYmEoMCwgMCwgMCwgMC4xMik7XG59XG5cbi5jZGstZHJhZy1wbGFjZWhvbGRlciB7XG4gIG9wYWNpdHk6IDA7XG59XG5cbi5jZGstZHJhZy1hbmltYXRpbmcge1xuICB0cmFuc2l0aW9uOiB0cmFuc2Zvcm0gMjUwbXMgY3ViaWMtYmV6aWVyKDAsIDAsIDAuMiwgMSk7XG59XG5cbi5kbmQtbGlzdC5jZGstZHJvcC1saXN0LWRyYWdnaW5nIC5kbmQtaXRlbTpub3QoLmNkay1kcmFnLXBsYWNlaG9sZGVyKSB7XG4gIHRyYW5zaXRpb246IHRyYW5zZm9ybSAyNTBtcyBjdWJpYy1iZXppZXIoMCwgMCwgMC4yLCAxKTtcbn0iXX0= */");

/***/ }),

/***/ "./src/app/manage-content-list/manage-content-list.component.ts":
/*!**********************************************************************!*\
  !*** ./src/app/manage-content-list/manage-content-list.component.ts ***!
  \**********************************************************************/
/*! exports provided: ManageContentListComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ManageContentListComponent", function() { return ManageContentListComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_cdk_drag_drop__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/cdk/drag-drop */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/drag-drop.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _services_content_group_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./services/content-group.service */ "./src/app/manage-content-list/services/content-group.service.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");










var ManageContentListComponent = /** @class */ (function () {
    function ManageContentListComponent(dialogRef, contentGroupService, route, router, snackBar) {
        this.dialogRef = dialogRef;
        this.contentGroupService = contentGroupService;
        this.route = route;
        this.router = router;
        this.snackBar = snackBar;
        this.hostClass = 'dialog-component';
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_6__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild;
        this.contentGroup = {
            id: null,
            guid: this.route.snapshot.paramMap.get('guid'),
            part: this.route.snapshot.paramMap.get('part'),
            index: parseInt(this.route.snapshot.paramMap.get('index'), 10),
        };
    }
    ManageContentListComponent.prototype.ngOnInit = function () {
        this.fetchList();
        this.fetchHeader();
        this.refreshOnChildClosed();
    };
    ManageContentListComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ManageContentListComponent.prototype.saveList = function () {
        var _this = this;
        this.snackBar.open('Saving...');
        this.contentGroupService.saveList(this.contentGroup, this.items).subscribe(function (res) {
            _this.snackBar.open('Saved');
            _this.closeDialog();
        });
    };
    ManageContentListComponent.prototype.editHeader = function () {
        var form = {
            items: [
                {
                    Group: {
                        Guid: this.contentGroup.guid,
                        Index: 0,
                        Part: 'listcontent',
                        Add: this.header.Id === 0,
                    },
                },
                {
                    Group: {
                        Guid: this.contentGroup.guid,
                        Index: 0,
                        Part: 'listpresentation',
                        Add: this.header.Id === 0,
                    },
                },
            ],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_9__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    ManageContentListComponent.prototype.editItem = function (id) {
        var form = {
            items: [{ EntityId: id }],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_9__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    ManageContentListComponent.prototype.drop = function (event) {
        Object(_angular_cdk_drag_drop__WEBPACK_IMPORTED_MODULE_3__["moveItemInArray"])(this.items, event.previousIndex, event.currentIndex);
    };
    ManageContentListComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ManageContentListComponent.prototype.fetchList = function () {
        var _this = this;
        this.contentGroupService.getList(this.contentGroup).subscribe(function (res) {
            _this.items = res;
        });
    };
    ManageContentListComponent.prototype.fetchHeader = function () {
        var _this = this;
        this.contentGroupService.getHeader(this.contentGroup).subscribe(function (res) {
            _this.header = res;
        });
    };
    ManageContentListComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchList();
                _this.fetchHeader();
            }
        }));
    };
    ManageContentListComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogRef"] },
        { type: _services_content_group_service__WEBPACK_IMPORTED_MODULE_8__["ContentGroupService"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBar"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ManageContentListComponent.prototype, "hostClass", void 0);
    ManageContentListComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-manage-content-list',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./manage-content-list.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/manage-content-list/manage-content-list.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./manage-content-list.component.scss */ "./src/app/manage-content-list/manage-content-list.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogRef"],
            _services_content_group_service__WEBPACK_IMPORTED_MODULE_8__["ContentGroupService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBar"]])
    ], ManageContentListComponent);
    return ManageContentListComponent;
}());



/***/ }),

/***/ "./src/app/manage-content-list/manage-content-list.module.ts":
/*!*******************************************************************!*\
  !*** ./src/app/manage-content-list/manage-content-list.module.ts ***!
  \*******************************************************************/
/*! exports provided: ManageContentListModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ManageContentListModule", function() { return ManageContentListModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_cdk_drag_drop__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/cdk/drag-drop */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/drag-drop.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _manage_content_list_routing_module__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./manage-content-list-routing.module */ "./src/app/manage-content-list/manage-content-list-routing.module.ts");
/* harmony import */ var _manage_content_list_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./manage-content-list.component */ "./src/app/manage-content-list/manage-content-list.component.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _services_content_group_service__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./services/content-group.service */ "./src/app/manage-content-list/services/content-group.service.ts");














var ManageContentListModule = /** @class */ (function () {
    function ManageContentListModule() {
    }
    ManageContentListModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _manage_content_list_component__WEBPACK_IMPORTED_MODULE_10__["ManageContentListComponent"],
            ],
            entryComponents: [
                _manage_content_list_component__WEBPACK_IMPORTED_MODULE_10__["ManageContentListComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _manage_content_list_routing_module__WEBPACK_IMPORTED_MODULE_9__["ManageContentListRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_11__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__["MatTooltipModule"],
                _angular_cdk_drag_drop__WEBPACK_IMPORTED_MODULE_3__["DragDropModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_8__["MatSnackBarModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_12__["Context"],
                _services_content_group_service__WEBPACK_IMPORTED_MODULE_13__["ContentGroupService"],
            ]
        })
    ], ManageContentListModule);
    return ManageContentListModule;
}());



/***/ })

}]);
//# sourceMappingURL=manage-content-list-manage-content-list-module.js.map