$(function () {
    "use strict";

    //var enableModuleMove = false; // not implemented yet
    var selectors = {
        listContainerSelector: ".sc-content-block-list",
        contentBlockClass: "sc-content-block",
        contentBlockSelector: ".sc-content-block",
        moduleSelector: ".DnnModule",
        eitherCbOrMod: ".DnnModule, .sc-content-block",
        paneSelector: ".DNNEmptyPane, .dnnDropEmptyPanes, :has(>.DnnModule)", // Found no better way to get all panes - the hidden variable does not exist when not in edit page mode
        listDataAttr: "data-list-context",
        selected: "sc-cb-is-selected"
    };

    var hasContentBlocks = ($(selectors.listContainerSelector).length > 0);

    function btn(action, icon, i18n, invisible, unavailable, classes) {
        return "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-" + icon + " " 
            + (invisible ? " sc-invisible " : "") 
            + (unavailable ? " sc-unavailable " : "")
            + classes + "' data-action='" + action + "' data-i18n='[title]QuickInsertMenu." + i18n + "'></a>";
    }

    // the quick-insert object
    var qi = {
        enableCb: hasContentBlocks, // for now, ContentBlocks are only enabled if they exist on the page
        enableMod: !hasContentBlocks,   // if it has inner-content, then it's probably a details page, where quickly adding modules would be a problem, so for now, disable modules in this case
        body: $("body"),
        win: $(window),
        main: $("<div class='sc-content-block-menu sc-content-block-quick-insert sc-i18n'></div>"),
        template: "<a class='sc-content-block-menu-addcontent sc-invisible' data-type='Default' data-i18n='[title]QuickInsertMenu.AddBlockContent'>x</a>"
            + "<a class='sc-content-block-menu-addapp sc-invisible' data-type='' data-i18n='[title]QuickInsertMenu.AddBlockApp'>x</a>"
            + btn("select", "ok", "Select", true)
            + btn("paste", "paste", "Paste", true, true),
        selected: $("<div class='sc-content-block-menu sc-content-block-selected-menu sc-i18n'></div>")
            .append(btn("cancel", "ok", "Cancel") + btn("delete", "trash-empty", "Delete")),
        contentBlocks: null,
        modules: null,
        nearestCb: null, 
        nearestMod: null
    };

    // add stuff which must be added in a second run
    $2sxc._lib.extend(qi, {
        cbActions: $(qi.template),
        modActions: $(qi.template.replace(/QuickInsertMenu.AddBlock/g, "QuickInsertMenu.AddModule")).attr("data-context", "module").addClass("sc-content-block-menu-module"),
    });

    qi.init = function() {
        qi.body.append(qi.main);
        qi.body.append(qi.selected);

        // content blocks actions
        if (qi.enableCb)
            qi.main.append(qi.cbActions);

        // module actions
        if (qi.enableMod)
            qi.main.append(qi.modActions);
    };

    qi.init();

    qi.selected.toggle = function(target) {
        if (!target)
            return qi.selected.hide();

        var coords = qi.getCoordinates(target);
        coords.yh = coords.y + 20;
        qi.positionAndAlign(qi.selected, coords);
        qi.selected.target = target;
    };
    
    // give all actions
    $("a", qi.selected).click(function () {
        var action = $(this).data("action");
        var clip = qi.clipboard.data;
        switch(action) {
            case "cancel":
                return qi.clipboard.clear();
            case "delete":
                $2sxc(clip.list).manage.deleteContentBlock(clip.parent, clip.field, clip.index);

        }
    });

    qi.cbActions.click(function () {
        var list = qi.main.actionsForCb.closest(selectors.listContainerSelector);
        var listItems = list.find(selectors.contentBlockSelector);
        var actionConfig = JSON.parse(list.attr(selectors.listDataAttr));
        var index = 0;

        if (qi.main.actionsForCb.hasClass(selectors.contentBlockClass))
            index = listItems.index(qi.main.actionsForCb[0]) + 1;

        // check cut/paste
        var cbAction = $(this).data("action");
        if (!cbAction) {
            var appOrContent = $(this).data("type");
            return $2sxc(list).manage.createContentBlock(actionConfig.parent, actionConfig.field, index, appOrContent, list);
        } else
            // this is a cut/paste action
            return qi.copyPasteInPage(cbAction, list, index, "contentblock");
    });

    qi.copyPasteInPage = function (cbAction, list, index, type) {
        var clip = qi.clipboard.createSpecs(type, list, index);

        // action!
        if (cbAction === "select") {
            qi.clipboard.mark(clip); 
        } else if (cbAction === "paste") {
            var from = qi.clipboard.data.index, to = clip.index;
            if (isNaN(from) || isNaN(to) || from === to || from + 1 === to) // this moves it to the same spot, so ignore
                return qi.clipboard.clear(); // don't do anything

            $2sxc(list).manage.moveContentBlock(clip.parent, clip.field, from, to);
            qi.clipboard.clear();
        } 
    };

    qi.clipboard = {
        data: {},
        mark: function (newData) {
            if (newData) {
                // if it was already selected with the same thing, then release it
                if (qi.clipboard.data && qi.clipboard.data.item === newData.item)
                    return qi.clipboard.clear();
                qi.clipboard.data = newData;
            }
            $("." + selectors.selected).removeClass(selectors.selected); // clear previous markings
            var cb = $(qi.clipboard.data.item);
            cb.addClass(selectors.selected);
            if (cb.prev().is("iframe"))
                cb.prev().addClass(selectors.selected);
            qi.setSecondaryActionsState(true);
            qi.selected.toggle(cb);
        },
        clear: function() {
            $("." + selectors.selected).removeClass(selectors.selected);
            qi.clipboard.data = null;
            qi.setSecondaryActionsState(false);
            qi.selected.toggle(false);
        },
        createSpecs: function (type, list, index) {
            var listItems = list.find(selectors.contentBlockSelector);
            if (index >= listItems.length) index = listItems.length - 1; // sometimes the index is 1 larger than the length, then select last
            var currentItem = listItems[index];
            var actionConfig = JSON.parse(list.attr(selectors.listDataAttr));
            return { parent: actionConfig.parent, field: actionConfig.field, list: list, item: currentItem, index: index, type: type };
        }
    };

    qi.setSecondaryActionsState = function(state) {
        var btns = $("a.sc-content-block-menu-btn");
        btns = btns.filter(".icon-sxc-paste"); // later also : , .icon-sxc-trash"); // only on the main one...?
        btns.toggleClass("sc-unavailable", !state);
    };

    qi.modActions.click(function () {
        var type = $(this).data("type");
        var pane = qi.main.actionsForModule.closest(selectors.paneSelector);
        var paneName = pane.attr("id").replace("dnn_", "");

        var index = 0;
        if (qi.main.actionsForModule.hasClass("DnnModule"))
            index = pane.find(".DnnModule").index(qi.main.actionsForModule[0]) + 1;

        // todo: try to use $2sxc(...).webApi instead of custom re-assembling these common build-up things
        // how: create a object containing the url, data, then just use the sxc.webApi(yourobject)
        var service = $.dnnSF();
        var serviceUrl = service.getServiceRoot("internalservices") + "controlbar/";

        var xhrError = function(xhr) {
            alert("Error while adding module.");
            console.log(xhr);
        };
        $.ajax({
            url: serviceUrl + "GetPortalDesktopModules",
            type: "GET",
            data: "category=All&loadingStartIndex=0&loadingPageSize=100&searchTerm=",
            beforeSend: service.setModuleHeaders,
            success: function (desktopModules) {
                var moduleToFind = type === "Default" ? " Content" : " App";
                var module = null;
                
                desktopModules.forEach(function (e,i) {
                    if (e.ModuleName === moduleToFind)
                        module = e;
                });

                if (!module)
                    return alert(moduleToFind + " module not found.");

                var postData = {
                    Module: module.ModuleID,
                    Page: "",
                    Pane: paneName,
                    Position: -1,
                    Sort: index,
                    Visibility: 0,
                    AddExistingModule: false,
                    CopyModule: false
                };



                $.ajax({
                    url: serviceUrl + "AddModule",
                    type: "POST",
                    data: postData,
                    beforeSend: service.setModuleHeaders,
                    success: function (d) {
                        window.location.reload();
                    },
                    error: xhrError
                });
            },
            error: xhrError
        });

        

    });

    var refreshTimeout = null;
    $("body").on("mousemove", function (e) {
        
        if (refreshTimeout === null)
            refreshTimeout = window.setTimeout(function () {
                requestAnimationFrame(function () {
                    qi.refresh(e);
                    refreshTimeout = null;
                });
            }, 20);

    });
    
    // Prepare offset calculation based on body positioning
    qi.getBodyPosition = function() {
        var bodyPos = qi.body.css("position");
        return bodyPos === "relative" || bodyPos === "absolute"
            ? { x: qi.body.offset().left, y: qi.body.offset().top }
            : { x: 0, y: 0 };
    };

    qi.bodyOffset = qi.getBodyPosition();

    // Refresh content block and modules elements
    qi.refreshDomObjects = function() {
        qi.bodyOffset = qi.getBodyPosition(); // must update this, as sometimes after finishing page load the position changes, like when dnn adds the toolbar
        if (qi.enableCb)
            qi.contentBlocks = $(selectors.listContainerSelector).find(selectors.contentBlockSelector).add(selectors.listContainerSelector);
        if (qi.enableMod)
            qi.modules = $(selectors.paneSelector).find(selectors.moduleSelector).add(selectors.paneSelector);
    };

    // position, align and show a menu linked to another item
    qi.positionAndAlign = function(element, coords) {
        return element.css({
            'left': coords.x - qi.bodyOffset.x,
            'top': coords.yh - qi.bodyOffset.y,
            'width': coords.element.width()
        }).show();
    };

    // Refresh positioning / visibility of the quick-insert bar
    qi.refresh = function(e) {

        if (!qi.refreshDomObjects.lastCall || (new Date() - qi.refreshDomObjects.lastCall > 1000)) {
            // console.log('refreshed contentblock and modules');
            qi.refreshDomObjects.lastCall = new Date();
            qi.refreshDomObjects();
        }

        if (qi.enableCb && qi.contentBlocks) {
            qi.nearestCb = qi.findNearest(qi.contentBlocks, { x: e.clientX, y: e.clientY }, selectors.contentBlockSelector);
        }

        if (qi.enableMod && qi.modules) {
            qi.nearestMod = qi.findNearest(qi.modules, { x: e.clientX, y: e.clientY }, selectors.moduleSelector);
        }

        qi.modActions.toggleClass("sc-invisible", qi.nearestMod === null);
        qi.cbActions.toggleClass("sc-invisible", qi.nearestCb === null);

        // if previously a parent-pane was highlighted, un-highlight it now
        if (qi.main.parentContainer)
            $(qi.main.parentContainer).removeClass("sc-cb-highlight-for-insert");

        if (qi.nearestCb !== null || qi.nearestMod !== null) {
            var alignTo = qi.nearestCb || qi.nearestMod;

            // find parent pane to highlight
            var parentPane = $(alignTo.element).closest(selectors.paneSelector);
            var parentCbList = $(alignTo.element).closest(selectors.listContainerSelector);
            var parentContainer = (parentCbList.length ? parentCbList : parentPane)[0];

            qi.positionAndAlign(qi.main, alignTo, true);

            // Keep current block as current on menu
            qi.main.actionsForCb = qi.nearestCb ? qi.nearestCb.element : null;
            qi.main.actionsForModule = qi.nearestMod ? qi.nearestMod.element : null;
            qi.main.parentContainer = parentContainer;
            $(parentContainer).addClass("sc-cb-highlight-for-insert");
        } else {
            qi.main.hide();
        }
    };

    // Return the nearest element to the mouse cursor from elements (jQuery elements)
    qi.findNearest = function (elements, position) {
        var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed

        var nearestItem = null;
        var nearestDistance = maxDistance;

        var posX = position.x + qi.win.scrollLeft();
        var posY = position.y + qi.win.scrollTop();

        // Find nearest element
        elements.each(function () {
            var e = qi.getCoordinates($(this));

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
    };

    qi.getCoordinates = function (element) {
        return {
            element: element,
            x: element.offset().left,
            w: element.width(),
            y: element.offset().top,
            // For content-block ITEMS, the menu must be visible at the end
            // For content-block-LISTS, the menu must be at top
            yh: element.offset().top + (element.is(selectors.eitherCbOrMod) ? element.height() : 0)
        };
    };
});