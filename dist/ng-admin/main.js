(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ "./$$_lazy_route_resource lazy recursive":
/*!******************************************************!*\
  !*** ./$$_lazy_route_resource lazy namespace object ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncaught exception popping up in devtools
	return Promise.resolve().then(function() {
		var e = new Error("Cannot find module '" + req + "'");
		e.code = 'MODULE_NOT_FOUND';
		throw e;
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "./$$_lazy_route_resource lazy recursive";

/***/ }),

/***/ "./src/app/app.component.css":
/*!***********************************!*\
  !*** ./src/app/app.component.css ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9lYXYtaW5mb3JtYXRpb24tZGlhbG9ncy9zcmMvYXBwL2FwcC5jb21wb25lbnQuY3NzIn0= */"

/***/ }),

/***/ "./src/app/app.component.html":
/*!************************************!*\
  !*** ./src/app/app.component.html ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div>\r\n  <router-outlet></router-outlet>\r\n</div>\r\n<div>\r\n  <h2>Test Link</h2>\r\n  <ul>\r\n    <li>\r\n      <a \r\n        [routerLink]=\"['/rest/content-type/blog-post']\" \r\n        [queryParams]=\"{ \r\n          sxcver: '9.42.00',\r\n          systype: 'dnn',\r\n          sysver: '9.2.2',\r\n          z: 3,\r\n          a: 3,\r\n          p: 3,\r\n          i: 3,\r\n          c: 3,\r\n          d: false,\r\n          lc: ['us','en'],\r\n          lui: ['us','en'],\r\n          lp: '',\r\n          rtt: '',\r\n          rtw: '/',\r\n          rta: '',\r\n          pop: false,\r\n          uid: true,\r\n          uic: true,\r\n          uis: true\r\n        }\"\r\n        routerLinkActive=\"active\">\r\n        content-type with parameters\r\n      </a>\r\n    </li>\r\n    <li>\r\n        <a \r\n          [routerLink]=\"['/rest/item']\"\r\n          routerLinkActive=\"active\">\r\n          item without parameters\r\n        </a>\r\n    </li>\r\n    <li>\r\n        <a \r\n          [routerLink]=\"['/rest/query']\"\r\n          [queryParams]=\"{ \r\n            sxcver: '7.42.00',\r\n            systype: 'dnn',\r\n            sysver: '7.2.2',\r\n            z: 4,\r\n            a: 4,\r\n            p: 4,\r\n            i: 4,\r\n            c: 4,\r\n            d: false,\r\n            lp: '',\r\n            rtt: '',\r\n            rtw: '/',\r\n            rta: '',\r\n            pop: false,\r\n            uid: false,\r\n            uic: false,\r\n            uis: false\r\n          }\"\r\n          routerLinkActive=\"active\">\r\n          query with parameters\r\n        </a>\r\n    </li>\r\n    <li>\r\n      <a href=\"/#/?sxcver=9.42.00&systype=dnn&sysver=9.2.2&z=1&a=1&p=1&i=1&c=1&d=false&lc=[de,en]&lui=[de,en]&lp=&rtt=&rtw=%2F&rta=&pop=false&uid=false&uic=false&uis=false\">Test Link with all parameters</a>\r\n    </li>\r\n    <li>\r\n      <a href=\"/#/?sxcver=9.42.00&systype=dnn&z=2&a=2&p=2&i=2&c=2&d=true&lc=[de,en]&lui=[de,en]&lp=&rtt=&rtw=%2F&rta=&pop=false&uid=false&uic=false&uis=false\">Test Link without sysver</a>\r\n    </li>\r\n  </ul>\r\n</div>\r\n\r\n<div class=\"debug-content\">\r\n  <h2>Storage</h2>\r\n  <a href=\"javascript:void(0)\" onclick=\"window.localStorage.clear(); location.href='/'\">clear storage</a><br />\r\n  <a href=\"javascript:void(0)\" (click)=\"getLocalStorageParamsByName('sysVersion')\">get sysVersion</a><br />\r\n  <a href=\"javascript:void(0)\" (click)=\"getLocalStorageParamsByName('sxcVersion')\">get sxcVersion</a><br />\r\n\r\n  <ul id=\"storage-value\"></ul>\r\n</div>"

/***/ }),

/***/ "./src/app/app.component.ts":
/*!**********************************!*\
  !*** ./src/app/app.component.ts ***!
  \**********************************/
/*! exports provided: AppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppComponent", function() { return AppComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _state_state_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./state/state.service */ "./src/app/state/state.service.ts");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");





var AppComponent = /** @class */ (function () {
    function AppComponent(state, route) {
        this.state = state;
        this.route = route;
    }
    AppComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.route.queryParams.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_4__["skip"])(1)).subscribe(function (params) {
            _this.state.putUrlParamsInLocalStorage(params);
        });
        this.state.showLocalStorageParams();
    };
    AppComponent.prototype.getLocalStorageParamsByName = function (name) {
        alert(this.state.getLocalStorageParamsByName(name));
    };
    AppComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-root',
            template: __webpack_require__(/*! ./app.component.html */ "./src/app/app.component.html"),
            styles: [__webpack_require__(/*! ./app.component.css */ "./src/app/app.component.css")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_state_state_service__WEBPACK_IMPORTED_MODULE_3__["StateService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"]])
    ], AppComponent);
    return AppComponent;
}());



/***/ }),

/***/ "./src/app/app.module.ts":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/fesm5/platform-browser.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _app_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./app.component */ "./src/app/app.component.ts");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/platform-browser/animations */ "../../node_modules/@angular/platform-browser/fesm5/animations.js");
/* harmony import */ var _routing_app_routing_module__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./routing/app-routing.module */ "./src/app/routing/app-routing.module.ts");
/* harmony import */ var _rest_rest_module__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./rest/rest.module */ "./src/app/rest/rest.module.ts");







var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["NgModule"])({
            declarations: [
                _app_component__WEBPACK_IMPORTED_MODULE_3__["AppComponent"],
            ],
            imports: [
                _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__["BrowserModule"],
                _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_4__["BrowserAnimationsModule"],
                _rest_rest_module__WEBPACK_IMPORTED_MODULE_6__["RestModule"],
                _routing_app_routing_module__WEBPACK_IMPORTED_MODULE_5__["AppRoutingModule"],
            ],
            providers: [],
            bootstrap: [_app_component__WEBPACK_IMPORTED_MODULE_3__["AppComponent"]]
        })
    ], AppModule);
    return AppModule;
}());



/***/ }),

/***/ "./src/app/rest/content-type-rest/content-type-rest.component.css":
/*!************************************************************************!*\
  !*** ./src/app/rest/content-type-rest/content-type-rest.component.css ***!
  \************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9lYXYtaW5mb3JtYXRpb24tZGlhbG9ncy9zcmMvYXBwL3Jlc3QvY29udGVudC10eXBlLXJlc3QvY29udGVudC10eXBlLXJlc3QuY29tcG9uZW50LmNzcyJ9 */"

/***/ }),

/***/ "./src/app/rest/content-type-rest/content-type-rest.component.html":
/*!*************************************************************************!*\
  !*** ./src/app/rest/content-type-rest/content-type-rest.component.html ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!-- the title should be in the header of the dialog -->\r\n<h1>Using REST with {{typeName$ | async}}</h1>\r\n<p>\r\n  All the data in this system can easily be used in JavaScript using a REST API. This information should help you get started. \r\n</p>\r\n<!-- this should look like an important info -->\r\n<em>Important: As a host-user you can always access these REST endpoints, but if you want to use them in a public UI, you must remember to set the permissions to allow this.</em>\r\n\r\n<h2>REST Endpoints for type {{typeName$ | async}}</h2>\r\n\r\ntodo: dropdown for dnn7/8/9 - build using \"environments\"\r\n\r\n<ul>\r\n  <li>\r\n    Read list of all items (test here)<br>\r\n    <code>GET</code>: <code>{{rootAuto$ | async}}</code>\r\n  </li>\r\n  <li>\r\n    Read list of all items - when not accessing from inside a 2sxc app<br>\r\n    <code>GET</code>: <code>{{rootNamed$ | async}}</code>\r\n  </li>\r\n\r\n</ul>"

/***/ }),

/***/ "./src/app/rest/content-type-rest/content-type-rest.component.ts":
/*!***********************************************************************!*\
  !*** ./src/app/rest/content-type-rest/content-type-rest.component.ts ***!
  \***********************************************************************/
/*! exports provided: ContentTypeRestComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeRestComponent", function() { return ContentTypeRestComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var _state_state_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../state/state.service */ "./src/app/state/state.service.ts");
/* harmony import */ var _environments__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../environments */ "./src/app/rest/environments.ts");







var pathToContent = '{appname}/content/{typename}';
var ContentTypeRestComponent = /** @class */ (function () {
    function ContentTypeRestComponent(route, state) {
        this.route = route;
        this.state = state;
        this.environments = _environments__WEBPACK_IMPORTED_MODULE_6__["Environments"];
        this.system = new rxjs__WEBPACK_IMPORTED_MODULE_4__["BehaviorSubject"](_environments__WEBPACK_IMPORTED_MODULE_6__["Environments"][0].key);
    }
    ContentTypeRestComponent.prototype.ngOnInit = function () {
        this.wireUpObservables();
    };
    /** setup all observables */
    ContentTypeRestComponent.prototype.wireUpObservables = function () {
        // type name
        this.typeName$ = this.route.paramMap.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (params) {
            return params.get('name');
        }));
        var rootWithoutApp$ = Object(rxjs__WEBPACK_IMPORTED_MODULE_4__["combineLatest"])(this.typeName$, this.system.asObservable(), function (t, s) {
            return _environments__WEBPACK_IMPORTED_MODULE_6__["Environments"].find(function (e) { return e.key === s; }).rootPath
                + pathToContent.replace('{typename}', t);
        });
        this.rootAuto$ = rootWithoutApp$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (r) { return r.replace('{appname}', 'auto'); }));
        this.rootNamed$ = rootWithoutApp$.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (r) { return r.replace('{appname}', 'put-the-app-name-here'); }));
    };
    ContentTypeRestComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-type-rest',
            template: __webpack_require__(/*! ./content-type-rest.component.html */ "./src/app/rest/content-type-rest/content-type-rest.component.html"),
            styles: [__webpack_require__(/*! ./content-type-rest.component.css */ "./src/app/rest/content-type-rest/content-type-rest.component.css")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _state_state_service__WEBPACK_IMPORTED_MODULE_5__["StateService"]])
    ], ContentTypeRestComponent);
    return ContentTypeRestComponent;
}());



/***/ }),

/***/ "./src/app/rest/environments.ts":
/*!**************************************!*\
  !*** ./src/app/rest/environments.ts ***!
  \**************************************/
/*! exports provided: Environments */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Environments", function() { return Environments; });
var Environment = /** @class */ (function () {
    function Environment() {
    }
    return Environment;
}());
var Environments = [
    {
        key: 'dnn7',
        name: 'DNN 7 or higher',
        rootPath: '/desktopmodules/2sxc/api/app/'
    },
    {
        key: 'dnn8',
        name: 'DNN 8 or higher',
        rootPath: '/api/2sxc/app/'
    }
];


/***/ }),

/***/ "./src/app/rest/item-rest/item-rest.component.css":
/*!********************************************************!*\
  !*** ./src/app/rest/item-rest/item-rest.component.css ***!
  \********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9lYXYtaW5mb3JtYXRpb24tZGlhbG9ncy9zcmMvYXBwL3Jlc3QvaXRlbS1yZXN0L2l0ZW0tcmVzdC5jb21wb25lbnQuY3NzIn0= */"

/***/ }),

/***/ "./src/app/rest/item-rest/item-rest.component.html":
/*!*********************************************************!*\
  !*** ./src/app/rest/item-rest/item-rest.component.html ***!
  \*********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<p>\r\n  item-rest works!\r\n</p>\r\n"

/***/ }),

/***/ "./src/app/rest/item-rest/item-rest.component.ts":
/*!*******************************************************!*\
  !*** ./src/app/rest/item-rest/item-rest.component.ts ***!
  \*******************************************************/
/*! exports provided: ItemRestComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ItemRestComponent", function() { return ItemRestComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _state_state_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../state/state.service */ "./src/app/state/state.service.ts");



var ItemRestComponent = /** @class */ (function () {
    function ItemRestComponent(state) {
        this.state = state;
    }
    ItemRestComponent.prototype.ngOnInit = function () {
    };
    ItemRestComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-item-rest',
            template: __webpack_require__(/*! ./item-rest.component.html */ "./src/app/rest/item-rest/item-rest.component.html"),
            styles: [__webpack_require__(/*! ./item-rest.component.css */ "./src/app/rest/item-rest/item-rest.component.css")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_state_state_service__WEBPACK_IMPORTED_MODULE_2__["StateService"]])
    ], ItemRestComponent);
    return ItemRestComponent;
}());



/***/ }),

/***/ "./src/app/rest/query-rest/query-rest.component.css":
/*!**********************************************************!*\
  !*** ./src/app/rest/query-rest/query-rest.component.css ***!
  \**********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9lYXYtaW5mb3JtYXRpb24tZGlhbG9ncy9zcmMvYXBwL3Jlc3QvcXVlcnktcmVzdC9xdWVyeS1yZXN0LmNvbXBvbmVudC5jc3MifQ== */"

/***/ }),

/***/ "./src/app/rest/query-rest/query-rest.component.html":
/*!***********************************************************!*\
  !*** ./src/app/rest/query-rest/query-rest.component.html ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<p>\r\n  query-rest works!\r\n</p>\r\n"

/***/ }),

/***/ "./src/app/rest/query-rest/query-rest.component.ts":
/*!*********************************************************!*\
  !*** ./src/app/rest/query-rest/query-rest.component.ts ***!
  \*********************************************************/
/*! exports provided: QueryRestComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "QueryRestComponent", function() { return QueryRestComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _state_state_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../state/state.service */ "./src/app/state/state.service.ts");



var QueryRestComponent = /** @class */ (function () {
    function QueryRestComponent(state) {
        this.state = state;
    }
    QueryRestComponent.prototype.ngOnInit = function () {
    };
    QueryRestComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-query-rest',
            template: __webpack_require__(/*! ./query-rest.component.html */ "./src/app/rest/query-rest/query-rest.component.html"),
            styles: [__webpack_require__(/*! ./query-rest.component.css */ "./src/app/rest/query-rest/query-rest.component.css")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_state_state_service__WEBPACK_IMPORTED_MODULE_2__["StateService"]])
    ], QueryRestComponent);
    return QueryRestComponent;
}());



/***/ }),

/***/ "./src/app/rest/rest-routing.module.ts":
/*!*********************************************!*\
  !*** ./src/app/rest/rest-routing.module.ts ***!
  \*********************************************/
/*! exports provided: RestRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "RestRoutingModule", function() { return RestRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _item_rest_item_rest_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./item-rest/item-rest.component */ "./src/app/rest/item-rest/item-rest.component.ts");
/* harmony import */ var _query_rest_query_rest_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./query-rest/query-rest.component */ "./src/app/rest/query-rest/query-rest.component.ts");
/* harmony import */ var _content_type_rest_content_type_rest_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./content-type-rest/content-type-rest.component */ "./src/app/rest/content-type-rest/content-type-rest.component.ts");






var routes = [
    { path: 'rest/query', component: _query_rest_query_rest_component__WEBPACK_IMPORTED_MODULE_4__["QueryRestComponent"] },
    { path: 'rest/item', component: _item_rest_item_rest_component__WEBPACK_IMPORTED_MODULE_3__["ItemRestComponent"] },
    { path: 'rest/content-type/:name', component: _content_type_rest_content_type_rest_component__WEBPACK_IMPORTED_MODULE_5__["ContentTypeRestComponent"] },
];
var RestRoutingModule = /** @class */ (function () {
    function RestRoutingModule() {
    }
    RestRoutingModule = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], RestRoutingModule);
    return RestRoutingModule;
}());



/***/ }),

/***/ "./src/app/rest/rest.module.ts":
/*!*************************************!*\
  !*** ./src/app/rest/rest.module.ts ***!
  \*************************************/
/*! exports provided: RestModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "RestModule", function() { return RestModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/fesm5/common.js");
/* harmony import */ var _content_type_rest_content_type_rest_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./content-type-rest/content-type-rest.component */ "./src/app/rest/content-type-rest/content-type-rest.component.ts");
/* harmony import */ var _query_rest_query_rest_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./query-rest/query-rest.component */ "./src/app/rest/query-rest/query-rest.component.ts");
/* harmony import */ var _item_rest_item_rest_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./item-rest/item-rest.component */ "./src/app/rest/item-rest/item-rest.component.ts");
/* harmony import */ var _rest_routing_module__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./rest-routing.module */ "./src/app/rest/rest-routing.module.ts");







var RestModule = /** @class */ (function () {
    function RestModule() {
    }
    RestModule = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _content_type_rest_content_type_rest_component__WEBPACK_IMPORTED_MODULE_3__["ContentTypeRestComponent"],
                _query_rest_query_rest_component__WEBPACK_IMPORTED_MODULE_4__["QueryRestComponent"],
                _item_rest_item_rest_component__WEBPACK_IMPORTED_MODULE_5__["ItemRestComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _rest_routing_module__WEBPACK_IMPORTED_MODULE_6__["RestRoutingModule"]
            ]
        })
    ], RestModule);
    return RestModule;
}());



/***/ }),

/***/ "./src/app/routing/app-routing.module.ts":
/*!***********************************************!*\
  !*** ./src/app/routing/app-routing.module.ts ***!
  \***********************************************/
/*! exports provided: AppRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppRoutingModule", function() { return AppRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/fesm5/router.js");



var appRoutes = [];
var AppRoutingModule = /** @class */ (function () {
    function AppRoutingModule() {
    }
    AppRoutingModule = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [
                _angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forRoot(appRoutes, {
                    useHash: true,
                    enableTracing: false // <-- debugging purposes only
                }),
            ],
            exports: [
                _angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]
            ]
        })
    ], AppRoutingModule);
    return AppRoutingModule;
}());



/***/ }),

/***/ "./src/app/state/state.service.ts":
/*!****************************************!*\
  !*** ./src/app/state/state.service.ts ***!
  \****************************************/
/*! exports provided: StateService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "StateService", function() { return StateService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");


var unknownId = 0;
var unknownDebug = false;
var initParams = {
    sxcver: {
        name: 'sxcVersion',
        default: '00.00.00'
    },
    systype: {
        name: 'sysType',
        default: 'dnn'
    },
    sysver: {
        name: 'sysVersion',
        default: '00.00.00'
    },
    z: {
        name: 'zoneId',
        default: null
    },
    a: {
        name: 'appId',
        default: null
    },
    p: {
        name: 'pageId',
        default: null
    },
    i: {
        name: 'instanceId',
        default: null
    },
    c: {
        name: 'contentBlockId',
        default: null
    },
    d: {
        name: 'debug',
        default: false
    },
    lc: {
        name: 'languagesContent',
        default: []
    },
    lui: {
        name: 'languagesUi',
        default: []
    },
    lp: {
        name: 'languagePrimary',
        default: ''
    },
    rtt: {
        name: 'tenantRoot',
        default: ''
    },
    rtw: {
        name: 'websiteRoot',
        default: '/'
    },
    rta: {
        name: 'appRoot',
        default: ''
    },
    pop: {
        name: 'partOfPage',
        default: false
    },
    uid: {
        name: 'userIsDesigner',
        default: false
    },
    uic: {
        name: 'userIsCoder',
        default: false
    },
    uis: {
        name: 'userIsSuperuser',
        default: false
    }
};
var StateService = /** @class */ (function () {
    //#endregion
    function StateService() {
        //#region basic IDs
        this.appId = unknownId;
        this.zoneId = unknownId;
        this.pageId = unknownId;
        this.contentBlockId = unknownId;
        this.instanceId = unknownId;
        //#endregion
        //#region system / environment stuff
        this.debug = unknownDebug;
        this.enableSuperUser = false; // not implemented yet
    }
    StateService.prototype.putUrlParamsInLocalStorage = function (paramsObj) {
        Object.keys(paramsObj).map(function (key, index) {
            var value = paramsObj[key];
            if (typeof initParams[key] === 'undefined') {
                localStorage.setItem(key, value);
            }
            else {
                var name_1 = initParams[key].name;
                localStorage.setItem(name_1, value);
            }
        });
        this.checkForDefaultValues();
        this.transferToProperties();
        this.showLocalStorageParams();
    };
    StateService.prototype.checkForDefaultValues = function () {
        Object.keys(initParams).map(function (key) {
            var name = initParams[key].name;
            // const name = key;
            var value = initParams[key].default;
            if (localStorage.getItem(name) === null) {
                localStorage.setItem(name, value);
            }
        });
    };
    StateService.prototype.transferToProperties = function () {
        // tslint:disable:radix
        //#region basic IDs
        this.appId = parseInt(this.getLocalStorageParamsByName('appId')) || unknownId;
        this.zoneId = parseInt(this.getLocalStorageParamsByName('zoneId')) || unknownId;
        this.instanceId = parseInt(this.getLocalStorageParamsByName('instanceId')) || unknownId;
        this.pageId = parseInt(this.getLocalStorageParamsByName('pageId')) || unknownId;
        this.contentBlockId = parseInt(this.getLocalStorageParamsByName('contentBlockId')) || unknownId;
        //#endregion
        //#region environment
        this.debug = this.getLocalStorageParamsByName('debug') === 'true';
        //#endregion
        //#region features
        this.enableApp = this.getLocalStorageParamsByName('fa') === 'true';
        this.enableCode = this.getLocalStorageParamsByName('fc') === 'true';
        this.enableDesign = this.getLocalStorageParamsByName('fd') === 'true';
        //#endregion
    };
    StateService.prototype.getLocalStorageParamsByName = function (name) {
        if (localStorage.getItem(name) === null) {
            return '';
        }
        return localStorage.getItem(name);
    };
    // todo: 2ro move to a debug-module, and put this into a view;
    // important: never do DOM manipulations in angular
    StateService.prototype.showLocalStorageParams = function () {
        var node = document.getElementById('storage-value');
        node.innerHTML = '';
        Object.keys(localStorage).map(function (key, index) {
            var e = document.createElement('div');
            e.innerHTML = "<li>" + key + ": " + localStorage.getItem(key) + "</li>";
            node.append(e);
        });
    };
    StateService.prototype.getKeyByValue = function (object, value) {
        return Object.keys(object).find(function (key) { return object[key].name === value; });
    };
    StateService = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])({
            providedIn: 'root'
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [])
    ], StateService);
    return StateService;
}());



/***/ }),

/***/ "./src/environments/environment.ts":
/*!*****************************************!*\
  !*** ./src/environments/environment.ts ***!
  \*****************************************/
/*! exports provided: environment */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "environment", function() { return environment; });
// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.
var environment = {
    production: false
};
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.


/***/ }),

/***/ "./src/main.ts":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var hammerjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! hammerjs */ "../../node_modules/hammerjs/hammer.js");
/* harmony import */ var hammerjs__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(hammerjs__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser-dynamic */ "../../node_modules/@angular/platform-browser-dynamic/fesm5/platform-browser-dynamic.js");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./app/app.module */ "./src/app/app.module.ts");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./environments/environment */ "./src/environments/environment.ts");





if (_environments_environment__WEBPACK_IMPORTED_MODULE_4__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["enableProdMode"])();
}
Object(_angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__["platformBrowserDynamic"])().bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_3__["AppModule"])
    .catch(function (err) { return console.error(err); });


/***/ }),

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! C:\Projects\eav-admin-angular\projects\eav-information-dialogs\src\main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map