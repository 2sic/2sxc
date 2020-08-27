(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["replace-content-replace-content-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/replace-content/replace-content.component.html":
/*!****************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/replace-content/replace-content.component.html ***!
  \****************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Replace Content Item</div>\r\n</div>\r\n\r\n<router-outlet></router-outlet>\r\n\r\n<p class=\"dialog-description\">\r\n  By replacing a content-item you can make a other content appear in the slot of the original content.\r\n</p>\r\n\r\n<div class=\"options-box\">\r\n  <mat-form-field class=\"options-box__field\" appearance=\"standard\" color=\"accent\">\r\n    <mat-label>Choose item</mat-label>\r\n    <mat-select [(ngModel)]=\"item.id\" name=\"Language\">\r\n      <mat-option *ngFor=\"let option of options\" [value]=\"option.value\">{{ option.label }}</mat-option>\r\n    </mat-select>\r\n  </mat-form-field>\r\n  <button mat-icon-button class=\"options-box__copy\" matTooltip=\"Copy\" (click)=\"copySelected()\">\r\n    <mat-icon>file_copy</mat-icon>\r\n  </button>\r\n</div>\r\n\r\n<div class=\"dialog-component-actions\">\r\n  <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n  <button mat-raised-button color=\"accent\" (click)=\"save()\">Save</button>\r\n</div>\r\n");

/***/ }),

/***/ "./src/app/replace-content/replace-content-dialog.config.ts":
/*!******************************************************************!*\
  !*** ./src/app/replace-content/replace-content-dialog.config.ts ***!
  \******************************************************************/
/*! exports provided: replaceContentDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "replaceContentDialog", function() { return replaceContentDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var replaceContentDialog = {
    name: 'REPLACE_CONTENT_DIALOG',
    initContext: true,
    panelSize: 'small',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ReplaceContentComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./replace-content.component */ "./src/app/replace-content/replace-content.component.ts"))];
                    case 1:
                        ReplaceContentComponent = (_a.sent()).ReplaceContentComponent;
                        return [2 /*return*/, ReplaceContentComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/replace-content/replace-content-routing.module.ts":
/*!*******************************************************************!*\
  !*** ./src/app/replace-content/replace-content-routing.module.ts ***!
  \*******************************************************************/
/*! exports provided: ReplaceContentRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ReplaceContentRoutingModule", function() { return ReplaceContentRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _replace_content_dialog_config__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./replace-content-dialog.config */ "./src/app/replace-content/replace-content-dialog.config.ts");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../edit/edit.matcher */ "../edit/edit.matcher.ts");






var routes = [
    {
        path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _replace_content_dialog_config__WEBPACK_IMPORTED_MODULE_4__["replaceContentDialog"] }, children: [
            {
                matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); }
            },
        ]
    },
];
var ReplaceContentRoutingModule = /** @class */ (function () {
    function ReplaceContentRoutingModule() {
    }
    ReplaceContentRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], ReplaceContentRoutingModule);
    return ReplaceContentRoutingModule;
}());



/***/ }),

/***/ "./src/app/replace-content/replace-content.component.scss":
/*!****************************************************************!*\
  !*** ./src/app/replace-content/replace-content.component.scss ***!
  \****************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".options-box {\n  display: flex;\n  align-items: flex-end;\n}\n.options-box__field {\n  width: 40%;\n}\n.options-box__copy {\n  margin-left: 8px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9yZXBsYWNlLWNvbnRlbnQvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXHJlcGxhY2UtY29udGVudFxccmVwbGFjZS1jb250ZW50LmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL3JlcGxhY2UtY29udGVudC9yZXBsYWNlLWNvbnRlbnQuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxhQUFBO0VBQ0EscUJBQUE7QUNDRjtBRENFO0VBQ0UsVUFBQTtBQ0NKO0FERUU7RUFDRSxnQkFBQTtBQ0FKIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9yZXBsYWNlLWNvbnRlbnQvcmVwbGFjZS1jb250ZW50LmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLm9wdGlvbnMtYm94IHtcclxuICBkaXNwbGF5OiBmbGV4O1xyXG4gIGFsaWduLWl0ZW1zOiBmbGV4LWVuZDtcclxuXHJcbiAgJl9fZmllbGQge1xyXG4gICAgd2lkdGg6IDQwJTtcclxuICB9XHJcblxyXG4gICZfX2NvcHkge1xyXG4gICAgbWFyZ2luLWxlZnQ6IDhweDtcclxuICB9XHJcbn1cclxuIiwiLm9wdGlvbnMtYm94IHtcbiAgZGlzcGxheTogZmxleDtcbiAgYWxpZ24taXRlbXM6IGZsZXgtZW5kO1xufVxuLm9wdGlvbnMtYm94X19maWVsZCB7XG4gIHdpZHRoOiA0MCU7XG59XG4ub3B0aW9ucy1ib3hfX2NvcHkge1xuICBtYXJnaW4tbGVmdDogOHB4O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/replace-content/replace-content.component.ts":
/*!**************************************************************!*\
  !*** ./src/app/replace-content/replace-content.component.ts ***!
  \**************************************************************/
/*! exports provided: ReplaceContentComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ReplaceContentComponent", function() { return ReplaceContentComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _manage_content_list_services_content_group_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../manage-content-list/services/content-group.service */ "./src/app/manage-content-list/services/content-group.service.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");









var ReplaceContentComponent = /** @class */ (function () {
    function ReplaceContentComponent(dialogRef, contentGroupService, router, route, snackBar) {
        this.dialogRef = dialogRef;
        this.contentGroupService = contentGroupService;
        this.router = router;
        this.route = route;
        this.snackBar = snackBar;
        this.hostClass = 'dialog-component';
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_5__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild;
        this.item = {
            id: null,
            guid: this.route.snapshot.paramMap.get('guid'),
            part: this.route.snapshot.paramMap.get('part'),
            index: parseInt(this.route.snapshot.paramMap.get('index'), 10),
            add: !!this.route.snapshot.queryParamMap.get('add'),
        };
    }
    ReplaceContentComponent.prototype.ngOnInit = function () {
        this.getConfig();
        this.refreshOnChildClosed();
    };
    ReplaceContentComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ReplaceContentComponent.prototype.save = function () {
        var _this = this;
        this.snackBar.open('Saving...');
        this.contentGroupService.saveItem(this.item).subscribe(function () {
            _this.snackBar.open('Saved', null, { duration: 2000 });
            _this.closeDialog();
        });
    };
    ReplaceContentComponent.prototype.copySelected = function () {
        var form = {
            items: [{ ContentTypeName: this.contentTypeName, DuplicateEntity: this.item.id }],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_8__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route });
    };
    ReplaceContentComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ReplaceContentComponent.prototype.getConfig = function () {
        var _this = this;
        this.contentGroupService.getItems(this.item).subscribe(function (replaceConfig) {
            var e_1, _a;
            var itemKeys = Object.keys(replaceConfig.Items);
            _this.options = [];
            try {
                for (var itemKeys_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(itemKeys), itemKeys_1_1 = itemKeys_1.next(); !itemKeys_1_1.done; itemKeys_1_1 = itemKeys_1.next()) {
                    var key = itemKeys_1_1.value;
                    var nKey = parseInt(key, 10);
                    var itemName = replaceConfig.Items[nKey];
                    _this.options.push({ label: itemName + " (" + nKey + ")", value: nKey });
                }
            }
            catch (e_1_1) { e_1 = { error: e_1_1 }; }
            finally {
                try {
                    if (itemKeys_1_1 && !itemKeys_1_1.done && (_a = itemKeys_1.return)) _a.call(itemKeys_1);
                }
                finally { if (e_1) throw e_1.error; }
            }
            // don't set the ID if the dialog should be in add-mode
            if (!_this.item.id && !_this.item.add) {
                _this.item.id = replaceConfig.SelectedId;
            }
            if (!_this.contentTypeName) {
                _this.contentTypeName = replaceConfig.ContentTypeName;
            }
        });
    };
    ReplaceContentComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_6__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var _a;
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.getConfig();
                var navigation = _this.router.getCurrentNavigation();
                var editResult = (_a = navigation.extras) === null || _a === void 0 ? void 0 : _a.state;
                if (editResult) {
                    _this.item.id = editResult[Object.keys(editResult)[0]];
                }
            }
        }));
    };
    ReplaceContentComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _manage_content_list_services_content_group_service__WEBPACK_IMPORTED_MODULE_7__["ContentGroupService"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ReplaceContentComponent.prototype, "hostClass", void 0);
    ReplaceContentComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-replace-content',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./replace-content.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/replace-content/replace-content.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./replace-content.component.scss */ "./src/app/replace-content/replace-content.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _manage_content_list_services_content_group_service__WEBPACK_IMPORTED_MODULE_7__["ContentGroupService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"]])
    ], ReplaceContentComponent);
    return ReplaceContentComponent;
}());



/***/ }),

/***/ "./src/app/replace-content/replace-content.module.ts":
/*!***********************************************************!*\
  !*** ./src/app/replace-content/replace-content.module.ts ***!
  \***********************************************************/
/*! exports provided: ReplaceContentModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ReplaceContentModule", function() { return ReplaceContentModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/form-field */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/form-field.js");
/* harmony import */ var _angular_material_select__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/select */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/select.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _replace_content_routing_module__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./replace-content-routing.module */ "./src/app/replace-content/replace-content-routing.module.ts");
/* harmony import */ var _replace_content_component__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./replace-content.component */ "./src/app/replace-content/replace-content.component.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _manage_content_list_services_content_group_service__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ../manage-content-list/services/content-group.service */ "./src/app/manage-content-list/services/content-group.service.ts");
















var ReplaceContentModule = /** @class */ (function () {
    function ReplaceContentModule() {
    }
    ReplaceContentModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _replace_content_component__WEBPACK_IMPORTED_MODULE_12__["ReplaceContentComponent"],
            ],
            entryComponents: [
                _replace_content_component__WEBPACK_IMPORTED_MODULE_12__["ReplaceContentComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _replace_content_routing_module__WEBPACK_IMPORTED_MODULE_11__["ReplaceContentRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_13__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_4__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__["MatTooltipModule"],
                _angular_material_form_field__WEBPACK_IMPORTED_MODULE_8__["MatFormFieldModule"],
                _angular_material_select__WEBPACK_IMPORTED_MODULE_9__["MatSelectModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_3__["FormsModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_10__["MatSnackBarModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_14__["Context"],
                _manage_content_list_services_content_group_service__WEBPACK_IMPORTED_MODULE_15__["ContentGroupService"],
            ]
        })
    ], ReplaceContentModule);
    return ReplaceContentModule;
}());



/***/ })

}]);
//# sourceMappingURL=replace-content-replace-content-module.js.map