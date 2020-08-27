(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["common"],{

/***/ "../../node_modules/@ngx-translate/http-loader/__ivy_ngcc__/fesm5/ngx-translate-http-loader.js":
/*!***********************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/@ngx-translate/http-loader/__ivy_ngcc__/fesm5/ngx-translate-http-loader.js ***!
  \***********************************************************************************************************************************/
/*! exports provided: TranslateHttpLoader */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TranslateHttpLoader", function() { return TranslateHttpLoader; });
/**
 * @fileoverview added by tsickle
 * @suppress {checkTypes,extraRequire,missingReturn,uselessCode} checked by tsc
 */
var TranslateHttpLoader = /** @class */ (function () {
    function TranslateHttpLoader(http, prefix, suffix) {
        if (prefix === void 0) { prefix = "/assets/i18n/"; }
        if (suffix === void 0) { suffix = ".json"; }
        this.http = http;
        this.prefix = prefix;
        this.suffix = suffix;
    }
    /**
     * Gets the translations from the server
     */
    /**
     * Gets the translations from the server
     * @param {?} lang
     * @return {?}
     */
    TranslateHttpLoader.prototype.getTranslation = /**
     * Gets the translations from the server
     * @param {?} lang
     * @return {?}
     */
    function (lang) {
        return this.http.get("" + this.prefix + lang + this.suffix);
    };
    return TranslateHttpLoader;
}());

/**
 * @fileoverview added by tsickle
 * @suppress {checkTypes,extraRequire,missingReturn,uselessCode} checked by tsc
 */

/**
 * @fileoverview added by tsickle
 * @suppress {checkTypes,extraRequire,missingReturn,uselessCode} checked by tsc
 */



//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibmd4LXRyYW5zbGF0ZS1odHRwLWxvYWRlci5qcyIsInNvdXJjZXMiOlsibmc6L0BuZ3gtdHJhbnNsYXRlL2h0dHAtbG9hZGVyL2xpYi9odHRwLWxvYWRlci50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7O0FBSUE7SUFDRSw2QkFBb0IsSUFBZ0IsRUFBUyxNQUFnQyxFQUFTLE1BQXdCO1FBQWpFLHVCQUFBLEVBQUEsd0JBQWdDO1FBQVMsdUJBQUEsRUFBQSxnQkFBd0I7UUFBMUYsU0FBSSxHQUFKLElBQUksQ0FBWTtRQUFTLFdBQU0sR0FBTixNQUFNLENBQTBCO1FBQVMsV0FBTSxHQUFOLE1BQU0sQ0FBa0I7S0FBSTs7Ozs7Ozs7O0lBSzNHLDRDQUFjOzs7OztJQUFyQixVQUFzQixJQUFZO1FBQ2hDLE9BQU8sSUFBSSxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsS0FBRyxJQUFJLENBQUMsTUFBTSxHQUFHLElBQUksR0FBRyxJQUFJLENBQUMsTUFBUSxDQUFDLENBQUM7S0FDN0Q7SUFDSCwwQkFBQztDQUFBIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHtIdHRwQ2xpZW50fSBmcm9tIFwiQGFuZ3VsYXIvY29tbW9uL2h0dHBcIjtcbmltcG9ydCB7VHJhbnNsYXRlTG9hZGVyfSBmcm9tIFwiQG5neC10cmFuc2xhdGUvY29yZVwiO1xuaW1wb3J0IHtPYnNlcnZhYmxlfSBmcm9tICdyeGpzJztcblxuZXhwb3J0IGNsYXNzIFRyYW5zbGF0ZUh0dHBMb2FkZXIgaW1wbGVtZW50cyBUcmFuc2xhdGVMb2FkZXIge1xuICBjb25zdHJ1Y3Rvcihwcml2YXRlIGh0dHA6IEh0dHBDbGllbnQsIHB1YmxpYyBwcmVmaXg6IHN0cmluZyA9IFwiL2Fzc2V0cy9pMThuL1wiLCBwdWJsaWMgc3VmZml4OiBzdHJpbmcgPSBcIi5qc29uXCIpIHt9XG5cbiAgLyoqXG4gICAqIEdldHMgdGhlIHRyYW5zbGF0aW9ucyBmcm9tIHRoZSBzZXJ2ZXJcbiAgICovXG4gIHB1YmxpYyBnZXRUcmFuc2xhdGlvbihsYW5nOiBzdHJpbmcpOiBPYnNlcnZhYmxlPE9iamVjdD4ge1xuICAgIHJldHVybiB0aGlzLmh0dHAuZ2V0KGAke3RoaXMucHJlZml4fSR7bGFuZ30ke3RoaXMuc3VmZml4fWApO1xuICB9XG59XG4iXX0=

/***/ }),

/***/ "./src/app/app-administration/services/content-export.service.ts":
/*!***********************************************************************!*\
  !*** ./src/app/app-administration/services/content-export.service.ts ***!
  \***********************************************************************/
/*! exports provided: ContentExportService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentExportService", function() { return ContentExportService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");




var ContentExportService = /** @class */ (function () {
    function ContentExportService(context, dnnContext) {
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ContentExportService.prototype.exportContent = function (values, selectedIds) {
        var selectedIdsString = selectedIds ? '&selectedids=' + selectedIds.join() : '';
        var url = this.dnnContext.$2sxc.http.apiUrl('eav/ContentExport/ExportContent')
            + '?appId=' + this.context.appId
            + '&language=' + values.language
            + '&defaultLanguage=' + values.defaultLanguage
            + '&contentType=' + values.contentTypeStaticName
            + '&recordExport=' + values.recordExport
            + '&resourcesReferences=' + values.resourcesReferences
            + '&languageReferences=' + values.languageReferences
            + selectedIdsString;
        window.open(url, '_blank', '');
    };
    ContentExportService.prototype.exportJson = function (typeName) {
        var url = this.dnnContext.$2sxc.http.apiUrl('eav/ContentExport/DownloadTypeAsJson')
            + '?appId=' + this.context.appId
            + '&name=' + typeName;
        window.open(url, '_blank', '');
    };
    ContentExportService.prototype.exportEntity = function (id, prefix, metadata) {
        var url = this.dnnContext.$2sxc.http.apiUrl('eav/ContentExport/DownloadEntityAsJson')
            + '?appId=' + this.context.appId
            + '&id=' + id
            + '&prefix=' + prefix
            + '&withMetadata=' + metadata;
        window.open(url, '_blank', '');
    };
    ContentExportService.ctorParameters = function () { return [
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_3__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_2__["Context"] }
    ]; };
    ContentExportService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_shared_services_context__WEBPACK_IMPORTED_MODULE_3__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_2__["Context"]])
    ], ContentExportService);
    return ContentExportService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/content-types.service.ts":
/*!**********************************************************************!*\
  !*** ./src/app/app-administration/services/content-types.service.ts ***!
  \**********************************************************************/
/*! exports provided: ContentTypesService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypesService", function() { return ContentTypesService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");






var ContentTypesService = /** @class */ (function () {
    function ContentTypesService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ContentTypesService.prototype.retrieveContentType = function (staticName) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/get'), {
            params: { appId: this.context.appId.toString(), contentTypeId: staticName }
        });
    };
    ContentTypesService.prototype.retrieveContentTypes = function (scope) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/get'), {
            params: { appId: this.context.appId.toString(), scope: scope }
        });
    };
    ContentTypesService.prototype.getScopes = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/scopes'), {
            params: { appId: this.context.appId.toString() }
        }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (scopes) {
            var scopeOptions = Object.keys(scopes).map(function (key) { return ({ name: scopes[key], value: key }); });
            return scopeOptions;
        }));
    };
    ContentTypesService.prototype.save = function (contentType) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/save'), contentType, {
            params: { appid: this.context.appId.toString() },
        });
    };
    ContentTypesService.prototype.delete = function (contentType) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/delete'), {
            params: { appid: this.context.appId.toString(), staticName: contentType.StaticName },
        });
    };
    ContentTypesService.prototype.createGhost = function (sourceStaticName) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/createghost'), {
            params: { appid: this.context.appId.toString(), sourceStaticName: sourceStaticName },
        });
    };
    ContentTypesService.prototype.getDetails = function (contentTypeName, config) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/GetSingle'), { params: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, config), { appid: this.context.appId.toString(), contentTypeStaticName: contentTypeName }) });
    };
    ContentTypesService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"] }
    ]; };
    ContentTypesService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"]])
    ], ContentTypesService);
    return ContentTypesService;
}());



/***/ }),

/***/ "./src/app/apps-management/services/apps-list.service.ts":
/*!***************************************************************!*\
  !*** ./src/app/apps-management/services/apps-list.service.ts ***!
  \***************************************************************/
/*! exports provided: AppsListService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppsListService", function() { return AppsListService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var AppsListService = /** @class */ (function () {
    function AppsListService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    AppsListService.prototype.getAll = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/apps'), {
            params: { zoneId: this.context.zoneId.toString() }
        });
    };
    AppsListService.prototype.create = function (name) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/app'), {}, {
            params: { zoneId: this.context.zoneId.toString(), name: name }
        });
    };
    AppsListService.prototype.delete = function (appId) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/deleteapp'), {
            params: { zoneId: this.context.zoneId.toString(), appId: appId.toString() },
        });
    };
    AppsListService.prototype.flushCache = function (appId) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/flushcache'), {
            params: { zoneId: this.context.zoneId.toString(), appId: appId.toString() },
        });
    };
    AppsListService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    AppsListService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], AppsListService);
    return AppsListService;
}());



/***/ }),

/***/ "./src/app/content-items/services/content-items.service.ts":
/*!*****************************************************************!*\
  !*** ./src/app/content-items/services/content-items.service.ts ***!
  \*****************************************************************/
/*! exports provided: ContentItemsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentItemsService", function() { return ContentItemsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _shared_helpers_file_to_base64_helper__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/helpers/file-to-base64.helper */ "./src/app/shared/helpers/file-to-base64.helper.ts");






var ContentItemsService = /** @class */ (function () {
    function ContentItemsService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ContentItemsService.prototype.getAll = function (contentTypeStaticName) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/entities/GetAllOfTypeForAdmin'), {
            params: { appId: this.context.appId.toString(), contentType: contentTypeStaticName }
        });
    };
    ContentItemsService.prototype.getColumns = function (contentTypeStaticName) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/getfields'), {
            params: { appId: this.context.appId.toString(), staticName: contentTypeStaticName }
        });
    };
    ContentItemsService.prototype.importItem = function (file) {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _a, _b, _c, _d;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_e) {
                switch (_e.label) {
                    case 0:
                        _b = (_a = this.http).post;
                        _c = [this.dnnContext.$2sxc.http.apiUrl('eav/contentimport/import')];
                        _d = {
                            AppId: this.context.appId.toString()
                        };
                        return [4 /*yield*/, Object(_shared_helpers_file_to_base64_helper__WEBPACK_IMPORTED_MODULE_5__["toBase64"])(file)];
                    case 1: return [2 /*return*/, _b.apply(_a, _c.concat([(_d.ContentBase64 = _e.sent(),
                                _d)]))];
                }
            });
        });
    };
    ContentItemsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ContentItemsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ContentItemsService);
    return ContentItemsService;
}());



/***/ }),

/***/ "./src/app/content-items/services/entities.service.ts":
/*!************************************************************!*\
  !*** ./src/app/content-items/services/entities.service.ts ***!
  \************************************************************/
/*! exports provided: EntitiesService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EntitiesService", function() { return EntitiesService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var EntitiesService = /** @class */ (function () {
    function EntitiesService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    EntitiesService.prototype.delete = function (type, id, tryForce) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/entities/delete'), {
            params: { contentType: type, id: id.toString(), appId: this.context.appId.toString(), force: tryForce.toString() },
        });
    };
    EntitiesService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    EntitiesService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], EntitiesService);
    return EntitiesService;
}());



/***/ }),

/***/ "./src/app/content-type-fields/constants/data-type.constants.ts":
/*!**********************************************************************!*\
  !*** ./src/app/content-type-fields/constants/data-type.constants.ts ***!
  \**********************************************************************/
/*! exports provided: DataTypeConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DataTypeConstants", function() { return DataTypeConstants; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var DataTypeConstants = /** @class */ (function () {
    function DataTypeConstants() {
    }
    DataTypeConstants.String = 'String';
    DataTypeConstants.Entity = 'Entity';
    DataTypeConstants.Boolean = 'Boolean';
    DataTypeConstants.Number = 'Number';
    DataTypeConstants.Custom = 'Custom';
    DataTypeConstants.DateTime = 'DateTime';
    DataTypeConstants.Hyperlink = 'Hyperlink';
    DataTypeConstants.Empty = 'Empty';
    return DataTypeConstants;
}());



/***/ }),

/***/ "./src/app/content-type-fields/constants/input-type.constants.ts":
/*!***********************************************************************!*\
  !*** ./src/app/content-type-fields/constants/input-type.constants.ts ***!
  \***********************************************************************/
/*! exports provided: InputTypeConstants */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "InputTypeConstants", function() { return InputTypeConstants; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var InputTypeConstants = /** @class */ (function () {
    function InputTypeConstants() {
    }
    // String
    InputTypeConstants.StringDefault = 'string-default';
    InputTypeConstants.StringUrlPath = 'string-url-path';
    InputTypeConstants.StringDropdown = 'string-dropdown';
    InputTypeConstants.StringDropdownQuery = 'string-dropdown-query';
    InputTypeConstants.StringFontIconPicker = 'string-font-icon-picker';
    InputTypeConstants.StringTemplatePicker = 'string-template-picker';
    InputTypeConstants.StringWysiwyg = 'string-wysiwyg';
    // Boolean
    InputTypeConstants.BooleanDefault = 'boolean-default';
    // DateTime
    InputTypeConstants.DatetimeDefault = 'datetime-default';
    // Empty
    InputTypeConstants.EmptyDefault = 'empty-default';
    // Number
    InputTypeConstants.NumberDefault = 'number-default';
    // Entity
    InputTypeConstants.EntityDefault = 'entity-default';
    InputTypeConstants.EntityQuery = 'entity-query';
    InputTypeConstants.EntityContentBlocks = 'entity-content-blocks';
    // Hyperlink
    InputTypeConstants.HyperlinkDefault = 'hyperlink-default';
    InputTypeConstants.HyperlinkLibrary = 'hyperlink-library';
    // Custom
    InputTypeConstants.ExternalWebComponent = 'external-web-component';
    InputTypeConstants.CustomGPS = 'custom-gps';
    InputTypeConstants.CustomDefault = 'custom-default';
    // Old
    InputTypeConstants.OldTypeDropdown = 'dropdown';
    InputTypeConstants.OldTypeWysiwyg = 'wysiwyg';
    InputTypeConstants.OldTypeDefault = 'default';
    InputTypeConstants.StringWysiwygTinymce = 'string-wysiwyg-tinymce';
    InputTypeConstants.StringWysiwygAdv = 'string-wysiwyg-adv';
    InputTypeConstants.StringWysiwygDnn = 'string-wysiwyg-dnn';
    // Default
    InputTypeConstants.DefaultSuffix = '-default';
    return InputTypeConstants;
}());



/***/ }),

/***/ "./src/app/manage-content-list/services/content-group.service.ts":
/*!***********************************************************************!*\
  !*** ./src/app/manage-content-list/services/content-group.service.ts ***!
  \***********************************************************************/
/*! exports provided: ContentGroupService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentGroupService", function() { return ContentGroupService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var ContentGroupService = /** @class */ (function () {
    function ContentGroupService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ContentGroupService.prototype.getItems = function (item) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/contentgroup/replace'), {
            params: { appId: this.context.appId.toString(), guid: item.guid, part: item.part, index: item.index.toString() }
        });
    };
    ContentGroupService.prototype.saveItem = function (item) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/contentgroup/replace'), {}, {
            params: { guid: item.guid, part: item.part, index: item.index.toString(), entityId: item.id.toString(), add: "" + item.add }
        });
    };
    ContentGroupService.prototype.getList = function (contentGroup) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/contentgroup/itemlist'), {
            params: { appId: this.context.appId.toString(), guid: contentGroup.guid, part: contentGroup.part }
        });
    };
    ContentGroupService.prototype.saveList = function (contentGroup, resortedList) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/contentgroup/itemlist'), resortedList, {
            params: { appId: this.context.appId.toString(), guid: contentGroup.guid, part: contentGroup.part }
        });
    };
    ContentGroupService.prototype.getHeader = function (contentGroup) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/contentgroup/header'), {
            params: { appId: this.context.appId.toString(), guid: contentGroup.guid }
        });
    };
    ContentGroupService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ContentGroupService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ContentGroupService);
    return ContentGroupService;
}());



/***/ }),

/***/ "./src/app/permissions/services/metadata.service.ts":
/*!**********************************************************!*\
  !*** ./src/app/permissions/services/metadata.service.ts ***!
  \**********************************************************/
/*! exports provided: MetadataService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MetadataService", function() { return MetadataService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var MetadataService = /** @class */ (function () {
    function MetadataService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    /**
     * Fetches metadata for given key in metadata content type
     * @param typeId metadataOf something. For more info checkout out eavConstants file
     * @param keyType e.g. for keyType === guid, key === contentTypeStaticName
     * @param key key of content type for which we search for permissions. Key is connected with keyType
     * @param contentTypeName name of content type where permissions are stored
     */
    MetadataService.prototype.getMetadata = function (typeId, keyType, key, contentTypeName) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/metadata/get'), {
            params: {
                appId: this.context.appId.toString(),
                targetType: typeId.toString(),
                keyType: keyType,
                key: key,
                contentType: contentTypeName,
            },
        });
    };
    MetadataService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    MetadataService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], MetadataService);
    return MetadataService;
}());



/***/ }),

/***/ "./src/app/shared/helpers/file-to-base64.helper.ts":
/*!*********************************************************!*\
  !*** ./src/app/shared/helpers/file-to-base64.helper.ts ***!
  \*********************************************************/
/*! exports provided: toBase64 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "toBase64", function() { return toBase64; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function toBase64(file) {
    return new Promise(function (resolve, reject) {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function () { return resolve(reader.result.split(',')[1]); };
        reader.onerror = function (error) { return reject(error); };
    });
}


/***/ }),

/***/ "./src/app/shared/helpers/load-scripts.helper.ts":
/*!*******************************************************!*\
  !*** ./src/app/shared/helpers/load-scripts.helper.ts ***!
  \*******************************************************/
/*! exports provided: loadScripts, ScriptObject */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "loadScripts", function() { return loadScripts; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ScriptObject", function() { return ScriptObject; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function loadScripts(scriptObjects, callback, iteration) {
    if (iteration === void 0) { iteration = 0; }
    var isLast = scriptObjects.length === iteration + 1;
    var newCallback = isLast ? callback : loadScripts.bind(this, scriptObjects, callback, iteration + 1);
    var scrObj = scriptObjects[iteration];
    var global = typeof scrObj.test === 'string' ? scrObj.test : null;
    var test = typeof scrObj.test === 'function' ? scrObj.test : null;
    if (global != null && !!window[global]) {
        callback();
        return;
    }
    if (test != null && test()) {
        callback();
        return;
    }
    var scriptInDom = document.querySelector("script[src=\"" + scrObj.src + "\"]");
    if (scriptInDom) {
        scriptInDom.addEventListener('load', newCallback, { once: true });
        return;
    }
    var scriptEl = document.createElement('script');
    scriptEl.src = scrObj.src;
    scriptEl.addEventListener('load', newCallback, { once: true });
    document.head.appendChild(scriptEl);
}
var ScriptObject = /** @class */ (function () {
    function ScriptObject() {
    }
    return ScriptObject;
}());



/***/ }),

/***/ "./src/app/shared/services/dialog.service.ts":
/*!***************************************************!*\
  !*** ./src/app/shared/services/dialog.service.ts ***!
  \***************************************************/
/*! exports provided: DialogService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DialogService", function() { return DialogService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _context__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../constants/dialog-types.constants */ "./src/app/shared/constants/dialog-types.constants.ts");
/* harmony import */ var _constants_session_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../constants/session.constants */ "./src/app/shared/constants/session.constants.ts");




// tslint:disable-next-line:max-line-length

var DialogService = /** @class */ (function () {
    function DialogService(context) {
        this.context = context;
    }
    DialogService.prototype.openCodeFile = function (path) {
        var dialog = _constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].Develop;
        var form = {
            items: [{ Path: path }]
        };
        var oldHref = sessionStorage.getItem(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUrl"]);
        var oldUrl = new URL(oldHref);
        var newHref = oldUrl.origin + oldUrl.pathname + oldUrl.search;
        var newHash = this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyZoneId"], this.context.zoneId.toString()).replace('&', '#') +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyAppId"], this.context.appId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyTabId"], this.context.tabId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyModuleId"], this.context.moduleId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyContentBlockId"], this.context.contentBlockId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyLang"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyLangPri"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyLangs"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPortalRoot"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyWebsiteRoot"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPartOfPage"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUserCanDesign"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUserCanDevelop"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyAppRoot"], this.context.appRoot) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyFa"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyRequestToken"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDialog"], dialog) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyItems"], JSON.stringify(form.items)) +
            (sessionStorage.getItem(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDebug"]) ? this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDebug"]) : '') +
            '';
        var url = newHref + newHash;
        window.open(url, '_blank');
    };
    DialogService.prototype.openQueryDesigner = function (queryId) {
        var dialog = _constants_dialog_types_constants__WEBPACK_IMPORTED_MODULE_3__["DialogTypeConstants"].PipelineDesigner;
        var form = {
            items: [{ EntityId: queryId }],
        };
        var oldHref = sessionStorage.getItem(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUrl"]);
        var oldUrl = new URL(oldHref);
        var newHref = oldUrl.origin + oldUrl.pathname + oldUrl.search;
        var newHash = this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyZoneId"], this.context.zoneId.toString()).replace('&', '#') +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyAppId"], this.context.appId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyTabId"], this.context.tabId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyModuleId"], this.context.moduleId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyContentBlockId"], this.context.contentBlockId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyLang"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyLangPri"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyLangs"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPortalRoot"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyWebsiteRoot"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPartOfPage"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUserCanDesign"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyUserCanDevelop"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyAppRoot"], this.context.appRoot) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyFa"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyRequestToken"]) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDialog"], dialog) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyPipelineId"], queryId.toString()) +
            this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyItems"], JSON.stringify(form.items)) +
            (sessionStorage.getItem(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDebug"]) ? this.buildHashParam(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["keyDebug"]) : '') +
            '';
        var url = newHref + newHash;
        window.open(url, '_blank');
    };
    /** Encodes param if necessary */
    DialogService.prototype.buildHashParam = function (key, value) {
        var rawKey = key.replace(_constants_session_constants__WEBPACK_IMPORTED_MODULE_4__["prefix"], '');
        var valueTemp = (value != null) ? value : sessionStorage.getItem(key);
        var rawValue = encodeURIComponent(valueTemp);
        var hashParam = "&" + rawKey + "=" + rawValue;
        return hashParam;
    };
    DialogService.ctorParameters = function () { return [
        { type: _context__WEBPACK_IMPORTED_MODULE_2__["Context"] }
    ]; };
    DialogService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_context__WEBPACK_IMPORTED_MODULE_2__["Context"]])
    ], DialogService);
    return DialogService;
}());



/***/ })

}]);
//# sourceMappingURL=common.js.map