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
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
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
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 106);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var expand_button_config_1 = __webpack_require__(14);
var command_definition_1 = __webpack_require__(103);
var commands_1 = __webpack_require__(10);
var CommandBase = /** @class */ (function () {
    function CommandBase() {
        this.commandDefinition = new command_definition_1.CommandDefinition();
    }
    // quick helper so we can better debug the creation of definitions
    CommandBase.prototype.makeDef = function (name, translateKey, icon, uiOnly, partOfPage, more) {
        if (typeof (partOfPage) !== 'boolean') {
            throw 'partOfPage in commands not provided, order will be wrong!';
        }
        // Toolbar API v2
        this.commandDefinition.name = name;
        this.commandDefinition.buttonConfig = expand_button_config_1.getButtonConfigDefaultsV1(name, icon, translateKey, uiOnly, partOfPage, more);
        this.registerInCatalog();
    };
    /** register new CommandDefinition with in Commands */
    CommandBase.prototype.registerInCatalog = function () {
        commands_1.Commands.getInstance().addDef(this.commandDefinition);
    };
    return CommandBase;
}());
exports.CommandBase = CommandBase;


/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
// ReSharper restore InconsistentNaming
exports.windowInPage = window;


/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var positioning_1 = __webpack_require__(38);
/**
 * the quick-edit object
 * the quick-insert object
 */
var QuickE = /** @class */ (function () {
    function QuickE() {
        var _this = this;
        this.body = $('body');
        this.win = $(window);
        this.main = $("<div class='sc-content-block-menu sc-content-block-quick-insert sc-i18n'></div>");
        this.template = "<a class='sc-content-block-menu-addcontent sc-invisible' data-type='Default' data-i18n='[titleTemplate]QuickInsertMenu.AddBlockContent'>x</a><a class='sc-content-block-menu-addapp sc-invisible' data-type='' data-i18n='[titleTemplate]QuickInsertMenu.AddBlockApp'>x</a>" + btn('select', 'ok', 'Select', true) + btn('paste', 'paste', 'Paste', true, true);
        this.selected = $("<div class='sc-content-block-menu sc-content-block-selected-menu sc-i18n'></div>")
            .append(btn('delete', 'trash-empty', 'Delete'), btn('sendToPane', 'export', 'Move', null, null, 'sc-cb-mod-only'), "<div id='paneList'></div>");
        // will be populated later in the module section
        this.contentBlocks = null;
        this.cachedPanes = null;
        this.modules = null;
        this.nearestCb = null;
        this.nearestMod = null;
        this.modManage = null;
        // add stuff which depends on other values to create
        this.cbActions = $(this.template);
        this.modActions = $(this.template.replace(/QuickInsertMenu.AddBlock/g, 'QuickInsertMenu.AddModule'))
            .attr('data-context', 'module')
            .addClass('sc-content-block-menu-module');
        this.selected.toggle = function (target) {
            if (!target || target.length === 0) {
                _this.selected.hide();
            }
            else {
                var coords = positioning_1.getCoordinates(target);
                coords.yh = coords.y + 20;
                positioning_1.positionAndAlign(_this.selected, coords);
                _this.selected.target = target;
            }
        };
    }
    return QuickE;
}());
exports.$quickE = new QuickE();
function btn(action, icon, i18N, invisible, unavailable, classes) {
    return "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-" + icon + " " + (invisible ? ' sc-invisible ' : '') + (unavailable ? ' sc-unavailable ' : '') + classes + "' data-action='" + action + "' data-i18n='[title]QuickInsertMenu." + i18N + "'></a>";
}
/**
 * build the toolbar (hidden, but ready to show)
 */
function prepareToolbarInDom() {
    exports.$quickE.body.append(exports.$quickE.main)
        .append(exports.$quickE.selected);
    exports.$quickE.main.append(exports.$quickE.cbActions)
        .append(exports.$quickE.modActions);
}
exports.prepareToolbarInDom = prepareToolbarInDom;


/***/ }),
/* 3 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var window_in_page_1 = __webpack_require__(1);
// ReSharper restore InconsistentNaming
exports.$2sxcInPage = window_in_page_1.windowInPage.$2sxc;


/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * get edit-context info of html element or sxc-object
 * @param {SxcInstanceWithInternals} sxc
 * @param {HTMLElement} htmlElement
 * @return {DataEditContext} edit context info
 */
function getEditContext(sxc, htmlElement) {
    var editContextTag;
    if (htmlElement) {
        editContextTag = getContainerTag(htmlElement);
    }
    else {
        editContextTag = getTag(sxc);
    }
    return getEditContextOfTag(editContextTag);
}
exports.getEditContext = getEditContext;
/**
 * get nearest html tag of the sxc instance with data-edit-context
 * @param htmlTag
 */
function getContainerTag(htmlTag) {
    return $(htmlTag).closest('div[data-edit-context]')[0];
}
exports.getContainerTag = getContainerTag;
/**
 * get a html tag of the sxc instance
 * @param {SxcInstanceWithInternals} sxci
 * @return {jquery} - resulting html
 */
function getTag(sxci) {
    return $("div[data-cb-id='" + sxci.cbid + "']")[0];
}
exports.getTag = getTag;
/**
 * get the edit-context object (a json object) of the current tag/sxc-instance
 * @param {any} htmlTag
 * @return {DataEditContext} edit-context object
 */
function getEditContextOfTag(htmlTag) {
    var attr = htmlTag.getAttribute('data-edit-context');
    return JSON.parse(attr || '{ }');
}
exports.getEditContextOfTag = getEditContextOfTag;


/***/ }),
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var sxc_controller_in_page_1 = __webpack_require__(3);
var api_1 = __webpack_require__(4);
var sxc_1 = __webpack_require__(6);
var system_context_1 = __webpack_require__(53);
var tenant_context_1 = __webpack_require__(54);
var user_context_1 = __webpack_require__(55);
var content_block_context_1 = __webpack_require__(56);
var context_of_button_1 = __webpack_require__(57);
var app_context_1 = __webpack_require__(64);
var instance_context_1 = __webpack_require__(65);
var item_context_1 = __webpack_require__(66);
var page_context_1 = __webpack_require__(67);
var is_1 = __webpack_require__(68);
var ui_context_1 = __webpack_require__(69);
/**
 * Primary API to get the context (context is cached)
 * @param htmlElement or Id (moduleId)
 * @param cbid
 */
function context(tagOrSxc, cbid) {
    var sxc;
    var containerTag = null;
    if (is_1.isSxcInstance(tagOrSxc)) { // it is SxcInstance
        sxc = tagOrSxc;
    }
    else if (typeof tagOrSxc === 'number') { // it is number
        sxc = sxc_1.getSxcInstance(tagOrSxc, cbid);
    }
    else { // it is HTMLElement
        sxc = sxc_1.getSxcInstance(tagOrSxc);
        containerTag = api_1.getContainerTag(tagOrSxc);
    }
    ;
    var contextOfButton = getContextInstance(sxc, containerTag);
    contextOfButton.sxc = sxc;
    return contextOfButton;
}
exports.context = context;
/**
 * Create copy of context, so it can be modified before use
 * @param htmlElement or Id (moduleId)
 * @param cbid
 */
function contextCopy(htmlElementOrId, cbid) {
    var contextOfButton = context(htmlElementOrId, cbid);
    // set sxc to null because of cyclic reference, so we can serialize it
    contextOfButton.sxc = null;
    // make a copy
    var copyOfContext = JSON.parse(JSON.stringify(contextOfButton));
    // bring sxc back to context
    contextOfButton.sxc = sxc_1.getSxcInstance(htmlElementOrId);
    return copyOfContext;
}
exports.contextCopy = contextCopy;
/**
 * Create new context
 * @param sxc
 * @param htmlElement
 */
function getContextInstance(sxc, htmlElement) {
    var editContext = api_1.getEditContext(sxc, htmlElement);
    return createContextFromEditContext(editContext);
}
exports.getContextInstance = getContextInstance;
/**
 * create part of context object (it is not cached)
 * @param editCtx
 */
function createContextFromEditContext(editCtx) {
    var btnCtx = new context_of_button_1.ContextOfButton();
    // *** ContextOf ***
    // this will be everything about the current system, like system / api -paths etc.
    btnCtx.system = new system_context_1.SystemContext();
    if (editCtx.error) {
        btnCtx.system.error = editCtx.error.type;
    }
    // empty
    // this will be something about the current tenant(the dnn portal)
    btnCtx.tenant = new tenant_context_1.TenantContext();
    if (editCtx.Environment) {
        btnCtx.tenant.id = editCtx.Environment.WebsiteId; // InstanceConfig.portalId
        btnCtx.tenant.url = editCtx.Environment.WebsiteUrl; // NgDialogParams.portalroot
    }
    // things about the user
    btnCtx.user = new user_context_1.UserContext();
    if (editCtx.User) {
        btnCtx.user.canDesign = editCtx.User.CanDesign;
        btnCtx.user.canDevelop = editCtx.User.CanDevelop;
    }
    // *** ContextOfPage ***
    // this will be information related to the current page
    btnCtx.page = new page_context_1.PageContext();
    if (editCtx.Environment) {
        btnCtx.page.id = editCtx.Environment.PageId; // InstanceConfig.tabId, NgDialogParams.tid
        btnCtx.page.url = editCtx.Environment.PageUrl;
    }
    // *** ContextOfInstance ***
    // information related to the current DNN module, incl.instanceId, etc.
    btnCtx.instance = new instance_context_1.InstanceContext();
    if (editCtx.Environment) {
        btnCtx.instance.id = editCtx.Environment.InstanceId; // InstanceConfig.moduleId, NgDialogParams.mid
        btnCtx.instance.isEditable = editCtx.Environment.IsEditable;
        // sxc
        btnCtx.instance.sxcVersion = editCtx.Environment.SxcVersion;
        btnCtx.instance.parameters = editCtx.Environment.parameters;
        btnCtx.instance.sxcRootUrl = editCtx.Environment.SxcRootUrl; // NgDialogParams.websiteroot
    }
    if (editCtx.ContentBlock) {
        btnCtx.instance.allowPublish = editCtx.ContentBlock.VersioningRequirements === sxc_controller_in_page_1.$2sxcInPage.c.publishAllowed; // NgDialogParams.publishing
    }
    // this will be about the current app, settings of the app, app - paths, etc.
    btnCtx.app = new app_context_1.AppContext();
    if (editCtx.ContentGroup) {
        btnCtx.app.id = editCtx.ContentGroup.AppId; // or NgDialogParams.appId
        btnCtx.app.isContent = editCtx.ContentGroup.IsContent;
        btnCtx.app.resourcesId = editCtx.ContentGroup.AppResourcesId;
        btnCtx.app.settingsId = editCtx.ContentGroup.AppSettingsId;
        btnCtx.app.appPath = editCtx.ContentGroup.AppUrl; // InstanceConfig.appPath, NgDialogParams.approot, this is the only value which doesn't have a slash by default. note that the app-root doesn't exist when opening "manage-app"
        btnCtx.app.hasContent = editCtx.ContentGroup.HasContent;
        btnCtx.app.supportsAjax = editCtx.ContentGroup.SupportsAjax;
        btnCtx.app.zoneId = editCtx.ContentGroup.ZoneId; // or NgDialogParams.zoneId
    }
    if (editCtx.Language) {
        // languages
        btnCtx.app.currentLanguage = editCtx.Language.Current; // NgDialogParams.lang
        btnCtx.app.primaryLanguage = editCtx.Language.Primary; // NgDialogParams.langpri
        btnCtx.app.allLanguages = editCtx.Language.All; // or NgDialogParams.langs
    }
    // ensure that the UI will load the correct assets to enable editing
    btnCtx.ui = new ui_context_1.UiContext();
    if (editCtx.Ui) {
        btnCtx.ui.autoToolbar = editCtx.Ui.AutoToolbar; // toolbar auto-show
        if (editCtx.Ui.Form)
            btnCtx.ui.form = editCtx.Ui.Form; // decide which dialog opens, eg ng8
    }
    // *** ContextOfContentBlock ***
    // information related to the current contentBlock
    btnCtx.contentBlock = new content_block_context_1.ContentBlockContext();
    if (editCtx.ContentBlock) {
        btnCtx.contentBlock.id = editCtx.ContentBlock.Id; // or sxc.cbid or InstanceConfig.cbid
        btnCtx.contentBlock.isEntity = editCtx.ContentBlock.IsEntity; // ex: InstanceConfig.cbIsEntity
        btnCtx.contentBlock.showTemplatePicker = editCtx.ContentBlock.ShowTemplatePicker;
        btnCtx.contentBlock.versioningRequirements = editCtx.ContentBlock.VersioningRequirements;
        btnCtx.contentBlock.parentFieldName = editCtx.ContentBlock.ParentFieldName;
        btnCtx.contentBlock.parentFieldSortOrder = editCtx.ContentBlock.ParentFieldSortOrder;
        btnCtx.contentBlock.partOfPage = editCtx.ContentBlock.PartOfPage; // NgDialogParams.partOfPage
    }
    if (editCtx.ContentGroup) {
        btnCtx.contentBlock.isCreated = editCtx.ContentGroup.IsCreated;
        btnCtx.contentBlock.isList = editCtx.ContentGroup.IsList; // ex: InstanceConfig.isList
        btnCtx.contentBlock.queryId = editCtx.ContentGroup.QueryId;
        btnCtx.contentBlock.templateId = editCtx.ContentGroup.TemplateId;
        btnCtx.contentBlock.contentTypeId = editCtx.ContentGroup.ContentTypeName;
        btnCtx.contentBlock.contentGroupId = editCtx.ContentGroup.Guid; // ex: InstanceConfig.contentGroupId
    }
    // *** ContextOfItem ***
    // information about the current item
    btnCtx.item = new item_context_1.ItemContext();
    // empty
    // *** ContextOfToolbar ***
    // fill externally
    // *** ContextOfButton ***
    // fill externally
    return btnCtx;
}
exports.createContextFromEditContext = createContextFromEditContext;


/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var sxc_controller_in_page_1 = __webpack_require__(3);
function getSxcInstance(module, cbid) {
    var sxc = sxc_controller_in_page_1.$2sxcInPage(module, cbid);
    return sxc;
}
exports.getSxcInstance = getSxcInstance;


/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var entry_1 = __webpack_require__(72);
var maxScopeLen = 3;
var maxNameLen = 6;
var liveDump = false;
var Log = /** @class */ (function () {
    /**
     * Create a logger and optionally attach it to a parent logger
     * @param string name this logger should use
     * @param Log optional parrent logger to attach to
     * @param string optional initial message to log
     */
    function Log(name, parent, initialMessage) {
        var _this = this;
        /**
         * all log-entries on this logger
         */
        this.entries = new Array();
        /**
         * Full identifier of this log-object, with full hierarchy
         */
        this.fullIdentifier = function () {
            return "" + (_this.parent ? _this.parent.fullIdentifier() : '') + _this.identifier();
        };
        /**
         * link this log to a parent
         * usually happens in constructor, but in rare cases
         * this must be called manually
         */
        this.linkLog = function (parent) {
            _this.parent = parent || _this.parent; // if new parent isn't defined, don't replace
        };
        /**
         * scope of this logger - to easily see which ones
         * are about the same topic
         */
        this.scope = 'tdo';
        /**
         * name of this logger
         */
        this.name = 'unknwn';
        /**
         * Unique 2-character ID of this specific log object
         */
        this.id = function () { return _this.idCache || (_this.idCache = _this.randomString(2)); };
        /**
         * Unique identifier of this log object, with name and ID
         */
        this.identifier = function () { return "" + _this.scope + _this.name + "(" + _this.id() + ")"; };
        this.rename(name);
        this.linkLog(parent);
        if (initialMessage != null)
            this.add(initialMessage);
    }
    /**
     * give this logger a new name
     * usually happens in constructor, but in rare cases
     * it's called manually
     * @param name
     */
    Log.prototype.rename = function (name) {
        try {
            var dot = name.indexOf('.');
            this.scope = dot > 0 ? name.substr(0, Math.min(dot, maxScopeLen)) + '.' : '';
            var rest = dot > 0 ? name.substr(dot + 1) : name;
            this.name = rest.substr(0, Math.min(rest.length, maxNameLen));
            this.name = this.name.substr(0, Math.min(this.name.length, maxNameLen));
        }
        catch (e) {
            /* ignore */
        }
    };
    /**
     * add a message to the log-list
     * @param message
     *
     * preferred usage is with string parameter:
     * log.add(`description ${ parameter }`);
     *
     * in case that we experience error with normal string parameter, we can use arrow function to enclose parameter like this () => parameter
     * but use it very rarely, because there is certainly a performance implication!
     * log.add(`description ${() => parameter}`);
     */
    Log.prototype.add = function (message) {
        var messageText;
        if (message instanceof Function) {
            try {
                messageText = (message()).toString();
                message = null; // maybe it is unnecessary, but added to be safe as possible that arrow function parameter will be garbage collected
            }
            catch (e) {
                messageText = 'undefined';
            }
        }
        else {
            messageText = message.toString();
        }
        var entry = new entry_1.Entry(this, messageText);
        this.addEntry(entry);
        if (liveDump)
            console.log(this.dump(undefined, undefined, undefined, entry));
        return messageText;
    };
    /**
     * helper to create a text-output of the log info
     * @param separator
     * @param start
     * @param end
     */
    Log.prototype.dump = function (separator, start, end, one) {
        if (separator === void 0) { separator = ' - '; }
        if (start === void 0) { start = ''; }
        if (end === void 0) { end = ''; }
        if (one === void 0) { one = null; }
        var lg = start;
        var dumpOne = function (e) { return lg += e.source() + separator + e.message + '\n'; };
        if (one)
            dumpOne(one);
        else
            this.entries.forEach(dumpOne);
        lg += end;
        return lg;
    };
    /**
     * add an entry-object to this logger
     * this is often called by sub-loggers to add to parent
     * @param entry
     */
    Log.prototype.addEntry = function (entry) {
        this.entries.push(entry);
        if (this.parent)
            this.parent.addEntry(entry);
    };
    /**
     * helper to generate a random 2-char ID
     * @param stringLength
     */
    Log.prototype.randomString = function (stringLength) {
        var chars = '0123456789abcdefghiklmnopqrstuvwxyz';
        var randomstring = '';
        for (var i = 0; i < stringLength; i++) {
            var rnum = Math.floor(Math.random() * chars.length);
            randomstring += chars.substring(rnum, rnum + 1);
        }
        return randomstring;
    };
    return Log;
}());
exports.Log = Log;


/***/ }),
/* 8 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * selectors used all over the in-page-editing, centralized to ensure consistency
 */
exports.selectors = {
    cb: {
        id: 'cb',
        class: 'sc-content-block',
        selector: '.sc-content-block',
        listSelector: '.sc-content-block-list',
        context: 'data-list-context',
        singleItem: 'single-item',
    },
    mod: {
        id: 'mod',
        class: 'DnnModule',
        selector: '.DnnModule',
        listSelector: '.DNNEmptyPane, .dnnDropEmptyPanes, :has(>.DnnModule)',
        context: null,
    },
    eitherCbOrMod: '.DnnModule, .sc-content-block',
    selected: 'sc-cb-is-selected',
};


/***/ }),
/* 9 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * provide an official translate API for 2sxc - currently internally using a jQuery library, but this may change
 * @param key
 */
function translate(key) {
    // return key;
    return ($.t && $.t(key)) || key;
}
exports.translate = translate;


/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Commands = /** @class */ (function () {
    function Commands() {
        var _this = this;
        this.commandList = [];
        this.list = {}; // hash - table of action definitions, to be used a list()["action - name"]
        this.get = function (name) { return _this.list[name]; }; // a specific action definition
        this.addDef = function (def) {
            if (!_this.list[def.name]) {
                // add
                _this.commandList.push(def);
                _this.list[def.name] = def;
            }
            else if (_this.list[def.name] !== def) {
                // update
                _this.list[def.name] = def;
            }
        };
    }
    Commands.getInstance = function () {
        if (!Commands.instance) {
            Commands.instance = new Commands();
        }
        return Commands.instance;
    };
    return Commands;
}());
exports.Commands = Commands;


/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var render_1 = __webpack_require__(16);
/*
 * this is a content block in the browser
 *
 * A Content Block is a stand alone unit of content, with it's own definition of
 * 1. content items
 * 2. template
 * + some other stuff
 *
 * it should be able to render itself
 */
/**
 * internal helper, to do something and reload the content block
 * @param {ContextOfButton} context
 * @param {string} url
 * @param {ActionParams} params
 * @returns {any}
 */
function getAndReload(context, url, params) {
    return new Promise(function (resolve, reject) {
        context.sxc.webApi.get({
            url: url,
            params: params,
        }).done(function (data, textStatus, jqXHR) {
            if (jqXHR.status === 204 || jqXHR.status === 200) {
                // resolve the promise with the response text
                resolve(data);
            }
            else {
                // otherwise reject with the status text
                // which will hopefully be a meaningful error
                reject(Error(textStatus));
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            reject(Error(errorThrown));
        });
        ;
    }).then(function () { render_1.renderer.reloadAndReInitialize(context); });
}
/**
 * remove an item from a list, then reload
 * @param {ContextOfButton} context
 * @param {number} sortOrder
 * @returns {any}
 */
function removeFromList(context, sortOrder) {
    return getAndReload(context, 'view/module/removefromlist', {
        sortOrder: sortOrder
    });
}
exports.removeFromList = removeFromList;
/**
 * change the order of an item in a list, then reload
 * @param {ContextOfButton} context
 * @param {number} initOrder
 * @param {number} newOrder
 * @returns {any}
 */
function changeOrder(context, initOrder, newOrder) {
    return getAndReload(context, 'view/module/changeorder', {
        sortOrder: initOrder,
        destinationSortOrder: newOrder
    });
}
exports.changeOrder = changeOrder;
/**
 * add an item to the list at this position
 * @param {ContextOfButton} context
 * @param {number} sortOrder
 * @returns {any}
 */
function addItem(context, sortOrder) {
    return getAndReload(context, 'view/module/additem', {
        sortOrder: sortOrder
    });
}
exports.addItem = addItem;
/**
 * set a content-item in this block to published, then reload
 * @param {ContextOfButton} context
 * @param {string} part
 * @param {number} sortOrder
 * @returns {any}
 */
function publish(context, part, sortOrder) {
    return getAndReload(context, 'view/module/publish', {
        part: part,
        sortOrder: sortOrder
    });
}
exports.publish = publish;
/**
 * publish an item using it's ID
 * @param {ContextOfButton} context
 * @param {number} entityId
 * @returns {any}
 */
function publishId(context, entityId) {
    return getAndReload(context, 'view/module/publish', {
        id: entityId
    });
}
exports.publishId = publishId;


/***/ }),
/* 12 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var context_1 = __webpack_require__(5);
var sxc_controller_in_page_1 = __webpack_require__(3);
var api_1 = __webpack_require__(4);
var render_toolbar_1 = __webpack_require__(13);
var toolbar_expand_config_1 = __webpack_require__(29);
var toolbar_settings_1 = __webpack_require__(33);
var log_1 = __webpack_require__(7);
var Constants = __webpack_require__(35);
var tag_toolbar_1 = __webpack_require__(36);
// quick debug - set to false if not needed for production
var dbg = false;
/**
 * create a process - toolbar command to generate toolbars inside a tag
 * @param parentLog
 * @param parentTag
 * @param optionalId
 */
function buildToolbars(parentLog, parentTag, optionalId) {
    var log = new log_1.Log('Tlb.BldAll', parentLog);
    parentTag = $(parentTag || '.DnnModule-' + optionalId);
    // if something says the toolbars are disabled, then skip
    if (parentTag.attr(Constants.toolbar.attr.disable))
        return;
    var toolbars = getToolbarTags(parentTag);
    // no toolbars found, must help a bit because otherwise editing is hard
    if (toolbars.length === 0) {
        toolbars = addFallbackToolbar(parentTag);
        if (toolbars == null)
            return;
    }
    for (var i = 0; i < toolbars.length; i++) {
        var tag = $(toolbars[i]);
        var config = loadConfigFromAttributes(toolbars[i]);
        if (config != null) // is null if load failed
            try {
                convertConfigToToolbarTags(tag, config, log);
            }
            catch (err2) {
                // catch any errors, as this is very common - make sure the others are still rendered
                console.error('error creating toolbar - will skip this one', err2);
            }
    }
}
exports.buildToolbars = buildToolbars;
//////////////////////////////// Private Functions ////////////////////////////////////
/**
 * Load the toolbar configuration from the sxc-toolbar attribute OR the old schema
 * @param tag
 * @return a configuration object or null in case of an error
 */
function loadConfigFromAttributes(tag) {
    try {
        var newConfigFormat = tryGetAttrText(tag, Constants.toolbar.attr.full);
        if (newConfigFormat) {
            return JSON.parse(newConfigFormat);
        }
        else {
            var at = sxc_controller_in_page_1.$2sxcInPage.c.attr;
            var data = getFirstAttribute(tag, at.toolbar, at.toolbarData);
            var settings = getFirstAttribute(tag, at.settings, at.settingsData);
            return {
                toolbar: JSON.parse(data),
                settings: JSON.parse(settings)
            };
        }
    }
    catch (err) {
        console.error('error in settings JSON - probably invalid - make sure you also quote your properties like "name": ...', tag, err);
        return null;
    }
}
/**
 * Take a configuration and convert into a toolbar-menu; also attach the hover-attribute
 * @param tag
 * @param config
 * @param log
 */
function convertConfigToToolbarTags(tag, config, log) {
    var cnt = context_1.context(tag);
    cnt.toolbar = toolbar_expand_config_1.expandToolbarConfig(cnt, config.toolbar, config.settings, log);
    if (tag.attr(Constants.toolbar.attr.full)) {
        // new case, where the full toolbar is included in one setting
        // ReSharper disable once WrongExpressionStatement
        new tag_toolbar_1.TagToolbar(tag, cnt);
        ensureToolbarHoverClass(tag);
    }
    else {
        var toolbar = render_toolbar_1.renderToolbar(cnt);
        // default case, tag is the old <ul> tag, so find the sc-element parent before replacing
        var scElementParent = tag.closest(Constants.toolbar.selectors.ofOldHover);
        tag.replaceWith(toolbar);
        if (scElementParent.length > 0)
            ensureToolbarHoverClass(scElementParent);
    }
}
/** find current toolbars inside this wrapper-tag */
function getToolbarTags(parentTag) {
    var allInner = $(".sc-menu[toolbar],.sc-menu[data-toolbar],[" + Constants.toolbar.attr.full + "]", parentTag);
    // return only those, which don't belong to a sub-item
    var onlyDirectDescendents = allInner
        .filter(function (i, e) { return $(e).closest(Constants.cb.selectors.ofName)[0] === parentTag[0]; });
    if (dbg)
        console.log('found toolbars for parent', parentTag, onlyDirectDescendents);
    return onlyDirectDescendents;
}
/** add hover-attribute to tag */
function ensureToolbarHoverClass(jtag) {
    if (jtag.length <= 0)
        return; // skip in case nothing was given
    var tag = jtag[0];
    if (!tag.hasAttribute(Constants.toolbar.attr.hover))
        tag.setAttribute(Constants.toolbar.attr.hover, '');
}
/** Create a default/fallback toolbar and return it */
function addFallbackToolbar(parentTag) {
    if (dbg)
        console.log("didn't find toolbar, so will auto-create", parentTag);
    var outsideCb = !parentTag.hasClass(Constants.cb.classes.name);
    var contentTag = outsideCb ? parentTag.find("div" + Constants.cb.selectors.ofName) : parentTag;
    // auto toolbar
    var ctx = context_1.context(contentTag);
    if (ctx.ui.autoToolbar === false)
        return null;
    contentTag.attr(Constants.toolbar.attr.full, JSON.stringify(toolbar_settings_1.emptyToolbar));
    return contentTag;
}
/** Find the text of one or more attributes in fallback order, till we found one */
function getFirstAttribute(toolbar, name1, name2) {
    return tryGetAttrText(toolbar, name1) || tryGetAttrText(toolbar, name2) || '{}';
}
/** Get text-content of an attribute (or return null) */
function tryGetAttrText(tag, name) {
    var item1 = tag.attributes.getNamedItem(name);
    return item1 && item1.textContent;
}
function disable(tag) {
    var jtag = $(tag);
    jtag.attr(Constants.toolbar.attr.disable, 'true');
}
exports.disable = disable;
function isDisabled(sxc) {
    var tag = $(api_1.getTag(sxc));
    return !!tag.attr(Constants.toolbar.attr.disable);
}
exports.isDisabled = isDisabled;


/***/ }),
/* 13 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var render_groups_1 = __webpack_require__(70);
var render_helpers_1 = __webpack_require__(28);
function renderToolbar(context) {
    // render groups of buttons
    var groups = render_groups_1.renderGroups(context);
    // render toolbar
    var toolbar = document.createElement('ul');
    toolbar.classList.add('sc-menu');
    toolbar.classList.add('group-0'); // IE11 fix, add each class separately
    // add behaviour classes
    toolbar.classList.add("sc-tb-hover-" + context.toolbar.settings.hover);
    toolbar.classList.add("sc-tb-show-" + context.toolbar.settings.show);
    if (context.toolbar.params.sortOrder === -1) {
        toolbar.classList.add('listContent');
    }
    render_helpers_1.addClasses(toolbar, context.toolbar.settings.classes, ' ');
    // stv: commented because I do not see that we need click event
    // toolbar.setAttribute('onclick', 'var e = arguments[0] || window.event; e.preventDefault();'); // serialize JavaScript because of ajax
    // add button groups to toolbar
    toolbar.setAttribute('group-count', context.toolbar.groups.length.toString());
    for (var g = 0; g < groups.length; g++) {
        toolbar.appendChild(groups[g]);
    }
    return toolbar.outerHTML;
}
exports.renderToolbar = renderToolbar;


/***/ }),
/* 14 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var log_1 = __webpack_require__(7);
// takes an object like "actionname" or { action: "actionname", ... } and changes it to a { command: { action: "actionname" }, ... }
// ReSharper disable once UnusedParameter
function expandButtonConfig(original, sharedProps, parentLog) {
    var log = new log_1.Log('Tlb.ExpBtn', parentLog, 'start');
    // prevent multiple inits
    if (original._expanded || original.command) {
        log.add("already expanded, won't modify");
        return original;
    }
    ;
    // if just a name, turn into a command
    if (typeof original === 'string') {
        log.add("name \"" + original + "\" found, will re-map to .command.action");
        original = { command: { action: original.trim() } };
    }
    ;
    // if it's a command w/action, wrap into command + trim
    if (typeof original.action === 'string') {
        log.add("action found, will move down to .command");
        original.action = original.action.trim();
        original = { command: original };
    }
    // some clean-up
    delete original.action; // remove the action property
    original._expanded = true;
    log.add('done');
    return original;
}
exports.expandButtonConfig = expandButtonConfig;
function getButtonConfigDefaultsV1(name, icon, translateKey, uiOnly, partOfPage, more) {
    // 
    // stv: v1 code
    var partialButtonConfig = {
        icon: function (context) { return "icon-sxc-" + icon; },
        title: function (context) { return "Toolbar." + translateKey; },
        uiActionOnly: function (context) { return uiOnly; },
        partOfPage: function (context) { return partOfPage; },
    };
    Object.assign(partialButtonConfig, more);
    return partialButtonConfig;
}
exports.getButtonConfigDefaultsV1 = getButtonConfigDefaultsV1;
// remove buttons which are not valid based on add condition
function removeDisableButtons(context, full, config, parentLog) {
    var log = new log_1.Log("Tlb.RmvDsb', parentLog, 'start remove disabled buttons for " + full.groups.length + " groups");
    var btnGroups = full.groups;
    for (var g = 0; g < btnGroups.length; g++) {
        var btns = btnGroups[g].buttons;
        removeUnfitButtons(context, btns, config, log);
        log.add('will disable appropriate buttons');
        disableButtons(context, btns, config);
        // remove the group, if no buttons left, or only "more"
        // if (btns.length === 0 || (btns.length === 1 && btns[0].command.action === 'more'))
        if (btns.length === 0 || (btns.length === 1 && btns[0].action.name === 'more')) {
            log.add("found no more buttons except for the \"more\" - will remove that too");
            btnGroups.splice(g--, 1);
        } // remove, and decrement counter
    }
}
exports.removeDisableButtons = removeDisableButtons;
function removeUnfitButtons(context, btns, config, log) {
    var removals = '';
    for (var i = 0; i < btns.length; i++) {
        // let add = btns[i].showCondition;
        // if (add !== undefined)
        //    if (typeof (add) === "function" ? !add(btns[i].command, config) : !add)
        // if (!evalPropOrFunction(btns[i].showCondition, btns[i].command, config, true))
        context.button = btns[i];
        if (btns[i].action && !evalPropOrFunction(btns[i].showCondition, context, config, true)) {
            removals += "#" + i + " \"" + btns[i].action.name + "\"; ";
            btns.splice(i--, 1);
        }
    }
    if (removals)
        log.add("removed buttons: " + removals);
}
function disableButtons(context, btns, config) {
    for (var i = 0; i < btns.length; i++) {
        // btns[i].disabled = evalPropOrFunction(btns[i].disabled, btns[i].command, config, false);
        context.button = btns[i];
        if (btns[i].action) {
            btns[i].disabled = evalPropOrFunction(btns[i].disabled, context, config, false);
        }
        else {
            btns[i].disabled = (function (context) { return false; });
        }
    }
}
function evalPropOrFunction(propOrFunction, context, config, fallback) {
    if (propOrFunction === undefined || propOrFunction === null) {
        return fallback;
    }
    if (typeof (propOrFunction) === 'function') {
        return propOrFunction(context, config);
    }
    else {
        return propOrFunction;
    }
}
/**
 * enhance button-object with default icons, etc.
 * @param btn
 * @param group
 * @param fullToolbarConfig
 * @param actions
 */
function addDefaultBtnSettings(btn, group, fullToolbarConfig, actions, log) {
    // log.add(`adding default btn settings for ${btn.action.name}`);
    log.add("adding default btn settings for " + function () { return btn.action.name; });
    for (var d = 0; d < btnProperties.length; d++) {
        fallbackBtnSetting(btn, group, fullToolbarConfig, actions, btnProperties[d]);
    }
}
exports.addDefaultBtnSettings = addDefaultBtnSettings;
var btnProperties = [
    'classes',
    'icon',
    'title',
    'dynamicClasses',
    'showCondition',
    'disabled'
];
var prvProperties = [
    'defaults',
    'params',
    'name'
];
/**
 * configure missing button properties with various fallback options
 * @param btn
 * @param group
 * @param fullToolbarConfig
 * @param actions
 * @param propName
 */
function fallbackBtnSetting(btn, group, fullToolbarConfig, actions, propName) {
    if (btn[propName]) {
        // if already defined, use the already defined property
        btn[propName] = btn[propName];
    }
    else if (group.defaults &&
        group.defaults[propName]) {
        // if the group has defaults, try use that property
        btn[propName] = group.defaults[propName];
    }
    else if (fullToolbarConfig &&
        fullToolbarConfig.defaults &&
        fullToolbarConfig.defaults[propName]) {
        // if the toolbar has defaults, try use that property
        btn[propName] = fullToolbarConfig.defaults[propName];
    }
    else if (btn.action &&
        btn.action.name &&
        actions.get(btn.action.name) &&
        actions.get(btn.action.name).buttonConfig &&
        actions.get(btn.action.name).buttonConfig[propName]) {
        // if there is an action, try to use that property name
        btn[propName] = actions.get(btn.action.name).buttonConfig[propName];
    }
}
// ReSharper disable once UnusedParameter
function customize(toolbar) {
    // if (!toolbar.settings) return;
    // let set = toolbar.settings;
    // if (set.autoAddMore) {
    //    console.log("auto-more");
    //    let grps = toolbar.groups;
    //    for (let g = 0; g < grps.length; g++) {
    //        let btns = grps[g];
    //        for (let i = 0; i < btns.length; i++) {
    //        }
    //    }
    // }
}
exports.customize = customize;


/***/ }),
/* 15 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var log_1 = __webpack_require__(7);
var HasLog = /** @class */ (function () {
    /**
     * initialize the logger
     * ideally it has a parent-logger to attach to
     * @param logName name to show in the logger
     * @param parentLog parent-logger to attach to
     * @param initialMessage optional start-message to log
     */
    function HasLog(logName, parentLog, initialMessage) {
        var _this = this;
        this.parentLog = parentLog;
        this.initLog = function (name, parentLog, initialMessage) { return _this.initLogInternal(name, parentLog, initialMessage); };
        this.logId = 'unknwn';
        this.linkLog = function (parentLog) { return _this.log.linkLog(parentLog); };
        this.initLogInternal(logName, parentLog, initialMessage);
    }
    HasLog.prototype.initLogInternal = function (name, parentLog, initialMessage) {
        if (this.log == null)
            // standard & most common case: just create log
            this.log = new log_1.Log(name, parentLog, initialMessage);
        else {
            // late-init case, where the log was already created - just reconfig keeping what was in it
            this.log.rename(name);
            this.linkLog(parentLog);
            if (initialMessage != null)
                this.log.add(initialMessage);
        }
    };
    return HasLog;
}());
exports.HasLog = HasLog;


/***/ }),
/* 16 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var window_in_page_1 = __webpack_require__(1);
var api_1 = __webpack_require__(4);
var start_1 = __webpack_require__(37);
var build_toolbars_1 = __webpack_require__(12);
var main_content_block_1 = __webpack_require__(81);
var web_api_promises_1 = __webpack_require__(39);
/*
 * this is the content block manager in the browser
 *
 * A Content Block is a stand alone unit of content, with it's own definition of
 * 1. content items
 * 2. template
 * + some other stuff
 *
 * it should be able to render itself
 */
var Renderer = /** @class */ (function () {
    function Renderer() {
    }
    /**
     * Show a message where the content of a module should be - usually as placeholder till something else happens
     * @param {ContextOfButton} context
     * @param {string} newContent
     * @returns {} nothing
     */
    Renderer.prototype.showMessage = function (context, newContent) {
        $(api_1.getTag(context.sxc)).html(newContent);
    };
    /**
     * this one assumes a replace / change has already happened, but now must be finalized...
     * @param {ContextOfButton} context
     * @param {boolean} forceAjax
     * @param {boolean} preview
     */
    Renderer.prototype.reloadAndReInitialize = function (context, forceAjax, preview) {
        // if ajax is not supported, we must reload the whole page
        if (!forceAjax && !context.app.supportsAjax) {
            window_in_page_1.windowInPage.location.reload();
            return Promise.resolve();
        }
        return this.ajaxLoad(context, main_content_block_1.MainContentBlock.cUseExistingTemplate, preview)
            .then(function (result) {
            // If Evoq, tell Evoq that page has changed if it has changed (Ajax call)
            if (window_in_page_1.windowInPage.dnn_tabVersioningEnabled) { // this only exists in evoq or on new DNNs with tabVersioning
                try {
                    window_in_page_1.windowInPage.dnn.ContentEditorManager.triggerChangeOnPageContentEvent();
                }
                catch (e) {
                    // ignore
                }
            }
            // 2017-09-02 2dm - believe this was meant to re-init the dialog manager, but it doesn't actually work
            // must check for side-effects, which would need the manager to re-build the configuration
            // 2018-11-03 2dm disabled completely for now
            // quickDialog.hide();
            return result;
        }).catch(function (error) { return console.log('Error in reloadAndReInitialize', error); });
    };
    /**
     * ajax-call, then replace
     * @param {ContextOfButton} context
     * @param {number} alternateTemplateId
     * @param {boolean} justPreview
     */
    Renderer.prototype.ajaxLoad = function (context, alternateTemplateId, justPreview) {
        var _this = this;
        return web_api_promises_1.getPreviewWithTemplate(context, alternateTemplateId)
            .then(function (result) {
            _this.replaceContentBlock(context, result, justPreview);
        })
            .then(function () {
            start_1.reset();
        }); // reset quick-edit, because the config could have changed
    };
    /**
     * ajax update/replace the content of the content-block
     * optionally also initialize the toolbar (if not just preview)
     * @param {ContextOfButton} context
     * @param {string} newContent
     * @param {boolean} justPreview
     */
    Renderer.prototype.replaceContentBlock = function (context, newContent, justPreview) {
        try {
            var newDom = $(newContent);
            // Must disable toolbar before we attach to DOM
            if (justPreview)
                build_toolbars_1.disable(newDom);
            $(api_1.getTag(context.sxc)).replaceWith(newDom);
            // reset the cache, so the sxc-object is refreshed
            context.sxc.recreate(true);
        }
        catch (e) {
            console.log('Error while rendering template:', e);
        }
    };
    return Renderer;
}());
exports.renderer = new Renderer();


/***/ }),
/* 17 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
// ReSharper disable once InconsistentNaming
exports.DebugConfig = {
    cms: {
        autoDump: false,
        run: true
    },
    qDialog: {
        showHide: true
    },
    bootstrap: {
        initInstance: true
    },
    state: {
        change: true,
        get: false
    }
};


/***/ }),
/* 18 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var build_toolbars_1 = __webpack_require__(12);
var render_1 = __webpack_require__(16);
var web_api_promises_1 = __webpack_require__(39);
/**
 * prepare the instance so content can be added
 * this ensure the content-group has been created, which is required to add content
 * @param {ContextOfButton} context
 * @returns {any}
 */
function prepareToAddContent(context, useModuleList) {
    var isCreated = context.contentBlock.isCreated;
    if (isCreated || !useModuleList)
        return Promise.resolve();
    // return persistTemplate(sxc, null);
    // let manage = sxc.manage;
    // let contentGroup = manage._editContext.ContentGroup;
    // let showingAjaxPreview = $2sxc._toolbarManager.isDisabled(sxc);
    // let groupExistsAndTemplateUnchanged = !!contentGroup.HasContent; // && !showingAjaxPreview;
    var templateId = context.contentBlock.templateId;
    // template has not changed
    // if (groupExistsAndTemplateUnchanged) return $.when(null);
    // persist the template
    return updateTemplate(context, templateId, true);
}
exports.prepareToAddContent = prepareToAddContent;
/**
 * Update the template and adjust UI accordingly.
 * @param {ContextOfButton} context
 * @param {number} templateId
 * @param {boolean} forceCreate
 */
function updateTemplateFromDia(context, templateId) {
    var wasShowingPreview = build_toolbars_1.isDisabled(context.sxc);
    return updateTemplate(context, templateId, false)
        .then(function () {
        // only reload on ajax, not on app as that was already re-loaded on the preview
        // necessary to show the original template again
        if (wasShowingPreview)
            render_1.renderer.reloadAndReInitialize(context);
    });
}
exports.updateTemplateFromDia = updateTemplateFromDia;
/**
 * Update the template.
 */
function updateTemplate(context, templateId, forceCreate) {
    return web_api_promises_1.saveTemplate(context, templateId, forceCreate).then(function (data) {
        if (!data)
            return null;
        // fixes a special case where the guid is given with quotes (depends on version of angularjs) issue #532
        var newGuid = data.replace(/[\",\']/g, '');
        if (console)
            console.log("created content group {" + newGuid + "}");
        return context.contentBlock.contentGroupId = newGuid;
    }).catch(function () {
        // error handling
        return alert('error - result not ok, was not able to create ContentGroup');
    });
}


/***/ }),
/* 19 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var old_parameters_adapter_1 = __webpack_require__(71);
var render_helpers_1 = __webpack_require__(28);
/**
 * generate the html for a button
 * @param sxc instance sxc
 * @param buttonConfig
 * @param groupIndex group-index in which the button is shown
 */
function renderButton(context, groupIndex) {
    var buttonConfig = context.button;
    // retrieve configuration for this button
    var oldParamsAdapter = old_parameters_adapter_1.oldParametersAdapter(buttonConfig.action);
    var onclick = '';
    if (!buttonConfig.disabled) {
        onclick = "$2sxc(" + context.instance.id + ", " + context.contentBlock.id + ").manage.run(" + JSON.stringify(oldParamsAdapter) + ", event);";
        // onclick = `$2sxc(${context.instance.id}, ${context.contentBlock.id}).manage.run2($2sxc.context(this), ${JSON.stringify(oldParamsAdapter)}, event);`;
    }
    var button = document.createElement('a');
    if (buttonConfig.action) {
        button.classList.add("sc-" + buttonConfig.action.name);
    }
    button.classList.add("group-" + groupIndex);
    if (buttonConfig.disabled) {
        button.classList.add('disabled');
    }
    render_helpers_1.addClasses(button, buttonConfig.classes, ',');
    if (buttonConfig.dynamicClasses) {
        var dynamicClasses = buttonConfig.dynamicClasses(context);
        render_helpers_1.addClasses(button, dynamicClasses, ' ');
    }
    button.setAttribute('onclick', onclick); // serialize JavaScript because of ajax
    if (buttonConfig.title) {
        button.setAttribute('data-i18n', "[title]" + buttonConfig.title(context)); // localization support
    }
    var box = document.createElement('div');
    var symbol = document.createElement('i');
    if (buttonConfig.icon) {
        render_helpers_1.addClasses(symbol, buttonConfig.icon(context), ' ');
    }
    symbol.setAttribute('aria-hidden', 'true');
    box.appendChild(symbol);
    button.appendChild(box);
    return button;
}
exports.renderButton = renderButton;


/***/ }),
/* 20 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ButtonAction = /** @class */ (function () {
    function ButtonAction(name, contentType, params) {
        this.name = name;
        this.params = params;
        if (!params) {
            this.params = {};
        }
        if (contentType) {
            Object.assign(this.params, { contentType: contentType });
        }
    }
    return ButtonAction;
}());
exports.ButtonAction = ButtonAction;


/***/ }),
/* 21 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ButtonConfig = /** @class */ (function () {
    function ButtonConfig(action, partialConfig) {
        this.name = '';
        this.classes = '';
        this.show = null; // maybe
        this.dynamicDisabled = function () { return false; }; // maybe
        if (action && action.commandDefinition && action.commandDefinition.buttonConfig) {
            this.action = action;
            // get defaults from action commandDefinition
            Object.assign(this, action.commandDefinition.buttonConfig);
        }
        if (partialConfig) {
            Object.assign(this, partialConfig);
        }
    }
    return ButtonConfig;
}());
exports.ButtonConfig = ButtonConfig;


/***/ }),
/* 22 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var DebugConfig_1 = __webpack_require__(17);
var Container = __webpack_require__(41);
var ContainerSize = __webpack_require__(42);
var QuickEditState = __webpack_require__(43);
var UrlHandler = __webpack_require__(85);
var dbg = DebugConfig_1.DebugConfig.qDialog;
var diagShowClass = 'dia-select';
/** dialog manager - the currently active dialog object */
var current = null;
/**
 * this is a dialog manager which is in charge of all quick-dialogues
 * it always has a reference to the latest dialog created by any module instance
 */
var QuickDialogManager = /** @class */ (function () {
    function QuickDialogManager() {
    }
    /**
     * Determines if any dialog is currently showing
     */
    QuickDialogManager.prototype.isVisible = function () {
        return current != null;
    };
    /**
     * toggle visibility
     * @param {boolean} [show] true/false optional
     */
    QuickDialogManager.prototype.setVisible = function (show) {
        var cont = Container.getOrCreate();
        // if (show === undefined)
        //  show = !cont.hasClass(diagShowClass);
        // show/hide visually
        cont.toggleClass(diagShowClass, show);
        this.rememberDialogState(Container.getIFrame(cont), show);
        current = show ? Container.getIFrame() : null;
    };
    /**
     * show / reset the current iframe to use new url and callback
     * @param {ContextOfButton} context object
     * @param {string} url - url to show
     * @param {function()} closeCallback - callback event
     * @param {boolean} isFullscreen - if it should open full screen
     * @param {string} [dialogName] - optional name of dialog, to check if it's already open
     * @returns {any} jquery object of the iframe
     */
    QuickDialogManager.prototype.showOrToggleFromToolbar = function (context, url, isFullscreen, dialogName) {
        ContainerSize.setSize(isFullscreen);
        var iFrame = Container.getIFrame();
        // in case it's a toggle
        if (this.isVisible()) {
            // check if we're just toggling the current, or will show a new one afterwards
            var currentPromise = dialogName &&
                current &&
                current.bridge.isConfiguredFor(context.sxc.cacheKey, dialogName)
                ? this.promise
                : null;
            this.cancel(current.bridge);
            // just a hide this, return the old promise
            if (currentPromise)
                return currentPromise;
        }
        var dialogUrl = UrlHandler.setUrlToQuickDialog(url);
        iFrame.bridge.setup(context.sxc, dialogName);
        iFrame.setAttribute('src', dialogUrl);
        // if the window had already been loaded, re-init
        if (iFrame.contentWindow && iFrame.contentWindow.reboot)
            iFrame.contentWindow.reboot();
        // make sure it's visible'
        this.setVisible(true);
        return this.promiseRestart();
    };
    QuickDialogManager.prototype.cancel = function (bridge) {
        this.setVisible(false);
        QuickEditState.cancelled.set('true');
        this.resolvePromise(bridge.changed);
    };
    QuickDialogManager.prototype.rememberDialogState = function (iframe, state) {
        if (dbg.showHide)
            console.log("qDialog persistDia(..., " + state + ")");
        if (state) {
            var cbId = iframe.bridge
                .getContext()
                .contentBlock.id.toString();
            if (dbg.showHide)
                console.log("contentBlockId: " + cbId + ")");
            return QuickEditState.cbId.set(cbId);
        }
        else
            return QuickEditState.cbId.remove();
    };
    QuickDialogManager.prototype.promiseRestart = function () {
        var _this = this;
        this.promise = new Promise(function (resolve) { return (_this.resolvePromise = resolve); });
        return this.promise;
    };
    return QuickDialogManager;
}());
exports.quickDialog = new QuickDialogManager();


/***/ }),
/* 23 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var UserOfEditContext = /** @class */ (function () {
    function UserOfEditContext() {
    }
    UserOfEditContext.fromContext = function (context) {
        var user = new UserOfEditContext();
        user.canDesign = context.user.canDesign;
        user.canDevelop = context.user.canDevelop;
        return user;
    };
    return UserOfEditContext;
}());
exports.UserOfEditContext = UserOfEditContext;


/***/ }),
/* 24 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var DialogPaths;
(function (DialogPaths) {
    DialogPaths["ng1"] = "dist/dnn/ui.html";
    DialogPaths["quickDialog"] = "dist/ng/ui.html";
    DialogPaths["ng8"] = "dist/ng-edit/ui.html";
})(DialogPaths = exports.DialogPaths || (exports.DialogPaths = {}));


/***/ }),
/* 25 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var sxc_1 = __webpack_require__(6);
var cmds_strategy_factory_1 = __webpack_require__(105);
var mod_1 = __webpack_require__(45);
var quick_e_1 = __webpack_require__(2);
var selectors_instance_1 = __webpack_require__(8);
/** add a clipboard to the quick edit */
/**
 * perform copy and paste commands - needs the clipboard
 * @param cbAction
 * @param list
 * @param index
 * @param type
 */
function copyPasteInPage(cbAction, list, index, type) {
    var newClip = createSpecs(type, list, index);
    // action!
    switch (cbAction) {
        case 'select':
            mark(newClip);
            break;
        case 'paste':
            var from = exports.data.index;
            var to = newClip.index;
            // check that we only move block-to-block or module to module
            if (exports.data.type !== newClip.type)
                return alert("can't move module-to-block; move only works from module-to-module or block-to-block");
            if (isNaN(from) || isNaN(to) || from === to) // || from + 1 === to) // this moves it to the same spot, so ignore
                return clear(); // don't do anything
            // cb-numbering is a bit different, because the selector is at the bottom
            // only there we should also skip on +1;
            if (newClip.type === selectors_instance_1.selectors.cb.id && from + 1 === to)
                return clear(); // don't do anything
            if (type === selectors_instance_1.selectors.cb.id) {
                var sxc = sxc_1.getSxcInstance(list);
                sxc.manage._getCbManipulator().move(newClip.parent, newClip.field, from, to);
            }
            else {
                // sometimes missing oldClip.item
                // if (clipboard.data.item)
                mod_1.Mod.move(exports.data, newClip, from, to);
            }
            clear();
            break;
        default:
    }
    return null;
}
exports.copyPasteInPage = copyPasteInPage;
/**
 * clipboard object - remembers what module (or content-block) was previously copied / needs to be pasted
 */
exports.data = {};
function mark(newData) {
    if (newData) {
        // if it was already selected with the same thing, then release it
        if (exports.data && exports.data.item === newData.item)
            return clear();
        exports.data = newData;
    }
    $("." + selectors_instance_1.selectors.selected).removeClass(selectors_instance_1.selectors.selected); // clear previous markings
    // sometimes missing data.item
    if (!exports.data.item) {
        return;
    }
    var cb = $(exports.data.item);
    cb.addClass(selectors_instance_1.selectors.selected);
    if (cb.prev().is('iframe'))
        cb.prev().addClass(selectors_instance_1.selectors.selected);
    setSecondaryActionsState(true);
    quick_e_1.$quickE.selected.toggle(cb, exports.data.type);
}
exports.mark = mark;
function clear() {
    $("." + selectors_instance_1.selectors.selected).removeClass(selectors_instance_1.selectors.selected);
    exports.data = null;
    setSecondaryActionsState(false);
    quick_e_1.$quickE.selected.toggle(false);
}
exports.clear = clear;
function createSpecs(type, list, index) {
    var listItems = list.find(selectors_instance_1.selectors[type].selector);
    var currentItem;
    if (index >= listItems.length) {
        // when paste module below the last module in pane
        // index is 1 larger than the length, then select last
        currentItem = listItems[listItems.length - 1];
    }
    else {
        currentItem = listItems[index];
    }
    var editContext = JSON.parse(list.attr(selectors_instance_1.selectors.cb.context) || null) || { parent: 'dnn', field: list.id };
    return {
        parent: editContext.parent,
        field: editContext.field,
        list: list,
        item: currentItem,
        index: index,
        type: type,
    };
}
exports.createSpecs = createSpecs;
function setSecondaryActionsState(state) {
    var btns = $('a.sc-content-block-menu-btn');
    btns = btns.filter('.icon-sxc-paste');
    btns.toggleClass('sc-unavailable', !state);
}
var cmdsStrategyFactory = new cmds_strategy_factory_1.CmdsStrategyFactory();
/**
 * bind clipboard actions
 */
$('a', quick_e_1.$quickE.selected).click(function () {
    var action = $(this).data('action');
    var clip = exports.data;
    switch (action) {
        case 'delete':
            return cmdsStrategyFactory.delete(clip);
        case 'sendToPane':
            return mod_1.Mod.sendToPane();
        default:
            throw new Error("unexpected action: " + action);
    }
});


/***/ }),
/* 26 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var engine_1 = __webpack_require__(52);
var context_1 = __webpack_require__(5);
var context_of_instance_1 = __webpack_require__(27);
var DebugConfig_1 = __webpack_require__(17);
var has_log_1 = __webpack_require__(15);
var log_1 = __webpack_require__(7);
var logId = 'Cms.Api';
var Cms = /** @class */ (function (_super) {
    __extends(Cms, _super);
    function Cms() {
        var _this = _super.call(this, logId, null) || this;
        /**
         * if true (default) will reset the log everytime something is done
         * if false, will preserve the log over multiple calls
         */
        _this.autoReset = true;
        _this.autoDump = DebugConfig_1.DebugConfig.cms.autoDump;
        return _this;
    }
    /**
     * reset / clear the log
     */
    Cms.prototype.resetLog = function () {
        this.log = new log_1.Log(logId, null, 'log was reset');
    };
    Cms.prototype.run = function (context, nameOrSettings, eventOrSettings, event) {
        var _this = this;
        var realContext = context_of_instance_1.isContextOfInstance(context)
            ? context
            : context_1.context(context);
        return this.do(function () {
            return new engine_1.Engine(_this.log).detectParamsAndRun(realContext, nameOrSettings, eventOrSettings, event);
        });
    };
    /**
     * reset/clear the log if alwaysResetLog is true
     */
    Cms.prototype.do = function (innerCall) {
        if (this.autoReset)
            this.resetLog();
        var result = innerCall();
        if (this.autoDump)
            console.log(this.log.dump());
        return result;
    };
    return Cms;
}(has_log_1.HasLog));
exports.Cms = Cms;


/***/ }),
/* 27 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var context_of_page_1 = __webpack_require__(61);
var ContextOfInstance = /** @class */ (function (_super) {
    __extends(ContextOfInstance, _super);
    function ContextOfInstance() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContextOfInstance;
}(context_of_page_1.ContextOfPage));
exports.ContextOfInstance = ContextOfInstance;
function isContextOfInstance(thing) {
    var maybeButton = thing;
    return maybeButton.sxc !== undefined && maybeButton.instance !== undefined;
}
exports.isContextOfInstance = isContextOfInstance;


/***/ }),
/* 28 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * helper method to add list of zero to many classes to Element
 * @param element
 * @param classes
 * @param spliter
 */
function addClasses(element, classes, spliter) {
    if (classes) {
        var classessArray = classes.split(spliter);
        for (var c = 0; c < classessArray.length; c++) {
            if (classessArray[c]) {
                element.classList.add(classessArray[c]);
            }
        }
    }
}
exports.addClasses = addClasses;


/***/ }),
/* 29 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var log_1 = __webpack_require__(7);
var instance_config_1 = __webpack_require__(73);
var old_toolbar_settings_adapter_1 = __webpack_require__(74);
var expand_button_config_1 = __webpack_require__(14);
var expand_group_config_1 = __webpack_require__(75);
var toolbar_config_1 = __webpack_require__(76);
var toolbar_settings_1 = __webpack_require__(33);
var toolbar_config_templates_1 = __webpack_require__(34);
function expandToolbarConfig(context, toolbarData, toolbarSettings, parentLog) {
    var log = new log_1.Log('Tlb.ExpTop', parentLog, 'expand start');
    if (toolbarData === {} && toolbarSettings === {}) {
        log.add('no data or settings found, will use default toolbar');
        toolbarSettings = toolbar_settings_1.settingsForEmptyToolbar;
    }
    // if it has an action or is an array, keep that. Otherwise get standard buttons
    toolbarData = toolbarData || {}; // if null/undefined, use empty object
    var unstructuredConfig = toolbarData;
    if (!toolbarData.action && !toolbarData.groups && !toolbarData.buttons && !Array.isArray(toolbarData)) {
        log.add('no toolbar details found, will use standard toolbar template');
        var toolbarTemplate = toolbar_config_templates_1.ToolbarConfigTemplates.Instance(log).get('default'); // use default toolbar template
        unstructuredConfig = JSON.parse(JSON.stringify(toolbarTemplate)); // deep copy toolbar template
        unstructuredConfig.params = ((toolbarData) && Array.isArray(toolbarData) && toolbarData[0]) || toolbarData; // these are the default command parameters
    }
    var instanceConfig = instance_config_1.InstanceConfig.fromContext(context);
    // whatever we had, if more settings were provided, override with these...
    var config = buildFullDefinition(context, unstructuredConfig, instanceConfig, toolbarSettings, log);
    log.add('expand done');
    return config;
}
exports.expandToolbarConfig = expandToolbarConfig;
/**
 * take any common input format and convert it to a full toolbar-structure definition
 * can handle the following input formats (the param unstructuredConfig):
 * complete tree (detected by "groups): { groups: [ {}, {}], name: ..., defaults: {...} }
 * group of buttons (detected by "buttons): { buttons: "..." | [], name: ..., ... }
 * list of buttons (detected by IsArray with action): [ { action: "..." | []}, { action: ""|[]} ]
 * button (detected by "command"): { command: ""|[], icon: "..", ... }
 * just a command (detected by "action"): { entityId: 17, action: "edit" }
 * array of commands: [{entityId: 17, action: "edit"}, {contentType: "blog", action: "new"}]
 * @param unstructuredConfig
 * @param allActions
 * @param instanceConfig
 * @param toolbarSettings
 */
function buildFullDefinition(toolbarContext, unstructuredConfig, instanceConfig, toolbarSettings, parentLog) {
    var log = new log_1.Log('Tlb.BldFul', parentLog, 'start');
    var fullConfig = ensureDefinitionTree(unstructuredConfig, toolbarSettings, log);
    // ToDo: don't use console.log in production
    if (unstructuredConfig.debug)
        console.log('toolbar: detailed debug on; start build full Def');
    expand_group_config_1.expandButtonGroups(fullConfig, log);
    expand_button_config_1.removeDisableButtons(toolbarContext, fullConfig, instanceConfig, log);
    if (fullConfig.debug)
        console.log('after remove: ', fullConfig);
    expand_button_config_1.customize(fullConfig);
    return fullConfig;
}
;
//#region build initial toolbar object
/**
 * this will take an input which could already be a tree, but it could also be a
 * button-definition, or just a string, and make sure that afterwards it's a tree with groups
 * the groups could still be in compact form, or already expanded, depending on the input
 * output is object with:
 * - groups containing buttons[], but buttons could still be very flat
 * - defaults, already officially formatted
 * - params, officially formatted
 * @param unstructuredConfig
 * @param toolbarSettings
 */
function ensureDefinitionTree(unstructuredConfig, toolbarSettings, parentLog) {
    var log = new log_1.Log("Tlb.DefTre", parentLog, "start");
    // original is null/undefined, just return empty set
    if (!unstructuredConfig)
        throw ("preparing toolbar, with nothing to work on: " + unstructuredConfig);
    // ensure that if it's just actions or buttons, they are then processed as arrays with 1 entry
    if (!Array.isArray(unstructuredConfig) && (unstructuredConfig.action || unstructuredConfig.buttons)) {
        log.add('found no array, but detected action/buttons properties, will wrap config into array');
        unstructuredConfig = [unstructuredConfig];
    }
    // ensure that arrays of actions or buttons are re-mapped to the right structure node
    if (Array.isArray(unstructuredConfig) && unstructuredConfig.length) {
        log.add('detected array with length');
        if (unstructuredConfig[0].buttons) {
            log.add('detected buttons on first item, assume button-group, moving into .groups');
            unstructuredConfig.groups = unstructuredConfig; // move "down"
        }
        else if (unstructuredConfig[0].command || unstructuredConfig[0].action) {
            log.add('detected command or action on first item, assume buttons, move into .groups[buttons] ');
            unstructuredConfig = { groups: [{ buttons: unstructuredConfig }] };
        }
        else {
            log.add('can\'t detect what this is - show warning');
            console.warn("toolbar tried to build toolbar but couldn't detect type of this:", unstructuredConfig);
        }
    }
    else
        log.add('not array or has no items');
    var toolbarConfig = new toolbar_config_1.ToolbarConfig();
    // toolbarConfig.groupConfig = new GroupConfig(original.groups as ButtonConfig[]);
    toolbarConfig.groups = unstructuredConfig.groups || []; // the groups of buttons
    toolbarConfig.params = unstructuredConfig.params || {}; // these are the default command parameters
    toolbarConfig.settings = Object.assign({}, toolbar_settings_1.defaultToolbarSettings, unstructuredConfig.settings, old_toolbar_settings_adapter_1.oldToolbarSettingsAddapter(toolbarSettings));
    // todo: old props, remove
    toolbarConfig.name = unstructuredConfig.name || 'toolbar'; // name, no real use
    toolbarConfig.debug = unstructuredConfig.debug || false; // show more debug info
    toolbarConfig.defaults = unstructuredConfig.defaults || {}; // the button defaults like icon, etc.
    log.add('done');
    return toolbarConfig;
}
;
//#endregion initial toolbar object


/***/ }),
/* 30 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function parametersAdapter(oldParameters) {
    var newParams = oldParameters;
    // some clean-up
    delete newParams.action; // remove the action property
    return newParams;
}
exports.parametersAdapter = parametersAdapter;


/***/ }),
/* 31 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function settingsAdapter(oldSettings) {
    var newSettings = {};
    // 'classes',
    if (oldSettings.classes) {
        newSettings.classes = oldSettings.classes;
    }
    // 'dialog',
    if (oldSettings.dialog) {
        newSettings.dialog = evalPropOrFunction(oldSettings.dialog);
    }
    // 'disabled'
    if (oldSettings.disabled) {
        newSettings.disabled = evalPropOrFunction(oldSettings.disabled);
    }
    // 'dynamicClasses',
    if (oldSettings.dynamicClasses) {
        newSettings.dynamicClasses = evalPropOrFunction(oldSettings.dynamicClasses);
    }
    // 'fullScreen',
    if (oldSettings.fullScreen) {
        newSettings.fullScreen = evalPropOrFunction(oldSettings.fullScreen);
    }
    // 'icon',
    if (oldSettings.icon) {
        newSettings.icon = evalPropOrFunction(oldSettings.icon);
    }
    // 'inlineWindow',
    if (oldSettings.inlineWindow) {
        newSettings.inlineWindow = evalPropOrFunction(oldSettings.inlineWindow);
    }
    // 'newWindow',
    if (oldSettings.newWindow) {
        newSettings.newWindow = evalPropOrFunction(oldSettings.newWindow);
    }
    // partOfPage
    if (oldSettings.partOfPage) {
        newSettings.partOfPage = evalPropOrFunction(oldSettings.partOfPage);
    }
    // 'showCondition',
    if (oldSettings.showCondition) {
        newSettings.showCondition = evalPropOrFunction(oldSettings.showCondition);
    }
    // 'title',
    if (oldSettings.title) {
        newSettings.title = evalPropOrFunction(oldSettings.title);
    }
    return newSettings;
}
exports.settingsAdapter = settingsAdapter;
function evalPropOrFunction(propOrFunction) {
    if (propOrFunction === undefined || propOrFunction === null) {
        return false;
    }
    if (typeof (propOrFunction) === 'function') {
        return propOrFunction;
    }
    else {
        return function (context) { return propOrFunction; };
    }
}


/***/ }),
/* 32 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * entity support (vertical compatibility for pre 2sxc v9.x)
 * does some clean-up work on a button-definition object
 * because the target item could be specified directly, or in a complex internal object called entity
 * @param actDef
 */
function flattenActionDefinition(actDef) {
    if (!actDef.entity || !actDef.entity._2sxcEditInformation) {
        return;
    }
    var editInfo = actDef.entity._2sxcEditInformation;
    actDef.useModuleList = (editInfo.sortOrder !== undefined); // has sort-order, so use list
    if (actDef.entity.EntityId !== undefined) {
        actDef.entityId = actDef.entity.EntityId;
    }
    if (editInfo.sortOrder !== undefined) {
        actDef.sortOrder = editInfo.sortOrder;
    }
    delete actDef.entity; // clean up edit-info
}
exports.flattenActionDefinition = flattenActionDefinition;


/***/ }),
/* 33 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/** contains toolbar behaviour settings like float, etc. */
var ToolbarSettings = /** @class */ (function () {
    function ToolbarSettings(toolbarSettings) {
        this.autoAddMore = null; //  [true: used to be right/start]
        this.hover = 'right';
        this.show = 'hover';
        this.classes = '';
        if (toolbarSettings) {
            Object.assign(this, toolbarSettings);
        }
    }
    return ToolbarSettings;
}());
exports.ToolbarSettings = ToolbarSettings;
// ToDo: refactor to avoid side-effects
exports.defaultToolbarSettings = new ToolbarSettings({
    autoAddMore: null,
    hover: 'right',
    show: 'hover',
});
/** default / fallback settings for toolbars when nothings is specified */
exports.settingsForEmptyToolbar = new ToolbarSettings({
    autoAddMore: 'start',
    hover: 'left',
    show: 'hover',
});
exports.emptyToolbar = {
    toolbar: '',
    settings: exports.settingsForEmptyToolbar,
};


/***/ }),
/* 34 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var default_toolbar_template_1 = __webpack_require__(77);
var left_toolbar_template_1 = __webpack_require__(78);
var has_log_1 = __webpack_require__(15);
var ToolbarConfigTemplates = /** @class */ (function (_super) {
    __extends(ToolbarConfigTemplates, _super);
    function ToolbarConfigTemplates(parentLog) {
        var _this = _super.call(this, 'Tlb.TmpMan', parentLog, "build") || this;
        _this.configTemplateList = [];
        _this.list = {}; // hash - table of templates, to be used a list()['template - name']
        _this.add('default', default_toolbar_template_1.defaultToolbarTemplate);
        _this.add('left', left_toolbar_template_1.leftToolbarTemplate);
        return _this;
    }
    ToolbarConfigTemplates.Instance = function (parentLog) {
        // check if an instance of the class is already created
        if (this.singleton == null) {
            // If not created create an instance of the class
            // store the instance in the variable
            this.singleton = new ToolbarConfigTemplates(parentLog);
        }
        // return the singleton object
        return this.singleton;
    };
    // a single template – usually 'default'
    ToolbarConfigTemplates.prototype.get = function (name) {
        return this.list[name];
    };
    // adds a config to the list, if it doesn't exist
    ToolbarConfigTemplates.prototype.add = function (name, template, force) {
        this.list[name] = template;
    };
    ToolbarConfigTemplates.singleton = null; // A variable which stores the singleton object. Initially, the variable acts like a placeholder
    return ToolbarConfigTemplates;
}(has_log_1.HasLog));
exports.ToolbarConfigTemplates = ToolbarConfigTemplates;


/***/ }),
/* 35 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/** Toolbar constants */
exports.toolbar = {
    attr: {
        full: 'sxc-toolbar',
        hover: 'sxc-toolbar-hover',
        disable: 'data-disable-toolbar',
    },
    classes: {
        oldHover: 'sc-element',
    },
    selectors: {
        ofOldHover: '.sc-element'
    }
};
/** ContentBlock constants  */
exports.cb = {
    classes: {
        name: 'sc-content-block'
    },
    selectors: {
        ofName: '.sc-content-block'
    }
};


/***/ }),
/* 36 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var render_toolbar_1 = __webpack_require__(13);
/**
 * Remove orphan tag-toolbars from DOM
 */
function CleanupTagToolbars() {
    var tagToolbars = $("[" + tagToolbarForAttr + "]");
    tagToolbars.each(function (i, e) {
        var id = $(e).attr(tagToolbarForAttr);
        if (!$("[" + tagToolbarAttr + "=" + id + "]").length) {
            $(e).remove();
        }
    });
}
exports.CleanupTagToolbars = CleanupTagToolbars;
var tagToolbarPadding = 4, tagToolbarPaddingRight = 0, toolbarHeight = 20;
var tagToolbarAttr = 'data-tagtoolbar';
var tagToolbarForAttr = 'data-tagtoolbar-for';
/**
 * Returns the body offset if positioning is relative or absolute
 */
function getBodyOffset() {
    var body = $('body');
    var bodyPos = body.css('position');
    if (bodyPos === 'relative' || bodyPos === 'absolute') {
        var offset = body.offset();
        return {
            top: offset.top,
            left: offset.left
        };
    }
    return {
        top: 0,
        left: 0
    };
}
/**
 * Number generator used for TagToolbars
 */
var lastMenuId = 0;
function getMenuNumber() {
    return lastMenuId++;
}
/** The current mouseposition, always updated when the mouse changes */
var mousePosition = {
    x: 0,
    y: 0
};
/**
 * Keep the mouse-position update for future use
 */
$(window).on('mousemove', function (e) {
    mousePosition.x = e.clientX;
    mousePosition.y = e.clientY;
});
var TagToolbar = /** @class */ (function () {
    function TagToolbar(tag, cnt) {
        var _this = this;
        this.tag = tag;
        this.cnt = cnt;
        this.toolbarElement = null;
        this.initialized = false;
        this.updatePosition = this.updatePosition.bind(this);
        // Ensure toolbar gets visible when hovering
        tag.on('mouseenter', function () {
            _this.initialize();
            _this.showToolbar();
        });
        tag.on('mouseleave', function (e) {
            _this.initialize();
            // if we hover the menu itself now, don't hide it
            if (!$.contains(_this.toolbarElement[0], e.relatedTarget) && _this.toolbarElement[0] !== e.relatedTarget)
                _this.hideToolbar();
        });
    }
    TagToolbar.prototype.initialize = function () {
        var _this = this;
        if (this.initialized)
            return;
        var toolbarId = this.cnt.instance.id + "-" + this.cnt.contentBlock.id + "-" + getMenuNumber();
        // render toolbar and append tag to body
        this.toolbarElement = $(render_toolbar_1.renderToolbar(this.cnt));
        this.toolbarElement.on('mouseleave', function (e) {
            // if we do not hover the tag now, hide it
            if (!$.contains(_this.tag[0], e.relatedTarget) && _this.tag[0] !== e.relatedTarget)
                _this.hideToolbar();
        });
        $('body').append(this.toolbarElement);
        this.toolbarElement.attr(tagToolbarForAttr, toolbarId);
        this.tag.attr(tagToolbarAttr, toolbarId);
        this.toolbarElement.css({ display: 'none', position: 'absolute', transition: 'top 0.5s ease-out' });
        this.initialized = true;
    };
    TagToolbar.prototype.updatePosition = function () {
        var position = {
            top: 'auto',
            left: 'auto',
            right: 'auto',
            viewportOffset: this.tag[0].getBoundingClientRect().top,
            bodyOffset: getBodyOffset(),
            tagScrollOffset: 0,
            tagOffset: this.tag.offset(),
            tagWidth: this.tag.outerWidth(),
            mousePos: mousePosition,
            win: {
                scrollY: window.scrollY,
                width: $('body').width()
            },
            padding: tagToolbarPadding
        };
        // If we scrolled down, the toolbar might not be visible - calculate offset
        position.tagScrollOffset = Math.min(position.viewportOffset - position.bodyOffset.top, 0);
        // Update top coordinates
        if (position.tagScrollOffset === 0)
            position.top = position.tagOffset.top + tagToolbarPadding - position.bodyOffset.top;
        else
            position.top = position.mousePos.y + position.win.scrollY - position.bodyOffset.top - toolbarHeight / 2;
        // Update left / right coordinates
        // todo: try to change class to use attribute or something
        if (this.toolbarElement.hasClass('sc-tb-hover-right'))
            position.right = position.win.width - position.tagOffset.left - position.tagWidth + tagToolbarPaddingRight - position.bodyOffset.left;
        else
            position.left = position.tagOffset.left + tagToolbarPadding + position.bodyOffset.left;
        var cssPos = {
            top: position.top,
            left: position.left,
            right: position.right
        };
        this.toolbarElement.css(cssPos);
    };
    TagToolbar.prototype.hideToolbar = function () {
        $(window).off('scroll', this.updatePosition);
        this.toolbarElement.css({ display: 'none' });
    };
    TagToolbar.prototype.showToolbar = function () {
        /*if (this.toolbarElement.is(':visible'))
          return;*/
        this.toolbarElement.css({ display: 'block' });
        $(window).on('scroll', this.updatePosition);
        this.updatePosition();
    };
    return TagToolbar;
}());
exports.TagToolbar = TagToolbar;


/***/ }),
/* 37 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var config_1 = __webpack_require__(79);
var positioning_1 = __webpack_require__(38);
var quick_e_1 = __webpack_require__(2);
var selectors_instance_1 = __webpack_require__(8);
function enable() {
    // build all toolbar html-elements
    quick_e_1.prepareToolbarInDom();
    // Cache the panes (because panes can't change dynamically)
    initPanes();
}
/**
 * start watching for mouse-move
 */
function watchMouse() {
    var refreshTimeout = null;
    $('body').on('mousemove', function (e) {
        if (refreshTimeout === null)
            refreshTimeout = window.setTimeout(function () {
                requestAnimationFrame(function () {
                    positioning_1.refresh(e);
                    refreshTimeout = null;
                });
            }, 20);
    });
}
function start() {
    try {
        config_1._readPageConfig();
        if (quick_e_1.$quickE.config.enable) {
            // initialize first body-offset
            quick_e_1.$quickE.bodyOffset = positioning_1.getBodyPosition();
            enable();
            toggleParts();
            watchMouse();
        }
    }
    catch (e) {
        console.error("couldn't start quick-edit", e);
    }
}
exports.start = start;
/**
 * cache the panes which can contain modules
 */
function initPanes() {
    quick_e_1.$quickE.cachedPanes = $(selectors_instance_1.selectors.mod.listSelector);
    quick_e_1.$quickE.cachedPanes.addClass('sc-cb-pane-glow');
}
/**
 * enable/disable module/content-blocks as configured
 */
function toggleParts() {
    //// content blocks actions
    // quickE.cbActions.toggle(quickE.config.innerBlocks.enable);
    //// module actions
    // quickE.modActions.hide(quickE.config.modules.enable);
}
/**
 * reset the quick-edit
 * for example after ajax-loading a content-block, which may cause changed configurations
 */
function reset() {
    config_1._readPageConfig();
    toggleParts();
}
exports.reset = reset;


/***/ }),
/* 38 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var coords_1 = __webpack_require__(80);
var quick_e_1 = __webpack_require__(2);
var selectors_instance_1 = __webpack_require__(8);
/**
 * Module with everything related to positioning the quick-edit in-page editing
 */
/**
 * Point is used as return type to store X,Y coordinates
 */
/**
 * Prepare offset calculation based on body positioning
 * @returns Point
 */
function getBodyPosition() {
    var bodyPos = quick_e_1.$quickE.body.css('position');
    return bodyPos === 'relative' || bodyPos === 'absolute'
        ? new coords_1.Coords(quick_e_1.$quickE.body.offset().left, quick_e_1.$quickE.body.offset().top)
        : new coords_1.Coords(0, 0);
}
exports.getBodyPosition = getBodyPosition;
/**
 * Refresh content block and modules elements
 */
function refreshDomObjects() {
    quick_e_1.$quickE.bodyOffset =
        getBodyPosition(); // must update this, as sometimes after finishing page load the position changes, like when dnn adds the toolbar
    //// Cache the panes (because panes can't change dynamically)
    // if (!quickE.cachedPanes)
    //    quickE.cachedPanes = $(selectors.mod.listSelector);
    if (quick_e_1.$quickE.config.innerBlocks.enable) {
        // get all content-block lists which are empty, or which allow multiple child-items
        var lists = $(selectors_instance_1.selectors.cb.listSelector).filter(":not(." + selectors_instance_1.selectors.cb.singleItem + "), :empty");
        quick_e_1.$quickE.contentBlocks = lists // $(selectors.cb.listSelector)
            .find(selectors_instance_1.selectors.cb.selector)
            .add(lists); // selectors.cb.listSelector);
    }
    if (quick_e_1.$quickE.config.modules.enable)
        quick_e_1.$quickE.modules = quick_e_1.$quickE.cachedPanes
            .find(selectors_instance_1.selectors.mod.selector)
            .add(quick_e_1.$quickE.cachedPanes);
}
/**
 * Last time when contentblock and modules are refreshed.
 * Helps to skip unnecessary calls to refresh(e).
 */
(function (refreshDomObjects) {
})(refreshDomObjects || (refreshDomObjects = {}));
/**
 * position, align and show a menu linked to another item
 */
function positionAndAlign(element, coords) {
    return element.css({
        left: coords.x - quick_e_1.$quickE.bodyOffset.x,
        top: coords.yh - quick_e_1.$quickE.bodyOffset.y,
        width: coords.element.width(),
    }).show();
}
exports.positionAndAlign = positionAndAlign;
/**
 * Refresh positioning / visibility of the quick-insert bar
 * @param e
 */
function refresh(e) {
    var highlightClass = 'sc-cb-highlight-for-insert';
    var newDate = new Date();
    if ((!refreshDomObjects.lastCall) || (newDate.getTime() - refreshDomObjects.lastCall.getTime() > 1000)) {
        // console.log('refreshed contentblock and modules');
        refreshDomObjects.lastCall = newDate;
        refreshDomObjects();
    }
    if (quick_e_1.$quickE.config.innerBlocks.enable && quick_e_1.$quickE.contentBlocks) {
        quick_e_1.$quickE.nearestCb = findNearest(quick_e_1.$quickE.contentBlocks, new coords_1.Coords(e.clientX, e.clientY));
    }
    if (quick_e_1.$quickE.config.modules.enable && quick_e_1.$quickE.modules) {
        quick_e_1.$quickE.nearestMod = findNearest(quick_e_1.$quickE.modules, new coords_1.Coords(e.clientX, e.clientY));
    }
    quick_e_1.$quickE.modActions.toggleClass('sc-invisible', quick_e_1.$quickE.nearestMod === null);
    quick_e_1.$quickE.cbActions.toggleClass('sc-invisible', quick_e_1.$quickE.nearestCb === null);
    var oldParent = quick_e_1.$quickE.main.parentContainer;
    if (quick_e_1.$quickE.nearestCb !== null || quick_e_1.$quickE.nearestMod !== null) {
        var alignTo = quick_e_1.$quickE.nearestCb || quick_e_1.$quickE.nearestMod;
        // find parent pane to highlight
        var parentPane = $(alignTo.element).closest(selectors_instance_1.selectors.mod.listSelector);
        var parentCbList = $(alignTo.element).closest(selectors_instance_1.selectors.cb.listSelector);
        var parentContainer = (parentCbList.length ? parentCbList : parentPane)[0];
        // put part of the pane-name into the button-labels
        if (parentPane.length > 0) {
            var paneName_1 = parentPane.attr('id') || '';
            if (paneName_1.length > 4)
                paneName_1 = paneName_1.substr(4);
            quick_e_1.$quickE.modActions.filter('[titleTemplate]').each(function () {
                var t = $(this);
                t.attr('title', t.attr('titleTemplate').replace('{0}', paneName_1));
            });
        }
        positionAndAlign(quick_e_1.$quickE.main, alignTo);
        // Keep current block as current on menu
        quick_e_1.$quickE.main.actionsForCb = quick_e_1.$quickE.nearestCb ? quick_e_1.$quickE.nearestCb.element : null;
        quick_e_1.$quickE.main.actionsForModule = quick_e_1.$quickE.nearestMod ? quick_e_1.$quickE.nearestMod.element : null;
        quick_e_1.$quickE.main.parentContainer = parentContainer;
        $(parentContainer).addClass(highlightClass);
    }
    else {
        quick_e_1.$quickE.main.parentContainer = null;
        quick_e_1.$quickE.main.hide();
    }
    // if previously a parent-pane was highlighted, un-highlight it now
    if (oldParent && oldParent !== quick_e_1.$quickE.main.parentContainer)
        $(oldParent).removeClass(highlightClass);
}
exports.refresh = refresh;
/**
 * Return the nearest element to the mouse cursor from elements (jQuery elements)
 * @param elements
 * @param position
 */
function findNearest(elements, position) {
    var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed
    var nearestItem = null;
    var nearestDistance = maxDistance;
    var posX = position.x + quick_e_1.$quickE.win.scrollLeft();
    var posY = position.y + quick_e_1.$quickE.win.scrollTop();
    // Find nearest element
    elements.each(function () {
        var e = getCoordinates($(this));
        // First check x coordinates - must be within container
        if (posX < e.x || posX > e.x + e.w)
            return;
        // Check if y coordinates are within boundaries
        var distance = Math.abs(posY - e.yh);
        if (distance < maxDistance && distance < nearestDistance) {
            nearestItem = e;
            nearestDistance = distance;
        }
    });
    return nearestItem;
}
exports.findNearest = findNearest;
function getCoordinates(element) {
    // sometimes element.length === 0 and element.offset() = undefined
    // console.log("element.offset():", element.offset());
    // console.log("element.length:", element.length);
    var coords = {
        element: element,
        x: element.offset().left,
        w: element.width(),
        y: element.offset().top,
        // For content-block ITEMS, the menu must be visible at the end
        // For content-block-LISTS, the menu must be at top
        yh: element.offset().top + (element.is(selectors_instance_1.selectors.eitherCbOrMod) ? element.height() : 0),
    };
    return coords;
}
exports.getCoordinates = getCoordinates;


/***/ }),
/* 39 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/*
 * this is a content block in the browser
 *
 * A Content Block is a stand alone unit of content, with it's own definition of
 * 1. content items
 * 2. template
 * + some other stuff
 *
 * it should be able to render itself
 */
//#region functions working only with what they are given
// 2017-08-27 2dm: I'm working on cleaning up this code, and an important part
// is to have code which doesn't use old state (like object-properties initialized earlier)
// extracting these methods is part of the work
/**
 * TODO - unclear if still in use
 * @param {object} sxc
 * @param {boolean} state
 * @returns {promise}
 */
// 2017-09-02 2dm removed, deprecated, it's not stored on the server any more
// cbm.setTemplateChooserState = function(sxc, state) {
//    return sxc.webApi.get({
//        url: "view/module/SetTemplateChooserState",
//        params: { state: state }
//    });
// };
/**
 * Save the template configuration for this instance
 * @param {ContextOfButton} context
 * @param {number} templateId
 * @param {boolean} [forceCreateContentGroup]
 * @returns {promise}
 */
function saveTemplate(context, templateId, forceCreateContentGroup) {
    var params = {
        templateId: templateId,
        forceCreateContentGroup: forceCreateContentGroup,
        newTemplateChooserState: false,
    };
    return new Promise(function (resolve, reject) {
        context.sxc.webApi.get({
            url: 'view/module/savetemplateid',
            params: params,
        }).done(function (data, textStatus, jqXhr) {
            // resolve or reject based on http-status: 200 & 204 = ok
            if (jqXhr.status === 204 || jqXhr.status === 200)
                resolve(data);
            else
                reject(Error(textStatus));
        }).fail(function (jqXhr, textStatus, errorThrown) {
            reject(Error(errorThrown));
        });
    });
}
exports.saveTemplate = saveTemplate;
/**
 * Retrieve the preview from the web-api
 * @param {ContextOfButton} context
 * @param {number} templateId
 * @returns {promise} promise with the html in the result
 */
function getPreviewWithTemplate(context, templateId) {
    templateId = templateId || -1; // fallback, meaning use saved ID
    var params = {
        templateId: templateId,
        lang: context.app.currentLanguage,
        cbisentity: context.contentBlock.isEntity,
        cbid: context.contentBlock.id,
        originalparameters: JSON.stringify(context.instance.parameters),
    };
    return new Promise(function (resolve, reject) {
        context.sxc.webApi.get({
            url: 'view/module/rendertemplate',
            params: params,
            dataType: 'html',
        }).done(function (data, textStatus, jqXhr) {
            if (jqXhr.status === 204 || jqXhr.status === 200) {
                // resolve the promise with the response text
                resolve(data);
            }
            else {
                // otherwise reject with the status text
                // which will hopefully be a meaningful error
                reject(Error(textStatus));
            }
        }).fail(function (jqXhr, textStatus, errorThrown) {
            reject(Error(errorThrown));
        });
    });
}
exports.getPreviewWithTemplate = getPreviewWithTemplate;
//#endregion


/***/ }),
/* 40 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var render_1 = __webpack_require__(16);
var sxc_controller_in_page_1 = __webpack_require__(3);
var window_in_page_1 = __webpack_require__(1);
var quick_dialog_1 = __webpack_require__(22);
var DialogPaths_1 = __webpack_require__(24);
var command_link_to_ng_dialog_1 = __webpack_require__(86);
/**
 * open a new dialog of the angular-ui
 * @param settings
 * @param event
 * @param sxc
 * @param editContext
 */
function commandOpenNgDialog(context, event) {
    // the link contains everything to open a full dialog (lots of params added)
    var link = command_link_to_ng_dialog_1.commandLinkToNgDialog(context);
    var fullScreen = false;
    var origEvent = event || window_in_page_1.windowInPage.event;
    return new Promise(function (resolvePromise) {
        // prepare promise for callback when the dialog closes
        // to reload the in-page view w/ajax or page reload
        var resolveAndReInit = function () {
            resolvePromise(context);
            render_1.renderer.reloadAndReInitialize(context);
        };
        // check if inline window (quick-dialog)
        if (context.button.inlineWindow) {
            // test if it should be full screen (value or resolve-function)
            if (typeof context.button.fullScreen === 'function')
                fullScreen = context.button.fullScreen(context);
            var diagName = context.button.dialog(context).toString();
            quick_dialog_1.quickDialog
                .showOrToggleFromToolbar(context, link, fullScreen, diagName)
                .then(function (isChanged) {
                if (isChanged)
                    resolveAndReInit();
            });
            // else it's a normal pop-up dialog
        }
        else {
            // check if alt-key pressed, to open the old/new dialog instead
            if (origEvent && origEvent.altKey) {
                var toOld = link.indexOf(DialogPaths_1.DialogPaths.ng8) > 0;
                link = link.replace(toOld ? DialogPaths_1.DialogPaths.ng8 : DialogPaths_1.DialogPaths.ng1, toOld ? DialogPaths_1.DialogPaths.ng1 : DialogPaths_1.DialogPaths.ng8);
            }
            // check if new-window
            if (context.button.newWindow || (origEvent && origEvent.shiftKey)) {
                resolvePromise(context);
                window_in_page_1.windowInPage.open(link);
            }
            else {
                sxc_controller_in_page_1.$2sxcInPage.totalPopup.open(link, resolveAndReInit);
            }
        }
    });
}
exports.commandOpenNgDialog = commandOpenNgDialog;


/***/ }),
/* 41 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Iframebridge = __webpack_require__(82);
var ContainerSize = __webpack_require__(42);
/**
 * this is a dialog manager which is in charge of all quick-dialogues
 * it always has a reference to the latest dialog created by any module instance
 */
var containerClass = 'inpage-frame-wrapper';
var iframeClass = 'inpage-frame';
var iframeTag = 'iframe';
var containerTemplate = "<div class=\"" + containerClass + "\"><div class=\"" + iframeClass + "\"></div></div>";
/**
 * get the current container
 * @returns {element} html element of the div
 */
function getOrCreate() {
    var container = $("." + containerClass);
    return container.length > 0 ? container : buildContainerAndIFrame();
}
exports.getOrCreate = getOrCreate;
/**
 * find the iframe which hosts the dialog
 * @param {html} [container] - html-container as jQuery object
 * @returns {html} iframe object
 */
function getIFrame(container) {
    if (!container)
        container = getOrCreate();
    return container.find(iframeTag)[0];
}
exports.getIFrame = getIFrame;
/**
 * build the container in the dom w/iframe for re-use
 * @return {jquery} jquery dom-object
 */
function buildContainerAndIFrame() {
    var container = $(containerTemplate);
    if ($("#personaBar-iframe").length > 0)
        container.addClass("persona-bar-visible");
    var newIFrame = document.createElement(iframeTag);
    var extendedIFrame = Iframebridge.build(newIFrame);
    container.find("." + iframeClass).append(extendedIFrame);
    $('body').append(container);
    ContainerSize.watchForResize(container);
    return container;
}


/***/ }),
/* 42 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Container = __webpack_require__(41);
/**
 * this is a dialog manager which is in charge of all quick-dialogues
 * it always has a reference to the latest dialog created by any module instance
 */
var isFullscreen = false;
/**
 * set container css for size
 * @param {boolean} fullScreen
 */
function setSize(fullScreen) {
    var container = Container.getOrCreate();
    // set container height
    container.css('min-height', fullScreen ? '100%' : '225px');
    isFullscreen = fullScreen;
}
exports.setSize = setSize;
var resizeInterval = 200;
var resizeWatcher = null;
/**
 * create watcher which monitors the iframe size and adjusts the container as needed
 */
function watchForResize(container) {
    if (!resizeWatcher) // only add a timer if not already running
        resizeWatcher = window.setInterval(function () {
            try {
                var frm = Container.getIFrame(container);
                if (!frm)
                    return;
                var height = frm.contentDocument.body.offsetHeight;
                if (frm.previousHeight === height)
                    return;
                frm.style.minHeight = container.css('min-height');
                frm.style.height = height + 'px';
                frm.previousHeight = height;
                if (isFullscreen) {
                    frm.style.height = '100%';
                    frm.style.position = 'absolute';
                }
            }
            catch (e) {
                // ignore
            }
        }, resizeInterval);
}
exports.watchForResize = watchForResize;


/***/ }),
/* 43 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var session_state_handler_1 = __webpack_require__(84);
exports.cbId = new session_state_handler_1.SessionStateHandler('dia-cbid');
exports.cancelled = new session_state_handler_1.SessionStateHandler('cancelled-dialog');


/***/ }),
/* 44 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var sxc_1 = __webpack_require__(6);
/**
 * extend the quick edit with the core commands
 */
var Cb = /** @class */ (function () {
    function Cb() {
    }
    Cb.prototype.delete = function (clip) {
        var sxc = sxc_1.getSxcInstance(clip.list);
        return sxc.manage._getCbManipulator().delete(clip.parent, clip.field, clip.index);
    };
    Cb.create = function (parent, field, index, appOrContent, list, newGuid) {
        var sxc = sxc_1.getSxcInstance(list);
        return sxc.manage._getCbManipulator().create(parent, field, index, appOrContent, list, newGuid);
    };
    return Cb;
}());
exports.Cb = Cb;


/***/ }),
/* 45 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var mod_manage_1 = __webpack_require__(46);
var quick_e_1 = __webpack_require__(2);
var selectors_instance_1 = __webpack_require__(8);
var Mod = /** @class */ (function () {
    function Mod() {
    }
    Mod.prototype.delete = function (clip) {
        if (!confirm('are you sure?'))
            return;
        var modId = mod_manage_1.modManage.getModuleId(clip.item.className);
        mod_manage_1.modManage.delete(modId);
    };
    // todo: unsure if this is a good place for this bit of code...
    Mod.move = function (oldClip, newClip, from, to) {
        var modId = mod_manage_1.modManage.getModuleId(oldClip.item.className);
        var pane = mod_manage_1.modManage.getPaneName(newClip.list);
        mod_manage_1.modManage.move(modId, pane, to);
    };
    Mod.sendToPane = function () {
        var pane = quick_e_1.$quickE.main.actionsForModule.closest(selectors_instance_1.selectors.mod.listSelector);
        // show the pane-options
        var pl = quick_e_1.$quickE.selected.find('#paneList');
        // ReSharper disable once CssBrowserCompatibility
        if (!pl.is(':empty'))
            pl.empty();
        pl.append(mod_manage_1.modManage.getMoveButtons(mod_manage_1.modManage.getPaneName(pane)));
    };
    return Mod;
}());
exports.Mod = Mod;


/***/ }),
/* 46 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var clipboard_1 = __webpack_require__(25);
var quick_e_1 = __webpack_require__(2);
/**
 * module specific stuff
 */
var ModManage = /** @class */ (function () {
    function ModManage() {
        this.delete = deleteMod;
        this.create = createModWithTypeName;
        this.move = moveMod;
        this.getPaneName = getPaneName;
        this.getModuleId = getModuleId;
        this.getMoveButtons = generatePaneMoveButtons;
    }
    return ModManage;
}());
exports.ModManage = ModManage;
exports.modManage = new ModManage();
function getPaneName(pane) {
    return $(pane).attr('id').replace('dnn_', '');
}
// find the correct module id from a list of classes - used on the module-wrapper
function getModuleId(classes) {
    var result = classes.match(/DnnModule-([0-9]+)(?:\W|$)/);
    return (result && result.length === 2) ? Number(result[1]) : null;
}
// show an error when an xhr error occurs
function xhrError(xhr, optionalMessage) {
    alert(optionalMessage || 'Error while talking to server.');
    console.log(xhr);
}
// service calls we'll need
function createModWithTypeName(paneName, index, type) {
    return sendDnnAjax(null, 'controlbar/GetPortalDesktopModules', {
        data: 'category=All&loadingStartIndex=0&loadingPageSize=100&searchTerm=',
        success: function (desktopModules) {
            var moduleToFind = type === 'Default' ? ' Content' : ' App';
            var module = null;
            // ReSharper disable once UnusedParameter
            desktopModules.forEach(function (e, i) {
                if (e.ModuleName === moduleToFind)
                    module = e;
            });
            return (!module)
                ? alert(moduleToFind + ' module not found.')
                : createMod(paneName, index, module.ModuleID);
        },
    });
}
// move a dnn module
function moveMod(modId, pane, order) {
    var service = $.dnnSF(modId);
    var tabId = service.getTabId();
    var dataVar = {
        TabId: tabId,
        ModuleId: modId,
        Pane: pane,
        ModuleOrder: (2 * order + 0),
    };
    sendDnnAjax(modId, 'ModuleService/MoveModule', {
        type: 'POST',
        data: dataVar,
        success: function () { return window.location.reload(); },
    });
    // fire window resize to reposition action menus
    $(window).resize();
}
// delete a module
function deleteMod(modId) {
    var service = $.dnnSF(modId);
    var tabId = service.getTabId();
    return sendDnnAjax(modId, '2sxc/dnn/module/delete', {
        url: $.dnnSF().getServiceRoot('2sxc') + 'dnn/module/delete',
        type: 'GET',
        data: {
            tabId: tabId,
            modId: modId,
        },
        // ReSharper disable once UnusedParameter
        success: function (d) { return window.location.reload(); },
    });
}
// call an api on dnn
function sendDnnAjax(modId, serviceName, options) {
    var service = $.dnnSF(modId);
    return $.ajax($.extend({
        type: 'GET',
        url: service.getServiceRoot('internalservices') + serviceName,
        beforeSend: service.setModuleHeaders,
        error: xhrError,
    }, options));
}
// create / insert a new module
function createMod(paneName, position, modId) {
    var postData = {
        Module: modId,
        Page: '',
        Pane: paneName,
        Position: -1,
        Sort: position,
        Visibility: 0,
        AddExistingModule: false,
        CopyModule: false,
    };
    return sendDnnAjax(null, 'controlbar/AddModule', {
        type: 'POST',
        data: postData,
        // ReSharper disable once UnusedParameter
        success: function (d) { return window.location.reload(); },
    });
}
function generatePaneMoveButtons(current) {
    var pns = quick_e_1.$quickE.cachedPanes;
    // generate list of panes as links
    var targets = $('<div>');
    for (var p = 0; p < pns.length; p++) {
        var pName = getPaneName(pns[p]);
        var selected = (current === pName) ? ' selected ' : '';
        if (selected === '')
            targets.append("<a data='" + pName + "'>" + pName + "</a>");
    }
    // attach click event...
    // ReSharper disable once UnusedParameter
    targets.find('a').click(function (d) {
        var link = $(this);
        var clip = clipboard_1.data;
        var modId = getModuleId(clip.item.className);
        var newPane = link.attr('data');
        moveMod(modId, newPane, 0);
    });
    return targets;
}


/***/ }),
/* 47 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
// polyfills
__webpack_require__(48); // fix for IE11 Array.find
__webpack_require__(49); // fix for IE11 Object.assign
__webpack_require__(108); // polyfill of the ES6 Promise
__webpack_require__(50); // fix for IE11 Element.closest
__webpack_require__(51); // fix for IE11 window.CustomEvent


/***/ }),
/* 48 */
/***/ (function(module, exports) {

// https://tc39.github.io/ecma262/#sec-array.prototype.find
// https://stackoverflow.com/questions/31455805/find-object-in-array-using-typescript
if (!Array.prototype.find) {
    Object.defineProperty(Array.prototype, 'find', {
        value: function (predicate) {
            // 1. Let O be ? ToObject(this value).
            if (this == null) { // jshint ignore:line
                throw new TypeError('"this" is null or not defined');
            }
            var o = Object(this);
            // 2. Let len be ? ToLength(? Get(O, "length")).
            var len = o.length >>> 0;
            // 3. If IsCallable(predicate) is false, throw a TypeError exception.
            if (typeof predicate !== 'function') {
                throw new TypeError('predicate must be a function');
            }
            // 4. If thisArg was supplied, let T be thisArg; else let T be undefined.
            var thisArg = arguments[1];
            // 5. Let k be 0.
            var k = 0;
            // 6. Repeat, while k < len
            while (k < len) {
                // a. Let Pk be ! ToString(k).
                // b. Let kValue be ? Get(O, Pk).
                // c. Let testResult be ToBoolean(? Call(predicate, T, « kValue, k, O »)).
                // d. If testResult is true, return kValue.
                var kValue = o[k];
                if (predicate.call(thisArg, kValue, k, o)) {
                    return kValue;
                }
                // e. Increase k by 1.
                k++;
            }
            // 7. Return undefined.
            return undefined;
        }
    });
}


/***/ }),
/* 49 */
/***/ (function(module, exports) {

if (typeof Object.assign != 'function') {
    // ReSharper disable once UnusedParameter
    Object.assign = function (target, varArgs) {
        'use strict';
        if (target === null) { // TypeError if undefined or null
            throw new TypeError('Cannot convert undefined or null to object');
        }
        var to = Object(target);
        for (var index = 1; index < arguments.length; index++) {
            var nextSource = arguments[index];
            if (nextSource !== null) { // Skip over if undefined or null
                for (var nextKey in nextSource) {
                    // Avoid bugs when hasOwnProperty is shadowed
                    if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
                        to[nextKey] = nextSource[nextKey];
                    }
                }
            }
        }
        return to;
    };
}


/***/ }),
/* 50 */
/***/ (function(module, exports) {

// https://developer.mozilla.org/en-US/docs/Web/API/Element/closest
if (!Element.prototype.closest) {
    Element.prototype.closest =
        function (s) {
            var matches = (this.document || this.ownerDocument).querySelectorAll(s);
            var i;
            var el = this;
            do {
                i = matches.length;
                while (--i >= 0 && matches.item(i) !== el) { }
                ;
            } while ((i < 0) && (el = el.parentElement));
            return el;
        };
}


/***/ }),
/* 51 */
/***/ (function(module, exports) {

// Polyfill for creating CustomEvents on IE9/10/11
// https://raw.githubusercontent.com/krambuhl/custom-event-polyfill/master/custom-event-polyfill.js
try {
    var ce = new window.CustomEvent('test');
    ce.preventDefault();
    if (ce.defaultPrevented !== true) {
        // IE has problems with .preventDefault() on custom events
        // http://stackoverflow.com/questions/23349191
        throw new Error('Could not prevent default');
    }
}
catch (e) {
    var CustomEvent = function (event, params) {
        params = params || {
            bubbles: false,
            cancelable: false,
            detail: undefined
        };
        var evt = document.createEvent('CustomEvent');
        evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
        var origPrevent = evt.preventDefault;
        evt.preventDefault = function () {
            origPrevent.call(this);
            try {
                Object.defineProperty(this, 'defaultPrevented', {
                    get: function () { return true; }
                });
            }
            catch (e) {
                this.defaultPrevented = true;
            }
        };
        return evt;
    };
    CustomEvent.prototype = window.Event.prototype;
    window.CustomEvent = CustomEvent; // expose definition to window
}


/***/ }),
/* 52 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var templates_1 = __webpack_require__(18);
var has_log_1 = __webpack_require__(15);
var settings_adapter_1 = __webpack_require__(31);
var button_action_1 = __webpack_require__(20);
var button_config_1 = __webpack_require__(21);
var command_open_ng_dialog_1 = __webpack_require__(40);
var commands_1 = __webpack_require__(10);
var Engine = /** @class */ (function (_super) {
    __extends(Engine, _super);
    function Engine(parentLog) {
        return _super.call(this, 'Cmd.Exec', parentLog) || this;
    }
    Engine.prototype.detectParamsAndRun = function (context, nameOrSettings, eventOrSettings, event) {
        this.log.add("detecting params and running - has " + arguments.length + " params");
        var settings;
        var thirdParamIsEvent = !event &&
            eventOrSettings &&
            typeof eventOrSettings.altKey !== 'undefined';
        this.log.add("might cycle parameters, in case not all were given. third is event=" + thirdParamIsEvent);
        if (thirdParamIsEvent) {
            // no event param, but settings contains the event-object
            this.log.add('cycling parameters as event was missing & eventOrSettings seems to be an event; settings must be empty');
            event = eventOrSettings; // move it to the correct variable
            settings = this.nameOrSettingsAdapter(nameOrSettings);
        }
        else {
            settings = Object.assign(eventOrSettings || {}, this.nameOrSettingsAdapter(nameOrSettings));
        }
        // ensure we have the right event despite browser differences
        event = event || window.event;
        return this.run(context, settings, event);
    };
    /**
     * run a command
     * this method expects a clear order of parameters
     * @param context
     * @param settings
     * @param event
     */
    Engine.prototype.run = function (context, nameOrSettings, event) {
        // | any is temporary, just to get it to work; should be improved to only give a promise
        var settings = this.nameOrSettingsAdapter(nameOrSettings);
        settings = this.expandSettingsWithDefaults(settings);
        var origEvent = event;
        var name = settings.action;
        var contentType = settings.contentType;
        this.log.add("run command " + name + " for type " + contentType);
        // Toolbar API v2
        var newButtonAction = new button_action_1.ButtonAction(name, contentType, settings);
        newButtonAction.commandDefinition = commands_1.Commands.getInstance().get(name);
        var newButtonConfig = new button_config_1.ButtonConfig(newButtonAction);
        newButtonConfig.name = name;
        var button = (context.button = Object.assign(newButtonConfig, newButtonAction.commandDefinition.buttonConfig, settings_adapter_1.settingsAdapter(settings))); // merge conf & settings, but settings has higher priority
        // todo: stv, fix this in case that is function
        if (!button.dialog) {
            this.log.add('button.dialog method missing, must be old implementation which used the action-name - generating method');
            button.dialog = function () {
                return name;
            };
        }
        // todo: stv, fix this in case that is function
        if (!button.code) {
            this.log.add('simple button without code - generating code to open standard dialog');
            button.code = function (contextParam, evt) {
                return command_open_ng_dialog_1.commandOpenNgDialog(contextParam, evt);
            };
        }
        if (button.uiActionOnly(context)) {
            this.log.add('just a UI command, will not run pre-flight to ensure content-block - now running the code');
            return button.code(context, origEvent);
        }
        // if more than just a UI-action, then it needs to be sure the content-group is created first
        this.log.add('command might change data, will wrap in pre-flight to ensure content-block');
        return templates_1.prepareToAddContent(context, settings.useModuleList).then(function () {
            return context.button.code(context, origEvent);
        });
    };
    /**
     * name or settings adapter to settings
     * @param nameOrSettings
     * @returns settings
     */
    Engine.prototype.nameOrSettingsAdapter = function (nameOrSettings) {
        var settings;
        // check if nameOrString is name (string) or object (settings)
        var nameIsString = typeof nameOrSettings === 'string';
        this.log.add("adapting settings; name is string: " + nameIsString + "; name = " + nameOrSettings);
        if (nameIsString) {
            settings = Object.assign({}, { action: nameOrSettings }); // place the name as an action-name into a command-object
        }
        else {
            settings = nameOrSettings;
        }
        return settings;
    };
    /**
     * Take a settings-name or partial settings object,
     * and return a full settings object with all defaults from
     * the command definition
     * @param settings
     */
    Engine.prototype.expandSettingsWithDefaults = function (settings) {
        var name = settings.action;
        this.log.add("will add defaults for " + name + " from buttonConfig");
        var conf = commands_1.Commands.getInstance().get(name).buttonConfig;
        var full = Object.assign({}, conf, settings); // merge conf & settings, but settings has higher priority
        return full;
    };
    return Engine;
}(has_log_1.HasLog));
exports.Engine = Engine;


/***/ }),
/* 53 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * this will be everything about the current system, like system / api -paths etc.
 */
var SystemContext = /** @class */ (function () {
    function SystemContext() {
    }
    return SystemContext;
}());
exports.SystemContext = SystemContext;


/***/ }),
/* 54 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * this will be something about the current tenant(the dnn portal)
 */
var TenantContext = /** @class */ (function () {
    function TenantContext() {
    }
    return TenantContext;
}());
exports.TenantContext = TenantContext;


/***/ }),
/* 55 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * things about the user
 */
var UserContext = /** @class */ (function () {
    function UserContext() {
    }
    return UserContext;
}());
exports.UserContext = UserContext;


/***/ }),
/* 56 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * information related to the current contentBlock, incl
 */
var ContentBlockContext = /** @class */ (function () {
    function ContentBlockContext() {
    }
    return ContentBlockContext;
}());
exports.ContentBlockContext = ContentBlockContext;


/***/ }),
/* 57 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var context_of_toolbar_1 = __webpack_require__(58);
var ContextOfButton = /** @class */ (function (_super) {
    __extends(ContextOfButton, _super);
    function ContextOfButton() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContextOfButton;
}(context_of_toolbar_1.ContextOfToolbar));
exports.ContextOfButton = ContextOfButton;
function isContextOfButton(thing) {
    var maybeButton = thing;
    return maybeButton.button !== undefined && maybeButton.tenant !== undefined;
}
exports.isContextOfButton = isContextOfButton;


/***/ }),
/* 58 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var context_of_item_1 = __webpack_require__(59);
var ContextOfToolbar = /** @class */ (function (_super) {
    __extends(ContextOfToolbar, _super);
    function ContextOfToolbar() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContextOfToolbar;
}(context_of_item_1.ContextOfItem));
exports.ContextOfToolbar = ContextOfToolbar;


/***/ }),
/* 59 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var context_of_content_block_1 = __webpack_require__(60);
var ContextOfItem = /** @class */ (function (_super) {
    __extends(ContextOfItem, _super);
    function ContextOfItem() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContextOfItem;
}(context_of_content_block_1.ContextOfContentBlock));
exports.ContextOfItem = ContextOfItem;


/***/ }),
/* 60 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var context_of_instance_1 = __webpack_require__(27);
var ContextOfContentBlock = /** @class */ (function (_super) {
    __extends(ContextOfContentBlock, _super);
    function ContextOfContentBlock() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContextOfContentBlock;
}(context_of_instance_1.ContextOfInstance));
exports.ContextOfContentBlock = ContextOfContentBlock;


/***/ }),
/* 61 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var context_of_1 = __webpack_require__(62);
var ContextOfPage = /** @class */ (function (_super) {
    __extends(ContextOfPage, _super);
    function ContextOfPage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContextOfPage;
}(context_of_1.ContextOf));
exports.ContextOfPage = ContextOfPage;


/***/ }),
/* 62 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var base_context_1 = __webpack_require__(63);
var ContextOf = /** @class */ (function (_super) {
    __extends(ContextOf, _super);
    function ContextOf() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ContextOf;
}(base_context_1.BaseContext));
exports.ContextOf = ContextOf;


/***/ }),
/* 63 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var BaseContext = /** @class */ (function () {
    function BaseContext() {
        // tbd
        // ReSharper disable once InconsistentNaming
        this._isContext = true;
    }
    return BaseContext;
}());
exports.BaseContext = BaseContext;
function isContext(thing) {
    var maybeButton = thing;
    return maybeButton._isContext !== undefined;
}
exports.isContext = isContext;


/***/ }),
/* 64 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * this will be about the current app, settings of the app, app - paths, etc.
 */
var AppContext = /** @class */ (function () {
    function AppContext() {
    }
    return AppContext;
}());
exports.AppContext = AppContext;


/***/ }),
/* 65 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * information related to the current DNN module, incl.instanceId,
 */
var InstanceContext = /** @class */ (function () {
    function InstanceContext() {
    }
    return InstanceContext;
}());
exports.InstanceContext = InstanceContext;


/***/ }),
/* 66 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * information about the current item
 */
var ItemContext = /** @class */ (function () {
    function ItemContext() {
    }
    return ItemContext;
}());
exports.ItemContext = ItemContext;


/***/ }),
/* 67 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * this will be information related to the current page
 */
var PageContext = /** @class */ (function () {
    function PageContext() {
    }
    return PageContext;
}());
exports.PageContext = PageContext;


/***/ }),
/* 68 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function isSxcInstance(thing) {
    return thing.showDetailedHttpError !== undefined;
}
exports.isSxcInstance = isSxcInstance;


/***/ }),
/* 69 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * ensure that the UI will load the correct assets to enable editing
 */
var UiContext = /** @class */ (function () {
    function UiContext() {
    }
    return UiContext;
}());
exports.UiContext = UiContext;


/***/ }),
/* 70 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var render_button_1 = __webpack_require__(19);
/**
 * render groups of buttons in toolbar
 * @param sxc
 * @param toolbarConfig
 */
function renderGroups(context) {
    var groupsBuffer = []; // temporary storage for detached HTML DOM objects
    var btnGroups = context.toolbar.groups;
    for (var i = 0; i < btnGroups.length; i++) {
        var btns = btnGroups[i].buttons;
        for (var h = 0; h < btns.length; h++) {
            context.button = btns[h];
            // create one button
            var button = render_button_1.renderButton(context, i);
            // add button to group of buttons
            var item = document.createElement('li');
            item.appendChild(button);
            groupsBuffer.push(item);
        }
    }
    return groupsBuffer;
}
exports.renderGroups = renderGroups;


/***/ }),
/* 71 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function oldParametersAdapter(action) {
    var params = {};
    if (action) {
        if (action.name) {
            params.action = action.name;
        }
        if (action.params) {
            Object.assign(params, action.params);
        }
    }
    return params;
}
exports.oldParametersAdapter = oldParametersAdapter;


/***/ }),
/* 72 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Entry = /** @class */ (function () {
    function Entry(log, message) {
        var _this = this;
        this.log = log;
        this.message = message;
        this.source = function () { return _this.log.fullIdentifier(); };
    }
    return Entry;
}());
exports.Entry = Entry;


/***/ }),
/* 73 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * used to build instance config
 */
var InstanceConfig = /** @class */ (function () {
    function InstanceConfig() {
    }
    //constructor(editContext: DataEditContext) {
    //  const ce = editContext.Environment;
    //  const cg = editContext.ContentGroup;
    //  const cb = editContext.ContentBlock;
    //  this.portalId = ce.WebsiteId;
    //  this.tabId = ce.PageId;
    //  this.moduleId = ce.InstanceId;
    //  this.version = ce.SxcVersion;
    //  this.contentGroupId = cg.Guid;
    //  this.cbIsEntity = cb.IsEntity;
    //  this.cbId = cb.Id;
    //  this.appPath = cg.AppUrl;
    //  this.isList = cg.IsList;
    //}
    InstanceConfig.fromContext = function (contextOfButton) {
        var config = new InstanceConfig();
        config.portalId = contextOfButton.tenant.id;
        config.tabId = contextOfButton.page.id;
        config.moduleId = contextOfButton.instance.id;
        config.version = contextOfButton.instance.sxcVersion;
        config.contentGroupId = contextOfButton.contentBlock.contentGroupId;
        config.cbIsEntity = contextOfButton.contentBlock.isEntity;
        config.cbId = contextOfButton.contentBlock.id;
        config.appPath = contextOfButton.app.appPath;
        config.isList = contextOfButton.contentBlock.isList;
        return config;
    };
    return InstanceConfig;
}());
exports.InstanceConfig = InstanceConfig;


/***/ }),
/* 74 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * removes autoAddMore and classes if are null or empty, to keep same behaviour like in v1
 * @param toolbarSettings
 */
function oldToolbarSettingsAddapter(toolbarSettings) {
    var partialToolbaSettings = Object.assign({}, toolbarSettings);
    if (!partialToolbaSettings.autoAddMore) {
        delete partialToolbaSettings.autoAddMore;
    }
    if (!partialToolbaSettings.classes) {
        delete partialToolbaSettings.classes;
    }
    return partialToolbaSettings;
}
exports.oldToolbarSettingsAddapter = oldToolbarSettingsAddapter;


/***/ }),
/* 75 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var commands_1 = __webpack_require__(10);
var parameters_adapter_1 = __webpack_require__(30);
var settings_adapter_1 = __webpack_require__(31);
var button_action_1 = __webpack_require__(20);
var button_config_1 = __webpack_require__(21);
var expand_button_config_1 = __webpack_require__(14);
var log_1 = __webpack_require__(7);
var flatten_action_definition_1 = __webpack_require__(32);
/**
 * this will traverse a groups-tree and expand each group
 * so if groups were just strings like "edit,new" or compact buttons, they will be expanded afterwards
 * @param fullToolbarConfig
 */
function expandButtonGroups(fullToolbarConfig, parentLog) {
    var log = new log_1.Log('Tlb.ExpGrp', parentLog, 'start');
    var actions = commands_1.Commands.getInstance();
    // by now we should have a structure, let's check/fix the buttons
    log.add("will expand groups - found " + fullToolbarConfig.groups.length + " items");
    for (var g = 0; g < fullToolbarConfig.groups.length; g++) {
        // expand a verb-list like "edit,new" into objects like [{ action: "edit" }, {action: "new"}]
        expandButtonList(fullToolbarConfig.groups[g], fullToolbarConfig.settings, log);
        // fix all the buttons
        var btns = fullToolbarConfig.groups[g].buttons;
        var buttonConfigs = [];
        if (Array.isArray(btns)) {
            log.add("will process " + btns.length + " buttons");
            for (var b = 0; b < btns.length; b++) {
                var btn = btns[b];
                if (!(actions.get(btn.command.action))) {
                    log.add("couldn't find action " + btn.command.action + " - show warning");
                    console.warn('warning: toolbar-button with unknown action-name:', btn.command.action);
                }
                var name = btn.command.action;
                var contentType = btn.command.contentType;
                // if the button belongs to a content-item, move the specs up to the item into the settings-object
                flatten_action_definition_1.flattenActionDefinition(btn.command);
                // parameters adapter from v1 to v2
                var params = parameters_adapter_1.parametersAdapter(btn.command);
                Object.assign(params, fullToolbarConfig.params);
                // Toolbar API v2
                var newButtonAction = new button_action_1.ButtonAction(name, contentType, params);
                newButtonAction.commandDefinition = actions.get(name);
                var newButtonConfig = new button_config_1.ButtonConfig(newButtonAction);
                newButtonConfig.name = name;
                // settings adapter from v1 to v2
                var settings = settings_adapter_1.settingsAdapter(btn);
                Object.assign(newButtonConfig, settings);
                expand_button_config_1.addDefaultBtnSettings(newButtonConfig, fullToolbarConfig.groups[g], fullToolbarConfig, actions, log); // ensure all buttons have either own settings, or the fallback
                buttonConfigs.push(newButtonConfig);
            }
        }
        else
            log.add("no button array found, won't do anything");
        // Toolbar API v2 overwrite V1
        fullToolbarConfig.groups[g].buttons = buttonConfigs;
    }
}
exports.expandButtonGroups = expandButtonGroups;
/**
 * take a list of buttons (objects OR strings)
 * and convert to proper array of buttons with actions
 * on the in is a object with buttons, which are either:
 * - a string like "edit" or multi-value "layout,more"
 * - an array of such strings incl. optional complex objects which are
 * @param root
 * @param settings
 */
function expandButtonList(root, settings, parentLog) {
    var log = new log_1.Log('Tlb.ExpBts', parentLog, 'start');
    // let root = grp; // the root object which has all params of the command
    var btns = [];
    var sharedProperties = null;
    // convert compact buttons (with multi-verb action objects) into own button-objects
    // important because an older syntax allowed {action: "new,edit", entityId: 17}
    if (Array.isArray(root.buttons)) {
        log.add("detected array of btns (" + root.buttons.length + "), will ensure it's an object");
        for (var b = 0; b < root.buttons.length; b++) {
            var btn = root.buttons[b];
            var actionString = btn.action;
            if (typeof actionString === 'string' && actionString.indexOf(',') > -1) {
                log.add("button def \"" + btn + " is string of many names, will expand into array with action-properties\"");
                var acts = actionString.split(',');
                for (var a = 0; a < acts.length; a++) {
                    btns.push($.extend(true, {}, btn, { action: acts[a] }));
                }
            }
            else {
                btns.push(btn);
            }
        }
    }
    else if (typeof root.buttons === 'string') {
        log.add("detected that it is a string \"" + root.buttons + "\", will split by \",\" and ...");
        btns = root.buttons.split(',');
        sharedProperties = Object.assign({}, root); // inherit all fields used in the button
        delete sharedProperties.buttons; // this one's not needed
        delete sharedProperties.name; // this one's not needed
        delete sharedProperties.action; //
    }
    else {
        log.add("no special case detected, will use the buttons-object as is");
        btns = root.buttons;
    }
    log.add("after check, found " + btns.length + " buttons");
    // optionally add a more-button in each group
    if (settings.autoAddMore) {
        if ((settings.autoAddMore === 'end')
            || (settings.autoAddMore.toString() === 'right') // fallback for older v1 setting
        ) {
            log.add('will add a more "..." button to end');
            btns.push('more');
        }
        else {
            log.add('will add a more "..." button to start');
            btns.unshift('more');
        }
    }
    else {
        log.add('will not add more "..." button');
    }
    // add each button - check if it's already an object or just the string
    for (var v = 0; v < btns.length; v++) {
        btns[v] = expand_button_config_1.expandButtonConfig(btns[v], sharedProperties, log);
        // todo: refactor this out, not needed any more as they are all together now
        // btns[v].group = root;// grp;    // attach group reference, needed for fallback etc.
    }
    root.buttons = btns; // ensure the internal def is also an array now
    log.add('done');
}


/***/ }),
/* 76 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/** contains a toolbar config + settings + many groups */
var ToolbarConfig = /** @class */ (function () {
    function ToolbarConfig() {
        this.groups = [];
        // todo: old props, remove
        this.name = 'toolbar'; // name, no real use
        this.debug = false; // show more debug info
    }
    return ToolbarConfig;
}());
exports.ToolbarConfig = ToolbarConfig;


/***/ }),
/* 77 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
// the default / initial buttons in a standard toolbar
// ToDo: refactor to avoid side-effects
exports.defaultToolbarTemplate = {
    groups: [
        {
            name: 'default',
            buttons: 'edit,new,metadata,publish,layout',
        }, {
            name: 'list',
            buttons: 'add,remove,moveup,movedown,instance-list,replace,item-history',
        }, {
            name: 'data',
            buttons: 'delete',
        }, {
            name: 'instance',
            buttons: 'template-develop,template-settings,contentitems,template-query,contenttype',
            defaults: {
                classes: 'group-pro',
            },
        }, {
            name: 'app',
            buttons: 'app,app-settings,app-resources,zone',
            defaults: {
                classes: 'group-pro',
            },
        },
    ],
    defaults: {},
    params: {},
    settings: {
        autoAddMore: 'end',
    },
};


/***/ }),
/* 78 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
// the default / initial buttons in a standard toolbar
// ToDo: refactor to avoid side-effects
exports.leftToolbarTemplate = {
    groups: [
        {
            name: 'default',
            buttons: 'edit,new,metadata,publish,layout',
        }, {
            name: 'list',
            buttons: 'add,remove,moveup,movedown,instance-list,replace,item-history',
        }, {
            name: 'data',
            buttons: 'delete',
        }, {
            name: 'instance',
            buttons: 'template-develop,template-settings,contentitems,template-query,contenttype',
            defaults: {
                classes: 'group-pro',
            },
        }, {
            name: 'app',
            buttons: 'app,app-settings,app-resources,zone',
            defaults: {
                classes: 'group-pro',
            },
        },
    ],
    defaults: {},
    params: {},
    settings: {
        autoAddMore: 'start',
    },
};


/***/ }),
/* 79 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var quick_e_1 = __webpack_require__(2);
var selectors_instance_1 = __webpack_require__(8);
var configAttr = 'quick-edit-config';
/**
 * the initial configuration
 */
var conf = quick_e_1.$quickE.config = {
    enable: true,
    innerBlocks: {
        enable: null,
    },
    modules: {
        enable: null,
    },
};
function _readPageConfig() {
    var configs /*: Conf[]*/ = $("[" + configAttr + "]");
    var confJ;
    // any inner blocks found? will currently affect if modules can be inserted...
    var hasInnerCBs = ($(selectors_instance_1.selectors.cb.listSelector).length > 0);
    if (configs.length > 0) {
        // go through reverse list, as the last is the most important...
        var finalConfig = {};
        for (var c = configs.length; c >= 0; c--) {
            confJ = configs[0].getAttribute(configAttr);
            try {
                var confO = JSON.parse(confJ);
                Object.assign(finalConfig, confO);
            }
            catch (e) {
                console.warn('had trouble with json', e);
            }
        }
        Object.assign(conf, finalConfig);
    }
    // re-check "auto" or "null"
    // if it has inner-content, then it's probably a details page, where quickly adding modules would be a problem, so for now, disable modules in this case
    if (conf.modules.enable === null || conf.modules.enable === 'auto')
        conf.modules.enable = !hasInnerCBs;
    // for now, ContentBlocks are only enabled if they exist on the page
    if (conf.innerBlocks.enable === null || conf.innerBlocks.enable === 'auto')
        conf.innerBlocks.enable = hasInnerCBs;
}
exports._readPageConfig = _readPageConfig;


/***/ }),
/* 80 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Coords = /** @class */ (function () {
    function Coords(x, y, w, yh, element) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.yh = yh;
        this.element = element;
    }
    return Coords;
}());
exports.Coords = Coords;


/***/ }),
/* 81 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var templates_1 = __webpack_require__(18);
/*
 * this is a content block in the browser
 *
 * A Content Block is a stand alone unit of content, with it's own definition of
 * 1. content items
 * 2. template
 * + some other stuff
 *
 * it should be able to render itself
 *
 * Maybe ToDo 2cb:
 * 2sxc should have one entry point (interface to browser context) only.
 * Otherwise, we cannot know, when which part will be executed and debugging becomes very difficult.
 *
 */
var MainContentBlock = /** @class */ (function () {
    function MainContentBlock() {
        this.prepareToAddContent = templates_1.prepareToAddContent;
        //updateTemplateFromDia = updateTemplateFromDia;
    }
    // constants
    MainContentBlock.cViewWithoutContent = '_LayoutElement'; // needed to differentiate the "select item" from the "empty-is-selected" which are both empty
    MainContentBlock.cUseExistingTemplate = -1;
    return MainContentBlock;
}());
exports.MainContentBlock = MainContentBlock;
/**
 * The main content-block manager
 */
// ReSharper disable once InconsistentNaming
exports._contentBlock = new MainContentBlock();


/***/ }),
/* 82 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var render_1 = __webpack_require__(16);
var templates_1 = __webpack_require__(18);
var context_1 = __webpack_require__(5);
var api_1 = __webpack_require__(4);
var quick_dialog_1 = __webpack_require__(22);
var quick_dialog_config_1 = __webpack_require__(83);
var scrollTopOffset = 80;
var animationTime = 400;
function build(iFrame) {
    console.log('prot: ', IFrameBridge.prototype);
    var iFrameExtended = iFrame;
    iFrameExtended.bridge = new IFrameBridge();
    console.log('extensions: ', iFrameExtended.bridge);
    return iFrameExtended;
}
exports.build = build;
/**
 *
 */
// ReSharper disable once InconsistentNaming
var IFrameBridge = /** @class */ (function () {
    function IFrameBridge() {
        this.changed = false;
    }
    /**
     * get the sxc-object of this iframe
     * @returns {Object<any>} refreshed sxc-object
     */
    IFrameBridge.prototype.uncachedSxc = function () {
        if (!this.instanceSxc)
            throw "can't find sxc-instance of IFrame, probably it wasn't initialized yet";
        return this.instanceSxc.recreate();
    };
    IFrameBridge.prototype.getContext = function () { return context_1.context(this.uncachedSxc()); };
    IFrameBridge.prototype.getAdditionalDashboardConfig = function () { return quick_dialog_config_1.QuickDialogConfig.fromContext(this.getContext()); };
    IFrameBridge.prototype.hide = function () { quick_dialog_1.quickDialog.setVisible(false); };
    IFrameBridge.prototype.run = function (verb) { this.uncachedSxc().manage.run(verb); };
    IFrameBridge.prototype.cancel = function () { quick_dialog_1.quickDialog.cancel(this); };
    ;
    IFrameBridge.prototype.showMessage = function (message) {
        render_1.renderer.showMessage(this.getContext(), "<p class=\"no-live-preview-available\">" + message + "</p>");
        scrollToTarget(this.tagModule);
    };
    IFrameBridge.prototype.reloadAndReInit = function () {
        var _this = this;
        this.changed = false;
        return render_1.renderer.reloadAndReInitialize(this.getContext(), true, true)
            .then(function () { return scrollToTarget(_this.tagModule); })
            .then(function () { return Promise.resolve(_this.getAdditionalDashboardConfig()); });
    };
    IFrameBridge.prototype.setTemplate = function (templateId, templateName, final) {
        var _this = this;
        this.changed = true;
        var config = this.getAdditionalDashboardConfig(), context = this.getContext();
        var ajax = config.isContent || config.supportsAjax;
        // add msg on full-reload, as it takes longer
        // don't add this on ajax, as it will have side-effects because sometimes
        // in ajax the content won't be replaced
        if (!ajax)
            this.showMessage("refreshing <b>" + templateName + "</b>...");
        var reallySave = final || !ajax;
        var promise = reallySave
            ? templates_1.updateTemplateFromDia(context, templateId)
            : render_1.renderer.ajaxLoad(context, templateId, true);
        if (final)
            promise = promise
                .then(function () { return quick_dialog_1.quickDialog.setVisible(false); });
        promise = ajax
            ? promise.then(function () { return scrollToTarget(_this.tagModule); })
            : promise.then(function () { return window.parent.location.reload(); });
        // return true if ajax, so upstream can update UIs
        return promise.then(function () { return ajax; });
    };
    /**
     * prepare the bridge with the info of the current instance
     */
    IFrameBridge.prototype.setup = function (sxc, dialogName) {
        console.log('rewire with sxc: ', sxc);
        this.changed = false;
        this.instanceSxc = sxc;
        this.tagModule = $($(api_1.getTag(sxc)).parent().eq(0));
        this.sxcCacheKey = sxc.cacheKey;
        if (dialogName)
            this.dialogName = dialogName;
    };
    ;
    /**
    * check if the dialog is showing for the current sxc-instance
    * @param {string} dialogName - name of dialog
    * @returns {boolean} true if it's currently showing for this sxc-instance
    */
    IFrameBridge.prototype.isConfiguredFor = function (instanceId, dialogName) {
        return this.sxcCacheKey === instanceId // the iframe is showing for the current sxc
            && this.dialogName === dialogName; // the view is the same as previously
    };
    return IFrameBridge;
}());
exports.IFrameBridge = IFrameBridge;
function scrollToTarget(target) {
    var specs = {
        scrollTop: target.offset().top - scrollTopOffset
    };
    $('body').animate(specs, animationTime);
}
;


/***/ }),
/* 83 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var user_of_edit_context_1 = __webpack_require__(23);
var QuickDialogConfig = /** @class */ (function () {
    function QuickDialogConfig() {
    }
    //constructor(editContext: DataEditContext) {
    //  this.appId = editContext.ContentGroup.AppId;
    //  this.isContent = editContext.ContentGroup.IsContent;
    //  this.hasContent = editContext.ContentGroup.HasContent;
    //  this.isList = editContext.ContentGroup.IsList;
    //  this.templateId = editContext.ContentGroup.TemplateId;
    //  this.contentTypeId = editContext.ContentGroup.ContentTypeName;
    //  this.templateChooserVisible = editContext.ContentBlock.ShowTemplatePicker; // todo = maybe move to content-group
    //  this.user = getUserOfEditContext(editContext);
    //  this.supportsAjax = editContext.ContentGroup.SupportsAjax;
    //}
    QuickDialogConfig.fromContext = function (context) {
        var config = new QuickDialogConfig();
        config.appId = context.app.id;
        config.isContent = context.app.isContent;
        config.isInnerContent = context.instance.id !== context.contentBlock.id; // if it differs, it's inner
        config.hasContent = context.app.hasContent;
        config.isList = context.contentBlock.isList;
        config.templateId = context.contentBlock.templateId;
        config.contentTypeId = context.contentBlock.contentTypeId;
        config.user = user_of_edit_context_1.UserOfEditContext.fromContext(context);
        config.supportsAjax = context.app.supportsAjax;
        config.debug = window.$2sxc.debug.load;
        return config;
    };
    return QuickDialogConfig;
}());
exports.QuickDialogConfig = QuickDialogConfig;


/***/ }),
/* 84 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var DebugConfig_1 = __webpack_require__(17);
/**
 * This object helps persist / load / reset
 * a setting in the session-state
 * */
var SessionStateHandler = /** @class */ (function () {
    function SessionStateHandler(key) {
        this.key = key;
    }
    SessionStateHandler.prototype.set = function (value) {
        if (DebugConfig_1.DebugConfig.state.change)
            console.log("state '" + this.key + "' set(" + value + ")");
        sessionStorage.setItem(this.key, value);
    };
    ;
    SessionStateHandler.prototype.remove = function () {
        if (DebugConfig_1.DebugConfig.state.change)
            console.log("state '" + this.key + "' remove()");
        sessionStorage.removeItem(this.key);
    };
    SessionStateHandler.prototype.get = function () {
        var result = SessionStorageHelper.getItemValue(this.key);
        if (DebugConfig_1.DebugConfig.state.get)
            console.log("state '" + this.key + "' get() = '" + result + "'");
        return result;
    };
    return SessionStateHandler;
}());
exports.SessionStateHandler = SessionStateHandler;
/**
 * session storage helper to get typed values from it
 */
var SessionStorageHelper = /** @class */ (function () {
    function SessionStorageHelper() {
    }
    SessionStorageHelper.getItemValueString = function (key) {
        var value = sessionStorage.getItem(key);
        return value;
    };
    SessionStorageHelper.getItemValue = function (key) {
        var value = sessionStorage.getItem(key);
        return JSON.parse(value);
    };
    return SessionStorageHelper;
}());


/***/ }),
/* 85 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var DialogPaths_1 = __webpack_require__(24);
/**
 * rewrite the url to fit the quick-dialog situation
 * optionally with a live-compiled version from ng-serve
 * @param {string} url - original url pointing to the default dialog
 * @returns {string} new url pointing to quick dialog
 */
function setUrlToQuickDialog(url) {
    // change default url-schema from the primary angular-app to the quick-dialog
    url = url.replace(DialogPaths_1.DialogPaths.ng1, DialogPaths_1.DialogPaths.quickDialog)
        .replace(DialogPaths_1.DialogPaths.ng8, DialogPaths_1.DialogPaths.quickDialog);
    url = changePathToLocalhostForDev(url);
    return url;
}
exports.setUrlToQuickDialog = setUrlToQuickDialog;
/**
 * special debug-code when running on local ng-serve
 * this is only activated if the developer manually sets a value in the localStorage
 * @param url
 */
function changePathToLocalhostForDev(url) {
    try {
        var devMode = localStorage.getItem('devMode');
        if (devMode && !!devMode) {
            return url.replace('/desktopmodules/tosic_sexycontent/dist/ng/ui.html', 'http://localhost:4200');
        }
    }
    catch (e) {
        // ignore
    }
    return url;
}


/***/ }),
/* 86 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var command_create_1 = __webpack_require__(87);
/**
 * create a dialog link
 * @param sxc
 * @param specialSettings
 */
function commandLinkToNgDialog(context) {
    var cmd = command_create_1.commandCreate(context);
    if (cmd.context.button.action.params.useModuleList) {
        cmd.addContentGroupItemSetsToEditList(true);
    }
    else {
        cmd.addSimpleItem();
    }
    ;
    // if the command has own configuration stuff, do that now
    if (cmd.context.button.configureCommand) {
        cmd.context.button.configureCommand(context, cmd);
    }
    return cmd.generateLink(context);
}
exports.commandLinkToNgDialog = commandLinkToNgDialog;


/***/ }),
/* 87 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var window_in_page_1 = __webpack_require__(1);
var DialogPaths_1 = __webpack_require__(24);
var command_1 = __webpack_require__(88);
/**
 * assemble an object which will store the configuration and execute it
 */
function commandCreate(context) {
    var ngDialogUrl = context.instance.sxcRootUrl +
        'desktopmodules/tosic_sexycontent/' +
        ((context.ui.form === 'ng8' && context.button.dialog(context) === 'edit') ? DialogPaths_1.DialogPaths.ng8 : DialogPaths_1.DialogPaths.ng1) +
        '?sxcver=' + context.instance.sxcVersion;
    var debugUrlParam = window_in_page_1.windowInPage.$2sxc.urlParams.get('debug') ? '&debug=true' : '';
    var cmd = new command_1.Command(context, ngDialogUrl, debugUrlParam);
    return cmd;
}
exports.commandCreate = commandCreate;


/***/ }),
/* 88 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
Object.defineProperty(exports, "__esModule", { value: true });
var ng_dialog_params_1 = __webpack_require__(89);
var _2sxc_translate_1 = __webpack_require__(9);
var Command = /** @class */ (function () {
    function Command(context, ngDialogUrl, isDebug) {
        var _this = this;
        this.context = context;
        this.ngDialogUrl = ngDialogUrl;
        this.isDebug = isDebug;
        this.evalPropOrFunction = function (propOrFunction, context, fallback) {
            if (propOrFunction === undefined || propOrFunction === null) {
                return fallback;
            }
            return (typeof (propOrFunction) === 'function' ? propOrFunction(context) : propOrFunction);
        };
        this.addSimpleItem = function () {
            var item = {};
            var params = _this.context.button.action.params;
            var ct = params.contentType || params.attributeSetName; // two ways to name the content-type-name this, v 7.2+ and older
            if (params.entityId)
                item.EntityId = params.entityId;
            if (ct)
                item.ContentTypeName = ct;
            // only add if there was stuff to add
            if (item.EntityId || item.ContentTypeName) {
                console.warn('used the simple item header - test if dialog still works!');
                // this.items.push(item);
                _this.items.push(__assign({}, item, { Title: _2sxc_translate_1.translate(_this.findTranslationKey(_this.findPartName(true))) }));
            }
        };
        // this will tell the command to edit a item from the sorted list in the group, optionally together with the presentation item
        this.addContentGroupItemSetsToEditList = function (withPresentation) {
            var isContentAndNotHeader = (_this.context.button.action.params.sortOrder !== -1);
            var index = isContentAndNotHeader ? _this.context.button.action.params.sortOrder : 0;
            var cTerm = _this.findPartName(true);
            var pTerm = _this.findPartName(false);
            var isAdd = _this.context.button.action.name === 'new';
            var groupId = _this.context.contentBlock.contentGroupId;
            _this.addContentGroupItem(groupId, index, cTerm, isAdd);
            if (withPresentation)
                _this.addContentGroupItem(groupId, index, pTerm, isAdd);
        };
        // build the link, combining specific params with global ones and put all in the url
        this.generateLink = function (context) {
            // if there is no items-array, create an empty one (it's required later on)
            if (!context.button.action.params.items) {
                context.button.action.params.items = [];
            }
            //#region steps for all actions: prefill, serialize, open-dialog
            // when doing new, there may be a prefill in the link to initialize the new item
            if (context.button.action.params.prefill) {
                for (var i = 0; i < _this.items.length; i++) {
                    _this.items[i].Prefill = context.button.action.params.prefill;
                }
            }
            _this.params.items = JSON.stringify(_this.items); // Serialize/json-ify the complex items-list
            // clone the params and adjust parts based on partOfPage settings...
            var ngDialogParams = ng_dialog_params_1.NgDialogParams.fromContext(context); // 2dm simplified buildNgDialogParams(context);
            var sharedParams = Object.assign({}, ngDialogParams);
            var partOfPage = context.button.partOfPage(context);
            if (!partOfPage) {
                delete sharedParams.versioningRequirements;
                delete sharedParams.publishing;
                sharedParams.partOfPage = false;
            }
            return _this.ngDialogUrl +
                '#' +
                $.param(sharedParams) +
                '&' +
                $.param(_this.params) +
                _this.isDebug;
            //#endregion
        };
        // this.settings = settings;
        this.items = context.button.action.params.items || []; // use predefined or create empty array
        // todo: stv, clean this
        var params = this.evalPropOrFunction(context.button.params, context, {});
        var dialog = this.evalPropOrFunction(context.button.dialog, context, {});
        this.params = Object.assign({
            dialog: dialog || context.button.action.name,
        }, params);
    }
    // this adds an item of the content-group, based on the group GUID and the sequence number
    Command.prototype.addContentGroupItem = function (guid, index, part, isAdd) {
        this.items.push({
            Group: {
                Guid: guid,
                Index: index,
                Part: part.toLocaleLowerCase(),
                Add: isAdd,
            },
            Title: _2sxc_translate_1.translate(this.findTranslationKey(part)),
        });
    };
    /** find the part name for both the API to give the right item (when using groups) and for i18n */
    Command.prototype.findPartName = function (content) {
        var isContentAndNotHeader = (this.context.button.action.params.sortOrder !== -1);
        return (isContentAndNotHeader ? '' : 'List') + (content ? 'Content' : 'Presentation');
    };
    /** find the correct i18n key for this part */
    Command.prototype.findTranslationKey = function (partName) {
        return "EditFormTitle." + partName;
    };
    return Command;
}());
exports.Command = Command;


/***/ }),
/* 89 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var user_of_edit_context_1 = __webpack_require__(23);
var NgDialogParams = /** @class */ (function () {
    function NgDialogParams() {
    }
    // constructor(sxc: SxcInstanceWithInternals, editContext: DataEditContext) {
    //  this.zoneId = editContext.ContentGroup.ZoneId;
    //  this.appId = editContext.ContentGroup.AppId;
    //  this.tid = editContext.Environment.PageId;
    //  this.mid = editContext.Environment.InstanceId;
    //  this.cbid = sxc.cbid;
    //  this.lang = editContext.Language.Current;
    //  this.langpri = editContext.Language.Primary;
    //  this.langs = JSON.stringify(editContext.Language.All);
    //  this.portalroot = editContext.Environment.WebsiteUrl;
    //  this.websiteroot = editContext.Environment.SxcRootUrl;
    //  this.partOfPage = editContext.ContentBlock.PartOfPage;
    //  // versioningRequirements= editContext.ContentBlock.VersioningRequirements;
    //  this.publishing = editContext.ContentBlock.VersioningRequirements;
    //  // todo= probably move the user into the dashboard info
    //  this.user = getUserOfEditContext(editContext);
    //  this.approot = editContext.ContentGroup.AppUrl || null; // this is the only value which doesn't have a slash by default. note that the app-root doesn't exist when opening "manage-app"
    // }
    NgDialogParams.fromContext = function (context) {
        var params = new NgDialogParams();
        params.zoneId = context.app.zoneId;
        params.appId = context.app.id;
        params.tid = context.page.id;
        params.mid = context.instance.id;
        params.cbid = context.contentBlock.id;
        params.lang = context.app.currentLanguage;
        params.langpri = context.app.primaryLanguage;
        params.langs = JSON.stringify(context.app.allLanguages);
        params.portalroot = context.tenant.url;
        params.websiteroot = context.instance.sxcRootUrl;
        params.partOfPage = context.contentBlock.partOfPage;
        // versioningRequirements= editContext.ContentBlock.VersioningRequirements;
        params.publishing = context.contentBlock.versioningRequirements;
        // todo= probably move the user into the dashboard info
        params.user = user_of_edit_context_1.UserOfEditContext.fromContext(context);
        params.approot = context.app.appPath || null; // this is the only value which doesn't have a slash by default. note that the app-root doesn't exist when opening "manage-app"
        params.fa = !context.app.isContent;
        params.rvt = $.ServicesFramework(0).getAntiForgeryValue();
        console.log('rvt', params.rvt);
        return params;
    };
    return NgDialogParams;
}());
exports.NgDialogParams = NgDialogParams;


/***/ }),
/* 90 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var create_1 = __webpack_require__(91);
/**
 * A helper-controller in charge of opening edit-dialogues + creating the toolbars for it
 * all in-page toolbars etc.
 * if loaded, it's found under the $2sxc(module).manage
 * it has commands to
 * - getButton
 * - getToolbar
 * - run(...)
 * - isEditMode
 */
var Manage = /** @class */ (function () {
    function Manage() {
        this.initInstance = create_1.initInstance;
    }
    return Manage;
}());
exports._manage = new Manage(); // used out of this project in ToSic.Sxc.Instance and 2sxc.api.js


/***/ }),
/* 91 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var instance_engine_1 = __webpack_require__(92);
var manipulate_1 = __webpack_require__(93);
var context_1 = __webpack_require__(5);
var render_button_1 = __webpack_require__(19);
var render_toolbar_1 = __webpack_require__(13);
var toolbar_expand_config_1 = __webpack_require__(29);
var api_1 = __webpack_require__(4);
var user_of_edit_context_1 = __webpack_require__(23);
var button_config_adapter_1 = __webpack_require__(95);
/**
 * A helper-controller in charge of opening edit-dialogues + creating the toolbars for it
 * all in-page toolbars etc.
 * if loaded, it's found under the $2sxc(module).manage
 * it has commands to
 * - getButton
 * - getToolbar
 * - run(...)
 * - isEditMode
 * @param sxc
 *
 * we must keep signature of initInstance for compatibility because it is used out of this project in ToSic.Sxc.Instance and 2sxc.api.js
 */
function initInstance(sxc) {
    try {
        _initInstance(sxc);
    }
    catch (e) {
        console.error('error in 2sxc - will log but not throw', e);
    }
}
exports.initInstance = initInstance;
// ReSharper disable once InconsistentNaming
function _initInstance(sxc) {
    var myContext = context_1.context(sxc);
    var editContext = api_1.getEditContext(myContext.sxc);
    var userInfo = user_of_edit_context_1.UserOfEditContext.fromContext(myContext); // 2dm simplified getUserOfEditContext(context);
    var cmdEngine = new instance_engine_1.InstanceEngine(myContext.sxc);
    var editManager = new EditManager(myContext.sxc, editContext, userInfo, cmdEngine, myContext);
    sxc.manage = editManager;
    editManager.init();
    return editManager;
}
var EditManager = /** @class */ (function () {
    function EditManager(sxc, editContext, userInfo, cmdEngine, context) {
        var _this = this;
        this.sxc = sxc;
        this.editContext = editContext;
        this.userInfo = userInfo;
        this.cmdEngine = cmdEngine;
        this.context = context;
        //#region Official, public properties and commands, which are stable for use from the outside
        /**
         * run a command - command used in toolbars and custom buttons
         * it is publicly used out of inpage, so take a care to preserve function signature
         */
        this.run = this.cmdEngine.run;
        /**
         * Generate a button (an <a>-tag) for one specific toolbar-action.
         * @param {Object<any>} actDef - settings, an object containing the spec for the expected button
         * @param {int} groupIndex - number what button-group it's in'
         * @returns {string} html of a button
         * it is publicly used out of inpage, so take a care to preserve function signature
         */
        this.getButton = function (actDef, groupIndex) {
            //const tag: any = getTag(this.sxc);
            //const myContext = context(tag);
            var newButtonConfig = button_config_adapter_1.buttonConfigAdapter(_this.context, actDef, groupIndex);
            _this.context.button = newButtonConfig;
            var button = render_button_1.renderButton(_this.context, groupIndex);
            return button.outerHTML;
        };
        /**
         * Builds the toolbar and returns it as HTML
         * @param {Object<any>} tbConfig - general toolbar config
         * @param {Object<any>} moreSettings - additional / override settings
         * @returns {string} html of the current toolbar
         *
         * it is publicly used out of inpage, so take a care to preserve function signature
         */
        this.getToolbar = function (tbConfig, moreSettings) {
            //const tag: any = getTag(this.sxc);
            //const myContext = context(tag);
            var toolbarConfig = toolbar_expand_config_1.expandToolbarConfig(_this.context, tbConfig, moreSettings);
            _this.context.toolbar = toolbarConfig;
            return render_toolbar_1.renderToolbar(_this.context);
        };
        //#endregion official, public properties - everything below this can change at any time
        this._context = this.context;
        // ReSharper disable InconsistentNaming
        /**
         * internal method to find out if it's in edit-mode
         */
        this._isEditMode = function () { return _this.editContext.Environment.IsEditable; };
        /**
         * used for various dialogues
         */
        this._reloadWithAjax = this.context.app.supportsAjax;
        // #region 2dm disabled / todo q2stv
        // todo q2stv - I think we don't need this any more
        // 
        //_dialogParameters = buildNgDialogParams(this.context);
        // 2dm disabled
        // todo q2stv - I think we don't need this any more
        /**
          * used to configure buttons / toolbars
          */
        //_instanceConfig = buildInstanceConfig(this.context);
        // 2dm disabled
        // todo q2stv - I think we don't need this any more
        /**
         * used for in-page dialogues
         */
        //_quickDialogConfig = buildQuickDialogConfig(this.context);
        //#endregion
        /** metadata necessary to know what/how to edit */
        this._editContext = this.editContext;
        /** used to handle the commands for this content-block */
        this._commands = this.cmdEngine;
        this._user = this.userInfo;
        this._getCbManipulator = function () { return manipulate_1.manipulator(_this.sxc); };
        // ReSharper restore InconsistentNaming
        /**
         * init this object
         */
        this.init = function () {
            var tag = api_1.getTag(_this.sxc);
            // enhance UI in case there are known errors / issues
            var isErrorState = _this.editContext && _this.editContext.error && _this.editContext.error.type;
            if (isErrorState)
                handleErrors(_this.editContext.error.type, tag);
        };
    }
    /**
     * change config by replacing the guid, and refreshing dependent sub-objects
     */
    EditManager.prototype._updateContentGroupGuid = function (context, newGuid) {
        context.contentBlock.contentGroupId = newGuid;
        this.editContext.ContentGroup.Guid = newGuid;
        // 2dm disabled, doesn't seem used - 
        // todo q2stv - question, pls confirm
        //this._instanceConfig = InstanceConfig.fromContext(context);// 2dm simplified buildInstanceConfig(context);
    };
    return EditManager;
}());
exports.EditManager = EditManager;
/**
 * private: show error when the app-data hasn't been installed yet for this imported-module
 */
function handleErrors(errType, cbTag) {
    var errWrapper = $('<div class="dnnFormMessage dnnFormWarning sc-element"></div>');
    var msg = '';
    var toolbar = $("<ul class='sc-menu'></ul>");
    if (errType === 'DataIsMissing') {
        msg =
            'Error: System.Exception: Data is missing - usually when a site is copied but the content / apps have not been imported yet - check 2sxc.org/help?tag=export-import';
        toolbar.attr('data-toolbar', '[{\"action\": \"zone\"}, {\"action\": \"more\"}]');
    }
    errWrapper.append(msg);
    errWrapper.append(toolbar);
    $(cbTag).append(errWrapper);
}


/***/ }),
/* 92 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Cms_1 = __webpack_require__(26);
var context_1 = __webpack_require__(5);
var InstanceEngine = /** @class */ (function () {
    function InstanceEngine(sxc) {
        this.sxc = sxc;
    }
    InstanceEngine.prototype.run = function (nameOrSettings, eventOrSettings, event) {
        var cntx = context_1.context(this.sxc);
        return new Cms_1.Cms().run(cntx, nameOrSettings, eventOrSettings, event);
    };
    return InstanceEngine;
}());
exports.InstanceEngine = InstanceEngine;


/***/ }),
/* 93 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var toolbar_manager_1 = __webpack_require__(94);
var _2sxc_translate_1 = __webpack_require__(9);
var sxc_1 = __webpack_require__(6);
/** contains commands to create/move/delete a contentBlock in a page */
var sxcInstance;
/**
 * create content block
 * @param parentId
 * @param fieldName
 * @param index
 * @param appName
 * @param container
 * @param newGuid
 */
function create(parentId, fieldName, index, appName, container, newGuid) {
    // the wrapper, into which this will be placed and the list of pre-existing blocks
    var listTag = container;
    if (listTag.length === 0)
        return alert('can\'t add content-block as we couldn\'t find the list');
    var cblockList = listTag.find('div.sc-content-block');
    if (index > cblockList.length)
        index = cblockList.length; // make sure index is never greater than the amount of items
    var params = {
        parentId: parentId,
        field: fieldName,
        sortOrder: index,
        app: appName,
        guid: newGuid,
    };
    return sxcInstance.webApi.get({ url: 'view/module/generatecontentblock', params: params })
        .then(function (result) {
        var newTag = $(result); // prepare tag for inserting
        // should I add it to a specific position...
        if (cblockList.length > 0 && index > 0)
            $(cblockList[cblockList.length > index - 1 ? index - 1 : cblockList.length - 1])
                .after(newTag);
        else // ...or just at the beginning?
            listTag.prepend(newTag);
        // ReSharper disable once UnusedLocals
        var sxcNew = sxc_1.getSxcInstance(newTag);
        toolbar_manager_1._toolbarManager.buildToolbars(newTag);
    });
}
/**
 * move content block
 * @param parentId
 * @param field
 * @param indexFrom
 * @param indexTo
 */
function move(parentId, field, indexFrom, indexTo) {
    var params = {
        parentId: parentId,
        field: field,
        indexFrom: indexFrom,
        indexTo: indexTo,
    };
    return sxcInstance.webApi.get({ url: 'view/module/moveiteminlist', params: params })
        .then(function () {
        console.log('done moving!');
        window.location.reload();
    });
}
/**
 * delete a content-block inside a list of content-blocks
 * @param parentId
 * @param field
 * @param index
 */
function remove(parentId, field, index) {
    if (!confirm(_2sxc_translate_1.translate('QuickInsertMenu.ConfirmDelete')))
        return null;
    var params = {
        parentId: parentId,
        field: field,
        index: index,
    };
    return sxcInstance.webApi.get({ url: 'view/module/RemoveItemInList', params: params })
        .then(function () {
        console.log('done deleting!');
        window.location.reload();
    });
}
var Manipulator = /** @class */ (function () {
    function Manipulator() {
        this.create = create;
        this.move = move;
        this.delete = remove;
    }
    return Manipulator;
}());
exports.Manipulator = Manipulator;
function manipulator(sxc) {
    sxcInstance = sxc;
    return new Manipulator();
}
exports.manipulator = manipulator;


/***/ }),
/* 94 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var has_log_1 = __webpack_require__(15);
var build_toolbars_1 = __webpack_require__(12);
var render_button_1 = __webpack_require__(19);
var render_toolbar_1 = __webpack_require__(13);
var toolbar_config_templates_1 = __webpack_require__(34);
/**
 * Toolbar manager for the whole page - basically a set of APIs
 * the toolbar manager is an internal helper taking care of toolbars, buttons etc.
 */
var ToolbarManager = /** @class */ (function (_super) {
    __extends(ToolbarManager, _super);
    function ToolbarManager(parentLog) {
        var _this = _super.call(this, 'Tlb.Mngr', parentLog, 'init') || this;
        _this.disable = build_toolbars_1.disable;
        _this.isDisabled = build_toolbars_1.isDisabled;
        // generate button html
        _this.generateButtonHtml = render_button_1.renderButton;
        _this.generateToolbarHtml = render_toolbar_1.renderToolbar;
        _this.toolbarTemplate = toolbar_config_templates_1.ToolbarConfigTemplates.Instance(_this.log).get('default');
        return _this;
    }
    ToolbarManager.prototype.buildToolbars = function (parentTag, optionalId) {
        build_toolbars_1.buildToolbars(this.log, parentTag, optionalId);
    };
    return ToolbarManager;
}(has_log_1.HasLog));
exports.ToolbarManager = ToolbarManager;
//2dm 2018-03-22 this seems to be unused
var sharedTbm = new ToolbarManager(null);
exports._toolbarManager = sharedTbm;


/***/ }),
/* 95 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var commands_1 = __webpack_require__(10);
var button_action_1 = __webpack_require__(20);
var button_config_1 = __webpack_require__(21);
var expand_button_config_1 = __webpack_require__(14);
var mod_config_1 = __webpack_require__(96);
var flatten_action_definition_1 = __webpack_require__(32);
var parameters_adapter_1 = __webpack_require__(30);
function buttonConfigAdapter(context, actDef, groupIndex) {
    var partialButtonConfig = {};
    if (actDef.code) {
        partialButtonConfig.code = function (context) {
            var modConfig = new mod_config_1.ModConfig();
            // todo: stv find this data
            // modConfig.target = '';
            // modConfig.isList = false;
            return actDef.code(context.button.action.params, modConfig);
        };
    }
    if (actDef.icon) {
        partialButtonConfig.icon = function (context) {
            return "icon-sxc-" + actDef.icon;
        };
    }
    if (actDef.classes) {
        partialButtonConfig.classes = actDef.classes;
    }
    if (actDef.dialog) {
        partialButtonConfig.dialog = function (context) {
            return actDef.dialog;
        };
    }
    if (actDef.disabled) {
        partialButtonConfig.disabled = function (context) {
            return actDef.disabled;
        };
    }
    if (actDef.dynamicClasses) {
        partialButtonConfig.dynamicClasses = function (context) {
            return actDef.dynamicClasses(context.button.action.params);
        };
    }
    if (actDef.fullScreen) {
        partialButtonConfig.fullScreen = function (context) {
            return actDef.fullScreen;
        };
    }
    if (actDef.inlineWindow) {
        partialButtonConfig.inlineWindow = function (context) {
            return actDef.inlineWindow;
        };
    }
    if (actDef.name) {
        partialButtonConfig.name = actDef.name;
    }
    if (actDef.newWindow) {
        partialButtonConfig.newWindow = function (context) {
            return actDef.newWindow;
        };
    }
    if (actDef.params) {
        // todo: stv, this do not looking good, because old simple parameters become methods with context as parameter,
        // we need parameter adapter to do this...
        Object.assign(partialButtonConfig.params, actDef.params);
    }
    if (actDef.partOfPage) {
        partialButtonConfig.partOfPage = function (context) {
            return actDef.partOfPage;
        };
    }
    if (actDef.showCondition) {
        partialButtonConfig.showCondition = function (context) {
            var modConfig = new mod_config_1.ModConfig();
            // todo: stv find this data
            // modConfig.target = '';
            // modConfig.isList = false;
            return actDef.showCondition(context.button.action.params, modConfig);
        };
    }
    if (actDef.title) {
        partialButtonConfig.title = function (context) {
            return "Toolbar." + actDef.title;
        };
    }
    if (actDef.uiActionOnly) {
        partialButtonConfig.uiActionOnly = function (context) {
            return actDef.uiActionOnly;
        };
    }
    actDef = (expand_button_config_1.expandButtonConfig(actDef, [], null));
    var name = actDef.command.action;
    var contentType = actDef.command.contentType;
    // if the button belongs to a content-item, move the specs up to the item into the settings-object
    flatten_action_definition_1.flattenActionDefinition(actDef.command);
    // parameters adapter from v1 to v2
    var params = parameters_adapter_1.parametersAdapter(actDef.command);
    // Toolbar API v2
    var actions = commands_1.Commands.getInstance();
    var newButtonAction = new button_action_1.ButtonAction(name, contentType, params);
    newButtonAction.commandDefinition = actions.get(name);
    var newButtonConfig = new button_config_1.ButtonConfig(newButtonAction);
    newButtonConfig.name = name;
    return newButtonConfig;
}
exports.buttonConfigAdapter = buttonConfigAdapter;


/***/ }),
/* 96 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ModConfig = /** @class */ (function () {
    function ModConfig() {
    }
    return ModConfig;
}());
exports.ModConfig = ModConfig;


/***/ }),
/* 97 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var i18next = __webpack_require__(98);
var i18nextXHRBackend = __webpack_require__(99);
var jqueryI18next = __webpack_require__(100);
var context_1 = __webpack_require__(5);
var window_in_page_1 = __webpack_require__(1);
var api_1 = __webpack_require__(4);
var sxc_1 = __webpack_require__(6);
/**
 * initialize the translation system; ensure toolbars etc. are translated
 */
window_in_page_1.windowInPage.i18next = i18next;
window_in_page_1.windowInPage.i18nextXHRBackend = i18nextXHRBackend;
var initialized = false;
// ReSharper disable once InconsistentNaming
function _translateInit(manage) {
    if (initialized) {
        return;
    }
    var context = manage._context;
    if (!context) {
        initialized = true; // getScxInstance is calling _translate so that we can skip the loop...
        // trying to get context...
        var htmlElementOrId = $('div[data-cb-id]')[0];
        var sxc = sxc_1.getSxcInstance(htmlElementOrId);
        initialized = false; // for real, it is not initialized...
        var editContext = api_1.getEditContext(sxc);
        context = context_1.createContextFromEditContext(editContext);
        context.sxc = sxc;
    }
    //console.log('stv: compare #1',
    //  manage._editContext.Language.Current.substr(0, 2),
    //  context.app.currentLanguage.substr(0, 2));
    //console.log('stv: compare #2',
    //  manage._editContext.Environment.SxcRootUrl,
    //  context.instance.sxcRootUrl);
    window_in_page_1.windowInPage.i18next
        .use(i18nextXHRBackend)
        .init({
        lng: context.app.currentLanguage.substr(0, 2),
        fallbackLng: 'en',
        whitelist: ['en', 'de', 'fr', 'it', 'uk', 'nl'],
        preload: ['en'],
        backend: {
            loadPath: context.instance.sxcRootUrl + 'desktopmodules/tosic_sexycontent/dist/i18n/inpage-{{lng}}.js',
        },
    }, function (err, t) {
        // ReSharper restore UnusedParameter
        // for options see
        // https://github.com/i18next/jquery-i18next#initialize-the-plugin
        // ReSharper disable once TsResolvedFromInaccessibleModule
        jqueryI18next.init(i18next, $);
        // start localizing, details:
        // https://github.com/i18next/jquery-i18next#usage-of-selector-function
        $('ul.sc-menu').localize(); // inline toolbars
        $('.sc-i18n').localize(); // quick-insert menus
    });
    initialized = true;
}
exports._translateInit = _translateInit;


/***/ }),
/* 98 */
/***/ (function(module, exports, __webpack_require__) {

!function(e,t){ true?module.exports=t():"function"==typeof define&&define.amd?define("i18next",t):e.i18next=t()}(this,function(){"use strict";function e(e){return null==e?"":""+e}function t(e,t,n){e.forEach(function(e){t[e]&&(n[e]=t[e])})}function n(e,t,n){function o(e){return e&&e.indexOf("###")>-1?e.replace(/###/g,"."):e}for(var r="string"!=typeof t?[].concat(t):t.split(".");r.length>1;){if(!e)return{};var i=o(r.shift());!e[i]&&n&&(e[i]=new n),e=e[i]}return e?{obj:e,k:o(r.shift())}:{}}function o(e,t,o){var r=n(e,t,Object),i=r.obj,s=r.k;i[s]=o}function r(e,t,o,r){var i=n(e,t,Object),s=i.obj,a=i.k;s[a]=s[a]||[],r&&(s[a]=s[a].concat(o)),r||s[a].push(o)}function i(e,t){var o=n(e,t),r=o.obj,i=o.k;return r?r[i]:void 0}function s(e,t,n){for(var o in t)o in e?"string"==typeof e[o]||e[o]instanceof String||"string"==typeof t[o]||t[o]instanceof String?n&&(e[o]=t[o]):s(e[o],t[o],n):e[o]=t[o];return e}function a(e){return e.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g,"\\$&")}function l(e){return"string"==typeof e?e.replace(/[&<>"'\/]/g,function(e){return C[e]}):e}function u(e){return e.interpolation={unescapeSuffix:"HTML"},e.interpolation.prefix=e.interpolationPrefix||"__",e.interpolation.suffix=e.interpolationSuffix||"__",e.interpolation.escapeValue=e.escapeInterpolation||!1,e.interpolation.nestingPrefix=e.reusePrefix||"$t(",e.interpolation.nestingSuffix=e.reuseSuffix||")",e}function c(e){return e.resStore&&(e.resources=e.resStore),e.ns&&e.ns.defaultNs?(e.defaultNS=e.ns.defaultNs,e.ns=e.ns.namespaces):e.defaultNS=e.ns||"translation",e.fallbackToDefaultNS&&e.defaultNS&&(e.fallbackNS=e.defaultNS),e.saveMissing=e.sendMissing,e.saveMissingTo=e.sendMissingTo||"current",e.returnNull=!e.fallbackOnNull,e.returnEmptyString=!e.fallbackOnEmpty,e.returnObjects=e.returnObjectTrees,e.joinArrays="\n",e.returnedObjectHandler=e.objectTreeKeyHandler,e.parseMissingKeyHandler=e.parseMissingKey,e.appendNamespaceToMissingKey=!0,e.nsSeparator=e.nsseparator,e.keySeparator=e.keyseparator,"sprintf"===e.shortcutFunction&&(e.overloadTranslationOptionHandler=function(e){for(var t=[],n=1;n<e.length;n++)t.push(e[n]);return{postProcess:"sprintf",sprintf:t}}),e.whitelist=e.lngWhitelist,e.preload=e.preload,"current"===e.load&&(e.load="currentOnly"),"unspecific"===e.load&&(e.load="languageOnly"),e.backend=e.backend||{},e.backend.loadPath=e.resGetPath||"locales/__lng__/__ns__.json",e.backend.addPath=e.resPostPath||"locales/add/__lng__/__ns__",e.backend.allowMultiLoading=e.dynamicLoad,e.cache=e.cache||{},e.cache.prefix="res_",e.cache.expirationTime=6048e5,e.cache.enabled=!!e.useLocalStorage,e=u(e),e.defaultVariables&&(e.interpolation.defaultVariables=e.defaultVariables),e}function p(e){return e=u(e),e.joinArrays="\n",e}function f(e){return(e.interpolationPrefix||e.interpolationSuffix||e.escapeInterpolation)&&(e=u(e)),e.nsSeparator=e.nsseparator,e.keySeparator=e.keyseparator,e.returnObjects=e.returnObjectTrees,e}function h(e){e.lng=function(){return S.deprecate("i18next.lng() can be replaced by i18next.language for detected language or i18next.languages for languages ordered by translation lookup."),e.services.languageUtils.toResolveHierarchy(e.language)[0]},e.preload=function(t,n){S.deprecate("i18next.preload() can be replaced with i18next.loadLanguages()"),e.loadLanguages(t,n)},e.setLng=function(t,n,o){return S.deprecate("i18next.setLng() can be replaced with i18next.changeLanguage() or i18next.getFixedT() to get a translation function with fixed language or namespace."),"function"==typeof n&&(o=n,n={}),n||(n={}),n.fixLng===!0&&o?o(null,e.getFixedT(t)):void e.changeLanguage(t,o)},e.addPostProcessor=function(t,n){S.deprecate("i18next.addPostProcessor() can be replaced by i18next.use({ type: 'postProcessor', name: 'name', process: fc })"),e.use({type:"postProcessor",name:t,process:n})}}function g(e){return e.charAt(0).toUpperCase()+e.slice(1)}function d(){var e={};return R.forEach(function(t){t.lngs.forEach(function(n){return e[n]={numbers:t.nr,plurals:P[t.fc]}})}),e}function v(e,t){for(var n=e.indexOf(t);-1!==n;)e.splice(n,1),n=e.indexOf(t)}function y(){return{debug:!1,ns:["translation"],defaultNS:["translation"],fallbackLng:["dev"],fallbackNS:!1,whitelist:!1,load:"all",preload:!1,keySeparator:".",nsSeparator:":",pluralSeparator:"_",contextSeparator:"_",saveMissing:!1,saveMissingTo:"fallback",missingKeyHandler:!1,postProcess:!1,returnNull:!0,returnEmptyString:!0,returnObjects:!1,joinArrays:!1,returnedObjectHandler:function(){},parseMissingKeyHandler:!1,appendNamespaceToMissingKey:!1,overloadTranslationOptionHandler:function(e){return{defaultValue:e[1]}},interpolation:{escapeValue:!0,prefix:"{{",suffix:"}}",unescapePrefix:"-",nestingPrefix:"$t(",nestingSuffix:")",defaultVariables:void 0}}}function b(e){return"string"==typeof e.ns&&(e.ns=[e.ns]),"string"==typeof e.fallbackLng&&(e.fallbackLng=[e.fallbackLng]),"string"==typeof e.fallbackNS&&(e.fallbackNS=[e.fallbackNS]),e.whitelist&&e.whitelist.indexOf("cimode")<0&&e.whitelist.push("cimode"),e}var m={};m["typeof"]="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(e){return typeof e}:function(e){return e&&"function"==typeof Symbol&&e.constructor===Symbol?"symbol":typeof e},m.classCallCheck=function(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")},m["extends"]=Object.assign||function(e){for(var t=1;t<arguments.length;t++){var n=arguments[t];for(var o in n)Object.prototype.hasOwnProperty.call(n,o)&&(e[o]=n[o])}return e},m.inherits=function(e,t){if("function"!=typeof t&&null!==t)throw new TypeError("Super expression must either be null or a function, not "+typeof t);e.prototype=Object.create(t&&t.prototype,{constructor:{value:e,enumerable:!1,writable:!0,configurable:!0}}),t&&(Object.setPrototypeOf?Object.setPrototypeOf(e,t):e.__proto__=t)},m.possibleConstructorReturn=function(e,t){if(!e)throw new ReferenceError("this hasn't been initialised - super() hasn't been called");return!t||"object"!=typeof t&&"function"!=typeof t?e:t},m.slicedToArray=function(){function e(e,t){var n=[],o=!0,r=!1,i=void 0;try{for(var s,a=e[Symbol.iterator]();!(o=(s=a.next()).done)&&(n.push(s.value),!t||n.length!==t);o=!0);}catch(l){r=!0,i=l}finally{try{!o&&a["return"]&&a["return"]()}finally{if(r)throw i}}return n}return function(t,n){if(Array.isArray(t))return t;if(Symbol.iterator in Object(t))return e(t,n);throw new TypeError("Invalid attempt to destructure non-iterable instance")}}();var x={type:"logger",log:function(e){this._output("log",e)},warn:function(e){this._output("warn",e)},error:function(e){this._output("error",e)},_output:function(e,t){console&&console[e]&&console[e].apply(console,Array.prototype.slice.call(t))}},k=function(){function e(t){var n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];m.classCallCheck(this,e),this.subs=[],this.init(t,n)}return e.prototype.init=function(e){var t=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];this.prefix=t.prefix||"i18next:",this.logger=e||x,this.options=t,this.debug=t.debug!==!1},e.prototype.setDebug=function(e){this.debug=e,this.subs.forEach(function(t){t.setDebug(e)})},e.prototype.log=function(){this.forward(arguments,"log","",!0)},e.prototype.warn=function(){this.forward(arguments,"warn","",!0)},e.prototype.error=function(){this.forward(arguments,"error","")},e.prototype.deprecate=function(){this.forward(arguments,"warn","WARNING DEPRECATED: ",!0)},e.prototype.forward=function(e,t,n,o){o&&!this.debug||("string"==typeof e[0]&&(e[0]=n+this.prefix+" "+e[0]),this.logger[t](e))},e.prototype.create=function(t){var n=new e(this.logger,m["extends"]({prefix:this.prefix+":"+t+":"},this.options));return this.subs.push(n),n},e}(),S=new k,w=function(){function e(){m.classCallCheck(this,e),this.observers={}}return e.prototype.on=function(e,t){var n=this;e.split(" ").forEach(function(e){n.observers[e]=n.observers[e]||[],n.observers[e].push(t)})},e.prototype.off=function(e,t){var n=this;this.observers[e]&&this.observers[e].forEach(function(){if(t){var o=n.observers[e].indexOf(t);o>-1&&n.observers[e].splice(o,1)}else delete n.observers[e]})},e.prototype.emit=function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),o=1;t>o;o++)n[o-1]=arguments[o];this.observers[e]&&this.observers[e].forEach(function(e){e.apply(void 0,n)}),this.observers["*"]&&this.observers["*"].forEach(function(t){var o;t.apply(t,(o=[e]).concat.apply(o,n))})},e}(),C={"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;","/":"&#x2F;"},L=function(e){function t(){var n=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],o=arguments.length<=1||void 0===arguments[1]?{ns:["translation"],defaultNS:"translation"}:arguments[1];m.classCallCheck(this,t);var r=m.possibleConstructorReturn(this,e.call(this));return r.data=n,r.options=o,r}return m.inherits(t,e),t.prototype.addNamespaces=function(e){this.options.ns.indexOf(e)<0&&this.options.ns.push(e)},t.prototype.removeNamespaces=function(e){var t=this.options.ns.indexOf(e);t>-1&&this.options.ns.splice(t,1)},t.prototype.getResource=function(e,t,n){var o=arguments.length<=3||void 0===arguments[3]?{}:arguments[3],r=o.keySeparator||this.options.keySeparator;void 0===r&&(r=".");var s=[e,t];return n&&"string"!=typeof n&&(s=s.concat(n)),n&&"string"==typeof n&&(s=s.concat(r?n.split(r):n)),e.indexOf(".")>-1&&(s=e.split(".")),i(this.data,s)},t.prototype.addResource=function(e,t,n,r){var i=arguments.length<=4||void 0===arguments[4]?{silent:!1}:arguments[4],s=this.options.keySeparator;void 0===s&&(s=".");var a=[e,t];n&&(a=a.concat(s?n.split(s):n)),e.indexOf(".")>-1&&(a=e.split("."),r=t,t=a[1]),this.addNamespaces(t),o(this.data,a,r),i.silent||this.emit("added",e,t,n,r)},t.prototype.addResources=function(e,t,n){for(var o in n)"string"==typeof n[o]&&this.addResource(e,t,o,n[o],{silent:!0});this.emit("added",e,t,n)},t.prototype.addResourceBundle=function(e,t,n,r,a){var l=[e,t];e.indexOf(".")>-1&&(l=e.split("."),r=n,n=t,t=l[1]),this.addNamespaces(t);var u=i(this.data,l)||{};r?s(u,n,a):u=m["extends"]({},u,n),o(this.data,l,u),this.emit("added",e,t,n)},t.prototype.removeResourceBundle=function(e,t){this.hasResourceBundle(e,t)&&delete this.data[e][t],this.removeNamespaces(t),this.emit("removed",e,t)},t.prototype.hasResourceBundle=function(e,t){return void 0!==this.getResource(e,t)},t.prototype.getResourceBundle=function(e,t){return t||(t=this.options.defaultNS),"v1"===this.options.compatibilityAPI?m["extends"]({},this.getResource(e,t)):this.getResource(e,t)},t.prototype.toJSON=function(){return this.data},t}(w),N={processors:{},addPostProcessor:function(e){this.processors[e.name]=e},handle:function(e,t,n,o,r){var i=this;return e.forEach(function(e){i.processors[e]&&(t=i.processors[e].process(t,n,o,r))}),t}},O=function(e){function n(o){var r=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];m.classCallCheck(this,n);var i=m.possibleConstructorReturn(this,e.call(this));return t(["resourceStore","languageUtils","pluralResolver","interpolator","backendConnector"],o,i),i.options=r,i.logger=S.create("translator"),i}return m.inherits(n,e),n.prototype.changeLanguage=function(e){e&&(this.language=e)},n.prototype.exists=function(e){var t=arguments.length<=1||void 0===arguments[1]?{interpolation:{}}:arguments[1];return"v1"===this.options.compatibilityAPI&&(t=f(t)),void 0!==this.resolve(e,t)},n.prototype.extractFromKey=function(e,t){var n=t.nsSeparator||this.options.nsSeparator;void 0===n&&(n=":");var o=t.ns||this.options.defaultNS;if(n&&e.indexOf(n)>-1){var r=e.split(n);o=r[0],e=r[1]}return"string"==typeof o&&(o=[o]),{key:e,namespaces:o}},n.prototype.translate=function(e){var t=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];if("object"!==("undefined"==typeof t?"undefined":m["typeof"](t))?t=this.options.overloadTranslationOptionHandler(arguments):"v1"===this.options.compatibilityAPI&&(t=f(t)),void 0===e||null===e||""===e)return"";"number"==typeof e&&(e=String(e)),"string"==typeof e&&(e=[e]);var n=t.lng||this.language;if(n&&"cimode"===n.toLowerCase())return e[e.length-1];var o=t.keySeparator||this.options.keySeparator||".",r=this.extractFromKey(e[e.length-1],t),i=r.key,s=r.namespaces,a=s[s.length-1],l=this.resolve(e,t),u=Object.prototype.toString.apply(l),c=["[object Number]","[object Function]","[object RegExp]"],p=void 0!==t.joinArrays?t.joinArrays:this.options.joinArrays;if(l&&"string"!=typeof l&&c.indexOf(u)<0&&(!p||"[object Array]"!==u)){if(!t.returnObjects&&!this.options.returnObjects)return this.logger.warn("accessing an object - but returnObjects options is not enabled!"),this.options.returnedObjectHandler?this.options.returnedObjectHandler(i,l,t):"key '"+i+" ("+this.language+")' returned an object instead of string.";var h="[object Array]"===u?[]:{};for(var g in l)h[g]=this.translate(""+i+o+g,m["extends"]({joinArrays:!1,ns:s},t));l=h}else if(p&&"[object Array]"===u)l=l.join(p),l&&(l=this.extendTranslation(l,i,t));else{var d=!1,v=!1;if(!this.isValidLookup(l)&&t.defaultValue&&(d=!0,l=t.defaultValue),this.isValidLookup(l)||(v=!0,l=i),(v||d)&&(this.logger.log("missingKey",n,a,i,l),this.options.saveMissing)){var y=[];if("fallback"===this.options.saveMissingTo&&this.options.fallbackLng&&this.options.fallbackLng[0])for(var b=0;b<this.options.fallbackLng.length;b++)y.push(this.options.fallbackLng[b]);else"all"===this.options.saveMissingTo?y=this.languageUtils.toResolveHierarchy(t.lng||this.language):y.push(t.lng||this.language);this.options.missingKeyHandler?this.options.missingKeyHandler(y,a,i,l):this.backendConnector&&this.backendConnector.saveMissing&&this.backendConnector.saveMissing(y,a,i,l),this.emit("missingKey",y,a,i,l)}l=this.extendTranslation(l,i,t),v&&l===i&&this.options.appendNamespaceToMissingKey&&(l=a+":"+i),v&&this.options.parseMissingKeyHandler&&(l=this.options.parseMissingKeyHandler(l))}return l},n.prototype.extendTranslation=function(e,t,n){var o=this;n.interpolation&&this.interpolator.init(n);var r=n.replace&&"string"!=typeof n.replace?n.replace:n;this.options.interpolation.defaultVariables&&(r=m["extends"]({},this.options.interpolation.defaultVariables,r)),e=this.interpolator.interpolate(e,r),e=this.interpolator.nest(e,function(){for(var e=arguments.length,t=Array(e),n=0;e>n;n++)t[n]=arguments[n];return o.translate.apply(o,t)},n),n.interpolation&&this.interpolator.reset();var i=n.postProcess||this.options.postProcess,s="string"==typeof i?[i]:i;return void 0!==e&&s&&s.length&&n.applyPostProcessor!==!1&&(e=N.handle(s,e,t,n,this)),e},n.prototype.resolve=function(e){var t=this,n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1],o=void 0;return"string"==typeof e&&(e=[e]),e.forEach(function(e){if(!t.isValidLookup(o)){var r=t.extractFromKey(e,n),i=r.key,s=r.namespaces;t.options.fallbackNS&&(s=s.concat(t.options.fallbackNS));var a=void 0!==n.count&&"string"!=typeof n.count,l=void 0!==n.context&&"string"==typeof n.context&&""!==n.context,u=n.lngs?n.lngs:t.languageUtils.toResolveHierarchy(n.lng||t.language);s.forEach(function(e){t.isValidLookup(o)||u.forEach(function(r){if(!t.isValidLookup(o)){var s=i,u=[s],c=void 0;a&&(c=t.pluralResolver.getSuffix(r,n.count)),a&&l&&u.push(s+c),l&&u.push(s+=""+t.options.contextSeparator+n.context),a&&u.push(s+=c);for(var p=void 0;p=u.pop();)t.isValidLookup(o)||(o=t.getResource(r,e,p,n))}})})}}),o},n.prototype.isValidLookup=function(e){return!(void 0===e||!this.options.returnNull&&null===e||!this.options.returnEmptyString&&""===e)},n.prototype.getResource=function(e,t,n){var o=arguments.length<=3||void 0===arguments[3]?{}:arguments[3];return this.resourceStore.getResource(e,t,n,o)},n}(w),j=function(){function e(t){m.classCallCheck(this,e),this.options=t,this.whitelist=this.options.whitelist||!1,this.logger=S.create("languageUtils")}return e.prototype.getLanguagePartFromCode=function(e){if(e.indexOf("-")<0)return e;var t=["NB-NO","NN-NO","nb-NO","nn-NO","nb-no","nn-no"],n=e.split("-");return this.formatLanguageCode(t.indexOf(e)>-1?n[1].toLowerCase():n[0])},e.prototype.formatLanguageCode=function(e){if("string"==typeof e&&e.indexOf("-")>-1){var t=["hans","hant","latn","cyrl","cans","mong","arab"],n=e.split("-");return this.options.lowerCaseLng?n=n.map(function(e){return e.toLowerCase()}):2===n.length?(n[0]=n[0].toLowerCase(),n[1]=n[1].toUpperCase(),t.indexOf(n[1].toLowerCase())>-1&&(n[1]=g(n[1].toLowerCase()))):3===n.length&&(n[0]=n[0].toLowerCase(),2===n[1].length&&(n[1]=n[1].toUpperCase()),"sgn"!==n[0]&&2===n[2].length&&(n[2]=n[2].toUpperCase()),t.indexOf(n[1].toLowerCase())>-1&&(n[1]=g(n[1].toLowerCase())),t.indexOf(n[2].toLowerCase())>-1&&(n[2]=g(n[2].toLowerCase()))),n.join("-")}return this.options.cleanCode||this.options.lowerCaseLng?e.toLowerCase():e},e.prototype.isWhitelisted=function(e){return"languageOnly"===this.options.load&&(e=this.getLanguagePartFromCode(e)),!this.whitelist||!this.whitelist.length||this.whitelist.indexOf(e)>-1},e.prototype.toResolveHierarchy=function(e,t){var n=this;t=t||this.options.fallbackLng||[],"string"==typeof t&&(t=[t]);var o=[],r=function(e){n.isWhitelisted(e)?o.push(e):n.logger.warn("rejecting non-whitelisted language code: "+e)};return"string"==typeof e&&e.indexOf("-")>-1?("languageOnly"!==this.options.load&&r(this.formatLanguageCode(e)),"currentOnly"!==this.options.load&&r(this.getLanguagePartFromCode(e))):"string"==typeof e&&r(this.formatLanguageCode(e)),t.forEach(function(e){o.indexOf(e)<0&&r(n.formatLanguageCode(e))}),o},e}(),R=[{lngs:["ach","ak","am","arn","br","fil","gun","ln","mfe","mg","mi","oc","tg","ti","tr","uz","wa"],nr:[1,2],fc:1},{lngs:["af","an","ast","az","bg","bn","ca","da","de","dev","el","en","eo","es","es_ar","et","eu","fi","fo","fur","fy","gl","gu","ha","he","hi","hu","hy","ia","it","kn","ku","lb","mai","ml","mn","mr","nah","nap","nb","ne","nl","nn","no","nso","pa","pap","pms","ps","pt","pt_br","rm","sco","se","si","so","son","sq","sv","sw","ta","te","tk","ur","yo"],nr:[1,2],fc:2},{lngs:["ay","bo","cgg","fa","id","ja","jbo","ka","kk","km","ko","ky","lo","ms","sah","su","th","tt","ug","vi","wo","zh"],nr:[1],fc:3},{lngs:["be","bs","dz","hr","ru","sr","uk"],nr:[1,2,5],fc:4},{lngs:["ar"],nr:[0,1,2,3,11,100],fc:5},{lngs:["cs","sk"],nr:[1,2,5],fc:6},{lngs:["csb","pl"],nr:[1,2,5],fc:7},{lngs:["cy"],nr:[1,2,3,8],fc:8},{lngs:["fr"],nr:[1,2],fc:9},{lngs:["ga"],nr:[1,2,3,7,11],fc:10},{lngs:["gd"],nr:[1,2,3,20],fc:11},{lngs:["is"],nr:[1,2],fc:12},{lngs:["jv"],nr:[0,1],fc:13},{lngs:["kw"],nr:[1,2,3,4],fc:14},{lngs:["lt"],nr:[1,2,10],fc:15},{lngs:["lv"],nr:[1,2,0],fc:16},{lngs:["mk"],nr:[1,2],fc:17},{lngs:["mnk"],nr:[0,1,2],fc:18},{lngs:["mt"],nr:[1,2,11,20],fc:19},{lngs:["or"],nr:[2,1],fc:2},{lngs:["ro"],nr:[1,2,20],fc:20},{lngs:["sl"],nr:[5,1,2,3],fc:21}],P={1:function(e){return Number(e>1)},2:function(e){return Number(1!=e)},3:function(e){return 0},4:function(e){return Number(e%10==1&&e%100!=11?0:e%10>=2&&4>=e%10&&(10>e%100||e%100>=20)?1:2)},5:function(e){return Number(0===e?0:1==e?1:2==e?2:e%100>=3&&10>=e%100?3:e%100>=11?4:5)},6:function(e){return Number(1==e?0:e>=2&&4>=e?1:2)},7:function(e){return Number(1==e?0:e%10>=2&&4>=e%10&&(10>e%100||e%100>=20)?1:2)},8:function(e){return Number(1==e?0:2==e?1:8!=e&&11!=e?2:3)},9:function(e){return Number(e>=2)},10:function(e){return Number(1==e?0:2==e?1:7>e?2:11>e?3:4)},11:function(e){return Number(1==e||11==e?0:2==e||12==e?1:e>2&&20>e?2:3)},12:function(e){return Number(e%10!=1||e%100==11)},13:function(e){return Number(0!==e)},14:function(e){return Number(1==e?0:2==e?1:3==e?2:3)},15:function(e){return Number(e%10==1&&e%100!=11?0:e%10>=2&&(10>e%100||e%100>=20)?1:2)},16:function(e){return Number(e%10==1&&e%100!=11?0:0!==e?1:2)},17:function(e){return Number(1==e||e%10==1?0:1)},18:function(e){return Number(0==e?0:1==e?1:2)},19:function(e){return Number(1==e?0:0===e||e%100>1&&11>e%100?1:e%100>10&&20>e%100?2:3)},20:function(e){return Number(1==e?0:0===e||e%100>0&&20>e%100?1:2)},21:function(e){return Number(e%100==1?1:e%100==2?2:e%100==3||e%100==4?3:0)}},E=function(){function e(t){var n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];m.classCallCheck(this,e),this.languageUtils=t,this.options=n,this.logger=S.create("pluralResolver"),this.rules=d()}return e.prototype.addRule=function(e,t){this.rules[e]=t},e.prototype.getRule=function(e){return this.rules[this.languageUtils.getLanguagePartFromCode(e)]},e.prototype.needsPlural=function(e){var t=this.getRule(e);return!(t&&t.numbers.length<=1)},e.prototype.getSuffix=function(e,t){var n=this.getRule(e);if(n){if(1===n.numbers.length)return"";var o=n.noAbs?n.plurals(t):n.plurals(Math.abs(t)),r=n.numbers[o];if(2===n.numbers.length&&1===n.numbers[0]&&(2===r?r="plural":1===r&&(r="")),"v1"===this.options.compatibilityJSON){if(1===r)return"";if("number"==typeof r)return"_plural_"+r.toString()}return this.options.prepend&&r.toString()?this.options.prepend+r.toString():r.toString()}return this.logger.warn("no plural rule found for: "+e),""},e}(),_=function(){function t(){var e=arguments.length<=0||void 0===arguments[0]?{}:arguments[0];m.classCallCheck(this,t),this.logger=S.create("interpolator"),this.init(e,!0)}return t.prototype.init=function(){var e=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],t=arguments[1];t&&(this.options=e),e.interpolation||(e.interpolation={escapeValue:!0});var n=e.interpolation;this.escapeValue=n.escapeValue,this.prefix=n.prefix?a(n.prefix):n.prefixEscaped||"{{",this.suffix=n.suffix?a(n.suffix):n.suffixEscaped||"}}",this.unescapePrefix=n.unescapeSuffix?"":n.unescapePrefix||"-",this.unescapeSuffix=this.unescapePrefix?"":n.unescapeSuffix||"",this.nestingPrefix=n.nestingPrefix?a(n.nestingPrefix):n.nestingPrefixEscaped||a("$t("),this.nestingSuffix=n.nestingSuffix?a(n.nestingSuffix):n.nestingSuffixEscaped||a(")");var o=this.prefix+"(.+?)"+this.suffix;this.regexp=new RegExp(o,"g");var r=this.prefix+this.unescapePrefix+"(.+?)"+this.unescapeSuffix+this.suffix;this.regexpUnescape=new RegExp(r,"g");var i=this.nestingPrefix+"(.+?)"+this.nestingSuffix;this.nestingRegexp=new RegExp(i,"g")},t.prototype.reset=function(){this.options&&this.init(this.options)},t.prototype.interpolate=function(t,n){function o(e){return e.replace(/\$/g,"$$$$")}for(var r=void 0,s=void 0;r=this.regexpUnescape.exec(t);){var a=i(n,r[1].trim());t=t.replace(r[0],a)}for(;r=this.regexp.exec(t);)s=i(n,r[1].trim()),"string"!=typeof s&&(s=e(s)),s||(this.logger.warn("missed to pass in variable "+r[1]+" for interpolating "+t),s=""),s=o(this.escapeValue?l(s):s),t=t.replace(r[0],s),this.regexp.lastIndex=0;return t},t.prototype.nest=function(t,n){function o(e){return e.replace(/\$/g,"$$$$")}function r(e){if(e.indexOf(",")<0)return e;var t=e.split(",");e=t.shift();var n=t.join(",");n=this.interpolate(n,u);try{u=JSON.parse(n)}catch(o){this.logger.error("failed parsing options string in nesting for key "+e,o)}return e}var i=arguments.length<=2||void 0===arguments[2]?{}:arguments[2],s=void 0,a=void 0,u=JSON.parse(JSON.stringify(i));for(u.applyPostProcessor=!1;s=this.nestingRegexp.exec(t);)a=n(r.call(this,s[1].trim()),u),"string"!=typeof a&&(a=e(a)),a||(this.logger.warn("missed to pass in variable "+s[1]+" for interpolating "+t),a=""),a=o(this.escapeValue?l(a):a),t=t.replace(s[0],a),this.regexp.lastIndex=0;return t},t}(),T=function(e){function t(n,o,r){var i=arguments.length<=3||void 0===arguments[3]?{}:arguments[3];m.classCallCheck(this,t);var s=m.possibleConstructorReturn(this,e.call(this));return s.backend=n,s.store=o,s.services=r,s.options=i,s.logger=S.create("backendConnector"),s.state={},s.queue=[],s.backend&&s.backend.init&&s.backend.init(r,i.backend,i),s}return m.inherits(t,e),t.prototype.queueLoad=function(e,t,n){var o=this,r=[],i=[],s=[],a=[];return e.forEach(function(e){var n=!0;t.forEach(function(t){var s=e+"|"+t;o.store.hasResourceBundle(e,t)?o.state[s]=2:o.state[s]<0||(1===o.state[s]?i.indexOf(s)<0&&i.push(s):(o.state[s]=1,n=!1,i.indexOf(s)<0&&i.push(s),r.indexOf(s)<0&&r.push(s),a.indexOf(t)<0&&a.push(t)))}),n||s.push(e)}),(r.length||i.length)&&this.queue.push({pending:i,loaded:{},errors:[],callback:n}),{toLoad:r,pending:i,toLoadLanguages:s,toLoadNamespaces:a}},t.prototype.loaded=function(e,t,n){var o=this,i=e.split("|"),s=m.slicedToArray(i,2),a=s[0],l=s[1];t&&this.emit("failedLoading",a,l,t),n&&this.store.addResourceBundle(a,l,n),this.state[e]=t?-1:2,this.queue.forEach(function(n){r(n.loaded,[a],l),v(n.pending,e),t&&n.errors.push(t),0!==n.pending.length||n.done||(n.errors.length?n.callback(n.errors):n.callback(),o.emit("loaded",n.loaded),n.done=!0)}),this.queue=this.queue.filter(function(e){return!e.done})},t.prototype.read=function(e,t,n,o,r,i){var s=this;return o||(o=0),r||(r=250),e.length?void this.backend[n](e,t,function(a,l){return a&&l&&5>o?void setTimeout(function(){s.read.call(s,e,t,n,++o,2*r,i)},r):void i(a,l)}):i(null,{})},t.prototype.load=function(e,t,n){var o=this;if(!this.backend)return this.logger.warn("No backend was added via i18next.use. Will not load resources."),n&&n();var r=m["extends"]({},this.backend.options,this.options.backend);"string"==typeof e&&(e=this.services.languageUtils.toResolveHierarchy(e)),"string"==typeof t&&(t=[t]);var s=this.queueLoad(e,t,n);return s.toLoad.length?void(r.allowMultiLoading&&this.backend.readMulti?this.read(s.toLoadLanguages,s.toLoadNamespaces,"readMulti",null,null,function(e,t){e&&o.logger.warn("loading namespaces "+s.toLoadNamespaces.join(", ")+" for languages "+s.toLoadLanguages.join(", ")+" via multiloading failed",e),!e&&t&&o.logger.log("loaded namespaces "+s.toLoadNamespaces.join(", ")+" for languages "+s.toLoadLanguages.join(", ")+" via multiloading",t),s.toLoad.forEach(function(n){var r=n.split("|"),s=m.slicedToArray(r,2),a=s[0],l=s[1],u=i(t,[a,l]);if(u)o.loaded(n,e,u);else{var c="loading namespace "+l+" for language "+a+" via multiloading failed";o.loaded(n,c),o.logger.error(c)}})}):!function(){var e=function(e){var t=this,n=e.split("|"),o=m.slicedToArray(n,2),r=o[0],i=o[1];this.read(r,i,"read",null,null,function(n,o){n&&t.logger.warn("loading namespace "+i+" for language "+r+" failed",n),!n&&o&&t.logger.log("loaded namespace "+i+" for language "+r,o),t.loaded(e,n,o)})};s.toLoad.forEach(function(t){e.call(o,t)})}()):void(s.pending.length||n())},t.prototype.saveMissing=function(e,t,n,o){this.backend&&this.backend.create&&this.backend.create(e,t,n,o),this.store.addResource(e[0],t,n,o)},t}(w),A=function(e){function t(n,o,r){var i=arguments.length<=3||void 0===arguments[3]?{}:arguments[3];m.classCallCheck(this,t);var s=m.possibleConstructorReturn(this,e.call(this));return s.cache=n,s.store=o,s.services=r,s.options=i,s.logger=S.create("cacheConnector"),s.cache&&s.cache.init&&s.cache.init(r,i.cache,i),s}return m.inherits(t,e),t.prototype.load=function(e,t,n){var o=this;if(!this.cache)return n&&n();var r=m["extends"]({},this.cache.options,this.options.cache);"string"==typeof e&&(e=this.services.languageUtils.toResolveHierarchy(e)),"string"==typeof t&&(t=[t]),r.enabled?this.cache.load(e,function(t,r){if(t&&o.logger.error("loading languages "+e.join(", ")+" from cache failed",t),r)for(var i in r)for(var s in r[i])if("i18nStamp"!==s){var a=r[i][s];a&&o.store.addResourceBundle(i,s,a)}n&&n()}):n&&n()},t.prototype.save=function(){this.cache&&this.options.cache&&this.options.cache.enabled&&this.cache.save(this.store.data)},t}(w),M=function(e){function t(){var n=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],o=arguments[1];m.classCallCheck(this,t);var r=m.possibleConstructorReturn(this,e.call(this));return r.options=b(n),r.services={},r.logger=S,r.modules={},o&&!r.isInitialized&&r.init(n,o),r}return m.inherits(t,e),t.prototype.init=function(e,t){function n(e){return e?"function"==typeof e?new e:e:void 0}var o=this;if("function"==typeof e&&(t=e,e={}),e||(e={}),"v1"===e.compatibilityAPI?this.options=m["extends"]({},y(),b(c(e)),{}):"v1"===e.compatibilityJSON?this.options=m["extends"]({},y(),b(p(e)),{}):this.options=m["extends"]({},y(),this.options,b(e)),t||(t=function(){}),!this.options.isClone){this.modules.logger?S.init(n(this.modules.logger),this.options):S.init(null,this.options);var r=new j(this.options);this.store=new L(this.options.resources,this.options);var i=this.services;i.logger=S,i.resourceStore=this.store,i.resourceStore.on("added removed",function(e,t){i.cacheConnector.save()}),i.languageUtils=r,i.pluralResolver=new E(r,{prepend:this.options.pluralSeparator,compatibilityJSON:this.options.compatibilityJSON}),i.interpolator=new _(this.options),i.backendConnector=new T(n(this.modules.backend),i.resourceStore,i,this.options),i.backendConnector.on("*",function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),r=1;t>r;r++)n[r-1]=arguments[r];o.emit.apply(o,[e].concat(n))}),i.backendConnector.on("loaded",function(e){i.cacheConnector.save()}),i.cacheConnector=new A(n(this.modules.cache),i.resourceStore,i,this.options),i.cacheConnector.on("*",function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),r=1;t>r;r++)n[r-1]=arguments[r];o.emit.apply(o,[e].concat(n))}),this.modules.languageDetector&&(i.languageDetector=n(this.modules.languageDetector),i.languageDetector.init(i,this.options.detection,this.options)),this.translator=new O(this.services,this.options),this.translator.on("*",function(e){for(var t=arguments.length,n=Array(t>1?t-1:0),r=1;t>r;r++)n[r-1]=arguments[r];o.emit.apply(o,[e].concat(n))})}var s=["getResource","addResource","addResources","addResourceBundle","removeResourceBundle","hasResourceBundle","getResourceBundle"];s.forEach(function(e){o[e]=function(){return this.store[e].apply(this.store,arguments)}}),"v1"===this.options.compatibilityAPI&&h(this);var a=function(){o.changeLanguage(o.options.lng,function(e,n){o.emit("initialized",o.options),o.logger.log("initialized",o.options),t(e,n)})};return this.options.resources?a():setTimeout(a,10),this},t.prototype.loadResources=function(e){var t=this;if(e||(e=function(){}),this.options.resources)e(null);else{var n=function(){if(t.language&&"cimode"===t.language.toLowerCase())return{v:e()};var n=[],o=function(e){var o=t.services.languageUtils.toResolveHierarchy(e);o.forEach(function(e){n.indexOf(e)<0&&n.push(e)})};o(t.language),t.options.preload&&t.options.preload.forEach(function(e){o(e)}),t.services.cacheConnector.load(n,t.options.ns,function(){t.services.backendConnector.load(n,t.options.ns,e)})}();if("object"===("undefined"==typeof n?"undefined":m["typeof"](n)))return n.v}},t.prototype.use=function(e){return"backend"===e.type&&(this.modules.backend=e),"cache"===e.type&&(this.modules.cache=e),("logger"===e.type||e.log&&e.warn&&e.warn)&&(this.modules.logger=e),"languageDetector"===e.type&&(this.modules.languageDetector=e),"postProcessor"===e.type&&N.addPostProcessor(e),this},t.prototype.changeLanguage=function(e,t){var n=this,o=function(o){e&&(n.emit("languageChanged",e),n.logger.log("languageChanged",e)),t&&t(o,function(){for(var e=arguments.length,t=Array(e),o=0;e>o;o++)t[o]=arguments[o];return n.t.apply(n,t)})};!e&&this.services.languageDetector&&(e=this.services.languageDetector.detect()),e&&(this.language=e,this.languages=this.services.languageUtils.toResolveHierarchy(e),this.translator.changeLanguage(e),this.services.languageDetector&&this.services.languageDetector.cacheUserLanguage(e)),this.loadResources(function(e){o(e)})},t.prototype.getFixedT=function(e,t){var n=this,o=function r(e,t){return t=t||{},t.lng=t.lng||r.lng,t.ns=t.ns||r.ns,n.t(e,t)};return o.lng=e,o.ns=t,o},t.prototype.t=function(){return this.translator&&this.translator.translate.apply(this.translator,arguments)},t.prototype.exists=function(){return this.translator&&this.translator.exists.apply(this.translator,arguments)},t.prototype.setDefaultNamespace=function(e){this.options.defaultNS=e},t.prototype.loadNamespaces=function(e,t){var n=this;return this.options.ns?("string"==typeof e&&(e=[e]),e.forEach(function(e){n.options.ns.indexOf(e)<0&&n.options.ns.push(e)}),void this.loadResources(t)):t&&t()},t.prototype.loadLanguages=function(e,t){"string"==typeof e&&(e=[e]);var n=this.options.preload||[],o=e.filter(function(e){return n.indexOf(e)<0});return o.length?(this.options.preload=n.concat(o),
void this.loadResources(t)):t()},t.prototype.dir=function(e){e||(e=this.language);var t=["ar","shu","sqr","ssh","xaa","yhd","yud","aao","abh","abv","acm","acq","acw","acx","acy","adf","ads","aeb","aec","afb","ajp","apc","apd","arb","arq","ars","ary","arz","auz","avl","ayh","ayl","ayn","ayp","bbz","pga","he","iw","ps","pbt","pbu","pst","prp","prd","ur","ydd","yds","yih","ji","yi","hbo","men","xmn","fa","jpr","peo","pes","prs","dv","sam"];return t.indexOf(this.services.languageUtils.getLanguagePartFromCode(e))?"ltr":"rtl"},t.prototype.createInstance=function(){var e=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],n=arguments[1];return new t(e,n)},t.prototype.cloneInstance=function(){var e=this,n=arguments.length<=0||void 0===arguments[0]?{}:arguments[0],o=arguments[1],r=new t(m["extends"]({},n,this.options,{isClone:!0}),o),i=["store","translator","services","language"];return i.forEach(function(t){r[t]=e[t]}),r},t}(w),H=new M;return H});

/***/ }),
/* 99 */
/***/ (function(module, exports, __webpack_require__) {

!function(e,t){ true?module.exports=t():"function"==typeof define&&define.amd?define("i18nextXHRBackend",t):e.i18nextXHRBackend=t()}(this,function(){"use strict";function e(e){return a.call(r.call(arguments,1),function(t){if(t)for(var n in t)void 0===e[n]&&(e[n]=t[n])}),e}function t(e,t,n,i,a){if(i&&"object"===("undefined"==typeof i?"undefined":o["typeof"](i))){var r="",s=encodeURIComponent;for(var l in i)r+="&"+s(l)+"="+s(i[l]);i=r.slice(1)+(a?"":"&_t="+new Date)}try{var c=new(XMLHttpRequest||ActiveXObject)("MSXML2.XMLHTTP.3.0");c.open(i?"POST":"GET",e,1),t.crossDomain||c.setRequestHeader("X-Requested-With","XMLHttpRequest"),c.setRequestHeader("Content-type","application/x-www-form-urlencoded"),c.onreadystatechange=function(){c.readyState>3&&n&&n(c.responseText,c)},c.send(i)}catch(s){window.console&&console.log(s)}}function n(){return{loadPath:"/locales/{{lng}}/{{ns}}.json",addPath:"locales/add/{{lng}}/{{ns}}",allowMultiLoading:!1,parse:JSON.parse,crossDomain:!1,ajax:t}}var o={};o["typeof"]="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(e){return typeof e}:function(e){return e&&"function"==typeof Symbol&&e.constructor===Symbol?"symbol":typeof e},o.classCallCheck=function(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")},o.createClass=function(){function e(e,t){for(var n=0;n<t.length;n++){var o=t[n];o.enumerable=o.enumerable||!1,o.configurable=!0,"value"in o&&(o.writable=!0),Object.defineProperty(e,o.key,o)}}return function(t,n,o){return n&&e(t.prototype,n),o&&e(t,o),t}}();var i=[],a=i.forEach,r=i.slice,s=function(){function t(e){var n=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];o.classCallCheck(this,t),this.init(e,n),this.type="backend"}return o.createClass(t,[{key:"init",value:function(t){var o=arguments.length<=1||void 0===arguments[1]?{}:arguments[1];this.services=t,this.options=e(o,this.options||{},n())}},{key:"readMulti",value:function(e,t,n){var o=this.services.interpolator.interpolate(this.options.loadPath,{lng:e.join("+"),ns:t.join("+")});this.loadUrl(o,n)}},{key:"read",value:function(e,t,n){var o=this.services.interpolator.interpolate(this.options.loadPath,{lng:e,ns:t});this.loadUrl(o,n)}},{key:"loadUrl",value:function(e,t){var n=this;this.options.ajax(e,this.options,function(o,i){var a=i.status.toString();if(0===a.indexOf("5"))return t("failed loading "+e,!0);if(0===a.indexOf("4"))return t("failed loading "+e,!1);var r=void 0,s=void 0;try{r=n.options.parse(o)}catch(l){s="failed parsing "+e+" to json"}return s?t(s,!1):void t(null,r)})}},{key:"create",value:function(e,t,n,o){var i=this;"string"==typeof e&&(e=[e]);var a={};a[n]=o||"",e.forEach(function(e){var n=i.services.interpolator.interpolate(i.options.addPath,{lng:e,ns:t});i.options.ajax(n,i.options,function(e,t){},a)})}}]),t}();return s.type="backend",s});

/***/ }),
/* 100 */
/***/ (function(module, exports, __webpack_require__) {

!function(t,e){ true?module.exports=e():"function"==typeof define&&define.amd?define("jqueryI18next",e):t.jqueryI18next=e()}(this,function(){"use strict";function t(t,a){function r(n,a,r){function i(t,n){return s.parseDefaultValueFromContent?e["extends"]({},t,{defaultValue:n}):t}if(0!==a.length){var o="text";if(0===a.indexOf("[")){var f=a.split("]");a=f[1],o=f[0].substr(1,f[0].length-1)}if(a.indexOf(";")===a.length-1&&(a=a.substr(0,a.length-2)),"html"===o)n.html(t.t(a,i(r,n.html())));else if("text"===o)n.text(t.t(a,i(r,n.text())));else if("prepend"===o)n.prepend(t.t(a,i(r,n.html())));else if("append"===o)n.append(t.t(a,i(r,n.html())));else if(0===o.indexOf("data-")){var l=o.substr("data-".length),d=t.t(a,i(r,n.data(l)));n.data(l,d),n.attr(o,d)}else n.attr(o,t.t(a,i(r,n.attr(o))))}}function i(t,n){var i=t.attr(s.selectorAttr);if(i||"undefined"==typeof i||i===!1||(i=t.text()||t.val()),i){var o=t,f=t.data(s.targetAttr);if(f&&(o=t.find(f)||t),n||s.useOptionsAttr!==!0||(n=t.data(s.optionsAttr)),n=n||{},i.indexOf(";")>=0){var l=i.split(";");a.each(l,function(t,e){""!==e&&r(o,e,n)})}else r(o,i,n);if(s.useOptionsAttr===!0){var d={};d=e["extends"]({clone:d},n),delete d.lng,t.data(s.optionsAttr,d)}}}function o(t){return this.each(function(){i(a(this),t);var e=a(this).find("["+s.selectorAttr+"]");e.each(function(){i(a(this),t)})})}var s=arguments.length<=2||void 0===arguments[2]?{}:arguments[2];s=e["extends"]({},n,s),a[s.tName]=t.t.bind(t),a[s.i18nName]=t,a.fn[s.handleName]=o}var e={};e["extends"]=Object.assign||function(t){for(var e=1;e<arguments.length;e++){var n=arguments[e];for(var a in n)Object.prototype.hasOwnProperty.call(n,a)&&(t[a]=n[a])}return t};var n={tName:"t",i18nName:"i18n",handleName:"localize",selectorAttr:"data-i18n",targetAttr:"i18n-target",optionsAttr:"i18n-options",useOptionsAttr:!1,parseDefaultValueFromContent:!0},a={init:t};return a});

/***/ }),
/* 101 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var api_1 = __webpack_require__(4);
var build_toolbars_1 = __webpack_require__(12);
var sxc_1 = __webpack_require__(6);
var log_1 = __webpack_require__(7);
var log_utils_1 = __webpack_require__(102);
var quick_dialog_1 = __webpack_require__(22);
var QuickEditState = __webpack_require__(43);
var window_in_page_1 = __webpack_require__(1);
var DebugConfig_1 = __webpack_require__(17);
var tag_toolbar_1 = __webpack_require__(36);
/**
 * module & toolbar bootstrapping (initialize all toolbars after loading page)
 * this will run onReady...
 */
var initializedInstances = [];
var openedTemplatePickerOnce = false;
var diagCancelStateOnStart = QuickEditState.cancelled.get();
$(document).ready(function () {
    // reset cancelled state after one reload
    if (diagCancelStateOnStart)
        QuickEditState.cancelled.remove();
    // initialize all modules
    initAllInstances(true);
    // start observing the body for configured mutations
    watchDomChanges();
});
/**
 * Scan all instances and initialize them
 * @param isFirstRun should be true only on the very initial call
 */
function initAllInstances(isFirstRun) {
    window_in_page_1.windowInPage.$2sxc.stats.watchDomChanges++;
    $('div[data-edit-context]').each(function () { initInstance(this, isFirstRun); });
    if (isFirstRun)
        tryShowTemplatePicker();
}
/**
 * create an observer instance and start observing
 */
function watchDomChanges() {
    var observer = new MutationObserver(function () { return initAllInstances(false); });
    observer.observe(document.body, { attributes: false, childList: true, subtree: true });
}
/**
 * Show the template picker if
 * - template picker has not yet been opened
 * - dialog has not been cancelled
 * - only one uninitialized module on page
 * @returns
 */
function tryShowTemplatePicker() {
    var sxc = undefined;
    // first check if we should show one according to the state-settings
    var openDialogId = QuickEditState.cbId.get();
    if (openDialogId) {
        // must check if it's on this page, as it could be from another page
        var found = $("[data-cb-id=\"" + openDialogId + "\"]");
        if (found.length)
            sxc = window_in_page_1.windowInPage.$2sxc(openDialogId);
    }
    if (!sxc) {
        var uninitializedModules = $('.sc-uninitialized');
        if (diagCancelStateOnStart || openedTemplatePickerOnce)
            return false;
        // already showing a dialog
        if (quick_dialog_1.quickDialog.isVisible())
            return false;
        // not exactly one uninitialized module
        if (uninitializedModules.length !== 1)
            return false;
        // show the template picker of this module
        var module = uninitializedModules.parent('div[data-edit-context]')[0];
        sxc = sxc_1.getSxcInstance(module);
    }
    if (sxc) {
        sxc.manage.run('layout');
        openedTemplatePickerOnce = true;
    }
    return true;
}
function initInstance(module, isFirstRun) {
    // check if module is already in the list of initialized modules
    if (initializedInstances.find(function (m) { return m === module; }))
        return;
    // add to modules-list first, in case we run into recursions
    initializedInstances.push(module);
    var sxc = sxc_1.getSxcInstance(module);
    // check if the sxc must be re-created. This is necessary when modules are dynamically changed
    // because the configuration may change, and that is cached otherwise, resulting in toolbars with wrong config
    if (!isFirstRun)
        sxc = sxc.recreate(true);
    // check if we must show the glasses
    // this must always run because it can be added ajax-style
    var wasEmpty = showGlassesButtonIfUninitialized(sxc);
    // Remove orphan tag-toolbars
    if (!isFirstRun)
        tag_toolbar_1.CleanupTagToolbars();
    if (isFirstRun || !wasEmpty) {
        // use a logger for each iteration
        var log = new log_1.Log('Bts.Module');
        build_toolbars_1.buildToolbars(log, module);
        if (DebugConfig_1.DebugConfig.bootstrap.initInstance)
            log_utils_1.LogUtils.logDump(log);
    }
    ;
}
function showGlassesButtonIfUninitialized(sxci) {
    // already initialized
    if (isInitialized(sxci))
        return false;
    // already has a glasses button
    var tag = $(api_1.getTag(sxci));
    if (tag.find('.sc-uninitialized').length !== 0)
        return false;
    // note: title is added on mouseover, as the translation isn't ready at page-load
    var btn = $('<div class="sc-uninitialized" onmouseover="this.title = $2sxc.translate(this.title)" title="InPage.NewElement">'
        + '<div class="icon-sxc-glasses"></div>'
        + '</div>');
    btn.on('click', function () { return sxci.manage.run('layout'); });
    tag.append(btn);
    return true;
}
function isInitialized(sxci) {
    var cg = sxci && sxci.manage && sxci.manage._editContext && sxci.manage._editContext.ContentGroup;
    return (cg && cg.TemplateId !== 0);
}


/***/ }),
/* 102 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var sxc_controller_in_page_1 = __webpack_require__(3);
/**
 * logDump - to write whole log to console if is enabled
 */
var LogUtils = /** @class */ (function () {
    function LogUtils() {
    }
    /**
     * Dump log to console, when debug logging is enabled by url query string parameters
     * @param log
     */
    LogUtils.logDump = function (log) {
        // 'jslog' is additional query string url parameter, to enable log dump (debug=true is required)
        // in the future would support more variations like jslog = toolbar etc.
        var jsLogUrlParam = sxc_controller_in_page_1.$2sxcInPage.urlParams.get('jslog');
        //if ($2sxc.debug.load) {
        //  console.log(log.dump());
        //}
        if (jsLogUrlParam) {
            console.log(log.dump());
        }
    };
    return LogUtils;
}());
exports.LogUtils = LogUtils;


/***/ }),
/* 103 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var CommandDefinition = /** @class */ (function () {
    function CommandDefinition() {
    }
    return CommandDefinition;
}());
exports.CommandDefinition = CommandDefinition;


/***/ }),
/* 104 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var _2sxc_translate_1 = __webpack_require__(9);
/**
 * this enhances the $2sxc client controller with stuff only needed when logged in
 */
// #region contentItem Commands
exports.contentItems = {
    // delete command - try to really delete a content-item
    delete: function (context, itemId, itemGuid, itemTitle) {
        // first show main warning / get ok
        var ok = confirm(_2sxc_translate_1.translate('Delete.Confirm')
            .replace('{id}', String(itemId))
            .replace('{title}', itemTitle));
        if (!ok) {
            return Promise.resolve();
        }
        /**
         * ZoneId and AppId are sent becase of rare, special case that is not default
         * (default is that 2sxc is finding ZoneId and AppId on server side from ModuleId)
         * when we need to delete entity from other app or zone, than current one.
         * TODO: send this params, only when is necesary (value change detection for ZoneId, AppId)
         */
        var params = {
            zoneId: context.app.zoneId,
            appId: context.app.id
        };
        return new Promise(function (resolve, reject) {
            context.sxc.webApi.delete("app-content/any/" + itemGuid, params, null, true)
                .done(function (data, textStatus, jqXHR) {
                if (jqXHR.status === 204 || jqXHR.status === 200) {
                    // resolve the promise with the response text
                    resolve(data);
                }
                else {
                    // check if it's a permission config problem
                    var msgJs = _2sxc_translate_1.translate('Delete.ErrCheckConsole');
                    if (jqXHR.status === 401)
                        alert(_2sxc_translate_1.translate('Delete.ErrPermission') + msgJs);
                    if (jqXHR.status === 400)
                        alert(_2sxc_translate_1.translate('Delete.ErrInUse') + msgJs);
                    // otherwise reject with the status text
                    // which will hopefully be a meaningful error
                    reject(Error(textStatus));
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                reject(Error(errorThrown));
            });
        }).then(function (result) {
            location.reload();
        }).catch(function (error) {
            console.log(error);
        });
    },
};


/***/ }),
/* 105 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var cb_1 = __webpack_require__(44);
var mod_1 = __webpack_require__(45);
var CmdsStrategyFactory = /** @class */ (function () {
    function CmdsStrategyFactory() {
        this.cmds = {};
        this.cmds.cb = new cb_1.Cb();
        this.cmds.mod = new mod_1.Mod();
    }
    CmdsStrategyFactory.prototype.getCmds = function (cliptype) {
        return this.cmds[cliptype];
    };
    CmdsStrategyFactory.prototype.delete = function (clip) {
        return this.cmds[clip.type].delete(clip);
    };
    return CmdsStrategyFactory;
}());
exports.CmdsStrategyFactory = CmdsStrategyFactory;


/***/ }),
/* 106 */
/***/ (function(module, exports, __webpack_require__) {

__webpack_require__(107);
__webpack_require__(98);
__webpack_require__(99);
__webpack_require__(100);
__webpack_require__(111);
__webpack_require__(112);
__webpack_require__(26);
__webpack_require__(0);
__webpack_require__(87);
__webpack_require__(103);
__webpack_require__(86);
__webpack_require__(40);
__webpack_require__(88);
__webpack_require__(113);
__webpack_require__(114);
__webpack_require__(115);
__webpack_require__(116);
__webpack_require__(117);
__webpack_require__(118);
__webpack_require__(119);
__webpack_require__(120);
__webpack_require__(121);
__webpack_require__(122);
__webpack_require__(123);
__webpack_require__(124);
__webpack_require__(125);
__webpack_require__(126);
__webpack_require__(127);
__webpack_require__(128);
__webpack_require__(129);
__webpack_require__(130);
__webpack_require__(131);
__webpack_require__(132);
__webpack_require__(133);
__webpack_require__(134);
__webpack_require__(135);
__webpack_require__(136);
__webpack_require__(137);
__webpack_require__(10);
__webpack_require__(138);
__webpack_require__(139);
__webpack_require__(52);
__webpack_require__(92);
__webpack_require__(140);
__webpack_require__(141);
__webpack_require__(35);
__webpack_require__(142);
__webpack_require__(11);
__webpack_require__(81);
__webpack_require__(143);
__webpack_require__(93);
__webpack_require__(16);
__webpack_require__(18);
__webpack_require__(144);
__webpack_require__(39);
__webpack_require__(63);
__webpack_require__(53);
__webpack_require__(54);
__webpack_require__(55);
__webpack_require__(56);
__webpack_require__(57);
__webpack_require__(60);
__webpack_require__(27);
__webpack_require__(59);
__webpack_require__(61);
__webpack_require__(58);
__webpack_require__(62);
__webpack_require__(5);
__webpack_require__(64);
__webpack_require__(65);
__webpack_require__(69);
__webpack_require__(66);
__webpack_require__(67);
__webpack_require__(145);
__webpack_require__(146);
__webpack_require__(147);
__webpack_require__(148);
__webpack_require__(149);
__webpack_require__(150);
__webpack_require__(151);
__webpack_require__(152);
__webpack_require__(153);
__webpack_require__(17);
__webpack_require__(154);
__webpack_require__(155);
__webpack_require__(104);
__webpack_require__(156);
__webpack_require__(157);
__webpack_require__(158);
__webpack_require__(159);
__webpack_require__(160);
__webpack_require__(161);
__webpack_require__(162);
__webpack_require__(3);
__webpack_require__(1);
__webpack_require__(163);
__webpack_require__(164);
__webpack_require__(72);
__webpack_require__(15);
__webpack_require__(165);
__webpack_require__(102);
__webpack_require__(7);
__webpack_require__(4);
__webpack_require__(91);
__webpack_require__(73);
__webpack_require__(90);
__webpack_require__(89);
__webpack_require__(84);
__webpack_require__(23);
__webpack_require__(68);
__webpack_require__(48);
__webpack_require__(50);
__webpack_require__(47);
__webpack_require__(49);
__webpack_require__(166);
__webpack_require__(51);
__webpack_require__(42);
__webpack_require__(41);
__webpack_require__(167);
__webpack_require__(82);
__webpack_require__(83);
__webpack_require__(22);
__webpack_require__(43);
__webpack_require__(85);
__webpack_require__(168);
__webpack_require__(44);
__webpack_require__(25);
__webpack_require__(105);
__webpack_require__(169);
__webpack_require__(79);
__webpack_require__(170);
__webpack_require__(80);
__webpack_require__(171);
__webpack_require__(172);
__webpack_require__(46);
__webpack_require__(45);
__webpack_require__(173);
__webpack_require__(38);
__webpack_require__(2);
__webpack_require__(8);
__webpack_require__(174);
__webpack_require__(175);
__webpack_require__(37);
__webpack_require__(24);
__webpack_require__(95);
__webpack_require__(32);
__webpack_require__(71);
__webpack_require__(74);
__webpack_require__(30);
__webpack_require__(31);
__webpack_require__(12);
__webpack_require__(20);
__webpack_require__(21);
__webpack_require__(176);
__webpack_require__(14);
__webpack_require__(75);
__webpack_require__(177);
__webpack_require__(96);
__webpack_require__(178);
__webpack_require__(19);
__webpack_require__(70);
__webpack_require__(28);
__webpack_require__(13);
__webpack_require__(179);
__webpack_require__(36);
__webpack_require__(180);
__webpack_require__(181);
__webpack_require__(94);
__webpack_require__(182);
__webpack_require__(77);
__webpack_require__(78);
__webpack_require__(183);
__webpack_require__(34);
__webpack_require__(76);
__webpack_require__(29);
__webpack_require__(33);
__webpack_require__(97);
__webpack_require__(9);
__webpack_require__(101);
module.exports = __webpack_require__(6);


/***/ }),
/* 107 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
__webpack_require__(47);
var sxc_controller_in_page_1 = __webpack_require__(3);
var window_in_page_1 = __webpack_require__(1);
var commands_1 = __webpack_require__(10);
var Cms_1 = __webpack_require__(26);
var context_1 = __webpack_require__(5);
var manage_1 = __webpack_require__(90);
var quick_e_1 = __webpack_require__(2);
var start_1 = __webpack_require__(37);
var _2sxc__translateInit_1 = __webpack_require__(97);
var _2sxc_translate_1 = __webpack_require__(9);
__webpack_require__(101);
sxc_controller_in_page_1.$2sxcInPage.context = context_1.context; // primary API to get the context
sxc_controller_in_page_1.$2sxcInPage._translateInit = _2sxc__translateInit_1._translateInit; // reference in ./2sxc-api/js/ToSic.Sxc.Instance.ts
sxc_controller_in_page_1.$2sxcInPage.translate = _2sxc_translate_1.translate; // provide an official translate API for 2sxc
sxc_controller_in_page_1.$2sxcInPage._commands = commands_1.Commands.getInstance();
sxc_controller_in_page_1.$2sxcInPage._manage = manage_1._manage; // used out of this project in ToSic.Sxc.Instance and 2sxc.api.js
window_in_page_1.windowInPage.$quickE = quick_e_1.$quickE;
$(start_1.start); // run on-load
sxc_controller_in_page_1.$2sxcInPage.cms = new Cms_1.Cms();


/***/ }),
/* 108 */
/***/ (function(module, exports, __webpack_require__) {

/* WEBPACK VAR INJECTION */(function(process, global) {/*!
 * @overview es6-promise - a tiny implementation of Promises/A+.
 * @copyright Copyright (c) 2014 Yehuda Katz, Tom Dale, Stefan Penner and contributors (Conversion to ES6 API by Jake Archibald)
 * @license   Licensed under MIT license
 *            See https://raw.githubusercontent.com/stefanpenner/es6-promise/master/LICENSE
 * @version   v4.2.4+314e4831
 */

(function (global, factory) {
	 true ? module.exports = factory() :
	typeof define === 'function' && define.amd ? define(factory) :
	(global.ES6Promise = factory());
}(this, (function () { 'use strict';

function objectOrFunction(x) {
  var type = typeof x;
  return x !== null && (type === 'object' || type === 'function');
}

function isFunction(x) {
  return typeof x === 'function';
}



var _isArray = void 0;
if (Array.isArray) {
  _isArray = Array.isArray;
} else {
  _isArray = function (x) {
    return Object.prototype.toString.call(x) === '[object Array]';
  };
}

var isArray = _isArray;

var len = 0;
var vertxNext = void 0;
var customSchedulerFn = void 0;

var asap = function asap(callback, arg) {
  queue[len] = callback;
  queue[len + 1] = arg;
  len += 2;
  if (len === 2) {
    // If len is 2, that means that we need to schedule an async flush.
    // If additional callbacks are queued before the queue is flushed, they
    // will be processed by this flush that we are scheduling.
    if (customSchedulerFn) {
      customSchedulerFn(flush);
    } else {
      scheduleFlush();
    }
  }
};

function setScheduler(scheduleFn) {
  customSchedulerFn = scheduleFn;
}

function setAsap(asapFn) {
  asap = asapFn;
}

var browserWindow = typeof window !== 'undefined' ? window : undefined;
var browserGlobal = browserWindow || {};
var BrowserMutationObserver = browserGlobal.MutationObserver || browserGlobal.WebKitMutationObserver;
var isNode = typeof self === 'undefined' && typeof process !== 'undefined' && {}.toString.call(process) === '[object process]';

// test for web worker but not in IE10
var isWorker = typeof Uint8ClampedArray !== 'undefined' && typeof importScripts !== 'undefined' && typeof MessageChannel !== 'undefined';

// node
function useNextTick() {
  // node version 0.10.x displays a deprecation warning when nextTick is used recursively
  // see https://github.com/cujojs/when/issues/410 for details
  return function () {
    return process.nextTick(flush);
  };
}

// vertx
function useVertxTimer() {
  if (typeof vertxNext !== 'undefined') {
    return function () {
      vertxNext(flush);
    };
  }

  return useSetTimeout();
}

function useMutationObserver() {
  var iterations = 0;
  var observer = new BrowserMutationObserver(flush);
  var node = document.createTextNode('');
  observer.observe(node, { characterData: true });

  return function () {
    node.data = iterations = ++iterations % 2;
  };
}

// web worker
function useMessageChannel() {
  var channel = new MessageChannel();
  channel.port1.onmessage = flush;
  return function () {
    return channel.port2.postMessage(0);
  };
}

function useSetTimeout() {
  // Store setTimeout reference so es6-promise will be unaffected by
  // other code modifying setTimeout (like sinon.useFakeTimers())
  var globalSetTimeout = setTimeout;
  return function () {
    return globalSetTimeout(flush, 1);
  };
}

var queue = new Array(1000);
function flush() {
  for (var i = 0; i < len; i += 2) {
    var callback = queue[i];
    var arg = queue[i + 1];

    callback(arg);

    queue[i] = undefined;
    queue[i + 1] = undefined;
  }

  len = 0;
}

function attemptVertx() {
  try {
    var vertx = Function('return this')().require('vertx');
    vertxNext = vertx.runOnLoop || vertx.runOnContext;
    return useVertxTimer();
  } catch (e) {
    return useSetTimeout();
  }
}

var scheduleFlush = void 0;
// Decide what async method to use to triggering processing of queued callbacks:
if (isNode) {
  scheduleFlush = useNextTick();
} else if (BrowserMutationObserver) {
  scheduleFlush = useMutationObserver();
} else if (isWorker) {
  scheduleFlush = useMessageChannel();
} else if (browserWindow === undefined && "function" === 'function') {
  scheduleFlush = attemptVertx();
} else {
  scheduleFlush = useSetTimeout();
}

function then(onFulfillment, onRejection) {
  var parent = this;

  var child = new this.constructor(noop);

  if (child[PROMISE_ID] === undefined) {
    makePromise(child);
  }

  var _state = parent._state;


  if (_state) {
    var callback = arguments[_state - 1];
    asap(function () {
      return invokeCallback(_state, child, callback, parent._result);
    });
  } else {
    subscribe(parent, child, onFulfillment, onRejection);
  }

  return child;
}

/**
  `Promise.resolve` returns a promise that will become resolved with the
  passed `value`. It is shorthand for the following:

  ```javascript
  let promise = new Promise(function(resolve, reject){
    resolve(1);
  });

  promise.then(function(value){
    // value === 1
  });
  ```

  Instead of writing the above, your code now simply becomes the following:

  ```javascript
  let promise = Promise.resolve(1);

  promise.then(function(value){
    // value === 1
  });
  ```

  @method resolve
  @static
  @param {Any} value value that the returned promise will be resolved with
  Useful for tooling.
  @return {Promise} a promise that will become fulfilled with the given
  `value`
*/
function resolve$1(object) {
  /*jshint validthis:true */
  var Constructor = this;

  if (object && typeof object === 'object' && object.constructor === Constructor) {
    return object;
  }

  var promise = new Constructor(noop);
  resolve(promise, object);
  return promise;
}

var PROMISE_ID = Math.random().toString(36).substring(2);

function noop() {}

var PENDING = void 0;
var FULFILLED = 1;
var REJECTED = 2;

var TRY_CATCH_ERROR = { error: null };

function selfFulfillment() {
  return new TypeError("You cannot resolve a promise with itself");
}

function cannotReturnOwn() {
  return new TypeError('A promises callback cannot return that same promise.');
}

function getThen(promise) {
  try {
    return promise.then;
  } catch (error) {
    TRY_CATCH_ERROR.error = error;
    return TRY_CATCH_ERROR;
  }
}

function tryThen(then$$1, value, fulfillmentHandler, rejectionHandler) {
  try {
    then$$1.call(value, fulfillmentHandler, rejectionHandler);
  } catch (e) {
    return e;
  }
}

function handleForeignThenable(promise, thenable, then$$1) {
  asap(function (promise) {
    var sealed = false;
    var error = tryThen(then$$1, thenable, function (value) {
      if (sealed) {
        return;
      }
      sealed = true;
      if (thenable !== value) {
        resolve(promise, value);
      } else {
        fulfill(promise, value);
      }
    }, function (reason) {
      if (sealed) {
        return;
      }
      sealed = true;

      reject(promise, reason);
    }, 'Settle: ' + (promise._label || ' unknown promise'));

    if (!sealed && error) {
      sealed = true;
      reject(promise, error);
    }
  }, promise);
}

function handleOwnThenable(promise, thenable) {
  if (thenable._state === FULFILLED) {
    fulfill(promise, thenable._result);
  } else if (thenable._state === REJECTED) {
    reject(promise, thenable._result);
  } else {
    subscribe(thenable, undefined, function (value) {
      return resolve(promise, value);
    }, function (reason) {
      return reject(promise, reason);
    });
  }
}

function handleMaybeThenable(promise, maybeThenable, then$$1) {
  if (maybeThenable.constructor === promise.constructor && then$$1 === then && maybeThenable.constructor.resolve === resolve$1) {
    handleOwnThenable(promise, maybeThenable);
  } else {
    if (then$$1 === TRY_CATCH_ERROR) {
      reject(promise, TRY_CATCH_ERROR.error);
      TRY_CATCH_ERROR.error = null;
    } else if (then$$1 === undefined) {
      fulfill(promise, maybeThenable);
    } else if (isFunction(then$$1)) {
      handleForeignThenable(promise, maybeThenable, then$$1);
    } else {
      fulfill(promise, maybeThenable);
    }
  }
}

function resolve(promise, value) {
  if (promise === value) {
    reject(promise, selfFulfillment());
  } else if (objectOrFunction(value)) {
    handleMaybeThenable(promise, value, getThen(value));
  } else {
    fulfill(promise, value);
  }
}

function publishRejection(promise) {
  if (promise._onerror) {
    promise._onerror(promise._result);
  }

  publish(promise);
}

function fulfill(promise, value) {
  if (promise._state !== PENDING) {
    return;
  }

  promise._result = value;
  promise._state = FULFILLED;

  if (promise._subscribers.length !== 0) {
    asap(publish, promise);
  }
}

function reject(promise, reason) {
  if (promise._state !== PENDING) {
    return;
  }
  promise._state = REJECTED;
  promise._result = reason;

  asap(publishRejection, promise);
}

function subscribe(parent, child, onFulfillment, onRejection) {
  var _subscribers = parent._subscribers;
  var length = _subscribers.length;


  parent._onerror = null;

  _subscribers[length] = child;
  _subscribers[length + FULFILLED] = onFulfillment;
  _subscribers[length + REJECTED] = onRejection;

  if (length === 0 && parent._state) {
    asap(publish, parent);
  }
}

function publish(promise) {
  var subscribers = promise._subscribers;
  var settled = promise._state;

  if (subscribers.length === 0) {
    return;
  }

  var child = void 0,
      callback = void 0,
      detail = promise._result;

  for (var i = 0; i < subscribers.length; i += 3) {
    child = subscribers[i];
    callback = subscribers[i + settled];

    if (child) {
      invokeCallback(settled, child, callback, detail);
    } else {
      callback(detail);
    }
  }

  promise._subscribers.length = 0;
}

function tryCatch(callback, detail) {
  try {
    return callback(detail);
  } catch (e) {
    TRY_CATCH_ERROR.error = e;
    return TRY_CATCH_ERROR;
  }
}

function invokeCallback(settled, promise, callback, detail) {
  var hasCallback = isFunction(callback),
      value = void 0,
      error = void 0,
      succeeded = void 0,
      failed = void 0;

  if (hasCallback) {
    value = tryCatch(callback, detail);

    if (value === TRY_CATCH_ERROR) {
      failed = true;
      error = value.error;
      value.error = null;
    } else {
      succeeded = true;
    }

    if (promise === value) {
      reject(promise, cannotReturnOwn());
      return;
    }
  } else {
    value = detail;
    succeeded = true;
  }

  if (promise._state !== PENDING) {
    // noop
  } else if (hasCallback && succeeded) {
    resolve(promise, value);
  } else if (failed) {
    reject(promise, error);
  } else if (settled === FULFILLED) {
    fulfill(promise, value);
  } else if (settled === REJECTED) {
    reject(promise, value);
  }
}

function initializePromise(promise, resolver) {
  try {
    resolver(function resolvePromise(value) {
      resolve(promise, value);
    }, function rejectPromise(reason) {
      reject(promise, reason);
    });
  } catch (e) {
    reject(promise, e);
  }
}

var id = 0;
function nextId() {
  return id++;
}

function makePromise(promise) {
  promise[PROMISE_ID] = id++;
  promise._state = undefined;
  promise._result = undefined;
  promise._subscribers = [];
}

function validationError() {
  return new Error('Array Methods must be provided an Array');
}

var Enumerator = function () {
  function Enumerator(Constructor, input) {
    this._instanceConstructor = Constructor;
    this.promise = new Constructor(noop);

    if (!this.promise[PROMISE_ID]) {
      makePromise(this.promise);
    }

    if (isArray(input)) {
      this.length = input.length;
      this._remaining = input.length;

      this._result = new Array(this.length);

      if (this.length === 0) {
        fulfill(this.promise, this._result);
      } else {
        this.length = this.length || 0;
        this._enumerate(input);
        if (this._remaining === 0) {
          fulfill(this.promise, this._result);
        }
      }
    } else {
      reject(this.promise, validationError());
    }
  }

  Enumerator.prototype._enumerate = function _enumerate(input) {
    for (var i = 0; this._state === PENDING && i < input.length; i++) {
      this._eachEntry(input[i], i);
    }
  };

  Enumerator.prototype._eachEntry = function _eachEntry(entry, i) {
    var c = this._instanceConstructor;
    var resolve$$1 = c.resolve;


    if (resolve$$1 === resolve$1) {
      var _then = getThen(entry);

      if (_then === then && entry._state !== PENDING) {
        this._settledAt(entry._state, i, entry._result);
      } else if (typeof _then !== 'function') {
        this._remaining--;
        this._result[i] = entry;
      } else if (c === Promise$2) {
        var promise = new c(noop);
        handleMaybeThenable(promise, entry, _then);
        this._willSettleAt(promise, i);
      } else {
        this._willSettleAt(new c(function (resolve$$1) {
          return resolve$$1(entry);
        }), i);
      }
    } else {
      this._willSettleAt(resolve$$1(entry), i);
    }
  };

  Enumerator.prototype._settledAt = function _settledAt(state, i, value) {
    var promise = this.promise;


    if (promise._state === PENDING) {
      this._remaining--;

      if (state === REJECTED) {
        reject(promise, value);
      } else {
        this._result[i] = value;
      }
    }

    if (this._remaining === 0) {
      fulfill(promise, this._result);
    }
  };

  Enumerator.prototype._willSettleAt = function _willSettleAt(promise, i) {
    var enumerator = this;

    subscribe(promise, undefined, function (value) {
      return enumerator._settledAt(FULFILLED, i, value);
    }, function (reason) {
      return enumerator._settledAt(REJECTED, i, reason);
    });
  };

  return Enumerator;
}();

/**
  `Promise.all` accepts an array of promises, and returns a new promise which
  is fulfilled with an array of fulfillment values for the passed promises, or
  rejected with the reason of the first passed promise to be rejected. It casts all
  elements of the passed iterable to promises as it runs this algorithm.

  Example:

  ```javascript
  let promise1 = resolve(1);
  let promise2 = resolve(2);
  let promise3 = resolve(3);
  let promises = [ promise1, promise2, promise3 ];

  Promise.all(promises).then(function(array){
    // The array here would be [ 1, 2, 3 ];
  });
  ```

  If any of the `promises` given to `all` are rejected, the first promise
  that is rejected will be given as an argument to the returned promises's
  rejection handler. For example:

  Example:

  ```javascript
  let promise1 = resolve(1);
  let promise2 = reject(new Error("2"));
  let promise3 = reject(new Error("3"));
  let promises = [ promise1, promise2, promise3 ];

  Promise.all(promises).then(function(array){
    // Code here never runs because there are rejected promises!
  }, function(error) {
    // error.message === "2"
  });
  ```

  @method all
  @static
  @param {Array} entries array of promises
  @param {String} label optional string for labeling the promise.
  Useful for tooling.
  @return {Promise} promise that is fulfilled when all `promises` have been
  fulfilled, or rejected if any of them become rejected.
  @static
*/
function all(entries) {
  return new Enumerator(this, entries).promise;
}

/**
  `Promise.race` returns a new promise which is settled in the same way as the
  first passed promise to settle.

  Example:

  ```javascript
  let promise1 = new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('promise 1');
    }, 200);
  });

  let promise2 = new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('promise 2');
    }, 100);
  });

  Promise.race([promise1, promise2]).then(function(result){
    // result === 'promise 2' because it was resolved before promise1
    // was resolved.
  });
  ```

  `Promise.race` is deterministic in that only the state of the first
  settled promise matters. For example, even if other promises given to the
  `promises` array argument are resolved, but the first settled promise has
  become rejected before the other promises became fulfilled, the returned
  promise will become rejected:

  ```javascript
  let promise1 = new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('promise 1');
    }, 200);
  });

  let promise2 = new Promise(function(resolve, reject){
    setTimeout(function(){
      reject(new Error('promise 2'));
    }, 100);
  });

  Promise.race([promise1, promise2]).then(function(result){
    // Code here never runs
  }, function(reason){
    // reason.message === 'promise 2' because promise 2 became rejected before
    // promise 1 became fulfilled
  });
  ```

  An example real-world use case is implementing timeouts:

  ```javascript
  Promise.race([ajax('foo.json'), timeout(5000)])
  ```

  @method race
  @static
  @param {Array} promises array of promises to observe
  Useful for tooling.
  @return {Promise} a promise which settles in the same way as the first passed
  promise to settle.
*/
function race(entries) {
  /*jshint validthis:true */
  var Constructor = this;

  if (!isArray(entries)) {
    return new Constructor(function (_, reject) {
      return reject(new TypeError('You must pass an array to race.'));
    });
  } else {
    return new Constructor(function (resolve, reject) {
      var length = entries.length;
      for (var i = 0; i < length; i++) {
        Constructor.resolve(entries[i]).then(resolve, reject);
      }
    });
  }
}

/**
  `Promise.reject` returns a promise rejected with the passed `reason`.
  It is shorthand for the following:

  ```javascript
  let promise = new Promise(function(resolve, reject){
    reject(new Error('WHOOPS'));
  });

  promise.then(function(value){
    // Code here doesn't run because the promise is rejected!
  }, function(reason){
    // reason.message === 'WHOOPS'
  });
  ```

  Instead of writing the above, your code now simply becomes the following:

  ```javascript
  let promise = Promise.reject(new Error('WHOOPS'));

  promise.then(function(value){
    // Code here doesn't run because the promise is rejected!
  }, function(reason){
    // reason.message === 'WHOOPS'
  });
  ```

  @method reject
  @static
  @param {Any} reason value that the returned promise will be rejected with.
  Useful for tooling.
  @return {Promise} a promise rejected with the given `reason`.
*/
function reject$1(reason) {
  /*jshint validthis:true */
  var Constructor = this;
  var promise = new Constructor(noop);
  reject(promise, reason);
  return promise;
}

function needsResolver() {
  throw new TypeError('You must pass a resolver function as the first argument to the promise constructor');
}

function needsNew() {
  throw new TypeError("Failed to construct 'Promise': Please use the 'new' operator, this object constructor cannot be called as a function.");
}

/**
  Promise objects represent the eventual result of an asynchronous operation. The
  primary way of interacting with a promise is through its `then` method, which
  registers callbacks to receive either a promise's eventual value or the reason
  why the promise cannot be fulfilled.

  Terminology
  -----------

  - `promise` is an object or function with a `then` method whose behavior conforms to this specification.
  - `thenable` is an object or function that defines a `then` method.
  - `value` is any legal JavaScript value (including undefined, a thenable, or a promise).
  - `exception` is a value that is thrown using the throw statement.
  - `reason` is a value that indicates why a promise was rejected.
  - `settled` the final resting state of a promise, fulfilled or rejected.

  A promise can be in one of three states: pending, fulfilled, or rejected.

  Promises that are fulfilled have a fulfillment value and are in the fulfilled
  state.  Promises that are rejected have a rejection reason and are in the
  rejected state.  A fulfillment value is never a thenable.

  Promises can also be said to *resolve* a value.  If this value is also a
  promise, then the original promise's settled state will match the value's
  settled state.  So a promise that *resolves* a promise that rejects will
  itself reject, and a promise that *resolves* a promise that fulfills will
  itself fulfill.


  Basic Usage:
  ------------

  ```js
  let promise = new Promise(function(resolve, reject) {
    // on success
    resolve(value);

    // on failure
    reject(reason);
  });

  promise.then(function(value) {
    // on fulfillment
  }, function(reason) {
    // on rejection
  });
  ```

  Advanced Usage:
  ---------------

  Promises shine when abstracting away asynchronous interactions such as
  `XMLHttpRequest`s.

  ```js
  function getJSON(url) {
    return new Promise(function(resolve, reject){
      let xhr = new XMLHttpRequest();

      xhr.open('GET', url);
      xhr.onreadystatechange = handler;
      xhr.responseType = 'json';
      xhr.setRequestHeader('Accept', 'application/json');
      xhr.send();

      function handler() {
        if (this.readyState === this.DONE) {
          if (this.status === 200) {
            resolve(this.response);
          } else {
            reject(new Error('getJSON: `' + url + '` failed with status: [' + this.status + ']'));
          }
        }
      };
    });
  }

  getJSON('/posts.json').then(function(json) {
    // on fulfillment
  }, function(reason) {
    // on rejection
  });
  ```

  Unlike callbacks, promises are great composable primitives.

  ```js
  Promise.all([
    getJSON('/posts'),
    getJSON('/comments')
  ]).then(function(values){
    values[0] // => postsJSON
    values[1] // => commentsJSON

    return values;
  });
  ```

  @class Promise
  @param {Function} resolver
  Useful for tooling.
  @constructor
*/

var Promise$2 = function () {
  function Promise(resolver) {
    this[PROMISE_ID] = nextId();
    this._result = this._state = undefined;
    this._subscribers = [];

    if (noop !== resolver) {
      typeof resolver !== 'function' && needsResolver();
      this instanceof Promise ? initializePromise(this, resolver) : needsNew();
    }
  }

  /**
  The primary way of interacting with a promise is through its `then` method,
  which registers callbacks to receive either a promise's eventual value or the
  reason why the promise cannot be fulfilled.
   ```js
  findUser().then(function(user){
    // user is available
  }, function(reason){
    // user is unavailable, and you are given the reason why
  });
  ```
   Chaining
  --------
   The return value of `then` is itself a promise.  This second, 'downstream'
  promise is resolved with the return value of the first promise's fulfillment
  or rejection handler, or rejected if the handler throws an exception.
   ```js
  findUser().then(function (user) {
    return user.name;
  }, function (reason) {
    return 'default name';
  }).then(function (userName) {
    // If `findUser` fulfilled, `userName` will be the user's name, otherwise it
    // will be `'default name'`
  });
   findUser().then(function (user) {
    throw new Error('Found user, but still unhappy');
  }, function (reason) {
    throw new Error('`findUser` rejected and we're unhappy');
  }).then(function (value) {
    // never reached
  }, function (reason) {
    // if `findUser` fulfilled, `reason` will be 'Found user, but still unhappy'.
    // If `findUser` rejected, `reason` will be '`findUser` rejected and we're unhappy'.
  });
  ```
  If the downstream promise does not specify a rejection handler, rejection reasons will be propagated further downstream.
   ```js
  findUser().then(function (user) {
    throw new PedagogicalException('Upstream error');
  }).then(function (value) {
    // never reached
  }).then(function (value) {
    // never reached
  }, function (reason) {
    // The `PedgagocialException` is propagated all the way down to here
  });
  ```
   Assimilation
  ------------
   Sometimes the value you want to propagate to a downstream promise can only be
  retrieved asynchronously. This can be achieved by returning a promise in the
  fulfillment or rejection handler. The downstream promise will then be pending
  until the returned promise is settled. This is called *assimilation*.
   ```js
  findUser().then(function (user) {
    return findCommentsByAuthor(user);
  }).then(function (comments) {
    // The user's comments are now available
  });
  ```
   If the assimliated promise rejects, then the downstream promise will also reject.
   ```js
  findUser().then(function (user) {
    return findCommentsByAuthor(user);
  }).then(function (comments) {
    // If `findCommentsByAuthor` fulfills, we'll have the value here
  }, function (reason) {
    // If `findCommentsByAuthor` rejects, we'll have the reason here
  });
  ```
   Simple Example
  --------------
   Synchronous Example
   ```javascript
  let result;
   try {
    result = findResult();
    // success
  } catch(reason) {
    // failure
  }
  ```
   Errback Example
   ```js
  findResult(function(result, err){
    if (err) {
      // failure
    } else {
      // success
    }
  });
  ```
   Promise Example;
   ```javascript
  findResult().then(function(result){
    // success
  }, function(reason){
    // failure
  });
  ```
   Advanced Example
  --------------
   Synchronous Example
   ```javascript
  let author, books;
   try {
    author = findAuthor();
    books  = findBooksByAuthor(author);
    // success
  } catch(reason) {
    // failure
  }
  ```
   Errback Example
   ```js
   function foundBooks(books) {
   }
   function failure(reason) {
   }
   findAuthor(function(author, err){
    if (err) {
      failure(err);
      // failure
    } else {
      try {
        findBoooksByAuthor(author, function(books, err) {
          if (err) {
            failure(err);
          } else {
            try {
              foundBooks(books);
            } catch(reason) {
              failure(reason);
            }
          }
        });
      } catch(error) {
        failure(err);
      }
      // success
    }
  });
  ```
   Promise Example;
   ```javascript
  findAuthor().
    then(findBooksByAuthor).
    then(function(books){
      // found books
  }).catch(function(reason){
    // something went wrong
  });
  ```
   @method then
  @param {Function} onFulfilled
  @param {Function} onRejected
  Useful for tooling.
  @return {Promise}
  */

  /**
  `catch` is simply sugar for `then(undefined, onRejection)` which makes it the same
  as the catch block of a try/catch statement.
  ```js
  function findAuthor(){
  throw new Error('couldn't find that author');
  }
  // synchronous
  try {
  findAuthor();
  } catch(reason) {
  // something went wrong
  }
  // async with promises
  findAuthor().catch(function(reason){
  // something went wrong
  });
  ```
  @method catch
  @param {Function} onRejection
  Useful for tooling.
  @return {Promise}
  */


  Promise.prototype.catch = function _catch(onRejection) {
    return this.then(null, onRejection);
  };

  /**
    `finally` will be invoked regardless of the promise's fate just as native
    try/catch/finally behaves
  
    Synchronous example:
  
    ```js
    findAuthor() {
      if (Math.random() > 0.5) {
        throw new Error();
      }
      return new Author();
    }
  
    try {
      return findAuthor(); // succeed or fail
    } catch(error) {
      return findOtherAuther();
    } finally {
      // always runs
      // doesn't affect the return value
    }
    ```
  
    Asynchronous example:
  
    ```js
    findAuthor().catch(function(reason){
      return findOtherAuther();
    }).finally(function(){
      // author was either found, or not
    });
    ```
  
    @method finally
    @param {Function} callback
    @return {Promise}
  */


  Promise.prototype.finally = function _finally(callback) {
    var promise = this;
    var constructor = promise.constructor;

    return promise.then(function (value) {
      return constructor.resolve(callback()).then(function () {
        return value;
      });
    }, function (reason) {
      return constructor.resolve(callback()).then(function () {
        throw reason;
      });
    });
  };

  return Promise;
}();

Promise$2.prototype.then = then;
Promise$2.all = all;
Promise$2.race = race;
Promise$2.resolve = resolve$1;
Promise$2.reject = reject$1;
Promise$2._setScheduler = setScheduler;
Promise$2._setAsap = setAsap;
Promise$2._asap = asap;

/*global self*/
function polyfill() {
  var local = void 0;

  if (typeof global !== 'undefined') {
    local = global;
  } else if (typeof self !== 'undefined') {
    local = self;
  } else {
    try {
      local = Function('return this')();
    } catch (e) {
      throw new Error('polyfill failed because global object is unavailable in this environment');
    }
  }

  var P = local.Promise;

  if (P) {
    var promiseToString = null;
    try {
      promiseToString = Object.prototype.toString.call(P.resolve());
    } catch (e) {
      // silently ignored
    }

    if (promiseToString === '[object Promise]' && !P.cast) {
      return;
    }
  }

  local.Promise = Promise$2;
}

// Strange compat..
Promise$2.polyfill = polyfill;
Promise$2.Promise = Promise$2;

Promise$2.polyfill();

return Promise$2;

})));


/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(109), __webpack_require__(110)))

/***/ }),
/* 109 */
/***/ (function(module, exports) {

// shim for using process in browser
var process = module.exports = {};

// cached from whatever global is present so that test runners that stub it
// don't break things.  But we need to wrap it in a try catch in case it is
// wrapped in strict mode code which doesn't define any globals.  It's inside a
// function because try/catches deoptimize in certain engines.

var cachedSetTimeout;
var cachedClearTimeout;

function defaultSetTimout() {
    throw new Error('setTimeout has not been defined');
}
function defaultClearTimeout () {
    throw new Error('clearTimeout has not been defined');
}
(function () {
    try {
        if (typeof setTimeout === 'function') {
            cachedSetTimeout = setTimeout;
        } else {
            cachedSetTimeout = defaultSetTimout;
        }
    } catch (e) {
        cachedSetTimeout = defaultSetTimout;
    }
    try {
        if (typeof clearTimeout === 'function') {
            cachedClearTimeout = clearTimeout;
        } else {
            cachedClearTimeout = defaultClearTimeout;
        }
    } catch (e) {
        cachedClearTimeout = defaultClearTimeout;
    }
} ())
function runTimeout(fun) {
    if (cachedSetTimeout === setTimeout) {
        //normal enviroments in sane situations
        return setTimeout(fun, 0);
    }
    // if setTimeout wasn't available but was latter defined
    if ((cachedSetTimeout === defaultSetTimout || !cachedSetTimeout) && setTimeout) {
        cachedSetTimeout = setTimeout;
        return setTimeout(fun, 0);
    }
    try {
        // when when somebody has screwed with setTimeout but no I.E. maddness
        return cachedSetTimeout(fun, 0);
    } catch(e){
        try {
            // When we are in I.E. but the script has been evaled so I.E. doesn't trust the global object when called normally
            return cachedSetTimeout.call(null, fun, 0);
        } catch(e){
            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error
            return cachedSetTimeout.call(this, fun, 0);
        }
    }


}
function runClearTimeout(marker) {
    if (cachedClearTimeout === clearTimeout) {
        //normal enviroments in sane situations
        return clearTimeout(marker);
    }
    // if clearTimeout wasn't available but was latter defined
    if ((cachedClearTimeout === defaultClearTimeout || !cachedClearTimeout) && clearTimeout) {
        cachedClearTimeout = clearTimeout;
        return clearTimeout(marker);
    }
    try {
        // when when somebody has screwed with setTimeout but no I.E. maddness
        return cachedClearTimeout(marker);
    } catch (e){
        try {
            // When we are in I.E. but the script has been evaled so I.E. doesn't  trust the global object when called normally
            return cachedClearTimeout.call(null, marker);
        } catch (e){
            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error.
            // Some versions of I.E. have different rules for clearTimeout vs setTimeout
            return cachedClearTimeout.call(this, marker);
        }
    }



}
var queue = [];
var draining = false;
var currentQueue;
var queueIndex = -1;

function cleanUpNextTick() {
    if (!draining || !currentQueue) {
        return;
    }
    draining = false;
    if (currentQueue.length) {
        queue = currentQueue.concat(queue);
    } else {
        queueIndex = -1;
    }
    if (queue.length) {
        drainQueue();
    }
}

function drainQueue() {
    if (draining) {
        return;
    }
    var timeout = runTimeout(cleanUpNextTick);
    draining = true;

    var len = queue.length;
    while(len) {
        currentQueue = queue;
        queue = [];
        while (++queueIndex < len) {
            if (currentQueue) {
                currentQueue[queueIndex].run();
            }
        }
        queueIndex = -1;
        len = queue.length;
    }
    currentQueue = null;
    draining = false;
    runClearTimeout(timeout);
}

process.nextTick = function (fun) {
    var args = new Array(arguments.length - 1);
    if (arguments.length > 1) {
        for (var i = 1; i < arguments.length; i++) {
            args[i - 1] = arguments[i];
        }
    }
    queue.push(new Item(fun, args));
    if (queue.length === 1 && !draining) {
        runTimeout(drainQueue);
    }
};

// v8 likes predictible objects
function Item(fun, array) {
    this.fun = fun;
    this.array = array;
}
Item.prototype.run = function () {
    this.fun.apply(null, this.array);
};
process.title = 'browser';
process.browser = true;
process.env = {};
process.argv = [];
process.version = ''; // empty string to avoid regexp issues
process.versions = {};

function noop() {}

process.on = noop;
process.addListener = noop;
process.once = noop;
process.off = noop;
process.removeListener = noop;
process.removeAllListeners = noop;
process.emit = noop;
process.prependListener = noop;
process.prependOnceListener = noop;

process.listeners = function (name) { return [] }

process.binding = function (name) {
    throw new Error('process.binding is not supported');
};

process.cwd = function () { return '/' };
process.chdir = function (dir) {
    throw new Error('process.chdir is not supported');
};
process.umask = function() { return 0; };


/***/ }),
/* 110 */
/***/ (function(module, exports) {

var g;

// This works in non-strict mode
g = (function() {
	return this;
})();

try {
	// This works if eval is allowed (see CSP)
	g = g || Function("return this")() || (1,eval)("this");
} catch(e) {
	// This works if the window reference is available
	if(typeof window === "object")
		g = window;
}

// g can still be undefined, but nothing to do about it...
// We return undefined, instead of nothing here, so it's
// easier to handle this case. if(!global) { ...}

module.exports = g;


/***/ }),
/* 111 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var sxc_controller_in_page_1 = __webpack_require__(3);
var window_in_page_1 = __webpack_require__(1);
if (window_in_page_1.windowInPage.$2sxc && !window_in_page_1.windowInPage.$2sxc.consts) {
    sxc_controller_in_page_1.$2sxcInPage.c = sxc_controller_in_page_1.$2sxcInPage.consts = {
        // classes
        cls: {
            scMenu: 'sc-menu',
        },
        // attributes
        attr: {
            toolbar: 'toolbar',
            toolbarData: 'data-toolbar',
            settings: 'settings',
            settingsData: 'data-settings',
        },
        publishAllowed: 'DraftOptional',
    };
    // selectors
    var sel_1 = sxc_controller_in_page_1.$2sxcInPage.c.sel = {};
    // ReSharper disable once UnusedParameter
    Object.keys(sxc_controller_in_page_1.$2sxcInPage.c.cls).forEach(function (key, index) {
        sel_1[key] = "." + sxc_controller_in_page_1.$2sxcInPage.c.cls[key];
    });
    /*
    ToDo: functional programming
    $2sxc.c.sel = Object.entries($2sxc.c.cls).reduce((res, current) => {
        res[entry[0]] = entry[1];
        return t;
    }, {});
    */
}


/***/ }),
/* 112 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var window_in_page_1 = __webpack_require__(1);
/** this enhances the $2sxc client controller with stuff only needed when logged in */
if (window_in_page_1.windowInPage.$2sxc && !window_in_page_1.windowInPage.$2sxc.system) {
    window_in_page_1.windowInPage.$2sxc.system = {
        finishUpgrade: finishUpgrade,
    };
}
// upgrade command - started when an error contains a link to start this
function finishUpgrade(domElement) {
    var mc = window_in_page_1.windowInPage.$2sxc(domElement);
    $.ajax({
        type: 'get',
        url: mc.resolveServiceUrl('view/module/finishinstallation'),
        beforeSend: $.ServicesFramework(mc.id).setModuleHeaders,
    }).success(function () {
        alert('Upgrade ok, restarting the CMS and reloading...');
        location.reload();
    });
    alert('starting upgrade. This could take a few minutes. You\'ll see an \'ok\' when it\'s done. Please wait...');
}


/***/ }),
/* 113 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var actions_1 = __webpack_require__(11);
var command_base_1 = __webpack_require__(0);
/**
 * add brings no dialog, just add an empty item
 *
 * import this module to commands.ts
 */
var Add = /** @class */ (function (_super) {
    __extends(Add, _super);
    function Add() {
        var _this = _super.call(this) || this;
        _this.makeDef('add', 'AddDemo', 'plus-circled', false, true, {
            showCondition: function (context) {
                return (context.contentBlock.isList) && (context.button.action.params.useModuleList) && (context.button.action.params.sortOrder !== -1);
            },
            code: function (context) {
                return actions_1.addItem(context, context.button.action.params.sortOrder + 1);
            },
        });
        return _this;
    }
    return Add;
}(command_base_1.CommandBase));
exports.Add = Add;
// ReSharper disable once UnusedLocals
var cmd = new Add();


/***/ }),
/* 114 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * open the import dialog
 *
 * import this module to commands.ts
 */
var AppImport = /** @class */ (function (_super) {
    __extends(AppImport, _super);
    function AppImport() {
        var _this = _super.call(this) || this;
        _this.makeDef('app-import', 'Dashboard', '', true, false, {});
        return _this;
    }
    return AppImport;
}(command_base_1.CommandBase));
exports.AppImport = AppImport;
// ReSharper disable once UnusedLocals
var cmd = new AppImport();


/***/ }),
/* 115 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var AppResources = /** @class */ (function (_super) {
    __extends(AppResources, _super);
    function AppResources() {
        var _this = _super.call(this) || this;
        _this.makeDef('app-resources', 'AppResources', 'language', true, false, {
            dialog: function (context) { return 'edit'; },
            disabled: function (context) {
                return context.app.resourcesId === null;
            },
            title: function (context) { return "Toolbar.AppResources" + (context.app.resourcesId === null ? 'Disabled' : ''); },
            showCondition: function (context) {
                return (context.user.canDesign) && (!context.app.isContent); // only if resources exist or are 0 (to be created)...
            },
            configureCommand: function (context, command) {
                command.items = [{ EntityId: context.app.resourcesId }];
            },
            dynamicClasses: function (context) {
                return context.app.resourcesId !== null ? '' : 'empty'; // if it doesn't have a query, make it less strong
            },
        });
        return _this;
    }
    return AppResources;
}(command_base_1.CommandBase));
exports.AppResources = AppResources;
// ReSharper disable once UnusedLocals
var cmd = new AppResources();


/***/ }),
/* 116 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var AppSettings = /** @class */ (function (_super) {
    __extends(AppSettings, _super);
    function AppSettings() {
        var _this = _super.call(this) || this;
        _this.makeDef('app-settings', 'AppSettings', 'sliders', true, false, {
            dialog: function (context) { return 'edit'; },
            disabled: function (context) {
                return context.app.settingsId === null;
            },
            title: function (context) { return "Toolbar.AppSettings" + (context.app.settingsId === null ? 'Disabled' : ''); },
            showCondition: function (context) {
                return (context.user.canDesign) && (!context.app.isContent); // only if settings exist, or are 0 (to be created)
            },
            configureCommand: function (context, command) {
                command.items = [{ EntityId: context.app.settingsId }];
            },
            dynamicClasses: function (context) {
                return context.app.settingsId !== null ? '' : 'empty'; // if it doesn't have a query, make it less strong
            },
        });
        return _this;
    }
    return AppSettings;
}(command_base_1.CommandBase));
exports.AppSettings = AppSettings;
// ReSharper disable once UnusedLocals
var cmd = new AppSettings();


/***/ }),
/* 117 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        var _this = _super.call(this) || this;
        _this.makeDef('app', 'App', 'settings', true, false, {
            showCondition: function (context) {
                return context.user.canDesign;
            },
        });
        return _this;
    }
    return App;
}(command_base_1.CommandBase));
exports.App = App;
// ReSharper disable once UnusedLocals
var cmd = new App();


/***/ }),
/* 118 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var ContentItems = /** @class */ (function (_super) {
    __extends(ContentItems, _super);
    function ContentItems() {
        var _this = _super.call(this) || this;
        _this.makeDef('contentitems', 'ContentItems', 'table', true, false, {
            params: function (context) {
                return { contentTypeName: context.contentBlock.contentTypeId };
            },
            showCondition: function (context) {
                return (context.user.canDesign) && ((!!context.button.action.params.contentType) || (!!context.contentBlock.contentTypeId));
            },
            configureCommand: function (context, command) {
                if (command.context.button.action.params.contentType) // optionally override with custom type
                    command.params.contentTypeName = command.context.button.action.params.contentType;
                // maybe: if item doesn't have a type, use that of template
                // else if (cmdSpecs.contentTypeId)
                //    cmd.params.contentTypeName = cmdSpecs.contentTypeId;
                if (context.button.action.params.filters) {
                    var enc = JSON.stringify(context.button.action.params.filters);
                    // special case - if it contains a "+" character, this won't survive
                    // encoding through the hash as it's always replaced with a space, even if it would be pre converted to %2b
                    // so we're base64 encoding it - see https://github.com/2sic/2sxc/issues/1061
                    if (enc.indexOf('+') > -1)
                        enc = btoa(enc);
                    command.params.filters = enc;
                }
            },
        });
        return _this;
    }
    return ContentItems;
}(command_base_1.CommandBase));
exports.ContentItems = ContentItems;
// ReSharper disable once UnusedLocals
var cmd = new ContentItems();


/***/ }),
/* 119 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var ContentType = /** @class */ (function (_super) {
    __extends(ContentType, _super);
    function ContentType() {
        var _this = _super.call(this) || this;
        _this.makeDef('contenttype', 'ContentType', 'fields', true, false, {
            showCondition: function (context) {
                return context.user.canDesign;
            },
        });
        return _this;
    }
    return ContentType;
}(command_base_1.CommandBase));
exports.ContentType = ContentType;
// ReSharper disable once UnusedLocals
var cmd = new ContentType();


/***/ }),
/* 120 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var Custom = /** @class */ (function (_super) {
    __extends(Custom, _super);
    function Custom() {
        var _this = _super.call(this) || this;
        _this.makeDef('custom', 'Custom', 'bomb', true, false, {
            code: function (context, event) {
                return new Promise(function (resolve, reject) {
                    console.log('custom action with code - BETA feature, may change');
                    if (!context.button.action.params.customCode) {
                        console.warn('custom code action, but no onclick found to run', context.button.action.params);
                        resolve();
                    }
                    try {
                        var fn = new Function('context', 'event', context.button.action.params.customCode); // jshint ignore:line
                        resolve(fn(context, event));
                    }
                    catch (err) {
                        console.error('error in custom button-code: ', context.button.action.params);
                        reject(err);
                    }
                });
            },
        });
        return _this;
    }
    return Custom;
}(command_base_1.CommandBase));
exports.Custom = Custom;
// ReSharper disable once UnusedLocals
var cmd = new Custom();


/***/ }),
/* 121 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var item_commands_1 = __webpack_require__(104);
var command_base_1 = __webpack_require__(0);
/**
 * todo: work in progress related to https://github.com/2sic/2sxc/issues/618
 *
 * import this module to commands.ts
 */
var Delete = /** @class */ (function (_super) {
    __extends(Delete, _super);
    function Delete() {
        var _this = _super.call(this) || this;
        _this.makeDef('delete', 'Delete', 'cancel', true, false, {
            // disabled: true,
            showCondition: function (context) {
                // can never be used for a modulelist item, as it is always in use somewhere
                if (context.button.action.params.useModuleList) {
                    return false;
                }
                // check if all data exists required for deleting
                return ((!!context.button.action.params.entityId)
                    && (!!context.button.action.params.entityGuid)
                    && (!!context.button.action.params.entityTitle));
            },
            code: function (context) {
                return item_commands_1.contentItems.delete(context, context.button.action.params.entityId, context.button.action.params.entityGuid, context.button.action.params.entityTitle);
            },
        });
        return _this;
    }
    return Delete;
}(command_base_1.CommandBase));
exports.Delete = Delete;
// ReSharper disable once UnusedLocals
var cmd = new Delete();


/***/ }),
/* 122 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * open an edit-item dialog
 *
 * import this module to commands.ts
 */
var Edit = /** @class */ (function (_super) {
    __extends(Edit, _super);
    function Edit() {
        var _this = _super.call(this) || this;
        _this.makeDef('edit', 'Edit', 'pencil', false, true, {
            params: function (context) {
                return { mode: 'edit' };
            },
            showCondition: function (context) {
                return (!!context.button.action.params.entityId) || (context.button.action.params.useModuleList); // need ID or a "slot", otherwise edit won't work
            },
        });
        return _this;
    }
    return Edit;
}(command_base_1.CommandBase));
exports.Edit = Edit;
// ReSharper disable once UnusedLocals
var cmd = new Edit();


/***/ }),
/* 123 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var InstanceList = /** @class */ (function (_super) {
    __extends(InstanceList, _super);
    function InstanceList() {
        var _this = _super.call(this) || this;
        _this.makeDef('instance-list', 'Sort', 'list-numbered', false, true, {
            showCondition: function (context) {
                return (context.contentBlock.isList)
                    && (context.button.action.params.useModuleList)
                    && (context.button.action.params.sortOrder !== -1);
            },
        });
        return _this;
    }
    return InstanceList;
}(command_base_1.CommandBase));
exports.InstanceList = InstanceList;
// ReSharper disable once UnusedLocals
var cmd = new InstanceList();


/***/ }),
/* 124 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * show the version dialog
 *
 * import this module to commands.ts
 */
var ItemHistory = /** @class */ (function (_super) {
    __extends(ItemHistory, _super);
    function ItemHistory() {
        var _this = _super.call(this) || this;
        _this.makeDef('item-history', 'ItemHistory', 'clock', true, false, {
            inlineWindow: function (context) { return true; },
            fullScreen: function (context) { return true; },
        });
        return _this;
    }
    return ItemHistory;
}(command_base_1.CommandBase));
exports.ItemHistory = ItemHistory;
// ReSharper disable once UnusedLocals
var cmd = new ItemHistory();


/***/ }),
/* 125 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var Layout = /** @class */ (function (_super) {
    __extends(Layout, _super);
    function Layout() {
        var _this = _super.call(this) || this;
        _this.makeDef('layout', 'ChangeLayout', 'glasses', true, true, {
            inlineWindow: function (context) { return true; },
        });
        return _this;
    }
    return Layout;
}(command_base_1.CommandBase));
exports.Layout = Layout;
// ReSharper disable once UnusedLocals
var cmd = new Layout();


/***/ }),
/* 126 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * create a metadata toolbar
 *
 * import this module to commands.ts
 */
var Metadata = /** @class */ (function (_super) {
    __extends(Metadata, _super);
    function Metadata() {
        var _this = _super.call(this) || this;
        _this.makeDef('metadata', 'Metadata', 'tag', false, false, {
            params: function (context) {
                return { mode: 'new' };
            },
            dialog: function (context) { return 'edit'; },
            dynamicClasses: function (context) {
                // if it doesn't have data yet, make it less strong
                return context.button.action.params.entityId ? '' : 'empty';
                // return settings.items && settings.items[0].entityId ? "" : "empty";
            },
            showCondition: function (context) {
                return (!!context.button.action.params.metadata);
            },
            configureCommand: function (context, command) {
                var itm = {
                    Title: 'EditFormTitle.Metadata',
                    Metadata: Object.assign({ keyType: 'string', targetType: 10 }, command.context.button.action.params.metadata),
                };
                Object.assign(command.items[0], itm);
            },
        });
        return _this;
    }
    return Metadata;
}(command_base_1.CommandBase));
exports.Metadata = Metadata;
// ReSharper disable once UnusedLocals
var cmd = new Metadata();


/***/ }),
/* 127 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
var Constants = __webpack_require__(35);
/**
 * import this module to commands.ts
 */
var More = /** @class */ (function (_super) {
    __extends(More, _super);
    function More() {
        var _this = _super.call(this) || this;
        _this.makeDef('more', 'MoreActions', 'options btn-mode', true, false, {
            code: function (context, event) {
                return new Promise(function (resolve, reject) {
                    var btn2 = event.target;
                    var fullMenu2 = btn2.closest('ul.sc-menu');
                    var oldState2 = Number(fullMenu2.getAttribute('data-state') || 0);
                    var max2 = Number(fullMenu2.getAttribute('group-count'));
                    var newState2 = (oldState2 + 1) % max2;
                    fullMenu2.classList.remove("group-" + oldState2);
                    fullMenu2.classList.add("group-" + newState2);
                    fullMenu2.setAttribute('data-state', String(newState2));
                    event.preventDefault();
                    function mouseenterHandler(e) {
                        fullMenu2.style.opacity = '1';
                    }
                    function mouseleaveHandler(e) {
                        if (e.screenX != 0 && e.screenY != 0) {
                            // hide toolbar on mouseleave
                            fullMenu2.style.opacity = '0';
                        }
                        else {
                            // this is fix for Chrome issue
                            // ensure to show toolbar because X=0 and Y=0
                            fullMenu2.style.opacity = '1';
                            console.warn('workaround for toolbar hide onmouseleave issue', e.screenX, e.screenY, e.target);
                        }
                    }
                    // because of issue in Chrome we need to override CSS rules in edit.css for toolbar toggle on mouse hover
                    var scElement = fullMenu2.closest('.' + Constants.toolbar.classes.oldHover);
                    // add mouseenter and mouseleave events to parent sc-element if not already added
                    if (scElement && fullMenu2.getAttribute('listener') !== 'true') {
                        scElement.addEventListener('mouseenter', mouseenterHandler);
                        scElement.addEventListener('mouseleave', mouseleaveHandler);
                        fullMenu2.setAttribute('listener', 'true'); // flag that events are added
                    }
                    resolve();
                });
            },
        });
        return _this;
    }
    return More;
}(command_base_1.CommandBase));
exports.More = More;
// ReSharper disable once UnusedLocals
var cmd = new More();


/***/ }),
/* 128 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var actions_1 = __webpack_require__(11);
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var MoveDown = /** @class */ (function (_super) {
    __extends(MoveDown, _super);
    function MoveDown() {
        var _this = _super.call(this) || this;
        _this.makeDef('movedown', 'MoveDown', 'move-down', false, true, {
            showCondition: function (context) {
                // TODO: do not display if is last item in list
                return (context.contentBlock.isList)
                    && (context.button.action.params.useModuleList)
                    && (context.button.action.params.sortOrder !== -1);
            },
            code: function (context) {
                // TODO: make sure index is never greater than the amount of items
                return actions_1.changeOrder(context, context.button.action.params.sortOrder, context.button.action.params.sortOrder + 1);
            },
        });
        return _this;
    }
    return MoveDown;
}(command_base_1.CommandBase));
exports.MoveDown = MoveDown;
// ReSharper disable once UnusedLocals
var cmd = new MoveDown();


/***/ }),
/* 129 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var actions_1 = __webpack_require__(11);
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var MoveUp = /** @class */ (function (_super) {
    __extends(MoveUp, _super);
    function MoveUp() {
        var _this = _super.call(this) || this;
        _this.makeDef('moveup', 'MoveUp', 'move-up', false, true, {
            showCondition: function (context) {
                return (context.contentBlock.isList) &&
                    (context.button.action.params.useModuleList) &&
                    (context.button.action.params.sortOrder !== -1) &&
                    (context.button.action.params.sortOrder !== 0);
            },
            code: function (context) {
                return actions_1.changeOrder(context, context.button.action.params.sortOrder, Math.max(context.button.action.params.sortOrder - 1, 0));
            },
        });
        return _this;
    }
    return MoveUp;
}(command_base_1.CommandBase));
exports.MoveUp = MoveUp;
// ReSharper disable once UnusedLocals
var cmd = new MoveUp();


/***/ }),
/* 130 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
var command_open_ng_dialog_1 = __webpack_require__(40);
/**
 * new is a dialog to add something, and will not add if cancelled
 * new can also be used for mini-toolbars which just add an entity not attached to a module
 * in that case it's essential to add a contentType like
 * <ul class="sc-menu" data-toolbar='{"action":"new", "contentType": "Category"}'></ul>
 *
 * import this module to commands.ts
 */
var New = /** @class */ (function (_super) {
    __extends(New, _super);
    function New() {
        var _this = _super.call(this) || this;
        _this.makeDef('new', 'New', 'plus', false, true, {
            params: function (context) {
                return { mode: 'new' };
            },
            dialog: function (context) { return 'edit'; },
            showCondition: function (context) {
                return (!!context.button.action.params.contentType) ||
                    ((context.contentBlock.isList) && (context.button.action.params.useModuleList) && (context.button.action.params.sortOrder !== -1)); // don't provide new on the header-item
            },
            code: function (context, event) {
                // todo - should refactor this to be a toolbarManager.contentBlock command
                Object.assign(context.button.action.params, { sortOrder: context.button.action.params.sortOrder + 1 });
                return command_open_ng_dialog_1.commandOpenNgDialog(context, event);
            },
        });
        return _this;
    }
    return New;
}(command_base_1.CommandBase));
exports.New = New;
// ReSharper disable once UnusedLocals
var cmd = new New();


/***/ }),
/* 131 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var actions_1 = __webpack_require__(11);
var _2sxc_translate_1 = __webpack_require__(9);
var command_base_1 = __webpack_require__(0);
/**
 * todo: shouldn't be available if changes are not allowed
 *
 * import this module to commands.ts
 */
var Publish = /** @class */ (function (_super) {
    __extends(Publish, _super);
    function Publish() {
        var _this = _super.call(this) || this;
        _this.makeDef('publish', 'Unpublished', 'eye-off', false, false, {
            showCondition: function (context) {
                return (context.button.action.params.isPublished === false);
            },
            disabled: function (context) {
                return !context.instance.allowPublish;
            },
            code: function (context, event) {
                return new Promise(function (resolve, reject) {
                    if (context.button.action.params.isPublished) {
                        alert(_2sxc_translate_1.translate('Toolbar.AlreadyPublished'));
                        return resolve();
                    }
                    // if we have an entity-id, publish based on that
                    if (context.button.action.params.entityId) {
                        return actions_1.publishId(context, context.button.action.params.entityId);
                    }
                    var part = context.button.action.params.sortOrder === -1 ? 'listcontent' : 'content';
                    var index = context.button.action.params.sortOrder === -1 ? 0 : context.button.action.params.sortOrder;
                    return actions_1.publish(context, part, index);
                });
            },
        });
        return _this;
    }
    return Publish;
}(command_base_1.CommandBase));
exports.Publish = Publish;
// ReSharper disable once UnusedLocals
var cmd = new Publish();


/***/ }),
/* 132 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var actions_1 = __webpack_require__(11);
var _2sxc_translate_1 = __webpack_require__(9);
var command_base_1 = __webpack_require__(0);
/**
 * remove an item from the placeholder (usually for lists)
 *
 * import this module to commands.ts
 */
var Remove = /** @class */ (function (_super) {
    __extends(Remove, _super);
    function Remove() {
        var _this = _super.call(this) || this;
        _this.makeDef('remove', 'Remove', 'minus-circled', false, true, {
            showCondition: function (context) {
                return (context.contentBlock.isList) &&
                    (context.button.action.params.useModuleList) &&
                    (context.button.action.params.sortOrder !== -1);
            },
            code: function (context) {
                return new Promise(function (resolve, reject) {
                    if (confirm(_2sxc_translate_1.translate('Toolbar.ConfirmRemove'))) {
                        return actions_1.removeFromList(context, context.button.action.params.sortOrder);
                    }
                    return resolve();
                });
            },
        });
        return _this;
    }
    return Remove;
}(command_base_1.CommandBase));
exports.Remove = Remove;
// ReSharper disable once UnusedLocals
var cmd = new Remove();


/***/ }),
/* 133 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var Replace = /** @class */ (function (_super) {
    __extends(Replace, _super);
    function Replace() {
        var _this = _super.call(this) || this;
        _this.makeDef('replace', 'Replace', 'replace', false, true, {
            showCondition: function (context) {
                return (context.button.action.params.useModuleList);
            },
        });
        return _this;
    }
    return Replace;
}(command_base_1.CommandBase));
exports.Replace = Replace;
// ReSharper disable once UnusedLocals
var cmd = new Replace();


/***/ }),
/* 134 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var TemplateDevelop = /** @class */ (function (_super) {
    __extends(TemplateDevelop, _super);
    function TemplateDevelop() {
        var _this = _super.call(this) || this;
        _this.makeDef('template-develop', 'Develop', 'code', true, false, {
            newWindow: function (context) { return true; },
            dialog: function (context) { return 'develop'; },
            showCondition: function (context) {
                return (context.user.canDesign);
            },
            configureCommand: function (context, command) {
                command.items = [{ EntityId: context.contentBlock.templateId }];
            },
        });
        return _this;
    }
    return TemplateDevelop;
}(command_base_1.CommandBase));
exports.TemplateDevelop = TemplateDevelop;
// ReSharper disable once UnusedLocals
var cmd = new TemplateDevelop();


/***/ }),
/* 135 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var TemplateQuery = /** @class */ (function (_super) {
    __extends(TemplateQuery, _super);
    function TemplateQuery() {
        var _this = _super.call(this) || this;
        _this.makeDef('template-query', 'QueryEdit', 'filter', true, false, {
            dialog: function (context) { return 'pipeline-designer'; },
            params: function (context) {
                return { pipelineId: context.contentBlock.queryId };
            },
            newWindow: function (context) { return true; },
            disabled: function (context) {
                return context.app.settingsId === null;
            },
            title: function (context) { return "Toolbar.QueryEdit" + (context.contentBlock.queryId === null ? 'Disabled' : ''); },
            showCondition: function (context) {
                return (context.user.canDesign) && (!context.app.isContent);
            },
            dynamicClasses: function (context) {
                return context.contentBlock.queryId ? '' : 'empty'; // if it doesn't have a query, make it less strong
            },
        });
        return _this;
    }
    return TemplateQuery;
}(command_base_1.CommandBase));
exports.TemplateQuery = TemplateQuery;
// ReSharper disable once UnusedLocals
var cmd = new TemplateQuery();


/***/ }),
/* 136 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var TemplateSettings = /** @class */ (function (_super) {
    __extends(TemplateSettings, _super);
    function TemplateSettings() {
        var _this = _super.call(this) || this;
        _this.makeDef('template-settings', 'TemplateSettings', 'sliders', true, false, {
            dialog: function (context) { return 'edit'; },
            showCondition: function (context) {
                return (context.user.canDesign) && (!context.app.isContent);
            },
            configureCommand: function (context, command) {
                command.items = [{ EntityId: context.contentBlock.templateId }];
            },
        });
        return _this;
    }
    return TemplateSettings;
}(command_base_1.CommandBase));
exports.TemplateSettings = TemplateSettings;
// ReSharper disable once UnusedLocals
var cmd = new TemplateSettings();


/***/ }),
/* 137 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var command_base_1 = __webpack_require__(0);
/**
 * import this module to commands.ts
 */
var Zone = /** @class */ (function (_super) {
    __extends(Zone, _super);
    function Zone() {
        var _this = _super.call(this) || this;
        _this.makeDef('zone', 'Zone', 'manage', true, false, {
            showCondition: function (context) {
                return (context.user.canDesign);
            },
        });
        return _this;
    }
    return Zone;
}(command_base_1.CommandBase));
exports.Zone = Zone;
// ReSharper disable once UnusedLocals
var cmd = new Zone();


/***/ }),
/* 138 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * Command definition, for creation of commands
 */
var Definition = /** @class */ (function () {
    function Definition() {
    }
    return Definition;
}());
exports.Definition = Definition;


/***/ }),
/* 139 */
/***/ (function(module, exports) {



/***/ }),
/* 140 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Params = /** @class */ (function () {
    function Params() {
    }
    return Params;
}());
exports.Params = Params;


/***/ }),
/* 141 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Settings = /** @class */ (function () {
    function Settings() {
    }
    return Settings;
}());
exports.Settings = Settings;


/***/ }),
/* 142 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * params for getAndReload WebAPI
 */
var ActionParams = /** @class */ (function () {
    function ActionParams() {
    }
    return ActionParams;
}());
exports.ActionParams = ActionParams;


/***/ }),
/* 143 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ManipulateParams = /** @class */ (function () {
    function ManipulateParams() {
    }
    return ManipulateParams;
}());
exports.ManipulateParams = ManipulateParams;


/***/ }),
/* 144 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var WebApiParams = /** @class */ (function () {
    function WebApiParams() {
    }
    return WebApiParams;
}());
exports.WebApiParams = WebApiParams;


/***/ }),
/* 145 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ContentBlock = /** @class */ (function () {
    function ContentBlock() {
    }
    return ContentBlock;
}());
exports.ContentBlock = ContentBlock;


/***/ }),
/* 146 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ContentGroup = /** @class */ (function () {
    function ContentGroup() {
    }
    return ContentGroup;
}());
exports.ContentGroup = ContentGroup;


/***/ }),
/* 147 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var DataEditContext = /** @class */ (function () {
    function DataEditContext() {
    }
    return DataEditContext;
}());
exports.DataEditContext = DataEditContext;


/***/ }),
/* 148 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Environment = /** @class */ (function () {
    function Environment() {
    }
    return Environment;
}());
exports.Environment = Environment;


/***/ }),
/* 149 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Error = /** @class */ (function () {
    function Error() {
    }
    return Error;
}());
exports.Error = Error;


/***/ }),
/* 150 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Language = /** @class */ (function () {
    function Language() {
    }
    return Language;
}());
exports.Language = Language;


/***/ }),
/* 151 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ParametersEntity = /** @class */ (function () {
    function ParametersEntity() {
    }
    return ParametersEntity;
}());
exports.ParametersEntity = ParametersEntity;


/***/ }),
/* 152 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Ui = /** @class */ (function () {
    function Ui() {
    }
    return Ui;
}());
exports.Ui = Ui;


/***/ }),
/* 153 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var User = /** @class */ (function () {
    function User() {
    }
    return User;
}());
exports.User = User;


/***/ }),
/* 154 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var window_in_page_1 = __webpack_require__(1);
var api_1 = __webpack_require__(4);
var sxc_1 = __webpack_require__(6);
/**
 * Maps actions of the module menu to JS actions - needed because onclick event can't be set (actually, a bug in DNN)
 */
var ActionMenuMapper = /** @class */ (function () {
    function ActionMenuMapper(moduleId) {
        var _this = this;
        this.changeLayoutOrContent = function () { _this.run('layout'); };
        this.addItem = function () { _this.run('add', { useModuleList: true, sortOrder: 0 }); };
        this.edit = function () {
            _this.run('edit', { useModuleList: true, sortOrder: 0 });
        };
        this.adminApp = function () { _this.run('app'); };
        this.adminZone = function () { _this.run('zone'); };
        this.develop = function () { _this.run('template-develop'); };
        this.sxc = sxc_1.getSxcInstance(moduleId);
        this.tag = api_1.getTag(this.sxc);
        this.run = this.sxc.manage.run;
    }
    return ActionMenuMapper;
}());
exports.ActionMenuMapper = ActionMenuMapper;
window_in_page_1.windowInPage.$2sxcActionMenuMapper = function (moduleId) {
    return new ActionMenuMapper(moduleId);
};


/***/ }),
/* 155 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
// The following script fixes a bug in DNN 08.00.04
// the bug tries to detect a module-ID based on classes in a tag,
// but uses a bad regex and captures the number 2 on all 2sxc-modules
// instead of the real id
// this patch changes the order of the className of 2sxc modules when
// they are accessed through '$.fn.attr'
// 'DnnModule-2sxc DnnModule-xxx' -> DNN thinks the mod id is 2 (false)
// 'DnnModule-xxx DnnModule-2sxc' -> DNN thinks the mod id is xxx (correct)
// documented here https://github.com/2sic/2sxc/issues/986
/**
 * Fix drag-drop functionality in dnn 08.00.04 - it has an incorrect regex
 */

(function () {
    var fn = $.fn.attr;
    $.fn.attr = function () {
        var val = fn.apply(this, arguments);
        if (arguments[0] !== 'class' || typeof val !== 'string' || val.search('DnnModule-2sxc ') === -1)
            return val;
        return val.replace('DnnModule-2sxc ', '') + ' DnnModule-2sxc';
    };
})();


/***/ }),
/* 156 */
/***/ (function(module, exports) {



/***/ }),
/* 157 */
/***/ (function(module, exports) {



/***/ }),
/* 158 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

//
// Note: this interface is copied/shared between this and angular quick-edit
//
Object.defineProperty(exports, "__esModule", { value: true });


/***/ }),
/* 159 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

//
// Note: this interface is copied/shared between this and angular quick-edit
//
Object.defineProperty(exports, "__esModule", { value: true });


/***/ }),
/* 160 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });


/***/ }),
/* 161 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

//
// Note: this interface is copied/shared between this and angular quick-edit
//
Object.defineProperty(exports, "__esModule", { value: true });


/***/ }),
/* 162 */
/***/ (function(module, exports) {



/***/ }),
/* 163 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
// ReSharper disable once UnusedParameter
function extend() {
    var args = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        args[_i] = arguments[_i];
    }
    for (var i = 1; i < arguments.length; i++)
        for (var key in arguments[i])
            if (arguments[i].hasOwnProperty(key))
                arguments[0][key] = arguments[i][key];
    return arguments[0];
}
exports.extend = extend;


/***/ }),
/* 164 */
/***/ (function(module, exports) {

/**
 * jquery helper
 */
//declare let $: any;


/***/ }),
/* 165 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });


/***/ }),
/* 166 */
/***/ (function(module, exports) {

/**
 * Symbol polyfill for es5 from lib.es6
 * https://github.com/Microsoft/TypeScript/blob/f17bf54bfe3f1e02e47af7660336a88f9ed2a316/lib/lib.es6.d.ts#L5501
 */


/***/ }),
/* 167 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });


/***/ }),
/* 168 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * used in Selectors class
 */
var CbOrMod = /** @class */ (function () {
    function CbOrMod() {
    }
    return CbOrMod;
}());
exports.CbOrMod = CbOrMod;


/***/ }),
/* 169 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Conf = /** @class */ (function () {
    function Conf() {
    }
    return Conf;
}());
exports.Conf = Conf;


/***/ }),
/* 170 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var cb_1 = __webpack_require__(44);
var clipboard_1 = __webpack_require__(25);
var quick_e_1 = __webpack_require__(2);
var selectors_instance_1 = __webpack_require__(8);
/**
 * content-block specific stuff like actions
 */
function onCbButtonClick() {
    var list = quick_e_1.$quickE.main.actionsForCb.closest(selectors_instance_1.selectors.cb.listSelector);
    var listItems = list.find(selectors_instance_1.selectors.cb.selector);
    var actionConfig = JSON.parse(list.attr(selectors_instance_1.selectors.cb.context));
    var index = 0;
    var newGuid = actionConfig.guid || null;
    if (quick_e_1.$quickE.main.actionsForCb.hasClass(selectors_instance_1.selectors.cb.class))
        index = listItems.index(quick_e_1.$quickE.main.actionsForCb[0]) + 1;
    // check cut/paste
    var cbAction = $(this).data('action');
    if (cbAction) {
        // this is a cut/paste action
        return clipboard_1.copyPasteInPage(cbAction, list, index, selectors_instance_1.selectors.cb.id);
    }
    else {
        var appOrContent = $(this).data('type');
        return cb_1.Cb.create(actionConfig.parent, actionConfig.field, index, appOrContent, list, newGuid);
    }
}
quick_e_1.$quickE.cbActions.click(onCbButtonClick);


/***/ }),
/* 171 */
/***/ (function(module, exports) {



/***/ }),
/* 172 */
/***/ (function(module, exports) {



/***/ }),
/* 173 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var clipboard_1 = __webpack_require__(25);
var mod_manage_1 = __webpack_require__(46);
var quick_e_1 = __webpack_require__(2);
var selectors_instance_1 = __webpack_require__(8);
/**
 * module specific stuff
 */
function onModuleButtonClick() {
    var type = $(this).data('type');
    var dnnMod = quick_e_1.$quickE.main.actionsForModule;
    var pane = dnnMod.closest(selectors_instance_1.selectors.mod.listSelector);
    var index = 0;
    if (dnnMod.hasClass('DnnModule'))
        index = pane.find('.DnnModule').index(dnnMod[0]) + 1;
    var cbAction = $(this).data('action');
    if (cbAction) {
        return clipboard_1.copyPasteInPage(cbAction, pane, index, selectors_instance_1.selectors.mod.id); // copy/paste
    }
    return mod_manage_1.modManage.create(mod_manage_1.modManage.getPaneName(pane), index, type);
}
/**
 * bind module actions click
 */
quick_e_1.$quickE.modActions.click(onModuleButtonClick);


/***/ }),
/* 174 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * Selectors class used to host all QickE selectors in one place
 */
var Selectors = /** @class */ (function () {
    function Selectors() {
    }
    return Selectors;
}());
exports.Selectors = Selectors;


/***/ }),
/* 175 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Specs = /** @class */ (function () {
    function Specs() {
    }
    return Specs;
}());
exports.Specs = Specs;


/***/ }),
/* 176 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
/**
 * Button Definition v1. from old API
 * it is publicly used out of inpage, so take a care to preserve its signature
 */
var ButtonDefinition = /** @class */ (function () {
    function ButtonDefinition() {
    }
    return ButtonDefinition;
}());
exports.ButtonDefinition = ButtonDefinition;


/***/ }),
/* 177 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var GroupConfig = /** @class */ (function () {
    function GroupConfig(buttons) {
        this.buttons = []; // array of buttons
        this.defaults = []; // v1
        // adds these to the items
        this.buttons = buttons;
    }
    GroupConfig.fromNameAndParams = function (name, params) {
        var groupConfig = new GroupConfig([]);
        // builds buttons from name and params, then adds
        return groupConfig;
    };
    return GroupConfig;
}());
exports.GroupConfig = GroupConfig;


/***/ }),
/* 178 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ItemRender = /** @class */ (function () {
    function ItemRender() {
    }
    return ItemRender;
}());
exports.ItemRender = ItemRender;


/***/ }),
/* 179 */
/***/ (function(module, exports) {

/*
 * Author: Alex Gibson
 * https://github.com/alexgibson/shake.js
 * License: MIT license
 */
(function (global, factory) {
    global.Shake = factory(global, global.document);
}(typeof window !== 'undefined' ? window : this, function (window, document) {
    'use strict';
    function Shake(options) {
        //feature detect
        this.hasDeviceMotion = 'ondevicemotion' in window;
        this.options = {
            threshold: 15,
            timeout: 1000,
            callback: null,
        };
        if (typeof options === 'object') {
            for (var i in options) {
                if (options.hasOwnProperty(i)) {
                    this.options[i] = options[i];
                }
            }
        }
        //use date to prevent multiple shakes firing
        this.lastTime = new Date();
        //accelerometer values
        this.lastX = null;
        this.lastY = null;
        this.lastZ = null;
    }
    //reset timer values
    Shake.prototype.reset = function () {
        this.lastTime = new Date();
        this.lastX = null;
        this.lastY = null;
        this.lastZ = null;
    };
    //start listening for devicemotion
    Shake.prototype.start = function () {
        this.reset();
        if (this.hasDeviceMotion) {
            window.addEventListener('devicemotion', this, false);
        }
    };
    //stop listening for devicemotion
    Shake.prototype.stop = function () {
        if (this.hasDeviceMotion) {
            window.removeEventListener('devicemotion', this, false);
        }
        this.reset();
    };
    //calculates if shake did occur
    Shake.prototype.devicemotion = function (e) {
        var current = e.accelerationIncludingGravity;
        var deltaX = 0;
        var deltaY = 0;
        var deltaZ = 0;
        if ((this.lastX === null) && (this.lastY === null) && (this.lastZ === null)) {
            this.lastX = current.x;
            this.lastY = current.y;
            this.lastZ = current.z;
            return;
        }
        deltaX = Math.abs(this.lastX - current.x);
        deltaY = Math.abs(this.lastY - current.y);
        deltaZ = Math.abs(this.lastZ - current.z);
        if (((deltaX > this.options.threshold) && (deltaY > this.options.threshold)) ||
            ((deltaX > this.options.threshold) && (deltaZ > this.options.threshold)) ||
            ((deltaY > this.options.threshold) && (deltaZ > this.options.threshold))) {
            //calculate time in milliseconds since last shake registered
            var currentTime = void 0;
            currentTime = new Date();
            var timeDifference = void 0;
            timeDifference = currentTime.getTime() - this.lastTime.getTime();
            if (timeDifference > this.options.timeout) {
                // once triggered, execute  the callback
                if (typeof this.options.callback === 'function') {
                    this.options.callback();
                }
                else
                    console.log('shake event without callback detected');
                this.lastTime = new Date();
            }
        }
        this.lastX = current.x;
        this.lastY = current.y;
        this.lastZ = current.z;
    };
    //event handler
    Shake.prototype.handleEvent = function (e) {
        if (typeof (this[e.type]) === 'function') {
            return this[e.type](e);
        }
    };
    return Shake;
}));


/***/ }),
/* 180 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var sxc_controller_in_page_1 = __webpack_require__(3);
// prevent propagation of the click (if menu was clicked)
$(sxc_controller_in_page_1.$2sxcInPage.c.sel.scMenu).click(function (e) { return e.stopPropagation(); });


/***/ }),
/* 181 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });


/***/ }),
/* 182 */
/***/ (function(module, exports) {

// enable shake detection on all toolbars
$(function () {
    // this will add a css-class to auto-show all toolbars (or remove it again)
    function toggleAllToolbars() {
        $(document.body).toggleClass('sc-tb-show-all');
    }
    // start shake-event monitoring, which will then generate a window-event
    (new Shake({ callback: toggleAllToolbars })).start();
});


/***/ }),
/* 183 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var ToolbarConfigTemplate = /** @class */ (function () {
    function ToolbarConfigTemplate() {
        this.groups = [];
        this.defaults = {};
        this.params = {};
        this.settings = {};
    }
    return ToolbarConfigTemplate;
}());
exports.ToolbarConfigTemplate = ToolbarConfigTemplate;
var item = /** @class */ (function () {
    function item() {
        this.defaults = {};
    }
    return item;
}());


/***/ })
/******/ ]);
//# sourceMappingURL=inpage.js.map