(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["app-administration-app-administration-module"],{

/***/ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/accordion.js":
/*!*****************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/@angular/cdk/__ivy_ngcc__/fesm5/accordion.js ***!
  \*****************************************************************************************************/
/*! exports provided: CdkAccordion, CdkAccordionItem, CdkAccordionModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CdkAccordion", function() { return CdkAccordion; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CdkAccordionItem", function() { return CdkAccordionItem; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CdkAccordionModule", function() { return CdkAccordionModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/cdk/collections */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/collections.js");
/* harmony import */ var _angular_cdk_coercion__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/cdk/coercion */ "../../node_modules/@angular/cdk/fesm5/coercion.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");





/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/** Used to generate unique ID for each accordion. */


var nextId = 0;
/**
 * Directive whose purpose is to manage the expanded state of CdkAccordionItem children.
 */
var CdkAccordion = /** @class */ (function () {
    function CdkAccordion() {
        /** Emits when the state of the accordion changes */
        this._stateChanges = new rxjs__WEBPACK_IMPORTED_MODULE_3__["Subject"]();
        /** Stream that emits true/false when openAll/closeAll is triggered. */
        this._openCloseAllActions = new rxjs__WEBPACK_IMPORTED_MODULE_3__["Subject"]();
        /** A readonly id value to use for unique selection coordination. */
        this.id = "cdk-accordion-" + nextId++;
        this._multi = false;
    }
    Object.defineProperty(CdkAccordion.prototype, "multi", {
        /** Whether the accordion should allow multiple expanded accordion items simultaneously. */
        get: function () { return this._multi; },
        set: function (multi) { this._multi = Object(_angular_cdk_coercion__WEBPACK_IMPORTED_MODULE_2__["coerceBooleanProperty"])(multi); },
        enumerable: true,
        configurable: true
    });
    /** Opens all enabled accordion items in an accordion where multi is enabled. */
    CdkAccordion.prototype.openAll = function () {
        this._openCloseAll(true);
    };
    /** Closes all enabled accordion items in an accordion where multi is enabled. */
    CdkAccordion.prototype.closeAll = function () {
        this._openCloseAll(false);
    };
    CdkAccordion.prototype.ngOnChanges = function (changes) {
        this._stateChanges.next(changes);
    };
    CdkAccordion.prototype.ngOnDestroy = function () {
        this._stateChanges.complete();
    };
    CdkAccordion.prototype._openCloseAll = function (expanded) {
        if (this.multi) {
            this._openCloseAllActions.next(expanded);
        }
    };
    CdkAccordion.propDecorators = {
        multi: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"] }]
    };
CdkAccordion.ɵfac = function CdkAccordion_Factory(t) { return new (t || CdkAccordion)(); };
CdkAccordion.ɵdir = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdefineDirective"]({ type: CdkAccordion, selectors: [["cdk-accordion"], ["", "cdkAccordion", ""]], inputs: { multi: "multi" }, exportAs: ["cdkAccordion"], features: [_angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵNgOnChangesFeature"]] });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵsetClassMetadata"](CdkAccordion, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Directive"],
        args: [{
                selector: 'cdk-accordion, [cdkAccordion]',
                exportAs: 'cdkAccordion'
            }]
    }], function () { return []; }, { multi: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"]
        }] }); })();
    return CdkAccordion;
}());

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/** Used to generate unique ID for each accordion item. */
var nextId$1 = 0;
var ɵ0 = undefined;
/**
 * An basic directive expected to be extended and decorated as a component.  Sets up all
 * events and attributes needed to be managed by a CdkAccordion parent.
 */
var CdkAccordionItem = /** @class */ (function () {
    function CdkAccordionItem(accordion, _changeDetectorRef, _expansionDispatcher) {
        var _this = this;
        this.accordion = accordion;
        this._changeDetectorRef = _changeDetectorRef;
        this._expansionDispatcher = _expansionDispatcher;
        /** Subscription to openAll/closeAll events. */
        this._openCloseAllSubscription = rxjs__WEBPACK_IMPORTED_MODULE_3__["Subscription"].EMPTY;
        /** Event emitted every time the AccordionItem is closed. */
        this.closed = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        /** Event emitted every time the AccordionItem is opened. */
        this.opened = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        /** Event emitted when the AccordionItem is destroyed. */
        this.destroyed = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        /**
         * Emits whenever the expanded state of the accordion changes.
         * Primarily used to facilitate two-way binding.
         * @docs-private
         */
        this.expandedChange = new _angular_core__WEBPACK_IMPORTED_MODULE_0__["EventEmitter"]();
        /** The unique AccordionItem id. */
        this.id = "cdk-accordion-child-" + nextId$1++;
        this._expanded = false;
        this._disabled = false;
        /** Unregister function for _expansionDispatcher. */
        this._removeUniqueSelectionListener = function () { };
        this._removeUniqueSelectionListener =
            _expansionDispatcher.listen(function (id, accordionId) {
                if (_this.accordion && !_this.accordion.multi &&
                    _this.accordion.id === accordionId && _this.id !== id) {
                    _this.expanded = false;
                }
            });
        // When an accordion item is hosted in an accordion, subscribe to open/close events.
        if (this.accordion) {
            this._openCloseAllSubscription = this._subscribeToOpenCloseAllActions();
        }
    }
    Object.defineProperty(CdkAccordionItem.prototype, "expanded", {
        /** Whether the AccordionItem is expanded. */
        get: function () { return this._expanded; },
        set: function (expanded) {
            expanded = Object(_angular_cdk_coercion__WEBPACK_IMPORTED_MODULE_2__["coerceBooleanProperty"])(expanded);
            // Only emit events and update the internal value if the value changes.
            if (this._expanded !== expanded) {
                this._expanded = expanded;
                this.expandedChange.emit(expanded);
                if (expanded) {
                    this.opened.emit();
                    /**
                     * In the unique selection dispatcher, the id parameter is the id of the CdkAccordionItem,
                     * the name value is the id of the accordion.
                     */
                    var accordionId = this.accordion ? this.accordion.id : this.id;
                    this._expansionDispatcher.notify(this.id, accordionId);
                }
                else {
                    this.closed.emit();
                }
                // Ensures that the animation will run when the value is set outside of an `@Input`.
                // This includes cases like the open, close and toggle methods.
                this._changeDetectorRef.markForCheck();
            }
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CdkAccordionItem.prototype, "disabled", {
        /** Whether the AccordionItem is disabled. */
        get: function () { return this._disabled; },
        set: function (disabled) { this._disabled = Object(_angular_cdk_coercion__WEBPACK_IMPORTED_MODULE_2__["coerceBooleanProperty"])(disabled); },
        enumerable: true,
        configurable: true
    });
    /** Emits an event for the accordion item being destroyed. */
    CdkAccordionItem.prototype.ngOnDestroy = function () {
        this.opened.complete();
        this.closed.complete();
        this.destroyed.emit();
        this.destroyed.complete();
        this._removeUniqueSelectionListener();
        this._openCloseAllSubscription.unsubscribe();
    };
    /** Toggles the expanded state of the accordion item. */
    CdkAccordionItem.prototype.toggle = function () {
        if (!this.disabled) {
            this.expanded = !this.expanded;
        }
    };
    /** Sets the expanded state of the accordion item to false. */
    CdkAccordionItem.prototype.close = function () {
        if (!this.disabled) {
            this.expanded = false;
        }
    };
    /** Sets the expanded state of the accordion item to true. */
    CdkAccordionItem.prototype.open = function () {
        if (!this.disabled) {
            this.expanded = true;
        }
    };
    CdkAccordionItem.prototype._subscribeToOpenCloseAllActions = function () {
        var _this = this;
        return this.accordion._openCloseAllActions.subscribe(function (expanded) {
            // Only change expanded state if item is enabled
            if (!_this.disabled) {
                _this.expanded = expanded;
            }
        });
    };
    /** @nocollapse */
    CdkAccordionItem.ctorParameters = function () { return [
        { type: CdkAccordion, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Optional"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["SkipSelf"] }] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ChangeDetectorRef"] },
        { type: _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_1__["UniqueSelectionDispatcher"] }
    ]; };
    CdkAccordionItem.propDecorators = {
        closed: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"] }],
        opened: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"] }],
        destroyed: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"] }],
        expandedChange: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"] }],
        expanded: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"] }],
        disabled: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"] }]
    };
CdkAccordionItem.ɵfac = function CdkAccordionItem_Factory(t) { return new (t || CdkAccordionItem)(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdirectiveInject"](CdkAccordion, 12), _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_0__["ChangeDetectorRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdirectiveInject"](_angular_cdk_collections__WEBPACK_IMPORTED_MODULE_1__["UniqueSelectionDispatcher"])); };
CdkAccordionItem.ɵdir = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdefineDirective"]({ type: CdkAccordionItem, selectors: [["cdk-accordion-item"], ["", "cdkAccordionItem", ""]], inputs: { expanded: "expanded", disabled: "disabled" }, outputs: { closed: "closed", opened: "opened", destroyed: "destroyed", expandedChange: "expandedChange" }, exportAs: ["cdkAccordionItem"], features: [_angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵProvidersFeature"]([
            // Provide CdkAccordion as undefined to prevent nested accordion items from registering
            // to the same accordion.
            { provide: CdkAccordion, useValue: ɵ0 },
        ])] });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵsetClassMetadata"](CdkAccordionItem, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Directive"],
        args: [{
                selector: 'cdk-accordion-item, [cdkAccordionItem]',
                exportAs: 'cdkAccordionItem',
                providers: [
                    // Provide CdkAccordion as undefined to prevent nested accordion items from registering
                    // to the same accordion.
                    { provide: CdkAccordion, useValue: ɵ0 },
                ]
            }]
    }], function () { return [{ type: CdkAccordion, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Optional"]
            }, {
                type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["SkipSelf"]
            }] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["ChangeDetectorRef"] }, { type: _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_1__["UniqueSelectionDispatcher"] }]; }, { closed: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"]
        }], opened: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"]
        }], destroyed: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"]
        }], expandedChange: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Output"]
        }], expanded: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"]
        }], disabled: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"]
        }] }); })();
    return CdkAccordionItem;
}());

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
var CdkAccordionModule = /** @class */ (function () {
    function CdkAccordionModule() {
    }
CdkAccordionModule.ɵmod = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdefineNgModule"]({ type: CdkAccordionModule });
CdkAccordionModule.ɵinj = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdefineInjector"]({ factory: function CdkAccordionModule_Factory(t) { return new (t || CdkAccordionModule)(); } });
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵsetNgModuleScope"](CdkAccordionModule, { declarations: [CdkAccordion,
        CdkAccordionItem], exports: [CdkAccordion,
        CdkAccordionItem] }); })();
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵsetClassMetadata"](CdkAccordionModule, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"],
        args: [{
                exports: [CdkAccordion, CdkAccordionItem],
                declarations: [CdkAccordion, CdkAccordionItem]
            }]
    }], function () { return []; }, null); })();
    return CdkAccordionModule;
}());

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

/**
 * Generated bundle index. Do not edit.
 */



//# sourceMappingURL=accordion.js.map

/***/ }),

/***/ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/expansion.js":
/*!**********************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/@angular/material/__ivy_ngcc__/fesm5/expansion.js ***!
  \**********************************************************************************************************/
/*! exports provided: EXPANSION_PANEL_ANIMATION_TIMING, MAT_ACCORDION, MAT_EXPANSION_PANEL_DEFAULT_OPTIONS, MatAccordion, MatExpansionModule, MatExpansionPanel, MatExpansionPanelActionRow, MatExpansionPanelContent, MatExpansionPanelDescription, MatExpansionPanelHeader, MatExpansionPanelTitle, matExpansionAnimations, ɵ0 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EXPANSION_PANEL_ANIMATION_TIMING", function() { return EXPANSION_PANEL_ANIMATION_TIMING; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MAT_ACCORDION", function() { return MAT_ACCORDION; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MAT_EXPANSION_PANEL_DEFAULT_OPTIONS", function() { return MAT_EXPANSION_PANEL_DEFAULT_OPTIONS; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatAccordion", function() { return MatAccordion; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatExpansionModule", function() { return MatExpansionModule; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatExpansionPanel", function() { return MatExpansionPanel; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatExpansionPanelActionRow", function() { return MatExpansionPanelActionRow; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatExpansionPanelContent", function() { return MatExpansionPanelContent; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatExpansionPanelDescription", function() { return MatExpansionPanelDescription; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatExpansionPanelHeader", function() { return MatExpansionPanelHeader; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MatExpansionPanelTitle", function() { return MatExpansionPanelTitle; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "matExpansionAnimations", function() { return matExpansionAnimations; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ɵ0", function() { return ɵ0; });
/* harmony import */ var _angular_cdk_accordion__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/cdk/accordion */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/accordion.js");
/* harmony import */ var _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/cdk/portal */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/portal.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_cdk_coercion__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/cdk/coercion */ "../../node_modules/@angular/cdk/fesm5/coercion.js");
/* harmony import */ var _angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/cdk/a11y */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/a11y.js");
/* harmony import */ var _angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/cdk/keycodes */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/keycodes.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var _angular_animations__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/animations */ "../../node_modules/@angular/animations/__ivy_ngcc__/fesm5/animations.js");
/* harmony import */ var _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/cdk/collections */ "../../node_modules/@angular/cdk/__ivy_ngcc__/fesm5/collections.js");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/platform-browser/animations */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/animations.js");














/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/**
 * Token used to provide a `MatAccordion` to `MatExpansionPanel`.
 * Used primarily to avoid circular imports between `MatAccordion` and `MatExpansionPanel`.
 */






var _c0 = ["body"];
function MatExpansionPanel_ng_template_5_Template(rf, ctx) { }
var _c1 = [[["mat-expansion-panel-header"]], "*", [["mat-action-row"]]];
var _c2 = ["mat-expansion-panel-header", "*", "mat-action-row"];
var _c3 = function (a0, a1) { return { collapsedHeight: a0, expandedHeight: a1 }; };
var _c4 = function (a0, a1) { return { value: a0, params: a1 }; };
function MatExpansionPanelHeader_span_4_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵelement"](0, "span", 2);
} if (rf & 2) {
    var ctx_r81 = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵproperty"]("@indicatorRotate", ctx_r81._getExpandedState());
} }
var _c5 = [[["mat-panel-title"]], [["mat-panel-description"]], "*"];
var _c6 = ["mat-panel-title", "mat-panel-description", "*"];
var MAT_ACCORDION = new _angular_core__WEBPACK_IMPORTED_MODULE_3__["InjectionToken"]('MAT_ACCORDION');

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/** Time and timing curve for expansion panel animations. */
var EXPANSION_PANEL_ANIMATION_TIMING = '225ms cubic-bezier(0.4,0.0,0.2,1)';
/**
 * Animations used by the Material expansion panel.
 *
 * A bug in angular animation's `state` when ViewContainers are moved using ViewContainerRef.move()
 * causes the animation state of moved components to become `void` upon exit, and not update again
 * upon reentry into the DOM.  This can lead a to situation for the expansion panel where the state
 * of the panel is `expanded` or `collapsed` but the animation state is `void`.
 *
 * To correctly handle animating to the next state, we animate between `void` and `collapsed` which
 * are defined to have the same styles. Since angular animates from the current styles to the
 * destination state's style definition, in situations where we are moving from `void`'s styles to
 * `collapsed` this acts a noop since no style values change.
 *
 * In the case where angular's animation state is out of sync with the expansion panel's state, the
 * expansion panel being `expanded` and angular animations being `void`, the animation from the
 * `expanded`'s effective styles (though in a `void` animation state) to the collapsed state will
 * occur as expected.
 *
 * Angular Bug: https://github.com/angular/angular/issues/18847
 *
 * @docs-private
 */
var matExpansionAnimations = {
    /** Animation that rotates the indicator arrow. */
    indicatorRotate: Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["trigger"])('indicatorRotate', [
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["state"])('collapsed, void', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["style"])({ transform: 'rotate(0deg)' })),
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["state"])('expanded', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["style"])({ transform: 'rotate(180deg)' })),
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["transition"])('expanded <=> collapsed, void => collapsed', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["animate"])(EXPANSION_PANEL_ANIMATION_TIMING)),
    ]),
    /** Animation that expands and collapses the panel header height. */
    expansionHeaderHeight: Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["trigger"])('expansionHeight', [
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["state"])('collapsed, void', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["style"])({
            height: '{{collapsedHeight}}',
        }), {
            params: { collapsedHeight: '48px' },
        }),
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["state"])('expanded', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["style"])({
            height: '{{expandedHeight}}'
        }), {
            params: { expandedHeight: '64px' }
        }),
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["transition"])('expanded <=> collapsed, void => collapsed', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["group"])([
            Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["query"])('@indicatorRotate', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["animateChild"])(), { optional: true }),
            Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["animate"])(EXPANSION_PANEL_ANIMATION_TIMING),
        ])),
    ]),
    /** Animation that expands and collapses the panel content. */
    bodyExpansion: Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["trigger"])('bodyExpansion', [
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["state"])('collapsed, void', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["style"])({ height: '0px', visibility: 'hidden' })),
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["state"])('expanded', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["style"])({ height: '*', visibility: 'visible' })),
        Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["transition"])('expanded <=> collapsed, void => collapsed', Object(_angular_animations__WEBPACK_IMPORTED_MODULE_10__["animate"])(EXPANSION_PANEL_ANIMATION_TIMING)),
    ])
};

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/**
 * Expansion panel content that will be rendered lazily
 * after the panel is opened for the first time.
 */
var MatExpansionPanelContent = /** @class */ (function () {
    function MatExpansionPanelContent(_template) {
        this._template = _template;
    }
    /** @nocollapse */
    MatExpansionPanelContent.ctorParameters = function () { return [
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["TemplateRef"] }
    ]; };
MatExpansionPanelContent.ɵfac = function MatExpansionPanelContent_Factory(t) { return new (t || MatExpansionPanelContent)(_angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_3__["TemplateRef"])); };
MatExpansionPanelContent.ɵdir = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineDirective"]({ type: MatExpansionPanelContent, selectors: [["ng-template", "matExpansionPanelContent", ""]] });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatExpansionPanelContent, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Directive"],
        args: [{
                selector: 'ng-template[matExpansionPanelContent]'
            }]
    }], function () { return [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["TemplateRef"] }]; }, null); })();
    return MatExpansionPanelContent;
}());

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/** Counter for generating unique element ids. */
var uniqueId = 0;
/**
 * Injection token that can be used to configure the defalt
 * options for the expansion panel component.
 */
var MAT_EXPANSION_PANEL_DEFAULT_OPTIONS = new _angular_core__WEBPACK_IMPORTED_MODULE_3__["InjectionToken"]('MAT_EXPANSION_PANEL_DEFAULT_OPTIONS');
var ɵ0 = undefined;
/**
 * `<mat-expansion-panel>`
 *
 * This component can be used as a single element to show expandable content, or as one of
 * multiple children of an element with the MatAccordion directive attached.
 */
var MatExpansionPanel = /** @class */ (function (_super) {
    Object(tslib__WEBPACK_IMPORTED_MODULE_4__["__extends"])(MatExpansionPanel, _super);
    function MatExpansionPanel(accordion, _changeDetectorRef, _uniqueSelectionDispatcher, _viewContainerRef, _document, _animationMode, defaultOptions) {
        var _this = _super.call(this, accordion, _changeDetectorRef, _uniqueSelectionDispatcher) || this;
        _this._viewContainerRef = _viewContainerRef;
        _this._animationMode = _animationMode;
        _this._hideToggle = false;
        /** An event emitted after the body's expansion animation happens. */
        _this.afterExpand = new _angular_core__WEBPACK_IMPORTED_MODULE_3__["EventEmitter"]();
        /** An event emitted after the body's collapse animation happens. */
        _this.afterCollapse = new _angular_core__WEBPACK_IMPORTED_MODULE_3__["EventEmitter"]();
        /** Stream that emits for changes in `@Input` properties. */
        _this._inputChanges = new rxjs__WEBPACK_IMPORTED_MODULE_9__["Subject"]();
        /** ID for the associated header element. Used for a11y labelling. */
        _this._headerId = "mat-expansion-panel-header-" + uniqueId++;
        /** Stream of body animation done events. */
        _this._bodyAnimationDone = new rxjs__WEBPACK_IMPORTED_MODULE_9__["Subject"]();
        _this.accordion = accordion;
        _this._document = _document;
        // We need a Subject with distinctUntilChanged, because the `done` event
        // fires twice on some browsers. See https://github.com/angular/angular/issues/24084
        _this._bodyAnimationDone.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["distinctUntilChanged"])(function (x, y) {
            return x.fromState === y.fromState && x.toState === y.toState;
        })).subscribe(function (event) {
            if (event.fromState !== 'void') {
                if (event.toState === 'expanded') {
                    _this.afterExpand.emit();
                }
                else if (event.toState === 'collapsed') {
                    _this.afterCollapse.emit();
                }
            }
        });
        if (defaultOptions) {
            _this.hideToggle = defaultOptions.hideToggle;
        }
        return _this;
    }
    Object.defineProperty(MatExpansionPanel.prototype, "hideToggle", {
        /** Whether the toggle indicator should be hidden. */
        get: function () {
            return this._hideToggle || (this.accordion && this.accordion.hideToggle);
        },
        set: function (value) {
            this._hideToggle = Object(_angular_cdk_coercion__WEBPACK_IMPORTED_MODULE_5__["coerceBooleanProperty"])(value);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(MatExpansionPanel.prototype, "togglePosition", {
        /** The position of the expansion indicator. */
        get: function () {
            return this._togglePosition || (this.accordion && this.accordion.togglePosition);
        },
        set: function (value) {
            this._togglePosition = value;
        },
        enumerable: true,
        configurable: true
    });
    /** Determines whether the expansion panel should have spacing between it and its siblings. */
    MatExpansionPanel.prototype._hasSpacing = function () {
        if (this.accordion) {
            return this.expanded && this.accordion.displayMode === 'default';
        }
        return false;
    };
    /** Gets the expanded state string. */
    MatExpansionPanel.prototype._getExpandedState = function () {
        return this.expanded ? 'expanded' : 'collapsed';
    };
    /** Toggles the expanded state of the expansion panel. */
    MatExpansionPanel.prototype.toggle = function () {
        this.expanded = !this.expanded;
    };
    /** Sets the expanded state of the expansion panel to false. */
    MatExpansionPanel.prototype.close = function () {
        this.expanded = false;
    };
    /** Sets the expanded state of the expansion panel to true. */
    MatExpansionPanel.prototype.open = function () {
        this.expanded = true;
    };
    MatExpansionPanel.prototype.ngAfterContentInit = function () {
        var _this = this;
        if (this._lazyContent) {
            // Render the content as soon as the panel becomes open.
            this.opened.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["startWith"])(null), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["filter"])(function () { return _this.expanded && !_this._portal; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["take"])(1)).subscribe(function () {
                _this._portal = new _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_1__["TemplatePortal"](_this._lazyContent._template, _this._viewContainerRef);
            });
        }
    };
    MatExpansionPanel.prototype.ngOnChanges = function (changes) {
        this._inputChanges.next(changes);
    };
    MatExpansionPanel.prototype.ngOnDestroy = function () {
        _super.prototype.ngOnDestroy.call(this);
        this._bodyAnimationDone.complete();
        this._inputChanges.complete();
    };
    /** Checks whether the expansion panel's content contains the currently-focused element. */
    MatExpansionPanel.prototype._containsFocus = function () {
        if (this._body) {
            var focusedElement = this._document.activeElement;
            var bodyElement = this._body.nativeElement;
            return focusedElement === bodyElement || bodyElement.contains(focusedElement);
        }
        return false;
    };
    /** @nocollapse */
    MatExpansionPanel.ctorParameters = function () { return [
        { type: undefined, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["SkipSelf"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"], args: [MAT_ACCORDION,] }] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectorRef"] },
        { type: _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_11__["UniqueSelectionDispatcher"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ViewContainerRef"] },
        { type: undefined, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"], args: [_angular_common__WEBPACK_IMPORTED_MODULE_2__["DOCUMENT"],] }] },
        { type: String, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"], args: [_angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_12__["ANIMATION_MODULE_TYPE"],] }] },
        { type: undefined, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"], args: [MAT_EXPANSION_PANEL_DEFAULT_OPTIONS,] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"] }] }
    ]; };
    MatExpansionPanel.propDecorators = {
        hideToggle: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"] }],
        togglePosition: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"] }],
        afterExpand: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Output"] }],
        afterCollapse: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Output"] }],
        _lazyContent: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ContentChild"], args: [MatExpansionPanelContent,] }],
        _body: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ViewChild"], args: ['body',] }]
    };
MatExpansionPanel.ɵfac = function MatExpansionPanel_Factory(t) { return new (t || MatExpansionPanel)(_angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](MAT_ACCORDION, 12), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectorRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_cdk_collections__WEBPACK_IMPORTED_MODULE_11__["UniqueSelectionDispatcher"]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_3__["ViewContainerRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_common__WEBPACK_IMPORTED_MODULE_2__["DOCUMENT"]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_12__["ANIMATION_MODULE_TYPE"], 8), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](MAT_EXPANSION_PANEL_DEFAULT_OPTIONS, 8)); };
MatExpansionPanel.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineComponent"]({ type: MatExpansionPanel, selectors: [["mat-expansion-panel"]], contentQueries: function MatExpansionPanel_ContentQueries(rf, ctx, dirIndex) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵcontentQuery"](dirIndex, MatExpansionPanelContent, true);
    } if (rf & 2) {
        var _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵloadQuery"]()) && (ctx._lazyContent = _t.first);
    } }, viewQuery: function MatExpansionPanel_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵviewQuery"](_c0, true);
    } if (rf & 2) {
        var _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵloadQuery"]()) && (ctx._body = _t.first);
    } }, hostAttrs: [1, "mat-expansion-panel"], hostVars: 6, hostBindings: function MatExpansionPanel_HostBindings(rf, ctx) { if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵclassProp"]("mat-expanded", ctx.expanded)("_mat-animation-noopable", ctx._animationMode === "NoopAnimations")("mat-expansion-panel-spacing", ctx._hasSpacing());
    } }, inputs: { disabled: "disabled", expanded: "expanded", hideToggle: "hideToggle", togglePosition: "togglePosition" }, outputs: { opened: "opened", closed: "closed", expandedChange: "expandedChange", afterExpand: "afterExpand", afterCollapse: "afterCollapse" }, exportAs: ["matExpansionPanel"], features: [_angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵProvidersFeature"]([
            // Provide MatAccordion as undefined to prevent nested expansion panels from registering
            // to the same accordion.
            { provide: MAT_ACCORDION, useValue: ɵ0 },
        ]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵInheritDefinitionFeature"], _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵNgOnChangesFeature"]], ngContentSelectors: _c2, decls: 7, vars: 4, consts: [["role", "region", 1, "mat-expansion-panel-content", 3, "id"], ["body", ""], [1, "mat-expansion-panel-body"], [3, "cdkPortalOutlet"]], template: function MatExpansionPanel_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojectionDef"](_c1);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojection"](0);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵelementStart"](1, "div", 0, 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵlistener"]("@bodyExpansion.done", function MatExpansionPanel_Template_div_animation_bodyExpansion_done_1_listener($event) { return ctx._bodyAnimationDone.next($event); });
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵelementStart"](3, "div", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojection"](4, 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵtemplate"](5, MatExpansionPanel_ng_template_5_Template, 0, 0, "ng-template", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojection"](6, 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵproperty"]("@bodyExpansion", ctx._getExpandedState())("id", ctx.id);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵattribute"]("aria-labelledby", ctx._headerId);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵproperty"]("cdkPortalOutlet", ctx._portal);
    } }, directives: [_angular_cdk_portal__WEBPACK_IMPORTED_MODULE_1__["CdkPortalOutlet"]], styles: [".mat-expansion-panel{box-sizing:content-box;display:block;margin:0;border-radius:4px;overflow:hidden;transition:margin 225ms cubic-bezier(0.4, 0, 0.2, 1),box-shadow 280ms cubic-bezier(0.4, 0, 0.2, 1)}.mat-accordion .mat-expansion-panel:not(.mat-expanded),.mat-accordion .mat-expansion-panel:not(.mat-expansion-panel-spacing){border-radius:0}.mat-accordion .mat-expansion-panel:first-of-type{border-top-right-radius:4px;border-top-left-radius:4px}.mat-accordion .mat-expansion-panel:last-of-type{border-bottom-right-radius:4px;border-bottom-left-radius:4px}.cdk-high-contrast-active .mat-expansion-panel{outline:solid 1px}.mat-expansion-panel.ng-animate-disabled,.ng-animate-disabled .mat-expansion-panel,.mat-expansion-panel._mat-animation-noopable{transition:none}.mat-expansion-panel-content{display:flex;flex-direction:column;overflow:visible}.mat-expansion-panel-body{padding:0 24px 16px}.mat-expansion-panel-spacing{margin:16px 0}.mat-accordion>.mat-expansion-panel-spacing:first-child,.mat-accordion>*:first-child:not(.mat-expansion-panel) .mat-expansion-panel-spacing{margin-top:0}.mat-accordion>.mat-expansion-panel-spacing:last-child,.mat-accordion>*:last-child:not(.mat-expansion-panel) .mat-expansion-panel-spacing{margin-bottom:0}.mat-action-row{border-top-style:solid;border-top-width:1px;display:flex;flex-direction:row;justify-content:flex-end;padding:16px 8px 16px 24px}.mat-action-row button.mat-button-base,.mat-action-row button.mat-mdc-button-base{margin-left:8px}[dir=rtl] .mat-action-row button.mat-button-base,[dir=rtl] .mat-action-row button.mat-mdc-button-base{margin-left:0;margin-right:8px}\n"], encapsulation: 2, data: { animation: [matExpansionAnimations.bodyExpansion] }, changeDetection: 0 });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatExpansionPanel, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Component"],
        args: [{
                selector: 'mat-expansion-panel',
                exportAs: 'matExpansionPanel',
                template: "<ng-content select=\"mat-expansion-panel-header\"></ng-content>\n<div class=\"mat-expansion-panel-content\"\n     role=\"region\"\n     [@bodyExpansion]=\"_getExpandedState()\"\n     (@bodyExpansion.done)=\"_bodyAnimationDone.next($event)\"\n     [attr.aria-labelledby]=\"_headerId\"\n     [id]=\"id\"\n     #body>\n  <div class=\"mat-expansion-panel-body\">\n    <ng-content></ng-content>\n    <ng-template [cdkPortalOutlet]=\"_portal\"></ng-template>\n  </div>\n  <ng-content select=\"mat-action-row\"></ng-content>\n</div>\n",
                encapsulation: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ViewEncapsulation"].None,
                changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectionStrategy"].OnPush,
                inputs: ['disabled', 'expanded'],
                outputs: ['opened', 'closed', 'expandedChange'],
                animations: [matExpansionAnimations.bodyExpansion],
                providers: [
                    // Provide MatAccordion as undefined to prevent nested expansion panels from registering
                    // to the same accordion.
                    { provide: MAT_ACCORDION, useValue: ɵ0 },
                ],
                host: {
                    'class': 'mat-expansion-panel',
                    '[class.mat-expanded]': 'expanded',
                    '[class._mat-animation-noopable]': '_animationMode === "NoopAnimations"',
                    '[class.mat-expansion-panel-spacing]': '_hasSpacing()'
                },
                styles: [".mat-expansion-panel{box-sizing:content-box;display:block;margin:0;border-radius:4px;overflow:hidden;transition:margin 225ms cubic-bezier(0.4, 0, 0.2, 1),box-shadow 280ms cubic-bezier(0.4, 0, 0.2, 1)}.mat-accordion .mat-expansion-panel:not(.mat-expanded),.mat-accordion .mat-expansion-panel:not(.mat-expansion-panel-spacing){border-radius:0}.mat-accordion .mat-expansion-panel:first-of-type{border-top-right-radius:4px;border-top-left-radius:4px}.mat-accordion .mat-expansion-panel:last-of-type{border-bottom-right-radius:4px;border-bottom-left-radius:4px}.cdk-high-contrast-active .mat-expansion-panel{outline:solid 1px}.mat-expansion-panel.ng-animate-disabled,.ng-animate-disabled .mat-expansion-panel,.mat-expansion-panel._mat-animation-noopable{transition:none}.mat-expansion-panel-content{display:flex;flex-direction:column;overflow:visible}.mat-expansion-panel-body{padding:0 24px 16px}.mat-expansion-panel-spacing{margin:16px 0}.mat-accordion>.mat-expansion-panel-spacing:first-child,.mat-accordion>*:first-child:not(.mat-expansion-panel) .mat-expansion-panel-spacing{margin-top:0}.mat-accordion>.mat-expansion-panel-spacing:last-child,.mat-accordion>*:last-child:not(.mat-expansion-panel) .mat-expansion-panel-spacing{margin-bottom:0}.mat-action-row{border-top-style:solid;border-top-width:1px;display:flex;flex-direction:row;justify-content:flex-end;padding:16px 8px 16px 24px}.mat-action-row button.mat-button-base,.mat-action-row button.mat-mdc-button-base{margin-left:8px}[dir=rtl] .mat-action-row button.mat-button-base,[dir=rtl] .mat-action-row button.mat-mdc-button-base{margin-left:0;margin-right:8px}\n"]
            }]
    }], function () { return [{ type: undefined, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"]
            }, {
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["SkipSelf"]
            }, {
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"],
                args: [MAT_ACCORDION]
            }] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectorRef"] }, { type: _angular_cdk_collections__WEBPACK_IMPORTED_MODULE_11__["UniqueSelectionDispatcher"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ViewContainerRef"] }, { type: undefined, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"],
                args: [_angular_common__WEBPACK_IMPORTED_MODULE_2__["DOCUMENT"]]
            }] }, { type: String, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"]
            }, {
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"],
                args: [_angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_12__["ANIMATION_MODULE_TYPE"]]
            }] }, { type: undefined, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"],
                args: [MAT_EXPANSION_PANEL_DEFAULT_OPTIONS]
            }, {
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"]
            }] }]; }, { hideToggle: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"]
        }], togglePosition: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"]
        }], afterExpand: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Output"]
        }], afterCollapse: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Output"]
        }], _lazyContent: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ContentChild"],
            args: [MatExpansionPanelContent]
        }], _body: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ViewChild"],
            args: ['body']
        }] }); })();
    return MatExpansionPanel;
}(_angular_cdk_accordion__WEBPACK_IMPORTED_MODULE_0__["CdkAccordionItem"]));
var MatExpansionPanelActionRow = /** @class */ (function () {
    function MatExpansionPanelActionRow() {
    }
MatExpansionPanelActionRow.ɵfac = function MatExpansionPanelActionRow_Factory(t) { return new (t || MatExpansionPanelActionRow)(); };
MatExpansionPanelActionRow.ɵdir = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineDirective"]({ type: MatExpansionPanelActionRow, selectors: [["mat-action-row"]], hostAttrs: [1, "mat-action-row"] });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatExpansionPanelActionRow, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Directive"],
        args: [{
                selector: 'mat-action-row',
                host: {
                    class: 'mat-action-row'
                }
            }]
    }], function () { return []; }, null); })();
    return MatExpansionPanelActionRow;
}());

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/**
 * `<mat-expansion-panel-header>`
 *
 * This component corresponds to the header element of an `<mat-expansion-panel>`.
 */
var MatExpansionPanelHeader = /** @class */ (function () {
    function MatExpansionPanelHeader(panel, _element, _focusMonitor, _changeDetectorRef, defaultOptions) {
        var _this = this;
        this.panel = panel;
        this._element = _element;
        this._focusMonitor = _focusMonitor;
        this._changeDetectorRef = _changeDetectorRef;
        this._parentChangeSubscription = rxjs__WEBPACK_IMPORTED_MODULE_9__["Subscription"].EMPTY;
        /** Whether Angular animations in the panel header should be disabled. */
        this._animationsDisabled = true;
        var accordionHideToggleChange = panel.accordion ?
            panel.accordion._stateChanges.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["filter"])(function (changes) { return !!(changes['hideToggle'] || changes['togglePosition']); })) :
            rxjs__WEBPACK_IMPORTED_MODULE_9__["EMPTY"];
        // Since the toggle state depends on an @Input on the panel, we
        // need to subscribe and trigger change detection manually.
        this._parentChangeSubscription =
            Object(rxjs__WEBPACK_IMPORTED_MODULE_9__["merge"])(panel.opened, panel.closed, accordionHideToggleChange, panel._inputChanges.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["filter"])(function (changes) {
                return !!(changes['hideToggle'] ||
                    changes['disabled'] ||
                    changes['togglePosition']);
            })))
                .subscribe(function () { return _this._changeDetectorRef.markForCheck(); });
        // Avoids focus being lost if the panel contained the focused element and was closed.
        panel.closed
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["filter"])(function () { return panel._containsFocus(); }))
            .subscribe(function () { return _focusMonitor.focusVia(_element, 'program'); });
        _focusMonitor.monitor(_element).subscribe(function (origin) {
            if (origin && panel.accordion) {
                panel.accordion._handleHeaderFocus(_this);
            }
        });
        if (defaultOptions) {
            this.expandedHeight = defaultOptions.expandedHeight;
            this.collapsedHeight = defaultOptions.collapsedHeight;
        }
    }
    MatExpansionPanelHeader.prototype._animationStarted = function () {
        // Currently the `expansionHeight` animation has a `void => collapsed` transition which is
        // there to work around a bug in Angular (see #13088), however this introduces a different
        // issue. The new transition will cause the header to animate in on init (see #16067), if the
        // consumer has set a header height that is different from the default one. We work around it
        // by disabling animations on the header and re-enabling them after the first animation has run.
        // Note that Angular dispatches animation events even if animations are disabled. Ideally this
        // wouldn't be necessary if we remove the `void => collapsed` transition, but we have to wait
        // for https://github.com/angular/angular/issues/18847 to be resolved.
        this._animationsDisabled = false;
    };
    Object.defineProperty(MatExpansionPanelHeader.prototype, "disabled", {
        /**
         * Whether the associated panel is disabled. Implemented as a part of `FocusableOption`.
         * @docs-private
         */
        get: function () {
            return this.panel.disabled;
        },
        enumerable: true,
        configurable: true
    });
    /** Toggles the expanded state of the panel. */
    MatExpansionPanelHeader.prototype._toggle = function () {
        if (!this.disabled) {
            this.panel.toggle();
        }
    };
    /** Gets whether the panel is expanded. */
    MatExpansionPanelHeader.prototype._isExpanded = function () {
        return this.panel.expanded;
    };
    /** Gets the expanded state string of the panel. */
    MatExpansionPanelHeader.prototype._getExpandedState = function () {
        return this.panel._getExpandedState();
    };
    /** Gets the panel id. */
    MatExpansionPanelHeader.prototype._getPanelId = function () {
        return this.panel.id;
    };
    /** Gets the toggle position for the header. */
    MatExpansionPanelHeader.prototype._getTogglePosition = function () {
        return this.panel.togglePosition;
    };
    /** Gets whether the expand indicator should be shown. */
    MatExpansionPanelHeader.prototype._showToggle = function () {
        return !this.panel.hideToggle && !this.panel.disabled;
    };
    /** Handle keydown event calling to toggle() if appropriate. */
    MatExpansionPanelHeader.prototype._keydown = function (event) {
        switch (event.keyCode) {
            // Toggle for space and enter keys.
            case _angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__["SPACE"]:
            case _angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__["ENTER"]:
                if (!Object(_angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__["hasModifierKey"])(event)) {
                    event.preventDefault();
                    this._toggle();
                }
                break;
            default:
                if (this.panel.accordion) {
                    this.panel.accordion._handleHeaderKeydown(event);
                }
                return;
        }
    };
    /**
     * Focuses the panel header. Implemented as a part of `FocusableOption`.
     * @param origin Origin of the action that triggered the focus.
     * @docs-private
     */
    MatExpansionPanelHeader.prototype.focus = function (origin, options) {
        if (origin === void 0) { origin = 'program'; }
        this._focusMonitor.focusVia(this._element, origin, options);
    };
    MatExpansionPanelHeader.prototype.ngOnDestroy = function () {
        this._parentChangeSubscription.unsubscribe();
        this._focusMonitor.stopMonitoring(this._element);
    };
    /** @nocollapse */
    MatExpansionPanelHeader.ctorParameters = function () { return [
        { type: MatExpansionPanel, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Host"] }] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ElementRef"] },
        { type: _angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_6__["FocusMonitor"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectorRef"] },
        { type: undefined, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"], args: [MAT_EXPANSION_PANEL_DEFAULT_OPTIONS,] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"] }] }
    ]; };
    MatExpansionPanelHeader.propDecorators = {
        expandedHeight: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"] }],
        collapsedHeight: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"] }]
    };
MatExpansionPanelHeader.ɵfac = function MatExpansionPanelHeader_Factory(t) { return new (t || MatExpansionPanelHeader)(_angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](MatExpansionPanel, 1), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_3__["ElementRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_6__["FocusMonitor"]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](_angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectorRef"]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdirectiveInject"](MAT_EXPANSION_PANEL_DEFAULT_OPTIONS, 8)); };
MatExpansionPanelHeader.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineComponent"]({ type: MatExpansionPanelHeader, selectors: [["mat-expansion-panel-header"]], hostAttrs: ["role", "button", 1, "mat-expansion-panel-header"], hostVars: 19, hostBindings: function MatExpansionPanelHeader_HostBindings(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵcomponentHostSyntheticListener"]("@expansionHeight.start", function MatExpansionPanelHeader_animation_expansionHeight_start_HostBindingHandler() { return ctx._animationStarted(); });
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵlistener"]("click", function MatExpansionPanelHeader_click_HostBindingHandler() { return ctx._toggle(); })("keydown", function MatExpansionPanelHeader_keydown_HostBindingHandler($event) { return ctx._keydown($event); });
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵattribute"]("id", ctx.panel._headerId)("tabindex", ctx.disabled ? 0 - 1 : 0)("aria-controls", ctx._getPanelId())("aria-expanded", ctx._isExpanded())("aria-disabled", ctx.panel.disabled);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵupdateSyntheticHostBinding"]("@.disabled", ctx._animationsDisabled)("@expansionHeight", _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵpureFunction2"](16, _c4, ctx._getExpandedState(), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵpureFunction2"](13, _c3, ctx.collapsedHeight, ctx.expandedHeight)));
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵclassProp"]("mat-expanded", ctx._isExpanded())("mat-expansion-toggle-indicator-after", ctx._getTogglePosition() === "after")("mat-expansion-toggle-indicator-before", ctx._getTogglePosition() === "before");
    } }, inputs: { expandedHeight: "expandedHeight", collapsedHeight: "collapsedHeight" }, ngContentSelectors: _c6, decls: 5, vars: 1, consts: [[1, "mat-content"], ["class", "mat-expansion-indicator", 4, "ngIf"], [1, "mat-expansion-indicator"]], template: function MatExpansionPanelHeader_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojectionDef"](_c5);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵelementStart"](0, "span", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojection"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojection"](2, 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵprojection"](3, 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵtemplate"](4, MatExpansionPanelHeader_span_4_Template, 1, 1, "span", 1);
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵproperty"]("ngIf", ctx._showToggle());
    } }, directives: [_angular_common__WEBPACK_IMPORTED_MODULE_2__["NgIf"]], styles: [".mat-expansion-panel-header{display:flex;flex-direction:row;align-items:center;padding:0 24px;border-radius:inherit}.mat-expansion-panel-header:focus,.mat-expansion-panel-header:hover{outline:none}.mat-expansion-panel-header.mat-expanded:focus,.mat-expansion-panel-header.mat-expanded:hover{background:inherit}.mat-expansion-panel-header:not([aria-disabled=true]){cursor:pointer}.mat-expansion-panel-header.mat-expansion-toggle-indicator-before{flex-direction:row-reverse}.mat-expansion-panel-header.mat-expansion-toggle-indicator-before .mat-expansion-indicator{margin:0 16px 0 0}[dir=rtl] .mat-expansion-panel-header.mat-expansion-toggle-indicator-before .mat-expansion-indicator{margin:0 0 0 16px}.mat-content{display:flex;flex:1;flex-direction:row;overflow:hidden}.mat-expansion-panel-header-title,.mat-expansion-panel-header-description{display:flex;flex-grow:1;margin-right:16px}[dir=rtl] .mat-expansion-panel-header-title,[dir=rtl] .mat-expansion-panel-header-description{margin-right:0;margin-left:16px}.mat-expansion-panel-header-description{flex-grow:2}.mat-expansion-indicator::after{border-style:solid;border-width:0 2px 2px 0;content:\"\";display:inline-block;padding:3px;transform:rotate(45deg);vertical-align:middle}\n"], encapsulation: 2, data: { animation: [
            matExpansionAnimations.indicatorRotate,
            matExpansionAnimations.expansionHeaderHeight
        ] }, changeDetection: 0 });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatExpansionPanelHeader, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Component"],
        args: [{
                selector: 'mat-expansion-panel-header',
                template: "<span class=\"mat-content\">\n  <ng-content select=\"mat-panel-title\"></ng-content>\n  <ng-content select=\"mat-panel-description\"></ng-content>\n  <ng-content></ng-content>\n</span>\n<span [@indicatorRotate]=\"_getExpandedState()\" *ngIf=\"_showToggle()\"\n      class=\"mat-expansion-indicator\"></span>\n",
                encapsulation: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ViewEncapsulation"].None,
                changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectionStrategy"].OnPush,
                animations: [
                    matExpansionAnimations.indicatorRotate,
                    matExpansionAnimations.expansionHeaderHeight
                ],
                host: {
                    'class': 'mat-expansion-panel-header',
                    'role': 'button',
                    '[attr.id]': 'panel._headerId',
                    '[attr.tabindex]': 'disabled ? -1 : 0',
                    '[attr.aria-controls]': '_getPanelId()',
                    '[attr.aria-expanded]': '_isExpanded()',
                    '[attr.aria-disabled]': 'panel.disabled',
                    '[class.mat-expanded]': '_isExpanded()',
                    '[class.mat-expansion-toggle-indicator-after]': "_getTogglePosition() === 'after'",
                    '[class.mat-expansion-toggle-indicator-before]': "_getTogglePosition() === 'before'",
                    '(click)': '_toggle()',
                    '(keydown)': '_keydown($event)',
                    '[@.disabled]': '_animationsDisabled',
                    '(@expansionHeight.start)': '_animationStarted()',
                    '[@expansionHeight]': "{\n        value: _getExpandedState(),\n        params: {\n          collapsedHeight: collapsedHeight,\n          expandedHeight: expandedHeight\n        }\n    }"
                },
                styles: [".mat-expansion-panel-header{display:flex;flex-direction:row;align-items:center;padding:0 24px;border-radius:inherit}.mat-expansion-panel-header:focus,.mat-expansion-panel-header:hover{outline:none}.mat-expansion-panel-header.mat-expanded:focus,.mat-expansion-panel-header.mat-expanded:hover{background:inherit}.mat-expansion-panel-header:not([aria-disabled=true]){cursor:pointer}.mat-expansion-panel-header.mat-expansion-toggle-indicator-before{flex-direction:row-reverse}.mat-expansion-panel-header.mat-expansion-toggle-indicator-before .mat-expansion-indicator{margin:0 16px 0 0}[dir=rtl] .mat-expansion-panel-header.mat-expansion-toggle-indicator-before .mat-expansion-indicator{margin:0 0 0 16px}.mat-content{display:flex;flex:1;flex-direction:row;overflow:hidden}.mat-expansion-panel-header-title,.mat-expansion-panel-header-description{display:flex;flex-grow:1;margin-right:16px}[dir=rtl] .mat-expansion-panel-header-title,[dir=rtl] .mat-expansion-panel-header-description{margin-right:0;margin-left:16px}.mat-expansion-panel-header-description{flex-grow:2}.mat-expansion-indicator::after{border-style:solid;border-width:0 2px 2px 0;content:\"\";display:inline-block;padding:3px;transform:rotate(45deg);vertical-align:middle}\n"]
            }]
    }], function () { return [{ type: MatExpansionPanel, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Host"]
            }] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ElementRef"] }, { type: _angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_6__["FocusMonitor"] }, { type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ChangeDetectorRef"] }, { type: undefined, decorators: [{
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Inject"],
                args: [MAT_EXPANSION_PANEL_DEFAULT_OPTIONS]
            }, {
                type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Optional"]
            }] }]; }, { expandedHeight: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"]
        }], collapsedHeight: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"]
        }] }); })();
    return MatExpansionPanelHeader;
}());
/**
 * `<mat-panel-description>`
 *
 * This directive is to be used inside of the MatExpansionPanelHeader component.
 */
var MatExpansionPanelDescription = /** @class */ (function () {
    function MatExpansionPanelDescription() {
    }
MatExpansionPanelDescription.ɵfac = function MatExpansionPanelDescription_Factory(t) { return new (t || MatExpansionPanelDescription)(); };
MatExpansionPanelDescription.ɵdir = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineDirective"]({ type: MatExpansionPanelDescription, selectors: [["mat-panel-description"]], hostAttrs: [1, "mat-expansion-panel-header-description"] });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatExpansionPanelDescription, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Directive"],
        args: [{
                selector: 'mat-panel-description',
                host: {
                    class: 'mat-expansion-panel-header-description'
                }
            }]
    }], function () { return []; }, null); })();
    return MatExpansionPanelDescription;
}());
/**
 * `<mat-panel-title>`
 *
 * This directive is to be used inside of the MatExpansionPanelHeader component.
 */
var MatExpansionPanelTitle = /** @class */ (function () {
    function MatExpansionPanelTitle() {
    }
MatExpansionPanelTitle.ɵfac = function MatExpansionPanelTitle_Factory(t) { return new (t || MatExpansionPanelTitle)(); };
MatExpansionPanelTitle.ɵdir = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineDirective"]({ type: MatExpansionPanelTitle, selectors: [["mat-panel-title"]], hostAttrs: [1, "mat-expansion-panel-header-title"] });
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatExpansionPanelTitle, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Directive"],
        args: [{
                selector: 'mat-panel-title',
                host: {
                    class: 'mat-expansion-panel-header-title'
                }
            }]
    }], function () { return []; }, null); })();
    return MatExpansionPanelTitle;
}());

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
/**
 * Directive for a Material Design Accordion.
 */
var MatAccordion = /** @class */ (function (_super) {
    Object(tslib__WEBPACK_IMPORTED_MODULE_4__["__extends"])(MatAccordion, _super);
    function MatAccordion() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        /** Headers belonging to this accordion. */
        _this._ownHeaders = new _angular_core__WEBPACK_IMPORTED_MODULE_3__["QueryList"]();
        _this._hideToggle = false;
        /**
         * Display mode used for all expansion panels in the accordion. Currently two display
         * modes exist:
         *  default - a gutter-like spacing is placed around any expanded panel, placing the expanded
         *     panel at a different elevation from the rest of the accordion.
         *  flat - no spacing is placed around expanded panels, showing all panels at the same
         *     elevation.
         */
        _this.displayMode = 'default';
        /** The position of the expansion indicator. */
        _this.togglePosition = 'after';
        return _this;
    }
    Object.defineProperty(MatAccordion.prototype, "hideToggle", {
        /** Whether the expansion indicator should be hidden. */
        get: function () { return this._hideToggle; },
        set: function (show) { this._hideToggle = Object(_angular_cdk_coercion__WEBPACK_IMPORTED_MODULE_5__["coerceBooleanProperty"])(show); },
        enumerable: true,
        configurable: true
    });
    MatAccordion.prototype.ngAfterContentInit = function () {
        var _this = this;
        this._headers.changes
            .pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_8__["startWith"])(this._headers))
            .subscribe(function (headers) {
            _this._ownHeaders.reset(headers.filter(function (header) { return header.panel.accordion === _this; }));
            _this._ownHeaders.notifyOnChanges();
        });
        this._keyManager = new _angular_cdk_a11y__WEBPACK_IMPORTED_MODULE_6__["FocusKeyManager"](this._ownHeaders).withWrap();
    };
    /** Handles keyboard events coming in from the panel headers. */
    MatAccordion.prototype._handleHeaderKeydown = function (event) {
        var keyCode = event.keyCode;
        var manager = this._keyManager;
        if (keyCode === _angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__["HOME"]) {
            if (!Object(_angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__["hasModifierKey"])(event)) {
                manager.setFirstItemActive();
                event.preventDefault();
            }
        }
        else if (keyCode === _angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__["END"]) {
            if (!Object(_angular_cdk_keycodes__WEBPACK_IMPORTED_MODULE_7__["hasModifierKey"])(event)) {
                manager.setLastItemActive();
                event.preventDefault();
            }
        }
        else {
            this._keyManager.onKeydown(event);
        }
    };
    MatAccordion.prototype._handleHeaderFocus = function (header) {
        this._keyManager.updateActiveItem(header);
    };
    MatAccordion.propDecorators = {
        _headers: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ContentChildren"], args: [MatExpansionPanelHeader, { descendants: true },] }],
        hideToggle: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"] }],
        displayMode: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"] }],
        togglePosition: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"] }]
    };
MatAccordion.ɵfac = function MatAccordion_Factory(t) { return ɵMatAccordion_BaseFactory(t || MatAccordion); };
MatAccordion.ɵdir = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineDirective"]({ type: MatAccordion, selectors: [["mat-accordion"]], contentQueries: function MatAccordion_ContentQueries(rf, ctx, dirIndex) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵcontentQuery"](dirIndex, MatExpansionPanelHeader, true);
    } if (rf & 2) {
        var _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵloadQuery"]()) && (ctx._headers = _t);
    } }, hostAttrs: [1, "mat-accordion"], hostVars: 2, hostBindings: function MatAccordion_HostBindings(rf, ctx) { if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵclassProp"]("mat-accordion-multi", ctx.multi);
    } }, inputs: { multi: "multi", hideToggle: "hideToggle", displayMode: "displayMode", togglePosition: "togglePosition" }, exportAs: ["matAccordion"], features: [_angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵProvidersFeature"]([{
                provide: MAT_ACCORDION,
                useExisting: MatAccordion
            }]), _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵInheritDefinitionFeature"]] });
var ɵMatAccordion_BaseFactory = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵgetInheritedFactory"](MatAccordion);
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatAccordion, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Directive"],
        args: [{
                selector: 'mat-accordion',
                exportAs: 'matAccordion',
                inputs: ['multi'],
                providers: [{
                        provide: MAT_ACCORDION,
                        useExisting: MatAccordion
                    }],
                host: {
                    class: 'mat-accordion',
                    // Class binding which is only used by the test harness as there is no other
                    // way for the harness to detect if multiple panel support is enabled.
                    '[class.mat-accordion-multi]': 'this.multi'
                }
            }]
    }], null, { hideToggle: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"]
        }], _headers: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["ContentChildren"],
            args: [MatExpansionPanelHeader, { descendants: true }]
        }], displayMode: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"]
        }], togglePosition: [{
            type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["Input"]
        }] }); })();
    return MatAccordion;
}(_angular_cdk_accordion__WEBPACK_IMPORTED_MODULE_0__["CdkAccordion"]));

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */
var MatExpansionModule = /** @class */ (function () {
    function MatExpansionModule() {
    }
MatExpansionModule.ɵmod = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineNgModule"]({ type: MatExpansionModule });
MatExpansionModule.ɵinj = _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵdefineInjector"]({ factory: function MatExpansionModule_Factory(t) { return new (t || MatExpansionModule)(); }, imports: [[_angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"], _angular_cdk_accordion__WEBPACK_IMPORTED_MODULE_0__["CdkAccordionModule"], _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_1__["PortalModule"]]] });
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵɵsetNgModuleScope"](MatExpansionModule, { declarations: function () { return [MatAccordion,
        MatExpansionPanel,
        MatExpansionPanelActionRow,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        MatExpansionPanelDescription,
        MatExpansionPanelContent]; }, imports: function () { return [_angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"], _angular_cdk_accordion__WEBPACK_IMPORTED_MODULE_0__["CdkAccordionModule"], _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_1__["PortalModule"]]; }, exports: function () { return [MatAccordion,
        MatExpansionPanel,
        MatExpansionPanelActionRow,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        MatExpansionPanelDescription,
        MatExpansionPanelContent]; } }); })();
/*@__PURE__*/ (function () { _angular_core__WEBPACK_IMPORTED_MODULE_3__["ɵsetClassMetadata"](MatExpansionModule, [{
        type: _angular_core__WEBPACK_IMPORTED_MODULE_3__["NgModule"],
        args: [{
                imports: [_angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"], _angular_cdk_accordion__WEBPACK_IMPORTED_MODULE_0__["CdkAccordionModule"], _angular_cdk_portal__WEBPACK_IMPORTED_MODULE_1__["PortalModule"]],
                exports: [
                    MatAccordion,
                    MatExpansionPanel,
                    MatExpansionPanelActionRow,
                    MatExpansionPanelHeader,
                    MatExpansionPanelTitle,
                    MatExpansionPanelDescription,
                    MatExpansionPanelContent,
                ],
                declarations: [
                    MatAccordion,
                    MatExpansionPanel,
                    MatExpansionPanelActionRow,
                    MatExpansionPanelHeader,
                    MatExpansionPanelTitle,
                    MatExpansionPanelDescription,
                    MatExpansionPanelContent,
                ]
            }]
    }], function () { return []; }, null); })();
    return MatExpansionModule;
}());

/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

/**
 * Generated bundle index. Do not edit.
 */



//# sourceMappingURL=expansion.js.map

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.html":
/*!************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.html ***!
  \************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple *ngIf=\"!contentType.UsesSharedDef\" matTooltip=\"Metadata\"\r\n    (click)=\"createOrEditMetadata()\">\r\n    <mat-icon>edit</mat-icon>\r\n  </div>\r\n  <div class=\"like-button disabled\" *ngIf=\"contentType.UsesSharedDef\">\r\n    <mat-icon>edit</mat-icon>\r\n  </div>\r\n\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Export\" (click)=\"openExport()\">\r\n    <mat-icon>cloud_download</mat-icon>\r\n  </div>\r\n\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Import\" (click)=\"openImport()\">\r\n    <mat-icon>cloud_upload</mat-icon>\r\n  </div>\r\n\r\n  <div class=\"like-button highlight\" matRipple *ngIf=\"showPermissions\" matTooltip=\"Permissions\"\r\n    (click)=\"openPermissions()\">\r\n    <mat-icon>person</mat-icon>\r\n  </div>\r\n  <div class=\"like-button disabled\" *ngIf=\"!showPermissions\">\r\n    <mat-icon>person</mat-icon>\r\n  </div>\r\n\r\n  <div class=\"like-button highlight\" matRipple *ngIf=\"!contentType.UsesSharedDef\" matTooltip=\"Delete\"\r\n    (click)=\"deleteContentType()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n  <div class=\"like-button disabled\" *ngIf=\"contentType.UsesSharedDef\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.html":
/*!**********************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.html ***!
  \**********************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div matRipple class=\"chip-component highlight\" matTooltip=\"Edit fields\" *ngIf=\"!contentType.UsesSharedDef\">\r\n  <div class=\"chip-box\">\r\n    <span class=\"chip\">{{ value }}</span>\r\n  </div>\r\n  <div class=\"like-button\">\r\n    <mat-icon>dns</mat-icon>\r\n  </div>\r\n</div>\r\n\r\n<div class=\"chip-component disabled\" *ngIf=\"contentType.UsesSharedDef\"\r\n  matTooltip=\"This content-type shares the definition of #{{contentType.SharedDefId}} so you can't edit it here. Read 2sxc.org/help?tag=shared-types\">\r\n  <div class=\"chip-box\">\r\n    <span class=\"chip\">{{ value }}</span>\r\n  </div>\r\n  <div class=\"like-button disabled\">\r\n    <mat-icon>share</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-items/data-items.component.html":
/*!********************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-items/data-items.component.html ***!
  \********************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div matRipple class=\"chip-component highlight\" matTooltip=\"Add item\">\r\n  <div class=\"chip-box\">\r\n    <span class=\"chip\">{{ value }}</span>\r\n  </div>\r\n  <div class=\"like-button\">\r\n    <mat-icon>add</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.html":
/*!******************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.html ***!
  \******************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Edit\" (click)=\"editQuery()\">\r\n    <mat-icon>edit</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Copy\" (click)=\"cloneQuery()\">\r\n    <mat-icon>file_copy</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Export\" (click)=\"exportQuery()\">\r\n    <mat-icon>cloud_download</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Permissions\" (click)=\"openPermissions()\">\r\n    <mat-icon>person</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Delete\" (click)=\"deleteQuery()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.html":
/*!**************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.html ***!
  \**************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Code\" (click)=\"openCode()\">\r\n    <mat-icon>code</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Permissions\" (click)=\"openPermissions()\">\r\n    <mat-icon>person</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Delete\" (click)=\"deleteView()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-show/views-show.component.html":
/*!********************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-show/views-show.component.html ***!
  \********************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\">\r\n  <mat-icon *ngIf=\"value\">visibility</mat-icon>\r\n  <mat-icon *ngIf=\"!value\">visibility_off</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-type/views-type.component.html":
/*!********************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-type/views-type.component.html ***!
  \********************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"icon-container\" matTooltip=\"{{ value }}\">\r\n  <mat-icon>{{ icon }}</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.html":
/*!****************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.html ***!
  \****************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div matRipple class=\"id-box highlight\" matTooltip=\"{{ tooltip }}\" *ngIf=\"id\" (click)=\"copy()\">\r\n  <span class=\"id\">{{ id }}</span>\r\n  <mat-icon class=\"icon\">file_copy</mat-icon>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.html":
/*!**************************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.html ***!
  \**************************************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"title\">Visible</div>\r\n<mat-radio-group [(ngModel)]=\"isVisible\" (ngModelChange)=\"filterChanged()\">\r\n  <mat-radio-button value=\"\">All</mat-radio-button>\r\n  <mat-radio-button value=\"true\">Visible</mat-radio-button>\r\n  <mat-radio-button value=\"false\">Hidden</mat-radio-button>\r\n</mat-radio-group>\r\n\r\n<div class=\"title\">Deleted</div>\r\n<mat-radio-group [(ngModel)]=\"isDeleted\" (ngModelChange)=\"filterChanged()\">\r\n  <mat-radio-button value=\"\">All</mat-radio-button>\r\n  <mat-radio-button value=\"true\">Is deleted</mat-radio-button>\r\n  <mat-radio-button value=\"false\">Is not deleted</mat-radio-button>\r\n</mat-radio-group>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.html":
/*!******************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.html ***!
  \******************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"actions-component\">\r\n  <div class=\"like-button highlight\" matRipple matTooltip=\"Code\" (click)=\"openCode()\">\r\n    <mat-icon>code</mat-icon>\r\n  </div>\r\n  <div class=\"like-button highlight not-implemented\" style=\"display: none;\" matRipple matTooltip=\"Delete\"\r\n    (click)=\"deleteApi()\">\r\n    <mat-icon>delete</mat-icon>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/app-administration-nav/app-administration-nav.component.html":
/*!*************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/app-administration-nav/app-administration-nav.component.html ***!
  \*************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"nav-component-wrapper\" (click)=\"toggleDebugEnabled($event)\">\r\n  <div mat-dialog-title>\r\n    <div class=\"dialog-title-box\">\r\n      <div>{{ app?.Name }} Admin</div>\r\n      <button mat-icon-button matTooltip=\"Close dialog\" (click)=\"closeDialog()\">\r\n        <mat-icon>close</mat-icon>\r\n      </button>\r\n    </div>\r\n  </div>\r\n\r\n  <router-outlet></router-outlet>\r\n\r\n  <!-- spm NOTE: we use mat-tab-group because mat-tab-nav-bar doesn't have animations and doesn't look pretty -->\r\n  <mat-tab-group dynamicHeight color=\"accent\" *ngIf=\"dialogSettings\" (selectedTabChange)=\"changeTab($event)\"\r\n    [selectedIndex]=\"tabIndex\">\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"Getting Started\">\r\n        <mat-icon>info</mat-icon>\r\n        <span>Info</span>\r\n      </div>\r\n      <app-getting-started [gettingStartedUrl]=\"dialogSettings.GettingStartedUrl\">\r\n      </app-getting-started>\r\n    </mat-tab>\r\n\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"Content\">\r\n        <mat-icon>menu</mat-icon>\r\n        <span>Data</span>\r\n      </div>\r\n      <app-data *matTabContent></app-data>\r\n    </mat-tab>\r\n\r\n    <mat-tab *ngIf=\"!dialogSettings.IsContent\">\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"Query Designer\">\r\n        <mat-icon>filter_list</mat-icon>\r\n        <span>Queries</span>\r\n      </div>\r\n      <app-queries *matTabContent></app-queries>\r\n    </mat-tab>\r\n\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"Views / Templates\">\r\n        <mat-icon>layers</mat-icon>\r\n        <span>Views</span>\r\n      </div>\r\n      <app-views *matTabContent></app-views>\r\n    </mat-tab>\r\n\r\n    <mat-tab *ngIf=\"!dialogSettings.IsContent\">\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"WebApi / Data\">\r\n        <mat-icon>offline_bolt</mat-icon>\r\n        <span>WebApi</span>\r\n      </div>\r\n      <app-web-api *matTabContent></app-web-api>\r\n    </mat-tab>\r\n\r\n    <mat-tab>\r\n      <div *matTabLabel class=\"mat-tab-label-box\" matTooltip=\"App Settings\">\r\n        <mat-icon>settings_applications</mat-icon>\r\n        <span>App</span>\r\n      </div>\r\n      <app-app-configuration *matTabContent></app-app-configuration>\r\n    </mat-tab>\r\n  </mat-tab-group>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/app-configuration/app-configuration.component.html":
/*!***************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/app-configuration/app-configuration.component.html ***!
  \***************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"cards-box\">\r\n  <mat-card class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>App Settings</mat-card-title>\r\n      <div class=\"actions-box\">\r\n        <button mat-icon-button matTooltip=\"Edit app settings\" (click)=\"edit(eavConstants.contentTypes.settings)\">\r\n          <mat-icon>edit</mat-icon>\r\n        </button>\r\n        <button mat-icon-button matTooltip=\"Configure app settings\"\r\n          (click)=\"config(eavConstants.contentTypes.settings)\">\r\n          <mat-icon>dns</mat-icon>\r\n        </button>\r\n      </div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      Settings are configurations used by the app - like SQL-connection strings, default \"items-to-show\" numbers and\r\n      things like that. They can also be multi-language, so that a setting (like default RSS-Feed) could be different in\r\n      each language.\r\n    </mat-card-content>\r\n  </mat-card>\r\n\r\n  <mat-card class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>App Resources</mat-card-title>\r\n      <div class=\"actions-box\">\r\n        <button mat-icon-button matTooltip=\"Edit app resources\" (click)=\"edit(eavConstants.contentTypes.resources)\">\r\n          <mat-icon>edit</mat-icon>\r\n        </button>\r\n        <button mat-icon-button matTooltip=\"Configure app resources\"\r\n          (click)=\"config(eavConstants.contentTypes.resources)\">\r\n          <mat-icon>dns</mat-icon>\r\n        </button>\r\n      </div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      Resources are used for labels and things like that in the App. They are usually needed to create multi-lingual\r\n      views and such, and should not be used for App-Settings.\r\n    </mat-card-content>\r\n  </mat-card>\r\n</div>\r\n\r\n\r\n<div class=\"cards-box\">\r\n  <mat-card class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>App Package Definition</mat-card-title>\r\n      <div class=\"actions-box\">\r\n        <button mat-icon-button matTooltip=\"Edit app definition\" (click)=\"edit(eavConstants.scopes.app.value)\">\r\n          <mat-icon>redeem</mat-icon>\r\n        </button>\r\n      </div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      The app-package definition is important when exporting/importing this app.\r\n    </mat-card-content>\r\n  </mat-card>\r\n\r\n  <mat-card *ngIf=\"showPermissions\" class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>App Permissions</mat-card-title>\r\n      <div class=\"actions-box\">\r\n        <button mat-icon-button matTooltip=\"Edit permissions\" (click)=\"openPermissions()\">\r\n          <mat-icon>person</mat-icon>\r\n        </button>\r\n      </div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      Permissions give access to people to the entire app (all content-types / data), so use with care.\r\n    </mat-card-content>\r\n  </mat-card>\r\n</div>\r\n\r\n\r\n<div class=\"cards-box\">\r\n  <mat-card class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>Export this entire App</mat-card-title>\r\n      <div class=\"actions-box\">\r\n        <button mat-icon-button matTooltip=\"Export\" (click)=\"exportApp()\">\r\n          <mat-icon>cloud_download</mat-icon>\r\n        </button>\r\n      </div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      Create an app-package (zip) which can be installed in another portal.\r\n    </mat-card-content>\r\n  </mat-card>\r\n\r\n  <mat-card class=\"mat-elevation-z2\">\r\n    <mat-card-header>\r\n      <mat-card-title>Export or Import parts of this App</mat-card-title>\r\n      <div class=\"actions-box\">\r\n        <button mat-icon-button matTooltip=\"Export\" (click)=\"exportParts()\">\r\n          <mat-icon>cloud_download</mat-icon>\r\n        </button>\r\n        <button mat-icon-button matTooltip=\"Import\" (click)=\"importParts()\">\r\n          <mat-icon>cloud_upload</mat-icon>\r\n        </button>\r\n      </div>\r\n    </mat-card-header>\r\n    <mat-card-content>\r\n      Create an xml containing parts of the app, e.g. content and templates.\r\n      Import parts from such an xml into this app.\r\n    </mat-card-content>\r\n  </mat-card>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/data/data.component.html":
/*!*************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/data/data.component.html ***!
  \*************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"grid-wrapper mat-tab-grid-wrapper\">\r\n  <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"contentTypes\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n  </ag-grid-angular>\r\n\r\n  <div class=\"scope-box\">\r\n    <button mat-icon-button color=\"accent\" matTooltip=\"Create a ghost content type\" *ngIf=\"debugEnabled\"\r\n      (click)=\"createGhost()\">\r\n      <mat-icon>share</mat-icon>\r\n    </button>\r\n    <mat-form-field appearance=\"standard\" color=\"accent\" class=\"scope-box__dropdown\">\r\n      <mat-select [ngModel]=\"scope\" name=\"Scope\" (selectionChange)=\"changeScope($event)\">\r\n        <mat-option *ngFor=\"let scopeOption of scopeOptions\" [value]=\"scopeOption.value\">\r\n          {{ 'Scope: ' + scopeOption.name }}\r\n        </mat-option>\r\n        <mat-option value=\"Other\">Scope: Other...</mat-option>\r\n      </mat-select>\r\n    </mat-form-field>\r\n  </div>\r\n\r\n  <button mat-fab mat-elevation-z24 class=\"grid-fab\" matTooltip=\"Create a new content type\"\r\n    (click)=\"editContentType(null)\">\r\n    <mat-icon>add</mat-icon>\r\n  </button>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/getting-started/getting-started.component.html":
/*!***********************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/getting-started/getting-started.component.html ***!
  \***********************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<iframe class=\"iframe\" [src]=\"gettingStartedSafe\"></iframe>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/queries/queries.component.html":
/*!*******************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/queries/queries.component.html ***!
  \*******************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"grid-wrapper mat-tab-grid-wrapper\">\r\n  <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"queries\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n  </ag-grid-angular>\r\n\r\n  <eco-fab-speed-dial class=\"grid-fab\">\r\n    <eco-fab-speed-dial-trigger spin=\"true\">\r\n      <button mat-fab>\r\n        <mat-icon class=\"spin180\">add</mat-icon>\r\n      </button>\r\n    </eco-fab-speed-dial-trigger>\r\n\r\n    <eco-fab-speed-dial-actions>\r\n      <button mat-mini-fab matTooltip=\"Import query\" (click)=\"importQuery()\">\r\n        <mat-icon>cloud_upload</mat-icon>\r\n      </button>\r\n      <button mat-mini-fab matTooltip=\"Create a new query\" (click)=\"editQuery(null)\">\r\n        <mat-icon>add</mat-icon>\r\n      </button>\r\n    </eco-fab-speed-dial-actions>\r\n  </eco-fab-speed-dial>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/content-import/content-import.component.html":
/*!*********************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/content-import/content-import.component.html ***!
  \*********************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<!-- HEADER -->\r\n<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">\r\n    Import Content / Data\r\n    <ng-container *ngIf=\"viewStateSelected > 0\">Step {{ viewStateSelected }} of 3</ng-container>\r\n  </div>\r\n</div>\r\n\r\n<p class=\"dialog-description\">\r\n  This will import content-items into 2sxc. It requires that you already defined the content-type before you try\r\n  importing, and that you created the import-file using the template provided by the Export. Please visit\r\n  <a href=\"http://2sxc.org/help\" target=\"_blank\">http://2sxc.org/help</a> for more instructions.\r\n</p>\r\n<!-- END HEADER -->\r\n\r\n<ng-container [ngSwitch]=\"viewStateSelected\">\r\n\r\n  <!-- FORM -->\r\n  <form #ngForm=\"ngForm\" class=\"dialog-form\" *ngSwitchCase=\"1\">\r\n    <div class=\"dialog-form-content fancy-scrollbar-light\">\r\n      <div>\r\n        <button mat-raised-button matTooltip=\"Open file browser\" (click)=\"fileInput.click()\">\r\n          <span *ngIf=\"!formValues.file\">Select file</span>\r\n          <span *ngIf=\"formValues.file\">{{ formValues.file.name }}</span>\r\n        </button>\r\n        <input #fileInput type=\"file\" (change)=\"fileChange($event)\" class=\"hide\" />\r\n      </div>\r\n\r\n      <div>\r\n        <p class=\"field-label\">References to pages / files</p>\r\n        <mat-radio-group [(ngModel)]=\"formValues.resourcesReferences\" name=\"ResourcesReferences\">\r\n          <mat-radio-button value=\"Keep\">\r\n            Import links as written in the file (for example /Portals/...)\r\n          </mat-radio-button>\r\n          <mat-radio-button value=\"Resolve\">\r\n            Try to resolve paths to references\r\n          </mat-radio-button>\r\n        </mat-radio-group>\r\n      </div>\r\n\r\n      <div>\r\n        <p class=\"field-label\">Clear all other entities</p>\r\n        <mat-radio-group [(ngModel)]=\"formValues.clearEntities\" name=\"ClearEntities\">\r\n          <mat-radio-button value=\"None\">\r\n            Keep all entities not found in import\r\n          </mat-radio-button>\r\n          <mat-radio-button value=\"All\">\r\n            Remove all entities not found in import\r\n          </mat-radio-button>\r\n        </mat-radio-group>\r\n      </div>\r\n\r\n      <p class=\"hint\">Remember to backup your DNN first!</p>\r\n    </div>\r\n\r\n    <div class=\"dialog-form-actions\">\r\n      <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n      <button mat-raised-button color=\"accent\" [disabled]=\"!formValues.file || !formValues.file.name\"\r\n        (click)=\"evaluateContent()\">\r\n        Preview Import\r\n      </button>\r\n    </div>\r\n  </form>\r\n  <!-- END FORM -->\r\n\r\n  <!-- WAITING -->\r\n  <p *ngSwitchCase=\"0\" class=\"progress-message\">Please wait while processing...</p>\r\n  <!-- END WAITING -->\r\n\r\n  <!-- EVALUATION RESULT -->\r\n  <ng-container *ngSwitchCase=\"2\">\r\n    <ng-container *ngIf=\"evaluationResult\">\r\n      <!-- DETAILS / STATISTICS -->\r\n      <div *ngIf=\"evaluationResult.Succeeded\" class=\"dialog-component-content fancy-scrollbar-light\">\r\n        <p class=\"evaluation__title\">Try to import file '{{ formValues.file.name }}'</p>\r\n        <p class=\"evaluation__title\">File contains:</p>\r\n        <ul class=\"evaluation__content\">\r\n          <li>{{ evaluationResult.Detail.DocumentElementsCount }} content-items (records/entities)</li>\r\n          <li>{{ evaluationResult.Detail.LanguagesInDocumentCount }} languages</li>\r\n          <li>{{ evaluationResult.Detail.AttributeNamesInDocument.length }} columns:\r\n            {{ evaluationResult.Detail.AttributeNamesInDocument.join(', ') }}</li>\r\n        </ul>\r\n        <p class=\"evaluation__title\">If you press Import, it will:</p>\r\n        <ul class=\"evaluation__content\">\r\n          <li>Create {{ evaluationResult.Detail.AmountOfEntitiesCreated }} content-items</li>\r\n          <li>Update {{ evaluationResult.Detail.AmountOfEntitiesUpdated }} content-items</li>\r\n          <li>Delete {{ evaluationResult.Detail.AmountOfEntitiesDeleted }} content-items</li>\r\n          <li>Ignore {{ evaluationResult.Detail.AttributeNamesNotImported.length }} columns:\r\n            {{ evaluationResult.Detail.AttributeNamesNotImported.join(', ') }}</li>\r\n        </ul>\r\n        <p class=\"hint\">Note: The import validates much data and may take several minutes.</p>\r\n      </div>\r\n      <!-- END DETAILS / STATISTICS -->\r\n      <!-- ERRORS -->\r\n      <div *ngIf=\"!evaluationResult.Succeeded\" class=\"dialog-component-content fancy-scrollbar-light\">\r\n        <p class=\"evaluation__title\">Try to import file '{{ formValues.file.name }}'</p>\r\n        <ul class=\"evaluation__content\">\r\n          <li *ngFor=\"let error of evaluationResult.Detail\">\r\n            <div>{{ errors[error.ErrorCode] }}</div>\r\n            <div *ngIf=\"error.ErrorDetail\"><i>Details: {{ error.ErrorDetail }}</i></div>\r\n            <div *ngIf=\"error.LineNumber\"><i>Line-no: {{ error.LineNumber }}</i></div>\r\n            <div *ngIf=\"error.LineDetail\"><i>Line-details: {{ error.LineDetail }}</i></div>\r\n          </li>\r\n        </ul>\r\n      </div>\r\n      <!-- END ERRORS -->\r\n      <div class=\"dialog-component-actions\">\r\n        <button mat-raised-button (click)=\"back()\">Back</button>\r\n        <button mat-raised-button color=\"accent\" [disabled]=\"!evaluationResult.Succeeded\" (click)=\"importContent()\">\r\n          Import\r\n        </button>\r\n      </div>\r\n    </ng-container>\r\n  </ng-container>\r\n  <!-- END EVALUATION RESULT -->\r\n\r\n  <!-- IMPORT RESULT -->\r\n  <div *ngSwitchCase=\"3\">\r\n    <div *ngIf=\"importResult\" class=\"progress-message\">\r\n      <p *ngIf=\"importResult.Succeeded\">Import done.</p>\r\n      <p *ngIf=\"!importResult.Succeeded\">Import failed.</p>\r\n    </div>\r\n    <div class=\"dialog-component-actions\">\r\n      <button mat-raised-button color=\"accent\" (click)=\"closeDialog()\">Close</button>\r\n    </div>\r\n  </div>\r\n  <!-- END IMPORT RESULT -->\r\n</ng-container>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.html":
/*!***************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.html ***!
  \***************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">{{ id ? 'Edit Content Type' : 'New Content Type' }}</div>\r\n</div>\r\n\r\n<form class=\"dialog-form\" *ngIf=\"contentType\" #ngForm=\"ngForm\">\r\n  <div class=\"dialog-form-content fancy-scrollbar-light\">\r\n    <div class=\"edit-input\">\r\n      <mat-form-field appearance=\"standard\" color=\"accent\">\r\n        <mat-label>Name</mat-label>\r\n        <input matInput type=\"text\" [pattern]=\"contentTypeNamePattern\" [(ngModel)]=\"contentType.Name\" name=\"Name\"\r\n          required #name=\"ngModel\">\r\n      </mat-form-field>\r\n      <ng-container *ngIf=\"name.touched && name.errors\">\r\n        <app-field-hint *ngIf=\"name.errors.required\" [isError]=\"true\">This field is required</app-field-hint>\r\n        <app-field-hint *ngIf=\"name.errors.pattern\" [isError]=\"true\">{{ contentTypeNameError }}</app-field-hint>\r\n      </ng-container>\r\n    </div>\r\n    <mat-accordion [@.disabled]=\"disableAnimation\">\r\n      <mat-expansion-panel expanded=\"false\">\r\n        <mat-expansion-panel-header>\r\n          <mat-panel-title>Advanced</mat-panel-title>\r\n          <mat-panel-description></mat-panel-description>\r\n        </mat-expansion-panel-header>\r\n\r\n        <!-- spm TODO: Remove description from here. It will only be available in metadata -->\r\n        <div class=\"edit-input\">\r\n          <mat-form-field appearance=\"standard\" color=\"accent\">\r\n            <mat-label>Description</mat-label>\r\n            <input matInput type=\"text\" [ngModel]=\"contentType.Description\" name=\"Description\" disabled>\r\n          </mat-form-field>\r\n        </div>\r\n\r\n        <div class=\"edit-input\">\r\n          <mat-form-field appearance=\"standard\" color=\"accent\">\r\n            <mat-label>Scope</mat-label>\r\n            <mat-select [ngModel]=\"contentType.Scope\" name=\"Scope\" (selectionChange)=\"changeScope($event)\"\r\n              [disabled]=\"lockScope\">\r\n              <mat-option *ngFor=\"let scopeOption of scopeOptions\" [value]=\"scopeOption.value\">{{ scopeOption.name }}\r\n              </mat-option>\r\n              <mat-option value=\"Other\">Other...</mat-option>\r\n            </mat-select>\r\n            <button mat-icon-button type=\"button\" matSuffix [matTooltip]=\"lockScope ? 'Unlock' : 'Lock'\">\r\n              <mat-icon *ngIf=\"lockScope\" (click)=\"unlockScope($event)\">lock</mat-icon>\r\n              <mat-icon *ngIf=\"!lockScope\" (click)=\"unlockScope($event)\">lock_open</mat-icon>\r\n            </button>\r\n          </mat-form-field>\r\n          <app-field-hint>\r\n            The scope should almost never be changed -\r\n            <a href=\"http://2sxc.org/help?tag=scope\" target=\"_blank\" appClickStopPropagation>see help</a>\r\n          </app-field-hint>\r\n        </div>\r\n\r\n        <div class=\"edit-input\">\r\n          <mat-form-field appearance=\"standard\" color=\"accent\">\r\n            <mat-label>Static Name</mat-label>\r\n            <input matInput type=\"text\" [ngModel]=\"contentType.StaticName\" name=\"StaticName\" disabled>\r\n          </mat-form-field>\r\n        </div>\r\n\r\n        <div *ngIf=\"contentType.SharedDefId\" class=\"edit-input\">\r\n          <h3>Shared Content Type (Ghost)</h3>\r\n          <p>Note: this can't be edited in the UI, for now if you really know what you're doing, do it in the DB</p>\r\n          <p>Uses Type Definition of: {{ contentType.SharedDefId }}</p>\r\n        </div>\r\n      </mat-expansion-panel>\r\n    </mat-accordion>\r\n  </div>\r\n\r\n  <div class=\"dialog-form-actions\">\r\n    <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n    <button mat-raised-button color=\"accent\" [disabled]=\"!ngForm.form.valid\" (click)=\"save()\">Save</button>\r\n  </div>\r\n</form>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.html":
/*!*************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.html ***!
  \*************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Export Content and Templates from this App</div>\r\n</div>\r\n\r\n<p class=\"dialog-description\">\r\n  This is an advanced feature to export parts of the app.\r\n  The export will create an xml file which can be imported into another site or app.\r\n  To export the entire content of the app (for example when duplicating the entire site), go to the app export.\r\n  For further help visit <a href=\"http://2sxc.org/en/help?tag=export\" target=\"_blank\">2sxc Help</a>.\r\n</p>\r\n\r\n<div class=\"dialog-component-content fancy-scrollbar-light\">\r\n  <div class=\"edit-input\">\r\n    <mat-form-field appearance=\"standard\" color=\"accent\">\r\n      <mat-label>Scope</mat-label>\r\n      <mat-select [ngModel]=\"exportScope\" name=\"Scope\" (selectionChange)=\"changeScope($event)\" [disabled]=\"lockScope\">\r\n        <mat-option *ngFor=\"let scopeOption of scopeOptions\" [value]=\"scopeOption.value\">\r\n          {{ scopeOption.name }}\r\n        </mat-option>\r\n        <mat-option value=\"Other\">Other...</mat-option>\r\n      </mat-select>\r\n      <button mat-icon-button type=\"button\" matSuffix [matTooltip]=\"lockScope ? 'Unlock' : 'Lock'\">\r\n        <mat-icon *ngIf=\"lockScope\" (click)=\"unlockScope($event)\">lock</mat-icon>\r\n        <mat-icon *ngIf=\"!lockScope\" (click)=\"unlockScope($event)\">lock_open</mat-icon>\r\n      </button>\r\n    </mat-form-field>\r\n    <app-field-hint>\r\n      The scope should almost never be changed -\r\n      <a href=\"http://2sxc.org/help?tag=scope\" target=\"_blank\" appClickStopPropagation>see help</a>\r\n    </app-field-hint>\r\n  </div>\r\n\r\n  <div *ngIf=\"contentInfo\">\r\n    <ul class=\"content-info__list content-info__base\">\r\n      <p class=\"content-info__title\">Content Types</p>\r\n      <li class=\"content-info__item\" *ngFor=\"let contentType of contentInfo.ContentTypes\">\r\n        <div class=\"option-box\">\r\n          <mat-checkbox [(ngModel)]=\"contentType._export\">\r\n            <span class=\"option-box__text\">{{ contentType.Name }} ({{ contentType.Id }})</span>\r\n          </mat-checkbox>\r\n        </div>\r\n\r\n        <ul class=\"content-info__list\" *ngIf=\"contentType.Templates.length > 0\">\r\n          <p class=\"content-info__subtitle\">Templates</p>\r\n          <li class=\"content-info__item\" *ngFor=\"let template of contentType.Templates\">\r\n            <div class=\"option-box\">\r\n              <mat-checkbox [(ngModel)]=\"template._export\">\r\n                <span class=\"option-box__text\">{{ template.Name }} ({{ template.Id }})</span>\r\n              </mat-checkbox>\r\n            </div>\r\n          </li>\r\n        </ul>\r\n\r\n        <ul class=\"content-info__list\" *ngIf=\"contentType.Entities.length > 0\">\r\n          <p class=\"content-info__subtitle\">Entities</p>\r\n          <li class=\"content-info__item\" *ngFor=\"let entity of contentType.Entities\">\r\n            <div class=\"option-box\">\r\n              <mat-checkbox [(ngModel)]=\"entity._export\">\r\n                <span class=\"option-box__text\">{{ entity.Title }} ({{ entity.Id }})</span>\r\n              </mat-checkbox>\r\n            </div>\r\n          </li>\r\n        </ul>\r\n      </li>\r\n    </ul>\r\n\r\n    <ul class=\"content-info__list content-info__base\">\r\n      <p class=\"content-info__title\">Templates Without Content Types</p>\r\n      <li class=\"content-info__item\" *ngFor=\"let template of contentInfo.TemplatesWithoutContentTypes\">\r\n        <div class=\"option-box\">\r\n          <mat-checkbox [(ngModel)]=\"template._export\">\r\n            <span class=\"option-box__text\">{{ template.Name }} ({{ template.Id }})</span>\r\n          </mat-checkbox>\r\n        </div>\r\n      </li>\r\n    </ul>\r\n  </div>\r\n</div>\r\n\r\n<div class=\"dialog-component-actions\">\r\n  <button mat-raised-button [disabled]=\"isExporting\" (click)=\"closeDialog()\">Cancel</button>\r\n  <button mat-raised-button color=\"accent\" [disabled]=\"isExporting\" (click)=\"exportAppParts()\">Export</button>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/export-app/export-app.component.html":
/*!*************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/export-app/export-app.component.html ***!
  \*************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Export app</div>\r\n</div>\r\n\r\n<p class=\"dialog-description\">\r\n  Pack the entire app to a <em>zip</em> folder which can be imported again to another site.\r\n  For further help visit <a href=\"http://2sxc.org/en/help?tag=export-app\" target=\"_blank\">2sxc Help</a>.\r\n</p>\r\n\r\n<div class=\"dialog-component-content fancy-scrollbar-light\" *ngIf=\"appInfo\">\r\n  <p class=\"app-info__title\">Specs</p>\r\n  <ul class=\"app-info__content\">\r\n    <li>Name: {{appInfo.Name}}</li>\r\n    <li>Guid: {{appInfo.Guid}}</li>\r\n    <li>Version: {{appInfo.Version}}</li>\r\n  </ul>\r\n\r\n  <p class=\"app-info__title\">Contains</p>\r\n  <ul class=\"app-info__content\">\r\n    <li>{{appInfo.EntitiesCount}} entities</li>\r\n    <li>{{appInfo.LanguagesCount}} languages</li>\r\n    <li>\r\n      {{appInfo.TemplatesCount}} templates (Token: {{appInfo.HasTokenTemplates}}, Razor: {{appInfo.HasRazorTemplates}})\r\n    </li>\r\n    <li>{{appInfo.TransferableFilesCount}} files to export</li>\r\n    <li>{{appInfo.FilesCount}} files in the app folder totally</li>\r\n  </ul>\r\n\r\n  <div class=\"options-wrapper\">\r\n    <div class=\"option-box\">\r\n      <mat-checkbox [(ngModel)]=\"includeContentGroups\" [disabled]=\"resetAppGuid\">\r\n        <span class=\"option-box__text\">\r\n          Include all content-groups to re-import the app in an exact copy of this site.\r\n          Only select this option when you copy an entire DNN site.\r\n        </span>\r\n      </mat-checkbox>\r\n    </div>\r\n    <div class=\"option-box\">\r\n      <mat-checkbox [(ngModel)]=\"resetAppGuid\" [disabled]=\"includeContentGroups\">\r\n        <span class=\"option-box__text\">\r\n          Reset the app GUID to zero. You only need this option for special tutorial apps, and usually must not select\r\n          it.\r\n        </span>\r\n      </mat-checkbox>\r\n    </div>\r\n  </div>\r\n</div>\r\n\r\n<div class=\"dialog-component-actions\">\r\n  <button mat-raised-button [disabled]=\"isExporting\" (click)=\"closeDialog()\">Cancel</button>\r\n  <button mat-raised-button [disabled]=\"isExporting\" (click)=\"exportGit()\">Export Data for Github versioning</button>\r\n  <button mat-raised-button color=\"accent\" [disabled]=\"isExporting\" (click)=\"exportApp()\">Export App</button>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.html":
/*!*************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.html ***!
  \*************************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Import Content and Templates into this App</div>\r\n</div>\r\n\r\n<mat-spinner *ngIf=\"isImporting\" mode=\"indeterminate\" diameter=\"20\" color=\"accent\"></mat-spinner>\r\n\r\n<div *ngIf=\"!importResult || (importResult && !importResult.Messages)\">\r\n  <p class=\"dialog-description\">\r\n    Import content and templates from a content export (zip) or partial export (xml) to this app.\r\n    The entire content of the selected file will be imported.\r\n    If you want to import an entire app, go to the <em>App-Management</em>.\r\n    For further help visit <a href=\"http://2sxc.org/en/help?tag=import\" target=\"_blank\">2sxc Help</a>.\r\n  </p>\r\n\r\n  <div>\r\n    <button mat-raised-button matTooltip=\"Open file browser\" (click)=\"fileInput.click()\">\r\n      <span *ngIf=\"!importFile\">Select file</span>\r\n      <span *ngIf=\"importFile\">{{ importFile.name }}</span>\r\n    </button>\r\n    <input #fileInput type=\"file\" (change)=\"fileChange($event)\" class=\"hide\" />\r\n  </div>\r\n\r\n  <div class=\"dialog-component-actions\">\r\n    <button mat-raised-button [disabled]=\"isImporting\" (click)=\"closeDialog()\">Cancel</button>\r\n    <button mat-raised-button color=\"accent\" [disabled]=\"!importFile || isImporting\"\r\n      (click)=\"importAppParts()\">Import</button>\r\n  </div>\r\n</div>\r\n\r\n<ng-container *ngIf=\"importResult && importResult.Messages && !isImporting\">\r\n  <div class=\"dialog-component-content fancy-scrollbar-light\">\r\n    <div *ngIf=\"importResult.Succeeded\" class=\"sxc-message sxc-message-info\">\r\n      The import has been done. See the messages below for more information.\r\n    </div>\r\n    <div *ngIf=\"!importResult.Succeeded\" class=\"sxc-message sxc-message-error\">\r\n      The import failed. See the messages below for more information.\r\n    </div>\r\n    <div *ngFor=\"let message of importResult.Messages\" class=\"sxc-message\" [ngClass]=\"{\r\n    'sxc-message-warning': message.MessageType === 0,\r\n    'sxc-message-success': message.MessageType === 1,\r\n    'sxc-message-error': message.MessageType === 2\r\n  }\">\r\n      {{ message.Text }}\r\n    </div>\r\n  </div>\r\n  <div class=\"dialog-component-actions\">\r\n    <button mat-raised-button color=\"accent\" (click)=\"closeDialog()\">Close</button>\r\n  </div>\r\n</ng-container>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/import-query/import-query.component.html":
/*!*****************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/import-query/import-query.component.html ***!
  \*****************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Import Query</div>\r\n</div>\r\n\r\n<div [ngSwitch]=\"viewState\">\r\n  <div *ngSwitchCase=\"1\">\r\n    <div>\r\n      <button mat-raised-button matTooltip=\"Open file browser\" (click)=\"fileInput.click()\">\r\n        <span *ngIf=\"!importFile\">Select file</span>\r\n        <span *ngIf=\"importFile\">{{ importFile.name }}</span>\r\n      </button>\r\n      <input #fileInput type=\"file\" (change)=\"fileChange($event)\" class=\"hide\" />\r\n    </div>\r\n\r\n    <div class=\"dialog-component-actions\">\r\n      <button mat-raised-button (click)=\"closeDialog()\">Cancel</button>\r\n      <button mat-raised-button color=\"accent\" [disabled]=\"!importFile\" (click)=\"importQuery()\">Import</button>\r\n    </div>\r\n  </div>\r\n  <div *ngSwitchCase=\"2\">\r\n    <mat-spinner mode=\"indeterminate\" diameter=\"20\" color=\"accent\"></mat-spinner>\r\n    <div class=\"dialog-component-actions\">\r\n      <button mat-raised-button disabled>Cancel</button>\r\n      <button mat-raised-button color=\"accent\" disabled>Import</button>\r\n    </div>\r\n  </div>\r\n  <div *ngSwitchCase=\"3\">\r\n    <p>Import completed!</p>\r\n    <div class=\"dialog-component-actions\">\r\n      <button mat-raised-button color=\"accent\" (click)=\"closeDialog()\">Close</button>\r\n    </div>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.html":
/*!***************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.html ***!
  \***************************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"nav-component-wrapper\">\r\n  <div mat-dialog-title>\r\n    <div class=\"dialog-title-box\">\r\n      <div matTooltip=\"{{ viewTooltip }}\">View: {{ viewUsage?.Name }}</div>\r\n      <button mat-icon-button matTooltip=\"Close dialog\" (click)=\"closeDialog()\">\r\n        <mat-icon>close</mat-icon>\r\n      </button>\r\n    </div>\r\n  </div>\r\n\r\n  <p class=\"dialog-description\">\r\n    This shows where this view is being used. Read about the\r\n    <a href=\"https://r.2sxc.org/content-in-dnn\" target=\"_blank\">connections between DNN and 2sxc Views</a>\r\n    to make good decisions if you're doing clean-up or just want to better know what this is all about.\r\n    <em>Before you get confused about some things you see here, do read the FAQ on that page.</em>\r\n  </p>\r\n\r\n  <router-outlet></router-outlet>\r\n\r\n  <div class=\"grid-wrapper\">\r\n    <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"data\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n    </ag-grid-angular>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/views/views.component.html":
/*!***************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/views/views.component.html ***!
  \***************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"grid-wrapper mat-tab-grid-wrapper\">\r\n  <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"views\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n  </ag-grid-angular>\r\n\r\n  <div class=\"more-actions-box\">\r\n    <button mat-raised-button matTooltip=\"Configure Polymorphism\" (click)=\"editPolymorphisms()\">\r\n      <img class=\"polymorph-logo\" [src]=\"polymorphLogo\">\r\n      <span>Polymorphism {{ polymorphStatus }}</span>\r\n    </button>\r\n  </div>\r\n\r\n  <button mat-fab mat-elevation-z24 class=\"grid-fab\" matTooltip=\"Create a new view\" (click)=\"editView(null)\">\r\n    <mat-icon>add</mat-icon>\r\n  </button>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/web-api/web-api.component.html":
/*!*******************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/web-api/web-api.component.html ***!
  \*******************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"grid-wrapper mat-tab-grid-wrapper\">\r\n  <ag-grid-angular class=\"ag-theme-material\" [rowData]=\"webApis\" [modules]=\"modules\" [gridOptions]=\"gridOptions\">\r\n  </ag-grid-angular>\r\n\r\n  <button mat-fab mat-elevation-z24 class=\"grid-fab\" matTooltip=\"Create a new controller\" (click)=\"addController()\">\r\n    <mat-icon>add</mat-icon>\r\n  </button>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/url-loader/dist/cjs.js!./src/app/app-administration/views/polymorph-logo.png":
/*!*************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/url-loader/dist/cjs.js!./src/app/app-administration/views/polymorph-logo.png ***!
  \*************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAOUAAADcCAYAAACPvGZzAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAFIVJREFUeNrsnVusJFUVhnftbnw2JibGhHcfNRpuE2AiIBIYZuacMzPMjCFeIBiIEYjxipAYDMIDRAlqVAKJAg4wDKCCRlSQEKPIu+HVxJiYGB9FurusXbfeXae6u6r2ba1d/5r09PV0V1Xvr/5/rbWrOknTVCAQCDohsQkQCECJQCAAJQIBKBEIBKBEIAAlAoEAlAgEoEQgEIASgQCUCASCUEyxCfpHMr0ccxMHRjp7LcFW2DK+MPcVAAJUQAkQEQAUUAJEwAkoASMCcAJKwAg4ASWARABOQAkYEfGDORooXQCZDQ4Q0m3bA05AaR9IAEgT1hjBjB5KEyABIg84YwMzaihtWVbASd/axgRmtFAOBfIjTz8hptnX++ax04DTM4xq+5pAGguYUUJpqpAXP/ukmIjiEJo/7p0CmB5grJ9PLls+MUlGCaYEkELcdtOH8ksV52UXpZbvybbO1c89uW9Quaomjg1IBeMKkNrz53/mDnH+jbcLMU+df/+AkkEOqdRRQVnAmYhD554Sh59/CnAawNgG5L4oAVRAUhgHgNIzkLoytj32ys6pHMhJqZqT7PbuCz8Xe9kFcJrB2Abktm144O5vj2a7jfLMA21Atm6cpEhrZAak2lA5oNmNk784I05nlz6DCjC+tjkf76CSfcDkrJYykkGQ2gJSf/6FIydzICdlzWEi1e3ivgL2xl+egWr2zBu7/M3YFXNUStlVIfXXKesqpajhlMlSQc/LLje9dEbc/NLTo4ezc97YBmTPXLIrmFzVUkYwGFKbQDbjzPU3rKhjBeO0srUK1uz2rS8/LW779TOjs7SDrGqLbe0bMYMpAeT2v1Ewqlwyt7BiFcbC2iaFtc3u3/GbZ0ahmsYwiv0tEMQI7evQ+Ol1J+q8UodRh7MqBKn7X/rts+LL2SVWOE1h7FPcGaNaSsYDw6ltbf7to9eeyIGblgBWt6ctapnfz1709d+dFXdll1jgHJo3mhZ3xlb4ifq8ryZAtsVSHbNBVd5PsvtJfl3dr24n9e17/nA2/5t7Du6uDEouU/a2TY0b9H6WJgr02YlzmYLHUil92hEd7B9cc7y2qtPSulbFn4lWpc2fk2WlVrO99772HCvVtJE32izujMXGRptT2lRJ/b0qq1rDWFrV3L6KMr+sLW7R29R7nfe//px44PVz5OF0AiOKO3FCGXpv99DVxwrYNOhqGOWyjynz/HIJ4/K5AugH3zgnHnqDHpw280YXxZ0xqKWMEUjbuWTzPVcmFOjAica0vESDVBSQVgpaXb73p+fFw9nFRRGEhFUNvF5QypHEd67c62RVpTbBQAdRahXcygb/8M/PB1NN1zC6VsnY1JINlCFVsu29771idwlei1WdtBWC6il6Sfk3Rfm7eu2P//KCePTNF7zB6dyqBlDJGFokUbVEXALZukdLli2RSdkmydsgomyblI/J+rn9r5VaSyUpbe7jb72Yv/+nP3r9vkFtu9jiGsb68zy3QLrs5Km2SFgoJSW7oYN/98Hd+njLVqtaKadmVfXXVlXcKidV9xNtmCg4K0BtKI6PvHGTbfUV3G1sNDmlT5VcmRcrV2FsWlXZsKqtOaYoHlsXbWD2hTMIjAItkCihpJ6Uf/XSnZXDuWRDNfVJ61IuYZxoMCcdTNQ61dwGp8+8MURxJ0a1lDEA6TuX3KeWWgVWV8JVWytWrGpXGLvCScaqBijuxFb0QUvEQtx54OgKfCZWtQ+cbRBSgDG0SnJ3Y5L7hgqhkm2f/cVLjlqzqiaqSQJGQWeiAEe1ZK2UIYFs3ZiWrSpXMCi2QDippcQGsrdj+PyFR0pVtGtV14XqY+q9zHW2NpRtpRLcij6SK5CUVHJf0ceBVW0Dcp2VtdFCsaHOaIGM0L5SjM9+7LBzGLep47p80wucRG0rJ7UkBSVHlWxbpiY0LmHcVOjxDSf1o0C4FH3YzX2lCKTLaAN8G4jr4Gy+n/VTkjAr7mwSh5DzYiWlDcEdINtqOUQZu8JpWzW5HCvJQS0l10Ef8zJ2zRtNwLQJJ8cWCGWRkGPfAL6tp4u8MTicc15fH/Wiz5QLkJxySbWsj/zkb97zRlNLOyTfRAtk5PY1RrX0qYxO8k2mtpWyWgaFMkaVbFvmNjBd540+LC33E2FRLfpMOQ3uWJWTAox9LW2MxZ1N4uGzRSJDrmjs1rWplpSsqrGlncfx9VG0sZLLoI4BTE4wdl1eFHcigXIMKqlCVWCbVVhOMK6FU1PJvz/2UH6BWjLOKWMu7ugwxhrrqskKTKjmCOxrDEBeeXCSX7YNajahFXeaEHJWTUpq6RXKmFWyzarqMMYAZttyxwQnlRYJuZYINyDbrKoOYFS2dUMLpHpMh7G6HZutdd0ikT5XJDabus2qboKVnVp2bIGsU07YWGJQxmZb+8LIXUX1HUhX1Ysp3/QdKPRYzBtt5GiUVbKvDeWab4ZWS+dQxqCSQ6xqV7WkDqaN5eMIZ8iiT1Q/hefapnK1nyFUcpOlbRaDOBeCXBR9pOsF5qqSNpWRq1q6Wi4uqhlKLYMrJTUgfaqjet9XXp3TBdLhUSAxtVBsq6V0uaDcrKprdQyhSqa21XVQb6GEKPrIkEBSUcmQMFLMUYe0QMZiaX0EWiKBYCStloEOXm5TzTGqZZKmdm0KB5WkWFXVc8uQh3e5ziX7RK2Uk6Q3IC7ijW99Y+trbOSWQZQyFJBteSNVCzk2lexiabuCETJsqKWktkA+80Zq1jm0jeUyw0iBGQJOXyrtvSXiWyU5KWPIFgkl27oJCB3G6ja1s9KZtkikzQWhpowhq6rsVIvJibAUgE0IfSqnj6KP9AmkD5XkDmOI5QzRAnEFZwwRVUuEM4xB1ZLx+VtDqKZrtTSGkoJK2j6kipJaugaT/TmDAllal3msl0KPKyDR3rAAZERnOV9XDApZCBpS9JGmHxjKpsZmVYOo5TzO0+/6Uk1XNtZ5TmlbJWOH0ZfqcyzuULe0tmIwlL5VMra8MXjuN5If53ENpwu1lC6BtKGSY7CqvtUyhuKOLTh9gknGvgJGgjCNRCVD5psmYiZdvbGJSgLG9WppCuYYVdKHpbWplk5aIkOBRIvDg9KOWCXXgeRzPm2XFgkJ+wqr6kkt5yk2pMN80xbIsi/ltlUSMA4D08S2QiXp5ZsklHLsLQ4rVnSASgJIt/lmF7XcJm5Wc8ouKom80Vwtq2MuFZhdTh2C4g6PfLO3UppOFkDeGDCgkl7zTVNovdhXwOgut9ymglBJmvnmJpGzZl/brCusqh8buxFIqCQ7S+tEKWFV/cZaNUQLJJilNYFUAsY4bOwmUKGSPCxtL/vapciD9kZ4tVypxMK2Bre0pOwrgAyjlpU6orhDw9I6VUrAyCygkqSUM4hSIm8kpJYfPoQNQjDf7BOdfuCH229NQiUji0kS5Ve17mgR/BQeAkEsppaIZ7nymQPIr0P+9JytqIs7parEsE76eg1dn0XmBOcLda1MRJobiUV5Xd1Wz820xxfZ6+ei+Lt5/ly6fF6ZkUVxPctfVzxePT8rX/9Odv373VPhcspqcCMIQooIEkOBtGpfFZiAMzyA//zgfwBm4Hh555TR31s/HUgFJldLGwuQCP9x7shJK+/TSSm7nHa9CSGUM1xALf3GmetvEGcP37CRh2BKWS2IDqO6DdX0r5Lq/gf+8V5sJNMxveG5nx06sVWcguaUUE0ayrgNXoSdePzaE/vGvD7uTca7NaVsKuI61US+aV8lN8EKteynftviR9ccd6KOzuwrLC09ldQhjqV3GSIe+eQxqzBuqtN4m9EDS+tXJdughY3tHw9+Yk989+pjG61qm2v0klN2qcBuW5i2lQGcw4FEC8RtPHDVXi8YbUWQua+AM5zFhVpuj3uv2BX3XbHnNG/0mlP2yReRb/pTSRR9tsc9B3eN88YuwmL1t0T6/nY78k33yuc6Jx1LfPPyHWOramvMOrGvQxYOltY9SLCx++Mrl+6Ir122482qdhG26ZA3dXnQM/qbblUSUcSdB44aW1UXKulMKW0s5DrlhEpCLU3iCxcfEbdfctTYqrpSycFQusotkW9uBtKWSo5VbW+96IgTGG2PR6ctEVsLi3yThwpTjZsvOCxuufCwt7zRVMimJh/SJbe02eIYW77pcqLAGFokbTsc2+PEhTCwPHHW2PJN13YzNrVU69NcJxd5Y9cx1zfdk4ZwJDYXHvmmX1BiLPr4gNF1TC2AkYQ8L2yMlhbzW2laVR8q6dW+ulYwtFDGqZa+rKpLJ2ldKfuopY95rer9OatmCJXkWPTxrYw+d/ZRniE9hhZKKNvKQS055I0mvfypzYWgopZc882QQHBQy5Dq6DMlklT2Dsg3w6skZbWkoI4uizvB7WsoICjnlRRA4FD04djiCGZf+9rYkF9qc8dg+iMyNoFEC4SOVQ2hkvlndfl9ygErkPYBhNqG9g0nNSj13DLktuAwVlykb65Oxhx0pk/faA48n9aNokqGWI51/UYK4Xuc4kdjN4DpE06qttXHNqDc4vBpW53llH1zS0onyqrA1AeJy3yTck/QR4uEWt5IJZwqJdUWSRc4fVpa6sUd2+tOeWpcaJUkY1+p9gxdWloOM2dst0i4wBhaeCTHhaagmiaDlFMLxNbycYMxpFBIbIRwcHKLIetKuapKVXAkpYXnMGHcNN/kOFFgyHJytqqhxyFaIoHyTa4zd7atI/e8MVRxJwiUManlUEvL2ep2KfrEVsQJFVOfH0Z9Xqypanbtb8Y2vzWWfiMFlcyXw8Xc1y0rTmZebPUlPPbXF0WSuBukCsxYJp1vm1Dg6oxxPsdDSCCD5JQUbewi2zGpi63906ZCEHeVXLf83K0qpbQJhZ4s5qkCM7sIu2C2WVfuJ0BuW/6x5I2+eu6S8sr52nsVSpldLxSgdlVz3cDmBue6ZY4BSGrFRShlFrMMwPmiBNOBaqr49+fen1+4qWYbjLqF5X4aTyrFHRJQUlJLBeFcLFVyvrCjmm1VyTYwqcLZBmMFZExgUotpyA+n0iKpwEuTJFNIISZCLVIi1F5DPS5lKhJ13aNEqwPZBLG6/75H/7UPAGpnHmhTx1iCokqysa+u98R1oafMLSvVrC3tYvl8zJZ2nVVdB+QY1DLEARWSy0q7/NKrPHJegqdgnJdw5vcrWBfL9slQlVwHZ0hL2xfGGBSU8k5kKhCFAmZmVe2h0npPleYGNs3trLKxaW1fc1ub/ZPqFUl32LqA6dvSbiriDBnoMbVHQh12KDmtvKu9m25dK8Wcp0tVbFPNxRrVNJ3fus7S2lbObVXVPsFNLalbbTI5ZciDoWc1jOk+OOvHsxv168pcU3++6Wj7qKTPfNPEqsYw4KkWd9gVelx/6QrEWaPgU+eRdd+yfF2jfaJPOnBxFIhN1XQJI1okkeaUoVokCqxESV1SZJKTMm+s8kqh5Zp5llm2T4SWa95ywWFrKmk73xxLiyMGlWSplC72xHlVVSzzyEo1q2prUzXr12mq6SP65puurWqMaknhnFKS60ax+aXPRVr3InPgFgWIc6FNu9PB1ItD2Y1bLzriTCWH5Ju+YeSgwJx2Epj7KoqccFZWWReLVeiaqrloUU2fQPbNN33BSBkELraVNJS+1VKvvirVnK1RzdUqbfH8HQeOBt1WbaoZWrVQ9Imo0BOq6FP0GrW5rklxX5bVHXU3L+jIvBJUPJ6kIhHhVLIKvfizrsAz5p/X46aSpKHss9FNZ5Eoiyoz6CblDJ78S1K9R6mUMhGT7OvKi62ZPKZJOXE9e+Fdl+0EW+82GPUdQ7NS6xtM/bdIqM/0oXbCcNI5pa+NVRVvZmXOWFdVF1WVNS17kcvJA3ddvhNMJZtAtllYCoeIhVZortY5irmvpnvi6ugP1W9USihLO6v+zw/lyvuSxVzX/PMC7Ve3qeO65ygcIkZVLSn+rAZ5KH38pJ5SwKRMKHMes9wxze1smWkmZR6ZT0MX4r6P73lVyb4wdoXTV4skxOFonAtMOEqktK35kSCiyB/TRbqimisHQHtsgZjC2Pa3IVXTl1pyLO6wySn7bryhe0ellLN6zqt2RMhimV9W/cn7r9ojkzcOBdN3vokWSaRK6bJFsiims+Y2ddFQzaTKL+tc061K2lZHDvkmVDJy+zrEIs0yBCdpUhZ7RHlgc5E/5ja2tLAPX3OcjVWllm9SapFQ/81UVtPsXNnY6tCraq6rsrDv6qcHya6/rwFpGxZXVnUInC4trWsFjsUao9AjRA1jro6JyFWzor9SzZjUkYKlDaWWHH5ZnB2ULlokefU1Kexr3ZMs76sPevy6E1ahoQajTzhdtUhiKiBBKUuVFPmJsIrWR92TzHuXiTVwqMPoO9+0pZYxFHfY5pSucstZNbWuOgJElC2S7MYTh05EkzdSyTfRIolUKW22SN7J3uW8Mm9UU+ny2TwZ9mePnDRWNG7qSCHfHLNKjsK+drFIs3JC+jwv8qRimpR9kRFZVd9w+myRcAKSPZS2ij7viuX0uUXxzuJXO6cGwRQrjC7yTdOiT6zWF6cDKXPKd0sbq1TznYHzWznnjaHzzb6AxWhbo4HSRtFHQfi/RVrD+eruqd5WtQ3IsYTJfNoh+WjsxaEockpTG5sfxFzmkQvRfX7rWKyqL0vbJbfsAyRHlRylfW37UmclmP/Nrt86dnqwMo4ZSBNL27VFMpb2STTV1z4tkrY9sirwvH389FbFG7NNtaGaviwrV5WMTin7fBF9v+Sx542u802bEwo4AxmVUprG2yc+1Qob8kZ/+aaNebHcgcx3SmmaRjcATGb6bAISMNqJth1dBacJlDEAGS2UJmA2z2MDIP3COXYgo4bSVDEBIx84YwJSRdQtERtfls29OQJAjl4pbasm1BOWFVASBhPB3wUBSoCJAIyAEnACSEAJOBGAEVACTsAIKAEnAjACSgAKEAElApACQkCJQCCKwImzEAhAiUAgACUCASgRCASgRCAAJQKBAJQIBKBEIBCAEoEAlAgEAlAiEAhAiUBwif8LMAC8si4r3FdHgQAAAABJRU5ErkJggg==");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.scss":
/*!************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.scss ***!
  \************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy9kYXRhLWFjdGlvbnMvZGF0YS1hY3Rpb25zLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.ts":
/*!**********************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.ts ***!
  \**********************************************************************************************/
/*! exports provided: DataActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DataActionsComponent", function() { return DataActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var DataActionsComponent = /** @class */ (function () {
    function DataActionsComponent() {
    }
    DataActionsComponent.prototype.agInit = function (params) {
        this.params = params;
        this.contentType = this.params.data;
        var enableAppFeatures = this.params.enableAppFeaturesGetter();
        this.showPermissions = enableAppFeatures && this.isGuid(this.contentType.StaticName);
    };
    DataActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    DataActionsComponent.prototype.createOrEditMetadata = function () {
        this.params.onCreateOrEditMetadata(this.contentType);
    };
    DataActionsComponent.prototype.openExport = function () {
        this.params.onOpenExport(this.contentType);
    };
    DataActionsComponent.prototype.openImport = function () {
        this.params.onOpenImport(this.contentType);
    };
    DataActionsComponent.prototype.openPermissions = function () {
        this.params.onOpenPermissions(this.contentType);
    };
    DataActionsComponent.prototype.deleteContentType = function () {
        this.params.onDelete(this.contentType);
    };
    DataActionsComponent.prototype.isGuid = function (txtToTest) {
        var patt = new RegExp(/[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i);
        return patt.test(txtToTest);
    };
    DataActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-data-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./data-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./data-actions.component.scss */ "./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.scss")).default]
        })
    ], DataActionsComponent);
    return DataActionsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.scss":
/*!**********************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.scss ***!
  \**********************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".chip-box {\n  width: 32px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vYWctZ3JpZC1jb21wb25lbnRzL2RhdGEtZmllbGRzL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxhcHAtYWRtaW5pc3RyYXRpb25cXGFnLWdyaWQtY29tcG9uZW50c1xcZGF0YS1maWVsZHNcXGRhdGEtZmllbGRzLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9hZy1ncmlkLWNvbXBvbmVudHMvZGF0YS1maWVsZHMvZGF0YS1maWVsZHMuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxXQUFBO0FDQ0YiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9hZy1ncmlkLWNvbXBvbmVudHMvZGF0YS1maWVsZHMvZGF0YS1maWVsZHMuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuY2hpcC1ib3gge1xyXG4gIHdpZHRoOiAzMnB4O1xyXG59XHJcbiIsIi5jaGlwLWJveCB7XG4gIHdpZHRoOiAzMnB4O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.ts":
/*!********************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.ts ***!
  \********************************************************************************************/
/*! exports provided: DataFieldsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DataFieldsComponent", function() { return DataFieldsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var DataFieldsComponent = /** @class */ (function () {
    function DataFieldsComponent() {
    }
    DataFieldsComponent.prototype.agInit = function (params) {
        this.contentType = params.data;
        this.value = params.value;
    };
    DataFieldsComponent.prototype.refresh = function (params) {
        return true;
    };
    DataFieldsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-data-fields',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./data-fields.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./data-fields.component.scss */ "./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.scss")).default]
        })
    ], DataFieldsComponent);
    return DataFieldsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/data-items/data-items.component.scss":
/*!********************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/data-items/data-items.component.scss ***!
  \********************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".chip-box {\n  width: 40px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vYWctZ3JpZC1jb21wb25lbnRzL2RhdGEtaXRlbXMvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXGFwcC1hZG1pbmlzdHJhdGlvblxcYWctZ3JpZC1jb21wb25lbnRzXFxkYXRhLWl0ZW1zXFxkYXRhLWl0ZW1zLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9hZy1ncmlkLWNvbXBvbmVudHMvZGF0YS1pdGVtcy9kYXRhLWl0ZW1zLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsV0FBQTtBQ0NGIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vYWctZ3JpZC1jb21wb25lbnRzL2RhdGEtaXRlbXMvZGF0YS1pdGVtcy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5jaGlwLWJveCB7XHJcbiAgd2lkdGg6IDQwcHg7XHJcbn1cclxuIiwiLmNoaXAtYm94IHtcbiAgd2lkdGg6IDQwcHg7XG59Il19 */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/data-items/data-items.component.ts":
/*!******************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/data-items/data-items.component.ts ***!
  \******************************************************************************************/
/*! exports provided: DataItemsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DataItemsComponent", function() { return DataItemsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var DataItemsComponent = /** @class */ (function () {
    function DataItemsComponent() {
    }
    DataItemsComponent.prototype.agInit = function (params) {
        this.value = params.value;
    };
    DataItemsComponent.prototype.refresh = function (params) {
        return true;
    };
    DataItemsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-data-items',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./data-items.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/data-items/data-items.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./data-items.component.scss */ "./src/app/app-administration/ag-grid-components/data-items/data-items.component.scss")).default]
        })
    ], DataItemsComponent);
    return DataItemsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.scss":
/*!******************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.scss ***!
  \******************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy9xdWVyaWVzLWFjdGlvbnMvcXVlcmllcy1hY3Rpb25zLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.ts":
/*!****************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.ts ***!
  \****************************************************************************************************/
/*! exports provided: QueriesActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "QueriesActionsComponent", function() { return QueriesActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var QueriesActionsComponent = /** @class */ (function () {
    function QueriesActionsComponent() {
    }
    QueriesActionsComponent.prototype.agInit = function (params) {
        this.params = params;
    };
    QueriesActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    QueriesActionsComponent.prototype.editQuery = function () {
        var query = this.params.data;
        this.params.onEditQuery(query);
    };
    QueriesActionsComponent.prototype.cloneQuery = function () {
        var query = this.params.data;
        this.params.onCloneQuery(query);
    };
    QueriesActionsComponent.prototype.exportQuery = function () {
        var query = this.params.data;
        this.params.onExportQuery(query);
    };
    QueriesActionsComponent.prototype.openPermissions = function () {
        var query = this.params.data;
        this.params.onOpenPermissions(query);
    };
    QueriesActionsComponent.prototype.deleteQuery = function () {
        var query = this.params.data;
        this.params.onDelete(query);
    };
    QueriesActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-queries-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./queries-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./queries-actions.component.scss */ "./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.scss")).default]
        })
    ], QueriesActionsComponent);
    return QueriesActionsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.scss":
/*!**************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.scss ***!
  \**************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy92aWV3cy1hY3Rpb25zL3ZpZXdzLWFjdGlvbnMuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.ts":
/*!************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.ts ***!
  \************************************************************************************************/
/*! exports provided: ViewsActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsActionsComponent", function() { return ViewsActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ViewsActionsComponent = /** @class */ (function () {
    function ViewsActionsComponent() {
    }
    ViewsActionsComponent.prototype.agInit = function (params) {
        this.params = params;
    };
    ViewsActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    ViewsActionsComponent.prototype.openCode = function () {
        var view = this.params.data;
        this.params.onOpenCode(view);
    };
    ViewsActionsComponent.prototype.openPermissions = function () {
        var view = this.params.data;
        this.params.onOpenPermissions(view);
    };
    ViewsActionsComponent.prototype.deleteView = function () {
        var view = this.params.data;
        this.params.onDelete(view);
    };
    ViewsActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-views-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./views-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./views-actions.component.scss */ "./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.scss")).default]
        })
    ], ViewsActionsComponent);
    return ViewsActionsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-show/views-show.component.scss":
/*!********************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-show/views-show.component.scss ***!
  \********************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy92aWV3cy1zaG93L3ZpZXdzLXNob3cuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-show/views-show.component.ts":
/*!******************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-show/views-show.component.ts ***!
  \******************************************************************************************/
/*! exports provided: ViewsShowComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsShowComponent", function() { return ViewsShowComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ViewsShowComponent = /** @class */ (function () {
    function ViewsShowComponent() {
    }
    ViewsShowComponent.prototype.agInit = function (params) {
        this.value = params.value;
    };
    ViewsShowComponent.prototype.refresh = function (params) {
        return true;
    };
    ViewsShowComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-views-show',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./views-show.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-show/views-show.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./views-show.component.scss */ "./src/app/app-administration/ag-grid-components/views-show/views-show.component.scss")).default]
        })
    ], ViewsShowComponent);
    return ViewsShowComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-type/views-type.component.scss":
/*!********************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-type/views-type.component.scss ***!
  \********************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy92aWV3cy10eXBlL3ZpZXdzLXR5cGUuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-type/views-type.component.ts":
/*!******************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-type/views-type.component.ts ***!
  \******************************************************************************************/
/*! exports provided: ViewsTypeComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsTypeComponent", function() { return ViewsTypeComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _views_views_helpers__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../views/views.helpers */ "./src/app/app-administration/views/views.helpers.ts");



var ViewsTypeComponent = /** @class */ (function () {
    function ViewsTypeComponent() {
    }
    ViewsTypeComponent.prototype.agInit = function (params) {
        this.value = params.value;
        var view = params.data;
        var type = Object(_views_views_helpers__WEBPACK_IMPORTED_MODULE_2__["calculateViewType"])(view);
        this.icon = type.icon;
    };
    ViewsTypeComponent.prototype.refresh = function (params) {
        return true;
    };
    ViewsTypeComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-views-type',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./views-type.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-type/views-type.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./views-type.component.scss */ "./src/app/app-administration/ag-grid-components/views-type/views-type.component.scss")).default]
        })
    ], ViewsTypeComponent);
    return ViewsTypeComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.scss":
/*!****************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.scss ***!
  \****************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".id-box {\n  padding: 0 8px;\n  text-align: end;\n  height: 100%;\n  display: flex;\n  align-items: center;\n  justify-content: flex-end;\n}\n.id-box .id {\n  max-width: 100%;\n  text-overflow: ellipsis;\n  overflow: hidden;\n}\n.id-box:hover {\n  text-decoration: none;\n}\n.id-box:hover .id {\n  display: none;\n}\n.id-box:not(:hover) .icon {\n  display: none;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vYWctZ3JpZC1jb21wb25lbnRzL3ZpZXdzLXVzYWdlLWlkL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxhcHAtYWRtaW5pc3RyYXRpb25cXGFnLWdyaWQtY29tcG9uZW50c1xcdmlld3MtdXNhZ2UtaWRcXHZpZXdzLXVzYWdlLWlkLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9hZy1ncmlkLWNvbXBvbmVudHMvdmlld3MtdXNhZ2UtaWQvdmlld3MtdXNhZ2UtaWQuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxjQUFBO0VBQ0EsZUFBQTtFQUNBLFlBQUE7RUFDQSxhQUFBO0VBQ0EsbUJBQUE7RUFDQSx5QkFBQTtBQ0NGO0FEQ0U7RUFDRSxlQUFBO0VBQ0EsdUJBQUE7RUFDQSxnQkFBQTtBQ0NKO0FERUU7RUFDRSxxQkFBQTtBQ0FKO0FERUk7RUFDRSxhQUFBO0FDQU47QURLSTtFQUNFLGFBQUE7QUNITiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy92aWV3cy11c2FnZS1pZC92aWV3cy11c2FnZS1pZC5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5pZC1ib3gge1xyXG4gIHBhZGRpbmc6IDAgOHB4O1xyXG4gIHRleHQtYWxpZ246IGVuZDtcclxuICBoZWlnaHQ6IDEwMCU7XHJcbiAgZGlzcGxheTogZmxleDtcclxuICBhbGlnbi1pdGVtczogY2VudGVyO1xyXG4gIGp1c3RpZnktY29udGVudDogZmxleC1lbmQ7XHJcblxyXG4gIC5pZCB7XHJcbiAgICBtYXgtd2lkdGg6IDEwMCU7XHJcbiAgICB0ZXh0LW92ZXJmbG93OiBlbGxpcHNpcztcclxuICAgIG92ZXJmbG93OiBoaWRkZW47XHJcbiAgfVxyXG5cclxuICAmOmhvdmVyIHtcclxuICAgIHRleHQtZGVjb3JhdGlvbjogbm9uZTtcclxuXHJcbiAgICAuaWQge1xyXG4gICAgICBkaXNwbGF5OiBub25lO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgJjpub3QoOmhvdmVyKSB7XHJcbiAgICAuaWNvbiB7XHJcbiAgICAgIGRpc3BsYXk6IG5vbmU7XHJcbiAgICB9XHJcbiAgfVxyXG59XHJcbiIsIi5pZC1ib3gge1xuICBwYWRkaW5nOiAwIDhweDtcbiAgdGV4dC1hbGlnbjogZW5kO1xuICBoZWlnaHQ6IDEwMCU7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XG4gIGp1c3RpZnktY29udGVudDogZmxleC1lbmQ7XG59XG4uaWQtYm94IC5pZCB7XG4gIG1heC13aWR0aDogMTAwJTtcbiAgdGV4dC1vdmVyZmxvdzogZWxsaXBzaXM7XG4gIG92ZXJmbG93OiBoaWRkZW47XG59XG4uaWQtYm94OmhvdmVyIHtcbiAgdGV4dC1kZWNvcmF0aW9uOiBub25lO1xufVxuLmlkLWJveDpob3ZlciAuaWQge1xuICBkaXNwbGF5OiBub25lO1xufVxuLmlkLWJveDpub3QoOmhvdmVyKSAuaWNvbiB7XG4gIGRpc3BsYXk6IG5vbmU7XG59Il19 */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.ts":
/*!**************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.ts ***!
  \**************************************************************************************************/
/*! exports provided: ViewsUsageIdComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsUsageIdComponent", function() { return ViewsUsageIdComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _shared_helpers_copy_to_clipboard_helper__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../shared/helpers/copy-to-clipboard.helper */ "./src/app/shared/helpers/copy-to-clipboard.helper.ts");




var ViewsUsageIdComponent = /** @class */ (function () {
    function ViewsUsageIdComponent(snackBar) {
        this.snackBar = snackBar;
    }
    ViewsUsageIdComponent.prototype.agInit = function (params) {
        this.tooltip = params.value;
        if (this.tooltip == null) {
            return;
        }
        var isMultiline = this.tooltip.indexOf('\n') !== -1;
        this.id = this.tooltip.substring(this.tooltip.indexOf(' ') + 1, isMultiline ? this.tooltip.indexOf('\n') : undefined);
    };
    ViewsUsageIdComponent.prototype.refresh = function (params) {
        return true;
    };
    ViewsUsageIdComponent.prototype.copy = function () {
        Object(_shared_helpers_copy_to_clipboard_helper__WEBPACK_IMPORTED_MODULE_3__["copyToClipboard"])(this.tooltip);
        this.snackBar.open('Copied to clipboard', null, { duration: 2000 });
    };
    ViewsUsageIdComponent.ctorParameters = function () { return [
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"] }
    ]; };
    ViewsUsageIdComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-views-usage-id',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./views-usage-id.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./views-usage-id.component.scss */ "./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"]])
    ], ViewsUsageIdComponent);
    return ViewsUsageIdComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.scss":
/*!**************************************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.scss ***!
  \**************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".title {\n  padding: 12px 12px 0;\n}\n\n.mat-radio-group {\n  display: flex;\n  flex-direction: column;\n  justify-content: space-between;\n  overflow: hidden;\n  padding: 12px;\n  width: 160px;\n  height: 104px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vYWctZ3JpZC1jb21wb25lbnRzL3ZpZXdzLXVzYWdlLXN0YXR1cy1maWx0ZXIvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXGFwcC1hZG1pbmlzdHJhdGlvblxcYWctZ3JpZC1jb21wb25lbnRzXFx2aWV3cy11c2FnZS1zdGF0dXMtZmlsdGVyXFx2aWV3cy11c2FnZS1zdGF0dXMtZmlsdGVyLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9hZy1ncmlkLWNvbXBvbmVudHMvdmlld3MtdXNhZ2Utc3RhdHVzLWZpbHRlci92aWV3cy11c2FnZS1zdGF0dXMtZmlsdGVyLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0Usb0JBQUE7QUNDRjs7QURFQTtFQUNFLGFBQUE7RUFDQSxzQkFBQTtFQUNBLDhCQUFBO0VBQ0EsZ0JBQUE7RUFDQSxhQUFBO0VBQ0EsWUFBQTtFQUNBLGFBQUE7QUNDRiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy92aWV3cy11c2FnZS1zdGF0dXMtZmlsdGVyL3ZpZXdzLXVzYWdlLXN0YXR1cy1maWx0ZXIuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIudGl0bGUge1xyXG4gIHBhZGRpbmc6IDEycHggMTJweCAwO1xyXG59XHJcblxyXG4ubWF0LXJhZGlvLWdyb3VwIHtcclxuICBkaXNwbGF5OiBmbGV4O1xyXG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XHJcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xyXG4gIG92ZXJmbG93OiBoaWRkZW47XHJcbiAgcGFkZGluZzogMTJweDtcclxuICB3aWR0aDogMTYwcHg7XHJcbiAgaGVpZ2h0OiAxMDRweDtcclxufVxyXG4iLCIudGl0bGUge1xuICBwYWRkaW5nOiAxMnB4IDEycHggMDtcbn1cblxuLm1hdC1yYWRpby1ncm91cCB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XG4gIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2VlbjtcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcbiAgcGFkZGluZzogMTJweDtcbiAgd2lkdGg6IDE2MHB4O1xuICBoZWlnaHQ6IDEwNHB4O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.ts":
/*!************************************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.ts ***!
  \************************************************************************************************************************/
/*! exports provided: ViewsUsageStatusFilterComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsUsageStatusFilterComponent", function() { return ViewsUsageStatusFilterComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ViewsUsageStatusFilterComponent = /** @class */ (function () {
    function ViewsUsageStatusFilterComponent() {
        this.isVisible = '';
        this.isDeleted = '';
    }
    ViewsUsageStatusFilterComponent.prototype.agInit = function (params) {
        this.params = params;
    };
    ViewsUsageStatusFilterComponent.prototype.isFilterActive = function () {
        return this.isVisible !== '' || this.isDeleted !== '';
    };
    ViewsUsageStatusFilterComponent.prototype.doesFilterPass = function (params) {
        var visiblePassed = false;
        var deletedPassed = false;
        var value = this.params.valueGetter(params.node);
        if (value == null) {
            return false;
        }
        if (this.isVisible !== '') {
            visiblePassed = (value.IsVisible == null) ? false : value.IsVisible.toString() === this.isVisible;
        }
        else {
            visiblePassed = true;
        }
        if (this.isDeleted !== '') {
            deletedPassed = (value.IsDeleted == null) ? false : value.IsDeleted.toString() === this.isDeleted;
        }
        else {
            deletedPassed = true;
        }
        return visiblePassed && deletedPassed;
    };
    ViewsUsageStatusFilterComponent.prototype.getModel = function () {
        if (!this.isFilterActive()) {
            return;
        }
        return {
            filterType: 'views-usage-status',
            isVisible: this.isVisible,
            isDeleted: this.isDeleted,
        };
    };
    ViewsUsageStatusFilterComponent.prototype.setModel = function (model) {
        this.isVisible = model ? model.isVisible : '';
        this.isDeleted = model ? model.isDeleted : '';
    };
    ViewsUsageStatusFilterComponent.prototype.afterGuiAttached = function (params) {
    };
    ViewsUsageStatusFilterComponent.prototype.filterChanged = function () {
        this.params.filterChangedCallback();
    };
    ViewsUsageStatusFilterComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-views-usage-status-filter',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./views-usage-status-filter.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./views-usage-status-filter.component.scss */ "./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.scss")).default]
        })
    ], ViewsUsageStatusFilterComponent);
    return ViewsUsageStatusFilterComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.scss":
/*!******************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.scss ***!
  \******************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FnLWdyaWQtY29tcG9uZW50cy93ZWItYXBpLWFjdGlvbnMvd2ViLWFwaS1hY3Rpb25zLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.ts":
/*!****************************************************************************************************!*\
  !*** ./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.ts ***!
  \****************************************************************************************************/
/*! exports provided: WebApiActionsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "WebApiActionsComponent", function() { return WebApiActionsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var WebApiActionsComponent = /** @class */ (function () {
    function WebApiActionsComponent() {
    }
    WebApiActionsComponent.prototype.agInit = function (params) {
        this.params = params;
    };
    WebApiActionsComponent.prototype.refresh = function (params) {
        return true;
    };
    WebApiActionsComponent.prototype.openCode = function () {
        var api = this.params.data;
        this.params.onOpenCode(api);
    };
    WebApiActionsComponent.prototype.deleteApi = function () {
        var api = this.params.data;
        this.params.onDelete(api);
    };
    WebApiActionsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-web-api-actions',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./web-api-actions.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./web-api-actions.component.scss */ "./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.scss")).default]
        })
    ], WebApiActionsComponent);
    return WebApiActionsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/app-administration-nav/app-administration-dialog.config.ts":
/*!***********************************************************************************************!*\
  !*** ./src/app/app-administration/app-administration-nav/app-administration-dialog.config.ts ***!
  \***********************************************************************************************/
/*! exports provided: appAdministrationDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "appAdministrationDialog", function() { return appAdministrationDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var appAdministrationDialog = {
    name: 'APP_ADMINISTRATION_DIALOG',
    initContext: true,
    panelSize: 'large',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var AppAdministrationNavComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./app-administration-nav.component */ "./src/app/app-administration/app-administration-nav/app-administration-nav.component.ts"))];
                    case 1:
                        AppAdministrationNavComponent = (_a.sent()).AppAdministrationNavComponent;
                        return [2 /*return*/, AppAdministrationNavComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/app-administration-nav/app-administration-nav.component.scss":
/*!*************************************************************************************************!*\
  !*** ./src/app/app-administration/app-administration-nav/app-administration-nav.component.scss ***!
  \*************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FwcC1hZG1pbmlzdHJhdGlvbi1uYXYvYXBwLWFkbWluaXN0cmF0aW9uLW5hdi5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/app-administration/app-administration-nav/app-administration-nav.component.ts":
/*!***********************************************************************************************!*\
  !*** ./src/app/app-administration/app-administration-nav/app-administration-nav.component.ts ***!
  \***********************************************************************************************/
/*! exports provided: AppAdministrationNavComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppAdministrationNavComponent", function() { return AppAdministrationNavComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _apps_management_services_apps_list_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../apps-management/services/apps-list.service */ "./src/app/apps-management/services/apps-list.service.ts");
/* harmony import */ var _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../services/app-dialog-config.service */ "./src/app/app-administration/services/app-dialog-config.service.ts");
/* harmony import */ var _edit_shared_services_global_configuration_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../../../../../edit/shared/services/global-configuration.service */ "../edit/shared/services/global-configuration.service.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");










var AppAdministrationNavComponent = /** @class */ (function () {
    function AppAdministrationNavComponent(dialogRef, appsListService, appDialogConfigService, router, route, globalConfigurationService, context) {
        this.dialogRef = dialogRef;
        this.appsListService = appsListService;
        this.appDialogConfigService = appDialogConfigService;
        this.router = router;
        this.route = route;
        this.globalConfigurationService = globalConfigurationService;
        this.context = context;
        this.tabs = ['home', 'data', 'queries', 'views', 'web-api', 'app']; // tabs have to match template and filter below
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_4__["Subscription"]();
    }
    AppAdministrationNavComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.appsListService.getAll().subscribe(function (apps) {
            _this.app = apps.find(function (app) { return app.Id === _this.context.appId; });
        });
        this.appDialogConfigService.getDialogSettings().subscribe(function (dialogSettings) {
            _this.context.appRoot = dialogSettings.AppPath;
            if (dialogSettings.IsContent) {
                _this.tabs = _this.tabs.filter(function (tab) { return !(tab === 'queries' || tab === 'web-api'); });
            }
            _this.tabIndex = _this.tabs.indexOf(_this.route.snapshot.firstChild.url[0].path); // set tab initially
            _this.dialogSettings = dialogSettings; // needed to filter tabs
        });
        this.subscription.add(
        // change tab when route changed
        this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_5__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            _this.tabIndex = _this.tabs.indexOf(_this.route.snapshot.firstChild.url[0].path);
        }));
    };
    AppAdministrationNavComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
        this.subscription = null;
    };
    AppAdministrationNavComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    AppAdministrationNavComponent.prototype.changeTab = function (event) {
        var path = this.tabs[event.index];
        this.router.navigate([path], { relativeTo: this.route });
    };
    AppAdministrationNavComponent.prototype.toggleDebugEnabled = function (event) {
        var enableDebugEvent = (navigator.platform.match('Mac') ? event.metaKey : event.ctrlKey) && event.shiftKey && event.altKey;
        if (enableDebugEvent) {
            this.globalConfigurationService.toggleDebugEnabled();
        }
    };
    AppAdministrationNavComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _apps_management_services_apps_list_service__WEBPACK_IMPORTED_MODULE_6__["AppsListService"] },
        { type: _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_7__["AppDialogConfigService"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _edit_shared_services_global_configuration_service__WEBPACK_IMPORTED_MODULE_8__["GlobalConfigurationService"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_9__["Context"] }
    ]; };
    AppAdministrationNavComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-app-administration-nav',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./app-administration-nav.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/app-administration-nav/app-administration-nav.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./app-administration-nav.component.scss */ "./src/app/app-administration/app-administration-nav/app-administration-nav.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _apps_management_services_apps_list_service__WEBPACK_IMPORTED_MODULE_6__["AppsListService"],
            _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_7__["AppDialogConfigService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _edit_shared_services_global_configuration_service__WEBPACK_IMPORTED_MODULE_8__["GlobalConfigurationService"],
            _shared_services_context__WEBPACK_IMPORTED_MODULE_9__["Context"]])
    ], AppAdministrationNavComponent);
    return AppAdministrationNavComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/app-administration-routing.module.ts":
/*!*************************************************************************!*\
  !*** ./src/app/app-administration/app-administration-routing.module.ts ***!
  \*************************************************************************/
/*! exports provided: AppAdministrationRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppAdministrationRoutingModule", function() { return AppAdministrationRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../shared/components/dialog-entry/dialog-entry.component */ "./src/app/shared/components/dialog-entry/dialog-entry.component.ts");
/* harmony import */ var _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../shared/components/empty-route/empty-route.component */ "./src/app/shared/components/empty-route/empty-route.component.ts");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../edit/edit.matcher */ "../edit/edit.matcher.ts");
/* harmony import */ var _app_administration_nav_app_administration_dialog_config__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./app-administration-nav/app-administration-dialog.config */ "./src/app/app-administration/app-administration-nav/app-administration-dialog.config.ts");
/* harmony import */ var _sub_dialogs_edit_content_type_edit_content_type_dialog_config__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./sub-dialogs/edit-content-type/edit-content-type-dialog.config */ "./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type-dialog.config.ts");
/* harmony import */ var _sub_dialogs_content_import_content_import_dialog_config__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./sub-dialogs/content-import/content-import-dialog.config */ "./src/app/app-administration/sub-dialogs/content-import/content-import-dialog.config.ts");
/* harmony import */ var _sub_dialogs_import_query_import_query_dialog_config__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./sub-dialogs/import-query/import-query-dialog.config */ "./src/app/app-administration/sub-dialogs/import-query/import-query-dialog.config.ts");
/* harmony import */ var _sub_dialogs_export_app_export_app_dialog_config__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./sub-dialogs/export-app/export-app-dialog.config */ "./src/app/app-administration/sub-dialogs/export-app/export-app-dialog.config.ts");
/* harmony import */ var _sub_dialogs_export_app_parts_export_app_parts_dialog_config__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./sub-dialogs/export-app-parts/export-app-parts-dialog.config */ "./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts-dialog.config.ts");
/* harmony import */ var _sub_dialogs_import_app_parts_import_app_parts_dialog_config__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./sub-dialogs/import-app-parts/import-app-parts-dialog.config */ "./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts-dialog.config.ts");
/* harmony import */ var _sub_dialogs_views_usage_views_usage_dialog_config__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./sub-dialogs/views-usage/views-usage-dialog.config */ "./src/app/app-administration/sub-dialogs/views-usage/views-usage-dialog.config.ts");














var appAdministrationRoutes = [
    {
        path: '', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _app_administration_nav_app_administration_dialog_config__WEBPACK_IMPORTED_MODULE_6__["appAdministrationDialog"] }, children: [
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], data: { title: 'App Home' } },
            {
                path: 'data', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], children: [
                    {
                        path: 'items/:contentTypeStaticName',
                        loadChildren: function () { return Promise.all(/*! import() | content-items-content-items-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~33e631e1"), __webpack_require__.e("common"), __webpack_require__.e("content-items-content-items-module")]).then(__webpack_require__.bind(null, /*! ../content-items/content-items.module */ "./src/app/content-items/content-items.module.ts")).then(function (m) { return m.ContentItemsModule; }); }
                    },
                    {
                        matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                        loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); }
                    },
                    {
                        path: ':scope/add',
                        component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"],
                        data: { dialog: _sub_dialogs_edit_content_type_edit_content_type_dialog_config__WEBPACK_IMPORTED_MODULE_7__["editContentTypeDialog"], title: 'Add Content Type' },
                    },
                    {
                        path: ':scope/:id/edit',
                        component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"],
                        data: { dialog: _sub_dialogs_edit_content_type_edit_content_type_dialog_config__WEBPACK_IMPORTED_MODULE_7__["editContentTypeDialog"], title: 'Edit Content Type' },
                    },
                    {
                        path: 'fields/:contentTypeStaticName',
                        loadChildren: function () { return Promise.all(/*! import() | content-type-fields-content-type-fields-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("content-type-fields-content-type-fields-module")]).then(__webpack_require__.bind(null, /*! ../content-type-fields/content-type-fields.module */ "./src/app/content-type-fields/content-type-fields.module.ts")).then(function (m) { return m.ContentTypeFieldsModule; }); },
                        data: { title: 'Content Type Fields' },
                    },
                    {
                        path: 'export/:contentTypeStaticName',
                        loadChildren: function () { return Promise.all(/*! import() | content-export-content-export-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("common"), __webpack_require__.e("content-export-content-export-module")]).then(__webpack_require__.bind(null, /*! ../content-export/content-export.module */ "./src/app/content-export/content-export.module.ts")).then(function (m) { return m.ContentExportModule; }); },
                        data: { title: 'Export Items' },
                    },
                    {
                        path: ':contentTypeStaticName/import',
                        component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"],
                        data: { dialog: _sub_dialogs_content_import_content_import_dialog_config__WEBPACK_IMPORTED_MODULE_8__["contentImportDialog"], title: 'Import Items' },
                    },
                    {
                        path: 'permissions/:type/:keyType/:key',
                        loadChildren: function () { return Promise.all(/*! import() | permissions-permissions-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("permissions-permissions-module")]).then(__webpack_require__.bind(null, /*! ../permissions/permissions.module */ "./src/app/permissions/permissions.module.ts")).then(function (m) { return m.PermissionsModule; }); },
                        data: { title: 'Permission' },
                    },
                ],
                data: { title: 'App Data' },
            },
            {
                path: 'queries', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], children: [
                    {
                        path: 'import',
                        component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"],
                        data: { dialog: _sub_dialogs_import_query_import_query_dialog_config__WEBPACK_IMPORTED_MODULE_9__["importQueryDialog"], title: 'Import Query' }
                    },
                    {
                        matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                        loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); },
                        data: { title: 'Edit Query Name and Description' },
                    },
                    {
                        path: 'permissions/:type/:keyType/:key',
                        loadChildren: function () { return Promise.all(/*! import() | permissions-permissions-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("permissions-permissions-module")]).then(__webpack_require__.bind(null, /*! ../permissions/permissions.module */ "./src/app/permissions/permissions.module.ts")).then(function (m) { return m.PermissionsModule; }); },
                        data: { title: 'Query Permissions' },
                    },
                ],
                data: { title: 'App Queries' },
            },
            {
                path: 'views', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], children: [
                    { path: 'usage/:guid', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _sub_dialogs_views_usage_views_usage_dialog_config__WEBPACK_IMPORTED_MODULE_13__["viewsUsageDialog"] } },
                    {
                        matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                        loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); },
                        data: { title: 'Edit View' },
                    },
                    {
                        path: 'permissions/:type/:keyType/:key',
                        loadChildren: function () { return Promise.all(/*! import() | permissions-permissions-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("permissions-permissions-module")]).then(__webpack_require__.bind(null, /*! ../permissions/permissions.module */ "./src/app/permissions/permissions.module.ts")).then(function (m) { return m.PermissionsModule; }); },
                        data: { title: 'View Permissions' },
                    },
                ],
                data: { title: 'App Views' },
            },
            { path: 'web-api', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], data: { title: 'App WebApi' }, },
            {
                path: 'app', component: _shared_components_empty_route_empty_route_component__WEBPACK_IMPORTED_MODULE_4__["EmptyRouteComponent"], children: [
                    {
                        matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_5__["edit"],
                        loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); },
                        data: { title: 'Edit App Properties' },
                    },
                    {
                        path: 'fields/:contentTypeStaticName',
                        loadChildren: function () { return Promise.all(/*! import() | content-type-fields-content-type-fields-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("content-type-fields-content-type-fields-module")]).then(__webpack_require__.bind(null, /*! ../content-type-fields/content-type-fields.module */ "./src/app/content-type-fields/content-type-fields.module.ts")).then(function (m) { return m.ContentTypeFieldsModule; }); },
                        data: { title: 'Edit Fields of App Settings & Resources' },
                    },
                    {
                        path: 'permissions/:type/:keyType/:key',
                        loadChildren: function () { return Promise.all(/*! import() | permissions-permissions-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~fd907a9b"), __webpack_require__.e("common"), __webpack_require__.e("permissions-permissions-module")]).then(__webpack_require__.bind(null, /*! ../permissions/permissions.module */ "./src/app/permissions/permissions.module.ts")).then(function (m) { return m.PermissionsModule; }); },
                        data: { title: 'App Permission' },
                    },
                    { path: 'export', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _sub_dialogs_export_app_export_app_dialog_config__WEBPACK_IMPORTED_MODULE_10__["exportAppDialog"], title: 'Export App' } },
                    { path: 'export/parts', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _sub_dialogs_export_app_parts_export_app_parts_dialog_config__WEBPACK_IMPORTED_MODULE_11__["exportAppPartsDialog"], title: 'Export App Parts' } },
                    { path: 'import/parts', component: _shared_components_dialog_entry_dialog_entry_component__WEBPACK_IMPORTED_MODULE_3__["DialogEntryComponent"], data: { dialog: _sub_dialogs_import_app_parts_import_app_parts_dialog_config__WEBPACK_IMPORTED_MODULE_12__["importAppPartsDialog"], title: 'Import App Parts' } },
                ],
                data: { title: 'Manage App' },
            },
        ]
    },
];
var AppAdministrationRoutingModule = /** @class */ (function () {
    function AppAdministrationRoutingModule() {
    }
    AppAdministrationRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(appAdministrationRoutes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], AppAdministrationRoutingModule);
    return AppAdministrationRoutingModule;
}());



/***/ }),

/***/ "./src/app/app-administration/app-administration.module.ts":
/*!*****************************************************************!*\
  !*** ./src/app/app-administration/app-administration.module.ts ***!
  \*****************************************************************/
/*! exports provided: AppAdministrationModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppAdministrationModule", function() { return AppAdministrationModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @ag-grid-community/angular */ "../../node_modules/@ag-grid-community/angular/__ivy_ngcc__/fesm5/ag-grid-community-angular.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_tabs__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/tabs */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tabs.js");
/* harmony import */ var _angular_material_input__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/input */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/input.js");
/* harmony import */ var _angular_material_select__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/select */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/select.js");
/* harmony import */ var _angular_material_radio__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/radio */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/radio.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/material/checkbox */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/checkbox.js");
/* harmony import */ var _angular_material_expansion__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! @angular/material/expansion */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/expansion.js");
/* harmony import */ var _angular_material_card__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @angular/material/card */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/card.js");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! @angular/material/core */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! @angular/material/progress-spinner */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/progress-spinner.js");
/* harmony import */ var _ecodev_fab_speed_dial__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! @ecodev/fab-speed-dial */ "../../node_modules/@ecodev/fab-speed-dial/__ivy_ngcc__/fesm5/ecodev-fab-speed-dial.js");
/* harmony import */ var _app_administration_nav_app_administration_nav_component__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ./app-administration-nav/app-administration-nav.component */ "./src/app/app-administration/app-administration-nav/app-administration-nav.component.ts");
/* harmony import */ var _getting_started_getting_started_component__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./getting-started/getting-started.component */ "./src/app/app-administration/getting-started/getting-started.component.ts");
/* harmony import */ var _data_data_component__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ./data/data.component */ "./src/app/app-administration/data/data.component.ts");
/* harmony import */ var _queries_queries_component__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ./queries/queries.component */ "./src/app/app-administration/queries/queries.component.ts");
/* harmony import */ var _views_views_component__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ./views/views.component */ "./src/app/app-administration/views/views.component.ts");
/* harmony import */ var _web_api_web_api_component__WEBPACK_IMPORTED_MODULE_24__ = __webpack_require__(/*! ./web-api/web-api.component */ "./src/app/app-administration/web-api/web-api.component.ts");
/* harmony import */ var _app_configuration_app_configuration_component__WEBPACK_IMPORTED_MODULE_25__ = __webpack_require__(/*! ./app-configuration/app-configuration.component */ "./src/app/app-administration/app-configuration/app-configuration.component.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_26__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _app_administration_routing_module__WEBPACK_IMPORTED_MODULE_27__ = __webpack_require__(/*! ./app-administration-routing.module */ "./src/app/app-administration/app-administration-routing.module.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_28__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _ag_grid_components_data_items_data_items_component__WEBPACK_IMPORTED_MODULE_29__ = __webpack_require__(/*! ./ag-grid-components/data-items/data-items.component */ "./src/app/app-administration/ag-grid-components/data-items/data-items.component.ts");
/* harmony import */ var _ag_grid_components_data_fields_data_fields_component__WEBPACK_IMPORTED_MODULE_30__ = __webpack_require__(/*! ./ag-grid-components/data-fields/data-fields.component */ "./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.ts");
/* harmony import */ var _ag_grid_components_data_actions_data_actions_component__WEBPACK_IMPORTED_MODULE_31__ = __webpack_require__(/*! ./ag-grid-components/data-actions/data-actions.component */ "./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.ts");
/* harmony import */ var _ag_grid_components_queries_actions_queries_actions_component__WEBPACK_IMPORTED_MODULE_32__ = __webpack_require__(/*! ./ag-grid-components/queries-actions/queries-actions.component */ "./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.ts");
/* harmony import */ var _ag_grid_components_views_type_views_type_component__WEBPACK_IMPORTED_MODULE_33__ = __webpack_require__(/*! ./ag-grid-components/views-type/views-type.component */ "./src/app/app-administration/ag-grid-components/views-type/views-type.component.ts");
/* harmony import */ var _ag_grid_components_views_show_views_show_component__WEBPACK_IMPORTED_MODULE_34__ = __webpack_require__(/*! ./ag-grid-components/views-show/views-show.component */ "./src/app/app-administration/ag-grid-components/views-show/views-show.component.ts");
/* harmony import */ var _ag_grid_components_views_actions_views_actions_component__WEBPACK_IMPORTED_MODULE_35__ = __webpack_require__(/*! ./ag-grid-components/views-actions/views-actions.component */ "./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.ts");
/* harmony import */ var _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_36__ = __webpack_require__(/*! ./services/app-dialog-config.service */ "./src/app/app-administration/services/app-dialog-config.service.ts");
/* harmony import */ var _services_content_types_service__WEBPACK_IMPORTED_MODULE_37__ = __webpack_require__(/*! ./services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _services_pipelines_service__WEBPACK_IMPORTED_MODULE_38__ = __webpack_require__(/*! ./services/pipelines.service */ "./src/app/app-administration/services/pipelines.service.ts");
/* harmony import */ var _services_views_service__WEBPACK_IMPORTED_MODULE_39__ = __webpack_require__(/*! ./services/views.service */ "./src/app/app-administration/services/views.service.ts");
/* harmony import */ var _sub_dialogs_edit_content_type_edit_content_type_component__WEBPACK_IMPORTED_MODULE_40__ = __webpack_require__(/*! ./sub-dialogs/edit-content-type/edit-content-type.component */ "./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.ts");
/* harmony import */ var _services_content_export_service__WEBPACK_IMPORTED_MODULE_41__ = __webpack_require__(/*! ./services/content-export.service */ "./src/app/app-administration/services/content-export.service.ts");
/* harmony import */ var _sub_dialogs_content_import_content_import_component__WEBPACK_IMPORTED_MODULE_42__ = __webpack_require__(/*! ./sub-dialogs/content-import/content-import.component */ "./src/app/app-administration/sub-dialogs/content-import/content-import.component.ts");
/* harmony import */ var _services_content_import_service__WEBPACK_IMPORTED_MODULE_43__ = __webpack_require__(/*! ./services/content-import.service */ "./src/app/app-administration/services/content-import.service.ts");
/* harmony import */ var _sub_dialogs_import_query_import_query_component__WEBPACK_IMPORTED_MODULE_44__ = __webpack_require__(/*! ./sub-dialogs/import-query/import-query.component */ "./src/app/app-administration/sub-dialogs/import-query/import-query.component.ts");
/* harmony import */ var _services_web_apis_service__WEBPACK_IMPORTED_MODULE_45__ = __webpack_require__(/*! ./services/web-apis.service */ "./src/app/app-administration/services/web-apis.service.ts");
/* harmony import */ var _content_items_services_content_items_service__WEBPACK_IMPORTED_MODULE_46__ = __webpack_require__(/*! ../content-items/services/content-items.service */ "./src/app/content-items/services/content-items.service.ts");
/* harmony import */ var _sub_dialogs_export_app_export_app_component__WEBPACK_IMPORTED_MODULE_47__ = __webpack_require__(/*! ./sub-dialogs/export-app/export-app.component */ "./src/app/app-administration/sub-dialogs/export-app/export-app.component.ts");
/* harmony import */ var _sub_dialogs_export_app_parts_export_app_parts_component__WEBPACK_IMPORTED_MODULE_48__ = __webpack_require__(/*! ./sub-dialogs/export-app-parts/export-app-parts.component */ "./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.ts");
/* harmony import */ var _sub_dialogs_import_app_parts_import_app_parts_component__WEBPACK_IMPORTED_MODULE_49__ = __webpack_require__(/*! ./sub-dialogs/import-app-parts/import-app-parts.component */ "./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.ts");
/* harmony import */ var _services_export_app_service__WEBPACK_IMPORTED_MODULE_50__ = __webpack_require__(/*! ./services/export-app.service */ "./src/app/app-administration/services/export-app.service.ts");
/* harmony import */ var _services_export_app_parts_service__WEBPACK_IMPORTED_MODULE_51__ = __webpack_require__(/*! ./services/export-app-parts.service */ "./src/app/app-administration/services/export-app-parts.service.ts");
/* harmony import */ var _services_import_app_parts_service__WEBPACK_IMPORTED_MODULE_52__ = __webpack_require__(/*! ./services/import-app-parts.service */ "./src/app/app-administration/services/import-app-parts.service.ts");
/* harmony import */ var _ag_grid_components_web_api_actions_web_api_actions_component__WEBPACK_IMPORTED_MODULE_53__ = __webpack_require__(/*! ./ag-grid-components/web-api-actions/web-api-actions.component */ "./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.ts");
/* harmony import */ var _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_54__ = __webpack_require__(/*! ../../../../edit/eav-material-controls/adam/sanitize.service */ "../edit/eav-material-controls/adam/sanitize.service.ts");
/* harmony import */ var _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_55__ = __webpack_require__(/*! ../shared/services/dialog.service */ "./src/app/shared/services/dialog.service.ts");
/* harmony import */ var _sub_dialogs_views_usage_views_usage_component__WEBPACK_IMPORTED_MODULE_56__ = __webpack_require__(/*! ./sub-dialogs/views-usage/views-usage.component */ "./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.ts");
/* harmony import */ var _ag_grid_components_views_usage_id_views_usage_id_component__WEBPACK_IMPORTED_MODULE_57__ = __webpack_require__(/*! ./ag-grid-components/views-usage-id/views-usage-id.component */ "./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.ts");
/* harmony import */ var _ag_grid_components_views_usage_status_filter_views_usage_status_filter_component__WEBPACK_IMPORTED_MODULE_58__ = __webpack_require__(/*! ./ag-grid-components/views-usage-status-filter/views-usage-status-filter.component */ "./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.ts");
/* harmony import */ var _apps_management_services_apps_list_service__WEBPACK_IMPORTED_MODULE_59__ = __webpack_require__(/*! ../apps-management/services/apps-list.service */ "./src/app/apps-management/services/apps-list.service.ts");




























































var AppAdministrationModule = /** @class */ (function () {
    function AppAdministrationModule() {
    }
    AppAdministrationModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _getting_started_getting_started_component__WEBPACK_IMPORTED_MODULE_20__["GettingStartedComponent"],
                _app_administration_nav_app_administration_nav_component__WEBPACK_IMPORTED_MODULE_19__["AppAdministrationNavComponent"],
                _data_data_component__WEBPACK_IMPORTED_MODULE_21__["DataComponent"],
                _queries_queries_component__WEBPACK_IMPORTED_MODULE_22__["QueriesComponent"],
                _views_views_component__WEBPACK_IMPORTED_MODULE_23__["ViewsComponent"],
                _web_api_web_api_component__WEBPACK_IMPORTED_MODULE_24__["WebApiComponent"],
                _app_configuration_app_configuration_component__WEBPACK_IMPORTED_MODULE_25__["AppConfigurationComponent"],
                _ag_grid_components_data_items_data_items_component__WEBPACK_IMPORTED_MODULE_29__["DataItemsComponent"],
                _ag_grid_components_data_fields_data_fields_component__WEBPACK_IMPORTED_MODULE_30__["DataFieldsComponent"],
                _ag_grid_components_data_actions_data_actions_component__WEBPACK_IMPORTED_MODULE_31__["DataActionsComponent"],
                _ag_grid_components_queries_actions_queries_actions_component__WEBPACK_IMPORTED_MODULE_32__["QueriesActionsComponent"],
                _ag_grid_components_views_type_views_type_component__WEBPACK_IMPORTED_MODULE_33__["ViewsTypeComponent"],
                _ag_grid_components_views_show_views_show_component__WEBPACK_IMPORTED_MODULE_34__["ViewsShowComponent"],
                _ag_grid_components_views_actions_views_actions_component__WEBPACK_IMPORTED_MODULE_35__["ViewsActionsComponent"],
                _sub_dialogs_edit_content_type_edit_content_type_component__WEBPACK_IMPORTED_MODULE_40__["EditContentTypeComponent"],
                _sub_dialogs_content_import_content_import_component__WEBPACK_IMPORTED_MODULE_42__["ContentImportComponent"],
                _sub_dialogs_import_query_import_query_component__WEBPACK_IMPORTED_MODULE_44__["ImportQueryComponent"],
                _sub_dialogs_export_app_export_app_component__WEBPACK_IMPORTED_MODULE_47__["ExportAppComponent"],
                _sub_dialogs_export_app_parts_export_app_parts_component__WEBPACK_IMPORTED_MODULE_48__["ExportAppPartsComponent"],
                _sub_dialogs_import_app_parts_import_app_parts_component__WEBPACK_IMPORTED_MODULE_49__["ImportAppPartsComponent"],
                _ag_grid_components_web_api_actions_web_api_actions_component__WEBPACK_IMPORTED_MODULE_53__["WebApiActionsComponent"],
                _sub_dialogs_views_usage_views_usage_component__WEBPACK_IMPORTED_MODULE_56__["ViewsUsageComponent"],
                _ag_grid_components_views_usage_id_views_usage_id_component__WEBPACK_IMPORTED_MODULE_57__["ViewsUsageIdComponent"],
                _ag_grid_components_views_usage_status_filter_views_usage_status_filter_component__WEBPACK_IMPORTED_MODULE_58__["ViewsUsageStatusFilterComponent"],
            ],
            entryComponents: [
                _app_administration_nav_app_administration_nav_component__WEBPACK_IMPORTED_MODULE_19__["AppAdministrationNavComponent"],
                _ag_grid_components_data_items_data_items_component__WEBPACK_IMPORTED_MODULE_29__["DataItemsComponent"],
                _ag_grid_components_data_fields_data_fields_component__WEBPACK_IMPORTED_MODULE_30__["DataFieldsComponent"],
                _ag_grid_components_data_actions_data_actions_component__WEBPACK_IMPORTED_MODULE_31__["DataActionsComponent"],
                _ag_grid_components_queries_actions_queries_actions_component__WEBPACK_IMPORTED_MODULE_32__["QueriesActionsComponent"],
                _ag_grid_components_views_type_views_type_component__WEBPACK_IMPORTED_MODULE_33__["ViewsTypeComponent"],
                _ag_grid_components_views_show_views_show_component__WEBPACK_IMPORTED_MODULE_34__["ViewsShowComponent"],
                _ag_grid_components_views_actions_views_actions_component__WEBPACK_IMPORTED_MODULE_35__["ViewsActionsComponent"],
                _sub_dialogs_edit_content_type_edit_content_type_component__WEBPACK_IMPORTED_MODULE_40__["EditContentTypeComponent"],
                _sub_dialogs_content_import_content_import_component__WEBPACK_IMPORTED_MODULE_42__["ContentImportComponent"],
                _sub_dialogs_import_query_import_query_component__WEBPACK_IMPORTED_MODULE_44__["ImportQueryComponent"],
                _sub_dialogs_export_app_export_app_component__WEBPACK_IMPORTED_MODULE_47__["ExportAppComponent"],
                _sub_dialogs_export_app_parts_export_app_parts_component__WEBPACK_IMPORTED_MODULE_48__["ExportAppPartsComponent"],
                _sub_dialogs_import_app_parts_import_app_parts_component__WEBPACK_IMPORTED_MODULE_49__["ImportAppPartsComponent"],
                _ag_grid_components_web_api_actions_web_api_actions_component__WEBPACK_IMPORTED_MODULE_53__["WebApiActionsComponent"],
                _sub_dialogs_views_usage_views_usage_component__WEBPACK_IMPORTED_MODULE_56__["ViewsUsageComponent"],
                _ag_grid_components_views_usage_id_views_usage_id_component__WEBPACK_IMPORTED_MODULE_57__["ViewsUsageIdComponent"],
                _ag_grid_components_views_usage_status_filter_views_usage_status_filter_component__WEBPACK_IMPORTED_MODULE_58__["ViewsUsageStatusFilterComponent"],
            ],
            imports: [
                _app_administration_routing_module__WEBPACK_IMPORTED_MODULE_27__["AppAdministrationRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_28__["SharedComponentsModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_12__["MatDialogModule"],
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_7__["MatTooltipModule"],
                _ag_grid_community_angular__WEBPACK_IMPORTED_MODULE_3__["AgGridModule"].withComponents([]),
                _angular_material_tabs__WEBPACK_IMPORTED_MODULE_8__["MatTabsModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_4__["FormsModule"],
                _angular_material_input__WEBPACK_IMPORTED_MODULE_9__["MatInputModule"],
                _angular_material_select__WEBPACK_IMPORTED_MODULE_10__["MatSelectModule"],
                _angular_material_radio__WEBPACK_IMPORTED_MODULE_11__["MatRadioModule"],
                _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_17__["MatProgressSpinnerModule"],
                _angular_material_checkbox__WEBPACK_IMPORTED_MODULE_13__["MatCheckboxModule"],
                _angular_material_expansion__WEBPACK_IMPORTED_MODULE_14__["MatExpansionModule"],
                _angular_material_card__WEBPACK_IMPORTED_MODULE_15__["MatCardModule"],
                _angular_material_core__WEBPACK_IMPORTED_MODULE_16__["MatRippleModule"],
                _ecodev_fab_speed_dial__WEBPACK_IMPORTED_MODULE_18__["EcoFabSpeedDialModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_26__["Context"],
                _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_36__["AppDialogConfigService"],
                _services_content_types_service__WEBPACK_IMPORTED_MODULE_37__["ContentTypesService"],
                _services_pipelines_service__WEBPACK_IMPORTED_MODULE_38__["PipelinesService"],
                _services_views_service__WEBPACK_IMPORTED_MODULE_39__["ViewsService"],
                _services_content_export_service__WEBPACK_IMPORTED_MODULE_41__["ContentExportService"],
                _services_content_import_service__WEBPACK_IMPORTED_MODULE_43__["ContentImportService"],
                _services_web_apis_service__WEBPACK_IMPORTED_MODULE_45__["WebApisService"],
                _content_items_services_content_items_service__WEBPACK_IMPORTED_MODULE_46__["ContentItemsService"],
                _services_export_app_service__WEBPACK_IMPORTED_MODULE_50__["ExportAppService"],
                _services_export_app_parts_service__WEBPACK_IMPORTED_MODULE_51__["ExportAppPartsService"],
                _services_import_app_parts_service__WEBPACK_IMPORTED_MODULE_52__["ImportAppPartsService"],
                _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_54__["SanitizeService"],
                _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_55__["DialogService"],
                _apps_management_services_apps_list_service__WEBPACK_IMPORTED_MODULE_59__["AppsListService"],
            ]
        })
    ], AppAdministrationModule);
    return AppAdministrationModule;
}());



/***/ }),

/***/ "./src/app/app-administration/app-configuration/app-configuration.component.scss":
/*!***************************************************************************************!*\
  !*** ./src/app/app-administration/app-configuration/app-configuration.component.scss ***!
  \***************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2FwcC1jb25maWd1cmF0aW9uL2FwcC1jb25maWd1cmF0aW9uLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/app-administration/app-configuration/app-configuration.component.ts":
/*!*************************************************************************************!*\
  !*** ./src/app/app-administration/app-configuration/app-configuration.component.ts ***!
  \*************************************************************************************/
/*! exports provided: AppConfigurationComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppConfigurationComponent", function() { return AppConfigurationComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _content_items_services_content_items_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../content-items/services/content-items.service */ "./src/app/content-items/services/content-items.service.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../services/app-dialog-config.service */ "./src/app/app-administration/services/app-dialog-config.service.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");








var AppConfigurationComponent = /** @class */ (function () {
    function AppConfigurationComponent(contentItemsService, router, route, context, appDialogConfigService) {
        this.contentItemsService = contentItemsService;
        this.router = router;
        this.route = route;
        this.context = context;
        this.appDialogConfigService = appDialogConfigService;
        this.eavConstants = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_3__["eavConstants"];
        this.showPermissions = false;
    }
    AppConfigurationComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.appDialogConfigService.getDialogSettings().subscribe(function (dialogSettings) {
            _this.showPermissions = !dialogSettings.IsContent;
        });
    };
    AppConfigurationComponent.prototype.edit = function (staticName) {
        var _this = this;
        this.contentItemsService.getAll(staticName).subscribe(function (contentItems) {
            if (contentItems.length !== 1) {
                throw new Error("Found too many settings for the type " + staticName);
            }
            var item = contentItems[0];
            var form = {
                items: [{ EntityId: item.Id }],
            };
            var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_7__["convertFormToUrl"])(form);
            _this.router.navigate(["edit/" + formUrl], { relativeTo: _this.route.firstChild });
        });
    };
    AppConfigurationComponent.prototype.config = function (staticName) {
        this.router.navigate(["fields/" + staticName], { relativeTo: this.route.firstChild });
    };
    AppConfigurationComponent.prototype.openPermissions = function () {
        this.router.navigate(["permissions/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_3__["eavConstants"].metadata.app.type + "/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_3__["eavConstants"].keyTypes.number + "/" + this.context.appId], { relativeTo: this.route.firstChild });
    };
    AppConfigurationComponent.prototype.exportApp = function () {
        this.router.navigate(["export"], { relativeTo: this.route.firstChild });
    };
    AppConfigurationComponent.prototype.exportParts = function () {
        this.router.navigate(["export/parts"], { relativeTo: this.route.firstChild });
    };
    AppConfigurationComponent.prototype.importParts = function () {
        this.router.navigate(["import/parts"], { relativeTo: this.route.firstChild });
    };
    AppConfigurationComponent.ctorParameters = function () { return [
        { type: _content_items_services_content_items_service__WEBPACK_IMPORTED_MODULE_4__["ContentItemsService"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"] },
        { type: _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_6__["AppDialogConfigService"] }
    ]; };
    AppConfigurationComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-app-configuration',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./app-configuration.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/app-configuration/app-configuration.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./app-configuration.component.scss */ "./src/app/app-administration/app-configuration/app-configuration.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_content_items_services_content_items_service__WEBPACK_IMPORTED_MODULE_4__["ContentItemsService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"],
            _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_6__["AppDialogConfigService"]])
    ], AppConfigurationComponent);
    return AppConfigurationComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/constants/content-type.patterns.ts":
/*!***********************************************************************!*\
  !*** ./src/app/app-administration/constants/content-type.patterns.ts ***!
  \***********************************************************************/
/*! exports provided: contentTypeNamePattern, contentTypeNameError */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentTypeNamePattern", function() { return contentTypeNamePattern; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentTypeNameError", function() { return contentTypeNameError; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

/**
 * The pattern determines what content-type names are allowed.
 * Basically it's A-Z and numbers after the first digit.
 * But there are two exceptions:
 *  - types describing an input-type begin with an `@` and can also contain `-` chars
 *  - types beginning with an `|` are very old type names for data-sources, they can contain anything!
 */
var contentTypeNamePattern = /(^[A-Za-z][A-Za-z0-9]+$)|(^@[A-Za-z][A-Za-z0-9-]*$)/;
var contentTypeNameError = 'Standard letters and numbers are allowed. Must start with a letter.';
// 2020-04-29 2dm - temporarily used this pattern while renaming unique named types containing '|' chars
// export const contentTypeNamePattern = /(^[A-Za-z][A-Za-z0-9]+$)|(^@[A-Za-z][A-Za-z0-9-]*$)|(^\|.*$)/;


/***/ }),

/***/ "./src/app/app-administration/data/data.component.scss":
/*!*************************************************************!*\
  !*** ./src/app/app-administration/data/data.component.scss ***!
  \*************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".scope-box {\n  margin-right: 66px;\n  margin-left: 8px;\n  display: flex;\n  align-items: flex-end;\n  overflow: hidden;\n}\n.scope-box__dropdown {\n  width: 200px;\n  margin-top: -14px;\n  font-size: 14px;\n  height: auto;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vZGF0YS9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcYXBwLWFkbWluaXN0cmF0aW9uXFxkYXRhXFxkYXRhLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9kYXRhL2RhdGEuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxrQkFBQTtFQUNBLGdCQUFBO0VBQ0EsYUFBQTtFQUNBLHFCQUFBO0VBQ0EsZ0JBQUE7QUNDRjtBRENFO0VBQ0UsWUFBQTtFQUNBLGlCQUFBO0VBQ0EsZUFBQTtFQUNBLFlBQUE7QUNDSiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2RhdGEvZGF0YS5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5zY29wZS1ib3gge1xyXG4gIG1hcmdpbi1yaWdodDogNjZweDtcclxuICBtYXJnaW4tbGVmdDogOHB4O1xyXG4gIGRpc3BsYXk6IGZsZXg7XHJcbiAgYWxpZ24taXRlbXM6IGZsZXgtZW5kO1xyXG4gIG92ZXJmbG93OiBoaWRkZW47XHJcblxyXG4gICZfX2Ryb3Bkb3duIHtcclxuICAgIHdpZHRoOiAyMDBweDtcclxuICAgIG1hcmdpbi10b3A6IC0xNHB4O1xyXG4gICAgZm9udC1zaXplOiAxNHB4O1xyXG4gICAgaGVpZ2h0OiBhdXRvO1xyXG4gIH1cclxufVxyXG4iLCIuc2NvcGUtYm94IHtcbiAgbWFyZ2luLXJpZ2h0OiA2NnB4O1xuICBtYXJnaW4tbGVmdDogOHB4O1xuICBkaXNwbGF5OiBmbGV4O1xuICBhbGlnbi1pdGVtczogZmxleC1lbmQ7XG4gIG92ZXJmbG93OiBoaWRkZW47XG59XG4uc2NvcGUtYm94X19kcm9wZG93biB7XG4gIHdpZHRoOiAyMDBweDtcbiAgbWFyZ2luLXRvcDogLTE0cHg7XG4gIGZvbnQtc2l6ZTogMTRweDtcbiAgaGVpZ2h0OiBhdXRvO1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/app-administration/data/data.component.ts":
/*!***********************************************************!*\
  !*** ./src/app/app-administration/data/data.component.ts ***!
  \***********************************************************/
/*! exports provided: DataComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DataComponent", function() { return DataComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _services_content_types_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _ag_grid_components_data_items_data_items_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../ag-grid-components/data-items/data-items.component */ "./src/app/app-administration/ag-grid-components/data-items/data-items.component.ts");
/* harmony import */ var _ag_grid_components_data_fields_data_fields_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../ag-grid-components/data-fields/data-fields.component */ "./src/app/app-administration/ag-grid-components/data-fields/data-fields.component.ts");
/* harmony import */ var _ag_grid_components_data_actions_data_actions_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../ag-grid-components/data-actions/data-actions.component */ "./src/app/app-administration/ag-grid-components/data-actions/data-actions.component.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _edit_shared_services_global_configuration_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../../../../../edit/shared/services/global-configuration.service */ "../edit/shared/services/global-configuration.service.ts");
/* harmony import */ var _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../services/app-dialog-config.service */ "./src/app/app-administration/services/app-dialog-config.service.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ../../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");

















var DataComponent = /** @class */ (function () {
    function DataComponent(router, route, contentTypesService, globalConfigurationService, appDialogConfigService, snackBar) {
        var _this = this;
        this.router = router;
        this.route = route;
        this.contentTypesService = contentTypesService;
        this.globalConfigurationService = globalConfigurationService;
        this.appDialogConfigService = appDialogConfigService;
        this.snackBar = snackBar;
        this.debugEnabled = false;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_6__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_15__["defaultGridOptions"]), { frameworkComponents: {
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_14__["IdFieldComponent"],
                dataItemsComponent: _ag_grid_components_data_items_data_items_component__WEBPACK_IMPORTED_MODULE_8__["DataItemsComponent"],
                dataFieldsComponent: _ag_grid_components_data_fields_data_fields_component__WEBPACK_IMPORTED_MODULE_9__["DataFieldsComponent"],
                dataActionsComponent: _ag_grid_components_data_actions_data_actions_component__WEBPACK_IMPORTED_MODULE_10__["DataActionsComponent"],
            }, columnDefs: [
                {
                    headerName: 'ID', field: 'Id', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
                },
                {
                    headerName: 'Content Type', field: 'Label', flex: 3, minWidth: 250, cellClass: 'primary-action highlight', sort: 'asc',
                    sortable: true, filter: 'agTextColumnFilter', onCellClicked: this.showContentItems.bind(this),
                },
                {
                    headerName: 'Items', field: 'Items', width: 102, headerClass: 'dense', cellClass: 'secondary-action no-padding',
                    sortable: true, filter: 'agNumberColumnFilter', cellRenderer: 'dataItemsComponent', onCellClicked: this.addItem.bind(this),
                },
                {
                    headerName: 'Fields', field: 'Fields', width: 94, headerClass: 'dense', cellClass: 'secondary-action no-padding',
                    sortable: true, filter: 'agNumberColumnFilter', cellRenderer: 'dataFieldsComponent', onCellClicked: this.editFields.bind(this),
                },
                {
                    width: 200, cellClass: 'secondary-action no-padding', cellRenderer: 'dataActionsComponent',
                    cellRendererParams: {
                        enableAppFeaturesGetter: this.enableAppFeaturesGetter.bind(this),
                        onCreateOrEditMetadata: this.createOrEditMetadata.bind(this),
                        onOpenExport: this.openExport.bind(this),
                        onOpenImport: this.openImport.bind(this),
                        onOpenPermissions: this.openPermissions.bind(this),
                        onDelete: this.deleteContentType.bind(this),
                    },
                },
                {
                    headerName: 'Name', field: 'Name', flex: 1, minWidth: 100, cellClass: this.nameCellClassGetter.bind(this),
                    sortable: true, filter: 'agTextColumnFilter', onCellClicked: function (event) { _this.editContentType(event.data); },
                },
                {
                    headerName: 'Description', field: 'Metadata.Description', flex: 3, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
            ] });
        this.enableAppFeatures = false;
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_4__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild.firstChild;
        this.scope = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__["eavConstants"].scopes.default.value;
        this.defaultScope = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__["eavConstants"].scopes.default.value;
    }
    DataComponent.prototype.ngOnInit = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var dialogSettings;
            var _this = this;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.appDialogConfigService.getDialogSettings().toPromise()];
                    case 1:
                        dialogSettings = _a.sent();
                        this.enableAppFeatures = !dialogSettings.IsContent;
                        this.fetchScopes();
                        this.fetchContentTypes();
                        this.refreshOnChildClosed();
                        this.subscription.add(this.globalConfigurationService.getDebugEnabled().subscribe(function (debugEnabled) {
                            _this.debugEnabled = debugEnabled;
                        }));
                        return [2 /*return*/];
                }
            });
        });
    };
    DataComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    DataComponent.prototype.showContentItems = function (params) {
        var contentType = params.data;
        this.router.navigate(["items/" + contentType.StaticName], { relativeTo: this.route.firstChild });
    };
    DataComponent.prototype.editContentType = function (contentType) {
        if (!contentType) {
            this.router.navigate([this.scope + "/add"], { relativeTo: this.route.firstChild });
        }
        else {
            if (contentType.UsesSharedDef) {
                return;
            }
            this.router.navigate([this.scope + "/" + contentType.Id + "/edit"], { relativeTo: this.route.firstChild });
        }
    };
    DataComponent.prototype.fetchContentTypes = function () {
        var _this = this;
        this.contentTypesService.retrieveContentTypes(this.scope).subscribe(function (contentTypes) {
            _this.contentTypes = contentTypes;
        });
    };
    DataComponent.prototype.fetchScopes = function () {
        var _this = this;
        this.contentTypesService.getScopes().subscribe(function (scopes) {
            _this.scopeOptions = scopes;
        });
    };
    DataComponent.prototype.createGhost = function () {
        var _this = this;
        var sourceName = window.prompt('To create a ghost content-type enter source static name / id - this is a very advanced operation - read more about it on 2sxc.org/help?tag=ghost');
        if (!sourceName) {
            return;
        }
        this.snackBar.open('Saving...');
        this.contentTypesService.createGhost(sourceName).subscribe(function (res) {
            _this.snackBar.open('Saved', null, { duration: 2000 });
            _this.fetchContentTypes();
        });
    };
    DataComponent.prototype.changeScope = function (event) {
        var newScope = event.value;
        if (newScope === 'Other') {
            newScope = prompt('This is an advanced feature to show content-types of another scope. Don\'t use this if you don\'t know what you\'re doing, as content-types of other scopes are usually hidden for a good reason.');
            if (!newScope) {
                newScope = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__["eavConstants"].scopes.default.value;
            }
            else if (!this.scopeOptions.find(function (option) { return option.value === newScope; })) {
                var newScopeOption = {
                    name: newScope,
                    value: newScope,
                };
                this.scopeOptions.push(newScopeOption);
            }
        }
        this.scope = newScope;
        this.fetchContentTypes();
        if (this.scope !== this.defaultScope) {
            this.snackBar.open('Warning! You are in a special scope. Changing things here could easily break functionality', null, { duration: 2000 });
        }
    };
    DataComponent.prototype.idValueGetter = function (params) {
        var contentType = params.data;
        return "ID: " + contentType.Id + "\nGUID: " + contentType.StaticName;
    };
    DataComponent.prototype.enableAppFeaturesGetter = function () {
        return this.enableAppFeatures;
    };
    DataComponent.prototype.nameCellClassGetter = function (params) {
        var contentType = params.data;
        if (contentType.UsesSharedDef) {
            return 'disabled';
        }
        else {
            return 'primary-action highlight';
        }
    };
    DataComponent.prototype.addItem = function (params) {
        var contentType = params.data;
        var form = {
            items: [{ ContentTypeName: contentType.StaticName }],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_16__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route.firstChild });
    };
    DataComponent.prototype.editFields = function (params) {
        var contentType = params.data;
        if (contentType.UsesSharedDef) {
            return;
        }
        this.router.navigate(["fields/" + contentType.StaticName], { relativeTo: this.route.firstChild });
    };
    DataComponent.prototype.createOrEditMetadata = function (contentType) {
        var form = {
            items: [
                !contentType.Metadata
                    ? {
                        ContentTypeName: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__["eavConstants"].contentTypes.contentType,
                        For: {
                            Target: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__["eavConstants"].metadata.contentType.target,
                            String: contentType.StaticName,
                        },
                        Prefill: { Label: contentType.Name, Description: contentType.Description },
                    }
                    : { EntityId: contentType.Metadata.Id }
            ],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_16__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route.firstChild });
    };
    DataComponent.prototype.openExport = function (contentType) {
        this.router.navigate(["export/" + contentType.StaticName], { relativeTo: this.route.firstChild });
    };
    DataComponent.prototype.openImport = function (contentType) {
        this.router.navigate([contentType.StaticName + "/import"], { relativeTo: this.route.firstChild });
    };
    DataComponent.prototype.openPermissions = function (contentType) {
        this.router.navigate(["permissions/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__["eavConstants"].metadata.entity.type + "/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_11__["eavConstants"].keyTypes.guid + "/" + contentType.StaticName], { relativeTo: this.route.firstChild });
    };
    DataComponent.prototype.deleteContentType = function (contentType) {
        var _this = this;
        if (!confirm("Are you sure you want to delete '" + contentType.Name + "' (" + contentType.Id + ")?")) {
            return;
        }
        this.snackBar.open('Deleting...');
        this.contentTypesService.delete(contentType).subscribe(function (result) {
            _this.snackBar.open('Deleted', null, { duration: 2000 });
            _this.fetchContentTypes();
        });
    };
    DataComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_5__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchContentTypes();
            }
        }));
    };
    DataComponent.ctorParameters = function () { return [
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_content_types_service__WEBPACK_IMPORTED_MODULE_7__["ContentTypesService"] },
        { type: _edit_shared_services_global_configuration_service__WEBPACK_IMPORTED_MODULE_12__["GlobalConfigurationService"] },
        { type: _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_13__["AppDialogConfigService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__["MatSnackBar"] }
    ]; };
    DataComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-data',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./data.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/data/data.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./data.component.scss */ "./src/app/app-administration/data/data.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_content_types_service__WEBPACK_IMPORTED_MODULE_7__["ContentTypesService"],
            _edit_shared_services_global_configuration_service__WEBPACK_IMPORTED_MODULE_12__["GlobalConfigurationService"],
            _services_app_dialog_config_service__WEBPACK_IMPORTED_MODULE_13__["AppDialogConfigService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__["MatSnackBar"]])
    ], DataComponent);
    return DataComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/getting-started/getting-started.component.scss":
/*!***********************************************************************************!*\
  !*** ./src/app/app-administration/getting-started/getting-started.component.scss ***!
  \***********************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".iframe {\n  border: none;\n  width: 100%;\n  height: calc(100vh - 209px);\n}\n@media (max-width: 600px) {\n  .iframe {\n    height: calc(100vh - 161px);\n  }\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vZ2V0dGluZy1zdGFydGVkL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxhcHAtYWRtaW5pc3RyYXRpb25cXGdldHRpbmctc3RhcnRlZFxcZ2V0dGluZy1zdGFydGVkLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9nZXR0aW5nLXN0YXJ0ZWQvZ2V0dGluZy1zdGFydGVkLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsWUFBQTtFQUNBLFdBQUE7RUFDQSwyQkFBQTtBQ0NGO0FEQ0U7RUFMRjtJQU1JLDJCQUFBO0VDRUY7QUFDRiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL2dldHRpbmctc3RhcnRlZC9nZXR0aW5nLXN0YXJ0ZWQuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuaWZyYW1lIHtcclxuICBib3JkZXI6IG5vbmU7XHJcbiAgd2lkdGg6IDEwMCU7XHJcbiAgaGVpZ2h0OiBjYWxjKDEwMHZoIC0gMjA5cHgpO1xyXG5cclxuICBAbWVkaWEgKG1heC13aWR0aDogNjAwcHgpIHtcclxuICAgIGhlaWdodDogY2FsYygxMDB2aCAtIDE2MXB4KTtcclxuICB9XHJcbn1cclxuIiwiLmlmcmFtZSB7XG4gIGJvcmRlcjogbm9uZTtcbiAgd2lkdGg6IDEwMCU7XG4gIGhlaWdodDogY2FsYygxMDB2aCAtIDIwOXB4KTtcbn1cbkBtZWRpYSAobWF4LXdpZHRoOiA2MDBweCkge1xuICAuaWZyYW1lIHtcbiAgICBoZWlnaHQ6IGNhbGMoMTAwdmggLSAxNjFweCk7XG4gIH1cbn0iXX0= */");

/***/ }),

/***/ "./src/app/app-administration/getting-started/getting-started.component.ts":
/*!*********************************************************************************!*\
  !*** ./src/app/app-administration/getting-started/getting-started.component.ts ***!
  \*********************************************************************************/
/*! exports provided: GettingStartedComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "GettingStartedComponent", function() { return GettingStartedComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/platform-browser.js");



var GettingStartedComponent = /** @class */ (function () {
    function GettingStartedComponent(sanitizer) {
        this.sanitizer = sanitizer;
    }
    GettingStartedComponent.prototype.ngOnInit = function () {
        this.gettingStartedSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.gettingStartedUrl);
    };
    GettingStartedComponent.ctorParameters = function () { return [
        { type: _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["DomSanitizer"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", String)
    ], GettingStartedComponent.prototype, "gettingStartedUrl", void 0);
    GettingStartedComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-getting-started',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./getting-started.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/getting-started/getting-started.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./getting-started.component.scss */ "./src/app/app-administration/getting-started/getting-started.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["DomSanitizer"]])
    ], GettingStartedComponent);
    return GettingStartedComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/models/content-type.model.ts":
/*!*****************************************************************!*\
  !*** ./src/app/app-administration/models/content-type.model.ts ***!
  \*****************************************************************/
/*! exports provided: ContentType, ContentTypeMetadata, ContentTypeEdit */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentType", function() { return ContentType; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeMetadata", function() { return ContentTypeMetadata; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentTypeEdit", function() { return ContentTypeEdit; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var ContentType = /** @class */ (function () {
    function ContentType() {
    }
    return ContentType;
}());

var ContentTypeMetadata = /** @class */ (function () {
    function ContentTypeMetadata() {
    }
    return ContentTypeMetadata;
}());

var ContentTypeEdit = /** @class */ (function (_super) {
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__extends"])(ContentTypeEdit, _super);
    function ContentTypeEdit() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContentTypeEdit;
}(ContentType));



/***/ }),

/***/ "./src/app/app-administration/queries/queries.component.scss":
/*!*******************************************************************!*\
  !*** ./src/app/app-administration/queries/queries.component.scss ***!
  \*******************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL3F1ZXJpZXMvcXVlcmllcy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/app-administration/queries/queries.component.ts":
/*!*****************************************************************!*\
  !*** ./src/app/app-administration/queries/queries.component.ts ***!
  \*****************************************************************/
/*! exports provided: QueriesComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "QueriesComponent", function() { return QueriesComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _ag_grid_components_queries_actions_queries_actions_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../ag-grid-components/queries-actions/queries-actions.component */ "./src/app/app-administration/ag-grid-components/queries-actions/queries-actions.component.ts");
/* harmony import */ var _services_pipelines_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../services/pipelines.service */ "./src/app/app-administration/services/pipelines.service.ts");
/* harmony import */ var _services_content_export_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../services/content-export.service */ "./src/app/app-administration/services/content-export.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../../shared/services/dialog.service */ "./src/app/shared/services/dialog.service.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");















var QueriesComponent = /** @class */ (function () {
    function QueriesComponent(router, route, pipelinesService, contentExportService, snackBar, dialogService) {
        this.router = router;
        this.route = route;
        this.pipelinesService = pipelinesService;
        this.contentExportService = contentExportService;
        this.snackBar = snackBar;
        this.dialogService = dialogService;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_6__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_13__["defaultGridOptions"]), { frameworkComponents: {
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_11__["IdFieldComponent"],
                queriesActionsComponent: _ag_grid_components_queries_actions_queries_actions_component__WEBPACK_IMPORTED_MODULE_7__["QueriesActionsComponent"],
            }, columnDefs: [
                {
                    headerName: 'ID', field: 'Id', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
                },
                {
                    headerName: 'Name', field: 'Name', flex: 2, minWidth: 250, cellClass: 'primary-action highlight', sortable: true,
                    filter: 'agTextColumnFilter', onCellClicked: this.openVisualQueryDesigner.bind(this),
                },
                {
                    width: 200, cellClass: 'secondary-action no-padding',
                    cellRenderer: 'queriesActionsComponent', cellRendererParams: {
                        onEditQuery: this.editQuery.bind(this),
                        onCloneQuery: this.cloneQuery.bind(this),
                        onOpenPermissions: this.openPermissions.bind(this),
                        onExportQuery: this.exportQuery.bind(this),
                        onDelete: this.deleteQuery.bind(this),
                    },
                },
                {
                    headerName: 'Description', field: 'Description', flex: 2, minWidth: 250, cellClass: 'no-outline', sortable: true,
                    filter: 'agTextColumnFilter',
                },
            ] });
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_4__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild.firstChild;
    }
    QueriesComponent.prototype.ngOnInit = function () {
        this.fetchQueries();
        this.refreshOnChildClosed();
    };
    QueriesComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    QueriesComponent.prototype.fetchQueries = function () {
        var _this = this;
        this.pipelinesService.getAll(_shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].contentTypes.query).subscribe(function (queries) {
            _this.queries = queries;
        });
    };
    QueriesComponent.prototype.importQuery = function () {
        this.router.navigate(['import'], { relativeTo: this.route.firstChild });
    };
    QueriesComponent.prototype.editQuery = function (query) {
        var form = {
            items: [
                query == null
                    ? {
                        ContentTypeName: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].contentTypes.query,
                        Prefill: { TestParameters: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].pipelineDesigner.testParameters }
                    }
                    : { EntityId: query.Id }
            ],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_14__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route.firstChild });
    };
    QueriesComponent.prototype.idValueGetter = function (params) {
        var query = params.data;
        return "ID: " + query.Id + "\nGUID: " + query.Guid;
    };
    QueriesComponent.prototype.openVisualQueryDesigner = function (params) {
        var query = params.data;
        this.dialogService.openQueryDesigner(query.Id);
    };
    QueriesComponent.prototype.cloneQuery = function (query) {
        var _this = this;
        this.snackBar.open('Copying...');
        this.pipelinesService.clonePipeline(query.Id).subscribe(function () {
            _this.snackBar.open('Copied', null, { duration: 2000 });
            _this.fetchQueries();
        });
    };
    QueriesComponent.prototype.openPermissions = function (query) {
        this.router.navigate(["permissions/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].metadata.entity.type + "/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_10__["eavConstants"].keyTypes.guid + "/" + query.Guid], { relativeTo: this.route.firstChild });
    };
    QueriesComponent.prototype.exportQuery = function (query) {
        this.contentExportService.exportEntity(query.Id, 'Query', true);
    };
    QueriesComponent.prototype.deleteQuery = function (query) {
        var _this = this;
        if (!confirm("Delete Pipeline '" + query.Name + "' (" + query.Id + ")?")) {
            return;
        }
        this.snackBar.open('Deleting...');
        this.pipelinesService.delete(query.Id).subscribe(function (res) {
            _this.snackBar.open('Deleted', null, { duration: 2000 });
            _this.fetchQueries();
        });
    };
    QueriesComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_5__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchQueries();
            }
        }));
    };
    QueriesComponent.ctorParameters = function () { return [
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_pipelines_service__WEBPACK_IMPORTED_MODULE_8__["PipelinesService"] },
        { type: _services_content_export_service__WEBPACK_IMPORTED_MODULE_9__["ContentExportService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__["MatSnackBar"] },
        { type: _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_12__["DialogService"] }
    ]; };
    QueriesComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-queries',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./queries.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/queries/queries.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./queries.component.scss */ "./src/app/app-administration/queries/queries.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_pipelines_service__WEBPACK_IMPORTED_MODULE_8__["PipelinesService"],
            _services_content_export_service__WEBPACK_IMPORTED_MODULE_9__["ContentExportService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__["MatSnackBar"],
            _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_12__["DialogService"]])
    ], QueriesComponent);
    return QueriesComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/services/app-dialog-config.service.ts":
/*!**************************************************************************!*\
  !*** ./src/app/app-administration/services/app-dialog-config.service.ts ***!
  \**************************************************************************/
/*! exports provided: AppDialogConfigService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppDialogConfigService", function() { return AppDialogConfigService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var AppDialogConfigService = /** @class */ (function () {
    function AppDialogConfigService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    AppDialogConfigService.prototype.getDialogSettings = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/system/dialogsettings'), {
            params: { appid: this.context.appId.toString() },
        });
    };
    AppDialogConfigService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    AppDialogConfigService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], AppDialogConfigService);
    return AppDialogConfigService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/content-import.service.ts":
/*!***********************************************************************!*\
  !*** ./src/app/app-administration/services/content-import.service.ts ***!
  \***********************************************************************/
/*! exports provided: ContentImportService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentImportService", function() { return ContentImportService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _shared_helpers_file_to_base64_helper__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/helpers/file-to-base64.helper */ "./src/app/shared/helpers/file-to-base64.helper.ts");






var ContentImportService = /** @class */ (function () {
    function ContentImportService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ContentImportService.prototype.evaluateContent = function (formValues) {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var requestData, _a;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _a = {
                            AppId: this.context.appId.toString(),
                            DefaultLanguage: formValues.defaultLanguage,
                            ContentType: formValues.contentType
                        };
                        return [4 /*yield*/, Object(_shared_helpers_file_to_base64_helper__WEBPACK_IMPORTED_MODULE_5__["toBase64"])(formValues.file)];
                    case 1:
                        requestData = (_a.ContentBase64 = _b.sent(),
                            _a.ResourcesReferences = formValues.resourcesReferences,
                            _a.ClearEntities = formValues.clearEntities,
                            _a);
                        return [2 /*return*/, (this.http.post(this.dnnContext.$2sxc.http.apiUrl('eav/ContentImport/EvaluateContent'), requestData))];
                }
            });
        });
    };
    ContentImportService.prototype.importContent = function (formValues) {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var requestData, _a;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _a = {
                            AppId: this.context.appId.toString(),
                            DefaultLanguage: formValues.defaultLanguage,
                            ContentType: formValues.contentType
                        };
                        return [4 /*yield*/, Object(_shared_helpers_file_to_base64_helper__WEBPACK_IMPORTED_MODULE_5__["toBase64"])(formValues.file)];
                    case 1:
                        requestData = (_a.ContentBase64 = _b.sent(),
                            _a.ResourcesReferences = formValues.resourcesReferences,
                            _a.ClearEntities = formValues.clearEntities,
                            _a);
                        return [2 /*return*/, (this.http.post(this.dnnContext.$2sxc.http.apiUrl('eav/ContentImport/ImportContent'), requestData))];
                }
            });
        });
    };
    ContentImportService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ContentImportService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ContentImportService);
    return ContentImportService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/export-app-parts.service.ts":
/*!*************************************************************************!*\
  !*** ./src/app/app-administration/services/export-app-parts.service.ts ***!
  \*************************************************************************/
/*! exports provided: ExportAppPartsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExportAppPartsService", function() { return ExportAppPartsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var ExportAppPartsService = /** @class */ (function () {
    function ExportAppPartsService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ExportAppPartsService.prototype.getContentInfo = function (scope) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/ImportExport/GetContentInfo'), {
            params: { appid: this.context.appId.toString(), zoneId: this.context.zoneId.toString(), scope: scope },
        });
    };
    ExportAppPartsService.prototype.exportParts = function (contentTypeIds, entityIds, templateIds) {
        window.open(this.dnnContext.$2sxc.http.apiUrl('app-sys/ImportExport/ExportContent')
            + '?appId=' + this.context.appId.toString()
            + '&zoneId=' + this.context.zoneId.toString()
            + '&contentTypeIdsString=' + contentTypeIds.join(';')
            + '&entityIdsString=' + entityIds.join(';')
            + '&templateIdsString=' + templateIds.join(';'), '_self', '');
    };
    ExportAppPartsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ExportAppPartsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ExportAppPartsService);
    return ExportAppPartsService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/export-app.service.ts":
/*!*******************************************************************!*\
  !*** ./src/app/app-administration/services/export-app.service.ts ***!
  \*******************************************************************/
/*! exports provided: ExportAppService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExportAppService", function() { return ExportAppService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var ExportAppService = /** @class */ (function () {
    function ExportAppService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ExportAppService.prototype.getAppInfo = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/ImportExport/GetAppInfo'), {
            params: { appid: this.context.appId.toString(), zoneId: this.context.zoneId.toString() },
        });
    };
    ExportAppService.prototype.exportApp = function (includeContentGroups, resetAppGuid) {
        var url = this.dnnContext.$2sxc.http.apiUrl('app-sys/ImportExport/ExportApp')
            + '?appId=' + this.context.appId
            + '&zoneId=' + this.context.zoneId
            + '&includeContentGroups=' + includeContentGroups
            + '&resetAppGuid=' + resetAppGuid;
        window.open(url, '_self', '');
    };
    ExportAppService.prototype.exportForVersionControl = function (includeContentGroups, resetAppGuid) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/ImportExport/ExportForVersionControl'), {
            params: {
                appid: this.context.appId.toString(),
                zoneId: this.context.zoneId.toString(),
                includeContentGroups: includeContentGroups.toString(),
                resetAppGuid: resetAppGuid.toString(),
            },
        });
    };
    ExportAppService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ExportAppService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ExportAppService);
    return ExportAppService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/import-app-parts.service.ts":
/*!*************************************************************************!*\
  !*** ./src/app/app-administration/services/import-app-parts.service.ts ***!
  \*************************************************************************/
/*! exports provided: ImportAppPartsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ImportAppPartsService", function() { return ImportAppPartsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var ImportAppPartsService = /** @class */ (function () {
    function ImportAppPartsService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ImportAppPartsService.prototype.importAppParts = function (file) {
        var formData = new FormData();
        formData.append('AppId', this.context.appId.toString());
        formData.append('ZoneId', this.context.zoneId.toString());
        formData.append('File', file);
        return (this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/ImportExport/ImportContent'), formData));
    };
    ImportAppPartsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ImportAppPartsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ImportAppPartsService);
    return ImportAppPartsService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/pipelines.service.ts":
/*!******************************************************************!*\
  !*** ./src/app/app-administration/services/pipelines.service.ts ***!
  \******************************************************************/
/*! exports provided: PipelinesService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PipelinesService", function() { return PipelinesService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _shared_helpers_file_to_base64_helper__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/helpers/file-to-base64.helper */ "./src/app/shared/helpers/file-to-base64.helper.ts");






var PipelinesService = /** @class */ (function () {
    function PipelinesService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    PipelinesService.prototype.getAll = function (contentType) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/Entities/GetEntities'), {
            params: { appId: this.context.appId.toString(), contentType: contentType }
        });
    };
    PipelinesService.prototype.importQuery = function (file) {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _a, _b, _c, _d;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_e) {
                switch (_e.label) {
                    case 0:
                        _b = (_a = this.http).post;
                        _c = [this.dnnContext.$2sxc.http.apiUrl('eav/pipelinedesigner/importquery')];
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
    PipelinesService.prototype.clonePipeline = function (id) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/PipelineDesigner/ClonePipeline'), {
            params: { Id: id.toString(), appId: this.context.appId.toString() }
        });
    };
    PipelinesService.prototype.delete = function (id) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/PipelineDesigner/DeletePipeline'), {
            params: { appId: this.context.appId.toString(), Id: id.toString() },
        });
    };
    PipelinesService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    PipelinesService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], PipelinesService);
    return PipelinesService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/views.service.ts":
/*!**************************************************************!*\
  !*** ./src/app/app-administration/services/views.service.ts ***!
  \**************************************************************/
/*! exports provided: ViewsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsService", function() { return ViewsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var ViewsService = /** @class */ (function () {
    function ViewsService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    ViewsService.prototype.getAll = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/template/getall'), {
            params: { appId: this.context.appId.toString() }
        });
    };
    ViewsService.prototype.delete = function (id) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/template/delete'), {
            params: { appId: this.context.appId.toString(), Id: id.toString() },
        });
    };
    ViewsService.prototype.getPolymorphism = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/template/polymorphism'), {
            params: { appId: this.context.appId.toString() }
        });
    };
    ViewsService.prototype.getUsage = function (guid) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/template/usage'), {
            params: { appId: this.context.appId.toString(), guid: guid }
        });
    };
    ViewsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    ViewsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], ViewsService);
    return ViewsService;
}());



/***/ }),

/***/ "./src/app/app-administration/services/web-apis.service.ts":
/*!*****************************************************************!*\
  !*** ./src/app/app-administration/services/web-apis.service.ts ***!
  \*****************************************************************/
/*! exports provided: WebApisService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "WebApisService", function() { return WebApisService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");





var WebApisService = /** @class */ (function () {
    function WebApisService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    WebApisService.prototype.getAll = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/appassets/list'), {
            params: { appId: this.context.appId.toString(), path: '', mask: '*Controller.cs', withSubfolders: 'true' },
        });
    };
    WebApisService.prototype.create = function (name) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/appassets/create'), {}, {
            params: { appId: this.context.appId.toString(), global: 'false', path: "api/" + name },
        });
    };
    WebApisService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    WebApisService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_4__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], WebApisService);
    return WebApisService;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/content-import/content-import-dialog.config.ts":
/*!***********************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/content-import/content-import-dialog.config.ts ***!
  \***********************************************************************************************/
/*! exports provided: contentImportDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "contentImportDialog", function() { return contentImportDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var contentImportDialog = {
    name: 'IMPORT_CONTENT_TYPE_DIALOG',
    initContext: false,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ContentImportComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./content-import.component */ "./src/app/app-administration/sub-dialogs/content-import/content-import.component.ts"))];
                    case 1:
                        ContentImportComponent = (_a.sent()).ContentImportComponent;
                        return [2 /*return*/, ContentImportComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/content-import/content-import.component.scss":
/*!*********************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/content-import/content-import.component.scss ***!
  \*********************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".field-label {\n  font-size: 18px;\n  margin: 24px 0 0;\n}\n\n.mat-radio-group {\n  display: flex;\n  flex-direction: column;\n  margin: 8px 0;\n}\n\n.mat-radio-button {\n  margin: 5px;\n  font-size: 14px;\n}\n\n.hint {\n  font-size: 18px;\n  margin-top: 24px;\n  margin-bottom: 16px;\n}\n\n.progress-message {\n  font-size: 18px;\n}\n\n.evaluation__title {\n  font-size: 18px;\n  margin: 24px 0 0;\n  font-weight: bold;\n}\n\n.evaluation__content {\n  font-size: 14px;\n}\n\n.evaluation__content li {\n  padding: 2px 0;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvY29udGVudC1pbXBvcnQvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXGFwcC1hZG1pbmlzdHJhdGlvblxcc3ViLWRpYWxvZ3NcXGNvbnRlbnQtaW1wb3J0XFxjb250ZW50LWltcG9ydC5jb21wb25lbnQuc2NzcyIsInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvY29udGVudC1pbXBvcnQvY29udGVudC1pbXBvcnQuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxlQUFBO0VBQ0EsZ0JBQUE7QUNDRjs7QURFQTtFQUNFLGFBQUE7RUFDQSxzQkFBQTtFQUNBLGFBQUE7QUNDRjs7QURFQTtFQUNFLFdBQUE7RUFDQSxlQUFBO0FDQ0Y7O0FERUE7RUFDRSxlQUFBO0VBQ0EsZ0JBQUE7RUFDQSxtQkFBQTtBQ0NGOztBREVBO0VBQ0UsZUFBQTtBQ0NGOztBREdFO0VBQ0UsZUFBQTtFQUNBLGdCQUFBO0VBQ0EsaUJBQUE7QUNBSjs7QURHRTtFQUNFLGVBQUE7QUNESjs7QURHSTtFQUNFLGNBQUE7QUNETiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL3N1Yi1kaWFsb2dzL2NvbnRlbnQtaW1wb3J0L2NvbnRlbnQtaW1wb3J0LmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLmZpZWxkLWxhYmVsIHtcclxuICBmb250LXNpemU6IDE4cHg7XHJcbiAgbWFyZ2luOiAyNHB4IDAgMDtcclxufVxyXG5cclxuLm1hdC1yYWRpby1ncm91cCB7XHJcbiAgZGlzcGxheTogZmxleDtcclxuICBmbGV4LWRpcmVjdGlvbjogY29sdW1uO1xyXG4gIG1hcmdpbjogOHB4IDA7XHJcbn1cclxuXHJcbi5tYXQtcmFkaW8tYnV0dG9uIHtcclxuICBtYXJnaW46IDVweDtcclxuICBmb250LXNpemU6IDE0cHg7XHJcbn1cclxuXHJcbi5oaW50IHtcclxuICBmb250LXNpemU6IDE4cHg7XHJcbiAgbWFyZ2luLXRvcDogMjRweDtcclxuICBtYXJnaW4tYm90dG9tOiAxNnB4O1xyXG59XHJcblxyXG4ucHJvZ3Jlc3MtbWVzc2FnZSB7XHJcbiAgZm9udC1zaXplOiAxOHB4O1xyXG59XHJcblxyXG4uZXZhbHVhdGlvbiB7XHJcbiAgJl9fdGl0bGUge1xyXG4gICAgZm9udC1zaXplOiAxOHB4O1xyXG4gICAgbWFyZ2luOiAyNHB4IDAgMDtcclxuICAgIGZvbnQtd2VpZ2h0OiBib2xkO1xyXG4gIH1cclxuXHJcbiAgJl9fY29udGVudCB7XHJcbiAgICBmb250LXNpemU6IDE0cHg7XHJcblxyXG4gICAgbGkge1xyXG4gICAgICBwYWRkaW5nOiAycHggMDtcclxuICAgIH1cclxuICB9XHJcbn1cclxuIiwiLmZpZWxkLWxhYmVsIHtcbiAgZm9udC1zaXplOiAxOHB4O1xuICBtYXJnaW46IDI0cHggMCAwO1xufVxuXG4ubWF0LXJhZGlvLWdyb3VwIHtcbiAgZGlzcGxheTogZmxleDtcbiAgZmxleC1kaXJlY3Rpb246IGNvbHVtbjtcbiAgbWFyZ2luOiA4cHggMDtcbn1cblxuLm1hdC1yYWRpby1idXR0b24ge1xuICBtYXJnaW46IDVweDtcbiAgZm9udC1zaXplOiAxNHB4O1xufVxuXG4uaGludCB7XG4gIGZvbnQtc2l6ZTogMThweDtcbiAgbWFyZ2luLXRvcDogMjRweDtcbiAgbWFyZ2luLWJvdHRvbTogMTZweDtcbn1cblxuLnByb2dyZXNzLW1lc3NhZ2Uge1xuICBmb250LXNpemU6IDE4cHg7XG59XG5cbi5ldmFsdWF0aW9uX190aXRsZSB7XG4gIGZvbnQtc2l6ZTogMThweDtcbiAgbWFyZ2luOiAyNHB4IDAgMDtcbiAgZm9udC13ZWlnaHQ6IGJvbGQ7XG59XG4uZXZhbHVhdGlvbl9fY29udGVudCB7XG4gIGZvbnQtc2l6ZTogMTRweDtcbn1cbi5ldmFsdWF0aW9uX19jb250ZW50IGxpIHtcbiAgcGFkZGluZzogMnB4IDA7XG59Il19 */");

/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/content-import/content-import.component.ts":
/*!*******************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/content-import/content-import.component.ts ***!
  \*******************************************************************************************/
/*! exports provided: ContentImportComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ContentImportComponent", function() { return ContentImportComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _services_content_import_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../services/content-import.service */ "./src/app/app-administration/services/content-import.service.ts");
/* harmony import */ var _shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");






var ContentImportComponent = /** @class */ (function () {
    function ContentImportComponent(dialogRef, route, contentImportService) {
        this.dialogRef = dialogRef;
        this.route = route;
        this.contentImportService = contentImportService;
        this.hostClass = 'dialog-component';
        this.errors = {
            0: 'Unknown error occured.',
            1: 'Selected content-type does not exist.',
            2: 'Document is not a valid XML file.',
            3: 'Selected content-type does not match the content-type in the XML file.',
            4: 'The language is not supported.',
            5: 'The document does not specify all languages for all entities.',
            6: 'Language reference cannot be parsed, the language is not supported.',
            7: 'Language reference cannot be parsed, the read-write protection is not supported.',
            8: 'Value cannot be read, because of it has an invalid format.'
        };
        this.viewStates = {
            waiting: 0,
            default: 1,
            evaluated: 2,
            imported: 3,
        };
        this.viewStateSelected = this.viewStates.default;
        this.formValues = {
            defaultLanguage: sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_5__["keyLangPri"]),
            contentType: this.route.snapshot.paramMap.get('contentTypeStaticName'),
            file: null,
            resourcesReferences: 'Keep',
            clearEntities: 'None',
        };
    }
    ContentImportComponent.prototype.ngOnInit = function () {
    };
    ContentImportComponent.prototype.evaluateContent = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _this = this;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.viewStateSelected = this.viewStates.waiting;
                        return [4 /*yield*/, this.contentImportService.evaluateContent(this.formValues)];
                    case 1: return [2 /*return*/, (_a.sent()).subscribe(function (result) {
                            _this.evaluationResult = result;
                            _this.viewStateSelected = _this.viewStates.evaluated;
                        })];
                }
            });
        });
    };
    ContentImportComponent.prototype.importContent = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _this = this;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.viewStateSelected = this.viewStates.waiting;
                        return [4 /*yield*/, this.contentImportService.importContent(this.formValues)];
                    case 1: return [2 /*return*/, (_a.sent()).subscribe(function (result) {
                            _this.importResult = result;
                            _this.viewStateSelected = _this.viewStates.imported;
                        })];
                }
            });
        });
    };
    ContentImportComponent.prototype.back = function () {
        this.viewStateSelected = this.viewStates.default;
        this.evaluationResult = undefined;
    };
    ContentImportComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ContentImportComponent.prototype.fileChange = function (event) {
        this.formValues.file = event.target.files[0];
    };
    ContentImportComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_content_import_service__WEBPACK_IMPORTED_MODULE_4__["ContentImportService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ContentImportComponent.prototype, "hostClass", void 0);
    ContentImportComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-content-import',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./content-import.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/content-import/content-import.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./content-import.component.scss */ "./src/app/app-administration/sub-dialogs/content-import/content-import.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_content_import_service__WEBPACK_IMPORTED_MODULE_4__["ContentImportService"]])
    ], ContentImportComponent);
    return ContentImportComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type-dialog.config.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type-dialog.config.ts ***!
  \*****************************************************************************************************/
/*! exports provided: editContentTypeDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "editContentTypeDialog", function() { return editContentTypeDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var editContentTypeDialog = {
    name: 'EDIT_CONTENT_TYPE_DIALOG',
    initContext: false,
    panelSize: 'small',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var EditContentTypeComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./edit-content-type.component */ "./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.ts"))];
                    case 1:
                        EditContentTypeComponent = (_a.sent()).EditContentTypeComponent;
                        return [2 /*return*/, EditContentTypeComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.scss":
/*!***************************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.scss ***!
  \***************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".edit-input {\n  padding-bottom: 8px;\n}\n\n.mat-hint {\n  font-size: 12px;\n}\n\n.mat-accordion {\n  padding-bottom: 8px;\n  display: block;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvZWRpdC1jb250ZW50LXR5cGUvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXGFwcC1hZG1pbmlzdHJhdGlvblxcc3ViLWRpYWxvZ3NcXGVkaXQtY29udGVudC10eXBlXFxlZGl0LWNvbnRlbnQtdHlwZS5jb21wb25lbnQuc2NzcyIsInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvZWRpdC1jb250ZW50LXR5cGUvZWRpdC1jb250ZW50LXR5cGUuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxtQkFBQTtBQ0NGOztBREVBO0VBQ0UsZUFBQTtBQ0NGOztBREVBO0VBQ0UsbUJBQUE7RUFDQSxjQUFBO0FDQ0YiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9zdWItZGlhbG9ncy9lZGl0LWNvbnRlbnQtdHlwZS9lZGl0LWNvbnRlbnQtdHlwZS5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5lZGl0LWlucHV0IHtcclxuICBwYWRkaW5nLWJvdHRvbTogOHB4O1xyXG59XHJcblxyXG4ubWF0LWhpbnQge1xyXG4gIGZvbnQtc2l6ZTogMTJweDtcclxufVxyXG5cclxuLm1hdC1hY2NvcmRpb24ge1xyXG4gIHBhZGRpbmctYm90dG9tOiA4cHg7XHJcbiAgZGlzcGxheTogYmxvY2s7XHJcbn1cclxuIiwiLmVkaXQtaW5wdXQge1xuICBwYWRkaW5nLWJvdHRvbTogOHB4O1xufVxuXG4ubWF0LWhpbnQge1xuICBmb250LXNpemU6IDEycHg7XG59XG5cbi5tYXQtYWNjb3JkaW9uIHtcbiAgcGFkZGluZy1ib3R0b206IDhweDtcbiAgZGlzcGxheTogYmxvY2s7XG59Il19 */");

/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.ts":
/*!*************************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.ts ***!
  \*************************************************************************************************/
/*! exports provided: EditContentTypeComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EditContentTypeComponent", function() { return EditContentTypeComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _models_content_type_model__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../models/content-type.model */ "./src/app/app-administration/models/content-type.model.ts");
/* harmony import */ var _services_content_types_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _constants_content_type_patterns__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../../constants/content-type.patterns */ "./src/app/app-administration/constants/content-type.patterns.ts");









var EditContentTypeComponent = /** @class */ (function () {
    function EditContentTypeComponent(dialogRef, route, contentTypesService, snackBar) {
        this.dialogRef = dialogRef;
        this.route = route;
        this.contentTypesService = contentTypesService;
        this.snackBar = snackBar;
        this.hostClass = 'dialog-component';
        this.lockScope = true;
        this.contentTypeNamePattern = _constants_content_type_patterns__WEBPACK_IMPORTED_MODULE_8__["contentTypeNamePattern"];
        this.contentTypeNameError = _constants_content_type_patterns__WEBPACK_IMPORTED_MODULE_8__["contentTypeNameError"];
        // Workaround for angular component issue #13870
        this.disableAnimation = true;
        this.scope = this.route.snapshot.paramMap.get('scope');
        this.id = parseInt(this.route.snapshot.paramMap.get('id'), 10);
    }
    EditContentTypeComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        // timeout required to avoid the dreaded 'ExpressionChangedAfterItHasBeenCheckedError'
        setTimeout(function () { return _this.disableAnimation = false; });
    };
    EditContentTypeComponent.prototype.ngOnInit = function () {
        this.fetchScopes();
        if (!this.id) {
            this.contentType = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, (new _models_content_type_model__WEBPACK_IMPORTED_MODULE_5__["ContentTypeEdit"]())), { StaticName: '', Name: '', Description: '', Scope: this.scope, ChangeStaticName: false, NewStaticName: '' });
        }
        else {
            this.fetchContentType();
        }
    };
    EditContentTypeComponent.prototype.changeScope = function (event) {
        var newScope = event.value;
        if (newScope === 'Other') {
            newScope = prompt('This is an advanced feature to show content-types of another scope. Don\'t use this if you don\'t know what you\'re doing, as content-types of other scopes are usually hidden for a good reason.');
            if (!newScope) {
                newScope = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_7__["eavConstants"].scopes.default.value;
            }
            else if (!this.scopeOptions.find(function (option) { return option.value === newScope; })) {
                var newScopeOption = {
                    name: newScope,
                    value: newScope,
                };
                this.scopeOptions.push(newScopeOption);
            }
        }
        this.contentType.Scope = newScope;
    };
    EditContentTypeComponent.prototype.unlockScope = function (event) {
        event.stopPropagation();
        this.lockScope = !this.lockScope;
        if (this.lockScope) {
            this.contentType.Scope = this.scope;
        }
    };
    EditContentTypeComponent.prototype.save = function () {
        var _this = this;
        this.snackBar.open('Saving...');
        this.contentTypesService.save(this.contentType).subscribe(function (result) {
            _this.snackBar.open('Saved', null, { duration: 2000 });
            _this.closeDialog();
        });
    };
    EditContentTypeComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    EditContentTypeComponent.prototype.fetchScopes = function () {
        var _this = this;
        this.contentTypesService.getScopes().subscribe(function (scopes) {
            _this.scopeOptions = scopes;
        });
    };
    EditContentTypeComponent.prototype.fetchContentType = function () {
        var _this = this;
        this.contentTypesService.retrieveContentTypes(this.scope).subscribe(function (contentTypes) {
            var contentType = contentTypes.find(function (ct) { return ct.Id === _this.id; });
            _this.contentType = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, contentType), { ChangeStaticName: false, NewStaticName: contentType.StaticName });
        });
    };
    EditContentTypeComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_content_types_service__WEBPACK_IMPORTED_MODULE_6__["ContentTypesService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], EditContentTypeComponent.prototype, "hostClass", void 0);
    EditContentTypeComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-edit-content-type',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./edit-content-type.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./edit-content-type.component.scss */ "./src/app/app-administration/sub-dialogs/edit-content-type/edit-content-type.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_content_types_service__WEBPACK_IMPORTED_MODULE_6__["ContentTypesService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"]])
    ], EditContentTypeComponent);
    return EditContentTypeComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts-dialog.config.ts":
/*!***************************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts-dialog.config.ts ***!
  \***************************************************************************************************/
/*! exports provided: exportAppPartsDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "exportAppPartsDialog", function() { return exportAppPartsDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var exportAppPartsDialog = {
    name: 'EXPORT_APP_PARTS',
    initContext: false,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ExportAppPartsComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./export-app-parts.component */ "./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.ts"))];
                    case 1:
                        ExportAppPartsComponent = (_a.sent()).ExportAppPartsComponent;
                        return [2 /*return*/, ExportAppPartsComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.scss":
/*!*************************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.scss ***!
  \*************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".edit-input {\n  padding-bottom: 8px;\n}\n\n.mat-hint {\n  font-size: 12px;\n}\n\n.content-info__title {\n  font-size: 18px;\n  font-weight: bold;\n}\n\n.content-info__subtitle {\n  font-size: 14px;\n  font-weight: bold;\n}\n\n.content-info__list {\n  font-size: 14px;\n  list-style-type: none;\n}\n\n.content-info__base {\n  padding: 0;\n}\n\n.content-info__item {\n  border-top: 1px solid #DDD;\n  padding: 2px;\n}\n\n.option-box {\n  margin: 8px 0;\n}\n\n.option-box__text {\n  white-space: normal;\n  font-size: 14px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvZXhwb3J0LWFwcC1wYXJ0cy9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcYXBwLWFkbWluaXN0cmF0aW9uXFxzdWItZGlhbG9nc1xcZXhwb3J0LWFwcC1wYXJ0c1xcZXhwb3J0LWFwcC1wYXJ0cy5jb21wb25lbnQuc2NzcyIsInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvZXhwb3J0LWFwcC1wYXJ0cy9leHBvcnQtYXBwLXBhcnRzLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsbUJBQUE7QUNDRjs7QURFQTtFQUNFLGVBQUE7QUNDRjs7QURHRTtFQUNFLGVBQUE7RUFDQSxpQkFBQTtBQ0FKOztBREdFO0VBQ0UsZUFBQTtFQUNBLGlCQUFBO0FDREo7O0FESUU7RUFDRSxlQUFBO0VBQ0EscUJBQUE7QUNGSjs7QURLRTtFQUNFLFVBQUE7QUNISjs7QURNRTtFQUNFLDBCQUFBO0VBQ0EsWUFBQTtBQ0pKOztBRFFBO0VBQ0UsYUFBQTtBQ0xGOztBRE9FO0VBQ0UsbUJBQUE7RUFDQSxlQUFBO0FDTEoiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi9zdWItZGlhbG9ncy9leHBvcnQtYXBwLXBhcnRzL2V4cG9ydC1hcHAtcGFydHMuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuZWRpdC1pbnB1dCB7XHJcbiAgcGFkZGluZy1ib3R0b206IDhweDtcclxufVxyXG5cclxuLm1hdC1oaW50IHtcclxuICBmb250LXNpemU6IDEycHg7XHJcbn1cclxuXHJcbi5jb250ZW50LWluZm8ge1xyXG4gICZfX3RpdGxlIHtcclxuICAgIGZvbnQtc2l6ZTogMThweDtcclxuICAgIGZvbnQtd2VpZ2h0OiBib2xkO1xyXG4gIH1cclxuXHJcbiAgJl9fc3VidGl0bGUge1xyXG4gICAgZm9udC1zaXplOiAxNHB4O1xyXG4gICAgZm9udC13ZWlnaHQ6IGJvbGQ7XHJcbiAgfVxyXG5cclxuICAmX19saXN0IHtcclxuICAgIGZvbnQtc2l6ZTogMTRweDtcclxuICAgIGxpc3Qtc3R5bGUtdHlwZTogbm9uZTtcclxuICB9XHJcblxyXG4gICZfX2Jhc2Uge1xyXG4gICAgcGFkZGluZzogMDtcclxuICB9XHJcblxyXG4gICZfX2l0ZW0ge1xyXG4gICAgYm9yZGVyLXRvcDogMXB4IHNvbGlkICNEREQ7XHJcbiAgICBwYWRkaW5nOiAycHg7XHJcbiAgfVxyXG59XHJcblxyXG4ub3B0aW9uLWJveCB7XHJcbiAgbWFyZ2luOiA4cHggMDtcclxuXHJcbiAgJl9fdGV4dCB7XHJcbiAgICB3aGl0ZS1zcGFjZTogbm9ybWFsO1xyXG4gICAgZm9udC1zaXplOiAxNHB4O1xyXG4gIH1cclxufVxyXG4iLCIuZWRpdC1pbnB1dCB7XG4gIHBhZGRpbmctYm90dG9tOiA4cHg7XG59XG5cbi5tYXQtaGludCB7XG4gIGZvbnQtc2l6ZTogMTJweDtcbn1cblxuLmNvbnRlbnQtaW5mb19fdGl0bGUge1xuICBmb250LXNpemU6IDE4cHg7XG4gIGZvbnQtd2VpZ2h0OiBib2xkO1xufVxuLmNvbnRlbnQtaW5mb19fc3VidGl0bGUge1xuICBmb250LXNpemU6IDE0cHg7XG4gIGZvbnQtd2VpZ2h0OiBib2xkO1xufVxuLmNvbnRlbnQtaW5mb19fbGlzdCB7XG4gIGZvbnQtc2l6ZTogMTRweDtcbiAgbGlzdC1zdHlsZS10eXBlOiBub25lO1xufVxuLmNvbnRlbnQtaW5mb19fYmFzZSB7XG4gIHBhZGRpbmc6IDA7XG59XG4uY29udGVudC1pbmZvX19pdGVtIHtcbiAgYm9yZGVyLXRvcDogMXB4IHNvbGlkICNEREQ7XG4gIHBhZGRpbmc6IDJweDtcbn1cblxuLm9wdGlvbi1ib3gge1xuICBtYXJnaW46IDhweCAwO1xufVxuLm9wdGlvbi1ib3hfX3RleHQge1xuICB3aGl0ZS1zcGFjZTogbm9ybWFsO1xuICBmb250LXNpemU6IDE0cHg7XG59Il19 */");

/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.ts":
/*!***********************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.ts ***!
  \***********************************************************************************************/
/*! exports provided: ExportAppPartsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExportAppPartsComponent", function() { return ExportAppPartsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _services_export_app_parts_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../services/export-app-parts.service */ "./src/app/app-administration/services/export-app-parts.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _services_content_types_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");






var ExportAppPartsComponent = /** @class */ (function () {
    function ExportAppPartsComponent(dialogRef, exportAppPartsService, contentTypesService) {
        this.dialogRef = dialogRef;
        this.exportAppPartsService = exportAppPartsService;
        this.contentTypesService = contentTypesService;
        this.hostClass = 'dialog-component';
        this.exportScope = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_4__["eavConstants"].scopes.default.value;
        this.lockScope = true;
        this.isExporting = false;
    }
    ExportAppPartsComponent.prototype.ngOnInit = function () {
        this.fetchScopes();
        this.fetchContentInfo();
    };
    ExportAppPartsComponent.prototype.exportAppParts = function () {
        this.isExporting = true;
        // spm TODO: maybe optimize these functions to not loop content types and entities multiple times for no reason
        // spm TODO: figure out how to capture window loading to disable export button
        var contentTypeIds = this.selectedContentTypes().map(function (contentType) { return contentType.Id; });
        var templateIds = this.selectedTemplates().map(function (template) { return template.Id; });
        var entityIds = this.selectedEntities().map(function (entity) { return entity.Id; });
        entityIds = entityIds.concat(templateIds);
        this.exportAppPartsService.exportParts(contentTypeIds, entityIds, templateIds);
        this.isExporting = false;
    };
    ExportAppPartsComponent.prototype.changeScope = function (event) {
        var newScope = event.value;
        if (newScope === 'Other') {
            newScope = prompt('This is an advanced feature to show content-types of another scope. Don\'t use this if you don\'t know what you\'re doing, as content-types of other scopes are usually hidden for a good reason.');
            if (!newScope) {
                newScope = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_4__["eavConstants"].scopes.default.value;
            }
            else if (!this.scopeOptions.find(function (option) { return option.value === newScope; })) {
                var newScopeOption = {
                    name: newScope,
                    value: newScope,
                };
                this.scopeOptions.push(newScopeOption);
            }
        }
        this.exportScope = newScope;
        this.fetchContentInfo();
    };
    ExportAppPartsComponent.prototype.unlockScope = function (event) {
        event.stopPropagation();
        this.lockScope = !this.lockScope;
        if (this.lockScope) {
            this.exportScope = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_4__["eavConstants"].scopes.default.value;
            this.fetchContentInfo();
        }
    };
    ExportAppPartsComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ExportAppPartsComponent.prototype.fetchScopes = function () {
        var _this = this;
        this.contentTypesService.getScopes().subscribe(function (scopes) {
            _this.scopeOptions = scopes;
        });
    };
    ExportAppPartsComponent.prototype.fetchContentInfo = function () {
        var _this = this;
        this.exportAppPartsService.getContentInfo(this.exportScope).subscribe(function (contentInfo) {
            _this.contentInfo = contentInfo;
        });
    };
    ExportAppPartsComponent.prototype.selectedContentTypes = function () {
        return this.contentInfo.ContentTypes.filter(function (contentType) { return contentType._export; });
    };
    ExportAppPartsComponent.prototype.selectedEntities = function () {
        var e_1, _a;
        var entities = [];
        try {
            for (var _b = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(this.contentInfo.ContentTypes), _c = _b.next(); !_c.done; _c = _b.next()) {
                var contentType = _c.value;
                entities = entities.concat(contentType.Entities.filter(function (entity) { return entity._export; }));
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (_c && !_c.done && (_a = _b.return)) _a.call(_b);
            }
            finally { if (e_1) throw e_1.error; }
        }
        return entities;
    };
    ExportAppPartsComponent.prototype.selectedTemplates = function () {
        var e_2, _a;
        var templates = [];
        try {
            // The ones with...
            for (var _b = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(this.contentInfo.ContentTypes), _c = _b.next(); !_c.done; _c = _b.next()) {
                var contentType = _c.value;
                templates = templates.concat(contentType.Templates.filter(function (template) { return template._export; }));
            }
        }
        catch (e_2_1) { e_2 = { error: e_2_1 }; }
        finally {
            try {
                if (_c && !_c.done && (_a = _b.return)) _a.call(_b);
            }
            finally { if (e_2) throw e_2.error; }
        }
        // ...and without content types
        templates = templates.concat(this.contentInfo.TemplatesWithoutContentTypes.filter(function (template) { return template._export; }));
        return templates;
    };
    ExportAppPartsComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _services_export_app_parts_service__WEBPACK_IMPORTED_MODULE_3__["ExportAppPartsService"] },
        { type: _services_content_types_service__WEBPACK_IMPORTED_MODULE_5__["ContentTypesService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ExportAppPartsComponent.prototype, "hostClass", void 0);
    ExportAppPartsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-export-app-parts',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./export-app-parts.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./export-app-parts.component.scss */ "./src/app/app-administration/sub-dialogs/export-app-parts/export-app-parts.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"],
            _services_export_app_parts_service__WEBPACK_IMPORTED_MODULE_3__["ExportAppPartsService"],
            _services_content_types_service__WEBPACK_IMPORTED_MODULE_5__["ContentTypesService"]])
    ], ExportAppPartsComponent);
    return ExportAppPartsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/export-app/export-app-dialog.config.ts":
/*!***************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/export-app/export-app-dialog.config.ts ***!
  \***************************************************************************************/
/*! exports provided: exportAppDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "exportAppDialog", function() { return exportAppDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var exportAppDialog = {
    name: 'EXPORT_APP',
    initContext: false,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ExportAppComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./export-app.component */ "./src/app/app-administration/sub-dialogs/export-app/export-app.component.ts"))];
                    case 1:
                        ExportAppComponent = (_a.sent()).ExportAppComponent;
                        return [2 /*return*/, ExportAppComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/export-app/export-app.component.scss":
/*!*************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/export-app/export-app.component.scss ***!
  \*************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".app-info__title {\n  font-size: 18px;\n  margin: 24px 0 0;\n  font-weight: bold;\n}\n.app-info__content {\n  font-size: 14px;\n}\n.app-info__content li {\n  padding: 2px 0;\n}\n.options-wrapper {\n  margin: 24px 0;\n}\n.option-box {\n  margin: 2px 0;\n}\n.option-box__text {\n  white-space: normal;\n  font-size: 14px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvZXhwb3J0LWFwcC9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcYXBwLWFkbWluaXN0cmF0aW9uXFxzdWItZGlhbG9nc1xcZXhwb3J0LWFwcFxcZXhwb3J0LWFwcC5jb21wb25lbnQuc2NzcyIsInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vc3ViLWRpYWxvZ3MvZXhwb3J0LWFwcC9leHBvcnQtYXBwLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUNFO0VBQ0UsZUFBQTtFQUNBLGdCQUFBO0VBQ0EsaUJBQUE7QUNBSjtBREdFO0VBQ0UsZUFBQTtBQ0RKO0FER0k7RUFDRSxjQUFBO0FDRE47QURNQTtFQUNFLGNBQUE7QUNIRjtBRE1BO0VBQ0UsYUFBQTtBQ0hGO0FES0U7RUFDRSxtQkFBQTtFQUNBLGVBQUE7QUNISiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL3N1Yi1kaWFsb2dzL2V4cG9ydC1hcHAvZXhwb3J0LWFwcC5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5hcHAtaW5mbyB7XHJcbiAgJl9fdGl0bGUge1xyXG4gICAgZm9udC1zaXplOiAxOHB4O1xyXG4gICAgbWFyZ2luOiAyNHB4IDAgMDtcclxuICAgIGZvbnQtd2VpZ2h0OiBib2xkO1xyXG4gIH1cclxuXHJcbiAgJl9fY29udGVudCB7XHJcbiAgICBmb250LXNpemU6IDE0cHg7XHJcblxyXG4gICAgbGkge1xyXG4gICAgICBwYWRkaW5nOiAycHggMDtcclxuICAgIH1cclxuICB9XHJcbn1cclxuXHJcbi5vcHRpb25zLXdyYXBwZXIge1xyXG4gIG1hcmdpbjogMjRweCAwO1xyXG59XHJcblxyXG4ub3B0aW9uLWJveCB7XHJcbiAgbWFyZ2luOiAycHggMDtcclxuXHJcbiAgJl9fdGV4dCB7XHJcbiAgICB3aGl0ZS1zcGFjZTogbm9ybWFsO1xyXG4gICAgZm9udC1zaXplOiAxNHB4O1xyXG4gIH1cclxufVxyXG4iLCIuYXBwLWluZm9fX3RpdGxlIHtcbiAgZm9udC1zaXplOiAxOHB4O1xuICBtYXJnaW46IDI0cHggMCAwO1xuICBmb250LXdlaWdodDogYm9sZDtcbn1cbi5hcHAtaW5mb19fY29udGVudCB7XG4gIGZvbnQtc2l6ZTogMTRweDtcbn1cbi5hcHAtaW5mb19fY29udGVudCBsaSB7XG4gIHBhZGRpbmc6IDJweCAwO1xufVxuXG4ub3B0aW9ucy13cmFwcGVyIHtcbiAgbWFyZ2luOiAyNHB4IDA7XG59XG5cbi5vcHRpb24tYm94IHtcbiAgbWFyZ2luOiAycHggMDtcbn1cbi5vcHRpb24tYm94X190ZXh0IHtcbiAgd2hpdGUtc3BhY2U6IG5vcm1hbDtcbiAgZm9udC1zaXplOiAxNHB4O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/export-app/export-app.component.ts":
/*!***********************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/export-app/export-app.component.ts ***!
  \***********************************************************************************/
/*! exports provided: ExportAppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExportAppComponent", function() { return ExportAppComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _services_export_app_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../services/export-app.service */ "./src/app/app-administration/services/export-app.service.ts");




var ExportAppComponent = /** @class */ (function () {
    function ExportAppComponent(dialogRef, exportAppService) {
        this.dialogRef = dialogRef;
        this.exportAppService = exportAppService;
        this.hostClass = 'dialog-component';
        this.includeContentGroups = false;
        this.resetAppGuid = false;
        this.isExporting = false;
    }
    ExportAppComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.exportAppService.getAppInfo().subscribe(function (appInfo) {
            _this.appInfo = appInfo;
        });
    };
    ExportAppComponent.prototype.exportApp = function () {
        // spm TODO: figure out how to capture window loading to disable export button
        this.isExporting = true;
        this.exportAppService.exportApp(this.includeContentGroups, this.resetAppGuid);
        this.isExporting = false;
    };
    ExportAppComponent.prototype.exportGit = function () {
        var _this = this;
        this.isExporting = true;
        this.exportAppService.exportForVersionControl(this.includeContentGroups, this.resetAppGuid).subscribe({
            next: function (res) {
                _this.isExporting = false;
                alert('Done - please check your \'.data\' folder');
            },
            error: function (error) {
                _this.isExporting = false;
            },
        });
    };
    ExportAppComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ExportAppComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _services_export_app_service__WEBPACK_IMPORTED_MODULE_3__["ExportAppService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ExportAppComponent.prototype, "hostClass", void 0);
    ExportAppComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-export-app',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./export-app.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/export-app/export-app.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./export-app.component.scss */ "./src/app/app-administration/sub-dialogs/export-app/export-app.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"], _services_export_app_service__WEBPACK_IMPORTED_MODULE_3__["ExportAppService"]])
    ], ExportAppComponent);
    return ExportAppComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts-dialog.config.ts":
/*!***************************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts-dialog.config.ts ***!
  \***************************************************************************************************/
/*! exports provided: importAppPartsDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "importAppPartsDialog", function() { return importAppPartsDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var importAppPartsDialog = {
    name: 'IMPORT_APP_PARTS',
    initContext: false,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ImportAppPartsComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./import-app-parts.component */ "./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.ts"))];
                    case 1:
                        ImportAppPartsComponent = (_a.sent()).ImportAppPartsComponent;
                        return [2 /*return*/, ImportAppPartsComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.scss":
/*!*************************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.scss ***!
  \*************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL3N1Yi1kaWFsb2dzL2ltcG9ydC1hcHAtcGFydHMvaW1wb3J0LWFwcC1wYXJ0cy5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.ts":
/*!***********************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.ts ***!
  \***********************************************************************************************/
/*! exports provided: ImportAppPartsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ImportAppPartsComponent", function() { return ImportAppPartsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _services_import_app_parts_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../services/import-app-parts.service */ "./src/app/app-administration/services/import-app-parts.service.ts");




var ImportAppPartsComponent = /** @class */ (function () {
    function ImportAppPartsComponent(dialogRef, importAppPartsService) {
        this.dialogRef = dialogRef;
        this.importAppPartsService = importAppPartsService;
        this.hostClass = 'dialog-component';
        this.isImporting = false;
    }
    ImportAppPartsComponent.prototype.ngOnInit = function () {
    };
    ImportAppPartsComponent.prototype.fileChange = function (event) {
        this.importFile = event.target.files[0];
    };
    ImportAppPartsComponent.prototype.importAppParts = function () {
        var _this = this;
        this.isImporting = true;
        this.importAppPartsService.importAppParts(this.importFile).subscribe({
            next: function (result) {
                _this.importResult = result;
                _this.isImporting = false;
            },
            error: function (error) {
                _this.isImporting = false;
            },
        });
    };
    ImportAppPartsComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ImportAppPartsComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _services_import_app_parts_service__WEBPACK_IMPORTED_MODULE_3__["ImportAppPartsService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ImportAppPartsComponent.prototype, "hostClass", void 0);
    ImportAppPartsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-import-app-parts',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./import-app-parts.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./import-app-parts.component.scss */ "./src/app/app-administration/sub-dialogs/import-app-parts/import-app-parts.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"], _services_import_app_parts_service__WEBPACK_IMPORTED_MODULE_3__["ImportAppPartsService"]])
    ], ImportAppPartsComponent);
    return ImportAppPartsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/import-query/import-query-dialog.config.ts":
/*!*******************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/import-query/import-query-dialog.config.ts ***!
  \*******************************************************************************************/
/*! exports provided: importQueryDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "importQueryDialog", function() { return importQueryDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var importQueryDialog = {
    name: 'IMPORT_QUERY_DIALOG',
    initContext: false,
    panelSize: 'medium',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ImportQueryComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./import-query.component */ "./src/app/app-administration/sub-dialogs/import-query/import-query.component.ts"))];
                    case 1:
                        ImportQueryComponent = (_a.sent()).ImportQueryComponent;
                        return [2 /*return*/, ImportQueryComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/import-query/import-query.component.scss":
/*!*****************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/import-query/import-query.component.scss ***!
  \*****************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL3N1Yi1kaWFsb2dzL2ltcG9ydC1xdWVyeS9pbXBvcnQtcXVlcnkuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/import-query/import-query.component.ts":
/*!***************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/import-query/import-query.component.ts ***!
  \***************************************************************************************/
/*! exports provided: ImportQueryComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ImportQueryComponent", function() { return ImportQueryComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _services_pipelines_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../services/pipelines.service */ "./src/app/app-administration/services/pipelines.service.ts");




var ImportQueryComponent = /** @class */ (function () {
    function ImportQueryComponent(dialogRef, pipelinesService) {
        this.dialogRef = dialogRef;
        this.pipelinesService = pipelinesService;
        this.hostClass = 'dialog-component';
        this.viewStates = {
            Default: 1,
            Waiting: 2,
            Imported: 3
        };
        this.viewState = this.viewStates.Default;
    }
    ImportQueryComponent.prototype.ngOnInit = function () {
    };
    ImportQueryComponent.prototype.importQuery = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _this = this;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.viewState = this.viewStates.Waiting;
                        return [4 /*yield*/, this.pipelinesService.importQuery(this.importFile)];
                    case 1:
                        (_a.sent()).subscribe({
                            next: function (res) {
                                _this.viewState = _this.viewStates.Imported;
                            },
                            error: function (error) {
                                _this.viewState = _this.viewStates.Default;
                            },
                        });
                        return [2 /*return*/];
                }
            });
        });
    };
    ImportQueryComponent.prototype.fileChange = function (event) {
        this.importFile = event.target.files[0];
    };
    ImportQueryComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ImportQueryComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] },
        { type: _services_pipelines_service__WEBPACK_IMPORTED_MODULE_3__["PipelinesService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["HostBinding"])('className'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], ImportQueryComponent.prototype, "hostClass", void 0);
    ImportQueryComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-import-query',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./import-query.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/import-query/import-query.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./import-query.component.scss */ "./src/app/app-administration/sub-dialogs/import-query/import-query.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"], _services_pipelines_service__WEBPACK_IMPORTED_MODULE_3__["PipelinesService"]])
    ], ImportQueryComponent);
    return ImportQueryComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/views-usage/views-usage-dialog.config.ts":
/*!*****************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/views-usage/views-usage-dialog.config.ts ***!
  \*****************************************************************************************/
/*! exports provided: viewsUsageDialog */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "viewsUsageDialog", function() { return viewsUsageDialog; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var viewsUsageDialog = {
    name: 'VIEWS_USAGE_DIALOG',
    initContext: false,
    panelSize: 'large',
    panelClass: null,
    getComponent: function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var ViewsUsageComponent;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, Promise.resolve(/*! import() */).then(__webpack_require__.bind(null, /*! ./views-usage.component */ "./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.ts"))];
                    case 1:
                        ViewsUsageComponent = (_a.sent()).ViewsUsageComponent;
                        return [2 /*return*/, ViewsUsageComponent];
                }
            });
        });
    }
};


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/views-usage/views-usage-grid.helpers.ts":
/*!****************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/views-usage/views-usage-grid.helpers.ts ***!
  \****************************************************************************************/
/*! exports provided: blockIdValueGetter, moduleIdValueGetter, moduleIdClassGetter, pageIdValueGetter, pageIdClassGetter, nameClassGetter, onNameClicked, statusCellRenderer */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "blockIdValueGetter", function() { return blockIdValueGetter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "moduleIdValueGetter", function() { return moduleIdValueGetter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "moduleIdClassGetter", function() { return moduleIdClassGetter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "pageIdValueGetter", function() { return pageIdValueGetter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "pageIdClassGetter", function() { return pageIdClassGetter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "nameClassGetter", function() { return nameClassGetter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "onNameClicked", function() { return onNameClicked; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "statusCellRenderer", function() { return statusCellRenderer; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function blockIdValueGetter(params) {
    var data = params.data;
    return "ID: " + data.Block.Id + "\nGUID: " + data.Block.Guid;
}
function moduleIdValueGetter(params) {
    var data = params.data;
    if (data.Module == null) {
        return;
    }
    return "ID: " + data.Module.Id + "\nGUID: " + data.Module.ModuleId + "\nTitle: " + data.Module.Title;
}
function moduleIdClassGetter(params) {
    if (params.value == null) {
        return 'no-outline';
    }
    return 'id-action no-padding no-outline';
}
function pageIdValueGetter(params) {
    var data = params.data;
    if (data.PageId == null) {
        return;
    }
    return "ID: " + data.PageId;
}
function pageIdClassGetter(params) {
    if (params.value == null) {
        return 'no-outline';
    }
    return 'id-action no-padding no-outline';
}
function nameClassGetter(params) {
    if (params.value == null) {
        return 'no-outline';
    }
    return 'primary-action highlight';
}
function onNameClicked(params) {
    if (params.value == null) {
        return;
    }
    var data = params.data;
    window.open(data.Url, '_blank');
}
function statusCellRenderer(params) {
    var status = params.value;
    if (status == null) {
        return;
    }
    return "\n    <div style=\"height: 100%;display: flex;align-items: center;\">\n      " + (status.IsVisible ? '<span class="material-icons-outlined">visibility</span>' : '<span class="material-icons-outlined">visibility_off</span>') + "\n      " + (status.IsDeleted ? '<span style="margin-left: 8px;" class="material-icons-outlined">delete</span>' : '') + "\n    </div>\n  ";
}


/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.scss":
/*!***************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.scss ***!
  \***************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL3N1Yi1kaWFsb2dzL3ZpZXdzLXVzYWdlL3ZpZXdzLXVzYWdlLmNvbXBvbmVudC5zY3NzIn0= */");

/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.ts":
/*!*************************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.ts ***!
  \*************************************************************************************/
/*! exports provided: ViewsUsageComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsUsageComponent", function() { return ViewsUsageComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _services_views_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../services/views.service */ "./src/app/app-administration/services/views.service.ts");
/* harmony import */ var _ag_grid_components_views_usage_id_views_usage_id_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../ag-grid-components/views-usage-id/views-usage-id.component */ "./src/app/app-administration/ag-grid-components/views-usage-id/views-usage-id.component.ts");
/* harmony import */ var _views_usage_helpers__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./views-usage.helpers */ "./src/app/app-administration/sub-dialogs/views-usage/views-usage.helpers.ts");
/* harmony import */ var _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./views-usage-grid.helpers */ "./src/app/app-administration/sub-dialogs/views-usage/views-usage-grid.helpers.ts");
/* harmony import */ var _ag_grid_components_views_usage_status_filter_views_usage_status_filter_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../../ag-grid-components/views-usage-status-filter/views-usage-status-filter.component */ "./src/app/app-administration/ag-grid-components/views-usage-status-filter/views-usage-status-filter.component.ts");









// tslint:disable-next-line:max-line-length


var ViewsUsageComponent = /** @class */ (function () {
    function ViewsUsageComponent(dialogRef, route, viewsService) {
        this.dialogRef = dialogRef;
        this.route = route;
        this.viewsService = viewsService;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_4__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_5__["defaultGridOptions"]), { frameworkComponents: {
                viewsUsageIdComponent: _ag_grid_components_views_usage_id_views_usage_id_component__WEBPACK_IMPORTED_MODULE_7__["ViewsUsageIdComponent"],
                viewsUsageStatusFilterComponent: _ag_grid_components_views_usage_status_filter_views_usage_status_filter_component__WEBPACK_IMPORTED_MODULE_10__["ViewsUsageStatusFilterComponent"],
            }, columnDefs: [
                {
                    headerName: 'Block', field: 'Block', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'viewsUsageIdComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["blockIdValueGetter"],
                },
                {
                    headerName: 'Module', field: 'Module', width: 76, headerClass: 'dense', cellRenderer: 'viewsUsageIdComponent',
                    sortable: true, filter: 'agTextColumnFilter', valueGetter: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["moduleIdValueGetter"], cellClass: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["moduleIdClassGetter"],
                },
                {
                    headerName: 'Page', field: 'PageId', width: 70, headerClass: 'dense', cellRenderer: 'viewsUsageIdComponent',
                    sortable: true, filter: 'agTextColumnFilter', valueGetter: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["pageIdValueGetter"], cellClass: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["pageIdClassGetter"],
                },
                {
                    headerName: 'Name', field: 'Name', flex: 2, minWidth: 250, sortable: true, filter: 'agTextColumnFilter',
                    cellClass: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["nameClassGetter"], onCellClicked: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["onNameClicked"],
                },
                { headerName: 'Language', field: 'Language', width: 90, cellClass: 'no-outline', sortable: true, filter: 'agTextColumnFilter' },
                {
                    headerName: 'Status', field: 'Status', width: 80, cellClass: 'icon no-outline', filter: 'viewsUsageStatusFilterComponent',
                    cellRenderer: _views_usage_grid_helpers__WEBPACK_IMPORTED_MODULE_9__["statusCellRenderer"],
                },
            ] });
        this.viewGuid = this.route.snapshot.paramMap.get('guid');
    }
    ViewsUsageComponent.prototype.ngOnInit = function () {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var _this = this;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                this.viewsService.getUsage(this.viewGuid).subscribe(function (viewUsages) {
                    _this.viewUsage = viewUsages[0];
                    _this.viewTooltip = "ID: " + _this.viewUsage.Id + "\nGUID: " + _this.viewUsage.Guid;
                    _this.data = Object(_views_usage_helpers__WEBPACK_IMPORTED_MODULE_8__["buildData"])(_this.viewUsage);
                });
                return [2 /*return*/];
            });
        });
    };
    ViewsUsageComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    ViewsUsageComponent.ctorParameters = function () { return [
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_views_service__WEBPACK_IMPORTED_MODULE_6__["ViewsService"] }
    ]; };
    ViewsUsageComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-views-usage',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./views-usage.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./views-usage.component.scss */ "./src/app/app-administration/sub-dialogs/views-usage/views-usage.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_3__["MatDialogRef"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_views_service__WEBPACK_IMPORTED_MODULE_6__["ViewsService"]])
    ], ViewsUsageComponent);
    return ViewsUsageComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/sub-dialogs/views-usage/views-usage.helpers.ts":
/*!***********************************************************************************!*\
  !*** ./src/app/app-administration/sub-dialogs/views-usage/views-usage.helpers.ts ***!
  \***********************************************************************************/
/*! exports provided: buildData */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "buildData", function() { return buildData; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function buildData(viewUsage) {
    var e_1, _a, e_2, _b;
    var data = [];
    try {
        for (var _c = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(viewUsage.Blocks), _d = _c.next(); !_d.done; _d = _c.next()) {
            var block = _d.value;
            if (block.Modules.length === 0) {
                data.push({
                    Block: { Id: block.Id, Guid: block.Guid },
                });
            }
            try {
                for (var _e = (e_2 = void 0, Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(block.Modules)), _f = _e.next(); !_f.done; _f = _e.next()) {
                    var module_1 = _f.value;
                    data.push({
                        Block: { Id: block.Id, Guid: block.Guid },
                        Module: { Id: module_1.Id, ModuleId: module_1.ModuleId, Title: module_1.Title },
                        PageId: module_1.Page.Id,
                        Name: module_1.Page.Name,
                        Url: module_1.Page.Url,
                        Language: module_1.Page.CultureCode,
                        Status: { IsVisible: module_1.Page.Visible, IsDeleted: module_1.IsDeleted },
                    });
                }
            }
            catch (e_2_1) { e_2 = { error: e_2_1 }; }
            finally {
                try {
                    if (_f && !_f.done && (_b = _e.return)) _b.call(_e);
                }
                finally { if (e_2) throw e_2.error; }
            }
        }
    }
    catch (e_1_1) { e_1 = { error: e_1_1 }; }
    finally {
        try {
            if (_d && !_d.done && (_a = _c.return)) _a.call(_c);
        }
        finally { if (e_1) throw e_1.error; }
    }
    return data;
}


/***/ }),

/***/ "./src/app/app-administration/views/views.component.scss":
/*!***************************************************************!*\
  !*** ./src/app/app-administration/views/views.component.scss ***!
  \***************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".more-actions-box {\n  margin-right: 66px;\n  display: flex;\n  align-items: flex-end;\n}\n\n.polymorph-logo {\n  width: 24px;\n  height: 24px;\n  margin-right: 4px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9hcHAtYWRtaW5pc3RyYXRpb24vdmlld3MvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXGFwcC1hZG1pbmlzdHJhdGlvblxcdmlld3NcXHZpZXdzLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi92aWV3cy92aWV3cy5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNFLGtCQUFBO0VBQ0EsYUFBQTtFQUNBLHFCQUFBO0FDQ0Y7O0FERUE7RUFDRSxXQUFBO0VBQ0EsWUFBQTtFQUNBLGlCQUFBO0FDQ0YiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2FwcC1hZG1pbmlzdHJhdGlvbi92aWV3cy92aWV3cy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5tb3JlLWFjdGlvbnMtYm94IHtcclxuICBtYXJnaW4tcmlnaHQ6IDY2cHg7XHJcbiAgZGlzcGxheTogZmxleDtcclxuICBhbGlnbi1pdGVtczogZmxleC1lbmQ7XHJcbn1cclxuXHJcbi5wb2x5bW9ycGgtbG9nbyB7XHJcbiAgd2lkdGg6IDI0cHg7XHJcbiAgaGVpZ2h0OiAyNHB4O1xyXG4gIG1hcmdpbi1yaWdodDogNHB4O1xyXG59XHJcbiIsIi5tb3JlLWFjdGlvbnMtYm94IHtcbiAgbWFyZ2luLXJpZ2h0OiA2NnB4O1xuICBkaXNwbGF5OiBmbGV4O1xuICBhbGlnbi1pdGVtczogZmxleC1lbmQ7XG59XG5cbi5wb2x5bW9ycGgtbG9nbyB7XG4gIHdpZHRoOiAyNHB4O1xuICBoZWlnaHQ6IDI0cHg7XG4gIG1hcmdpbi1yaWdodDogNHB4O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/app-administration/views/views.component.ts":
/*!*************************************************************!*\
  !*** ./src/app/app-administration/views/views.component.ts ***!
  \*************************************************************/
/*! exports provided: ViewsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ViewsComponent", function() { return ViewsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _url_loader_polymorph_logo_png__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! url-loader!./polymorph-logo.png */ "../../node_modules/url-loader/dist/cjs.js!./src/app/app-administration/views/polymorph-logo.png");
/* harmony import */ var _views_helpers__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./views.helpers */ "./src/app/app-administration/views/views.helpers.ts");
/* harmony import */ var _ag_grid_components_views_type_views_type_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../ag-grid-components/views-type/views-type.component */ "./src/app/app-administration/ag-grid-components/views-type/views-type.component.ts");
/* harmony import */ var _ag_grid_components_views_show_views_show_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../ag-grid-components/views-show/views-show.component */ "./src/app/app-administration/ag-grid-components/views-show/views-show.component.ts");
/* harmony import */ var _ag_grid_components_views_actions_views_actions_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../ag-grid-components/views-actions/views-actions.component */ "./src/app/app-administration/ag-grid-components/views-actions/views-actions.component.ts");
/* harmony import */ var _services_views_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../services/views.service */ "./src/app/app-administration/services/views.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../../shared/components/boolean-filter/boolean-filter.component */ "./src/app/shared/components/boolean-filter/boolean-filter.component.ts");
/* harmony import */ var _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ../../shared/components/id-field/id-field.component */ "./src/app/shared/components/id-field/id-field.component.ts");
/* harmony import */ var _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ../../shared/services/dialog.service */ "./src/app/shared/services/dialog.service.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ../../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");



















var ViewsComponent = /** @class */ (function () {
    function ViewsComponent(viewsService, router, route, snackBar, dialogService) {
        var _this = this;
        this.viewsService = viewsService;
        this.router = router;
        this.route = route;
        this.snackBar = snackBar;
        this.dialogService = dialogService;
        this.polymorphLogo = _url_loader_polymorph_logo_png__WEBPACK_IMPORTED_MODULE_7__["default"];
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_6__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_17__["defaultGridOptions"]), { frameworkComponents: {
                idFieldComponent: _shared_components_id_field_id_field_component__WEBPACK_IMPORTED_MODULE_15__["IdFieldComponent"],
                booleanFilterComponent: _shared_components_boolean_filter_boolean_filter_component__WEBPACK_IMPORTED_MODULE_14__["BooleanFilterComponent"],
                viewsTypeComponent: _ag_grid_components_views_type_views_type_component__WEBPACK_IMPORTED_MODULE_9__["ViewsTypeComponent"],
                viewsShowComponent: _ag_grid_components_views_show_views_show_component__WEBPACK_IMPORTED_MODULE_10__["ViewsShowComponent"],
                viewsActionsComponent: _ag_grid_components_views_actions_views_actions_component__WEBPACK_IMPORTED_MODULE_11__["ViewsActionsComponent"],
            }, columnDefs: [
                {
                    headerName: 'ID', field: 'Id', width: 70, headerClass: 'dense', cellClass: 'id-action no-padding no-outline',
                    cellRenderer: 'idFieldComponent', sortable: true, filter: 'agTextColumnFilter', valueGetter: this.idValueGetter,
                },
                {
                    headerName: 'Show', field: 'IsHidden', width: 70, headerClass: 'dense', cellClass: 'no-outline', cellRenderer: 'viewsShowComponent',
                    sortable: true, filter: 'booleanFilterComponent', valueGetter: this.showValueGetter,
                },
                {
                    headerName: 'Name', field: 'Name', flex: 2, minWidth: 250, cellClass: 'primary-action highlight',
                    sortable: true, filter: 'agTextColumnFilter', onCellClicked: this.editView.bind(this),
                },
                {
                    headerName: 'Type', field: 'Type', width: 70, headerClass: 'dense', cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter', cellRenderer: 'viewsTypeComponent', valueGetter: this.typeValueGetter,
                },
                {
                    headerName: 'Used', field: 'Used', width: 70, headerClass: 'dense', cellClass: 'primary-action highlight',
                    sortable: true, filter: 'agNumberColumnFilter', onCellClicked: function (event) { _this.openUsage(event); }
                },
                {
                    width: 120, cellClass: 'secondary-action no-padding', cellRenderer: 'viewsActionsComponent',
                    cellRendererParams: {
                        onOpenCode: this.openCode.bind(this),
                        onOpenPermissions: this.openPermissions.bind(this),
                        onDelete: this.deleteView.bind(this),
                    },
                },
                {
                    headerName: 'Url Key', field: 'ViewNameInUrl', flex: 1, minWidth: 150, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Path', field: 'TemplatePath', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Content', field: 'ContentType.Name', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Default', field: 'ContentType.DemoId', flex: 1, minWidth: 150, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter', valueGetter: this.contentDemoValueGetter,
                },
                {
                    headerName: 'Presentation', field: 'PresentationType.Name', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Default', field: 'PresentationType.DemoId', flex: 1, minWidth: 150, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter', valueGetter: this.presentationDemoValueGetter,
                },
                {
                    headerName: 'Header', field: 'ListContentType.Name', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Default', field: 'ListContentType.DemoId', flex: 1, minWidth: 150, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter', valueGetter: this.headerDemoValueGetter,
                },
                {
                    headerName: 'Header Presentation', field: 'ListPresentationType.Name', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Default', field: 'ListPresentationType.DemoId', flex: 1, minWidth: 150, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter', valueGetter: this.headerPresDemoValueGetter,
                },
            ] });
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_4__["Subscription"]();
        this.hasChild = !!this.route.snapshot.firstChild.firstChild;
    }
    ViewsComponent.prototype.ngOnInit = function () {
        this.fetchTemplates();
        this.fetchPolymorphism();
        this.refreshOnChildClosed();
    };
    ViewsComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ViewsComponent.prototype.fetchTemplates = function () {
        var _this = this;
        this.viewsService.getAll().subscribe(function (views) {
            _this.views = views;
        });
    };
    ViewsComponent.prototype.fetchPolymorphism = function () {
        var _this = this;
        this.viewsService.getPolymorphism().subscribe(function (polymorphism) {
            _this.polymorphism = polymorphism;
            _this.polymorphStatus = (polymorphism.Id === null)
                ? 'not configured'
                : (polymorphism.Resolver === null ? 'disabled' : 'using ' + polymorphism.Resolver);
        });
    };
    ViewsComponent.prototype.editView = function (params) {
        var view = params === null || params === void 0 ? void 0 : params.data;
        var form = {
            items: [
                view == null
                    ? { ContentTypeName: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_13__["eavConstants"].contentTypes.template }
                    : { EntityId: view.Id }
            ],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_18__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route.firstChild });
    };
    ViewsComponent.prototype.editPolymorphisms = function () {
        if (!this.polymorphism) {
            return;
        }
        var form = {
            items: [
                !this.polymorphism.Id
                    ? { ContentTypeName: this.polymorphism.TypeName }
                    : { EntityId: this.polymorphism.Id }
            ],
        };
        var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_18__["convertFormToUrl"])(form);
        this.router.navigate(["edit/" + formUrl], { relativeTo: this.route.firstChild });
    };
    ViewsComponent.prototype.idValueGetter = function (params) {
        var view = params.data;
        return "ID: " + view.Id + "\nGUID: " + view.Guid;
    };
    ViewsComponent.prototype.showValueGetter = function (params) {
        var view = params.data;
        return !view.IsHidden;
    };
    ViewsComponent.prototype.typeValueGetter = function (params) {
        var view = params.data;
        var type = Object(_views_helpers__WEBPACK_IMPORTED_MODULE_8__["calculateViewType"])(view);
        return type.value;
    };
    ViewsComponent.prototype.contentDemoValueGetter = function (params) {
        var view = params.data;
        return view.ContentType.DemoId + " " + view.ContentType.DemoTitle;
    };
    ViewsComponent.prototype.presentationDemoValueGetter = function (params) {
        var view = params.data;
        return view.PresentationType.DemoId + " " + view.PresentationType.DemoTitle;
    };
    ViewsComponent.prototype.headerDemoValueGetter = function (params) {
        var view = params.data;
        return view.ListContentType.DemoId + " " + view.ListContentType.DemoTitle;
    };
    ViewsComponent.prototype.headerPresDemoValueGetter = function (params) {
        var view = params.data;
        return view.ListPresentationType.DemoId + " " + view.ListPresentationType.DemoTitle;
    };
    ViewsComponent.prototype.openUsage = function (event) {
        var view = event.data;
        this.router.navigate(["usage/" + view.Guid], { relativeTo: this.route.firstChild });
    };
    ViewsComponent.prototype.openCode = function (view) {
        this.dialogService.openCodeFile(view.TemplatePath);
    };
    ViewsComponent.prototype.openPermissions = function (view) {
        this.router.navigate(["permissions/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_13__["eavConstants"].metadata.entity.type + "/" + _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_13__["eavConstants"].keyTypes.guid + "/" + view.Guid], { relativeTo: this.route.firstChild });
    };
    ViewsComponent.prototype.deleteView = function (view) {
        var _this = this;
        if (!confirm("Delete '" + view.Name + "' (" + view.Id + ")?")) {
            return;
        }
        this.snackBar.open('Deleting...');
        this.viewsService.delete(view.Id).subscribe(function (res) {
            _this.snackBar.open('Deleted', null, { duration: 2000 });
            _this.fetchTemplates();
        });
    };
    ViewsComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_5__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; })).subscribe(function (event) {
            var hadChild = _this.hasChild;
            _this.hasChild = !!_this.route.snapshot.firstChild.firstChild;
            if (!_this.hasChild && hadChild) {
                _this.fetchTemplates();
                _this.fetchPolymorphism();
            }
        }));
    };
    ViewsComponent.ctorParameters = function () { return [
        { type: _services_views_service__WEBPACK_IMPORTED_MODULE_12__["ViewsService"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__["MatSnackBar"] },
        { type: _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_16__["DialogService"] }
    ]; };
    ViewsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-views',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./views.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/views/views.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./views.component.scss */ "./src/app/app-administration/views/views.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_services_views_service__WEBPACK_IMPORTED_MODULE_12__["ViewsService"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_3__["MatSnackBar"],
            _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_16__["DialogService"]])
    ], ViewsComponent);
    return ViewsComponent;
}());



/***/ }),

/***/ "./src/app/app-administration/views/views.helpers.ts":
/*!***********************************************************!*\
  !*** ./src/app/app-administration/views/views.helpers.ts ***!
  \***********************************************************/
/*! exports provided: calculateViewType */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "calculateViewType", function() { return calculateViewType; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function calculateViewType(view) {
    var value = '';
    var icon = '';
    if (view.HasQuery) {
        value = 'Data (from query)';
        icon = 'filter_list';
    }
    else if (view.List) {
        value = 'Items (list)';
        icon = 'format_list_numbered';
    }
    else if (!view.ContentType && !view.HasQuery) {
        value = 'Code';
        icon = 'check_box_outline_blank';
    }
    else if (!view.List) {
        value = 'Item (one)';
        icon = 'looks_one';
    }
    return { value: value, icon: icon };
}


/***/ }),

/***/ "./src/app/app-administration/web-api/web-api.component.scss":
/*!*******************************************************************!*\
  !*** ./src/app/app-administration/web-api/web-api.component.scss ***!
  \*******************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvYXBwLWFkbWluaXN0cmF0aW9uL3dlYi1hcGkvd2ViLWFwaS5jb21wb25lbnQuc2NzcyJ9 */");

/***/ }),

/***/ "./src/app/app-administration/web-api/web-api.component.ts":
/*!*****************************************************************!*\
  !*** ./src/app/app-administration/web-api/web-api.component.ts ***!
  \*****************************************************************/
/*! exports provided: WebApiComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "WebApiComponent", function() { return WebApiComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @ag-grid-community/all-modules */ "../../node_modules/@ag-grid-community/all-modules/dist/es6/main.js");
/* harmony import */ var _services_web_apis_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../services/web-apis.service */ "./src/app/app-administration/services/web-apis.service.ts");
/* harmony import */ var _ag_grid_components_web_api_actions_web_api_actions_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../ag-grid-components/web-api-actions/web-api-actions.component */ "./src/app/app-administration/ag-grid-components/web-api-actions/web-api-actions.component.ts");
/* harmony import */ var _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../../../../edit/eav-material-controls/adam/sanitize.service */ "../edit/eav-material-controls/adam/sanitize.service.ts");
/* harmony import */ var _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../shared/services/dialog.service */ "./src/app/shared/services/dialog.service.ts");
/* harmony import */ var _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../../shared/constants/default-grid-options.constants */ "./src/app/shared/constants/default-grid-options.constants.ts");
/* harmony import */ var _shared_constants_file_names_constants__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../../shared/constants/file-names.constants */ "./src/app/shared/constants/file-names.constants.ts");










var WebApiComponent = /** @class */ (function () {
    function WebApiComponent(webApisService, sanitizeService, snackBar, dialogService) {
        this.webApisService = webApisService;
        this.sanitizeService = sanitizeService;
        this.snackBar = snackBar;
        this.dialogService = dialogService;
        this.modules = _ag_grid_community_all_modules__WEBPACK_IMPORTED_MODULE_3__["AllCommunityModules"];
        this.gridOptions = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])(Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, _shared_constants_default_grid_options_constants__WEBPACK_IMPORTED_MODULE_8__["defaultGridOptions"]), { frameworkComponents: {
                webApiActions: _ag_grid_components_web_api_actions_web_api_actions_component__WEBPACK_IMPORTED_MODULE_5__["WebApiActionsComponent"],
            }, columnDefs: [
                {
                    headerName: 'Folder', field: 'folder', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    headerName: 'Name', field: 'name', flex: 2, minWidth: 250, cellClass: 'no-outline',
                    sortable: true, filter: 'agTextColumnFilter',
                },
                {
                    width: 80, cellClass: 'secondary-action no-padding', cellRenderer: 'webApiActions', cellRendererParams: {
                        onOpenCode: this.openCode.bind(this),
                        onDelete: this.deleteApi.bind(this),
                    },
                },
            ] });
    }
    WebApiComponent.prototype.ngOnInit = function () {
        this.fetchWebApis();
    };
    WebApiComponent.prototype.addController = function () {
        var _this = this;
        var name = prompt('Controller name:', _shared_constants_file_names_constants__WEBPACK_IMPORTED_MODULE_9__["defaultControllerName"]);
        if (name === null || name.length === 0) {
            return;
        }
        name = this.sanitizeService.sanitizePath(name);
        name = name.replace(/\s/g, ''); // remove all whitespaces
        // find name without extension
        var nameLower = name.toLocaleLowerCase();
        var extIndex = nameLower.lastIndexOf('.cs');
        if (extIndex > 0) {
            nameLower = nameLower.substring(0, extIndex);
        }
        var typeIndex = nameLower.lastIndexOf('controller');
        if (typeIndex > 0) {
            nameLower = nameLower.substring(0, typeIndex);
        }
        // uppercase first letter, take other letters as is and append extension
        name = name.charAt(0).toLocaleUpperCase() + name.substring(1, nameLower.length) + 'Controller.cs';
        this.snackBar.open('Saving...');
        this.webApisService.create(name).subscribe(function (res) {
            _this.snackBar.open('Saved', null, { duration: 2000 });
            _this.fetchWebApis();
        });
    };
    WebApiComponent.prototype.fetchWebApis = function () {
        var _this = this;
        this.webApisService.getAll().subscribe(function (paths) {
            _this.webApis = paths.map(function (path) {
                var splitIndex = path.lastIndexOf('/');
                var fileExtIndex = path.lastIndexOf('.');
                var folder = path.substring(0, splitIndex);
                var name = path.substring(splitIndex + 1, fileExtIndex);
                return {
                    folder: folder,
                    name: name,
                };
            });
        });
    };
    WebApiComponent.prototype.openCode = function (api) {
        this.dialogService.openCodeFile(api.folder + "/" + api.name + ".cs");
    };
    WebApiComponent.prototype.deleteApi = function (api) {
        alert('Delete api');
    };
    WebApiComponent.ctorParameters = function () { return [
        { type: _services_web_apis_service__WEBPACK_IMPORTED_MODULE_4__["WebApisService"] },
        { type: _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_6__["SanitizeService"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"] },
        { type: _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_7__["DialogService"] }
    ]; };
    WebApiComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-web-api',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./web-api.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/app-administration/web-api/web-api.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./web-api.component.scss */ "./src/app/app-administration/web-api/web-api.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_services_web_apis_service__WEBPACK_IMPORTED_MODULE_4__["WebApisService"],
            _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_6__["SanitizeService"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"],
            _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_7__["DialogService"]])
    ], WebApiComponent);
    return WebApiComponent;
}());



/***/ })

}]);
//# sourceMappingURL=app-administration-app-administration-module.js.map