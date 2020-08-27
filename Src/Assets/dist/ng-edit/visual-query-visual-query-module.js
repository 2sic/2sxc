(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["visual-query-visual-query-module"],{

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/add-explorer/add-explorer.component.html":
/*!***********************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/add-explorer/add-explorer.component.html ***!
  \***********************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"editor-active-explorer fancy-scrollbar-dark\" *ngIf=\"dataSources\">\r\n\r\n  <mat-slide-toggle labelPosition=\"before\" (change)=\"onDifficultyChanged($event)\">\r\n    Show Advanced\r\n  </mat-slide-toggle>\r\n\r\n  <ng-container *ngFor=\"let item of sorted | keyvalue\">\r\n    <div class=\"collapsible\" [matTooltip]=\"item.key\" matTooltipShowDelay=\"750\" (click)=\"toggleItem(item.key)\">\r\n      <mat-icon>{{ toggledItems.includes(item.key) ? 'keyboard_arrow_down' : 'keyboard_arrow_right' }}</mat-icon>\r\n      <span>{{ item.key }}</span>\r\n    </div>\r\n\r\n    <div class=\"list\" *ngIf=\"toggledItems.includes(item.key)\">\r\n      <div class=\"list-item\" *ngFor=\"let value of item.value\" [matTooltip]=\"value.Name\" matTooltipShowDelay=\"750\"\r\n        (click)=\"addDataSource(value)\">\r\n        {{ value.Name }}\r\n      </div>\r\n    </div>\r\n  </ng-container>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/plumb-editor/plumb-editor.component.html":
/*!***********************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/plumb-editor/plumb-editor.component.html ***!
  \***********************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div id=\"pipelineContainer\" *ngIf=\"queryDef.data.DataSources\">\r\n  <div #dataSourceElement *ngFor=\"let dataSource of queryDef.data.DataSources; index as index\" datasource\r\n    [attr.guid]=\"dataSource.EntityGuid\" id=\"dataSource_{{ dataSource.EntityGuid }}\" class=\"dataSource\" [ngStyle]=\"{\r\n      'top': dataSource.VisualDesignerData.Top + 'px',\r\n      'left': dataSource.VisualDesignerData.Left + 'px',\r\n      'min-width': dataSource.VisualDesignerData.Width ? dataSource.VisualDesignerData.Width + 'px' : null\r\n    }\">\r\n\r\n    <div class=\"configure\" (click)=\"configureDataSource(dataSource)\" title=\"Configure DataSource\"\r\n      *ngIf=\"!dataSource.ReadOnly && typeInfo(dataSource).config\">\r\n      <mat-icon class=\"eav-icon-settings\">settings</mat-icon>\r\n    </div>\r\n\r\n    <mat-icon class=\"type-info\" [title]=\"typeInfo(dataSource).notes\">\r\n      {{ typeInfo(dataSource).icon }}\r\n    </mat-icon>\r\n\r\n    <div class=\"name noselect\" title=\"Click to edit Name\" (click)=\"editName(dataSource)\">\r\n      <span>{{ dataSource.Name || '(no name)' }}</span>\r\n      <mat-icon class=\"show-hover-inline eav-icon-pencil\">edit</mat-icon>\r\n    </div>\r\n    <br />\r\n\r\n    <div class=\"description noselect\" title=\"Click to edit Description\" (click)=\"editDescription(dataSource)\">\r\n      <span>{{ dataSource.Description }}</span>\r\n      <mat-icon class=\"show-hover-inline eav-icon-pencil\">edit</mat-icon>\r\n    </div>\r\n    <br />\r\n\r\n    <div class=\"typename\" [title]=\"dataSource.PartAssemblyAndType\">\r\n      Type: {{ typeNameFilter(dataSource.PartAssemblyAndType, 'className') }}\r\n    </div>\r\n\r\n    <div class=\"add-endpoint\" title=\"Drag a new Out-Connection\"\r\n      *ngIf=\"!dataSource.ReadOnly && typeInfo(dataSource).dynamicOut\">\r\n      <mat-icon class=\"new-connection eav-icon-up-dir\">arrow_drop_up</mat-icon>\r\n    </div>\r\n\r\n    <mat-icon class=\"delete eav-icon-cancel\" title=\"Delete\" (click)=\"remove(index)\" *ngIf=\"!dataSource.ReadOnly\">\r\n      delete\r\n    </mat-icon>\r\n\r\n    <a class=\"help eav-icon-help-circled\" title=\"Help for this DataSource\" [href]=\"typeInfo(dataSource).helpLink\"\r\n      target=\"_blank\" *ngIf=\"typeInfo(dataSource).helpLink\">\r\n      <mat-icon>help_outline</mat-icon>\r\n    </a>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/query-result/query-result.component.html":
/*!***********************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/query-result/query-result.component.html ***!
  \***********************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">\r\n    <div>Query Results</div>\r\n    <button mat-icon-button matTooltip=\"Close dialog\" (click)=\"closeDialog()\">\r\n      <mat-icon>close</mat-icon>\r\n    </button>\r\n  </div>\r\n</div>\r\n<p class=\"dialog-description\">\r\n  The Full result was logged to the Browser Console. Further down you'll find more debug-infos.\r\n</p>\r\n\r\n<br />\r\n\r\n<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Parameters & Statistics</div>\r\n</div>\r\n<p class=\"description\">\r\n  Executed in {{ timeUsed }}ms ({{ ticksUsed }} ticks)\r\n</p>\r\n<ul class=\"description\">\r\n  <li *ngFor=\"let param of testParameters?.split('\\n')\">{{ param }}</li>\r\n</ul>\r\n\r\n<br />\r\n\r\n<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Query Results</div>\r\n</div>\r\n<pre>\r\n  {{ result | json }}\r\n</pre>\r\n\r\n<br />\r\n\r\n<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Sources</div>\r\n</div>\r\n<table class=\"description\">\r\n  <tr>\r\n    <th>Guid</th>\r\n    <th>Type</th>\r\n    <th>Configuration</th>\r\n    <th>Error</th>\r\n  </tr>\r\n  <tr *ngFor=\"let source of sources | keyvalue\">\r\n    <td>\r\n      <pre>{{ source.value.Guid }}</pre>\r\n    </td>\r\n    <td>{{ source.value.Type }}</td>\r\n    <td>\r\n      <ol>\r\n        <li *ngFor=\"let config of source.value.Configuration | keyvalue\">\r\n          <b>{{ config.key }}</b>=<em>{{ config.value }}</em></li>\r\n      </ol>\r\n    </td>\r\n    <td>{{ source.value.Error }}</td>\r\n  </tr>\r\n</table>\r\n\r\n<br />\r\n\r\n<div mat-dialog-title>\r\n  <div class=\"dialog-title-box\">Streams</div>\r\n</div>\r\n<table class=\"description\">\r\n  <tr>\r\n    <th>Source</th>\r\n    <th>Target</th>\r\n    <th>Items</th>\r\n    <th>Error</th>\r\n  </tr>\r\n  <tr *ngFor=\"let stream of streams\">\r\n    <td>\r\n      <pre>{{ stream.Source + \":\" + stream.SourceOut }}</pre>\r\n    </td>\r\n    <td>\r\n      <pre>{{ stream.Target + \":\" + stream.TargetIn }}</pre>\r\n    </td>\r\n    <td>{{ stream.Count }}</td>\r\n    <td>{{ stream.Error }}</td>\r\n  </tr>\r\n</table>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/run-explorer/run-explorer.component.html":
/*!***********************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/run-explorer/run-explorer.component.html ***!
  \***********************************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"editor-active-explorer fancy-scrollbar-dark\" *ngIf=\"queryDef\">\r\n\r\n  <div class=\"wrapper\">\r\n    <div class=\"actions\">\r\n      <button mat-raised-button class=\"action\" color=\"accent\" (click)=\"saveAndRunQuery(true, true)\">\r\n        Save and run\r\n      </button>\r\n      <button mat-raised-button class=\"action\" (click)=\"saveAndRunQuery(false, true)\">\r\n        Run\r\n      </button>\r\n      <button mat-raised-button class=\"action\" (click)=\"saveAndRunQuery(true, false)\">\r\n        Save\r\n      </button>\r\n      <button mat-raised-button class=\"action\" (click)=\"doRepaint()\">\r\n        Repaint\r\n      </button>\r\n    </div>\r\n\r\n    <div class=\"parameters\">\r\n      <div class=\"title\">\r\n        <div>Parameters</div>\r\n        <div>\r\n          <button mat-icon-button matTooltip=\"Edit parameters\" (click)=\"editPipeline()\">\r\n            <mat-icon>edit</mat-icon>\r\n          </button>\r\n          <button mat-icon-button matTooltip=\"Query Params Docs\" (click)=\"openParamsHelp()\">\r\n            <mat-icon>info</mat-icon>\r\n          </button>\r\n        </div>\r\n      </div>\r\n      <ul class=\"values\" *ngIf=\"queryDef.data.Pipeline.Params?.length\">\r\n        <li *ngFor=\"let param of queryDef.data.Pipeline.Params?.split('\\n')\">\r\n          {{ param }}\r\n        </li>\r\n      </ul>\r\n    </div>\r\n\r\n    <div class=\"parameters\">\r\n      <div class=\"title\">Test Parameters</div>\r\n      <ul class=\"values\" *ngIf=\"queryDef.data.Pipeline.TestParameters?.length\">\r\n        <li *ngFor=\"let param of queryDef.data.Pipeline.TestParameters?.split('\\n')\">\r\n          {{ param }}\r\n        </li>\r\n      </ul>\r\n    </div>\r\n\r\n    <div class=\"warnings\" *ngIf=\"warnings.length\">\r\n      <div class=\"title\">\r\n        <span>Warnings</span>\r\n        <mat-icon style=\"color: red\">warning</mat-icon>\r\n      </div>\r\n      <ol class=\"values\">\r\n        <li *ngFor=\"let warning of warnings\">\r\n          {{ warning }}\r\n        </li>\r\n      </ol>\r\n    </div>\r\n\r\n    <div class=\"description\" *ngIf=\"queryDef.data.Pipeline.Description\">\r\n      <div class=\"title\">Query Description</div>\r\n      <div>{{ queryDef.data.Pipeline.Description }}</div>\r\n    </div>\r\n  </div>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/visual-query.component.html":
/*!**********************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/visual-query.component.html ***!
  \**********************************************************************************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("<div class=\"editor-root\">\r\n  <div class=\"editor-side-toolbar\">\r\n    <div class=\"button\" matTooltip=\"Run\" [ngClass]=\"{ 'active': activeExplorer === explorer.run }\"\r\n      (click)=\"toggleExplorer(explorer.run)\">\r\n      <mat-icon>play_arrow</mat-icon>\r\n    </div>\r\n    <div class=\"button\" matTooltip=\"Add\" [ngClass]=\"{ 'active': activeExplorer === explorer.add }\"\r\n      (click)=\"toggleExplorer(explorer.add)\">\r\n      <mat-icon>add</mat-icon>\r\n    </div>\r\n    <div class=\"spacer\"></div>\r\n    <div class=\"button\" matTooltip=\"Help\" (click)=\"openHelp()\">\r\n      <mat-icon>help_outline</mat-icon>\r\n    </div>\r\n    <router-outlet></router-outlet>\r\n  </div>\r\n\r\n  <app-run-explorer [hidden]=\"activeExplorer !== explorer.run\" *ngIf=\"queryDef\" [queryDef]=\"queryDef\"\r\n    (editPipelineEntity)=\"editPipelineEntity()\" (saveAndRun)=\"saveAndRun($event)\" (repaint)=\"repaint()\">\r\n  </app-run-explorer>\r\n\r\n  <app-add-explorer [hidden]=\"activeExplorer !== explorer.add\" *ngIf=\"queryDef\"\r\n    [dataSources]=\"queryDef.data.InstalledDataSources\" (addSelectedDataSource)=\"addSelectedDataSource($event)\">\r\n  </app-add-explorer>\r\n\r\n  <app-plumb-editor *ngIf=\"queryDef\" [queryDef]=\"queryDef\" (save)=\"savePipeline()\"\r\n    (editDataSourcePart)=\"editDataSourcePart($event)\" (instanceChanged)=\"instanceChanged($event)\">\r\n  </app-plumb-editor>\r\n</div>\r\n");

/***/ }),

/***/ "../../node_modules/script-loader/addScript.js":
/*!***********************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/script-loader/addScript.js ***!
  \***********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

/*
	MIT License http://www.opensource.org/licenses/mit-license.php
	Author Tobias Koppers @sokra
*/
module.exports = function(src) {
	function log(error) {
		(typeof console !== "undefined")
		&& (console.error || console.log)("[Script Loader]", error);
	}

	// Check for IE =< 8
	function isIE() {
		return typeof attachEvent !== "undefined" && typeof addEventListener === "undefined";
	}

	try {
		if (typeof execScript !== "undefined" && isIE()) {
			execScript(src);
		} else if (typeof eval !== "undefined") {
			eval.call(null, src);
		} else {
			log("EvalError: No eval function available");
		}
	} catch (error) {
		log(error);
	}
}


/***/ }),

/***/ "../../node_modules/script-loader/index.js!../../node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js":
/*!************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/script-loader!C:/Projects/eav-item-dialog-angular/node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js ***!
  \************************************************************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

__webpack_require__(/*! !C:/Projects/eav-item-dialog-angular/node_modules/script-loader/addScript.js */ "../../node_modules/script-loader/addScript.js")(__webpack_require__(/*! !C:/Projects/eav-item-dialog-angular/node_modules/script-loader/node_modules/raw-loader!C:/Projects/eav-item-dialog-angular/node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js */ "../../node_modules/script-loader/node_modules/raw-loader/index.js!../../node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js"))

/***/ }),

/***/ "../../node_modules/script-loader/node_modules/raw-loader/index.js!../../node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js":
/*!************************************************************************************************************************************************************************************!*\
  !*** C:/Projects/eav-item-dialog-angular/node_modules/script-loader/node_modules/raw-loader!C:/Projects/eav-item-dialog-angular/node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js ***!
  \************************************************************************************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "(function(){\"undefined\"==typeof Math.sgn&&(Math.sgn=function(a){return 0==a?0:a>0?1:-1});var a={subtract:function(a,b){return{x:a.x-b.x,y:a.y-b.y}},dotProduct:function(a,b){return a.x*b.x+a.y*b.y},square:function(a){return Math.sqrt(a.x*a.x+a.y*a.y)},scale:function(a,b){return{x:a.x*b,y:a.y*b}}},b=64,c=Math.pow(2,-b-1),d=function(b,c){for(var d=[],e=f(b,c),h=c.length-1,i=2*h-1,j=g(e,i,d,0),k=a.subtract(b,c[0]),m=a.square(k),n=0,o=0;j>o;o++){k=a.subtract(b,l(c,h,d[o],null,null));var p=a.square(k);m>p&&(m=p,n=d[o])}return k=a.subtract(b,c[h]),p=a.square(k),m>p&&(m=p,n=1),{location:n,distance:m}},e=function(a,b){var c=d(a,b);return{point:l(b,b.length-1,c.location,null,null),location:c.location}},f=function(b,c){for(var d=c.length-1,e=2*d-1,f=[],g=[],h=[],i=[],k=[[1,.6,.3,.1],[.4,.6,.6,.4],[.1,.3,.6,1]],l=0;d>=l;l++)f[l]=a.subtract(c[l],b);for(var l=0;d-1>=l;l++)g[l]=a.subtract(c[l+1],c[l]),g[l]=a.scale(g[l],3);for(var m=0;d-1>=m;m++)for(var n=0;d>=n;n++)h[m]||(h[m]=[]),h[m][n]=a.dotProduct(g[m],f[n]);for(l=0;e>=l;l++)i[l]||(i[l]=[]),i[l].y=0,i[l].x=parseFloat(l)/e;for(var o=d,p=d-1,q=0;o+p>=q;q++){var r=Math.max(0,q-p),s=Math.min(q,o);for(l=r;s>=l;l++)j=q-l,i[l+j].y+=h[j][l]*k[j][l]}return i},g=function(a,c,d,e){var f,j,m=[],n=[],o=[],p=[];switch(h(a,c)){case 0:return 0;case 1:if(e>=b)return d[0]=(a[0].x+a[c].x)/2,1;if(i(a,c))return d[0]=k(a,c),1}l(a,c,.5,m,n),f=g(m,c,o,e+1),j=g(n,c,p,e+1);for(var q=0;f>q;q++)d[q]=o[q];for(var q=0;j>q;q++)d[q+f]=p[q];return f+j},h=function(a,b){var c,d,e=0;c=d=Math.sgn(a[0].y);for(var f=1;b>=f;f++)c=Math.sgn(a[f].y),c!=d&&e++,d=c;return e},i=function(a,b){var d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s;i=a[0].y-a[b].y,j=a[b].x-a[0].x,k=a[0].x*a[b].y-a[b].x*a[0].y;for(var t=max_distance_below=0,u=1;b>u;u++){var v=i*a[u].x+j*a[u].y+k;v>t?t=v:max_distance_below>v&&(max_distance_below=v)}return n=0,o=1,p=0,q=i,r=j,s=k-t,l=n*r-q*o,m=1/l,e=(o*s-r*p)*m,q=i,r=j,s=k-max_distance_below,l=n*r-q*o,m=1/l,f=(o*s-r*p)*m,g=Math.min(e,f),h=Math.max(e,f),d=h-g,c>d?1:0},k=function(a,b){var c=1,d=0,e=a[b].x-a[0].x,f=a[b].y-a[0].y,g=a[0].x-0,h=a[0].y-0,i=e*d-f*c,j=1/i,k=(e*h-f*g)*j;return 0+c*k},l=function(a,b,c,d,e){for(var f=[[]],g=0;b>=g;g++)f[0][g]=a[g];for(var h=1;b>=h;h++)for(var g=0;b-h>=g;g++)f[h]||(f[h]=[]),f[h][g]||(f[h][g]={}),f[h][g].x=(1-c)*f[h-1][g].x+c*f[h-1][g+1].x,f[h][g].y=(1-c)*f[h-1][g].y+c*f[h-1][g+1].y;if(null!=d)for(g=0;b>=g;g++)d[g]=f[g][0];if(null!=e)for(g=0;b>=g;g++)e[g]=f[b-g][g];return f[b][0]},m={},n=function(a){var b=m[a];if(!b){b=[];var c=function(){return function(b){return Math.pow(b,a)}},d=function(){return function(b){return Math.pow(1-b,a)}},e=function(a){return function(){return a}},f=function(){return function(a){return a}},g=function(){return function(a){return 1-a}},h=function(a){return function(b){for(var c=1,d=0;d<a.length;d++)c*=a[d](b);return c}};b.push(new c);for(var i=1;a>i;i++){for(var j=[new e(a)],k=0;a-i>k;k++)j.push(new f);for(var k=0;i>k;k++)j.push(new g);b.push(new h(j))}b.push(new d),m[a]=b}return b},o=function(a,b){for(var c=n(a.length-1),d=0,e=0,f=0;f<a.length;f++)d+=a[f].x*c[f](b),e+=a[f].y*c[f](b);return{x:d,y:e}},p=function(a,b){return Math.sqrt(Math.pow(a.x-b.x,2)+Math.pow(a.y-b.y,2))},q=function(a){return a[0].x==a[1].x&&a[0].y==a[1].y},r=function(a,b,c){if(q(a))return{point:a[0],location:b};for(var d=o(a,b),e=0,f=b,g=c>0?1:-1,h=null;e<Math.abs(c);)f+=.005*g,h=o(a,f),e+=p(h,d),d=h;return{point:h,location:f}},s=function(a){if(q(a))return 0;for(var b=o(a,0),c=0,d=0,e=1,f=null;1>d;)d+=.005*e,f=o(a,d),c+=p(f,b),b=f;return c},t=function(a,b,c){return r(a,b,c).point},u=function(a,b,c){return r(a,b,c).location},v=function(a,b){var c=o(a,b),d=o(a.slice(0,a.length-1),b),e=d.y-c.y,f=d.x-c.x;return 0==e?1/0:Math.atan(e/f)},w=function(a,b,c){var d=r(a,b,c);return d.location>1&&(d.location=1),d.location<0&&(d.location=0),v(a,d.location)},x=function(a,b,c,d){d=null==d?0:d;var e=r(a,b,d),f=v(a,e.location),g=Math.atan(-1/f),h=c/2*Math.sin(g),i=c/2*Math.cos(g);return[{x:e.point.x+i,y:e.point.y+h},{x:e.point.x-i,y:e.point.y-h}]};this.jsBezier={distanceFromCurve:d,gradientAtPoint:v,gradientAtPointAlongCurveFrom:w,nearestPointOnCurve:e,pointOnCurve:o,pointAlongCurveFrom:t,perpendicularToCurveAt:x,locationAlongCurveFrom:u,getLength:s}}).call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.Biltong={},c=function(a){return\"[object Array]\"===Object.prototype.toString.call(a)},d=function(a,b,d){return a=c(a)?a:[a.x,a.y],b=c(b)?b:[b.x,b.y],d(a,b)},e=b.gradient=function(a,b){return d(a,b,function(a,b){return b[0]==a[0]?b[1]>a[1]?1/0:-1/0:b[1]==a[1]?b[0]>a[0]?0:-0:(b[1]-a[1])/(b[0]-a[0])})},f=(b.normal=function(a,b){return-1/e(a,b)},b.lineLength=function(a,b){return d(a,b,function(a,b){return Math.sqrt(Math.pow(b[1]-a[1],2)+Math.pow(b[0]-a[0],2))})},b.quadrant=function(a,b){return d(a,b,function(a,b){return b[0]>a[0]?b[1]>a[1]?2:1:b[0]==a[0]?b[1]>a[1]?2:1:b[1]>a[1]?3:4})}),g=(b.theta=function(a,b){return d(a,b,function(a,b){var c=e(a,b),d=Math.atan(c),g=f(a,b);return(4==g||3==g)&&(d+=Math.PI),0>d&&(d+=2*Math.PI),d})},b.intersects=function(a,b){var c=a.x,d=a.x+a.w,e=a.y,f=a.y+a.h,g=b.x,h=b.x+b.w,i=b.y,j=b.y+b.h;return g>=c&&d>=g&&i>=e&&f>=i||h>=c&&d>=h&&i>=e&&f>=i||g>=c&&d>=g&&j>=e&&f>=j||h>=c&&d>=g&&j>=e&&f>=j||c>=g&&h>=c&&e>=i&&j>=e||d>=g&&h>=d&&e>=i&&j>=e||c>=g&&h>=c&&f>=i&&j>=f||d>=g&&h>=c&&f>=i&&j>=f},b.encloses=function(a,b,c){var d=a.x,e=a.x+a.w,f=a.y,g=a.y+a.h,h=b.x,i=b.x+b.w,j=b.y,k=b.y+b.h,l=function(a,b,d,e){return c?b>=a&&d>=e:b>a&&d>e};return l(d,h,e,i)&&l(f,j,g,k)},[null,[1,-1],[1,1],[-1,1],[-1,-1]]),h=[null,[-1,-1],[-1,1],[1,1],[1,-1]];b.pointOnLine=function(a,b,c){var d=e(a,b),i=f(a,b),j=c>0?g[i]:h[i],k=Math.atan(d),l=Math.abs(c*Math.sin(k))*j[1],m=Math.abs(c*Math.cos(k))*j[0];return{x:a.x+m,y:a.y+l}},b.perpendicularLineTo=function(a,b,c){var d=e(a,b),f=Math.atan(-1/d),g=c/2*Math.sin(f),h=c/2*Math.cos(f);return[{x:b.x+h,y:b.y+g},{x:b.x-h,y:b.y-g}]}}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b={android:navigator.userAgent.toLowerCase().indexOf(\"android\")>-1},c=function(a,b,c){c=c||a.parentNode;for(var d=c.querySelectorAll(b),e=0;e<d.length;e++)if(d[e]===a)return!0;return!1},d=function(a){return\"string\"==typeof a||a.constructor===String?document.getElementById(a):a},e=function(a){return a.srcElement||a.target},f=function(a,b,c,d){if(d){if(\"undefined\"!=typeof a.path)return{path:a.path,end:a.path.indexOf(c)};var e={path:[],end:-1},f=function(a){e.path.push(a),a===c?e.end=e.path.length-1:null!=a.parentNode&&f(a.parentNode)};return f(b),e}return{path:[b],end:1}},g=function(a,b){for(var c=0,d=a.length;d>c&&a[c]!=b;c++);c<a.length&&a.splice(c,1)},h=1,i=function(a,b,c){var d=h++;return a.__ta=a.__ta||{},a.__ta[b]=a.__ta[b]||{},a.__ta[b][d]=c,c.__tauid=d,d},j=function(a,b,c){if(a.__ta&&a.__ta[b]&&delete a.__ta[b][c.__tauid],c.__taExtra){for(var d=0;d<c.__taExtra.length;d++)F(a,c.__taExtra[d][0],c.__taExtra[d][1]);c.__taExtra.length=0}c.__taUnstore&&c.__taUnstore()},k=function(a,b,d,g){if(null==a)return d;var h=a.split(\",\"),i=function(g){i.__tauid=d.__tauid;var j=e(g),k=j,l=f(g,j,b,null!=a);if(-1!=l.end)for(var m=0;m<l.end;m++){k=l.path[m];for(var n=0;n<h.length;n++)c(k,h[n],b)&&d.apply(k,arguments)}};return l(d,g,i),i},l=function(a,b,c){a.__taExtra=a.__taExtra||[],a.__taExtra.push([b,c])},m=function(a,b,c,d){if(s&&u[b]){var e=k(d,a,c,u[b]);E(a,u[b],e,c)}\"focus\"===b&&null==a.getAttribute(\"tabindex\")&&a.setAttribute(\"tabindex\",\"1\"),E(a,b,k(d,a,c,b),c)},n=function(a,b,c,d){if(null==a.__taSmartClicks){var f=function(b){a.__tad=y(b)},h=function(b){a.__tau=y(b)},i=function(b){if(a.__tad&&a.__tau&&a.__tad[0]===a.__tau[0]&&a.__tad[1]===a.__tau[1])for(var c=0;c<a.__taSmartClicks.length;c++)a.__taSmartClicks[c].apply(e(b),[b])};m(a,\"mousedown\",f,d),m(a,\"mouseup\",h,d),m(a,\"click\",i,d),a.__taSmartClicks=[]}a.__taSmartClicks.push(c),c.__taUnstore=function(){g(a.__taSmartClicks,c)}},o={tap:{touches:1,taps:1},dbltap:{touches:1,taps:2},contextmenu:{touches:2,taps:1}},p=function(a,b){return function(d,h,i,j){if(\"contextmenu\"==h&&t)m(d,h,i,j);else{if(null==d.__taTapHandler){var k=d.__taTapHandler={tap:[],dbltap:[],contextmenu:[],down:!1,taps:0,downSelectors:[]},l=function(g){for(var h=e(g),i=f(g,h,d,null!=j),l=!1,m=0;m<i.end;m++){if(l)return;h=i.path[m];for(var n=0;n<k.downSelectors.length;n++)if(null==k.downSelectors[n]||c(h,k.downSelectors[n],d)){k.down=!0,setTimeout(p,a),setTimeout(q,b),l=!0;break}}},n=function(a){if(k.down){var b,g,h=e(a);k.taps++;var i=D(a);for(var j in o)if(o.hasOwnProperty(j)){var l=o[j];if(l.touches===i&&(1===l.taps||l.taps===k.taps))for(var m=0;m<k[j].length;m++){g=f(a,h,d,null!=k[j][m][1]);for(var n=0;n<g.end;n++)if(b=g.path[n],null==k[j][m][1]||c(b,k[j][m][1],d)){k[j][m][0].apply(b,[a]);break}}}}},p=function(){k.down=!1},q=function(){k.taps=0};m(d,\"mousedown\",l),m(d,\"mouseup\",n)}d.__taTapHandler.downSelectors.push(j),d.__taTapHandler[h].push([i,j]),i.__taUnstore=function(){g(d.__taTapHandler[h],i)}}}},q=function(a,b,c,d){for(var e in c.__tamee[a])c.__tamee[a].hasOwnProperty(e)&&c.__tamee[a][e].apply(d,[b])},r=function(){var a=[];return function(b,d,f,g){if(!b.__tamee){b.__tamee={over:!1,mouseenter:[],mouseexit:[]};var h=function(d){var f=e(d);(null==g&&f==b&&!b.__tamee.over||c(f,g,b)&&(null==f.__tamee||!f.__tamee.over))&&(q(\"mouseenter\",d,b,f),f.__tamee=f.__tamee||{},f.__tamee.over=!0,a.push(f))},j=function(d){for(var f=e(d),g=0;g<a.length;g++)f!=a[g]||c(d.relatedTarget||d.toElement,\"*\",f)||(f.__tamee.over=!1,a.splice(g,1),q(\"mouseexit\",d,b,f))};E(b,\"mouseover\",k(g,b,h,\"mouseover\"),h),E(b,\"mouseout\",k(g,b,j,\"mouseout\"),j)}f.__taUnstore=function(){delete b.__tamee[d][f.__tauid]},i(b,d,f),b.__tamee[d][f.__tauid]=f}},s=\"ontouchstart\"in document.documentElement,t=\"onmousedown\"in document.documentElement,u={mousedown:\"touchstart\",mouseup:\"touchend\",mousemove:\"touchmove\"},v=function(){var a=-1;if(\"Microsoft Internet Explorer\"==navigator.appName){var b=navigator.userAgent,c=new RegExp(\"MSIE ([0-9]{1,}[.0-9]{0,})\");null!=c.exec(b)&&(a=parseFloat(RegExp.$1))}return a}(),w=v>-1&&9>v,x=function(a,b){if(null==a)return[0,0];var c=C(a),d=B(c,0);return[d[b+\"X\"],d[b+\"Y\"]]},y=function(a){return null==a?[0,0]:w?[a.clientX+document.documentElement.scrollLeft,a.clientY+document.documentElement.scrollTop]:x(a,\"page\")},z=function(a){return x(a,\"screen\")},A=function(a){return x(a,\"client\")},B=function(a,b){return a.item?a.item(b):a[b]},C=function(a){return a.touches&&a.touches.length>0?a.touches:a.changedTouches&&a.changedTouches.length>0?a.changedTouches:a.targetTouches&&a.targetTouches.length>0?a.targetTouches:[a]},D=function(a){return C(a).length},E=function(a,b,c,d){if(i(a,b,c),d.__tauid=c.__tauid,a.addEventListener)a.addEventListener(b,c,!1);else if(a.attachEvent){var e=b+c.__tauid;a[\"e\"+e]=c,a[e]=function(){a[\"e\"+e]&&a[\"e\"+e](window.event)},a.attachEvent(\"on\"+b,a[e])}},F=function(a,b,c){null!=c&&G(a,function(){var e=d(this);if(j(e,b,c),null!=c.__tauid)if(e.removeEventListener)e.removeEventListener(b,c,!1),s&&u[b]&&e.removeEventListener(u[b],c,!1);else if(this.detachEvent){var f=b+c.__tauid;e[f]&&e.detachEvent(\"on\"+b,e[f]),e[f]=null,e[\"e\"+f]=null}c.__taTouchProxy&&F(a,c.__taTouchProxy[1],c.__taTouchProxy[0])})},G=function(a,b){if(null!=a){a=\"undefined\"!=typeof Window&&\"unknown\"!=typeof a.top&&a==a.top?[a]:\"string\"!=typeof a&&null==a.tagName&&null!=a.length?a:\"string\"==typeof a?document.querySelectorAll(a):[a];for(var c=0;c<a.length;c++)b.apply(a[c])}};a.Mottle=function(a){a=a||{};var c=a.clickThreshold||250,e=a.dblClickThreshold||450,f=new r,g=new p(c,e),h=a.smartClicks,i=function(a,b,c,e){null!=c&&G(a,function(){var a=d(this);h&&\"click\"===b?n(a,b,c,e):\"tap\"===b||\"dbltap\"===b||\"contextmenu\"===b?g(a,b,c,e):\"mouseenter\"===b||\"mouseexit\"==b?f(a,b,c,e):m(a,b,c,e)})};this.remove=function(a){return G(a,function(){var a=d(this);if(a.__ta)for(var b in a.__ta)if(a.__ta.hasOwnProperty(b))for(var c in a.__ta[b])a.__ta[b].hasOwnProperty(c)&&F(a,b,a.__ta[b][c]);a.parentNode&&a.parentNode.removeChild(a)}),this},this.on=function(){var a=arguments[0],b=4==arguments.length?arguments[2]:null,c=arguments[1],d=arguments[arguments.length-1];return i(a,c,d,b),this},this.off=function(a,b,c){return F(a,b,c),this},this.trigger=function(a,c,e,f){var g=t&&(\"undefined\"==typeof MouseEvent||null==e||e.constructor===MouseEvent),h=s&&!t&&u[c]?u[c]:c,i=!(s&&!t&&u[c]),j=y(e),k=z(e),l=A(e);return G(a,function(){var a,m=d(this);e=e||{screenX:k[0],screenY:k[1],clientX:l[0],clientY:l[1]};var n=function(a){f&&(a.payload=f)},o={TouchEvent:function(a){var b=document.createTouch(window,m,0,j[0],j[1],k[0],k[1],l[0],l[1],0,0,0,0),c=document.createTouchList(b),d=document.createTouchList(b),e=document.createTouchList(b);a.initTouchEvent(h,!0,!0,window,null,k[0],k[1],l[0],l[1],!1,!1,!1,!1,c,d,e,1,0)},MouseEvents:function(a){if(a.initMouseEvent(h,!0,!0,window,0,k[0],k[1],l[0],l[1],!1,!1,!1,!1,1,m),b.android){var c=document.createTouch(window,m,0,j[0],j[1],k[0],k[1],l[0],l[1],0,0,0,0);a.touches=a.targetTouches=a.changedTouches=document.createTouchList(c)}}};if(document.createEvent){var p=!i&&!g&&s&&u[c]&&!b.android,q=p?\"TouchEvent\":\"MouseEvents\";a=document.createEvent(q),o[q](a),n(a),m.dispatchEvent(a)}else document.createEventObject&&(a=document.createEventObject(),a.eventType=a.eventName=h,a.screenX=k[0],a.screenY=k[1],a.clientX=l[0],a.clientY=l[1],n(a),m.fireEvent(\"on\"+h,a))}),this}},a.Mottle.consume=function(a,b){a.stopPropagation?a.stopPropagation():a.returnValue=!1,!b&&a.preventDefault&&a.preventDefault()},a.Mottle.pageLocation=y,a.Mottle.setForceTouchEvents=function(a){s=a},a.Mottle.setForceMouseEvents=function(a){t=a}}.call(\"undefined\"==typeof window?this:window),function(){\"use strict\";var a=this,b=function(a,b,c){return-1===a.indexOf(b)?(c?a.unshift(b):a.push(b),!0):!1},c=function(a,b){var c=a.indexOf(b);-1!=c&&a.splice(c,1)},d=function(a,b){for(var c=[],d=0;d<a.length;d++)-1==b.indexOf(a[d])&&c.push(a[d]);return c},e=function(a){return null==a?!1:\"string\"==typeof a||a.constructor==String},f=function(a){var b=a.getBoundingClientRect(),c=document.body,d=document.documentElement,e=window.pageYOffset||d.scrollTop||c.scrollTop,f=window.pageXOffset||d.scrollLeft||c.scrollLeft,g=d.clientTop||c.clientTop||0,h=d.clientLeft||c.clientLeft||0,i=b.top+e-g,j=b.left+f-h;return{top:Math.round(i),left:Math.round(j)}},g=function(a,b,c){c=c||a.parentNode;for(var d=c.querySelectorAll(b),e=0;e<d.length;e++)if(d[e]===a)return!0;return!1},h=function(){var a=-1;if(\"Microsoft Internet Explorer\"==navigator.appName){var b=navigator.userAgent,c=new RegExp(\"MSIE ([0-9]{1,}[.0-9]{0,})\");null!=c.exec(b)&&(a=parseFloat(RegExp.$1))}return a}(),i=50,j=50,k=h>-1&&9>h,l=9==h,m=function(a){if(k)return[a.clientX+document.documentElement.scrollLeft,a.clientY+document.documentElement.scrollTop];var b=o(a),c=n(b,0);return l?[c.pageX||c.clientX,c.pageY||c.clientY]:[c.pageX,c.pageY]},n=function(a,b){return a.item?a.item(b):a[b]},o=function(a){return a.touches&&a.touches.length>0?a.touches:a.changedTouches&&a.changedTouches.length>0?a.changedTouches:a.targetTouches&&a.targetTouches.length>0?a.targetTouches:[a]},p={draggable:\"katavorio-draggable\",droppable:\"katavorio-droppable\",drag:\"katavorio-drag\",selected:\"katavorio-drag-selected\",active:\"katavorio-drag-active\",hover:\"katavorio-drag-hover\",noSelect:\"katavorio-drag-no-select\",ghostProxy:\"katavorio-ghost-proxy\"},q=\"katavorio-drag-scope\",r=[\"stop\",\"start\",\"drag\",\"drop\",\"over\",\"out\",\"beforeStart\"],s=function(){},t=function(){return!0},u=function(a,b,c){for(var d=0;d<a.length;d++)a[d]!=c&&b(a[d])},v=function(a,b,c,d){u(a,function(a){a.setActive(b),b&&a.updatePosition(),c&&a.setHover(d,b)})},w=function(a,b){if(null!=a){a=e(a)||null!=a.tagName||null==a.length?[a]:a;for(var c=0;c<a.length;c++)b.apply(a[c],[a[c]])}},x=function(a){a.stopPropagation?(a.stopPropagation(),a.preventDefault()):a.returnValue=!1},y=\"input,textarea,select,button,option\",z=function(a,b,c){var d=a.srcElement||a.target;return!g(d,c.getInputFilterSelector(),b)},A=function(a,b,c,d){this.params=b||{},this.el=a,this.params.addClass(this.el,this._class),this.uuid=F();var e=!0;return this.setEnabled=function(a){e=a},this.isEnabled=function(){return e},this.toggleEnabled=function(){e=!e},this.setScope=function(a){this.scopes=a?a.split(/\\s+/):[d]},this.addScope=function(a){var b={};w(this.scopes,function(a){b[a]=!0}),w(a?a.split(/\\s+/):[],function(a){b[a]=!0}),this.scopes=[];for(var c in b)this.scopes.push(c)},this.removeScope=function(a){var b={};w(this.scopes,function(a){b[a]=!0}),w(a?a.split(/\\s+/):[],function(a){delete b[a]}),this.scopes=[];for(var c in b)this.scopes.push(c)},this.toggleScope=function(a){var b={};w(this.scopes,function(a){b[a]=!0}),w(a?a.split(/\\s+/):[],function(a){b[a]?delete b[a]:b[a]=!0}),this.scopes=[];for(var c in b)this.scopes.push(c)},this.setScope(b.scope),this.k=b.katavorio,b.katavorio},B=function(){return!0},C=function(){return!1},D=function(a,b,c){this._class=c.draggable;var d=A.apply(this,arguments);this.rightButtonCanDrag=this.params.rightButtonCanDrag;var h=[0,0],k=null,l=null,n=[0,0],o=!1,q=this.params.consumeStartEvent!==!1,r=this.el,s=this.params.clone,u=(this.params.scroll,b.multipleDrop!==!1),w=!1,y=b.ghostProxy===!0?B:b.ghostProxy&&\"function\"==typeof b.ghostProxy?b.ghostProxy:C,D=function(a){return a.cloneNode(!0)},E=b.snapThreshold||5,G=function(a,b,c,d,e){d=d||E,e=e||E;var f=Math.floor(a[0]/b),g=b*f,h=g+b,i=Math.abs(a[0]-g)<=d?g:Math.abs(h-a[0])<=d?h:a[0],j=Math.floor(a[1]/c),k=c*j,l=k+c,m=Math.abs(a[1]-k)<=e?k:Math.abs(l-a[1])<=e?l:a[1];return[i,m]};this.posses=[],this.posseRoles={},this.toGrid=function(a){return null==this.params.grid?a:G(a,this.params.grid[0],this.params.grid[1])},this.snap=function(a,b){if(null!=r){a=a||(this.params.grid?this.params.grid[0]:i),b=b||(this.params.grid?this.params.grid[1]:j);var c=this.params.getPosition(r);this.params.setPosition(r,G(c,a,b,a,b))}},this.setUseGhostProxy=function(a){y=a?B:C};var H,I=function(a){return b.allowNegative===!1?[Math.max(0,a[0]),Math.max(0,a[1])]:a},J=function(a){H=\"function\"==typeof a?a:a?function(a){return I([Math.max(0,Math.min(P.w-this.size[0],a[0])),Math.max(0,Math.min(P.h-this.size[1],a[1]))])}.bind(this):function(a){return I(a)}}.bind(this);J(\"function\"==typeof this.params.constrain?this.params.constrain:this.params.constrain||this.params.containment),this.setConstrain=function(a){J(a)};var K;this.setRevert=function(a){K=a};var L=function(a){return\"function\"==typeof a?(a._katavorioId=F(),a._katavorioId):a},M={},N=function(a){for(var b in M){var c=M[b],d=c[0](a);if(c[1]&&(d=!d),!d)return!1}return!0},O=this.setFilter=function(b,c){if(b){var d=L(b);M[d]=[function(c){var d,f=c.srcElement||c.target;return e(b)?d=g(f,b,a):\"function\"==typeof b&&(d=b(c,a)),d},c!==!1]}};this.addFilter=O,this.removeFilter=function(a){var b=\"function\"==typeof a?a._katavorioId:a;delete M[b]},this.clearAllFilters=function(){M={}},this.canDrag=this.params.canDrag||t;var P,Q=[],R=[];this.downListener=function(a){var b=this.rightButtonCanDrag||3!==a.which&&2!==a.button;if(b&&this.isEnabled()&&this.canDrag()){var e=N(a)&&z(a,this.el,this.k);if(e){if(s){r=this.el.cloneNode(!0),r.setAttribute(\"id\",null),r.style.position=\"absolute\";var g=f(this.el);r.style.left=g.left+\"px\",r.style.top=g.top+\"px\",document.body.appendChild(r)}else r=this.el;q&&x(a),h=m(a),this.params.bind(document,\"mousemove\",this.moveListener),this.params.bind(document,\"mouseup\",this.upListener),d.markSelection(this),d.markPosses(this),this.params.addClass(document.body,c.noSelect),T(\"beforeStart\",{el:this.el,pos:k,e:a,drag:this})}else this.params.consumeFilteredEvents&&x(a)}}.bind(this),this.moveListener=function(a){if(h){if(!o){var b=T(\"start\",{el:this.el,pos:k,e:a,drag:this});if(b!==!1){if(!h)return;this.mark(!0),o=!0}}if(h){R.length=0;var c=m(a),e=c[0]-h[0],f=c[1]-h[1],g=this.params.ignoreZoom?1:d.getZoom();e/=g,f/=g,this.moveBy(e,f,a),d.updateSelection(e,f,this),d.updatePosses(e,f,this)}}}.bind(this),this.upListener=function(a){h&&(h=null,this.params.unbind(document,\"mousemove\",this.moveListener),this.params.unbind(document,\"mouseup\",this.upListener),this.params.removeClass(document.body,c.noSelect),this.unmark(a),d.unmarkSelection(this,a),d.unmarkPosses(this,a),this.stop(a),d.notifySelectionDragStop(this,a),d.notifyPosseDragStop(this,a),o=!1,s&&(r&&r.parentNode&&r.parentNode.removeChild(r),r=null),K&&K(this.el,this.params.getPosition(this.el))===!0&&(this.params.setPosition(this.el,k),T(\"revert\",this.el)))}.bind(this),this.getFilters=function(){return M},this.abort=function(){null!=h&&this.upListener()},this.getDragElement=function(){return r||this.el};var S={start:[],drag:[],stop:[],over:[],out:[],beforeStart:[],revert:[]};b.events.start&&S.start.push(b.events.start),b.events.beforeStart&&S.beforeStart.push(b.events.beforeStart),b.events.stop&&S.stop.push(b.events.stop),b.events.drag&&S.drag.push(b.events.drag),b.events.revert&&S.revert.push(b.events.revert),this.on=function(a,b){S[a]&&S[a].push(b)},this.off=function(a,b){if(S[a]){for(var c=[],d=0;d<S[a].length;d++)S[a][d]!==b&&c.push(S[a][d]);S[a]=c}};var T=function(a,b){if(S[a])for(var c=0;c<S[a].length;c++)try{S[a][c](b)}catch(d){}};this.notifyStart=function(a){T(\"start\",{el:this.el,pos:this.params.getPosition(r),e:a,drag:this})},this.stop=function(a,b){if(b||o){var c=[],e=d.getSelection(),f=this.params.getPosition(r);if(e.length>1)for(var g=0;g<e.length;g++){var h=this.params.getPosition(e[g].el);c.push([e[g].el,{left:h[0],top:h[1]},e[g]])}else c.push([r,{left:f[0],top:f[1]},this]);T(\"stop\",{el:r,pos:U||f,finalPos:f,e:a,drag:this,selection:c})}},this.mark=function(a){k=this.params.getPosition(r),l=this.params.getPosition(r,!0),n=[l[0]-k[0],l[1]-k[1]],this.size=this.params.getSize(r),Q=d.getMatchingDroppables(this),v(Q,!0,!1,this),this.params.addClass(r,this.params.dragClass||c.drag);var b=this.params.getSize(r.parentNode);P={w:b[0],h:b[1]},a&&d.notifySelectionDragStart(this)};var U;this.unmark=function(a,d){if(v(Q,!1,!0,this),w&&y(this.el)?(U=[r.offsetLeft,r.offsetTop],this.el.parentNode.removeChild(r),r=this.el):U=null,this.params.removeClass(r,this.params.dragClass||c.drag),Q.length=0,w=!1,!d){R.length>0&&U&&b.setPosition(this.el,U);for(var e=0;e<R.length;e++){var f=R[e].drop(this,a);if(f===!0)break}}},this.moveBy=function(a,c,d){R.length=0;var e=this.toGrid([k[0]+a,k[1]+c]),f=H(e,r);if(y(this.el))if(e[0]!=f[0]||e[1]!=f[1]){if(!w){var g=D(this.el);b.addClass(g,p.ghostProxy),this.el.parentNode.appendChild(g),r=g,w=!0}f=e}else w&&(this.el.parentNode.removeChild(r),r=this.el,w=!1);var h={x:f[0],y:f[1],w:this.size[0],h:this.size[1]},i={x:h.x+n[0],y:h.y+n[1],w:h.w,h:h.h},j=null;this.params.setPosition(r,f);for(var l=0;l<Q.length;l++){var m={x:Q[l].pagePosition[0],y:Q[l].pagePosition[1],w:Q[l].size[0],h:Q[l].size[1]};this.params.intersects(i,m)&&(u||null==j||j==Q[l].el)&&Q[l].canDrop(this)?(j||(j=Q[l].el),R.push(Q[l]),Q[l].setHover(this,!0,d)):Q[l].isHover()&&Q[l].setHover(this,!1,d)}T(\"drag\",{el:this.el,pos:f,e:d,drag:this})},this.destroy=function(){this.params.unbind(this.el,\"mousedown\",this.downListener),this.params.unbind(document,\"mousemove\",this.moveListener),this.params.unbind(document,\"mouseup\",this.upListener),this.downListener=null,this.upListener=null,this.moveListener=null},this.params.bind(this.el,\"mousedown\",this.downListener),this.params.handle?O(this.params.handle,!1):O(this.params.filter,this.params.filterExclude)},E=function(a,b,c){this._class=c.droppable,this.params=b||{},this._activeClass=this.params.activeClass||c.active,this._hoverClass=this.params.hoverClass||c.hover,A.apply(this,arguments);var d=!1;this.allowLoopback=this.params.allowLoopback!==!1,this.setActive=function(a){this.params[a?\"addClass\":\"removeClass\"](this.el,this._activeClass)},this.updatePosition=function(){this.position=this.params.getPosition(this.el),this.pagePosition=this.params.getPosition(this.el,!0),this.size=this.params.getSize(this.el)},this.canDrop=this.params.canDrop||function(){return!0},this.isHover=function(){return d},this.setHover=function(a,b,c){(b||null==this.el._katavorioDragHover||this.el._katavorioDragHover==a.el._katavorio)&&(this.params[b?\"addClass\":\"removeClass\"](this.el,this._hoverClass),this.el._katavorioDragHover=b?a.el._katavorio:null,d!==b&&this.params.events[b?\"over\":\"out\"]({el:this.el,e:c,drag:a,drop:this}),d=b)},this.drop=function(a,b){return this.params.events.drop({drag:a,e:b,drop:this})},this.destroy=function(){this._class=null,this._activeClass=null,this._hoverClass=null,d=null}},F=function(){return\"xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx\".replace(/[xy]/g,function(a){var b=0|16*Math.random(),c=\"x\"==a?b:8|3&b;return c.toString(16)})},G=function(a){return null==a?null:(a=\"string\"==typeof a||a.constructor==String?document.getElementById(a):a,null==a?null:(a._katavorio=a._katavorio||F(),a))};a.Katavorio=function(a){var f=[],g={};this._dragsByScope={},this._dropsByScope={};var h=1,i=function(a,b){w(a,function(a){for(var c=0;c<a.scopes.length;c++)b[a.scopes[c]]=b[a.scopes[c]]||[],b[a.scopes[c]].push(a)})},j=function(b,c){var d=0;return w(b,function(b){for(var e=0;e<b.scopes.length;e++)if(c[b.scopes[e]]){var f=a.indexOf(c[b.scopes[e]],b);-1!=f&&(c[b.scopes[e]].splice(f,1),d++)}}),d>0},k=(this.getMatchingDroppables=function(a){for(var b=[],c={},d=0;d<a.scopes.length;d++){var e=this._dropsByScope[a.scopes[d]];if(e)for(var f=0;f<e.length;f++)!e[f].canDrop(a)||c[e[f].uuid]||!e[f].allowLoopback&&e[f].el===a.el||(c[e[f].uuid]=!0,b.push(e[f]))}return b},function(b){b=b||{};var c,d={events:{}};for(c in a)d[c]=a[c];for(c in b)d[c]=b[c];for(c=0;c<r.length;c++)d.events[r[c]]=b[r[c]]||s;return d.katavorio=this,d}.bind(this)),l=function(a,b){for(var c=0;c<r.length;c++)b[r[c]]&&a.on(r[c],b[r[c]])}.bind(this),m={},n=a.css||{},o=a.scope||q;for(var t in p)m[t]=p[t];for(var t in n)m[t]=n[t];var v=a.inputFilterSelector||y;this.getInputFilterSelector=function(){return v},this.setInputFilterSelector=function(a){return v=a,this},this.draggable=function(b,c){var d=[];return w(b,function(b){if(b=G(b),null!=b)if(null==b._katavorioDrag){var e=k(c);b._katavorioDrag=new D(b,e,m,o),i(b._katavorioDrag,this._dragsByScope),d.push(b._katavorioDrag),a.addClass(b,m.draggable)}else l(b._katavorioDrag,c)}.bind(this)),d},this.droppable=function(b,c){var d=[];return w(b,function(b){if(b=G(b),null!=b){var e=new E(b,k(c),m,o);b._katavorioDrop=b._katavorioDrop||[],b._katavorioDrop.push(e),i(e,this._dropsByScope),d.push(e),a.addClass(b,m.droppable)}}.bind(this)),d},this.select=function(b){return w(b,function(){var b=G(this);b&&b._katavorioDrag&&(g[b._katavorio]||(f.push(b._katavorioDrag),g[b._katavorio]=[b,f.length-1],a.addClass(b,m.selected)))}),this},this.deselect=function(b){return w(b,function(){var b=G(this);if(b&&b._katavorio){var c=g[b._katavorio];if(c){for(var d=[],e=0;e<f.length;e++)f[e].el!==b&&d.push(f[e]);f=d,delete g[b._katavorio],a.removeClass(b,m.selected)}}}),this},this.deselectAll=function(){for(var b in g){var c=g[b];a.removeClass(c[0],m.selected)}f.length=0,g={}},this.markSelection=function(a){u(f,function(a){a.mark()},a)},this.markPosses=function(a){a.posses&&w(a.posses,function(b){a.posseRoles[b]&&B[b]&&u(B[b].members,function(a){a.mark()},a)})},this.unmarkSelection=function(a,b){u(f,function(a){a.unmark(b)},a)},this.unmarkPosses=function(a,b){a.posses&&w(a.posses,function(c){a.posseRoles[c]&&B[c]&&u(B[c].members,function(a){a.unmark(b,!0)},a)})},this.getSelection=function(){return f.slice(0)},this.updateSelection=function(a,b,c){u(f,function(c){c.moveBy(a,b)},c)};var x=function(a,b){b.posses&&w(b.posses,function(c){b.posseRoles[c]&&B[c]&&u(B[c].members,function(b){a(b)},b)})};this.updatePosses=function(a,b,c){x(function(c){c.moveBy(a,b)},c)},this.notifyPosseDragStop=function(a,b){x(function(a){a.stop(b,!0)},a)},this.notifySelectionDragStop=function(a,b){u(f,function(a){a.stop(b,!0)},a)},this.notifySelectionDragStart=function(a,b){u(f,function(a){a.notifyStart(b)},a)},this.setZoom=function(a){h=a},this.getZoom=function(){return h};var z=function(a,b,c,d){w(a,function(a){j(a,c),a[d](b),i(a,c)})};w([\"set\",\"add\",\"remove\",\"toggle\"],function(a){this[a+\"Scope\"]=function(b,c){z(b._katavorioDrag,c,this._dragsByScope,a+\"Scope\"),z(b._katavorioDrop,c,this._dropsByScope,a+\"Scope\")}.bind(this),this[a+\"DragScope\"]=function(b,c){z(b.constructor===D?b:b._katavorioDrag,c,this._dragsByScope,a+\"Scope\")}.bind(this),this[a+\"DropScope\"]=function(b,c){z(b.constructor===E?b:b._katavorioDrop,c,this._dropsByScope,a+\"Scope\")}.bind(this)}.bind(this)),this.snapToGrid=function(a,b){for(var c in this._dragsByScope)u(this._dragsByScope[c],function(c){c.snap(a,b)})},this.getDragsForScope=function(a){return this._dragsByScope[a]},this.getDropsForScope=function(a){return this._dropsByScope[a]};var A=function(a,b,c){a=G(a),a[b]&&(j(a[b],c)&&w(a[b],function(a){a.destroy()}),delete a[b])};this.elementRemoved=function(a){this.destroyDraggable(a),this.destroyDroppable(a)},this.destroyDraggable=function(a){A(a,\"_katavorioDrag\",this._dragsByScope)},this.destroyDroppable=function(a){A(a,\"_katavorioDrop\",this._dropsByScope)},this.reset=function(){this._dragsByScope={},this._dropsByScope={},f=[],g={},B={}};var B={},C=function(a,c,d){var f=e(c)?c:c.id,g=e(c)?!0:c.active!==!1,h=B[f]||function(){var a={name:f,members:[]};return B[f]=a,a}();return w(a,function(a){if(a._katavorioDrag){if(d&&null!=a._katavorioDrag.posseRoles[h.name])return;b(h.members,a._katavorioDrag),b(a._katavorioDrag.posses,h.name),a._katavorioDrag.posseRoles[h.name]=g}}),h};this.addToPosse=function(a){for(var b=[],c=1;c<arguments.length;c++)b.push(C(a,arguments[c]));return 1==b.length?b[0]:b},this.setPosse=function(a){for(var b=[],c=1;c<arguments.length;c++)b.push(C(a,arguments[c],!0).name);return w(a,function(a){if(a._katavorioDrag){var c=d(a._katavorioDrag.posses,b),e=[];Array.prototype.push.apply(e,a._katavorioDrag.posses);for(var f=0;f<c.length;f++)this.removeFromPosse(a,c[f])}}.bind(this)),1==b.length?b[0]:b},this.removeFromPosse=function(a,b){if(arguments.length<2)throw new TypeError(\"No posse id provided for remove operation\");for(var d=1;d<arguments.length;d++)b=arguments[d],w(a,function(a){if(a._katavorioDrag&&a._katavorioDrag.posses){var d=a._katavorioDrag;w(b,function(a){c(B[a].members,d),c(d.posses,a),delete d.posseRoles[a]})}})},this.removeFromAllPosses=function(a){w(a,function(a){if(a._katavorioDrag&&a._katavorioDrag.posses){var b=a._katavorioDrag;w(b.posses,function(a){c(B[a].members,b)}),b.posses.length=0,b.posseRoles={}}})},this.setPosseState=function(a,b,c){var d=B[b];d&&w(a,function(a){a._katavorioDrag&&a._katavorioDrag.posses&&(a._katavorioDrag.posseRoles[d.name]=c)})}}}.call(\"undefined\"!=typeof window?window:this),function(){var a=function(a){return\"[object Array]\"===Object.prototype.toString.call(a)},b=function(a){return\"[object Number]\"===Object.prototype.toString.call(a)},c=function(a){return\"string\"==typeof a},d=function(a){return\"boolean\"==typeof a},e=function(a){return null==a},f=function(a){return null==a?!1:\"[object Object]\"===Object.prototype.toString.call(a)},g=function(a){return\"[object Date]\"===Object.prototype.toString.call(a)},h=function(a){return\"[object Function]\"===Object.prototype.toString.call(a)},i=function(a){for(var b in a)if(a.hasOwnProperty(b))return!1;return!0},j=this,k=j.jsPlumbUtil={isArray:a,isString:c,isBoolean:d,isNull:e,isObject:f,isDate:g,isFunction:h,isEmpty:i,isNumber:b,clone:function(b){if(c(b))return\"\"+b;if(d(b))return!!b;if(g(b))return new Date(b.getTime());if(h(b))return b;if(a(b)){for(var e=[],i=0;i<b.length;i++)e.push(this.clone(b[i]));return e}if(f(b)){var j={};for(var k in b)j[k]=this.clone(b[k]);return j}return b},merge:function(b,e,g){var h,i,j={};for(g=g||[],i=0;i<g.length;i++)j[g[i]]=!0;var k=this.clone(b);for(i in e)if(null==k[i])k[i]=e[i];else if(c(e[i])||d(e[i]))j[i]?(h=[],h.push.apply(h,a(k[i])?k[i]:[k[i]]),h.push.apply(h,a(e[i])?e[i]:[e[i]]),k[i]=h):k[i]=e[i];else if(a(e[i]))h=[],a(k[i])&&h.push.apply(h,k[i]),h.push.apply(h,e[i]),k[i]=h;else if(f(e[i])){f(k[i])||(k[i]={});for(var l in e[i])k[i][l]=e[i][l]\n}return k},replace:function(a,b,c){if(null!=a){var d=a,e=d;return b.replace(/([^\\.])+/g,function(a,b,d,f){var g=a.match(/([^\\[0-9]+){1}(\\[)([0-9+])/),h=d+a.length>=f.length,i=function(){return e[g[1]]||function(){return e[g[1]]=[],e[g[1]]}()};if(h)g?i()[g[3]]=c:e[a]=c;else if(g){var j=i();e=j[g[3]]||function(){return j[g[3]]={},j[g[3]]}()}else e=e[a]||function(){return e[a]={},e[a]}()}),a}},functionChain:function(a,b,c){for(var d=0;d<c.length;d++){var e=c[d][0][c[d][1]].apply(c[d][0],c[d][2]);if(e===b)return e}return a},populate:function(b,d,e){var g=function(a){var b=a.match(/(\\${.*?})/g);if(null!=b)for(var c=0;c<b.length;c++){var e=d[b[c].substring(2,b[c].length-1)]||\"\";null!=e&&(a=a.replace(b[c],e))}return a},i=function(b){if(null!=b){if(c(b))return g(b);if(!h(b)||null!=e&&0!==(b.name||\"\").indexOf(e)){if(a(b)){for(var j=[],k=0;k<b.length;k++)j.push(i(b[k]));return j}if(f(b)){var l={};for(var m in b)l[m]=i(b[m]);return l}return b}return b(d)}};return i(b)},findWithFunction:function(a,b){if(a)for(var c=0;c<a.length;c++)if(b(a[c]))return c;return-1},removeWithFunction:function(a,b){var c=k.findWithFunction(a,b);return c>-1&&a.splice(c,1),-1!=c},remove:function(a,b){var c=a.indexOf(b);return c>-1&&a.splice(c,1),-1!=c},addWithFunction:function(a,b,c){-1==k.findWithFunction(a,c)&&a.push(b)},addToList:function(a,b,c,d){var e=a[b];return null==e&&(e=[],a[b]=e),e[d?\"unshift\":\"push\"](c),e},suggest:function(a,b,c){return-1===a.indexOf(b)?(c?a.unshift(b):a.push(b),!0):!1},extend:function(b,c){var d;for(c=a(c)?c:[c],d=0;d<c.length;d++)for(var e in c[d].prototype)c[d].prototype.hasOwnProperty(e)&&(b.prototype[e]=c[d].prototype[e]);var f=function(a,b){return function(){for(d=0;d<c.length;d++)c[d].prototype[a]&&c[d].prototype[a].apply(this,arguments);return b.apply(this,arguments)}},g=function(a){for(var c in a)b.prototype[c]=f(c,a[c])};if(arguments.length>2)for(d=2;d<arguments.length;d++)g(arguments[d]);return b},uuid:function(){return\"xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx\".replace(/[xy]/g,function(a){var b=0|16*Math.random(),c=\"x\"==a?b:8|3&b;return c.toString(16)})},logEnabled:!0,log:function(){if(k.logEnabled&&\"undefined\"!=typeof console)try{var a=arguments[arguments.length-1];console.log(a)}catch(b){}},wrap:function(a,b,c){return a=a||function(){},b=b||function(){},function(){var d=null;try{d=b.apply(this,arguments)}catch(e){k.log(\"jsPlumb function failed : \"+e)}if(null==c||d!==c)try{d=a.apply(this,arguments)}catch(e){k.log(\"wrapped function failed : \"+e)}return d}}};k.EventGenerator=function(){var a={},b=!1,c={ready:!0};this.bind=function(b,c,d){var e=function(b){k.addToList(a,b,c,d),c.__jsPlumb=c.__jsPlumb||{},c.__jsPlumb[j.jsPlumbUtil.uuid()]=b};if(\"string\"==typeof b)e(b);else if(null!=b.length)for(var f=0;f<b.length;f++)e(b[f]);return this},this.fire=function(d,e,f){if(!b&&a[d]){var g=a[d].length,h=0,i=!1,j=null;if(!this.shouldFireEvent||this.shouldFireEvent(d,e,f))for(;!i&&g>h&&j!==!1;){if(c[d])a[d][h].apply(this,[e,f]);else try{j=a[d][h].apply(this,[e,f])}catch(l){k.log(\"jsPlumb: fire failed for event \"+d+\" : \"+l)}h++,(null==a||null==a[d])&&(i=!0)}}return this},this.unbind=function(b,c){if(0===arguments.length)a={};else if(1===arguments.length){if(\"string\"==typeof b)delete a[b];else if(b.__jsPlumb){var d;for(var e in b.__jsPlumb)d=b.__jsPlumb[e],k.remove(a[d]||[],b)}}else 2===arguments.length&&k.remove(a[b]||[],c);return this},this.getListener=function(b){return a[b]},this.setSuspendEvents=function(a){b=a},this.isSuspendEvents=function(){return b},this.silently=function(a){this.setSuspendEvents(!0);try{a()}catch(b){j.jsPlumbUtil.log(\"Cannot execute silent function \"+b)}this.setSuspendEvents(!1)},this.cleanupListeners=function(){for(var b in a)a[b]=null}},k.EventGenerator.prototype={cleanup:function(){this.cleanupListeners()}}}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumbUtil;b.matchesSelector=function(a,b,c){c=c||a.parentNode;for(var d=c.querySelectorAll(b),e=0;e<d.length;e++)if(d[e]===a)return!0;return!1},b.consume=function(a,b){a.stopPropagation?a.stopPropagation():a.returnValue=!1,!b&&a.preventDefault&&a.preventDefault()},b.sizeElement=function(a,b,c,d,e){a&&(a.style.height=e+\"px\",a.height=e,a.style.width=d+\"px\",a.width=d,a.style.left=b+\"px\",a.style.top=c+\"px\")}}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a,b=this,c=[],d=b.jsPlumbUtil,e=function(){return\"\"+(new Date).getTime()},f=function(a){if(a._jsPlumb.paintStyle&&a._jsPlumb.hoverPaintStyle){var b={};r.extend(b,a._jsPlumb.paintStyle),r.extend(b,a._jsPlumb.hoverPaintStyle),delete a._jsPlumb.hoverPaintStyle,b.gradient&&a._jsPlumb.paintStyle.fillStyle&&delete b.gradient,a._jsPlumb.hoverPaintStyle=b}},g=[\"tap\",\"dbltap\",\"click\",\"dblclick\",\"mouseover\",\"mouseout\",\"mousemove\",\"mousedown\",\"mouseup\",\"contextmenu\"],h=function(a,b,c,d){var e=a.getAttachedElements();if(e)for(var f=0,g=e.length;g>f;f++)d&&d==e[f]||e[f].setHover(b,!0,c)},i=function(a){return null==a?null:a.split(\" \")},j=function(a,b,c){for(var d in b)a[d]=c},k=function(a,b){b=d.isArray(b)||null!=b.length&&!d.isString(b)?b:[b];for(var c=0;c<b.length;c++)try{a.apply(b[c],[b[c]])}catch(e){d.log(\".each iteration failed : \"+e)}},l=function(a,b,c){if(a.getDefaultType){var e=a.getTypeDescriptor(),f={},g=a.getDefaultType(),h=d.merge({},g);j(f,g,\"__default\");for(var i=0,k=a._jsPlumb.types.length;k>i;i++){var l=a._jsPlumb.types[i];if(\"__default\"!==l){var m=a._jsPlumb.instance.getType(l,e);null!=m&&(h=d.merge(h,m,[\"cssClass\"]),j(f,m,l))}}b&&(h=d.populate(h,b,\"_\")),a.applyType(h,c,f),c||a.repaint()}},m=b.jsPlumbUIComponent=function(a){d.EventGenerator.apply(this,arguments);var b=this,c=arguments,e=b.idPrefix,f=e+(new Date).getTime();this._jsPlumb={instance:a._jsPlumb,parameters:a.parameters||{},paintStyle:null,hoverPaintStyle:null,paintStyleInUse:null,hover:!1,beforeDetach:a.beforeDetach,beforeDrop:a.beforeDrop,overlayPlacements:[],hoverClass:a.hoverClass||a._jsPlumb.Defaults.HoverClass,types:[],typeCache:{}},this.cacheTypeItem=function(a,b,c){this._jsPlumb.typeCache[c]=this._jsPlumb.typeCache[c]||{},this._jsPlumb.typeCache[c][a]=b},this.getCachedTypeItem=function(a,b){return this._jsPlumb.typeCache[b]?this._jsPlumb.typeCache[b][a]:null},this.getId=function(){return f};var g=a.overlays||[],h={};if(this.defaultOverlayKeys){for(var i=0;i<this.defaultOverlayKeys.length;i++)Array.prototype.push.apply(g,this._jsPlumb.instance.Defaults[this.defaultOverlayKeys[i]]||[]);for(i=0;i<g.length;i++){var j=r.convertToFullOverlaySpec(g[i]);h[j[1].id]=j}}var k={overlays:h,parameters:a.parameters||{},scope:a.scope||this._jsPlumb.instance.getDefaultScope()};if(this.getDefaultType=function(){return k},this.appendToDefaultType=function(a){for(var b in a)k[b]=a[b]},a.events)for(i in a.events)b.bind(i,a.events[i]);this.clone=function(){var a=Object.create(this.constructor.prototype);return this.constructor.apply(a,c),a}.bind(this),this.isDetachAllowed=function(a){var b=!0;if(this._jsPlumb.beforeDetach)try{b=this._jsPlumb.beforeDetach(a)}catch(c){d.log(\"jsPlumb: beforeDetach callback failed\",c)}return b},this.isDropAllowed=function(a,b,c,e,f,g,h){var i=this._jsPlumb.instance.checkCondition(\"beforeDrop\",{sourceId:a,targetId:b,scope:c,connection:e,dropEndpoint:f,source:g,target:h});if(this._jsPlumb.beforeDrop)try{i=this._jsPlumb.beforeDrop({sourceId:a,targetId:b,scope:c,connection:e,dropEndpoint:f,source:g,target:h})}catch(j){d.log(\"jsPlumb: beforeDrop callback failed\",j)}return i};var l=[];this.setListenerComponent=function(a){for(var b=0;b<l.length;b++)l[b][3]=a}},n=function(a,b){var c=a._jsPlumb.types[b],d=a._jsPlumb.instance.getType(c,a.getTypeDescriptor());null!=d&&d.cssClass&&a.canvas&&a._jsPlumb.instance.removeClass(a.canvas,d.cssClass)};d.extend(b.jsPlumbUIComponent,d.EventGenerator,{getParameter:function(a){return this._jsPlumb.parameters[a]},setParameter:function(a,b){this._jsPlumb.parameters[a]=b},getParameters:function(){return this._jsPlumb.parameters},setParameters:function(a){this._jsPlumb.parameters=a},getClass:function(){return r.getClass(this.canvas)},hasClass:function(a){return r.hasClass(this.canvas,a)},addClass:function(a){r.addClass(this.canvas,a)},removeClass:function(a){r.removeClass(this.canvas,a)},updateClasses:function(a,b){r.updateClasses(this.canvas,a,b)},setType:function(a,b,c){this.clearTypes(),this._jsPlumb.types=i(a)||[],l(this,b,c)},getType:function(){return this._jsPlumb.types},reapplyTypes:function(a,b){l(this,a,b)},hasType:function(a){return-1!=this._jsPlumb.types.indexOf(a)},addType:function(a,b,c){var d=i(a),e=!1;if(null!=d){for(var f=0,g=d.length;g>f;f++)this.hasType(d[f])||(this._jsPlumb.types.push(d[f]),e=!0);e&&l(this,b,c)}},removeType:function(a,b,c){var d=i(a),e=!1,f=function(a){var b=this._jsPlumb.types.indexOf(a);return-1!=b?(n(this,b),this._jsPlumb.types.splice(b,1),!0):!1}.bind(this);if(null!=d){for(var g=0,h=d.length;h>g;g++)e=f(d[g])||e;e&&l(this,b,c)}},clearTypes:function(a,b){for(var c=this._jsPlumb.types.length,d=0;c>d;d++)n(this,0),this._jsPlumb.types.splice(0,1);l(this,a,b)},toggleType:function(a,b,c){var d=i(a);if(null!=d){for(var e=0,f=d.length;f>e;e++){var g=this._jsPlumb.types.indexOf(d[e]);-1!=g?(n(this,g),this._jsPlumb.types.splice(g,1)):this._jsPlumb.types.push(d[e])}l(this,b,c)}},applyType:function(a,b){if(this.setPaintStyle(a.paintStyle,b),this.setHoverPaintStyle(a.hoverPaintStyle,b),a.parameters)for(var c in a.parameters)this.setParameter(c,a.parameters[c]);this._jsPlumb.paintStyleInUse=this.getPaintStyle()},setPaintStyle:function(a,b){this._jsPlumb.paintStyle=a,this._jsPlumb.paintStyleInUse=this._jsPlumb.paintStyle,f(this),b||this.repaint()},getPaintStyle:function(){return this._jsPlumb.paintStyle},setHoverPaintStyle:function(a,b){this._jsPlumb.hoverPaintStyle=a,f(this),b||this.repaint()},getHoverPaintStyle:function(){return this._jsPlumb.hoverPaintStyle},destroy:function(a){(a||null==this.typeId)&&(this.cleanupListeners(),this.clone=null,this._jsPlumb=null)},isHover:function(){return this._jsPlumb.hover},setHover:function(a,b,c){if(this._jsPlumb&&!this._jsPlumb.instance.currentlyDragging&&!this._jsPlumb.instance.isHoverSuspended()){if(this._jsPlumb.hover=a,null!=this.canvas){if(null!=this._jsPlumb.instance.hoverClass){var d=a?\"addClass\":\"removeClass\";this._jsPlumb.instance[d](this.canvas,this._jsPlumb.instance.hoverClass)}null!=this._jsPlumb.hoverClass&&this._jsPlumb.instance[d](this.canvas,this._jsPlumb.hoverClass)}null!=this._jsPlumb.hoverPaintStyle&&(this._jsPlumb.paintStyleInUse=a?this._jsPlumb.hoverPaintStyle:this._jsPlumb.paintStyle,this._jsPlumb.instance.isSuspendDrawing()||(c=c||e(),this.repaint({timestamp:c,recalc:!1}))),this.getAttachedElements&&!b&&h(this,a,e(),this)}}});var o=0,p=function(){var a=o+1;return o++,a},q=b.jsPlumbInstance=function(f){this.Defaults={Anchor:\"Bottom\",Anchors:[null,null],ConnectionsDetachable:!0,ConnectionOverlays:[],Connector:\"Bezier\",Container:null,DoNotThrowErrors:!1,DragOptions:{},DropOptions:{},Endpoint:\"Dot\",EndpointOverlays:[],Endpoints:[null,null],EndpointStyle:{fillStyle:\"#456\"},EndpointStyles:[null,null],EndpointHoverStyle:null,EndpointHoverStyles:[null,null],HoverPaintStyle:null,LabelStyle:{color:\"black\"},LogEnabled:!1,Overlays:[],MaxConnections:1,PaintStyle:{lineWidth:4,strokeStyle:\"#456\"},ReattachConnections:!1,RenderMode:\"svg\",Scope:\"jsPlumb_DefaultScope\"},f&&r.extend(this.Defaults,f),this.logEnabled=this.Defaults.LogEnabled,this._connectionTypes={},this._endpointTypes={},d.EventGenerator.apply(this);var h=this,i=p(),j=h.bind,l={},n=1,o=function(a){if(null==a)return null;if(3==a.nodeType||8==a.nodeType)return{el:a,text:!0};var b=h.getElement(a);return{el:b,id:d.isString(a)&&null==b?a:Z(b)}};this.getInstanceIndex=function(){return i},this.setZoom=function(a,b){return n=a,h.fire(\"zoom\",n),b&&h.repaintEverything(),!0},this.getZoom=function(){return n};for(var q in this.Defaults)l[q]=this.Defaults[q];var s,t=[];this.unbindContainer=function(){if(null!=s&&t.length>0)for(var a=0;a<t.length;a++)h.off(s,t[a][0],t[a][1])},this.setContainer=function(a){this.unbindContainer(),a=this.getElement(a),this.select().each(function(b){b.moveParent(a)}),this.selectEndpoints().each(function(b){b.moveParent(a)});var b=s;s=a,t.length=0;for(var c={endpointclick:\"endpointClick\",endpointdblclick:\"endpointDblClick\"},d=function(a,b,d){var e=b.srcElement||b.target,f=(e&&e.parentNode?e.parentNode._jsPlumb:null)||(e?e._jsPlumb:null)||(e&&e.parentNode&&e.parentNode.parentNode?e.parentNode.parentNode._jsPlumb:null);if(f){f.fire(a,f,b);var g=d?c[d+a]||a:a;h.fire(g,f.component||f,b)}},e=function(a,b,c){t.push([a,c]),h.on(s,a,b,c)},f=function(a){e(a,\".jsplumb-connector\",function(b){d(a,b)}),e(a,\".jsplumb-endpoint\",function(b){d(a,b,\"endpoint\")}),e(a,\".jsplumb-overlay\",function(b){d(a,b)})},i=0;i<g.length;i++)f(g[i]);for(var j in z){var k=z[j].el;k.parentNode===b&&(b.removeChild(k),s.appendChild(k))}},this.getContainer=function(){return s},this.bind=function(a,b){\"ready\"===a&&v?b():j.apply(h,[a,b])},h.importDefaults=function(a){for(var b in a)h.Defaults[b]=a[b];return a.Container&&h.setContainer(a.Container),h},h.restoreDefaults=function(){return h.Defaults=r.extend({},l),h};var u=null,v=!1,w=[],x={},y={},z={},A={},B={},C={},D=!1,E=[],F=!1,G=null,H=this.Defaults.Scope,I=1,J=function(){return\"\"+I++},K=function(a,b){s?s.appendChild(a):b?this.getElement(b).appendChild(a):this.appendToRoot(a)}.bind(this),L=function(a,b,c,d){if(!r.headless&&!F){var f=Z(a),g=h.getDragManager().getElementsForDraggable(f);null==c&&(c=e());var i=rb({elId:f,offset:b,recalc:!1,timestamp:c});if(g)for(var j in g)rb({elId:g[j].id,offset:{left:i.o.left+g[j].offset.left,top:i.o.top+g[j].offset.top},recalc:!1,timestamp:c});if(h.anchorManager.redraw(f,b,c,null,d),g)for(var k in g)h.anchorManager.redraw(g[k].id,b,c,g[k].offset,d,!0)}},M=function(a){return y[a]},N=function(a,b,c,e,f){if(!r.headless){var g=null==b?!1:b;if(g&&r.isDragSupported(a,h)){var i=c||h.Defaults.DragOptions;if(i=r.extend({},i),r.isAlreadyDraggable(a,h))c.force&&h.initDraggable(a,i);else{var j=r.dragEvents.drag,k=r.dragEvents.stop,l=r.dragEvents.start,m=!1;qb(e,a),i[l]=d.wrap(i[l],function(){return h.setHoverSuspended(!0),h.select({source:a}).addClass(h.elementDraggingClass+\" \"+h.sourceElementDraggingClass,!0),h.select({target:a}).addClass(h.elementDraggingClass+\" \"+h.targetElementDraggingClass,!0),h.setConnectionBeingDragged(!0),i.canDrag?c.canDrag():void 0},!1),i[j]=d.wrap(i[j],function(){var b=h.getUIPosition(arguments,h.getZoom());null!=b&&(L(a,b,null,!0),m&&h.addClass(a,\"jsplumb-dragged\"),m=!0)}),i[k]=d.wrap(i[k],function(){for(var a,b=arguments[0].selection,c=function(b){null!=b[1]&&(a=h.getUIPosition([{el:b[2].el,pos:[b[1].left,b[1].top]}]),L(b[2].el,a)),h.removeClass(b[0],\"jsplumb-dragged\"),h.select({source:b[2].el}).removeClass(h.elementDraggingClass+\" \"+h.sourceElementDraggingClass,!0),h.select({target:b[2].el}).removeClass(h.elementDraggingClass+\" \"+h.targetElementDraggingClass,!0),h.getDragManager().dragEnded(b[2].el)},d=0;d<b.length;d++)c(b[d]);m=!1,h.setHoverSuspended(!1),h.setConnectionBeingDragged(!1)});var n=Z(a);C[n]=!0;var o=C[n];i.disabled=null==o?!1:!o,h.initDraggable(a,i),h.getDragManager().register(a),f&&h.fire(\"elementDraggable\",{el:a,options:i})}}}},O=function(a,b){for(var c=a.scope.split(/\\s/),d=b.scope.split(/\\s/),e=0;e<c.length;e++)for(var f=0;f<d.length;f++)if(d[f]==c[e])return!0;return!1},P=function(a,b){var c=r.extend({},a);if(b&&r.extend(c,b),c.source&&(c.source.endpoint?c.sourceEndpoint=c.source:c.source=h.getElement(c.source)),c.target&&(c.target.endpoint?c.targetEndpoint=c.target:c.target=h.getElement(c.target)),a.uuids&&(c.sourceEndpoint=M(a.uuids[0]),c.targetEndpoint=M(a.uuids[1])),c.sourceEndpoint&&c.sourceEndpoint.isFull())return d.log(h,\"could not add connection; source endpoint is full\"),void 0;if(c.targetEndpoint&&c.targetEndpoint.isFull())return d.log(h,\"could not add connection; target endpoint is full\"),void 0;if(!c.type&&c.sourceEndpoint&&(c.type=c.sourceEndpoint.connectionType),c.sourceEndpoint&&c.sourceEndpoint.connectorOverlays){c.overlays=c.overlays||[];for(var e=0,f=c.sourceEndpoint.connectorOverlays.length;f>e;e++)c.overlays.push(c.sourceEndpoint.connectorOverlays[e])}c.sourceEndpoint&&c.sourceEndpoint.scope&&(c.scope=c.sourceEndpoint.scope),!c[\"pointer-events\"]&&c.sourceEndpoint&&c.sourceEndpoint.connectorPointerEvents&&(c[\"pointer-events\"]=c.sourceEndpoint.connectorPointerEvents);var g=function(a,b){var c=r.extend({},a);for(var d in b)b[d]&&(c[d]=b[d]);return c},i=function(a,b,d){return h.addEndpoint(a,g(b,{anchor:c.anchors?c.anchors[d]:c.anchor,endpoint:c.endpoints?c.endpoints[d]:c.endpoint,paintStyle:c.endpointStyles?c.endpointStyles[d]:c.endpointStyle,hoverPaintStyle:c.endpointHoverStyles?c.endpointHoverStyles[d]:c.endpointHoverStyle}))},j=function(a,b,d,e){if(c[a]&&!c[a].endpoint&&!c[a+\"Endpoint\"]&&!c.newConnection){var f=Z(c[a]),g=d[f];if(g=g?g[e]:null){if(!g.enabled)return!1;var h=null!=g.endpoint&&g.endpoint._jsPlumb?g.endpoint:i(c[a],g.def,b);if(h.isFull())return!1;c[a+\"Endpoint\"]=h,h._doNotDeleteOnDetach=!1,h._deleteOnDetach=!0,g.uniqueEndpoint&&(g.endpoint?h.finalEndpoint=g.endpoint:(g.endpoint=h,h._deleteOnDetach=!1,h._doNotDeleteOnDetach=!0))}}};return j(\"source\",0,this.sourceEndpointDefinitions,c.type||\"default\")!==!1&&j(\"target\",1,this.targetEndpointDefinitions,c.type||\"default\")!==!1?(c.sourceEndpoint&&c.targetEndpoint&&(O(c.sourceEndpoint,c.targetEndpoint)||(c=null)),c):void 0}.bind(h),Q=function(a){var b=h.Defaults.ConnectionType||h.getDefaultConnectionType();a._jsPlumb=h,a.newConnection=Q,a.newEndpoint=S,a.endpointsByUUID=y,a.endpointsByElement=x,a.finaliseConnection=R,a.id=\"con_\"+J();var c=new b(a);return c.isDetachable()&&(c.endpoints[0].initDraggable(\"_jsPlumbSource\"),c.endpoints[1].initDraggable(\"_jsPlumbTarget\")),c},R=h.finaliseConnection=function(a,b,c,d){if(b=b||{},a.suspendedEndpoint||w.push(a),a.pending=null,a.endpoints[0].isTemporarySource=!1,d!==!1&&h.anchorManager.newConnection(a),L(a.source),!b.doNotFireConnectionEvent&&b.fireEvent!==!1){var e={connection:a,source:a.source,target:a.target,sourceId:a.sourceId,targetId:a.targetId,sourceEndpoint:a.endpoints[0],targetEndpoint:a.endpoints[1]};h.fire(\"connection\",e,c)}},S=function(a,b){var c=h.Defaults.EndpointType||r.Endpoint,d=r.extend({},a);d._jsPlumb=h,d.newConnection=Q,d.newEndpoint=S,d.endpointsByUUID=y,d.endpointsByElement=x,d.fireDetachEvent=ab,d.elementId=b||Z(d.source);var e=new c(d);return e.id=\"ep_\"+J(),qb(d.elementId,d.source),r.headless||h.getDragManager().endpointAdded(d.source,b),e},T=function(a,b,c){var d=x[a];if(d&&d.length)for(var e=0,f=d.length;f>e;e++){for(var g=0,h=d[e].connections.length;h>g;g++){var i=b(d[e].connections[g]);if(i)return}c&&c(d[e])}},U=function(a,b){return r.each(a,function(a){h.isDragSupported(a)&&(C[h.getAttribute(a,\"id\")]=b,h.setElementDraggable(a,b))})},V=function(a,b,c){b=\"block\"===b;var d=null;c&&(d=function(a){a.setVisible(b,!0,!0)});var e=o(a);T(e.id,function(a){if(b&&c){var d=a.sourceId===e.id?1:0;a.endpoints[d].isVisible()&&a.setVisible(!0)}else a.setVisible(b)},d)},W=function(a){var b;return r.each(a,function(a){var c=h.getAttribute(a,\"id\");return b=null==C[c]?!1:C[c],b=!b,C[c]=b,h.setDraggable(a,b),b}.bind(this)),b},X=function(a,b){var c=null;b&&(c=function(a){var b=a.isVisible();a.setVisible(!b)}),T(a,function(a){var b=a.isVisible();a.setVisible(!b)},c)},Y=function(a){var b=A[a];return b?{o:b,s:E[a]}:rb({elId:a})},Z=function(a,b,c){if(d.isString(a))return a;if(null==a)return null;var e=h.getAttribute(a,\"id\");return e&&\"undefined\"!==e||(2==arguments.length&&void 0!==arguments[1]?e=b:(1==arguments.length||3==arguments.length&&!arguments[2])&&(e=\"jsPlumb_\"+i+\"_\"+J()),c||h.setAttribute(a,\"id\",e)),e};this.setConnectionBeingDragged=function(a){D=a},this.isConnectionBeingDragged=function(){return D},this.getManagedElements=function(){return z},this.getRenderMode=function(){return\"svg\"},this.connectorClass=\"jsplumb-connector\",this.connectorOutlineClass=\"jsplumb-connector-outline\",this.editableConnectorClass=\"jsplumb-connector-editable\",this.connectedClass=\"jsplumb-connected\",this.hoverClass=\"jsplumb-hover\",this.endpointClass=\"jsplumb-endpoint\",this.endpointConnectedClass=\"jsplumb-endpoint-connected\",this.endpointFullClass=\"jsplumb-endpoint-full\",this.endpointDropAllowedClass=\"jsplumb-endpoint-drop-allowed\",this.endpointDropForbiddenClass=\"jsplumb-endpoint-drop-forbidden\",this.overlayClass=\"jsplumb-overlay\",this.draggingClass=\"jsplumb-dragging\",this.elementDraggingClass=\"jsplumb-element-dragging\",this.sourceElementDraggingClass=\"jsplumb-source-element-dragging\",this.targetElementDraggingClass=\"jsplumb-target-element-dragging\",this.endpointAnchorClassPrefix=\"jsplumb-endpoint-anchor\",this.hoverSourceClass=\"jsplumb-source-hover\",this.hoverTargetClass=\"jsplumb-target-hover\",this.dragSelectClass=\"jsplumb-drag-select\",this.Anchors={},this.Connectors={svg:{}},this.Endpoints={svg:{}},this.Overlays={svg:{}},this.ConnectorRenderers={},this.SVG=\"svg\",this.addEndpoint=function(a,b,c){c=c||{};var e=r.extend({},c);r.extend(e,b),e.endpoint=e.endpoint||h.Defaults.Endpoint,e.paintStyle=e.paintStyle||h.Defaults.EndpointStyle;for(var f=[],g=d.isArray(a)||null!=a.length&&!d.isString(a)?a:[a],i=0,j=g.length;j>i;i++){e.source=h.getElement(g[i]),ob(e.source);var k=Z(e.source),l=S(e,k),m=qb(k,e.source).info.o;d.addToList(x,k,l),F||l.paint({anchorLoc:l.anchor.compute({xy:[m.left,m.top],wh:E[k],element:l,timestamp:G}),timestamp:G}),f.push(l),l._doNotDeleteOnDetach=!0}return 1==f.length?f[0]:f},this.addEndpoints=function(a,b,c){for(var e=[],f=0,g=b.length;g>f;f++){var i=h.addEndpoint(a,b[f],c);d.isArray(i)?Array.prototype.push.apply(e,i):e.push(i)}return e},this.animate=function(a,b,c){if(!this.animationSupported)return!1;c=c||{};var e=h.getElement(a),f=Z(e),g=r.animEvents.step,i=r.animEvents.complete;c[g]=d.wrap(c[g],function(){h.revalidate(f)}),c[i]=d.wrap(c[i],function(){h.revalidate(f)}),h.doAnimate(e,b,c)},this.checkCondition=function(a){var b=h.getListener(a),c=!0;if(b&&b.length>0){var e=Array.prototype.slice.call(arguments,1);try{for(var f=0,g=b.length;g>f;f++)c=c&&b[f].apply(b[f],e)}catch(i){d.log(h,\"cannot check condition [\"+a+\"]\"+i)}}return c},this.connect=function(a,b){var c,e=P(a,b);if(e){if(null==e.source&&null==e.sourceEndpoint)return d.log(\"Cannot establish connection - source does not exist\"),void 0;if(null==e.target&&null==e.targetEndpoint)return d.log(\"Cannot establish connection - target does not exist\"),void 0;ob(e.source),c=Q(e),R(c,e)}return c};var $=[{el:\"source\",elId:\"sourceId\",epDefs:\"sourceEndpointDefinitions\"},{el:\"target\",elId:\"targetId\",epDefs:\"targetEndpointDefinitions\"}],_=function(a,b,c,d){var e,f,g,h=$[c],i=a[h.elId],j=(a[h.el],a.endpoints[c]),k={index:c,originalSourceId:0===c?i:a.sourceId,newSourceId:a.sourceId,originalTargetId:1==c?i:a.targetId,newTargetId:a.targetId,connection:a};if(b.constructor==r.Endpoint)e=b,e.addConnection(a),b=e.element;else if(f=Z(b),g=this[h.epDefs][f],f===a[h.elId])e=null;else if(g)for(var l in g){if(!g[l].enabled)return;e=null!=g[l].endpoint&&g[l].endpoint._jsPlumb?g[l].endpoint:this.addEndpoint(b,g[l].def),g[l].uniqueEndpoint&&(g[l].endpoint=e),e._doNotDeleteOnDetach=!1,e._deleteOnDetach=!0,e.addConnection(a)}else e=a.makeEndpoint(0===c,b,f),e._doNotDeleteOnDetach=!1,e._deleteOnDetach=!0;return null!=e&&(j.detachFromConnection(a),a.endpoints[c]=e,a[h.el]=e.element,a[h.elId]=e.elementId,k[0===c?\"newSourceId\":\"newTargetId\"]=e.elementId,bb(k),d||a.repaint()),k.element=b,k}.bind(this);this.setSource=function(a,b,c){var d=_(a,b,0,c);this.anchorManager.sourceChanged(d.originalSourceId,d.newSourceId,a,d.el)},this.setTarget=function(a,b,c){var d=_(a,b,1,c);this.anchorManager.updateOtherEndpoint(d.originalSourceId,d.originalTargetId,d.newTargetId,a)},this.deleteEndpoint=function(a,b,c){var d=\"string\"==typeof a?y[a]:a;return d&&h.deleteObject({endpoint:d,dontUpdateHover:b,deleteAttachedObjects:c}),h},this.deleteEveryEndpoint=function(){var a=h.setSuspendDrawing(!0);for(var b in x){var c=x[b];if(c&&c.length)for(var d=0,e=c.length;e>d;d++)h.deleteEndpoint(c[d],!0)}return x={},z={},y={},A={},B={},h.anchorManager.reset(),h.getDragManager().reset(),a||h.setSuspendDrawing(!1),h};var ab=function(a,b,c){var d=h.Defaults.ConnectionType||h.getDefaultConnectionType(),e=a.constructor==d,f=e?{connection:a,source:a.source,target:a.target,sourceId:a.sourceId,targetId:a.targetId,sourceEndpoint:a.endpoints[0],targetEndpoint:a.endpoints[1]}:a;b&&h.fire(\"connectionDetached\",f,c),h.anchorManager.connectionDetached(f)},bb=h.fireMoveEvent=function(a,b){h.fire(\"connectionMoved\",a,b)};this.unregisterEndpoint=function(a){a._jsPlumb.uuid&&(y[a._jsPlumb.uuid]=null),h.anchorManager.deleteEndpoint(a);for(var b in x){var c=x[b];if(c){for(var d=[],e=0,f=c.length;f>e;e++)c[e]!=a&&d.push(c[e]);x[b]=d}x[b].length<1&&delete x[b]}},this.detach=function(){if(0!==arguments.length){var a=h.Defaults.ConnectionType||h.getDefaultConnectionType(),b=arguments[0].constructor==a,c=2==arguments.length?b?arguments[1]||{}:arguments[0]:arguments[0],e=c.fireEvent!==!1,f=c.forceDetach,g=b?arguments[0]:c.connection,i=b?null:c.deleteAttachedObjects;if(g)(f||d.functionChain(!0,!1,[[g.endpoints[0],\"isDetachAllowed\",[g]],[g.endpoints[1],\"isDetachAllowed\",[g]],[g,\"isDetachAllowed\",[g]],[h,\"checkCondition\",[\"beforeDetach\",g]]]))&&g.endpoints[0].detach({connection:g,ignoreTarget:!1,forceDetach:!0,fireEvent:e,deleteAttachedObjects:i});else{var j=r.extend({},c);if(j.uuids)M(j.uuids[0]).detachFrom(M(j.uuids[1]),e);else if(j.sourceEndpoint&&j.targetEndpoint)j.sourceEndpoint.detachFrom(j.targetEndpoint);else{var k=Z(h.getElement(j.source)),l=Z(h.getElement(j.target));T(k,function(a){(a.sourceId==k&&a.targetId==l||a.targetId==k&&a.sourceId==l)&&h.checkCondition(\"beforeDetach\",a)&&a.endpoints[0].detach({connection:a,ignoreTarget:!1,forceDetach:!0,fireEvent:e})})}}}},this.detachAllConnections=function(a,b){b=b||{},a=h.getElement(a);var c=Z(a),d=x[c];if(d&&d.length)for(var e=0,f=d.length;f>e;e++)d[e].detachAll(b.fireEvent!==!1,b.forceDetach);return h},this.detachEveryConnection=function(a){return a=a||{},h.batch(function(){for(var b in x){var c=x[b];if(c&&c.length)for(var d=0,e=c.length;e>d;d++)c[d].detachAll(a.fireEvent!==!1,a.forceDetach)}w.length=0}),h},this.deleteObject=function(a){var b={endpoints:{},connections:{},endpointCount:0,connectionCount:0},c=(a.fireEvent!==!1,a.deleteAttachedObjects!==!1),e=function(d){if(null!=d&&null==b.connections[d.id]&&(a.dontUpdateHover||null==d._jsPlumb||d.setHover(!1),b.connections[d.id]=d,b.connectionCount++,c))for(var e=0;e<d.endpoints.length;e++)d.endpoints[e]._deleteOnDetach&&f(d.endpoints[e])},f=function(d){if(null!=d&&null==b.endpoints[d.id]&&(a.dontUpdateHover||null==d._jsPlumb||d.setHover(!1),b.endpoints[d.id]=d,b.endpointCount++,c))for(var f=0;f<d.connections.length;f++){var g=d.connections[f];e(g)}};a.connection?e(a.connection):f(a.endpoint);for(var g in b.connections){var i=b.connections[g];if(i._jsPlumb){d.removeWithFunction(w,function(a){return i.id==a.id}),ab(i,a.fireEvent===!1?!1:!i.pending,a.originalEvent);var j=null==a.deleteAttachedObjects?null:!a.deleteAttachedObjects;i.endpoints[0].detachFromConnection(i,null,j),i.endpoints[1].detachFromConnection(i,null,j),i.cleanup(!0),i.destroy(!0)}}for(var k in b.endpoints){var l=b.endpoints[k];l._jsPlumb&&(h.unregisterEndpoint(l),l.cleanup(!0),l.destroy(!0))}return b},this.draggable=function(a,b){var c;return k(function(a){c=o(a),c.el&&N(c.el,!0,b,c.id,!0)},a),h},this.droppable=function(a,b){var c;return b=b||{},b.allowLoopback=!1,k(function(a){c=o(a),c.el&&h.initDroppable(c.el,b)},a),h};var cb=function(a,b,c,d){for(var e=0,f=a.length;f>e;e++)a[e][b].apply(a[e],c);return d(a)},db=function(a,b,c){for(var d=[],e=0,f=a.length;f>e;e++)d.push([a[e][b].apply(a[e],c),a[e]]);return d},eb=function(a,b,c){return function(){return cb(a,b,arguments,c)}},fb=function(a,b){return function(){return db(a,b,arguments)}},gb=function(a,b){var c=[];if(a)if(\"string\"==typeof a){if(\"*\"===a)return a;c.push(a)}else if(b)c=a;else if(a.length)for(var d=0,e=a.length;e>d;d++)c.push(o(a[d]).id);else c.push(o(a).id);return c},hb=function(a,b,c){return\"*\"===a?!0:a.length>0?-1!=a.indexOf(b):!c};this.getConnections=function(a,b){a?a.constructor==String&&(a={scope:a}):a={};for(var c=a.scope||h.getDefaultScope(),d=gb(c,!0),e=gb(a.source),f=gb(a.target),g=!b&&d.length>1?{}:[],i=function(a,c){if(!b&&d.length>1){var e=g[a];null==e&&(e=g[a]=[]),e.push(c)}else g.push(c)},j=0,k=w.length;k>j;j++){var l=w[j],m=l.proxies&&l.proxies[0]?l.proxies[0].originalEp.elementId:l.sourceId,n=l.proxies&&l.proxies[1]?l.proxies[1].originalEp.elementId:l.targetId;hb(d,l.scope)&&hb(e,m)&&hb(f,n)&&i(l.scope,l)}return g};var ib=function(a,b){return function(c){for(var d=0,e=a.length;e>d;d++)c(a[d]);return b(a)}},jb=function(a){return function(b){return a[b]}},kb=function(a,b){var c,d,e={length:a.length,each:ib(a,b),get:jb(a)},f=[\"setHover\",\"removeAllOverlays\",\"setLabel\",\"addClass\",\"addOverlay\",\"removeOverlay\",\"removeOverlays\",\"showOverlay\",\"hideOverlay\",\"showOverlays\",\"hideOverlays\",\"setPaintStyle\",\"setHoverPaintStyle\",\"setSuspendEvents\",\"setParameter\",\"setParameters\",\"setVisible\",\"repaint\",\"addType\",\"toggleType\",\"removeType\",\"removeClass\",\"setType\",\"bind\",\"unbind\"],g=[\"getLabel\",\"getOverlay\",\"isHover\",\"getParameter\",\"getParameters\",\"getPaintStyle\",\"getHoverPaintStyle\",\"isVisible\",\"hasType\",\"getType\",\"isSuspendEvents\"];for(c=0,d=f.length;d>c;c++)e[f[c]]=eb(a,f[c],b);for(c=0,d=g.length;d>c;c++)e[g[c]]=fb(a,g[c]);return e},lb=function(a){var b=kb(a,lb);return r.extend(b,{setDetachable:eb(a,\"setDetachable\",lb),setReattach:eb(a,\"setReattach\",lb),setConnector:eb(a,\"setConnector\",lb),detach:function(){for(var b=0,c=a.length;c>b;b++)h.detach(a[b])},isDetachable:fb(a,\"isDetachable\"),isReattach:fb(a,\"isReattach\")})},mb=function(a){var b=kb(a,mb);return r.extend(b,{setEnabled:eb(a,\"setEnabled\",mb),setAnchor:eb(a,\"setAnchor\",mb),isEnabled:fb(a,\"isEnabled\"),detachAll:function(){for(var b=0,c=a.length;c>b;b++)a[b].detachAll()},remove:function(){for(var b=0,c=a.length;c>b;b++)h.deleteObject({endpoint:a[b]})}})};this.select=function(a){return a=a||{},a.scope=a.scope||\"*\",lb(a.connections||h.getConnections(a,!0))},this.selectEndpoints=function(a){a=a||{},a.scope=a.scope||\"*\";var b=!a.element&&!a.source&&!a.target,c=b?\"*\":gb(a.element),d=b?\"*\":gb(a.source),e=b?\"*\":gb(a.target),f=gb(a.scope,!0),g=[];for(var h in x){var i=hb(c,h,!0),j=hb(d,h,!0),k=\"*\"!=d,l=hb(e,h,!0),m=\"*\"!=e;if(i||j||l)a:for(var n=0,o=x[h].length;o>n;n++){var p=x[h][n];if(hb(f,p.scope,!0)){var q=k&&d.length>0&&!p.isSource,r=m&&e.length>0&&!p.isTarget;if(q||r)continue a;g.push(p)}}}return mb(g)},this.getAllConnections=function(){return w},this.getDefaultScope=function(){return H},this.getEndpoint=M,this.getEndpoints=function(a){return x[o(a).id]},this.getDefaultEndpointType=function(){return r.Endpoint},this.getDefaultConnectionType=function(){return r.Connection},this.getId=Z,this.appendElement=K;var nb=!1;this.isHoverSuspended=function(){return nb},this.setHoverSuspended=function(a){nb=a},this.hide=function(a,b){return V(a,\"none\",b),h},this.idstamp=J,this.connectorsInitialized=!1,this.registerConnectorType=function(a,b){c.push([a,b])};var ob=function(a){if(!s&&a){var b=h.getElement(a);b.offsetParent&&h.setContainer(b.offsetParent)}},pb=function(){h.Defaults.Container&&h.setContainer(h.Defaults.Container)},qb=h.manage=function(a,b,c){return z[a]||(z[a]={el:b,endpoints:[],connections:[]},z[a].info=rb({elId:a,timestamp:G}),c||h.fire(\"manageElement\",{id:a,info:z[a].info,el:b})),z[a]},rb=this.updateOffset=function(a){var b,c=a.timestamp,d=a.recalc,e=a.offset,f=a.elId;return F&&!c&&(c=G),!d&&c&&c===B[f]?{o:a.offset||A[f],s:E[f]}:(d||!e&&null==A[f]?(b=z[f]?z[f].el:null,null!=b&&(E[f]=h.getSize(b),A[f]=h.getOffset(b),B[f]=c)):(A[f]=e||A[f],null==E[f]&&(b=z[f].el,null!=b&&(E[f]=h.getSize(b))),B[f]=c),A[f]&&!A[f].right&&(A[f].right=A[f].left+E[f][0],A[f].bottom=A[f].top+E[f][1],A[f].width=E[f][0],A[f].height=E[f][1],A[f].centerx=A[f].left+A[f].width/2,A[f].centery=A[f].top+A[f].height/2),{o:A[f],s:E[f]})\n};this.init=function(){a=b.jsPlumb.getRenderModes();var e=function(a,c,e){b.jsPlumb.Connectors[a][c]=function(){e.apply(this,arguments),b.jsPlumb.ConnectorRenderers[a].apply(this,arguments)},d.extend(b.jsPlumb.Connectors[a][c],[e,b.jsPlumb.ConnectorRenderers[a]])};if(!b.jsPlumb.connectorsInitialized){for(var f=0;f<c.length;f++)for(var g=0;g<a.length;g++)e(a[g],c[f][1],c[f][0]);b.jsPlumb.connectorsInitialized=!0}v||(pb(),h.anchorManager=new b.jsPlumb.AnchorManager({jsPlumbInstance:h}),v=!0,h.fire(\"ready\",h))}.bind(this),this.log=u,this.jsPlumbUIComponent=m,this.makeAnchor=function(){var a,c=function(a,c){if(b.jsPlumb.Anchors[a])return new b.jsPlumb.Anchors[a](c);if(!h.Defaults.DoNotThrowErrors)throw{msg:\"jsPlumb: unknown anchor type '\"+a+\"'\"}};if(0===arguments.length)return null;var e=arguments[0],f=arguments[1],g=(arguments[2],null);if(e.compute&&e.getOrientation)return e;if(\"string\"==typeof e)g=c(arguments[0],{elementId:f,jsPlumbInstance:h});else if(d.isArray(e))if(d.isArray(e[0])||d.isString(e[0]))2==e.length&&d.isObject(e[1])?d.isString(e[0])?(a=b.jsPlumb.extend({elementId:f,jsPlumbInstance:h},e[1]),g=c(e[0],a)):(a=b.jsPlumb.extend({elementId:f,jsPlumbInstance:h,anchors:e[0]},e[1]),g=new b.jsPlumb.DynamicAnchor(a)):g=new r.DynamicAnchor({anchors:e,selector:null,elementId:f,jsPlumbInstance:h});else{var i={x:e[0],y:e[1],orientation:e.length>=4?[e[2],e[3]]:[0,0],offsets:e.length>=6?[e[4],e[5]]:[0,0],elementId:f,jsPlumbInstance:h,cssClass:7==e.length?e[6]:null};g=new b.jsPlumb.Anchor(i),g.clone=function(){return new b.jsPlumb.Anchor(i)}}return g.id||(g.id=\"anchor_\"+J()),g},this.makeAnchors=function(a,c,e){for(var f=[],g=0,i=a.length;i>g;g++)\"string\"==typeof a[g]?f.push(b.jsPlumb.Anchors[a[g]]({elementId:c,jsPlumbInstance:e})):d.isArray(a[g])&&f.push(h.makeAnchor(a[g],c,e));return f},this.makeDynamicAnchor=function(a,c){return new b.jsPlumb.DynamicAnchor({anchors:a,selector:c,elementId:null,jsPlumbInstance:h})},this.targetEndpointDefinitions={};var sb=function(){};this.sourceEndpointDefinitions={};var tb=function(a,b,c,d,e){for(var f=a.target||a.srcElement,g=!1,h=d.getSelector(b,c),i=0;i<h.length;i++)if(h[i]==f){g=!0;break}return e?!g:g},ub=function(a,c,e,f,g){var i=new m(c),j=c._jsPlumb.EndpointDropHandler({jsPlumb:h,enabled:function(){return a.def.enabled},isFull:function(){var b=h.select({target:a.id}).length;return a.def.maxConnections>0&&b>=a.def.maxConnections},element:a.el,elementId:a.id,isSource:f,isTarget:g,addClass:function(b){h.addClass(a.el,b)},removeClass:function(b){h.removeClass(a.el,b)},onDrop:function(a){var b=a.endpoints[0];b.anchor.locked=!1},isDropAllowed:function(){return i.isDropAllowed.apply(i,arguments)},isRedrop:function(b){return null!=b.suspendedElement&&null!=b.suspendedEndpoint&&b.suspendedEndpoint.element===a.el},getEndpoint:function(d){var e=a.def.endpoint;if(null==e||null==e._jsPlumb){var f=h.deriveEndpointAndAnchorSpec(d.getType().join(\" \"),!0),g=f.endpoints?b.jsPlumb.extend(c,{endpoint:a.def.def.endpoint||f.endpoints[1]}):c;f.anchors&&(g=b.jsPlumb.extend(g,{anchor:a.def.def.anchor||f.anchors[1]})),e=h.addEndpoint(a.el,g),e._mtNew=!0}if(c.uniqueEndpoint&&(a.def.endpoint=e),e._doNotDeleteOnDetach=!1,e._deleteOnDetach=!0,d.isDetachable()&&e.initDraggable(),null!=e.anchor.positionFinder){var i=h.getUIPosition(arguments,h.getZoom()),j=h.getOffset(a.el),k=h.getSize(a.el),l=null==i?[0,0]:e.anchor.positionFinder(i,j,k,e.anchor.constructorParams);e.anchor.x=l[0],e.anchor.y=l[1]}return e},maybeCleanup:function(a){a._mtNew&&0===a.connections.length?h.deleteObject({endpoint:a}):delete a._mtNew}}),k=b.jsPlumb.dragEvents.drop;return e.scope=e.scope||c.scope||h.Defaults.Scope,e[k]=d.wrap(e[k],j,!0),g&&(e[b.jsPlumb.dragEvents.over]=function(){return!0}),c.allowLoopback===!1&&(e.canDrop=function(b){var c=b.getDragElement()._jsPlumbRelatedElement;return c!=a.el}),h.initDroppable(a.el,e,\"internal\"),j};this.makeTarget=function(a,c,d){var e=b.jsPlumb.extend({_jsPlumb:this},d);b.jsPlumb.extend(e,c),sb(e,1,this);for(var f=(!(e.deleteEndpointsOnDetach===!1),e.maxConnections||-1),g=function(a){var c=o(a),d=c.id,g=b.jsPlumb.extend({},e.dropOptions||{}),h=\"default\";this.targetEndpointDefinitions[d]=this.targetEndpointDefinitions[d]||{},ob(d);var i={def:b.jsPlumb.extend({},e),uniqueEndpoint:e.uniqueEndpoint,maxConnections:f,enabled:!0};c.def=i,this.targetEndpointDefinitions[d][h]=i,ub(c,e,g,e.isSource===!0,!0),c.el._katavorioDrop[c.el._katavorioDrop.length-1].targetDef=i}.bind(this),h=a.length&&a.constructor!=String?a:[a],i=0,j=h.length;j>i;i++)g(h[i]);return this},this.unmakeTarget=function(a,b){var c=o(a);return h.destroyDroppable(c.el,\"internal\"),b||delete this.targetEndpointDefinitions[c.id],this},this.makeSource=function(a,c,e){var f=b.jsPlumb.extend({_jsPlumb:this},e);b.jsPlumb.extend(f,c);var g=f.connectionType||\"default\",i=h.deriveEndpointAndAnchorSpec(g);f.endpoint=f.endpoint||i.endpoints[0],f.anchor=f.anchor||i.anchors[0],sb(f,0,this);for(var j=f.maxConnections||-1,k=f.onMaxConnections,l=function(a){var c=a.id,e=this.getElement(a.el);this.sourceEndpointDefinitions[c]=this.sourceEndpointDefinitions[c]||{},ob(c);var i={def:b.jsPlumb.extend({},f),uniqueEndpoint:f.uniqueEndpoint,maxConnections:j,enabled:!0};this.sourceEndpointDefinitions[c][g]=i,a.def=i;var l=b.jsPlumb.dragEvents.stop,m=b.jsPlumb.dragEvents.drag,o=b.jsPlumb.extend({},f.dragOptions||{}),p=o.drag,q=o.stop,r=null,s=!1;o.scope=o.scope||f.scope,o[m]=d.wrap(o[m],function(){p&&p.apply(this,arguments),s=!1}),o[l]=d.wrap(o[l],function(){if(q&&q.apply(this,arguments),this.currentlyDragging=!1,null!=r._jsPlumb){var a=f.anchor||this.Defaults.Anchor,b=r.anchor,d=r.connections[0],e=this.makeAnchor(a,c,this),g=r.element;if(null!=e.positionFinder){var i=h.getOffset(g),j=this.getSize(g),k={left:i.left+b.x*j[0],top:i.top+b.y*j[1]},l=e.positionFinder(k,i,j,e.constructorParams);e.x=l[0],e.y=l[1]}r.setAnchor(e,!0),r.repaint(),this.repaint(r.elementId),null!=d&&this.repaint(d.targetId)}}.bind(this));var t=function(i){if(3!==i.which&&2!==i.button){var l=this.sourceEndpointDefinitions[c][g];if(l.enabled){if(c=this.getId(this.getElement(a.el)),f.filter){var m=d.isString(f.filter)?tb(i,a.el,f.filter,this,f.filterExclude):f.filter(i,a.el);if(m===!1)return}var p=this.select({source:c}).length;if(l.maxConnections>=0&&p>=l.maxConnections)return k&&k({element:a.el,maxConnections:j},i),!1;var q=b.jsPlumb.getPositionOnElement(i,e,n),t={};b.jsPlumb.extend(t,f),t.isTemporarySource=!0,t.anchor=[q[0],q[1],0,0],t.dragOptions=o,l.def.scope&&(t.scope=l.def.scope),r=this.addEndpoint(c,t),s=!0,r._doNotDeleteOnDetach=!1,r._deleteOnDetach=!0,l.uniqueEndpoint&&(l.endpoint?r.finalEndpoint=l.endpoint:(l.endpoint=r,r._deleteOnDetach=!1,r._doNotDeleteOnDetach=!0));var u=function(){h.off(r.canvas,\"mouseup\",u),h.off(a.el,\"mouseup\",u),s&&(s=!1,h.deleteEndpoint(r))};h.on(r.canvas,\"mouseup\",u),h.on(a.el,\"mouseup\",u);var v={};if(l.def.extract)for(var w in l.def.extract){var x=(i.srcElement||i.target).getAttribute(w);x&&(v[l.def.extract[w]]=x)}h.trigger(r.canvas,\"mousedown\",i,v),d.consume(i)}}}.bind(this);this.on(a.el,\"mousedown\",t),i.trigger=t,f.filter&&(d.isString(f.filter)||d.isFunction(f.filter))&&h.setDragFilter(a.el,f.filter);var u=b.jsPlumb.extend({},f.dropOptions||{});ub(a,f,u,!0,f.isTarget===!0)}.bind(this),m=a.length&&a.constructor!=String?a:[a],p=0,q=m.length;q>p;p++)l(o(m[p]));return this},this.unmakeSource=function(a,b,c){var d=o(a);h.destroyDroppable(d.el,\"internal\");var e=this.sourceEndpointDefinitions[d.id];if(e)for(var f in e)if(null==b||b===f){var g=e[f].trigger;g&&h.off(d.el,\"mousedown\",g),c||delete this.sourceEndpointDefinitions[d.id][f]}return this},this.unmakeEverySource=function(){for(var a in this.sourceEndpointDefinitions)h.unmakeSource(a,null,!0);return this.sourceEndpointDefinitions={},this};var vb=function(a,b,c){b=d.isArray(b)?b:[b];var e=Z(a);c=c||\"default\";for(var f=0;f<b.length;f++){var g=this[b[f]][e];if(g&&g[c])return g[c].def.scope||this.Defaults.Scope}}.bind(this),wb=function(a,b,c,e){c=d.isArray(c)?c:[c];var f=Z(a);e=e||\"default\";for(var g=0;g<c.length;g++){var h=this[c[g]][f];h&&h[e]&&(h[e].def.scope=b)}}.bind(this);this.getScope=function(a){return vb(a,[\"sourceEndpointDefinitions\",\"targetEndpointDefinitions\"])},this.getSourceScope=function(a){return vb(a,\"sourceEndpointDefinitions\")},this.getTargetScope=function(a){return vb(a,\"targetEndpointDefinitions\")},this.setScope=function(a,b,c){this.setSourceScope(a,b,c),this.setTargetScope(a,b,c)},this.setSourceScope=function(a,b,c){wb(a,b,\"sourceEndpointDefinitions\",c),this.setDragScope(a,b)},this.setTargetScope=function(a,b,c){wb(a,b,\"targetEndpointDefinitions\",c),this.setDropScope(a,b)},this.unmakeEveryTarget=function(){for(var a in this.targetEndpointDefinitions)h.unmakeTarget(a,!0);return this.targetEndpointDefinitions={},this};var xb=function(a,b,c,e,f){var g,i,j,k=\"source\"==a?this.sourceEndpointDefinitions:this.targetEndpointDefinitions;if(f=f||\"default\",b.length&&!d.isString(b)){g=[];for(var l=0,m=b.length;m>l;l++)i=o(b[l]),k[i.id]&&k[i.id][f]&&(g[l]=k[i.id][f].enabled,j=e?!g[l]:c,k[i.id][f].enabled=j,h[j?\"removeClass\":\"addClass\"](i.el,\"jtk-\"+a+\"-disabled\"))}else{i=o(b);var n=i.id;k[n]&&k[n][f]&&(g=k[n][f].enabled,j=e?!g:c,k[n][f].enabled=j,h[j?\"removeClass\":\"addClass\"](i.el,\"jtk-\"+a+\"-disabled\"))}return g}.bind(this),yb=function(a,b){return d.isString(a)||!a.length?b.apply(this,[a]):a.length?b.apply(this,[a[0]]):void 0}.bind(this);this.toggleSourceEnabled=function(a,b){return xb(\"source\",a,null,!0,b),this.isSourceEnabled(a,b)},this.setSourceEnabled=function(a,b,c){return xb(\"source\",a,b,null,c)},this.isSource=function(a,b){return b=b||\"default\",yb(a,function(a){var c=this.sourceEndpointDefinitions[o(a).id];return null!=c&&null!=c[b]}.bind(this))},this.isSourceEnabled=function(a,b){return b=b||\"default\",yb(a,function(a){var c=this.sourceEndpointDefinitions[o(a).id];return c&&c[b]&&c[b].enabled===!0}.bind(this))},this.toggleTargetEnabled=function(a,b){return xb(\"target\",a,null,!0,b),this.isTargetEnabled(a,b)},this.isTarget=function(a,b){return b=b||\"default\",yb(a,function(a){var c=this.targetEndpointDefinitions[o(a).id];return null!=c&&null!=c[b]}.bind(this))},this.isTargetEnabled=function(a,b){return b=b||\"default\",yb(a,function(a){var c=this.targetEndpointDefinitions[o(a).id];return c&&c[b]&&c[b].enabled===!0}.bind(this))},this.setTargetEnabled=function(a,b,c){return xb(\"target\",a,b,null,c)},this.ready=function(a){h.bind(\"ready\",a)};var zb=function(a,b){if(\"object\"==typeof a&&a.length)for(var c=0,d=a.length;d>c;c++)b(a[c]);else b(a);return h};this.repaint=function(a,b,c){return zb(a,function(a){L(a,b,c)})},this.revalidate=function(a,b,c){return zb(a,function(a){var d=c?a:h.getId(a);h.updateOffset({elId:d,recalc:!0,timestamp:b}),h.repaint(a)})},this.repaintEverything=function(){var a,b=e();for(a in x)h.updateOffset({elId:a,recalc:!0,timestamp:b});for(a in x)L(a,null,b);return this},this.removeAllEndpoints=function(a,b,c){c=c||[];var d=function(a){var e,f,g=o(a),i=x[g.id];if(i)for(c.push(g),e=0,f=i.length;f>e;e++)h.deleteEndpoint(i[e],!1);if(delete x[g.id],b&&g.el&&3!=g.el.nodeType&&8!=g.el.nodeType)for(e=0,f=g.el.childNodes.length;f>e;e++)d(g.el.childNodes[e])};return d(a),this};var Ab=function(a,b){h.removeAllEndpoints(a.id,!0,b);for(var c=function(a){h.getDragManager().elementRemoved(a.id),h.anchorManager.clearFor(a.id),h.anchorManager.removeFloatingConnection(a.id),h.isSource(a.el)&&h.unmakeSource(a.el),h.isTarget(a.el)&&h.unmakeTarget(a.el),h.destroyDraggable(a.el),h.destroyDroppable(a.el),delete h.floatingConnections[a.id],delete z[a.id],delete A[a.id],a.el&&(h.removeElement(a.el),a.el._jsPlumb=null)},d=1;d<b.length;d++)c(b[d]);c(a)};this.remove=function(a,b){var c=o(a),d=[];return c.text?c.el.parentNode.removeChild(c.el):c.id&&h.batch(function(){Ab(c,d)},b===!1),h},this.empty=function(a,b){var c=[],d=function(a,b){var e=o(a);if(e.text)e.el.parentNode.removeChild(e.el);else if(e.el){for(;e.el.childNodes.length>0;)d(e.el.childNodes[0]);b||Ab(e,c)}};return h.batch(function(){d(a,!0)},b===!1),h},this.reset=function(){h.silently(function(){nb=!1,h.removeAllGroups(),h.removeGroupManager(),h.deleteEveryEndpoint(),h.unbind(),this.targetEndpointDefinitions={},this.sourceEndpointDefinitions={},w.length=0,this.doReset&&this.doReset()}.bind(this))};var Bb=function(a){a.canvas&&a.canvas.parentNode&&a.canvas.parentNode.removeChild(a.canvas),a.cleanup(),a.destroy()};this.clear=function(){h.select().each(Bb),h.selectEndpoints().each(Bb),x={},y={}},this.setDefaultScope=function(a){return H=a,h},this.setDraggable=U,this.deriveEndpointAndAnchorSpec=function(a,b){for(var c=((b?\"\":\"default \")+a).split(/[\\s]/),d=null,e=null,f=null,g=null,i=0;i<c.length;i++){var j=h.getType(c[i],\"connection\");j&&(j.endpoints&&(d=j.endpoints),j.endpoint&&(e=j.endpoint),j.anchors&&(g=j.anchors),j.anchor&&(f=j.anchor))}return{endpoints:d?d:[e,e],anchors:g?g:[f,f]}},this.setId=function(a,b,c){var e;d.isString(a)?e=a:(a=this.getElement(a),e=this.getId(a));var f=this.getConnections({source:e,scope:\"*\"},!0),g=this.getConnections({target:e,scope:\"*\"},!0);b=\"\"+b,c?a=this.getElement(b):(a=this.getElement(e),this.setAttribute(a,\"id\",b)),x[b]=x[e]||[];for(var h=0,i=x[b].length;i>h;h++)x[b][h].setElementId(b),x[b][h].setReferenceElement(a);delete x[e],this.sourceEndpointDefinitions[b]=this.sourceEndpointDefinitions[e],delete this.sourceEndpointDefinitions[e],this.targetEndpointDefinitions[b]=this.targetEndpointDefinitions[e],delete this.targetEndpointDefinitions[e],this.anchorManager.changeId(e,b),this.getDragManager().changeId(e,b),z[b]=z[e],delete z[e];var j=function(c,d,e){for(var f=0,g=c.length;g>f;f++)c[f].endpoints[d].setElementId(b),c[f].endpoints[d].setReferenceElement(a),c[f][e+\"Id\"]=b,c[f][e]=a};j(f,0,\"source\"),j(g,1,\"target\"),this.repaint(b)},this.setDebugLog=function(a){u=a},this.setSuspendDrawing=function(a,b){var c=F;return F=a,G=a?(new Date).getTime():null,b&&this.repaintEverything(),c},this.isSuspendDrawing=function(){return F},this.getSuspendedAt=function(){return G},this.batch=function(a,b){var c=this.isSuspendDrawing();c||this.setSuspendDrawing(!0);try{a()}catch(e){d.log(\"Function run while suspended failed\",e)}c||this.setSuspendDrawing(!1,!b)},this.doWhileSuspended=this.batch,this.getCachedData=Y,this.timestamp=e,this.show=function(a,b){return V(a,\"block\",b),h},this.toggleVisible=X,this.toggleDraggable=W,this.addListener=this.bind};d.extend(b.jsPlumbInstance,d.EventGenerator,{setAttribute:function(a,b,c){this.setAttribute(a,b,c)},getAttribute:function(a,c){return this.getAttribute(b.jsPlumb.getElement(a),c)},convertToFullOverlaySpec:function(a){return d.isString(a)&&(a=[a,{}]),a[1].id=a[1].id||d.uuid(),a},registerConnectionType:function(a,c){if(this._connectionTypes[a]=b.jsPlumb.extend({},c),c.overlays){for(var d={},e=0;e<c.overlays.length;e++){var f=this.convertToFullOverlaySpec(c.overlays[e]);d[f[1].id]=f}this._connectionTypes[a].overlays=d}},registerConnectionTypes:function(a){for(var b in a)this.registerConnectionType(b,a[b])},registerEndpointType:function(a,c){if(this._endpointTypes[a]=b.jsPlumb.extend({},c),c.overlays){for(var d={},e=0;e<c.overlays.length;e++){var f=this.convertToFullOverlaySpec(c.overlays[e]);d[f[1].id]=f}this._endpointTypes[a].overlays=d}},registerEndpointTypes:function(a){for(var b in a)this.registerEndpointType(b,a[b])},getType:function(a,b){return\"connection\"===b?this._connectionTypes[a]:this._endpointTypes[a]},setIdChanged:function(a,b){this.setId(a,b,!0)},setParent:function(a,b){var c=this.getElement(a),d=this.getId(c),e=this.getElement(b),f=this.getId(e);c.parentNode.removeChild(c),e.appendChild(c),this.getDragManager().setParent(c,d,e,f)},extend:function(a,b,c){var d;if(c)for(d=0;d<c.length;d++)a[c[d]]=b[c[d]];else for(d in b)a[d]=b[d];return a},floatingConnections:{},getFloatingAnchorIndex:function(a){return a.endpoints[0].isFloating()?0:a.endpoints[1].isFloating()?1:-1}});var r=new q;b.jsPlumb=r,r.getInstance=function(a){var b=new q(a);return b.init(),b},r.each=function(a,b){if(null!=a)if(\"string\"==typeof a)b(r.getElement(a));else if(null!=a.length)for(var c=0;c<a.length;c++)b(r.getElement(a[c]));else b(a)},\"function\"==typeof define&&(define(\"jsplumb\",[],function(){return r}),define(\"jsplumbinstance\",[],function(){return r.getInstance()})),\"undefined\"!=typeof exports&&(exports.jsPlumb=r),\"undefined\"!=typeof module&&(module.exports=r)}.call(\"undefined\"!=typeof window?window:this),function(){var a=this,b=a.jsPlumbUtil,c=function(a,b){if(null==b)return[0,0];var c=h(b),d=g(c,0);return[d[a+\"X\"],d[a+\"Y\"]]},d=c.bind(this,\"page\"),e=c.bind(this,\"screen\"),f=c.bind(this,\"client\"),g=function(a,b){return a.item?a.item(b):a[b]},h=function(a){return a.touches&&a.touches.length>0?a.touches:a.changedTouches&&a.changedTouches.length>0?a.changedTouches:a.targetTouches&&a.targetTouches.length>0?a.targetTouches:[a]},i=function(a){var b={},c=[],d={},e={},f={};this.register=function(g){var h=a.getId(g),i=a.getOffset(g);b[h]||(b[h]=g,c.push(g),d[h]={});var j=function(b){if(b)for(var c=0;c<b.childNodes.length;c++)if(3!=b.childNodes[c].nodeType&&8!=b.childNodes[c].nodeType){var g=jsPlumb.getElement(b.childNodes[c]),k=a.getId(b.childNodes[c],null,!0);if(k&&e[k]&&e[k]>0){var l=a.getOffset(g);d[h][k]={id:k,offset:{left:l.left-i.left,top:l.top-i.top}},f[k]=h}j(b.childNodes[c])}};j(g)},this.updateOffsets=function(b,c){if(null!=b){c=c||{};var e=jsPlumb.getElement(b),g=a.getId(e),h=d[g],i=a.getOffset(e);if(h)for(var j in h)if(h.hasOwnProperty(j)){var k=jsPlumb.getElement(j),l=c[j]||a.getOffset(k);if(null==k.offsetParent&&null!=d[g][j])continue;d[g][j]={id:j,offset:{left:l.left-i.left,top:l.top-i.top}},f[j]=g}}},this.endpointAdded=function(c,g){g=g||a.getId(c);var h=document.body,i=c.parentNode;for(e[g]=e[g]?e[g]+1:1;null!=i&&i!=h;){var j=a.getId(i,null,!0);if(j&&b[j]){var k=a.getOffset(i);if(null==d[j][g]){var l=a.getOffset(c);d[j][g]={id:g,offset:{left:l.left-k.left,top:l.top-k.top}},f[g]=j}break}i=i.parentNode}},this.endpointDeleted=function(a){if(e[a.elementId]&&(e[a.elementId]--,e[a.elementId]<=0))for(var b in d)d.hasOwnProperty(b)&&d[b]&&(delete d[b][a.elementId],delete f[a.elementId])},this.changeId=function(a,b){d[b]=d[a],d[a]={},f[b]=f[a],f[a]=null},this.getElementsForDraggable=function(a){return d[a]},this.elementRemoved=function(a){var b=f[a];b&&(delete d[b][a],delete f[a])},this.reset=function(){b={},c=[],d={},e={}},this.dragEnded=function(b){if(null!=b.offsetParent){var c=a.getId(b),d=f[c];d&&this.updateOffsets(d)}},this.setParent=function(b,c,e,g,h){var i=f[c];d[g]||(d[g]={});var j=a.getOffset(e),k=h||a.getOffset(b);i&&delete d[i][c],d[g][c]={id:c,offset:{left:k.left-j.left,top:k.top-j.top}},f[c]=g},this.clearParent=function(a,b){var c=f[b];c&&(delete d[c][b],delete f[b])},this.revalidateParent=function(b,c,d){var e=f[c];if(e){var g={};g[c]=d,this.updateOffsets(e,g),a.revalidate(e)}},this.getDragAncestor=function(b){var c=jsPlumb.getElement(b),d=a.getId(c),e=f[d];return e?jsPlumb.getElement(e):null}},j=function(a){return null==a?null:a.replace(/^\\s\\s*/,\"\").replace(/\\s\\s*$/,\"\")},k=function(a,b){b=j(b),\"undefined\"!=typeof a.className.baseVal?a.className.baseVal=b:a.className=b},l=function(a){return\"undefined\"==typeof a.className.baseVal?a.className:a.className.baseVal},m=function(a,c,d){c=null==c?[]:b.isArray(c)?c:c.split(/\\s+/),d=null==d?[]:b.isArray(d)?d:d.split(/\\s+/);var e=l(a),f=e.split(/\\s+/),g=function(a,b){for(var c=0;c<b.length;c++)if(a)-1==f.indexOf(b[c])&&f.push(b[c]);else{var d=f.indexOf(b[c]);-1!=d&&f.splice(d,1)}};g(!0,c),g(!1,d),k(a,f.join(\" \"))};a.jsPlumb.extend(a.jsPlumbInstance.prototype,{headless:!1,pageLocation:d,screenLocation:e,clientLocation:f,getDragManager:function(){return null==this.dragManager&&(this.dragManager=new i(this)),this.dragManager},recalculateOffsets:function(a){this.getDragManager().updateOffsets(a)},createElement:function(a,b,c,d){return this.createElementNS(null,a,b,c,d)},createElementNS:function(a,b,c,d,e){var f,g=null==a?document.createElement(b):document.createElementNS(a,b);c=c||{};for(f in c)g.style[f]=c[f];d&&(g.className=d),e=e||{};for(f in e)g.setAttribute(f,\"\"+e[f]);return g},getAttribute:function(a,b){return null!=a.getAttribute?a.getAttribute(b):null},setAttribute:function(a,b,c){null!=a.setAttribute&&a.setAttribute(b,c)},setAttributes:function(a,b){for(var c in b)b.hasOwnProperty(c)&&a.setAttribute(c,b[c])},appendToRoot:function(a){document.body.appendChild(a)},getRenderModes:function(){return[\"svg\"]},getClass:l,addClass:function(a,b){jsPlumb.each(a,function(a){m(a,b)})},hasClass:function(a,b){return a=jsPlumb.getElement(a),a.classList?a.classList.contains(b):-1!=l(a).indexOf(b)},removeClass:function(a,b){jsPlumb.each(a,function(a){m(a,null,b)})},updateClasses:function(a,b,c){jsPlumb.each(a,function(a){m(a,b,c)})},setClass:function(a,b){jsPlumb.each(a,function(a){k(a,b)})},setPosition:function(a,b){a.style.left=b.left+\"px\",a.style.top=b.top+\"px\"},getPosition:function(a){var b=function(b){var c=a.style[b];return c?c.substring(0,c.length-2):0};return{left:b(\"left\"),top:b(\"top\")}},getStyle:function(a,b){return\"undefined\"!=typeof window.getComputedStyle?getComputedStyle(a,null).getPropertyValue(b):a.currentStyle[b]},getSelector:function(a,b){var c=null;return c=1==arguments.length?null!=a.nodeType?a:document.querySelectorAll(a):a.querySelectorAll(b)},getOffset:function(a,b,c){a=jsPlumb.getElement(a),c=c||this.getContainer();for(var d={left:a.offsetLeft,top:a.offsetTop},e=b||null!=c&&a!=c&&a.offsetParent!=c?a.offsetParent:null,f=function(a){null!=a&&a!==document.body&&(a.scrollTop>0||a.scrollLeft>0)&&(d.left-=a.scrollLeft,d.top-=a.scrollTop)}.bind(this);null!=e;)d.left+=e.offsetLeft,d.top+=e.offsetTop,f(e),e=b?e.offsetParent:e.offsetParent==c?null:e.offsetParent;if(null!=c&&!b&&(c.scrollTop>0||c.scrollLeft>0)){var g=null!=a.offsetParent?this.getStyle(a.offsetParent,\"position\"):\"static\",h=this.getStyle(a,\"position\");\"absolute\"!==h&&\"fixed\"!==h&&\"absolute\"!==g&&\"fixed\"!=g&&(d.left-=c.scrollLeft,d.top-=c.scrollTop)}return d},getPositionOnElement:function(a,b,c){var d=\"undefined\"!=typeof b.getBoundingClientRect?b.getBoundingClientRect():{left:0,top:0,width:0,height:0},e=document.body,f=document.documentElement,g=window.pageYOffset||f.scrollTop||e.scrollTop,h=window.pageXOffset||f.scrollLeft||e.scrollLeft,i=f.clientTop||e.clientTop||0,j=f.clientLeft||e.clientLeft||0,k=0,l=0,m=d.top+g-i+k*c,n=d.left+h-j+l*c,o=jsPlumb.pageLocation(a),p=d.width||b.offsetWidth*c,q=d.height||b.offsetHeight*c,r=(o[0]-n)/p,s=(o[1]-m)/q;return[r,s]},getAbsolutePosition:function(a){var b=function(b){var c=a.style[b];return c?parseFloat(c.substring(0,c.length-2)):void 0};return[b(\"left\"),b(\"top\")]},setAbsolutePosition:function(a,b,c,d){c?this.animate(a,{left:\"+=\"+(b[0]-c[0]),top:\"+=\"+(b[1]-c[1])},d):(a.style.left=b[0]+\"px\",a.style.top=b[1]+\"px\")},getSize:function(a){return[a.offsetWidth,a.offsetHeight]},getWidth:function(a){return a.offsetWidth},getHeight:function(a){return a.offsetHeight}})}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d=\"__label\",e=function(a,c){var e={cssClass:c.cssClass,labelStyle:a.labelStyle,id:d,component:a,_jsPlumb:a._jsPlumb.instance},f=b.extend(e,c);return new(b.Overlays[a._jsPlumb.instance.getRenderMode()].Label)(f)},f=function(a,d){var e=null;if(c.isArray(d)){var f=d[0],g=b.extend({component:a,_jsPlumb:a._jsPlumb.instance},d[1]);3==d.length&&b.extend(g,d[2]),e=new(b.Overlays[a._jsPlumb.instance.getRenderMode()][f])(g)}else e=d.constructor==String?new(b.Overlays[a._jsPlumb.instance.getRenderMode()][d])({component:a,_jsPlumb:a._jsPlumb.instance}):d;return e.id=e.id||c.uuid(),a.cacheTypeItem(\"overlay\",e,e.id),a._jsPlumb.overlays[e.id]=e,e};b.OverlayCapableJsPlumbUIComponent=function(b){a.jsPlumbUIComponent.apply(this,arguments),this._jsPlumb.overlays={},this._jsPlumb.overlayPositions={},b.label&&(this.getDefaultType().overlays[d]=[\"Label\",{label:b.label,location:b.labelLocation||this.defaultLabelLocation||.5,labelStyle:b.labelStyle||this._jsPlumb.instance.Defaults.LabelStyle,id:d}]),this.setListenerComponent=function(a){if(this._jsPlumb)for(var b in this._jsPlumb.overlays)this._jsPlumb.overlays[b].setListenerComponent(a)}},b.OverlayCapableJsPlumbUIComponent.applyType=function(a,b){if(b.overlays){var c,d={};for(c in b.overlays){var e=a._jsPlumb.overlays[b.overlays[c][1].id];if(e)e.updateFrom(b.overlays[c][1]),d[b.overlays[c][1].id]=!0;else{var f=a.getCachedTypeItem(\"overlay\",b.overlays[c][1].id);null!=f?(f.reattach(a._jsPlumb.instance),f.setVisible(!0),f.updateFrom(b.overlays[c][1]),a._jsPlumb.overlays[f.id]=f):f=a.addOverlay(b.overlays[c],!0),d[f.id]=!0}}for(c in a._jsPlumb.overlays)null==d[a._jsPlumb.overlays[c].id]&&a.removeOverlay(a._jsPlumb.overlays[c].id,!0)}},c.extend(b.OverlayCapableJsPlumbUIComponent,a.jsPlumbUIComponent,{setHover:function(a){if(this._jsPlumb&&!this._jsPlumb.instance.isConnectionBeingDragged())for(var b in this._jsPlumb.overlays)this._jsPlumb.overlays[b][a?\"addClass\":\"removeClass\"](this._jsPlumb.instance.hoverClass)},addOverlay:function(a,b){var c=f(this,a);return b||this.repaint(),c},getOverlay:function(a){return this._jsPlumb.overlays[a]},getOverlays:function(){return this._jsPlumb.overlays},hideOverlay:function(a){var b=this.getOverlay(a);b&&b.hide()},hideOverlays:function(){for(var a in this._jsPlumb.overlays)this._jsPlumb.overlays[a].hide()},showOverlay:function(a){var b=this.getOverlay(a);b&&b.show()},showOverlays:function(){for(var a in this._jsPlumb.overlays)this._jsPlumb.overlays[a].show()},removeAllOverlays:function(a){for(var b in this._jsPlumb.overlays)this._jsPlumb.overlays[b].cleanup&&this._jsPlumb.overlays[b].cleanup();this._jsPlumb.overlays={},this._jsPlumb.overlayPositions=null,a||this.repaint()},removeOverlay:function(a,b){var c=this._jsPlumb.overlays[a];c&&(c.setVisible(!1),!b&&c.cleanup&&c.cleanup(),delete this._jsPlumb.overlays[a],this._jsPlumb.overlayPositions&&delete this._jsPlumb.overlayPositions[a])},removeOverlays:function(){for(var a=0,b=arguments.length;b>a;a++)this.removeOverlay(arguments[a])},moveParent:function(a){if(this.bgCanvas&&(this.bgCanvas.parentNode.removeChild(this.bgCanvas),a.appendChild(this.bgCanvas)),this.canvas&&this.canvas.parentNode){this.canvas.parentNode.removeChild(this.canvas),a.appendChild(this.canvas);for(var b in this._jsPlumb.overlays)if(this._jsPlumb.overlays[b].isAppendedAtTopLevel){var c=this._jsPlumb.overlays[b].getElement();c.parentNode.removeChild(c),a.appendChild(c)}}},getLabel:function(){var a=this.getOverlay(d);return null!=a?a.getLabel():null},getLabelOverlay:function(){return this.getOverlay(d)},setLabel:function(a){var b=this.getOverlay(d);if(b)a.constructor==String||a.constructor==Function?b.setLabel(a):(a.label&&b.setLabel(a.label),a.location&&b.setLocation(a.location));else{var c=a.constructor==String||a.constructor==Function?{label:a}:a;b=e(this,c),this._jsPlumb.overlays[d]=b}this._jsPlumb.instance.isSuspendDrawing()||this.repaint()},cleanup:function(a){for(var b in this._jsPlumb.overlays)this._jsPlumb.overlays[b].cleanup(a),this._jsPlumb.overlays[b].destroy(a);a&&(this._jsPlumb.overlays={},this._jsPlumb.overlayPositions=null)},setVisible:function(a){this[a?\"showOverlays\":\"hideOverlays\"]()},setAbsoluteOverlayPosition:function(a,b){this._jsPlumb.overlayPositions[a.id]=b},getAbsoluteOverlayPosition:function(a){return this._jsPlumb.overlayPositions?this._jsPlumb.overlayPositions[a.id]:null},_clazzManip:function(a,b,c){if(!c)for(var d in this._jsPlumb.overlays)this._jsPlumb.overlays[d][a+\"Class\"](b)},addClass:function(a,b){this._clazzManip(\"add\",a,b)},removeClass:function(a,b){this._clazzManip(\"remove\",a,b)}})}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d=function(a,b,c){var d=!1;return{drag:function(){if(d)return d=!1,!0;if(b.element){var e=c.getUIPosition(arguments,c.getZoom());null!=e&&jsPlumb.setPosition(b.element,e),c.repaint(b.element,e),a.paint({anchorPoint:a.anchor.getCurrentLocation({element:a})})}},stopDrag:function(){d=!0}}},e=function(a,b,c,d){var e=jsPlumb.createElement(\"div\",{position:\"absolute\"});b.appendElement(e);var f=b.getId(e);jsPlumb.setPosition(e,c),e.style.width=d[0]+\"px\",e.style.height=d[1]+\"px\",b.manage(f,e,!0),a.id=f,a.element=e},f=function(a,c,d,e,f,g,h,i){var j=new b.FloatingAnchor({reference:c,referenceCanvas:e,jsPlumbInstance:g});return h({paintStyle:a,endpoint:d,anchor:j,source:f,scope:i})},g=[\"connectorStyle\",\"connectorHoverStyle\",\"connectorOverlays\",\"connector\",\"connectionType\",\"connectorClass\",\"connectorHoverClass\"],h=function(a,b){var c=0;if(null!=b)for(var d=0;d<a.connections.length;d++)if(a.connections[d].sourceId==b||a.connections[d].targetId==b){c=d;break}return a.connections[c]};b.Endpoint=function(a){var i=a._jsPlumb,j=a.newConnection,k=a.newEndpoint;this.idPrefix=\"_jsplumb_e_\",this.defaultLabelLocation=[.5,.5],this.defaultOverlayKeys=[\"Overlays\",\"EndpointOverlays\"],b.OverlayCapableJsPlumbUIComponent.apply(this,arguments),this.appendToDefaultType({connectionType:a.connectionType,maxConnections:null==a.maxConnections?this._jsPlumb.instance.Defaults.MaxConnections:a.maxConnections,paintStyle:a.endpointStyle||a.paintStyle||a.style||this._jsPlumb.instance.Defaults.EndpointStyle||b.Defaults.EndpointStyle,hoverPaintStyle:a.endpointHoverStyle||a.hoverPaintStyle||this._jsPlumb.instance.Defaults.EndpointHoverStyle||b.Defaults.EndpointHoverStyle,connectorStyle:a.connectorStyle,connectorHoverStyle:a.connectorHoverStyle,connectorClass:a.connectorClass,connectorHoverClass:a.connectorHoverClass,connectorOverlays:a.connectorOverlays,connector:a.connector,connectorTooltip:a.connectorTooltip}),this._jsPlumb.enabled=!(a.enabled===!1),this._jsPlumb.visible=!0,this.element=b.getElement(a.source),this._jsPlumb.uuid=a.uuid,this._jsPlumb.floatingEndpoint=null;var l=null;this._jsPlumb.uuid&&(a.endpointsByUUID[this._jsPlumb.uuid]=this),this.elementId=a.elementId,this.dragProxy=a.dragProxy,this._jsPlumb.connectionCost=a.connectionCost,this._jsPlumb.connectionsDirected=a.connectionsDirected,this._jsPlumb.currentAnchorClass=\"\",this._jsPlumb.events={};var m=function(){var a=i.endpointAnchorClassPrefix+\"-\"+this._jsPlumb.currentAnchorClass;this._jsPlumb.currentAnchorClass=this.anchor.getCssClass();var b=i.endpointAnchorClassPrefix+(this._jsPlumb.currentAnchorClass?\"-\"+this._jsPlumb.currentAnchorClass:\"\");this.removeClass(a),this.addClass(b),jsPlumb.updateClasses(this.element,b,a)}.bind(this);this.prepareAnchor=function(a){var b=this._jsPlumb.instance.makeAnchor(a,this.elementId,i);return b.bind(\"anchorChanged\",function(a){this.fire(\"anchorChanged\",{endpoint:this,anchor:a}),m()}.bind(this)),b},this.setPreparedAnchor=function(a,b){return this._jsPlumb.instance.continuousAnchorFactory.clear(this.elementId),this.anchor=a,m(),b||this._jsPlumb.instance.repaint(this.elementId),this},this.setAnchor=function(a,b){var c=this.prepareAnchor(a);return this.setPreparedAnchor(c,b),this};var n=function(a){if(this.connections.length>0)for(var b=0;b<this.connections.length;b++)this.connections[b].setHover(a,!1);else this.setHover(a)}.bind(this);this.bind(\"mouseover\",function(){n(!0)}),this.bind(\"mouseout\",function(){n(!1)}),a._transient||this._jsPlumb.instance.anchorManager.add(this,this.elementId),this.prepareEndpoint=function(d,e){var f,g=function(a,c){var d=i.getRenderMode();if(b.Endpoints[d][a])return new b.Endpoints[d][a](c);if(!i.Defaults.DoNotThrowErrors)throw{msg:\"jsPlumb: unknown endpoint type '\"+a+\"'\"}},h={_jsPlumb:this._jsPlumb.instance,cssClass:a.cssClass,container:a.container,tooltip:a.tooltip,connectorTooltip:a.connectorTooltip,endpoint:this};return c.isString(d)?f=g(d,h):c.isArray(d)?(h=c.merge(d[1],h),f=g(d[0],h)):f=d.clone(),f.clone=function(){return c.isString(d)?g(d,h):c.isArray(d)?(h=c.merge(d[1],h),g(d[0],h)):void 0}.bind(this),f.typeId=e,f},this.setEndpoint=function(a){var b=this.prepareEndpoint(a);this.setPreparedEndpoint(b,!0)},this.setPreparedEndpoint=function(a){null!=this.endpoint&&(this.endpoint.cleanup(),this.endpoint.destroy()),this.endpoint=a,this.type=this.endpoint.type,this.canvas=this.endpoint.canvas\n},b.extend(this,a,g),this.isSource=a.isSource||!1,this.isTemporarySource=a.isTemporarySource||!1,this.isTarget=a.isTarget||!1,this.connections=a.connections||[],this.connectorPointerEvents=a[\"connector-pointer-events\"],this.scope=a.scope||i.getDefaultScope(),this.timestamp=null,this.reattachConnections=a.reattach||i.Defaults.ReattachConnections,this.connectionsDetachable=i.Defaults.ConnectionsDetachable,(a.connectionsDetachable===!1||a.detachable===!1)&&(this.connectionsDetachable=!1),this.dragAllowedWhenFull=a.dragAllowedWhenFull!==!1,a.onMaxConnections&&this.bind(\"maxConnections\",a.onMaxConnections),this.addConnection=function(a){this.connections.push(a),this[(this.connections.length>0?\"add\":\"remove\")+\"Class\"](i.endpointConnectedClass),this[(this.isFull()?\"add\":\"remove\")+\"Class\"](i.endpointFullClass)},this.detachFromConnection=function(a,b,c){b=null==b?this.connections.indexOf(a):b,b>=0&&(this.connections.splice(b,1),this[(this.connections.length>0?\"add\":\"remove\")+\"Class\"](i.endpointConnectedClass),this[(this.isFull()?\"add\":\"remove\")+\"Class\"](i.endpointFullClass)),(this._forceDeleteOnDetach||!c&&this._deleteOnDetach)&&0===this.connections.length&&i.deleteObject({endpoint:this,fireEvent:!1,deleteAttachedObjects:c!==!0})},this.detach=function(a){var b=a.connectionIndex,c=a.connection,d=a.ignoreTarget,e=a.fireEvent,f=a.originalEvent,g=a.endpointBeingDeleted,h=a.forceDetach,j=null==b?this.connections.indexOf(c):b,k=!1;return e=e!==!1,j>=0&&(h||c._forceDetach||c.isDetachable()&&c.isDetachAllowed(c)&&this.isDetachAllowed(c)&&i.checkCondition(\"beforeDetach\",c,g))&&(i.deleteObject({connection:c,fireEvent:!d&&e,originalEvent:f,deleteAttachedObjects:a.deleteAttachedObjects}),k=!0),k},this.detachAll=function(a,b){for(var c=[];this.connections.length>0;){var d=this.detach({connection:this.connections[0],ignoreTarget:!1,forceDetach:b===!0,fireEvent:a!==!1,originalEvent:null,endpointBeingDeleted:this,connectionIndex:0});d||(c.push(this.connections[0]),this.connections.splice(0,1))}return this.connections=c,this},this.detachFrom=function(a,b,c){for(var d=[],e=0;e<this.connections.length;e++)(this.connections[e].endpoints[1]==a||this.connections[e].endpoints[0]==a)&&d.push(this.connections[e]);for(var f=0;f<d.length;f++)this.detach({connection:d[f],ignoreTarget:!1,forceDetach:!0,fireEvent:b,originalEvent:c});return this},this.getElement=function(){return this.element},this.setElement=function(b){var d=this._jsPlumb.instance.getId(b),e=this.elementId;return c.removeWithFunction(a.endpointsByElement[this.elementId],function(a){return a.id==this.id}.bind(this)),this.element=jsPlumb.getElement(b),this.elementId=i.getId(this.element),i.anchorManager.rehomeEndpoint(this,e,this.element),i.dragManager.endpointAdded(this.element),c.addToList(a.endpointsByElement,d,this),this},this.makeInPlaceCopy=function(){var b=this.anchor.getCurrentLocation({element:this}),c=this.anchor.getOrientation(this),d=this.anchor.getCssClass(),e={bind:function(){},compute:function(){return[b[0],b[1]]},getCurrentLocation:function(){return[b[0],b[1]]},getOrientation:function(){return c},getCssClass:function(){return d}};return k({dropOptions:a.dropOptions,anchor:e,source:this.element,paintStyle:this.getPaintStyle(),endpoint:a.hideOnDrag?\"Blank\":this.endpoint,_transient:!0,scope:this.scope,reference:this})},this.connectorSelector=function(){var a=this.connections[0];return a?a:this.connections.length<this._jsPlumb.maxConnections||-1==this._jsPlumb.maxConnections?null:a},this.setStyle=this.setPaintStyle,this.paint=function(a){a=a||{};var b=a.timestamp,c=!(a.recalc===!1);if(!b||this.timestamp!==b){var d=i.updateOffset({elId:this.elementId,timestamp:b}),e=a.offset?a.offset.o:d.o;if(null!=e){var f=a.anchorPoint,g=a.connectorPaintStyle;if(null==f){var j=a.dimensions||d.s,k={xy:[e.left,e.top],wh:j,element:this,timestamp:b};if(c&&this.anchor.isDynamic&&this.connections.length>0){var l=h(this,a.elementWithPrecedence),m=l.endpoints[0]==this?1:0,n=0===m?l.sourceId:l.targetId,o=i.getCachedData(n),p=o.o,q=o.s;k.txy=[p.left,p.top],k.twh=q,k.tElement=l.endpoints[m]}f=this.anchor.compute(k)}this.endpoint.compute(f,this.anchor.getOrientation(this),this._jsPlumb.paintStyleInUse,g||this.paintStyleInUse),this.endpoint.paint(this._jsPlumb.paintStyleInUse,this.anchor),this.timestamp=b;for(var r in this._jsPlumb.overlays)if(this._jsPlumb.overlays.hasOwnProperty(r)){var s=this._jsPlumb.overlays[r];s.isVisible()&&(this._jsPlumb.overlayPlacements[r]=s.draw(this.endpoint,this._jsPlumb.paintStyleInUse),s.paint(this._jsPlumb.overlayPlacements[r]))}}}},this.getTypeDescriptor=function(){return\"endpoint\"},this.isVisible=function(){return this._jsPlumb.visible},this.repaint=this.paint;var o=!1;this.initDraggable=function(){if(!o&&b.isDragSupported(this.element)){var g,h={id:null,element:null},m=null,n=!1,p=null,q=d(this,h,i),r=a.dragOptions||{},s={},t=b.dragEvents.start,u=b.dragEvents.stop,v=b.dragEvents.drag,w=b.dragEvents.beforeStart,x=function(a){g=a.e.payload||{}},y=function(){m=this.connectorSelector();var b=!0;this.isEnabled()||(b=!1),null!=m||this.isSource||this.isTemporarySource||(b=!1),!this.isSource||!this.isFull()||null!=m&&this.dragAllowedWhenFull||(b=!1),null==m||m.isDetachable(this)||(b=!1);var d=i.checkCondition(null==m?\"beforeDrag\":\"beforeStartDetach\",{endpoint:this,source:this.element,sourceId:this.elementId,connection:m});if(d===!1?b=!1:\"object\"==typeof d?jsPlumb.extend(d,g||{}):d=g||{},b===!1)return i.stopDrag&&i.stopDrag(this.canvas),q.stopDrag(),!1;for(var l=0;l<this.connections.length;l++)this.connections[l].setHover(!1);this.addClass(\"endpointDrag\"),i.setConnectionBeingDragged(!0),m&&!this.isFull()&&this.isSource&&(m=null),i.updateOffset({elId:this.elementId});var o=this._jsPlumb.instance.getOffset(this.canvas),r=this.canvas,s=this._jsPlumb.instance.getSize(this.canvas);e(h,i,o,s),i.setAttributes(this.canvas,{dragId:h.id,elId:this.elementId});var t=this.dragProxy||this.endpoint;if(null==this.dragProxy&&null!=this.connectionType){var u=this._jsPlumb.instance.deriveEndpointAndAnchorSpec(this.connectionType);u.endpoints[1]&&(t=u.endpoints[1])}var v=this._jsPlumb.instance.makeAnchor(\"Center\");v.isFloating=!0,this._jsPlumb.floatingEndpoint=f(this.getPaintStyle(),v,t,this.canvas,h.element,i,k,this.scope);var w=this._jsPlumb.floatingEndpoint.anchor;if(null==m)this.setHover(!1,!1),m=j({sourceEndpoint:this,targetEndpoint:this._jsPlumb.floatingEndpoint,source:this.element,target:h.element,anchors:[this.anchor,this._jsPlumb.floatingEndpoint.anchor],paintStyle:a.connectorStyle,hoverPaintStyle:a.connectorHoverStyle,connector:a.connector,overlays:a.connectorOverlays,type:this.connectionType,cssClass:this.connectorClass,hoverClass:this.connectorHoverClass,scope:a.scope,data:d}),m.pending=!0,m.addClass(i.draggingClass),this._jsPlumb.floatingEndpoint.addClass(i.draggingClass),this._jsPlumb.floatingEndpoint.anchor=w,i.fire(\"connectionDrag\",m),i.anchorManager.newConnection(m);else{n=!0,m.setHover(!1);var x=m.endpoints[0].id==this.id?0:1;this.detachFromConnection(m,null,!0);var y=i.getDragScope(r);i.setAttribute(this.canvas,\"originalScope\",y),i.fire(\"connectionDrag\",m),0===x?(p=[m.source,m.sourceId,r,y],i.anchorManager.sourceChanged(m.endpoints[x].elementId,h.id,m,h.element)):(p=[m.target,m.targetId,r,y],m.target=h.element,m.targetId=h.id,i.anchorManager.updateOtherEndpoint(m.sourceId,m.endpoints[x].elementId,m.targetId,m)),m.suspendedEndpoint=m.endpoints[x],m.suspendedElement=m.endpoints[x].getElement(),m.suspendedElementId=m.endpoints[x].elementId,m.suspendedElementType=0===x?\"source\":\"target\",m.suspendedEndpoint.setHover(!1),this._jsPlumb.floatingEndpoint.referenceEndpoint=m.suspendedEndpoint,m.endpoints[x]=this._jsPlumb.floatingEndpoint,m.addClass(i.draggingClass),this._jsPlumb.floatingEndpoint.addClass(i.draggingClass)}i.floatingConnections[h.id]=m,c.addToList(a.endpointsByElement,h.id,this._jsPlumb.floatingEndpoint),i.currentlyDragging=!0}.bind(this),z=function(){if(i.setConnectionBeingDragged(!1),m&&null!=m.endpoints){var a=i.getDropEvent(arguments),b=i.getFloatingAnchorIndex(m);if(m.endpoints[0===b?1:0].anchor.locked=!1,m.removeClass(i.draggingClass),this._jsPlumb&&(m.deleteConnectionNow||m.endpoints[b]==this._jsPlumb.floatingEndpoint)&&n&&m.suspendedEndpoint){0===b?(m.floatingElement=m.source,m.floatingId=m.sourceId,m.floatingEndpoint=m.endpoints[0],m.floatingIndex=0,m.source=p[0],m.sourceId=p[1]):(m.floatingElement=m.target,m.floatingId=m.targetId,m.floatingEndpoint=m.endpoints[1],m.floatingIndex=1,m.target=p[0],m.targetId=p[1]);var c=this._jsPlumb.floatingEndpoint;i.setDragScope(p[2],p[3]),m.endpoints[b]=m.suspendedEndpoint,m.isReattach()||m._forceReattach||m._forceDetach||!m.endpoints[0===b?1:0].detach({connection:m,ignoreTarget:!1,forceDetach:!1,fireEvent:!0,originalEvent:a,endpointBeingDeleted:!0})?(m.setHover(!1),m._forceDetach=null,m._forceReattach=null,this._jsPlumb.floatingEndpoint.detachFromConnection(m),m.suspendedEndpoint.addConnection(m),1==b?i.anchorManager.updateOtherEndpoint(m.sourceId,m.floatingId,m.targetId,m):i.anchorManager.sourceChanged(m.floatingId,m.sourceId,m,m.source),i.repaint(p[1])):i.deleteObject({endpoint:c})}this.deleteAfterDragStop?i.deleteObject({endpoint:this}):this._jsPlumb&&this.paint({recalc:!1}),i.fire(\"connectionDragStop\",m,a),m.pending&&i.fire(\"connectionAborted\",m,a),i.currentlyDragging=!1,m.suspendedElement=null,m.suspendedEndpoint=null,m=null}h&&h.element&&i.remove(h.element,!1,!1),l&&i.deleteObject({endpoint:l}),this._jsPlumb&&(this.canvas.style.visibility=\"visible\",this.anchor.locked=!1,this._jsPlumb.floatingEndpoint=null)}.bind(this);r=b.extend(s,r),r.scope=this.scope||r.scope,r[w]=c.wrap(r[w],x,!1),r[t]=c.wrap(r[t],y,!1),r[v]=c.wrap(r[v],q.drag),r[u]=c.wrap(r[u],z),r.multipleDrop=!1,r.canDrag=function(){return this.isSource||this.isTemporarySource||this.connections.length>0}.bind(this),i.initDraggable(this.canvas,r,\"internal\"),this.canvas._jsPlumbRelatedElement=this.element,o=!0}};var p=a.endpoint||this._jsPlumb.instance.Defaults.Endpoint||b.Defaults.Endpoint;this.setEndpoint(p,!0);var q=a.anchor?a.anchor:a.anchors?a.anchors:i.Defaults.Anchor||\"Top\";this.setAnchor(q,!0);var r=[\"default\",a.type||\"\"].join(\" \");this.addType(r,a.data,!0),this.canvas=this.endpoint.canvas,this.canvas._jsPlumb=this,this.initDraggable();var s=function(d,e,f,g){if(b.isDropSupported(this.element)){var h=a.dropOptions||i.Defaults.DropOptions||b.Defaults.DropOptions;h=b.extend({},h),h.scope=h.scope||this.scope;var j=b.dragEvents.drop,k=b.dragEvents.over,l=b.dragEvents.out,m=this,n=i.EndpointDropHandler({getEndpoint:function(){return m},jsPlumb:i,enabled:function(){return null!=f?f.isEnabled():!0},isFull:function(){return f.isFull()},element:this.element,elementId:this.elementId,isSource:this.isSource,isTarget:this.isTarget,addClass:function(a){m.addClass(a)},removeClass:function(a){m.removeClass(a)},isDropAllowed:function(){return m.isDropAllowed.apply(m,arguments)},reference:g,isRedrop:function(a,b){return a.suspendedEndpoint&&b.reference&&a.suspendedEndpoint.id===b.reference.id}});h[j]=c.wrap(h[j],n,!0),h[k]=c.wrap(h[k],function(){var a=b.getDragObject(arguments),c=i.getAttribute(b.getElement(a),\"dragId\"),d=i.floatingConnections[c];if(null!=d){var e=i.getFloatingAnchorIndex(d),f=this.isTarget&&0!==e||d.suspendedEndpoint&&this.referenceEndpoint&&this.referenceEndpoint.id==d.suspendedEndpoint.id;if(f){var g=i.checkCondition(\"checkDropAllowed\",{sourceEndpoint:d.endpoints[e],targetEndpoint:this,connection:d});this[(g?\"add\":\"remove\")+\"Class\"](i.endpointDropAllowedClass),this[(g?\"remove\":\"add\")+\"Class\"](i.endpointDropForbiddenClass),d.endpoints[e].anchor.over(this.anchor,this)}}}.bind(this)),h[l]=c.wrap(h[l],function(){var a=b.getDragObject(arguments),c=null==a?null:i.getAttribute(b.getElement(a),\"dragId\"),d=c?i.floatingConnections[c]:null;if(null!=d){var e=i.getFloatingAnchorIndex(d),f=this.isTarget&&0!==e||d.suspendedEndpoint&&this.referenceEndpoint&&this.referenceEndpoint.id==d.suspendedEndpoint.id;f&&(this.removeClass(i.endpointDropAllowedClass),this.removeClass(i.endpointDropForbiddenClass),d.endpoints[e].anchor.out())}}.bind(this)),i.initDroppable(d,h,\"internal\",e)}}.bind(this);return this.anchor.isFloating||s(this.canvas,!(a._transient||this.anchor.isFloating),this,a.reference),this},c.extend(b.Endpoint,b.OverlayCapableJsPlumbUIComponent,{setVisible:function(a,b,c){if(this._jsPlumb.visible=a,this.canvas&&(this.canvas.style.display=a?\"block\":\"none\"),this[a?\"showOverlays\":\"hideOverlays\"](),!b)for(var d=0;d<this.connections.length;d++)if(this.connections[d].setVisible(a),!c){var e=this===this.connections[d].endpoints[0]?1:0;1==this.connections[d].endpoints[e].connections.length&&this.connections[d].endpoints[e].setVisible(a,!0,!0)}},getAttachedElements:function(){return this.connections},applyType:function(a,c){this.setPaintStyle(a.endpointStyle||a.paintStyle,c),this.setHoverPaintStyle(a.endpointHoverStyle||a.hoverPaintStyle,c),null!=a.maxConnections&&(this._jsPlumb.maxConnections=a.maxConnections),a.scope&&(this.scope=a.scope),b.extend(this,a,g),null!=a.cssClass&&this.canvas&&this._jsPlumb.instance.addClass(this.canvas,a.cssClass),b.OverlayCapableJsPlumbUIComponent.applyType(this,a)},isEnabled:function(){return this._jsPlumb.enabled},setEnabled:function(a){this._jsPlumb.enabled=a},cleanup:function(){var a=this._jsPlumb.instance.endpointAnchorClassPrefix+(this._jsPlumb.currentAnchorClass?\"-\"+this._jsPlumb.currentAnchorClass:\"\");jsPlumb.removeClass(this.element,a),this.anchor=null,this.endpoint.cleanup(!0),this.endpoint.destroy(),this.endpoint=null,this._jsPlumb.instance.destroyDraggable(this.canvas,\"internal\"),this._jsPlumb.instance.destroyDroppable(this.canvas,\"internal\")},setHover:function(a){this.endpoint&&this._jsPlumb&&!this._jsPlumb.instance.isConnectionBeingDragged()&&this.endpoint.setHover(a)},isFull:function(){return 0===this._jsPlumb.maxConnections?!0:!(this.isFloating()||this._jsPlumb.maxConnections<0||this.connections.length<this._jsPlumb.maxConnections)},isFloating:function(){return null!=this.anchor&&this.anchor.isFloating},isConnectedTo:function(a){var b=!1;if(a)for(var c=0;c<this.connections.length;c++)if(this.connections[c].endpoints[1]==a||this.connections[c].endpoints[0]==a){b=!0;break}return b},getConnectionCost:function(){return this._jsPlumb.connectionCost},setConnectionCost:function(a){this._jsPlumb.connectionCost=a},areConnectionsDirected:function(){return this._jsPlumb.connectionsDirected},setConnectionsDirected:function(a){this._jsPlumb.connectionsDirected=a},setElementId:function(a){this.elementId=a,this.anchor.elementId=a},setReferenceElement:function(a){this.element=b.getElement(a)},setDragAllowedWhenFull:function(a){this.dragAllowedWhenFull=a},equals:function(a){return this.anchor.equals(a.anchor)},getUuid:function(){return this._jsPlumb.uuid},computeAnchor:function(a){return this.anchor.compute(a)}}),a.jsPlumbInstance.prototype.EndpointDropHandler=function(a){return function(b){var d=a.jsPlumb;a.removeClass(d.endpointDropAllowedClass),a.removeClass(d.endpointDropForbiddenClass);var e=d.getDropEvent(arguments),f=d.getDragObject(arguments),g=d.getAttribute(f,\"dragId\"),h=(d.getAttribute(f,\"elId\"),d.getAttribute(f,\"originalScope\")),i=d.floatingConnections[g];if(null!=i){var j=null!=i.suspendedEndpoint;if(!j||null!=i.suspendedEndpoint._jsPlumb){var k=a.getEndpoint(i);if(null!=k){if(a.isRedrop(i,a))return i._forceReattach=!0,i.setHover(!1),a.maybeCleanup&&a.maybeCleanup(k),void 0;var l=d.getFloatingAnchorIndex(i);if(0===l&&!a.isSource||1===l&&!a.isTarget)return a.maybeCleanup&&a.maybeCleanup(k),void 0;a.onDrop&&a.onDrop(i),h&&d.setDragScope(f,h);var m=a.isFull(b);if(m&&k.fire(\"maxConnections\",{endpoint:this,connection:i,maxConnections:k._jsPlumb.maxConnections},e),!m&&a.enabled()){var n=!0;0===l?(i.floatingElement=i.source,i.floatingId=i.sourceId,i.floatingEndpoint=i.endpoints[0],i.floatingIndex=0,i.source=a.element,i.sourceId=a.elementId):(i.floatingElement=i.target,i.floatingId=i.targetId,i.floatingEndpoint=i.endpoints[1],i.floatingIndex=1,i.target=a.element,i.targetId=a.elementId),j&&i.suspendedEndpoint.id!=k.id&&(i.isDetachAllowed(i)&&i.endpoints[l].isDetachAllowed(i)&&i.suspendedEndpoint.isDetachAllowed(i)&&d.checkCondition(\"beforeDetach\",i)||(n=!1));var o=function(a){i.endpoints[l].detachFromConnection(i),i.suspendedEndpoint&&i.suspendedEndpoint.detachFromConnection(i),i.endpoints[l]=k,k.addConnection(i);var b=k.getParameters();for(var f in b)i.setParameter(f,b[f]);if(j){var g=i.suspendedEndpoint.elementId;d.fireMoveEvent({index:l,originalSourceId:0===l?g:i.sourceId,newSourceId:0===l?k.elementId:i.sourceId,originalTargetId:1==l?g:i.targetId,newTargetId:1==l?k.elementId:i.targetId,originalSourceEndpoint:0===l?i.suspendedEndpoint:i.endpoints[0],newSourceEndpoint:0===l?k:i.endpoints[0],originalTargetEndpoint:1==l?i.suspendedEndpoint:i.endpoints[1],newTargetEndpoint:1==l?k:i.endpoints[1],connection:i},e)}else b.draggable&&d.initDraggable(this.element,dragOptions,\"internal\",d);if(1==l?d.anchorManager.updateOtherEndpoint(i.sourceId,i.floatingId,i.targetId,i):d.anchorManager.sourceChanged(i.floatingId,i.sourceId,i,i.source),i.endpoints[0].finalEndpoint){var h=i.endpoints[0];h.detachFromConnection(i),i.endpoints[0]=i.endpoints[0].finalEndpoint,i.endpoints[0].addConnection(i)}c.isObject(a)&&i.mergeData(a),d.finaliseConnection(i,null,e,!1),i.setHover(!1)}.bind(this),p=function(){i.suspendedEndpoint&&(i.endpoints[l]=i.suspendedEndpoint,i.setHover(!1),i._forceDetach=!0,0===l?(i.source=i.suspendedEndpoint.element,i.sourceId=i.suspendedEndpoint.elementId):(i.target=i.suspendedEndpoint.element,i.targetId=i.suspendedEndpoint.elementId),i.suspendedEndpoint.addConnection(i),1==l?d.anchorManager.updateOtherEndpoint(i.sourceId,i.floatingId,i.targetId,i):d.anchorManager.sourceChanged(i.floatingId,i.sourceId,i,i.source),d.repaint(i.sourceId),i._forceDetach=!1)};if(n=n&&a.isDropAllowed(i.sourceId,i.targetId,i.scope,i,k))return o(n),!0;p()}a.maybeCleanup&&a.maybeCleanup(k),d.currentlyDragging=!1}}}}}}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d=function(a,c,d,e,f){if(!a.Defaults.DoNotThrowErrors&&null==jsPlumb.Connectors[c][d])throw{msg:\"jsPlumb: unknown connector type '\"+d+\"'\"};return new b.Connectors[c][d](e,f)},e=function(a,b,c){return a?c.makeAnchor(a,b,c):null},f=function(a,b,d,e){null!=b&&(b._jsPlumbConnections=b._jsPlumbConnections||{},e?delete b._jsPlumbConnections[a.id]:b._jsPlumbConnections[a.id]=!0,c.isEmpty(b._jsPlumbConnections)?d.removeClass(b,d.connectedClass):d.addClass(b,d.connectedClass))};b.Connection=function(a){var d=a.newEndpoint;this.id=a.id,this.connector=null,this.idPrefix=\"_jsplumb_c_\",this.defaultLabelLocation=.5,this.defaultOverlayKeys=[\"Overlays\",\"ConnectionOverlays\"],this.previousConnection=a.previousConnection,this.source=b.getElement(a.source),this.target=b.getElement(a.target),a.sourceEndpoint&&(this.source=a.sourceEndpoint.getElement()),a.targetEndpoint&&(this.target=a.targetEndpoint.getElement()),b.OverlayCapableJsPlumbUIComponent.apply(this,arguments),this.sourceId=this._jsPlumb.instance.getId(this.source),this.targetId=this._jsPlumb.instance.getId(this.target),this.scope=a.scope,this.endpoints=[],this.endpointStyles=[];var e=this._jsPlumb.instance;e.manage(this.sourceId,this.source),e.manage(this.targetId,this.target),this._jsPlumb.visible=!0,this._jsPlumb.editable=a.editable===!0,this._jsPlumb.params={cssClass:a.cssClass,container:a.container,\"pointer-events\":a[\"pointer-events\"],editorParams:a.editorParams,overlays:a.overlays},this._jsPlumb.lastPaintedAt=null,this.bind(\"mouseover\",function(){this.setHover(!0)}.bind(this)),this.bind(\"mouseout\",function(){this.setHover(!1)}.bind(this)),this.editableRequested=a.editable!==!1,this.setEditable=function(a){return this.connector?this.connector.setEditable(a):!1},this.isEditable=function(){return this.connector?this.connector.isEditable():!1},this.isEditing=function(){return this.connector?this.connector.isEditing():!1},this.makeEndpoint=function(b,c,f,g){return f=f||this._jsPlumb.instance.getId(c),this.prepareEndpoint(e,d,this,g,b?0:1,a,c,f)},a.type&&(a.endpoints=this._jsPlumb.instance.deriveEndpointAndAnchorSpec(a.type).endpoints);var f=this.makeEndpoint(!0,this.source,this.sourceId,a.sourceEndpoint),g=this.makeEndpoint(!1,this.target,this.targetId,a.targetEndpoint);f&&c.addToList(a.endpointsByElement,this.sourceId,f),g&&c.addToList(a.endpointsByElement,this.targetId,g),this.scope||(this.scope=this.endpoints[0].scope),null!=a.deleteEndpointsOnDetach?(this.endpoints[0]._deleteOnDetach=a.deleteEndpointsOnDetach,this.endpoints[1]._deleteOnDetach=a.deleteEndpointsOnDetach):(this.endpoints[0]._doNotDeleteOnDetach||(this.endpoints[0]._deleteOnDetach=!0),this.endpoints[1]._doNotDeleteOnDetach||(this.endpoints[1]._deleteOnDetach=!0));var h=e.Defaults.ConnectionsDetachable;a.detachable===!1&&(h=!1),this.endpoints[0].connectionsDetachable===!1&&(h=!1),this.endpoints[1].connectionsDetachable===!1&&(h=!1);var i=a.reattach||this.endpoints[0].reattachConnections||this.endpoints[1].reattachConnections||e.Defaults.ReattachConnections;this.appendToDefaultType({detachable:h,reattach:i,paintStyle:this.endpoints[0].connectorStyle||this.endpoints[1].connectorStyle||a.paintStyle||e.Defaults.PaintStyle||jsPlumb.Defaults.PaintStyle,hoverPaintStyle:this.endpoints[0].connectorHoverStyle||this.endpoints[1].connectorHoverStyle||a.hoverPaintStyle||e.Defaults.HoverPaintStyle||jsPlumb.Defaults.HoverPaintStyle});var j=e.getSuspendedAt();if(!e.isSuspendDrawing()){var k=e.getCachedData(this.sourceId),l=k.o,m=k.s,n=e.getCachedData(this.targetId),o=n.o,p=n.s,q=j||e.timestamp(),r=this.endpoints[0].anchor.compute({xy:[l.left,l.top],wh:m,element:this.endpoints[0],elementId:this.endpoints[0].elementId,txy:[o.left,o.top],twh:p,tElement:this.endpoints[1],timestamp:q});this.endpoints[0].paint({anchorLoc:r,timestamp:q}),r=this.endpoints[1].anchor.compute({xy:[o.left,o.top],wh:p,element:this.endpoints[1],elementId:this.endpoints[1].elementId,txy:[l.left,l.top],twh:m,tElement:this.endpoints[0],timestamp:q}),this.endpoints[1].paint({anchorLoc:r,timestamp:q})}this.getTypeDescriptor=function(){return\"connection\"},this.getAttachedElements=function(){return this.endpoints},this.isDetachable=function(){return this._jsPlumb.detachable===!0},this.setDetachable=function(a){this._jsPlumb.detachable=a===!0},this.isReattach=function(){return this._jsPlumb.reattach===!0||this.endpoints[0].reattachConnections===!0||this.endpoints[1].reattachConnections===!0},this.setReattach=function(a){this._jsPlumb.reattach=a===!0},this._jsPlumb.cost=a.cost||this.endpoints[0].getConnectionCost(),this._jsPlumb.directed=a.directed,null==a.directed&&(this._jsPlumb.directed=this.endpoints[0].areConnectionsDirected());var s=jsPlumb.extend({},this.endpoints[1].getParameters());b.extend(s,this.endpoints[0].getParameters()),b.extend(s,this.getParameters()),this.setParameters(s),this.setConnector(this.endpoints[0].connector||this.endpoints[1].connector||a.connector||e.Defaults.Connector||b.Defaults.Connector,!0),a.geometry&&this.connector.setGeometry(a.geometry);var t=null!=a.data&&c.isObject(a.data)?a.data:{};this.getData=function(){return t},this.setData=function(a){t=a||{}},this.mergeData=function(a){t=jsPlumb.extend(t,a)};var u=[\"default\",this.endpoints[0].connectionType,this.endpoints[1].connectionType,a.type].join(\" \");/[^\\s]/.test(u)&&this.addType(u,a.data,!0),this.updateConnectedClass()},c.extend(b.Connection,b.OverlayCapableJsPlumbUIComponent,{applyType:function(a,c,d){null!=a.detachable&&this.setDetachable(a.detachable),null!=a.reattach&&this.setReattach(a.reattach),a.scope&&(this.scope=a.scope),null!=a.cssClass&&this.canvas&&this._jsPlumb.instance.addClass(this.canvas,a.cssClass);var e=null;a.anchor?(e=this.getCachedTypeItem(\"anchors\",d.anchor),null==e&&(e=[this._jsPlumb.instance.makeAnchor(a.anchor),this._jsPlumb.instance.makeAnchor(a.anchor)],this.cacheTypeItem(\"anchors\",e,d.anchor))):a.anchors&&(e=this.getCachedTypeItem(\"anchors\",d.anchors),null==e&&(e=[this._jsPlumb.instance.makeAnchor(a.anchors[0]),this._jsPlumb.instance.makeAnchor(a.anchors[1])],this.cacheTypeItem(\"anchors\",e,d.anchors))),null!=e&&(this.endpoints[0].anchor=e[0],this.endpoints[1].anchor=e[1],this.endpoints[1].anchor.isDynamic&&this._jsPlumb.instance.repaint(this.endpoints[1].elementId)),b.OverlayCapableJsPlumbUIComponent.applyType(this,a)},addClass:function(a,b){b&&(this.endpoints[0].addClass(a),this.endpoints[1].addClass(a),this.suspendedEndpoint&&this.suspendedEndpoint.addClass(a)),this.connector&&this.connector.addClass(a)},removeClass:function(a,b){b&&(this.endpoints[0].removeClass(a),this.endpoints[1].removeClass(a),this.suspendedEndpoint&&this.suspendedEndpoint.removeClass(a)),this.connector&&this.connector.removeClass(a)},isVisible:function(){return this._jsPlumb.visible},setVisible:function(a){this._jsPlumb.visible=a,this.connector&&this.connector.setVisible(a),this.repaint()},cleanup:function(){this.updateConnectedClass(!0),this.endpoints=null,this.source=null,this.target=null,null!=this.connector&&(this.connector.cleanup(!0),this.connector.destroy(!0)),this.connector=null},updateConnectedClass:function(a){this._jsPlumb&&(f(this,this.source,this._jsPlumb.instance,a),f(this,this.target,this._jsPlumb.instance,a))},setHover:function(b){this.connector&&this._jsPlumb&&!this._jsPlumb.instance.isConnectionBeingDragged()&&(this.connector.setHover(b),a.jsPlumb[b?\"addClass\":\"removeClass\"](this.source,this._jsPlumb.instance.hoverSourceClass),a.jsPlumb[b?\"addClass\":\"removeClass\"](this.target,this._jsPlumb.instance.hoverTargetClass))},getUuids:function(){return[this.endpoints[0].getUuid(),this.endpoints[1].getUuid()]},getCost:function(){return this._jsPlumb?this._jsPlumb.cost:-1/0},setCost:function(a){this._jsPlumb.cost=a},isDirected:function(){return this._jsPlumb.directed===!0},getConnector:function(){return this.connector},getGeometry:function(){return this.connector?this.connector.getGeometry():null},setGeometry:function(a){this.connector&&this.connector.setGeometry(a)},prepareConnector:function(a,b){var e,f={_jsPlumb:this._jsPlumb.instance,cssClass:(this._jsPlumb.params.cssClass||\"\")+(this.isEditable()?this._jsPlumb.instance.editableConnectorClass:\"\"),container:this._jsPlumb.params.container,\"pointer-events\":this._jsPlumb.params[\"pointer-events\"],editable:this.editableRequested},g=this._jsPlumb.instance.getRenderMode();return c.isString(a)?e=d(this._jsPlumb.instance,g,a,f,this):c.isArray(a)&&(e=1==a.length?d(this._jsPlumb.instance,g,a[0],f,this):d(this._jsPlumb.instance,g,a[0],c.merge(a[1],f),this)),null!=b&&(e.typeId=b),e},setPreparedConnector:function(a,b,c,d){var e,f=\"\";if(null!=this.connector&&(e=this.connector,f=e.getClass(),this.connector.cleanup(),this.connector.destroy()),this.connector=a,d&&this.cacheTypeItem(\"connector\",a,d),this.canvas=this.connector.canvas,this.bgCanvas=this.connector.bgCanvas,this.addClass(f),this.canvas&&(this.canvas._jsPlumb=this),this.bgCanvas&&(this.bgCanvas._jsPlumb=this),null!=e)for(var g=this.getOverlays(),h=0;h<g.length;h++)g[h].transfer&&g[h].transfer(this.connector);c||this.setListenerComponent(this.connector),b||this.repaint()},setConnector:function(a,b,c,d){var e=this.prepareConnector(a,d);this.setPreparedConnector(e,b,c,d)},paint:function(a){if(!this._jsPlumb.instance.isSuspendDrawing()&&this._jsPlumb.visible){a=a||{};var b=a.timestamp,c=!1,d=c?this.sourceId:this.targetId,e=c?this.targetId:this.sourceId,f=c?0:1,g=c?1:0;if(null==b||b!=this._jsPlumb.lastPaintedAt){var h=this._jsPlumb.instance.updateOffset({elId:e}).o,i=this._jsPlumb.instance.updateOffset({elId:d}).o,j=this.endpoints[g],k=this.endpoints[f],l=j.anchor.getCurrentLocation({xy:[h.left,h.top],wh:[h.width,h.height],element:j,timestamp:b}),m=k.anchor.getCurrentLocation({xy:[i.left,i.top],wh:[i.width,i.height],element:k,timestamp:b});this.connector.resetBounds(),this.connector.compute({sourcePos:l,targetPos:m,sourceEndpoint:this.endpoints[g],targetEndpoint:this.endpoints[f],lineWidth:this._jsPlumb.paintStyleInUse.lineWidth,sourceInfo:h,targetInfo:i});var n={minX:1/0,minY:1/0,maxX:-1/0,maxY:-1/0};for(var o in this._jsPlumb.overlays)if(this._jsPlumb.overlays.hasOwnProperty(o)){var p=this._jsPlumb.overlays[o];p.isVisible()&&(this._jsPlumb.overlayPlacements[o]=p.draw(this.connector,this._jsPlumb.paintStyleInUse,this.getAbsoluteOverlayPosition(p)),n.minX=Math.min(n.minX,this._jsPlumb.overlayPlacements[o].minX),n.maxX=Math.max(n.maxX,this._jsPlumb.overlayPlacements[o].maxX),n.minY=Math.min(n.minY,this._jsPlumb.overlayPlacements[o].minY),n.maxY=Math.max(n.maxY,this._jsPlumb.overlayPlacements[o].maxY))}var q=parseFloat(this._jsPlumb.paintStyleInUse.lineWidth||1)/2,r=parseFloat(this._jsPlumb.paintStyleInUse.lineWidth||0),s={xmin:Math.min(this.connector.bounds.minX-(q+r),n.minX),ymin:Math.min(this.connector.bounds.minY-(q+r),n.minY),xmax:Math.max(this.connector.bounds.maxX+(q+r),n.maxX),ymax:Math.max(this.connector.bounds.maxY+(q+r),n.maxY)};this.connector.paint(this._jsPlumb.paintStyleInUse,null,s);for(var t in this._jsPlumb.overlays)if(this._jsPlumb.overlays.hasOwnProperty(t)){var u=this._jsPlumb.overlays[t];u.isVisible()&&u.paint(this._jsPlumb.overlayPlacements[t],s)}}this._jsPlumb.lastPaintedAt=b}},repaint:function(a){a=a||{},this.paint({elId:this.sourceId,recalc:!(a.recalc===!1),timestamp:a.timestamp})},prepareEndpoint:function(a,c,d,f,g,h,i,j){var k;if(f)d.endpoints[g]=f,f.addConnection(d);else{h.endpoints||(h.endpoints=[null,null]);var l=h.endpoints[g]||h.endpoint||a.Defaults.Endpoints[g]||jsPlumb.Defaults.Endpoints[g]||a.Defaults.Endpoint||jsPlumb.Defaults.Endpoint;h.endpointStyles||(h.endpointStyles=[null,null]),h.endpointHoverStyles||(h.endpointHoverStyles=[null,null]);var m=h.endpointStyles[g]||h.endpointStyle||a.Defaults.EndpointStyles[g]||jsPlumb.Defaults.EndpointStyles[g]||a.Defaults.EndpointStyle||jsPlumb.Defaults.EndpointStyle;null==m.fillStyle&&null!=h.paintStyle&&(m.fillStyle=h.paintStyle.strokeStyle),null==m.outlineColor&&null!=h.paintStyle&&(m.outlineColor=h.paintStyle.outlineColor),null==m.outlineWidth&&null!=h.paintStyle&&(m.outlineWidth=h.paintStyle.outlineWidth);var n=h.endpointHoverStyles[g]||h.endpointHoverStyle||a.Defaults.EndpointHoverStyles[g]||jsPlumb.Defaults.EndpointHoverStyles[g]||a.Defaults.EndpointHoverStyle||jsPlumb.Defaults.EndpointHoverStyle;null!=h.hoverPaintStyle&&(null==n&&(n={}),null==n.fillStyle&&(n.fillStyle=h.hoverPaintStyle.strokeStyle));var o=h.anchors?h.anchors[g]:h.anchor?h.anchor:e(a.Defaults.Anchors[g],j,a)||e(b.Defaults.Anchors[g],j,a)||e(a.Defaults.Anchor,j,a)||e(b.Defaults.Anchor,j,a),p=h.uuids?h.uuids[g]:null;k=c({paintStyle:m,hoverPaintStyle:n,endpoint:l,connections:[d],uuid:p,anchor:o,source:i,scope:h.scope,reattach:h.reattach||a.Defaults.ReattachConnections,detachable:h.detachable||a.Defaults.ConnectionsDetachable}),d.endpoints[g]=k,h.drawEndpoints===!1&&k.setVisible(!1,!0,!0)}return k}})}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumbUtil,c=a.jsPlumb;c.AnchorManager=function(a){var d={},e={},f={},g={},h={HORIZONTAL:\"horizontal\",VERTICAL:\"vertical\",DIAGONAL:\"diagonal\",IDENTITY:\"identity\"},i=[\"left\",\"top\",\"right\",\"bottom\"],j={},k=this,l={},m=a.jsPlumbInstance,n={},o=function(a,b,c,d,e,f){if(a===b)return{orientation:h.IDENTITY,a:[\"top\",\"top\"]};var g=Math.atan2(d.centery-c.centery,d.centerx-c.centerx),j=Math.atan2(c.centery-d.centery,c.centerx-d.centerx),k=[],l={};!function(a,b){for(var c=0;c<a.length;c++)l[a[c]]={left:[b[c].left,b[c].centery],right:[b[c].right,b[c].centery],top:[b[c].centerx,b[c].top],bottom:[b[c].centerx,b[c].bottom]}}([\"source\",\"target\"],[c,d]);for(var m=0;m<i.length;m++)for(var n=0;n<i.length;n++)k.push({source:i[m],target:i[n],dist:Biltong.lineLength(l.source[i[m]],l.target[i[n]])});k.sort(function(a,b){return a.dist<b.dist?-1:a.dist>b.dist?1:0});for(var o=k[0].source,p=k[0].target,q=0;q<k.length&&(o=!e.isContinuous||e.isEdgeSupported(k[q].source)?k[q].source:null,p=!f.isContinuous||f.isEdgeSupported(k[q].target)?k[q].target:null,null==o||null==p);q++);return{a:[o,p],theta:g,theta2:j}\n},p=function(a,b,c,d,e,f,g){for(var h=[],i=b[e?0:1]/(d.length+1),j=0;j<d.length;j++){var k=(j+1)*i,l=f*b[e?1:0];g&&(k=b[e?0:1]-k);var m=e?k:l,n=c[0]+m,o=m/b[0],p=e?l:k,q=c[1]+p,r=p/b[1];h.push([n,q,o,r,d[j][1],d[j][2]])}return h},q=function(a){return function(b,c){var d=!0;return d=a?b[0][0]<c[0][0]:b[0][0]>c[0][0],d===!1?-1:1}},r=function(a,b){var c=a[0][0]<0?-Math.PI-a[0][0]:Math.PI-a[0][0],d=b[0][0]<0?-Math.PI-b[0][0]:Math.PI-b[0][0];return c>d?1:a[0][1]>b[0][1]?1:-1},s={top:function(a,b){return a[0]>b[0]?1:-1},right:q(!0),bottom:q(!0),left:r},t=function(a,b){return a.sort(b)},u=function(a,b){var c=m.getCachedData(a),d=c.s,f=c.o,h=function(b,c,d,f,h,i,j){if(f.length>0)for(var k=t(f,s[b]),l=\"right\"===b||\"top\"===b,m=p(b,c,d,k,h,i,l),n=function(a,b){e[a.id]=[b[0],b[1],b[2],b[3]],g[a.id]=j},o=0;o<m.length;o++){var q=m[o][4],r=q.endpoints[0].elementId===a,u=q.endpoints[1].elementId===a;r&&n(q.endpoints[0],m[o]),u&&n(q.endpoints[1],m[o])}};h(\"bottom\",d,[f.left,f.top],b.bottom,!0,1,[0,1]),h(\"top\",d,[f.left,f.top],b.top,!0,0,[0,-1]),h(\"left\",d,[f.left,f.top],b.left,!1,0,[-1,0]),h(\"right\",d,[f.left,f.top],b.right,!1,1,[1,0])};this.reset=function(){d={},j={},l={}},this.addFloatingConnection=function(a,b){n[a]=b},this.removeFloatingConnection=function(a){delete n[a]},this.newConnection=function(a){var d=a.sourceId,e=a.targetId,f=a.endpoints,g=!0,h=function(h,i,k,l,m){d==e&&k.isContinuous&&(a._jsPlumb.instance.removeElement(f[1].canvas),g=!1),b.addToList(j,l,[m,i,k.constructor==c.DynamicAnchor])};h(0,f[0],f[0].anchor,e,a),g&&h(1,f[1],f[1].anchor,d,a)};var v=function(a){!function(a,c){if(a){var d=function(a){return a[4]==c};b.removeWithFunction(a.top,d),b.removeWithFunction(a.left,d),b.removeWithFunction(a.bottom,d),b.removeWithFunction(a.right,d)}}(l[a.elementId],a.id)};this.connectionDetached=function(a,c){var d=a.connection||a,e=a.sourceId,f=a.targetId,g=d.endpoints,h=function(a,c,d,e,f){b.removeWithFunction(j[e],function(a){return a[0].id==f.id})};h(1,g[1],g[1].anchor,e,d),h(0,g[0],g[0].anchor,f,d),d.floatingId&&(h(d.floatingIndex,d.floatingEndpoint,d.floatingEndpoint.anchor,d.floatingId,d),v(d.floatingEndpoint)),v(d.endpoints[0]),v(d.endpoints[1]),c||(k.redraw(d.sourceId),d.targetId!==d.sourceId&&k.redraw(d.targetId))},this.add=function(a,c){b.addToList(d,c,a)},this.changeId=function(a,b){j[b]=j[a],d[b]=d[a],delete j[a],delete d[a]},this.getConnectionsFor=function(a){return j[a]||[]},this.getEndpointsFor=function(a){return d[a]||[]},this.deleteEndpoint=function(a){b.removeWithFunction(d[a.elementId],function(b){return b.id==a.id}),v(a)},this.clearFor=function(a){delete d[a],d[a]=[]};var w=function(c,d,e,f,g,h,i,j,k,l,m,n){var o,p,q=-1,r=-1,s=f.endpoints[i],t=s.id,u=[1,0][i],v=[[d,e],f,g,h,t],w=c[k],x=s._continuousAnchorEdge?c[s._continuousAnchorEdge]:null;if(x){var y=b.findWithFunction(x,function(a){return a[4]==t});if(-1!=y)for(x.splice(y,1),o=0;o<x.length;o++)p=x[o][1],b.addWithFunction(m,p,function(a){return a.id==p.id}),b.addWithFunction(n,x[o][1].endpoints[i],function(a){return a.id==p.endpoints[i].id}),b.addWithFunction(n,x[o][1].endpoints[u],function(a){return a.id==p.endpoints[u].id})}for(o=0;o<w.length;o++)p=w[o][1],1==a.idx&&w[o][3]===h&&-1==r&&(r=o),b.addWithFunction(m,p,function(a){return a.id==p.id}),b.addWithFunction(n,w[o][1].endpoints[i],function(a){return a.id==p.endpoints[i].id}),b.addWithFunction(n,w[o][1].endpoints[u],function(a){return a.id==p.endpoints[u].id});if(-1!=q)w[q]=v;else{var z=j?-1!=r?r:0:w.length;w.splice(z,0,v)}s._continuousAnchorEdge=k};this.updateOtherEndpoint=function(a,d,e,f){var g=b.findWithFunction(j[a],function(a){return a[0].id===f.id}),h=b.findWithFunction(j[d],function(a){return a[0].id===f.id});-1!=g&&(j[a][g][0]=f,j[a][g][1]=f.endpoints[1],j[a][g][2]=f.endpoints[1].anchor.constructor==c.DynamicAnchor),h>-1&&(j[d].splice(h,1),b.addToList(j,e,[f,f.endpoints[0],f.endpoints[0].anchor.constructor==c.DynamicAnchor])),f.updateConnectedClass()},this.sourceChanged=function(a,d,e,f){if(a!==d){e.sourceId=d,e.source=f,b.removeWithFunction(j[a],function(a){return a[0].id===e.id});var g=b.findWithFunction(j[e.targetId],function(a){return a[0].id===e.id});g>-1&&(j[e.targetId][g][0]=e,j[e.targetId][g][1]=e.endpoints[0],j[e.targetId][g][2]=e.endpoints[0].anchor.constructor==c.DynamicAnchor),b.addToList(j,d,[e,e.endpoints[1],e.endpoints[1].anchor.constructor==c.DynamicAnchor]),e.endpoints[1].anchor.isContinuous&&(e.source===e.target?e._jsPlumb.instance.removeElement(e.endpoints[1].canvas):null==e.endpoints[1].canvas.parentNode&&e._jsPlumb.instance.appendElement(e.endpoints[1].canvas)),e.updateConnectedClass()}},this.rehomeEndpoint=function(a,b,c){var e=d[b]||[],f=m.getId(c);if(f!==b){var g=e.indexOf(a);if(g>-1){var h=e.splice(g,1)[0];k.add(h,f)}}for(var i=0;i<a.connections.length;i++)a.connections[i].sourceId==b?k.sourceChanged(b,a.elementId,a.connections[i],a.element):a.connections[i].targetId==b&&(a.connections[i].targetId=a.elementId,a.connections[i].target=a.element,k.updateOtherEndpoint(a.connections[i].sourceId,b,a.elementId,a.connections[i]))},this.redraw=function(a,e,f,g,h,i){if(!m.isSuspendDrawing()){var k=d[a]||[],p=j[a]||[],q=[],r=[],s=[];f=f||m.timestamp(),g=g||{left:0,top:0},e&&(e={left:e.left+g.left,top:e.top+g.top});for(var t=m.updateOffset({elId:a,offset:e,recalc:!1,timestamp:f}),v={},x=0;x<p.length;x++){var y=p[x][0],z=y.sourceId,A=y.targetId,B=y.endpoints[0].anchor.isContinuous,C=y.endpoints[1].anchor.isContinuous;if(B||C){var D=z+\"_\"+A,E=v[D],F=y.sourceId==a?1:0;B&&!l[z]&&(l[z]={top:[],right:[],bottom:[],left:[]}),C&&!l[A]&&(l[A]={top:[],right:[],bottom:[],left:[]}),a!=A&&m.updateOffset({elId:A,timestamp:f}),a!=z&&m.updateOffset({elId:z,timestamp:f});var G=m.getCachedData(A),H=m.getCachedData(z);A==z&&(B||C)?(w(l[z],-Math.PI/2,0,y,!1,A,0,!1,\"top\",z,q,r),w(l[A],-Math.PI/2,0,y,!1,z,1,!1,\"top\",A,q,r)):(E||(E=o(z,A,H.o,G.o,y.endpoints[0].anchor,y.endpoints[1].anchor),v[D]=E),B&&w(l[z],E.theta,0,y,!1,A,0,!1,E.a[0],z,q,r),C&&w(l[A],E.theta2,-1,y,!0,z,1,!0,E.a[1],A,q,r)),B&&b.addWithFunction(s,z,function(a){return a===z}),C&&b.addWithFunction(s,A,function(a){return a===A}),b.addWithFunction(q,y,function(a){return a.id==y.id}),(B&&0===F||C&&1===F)&&b.addWithFunction(r,y.endpoints[F],function(a){return a.id==y.endpoints[F].id})}}for(x=0;x<k.length;x++)0===k[x].connections.length&&k[x].anchor.isContinuous&&(l[a]||(l[a]={top:[],right:[],bottom:[],left:[]}),w(l[a],-Math.PI/2,0,{endpoints:[k[x],k[x]],paint:function(){}},!1,a,0,!1,k[x].anchor.getDefaultFace(),a,q,r),b.addWithFunction(s,a,function(b){return b===a}));for(x=0;x<s.length;x++)u(s[x],l[s[x]]);for(x=0;x<k.length;x++)k[x].paint({timestamp:f,offset:t,dimensions:t.s,recalc:i!==!0});for(x=0;x<r.length;x++){var I=m.getCachedData(r[x].elementId);r[x].paint({timestamp:f,offset:I,dimensions:I.s})}for(x=0;x<p.length;x++){var J=p[x][1];if(J.anchor.constructor==c.DynamicAnchor){J.paint({elementWithPrecedence:a,timestamp:f}),b.addWithFunction(q,p[x][0],function(a){return a.id==p[x][0].id});for(var K=0;K<J.connections.length;K++)J.connections[K]!==p[x][0]&&b.addWithFunction(q,J.connections[K],function(a){return a.id==J.connections[K].id})}else J.anchor.constructor==c.Anchor&&b.addWithFunction(q,p[x][0],function(a){return a.id==p[x][0].id})}var L=n[a];for(L&&L.paint({timestamp:f,recalc:!1,elId:a}),x=0;x<q.length;x++)q[x].paint({elId:a,timestamp:f,recalc:!1,clearEdits:h})}};var x=function(a){b.EventGenerator.apply(this),this.type=\"Continuous\",this.isDynamic=!0,this.isContinuous=!0;for(var c=a.faces||[\"top\",\"right\",\"bottom\",\"left\"],d=!(a.clockwise===!1),h={},i={top:\"bottom\",right:\"left\",left:\"right\",bottom:\"top\"},j={top:\"right\",right:\"bottom\",left:\"top\",bottom:\"left\"},k={top:\"left\",right:\"top\",left:\"bottom\",bottom:\"right\"},l=d?j:k,m=d?k:j,n=a.cssClass||\"\",o=0;o<c.length;o++)h[c[o]]=!0;this.getDefaultFace=function(){return 0===c.length?\"top\":c[0]},this.verifyEdge=function(a){return h[a]?a:h[i[a]]?i[a]:h[l[a]]?l[a]:h[m[a]]?m[a]:a},this.isEdgeSupported=function(a){return h[a]===!0},this.compute=function(a){return f[a.element.id]||e[a.element.id]||[0,0]},this.getCurrentLocation=function(a){return f[a.element.id]||e[a.element.id]||[0,0]},this.getOrientation=function(a){return g[a.id]||[0,0]},this.clearUserDefinedLocation=function(){delete f[a.elementId]},this.setUserDefinedLocation=function(b){f[a.elementId]=b},this.getCssClass=function(){return n}};m.continuousAnchorFactory={get:function(a){return new x(a)},clear:function(a){delete f[a],delete e[a]}}},c.Anchor=function(a){this.x=a.x||0,this.y=a.y||0,this.elementId=a.elementId,this.cssClass=a.cssClass||\"\",this.userDefinedLocation=null,this.orientation=a.orientation||[0,0],this.lastReturnValue=null,this.offsets=a.offsets||[0,0],this.timestamp=null,b.EventGenerator.apply(this),this.compute=function(a){var b=a.xy,c=a.wh,d=a.timestamp;return a.clearUserDefinedLocation&&(this.userDefinedLocation=null),d&&d===self.timestamp?this.lastReturnValue:(this.lastReturnValue=null!=this.userDefinedLocation?this.userDefinedLocation:[b[0]+this.x*c[0]+this.offsets[0],b[1]+this.y*c[1]+this.offsets[1]],this.timestamp=d,this.lastReturnValue)},this.getCurrentLocation=function(a){return a=a||{},null==this.lastReturnValue||null!=a.timestamp&&this.timestamp!=a.timestamp?this.compute(a):this.lastReturnValue}},b.extend(c.Anchor,b.EventGenerator,{equals:function(a){if(!a)return!1;var b=a.getOrientation(),c=this.getOrientation();return this.x==a.x&&this.y==a.y&&this.offsets[0]==a.offsets[0]&&this.offsets[1]==a.offsets[1]&&c[0]==b[0]&&c[1]==b[1]},getUserDefinedLocation:function(){return this.userDefinedLocation},setUserDefinedLocation:function(a){this.userDefinedLocation=a},clearUserDefinedLocation:function(){this.userDefinedLocation=null},getOrientation:function(){return this.orientation},getCssClass:function(){return this.cssClass}}),c.FloatingAnchor=function(a){c.Anchor.apply(this,arguments);var b=a.reference,d=a.referenceCanvas,e=c.getSize(d),f=0,g=0,h=null,i=null;this.orientation=null,this.x=0,this.y=0,this.isFloating=!0,this.compute=function(a){var b=a.xy,c=[b[0]+e[0]/2,b[1]+e[1]/2];return i=c,c},this.getOrientation=function(a){if(h)return h;var c=b.getOrientation(a);return[-1*Math.abs(c[0])*f,-1*Math.abs(c[1])*g]},this.over=function(a,b){h=a.getOrientation(b)},this.out=function(){h=null},this.getCurrentLocation=function(a){return null==i?this.compute(a):i}},b.extend(c.FloatingAnchor,c.Anchor);var d=function(a,b,d){return a.constructor==c.Anchor?a:b.makeAnchor(a,d,b)};c.DynamicAnchor=function(a){c.Anchor.apply(this,arguments),this.isDynamic=!0,this.anchors=[],this.elementId=a.elementId,this.jsPlumbInstance=a.jsPlumbInstance;for(var b=0;b<a.anchors.length;b++)this.anchors[b]=d(a.anchors[b],this.jsPlumbInstance,this.elementId);this.getAnchors=function(){return this.anchors},this.locked=!1;var e=this.anchors.length>0?this.anchors[0]:null,f=e,g=this,h=function(a,b,c,d,e){var f=d[0]+a.x*e[0],g=d[1]+a.y*e[1],h=d[0]+e[0]/2,i=d[1]+e[1]/2;return Math.sqrt(Math.pow(b-f,2)+Math.pow(c-g,2))+Math.sqrt(Math.pow(h-f,2)+Math.pow(i-g,2))},i=a.selector||function(a,b,c,d,e){for(var f=c[0]+d[0]/2,g=c[1]+d[1]/2,i=-1,j=1/0,k=0;k<e.length;k++){var l=h(e[k],f,g,a,b);j>l&&(i=k+0,j=l)}return e[i]};this.compute=function(a){var b=a.xy,c=a.wh,d=a.txy,h=a.twh;this.timestamp=a.timestamp;var j=g.getUserDefinedLocation();return null!=j?j:this.locked||null==d||null==h?e.compute(a):(a.timestamp=null,e=i(b,c,d,h,this.anchors),this.x=e.x,this.y=e.y,e!=f&&this.fire(\"anchorChanged\",e),f=e,e.compute(a))},this.getCurrentLocation=function(a){return this.getUserDefinedLocation()||(null!=e?e.getCurrentLocation(a):null)},this.getOrientation=function(a){return null!=e?e.getOrientation(a):[0,0]},this.over=function(a,b){null!=e&&e.over(a,b)},this.out=function(){null!=e&&e.out()},this.getCssClass=function(){return e&&e.getCssClass()||\"\"}},b.extend(c.DynamicAnchor,c.Anchor);var e=function(a,b,d,e,f,g){c.Anchors[f]=function(c){var h=c.jsPlumbInstance.makeAnchor([a,b,d,e,0,0],c.elementId,c.jsPlumbInstance);return h.type=f,g&&g(h,c),h}};e(.5,0,0,-1,\"TopCenter\"),e(.5,1,0,1,\"BottomCenter\"),e(0,.5,-1,0,\"LeftMiddle\"),e(1,.5,1,0,\"RightMiddle\"),e(.5,0,0,-1,\"Top\"),e(.5,1,0,1,\"Bottom\"),e(0,.5,-1,0,\"Left\"),e(1,.5,1,0,\"Right\"),e(.5,.5,0,0,\"Center\"),e(1,0,0,-1,\"TopRight\"),e(1,1,0,1,\"BottomRight\"),e(0,0,0,-1,\"TopLeft\"),e(0,1,0,1,\"BottomLeft\"),c.Defaults.DynamicAnchors=function(a){return a.jsPlumbInstance.makeAnchors([\"TopCenter\",\"RightMiddle\",\"BottomCenter\",\"LeftMiddle\"],a.elementId,a.jsPlumbInstance)},c.Anchors.AutoDefault=function(a){var b=a.jsPlumbInstance.makeDynamicAnchor(c.Defaults.DynamicAnchors(a));return b.type=\"AutoDefault\",b};var f=function(a,b){c.Anchors[a]=function(c){var d=c.jsPlumbInstance.makeAnchor([\"Continuous\",{faces:b}],c.elementId,c.jsPlumbInstance);return d.type=a,d}};c.Anchors.Continuous=function(a){return a.jsPlumbInstance.continuousAnchorFactory.get(a)},f(\"ContinuousLeft\",[\"left\"]),f(\"ContinuousTop\",[\"top\"]),f(\"ContinuousBottom\",[\"bottom\"]),f(\"ContinuousRight\",[\"right\"]),e(0,0,0,0,\"Assign\",function(a,b){var c=b.position||\"Fixed\";a.positionFinder=c.constructor==String?b.jsPlumbInstance.AnchorPositionFinders[c]:c,a.constructorParams=b}),a.jsPlumbInstance.prototype.AnchorPositionFinders={Fixed:function(a,b,c){return[(a.left-b.left)/c[0],(a.top-b.top)/c[1]]},Grid:function(a,b,c,d){var e=a.left-b.left,f=a.top-b.top,g=c[0]/d.grid[0],h=c[1]/d.grid[1],i=Math.floor(e/g),j=Math.floor(f/h);return[(i*g+g/2)/c[0],(j*h+h/2)/c[1]]}},c.Anchors.Perimeter=function(a){a=a||{};var b=a.anchorCount||60,c=a.shape;if(!c)throw new Error(\"no shape supplied to Perimeter Anchor type\");var d=function(){for(var a=.5,c=2*Math.PI/b,d=0,e=[],f=0;b>f;f++){var g=a+a*Math.sin(d),h=a+a*Math.cos(d);e.push([g,h,0,0]),d+=c}return e},e=function(a){for(var c=b/a.length,d=[],e=function(a,e,f,g,h){c=b*h;for(var i=(f-a)/c,j=(g-e)/c,k=0;c>k;k++)d.push([a+i*k,e+j*k,0,0])},f=0;f<a.length;f++)e.apply(null,a[f]);return d},f=function(a){for(var b=[],c=0;c<a.length;c++)b.push([a[c][0],a[c][1],a[c][2],a[c][3],1/a.length]);return e(b)},g=function(){return f([[0,0,1,0],[1,0,1,1],[1,1,0,1],[0,1,0,0]])},h={Circle:d,Ellipse:d,Diamond:function(){return f([[.5,0,1,.5],[1,.5,.5,1],[.5,1,0,.5],[0,.5,.5,0]])},Rectangle:g,Square:g,Triangle:function(){return f([[.5,0,1,1],[1,1,0,1],[0,1,.5,0]])},Path:function(a){for(var b=a.points,c=[],d=0,f=0;f<b.length-1;f++){var g=Math.sqrt(Math.pow(b[f][2]-b[f][0])+Math.pow(b[f][3]-b[f][1]));d+=g,c.push([b[f][0],b[f][1],b[f+1][0],b[f+1][1],g])}for(var h=0;h<c.length;h++)c[h][4]=c[h][4]/d;return e(c)}},i=function(a,b){for(var c=[],d=b/180*Math.PI,e=0;e<a.length;e++){var f=a[e][0]-.5,g=a[e][1]-.5;c.push([.5+(f*Math.cos(d)-g*Math.sin(d)),.5+(f*Math.sin(d)+g*Math.cos(d)),a[e][2],a[e][3]])}return c};if(!h[c])throw new Error(\"Shape [\"+c+\"] is unknown by Perimeter Anchor type\");var j=h[c](a);a.rotation&&(j=i(j,a.rotation));var k=a.jsPlumbInstance.makeDynamicAnchor(j);return k.type=\"Perimeter\",k}}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d=a.Biltong;b.Segments={AbstractSegment:function(a){this.params=a,this.findClosestPointOnPath=function(){return{d:1/0,x:null,y:null,l:null}},this.getBounds=function(){return{minX:Math.min(a.x1,a.x2),minY:Math.min(a.y1,a.y2),maxX:Math.max(a.x1,a.x2),maxY:Math.max(a.y1,a.y2)}}},Straight:function(a){var c,e,f,g,h,i,j,k=(b.Segments.AbstractSegment.apply(this,arguments),function(){c=Math.sqrt(Math.pow(h-g,2)+Math.pow(j-i,2)),e=d.gradient({x:g,y:i},{x:h,y:j}),f=-1/e});this.type=\"Straight\",this.getLength=function(){return c},this.getGradient=function(){return e},this.getCoordinates=function(){return{x1:g,y1:i,x2:h,y2:j}},this.setCoordinates=function(a){g=a.x1,i=a.y1,h=a.x2,j=a.y2,k()},this.setCoordinates({x1:a.x1,y1:a.y1,x2:a.x2,y2:a.y2}),this.getBounds=function(){return{minX:Math.min(g,h),minY:Math.min(i,j),maxX:Math.max(g,h),maxY:Math.max(i,j)}},this.pointOnPath=function(a,b){if(0!==a||b){if(1!=a||b){var e=b?a>0?a:c+a:a*c;return d.pointOnLine({x:g,y:i},{x:h,y:j},e)}return{x:h,y:j}}return{x:g,y:i}},this.gradientAtPoint=function(){return e},this.pointAlongPathFrom=function(a,b,c){var e=this.pointOnPath(a,c),f=0>=b?{x:g,y:i}:{x:h,y:j};return 0>=b&&Math.abs(b)>1&&(b*=-1),d.pointOnLine(e,f,b)};var l=function(a,b,c){return c>=Math.min(a,b)&&c<=Math.max(a,b)},m=function(a,b,c){return Math.abs(c-a)<Math.abs(c-b)?a:b};this.findClosestPointOnPath=function(a,b){var k={d:1/0,x:null,y:null,l:null,x1:g,x2:h,y1:i,y2:j};if(0===e)k.y=i,k.x=l(g,h,a)?a:m(g,h,a);else if(1/0==e||e==-1/0)k.x=g,k.y=l(i,j,b)?b:m(i,j,b);else{var n=i-e*g,o=b-f*a,p=(o-n)/(e-f),q=e*p+n;k.x=l(g,h,p)?p:m(g,h,p),k.y=l(i,j,q)?q:m(i,j,q)}var r=d.lineLength([k.x,k.y],[g,i]);return k.d=d.lineLength([a,b],[k.x,k.y]),k.l=r/c,k}},Arc:function(a){var c=(b.Segments.AbstractSegment.apply(this,arguments),function(b,c){return d.theta([a.cx,a.cy],[b,c])}),e=function(a,b){if(a.anticlockwise){var c=a.startAngle<a.endAngle?a.startAngle+f:a.startAngle,d=Math.abs(c-a.endAngle);return c-d*b}var e=a.endAngle<a.startAngle?a.endAngle+f:a.endAngle,g=Math.abs(e-a.startAngle);return a.startAngle+g*b},f=2*Math.PI;this.radius=a.r,this.anticlockwise=a.ac,this.type=\"Arc\",a.startAngle&&a.endAngle?(this.startAngle=a.startAngle,this.endAngle=a.endAngle,this.x1=a.cx+this.radius*Math.cos(a.startAngle),this.y1=a.cy+this.radius*Math.sin(a.startAngle),this.x2=a.cx+this.radius*Math.cos(a.endAngle),this.y2=a.cy+this.radius*Math.sin(a.endAngle)):(this.startAngle=c(a.x1,a.y1),this.endAngle=c(a.x2,a.y2),this.x1=a.x1,this.y1=a.y1,this.x2=a.x2,this.y2=a.y2),this.endAngle<0&&(this.endAngle+=f),this.startAngle<0&&(this.startAngle+=f);var g=this.endAngle<this.startAngle?this.endAngle+f:this.endAngle;this.sweep=Math.abs(g-this.startAngle),this.anticlockwise&&(this.sweep=f-this.sweep);var h=2*Math.PI*this.radius,i=this.sweep/f,j=h*i;this.getLength=function(){return j},this.getBounds=function(){return{minX:a.cx-a.r,maxX:a.cx+a.r,minY:a.cy-a.r,maxY:a.cy+a.r}};var k=1e-10,l=function(a){var b=Math.floor(a),c=Math.ceil(a);return k>a-b?b:k>c-a?c:a};this.pointOnPath=function(b,c){if(0===b)return{x:this.x1,y:this.y1,theta:this.startAngle};if(1==b)return{x:this.x2,y:this.y2,theta:this.endAngle};c&&(b/=j);var d=e(this,b),f=a.cx+a.r*Math.cos(d),g=a.cy+a.r*Math.sin(d);return{x:l(f),y:l(g),theta:d}},this.gradientAtPoint=function(b,c){var e=this.pointOnPath(b,c),f=d.normal([a.cx,a.cy],[e.x,e.y]);return this.anticlockwise||1/0!=f&&f!=-1/0||(f*=-1),f},this.pointAlongPathFrom=function(b,c,d){var e=this.pointOnPath(b,d),f=2*(c/h)*Math.PI,g=this.anticlockwise?-1:1,i=e.theta+g*f,j=a.cx+this.radius*Math.cos(i),k=a.cy+this.radius*Math.sin(i);return{x:j,y:k}}},Bezier:function(c){this.curve=[{x:c.x1,y:c.y1},{x:c.cp1x,y:c.cp1y},{x:c.cp2x,y:c.cp2y},{x:c.x2,y:c.y2}],b.Segments.AbstractSegment.apply(this,arguments),this.bounds={minX:Math.min(c.x1,c.x2,c.cp1x,c.cp2x),minY:Math.min(c.y1,c.y2,c.cp1y,c.cp2y),maxX:Math.max(c.x1,c.x2,c.cp1x,c.cp2x),maxY:Math.max(c.y1,c.y2,c.cp1y,c.cp2y)},this.type=\"Bezier\";var d=function(b,c,d){return d&&(c=a.jsBezier.locationAlongCurveFrom(b,c>0?0:1,c)),c};this.pointOnPath=function(b,c){return b=d(this.curve,b,c),a.jsBezier.pointOnCurve(this.curve,b)},this.gradientAtPoint=function(b,c){return b=d(this.curve,b,c),a.jsBezier.gradientAtPoint(this.curve,b)},this.pointAlongPathFrom=function(b,c,e){return b=d(this.curve,b,e),a.jsBezier.pointAlongCurveFrom(this.curve,b,c)},this.getLength=function(){return a.jsBezier.getLength(this.curve)},this.getBounds=function(){return this.bounds}}};var e=function(){this.resetBounds=function(){this.bounds={minX:1/0,minY:1/0,maxX:-1/0,maxY:-1/0}},this.resetBounds()};b.Connectors.AbstractConnector=function(a){e.apply(this,arguments);var f=[],g=0,h=[],i=[],j=a.stub||0,k=c.isArray(j)?j[0]:j,l=c.isArray(j)?j[1]:j,m=a.gap||0,n=c.isArray(m)?m[0]:m,o=c.isArray(m)?m[1]:m,p=null,q=!1,r=null,s=null,t=a.editable!==!1&&null!=jsPlumb.ConnectorEditors&&null!=jsPlumb.ConnectorEditors[this.type],u=this.setGeometry=function(a,b){q=!b,s=a},v=this.getGeometry=function(){return s};this.hasBeenEdited=function(){return q},this.isEditing=function(){return null!=this.editor&&this.editor.isActive()},this.setEditable=function(a){return t=a&&null!=jsPlumb.ConnectorEditors&&null!=jsPlumb.ConnectorEditors[this.type]&&(null==this.overrideSetEditable||this.overrideSetEditable())?a:!1},this.isEditable=function(){return t},this.findSegmentForPoint=function(a,b){for(var c={d:1/0,s:null,x:null,y:null,l:null},d=0;d<f.length;d++){var e=f[d].findClosestPointOnPath(a,b);e.d<c.d&&(c.d=e.d,c.l=e.l,c.x=e.x,c.y=e.y,c.s=f[d],c.x1=e.x1,c.x2=e.x2,c.y1=e.y1,c.y2=e.y2,c.index=d)}return c};var w=function(){for(var a=0,b=0;b<f.length;b++){var c=f[b].getLength();i[b]=c/g,h[b]=[a,a+=c/g]}},x=function(a,b){b&&(a=a>0?a/g:(g+a)/g);for(var c=h.length-1,d=1,e=0;e<h.length;e++)if(h[e][1]>=a){c=e,d=1==a?1:0===a?0:(a-h[e][0])/i[e];break}return{segment:f[c],proportion:d,index:c}},y=function(a,c,d){if(d.x1!=d.x2||d.y1!=d.y2){var e=new b.Segments[c](d);f.push(e),g+=e.getLength(),a.updateBounds(e)}},z=function(){g=f.length=h.length=i.length=0};this.setSegments=function(a){p=[],g=0;for(var b=0;b<a.length;b++)p.push(a[b]),g+=a[b].getLength()},this.getLength=function(){return g};var A=function(a){this.lineWidth=a.lineWidth;var b=d.quadrant(a.sourcePos,a.targetPos),c=a.targetPos[0]<a.sourcePos[0],e=a.targetPos[1]<a.sourcePos[1],f=a.lineWidth||1,g=a.sourceEndpoint.anchor.getOrientation(a.sourceEndpoint),h=a.targetEndpoint.anchor.getOrientation(a.targetEndpoint),i=c?a.targetPos[0]:a.sourcePos[0],j=e?a.targetPos[1]:a.sourcePos[1],m=Math.abs(a.targetPos[0]-a.sourcePos[0]),p=Math.abs(a.targetPos[1]-a.sourcePos[1]);if(0===g[0]&&0===g[1]||0===h[0]&&0===h[1]){var q=m>p?0:1,r=[1,0][q];g=[],h=[],g[q]=a.sourcePos[q]>a.targetPos[q]?-1:1,h[q]=a.sourcePos[q]>a.targetPos[q]?1:-1,g[r]=0,h[r]=0}var s=c?m+n*g[0]:n*g[0],t=e?p+n*g[1]:n*g[1],u=c?o*h[0]:m+o*h[0],v=e?o*h[1]:p+o*h[1],w=g[0]*h[0]+g[1]*h[1],x={sx:s,sy:t,tx:u,ty:v,lw:f,xSpan:Math.abs(u-s),ySpan:Math.abs(v-t),mx:(s+u)/2,my:(t+v)/2,so:g,to:h,x:i,y:j,w:m,h:p,segment:b,startStubX:s+g[0]*k,startStubY:t+g[1]*k,endStubX:u+h[0]*l,endStubY:v+h[1]*l,isXGreaterThanStubTimes2:Math.abs(s-u)>k+l,isYGreaterThanStubTimes2:Math.abs(t-v)>k+l,opposite:-1==w,perpendicular:0===w,orthogonal:1==w,sourceAxis:0===g[0]?\"y\":\"x\",points:[i,j,m,p,s,t,u,v]};return x.anchorOrientation=x.opposite?\"opposite\":x.orthogonal?\"orthogonal\":\"perpendicular\",x};return this.getSegments=function(){return f},this.updateBounds=function(a){var b=a.getBounds();this.bounds.minX=Math.min(this.bounds.minX,b.minX),this.bounds.maxX=Math.max(this.bounds.maxX,b.maxX),this.bounds.minY=Math.min(this.bounds.minY,b.minY),this.bounds.maxY=Math.max(this.bounds.maxY,b.maxY)},this.pointOnPath=function(a,b){var c=x(a,b);return c.segment&&c.segment.pointOnPath(c.proportion,!1)||[0,0]},this.gradientAtPoint=function(a,b){var c=x(a,b);return c.segment&&c.segment.gradientAtPoint(c.proportion,!1)||0},this.pointAlongPathFrom=function(a,b,c){var d=x(a,c);return d.segment&&d.segment.pointAlongPathFrom(d.proportion,b,!1)||[0,0]},this.compute=function(a){r=A.call(this,a),z(),this._compute(r,a),this.x=r.points[0],this.y=r.points[1],this.w=r.points[2],this.h=r.points[3],this.segment=r.segment,w()},{addSegment:y,prepareCompute:A,sourceStub:k,targetStub:l,maxStub:Math.max(k,l),sourceGap:n,targetGap:o,maxGap:Math.max(n,o),setGeometry:u,getGeometry:v}},c.extend(b.Connectors.AbstractConnector,e);var f=b.Connectors.Straight=function(){this.type=\"Straight\";var a=b.Connectors.AbstractConnector.apply(this,arguments);this._compute=function(b){a.addSegment(this,\"Straight\",{x1:b.sx,y1:b.sy,x2:b.startStubX,y2:b.startStubY}),a.addSegment(this,\"Straight\",{x1:b.startStubX,y1:b.startStubY,x2:b.endStubX,y2:b.endStubY}),a.addSegment(this,\"Straight\",{x1:b.endStubX,y1:b.endStubY,x2:b.tx,y2:b.ty})}};c.extend(b.Connectors.Straight,b.Connectors.AbstractConnector),b.registerConnectorType(f,\"Straight\"),b.Endpoints.AbstractEndpoint=function(a){e.apply(this,arguments);var b=this.compute=function(){var a=this._compute.apply(this,arguments);return this.x=a[0],this.y=a[1],this.w=a[2],this.h=a[3],this.bounds.minX=this.x,this.bounds.minY=this.y,this.bounds.maxX=this.x+this.w,this.bounds.maxY=this.y+this.h,a};return{compute:b,cssClass:a.cssClass}},c.extend(b.Endpoints.AbstractEndpoint,e),b.Endpoints.Dot=function(a){this.type=\"Dot\",b.Endpoints.AbstractEndpoint.apply(this,arguments),a=a||{},this.radius=a.radius||10,this.defaultOffset=.5*this.radius,this.defaultInnerRadius=this.radius/3,this._compute=function(a,b,c){this.radius=c.radius||this.radius;var d=a[0]-this.radius,e=a[1]-this.radius,f=2*this.radius,g=2*this.radius;if(c.strokeStyle){var h=c.lineWidth||1;d-=h,e-=h,f+=2*h,g+=2*h}return[d,e,f,g,this.radius]}},c.extend(b.Endpoints.Dot,b.Endpoints.AbstractEndpoint),b.Endpoints.Rectangle=function(a){this.type=\"Rectangle\",b.Endpoints.AbstractEndpoint.apply(this,arguments),a=a||{},this.width=a.width||20,this.height=a.height||20,this._compute=function(a,b,c){var d=c.width||this.width,e=c.height||this.height,f=a[0]-d/2,g=a[1]-e/2;return[f,g,d,e]}},c.extend(b.Endpoints.Rectangle,b.Endpoints.AbstractEndpoint);var g=function(){b.jsPlumbUIComponent.apply(this,arguments),this._jsPlumb.displayElements=[]};c.extend(g,b.jsPlumbUIComponent,{getDisplayElements:function(){return this._jsPlumb.displayElements},appendDisplayElement:function(a){this._jsPlumb.displayElements.push(a)}}),b.Endpoints.Image=function(d){this.type=\"Image\",g.apply(this,arguments),b.Endpoints.AbstractEndpoint.apply(this,arguments);var e=d.onload,f=d.src||d.url,h=d.cssClass?\" \"+d.cssClass:\"\";this._jsPlumb.img=new Image,this._jsPlumb.ready=!1,this._jsPlumb.initialized=!1,this._jsPlumb.deleted=!1,this._jsPlumb.widthToUse=d.width,this._jsPlumb.heightToUse=d.height,this._jsPlumb.endpoint=d.endpoint,this._jsPlumb.img.onload=function(){null!=this._jsPlumb&&(this._jsPlumb.ready=!0,this._jsPlumb.widthToUse=this._jsPlumb.widthToUse||this._jsPlumb.img.width,this._jsPlumb.heightToUse=this._jsPlumb.heightToUse||this._jsPlumb.img.height,e&&e(this))}.bind(this),this._jsPlumb.endpoint.setImage=function(a,b){var c=a.constructor==String?a:a.src;e=b,this._jsPlumb.img.src=c,null!=this.canvas&&this.canvas.setAttribute(\"src\",this._jsPlumb.img.src)}.bind(this),this._jsPlumb.endpoint.setImage(f,e),this._compute=function(a){return this.anchorPoint=a,this._jsPlumb.ready?[a[0]-this._jsPlumb.widthToUse/2,a[1]-this._jsPlumb.heightToUse/2,this._jsPlumb.widthToUse,this._jsPlumb.heightToUse]:[0,0,0,0]},this.canvas=jsPlumb.createElement(\"img\",{position:\"absolute\",margin:0,padding:0,outline:0},this._jsPlumb.instance.endpointClass+h),this._jsPlumb.widthToUse&&this.canvas.setAttribute(\"width\",this._jsPlumb.widthToUse),this._jsPlumb.heightToUse&&this.canvas.setAttribute(\"height\",this._jsPlumb.heightToUse),this._jsPlumb.instance.appendElement(this.canvas),this.actuallyPaint=function(){if(!this._jsPlumb.deleted){this._jsPlumb.initialized||(this.canvas.setAttribute(\"src\",this._jsPlumb.img.src),this.appendDisplayElement(this.canvas),this._jsPlumb.initialized=!0);var a=this.anchorPoint[0]-this._jsPlumb.widthToUse/2,b=this.anchorPoint[1]-this._jsPlumb.heightToUse/2;c.sizeElement(this.canvas,a,b,this._jsPlumb.widthToUse,this._jsPlumb.heightToUse)}},this.paint=function(b,c){null!=this._jsPlumb&&(this._jsPlumb.ready?this.actuallyPaint(b,c):a.setTimeout(function(){this.paint(b,c)}.bind(this),200))}},c.extend(b.Endpoints.Image,[g,b.Endpoints.AbstractEndpoint],{cleanup:function(a){a&&(this._jsPlumb.deleted=!0,this.canvas&&this.canvas.parentNode.removeChild(this.canvas),this.canvas=null)}}),b.Endpoints.Blank=function(a){b.Endpoints.AbstractEndpoint.apply(this,arguments),this.type=\"Blank\",g.apply(this,arguments),this._compute=function(a){return[a[0],a[1],10,0]};var d=a.cssClass?\" \"+a.cssClass:\"\";this.canvas=jsPlumb.createElement(\"div\",{display:\"block\",width:\"1px\",height:\"1px\",background:\"transparent\",position:\"absolute\"},this._jsPlumb.instance.endpointClass+d),this._jsPlumb.instance.appendElement(this.canvas),this.paint=function(){c.sizeElement(this.canvas,this.x,this.y,this.w,this.h)}},c.extend(b.Endpoints.Blank,[b.Endpoints.AbstractEndpoint,g],{cleanup:function(){this.canvas&&this.canvas.parentNode&&this.canvas.parentNode.removeChild(this.canvas)}}),b.Endpoints.Triangle=function(a){this.type=\"Triangle\",b.Endpoints.AbstractEndpoint.apply(this,arguments),a=a||{},a.width=a.width||55,a.height=a.height||55,this.width=a.width,this.height=a.height,this._compute=function(a,b,c){var d=c.width||self.width,e=c.height||self.height,f=a[0]-d/2,g=a[1]-e/2;return[f,g,d,e]}};var h=b.Overlays.AbstractOverlay=function(a){this.visible=!0,this.isAppendedAtTopLevel=!0,this.component=a.component,this.loc=null==a.location?.5:a.location,this.endpointLoc=null==a.endpointLocation?[.5,.5]:a.endpointLocation,this.visible=a.visible!==!1};h.prototype={cleanup:function(a){a&&(this.component=null,this.canvas=null,this.endpointLoc=null)},reattach:function(){},setVisible:function(a){this.visible=a,this.component.repaint()},isVisible:function(){return this.visible},hide:function(){this.setVisible(!1)},show:function(){this.setVisible(!0)},incrementLocation:function(a){this.loc+=a,this.component.repaint()},setLocation:function(a){this.loc=a,this.component.repaint()},getLocation:function(){return this.loc},updateFrom:function(){}},b.Overlays.Arrow=function(a){this.type=\"Arrow\",h.apply(this,arguments),this.isAppendedAtTopLevel=!1,a=a||{},this.length=a.length||20,this.width=a.width||20,this.id=a.id;var b=(a.direction||1)<0?-1:1,e=a.paintStyle||{lineWidth:1},f=a.foldback||.623;this.computeMaxSize=function(){return 1.5*self.width},this.elementCreated=function(b){if(this.path=b,a.events)for(var c in a.events)jsPlumb.on(b,c,a.events[c])},this.draw=function(a,g){var h,i,j,k,l;if(a.pointAlongPathFrom){if(c.isString(this.loc)||this.loc>1||this.loc<0){var m=parseInt(this.loc,10),n=this.loc<0?1:0;h=a.pointAlongPathFrom(n,m,!1),i=a.pointAlongPathFrom(n,m-b*this.length/2,!1),j=d.pointOnLine(h,i,this.length)}else if(1==this.loc){if(h=a.pointOnPath(this.loc),i=a.pointAlongPathFrom(this.loc,-this.length),j=d.pointOnLine(h,i,this.length),-1==b){var o=j;j=h,h=o}}else if(0===this.loc){if(j=a.pointOnPath(this.loc),i=a.pointAlongPathFrom(this.loc,this.length),h=d.pointOnLine(j,i,this.length),-1==b){var p=j;j=h,h=p}}else h=a.pointAlongPathFrom(this.loc,b*this.length/2),i=a.pointOnPath(this.loc),j=d.pointOnLine(h,i,this.length);k=d.perpendicularLineTo(h,j,this.width),l=d.pointOnLine(h,j,f*this.length);var q={hxy:h,tail:k,cxy:l},r=e.strokeStyle||g.strokeStyle,s=e.fillStyle||g.strokeStyle,t=e.lineWidth||g.lineWidth;return{component:a,d:q,lineWidth:t,strokeStyle:r,fillStyle:s,minX:Math.min(h.x,k[0].x,k[1].x),maxX:Math.max(h.x,k[0].x,k[1].x),minY:Math.min(h.y,k[0].y,k[1].y),maxY:Math.max(h.y,k[0].y,k[1].y)}}return{component:a,minX:0,maxX:0,minY:0,maxY:0}}},c.extend(b.Overlays.Arrow,h,{updateFrom:function(a){this.length=a.length||this.length,this.width=a.width||this.width,this.direction=null!=a.direction?a.direction:this.direction,this.foldback=a.foldback||this.foldback}}),b.Overlays.PlainArrow=function(a){a=a||{};var c=b.extend(a,{foldback:1});b.Overlays.Arrow.call(this,c),this.type=\"PlainArrow\"},c.extend(b.Overlays.PlainArrow,b.Overlays.Arrow),b.Overlays.Diamond=function(a){a=a||{};var c=a.length||40,d=jsPlumb.extend(a,{length:c/2,foldback:2});b.Overlays.Arrow.call(this,d),this.type=\"Diamond\"},c.extend(b.Overlays.Diamond,b.Overlays.Arrow);var i=function(a,b){return(null==a._jsPlumb.cachedDimensions||b)&&(a._jsPlumb.cachedDimensions=a.getDimensions()),a._jsPlumb.cachedDimensions},j=function(a){b.jsPlumbUIComponent.apply(this,arguments),h.apply(this,arguments);var d=this.fire;this.fire=function(){d.apply(this,arguments),this.component&&this.component.fire.apply(this.component,arguments)},this.detached=!1,this.id=a.id,this._jsPlumb.div=null,this._jsPlumb.initialised=!1,this._jsPlumb.component=a.component,this._jsPlumb.cachedDimensions=null,this._jsPlumb.create=a.create,this._jsPlumb.initiallyInvisible=a.visible===!1,this.getElement=function(){if(null==this._jsPlumb.div){var b=this._jsPlumb.div=jsPlumb.getElement(this._jsPlumb.create(this._jsPlumb.component));\nb.style.position=\"absolute\",b.className=this._jsPlumb.instance.overlayClass+\" \"+(this.cssClass?this.cssClass:a.cssClass?a.cssClass:\"\"),this._jsPlumb.instance.appendElement(b),this._jsPlumb.instance.getId(b),this.canvas=b;var c=\"translate(-50%, -50%)\";b.style.webkitTransform=c,b.style.mozTransform=c,b.style.msTransform=c,b.style.oTransform=c,b.style.transform=c,b._jsPlumb=this,a.visible===!1&&(b.style.display=\"none\")}return this._jsPlumb.div},this.draw=function(a,b,d){var e=i(this);if(null!=e&&2==e.length){var f={x:0,y:0};if(d)f={x:d[0],y:d[1]};else if(a.pointOnPath){var g=this.loc,h=!1;(c.isString(this.loc)||this.loc<0||this.loc>1)&&(g=parseInt(this.loc,10),h=!0),f=a.pointOnPath(g,h)}else{var j=this.loc.constructor==Array?this.loc:this.endpointLoc;f={x:j[0]*a.w,y:j[1]*a.h}}var k=f.x-e[0]/2,l=f.y-e[1]/2;return{component:a,d:{minx:k,miny:l,td:e,cxy:f},minX:k,maxX:k+e[0],minY:l,maxY:l+e[1]}}return{minX:0,maxX:0,minY:0,maxY:0}}};c.extend(j,[b.jsPlumbUIComponent,h],{getDimensions:function(){return[1,1]},setVisible:function(a){this._jsPlumb.div&&(this._jsPlumb.div.style.display=a?\"block\":\"none\",a&&this._jsPlumb.initiallyInvisible&&(i(this,!0),this.component.repaint(),this._jsPlumb.initiallyInvisible=!1))},clearCachedDimensions:function(){this._jsPlumb.cachedDimensions=null},cleanup:function(a){a?null!=this._jsPlumb.div&&(this._jsPlumb.div._jsPlumb=null,this._jsPlumb.instance.removeElement(this._jsPlumb.div)):(this._jsPlumb&&this._jsPlumb.div&&this._jsPlumb.div.parentNode&&this._jsPlumb.div.parentNode.removeChild(this._jsPlumb.div),this.detached=!0)},reattach:function(a){null!=this._jsPlumb.div&&a.getContainer().appendChild(this._jsPlumb.div),this.detached=!1},computeMaxSize:function(){var a=i(this);return Math.max(a[0],a[1])},paint:function(a){this._jsPlumb.initialised||(this.getElement(),a.component.appendDisplayElement(this._jsPlumb.div),this._jsPlumb.initialised=!0,this.detached&&this._jsPlumb.div.parentNode.removeChild(this._jsPlumb.div)),this._jsPlumb.div.style.left=a.component.x+a.d.minx+\"px\",this._jsPlumb.div.style.top=a.component.y+a.d.miny+\"px\"}}),b.Overlays.Custom=function(){this.type=\"Custom\",j.apply(this,arguments)},c.extend(b.Overlays.Custom,j),b.Overlays.GuideLines=function(){var a=this;a.length=50,a.lineWidth=5,this.type=\"GuideLines\",h.apply(this,arguments),b.jsPlumbUIComponent.apply(this,arguments),this.draw=function(b){var c=b.pointAlongPathFrom(a.loc,a.length/2),e=b.pointOnPath(a.loc),f=d.pointOnLine(c,e,a.length),g=d.perpendicularLineTo(c,f,40),h=d.perpendicularLineTo(f,c,20);return{connector:b,head:c,tail:f,headLine:h,tailLine:g,minX:Math.min(c.x,f.x,h[0].x,h[1].x),minY:Math.min(c.y,f.y,h[0].y,h[1].y),maxX:Math.max(c.x,f.x,h[0].x,h[1].x),maxY:Math.max(c.y,f.y,h[0].y,h[1].y)}}},b.Overlays.Label=function(a){this.labelStyle=a.labelStyle,this.cssClass=null!=this.labelStyle?this.labelStyle.cssClass:null;var c=b.extend({create:function(){return jsPlumb.createElement(\"div\")}},a);if(b.Overlays.Custom.call(this,c),this.type=\"Label\",this.label=a.label||\"\",this.labelText=null,this.labelStyle){var d=this.getElement();if(this.labelStyle.font=this.labelStyle.font||\"12px sans-serif\",d.style.font=this.labelStyle.font,d.style.color=this.labelStyle.color||\"black\",this.labelStyle.fillStyle&&(d.style.background=this.labelStyle.fillStyle),this.labelStyle.borderWidth>0){var e=this.labelStyle.borderStyle?this.labelStyle.borderStyle:\"black\";d.style.border=this.labelStyle.borderWidth+\"px solid \"+e}this.labelStyle.padding&&(d.style.padding=this.labelStyle.padding)}},c.extend(b.Overlays.Label,b.Overlays.Custom,{cleanup:function(a){a&&(this.div=null,this.label=null,this.labelText=null,this.cssClass=null,this.labelStyle=null)},getLabel:function(){return this.label},setLabel:function(a){this.label=a,this.labelText=null,this.clearCachedDimensions(),this.update(),this.component.repaint()},getDimensions:function(){return this.update(),j.prototype.getDimensions.apply(this,arguments)},update:function(){if(\"function\"==typeof this.label){var a=this.label(this);this.getElement().innerHTML=a.replace(/\\r\\n/g,\"<br/>\")}else null==this.labelText&&(this.labelText=this.label,this.getElement().innerHTML=this.labelText.replace(/\\r\\n/g,\"<br/>\"))},updateFrom:function(a){a.label&&this.setLabel(a.label)}})}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=function(b){var c=b._mottle;return c||(c=b._mottle=new a.Mottle),c};b.extend(a.jsPlumbInstance.prototype,{getEventManager:function(){return c(this)},on:function(){return this.getEventManager().on.apply(this,arguments),this},off:function(){return this.getEventManager().off.apply(this,arguments),this}})}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumbUtil,c=a.jsPlumbInstance,d=\"jsplumb-group-collapsed\",e=\"jsplumb-group-expanded\",f=\"[jsplumb-group-content]\",g=\"elementDraggable\",h=\"stop\",i=\"revert\",j=\"_groupManager\",k=\"_jsPlumbGroup\",l=\"_jsPlumbGroupDrag\",m=\"group:addMember\",n=\"group:removeMember\",o=\"group:add\",p=\"group:remove\",q=\"group:expand\",r=\"group:collapse\",s=function(a){function c(a){delete a.proxies;var c,d=i[a.id];null!=d&&(c=function(b){return b.id===a.id},b.removeWithFunction(d.connections.source,c),b.removeWithFunction(d.connections.target,c),delete i[a.id]),d=j[a.id],null!=d&&(c=function(b){return b.id===a.id},b.removeWithFunction(d.connections.source,c),b.removeWithFunction(d.connections.target,c),delete j[a.id])}function f(b,c){for(var d=b.getMembers(),e=0;e<d.length;e++)a[c?\"show\":\"hide\"](d[e],!0)}function g(b){var c=b.getMembers(),d=a.getConnections({source:c},!0),e=a.getConnections({target:c},!0),f={};b.connections.source.length=0,b.connections.target.length=0;var g=function(a){for(var c=0;c<a.length;c++)f[a[c].id]||(f[a[c].id]=!0,a[c].source._jsPlumbGroup===b?(a[c].target._jsPlumbGroup!==b&&b.connections.source.push(a[c]),i[a[c].id]=b):a[c].target._jsPlumbGroup===b&&(b.connections.target.push(a[c]),j[a[c].id]=b))};g(d),g(e)}var h={},i={},j={};a.bind(\"connection\",function(a){null!=a.source[k]&&null!=a.target[k]&&a.source[k]===a.target[k]?(i[a.connection.id]=a.source[k],j[a.connection.id]=a.source[k]):(null!=a.source[k]&&(b.suggest(a.source[k].connections.source,a.connection),i[a.connection.id]=a.source[k]),null!=a.target[k]&&(b.suggest(a.target[k].connections.target,a.connection),j[a.connection.id]=a.target[k]))}),a.bind(\"connectionDetached\",function(a){c(a.connection)}),a.bind(\"connectionMoved\",function(a){var b=0===a.index?i:j,c=b[a.connection.id];if(c){var d=c.connections[0===a.index?\"source\":\"target\"],e=d.indexOf(a.connection);-1!=e&&d.splice(e,1)}}),this.addGroup=function(b){a.addClass(b.el,e),h[b.id]=b,b.manager=this,g(b),a.fire(o,{group:b})},this.addToGroup=function(a,b,c){a=this.getGroup(a),a&&a.add(b,c)},this.removeFromGroup=function(a,b,c){a=this.getGroup(a),a&&a.remove(b,null,c)},this.getGroup=function(a){var c=a;if(b.isString(a)&&(c=h[a],null==c))throw new TypeError(\"No such group [\"+a+\"]\");return c},this.getGroups=function(){var a=[];for(var b in h)a.push(h[b]);return a},this.removeGroup=function(b,c){b=this.getGroup(b),this.expandGroup(b,!0),b[c?\"removeAll\":\"orphanAll\"](),a.remove(b.getEl()),delete h[b.id],delete a._groups[b.id],a.fire(p,{group:b})},this.removeAllGroups=function(a){for(var b in h)this.removeGroup(h[b],a)};var l=this.collapseConnection=function(b,c,d){var e,f=d.getEl(),g=a.getId(f),h=b.endpoints[c].elementId,i=b.endpoints[0===c?1:0].element;i[k]&&!i[k].shouldProxy()&&i[k].collapsed||(b.proxies=b.proxies||[],b.proxies[c]?e=b.proxies[c].ep:(e=a.addEndpoint(f,{endpoint:d.getEndpoint(b,c),anchor:d.getAnchor(b,c),parameters:{isProxyEndpoint:!0}}),e._forceDeleteOnDetach=!0),b.proxies[c]={ep:e,originalEp:b.endpoints[c]},0===c?a.anchorManager.sourceChanged(h,g,b,f):(a.anchorManager.updateOtherEndpoint(b.endpoints[0].elementId,h,g,b),b.target=f,b.targetId=g),b.proxies[c].originalEp.detachFromConnection(b,null,!0),e.connections=[b],b.endpoints[c]=e,b.setVisible(!0))};this.collapseGroup=function(b){if(b=this.getGroup(b),null!=b&&!b.collapsed){var c=b.getEl();if(f(b,!1),b.shouldProxy()){var g=function(a,c){for(var d=0;d<a.length;d++){var e=a[d];l(e,c,b)}};g(b.connections.source,0),g(b.connections.target,1)}b.collapsed=!0,a.removeClass(c,e),a.addClass(c,d),a.revalidate(c),a.fire(r,{group:b})}};var m=this.expandConnection=function(b,c,d){if(null!=b.proxies&&null!=b.proxies[c]){var e=a.getId(d.getEl()),f=b.proxies[c].originalEp.element,g=b.proxies[c].originalEp.elementId;b.endpoints[c]=b.proxies[c].originalEp,0===c?a.anchorManager.sourceChanged(e,g,b,f):(a.anchorManager.updateOtherEndpoint(b.endpoints[0].elementId,e,g,b),b.target=f,b.targetId=g),b.proxies[c].ep.detachFromConnection(b,null,!0),b.proxies[c].originalEp.addConnection(b),delete b.proxies[c]}};this.expandGroup=function(b,c){if(b=this.getGroup(b),null!=b&&b.collapsed){var g=b.getEl();if(f(b,!0),b.shouldProxy()){var h=function(a,c){for(var d=0;d<a.length;d++){var e=a[d];m(e,c,b)}};h(b.connections.source,0),h(b.connections.target,1)}b.collapsed=!1,a.addClass(g,e),a.removeClass(g,d),a.revalidate(g),this.repaintGroup(b),c||a.fire(q,{group:b})}},this.repaintGroup=function(b){b=this.getGroup(b);for(var c=b.getMembers(),d=0;d<c.length;d++)a.revalidate(c[d])},this.updateConnectionsForGroup=g,this.refreshAllGroups=function(){for(var b in h)g(h[b]),a.dragManager.updateOffsets(a.getId(h[b].getEl()))}},t=function(c,d){function e(a){return a.offsetParent}function j(a,b){var d=e(a),f=c.getSize(d),g=c.getSize(a),h=b[0],i=h+g[0],j=b[1],k=j+g[1];return i>0&&h<f[0]&&k>0&&j<f[1]}function o(a){var b=c.getId(a),d=c.getOffset(a);a.parentNode.removeChild(a),c.getContainer().appendChild(a),c.setPosition(a,d),delete a._jsPlumbGroup,r(a),c.dragManager.clearParent(a,b)}function p(a){j(a.el,a.pos)||(a.el._jsPlumbGroup.remove(a.el),B?c.remove(a.el):o(a.el))}function q(a){var b=c.getId(a);c.revalidate(a),c.dragManager.revalidateParent(a,b)}function r(a){a._katavorioDrag&&((B||A)&&a._katavorioDrag.off(h,p),B||A||!z||(a._katavorioDrag.off(i,q),a._katavorioDrag.setRevert(null)))}function s(a){a._katavorioDrag&&((B||A)&&a._katavorioDrag.on(h,p),y&&a._katavorioDrag.setConstrain(!0),x&&a._katavorioDrag.setUseGhostProxy(!0),B||A||!z||(a._katavorioDrag.on(i,q),a._katavorioDrag.setRevert(function(a,b){return!j(a,b)})))}var t=this,u=d.el;this.getEl=function(){return u},this.id=d.id||b.uuid(),u._isJsPlumbGroup=!0;var v=c.getSelector(u,f),w=v&&v.length>0?v[0]:u,x=d.ghost===!0,y=x||d.constrain===!0,z=d.revert!==!1,A=d.orphan===!0,B=d.prune===!0,C=d.dropOverride===!0,D=d.proxied!==!1,E=[];if(this.connections={source:[],target:[],internal:[]},this.getAnchor=function(){return d.anchor||\"Continuous\"},this.getEndpoint=function(){return d.endpoint||[\"Dot\",{radius:10}]},this.collapsed=!1,d.draggable!==!1){var F={stop:function(a){c.fire(\"groupDragStop\",jsPlumb.extend(a,{group:t}))},scope:l};d.dragOptions&&a.jsPlumb.extend(F,d.dragOptions),c.draggable(d.el,F)}d.droppable!==!1&&c.droppable(d.el,{drop:function(a){var b=c.getGroupManager(),d=a.drag.el;if(!d._isJsPlumbGroup){var e=d._jsPlumbGroup;if(e!==t){var f=c.getOffset(d,!0),g=t.collapsed?c.getOffset(u,!0):c.getOffset(w,!0);if(null!=e){if(e.overrideDrop(d,t))return;e.remove(d,!0),b.updateConnectionsForGroup(e)}t.add(d,!0);var h=function(a,c){var d=0==c?1:0;a.each(function(a){a.setVisible(!1),a.endpoints[d].element._jsPlumbGroup===t?(a.endpoints[d].setVisible(!1),b.expandConnection(a,d,t)):(a.endpoints[c].setVisible(!1),b.collapseConnection(a,c,t))})};t.collapsed&&(h(c.select({source:d}),0),h(c.select({target:d}),1));var i=c.getId(d);c.dragManager.setParent(d,i,u,c.getId(u),f),c.setPosition(d,{left:f.left-g.left,top:f.top-g.top}),c.dragManager.revalidateParent(d,i,f),b.updateConnectionsForGroup(t),setTimeout(function(){c.fire(m,{group:t,el:d})},0)}}}});var G=function(a,b){for(var c=null==a.nodeType?a:[a],d=0;d<c.length;d++)b(c[d])};this.overrideDrop=function(){return C&&(z||B||A)},this.add=function(a,b){G(a,function(a){a._jsPlumbGroup=t,E.push(a),c.isAlreadyDraggable(a)&&s(a),a.parentNode!=w&&w.appendChild(a),b||c.fire(m,{group:t,el:a})}),c.getGroupManager().updateConnectionsForGroup(t)},this.remove=function(a,d,e,f){G(a,function(a){if(delete a._jsPlumbGroup,b.removeWithFunction(E,function(b){return b===a}),d)try{t.getEl().removeChild(a)}catch(f){jsPlumbUtil.log(\"Could not remove element from Group \"+f)}r(a),e||c.fire(n,{group:t,el:a})}),f||c.getGroupManager().updateConnectionsForGroup(t)},this.removeAll=function(a,b){for(var d=0,e=E.length;e>d;d++)t.remove(E[0],a,b,!0);E.length=0,c.getGroupManager().updateConnectionsForGroup(t)},this.orphanAll=function(){for(var a=0;a<E.length;a++)o(E[a]);E.length=0},this.getMembers=function(){return E},u[k]=this,c.bind(g,function(a){a.el._jsPlumbGroup==this&&s(a.el)}.bind(this)),this.shouldProxy=function(){return D},c.getGroupManager().addGroup(this)};c.prototype.addGroup=function(a){var b=this;if(b._groups=b._groups||{},null!=b._groups[a.id])throw new TypeError(\"cannot create Group [\"+a.id+\"]; a Group with that ID exists\");if(null!=a.el[k])throw new TypeError(\"cannot create Group [\"+a.id+\"]; the given element is already a Group\");var c=new t(b,a);return b._groups[c.id]=c,c},c.prototype.addToGroup=function(a,b,c){this.getGroupManager().addToGroup(a,b,c)},c.prototype.removeFromGroup=function(a,b,c){this.getGroupManager().removeFromGroup(a,b,c)},c.prototype.removeGroup=function(a,b){this.getGroupManager().removeGroup(a,b)},c.prototype.removeAllGroups=function(a){this.getGroupManager().removeAllGroups(a)},c.prototype.getGroup=function(a){return this.getGroupManager().getGroup(a)},c.prototype.getGroups=function(){return this.getGroupManager().getGroups()},c.prototype.expandGroup=function(a){this.getGroupManager().expandGroup(a)},c.prototype.collapseGroup=function(a){this.getGroupManager().collapseGroup(a)},c.prototype.repaintGroup=function(a){this.getGroupManager().repaintGroup(a)},c.prototype.toggleGroup=function(a){a=this.getGroupManager().getGroup(a),null!=a&&this.getGroupManager()[a.collapsed?\"expandGroup\":\"collapseGroup\"](a)},c.prototype.getGroupManager=function(){var a=this[j];return null==a&&(a=this[j]=new s(this)),a},c.prototype.removeGroupManager=function(){delete this[j]},c.prototype.getGroupFor=function(a){return a=this.getElement(a),a?a[k]:void 0}}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d=function(a){this.type=\"Flowchart\",a=a||{},a.stub=null==a.stub?30:a.stub;var c,d,e=b.Connectors.AbstractConnector.apply(this,arguments),f=null==a.midpoint?.5:a.midpoint,g=a.alwaysRespectStubs===!0,h=null,i=null,j=null!=a.cornerRadius?a.cornerRadius:0,k=(a.loopbackRadius||25,function(a){return 0>a?-1:0===a?0:1}),l=function(a,b,c,d){if(h!=b||i!=c){var e=null==h?d.sx:h,f=null==i?d.sy:i,g=e==b?\"v\":\"h\",j=k(b-e),l=k(c-f);h=b,i=c,a.push([e,f,b,c,g,j,l])}},m=function(a){return Math.sqrt(Math.pow(a[0]-a[2],2)+Math.pow(a[1]-a[3],2))},n=function(a){var b=[];return b.push.apply(b,a),b},o=function(a,b,c){for(var d,f=null,g=0;g<b.length-1;g++){if(f=f||n(b[g]),d=n(b[g+1]),j>0&&f[4]!=d[4]){var h=Math.min(j,m(f),m(d));f[2]-=f[5]*h,f[3]-=f[6]*h,d[0]+=d[5]*h,d[1]+=d[6]*h;var i=f[6]==d[5]&&1==d[5]||f[6]==d[5]&&0===d[5]&&f[5]!=d[6]||f[6]==d[5]&&-1==d[5],k=d[1]>f[3]?1:-1,l=d[0]>f[2]?1:-1,o=k==l,p=o&&i||!o&&!i?d[0]:f[2],q=o&&i||!o&&!i?f[3]:d[1];e.addSegment(a,\"Straight\",{x1:f[0],y1:f[1],x2:f[2],y2:f[3]}),e.addSegment(a,\"Arc\",{r:h,x1:f[2],y1:f[3],x2:d[0],y2:d[1],cx:p,cy:q,ac:i})}else{var r=f[2]==f[0]?0:f[2]>f[0]?c.lw/2:-(c.lw/2),s=f[3]==f[1]?0:f[3]>f[1]?c.lw/2:-(c.lw/2);e.addSegment(a,\"Straight\",{x1:f[0]-r,y1:f[1]-s,x2:f[2]+r,y2:f[3]+s})}f=d}null!=d&&e.addSegment(a,\"Straight\",{x1:d[0],y1:d[1],x2:d[2],y2:d[3]})};this._compute=function(a,b){c=[],h=null,i=null,d=null;var j=function(){return[a.startStubX,a.startStubY,a.endStubX,a.endStubY]},k={perpendicular:j,orthogonal:j,opposite:function(b){var c=a,d=\"x\"==b?0:1,e={x:function(){return 1==c.so[d]&&(c.startStubX>c.endStubX&&c.tx>c.startStubX||c.sx>c.endStubX&&c.tx>c.sx)||-1==c.so[d]&&(c.startStubX<c.endStubX&&c.tx<c.startStubX||c.sx<c.endStubX&&c.tx<c.sx)},y:function(){return 1==c.so[d]&&(c.startStubY>c.endStubY&&c.ty>c.startStubY||c.sy>c.endStubY&&c.ty>c.sy)||-1==c.so[d]&&(c.startStubY<c.endStubY&&c.ty<c.startStubY||c.sy<c.endStubY&&c.ty<c.sy)}};return!g&&e[b]()?{x:[(a.sx+a.tx)/2,a.startStubY,(a.sx+a.tx)/2,a.endStubY],y:[a.startStubX,(a.sy+a.ty)/2,a.endStubX,(a.sy+a.ty)/2]}[b]:[a.startStubX,a.startStubY,a.endStubX,a.endStubY]}},m=k[a.anchorOrientation](a.sourceAxis),n=\"x\"==a.sourceAxis?0:1,p=\"x\"==a.sourceAxis?1:0,q=m[n],r=m[p],s=m[n+2],t=m[p+2];l(c,m[0],m[1],a);var u=a.startStubX+(a.endStubX-a.startStubX)*f,v=a.startStubY+(a.endStubY-a.startStubY)*f,w={x:[0,1],y:[1,0]},x={perpendicular:function(b){var c=a,d={x:[[[1,2,3,4],null,[2,1,4,3]],null,[[4,3,2,1],null,[3,4,1,2]]],y:[[[3,2,1,4],null,[2,3,4,1]],null,[[4,1,2,3],null,[1,4,3,2]]]},e={x:[[c.startStubX,c.endStubX],null,[c.endStubX,c.startStubX]],y:[[c.startStubY,c.endStubY],null,[c.endStubY,c.startStubY]]},f={x:[[u,c.startStubY],[u,c.endStubY]],y:[[c.startStubX,v],[c.endStubX,v]]},g={x:[[c.endStubX,c.startStubY]],y:[[c.startStubX,c.endStubY]]},h={x:[[c.startStubX,c.endStubY],[c.endStubX,c.endStubY]],y:[[c.endStubX,c.startStubY],[c.endStubX,c.endStubY]]},i={x:[[c.startStubX,v],[c.endStubX,v],[c.endStubX,c.endStubY]],y:[[u,c.startStubY],[u,c.endStubY],[c.endStubX,c.endStubY]]},j={x:[c.startStubY,c.endStubY],y:[c.startStubX,c.endStubX]},k=w[b][0],l=w[b][1],m=c.so[k]+1,n=c.to[l]+1,o=-1==c.to[l]&&j[b][1]<j[b][0]||1==c.to[l]&&j[b][1]>j[b][0],p=e[b][m][0],q=e[b][m][1],r=d[b][m][n];return c.segment==r[3]||c.segment==r[2]&&o?f[b]:c.segment==r[2]&&p>q?g[b]:c.segment==r[2]&&q>=p||c.segment==r[1]&&!o?i[b]:c.segment==r[0]||c.segment==r[1]&&o?h[b]:void 0},orthogonal:function(b,c,d,e,f){var g=a,h={x:-1==g.so[0]?Math.min(c,e):Math.max(c,e),y:-1==g.so[1]?Math.min(c,e):Math.max(c,e)}[b];return{x:[[h,d],[h,f],[e,f]],y:[[d,h],[f,h],[f,e]]}[b]},opposite:function(c,d,f,g){var h=a,i={x:\"y\",y:\"x\"}[c],j={x:\"height\",y:\"width\"}[c],k=h[\"is\"+c.toUpperCase()+\"GreaterThanStubTimes2\"];if(b.sourceEndpoint.elementId==b.targetEndpoint.elementId){var l=f+(1-b.sourceEndpoint.anchor[i])*b.sourceInfo[j]+e.maxStub;return{x:[[d,l],[g,l]],y:[[l,d],[l,g]]}[c]}return!k||1==h.so[n]&&d>g||-1==h.so[n]&&g>d?{x:[[d,v],[g,v]],y:[[u,d],[u,g]]}[c]:1==h.so[n]&&g>d||-1==h.so[n]&&d>g?{x:[[u,h.sy],[u,h.ty]],y:[[h.sx,v],[h.tx,v]]}[c]:void 0}},y=x[a.anchorOrientation](a.sourceAxis,q,r,s,t);if(y)for(var z=0;z<y.length;z++)l(c,y[z][0],y[z][1],a);l(c,m[2],m[3],a),l(c,a.tx,a.ty,a),o(this,c,a)}};c.extend(d,b.Connectors.AbstractConnector),b.registerConnectorType(d,\"Flowchart\")}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil;b.Connectors.AbstractBezierConnector=function(a){a=a||{};var c,d=a.showLoopback!==!1,e=(a.curviness||10,a.margin||5),f=(a.proximityLimit||80,a.orientation&&\"clockwise\"===a.orientation),g=a.loopbackRadius||25,h=!1;return this.overrideSetEditable=function(){return!h},this._compute=function(a,b){var i=b.sourcePos,j=b.targetPos,k=Math.abs(i[0]-j[0]),l=Math.abs(i[1]-j[1]);if(d&&b.sourceEndpoint.elementId===b.targetEndpoint.elementId){h=!0;var m=b.sourcePos[0],n=b.sourcePos[1]-e,o=m,p=n-g,q=o-g,r=p-g;k=2*g,l=2*g,a.points[0]=q,a.points[1]=r,a.points[2]=k,a.points[3]=l,c.addSegment(this,\"Arc\",{loopback:!0,x1:m-q+4,y1:n-r,startAngle:0,endAngle:2*Math.PI,r:g,ac:!f,x2:m-q-4,y2:n-r,cx:o-q,cy:p-r})}else h=!1,this._computeBezier(a,b,i,j,k,l)},c=b.Connectors.AbstractConnector.apply(this,arguments)},c.extend(b.Connectors.AbstractBezierConnector,b.Connectors.AbstractConnector);var d=function(a){a=a||{},this.type=\"Bezier\";var c=b.Connectors.AbstractBezierConnector.apply(this,arguments),d=a.curviness||150,e=10;this.getCurviness=function(){return d},this._findControlPoint=function(a,b,c,f,g,h,i){var j=h[0]!=i[0]||h[1]==i[1],k=[];return j?(0===i[0]?k.push(c[0]<b[0]?a[0]+e:a[0]-e):k.push(a[0]+d*i[0]),0===i[1]?k.push(c[1]<b[1]?a[1]+e:a[1]-e):k.push(a[1]+d*h[1])):(0===h[0]?k.push(b[0]<c[0]?a[0]+e:a[0]-e):k.push(a[0]-d*h[0]),0===h[1]?k.push(b[1]<c[1]?a[1]+e:a[1]-e):k.push(a[1]+d*i[1])),k},this._computeBezier=function(a,b,d,e,f,g){var h,i,j=this.getGeometry(),k=d[0]<e[0]?f:0,l=d[1]<e[1]?g:0,m=d[0]<e[0]?0:f,n=d[1]<e[1]?0:g;(this.hasBeenEdited()||this.isEditing())&&null!=j&&null!=j.controlPoints&&null!=j.controlPoints[0]&&null!=j.controlPoints[1]?(h=j.controlPoints[0],i=j.controlPoints[1]):(h=this._findControlPoint([k,l],d,e,b.sourceEndpoint,b.targetEndpoint,a.so,a.to),i=this._findControlPoint([m,n],e,d,b.targetEndpoint,b.sourceEndpoint,a.to,a.so)),c.setGeometry({controlPoints:[h,i]},!0),c.addSegment(this,\"Bezier\",{x1:k,y1:l,x2:m,y2:n,cp1x:h[0],cp1y:h[1],cp2x:i[0],cp2y:i[1]})}};c.extend(d,b.Connectors.AbstractBezierConnector),b.registerConnectorType(d,\"Bezier\")}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d=function(a,b,c,d){return c>=a&&b>=d?1:c>=a&&d>=b?2:a>=c&&d>=b?3:4},e=function(a,b,c,d,e,f,g,h,i){return i>=h?[a,b]:1===c?d[3]<=0&&e[3]>=1?[a+(d[2]<.5?-1*f:f),b]:d[2]>=1&&e[2]<=0?[a,b+(d[3]<.5?-1*g:g)]:[a+-1*f,b+-1*g]:2===c?d[3]>=1&&e[3]<=0?[a+(d[2]<.5?-1*f:f),b]:d[2]>=1&&e[2]<=0?[a,b+(d[3]<.5?-1*g:g)]:[a+f,b+-1*g]:3===c?d[3]>=1&&e[3]<=0?[a+(d[2]<.5?-1*f:f),b]:d[2]<=0&&e[2]>=1?[a,b+(d[3]<.5?-1*g:g)]:[a+-1*f,b+-1*g]:4===c?d[3]<=0&&e[3]>=1?[a+(d[2]<.5?-1*f:f),b]:d[2]<=0&&e[2]>=1?[a,b+(d[3]<.5?-1*g:g)]:[a+f,b+-1*g]:void 0},f=function(a){a=a||{},this.type=\"StateMachine\";var c,f=b.Connectors.AbstractBezierConnector.apply(this,arguments),g=a.curviness||10,h=a.margin||5,i=a.proximityLimit||80;a.orientation&&\"clockwise\"===a.orientation,this._computeBezier=function(a,b,j,k,l,m){var n=b.sourcePos[0]<b.targetPos[0]?0:l,o=b.sourcePos[1]<b.targetPos[1]?0:m,p=b.sourcePos[0]<b.targetPos[0]?l:0,q=b.sourcePos[1]<b.targetPos[1]?m:0;0===b.sourcePos[2]&&(n-=h),1===b.sourcePos[2]&&(n+=h),0===b.sourcePos[3]&&(o-=h),1===b.sourcePos[3]&&(o+=h),0===b.targetPos[2]&&(p-=h),1===b.targetPos[2]&&(p+=h),0===b.targetPos[3]&&(q-=h),1===b.targetPos[3]&&(q+=h);var r,s,t,u,v=(n+p)/2,w=(o+q)/2,x=d(n,o,p,q),y=Math.sqrt(Math.pow(p-n,2)+Math.pow(q-o,2)),z=f.getGeometry();(this.hasBeenEdited()||this.isEditing())&&null!=z?(r=z.controlPoints[0][0],t=z.controlPoints[0][1],s=z.controlPoints[1][0],u=z.controlPoints[1][1]):(c=e(v,w,x,b.sourcePos,b.targetPos,g,g,y,i),r=c[0],s=c[0],t=c[1],u=c[1],f.setGeometry({controlPoints:[c,c]},!0)),f.addSegment(this,\"Bezier\",{x1:p,y1:q,x2:n,y2:o,cp1x:r,cp1y:t,cp2x:s,cp2y:u})}};c.extend(f,b.Connectors.AbstractBezierConnector),b.registerConnectorType(f,\"StateMachine\")}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d={\"stroke-linejoin\":\"stroke-linejoin\",\"stroke-dashoffset\":\"stroke-dashoffset\",\"stroke-linecap\":\"stroke-linecap\"},e=\"stroke-dasharray\",f=\"dashstyle\",g=\"linearGradient\",h=\"radialGradient\",i=\"defs\",j=\"fill\",k=\"stop\",l=\"stroke\",m=\"stroke-width\",n=\"style\",o=\"none\",p=\"jsplumb_gradient_\",q=\"lineWidth\",r={svg:\"http://www.w3.org/2000/svg\",xhtml:\"http://www.w3.org/1999/xhtml\"},s=function(a,b){for(var c in b)a.setAttribute(c,\"\"+b[c])},t=function(a,b){return b=b||{},b.version=\"1.1\",b.xmlns=r.xhtml,jsPlumb.createElementNS(r.svg,a,null,null,b)},u=function(a){return\"position:absolute;left:\"+a[0]+\"px;top:\"+a[1]+\"px\"},v=function(a){for(var b=a.querySelectorAll(\" defs,linearGradient,radialGradient\"),c=0;c<b.length;c++)b[c].parentNode.removeChild(b[c])},w=function(a,b,c,d,e){var f=p+e._jsPlumb.instance.idstamp();v(a);var m;m=c.gradient.offset?t(h,{id:f}):t(g,{id:f,gradientUnits:\"userSpaceOnUse\"});var n=t(i);a.appendChild(n),n.appendChild(m);for(var o=0;o<c.gradient.stops.length;o++){var q=1==e.segment||2==e.segment?o:c.gradient.stops.length-1-o,r=c.gradient.stops[q][1],s=t(k,{offset:Math.floor(100*c.gradient.stops[o][0])+\"%\",\"stop-color\":r});m.appendChild(s)}var u=c.strokeStyle?l:j;b.setAttribute(u,\"url(#\"+f+\")\")},x=function(a,b,c,g,h){if(b.setAttribute(j,c.fillStyle?c.fillStyle:o),b.setAttribute(l,c.strokeStyle?c.strokeStyle:o),c.gradient?w(a,b,c,g,h):(v(a),b.setAttribute(n,\"\")),c.lineWidth&&b.setAttribute(m,c.lineWidth),c[f]&&c[q]&&!c[e]){var i=-1==c[f].indexOf(\",\")?\" \":\",\",k=c[f].split(i),p=\"\";k.forEach(function(a){p+=Math.floor(a*c.lineWidth)+i}),b.setAttribute(e,p)}else c[e]&&b.setAttribute(e,c[e]);for(var r in d)c[r]&&b.setAttribute(d[r],c[r])},y=function(a,b,c){a.childNodes.length>c?a.insertBefore(b,a.childNodes[c]):a.appendChild(b)};c.svg={node:t,attr:s,pos:u};var z=function(a){var d=a.pointerEventsSpec||\"all\",e={};b.jsPlumbUIComponent.apply(this,a.originalArgs),this.canvas=null,this.path=null,this.svg=null,this.bgCanvas=null;var f=a.cssClass+\" \"+(a.originalArgs[0].cssClass||\"\"),g={style:\"\",width:0,height:0,\"pointer-events\":d,position:\"absolute\"};this.svg=t(\"svg\",g),a.useDivWrapper?(this.canvas=jsPlumb.createElement(\"div\",{position:\"absolute\"}),c.sizeElement(this.canvas,0,0,1,1),this.canvas.className=f):(s(this.svg,{\"class\":f}),this.canvas=this.svg),a._jsPlumb.appendElement(this.canvas,a.originalArgs[0].parent),a.useDivWrapper&&this.canvas.appendChild(this.svg);var h=[this.canvas];return this.getDisplayElements=function(){return h},this.appendDisplayElement=function(a){h.push(a)},this.paint=function(b,d,f){if(null!=b){var g,h=[this.x,this.y],i=[this.w,this.h];null!=f&&(f.xmin<0&&(h[0]+=f.xmin),f.ymin<0&&(h[1]+=f.ymin),i[0]=f.xmax+(f.xmin<0?-f.xmin:0),i[1]=f.ymax+(f.ymin<0?-f.ymin:0)),a.useDivWrapper?(c.sizeElement(this.canvas,h[0],h[1],i[0],i[1]),h[0]=0,h[1]=0,g=u([0,0])):g=u([h[0],h[1]]),e.paint.apply(this,arguments),s(this.svg,{style:g,width:i[0]||0,height:i[1]||0})}},{renderer:e}};c.extend(z,b.jsPlumbUIComponent,{cleanup:function(a){a||null==this.typeId?(this.canvas&&(this.canvas._jsPlumb=null),this.svg&&(this.svg._jsPlumb=null),this.bgCanvas&&(this.bgCanvas._jsPlumb=null),this.canvas&&this.canvas.parentNode&&this.canvas.parentNode.removeChild(this.canvas),this.bgCanvas&&this.bgCanvas.parentNode&&this.canvas.parentNode.removeChild(this.canvas),this.svg=null,this.canvas=null,this.path=null,this.group=null):(this.canvas&&this.canvas.parentNode&&this.canvas.parentNode.removeChild(this.canvas),this.bgCanvas&&this.bgCanvas.parentNode&&this.bgCanvas.parentNode.removeChild(this.bgCanvas))},reattach:function(a){var b=a.getContainer();this.canvas&&null==this.canvas.parentNode&&b.appendChild(this.canvas),this.bgCanvas&&null==this.bgCanvas.parentNode&&b.appendChild(this.bgCanvas)},setVisible:function(a){this.canvas&&(this.canvas.style.display=a?\"block\":\"none\")}}),b.ConnectorRenderers.svg=function(a){var c=this,d=z.apply(this,[{cssClass:a._jsPlumb.connectorClass+(this.isEditable()?\" \"+a._jsPlumb.editableConnectorClass:\"\"),originalArgs:arguments,pointerEventsSpec:\"none\",_jsPlumb:a._jsPlumb}]),e=this.setEditable;this.setEditable=function(a){var b=e.apply(this,[a]);jsPlumb[b?\"addClass\":\"removeClass\"](this.canvas,this._jsPlumb.instance.editableConnectorClass)},d.renderer.paint=function(d,e,f){var g=c.getSegments(),h=\"\",i=[0,0];if(f.xmin<0&&(i[0]=-f.xmin),f.ymin<0&&(i[1]=-f.ymin),g.length>0){for(var j=0;j<g.length;j++)h+=b.Segments.svg.SegmentRenderer.getPath(g[j]),h+=\" \";var k={d:h,transform:\"translate(\"+i[0]+\",\"+i[1]+\")\",\"pointer-events\":a[\"pointer-events\"]||\"visibleStroke\"},l=null,m=[c.x,c.y,c.w,c.h];if(d.outlineColor){var n=d.outlineWidth||1,o=d.lineWidth+2*n;l=b.extend({},d),delete l.gradient,l.strokeStyle=d.outlineColor,l.lineWidth=o,null==c.bgPath?(c.bgPath=t(\"path\",k),b.addClass(c.bgPath,b.connectorOutlineClass),y(c.svg,c.bgPath,0)):s(c.bgPath,k),x(c.svg,c.bgPath,l,m,c)}null==c.path?(c.path=t(\"path\",k),y(c.svg,c.path,d.outlineColor?1:0)):s(c.path,k),x(c.svg,c.path,d,m,c)}}},c.extend(b.ConnectorRenderers.svg,z),b.Segments.svg={SegmentRenderer:{getPath:function(a){return{Straight:function(){var b=a.getCoordinates();return\"M \"+b.x1+\" \"+b.y1+\" L \"+b.x2+\" \"+b.y2},Bezier:function(){var b=a.params;return\"M \"+b.x1+\" \"+b.y1+\" C \"+b.cp1x+\" \"+b.cp1y+\" \"+b.cp2x+\" \"+b.cp2y+\" \"+b.x2+\" \"+b.y2},Arc:function(){var b=a.params,c=a.sweep>Math.PI?1:0,d=a.anticlockwise?0:1;return\"M\"+a.x1+\" \"+a.y1+\" A \"+a.radius+\" \"+b.r+\" 0 \"+c+\",\"+d+\" \"+a.x2+\" \"+a.y2}}[a.type]()}}};var A=b.SvgEndpoint=function(a){var c=z.apply(this,[{cssClass:a._jsPlumb.endpointClass,originalArgs:arguments,pointerEventsSpec:\"all\",useDivWrapper:!0,_jsPlumb:a._jsPlumb}]);c.renderer.paint=function(a){var c=b.extend({},a);c.outlineColor&&(c.strokeWidth=c.outlineWidth,c.strokeStyle=c.outlineColor),null==this.node?(this.node=this.makeNode(c),this.svg.appendChild(this.node)):null!=this.updateNode&&this.updateNode(this.node),x(this.svg,this.node,c,[this.x,this.y,this.w,this.h],this),u(this.node,[this.x,this.y])}.bind(this)};c.extend(A,z),b.Endpoints.svg.Dot=function(){b.Endpoints.Dot.apply(this,arguments),A.apply(this,arguments),this.makeNode=function(){return t(\"circle\",{cx:this.w/2,cy:this.h/2,r:this.radius})},this.updateNode=function(a){s(a,{cx:this.w/2,cy:this.h/2,r:this.radius})}},c.extend(b.Endpoints.svg.Dot,[b.Endpoints.Dot,A]),b.Endpoints.svg.Rectangle=function(){b.Endpoints.Rectangle.apply(this,arguments),A.apply(this,arguments),this.makeNode=function(){return t(\"rect\",{width:this.w,height:this.h})},this.updateNode=function(a){s(a,{width:this.w,height:this.h})}},c.extend(b.Endpoints.svg.Rectangle,[b.Endpoints.Rectangle,A]),b.Endpoints.svg.Image=b.Endpoints.Image,b.Endpoints.svg.Blank=b.Endpoints.Blank,b.Overlays.svg.Label=b.Overlays.Label,b.Overlays.svg.Custom=b.Overlays.Custom;var B=function(a,c){a.apply(this,c),b.jsPlumbUIComponent.apply(this,c),this.isAppendedAtTopLevel=!1,this.path=null,this.paint=function(a,b){if(a.component.svg&&b){null==this.path&&(this.path=t(\"path\",{\"pointer-events\":\"all\"}),a.component.svg.appendChild(this.path),this.elementCreated&&this.elementCreated(this.path,a.component),this.canvas=a.component.svg);var e=c&&1==c.length?c[0].cssClass||\"\":\"\",f=[0,0];b.xmin<0&&(f[0]=-b.xmin),b.ymin<0&&(f[1]=-b.ymin),s(this.path,{d:d(a.d),\"class\":e,stroke:a.strokeStyle?a.strokeStyle:null,fill:a.fillStyle?a.fillStyle:null,transform:\"translate(\"+f[0]+\",\"+f[1]+\")\"})}};var d=function(a){return isNaN(a.cxy.x)||isNaN(a.cxy.y)?\"\":\"M\"+a.hxy.x+\",\"+a.hxy.y+\" L\"+a.tail[0].x+\",\"+a.tail[0].y+\" L\"+a.cxy.x+\",\"+a.cxy.y+\" L\"+a.tail[1].x+\",\"+a.tail[1].y+\" L\"+a.hxy.x+\",\"+a.hxy.y};this.transfer=function(a){a.canvas&&this.path&&this.path.parentNode&&(this.path.parentNode.removeChild(this.path),a.canvas.appendChild(this.path))}};c.extend(B,[b.jsPlumbUIComponent,b.Overlays.AbstractOverlay],{cleanup:function(a){null!=this.path&&(a?this._jsPlumb.instance.removeElement(this.path):this.path.parentNode&&this.path.parentNode.removeChild(this.path))},reattach:function(){this.path&&this.canvas&&null==this.path.parentNode&&this.canvas.appendChild(this.path)},setVisible:function(a){null!=this.path&&(this.path.style.display=a?\"block\":\"none\")}}),b.Overlays.svg.Arrow=function(){B.apply(this,[b.Overlays.Arrow,arguments])},c.extend(b.Overlays.svg.Arrow,[b.Overlays.Arrow,B]),b.Overlays.svg.PlainArrow=function(){B.apply(this,[b.Overlays.PlainArrow,arguments])},c.extend(b.Overlays.svg.PlainArrow,[b.Overlays.PlainArrow,B]),b.Overlays.svg.Diamond=function(){B.apply(this,[b.Overlays.Diamond,arguments])},c.extend(b.Overlays.svg.Diamond,[b.Overlays.Diamond,B]),b.Overlays.svg.GuideLines=function(){var a,c,d=null,e=this;b.Overlays.GuideLines.apply(this,arguments),this.paint=function(b,g){null==d&&(d=t(\"path\"),b.connector.svg.appendChild(d),e.attachListeners(d,b.connector),e.attachListeners(d,e),a=t(\"path\"),b.connector.svg.appendChild(a),e.attachListeners(a,b.connector),e.attachListeners(a,e),c=t(\"path\"),b.connector.svg.appendChild(c),e.attachListeners(c,b.connector),e.attachListeners(c,e));var h=[0,0];g.xmin<0&&(h[0]=-g.xmin),g.ymin<0&&(h[1]=-g.ymin),s(d,{d:f(b.head,b.tail),stroke:\"red\",fill:null,transform:\"translate(\"+h[0]+\",\"+h[1]+\")\"}),s(a,{d:f(b.tailLine[0],b.tailLine[1]),stroke:\"blue\",fill:null,transform:\"translate(\"+h[0]+\",\"+h[1]+\")\"}),s(c,{d:f(b.headLine[0],b.headLine[1]),stroke:\"green\",fill:null,transform:\"translate(\"+h[0]+\",\"+h[1]+\")\"})\n};var f=function(a,b){return\"M \"+a.x+\",\"+a.y+\" L\"+b.x+\",\"+b.y}},c.extend(b.Overlays.svg.GuideLines,b.Overlays.GuideLines)}.call(\"undefined\"!=typeof window?window:this),function(){\"use strict\";var a=this,b=a.jsPlumb,c=a.jsPlumbUtil,d=a.Katavorio,e=a.Biltong,f=function(a,b){b=b||\"main\";var c=\"_katavorio_\"+b,f=a[c],g=a.getEventManager();return f||(f=new d({bind:g.on,unbind:g.off,getSize:jsPlumb.getSize,getPosition:function(b,c){var d=a.getOffset(b,c,b._katavorioDrag?b.offsetParent:null);return[d.left,d.top]},setPosition:function(a,b){a.style.left=b[0]+\"px\",a.style.top=b[1]+\"px\"},addClass:jsPlumb.addClass,removeClass:jsPlumb.removeClass,intersects:e.intersects,indexOf:function(a,b){return a.indexOf(b)},css:{noSelect:a.dragSelectClass,droppable:\"jsplumb-droppable\",draggable:\"jsplumb-draggable\",drag:\"jsplumb-drag\",selected:\"jsplumb-drag-selected\",active:\"jsplumb-drag-active\",hover:\"jsplumb-drag-hover\",ghostProxy:\"jsplumb-ghost-proxy\"}}),a[c]=f,a.bind(\"zoom\",f.setZoom)),f},g=function(a,b){var d=function(d){if(null!=b[d]){if(c.isString(b[d])){var e=b[d].match(/-=/)?-1:1,f=b[d].substring(2);return a[d]+e*f}return b[d]}return a[d]};return[d(\"left\"),d(\"top\")]};b.extend(a.jsPlumbInstance.prototype,{animationSupported:!0,getElement:function(a){return null==a?null:(a=\"string\"==typeof a?a:null!=a.length&&null==a.enctype?a[0]:a,\"string\"==typeof a?document.getElementById(a):a)},removeElement:function(a){f(this).elementRemoved(a),this.getEventManager().remove(a)},doAnimate:function(a,b,c){c=c||{};var d=this.getOffset(a),e=g(d,b),f=e[0]-d.left,h=e[1]-d.top,i=c.duration||250,j=15,k=i/j,l=j/i*f,m=j/i*h,n=0,o=setInterval(function(){jsPlumb.setPosition(a,{left:d.left+l*(n+1),top:d.top+m*(n+1)}),null!=c.step&&c.step(n,Math.ceil(k)),n++,n>=k&&(window.clearInterval(o),null!=c.complete&&c.complete())},j)},destroyDraggable:function(a,b){f(this,b).destroyDraggable(a)},destroyDroppable:function(a,b){f(this,b).destroyDroppable(a)},initDraggable:function(a,b,c){f(this,c).draggable(a,b)},initDroppable:function(a,b,c){f(this,c).droppable(a,b)},isAlreadyDraggable:function(a){return null!=a._katavorioDrag},isDragSupported:function(){return!0},isDropSupported:function(){return!0},isElementDraggable:function(a){return a=jsPlumb.getElement(a),a._katavorioDrag&&a._katavorioDrag.isEnabled()},getDragObject:function(a){return a[0].drag.getDragElement()},getDragScope:function(a){return a._katavorioDrag&&a._katavorioDrag.scopes.join(\" \")||\"\"},getDropEvent:function(a){return a[0].e},getUIPosition:function(a){var b=a[0].el;if(null==b.offsetParent)return null;var c=a[0].finalPos||a[0].pos,d={left:c[0],top:c[1]};if(b._katavorioDrag&&b.offsetParent!==this.getContainer()){var e=this.getOffset(b.offsetParent);d.left+=e.left,d.top+=e.top}return d},setDragFilter:function(a,b,c){a._katavorioDrag&&a._katavorioDrag.setFilter(b,c)},setElementDraggable:function(a,b){a=jsPlumb.getElement(a),a._katavorioDrag&&a._katavorioDrag.setEnabled(b)},setDragScope:function(a,b){a._katavorioDrag&&a._katavorioDrag.k.setDragScope(a,b)},setDropScope:function(a,b){a._katavorioDrop&&a._katavorioDrop.length>0&&a._katavorioDrop[0].k.setDropScope(a,b)},addToPosse:function(a){var b=Array.prototype.slice.call(arguments,1),c=f(this);jsPlumb.each(a,function(a){a=[jsPlumb.getElement(a)],a.push.apply(a,b),c.addToPosse.apply(c,a)})},setPosse:function(a){var b=Array.prototype.slice.call(arguments,1),c=f(this);jsPlumb.each(a,function(a){a=[jsPlumb.getElement(a)],a.push.apply(a,b),c.setPosse.apply(c,a)})},removeFromPosse:function(a){var b=Array.prototype.slice.call(arguments,1),c=f(this);jsPlumb.each(a,function(a){a=[jsPlumb.getElement(a)],a.push.apply(a,b),c.removeFromPosse.apply(c,a)})},removeFromAllPosses:function(a){var b=f(this);jsPlumb.each(a,function(a){b.removeFromAllPosses(jsPlumb.getElement(a))})},setPosseState:function(a,b,c){var d=f(this);jsPlumb.each(a,function(a){d.setPosseState(jsPlumb.getElement(a),b,c)})},dragEvents:{start:\"start\",stop:\"stop\",drag:\"drag\",step:\"step\",over:\"over\",out:\"out\",drop:\"drop\",complete:\"complete\",beforeStart:\"beforeStart\"},animEvents:{step:\"step\",complete:\"complete\"},stopDrag:function(a){a._katavorioDrag&&a._katavorioDrag.abort()},addToDragSelection:function(a){f(this).select(a)},removeFromDragSelection:function(a){f(this).deselect(a)},clearDragSelection:function(){f(this).deselectAll()},trigger:function(a,b,c,d){this.getEventManager().trigger(a,b,c,d)},doReset:function(){for(var a in this)0===a.indexOf(\"_katavorio_\")&&this[a].reset()}});var h=function(a){var b=function(){/complete|loaded|interactive/.test(document.readyState)&&\"undefined\"!=typeof document.body&&null!=document.body?a():setTimeout(b,9)};b()};h(b.init)}.call(\"undefined\"!=typeof window?window:this);"

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


/***/ }),

/***/ "./src/app/visual-query/add-explorer/add-explorer.component.scss":
/*!***********************************************************************!*\
  !*** ./src/app/visual-query/add-explorer/add-explorer.component.scss ***!
  \***********************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".mat-slide-toggle {\n  margin: 6px;\n}\n\n.collapsible {\n  display: flex;\n  align-items: center;\n  cursor: pointer;\n  overflow: hidden;\n  white-space: nowrap;\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none;\n  padding: 3px 0;\n}\n\n.collapsible:hover {\n  background-color: #2a2d2e;\n}\n\n.collapsible .mat-icon {\n  width: 18px;\n  height: 18px;\n  font-size: 18px;\n}\n\n.list .list-item {\n  cursor: pointer;\n  padding: 4px 0 4px 24px;\n  overflow: hidden;\n  white-space: nowrap;\n  text-overflow: ellipsis;\n}\n\n.list .list-item:hover {\n  background-color: #2a2d2e;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC92aXN1YWwtcXVlcnkvYWRkLWV4cGxvcmVyL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFx2aXN1YWwtcXVlcnlcXGFkZC1leHBsb3JlclxcYWRkLWV4cGxvcmVyLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL3Zpc3VhbC1xdWVyeS9hZGQtZXhwbG9yZXIvYWRkLWV4cGxvcmVyLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsV0FBQTtBQ0NGOztBREVBO0VBQ0UsYUFBQTtFQUNBLG1CQUFBO0VBQ0EsZUFBQTtFQUNBLGdCQUFBO0VBQ0EsbUJBQUE7RUFDQSx5QkFBQTtLQUFBLHNCQUFBO01BQUEscUJBQUE7VUFBQSxpQkFBQTtFQUNBLGNBQUE7QUNDRjs7QURDRTtFQUNFLHlCQUFBO0FDQ0o7O0FERUU7RUFDRSxXQUFBO0VBQ0EsWUFBQTtFQUNBLGVBQUE7QUNBSjs7QURJQTtFQUNFLGVBQUE7RUFDQSx1QkFBQTtFQUNBLGdCQUFBO0VBQ0EsbUJBQUE7RUFDQSx1QkFBQTtBQ0RGOztBREdFO0VBQ0UseUJBQUE7QUNESiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvdmlzdWFsLXF1ZXJ5L2FkZC1leHBsb3Jlci9hZGQtZXhwbG9yZXIuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIubWF0LXNsaWRlLXRvZ2dsZSB7XHJcbiAgbWFyZ2luOiA2cHg7XHJcbn1cclxuXHJcbi5jb2xsYXBzaWJsZSB7XHJcbiAgZGlzcGxheTogZmxleDtcclxuICBhbGlnbi1pdGVtczogY2VudGVyO1xyXG4gIGN1cnNvcjogcG9pbnRlcjtcclxuICBvdmVyZmxvdzogaGlkZGVuO1xyXG4gIHdoaXRlLXNwYWNlOiBub3dyYXA7XHJcbiAgdXNlci1zZWxlY3Q6IG5vbmU7XHJcbiAgcGFkZGluZzogM3B4IDA7XHJcblxyXG4gICY6aG92ZXIge1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogcmdiKDQyLCA0NSwgNDYpO1xyXG4gIH1cclxuXHJcbiAgLm1hdC1pY29uIHtcclxuICAgIHdpZHRoOiAxOHB4O1xyXG4gICAgaGVpZ2h0OiAxOHB4O1xyXG4gICAgZm9udC1zaXplOiAxOHB4O1xyXG4gIH1cclxufVxyXG5cclxuLmxpc3QgLmxpc3QtaXRlbSB7XHJcbiAgY3Vyc29yOiBwb2ludGVyO1xyXG4gIHBhZGRpbmc6IDRweCAwIDRweCAyNHB4O1xyXG4gIG92ZXJmbG93OiBoaWRkZW47XHJcbiAgd2hpdGUtc3BhY2U6IG5vd3JhcDtcclxuICB0ZXh0LW92ZXJmbG93OiBlbGxpcHNpcztcclxuXHJcbiAgJjpob3ZlciB7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2IoNDIsIDQ1LCA0Nik7XHJcbiAgfVxyXG59XHJcbiIsIi5tYXQtc2xpZGUtdG9nZ2xlIHtcbiAgbWFyZ2luOiA2cHg7XG59XG5cbi5jb2xsYXBzaWJsZSB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XG4gIGN1cnNvcjogcG9pbnRlcjtcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcbiAgd2hpdGUtc3BhY2U6IG5vd3JhcDtcbiAgdXNlci1zZWxlY3Q6IG5vbmU7XG4gIHBhZGRpbmc6IDNweCAwO1xufVxuLmNvbGxhcHNpYmxlOmhvdmVyIHtcbiAgYmFja2dyb3VuZC1jb2xvcjogIzJhMmQyZTtcbn1cbi5jb2xsYXBzaWJsZSAubWF0LWljb24ge1xuICB3aWR0aDogMThweDtcbiAgaGVpZ2h0OiAxOHB4O1xuICBmb250LXNpemU6IDE4cHg7XG59XG5cbi5saXN0IC5saXN0LWl0ZW0ge1xuICBjdXJzb3I6IHBvaW50ZXI7XG4gIHBhZGRpbmc6IDRweCAwIDRweCAyNHB4O1xuICBvdmVyZmxvdzogaGlkZGVuO1xuICB3aGl0ZS1zcGFjZTogbm93cmFwO1xuICB0ZXh0LW92ZXJmbG93OiBlbGxpcHNpcztcbn1cbi5saXN0IC5saXN0LWl0ZW06aG92ZXIge1xuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMmEyZDJlO1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/visual-query/add-explorer/add-explorer.component.ts":
/*!*********************************************************************!*\
  !*** ./src/app/visual-query/add-explorer/add-explorer.component.ts ***!
  \*********************************************************************/
/*! exports provided: AddExplorerComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AddExplorerComponent", function() { return AddExplorerComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _add_explorer_helpers__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./add-explorer.helpers */ "./src/app/visual-query/add-explorer/add-explorer.helpers.ts");



var AddExplorerComponent = /** @class */ (function () {
    function AddExplorerComponent() {
        this.addSelectedDataSource = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.difficulty = {
            default: 100,
            advanced: 200,
        };
        this.activeDiff = this.difficulty.default;
        this.toggledItems = [];
    }
    AddExplorerComponent.prototype.ngOnInit = function () {
    };
    AddExplorerComponent.prototype.ngOnChanges = function (changes) {
        var _a;
        if ((_a = changes.dataSources) === null || _a === void 0 ? void 0 : _a.currentValue) {
            this.sorted = Object(_add_explorer_helpers__WEBPACK_IMPORTED_MODULE_2__["filterAndSortDataSources"])(this.dataSources, this.activeDiff);
        }
    };
    AddExplorerComponent.prototype.onDifficultyChanged = function (event) {
        this.activeDiff = event.checked ? this.difficulty.advanced : this.difficulty.default;
        this.sorted = Object(_add_explorer_helpers__WEBPACK_IMPORTED_MODULE_2__["filterAndSortDataSources"])(this.dataSources, this.activeDiff);
    };
    AddExplorerComponent.prototype.addDataSource = function (dataSource) {
        this.addSelectedDataSource.emit(dataSource);
    };
    AddExplorerComponent.prototype.toggleItem = function (item) {
        Object(_add_explorer_helpers__WEBPACK_IMPORTED_MODULE_2__["toggleInArray"])(item, this.toggledItems);
    };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Array)
    ], AddExplorerComponent.prototype, "dataSources", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], AddExplorerComponent.prototype, "addSelectedDataSource", void 0);
    AddExplorerComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-add-explorer',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./add-explorer.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/add-explorer/add-explorer.component.html")).default,
            changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectionStrategy"].OnPush,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./add-explorer.component.scss */ "./src/app/visual-query/add-explorer/add-explorer.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [])
    ], AddExplorerComponent);
    return AddExplorerComponent;
}());



/***/ }),

/***/ "./src/app/visual-query/add-explorer/add-explorer.helpers.ts":
/*!*******************************************************************!*\
  !*** ./src/app/visual-query/add-explorer/add-explorer.helpers.ts ***!
  \*******************************************************************/
/*! exports provided: filterAndSortDataSources, toggleInArray */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "filterAndSortDataSources", function() { return filterAndSortDataSources; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "toggleInArray", function() { return toggleInArray; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! lodash-es/cloneDeep */ "../../node_modules/lodash-es/cloneDeep.js");


function filterAndSortDataSources(dataSources, maxDifficulty) {
    var e_1, _a;
    var cloned = Object(lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_1__["default"])(dataSources);
    var filtered = cloned.filter(function (dataSource) {
        return (dataSource.Difficulty <= maxDifficulty) && (dataSource.allowNew == null);
    });
    filtered.sort(function (a, b) { return a.Name.toLocaleLowerCase().localeCompare(b.Name.toLocaleLowerCase()); });
    var sorted = {};
    try {
        for (var filtered_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(filtered), filtered_1_1 = filtered_1.next(); !filtered_1_1.done; filtered_1_1 = filtered_1.next()) {
            var dataSource = filtered_1_1.value;
            var type = dataSource.PrimaryType;
            sorted[type] ? sorted[type].push(dataSource) : sorted[type] = [dataSource];
        }
    }
    catch (e_1_1) { e_1 = { error: e_1_1 }; }
    finally {
        try {
            if (filtered_1_1 && !filtered_1_1.done && (_a = filtered_1.return)) _a.call(filtered_1);
        }
        finally { if (e_1) throw e_1.error; }
    }
    return sorted;
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

/***/ "./src/app/visual-query/plumb-editor/plumb-editor.component.scss":
/*!***********************************************************************!*\
  !*** ./src/app/visual-query/plumb-editor/plumb-editor.component.scss ***!
  \***********************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = ("\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvdmlzdWFsLXF1ZXJ5L3BsdW1iLWVkaXRvci9wbHVtYi1lZGl0b3IuY29tcG9uZW50LnNjc3MifQ== */");

/***/ }),

/***/ "./src/app/visual-query/plumb-editor/plumb-editor.component.ts":
/*!*********************************************************************!*\
  !*** ./src/app/visual-query/plumb-editor/plumb-editor.component.ts ***!
  \*********************************************************************/
/*! exports provided: PlumbEditorComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PlumbEditorComponent", function() { return PlumbEditorComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _services_query_definition_service__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../services/query-definition.service */ "./src/app/visual-query/services/query-definition.service.ts");
/* harmony import */ var _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../services/plumb-gui.service */ "./src/app/visual-query/services/plumb-gui.service.ts");




var PlumbEditorComponent = /** @class */ (function () {
    function PlumbEditorComponent(queryDefinitionService, plumbGuiService) {
        this.queryDefinitionService = queryDefinitionService;
        this.plumbGuiService = plumbGuiService;
        this.save = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.editDataSourcePart = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.instanceChanged = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
    }
    PlumbEditorComponent.prototype.ngOnInit = function () {
    };
    PlumbEditorComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        // executed first because <app-plumb-editor> has *ngIf on it and *ngFor for dataSourceElements won't have changes at first init
        jsPlumb.ready(function () { _this.jsPlumbReady(); });
        // https://stackoverflow.com/questions/37087864/execute-a-function-when-ngfor-finished-in-angular-2/37088348#37088348
        // executed when queryDef changes value
        this.dataSourceElements.changes.subscribe(function (elements) {
            _this.instance.reset();
            _this.plumbGuiService.connectionsInitialized = false;
            jsPlumb.ready(function () { _this.jsPlumbReady(); });
        });
    };
    PlumbEditorComponent.prototype.jsPlumbReady = function () {
        var _this = this;
        this.instance = this.plumbGuiService.buildInstance(this.queryDef);
        this.instanceChanged.emit(this.instance);
        if (this.plumbGuiService.connectionsInitialized) {
            return;
        }
        this.dataSourceElements.forEach(function (dsEl) {
            var dataSource = _this.plumbGuiService.findDataSourceOfElement(dsEl.nativeElement, _this.queryDef);
            _this.makeDataSource(dataSource, dsEl.nativeElement);
        });
        this.instance.batch(function () { _this.plumbGuiService.initWirings(_this.queryDef, _this.instance); }); // suspend drawing and initialise
        this.instance.repaintEverything(); // repaint so continuous connections are aligned correctly
        this.plumbGuiService.connectionsInitialized = true;
    };
    PlumbEditorComponent.prototype.makeDataSource = function (dataSource, element) {
        var _this = this;
        this.plumbGuiService.makeSource(dataSource, element, function (draggedWrapper) { _this.dataSourceDrag(draggedWrapper); }, this.queryDef, this.instance);
        this.queryDef.dsCount++;
    };
    PlumbEditorComponent.prototype.dataSourceDrag = function (draggedWrapper) {
        var offset = this.getElementOffset(draggedWrapper.el);
        var dataSource = this.plumbGuiService.findDataSourceOfElement(draggedWrapper.el, this.queryDef);
        dataSource.VisualDesignerData.Top = Math.round(offset.top);
        dataSource.VisualDesignerData.Left = Math.round(offset.left);
    };
    PlumbEditorComponent.prototype.configureDataSource = function (dataSource) {
        if (dataSource.ReadOnly) {
            return;
        }
        // Ensure dataSource Entity is saved
        if (dataSource.EntityGuid.includes('unsaved')) {
            this.save.emit();
        }
        else {
            this.editDataSourcePart.emit(dataSource);
        }
    };
    PlumbEditorComponent.prototype.typeInfo = function (dataSource) {
        var typeInfo = this.queryDefinitionService.dsTypeInfo(dataSource, this.queryDef);
        return typeInfo;
    };
    PlumbEditorComponent.prototype.typeNameFilter = function (input, format) {
        var filtered = this.queryDefinitionService.typeNameFilter(input, format);
        return filtered;
    };
    PlumbEditorComponent.prototype.remove = function (index) {
        var dataSource = this.queryDef.data.DataSources[index];
        if (!confirm("Delete DataSource " + (dataSource.Name || '(unnamed)') + "?")) {
            return;
        }
        var elementId = _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_3__["dataSrcIdPrefix"] + dataSource.EntityGuid;
        this.instance.selectEndpoints({ element: elementId }).remove();
        this.queryDef.data.DataSources.splice(index, 1);
    };
    PlumbEditorComponent.prototype.editName = function (dataSource) {
        var _a;
        if (dataSource.ReadOnly) {
            return;
        }
        var newName = (_a = prompt('Rename DataSource', dataSource.Name)) === null || _a === void 0 ? void 0 : _a.trim();
        if (newName != null && newName !== '') {
            dataSource.Name = newName;
        }
    };
    PlumbEditorComponent.prototype.editDescription = function (dataSource) {
        var _a;
        if (dataSource.ReadOnly) {
            return;
        }
        var newDescription = (_a = prompt('Edit Description', dataSource.Description)) === null || _a === void 0 ? void 0 : _a.trim();
        if (newDescription != null) {
            dataSource.Description = newDescription;
        }
    };
    PlumbEditorComponent.prototype.getElementOffset = function (element) {
        var container = document.getElementById('pipelineContainer');
        var containerBox = container.getBoundingClientRect();
        var box = element.getBoundingClientRect();
        var top = box.top + container.scrollTop - containerBox.top;
        var left = box.left + container.scrollLeft - containerBox.left;
        return { top: top, left: left };
    };
    PlumbEditorComponent.ctorParameters = function () { return [
        { type: _services_query_definition_service__WEBPACK_IMPORTED_MODULE_2__["QueryDefinitionService"] },
        { type: _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_3__["PlumbGuiService"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], PlumbEditorComponent.prototype, "queryDef", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], PlumbEditorComponent.prototype, "save", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], PlumbEditorComponent.prototype, "editDataSourcePart", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], PlumbEditorComponent.prototype, "instanceChanged", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ViewChildren"])('dataSourceElement'),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", _angular_core__WEBPACK_IMPORTED_MODULE_1__["QueryList"])
    ], PlumbEditorComponent.prototype, "dataSourceElements", void 0);
    PlumbEditorComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-plumb-editor',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./plumb-editor.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/plumb-editor/plumb-editor.component.html")).default,
            changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectionStrategy"].OnPush,
            styles: [":host {display: block; width: 100%; height: 100%}", Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./plumb-editor.component.scss */ "./src/app/visual-query/plumb-editor/plumb-editor.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_services_query_definition_service__WEBPACK_IMPORTED_MODULE_2__["QueryDefinitionService"], _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_3__["PlumbGuiService"]])
    ], PlumbEditorComponent);
    return PlumbEditorComponent;
}());



/***/ }),

/***/ "./src/app/visual-query/query-result/query-result.component.scss":
/*!***********************************************************************!*\
  !*** ./src/app/visual-query/query-result/query-result.component.scss ***!
  \***********************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".description {\n  font-size: 14px;\n}\n\npre {\n  display: block;\n  padding: 9.5px;\n  margin: 0 0 10px;\n  font-size: 13px;\n  line-height: 1.42857143;\n  color: #333;\n  word-break: break-all;\n  word-wrap: break-word;\n  background-color: #f5f5f5;\n  border: 1px solid #ccc;\n  border-radius: 4px;\n  overflow-x: auto;\n}\n\nth {\n  text-align: left;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC92aXN1YWwtcXVlcnkvcXVlcnktcmVzdWx0L0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFx2aXN1YWwtcXVlcnlcXHF1ZXJ5LXJlc3VsdFxccXVlcnktcmVzdWx0LmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL3Zpc3VhbC1xdWVyeS9xdWVyeS1yZXN1bHQvcXVlcnktcmVzdWx0LmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsZUFBQTtBQ0NGOztBREVBO0VBQ0UsY0FBQTtFQUNBLGNBQUE7RUFDQSxnQkFBQTtFQUNBLGVBQUE7RUFDQSx1QkFBQTtFQUNBLFdBQUE7RUFDQSxxQkFBQTtFQUNBLHFCQUFBO0VBQ0EseUJBQUE7RUFDQSxzQkFBQTtFQUNBLGtCQUFBO0VBQ0EsZ0JBQUE7QUNDRjs7QURFQTtFQUNFLGdCQUFBO0FDQ0YiLCJmaWxlIjoicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL3Zpc3VhbC1xdWVyeS9xdWVyeS1yZXN1bHQvcXVlcnktcmVzdWx0LmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLmRlc2NyaXB0aW9uIHtcclxuICBmb250LXNpemU6IDE0cHg7XHJcbn1cclxuXHJcbnByZSB7XHJcbiAgZGlzcGxheTogYmxvY2s7XHJcbiAgcGFkZGluZzogOS41cHg7XHJcbiAgbWFyZ2luOiAwIDAgMTBweDtcclxuICBmb250LXNpemU6IDEzcHg7XHJcbiAgbGluZS1oZWlnaHQ6IDEuNDI4NTcxNDM7XHJcbiAgY29sb3I6ICMzMzM7XHJcbiAgd29yZC1icmVhazogYnJlYWstYWxsO1xyXG4gIHdvcmQtd3JhcDogYnJlYWstd29yZDtcclxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZjVmNWY1O1xyXG4gIGJvcmRlcjogMXB4IHNvbGlkICNjY2M7XHJcbiAgYm9yZGVyLXJhZGl1czogNHB4O1xyXG4gIG92ZXJmbG93LXg6IGF1dG87XHJcbn1cclxuXHJcbnRoIHtcclxuICB0ZXh0LWFsaWduOiBsZWZ0O1xyXG59XHJcbiIsIi5kZXNjcmlwdGlvbiB7XG4gIGZvbnQtc2l6ZTogMTRweDtcbn1cblxucHJlIHtcbiAgZGlzcGxheTogYmxvY2s7XG4gIHBhZGRpbmc6IDkuNXB4O1xuICBtYXJnaW46IDAgMCAxMHB4O1xuICBmb250LXNpemU6IDEzcHg7XG4gIGxpbmUtaGVpZ2h0OiAxLjQyODU3MTQzO1xuICBjb2xvcjogIzMzMztcbiAgd29yZC1icmVhazogYnJlYWstYWxsO1xuICB3b3JkLXdyYXA6IGJyZWFrLXdvcmQ7XG4gIGJhY2tncm91bmQtY29sb3I6ICNmNWY1ZjU7XG4gIGJvcmRlcjogMXB4IHNvbGlkICNjY2M7XG4gIGJvcmRlci1yYWRpdXM6IDRweDtcbiAgb3ZlcmZsb3cteDogYXV0bztcbn1cblxudGgge1xuICB0ZXh0LWFsaWduOiBsZWZ0O1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/visual-query/query-result/query-result.component.ts":
/*!*********************************************************************!*\
  !*** ./src/app/visual-query/query-result/query-result.component.ts ***!
  \*********************************************************************/
/*! exports provided: QueryResultComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "QueryResultComponent", function() { return QueryResultComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");



var QueryResultComponent = /** @class */ (function () {
    function QueryResultComponent(data, dialogRef) {
        this.data = data;
        this.dialogRef = dialogRef;
    }
    QueryResultComponent.prototype.ngOnInit = function () {
        this.testParameters = this.data.testParameters;
        this.timeUsed = this.data.result.QueryTimer.Milliseconds;
        this.ticksUsed = this.data.result.QueryTimer.Ticks;
        this.result = this.data.result.Query;
        this.sources = this.data.result.Sources;
        this.streams = this.data.result.Streams;
    };
    QueryResultComponent.prototype.closeDialog = function () {
        this.dialogRef.close();
    };
    QueryResultComponent.ctorParameters = function () { return [
        { type: undefined, decorators: [{ type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["Inject"], args: [_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MAT_DIALOG_DATA"],] }] },
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"] }
    ]; };
    QueryResultComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-query-result',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./query-result.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/query-result/query-result.component.html")).default,
            changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectionStrategy"].OnPush,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./query-result.component.scss */ "./src/app/visual-query/query-result/query-result.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__param"])(0, Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Inject"])(_angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MAT_DIALOG_DATA"])),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [Object, _angular_material_dialog__WEBPACK_IMPORTED_MODULE_2__["MatDialogRef"]])
    ], QueryResultComponent);
    return QueryResultComponent;
}());



/***/ }),

/***/ "./src/app/visual-query/run-explorer/run-explorer.component.scss":
/*!***********************************************************************!*\
  !*** ./src/app/visual-query/run-explorer/run-explorer.component.scss ***!
  \***********************************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".wrapper {\n  padding: 24px;\n}\n\n.actions {\n  display: flex;\n  flex-direction: column;\n  padding-bottom: 32px;\n}\n\n.actions .action:not(:last-of-type) {\n  margin-bottom: 8px;\n}\n\n.parameters,\n.warnings,\n.description {\n  margin: 16px 0;\n}\n\n.parameters .title,\n.warnings .title,\n.description .title {\n  height: 40px;\n  display: flex;\n  align-items: center;\n  justify-content: space-between;\n  font-size: 14px;\n}\n\n.parameters .title .mat-icon,\n.warnings .title .mat-icon,\n.description .title .mat-icon {\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none;\n}\n\n.parameters .values,\n.warnings .values {\n  margin: 0;\n  padding-left: 16px;\n}\n\n.parameters .values li,\n.warnings .values li {\n  padding: 1px 0;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC92aXN1YWwtcXVlcnkvcnVuLWV4cGxvcmVyL0M6XFxQcm9qZWN0c1xcZWF2LWl0ZW0tZGlhbG9nLWFuZ3VsYXIvcHJvamVjdHNcXG5nLWRpYWxvZ3NcXHNyY1xcYXBwXFx2aXN1YWwtcXVlcnlcXHJ1bi1leHBsb3JlclxccnVuLWV4cGxvcmVyLmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL3Zpc3VhbC1xdWVyeS9ydW4tZXhwbG9yZXIvcnVuLWV4cGxvcmVyLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UsYUFBQTtBQ0NGOztBREVBO0VBQ0UsYUFBQTtFQUNBLHNCQUFBO0VBQ0Esb0JBQUE7QUNDRjs7QURDRTtFQUNFLGtCQUFBO0FDQ0o7O0FER0E7OztFQUdFLGNBQUE7QUNBRjs7QURFRTs7O0VBQ0UsWUFBQTtFQUNBLGFBQUE7RUFDQSxtQkFBQTtFQUNBLDhCQUFBO0VBQ0EsZUFBQTtBQ0VKOztBREFJOzs7RUFDRSx5QkFBQTtLQUFBLHNCQUFBO01BQUEscUJBQUE7VUFBQSxpQkFBQTtBQ0lOOztBREdFOztFQUNFLFNBQUE7RUFDQSxrQkFBQTtBQ0NKOztBRENJOztFQUNFLGNBQUE7QUNFTiIsImZpbGUiOiJwcm9qZWN0cy9uZy1kaWFsb2dzL3NyYy9hcHAvdmlzdWFsLXF1ZXJ5L3J1bi1leHBsb3Jlci9ydW4tZXhwbG9yZXIuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIud3JhcHBlciB7XHJcbiAgcGFkZGluZzogMjRweDtcclxufVxyXG5cclxuLmFjdGlvbnMge1xyXG4gIGRpc3BsYXk6IGZsZXg7XHJcbiAgZmxleC1kaXJlY3Rpb246IGNvbHVtbjtcclxuICBwYWRkaW5nLWJvdHRvbTogMzJweDtcclxuXHJcbiAgLmFjdGlvbjpub3QoOmxhc3Qtb2YtdHlwZSkge1xyXG4gICAgbWFyZ2luLWJvdHRvbTogOHB4O1xyXG4gIH1cclxufVxyXG5cclxuLnBhcmFtZXRlcnMsXHJcbi53YXJuaW5ncyxcclxuLmRlc2NyaXB0aW9uIHtcclxuICBtYXJnaW46IDE2cHggMDtcclxuXHJcbiAgLnRpdGxlIHtcclxuICAgIGhlaWdodDogNDBweDtcclxuICAgIGRpc3BsYXk6IGZsZXg7XHJcbiAgICBhbGlnbi1pdGVtczogY2VudGVyO1xyXG4gICAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xyXG4gICAgZm9udC1zaXplOiAxNHB4O1xyXG5cclxuICAgIC5tYXQtaWNvbiB7XHJcbiAgICAgIHVzZXItc2VsZWN0OiBub25lO1xyXG4gICAgfVxyXG4gIH1cclxufVxyXG5cclxuLnBhcmFtZXRlcnMsXHJcbi53YXJuaW5ncyB7XHJcbiAgLnZhbHVlcyB7XHJcbiAgICBtYXJnaW46IDA7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDE2cHg7XHJcblxyXG4gICAgbGkge1xyXG4gICAgICBwYWRkaW5nOiAxcHggMDtcclxuICAgIH1cclxuICB9XHJcbn1cclxuIiwiLndyYXBwZXIge1xuICBwYWRkaW5nOiAyNHB4O1xufVxuXG4uYWN0aW9ucyB7XG4gIGRpc3BsYXk6IGZsZXg7XG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XG4gIHBhZGRpbmctYm90dG9tOiAzMnB4O1xufVxuLmFjdGlvbnMgLmFjdGlvbjpub3QoOmxhc3Qtb2YtdHlwZSkge1xuICBtYXJnaW4tYm90dG9tOiA4cHg7XG59XG5cbi5wYXJhbWV0ZXJzLFxuLndhcm5pbmdzLFxuLmRlc2NyaXB0aW9uIHtcbiAgbWFyZ2luOiAxNnB4IDA7XG59XG4ucGFyYW1ldGVycyAudGl0bGUsXG4ud2FybmluZ3MgLnRpdGxlLFxuLmRlc2NyaXB0aW9uIC50aXRsZSB7XG4gIGhlaWdodDogNDBweDtcbiAgZGlzcGxheTogZmxleDtcbiAgYWxpZ24taXRlbXM6IGNlbnRlcjtcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuO1xuICBmb250LXNpemU6IDE0cHg7XG59XG4ucGFyYW1ldGVycyAudGl0bGUgLm1hdC1pY29uLFxuLndhcm5pbmdzIC50aXRsZSAubWF0LWljb24sXG4uZGVzY3JpcHRpb24gLnRpdGxlIC5tYXQtaWNvbiB7XG4gIHVzZXItc2VsZWN0OiBub25lO1xufVxuXG4ucGFyYW1ldGVycyAudmFsdWVzLFxuLndhcm5pbmdzIC52YWx1ZXMge1xuICBtYXJnaW46IDA7XG4gIHBhZGRpbmctbGVmdDogMTZweDtcbn1cbi5wYXJhbWV0ZXJzIC52YWx1ZXMgbGksXG4ud2FybmluZ3MgLnZhbHVlcyBsaSB7XG4gIHBhZGRpbmc6IDFweCAwO1xufSJdfQ== */");

/***/ }),

/***/ "./src/app/visual-query/run-explorer/run-explorer.component.ts":
/*!*********************************************************************!*\
  !*** ./src/app/visual-query/run-explorer/run-explorer.component.ts ***!
  \*********************************************************************/
/*! exports provided: RunExplorerComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "RunExplorerComponent", function() { return RunExplorerComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _run_explorer_helpers__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./run-explorer.helpers */ "./src/app/visual-query/run-explorer/run-explorer.helpers.ts");




var RunExplorerComponent = /** @class */ (function () {
    function RunExplorerComponent(context) {
        this.context = context;
        this.editPipelineEntity = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.saveAndRun = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.repaint = new _angular_core__WEBPACK_IMPORTED_MODULE_1__["EventEmitter"]();
        this.warnings = [];
    }
    RunExplorerComponent.prototype.ngOnInit = function () {
    };
    RunExplorerComponent.prototype.ngOnChanges = function (changes) {
        var _a;
        if ((_a = changes.queryDef) === null || _a === void 0 ? void 0 : _a.currentValue) {
            this.warnings = Object(_run_explorer_helpers__WEBPACK_IMPORTED_MODULE_3__["calculateWarnings"])(this.queryDef.data, this.context);
        }
    };
    RunExplorerComponent.prototype.editPipeline = function () {
        this.editPipelineEntity.emit();
    };
    RunExplorerComponent.prototype.openParamsHelp = function () {
        window.open('https://r.2sxc.org/QueryParams', '_blank');
    };
    RunExplorerComponent.prototype.saveAndRunQuery = function (save, run) {
        this.saveAndRun.emit({ save: save, run: run });
    };
    RunExplorerComponent.prototype.doRepaint = function () {
        this.repaint.emit();
    };
    RunExplorerComponent.ctorParameters = function () { return [
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_2__["Context"] }
    ]; };
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Input"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], RunExplorerComponent.prototype, "queryDef", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], RunExplorerComponent.prototype, "editPipelineEntity", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], RunExplorerComponent.prototype, "saveAndRun", void 0);
    Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Output"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:type", Object)
    ], RunExplorerComponent.prototype, "repaint", void 0);
    RunExplorerComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-run-explorer',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./run-explorer.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/run-explorer/run-explorer.component.html")).default,
            changeDetection: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ChangeDetectionStrategy"].OnPush,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./run-explorer.component.scss */ "./src/app/visual-query/run-explorer/run-explorer.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_shared_services_context__WEBPACK_IMPORTED_MODULE_2__["Context"]])
    ], RunExplorerComponent);
    return RunExplorerComponent;
}());



/***/ }),

/***/ "./src/app/visual-query/run-explorer/run-explorer.helpers.ts":
/*!*******************************************************************!*\
  !*** ./src/app/visual-query/run-explorer/run-explorer.helpers.ts ***!
  \*******************************************************************/
/*! exports provided: calculateWarnings */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "calculateWarnings", function() { return calculateWarnings; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");

/**
 * Check if there are special warnings the developer should know.
 * Typically when the test-module-id is different from the one we're currently
 * working on, or if no test-module-id is provided
 */
function calculateWarnings(queryDefData, context) {
    var warnings = [];
    try { // catch various not-initialized errors
        var regex = /^\[module:moduleid\]=([0-9]*)$/gmi; // capture the mod-id
        var testParams = queryDefData.Pipeline.TestParameters;
        var matches = regex.exec(testParams);
        var testMid = matches[1];
        var urlMid = context.moduleId.toString();
        if (testMid !== urlMid) {
            warnings.push("Your test moduleid (" + testMid + ") is different from the current moduleid (" + urlMid + "). Note that 2sxc 9.33 automatically provides the moduleid - so you usually do not need to set it any more.");
        }
    }
    catch (error) { }
    return warnings;
}


/***/ }),

/***/ "./src/app/visual-query/services/plumb-gui.service.ts":
/*!************************************************************!*\
  !*** ./src/app/visual-query/services/plumb-gui.service.ts ***!
  \************************************************************/
/*! exports provided: dataSrcIdPrefix, PlumbGuiService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "dataSrcIdPrefix", function() { return dataSrcIdPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PlumbGuiService", function() { return PlumbGuiService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");


var linePaintDefault = {
    lineWidth: 4,
    strokeStyle: '#61B7CF',
    joinstyle: 'round',
    outlineColor: 'white',
    outlineWidth: 2
};
var lineCount = 0;
var lineColors = [
    '#009688', '#00bcd4', '#3f51b5', '#9c27b0', '#e91e63',
    '#db4437', '#ff9800', '#60a917', '#60a917', '#008a00',
    '#00aba9', '#1ba1e2', '#0050ef', '#6a00ff', '#aa00ff',
    '#f472d0', '#d80073', '#a20025', '#e51400', '#fa6800',
    '#f0a30a', '#e3c800', '#825a2c', '#6d8764', '#647687',
    '#76608a', '#a0522d'
];
var uuidColorMap = {};
var maxCols = lineColors.length - 1;
function nextLinePaintStyle(uuid) {
    return uuidColorMap[uuid]
        || (uuidColorMap[uuid] = Object.assign({}, linePaintDefault, { strokeStyle: lineColors[lineCount++ % maxCols] }));
}
var instanceTemplate = {
    Connector: ['Bezier', { curviness: 70 }],
    HoverPaintStyle: {
        lineWidth: 4,
        strokeStyle: '#216477',
        outlineWidth: 2,
        outlineColor: 'white'
    },
    PaintStyle: nextLinePaintStyle('dummy'),
    Container: 'pipelineContainer'
};
var dataSrcIdPrefix = 'dataSource_';
var PlumbGuiService = /** @class */ (function () {
    function PlumbGuiService() {
        this.connectionsInitialized = false;
    }
    PlumbGuiService.prototype.buildInstance = function (queryDef) {
        var _this = this;
        var instance = jsPlumb.getInstance(instanceTemplate);
        // If connection on Out-DataSource was removed, remove custom Endpoint
        instance.bind('connectionDetached', function (info) {
            if (info.targetId !== dataSrcIdPrefix + 'Out') {
                return;
            }
            var element = info.target;
            var fixedEndpoints = _this.findDataSourceOfElement(element, queryDef).Definition().In;
            var label = info.targetEndpoint.getOverlay('endpointLabel').label;
            if (!fixedEndpoints.includes(label)) {
                setTimeout(function () { instance.deleteEndpoint(info.targetEndpoint); }, 0);
            }
        });
        instance.bind('connection', function (info) {
            if (!_this.connectionsInitialized) {
                return;
            }
            var endpointLabel = info.targetEndpoint.getOverlay('endpointLabel');
            var labelPrompt = endpointLabel.getLabel();
            var endpoints = instance.getEndpoints(info.target.id);
            var targetEndpointHasSameLabel = endpoints.some(function (endpoint) {
                var label = endpoint.getOverlay('endpointLabel').getLabel();
                return label === labelPrompt &&
                    info.targetEndpoint.id !== endpoint.id &&
                    endpoint.canvas.classList.contains('targetEndpoint');
            });
            if (targetEndpointHasSameLabel) {
                endpointLabel.setLabel("PleaseRename" + Math.floor(Math.random() * 99999));
            }
        });
        return instance;
    };
    // this will retrieve the dataSource info-object for a DOM element
    PlumbGuiService.prototype.findDataSourceOfElement = function (element, queryDef) {
        element = this.fixElementNodeList(element);
        var guid = element.attributes.guid.value;
        var dataSources = queryDef.data.DataSources;
        var found = dataSources.find(function (dataSource) { return dataSource.EntityGuid === guid; });
        return found;
    };
    PlumbGuiService.prototype.initWirings = function (queryDef, instance) {
        var _this = this;
        var _a;
        (_a = queryDef.data.Pipeline.StreamWiring) === null || _a === void 0 ? void 0 : _a.forEach(function (wire) {
            // read connections from Pipeline
            var sourceElementId = dataSrcIdPrefix + wire.From;
            var fromUuid = sourceElementId + '_out_' + wire.Out;
            var targetElementId = dataSrcIdPrefix + wire.To;
            var toUuid = targetElementId + '_in_' + wire.In;
            // Ensure In- and Out-Endpoint exist
            if (!instance.getEndpoint(fromUuid)) {
                _this.addEndpoint(jsPlumb.getSelector('#' + sourceElementId), wire.Out, false, queryDef, instance);
            }
            if (!instance.getEndpoint(toUuid)) {
                _this.addEndpoint(jsPlumb.getSelector('#' + targetElementId), wire.In, true, queryDef, instance);
            }
            try {
                instance.connect({
                    uuids: [fromUuid, toUuid],
                    paintStyle: nextLinePaintStyle(fromUuid)
                });
            }
            catch (e) {
                console.error({ message: 'Connection failed', from: fromUuid, to: toUuid });
            }
        });
    };
    // Add a jsPlumb Endpoint to an Element
    PlumbGuiService.prototype.addEndpoint = function (element, name, isIn, queryDef, instance) {
        element = this.fixElementNodeList(element);
        if (element == null) {
            console.error({ message: 'Element not found', selector: element });
            return;
        }
        var dataSource = this.findDataSourceOfElement(element, queryDef);
        var uuid = element.id + (isIn ? '_in_' : '_out_') + name;
        var params = {
            uuid: uuid,
            enabled: !dataSource.ReadOnly || dataSource.EntityGuid === 'Out' // Endpoints on Out-DataSource must be always enabled
        };
        var endPoint = instance.addEndpoint(element, (isIn ? this.buildTargetEndpoint(queryDef) : this.buildSourceEndpoint(queryDef)), params);
        endPoint.getOverlay('endpointLabel').setLabel(name);
    };
    // the definition of source endpoints (the small blue ones)
    PlumbGuiService.prototype.buildSourceEndpoint = function (queryDef) {
        return {
            paintStyle: { fillStyle: 'transparent', radius: 10, lineWidth: 0 },
            cssClass: 'sourceEndpoint',
            maxConnections: -1,
            isSource: true,
            anchor: ['Continuous', { faces: ['top'] }],
            overlays: this.getEndpointOverlays(true, queryDef.readOnly)
        };
    };
    // the definition of target endpoints (will appear when the user drags a connection)
    PlumbGuiService.prototype.buildTargetEndpoint = function (queryDef) {
        return {
            paintStyle: { fillStyle: 'transparent', radius: 10, lineWidth: 0 },
            cssClass: 'targetEndpoint',
            maxConnections: 1,
            isTarget: true,
            anchor: ['Continuous', { faces: ['bottom'] }],
            overlays: this.getEndpointOverlays(false, queryDef.readOnly),
            dropOptions: { hoverClass: 'hover', activeClass: 'active' }
        };
    };
    // #region jsPlumb Endpoint Definitions
    PlumbGuiService.prototype.getEndpointOverlays = function (isSource, readOnlyMode) {
        return [
            [
                'Label', {
                    id: 'endpointLabel',
                    // location: [0.5, isSource ? -0.5 : 1.5],
                    location: [0.5, isSource ? 0 : 1],
                    label: 'Default',
                    cssClass: 'noselect ' + (isSource ? 'endpointSourceLabel' : 'endpointTargetLabel'),
                    events: {
                        dblclick: function (labelOverlay) {
                            if (readOnlyMode) {
                                return;
                            }
                            var newLabel = prompt('Rename Stream', labelOverlay.label);
                            if (newLabel) {
                                labelOverlay.setLabel(newLabel);
                            }
                        }
                    }
                }
            ]
        ];
    };
    // tslint:disable-next-line:max-line-length
    PlumbGuiService.prototype.makeSource = function (dataSource, element, dragHandler, queryDef, instance) {
        var _this = this;
        // suspend drawing and initialise
        element = this.fixElementNodeList(element);
        instance.batch(function () {
            var _a, _b;
            // make DataSources draggable. Must happen before makeSource()!
            if (!queryDef.readOnly) {
                instance.draggable(element, { grid: [20, 20], drag: dragHandler });
            }
            // Add Out- and In-Endpoints from Definition
            var dataSourceDefinition = dataSource.Definition();
            if (dataSourceDefinition) {
                // Add Out-Endpoints
                (_a = dataSourceDefinition.Out) === null || _a === void 0 ? void 0 : _a.forEach(function (name) {
                    _this.addEndpoint(element, name, false, queryDef, instance);
                });
                // Add In-Endpoints
                (_b = dataSourceDefinition.In) === null || _b === void 0 ? void 0 : _b.forEach(function (name) {
                    _this.addEndpoint(element, name, true, queryDef, instance);
                });
                // make the DataSource a Target for new Endpoints (if .In is an Array)
                if (dataSourceDefinition.In) {
                    var targetEndpointUnlimited = _this.buildTargetEndpoint(queryDef);
                    targetEndpointUnlimited.maxConnections = -1;
                    instance.makeTarget(element, targetEndpointUnlimited);
                }
                if (dataSourceDefinition.DynamicOut) {
                    instance.makeSource(element, _this.buildSourceEndpoint(queryDef), { filter: '.add-endpoint .new-connection' });
                }
            }
        });
    };
    PlumbGuiService.prototype.pushPlumbConfigToQueryDef = function (instance, queryDef) {
        var connectionInfos = instance.getAllConnections().map(function (connection) {
            var wire = {
                From: connection.sourceId.substr(dataSrcIdPrefix.length),
                Out: connection.endpoints[0].getOverlay('endpointLabel').label,
                To: connection.targetId.substr(dataSrcIdPrefix.length),
                In: connection.endpoints[1].getOverlay('endpointLabel').label,
            };
            return wire;
        });
        queryDef.data.Pipeline.StreamWiring = connectionInfos;
        var streamsOut = [];
        instance.selectEndpoints({ target: dataSrcIdPrefix + 'Out' }).each(function (endpoint) {
            streamsOut.push(endpoint.getOverlay('endpointLabel').label);
        });
        queryDef.data.Pipeline.StreamsOut = streamsOut.join(',');
    };
    PlumbGuiService.prototype.putEntityCountOnConnections = function (result, queryDef, instance) {
        var _a;
        (_a = result.Streams) === null || _a === void 0 ? void 0 : _a.forEach(function (stream) {
            var _a;
            // Find jsPlumb Connection for the current Stream
            var sourceElementId = dataSrcIdPrefix + stream.Source;
            var outTargets = ['00000000-0000-0000-0000-000000000000', queryDef.data.Pipeline.EntityGuid];
            var targetElementId = outTargets.includes(stream.Target) ? dataSrcIdPrefix + 'Out' : dataSrcIdPrefix + stream.Target;
            var fromUuid = sourceElementId + '_out_' + stream.SourceOut;
            var toUuid = targetElementId + '_in_' + stream.TargetIn;
            var sEndp = instance.getEndpoint(fromUuid);
            (_a = sEndp === null || sEndp === void 0 ? void 0 : sEndp.connections) === null || _a === void 0 ? void 0 : _a.forEach(function (connection) {
                if (connection.endpoints[1].getUuid() !== toUuid) {
                    return;
                }
                // when connection found, update it's label with the Entities-Count
                connection.setLabel({
                    label: stream.Count.toString(),
                    cssClass: 'streamEntitiesCount'
                });
            });
        });
    };
    /** selectors in jsPlumb return array */
    PlumbGuiService.prototype.fixElementNodeList = function (element) {
        var el = (element instanceof NodeList ? element[0] : element);
        return el;
    };
    PlumbGuiService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [])
    ], PlumbGuiService);
    return PlumbGuiService;
}());



/***/ }),

/***/ "./src/app/visual-query/services/query-definition.service.ts":
/*!*******************************************************************!*\
  !*** ./src/app/visual-query/services/query-definition.service.ts ***!
  \*******************************************************************/
/*! exports provided: QueryDefinitionService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "QueryDefinitionService", function() { return QueryDefinitionService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common/http */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/http.js");
/* harmony import */ var _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @2sic.com/dnn-sxc-angular */ "../../node_modules/@2sic.com/dnn-sxc-angular/__ivy_ngcc__/fesm5/2sic.com-dnn-sxc-angular.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! lodash-es/cloneDeep */ "../../node_modules/lodash-es/cloneDeep.js");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");









var guiTypes = {
    Cache: { name: 'Cache', icon: 'history', notes: 'Caching of data' },
    Filter: { name: 'Filter', icon: 'filter_list', notes: 'Filter data - usually returning less items than came in' },
    Logic: { name: 'Logic', icon: 'share', notes: 'Logic operations - usually choosing between different streams' },
    Lookup: { name: 'Lookup', icon: 'search', notes: 'Lookup operation - usually looking for other data based on a criteria' },
    Modify: { name: 'Modify', icon: 'star_half', notes: 'Modify data - usually changing, adding or removing values' },
    Security: { name: 'Security', icon: 'account_circle', notes: 'Security - usually limit what the user sees based on his identity' },
    Sort: { name: 'Sort', icon: 'sort', notes: 'Sort the items' },
    Source: { name: 'Source', icon: 'cloud_upload', notes: 'Source of new data - usually SQL, CSV or similar' },
    Target: { name: 'Target', icon: 'adjust', notes: 'Target - usually just a destination of data' },
    Unknown: { name: 'Unknown', icon: 'fiber_manual_record', notes: 'Unknown type' },
};
var QueryDefinitionService = /** @class */ (function () {
    function QueryDefinitionService(http, context, dnnContext) {
        this.http = http;
        this.context = context;
        this.dnnContext = dnnContext;
    }
    QueryDefinitionService.prototype.loadQuery = function (pipelineEntityId) {
        var _this = this;
        var pipelineModel$ = this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/PipelineDesigner/GetPipeline'), {
            params: { appId: this.context.appId.toString(), id: pipelineEntityId.toString() }
        });
        var installedDataSources$ = this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/PipelineDesigner/GetInstalledDataSources'));
        return Object(rxjs__WEBPACK_IMPORTED_MODULE_4__["forkJoin"])([pipelineModel$, installedDataSources$]).pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_5__["map"])(function (combined) {
            var pipelineModel = combined[0];
            var installedDataSources = combined[1];
            var queryDefData = {
                DataSources: pipelineModel.DataSources,
                InstalledDataSources: installedDataSources,
                Pipeline: pipelineModel.Pipeline,
            };
            // Init new Pipeline Object
            if (!pipelineEntityId) {
                queryDefData.Pipeline = {
                    AllowEdit: true,
                    Description: undefined,
                    EntityGuid: undefined,
                    EntityId: undefined,
                    Name: undefined,
                    ParametersGroup: undefined,
                    Params: undefined,
                    StreamWiring: undefined,
                    StreamsOut: undefined,
                    TestParameters: undefined,
                };
            }
            var outDs = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_8__["eavConstants"].pipelineDesigner.outDataSource;
            var outDsConst = {
                ContentType: undefined,
                Difficulty: 100,
                DynamicOut: false,
                EnableConfig: undefined,
                HelpLink: undefined,
                Icon: undefined,
                In: outDs.in,
                Name: outDs.name || outDs.className,
                Out: undefined,
                PartAssemblyAndType: outDs.className,
                PrimaryType: 'Target',
                UiHint: undefined,
                allowNew: false,
            };
            installedDataSources.push(outDsConst);
            _this.postProcessDataSources(queryDefData);
            var queryDef = {
                id: pipelineEntityId,
                data: queryDefData,
                readOnly: false,
            };
            // If a new (empty) Pipeline is made, init new Pipeline
            if (!queryDef.id || queryDef.data.DataSources.length === 1) {
                queryDef.readOnly = false;
                _this.loadQueryFromDefaultTemplate(queryDef);
            }
            else {
                // if read only, show message
                queryDef.readOnly = !queryDef.data.Pipeline.AllowEdit;
            }
            return queryDef;
        }));
    };
    // Extend Pipeline-Model retrieved from the Server
    QueryDefinitionService.prototype.postProcessDataSources = function (queryDefData) {
        var e_1, _a;
        var _this = this;
        // stop Post-Process if the model already contains the Out-DataSource
        if (queryDefData.DataSources.find(function (dataSource) { return dataSource.EntityGuid === 'Out'; })) {
            return;
        }
        var outDs = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_8__["eavConstants"].pipelineDesigner.outDataSource;
        var outDsConst = {
            Description: outDs.description,
            EntityGuid: 'Out',
            EntityId: undefined,
            Name: outDs.name,
            PartAssemblyAndType: outDs.className,
            VisualDesignerData: outDs.visualDesignerData,
            ReadOnly: true,
            Difficulty: 100,
        };
        // Append Out-DataSource for the UI
        queryDefData.DataSources.push(outDsConst);
        var _loop_1 = function (dataSource) {
            dataSource.Definition = function () { return _this.getDataSourceDefinitionProperty(queryDefData, dataSource); };
            dataSource.ReadOnly = dataSource.ReadOnly || !queryDefData.Pipeline.AllowEdit;
            // in case server returns null, use a default setting
            dataSource.VisualDesignerData = dataSource.VisualDesignerData || { Top: 50, Left: 50 };
        };
        try {
            // Extend each DataSource with Definition-Property and ReadOnly Status
            for (var _b = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(queryDefData.DataSources), _c = _b.next(); !_c.done; _c = _b.next()) {
                var dataSource = _c.value;
                _loop_1(dataSource);
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (_c && !_c.done && (_a = _b.return)) _a.call(_b);
            }
            finally { if (e_1) throw e_1.error; }
        }
    };
    // Get the Definition of a DataSource
    QueryDefinitionService.prototype.getDataSourceDefinitionProperty = function (queryDefData, dataSource) {
        var definition = queryDefData.InstalledDataSources
            .find(function (installedDataSource) { return installedDataSource.PartAssemblyAndType === installedDataSource.PartAssemblyAndType; });
        if (definition == null) {
            throw new Error("DataSource Definition not found: " + dataSource.PartAssemblyAndType);
        }
        return definition;
    };
    // Init a new Pipeline with DataSources and Wirings from Configuration
    QueryDefinitionService.prototype.loadQueryFromDefaultTemplate = function (queryDef) {
        var e_2, _a;
        var templateForNew = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_8__["eavConstants"].pipelineDesigner.defaultPipeline.dataSources;
        try {
            for (var templateForNew_1 = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__values"])(templateForNew), templateForNew_1_1 = templateForNew_1.next(); !templateForNew_1_1.done; templateForNew_1_1 = templateForNew_1.next()) {
                var dataSource = templateForNew_1_1.value;
                this.addDataSource(queryDef, dataSource.partAssemblyAndType, dataSource.visualDesignerData, dataSource.entityGuid, null);
            }
        }
        catch (e_2_1) { e_2 = { error: e_2_1 }; }
        finally {
            try {
                if (templateForNew_1_1 && !templateForNew_1_1.done && (_a = templateForNew_1.return)) _a.call(templateForNew_1);
            }
            finally { if (e_2) throw e_2.error; }
        }
        // attach template wiring
        queryDef.data.Pipeline.StreamWiring = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_8__["eavConstants"].pipelineDesigner.defaultPipeline.streamWiring;
    };
    QueryDefinitionService.prototype.addDataSource = function (queryDef, partAssemblyAndType, visualDesignerData, entityGuid, name) {
        var _this = this;
        if (visualDesignerData == null) {
            visualDesignerData = { Top: 100, Left: 100 };
        }
        var newDataSource = {
            Description: '',
            EntityGuid: entityGuid || 'unsaved' + (queryDef.dsCount + 1),
            EntityId: undefined,
            Name: name || this.typeNameFilter(partAssemblyAndType, 'className'),
            PartAssemblyAndType: partAssemblyAndType,
            VisualDesignerData: visualDesignerData,
        };
        newDataSource.Definition = function () { return _this.getDataSourceDefinitionProperty(queryDef.data, newDataSource); };
        queryDef.data.DataSources.push(newDataSource);
    };
    QueryDefinitionService.prototype.typeNameFilter = function (input, format) {
        var globalParts = input.match(/[^,\s]+/g);
        switch (format) {
            case 'classFullName':
                if (globalParts) {
                    return globalParts[0];
                }
                break;
            case 'className':
                if (globalParts) {
                    var classFullName = globalParts[0].match(/[^\.]+/g);
                    return classFullName[classFullName.length - 1];
                }
        }
        return input;
    };
    QueryDefinitionService.prototype.dsTypeInfo = function (dataSource, queryDef) {
        // maybe we already retrieved it before...
        var cacheKey = dataSource.EntityGuid;
        if (!queryDef._typeInfos) {
            queryDef._typeInfos = {};
        }
        if (queryDef._typeInfos[cacheKey]) {
            return queryDef._typeInfos[cacheKey];
        }
        var typeInfo;
        // try to find the type on the source
        var def = queryDef.data.InstalledDataSources.find(function (ids) { return ids.PartAssemblyAndType === dataSource.PartAssemblyAndType; });
        if (def) {
            typeInfo = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__assign"])({}, (def.PrimaryType ? guiTypes[def.PrimaryType] : guiTypes.Unknown));
            if (def.Icon) {
                typeInfo.icon = def.Icon;
            }
            if (def.DynamicOut) {
                typeInfo.dynamicOut = true;
            }
            if (def.HelpLink) {
                typeInfo.helpLink = def.HelpLink;
            }
            if (def.EnableConfig) {
                typeInfo.config = def.EnableConfig;
            }
        }
        if (!typeInfo) {
            typeInfo = guiTypes.Unknown;
        }
        queryDef._typeInfos[cacheKey] = typeInfo;
        return typeInfo;
    };
    // save the current query and reload entire definition as returned from server
    QueryDefinitionService.prototype.save = function (queryDef) {
        var pipeline = queryDef.data.Pipeline;
        // Remove some Properties from the DataSource before Saving
        var dataSourcesPrepared = queryDef.data.DataSources.map(function (dataSource) {
            var dataSourceClone = Object(lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_6__["default"])(dataSource);
            delete dataSourceClone.ReadOnly;
            return dataSourceClone;
        });
        return this.http.post(this.dnnContext.$2sxc.http.apiUrl('eav/PipelineDesigner/SavePipeline'), { pipeline: pipeline, dataSources: dataSourcesPrepared }, { params: { appId: this.context.appId.toString(), Id: pipeline.EntityId.toString() } });
    };
    QueryDefinitionService.prototype.queryPipeline = function (id) {
        return this.http.get(this.dnnContext.$2sxc.http.apiUrl('eav/PipelineDesigner/QueryPipeline'), {
            params: { appId: this.context.appId.toString(), id: id.toString() }
        });
    };
    QueryDefinitionService.ctorParameters = function () { return [
        { type: _angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"] },
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_7__["Context"] },
        { type: _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"] }
    ]; };
    QueryDefinitionService = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Injectable"])(),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_2__["HttpClient"], _shared_services_context__WEBPACK_IMPORTED_MODULE_7__["Context"], _2sic_com_dnn_sxc_angular__WEBPACK_IMPORTED_MODULE_3__["Context"]])
    ], QueryDefinitionService);
    return QueryDefinitionService;
}());



/***/ }),

/***/ "./src/app/visual-query/visual-query-routing.module.ts":
/*!*************************************************************!*\
  !*** ./src/app/visual-query/visual-query-routing.module.ts ***!
  \*************************************************************/
/*! exports provided: VisualQueryRoutingModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "VisualQueryRoutingModule", function() { return VisualQueryRoutingModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _visual_query_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./visual-query.component */ "./src/app/visual-query/visual-query.component.ts");
/* harmony import */ var _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../../edit/edit.matcher */ "../edit/edit.matcher.ts");





var routes = [
    {
        path: '', component: _visual_query_component__WEBPACK_IMPORTED_MODULE_3__["VisualQueryComponent"], children: [
            {
                matcher: _edit_edit_matcher__WEBPACK_IMPORTED_MODULE_4__["edit"],
                loadChildren: function () { return Promise.all(/*! import() | edit-edit-module */[__webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~code-edi~f013b1c2"), __webpack_require__.e("default~app-administration-app-administration-module~code-editor-code-editor-module~content-export-c~727f2324"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~2c2e19c5"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~content-~4a56a0b6"), __webpack_require__.e("default~app-administration-app-administration-module~apps-management-apps-management-module~edit-edit-module"), __webpack_require__.e("default~code-editor-code-editor-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~apps-management-apps-management-module~edit-edit-module~visual-query-visual-query-module"), __webpack_require__.e("default~edit-edit-module~manage-content-list-manage-content-list-module"), __webpack_require__.e("default~app-administration-app-administration-module~edit-edit-module"), __webpack_require__.e("common"), __webpack_require__.e("edit-edit-module")]).then(__webpack_require__.bind(null, /*! ../../../../edit/edit.module */ "../edit/edit.module.ts")).then(function (m) { return m.EditModule; }); }
            },
        ]
    },
];
var VisualQueryRoutingModule = /** @class */ (function () {
    function VisualQueryRoutingModule() {
    }
    VisualQueryRoutingModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            imports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"].forChild(routes)],
            exports: [_angular_router__WEBPACK_IMPORTED_MODULE_2__["RouterModule"]]
        })
    ], VisualQueryRoutingModule);
    return VisualQueryRoutingModule;
}());



/***/ }),

/***/ "./src/app/visual-query/visual-query.component.scss":
/*!**********************************************************!*\
  !*** ./src/app/visual-query/visual-query.component.scss ***!
  \**********************************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony default export */ __webpack_exports__["default"] = (".editor-side-toolbar {\n  display: flex;\n  flex-direction: column;\n}\n.editor-side-toolbar .spacer {\n  flex-grow: 1;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC92aXN1YWwtcXVlcnkvQzpcXFByb2plY3RzXFxlYXYtaXRlbS1kaWFsb2ctYW5ndWxhci9wcm9qZWN0c1xcbmctZGlhbG9nc1xcc3JjXFxhcHBcXHZpc3VhbC1xdWVyeVxcdmlzdWFsLXF1ZXJ5LmNvbXBvbmVudC5zY3NzIiwicHJvamVjdHMvbmctZGlhbG9ncy9zcmMvYXBwL3Zpc3VhbC1xdWVyeS92aXN1YWwtcXVlcnkuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDRSxhQUFBO0VBQ0Esc0JBQUE7QUNDRjtBRENFO0VBQ0UsWUFBQTtBQ0NKIiwiZmlsZSI6InByb2plY3RzL25nLWRpYWxvZ3Mvc3JjL2FwcC92aXN1YWwtcXVlcnkvdmlzdWFsLXF1ZXJ5LmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLmVkaXRvci1zaWRlLXRvb2xiYXIge1xyXG4gIGRpc3BsYXk6IGZsZXg7XHJcbiAgZmxleC1kaXJlY3Rpb246IGNvbHVtbjtcclxuXHJcbiAgLnNwYWNlciB7XHJcbiAgICBmbGV4LWdyb3c6IDE7XHJcbiAgfVxyXG59XHJcbiIsIi5lZGl0b3Itc2lkZS10b29sYmFyIHtcbiAgZGlzcGxheTogZmxleDtcbiAgZmxleC1kaXJlY3Rpb246IGNvbHVtbjtcbn1cbi5lZGl0b3Itc2lkZS10b29sYmFyIC5zcGFjZXIge1xuICBmbGV4LWdyb3c6IDE7XG59Il19 */");

/***/ }),

/***/ "./src/app/visual-query/visual-query.component.ts":
/*!********************************************************!*\
  !*** ./src/app/visual-query/visual-query.component.ts ***!
  \********************************************************/
/*! exports provided: VisualQueryComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "VisualQueryComponent", function() { return VisualQueryComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/router */ "../../node_modules/@angular/router/__ivy_ngcc__/fesm5/router.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/platform-browser */ "../../node_modules/@angular/platform-browser/__ivy_ngcc__/fesm5/platform-browser.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var rxjs__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! rxjs */ "../../node_modules/rxjs/_esm5/index.js");
/* harmony import */ var rxjs_operators__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! rxjs/operators */ "../../node_modules/rxjs/_esm5/operators/index.js");
/* harmony import */ var lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! lodash-es/cloneDeep */ "../../node_modules/lodash-es/cloneDeep.js");
/* harmony import */ var script_loader_node_modules_jsplumb_dist_js_jsPlumb_2_1_7_min_js__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! script-loader!node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js */ "../../node_modules/script-loader/index.js!../../node_modules/jsplumb/dist/js/jsPlumb-2.1.7-min.js");
/* harmony import */ var script_loader_node_modules_jsplumb_dist_js_jsPlumb_2_1_7_min_js__WEBPACK_IMPORTED_MODULE_9___default = /*#__PURE__*/__webpack_require__.n(script_loader_node_modules_jsplumb_dist_js_jsPlumb_2_1_7_min_js__WEBPACK_IMPORTED_MODULE_9__);
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _services_query_definition_service__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./services/query-definition.service */ "./src/app/visual-query/services/query-definition.service.ts");
/* harmony import */ var _permissions_services_metadata_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ../permissions/services/metadata.service */ "./src/app/permissions/services/metadata.service.ts");
/* harmony import */ var _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ../app-administration/services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ../shared/constants/eav.constants */ "./src/app/shared/constants/eav.constants.ts");
/* harmony import */ var _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./services/plumb-gui.service */ "./src/app/visual-query/services/plumb-gui.service.ts");
/* harmony import */ var _query_result_query_result_component__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./query-result/query-result.component */ "./src/app/visual-query/query-result/query-result.component.ts");
/* harmony import */ var _shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../shared/helpers/url-prep.helper */ "./src/app/shared/helpers/url-prep.helper.ts");


















var VisualQueryComponent = /** @class */ (function () {
    function VisualQueryComponent(context, router, route, queryDefinitionService, titleService, snackBar, metadataService, contentTypesService, plumbGuiService, zone, dialog, viewContainerRef) {
        this.context = context;
        this.router = router;
        this.route = route;
        this.queryDefinitionService = queryDefinitionService;
        this.titleService = titleService;
        this.snackBar = snackBar;
        this.metadataService = metadataService;
        this.contentTypesService = contentTypesService;
        this.plumbGuiService = plumbGuiService;
        this.zone = zone;
        this.dialog = dialog;
        this.viewContainerRef = viewContainerRef;
        this.explorer = {
            run: 'run',
            add: 'add'
        };
        this.activeExplorer = this.explorer.run;
        this.subscription = new rxjs__WEBPACK_IMPORTED_MODULE_6__["Subscription"]();
        this.context.init(this.route);
        var pipelineId = this.route.snapshot.paramMap.get('pipelineId');
        this.pipelineId = parseInt(pipelineId, 10);
    }
    VisualQueryComponent.prototype.ngOnInit = function () {
        this.loadQuery();
        this.attachKeyboardSave();
        this.refreshOnChildClosed();
    };
    VisualQueryComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    VisualQueryComponent.prototype.toggleExplorer = function (explorer) {
        this.activeExplorer = (this.activeExplorer === explorer) ? null : explorer;
    };
    VisualQueryComponent.prototype.openHelp = function () {
        window.open('http://2sxc.org/help', '_blank');
    };
    VisualQueryComponent.prototype.editPipelineEntity = function () {
        var _this = this;
        // save Pipeline, then open Edit Dialog
        this.savePipeline(function () {
            var form = {
                items: [{ EntityId: _this.queryDef.id }],
            };
            var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_17__["convertFormToUrl"])(form);
            _this.router.navigate(["edit/" + formUrl], { relativeTo: _this.route });
        });
    };
    VisualQueryComponent.prototype.saveAndRun = function (saveAndRun) {
        var _this = this;
        var save = saveAndRun.save;
        var run = saveAndRun.run;
        if (save && run) {
            this.savePipeline(function () { _this.runQuery(); });
        }
        else if (save) {
            this.savePipeline();
        }
        else if (run) {
            this.runQuery();
        }
    };
    VisualQueryComponent.prototype.savePipeline = function (callback) {
        var _this = this;
        this.snackBar.open('Saving...');
        this.queryDef.readOnly = true;
        this.plumbGuiService.pushPlumbConfigToQueryDef(this.instance, this.queryDef);
        this.queryDefinitionService.save(this.queryDef).subscribe({
            next: function (pipelineModel) {
                _this.snackBar.open('Saved', null, { duration: 2000 });
                // Update PipelineData with data retrieved from the Server
                var newQueryDef = Object(lodash_es_cloneDeep__WEBPACK_IMPORTED_MODULE_8__["default"])(_this.queryDef);
                newQueryDef.data.Pipeline = pipelineModel.Pipeline;
                newQueryDef.id = pipelineModel.Pipeline.EntityId;
                _this.router.navigateByUrl(_this.router.url.replace('pipelineId', pipelineModel.Pipeline.EntityId.toString()));
                newQueryDef.readOnly = !pipelineModel.Pipeline.AllowEdit;
                newQueryDef.data.DataSources = pipelineModel.DataSources;
                _this.queryDefinitionService.postProcessDataSources(newQueryDef.data);
                _this.queryDef = newQueryDef;
                if (callback != null) {
                    callback();
                }
            },
            error: function (error) {
                _this.snackBar.open('Save Pipeline failed', null, { duration: 2000 });
                _this.queryDef.readOnly = false;
            }
        });
    };
    VisualQueryComponent.prototype.runQuery = function () {
        var _this = this;
        this.snackBar.open('Running query...');
        this.queryDefinitionService.queryPipeline(this.queryDef.id).subscribe({
            next: function (pipelineResult) {
                _this.snackBar.open('Query worked', null, { duration: 2000 });
                // open modal with the results
                var dialogData = { testParameters: _this.queryDef.data.Pipeline.TestParameters, result: pipelineResult };
                _this.dialog.open(_query_result_query_result_component__WEBPACK_IMPORTED_MODULE_16__["QueryResultComponent"], {
                    data: dialogData,
                    backdropClass: 'dialog-backdrop',
                    panelClass: ['dialog-panel', "dialog-panel-medium", 'no-scrollbar'],
                    viewContainerRef: _this.viewContainerRef,
                    autoFocus: false,
                    closeOnNavigation: false,
                    // spm NOTE: used to force align-items: flex-start; on cdk-global-overlay-wrapper.
                    // Real top margin is overwritten in css e.g. dialog-panel-large
                    position: { top: '0' },
                });
                console.warn(pipelineResult);
                setTimeout(function () { _this.plumbGuiService.putEntityCountOnConnections(pipelineResult, _this.queryDef, _this.instance); }, 0);
            },
            error: function (error) {
                _this.snackBar.open('Query failed', null, { duration: 2000 });
            }
        });
    };
    VisualQueryComponent.prototype.repaint = function () {
        this.instance.repaintEverything();
    };
    VisualQueryComponent.prototype.addSelectedDataSource = function (dataSource) {
        this.queryDefinitionService.addDataSource(this.queryDef, dataSource.PartAssemblyAndType, null, null, dataSource.Name);
        this.savePipeline();
    };
    VisualQueryComponent.prototype.instanceChanged = function (instance) {
        this.instance = instance;
    };
    // Get the URL to configure a DataSource
    VisualQueryComponent.prototype.editDataSourcePart = function (dataSource) {
        var _this = this;
        var sourceDef = this.queryDef.data.InstalledDataSources
            .find(function (installedDataSource) { return installedDataSource.PartAssemblyAndType === dataSource.PartAssemblyAndType; });
        var contentTypeName = (sourceDef === null || sourceDef === void 0 ? void 0 : sourceDef.ContentType)
            ? sourceDef.ContentType
            : '|Config ' + this.queryDefinitionService.typeNameFilter(dataSource.PartAssemblyAndType, 'classFullName');
        var assignmentObjectTypeId = _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_14__["eavConstants"].metadata.entity.type;
        var keyGuid = dataSource.EntityGuid;
        // Query for existing Entity
        this.metadataService
            .getMetadata(assignmentObjectTypeId, _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_14__["eavConstants"].keyTypes.guid, keyGuid, contentTypeName)
            .subscribe(function (metadata) {
            // Edit existing Entity
            if (metadata.length) {
                var form = {
                    items: [{ EntityId: metadata[0].Id }],
                };
                var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_17__["convertFormToUrl"])(form);
                _this.router.navigate(["edit/" + formUrl], { relativeTo: _this.route });
                return;
            }
            // Check if the type exists, and if yes, create new Entity
            _this.contentTypesService.getDetails(contentTypeName, { ignoreErrors: 'true' }).subscribe({
                next: function (contentType) {
                    var form = {
                        items: [{
                                ContentTypeName: contentTypeName,
                                For: {
                                    Target: _shared_constants_eav_constants__WEBPACK_IMPORTED_MODULE_14__["eavConstants"].metadata.entity.target,
                                    Guid: keyGuid,
                                }
                            }],
                    };
                    var formUrl = Object(_shared_helpers_url_prep_helper__WEBPACK_IMPORTED_MODULE_17__["convertFormToUrl"])(form);
                    _this.router.navigate(["edit/" + formUrl], { relativeTo: _this.route });
                },
                error: function (error) {
                    alert('Server reports error - this usually means that this data-source doesn\'t have any configuration');
                }
            });
        });
    };
    VisualQueryComponent.prototype.loadQuery = function (reloadingSnackBar) {
        var _this = this;
        if (reloadingSnackBar === void 0) { reloadingSnackBar = false; }
        if (reloadingSnackBar) {
            this.snackBar.open('Reloading query, please wait...');
        }
        this.queryDefinitionService.loadQuery(this.pipelineId).subscribe(function (queryDef) {
            if (reloadingSnackBar) {
                _this.snackBar.open('Query reloaded', null, { duration: 2000 });
            }
            _this.queryDef = queryDef;
            _this.titleService.setTitle(_this.queryDef.data.Pipeline.Name + " - Visual Query");
        });
    };
    VisualQueryComponent.prototype.attachKeyboardSave = function () {
        var _this = this;
        this.zone.runOutsideAngular(function () {
            _this.subscription.add(Object(rxjs__WEBPACK_IMPORTED_MODULE_6__["fromEvent"])(window, 'keydown').pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["filter"])(function () { return !_this.route.snapshot.firstChild; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["filter"])(function (event) {
                var CTRL_S = (navigator.platform.match('Mac') ? event.metaKey : event.ctrlKey) && event.keyCode === 83;
                return CTRL_S;
            })).subscribe(function (event) {
                event.preventDefault();
                _this.zone.run(function () { _this.savePipeline(); });
            }));
        });
    };
    VisualQueryComponent.prototype.refreshOnChildClosed = function () {
        var _this = this;
        this.subscription.add(this.router.events.pipe(Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["filter"])(function (event) { return event instanceof _angular_router__WEBPACK_IMPORTED_MODULE_2__["NavigationEnd"]; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["startWith"])(!!this.route.snapshot.firstChild), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["map"])(function () { return !!_this.route.snapshot.firstChild; }), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["pairwise"])(), Object(rxjs_operators__WEBPACK_IMPORTED_MODULE_7__["filter"])(function (hadAndHasChild) { return hadAndHasChild[0] && !hadAndHasChild[1]; })).subscribe(function () {
            _this.loadQuery(true);
        }));
    };
    VisualQueryComponent.ctorParameters = function () { return [
        { type: _shared_services_context__WEBPACK_IMPORTED_MODULE_10__["Context"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"] },
        { type: _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"] },
        { type: _services_query_definition_service__WEBPACK_IMPORTED_MODULE_11__["QueryDefinitionService"] },
        { type: _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__["Title"] },
        { type: _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"] },
        { type: _permissions_services_metadata_service__WEBPACK_IMPORTED_MODULE_12__["MetadataService"] },
        { type: _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_13__["ContentTypesService"] },
        { type: _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_15__["PlumbGuiService"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["NgZone"] },
        { type: _angular_material_dialog__WEBPACK_IMPORTED_MODULE_5__["MatDialog"] },
        { type: _angular_core__WEBPACK_IMPORTED_MODULE_1__["ViewContainerRef"] }
    ]; };
    VisualQueryComponent = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-visual-query',
            template: Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! raw-loader!./visual-query.component.html */ "../../node_modules/raw-loader/dist/cjs.js!./src/app/visual-query/visual-query.component.html")).default,
            styles: [Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__importDefault"])(__webpack_require__(/*! ./visual-query.component.scss */ "./src/app/visual-query/visual-query.component.scss")).default]
        }),
        Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"])("design:paramtypes", [_shared_services_context__WEBPACK_IMPORTED_MODULE_10__["Context"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["Router"],
            _angular_router__WEBPACK_IMPORTED_MODULE_2__["ActivatedRoute"],
            _services_query_definition_service__WEBPACK_IMPORTED_MODULE_11__["QueryDefinitionService"],
            _angular_platform_browser__WEBPACK_IMPORTED_MODULE_3__["Title"],
            _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_4__["MatSnackBar"],
            _permissions_services_metadata_service__WEBPACK_IMPORTED_MODULE_12__["MetadataService"],
            _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_13__["ContentTypesService"],
            _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_15__["PlumbGuiService"],
            _angular_core__WEBPACK_IMPORTED_MODULE_1__["NgZone"],
            _angular_material_dialog__WEBPACK_IMPORTED_MODULE_5__["MatDialog"],
            _angular_core__WEBPACK_IMPORTED_MODULE_1__["ViewContainerRef"]])
    ], VisualQueryComponent);
    return VisualQueryComponent;
}());



/***/ }),

/***/ "./src/app/visual-query/visual-query.module.ts":
/*!*****************************************************!*\
  !*** ./src/app/visual-query/visual-query.module.ts ***!
  \*****************************************************/
/*! exports provided: VisualQueryModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "VisualQueryModule", function() { return VisualQueryModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "../../node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "../../node_modules/@angular/core/__ivy_ngcc__/fesm5/core.js");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/common */ "../../node_modules/@angular/common/__ivy_ngcc__/fesm5/common.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/icon */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/icon.js");
/* harmony import */ var _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/tooltip */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/tooltip.js");
/* harmony import */ var _angular_material_button__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/button */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/button.js");
/* harmony import */ var _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/slide-toggle */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/slide-toggle.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material/snack-bar */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/snack-bar.js");
/* harmony import */ var _angular_material_dialog__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/dialog */ "../../node_modules/@angular/material/__ivy_ngcc__/fesm5/dialog.js");
/* harmony import */ var _visual_query_routing_module__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./visual-query-routing.module */ "./src/app/visual-query/visual-query-routing.module.ts");
/* harmony import */ var _visual_query_component__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./visual-query.component */ "./src/app/visual-query/visual-query.component.ts");
/* harmony import */ var _shared_services_context__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ../shared/services/context */ "./src/app/shared/services/context.ts");
/* harmony import */ var _services_query_definition_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./services/query-definition.service */ "./src/app/visual-query/services/query-definition.service.ts");
/* harmony import */ var _run_explorer_run_explorer_component__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./run-explorer/run-explorer.component */ "./src/app/visual-query/run-explorer/run-explorer.component.ts");
/* harmony import */ var _add_explorer_add_explorer_component__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! ./add-explorer/add-explorer.component */ "./src/app/visual-query/add-explorer/add-explorer.component.ts");
/* harmony import */ var _plumb_editor_plumb_editor_component__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! ./plumb-editor/plumb-editor.component */ "./src/app/visual-query/plumb-editor/plumb-editor.component.ts");
/* harmony import */ var _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! ./services/plumb-gui.service */ "./src/app/visual-query/services/plumb-gui.service.ts");
/* harmony import */ var _permissions_services_metadata_service__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ../permissions/services/metadata.service */ "./src/app/permissions/services/metadata.service.ts");
/* harmony import */ var _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ../app-administration/services/content-types.service */ "./src/app/app-administration/services/content-types.service.ts");
/* harmony import */ var _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ../shared/shared-components.module */ "./src/app/shared/shared-components.module.ts");
/* harmony import */ var _query_result_query_result_component__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./query-result/query-result.component */ "./src/app/visual-query/query-result/query-result.component.ts");





















var VisualQueryModule = /** @class */ (function () {
    function VisualQueryModule() {
    }
    VisualQueryModule = Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"])([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["NgModule"])({
            declarations: [
                _visual_query_component__WEBPACK_IMPORTED_MODULE_10__["VisualQueryComponent"],
                _run_explorer_run_explorer_component__WEBPACK_IMPORTED_MODULE_13__["RunExplorerComponent"],
                _add_explorer_add_explorer_component__WEBPACK_IMPORTED_MODULE_14__["AddExplorerComponent"],
                _plumb_editor_plumb_editor_component__WEBPACK_IMPORTED_MODULE_15__["PlumbEditorComponent"],
                _query_result_query_result_component__WEBPACK_IMPORTED_MODULE_20__["QueryResultComponent"],
            ],
            entryComponents: [
                _visual_query_component__WEBPACK_IMPORTED_MODULE_10__["VisualQueryComponent"],
                _run_explorer_run_explorer_component__WEBPACK_IMPORTED_MODULE_13__["RunExplorerComponent"],
                _add_explorer_add_explorer_component__WEBPACK_IMPORTED_MODULE_14__["AddExplorerComponent"],
                _plumb_editor_plumb_editor_component__WEBPACK_IMPORTED_MODULE_15__["PlumbEditorComponent"],
                _query_result_query_result_component__WEBPACK_IMPORTED_MODULE_20__["QueryResultComponent"],
            ],
            imports: [
                _angular_common__WEBPACK_IMPORTED_MODULE_2__["CommonModule"],
                _shared_shared_components_module__WEBPACK_IMPORTED_MODULE_19__["SharedComponentsModule"],
                _visual_query_routing_module__WEBPACK_IMPORTED_MODULE_9__["VisualQueryRoutingModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_3__["MatIconModule"],
                _angular_material_tooltip__WEBPACK_IMPORTED_MODULE_4__["MatTooltipModule"],
                _angular_material_button__WEBPACK_IMPORTED_MODULE_5__["MatButtonModule"],
                _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_6__["MatSlideToggleModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_7__["MatSnackBarModule"],
                _angular_material_dialog__WEBPACK_IMPORTED_MODULE_8__["MatDialogModule"],
            ],
            providers: [
                _shared_services_context__WEBPACK_IMPORTED_MODULE_11__["Context"],
                _services_query_definition_service__WEBPACK_IMPORTED_MODULE_12__["QueryDefinitionService"],
                _services_plumb_gui_service__WEBPACK_IMPORTED_MODULE_16__["PlumbGuiService"],
                _permissions_services_metadata_service__WEBPACK_IMPORTED_MODULE_17__["MetadataService"],
                _app_administration_services_content_types_service__WEBPACK_IMPORTED_MODULE_18__["ContentTypesService"],
            ]
        })
    ], VisualQueryModule);
    return VisualQueryModule;
}());



/***/ })

}]);
//# sourceMappingURL=visual-query-visual-query-module.js.map