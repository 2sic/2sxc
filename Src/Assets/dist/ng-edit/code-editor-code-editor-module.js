(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["code-editor-code-editor-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-editor.component.html":
/*!********************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-editor.component.html ***!
  \********************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"editor-root\">\r\n  <div class=\"editor-side-toolbar\">\r\n    <div class=\"button\" [ngClass]=\"{'active': activeExplorer === explorer.templates}\" matTooltip=\"Templates\"\r\n      (click)=\"toggleExplorer(explorer.templates)\">\r\n      <mat-icon>file_copy</mat-icon>\r\n    </div>\r\n    <div class=\"button\" [ngClass]=\"{'active': activeExplorer === explorer.snippets}\" matTooltip=\"Snippets\"\r\n      (click)=\"toggleExplorer(explorer.snippets)\">\r\n      <mat-icon>code</mat-icon>\r\n    </div>\r\n  </div>\r\n\r\n  <app-code-templates [hidden]=\"activeExplorer !== explorer.templates\" [view]=\"view\" [templates]=\"templates\"\r\n    (createTemplate)=\"createTemplate($event)\">\r\n  </app-code-templates>\r\n  <app-code-snippets [hidden]=\"activeExplorer !== explorer.snippets\" [snippets]=\"explorerSnipps\"\r\n    (insertSnippet)=\"changeInsertSnipp($event)\">\r\n  </app-code-snippets>\r\n\r\n  <app-ace-editor *ngIf=\"view\" [filename]=\"view.FileName\" [(ngModel)]=\"view.Code\" [snippets]=\"editorSnipps\"\r\n    [insertSnippet]=\"insertSnipp\" [toggleResize]=\"!this.activeExplorer\">\r\n  </app-ace-editor>\r\n\r\n  <button mat-fab mat-elevation-z24 matTooltip=\"Click to save or CTRL + S\" (click)=\"save()\">\r\n    <mat-icon>done</mat-icon>\r\n  </button>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-snippets/code-snippets.component.html":
/*!************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-snippets/code-snippets.component.html ***!
  \************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"editor-active-explorer fancy-scrollbar-dark\">\r\n  <div *ngFor=\"let section of snippets | keyvalue\">\r\n\r\n    <div class=\"collapsible-header section\" [matTooltip]=\"'SourceEditorSnippets.' + section.key + '.Help' | translate\"\r\n      matTooltipShowDelay=\"750\" (click)=\"toggleSection(section)\">\r\n      <mat-icon>{{ toggledSections.includes(section) ? 'keyboard_arrow_down' : 'keyboard_arrow_right' }}</mat-icon>\r\n      <span>{{ 'SourceEditorSnippets.' + section.key + '.Title' | translate }}</span>\r\n    </div>\r\n\r\n    <ng-container *ngIf=\"toggledSections.includes(section)\">\r\n      <div *ngFor=\"let folder of section.value | keyvalue\">\r\n\r\n        <div class=\"collapsible-header folder\"\r\n          [matTooltip]=\"'SourceEditorSnippets.' + section.key + '.' + folder.key + '.Help' | translate\"\r\n          matTooltipShowDelay=\"750\" (click)=\"toggleFolder(folder)\">\r\n          <mat-icon>{{ toggledFolders.includes(folder) ? 'keyboard_arrow_down' : 'keyboard_arrow_right' }}</mat-icon>\r\n          <span>{{ 'SourceEditorSnippets.' + section.key + '.' + folder.key + '.Title' | translate }}</span>\r\n        </div>\r\n\r\n        <ng-container *ngIf=\"toggledFolders.includes(folder)\">\r\n          <div class=\"snippet\" *ngFor=\"let snippet of (folder.value | objectToArray)\">\r\n\r\n            <div class=\"snippet-label\">\r\n              <div matRipple class=\"text\" [matTooltip]=\"snippet.snip\" matTooltipShowDelay=\"750\"\r\n                (click)=\"addSnippet(snippet.snip)\">\r\n                {{ snippet.label }}\r\n              </div>\r\n              <mat-icon *ngIf=\"snippet.more\" matTooltip=\"Show More\" matTooltipShowDelay=\"750\"\r\n                (click)=\"toggleMore(snippet)\">\r\n                {{ toggledMores.includes(snippet) ? 'more_vert' : 'more_horiz' }}\r\n              </mat-icon>\r\n              <mat-icon matTooltip=\"Show Help\" matTooltipShowDelay=\"750\" appClickStopPropagation\r\n                (click)=\"toggleInfo(snippet)\">\r\n                info\r\n              </mat-icon>\r\n            </div>\r\n\r\n            <div class=\"snippet-help\" *ngIf=\"toggledInfos.includes(snippet)\">\r\n              <div class=\"text\" [innerHTML]=\"snippet.help | safeHtml\"></div>\r\n              <div *ngFor=\"let link of snippet.links\">\r\n                <a [href]=\"link.url\" target=\"_blank\">{{ link.name }}</a>\r\n              </div>\r\n            </div>\r\n\r\n            <ng-container *ngIf=\"toggledMores.includes(snippet)\">\r\n              <div *ngFor=\"let more of (snippet.more | objectToArray)\">\r\n\r\n                <div class=\"snippet-label\">\r\n                  <div matRipple class=\"text text-more\" [matTooltip]=\"more.snip\" matTooltipShowDelay=\"750\"\r\n                    (click)=\"addSnippet(more.snip)\">\r\n                    {{ more.label }}\r\n                  </div>\r\n                  <mat-icon matTooltip=\"Show Help\" matTooltipShowDelay=\"750\" appClickStopPropagation\r\n                    (click)=\"toggleInfo(more)\">\r\n                    info\r\n                  </mat-icon>\r\n                </div>\r\n\r\n                <div class=\"snippet-help snippet-help-more\" *ngIf=\"toggledInfos.includes(more)\">\r\n                  <div class=\"text\" [innerHTML]=\"more.help | safeHtml\"></div>\r\n                  <div *ngFor=\"let link of more.links\">\r\n                    <a [href]=\"link.url\" target=\"_blank\">{{ link.name }}</a>\r\n                  </div>\r\n                </div>\r\n\r\n              </div>\r\n            </ng-container>\r\n\r\n          </div>\r\n        </ng-container>\r\n\r\n      </div>\r\n    </ng-container>\r\n\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-templates/code-templates.component.html":
/*!**************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-templates/code-templates.component.html ***!
  \**************************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"editor-active-explorer fancy-scrollbar-dark explorer-wrapper\">\r\n\r\n  <div>\r\n    <ng-container *ngFor=\"let item of (tree | sortItems)\">\r\n      <ng-container *ngTemplateOutlet=\"fileOrFolder; context: {item: item}\"></ng-container>\r\n    </ng-container>\r\n\r\n    <ng-template #fileOrFolder let-item=\"item\">\r\n\r\n      <div class=\"file\" *ngIf=\"!item.isFolder\" [style]=\"item.depth | depthPadding: item.isFolder\"\r\n        [ngClass]=\"{'active': toggledItems.includes(item.pathFromRoot) }\" matTooltipShowDelay=\"750\"\r\n        [matTooltip]=\"item.name\" (click)=\"openTemplate(item.pathFromRoot)\">\r\n        {{ item.name }}\r\n      </div>\r\n\r\n      <div *ngIf=\"item.isFolder\">\r\n        <div class=\"folder-name-wrapper\" [style]=\"item.depth | depthPadding: item.isFolder\"\r\n          (click)=\"toggleItem(item.pathFromRoot)\">\r\n          <div class=\"folder-name\" matTooltipShowDelay=\"750\" [matTooltip]=\"item.name\">\r\n            <mat-icon>\r\n              {{ toggledItems.includes(item.pathFromRoot) ? 'keyboard_arrow_down' : 'keyboard_arrow_right' }}\r\n            </mat-icon>\r\n            <span>{{ item.name }}</span>\r\n          </div>\r\n          <div class=\"add-item\" matTooltipShowDelay=\"750\" matTooltip=\"Create file in this folder\"\r\n            appClickStopPropagation (click)=\"addFile(item.pathFromRoot)\">\r\n            <mat-icon>add</mat-icon>\r\n          </div>\r\n        </div>\r\n\r\n        <ng-container *ngIf=\"toggledItems.includes(item.pathFromRoot)\">\r\n          <ng-container *ngFor=\"let item1 of (item.children | sortItems)\">\r\n            <ng-container *ngTemplateOutlet=\"fileOrFolder; context: {item: item1}\"></ng-container>\r\n          </ng-container>\r\n        </ng-container>\r\n      </div>\r\n\r\n    </ng-template>\r\n  </div>\r\n\r\n  <div class=\"create-button-wrapper\">\r\n    <button mat-icon-button matTooltip=\"Create file\" (click)=\"addFile()\">\r\n      <mat-icon>add</mat-icon>\r\n    </button>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../edit/eav-material-controls/adam/sanitize.service.ts":
/*!**************************************************************!*\
  !*** ../edit/eav-material-controls/adam/sanitize.service.ts ***!
  \**************************************************************/
/*! exports provided: SanitizeService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SanitizeService", function() { return SanitizeService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var SanitizeService = /** @class */ (function () {
    function SanitizeService() {
        this.cleanBadPath = function (sanitized) {
            // check for undefined
            if (!sanitized) {
                return sanitized;
            }
            var goodChar = '_';
            var illegalRe = /[\?<>\\:\*\|":]/g;
            var controlRe = /[\x00-\x1f\x80-\x9f]/g;
            var reservedRe = /^\.+$/;
            var windowsReservedRe = /^(con|prn|aux|nul|com[0-9]|lpt[0-9])(\..*)?$/i;
            var windowsTrailingRe = /[\. ]+$/;
            return sanitized
                .replace(illegalRe, goodChar)
                .replace(controlRe, goodChar)
                .replace(reservedRe, goodChar)
                .replace(windowsReservedRe, goodChar)
                .replace(windowsTrailingRe, goodChar);
        };
    }
    SanitizeService.prototype.removeFromStart = function (sanitized, charToRemove) {
        // check for undefined
        if (!sanitized) {
            return sanitized;
        }
        while (sanitized.substring(0, 1) === charToRemove) {
            sanitized = sanitized.substring(1);
        }
        return sanitized;
    };
    SanitizeService.prototype.removeFromEnd = function (sanitized, charToRemove) {
        // check for undefined
        if (!sanitized) {
            return sanitized;
        }
        while (sanitized.substring(sanitized.length - 1, sanitized.length) === charToRemove) {
            sanitized = sanitized.substring(0, sanitized.length - 1);
        }
        return sanitized;
    };
    // sanitize path
    SanitizeService.prototype.sanitizePath = function (sanitized) {
        // check for undefined
        if (!sanitized) {
            return sanitized;
        }
        // remove slashes form start of path
        sanitized = this.removeFromStart(sanitized, '\/');
        // remove slashed form end of path
        sanitized = this.removeFromEnd(sanitized, '\/');
        // remove backslashes form start of path
        sanitized = this.removeFromStart(sanitized, '\\');
        // remove backslashes form end of path
        sanitized = this.removeFromEnd(sanitized, '\\');
        // replace bad
        sanitized = this.cleanBadPath(sanitized);
        return sanitized;
    };
    // sanitize file or folder name
    SanitizeService.prototype.sanitizeName = function (sanitized) {
        // check for undefined
        if (!sanitized) {
            return sanitized;
        }
        // in addition to all path validation rules
        // slashes are not valid in file or folder name
        var replacement = '_';
        var illegalRe = /\//g;
        return this.sanitizePath(sanitized)
            .replace(illegalRe, replacement);
    };
    SanitizeService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [])
    ], SanitizeService);
    return SanitizeService;
}());



/***/ }),

/***/ "./src/app/code-editor/ace-editor/ace-editor.component.ts":
/*!****************************************************************!*\
  !*** ./src/app/code-editor/ace-editor/ace-editor.component.ts ***!
  \****************************************************************/
/*! exports provided: AceEditorComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AceEditorComponent", function() { return AceEditorComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _shared_helpers_load_scripts_helper__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../shared/helpers/load-scripts.helper */ "./src/app/shared/helpers/load-scripts.helper.ts");
/* harmony import */ var _ace_options__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./ace-options */ "./src/app/code-editor/ace-editor/ace-options.ts");

// tslint:disable-next-line:max-line-length




var AceEditorComponent = /** @class */ (function () {
    function AceEditorComponent(zone) {
        this.zone = zone;
        this.value = '';
        this.propagateChange = function () { };
        this.propagateTouched = function () { };
    }
    AceEditorComponent_1 = AceEditorComponent;
    AceEditorComponent.prototype.ngOnInit = function () {
        Object(_shared_helpers_load_scripts_helper__WEBPACK_IMPORTED_MODULE_3__["loadScripts"])([
            { test: 'ace', src: 'https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11/ace.min.js' },
            { test: null, src: 'https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11/ext-modelist.min.js' },
            { test: null, src: 'https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11/ext-language_tools.min.js' },
        ], this.aceLoaded.bind(this));
    };
    AceEditorComponent.prototype.ngOnChanges = function (changes) {
        var _this = this;
        var _a, _b, _c, _d, _e;
        var filename = (_a = changes.filename) === null || _a === void 0 ? void 0 : _a.currentValue;
        var snippets = (_b = changes.snippets) === null || _b === void 0 ? void 0 : _b.currentValue;
        this.updateValues(filename, snippets);
        if (!this.editor) {
            return;
        }
        var resize = ((_c = changes.toggleResize) === null || _c === void 0 ? void 0 : _c.currentValue) !== ((_d = changes.toggleResize) === null || _d === void 0 ? void 0 : _d.previousValue);
        if (resize) {
            this.zone.runOutsideAngular(function () {
                // wait for Angular to update DOM to be able to get proper size of the container
                setTimeout(function () { _this.editor.resize(); }, 50);
            });
        }
        var snippet = (_e = changes.insertSnippet) === null || _e === void 0 ? void 0 : _e.currentValue;
        if (snippet) {
            this.zone.runOutsideAngular(function () {
                var snippetManager = ace.require('ace/snippets').snippetManager;
                snippetManager.insertSnippet(_this.editor, snippet);
                _this.editor.focus();
            });
        }
    };
    AceEditorComponent.prototype.writeValue = function (value) {
        var _this = this;
        this.value = value || '';
        if (!this.editor) {
            return;
        }
        this.zone.runOutsideAngular(function () {
            var p = _this.editor.getCursorPosition();
            _this.editor.setValue(_this.value, -1);
            _this.editor.moveCursorToPosition(p);
        });
    };
    AceEditorComponent.prototype.registerOnChange = function (fn) {
        this.propagateChange = fn;
    };
    AceEditorComponent.prototype.registerOnTouched = function (fn) {
        this.propagateTouched = fn;
    };
    AceEditorComponent.prototype.ngOnDestroy = function () {
        var _this = this;
        this.zone.runOutsideAngular(function () {
            _this.editor.destroy();
            _this.editor.container.remove();
            _this.editor = null;
        });
    };
    AceEditorComponent.prototype.aceLoaded = function () {
        var _this = this;
        this.zone.runOutsideAngular(function () {
            ace.config.set('basePath', 'https://cdnjs.cloudflare.com/ajax/libs/ace/1.4.11');
            _this.editor = ace.edit(_this.editorRef.nativeElement, _ace_options__WEBPACK_IMPORTED_MODULE_4__["aceOptions"]);
            _this.editor.$blockScrolling = Infinity;
            _this.editor.session.setValue(_this.value); // set value and reset undo history
            _this.updateValues(_this.filename, _this.snippets);
            _this.editor.on('change', _this.onEditorValueChange.bind(_this));
            _this.editor.on('blur', _this.onEditorBlurred.bind(_this));
            _this.editor.focus();
        });
    };
    AceEditorComponent.prototype.onEditorValueChange = function () {
        var _this = this;
        this.zone.run(function () {
            _this.propagateChange(_this.editor.getValue());
        });
    };
    AceEditorComponent.prototype.onEditorBlurred = function () {
        var _this = this;
        this.zone.run(function () {
            _this.propagateTouched(_this.editor.getValue());
        });
    };
    AceEditorComponent.prototype.updateValues = function (filename, snippets) {
        var _this = this;
        if (!this.editor) {
            return;
        }
        this.zone.runOutsideAngular(function () {
            if (filename) {
                var modelist = ace.require('ace/ext/modelist');
                var mode = modelist.getModeForPath(filename).mode;
                _this.editor.session.setMode(mode);
            }
            if (snippets) {
                var snippetManager = ace.require('ace/snippets').snippetManager;
                snippetManager.register(_this.snippets);
            }
        });
    };
    var AceEditorComponent_1;
    AceEditorComponent.ctorParameters = function () { return [
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["NgZone"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ViewChild"])('editor'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_1__["ElementRef"])
    ], AceEditorComponent.prototype, "editorRef", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", String)
    ], AceEditorComponent.prototype, "filename", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Array)
    ], AceEditorComponent.prototype, "snippets", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], AceEditorComponent.prototype, "insertSnippet", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Boolean)
    ], AceEditorComponent.prototype, "toggleResize", void 0);
    AceEditorComponent = AceEditorComponent_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-ace-editor',
            template: "<div style=\"width: 100%; height: 100%;\" #editor></div>",
            providers: [{
                    provide: _angular_forms__WEBPACK_IMPORTED_MODULE_2__["NG_VALUE_ACCESSOR"],
                    useExisting: Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["forwardRef"])(function () { return AceEditorComponent_1; }),
                    multi: true
                }],
            changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectionStrategy"].OnPush,
            styles: [":host {display: block; width: 100%; height: 100%}"]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgZone"]])
    ], AceEditorComponent);
    return AceEditorComponent;
}());



/***/ }),

/***/ "./src/app/code-editor/ace-editor/ace-options.ts":
/*!*******************************************************!*\
  !*** ./src/app/code-editor/ace-editor/ace-options.ts ***!
  \*******************************************************/
/*! exports provided: aceOptions */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "aceOptions", function() { return aceOptions; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var aceOptions = {
    mode: 'ace/mode/razor',
    wrap: true,
    useSoftTabs: true,
    theme: 'ace/theme/sqlserver',
    fontSize: 14,
    fontFamily: 'Consolas, Courier New, monospace',
    showGutter: true,
    enableBasicAutocompletion: true,
    enableLiveAutocompletion: true,
    enableSnippets: true,
};


/***/ }),

/***/ "./src/app/code-editor/code-editor-routing.module.ts":
/*!***********************************************************!*\
  !*** ./src/app/code-editor/code-editor-routing.module.ts ***!
  \***********************************************************/
/*! exports provided: CodeEditorRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CodeEditorRoutingModule", function() { return CodeEditorRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _code_editor_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./code-editor.component */ "./src/app/code-editor/code-editor.component.ts");




var routes = [
    { path: '', component: _code_editor_component__WEBPACK_IMPORTED_MODULE_3__["CodeEditorComponent"] },
];
var CodeEditorRoutingModule = /** @class */ (function () {
    function CodeEditorRoutingModule() {
    }
    CodeEditorRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], CodeEditorRoutingModule);
    return CodeEditorRoutingModule;
}());



/***/ }),

/***/ "./src/app/code-editor/code-editor.component.scss":
/*!********************************************************!*\
  !*** ./src/app/code-editor/code-editor.component.scss ***!
  \********************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".mat-fab {\n  position: absolute;\n  right: 48px;\n  bottom: 32px;\n  z-index: 10;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb2RlLWVkaXRvci9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcY29kZS1lZGl0b3JcXGNvZGUtZWRpdG9yLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2NvZGUtZWRpdG9yL2NvZGUtZWRpdG9yLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0Usa0JBQUE7RUFDQSxXQUFBO0VBQ0EsWUFBQTtFQUNBLFdBQUE7QUNDRiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvY29kZS1lZGl0b3IvY29kZS1lZGl0b3IuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIubWF0LWZhYiB7XHJcbiAgcG9zaXRpb246IGFic29sdXRlO1xyXG4gIHJpZ2h0OiA0OHB4O1xyXG4gIGJvdHRvbTogMzJweDtcclxuICB6LWluZGV4OiAxMDtcclxufVxyXG4iLCIubWF0LWZhYiB7XG4gIHBvc2l0aW9uOiBhYnNvbHV0ZTtcbiAgcmlnaHQ6IDQ4cHg7XG4gIGJvdHRvbTogMzJweDtcbiAgei1pbmRleDogMTA7XG59Il19 */");

/***/ }),

/***/ "./src/app/code-editor/code-editor.component.ts":
/*!******************************************************!*\
  !*** ./src/app/code-editor/code-editor.component.ts ***!
  \******************************************************/
/*! exports provided: CodeEditorComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CodeEditorComponent", function() { return CodeEditorComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/platform-browser.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../shared/constants/session.constants */ "./src/app/shared/constants/session.constants.ts");
/* harmony import */ var _services_source_service__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./services/source.service */ "./src/app/code-editor/services/source.service.ts");
/* harmony import */ var _services_snippets_service__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./services/snippets.service */ "./src/app/code-editor/services/snippets.service.ts");
/* harmony import */ var _shared_services_snack_bar_stack_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../shared/services/snack-bar-stack.service */ "./src/app/shared/services/snack-bar-stack.service.ts");
/* harmony import */ var _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../shared/services/dialog.service */ "./src/app/shared/services/dialog.service.ts");
/* harmony import */ var _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../../../../edit/eav-material-controls/adam/sanitize.service */ "../edit/eav-material-controls/adam/sanitize.service.ts");
/* harmony import */ var _shared_constants_file_names_constants__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../shared/constants/file-names.constants */ "./src/app/shared/constants/file-names.constants.ts");














var CodeEditorComponent = /** @class */ (function () {
    function CodeEditorComponent(context, route, snackBar, snackBarStack, sourceService, snippetsService, zone, titleService, dialogService, sanitizeService) {
        this.context = context;
        this.route = route;
        this.snackBar = snackBar;
        this.snackBarStack = snackBarStack;
        this.sourceService = sourceService;
        this.snippetsService = snippetsService;
        this.zone = zone;
        this.titleService = titleService;
        this.dialogService = dialogService;
        this.sanitizeService = sanitizeService;
        this.explorer = {
            templates: 'templates',
            snippets: 'snippets'
        };
        this.activeExplorer = this.explorer.templates;
        this.eventListeners = [];
        this.context.init(this.route);
        this.calculateViewKey();
        this.attachListeners();
    }
    CodeEditorComponent.prototype.ngOnInit = function () {
        var _this = this;
        Object(rxjs__WEBPACK_IMPORTED_MODULE_5__["forkJoin"])({
            view: this.sourceService.get(this.viewKey),
            templates: this.sourceService.getTemplates(),
        }).subscribe(function (result) {
            _this.view = result.view;
            _this.savedCode = _this.view.Code;
            _this.titleService.setTitle(_this.view.FileName + " - Code Editor");
            _this.templates = result.templates;
            _this.showCodeAndEditionWarnings(result.view, result.templates);
            _this.snippetsService.getSnippets(_this.view).then(function (res) {
                _this.explorerSnipps = res.sets;
                _this.editorSnipps = res.list;
            });
        });
    };
    CodeEditorComponent.prototype.ngOnDestroy = function () {
        this.detachListeners();
    };
    CodeEditorComponent.prototype.toggleExplorer = function (explorer) {
        if (this.activeExplorer === explorer) {
            this.activeExplorer = null;
        }
        else {
            this.activeExplorer = explorer;
        }
    };
    CodeEditorComponent.prototype.createTemplate = function (folder) {
        var _this = this;
        var question = 'File name:';
        var suggestion = _shared_constants_file_names_constants__WEBPACK_IMPORTED_MODULE_13__["defaultTemplateName"];
        if (folder === 'api' || (folder === null || folder === void 0 ? void 0 : folder.startsWith('api/'))) {
            question = 'Controller name:';
            suggestion = _shared_constants_file_names_constants__WEBPACK_IMPORTED_MODULE_13__["defaultControllerName"];
        }
        var name = prompt(question, suggestion);
        if (name === null || name.length === 0) {
            return;
        }
        name = this.sanitizeService.sanitizePath(name);
        if (folder != null) {
            name = folder + "/" + name;
        }
        this.sourceService.createTemplate(name).subscribe(function (res) {
            _this.sourceService.getTemplates().subscribe(function (files) {
                _this.templates = files;
            });
        });
    };
    CodeEditorComponent.prototype.changeInsertSnipp = function (snippet) {
        this.insertSnipp = snippet;
    };
    CodeEditorComponent.prototype.save = function () {
        var _this = this;
        this.snackBar.open('Saving...');
        var codeToSave = this.view.Code;
        this.sourceService.save(this.viewKey, this.view).subscribe({
            next: function (res) {
                if (!res) {
                    _this.snackBar.open('Failed', null, { duration: 2000 });
                    return;
                }
                _this.savedCode = codeToSave;
                codeToSave = null;
                _this.snackBar.open('Saved', null, { duration: 2000 });
            },
            error: function () {
                _this.snackBar.open('Failed', null, { duration: 2000 });
            }
        });
    };
    CodeEditorComponent.prototype.calculateViewKey = function () {
        // spm TODO: Move items for code-editor to the url?
        var itemsRaw = sessionStorage.getItem(_shared_constants_session_constants__WEBPACK_IMPORTED_MODULE_7__["keyItems"]);
        var editItems = JSON.parse(itemsRaw);
        var item = editItems[0];
        this.viewKey = item.EntityId || item.Path;
    };
    /** Show info about editions if other files with the same name exist */
    CodeEditorComponent.prototype.showCodeAndEditionWarnings = function (view, files) {
        var _this = this;
        var pathAndName = view.FileName;
        var pathSeparator = pathAndName.indexOf('/') > -1 ? pathAndName.lastIndexOf('/') + 1 : 0;
        var pathWithSlash = pathSeparator === 0 ? '' : pathAndName.substring(0, pathSeparator);
        var fullName = pathAndName.substring(pathSeparator);
        var name = fullName.substring(0, fullName.length - view.Extension.length);
        var nameCode = name + '.code' + view.Extension;
        // find out if we also have a code file
        var codeFile = files.find(function (file) { return file === pathWithSlash + nameCode; });
        var otherEditions = files.filter(function (file) { return file.endsWith(fullName); }).length - 1;
        if (codeFile) {
            this.snackBarStack
                .add("This template also has a code-behind file '" + codeFile + "'.", 'Open')
                .subscribe(function () {
                _this.dialogService.openCodeFile(codeFile);
            });
        }
        if (otherEditions) {
            this.snackBarStack
                .add("There are " + otherEditions + " other editions of this. You may be editing an edition which is not the one you see.", 'Help')
                .subscribe(function () {
                window.open('https://r.2sxc.org/polymorphism', '_blank');
            });
        }
    };
    CodeEditorComponent.prototype.attachListeners = function () {
        var _this = this;
        this.zone.runOutsideAngular(function () {
            var closing = _this.stopClose.bind(_this);
            var save = _this.keyboardSave.bind(_this);
            window.addEventListener('beforeunload', closing);
            window.addEventListener('keydown', save);
            _this.eventListeners.push({ element: window, type: 'beforeunload', listener: closing });
            _this.eventListeners.push({ element: window, type: 'keydown', listener: save });
        });
    };
    CodeEditorComponent.prototype.detachListeners = function () {
        var _this = this;
        this.zone.runOutsideAngular(function () {
            _this.eventListeners.forEach(function (listener) {
                listener.element.removeEventListener(listener.type, listener.listener);
            });
            _this.eventListeners = null;
        });
    };
    CodeEditorComponent.prototype.stopClose = function (e) {
        if (this.savedCode === this.view.Code) {
            return;
        }
        e.preventDefault(); // Cancel the event
        e.returnValue = ''; // Chrome requires returnValue to be set
    };
    CodeEditorComponent.prototype.keyboardSave = function (e) {
        var _this = this;
        var CTRL_S = e.keyCode === 83 && (navigator.platform.match('Mac') ? e.metaKey : e.ctrlKey);
        if (!CTRL_S) {
            return;
        }
        e.preventDefault();
        this.zone.run(function () { _this.save(); });
    };
    CodeEditorComponent.ctorParameters = function () { return [
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_6__["Context"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"] },
        { type: _shared_services_snack_bar_stack_service__WEBPACK_IMPORTED_MODULE_10__["SnackBarStackService"] },
        { type: _services_source_service__WEBPACK_IMPORTED_MODULE_8__["SourceService"] },
        { type: _services_snippets_service__WEBPACK_IMPORTED_MODULE_9__["SnippetsService"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["NgZone"] },
        { type: _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["Title"] },
        { type: _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_11__["DialogService"] },
        { type: _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_12__["SanitizeService"] }
    ]; };
    CodeEditorComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-code-editor',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./code-editor.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-editor.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./code-editor.component.scss */ "./src/app/code-editor/code-editor.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_shared_services_context__WEBPACK_IMPORTED_MODULE_6__["Context"],
            _angular_router__WEBPACK_IMPORTED_MODULE_3__["ActivatedRoute"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"],
            _shared_services_snack_bar_stack_service__WEBPACK_IMPORTED_MODULE_10__["SnackBarStackService"],
            _services_source_service__WEBPACK_IMPORTED_MODULE_8__["SourceService"],
            _services_snippets_service__WEBPACK_IMPORTED_MODULE_9__["SnippetsService"],
            _angular_core__WEBPACK_IMPORTED_MODULE_1__["NgZone"],
            _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["Title"],
            _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_11__["DialogService"],
            _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_12__["SanitizeService"]])
    ], CodeEditorComponent);
    return CodeEditorComponent;
}());



/***/ }),

/***/ "./src/app/code-editor/code-editor.module.ts":
/*!***************************************************!*\
  !*** ./src/app/code-editor/code-editor.module.ts ***!
  \***************************************************/
/*! exports provided: translateLoaderFactory, CodeEditorModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "translateLoaderFactory", function() { return translateLoaderFactory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CodeEditorModule", function() { return CodeEditorModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/forms */ "../../node_modules/@angular/forms/__ivy_ngcc__/fesm5/forms.js");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @ngx-translate/core */ "../../node_modules/@ngx-translate/core/__ivy_ngcc__/fesm5/ngx-translate-core.js");
/* harmony import */ var _ngx_translate_http_loader__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @ngx-translate/http-loader */ "../../node_modules/@ngx-translate/http-loader/__ivy_ngcc__/fesm5/ngx-translate-http-loader.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _angular_material_select__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/select */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/select.js");
/* harmony import */ var _angular_material_core__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/material/core */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _code_editor_routing_module__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./code-editor-routing.module */ "./src/app/code-editor/code-editor-routing.module.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _code_editor_component__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./code-editor.component */ "./src/app/code-editor/code-editor.component.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _services_source_service__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ./services/source.service */ "./src/app/code-editor/services/source.service.ts");
/* harmony import */ var _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ../shared/services/dialog.service */ "./src/app/shared/services/dialog.service.ts");
/* harmony import */ var _code_snippets_code_snippets_component__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./code-snippets/code-snippets.component */ "./src/app/code-editor/code-snippets/code-snippets.component.ts");
/* harmony import */ var _services_snippets_service__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ./services/snippets.service */ "./src/app/code-editor/services/snippets.service.ts");
/* harmony import */ var _code_snippets_object_to_array_pipe__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ./code-snippets/object-to-array.pipe */ "./src/app/code-editor/code-snippets/object-to-array.pipe.ts");
/* harmony import */ var _code_templates_code_templates_component__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ./code-templates/code-templates.component */ "./src/app/code-editor/code-templates/code-templates.component.ts");
/* harmony import */ var _ace_editor_ace_editor_component__WEBPACK_IMPORTED_MODULE_24__ = __webpack_require__(/*! ./ace-editor/ace-editor.component */ "./src/app/code-editor/ace-editor/ace-editor.component.ts");
/* harmony import */ var _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_25__ = __webpack_require__(/*! ../../../../edit/eav-material-controls/adam/sanitize.service */ "../edit/eav-material-controls/adam/sanitize.service.ts");
/* harmony import */ var _code_templates_depth_padding_pipe__WEBPACK_IMPORTED_MODULE_26__ = __webpack_require__(/*! ./code-templates/depth-padding.pipe */ "./src/app/code-editor/code-templates/depth-padding.pipe.ts");
/* harmony import */ var _code_templates_order_items_pipe__WEBPACK_IMPORTED_MODULE_27__ = __webpack_require__(/*! ./code-templates/order-items.pipe */ "./src/app/code-editor/code-templates/order-items.pipe.ts");




























function translateLoaderFactory(http) {
    return new _ngx_translate_http_loader__WEBPACK_IMPORTED_MODULE_6__["TranslateHttpLoader"](http, './i18n/code-editor.', ".js?" + sxcVersion);
}
var CodeEditorModule = /** @class */ (function () {
    function CodeEditorModule() {
    }
    CodeEditorModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _code_editor_component__WEBPACK_IMPORTED_MODULE_16__["CodeEditorComponent"],
                _code_snippets_code_snippets_component__WEBPACK_IMPORTED_MODULE_20__["CodeSnippetsComponent"],
                _code_snippets_object_to_array_pipe__WEBPACK_IMPORTED_MODULE_22__["ObjectToArrayPipe"],
                _code_templates_code_templates_component__WEBPACK_IMPORTED_MODULE_23__["CodeTemplatesComponent"],
                _ace_editor_ace_editor_component__WEBPACK_IMPORTED_MODULE_24__["AceEditorComponent"],
                _code_templates_depth_padding_pipe__WEBPACK_IMPORTED_MODULE_26__["DepthPaddingPipe"],
                _code_templates_order_items_pipe__WEBPACK_IMPORTED_MODULE_27__["SortItemsPipe"],
            ],
            entryComponents: [
                _code_editor_component__WEBPACK_IMPORTED_MODULE_16__["CodeEditorComponent"],
                _code_snippets_code_snippets_component__WEBPACK_IMPORTED_MODULE_20__["CodeSnippetsComponent"],
                _code_templates_code_templates_component__WEBPACK_IMPORTED_MODULE_23__["CodeTemplatesComponent"],
                _ace_editor_ace_editor_component__WEBPACK_IMPORTED_MODULE_24__["AceEditorComponent"],
            ],
            imports: [
                _code_editor_routing_module__WEBPACK_IMPORTED_MODULE_14__["CodeEditorRoutingModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_15__["SharedComponentsModule"],
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_7__["MatDialogModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_8__["MatButtonModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_9__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_10__["MatTooltipModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_11__["MatSnackBarModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_4__["FormsModule"],
                _angular_material_select__WEBPACK_IMPORTED_MODULE_12__["MatSelectModule"],
                _angular_material_core__WEBPACK_IMPORTED_MODULE_13__["MatRippleModule"],
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_5__["TranslateModule"].forChild({
                    loader: {
                        provide: _ngx_translate_core__WEBPACK_IMPORTED_MODULE_5__["TranslateLoader"],
                        useFactory: (translateLoaderFactory),
                        deps: [_angular_common_http__WEBPACK_IMPORTED_MODULE_3__["HttpClient"]],
                    },
                    defaultLanguage: 'en',
                    isolate: true,
                }),
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_17__["Context"],
                _services_source_service__WEBPACK_IMPORTED_MODULE_18__["SourceService"],
                _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_19__["DialogService"],
                _services_snippets_service__WEBPACK_IMPORTED_MODULE_21__["SnippetsService"],
                _edit_eav_material_controls_adam_sanitize_service__WEBPACK_IMPORTED_MODULE_25__["SanitizeService"],
                _ngx_translate_core__WEBPACK_IMPORTED_MODULE_5__["TranslateService"],
            ]
        })
    ], CodeEditorModule);
    return CodeEditorModule;
}());



/***/ }),

/***/ "./src/app/code-editor/code-snippets/code-snippets.component.scss":
/*!************************************************************************!*\
  !*** ./src/app/code-editor/code-snippets/code-snippets.component.scss ***!
  \************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".collapsible-header {\n  display: flex;\n  align-items: center;\n  cursor: pointer;\n  overflow: hidden;\n  white-space: nowrap;\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none;\n  padding: 3px 0;\n}\n.collapsible-header.section {\n  background-color: #383838;\n  font-weight: 500;\n}\n.collapsible-header.folder {\n  padding-left: 8px;\n}\n.collapsible-header.folder:hover {\n  background-color: #2a2d2e;\n}\n.collapsible-header .mat-icon {\n  width: 18px;\n  height: 18px;\n  font-size: 18px;\n}\n.snippet {\n  padding-right: 8px;\n}\n.snippet:hover {\n  background-color: #2a2d2e;\n}\n.snippet-label {\n  display: flex;\n  align-items: center;\n  justify-content: space-between;\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none;\n}\n.snippet-label .text {\n  cursor: pointer;\n  padding: 3px 0 3px 32px;\n  flex: 1 1 auto;\n  overflow: hidden;\n  white-space: nowrap;\n  text-overflow: ellipsis;\n}\n.snippet-label .text-more {\n  padding-left: 40px;\n}\n.snippet-label .mat-icon {\n  cursor: pointer;\n  width: 18px;\n  height: 18px;\n  font-size: 18px;\n}\n.snippet-help {\n  padding: 3px 18px 3px 32px;\n}\n.snippet-help-more {\n  padding-left: 40px;\n}\n.snippet-help .text {\n  font-size: 12px;\n  font-style: oblique;\n  word-break: break-word;\n}\n.snippet-help .text ::ng-deep p {\n  margin: 0;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb2RlLWVkaXRvci9jb2RlLXNuaXBwZXRzL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFxjb2RlLWVkaXRvclxcY29kZS1zbmlwcGV0c1xcY29kZS1zbmlwcGV0cy5jb21wb25lbnQuc2NzcyIsInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb2RlLWVkaXRvci9jb2RlLXNuaXBwZXRzL2NvZGUtc25pcHBldHMuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxhQUFBO0VBQ0EsbUJBQUE7RUFDQSxlQUFBO0VBQ0EsZ0JBQUE7RUFDQSxtQkFBQTtFQUNBLHlCQUFBO0tBQUEsc0JBQUE7TUFBQSxxQkFBQTtVQUFBLGlCQUFBO0VBQ0EsY0FBQTtBQ0NGO0FEQ0U7RUFDRSx5QkFBQTtFQUNBLGdCQUFBO0FDQ0o7QURFRTtFQUNFLGlCQUFBO0FDQUo7QURFSTtFQUNFLHlCQUFBO0FDQU47QURJRTtFQUNFLFdBQUE7RUFDQSxZQUFBO0VBQ0EsZUFBQTtBQ0ZKO0FETUE7RUFDRSxrQkFBQTtBQ0hGO0FES0U7RUFDRSx5QkFBQTtBQ0hKO0FETUU7RUFDRSxhQUFBO0VBQ0EsbUJBQUE7RUFDQSw4QkFBQTtFQUNBLHlCQUFBO0tBQUEsc0JBQUE7TUFBQSxxQkFBQTtVQUFBLGlCQUFBO0FDSko7QURNSTtFQUNFLGVBQUE7RUFDQSx1QkFBQTtFQUNBLGNBQUE7RUFDQSxnQkFBQTtFQUNBLG1CQUFBO0VBQ0EsdUJBQUE7QUNKTjtBRE1NO0VBQ0Usa0JBQUE7QUNKUjtBRFFJO0VBQ0UsZUFBQTtFQUNBLFdBQUE7RUFDQSxZQUFBO0VBQ0EsZUFBQTtBQ05OO0FEVUU7RUFDRSwwQkFBQTtBQ1JKO0FEVUk7RUFDRSxrQkFBQTtBQ1JOO0FEV0k7RUFDRSxlQUFBO0VBQ0EsbUJBQUE7RUFDQSxzQkFBQTtBQ1ROO0FEV007RUFDRSxTQUFBO0FDVFIiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL2NvZGUtZWRpdG9yL2NvZGUtc25pcHBldHMvY29kZS1zbmlwcGV0cy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5jb2xsYXBzaWJsZS1oZWFkZXIge1xyXG4gIGRpc3BsYXk6IGZsZXg7XHJcbiAgYWxpZ24taXRlbXM6IGNlbnRlcjtcclxuICBjdXJzb3I6IHBvaW50ZXI7XHJcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcclxuICB3aGl0ZS1zcGFjZTogbm93cmFwO1xyXG4gIHVzZXItc2VsZWN0OiBub25lO1xyXG4gIHBhZGRpbmc6IDNweCAwO1xyXG5cclxuICAmLnNlY3Rpb24ge1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogcmdiKDU2LCA1NiwgNTYpO1xyXG4gICAgZm9udC13ZWlnaHQ6IDUwMDtcclxuICB9XHJcblxyXG4gICYuZm9sZGVyIHtcclxuICAgIHBhZGRpbmctbGVmdDogOHB4O1xyXG5cclxuICAgICY6aG92ZXIge1xyXG4gICAgICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2IoNDIsIDQ1LCA0Nik7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICAubWF0LWljb24ge1xyXG4gICAgd2lkdGg6IDE4cHg7XHJcbiAgICBoZWlnaHQ6IDE4cHg7XHJcbiAgICBmb250LXNpemU6IDE4cHg7XHJcbiAgfVxyXG59XHJcblxyXG4uc25pcHBldCB7XHJcbiAgcGFkZGluZy1yaWdodDogOHB4O1xyXG5cclxuICAmOmhvdmVyIHtcclxuICAgIGJhY2tncm91bmQtY29sb3I6IHJnYig0MiwgNDUsIDQ2KTtcclxuICB9XHJcblxyXG4gICYtbGFiZWwge1xyXG4gICAgZGlzcGxheTogZmxleDtcclxuICAgIGFsaWduLWl0ZW1zOiBjZW50ZXI7XHJcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWJldHdlZW47XHJcbiAgICB1c2VyLXNlbGVjdDogbm9uZTtcclxuXHJcbiAgICAudGV4dCB7XHJcbiAgICAgIGN1cnNvcjogcG9pbnRlcjtcclxuICAgICAgcGFkZGluZzogM3B4IDAgM3B4IDMycHg7XHJcbiAgICAgIGZsZXg6IDEgMSBhdXRvO1xyXG4gICAgICBvdmVyZmxvdzogaGlkZGVuO1xyXG4gICAgICB3aGl0ZS1zcGFjZTogbm93cmFwO1xyXG4gICAgICB0ZXh0LW92ZXJmbG93OiBlbGxpcHNpcztcclxuXHJcbiAgICAgICYtbW9yZSB7XHJcbiAgICAgICAgcGFkZGluZy1sZWZ0OiA0MHB4O1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgLm1hdC1pY29uIHtcclxuICAgICAgY3Vyc29yOiBwb2ludGVyO1xyXG4gICAgICB3aWR0aDogMThweDtcclxuICAgICAgaGVpZ2h0OiAxOHB4O1xyXG4gICAgICBmb250LXNpemU6IDE4cHg7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICAmLWhlbHAge1xyXG4gICAgcGFkZGluZzogM3B4IDE4cHggM3B4IDMycHg7XHJcblxyXG4gICAgJi1tb3JlIHtcclxuICAgICAgcGFkZGluZy1sZWZ0OiA0MHB4O1xyXG4gICAgfVxyXG5cclxuICAgIC50ZXh0IHtcclxuICAgICAgZm9udC1zaXplOiAxMnB4O1xyXG4gICAgICBmb250LXN0eWxlOiBvYmxpcXVlO1xyXG4gICAgICB3b3JkLWJyZWFrOiBicmVhay13b3JkO1xyXG5cclxuICAgICAgOjpuZy1kZWVwIHAge1xyXG4gICAgICAgIG1hcmdpbjogMDtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH1cclxufVxyXG4iLCIuY29sbGFwc2libGUtaGVhZGVyIHtcbiAgZGlzcGxheTogZmxleDtcbiAgYWxpZ24taXRlbXM6IGNlbnRlcjtcbiAgY3Vyc29yOiBwb2ludGVyO1xuICBvdmVyZmxvdzogaGlkZGVuO1xuICB3aGl0ZS1zcGFjZTogbm93cmFwO1xuICB1c2VyLXNlbGVjdDogbm9uZTtcbiAgcGFkZGluZzogM3B4IDA7XG59XG4uY29sbGFwc2libGUtaGVhZGVyLnNlY3Rpb24ge1xuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzgzODM4O1xuICBmb250LXdlaWdodDogNTAwO1xufVxuLmNvbGxhcHNpYmxlLWhlYWRlci5mb2xkZXIge1xuICBwYWRkaW5nLWxlZnQ6IDhweDtcbn1cbi5jb2xsYXBzaWJsZS1oZWFkZXIuZm9sZGVyOmhvdmVyIHtcbiAgYmFja2dyb3VuZC1jb2xvcjogIzJhMmQyZTtcbn1cbi5jb2xsYXBzaWJsZS1oZWFkZXIgLm1hdC1pY29uIHtcbiAgd2lkdGg6IDE4cHg7XG4gIGhlaWdodDogMThweDtcbiAgZm9udC1zaXplOiAxOHB4O1xufVxuXG4uc25pcHBldCB7XG4gIHBhZGRpbmctcmlnaHQ6IDhweDtcbn1cbi5zbmlwcGV0OmhvdmVyIHtcbiAgYmFja2dyb3VuZC1jb2xvcjogIzJhMmQyZTtcbn1cbi5zbmlwcGV0LWxhYmVsIHtcbiAgZGlzcGxheTogZmxleDtcbiAgYWxpZ24taXRlbXM6IGNlbnRlcjtcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xuICB1c2VyLXNlbGVjdDogbm9uZTtcbn1cbi5zbmlwcGV0LWxhYmVsIC50ZXh0IHtcbiAgY3Vyc29yOiBwb2ludGVyO1xuICBwYWRkaW5nOiAzcHggMCAzcHggMzJweDtcbiAgZmxleDogMSAxIGF1dG87XG4gIG92ZXJmbG93OiBoaWRkZW47XG4gIHdoaXRlLXNwYWNlOiBub3dyYXA7XG4gIHRleHQtb3ZlcmZsb3c6IGVsbGlwc2lzO1xufVxuLnNuaXBwZXQtbGFiZWwgLnRleHQtbW9yZSB7XG4gIHBhZGRpbmctbGVmdDogNDBweDtcbn1cbi5zbmlwcGV0LWxhYmVsIC5tYXQtaWNvbiB7XG4gIGN1cnNvcjogcG9pbnRlcjtcbiAgd2lkdGg6IDE4cHg7XG4gIGhlaWdodDogMThweDtcbiAgZm9udC1zaXplOiAxOHB4O1xufVxuLnNuaXBwZXQtaGVscCB7XG4gIHBhZGRpbmc6IDNweCAxOHB4IDNweCAzMnB4O1xufVxuLnNuaXBwZXQtaGVscC1tb3JlIHtcbiAgcGFkZGluZy1sZWZ0OiA0MHB4O1xufVxuLnNuaXBwZXQtaGVscCAudGV4dCB7XG4gIGZvbnQtc2l6ZTogMTJweDtcbiAgZm9udC1zdHlsZTogb2JsaXF1ZTtcbiAgd29yZC1icmVhazogYnJlYWstd29yZDtcbn1cbi5zbmlwcGV0LWhlbHAgLnRleHQgOjpuZy1kZWVwIHAge1xuICBtYXJnaW46IDA7XG59Il19 */");

/***/ }),

/***/ "./src/app/code-editor/code-snippets/code-snippets.component.ts":
/*!**********************************************************************!*\
  !*** ./src/app/code-editor/code-snippets/code-snippets.component.ts ***!
  \**********************************************************************/
/*! exports provided: CodeSnippetsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CodeSnippetsComponent", function() { return CodeSnippetsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _code_snippets_helpers__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./code-snippets.helpers */ "./src/app/code-editor/code-snippets/code-snippets.helpers.ts");



var CodeSnippetsComponent = /** @class */ (function () {
    function CodeSnippetsComponent() {
        this.insertSnippet = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.toggledSections = [];
        this.toggledFolders = [];
        this.toggledInfos = [];
        this.toggledMores = [];
    }
    CodeSnippetsComponent.prototype.ngOnInit = function () {
    };
    CodeSnippetsComponent.prototype.addSnippet = function (snippet) {
        this.insertSnippet.emit(snippet);
    };
    CodeSnippetsComponent.prototype.toggleSection = function (set) {
        Object(_code_snippets_helpers__WEBPACK_IMPORTED_MODULE_2__["toggleInArray"])(set, this.toggledSections);
    };
    CodeSnippetsComponent.prototype.toggleFolder = function (item) {
        Object(_code_snippets_helpers__WEBPACK_IMPORTED_MODULE_2__["toggleInArray"])(item, this.toggledFolders);
    };
    CodeSnippetsComponent.prototype.toggleInfo = function (info) {
        Object(_code_snippets_helpers__WEBPACK_IMPORTED_MODULE_2__["toggleInArray"])(info, this.toggledInfos);
    };
    CodeSnippetsComponent.prototype.toggleMore = function (more) {
        Object(_code_snippets_helpers__WEBPACK_IMPORTED_MODULE_2__["toggleInArray"])(more, this.toggledMores);
    };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], CodeSnippetsComponent.prototype, "snippets", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"])
    ], CodeSnippetsComponent.prototype, "insertSnippet", void 0);
    CodeSnippetsComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-code-snippets',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./code-snippets.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-snippets/code-snippets.component.html")).default,
            changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectionStrategy"].OnPush,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./code-snippets.component.scss */ "./src/app/code-editor/code-snippets/code-snippets.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [])
    ], CodeSnippetsComponent);
    return CodeSnippetsComponent;
}());



/***/ }),

/***/ "./src/app/code-editor/code-snippets/code-snippets.helpers.ts":
/*!********************************************************************!*\
  !*** ./src/app/code-editor/code-snippets/code-snippets.helpers.ts ***!
  \********************************************************************/
/*! exports provided: toggleInArray */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "toggleInArray", function() { return toggleInArray; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function toggleInArray(item, array) {
    var index = array.indexOf(item);
    if (index === -1) {
        array.push(item);
    }
    else {
        array.splice(index, 1);
    }
}


/***/ }),

/***/ "./src/app/code-editor/code-snippets/object-to-array.pipe.ts":
/*!*******************************************************************!*\
  !*** ./src/app/code-editor/code-snippets/object-to-array.pipe.ts ***!
  \*******************************************************************/
/*! exports provided: ObjectToArrayPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ObjectToArrayPipe", function() { return ObjectToArrayPipe; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var ObjectToArrayPipe = /** @class */ (function () {
    function ObjectToArrayPipe() {
    }
    ObjectToArrayPipe.prototype.transform = function (obj) {
        if (typeof obj !== typeof {}) {
            return obj;
        }
        return Object.keys(obj).map(function (key) {
            return obj[key];
        });
    };
    ObjectToArrayPipe = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Pipe"])({ name: 'objectToArray' })
    ], ObjectToArrayPipe);
    return ObjectToArrayPipe;
}());



/***/ }),

/***/ "./src/app/code-editor/code-templates/code-templates.component.scss":
/*!**************************************************************************!*\
  !*** ./src/app/code-editor/code-templates/code-templates.component.scss ***!
  \**************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".explorer-wrapper {\n  display: flex;\n  flex-direction: column;\n  justify-content: space-between;\n}\n\n.create-button-wrapper {\n  display: flex;\n  justify-content: flex-end;\n}\n\n.file {\n  cursor: pointer;\n  padding: 4px 0 4px 8px;\n  overflow: hidden;\n  white-space: nowrap;\n  text-overflow: ellipsis;\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none;\n}\n\n.file:hover {\n  background-color: #2a2d2e;\n}\n\n.file.active {\n  background-color: #37373d;\n}\n\n.folder-name-wrapper {\n  display: flex;\n  cursor: pointer;\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none;\n  align-items: center;\n}\n\n.folder-name-wrapper .mat-icon {\n  width: 18px;\n  height: 18px;\n  font-size: 18px;\n}\n\n.folder-name-wrapper .folder-name {\n  display: flex;\n  align-items: center;\n  overflow: hidden;\n  white-space: nowrap;\n  padding: 3px 0;\n  flex: 1 1 auto;\n}\n\n.folder-name-wrapper .add-item {\n  margin-right: 4px;\n  display: none;\n  align-items: center;\n  justify-content: center;\n  padding: 3px;\n}\n\n.folder-name-wrapper:hover {\n  background-color: #2a2d2e;\n}\n\n.folder-name-wrapper:hover .add-item {\n  display: flex;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb2RlLWVkaXRvci9jb2RlLXRlbXBsYXRlcy9DOlxcUHJvamVjdHNcXGVhdi1pdGVtLWRpYWxvZy1hbmd1bGFyL3Byb2plY3RzXFxuZy1kaWFsb2dzXFxzcmNcXGFwcFxcY29kZS1lZGl0b3JcXGNvZGUtdGVtcGxhdGVzXFxjb2RlLXRlbXBsYXRlcy5jb21wb25lbnQuc2NzcyIsInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb2RlLWVkaXRvci9jb2RlLXRlbXBsYXRlcy9jb2RlLXRlbXBsYXRlcy5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNFLGFBQUE7RUFDQSxzQkFBQTtFQUNBLDhCQUFBO0FDQ0Y7O0FERUE7RUFDRSxhQUFBO0VBQ0EseUJBQUE7QUNDRjs7QURFQTtFQUNFLGVBQUE7RUFDQSxzQkFBQTtFQUNBLGdCQUFBO0VBQ0EsbUJBQUE7RUFDQSx1QkFBQTtFQUNBLHlCQUFBO0tBQUEsc0JBQUE7TUFBQSxxQkFBQTtVQUFBLGlCQUFBO0FDQ0Y7O0FEQ0U7RUFDRSx5QkFBQTtBQ0NKOztBREVFO0VBQ0UseUJBQUE7QUNBSjs7QURJQTtFQUNFLGFBQUE7RUFDQSxlQUFBO0VBQ0EseUJBQUE7S0FBQSxzQkFBQTtNQUFBLHFCQUFBO1VBQUEsaUJBQUE7RUFDQSxtQkFBQTtBQ0RGOztBREdFO0VBQ0UsV0FBQTtFQUNBLFlBQUE7RUFDQSxlQUFBO0FDREo7O0FESUU7RUFDRSxhQUFBO0VBQ0EsbUJBQUE7RUFDQSxnQkFBQTtFQUNBLG1CQUFBO0VBQ0EsY0FBQTtFQUNBLGNBQUE7QUNGSjs7QURLRTtFQUNFLGlCQUFBO0VBQ0EsYUFBQTtFQUNBLG1CQUFBO0VBQ0EsdUJBQUE7RUFDQSxZQUFBO0FDSEo7O0FETUU7RUFDRSx5QkFBQTtBQ0pKOztBRE1JO0VBQ0UsYUFBQTtBQ0pOIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC9jb2RlLWVkaXRvci9jb2RlLXRlbXBsYXRlcy9jb2RlLXRlbXBsYXRlcy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5leHBsb3Jlci13cmFwcGVyIHtcclxuICBkaXNwbGF5OiBmbGV4O1xyXG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XHJcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xyXG59XHJcblxyXG4uY3JlYXRlLWJ1dHRvbi13cmFwcGVyIHtcclxuICBkaXNwbGF5OiBmbGV4O1xyXG4gIGp1c3RpZnktY29udGVudDogZmxleC1lbmQ7XHJcbn1cclxuXHJcbi5maWxlIHtcclxuICBjdXJzb3I6IHBvaW50ZXI7XHJcbiAgcGFkZGluZzogNHB4IDAgNHB4IDhweDtcclxuICBvdmVyZmxvdzogaGlkZGVuO1xyXG4gIHdoaXRlLXNwYWNlOiBub3dyYXA7XHJcbiAgdGV4dC1vdmVyZmxvdzogZWxsaXBzaXM7XHJcbiAgdXNlci1zZWxlY3Q6IG5vbmU7XHJcblxyXG4gICY6aG92ZXIge1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogcmdiKDQyLCA0NSwgNDYpO1xyXG4gIH1cclxuXHJcbiAgJi5hY3RpdmUge1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogcmdiKDU1LCA1NSwgNjEpO1xyXG4gIH1cclxufVxyXG5cclxuLmZvbGRlci1uYW1lLXdyYXBwZXIge1xyXG4gIGRpc3BsYXk6IGZsZXg7XHJcbiAgY3Vyc29yOiBwb2ludGVyO1xyXG4gIHVzZXItc2VsZWN0OiBub25lO1xyXG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XHJcblxyXG4gIC5tYXQtaWNvbiB7XHJcbiAgICB3aWR0aDogMThweDtcclxuICAgIGhlaWdodDogMThweDtcclxuICAgIGZvbnQtc2l6ZTogMThweDtcclxuICB9XHJcblxyXG4gIC5mb2xkZXItbmFtZSB7XHJcbiAgICBkaXNwbGF5OiBmbGV4O1xyXG4gICAgYWxpZ24taXRlbXM6IGNlbnRlcjtcclxuICAgIG92ZXJmbG93OiBoaWRkZW47XHJcbiAgICB3aGl0ZS1zcGFjZTogbm93cmFwO1xyXG4gICAgcGFkZGluZzogM3B4IDA7XHJcbiAgICBmbGV4OiAxIDEgYXV0bztcclxuICB9XHJcblxyXG4gIC5hZGQtaXRlbSB7XHJcbiAgICBtYXJnaW4tcmlnaHQ6IDRweDtcclxuICAgIGRpc3BsYXk6IG5vbmU7XHJcbiAgICBhbGlnbi1pdGVtczogY2VudGVyO1xyXG4gICAganVzdGlmeS1jb250ZW50OiBjZW50ZXI7XHJcbiAgICBwYWRkaW5nOiAzcHg7XHJcbiAgfVxyXG5cclxuICAmOmhvdmVyIHtcclxuICAgIGJhY2tncm91bmQtY29sb3I6IHJnYig0MiwgNDUsIDQ2KTtcclxuXHJcbiAgICAuYWRkLWl0ZW0ge1xyXG4gICAgICBkaXNwbGF5OiBmbGV4O1xyXG4gICAgfVxyXG4gIH1cclxufVxyXG4iLCIuZXhwbG9yZXItd3JhcHBlciB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XG4gIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2Vlbjtcbn1cblxuLmNyZWF0ZS1idXR0b24td3JhcHBlciB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGp1c3RpZnktY29udGVudDogZmxleC1lbmQ7XG59XG5cbi5maWxlIHtcbiAgY3Vyc29yOiBwb2ludGVyO1xuICBwYWRkaW5nOiA0cHggMCA0cHggOHB4O1xuICBvdmVyZmxvdzogaGlkZGVuO1xuICB3aGl0ZS1zcGFjZTogbm93cmFwO1xuICB0ZXh0LW92ZXJmbG93OiBlbGxpcHNpcztcbiAgdXNlci1zZWxlY3Q6IG5vbmU7XG59XG4uZmlsZTpob3ZlciB7XG4gIGJhY2tncm91bmQtY29sb3I6ICMyYTJkMmU7XG59XG4uZmlsZS5hY3RpdmUge1xuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzczNzNkO1xufVxuXG4uZm9sZGVyLW5hbWUtd3JhcHBlciB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGN1cnNvcjogcG9pbnRlcjtcbiAgdXNlci1zZWxlY3Q6IG5vbmU7XG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XG59XG4uZm9sZGVyLW5hbWUtd3JhcHBlciAubWF0LWljb24ge1xuICB3aWR0aDogMThweDtcbiAgaGVpZ2h0OiAxOHB4O1xuICBmb250LXNpemU6IDE4cHg7XG59XG4uZm9sZGVyLW5hbWUtd3JhcHBlciAuZm9sZGVyLW5hbWUge1xuICBkaXNwbGF5OiBmbGV4O1xuICBhbGlnbi1pdGVtczogY2VudGVyO1xuICBvdmVyZmxvdzogaGlkZGVuO1xuICB3aGl0ZS1zcGFjZTogbm93cmFwO1xuICBwYWRkaW5nOiAzcHggMDtcbiAgZmxleDogMSAxIGF1dG87XG59XG4uZm9sZGVyLW5hbWUtd3JhcHBlciAuYWRkLWl0ZW0ge1xuICBtYXJnaW4tcmlnaHQ6IDRweDtcbiAgZGlzcGxheTogbm9uZTtcbiAgYWxpZ24taXRlbXM6IGNlbnRlcjtcbiAganVzdGlmeS1jb250ZW50OiBjZW50ZXI7XG4gIHBhZGRpbmc6IDNweDtcbn1cbi5mb2xkZXItbmFtZS13cmFwcGVyOmhvdmVyIHtcbiAgYmFja2dyb3VuZC1jb2xvcjogIzJhMmQyZTtcbn1cbi5mb2xkZXItbmFtZS13cmFwcGVyOmhvdmVyIC5hZGQtaXRlbSB7XG4gIGRpc3BsYXk6IGZsZXg7XG59Il19 */");

/***/ }),

/***/ "./src/app/code-editor/code-templates/code-templates.component.ts":
/*!************************************************************************!*\
  !*** ./src/app/code-editor/code-templates/code-templates.component.ts ***!
  \************************************************************************/
/*! exports provided: CodeTemplatesComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CodeTemplatesComponent", function() { return CodeTemplatesComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _models_source_view_model__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../models/source-view.model */ "./src/app/code-editor/models/source-view.model.ts");
/* harmony import */ var _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../shared/services/dialog.service */ "./src/app/shared/services/dialog.service.ts");
/* harmony import */ var _code_templates_helpers__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./code-templates.helpers */ "./src/app/code-editor/code-templates/code-templates.helpers.ts");





var CodeTemplatesComponent = /** @class */ (function () {
    function CodeTemplatesComponent(dialogService) {
        this.dialogService = dialogService;
        this.createTemplate = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.toggledItems = [];
    }
    CodeTemplatesComponent.prototype.ngOnInit = function () {
    };
    CodeTemplatesComponent.prototype.ngOnChanges = function (changes) {
        var _a, _b;
        if ((_a = changes.templates) === null || _a === void 0 ? void 0 : _a.currentValue) {
            this.tree = Object(_code_templates_helpers__WEBPACK_IMPORTED_MODULE_4__["calculateTree"])(this.templates);
        }
        if ((_b = changes.view) === null || _b === void 0 ? void 0 : _b.currentValue) {
            this.showFileInTree(this.view.FileName);
        }
    };
    CodeTemplatesComponent.prototype.openTemplate = function (path) {
        this.dialogService.openCodeFile(path);
    };
    CodeTemplatesComponent.prototype.toggleItem = function (path) {
        Object(_code_templates_helpers__WEBPACK_IMPORTED_MODULE_4__["toggleInArray"])(path, this.toggledItems);
    };
    CodeTemplatesComponent.prototype.addFile = function (folder) {
        this.createTemplate.emit(folder);
    };
    CodeTemplatesComponent.prototype.showFileInTree = function (file) {
        var e_1, _a;
        if (file == null) {
            return;
        }
        if (this.toggledItems.includes(file)) {
            return;
        }
        var paths = file.split('/');
        var pathFromRoot = '';
        try {
            for (var paths_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(paths), paths_1_1 = paths_1.next(); !paths_1_1.done; paths_1_1 = paths_1.next()) {
                var path = paths_1_1.value;
                pathFromRoot = !pathFromRoot ? path : pathFromRoot + "/" + path;
                if (this.toggledItems.includes(pathFromRoot)) {
                    continue;
                }
                this.toggleItem(pathFromRoot);
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (paths_1_1 && !paths_1_1.done && (_a = paths_1.return)) _a.call(paths_1);
            }
            finally { if (e_1) throw e_1.error; }
        }
    };
    CodeTemplatesComponent.ctorParameters = function () { return [
        { type: _shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_3__["DialogService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", _models_source_view_model__WEBPACK_IMPORTED_MODULE_2__["SourceView"])
    ], CodeTemplatesComponent.prototype, "view", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Array)
    ], CodeTemplatesComponent.prototype, "templates", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"])
    ], CodeTemplatesComponent.prototype, "createTemplate", void 0);
    CodeTemplatesComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-code-templates',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./code-templates.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/code-editor/code-templates/code-templates.component.html")).default,
            changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectionStrategy"].OnPush,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./code-templates.component.scss */ "./src/app/code-editor/code-templates/code-templates.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_shared_services_dialog_service__WEBPACK_IMPORTED_MODULE_3__["DialogService"]])
    ], CodeTemplatesComponent);
    return CodeTemplatesComponent;
}());



/***/ }),

/***/ "./src/app/code-editor/code-templates/code-templates.helpers.ts":
/*!**********************************************************************!*\
  !*** ./src/app/code-editor/code-templates/code-templates.helpers.ts ***!
  \**********************************************************************/
/*! exports provided: calculateTree, toggleInArray */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "calculateTree", function() { return calculateTree; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "toggleInArray", function() { return toggleInArray; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

function calculateTree(templates) {
    var e_1, _a;
    if (!templates) {
        return [];
    }
    var tree = [];
    try {
        for (var templates_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(templates), templates_1_1 = templates_1.next(); !templates_1_1.done; templates_1_1 = templates_1.next()) {
            var template = templates_1_1.value;
            var parent_1 = tree;
            var paths = template.split('/');
            var last = paths[paths.length - 1];
            var pathFromRoot = '';
            var _loop_1 = function (i) {
                var path = paths[i];
                pathFromRoot += i ? "/" + path : path;
                var existing = parent_1.find(function (item) { return item.name === path; });
                if (existing) {
                    parent_1 = existing.children;
                }
                else {
                    var item = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({ depth: i, name: path, pathFromRoot: pathFromRoot, isFolder: path !== last }, (path !== last && { children: [] }));
                    parent_1.push(item);
                    parent_1 = item.children;
                }
            };
            for (var i = 0; i < paths.length; i++) {
                _loop_1(i);
            }
        }
    }
    catch (e_1_1) { e_1 = { error: e_1_1 }; }
    finally {
        try {
            if (templates_1_1 && !templates_1_1.done && (_a = templates_1.return)) _a.call(templates_1);
        }
        finally { if (e_1) throw e_1.error; }
    }
    return tree;
}
function toggleInArray(item, array) {
    var index = array.indexOf(item);
    if (index === -1) {
        array.push(item);
    }
    else {
        array.splice(index, 1);
    }
}


/***/ }),

/***/ "./src/app/code-editor/code-templates/depth-padding.pipe.ts":
/*!******************************************************************!*\
  !*** ./src/app/code-editor/code-templates/depth-padding.pipe.ts ***!
  \******************************************************************/
/*! exports provided: DepthPaddingPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DepthPaddingPipe", function() { return DepthPaddingPipe; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/platform-browser.js");



var DepthPaddingPipe = /** @class */ (function () {
    function DepthPaddingPipe(sanitizer) {
        this.sanitizer = sanitizer;
    }
    DepthPaddingPipe.prototype.transform = function (depth, isFolder) {
        var padding;
        if (isFolder) {
            padding = depth * 8;
        }
        else {
            padding = (depth === 0) ? 8 : (depth * 8 + 16);
        }
        return this.sanitizer.bypassSecurityTrustStyle("padding-left: " + padding + "px;");
    };
    DepthPaddingPipe.ctorParameters = function () { return [
        { type: _angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["DomSanitizer"] }
    ]; };
    DepthPaddingPipe = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Pipe"])({ name: 'depthPadding' }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_platform_browser__WEBPACK_IMPORTED_MODULE_2__["DomSanitizer"]])
    ], DepthPaddingPipe);
    return DepthPaddingPipe;
}());



/***/ }),

/***/ "./src/app/code-editor/code-templates/order-items.pipe.ts":
/*!****************************************************************!*\
  !*** ./src/app/code-editor/code-templates/order-items.pipe.ts ***!
  \****************************************************************/
/*! exports provided: SortItemsPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SortItemsPipe", function() { return SortItemsPipe; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


/** Sorts folders before files */
var SortItemsPipe = /** @class */ (function () {
    function SortItemsPipe() {
    }
    SortItemsPipe.prototype.transform = function (items) {
        if (items == null) {
            return items;
        }
        items.sort(function (item1, item2) {
            if (item1.isFolder < item2.isFolder) {
                return 1;
            }
            else if (item1.isFolder > item2.isFolder) {
                return -1;
            }
            else {
                return 0;
            }
        });
        return items;
    };
    SortItemsPipe = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Pipe"])({ name: 'sortItems' })
    ], SortItemsPipe);
    return SortItemsPipe;
}());



/***/ }),

/***/ "./src/app/code-editor/models/source-view.model.ts":
/*!*********************************************************!*\
  !*** ./src/app/code-editor/models/source-view.model.ts ***!
  \*********************************************************/
/*! exports provided: SourceView, SourceViewStreams */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SourceView", function() { return SourceView; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SourceViewStreams", function() { return SourceViewStreams; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var SourceView = /** @class */ (function () {
    function SourceView() {
    }
    return SourceView;
}());

var SourceViewStreams = /** @class */ (function () {
    function SourceViewStreams() {
    }
    return SourceViewStreams;
}());



/***/ }),

/***/ "./src/app/code-editor/services/snippets.service.ts":
/*!**********************************************************!*\
  !*** ./src/app/code-editor/services/snippets.service.ts ***!
  \**********************************************************/
/*! exports provided: SnippetsService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SnippetsService", function() { return SnippetsService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _ngx_translate_core__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @ngx-translate/core */ "../../node_modules/@ngx-translate/core/__ivy_ngcc__/fesm5/ngx-translate-core.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! lodash-es/cloneDeep */ "../../node_modules/lodash-es/cloneDeep.js");
/* harmony import */ var _content_type_fields_constants_data_type_constants__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ../../content-type-fields/constants/data-type.constants */ "./src/app/content-type-fields/constants/data-type.constants.ts");







var SnippetsService = /** @class */ (function () {
    function SnippetsService(http, dnnContext, translate) {
        this.http = http;
        this.dnnContext = dnnContext;
        this.translate = translate;
        this.keyPrefixes = ['@', '['];
        this.keyPrefixIndex = function (view) { return (view.Type.indexOf('Razor') > -1) ? 0 : 1; };
    }
    SnippetsService.prototype.getSnippets = function (view) {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function () {
            var res, snippets, relevant, standAndInputSnips, sets;
            return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__generator"])(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.http.get('../ng-assets/snippets.json.js').toPromise()];
                    case 1:
                        res = _a.sent();
                        snippets = res.snippets;
                        relevant = this.filterAwayNotNeededSnippetsList(snippets, view);
                        standAndInputSnips = this.extractInputTypeSnippets(relevant);
                        sets = this.initSnippetsWithConfig(standAndInputSnips.standardArray, view, standAndInputSnips.inputTypeSnippets);
                        return [2 /*return*/, { sets: sets, list: standAndInputSnips.standardArray }];
                }
            });
        });
    };
    // scan the list for sets starting with @ or [ and filter if not needed right now
    SnippetsService.prototype.filterAwayNotNeededSnippetsList = function (list, view) {
        var e_1, _a;
        var newList = [];
        try {
            for (var list_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(list), list_1_1 = list_1.next(); !list_1_1.done; list_1_1 = list_1.next()) {
                var itm = list_1_1.value;
                var setHasPrefix = this.keyPrefixes.indexOf(itm.set[0]);
                if (setHasPrefix === -1 || (setHasPrefix === this.keyPrefixIndex(view))) {
                    // if necessary, remove first char
                    if (setHasPrefix === this.keyPrefixIndex(view)) {
                        itm.set = itm.set.substr(1);
                    }
                    newList.push(itm);
                }
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (list_1_1 && !list_1_1.done && (_a = list_1.return)) _a.call(list_1);
            }
            finally { if (e_1) throw e_1.error; }
        }
        return newList;
    };
    SnippetsService.prototype.extractInputTypeSnippets = function (list) {
        var e_2, _a;
        var standardArray = [];
        var inputTypeArray = [];
        try {
            for (var list_2 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(list), list_2_1 = list_2.next(); !list_2_1.done; list_2_1 = list_2.next()) {
                var itm = list_2_1.value;
                var systemSnippet = itm.set[0] === '\\';
                if (!systemSnippet) {
                    standardArray.push(itm);
                }
                else {
                    itm.set = itm.set.substr(1);
                    inputTypeArray.push(itm);
                }
            }
        }
        catch (e_2_1) { e_2 = { error: e_2_1 }; }
        finally {
            try {
                if (list_2_1 && !list_2_1.done && (_a = list_2.return)) _a.call(list_2);
            }
            finally { if (e_2) throw e_2.error; }
        }
        var inputTypeSnippets = this.catalogInputTypeSnippets(inputTypeArray);
        return {
            standardArray: standardArray,
            inputTypeSnippets: inputTypeSnippets,
        };
    };
    SnippetsService.prototype.catalogInputTypeSnippets = function (list) {
        var e_3, _a;
        var inputTypeList = {};
        try {
            for (var list_3 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(list), list_3_1 = list_3.next(); !list_3_1.done; list_3_1 = list_3.next()) {
                var itm = list_3_1.value;
                if (inputTypeList[itm.subset] === undefined) {
                    inputTypeList[itm.subset] = [];
                }
                inputTypeList[itm.subset].push(itm);
            }
        }
        catch (e_3_1) { e_3 = { error: e_3_1 }; }
        finally {
            try {
                if (list_3_1 && !list_3_1.done && (_a = list_3.return)) _a.call(list_3);
            }
            finally { if (e_3) throw e_3.error; }
        }
        return inputTypeList;
    };
    SnippetsService.prototype.initSnippetsWithConfig = function (sets, templateConfiguration, inputTypeSnippets) {
        sets = this.makeTree(sets);
        // retrieve all relevant content-types and infos
        sets.Content = Object.assign({}, sets.Content, { Fields: {}, PresentationFields: {} });
        if (templateConfiguration.TypeContent) {
            this.loadContentType(sets.Content.Fields, templateConfiguration.TypeContent, 'Content', templateConfiguration, inputTypeSnippets);
        }
        if (templateConfiguration.TypeContentPresentation) {
            this.loadContentType(sets.Content.PresentationFields, templateConfiguration.TypeContentPresentation, 'Content.Presentation', templateConfiguration, inputTypeSnippets);
        }
        if (templateConfiguration.HasList) {
            sets.List = Object.assign({}, sets.List, { Fields: {}, PresentationFields: {} });
            if (templateConfiguration.TypeList) {
                this.loadContentType(sets.List.Fields, templateConfiguration.TypeList, 'Header', templateConfiguration, inputTypeSnippets);
            }
            if (templateConfiguration.TypeListPresentation) {
                this.loadContentType(sets.List.PresentationFields, templateConfiguration.TypeListPresentation, 'Header.Presentation', templateConfiguration, inputTypeSnippets);
            }
        }
        else {
            delete sets.List;
        }
        // maybe App-infos
        if (templateConfiguration.HasApp) {
            sets.App.Resources = {};
            sets.App.Settings = {};
            this.loadContentType(sets.App.Resources, 'App-Resources', 'App.Resources', templateConfiguration, inputTypeSnippets);
            this.loadContentType(sets.App.Settings, 'App-Settings', 'App.Settings', templateConfiguration, inputTypeSnippets);
        }
        return sets;
    };
    // Convert the list into a tree with set/subset/item
    SnippetsService.prototype.makeTree = function (list) {
        var e_4, _a;
        var tree = {};
        try {
            for (var list_4 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(list), list_4_1 = list_4.next(); !list_4_1.done; list_4_1 = list_4.next()) {
                var o = list_4_1.value;
                if (tree[o.set] === undefined) {
                    tree[o.set] = {};
                }
                if (tree[o.set][o.subset] === undefined) {
                    tree[o.set][o.subset] = [];
                }
                var reformatted = {
                    key: o.name,
                    label: this.label(o.set, o.subset, o.name),
                    snip: o.content,
                    help: o.help || this.help(o.set, o.subset, o.name),
                    links: this.linksList(o.links)
                };
                tree[o.set][o.subset].push(reformatted);
            }
        }
        catch (e_4_1) { e_4 = { error: e_4_1 }; }
        finally {
            try {
                if (list_4_1 && !list_4_1.done && (_a = list_4.return)) _a.call(list_4);
            }
            finally { if (e_4) throw e_4.error; }
        }
        return tree;
    };
    SnippetsService.prototype.label = function (set, subset, snip) {
        var key = this.getHelpKey(set, subset, snip, '.Key');
        var result = this.translate.instant(key);
        if (result === key) {
            result = snip;
        }
        return result;
    };
    SnippetsService.prototype.getHelpKey = function (set, subset, snip, addition) {
        return 'SourceEditorSnippets' + '.' + set + '.' + subset + '.' + snip + addition;
    };
    SnippetsService.prototype.help = function (set, subset, snip) {
        var key = this.getHelpKey(set, subset, snip, '.Help');
        var result = this.translate.instant(key);
        if (result === key) {
            result = '';
        }
        return result;
    };
    SnippetsService.prototype.linksList = function (linksString) {
        var e_5, _a;
        if (!linksString) {
            return null;
        }
        var links = [];
        var llist = linksString.split('\n');
        try {
            for (var llist_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(llist), llist_1_1 = llist_1.next(); !llist_1_1.done; llist_1_1 = llist_1.next()) {
                var l = llist_1_1.value;
                var pair = l.split(':');
                if (pair.length === 3) {
                    links.push({ name: pair[0].trim(), url: pair[1].trim() + ':' + pair[2].trim() });
                }
            }
        }
        catch (e_5_1) { e_5 = { error: e_5_1 }; }
        finally {
            try {
                if (llist_1_1 && !llist_1_1.done && (_a = llist_1.return)) _a.call(llist_1);
            }
            finally { if (e_5) throw e_5.error; }
        }
        if (links.length === 0) {
            return null;
        }
        return links;
    };
    // get fields in content types
    SnippetsService.prototype.loadContentType = function (target, type, prefix, templateConfiguration, inputTypeSnippets) {
        var _this = this;
        this.getFields(templateConfiguration.AppId, type)
            .then(function (fields) {
            var e_6, _a;
            try {
                // first add common items if the content-type actually exists
                for (var fields_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(fields), fields_1_1 = fields_1.next(); !fields_1_1.done; fields_1_1 = fields_1.next()) {
                    var value = fields_1_1.value;
                    var fieldname = value.StaticName;
                    target[fieldname] = {
                        key: fieldname,
                        label: fieldname,
                        snip: _this.valuePlaceholder(prefix, fieldname, templateConfiguration),
                        help: value.Metadata.merged.Notes || '' + ' (' + value.Type.toLowerCase() + ') '
                    };
                    // try to add generic snippets specific to this input-type
                    var snipDefaults = Object(lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_5__["default"])(target[fieldname]); // must be a copy, because target[fieldname] will grow
                    _this.attachSnippets(target, prefix, fieldname, value.InputType, snipDefaults, inputTypeSnippets);
                }
            }
            catch (e_6_1) { e_6 = { error: e_6_1 }; }
            finally {
                try {
                    if (fields_1_1 && !fields_1_1.done && (_a = fields_1.return)) _a.call(fields_1);
                }
                finally { if (e_6) throw e_6.error; }
            }
            var std = ['EntityId', 'EntityTitle', 'EntityGuid', 'EntityType', 'IsPublished', 'Modified'];
            if (fields.length) {
                // tslint:disable-next-line:prefer-for-of
                for (var i = 0; i < std.length; i++) {
                    target[std[i]] = {
                        key: std[i],
                        label: std[i],
                        snip: _this.valuePlaceholder(prefix, std[i], templateConfiguration),
                        help: _this.translate.instant('SourceEditorSnippets.StandardFields.' + std[i] + '.Help'),
                    };
                }
            }
        });
    };
    SnippetsService.prototype.valuePlaceholder = function (obj, val, templateConfiguration) {
        return (templateConfiguration.Type.indexOf('Razor') > -1)
            ? '@' + obj + '.' + val
            : '[' + obj.replace('.', ':') + ':' + val + ']';
    };
    SnippetsService.prototype.getFields = function (appId, staticName) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/contenttype/getfields'), {
            params: { appid: appId.toString(), staticName: staticName },
        })
            .toPromise()
            .then(function (fields) {
            var e_7, _a;
            fields = fields.filter(function (field) { return field.Type !== _content_type_fields_constants_data_type_constants__WEBPACK_IMPORTED_MODULE_6__["DataTypeConstants"].Empty; });
            if (fields) {
                try {
                    for (var fields_2 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(fields), fields_2_1 = fields_2.next(); !fields_2_1.done; fields_2_1 = fields_2.next()) {
                        var fld = fields_2_1.value;
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
                catch (e_7_1) { e_7 = { error: e_7_1 }; }
                finally {
                    try {
                        if (fields_2_1 && !fields_2_1.done && (_a = fields_2.return)) _a.call(fields_2);
                    }
                    finally { if (e_7) throw e_7.error; }
                }
            }
            return fields;
        });
    };
    SnippetsService.prototype.attachSnippets = function (target, prefix, fieldname, inputType, snipDefaults, inputTypeSnippets) {
        var genericSnippet = inputTypeSnippets[inputType];
        if (inputType.indexOf('-')) { // if it's a sub-type, let's also get the master-type
            var fieldType = inputType.substr(0, inputType.indexOf('-'));
            if (fieldType) {
                var typeSnips = inputTypeSnippets[fieldType];
                if (typeSnips) {
                    genericSnippet = genericSnippet ? genericSnippet.concat(typeSnips) : typeSnips;
                }
            }
        }
        if (!genericSnippet) {
            return;
        }
        if (target[fieldname].more === undefined) {
            target[fieldname].more = [];
        }
        var fieldSnips = target[fieldname].more;
        // tslint:disable-next-line:prefer-for-of
        for (var g = 0; g < genericSnippet.length; g++) {
            try {
                fieldSnips[fieldname + '-' + genericSnippet[g].name] = Object.assign({}, snipDefaults, {
                    key: fieldname + ' - ' + genericSnippet[g].name,
                    label: genericSnippet[g].name,
                    snip: this.localizeGenericSnippet(genericSnippet[g].content, prefix, fieldname),
                    collapse: true
                });
            }
            finally { }
        }
    };
    SnippetsService.prototype.localizeGenericSnippet = function (snip, objName, fieldName) {
        snip = snip.replace(/(\$\{[0-9]+\:)var(\})/gi, '$1' + objName + '$2')
            .replace(/(\$\{[0-9]+\:)prop(\})/gi, '$1' + fieldName + '$2');
        return snip;
    };
    SnippetsService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"] },
        { type: _ngx_translate_core__WEBPACK_IMPORTED_MODULE_3__["TranslateService"] }
    ]; };
    SnippetsService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"],
            _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"],
            _ngx_translate_core__WEBPACK_IMPORTED_MODULE_3__["TranslateService"]])
    ], SnippetsService);
    return SnippetsService;
}());



/***/ }),

/***/ "./src/app/code-editor/services/source.service.ts":
/*!********************************************************!*\
  !*** ./src/app/code-editor/services/source.service.ts ***!
  \********************************************************/
/*! exports provided: SourceService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SourceService", function() { return SourceService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");






var SourceService = /** @class */ (function () {
    function SourceService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    /** Key is templateId or path */
    SourceService.prototype.get = function (key) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/appassets/asset'), {
            params: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({ appId: this.context.appId.toString() }, this.templateIdOrPath(key))
        }).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_3__["map"])(function (view) {
            if (view.Type.toLowerCase() === 'auto') {
                switch (view.Extension.toLowerCase()) {
                    case '.cs':
                    case '.cshtml':
                        view.Type = 'Razor';
                        break;
                    case '.html':
                    case '.css':
                    case '.js':
                        view.Type = 'Token';
                        break;
                }
            }
            return view;
        }));
    };
    /** Key is templateId or path */
    SourceService.prototype.save = function (key, view) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/appassets/asset'), view, {
            params: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({ appId: this.context.appId.toString() }, this.templateIdOrPath(key)),
        });
    };
    SourceService.prototype.getTemplates = function () {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('app-sys/appassets/list'), {
            params: { appId: this.context.appId.toString(), global: 'false', withSubfolders: 'true' },
        });
    };
    SourceService.prototype.createTemplate = function (name) {
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('app-sys/appassets/create'), {}, {
            params: { appId: this.context.appId.toString(), global: 'false', path: name },
        });
    };
    SourceService.prototype.templateIdOrPath = function (key) {
        if (typeof key === typeof 0) {
            return { templateId: key.toString() };
        }
        else {
            return { path: key };
        }
    };
    SourceService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"] }
    ]; };
    SourceService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_5__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_4__["Context"]])
    ], SourceService);
    return SourceService;
}());



/***/ }),

/***/ "./src/app/shared/constants/file-names.constants.ts":
/*!**********************************************************!*\
  !*** ./src/app/shared/constants/file-names.constants.ts ***!
  \**********************************************************/
/*! exports provided: defaultControllerName, defaultTemplateName, defaultTokenName */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "defaultControllerName", function() { return defaultControllerName; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "defaultTemplateName", function() { return defaultTemplateName; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "defaultTokenName", function() { return defaultTokenName; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

var defaultControllerName = 'MyController.cs';
var defaultTemplateName = '_MyFile.cshtml';
var defaultTokenName = 'MyFile.html';


/***/ }),

/***/ "./src/app/shared/services/snack-bar-stack.service.ts":
/*!************************************************************!*\
  !*** ./src/app/shared/services/snack-bar-stack.service.ts ***!
  \************************************************************/
/*! exports provided: SnackBarStackService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SnackBarStackService", function() { return SnackBarStackService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");




/** This service ensures that multiple SnackBars are shown one after another. */
var SnackBarStackService = /** @class */ (function () {
    function SnackBarStackService(snackBar) {
        this.snackBar = snackBar;
        this.defaultDuration = 3000;
        this.processingMessage = false;
        this.messageQueue = [];
    }
    /**
     * Add a message to the stack queue.
     * @returns observable that fires if the action is triggered.
     * Service will complete the observable
     */
    SnackBarStackService.prototype.add = function (message, action, config) {
        if (config === void 0) { config = { duration: this.defaultDuration }; }
        var triggered = new rxjs__WEBPACK_IMPORTED_MODULE_3__["Subject"]();
        this.messageQueue.push({ message: message, action: action, config: config, triggered: triggered });
        if (!this.processingMessage) {
            this.showSnackBar();
        }
        return triggered.asObservable();
    };
    SnackBarStackService.prototype.showSnackBar = function () {
        var _this = this;
        var nextMsg = this.messageQueue.shift();
        if (nextMsg == null) {
            this.processingMessage = false;
            return;
        }
        this.processingMessage = true;
        var snackBarRef = this.snackBar.open(nextMsg.message, nextMsg.action, nextMsg.config);
        snackBarRef.afterDismissed().subscribe(function () {
            nextMsg.triggered.complete();
            _this.showSnackBar();
        });
        snackBarRef.onAction().subscribe(function () {
            nextMsg.triggered.next();
        });
    };
    SnackBarStackService.prototype.ngOnDestroy = function () {
        var e_1, _a;
        try {
            for (var _b = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(this.messageQueue), _c = _b.next(); !_c.done; _c = _b.next()) {
                var message = _c.value;
                message.triggered.complete();
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (_c && !_c.done && (_a = _b.return)) _a.call(_b);
            }
            finally { if (e_1) throw e_1.error; }
        }
        this.messageQueue = null;
    };
    SnackBarStackService.ctorParameters = function () { return [
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"] }
    ]; };
    SnackBarStackService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])({
            providedIn: 'root',
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"]])
    ], SnackBarStackService);
    return SnackBarStackService;
}());

var SnackBarData = /** @class */ (function () {
    function SnackBarData() {
    }
    return SnackBarData;
}());


/***/ })

}]);
//# sourceMappingURL=code-editor-code-editor-module.js.map