(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["default~app-administration-app-administration-module~apps-management-apps-management-module~content-~33e631e1"],{

/***/ "../../node_modules/@ecodev/fab-speed-dial/__ivy_ngcc__/fesm5/ecodev-fab-speed-dial.js":
/*!***************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/@ecodev/fab-speed-dial/__ivy_ngcc__/fesm5/ecodev-fab-speed-dial.js ***!
  \***************************************************************************************************************************/
/*! exports provided: EcoFabSpeedDialActionsComponent, EcoFabSpeedDialComponent, EcoFabSpeedDialModule, EcoFabSpeedDialTriggerComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EcoFabSpeedDialActionsComponent", function() { return EcoFabSpeedDialActionsComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EcoFabSpeedDialComponent", function() { return EcoFabSpeedDialComponent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EcoFabSpeedDialModule", function() { return EcoFabSpeedDialModule; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EcoFabSpeedDialTriggerComponent", function() { return EcoFabSpeedDialTriggerComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");










function EcoFabSpeedDialActionsComponent_0_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵprojection"](0, 0, ["*ngIf", "miniFabVisible"]);
} }
var _c0 = [[["", "mat-mini-fab", ""]]];
var _c1 = ["[mat-mini-fab]"];
var _c2 = [[["eco-fab-speed-dial-trigger"]], [["eco-fab-speed-dial-actions"]]];
var _c3 = ["eco-fab-speed-dial-trigger", "eco-fab-speed-dial-actions"];
var _c4 = [[["", "mat-fab", ""]]];
var _c5 = ["[mat-fab]"];
var Z_INDEX_ITEM = 23;
var EcoFabSpeedDialActionsComponent = /** @class */ (function () {
    function EcoFabSpeedDialActionsComponent(injector, renderer) {
        this.renderer = renderer;
        /**
         * Whether the min-fab button exist in DOM
         */
        this.miniFabVisible = false;
        this._parent = injector.get(EcoFabSpeedDialComponent);
    }
    EcoFabSpeedDialActionsComponent.prototype.ngAfterContentInit = function () {
        var _this = this;
        this._buttons.changes.subscribe(function () {
            _this.initButtonStates();
            _this._parent.setActionsVisibility();
        });
        this.initButtonStates();
    };
    EcoFabSpeedDialActionsComponent.prototype.initButtonStates = function () {
        var _this = this;
        this._buttons.forEach(function (button, i) {
            _this.renderer.addClass(button._getHostElement(), 'eco-fab-action-item');
            _this.changeElementStyle(button._getHostElement(), 'z-index', '' + (Z_INDEX_ITEM - i));
        });
    };
    EcoFabSpeedDialActionsComponent.prototype.show = function () {
        var _this = this;
        if (!this._buttons) {
            return;
        }
        this.resetAnimationState();
        this.miniFabVisible = true;
        this.showMiniFabAnimation = setTimeout(function () {
            _this._buttons.forEach(function (button, i) {
                var transitionDelay = 0;
                var transform;
                if (_this._parent.animationMode === 'scale') {
                    // Incremental transition delay of 65ms for each action button
                    transitionDelay = 3 + (65 * i);
                    transform = 'scale(1)';
                }
                else {
                    transform = _this.getTranslateFunction('0');
                }
                var hostElement = button._getHostElement();
                _this.changeElementStyle(hostElement, 'transition-delay', transitionDelay + 'ms');
                _this.changeElementStyle(hostElement, 'opacity', '1');
                _this.changeElementStyle(hostElement, 'transform', transform);
            });
        }, 50); // Be sure that *ngIf can show elements before trying to animate them
    };
    EcoFabSpeedDialActionsComponent.prototype.resetAnimationState = function () {
        clearTimeout(this.showMiniFabAnimation);
        if (this.hideMiniFab) {
            this.hideMiniFab.unsubscribe();
            this.hideMiniFab = null;
        }
    };
    EcoFabSpeedDialActionsComponent.prototype.hide = function () {
        var _this = this;
        if (!this._buttons) {
            return;
        }
        this.resetAnimationState();
        var obs = this._buttons.map(function (button, i) {
            var opacity = '1';
            var transitionDelay = 0;
            var transform;
            if (_this._parent.animationMode === 'scale') {
                transitionDelay = 3 - (65 * i);
                transform = 'scale(0)';
                opacity = '0';
            }
            else {
                transform = _this.getTranslateFunction((55 * (i + 1) - (i * 5)) + 'px');
            }
            var hostElement = button._getHostElement();
            _this.changeElementStyle(hostElement, 'transition-delay', transitionDelay + 'ms');
            _this.changeElementStyle(hostElement, 'opacity', opacity);
            _this.changeElementStyle(hostElement, 'transform', transform);
            return Object(rxjs__WEBPACK_IMPORTED_MODULE_4__["fromEvent"])(hostElement, 'transitionend').pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_5__["take"])(1));
        });
        // Wait for all animation to finish, then destroy their elements
        this.hideMiniFab = Object(rxjs__WEBPACK_IMPORTED_MODULE_4__["forkJoin"])(obs).subscribe(function () { return _this.miniFabVisible = false; });
    };
    EcoFabSpeedDialActionsComponent.prototype.getTranslateFunction = function (value) {
        var dir = this._parent.direction;
        var translateFn = (dir === 'up' || dir === 'down') ? 'translateY' : 'translateX';
        var sign = (dir === 'down' || dir === 'right') ? '-' : '';
        return translateFn + '(' + sign + value + ')';
    };
    EcoFabSpeedDialActionsComponent.prototype.changeElementStyle = function (elem, style, value) {
        // FIXME - Find a way to create a "wrapper" around the action button(s) provided by the user, so we don't change it's style tag
        this.renderer.setStyle(elem, style, value);
    };
    EcoFabSpeedDialActionsComponent.ctorParameters = function () { return [
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Injector"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Renderer2"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ContentChildren"])(_angular_material_button__WEBPACK_IMPORTED_MODULE_2__["MatButton"])
    ], EcoFabSpeedDialActionsComponent.prototype, "_buttons", void 0);
EcoFabSpeedDialActionsComponent.ɵfac = function EcoFabSpeedDialActionsComponent_Factory(t) { return new (t || EcoFabSpeedDialActionsComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injector"]), _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_1__["Renderer2"])); };
EcoFabSpeedDialActionsComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: EcoFabSpeedDialActionsComponent, selectors: [["eco-fab-speed-dial-actions"]], contentQueries: function EcoFabSpeedDialActionsComponent_ContentQueries(rf, ctx, dirIndex) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵcontentQuery"](dirIndex, _angular_material_button__WEBPACK_IMPORTED_MODULE_2__["MatButton"], false);
    } if (rf & 2) {
        var _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵloadQuery"]()) && (ctx._buttons = _t);
    } }, ngContentSelectors: _c1, decls: 1, vars: 1, consts: [[4, "ngIf"]], template: function EcoFabSpeedDialActionsComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵprojectionDef"](_c0);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](0, EcoFabSpeedDialActionsComponent_0_Template, 1, 0, undefined, 0);
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx.miniFabVisible);
    } }, directives: [_angular_common__WEBPACK_IMPORTED_MODULE_3__["NgIf"]], encapsulation: 2 });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵsetClassMetadata"](EcoFabSpeedDialActionsComponent, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"],
        args: [{
                selector: 'eco-fab-speed-dial-actions',
                template: "\n        <ng-content select=\"[mat-mini-fab]\" *ngIf=\"miniFabVisible\"></ng-content>"
            }]
    }], function () { return [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Injector"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Renderer2"] }]; }, { _buttons: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ContentChildren"],
            args: [_angular_material_button__WEBPACK_IMPORTED_MODULE_2__["MatButton"]]
        }] }); })();
    return EcoFabSpeedDialActionsComponent;
}());
/** @dynamic @see https://github.com/angular/angular/issues/20351#issuecomment-344009887 */
var EcoFabSpeedDialComponent = /** @class */ (function () {
    function EcoFabSpeedDialComponent(elementRef, renderer, document) {
        this.elementRef = elementRef;
        this.renderer = renderer;
        this.document = document;
        this.isInitialized = false;
        this._direction = 'up';
        this._open = false;
        this._animationMode = 'fling';
        this._fixed = false;
        this._documentClickUnlistener = null;
        this.openChange = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
    }
    Object.defineProperty(EcoFabSpeedDialComponent.prototype, "fixed", {
        /**
         * Whether this speed dial is fixed on screen (user cannot change it by clicking)
         */
        get: function () {
            return this._fixed;
        },
        set: function (fixed) {
            this._fixed = fixed;
            this._processOutsideClickState();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EcoFabSpeedDialComponent.prototype, "open", {
        /**
         * Whether this speed dial is opened
         */
        get: function () {
            return this._open;
        },
        set: function (open) {
            var previousOpen = this._open;
            this._open = open;
            if (previousOpen !== this._open) {
                this.openChange.emit(this._open);
                if (this.isInitialized) {
                    this.setActionsVisibility();
                }
            }
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EcoFabSpeedDialComponent.prototype, "direction", {
        /**
         * The direction of the speed dial. Can be 'up', 'down', 'left' or 'right'
         */
        get: function () {
            return this._direction;
        },
        set: function (direction) {
            var previousDirection = this._direction;
            this._direction = direction;
            if (previousDirection !== this.direction) {
                this._setElementClass(previousDirection, false);
                this._setElementClass(this.direction, true);
                if (this.isInitialized) {
                    this.setActionsVisibility();
                }
            }
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EcoFabSpeedDialComponent.prototype, "animationMode", {
        /**
         * The animation mode to open the speed dial. Can be 'fling' or 'scale'
         */
        get: function () {
            return this._animationMode;
        },
        set: function (animationMode) {
            var _this = this;
            var previousAnimationMode = this._animationMode;
            this._animationMode = animationMode;
            if (previousAnimationMode !== this._animationMode) {
                this._setElementClass(previousAnimationMode, false);
                this._setElementClass(this.animationMode, true);
                if (this.isInitialized) {
                    // To start another detect lifecycle and force the "close" on the action buttons
                    Promise.resolve(null).then(function () { return _this.open = false; });
                }
            }
        },
        enumerable: true,
        configurable: true
    });
    EcoFabSpeedDialComponent.prototype.ngAfterContentInit = function () {
        this.isInitialized = true;
        this.setActionsVisibility();
        this._setElementClass(this.direction, true);
        this._setElementClass(this.animationMode, true);
    };
    EcoFabSpeedDialComponent.prototype.ngOnDestroy = function () {
        this._unsetDocumentClickListener();
    };
    /**
     * Toggle the open state of this speed dial
     */
    EcoFabSpeedDialComponent.prototype.toggle = function () {
        this.open = !this.open;
    };
    EcoFabSpeedDialComponent.prototype._onClick = function () {
        if (!this.fixed && this.open) {
            this.open = false;
        }
    };
    EcoFabSpeedDialComponent.prototype.setActionsVisibility = function () {
        if (!this._childActions) {
            return;
        }
        if (this.open) {
            this._childActions.show();
        }
        else {
            this._childActions.hide();
        }
        this._processOutsideClickState();
    };
    EcoFabSpeedDialComponent.prototype._setElementClass = function (elemClass, isAdd) {
        var finalClass = "eco-" + elemClass;
        if (isAdd) {
            this.renderer.addClass(this.elementRef.nativeElement, finalClass);
        }
        else {
            this.renderer.removeClass(this.elementRef.nativeElement, finalClass);
        }
    };
    EcoFabSpeedDialComponent.prototype._processOutsideClickState = function () {
        if (!this.fixed && this.open) {
            this._setDocumentClickListener();
        }
        else {
            this._unsetDocumentClickListener();
        }
    };
    EcoFabSpeedDialComponent.prototype._setDocumentClickListener = function () {
        var _this = this;
        if (!this._documentClickUnlistener) {
            this._documentClickUnlistener = this.renderer.listen(this.document, 'click', function () {
                _this.open = false;
            });
        }
    };
    EcoFabSpeedDialComponent.prototype._unsetDocumentClickListener = function () {
        if (this._documentClickUnlistener) {
            this._documentClickUnlistener();
            this._documentClickUnlistener = null;
        }
    };
    EcoFabSpeedDialComponent.ctorParameters = function () { return [
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Renderer2"] },
        { type: Document, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Inject"], args: [_angular_common__WEBPACK_IMPORTED_MODULE_3__["DOCUMENT"],] }] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])()
    ], EcoFabSpeedDialComponent.prototype, "fixed", null);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('class.eco-opened'),
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])()
    ], EcoFabSpeedDialComponent.prototype, "open", null);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])()
    ], EcoFabSpeedDialComponent.prototype, "direction", null);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])()
    ], EcoFabSpeedDialComponent.prototype, "animationMode", null);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])()
    ], EcoFabSpeedDialComponent.prototype, "openChange", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ContentChild"])(EcoFabSpeedDialActionsComponent)
    ], EcoFabSpeedDialComponent.prototype, "_childActions", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostListener"])('click')
    ], EcoFabSpeedDialComponent.prototype, "_onClick", null);
    EcoFabSpeedDialComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([ Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__param"])(2, Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Inject"])(_angular_common__WEBPACK_IMPORTED_MODULE_3__["DOCUMENT"]))
    ], EcoFabSpeedDialComponent);
EcoFabSpeedDialComponent.ɵfac = function EcoFabSpeedDialComponent_Factory(t) { return new (t || EcoFabSpeedDialComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_1__["Renderer2"]), _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_common__WEBPACK_IMPORTED_MODULE_3__["DOCUMENT"])); };
EcoFabSpeedDialComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: EcoFabSpeedDialComponent, selectors: [["eco-fab-speed-dial"]], contentQueries: function EcoFabSpeedDialComponent_ContentQueries(rf, ctx, dirIndex) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵcontentQuery"](dirIndex, EcoFabSpeedDialActionsComponent, true);
    } if (rf & 2) {
        var _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵloadQuery"]()) && (ctx._childActions = _t.first);
    } }, hostVars: 2, hostBindings: function EcoFabSpeedDialComponent_HostBindings(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function EcoFabSpeedDialComponent_click_HostBindingHandler() { return ctx._onClick(); });
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵclassProp"]("eco-opened", ctx.open);
    } }, inputs: { fixed: "fixed", open: "open", direction: "direction", animationMode: "animationMode" }, outputs: { openChange: "openChange" }, ngContentSelectors: _c3, decls: 3, vars: 0, consts: [[1, "eco-fab-speed-dial-container"]], template: function EcoFabSpeedDialComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵprojectionDef"](_c2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵprojection"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵprojection"](2, 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    } }, styles: ["eco-fab-speed-dial{display:inline-block}eco-fab-speed-dial.eco-opened .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin180{transform:rotate(180deg)}eco-fab-speed-dial.eco-opened .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin360{transform:rotate(360deg)}eco-fab-speed-dial .eco-fab-speed-dial-container{position:relative;display:flex;align-items:center;z-index:20}eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{pointer-events:auto;z-index:24}eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin180,eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin360{transition:.6s cubic-bezier(.4,0,.2,1)}eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-actions{display:flex;position:absolute;height:0;width:0}eco-fab-speed-dial.eco-fling .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{display:block;opacity:1;transition:.3s cubic-bezier(.55,0,.55,.2)}eco-fab-speed-dial.eco-scale .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{transform:scale(0);transition:.3s cubic-bezier(.55,0,.55,.2);transition-duration:.14286s}eco-fab-speed-dial.eco-down eco-fab-speed-dial-actions{bottom:2px;left:7px}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container{flex-direction:column}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:1}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:column;order:2}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-top:10px}eco-fab-speed-dial.eco-up eco-fab-speed-dial-actions{top:2px;left:7px}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container{flex-direction:column}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:2}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:column-reverse;order:1}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-bottom:10px}eco-fab-speed-dial.eco-left eco-fab-speed-dial-actions{top:7px;left:2px}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container{flex-direction:row}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:2}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:row-reverse;order:1}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-right:10px}eco-fab-speed-dial.eco-right eco-fab-speed-dial-actions{top:7px;right:2px}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container{flex-direction:row}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:1}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:row;order:2}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-left:10px}"], encapsulation: 2 });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵsetClassMetadata"](EcoFabSpeedDialComponent, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"],
        args: [{
                selector: 'eco-fab-speed-dial',
                template: "\n        <div class=\"eco-fab-speed-dial-container\">\n            <ng-content select=\"eco-fab-speed-dial-trigger\"></ng-content>\n            <ng-content select=\"eco-fab-speed-dial-actions\"></ng-content>\n        </div>\n    ",
                encapsulation: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ViewEncapsulation"].None,
                styles: ["eco-fab-speed-dial{display:inline-block}eco-fab-speed-dial.eco-opened .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin180{transform:rotate(180deg)}eco-fab-speed-dial.eco-opened .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin360{transform:rotate(360deg)}eco-fab-speed-dial .eco-fab-speed-dial-container{position:relative;display:flex;align-items:center;z-index:20}eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{pointer-events:auto;z-index:24}eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin180,eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-trigger.eco-spin .spin360{transition:.6s cubic-bezier(.4,0,.2,1)}eco-fab-speed-dial .eco-fab-speed-dial-container eco-fab-speed-dial-actions{display:flex;position:absolute;height:0;width:0}eco-fab-speed-dial.eco-fling .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{display:block;opacity:1;transition:.3s cubic-bezier(.55,0,.55,.2)}eco-fab-speed-dial.eco-scale .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{transform:scale(0);transition:.3s cubic-bezier(.55,0,.55,.2);transition-duration:.14286s}eco-fab-speed-dial.eco-down eco-fab-speed-dial-actions{bottom:2px;left:7px}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container{flex-direction:column}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:1}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:column;order:2}eco-fab-speed-dial.eco-down .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-top:10px}eco-fab-speed-dial.eco-up eco-fab-speed-dial-actions{top:2px;left:7px}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container{flex-direction:column}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:2}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:column-reverse;order:1}eco-fab-speed-dial.eco-up .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-bottom:10px}eco-fab-speed-dial.eco-left eco-fab-speed-dial-actions{top:7px;left:2px}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container{flex-direction:row}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:2}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:row-reverse;order:1}eco-fab-speed-dial.eco-left .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-right:10px}eco-fab-speed-dial.eco-right eco-fab-speed-dial-actions{top:7px;right:2px}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container{flex-direction:row}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container eco-fab-speed-dial-trigger{order:1}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container eco-fab-speed-dial-actions{flex-direction:row;order:2}eco-fab-speed-dial.eco-right .eco-fab-speed-dial-container eco-fab-speed-dial-actions .eco-fab-action-item{margin-left:10px}"]
            }]
    }], function () { return [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Renderer2"] }, { type: Document, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Inject"],
                args: [_angular_common__WEBPACK_IMPORTED_MODULE_3__["DOCUMENT"]]
            }] }]; }, { openChange: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"]
        }], fixed: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"]
        }], open: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"],
            args: ['class.eco-opened']
        }, {
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"]
        }], direction: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"]
        }], animationMode: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"]
        }], _onClick: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["HostListener"],
            args: ['click']
        }], _childActions: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ContentChild"],
            args: [EcoFabSpeedDialActionsComponent]
        }] }); })();
    return EcoFabSpeedDialComponent;
}());
var EcoFabSpeedDialTriggerComponent = /** @class */ (function () {
    function EcoFabSpeedDialTriggerComponent(injector) {
        this.spin = false;
        this._parent = injector.get(EcoFabSpeedDialComponent);
    }
    Object.defineProperty(EcoFabSpeedDialTriggerComponent.prototype, "sp", {
        /**
         * Whether this trigger should spin (360dg) while opening the speed dial
         */
        get: function () {
            return this.spin;
        },
        enumerable: true,
        configurable: true
    });
    EcoFabSpeedDialTriggerComponent.prototype._onClick = function (event) {
        if (!this._parent.fixed) {
            this._parent.toggle();
            event.stopPropagation();
        }
    };
    EcoFabSpeedDialTriggerComponent.ctorParameters = function () { return [
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Injector"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('class.eco-spin')
    ], EcoFabSpeedDialTriggerComponent.prototype, "sp", null);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])()
    ], EcoFabSpeedDialTriggerComponent.prototype, "spin", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostListener"])('click', ['$event'])
    ], EcoFabSpeedDialTriggerComponent.prototype, "_onClick", null);
EcoFabSpeedDialTriggerComponent.ɵfac = function EcoFabSpeedDialTriggerComponent_Factory(t) { return new (t || EcoFabSpeedDialTriggerComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injector"])); };
EcoFabSpeedDialTriggerComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: EcoFabSpeedDialTriggerComponent, selectors: [["eco-fab-speed-dial-trigger"]], hostVars: 2, hostBindings: function EcoFabSpeedDialTriggerComponent_HostBindings(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function EcoFabSpeedDialTriggerComponent_click_HostBindingHandler($event) { return ctx._onClick($event); });
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵclassProp"]("eco-spin", ctx.sp);
    } }, inputs: { spin: "spin" }, ngContentSelectors: _c5, decls: 1, vars: 0, template: function EcoFabSpeedDialTriggerComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵprojectionDef"](_c4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵprojection"](0);
    } }, encapsulation: 2 });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵsetClassMetadata"](EcoFabSpeedDialTriggerComponent, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"],
        args: [{
                selector: 'eco-fab-speed-dial-trigger',
                template: "\n        <ng-content select=\"[mat-fab]\"></ng-content>"
            }]
    }], function () { return [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Injector"] }]; }, { spin: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"]
        }], sp: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"],
            args: ['class.eco-spin']
        }], _onClick: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["HostListener"],
            args: ['click', ['$event']]
        }] }); })();
    return EcoFabSpeedDialTriggerComponent;
}());

var EcoFabSpeedDialModule = /** @class */ (function () {
    function EcoFabSpeedDialModule() {
    }
EcoFabSpeedDialModule.ɵmod = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineNgModule"]({ type: EcoFabSpeedDialModule });
EcoFabSpeedDialModule.ɵinj = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineInjector"]({ factory: function EcoFabSpeedDialModule_Factory(t) { return new (t || EcoFabSpeedDialModule)(); }, imports: [[_angular_common__WEBPACK_IMPORTED_MODULE_3__["CommonModule"]]] });
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵsetNgModuleScope"](EcoFabSpeedDialModule, { declarations: function () { return [EcoFabSpeedDialActionsComponent,
        EcoFabSpeedDialComponent,
        EcoFabSpeedDialTriggerComponent]; }, imports: function () { return [_angular_common__WEBPACK_IMPORTED_MODULE_3__["CommonModule"]]; }, exports: function () { return [EcoFabSpeedDialActionsComponent,
        EcoFabSpeedDialComponent,
        EcoFabSpeedDialTriggerComponent]; } }); })();
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵsetClassMetadata"](EcoFabSpeedDialModule, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"],
        args: [{
                imports: [_angular_common__WEBPACK_IMPORTED_MODULE_3__["CommonModule"]],
                declarations: [
                    EcoFabSpeedDialActionsComponent,
                    EcoFabSpeedDialComponent,
                    EcoFabSpeedDialTriggerComponent,
                ],
                exports: [
                    EcoFabSpeedDialActionsComponent,
                    EcoFabSpeedDialComponent,
                    EcoFabSpeedDialTriggerComponent,
                ]
            }]
    }], function () { return []; }, null); })();
    return EcoFabSpeedDialModule;
}());

/*
 * Public API Surface of fab-speed-dial
 */

/**
 * Generated bundle index. Do not edit.
 */



//# sourceMappingURL=ecodev-fab-speed-dial.js.map

/***/ })

}]);
//# sourceMappingURL=default~app-administration-app-administration-module~apps-management-apps-management-module~content-~33e631e1.js.map