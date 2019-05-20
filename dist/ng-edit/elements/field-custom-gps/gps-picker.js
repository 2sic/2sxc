/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 0);
/******/ })
/************************************************************************/
/******/ ({

/***/ "./projects/field-custom-gps/src/main/main.css":
/*!*****************************************************!*\
  !*** ./projects/field-custom-gps/src/main/main.css ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".custom-gps-container {\r\n  display: flex;\r\n  flex-direction: column;\r\n  height: 100%;\r\n}\r\n\r\n.map-info {\r\n  flex: 0 0 32px;\r\n  display: flex;\r\n  align-items: center;\r\n  padding: 4px;\r\n  padding-left: 10px;\r\n  border-bottom: 1px solid #e1e1e1;\r\n  background: white;\r\n}\r\n\r\n.map-info label,\r\n.map-info #icon-search {\r\n  margin-right: 8px;\r\n  display: flex;\r\n  justify-content: center;\r\n  align-items: center;\r\n  font-size: 12px;\r\n  text-transform: uppercase;\r\n  padding: 4px;\r\n}\r\n\r\n.map-info input {\r\n  margin-right: 8px;\r\n  padding: 4px 16px;\r\n  border: none;\r\n  background: transparent;\r\n  outline: none !important;\r\n}\r\n\r\n.map-info__map {\r\n  flex: 1 1 auto;\r\n  width: 100%;\r\n  display: block;\r\n}\r\n\r\n.hidden {\r\n  display: none;\r\n}\r\n\r\n.btn {\r\n  border: 1px solid silver;\r\n  border-radius: 4px;\r\n}\r\n.btn:hover {\r\n  background-color: rgba(69, 79, 99, 0.08);\r\n  cursor: pointer;\r\n}\r\n\r\n.input-component {\r\n  display: flex;\r\n  background-color: rgba(69, 79, 99, 0.08);\r\n  padding: 4px;\r\n  border-radius: 4px 4px 0 0;\r\n  border-bottom: 1px solid silver;\r\n  margin: 8px 0;\r\n}\r\n.input-component:hover {\r\n  border-bottom: 1px solid #0087f4;\r\n}\r\n"

/***/ }),

/***/ "./projects/field-custom-gps/src/main/main.html":
/*!******************************************************!*\
  !*** ./projects/field-custom-gps/src/main/main.html ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"custom-gps-container\">\r\n  <div class=\"map-info\">\r\n    <div class=\"input-component\">\r\n      <label for=\"lat\">Lat:</label>\r\n      <input id=\"lat\" type=\"number\" step=\"0.001\" />\r\n    </div>\r\n    &nbsp;\r\n    <div class=\"input-component\">\r\n      <label for=\"lng\">Lng:</label>\r\n      <input id=\"lng\" type=\"number\" step=\"0.001\" />\r\n    </div>\r\n  </div>\r\n\r\n  <div id=\"address-mask-container\" class=\"map-info hidden\">\r\n    <a id=\"icon-search\" class=\"btn\">\r\n      <span class=\"eav-icon-search\" icon=\"search\"></span>\r\n    </a>\r\n    <span id=\"formatted-address-container\"></span>\r\n  </div>\r\n\r\n  <div id=\"map\" class=\"map-info__map\"></div>\r\n</div>\r\n"

/***/ }),

/***/ "./projects/field-custom-gps/src/main/main.ts":
/*!****************************************************!*\
  !*** ./projects/field-custom-gps/src/main/main.ts ***!
  \****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var models_1 = __webpack_require__(/*! ../shared/models */ "./projects/field-custom-gps/src/shared/models.ts");
var helpers_1 = __webpack_require__(/*! ../shared/helpers */ "./projects/field-custom-gps/src/shared/helpers.ts");
var constants_1 = __webpack_require__(/*! ../shared/constants */ "./projects/field-custom-gps/src/shared/constants.ts");
var template = __webpack_require__(/*! ./main.html */ "./projects/field-custom-gps/src/main/main.html");
var styles = __webpack_require__(/*! ./main.css */ "./projects/field-custom-gps/src/main/main.css");
var field_mask_service_1 = __webpack_require__(/*! ../../../shared/field-mask.service */ "./projects/shared/field-mask.service.ts");
var FieldCustomGps = /** @class */ (function (_super) {
    __extends(FieldCustomGps, _super);
    function FieldCustomGps() {
        var _this = _super.call(this) || this;
        console.log('FieldCustomGps constructor called');
        var mapApiKey = 'AIzaSyDPhnNKpEg8FmY8nooE7Zwnue6SusxEnHE';
        _this.mapApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + mapApiKey;
        _this.fieldInitialized = false;
        _this.eventListeners = [];
        return _this;
    }
    FieldCustomGps.prototype.connectedCallback = function () {
        console.log('FieldCustomGps connectedCallback called');
        // spm prevents connectedCallback from being called more than once. Don't know if it's necessary
        // https://html.spec.whatwg.org/multipage/custom-elements.html#custom-element-conformance
        if (this.fieldInitialized) {
            return;
        }
        this.fieldInitialized = true;
        this.innerHTML = helpers_1.buildTemplate(template, styles);
        this.latInput = this.querySelector('#lat');
        this.lngInput = this.querySelector('#lng');
        var addressMaskContainer = this.querySelector('#address-mask-container');
        this.iconSearch = this.querySelector('#icon-search');
        var formattedAddressContainer = this.querySelector('#formatted-address-container');
        this.mapContainer = this.querySelector('#map');
        var allInputNames = this.experimental.allInputTypeNames.map(function (inputType) { return inputType.name; });
        if (allInputNames.indexOf(this.connector.field.settings.LatField) !== -1) {
            this.latFieldName = this.connector.field.settings.LatField;
        }
        if (allInputNames.indexOf(this.connector.field.settings.LongField) !== -1) {
            this.lngFieldName = this.connector.field.settings.LongField;
        }
        var addressMask = this.connector.field.settings.AddressMask || this.connector.field.settings['Address Mask'];
        this.addressMaskService = new field_mask_service_1.FieldMaskService(addressMask, this.experimental.formGroup.controls, null, null);
        console.log('FieldCustomGps addressMask:', addressMask);
        if (addressMask) {
            addressMaskContainer.classList.remove('hidden');
            formattedAddressContainer.innerText = this.addressMaskService.resolve();
        }
        var mapScriptLoaded = !!window.google;
        if (mapScriptLoaded) {
            this.mapScriptLoaded();
        }
        else {
            var script = document.createElement('script');
            script.src = this.mapApiUrl;
            script.onload = this.mapScriptLoaded.bind(this);
            this.appendChild(script);
        }
    };
    FieldCustomGps.prototype.mapScriptLoaded = function () {
        console.log('FieldCustomGps mapScriptLoaded called');
        this.map = new google.maps.Map(this.mapContainer, { zoom: 15, center: constants_1.defaultCoordinates });
        this.marker = new google.maps.Marker({ position: constants_1.defaultCoordinates, map: this.map, draggable: true });
        this.geocoder = new google.maps.Geocoder();
        // set initial values
        if (!this.connector.data.value) {
            this.updateHtml(constants_1.defaultCoordinates);
        }
        else {
            this.updateHtml(helpers_1.parseLatLng(this.connector.data.value));
        }
        // listen to inputs, iconSearch and marker. Update inputs, map, marker and form
        var onLatLngInputChangeBound = this.onLatLngInputChange.bind(this);
        this.latInput.addEventListener('change', onLatLngInputChangeBound);
        this.lngInput.addEventListener('change', onLatLngInputChangeBound);
        var autoSelectBound = this.autoSelect.bind(this);
        this.iconSearch.addEventListener('click', autoSelectBound);
        this.eventListeners.push({ element: this.latInput, type: 'change', listener: onLatLngInputChangeBound }, { element: this.lngInput, type: 'change', listener: onLatLngInputChangeBound }, { element: this.iconSearch, type: 'click', listener: autoSelectBound });
        this.marker.addListener('dragend', this.onMarkerDragend.bind(this));
    };
    FieldCustomGps.prototype.updateHtml = function (latLng) {
        this.latInput.value = latLng.lat ? latLng.lat.toString() : '';
        this.lngInput.value = latLng.lng ? latLng.lng.toString() : '';
        this.map.setCenter(latLng);
        this.marker.setPosition(latLng);
    };
    FieldCustomGps.prototype.updateForm = function (latLng) {
        this.connector.data.update(helpers_1.stringifyLatLng(latLng));
        if (this.latFieldName) {
            this.experimental.updateField(this.latFieldName, latLng.lat);
        }
        if (this.lngFieldName) {
            this.experimental.updateField(this.lngFieldName, latLng.lng);
        }
    };
    FieldCustomGps.prototype.onLatLngInputChange = function () {
        console.log('FieldCustomGps input changed');
        var latLng = {
            lat: this.latInput.value.length > 0 ? parseFloat(this.latInput.value) : null,
            lng: this.lngInput.value.length > 0 ? parseFloat(this.lngInput.value) : null,
        };
        this.updateHtml(latLng);
        this.updateForm(latLng);
    };
    FieldCustomGps.prototype.autoSelect = function () {
        var _this = this;
        console.log('FieldCustomGps geocoder called');
        var address = this.addressMaskService.resolve();
        this.geocoder.geocode({
            address: address
        }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                var result = results[0].geometry.location;
                var latLng = {
                    lat: result.lat(),
                    lng: result.lng(),
                };
                _this.updateHtml(latLng);
                _this.updateForm(latLng);
            }
            else {
                alert("Could not locate address: " + address);
            }
        });
    };
    FieldCustomGps.prototype.onMarkerDragend = function (event) {
        console.log('FieldCustomGps marker changed');
        var latLng = {
            lat: event.latLng.lat(),
            lng: event.latLng.lng(),
        };
        this.updateHtml(latLng);
        this.updateForm(latLng);
    };
    FieldCustomGps.prototype.disconnectedCallback = function () {
        console.log('FieldCustomGps disconnectedCallback called');
        if (!!window.google) {
            google.maps.event.clearInstanceListeners(this.marker);
            google.maps.event.clearInstanceListeners(this.map);
        }
        this.eventListeners.forEach(function (eventListener) {
            var element = eventListener.element;
            var type = eventListener.type;
            var listener = eventListener.listener;
            element.removeEventListener(type, listener);
        });
    };
    return FieldCustomGps;
}(models_1.EavExperimentalInputField));
customElements.define('field-custom-gps', FieldCustomGps);


/***/ }),

/***/ "./projects/field-custom-gps/src/preview/preview.css":
/*!***********************************************************!*\
  !*** ./projects/field-custom-gps/src/preview/preview.css ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./projects/field-custom-gps/src/preview/preview.html":
/*!************************************************************!*\
  !*** ./projects/field-custom-gps/src/preview/preview.html ***!
  \************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "Lat: <span id=\"lat-container\"></span>, Lng: <span id=\"lng-container\"></span>\r\n"

/***/ }),

/***/ "./projects/field-custom-gps/src/preview/preview.ts":
/*!**********************************************************!*\
  !*** ./projects/field-custom-gps/src/preview/preview.ts ***!
  \**********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var eav_custom_input_field_1 = __webpack_require__(/*! ../../../shared/eav-custom-input-field */ "./projects/shared/eav-custom-input-field.ts");
var helpers_1 = __webpack_require__(/*! ../shared/helpers */ "./projects/field-custom-gps/src/shared/helpers.ts");
var constants_1 = __webpack_require__(/*! ../shared/constants */ "./projects/field-custom-gps/src/shared/constants.ts");
var template = __webpack_require__(/*! ./preview.html */ "./projects/field-custom-gps/src/preview/preview.html");
var styles = __webpack_require__(/*! ./preview.css */ "./projects/field-custom-gps/src/preview/preview.css");
var FieldCustomGpsPreview = /** @class */ (function (_super) {
    __extends(FieldCustomGpsPreview, _super);
    function FieldCustomGpsPreview() {
        var _this = _super.call(this) || this;
        console.log('FieldCustomGpsPreview constructor called');
        return _this;
    }
    FieldCustomGpsPreview.prototype.connectedCallback = function () {
        var _this = this;
        console.log('FieldCustomGpsPreview connectedCallback called');
        this.innerHTML = helpers_1.buildTemplate(template, styles);
        this.latContainer = this.querySelector('#lat-container');
        this.lngContainer = this.querySelector('#lng-container');
        // set initial value
        if (!this.connector.data.value) {
            this.updateHtml(constants_1.defaultCoordinates);
        }
        else {
            this.updateHtml(helpers_1.parseLatLng(this.connector.data.value));
        }
        // update on value change
        this.connector.data.onValueChange(function (value) {
            if (!value) {
                _this.updateHtml(constants_1.defaultCoordinates);
            }
            else {
                var latLng = helpers_1.parseLatLng(value);
                _this.updateHtml(latLng);
            }
        });
    };
    FieldCustomGpsPreview.prototype.updateHtml = function (latLng) {
        this.latContainer.innerText = latLng.lat ? latLng.lat.toString() : '';
        this.lngContainer.innerText = latLng.lng ? latLng.lng.toString() : '';
    };
    FieldCustomGpsPreview.prototype.disconnectedCallback = function () {
        console.log('FieldCustomGpsPreview disconnectedCallback called');
    };
    return FieldCustomGpsPreview;
}(eav_custom_input_field_1.EavCustomInputField));
customElements.define('field-custom-gps-preview', FieldCustomGpsPreview);


/***/ }),

/***/ "./projects/field-custom-gps/src/shared/constants.ts":
/*!***********************************************************!*\
  !*** ./projects/field-custom-gps/src/shared/constants.ts ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.defaultCoordinates = {
    lat: 47.17465989999999,
    lng: 9.469142499999975,
};


/***/ }),

/***/ "./projects/field-custom-gps/src/shared/helpers.ts":
/*!*********************************************************!*\
  !*** ./projects/field-custom-gps/src/shared/helpers.ts ***!
  \*********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function buildTemplate(template, styles) {
    return template + "<style>\n" + styles + "\n</style>";
}
exports.buildTemplate = buildTemplate;
function parseLatLng(value) {
    var latLng = JSON.parse(value.replace('latitude', 'lat').replace('longitude', 'lng'));
    return latLng;
}
exports.parseLatLng = parseLatLng;
function stringifyLatLng(latLng) {
    var value = JSON.stringify(latLng).replace('lat', 'latitude').replace('lng', 'longitude');
    return value;
}
exports.stringifyLatLng = stringifyLatLng;


/***/ }),

/***/ "./projects/field-custom-gps/src/shared/models.ts":
/*!********************************************************!*\
  !*** ./projects/field-custom-gps/src/shared/models.ts ***!
  \********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var eav_custom_input_field_1 = __webpack_require__(/*! ../../../shared/eav-custom-input-field */ "./projects/shared/eav-custom-input-field.ts");
var EavExperimentalInputField = /** @class */ (function (_super) {
    __extends(EavExperimentalInputField, _super);
    function EavExperimentalInputField() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return EavExperimentalInputField;
}(eav_custom_input_field_1.EavCustomInputField));
exports.EavExperimentalInputField = EavExperimentalInputField;
var ElementEventListener = /** @class */ (function () {
    function ElementEventListener() {
    }
    return ElementEventListener;
}());
exports.ElementEventListener = ElementEventListener;


/***/ }),

/***/ "./projects/shared/eav-custom-input-field.ts":
/*!***************************************************!*\
  !*** ./projects/shared/eav-custom-input-field.ts ***!
  \***************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var EavCustomInputField = /** @class */ (function (_super) {
    __extends(EavCustomInputField, _super);
    function EavCustomInputField() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return EavCustomInputField;
}(HTMLElement));
exports.EavCustomInputField = EavCustomInputField;
var EavCustomInputFieldObservable = /** @class */ (function (_super) {
    __extends(EavCustomInputFieldObservable, _super);
    function EavCustomInputFieldObservable() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return EavCustomInputFieldObservable;
}(EavCustomInputField));
exports.EavCustomInputFieldObservable = EavCustomInputFieldObservable;


/***/ }),

/***/ "./projects/shared/field-mask.service.ts":
/*!***********************************************!*\
  !*** ./projects/shared/field-mask.service.ts ***!
  \***********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * Create a new FieldMaskService instance and access result with resolve
 * @example
 * const mask = new FieldMaskService("[FirstName]", formGroup.controls);
 * const maskValue = mask.resolve();
 *
 * @param mask a string like "[FirstName] [LastName]"
 * @param model usually FormGroup controls, passed into here
 * @param overloadPreCleanValues a function which will "scrub" the found field-values
 */
var FieldMaskService = /** @class */ (function () {
    function FieldMaskService(mask, model, changeEvent, overloadPreCleanValues) {
        this.changeEvent = changeEvent;
        this.fields = [];
        this.findFields = /\[.*?\]/ig;
        this.unwrapField = /[\[\]]/ig;
        this.subscriptions = [];
        this.mask = mask;
        this.model = model;
        this.fields = this.fieldList();
        if (overloadPreCleanValues) {
            this.preClean = overloadPreCleanValues;
        }
        // bind auto-watch only if needed...
        if (model && changeEvent) {
            this.watchAllFields();
        }
    }
    /** Resolves a mask to the final value */
    FieldMaskService.prototype.resolve = function () {
        var _this = this;
        var value = this.mask;
        this.fields.forEach(function (e, i) {
            var replaceValue = _this.model.hasOwnProperty(e) && _this.model[e] && _this.model[e].value ? _this.model[e].value : '';
            var cleaned = _this.preClean(e, replaceValue);
            value = value.replace('[' + e + ']', cleaned);
        });
        return value;
    };
    /** Retrieves a list of all fields used in the mask */
    FieldMaskService.prototype.fieldList = function () {
        var _this = this;
        var result = [];
        if (!this.mask) {
            return result;
        }
        var matches = this.mask.match(this.findFields);
        if (matches) {
            matches.forEach(function (e, i) {
                var staticName = e.replace(_this.unwrapField, '');
                result.push(staticName);
            });
        }
        else { // TODO: ask is this good
            result.push(this.mask);
        }
        return result;
    };
    /** Default preClean function */
    FieldMaskService.prototype.preClean = function (key, value) {
        return value;
    };
    /** Change-event - will only fire if it really changes */
    FieldMaskService.prototype.onChange = function () {
        console.log('StringTemplatePickerComponent onChange called');
        var maybeNew = this.resolve();
        if (this.value !== maybeNew) {
            this.changeEvent(maybeNew);
        }
        this.value = maybeNew;
    };
    /** Add watcher and execute onChange */
    FieldMaskService.prototype.watchAllFields = function () {
        var _this = this;
        console.log('StringTemplatePickerComponent watchAllFields called');
        // add a watch for each field in the field-mask
        this.fields.forEach(function (field) {
            var valSub = _this.model[field].valueChanges.subscribe(function (value) { return _this.onChange(); });
            _this.subscriptions.push(valSub);
        });
    };
    FieldMaskService.prototype.destroy = function () {
        this.subscriptions.forEach(function (sub) { return sub.unsubscribe(); });
    };
    return FieldMaskService;
}());
exports.FieldMaskService = FieldMaskService;


/***/ }),

/***/ 0:
/*!*************************************************************************************************************!*\
  !*** multi ./projects/field-custom-gps/src/main/main.ts ./projects/field-custom-gps/src/preview/preview.ts ***!
  \*************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

__webpack_require__(/*! ./projects/field-custom-gps/src/main/main.ts */"./projects/field-custom-gps/src/main/main.ts");
module.exports = __webpack_require__(/*! ./projects/field-custom-gps/src/preview/preview.ts */"./projects/field-custom-gps/src/preview/preview.ts");


/***/ })

/******/ });
//# sourceMappingURL=gps-picker.js.map