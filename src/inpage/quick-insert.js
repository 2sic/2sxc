$(function() {
    "use strict";
    //var enableModuleMove = false; // not implemented yet
    var selectors = {
        listContainerSelector: ".sc-content-block-list",
        contentBlockClass: "sc-content-block",
        contentBlockSelector: ".sc-content-block",
        moduleSelector: ".DnnModule",
        paneSelector: ".DNNEmptyPane, .dnnDropEmptyPanes, :has(>.DnnModule)", // Found no better way to get all panes - the hidden variable does not exist when not in edit page mode
        listDataAttr: "data-list-context",
        selected: "sc-cb-is-selected"
    };

    var hasContentBlocks = ($(selectors.listContainerSelector).length > 0);
    // the quick-insert object
    var qi = {
        enableCb: hasContentBlocks, // for now, ContentBlocks are only enabled if they exist on the page
        enableMod: !hasContentBlocks,   // if it has inner-content, then it's probably a details page, where quickly adding modules would be a problem, so for now, disable modules in this case
        body: $("body"),
        main: $("<div class='sc-content-block-menu sc-i18n'></div>"),
        template: "<a class='sc-content-block-menu-addcontent sc-invisible' data-type='Default' data-i18n='[title]QuickInsertMenu.AddBlockContent'>content</a>"
            + "<a class='sc-content-block-menu-addapp sc-invisible' data-type='' data-i18n='[title]QuickInsertMenu.AddBlockApp'>app</a>"
            + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-scissors sc-invisible' data-action='cut' data-i18n='[title]QuickInsertMenu.Cut'></a>"
            + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-paste sc-invisible sc-unavailable' data-action='paste' data-i18n='[title]QuickInsertMenu.Paste'></a>"
            + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-trash sc-invisible sc-unavailable' data-action='delete' data-i18n='[title]QuickInsertMenu.Delete'></a>",
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

        // content blocks actions
        if (qi.enableCb)
            qi.main.append(qi.cbActions);

        // module actions
        if (qi.enableMod)
            qi.main.append(qi.modActions);
    };

    qi.init();

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
        {
            return qi.copyPasteInPage(cbAction, list, index);
        }
    });

    qi.copyPasteInPage = function(cbAction, list, index) {
        var listItems = list.find(selectors.contentBlockSelector);
        var actionConfig = JSON.parse(list.attr(selectors.listDataAttr));
        var currentItem = listItems[index];

        // action!
        if (cbAction === "cut") {
            qi.selectCbOrMod(currentItem);
            $2sxc._cbClipboard = { type: "cb", index: index, item: currentItem };
        } else if (cbAction === "paste") {
            var from = $2sxc._cbClipboard.index, to = index;
            if (isNaN(from) || isNaN(to) || from === to || from + 1 === to) // this moves it to the same spot, so ignore
                return qi.unselectAll(); // don't do anything

            $2sxc(list).manage.moveContentBlock(actionConfig.parent, actionConfig.field, from, to);
            qi.unselectAll();
            $2sxc._cbClipboard = null;
        } else if (cbAction === "cancel") // todo: ui not implemented yet
            qi.unselectAll();
        else if (cbAction === "delete") {
            alert("todo not implemented yet");
        }
    };

    qi.selectCbOrMod = function(element) {
        $("." + selectors.selected).removeClass(selectors.selected);
        var cb = $(element);
        cb.addClass(selectors.selected);
        if (cb.prev().is("iframe"))
            cb.prev().addClass(selectors.selected);
        qi.setSecondaryActionsState(true);
    };

    qi.unselectAll = function() {
        $("." + selectors.selected).removeClass(selectors.selected);
        $2sxc._cbClipboard = {};
        qi.setSecondaryActionsState(false);
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

    qi.refresh = function(e) { // ToDo: Performance is not solved with requestAnimationFrame, needs throttling (or more performant selectors etc.)

        if (!qi.refreshDomObjects.lastCall || (new Date() - qi.refreshDomObjects.lastCall > 1000)) {
            console.log('refreshed contentblock and modules');
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

            qi.main.css({
                'left': alignTo.x - qi.bodyOffset.x,
                'top': alignTo.y - qi.bodyOffset.y,
                'width': alignTo.element.width()
            }).show();

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
    qi.findNearest = function(elements, position, useBottomLineSelector) {
        var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed

        var nearest = null;
        var nearestDistance = maxDistance;

        // Find nearest element
        elements.each(function() {
            var element = $(this);
            var x = element.offset().left;
            var w = element.width();
            var y = element.offset().top;
            var h = element.height();

            var posX = position.x + $(window).scrollLeft();
            var posY = position.y + $(window).scrollTop();

            // First check x coordinates - must be within container
            if (posX < x || posX > x + w)
                return;

            // For content-block elements, the menu must be visible at the end
            // For content-block-lists, the menu must be at top
            var cmpHeight = element.is(useBottomLineSelector) ? y + h : y;

            // Check if y coordinates are within boundaries
            var distance = Math.abs(posY - cmpHeight);

            if (distance < maxDistance && distance < nearestDistance) {
                nearest = { x: x, y: cmpHeight, element: element };
                nearestDistance = distance;
            }
        });

        return nearest;
    };
});