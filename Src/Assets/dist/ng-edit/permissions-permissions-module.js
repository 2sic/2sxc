(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["permissions-permissions-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.html":
/*!*******************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.html ***!
  \*******************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Delete\" (click)=\"deletePermission()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/permissions/permissions.component.html":
/*!********************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/permissions/permissions.component.html ***!
  \********************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"nav-component-wrapper\">\r\n  <div mat-dialog-title>\r\n    <div class=\"dialog-title-box\">\r\n      <div>Permissions</div>\r\n      <button mat-icon-button matTooltip=\"Close dialog\" (click)=\"closeDialog()\">\r\n        <mat-icon>close</mat-icon>\r\n      </button>\r\n    </div>\r\n  </div>\r\n\r\n  <router-outlet></router-outlet>\r\n\r\n  <div class=\"grid-wrapper\">\r\n    <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"permissions\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n    </ag-grid-angular>\r\n\r\n    <button mat-fab mat-elevation-z24 class=\"grid-fab\" matTooltip=\"Create a new permission\"\r\n      (click)=\"editPermission(null)\">\r\n      <mat-icon>add</mat-icon>\r\n    </button>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.scss":
/*!*******************************************************************************************************!*\
  !*** ./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.scss ***!
  \*******************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvcGVybWlzc2lvbnMvYWctZ3JpZC1jb21wb25lbnRzL3Blcm1pc3Npb25zLWFjdGlvbnMvcGVybWlzc2lvbnMtYWN0aW9ucy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.ts ***!
  \*****************************************************************************************************/
/*! exports provided: PermissionsActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PermissionsActionsComponent", function() { return PermissionsActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var PermissionsActionsComponent = /** @class */ (function () {
    function PermissionsActionsComponent() {
    }
    PermissionsActionsComponent.prototype.agInit = function (params) {
        this.params = params;
    };
    PermissionsActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    PermissionsActionsComponent.prototype.deletePermission = function () {
        var permission = this.params.data;
        this.params.onDelete(permission);
    };
    PermissionsActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-permissions-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./permissions-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./permissions-actions.component.scss */ "./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.scss")).default]
        })
    ], PermissionsActionsComponent);
    return PermissionsActionsComponent;
}());



/***/ }),

/***/ "./src/app/permissions/permissions-dialog.config.ts":
/*!**********************************************************!*\
  !*** ./src/app/permissions/permissions-dialog.config.ts ***!
  \**********************************************************/
/*! exports provided: permissionsDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "permissionsDialog", function() { return permissionsDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var permissionsDialog = {
    name: 'SET_PERMISSIONS_DIALOG',
    initContext: true,
    panelSize: 'large',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var PermissionsComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./permissions.component */ "./src/app/permissions/permissions.component.ts"))];
                    case 1:
                        PermissionsComponent = (_a.sent()).PermissionsComponent;
                        return [2 /*return*/, PermissionsComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/permissions/permissions-routing.module.ts":
/*!***********************************************************!*\
  !*** ./src/app/permissions/permissions-routing.module.ts ***!
  \***********************************************************/
/*! exports provided: PermissionsRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PermissionsRoutingModule", function() { return PermissionsRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _permissions_dialog_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./permissions-dialog.config */ "./src/app/permissions/permissions-dialog.config.ts");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../edit/edit.matcher */ "../edit/edit.matcher.ts");






var routes = [
    {
        path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _permissions_dialog_config__WEBPACK_IMPORTED_MODULE_4__["permissionsDialog"] }, children: [
            {
                matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); }
            },
        ]
    },
];
var PermissionsRoutingModule = /** @class */ (function () {
    function PermissionsRoutingModule() {
    }
    PermissionsRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], PermissionsRoutingModule);
    return PermissionsRoutingModule;
}());



/***/ }),

/***/ "./src/app/permissions/permissions.component.scss":
/*!********************************************************!*\
  !*** ./src/app/permissions/permissions.component.scss ***!
  \********************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvcGVybWlzc2lvbnMvcGVybWlzc2lvbnMuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/permissions/permissions.component.ts":
/*!******************************************************!*\
  !*** ./src/app/permissions/permissions.component.ts ***!
  \******************************************************/
/*! exports provided: PermissionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PermissionsComponent", function() { return PermissionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _services_permissions_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./services/permissions.service */ "./src/app/permissions/services/permissions.service.ts");
/* harmony import */ var _ag_grid_components_permissions_actions_permissions_actions_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./ag-grid-components/permissions-actions/permissions-actions.component */ "./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");














var PermissionsComponent = /** @class */ (function () {
    function PermissionsComponent(dialogRef, router, route, permissionsService, snackBar) {
        this.dialogRef = dialogRef;
        this.router = router;
        this.route = route;
        this.permissionsService = permissionsService;
        this.snackBar = snackBar;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_7__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_12__["defaultGridOptions"]), { frameworkComponents: {
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_11__["IdFieldComponent"],
                permissionsActionsComponent: _ag_grid_components_permissions_actions_permissions_actions_component__WEBPACK_IMPORTED_MODULE_9__["PermissionsActionsComponent"],
            }, columnDefs: [
                {
                    headerName: 'ID', field: 'Id', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
                },
                {
                    headerName: 'Name', field: 'Title', flex: 2, minWidth: 250, cellClass: 'primary-action highlight',
                    sortable: true, filter: 'agTextColumnFilter', onCellClicked: this.editPermission.bind(this),
                },
                {
                    width: 40, cellClass: 'secondary-action no-padding', cellRenderer: 'permissionsActionsComponent',
                    cellRendererParams: {
                        onDelete: this.deletePermission.bind(this),
                    },
                },
                {
                    headerName: 'Identity', field: 'Identity', flex: 2, minWidth: 250, cellClass: 'no-outline', sortable: true,
                    filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Condition', field: 'Condition', flex: 2, minWidth: 250, cellClass: 'no-outline', sortable: true,
                    filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Grant', field: 'Grant', width: 70, headerClass: 'dense', cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
            ] });
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_5__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild;
    }
    PermissionsComponent.prototype.ngOnInit = function () {
        this.targetType = parseInt(this.route.snapshot.paramMap.get('type'), 10);
        this.keyType = this.route.snapshot.paramMap.get('keyType');
        this.key = this.route.snapshot.paramMap.get('key');
        this.fetchPermissions();
        this.refreshOnChildClosed();
    };
    PermissionsComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    PermissionsComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    PermissionsComponent.prototype.idValueGetter = function (params) {
        var permission = params.data;
        return "ID: " + permission.Id + "\nGUID: " + permission.Guid;
    };
    PermissionsComponent.prototype.fetchPermissions = function () {
        var _this = this;
        this.permissionsService.getAll(this.targetType, this.keyType, this.key).subscribe(function (permissions) {
            _this.permissions = permissions;
        });
    };
    PermissionsComponent.prototype.editPermission = function (params) {
        var e_1, _a;
        var form;
        if (params == null) {
            var target = void 0;
            var keys = Object.keys(_shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].metadata);
            try {
                for (var keys_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(keys), keys_1_1 = keys_1.next(); !keys_1_1.done; keys_1_1 = keys_1.next()) {
                    var key = keys_1_1.value;
                    if (_shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].metadata[key].type !== this.targetType) {
                        continue;
                    }
                    target = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].metadata[key].target;
                    break;
                }
            }
            catch (e_1_1) { e_1 = { error: e_1_1 }; }
            finally {
                try {
                    if (keys_1_1 && !keys_1_1.done && (_a = keys_1.return)) _a.call(keys_1);
                }
                finally { if (e_1) throw e_1.error; }
            }
            form = {
                items: [{
                        ContentTypeName: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].contentTypes.permissions,
                        For: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({ Target: target }, (this.keyType === _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].keyTypes.guid && { Guid: this.key })), (this.keyType === _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].keyTypes.number && { Number: parseInt(this.key, 10) })), (this.keyType === _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].keyTypes.string && { String: this.key }))
                    }],
            };
        }
        else {
            var permission = params.data;
            form = {
                items: [{ EntityId: permission.Id }],
            };
        }
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_13__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    PermissionsComponent.prototype.deletePermission = function (permission) {
        var _this = this;
        if (!confirm("Delete '" + permission.Title + "' (" + permission.Id + ")?")) {
            return;
        }
        this.snackBar.open('Deleting...');
        this.permissionsService.delete(permission.Id).subscribe(function () {
            _this.snackBar.open('Deleted', null, { duration: 2000 });
            _this.fetchPermissions();
        });
    };
    PermissionsComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_6__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchPermissions();
            }
        }));
    };
    PermissionsComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_permissions_service__WEBPACK_IMPORTED_MODULE_8__["PermissionsService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"] }
    ]; };
    PermissionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-permissions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./permissions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/permissions/permissions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./permissions.component.scss */ "./src/app/permissions/permissions.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_permissions_service__WEBPACK_IMPORTED_MODULE_8__["PermissionsService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"]])
    ], PermissionsComponent);
    return PermissionsComponent;
}());



/***/ }),

/***/ "./src/app/permissions/permissions.module.ts":
/*!***************************************************!*\
  !*** ./src/app/permissions/permissions.module.ts ***!
  \***************************************************/
/*! exports provided: PermissionsModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PermissionsModule", function() { return PermissionsModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/core */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @ag-grid-community/angular */ "../../node_modules/@ag-grid-community/angular/__ivy_ngcc__/fesm5/ag-grid-community-angular.js");
/* harmony import */ var _permissions_routing_module__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./permissions-routing.module */ "./src/app/permissions/permissions-routing.module.ts");
/* harmony import */ var _permissions_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./permissions.component */ "./src/app/permissions/permissions.component.ts");
/* harmony import */ var _ag_grid_components_permissions_actions_permissions_actions_component__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./ag-grid-components/permissions-actions/permissions-actions.component */ "./src/app/permissions/ag-grid-components/permissions-actions/permissions-actions.component.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _services_permissions_service__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./services/permissions.service */ "./src/app/permissions/services/permissions.service.ts");
/* harmony import */ var _services_metadata_service__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./services/metadata.service */ "./src/app/permissions/services/metadata.service.ts");
/* harmony import */ var _content_items_services_entities_service__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../content-items/services/entities.service */ "./src/app/content-items/services/entities.service.ts");


















var PermissionsModule = /** @class */ (function () {
    function PermissionsModule() {
    }
    PermissionsModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _permissions_component__WEBPACK_IMPORTED_MODULE_11__["PermissionsComponent"],
                _ag_grid_components_permissions_actions_permissions_actions_component__WEBPACK_IMPORTED_MODULE_12__["PermissionsActionsComponent"],
            ],
            entryComponents: [
                _permissions_component__WEBPACK_IMPORTED_MODULE_11__["PermissionsComponent"],
                _ag_grid_components_permissions_actions_permissions_actions_component__WEBPACK_IMPORTED_MODULE_12__["PermissionsActionsComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _permissions_routing_module__WEBPACK_IMPORTED_MODULE_10__["PermissionsRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_13__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_4__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_5__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_6__["MatTooltipModule"],
                _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_9__["AgGridModule"].withComponents([]),
                _angular_material_core__WEBPACK_IMPORTED_MODULE_7__["MatRippleModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_8__["MatSnackBarModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_14__["Context"],
                _services_permissions_service__WEBPACK_IMPORTED_MODULE_15__["PermissionsService"],
                _services_metadata_service__WEBPACK_IMPORTED_MODULE_16__["MetadataService"],
                _content_items_services_entities_service__WEBPACK_IMPORTED_MODULE_17__["EntitiesService"],
            ]
        })
    ], PermissionsModule);
    return PermissionsModule;
}());



/***/ }),

/***/ "./src/app/permissions/services/permissions.service.ts":
/*!*************************************************************!*\
  !*** ./src/app/permissions/services/permissions.service.ts ***!
  \*************************************************************/
/*! exports provided: PermissionsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PermissionsService", function() { return PermissionsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _metadata_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./metadata.service */ "./src/app/permissions/services/metadata.service.ts");
/* harmony import */ var _content_items_services_entities_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../content-items/services/entities.service */ "./src/app/content-items/services/entities.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");





var PermissionsService = /** @class */ (function () {
    function PermissionsService(metadataService, entitiesService) {
        this.metadataService = metadataService;
        this.entitiesService = entitiesService;
    }
    PermissionsService.prototype.getAll = function (targetType, keyType, key) {
        return this.metadataService.getMetadata(targetType, keyType, key, _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_4__["eavConstants"].contentTypes.permissions);
    };
    PermissionsService.prototype.delete = function (id) {
        return this.entitiesService.delete(_shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_4__["eavConstants"].contentTypes.permissions, id, false);
    };
    PermissionsService.ctorParameters = function () { return [
        { type: _metadata_service__WEBPACK_IMPORTED_MODULE_2__["MetadataService"] },
        { type: _content_items_services_entities_service__WEBPACK_IMPORTED_MODULE_3__["EntitiesService"] }
    ]; };
    PermissionsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_metadata_service__WEBPACK_IMPORTED_MODULE_2__["MetadataService"], _content_items_services_entities_service__WEBPACK_IMPORTED_MODULE_3__["EntitiesService"]])
    ], PermissionsService);
    return PermissionsService;
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
//# sourceMappingURL=permissions-permissions-module.js.map